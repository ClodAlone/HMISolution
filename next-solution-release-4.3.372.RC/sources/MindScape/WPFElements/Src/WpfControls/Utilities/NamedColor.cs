using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Associates a name with a color according to the WPF color naming standard.
  /// </summary>
  public class NamedColor
  {
    private readonly string _name;

    /// <summary>
    /// Gets the name of the color.
    /// </summary>
    public string Name
    {
      get { return _name; }
    }

    private readonly Color _color;

    /// <summary>
    /// Gets the color value.
    /// </summary>
    public Color Color
    {
      get { return _color; }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="NamedColor"/> class.
    /// </summary>
    /// <param name="name">The name of the color.</param>
    /// <param name="color">The color value.</param>
    public NamedColor(string name, Color color)
    {
      Invariant.ArgumentNotEmpty(name, "name"); // TODO: Do we really need this?

      _name = name;
      _color = color;
    }

    internal NamedColor(Color color, string name)
      : this(name, color)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedColor"/> class.
    /// </summary>
    /// <param name="color">The color value.</param>
    public NamedColor(Color color)
    {
      bool isNamed = NamedColor.ColorNames.TryGetValue(color, out _name);
      if (!isNamed)
      {
        _name = color.ToString();
      }
      _color = color;
    }

    private static List<NamedColor> _namedColors;
    private static Dictionary<Color, string> _colorNames;

    private static object _sharedInitialisationLock = new object();

    /// <summary>
    /// Gets a list of all named colors, as defined in the WPF <see cref="Colors"/> class.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> NamedColors
    {
      get
      {
        if (_namedColors == null)
        {
          lock (_sharedInitialisationLock)
          {
            if (_namedColors == null)
            {
              _namedColors = new List<NamedColor>();
              ForEachNamedColor(delegate(Color color, string name)
              {
                _namedColors.Add(new NamedColor(name, color));
              });
            }
          }
        }

        return _namedColors.AsReadOnly();
      }
    }

    /// <summary>
    /// Gets a mapping of colors to names, as defined in the WPF <see cref="Colors"/> class.
    /// </summary>
    /// <remarks>In rare cases a color value may have multiple names (e.g. Cyan and Aqua).  In such cases, 
    /// the ColorNames dictionary will contain only one of these names; which name is chosen is not defined.</remarks>
    public static Dictionary<Color, string> ColorNames
    {
      get
      {
        if (_colorNames == null)
        {
          lock (_sharedInitialisationLock)
          {
            if (_colorNames == null)
            {
              _colorNames = new Dictionary<Color, string>();
              ForEachNamedColor(delegate(Color color, string name)
              {
                try
                {
                  _colorNames.Add(color, name);
                }
                catch (ArgumentException)
                {
                  // there are some duplicates (fuschia/magenta and cyan/aqua) -- ignore as benign
                }
              });
            }
          }
        }

        return _colorNames;
      }
    }

    internal static string GetClosestColorName(Color color)
    {
      double closestScore = Double.MaxValue;
      string closestName = color.ToString();
      if (color.A < 20)
      {
        return "Transparent";
      }
      foreach (NamedColor namedColor in NamedColors)
      {
        double score = GetColorScore(namedColor.Color, color);
        if (score < closestScore && namedColor.Color.A == 255)
        {
          closestScore = score;
          closestName = namedColor.Name;
        }
      }
      foreach (NamedColor namedColor in StandardPalettes.OfficePalette)
      {
        double score = GetColorScore(namedColor.Color, color);
        if (score < closestScore)
        {
          closestScore = score;
          closestName = namedColor.Name.Split(',')[0];
        }
      }
      return closestName;
    }

    private static double GetColorScore(Color color1, Color color2)
    {
      double h1, s1, v1, h2, s2, v2;
      ColorUtils.ColorToHSV(color1, out h1, out s1, out v1);
      ColorUtils.ColorToHSV(color2, out h2, out s2, out v2);
      double rDiff = Math.Abs(color1.R - color2.R);
      double gDiff = Math.Abs(color1.G - color2.G);
      double bDiff = Math.Abs(color1.B - color2.B);
      double hDiff = Math.Abs(h1 - h2);
      double sDiff = Math.Abs(s1 - s2);
      double vDiff = Math.Abs(v1 - v2);
      if (Double.IsNaN(hDiff))
      {
        hDiff = 0;
      }
      return rDiff + gDiff + bDiff + hDiff + sDiff + vDiff;
    }

    private delegate void NamedColorAction(Color color, string name);

    private static void ForEachNamedColor(NamedColorAction action)
    {
      foreach (PropertyInfo namedColorProperty in typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static))
      {
        if (namedColorProperty.PropertyType == typeof(Color))
        {
          Color color = (Color)(namedColorProperty.GetValue(null, null));
          string name = namedColorProperty.Name;
          action(color, name);
        }
      }
    }
  }
}
