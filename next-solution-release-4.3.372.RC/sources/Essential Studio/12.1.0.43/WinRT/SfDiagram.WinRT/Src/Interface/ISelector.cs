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
using System.Windows;

#if WINRT_USING
using System.Threading.Tasks;
using Syncfusion.UI.Xaml.Diagram.Panels;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using Windows.UI.Xaml;
#else
using System.Windows.Input;
using PointerRoutedEventArgs = System.Windows.Input.MouseEventArgs;
using TappedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface ISelector : IGroup
    {
        Visibility QuickCommands { get; set; }
    }

    public interface ISelectorInfo : 
        IGroupInfo
    {
    }

    internal interface IInternalSelector :
        ISelector,
        ISelectorInfo,
        IInternalGroup
    {
        void MovePivotDelta(Point delta);
        bool HasItems { get; }
        void ClearSelection();
        IInternalGroupable FirstSelectedItem { get; set; }
        void ManualPressTargetThumb(MouseEventArgs e);
    }
}
