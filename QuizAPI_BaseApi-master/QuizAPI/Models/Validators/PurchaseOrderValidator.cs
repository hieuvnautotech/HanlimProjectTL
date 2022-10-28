using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class PurchaseOrderValidator : AbstractValidator<PurchaseOrderDto>
    {
        public PurchaseOrderValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.PoCode).NotEmpty().WithMessage("purchase_order.PoCode_required").MaximumLength(50).WithMessage("purchase_order.PoCode_maxLength");
            RuleFor(s => s.DeliveryDate).NotEmpty().WithMessage("purchase_order.DeliveryDate_required");
            RuleFor(s => s.DueDate).NotEmpty().WithMessage("purchase_order.DueDate_required");
        }
    }
}
