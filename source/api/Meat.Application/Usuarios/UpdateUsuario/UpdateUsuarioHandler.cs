using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Linq;

namespace Meat.Application.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioHandler : IRequestHandler<UpdateUsuarioRequest, UpdateUsuarioResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateUsuarioHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateUsuarioResponse> Handle(UpdateUsuarioRequest request, CancellationToken cancellationToken)
        {
            var usuario = await this.context.Usuarios
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.Empresa.CodigoEmpresa == request.CodigoEmpresa);

            if (usuario == null)
            {
                throw new ValidationException("El usuario no existe");
            }

            //var usuarioEmpleado = request.EmpleadoId != null ? await this.context.Usuarios.FirstOrDefaultAsync(x => x.Id != request.Id && x.EmpleadoId == request.EmpleadoId) : null;

            //if (usuarioEmpleado != null)
            //{
            //    throw new ValidationException("El legajo ya está asignado a un usuario");
            //}

            if (this.context.Usuarios.FirstOrDefault(x => x.UserName == request.UserName && x.Id != request.Id) != null)
            {
                throw new ValidationException("Ya existe un usuario con el nombre de usuario ingresado.");
            }

            usuario.UserName = request.UserName;
            usuario.FechaActualizacion = DateTime.UtcNow;

            this.mapper.Map(request, usuario);

            usuario.FechaActualizacion = System.DateTime.Now;

            await this.context.SaveChangesAsync();

            return new UpdateUsuarioResponse();
        }
    }
}