using ShareMarket.Core.Models.Dtos.Trading;

namespace ShareMarket.Core.Validations.Trading;

public class TradeBookDtoValidator : AbstractValidator<TradeBookDto>
{
    public TradeBookDtoValidator()
    {
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(m => m.Code)
            .NotEmpty()
            .WithMessage("Stock is required")
            .MaximumLength(StaticConfiguration.COMMAN_LENGTH)
            .WithMessage("Stock is too long");

        RuleFor(m => m.Strategy)
           .NotEmpty()
           .WithMessage("Statergy is required");

        RuleFor(m => m.BuyDate)
           .NotEmpty()
           .WithMessage("Statergy is required");

        RuleFor(m => m.TradeType)
           .NotEmpty()
           .WithMessage("TradeType is required")
           .IsInEnum()
           .WithMessage("TradeType is required");

        RuleFor(m => m.TradingAccount)
           .NotEmpty()
           .WithMessage("Trading Account is required");

        RuleFor(m => m.Quantity)
           .NotEmpty()
           .WithMessage("Quantity is required")
           .GreaterThan(0)
           .WithMessage("Quantity sholud be grater than 0");

        RuleFor(m => m.BuyRate)
           .NotEmpty()
           .WithMessage("BuyRate is required")
           .GreaterThan(0)
           .WithMessage("Order Rate sholud be grater than 0");
    }
}
