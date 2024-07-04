using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using WebAPI.Models;

namespace WebAPI.ModelBinders
{
    public class EmployeeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Employee))
            {
                return new BinderTypeModelBinder(typeof(EmployeeModelBinder));
            }

            return null;
        }
    }
}
