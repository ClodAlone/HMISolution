#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Delegate for creating drag events
    /// </summary>
    /// <param name="sender">The dragging sender.</param>
    /// <param name="args">Drag event args.</param>
    public delegate void TileViewDragEventHandler(object sender, TileViewDragEventArgs args);

#if WPF
    /// <summary>
    /// Delegate for creating order change events
    /// </summary>
    /// <param name="sender">The order change sender.</param>
    /// <param name="args">order change event args.</param>
    public delegate void TileViewOrderChangeEventHandler(object sender, TileViewEventArgs args);

    /// <summary>
    /// Delegate for creating cancel Repositioning events
    /// </summary>
    /// <param name="sender">The cancel Repositioning sender.</param>
    /// <param name="args">cancel Repositioning event args.</param>
    public delegate void TileViewCancelRepositioningEventHandler(object sender, TileViewCancelEventArgs args);
#endif

    /// <summary>
    /// Class to represent dragging event arguments
    /// </summary>
    public class TileViewDragEventArgs : EventArgs
    {
        /// <summary>
        /// Blank Constuctor
        /// </summary>
        internal TileViewDragEventArgs()
        {

        }

        /// <summary>
        /// Contstructor with parameters
        /// </summary>
        /// <param name="horizontalChange">Horizontal change</param>
        /// <param name="verticalChange">Vertical change</param>
        /// <param name="mouseEventArgs">The mouse event args</param>
        /// <param name="eventName">The Event Name</param>
        public TileViewDragEventArgs(double horizontalChange, double verticalChange, MouseEventArgs mouseEventArgs, string eventName)
        {
            this.HorizontalChange = horizontalChange;
            this.VerticalChange = verticalChange;
            this.MouseEventArgs = mouseEventArgs;
            this.Event = eventName;
        }

        /// <summary>
        /// Gets or sets the horizontal change of the drag
        /// </summary>
        public double HorizontalChange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the event
        /// </summary>
        public string Event
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vertical change of the drag
        /// </summary>
        public double VerticalChange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse event args
        /// </summary>
        public MouseEventArgs MouseEventArgs
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the Tile View Cancel Event Args.
    /// </summary>
    public class TileViewCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Blank Constuctor
        /// </summary>
        public TileViewCancelEventArgs()
        {

        }

        /// <summary>
        /// Gets or sets the Source
        /// </summary>
        public object Source
        {
            get;
            set;
        }
#if WPF
        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        /// <value></value>
        /// <returns>true if the event should be canceled; otherwise, false.
        /// </returns>
        public new bool Cancel
        {
            get;
            set;
        }
#endif
    }

    /// <summary>
    /// Represents the Tile View Event Args.
    /// </summary>
    public class TileViewEventArgs : EventArgs
    {
        /// <summary>
        /// Blank Constuctor
        /// </summary>
        public TileViewEventArgs()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TileViewEventArgs"/> class.
        /// </summary>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        public TileViewEventArgs(TileViewItemState oldState, TileViewItemState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        /// <summary>
        /// Gets or sets the old state.
        /// </summary>
        /// <value>The old state.</value>
        public TileViewItemState OldState 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the new state.
        /// </summary>
        /// <value>The new state.</value>
        public TileViewItemState NewState 
        { 
            get; 
            set; 
        }
        
        /// <summary>
        /// Gets or sets Source
        /// </summary>
        public object Source
        {
            get;
            set;
        }
    }
}
