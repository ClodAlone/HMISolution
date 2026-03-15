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
using Syncfusion.Windows.Controls.PivotGrid;
#else
using Syncfusion.Silverlight.Controls.PivotGrid;
#endif

#if !SILVERLIGHT
namespace Syncfusion.Windows.Olap.MVVM
#else
namespace Syncfusion.Silverlight.Olap.MVVM
#endif
{
    #region PivotGridControlSelectionChangedCommand
    /// <summary>
    /// A command class to raise SelectionChanged event of <see cref=”PivotGridControl”/>.
    /// </summary>
    public class PivotGridControlSelectionChangedCommand : ControlCommandBase<PivotGridControlSelectionChangedCommandBehavior, PivotGridControl>
    { }

    /// <summary>
    ///A behavior class to execute command when SelectionChanged event is raised.
    /// </summary>
    public class PivotGridControlSelectionChangedCommandBehavior : CommandBehaviorBase<PivotGridControl>
    {
        protected virtual void OnEventRaised(object sender, PivotGridSelectionChangedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic PivotGridControlSelectionChangedCommandBehavior type class.
    /// </summary>
    public class PivotGridControlSelectionChangedCommandBehavior<T> : PivotGridControlSelectionChangedCommandBehavior
    { }
    #endregion

    #region PivotGridControlExpandingCommand
    /// <summary>
    /// A command class to raise the Expanding event of <see cref=”PivotGridControl”/>.
    /// </summary>
    public class PivotGridControlExpandingCommand : ControlCommandBase<PivotGridControlExpandingCommandBehavior, PivotGridControl>
    { }

    /// <summary>
    /// A behavior class to execute command when Expanding event is raised.
    /// </summary>
    public class PivotGridControlExpandingCommandBehavior : CommandBehaviorBase<PivotGridControl>
    {
        protected virtual void OnEventRaised(object sender, ExpandingEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.Expanding += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic PivotGridControlExpandingCommandBehavior type class.
    /// </summary>
    public class PivotGridControlExpandingCommandBehavior<T> : PivotGridControlExpandingCommandBehavior
    { }
    #endregion

    #region PivotGridControlExpandedCommand
    /// <summary>
    /// A command class to raise the Expanded event of <see cref=”PivotGridControl”/>.
    /// </summary>
    public class PivotGridControlExpandedCommand : ControlCommandBase<PivotGridControlExpandedCommandBehavior, PivotGridControl>
    { }

    /// <summary>
    /// A behavior class to execute command when Expanded event is raised.
    /// </summary>
    public class PivotGridControlExpandedCommandBehavior : CommandBehaviorBase<PivotGridControl>
    {
        protected virtual void OnEventRaised(object sender, ExpandedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.Expanded += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic PivotGridControlExpandedCommandBehavior type class
    /// </summary>
    public class PivotGridControlExpandedCommandBehavior<T> : PivotGridControlExpandedCommandBehavior
    { }
    #endregion

    #region PivotGridControlCollapsingCommand
    /// <summary>
    /// A command class to raise the Collapsing event of <see cref=”PivotGridControl”/>.
    /// </summary>
    public class PivotGridControlCollapsingCommand : ControlCommandBase<PivotGridControlCollapsingCommandBehavior, PivotGridControl>
    { }

    /// <summary>
    /// A behavior class to execute command when Collapsing event is raised.
    /// </summary>
    public class PivotGridControlCollapsingCommandBehavior : CommandBehaviorBase<PivotGridControl>
    {
        protected virtual void OnEventRaised(object sender, CollapsingEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.Collapsing += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic PivotGridControlCollapsingCommandBehavior type class.
    /// </summary>
    public class PivotGridControlCollapsingCommandBehavior<T> : PivotGridControlCollapsingCommandBehavior
    { }
    #endregion

    #region PivotGridControlCollapsedCommand
    /// <summary>
    /// A command class to raise the Collapsed event of <see cref=”PivotGridControl”/>.
    /// </summary>
    public class PivotGridControlCollapsedCommand : ControlCommandBase<PivotGridControlCollapsedCommandBehavior, PivotGridControl>
    { }

    /// <summary>
    /// A behavior class to execute command when Collapsed event is raised.
    /// </summary>
    public class PivotGridControlCollapsedCommandBehavior : CommandBehaviorBase<PivotGridControl>
    {
        protected virtual void OnEventRaised(object sender, CollapsedEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.Collapsed += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic PivotGridControlCollapsedCommandBehavior type class.
    /// </summary>
    public class PivotGridControlCollapsedCommandBehavior<T> : PivotGridControlCollapsedCommandBehavior
    { }
    #endregion

    #region PivotGridControlHyperlinkCellClickCommand
    /// <summary>
    /// A command class to raise the HyperLinkCellClick event of <see cref=”PivotGridControl”/>.
    /// </summary>
    public class PivotGridControlHyperlinkCellClickCommand : ControlCommandBase<PivotGridControlHyperlinkCellClickCommandBehavior, PivotGridControl>
    { }

    /// <summary>
    /// A behavior class to execute command when HyperlinkCellClick event is raised.
    /// </summary>
    public class PivotGridControlHyperlinkCellClickCommandBehavior : CommandBehaviorBase<PivotGridControl>
    {
        protected virtual void OnEventRaised(object sender, HyperlinkCellClickEventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.HyperlinkCellClick += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic PivotGridControlHyperlinkCellClickCommandBehavior type class.
    /// </summary>
    public class PivotGridControlHyperlinkCellClickCommandBehavior<T> : PivotGridControlHyperlinkCellClickCommandBehavior
    { }
    #endregion

    #region PivotGridControlDataRefreshingCommand
    /// <summary>
    /// A command class to raise the DataRefreshing event of <see cref=”PivotGridControl”/>.
    /// </summary>
    public class PivotGridControlDataRefreshingCommand : ControlCommandBase<PivotGridControlDataRefreshingCommandBehavior, PivotGridControl>
    { }

    /// <summary>
    /// A behavior class to execute command when DataRefreshing event is raised.
    /// </summary>
    public class PivotGridControlDataRefreshingCommandBehavior : CommandBehaviorBase<PivotGridControl>
    {
        protected virtual void OnEventRaised(object sender, DataRefreshingArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.DataRefreshing += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic PivotGridControlDataRefreshingCommandBehavior type class.
    /// </summary>
    public class PivotGridControlDataRefreshingCommandBehavior<T> : PivotGridControlDataRefreshingCommandBehavior
    { }
    #endregion

    #region PivotGridControlDataRefreshedCommand
    /// <summary>
    /// A command class to raise the DataRefreshed event of <see cref=”PivotGridControl”/>
    /// </summary>
    public class PivotGridControlDataRefreshedCommand : ControlCommandBase<PivotGridControlDataRefreshedCommandBehavior, PivotGridControl>
    { }

    /// <summary>
    /// A behavior class to execute command when DataRefreshed event is raised.
    /// </summary>
    public class PivotGridControlDataRefreshedCommandBehavior : CommandBehaviorBase<PivotGridControl>
    {
        protected virtual void OnEventRaised(object sender, DataRefreshedArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.DataRefreshed += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic PivotGridControlDataRefreshedCommandBehavior type class.
    /// </summary>
    public class PivotGridControlDataRefreshedCommandBehavior<T> : PivotGridControlDataRefreshedCommandBehavior
    { }
    #endregion

    #region PivotGridControlGroupingBarLoadedCommand
    /// <summary>
    /// A command class to raise the GroupingBarLoaded event of <see cref=”PivotGridControl”/>.
    /// </summary>
    public class PivotGridControlGroupingBarLoadedCommand : ControlCommandBase<PivotGridControlGroupingBarLoadedCommandBehavior, PivotGridControl>
    { }

    /// <summary>
    /// A behavior class to execute command when GroupingBarLoaded event is raised.
    /// </summary>
    public class PivotGridControlGroupingBarLoadedCommandBehavior : CommandBehaviorBase<PivotGridControl>
    {
        protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }

        protected override void OnTargetAttached()
        {
            TargetObject.GroupingBarLoaded += OnEventRaised;
        }
    }

    /// <summary>
    /// Represents a generic PivotGridControlGroupingBarLoadedCommandBehavior type class.
    /// </summary>
    public class PivotGridControlGroupingBarLoadedCommandBehavior<T> : PivotGridControlGroupingBarLoadedCommandBehavior
    { }
    #endregion
}
