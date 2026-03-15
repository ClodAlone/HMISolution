#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.IO;

#if WINRT
using Windows.UI.Text;
#else
using System.Windows.Data;
#endif

namespace Syncfusion.RDL.Internal
{
    internal class ReportingFontWeightConverter
    {
#if !SILVERLIGHT
        FontWeightConverter converter = new FontWeightConverter();
#endif
        public FontWeight ConvertFromString(string text)
        {
#if !SILVERLIGHT
            return (FontWeight)converter.ConvertFromString(text);
#else
            try
            {
                Type fontType = (typeof(FontWeights));
                object o = ReflectionHelper.GetValue(fontType, text);
                if (o != null)
                    return (FontWeight)o;
            }
            catch
            {
            }
            return FontWeights.Normal;
#endif
        }

        public FontWeight ConvertFromInvariantString(string text)
        {
#if !SILVERLIGHT
            return (FontWeight)converter.ConvertFromInvariantString(text);
#else
            return this.ConvertFromString(text);
#endif
        }
    }

    internal class ReportingFontStyleConverter
    {
#if !SILVERLIGHT
        FontStyleConverter converter = new FontStyleConverter();
#endif
        public FontStyle ConvertFromString(string text)
        {
#if !SILVERLIGHT
            return (FontStyle)converter.ConvertFromString(text);
#elif WINRT
            try
            {
                FontStyle style = (FontStyle)Enum.Parse(typeof(FontStyle), text);
                return style;
            }
            catch
            {
                return FontStyle.Normal;
            }
#else
            Type fontStyle = (typeof(FontStyles));
            object o = ReflectionHelper.GetValue(fontStyle, text);
            if (o != null)
                return (FontStyle)o;
            return FontStyles.Normal;
#endif
        }

        public FontStyle ConvertFromInvariantString(string text)
        {
#if !SILVERLIGHT
            return (FontStyle)converter.ConvertFromInvariantString(text);
#else
            return this.ConvertFromString(text);
#endif
        }
    }

#if WINRT
    internal class ReflectionHelper
    {
        public static object GetValue(Type type, string propertyName)
        {
            var propty = (from property in type.GetTypeInfo().DeclaredProperties
                          where property.Name == propertyName
                          select property).FirstOrDefault();

            if (propty != null)
            {
                object o = propty.GetValue(null);
                return o;
            }
            return null;
        }
    }
#elif SILVERLIGHT
    internal class ReflectionHelper
    {
        public static object GetValue(Type type, string property)
        {
            if (type.GetProperty(property) != null)
            {
                object o = type.InvokeMember(property, BindingFlags.GetProperty, null, null, null);
                return o;
            }
            return null;
        }
    }
#endif

    /// <summary>
    /// Represents constants for adding column type to identify the description of the column.
    /// </summary>
    internal class ReportingConstants
    {
        #region Constants

        /// <summary>
        /// Represents column type as Name.
        /// </summary>
        public const string NameColumn = "Name";

        /// <summary>
        /// Represents column type as Schema.
        /// </summary>
        public const string SchemaColumn = "Schema";

        /// <summary>
        /// Represents column type as Table.
        /// </summary>
        public const string TableNameColumn = "Table";

        #endregion
    }
}
