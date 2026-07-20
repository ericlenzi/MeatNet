using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Repositories
{
    public class MeatContext : DbContext
    {
        public virtual DbSet<Domain.Empresas.Empresa> Empresas { get; set; }
        public virtual DbSet<Domain.Sucursales.Sucursal> Sucursales { get; set; }
        public virtual DbSet<Domain.Parametros.Parametro> Parametros { get; set; }
        public virtual DbSet<Domain.Clientes.Cliente> Clientes { get; set; }
        public virtual DbSet<Domain.ClientesEstablecimientos.ClienteEstablecimiento> ClientesEstablecimientos { get; set; }
        public virtual DbSet<Domain.Establecimientos.Establecimiento> Establecimientos { get; set; }
        public virtual DbSet<Domain.EstablecimientosEspecies.EstablecimientoEspecie> EstablecimientosEspecies { get; set; }
        public virtual DbSet<Domain.Roles.Rol> Roles { get; set; }
        public virtual DbSet<Domain.Usuarios.Usuario> Usuarios { get; set; }
        public virtual DbSet<Domain.UsuariosSucursales.UsuarioSucursal> UsuariosSucursales { get; set; }
        public virtual DbSet<Domain.UsuariosEstablecimientos.UsuarioEstablecimiento> UsuariosEstablecimientos { get; set; }
        public virtual DbSet<Domain.Provincias.Provincia> Provincias { get; set; }
        public virtual DbSet<Domain.Puestos.Puesto> Puestos { get; set; }
        public virtual DbSet<Domain.Almacenes.Almacen> Almacenes { get; set; }
        public virtual DbSet<Domain.Materiales.Material> Materiales { get; set; }
        public virtual DbSet<Domain.Especies.Especie> Especies { get; set; }
        public virtual DbSet<Domain.TiposEmpresas.TipoEmpresa> TiposEmpresas { get; set; }
        public virtual DbSet<Domain.TiposClientes.TipoCliente> TiposClientes { get; set; }
        public virtual DbSet<Domain.TiposAlmacenes.TipoAlmacen> TiposAlmacenes { get; set; }
        public virtual DbSet<Domain.TiposSexos.TipoSexo> TiposSexos { get; set; }
        public virtual DbSet<Domain.TiposEspecies.TipoEspecie> TiposEspecies { get; set; }
        public virtual DbSet<Domain.OrigenesHaciendas.OrigenHacienda> OrigenesHaciendas { get; set; }
        public virtual DbSet<Domain.UsosHaciendas.UsoHacienda> UsosHaciendas { get; set; }
        public virtual DbSet<Domain.TiposMateriales.TipoMaterial> TiposMateriales { get; set; }
        public virtual DbSet<Domain.UnidadesMedidas.UnidadMedida> UnidadesMedidas { get; set; }
        public virtual DbSet<Domain.NumeradoresTropas.NumeradorTropa> NumeradoresTropas { get; set; }
        public virtual DbSet<Domain.TiposEstadosIngresos.TipoEstadoIngreso> TiposEstadosIngresos { get; set; }
        public virtual DbSet<Domain.TiposEstadosTropas.TipoEstadoTropa> TiposEstadosTropas { get; set; }
        public virtual DbSet<Domain.TiposEstadosHacienda.TipoEstadoHacienda> TiposEstadosHacienda { get; set; }
        public virtual DbSet<Domain.IngresosHaciendas.IngresoHacienda> IngresosHaciendas { get; set; }
        public virtual DbSet<Domain.IngresosHaciendas.IngresoHaciendaPesada> IngresosHaciendasPesadas { get; set; }
        public virtual DbSet<Domain.IngresosHaciendas.IngresoHaciendaUbicacion> IngresosHaciendasUbicaciones { get; set; }
        public virtual DbSet<Domain.Tropas.Tropa> Tropas { get; set; }
        public virtual DbSet<Domain.Tropas.TropaMovimiento> TropasMovimientos { get; set; }
        public virtual DbSet<Domain.TiposEstadosListasMatanzas.TipoEstadoListaMatanza> TiposEstadosListasMatanzas { get; set; }
        public virtual DbSet<Domain.ListasMatanzas.ListaMatanza> ListasMatanzas { get; set; }
        public virtual DbSet<Domain.ListasMatanzas.ListaMatanzaDetalle> ListasMatanzasDetalles { get; set; }
        public virtual DbSet<Domain.ListasMatanzas.ListaMatanzaMovimiento> ListasMatanzasMovimientos { get; set; }

        // Ejecucion de Faena - master data
        public virtual DbSet<Domain.TiposPuestos.TipoPuesto> TiposPuestos { get; set; }
        public virtual DbSet<Domain.TiposMediciones.TipoMedicion> TiposMediciones { get; set; }
        public virtual DbSet<Domain.DestinosComerciales.DestinoComercial> DestinosComerciales { get; set; }
        public virtual DbSet<Domain.TipificacionesOficiales.TipificacionOficial> TipificacionesOficiales { get; set; }
        public virtual DbSet<Domain.TiposDenticiones.TipoDenticion> TiposDenticiones { get; set; }
        public virtual DbSet<Domain.Denticiones.Denticion> Denticiones { get; set; }
        public virtual DbSet<Domain.TiposContusiones.TipoContusion> TiposContusiones { get; set; }
        public virtual DbSet<Domain.MotivosDecomisos.MotivoDecomiso> MotivosDecomisos { get; set; }
        public virtual DbSet<Domain.Numeradores.Numerador> Numeradores { get; set; }
        public virtual DbSet<Domain.UnidadesFaenas.UnidadFaena> UnidadesFaenas { get; set; }
        public virtual DbSet<Domain.Tipificaciones.Tipificacion> Tipificaciones { get; set; }

        // Ejecucion de Faena - romaneo (paso 3)
        public virtual DbSet<Domain.Romaneos.Romaneo> Romaneos { get; set; }
        public virtual DbSet<Domain.Romaneos.RomaneoPieza> RomaneosPiezas { get; set; }
        public virtual DbSet<Domain.Romaneos.RomaneoPiezaMedicion> RomaneosPiezasMediciones { get; set; }

        public MeatContext(DbContextOptions<MeatContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Soft Deleting

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.IsOwned())
                {
                    continue;
                }

                entityType.AddProperty("FechaBaja", typeof(DateTime?));

                var parameter = Expression.Parameter(entityType.ClrType);

                var propertyMethodInfo = typeof(EF).GetMethod("Property").MakeGenericMethod(typeof(DateTime?));
                var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("FechaBaja"));

                BinaryExpression compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(null));

                var lambda = Expression.Lambda(compareExpression, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }

            #endregion Soft Deleting

            #region FechaActualizacion Default Value

            //modelBuilder.Entity<Domain.Empleados.Empleado>().Property(x => x.FechaActualizacion).HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Domain.Puestos.Puesto>().Property(x => x.FechaActualizacion).HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Domain.Sucursales.Sucursal>().Property(x => x.FechaActualizacion).HasDefaultValueSql("getdate()");

            #endregion FechaActualizacion Default Value

            #region Indices Unicos

            // Tablas de relacion (unicidad compuesta)
            modelBuilder.Entity<Domain.NumeradoresTropas.NumeradorTropa>()
                .HasIndex(nt => new { nt.ClienteEstablecimientoId, nt.EspecieCodigo })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            modelBuilder.Entity<Domain.ClientesEstablecimientos.ClienteEstablecimiento>()
                .HasIndex(ce => new { ce.ClienteId, ce.EstablecimientoId })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            modelBuilder.Entity<Domain.UsuariosEstablecimientos.UsuarioEstablecimiento>()
                .HasIndex(ue => new { ue.UsuarioId, ue.EstablecimientoId })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            modelBuilder.Entity<Domain.UsuariosSucursales.UsuarioSucursal>()
                .HasIndex(us => new { us.UsuarioId, us.SucursalId })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            modelBuilder.Entity<Domain.EstablecimientosEspecies.EstablecimientoEspecie>()
                .HasIndex(ee => new { ee.EstablecimientoId, ee.EspecieId })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            // Codigos unicos (una columna)
            modelBuilder.Entity<Domain.Sucursales.Sucursal>()
                .HasIndex(s => s.CodigoSucursal)
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            modelBuilder.Entity<Domain.Usuarios.Usuario>()
                .HasIndex(u => u.UserName)
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            modelBuilder.Entity<Domain.Establecimientos.Establecimiento>()
                .HasIndex(e => e.CodigoEstablecimiento)
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            modelBuilder.Entity<Domain.Clientes.Cliente>()
                .HasIndex(c => c.CodigoCliente)
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            // Numero de ingreso correlativo por establecimiento
            modelBuilder.Entity<Domain.IngresosHaciendas.IngresoHacienda>()
                .HasIndex(i => new { i.EstablecimientoId, i.NumeroIngreso })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            // Numero de tropa unico por Cliente-Establecimiento + Especie
            modelBuilder.Entity<Domain.Tropas.Tropa>()
                .HasIndex(t => new { t.ClienteEstablecimientoId, t.EspecieCodigo, t.NumeroTropa })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            // Secuencia unica por tropa en el historial de trazabilidad
            modelBuilder.Entity<Domain.Tropas.TropaMovimiento>()
                .HasIndex(m => new { m.TropaId, m.Secuencia })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            // Una LM activa (no cancelada) por Establecimiento + Fecha + Especie
            modelBuilder.Entity<Domain.ListasMatanzas.ListaMatanza>()
                .HasIndex(lm => new { lm.EstablecimientoId, lm.Fecha, lm.EspecieId })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL AND [EstadoListaMatanzaId] <> 'ANULADA'");

            // Numero de lista correlativo por establecimiento
            modelBuilder.Entity<Domain.ListasMatanzas.ListaMatanza>()
                .HasIndex(lm => new { lm.EstablecimientoId, lm.NumeroLista })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            // Numerador unico por Establecimiento + Especie + Codigo
            modelBuilder.Entity<Domain.Numeradores.Numerador>()
                .HasIndex(n => new { n.EstablecimientoId, n.EspecieCodigo, n.Codigo })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            // Numero de unidad de faena unico por Especie
            modelBuilder.Entity<Domain.UnidadesFaenas.UnidadFaena>()
                .HasIndex(u => new { u.EspecieId, u.Numero })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL");

            // Garron unico por jornada (LM), entre romaneos no anulados
            modelBuilder.Entity<Domain.Romaneos.Romaneo>()
                .HasIndex(r => new { r.ListaMatanzaId, r.NumeroGarron })
                .IsUnique()
                .HasFilter("[FechaBaja] IS NULL AND [Anulado] = 0");

            // Indice de apoyo para lecturas de la jornada por nro de romaneo
            modelBuilder.Entity<Domain.Romaneos.Romaneo>()
                .HasIndex(r => new { r.ListaMatanzaId, r.NumeroRomaneo })
                .HasFilter("[FechaBaja] IS NULL");

            #endregion Indices Unicos

            #region Relaciones - Ingreso de Hacienda

            modelBuilder.Entity<Domain.IngresosHaciendas.IngresoHacienda>(e =>
            {
                e.HasOne(x => x.Establecimiento).WithMany().HasForeignKey(x => x.EstablecimientoId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.ClienteEstablecimiento).WithMany().HasForeignKey(x => x.ClienteEstablecimientoId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Provincia).WithMany().HasForeignKey(x => x.ProvinciaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.OrigenHacienda).WithMany().HasForeignKey(x => x.OrigenHaciendaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.UsoHacienda).WithMany().HasForeignKey(x => x.UsoHaciendaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.EstadoIngreso).WithMany().HasForeignKey(x => x.EstadoIngresoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.Tropas.Tropa>(e =>
            {
                e.HasOne(x => x.IngresoHacienda).WithMany(i => i.Tropas).HasForeignKey(x => x.IngresoHaciendaId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieCodigo).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.EstadoTropa).WithMany().HasForeignKey(x => x.EstadoTropaId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.Tropas.TropaMovimiento>(e =>
            {
                e.HasOne(x => x.Tropa).WithMany().HasForeignKey(x => x.TropaId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.EstadoResultante).WithMany().HasForeignKey(x => x.EstadoResultanteId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.IngresosHaciendas.IngresoHaciendaPesada>(e =>
            {
                e.HasOne(x => x.IngresoHacienda).WithMany(i => i.Pesadas).HasForeignKey(x => x.IngresoHaciendaId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.TipoEspecie).WithMany().HasForeignKey(x => x.TipoEspecieId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.IngresosHaciendas.IngresoHaciendaUbicacion>(e =>
            {
                e.HasOne(x => x.IngresoHacienda).WithMany(i => i.Ubicaciones).HasForeignKey(x => x.IngresoHaciendaId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Tropa).WithMany().HasForeignKey(x => x.TropaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.TipoEspecie).WithMany().HasForeignKey(x => x.TipoEspecieId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Almacen).WithMany().HasForeignKey(x => x.AlmacenId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.EstadoHacienda).WithMany().HasForeignKey(x => x.EstadoHaciendaId).OnDelete(DeleteBehavior.Restrict);
            });

            // Almacen ahora pertenece a un Establecimiento
            modelBuilder.Entity<Domain.Almacenes.Almacen>()
                .HasOne(a => a.Establecimiento).WithMany().HasForeignKey(a => a.EstablecimientoId).OnDelete(DeleteBehavior.Restrict);

            #endregion Relaciones - Ingreso de Hacienda

            #region Relaciones - Lista de Matanza

            modelBuilder.Entity<Domain.ListasMatanzas.ListaMatanza>(e =>
            {
                e.Property(x => x.Fecha).HasColumnType("date");
                e.HasOne(x => x.Establecimiento).WithMany().HasForeignKey(x => x.EstablecimientoId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.EstadoListaMatanza).WithMany().HasForeignKey(x => x.EstadoListaMatanzaId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.ListasMatanzas.ListaMatanzaDetalle>(e =>
            {
                e.HasOne(x => x.ListaMatanza).WithMany(lm => lm.Renglones).HasForeignKey(x => x.ListaMatanzaId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Tropa).WithMany().HasForeignKey(x => x.TropaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Almacen).WithMany().HasForeignKey(x => x.AlmacenId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.AlmacenDestino).WithMany().HasForeignKey(x => x.AlmacenDestinoId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.TipoEspecie).WithMany().HasForeignKey(x => x.TipoEspecieId).OnDelete(DeleteBehavior.Restrict).IsRequired();
            });

            modelBuilder.Entity<Domain.ListasMatanzas.ListaMatanzaMovimiento>(e =>
            {
                e.HasOne(x => x.ListaMatanza).WithMany(lm => lm.Movimientos).HasForeignKey(x => x.ListaMatanzaId).OnDelete(DeleteBehavior.Cascade);
            });

            // Puesto asignado a la LM (opcional)
            modelBuilder.Entity<Domain.ListasMatanzas.ListaMatanza>()
                .HasOne(x => x.Puesto).WithMany().HasForeignKey(x => x.PuestoId).OnDelete(DeleteBehavior.Restrict);

            #endregion Relaciones - Lista de Matanza

            #region Relaciones - Puestos

            modelBuilder.Entity<Domain.Puestos.Puesto>(e =>
            {
                e.HasOne(x => x.Establecimiento).WithMany().HasForeignKey(x => x.EstablecimientoId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.TipoPuesto).WithMany().HasForeignKey(x => x.TipoPuestoId).OnDelete(DeleteBehavior.Restrict);
            });

            #endregion Relaciones - Puestos

            #region Relaciones - Ejecucion de Faena (master data)

            modelBuilder.Entity<Domain.TipificacionesOficiales.TipificacionOficial>()
                .HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Domain.TiposDenticiones.TipoDenticion>()
                .HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Domain.Denticiones.Denticion>(e =>
            {
                e.HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.TipoDenticion).WithMany().HasForeignKey(x => x.TipoDenticionId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.TiposContusiones.TipoContusion>()
                .HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Domain.MotivosDecomisos.MotivoDecomiso>()
                .HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Domain.Numeradores.Numerador>(e =>
            {
                e.HasOne(x => x.Establecimiento).WithMany().HasForeignKey(x => x.EstablecimientoId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieCodigo).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.UnidadesFaenas.UnidadFaena>()
                .HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Domain.Tipificaciones.Tipificacion>(e =>
            {
                e.HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.TipoEspecie).WithMany().HasForeignKey(x => x.TipoEspecieId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.UnidadFaena).WithMany().HasForeignKey(x => x.UnidadFaenaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.DestinoComercial).WithMany().HasForeignKey(x => x.DestinoComercialId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.TipificacionOficial).WithMany().HasForeignKey(x => x.TipificacionOficialId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.UnidadMedida).WithMany().HasForeignKey(x => x.UnidadMedidaId).OnDelete(DeleteBehavior.Restrict);
            });

            #endregion Relaciones - Ejecucion de Faena (master data)

            #region Relaciones - Romaneo (Ejecucion de Faena)

            modelBuilder.Entity<Domain.Romaneos.Romaneo>(e =>
            {
                e.HasOne(x => x.ListaMatanza).WithMany().HasForeignKey(x => x.ListaMatanzaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.ListaMatanzaDetalle).WithMany().HasForeignKey(x => x.ListaMatanzaDetalleId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Tropa).WithMany().HasForeignKey(x => x.TropaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Especie).WithMany().HasForeignKey(x => x.EspecieId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.UnidadFaena).WithMany().HasForeignKey(x => x.UnidadFaenaId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.Romaneos.RomaneoPieza>(e =>
            {
                e.HasOne(x => x.Romaneo).WithMany(r => r.Piezas).HasForeignKey(x => x.RomaneoId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Tipificacion).WithMany().HasForeignKey(x => x.TipificacionId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.Romaneos.RomaneoPiezaMedicion>(e =>
            {
                e.HasOne(x => x.RomaneoPieza).WithMany(p => p.Mediciones).HasForeignKey(x => x.RomaneoPiezaId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.TipoMedicion).WithMany().HasForeignKey(x => x.TipoMedicionId).OnDelete(DeleteBehavior.Restrict);
            });

            #endregion Relaciones - Romaneo (Ejecucion de Faena)
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["FechaBaja"] = DateTime.Now;
                        break;
                }
            }
        }
    }
}