#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{

    #region SimpleMenuButtonDescriptionChangedCommand
    /// <summary>
    /// SimpleMenuButtonDescriptionChangedCommand
    /// </summary>
    public class SimpleMenuButtonDescriptionChangedCommand : ControlCommandBase<SimpleMenuButtonDescriptionChangedCommandBehavior, SimpleMenuButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class SimpleMenuButtonDescriptionChangedCommandBehavior : CommandBehaviorBase<SimpleMenuButton>
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
    /// SimpleMenuButtonDescriptionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleMenuButtonDescriptionChangedCommandBehavior<T> : SimpleMenuButtonDescriptionChangedCommandBehavior
    { }
    #endregion

    #region SimpleMenuButtonIconChangedCommand
    /// <summary>
    /// SimpleMenuButtonIconChangedCommand
    /// </summary>
    public class SimpleMenuButtonIconChangedCommand : ControlCommandBase<SimpleMenuButtonIconChangedCommandBehavior, SimpleMenuButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class SimpleMenuButtonIconChangedCommandBehavior : CommandBehaviorBase<SimpleMenuButton>
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
    /// SimpleMenuButtonIconChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleMenuButtonIconChangedCommandBehavior<T> : SimpleMenuButtonIconChangedCommandBehavior
    { }
    #endregion

    #region SimpleMenuButtonIconSizeChangedCommand
    /// <summary>
    /// SimpleMenuButtonIconSizeChangedCommand
    /// </summary>
    public class SimpleMenuButtonIconSizeChangedCommand : ControlCommandBase<SimpleMenuButtonIconSizeChangedCommandBehavior, SimpleMenuButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class SimpleMenuButtonIconSizeChangedCommandBehavior : CommandBehaviorBase<SimpleMenuButton>
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
    /// SimpleMenuButtonIconSizeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleMenuButtonIconSizeChangedCommandBehavior<T> : SimpleMenuButtonIconSizeChangedCommandBehavior
    { }
    #endregion

    #region SimpleMenuButtonLabelChangedCommand
    /// <summary>
    /// SimpleMenuButtonLabelChangedCommand
    /// </summary>
    public class SimpleMenuButtonLabelChangedCommand : ControlCommandBase<SimpleMenuButtonLabelChangedCommandBehavior, SimpleMenuButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class SimpleMenuButtonLabelChangedCommandBehavior : CommandBehaviorBase<SimpleMenuButton>
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
    /// SimpleMenuButtonLabelChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleMenuButtonLabelChangedCommandBehavior<T> : SimpleMenuButtonLabelChangedCommandBehavior
    { }
    #endregion
}


