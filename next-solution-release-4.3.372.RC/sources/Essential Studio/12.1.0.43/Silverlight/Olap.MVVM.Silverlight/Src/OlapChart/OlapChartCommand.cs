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
using Syncfusion.Silverlight.Chart.Olap;

namespace Syncfusion.Silverlight.Olap.MVVM
{
    #region OlapChartOnDataRefreshCompletedCommand

    /// <summary>
    /// A command class to raise OnDataRefreshCompleted event of OlapChart.
    /// </summary>
    public class OlapChartOnDataRefreshCompletedCommand : ControlCommandBase<OlapChartOnDataRefreshCompletedCommandBehavior, OlapChart>
    { }

    /// <summary>
    /// A behavior class to execute command when OnDataRefreshCompleted event is raised.
    /// </summary>
    public class OlapChartOnDataRefreshCompletedCommandBehavior : CommandBehaviorBase<OlapChart>
    {
        protected virtual void OnEventRaised(object sender, DataRefreshCompletedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.OnDataRefreshCompleted += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic OlapChartOnDataRefreshCompletedCommandBehavior type class.
    /// </summary>
    public class OlapChartOnDataRefreshCompletedCommandBehavior<T> : OlapChartOnDataRefreshCompletedCommandBehavior
    { }
    #endregion

    #region OlapChartOnDataRefreshBeginCommand
    /// <summary>
    /// A command class to raise OnDataRefreshBegin event of OlapChart.
    /// </summary>
    public class OlapChartOnDataRefreshBeginCommand : ControlCommandBase<OlapChartOnDataRefreshBeginCommandBehavior, OlapChart>
    { }

    /// <summary>
    /// A behavior class to execute command when OnDataRefreshBegin event is raised.
    /// </summary>
    public class OlapChartOnDataRefreshBeginCommandBehavior : CommandBehaviorBase<OlapChart>
    {
        protected virtual void OnEventRaised(object sender, DataRefreshBeginEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.OnDataRefreshBegin += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic OlapChartOnDataRefreshBeginCommandBehavior type class. 
    /// </summary>
    public class OlapChartOnDataRefreshBeginCommandBehavior<T> : OlapChartOnDataRefreshBeginCommandBehavior
    { }
    #endregion
}
