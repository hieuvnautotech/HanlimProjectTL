using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class ForecastValidator : AbstractValidator<ForecastPODto>
    {
        public ForecastValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.MaterialId)
                .NotNull()
                .WithMessage("forecast.MaterialId_required")
                .GreaterThan(0)
                .WithMessage("forecast.MaterialId_required");
            RuleFor(s => s.LineId)
                .NotNull()
                .WithMessage("forecast.LineId_required")
                .GreaterThan(0)
                .WithMessage("forecast.LineId_required");
            RuleFor(s => s.Week).NotNull().WithMessage("forecast.Week_required").InclusiveBetween(1, 52)
            .WithMessage("forecast.Week_required_range");
            RuleFor(s => s.Year).NotNull().WithMessage("forecast.Year_required").InclusiveBetween(2022, 2050)
            .WithMessage("forecast.Year_required_range");
            RuleFor(s => s.Amount).NotNull().WithMessage("forecast.Amount_required");
        }
    }
}
