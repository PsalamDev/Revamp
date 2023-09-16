using Core.Common.Model.IdentityModels.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Controllers;

namespace API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class UserController : BaseController
    {

        [HttpPost]
        [Route("assignrole")]
        //[MustHavePermission(FSHAction.Create, FSHResource.Tenants)]
        //[OpenApiOperation("Create a new tenant.", "")]
        public async Task<IActionResult> AssignUserRole([FromBody] AssignUserRoleModel request)
        {
            return Ok(await Mediator.Send(request));
        }
    }
}
