using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class TrayValidator : AbstractValidator<TrayDto>
    {
        public TrayValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.TrayCode).NotEmpty().WithMessage("tray.TrayCode_required");
            RuleFor(s => s.TrayType)
                .NotNull()
                .WithMessage("tray.TrayCode_required")
                .GreaterThan(0)
                .WithMessage("tray.TrayCode_required");
        }
    }
}
