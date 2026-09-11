using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Tipificadores.CreateTipificador;
using Meat.Application.Tipificadores.DeleteTipificador;
using Meat.Application.Tipificadores.GetTipificador;
using Meat.Application.Tipificadores.GetTipificadores;
using Meat.Application.Tipificadores.UpdateTipificador;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Tipificadores habilitados por Establecimiento + Especie. La lectura la usa el Tipificador
    /// para proponer quien esta en el palco, asi que queda en el rol operativo; el alta y la
    /// edicion son de administracion.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class TipificadoresController : MeatBaseController
    {
        public TipificadoresController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetTipificadoresAsync([FromQuery] GetTipificadoresRequest request) =>
            await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipificadorByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetTipificadorRequest { Id = id });

        [HttpPost]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> CreateTipificadorAsync([FromBody] CreateTipificadorRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> UpdateTipificadorAsync([FromRoute] Guid id, [FromBody] UpdateTipificadorRequestFromBody body) =>
            await this.Handle(new UpdateTipificadorRequest
            {
                Id = id,
                Nombre = body.Nombre,
                Matricula = body.Matricula,
                EstablecimientoId = body.EstablecimientoId,
                EspecieId = body.EspecieId,
                PorDefecto = body.PorDefecto,
                Activo = body.Activo
            });

        [HttpDelete("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> DeleteTipificadorByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteTipificadorRequest { Id = id });
    }
}
