using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class SupplierValidator : AbstractValidator<SupplierDto>
    {
        public SupplierValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.SupplierCode).NotEmpty().WithMessage("supplier.SupplierCode_required");
            RuleFor(s => s.SupplierName).NotEmpty().WithMessage("supplier.SupplierName_required");
        }
    }
}
