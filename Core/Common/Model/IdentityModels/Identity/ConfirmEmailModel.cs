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
    public class ConfirmEmailModel : IRequest<ResponseModel<bool>>
    {
        public Guid UserId { get; set; } = default!;
        public string Code { get; set; } = default!;
    }


    public class ConfirmEmailModelValidator : AbstractValidator<ConfirmEmailModel>
    {
        public ConfirmEmailModelValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull().WithMessage("User Identifier is required");
            RuleFor(x => x.Code).NotNull().WithMessage("Code is required");
        }
    }
}