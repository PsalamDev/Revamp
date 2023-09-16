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
    public class LoginRequestModel
    {
        public class LoginModel : TokenRequestModel, IRequest<ResponseModel<TokenResponseModel>>
        {
        }

        public class LoginModelValidator : AbstractValidator<LoginModel>
        {
            public LoginModelValidator()
            {
                RuleFor(x => x.Email).Cascade(CascadeMode.Stop).EmailAddress().WithMessage("Valid email address required");
                RuleFor(x => x.Password).NotEmpty().NotNull().MinimumLength(6).WithMessage("Password required");
            }
        }
    }
}
