using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A color picker control that uses color sliders to modify the individual color channels (red, green, blue, alpha).
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class ChannelColorPicker : NotifyingColorPickerBase
  {
    static ChannelColorPicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChannelColorPicker),
        new FrameworkPropertyMetadata(typeof(ChannelColorPicker)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelColorPicker"/>.
    /// </summary>
    public ChannelColorPicker()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing
    }

    #region IsRgbVisible Property

    /// <summary>
    /// Gets or sets whether the RGB channel editors are visible. The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsRgbVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsRgbVisible
    {
      get { return (bool)GetValue(IsRgbVisibleProperty); }
      set { SetValue(IsRgbVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsRgbVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsRgbVisibleProperty =
      DependencyProperty.Register("IsRgbVisible", typeof(bool), typeof(ChannelColorPicker),
      new FrameworkPropertyMetadata(true, OnIsRgbVisibleChanged));

    private static void OnIsRgbVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChannelColorPicker)d).OnIsRgbVisibleChanged();
    }

    private void OnIsRgbVisibleChanged()
    {
    }

    #endregion // IsRgbVisible Property

    #region IsHsvVisible Property

    /// <summary>
    /// Gets or sets whether the HSV channel editors are visible. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsHsvVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsHsvVisible
    {
      get { return (bool)GetValue(IsHsvVisibleProperty); }
      set { SetValue(IsHsvVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsHsvVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsHsvVisibleProperty =
      DependencyProperty.Register("IsHsvVisible", typeof(bool), typeof(ChannelColorPicker),
      new FrameworkPropertyMetadata(false, OnIsHsvVisibleChanged));

    private static void OnIsHsvVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChannelColorPicker)d).OnIsHsvVisibleChanged();
    }

    private void OnIsHsvVisibleChanged()
    {
    }

    #endregion // IsHsvVisible Property

    #region IsAlphaVisible Property

    /// <summary>
    /// Gets or sets whether the Alpha channel editor is visible. The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsAlphaVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsAlphaVisible
    {
      get { return (bool)GetValue(IsAlphaVisibleProperty); }
      set { SetValue(IsAlphaVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsAlphaVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsAlphaVisibleProperty =
      DependencyProperty.Register("IsAlphaVisible", typeof(bool), typeof(ChannelColorPicker),
      new FrameworkPropertyMetadata(true, OnIsAlphaVisibleChanged));

    private static void OnIsAlphaVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChannelColorPicker)d).OnIsAlphaVisibleChanged();
    }

    private void OnIsAlphaVisibleChanged()
    {
    }

    #endregion // IsAlphaVisible Property
  }
}
