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

    #region TabControlAdvTabItemStyleChangedCommand
    // TabControlAdvTabItemStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemStyleChangedCommand : ControlCommandBase<TabControlAdvTabItemStyleChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemStyleChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.TabItemStyleChanged += OnEventRaised;
        }
    }

    // TabControlAdvTabItemStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemStyleChangedCommandBehavior<T> : TabControlAdvTabItemStyleChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvSelectedIndexChangedCommand
    // TabControlAdvSelectedIndexChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvSelectedIndexChangedCommand : ControlCommandBase<TabControlAdvSelectedIndexChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvSelectedIndexChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.SelectedIndexChanged += OnEventRaised;
        }
    }

    // TabControlAdvSelectedIndexChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvSelectedIndexChangedCommandBehavior<T> : TabControlAdvSelectedIndexChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvSelectedItemChangedCommand
    // TabControlAdvSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvSelectedItemChangedCommand : ControlCommandBase<TabControlAdvSelectedItemChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvSelectedItemChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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

    // TabControlAdvSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvSelectedItemChangedCommandBehavior<T> : TabControlAdvSelectedItemChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvTabStripPlacementChangedCommand
    // TabControlAdvTabStripPlacementChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabStripPlacementChangedCommand : ControlCommandBase<TabControlAdvTabStripPlacementChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabStripPlacementChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.TabStripPlacementChanged += OnEventRaised;
        }
    }

    // TabControlAdvTabStripPlacementChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvTabStripPlacementChangedCommandBehavior<T> : TabControlAdvTabStripPlacementChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvScrollingTimeChangedCommand
    // TabControlAdvScrollingTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvScrollingTimeChangedCommand : ControlCommandBase<TabControlAdvScrollingTimeChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvScrollingTimeChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.ScrollingTimeChanged += OnEventRaised;
        }
    }

    // TabControlAdvScrollingTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvScrollingTimeChangedCommandBehavior<T> : TabControlAdvScrollingTimeChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvIsAllTabsClosedChangedCommand
    // TabControlAdvIsAllTabsClosedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvIsAllTabsClosedChangedCommand : ControlCommandBase<TabControlAdvIsAllTabsClosedChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvIsAllTabsClosedChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.IsAllTabsClosedChanged += OnEventRaised;
        }
    }

    // TabControlAdvIsAllTabsClosedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvIsAllTabsClosedChangedCommandBehavior<T> : TabControlAdvIsAllTabsClosedChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvEnableLabelEditChangedCommand
    // TabControlAdvEnableLabelEditChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvEnableLabelEditChangedCommand : ControlCommandBase<TabControlAdvEnableLabelEditChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvEnableLabelEditChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.EnableLabelEditChanged += OnEventRaised;
        }
    }

    // TabControlAdvEnableLabelEditChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvEnableLabelEditChangedCommandBehavior<T> : TabControlAdvEnableLabelEditChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvShowTabListContextMenuChangedCommand
    // TabControlAdvShowTabListContextMenuChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvShowTabListContextMenuChangedCommand : ControlCommandBase<TabControlAdvShowTabListContextMenuChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvShowTabListContextMenuChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.ShowTabListContextMenuChanged += OnEventRaised;
        }
    }

    // TabControlAdvShowTabListContextMenuChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvShowTabListContextMenuChangedCommandBehavior<T> : TabControlAdvShowTabListContextMenuChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvCloseButtonTypeChangedCommand
    // TabControlAdvCloseButtonTypeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvCloseButtonTypeChangedCommand : ControlCommandBase<TabControlAdvCloseButtonTypeChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvCloseButtonTypeChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.CloseButtonTypeChanged += OnEventRaised;
        }
    }

    // TabControlAdvCloseButtonTypeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvCloseButtonTypeChangedCommandBehavior<T> : TabControlAdvCloseButtonTypeChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvHotTrackingEnabledChangedCommand
    // TabControlAdvHotTrackingEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvHotTrackingEnabledChangedCommand : ControlCommandBase<TabControlAdvHotTrackingEnabledChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvHotTrackingEnabledChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.HotTrackingEnabledChanged += OnEventRaised;
        }
    }

    // TabControlAdvHotTrackingEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvHotTrackingEnabledChangedCommandBehavior<T> : TabControlAdvHotTrackingEnabledChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvRotateTextWhenVerticalChangedCommand
    // TabControlAdvRotateTextWhenVerticalChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvRotateTextWhenVerticalChangedCommand : ControlCommandBase<TabControlAdvRotateTextWhenVerticalChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvRotateTextWhenVerticalChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.RotateTextWhenVerticalChanged += OnEventRaised;
        }
    }

    // TabControlAdvRotateTextWhenVerticalChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvRotateTextWhenVerticalChangedCommandBehavior<T> : TabControlAdvRotateTextWhenVerticalChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvTabPanelStyleChangedCommand
    // TabControlAdvTabPanelStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabPanelStyleChangedCommand : ControlCommandBase<TabControlAdvTabPanelStyleChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabPanelStyleChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.TabPanelStyleChanged += OnEventRaised;
        }
    }

    // TabControlAdvTabPanelStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvTabPanelStyleChangedCommandBehavior<T> : TabControlAdvTabPanelStyleChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvTabScrollButtonVisibilityChangedCommand
    // TabControlAdvTabScrollButtonVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabScrollButtonVisibilityChangedCommand : ControlCommandBase<TabControlAdvTabScrollButtonVisibilityChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabScrollButtonVisibilityChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.TabScrollButtonVisibilityChanged += OnEventRaised;
        }
    }

    // TabControlAdvTabScrollButtonVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvTabScrollButtonVisibilityChangedCommandBehavior<T> : TabControlAdvTabScrollButtonVisibilityChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvTabScrollStyleChangedCommand
    // TabControlAdvTabScrollStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabScrollStyleChangedCommand : ControlCommandBase<TabControlAdvTabScrollStyleChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabScrollStyleChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.TabScrollStyleChanged += OnEventRaised;
        }
    }

    // TabControlAdvTabScrollStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvTabScrollStyleChangedCommandBehavior<T> : TabControlAdvTabScrollStyleChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvTabItemLayoutChangedCommand
    // TabControlAdvTabItemLayoutChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemLayoutChangedCommand : ControlCommandBase<TabControlAdvTabItemLayoutChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemLayoutChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.TabItemLayoutChanged += OnEventRaised;
        }
    }

    // TabControlAdvTabItemLayoutChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemLayoutChangedCommandBehavior<T> : TabControlAdvTabItemLayoutChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvTabItemSizeModeChangedCommand
    // TabControlAdvTabItemSizeModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemSizeModeChangedCommand : ControlCommandBase<TabControlAdvTabItemSizeModeChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemSizeModeChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.TabItemSizeModeChanged += OnEventRaised;
        }
    }

    // TabControlAdvTabItemSizeModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemSizeModeChangedCommandBehavior<T> : TabControlAdvTabItemSizeModeChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvKeepTabInFrontChangedCommand
    // TabControlAdvKeepTabInFrontChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvKeepTabInFrontChangedCommand : ControlCommandBase<TabControlAdvKeepTabInFrontChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvKeepTabInFrontChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.KeepTabInFrontChanged += OnEventRaised;
        }
    }

    // TabControlAdvKeepTabInFrontChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvKeepTabInFrontChangedCommandBehavior<T> : TabControlAdvKeepTabInFrontChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvSelectedItemFontWeightChangedCommand
    // TabControlAdvSelectedItemFontWeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvSelectedItemFontWeightChangedCommand : ControlCommandBase<TabControlAdvSelectedItemFontWeightChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvSelectedItemFontWeightChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.SelectedItemFontWeightChanged += OnEventRaised;
        }
    }

    // TabControlAdvSelectedItemFontWeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvSelectedItemFontWeightChangedCommandBehavior<T> : TabControlAdvSelectedItemFontWeightChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvTabClosingCommand
    // TabControlAdvTabClosingCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabClosingCommand : ControlCommandBase<TabControlAdvTabClosingCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabClosingCommandBehavior : CommandBehaviorBase<TabControlAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, CloseTabEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TabClosing += OnEventRaised;
        }
    }

    // TabControlAdvTabClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvTabClosingCommandBehavior<T> : TabControlAdvTabClosingCommandBehavior
    { }
    #endregion

    #region TabControlAdvBeforeLabelEditCommand
    // TabControlAdvBeforeLabelEditCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvBeforeLabelEditCommand : ControlCommandBase<TabControlAdvBeforeLabelEditCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvBeforeLabelEditCommandBehavior : CommandBehaviorBase<TabControlAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, BeforeLabelEditEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BeforeLabelEdit += OnEventRaised;
        }
    }

    // TabControlAdvBeforeLabelEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvBeforeLabelEditCommandBehavior<T> : TabControlAdvBeforeLabelEditCommandBehavior
    { }
    #endregion

    #region TabControlAdvAfterLabelEditCommand
    // TabControlAdvAfterLabelEditCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvAfterLabelEditCommand : ControlCommandBase<TabControlAdvAfterLabelEditCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvAfterLabelEditCommandBehavior : CommandBehaviorBase<TabControlAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, AfterLabelEditEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.AfterLabelEdit += OnEventRaised;
        }
    }

    // TabControlAdvAfterLabelEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvAfterLabelEditCommandBehavior<T> : TabControlAdvAfterLabelEditCommandBehavior
    { }
    #endregion

    #region TabControlAdvDropDownContextMenuOpenCommand
    // TabControlAdvDropDownContextMenuOpenCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvDropDownContextMenuOpenCommand : ControlCommandBase<TabControlAdvDropDownContextMenuOpenCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvDropDownContextMenuOpenCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.DropDownContextMenuOpen += OnEventRaised;
        }
    }

    // TabControlAdvDropDownContextMenuOpenCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvDropDownContextMenuOpenCommandBehavior<T> : TabControlAdvDropDownContextMenuOpenCommandBehavior
    { }
    #endregion

    #region TabControlAdvDropDownContextMenuCloseCommand
    // TabControlAdvDropDownContextMenuCloseCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvDropDownContextMenuCloseCommand : ControlCommandBase<TabControlAdvDropDownContextMenuCloseCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvDropDownContextMenuCloseCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.DropDownContextMenuClose += OnEventRaised;
        }
    }

    // TabControlAdvDropDownContextMenuCloseCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvDropDownContextMenuCloseCommandBehavior<T> : TabControlAdvDropDownContextMenuCloseCommandBehavior
    { }
    #endregion

    #region TabControlAdvDragStartCommand
    // TabControlAdvDragStartCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvDragStartCommand : ControlCommandBase<TabControlAdvDragStartCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvDragStartCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.DragStart += OnEventRaised;
        }
    }

    // TabControlAdvDragStartCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvDragStartCommandBehavior<T> : TabControlAdvDragStartCommandBehavior
    { }
    #endregion

    #region TabControlAdvDragEndCommand
    // TabControlAdvDragEndCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvDragEndCommand : ControlCommandBase<TabControlAdvDragEndCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvDragEndCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.DragEnd += OnEventRaised;
        }
    }

    // TabControlAdvDragEndCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvDragEndCommandBehavior<T> : TabControlAdvDragEndCommandBehavior
    { }
    #endregion

    #region TabControlAdvAllowDragDropChangedCommand
    // TabControlAdvAllowDragDropChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvAllowDragDropChangedCommand : ControlCommandBase<TabControlAdvAllowDragDropChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvAllowDragDropChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.AllowDragDropChanged += OnEventRaised;
        }
    }

    // TabControlAdvAllowDragDropChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvAllowDragDropChangedCommandBehavior<T> : TabControlAdvAllowDragDropChangedCommandBehavior
    { }
    #endregion

    #region TabControlAdvTabItemOpenModeChangedCommand
    // TabControlAdvTabItemOpenModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemOpenModeChangedCommand : ControlCommandBase<TabControlAdvTabItemOpenModeChangedCommandBehavior, TabControlAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemOpenModeChangedCommandBehavior : CommandBehaviorBase<TabControlAdv>
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
            TargetObject.TabItemOpenModeChanged += OnEventRaised;
        }
    }

    // TabControlAdvTabItemOpenModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabControlAdvTabItemOpenModeChangedCommandBehavior<T> : TabControlAdvTabItemOpenModeChangedCommandBehavior
    { }
    #endregion
}
