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
    public class ForgotPasswordModel : IRequest<ResponseModel<string>>
    {
        public string Email { get; set; } = default!;

    }


    public class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
    {
        public ForgotPasswordModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("Email is required");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Valid email address is required");
        }
    }
}
