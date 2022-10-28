using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class PODetailValidator : AbstractValidator<PODetailDto>
    {
        public PODetailValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.PoId).NotEmpty().WithMessage("podetail.PoId_required");
            RuleFor(s => s.MaterialId).NotEmpty().WithMessage("podetail.MaterialId_required");
            RuleFor(s => s.Qty).NotEmpty().WithMessage("purchaseorder.Qty_required").GreaterThan(0).WithMessage("purchaseorder.Qty_GreaterThan0");
        }
    }
}
