using System.Reflection;
using Common.WebApi.Utils;
using Common.WebApi.Utils.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.WebApi.Shared;

public class NullableEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        bool isReferenceType =
            TypeUtils.IsReference(context.Type) &&
            !TypeUtils.IsCLR(context.Type) &&
            !TypeUtils.IsMicrosoft(context.Type);

        if (!isReferenceType)
            return;

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

        MemberInfo[] members = context.Type
            .GetFields(bindingFlags)
            .Cast<MemberInfo>()
            .Concat(context.Type.GetProperties(bindingFlags))
            .ToArray();

        bool hasNullableEnumMembers = members.Any(x => TypeUtils.IsNullableEnum(x.GetMemberType()));

        if (!hasNullableEnumMembers)
            return;

        schema.Properties.Where(x => !x.Value.Nullable).ToList().ForEach(property =>
        {
            string name = property.Key;

            string[] possibleNames = [
                name,
                TextCaseUtils.ToPascalCase(name),
            ];

            MemberInfo? sourceMember = possibleNames
                .Select(n => context.Type.GetMember(n, bindingFlags).FirstOrDefault())
                .Where(x => x is not null)
                .FirstOrDefault();

            if (sourceMember is null)
                return;

            Type? sourceMemberType = sourceMember.GetMemberType();

            if (sourceMemberType is null || !TypeUtils.IsNullableEnum(sourceMemberType))
                return;

            if (property.Value.Reference is not null)
            {
                property.Value.Nullable = true;
                property.Value.AllOf = [
                    new() { Reference = property.Value.Reference }
                ];
                property.Value.Reference = null;
            }
        });
    }
}