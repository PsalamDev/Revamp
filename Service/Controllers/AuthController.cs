using Core.Common.Model;
using Core.Common.Model.IdentityModels.Identity;
using Core.Interfaces;
using HRShared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using static Core.Common.Model.IdentityModels.Identity.LoginRequestModel;

namespace Service.Controllers
{
   [ ApiController]
    public class AuthController : BaseController
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }


        [HttpPost]
        [Route("Login")]
        [OpenApiOperation("Login", "General login to the system")]
        [ProducesResponseType(typeof(ResponseModel<TokenResponseModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] LoginModel request)
        {
            request.IPAddress = GetIpAddress();
            var result = await Mediator.Send(request);
            return StatusCode(result.StatusCode, result);
        }

        //login

        //confirm email

        [HttpPost]
        [Route("confirmemail")]
        [OpenApiOperation("Confirm Email", "Endpoint for verifying user supplied email during registration")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailModel request)
        {
            var result = await Mediator.Send(request);
            return StatusCode(result.StatusCode, result);
        }

        //confirm otp
        [HttpGet]
        [Route("confirmotp")]
        [OpenApiOperation("Confirm OTP", "Endpoint for verifying applicant supplied OTP during registration")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string Email, string OTP)
        {
            var result = await _identityService.ConfirmOtpAsync(Email, OTP);
            return StatusCode(result.StatusCode, result);
        }

        //forgot password

        //reset password

        //change password

        [HttpPost]
        [Route("forgotPassword")]
        [OpenApiOperation("Forgotten Password", "Endpoint for initiating forgotten password request")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> ForgottenPassword([FromBody] ForgotPasswordModel request)
        {
            var result = await Mediator.Send(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost]
        [Route("resetPassword")]
        [OpenApiOperation("Reset Password", "Endpoint for resetting forgotten password")]
        [ProducesResponseType(typeof(ResponseModel<TokenResponseModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel request)
        {
            var result = await Mediator.Send(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost]
        [Route("changePassword")]
        [OpenApiOperation("Change Password", "Endpoint for changing password")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel request)
        {
            var result = await Mediator.Send(request);
            return StatusCode(result.StatusCode, result);
        }


        //

        private string GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
        ? Request.Headers["X-Forwarded-For"]
        : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
    }
}


