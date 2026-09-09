using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Meat.Application.Empresas.Seed
{
    /// <summary>
    /// Deja operable a una empresa recien creada.
    ///
    /// Dos cosas la hacen usable: el master data del rubro (destinos, categorias, unidades de
    /// faena) y una estructura minima con la que alguien pueda entrar. Sin esto ultimo la
    /// empresa queda inalcanzable: crear su primera sucursal requiere estar parado en ella, y
    /// el filtro por empresa lo impide.
    ///
    /// Cada fila lleva su EmpresaId explicito. El MeatContext solo lo completa cuando viene
    /// vacio, asi que las filas caen en la empresa nueva y no en la del SUPERADMIN que la crea.
    /// </summary>
    public class EmpresaSeeder
    {
        private readonly MeatContext context;

        /// <summary>Codigos de la estructura inicial. Son un punto de partida editable.</summary>
        private const string SucursalInicial = "100";
        private const string EstablecimientoInicial = "100";
        private const string UsuarioInicialSufijo = "admin";

        public EmpresaSeeder(MeatContext context)
        {
            this.context = context;
        }

        public void Sembrar(string empresaId, string nombreEmpresa)
        {
            var baseDatos = Cargar();

            foreach (var p in baseDatos.Parametros)
            {
                var parametro = Domain.Parametros.ParametroFactory.Create();
                parametro.EmpresaId = empresaId;
                parametro.Codigo = p.Codigo;
                parametro.Nombre = p.Nombre;
                parametro.Valor = p.Valor;
                this.context.Parametros.Add(parametro);
            }

            foreach (var d in baseDatos.DestinosComerciales)
            {
                var destino = Domain.DestinosComerciales.DestinoComercialFactory.Create();
                destino.EmpresaId = empresaId;
                destino.Codigo = d.Codigo;
                destino.Nombre = d.Nombre;
                destino.Favorito = d.Favorito;
                this.context.DestinosComerciales.Add(destino);
            }

            foreach (var te in baseDatos.TiposEspecies)
            {
                var tipo = Domain.TiposEspecies.TipoEspecieFactory.Create();
                tipo.EmpresaId = empresaId;
                tipo.Codigo = te.Codigo;
                tipo.Nombre = te.Nombre;
                tipo.EspecieId = te.EspecieId;
                tipo.TipoSexoId = te.TipoSexoId;
                tipo.PesoTeorico = te.PesoTeorico;
                this.context.TiposEspecies.Add(tipo);
            }

            foreach (var uf in baseDatos.UnidadesFaenas)
            {
                var unidad = Domain.UnidadesFaenas.UnidadFaenaFactory.Create();
                unidad.EmpresaId = empresaId;
                unidad.Codigo = uf.Codigo;
                unidad.Nombre = uf.Nombre;
                unidad.EspecieId = uf.EspecieId;
                unidad.CantidadCuartos = uf.CantidadCuartos;
                unidad.PiezasPorAnimal = uf.PiezasPorAnimal;
                unidad.PorDefecto = uf.PorDefecto;
                unidad.TipoMaterialId = uf.TipoMaterialId;
                this.context.UnidadesFaenas.Add(unidad);
            }

            SembrarEstructura(empresaId, nombreEmpresa);
        }

        /// <summary>
        /// Sucursal, establecimiento y un usuario ADMIN: lo minimo para que alguien pueda
        /// entrar a la empresa nueva y terminar de configurarla desde la aplicacion.
        /// </summary>
        private void SembrarEstructura(string empresaId, string nombreEmpresa)
        {
            var sucursal = Domain.Sucursales.SucursalFactory.Create();
            sucursal.EmpresaId = empresaId;
            sucursal.CodigoSucursal = SucursalInicial;
            sucursal.Nombre = "Casa Central";
            sucursal.Activo = true;
            this.context.Sucursales.Add(sucursal);

            var establecimiento = new Domain.Establecimientos.Establecimiento
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                SucursalId = sucursal.Id,
                CodigoEstablecimiento = EstablecimientoInicial,
                Nombre = "Planta Principal",
                Activo = true,
                FechaActualizacion = DateTime.Now,
            };
            this.context.Establecimientos.Add(establecimiento);

            // El UserName es unico en todo el sistema, no por empresa, asi que lleva el codigo
            // de la empresa adelante para no chocar con el admin de otra.
            var usuario = Domain.Usuarios.UsuarioFactory.Create(
                userName: $"{empresaId}.{UsuarioInicialSufijo}".ToLowerInvariant(),
                passwordHash: PasswordHash.Calcular(ContrasenaInicial()),
                nombre: "Administrador",
                apellido: nombreEmpresa,
                email: null,
                legajo: null,
                rolId: "ADMIN",
                activo: true);
            usuario.EmpresaId = empresaId;
            this.context.Usuarios.Add(usuario);

            this.context.UsuariosSucursales.Add(new Domain.UsuariosSucursales.UsuarioSucursal
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                UsuarioId = usuario.Id,
                SucursalId = sucursal.Id,
                EsMain = true,
                FechaActualizacion = DateTime.Now,
            });

            this.context.UsuariosEstablecimientos.Add(new Domain.UsuariosEstablecimientos.UsuarioEstablecimiento
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                UsuarioId = usuario.Id,
                EstablecimientoId = establecimiento.Id,
                EsMain = true,
                FechaActualizacion = DateTime.Now,
            });
        }

        /// <summary>La misma que queda en el parametro PASSWORD_INICIAL del seed.</summary>
        private string ContrasenaInicial()
        {
            var baseDatos = Cargar();
            var p = baseDatos.Parametros.Find(x => x.Codigo == "PASSWORD_INICIAL");
            return p?.Valor ?? "inicio";
        }

        private static EmpresaBase cache;

        private static EmpresaBase Cargar()
        {
            if (cache != null)
                return cache;

            var ruta = Path.Combine(AppContext.BaseDirectory, "Empresas", "Seed", "empresa-base.json");
            if (!File.Exists(ruta))
                throw new ValidationException(
                    "Falta el archivo con el master data base de una empresa nueva (empresa-base.json).");

            cache = JsonSerializer.Deserialize<EmpresaBase>(
                File.ReadAllText(ruta),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return cache;
        }
    }
}
