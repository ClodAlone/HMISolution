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
using Syncfusion.Windows.Chart.Olap;

namespace Syncfusion.Windows.Olap.MVVM
{
    #region OlapChartAfterRefreshCommand

    /// <summary>
    /// A command class to raise AfterRefresh event of OlapChart.
    /// </summary>
    public class OlapChartAfterRefreshCommand : ControlCommandBase<OlapChartAfterRefreshCommandBehavior, OlapChart>
    { }

    /// <summary>
    /// A behavior class to execute command when AfterRefresh event is raised.
    /// </summary>
    public class OlapChartAfterRefreshCommandBehavior : CommandBehaviorBase<OlapChart>
    {
        protected virtual void OnEventRaised(object sender, OlapChartRefreshEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.AfterRefresh += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic OlapChartAfterRefreshCommandBehavior type class.
    /// </summary>
    public class OlapChartAfterRefreshCommandBehavior<T> : OlapChartAfterRefreshCommandBehavior
    { }

    #endregion

    #region OlapChartBeforeRefreshCommand

    /// <summary>
    /// A command class to raise BeforeRefresh event of OlapChart.
    /// </summary>
    public class OlapChartBeforeRefreshCommand : ControlCommandBase<OlapChartBeforeRefreshCommandBehavior, OlapChart>
    { }

    /// <summary>
    /// A behavior class to execute command when BeforeRefresh event is raised
    /// </summary>
    public class OlapChartBeforeRefreshCommandBehavior : CommandBehaviorBase<OlapChart>
    {
        protected virtual void OnEventRaised(object sender, OlapChartRefreshEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.BeforeRefresh += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic OlapChartBeforeRefreshCommandBehavior type class.
    /// </summary>
    public class OlapChartBeforeRefreshCommandBehavior<T> : OlapChartBeforeRefreshCommandBehavior
    { }
    #endregion
}
