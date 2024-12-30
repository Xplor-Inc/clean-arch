using ShareMarket.Core.Models.Dtos.Trading;

namespace ShareMarket.Core.Validations.Trading;

public class TradeOrderDtoValidator : AbstractValidator<TradeOrderDto>
{
    public TradeOrderDtoValidator()
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

        RuleFor(m => m.TradeType)
           .NotEmpty()
           .WithMessage("Trade Type is required")
           .IsInEnum()
           .WithMessage("Trade Type is required");

        RuleFor(m => m.TradingAccount)
           .NotEmpty()
           .WithMessage("Trading Account is required");

        RuleFor(m => m.Quantity)
           .NotEmpty()
           .WithMessage("Quantity is required")
           .GreaterThan(0)
           .WithMessage("Quantity sholud be grater than 0");

        RuleFor(m => m.OrderRate)
           .NotEmpty()
           .WithMessage("Order Rate is required");
    }
}