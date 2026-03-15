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
using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScrollButtonsBar :Control
    {
        static ScrollButtonsBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollButtonsBar), new FrameworkPropertyMetadata(typeof(ScrollButtonsBar)));
        }

        public ScrollButtonsBar()
        {
           
        }
    }
}
