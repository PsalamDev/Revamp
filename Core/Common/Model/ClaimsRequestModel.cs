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
    public class ClaimsRequestModel
    {
        public Guid Id { get; set; } = default!;
        public string ClaimName { get; set; } = default!;
        public string ClaimValue { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Group { get; set; } = default!;
        public bool? IsAdmin { get; set; } = default!;
    }


    public class ClaimsResponseModel : ClaimsRequestModel
    {

    }

    public class ClaimsCreateModel : ClaimsRequestModel, IRequest<ResponseModel<ClaimsResponseModel>>
    {
    }

    public class ClaimsCreateValidator : AbstractValidator<ClaimsCreateModel>
    {
        public ClaimsCreateValidator()
        {
            RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("Claim description is required");
            RuleFor(x => x.Group).NotNull().NotEmpty().WithMessage("Claim group is required");
            RuleFor(x => x.ClaimName).NotNull().NotEmpty().WithMessage("Claim name is required");
            RuleFor(x => x.ClaimValue).NotNull().NotEmpty().WithMessage("Claim value is required");
        }
    }
    public class ClaimsUpdateModel : ClaimsRequestModel, IRequest<ResponseModel<ClaimsResponseModel>>
    {
    }

    public class UpdateClaimsValidator : AbstractValidator<ClaimsUpdateModel>
    {
        public UpdateClaimsValidator()
        {
            RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("Claim description is required");
            RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage("Claim identifier is required");
            RuleFor(x => x.Group).NotNull().NotEmpty().WithMessage("Claim group is required");
            RuleFor(x => x.ClaimName).NotNull().NotEmpty().WithMessage("Claim name is required");
            RuleFor(x => x.ClaimValue).NotNull().NotEmpty().WithMessage("Claim value is required");
        }
    }
    public class GetSingleClaimsModel : IRequest<ResponseModel<ClaimsResponseModel>>
    {
        public Guid Id { get; set; } = default!;
    }

    public class GetSingleClaimsModelValidator : AbstractValidator<GetSingleClaimsModel>
    {
        public GetSingleClaimsModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Identifier required");
        }
    }

    public class GetPaginatedClaimsModel : ClaimsQueryModel, IRequest<ResponseModel<PagedResult<ClaimsResponseModel>>>
    {
    }

    public class GetPaginatedClaimsModelValidator : AbstractValidator<GetPaginatedClaimsModel>
    {
        public GetPaginatedClaimsModelValidator()
        {
            RuleFor(x => x.PageIndex).NotNull().WithMessage("Page index required");
            RuleFor(x => x.PageSize).NotNull().WithMessage("Page size required");
        }
    }
    public class ClaimsQueryModel : BaseQueryModel
    {
        public bool IsAdmin { get; set; } = false;
    }
}

