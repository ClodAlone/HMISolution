using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Diagnostics;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A base class for color pickers that provide individual color channel editing.
  /// </summary>
  public abstract class NotifyingColorPickerBase : Control
  {
    private bool _colorLock;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyingColorPickerBase"/> class.
    /// </summary>
    public NotifyingColorPickerBase()
    {
      SetupChannelChangeNotification();
    }

    #region SelectedColor Property

    /// <summary>
    /// Gets or sets the current <see cref="Color"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedColorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Color SelectedColor
    {
      get { return (Color)GetValue(SelectedColorProperty); }
      set { SetValue(SelectedColorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedColor"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedColorProperty =
      DependencyProperty.Register("SelectedColor", typeof(Color), typeof(NotifyingColorPickerBase),
      new FrameworkPropertyMetadata(OnSelectedColorChanged));

    private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NotifyingColorPickerBase b = d as NotifyingColorPickerBase;
      if (!b._colorLock)
      {
        b.SyncNotifyingColorToSelectedColor();
        b.OnSelectedColorChanged();
      }
    }

    /// <summary>
    /// Called when the selected color changes.
    /// </summary>
    protected virtual void OnSelectedColorChanged() { }

    #endregion // SelectedColor Property

    #region NotifyingColor Property

    /// <summary>
    /// Gets a <see cref="NotifyingColor"/> which provides support for changing individual color channels of the selected color.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="NotifyingColorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public NotifyingColor NotifyingColor
    {
      get { return (NotifyingColor)GetValue(NotifyingColorProperty); }
    }

    private static readonly DependencyPropertyKey NotifyingColorPropertyKey =
        DependencyProperty.RegisterReadOnly("NotifyingColor", typeof(NotifyingColor), typeof(NotifyingColorPickerBase), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="NotifyingColor"/> property.
    /// </summary>
    public static readonly DependencyProperty NotifyingColorProperty =
        NotifyingColorPropertyKey.DependencyProperty;

    private void SetupChannelChangeNotification()
    {
      NotifyingColor notifyingColor = new NotifyingColor();
      notifyingColor.SetFromColor(SelectedColor);
      notifyingColor.PropertyChanged += new PropertyChangedEventHandler(NotifyingColor_PropertyChanged);
      SetValue(NotifyingColorPropertyKey, notifyingColor);
    }

    private void NotifyingColor_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      SelectedColor = NotifyingColor.ToColor();
      /*if (!_colorLock)
      {
        _colorLock = true;
        
        
        _colorLock = false;
      }*/
      if (H != NotifyingColor.H)
      {
        H = NotifyingColor.H;
      }
      if (S != NotifyingColor.S)
      {
        S = NotifyingColor.S;
      }
      if (V != NotifyingColor.V)
      {
        V = NotifyingColor.V;
      }
    }

    private void SyncNotifyingColorToSelectedColor()
    {
      if (!_colorLock)
      {
        _colorLock = true;
        NotifyingColor.SetFromColor(SelectedColor);
        _colorLock = false;
      }
      if (H != NotifyingColor.H) // These if statements are used to prevent blitzing any bindings while loading the control.
      {
        H = NotifyingColor.H;
      }
      if (S != NotifyingColor.S)
      {
        S = NotifyingColor.S;
      }
      if (V != NotifyingColor.V)
      {
        V = NotifyingColor.V;
      }
    }

    #endregion // NotifyingColor Property

    #region H Property

    /// <summary>
    /// Gets or sets the hue of the selected color.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double H
    {
      get { return (double)GetValue(HProperty); }
      set { SetValue(HProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="H"/> property.
    /// </summary>
    public static readonly DependencyProperty HProperty =
      DependencyProperty.Register("H", typeof(double), typeof(NotifyingColorPickerBase),
      new FrameworkPropertyMetadata(OnHChanged));

    private static void OnHChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NotifyingColorPickerBase)d).OnHChanged();
    }

    private void OnHChanged()
    {
      if (!_colorLock)
      {
        _colorLock = true;
        NotifyingColor.H = H;
        _colorLock = false;
      }
    }

    #endregion // H Property

    #region S Property

    /// <summary>
    /// Gets or sets the saturation of the selected color.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double S
    {
      get { return (double)GetValue(SProperty); }
      set { SetValue(SProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="S"/> property.
    /// </summary>
    public static readonly DependencyProperty SProperty =
      DependencyProperty.Register("S", typeof(double), typeof(NotifyingColorPickerBase),
      new FrameworkPropertyMetadata(OnSChanged));

    private static void OnSChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NotifyingColorPickerBase)d).OnSChanged();
    }

    private void OnSChanged()
    {
      if (!_colorLock)
      {
        _colorLock = true;
        NotifyingColor.S = S;
        _colorLock = false;
      }
    }

    #endregion // S Property

    #region V Property

    /// <summary>
    /// Gets or sets the value (brightness) of the selected color.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="VProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double V
    {
      get { return (double)GetValue(VProperty); }
      set { SetValue(VProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="V"/> property.
    /// </summary>
    public static readonly DependencyProperty VProperty =
      DependencyProperty.Register("V", typeof(double), typeof(NotifyingColorPickerBase),
      new FrameworkPropertyMetadata(OnVChanged));

    private static void OnVChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NotifyingColorPickerBase)d).OnVChanged();
    }

    private void OnVChanged()
    {
      if (!_colorLock)
      {
        _colorLock = true;
        NotifyingColor.V = V;
        _colorLock = false;
      }
    }

    #endregion // V Property
  }
}
