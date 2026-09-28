<#
.SYNOPSIS
    Deploy de MeatNet a produccion: publica la API en el VPS y recien despues el frontend en Vercel.

.DESCRIPTION
    Automatiza el procedimiento de docs/infraestructure.md (seccion 6.4). Se corre parado en la rama
    development, con todo commiteado y pusheado, desde la raiz del repo:

        .\tools\deploy.ps1

    Pasos: verifica el estado de git, compila API y web, hace un backup de Supabase, mergea development
    en master, publica la API en el VPS y, solo si la API quedo respondiendo, pushea master (lo que
    dispara el build de Vercel). Si algo falla, corta antes de pushear y el VPS vuelve solo a la
    version anterior.

    Configuracion: tools/deploy.config.json (ver tools/deploy.config.example.json). No se versiona.

.PARAMETER SkipBuild
    No compila localmente. Solo para reintentar un deploy que ya compilo.

.PARAMETER SkipBackup
    No hace el backup de Supabase. Usarlo unicamente si la version no trae migraciones.

.PARAMETER Force
    No pide confirmacion antes de deployar.

.PARAMETER ConfigPath
    Ruta alternativa al archivo de configuracion.
#>
[CmdletBinding()]
param(
    [switch]$SkipBuild,
    [switch]$SkipBackup,
    [switch]$Force,
    [string]$ConfigPath
)

$ErrorActionPreference = 'Stop'

$Repo = Split-Path $PSScriptRoot -Parent
$TarballLocal = Join-Path $env:TEMP 'meatnet-api.tar.gz'
$PublishDir = Join-Path $env:TEMP 'meatnet-publish'
$TarballRemoto = '/tmp/meatnet-api.tar.gz'

# ---------------------------------------------------------------- helpers

function Write-Paso {
    param([string]$Texto)
    Write-Host ''
    Write-Host "==> $Texto" -ForegroundColor Cyan
}

function Write-Aviso {
    param([string]$Texto)
    Write-Host "    ! $Texto" -ForegroundColor Yellow
}

# Corre un ejecutable externo y corta si devuelve un codigo distinto de cero.
# PowerShell no lo hace solo: $ErrorActionPreference no aplica a los .exe.
function Invoke-Externo {
    param(
        [Parameter(Mandatory = $true)][string]$Comando,
        [string[]]$Argumentos = @(),
        [string]$Detalle
    )
    & $Comando @Argumentos
    if ($LASTEXITCODE -ne 0) {
        if (-not $Detalle) { $Detalle = "$Comando $($Argumentos -join ' ')" }
        throw "Fallo: $Detalle (codigo $LASTEXITCODE)"
    }
}

function Get-GitSalida {
    param([Parameter(Mandatory = $true)][string[]]$Argumentos)
    $salida = & git @Argumentos
    if ($LASTEXITCODE -ne 0) { throw "Fallo: git $($Argumentos -join ' ')" }
    return $salida
}

function Read-Config {
    $ruta = $ConfigPath
    if (-not $ruta) { $ruta = Join-Path $PSScriptRoot 'deploy.config.json' }
    if (-not (Test-Path $ruta)) {
        throw "Falta $ruta. Copiar tools\deploy.config.example.json, completarlo y volver a correr."
    }
    $config = Get-Content $ruta -Raw | ConvertFrom-Json
    foreach ($campo in @('SshHost', 'ApiPublicUrl', 'SupabaseHost', 'SupabaseUser', 'BackupDir', 'PgDump')) {
        if (-not $config.$campo) { throw "Falta '$campo' en $ruta." }
    }
    if ($config.SupabaseUser -like '*<project-ref>*') {
        throw "Completar el project-ref de Supabase en $ruta."
    }
    return $config
}

function Read-PasswordSupabase {
    if ($env:MEATNET_DB_PASSWORD) { return $env:MEATNET_DB_PASSWORD }
    $segura = Read-Host 'Password de meatnet en Supabase' -AsSecureString
    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($segura)
    try {
        return [Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr)
    } finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    }
}

# ---------------------------------------------------------------- pasos

function Test-EstadoGit {
    Write-Paso 'Estado de git'

    $rama = (Get-GitSalida @('rev-parse', '--abbrev-ref', 'HEAD')).Trim()
    if ($rama -ne 'development') {
        throw "Estas en '$rama'. El deploy se hace desde development."
    }

    if (Get-GitSalida @('status', '--porcelain')) {
        throw 'Hay cambios sin commitear. Commitear o descartar antes de deployar.'
    }

    Invoke-Externo 'git' @('fetch', '--quiet', 'origin')

    $local = (Get-GitSalida @('rev-parse', 'development')).Trim()
    $remoto = (Get-GitSalida @('rev-parse', 'origin/development')).Trim()
    if ($local -ne $remoto) {
        throw 'development no coincide con origin/development. Pushear (o pullear) antes de deployar.'
    }

    $commits = @(Get-GitSalida @('log', '--oneline', 'origin/master..development'))
    if ($commits.Count -eq 0) {
        throw 'No hay nada para deployar: master ya tiene todo lo de development.'
    }

    Write-Host "    Commits que salen a produccion ($($commits.Count)):"
    $commits | ForEach-Object { Write-Host "      $_" }

    $migraciones = @(Get-GitSalida @('diff', '--name-only', 'origin/master..development', '--',
        'source/api/Meat.Repositories/Migrations'))
    $config = @(Get-GitSalida @('diff', '--name-only', 'origin/master..development', '--',
        'source/api/Meat/appsettings.json', 'source/api/Meat/appsettings.Production.json',
        'source/web/vercel.json', 'source/web/package.json'))

    $hayMigraciones = $migraciones.Count -gt 0
    if ($hayMigraciones) {
        Write-Aviso "La version trae migraciones ($($migraciones.Count) archivos). Se aplican solas al arrancar la API."
    }
    if ($config.Count -gt 0) {
        Write-Aviso 'Cambio configuracion. Revisar si hace falta una variable nueva:'
        $config | ForEach-Object { Write-Host "      $_" }
        Write-Aviso 'De la API: /etc/meatnet/api.env en el VPS. Del frontend (VITE_): en Vercel, antes del push.'
    }

    return $hayMigraciones
}

# El build de la web necesita Node 20 (Tailwind 4 lo exige, y con una version menor npm ni siquiera
# instala su binario nativo: vite build falla recien al final, sin decir que el problema es Node).
function Test-Node {
    $version = & node --version
    if ($LASTEXITCODE -ne 0 -or -not $version) { throw 'No se encontro node en el PATH.' }
    $mayor = [int]($version.TrimStart('v').Split('.')[0])
    if ($mayor -lt 20) {
        throw "El build de la web necesita Node 20 o mayor y esta activo $version. Cambiar con 'nvm use 20' y volver a correr el script."
    }
    Write-Host "    Node $version"
}

# npm ci empieza por borrar node_modules y en Windows falla con EPERM (-4048) si el dev server de
# Vite tiene tomados los archivos, dejandolo ademas a medio borrar. Se chequea antes de tocar nada.
function Test-DevServer {
    try {
        $enUso = Get-NetTCPConnection -State Listen -LocalPort 5173 -ErrorAction SilentlyContinue
    } catch {
        return
    }
    if ($enUso) {
        throw 'Hay un dev server escuchando en el puerto 5173. Cerralo (Ctrl+C en la consola del npm run dev) y volve a correr el script.'
    }
}

function Invoke-Compilacion {
    Write-Paso 'Compilando API y web'
    Invoke-Externo 'dotnet' @('build', (Join-Path $Repo 'source\api\Meat.sln'), '-c', 'Release', '--nologo', '-tl:off', '-v', 'q')

    Push-Location (Join-Path $Repo 'source\web')
    try {
        # Vercel corre "tsc -b": un error de tipos que npm run dev tolera hace fallar el deploy alla.
        Write-Host '    npm ci (reinstala node_modules; tarda unos minutos y no imprime nada)'
        Invoke-Externo 'npm' @('ci', '--silent') -Detalle 'npm ci'
        Invoke-Externo 'npm' @('run', 'build') -Detalle 'npm run build'
    } catch {
        if ("$_" -match '-4048') {
            throw ('npm ci no pudo borrar node_modules (EPERM). Cerra el dev server o cualquier ' +
                   'programa que este usando source\web\node_modules, y volve a correr el script.')
        }
        throw
    } finally {
        Pop-Location
    }
}

function Invoke-Backup {
    param([Parameter(Mandatory = $true)]$Config)

    Write-Paso 'Backup de la base en Supabase'
    if (-not (Test-Path $Config.PgDump)) {
        throw "No se encontro pg_dump en $($Config.PgDump). Corregir 'PgDump' en la configuracion."
    }

    New-Item -ItemType Directory -Force $Config.BackupDir | Out-Null
    $archivo = Join-Path $Config.BackupDir ("meatnet-prod-{0}.dump" -f (Get-Date -Format 'yyyyMMdd-HHmm'))

    $env:PGPASSWORD = Read-PasswordSupabase
    $env:PGSSLMODE = 'require'
    try {
        # Ojo: Supabase bloquea la IP 30 minutos tras 2 passwords incorrectas seguidas.
        Invoke-Externo $Config.PgDump @(
            '-h', $Config.SupabaseHost, '-p', '5432', '-U', $Config.SupabaseUser,
            '-d', 'postgres', '-n', 'meat', '-Fc', '-f', $archivo
        ) -Detalle 'pg_dump contra Supabase'
    } catch {
        # pg_dump crea el archivo antes de conectarse: si fallo, queda uno vacio que parece un backup.
        if ((Test-Path $archivo) -and (Get-Item $archivo).Length -eq 0) { Remove-Item -Force $archivo }
        throw
    } finally {
        Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
        Remove-Item Env:PGSSLMODE -ErrorAction SilentlyContinue
    }

    $bytes = (Get-Item $archivo).Length
    if ($bytes -eq 0) {
        Remove-Item -Force $archivo
        throw 'El backup salio vacio. No se sigue sin un backup valido.'
    }
    Write-Host ("    $archivo ({0} MB)" -f [math]::Round($bytes / 1MB, 1))

    # Recien despues de tener el backup nuevo se borran los viejos, y solo los que genero el script.
    $conservar = 5
    if ($Config.BackupsAConservar) { $conservar = [int]$Config.BackupsAConservar }
    $viejos = @(Get-ChildItem -Path $Config.BackupDir -Filter 'meatnet-prod-*.dump' -File |
        Sort-Object LastWriteTime -Descending | Select-Object -Skip $conservar)
    if ($viejos.Count -gt 0) {
        $viejos | Remove-Item -Force
        Write-Host "    Borrados $($viejos.Count) backups viejos (se conservan los ultimos $conservar)"
    }
}

function Invoke-MergeAMaster {
    Write-Paso 'Merge de development en master (local, sin pushear)'
    Invoke-Externo 'git' @('checkout', '--quiet', 'master')
    try {
        Invoke-Externo 'git' @('pull', '--quiet', '--ff-only', 'origin', 'master')
        Invoke-Externo 'git' @('merge', '--ff-only', 'development')
    } catch {
        Invoke-Externo 'git' @('checkout', '--quiet', 'development')
        throw ("No se pudo mergear a master: {0}`n" -f $_.Exception.Message) +
              'Si master tiene commits propios (un hotfix), mergealo a mano y volve a correr el script.'
    }
}

function Publish-Api {
    param([Parameter(Mandatory = $true)]$Config)

    Write-Paso 'Publicando la API'
    # La carpeta tiene que quedar vacia: dotnet publish no borra archivos de una version anterior.
    if (Test-Path $PublishDir) { Remove-Item -Recurse -Force $PublishDir }
    Invoke-Externo 'dotnet' @('publish', (Join-Path $Repo 'source\api\Meat\Meat.csproj'),
        '-c', 'Release', '-o', $PublishDir, '--nologo', '-tl:off', '-v', 'q')

    if (Test-Path $TarballLocal) { Remove-Item -Force $TarballLocal }
    Invoke-Externo 'tar' @('-czf', $TarballLocal, '--exclude=./appsettings.Development.json', '-C', $PublishDir, '.')

    $mb = [math]::Round((Get-Item $TarballLocal).Length / 1MB, 1)
    Write-Host "    $TarballLocal ($mb MB)"

    Write-Paso "Copiando al VPS ($($Config.SshHost))"
    Invoke-Externo 'scp' @('-q', $TarballLocal, "$($Config.SshHost):$TarballRemoto") -Detalle 'scp al VPS'

    Write-Paso 'Intercambiando la version y arrancando el servicio'
    # El script del VPS deja la version anterior en app.old y vuelve solo a ella si la nueva no levanta.
    Invoke-Externo 'ssh' @($Config.SshHost, 'sudo /usr/local/sbin/meatnet-deploy') -Detalle 'meatnet-deploy en el VPS'
}

function Test-ApiPublica {
    param([Parameter(Mandatory = $true)]$Config)

    Write-Paso 'Verificando la API desde afuera'
    $url = "$($Config.ApiPublicUrl.TrimEnd('/'))/Usuarios"
    $codigo = 0
    try {
        $respuesta = Invoke-WebRequest -Uri $url -Method Get -UseBasicParsing -TimeoutSec 20
        $codigo = [int]$respuesta.StatusCode
    } catch [System.Net.WebException] {
        if ($_.Exception.Response) { $codigo = [int]$_.Exception.Response.StatusCode }
    }

    # Sin token, cualquier endpoint que no sea el login tiene que dar 401.
    if ($codigo -ne 401) {
        throw "$url devolvio $codigo y se esperaba 401. La API no quedo bien publicada; no se pushea master."
    }
    Write-Host "    $url -> 401 (correcto)"
}

function Publish-Frontend {
    Write-Paso 'Pusheando master (dispara el build de Vercel)'
    Invoke-Externo 'git' @('push', 'origin', 'master')
    Invoke-Externo 'git' @('checkout', '--quiet', 'development')
}

# ---------------------------------------------------------------- main

Push-Location $Repo
try {
    $config = Read-Config
    if (-not $SkipBuild) {
        Write-Paso 'Entorno local'
        Test-Node
        Test-DevServer
    }
    $hayMigraciones = Test-EstadoGit

    if ($SkipBackup -and $hayMigraciones) {
        Write-Aviso 'La version trae migraciones y pediste saltear el backup.'
    }

    if (-not $Force) {
        Write-Host ''
        $respuesta = Read-Host 'Deployar a produccion? (s/N)'
        if ($respuesta -notin @('s', 'S', 'si', 'SI', 'Si')) {
            Write-Host 'Cancelado.' -ForegroundColor Yellow
            return
        }
    }

    if (-not $SkipBuild) { Invoke-Compilacion } else { Write-Aviso 'Sin compilar (-SkipBuild).' }
    if (-not $SkipBackup) { Invoke-Backup $config } else { Write-Aviso 'Sin backup (-SkipBackup).' }

    Invoke-MergeAMaster
    Publish-Api $config
    Test-ApiPublica $config
    Publish-Frontend

    Write-Host ''
    Write-Host 'API publicada y master pusheado.' -ForegroundColor Green
    Write-Host 'Falta a mano: esperar el estado Ready en Vercel y probar https://meatnet.vercel.app con Ctrl+F5.'
    if ($hayMigraciones) {
        Write-Host 'Confirmar las migraciones en Supabase: select "MigrationId" from meat."__EFMigrationsHistory" order by 1;'
    }
} catch {
    Write-Host ''
    Write-Host "DEPLOY INTERRUMPIDO: $($_.Exception.Message)" -ForegroundColor Red
    $ramaActual = (& git rev-parse --abbrev-ref HEAD 2>$null)
    if ($ramaActual -eq 'master') {
        Write-Host 'Quedaste en master con el merge hecho pero SIN pushear: produccion sigue como estaba.' -ForegroundColor Yellow
        Write-Host 'Volve con "git checkout development". Corregi y volve a correr el script: repite el merge sin problema.' -ForegroundColor Yellow
    }
    exit 1
} finally {
    Pop-Location
}
