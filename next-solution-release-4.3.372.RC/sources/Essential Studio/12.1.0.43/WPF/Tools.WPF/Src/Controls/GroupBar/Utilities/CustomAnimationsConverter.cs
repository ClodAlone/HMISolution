// <copyright file="CustomAnimationsConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media.Animation;
using System.Windows;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using System.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Converts <see cref="string"/> objects to CustomAnimationsCollection objects.
    /// </summary>
    public class CustomAnimationsConverter : TypeConverter
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAnimationsConverter"/> class.
        /// </summary>
        public CustomAnimationsConverter()
        {
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Parses given string and creates collection of custom animations.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>The collection of custom animations.</returns>
        private CustomAnimationsCollection ParseString(string str)
        {
            CustomAnimationsCollection collection = new CustomAnimationsCollection();

            ////SourceName, {Class.}Event, Storyboard key
            string regexstring = @"(?'sourcename'\w+),\s*?((\w+\.)?(?'event'\w+)),\s*?(?'storyboard'\w+);?\s*?";
            Regex regex = new Regex(regexstring);
            MatchCollection matches = regex.Matches(str);
            foreach (Match match in matches)
            {
                string eventName = match.Groups["event"].Value;
                string storyboardKey = match.Groups["storyboard"].Value;
                string sourceName = match.Groups["sourcename"].Value;

                RoutedEvent routedEvent = null;
                foreach (RoutedEvent ev in EventManager.GetRoutedEvents())
                {
                    if (ev.Name == eventName)
                    {
                        routedEvent = ev;
                        break;
                    }
                }

                CustomAnimation animation = new CustomAnimation(sourceName, routedEvent, storyboardKey);

                collection.Add(animation);
            }

            return collection;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Returns whether the converter can convert an object of the <see cref="string"/> type 
        /// to the <see cref="CustomAnimationsCollection"/> type.
        /// </summary>
        /// <param name="context">Do not used.</param>
        /// <param name="sourceType">A <see cref="string"/> type that represents the type you want to convert from.</param>
        /// <returns>True if the converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        
        /// <summary>
        /// Returns whether the converter can convert the object to the <see cref="CustomAnimationsCollection"/> type.
        /// </summary>
        /// <param name="context">Do not used.</param>
        /// <param name="destinationType">A <see cref="CustomAnimationsCollection"/> type that represents the type you want to convert to.</param>
        /// <returns>True if the converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(CustomAnimationsCollection);
        }
        
        /// <summary>
        /// Converts the <see cref="string"/> object to the <see cref="CustomAnimationsCollection"/>.
        /// </summary>
        /// <param name="context">Do not used.</param>
        /// <param name="culture">do not used.</param>
        /// <param name="value">The <see cref="string"/> object to convert.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return ParseString(value as string);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="destinationType"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        /// <returns>The converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }
}
