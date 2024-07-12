using FluentValidation;
using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeModel>
    {
        public EmployeeValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("Name is required.")
                                 .MaximumLength(30).WithMessage("Name cannot exceed 30 characters.");

            RuleFor(e => e.Position).NotEmpty().WithMessage("Position is required.")
                                    .MaximumLength(30).WithMessage("Position cannot exceed 30 characters.");

            RuleFor(e => e.DepartmentId).GreaterThan(0).WithMessage("Invalid DepartmentId.");

            RuleFor(e => e.StartDate).NotEmpty().WithMessage("Start Date is required.");
        }
    }
}