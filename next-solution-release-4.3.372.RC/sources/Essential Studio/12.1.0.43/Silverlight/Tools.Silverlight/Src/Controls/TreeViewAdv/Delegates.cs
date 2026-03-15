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
    /// Represents the method that will handle the DragMove event
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data</param>
    public delegate void DragMoveHandler(object sender, DragMoveEventArgs e);

    /// <summary>
    /// Represents the TreeViewAdvDrag EventHandler.
    /// </summary>
    public delegate void TreeViewAdvDragEventHandler(object sender, TreeViewAdvDragEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void TreeViewAdvDragStartEventHandler(object sender, TreeViewAdvDragStartEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ExpandCollapseEventHandler(object sender, ExpandCollapseEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void NodeEditEventHandler(object sender, NodeEditEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void NodeEditorEventHandler(object sender,NodeEditorEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void NodeEditorCancellableEventHandler(object sender,NodeEditorCancellableEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void NodeCancellableEditEventHandler(object sender,NodeCancellableEditEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ContextMenuEventHandler(object sender,ContextMenuEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void LoadOnDemandEventHandler(object sender, LoadonDemandEventArgs args);
}
