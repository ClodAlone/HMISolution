//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableControlEvents.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using System.Xml;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid.Grouping.Design;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    // CancelMouseEventArgs
    // ControlEventArgs
    // EventArgs
    // GraphicsEventArgs
    // InvalidateEventArgs
    // KeyEventArgs
    // KeyPressEventArgs
    // LayoutEventArgs
    // MouseEventArgs
    // PaintEventArgs
    // PopupClosedEventArgs
    // ScrollTipFeedbackEventArgs
    // ScrollWindowEventArgs
    // GridCellButtonClickedEventArgs
    // GridCellClickEventArgs
    // GridCellClickEventArgs
    // GridCellClickEventArgs
    // GridCellCursorEventArgs
    // GridCellHitTestEventArgs
    // GridCellMouseEventArgs
    // GridCellMouseEventArgs
    // GridCellMouseEventArgs
    // GridCellPushButtonClickEventArgs
    // GridCurrentCellActivateFailedEventArgs
    // GridCurrentCellControlKeyMessageEventArgs
    // GridCurrentCellDeactivatedEventArgs
    // GridCurrentCellInitializeControlTextEventArgs
    // GridCurrentCellErrorMessageEventArgs
    // GridCurrentCellMovedEventArgs
    // GridCurrentCellMoveFailedEventArgs
    // GridCurrentCellMovingEventArgs
    // GridCurrentCellShowingDropDownEventArgs
    // GridCurrentCellValidateStringEventArgs
    // GridDrawCellBackgroundEventArgs
    // GridDrawCellBackgroundEventArgs
    // GridDrawCellButtonBackgroundEventArgs
    // GridDrawCellButtonEventArgs
    // GridDrawCellEventArgs
    // GridDrawCellEventArgs
    // GridDrawCurrentCellBorderEventArgs
    // GridPrepareViewStyleInfoEventArgs
    // GridQueryCanOleDragRangeEventArgs
    // GridQueryNextCurrentCellPositionEventArgs
    // GridResizingColumnsEventArgs
    // GridResizingRowsEventArgs
    // GridRowColIndexChangedEventArgs
    // GridRowColIndexChangedEventArgs
    // GridRowColIndexChangingEventArgs
    // GridScrollPositionChangedEventArgs
    // GridScrollPositionChangedEventArgs
    // GridScrollPositionChangingEventArgs
    // GridSelectionDragEventArgs
    // GridSelectionDragEventArgs
    // GridWrapCellNextControlInFormEventArgs

    // eva GridTableControlMouse Mouse GridTableControl tableControl MouseEventArgs inner
    // eva GridTableControlCancel Cancel GridTableControl tableControl CancelEventArgs inner
    // eva GridTableControlCancelMouse CancelMouse GridTableControl tableControl CancelMouseEventArgs inner
    // eva GridTableControlControl Control GridTableControl tableControl ControlEventArgs inner
    // eva GridTableControl Syncfusion GridTableControl tableControl EventArgs inner
    // eva GridTableControlGraphics Graphics GridTableControl tableControl GraphicsEventArgs inner
    // eva GridTableControlInvalidate Invalidate GridTableControl tableControl InvalidateEventArgs inner
    // eva GridTableControlKey Key GridTableControl tableControl KeyEventArgs inner
    // eva GridTableControlKeyPress KeyPress GridTableControl tableControl KeyPressEventArgs inner
    // eva GridTableControlLayout Layout GridTableControl tableControl LayoutEventArgs inner
    // eva GridTableControlMouse Mouse GridTableControl tableControl MouseEventArgs inner
    // eva GridTableControlPaint Paint GridTableControl tableControl PaintEventArgs inner
    // eva GridTableControlPopupClosed PopupClosed GridTableControl tableControl PopupClosedEventArgs inner
    // eva GridTableControlScrollTipFeedback ScrollTipFeedback GridTableControl tableControl ScrollTipFeedbackEventArgs inner
    // eva GridTableControlScrollWindow ScrollWindow GridTableControl tableControl ScrollWindowEventArgs inner
    // eva GridTableControlCellButtonClicked GridCellButtonClicked GridTableControl tableControl GridCellButtonClickedEventArgs inner
    // eva GridTableControlCellClick GridCellClick GridTableControl tableControl GridCellClickEventArgs inner
    // eva GridTableControlCellCursor GridCellCursor GridTableControl tableControl GridCellCursorEventArgs inner
    // eva GridTableControlCellHitTest GridCellHitTest GridTableControl tableControl GridCellHitTestEventArgs inner
    // eva GridTableControlCellMouse GridCellMouse GridTableControl tableControl GridCellMouseEventArgs inner
    // eva GridTableControlCellPushButtonClick GridCellPushButtonClick GridTableControl tableControl GridCellPushButtonClickEventArgs inner
    // eva GridTableControlCurrentCellActivateFailed GridCurrentCellActivateFailed GridTableControl tableControl GridCurrentCellActivateFailedEventArgs inner
    // eva GridTableControlCurrentCellActivating GridCurrentCellActivating GridTableControl tableControl GridCurrentCellActivatingEventArgs inner
    // eva GridTableControlCurrentCellControlKeyMessage GridCurrentCellControlKeyMessage GridTableControl tableControl GridCurrentCellControlKeyMessageEventArgs inner
    // eva GridTableControlCurrentCellDeactivated GridCurrentCellDeactivated GridTableControl tableControl GridCurrentCellDeactivatedEventArgs inner
    // eva GridTableControlCurrentCellInitializeControlText GridCurrentCellInitializeControlText GridTableControl tableControl GridCurrentCellInitializeControlTextEventArgs inner
    // eva GridTableControlCurrentCellErrorMessage GridCurrentCellErrorMessage GridTableControl tableControl GridCurrentCellErrorMessageEventArgs inner
    // eva GridTableControlCurrentCellMoved GridCurrentCellMoved GridTableControl tableControl GridCurrentCellMovedEventArgs inner
    // eva GridTableControlCurrentCellMoveFailed GridCurrentCellMoveFailed GridTableControl tableControl GridCurrentCellMoveFailedEventArgs inner
    // eva GridTableControlCurrentCellMoving GridCurrentCellMoving GridTableControl tableControl GridCurrentCellMovingEventArgs inner
    // eva GridTableControlCurrentCellShowingDropDown GridCurrentCellShowingDropDown GridTableControl tableControl GridCurrentCellShowingDropDownEventArgs inner
    // eva GridTableControlCurrentCellValidateString GridCurrentCellValidateString GridTableControl tableControl GridCurrentCellValidateStringEventArgs inner
    // eva GridTableControlDrawCellBackground GridDrawCellBackground GridTableControl tableControl GridDrawCellBackgroundEventArgs inner
    // eva GridTableControlDrawCellButtonBackground GridDrawCellButtonBackground GridTableControl tableControl GridDrawCellButtonBackgroundEventArgs inner
    // eva GridTableControlDrawCellButton GridDrawCellButton GridTableControl tableControl GridDrawCellButtonEventArgs inner
    // eva GridTableControlDrawCell GridDrawCell GridTableControl tableControl GridDrawCellEventArgs inner
    // eva GridTableControlDrawCurrentCellBorder GridDrawCurrentCellBorder GridTableControl tableControl GridDrawCurrentCellBorderEventArgs inner
    // eva GridTableControlPrepareViewStyleInfo GridPrepareViewStyleInfo GridTableControl tableControl GridPrepareViewStyleInfoEventArgs inner
    // eva GridTableControlQueryCanOleDragRange GridQueryCanOleDragRange GridTableControl tableControl GridQueryCanOleDragRangeEventArgs inner
    // eva GridTableControlQueryNextCurrentCellPosition GridQueryNextCurrentCellPosition GridTableControl tableControl GridQueryNextCurrentCellPositionEventArgs inner
    // eva GridTableControlResizingColumns GridResizingColumns GridTableControl tableControl GridResizingColumnsEventArgs inner
    // eva GridTableControlResizingRows GridResizingRows GridTableControl tableControl GridResizingRowsEventArgs inner
    // eva GridTableControlRowColIndexChanged GridRowColIndexChanged GridTableControl tableControl GridRowColIndexChangedEventArgs inner
    // eva GridTableControlRowColIndexChanging GridRowColIndexChanging GridTableControl tableControl GridRowColIndexChangingEventArgs inner
    // eva GridTableControlScrollPositionChanged GridScrollPositionChanged GridTableControl tableControl GridScrollPositionChangedEventArgs inner
    // eva GridTableControlScrollPositionChanging GridScrollPositionChanging GridTableControl tableControl GridScrollPositionChangingEventArgs inner
    // eva GridTableControlSelectionDrag GridSelectionDrag GridTableControl tableControl GridSelectionDragEventArgs inner
    // eva GridTableControlWrapCellNextControlInForm GridWrapCellNextControlInForm GridTableControl tableControl GridWrapCellNextControlInFormEventArgs inner

    // eva GridTableControlMouse Mouse GridTableControl tableControl MouseEventArgs inner
    
    /// <summary>
    /// Represents a method that handles mouse events of a <see cref="GridTableControl"/> child of a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlMouseEventHandler(object sender, GridTableControlMouseEventArgs e);
    
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="MouseEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlMouseEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        MouseEventArgs inner;
    
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="MouseEventArgs"/>
        /// that holds the event data of the underlying original event</param>
        public GridTableControlMouseEventArgs(GridTableControl tableControl, MouseEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
    
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiated the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
    
        /// <summary>
        /// The inner <see cref="MouseEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public MouseEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
    
    // eva GridTableControlCancel Cancel GridTableControl tableControl CancelEventArgs inner
    
    /// <summary>
    /// Represents a method that handles cancelable events of a <see cref="GridTableControl"/> child of a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCancelEventHandler(object sender, GridTableControlCancelEventArgs e);
    
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="CancelEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls.  <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead a programmer can subscribe to the TableControl* events raised by <see cref="GridGroupingControl"/>. Each of these
    /// events has <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCancelEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        CancelEventArgs inner;
    
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiated the original event.</param>
        /// <param name="inner">The inner <see cref="CancelEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCancelEventArgs(GridTableControl tableControl, CancelEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
    
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiated the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
    
        /// <summary>
        /// The inner <see cref="CancelEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public CancelEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }

    // eva GridTableControlCurrentCellActivating GridCurrentCellActivating GridTableControl tableControl GridCurrentCellActivatingEventArgs inner
    
    /// <summary>
    /// Represents a method that handles <see cref="GridControlBase.CurrentCellActivating"/> of a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellActivatingEventHandler(object sender, GridTableControlCurrentCellActivatingEventArgs e);
    
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellActivatingEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiples child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead a programmer can subscribe to the TableControl events raised by <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellActivatingEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellActivatingEventArgs inner;
    
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellActivatingEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellActivatingEventArgs(GridTableControl tableControl, GridCurrentCellActivatingEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
    
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
    
        /// <summary>
        /// The inner <see cref="GridCurrentCellActivatingEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellActivatingEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
    
    // eva GridTableControlCancelMouseEventArgs CancelMouse GridTableControl tableControl CancelMouseEventArgs inner
     
    /// <summary>
    /// Represents a method that handles cancelable mouse events of a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCancelMouseEventHandler(object sender, GridTableControlCancelMouseEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="CancelMouseEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCancelMouseEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        CancelMouseEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="CancelMouseEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCancelMouseEventArgs(GridTableControl tableControl, CancelMouseEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="CancelMouseEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public CancelMouseEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlControlEventArgs Control GridTableControl tableControl ControlEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="ControlEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlControlEventHandler(object sender, GridTableControlControlEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="ControlEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlControlEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        ControlEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="ControlEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlControlEventArgs(GridTableControl tableControl, ControlEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="ControlEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public ControlEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlEventArgs Syncfusion GridTableControl tableControl EventArgs inner
     
    /// <summary>
    /// Represents a method that handles events that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlEventHandler(object sender, GridTableControlEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="EventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        EventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="EventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlEventArgs(GridTableControl tableControl, EventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="EventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public EventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlGraphicsEventArgs Graphics GridTableControl tableControl GraphicsEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GraphicsEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlGraphicsEventHandler(object sender, GridTableControlGraphicsEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GraphicsEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlGraphicsEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GraphicsEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GraphicsEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlGraphicsEventArgs(GridTableControl tableControl, GraphicsEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GraphicsEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GraphicsEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlInvalidateEventArgs Invalidate GridTableControl tableControl InvalidateEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="InvalidateEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlInvalidateEventHandler(object sender, GridTableControlInvalidateEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="InvalidateEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlInvalidateEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        InvalidateEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="InvalidateEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlInvalidateEventArgs(GridTableControl tableControl, InvalidateEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="InvalidateEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public InvalidateEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlKeyEventArgs Key GridTableControl tableControl KeyEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="KeyEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlKeyEventHandler(object sender, GridTableControlKeyEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="KeyEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlKeyEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        KeyEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="KeyEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlKeyEventArgs(GridTableControl tableControl, KeyEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="KeyEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public KeyEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlKeyPressEventArgs KeyPress GridTableControl tableControl KeyPressEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="KeyPressEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlKeyPressEventHandler(object sender, GridTableControlKeyPressEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="KeyPressEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlKeyPressEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        KeyPressEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="KeyPressEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlKeyPressEventArgs(GridTableControl tableControl, KeyPressEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="KeyPressEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public KeyPressEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlLayoutEventArgs Layout GridTableControl tableControl LayoutEventArgs inner
    // eva GridTableControlMouseEventArgs Mouse GridTableControl tableControl MouseEventArgs inner
    // eva GridTableControlPaintEventArgs Paint GridTableControl tableControl PaintEventArgs inner
    // eva GridTableControlPopupClosedEventArgs PopupClosed GridTableControl tableControl PopupClosedEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="PopupClosedEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlPopupClosedEventHandler(object sender, GridTableControlPopupClosedEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="PopupClosedEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlPopupClosedEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        PopupClosedEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="PopupClosedEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlPopupClosedEventArgs(GridTableControl tableControl, PopupClosedEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="PopupClosedEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public PopupClosedEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlScrollTipFeedbackEventArgs ScrollTipFeedback GridTableControl tableControl ScrollTipFeedbackEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="ScrollTipFeedbackEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlScrollTipFeedbackEventHandler(object sender, GridTableControlScrollTipFeedbackEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="ScrollTipFeedbackEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlScrollTipFeedbackEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        ScrollTipFeedbackEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="ScrollTipFeedbackEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlScrollTipFeedbackEventArgs(GridTableControl tableControl, ScrollTipFeedbackEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="ScrollTipFeedbackEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public ScrollTipFeedbackEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlScrollWindowEventArgs ScrollWindow GridTableControl tableControl ScrollWindowEventArgs inner
    // eva GridTableControlCellButtonClickedEventArgs GridCellButtonClicked GridTableControl tableControl GridCellButtonClickedEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCellButtonClickedEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCellButtonClickedEventHandler(object sender, GridTableControlCellButtonClickedEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCellButtonClickedEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCellButtonClickedEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCellButtonClickedEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCellButtonClickedEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCellButtonClickedEventArgs(GridTableControl tableControl, GridCellButtonClickedEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCellButtonClickedEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCellButtonClickedEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCellClickEventArgs GridCellClick GridTableControl tableControl GridCellClickEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCellClickEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCellClickEventHandler(object sender, GridTableControlCellClickEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCellClickEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCellClickEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCellClickEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCellClickEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCellClickEventArgs(GridTableControl tableControl, GridCellClickEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCellClickEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCellClickEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCellCursorEventArgs GridCellCursor GridTableControl tableControl GridCellCursorEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCellCursorEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCellCursorEventHandler(object sender, GridTableControlCellCursorEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCellCursorEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCellCursorEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCellCursorEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCellCursorEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCellCursorEventArgs(GridTableControl tableControl, GridCellCursorEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCellCursorEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCellCursorEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridQueryScrollCellInViewEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlQueryScrollCellInViewEventHandler(object sender, GridTableControlQueryScrollCellInViewEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridQueryScrollCellInViewEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlQueryScrollCellInViewEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridQueryScrollCellInViewEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridQueryScrollCellInViewEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlQueryScrollCellInViewEventArgs(GridTableControl tableControl, GridQueryScrollCellInViewEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridQueryScrollCellInViewEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridQueryScrollCellInViewEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }    
    
    // eva GridTableControlCellHitTestEventArgs GridCellHitTest GridTableControl tableControl GridCellHitTestEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCellHitTestEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCellHitTestEventHandler(object sender, GridTableControlCellHitTestEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCellHitTestEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCellHitTestEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCellHitTestEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCellHitTestEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCellHitTestEventArgs(GridTableControl tableControl, GridCellHitTestEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCellHitTestEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCellHitTestEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCellMouseEventArgs GridCellMouse GridTableControl tableControl GridCellMouseEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCellMouseEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCellMouseEventHandler(object sender, GridTableControlCellMouseEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCellMouseEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCellMouseEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCellMouseEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCellMouseEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCellMouseEventArgs(GridTableControl tableControl, GridCellMouseEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCellMouseEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCellMouseEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCellPushButtonClickEventArgs GridCellPushButtonClick GridTableControl tableControl GridCellPushButtonClickEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCellPushButtonClickEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCellPushButtonClickEventHandler(object sender, GridTableControlCellPushButtonClickEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCellPushButtonClickEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCellPushButtonClickEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCellPushButtonClickEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCellPushButtonClickEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCellPushButtonClickEventArgs(GridTableControl tableControl, GridCellPushButtonClickEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCellPushButtonClickEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCellPushButtonClickEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCurrentCellActivateFailedEventArgs GridCurrentCellActivateFailed GridTableControl tableControl GridCurrentCellActivateFailedEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellActivateFailedEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellActivateFailedEventHandler(object sender, GridTableControlCurrentCellActivateFailedEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellActivateFailedEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellActivateFailedEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellActivateFailedEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellActivateFailedEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellActivateFailedEventArgs(GridTableControl tableControl, GridCurrentCellActivateFailedEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCurrentCellActivateFailedEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellActivateFailedEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCurrentCellControlKeyMessageEventArgs GridCurrentCellControlKeyMessage GridTableControl tableControl GridCurrentCellControlKeyMessageEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellControlKeyMessageEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellControlKeyMessageEventHandler(object sender, GridTableControlCurrentCellControlKeyMessageEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellControlKeyMessageEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellControlKeyMessageEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellControlKeyMessageEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellControlKeyMessageEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellControlKeyMessageEventArgs(GridTableControl tableControl, GridCurrentCellControlKeyMessageEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCurrentCellControlKeyMessageEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellControlKeyMessageEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCurrentCellDeactivatedEventArgs GridCurrentCellDeactivated GridTableControl tableControl GridCurrentCellDeactivatedEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellDeactivatedEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellDeactivatedEventHandler(object sender, GridTableControlCurrentCellDeactivatedEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellDeactivatedEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellDeactivatedEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellDeactivatedEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellDeactivatedEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellDeactivatedEventArgs(GridTableControl tableControl, GridCurrentCellDeactivatedEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCurrentCellDeactivatedEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellDeactivatedEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCurrentCellInitializeControlTextEventArgs GridCurrentCellInitializeControlText GridTableControl tableControl GridCurrentCellInitializeControlTextEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellInitializeControlTextEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellInitializeControlTextEventHandler(object sender, GridTableControlCurrentCellInitializeControlTextEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellInitializeControlTextEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellInitializeControlTextEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellInitializeControlTextEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellInitializeControlTextEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellInitializeControlTextEventArgs(GridTableControl tableControl, GridCurrentCellInitializeControlTextEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCurrentCellInitializeControlTextEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellInitializeControlTextEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
    
    // eva GridTableControlCurrentCellErrorMessageEventArgs GridCurrentCellErrorMessage GridTableControl tableControl GridCurrentCellErrorMessageEventArgs inner

    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellErrorMessageEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellErrorMessageEventHandler(object sender, GridTableControlCurrentCellErrorMessageEventArgs e);

    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellErrorMessageEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellErrorMessageEventArgs : SyncfusionEventArgs
    {
        GridTableControl tableControl;
        GridCurrentCellErrorMessageEventArgs inner;

        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellErrorMessageEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellErrorMessageEventArgs(GridTableControl tableControl, GridCurrentCellErrorMessageEventArgs inner)
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }

        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }

        /// <summary>
        /// The inner <see cref="GridCurrentCellErrorMessageEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellErrorMessageEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCurrentCellMovedEventArgs GridCurrentCellMoved GridTableControl tableControl GridCurrentCellMovedEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellMovedEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellMovedEventHandler(object sender, GridTableControlCurrentCellMovedEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellMovedEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellMovedEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellMovedEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellMovedEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellMovedEventArgs(GridTableControl tableControl, GridCurrentCellMovedEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCurrentCellMovedEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellMovedEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCurrentCellMoveFailedEventArgs GridCurrentCellMoveFailed GridTableControl tableControl GridCurrentCellMoveFailedEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellMoveFailedEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellMoveFailedEventHandler(object sender, GridTableControlCurrentCellMoveFailedEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellMoveFailedEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellMoveFailedEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellMoveFailedEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellMoveFailedEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellMoveFailedEventArgs(GridTableControl tableControl, GridCurrentCellMoveFailedEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCurrentCellMoveFailedEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellMoveFailedEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCurrentCellMovingEventArgs GridCurrentCellMoving GridTableControl tableControl GridCurrentCellMovingEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellMovingEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellMovingEventHandler(object sender, GridTableControlCurrentCellMovingEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellMovingEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellMovingEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellMovingEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellMovingEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellMovingEventArgs(GridTableControl tableControl, GridCurrentCellMovingEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCurrentCellMovingEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellMovingEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCurrentCellShowingDropDownEventArgs GridCurrentCellShowingDropDown GridTableControl tableControl GridCurrentCellShowingDropDownEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellShowingDropDownEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellShowingDropDownEventHandler(object sender, GridTableControlCurrentCellShowingDropDownEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellShowingDropDownEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellShowingDropDownEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellShowingDropDownEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellShowingDropDownEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellShowingDropDownEventArgs(GridTableControl tableControl, GridCurrentCellShowingDropDownEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCurrentCellShowingDropDownEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellShowingDropDownEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlCurrentCellValidateStringEventArgs GridCurrentCellValidateString GridTableControl tableControl GridCurrentCellValidateStringEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridCurrentCellValidateStringEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlCurrentCellValidateStringEventHandler(object sender, GridTableControlCurrentCellValidateStringEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridCurrentCellValidateStringEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlCurrentCellValidateStringEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridCurrentCellValidateStringEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridCurrentCellValidateStringEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlCurrentCellValidateStringEventArgs(GridTableControl tableControl, GridCurrentCellValidateStringEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridCurrentCellValidateStringEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCellValidateStringEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlDrawCellBackgroundEventArgs GridDrawCellBackground GridTableControl tableControl GridDrawCellBackgroundEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridDrawCellBackgroundEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlDrawCellBackgroundEventHandler(object sender, GridTableControlDrawCellBackgroundEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridDrawCellBackgroundEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlDrawCellBackgroundEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridDrawCellBackgroundEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridDrawCellBackgroundEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlDrawCellBackgroundEventArgs(GridTableControl tableControl, GridDrawCellBackgroundEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridDrawCellBackgroundEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridDrawCellBackgroundEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlDrawCellDisplayTextEventArgs GridDrawCellDisplayText GridTableControl tableControl GridDrawCellDisplayTextEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridDrawCellDisplayTextEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlDrawCellDisplayTextEventHandler(object sender, GridTableControlDrawCellDisplayTextEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridDrawCellDisplayTextEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlDrawCellDisplayTextEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridDrawCellDisplayTextEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridDrawCellDisplayTextEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlDrawCellDisplayTextEventArgs(GridTableControl tableControl, GridDrawCellDisplayTextEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridDrawCellDisplayTextEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridDrawCellDisplayTextEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
    
    // eva GridTableControlDrawCellButtonBackgroundEventArgs GridDrawCellButtonBackground GridTableControl tableControl GridDrawCellButtonBackgroundEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridDrawCellButtonBackgroundEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlDrawCellButtonBackgroundEventHandler(object sender, GridTableControlDrawCellButtonBackgroundEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridDrawCellButtonBackgroundEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlDrawCellButtonBackgroundEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridDrawCellButtonBackgroundEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridDrawCellButtonBackgroundEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlDrawCellButtonBackgroundEventArgs(GridTableControl tableControl, GridDrawCellButtonBackgroundEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridDrawCellButtonBackgroundEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridDrawCellButtonBackgroundEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlDrawCellButtonEventArgs GridDrawCellButton GridTableControl tableControl GridDrawCellButtonEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridDrawCellButtonEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlDrawCellButtonEventHandler(object sender, GridTableControlDrawCellButtonEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridDrawCellButtonEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlDrawCellButtonEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridDrawCellButtonEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridDrawCellButtonEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlDrawCellButtonEventArgs(GridTableControl tableControl, GridDrawCellButtonEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridDrawCellButtonEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridDrawCellButtonEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlDrawCellEventArgs GridDrawCell GridTableControl tableControl GridDrawCellEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridDrawCellEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlDrawCellEventHandler(object sender, GridTableControlDrawCellEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridDrawCellEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlDrawCellEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridDrawCellEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridDrawCellEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlDrawCellEventArgs(GridTableControl tableControl, GridDrawCellEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridDrawCellEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridDrawCellEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlDrawCurrentCellBorderEventArgs GridDrawCurrentCellBorder GridTableControl tableControl GridDrawCurrentCellBorderEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridDrawCurrentCellBorderEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlDrawCurrentCellBorderEventHandler(object sender, GridTableControlDrawCurrentCellBorderEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridDrawCurrentCellBorderEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlDrawCurrentCellBorderEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridDrawCurrentCellBorderEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridDrawCurrentCellBorderEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlDrawCurrentCellBorderEventArgs(GridTableControl tableControl, GridDrawCurrentCellBorderEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridDrawCurrentCellBorderEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridDrawCurrentCellBorderEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlPrepareViewStyleInfoEventArgs GridPrepareViewStyleInfo GridTableControl tableControl GridPrepareViewStyleInfoEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridPrepareViewStyleInfoEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlPrepareViewStyleInfoEventHandler(object sender, GridTableControlPrepareViewStyleInfoEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridPrepareViewStyleInfoEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlPrepareViewStyleInfoEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridPrepareViewStyleInfoEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridPrepareViewStyleInfoEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlPrepareViewStyleInfoEventArgs(GridTableControl tableControl, GridPrepareViewStyleInfoEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridPrepareViewStyleInfoEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridPrepareViewStyleInfoEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlQueryCanOleDragRangeEventArgs GridQueryCanOleDragRange GridTableControl tableControl GridQueryCanOleDragRangeEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridQueryCanOleDragRangeEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlQueryCanOleDragRangeEventHandler(object sender, GridTableControlQueryCanOleDragRangeEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridQueryCanOleDragRangeEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlQueryCanOleDragRangeEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridQueryCanOleDragRangeEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridQueryCanOleDragRangeEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlQueryCanOleDragRangeEventArgs(GridTableControl tableControl, GridQueryCanOleDragRangeEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridQueryCanOleDragRangeEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridQueryCanOleDragRangeEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }     

    // eva GridTableControlMoveCurrentCellDirectionEventArgs GridMoveCurrentCellDirection GridTableControl tableControl GridMoveCurrentCellDirectionEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridMoveCurrentCellDirectionEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlMoveCurrentCellDirectionEventHandler(object sender, GridTableControlMoveCurrentCellDirectionEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridMoveCurrentCellDirectionEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlMoveCurrentCellDirectionEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridMoveCurrentCellDirectionEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridMoveCurrentCellDirectionEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlMoveCurrentCellDirectionEventArgs(GridTableControl tableControl, GridMoveCurrentCellDirectionEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridMoveCurrentCellDirectionEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridMoveCurrentCellDirectionEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
          
    // eva GridTableControlQueryNextCurrentCellPositionEventArgs GridQueryNextCurrentCellPosition GridTableControl tableControl GridQueryNextCurrentCellPositionEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridQueryNextCurrentCellPositionEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlQueryNextCurrentCellPositionEventHandler(object sender, GridTableControlQueryNextCurrentCellPositionEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridQueryNextCurrentCellPositionEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlQueryNextCurrentCellPositionEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridQueryNextCurrentCellPositionEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridQueryNextCurrentCellPositionEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlQueryNextCurrentCellPositionEventArgs(GridTableControl tableControl, GridQueryNextCurrentCellPositionEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridQueryNextCurrentCellPositionEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridQueryNextCurrentCellPositionEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlResizingColumnsEventArgs GridResizingColumns GridTableControl tableControl GridResizingColumnsEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridResizingColumnsEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlResizingColumnsEventHandler(object sender, GridTableControlResizingColumnsEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridResizingColumnsEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlResizingColumnsEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridResizingColumnsEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridResizingColumnsEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlResizingColumnsEventArgs(GridTableControl tableControl, GridResizingColumnsEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridResizingColumnsEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridResizingColumnsEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlResizingRowsEventArgs GridResizingRows GridTableControl tableControl GridResizingRowsEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridResizingRowsEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlResizingRowsEventHandler(object sender, GridTableControlResizingRowsEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridResizingRowsEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlResizingRowsEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridResizingRowsEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridResizingRowsEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlResizingRowsEventArgs(GridTableControl tableControl, GridResizingRowsEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridResizingRowsEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridResizingRowsEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlRowColIndexChangedEventArgs GridRowColIndexChanged GridTableControl tableControl GridRowColIndexChangedEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridRowColIndexChangedEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlRowColIndexChangedEventHandler(object sender, GridTableControlRowColIndexChangedEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridRowColIndexChangedEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlRowColIndexChangedEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridRowColIndexChangedEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridRowColIndexChangedEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlRowColIndexChangedEventArgs(GridTableControl tableControl, GridRowColIndexChangedEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridRowColIndexChangedEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridRowColIndexChangedEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlRowColIndexChangingEventArgs GridRowColIndexChanging GridTableControl tableControl GridRowColIndexChangingEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridRowColIndexChangingEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlRowColIndexChangingEventHandler(object sender, GridTableControlRowColIndexChangingEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridRowColIndexChangingEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlRowColIndexChangingEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridRowColIndexChangingEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridRowColIndexChangingEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlRowColIndexChangingEventArgs(GridTableControl tableControl, GridRowColIndexChangingEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridRowColIndexChangingEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridRowColIndexChangingEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlScrollPositionChangedEventArgs GridScrollPositionChanged GridTableControl tableControl GridScrollPositionChangedEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridScrollPositionChangedEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlScrollPositionChangedEventHandler(object sender, GridTableControlScrollPositionChangedEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridScrollPositionChangedEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlScrollPositionChangedEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridScrollPositionChangedEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridScrollPositionChangedEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlScrollPositionChangedEventArgs(GridTableControl tableControl, GridScrollPositionChangedEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridScrollPositionChangedEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridScrollPositionChangedEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlScrollPositionChangingEventArgs GridScrollPositionChanging GridTableControl tableControl GridScrollPositionChangingEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridScrollPositionChangingEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlScrollPositionChangingEventHandler(object sender, GridTableControlScrollPositionChangingEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridScrollPositionChangingEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlScrollPositionChangingEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridScrollPositionChangingEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridScrollPositionChangingEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlScrollPositionChangingEventArgs(GridTableControl tableControl, GridScrollPositionChangingEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridScrollPositionChangingEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridScrollPositionChangingEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlSelectionDragEventArgs GridSelectionDrag GridTableControl tableControl GridSelectionDragEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridSelectionDragEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlSelectionDragEventHandler(object sender, GridTableControlSelectionDragEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridSelectionDragEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlSelectionDragEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridSelectionDragEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridSelectionDragEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlSelectionDragEventArgs(GridTableControl tableControl, GridSelectionDragEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridSelectionDragEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridSelectionDragEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }
     
    // eva GridTableControlWrapCellNextControlInFormEventArgs GridWrapCellNextControlInForm GridTableControl tableControl GridWrapCellNextControlInFormEventArgs inner
     
    /// <summary>
    /// Represents a method that handles events with <see cref="GridWrapCellNextControlInFormEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridTableControlWrapCellNextControlInFormEventHandler(object sender, GridTableControlWrapCellNextControlInFormEventArgs e);
     
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the original event and the inner <see cref="GridWrapCellNextControlInFormEventArgs"/>
    /// that holds the event data of the underlying original event.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridGroupingControl"/> can have one or multiple child table controls. Each of these controls raises various events.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> makes it easy for the programmer to subscribe to events raised by each of these child
    /// table controls. The <see cref="GridGroupingControl"/> internally listens to these events and raises a new event
    /// that wraps the inner arguments in an EventArgs class together with a reference to the original <see cref="GridTableControl"/>
    /// that raised the event. <para/>
    /// This removes the burden from the programmer to subscribe and unsubscribe to events of child table controls. <para/>
    /// Instead, a programmer can subscribe to the TableControl events raised by the <see cref="GridGroupingControl"/>. Each of these
    /// events has a <see cref="TableControl"/> property with reference to the <see cref="GridTableControl"/> and an <see cref="Inner"/>
    /// property that holds a reference to the original event arguments.
    /// </remarks>
    public sealed class GridTableControlWrapCellNextControlInFormEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridWrapCellNextControlInFormEventArgs inner;
     
        /// <summary>
        /// Initializes the event argument's object.
        /// </summary>
        /// <param name="tableControl">A <see cref="GridTableControl"/> that initiates the original event.</param>
        /// <param name="inner">The inner <see cref="GridWrapCellNextControlInFormEventArgs"/>
        /// that holds the event data of the underlying original event.</param>
        public GridTableControlWrapCellNextControlInFormEventArgs(GridTableControl tableControl, GridWrapCellNextControlInFormEventArgs inner) 
        {
            this.tableControl = tableControl;
            this.inner = inner;
        }
     
        /// <summary>
        /// A <see cref="GridTableControl"/> that initiates the original event.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
     
        /// <summary>
        /// The inner <see cref="GridWrapCellNextControlInFormEventArgs"/> that holds the event data of the underlying original event.
        /// </summary>
        [TraceProperty(true)]
        public GridWrapCellNextControlInFormEventArgs Inner
        {
            get
            {
                return inner;
            }
        }
    }

    // eva GridQueryAllowDragColumn Syncfusion GridTableControl tableControl GridColumnDescriptor column
    
    /// <summary>
    /// Represents a method that handles events with <see cref="GridQueryAllowDragColumnEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridQueryAllowDragColumnEventHandler(object sender, GridQueryAllowDragColumnEventArgs e);
     
    /// <summary>
    /// Reason why QueryAllowDragColumn event was raised (Show Red Indicator, MouseUp or HitTest).
    /// </summary>
    public enum GridQueryAllowDragColumnReason 
    {
        /// <summary>
        /// HitTest is occuring
        /// </summary>
        HitTest,

        /// <summary>
        /// RedArrowIndicator is displayed
        /// </summary>
        ShowRedArrowIndicator,

        /// <summary>
        /// Represent the MouseUp
        /// </summary>
        MouseUp,

        /// <summary>
        ///  Remove GroupedColumn
        /// </summary>
        Remove
    }

    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the event and the column 
    /// that is affected.
    /// </summary>
    /// <remarks>
    /// Set <see cref="AllowDrag"/> to False if you do not want to allow the user 
    /// to drag the specified <see cref="Column"/>.
    /// </remarks>
    public sealed class GridQueryAllowDragColumnEventArgs : SyncfusionHandledEventArgs 
    {
        GridTableControl tableControl;
        string column;
        string stackedColumn;
        bool allowDrag = true;
    
        private string insertBeforeColumn;
        private GridQueryAllowDragColumnReason reason;
        private MouseEventArgs mouseEventArgs;
            
        /// <overload>
        /// Initializes the event args.
        /// </overload>
        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="tableControl">The table control.</param>
        /// <param name="column">Column Name.</param>
        /// <param name="reason">The reason why this event is raised.</param>
        /// <param name="mouseEventArgs">A <see cref="MouseEventArgs"/> holding event data.</param>
        public GridQueryAllowDragColumnEventArgs(GridTableControl tableControl, GridColumnDescriptor column, GridQueryAllowDragColumnReason reason, MouseEventArgs mouseEventArgs)
            :this(tableControl,column,null,reason,mouseEventArgs)
        {
        }

        /// <overload>
        /// Initializes the event args.
        /// </overload>
        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="tableControl">The table control.</param>
        /// <param name="column">Column Name.</param>
        /// <param name="stackHeader">The stackedheaderdescriptor.</param>
        /// <param name="reason">The reason why this event is raised.</param>
        /// <param name="mouseEventArgs">A <see cref="MouseEventArgs"/> holding event data.</param>
        public GridQueryAllowDragColumnEventArgs(GridTableControl tableControl, GridColumnDescriptor column, GridStackedHeaderDescriptor stackHeader, GridQueryAllowDragColumnReason reason, MouseEventArgs mouseEventArgs) 
        {
            this.tableControl = tableControl;
            if (column != null)
            {
                this.column = column.Name;
            }
            if(stackHeader != null)
            {
                this.stackedColumn = stackHeader.Name;
            }
            this.reason = reason;
            this.mouseEventArgs = mouseEventArgs;
        }

        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="tableControl">The table control.</param>
        /// <param name="column">Column Name.</param>
        /// <param name="insertBeforeColumn">Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.</param>
        /// <param name="reason">Reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).</param>
        public GridQueryAllowDragColumnEventArgs(GridTableControl tableControl, string column, string insertBeforeColumn, GridQueryAllowDragColumnReason reason)
            :this(tableControl,column,null,insertBeforeColumn,reason)
        {
        }

        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="tableControl">The table control.</param>
        /// <param name="column">Column Name.</param>
        /// <param name="stackHeader">StackedHeader Name.</param>
        /// <param name="insertBeforeColumn">Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.</param>
        /// <param name="reason">Reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).</param>
        public GridQueryAllowDragColumnEventArgs(GridTableControl tableControl, string column, string stackHeader, string insertBeforeColumn, GridQueryAllowDragColumnReason reason) 
        {
            this.tableControl = tableControl;
            this.column = column;
            this.stackedColumn = stackHeader;
            this.insertBeforeColumn = insertBeforeColumn;
            this.reason = reason;
        }

        /// <summary>
        /// The table control.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }

        /// <summary>
        /// The MouseEventArgs for HitTest. Will be null if <see cref="Reason"/> is not HitTest.
        /// </summary>
        [TraceProperty(false)]
        public MouseEventArgs MouseEventArgs
        {
            get
            {
                return mouseEventArgs;
            }
        }
    
        /// <summary>
        /// Column Name. You can call TableDescriptor.Columns[Column] to get the GridColumnDescriptor.
        /// </summary>
        [TraceProperty(true)]
        public string Column
        {
            get
            {
                return column;
            }
        }

        /// <summary>
        /// Stacked Header Name.
        /// </summary>
        [TraceProperty(true)]
        public string StackedHeader
        {
            get
            {
                return stackedColumn;
            }
        }

        /// <summary>
        /// Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.
        /// </summary>
        [TraceProperty(true)]
        public string InsertBeforeColumn
        {
            get
            {
                return this.insertBeforeColumn;
            }
        }
       
        /// <summary>
        /// Reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).
        /// </summary>
        [TraceProperty(true)]
        public GridQueryAllowDragColumnReason Reason
        {
            get
            {
                return this.reason;
            }

            set
            {
                this.reason = value;
            }
        }

        /// <summary>
        /// Set <see cref="AllowDrag"/> to False if you do not want to allow the user 
        /// to drag the specified <see cref="Column"/>.
        /// </summary>
        [TraceProperty(true)]
        public bool AllowDrag
        {
            get
            {
                return allowDrag;
            }

            set
            {
                allowDrag = value;
            }
        }
    }

    // eva GridQueryAllowGroupByColumn Syncfusion GridTableControl tableControl GridColumnDescriptor column bool allowGroupByColumn
    
    /// <summary>
    /// Represents a method that handles events with <see cref="GridQueryAllowGroupByColumnEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridQueryAllowGroupByColumnEventHandler(object sender, GridQueryAllowGroupByColumnEventArgs e);
        
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the event and the column 
    /// that is affected.
    /// </summary>
    /// <remarks>
    /// Set <see cref="AllowGroupByColumn"/> to False if you do not want to allow the user 
    /// to group by the specified <see cref="Column"/>.
    /// </remarks>
    public sealed class GridQueryAllowGroupByColumnEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        string column;
        bool allowGroupBy = true;
    
        private GridQueryAllowDragColumnReason reason;
        private string insertBeforeColumn;

        /// <overload>
        /// Initializes the event args.
        /// </overload>
        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="tableControl">The table control.</param>
        /// <param name="column">Column Name.</param>
        public GridQueryAllowGroupByColumnEventArgs(GridTableControl tableControl, GridColumnDescriptor column) 
        {
            this.tableControl = tableControl;
            if (column != null)
            {
                this.column = column.Name;
            }

            this.reason = GridQueryAllowDragColumnReason.HitTest;
        }

        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="tableControl">The table control.</param>
        /// <param name="column">Column Name.</param>
        /// <param name="insertBeforeColumn">Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.</param>
        /// <param name="reason">Reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).</param>
        /// <param name="allowGroupBy"> Set <see cref="AllowGroupByColumn"/> to False if you do not want to allow the user 
        /// to group by the specified <see cref="Column"/>.</param>
        public GridQueryAllowGroupByColumnEventArgs(GridTableControl tableControl, string column, string insertBeforeColumn, GridQueryAllowDragColumnReason reason, bool allowGroupBy) 
        {
            this.tableControl = tableControl;
            this.column = column;
            this.insertBeforeColumn = insertBeforeColumn;
            this.reason = reason;
            this.allowGroupBy = allowGroupBy;
        }

        /// <summary>
        /// The table control.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
    
        /// <summary>
        /// Column Name. You can call TableDescriptor.Columns[Column] to get the GridColumnDescriptor.
        /// </summary>
        [TraceProperty(true)]
        public string Column
        {
            get
            {
                return column;
            }
        }

        /// <summary>
        /// Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.
        /// </summary>
        [TraceProperty(true)]
        public string InsertBeforeColumn
        {
            get
            {
                return this.insertBeforeColumn;
            }
        }
       
        /// <summary>
        /// Reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).
        /// </summary>
        [TraceProperty(true)]
        public GridQueryAllowDragColumnReason Reason
        {
            get
            {
                return this.reason;
            }

            set
            {
                this.reason = value;
            }
        }
        
        /// <summary>
        /// Set <see cref="AllowGroupByColumn"/> to False if you do not want to allow the user 
        /// to group by the specified <see cref="Column"/>.
        /// </summary>
        [TraceProperty(true)]
        public bool AllowGroupByColumn
        {
            get
            {
                return allowGroupBy;
            }

            set
            {
                allowGroupBy = value;
            }
        }
    }
    
    // eva GridQueryAllowSortColumn Syncfusion GridTableControl tableControl GridColumnDescriptor column
    
    /// <summary>
    /// Represents a method that handles events with <see cref="GridQueryAllowSortColumnEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridQueryAllowSortColumnEventHandler(object sender, GridQueryAllowSortColumnEventArgs e);
    
    /// <summary>
    /// Holds a reference to a <see cref="GridTableControl"/> that initiates the event and the column 
    /// that is affected.
    /// </summary>
    /// <remarks>
    /// Set <see cref="AllowSort"/> to False if you do not want to allow the user 
    /// to by clicking on the specified <see cref="Column"/>.
    /// </remarks>
    public sealed class GridQueryAllowSortColumnEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        GridColumnDescriptor column;
        bool allowSort = true;
        GridCellClickEventArgs cellClickEventArgs = null;
    
        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="tableControl">The table control.</param>
        /// <param name="column">Column Descriptor.</param>
        /// <param name="cellClickEventArgs">A <see cref="GridCellClickEventArgs"/>.</param>
        public GridQueryAllowSortColumnEventArgs(GridTableControl tableControl, GridColumnDescriptor column, GridCellClickEventArgs cellClickEventArgs) 
        {
            this.tableControl = tableControl;
            this.column = column;
            this.cellClickEventArgs = cellClickEventArgs;
        }
    
        /// <summary>
        /// The table control.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }
        }
    
        /// <summary>
        /// Column Descriptor.
        /// </summary>
        [TraceProperty(true)]
        public GridColumnDescriptor Column
        {
            get
            {
                return column;
            }
        }

        /// <summary>
        /// Set <see cref="AllowSort"/> to False if you do not want to allow the user 
        /// to sort by clicking on the specified <see cref="Column"/>.
        /// </summary>
        public bool AllowSort
        {
            get
            {
                return allowSort;
            }

            set
            {
                allowSort = value;
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="GridCellClickEventArgs"/> that triggered the QueryAllowSortColumn event. You can
        /// check the CellClickEventArgs to find out which mouse button was clicked or the exact position of the mouse pointer.
        /// </summary>
        public GridCellClickEventArgs CellClickEventArgs
        {
            get
            {
                return cellClickEventArgs;
            }
        }
    }
    
    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class GridTableClickCellsEventArgs : SyncfusionCancelEventArgs 
    {
        GridControlBase gridWindow;
        GridCellRendererBase cellRenderer;
        MouseEventArgs e;
        int rowIndex = -1;
        int colIndex = -1;
        
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridTableClickCellsEventArgs(GridControlBase gridWindow, GridCellRendererBase cellRenderer, MouseEventArgs e) 
        {
            this.gridWindow = gridWindow;
            this.cellRenderer = cellRenderer;
            this.e = e;

            if (e != null)
            {
                Point point = new Point(e.X, e.Y);
                int clientCol = cellRenderer.Grid.ViewLayout.PointToClientCol(point, false, GridCellSizeKind.VisibleSize);
                int clientRow = gridWindow.ViewLayout.PointToClientRow(point, false, GridCellSizeKind.VisibleSize);
                rowIndex = gridWindow.GetRow(clientRow);
                colIndex = cellRenderer.Grid.GetCol(clientCol);
            }
        }
        
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [TraceProperty(true)]
        public GridControlBase InnerGrid
        {
            get
            {
                return gridWindow;
            }

            set
            {
                gridWindow = value;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }        
        
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [TraceProperty(true)]
        public GridCellRendererBase CellRenderer
        {
            get
            {
                return cellRenderer;
            }

            set
            {
                cellRenderer = value;
            }
        }
        
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [TraceProperty(true)]
        public MouseEventArgs MouseEventArgs
        {
            get
            {
                return e;
            }

            set
            {
                e = value;
            }
        }
    }       

    // eva GridQueryAllowArrowKeyNavigateTo Syncfusion GridTableControl tableControl Element element
    
    /// <summary>
    /// Represents a method that handles events with <see cref="GridQueryAllowArrowKeyNavigateToEventArgs"/> that are raised by a <see cref="GridTableControl"/> child in a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridQueryAllowArrowKeyNavigateToEventHandler(object sender, GridQueryAllowArrowKeyNavigateToEventArgs e);
    
    /// <summary>
    /// Provides event data for the QueryAllowArrowKeyNavigateTo event. You can set AllowNavigateTo if you
    /// want arrow keys to skip over specific display elements (e.g. skip caption rows).
    /// </summary>
    public sealed class GridQueryAllowArrowKeyNavigateToEventArgs : SyncfusionEventArgs 
    {
        GridTableControl tableControl;
        Element element;
        bool allowNavigateTo;
    
        /// <summary>
        /// Constructor for GridQueryAllowArrowKeyNavigateToEventArgs
        /// </summary>
        /// <param name="tableControl">The table control</param>
        /// <param name="element">The display element (RecordRow or CaptionRow)</param>
        /// <param name="allowNavigateTo">Set <see cref="AllowNavigateTo"/> to False if you
        /// want arrow keys to skip over specific display elements (e.g. skip caption rows).</param>
        public GridQueryAllowArrowKeyNavigateToEventArgs(GridTableControl tableControl, Element element, bool allowNavigateTo) 
        {
            this.tableControl = tableControl;
            this.element = element;
            this.allowNavigateTo = allowNavigateTo;
        }
    
        /// <summary>
        /// The table control.
        /// </summary>
        [TraceProperty(true)]
        public GridTableControl TableControl
        {
            get
            {
                return tableControl;
            }

            set
            {
                tableControl = value;
            }
        }
    
        /// <summary>
        /// The display element (RecordRow or CaptionRow)
        /// </summary>
        [TraceProperty(true)]
        public Element Element
        {
            get
            {
                return element;
            }

            set
            {
                element = value;
            }
        }

        /// <summary>
        /// Set <see cref="AllowNavigateTo"/> to False if you
        /// want arrow keys to skip over specific display elements (e.g. skip caption rows)..
        /// </summary>
        public bool AllowNavigateTo
        {
            get
            {
                return allowNavigateTo;
            }

            set
            {
                allowNavigateTo = value;
            }
        }
    }
}
