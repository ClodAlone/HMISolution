// <copyright file="Utils.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// Utils contains static method to perform utility functions
    /// </summary>
    /// <remarks>
    /// The Util class used to bound the objects and also it has format methods like GetWidth, GetFormattedText.
    /// </remarks>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public static class Utils
    {
        /// <summary>
        /// GetWidth method used to calculate the text width based on the text, fontfamily, fontsize and
        /// foreground
        /// </summary>
        /// <remarks>
        /// This method used to calculate the width of the text.
        /// </remarks>
        /// <param name="text">The text to format.</param>
        /// <param name="fontFamily">Fontfamily of the text.</param>
        /// <param name="fontSize">Fontsize of the text.</param>
        /// <param name="foreground">Foreground of the text.</param>
        /// <returns>
        /// Returns the formatted text width.
        /// </returns>
        public static double GetWidth(string text, FontFamily fontFamily, double fontSize, Brush foreground)
        {
            return double.Parse(string.Format("{0:0.00}", Utils.GetFormattedText(text, fontFamily, fontSize, foreground).WidthIncludingTrailingWhitespace));
        }

        /// <summary>
        /// GetHeight method used to calculate the text height based on the text, fontfamily, fontsize and
        /// foreground
        /// </summary>
        /// <remarks>
        /// This method used to calculate the width of the text.
        /// </remarks>
        /// <param name="text">The text to format.</param>
        /// <param name="fontFamily">Fontfamily of the text.</param>
        /// <param name="fontSize">Fontsize of the text.</param>
        /// <param name="foreground">Foreground of the text.</param>
        /// <returns>
        /// Returns the formatted text width.
        /// </returns>
        public static double GetHeight(string text, FontFamily fontFamily, double fontSize, Brush foreground)
        {
            return double.Parse(string.Format("{0:0.00}", Utils.GetFormattedText(text, fontFamily, fontSize, foreground).Baseline));
        }

        /// <summary>
        /// GetFormattedText methods is used to get the formatted text based on the text, fontfamily, fontsize,
        /// foreground parameters.
        /// </summary>
        /// <remarks>
        /// This method used to get the formatted text with the specified fontfamily,
        /// fontsize and foreground.
        /// </remarks>
        /// <param name="text">The text to be format.</param>
        /// <param name="fontFamily">Fontfamily of the text.</param>
        /// <param name="fontSize">Fontsize of the text.</param>
        /// <param name="foreground">Foreground of the text.</param>
        /// <returns>
        /// Returns the formatted text.
        /// </returns>
        public static FormattedText GetFormattedText(string text, FontFamily fontFamily, double fontSize, Brush foreground)
        {
            FormattedText formattedtext = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(fontFamily.ToString()), fontSize, foreground);
            //formattedtext.SetFontStyle(FontStyles.Normal);
            //formattedtext.SetFontWeight(FontWeights.Normal);
            return formattedtext;
        }

        /// <summary>
        /// Helper method to bind objects with updatesourcetrigger
        /// </summary>
        /// <remarks>
        /// This method used to bind the target object from the source object.
        /// </remarks>
        /// <param name="target">The target.</param>
        /// <param name="property">Dependency property.</param>
        /// <param name="path">The path of the Source.</param>
        /// <param name="source">The source to bind.</param>
        /// <param name="mode">The mode of binding.</param>
        /// <param name="trigger">The update source trigger.</param>
        internal static void BindObjects(DependencyObject target, DependencyProperty property, string path, object source, BindingMode mode, UpdateSourceTrigger trigger)
        {
            Binding binding = new Binding(path);
            binding.Source = source;
            binding.Mode = mode;
            binding.UpdateSourceTrigger = trigger;
            BindingOperations.SetBinding(target, property, binding);
        }

        /// <summary>
        /// Helper method to bind objects with source, property, target and propertypath
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="property">Dependency property.</param>
        /// <param name="path">The path of the Source.</param>
        /// <param name="source">The source to bind.</param>
        internal static void BindObjects(DependencyObject target, DependencyProperty property, string path, object source)
        {
            Binding binding = new Binding(path);
            binding.Source = source;
            BindingOperations.SetBinding(target, property, binding);
        }

        /// <summary>
        /// Bind a EditControl property with the the target object property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="property">Dependency property.</param>
        /// <param name="path">The path of the Source.</param>
        internal static void BindObjectsToControl(DependencyObject target, DependencyProperty property, string path)
        {
            Binding binding = new Binding(path);
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) { AncestorType = typeof(EditControl) };
            BindingOperations.SetBinding(target, property, binding);
        }

        /// <summary>
        /// Bind a EditControl property with the target object property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="property">Dependency property.</param>
        /// <param name="path">The path of the Source.</param>
        /// <param name="mode">The mode of binding.</param>
        /// <param name="trigger">The trigger.</param>
        internal static void BindObjectsToControl(DependencyObject target, DependencyProperty property, string path, BindingMode mode, UpdateSourceTrigger trigger)
        {
            Binding binding = new Binding(path);
            RelativeSource relsource = new RelativeSource(RelativeSourceMode.FindAncestor);
            relsource.AncestorType = typeof(EditControl);
            binding.RelativeSource = relsource;
            binding.Mode = mode;
            binding.UpdateSourceTrigger = trigger;
            BindingOperations.SetBinding(target, property, binding);
        }
    }
}