using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Autenticacion;
using Meat.Application.Shared;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    public class MeatBaseController : ControllerBase
    {
        private IMediator mediator;
        private CurrentUser currentUser;

        public MeatBaseController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> Handle<TResponse>(IRequest<TResponse> request)
        {
            object response;
            try
            {
                response = await this.mediator.Send(request);
            }
            catch (ArgumentException ex)
            {
                return this.BadRequest(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return this.BadRequest(new { message = ex.Message });
            }

            if (HttpMethods.IsGet(this.Request.Method))
            {
                if (response != null)
                {
                    return this.Ok(response);
                }

                return this.NotFound();
            }
            else if (HttpMethods.IsDelete(this.Request.Method) || HttpMethods.IsPut(this.Request.Method) || HttpMethods.IsPatch(this.Request.Method))
            {
                return this.NoContent();
            }
            else if (HttpMethods.IsPost(this.Request.Method))
            {
                return this.Created(this.Request.Path.ToUriComponent(), response);
            }
            else
            {
                return this.StatusCode(501); // NotImplemented
            }
        }

        protected CurrentUser CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    var Identity = HttpContext.User.Identity as ClaimsIdentity;
                    currentUser = new CurrentUser()
                    {
                        Id = new Guid(Identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault()),
                        UserName = Identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault(),
                        RolId = Identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault(),
                        CodigoEmpresa = Identity.Claims.Where(c => c.Type == ClaimTypes.PrimarySid).Select(c => c.Value).SingleOrDefault()
                    };
                }

                return currentUser;
            }
        }
    }
}