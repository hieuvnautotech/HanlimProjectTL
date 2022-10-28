using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class BomValidator : AbstractValidator<BomDto>
    {
        public BomValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.MaterialId).NotEmpty().WithMessage("bom.MaterialId_required");
        }
    }
}
