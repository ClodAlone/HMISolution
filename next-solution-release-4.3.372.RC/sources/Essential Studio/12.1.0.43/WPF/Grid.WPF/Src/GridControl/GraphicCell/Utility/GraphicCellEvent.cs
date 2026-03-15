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
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.Grid
{
    public delegate void GraphicQueryCellInfoEventHandler(object sender, GraphicQueryCellInfoEventArgs e);

    public sealed class GraphicQueryCellInfoEventArgs : SyncfusionHandledEventArgs
    {
        int index;
        GraphicStyleInfo style;

        public GraphicQueryCellInfoEventArgs(int index, GraphicStyleInfo style)
        {
            this.index = index;
            this.style = style;
        }

#if !SILVERLIGHT
        [TraceProperty(true)]
#endif
        public int Index
        {
            get
            {
                return index;
            }
        }

#if !SILVERLIGHT
        [TraceProperty(true)]
#endif
        public GraphicStyleInfo Style
        {
            get
            {
                return style;
            }
            set
            {
                style.ModifyStyle(value, StyleModifyType.Copy);
            }
        }
    }

    public delegate void GraphicCommitCellInfoEventHandler(object sender, GraphicCommitCellInfoEventArgs e);

    public sealed class GraphicCommitCellInfoEventArgs : SyncfusionHandledEventArgs
    {
         int index;
        GraphicStyleInfo style;
        StyleInfoProperty sip;

        public GraphicCommitCellInfoEventArgs(int index, GraphicStyleInfo style, StyleInfoProperty sip)
        {
            this.index = index;
            this.style = style;
            this.sip = sip;
        }

#if !SILVERLIGHT
        [TraceProperty(true)]
#endif
        public int Index
        {
            get
            {
                return index;
            }
        }

#if !SILVERLIGHT
        [TraceProperty(true)]
#endif
        public GraphicStyleInfo Style
        {
            get
            {
                return style;
            }
            set
            {
                style.ModifyStyle(value, StyleModifyType.Copy);
            }
        }

        public StyleInfoProperty Sip
        {
            get { return sip; }
            set { sip = value; }
        }
    }

    public delegate void GraphicCellResizingEventHandler(object sender, GraphicCellResizingEventArgs e);

    public sealed class GraphicCellResizingEventArgs : SyncfusionCancelEventArgs
    {
        GraphicCellSpanInfo spanInfo;
        string name;
        public GraphicCellResizingEventArgs(GraphicCellSpanInfo spanInfo, string name)
        {
            this.spanInfo = spanInfo;
            this.name = name;
        }

        public GraphicCellSpanInfo CellSpanInfo
        {
            get { return spanInfo; }
        }

        public string Name
        {
            get { return name; }
        }
    }

    public delegate void GraphicCellResizedEventHandler(object sender, GraphicCellResizedEventArgs e);

    public sealed class GraphicCellResizedEventArgs : SyncfusionEventArgs
    {
        GraphicCellSpanInfo spanInfo;
        string name;
        public GraphicCellResizedEventArgs(GraphicCellSpanInfo spanInfo, string name)
        {
            this.spanInfo = spanInfo;
            this.name = name;
        }

        public GraphicCellSpanInfo CellSpanInfo
        {
            get { return spanInfo; }
        }

        public string Name
        {
            get { return name; }
        }
    }

    public delegate void GraphicCellMovingEventHandler(object sender, GraphicCellMovingEventArgs e);

    public sealed class GraphicCellMovingEventArgs : SyncfusionCancelEventArgs
    {
        GraphicCellSpanInfo spanInfo;
        string name;
        public GraphicCellMovingEventArgs(GraphicCellSpanInfo spanInfo, string name)
        {
            this.spanInfo = spanInfo;
            this.name = name;
        }

        public GraphicCellSpanInfo CellSpanInfo
        {
            get { return spanInfo; }
        }

        public string Name
        {
            get { return name; }
        }
    }

    public delegate void GraphicCellMovedEventHandler(object sender, GraphicCellMovedEventArgs e);

    public sealed class GraphicCellMovedEventArgs : SyncfusionEventArgs
    {
        GraphicCellSpanInfo spanInfo;
        string name;
        public GraphicCellMovedEventArgs(GraphicCellSpanInfo spanInfo, string name)
        {
            this.spanInfo = spanInfo;
            this.name = name;
        }

        public GraphicCellSpanInfo CellSpanInfo
        {
            get { return spanInfo; }
        }

        public string Name
        {
            get { return name; }
        }
    }

    public delegate void GraphicCellRemovingEventHandler(object sender, GraphicCellRemovingEventArgs e);

    public sealed class GraphicCellRemovingEventArgs : SyncfusionCancelEventArgs
    {
        GraphicCellSpanInfoCollection<GraphicCellSpanInfo> removedGraphicCells;
        public GraphicCellRemovingEventArgs(GraphicCellSpanInfoCollection<GraphicCellSpanInfo> spanInfo)
        {
            this.removedGraphicCells = spanInfo;
        }

        public GraphicCellSpanInfoCollection<GraphicCellSpanInfo> RemovedGraphicCells
        {
            get { return removedGraphicCells; }
        }
    }

    public delegate void GraphicCellRemovedEventHandler(object sender, GraphicCellRemovedEventArgs e);

    public sealed class GraphicCellRemovedEventArgs : SyncfusionEventArgs
    {
        GraphicCellSpanInfoCollection<GraphicCellSpanInfo> removedGraphicCells;
        public GraphicCellRemovedEventArgs(GraphicCellSpanInfoCollection<GraphicCellSpanInfo> spanInfo)
        {
            this.removedGraphicCells = spanInfo;
        }

        public GraphicCellSpanInfoCollection<GraphicCellSpanInfo> RemovedGraphicCells
        {
            get { return removedGraphicCells; }
        }
    }

    public delegate void CurrrentGraphicCellActivatingEventHandler(object sender, CurrrentGraphicCellActivatingEventArgs e);

    public sealed class CurrrentGraphicCellActivatingEventArgs : SyncfusionCancelEventArgs
    {
        GraphicCellSpanInfo span;
        GraphicCellSpanInfo newSpan;
        GraphicStyleInfo style;
        public CurrrentGraphicCellActivatingEventArgs(GraphicCellSpanInfo spanInfo, GraphicStyleInfo style, GraphicCellSpanInfo newSpan)
        {
            this.span = spanInfo;
            this.style = style;
            this.newSpan = newSpan;
        }

        public GraphicCellSpanInfo CurrentSpanInfo
        {
            get { return span; }
        }

        public GraphicStyleInfo CurrentGraphicStyle
        {
            get { return style; }
        }

        public GraphicCellSpanInfo NewSapnInfo
        {
            get { return newSpan; }
        }
    }

    public delegate void CurrrentGraphicCellActivatedEventHandler(object sender, CurrrentGraphicCellActivatedEventArgs e);

    public sealed class CurrrentGraphicCellActivatedEventArgs : SyncfusionEventArgs
    {
        GraphicCellSpanInfo span;
        GraphicStyleInfo style;
        public CurrrentGraphicCellActivatedEventArgs(GraphicCellSpanInfo spanInfo, GraphicStyleInfo style)
        {
            this.span = spanInfo;
            this.style = style;
        }

        public GraphicCellSpanInfo CurrentSpanInfo
        {
            get { return span; }
        }

        public GraphicStyleInfo CurrentGraphicStyle
        {
            get { return style; }
        }
    }

    public delegate void CurrrentGraphicCellDeactivatedEventHandler(object sender, CurrrentGraphicCellDeactivatedEventArgs e);

    public sealed class CurrrentGraphicCellDeactivatedEventArgs : SyncfusionEventArgs
    {
        GraphicCellSpanInfo span;
        GraphicStyleInfo style;
        public CurrrentGraphicCellDeactivatedEventArgs(GraphicCellSpanInfo spanInfo, GraphicStyleInfo style)
        {
            this.span = spanInfo;
            this.style = style;
        }

        public GraphicCellSpanInfo CurrentSpanInfo
        {
            get { return span; }
        }

        public GraphicStyleInfo CurrentGraphicStyle
        {
            get { return style; }
        }
    }
}
