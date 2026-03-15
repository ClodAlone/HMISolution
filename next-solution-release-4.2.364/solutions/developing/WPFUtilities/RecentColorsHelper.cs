using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Xml.Linq;
using System.Linq;
#if !WINDOWS_UWP
using DevExpress.Xpf.Editors;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Settings;
#endif

namespace Utilities
{
    /// <remark>
    /// This helper is intended to provide the base for a helper,
    /// which simplifies use of the Application.Properties property.
    /// The obvious next step in extending the class is to add
    /// argument validation.
    /// </remark>
    public static class RecentColorsHelper
    {
        public static event EventHandler RecentColorsChanged;

        static CircularList<Color> RecentColors { get; } = new CircularList<Color>(10);
        static bool bSettingColors = false;
        static bool bSynchronizingCollections = false;
        static Dictionary<Object, Object> mapResourceBrushes;
        static List<ColorPalette> customPalettes = null;

        const string palettesFileName = "ColorPalettes";
        const string paletteCollectionName = "paletteCollection";

        public static void AddRecentColors(object sender, CircularList<Color> newColors)
        {
            if (bSettingColors || bSynchronizingCollections)
                return;
            bSettingColors = true;

            try
            {
                if (!SameColors(RecentColors, newColors))
                {
                    Copy(newColors, RecentColors);
                    RecentColorsChanged?.Invoke(sender, EventArgs.Empty);
                }
            }
            finally
            {
                bSettingColors = false;
            }
        }

        public static void AddRecentColor(object sender, Color newColor)
        {
            if (bSettingColors || bSynchronizingCollections)
                return;
            bSettingColors = true;

            try
            {
                if (!RecentColors.Contains(newColor))
                {
                    RecentColors.Add(newColor);
                    RecentColorsChanged?.Invoke(sender, EventArgs.Empty);
                }
            }
            finally
            {
                bSettingColors = false;
            }
        }

        public static void UpdateRecentColors(CircularList<Color> colors)
        {
            if (!SameColors(RecentColors, colors))
                Copy(RecentColors, colors);
        }

        enum PaletteType
        {
            Standard,
            Gradient
        }

        static void LoadCustomPalettes()
        {
            customPalettes = new List<ColorPalette>();
            string filepath = string.Format("{0}\\{1}.xml", Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), palettesFileName);
            if (File.Exists(filepath))
            {
                try
                {
                    var xml = XDocument.Load(@filepath);
                    List<ColorPalette> palettes = new List<ColorPalette>();
                    foreach (XElement node in xml.Root.Descendants("Palette").ToList())
                    {
                        var paletteTitle = node.Attribute("Title").Value;
                        PaletteType paletteType;

                        if (String.IsNullOrEmpty(paletteTitle) || !Enum.TryParse(node.Attribute("Type").Value, out paletteType))
                            continue;

                        var colors = new List<Color>();
                        foreach (XElement color in node.Descendants("Color").ToList())
                        {
                            var clr = RGBStringToColor(color.Value);
                            if (clr != null)
                                colors.Add((Color)clr);
                        }
                        if (paletteType == PaletteType.Gradient)
                            palettes.Add(CustomPalette.CreateGradientPalette(paletteTitle, new ColorCollection(colors)));
                        else
                            palettes.Add(new CustomPalette(paletteTitle, colors));
                    }
                    if (palettes.Count > 0)
                        customPalettes = new PaletteCollection(paletteCollectionName, palettes.ToArray());
                }
                catch (Exception ex) { }
            }
        }

        static Color? RGBStringToColor(string rgbString)
        {
            Color? ret = null;
            try
            {
                var splitString = rgbString.Split(',');
                var splitInts = splitString.Select(item => byte.Parse(item.Trim())).ToArray();
                ret = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);
            }
            catch { }
            return ret;
        }

        public static void SetPalettes(PopupColorEdit colorEdit)
        {
            PaletteCollection palettes = null;
            if (customPalettes == null)
                LoadCustomPalettes();
            if (customPalettes.Count > 0)
                palettes = new PaletteCollection(paletteCollectionName, customPalettes.ToArray());
            else
                palettes = new PaletteCollection(
                    paletteCollectionName,
                    CustomPalette.CreateGradientPalette("Theme Colors", new ColorCollection(
                        new List<Color> {
                        Color.FromRgb(255, 255, 255),
                        Color.FromRgb(255, 255, 0),
                        Color.FromRgb(0, 255, 0),
                        Color.FromRgb(0, 128, 0),
                        Color.FromRgb(255, 128, 0),
                        Color.FromRgb(255, 0, 0),
                        Color.FromRgb(128, 0, 128),
                        Color.FromRgb(0, 0, 255),
                        Color.FromRgb(0, 128, 255),
                        Color.FromRgb(0, 0, 0),
                        }
                    )),
                    new CustomPalette("Standard Colors",
                        new List<Color>() {
                            Color.FromRgb(139, 0, 0),
                            Color.FromRgb(255, 0, 0),
                            Color.FromRgb(255, 165, 0),
                            Color.FromRgb(255, 255, 0),
                            Color.FromRgb(144, 238, 144),
                            Color.FromRgb(0, 128, 0),
                            Color.FromRgb(173, 216, 230),
                            Color.FromRgb(0, 0, 255),
                            Color.FromRgb(0, 0, 139),
                            Color.FromRgb(128, 0, 128),
                        })
                );
            colorEdit.Palettes = palettes;
        }

        static bool SameColors(CircularList<Color> list1, CircularList<Color> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            for (var i = 0; i < list1.Count; i++)
                if (list2[i] != list1[i])
                    return false;

            return true;
        }

        public static void Copy<T>(ICollection<T> source, ICollection<T> destination)
        {
            bSynchronizingCollections = true;
            try
            {
                if (destination == null)
                    return;
                destination.Clear();
                foreach (var item in source)
                {
                    destination.Add(item);
                }
            }
            finally
            {
                bSynchronizingCollections = false;
            }
        }

        public static Dictionary<object, object> LoadBrushResources()
        {
            if (mapResourceBrushes != null)
                return mapResourceBrushes;

            mapResourceBrushes = new Dictionary<Object, Object>();
            var list = ResourceDictionaryExtensions.GetAllResourceFileList();
            list.ForEach(resource =>
            {
                try
                {
                    var map = ResourceDictionaryExtensions.LoadFromFile(resource, typeof(Brush));
                    foreach (var key in map.Keys)
                    {
                        if (!mapResourceBrushes.ContainsKey(key))
                            mapResourceBrushes.Add(key, map[key]);
                    }
                }
                catch (Exception ex)
                {

                }
            });
            return mapResourceBrushes;
        }
    }
}
