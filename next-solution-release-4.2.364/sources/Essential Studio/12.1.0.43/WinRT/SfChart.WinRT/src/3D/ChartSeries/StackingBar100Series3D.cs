#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Linq;
#if WINDOWS_PHONE
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class StackingBar100Series3D : StackingColumn100Series3D
    {
        internal override void OnTransposeChanged(bool val)
        {
            IsActualTransposed = !val;
        }

        public StackingBar100Series3D()
        {
            IsActualTransposed = true;
        }
    }
}
