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

namespace Syncfusion.UI.Xaml.Diagram.Layout.Base
{
    public abstract class TreeLayoutBase :
        LayoutBase,
        IInternalLayout
    {
        public abstract object LayoutRoot { get; set; }

        public double HorizontalSpacing { get; set; }

        public double SpaceBetweenSubTrees { get; set; }

        public double VerticalSpacing { get; set; }

        IGraphInternal IInternalLayout.Graph { get; set; }
    }
}
