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
#if !SILVERLIGHT && !WINDOWS_PHONE
using System.Threading.Tasks;
#endif
using System.Collections.ObjectModel;
using System.Windows;
#if WinRT
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public delegate void GridCopyPasteEventHandler(object sender, GridCopyPasteEventArgs e);


    public class GridCopyPasteEventArgs : GridEventArgs
    {
        public bool Handled { get; set; }
        public ClipBoardAction ClipBoardAction { get; internal set; }
#if WinRT
        public DataPackage DataPackage { get; set; }
#elif WPF
        public IDataObject DataObject { get; set; }
#else
        public string ClipBoardText { get; set; }
#endif
#if WPF

        public GridCopyPasteEventArgs(bool handled, IDataObject dataObject, ClipBoardAction action,
                                      object originalSource)
            : base(originalSource)
        {
            this.Handled = handled;
            this.DataObject = dataObject;
            this.ClipBoardAction = action;
        }
#elif WinRT
        public GridCopyPasteEventArgs(bool handled, DataPackage dataPackage, ClipBoardAction action,
                                      object originalSource)
            : base(originalSource)
        {
            this.Handled = handled;
            this.DataPackage = dataPackage;
            this.ClipBoardAction = action;
        }
#elif SILVERLIGHT
        public GridCopyPasteEventArgs(bool handled, string clipBoardText, ClipBoardAction action, object originalSource)
            : base(originalSource)
        {
            this.Handled = handled;
            this.ClipBoardText = clipBoardText;
            this.ClipBoardAction = action;
        }
#endif
    }
}
