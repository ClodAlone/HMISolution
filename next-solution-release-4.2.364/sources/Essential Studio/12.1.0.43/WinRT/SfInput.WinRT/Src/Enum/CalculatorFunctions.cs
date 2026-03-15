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
#if WINDOWS_PHONE|| WINDOWS_PHONE_7
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
    /// Represents an enum list for the calculator functions
    /// </summary>
    public enum CalculatorFunctions
    {
        /// <summary>
        /// Add function
        /// </summary>
        Add,

        /// <summary>
        /// Subract function
        /// </summary>
        Subract,

        /// <summary>
        /// Multiply function
        /// </summary>
        Multiply,

        /// <summary>
        /// Divide function
        /// </summary>
        Divide,

        /// <summary>
        /// Back function
        /// </summary>
        Back,

        /// <summary>
        /// ClearEntry function
        /// </summary>
        ClearEntry,

        /// <summary>
        /// Clear function
        /// </summary>
        Clear,

        /// <summary>
        /// Percentage function
        /// </summary>
        Percentage,

        /// <summary>
        /// SquareRoot function
        /// </summary>
        SquareRoot,

        /// <summary>
        /// Reciproc function
        /// </summary>
        Reciproc,

        /// <summary>
        /// Memory function
        /// </summary>
        Memory,

        /// <summary>
        /// MemoryAdd function
        /// </summary>
        MemoryAdd,

        /// <summary>
        /// MemorySubract function
        /// </summary>
        MemorySubract,

        /// <summary>
        /// MemoryClear function
        /// </summary>
        MemoryClear,

        /// <summary>
        /// MemoryRecall function
        /// </summary>
        MemoryRecall,

        /// <summary>
        /// Return function
        /// </summary>
        Return,

        /// <summary>
        /// Sign function
        /// </summary>
        Sign,

        /// <summary>
        /// No function
        /// </summary>
        None
    }
}
