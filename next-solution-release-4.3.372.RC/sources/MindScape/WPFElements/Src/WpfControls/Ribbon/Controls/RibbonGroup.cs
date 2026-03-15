using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Mindscape.WpfElements;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A headered group of controls within a <see cref="RibbonTab"/>.
  /// </summary>
  public class RibbonGroup : HeaderedItemsControl
  {
    static RibbonGroup()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroup),
        new FrameworkPropertyMetadata(typeof(RibbonGroup)));
    }

    /// <summary>
    /// Prepares the specified element to display the specified item.
    /// </summary>
    /// <param name="element">The element used to display the specified item.</param>
    /// <param name="item">The item to display.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);

      FrameworkElement frameworkElement = element as FrameworkElement;
      if (frameworkElement != null)
      {
        ResourceKey styleKey = null;
        if (element is SplitButton)
        {
          styleKey = Ribbon.SplitButtonStyleKey;
        }
        else if (element is CheckBox)
        {
          styleKey = Ribbon.CheckBoxStyleKey;
        }
        else if (element is ToggleButton)
        {
          styleKey = Ribbon.ToggleButtonStyleKey;
        }
        else if (element is Button)
        {
          styleKey = Ribbon.ButtonStyleKey;
        }
        else if (element is NumericUpDown)
        {
          styleKey = Ribbon.NumericUpDownStyleKey;
        }
        else if (element is IntegerUpDown)
        {
          styleKey = Ribbon.IntegerUpDownStyleKey;
        }
        else if (element is ComboBox)
        {
          styleKey = Ribbon.ComboBoxStyleKey;
        }
        else if (element is DropDownColorPicker)
        {
          styleKey = Ribbon.DropDownColorPickerStyleKey;
        }
        else if (element is TextBlock)
        {
          styleKey = Ribbon.TextBlockStyleKey;
        }
        else if (element is Separator)
        {
          styleKey = Ribbon.SeparatorStyleKey;
        }

        if (styleKey != null)
        {
          frameworkElement.SetResourceReference(FrameworkElement.StyleProperty, styleKey);
        }
      }
    }

    internal bool CanShrink { get; set; }

    internal bool IsReadyToCollapse { get; set; }

    #region RibbonGroupCommand Property

    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> that will be executed by the ribbon group button.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RibbonGroupCommandProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ICommand RibbonGroupCommand
    {
      get { return (ICommand)GetValue(RibbonGroupCommandProperty); }
      set { SetValue(RibbonGroupCommandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RibbonGroupCommand"/> property.
    /// </summary>
    public static readonly DependencyProperty RibbonGroupCommandProperty =
      DependencyProperty.Register("RibbonGroupCommand", typeof(ICommand), typeof(RibbonGroup),
      new FrameworkPropertyMetadata(OnRibbonGroupCommandChanged));

    private static void OnRibbonGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RibbonGroup)d).OnRibbonGroupCommandChanged();
    }

    private void OnRibbonGroupCommandChanged()
    {
    }

    #endregion // RibbonGroupCommand Property

    #region IsExpanded Property

    /// <summary>
    /// Gets whether or not this <see cref="RibbonGroup"/> is expanded.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsExpandedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsExpanded
    {
      get { return (bool)GetValue(IsExpandedProperty); }
    }

    internal void SetIsExpanded(bool isExpanded)
    {
      SetValue(IsExpandedPropertyKey, isExpanded);
    }

    private static readonly DependencyPropertyKey IsExpandedPropertyKey =
        DependencyProperty.RegisterReadOnly("IsExpanded", typeof(bool), typeof(RibbonGroup), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsExpanded"/> property.
    /// </summary>
    public static readonly DependencyProperty IsExpandedProperty =
        IsExpandedPropertyKey.DependencyProperty;

    #endregion // IsExpanded Property

    #region CollapsedIcon Property

    /// <summary>
    /// Gets or sets the icon displayed when this <see cref="RibbonGroup"/> is collapsed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CollapsedIconProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    [TypeConverter(typeof(ObjectToIconTypeConverter))]
    public object CollapsedIcon
    {
      get { return GetValue(CollapsedIconProperty); }
      set { SetValue(CollapsedIconProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CollapsedIcon"/> property.
    /// </summary>
    public static readonly DependencyProperty CollapsedIconProperty =
      DependencyProperty.Register("CollapsedIcon", typeof(object), typeof(RibbonGroup),
      new FrameworkPropertyMetadata(OnCollapsedIconChanged));

    private static void OnCollapsedIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RibbonGroup)d).OnCollapsedIconChanged();
    }

    private void OnCollapsedIconChanged()
    {
    }

    #endregion // CollapsedIcon Property
  }
}
