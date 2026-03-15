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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Linq;
namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Docking Manager Class.
    /// </summary>
    public class DockManager : Grid
    {
        /// <summary>
        /// Represents whether Window Container is true or false.
        /// </summary>
        internal bool _windowContainer = false;

        /// <summary>
        /// Represents the root group window.
        /// </summary>
        protected internal Window rootGroup = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockManager"/> class.
        /// </summary>
        /// <param name="presentContainer">If set to <c>true</c> [present container].</param>
        /// <param name="_rootGroup">The _root group.</param>
        public DockManager(bool presentContainer, Window _rootGroup)
        {
            ////this.DefaultStyleKey = typeof(DockingGrid);
            _windowContainer = presentContainer;
            rootGroup = _rootGroup;            
            gridDocking = new DockingGrid();
            gridDocking.AttachDockManager(this);
            this.Children.Add(gridDocking);
            this.SizeChanged += new SizeChangedEventHandler(DockManager_SizeChanged);
        }

        void DockManager_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (Window w in this.DockingParent.WindowCollection.Values)
            {
                if (w.WindowChildElement != null && DockingManager.GetMaximizedState(w.WindowChildElement) == MaximizedState.Maximized)
                {
                    if (w.maximizeButton != null)
                        w.maximizeButton.IsChecked = true;
                    break;
                }

                if (w.DockState == DockState.AutoHidden)
                {
                    if (w.DockPosition == Dock.Bottom || w.DockPosition == Dock.Top)
                    {
                        w.Width = e.NewSize.Width;
                       
                    }
                    else if (w.DockPosition == Dock.Left || w.DockPosition == Dock.Right)
                    {
                        w.Height = e.NewSize.Height;
                    }
                }
               
            }          
          
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockManager"/> class.
        /// </summary>
        public DockManager()
        {

        }

        /// <summary>
        /// Serializes to json string.
        /// </summary>
        /// <param name="ob">The ob.</param>
        /// <returns>The string.</returns>
        protected internal static string SerializeToJsonString(object ob)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer ser = new DataContractSerializer(ob.GetType());
                ser.WriteObject(ms, ob);
                ms.Position = 0;
                using (StreamReader reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Gets or sets the docking parent.
        /// </summary>
        /// <value>The docking parent.</value>
        protected internal DockingManager DockingParent
        {
            get;
            set;
        }

        /// <summary>
        /// Handles the loaded event of the DockManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void DockManager_loaded(object sender, RoutedEventArgs e)
        {           
        }

        /// <summary>
        /// Attaches the pane events.
        /// </summary>
        /// <param name="pane">The pane.</param>
        internal void AttachPaneEvents(Window pane)
        {
            pane.OnStateChanged += new EventHandler(pane_OnStateChanged);
            gridDocking.AttachPaneEvents(pane);
        }

        /// <summary>
        /// Detach pane events handler.
        /// </summary>
        /// <param name="pane">Window object.</param>
        internal void DetachPaneEvents(Window pane)
        {
            pane.OnStateChanged -= new EventHandler(pane_OnStateChanged);
            gridDocking.DetachPaneEvents(pane);
        }

        /// <summary>
        /// Handles the OnStateChanged event of the pane control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void pane_OnStateChanged(object sender, EventArgs e)
        {
            Window pane = sender as Window;            
        }

        /// <summary>
        /// Handle dockable pane layout changing.
        /// </summary>
        /// <param name="sourcePane">Source pane to move.</param>
        /// <param name="destinationPane">Relative pane.</param>
        /// <param name="relativeDock">Dock Position.</param>
        internal void MoveTo(Window sourcePane, Window destinationPane, Dock relativeDock)
        {
            gridDocking.MoveTo(sourcePane, destinationPane, relativeDock);
        }

        /// <summary>
        /// Represents the Collection Window.
        /// </summary>
        List<Window> windowColletion = new List<Window>();

        /// <summary>
        /// Represents the Docking grid.
        /// </summary>
       protected internal DockingGrid gridDocking = null;

       /// <summary>
       /// Adds the window.
       /// </summary>
       /// <param name="window">The window.</param>
        protected internal void AddWindow(Window window)
        {
            window.DockManager = this;
            gridDocking.Add(window);
            AttachPaneEvents(window);
        }

        /// <summary>
        /// Adds the window collection.
        /// </summary>
        /// <param name="window">The window.</param>
        protected internal void AddWindowCollection(Window window)
        {
            if (window._Caption != string.Empty && !windowColletion.Contains(window))
            {
                windowColletion.Add(window);
            }
        }

        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="_window">The _window.</param>
        /// <param name="relativepane">The relativepane.</param>
        protected internal void AddElement(Window _window, Window relativepane)
        {
            if (gridDocking.rootWindow != _window)
            {
                _window.Visibility = Visibility.Visible;
                _window.DockManager = this;
                gridDocking.AddDockablePaneGroup(_window, relativepane);
                AttachPaneEvents(_window);
            }
        }

        /// <summary>
        /// Identifies whether OnApply is true or false.
        /// </summary>
        protected internal bool onApply = false;

        /// <summary>
        /// Identifies whether On Apply template finish is true or false.
        /// </summary>
        protected internal bool IsOnApplyeTemplatefinish = false;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            
            IsOnApplyeTemplatefinish = true;
            DockingParent.DockManager = this;
            if (DockingParent.DockFill)
            {
                if (gridDocking.rootWindow != null)
                {
                    gridDocking.rootWindow.DockState = DockState.Hidden;
                }
            }
            if (this.Parent is DockingManager)
            {
                List<Window> windowCollection = new List<Window>(DockingParent.WindowCollection.Values);
                foreach (Window w in windowCollection)
                {
                    AttachPaneEvents(w);
                    if (w.DockManager == null)
                    {
                        w.DockManager = this;                        
                    }
                    if (!onApply)
                    {
                        gridDocking.Add(w);
                        onApply = true;
                        if (DockingParent.GetWindow(DockingManager.GetTargetNameInDockedMode(w.WindowChildElement)) != null && DockingManager.GetTargetNameInDockedMode(w.WindowChildElement) != string.Empty)
                        {
                            gridDocking.RemoveGroup(w);
                        }
                    }
                }
                List<Window> floatWindowCollection = new List<Window>();
                IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) == null));
                IEnumerable<Window> floatWindowcollection = windowCollection.Where(tempwindow => ((Window)tempwindow).DockState == DockState.Float);
                foreach (Window w in floatWindowcollection)
                {
                    if (!floatWindowCollection.Contains(w))
                    {
                        floatWindowCollection.Add(w);
                    }
                }
                for (int ii = 0; ii < windowquery.Count(); ii++)
                {
                    if (DockingParent.DockFill)
                    {
                        if (ii == 0)
                        {
                            gridDocking.rootWindow.DockState = DockState.Hidden;
                        }                        
                        windowquery.ElementAt(ii).Visibility = Visibility.Visible;
                        //windowquery.ElementAt(ii).DockManager = this;
                        if (gridDocking.GetOrderofGroup(windowquery.ElementAt(ii)) <= 0)
                        {
                            gridDocking.Add(windowquery.ElementAt(ii));
                        }
                        AttachPaneEvents(windowquery.ElementAt(ii));
                        windowquery.ElementAt(ii).ApplyDockStyle();
                    }
                    else
                    {
                        if (ii == 0)
                        {
                            gridDocking.rootWindow.DockState = DockState.Dock;
                        }
                        windowquery.ElementAt(ii).Visibility = Visibility.Visible;
                        if (gridDocking.GetOrderofGroup(windowquery.ElementAt(ii)) <= 0)
                        {
                            gridDocking.Add(windowquery.ElementAt(ii));
                        }
                        AttachPaneEvents(windowquery.ElementAt(ii));
                        windowquery.ElementAt(ii).ApplyDockStyle();
                    }
                }

                // windowquery = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) != null && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed).GroupBy; 
                //                                                                                                                                                                                                                                                                                                                                                                                              && ((Window)tempwindow).DockState == DockState.Dock 
                var temp = from tempwindow in windowCollection where DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) != null && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed  group tempwindow by (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };
                {
                    foreach (var g in temp)
                    {
                        Window _w = DockingParent.GetWindow(g.windowKey.ToString());
                        //if (_w.DockState == DockState.Dock)
                        //{
                            bool ispaneGroupPresent = false;
                            int order = 0;
                            if (_w != null)
                            {
                                order = gridDocking.GetOrderofGroup(_w);
                                if (order > 0)
                                {
                                    ispaneGroupPresent = true;
                                }
                            }
                            foreach (var w in g.window)
                            {
                                if (ispaneGroupPresent)
                                {
                                    if (gridDocking.GetOrderofGroup(w) <= 0)
                                    {
                                        w.Visibility = Visibility.Visible;
                                        w.DockManager = this;
                                        DockState ds = DockingManager.GetDockState(w.WindowChildElement);
                                        //w.DockState = DockState.Dock;
                                        w.PaneHeight = DockingManager.GetDesiredHeightInDockedMode(w.WindowChildElement);
                                        w.PaneWidth = DockingManager.GetDesiredWidthInDockedMode(w.WindowChildElement);
                                        if (g.windowKey.ToString() == "ClientArea")
                                        {
                                           // w.DockManager.onApply = true; 
                                        }
                                        MoveTo(w, _w, DockingManager.GetSideInDockedMode(w.WindowChildElement));
                                        AttachPaneEvents(w);
                                        w.ApplyDockStyle();
                                        w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                                        w.DockingManager.HideTabPanel(w);
                                        //if (ds == DockState.Hidden)
                                        //{
                                        //    w.DockState = ds;
                                        //    w.DockingManager.ArrangeLayoutWhenStateIsHidden(w);
                                        //}
                                        //else if (ds == DockState.AutoHidden)
                                        //{
                                        //    w.DockState = DockState.AutoHidden;
                                        //    if (w.DockManager.gridDocking != null)
                                        //    {
                                        //        w.DockManager.gridDocking.ArrangeLayout();
                                        //    }
                                        //}

                                    }
                                }
                                else
                                {
                                    DockingParent.MoveDockWindowToTargetNameWindow(_w, w, this);
                                }
                            }
                        //}
                       
                    }
                }

                IEnumerable<Window> floatwindowquery = floatWindowCollection.Where(tempwindow => ((DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) == null) && DockingManager.GetDockState(((Window)tempwindow).WindowChildElement) == DockState.Float));
                    for (int ii = 0; ii < floatwindowquery.Count(); ii++)
                    {
                        if (floatwindowquery.ElementAt(ii).CustomTabControl != null)
                        {
                            foreach (CustomTabItem cstab in floatwindowquery.ElementAt(ii).CustomTabControl.Items)
                            {
                                if (cstab.OwnWindow != null && cstab.OwnWindow.DockManager == null)
                                {
                                    cstab.OwnWindow.DockManager = this;
                                }
                            }
                        }
                        if (((Canvas)floatwindowquery.ElementAt(ii).DockingManager).Children.Contains(floatwindowquery.ElementAt(ii)))
                        {                           
                            if (floatwindowquery.ElementAt(ii).CustomTabControl.Items.Count > 0)
                            {
                                floatwindowquery.ElementAt(ii).Visibility = Visibility.Visible;
                                floatwindowquery.ElementAt(ii)._leftValue = 0.0;
                                floatwindowquery.ElementAt(ii)._topValue = 0.0;
                                floatwindowquery.ElementAt(ii).GetParent((UIElement)floatwindowquery.ElementAt(ii).Parent);
                                //floatwindowquery.ElementAt(ii).Width = 200;
                                //floatwindowquery.ElementAt(ii).Height = 200;
                                //floatwindowquery.ElementAt(ii).FloatHeight = 200;
                                //floatwindowquery.ElementAt(ii).FloatWidth = 200;
                                //Canvas.SetLeft(floatwindowquery.ElementAt(ii), floatwindowquery.ElementAt(ii)._leftValue);
                                //Canvas.SetTop(floatwindowquery.ElementAt(ii), floatwindowquery.ElementAt(ii)._topValue);
                                floatwindowquery.ElementAt(ii).DockingManager.SetFloatWidthAndHeightToWindow(floatwindowquery.ElementAt(ii));
                                Canvas.SetZIndex(floatwindowquery.ElementAt(ii), ++Window.currentZIndex);
                                floatwindowquery.ElementAt(ii).UpdateFloatSize();
                                floatwindowquery.ElementAt(ii).ApplyBorderForFloatWindow();
                                if (!((Canvas)floatwindowquery.ElementAt(ii).DockingManager).Children.Contains(floatwindowquery.ElementAt(ii)))
                                {
                                    ((Canvas)floatwindowquery.ElementAt(ii).DockingManager).Children.Add(floatwindowquery.ElementAt(ii));
                                }
                                floatwindowquery.ElementAt(ii).CurrentStateMain = StateMaintanance.Float;
                                floatwindowquery.ElementAt(ii).PreviousStateMain = StateMaintanance.Dock;
                                if (floatwindowquery.ElementAt(ii).dockToggle != null)
                                {
                                    floatwindowquery.ElementAt(ii).dockToggle.Visibility = Visibility.Collapsed;
                                }
                            }
                            else
                            {
                                // floatWindowCollection[ii].DockingManager.Children.Remove(floatWindowCollection[ii]);
                            }
                        }
                    }
                    for (int i = 1; i <= this.DockingParent.WindowCollection.Count; i++)
                    {
                        if (this.DockingParent.WindowCollection[i].tempGrid != null)
                        {
                            this.DockingParent.WindowCollection[i].tempGrid.Visibility = Visibility.Visible;
                        }
                    }
                    this.DockingParent.UpdateSidePanelLayout();
                    if (windowCollection.Count == 0)
                    {
                        gridDocking.ArrangeLayout();
                    }
            }
            else
            {
                if (this.Parent is WindowContainer)
                {
                    WindowContainer windowcontain = this.Parent as WindowContainer;
                    if (!windowcontain._window.containerInvoke)
                    {
                        if (DockingParent._tarGetWindow != null)
                        {
                            if (DockingParent._tarGetWindow._Caption == string.Empty)
                            {
                                //DockManager oldValue = DockingParent.mouseHoveredWindow.DockManager;
                                //DockingParent.mouseHoveredWindow.DockManager = this;
                                //DockingParent.mouseHoveredWindow.OldValueDockManager = oldValue;
                                //DockingParent.mouseHoveredWindow.DockManager = this;
                                gridDocking.Add(DockingParent.mouseHoveredWindow);
                                AttachPaneEvents(DockingParent.mouseHoveredWindow);
                                DockingParent.AddWindowContainerToDock();
                            }
                            else
                            {
                                for (int ii = 0; ii < windowColletion.Count; ii++)
                                {
                                    //DockManager oldValue = windowColletion[ii].DockManager;
                                    //windowColletion[ii].DockManager = this;
                                    //windowColletion[ii].OldValueDockManager = oldValue;
                                    gridDocking.Add(windowColletion[ii]);
                                    AttachPaneEvents(windowColletion[ii]);
                                    windowColletion[ii].Visibility = Visibility.Visible;

                                }
                            }
                        }
                        else
                        {
                            for (int ii = 0; ii < windowColletion.Count; ii++)
                            {
                                DockManager oldValue = windowColletion[ii].DockManager;
                                windowColletion[ii].DockManager = this;
                                windowColletion[ii].OldValueDockManager = oldValue;
                                gridDocking.Add(windowColletion[ii]);
                                AttachPaneEvents(windowColletion[ii]);
                            }
                        }
                    }
                    else
                    {
                        if (windowcontain._group == -1 && windowcontain.handledLater)
                        {
                            windowcontain._window.DockingManager.GenerateWindowContainerFromLoad(this, windowcontain._window, windowcontain._group);
                            windowcontain.handledLater = false;
                            if (windowcontain.OnApply)
                            {
                                windowcontain._window.Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            windowcontain._window.DockingManager.GenerateWindowContainerFromXML(this, windowcontain._window, windowcontain._group);
                        }
                    }
                }
            }

          //  onApply = false;
            IsOnApplyeTemplatefinish = false;
        }


        /// <summary>
        /// Changes the order.
        /// </summary>
        /// <param name="source">The Source Window for which the order to be changed.</param>
        /// <param name="dest">The dest Window.</param>
        public void ChangeOrder(Window source, Window dest)
        {
            if (this.Parent is DockingManager || this.Parent.GetType() == typeof(WindowContainer))
            {
                RemoveAllPane();
                int sourceIndex = 0;
                int desIndex = 0;
                if (source.dockToggle.Visibility == Visibility.Visible && source.DockState != DockState.Float && source.DockingManager.ShowAwlButton && source.WindowChildElement!= null && DockingManager.GetAwlButtonVisible(source.WindowChildElement))
                {
                    dest.dockToggle.Visibility = Visibility.Visible;
                }
                else
                {
                    dest.dockToggle.Visibility = Visibility.Collapsed;
                }
                for (int ii = 1; ii <= DockingParent.WindowCollection.Count; ii++)
                {  
                  
                    Window temp = null;
                    if (DockingParent.WindowCollection[ii] == source)
                    {
                        sourceIndex = ii;
                        temp = dest;
                        temp.Height = double.NaN;
                        temp.Width = double.NaN;
                        temp.DockState = DockState.Dock;
                        temp.DockPosition = source.DockPosition;
                        temp.DockManager = this;
                        if (source.ActualHeight != 0 || source.ActualWidth != 0)
                        {
                            temp.PaneHeight = source.ActualHeight;
                            temp.PaneWidth = source.ActualWidth;
                        }

                        gridDocking.Add(temp);
                        AttachPaneEvents(temp);
                    }
                    else if (DockingParent.WindowCollection[ii] == dest)
                    {
                        desIndex = ii;
                        temp = source;                       
                        temp.DockState = DockState.Hidden;
                        ////temp.DockPosition = dest.DockPosition;
                        temp.DockManager = this;
                        gridDocking.Add(temp);
                        AttachPaneEvents(temp);
                    }
                    else
                    {
                        UIElement ele = (UIElement)DockingParent.WindowCollection[ii].Parent;
                        if (DockingParent.WindowCollection[ii].Parent == null)
                        {
                            temp = DockingParent.WindowCollection[ii];
                            temp.Height = double.NaN;
                            temp.Width = double.NaN;
                            temp.DockManager = this;
                            gridDocking.Add(temp);
                            AttachPaneEvents(temp);
                        } 
                    }                    
                }

                if (sourceIndex > 0)
                {
                    DockingParent.WindowCollection[sourceIndex] = dest;
                }

                if (desIndex > 0)
                {
                    DockingParent.WindowCollection[desIndex] = source;
                }

            }
        }

        /// <summary>
        /// Removes all pane.
        /// </summary>
        public void RemoveAllPane()
        {
            for (int ii = 1; ii <= DockingParent.WindowCollection.Count; ii++)
            {
                gridDocking.Remove(DockingParent.WindowCollection[ii]);
            }
            gridDocking.gridDocking.Children.Clear();
        }
    }
}
