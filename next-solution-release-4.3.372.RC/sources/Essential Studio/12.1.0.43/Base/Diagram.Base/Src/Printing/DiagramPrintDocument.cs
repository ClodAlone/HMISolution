#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Print document object for a diagram.
    /// </summary>
    [
        ToolboxItem(false)
    ]
    public class DiagramPrintDocument : PrintDocument
    {
        private IPrint printObj = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramPrintDocument"/> class.
        /// </summary>
        /// <param name="printObj">An object implementing the <see cref="Syncfusion.Windows.Forms.Diagram.IPrint"/> interface. This is usually the diagram's model component.</param>
        public DiagramPrintDocument(IPrint printObj)
        {
            this.printObj = printObj;
        }

        /// <summary>
        /// Overridden. See <see cref="System.Drawing.Printing.PrintDocument.OnBeginPrint"/>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Printing.PrintEventArgs"/> that contains the event data.</param>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Drawing.Printing.PrintDocument.OnPrintPage"/>.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Drawing.Printing.PrintPageEventArgs"/> instance containing the event data.</param>
        protected override void OnPrintPage(PrintPageEventArgs evtArgs)
        {
            if (this.printObj != null)
            {
                Graphics grfx = evtArgs.Graphics;

                if (grfx.PageUnit == GraphicsUnit.Pixel)
                {
                    // Scale from screen resolution to printer resolution???
                }
                this.printObj.PrintPage(evtArgs);
            }
            base.OnPrintPage(evtArgs);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Drawing.Printing.PrintDocument.OnQueryPageSettings"/>.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Drawing.Printing.QueryPageSettingsEventArgs"/> instance containing the event data.</param>
        protected override void OnQueryPageSettings(QueryPageSettingsEventArgs evtArgs)
        {
            if (this.printObj != null)
            {
                this.printObj.QueryPageSettings(evtArgs);
            }
            base.OnQueryPageSettings(evtArgs);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Drawing.Printing.PrintDocument.OnEndPrint"/>.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Drawing.Printing.PrintEventArgs"/> instance containing the event data.</param>
        protected override void OnEndPrint(PrintEventArgs evtArgs)
        {
            base.OnEndPrint(evtArgs);
        }
    }
}
