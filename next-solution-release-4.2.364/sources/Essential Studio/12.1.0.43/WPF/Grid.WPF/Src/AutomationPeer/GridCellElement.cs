#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid.Automation.Peers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    public class GridCellElement : FrameworkElement
    {
        public GridCellElement(DrawingVisual visual)
        {
            this.ContentVisual = visual;
        }

        public DrawingVisual ContentVisual
        {
            get;
            private set;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= 1)
            {
                throw new ArgumentException("index");
            }

            return this.ContentVisual;
        }

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new GridCellElementAutomationPeer(this);
        }
    }
}
