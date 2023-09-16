using FluentValidation;
using HRShared.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model.IdentityModels.Identity
{
    public class AssignUserRoleModel : UserRoleRequestModel, IRequest<ResponseModel<UserRoleResponseModel>>
    {
    }

    public class AssignUserRoleModelValidator : AbstractValidator<AssignUserRoleModel>
    {
        public AssignUserRoleModelValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty().NotNull().WithMessage("Role identifier is required");
            RuleFor(x => x.UserId).NotEmpty().NotNull().WithMessage("UserId identifier is required");
        }
    }

}
