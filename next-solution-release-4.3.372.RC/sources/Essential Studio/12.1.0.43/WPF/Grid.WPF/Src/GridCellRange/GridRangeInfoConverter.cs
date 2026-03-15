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
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// GridRangeInfo type, using the specified context.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <param name="sourceType">The type
        /// you want to convert from. </param>       
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        /// <summary>
        /// Converts the given object to the GridRangeInfo type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <param name="culture">Current culture information. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>        
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted range.
        /// </returns>
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
        /// <summary>
        /// Converts the given value object to the GridRangeInfo type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <param name="culture">Current culture information. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <param name="destinationType">The type to convert the
        /// value parameter to. </param>        
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted range.
        /// </returns>
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

        /// <override/>
        /// <summary>Creates an instance of the GridRangeInfo type, using the specified context, given a set of property
        /// values for the object.</summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="propertyValues">An System.Collections.IDictionary of new property values.</param>
        /// <returns>An System.Object representing the GridRange that this method creates.</returns>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            GridRangeInfoType type = (GridRangeInfoType)propertyValues["RangeType"];
            int top = (int)propertyValues["Top"];
            int left = (int)propertyValues["Left"];
            int bottom = (int)propertyValues["Bottom"];
            int right = (int)propertyValues["Right"];

            if (context != null)
            {
                // Find out what attribute changed 
                object o = context.PropertyDescriptor.GetValue(context.Instance);
                if (o is GridRangeInfo)
                {
                    GridRangeInfo oldRange = (GridRangeInfo)o;

                    if (type == oldRange.RangeType)
                    {
                        if (top != oldRange.Top)
                        {
                            if (top > bottom)
                            {
                                bottom = top;
                            }
                        }

                        if (bottom != oldRange.Bottom)
                        {
                            if (top > bottom)
                            {
                                top = bottom;
                            }
                        }

                        if (left != oldRange.Left)
                        {
                            if (left > right)
                            {
                                right = left;
                            }
                        }

                        if (right != oldRange.Right)
                        {
                            if (left > right)
                            {
                                left = right;
                            }
                        }
                    }
                }
            }

            return new GridRangeInfo(type, top, left, bottom, right);
        }

        /// <override/>
        /// <summary>Returns whether changing a value on this object requires a call to System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)
        /// to create a new value, using the specified context.</summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <returns>True.</returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <override/>
        /// <summary>Returns a collection of properties for the GridRangeInfo object, using the specified context and attributes.</summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="value">An System.Object that specifies the GridRangeInfo type for which to get properties.</param>
        /// <param name="attributes">An array of type System.Attribute that is used as a filter.</param>
        /// <returns>A System.ComponentModel.PropertyDescriptorCollection with the properties
        /// that are exposed for this GridRangeInfo type.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection propertyDescriptorCollection
                = TypeDescriptor.GetProperties(typeof(GridRangeInfo), attributes);

            string[] atts = new string[]
            {
                "RangeType",
                "Top",
                "Left",
                "Bottom",
                "Right"
            };

            return propertyDescriptorCollection.Sort(atts);
        }

        /// <override/>
        /// <summary>Returns whether changing a value on this object requires a call to System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)
        /// to create a new value, using the specified context.</summary>
        ///<param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        ///<returns>True.</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

