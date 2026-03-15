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
using Syncfusion.Windows.Shared;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools.MVVM
{

    #region RibbonSimpleMenuButtonCommmands


    #region RibbonSimpleMenuButtonDescriptionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonDescriptionChangedCommand : ControlCommandBase<RibbonSimpleMenuButtonDescriptionChangedCommandBehavior, SimpleMenuButton>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonDescriptionChangedCommandBehavior : CommandBehaviorBase<SimpleMenuButton>
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
            TargetObject.DescriptionChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonSimpleMenuButtonDescriptionChangedCommandBehavior<T> : RibbonSimpleMenuButtonDescriptionChangedCommandBehavior
    { }
    #endregion


    #region RibbonSimpleMenuButtonIconChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonIconChangedCommand : ControlCommandBase<RibbonSimpleMenuButtonIconChangedCommandBehavior, SimpleMenuButton>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonIconChangedCommandBehavior : CommandBehaviorBase<SimpleMenuButton>
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
            TargetObject.IconChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonSimpleMenuButtonIconChangedCommandBehavior<T> : RibbonSimpleMenuButtonIconChangedCommandBehavior
    { }
    #endregion

    #region RibbonSimpleMenuButtonIconSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonIconSizeChangedCommand : ControlCommandBase<RibbonSimpleMenuButtonIconSizeChangedCommandBehavior, SimpleMenuButton>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonIconSizeChangedCommandBehavior : CommandBehaviorBase<SimpleMenuButton>
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
            TargetObject.IconSizeChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonSimpleMenuButtonIconSizeChangedCommandBehavior<T> : RibbonSimpleMenuButtonIconSizeChangedCommandBehavior
    { }
    #endregion


    #region RibbonSimpleMenuButtonIconSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonLabelChangedCommand : ControlCommandBase<RibbonSimpleMenuButtonLabelChangedCommandCommandBehavior, SimpleMenuButton>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonLabelChangedCommandCommandBehavior : CommandBehaviorBase<SimpleMenuButton>
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
            TargetObject.LabelChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonSimpleMenuButtonLabelChangedCommandCommandBehavior<T> : RibbonSimpleMenuButtonLabelChangedCommandCommandBehavior
    { }
    #endregion

    #region RibbonSimpleMenuButtonSmallIconChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonSmallIconChangedCommand : ControlCommandBase<RibbonSimpleMenuButtonSmallIconChangedCommandBehavior, SimpleMenuButton>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSimpleMenuButtonSmallIconChangedCommandBehavior : CommandBehaviorBase<SimpleMenuButton>
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
            TargetObject.SmallIconChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonSimpleMenuButtonSmallIconChangedCommandBehavior<T> : RibbonSimpleMenuButtonSmallIconChangedCommandBehavior
    { }
    #endregion
    #endregion
}
