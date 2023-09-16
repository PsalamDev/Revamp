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
    public class RoleClaimsRequestModel
    {
        public int Id { get; set; } = default!;
        public Guid CompanyId { get; set; } = default!;
        public Guid RoleId { get; set; } = default!;
        public Guid ClaimId { get; set; } = default!;
        public Guid CreatedBy { get; set; } = default!;
    }

    public class RoleClaimsResponseModel : RoleClaimsRequestModel
    {
        public RoleResponseModel? Role { get; set; } = default!;
        public ClaimsResponseModel? Claims { get; set; } = default!;
    }

    public class RoleClaimsCreateCommand : RoleClaimsRequestModel, IRequest<ResponseModel<RoleClaimsResponseModel>>
    {
    }

    public class RoleClaimsCreateCommandValidator : AbstractValidator<RoleClaimsCreateCommand>
    {
        public RoleClaimsCreateCommandValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty().NotNull().WithMessage("Role identifier is required");
            RuleFor(x => x.CompanyId).NotEmpty().NotNull().WithMessage("Company identifier is required");
            RuleFor(x => x.ClaimId).NotEmpty().NotNull().WithMessage("Claim identifier description is required");
        }
    }

    public class RoleClaimsUpdateCommand : RoleClaimsRequestModel, IRequest<ResponseModel<RoleClaimsResponseModel>>
    {
    }

    public class RoleClaimsUpdateCommandValidator : AbstractValidator<RoleClaimsUpdateCommand>
    {
        public RoleClaimsUpdateCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Role claims identifier is required");
            RuleFor(x => x.CompanyId).NotEmpty().NotNull().WithMessage("Company identifier is required");
            RuleFor(x => x.ClaimId).NotEmpty().NotNull().WithMessage("Claims identifier is required");
            RuleFor(x => x.RoleId).NotEmpty().NotNull().WithMessage("Role identifier is required");
        }
    }
    public class GetSingleRoleClaimsModel : IRequest<ResponseModel<RoleClaimsResponseModel>>
    {
        public int Id { get; set; } = default!;
    }

    public class GetSingleRoleClaimsModelValidator : AbstractValidator<GetSingleRoleClaimsModel>
    {
        public GetSingleRoleClaimsModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Identifier required");
        }
    }

    public class GetPaginatedRoleClaimsModel : RoleClaimsQueryModel, IRequest<ResponseModel<PagedResult<RoleClaimsResponseModel>>>
    {
    }

    public class GetPaginatedRoleClaimsModelValidator : AbstractValidator<GetPaginatedRoleClaimsModel>
    {
        public GetPaginatedRoleClaimsModelValidator()
        {
            RuleFor(x => x.PageIndex).NotEmpty().NotNull().WithMessage("Page index required");
            RuleFor(x => x.PageSize).NotEmpty().NotNull().WithMessage("Page size required");
        }
    }

    public class RoleClaimsQueryModel : BaseQueryModel
    {
    }
}
