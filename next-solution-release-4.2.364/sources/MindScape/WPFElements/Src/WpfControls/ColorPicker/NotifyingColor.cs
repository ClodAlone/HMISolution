using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Encapsulates a Color and provides notification of changes to individual
  /// channels in order to support WPF data binding.
  /// </summary>
  public class NotifyingColor : INotifyPropertyChanged
  {
    private Color _color = new Color();
    private double _h, _s, _v;
    private bool _rgbLock;
    private bool _hsvLock;

    /// <summary>
    /// Returns the color of this <see cref="NotifyingColor"/>.
    /// </summary>
    /// <returns>The color of this NotifyingColor.</returns>
    public Color ToColor()
    {
      return _color;
    }

    /// <summary>
    /// Sets this <see cref="NotifyingColor"/> to wrap the specified color.
    /// </summary>
    /// <param name="color">The color to wrap.</param>
    public void SetFromColor(Color color)
    {
      _color.A = color.A;
      _color.R = color.R;
      _color.G = color.G;
      _color.B = color.B;

      OnPropertyChanged("A");
      OnPropertyChanged("R");
      OnPropertyChanged("G");
      OnPropertyChanged("B");
      UpdateHSV();
    }

    private void UpdateRGB()
    {
      if (!_rgbLock)
      {
        _hsvLock = true;
        _color = ColorUtils.ColorFromHSV(H, S, V);
        R = _color.R;
        G = _color.G;
        B = _color.B;
        _hsvLock = false;
      }
    }

    private void UpdateHSV()
    {
      if (!_hsvLock)
      {
        _rgbLock = true;
        double h, s, v;
        ColorUtils.ColorToHSV(_color, out h, out s, out v);
        H = Double.IsNaN(h) ? 0 : h;
        S = s;
        V = v;
        _rgbLock = false;
      }
    }

    /// <summary>
    /// Gets or sets the alpha channel value of the color.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A")]
    public byte A
    {
      get { return _color.A; }
      set
      {
        if (_color.A != value)
        {
          _color.A = value;
          OnPropertyChanged("A");
        }
      }
    }

    /// <summary>
    /// Gets or sets the red channel value of the color.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "R")]
    public byte R
    {
      get { return _color.R; }
      set
      {
        _color.R = value;
        OnPropertyChanged("R");
        UpdateHSV();
      }
    }

    /// <summary>
    /// Gets or sets the green channel value of the color.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "G")]
    public byte G
    {
      get { return _color.G; }
      set
      {
        _color.G = value;
        OnPropertyChanged("G");
        UpdateHSV();
      }
    }

    /// <summary>
    /// Gets or sets the blue channel value of the color.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "B")]
    public byte B
    {
      get { return _color.B; }
      set
      {
        _color.B = value;
        OnPropertyChanged("B");
        UpdateHSV();
      }
    }

    /// <summary>
    /// Gets or sets the hue channel of the color.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "H")]
    public double H
    {
      get { return _h; }
      set
      {
        if (_h != value)
        {
          _h = value;
          UpdateRGB();
          OnPropertyChanged("H");
        }
      }
    }

    /// <summary>
    /// Gets or sets the saturation channel of the color.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "S")]
    public double S
    {
      get { return _s; }
      set
      {
        if (_s != value)
        {
          _s = value;
          UpdateRGB();
          OnPropertyChanged("S");
        }
      }
    }

    /// <summary>
    /// Gets or sets the value (brightness) channel of the color.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
    public double V
    {
      get { return _v; }
      set
      {
        if (_v != value)
        {
          _v = value;
          UpdateRGB();
          OnPropertyChanged("V");
        }
      }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property whose value has changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
