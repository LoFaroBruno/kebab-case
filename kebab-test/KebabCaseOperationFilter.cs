namespace kebab_test
{
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Linq;

    public class KebabCaseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Loop through all parameters and replace PascalCase with kebab-case versions
            var parameters = operation.Parameters.ToList();

            foreach (var param in parameters)
            {
                var kebabName = ToKebabCase(param.Name);
                if (kebabName != param.Name)
                {
                    // Remove the PascalCase version if it exists
                    operation.Parameters.Remove(param);

                    // Add the kebab-case parameter
                    /*operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = kebabName,
                        In = param.In,
                        Required = param.Required,
                        Schema = param.Schema,
                        Description = param.Description
                    });*/
                }
            }
        }

        // Helper method to convert PascalCase to kebab-case
        private static string ToKebabCase(string input)
        {
            return string.Concat(input.Select((ch, i) =>
                i > 0 && char.IsUpper(ch) ? "-" + char.ToLower(ch) : char.ToLower(ch).ToString()));
        }
    }

}


