using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DevExpress.LookAndFeel;
using DevExpress.Xpf.Core;
using Utilities;
using Utilities.WPF;

namespace WPFUtilities
{
    static public class ThemeHelper
    {
        static ThemeHelper()
        {
            LoadCustomThemes();
        }

        #region Windows Theme
        static void LoadCustomThemes()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.CustomThemes))
            {
                var assemblyNames = Properties.Settings.Default.CustomThemes.Split(';');
                foreach (var assemblyName in assemblyNames)
                {
                    try
                    {
                        var assembly = System.Reflection.Assembly.Load(assemblyName);
                        var themeName = assemblyName.Replace("DevExpress.Xpf.Themes.", String.Empty);
                        var index = themeName.IndexOf(".v");
                        if (index > 0)
                            themeName = themeName.Substring(0, index);
                        Theme theme = new Theme(themeName, assemblyName);
                        theme.AssemblyName = assemblyName;
                        Theme.RegisterTheme(theme);
                    }
                    catch
                    { }
                }
            }
        }

        static public void SetTheme(this Window wnd)
        {
            var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            if (currentStyle == "None")
            {
                return;
            }

            if (currentStyle != null)
            {
                //if (!bOnlyDevexpress)
                //{
                //    SkinStorage.SetEnableOptimization(wnd, true);

                //    if (currentStyle == "TouchlineDark")
                //    {
                //        SkinStorage.SetEnableTouch(wnd, true);
                //        SkinStorage.SetVisualStyle(wnd, "Blend");
                //    }
                //    else
                //    {
                //        SkinStorage.SetEnableTouch(wnd, false);
                //        SkinStorage.SetVisualStyle(wnd, currentStyle);
                //    }
                //}

                switch (currentStyle)
                {
                    case "Default": ThemeManager.SetThemeName(wnd, "DXStyle"); break;
                    case "Blend": ThemeManager.SetThemeName(wnd, "MetropolisDark"); break;
                    case "VS2010": ThemeManager.SetThemeName(wnd, "MetropolisLight"); break;
                    case "Office2007Black": ThemeManager.SetThemeName(wnd, "Office2007Black"); break;
                    case "Office2007Silver": ThemeManager.SetThemeName(wnd, "Office2007Silver"); break;
                    case "Office2007Blue": ThemeManager.SetThemeName(wnd, "Office2007Blue"); break;
                    case "Office2010Black": ThemeManager.SetThemeName(wnd, "Office2010Black"); break;
                    case "Office2010Silver": ThemeManager.SetThemeName(wnd, "Office2010Silver"); break;
                    case "Office2010Blue": ThemeManager.SetThemeName(wnd, "Office2010Blue"); break;
                    case "Office2013": ThemeManager.SetThemeName(wnd, "Office2013"); break;
                    case "TouchlineDark": ThemeManager.SetThemeName(wnd, "TouchlineDark"); break;
                    case "VS2017Light": ThemeManager.SetThemeName(wnd, "VS2019Light"); break;
                    case "VS2017Dark": ThemeManager.SetThemeName(wnd, "VS2019Dark"); break;
                    case "VS2017Dark2": ThemeManager.SetThemeName(wnd, "VS2019Dark2"); break;
                    default: ThemeManager.SetThemeName(wnd, "LightGray"); break;
                }
            }
            else
            {
                //if (!bOnlyDevexpress)
                //    SkinStorage.SetVisualStyle(wnd, "Default");
                ThemeManager.SetThemeName(wnd, "Seven");
            }

            var currentStyleBackColor = ApplicationPropertiesHelper.GetProperty<Brush>("CurrentSkinBackColor");
            if (currentStyleBackColor != null)
                wnd.Background = currentStyleBackColor;
        }

        static public String GetTheme(this Window wnd)
        {
            var currentStyle = ThemeManager.GetThemeName(wnd);
            switch (currentStyle)
            {
                case "DXStyle": return "Default";
                case "MetropolisDark": return "Blend";
                case "MetropolisLight": return "VS2010";
                case "Office2007Black": return "Office2007Black";
                case "Office2007Silver": return "Office2007Silver";
                case "Office2007Blue": return "Office2007Blue";
                case "Office2010Black": return "Office2010Black";
                case "Office2010Silver": return "Office2010Silver";
                case "Office2010Blue": return "Office2010Blue";
                case "Office2013": return "Office2013";
                case "TouchlineDark": return "TouchlineDark";
                case "VS2017Light":
                case "VS2019Light":
                    return "VS2017Light";
                case "VS2017Dark":
                case "VS2019Dark":
                    return "VS2017Dark";
                case "VS2017Dark2":
                case "VS2019Dark2":
                    return "VS2017Dark2";
                default: return "Seven";
            }
        }

        static public void SetTheme(this Window wnd, String currentStyle, 
            Brush currentStyleBackColor = null)
        {
            if (currentStyle == "None")
            {
                return;
            }
            if (!String.IsNullOrEmpty(currentStyle))
            {
                // SkinStorage.SetEnableOptimization(wnd, true);

                //if (currentStyle == "TouchlineDark")
                //{
                //    SkinStorage.SetEnableTouch(wnd, true);
                //    SkinStorage.SetVisualStyle(wnd, "Blend");
                //}
                //else
                //{
                //    SkinStorage.SetEnableTouch(wnd, false);
                //    SkinStorage.SetVisualStyle(wnd, currentStyle);
                //}

                switch (currentStyle)
                {
                    case "Default": ThemeManager.SetThemeName(wnd, "DXStyle"); break;
                    case "Blend": ThemeManager.SetThemeName(wnd, "MetropolisDark"); break;
                    case "VS2010": ThemeManager.SetThemeName(wnd, "MetropolisLight"); break;
                    case "Office2007Black": ThemeManager.SetThemeName(wnd, "Office2007Black"); break;
                    case "Office2007Silver": ThemeManager.SetThemeName(wnd, "Office2007Silver"); break;
                    case "Office2007Blue": ThemeManager.SetThemeName(wnd, "Office2007Blue"); break;
                    case "Office2010Black": ThemeManager.SetThemeName(wnd, "Office2010Black"); break;
                    case "Office2010Silver": ThemeManager.SetThemeName(wnd, "Office2010Silver"); break;
                    case "Office2010Blue": ThemeManager.SetThemeName(wnd, "Office2010Blue"); break;
                    case "Office2013": ThemeManager.SetThemeName(wnd, "Office2013"); break;
                    case "TouchlineDark": ThemeManager.SetThemeName(wnd, "TouchlineDark"); break;
                    case "VS2017Light": ThemeManager.SetThemeName(wnd, "VS2019Light"); break;
                    case "VS2017Dark": ThemeManager.SetThemeName(wnd, "VS2019Dark"); break;
                    case "VS2017Dark2": ThemeManager.SetThemeName(wnd, "VS2019Dark2"); break;
                    default: ThemeManager.SetThemeName(wnd, "LightGray"); break;
                }
            }
            else
            {
                //SkinStorage.SetVisualStyle(wnd, "Default");
                ThemeManager.SetThemeName(wnd, "Seven");
            }

            if (currentStyleBackColor != null)
                wnd.Background = currentStyleBackColor;
        }

        static public void SetTheme(this DependencyObject wnd, String currentStyle)
        {
            if (currentStyle == "None")
            {
                return;
            }
            if (!String.IsNullOrEmpty(currentStyle))
            {
                // SkinStorage.SetEnableOptimization(wnd, true);

                //if (currentStyle == "TouchlineDark")
                //{
                //    SkinStorage.SetEnableTouch(wnd, true);
                //    SkinStorage.SetVisualStyle(wnd, "Blend");
                //}
                //else
                //{
                //    SkinStorage.SetEnableTouch(wnd, false);
                //    SkinStorage.SetVisualStyle(wnd, currentStyle);
                //}

                switch (currentStyle)
                {
                    case "Default": ThemeManager.SetThemeName(wnd, "DXStyle"); break;
                    case "Blend": ThemeManager.SetThemeName(wnd, "MetropolisDark"); break;
                    case "VS2010": ThemeManager.SetThemeName(wnd, "MetropolisLight"); break;
                    case "Office2007Black": ThemeManager.SetThemeName(wnd, "Office2007Black"); break;
                    case "Office2007Silver": ThemeManager.SetThemeName(wnd, "Office2007Silver"); break;
                    case "Office2007Blue": ThemeManager.SetThemeName(wnd, "Office2007Blue"); break;
                    case "Office2010Black": ThemeManager.SetThemeName(wnd, "Office2010Black"); break;
                    case "Office2010Silver": ThemeManager.SetThemeName(wnd, "Office2010Silver"); break;
                    case "Office2010Blue": ThemeManager.SetThemeName(wnd, "Office2010Blue"); break;
                    case "Office2013": ThemeManager.SetThemeName(wnd, "Office2013"); break;
                    case "TouchlineDark": ThemeManager.SetThemeName(wnd, "TouchlineDark"); break;
                    case "VS2017Light": ThemeManager.SetThemeName(wnd, "VS2019Light"); break;
                    case "VS2017Dark": ThemeManager.SetThemeName(wnd, "VS2019Dark"); break;
                    case "VS2017Dark2": ThemeManager.SetThemeName(wnd, "VS2019Dark2"); break;
                    default: ThemeManager.SetThemeName(wnd, "LightGray"); break;
                }
            }
            else
            {
                //SkinStorage.SetVisualStyle(wnd, "Default");
                ThemeManager.SetThemeName(wnd, "Seven");
            }
        }

        static public String GetTheme(String style)
        {
            switch (style)
            {
                case "Dark": return Properties.Settings.Default.DarkThemeName;
                case "Light": return Properties.Settings.Default.LightThemeName;
                default: return "Default";
            }
        }

        #endregion

        #region Application Theme

        static bool applicationThemeChanged = false;
        static string applicationThemeName = null;

        static public void SetApplicationTheme()
        {
            var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            if (currentStyle != null)
            {
                applicationThemeName = ThemeManager.ApplicationThemeName;
                applicationThemeChanged = true;

                switch (currentStyle)
                {
                    case "Default": ThemeManager.ApplicationThemeName = "DXStyle"; break;
                    case "Blend": ThemeManager.ApplicationThemeName = "MetropolisDark"; break;
                    case "VS2010": ThemeManager.ApplicationThemeName = "MetropolisLight"; break;
                    case "Office2007Black": ThemeManager.ApplicationThemeName = "Office2007Black"; break;
                    case "Office2007Silver": ThemeManager.ApplicationThemeName = "Office2007Silver"; break;
                    case "Office2007Blue": ThemeManager.ApplicationThemeName = "Office2007Blue"; break;
                    case "Office2010Black": ThemeManager.ApplicationThemeName = "Office2010Black"; break;
                    case "Office2010Silver": ThemeManager.ApplicationThemeName = "Office2010Silver"; break;
                    case "Office2010Blue": ThemeManager.ApplicationThemeName = "Office2010Blue"; break;
                    case "Office2013": ThemeManager.ApplicationThemeName = "Office2013"; break;
                    case "TouchlineDark": ThemeManager.ApplicationThemeName = "TouchlineDark"; break;
                    case "VS2017Light": ThemeManager.ApplicationThemeName = "VS2019Light"; break;
                    case "VS2017Dark": ThemeManager.ApplicationThemeName = "VS2019Dark"; break;
                    case "VS2017Dark2": ThemeManager.ApplicationThemeName = "VS2019Dark2"; break;
                    default: ThemeManager.ApplicationThemeName = "LightGray"; break;
                }
            }
        }
        
        static public void ResetApplicationTheme()
        {
            ThemeManager.ApplicationThemeName = Theme.Default.Name;
            applicationThemeChanged = false;
        }

        static public void RestoreApplicationTheme()
        {
            if (applicationThemeChanged)
            {
                ThemeManager.ApplicationThemeName = applicationThemeName ?? Theme.Default.Name;
                applicationThemeChanged = false;
            }
        }

        static public Brush GetHilightingThemeBrush(string currentStyle, bool getSecondHilight = false)
        {
            switch (currentStyle)
            {
                case "Blend":
                case "VS2010":
                case "TouchlineDark":
                    if(getSecondHilight)
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFAC99B"));
                    else
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF47F13"));
                    break;
                default:
                    if (getSecondHilight)
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF80A4BA"));
                    else
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF257FB8"));
                    break;
            }
        }

        static public Brush GetGridHilightingThemeBrush(string currentStyle, bool getForeground)
        {
            if(getForeground)
                switch (currentStyle)
                {
                    case "VS2010":
                    case "None":
                    case "VS2017Dark":
                    case "VS2017Dark2":
                    case "VS2017Light":
                        return Brushes.White;
                        break;
                    default:
                        return Brushes.Black;
                        break;
                }
            else
                switch (currentStyle)
                {
                    case "Blend":
                    case "VS2010":
                    case "TouchlineDark":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF78A09"));
                        break;
                    case "None":
                    case "VS2017Dark":
                    case "VS2017Dark2":
                    case "VS2017Light":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
                        break;
                    case "Office2010Silver":
                    case "Office2010Black":
                    case "Office2010Blue":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF8E695"));
                        break;
                    case "Office2013":
                    case "Default":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD9E5FF"));
                        break;
                    default:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF257FB8"));
                        break;
                }
        }

        static public bool IsLightWeightThemeBrush(string currentStyle)
        {
            switch (currentStyle)
            {
                case "None":
                case "VS2017Dark":
                case "VS2017Dark2":
                case "TouchlineDark":
                case "Blend":
                    return true;
                    break;
                default:
                    return false;
                    break;
            }
        }

        static public Brush GetLightWeightThemeBrush(string currentStyle, bool isMinorBrush = false)
        {
            if (isMinorBrush)
                return new SolidColorBrush(Color.FromRgb(71,71,71));
            switch (currentStyle)
            {
                case "None":
                case "VS2017Dark":
                case "VS2017Dark2":
                case "TouchlineDark":
                case "Blend":
                    return Brushes.White;
                    break;
                default:
                    return Brushes.Black;
                    break;
            }
        }

        public static LinearGradientBrush GetLinear(Brush start, Brush end)
        {
            Color endColor = GetDefColor(end);
            Color startColor = GetDefColor(start);

            GradientStopCollection gradientStopCollection = new GradientStopCollection();
            GradientStop gradientStop1 = new GradientStop() { Color = startColor, Offset = 1 };
            GradientStop gradientStop2 = new GradientStop() { Color = endColor, Offset = 0 };
            gradientStopCollection.Add(gradientStop1);
            gradientStopCollection.Add(gradientStop2);

            LinearGradientBrush linearBrush = new LinearGradientBrush();
            linearBrush.GradientStops = gradientStopCollection;
            return linearBrush;
        }
        
        static Color GetDefColor(Brush brush)
        {
          Color color = brush is SolidColorBrush ? (brush as SolidColorBrush).Color : brush is GradientBrush ? (brush as GradientBrush).GradientStops.FirstOrDefault().Color : Colors.White;
            return color;
        }
    #endregion

    #region DevExpress LookAndFeel

    static public void SetDefaultUserLookAndFeel()
        {
            //var defaultLookAndFeel = new DevExpress.LookAndFeel.DefaultLookAndFeel();

            var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            if (currentStyle != null)
            {
                //defaultLookAndFeel.LookAndFeel.UseDefaultLookAndFeel = false;
                //defaultLookAndFeel.LookAndFeel.UseWindowsXPTheme = false;
                //defaultLookAndFeel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            
                switch (currentStyle)
                {
                    case "Default": UserLookAndFeel.Default.SkinName = "DevExpress Style"; break;
                    case "Blend": UserLookAndFeel.Default.SkinName = "Metropolis Dark"; break;
                    case "VS2010": UserLookAndFeel.Default.SkinName = "Metropolis"; break;
                    case "Office2007Black": UserLookAndFeel.Default.SkinName = "Office 2007 Black"; break;
                    case "Office2007Silver": UserLookAndFeel.Default.SkinName = "Office 2007 Silver"; break;
                    case "Office2007Blue": UserLookAndFeel.Default.SkinName = "Office 2007 Blue"; break;
                    case "Office2010Black": UserLookAndFeel.Default.SkinName = "Office 2010 Black"; break;
                    case "Office2010Silver": UserLookAndFeel.Default.SkinName = "Office 2010 Silver"; break;
                    case "Office2010Blue": UserLookAndFeel.Default.SkinName = "Office 2010 Blue"; break;
                    case "Office2013": UserLookAndFeel.Default.SkinName = "Office 2013"; break;
                    case "TouchlineDark": UserLookAndFeel.Default.SkinName = "Darkroom"; break;
                    case "VS2017Light": UserLookAndFeel.Default.SkinName = "Visual Studio 2013 Light"; break;
                    case "VS2017Dark": UserLookAndFeel.Default.SkinName = "Visual Studio 2013 Dark"; break;
                    case "VS2017Dark2": UserLookAndFeel.Default.SkinName = "Visual Studio 2013 Dark"; break;
                    default: UserLookAndFeel.Default.SkinName = "Sharp"; break;
                }
            }
        }

        #endregion
    }
}
