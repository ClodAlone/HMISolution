// <copyright file="ColorItemList.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
  /// Contains list of system colors
  /// </summary>
    internal class xColorItemList : ObservableCollection<xColorItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorItemList"/> class.
        /// </summary>
        public xColorItemList()
            : base()
        {
            Type type = typeof(Brushes);
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (propertyInfo.PropertyType == typeof(SolidColorBrush))
                {
                    Add(new xColorItem(propertyInfo.Name, (SolidColorBrush)propertyInfo.GetValue(null, null)));
                }
            }
        }
    }
}
