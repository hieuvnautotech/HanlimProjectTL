using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class BuyerValidator : AbstractValidator<BuyerDto>
    {
        public BuyerValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.BuyerCode).NotEmpty().WithMessage("buyer.BuyerCode_required");
            RuleFor(s => s.BuyerName).NotEmpty().WithMessage("buyer.BuyerName_required");
        }
    }
}
