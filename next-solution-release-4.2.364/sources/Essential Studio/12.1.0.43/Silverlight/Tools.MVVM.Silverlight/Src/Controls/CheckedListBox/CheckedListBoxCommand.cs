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

    #region CheckedListBoxFullRowSelectionChangedCommand
    // CheckedListBoxFullRowSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxFullRowSelectionChangedCommand : ControlCommandBase<CheckedListBoxFullRowSelectionChangedCommandBehavior, CheckedListBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxFullRowSelectionChangedCommandBehavior : CommandBehaviorBase<CheckedListBox>
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
            TargetObject.FullRowSelectionChanged += OnEventRaised;
        }
    }

    // CheckedListBoxFullRowSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxFullRowSelectionChangedCommandBehavior<T> : CheckedListBoxFullRowSelectionChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxCheckOnClickChangedCommand
    // CheckedListBoxCheckOnClickChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxCheckOnClickChangedCommand : ControlCommandBase<CheckedListBoxCheckOnClickChangedCommandBehavior, CheckedListBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxCheckOnClickChangedCommandBehavior : CommandBehaviorBase<CheckedListBox>
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
            TargetObject.CheckOnClickChanged += OnEventRaised;
        }
    }

    // CheckedListBoxCheckOnClickChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxCheckOnClickChangedCommandBehavior<T> : CheckedListBoxCheckOnClickChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxAlignmentChangedCommand
    // CheckedListBoxAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxAlignmentChangedCommand : ControlCommandBase<CheckedListBoxAlignmentChangedCommandBehavior, CheckedListBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxAlignmentChangedCommandBehavior : CommandBehaviorBase<CheckedListBox>
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
            TargetObject.AlignmentChanged += OnEventRaised;
        }
    }

    // CheckedListBoxAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxAlignmentChangedCommandBehavior<T> : CheckedListBoxAlignmentChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxSelectionChangedCommand
    // CheckedListBoxSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxSelectionChangedCommand : ControlCommandBase<CheckedListBoxSelectionChangedCommandBehavior, CheckedListBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxSelectionChangedCommandBehavior : CommandBehaviorBase<CheckedListBox>
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
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

    // CheckedListBoxSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxSelectionChangedCommandBehavior<T> : CheckedListBoxSelectionChangedCommandBehavior
    { }
    #endregion

    
    #region CheckedListBoxCheckedItemsChangedCommand
    // CheckedListBoxCheckedItemsChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxCheckedItemsChangedCommand : ControlCommandBase<CheckedListBoxCheckedItemsChangedCommandBehavior, CheckedListBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxCheckedItemsChangedCommandBehavior : CommandBehaviorBase<CheckedListBox>
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
            TargetObject.CheckedItemsChanged += OnEventRaised;
        }
    }

    // CheckedListBoxCheckedItemsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxCheckedItemsChangedCommandBehavior<T> : CheckedListBoxCheckedItemsChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxSelectedItemBackgroundChangedCommand
    // CheckedListBoxSelectedItemBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxSelectedItemBackgroundChangedCommand : ControlCommandBase<CheckedListBoxSelectedItemBackgroundChangedCommandBehavior, CheckedListBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxSelectedItemBackgroundChangedCommandBehavior : CommandBehaviorBase<CheckedListBox>
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
            TargetObject.SelectedItemBackgroundChanged += OnEventRaised;
        }
    }

    // CheckedListBoxSelectedItemBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxSelectedItemBackgroundChangedCommandBehavior<T> : CheckedListBoxSelectedItemBackgroundChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxModeChangedCommand
    // CheckedListBoxModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxModeChangedCommand : ControlCommandBase<CheckedListBoxModeChangedCommandBehavior, CheckedListBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxModeChangedCommandBehavior : CommandBehaviorBase<CheckedListBox>
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
            TargetObject.ModeChanged += OnEventRaised;
        }
    }

    // CheckedListBoxModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxModeChangedCommandBehavior<T> : CheckedListBoxModeChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxMouseOverBackgroundChangedCommand
    // CheckedListBoxMouseOverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxMouseOverBackgroundChangedCommand : ControlCommandBase<CheckedListBoxMouseOverBackgroundChangedCommandBehavior, CheckedListBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxMouseOverBackgroundChangedCommandBehavior : CommandBehaviorBase<CheckedListBox>
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
            TargetObject.MouseOverBackgroundChanged += OnEventRaised;
        }
    }

    // CheckedListBoxMouseOverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxMouseOverBackgroundChangedCommandBehavior<T> : CheckedListBoxMouseOverBackgroundChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxCheckBoxStyleChangedCommand
    // CheckedListBoxCheckBoxStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxCheckBoxStyleChangedCommand : ControlCommandBase<CheckedListBoxCheckBoxStyleChangedCommandBehavior, CheckedListBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxCheckBoxStyleChangedCommandBehavior : CommandBehaviorBase<CheckedListBox>
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
            TargetObject.CheckBoxStyleChanged += OnEventRaised;
        }
    }

    // CheckedListBoxCheckBoxStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxCheckBoxStyleChangedCommandBehavior<T> : CheckedListBoxCheckBoxStyleChangedCommandBehavior
    { }
    #endregion
}
