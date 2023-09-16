using Core.Common.Model;
using Core.Interfaces;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation
{
    public class RoleClaimServiceHelper
    {
        private readonly IRoleClaimsService _roleClaimService;
        public RoleClaimServiceHelper(IRoleClaimsService roleClaimService)
        {
            _roleClaimService = roleClaimService;
        }
        public async Task<ResponseModel<bool>> CreateListAsync(List<RoleClaimsRequestModel> requests)
        {
            return await _roleClaimService.CreateListAsync(requests);
        }
    }
}
