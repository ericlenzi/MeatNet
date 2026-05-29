using Dapper;
using Meat.Domain.Sucursales;
using Meat.Queries.Dtos;
using Meat.Queries.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Meat.Queries.Informes
{
    public class VentasEmpleadosQueries : IVentasEmpleadosQueries
    {
        private readonly IDbConnection dbConnection;

        public VentasEmpleadosQueries(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public async Task<VentaEmpleadoResponse> GetVentasEmpleadosAsync(DateTime fechaDesde, DateTime fechaHasta, Sucursal sucursal, int pageSize, int pageIndex)
        {
            var queryCount = "SELECT COUNT(*) ";
            var querySelect = @"
                  SELECT    v.Id,	
                            v.Fecha,
		                    c.Nombre + ' ' + c.Apellido as NombreCliente,
                            c.SAP_Codigo as CodigoSAPCliente,
                            c.NumeroDocumento as NroDocCliente,
		                  	ISNULL(e.Legajo,emp.Legajo) as LegajoEmpleado,
		                    u.Credencial_UserName as UserName,
                            comp.Nombre as TipoComprobante,
                            FORMAT(v.NumeroPuntoVenta, '0000') + '-' + 	FORMAT(v.NumeroComprobante, '00000000') as NumeroComprobanteCompleto ,
		                    s.NumeroSucursal + '-' + s.Nombre as NumeroSucursal,
		                    v.TipoPuntoVenta,
                            CASE
                                When comp.TipoComprobanteId = 1 Then MontoTotal
                                When comp.TipoComprobanteId = 2 Then MontoTotal * (-1) 
								END as MontoTotal ";
            var query = @"
                            FROM Ventas v WITH (NOLOCK)
                            INNER JOIN Clientes c on v.ClienteId = c.Id
                            INNER JOIN Puestos p on v.PuestoId = p.Id
                            INNER JOIN Sucursales s on p.SucursalId = s.Id
                            INNER JOIN Comprobantes comp on v.ComprobanteId = comp.Id
                            INNER JOIN Usuarios u on v.UsuarioId = u.Id
                            LEFT JOIN Empleados e on e.NroDocumento = SUBSTRING(c.NumeroDocumento, 3, 8)
	                        LEFT JOIN Empleados emp on emp.NroDocumento = c.NumeroDocumento
                    WHERE
	                        v.EsEmpleadoCC = 1 AND
		                    v.Estado = 2 AND
                            (@sucursalId IS NULL OR p.SucursalId = @sucursalId) AND
                            v.Fecha >= @fechaDesde AND
                            v.Fecha <= @fechaHasta ";

                var queryFooter = @" ORDER BY v.Fecha DESC, v.comprobanteId DESC, v.NumeroPuntoVenta DESC, v.NumeroComprobante DESC 
                        OFFSET (@pageSize * @pageIndex) ROWS FETCH NEXT @pageSize ROWS ONLY";

            var parameters = new {
                sucursalId = (sucursal != null) ? sucursal.Id.ToString() : null,
                    fechaDesde = fechaDesde,
                    fechaHasta = fechaHasta,
                    pageSize = pageSize,
                    pageIndex = pageIndex
            };

            int totalRows = this.dbConnection.ExecuteScalar<int>(queryCount + query, parameters);
           

            var ventas = await dbConnection.QueryAsync<VentaEmpleadoDto>(querySelect + query + queryFooter, parameters);
            return new VentaEmpleadoResponse { 
            TotalRows = totalRows,
            VentasEmpleados = ventas
            };
        }
    }
}
