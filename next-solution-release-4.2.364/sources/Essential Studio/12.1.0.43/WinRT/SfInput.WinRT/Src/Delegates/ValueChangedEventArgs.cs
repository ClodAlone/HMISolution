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
#if WPF
namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
namespace Syncfusion.Tools.Controls.Input
#else
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents the Arguments for <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.ValueChangedEventHandler"/>
    /// </summary>
#if !SILVERLIGHT
    [ClassReference(IsReviewed = false,ShouldInclude=false)]
#endif
    public class ValueChangedEventArgs: EventArgs
    {
        // Summary:
        //     Gets the new value of a range value property.
        //
        // Returns:
        //     The new value.
        private object newValue;

        /// <summary>
        /// Gets or sets the new value of a range value property.
        /// </summary>
        public object NewValue
        {
            get { return newValue; }
            set { newValue = value; }
        }

        //
        // Summary:
        //     Gets the previous value of a range value property.
        //
        // Returns:
        //     The previous value.
        private object oldValue;

        /// <summary>
        /// Gets or sets the previous value of a range value property.
        /// </summary>
        public object OldValue
        {
            get { return oldValue; }
            set { oldValue = value; }
        }
        
    }
}
