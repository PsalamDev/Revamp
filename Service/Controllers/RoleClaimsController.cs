using Core.Common.Model;
using HRShared.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Service.Controllers;

namespace API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class RoleClaimsController : BaseController
    {
       

        //create claims
        [HttpPost]
        //[MustHavePermission(FSHAction.Create, FSHResource.Tenants)]
        [OpenApiOperation("Add role claims/permissions", "")]
        [ProducesResponseType(typeof(ResponseModel<RoleClaimsResponseModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] RoleClaimsCreateCommand request)
        {
            var result = await Mediator.Send(request);

            return StatusCode(result.StatusCode, result);
        }

        //update claims
        [HttpPut]

        //[MustHavePermission(FSHAction.Create, FSHResource.Tenants)]
        [OpenApiOperation("Update role claims/permission", "")]
        [ProducesResponseType(typeof(ResponseModel<RoleClaimsResponseModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> UpdateAsync([FromBody] RoleClaimsUpdateCommand request)
        {
            var result = await Mediator.Send(request);

            return StatusCode(result.StatusCode, result);
        }





        [HttpGet, Route("getroleclaim/{roleclaimId}")]
        //[MustHavePermission(FSHAction.Create, FSHResource.Tenants)]
        [OpenApiOperation("Get role claim by Id", "")]
        [ProducesResponseType(typeof(ResponseModel<RoleClaimsResponseModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> GetSingle(int roleclaimId)
        {
            var request = new GetSingleRoleClaimsModel
            {
                Id = roleclaimId
            };

            var result = await Mediator.Send(request);

            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("getroleclaims")]
        //[MustHavePermission(FSHAction.Create, FSHResource.Tenants)]
        [OpenApiOperation("Get role claims for role.", "")]
        [ProducesResponseType(typeof(ResponseModel<PagedResult<RoleClaimsResponseModel>>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> GetPaginated([FromQuery] RoleClaimsQueryModel model)
        {
            var request = new GetPaginatedRoleClaimsModel
            {
                PageIndex = model.PageIndex,
                Keyword = model.Keyword,
                PageSize = model.PageSize
            };

            var result = await Mediator.Send(request);

            return Ok(result);
        }
    }
}
