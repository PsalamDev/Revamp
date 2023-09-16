using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model.IdentityModels.Identity
{
    public record RefreshTokenModel(string Token, string RefreshToken);
}
