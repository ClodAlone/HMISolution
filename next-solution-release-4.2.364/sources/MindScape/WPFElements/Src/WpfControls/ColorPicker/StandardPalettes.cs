using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides standard palettes for use with the <see cref="ColorPicker"/> control.
  /// </summary>
  public static class StandardPalettes
  {
    private static ReadOnlyCollection<NamedColor> _officePalette;
    private static ReadOnlyCollection<NamedColor> _grayscalePalette;
    private static ReadOnlyCollection<NamedColor> _adjacencyPalette;
    private static ReadOnlyCollection<NamedColor> _anglesPalette;
    private static ReadOnlyCollection<NamedColor> _apexPalette;
    private static ReadOnlyCollection<NamedColor> _apothecaryPalette;
    private static ReadOnlyCollection<NamedColor> _aspectPalette;
    private static ReadOnlyCollection<NamedColor> _austinPalette;
    private static ReadOnlyCollection<NamedColor> _blackTiePalette;
    private static ReadOnlyCollection<NamedColor> _civicPalette;
    private static ReadOnlyCollection<NamedColor> _clarityPalette;
    private static ReadOnlyCollection<NamedColor> _compositePalette;
    private static ReadOnlyCollection<NamedColor> _concoursePalette;
    private static ReadOnlyCollection<NamedColor> _couturePalette;
    private static ReadOnlyCollection<NamedColor> _elementalPalette;
    private static ReadOnlyCollection<NamedColor> _equityPalette;
    private static ReadOnlyCollection<NamedColor> _essentialPalette;
    private static ReadOnlyCollection<NamedColor> _executivePalette;
    private static ReadOnlyCollection<NamedColor> _flowPalette;
    private static ReadOnlyCollection<NamedColor> _foundryPalette;
    private static ReadOnlyCollection<NamedColor> _gridPalette;
    private static ReadOnlyCollection<NamedColor> _hardcoverPalette;
    private static ReadOnlyCollection<NamedColor> _horizonPalette;
    private static ReadOnlyCollection<NamedColor> _medianPalette;
    private static ReadOnlyCollection<NamedColor> _metroPalette;
    private static ReadOnlyCollection<NamedColor> _modulePalette;
    private static ReadOnlyCollection<NamedColor> _newsprintPalette;
    private static ReadOnlyCollection<NamedColor> _opulentPalette;
    private static ReadOnlyCollection<NamedColor> _orielPalette;
    private static ReadOnlyCollection<NamedColor> _originPalette;
    private static ReadOnlyCollection<NamedColor> _paperPalette;
    private static ReadOnlyCollection<NamedColor> _perspectivePalette;
    private static ReadOnlyCollection<NamedColor> _pushpinPalette;
    private static ReadOnlyCollection<NamedColor> _slipstreamPalette;
    private static ReadOnlyCollection<NamedColor> _solsticePalette;
    private static ReadOnlyCollection<NamedColor> _technicPalette;
    private static ReadOnlyCollection<NamedColor> _thatchPalette;
    private static ReadOnlyCollection<NamedColor> _trekPalette;
    private static ReadOnlyCollection<NamedColor> _urbanPalette;
    private static ReadOnlyCollection<NamedColor> _vervePalette;
    private static ReadOnlyCollection<NamedColor> _waveformPalette;

    /// <summary>
    /// Gets a palette containing Office-style theme colors.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> OfficePalette
    {
      get
      {
        if (_officePalette == null)
        {
          BuildOfficePalette();
          /*IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 0, G = 0, B = 0 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 238, G = 236, B = 225 }, "Tan")); // EEECE1
          sample.Add(new NamedColor(new Color() { A = 255, R = 31, G = 73, B = 125 }, "Dark Blue")); // 1F497D
          sample.Add(new NamedColor(new Color() { A = 255, R = 79, G = 129, B = 189 }, "Blue")); // 4F81BD
          sample.Add(new NamedColor(new Color() { A = 255, R = 192, G = 80, B = 77 }, "Red")); // C0504D
          sample.Add(new NamedColor(new Color() { A = 255, R = 155, G = 187, B = 89 }, "Olive Green")); // 9BBB59
          sample.Add(new NamedColor(new Color() { A = 255, R = 128, G = 100, B = 162 }, "Purple")); // 8064A2
          sample.Add(new NamedColor(new Color() { A = 255, R = 75, G = 172, B = 198 }, "Aqua")); // 4BACC6
          sample.Add(new NamedColor(new Color() { A = 255, R = 247, G = 150, B = 70 }, "Orange")); // F79646
          _officePalette = BuildPalette(sample);*/
        }
        return _officePalette;
      }
    }

    // Office palettes:

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Grayscale' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> GrayscalePalette
    {
      get
      {
        if (_grayscalePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 248, G = 248, B = 248 }, "White")); // F8F8F8
          sample.Add(new NamedColor(new Color() { A = 255, R = 0, G = 0, B = 0 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 221, G = 221, B = 221 }, "Gray-25%")); // DDDDDD
          sample.Add(new NamedColor(new Color() { A = 255, R = 178, G = 178, B = 178 }, "Gray-25%")); // B2B2B2
          sample.Add(new NamedColor(new Color() { A = 255, R = 150, G = 150, B = 150 }, "Gray-50%")); // 969696
          sample.Add(new NamedColor(new Color() { A = 255, R = 128, G = 128, B = 128 }, "Gray-50%")); // 808080
          sample.Add(new NamedColor(new Color() { A = 255, R = 95, G = 95, B = 95 }, "Gray-80%")); // 5F5F5F
          sample.Add(new NamedColor(new Color() { A = 255, R = 77, G = 77, B = 77 }, "Gray-80%")); // 4D4D4D
          _grayscalePalette = BuildPalette(sample);
        }
        return _grayscalePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Adjacency' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> AdjacencyPalette
    {
      get
      {
        if (_adjacencyPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 47, G = 43, B = 32 }, "Brown")); // 2F2B20
          sample.Add(new NamedColor(new Color() { A = 255, R = 223, G = 220, B = 183 }, "Tan")); // DFDCB7
          sample.Add(new NamedColor(new Color() { A = 255, R = 103, G = 94, B = 71 }, "Brown")); // 675E47
          sample.Add(new NamedColor(new Color() { A = 255, R = 169, G = 165, B = 124 }, "Brown")); // A9A57C
          sample.Add(new NamedColor(new Color() { A = 255, R = 156, G = 190, B = 189 }, "Aqua")); // 9CBEBD
          sample.Add(new NamedColor(new Color() { A = 255, R = 210, G = 203, B = 108 }, "Gold")); // D2CB6C
          sample.Add(new NamedColor(new Color() { A = 255, R = 149, G = 163, B = 157 }, "Gray-50%")); // 95A39D
          sample.Add(new NamedColor(new Color() { A = 255, R = 200, G = 159, B = 93 }, "Tan")); // C89F5D
          sample.Add(new NamedColor(new Color() { A = 255, R = 177, G = 160, B = 137 }, "Tan")); // B1A089
          _adjacencyPalette = BuildPalette(sample);
        }
        return _adjacencyPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Angles' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> AnglesPalette
    {
      get
      {
        if (_anglesPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 205, G = 215, B = 217 }, "Ice Blue")); // CDD7D9
          sample.Add(new NamedColor(new Color() { A = 255, R = 67, G = 67, B = 66 }, "Gray-80%")); // 434342
          sample.Add(new NamedColor(new Color() { A = 255, R = 121, G = 123, B = 126 }, "Gray-50%")); // 797B7E
          sample.Add(new NamedColor(new Color() { A = 255, R = 249, G = 106, B = 27 }, "Orange")); // F96A1B
          sample.Add(new NamedColor(new Color() { A = 255, R = 8, G = 161, B = 217 }, "Turquoise")); // 08A1D9
          sample.Add(new NamedColor(new Color() { A = 255, R = 124, G = 152, B = 74 }, "Olive Green")); // 7C984A
          sample.Add(new NamedColor(new Color() { A = 255, R = 194, G = 173, B = 141 }, "Tan")); // C2AD8D
          sample.Add(new NamedColor(new Color() { A = 255, R = 80, G = 110, B = 148 }, "Blue-Gray")); // 506E94
          _anglesPalette = BuildPalette(sample);
        }
        return _anglesPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Apex' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> ApexPalette
    {
      get
      {
        if (_apexPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 201, G = 194, B = 209 }, "Lavender")); // C9C2D1
          sample.Add(new NamedColor(new Color() { A = 255, R = 105, G = 103, B = 109 }, "Gray-50%")); // 69676D
          sample.Add(new NamedColor(new Color() { A = 255, R = 206, G = 185, B = 102 }, "Tan")); // CEB966
          sample.Add(new NamedColor(new Color() { A = 255, R = 156, G = 176, B = 132 }, "Olive Green")); // 9CB084
          sample.Add(new NamedColor(new Color() { A = 255, R = 107, G = 177, B = 201 }, "Aqua")); // 6BB1C9
          sample.Add(new NamedColor(new Color() { A = 255, R = 101, G = 133, B = 207 }, "Blue")); // 6585CF
          sample.Add(new NamedColor(new Color() { A = 255, R = 126, G = 107, B = 201 }, "Lavender")); // 7E6BC9
          sample.Add(new NamedColor(new Color() { A = 255, R = 163, G = 121, B = 187 }, "Lavender")); // A379BB
          _apexPalette = BuildPalette(sample);
        }
        return _apexPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Apothecary' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> ApothecaryPalette
    {
      get
      {
        if (_apothecaryPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 236, G = 237, B = 209 }, "Tan")); // ECEDD1
          sample.Add(new NamedColor(new Color() { A = 255, R = 86, G = 75, B = 60 }, "Brown")); // 564B3C
          sample.Add(new NamedColor(new Color() { A = 255, R = 147, G = 162, B = 153 }, "Gray-50%")); // 93A299
          sample.Add(new NamedColor(new Color() { A = 255, R = 207, G = 84, B = 63 }, "Red")); // CF543F
          sample.Add(new NamedColor(new Color() { A = 255, R = 181, G = 174, B = 83 }, "Tan")); // B5AE53
          sample.Add(new NamedColor(new Color() { A = 255, R = 132, G = 128, B = 88 }, "Brown")); // 848058
          sample.Add(new NamedColor(new Color() { A = 255, R = 232, G = 181, B = 77 }, "Gold")); // E8B54D
          sample.Add(new NamedColor(new Color() { A = 255, R = 120, G = 108, B = 113 }, "Gray-50%")); // 786C71
          _apothecaryPalette = BuildPalette(sample);
        }
        return _apothecaryPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Aspect' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> AspectPalette
    {
      get
      {
        if (_aspectPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 227, G = 222, B = 209 }, "Tan")); // E3DED1
          sample.Add(new NamedColor(new Color() { A = 255, R = 50, G = 50, B = 50 }, "Gray-80%")); // 323232
          sample.Add(new NamedColor(new Color() { A = 255, R = 240, G = 127, B = 9 }, "Orange")); // F07F09
          sample.Add(new NamedColor(new Color() { A = 255, R = 159, G = 41, B = 54 }, "Red")); // 9F2936
          sample.Add(new NamedColor(new Color() { A = 255, R = 27, G = 88, B = 124 }, "Dark Blue")); // 1B587C
          sample.Add(new NamedColor(new Color() { A = 255, R = 78, G = 133, B = 66 }, "Dark Green")); // 4E8542
          sample.Add(new NamedColor(new Color() { A = 255, R = 96, G = 72, B = 120 }, "Dark Purple")); // 604878
          sample.Add(new NamedColor(new Color() { A = 255, R = 193, G = 152, B = 89 }, "Tan")); // C19859
          _aspectPalette = BuildPalette(sample);
        }
        return _aspectPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Austin' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> AustinPalette
    {
      get
      {
        if (_austinPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 202, G = 242, B = 120 }, "Light Green")); // CAF278
          sample.Add(new NamedColor(new Color() { A = 255, R = 62, G = 61, B = 45 }, "Brown")); // 3E3D2D
          sample.Add(new NamedColor(new Color() { A = 255, R = 148, G = 198, B = 0 }, "Green")); // 94C600
          sample.Add(new NamedColor(new Color() { A = 255, R = 113, G = 104, B = 90 }, "Brown")); // 71685A
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 103, B = 0 }, "Orange")); // FF6700
          sample.Add(new NamedColor(new Color() { A = 255, R = 144, G = 148, B = 101 }, "Olive Green")); // 909465
          sample.Add(new NamedColor(new Color() { A = 255, R = 149, G = 107, B = 67 }, "Brown")); // 956B43
          sample.Add(new NamedColor(new Color() { A = 255, R = 254, G = 160, B = 34 }, "Orange")); // FEA022
          _austinPalette = BuildPalette(sample);
        }
        return _austinPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Black Tie' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> BlackTiePalette
    {
      get
      {
        if (_blackTiePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 227, G = 220, B = 207 }, "Tan")); // E3DCCF
          sample.Add(new NamedColor(new Color() { A = 255, R = 70, G = 70, B = 74 }, "Gray-80%")); // 46464A
          sample.Add(new NamedColor(new Color() { A = 255, R = 111, G = 111, B = 116 }, "Gray-50%")); // 6F6F74
          sample.Add(new NamedColor(new Color() { A = 255, R = 167, G = 183, B = 137 }, "Olive Green")); // A7B789
          sample.Add(new NamedColor(new Color() { A = 255, R = 190, G = 174, B = 152 }, "Tan")); // BEAE98
          sample.Add(new NamedColor(new Color() { A = 255, R = 146, G = 169, B = 185 }, "Blue-Gray")); // 92A9B9
          sample.Add(new NamedColor(new Color() { A = 255, R = 156, G = 130, B = 101 }, "Brown")); // 9C8265
          sample.Add(new NamedColor(new Color() { A = 255, R = 141, G = 105, B = 116 }, "Brown")); // 8D6974
          _blackTiePalette = BuildPalette(sample);
        }
        return _blackTiePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Civic' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> CivicPalette
    {
      get
      {
        if (_civicPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 197, G = 209, B = 215 }, "Ice Blue")); // C5D1D7
          sample.Add(new NamedColor(new Color() { A = 255, R = 100, G = 107, B = 134 }, "Blue-Gray")); // 646B86
          sample.Add(new NamedColor(new Color() { A = 255, R = 209, G = 99, B = 73 }, "Red")); // D16349
          sample.Add(new NamedColor(new Color() { A = 255, R = 204, G = 180, B = 0 }, "Dark Yellow")); // CCB400
          sample.Add(new NamedColor(new Color() { A = 255, R = 140, G = 173, B = 174 }, "Teal")); // 8CADAE
          sample.Add(new NamedColor(new Color() { A = 255, R = 140, G = 123, B = 112 }, "Brown")); // 8C7B70
          sample.Add(new NamedColor(new Color() { A = 255, R = 143, G = 176, B = 140 }, "Green")); // 8FB08C
          sample.Add(new NamedColor(new Color() { A = 255, R = 209, G = 144, B = 73 }, "Orange")); // D19049
          _civicPalette = BuildPalette(sample);
        }
        return _civicPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Clarity' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> ClarityPalette
    {
      get
      {
        if (_clarityPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 41, G = 41, B = 52 }, "Indigo")); // 29293
          sample.Add(new NamedColor(new Color() { A = 255, R = 243, G = 242, B = 220 }, "Light Yellow")); // F3F2DC
          sample.Add(new NamedColor(new Color() { A = 255, R = 210, G = 83, B = 60 }, "Red")); // D2533C
          sample.Add(new NamedColor(new Color() { A = 255, R = 147, G = 162, B = 153 }, "Gray-50%")); // 93A299
          sample.Add(new NamedColor(new Color() { A = 255, R = 173, G = 143, B = 103 }, "Brown")); // AD8F67
          sample.Add(new NamedColor(new Color() { A = 255, R = 114, G = 96, B = 86 }, "Brown")); // 726056
          sample.Add(new NamedColor(new Color() { A = 255, R = 76, G = 90, B = 106 }, "Blue-Gray")); // 4C5A6A
          sample.Add(new NamedColor(new Color() { A = 255, R = 128, G = 141, B = 160 }, "Blue-Gray")); // 808DA0
          sample.Add(new NamedColor(new Color() { A = 255, R = 121, G = 70, B = 61 }, "Dark Red")); // 79463D
          _clarityPalette = BuildPalette(sample);
        }
        return _clarityPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Composite' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> CompositePalette
    {
      get
      {
        if (_compositePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 231, G = 236, B = 237 }, "Ice Blue")); // E7ECED
          sample.Add(new NamedColor(new Color() { A = 255, R = 91, G = 105, B = 115 }, "Blue-Gray")); // 5B6973
          sample.Add(new NamedColor(new Color() { A = 255, R = 152, G = 199, B = 35 }, "Lime")); // 98C723
          sample.Add(new NamedColor(new Color() { A = 255, R = 89, G = 176, B = 185 }, "Teal")); // 59B0B9
          sample.Add(new NamedColor(new Color() { A = 255, R = 222, G = 174, B = 0 }, "Gold")); // DEAE00
          sample.Add(new NamedColor(new Color() { A = 255, R = 183, G = 123, B = 180 }, "Lavender")); // B77BB4
          sample.Add(new NamedColor(new Color() { A = 255, R = 224, G = 119, B = 60 }, "Orange")); // E0773C
          sample.Add(new NamedColor(new Color() { A = 255, R = 169, G = 141, B = 99 }, "Brown")); // A98D63
          _compositePalette = BuildPalette(sample);
        }
        return _compositePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Concourse' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> ConcoursePalette
    {
      get
      {
        if (_concoursePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 222, G = 245, B = 250 }, "Light Turquoise")); // DEF5FA
          sample.Add(new NamedColor(new Color() { A = 255, R = 70, G = 70, B = 70 }, "Gray-80%")); // 464646
          sample.Add(new NamedColor(new Color() { A = 255, R = 45, G = 162, B = 191 }, "Turquoise")); // 2DA2BF
          sample.Add(new NamedColor(new Color() { A = 255, R = 218, G = 31, B = 40 }, "Red")); // DA1F28
          sample.Add(new NamedColor(new Color() { A = 255, R = 235, G = 100, B = 27 }, "Orange")); // EB641B
          sample.Add(new NamedColor(new Color() { A = 255, R = 57, G = 99, B = 157 }, "Blue")); // 39639D
          sample.Add(new NamedColor(new Color() { A = 255, R = 71, G = 75, B = 120 }, "Indigo")); // 474B78
          sample.Add(new NamedColor(new Color() { A = 255, R = 125, G = 60, B = 74 }, "Dark Red")); // 7D3C4A
          _concoursePalette = BuildPalette(sample);
        }
        return _concoursePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Couture' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> CouturePalette
    {
      get
      {
        if (_couturePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 208, G = 204, B = 185 }, "Tan")); // D0CCB9
          sample.Add(new NamedColor(new Color() { A = 255, R = 55, G = 48, B = 42 }, "Brown")); // 37302A
          sample.Add(new NamedColor(new Color() { A = 255, R = 158, G = 142, B = 92 }, "Brown")); // 9E8E5C
          sample.Add(new NamedColor(new Color() { A = 255, R = 160, G = 151, B = 129 }, "Brown")); // A09781
          sample.Add(new NamedColor(new Color() { A = 255, R = 133, G = 119, B = 109 }, "Brown")); // 85776D
          sample.Add(new NamedColor(new Color() { A = 255, R = 174, G = 175, B = 169 }, "Gray-25%")); // AEAFA9
          sample.Add(new NamedColor(new Color() { A = 255, R = 141, G = 135, B = 139 }, "Gray-50%")); // 8D878B
          sample.Add(new NamedColor(new Color() { A = 255, R = 107, G = 97, B = 73 }, "Brown")); // 6B6149
          _couturePalette = BuildPalette(sample);
        }
        return _couturePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Elemental' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> ElementalPalette
    {
      get
      {
        if (_elementalPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 172, G = 203, B = 249 }, "Light Blue")); // ACCBF9
          sample.Add(new NamedColor(new Color() { A = 255, R = 36, G = 40, B = 82 }, "Dark Purple")); // 242852
          sample.Add(new NamedColor(new Color() { A = 255, R = 98, G = 157, B = 209 }, "Blue")); // 629DD1
          sample.Add(new NamedColor(new Color() { A = 255, R = 41, G = 127, B = 213 }, "Blue")); // 297FD5
          sample.Add(new NamedColor(new Color() { A = 255, R = 127, G = 143, B = 169 }, "Blue-Gray")); // 7F8FA9
          sample.Add(new NamedColor(new Color() { A = 255, R = 74, G = 102, B = 172 }, "Blue-Gray")); // 4A66AC
          sample.Add(new NamedColor(new Color() { A = 255, R = 90, G = 162, B = 174 }, "Teal")); // 5AA2AE
          sample.Add(new NamedColor(new Color() { A = 255, R = 157, G = 144, B = 160 }, "Gray-50%")); // 9D90A0
          _elementalPalette = BuildPalette(sample);
        }
        return _elementalPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Equity' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> EquityPalette
    {
      get
      {
        if (_equityPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 233, G = 229, B = 220 }, "Tan")); // E9E5DC
          sample.Add(new NamedColor(new Color() { A = 255, R = 105, G = 100, B = 100 }, "Gray-50%")); // 696464
          sample.Add(new NamedColor(new Color() { A = 255, R = 211, G = 72, B = 23 }, "Orange")); // D34817
          sample.Add(new NamedColor(new Color() { A = 255, R = 155, G = 45, B = 31 }, "Dark Red")); // 9B2D1F
          sample.Add(new NamedColor(new Color() { A = 255, R = 162, G = 142, B = 106 }, "Brown")); // A28E6A
          sample.Add(new NamedColor(new Color() { A = 255, R = 149, G = 98, B = 81 }, "Brown")); // 956251
          sample.Add(new NamedColor(new Color() { A = 255, R = 145, G = 132, B = 133 }, "Gray-50%")); // 918485
          sample.Add(new NamedColor(new Color() { A = 255, R = 133, G = 93, B = 93 }, "Brown")); // 855D5D
          _equityPalette = BuildPalette(sample);
        }
        return _equityPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Essential' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> EssentialPalette
    {
      get
      {
        if (_essentialPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 0, G = 0, B = 0 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 200, G = 200, B = 177 }, "Tan")); // C8C8B1
          sample.Add(new NamedColor(new Color() { A = 255, R = 209, G = 040, B = 046 }, "Red")); // D1282E
          sample.Add(new NamedColor(new Color() { A = 255, R = 122, G = 122, B = 122 }, "Gray-50%")); // 7A7A7A
          sample.Add(new NamedColor(new Color() { A = 255, R = 245, G = 194, B = 001 }, "Gold")); // F5C201
          sample.Add(new NamedColor(new Color() { A = 255, R = 082, G = 109, B = 176 }, "Blue-Gray")); // 526DB0
          sample.Add(new NamedColor(new Color() { A = 255, R = 152, G = 154, B = 172 }, "Blue-Gray")); // 989AAC
          sample.Add(new NamedColor(new Color() { A = 255, R = 220, G = 089, B = 036 }, "Orange")); // DC5924
          sample.Add(new NamedColor(new Color() { A = 255, R = 180, G = 179, B = 146 }, "Tan")); // B4B392
          _essentialPalette = BuildPalette(sample);
        }
        return _essentialPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Executive' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> ExecutivePalette
    {
      get
      {
        if (_executivePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 228, G = 233, B = 239 }, "Ice Blue")); // E4E9EF
          sample.Add(new NamedColor(new Color() { A = 255, R = 47, G = 88, B = 151 }, "Dark Blue")); // 2F5897
          sample.Add(new NamedColor(new Color() { A = 255, R = 96, G = 118, B = 180 }, "Indigo")); // 6076B4
          sample.Add(new NamedColor(new Color() { A = 255, R = 156, G = 82, B = 82 }, "Red")); // 9C5252
          sample.Add(new NamedColor(new Color() { A = 255, R = 230, G = 132, B = 34 }, "Orange")); // E68422
          sample.Add(new NamedColor(new Color() { A = 255, R = 132, G = 102, B = 72 }, "Brown")); // 846648
          sample.Add(new NamedColor(new Color() { A = 255, R = 99, G = 137, B = 31 }, "Dark Green")); // 63891F
          sample.Add(new NamedColor(new Color() { A = 255, R = 117, G = 128, B = 133 }, "Gray-50%")); // 758085
          _executivePalette = BuildPalette(sample);
        }
        return _executivePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Flow' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> FlowPalette
    {
      get
      {
        if (_flowPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 219, G = 245, B = 249 }, "Light Turquoise")); // DBF5F9
          sample.Add(new NamedColor(new Color() { A = 255, R = 4, G = 97, B = 123 }, "Dark Teal")); // 04617B
          sample.Add(new NamedColor(new Color() { A = 255, R = 15, G = 111, B = 198 }, "Blue")); // 0F6FC6
          sample.Add(new NamedColor(new Color() { A = 255, R = 0, G = 157, B = 217 }, "Turquoise")); // 009DD9
          sample.Add(new NamedColor(new Color() { A = 255, R = 11, G = 208, B = 217 }, "Turquoise")); // 0BD0D9
          sample.Add(new NamedColor(new Color() { A = 255, R = 16, G = 207, B = 155 }, "Bright Green")); // 10CF9B
          sample.Add(new NamedColor(new Color() { A = 255, R = 124, G = 202, B = 98 }, "Green")); // 7CCA62
          sample.Add(new NamedColor(new Color() { A = 255, R = 165, G = 194, B = 73 }, "Lime")); // A5C249
          _flowPalette = BuildPalette(sample);
        }
        return _flowPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Foundry' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> FoundryPalette
    {
      get
      {
        if (_foundryPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 234, G = 235, B = 222 }, "Tan")); // EAEBDE
          sample.Add(new NamedColor(new Color() { A = 255, R = 103, G = 106, B = 85 }, "Olive Green")); // 676A55
          sample.Add(new NamedColor(new Color() { A = 255, R = 114, G = 163, B = 118 }, "Green")); // 72A376
          sample.Add(new NamedColor(new Color() { A = 255, R = 176, G = 204, B = 176 }, "Light Green")); // B0CCB0
          sample.Add(new NamedColor(new Color() { A = 255, R = 168, G = 205, B = 215 }, "Sky Blue")); // A8CDD7
          sample.Add(new NamedColor(new Color() { A = 255, R = 192, G = 190, B = 175 }, "Tan")); // C0BEAF
          sample.Add(new NamedColor(new Color() { A = 255, R = 206, G = 197, B = 151 }, "Tan")); // CEC597
          sample.Add(new NamedColor(new Color() { A = 255, R = 232, G = 183, B = 183 }, "Rose")); // E8B7B7
          _foundryPalette = BuildPalette(sample);
        }
        return _foundryPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Grid' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> GridPalette
    {
      get
      {
        if (_gridPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 204, G = 209, B = 185 }, "Tan")); // CCD1B9
          sample.Add(new NamedColor(new Color() { A = 255, R = 83, G = 73, B = 73 }, "Gray-80%")); // 534949
          sample.Add(new NamedColor(new Color() { A = 255, R = 198, G = 105, B = 81 }, "Tan")); // C66951
          sample.Add(new NamedColor(new Color() { A = 255, R = 191, G = 151, B = 77 }, "Tan")); // BF974D
          sample.Add(new NamedColor(new Color() { A = 255, R = 146, G = 139, B = 112 }, "Brown")); // 928B70
          sample.Add(new NamedColor(new Color() { A = 255, R = 135, G = 112, B = 107 }, "Brown")); // 87706B
          sample.Add(new NamedColor(new Color() { A = 255, R = 148, G = 115, B = 78 }, "Brown")); // 94734E
          sample.Add(new NamedColor(new Color() { A = 255, R = 111, G = 119, B = 125 }, "Gray-50%")); // 6F777D
          _gridPalette = BuildPalette(sample);
        }
        return _gridPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Hardcover' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> HardcoverPalette
    {
      get
      {
        if (_hardcoverPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 236, G = 233, B = 198 }, "Light Yellow")); // ECE9C6
          sample.Add(new NamedColor(new Color() { A = 255, R = 137, G = 93, B = 29 }, "Brown")); // 895D1D
          sample.Add(new NamedColor(new Color() { A = 255, R = 135, G = 54, B = 36 }, "Dark Red")); // 873624
          sample.Add(new NamedColor(new Color() { A = 255, R = 214, G = 134, B = 45 }, "Orange")); // D6862D
          sample.Add(new NamedColor(new Color() { A = 255, R = 208, G = 190, B = 64 }, "Gold")); // D0BE40
          sample.Add(new NamedColor(new Color() { A = 255, R = 135, G = 127, B = 108 }, "Brown")); // 877F6C
          sample.Add(new NamedColor(new Color() { A = 255, R = 151, G = 33, B = 9 }, "Dark Red")); // 972109
          sample.Add(new NamedColor(new Color() { A = 255, R = 174, G = 183, B = 149 }, "Olive Green")); // AEB795
          _hardcoverPalette = BuildPalette(sample);
        }
        return _hardcoverPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Horizon' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> HorizonPalette
    {
      get
      {
        if (_horizonPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 220, G = 158, B = 31 }, "Gold")); // DC9E1F
          sample.Add(new NamedColor(new Color() { A = 255, R = 31, G = 33, B = 35 }, "Gray-80%")); // 1F2123
          sample.Add(new NamedColor(new Color() { A = 255, R = 126, G = 151, B = 173 }, "Blue-Gray")); // 7E97AD
          sample.Add(new NamedColor(new Color() { A = 255, R = 204, G = 142, B = 96 }, "Tan")); // CC8E60
          sample.Add(new NamedColor(new Color() { A = 255, R = 122, G = 106, B = 96 }, "Brown")); // 7A6A60
          sample.Add(new NamedColor(new Color() { A = 255, R = 180, G = 147, B = 109 }, "Brown")); // B4936D
          sample.Add(new NamedColor(new Color() { A = 255, R = 103, G = 120, B = 123 }, "Teal")); // 67787B
          sample.Add(new NamedColor(new Color() { A = 255, R = 157, G = 147, B = 111 }, "Brown")); // 9D936F
          _horizonPalette = BuildPalette(sample);
        }
        return _horizonPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Median' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> MedianPalette
    {
      get
      {
        if (_medianPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 235, G = 221, B = 195 }, "Tan")); // EBDDC3
          sample.Add(new NamedColor(new Color() { A = 255, R = 119, G = 95, B = 85 }, "Brown")); // 775F55
          sample.Add(new NamedColor(new Color() { A = 255, R = 148, G = 182, B = 210 }, "Ice Blue")); // 94B6D2
          sample.Add(new NamedColor(new Color() { A = 255, R = 221, G = 128, B = 71 }, "Orange")); // DD8047
          sample.Add(new NamedColor(new Color() { A = 255, R = 165, G = 171, B = 129 }, "Olive Green")); // A5AB81
          sample.Add(new NamedColor(new Color() { A = 255, R = 216, G = 178, B = 92 }, "Gold")); // D8B25C
          sample.Add(new NamedColor(new Color() { A = 255, R = 123, G = 167, B = 157 }, "Green")); // 7BA79D
          sample.Add(new NamedColor(new Color() { A = 255, R = 150, G = 140, B = 140 }, "Gray-50%")); // 968C8C
          _medianPalette = BuildPalette(sample);
        }
        return _medianPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Metro' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> MetroPalette
    {
      get
      {
        if (_metroPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 214, G = 236, B = 255 }, "Light Blue")); // D6ECFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 78, G = 91, B = 111 }, "Blue-Gray")); // 4E5B6F
          sample.Add(new NamedColor(new Color() { A = 255, R = 127, G = 209, B = 59 }, "Green")); // 7FD13B
          sample.Add(new NamedColor(new Color() { A = 255, R = 234, G = 21, B = 122 }, "Pink")); // EA157A
          sample.Add(new NamedColor(new Color() { A = 255, R = 254, G = 184, B = 10 }, "Gold")); // FEB80A
          sample.Add(new NamedColor(new Color() { A = 255, R = 0, G = 173, B = 220 }, "Turquoise")); // 00ADDC
          sample.Add(new NamedColor(new Color() { A = 255, R = 115, G = 138, B = 200 }, "Periwinkle")); // 738AC8
          sample.Add(new NamedColor(new Color() { A = 255, R = 26, G = 179, B = 159 }, "Teal")); // 1AB39F
          _metroPalette = BuildPalette(sample);
        }
        return _metroPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Module' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> ModulePalette
    {
      get
      {
        if (_modulePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 212, G = 212, B = 214 }, "Gray-25%")); // D4D4D6
          sample.Add(new NamedColor(new Color() { A = 255, R = 90, G = 99, B = 120 }, "Blue-Gray")); // 5A6378
          sample.Add(new NamedColor(new Color() { A = 255, R = 240, G = 173, B = 0 }, "Gold")); // F0AD00
          sample.Add(new NamedColor(new Color() { A = 255, R = 96, G = 181, B = 204 }, "Aqua")); // 60B5CC
          sample.Add(new NamedColor(new Color() { A = 255, R = 230, G = 108, B = 125 }, "Rose")); // E66C7D
          sample.Add(new NamedColor(new Color() { A = 255, R = 107, G = 183, B = 109 }, "Green")); // 6BB76D
          sample.Add(new NamedColor(new Color() { A = 255, R = 232, G = 134, B = 81 }, "Orange")); // E88651
          sample.Add(new NamedColor(new Color() { A = 255, R = 198, G = 72, B = 71 }, "Red")); // C64847
          _modulePalette = BuildPalette(sample);
        }
        return _modulePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Newsprint' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> NewsprintPalette
    {
      get
      {
        if (_newsprintPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 222, G = 222, B = 224 }, "Gray-25%")); // DEDEE0
          sample.Add(new NamedColor(new Color() { A = 255, R = 48, G = 48, B = 48 }, "Gray-80%")); // 303030
          sample.Add(new NamedColor(new Color() { A = 255, R = 173, G = 1, B = 1 }, "Dark Red")); // AD0101
          sample.Add(new NamedColor(new Color() { A = 255, R = 114, G = 96, B = 86 }, "Brown")); // 726056
          sample.Add(new NamedColor(new Color() { A = 255, R = 172, G = 149, B = 110 }, "Brown")); // AC956E
          sample.Add(new NamedColor(new Color() { A = 255, R = 128, G = 141, B = 169 }, "Blue-Gray")); // 808DA9
          sample.Add(new NamedColor(new Color() { A = 255, R = 66, G = 78, B = 91 }, "Blue-Gray")); // 424E5B
          sample.Add(new NamedColor(new Color() { A = 255, R = 115, G = 14, B = 0 }, "Dark Red")); // 730E00
          _newsprintPalette = BuildPalette(sample);
        }
        return _newsprintPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Opulent' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> OpulentPalette
    {
      get
      {
        if (_opulentPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 244, G = 231, B = 237 }, "Lavender")); // F4E7ED
          sample.Add(new NamedColor(new Color() { A = 255, R = 177, G = 63, B = 154 }, "Pink")); // B13F9A
          sample.Add(new NamedColor(new Color() { A = 255, R = 184, G = 61, B = 104 }, "Pink")); // B83D68
          sample.Add(new NamedColor(new Color() { A = 255, R = 172, G = 102, B = 187 }, "Purple")); // AC66BB
          sample.Add(new NamedColor(new Color() { A = 255, R = 222, G = 108, B = 54 }, "Orange")); // DE6C36
          sample.Add(new NamedColor(new Color() { A = 255, R = 249, G = 182, B = 57 }, "Gold")); // F9B639
          sample.Add(new NamedColor(new Color() { A = 255, R = 207, G = 109, B = 164 }, "Pink")); // CF6DA4
          sample.Add(new NamedColor(new Color() { A = 255, R = 250, G = 141, B = 61 }, "Orange")); // FA8D3D
          _opulentPalette = BuildPalette(sample);
        }
        return _opulentPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Oriel' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> OrielPalette
    {
      get
      {
        if (_orielPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 243, B = 157 }, "Light Yellow")); // FFF39D
          sample.Add(new NamedColor(new Color() { A = 255, R = 87, G = 95, B = 109 }, "Blue-Gray")); // 575F6D
          sample.Add(new NamedColor(new Color() { A = 255, R = 254, G = 134, B = 55 }, "Orange")); // FE8637
          sample.Add(new NamedColor(new Color() { A = 255, R = 117, G = 152, B = 217 }, "Blue")); // 7598D9
          sample.Add(new NamedColor(new Color() { A = 255, R = 179, G = 44, B = 22 }, "Red")); // B32C16
          sample.Add(new NamedColor(new Color() { A = 255, R = 245, G = 205, B = 45 }, "Gold")); // F5CD2D
          sample.Add(new NamedColor(new Color() { A = 255, R = 174, G = 186, B = 213 }, "Ice Blue")); // AEBAD5
          sample.Add(new NamedColor(new Color() { A = 255, R = 119, G = 124, B = 132 }, "Gray-50%")); // 777C84
          _orielPalette = BuildPalette(sample);
        }
        return _orielPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Origin' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> OriginPalette
    {
      get
      {
        if (_originPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 221, G = 233, B = 236 }, "Ice Blue")); // DDE9EC
          sample.Add(new NamedColor(new Color() { A = 255, R = 70, G = 70, B = 83 }, "Indigo")); // 464653
          sample.Add(new NamedColor(new Color() { A = 255, R = 114, G = 124, B = 163 }, "Blue-Gray")); // 727CA3
          sample.Add(new NamedColor(new Color() { A = 255, R = 159, G = 184, B = 205 }, "Ice Blue")); // 9FB8CD
          sample.Add(new NamedColor(new Color() { A = 255, R = 210, G = 218, B = 122 }, "Lime")); // D2DA7A
          sample.Add(new NamedColor(new Color() { A = 255, R = 250, G = 218, B = 122 }, "Light Yellow")); // FADA7A
          sample.Add(new NamedColor(new Color() { A = 255, R = 184, G = 132, B = 114 }, "Brown")); // B88472
          sample.Add(new NamedColor(new Color() { A = 255, R = 142, G = 115, B = 106 }, "Brown")); // 8E736A
          _originPalette = BuildPalette(sample);
        }
        return _originPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Paper' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> PaperPalette
    {
      get
      {
        if (_paperPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 254, G = 250, B = 201 }, "Light Yellow")); // FEFAC9
          sample.Add(new NamedColor(new Color() { A = 255, R = 68, G = 77, B = 38 }, "Dark Green")); // 444D26
          sample.Add(new NamedColor(new Color() { A = 255, R = 165, G = 181, B = 146 }, "Olive Green")); // A5B592
          sample.Add(new NamedColor(new Color() { A = 255, R = 243, G = 164, B = 71 }, "Orange")); // F3A447
          sample.Add(new NamedColor(new Color() { A = 255, R = 231, G = 188, B = 41 }, "Gold")); // E7BC29
          sample.Add(new NamedColor(new Color() { A = 255, R = 208, G = 146, B = 167 }, "Lavender")); // D092A7
          sample.Add(new NamedColor(new Color() { A = 255, R = 156, G = 133, B = 192 }, "Lavender")); // 9C85C0
          sample.Add(new NamedColor(new Color() { A = 255, R = 128, G = 158, B = 194 }, "Blue-Gray")); // 809EC2
          _paperPalette = BuildPalette(sample);
        }
        return _paperPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Perspective' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> PerspectivePalette
    {
      get
      {
        if (_perspectivePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 134, B = 0 }, "Orange")); // FF8600
          sample.Add(new NamedColor(new Color() { A = 255, R = 40, G = 49, B = 56 }, "Blue-Gray")); // 283138
          sample.Add(new NamedColor(new Color() { A = 255, R = 131, G = 141, B = 155 }, "Blue-Gray")); // 838D9B
          sample.Add(new NamedColor(new Color() { A = 255, R = 210, G = 97, B = 12 }, "Brown")); // D2610C
          sample.Add(new NamedColor(new Color() { A = 255, R = 128, G = 113, B = 106 }, "Brown")); // 80716A
          sample.Add(new NamedColor(new Color() { A = 255, R = 148, G = 20, B = 124 }, "Dark Purple")); // 94147C
          sample.Add(new NamedColor(new Color() { A = 255, R = 93, G = 90, B = 210 }, "Indigo")); // 5D5AD2
          sample.Add(new NamedColor(new Color() { A = 255, R = 111, G = 108, B = 125 }, "Gray-50%")); // 6F6C7D
          _perspectivePalette = BuildPalette(sample);
        }
        return _perspectivePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Pushpin' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> PushpinPalette
    {
      get
      {
        if (_pushpinPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 204, G = 221, B = 234 }, "Ice Blue")); // CCDDEA
          sample.Add(new NamedColor(new Color() { A = 255, R = 70, G = 94, B = 156 }, "Indigo")); // 465E9C
          sample.Add(new NamedColor(new Color() { A = 255, R = 253, G = 160, B = 35 }, "Orange")); // FDA023
          sample.Add(new NamedColor(new Color() { A = 255, R = 170, G = 43, B = 30 }, "Red")); // AA2B1E
          sample.Add(new NamedColor(new Color() { A = 255, R = 113, G = 104, B = 92 }, "Brown")); // 71685C
          sample.Add(new NamedColor(new Color() { A = 255, R = 100, G = 167, B = 59 }, "Green")); // 64A73B
          sample.Add(new NamedColor(new Color() { A = 255, R = 235, G = 86, B = 5 }, "Orange")); // EB5605
          sample.Add(new NamedColor(new Color() { A = 255, R = 185, G = 202, B = 26 }, "Green")); // B9CA1A
          _pushpinPalette = BuildPalette(sample);
        }
        return _pushpinPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Slipstream' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> SlipstreamPalette
    {
      get
      {
        if (_slipstreamPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 180, G = 220, B = 250 }, "Light Blue")); // B4DCFA
          sample.Add(new NamedColor(new Color() { A = 255, R = 33, G = 39, B = 69 }, "Indigo")); // 212745
          sample.Add(new NamedColor(new Color() { A = 255, R = 78, G = 103, B = 200 }, "Blue")); // 4E67C8
          sample.Add(new NamedColor(new Color() { A = 255, R = 94, G = 204, B = 243 }, "Turquoise")); // 5ECCF3
          sample.Add(new NamedColor(new Color() { A = 255, R = 167, G = 234, B = 82 }, "Green")); // A7EA52
          sample.Add(new NamedColor(new Color() { A = 255, R = 93, G = 206, B = 175 }, "Green")); // 5DCEAF
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 128, B = 33 }, "Orange")); // FF8021
          sample.Add(new NamedColor(new Color() { A = 255, R = 241, G = 65, B = 36 }, "Red")); // F14124
          _slipstreamPalette = BuildPalette(sample);
        }
        return _slipstreamPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Solstice' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> SolsticePalette
    {
      get
      {
        if (_solsticePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 231, G = 222, B = 201 }, "Tan")); // E7DEC9
          sample.Add(new NamedColor(new Color() { A = 255, R = 79, G = 39, B = 28 }, "Brown")); // 4F271C
          sample.Add(new NamedColor(new Color() { A = 255, R = 56, G = 145, B = 167 }, "Aqua")); // 3891A7
          sample.Add(new NamedColor(new Color() { A = 255, R = 254, G = 184, B = 10 }, "Gold")); // FEB80A
          sample.Add(new NamedColor(new Color() { A = 255, R = 195, G = 45, B = 46 }, "Red")); // C32D2E
          sample.Add(new NamedColor(new Color() { A = 255, R = 132, G = 170, B = 51 }, "Green")); // 84AA33
          sample.Add(new NamedColor(new Color() { A = 255, R = 150, G = 67, B = 5 }, "Brown")); // 964305
          sample.Add(new NamedColor(new Color() { A = 255, R = 71, G = 90, B = 141 }, "Indigo")); // 475A8D
          _solsticePalette = BuildPalette(sample);
        }
        return _solsticePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Technic' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> TechnicPalette
    {
      get
      {
        if (_technicPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 212, G = 210, B = 208 }, "Gray-25%")); // D4D2D0
          sample.Add(new NamedColor(new Color() { A = 255, R = 59, G = 59, B = 59 }, "Gray-80%")); // 3B3B3B
          sample.Add(new NamedColor(new Color() { A = 255, R = 110, G = 160, B = 176 }, "Aqua")); // 6EA0B0
          sample.Add(new NamedColor(new Color() { A = 255, R = 204, G = 175, B = 10 }, "Gold")); // CCAF0A
          sample.Add(new NamedColor(new Color() { A = 255, R = 141, G = 137, B = 164 }, "Lavender")); // 8D89A4
          sample.Add(new NamedColor(new Color() { A = 255, R = 116, G = 133, B = 96 }, "Olive Green")); // 748560
          sample.Add(new NamedColor(new Color() { A = 255, R = 158, G = 146, B = 115 }, "Brown")); // 9E9273
          sample.Add(new NamedColor(new Color() { A = 255, R = 126, G = 132, B = 141 }, "Gray-50%")); // 7E848D
          _technicPalette = BuildPalette(sample);
        }
        return _technicPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Thatch' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> ThatchPalette
    {
      get
      {
        if (_thatchPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 223, G = 230, B = 208 }, "Light Green")); // DFE6D0
          sample.Add(new NamedColor(new Color() { A = 255, R = 29, G = 54, B = 65 }, "Dark Teal")); // 1D3641
          sample.Add(new NamedColor(new Color() { A = 255, R = 117, G = 154, B = 165 }, "Blue-Gray")); // 759AA5
          sample.Add(new NamedColor(new Color() { A = 255, R = 207, G = 198, B = 13 }, "Yellow")); // CFC60D
          sample.Add(new NamedColor(new Color() { A = 255, R = 153, G = 152, B = 127 }, "Brown")); // 99987F
          sample.Add(new NamedColor(new Color() { A = 255, R = 144, G = 172, B = 151 }, "Green")); // 90AC97
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 173, B = 28 }, "Gold")); // FFAD1C
          sample.Add(new NamedColor(new Color() { A = 255, R = 185, G = 171, B = 111 }, "Tan")); // B9AB6F
          _thatchPalette = BuildPalette(sample);
        }
        return _thatchPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Trek' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> TrekPalette
    {
      get
      {
        if (_trekPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 251, G = 238, B = 201 }, "Light Yellow")); // FBEEC9
          sample.Add(new NamedColor(new Color() { A = 255, R = 78, G = 59, B = 48 }, "Brown")); // 4E3B30
          sample.Add(new NamedColor(new Color() { A = 255, R = 240, G = 162, B = 46 }, "Orange")); // F0A22E
          sample.Add(new NamedColor(new Color() { A = 255, R = 165, G = 100, B = 78 }, "Brown")); // A5644E
          sample.Add(new NamedColor(new Color() { A = 255, R = 181, G = 139, B = 128 }, "Brown")); // B58B80
          sample.Add(new NamedColor(new Color() { A = 255, R = 195, G = 152, B = 109 }, "Brown")); // C3986D
          sample.Add(new NamedColor(new Color() { A = 255, R = 161, G = 149, B = 116 }, "Brown")); // A19574
          sample.Add(new NamedColor(new Color() { A = 255, R = 193, G = 117, B = 41 }, "Orange")); // C17529
          _trekPalette = BuildPalette(sample);
        }
        return _trekPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Urban' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> UrbanPalette
    {
      get
      {
        if (_urbanPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 222, G = 222, B = 222 }, "Gray-25%")); // DEDEDE
          sample.Add(new NamedColor(new Color() { A = 255, R = 66, G = 68, B = 86 }, "Blue-Gray")); // 424456
          sample.Add(new NamedColor(new Color() { A = 255, R = 83, G = 84, B = 138 }, "Indigo")); // 53548A
          sample.Add(new NamedColor(new Color() { A = 255, R = 67, G = 128, B = 134 }, "Teal")); // 438086
          sample.Add(new NamedColor(new Color() { A = 255, R = 160, G = 77, B = 163 }, "Purple")); // A04DA3
          sample.Add(new NamedColor(new Color() { A = 255, R = 196, G = 101, B = 45 }, "Orange")); // C4652D
          sample.Add(new NamedColor(new Color() { A = 255, R = 139, G = 93, B = 61 }, "Brown")); // 8B5D3D
          sample.Add(new NamedColor(new Color() { A = 255, R = 92, G = 146, B = 181 }, "Blue-Gray")); // 5C92B5
          _urbanPalette = BuildPalette(sample);
        }
        return _urbanPalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Verve' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> VervePalette
    {
      get
      {
        if (_vervePalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 210, G = 210, B = 210 }, "Gray-25%")); // D2D2D2
          sample.Add(new NamedColor(new Color() { A = 255, R = 102, G = 102, B = 102 }, "Gray-50%")); // 666666
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 56, B = 140 }, "Pink")); // FF388C
          sample.Add(new NamedColor(new Color() { A = 255, R = 228, G = 0, B = 89 }, "Pink")); // E40059
          sample.Add(new NamedColor(new Color() { A = 255, R = 156, G = 0, B = 127 }, "Plum")); // 9C007F
          sample.Add(new NamedColor(new Color() { A = 255, R = 104, G = 0, B = 127 }, "Dark Purple")); // 68007F
          sample.Add(new NamedColor(new Color() { A = 255, R = 0, G = 91, B = 211 }, "Blue")); // 005BD3
          sample.Add(new NamedColor(new Color() { A = 255, R = 0, G = 52, B = 158 }, "Dark Blue")); // 00349E
          _vervePalette = BuildPalette(sample);
        }
        return _vervePalette;
      }
    }

    /// <summary>
    /// Gets a palette containing the colors seen in the Office 'Waveform' style.
    /// </summary>
    public static ReadOnlyCollection<NamedColor> WaveformPalette
    {
      get
      {
        if (_waveformPalette == null)
        {
          IList<NamedColor> sample = new List<NamedColor>();
          sample.Add(new NamedColor(new Color() { A = 255, R = 255, G = 255, B = 255 }, "White")); // FFFFFF
          sample.Add(new NamedColor(new Color() { A = 255, R = 000, G = 000, B = 000 }, "Black")); // 000000
          sample.Add(new NamedColor(new Color() { A = 255, R = 198, G = 231, B = 252 }, "Light Blue")); // C6E7FC
          sample.Add(new NamedColor(new Color() { A = 255, R = 7, G = 62, B = 135 }, "Dark Blue")); // 073E87
          sample.Add(new NamedColor(new Color() { A = 255, R = 49, G = 182, B = 253 }, "Blue")); // 31B6FD
          sample.Add(new NamedColor(new Color() { A = 255, R = 69, G = 132, B = 211 }, "Blue")); // 4584D3
          sample.Add(new NamedColor(new Color() { A = 255, R = 91, G = 208, B = 120 }, "Green")); // 5BD078
          sample.Add(new NamedColor(new Color() { A = 255, R = 165, G = 208, B = 40 }, "Lime")); // A5D028
          sample.Add(new NamedColor(new Color() { A = 255, R = 245, G = 192, B = 64 }, "Gold")); // F5C040
          sample.Add(new NamedColor(new Color() { A = 255, R = 5, G = 224, B = 219 }, "Teal")); // 05E0DB
          _waveformPalette = BuildPalette(sample);
        }
        return _waveformPalette;
      }
    }

    private static Color GetShade(Color color, bool lighter, int percentage)
    {
      byte r = color.R;
      byte g = color.G;
      byte b = color.B;

      if (lighter)
      {
        r = (byte)((255 - r) * (percentage / 100.0) + r);
        g = (byte)((255 - g) * (percentage / 100.0) + g);
        b = (byte)((255 - b) * (percentage / 100.0) + b);
      }
      else
      {
        r = (byte)(r - r * (percentage / 100.0));
        g = (byte)(g - g * (percentage / 100.0));
        b = (byte)(b - b * (percentage / 100.0));
      }

      return new Color() { A = 255, R = r, G = g, B = b };
    }

    private static string[] _categories = new string[] { "Background 1", "Text 1", "Background 2", "Text 2", "Accent 1", "Accent 2", "Accent 3", "Accent 4", "Accent 5", "Accent 6" };

    private static ReadOnlyCollection<NamedColor> BuildPalette(IList<NamedColor> sample)
    {
      bool includeCategoryName = false;
      List<NamedColor> palette = new List<NamedColor>();
      int categoryIndex = 0;
      foreach (NamedColor color in sample)
      {
        palette.Add(new NamedColor(color.Name + (includeCategoryName ? ", " + _categories[categoryIndex] : ""), color.Color));
        categoryIndex++;
      }
      categoryIndex = 0;
      foreach (NamedColor color in sample)
      {
        foreach (ShadeInfo percentage in GetPercentages(color.Color))
        {
          if (includeCategoryName)
          {
            palette.Add(new NamedColor(GetNamedColorName(color, _categories[categoryIndex], percentage), GetShade(color.Color, percentage.IsLighter, percentage.Percentage)));
          }
          else
          {
            palette.Add(new NamedColor(GetNamedColorName(color, percentage), GetShade(color.Color, percentage.IsLighter, percentage.Percentage)));
          }
        }
        categoryIndex++;
      }

      palette.Add(new NamedColor("Dark Red", new Color() { A = 255, R = 192, G = 0, B = 0 })); // C00000
      palette.Add(new NamedColor("Red", new Color() { A = 255, R = 255, G = 0, B = 0 })); // FF0000
      palette.Add(new NamedColor("Orange", new Color() { A = 255, R = 255, G = 192, B = 0 })); // FFC000
      palette.Add(new NamedColor("Yellow", new Color() { A = 255, R = 255, G = 255, B = 0 })); // F5F5F5
      palette.Add(new NamedColor("Light Green", new Color() { A = 255, R = 146, G = 208, B = 80 })); // 92D050
      palette.Add(new NamedColor("Green", new Color() { A = 255, R = 0, G = 176, B = 80 })); // 00B050
      palette.Add(new NamedColor("Light Blue", new Color() { A = 255, R = 0, G = 176, B = 240 })); // 00B0F0
      palette.Add(new NamedColor("Blue", new Color() { A = 255, R = 0, G = 112, B = 192 })); // 0070C0
      palette.Add(new NamedColor("Dark Blue", new Color() { A = 255, R = 0, G = 32, B = 96 })); // 002060
      palette.Add(new NamedColor("Purple", new Color() { A = 255, R = 112, G = 48, B = 160 })); // 7030A0

      return palette.AsReadOnly();
    }

    private static string GetNamedColorName(NamedColor color, ShadeInfo percentage)
    {
      return color.Name + ", " + (percentage.IsLighter ? "Lighter " : "Darker ") + percentage.Percentage + "%";
    }

    private static string GetNamedColorName(NamedColor color, string category, ShadeInfo percentage)
    {
      return color.Name + ", " + category + ", " + (percentage.IsLighter ? "Lighter " : "Darker ") + percentage.Percentage + "%";
    }

    private static ShadeInfo[] _blackPercentages = new ShadeInfo[] { new ShadeInfo(50, true), new ShadeInfo(35, true), new ShadeInfo(25, true), new ShadeInfo(15, true), new ShadeInfo(5, true) };
    private static ShadeInfo[] _darkPercentages = new ShadeInfo[] { new ShadeInfo(90, true), new ShadeInfo(75, true), new ShadeInfo(50, true), new ShadeInfo(25, true), new ShadeInfo(10, true) };
    private static ShadeInfo[] _neutralPercentages = new ShadeInfo[] { new ShadeInfo(80, true), new ShadeInfo(60, true), new ShadeInfo(40, true), new ShadeInfo(25, false), new ShadeInfo(50, false) };
    private static ShadeInfo[] _lightPercentages = new ShadeInfo[] { new ShadeInfo(10, false), new ShadeInfo(25, false), new ShadeInfo(50, false), new ShadeInfo(75, false), new ShadeInfo(90, false) };
    private static ShadeInfo[] _whitePercentages = new ShadeInfo[] { new ShadeInfo(5, false), new ShadeInfo(15, false), new ShadeInfo(25, false), new ShadeInfo(35, false), new ShadeInfo(50, false) };

    private static ShadeInfo[] GetPercentages(Color color)
    {
      int avg = (color.R + color.G + color.B) / 3;
      ShadeInfo[] result = _neutralPercentages;
      if (avg == 0)
      {
        result = _blackPercentages;
      }
      else if (avg < 51)
      {
        result = _darkPercentages;
      }
      else if (avg == 255)
      {
        result = _whitePercentages;
      }
      else if (avg > 203)
      {
        result = _lightPercentages;
      }
      return result;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals")]
    private static void BuildOfficePalette()
    {
      IList<NamedColor> palette = new List<NamedColor>();
      palette.Add(new NamedColor("White", new Color() { A = 255, R = 255, G = 255, B = 255 })); // FFFFFF
      palette.Add(new NamedColor("Black", new Color() { A = 255, R = 0, G = 0, B = 0 })); // 000000
      palette.Add(new NamedColor("Tan", new Color() { A = 255, R = 238, G = 236, B = 225 })); // EEECE1
      palette.Add(new NamedColor("Dark Blue", new Color() { A = 255, R = 31, G = 73, B = 125 })); // 1F497D
      palette.Add(new NamedColor("Blue", new Color() { A = 255, R = 79, G = 129, B = 189 })); // 4F81BD
      palette.Add(new NamedColor("Red", new Color() { A = 255, R = 192, G = 80, B = 77 })); // C0504D
      palette.Add(new NamedColor("Olive Green", new Color() { A = 255, R = 155, G = 187, B = 89 })); // 9BBB59
      palette.Add(new NamedColor("Purple", new Color() { A = 255, R = 128, G = 100, B = 162 })); // 8064A2
      palette.Add(new NamedColor("Aqua", new Color() { A = 255, R = 75, G = 172, B = 198 })); // 4BACC6
      palette.Add(new NamedColor("Orange", new Color() { A = 255, R = 247, G = 150, B = 70 })); // F79646

      palette.Add(new NamedColor("White, Darker 5%", new Color() { A = 255, R = 242, G = 242, B = 242 })); // F2F2F2
      palette.Add(new NamedColor("White, Darker 15%", new Color() { A = 255, R = 216, G = 216, B = 216 })); // D8D8D8
      palette.Add(new NamedColor("White, Darker 25%", new Color() { A = 255, R = 191, G = 191, B = 191 })); // BFBFBF
      palette.Add(new NamedColor("White, Darker 35%", new Color() { A = 255, R = 165, G = 165, B = 165 })); // A5A5A5
      palette.Add(new NamedColor("White, Darker 50%", new Color() { A = 255, R = 127, G = 127, B = 127 })); // 7F7F7F

      palette.Add(new NamedColor("Black, Lighter 50%", new Color() { A = 255, R = 126, G = 126, B = 126 })); // 7E7E7E
      palette.Add(new NamedColor("Black, Lighter 35%", new Color() { A = 255, R = 89, G = 89, B = 89 })); // 595959
      palette.Add(new NamedColor("Black, Lighter 25%", new Color() { A = 255, R = 63, G = 63, B = 63 })); // 3F3F3F
      palette.Add(new NamedColor("Black, Lighter 15%", new Color() { A = 255, R = 38, G = 38, B = 38 })); // 262626
      palette.Add(new NamedColor("Black, Lighter 5%", new Color() { A = 255, R = 12, G = 12, B = 12 })); // 0C0C0C

      palette.Add(new NamedColor("Tan, Darker 10%", new Color() { A = 255, R = 221, G = 217, B = 195 })); // DDD9C3
      palette.Add(new NamedColor("Tan, Darker 25%", new Color() { A = 255, R = 196, G = 189, B = 151 })); // C4BD97
      palette.Add(new NamedColor("Tan, Darker 50%", new Color() { A = 255, R = 147, G = 137, B = 83 })); // 938953
      palette.Add(new NamedColor("Tan, Darker 75%", new Color() { A = 255, R = 73, G = 68, B = 41 })); // 494429
      palette.Add(new NamedColor("Tan, Darker 90%", new Color() { A = 255, R = 29, G = 27, B = 16 })); // 1D1B10

      palette.Add(new NamedColor("Dark Blue, Lighter 80%", new Color() { A = 255, R = 198, G = 217, B = 240 })); // C6D9F0
      palette.Add(new NamedColor("Dark Blue, Lighter 60%", new Color() { A = 255, R = 141, G = 179, B = 226 })); // 8DB3E2
      palette.Add(new NamedColor("Dark Blue, Lighter 40%", new Color() { A = 255, R = 84, G = 141, B = 212 })); // 548DD4
      palette.Add(new NamedColor("Dark Blue, Darker 25%", new Color() { A = 255, R = 23, G = 54, B = 93 })); // 17365D
      palette.Add(new NamedColor("Dark Blue, Darker 50%", new Color() { A = 255, R = 15, G = 36, B = 62 })); // 0F243E

      palette.Add(new NamedColor("Blue, Lighter 80%", new Color() { A = 255, R = 219, G = 229, B = 241 })); // DBE5F1
      palette.Add(new NamedColor("Blue, Lighter 60%", new Color() { A = 255, R = 184, G = 204, B = 228 })); // B8CCE4
      palette.Add(new NamedColor("Blue, Lighter 40%", new Color() { A = 255, R = 149, G = 179, B = 215 })); // 95B3D7
      palette.Add(new NamedColor("Blue, Darker 25%", new Color() { A = 255, R = 54, G = 96, B = 146 })); // 366092
      palette.Add(new NamedColor("Blue, Darker 50%", new Color() { A = 255, R = 36, G = 64, B = 97 })); // 244061

      palette.Add(new NamedColor("Red, Lighter 80%", new Color() { A = 255, R = 242, G = 220, B = 219 })); // F2DCDB
      palette.Add(new NamedColor("Red, Lighter 60%", new Color() { A = 255, R = 229, G = 185, B = 183 })); // E5B9B7
      palette.Add(new NamedColor("Red, Lighter 40%", new Color() { A = 255, R = 217, G = 150, B = 148 })); // D99694
      palette.Add(new NamedColor("Red, Darker 25%", new Color() { A = 255, R = 149, G = 55, B = 52 })); // 953734
      palette.Add(new NamedColor("Red, Darker 50%", new Color() { A = 255, R = 99, G = 36, B = 35 })); // 632423

      palette.Add(new NamedColor("Olive Green, Lighter 80%", new Color() { A = 255, R = 235, G = 241, B = 221 })); // EBF1DD
      palette.Add(new NamedColor("Olive Green, Lighter 60%", new Color() { A = 255, R = 215, G = 227, B = 188 })); // D7E3BC
      palette.Add(new NamedColor("Olive Green, Lighter 40%", new Color() { A = 255, R = 195, G = 214, B = 155 })); // C3D69B
      palette.Add(new NamedColor("Olive Green, Darker 25%", new Color() { A = 255, R = 118, G = 146, B = 60 })); // 76923C
      palette.Add(new NamedColor("Olive Green, Darker 50%", new Color() { A = 255, R = 79, G = 97, B = 40 })); // 4F6128

      palette.Add(new NamedColor("Purple, Lighter 80%", new Color() { A = 255, R = 229, G = 224, B = 236 })); // E5E0EC
      palette.Add(new NamedColor("Purple, Lighter 60%", new Color() { A = 255, R = 204, G = 193, B = 217 })); // CCC1D9
      palette.Add(new NamedColor("Purple, Lighter 40%", new Color() { A = 255, R = 178, G = 162, B = 199 })); // B2A2C7
      palette.Add(new NamedColor("Purple, Darker 25%", new Color() { A = 255, R = 95, G = 73, B = 122 })); // 5F497A
      palette.Add(new NamedColor("Purple, Darker 50%", new Color() { A = 255, R = 63, G = 49, B = 81 })); // 3F3151

      palette.Add(new NamedColor("Aqua, Lighter 80%", new Color() { A = 255, R = 219, G = 238, B = 243 })); // DBEEF3
      palette.Add(new NamedColor("Aqua, Lighter 60%", new Color() { A = 255, R = 183, G = 221, B = 232 })); // B7DDE8
      palette.Add(new NamedColor("Aqua, Lighter 40%", new Color() { A = 255, R = 146, G = 205, B = 220 })); // 92CDDC
      palette.Add(new NamedColor("Aqua, Darker 25%", new Color() { A = 255, R = 49, G = 133, B = 155 })); // 31859B
      palette.Add(new NamedColor("Aqua, Darker 50%", new Color() { A = 255, R = 32, G = 88, B = 103 })); // 205867

      palette.Add(new NamedColor("Orange, Lighter 80%", new Color() { A = 255, R = 253, G = 234, B = 218 })); // FDEADA
      palette.Add(new NamedColor("Orange, Lighter 60%", new Color() { A = 255, R = 251, G = 213, B = 181 })); // FBD5B5
      palette.Add(new NamedColor("Orange, Lighter 40%", new Color() { A = 255, R = 250, G = 192, B = 143 })); // FAC08F
      palette.Add(new NamedColor("Orange, Darker 25%", new Color() { A = 255, R = 227, G = 108, B = 9 })); // E36C09
      palette.Add(new NamedColor("Orange, Darker 50%", new Color() { A = 255, R = 151, G = 72, B = 6 })); // 974806

      palette.Add(new NamedColor("Dark Red", new Color() { A = 255, R = 192, G = 0, B = 0 }));
      palette.Add(new NamedColor("Red", new Color() { A = 255, R = 255, G = 0, B = 0 }));
      palette.Add(new NamedColor("Orange", new Color() { A = 255, R = 255, G = 192, B = 0 }));
      palette.Add(new NamedColor("Yellow", new Color() { A = 255, R = 255, G = 255, B = 0 })); // F5F5F5
      palette.Add(new NamedColor("Light Green", new Color() { A = 255, R = 146, G = 208, B = 80 })); // 92D050
      palette.Add(new NamedColor("Green", new Color() { A = 255, R = 0, G = 176, B = 80 })); // 00B050
      palette.Add(new NamedColor("Light Blue", new Color() { A = 255, R = 0, G = 176, B = 240 })); // 00B0F0
      palette.Add(new NamedColor("Blue", new Color() { A = 255, R = 0, G = 112, B = 192 })); // 0070C0
      palette.Add(new NamedColor("Dark Blue", new Color() { A = 255, R = 0, G = 32, B = 96 })); // 002060
      palette.Add(new NamedColor("Purple", new Color() { A = 255, R = 112, G = 48, B = 160 })); // 7030A0

      _officePalette = new ReadOnlyCollection<NamedColor>(palette);
    }

    private class ShadeInfo
    {
      public ShadeInfo(int i, bool b)
      {
        Percentage = i;
        IsLighter = b;
      }
      public int Percentage { get; private set; }
      public bool IsLighter { get; private set; }
    }
  }
}
