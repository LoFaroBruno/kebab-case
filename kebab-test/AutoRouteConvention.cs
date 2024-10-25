using kebab_test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

public class AutoRouteConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var controllerName = ToKebabCase(controller.ControllerName);

            foreach (var action in controller.Actions)
            {
                var actionName = ToKebabCase(action.ActionName);

                foreach(var p in action.Parameters)
                {
                    p.ParameterName = ToKebabCase(p.ParameterName);
                }

                var pathParameters = action.Parameters
                    .Select(p => $"{{{p.ParameterName}}}");

                var parameters = string.Join("/", pathParameters);

                var routeTemplate = $"{controllerName}/{actionName}/{parameters}";

                foreach (var selector in action.Selectors)
                {
                    selector.AttributeRouteModel = new AttributeRouteModel(
                        new RouteAttribute(routeTemplate));
                }
            }
        }
    }

    private static string ToKebabCase(string input)
    {
        return string.Concat(input.Select((ch, i) =>
            i > 0 && char.IsUpper(ch) ? "-" + char.ToLower(ch) : char.ToLower(ch).ToString()));
    }
}
