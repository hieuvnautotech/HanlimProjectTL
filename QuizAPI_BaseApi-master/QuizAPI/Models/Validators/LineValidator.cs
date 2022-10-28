using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class LineValidator : AbstractValidator<LineDto>
    {
        public LineValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.LineName).NotEmpty().WithMessage("line.LineName_required");
        }
    }
}
