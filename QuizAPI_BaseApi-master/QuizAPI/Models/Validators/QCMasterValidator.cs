using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class QCMasterValidator : AbstractValidator<QCMasterDto>
    {
        public QCMasterValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.QCMasterCode).NotEmpty().WithMessage("qcMaster.QCCode_required");
        }
    }
}
