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
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Automation.Provider;

namespace Syncfusion.Windows.Controls.Grid.Automation.Peers
{
    public class GridTreeControlAutomationPeer : FrameworkElementAutomationPeer
    {
        public GridTreeControlAutomationPeer(GridTreeControl owner)
            : base(owner)
        {
            if (owner == null)
            {
                throw new InvalidOperationException("Owner cannot be null");
            }
        }

        public GridTreeModel Model
        {
            get
            {
                var grid = this.Owner as GridTreeControl;
                return grid.Model;
            }
        }

        public GridTreeControl OwnerGrid
        {
            get
            {
                return this.Owner as GridTreeControl;
            }
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.DataGrid;
        }

        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Grid:
                case PatternInterface.Selection:
                case PatternInterface.Table:
                    {
                        var gridControlBase = this.OwnerGrid.FindElementOfType<GridTreeControlImpl>();
                        if (gridControlBase != null)
                        {
                            var gridPeer = UIElementAutomationPeer.CreatePeerForElement(gridControlBase);
                            return gridPeer;
                        }
                        else
                        {
                            throw new InvalidOperationException("No GridControlBase found in the visual tree");
                        }
                    }
                case PatternInterface.Scroll:
                    {
                        var scrollViewer = this.OwnerGrid.FindElementOfType<ScrollViewer>();
                        if (scrollViewer != null)
                        {
                            var scrollPeer = UIElementAutomationPeer.CreatePeerForElement(scrollViewer);
                            var scrollProvider = scrollPeer as IScrollProvider;
                            if (scrollPeer != null && scrollProvider != null)
                            {
                                scrollPeer.EventsSource = this;
                                return scrollProvider;
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("No ScrollViewer found in the parent");
                        }
                    }
                    break;
            }

            return base.GetPattern(patternInterface);
        }
    }
}
