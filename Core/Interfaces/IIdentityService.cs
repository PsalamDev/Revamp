using Core.Common.Model;
using Core.Common.Model.IdentityModels.Identity;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal);
        Task<string> RegisterAsync(RegisterUserModel request, string origin);
        Task<string> NewRegisterAsync(string firstName, string lastName, string Email, string Username,
            string password, string confirmpassword, string phonenumber, string CompanyId, string origin);

        Task<string> NewJobApplicantRegisterAsync(string firstName, string lastName, string Email, string Username,
            string password, string confirmpassword, string phonenumber, string CompanyId, string origin);
        Task<ResponseModel<bool>> ConfirmEmailAsync(Guid userId, string code, CancellationToken cancellationToken);
        Task<ResponseModel<bool>> ConfirmOtpAsync(string Email, string code);
        Task<string> ConfirmPhoneNumberAsync(Guid userId, string code);
        Task<ResponseModel<UserResponseModel>> GetSingle(Guid userId);
        Task<ResponseModel<UserResponseModel>> GetSingleByEmail(string Email);
        Task<ResponseModel<string>> ForgotPasswordAsync(UserResponseModel userModel, string origin, string companyName);
        Task<ResponseModel<string>> SendActivationMail(UserResponseModel userModel, string companyName);
        Task<ResponseModel<string>> SendAccountCreationMail(UserResponseModel userModel, string companyName, string defaultPassword);
        Task<ResponseModel<string>> ResetPasswordAsync(ResetPasswordModel request);
        Task UpdateProfileAsync(UpdateUserModel request, Guid userId);
        Task<ResponseModel<bool>> ChangePasswordAsync(ChangePasswordModel model, Guid userId);
    }
}
