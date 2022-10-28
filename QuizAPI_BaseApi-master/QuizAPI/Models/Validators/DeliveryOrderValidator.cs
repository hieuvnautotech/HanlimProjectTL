using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class DeliveryOrderValidator : AbstractValidator<DeliveryOrderDto>
    {
        public DeliveryOrderValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(s => s.DoCode)
                .NotEmpty().WithMessage("delivery_order.DoCode_required")
                .Length(12).WithMessage("delivery_order.DoCode_length")
            ;
            RuleFor(s => s.OrderQty)
                .NotEmpty().WithMessage("delivery_order.OrderQty_required")
                .GreaterThan(0).WithMessage("delivery_order.OrderQty_GreaterThan_0")
            ;
            RuleFor(s => s.ETDLoad)
                .NotEmpty().WithMessage("delivery_order.ETDLoad_required")
                .Must(date => date != default(DateTime)).WithMessage("delivery_order.ETDLoad_invalid")
            ;
            RuleFor(s => s.DeliveryTime)
                .NotEmpty().WithMessage("delivery_order.DeliveryTime_required")
                .Must(date => date != default(DateTime)).WithMessage("delivery_order.DeliveryTime_invalid")
                .GreaterThan(m => m.ETDLoad.Value).WithMessage("delivery_order.DeliveryTime_GreaterThanETDLoad").When(m => m.ETDLoad.HasValue)
            ;
        }

        private bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default(DateTime));
        }
    }
}
