#region Copyright Syncfusion Inc. 2001 - 2014
// -----------------------------------------------------------------------
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// -----------------------------------------------------------------------
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// GridRangeInfoListConverter is a class that can be used to convert
    /// a range list to a string or vice versa. Access this
    /// class through the TypeDescriptor.
    /// </summary>
    public class GridRangeInfoListConverter : TypeConverter
    {
        // TypeConvert.ConvertTo is implemented in base class (and calls object.ToString()).

        /// <override/>
        public override bool CanConvertFrom(
            ITypeDescriptorContext context,
            Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <override/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string stringValue = (string)value;
                return GridRangeInfoList.FromString(stringValue.Trim());
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
