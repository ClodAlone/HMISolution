using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A button that can be added to a <see cref="DropDownColorPicker"/> to allow the user to select a particular <see cref="Color"/>.
  /// This is useful for an 'Automatic' color button or 'No color' button.
  /// </summary>
  public class ColorButton : ToggleButton
  {
    private bool _isCheckedLock;

    static ColorButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorButton),
        new FrameworkPropertyMetadata(typeof(ColorButton)));
    }

    /// <summary>
    /// Called when this <see cref="ColorButton"/> is checked.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnChecked(RoutedEventArgs e)
    {
      base.OnChecked(e);

      if (!_isCheckedLock)
      {
        SelectedColor = Color;
      }
    }

    /// <summary>
    /// Called when this <see cref="ColorButton"/> is unchecked.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnUnchecked(RoutedEventArgs e)
    {
      base.OnUnchecked(e);

      if (!_isCheckedLock)
      {
        IsChecked = true;
      }
    }

    /// <summary>
    /// Called when the left mouse button is released over this <see cref="ColorButton"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonUp(e);

      SelectedColor = Color;
    }

    private void UpdateIsChecked()
    {
      _isCheckedLock = true;
      IsChecked = Color.Equals(SelectedColor);
      _isCheckedLock = false;
    }

    #region Color Property

    /// <summary>
    /// Gets or sets the <see cref="Color"/> of this <see cref="ColorButton"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Color Color
    {
      get { return (Color)GetValue(ColorProperty); }
      set { SetValue(ColorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Color"/> property.
    /// </summary>
    public static readonly DependencyProperty ColorProperty =
      DependencyProperty.Register("Color", typeof(Color), typeof(ColorButton),
      new FrameworkPropertyMetadata(OnColorChanged));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ColorButton)d).OnColorChanged();
    }

    private void OnColorChanged()
    {
      UpdateIsChecked();
    }

    #endregion // Color Property

    #region SelectedColor Property

    /// <summary>
    /// Gets or sets the currently selected color of the color picker.
    /// This is for determining if this <see cref="ColorButton"/> is checked or not.
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
      DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorButton),
      new FrameworkPropertyMetadata(new Color(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));

    private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ColorButton)d).OnSelectedColorChanged();
    }

    private void OnSelectedColorChanged()
    {
      UpdateIsChecked();
    }

    #endregion // SelectedColor Property
  }
}
