using System.Reflection;

namespace Common.WebApi.Utils.Extensions;

public static class MemberExtensions
{
    public static Type GetMemberType(this MemberInfo member)
    {
        if (member is FieldInfo field)
        {
            return field.FieldType;
        }
        else if (member is PropertyInfo property)
        {
            return property.PropertyType;
        }
        else if (member is MethodInfo method)
        {
            return method.ReturnType;
        }
        else if (member is EventInfo @event)
        {
            return @event.EventHandlerType!;
        }
        else
        {
            throw new NotSupportedException($"Unsupported member type: {member.MemberType}");
        }
    }
}