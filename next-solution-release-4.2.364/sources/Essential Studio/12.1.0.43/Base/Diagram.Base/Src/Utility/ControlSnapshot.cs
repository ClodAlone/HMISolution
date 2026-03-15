#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices.WinAPI;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Control Snap shot.
    /// </summary>
    [Documentation.DocumentationExclude()]
    public class ControlSnapshot
    {
        #region Constants
        //private const int HORZSIZE = 4;
        //private const int VERTSIZE = 6;
        //private const int HORZRES = 8;
        //private const int VERTRES = 10;
        //private const int SRCCOPY = 13369376; // 0x00CC0020
        #endregion

        #region static methods
        /// <summary>
        /// Captures the contents of a Windows Forms control using the WM_PRINTCLIENT or WM_PRINT message.
        /// </summary>
        /// <param name="control">The control to be captured.</param>
        /// <param name="size">The size of the window.</param>
        /// <returns>A Metafile with the display contents of the Windows Forms control.</returns>
        public static Image PrintControl(Control control, Size size)
        {
            if (control != null)
            {
                Size controlSize = control.Size;
                if (size != control.Size)
                    control.Size = size;
                Control parentCtrl = control.Parent;
                control.Parent = null;
                //new bitmap object to save the image        
                Bitmap bmp = new Bitmap(size.Width, size.Height);
                //Drawing control to the bitmap
                control.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, size.Width, size.Height));
                if (control.Size != controlSize)
                    control.Size = controlSize;
                if (control.BackColor == Color.Transparent)
                    bmp.MakeTransparent();
                control.Parent = parentCtrl;
                return bmp;
            }
            else 
                return null;
        }
        #endregion
    }
}
