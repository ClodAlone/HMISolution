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
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// GridRangeInfoConverter is a class that can be used to convert
    /// ranges from one data type to another. Access this
    /// class through the TypeDescriptor.
    /// </summary>
    public class GridRangeInfoConverter : TypeConverter
    {
        /// <override/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string s = (string)value;
                s = s.Trim();

                if (s.Length == 0)
                {
                    return GridRangeInfo.Empty;
                }

                GridRangeInfo range = GridRangeInfo.Parse(s);
                
                if (range.IsEmpty)
                {
                    throw new ArgumentException("TextParseFailedFormat"); ////SR.GetString(SR.TextParseFailedFormat, s, SR.TopLeftBottomRight));
                }

                return range;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <override/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(@"destinationType");
            }

            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return string.Empty;
                }

                GridRangeInfo range = (GridRangeInfo)value;
                return range.ToString("G", null);
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }




    }
}

