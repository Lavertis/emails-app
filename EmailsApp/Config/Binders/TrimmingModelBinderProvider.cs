using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmailsApp.Config.Binders;

public class TrimmingModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (context.Metadata.ModelType == typeof(string))
            return new TrimmingModelBinder();
        return null;
    }
}