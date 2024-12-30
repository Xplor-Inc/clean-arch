using ShareMarket.Core.Models.Dtos.Equities;
using ShareMarket.Core.Models.Dtos.Trading;

namespace ShareMarket.Core.Validations.Equity;

public class EquityStockDtoValidator : AbstractValidator<EquityStockDto>
{
    public EquityStockDtoValidator()
    {
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(m => m.Code)
            .NotEmpty()
            .WithMessage("Stock is required")
            .MaximumLength(StaticConfiguration.NAME_LENGTH)
            .WithMessage("Stock is too long");

        RuleFor(m => m.Name)
           .NotEmpty()
           .WithMessage("Statergy is required");

        RuleFor(m => m.Sector)
           .NotEmpty()
           .WithMessage("Statergy is required");

        RuleFor(m => m.Industry)
           .NotEmpty()
           .WithMessage("TradeType is required");
    }
}