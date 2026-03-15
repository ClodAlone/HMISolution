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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools.MVVM
{

    #region TreeViewAdvExpandingCommand
    // TreeViewAdvExpandingCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvExpandingCommand : ControlCommandBase<TreeViewAdvExpandingCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvExpandingCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, ExpandCollapseEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Expanding += OnEventRaised;
        }
    }

    // TreeViewAdvExpandingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvExpandingCommandBehavior<T> : TreeViewAdvExpandingCommandBehavior
    { }
    #endregion

    #region TreeViewAdvCollapsingCommand
    // TreeViewAdvCollapsingCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvCollapsingCommand : ControlCommandBase<TreeViewAdvCollapsingCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvCollapsingCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, ExpandCollapseEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Collapsing += OnEventRaised;
        }
    }

    // TreeViewAdvCollapsingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvCollapsingCommandBehavior<T> : TreeViewAdvCollapsingCommandBehavior
    { }
    #endregion

    #region TreeViewAdvNodeEditingCommand
    // TreeViewAdvNodeEditingCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditingCommand : ControlCommandBase<TreeViewAdvNodeEditingCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditingCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, NodeCancellableEditEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NodeEditing += OnEventRaised;
        }
    }

    // TreeViewAdvNodeEditingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditingCommandBehavior<T> : TreeViewAdvNodeEditingCommandBehavior
    { }
    #endregion

    #region TreeViewAdvNodeEditedCommand
    // TreeViewAdvNodeEditedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditedCommand : ControlCommandBase<TreeViewAdvNodeEditedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, NodeEditEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NodeEdited += OnEventRaised;
        }
    }

    // TreeViewAdvNodeEditedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditedCommandBehavior<T> : TreeViewAdvNodeEditedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvNodeEditCancelledCommand
    // TreeViewAdvNodeEditCancelledCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditCancelledCommand : ControlCommandBase<TreeViewAdvNodeEditCancelledCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditCancelledCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, NodeEditEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NodeEditCancelled += OnEventRaised;
        }
    }

    // TreeViewAdvNodeEditCancelledCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditCancelledCommandBehavior<T> : TreeViewAdvNodeEditCancelledCommandBehavior
    { }
    #endregion

    #region TreeViewAdvNodeEditorValidateStringCommand
    // TreeViewAdvNodeEditorValidateStringCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditorValidateStringCommand : ControlCommandBase<TreeViewAdvNodeEditorValidateStringCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditorValidateStringCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, NodeEditorCancellableEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NodeEditorValidateString += OnEventRaised;
        }
    }

    // TreeViewAdvNodeEditorValidateStringCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditorValidateStringCommandBehavior<T> : TreeViewAdvNodeEditorValidateStringCommandBehavior
    { }
    #endregion

    #region TreeViewAdvNodeEditorValidatingCommand
    // TreeViewAdvNodeEditorValidatingCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditorValidatingCommand : ControlCommandBase<TreeViewAdvNodeEditorValidatingCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditorValidatingCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, NodeEditorCancellableEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NodeEditorValidating += OnEventRaised;
        }
    }

    // TreeViewAdvNodeEditorValidatingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditorValidatingCommandBehavior<T> : TreeViewAdvNodeEditorValidatingCommandBehavior
    { }
    #endregion

    #region TreeViewAdvNodeEditorValidatedCommand
    // TreeViewAdvNodeEditorValidatedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditorValidatedCommand : ControlCommandBase<TreeViewAdvNodeEditorValidatedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditorValidatedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, NodeEditorEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NodeEditorValidated += OnEventRaised;
        }
    }

    // TreeViewAdvNodeEditorValidatedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvNodeEditorValidatedCommandBehavior<T> : TreeViewAdvNodeEditorValidatedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvLoadOnDemandCommand
    // TreeViewAdvLoadOnDemandCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvLoadOnDemandCommand : ControlCommandBase<TreeViewAdvLoadOnDemandCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvLoadOnDemandCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, LoadonDemandEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.LoadOnDemand += OnEventRaised;
        }
    }

    // TreeViewAdvLoadOnDemandCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvLoadOnDemandCommandBehavior<T> : TreeViewAdvLoadOnDemandCommandBehavior
    { }
    #endregion

    #region TreeViewAdvDragStartedCommand
    // TreeViewAdvDragStartedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragStartedCommand : ControlCommandBase<TreeViewAdvDragStartedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragStartedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, TreeViewAdvDragStartEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragStarted += OnEventRaised;
        }
    }

    // TreeViewAdvDragStartedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvDragStartedCommandBehavior<T> : TreeViewAdvDragStartedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvDragQueryCommand
    // TreeViewAdvDragQueryCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragQueryCommand : ControlCommandBase<TreeViewAdvDragQueryCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragQueryCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, TreeViewAdvDragEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragQuery += OnEventRaised;
        }
    }

    // TreeViewAdvDragQueryCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvDragQueryCommandBehavior<T> : TreeViewAdvDragQueryCommandBehavior
    { }
    #endregion

    #region TreeViewAdvDragDropCommand
    // TreeViewAdvDragDropCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragDropCommand : ControlCommandBase<TreeViewAdvDragDropCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragDropCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, TreeViewAdvDragEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragDrop += OnEventRaised;
        }
    }

    // TreeViewAdvDragDropCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvDragDropCommandBehavior<T> : TreeViewAdvDragDropCommandBehavior
    { }
    #endregion

    #region TreeViewAdvSelectOnExpandChangeChangedCommand
    // TreeViewAdvSelectOnExpandChangeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectOnExpandChangeChangedCommand : ControlCommandBase<TreeViewAdvSelectOnExpandChangeChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectOnExpandChangeChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectOnExpandChangeChanged += OnEventRaised;
        }
    }

    // TreeViewAdvSelectOnExpandChangeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectOnExpandChangeChangedCommandBehavior<T> : TreeViewAdvSelectOnExpandChangeChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvSelectedItemChangedCommand
    // TreeViewAdvSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedItemChangedCommand : ControlCommandBase<TreeViewAdvSelectedItemChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedItemChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedItemChanged += OnEventRaised;
        }
    }

    // TreeViewAdvSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedItemChangedCommandBehavior<T> : TreeViewAdvSelectedItemChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvSelectedNodeChangedCommand
    // TreeViewAdvSelectedNodeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedNodeChangedCommand : ControlCommandBase<TreeViewAdvSelectedNodeChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedNodeChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedNodeChanged += OnEventRaised;
        }
    }

    // TreeViewAdvSelectedNodeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedNodeChangedCommandBehavior<T> : TreeViewAdvSelectedNodeChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvIsInEditModeChangedCommand
    // TreeViewAdvIsInEditModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvIsInEditModeChangedCommand : ControlCommandBase<TreeViewAdvIsInEditModeChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvIsInEditModeChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.IsInEditModeChanged += OnEventRaised;
        }
    }

    // TreeViewAdvIsInEditModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvIsInEditModeChangedCommandBehavior<T> : TreeViewAdvIsInEditModeChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvSortModeChangedCommand
    // TreeViewAdvSortModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSortModeChangedCommand : ControlCommandBase<TreeViewAdvSortModeChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSortModeChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SortModeChanged += OnEventRaised;
        }
    }

    // TreeViewAdvSortModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvSortModeChangedCommandBehavior<T> : TreeViewAdvSortModeChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvDraggingEnabledChangedCommand
    // TreeViewAdvDraggingEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDraggingEnabledChangedCommand : ControlCommandBase<TreeViewAdvDraggingEnabledChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDraggingEnabledChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DraggingEnabledChanged += OnEventRaised;
        }
    }

    // TreeViewAdvDraggingEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvDraggingEnabledChangedCommandBehavior<T> : TreeViewAdvDraggingEnabledChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvIsMultiSelectChangedCommand
    // TreeViewAdvIsMultiSelectChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvIsMultiSelectChangedCommand : ControlCommandBase<TreeViewAdvIsMultiSelectChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvIsMultiSelectChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.IsMultiSelectChanged += OnEventRaised;
        }
    }

    // TreeViewAdvIsMultiSelectChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvIsMultiSelectChangedCommandBehavior<T> : TreeViewAdvIsMultiSelectChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvSelectedNodesChangedCommand
    // TreeViewAdvSelectedNodesChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedNodesChangedCommand : ControlCommandBase<TreeViewAdvSelectedNodesChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedNodesChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedNodesChanged += OnEventRaised;
        }
    }

    // TreeViewAdvSelectedNodesChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedNodesChangedCommandBehavior<T> : TreeViewAdvSelectedNodesChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvSelectedItemsChangedCommand
    // TreeViewAdvSelectedItemsChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedItemsChangedCommand : ControlCommandBase<TreeViewAdvSelectedItemsChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedItemsChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedItemsChanged += OnEventRaised;
        }
    }

    // TreeViewAdvSelectedItemsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvSelectedItemsChangedCommandBehavior<T> : TreeViewAdvSelectedItemsChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvLineStrokeArrayChangedCommand
    // TreeViewAdvLineStrokeArrayChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvLineStrokeArrayChangedCommand : ControlCommandBase<TreeViewAdvLineStrokeArrayChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvLineStrokeArrayChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.LineStrokeArrayChanged += OnEventRaised;
        }
    }

    // TreeViewAdvLineStrokeArrayChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvLineStrokeArrayChangedCommandBehavior<T> : TreeViewAdvLineStrokeArrayChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvRootLineStrokeChangedCommand
    // TreeViewAdvRootLineStrokeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvRootLineStrokeChangedCommand : ControlCommandBase<TreeViewAdvRootLineStrokeChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvRootLineStrokeChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.RootLineStrokeChanged += OnEventRaised;
        }
    }

    // TreeViewAdvRootLineStrokeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvRootLineStrokeChangedCommandBehavior<T> : TreeViewAdvRootLineStrokeChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvRootLineVisibilityChangedCommand
    // TreeViewAdvRootLineVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvRootLineVisibilityChangedCommand : ControlCommandBase<TreeViewAdvRootLineVisibilityChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvRootLineVisibilityChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.RootLineVisibilityChanged += OnEventRaised;
        }
    }

    // TreeViewAdvRootLineVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvRootLineVisibilityChangedCommandBehavior<T> : TreeViewAdvRootLineVisibilityChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvDragLineVisibilityChangedCommand
    // TreeViewAdvDragLineVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragLineVisibilityChangedCommand : ControlCommandBase<TreeViewAdvDragLineVisibilityChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragLineVisibilityChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragLineVisibilityChanged += OnEventRaised;
        }
    }

    // TreeViewAdvDragLineVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvDragLineVisibilityChangedCommandBehavior<T> : TreeViewAdvDragLineVisibilityChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvDragLineColorChangedCommand
    // TreeViewAdvDragLineColorChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragLineColorChangedCommand : ControlCommandBase<TreeViewAdvDragLineColorChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvDragLineColorChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragLineColorChanged += OnEventRaised;
        }
    }

    // TreeViewAdvDragLineColorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvDragLineColorChangedCommandBehavior<T> : TreeViewAdvDragLineColorChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvThemeChangedCommand
    // TreeViewAdvThemeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvThemeChangedCommand : ControlCommandBase<TreeViewAdvThemeChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvThemeChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ThemeChanged += OnEventRaised;
        }
    }

    // TreeViewAdvThemeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvThemeChangedCommandBehavior<T> : TreeViewAdvThemeChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvScrollingSpeedChangedCommand
    // TreeViewAdvScrollingSpeedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvScrollingSpeedChangedCommand : ControlCommandBase<TreeViewAdvScrollingSpeedChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvScrollingSpeedChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ScrollingSpeedChanged += OnEventRaised;
        }
    }

    // TreeViewAdvScrollingSpeedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvScrollingSpeedChangedCommandBehavior<T> : TreeViewAdvScrollingSpeedChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvExpanderTemplateChangedCommand
    // TreeViewAdvExpanderTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvExpanderTemplateChangedCommand : ControlCommandBase<TreeViewAdvExpanderTemplateChangedCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvExpanderTemplateChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ExpanderTemplateChanged += OnEventRaised;
        }
    }

    // TreeViewAdvExpanderTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvExpanderTemplateChangedCommandBehavior<T> : TreeViewAdvExpanderTemplateChangedCommandBehavior
    { }
    #endregion

    #region TreeViewAdvContextMenuOpeningCommand
    // TreeViewAdvContextMenuOpeningCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvContextMenuOpeningCommand : ControlCommandBase<TreeViewAdvContextMenuOpeningCommandBehavior, TreeViewAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewAdvContextMenuOpeningCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, ContextMenuEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuOpening += OnEventRaised;
        }
    }

    // TreeViewAdvContextMenuOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewAdvContextMenuOpeningCommandBehavior<T> : TreeViewAdvContextMenuOpeningCommandBehavior
    { }
    #endregion
}



