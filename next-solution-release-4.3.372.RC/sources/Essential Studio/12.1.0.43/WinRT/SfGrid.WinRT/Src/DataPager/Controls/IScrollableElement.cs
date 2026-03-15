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

#if WinRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Controls.DataPager
{
    internal interface IScrollableElement
    {
        FrameworkElement Element { get; }

        int Index { get; }

        bool IsElipsisElement { get; }
    }
}