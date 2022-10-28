using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class BomDetailValidator : AbstractValidator<BomDetailDto>
    {
        public BomDetailValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.BomId).NotEmpty().WithMessage("bomDetail.BomId_required");
            RuleFor(s => s.MaterialId).NotEmpty().WithMessage("bomDetail.MaterialId_required");
            RuleFor(s => s.Amount).NotEmpty().WithMessage("bomDetail.Amount_required");
        }
    }
}
