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
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a selectable item within a <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBox"/> control.
    /// </summary>
    public class SfComboBoxItem:ComboBoxItem
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBoxItem"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBox"/>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Input">Syncfusion.UI.Xaml.Controls.Input
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public SfComboBoxItem()
        {
            DefaultStyleKey = typeof(SfComboBoxItem);
        }

      //  internal SfComboBox ParentComboBox { get; set; }
    }
}
