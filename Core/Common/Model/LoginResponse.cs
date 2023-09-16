using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public record LoginResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiryTime, string companyId = null, string UserId = null);
}