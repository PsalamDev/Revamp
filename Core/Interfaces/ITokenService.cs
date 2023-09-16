using Core.Common.Model;
using Core.Common.Model.IdentityModels.Identity;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Common.Model.IdentityModels.Identity.LoginRequestModel;

namespace Core.Interfaces
{
    public interface ITokenService
    {
        Task<ResponseModel<LoginResponse>> GetTokenAsync(LoginModel request, string ipAddress, CancellationToken cancellationToken);

        Task<ResponseModel<LoginResponse>> RefreshTokenAsync(RefreshTokenModel request, string ipAddress);
    }
}
