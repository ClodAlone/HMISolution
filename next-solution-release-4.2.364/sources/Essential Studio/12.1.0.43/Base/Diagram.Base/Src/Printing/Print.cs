#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing.Printing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// This interface is implemented by classes that can print.
    /// </summary>
    public interface IPrint
    {
        /// <summary>
        /// Called in response to the <see cref="System.Drawing.Printing.PrintDocument.PrintPage"/> event.
        /// </summary>
        /// <remarks>
        /// This method is called when the output to print for the current page is needed.
        /// </remarks>
        /// <param name="evtArgs">A <see cref="System.Drawing.Printing.PrintPageEventArgs"/> value.</param>
        void PrintPage(PrintPageEventArgs evtArgs);

        /// <summary>
        /// Called immediately before each <see cref="System.Drawing.Printing.PrintDocument.PrintPage"/> event.
        /// </summary>
        /// <param name="evtArgs">A <see cref="System.Drawing.Printing.QueryPageSettingsEventArgs"/> value.</param>
        void QueryPageSettings(QueryPageSettingsEventArgs evtArgs);
    }
}
