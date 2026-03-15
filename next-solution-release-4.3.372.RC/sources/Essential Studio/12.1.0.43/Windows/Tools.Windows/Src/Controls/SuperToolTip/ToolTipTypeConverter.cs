#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    #region ToolTipTypeConverter
   public class ToolTipTypeConverter : ExpandableObjectConverter
    {
        #region Overrides
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo ci = typeof(ToolTipInfo).GetConstructor(new Type[] { });
                return new InstanceDescriptor(ci, new object[] { }, false);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }
    #endregion

    #region SuperToolTipTypeConverter
   public class SuperToolTipTypeConverter : TypeConverter
    {
        #region Overrides
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { typeof(System.Windows.Forms.Form) });

                System.Windows.Forms.Form owner = null;

                Component component = value as Component;

                if (component != null && component.Site != null)
                {
                    IDesignerHost host = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (host != null)
                    {
                        owner = host.RootComponent as System.Windows.Forms.Form;
                    }
                }

                return new InstanceDescriptor(ci, new object[] { owner });
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }
    #endregion
}

#endif