using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.AnalisisFaena.GetAnalisisFaena;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Analisis de Faena (Ciclo I paso 4b): la lectura de la jornada. Solo lectura.
    /// Ver docs/manuales/AnalisisFaena.md.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class AnalisisFaenaController : MeatBaseController
    {
        public AnalisisFaenaController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetAnalisisFaenaRequest request)
        {
            request.EmpresaId = base.CurrentUser.EmpresaId;
            return await this.Handle(request);
        }
    }
}
