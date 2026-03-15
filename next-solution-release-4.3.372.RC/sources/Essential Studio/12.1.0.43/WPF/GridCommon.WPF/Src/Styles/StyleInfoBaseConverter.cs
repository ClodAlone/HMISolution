#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Interop;

namespace Syncfusion.Windows.Styles
{
    /// <summary>
    ///    <para>Provides a type converter to convert expandable objects to and from various
    ///       other representations.</para>
    /// </summary>
    public class StyleInfoBaseConverter :
        TypeConverter
    {
        /// <summary>
        ///    <para>Indicates whether this object supports properties using the
        ///       specified context.</para>
        /// </summary>
        public override /*TypeConverter*/ bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        } // end of method GetPropertiesSupported


        /// <summary>
        ///    <para>Indicates whether this converter can
        ///       convert an object to the given destination type using the specified context.</para>
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        } // end of method CanConvertTo

        /// <summary>
        ///    <para>Converts the given value object to
        ///       the specified destination type using the specified context and arguments.</para>
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return string.Empty;
        } // end of method ConvertTo

        /// <summary>
        ///    <para>Returns a collection of properties for
        ///       the type of array specified by the value parameter using the specified context and
        ///       attributes.</para>
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            StyleInfoBase styleInfo = value as StyleInfoBase;
            if (styleInfo != null)
            {
                PropertyDescriptorCollection tdpsc
                    = TypeDescriptor.GetProperties(value, attributes);

                ICollection sips = styleInfo.Store.StyleInfoProperties;
                Type styleInfoType = styleInfo.GetType();

                PropertyDescriptorCollection pdsc = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

                //PropertyDescriptor[] array = new PropertyDescriptor[tdpsc.Count];
                //tdpsc.CopyTo(array, 0);

                foreach (PropertyDescriptor pd in tdpsc)
                {
                    StyleInfoProperty sip = styleInfo.Store.FindStyleInfoProperty(pd.Name);
                    if (sip == null || sip.IsBrowsable)
                        pdsc.Add(pd);
                }

                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    foreach (StyleInfoProperty sip in sips)
                    {
                        if (sip.ComponentType != null && !sip.ComponentType.IsAssignableFrom(styleInfoType))
                        {
                            System.Reflection.PropertyInfo pi = sip.GetPropertyInfo();
                            if (sip.IsBrowsable && pi != null)
                            {
                                System.Attribute[] atts = (Attribute[])pi.GetCustomAttributes(typeof(Attribute), false);
                                pdsc.Add(new StyleInfoPropertyPropertyDescriptor(sip, sip.ComponentType, atts));
                            }
                        }
                    }
                }
                else
                {
                    foreach (StyleInfoProperty sip in sips)
                    {
                        if (sip.ComponentType != null && !sip.ComponentType.IsAssignableFrom(styleInfoType))
                        {
                            var pd = sip.GetPropertyDescriptor();
                            if (sip.IsBrowsable && pd != null)
                            {
                                var attrs = TypeDescriptor.GetAttributes(sip);
                                System.Collections.Generic.List<Attribute> atts = new System.Collections.Generic.List<Attribute>();
                                foreach (Attribute attr in attrs)
                                {
                                    atts.Add(attr);
                                }
                                pdsc.Add(new StyleInfoPropertyPropertyDescriptor(sip, sip.ComponentType, atts.ToArray()));
                            }
                        }
                    }
                }

                string[] sorts = styleInfo.Store.PropertyGridSortOrder;
                return pdsc.Sort(sorts);
            }

            return base.GetProperties(context, value, attributes);
        } // end of method GetProperties

    }

}
