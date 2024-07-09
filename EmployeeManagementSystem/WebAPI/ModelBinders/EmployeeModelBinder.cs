using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebAPI.Models;

namespace PresentationLayer.ModelBinders
{
    public class EmployeeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var result = new Employee();

            var nameValueProviderResult = bindingContext.ValueProvider.GetValue("name");
            if (nameValueProviderResult != ValueProviderResult.None)
            {
                result.Name = nameValueProviderResult.FirstValue;
            }

            var positionValueProviderResult = bindingContext.ValueProvider.GetValue("position");
            if (positionValueProviderResult != ValueProviderResult.None)
            {
                result.Position = positionValueProviderResult.FirstValue;
            }

            var departmentIdValueProviderResult = bindingContext.ValueProvider.GetValue("departmentId");
            if (departmentIdValueProviderResult != ValueProviderResult.None && int.TryParse(departmentIdValueProviderResult.FirstValue, out var departmentId))
            {
                result.Department = DataService.GetDepartmentById(departmentId);
            }

            var startDateValueProviderResult = bindingContext.ValueProvider.GetValue("startDate");
            if (startDateValueProviderResult != ValueProviderResult.None && DateTime.TryParse(startDateValueProviderResult.FirstValue, out var startDate))
            {
                result.StartDate = startDate;
            }

            bindingContext.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;
        }
    }
}
