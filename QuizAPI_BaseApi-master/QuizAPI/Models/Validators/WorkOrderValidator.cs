using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class WorkOrderValidator : AbstractValidator<WorkOrderDto>
    {
        public WorkOrderValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(s => s.WoCode)
                .NotEmpty().WithMessage("work_order.WoCode_required")
                .Length(12).WithMessage("work_order.WoCode_length")
            ;
            RuleFor(s => s.OrderQty)
                .NotEmpty().WithMessage("work_order.OrderQty_required")
                .GreaterThan(0).WithMessage("work_order.OrderQty_GreaterThan_0")
            ;
            RuleFor(s => s.StartDate)
                .NotEmpty().WithMessage("work_order.StartDate_required")
                .Must(date => date != default(DateTime)).WithMessage("work_order.StartDate_invalid")
            ;
        }
    }
}
