#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public sealed class SelectionChangedEventArgs : EventArgs
    {
        public SelectionChangedEventArgs()
        {

        }

        internal TextPosition StartPosition
        {
            get;
            set;
        }

        internal TextPosition EndPosition
        {
            get;
            set;
        }

        internal string SelectedText
        {
            get;
            set;
        }
    }
}
