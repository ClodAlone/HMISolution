using System;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Provides access to property metadata.
  /// </summary>
  public interface IPropertyInfo : ICustomAttributeProvider
  {
    /// <summary>
    /// Gets the attributes for the property.
    /// </summary>
    PropertyAttributes Attributes { get; }

    /// <summary>
    /// Gets a value indicating whether the property can be read.
    /// </summary>
    bool CanRead { get; }

    /// <summary>
    /// Gets a value indicating whether the property can be written to.
    /// </summary>
    bool CanWrite { get; }

    /// <summary>
    /// Gets the class that declares this property.
    /// </summary>
    Type DeclaringType { get; }

    /// <summary>
    /// Returns an array of all the index parameters for the property.
    /// </summary>
    /// <returns>An array containing index parameter metadata.</returns>
    ParameterInfo[] GetIndexParameters();

    /// <summary>
    /// Returns the value of the property.
    /// </summary>
    /// <param name="obj">The object whose property value will be returned.</param>
    /// <param name="invokeAttr">The invocation attribute.</param>
    /// <param name="binder">A reflection binder, or null to use the default binder.</param>
    /// <param name="index">The index values for indexed properties, or null for 
    /// non-indexed properties.</param>
    /// <param name="culture">The culture to use for localization.</param>
    /// <returns>The property value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Attr", Justification="Consistency with PropertyInfo")]
    object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the display name of the property.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the category of the property.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Gets the description of the property.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets whether the property should be displayed in the grid.
    /// </summary>
    bool IsBrowsable { get; }

    /// <summary>
    /// Gets the type of this property.
    /// </summary>
    Type PropertyType { get; }

    /// <summary>
    /// Sets the value of the property.
    /// </summary>
    /// <param name="obj">The object whose property value will be set.</param>
    /// <param name="value">The value to which to set the property.</param>
    /// <param name="invokeAttr">The invocation attribute.</param>
    /// <param name="binder">A reflection binder, or null to use the default binder.</param>
    /// <param name="index">The index values for indexed properties, or null for 
    /// non-indexed properties.</param>
    /// <param name="culture">The culture to use for localization.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Attr", Justification = "Consistency with PropertyInfo")]
    void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

    /// <summary>
    /// Gets the type converter for this property.
    /// </summary>
    /// <remarks>This may return null if no type converter is defined for the property.</remarks>
    TypeConverter Converter { get; }

    /// <summary>
    /// Gets a <see cref="PropertyInfo"/> representing this property.
    /// </summary>
    /// <remarks>This member is provided primarily for compatibility.  Applications 
    /// should use IPropertyInfo members to access metadata unless they must interface to
    /// code that expects a <see cref="PropertyInfo"/>.</remarks>
    PropertyInfo AsPropertyInfo { get; }

    /// <summary>
    /// Gets a <see cref="PropertyDescriptor"/> representing this property.
    /// </summary>
    /// <remarks>This member is provided primarily for compatibility.  Applications 
    /// should use IPropertyInfo members to access metadata unless they must interface to
    /// code that expects a <see cref="PropertyDescriptor"/>.</remarks>
    PropertyDescriptor AsPropertyDescriptor { get; }
  }
}
