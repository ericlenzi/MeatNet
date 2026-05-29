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
        public virtual DbSet<Domain.ParametrosSucursales.ParametroSucursal> ParametrosSucursales { get; set; }
        public virtual DbSet<Domain.Puestos.Puesto> Puestos { get; set; }
        public virtual DbSet<Domain.Establecimientos.Establecimiento> Establecimientos { get; set; }
        public virtual DbSet<Domain.Roles.Rol> Roles { get; set; }
        public virtual DbSet<Domain.Usuarios.Usuario> Usuarios { get; set; }
        public virtual DbSet<Domain.UsuariosSucursales.UsuarioSucursal> UsuariosSucursales { get; set; }
        public virtual DbSet<Domain.Provincias.Provincia> Provincias { get; set; }
        public virtual DbSet<Domain.Almacenes.Almacen> Almacenes { get; set; }
        public virtual DbSet<Domain.Materiales.Material> Materiales { get; set; }
        public virtual DbSet<Domain.Especies.Especie> Especies { get; set; }
        public virtual DbSet<Domain.TiposEmpresas.TipoEmpresa> TiposEmpresas { get; set; }
        public virtual DbSet<Domain.TiposAlmacenes.TipoAlmacen> TiposAlmacenes { get; set; }
        public virtual DbSet<Domain.TiposSexos.TipoSexo> TiposSexos { get; set; }
        public virtual DbSet<Domain.TiposEspecies.TipoEspecie> TiposEspecies { get; set; }
        public virtual DbSet<Domain.AlmacenesMateriales.AlmacenMaterial> AlmacenesMateriales { get; set; }

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