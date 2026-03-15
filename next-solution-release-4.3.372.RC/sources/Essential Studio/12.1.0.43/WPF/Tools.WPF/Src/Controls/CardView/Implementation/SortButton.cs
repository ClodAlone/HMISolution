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
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
   internal class SortButton : Button
    {
        public SortButton()
        {

        }
        
        public SortingDirection SortButtonState
        {
            get { return (SortingDirection)GetValue(SortButtonStateProperty); }
            set { SetValue(SortButtonStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortButtonState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortButtonStateProperty =
            DependencyProperty.Register("SortButtonState", typeof(SortingDirection), typeof(SortButton), new PropertyMetadata(SortingDirection.None));

        protected override void OnClick()
        {           
            base.OnClick();  
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

    }
    
}
