using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;

namespace ServerEditorWeb.Services;

/// <summary>
/// Describes a single property for rendering in the property grid.
/// </summary>
public class PropertyDescriptor
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Category { get; set; }
    public string? Description { get; set; }
    public Type PropertyType { get; set; } = typeof(object);
    public bool IsReadOnly { get; set; }
    public bool IsNullable { get; set; }
    public bool IsComplexType { get; set; }
    public bool IsCollection { get; set; }
    public PropertyInfo PropertyInfo { get; set; } = default!;
    public string[] EnumValues { get; set; } = [];
}

/// <summary>
/// Groups properties by category for the property grid.
/// </summary>
public class PropertyGroup
{
    public string Category { get; set; } = "";
    public List<PropertyDescriptor> Properties { get; set; } = [];
}

/// <summary>
/// Service that uses reflection to inspect objects and produce property descriptors.
/// </summary>
public class PropertyGridService
{
    private static readonly HashSet<Type> SimpleTypes =
    [
        typeof(string), typeof(bool), typeof(int), typeof(long), typeof(short),
        typeof(double), typeof(float), typeof(decimal),
        typeof(byte), typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong),
        typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan), typeof(Guid)
    ];

    /// <summary>
    /// Inspects the object and returns grouped property descriptors.
    /// </summary>
    public List<PropertyGroup> GetPropertyGroups(object? target)
    {
        if (target == null) return [];

        var type = target.GetType();
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .OrderBy(p => p.Name);

        var descriptors = new List<PropertyDescriptor>();

        foreach (var prop in props)
        {
            // Skip indexers
            if (prop.GetIndexParameters().Length > 0) continue;

            // Skip properties annotated with JsonExtensionData or Browsable(false)
            if (prop.GetCustomAttribute<System.Text.Json.Serialization.JsonExtensionDataAttribute>() != null)
                continue;
            if (prop.GetCustomAttribute<BrowsableAttribute>() is { Browsable: false })
                continue;

            var propType = prop.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propType);
            var isNullable = underlyingType != null || !propType.IsValueType;
            var effectiveType = underlyingType ?? propType;

            var descriptor = new PropertyDescriptor
            {
                Name = prop.Name,
                DisplayName = prop.GetCustomAttribute<DisplayAttribute>()?.Name
                    ?? prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                    ?? SplitCamelCase(prop.Name),
                Category = prop.GetCustomAttribute<CategoryAttribute>()?.Category ?? GetAutoCategory(prop, type),
                Description = prop.GetCustomAttribute<DescriptionAttribute>()?.Description
                    ?? prop.GetCustomAttribute<DisplayAttribute>()?.Description,
                PropertyType = effectiveType,
                IsReadOnly = !prop.CanWrite,
                IsNullable = isNullable,
                IsComplexType = IsComplex(effectiveType),
                IsCollection = IsCollectionType(effectiveType),
                PropertyInfo = prop,
            };

            if (effectiveType.IsEnum)
                descriptor.EnumValues = Enum.GetNames(effectiveType);

            descriptors.Add(descriptor);
        }

        // Group by category
        return descriptors
            .GroupBy(d => d.Category ?? "General")
            .OrderBy(g => g.Key == "General" ? 0 : 1)
            .ThenBy(g => g.Key)
            .Select(g => new PropertyGroup
            {
                Category = g.Key,
                Properties = g.OrderBy(p => p.Name).ToList()
            })
            .ToList();
    }

    public object? GetValue(object target, PropertyDescriptor descriptor)
    {
        try { return descriptor.PropertyInfo.GetValue(target); }
        catch { return null; }
    }

    public bool SetValue(object target, PropertyDescriptor descriptor, object? value)
    {
        if (descriptor.IsReadOnly) return false;
        try
        {
            var oldValue = descriptor.PropertyInfo.GetValue(target);
            descriptor.PropertyInfo.SetValue(target, value);
            OnPropertySet?.Invoke(target, descriptor.PropertyInfo, oldValue, value);
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Raised after a property is set. Parameters: target, property, oldValue, newValue.
    /// Used by UndoRedoService to record changes.
    /// </summary>
    public event Action<object, System.Reflection.PropertyInfo, object?, object?>? OnPropertySet;

    /// <summary>
    /// Tries to convert a string input value to the target property type.
    /// </summary>
    public object? ConvertValue(string? input, Type targetType)
    {
        if (input == null) return null;

        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlying == typeof(string)) return input;
        if (string.IsNullOrWhiteSpace(input) && Nullable.GetUnderlyingType(targetType) != null) return null;

        try
        {
            if (underlying == typeof(bool)) return bool.Parse(input);
            if (underlying == typeof(int)) return int.Parse(input);
            if (underlying == typeof(long)) return long.Parse(input);
            if (underlying == typeof(short)) return short.Parse(input);
            if (underlying == typeof(double)) return double.Parse(input);
            if (underlying == typeof(float)) return float.Parse(input);
            if (underlying == typeof(decimal)) return decimal.Parse(input);
            if (underlying == typeof(byte)) return byte.Parse(input);
            if (underlying == typeof(ushort)) return ushort.Parse(input);
            if (underlying == typeof(uint)) return uint.Parse(input);
            if (underlying == typeof(ulong)) return ulong.Parse(input);
            if (underlying == typeof(DateTime)) return DateTime.Parse(input);
            if (underlying == typeof(TimeSpan)) return TimeSpan.Parse(input);
            if (underlying == typeof(Guid)) return Guid.Parse(input);
            if (underlying.IsEnum) return Enum.Parse(underlying, input);

            // For object type (e.g., Variable.Value), try numeric then keep as string
            if (underlying == typeof(object))
            {
                if (double.TryParse(input, out var d)) return d;
                if (bool.TryParse(input, out var b)) return b;
                return input;
            }

            return Convert.ChangeType(input, underlying);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Creates a new default instance of a complex type (for nullable sub-objects).
    /// </summary>
    public object? CreateInstance(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        try { return Activator.CreateInstance(underlying); }
        catch { return null; }
    }

    private static bool IsComplex(Type type)
    {
        if (SimpleTypes.Contains(type)) return false;
        if (type.IsEnum) return false;
        if (type == typeof(object)) return false;
        if (type == typeof(JsonElement)) return false;
        if (IsCollectionType(type)) return false;
        return type.IsClass || (type.IsValueType && !type.IsPrimitive);
    }

    private static bool IsCollectionType(Type type)
    {
        if (type == typeof(string)) return false;
        return type.IsArray
            || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
    }

    private static string GetAutoCategory(PropertyInfo prop, Type ownerType)
    {
        // Auto-categorise based on property type being a sub-object
        var effective = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        if (IsComplex(effective))
            return SplitCamelCase(prop.Name);

        return "General";
    }

    private static string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (i > 0 && char.IsUpper(input[i]) && !char.IsUpper(input[i - 1]))
                result.Append(' ');
            result.Append(input[i]);
        }
        return result.ToString();
    }
}
