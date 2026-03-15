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
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Layout
#else
namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
{
    /// <summary>
    /// Represents a class for defining the changes in the state.
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class StateChangedEventArgs:EventArgs
    {
        // Summary:
        //     Gets the new value of a state value property.
        //
        // Returns:
        //     The new value.
        private object newValue;

        /// <summary>
        /// Gets or sets the new value
        /// </summary>
        public object NewValue
        {
            get { return newValue; }
            set { newValue = value; }
        }

        //
        // Summary:
        //     Gets the previous value of a state value property.
        //
        // Returns:
        //     The previous value.
        private object oldValue;

        /// <summary>
        /// Gets or sets the previous value
        /// </summary>
        public object OldValue
        {
            get { return oldValue; }
            set { oldValue = value; }
        }     
    }
}
