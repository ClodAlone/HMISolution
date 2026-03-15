using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for exposing application commands to the user in a way that is easy to discover and navigate.
  /// </summary>
  public class Ribbon : Selector
  {
    static Ribbon()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon),
        new FrameworkPropertyMetadata(typeof(Ribbon)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ribbon"/> control.
    /// </summary>
    public Ribbon()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing
    }

    /// <summary>
    /// Called when items are added or removed from this <see cref="Ribbon"/> control.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);

      if (Items != null && Items.Count > 0 && SelectedItem == null)
      {
        RibbonTab tab = Items[0] as RibbonTab;
        if (tab != null)
        {
          tab.IsSelected = true;
          SelectedItem = tab;
        }
      }
    }

    /// <summary>
    /// Prepares the specified element to display the specified item.
    /// </summary>
    /// <param name="element">The element used to display the specified item.</param>
    /// <param name="item">The item to display.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);

      RibbonTab tab = element as RibbonTab;
      if (tab != null)
      {
        tab.IsSelectedChanged += new EventHandler(Tab_IsSelectedChanged);
      }
    }

    private void Tab_IsSelectedChanged(object sender, EventArgs e)
    {
      RibbonTab selectedTab = SelectedItem as RibbonTab;
      if (selectedTab != null)
      {
        selectedTab.IsSelected = false;
      }
      SelectedItem = sender as RibbonTab;
    }

    #region LargeIcon Attached Property

    /// <summary>
    /// Gets the large icon of the given control.
    /// </summary>
    /// <param name="obj">The control to get the large icon from.</param>
    /// <returns>The large icon of the given control.</returns>
    [TypeConverter(typeof(ObjectToIconTypeConverter))]
    public static object GetLargeIcon(DependencyObject obj)
    {
      return obj.GetValue(LargeIconProperty);
    }

    /// <summary>
    /// Sets the large icon of the given control.
    /// </summary>
    /// <param name="obj">The control to set the large icon.</param>
    /// <param name="value">The large icon.</param>
    public static void SetLargeIcon(DependencyObject obj, object value)
    {
      obj.SetValue(LargeIconProperty, value);
    }

    /// <summary>
    /// Identifies the LargeIcon attached property.
    /// </summary>
    public static readonly DependencyProperty LargeIconProperty =
      DependencyProperty.RegisterAttached("LargeIcon", typeof(object), typeof(Ribbon));

    #endregion // LargeIcon Attached Property

    #region SmallIcon Attached Property

    /// <summary>
    /// Gets the small icon of the given control.
    /// </summary>
    /// <param name="obj">The control to get the small icon from.</param>
    /// <returns>The small icon of the given control.</returns>
    [TypeConverter(typeof(ObjectToIconTypeConverter))]
    public static object GetSmallIcon(DependencyObject obj)
    {
      return obj.GetValue(SmallIconProperty);
    }

    /// <summary>
    /// Sets the small icon of the given control.
    /// </summary>
    /// <param name="obj">The control to set the small icon.</param>
    /// <param name="value">The small icon.</param>
    public static void SetSmallIcon(DependencyObject obj, object value)
    {
      obj.SetValue(SmallIconProperty, value);
    }

    /// <summary>
    /// IDentifies the SmallIcon attached property.
    /// </summary>
    public static readonly DependencyProperty SmallIconProperty =
      DependencyProperty.RegisterAttached("SmallIcon", typeof(object), typeof(Ribbon));

    #endregion // SmallIcon Attached Property

    #region EditorSize Property

    /// <summary>
    /// Gets the current <see cref="RibbonEditorSize"/> of the given control.
    /// </summary>
    /// <param name="obj">The control to get the <see cref="RibbonEditorSize"/> from.</param>
    /// <returns>The <see cref="RibbonEditorSize"/> of the given control.</returns>
    public static RibbonEditorSize GetEditorSize(DependencyObject obj)
    {
      return (RibbonEditorSize)obj.GetValue(EditorSizeProperty);
    }

    internal static void SetEditorSize(DependencyObject obj, RibbonEditorSize value)
    {
      obj.SetValue(EditorSizePropertyKey, value);
    }

    private static readonly DependencyPropertyKey EditorSizePropertyKey =
      DependencyProperty.RegisterAttachedReadOnly("EditorSize", typeof(RibbonEditorSize), typeof(Ribbon),
      new PropertyMetadata(RibbonEditorSize.Medium));

    /// <summary>
    /// Identifies the EditorSize attached property.
    /// </summary>
    public static readonly DependencyProperty EditorSizeProperty =
        EditorSizePropertyKey.DependencyProperty;

    #endregion // EditorSize Property

    #region EditorHeader Attached Property

    /// <summary>
    /// Gets the header of the given control.
    /// </summary>
    /// <param name="obj">The control to get the header from.</param>
    /// <returns>The header of the given control.</returns>
    public static object GetEditorHeader(DependencyObject obj)
    {
      return obj.GetValue(EditorHeaderProperty);
    }

    /// <summary>
    /// Sets the header of the given control.
    /// </summary>
    /// <param name="obj">The control to set the header.</param>
    /// <param name="value">The header.</param>
    public static void SetEditorHeader(DependencyObject obj, object value)
    {
      obj.SetValue(EditorHeaderProperty, value);
    }

    /// <summary>
    /// Identifies the EditorHeader attached property.
    /// </summary>
    public static readonly DependencyProperty EditorHeaderProperty =
      DependencyProperty.RegisterAttached("EditorHeader", typeof(object), typeof(Ribbon));

    #endregion // EditorHeader Attached Property

    #region EditorWidth Attached Property

    /// <summary>
    /// Gets the width of the given control.
    /// </summary>
    /// <param name="obj">The control to get the width from.</param>
    /// <returns>The width of the given control.</returns>
    public static double GetEditorWidth(DependencyObject obj)
    {
      return (double)obj.GetValue(EditorWidthProperty);
    }

    /// <summary>
    /// Sets the width of the given control.
    /// </summary>
    /// <param name="obj">The control to set the width.</param>
    /// <param name="value">The width.</param>
    public static void SetEditorWidth(DependencyObject obj, double value)
    {
      obj.SetValue(EditorWidthProperty, value);
    }

    /// <summary>
    /// Identifies the EditorWidth attached property.
    /// </summary>
    public static readonly DependencyProperty EditorWidthProperty =
      DependencyProperty.RegisterAttached("EditorWidth", typeof(double), typeof(Ribbon), new PropertyMetadata(Double.NaN));

    #endregion // EditorWidth Attached Property

    #region Resource Keys

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="Button"/> controls within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey ButtonStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonButtonStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="ToggleButton"/> controls within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey ToggleButtonStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonToggleButtonStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="SplitButton"/> controls within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey SplitButtonStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonSplitButtonStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="NumericUpDown"/> controls within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey NumericUpDownStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonNumericUpDownStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="IntegerUpDown"/> controls within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey IntegerUpDownStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonIntegerUpDownStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="ComboBox"/> controls within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey ComboBoxStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonComboBoxStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="CheckBox"/> controls within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey CheckBoxStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonCheckBoxStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="DropDownColorPicker"/> controls within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey DropDownColorPickerStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonDropDownColorPickerStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="TextBlock"/> elements within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey TextBlockStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonTextBlockStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the style applied to <see cref="Separator"/> elements within a <see cref="Ribbon"/>.
    /// </summary>
    public static ResourceKey SeparatorStyleKey
    {
      get { return new ComponentResourceKey(typeof(Ribbon), "RibbonSeparatorStyle"); }
    }

    #endregion // Resource Keys
  }
}
