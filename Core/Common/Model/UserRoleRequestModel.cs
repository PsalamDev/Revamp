using FluentValidation;
using HRShared.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class UserRoleRequestModel
    {
        public Guid Id { get; set; } = default!;
        public Guid UserId { get; set; } = default!;
        public Guid RoleId { get; set; } = default!;
    }

    public class UserRoleResponseModel : UserRoleRequestModel
    {
        public UserResponseModel? UserModel { get; set; } = default!;
        public RoleResponseModel? RoleModel { get; set; } = default!;
    }

    public class UserRoleQueryModel : BaseQueryModel
    {

    }
    public class GetSingleUserRoleModel : IRequest<ResponseModel<UserRoleResponseModel>>
    {
        public Guid Id { get; set; } = default!;
    }

    public class GetSingleUserRoleModelValidator : AbstractValidator<GetSingleUserRoleModel>
    {
        public GetSingleUserRoleModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Identifier required");
        }
    }

    public class GetPaginatedUserRoleModel : PaginationRequest
    {
    }

    public class GetPaginatedUserRoleModelValidator : AbstractValidator<GetPaginatedUserRoleModel>
    {
        public GetPaginatedUserRoleModelValidator()
        {
            RuleFor(x => x.PageNumber).NotEmpty().NotNull().WithMessage("Page index required");
            RuleFor(x => x.PageSize).NotEmpty().NotNull().WithMessage("Page size required");
        }
    }
}
