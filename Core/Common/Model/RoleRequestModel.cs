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
    public class RoleRequestModel
    {
        public Guid Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public Guid CompanyId { get; set; } = default!;
        public string Description { get; set; } = default!;
        public List<string> Claimlist { get; set; } = default!;
    }

    public class RoleResponseModel
    {
        public string Id { get; set; } = default!;
        public Guid CompanyId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        //  public List<ClaimsResponseModel> ClaimResponse { get; set; } = default!;

    }
    public class RoleCreateModel : RoleRequestModel, IRequest<ResponseModel<RoleResponseModel>>
    {
    }

    public class RoleCreateModelValidator : AbstractValidator<RoleCreateModel>
    {
        public RoleCreateModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Role name is required");
            RuleFor(x => x.Description).NotEmpty().NotNull().WithMessage("Role description is required");
        }
    }
    public class RoleUpdateModel : RoleRequestModel, IRequest<ResponseModel<RoleResponseModel>>
    {
    }

    public class RoleUpdateModelValidator : AbstractValidator<RoleUpdateModel>
    {
        public RoleUpdateModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Role identifier is required");
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Role name is required");
            RuleFor(x => x.Description).NotEmpty().NotNull().WithMessage("Role description is required");
        }
    }
    public class GetSingleRoleModel : IRequest<ResponseModel<RoleResponseModel>>
    {
        public string Id { get; set; } = default!;
    }

    public class GetSingleRoleModelValidator : AbstractValidator<GetSingleRoleModel>
    {
        public GetSingleRoleModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Identifier required");
        }
    }

    public class GetPaginatedRoleModel : RoleQueryModel, IRequest<ResponseModel<PagedResult<RoleResponseModel>>>
    {
    }

    public class GetPaginatedRoleModelValidator : AbstractValidator<GetPaginatedRoleModel>
    {
        public GetPaginatedRoleModelValidator()
        {
            RuleFor(x => x.PageIndex).NotEmpty().NotNull().WithMessage("Page index required");
            RuleFor(x => x.PageSize).NotEmpty().NotNull().WithMessage("Page size required");

        }
    }
    public class RoleQueryModel : BaseQueryModel
    {


    }
  
}
