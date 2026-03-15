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

    #region DataPagerExtPageIndexChangingCommand
    /// <summary>
    /// DataPagerExtPageIndexChangingCommand
    /// </summary>
    public class DataPagerExtPageIndexChangingCommand : ControlCommandBase<DataPagerExtPageIndexChangingCommandBehavior, DataPagerExt>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DataPagerExtPageIndexChangingCommandBehavior : CommandBehaviorBase<DataPagerExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, CancelEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.PageIndexChanging += OnEventRaised;
        }
    }

    /// <summary>
    /// DataPagerExtPageIndexChangingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataPagerExtPageIndexChangingCommandBehavior<T> : DataPagerExtPageIndexChangingCommandBehavior
    { }
    #endregion

    #region DataPagerExtPageIndexChangedCommand
    /// <summary>
    /// DataPagerExtPageIndexChangedCommand
    /// </summary>
    public class DataPagerExtPageIndexChangedCommand : ControlCommandBase<DataPagerExtPageIndexChangedCommandBehavior, DataPagerExt>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DataPagerExtPageIndexChangedCommandBehavior : CommandBehaviorBase<DataPagerExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.PageIndexChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DataPagerExtPageIndexChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataPagerExtPageIndexChangedCommandBehavior<T> : DataPagerExtPageIndexChangedCommandBehavior
    { }
    #endregion

    #region DataPagerExtOnDemandDataSourceLoadCommand
    /// <summary>
    /// DataPagerExtOnDemandDataSourceLoadCommand
    /// </summary>
    public class DataPagerExtOnDemandDataSourceLoadCommand : ControlCommandBase<DataPagerExtOnDemandDataSourceLoadCommandBehavior, DataPagerExt>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DataPagerExtOnDemandDataSourceLoadCommandBehavior : CommandBehaviorBase<DataPagerExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, GridDataOnDemandPageLoadingEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.OnDemandDataSourceLoad += OnEventRaised;
        }
    }

    /// <summary>
    /// DataPagerExtOnDemandDataSourceLoadCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataPagerExtOnDemandDataSourceLoadCommandBehavior<T> : DataPagerExtOnDemandDataSourceLoadCommandBehavior
    { }
    #endregion

    


}



