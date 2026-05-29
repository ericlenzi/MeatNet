using Dapper;
using Meat.Queries.Dtos.Response;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Meat.Queries.Articulos
{
    public class ArticulosPreciosQueries : IArticulosPreciosQueries
    {
        private readonly IDbConnection dbConnection;

        public ArticulosPreciosQueries(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public async Task<ArticuloPrecioResponse> UpdateArticuloPreciosAsync(string codigo, double precio, double precioPRV, double precioJLQ, double precioVTA, string nroSucursal, double tasaIvaFiscal, double tasaIvaFiscalValor, string fechaProceso)
        {

            var query = @"
                UPDATE a SET
                            a.Precio = @precio,
                            a.PrecioPRV = @precioPRV,
                            a.PrecioJLQ = @precioJLQ,
                            a.PrecioVTA = @precioVTA,
                            a.TasaIvaFiscal = @tasaIvaFiscal,
                            a.TasaIvaFiscalValor = @tasaIvaFiscalValor,
                            a.FechaActualizacionPrecio = @fechaProceso,
                            a.FechaActualizacion = @fechaProceso
                FROM Articulos a WITH (NOLOCK) 
                INNER JOIN Sucursales s WITH (NOLOCK) ON a.SucursalId = s.Id
                WHERE
                    Codigo = @codigo AND
                    (@nroSucursal IS NULL OR s.NumeroSucursal = @nroSucursal)";

            var parameters = new
            {
                codigo,
                precio,
                precioPRV,
                precioJLQ,
                precioVTA,
                nroSucursal,
                tasaIvaFiscal,
                tasaIvaFiscalValor,
                fechaProceso
            };

            var queryCount = await dbConnection.ExecuteAsync(query, parameters);

            return new ArticuloPrecioResponse
            {
                CantidadRegistrosActualizados = queryCount
            };

        }

        public async Task<bool> Exist(Guid idSucursal, string codigo)
        {

            var query = @"
                 SELECT COUNT(1)
                FROM articulos a
                JOIN Sucursales s
	                ON s.Id = a.SucursalId
                WHERE 
	                s.Id = @idSucursal
                AND
	                a.Codigo = @codigo
                AND
	                a.FechaBaja IS NULL
            ";

            var articulosBD = await dbConnection.ExecuteScalarAsync<int>(query, new { idSucursal, codigo });

            return articulosBD > 0;
        }
    }
}
