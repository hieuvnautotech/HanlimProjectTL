using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class ProductValidator : AbstractValidator<ProductDto>
    {
        public ProductValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.MaterialCode).NotEmpty().WithMessage("product.productCode_required");
            RuleFor(s => s.Model).NotEmpty().WithMessage("product.model_required");
            RuleFor(s => s.ProductType).NotEmpty().WithMessage("product.ProductType_required");
            RuleFor(s => s.Inch).NotNull().GreaterThan(0)
                  .ScalePrecision(3,9)
                .WithMessage("product.InchFormatError");
        }
    }
}
