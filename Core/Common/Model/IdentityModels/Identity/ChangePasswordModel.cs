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
    public class ChangePasswordModel : IRequest<ResponseModel<bool>>
    {
        public string Password { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmNewPassword { get; set; } = default!;

        public Guid userId { get; set; }
    }


    public class ChangePasswordModelValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordModelValidator()
        {
            RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("Old password  is required");
            RuleFor(x => x.NewPassword).NotNull().WithMessage("New password is required");
            RuleFor(x => x.ConfirmNewPassword).NotNull().WithMessage("Confirm new password is required");
            RuleFor(x => x.ConfirmNewPassword).Matches(x => x.Password).WithMessage("Confirm new password must match new password");
        }
    }
}
