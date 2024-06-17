namespace EmailsApp.Config.Binders;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

public class TrimmingModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult != ValueProviderResult.None && valueProviderResult.FirstValue is not null)
            bindingContext.Result = ModelBindingResult.Success(valueProviderResult.FirstValue.Trim());
        else
            bindingContext.Result = ModelBindingResult.Failed();

        return Task.CompletedTask;
    }
}