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

namespace Syncfusion.Windows.Shared.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void DragDropEventHandler(object sender, DragDropEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class DragDropEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public object PayLoad { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UIElement DragIcon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DropDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object DragSource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object DropTarget { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDragArrowEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Key DragKey { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public object OriginalSource { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public MouseEventArgs MouseEventArgs { get; internal set; }

     }

    /// <summary>
    /// 
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// 
        /// </summary>
        Cancel,

        /// <summary>
        /// 
        /// </summary>
        Impossible,

        /// <summary>
        /// 
        /// </summary>
        DragStarted,

        /// <summary>
        /// 
        /// </summary>
        DragInProgress,

        /// <summary>
        /// 
        /// </summary>
        DropSuccess
    }

}
