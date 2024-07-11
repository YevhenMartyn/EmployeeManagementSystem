using FluentValidation;
using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Validators
{
    public class DepartmentValidator : AbstractValidator<DepartmentModel>
    {
        public DepartmentValidator()
        {
            RuleFor(d => d.Name).NotEmpty().WithMessage("Name is required.")
                                .MaximumLength(30).WithMessage("Name cannot exceed 30 characters.");
        }
    }
}