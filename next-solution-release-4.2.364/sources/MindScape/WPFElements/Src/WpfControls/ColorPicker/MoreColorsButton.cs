using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A button that can be added to a <see cref="DropDownColorPicker"/> to provide advanced color editing.
  /// </summary>
  public class MoreColorsButton : DropDownPopupItem
  {
    static MoreColorsButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MoreColorsButton),
        new FrameworkPropertyMetadata(typeof(MoreColorsButton)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoreColorsButton"/> class.
    /// </summary>
    public MoreColorsButton()
    {
      CommandBindings.Add(new CommandBinding(MoreColorsButtonCommands.AcceptColor, AcceptColor_Executed));
      CommandBindings.Add(new CommandBinding(MoreColorsButtonCommands.CancelColor, CancelColor_Executed));
    }

    private void AcceptColor_Executed(object sender, ExecutedRoutedEventArgs args)
    {
      CurrentColor = NewColor;
      IsDropDownOpen = false;
    }

    private void CancelColor_Executed(object sender, ExecutedRoutedEventArgs args)
    {
      NewColor = CurrentColor;
      IsDropDownOpen = false;
    }

    #region NewColor Property

    /// <summary>
    /// Gets or sets the color being edited by the user.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="NewColorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Color NewColor
    {
      get { return (Color)GetValue(NewColorProperty); }
      set { SetValue(NewColorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="NewColor"/> property.
    /// </summary>
    public static readonly DependencyProperty NewColorProperty =
      DependencyProperty.Register("NewColor", typeof(Color), typeof(MoreColorsButton),
      new FrameworkPropertyMetadata(OnNewColorChanged));

    private static void OnNewColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MoreColorsButton)d).OnNewColorChanged();
    }

    private void OnNewColorChanged()
    {
    }

    #endregion // NewColor Property

    #region CurrentColor Property

    /// <summary>
    /// Gets or sets the current un-edited color.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CurrentColorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Color CurrentColor
    {
      get { return (Color)GetValue(CurrentColorProperty); }
      set { SetValue(CurrentColorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CurrentColor"/> property.
    /// </summary>
    public static readonly DependencyProperty CurrentColorProperty =
      DependencyProperty.Register("CurrentColor", typeof(Color), typeof(MoreColorsButton),
      new FrameworkPropertyMetadata(new Color(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCurrentColorChanged));

    private static void OnCurrentColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MoreColorsButton)d).OnCurrentColorChanged();
    }

    private void OnCurrentColorChanged()
    {
      NewColor = CurrentColor;
    }

    #endregion // CurrentColor Property
  }

  /// <summary>
  /// Contains commands used with the <see cref="MoreColorsButton"/> control.
  /// </summary>
  public static class MoreColorsButtonCommands
  {
    /// <summary>
    /// A command for selecting the current color of the <see cref="MoreColorsButton"/>.
    /// </summary>
    public static readonly RoutedCommand AcceptColor = new RoutedCommand("AcceptColor", typeof(MoreColorsButton));

    /// <summary>
    /// A command for ignoring the current color of the <see cref="MoreColorsButton"/> and closing the drop down.
    /// </summary>
    public static readonly RoutedCommand CancelColor = new RoutedCommand("CancelColor", typeof(MoreColorsButton));
  }
}
