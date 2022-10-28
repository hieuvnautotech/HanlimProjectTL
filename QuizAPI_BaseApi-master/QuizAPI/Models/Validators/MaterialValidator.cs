using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class MaterialValidator : AbstractValidator<MaterialDto>
    {
        public MaterialValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.MaterialCode).NotEmpty().WithMessage("material.MaterialCode_required");
            RuleFor(s => s.MaterialType).NotEmpty().WithMessage("material.MaterialType_required");
            RuleFor(s => s.Unit).NotEmpty().WithMessage("material.Unit_required");
        }
    }
}
