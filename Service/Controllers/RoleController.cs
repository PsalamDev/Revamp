using Core.Common.Model;
using Core.Interfaces;
using HRShared.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Service.Controllers;

namespace API.Controllers
{
    [ApiController]
    public class RoleController : BaseController
    {

        private readonly ICurrentUser _currentUser;

        public RoleController(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        //create claims
        [HttpPost]
        //[MustHavePermission(FSHAction.Create, FSHResource.Tenants)]
        [OpenApiOperation("Create a new role", "")]
        public async Task<IActionResult> CreateAsync([FromBody] RoleCreateModel request)
        {
            var result = await Mediator.Send(request);

            return StatusCode(result.StatusCode, result);
        }

        //update claims
        [HttpPut]
        [OpenApiOperation("Update role details", "")]
        public async Task<IActionResult> UpdateAsync([FromBody] RoleUpdateModel request)
        {
            var result = await Mediator.Send(request);

            return StatusCode(result.StatusCode, result);
        }


      


        [HttpGet, Route("getrole/{roleId}")]
        // [MustHavePermission(AdminPermissions.Role.View)]
        [OpenApiOperation("Get single role")]
        [ProducesResponseType(typeof(ResponseModel<RoleResponseModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> GetSingle([FromRoute] string roleId)
        {
            var user = _currentUser.GetUserId();
            var request = new GetSingleRoleModel
            {
                Id = roleId
            };

            var result = await Mediator.Send(request);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("getroles")]
        [OpenApiOperation("Get paginated role list")]
        //[TenantIdHeader]
        [ProducesResponseType(typeof(ResponseModel<List<RoleResponseModel>>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> GetPaginated([FromQuery] RoleQueryModel model)
        {
            var request = new GetPaginatedRoleModel
            {
                PageIndex = model.PageIndex,
                Keyword = model.Keyword,
                PageSize = model.PageSize
            };

            var result = await Mediator.Send(request);

            return StatusCode(result.StatusCode, result);
        }
    }
}