﻿using ShareMarket.Core.Models.Dtos.Users;

namespace ShareMarket.Core.Validations.Accounts;
public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(m => m.EmailAddress)
            .NotEmpty()
            .WithMessage("EmailAddress is required")
            .EmailAddress()
            .WithMessage("Invalid email address")
            .MaximumLength(StaticConfiguration.EMAIL_LENGTH)
            .WithMessage("Invalid emailAddress format");

        RuleFor(m => m.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required")
            .MaximumLength(StaticConfiguration.NAME_LENGTH)
            .WithMessage("FirstName is too long");

        RuleFor(m => m.LastName)
           .NotEmpty()
           .WithMessage("LastName is required")
           .MaximumLength(StaticConfiguration.NAME_LENGTH)
           .WithMessage("LastName is too long");

    }
}