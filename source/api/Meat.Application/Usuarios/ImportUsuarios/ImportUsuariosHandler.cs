using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Meat.Repositories;
using Meat.Application.Shared.GeneratePassword;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Meat.Application.Usuarios.ImportUsuarios
{
    public class ImportUsuariosHandler : IRequestHandler<ImportUsuariosRequest, ImportUsuariosResponse>
    {
        private readonly MeatContext context;
        private readonly IMediator mediator;

        public ImportUsuariosHandler(MeatContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public async Task<ImportUsuariosResponse> Handle(ImportUsuariosRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await this.RecursiveCreate(request.Data);
                var rowsAffected = await this.context.SaveChangesAsync();
                //await this.RecursiveUpdate(request.Data);
                //var rowsAffected = 0;

                return new ImportUsuariosResponse()
                {
                    Success = rowsAffected > 0,
                    Message = "OK",
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task RecursiveCreate(IEnumerable<ImportUsuariosItem> items)
        {
            foreach (var item in items)
            {
                var generatePasswordResponse = this.mediator.Send(new GeneratePasswordRequest()
                {
                    Contraseña = item.Apellido.Replace(" ", string.Empty).ToLower(),
                }).Result;

                var empresa = context.Empresas.FirstOrDefault(x => x.CodigoEmpresa == item.CodigoEmpresa);

                if (empresa != null)
                {
                    var usuario = Domain.Usuarios.UsuarioFactory.Create(
                                        item.UserName.ToLower(),
                                        generatePasswordResponse.PasswordHash,
                                        item.Nombre, 
                                        item.Apellido,
                                        item.Email,
                                        item.Legajo,
                                        item.RolId,
                                        empresa.Id,
                                        true
                                    );

                    this.context.Usuarios.Add(usuario);
                }
                else
                {
                    var usuario = Domain.Usuarios.UsuarioFactory.Create(
                                        item.UserName.ToLower(),
                                        generatePasswordResponse.PasswordHash, 
                                        item.Nombre,
                                        item.Apellido,
                                        item.Email,
                                        item.Legajo,
                                        item.RolId,
                                        empresa.Id,
                                        true
                                    );

                    this.context.Usuarios.Add(usuario);
                }
            }
        }

        private async Task RecursiveUpdate(IEnumerable<ImportUsuariosItem> items)
        {
            foreach (var item in items)
            {
                //var legajo = "000" + item.Legajo;
                var usuario = context.Usuarios.Include(x => x.Empresa).FirstOrDefault(x => x.Empresa.CodigoEmpresa == item.CodigoEmpresa);
                if (usuario != null)
                {
                    usuario.Email = item.Email;
                    this.context.Usuarios.Update(usuario);
                }
            }
            await this.context.SaveChangesAsync();
        }
    }
}