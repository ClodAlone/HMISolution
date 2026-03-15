using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Infralution.Licensing;
using System.ComponentModel;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A color picker control consisting of a customizable summary and drop-down detail pane.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class DropDownColorPicker : ColorPicker
  {
    static DropDownColorPicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownColorPicker),
        new FrameworkPropertyMetadata(typeof(DropDownColorPicker)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DropDownColorPicker"/> class.
    /// </summary>
    public DropDownColorPicker()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing
    }

    /// <summary>
    /// Called when the selected color changes.
    /// </summary>
    protected override void OnSelectedColorChanged()
    {
      base.OnSelectedColorChanged();

      if (!StaysOpen)
      {
        IsDropDownOpen = false;
      }
    }

    #region HeaderTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the header part of the <see cref="DropDownColorPicker"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HeaderTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate HeaderTemplate
    {
      get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
      set { SetValue(HeaderTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HeaderTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
      DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DropDownColorPicker),
      new FrameworkPropertyMetadata());

    #endregion // HeaderTemplate Property

    #region DropDownTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the drop down panel.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DropDownTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate DropDownTemplate
    {
      get { return (DataTemplate)GetValue(DropDownTemplateProperty); }
      set { SetValue(DropDownTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DropDownTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty DropDownTemplateProperty =
      DependencyProperty.Register("DropDownTemplate", typeof(DataTemplate), typeof(DropDownColorPicker),
      new FrameworkPropertyMetadata());

    #endregion // DropDownTemplate Property

    #region StaysOpen Property

    /// <summary>
    /// Gets or sets whether or not this <see cref="DropDownColorPicker"/> stays open after a color has been selected.
    /// The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StaysOpenProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool StaysOpen
    {
      get { return (bool)GetValue(StaysOpenProperty); }
      set { SetValue(StaysOpenProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StaysOpen"/> property.
    /// </summary>
    public static readonly DependencyProperty StaysOpenProperty =
      DependencyProperty.Register("StaysOpen", typeof(bool), typeof(DropDownColorPicker),
      new FrameworkPropertyMetadata());

    #endregion // StaysOpen Property

    #region IsDropDownOpen Property

    /// <summary>
    /// Gets or sets whether or not the drop down panel is open..
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsDropDownOpenProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsDropDownOpen
    {
      get { return (bool)GetValue(IsDropDownOpenProperty); }
      set { SetValue(IsDropDownOpenProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsDropDownOpen"/> property.
    /// </summary>
    public static readonly DependencyProperty IsDropDownOpenProperty =
      DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DropDownColorPicker),
      new FrameworkPropertyMetadata());

    #endregion // IsDropDownOpen Property
  }
}
