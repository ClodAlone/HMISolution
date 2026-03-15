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
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using System.Linq;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// Reresents the Docking Manager class.
    /// </summary>
    public partial class DockingManager
    {
        /// <summary>
        /// Represents the window coll.
        /// </summary>
        protected internal WindowParams windowcoll;

        /// <summary>
        /// Cleartargets the namein docked mode.
        /// </summary>
        protected internal void CleartargetNameinDockedMode()
        {

            for (int j = 1; j <= this.WindowCollection.Count; j++)
            {
                Window w = this.WindowCollection[j];
                w.TargetNameInDockedMode = string.Empty;
                w.TargetNameInFloatMode = string.Empty;
            }
            for (int j = 1; j <= this.WindowCollection.Count; j++)
            {
                Window w = this.WindowCollection[j];
                if (w.CustomTabControl != null)
                {
                    if (w.CustomTabControl.Items.Count > 1)
                    {
                        for (int i = 0; i < w.CustomTabControl.Items.Count; i++)
                        {
                            CustomTabItem cstab = w.CustomTabControl.Items[i] as CustomTabItem;

                            if (cstab.OwnWindow.CurrentStateMain == StateMaintanance.TabWithContainer || cstab.OwnWindow.CurrentStateMain == StateMaintanance.TabWithFloat)
                            {
                                cstab.OwnWindow.TargetNameInFloatMode = w._Caption;
                                cstab.OwnWindow.TargetNameInDockedMode = string.Empty;
                            }
                            else if (cstab.OwnWindow.CurrentStateMain == StateMaintanance.TabWithDock)
                            {
                                cstab.OwnWindow.TargetNameInDockedMode = w._Caption;
                                cstab.OwnWindow.TargetNameInFloatMode = string.Empty;
                            }
                            else if (w.DockManager.Parent is WindowContainer)
                            {
                                cstab.OwnWindow.TargetNameInDockedMode = string.Empty;
                                cstab.OwnWindow.TargetNameInFloatMode = w._Caption;
                            }
                        }
                    }
                }

            }
        }
        //protected internal WindowParams windowCollection;

        /// <summary>
        /// Saves state persisted for current docking location.
        /// </summary>
        /// <example>
        /// <para/>This example shows how to use SaveDockState( string path ) in C#.
        /// <code language="C#">
        /// dockingManager.SaveDockState( @"d:\docking_bin.bin" );
        /// </code>
        /// </example>
        /// <seealso cref="string"/>
        public void SaveDockState()
        {
            CleartargetNameinDockedMode();
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            windowcoll = new WindowParams(this);
            string content = this.SerializeElement(windowcoll);
            SaveIsolatedState(content);
            windowCollection = null;
        }

        /// <summary>
        /// Saves the state of the dock.
        /// </summary>
        /// <param name="content">The content.</param>
        public void SaveDockState(out string content)
        {
            CleartargetNameinDockedMode();
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            windowcoll = new WindowParams(this);
            content = string.Empty;
            content = this.SerializeElement(windowcoll);

            //SaveIsolatedState(content);
            windowCollection = null;
        }


        List<WindowItems> ContainerWindow = null;
        List<WindowItems> floatWindowCollection = null;


        /// <summary>
        /// Updates the float window.
        /// </summary>
        protected internal void UpdateFloatWindow()
        {
            IEnumerable<WindowItems> query = floatWindowCollection.OrderBy(tempwindow => ((WindowItems)tempwindow).ZindexForFloat);
            Window.currentZIndex = 0;
            for (int i = 0; i < query.Count(); i++)
            {
                Window w = GetWindow((query.ElementAt(i))._Caption);
                Canvas.SetZIndex(w, ++Window.currentZIndex);
                if (!base.Children.Contains(w))
                {
                    base.Children.Add(w);
                }
                w.ApplyBorderForFloatWindow();
            }
        }

        /// <summary>
        /// Moves to dock window as recursive manner.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="dm">The dm.</param>
        protected internal void MoveToDockWindowAsRecursiveManner(Window w, DockManager dm)
        {
            if (dm.gridDocking.GetOrderofGroup(w) <= 0 && w != null)
            {
                Window relativePane = null;
                if (w.MoveWindowTargetName == "ClientArea")
                {
                    relativePane = (dm.Children[0] as DockingGrid).rootWindow;
                }
                else
                {
                    relativePane = this.GetWindow(w.MoveWindowTargetName);
                }

                if (dm.gridDocking.GetOrderofGroup(relativePane) <= 0 && relativePane != null)
                {
                    MoveToDockWindowAsRecursiveManner(relativePane, dm);
                }
                if (relativePane != null)
                {
                    (dm.Children[0] as DockingGrid).MoveTo(w, relativePane, w.MoveDockPosition);
                }
                else
                {
                    dm.AddWindow(w);
                }
                ShowDockbutton(w);
                HideTabPanel(w);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected internal bool beforeInvoke = false;

        /// <summary>
        /// Retrieves the dockstate initially.
        /// </summary>
        /// <param name="savedState">State of the saved.</param>
        protected internal void RetrieveDockstateInitially(string savedState)
        {
            if (savedState != string.Empty && this.WindowCollection.Count > 0)
            {
                WindowParams windowparam = this.DeSerializeElement(savedState);
                if (this.m_bottomSideGrid != null)
                {
                    this.m_bottomSideGrid.Children.Clear();
                }
                if (this.m_leftSideGrid != null)
                {
                    this.m_leftSideGrid.Children.Clear();
                }
                if (this.m_topSideGrid != null)
                {
                    this.m_topSideGrid.Children.Clear();
                }
                if (this.m_rightSideGrid != null)
                {
                    this.m_rightSideGrid.Children.Clear();
                }
                //this.m_bottomSideGrid = null;
                //this.m_rightSideGrid = null;
                //this.m_leftSideGrid = null;
                //this.m_topSideGrid = null;
                tabNameCollection.Clear();
                btnPaneBottom.Children.Clear();
                btnPaneLeft.Children.Clear();
                btnPaneRight.Children.Clear();
                btnPaneTop.Children.Clear();
                List<UIElement> childrenCollection = new List<UIElement>();
                for (int i = base.Children.Count - 1; i >= 0; i--)
                {
                    if (base.Children[i].GetType() != typeof(DockManager))
                    {
                        if (base.Children[i] == m_leftSideGrid)
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                        else if (base.Children[i] == m_rightSideGrid)
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                        else if (base.Children[i] == m_topSideGrid)
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                        else if (base.Children[i] == m_bottomSideGrid)
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                        else if (base.Children[i].GetType() == typeof(Window))
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                    }
                }
                foreach (UIElement elem in childrenCollection)
                {
                    if (base.Children.Contains(elem))
                    {
                        base.Children.Remove(elem);
                    }
                }


                childrenCollection.Clear();
                IEnumerable<WindowItems> query = windowparam.WindowItems.OrderBy(tempwindow => ((WindowItems)tempwindow).Order);

                ContainerWindow = new List<WindowItems>();
                floatWindowCollection = new List<WindowItems>();
                List<Window> stateWindowCollection = new List<Window>();
                for (int j = 0; j < query.Count(); j++)
                {
                    Window window = GetWindow(query.ElementAt(j)._Caption);
                    if (window != null)
                    {
                        stateWindowCollection.Add(window);
                        window.InternalllyRaisedDockStateChanged = true;
                        if (query.ElementAt(j).ContainerName != string.Empty && query.ElementAt(j).ContainerName != null)
                        {
                            window.ContainerLeft = query.ElementAt(j).ContainerLeft;
                            window.ContainerTop = query.ElementAt(j).ContainerTop;
                            window.ContainerHeight = query.ElementAt(j).ContainerHeight;
                            window.ContainerWidth = query.ElementAt(j).ContainerWidth;
                        }
                        window.DockTabOrder = query.ElementAt(j).DockTabOrder;
                        window.FloatTabOrder = query.ElementAt(j).FloatTabOrder;
                        window.SelectedTabItem = query.ElementAt(j).SelectedTabItem;
                        window.ZindexOrder = query.ElementAt(j).Order;
                        window.LeftPosition = query.ElementAt(j).LeftPosition;
                        window.TopPosition = query.ElementAt(j).TopPosition;
                        window.DesiredWidthInFloatMode = query.ElementAt(j).DesiredWidthInFloatMode;
                        window.DesiredHeightInFloatMode = query.ElementAt(j).DesiredHeightInFloatMode;
                        window.FloatHeight = query.ElementAt(j).DesiredHeightInFloatMode;
                        window.FloatWidth = query.ElementAt(j).DesiredWidthInFloatMode;
                        window.CurrentStateMain = query.ElementAt(j).CurrentStateMain;
                        window.PreviousStateMain = query.ElementAt(j).PreviousStateMain;
                        window.AnimationHeight = query.ElementAt(j).AnimationHeight;
                        window.AnimationWidth = query.ElementAt(j).AnimationWidth;
                        window.CanAutoHide = query.ElementAt(j).CanAutoHide;
                        window.CanClose = query.ElementAt(j).CanClose;
                        window.CanDock = query.ElementAt(j).CanDock;
                        window.CanDrag = query.ElementAt(j).CanDrag;
                        window.CanFloat = query.ElementAt(j).CanFloat;
                        window.DockableState = query.ElementAt(j).DockableState;
                        //window.DockPosition = query.ElementAt(j).DockPosition;
                        window.InternalllyRaisedDockStateChanged = true;
                        if (query.ElementAt(j).DockState != DockState.AutoHidden)
                        {
                            window.DockState = query.ElementAt(j).DockState;
                            if (window.DockState == DockState.Dock)
                            {
                                if (window.dockToggle != null)
                                {
                                    if (window.DockingManager.ShowAwlButton && window.WindowChildElement != null && DockingManager.GetAwlButtonVisible(window.WindowChildElement))
                                    {
                                        window.dockToggle.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        window.dockToggle.Visibility = Visibility.Collapsed;
                                    }
                                }
                            }
                        }
                        else
                        {
                            window.DockState = DockState.Dock;
                        }
                        DockManager dockmanager = GetParentDockManager().Parent as DockManager;
                        window.Height = double.NaN;
                        window.Width = double.NaN;
                        window.DockManager = dockmanager;
                        window.PaneHeight = query.ElementAt(j).DesiredHeightInDockedMode;
                        window.PaneWidth = query.ElementAt(j).DesiredWidthInDockedMode;

                        if (query.ElementAt(j).DockState == DockState.Float)
                        {
                            if (query.ElementAt(j).ContainerName != string.Empty && query.ElementAt(j).ContainerName != null)
                            {
                                window.DockPosition = query.ElementAt(j).SideInFloatMode;
                            }
                            else
                            {
                                window.DockPosition = query.ElementAt(j).SideInDockedMode;
                            }
                        }
                        else
                        {
                            window.DockPosition = query.ElementAt(j).SideInDockedMode;
                        }

                        window.MoveWindowTargetName = query.ElementAt(j).MoveWindowTargetName;
                        window.MoveDockPosition = query.ElementAt(j).MoveDockPosition;
                        SetboolValueWithSideInMode(window, query.ElementAt(j).SideInDockedMode, DockState.Dock);
                        DockingManager.SetSideInDockedMode(window.WindowChildElement, query.ElementAt(j).SideInDockedMode);
                        SetboolValueWithSideInMode(window, query.ElementAt(j).SideInFloatMode, DockState.Float);
                        DockingManager.SetSideInFloatMode(window.WindowChildElement, query.ElementAt(j).SideInFloatMode);
                        SetboolValueWithTargetName(window, query.ElementAt(j).TargetNameInDockedMode, DockState.Dock);
                        DockingManager.SetTargetNameInDockedMode(window.WindowChildElement, query.ElementAt(j).TargetNameInDockedMode);
                        SetboolValueWithTargetName(window, query.ElementAt(j).TargetNameInDockedMode, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(window.WindowChildElement, query.ElementAt(j).TargetNameInFloatingMode);
                        window.InternalllyRaisedDockStateChanged = true;
                        DockingManager.SetDockState(window.WindowChildElement, query.ElementAt(j).DockState);
                        window.OldValueDockManager = null;
                    }
                }
                List<Window> windowCollection = new List<Window>(WindowCollection.Values);

                GenerateFloatWindowContainer(windowCollection, true);
                for (int j = 0; j < query.Count(); j++)
                {
                    Window window = GetWindow(query.ElementAt(j)._Caption);
                    if (window != null)
                    {
                        window.DockableState = query.ElementAt(j).DockableState;
                        //window.DockPosition = query.ElementAt(j).DockPosition;
                        window.InternalllyRaisedDockStateChanged = true;
                        if (query.ElementAt(j).DockState != DockState.AutoHidden)
                        {
                            window.DockState = query.ElementAt(j).DockState;
                        }
                        else
                        {
                            window.DockState = DockState.Dock;
                        }
                        DockManager dockmanager = GetParentDockManager().Parent as DockManager;
                        window.Height = double.NaN;
                        window.Width = double.NaN;
                        if (window.DockState != DockState.Float)
                        {
                            window.DockManager = dockmanager;
                        }
                        window.PaneHeight = query.ElementAt(j).DesiredHeightInDockedMode;
                        window.PaneWidth = query.ElementAt(j).DesiredWidthInDockedMode;
                        window.DockPosition = query.ElementAt(j).SideInDockedMode;
                        window.InternalllyRaisedDockStateChanged = true;
                        DockingManager.SetDockState(window.WindowChildElement, query.ElementAt(j).DockState);
                    }
                }
                IEnumerable<Window> windowquery = stateWindowCollection.Where(tempwindow => (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) == null));
                IEnumerable<Window> floatwindowquery = stateWindowCollection.Where(tempwindow => (DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) == null) && ((Window)tempwindow).DockState == DockState.Float).OrderBy(tempwindow => ((Window)tempwindow).ZindexOrder);

                foreach (Window w in floatwindowquery)
                {
                    w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                    if (w.DockState == DockState.Float)
                    {
                        IEnumerable<Window> floatwindowalone = stateWindowCollection.Where(tempwindow => (DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == w._Caption && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) != null) && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed && ((Window)tempwindow).DockState == DockState.Float).OrderBy(tempwindow => ((Window)tempwindow).ZindexOrder);
                        if (floatwindowalone.Count() <= 0)
                        {
                            if (base.Children.Contains(w))
                            {
                                base.Children.Remove(w);
                            }
                            w.Visibility = Visibility.Visible;
                            Canvas.SetLeft(w, w.LeftPosition);
                            Canvas.SetTop(w, w.TopPosition);
                            w.Height = w.DesiredHeightInFloatMode;
                            w.Width = w.DesiredWidthInFloatMode;
                            Canvas.SetZIndex(w, ++Window.currentZIndex);
                            if (!base.Children.Contains(w))
                            {
                                base.Children.Add(w);
                            }
                            RemoveDock(w);
                            HideTabPanel(w);
                            w.ApplyBorderForFloatWindow();
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Retrieves the state of the dock.
        /// </summary>
        /// <param name="savedState">State of the saved.</param>
        protected internal void RetrieveDockState(string savedState)
        {
            Window.currentZIndex = 3;


            if (savedState != string.Empty && this.WindowCollection.Count > 0)
            {
                WindowParams windowparam = this.DeSerializeElement(savedState);

                foreach (WindowItems windowItem in windowparam.WindowItems)
                {
                    bool isPresent = false;
                    foreach (Window w in this.WindowCollection.Values)
                    {
                        if (w.Caption == windowItem.Caption)
                        {
                            isPresent = true;
                        }
                    }

                    if (!isPresent)
                    {
                        if (InitializeControlOnLoad != null)
                        {
                            InitializeControlOnLoadEventArgs args = new InitializeControlOnLoadEventArgs(windowItem.Caption, windowItem.ElementType);
                            InitializeControlOnLoad(this, args);
                        }
                    }
                }

                if (this.m_bottomSideGrid != null)
                {
                    this.m_bottomSideGrid.Children.Clear();
                }
                if (this.m_leftSideGrid != null)
                {
                    this.m_leftSideGrid.Children.Clear();
                }
                if (this.m_topSideGrid != null)
                {
                    this.m_topSideGrid.Children.Clear();
                }
                if (this.m_rightSideGrid != null)
                {
                    this.m_rightSideGrid.Children.Clear();
                }
                //this.m_bottomSideGrid = null;
                //this.m_rightSideGrid = null;
                //this.m_leftSideGrid = null;
                //this.m_topSideGrid = null;
                tabNameCollection.Clear();
                btnPaneBottom.Children.Clear();
                btnPaneLeft.Children.Clear();
                btnPaneRight.Children.Clear();
                btnPaneTop.Children.Clear();
                List<UIElement> childrenCollection = new List<UIElement>();
                for (int i = base.Children.Count - 1; i >= 0; i--)
                {
                    if (base.Children[i].GetType() != typeof(DockManager))
                    {
                        if (base.Children[i] == m_leftSideGrid)
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                        else if (base.Children[i] == m_rightSideGrid)
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                        else if (base.Children[i] == m_topSideGrid)
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                        else if (base.Children[i] == m_bottomSideGrid)
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                        else if (base.Children[i].GetType() == typeof(Window))
                        {
                            //base.Children.Remove(base.Children[i]);
                            childrenCollection.Add(base.Children[i]);
                        }
                    }
                }
                foreach (UIElement elem in childrenCollection)
                {
                    if (base.Children.Contains(elem))
                    {
                        base.Children.Remove(elem);
                    }
                }


                childrenCollection.Clear();
                childrenCollection = base.Children.ToList();
                for (int i = 1, j = 0; i <= this.WindowCollection.Count; i++, j++)
                {
                    this.WindowCollection[i].MoveWindowTargetName = string.Empty;
                    if (this.WindowCollection[i].DockManager != null)
                    {
                        if (this.WindowCollection[i].DockManager.Parent is WindowContainer)
                        {
                            if (base.Children.Contains((this.WindowCollection[i].DockManager.Parent as WindowContainer)._window))
                            {
                                base.Children.Remove((this.WindowCollection[i].DockManager.Parent as WindowContainer)._window);
                            }
                        }

                        (this.WindowCollection[i].DockManager.Children[0] as DockingGrid).RemoveGroup(this.WindowCollection[i]);
                        (this.WindowCollection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
                    }
                    if (this.WindowCollection[i].OldValueDockManager != null)
                    {
                        if (this.WindowCollection[i].OldValueDockManager != this.WindowCollection[i].DockManager)
                        {
                            this.WindowCollection[i].DockManager = this.WindowCollection[i].OldValueDockManager;
                            (this.WindowCollection[i].DockManager.Children[0] as DockingGrid).RemoveGroup(this.WindowCollection[i]);
                            (this.WindowCollection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            if (this.WindowCollection[i].DockManager.Parent is WindowContainer)
                            {
                                if (base.Children.Contains((this.WindowCollection[i].DockManager.Parent as WindowContainer)._window))
                                {
                                    base.Children.Remove((this.WindowCollection[i].DockManager.Parent as WindowContainer)._window);
                                }
                            }
                        }
                    }
                    this.WindowCollection[i].Visibility = Visibility.Visible;
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int k = this.WindowCollection[i].CustomTabControl.Items.Count - 1; k >= 0; k--)
                        {
                            CustomTabItem cstab = (CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[k];
                            if (cstab.OwnWindow != null)
                            {
                                if (cstab.OwnWindow != this.WindowCollection[i])
                                {
                                    if (cstab.OwnWindow.CustomTabControl != null)
                                    {
                                        if (this.WindowCollection[i].CustomTabControl.Items.Contains(cstab))
                                        {
                                            this.WindowCollection[i].CustomTabControl.Items.Remove(cstab);
                                            cstab.OwnWindow.CustomTabControl.Items.Add(cstab);
                                        }
                                    }

                                }
                            }
                        }
                    }
                    HideTabPanel(this.WindowCollection[i]);
                }

                IEnumerable<WindowItems> query = windowparam.WindowItems.OrderBy(tempwindow => ((WindowItems)tempwindow).Order);

                ContainerWindow = new List<WindowItems>();
                floatWindowCollection = new List<WindowItems>();
                List<Window> stateWindowCollection = new List<Window>();
                for (int j = 0; j < query.Count(); j++)
                {
                    Window window = GetWindow(query.ElementAt(j)._Caption);
                    if (window != null)
                    {
                        stateWindowCollection.Add(window);
                        window.InternalllyRaisedDockStateChanged = true;
                        if (query.ElementAt(j).ContainerName != string.Empty && query.ElementAt(j).ContainerName != null)
                        {
                            window.ContainerLeft = query.ElementAt(j).ContainerLeft;
                            window.ContainerTop = query.ElementAt(j).ContainerTop;
                            window.ContainerHeight = query.ElementAt(j).ContainerHeight;
                            window.ContainerWidth = query.ElementAt(j).ContainerWidth;
                        }
                        window.DockTabOrder = query.ElementAt(j).DockTabOrder;
                        window.FloatTabOrder = query.ElementAt(j).FloatTabOrder;
                        window.SelectedTabItem = query.ElementAt(j).SelectedTabItem;
                        window.ZindexOrder = query.ElementAt(j).Order;
                        window.LeftPosition = query.ElementAt(j).LeftPosition;
                        window.TopPosition = query.ElementAt(j).TopPosition;
                        window.DesiredWidthInFloatMode = query.ElementAt(j).DesiredWidthInFloatMode;
                        window.DesiredHeightInFloatMode = query.ElementAt(j).DesiredHeightInFloatMode;
                        window.FloatHeight = query.ElementAt(j).DesiredHeightInFloatMode;
                        window.FloatWidth = query.ElementAt(j).DesiredWidthInFloatMode;
                        window.CurrentStateMain = query.ElementAt(j).CurrentStateMain;
                        window.PreviousStateMain = query.ElementAt(j).PreviousStateMain;
                        window.AnimationHeight = query.ElementAt(j).AnimationHeight;
                        window.AnimationWidth = query.ElementAt(j).AnimationWidth;
                        window.CanAutoHide = query.ElementAt(j).CanAutoHide;
                        window.CanClose = query.ElementAt(j).CanClose;
                        window.CanDock = query.ElementAt(j).CanDock;
                        window.CanDrag = query.ElementAt(j).CanDrag;
                        window.CanFloat = query.ElementAt(j).CanFloat;
                        window.DockableState = query.ElementAt(j).DockableState;
                        window.PreviousWidth = query.ElementAt(j).PreviousWidth;
                        window.PreviousHeight = query.ElementAt(j).PreviousHeight;
                        window.PreviousFloatHeight = query.ElementAt(j).PreviousFloatHeight;
                        window.PreviousFloatWidth = query.ElementAt(j).PreviousFloatWidth;
                        window.MaximizedState = query.ElementAt(j).MaximizedState;
                        window.PreviousLeftLocation = query.ElementAt(j).PreviousLeftLocation;
                        window.PreviousTopLocation = query.ElementAt(j).PreviousTopLocation;
                        //window.DockPosition = query.ElementAt(j).DockPosition;
                        window.InternalllyRaisedDockStateChanged = true;
                        if (query.ElementAt(j).DockState != DockState.AutoHidden)
                        {
                            window.DockState = query.ElementAt(j).DockState;
                            if (window.DockState == DockState.Dock)
                            {
                                if (window.dockToggle != null && window.WindowChildElement != null)
                                {
                                    if (window.DockingManager.ShowAwlButton && DockingManager.GetAwlButtonVisible(window.WindowChildElement))
                                    {
                                        window.dockToggle.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        window.dockToggle.Visibility = Visibility.Collapsed;
                                    }
                                }
                            }
                        }
                        else
                        {
                            window.DockState = DockState.Dock;
                        }
                        DockManager dockmanager = GetParentDockManager().Parent as DockManager;
                        window.Height = double.NaN;
                        window.Width = double.NaN;
                        window.DockManager = dockmanager;
                        window.PaneHeight = query.ElementAt(j).DesiredHeightInDockedMode;
                        window.PaneWidth = query.ElementAt(j).DesiredWidthInDockedMode;

                        if (query.ElementAt(j).DockState == DockState.Float)
                        {
                            if (query.ElementAt(j).ContainerName != string.Empty && query.ElementAt(j).ContainerName != null)
                            {
                                window.DockPosition = query.ElementAt(j).SideInFloatMode;
                            }
                            else
                            {
                                window.DockPosition = query.ElementAt(j).DockPosition;
                            }
                        }
                        else
                        {
                            window.DockPosition = query.ElementAt(j).DockPosition;
                        }

                        window.MoveWindowTargetName = query.ElementAt(j).MoveWindowTargetName;
                        window.MoveDockPosition = query.ElementAt(j).MoveDockPosition;
                        SetboolValueWithSideInMode(window, query.ElementAt(j).SideInDockedMode, DockState.Dock);
                        DockingManager.SetSideInDockedMode(window.WindowChildElement, query.ElementAt(j).SideInDockedMode);
                        SetboolValueWithSideInMode(window, query.ElementAt(j).SideInFloatMode, DockState.Float);
                        DockingManager.SetSideInFloatMode(window.WindowChildElement, query.ElementAt(j).SideInFloatMode);
                        SetboolValueWithTargetName(window, query.ElementAt(j).TargetNameInDockedMode, DockState.Dock);
                        DockingManager.SetTargetNameInDockedMode(window.WindowChildElement, query.ElementAt(j).TargetNameInDockedMode);
                        SetboolValueWithTargetName(window, query.ElementAt(j).TargetNameInDockedMode, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(window.WindowChildElement, query.ElementAt(j).TargetNameInFloatingMode);
                        window.InternalllyRaisedDockStateChanged = true;
                        DockingManager.SetDockState(window.WindowChildElement, query.ElementAt(j).DockState);
                        window.OldValueDockManager = null;
                    }
                }
                this._tabLoaded = false;

                tabNameCollection.Clear();
                btnPaneBottom.Children.Clear();
                btnPaneLeft.Children.Clear();
                btnPaneRight.Children.Clear();
                btnPaneTop.Children.Clear();

                IEnumerable<WindowItems> Containerquery = windowparam.WindowItems.Where(tempwindow => ((WindowItems)tempwindow).ContainerOrder > 0).OrderBy(tempwindow => ((WindowItems)tempwindow).ContainerOrder);

                for (int j = 0; j < Containerquery.Count(); j++)
                {
                    Window w = GetWindow(Containerquery.ElementAt(j)._Caption);
                    if (w != null)
                    {
                        if (Containerquery.ElementAt(j).ContainerHeight > 0 && Containerquery.ElementAt(j).ContainerName.Trim() != string.Empty && Containerquery.ElementAt(j).SideInDockedMode != Dock.Tabbed || Containerquery.ElementAt(j).SideInFloatMode != Dock.Tabbed)
                        {
                            //Add(GetWindow(query.ElementAt(j)._Caption).WindowChildElement);
                            List<Window> windowcollection = new List<Window>();
                            windowcollection.Add(GetWindow(Containerquery.ElementAt(j)._Caption));
                            //GenerateFloatWindowContainer(stateWindowCollection, false);
                            GenerateFloatWindowContainer(windowcollection, true);
                            if (windowcollection.Count > 0)
                            {
                                if (windowcollection[0].DockState == DockState.Float)
                                {
                                    windowcollection[0].Height = double.NaN;
                                    windowcollection[0].Width = double.NaN;
                                    windowcollection[0].PaneHeight = Containerquery.ElementAt(j).DesiredHeightInFloatMode;
                                    windowcollection[0].PaneWidth = Containerquery.ElementAt(j).DesiredWidthInFloatMode;

                                }
                            }
                            DockManager dockmanager = null;

                            if (w.OldValueDockManager != null)
                            {
                                if (w.OldValueDockManager.Parent is WindowContainer)
                                {
                                    dockmanager = w.OldValueDockManager;
                                }
                                else if (w.DockManager.Parent is WindowContainer)
                                {
                                    dockmanager = w.DockManager;
                                }
                            }
                            else if (w.DockManager.Parent is WindowContainer)
                            {
                                dockmanager = w.DockManager;
                            }
                            if (dockmanager != null)
                            {
                                Window _windowContainer = (dockmanager.Parent as WindowContainer)._window;
                                if (query.ElementAt(j).ContainerHeight > 0.0)
                                {
                                    Canvas.SetLeft(_windowContainer, Containerquery.ElementAt(j).ContainerLeft);
                                    Canvas.SetTop(_windowContainer, Containerquery.ElementAt(j).ContainerTop);
                                    _windowContainer.Width = Containerquery.ElementAt(j).ContainerWidth;
                                    _windowContainer.Height = Containerquery.ElementAt(j).ContainerHeight;
                                    Canvas.SetZIndex(_windowContainer, ++Window.currentZIndex);
                                }
                            }
                        }

                    }

                }

                for (int j = 0; j < query.Count(); j++)
                {
                    Window window = GetWindow(query.ElementAt(j)._Caption);
                    if (window != null)
                    {
                        window.DockableState = query.ElementAt(j).DockableState;
                        //window.DockPosition = query.ElementAt(j).DockPosition;
                        window.InternalllyRaisedDockStateChanged = true;
                        if (query.ElementAt(j).DockState != DockState.AutoHidden)
                        {
                            window.DockState = query.ElementAt(j).DockState;
                        }
                        else
                        {
                            window.DockState = DockState.Dock;
                        }
                        DockManager dockmanager = GetParentDockManager().Parent as DockManager;
                        window.Height = double.NaN;
                        window.Width = double.NaN;
                        if (window.DockState != DockState.Float)
                        {
                            window.DockManager = dockmanager;
                        }
                        window.PaneHeight = query.ElementAt(j).DesiredHeightInDockedMode;
                        window.PaneWidth = query.ElementAt(j).DesiredWidthInDockedMode;
                        window.DockPosition = query.ElementAt(j).DockPosition;
                        window.InternalllyRaisedDockStateChanged = true;
                        DockingManager.SetDockState(window.WindowChildElement, query.ElementAt(j).DockState);
                    }
                }



                IEnumerable<Window> windowquery = stateWindowCollection.Where(tempwindow => (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) == null));
                DockingGrid dockingGrid = GetParentDockManager();
                DockManager dm = null;
                if (dockingGrid != null && DockManager != null)
                {
                    dm = dockingGrid.Parent as DockManager;
                }

                foreach (Window w in windowquery)
                {
                    if (w.DockState != DockState.Float)
                    {
                        w.DockManager = dm;
                    }
                    if (dockingGrid.GetOrderofGroup(w) <= 0)
                    {
                        if (DockingManager.GetDockState(w.WindowChildElement) != DockState.AutoHidden)
                        {
                            w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                        }
                        (dm.Children[0] as DockingGrid).Add(w);

                        dm.AttachPaneEvents(w);
                        PreparePanel(w);
                    }
                    w.ApplyDockStyle();
                    w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                    if ((DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement) == string.Empty || w.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement)) == null) && DockingManager.GetDockState(w.WindowChildElement) == DockState.Float)
                    {
                        SetFloatWidthAndHeightToWindow(w);
                        Canvas.SetLeft(w, w.LeftPosition);
                        Canvas.SetTop(w, w.TopPosition);
                        Canvas.SetZIndex(w, ++Window.currentZIndex);
                        w.ApplyBorderForFloatWindow();
                        if (!((Canvas)w.DockingManager).Children.Contains(w))
                        {
                            w.DockManager.gridDocking.ArrangeLayout();
                            ((Canvas)w.DockingManager).Children.Add(w);
                        }
                        w.CurrentStateMain = StateMaintanance.Float;
                        w.PreviousStateMain = StateMaintanance.Dock;
                        if (w.dockToggle != null)
                        {
                            w.dockToggle.Visibility = Visibility.Collapsed;
                        }
                        w.Visibility = Visibility.Visible;
                    }
                }

                //var temp = from tempwindow in stateWindowCollection where DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) != null && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed && (((Window)tempwindow).DockState == DockState.Dock || ((Window)tempwindow).DockState == DockState.AutoHidden) group tempwindow by (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };

                var temp = from tempwindow in stateWindowCollection where DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) != null && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed group tempwindow by (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };
                {
                    foreach (var g in temp)
                    {
                        Window _w = GetWindow(g.windowKey.ToString());
                        //if (_w.DockState == DockState.Dock)
                        //{
                        bool ispaneGroupPresent = false;
                        int order = 0;
                        if (_w != null)
                        {
                            order = dockingGrid.GetOrderofGroup(_w);
                            if (order > 0)
                            {
                                ispaneGroupPresent = true;
                            }
                        }
                        foreach (var w in g.window)
                        {
                            if (w.Parent is Grid)
                            {
                                DockManager dockContainerManager = null;
                                bool isCurrentDockManager = false;
                                if (w.DockManager.Parent is WindowContainer)
                                {
                                    dockContainerManager = w.DockManager;
                                    isCurrentDockManager = true;
                                }
                                else if (w.OldValueDockManager != null)
                                {
                                    if (w.OldValueDockManager.Parent is WindowContainer)
                                    {
                                        dockContainerManager = w.OldValueDockManager;
                                    }
                                }
                                if (dockContainerManager != null)
                                {
                                    if (isCurrentDockManager)
                                    {
                                        DockManager swap = w.DockManager;
                                        w.OldValueDockManager = swap;
                                    }
                                    w.DockManager = dm;
                                    dockContainerManager.gridDocking.ArrangeLayout();
                                }
                            }
                            if (ispaneGroupPresent)
                            {
                                if (dockingGrid.GetOrderofGroup(w) <= 0)
                                {
                                    w.Visibility = Visibility.Visible;
                                    w.DockManager = dm;
                                    //w.DockState = DockState.Dock;

                                    //w.PaneHeight = DockingManager.GetDesiredHeightInDockedMode(w.WindowChildElement);
                                    //w.PaneWidth = DockingManager.GetDesiredWidthInDockedMode(w.WindowChildElement);
                                    dm.MoveTo(w, _w, DockingManager.GetSideInDockedMode(w.WindowChildElement));
                                    dm.AttachPaneEvents(w);
                                    w.ApplyDockStyle();
                                    w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                                    if (_w._Caption != string.Empty)
                                    {
                                        w.DockPosition = DockingManager.GetSideInDockedMode(GetExactParentWindowForNonTabDockedWindow(_w).WindowChildElement);//_w.DockPosition;
                                    }
                                    if (w.DockState == DockState.AutoHidden)
                                    {
                                        w.DockManager.gridDocking.ArrangeLayout();
                                    }
                                }
                            }
                            else
                            {
                                MoveDockWindowToTargetNameWindow(_w, w, dm);
                            }
                        }
                        //}
                    }
                }

                IEnumerable<Window> floatwindowquery = stateWindowCollection.Where(tempwindow => (DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) == null) && ((Window)tempwindow).DockState == DockState.Float).OrderBy(tempwindow => ((Window)tempwindow).ZindexOrder);

                foreach (var w in floatwindowquery)
                {
                    if (w.DockState != DockState.Float)
                        w.DockManager = dm;
                    if (dockingGrid.GetOrderofGroup(w) <= 0)
                    {
                        (dm.Children[0] as DockingGrid).Add(w);
                        dm.AttachPaneEvents(w);
                        PreparePanel(w);
                    }
                    w.ApplyDockStyle();
                    w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                    if (w.DockState == DockState.Float)
                    {
                        IEnumerable<Window> floatwindowalone = stateWindowCollection.Where(tempwindow => (DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == w._Caption 
                            && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) != null) 
                            && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed 
                            && ((Window)tempwindow).DockState == DockState.Float).OrderBy(tempwindow => ((Window)tempwindow).ZindexOrder);

                        if (floatwindowalone.Count() <= 0)
                        {
                            if (base.Children.Contains(w))
                            {
                                base.Children.Remove(w);
                            }
                            w.Visibility = Visibility.Visible;
                            Canvas.SetLeft(w, w.LeftPosition);
                            Canvas.SetTop(w, w.TopPosition);
                            w.Height = w.DesiredHeightInFloatMode;
                            w.Width = w.DesiredWidthInFloatMode;
                            Canvas.SetZIndex(w, ++Window.currentZIndex);
                            if (!base.Children.Contains(w))
                            {
                                base.Children.Add(w);
                            }
                            RemoveDock(w);
                            HideTabPanel(w);
                            w.ApplyBorderForFloatWindow();
                        }
                    }
                }

                var tempWindowGroup = stateWindowCollection.Where(
                    tempwindow =>
                    DockingManager.GetTargetNameInDockedMode(((Window) tempwindow).WindowChildElement) != string.Empty
                    &&
                    ((Window) tempwindow).DockingManager.GetWindow(
                        DockingManager.GetTargetNameInDockedMode(((Window) tempwindow).WindowChildElement)) != null
                    && DockingManager.GetSideInDockedMode(((Window) tempwindow).WindowChildElement) == Dock.Tabbed).
                    GroupBy(
                        tempwindow =>
                        (DockingManager.GetTargetNameInDockedMode(((Window) tempwindow).WindowChildElement))).Select(
                            Group => new {windowKey = Group.Key, window = Group});
                {
                    foreach (var key in tempWindowGroup)
                    {
                        var targetWin = GetWindow(key.windowKey);
                        if(targetWin != null && targetWin.DockState==DockState.AutoHidden)
                        {
                            if(tabNameCollection.Contains(targetWin._Caption))
                            {
                                tabNameCollection.Remove(targetWin._Caption);
                            }

                            var tempCollection = new List<SideButton>();

                            foreach (var w in key.window)
                            {
                                if (w.DockState == DockState.AutoHidden)
                                {
                                    if (tabNameCollection.Contains(w._Caption))
                                    {
                                        tabNameCollection.Remove(w._Caption);
                                    }
                                    foreach (var obj in btnPaneBottom.Children)
                                    {
                                        if (obj is SideButton)
                                        {
                                            var sideButton = obj as SideButton;
                                            if (sideButton.OwnWindow._Caption == w._Caption ||
                                                sideButton.OwnWindow._Caption == targetWin._Caption)
                                            {
                                                tempCollection.Add(sideButton);
                                            }
                                        }
                                    }

                                    foreach(var sideButton in tempCollection)
                                    {
                                        btnPaneBottom.Children.Remove(sideButton);
                                    }

                                    tempCollection.Clear();

                                    foreach (var obj in btnPaneLeft.Children)
                                    {
                                        if (obj is SideButton)
                                        {
                                            var sideButton = obj as SideButton;
                                            if (sideButton.OwnWindow._Caption == w._Caption ||
                                                sideButton.OwnWindow._Caption == targetWin._Caption)
                                            {
                                                tempCollection.Add(sideButton);
                                            }
                                        }
                                    }

                                    foreach (var sideButton in tempCollection)
                                    {
                                        btnPaneLeft.Children.Remove(sideButton);
                                    }

                                    tempCollection.Clear();

                                    foreach (var obj in btnPaneRight.Children)
                                    {
                                        if (obj is SideButton)
                                        {
                                            var sideButton = obj as SideButton;
                                            if (sideButton.OwnWindow._Caption == w._Caption ||
                                                sideButton.OwnWindow._Caption == targetWin._Caption)
                                            {
                                                tempCollection.Add(sideButton);
                                            }
                                        }
                                    }

                                    foreach (var sideButton in tempCollection)
                                    {
                                        btnPaneRight.Children.Remove(sideButton);
                                    }

                                    tempCollection.Clear();

                                    foreach (var obj in btnPaneTop.Children)
                                    {
                                        if (obj is SideButton)
                                        {
                                            var sideButton = obj as SideButton;
                                            if (sideButton.OwnWindow._Caption == w._Caption ||
                                                sideButton.OwnWindow._Caption == targetWin._Caption)
                                            {
                                                tempCollection.Add(sideButton);
                                            }
                                        }
                                    }

                                    foreach (var sideButton in tempCollection)
                                    {
                                        btnPaneTop.Children.Remove(sideButton);
                                    }

                                    tempCollection.Clear();
                                }
                            }
                        }
                    }
                }

                TabCreationwithLINQ(stateWindowCollection);
                IEnumerable<Window> tabWindowquery = stateWindowCollection.Where(tempwindow => ((Window)tempwindow).CustomTabControl.Items.Count > 1);//(DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed || DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed));                 
                foreach (Window _w in tabWindowquery)
                {
                    List<object> tabCollection = _w.CustomTabControl.Items.ToList();
                    IEnumerable<object> tabWindowOrder;
                    if (_w.DockState != DockState.Float)
                    {
                        tabWindowOrder = tabCollection.OrderBy(tempwindow => (((CustomTabItem)tempwindow).OwnWindow).DockTabOrder);
                    }
                    else
                    {
                        tabWindowOrder = tabCollection.OrderBy(tempwindow => (((CustomTabItem)tempwindow).OwnWindow).FloatTabOrder);
                    }    
                    foreach (CustomTabItem cstabItem in tabWindowOrder)
                    {
                        if (cstabItem.OwnWindow._Caption.ToString().ToLower() == _w.SelectedTabItem.ToString().ToLower())
                        {
                            custab = cstabItem;
                        }
                        _w.CustomTabControl.Items.Remove(cstabItem);
                        if (_w.DockState != DockState.Float)
                        {
                            _w.CustomTabControl.Items.Insert(cstabItem.OwnWindow.DockTabOrder, cstabItem);
                        }
                        else
                        {
                            _w.CustomTabControl.Items.Insert(cstabItem.OwnWindow.FloatTabOrder, cstabItem);
                        }
                    }
                    if (custab != null)
                    {
                        _w.CustomTabControl.SelectedItem = custab;
                        _w.Caption = DockingManager.GetHeader(custab.OwnWindow.WindowChildElement);
                    }

                }

                IEnumerable<Window> tabWindowContainerquery = stateWindowCollection.Where(tempwindow => ((Window)tempwindow).DockManager.Parent.GetType() == typeof(WindowContainer));//(DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed || DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed));
                IEnumerable<Window> tabWindowContainerforOldDockManager = stateWindowCollection.Where(tempwindow => ((Window)tempwindow).OldValueDockManager != null).Where(tempwindow => (((Window)tempwindow).OldValueDockManager.Parent.GetType() == typeof(WindowContainer)));
                foreach (Window tabWindow in tabWindowContainerforOldDockManager)
                {
                    IEnumerable<Window> tabWindowCollectionquery = stateWindowCollection.Where(tempwindow => DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed && DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == tabWindow._Caption);
                    foreach (Window _w in tabWindowCollectionquery)
                    {
                        DockManager dockManager = null;
                        if (tabWindow.DockManager.Parent is WindowContainer)
                        {
                            dockManager = tabWindow.DockManager;
                        }
                        else if (tabWindow.OldValueDockManager != null)
                        {
                            if (tabWindow.OldValueDockManager.Parent is WindowContainer)
                            {
                                dockManager = tabWindow.OldValueDockManager;
                            }
                        }
                        if (dockManager != null)
                        {
                            if (!(dockManager.Parent as WindowContainer)._window.WindowCollection.Contains(_w))
                            {
                                (dockManager.Parent as WindowContainer)._window.WindowCollection.Add(_w);
                            }
                        }
                    }
                }
                foreach (Window tabWindow in tabWindowContainerquery)
                {
                    IEnumerable<Window> tabWindowCollectionquery = stateWindowCollection.Where(tempwindow => DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed && DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == tabWindow._Caption);
                    foreach (Window _w in tabWindowCollectionquery)
                    {
                        DockManager dockManager = null;
                        if (tabWindow.DockManager.Parent is WindowContainer)
                        {
                            dockManager = tabWindow.DockManager;
                        }
                        else if (tabWindow.OldValueDockManager != null)
                        {
                            if (tabWindow.OldValueDockManager.Parent is WindowContainer)
                            {
                                dockManager = tabWindow.OldValueDockManager;
                            }
                        }
                        if (dockManager != null)
                        {
                            if (!(dockManager.Parent as WindowContainer)._window.WindowCollection.Contains(_w))
                            {
                                (dockManager.Parent as WindowContainer)._window.WindowCollection.Add(_w);
                            }
                        }
                    }
                }

                foreach (Window tabWindow in tabWindowquery)
                {

                    foreach (CustomTabItem csTabItem in tabWindow.CustomTabControl.Items)
                    {
                        csTabItem.OwnWindow.DockManager = tabWindow.DockManager;
                        csTabItem.OwnWindow.OldValueDockManager = tabWindow.OldValueDockManager;
                        DockManager dockManager = null;
                        if (tabWindow.DockManager.Parent is WindowContainer)
                        {
                            dockManager = tabWindow.DockManager;
                        }
                        else if (tabWindow.OldValueDockManager != null)
                        {
                            if (tabWindow.OldValueDockManager.Parent is WindowContainer)
                            {
                                dockManager = tabWindow.OldValueDockManager;
                            }
                        }
                        if (dockManager != null)
                        {
                            if ((dockManager.Parent as WindowContainer).handledLater && csTabItem.OwnWindow != tabWindow)
                            {
                                (dockManager.Parent as WindowContainer).ExternalWindow.Add(csTabItem.OwnWindow);
                            }
                            else
                            {
                                if (!(dockManager.Parent as WindowContainer)._window.WindowCollection.Contains(csTabItem.OwnWindow))
                                {
                                    (dockManager.Parent as WindowContainer)._window.WindowCollection.Add(csTabItem.OwnWindow);
                                }
                            }
                        }
                    }
                }

                // dm.gridDocking.ArrangeLayout();
                UpdateSidePanelLayout();

                List<UIElement> sideButtonCollection;
                if (btnPaneLeft != null)
                {
                    sideButtonCollection = btnPaneLeft.Children.ToList();

                    btnPaneLeft.Children.Clear();

                    foreach (string windowName in windowparam.SideButtonLeftOrder)
                    {
                        IEnumerable<UIElement> sideButtons =
                            sideButtonCollection.Where(button => ((SideButton) button).OwnWindow._Caption == windowName);
                        if (sideButtons.Count() > 0)
                            btnPaneLeft.Children.Add(sideButtons.ElementAt(0));
                    }

                    sideButtonCollection.Clear();
                }

                if (btnPaneRight != null)
                {
                    sideButtonCollection = btnPaneRight.Children.ToList();

                    btnPaneRight.Children.Clear();

                    foreach (string windowName in windowparam.SideButtomRightOrder)
                    {
                        IEnumerable<UIElement> sideButtons =
                            sideButtonCollection.Where(button => ((SideButton) button).OwnWindow._Caption == windowName);
                        if (sideButtons.Count() > 0 )
                            btnPaneRight.Children.Add(sideButtons.ElementAt(0));
                    }

                    sideButtonCollection.Clear();
                }

                if (btnPaneTop != null)
                {
                    sideButtonCollection = btnPaneTop.Children.ToList();

                    btnPaneTop.Children.Clear();

                    foreach (string windowName in windowparam.SideButtonTopOrder)
                    {
                        IEnumerable<UIElement> sideButtons =
                            sideButtonCollection.Where(button => ((SideButton) button).OwnWindow._Caption == windowName);
                        if (sideButtons.Count() > 0 )
                            btnPaneTop.Children.Add(sideButtons.ElementAt(0));
                    }

                    sideButtonCollection.Clear();
                }

                if (btnPaneBottom != null)
                {
                    sideButtonCollection = btnPaneBottom.Children.ToList();

                    btnPaneBottom.Children.Clear();

                    foreach (string windowName in windowparam.SideButtonBottomOrder)
                    {
                        IEnumerable<UIElement> sideButtons =
                            sideButtonCollection.Where(button => ((SideButton) button).OwnWindow._Caption == windowName);
                        if (sideButtons.Count() > 0)
                            btnPaneBottom.Children.Add(sideButtons.ElementAt(0));
                    }

                    sideButtonCollection.Clear();
                }
                if (!string.IsNullOrEmpty(windowparam.ActiveWindowName))
                {
                    var getActiveWindow =
                        stateWindowCollection.Where(tempwindow => tempwindow._Caption == windowparam.ActiveWindowName);
                    if (getActiveWindow.Count() > 0)
                        this.ActiveWindow = getActiveWindow.ElementAt(0);
                }

                IEnumerable<Window> windowCollection = stateWindowCollection.Where(tempwindow => tempwindow.MaximizedState == MaximizedState.Maximized);

                foreach (Window w in windowCollection)
                {
                    if ((w.MaximizedState == MaximizedState.Maximized) && w.maximizeButton != null && !(bool)w.maximizeButton.IsChecked)
                    {
                        w.InternallyChecked = true;
                        w.maximizeButton.IsChecked = true;
                        w.InternallyChecked = false;
                    }
                }

            }           
        }

        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        public void LoadDockState()
        {
            ChildrenCount = base.Children.Count;
            _savedState = string.Empty;
            string savedState = this.ReadIsolatedState();
            if (this.WindowCollection.Count >= 1)
            {
                tempwindow = this.WindowCollection[1] as Window;
            }
            if (tempwindow != null)
            {
                //if (!tempwindow.WindowLoaded)
                //{
                //    //RetrieveDockState(savedState);
                //    RetrieveDockstateInitially(savedState);
                //}
                //else
                //{
                //RetrieveDockState(savedState);
                //}
                if (savedState == string.Empty)
                {
                    this.Children.Clear();
                    this.Clear();
                }
                else
                {
                    RetrieveDockState(savedState);
                }
            }

        }
        private string _savedState = string.Empty;

        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        /// <param name="savedState">State of the saved.</param>
        public void LoadDockState(string savedState)
        {
            ChildrenCount = base.Children.Count;
            _savedState = savedState;
            if (this.WindowCollection.Count >= 1)
            {
                tempwindow = this.WindowCollection[1] as Window;
            }
            if (tempwindow != null)
            {
                //if (!tempwindow.WindowLoaded)
                //{
                //    RetrieveDockState(savedState);//RetrieveDockstateInitially(savedState);
                //}
                //else
                //{
                 //   RetrieveDockState(savedState);
                //}
                if (savedState == string.Empty)
                {
                    this.Children.Clear();
                    this.Clear();
                }
                else
                {
                    RetrieveDockState(savedState);
                }
            }
        }

        /// <summary>
        /// Tabs the creation from XML.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void TabCreationFromXml(WindowParams w)
        {
            for (int i = 0; i < w.WindowItems.Count; i++)
            {
                Window source = GetWindow(w.WindowItems[i]._Caption);
                Window targetWindow = null;
                if (w.WindowItems[i].TargetNameInDockedMode != string.Empty)
                {
                    targetWindow = GetWindow(w.WindowItems[i].TargetNameInDockedMode);
                }
                else
                {
                    targetWindow = GetWindow(w.WindowItems[i].TarGetNameInFloatMode);
                }
                if (targetWindow != null && targetWindow != source)
                {
                    if (targetWindow.CustomTabControl != null)
                    {
                        if (source.CustomTabControl.Items.Count == 1)
                        {
                            CustomTabItem cstabitem = source.CustomTabControl.Items[0] as CustomTabItem;
                            if (source.CustomTabControl.Items.Contains(cstabitem))
                            {
                                source.CustomTabControl.Items.Remove(cstabitem);
                            }
                            if (!targetWindow.CustomTabControl.Items.Contains(cstabitem))
                            {
                                targetWindow.CustomTabControl.Items.Add(cstabitem);
                            }
                            w.WindowItems[i].DockState = DockState.Hidden;
                        }

                    }
                }
                else if (targetWindow == null)
                {
                    //   w.WindowItems[i].DockState = DockState.Hidden;
                }
            }

        }



        /// <summary>
        /// Updates the window container collection from XML.
        /// </summary>
        /// <param name="relativePane">The relative pane.</param>
        /// <param name="dm">The dm.</param>
        /// <param name="windowContainer">The window container.</param>
        protected internal void UpdateWindowContainerCollectionFromXml(Window relativePane, DockManager dm, Window windowContainer)
        {
            if (relativePane.CustomTabControl != null)
            {
                for (int i = 0; i < relativePane.CustomTabControl.Items.Count; i++)
                {
                    CustomTabItem cstab = relativePane.CustomTabControl.Items[i] as CustomTabItem;
                    if (cstab != null)
                    {
                        Window _w = cstab.OwnWindow;
                        if (_w != null && _w != relativePane)
                        {
                            if (!windowContainer.WindowCollection.Contains(_w))
                            {
                                windowContainer.WindowCollection.Add(_w);
                            }
                            _w.DockManager = dm;
                        }
                    }
                }
            }


        }

        /// <summary>
        /// Updates the dock manager for tab item.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="windowcontaier">The windowcontaier.</param>
        private void UpdateDockManagerForTabItem(Window window, Window windowcontaier)
        {

            if (window.CustomTabControl != null && window.DockState == DockState.Float)
            {
                for (int i = 0; i < window.CustomTabControl.Items.Count; i++)
                {
                    CustomTabItem custab = window.CustomTabControl.Items[i] as CustomTabItem;
                    if (custab.OwnWindow != null)
                    {
                        custab.OwnWindow.DockManager = window.DockManager;
                        custab.OwnWindow.OldValueDockManager = window.OldValueDockManager;
                        if (!windowcontaier.WindowCollection.Contains(custab.OwnWindow))
                        {
                            windowcontaier.WindowCollection.Add(custab.OwnWindow);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates the window container from load.
        /// </summary>
        /// <param name="dm1">The DM1.</param>
        /// <param name="windowContainer">The window container.</param>
        /// <param name="_group">The _group.</param>
        protected internal void GenerateWindowContainerFromLoad(DockManager dm1, Window windowContainer, int _group)
        {
            DockingGrid dockingGrid = GetParentDockManager();
            DockManager parentDockManager = null;
            if (dockingGrid != null)
            {
                parentDockManager = dockingGrid._dockManager;
            }
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            if (dm1.gridDocking.gridDocking != null)
            {
                if (dm1.Parent is WindowContainer)
                {
                    if ((dm1.Parent as WindowContainer).ExternalWindow.Count > 0)
                    {
                        windowCollection.Clear();
                        windowCollection = (dm1.Parent as WindowContainer).ExternalWindow;
                    }
                }
                IEnumerable<Window> floatWindowalone = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == windowContainer.WindowCollection[0]._Caption);

                if (floatWindowalone.Count() > 0)
                {
                    Window w = floatWindowalone.ElementAt(0);
                    Window _w = windowContainer.WindowCollection[0];
                    if (_w == w && floatWindowalone.Count() > 1)
                    {
                        w = floatWindowalone.ElementAt(1);
                    }
                    if (_w.DockManager == dm1 || _w.OldValueDockManager == dm1 || (dm1.Parent as WindowContainer).DockManager == dm1)
                    {
                        if (dockingGrid != null)
                        {
                            parentDockManager = dockingGrid._dockManager;
                        }
                        if (dm1.gridDocking.GetOrderofGroup(_w) <= 0)
                        {
                            if (!windowContainer.WindowCollection.Contains(_w))
                            {
                                windowContainer.WindowCollection.Add(_w);
                            }
                            if (!windowContainer.WindowCollection.Contains(w))
                            {
                                windowContainer.WindowCollection.Add(w);
                            }
                            // _w.DockState = DockState.Dock;
                            _w.DockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                            if (!(dm1.Parent as WindowContainer).handledLater)
                            {
                                _w.DockManager = dm1;
                                _w.OldValueDockManager = parentDockManager;
                                _w.Height = double.NaN;
                                _w.Width = double.NaN;
                            }
                            else if (_w.DockState == DockState.Float && w.DockState == DockState.Float)
                            {
                                _w.DockManager = dm1;
                                _w.OldValueDockManager = parentDockManager;
                                _w.Height = double.NaN;
                                _w.Width = double.NaN;
                            }
                            else
                            {
                                _w.OldValueDockManager = dm1;
                                w.OldValueDockManager = dm1;
                            }
                            dm1.gridDocking.Add(_w);
                            dm1.AttachPaneEvents(_w);
                            RemoveDock(_w);
                            //w.DockState = DockState.Dock;
                            w.DockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                            if (!(dm1.Parent as WindowContainer).handledLater)
                            {
                                w.DockManager = dm1;
                                w.Height = double.NaN;
                                w.Width = double.NaN;
                                w.OldValueDockManager = parentDockManager;
                            }
                            else if (_w.DockState == DockState.Float && w.DockState == DockState.Float)
                            {
                                w.DockManager = dm1;
                                w.OldValueDockManager = parentDockManager;
                                w.Height = double.NaN;
                                w.Width = double.NaN;
                            }
                            dm1.gridDocking.Add(w);
                            dm1.AttachPaneEvents(w);
                            RemoveDock(w);
                            w.MoveWindowTargetName = _w._Caption;
                            w.MoveDockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                            w.StoredMoveToWindow = _w;
                            w.MovetToDockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                            HideTabPanel(_w);
                            RemoveDock(_w);
                            UpdateDockManagerForTabItem(_w, windowContainer);
                            UpdateDockManagerForTabItem(w, windowContainer);
                            _w.ApplyDockStyle();
                            w.ApplyDockStyle();
                            if (parentDockManager.gridDocking.GetOrderofGroup(w) >= 0 && w.DockState == DockState.Dock && !(dm1.Parent as WindowContainer).handledLater)
                            {
                                if (_w.DockState == DockState.Float)
                                {
                                    _w.Width = double.NaN;
                                    _w.Height = double.NaN;
                                    if (!base.Children.Contains(_w))
                                    {
                                        SetFloatWidthAndHeightToWindow(_w);
                                        base.Children.Add(_w);
                                    }
                                    (dm1.Parent as WindowContainer).OnApply = true;
                                }
                                DockManager swap = dm1;
                                w.DockManager = parentDockManager;
                                w.OldValueDockManager = dm1;
                                //w.StateTrans();
                                _w.DockManager = parentDockManager;
                                _w.OldValueDockManager = dm1;
                                (dm1.Parent as WindowContainer).OnApply = true;

                                //windowContainer.Visibility = Visibility.Collapsed;
                            }
                            if (parentDockManager.gridDocking.GetOrderofGroup(_w) >= 0 && _w.DockState == DockState.Dock && !(dm1.Parent as WindowContainer).handledLater)
                            {
                                //_w.StateTrans();
                                w.Width = double.NaN;
                                w.Height = double.NaN;
                                if (w.DockState == DockState.Float)
                                {
                                    if (!base.Children.Contains(w))
                                    {
                                        base.Children.Add(w);
                                        SetFloatWidthAndHeightToWindow(w);
                                    }
                                    (dm1.Parent as WindowContainer).OnApply = true;
                                }
                                w.DockManager = parentDockManager;
                                w.OldValueDockManager = dm1;
                                _w.DockManager = parentDockManager;
                                _w.OldValueDockManager = dm1;
                                //_w.StateTrans();
                                //windowContainer.Visibility = Visibility.Collapsed;
                            }

                        }
                    }
                }

                //IEnumerable<Window> floatWindowcollection = windowCollection.Where(tempwindow => ((Window)tempwindow).DockState == DockState.Float);
                var floatWindowColl = from tempwindow in windowCollection where DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) != null && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed group tempwindow by (DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };
                {
                    #region
                    //foreach (var g in floatWindowColl)
                    //{
                    //    Window _w = GetWindow(g.windowKey);
                    //    if (_w != null)
                    //    {
                    //        if (_w.DockManager == dm1)
                    //        {
                    //            int count = 0;
                    //            DockingGrid dockingGrid = GetParentDockManager();
                    //            DockManager parentDockManager = null;
                    //            if(dockingGrid != null)
                    //            {
                    //                parentDockManager = dockingGrid._dockManager;
                    //            }
                    //            foreach (Window w in g.window)
                    //            {
                    //                if (count == 0)
                    //                {
                    //                    if (dm1.gridDocking.GetOrderofGroup(_w) <= 0)
                    //                    {
                    //                        if (!windowContainer.WindowCollection.Contains(_w))
                    //                        {
                    //                            windowContainer.WindowCollection.Add(_w);
                    //                        }
                    //                        if (!windowContainer.WindowCollection.Contains(w))
                    //                        {
                    //                            windowContainer.WindowCollection.Add(w);
                    //                        }
                    //                        _w.DockState = DockState.Dock;
                    //                        _w.DockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                    //                        _w.DockManager = dm1;
                    //                        _w.OldValueDockManager = parentDockManager;
                    //                        _w.Height = double.NaN;
                    //                        _w.Width = double.NaN;
                    //                        dm1.gridDocking.Add(_w);
                    //                        dm1.AttachPaneEvents(_w);
                    //                        RemoveDock(_w);
                    //                        w.DockState = DockState.Dock;
                    //                        w.DockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                    //                        w.DockManager = dm1;
                    //                        w.Height = double.NaN;
                    //                        w.Width = double.NaN;
                    //                        dm1.gridDocking.Add(w);
                    //                        dm1.AttachPaneEvents(w);
                    //                        RemoveDock(w);
                    //                        w.StoredMoveToWindow = _w;
                    //                        w.MovetToDockPosition = _w.DockPosition;
                    //                        w.OldValueDockManager = parentDockManager;
                    //                        HideTabPanel(_w);
                    //                        RemoveDock(_w);
                    //                        count++;
                    //                        UpdateDockManagerForTabItem(_w);
                    //                        UpdateDockManagerForTabItem(w);
                    //                    }
                    //                    else
                    //                    {
                    //                        if (!windowContainer.WindowCollection.Contains(w))
                    //                        {
                    //                            windowContainer.WindowCollection.Add(w);
                    //                        }
                    //                        w.Height = double.NaN;
                    //                        w.Width = double.NaN;
                    //                        w.DockState = DockState.Dock;
                    //                        Window relativeWindow = _w;
                    //                        w.DockManager = dm1;
                    //                        Dock dockPosition = w.DockPosition;
                    //                        dm1.gridDocking.MoveTo(w, relativeWindow, w.DockPosition);
                    //                        RemoveDock(w);
                    //                        count++;
                    //                        w.StoredMoveToWindow = relativeWindow;
                    //                        w.MovetToDockPosition = dockPosition;
                    //                        w.OldValueDockManager = parentDockManager;
                    //                        w.DockPosition = dockPosition;
                    //                        w.MoveWindowTargetName = _w._Caption;
                    //                        w.MoveDockPosition = dockPosition;
                    //                        UpdateDockManagerForTabItem(w);
                    //                        count++;
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    if (!windowContainer.WindowCollection.Contains(w))
                    //                    {
                    //                        windowContainer.WindowCollection.Add(w);
                    //                    }
                    //                    w.Height = double.NaN;
                    //                    w.Width = double.NaN;
                    //                    w.DockState = DockState.Dock;
                    //                    Window relativeWindow = g.window.ElementAt(count - 1);
                    //                    w.DockManager = dm1;
                    //                    Dock dockPosition = w.DockPosition;
                    //                    dm1.gridDocking.MoveTo(w, relativeWindow, w.DockPosition);
                    //                    RemoveDock(w);
                    //                    count++;
                    //                    w.StoredMoveToWindow = relativeWindow;
                    //                    w.MovetToDockPosition = dockPosition;
                    //                    w.OldValueDockManager = parentDockManager;
                    //                    w.DockPosition = dockPosition;
                    //                    w.MoveWindowTargetName = _w._Caption;
                    //                    w.MoveDockPosition = dockPosition;
                    //                    count++;
                    //                    UpdateDockManagerForTabItem(w);
                    //                    //  UpdateWindowContainerCollectionFromXml(_w, dm1, windowContainer);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    foreach (var g in floatWindowColl)
                    {
                        Window _w = GetWindow(g.windowKey.ToString());
                        if (_w != null)
                        {
                            if (_w.DockManager == dm1 || _w.OldValueDockManager == dm1)
                            {
                                int count = 0;
                                if (dockingGrid != null)
                                {
                                    parentDockManager = dockingGrid._dockManager;
                                    bool ispaneGroupPresent = false;
                                    int order = 0;
                                    if (_w != null)
                                    {
                                        order = dm1.gridDocking.GetOrderofGroup(_w);
                                        if (order > 0)
                                        {
                                            ispaneGroupPresent = true;
                                        }
                                    }
                                    foreach (var w in g.window)
                                    {
                                        if (ispaneGroupPresent)
                                        {
                                            if (dm1.gridDocking.GetOrderofGroup(w) <= 0)
                                            {
                                                if (!windowContainer.WindowCollection.Contains(w))
                                                {
                                                    windowContainer.WindowCollection.Add(w);
                                                }
                                                w.Height = double.NaN;
                                                w.Width = double.NaN;
                                                //w.DockState = DockState.Dock;
                                                Window relativeWindow = _w;
                                                if (DockingManager.GetDockState(w.WindowChildElement) == DockState.Float)
                                                {
                                                    w.DockManager = dm1;
                                                    w.OldValueDockManager = parentDockManager;
                                                }
                                                else
                                                {
                                                    w.DockManager = parentDockManager;
                                                    w.OldValueDockManager = dm1;
                                                }
                                                Dock dockPosition = w.DockPosition;
                                                if (DockingManager.GetDockState(w.WindowChildElement) == DockState.Float)
                                                {
                                                    dockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                                                }
                                                //w.PaneHeight = DockingManager.GetDesiredHeightInDockedMode(w.WindowChildElement);
                                                //w.PaneWidth = DockingManager.GetDesiredWidthInDockedMode(w.WindowChildElement);
                                                dm1.gridDocking.MoveTo(w, relativeWindow, DockingManager.GetSideInFloatMode(w.WindowChildElement));
                                                RemoveDock(w);
                                                count++;
                                                w.MoveWindowTargetName = relativeWindow._Caption;
                                                w.MoveDockPosition = dockPosition;
                                                w.StoredMoveToWindow = relativeWindow;
                                                w.MovetToDockPosition = dockPosition;
                                                w.DockPosition = dockPosition;
                                                w.MoveWindowTargetName = _w._Caption;
                                                w.MoveDockPosition = dockPosition;
                                                UpdateDockManagerForTabItem(w, windowContainer);
                                                w.ApplyDockStyle();
                                                if (w.DockState == DockState.Float && relativeWindow.DockState == DockState.Float && base.Children.Contains(relativeWindow))
                                                {
                                                    //relativeWindow.DockManager = dm1;
                                                    //relativeWindow.OldValueDockManager = parentDockManager;
                                                    //relativeWindow.Height = double.NaN;
                                                    //relativeWindow.Width = double.NaN;
                                                    //dm1.gridDocking.ArrangeLayout();
                                                }
                                                if (parentDockManager.gridDocking.GetOrderofGroup(w) >= 0 && w.DockState == DockState.Dock)
                                                {
                                                    //_w.StateTrans();  
                                                    w.Width = double.NaN;
                                                    w.Height = double.NaN;
                                                    if (w.DockState == DockState.Float)
                                                    {
                                                        SetFloatWidthAndHeightToWindow(w);
                                                        if (!base.Children.Contains(w))
                                                        {
                                                            base.Children.Add(w);
                                                        }
                                                    }
                                                    w.DockManager = parentDockManager;
                                                    w.OldValueDockManager = dm1;

                                                }
                                            }
                                            else
                                            {
                                                if (DockingManager.GetDockState(w.WindowChildElement) == DockState.Float && base.Children.Contains(w) && g.window.Count() > 1)
                                                {
                                                    Window windowCon = (dm1.Parent as WindowContainer)._window;
                                                    //base.Children.Remove(w);
                                                    //w.Height = double.NaN;
                                                    //w.Width = double.NaN;
                                                    //w.DockManager = dm1;
                                                    //w.OldValueDockManager = parentDockManager;
                                                    //w.ApplyDockStyle();
                                                    //windowCon.Visibility = Visibility.Visible;
                                                    w.ApplyBorderForFloatWindow();
                                                }
                                                else
                                                {
                                                    //if(_w.DockState ==DockState.Dock || 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MoveFloatWindowToTargetNameWindow(_w, w, dm1, windowContainer, parentDockManager);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (dm1.Parent is WindowContainer)
                {
                    (dm1.Parent as WindowContainer).handledLater = true;
                }
            }

            Window windowContain = (dm1.Parent as WindowContainer)._window;
            int numberofFloatWindow = windowContain.NumberofFloatChildren(windowContain, dm1);
            int numberofChildren = windowContain.NumberofChildren(windowContain, dm1);
            if (numberofFloatWindow == 1 && windowContain.WindowCollection.Count > 2)
            {
                windowContain.Visibility = Visibility.Visible;

                windowContain.CheckChildrenPresent(windowContain, windowContain.WindowContainer.DockManager);
                Window _w = null;
                _w = windowContain.CheckFloatWindowIsPresent(windowContain, windowContain.WindowContainer.DockManager);
                if (_w != null)
                {
                    if (_w.DockState == DockState.Float && (_w.OldValueDockManager == dm1 || _w.DockManager == dm1))
                    {
                        _w.DockManager = dm1;
                        _w.OldValueDockManager = parentDockManager;
                        _w.Height = double.NaN;
                        _w.Width = double.NaN;
                        _w.ApplyDockStyle();
                        dm1.gridDocking.ArrangeLayout();
                    }
                }

            }
            else if (numberofChildren > 1)
            {
                windowContain.WindowContainer.OnApply = false;
                windowContain.Visibility = Visibility.Visible;
            }
            foreach (Window _w in windowCollection)
            {
                if (_w.DockState == DockState.Dock)
                {
                    _w.Width = double.NaN;
                    _w.Height = double.NaN;
                }
            }
            if (parentDockManager.gridDocking.gridDocking != null)
            {
                parentDockManager.gridDocking.ArrangeLayout();
            }
        }

        /// <summary>
        /// Generates the window container from XML.
        /// </summary>
        /// <param name="dm1">The DM1.</param>
        /// <param name="windowContainer">The window container.</param>
        /// <param name="_group">The _group.</param>
        protected internal void GenerateWindowContainerFromXML(DockManager dm1, Window windowContainer, int _group)
        {
            if (ContainerWindow != null)
            {
                var query1 = from element in ContainerWindow group element by ((WindowItems)element).ContainerName into Group select new { windowitemsKey = Group.Key, windowitems = Group };
                _group = _group - 1;
                var group = query1.ElementAt(_group);
                int count = 0;
                foreach (var witems in group.windowitems)
                {
                    List<Window> windowCollection = new List<Window>(WindowCollection.Values);
                    IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => ((Window)tempwindow)._Caption == witems._Caption);
                    if (windowquery.Count() > 0)
                    {
                        Window _w = windowquery.ElementAt(0);
                        if (_w != null)
                        {
                            if (!windowContainer.WindowCollection.Contains(_w))
                            {
                                windowContainer.WindowCollection.Add(_w);
                            }
                            if (witems.DockState != DockState.Hidden)
                            {
                                if (count < 2)
                                {
                                    _w.PaneHeight = witems.DesiredHeightInDockedMode; _w.DockState = witems.DockState;
                                    _w.PaneWidth = witems.DesiredWidthInDockedMode; _w.DockPosition = witems.DockPosition;
                                    _w.DockManager = dm1;
                                    _w.Height = double.NaN;
                                    _w.Width = double.NaN;
                                    dm1.gridDocking.Add(_w);
                                    dm1.AttachPaneEvents(_w);
                                    //UpdateWindowContainerCollectionFromXml(_w, dm1, windowContainer);

                                    if (count == 1)
                                    {
                                        _w.StoredMoveToWindow = GetWindow(group.windowitems.ElementAt(0)._Caption);
                                        _w.MovetToDockPosition = _w.DockPosition;
                                    }
                                    HideTabPanel(_w);
                                    RemoveDock(_w);
                                    count++;
                                }
                                else
                                {
                                    _w.Height = double.NaN;
                                    _w.Width = double.NaN;
                                    _w.PaneHeight = witems.DesiredHeightInDockedMode; _w.DockState = witems.DockState;
                                    _w.PaneWidth = witems.DesiredWidthInDockedMode; _w.DockPosition = witems.DockPosition;
                                    Window relativeWindow = GetWindow(witems.MoveWindowTargetName);
                                    if (relativeWindow != null)
                                    {
                                        _w.StoredMoveToWindow = relativeWindow;
                                        _w.MovetToDockPosition = witems.MoveDockPosition;
                                        _w.DockManager = dm1;
                                        dm1.gridDocking.MoveTo(_w, relativeWindow, witems.MoveDockPosition);
                                        RemoveDock(_w);
                                    }
                                    //  UpdateWindowContainerCollectionFromXml(_w, dm1, windowContainer);
                                }
                            }
                            else
                            {
                                _w.DockManager = dm1;
                                //UpdateWindowContainerCollectionFromXml(_w, dm1, windowContainer);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates the window container.
        /// </summary>
        /// <param name="relativeWindow">The relative window.</param>
        /// <param name="group">The group.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        protected internal DockManager GenerateWindowContainer(Window relativeWindow, int group, double left, double top, double height, double width)
        {
            Window w = new Window();
            //w.Name = "Container " + _name++.ToString();
            w.Background = WindowBackground;
            if (base.Children.Contains(relativeWindow))
            {
                base.Children.Remove(relativeWindow);
            }
            WindowContainer _windowContainer = new WindowContainer();
            _windowContainer.ContainerName = "Container " + _name++.ToString();
            _windowContainer._group = group;
            _windowContainer.DockingManager = this;
            w.DockingManager = this;
            w.WindowContainer = _windowContainer;
            _windowContainer._window = w;
            w.DockState = relativeWindow.DockState;
            w.CanDock = true;
            w.CanDrag = true;
            w.CanFloat = true;
            w.CanClose = true;
            w.DraggingEnabled = true;
            w.containerInvoke = true;
            //(relativeWindow.DockManager.Children[0] as DockingGrid).Remove(relativeWindow);
            DockManager dm = new DockManager(true, relativeWindow);
            dm.DockingParent = this;
            _windowContainer.Children.Add(dm);
            _windowContainer.DockManager = dm;
            relativeWindow.DockManager = dm;
            PreparePanel(w);
            //w.DockManager = dm;
            w.HeaderBackgroud = HeaderBackground;
            dm.Margin = new Thickness(2, 1, 2, 0);
            Canvas.SetLeft(w, left);
            Canvas.SetTop(w, top);
            w.Width = width;
            w.Height = height;
            Canvas.SetZIndex(w, ++Window.currentZIndex);
            if (!w.WindowCollection.Contains(relativeWindow))
            {
                w.WindowCollection.Add(relativeWindow);
            }
            base.Children.Add(w);
            return dm;
        }

        /// <summary>
        /// Serializes the element.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        private string SerializeElement(object obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(WindowParams));
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            xs.Serialize(writer, obj);
            writer.Close();
            return sb.ToString();
        }

        /// <summary>
        /// Serializes the dock manager element.
        /// </summary>
        protected internal void SerializeDockManagerElement()
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream oStream = new IsolatedStorageFileStream("tt.txt", FileMode.Create, isoStore))
                {

                }
            }
        }


        /// <summary>
        /// Saves the state of the isolated.
        /// </summary>
        /// <param name="strData">The STR data.</param>
        private void SaveIsolatedState(string strData)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(@"WindowsParams.xml", FileMode.Create, isoStore))
                {
                    using (StreamWriter writer = new StreamWriter(isoStream))
                    {
                        writer.Write(strData);
                    }
                }
            }
        }


        /// <summary>
        /// Reads the state of the isolated.
        /// </summary>
        /// <returns></returns>
        private string ReadIsolatedState()
        {
            string content = string.Empty;
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(@"WindowsParams.xml", FileMode.OpenOrCreate, isoStore))
                {
                    using (StreamReader sr = new StreamReader(isoStream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }
            return content;
        }


        /// <summary>
        /// Deserialize element.
        /// </summary>
        /// <param name="strSerializeObject">The STR serialize object.</param>
        /// <returns></returns>
        private WindowParams DeSerializeElement(string strSerializeObject)
        {
            XmlSerializer xs = new XmlSerializer(typeof(WindowParams));
            StringReader sr = new StringReader(strSerializeObject);
            WindowParams w;
            w = (WindowParams)xs.Deserialize(sr);

            return w;
        }

    }
}
