#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// IDateTimePickerAdvMenu interface.
    /// </summary>
    public interface IDateTimePickerAdvMenu
    {
        event EventHandler Cut;
        event EventHandler Copy;
        event EventHandler Paste;
        event EventHandler NoDateTime;
        event EventHandler Popup;

        bool ShowNoDateTime
        {
            get;
            set;
        }

        bool CopyEnabled
        {
            get;
            set;
        }
        bool CutEnabled
        {
            get;
            set;
        }
        bool PasteEnabled
        {
            get;
            set;
        }
        bool NoDateTimeEnabled
        {
            get;
            set;
        }
        bool NoDateTimeChecked
        {
            get;
            set;
        }
        void ShowPopup(System.Windows.Forms.Control control, System.Drawing.Point pt);
    }
}
