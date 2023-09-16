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
    public class ResetPasswordModel : IRequest<ResponseModel<string>>
    {
        public string Email { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string Token { get; set; } = default!;
    }


    public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordModel>
    {
        public ResetPasswordModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("Email is required");
            RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("New password is required");
            RuleFor(x => x.Token).NotEmpty().NotNull().WithMessage("Token is required");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Valid email address is required");
        }
    }

}
