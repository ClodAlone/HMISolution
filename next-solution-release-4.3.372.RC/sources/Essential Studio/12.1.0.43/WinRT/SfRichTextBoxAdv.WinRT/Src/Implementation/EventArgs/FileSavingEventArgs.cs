#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;
#if WPF
#else
using Windows.UI.Xaml;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class FileSavingEventArgs : RoutedEventArgs
    {
        public FileSavingEventArgs()
        {

        }

        public Stream DoucmentStream
        {
            get;
            set;
        }

        public string FormatType
        {
            get;
            internal set;
        }

#if !WPF
        public bool Handled
        {
            get;
            set;
        }
#endif
    }
}
