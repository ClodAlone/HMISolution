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

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Delegate for creating drag events.
    /// </summary>
    /// <param name="sender">The dragging sender.</param>
    /// <param name="args">Drag event args.</param>
    public delegate void DragEventHanlder(object sender, DragEventArgs args);

    /// <summary>
    /// Class to represent dragging event arguments.
    /// </summary>
    public class DragEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DragEventArgs"/> class.
        /// </summary>
        public DragEventArgs()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DragEventArgs"/> class.
        /// </summary>
        /// <param name="horizontalChange">The horizontal change.</param>
        /// <param name="verticalChange">The vertical change.</param>
        /// <param name="mouseEventArgs">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="eventName">Name of the event.</param>
        public DragEventArgs(double horizontalChange, double verticalChange, MouseEventArgs mouseEventArgs, string eventName)
        {
            this.HorizontalChange = horizontalChange;
            this.VerticalChange = verticalChange;
            this.MouseEventArgs = mouseEventArgs;
            this.Event = eventName;
        }

        /// <summary>
        /// Gets or sets the horizontal change of the drag.
        /// </summary>
        /// <value>The horizontal change.</value>
        public double HorizontalChange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the horizontal change of the drag.
        /// </summary>
        /// <value>The event.</value>
        public string Event
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vertical change of the drag.
        /// </summary>
        /// <value>The vertical change.</value>
        public double VerticalChange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse event args.
        /// </summary>
        /// <value>The mouse event args.</value>
        public MouseEventArgs MouseEventArgs
        {
            get;
            set;
        }
    }
}
