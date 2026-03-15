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


namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Invoked due to change in range
    /// </summary>
#if !SILVERLIGHT
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
#endif
    public class RangeChangedEventArgs: EventArgs
    {
        // Summary:
        //     Gets the new value of a state value property.
        //
        // Returns:
        //     The new value.
        private object newStartValue;

        /// <summary>
        /// Gets or sets the new value of a state value property.
        /// </summary>
        public object NewStartValue
        {
            get { return newStartValue; }
            set { newStartValue = value; }
        }

        //
        // Summary:
        //     Gets the previous value of a state value property.
        //
        // Returns:
        //     The previous value.
        private object oldStartValue;

        /// <summary>
        /// Gets or sets the previous value of a state value property.
        /// </summary>
        public object OldStartValue
        {
            get { return oldStartValue; }
            set { oldStartValue = value; }
        }

        // Summary:
        //     Gets the new value of a state value property.
        //
        // Returns:
        //     The new value.
        private object newEndValue;

        /// <summary>
        /// Gets or sets the new value of a state value property.
        /// </summary>
        public object NewEndValue
        {
            get { return newEndValue; }
            set { newEndValue = value; }
        }

        //
        // Summary:
        //     Gets the previous value of a state value property.
        //
        // Returns:
        //     The previous value.
        private object oldEndValue;

        /// <summary>
        /// Gets or sets the previous value of a state value property.
        /// </summary>
        public object OldEndValue
        {
            get { return oldEndValue; }
            set { oldEndValue = value; }
        }     
    }
}
