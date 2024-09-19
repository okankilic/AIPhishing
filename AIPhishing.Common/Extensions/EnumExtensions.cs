using System.ComponentModel;

namespace AIPhishing.Common.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var descriptionAttribute = value
            .GetType()
            .GetField(value.ToString())
            ?.GetCustomAttributes(false)
            .SingleOrDefault(attr => attr.GetType() == typeof(DescriptionAttribute));

        return descriptionAttribute == null 
            ? value.ToString() 
            : ((DescriptionAttribute)descriptionAttribute).Description;
    }
}