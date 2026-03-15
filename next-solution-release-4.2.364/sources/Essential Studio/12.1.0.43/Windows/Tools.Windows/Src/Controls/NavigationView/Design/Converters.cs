#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools.Navigation.Design
{
    internal class BarConvernter :
        ExpandableObjectConverter
    {
        #region Overrides

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object obj;
            Bar bar = value as Bar;

            if (bar != null && destinationType == typeof(InstanceDescriptor))
            {
                MemberInfo ctor = typeof(Bar).GetConstructor(new Type[0]);

                obj = new InstanceDescriptor(ctor, new object[0], false);
            }
            else
            {
                obj = base.ConvertTo(context, culture, value, destinationType);
            }

            return obj;
        }

        #endregion
    }
}