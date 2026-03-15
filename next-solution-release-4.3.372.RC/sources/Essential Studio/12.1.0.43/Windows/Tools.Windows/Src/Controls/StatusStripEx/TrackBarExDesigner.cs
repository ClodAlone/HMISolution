#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Designer for TrackBarEx.
    /// </summary>
   public class TrackBarExDesigner
        : ControlDesigner
    {
        #region Overrides
        /// <summary>
        /// Returns left and right sizers only when AutoSize is on.
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules result = base.SelectionRules;

                TrackBarEx trackBarEx = this.Component as TrackBarEx;

                if (trackBarEx != null && trackBarEx.AutoSize)
                {
                    result = SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable;
                }

                return result;
            }
        }
        #endregion
    }

   public class TrackBarExConverter : TypeConverter
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
                TrackBarEx trackBar = value as TrackBarEx;
                if (trackBar != null)
                {
                    System.Reflection.ConstructorInfo ci = trackBar.GetType().GetConstructor(new Type[] { typeof(int), typeof(int) });
                    if (ci != null)
                    {
                        return new InstanceDescriptor(ci, new object[] { trackBar.Minimum, trackBar.Maximum });
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }
}
#endif
