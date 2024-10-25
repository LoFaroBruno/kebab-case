using kebab_test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

public class AutoRouteConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            // Transform controller name to kebab-case
            var controllerName = new SlugifyParameterTransformer()
                .TransformOutbound(controller.ControllerName.Replace("Controller", ""));

            foreach (var action in controller.Actions)
            {
                // Transform action name to kebab-case
                var actionName = new SlugifyParameterTransformer()
                    .TransformOutbound(action.ActionName);

                // Filter only parameters that should be used in the route (e.g., path parameters)
                var pathParameters = action.Parameters
                    //.Where(p => IsRouteParameter(p)) // Custom logic to select path parameters
                    .Select(p => $"{{{ToKebabCase(p.ParameterName)}?}}");

                var parameters = string.Join("/", pathParameters);

                var routeTemplate = $"{controllerName}/{actionName}/{parameters}".TrimEnd('/');

                // Update existing selectors instead of clearing them
                foreach (var selector in action.Selectors)
                {
                    selector.AttributeRouteModel = new AttributeRouteModel(
                        new RouteAttribute(routeTemplate));
                }
            }
        }
    }

    private static bool IsRouteParameter(ParameterModel parameter)
    {
        // Define which parameters should be treated as path parameters.
        // Here, we assume parameters of type Guid are path parameters.
        return parameter.ParameterType == typeof(Guid);
    }

    private static string ToKebabCase(string input)
    {
        return string.Concat(input.Select((ch, i) =>
            i > 0 && char.IsUpper(ch) ? "-" + char.ToLower(ch) : char.ToLower(ch).ToString()));
    }
}
