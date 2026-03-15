using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for selecting colors from a list or by freely mixing color channels.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class ColorPicker : Control
  {
    static ColorPicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker),
        new FrameworkPropertyMetadata(typeof(ColorPicker)));
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ColorPicker"/> class.
    /// </summary>
    public ColorPicker()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      SetValue(RecentColorsPropertyKey, new ObservableCollection<NamedColor>());
    }

    #region SelectedColor Property

    /// <summary>
    /// Gets or sets the currently selected color.
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
        DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
        new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));

    private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ColorPicker control = (ColorPicker)d;
      if (control != null)
      {
        if (!Colors.Transparent.Equals(control.SelectedColor) && !new Color().Equals(control.SelectedColor))
        {
          bool isNewColor = true;
          foreach (NamedColor color in control.Palette)
          {
            if (color.Color.Equals(control.SelectedColor))
            {
              isNewColor = false;
              break;
            }
          }
          if (isNewColor)
          {
            foreach (NamedColor color in control.RecentColors)
            {
              if (color.Color.Equals(control.SelectedColor))
              {
                isNewColor = false;
                break;
              }
            }
            if (isNewColor)
            {
              control.RecentColors.Insert(0, new NamedColor(control.SelectedColor));
            }
          }
        }
        control.OnSelectedColorChanged();
      }
    }

    /// <summary>
    /// Called when the selected color changes.
    /// </summary>
    protected virtual void OnSelectedColorChanged() { }

    #endregion // SelectedColor Property

    #region Palette Property

    /// <summary>
    /// Gets or sets the Palette.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PaletteProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public IList<NamedColor> Palette
    {
      get { return (IList<NamedColor>)GetValue(PaletteProperty); }
      set { SetValue(PaletteProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Palette"/> property.
    /// </summary>
    public static readonly DependencyProperty PaletteProperty =
      DependencyProperty.Register("Palette", typeof(IList<NamedColor>), typeof(ColorPicker),
      new FrameworkPropertyMetadata(StandardPalettes.OfficePalette));

    #endregion // Palette Property

    #region RecentColors Property

    /// <summary>
    /// Gets the collection of recently used user-defined colors.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RecentColorsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ObservableCollection<NamedColor> RecentColors
    {
      get { return (ObservableCollection<NamedColor>)GetValue(RecentColorsProperty); }
    }

    private static readonly DependencyPropertyKey RecentColorsPropertyKey =
        DependencyProperty.RegisterReadOnly("RecentColors", typeof(ObservableCollection<NamedColor>), typeof(ColorPicker), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="RecentColors"/> property.
    /// </summary>
    public static readonly DependencyProperty RecentColorsProperty =
        RecentColorsPropertyKey.DependencyProperty;

    #endregion // RecentColors Property
  }
}
