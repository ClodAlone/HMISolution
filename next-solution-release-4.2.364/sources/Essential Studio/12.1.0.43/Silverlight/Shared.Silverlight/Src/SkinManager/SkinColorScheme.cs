#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Theming;
using System.Xml.Linq;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Data;

namespace Syncfusion.Windows.Controls.Theming
{
    internal class SkinColorScheme
    {

        #region Fields
        /// <summary>
        /// Color used for Skin.
        /// </summary>  
        public Color sskinColor = new Color();

        /// <summary>
        /// Used to stroe the value of resource dictionary.
        /// </summary>
        private static object value;
        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="SkinColorScheme"/> class.
        /// </summary>
        /// <param name="skinColor">Color of the skin.</param>
        public SkinColorScheme(Color skinColor)
        {
            sskinColor = skinColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinColorScheme"/> class.
        /// </summary>
        /// <param name="skinColor">Color of the skin.</param>
        public SkinColorScheme(int skinColor)
        {
            byte a = (byte)((skinColor & 0xff000000) >> 24);
            byte r = (byte)((skinColor & 0xff0000) >> 16);
            byte g = (byte)((skinColor & 0xff00) >> 8);
            byte b = (byte)(skinColor & 0xff);
            sskinColor = Color.FromArgb(a, r, g, b);
        }
        #endregion

        #region Implementation


        /// <summary>
        /// Returns skinned color from RGB value.
        /// </summary>
        /// <param name="rgb">RGB color to merge</param>
        /// <returns>Return the color</returns>
        internal Color GetColor(int rgb)
        {
            if (rgb == -1)
            {
                return new Color();
            }

            byte r = (byte)((rgb & 0xff0000) >> 16);
            byte g = (byte)((rgb & 0xff00) >> 8);
            byte b = (byte)(rgb & 0xff);

            return Color.FromArgb(0xff, MergeChannels(r, this.sskinColor.R), MergeChannels(g, this.sskinColor.G), MergeChannels(b, this.sskinColor.B));
        }

        /// <summary>
        /// Returns skinned color from Color value.
        /// </summary>
        /// <param name="baseColor">Base color to merge</param>
        /// <returns>Return the merged color</returns>
        internal Color GetColor(Color baseColor)
        {
            if (baseColor.A == 0)
            {
                return baseColor;
            }

            return Color.FromArgb(baseColor.A, MergeChannels(baseColor.R, sskinColor.R), MergeChannels(baseColor.G, sskinColor.G), MergeChannels(baseColor.B, sskinColor.B));
        }

        /// <summary>
        /// Returns skinned color from Color value.
        /// </summary>
        /// <param name="baseColor">Base color to merge</param>
        /// <param name="skinColor">skin color</param>
        /// <returns>Return the merged color</returns>
        internal static Color GetColor(Color baseColor, Color skinColor)
        {
            if (baseColor.A == 0)
            {
                return baseColor;
            }

            return Color.FromArgb(baseColor.A, MergeChannels(baseColor.R, skinColor.R), MergeChannels(baseColor.G, skinColor.G), MergeChannels(baseColor.B, skinColor.B));
        }

        /// <summary>
        /// Merges two color channels.
        /// </summary>
        /// <param name="baseChannel">Base channel to merge</param>
        /// <param name="skinChannel">skin channel to merge</param>
        /// <returns>Return the median for both channels</returns>
        internal static byte MergeChannels(int baseChannel, int skinChannel)
        {
            int mediana, dif, rest, max = 255;

            mediana = baseChannel * skinChannel / max;
            dif = ((max - baseChannel) * (max - skinChannel)) / max;
            rest = baseChannel * (max - dif - mediana);

            return (byte)(mediana + (rest / 255));
        }

        /// <summary>
        /// Merges two colors.
        /// </summary>
        /// <param name="color">color to merge</param>
        /// <param name="skinColor">Skin color of the control</param>
        /// <returns>Return the merged color</returns>
        internal static Color MergeChannels(Color color, Color skinColor)
        {
            return Color.FromArgb(color.A, MergeChannels(color.R, skinColor.R), MergeChannels(color.G, skinColor.G), MergeChannels(color.B, skinColor.B));
        }

        /// <summary>
        /// Clones the specified parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        /// <returns></returns>
        private static GradientStop clone(GradientStop parent, GradientStop child)
        {
            child.Color = parent.Color;
            child.Offset = parent.Offset;
            return child;
        }


        /// <summary>
        /// Applies the custom color scheme created with specified skin color to specified element.
        /// </summary>
        /// <param name="templatedictionary"></param>
        /// <param name="skinColor">Color of the skin.</param>
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

        /// <summary>
        /// Merges the colors.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="skinColor">Color of the skin.</param>
        /// <returns></returns>
        private static ResourceDictionary MergeColors(ResourceDictionary dictionary, Color skinColor)
        {
            SkinColorScheme colorScheme = new SkinColorScheme(skinColor);
            foreach (var key in dictionary.Keys)
            {
                value = dictionary[key];

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
                    else if (value is SolidColorBrush)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush();
                        (dictionary[key] as SolidColorBrush).Color = colorScheme.GetColor((value as SolidColorBrush).Color);

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
                }
            }
            return dictionary;
        }

        #endregion
    }
}
