namespace Common.EventBus.Utils;

public static class GenericTypeExtensions
{
    /// <summary>
    /// This method gets the generic type name of a type
    /// </summary>
    /// <param name="type">The type</param>
    /// <returns>The type name as a <see cref="string"/> </returns>
    public static string GetGenericTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            string genericTypes = string
                .Join(",", type.GetGenericArguments()
                .Select(t => t.Name)
                .ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    /// <summary>
    /// This method gets the generic type name of an object
    /// </summary>
    /// <param name="object">The object</param>
    /// <returns>The type name as a <see cref="string"/> </returns>
    public static string GetGenericTypeName(this object @object) =>
        @object.GetType().GetGenericTypeName();
}