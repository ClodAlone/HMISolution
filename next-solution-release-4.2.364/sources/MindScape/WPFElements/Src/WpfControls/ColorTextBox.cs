using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Selects a <see cref="Color"/> via text input.
  /// </summary>
  public class ColorTextBox : TextBox
  {
    static ColorTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorTextBox),
        new FrameworkPropertyMetadata(typeof(ColorTextBox)));
    }

    private bool _updateSelectedColorLock;

    private void UpdateSelectedColorFromText()
    {
      if (!_updateSelectedColorLock)
      {
        _updateTextLock = true;
        Color? color = ColorUtils.ColorFromHexString(Text);
        if (color != null)
        {
          SelectedColor = color.Value;
        }
        _updateTextLock = false;
      }
    }

    private bool _updateTextLock;

    private void UpdateTextFromSelectedColor()
    {
      if (!_updateTextLock)
      {
        _updateSelectedColorLock = true;
        Text = ColorUtils.ColorToHexString(SelectedColor, IsHashSymbolDisplayed, AlwaysShowAlphaHexEncoding, IsMinimizeHexEncodingEnabled);
        _updateSelectedColorLock = false;
      }
    }

    /// <summary>
    /// Called when a key is pressed while this <see cref="ColorTextBox"/> has focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (e.Key == Key.Enter)
      {
        if (!UpdateSelectedColorWhileEditing)
        {
          UpdateSelectedColorFromText();
        }
        UpdateTextFromSelectedColor();
        SelectionStart = Text.Length;
        SelectionLength = 0;
      }
    }

    /// <summary>
    /// Called when this <see cref="ColorTextBox"/> gains keyboard focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnGotKeyboardFocus(e);
      if (SelectAllOnEntry && Mouse.LeftButton == MouseButtonState.Released)
      {
        SelectAll();
      }
    }

    private bool _canSelectAll = false;

    /// <summary>
    /// Called when the left mouse button is pressed over this <see cref="ColorTextBox"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonDown(e);
      if (!IsFocused)
      {
        _canSelectAll = true;
      }
    }

    /// <summary>
    /// Called when the left mouse button is released over this <see cref="ColorTextBox"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonUp(e);
      if (SelectAllOnEntry && SelectionLength == 0 && _canSelectAll)
      {
        SelectAll();
      }
      _canSelectAll = false;
    }

    /// <summary>
    /// Called when this <see cref="ColorTextBox"/> loses focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnLostFocus(e);
      if (!UpdateSelectedColorWhileEditing)
      {
        UpdateSelectedColorFromText();
      }
      UpdateTextFromSelectedColor();
      try
      {
        SelectionLength = 0;
      }
      catch (Exception) { }
    }

    /// <summary>
    /// Called when the text of this <see cref="ColorTextBox"/> changes.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnTextChanged(TextChangedEventArgs e)
    {
      base.OnTextChanged(e);
      if (UpdateSelectedColorWhileEditing)
      {
        UpdateSelectedColorFromText();
      }
    }

    #region SelectedColor Property

    /// <summary>
    /// Gets or sets the selected <see cref="Color"/>.
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
      DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorTextBox),
      new FrameworkPropertyMetadata(OnSelectedColorChanged));

    private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ColorTextBox)d).OnSelectedColorChanged();
    }

    private void OnSelectedColorChanged()
    {
      UpdateTextFromSelectedColor();
    }

    #endregion // SelectedColor Property

    #region AlwaysShowAlphaHexEncoding Property

    /// <summary>
    /// Gets or sets whether or not alpha hex encoding should always be displayed when displaying the color as a hexadecimal value.
    /// If this property is false, the hexadecimal alpha value will not be displayed for opaque colors. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AlwaysShowAlphaHexEncodingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AlwaysShowAlphaHexEncoding
    {
      get { return (bool)GetValue(AlwaysShowAlphaHexEncodingProperty); }
      set { SetValue(AlwaysShowAlphaHexEncodingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AlwaysShowAlphaHexEncoding"/> property.
    /// </summary>
    public static readonly DependencyProperty AlwaysShowAlphaHexEncodingProperty =
      DependencyProperty.Register("AlwaysShowAlphaHexEncoding", typeof(bool), typeof(ColorTextBox),
      new FrameworkPropertyMetadata(false, OnAlwaysShowAlphaHexEncodingChanged));

    private static void OnAlwaysShowAlphaHexEncodingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ColorTextBox)d).OnAlwaysShowAlphaHexEncodingChanged();
    }

    private void OnAlwaysShowAlphaHexEncodingChanged()
    {
      UpdateTextFromSelectedColor();
    }

    #endregion // AlwaysShowAlphaHexEncoding Property

    #region IsHashSymbolDisplayed Property

    /// <summary>
    /// Gets or sets whether or not a hash (#) prefix should be displayed when displaying the color as a hexadecimal value. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsHashSymbolDisplayedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsHashSymbolDisplayed
    {
      get { return (bool)GetValue(IsHashSymbolDisplayedProperty); }
      set { SetValue(IsHashSymbolDisplayedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsHashSymbolDisplayed"/> property.
    /// </summary>
    public static readonly DependencyProperty IsHashSymbolDisplayedProperty =
      DependencyProperty.Register("IsHashSymbolDisplayed", typeof(bool), typeof(ColorTextBox),
      new FrameworkPropertyMetadata(false, OnIsHashSymbolDisplayedChanged));

    private static void OnIsHashSymbolDisplayedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ColorTextBox)d).OnIsHashSymbolDisplayedChanged();
    }

    private void OnIsHashSymbolDisplayedChanged()
    {
      UpdateTextFromSelectedColor();
    }

    #endregion // IsHashSymbolDisplayed Property

    #region IsMinimizeHexEncodingEnabled Property

    /// <summary>
    /// Gets or sets whether or not to minimize the hexadecimal color encoding when possible. This means if the red, green and blue
    /// color channels all produce a hex value with identical digits, the hex encoding will be minimized. For example #112233 would become #123.
    /// Note that this will only be used if the alpha hex encoding is not displayed. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsMinimizeHexEncodingEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsMinimizeHexEncodingEnabled
    {
      get { return (bool)GetValue(IsMinimizeHexEncodingEnabledProperty); }
      set { SetValue(IsMinimizeHexEncodingEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsMinimizeHexEncodingEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsMinimizeHexEncodingEnabledProperty =
      DependencyProperty.Register("IsMinimizeHexEncodingEnabled", typeof(bool), typeof(ColorTextBox),
      new FrameworkPropertyMetadata(false, OnIsMinimizeHexEncodingEnabledChanged));

    private static void OnIsMinimizeHexEncodingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ColorTextBox)d).OnIsMinimizeHexEncodingEnabledChanged();
    }

    private void OnIsMinimizeHexEncodingEnabledChanged()
    {
      UpdateTextFromSelectedColor();
    }

    #endregion // IsMinimizeHexEncodingEnabled Property

    #region SelectAllOnEntry Property

    /// <summary>
    /// Gets or sets whether this <see cref="ColorTextBox"/> should select all text when it gains focus. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectAllOnEntryProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool SelectAllOnEntry
    {
      get { return (bool)GetValue(SelectAllOnEntryProperty); }
      set { SetValue(SelectAllOnEntryProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectAllOnEntry"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectAllOnEntryProperty =
      DependencyProperty.Register("SelectAllOnEntry", typeof(bool), typeof(ColorTextBox),
      new FrameworkPropertyMetadata(false, OnSelectAllOnEntryChanged));

    private static void OnSelectAllOnEntryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ColorTextBox)d).OnSelectAllOnEntryChanged();
    }

    private void OnSelectAllOnEntryChanged()
    {
    }

    #endregion // SelectAllOnEntry Property

    #region UpdateSelectedColorWhileEditing Property

    /// <summary>
    /// Gets or sets whether or not the SelectedColor property is updated while the user is editing the text.
    /// If this property is set to false, the SelectedColor property will only update when this <see cref="ColorTextBox"/>
    /// loses focus, or the Enter key is pressed.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="UpdateSelectedColorWhileEditingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool UpdateSelectedColorWhileEditing
    {
      get { return (bool)GetValue(UpdateSelectedColorWhileEditingProperty); }
      set { SetValue(UpdateSelectedColorWhileEditingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="UpdateSelectedColorWhileEditing"/> property.
    /// </summary>
    public static readonly DependencyProperty UpdateSelectedColorWhileEditingProperty =
      DependencyProperty.Register("UpdateSelectedColorWhileEditing", typeof(bool), typeof(ColorTextBox),
      new FrameworkPropertyMetadata(true, OnUpdateSelectedColorWhileEditingChanged));

    private static void OnUpdateSelectedColorWhileEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ColorTextBox)d).OnUpdateSelectedColorWhileEditingChanged(e);
    }

    private void OnUpdateSelectedColorWhileEditingChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // UpdateSelectedColorWhileEditing Property
  }
}
