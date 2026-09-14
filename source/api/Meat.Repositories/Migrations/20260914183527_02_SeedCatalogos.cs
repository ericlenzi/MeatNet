using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _02_SeedCatalogos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Especies
            migrationBuilder.Sql(@"
INSERT INTO meat.""Especies"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"", ""RindeMaximo"", ""RindeMinimo"", ""MermaOreoReferencia"")
VALUES
    ('A', 'AVES', false, NULL, NULL, NULL, NULL),
    ('C', 'CAPRINOS', false, NULL, NULL, NULL, NULL),
    ('E', 'EQUINOS', false, NULL, NULL, NULL, NULL),
    ('P', 'PORCINOS', true, NULL, 85.0, 65.0, 2.0),
    ('U', 'BUBALINOS', false, NULL, NULL, NULL, NULL),
    ('V', 'VACUNO', true, NULL, 65.0, 45.0, 2.0)
ON CONFLICT DO NOTHING;");

            // OrigenesHaciendas
            migrationBuilder.Sql(@"
INSERT INTO meat.""OrigenesHaciendas"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('DI', 'DIRECTA CON INTERVENCIÓN', true, NULL),
    ('EE', 'ESTANCIA', true, NULL),
    ('ME', 'MERCADO', true, NULL),
    ('PP', 'PRODUCCION PROPIA', true, NULL)
ON CONFLICT DO NOTHING;");

            // Provincias
            migrationBuilder.Sql(@"
INSERT INTO meat.""Provincias"" (""Id"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    (0, 'CIUDAD AUTONOMA BUENOS AIRES', true, NULL),
    (1, 'BUENOS AIRES', true, NULL),
    (2, 'CATAMARCA', true, NULL),
    (3, 'CORDOBA', true, NULL),
    (4, 'CORRIENTES', true, NULL),
    (5, 'ENTRE RIOS', true, NULL),
    (6, 'JUJUY', true, NULL),
    (7, 'MENDOZA', true, NULL),
    (8, 'LA RIOJA', true, NULL),
    (9, 'SALTA', true, NULL),
    (10, 'SAN JUAN', true, NULL),
    (11, 'SAN LUIS', true, NULL),
    (12, 'SANTA FE', true, NULL),
    (13, 'SANTIAGO DEL ESTERO', true, NULL),
    (14, 'TUCUMAN', true, NULL),
    (16, 'CHACO', true, NULL),
    (17, 'CHUBUT', true, NULL),
    (18, 'FORMOSA', true, NULL),
    (19, 'MISIONES', true, NULL),
    (20, 'NEUQUEN', true, NULL),
    (21, 'LA PAMPA', true, NULL),
    (22, 'RIO NEGRO', true, NULL),
    (23, 'SANTA CRUZ', true, NULL),
    (24, 'TIERRA DEL FUEGO', true, NULL)
ON CONFLICT DO NOTHING;");

            // Roles
            migrationBuilder.Sql(@"
INSERT INTO meat.""Roles"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('ABAST', 'OPERADOR ABASTECIMIENTO', true, NULL),
    ('ABASTADMIN', 'ADMIN ABASTECIMIENTO', true, NULL),
    ('ADMIN', 'ADMINISTRADOR', true, NULL),
    ('FAENA', 'OPERADOR FAENA', true, NULL),
    ('FAENAADMIN', 'ADMIN FAENA', true, NULL),
    ('SUPERADMIN', 'SUPER ADMINISTRADOR', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposAlmacenes
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposAlmacenes"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"", ""Familia"")
VALUES
    ('CAMARA_FAENA', 'Camara de Faena', true, NULL, 'CAMARA'),
    ('CORRAL_CAIDOS', 'Corral de Caidos', true, NULL, 'CORRAL'),
    ('CORRAL_COMUN', 'Corral Comun', true, NULL, 'CORRAL'),
    ('CORRAL_MUERTOS', 'Corral de Muertos', true, NULL, 'CORRAL')
ON CONFLICT DO NOTHING;");

            // TiposClientes
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposClientes"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('E', 'EXTERNO', true, NULL),
    ('P', 'PROPIO', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposEmpresas
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposEmpresas"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('AD', 'ADMINISTRACION', true, NULL),
    ('FA', 'FAENADORA', true, NULL),
    ('FG', 'FRIGORIFICO', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposEstadosHacienda
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposEstadosHacienda"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('CAIDOS', 'Caidos', true, NULL),
    ('EN_PIE', 'En Pie', true, NULL),
    ('MUERTOS', 'Muertos', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposEstadosIngresos
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposEstadosIngresos"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('ANULADO', 'Anulado', true, NULL),
    ('APROBADO', 'Aprobado', true, NULL),
    ('BORRADOR', 'Borrador', true, NULL),
    ('PENDIENTE', 'Pendiente Aprobacion', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposEstadosListasMatanzas
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposEstadosListasMatanzas"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('ANULADA', 'Anulada', true, NULL),
    ('BORRADOR', 'Borrador', true, NULL),
    ('CONFIRMADA', 'Confirmada', true, NULL),
    ('EN_EJECUCION', 'En Ejecucion', true, NULL),
    ('FINALIZADA', 'Finalizada', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposEstadosTropas
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposEstadosTropas"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('ANULADA', 'Anulada', true, NULL),
    ('FAENADA', 'Faenada', true, NULL),
    ('RECEPCIONADA', 'Recepcionada', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposMagnitudes
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposMagnitudes"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('PESO', 'Peso', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposMateriales
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposMateriales"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('CUARTO', 'CUARTO', true, NULL),
    ('DECOM', 'DECOMISO', false, NULL),
    ('MEDIA_RES', 'MEDIA RES', true, NULL),
    ('MENUD', 'MENUDENCIA', false, NULL),
    ('RES', 'RES', true, NULL),
    ('SUB_PROD', 'SUBPRODUCTO', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposMediciones
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposMediciones"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('A', 'AUTOMATICA', true, NULL),
    ('B', 'BALANZA', true, NULL),
    ('M', 'MANUAL', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposMovimientosCamaras
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposMovimientosCamaras"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('EGRESO', 'Egreso a Ciclo II', true, NULL),
    ('INGRESO', 'Ingreso por liberacion', true, NULL),
    ('TRANSF_ALTA', 'Alta por transformacion', true, NULL),
    ('TRANSF_BAJA', 'Baja por transformacion', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposPuestos
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposPuestos"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('PAL', 'PALCO DE TIPIFICACION', true, NULL),
    ('PALCO-P', 'PALCO FAENA PORCINO', true, '2026-09-11 12:46:47.150841'),
    ('PALCO-V', 'PALCO FAENA VACUNO', true, '2026-09-11 12:46:47.150841')
ON CONFLICT DO NOTHING;");

            // TiposSexos
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposSexos"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('H', 'HEMBRA', true, NULL),
    ('M', 'MACHO', true, NULL)
ON CONFLICT DO NOTHING;");

            // UnidadesMedidas
            migrationBuilder.Sql(@"
INSERT INTO meat.""UnidadesMedidas"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('KG', 'KILOGRAMO', true, NULL),
    ('UN', 'UNIDADES', true, NULL)
ON CONFLICT DO NOTHING;");

            // UsosHaciendas
            migrationBuilder.Sql(@"
INSERT INTO meat.""UsosHaciendas"" (""Codigo"", ""Nombre"", ""Activo"", ""FechaBaja"")
VALUES
    ('E', 'EXPORTACION', true, NULL),
    ('F', 'FAENA CONSUMO INTERNO', true, NULL)
ON CONFLICT DO NOTHING;");

            // Conformaciones
            migrationBuilder.Sql(@"
INSERT INTO meat.""Conformaciones"" (""Codigo"", ""Nombre"", ""EspecieId"", ""Orden"", ""Activo"", ""FechaBaja"")
VALUES
    ('A', 'SUPERIOR', 'V', 1, true, NULL),
    ('B', 'BUENA', 'V', 2, true, NULL),
    ('C', 'INTERMEDIA', 'V', 3, true, NULL),
    ('D', 'DEFICIENTE', 'V', 4, true, NULL),
    ('E', 'INFERIOR', 'V', 5, true, NULL)
ON CONFLICT DO NOTHING;");

            // Denticiones
            migrationBuilder.Sql(@"
INSERT INTO meat.""Denticiones"" (""Codigo"", ""Nombre"", ""EspecieId"", ""Activo"", ""FechaBaja"", ""Orden"")
VALUES
    ('D0', 'DIENTE DE LECHE', 'V', true, NULL, 0),
    ('D2', 'DOS DIENTES', 'V', true, NULL, 1),
    ('D4', 'CUATRO DIENTES', 'V', true, NULL, 2),
    ('D6', 'SEIS DIENTES', 'V', true, NULL, 3),
    ('D8', 'BOCA LLENA', 'V', true, NULL, 4)
ON CONFLICT DO NOTHING;");

            // Empresas
            migrationBuilder.Sql(@"
INSERT INTO meat.""Empresas"" (""Nombre"", ""TipoEmpresaId"", ""NumeroCuit"", ""NumeroIngresosBrutos"", ""NumeroInscripcionRuca"", ""CodigoActividad"", ""Activo"", ""ERP_Codigo"", ""FechaActualizacion"", ""FechaBaja"", ""Id"", ""Color"", ""Logo"")
VALUES
    ('Administración', 'AD', NULL, NULL, NULL, NULL, true, NULL, '2021-01-01 00:00:00', NULL, '0', NULL, NULL)
ON CONFLICT DO NOTHING;");

            // GradosEngrasamiento
            migrationBuilder.Sql(@"
INSERT INTO meat.""GradosEngrasamiento"" (""Codigo"", ""Nombre"", ""EspecieId"", ""Orden"", ""Activo"", ""FechaBaja"")
VALUES
    ('0', 'SIN GRASA', 'V', 0, true, NULL),
    ('1', 'ESCASO', 'V', 1, true, NULL),
    ('2', 'ADECUADO', 'V', 2, true, NULL),
    ('3', 'ABUNDANTE', 'V', 3, true, NULL),
    ('4', 'EXCESIVO', 'V', 4, true, NULL)
ON CONFLICT DO NOTHING;");

            // MotivosDecomisos
            migrationBuilder.Sql(@"
INSERT INTO meat.""MotivosDecomisos"" (""Codigo"", ""Nombre"", ""EspecieId"", ""Activo"", ""FechaBaja"", ""Orden"", ""ExigeContusion"")
VALUES
    ('P-ABS', 'Abscesos', 'P', true, NULL, 2, false),
    ('P-ADH', 'Adherencias', 'P', true, NULL, 3, false),
    ('P-ART', 'Artritis', 'P', true, NULL, 6, false),
    ('P-CAQ', 'Caquexia', 'P', true, NULL, 12, false),
    ('P-CIST', 'Cisticercosis', 'P', true, NULL, 13, false),
    ('P-CONT', 'Contusiones', 'P', true, NULL, 1, true),
    ('P-CONTAM', 'Contaminacion', 'P', true, NULL, 7, false),
    ('P-DERM', 'Dermatitis y sarna', 'P', true, NULL, 10, false),
    ('P-ERIS', 'Erisipela', 'P', true, NULL, 9, false),
    ('P-ICT', 'Ictericia', 'P', true, NULL, 11, false),
    ('P-MSAN', 'Mala sangria', 'P', true, NULL, 8, false),
    ('P-NEUM', 'Neumonia', 'P', true, NULL, 5, false),
    ('P-OTR', 'Otros', 'P', true, NULL, 99, false),
    ('P-PERI', 'Pericarditis y pleuritis', 'P', true, NULL, 4, false),
    ('P-SEPT', 'Septicemia', 'P', true, NULL, 15, false),
    ('P-TBC', 'Tuberculosis', 'P', true, NULL, 14, false),
    ('V-ABS', 'Abscesos', 'V', true, NULL, 2, false),
    ('V-ADH', 'Adherencias', 'V', true, NULL, 3, false),
    ('V-CAQ', 'Caquexia', 'V', true, NULL, 7, false),
    ('V-CIST', 'Cisticercosis', 'V', true, NULL, 9, false),
    ('V-CONT', 'Contusiones', 'V', true, NULL, 1, true),
    ('V-CONTAM', 'Contaminacion', 'V', true, NULL, 4, false),
    ('V-HID', 'Hidatidosis', 'V', true, NULL, 10, false),
    ('V-ICT', 'Ictericia', 'V', true, NULL, 6, false),
    ('V-MSAN', 'Mala sangria', 'V', true, NULL, 5, false),
    ('V-OTR', 'Otros', 'V', true, NULL, 99, false),
    ('V-SEPT', 'Septicemia', 'V', true, NULL, 11, false),
    ('V-TBC', 'Tuberculosis', 'V', true, NULL, 8, false)
ON CONFLICT DO NOTHING;");

            // TipificacionesOficiales
            migrationBuilder.Sql(@"
INSERT INTO meat.""TipificacionesOficiales"" (""Codigo"", ""Nombre"", ""EspecieId"", ""Activo"", ""FechaBaja"")
VALUES
    ('CAP', 'CAPONES', 'P', true, NULL),
    ('CHA', 'CHANCHAS', 'P', true, NULL),
    ('MEI', 'MACHO ENTERO JOVEN', 'P', true, NULL),
    ('NOV', 'NOVILLO', 'V', true, NULL),
    ('PAD', 'PADRILLO', 'P', true, NULL),
    ('TOR', 'TORO', 'V', true, NULL),
    ('VAC', 'VACA', 'V', true, NULL),
    ('VQU', 'VAQUILLONA', 'V', true, NULL)
ON CONFLICT DO NOTHING;");

            // TiposContusiones
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposContusiones"" (""Codigo"", ""Nombre"", ""EspecieId"", ""Activo"", ""FechaBaja"", ""Orden"")
VALUES
    ('GRA', 'GRAVE', 'V', true, NULL, 3),
    ('LEV', 'LEVE', 'V', true, NULL, 1),
    ('MOD', 'MODERADA', 'V', true, NULL, 2),
    ('SC', 'SIN CONTUSION', 'V', true, NULL, 0)
ON CONFLICT DO NOTHING;");

            // TiposEspecies
            migrationBuilder.Sql(@"
INSERT INTO meat.""TiposEspecies"" (""Codigo"", ""Nombre"", ""EspecieId"", ""TipoSexoId"", ""Activo"", ""FechaBaja"", ""PesoTeoricoReferencia"")
VALUES
    ('CA', 'CAPONES / HEMBRAS SIN SERVICIO', 'P', 'M', true, NULL, 90.0),
    ('CH', 'CHANCHAS', 'P', 'H', true, NULL, 100.0),
    ('CO', 'CACHORRO', 'P', 'M', true, NULL, 85.0),
    ('HA', 'CACHORRA', 'P', 'H', true, NULL, 85.0),
    ('LE', 'LECHONES', 'P', 'M', true, NULL, 120.0),
    ('ME', 'MACHO ENTERO INMUNOLOGICAMENTE CASTRADO', 'P', 'M', true, NULL, 110.0),
    ('MJ', 'MEJ', 'V', 'M', true, NULL, 90.0),
    ('NO', 'NOVILLO', 'V', 'M', true, NULL, 90.0),
    ('NT', 'NOVILLITO', 'V', 'M', true, NULL, 85.0),
    ('PA', 'PADRILLOS', 'P', 'M', true, NULL, 100.0),
    ('TO', 'TORO', 'V', 'M', true, NULL, 94.0),
    ('VA', 'VACA', 'V', 'H', true, NULL, 92.0),
    ('VQ', 'VAQUILLONA', 'V', 'H', true, NULL, 98.0)
ON CONFLICT DO NOTHING;");

            // Usuarios
            migrationBuilder.Sql(@"
INSERT INTO meat.""Usuarios"" (""Id"", ""Nombre"", ""Apellido"", ""Email"", ""UserName"", ""PasswordHash"", ""FechaActualizacion"", ""Activo"", ""FechaBaja"", ""Legajo"", ""RolId"", ""EmpresaId"")
VALUES
    ('68FEE0F7-8F29-44F7-A0BA-153E31763568', 'Super', 'Administrador', NULL, 'superadmin', 'PBKDF2$210000$hfu+48YHPffNHVAo1eAkXA==$yZhyz3fGl69IbIvFB916HH0Ls0qcrJJpNPCx6WagXnw=', '2026-09-08 22:25:31.690000', true, NULL, NULL, 'SUPERADMIN', '0')
ON CONFLICT DO NOTHING;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM meat.""Usuarios"" WHERE ""Id"" IN ('68FEE0F7-8F29-44F7-A0BA-153E31763568');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposEspecies"" WHERE ""Codigo"" IN ('CA', 'CH', 'CO', 'HA', 'LE', 'ME', 'MJ', 'NO', 'NT', 'PA', 'TO', 'VA', 'VQ');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposContusiones"" WHERE ""Codigo"" IN ('GRA', 'LEV', 'MOD', 'SC');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TipificacionesOficiales"" WHERE ""Codigo"" IN ('CAP', 'CHA', 'MEI', 'NOV', 'PAD', 'TOR', 'VAC', 'VQU');");
            migrationBuilder.Sql(@"DELETE FROM meat.""MotivosDecomisos"" WHERE ""Codigo"" IN ('P-ABS', 'P-ADH', 'P-ART', 'P-CAQ', 'P-CIST', 'P-CONT', 'P-CONTAM', 'P-DERM', 'P-ERIS', 'P-ICT', 'P-MSAN', 'P-NEUM', 'P-OTR', 'P-PERI', 'P-SEPT', 'P-TBC', 'V-ABS', 'V-ADH', 'V-CAQ', 'V-CIST', 'V-CONT', 'V-CONTAM', 'V-HID', 'V-ICT', 'V-MSAN', 'V-OTR', 'V-SEPT', 'V-TBC');");
            migrationBuilder.Sql(@"DELETE FROM meat.""GradosEngrasamiento"" WHERE ""Codigo"" IN ('0', '1', '2', '3', '4');");
            migrationBuilder.Sql(@"DELETE FROM meat.""Empresas"" WHERE ""Id"" IN ('0');");
            migrationBuilder.Sql(@"DELETE FROM meat.""Denticiones"" WHERE ""Codigo"" IN ('D0', 'D2', 'D4', 'D6', 'D8');");
            migrationBuilder.Sql(@"DELETE FROM meat.""Conformaciones"" WHERE ""Codigo"" IN ('A', 'B', 'C', 'D', 'E');");
            migrationBuilder.Sql(@"DELETE FROM meat.""UsosHaciendas"" WHERE ""Codigo"" IN ('E', 'F');");
            migrationBuilder.Sql(@"DELETE FROM meat.""UnidadesMedidas"" WHERE ""Codigo"" IN ('KG', 'UN');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposSexos"" WHERE ""Codigo"" IN ('H', 'M');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposPuestos"" WHERE ""Codigo"" IN ('PAL', 'PALCO-P', 'PALCO-V');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposMovimientosCamaras"" WHERE ""Codigo"" IN ('EGRESO', 'INGRESO', 'TRANSF_ALTA', 'TRANSF_BAJA');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposMediciones"" WHERE ""Codigo"" IN ('A', 'B', 'M');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposMateriales"" WHERE ""Codigo"" IN ('CUARTO', 'DECOM', 'MEDIA_RES', 'MENUD', 'RES', 'SUB_PROD');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposMagnitudes"" WHERE ""Codigo"" IN ('PESO');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposEstadosTropas"" WHERE ""Codigo"" IN ('ANULADA', 'FAENADA', 'RECEPCIONADA');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposEstadosListasMatanzas"" WHERE ""Codigo"" IN ('ANULADA', 'BORRADOR', 'CONFIRMADA', 'EN_EJECUCION', 'FINALIZADA');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposEstadosIngresos"" WHERE ""Codigo"" IN ('ANULADO', 'APROBADO', 'BORRADOR', 'PENDIENTE');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposEstadosHacienda"" WHERE ""Codigo"" IN ('CAIDOS', 'EN_PIE', 'MUERTOS');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposEmpresas"" WHERE ""Codigo"" IN ('AD', 'FA', 'FG');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposClientes"" WHERE ""Codigo"" IN ('E', 'P');");
            migrationBuilder.Sql(@"DELETE FROM meat.""TiposAlmacenes"" WHERE ""Codigo"" IN ('CAMARA_FAENA', 'CORRAL_CAIDOS', 'CORRAL_COMUN', 'CORRAL_MUERTOS');");
            migrationBuilder.Sql(@"DELETE FROM meat.""Roles"" WHERE ""Codigo"" IN ('ABAST', 'ABASTADMIN', 'ADMIN', 'FAENA', 'FAENAADMIN', 'SUPERADMIN');");
            migrationBuilder.Sql(@"DELETE FROM meat.""Provincias"" WHERE ""Id"" IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 16, 17, 18, 19, 20, 21, 22, 23, 24);");
            migrationBuilder.Sql(@"DELETE FROM meat.""OrigenesHaciendas"" WHERE ""Codigo"" IN ('DI', 'EE', 'ME', 'PP');");
            migrationBuilder.Sql(@"DELETE FROM meat.""Especies"" WHERE ""Codigo"" IN ('A', 'C', 'E', 'P', 'U', 'V');");
        }
    }
}
