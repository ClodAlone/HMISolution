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

#if WINDOWS_PHONE || WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#elif WPF

namespace Syncfusion.Windows.Controls.Input
#elif SILVERLIGHT

namespace Syncfusion.Tools.Controls.Input
#else
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a constant for each function in 
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.Calculator"/> control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class CalcConstatnts
    {
        /// <summary>
        /// Sets the value for CAdd
        /// </summary>
        public static readonly string CAdd = "+";

        /// <summary>
        /// Sets the value for CMinus
        /// </summary>
        public static readonly string CMinus = "-";

        /// <summary>
        /// Sets the value for CMultiply
        /// </summary>
        public static readonly string CMultiply = "*";

        /// <summary>
        /// Sets the value for CDivide
        /// </summary>
        public static readonly string CDivide = "/";

        /// <summary>
        /// Sets the value for CSquareRoot
        /// </summary>
        public static readonly string CSquareRoot = "sqrt({0})";

        /// <summary>
        /// Sets the value for CReciproc
        /// </summary>
        public static readonly string CReciproc = "reciproc({0})";

        /// <summary>
        /// Sets the value for CError
        /// </summary>
        public static readonly string CError = "ERROR";

        /// <summary>
        /// Sets the value for CInvalid
        /// </summary>
        public static readonly string CInvalid="Invalid input";
    }
}
