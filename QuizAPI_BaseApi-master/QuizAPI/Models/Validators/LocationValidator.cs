using FluentValidation;
using QuizAPI.Models.Dtos;

namespace QuizAPI.Models.Validators
{
    public class LocationValidator : AbstractValidator<LocationDto>
    {
        public LocationValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(s => s.LocationCode).NotEmpty().WithMessage("location.LocationCode_required");
            RuleFor(s => s.AreaId).NotNull().WithMessage("location.LocationCode_required").GreaterThan(0).WithMessage("location.LocationCode_required");
        }
    }
}
