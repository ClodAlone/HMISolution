 using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Media;
using System.ComponentModel;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Provides value editing services built into the <see cref="PropertyGrid"/> control.
  /// </summary>
  /// <remarks>This class supports the implementation of the PropertyGrid control; it is not
  /// intended for use from user code.</remarks>
  public class BuiltInEditor : ObjectWrappingEditor
  {
    private readonly BuiltInEditorStyleCollection _builtInEditorStyles;

    /// <summary>
    /// Initialises a new instance of the <see cref="BuiltInEditor"/> class.
    /// </summary>
    /// <param name="builtInEditorStyles">The styles to be applied to built-in editors.</param>
    public BuiltInEditor(BuiltInEditorStyleCollection builtInEditorStyles)
    {
      Invariant.ArgumentNotNull(builtInEditorStyles, "builtInEditorStyles");

      _builtInEditorStyles = builtInEditorStyles;
    }

    /// <summary>
    /// Indicates whether the editor can edit the value of the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>true if the editor can edit the value of this node; otherwise false.</returns>
    public override bool CanEdit(Node node)
    {
      return GetEditSettings(node).CanEditInPlace;
    }

    private static bool IsCollectionType(Type type)
    {
      return typeof(ICollection).IsAssignableFrom(type)
        || TypeUtilities.IsGenericCollection(type);
    }

    private static bool IsCollectionNode(Node node)
    {
      if (node.Value != null && node.Value is ICustomTypeDescriptor)
      {
        return false;
      }

      return IsCollectionType(node.PropertyType);
    }

    /// <summary>
    /// Gets the built-in editing behavior for the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>An <see cref="InPlaceEditing"/> describing the edit capabilities.</returns>
    public static InPlaceEditing GetEditSettings(Node node)
    {
      bool allowExpand;
      bool canEditInPlace = (GetEditorKey(node, true, out allowExpand) != null);

      if (IsCollectionNode(node))
      {
        allowExpand = true;
      }
      return new InPlaceEditing(canEditInPlace, allowExpand);
    }

    // TODO: this could probably be a HashSet
    private static readonly List<Type> _textEditableTypes = new List<Type>(new Type[]
    {
      typeof(int),
      typeof(uint),
      typeof(long),
      typeof(ulong),
      typeof(short),
      typeof(ushort),
      typeof(byte),
      typeof(sbyte),
      typeof(float),
      typeof(double),
      typeof(decimal),
      typeof(string)
    });

    /// <summary>
    /// Attaches the editor data template to the appropriate predefined template resource.
    /// </summary>
    /// <param name="factory">The FrameworkElementFactory representing the data template under construction.</param>
    /// <param name="node">The node which the template will edit.</param>
    protected override void SetContentTemplate(FrameworkElementFactory factory, Node node)
    {
      object editorKey = GetEditorKey(node);

      factory.SetResourceReference(ContentControl.ContentTemplateProperty, editorKey);
    }

    /// <summary>
    /// Propagates host style overrides to the data template visual tree.
    /// </summary>
    /// <param name="factory">The FrameworkElementFactory representing the data template under construction.</param>
    /// <param name="node">The node which the template will edit.</param>
    protected override void OnCustomizeTemplate(FrameworkElementFactory factory, Node node)
    {
      object editorKey = GetEditorKey(node);
      Style style = _builtInEditorStyles.FindStyle(editorKey);

      if (style != null)
      {
        factory.SetValue(HostStyleProperty, style);
      }
    }

    private static object GetEditorKey(Node node)
    {
      bool editable = node.CanWrite && GetEditSettings(node).CanEditInPlace;
      bool dummy;
      return GetEditorKey(node, editable, out dummy);
    }

    internal static object GetEditorKey(Node node, bool editable, out bool allowExpand)
    {
      Type propertyType = node.PropertyType;
      object propertyValue = node.Value;
      allowExpand = false;

      if (propertyValue != null && propertyValue is Many)
      {
        return PropertyGrid.ManyEditorKey;
      }

      if (IsCollectionNode(node))
      {
        return PropertyGrid.CollectionDisplayKey;
      }

      if (!editable)
      {
        return PropertyGrid.ReadOnlyDisplayKey;
      }

      // If there is an explicit TypeConverter on a property that would normally
      // get a default list-style TypeConverter, we want the explicit TypeConverter
      // to take precedence.
      if (propertyType != null && !node.Property.IsDefined(typeof(TypeConverterAttribute), true))
      {
        if (propertyType.IsEnum || propertyType == typeof(bool))
        {
          return PropertyGrid.ListSelectEditorKey;
        }
      }

      if (node != null)
      {
        TypeConverter converter = node.Property.Converter;
        if (converter == null && node.Value != null)
        {
          converter = TypeDescriptor.GetConverter(node.Value);
        }
        if (ReflectionUtilities.ShouldUseStandardValues(converter, propertyType))
        {
          if (converter.GetStandardValuesExclusive())
          {
            allowExpand = (converter is ExpandableObjectConverter);
            return PropertyGrid.ListSelectNoTextEntryEditorKey;
          }
          else
          {
            return PropertyGrid.ListSelectEditorKey;
          }
        }
      }

      if (propertyType != null)
      {
          if (propertyType == typeof(DateTime))
          {
              // At this level, we do not distinguish date/time or time properties.
              // This cannot be done on type and would therefore need the user
              // to provide a PropertyEditor.
              return PropertyGrid.DateEditorKey;
          }

          if (propertyType == typeof(TimeSpan))
          {
              return PropertyGrid.TimeSpanEditorKey;
          }

          if (propertyType == typeof(Color))
          {
              return PropertyGrid.ColorEditorKey;
          }

          if (_textEditableTypes.Contains(propertyType))
          {
              return PropertyGrid.SimpleTextEditorKey;
          }

          Type underlyingType = Nullable.GetUnderlyingType(propertyType);
          if (underlyingType != null && underlyingType.IsValueType)
          {
              return PropertyGrid.SimpleTextEditorKey;
          }
      }

      allowExpand = true;
      return null;
    }
  }
}
