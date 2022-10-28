using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class StaffValidator : AbstractValidator<StaffDto>
    {
        public StaffValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.StaffCode).NotEmpty().WithMessage("staff.StaffCode_required");
            RuleFor(s => s.StaffName).NotEmpty().WithMessage("staff.StaffName_required");
        }
    }
}
