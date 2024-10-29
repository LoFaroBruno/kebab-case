using kebab_test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

public class AutoRouteConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var controllerName = ToKebabCase(controller.ControllerName);

            foreach (var action in controller.Actions)
            {
                if (action.Selectors.Any(s => s.AttributeRouteModel != null))
                    continue;

                var actionName = ToKebabCase(action.ActionName);

                var pathParameters = action.Parameters
                    .Where(p => !HasBindingAttribute(p))
                    .Select(p => $"{{{ToKebabCase(p.ParameterName)}?}}");

                var parameters = string.Join("/", pathParameters);

                var routeTemplate = $"{controllerName}/{actionName}/{parameters}".TrimEnd('/');

                foreach (var selector in action.Selectors)
                {
                    if (selector.AttributeRouteModel == null)
                    {
                        selector.AttributeRouteModel = new AttributeRouteModel(
                            new RouteAttribute(routeTemplate));
                    }
                }
            }
        }
    }

    private static bool HasBindingAttribute(ParameterModel parameter) =>
    parameter.Attributes.Any(attr => attr is IBindingSourceMetadata);

    private static bool IsRouteParameter(ParameterModel parameter)
    {
        return parameter.ParameterType == typeof(Guid);
    }

    private static string ToKebabCase(string input)
    {
        return string.Concat(input.Select((ch, i) =>
            i > 0 && char.IsUpper(ch) ? "-" + char.ToLower(ch) : char.ToLower(ch).ToString()));
    }
}
