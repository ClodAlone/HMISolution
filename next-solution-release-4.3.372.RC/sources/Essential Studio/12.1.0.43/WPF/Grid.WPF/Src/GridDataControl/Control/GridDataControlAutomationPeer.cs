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
using System.Windows.Automation;
using Syncfusion.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Grid.Automation.Peers
{
    public class GridDataControlAutomationPeer : GridControlAutomationPeer
    {
        public GridDataControlAutomationPeer(GridControlBase owner)
            : base(owner)
        {
            if (owner == null)
            {
                throw new InvalidOperationException("Owner cannot be null");
            }
        }

        public GridDataTableModel TableModel
        {
            get
            {
                var model = base.Model as GridDataTableModel;
                return model;
            }
        }

        protected override GridCellElement CreateGridCellElement(System.Windows.Media.DrawingVisual dVisual)
        {
            return new GridDataCellElement(dVisual);
        }

        /*protected override AutomationControlType GetAutomationControlTypeCore()
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
                        var gridControlBase = this.OwnerGrid.FindElementOfType<GridDataControlBaseImpl>();
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
        }*/
    }

    public class GridDataCellElement : GridCellElement
    {
        public GridDataCellElement(DrawingVisual visual)
            : base(visual)
        {
        }

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new GridDataCellElementAutomationPeer(this);
        }
    }

    public class GridDataCellElementAutomationPeer : GridCellElementAutomationPeer, IExpandCollapseProvider
    {
        public GridDataCellElementAutomationPeer(GridCellElement cell)
            : base(cell)
        {
        }

        public GridDataTableModel TableModel
        {
            get
            {
                var model = this.OwnerGrid.Model as GridDataTableModel;
                if (model != null)
                {
                    return model;
                }

                return null;
            }
        }

        private object GetRecordEntry()
        {
            object record = null;
            var rowIndex = this.RowColumnIndex.RowIndex;
            var model = this.TableModel;
            if (model != null)
            {
                var actualRowIndex = model.ResolveIndexToRecordPosition(rowIndex);
                record = !model.Table.HasGroups ? (GridDataRecord)model.View.Records[actualRowIndex] : (GridDataRecord)model.View.TopLevelGroup.DisplayElements[actualRowIndex];
            }

            return record;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.ExpandCollapse:
                    return this;
            }

            return base.GetPattern(patternInterface);
        }

        #region IExpandCollapseProvider Members

        void IExpandCollapseProvider.Collapse()
        {
            var record = this.GetRecordEntry();
            if (record != null)
            {
                var recordEntry = record as GridDataRecord;
                if (recordEntry != null)
                {
                    recordEntry.IsExpanded = false;
                }
                else
                {
                    var group = record as Group;
                    if (group != null)
                    {
                        group.IsExpanded = false;
                    }
                }
            }
        }

        void IExpandCollapseProvider.Expand()
        {
            var record = this.GetRecordEntry();
            if (record != null)
            {
                var recordEntry = record as GridDataRecord;
                if (recordEntry != null)
                {
                    recordEntry.IsExpanded = true;
                }
                else
                {
                    var group = record as Group;
                    if (group != null)
                    {
                        group.IsExpanded = true;
                    }
                }
            }
        }

        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                ExpandCollapseState state = ExpandCollapseState.Collapsed;
                var record = this.GetRecordEntry();
                if (record != null)
                {
                    var recordEntry = record as GridDataRecord;
                    if (recordEntry != null)
                    {
                        state = recordEntry.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
                    }
                    else
                    {
                        var group = record as Group;
                        state = group.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
                    }
                }

                return state;
            }
        }

        #endregion
    }
}
