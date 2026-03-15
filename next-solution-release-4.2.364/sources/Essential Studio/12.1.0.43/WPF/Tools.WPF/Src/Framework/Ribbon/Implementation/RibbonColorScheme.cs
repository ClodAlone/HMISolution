// <copyright file="RibbonColorScheme.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Provides a Ribbon color scheme.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class RibbonColorScheme
    {
        #region Class constants
        /// <summary>
        /// Key for search target.
        /// </summary>
        private const string C_Key = @"/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Generic.xaml";
        #endregion

        #region Fields
        /// <summary>
        /// Color used for blending.
        /// </summary>
        public Color m_blendColor = new Color();

        /// <summary>
        /// Used for accessing the value
        /// </summary>
        private static object value;

        /// <summary>
        /// String used for Storing ActiveSkin
        /// </summary>
        //private static string activeskin;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="RibbonColorScheme"/> class.
        /// </summary>
        static RibbonColorScheme()
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(C_Key, UriKind.RelativeOrAbsolute);
            //m_dictList = (Shared.DictionaryList)dictionary["SkinDictionary"];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonColorScheme"/> class.
        /// </summary>
        /// <param name="blendColor">Color of the blend.</param>
        public RibbonColorScheme(Color blendColor)
        {
            m_blendColor = blendColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonColorScheme"/> class.
        /// </summary>
        /// <param name="blendColor">Color of the blend.</param>
        public RibbonColorScheme(int blendColor)
        {
            byte a = (byte)((blendColor & 0xff000000) >> 24);
            byte r = (byte)((blendColor & 0xff0000) >> 16);
            byte g = (byte)((blendColor & 0xff00) >> 8);
            byte b = (byte)(blendColor & 0xff);

            m_blendColor = Color.FromArgb(a, r, g, b);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Applies custom color scheme created with specified blend color to specified element.
        /// </summary>
        /// <param name="element">Dependency object to which custom color scheme will be applied.</param>
        /// <param name="skinelement">The skinelement.</param>
        /// <param name="blendColor">Blend color value.</param>
        /// <seealso cref="Color"/>
        public static ResourceDictionary ApplyCustomColorScheme(ResourceDictionary templatedictionary, Color skinColor)
        {
            ResourceDictionary dictionary = null;
            if (skinColor != new Color())
            {
                foreach (ResourceDictionary mergeddic in templatedictionary.MergedDictionaries)
                {
                    if (mergeddic.Source.ToString().EndsWith("Brushes.xaml"))
                    {
                        dictionary = mergeddic;
                        break;
                    }
                }

                if (dictionary != null)
                {
                    dictionary = MergeColors(dictionary, skinColor);
                    templatedictionary.MergedDictionaries.Remove(dictionary);
                    templatedictionary.MergedDictionaries.Add(dictionary);
                }

                return MergeColors(templatedictionary, skinColor);
            }
            else
            {
                return templatedictionary;
            }
        }

        private static GradientStop clone(GradientStop parent, GradientStop child)
        {
            child.Color = parent.Color;
            child.Offset = parent.Offset;
            return child;
        }




        private static ResourceDictionary MergeColors(ResourceDictionary dictionary, Color skinColor)
        {
            RibbonColorScheme colorScheme = new RibbonColorScheme(skinColor);
            foreach (object key in dictionary.Keys)
            {
                value = dictionary[Convert.ToString(key)];

                if (value != null)
                {
                    if (value is LinearGradientBrush)
                    {
                        LinearGradientBrush brush = value as LinearGradientBrush;
                        LinearGradientBrush newBrush = new LinearGradientBrush();

                        newBrush.StartPoint = brush.StartPoint;
                        newBrush.EndPoint = brush.EndPoint;
                        newBrush.Transform = brush.Transform;

                        foreach (GradientStop stop in brush.GradientStops)
                        {
                            GradientStop newStop = new GradientStop();
                            newStop.Offset = stop.Offset;
                            newStop.Color = colorScheme.GetColor(stop.Color);
                            newBrush.GradientStops.Add(newStop);
                        }

                        try
                        {

                            (dictionary[key] as LinearGradientBrush).StartPoint = newBrush.StartPoint;
                            (dictionary[key] as LinearGradientBrush).EndPoint = newBrush.EndPoint;
                            (dictionary[key] as LinearGradientBrush).Transform = newBrush.Transform;
                            (dictionary[key] as LinearGradientBrush).GradientStops.Clear();

                            foreach (GradientStop newstops in newBrush.GradientStops)
                            {
                                GradientStop stop = clone(newstops, new GradientStop());
                                (dictionary[key] as LinearGradientBrush).GradientStops.Add(stop);
                            }
                        }
                        catch { }
                    }
                    else if (value is SolidColorBrush)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush();
                        try
                        {
                            (dictionary[key] as SolidColorBrush).Color = colorScheme.GetColor((value as SolidColorBrush).Color);
                        }
                        catch { }
                    }
                    else if (value is RadialGradientBrush)
                    {
                        RadialGradientBrush brush = value as RadialGradientBrush;
                        RadialGradientBrush newBrush = new RadialGradientBrush();

                        newBrush.Center = brush.Center;
                        newBrush.GradientOrigin = brush.GradientOrigin;
                        newBrush.RadiusX = brush.RadiusX;
                        newBrush.RadiusY = brush.RadiusY;
                        newBrush.RelativeTransform = brush.RelativeTransform;

                        foreach (GradientStop stop in brush.GradientStops)
                        {
                            GradientStop newStop = new GradientStop();
                            newStop.Offset = stop.Offset;
                            newStop.Color = colorScheme.GetColor(stop.Color);
                            newBrush.GradientStops.Add(newStop);
                        }

                        try
                        {
                            (dictionary[key] as RadialGradientBrush).Center = newBrush.Center;
                            (dictionary[key] as RadialGradientBrush).GradientOrigin = newBrush.GradientOrigin;
                            (dictionary[key] as RadialGradientBrush).RelativeTransform = newBrush.RelativeTransform;
                            (dictionary[key] as RadialGradientBrush).RadiusX = newBrush.RadiusX;
                            (dictionary[key] as RadialGradientBrush).RadiusY = newBrush.RadiusY;
                            (dictionary[key] as RadialGradientBrush).GradientStops.Clear();

                            foreach (GradientStop newstops in newBrush.GradientStops)
                            {
                                GradientStop stop = clone(newstops, new GradientStop());
                                (dictionary[key] as RadialGradientBrush).GradientStops.Add(stop);
                            }

                        }
                        catch { }
                    }
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <param name="rgb">The RGB color value.</param>
        /// <returns>Returns the Color</returns>
        internal Color GetColor(int rgb)
        {
            if (rgb == -1)
            {
                return new Color();
            }

            byte r = (byte)((rgb & 0xff0000) >> 16);
            byte g = (byte)((rgb & 0xff00) >> 8);
            byte b = (byte)(rgb & 0xff);
            return Color.FromArgb(0xff, MergeChannels(r, this.m_blendColor.R), MergeChannels(g, this.m_blendColor.G), MergeChannels(b, this.m_blendColor.B));
        }

        /// <summary>
        /// Returns blended color from Color value.
        /// </summary>
        /// <param name="baseColor">Color of the base.</param>
        /// <returns>blended color</returns>
        internal Color GetColor(Color baseColor)
        {
            if (baseColor.A == 0)
            {
                return baseColor;
            }

            return Color.FromArgb(baseColor.A, MergeChannels(baseColor.R, m_blendColor.R), MergeChannels(baseColor.G, m_blendColor.G), MergeChannels(baseColor.B, m_blendColor.B));
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <param name="baseColor">Color of the base.</param>
        /// <param name="blendColor">Color of the blend.</param>
        /// <returns>Returns the Color</returns>
        internal static Color GetColor(Color baseColor, Color blendColor)
        {
            if (baseColor.A == 0)
            {
                return baseColor;
            }

            return Color.FromArgb(baseColor.A, MergeChannels(baseColor.R, blendColor.R), MergeChannels(baseColor.G, blendColor.G), MergeChannels(baseColor.B, blendColor.B));
        }

        /// <summary>
        /// Merges the channels.
        /// </summary>
        /// <param name="baseChannel">The base channel.</param>
        /// <param name="blendChannel">The blend channel.</param>
        /// <returns>returns the merge channels</returns>
        internal static byte MergeChannels(int baseChannel, int blendChannel)
        {
            int mediana, dif, rest, max = 255;

            mediana = baseChannel * blendChannel / max;
            dif = (max - baseChannel) * (max - blendChannel) / max;
            rest = baseChannel * (max - dif - mediana);

            return (byte)(mediana + (rest / 255));
        }

        /// <summary>
        /// Merges the channels.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="blendColor">Color of the blend.</param>
        /// <returns>Merge channels</returns>
        internal static Color MergeChannels(Color color, Color blendColor)
        {
            return Color.FromArgb(color.A, MergeChannels(color.R, blendColor.R), MergeChannels(color.G, blendColor.G), MergeChannels(color.B, blendColor.B));
        }

        #endregion
    }

    /// <summary>
    /// Markup extension for work with skin visual style list.
    /// </summary>
    /// <exclude/>
    [MarkupExtensionReturnType(typeof(object))]
    public class RibbonSkinObjectExtension : MarkupExtension
    {
        #region Fields
        /// <summary>
        /// Defines the visual style list converter
        /// </summary>
        private static VisualStylesListConverter m_converter = new VisualStylesListConverter();

        /// <summary>
        /// Represents the Binding objects
        /// </summary>
        private static List<DependencyObject> m_bindingObjects = new List<DependencyObject>();
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonSkinObjectExtension"/> class.
        /// </summary>
        public RibbonSkinObjectExtension()
        {
        }
        #endregion

        #region Public method

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding b = new Binding();
            b.RelativeSource = new RelativeSource(RelativeSourceMode.Self); 
            b.Converter = m_converter;
            //b.Path = new PropertyPath(SkinStorage.VisualStylesListProperty);
            object obj = b.ProvideValue(serviceProvider);
            return obj;
        }

        /// <summary>
        /// Updates the bindings.
        /// </summary>
        internal static void UpdateBindings()
        {
            //foreach (DependencyObject dpObj in m_bindingObjects)
            //{
            //    Binding parentBinding = BindingOperations.GetBindingExpression(dpObj, SkinStorage.VisualStylesListProperty).ParentBinding;
            //    BindingOperations.ClearBinding(dpObj, SkinStorage.VisualStylesListProperty);
            //    BindingOperations.SetBinding(dpObj, SkinStorage.VisualStylesListProperty, parentBinding);
            //}
        }
        #endregion
    }

    /// <summary>
    /// Provides a way to apply custom logic to a binding data
    /// </summary>
    /// <exclude/>
    public class VisualStylesListConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding data source.</param>
        /// <param name="targetType">The type of the binding data target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (value == null)
            //{
            //    return RibbonColorScheme.CommonSkinDictionaryList;
            //}
            //else
            //{
                return value;
            //}
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding data target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("This is only one way converter.");
        }

        #endregion
    }
}
