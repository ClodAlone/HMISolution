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
using System.Reflection;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Extension for property info to handle some custom operation
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="propInfo">The prop info.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static object GetValue(this PropertyInfo propInfo, object obj)
        {
            return propInfo.GetValue(obj, null);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="propInfo">The prop info.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetValue(this PropertyInfo propInfo, object obj, object value)
        {
            // To Get the old/current value of the property
            object oldValue = propInfo.GetValue(obj);

            // To avoid enforcing the change for same value.
            if (!oldValue.Equals(value))
                propInfo.SetValue(obj, value, null);
        }
    }
}
