using Dapper;
using Meat.Queries.Dtos;
using Meat.Queries.Dtos.Response;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Meat.Queries
{
    public class UsuariosQueries : IUsuariosQueries
    {
        private readonly IDbConnection dbConnection;

        public UsuariosQueries(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public async Task<UsuariosResponse> GetUsuariosAsync(int? rol, int? estado, string filter, int pageSize, int pageIndex)
        {
           var queryFilter = string.Empty;
       
            var queryCount = "SELECT COUNT(*) ";
            var querySelect = @"
                    SELECT  Usuarios.Id,
                            Empleados.Legajo,
                            Usuarios.Nombre,
                            Usuarios.Apellido,
                            Usuarios.Email,
                            Usuarios.Rol,
                            Usuarios.Responsable,
                            Usuarios.Estado,
                            Usuarios.Credencial_UserName AS UserName,
                            Usuarios.EmpleadoId,
                            Usuarios.EsSuperUsuario ";
            var query = @"
                    FROM Usuarios
                    LEFT JOIN Empleados ON Empleados.Id = Usuarios.EmpleadoId
                    WHERE (@rol IS NULL OR Usuarios.Rol = @rol) AND
                          (@estado IS NULL OR Usuarios.Estado = @estado) AND
                          Usuarios.FechaBaja IS NULL
            ";

            if (!string.IsNullOrEmpty(filter))
            {
                queryFilter += @" AND (
                    Empleados.Legajo LIKE '%' + @filter + '%'       
                    OR Usuarios.Nombre LIKE '%' + @filter + '%'        
                    OR Usuarios.Apellido LIKE '%' + @filter + '%'
                    OR Usuarios.Email LIKE '%' + @filter + '%'
                    OR Usuarios.Credencial_UserName LIKE '%' + @filter + '%'
	                OR Usuarios.Nombre + ' ' + Usuarios.Apellido LIKE '%' + @filter + '%'
                    OR Usuarios.Apellido + ' ' + Usuarios.Nombre LIKE '%' + @filter + '%'
                ) ";
            }

            var queryFooter = @" ORDER BY Usuarios.Apellido, Usuarios.Nombre
                        OFFSET (@pageSize * @pageIndex) ROWS FETCH NEXT @pageSize ROWS ONLY   
            ";

            var parameters = new
            {
                rol = rol,
                estado = estado,
                filter = (filter != null ) ? filter.Replace(' ', '%') : filter,
                pageSize = pageSize,
                pageIndex = pageIndex

            };

            int totalRows = this.dbConnection.ExecuteScalar<int>(queryCount + query + queryFilter, parameters);

            var users = await dbConnection.QueryAsync<UsuarioDto>(querySelect + query + queryFilter + queryFooter, parameters);
            return new UsuariosResponse
            {
                TotalRows = totalRows,
                Usuarios = users
            };

        }
    }
}
