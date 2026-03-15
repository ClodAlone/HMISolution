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

    #region TreeViewItemAdvExpandingCommand
    // TreeViewItemAdvExpandingCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandingCommand : ControlCommandBase<TreeViewItemAdvExpandingCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandingCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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

    // TreeViewItemAdvExpandingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandingCommandBehavior<T> : TreeViewItemAdvExpandingCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvCollapsingCommand
    // TreeViewItemAdvCollapsingCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapsingCommand : ControlCommandBase<TreeViewItemAdvCollapsingCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapsingCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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

    // TreeViewItemAdvCollapsingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapsingCommandBehavior<T> : TreeViewItemAdvCollapsingCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvHasItemsChangedCommand
    // TreeViewItemAdvHasItemsChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvHasItemsChangedCommand : ControlCommandBase<TreeViewItemAdvHasItemsChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvHasItemsChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.HasItemsChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvHasItemsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvHasItemsChangedCommandBehavior<T> : TreeViewItemAdvHasItemsChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvHeaderChangedCommand
    // TreeViewItemAdvHeaderChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvHeaderChangedCommand : ControlCommandBase<TreeViewItemAdvHeaderChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvHeaderChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.HeaderChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvHeaderChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvHeaderChangedCommandBehavior<T> : TreeViewItemAdvHeaderChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvIsExpandedChangedCommand
    // TreeViewItemAdvIsExpandedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvIsExpandedChangedCommand : ControlCommandBase<TreeViewItemAdvIsExpandedChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvIsExpandedChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.IsExpandedChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvIsExpandedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvIsExpandedChangedCommandBehavior<T> : TreeViewItemAdvIsExpandedChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvIsMouseOverChangedCommand
    // TreeViewItemAdvIsMouseOverChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvIsMouseOverChangedCommand : ControlCommandBase<TreeViewItemAdvIsMouseOverChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvIsMouseOverChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.IsMouseOverChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvIsMouseOverChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvIsMouseOverChangedCommandBehavior<T> : TreeViewItemAdvIsMouseOverChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvIsSelectedChangedCommand
    // TreeViewItemAdvIsSelectedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvIsSelectedChangedCommand : ControlCommandBase<TreeViewItemAdvIsSelectedChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvIsSelectedChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.IsSelectedChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvIsSelectedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvIsSelectedChangedCommandBehavior<T> : TreeViewItemAdvIsSelectedChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvSelectOnExpandChangeChangedCommand
    // TreeViewItemAdvSelectOnExpandChangeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvSelectOnExpandChangeChangedCommand : ControlCommandBase<TreeViewItemAdvSelectOnExpandChangeChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvSelectOnExpandChangeChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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

    // TreeViewItemAdvSelectOnExpandChangeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvSelectOnExpandChangeChangedCommandBehavior<T> : TreeViewItemAdvSelectOnExpandChangeChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvLeftImageSourceChangedCommand
    // TreeViewItemAdvLeftImageSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvLeftImageSourceChangedCommand : ControlCommandBase<TreeViewItemAdvLeftImageSourceChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvLeftImageSourceChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.LeftImageSourceChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvLeftImageSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvLeftImageSourceChangedCommandBehavior<T> : TreeViewItemAdvLeftImageSourceChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvRightImageSourceChangedCommand
    // TreeViewItemAdvRightImageSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvRightImageSourceChangedCommand : ControlCommandBase<TreeViewItemAdvRightImageSourceChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvRightImageSourceChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.RightImageSourceChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvRightImageSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvRightImageSourceChangedCommandBehavior<T> : TreeViewItemAdvRightImageSourceChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvLeftImageWidthChangedCommand
    // TreeViewItemAdvLeftImageWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvLeftImageWidthChangedCommand : ControlCommandBase<TreeViewItemAdvLeftImageWidthChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvLeftImageWidthChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.LeftImageWidthChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvLeftImageWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvLeftImageWidthChangedCommandBehavior<T> : TreeViewItemAdvLeftImageWidthChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvRightImageWidthChangedCommand
    // TreeViewItemAdvRightImageWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvRightImageWidthChangedCommand : ControlCommandBase<TreeViewItemAdvRightImageWidthChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvRightImageWidthChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.RightImageWidthChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvRightImageWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvRightImageWidthChangedCommandBehavior<T> : TreeViewItemAdvRightImageWidthChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvLeftImageHeightChangedCommand
    // TreeViewItemAdvLeftImageHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvLeftImageHeightChangedCommand : ControlCommandBase<TreeViewItemAdvLeftImageHeightChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvLeftImageHeightChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.LeftImageHeightChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvLeftImageHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvLeftImageHeightChangedCommandBehavior<T> : TreeViewItemAdvLeftImageHeightChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvRightImageHeightChangedCommand
    // TreeViewItemAdvRightImageHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvRightImageHeightChangedCommand : ControlCommandBase<TreeViewItemAdvRightImageHeightChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvRightImageHeightChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.RightImageHeightChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvRightImageHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvRightImageHeightChangedCommandBehavior<T> : TreeViewItemAdvRightImageHeightChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvImageHorizontalAlignmentChangedCommand
    // TreeViewItemAdvImageHorizontalAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvImageHorizontalAlignmentChangedCommand : ControlCommandBase<TreeViewItemAdvImageHorizontalAlignmentChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvImageHorizontalAlignmentChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.ImageHorizontalAlignmentChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvImageHorizontalAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvImageHorizontalAlignmentChangedCommandBehavior<T> : TreeViewItemAdvImageHorizontalAlignmentChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvImageVerticalAlignmentChangedCommand
    // TreeViewItemAdvImageVerticalAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvImageVerticalAlignmentChangedCommand : ControlCommandBase<TreeViewItemAdvImageVerticalAlignmentChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvImageVerticalAlignmentChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.ImageVerticalAlignmentChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvImageVerticalAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvImageVerticalAlignmentChangedCommandBehavior<T> : TreeViewItemAdvImageVerticalAlignmentChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvImageMarginChangedCommand
    // TreeViewItemAdvImageMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvImageMarginChangedCommand : ControlCommandBase<TreeViewItemAdvImageMarginChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvImageMarginChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.ImageMarginChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvImageMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvImageMarginChangedCommandBehavior<T> : TreeViewItemAdvImageMarginChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvTextHorizontalAlignmentChangedCommand
    // TreeViewItemAdvTextHorizontalAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvTextHorizontalAlignmentChangedCommand : ControlCommandBase<TreeViewItemAdvTextHorizontalAlignmentChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvTextHorizontalAlignmentChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.TextHorizontalAlignmentChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvTextHorizontalAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvTextHorizontalAlignmentChangedCommandBehavior<T> : TreeViewItemAdvTextHorizontalAlignmentChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvTextVerticalAlignmentChangedCommand
    // TreeViewItemAdvTextVerticalAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvTextVerticalAlignmentChangedCommand : ControlCommandBase<TreeViewItemAdvTextVerticalAlignmentChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvTextVerticalAlignmentChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.TextVerticalAlignmentChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvTextVerticalAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvTextVerticalAlignmentChangedCommandBehavior<T> : TreeViewItemAdvTextVerticalAlignmentChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvTextMarginChangedCommand
    // TreeViewItemAdvTextMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvTextMarginChangedCommand : ControlCommandBase<TreeViewItemAdvTextMarginChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvTextMarginChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.TextMarginChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvTextMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvTextMarginChangedCommandBehavior<T> : TreeViewItemAdvTextMarginChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvExpanderTemplateChangedCommand
    // TreeViewItemAdvExpanderTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpanderTemplateChangedCommand : ControlCommandBase<TreeViewItemAdvExpanderTemplateChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpanderTemplateChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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

    // TreeViewItemAdvExpanderTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpanderTemplateChangedCommandBehavior<T> : TreeViewItemAdvExpanderTemplateChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvCollapseImageSourceChangedCommand
    // TreeViewItemAdvCollapseImageSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapseImageSourceChangedCommand : ControlCommandBase<TreeViewItemAdvCollapseImageSourceChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapseImageSourceChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.CollapseImageSourceChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvCollapseImageSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapseImageSourceChangedCommandBehavior<T> : TreeViewItemAdvCollapseImageSourceChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvExpandImageSourceChangedCommand
    // TreeViewItemAdvExpandImageSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandImageSourceChangedCommand : ControlCommandBase<TreeViewItemAdvExpandImageSourceChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandImageSourceChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.ExpandImageSourceChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvExpandImageSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandImageSourceChangedCommandBehavior<T> : TreeViewItemAdvExpandImageSourceChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvCollapseImageWidthChangedCommand
    // TreeViewItemAdvCollapseImageWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapseImageWidthChangedCommand : ControlCommandBase<TreeViewItemAdvCollapseImageWidthChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapseImageWidthChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.CollapseImageWidthChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvCollapseImageWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapseImageWidthChangedCommandBehavior<T> : TreeViewItemAdvCollapseImageWidthChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvCollapseImageHeightChangedCommand
    // TreeViewItemAdvCollapseImageHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapseImageHeightChangedCommand : ControlCommandBase<TreeViewItemAdvCollapseImageHeightChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapseImageHeightChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.CollapseImageHeightChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvCollapseImageHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvCollapseImageHeightChangedCommandBehavior<T> : TreeViewItemAdvCollapseImageHeightChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvExpandImageWidthChangedCommand
    // TreeViewItemAdvExpandImageWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandImageWidthChangedCommand : ControlCommandBase<TreeViewItemAdvExpandImageWidthChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandImageWidthChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.ExpandImageWidthChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvExpandImageWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandImageWidthChangedCommandBehavior<T> : TreeViewItemAdvExpandImageWidthChangedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvExpandImageHeightChangedCommand
    // TreeViewItemAdvExpandImageHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandImageHeightChangedCommand : ControlCommandBase<TreeViewItemAdvExpandImageHeightChangedCommandBehavior, TreeViewItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandImageHeightChangedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.ExpandImageHeightChanged += OnEventRaised;
        }
    }

    // TreeViewItemAdvExpandImageHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TreeViewItemAdvExpandImageHeightChangedCommandBehavior<T> : TreeViewItemAdvExpandImageHeightChangedCommandBehavior
    { }
    #endregion
}

