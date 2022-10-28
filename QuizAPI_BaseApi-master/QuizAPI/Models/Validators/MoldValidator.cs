using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class MoldValidator : AbstractValidator<MoldDto>
    {
        public MoldValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.MoldSerial).NotEmpty().WithMessage("mold.MoldSerial_required");
            RuleFor(s => s.MoldCode).NotEmpty().WithMessage("mold.MoldCode_required");
            RuleFor(s => s.Model).NotEmpty().WithMessage("mold.Model_required");
            RuleFor(s => s.Inch).NotEmpty().WithMessage("mold.Inch_required");
            RuleFor(s => s.MoldType).NotEmpty().WithMessage("mold.MoldType_required");
            RuleFor(s => s.MachineType).NotEmpty().WithMessage("mold.MachineType_required");
            RuleFor(s => s.Cabity).NotEmpty().WithMessage("mold.Cabity_required");
            RuleFor(s => s.ETAStatus).NotEmpty().WithMessage("mold.ETAStatus_required");
            RuleFor(s => s.ETADate).NotEmpty().WithMessage("mold.ETADate_required");
        }
    }
}
