using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class StandarQCValidator : AbstractValidator<StandardQCDto>
    {
        public StandarQCValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.QCCode).NotEmpty().WithMessage("standardQC.QCCode_required");
        }
    }
}
