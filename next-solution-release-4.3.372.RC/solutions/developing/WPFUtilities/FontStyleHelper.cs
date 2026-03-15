using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utilities
{
    [Flags]
    public enum TextDecorators : byte
    {
        Underline = 0x1,
        Strikethrough = 0x2,
        OverLine = 0x4,
        Baseline = 0x8
    }

    public class FontStyleHelper
    {
        public static int[] sizes = new int[27] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 28, 36, 48, 72, 82, 92, 102, 112, 122, 132, 142, 160, 180, 200, 220, 240 };
        public static FontFamily customFontFamily;
        public static double customFontSize;
        public static bool hasCustomFontSize;
        public static bool hasCustomFontFamily;
        public static void InitializeFontSizeComboBox(ComboBox r_combo)
        {
            for (int i = 0, cnt = sizes.Length; i < cnt; ++i)
            {
                ComboBoxItem item = new ComboBoxItem { Content = sizes[i] };
                r_combo.Items.Add(item);
            }
        }

        public static void InitializeFontComboBox(ComboBox r_Combo)
        {
            var list = (from c in Fonts.SystemFontFamilies
                        orderby c.Source
                        select c).ToList();
            foreach (FontFamily el in list)
            {
                ComboBoxItem item = new ComboBoxItem { Content = el };
                item.FontFamily = el;
                r_Combo.Items.Add(item);
            }
        }
        public static void InitializeFontStyleComboBox(ComboBox r_Combo)
        {
            List<PropertyInfo> list = (from f in typeof(FontStyles).GetProperties() select f).ToList();
            FontStyleConverter ffc = new FontStyleConverter();
            foreach (PropertyInfo el in list)
            {
                try
                {
                    FontStyle fs = (FontStyle)ffc.ConvertFromString((string)el.Name);
                    ComboBoxItem item = new ComboBoxItem { Content = fs};
                    item.FontStyle = fs;
                    r_Combo.Items.Add(item);
                }
                catch (Exception ex)
                {
                }
            }
        }
        public static void InitializeFontWeightComboBox(ComboBox r_Combo)
        {
            List<PropertyInfo> list = (from f in typeof(FontWeights).GetProperties() select f).ToList();
            List<FontWeight> _list = new List<FontWeight>();
            FontWeightConverter ffc = new FontWeightConverter();
            foreach (PropertyInfo el in list)
            {
                try
                {
                    FontWeight fs = (FontWeight)ffc.ConvertFromString((string)el.Name);
                    if(!_list.Contains(fs))
                    {
                        _list.Add(fs);
                        ComboBoxItem item = new ComboBoxItem { Content = fs };
                        item.FontWeight = fs;
                        r_Combo.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
