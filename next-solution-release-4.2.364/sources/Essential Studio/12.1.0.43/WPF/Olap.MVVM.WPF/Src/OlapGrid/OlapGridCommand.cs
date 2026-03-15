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
#if !SILVERLIGHT
using Syncfusion.Windows.Grid.Olap;
#else
using Syncfusion.Silverlight.Grid.Olap;
#endif

#if !SILVERLIGHT
namespace Syncfusion.Windows.Olap.MVVM
#else
namespace Syncfusion.Silverlight.Olap.MVVM
#endif
{
    #region OlapGridAfterRefreshCommand
    /// <summary>
    /// A command class to raise AfterRefresh event of OlapChart.
    /// </summary>
    public class OlapGridAfterRefreshCommand : ControlCommandBase<OlapGridAfterRefreshCommandBehavior, OlapGrid>
    { }

    /// <summary>
    /// A behavior class to execute command when AfterRefresh event is raised.
    /// </summary>
    public class OlapGridAfterRefreshCommandBehavior : CommandBehaviorBase<OlapGrid>
    {
        protected virtual void OnEventRaised(object sender, OlapGridDrillDownEventArgs e)
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
    public class OlapGridAfterRefreshCommandBehavior<T> : OlapGridAfterRefreshCommandBehavior
    { }

    #endregion

    #region OlapGridBeforeRefreshCommand
    /// <summary>
    /// A command class to raise BeforeRefresh event of OlapChart.
    /// </summary>
    public class OlapGridBeforeRefreshCommand : ControlCommandBase<OlapGridBeforeRefreshCommandBehavior, OlapGrid>
    { }

    /// <summary>
    /// A behavior class to execute command when BeforeRefresh event is raised.
    /// </summary>
    public class OlapGridBeforeRefreshCommandBehavior : CommandBehaviorBase<OlapGrid>
    {
        protected virtual void OnEventRaised(object sender, OlapGridDrillDownEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.BeforeRefresh += OnEventRaised;
        }
    }

    /// <summary>
    ///  Represents a generic OlapChartBedforeRefreshCommandBehavior type class.
    /// </summary>
    public class OlapGridBeforeRefreshCommandBehavior<T> : OlapGridBeforeRefreshCommandBehavior
    { }

    #endregion

    #region OlapGridLinkClickCommand
    /// <summary>
    /// A command class to raise LinkClick event of OlapChart.
    /// </summary>
    public class OlapGridLinkClickCommand : ControlCommandBase<OlapGridLinkClickCommandBehavior, OlapGrid>
    { }

    /// <summary>
    /// A behavior class to execute command when LinkClick event is raised.
    /// </summary>
    public class OlapGridLinkClickCommandBehavior : CommandBehaviorBase<OlapGrid>
    {
        protected virtual void OnEventRaised(object sender, LinkLabelEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.LinkClick += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic OlapGridLinkClickCommandBehavior type class.
    /// </summary>
    public class OlapGridLinkClickCommandBehavior<T> : OlapGridLinkClickCommandBehavior
    { }
    #endregion

    #region OlapGridSelectionChangedCommand

    /// <summary>
    /// A command class to raise SelectionChanged event of OlapChart.
    /// </summary>
    public class OlapGridSelectionChangedCommand : ControlCommandBase<OlapGridSelectionChangedCommandBehavior, OlapGrid>
    { }

    /// <summary>
    /// A behavior class to execute command when SelectionChanged event is raised.
    /// </summary>
    public class OlapGridSelectionChangedCommandBehavior : CommandBehaviorBase<OlapGrid>
    {
        protected virtual void OnEventRaised(object sender, OlapGridSelectionChangedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic OlapGridSelectionChangedCommandBehavior type class.
    /// </summary>
    /// <typeparam name="T">Any Type</typeparam>
    public class OlapGridSelectionChangedCommandBehavior<T> : OlapGridSelectionChangedCommandBehavior
    { }
    #endregion
}
