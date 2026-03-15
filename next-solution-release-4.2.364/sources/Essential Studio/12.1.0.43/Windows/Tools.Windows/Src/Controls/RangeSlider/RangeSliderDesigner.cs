#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    public class RangeSliderDesigner : ControlDesigner
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
                RangeSlider rangeSlider = this.Component as RangeSlider;
                if (rangeSlider.Orientation == Orientation.Horizontal)
                {
                    if (rangeSlider != null && rangeSlider.AutoSize)
                    {
                        result = SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable;
                    }
                }
                else
                {
                    if (rangeSlider != null && rangeSlider.AutoSize)
                    {
                        result = SelectionRules.TopSizeable | SelectionRules.BottomSizeable | SelectionRules.Moveable;
                    }
                }
                if (this.Locked)
                {
                    result = SelectionRules.Locked;
                }
                return result;
            }
        }

        private bool locked = false;
        public bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
            }
        }

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);
            properties["Locked"] = TypeDescriptor.CreateProperty(
               typeof(RangeSliderDesigner),
               (PropertyDescriptor)properties["Locked"],
               new Attribute[0]);
        }

        #endregion
    }
    public class RangeSliderConverter : TypeConverter
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
                RangeSlider rangeSlider = value as RangeSlider;
                if (rangeSlider != null)
                {
                    System.Reflection.ConstructorInfo ci = rangeSlider.GetType().GetConstructor(new Type[] { typeof(int), typeof(int) });
                    if (ci != null)
                    {
                        return new InstanceDescriptor(ci, new object[] { rangeSlider.Minimum, rangeSlider.Maximum });
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }
}
