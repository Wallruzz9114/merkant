using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.WebApi.Shared;

public class EnumDescriptionSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            string[] enumStringNames = Enum.GetNames(context.Type);
            IEnumerable<long> enumStringValues = Enum
                .GetValues(context.Type).Cast<int>().Select(i => Convert.ToInt64(i));

            IEnumerable<string> enumStringKeyValuePairs = enumStringNames
                .Zip(enumStringValues, (name, value) => $"{value} = {name}");

            OpenApiArray enumStringNamesAsOpenApiArray = [
                .. enumStringNames.Select(name => new OpenApiString(name)).ToArray()
            ];

            schema.Description = string.Join("\n", enumStringKeyValuePairs);
            schema.Extensions.Add("x-enum-varnames", enumStringNamesAsOpenApiArray);
            schema.Extensions.Add("x-enumNames", enumStringNamesAsOpenApiArray);
        }
    }
}