#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    ///  MonthCalendarAdvConverter class.
    /// </summary>
    public class MonthCalendarAdvConverter : /*System.ComponentModel.ComponentConverter //*/ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (context != null && context.Instance as DateTimePickerAdv != null)
            {
                System.ComponentModel.PropertyDescriptorCollection pds
                    = TypeDescriptor.GetProperties(typeof(MonthCalendarAdv), attributes);
                if (((DateTimePickerAdv)context.Instance).Calendar.DateTimePickerCalendar)
                {
                    try
                    {
                        // pds.Remove(pds.Find("AllowSelection",false));
                        ArrayList props = new ArrayList();
                        for (int i = 0; i < pds.Count; i++)
                        {
                            if (pds[i].DisplayName != "AllowSelection" && pds[i].DisplayName != "Culture" && pds[i].DisplayName != "SizeToFit")
                            {
                                props.Add(pds[i]);
                            }
                        }
                        pds = new PropertyDescriptorCollection(props.ToArray(typeof(PropertyDescriptor)) as PropertyDescriptor[]);
                    }
                    catch 
                    { 
                    }
                }
                return pds;
            }
            else
                return base.GetProperties(context, value, attributes);
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
