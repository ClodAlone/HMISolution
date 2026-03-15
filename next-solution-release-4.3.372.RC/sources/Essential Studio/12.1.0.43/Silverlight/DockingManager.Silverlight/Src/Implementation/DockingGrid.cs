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

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Docking Grid Class.
    /// </summary>
    public class DockingGrid : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockingGrid"/> class.
        /// </summary>
        public DockingGrid()
        {
            this.DefaultStyleKey = typeof(DockingGrid);
            colletion = new List<Window>();
        }

        /// <summary>
        /// Represents the Docking Grid.
        /// </summary>
        protected internal Grid gridDocking = null;

        /// <summary>
        /// Represents the Dock manager.
        /// </summary>
        protected internal DockManager _dockManager = null;

        /// <summary>
        /// Represents the root window.
        /// </summary>
        protected internal Window rootWindow = null;

        /// <summary>
        /// Represents the Root Window.
        /// </summary>
        protected internal Window rootedWindow = null;

        /// <summary>
        /// Attaches the dock manager.
        /// </summary>
        /// <param name="temp">The temp.</param>
        internal void AttachDockManager(DockManager temp)
        {
            if (!temp._windowContainer)
            {
                Window pane = new Window();
                _rootGroup = new DockablePaneGroup(pane, this);
                rootedWindow = pane;
                rootWindow = pane;                
                pane.DockManager = temp;
                _rootGroup.rootGrid = this;                
                //pane._Caption = "ClientArea";
               
            }
            else
            {
                Window pane = new Window();
                _rootGroup = new DockablePaneGroup(pane, this);
                rootedWindow = pane;
                pane.DockManager = temp;
                pane.DockState = DockState.Hidden;
                pane.PaneHeight = 0;
                pane.PaneWidth = 0;
                _rootGroup.rootGrid = this;

               //// _rootGroup = new DockablePaneGroup(temp.rootGroup);
            }

            this._dockManager = temp;
            //// ArrangeLayout();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            gridDocking = GetTemplateChild("PART_LayOuter") as Grid;
                _dockManager.OnApplyTemplate();
        }

        /// <summary>
        /// Represents the Root Group.
        /// </summary>
        DockablePaneGroup _rootGroup = null;

        /// <summary>
        /// Gets the pane group position.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns>The Dock.</returns>
        protected internal Dock GetPaneGroupPosition(Window pane)
        {
            DockablePaneGroup dg = GetPaneGroup(pane);
            if (dg != null)
            {
                return dg.ParentGroup.Dock;
            }
            else
            {
                return pane.DockPosition;
            }

        }

        /// <summary>
        /// Adds the dockable pane group.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="relativePane">The relative pane.</param>
        public void AddDockablePaneGroup(Window pane,Window relativePane)
        {
            DockablePaneGroup dg = GetPaneGroup(relativePane);
            if (dg != null)
            {
                DockingGrid dg1 = _rootGroup.rootGrid;
                _rootGroup = dg.AddPane(pane);
                _rootGroup.rootGrid = dg1;
                ArrangeLayout();
            }
            else
            {
                Add(pane);
            }
        }

        /// <summary>
        /// Adds the specified pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void Add(Window pane)
        {
            if (_rootGroup != null)
            {
                DockingGrid dg = _rootGroup.rootGrid;
                _rootGroup = _rootGroup.AddPane(pane);                
                _rootGroup.rootGrid = dg;
                ArrangeLayout();
            }
        }

        /// <summary>
        /// Arranges the layout by client grid.
        /// </summary>
        internal void ArrangeLayoutByClientGrid()
        {
            Clear(rootWindow.Parent as Grid);
            DockablePaneGroup dg = GetPaneGroup(rootWindow);
            if (rootWindow.Parent is Grid)
            {
                dg.Arrange(rootWindow.Parent as Grid , this);
            }
            //_rootGroup.Arrange(rootWindow.Parent as Grid, this);
        }

        /// <summary>
        /// Arrangings the order.
        /// </summary>
        internal void ArrangingOrder()
        {
            if (_rootGroup != null)
            {
                _rootGroup.ArrangeOrder(gridDocking, this);
            }
        }

        /// <summary>
        /// Arranges the layout.
        /// </summary>
        internal void ArrangeLayout()
        {
            ////_rootGroup.SaveChildPanesSize();
            if (_rootGroup != null)
            {
                Clear(gridDocking);
                _rootGroup.Arrange(gridDocking, this);
                Dump(_rootGroup, 0);
            }            
        }

        /// <summary>
        /// Attaches the pane events.
        /// </summary>
        /// <param name="pane">The pane.</param>
        internal void AttachPaneEvents(Window pane)
        {
            pane.OnStateChanged += new EventHandler(pane_OnStateChanged);
            pane.OnDockChanged += new EventHandler(pane_OnDockChanged);
        }

        /// <summary>
        /// Detaches the pane events.
        /// </summary>
        /// <param name="pane">The pane.</param>
        internal void DetachPaneEvents(Window pane)
        {
            pane.OnStateChanged -= new EventHandler(pane_OnStateChanged);
            pane.OnDockChanged -= new EventHandler(pane_OnDockChanged);
        }

        /// <summary>
        /// Handles the OnDockChanged event of the pane control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void pane_OnDockChanged(object sender, EventArgs e)
        {
            Window pane = sender as Window;
            Remove(pane);
            Add(pane);
        }

        /// <summary>
        /// Removes the group.
        /// </summary>
        /// <param name="pane">The pane.</param>
        protected internal void RemoveGroup(Window pane)
        {
            if (_rootGroup != null)
            {
                DockablePaneGroup groupToAttach = _rootGroup.RemovePane(pane);

                if (groupToAttach != null)
                {
                    _rootGroup = groupToAttach;
                    _rootGroup.ParentGroup = null;                  
                }
            }
        }

        /// <summary>
        /// Removes the specified pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void Remove(Window pane)
        {
            if (_rootGroup != null)
            {
                DockablePaneGroup groupToAttach = _rootGroup.RemovePane(pane);

                if (groupToAttach != null)
                {
                    _rootGroup = groupToAttach;
                    _rootGroup.ParentGroup = null;
                    ArrangeLayout(); //It should not come 
                }

                // ArrangeLayout(); //it should come on 22 jul
            }
        }

        /// <summary>
        /// Arranges the pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        protected internal void ArrangePane(Window pane)
        {           
            if (pane.StoredMoveToWindow.StoredMoveToWindow != null)
            {
                ArrangePane(pane.StoredMoveToWindow);
            }
            pane.DockState = DockState.Hidden;
            pane.StoredMoveToWindow.DockState = DockState.Hidden;
        }

        /// <summary>
        /// Arranges by the tab pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="changedPane">The changed pane.</param>
        protected internal void ArrangebyTabPane(Window pane, Window changedPane)
        {
            if (pane.StoredMoveToWindow != null)
            {
                pane.StoredMoveToWindow.MoveTo(changedPane, pane.MovetToDockPosition);               
            }
            if (pane.StoredMoveToWindow != null)
            {
                ArrangebyTabPane(pane.StoredMoveToWindow, pane.StoredMoveToWindow);
            }           
        }

        /// <summary>
        /// Arrangs the order.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="changedPane">The changed pane.</param>
        protected internal void ArrangOrder(Window pane, Window changedPane)
        {
                changedPane.StoredMoveToWindow = pane.StoredMoveToWindow;
                changedPane.MovetToDockPosition = pane.MovetToDockPosition;  
        }

        /// <summary>
        /// Changes the tab order.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="changedPane">The changed pane.</param>
        public void ChangeTabOrder(Window pane, Window changedPane)
        {
            if (pane.StoredMoveToWindow != null)
            {
                ArrangePane(pane);              
                Clear(gridDocking);
                _dockManager.ChangeOrder(pane, changedPane);
                ArrangebyTabPane(pane, changedPane);
                ArrangOrder(pane, changedPane);
            }
            else
            {
                Clear(gridDocking);
                _dockManager.ChangeOrder(pane, changedPane);
            }
        }

        /// <summary>
        /// Handles the OnStateChanged event of the pane control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void pane_OnStateChanged(object sender, EventArgs e)
        {
            Window pane = sender as Window;
            ////if (pane.State == PaneState.FloatingWindow)
            ////    Remove(pane);
            ////else
            ArrangeLayout();
        }
        /// <summary>
        /// Moves to window.
        /// </summary>
        /// <param name="sourcePane">The source pane.</param>
        /// <param name="destinationPane">The destination pane.</param>
        /// <param name="relativeDock">The relative dock.</param>
        public void MoveToWindow(Window sourcePane, Window destinationPane, Dock relativeDock)
        {
            Remove(sourcePane);
            Add(sourcePane, destinationPane, relativeDock);
        }
        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="sourcePane">The source pane.</param>
        /// <param name="destinationPane">The destination pane.</param>
        /// <param name="relativeDock">The relative dock.</param>
        public void MoveTo(Window sourcePane, Window destinationPane, Dock relativeDock)
        {
            if (sourcePane != null && destinationPane != null)
            {
                DockablePaneGroup group = GetPaneGroup(destinationPane);
                if (group != null)
                {
                    Remove(sourcePane);
                }

                Add(sourcePane, destinationPane, relativeDock);
                sourcePane.DockPosition = destinationPane.WindowChildElement != null
                                              ? destinationPane.DockPosition
                                              : relativeDock;
            }
        }

        /// <summary>
        /// Removes the panel.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void RemovePanel(Window pane)
        {
            Remove(pane);
        }

        /// <summary>
        /// Replaces the child.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="relativePane">The relative pane.</param>
        /// <param name="relativeDock">The relative dock.</param>
        public void ReplaceChild(Window pane, Window relativePane, Dock relativeDock)
        {
            DockablePaneGroup group = GetPaneGroup(relativePane);           
            if (_dockManager != null)
            {
                if (_dockManager.Parent != null)
                {
                    if (_dockManager.Parent is WindowContainer)
                    {
                        pane.DockState = DockState.Float;
                        if ((_dockManager.Parent as WindowContainer)._window.WindowCollection.Contains(relativePane))
                        {
                            //int index = (relativePane.DockManager.Parent as WindowContainer)._window.WindowCollection.IndexOf(relativePane);
                            //pane.DockManager = relativePane.DockManager;
                            //(relativePane.DockManager.Parent as WindowContainer)._window.WindowCollection[index] = pane;
                        }
                    }
                    else
                    {
                        pane.DockState = DockState.Dock;
                    }
                }
            }
            pane.Width = double.NaN;
            pane.Height = double.NaN;
            if (relativePane.DockPosition != Dock.Tabbed)
            {
                pane.DockPosition = relativePane.DockPosition;
                Dock dockposition = pane.DockPosition;
                if (pane.DockState == DockState.Dock)
                {
                    pane.DockingManager.SetboolValueWithSideInMode(pane, dockposition, DockState.Dock);
                    //DockingManager.SetSideInDockedMode(pane.WindowChildElement, dockposition);
                }
                else
                {
                }
            }
            else
            {
                if (relativeDock != Dock.Tabbed)
                {
                    pane.DockPosition = relativeDock;
                    Dock dockposition = pane.DockPosition;
                    if (pane.DockState == DockState.Dock)
                    {
                        pane.DockingManager.SetboolValueWithSideInMode(pane, dockposition, DockState.Dock);
                        //DockingManager.SetSideInDockedMode(pane.WindowChildElement, dockposition);
                    }
                    else
                    {
                    }
                }
            }
            pane.Visibility = Visibility.Visible;
            if (_dockManager.Parent is WindowContainer)
            {
                pane.DockingManager.RemoveDock(pane);
            }
            else
            {
                pane.DockingManager.ShowDockbutton(pane);
            }
            pane.ApplyDockStyle();

            pane.PaneWidth = relativePane.PaneWidth;
            pane.PaneHeight = relativePane.PaneHeight;
            //DockablePaneGroup parentGroup = group.ParentGroup;
            RemovePanel(pane);
            if (group != null)
            {
                group.ReplaceChild(pane);
                if (relativePane.DockState == DockState.Dock)
                {
                    //relativePane.DockState = DockState.Hidden;
                }
                //if (relativePane.Parent.GetType() == typeof(Grid))
                //{
                //    (relativePane.Parent as Grid).Children.Remove(relativePane);
                //}
                ArrangeLayout();
            }
        }

        /// <summary>
        /// Replaces the child.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="relativePane">The relative pane.</param>
        /// <param name="relativeDock">The relative dock.</param>
        /// <param name="allowToArrangeLayout">If set to <c>true</c> [allow to arrange layout].</param>
        public void ReplaceChild(Window pane, Window relativePane, Dock relativeDock, bool allowToArrangeLayout)
        {
            DockablePaneGroup group = GetPaneGroup(relativePane);
            if (_dockManager != null)
            {
                if (_dockManager.Parent != null)
                {
                    if (_dockManager.Parent is WindowContainer)
                    {
                        pane.DockState = DockState.Float;
                        if ((_dockManager.Parent as WindowContainer)._window.WindowCollection.Contains(relativePane))
                        {
                            //int index = (relativePane.DockManager.Parent as WindowContainer)._window.WindowCollection.IndexOf(relativePane);
                            //pane.DockManager = relativePane.DockManager;
                            //(relativePane.DockManager.Parent as WindowContainer)._window.WindowCollection[index] = pane;
                        }
                    }
                    else
                    {
                        pane.DockState = DockState.Dock;
                    }
                }
            }
            pane.Width = double.NaN;
            pane.Height = double.NaN;
            if (relativePane.DockPosition != Dock.Tabbed)
            {
                pane.DockPosition = relativePane.DockPosition;
            }
            else
            {
                if (relativeDock != Dock.Tabbed)
                {
                    pane.DockPosition = relativeDock;
                }
            }
            pane.Visibility = Visibility.Visible;
            if (_dockManager.Parent is WindowContainer)
            {
                pane.DockingManager.RemoveDock(pane);
            }
            else
            {
                pane.DockingManager.ShowDockbutton(pane);
            }
            pane.ApplyDockStyle();

            pane.PaneWidth = relativePane.PaneWidth;
            pane.PaneHeight = relativePane.PaneHeight;
            //DockablePaneGroup parentGroup = group.ParentGroup;
            RemovePanel(pane);
            if (group != null)
            {
                group.ReplaceChild(pane);
                if (relativePane.DockState == DockState.Dock)
                {
                    //relativePane.DockState = DockState.Hidden;
                }
                //if (relativePane.Parent.GetType() == typeof(Grid))
                //{
                //    (relativePane.Parent as Grid).Children.Remove(relativePane);
                //}
                if (allowToArrangeLayout)
                {
                    ArrangeLayout();
                }
            }
        }


        /// <summary>
        /// Attachedpanes the into group.
        /// </summary>
        /// <param name="relativeDock">The relative dock.</param>
        /// <param name="pane">The pane.</param>
        /// <param name="group">The group.</param>
        private void AttachedpaneIntoGroup(Dock relativeDock, Window pane, DockablePaneGroup group)
        {
            switch (relativeDock)
            {
                case Dock.Right:
                case Dock.Bottom:
                    {
                        if (group == _rootGroup)
                        {
                            if (pane.DockManager != null)
                            {
                                if (pane.DockManager.onApply)
                                {
                                    pane.DockManager.gridDocking.Add(pane);
                                    if (pane.DockingManager.WindowCollection.Count == 1)
                                    {
                                        Add(pane);
                                    }
                                    break;
                                }
                            }
                            DockingGrid dg = _rootGroup.rootGrid;
                            _rootGroup = new DockablePaneGroup(group, new DockablePaneGroup(pane,_rootGroup.rootGrid), relativeDock);                           
                            _rootGroup.rootGrid = dg;
                        }
                        else
                        {
                            DockablePaneGroup parentGroup = group.ParentGroup;
                            DockablePaneGroup newChildGroup = new DockablePaneGroup(group, new DockablePaneGroup(pane, _rootGroup.rootGrid), relativeDock);
                            parentGroup.ReplaceChildGroup(group, newChildGroup);
                        }
                    }

                    break;
                case Dock.Left:
                case Dock.Top:
                    {
                        if (group == _rootGroup)
                        {
                            if (pane.DockManager != null)
                            {
                                if (pane.DockManager.onApply)
                                {
                                    pane.DockManager.gridDocking.Add(pane);
                                    if (pane.DockingManager.WindowCollection.Count == 1)
                                    {
                                        Add(pane);
                                    }
                                    break;
                                }
                            }
                            DockingGrid dg = _rootGroup.rootGrid;
                            _rootGroup = new DockablePaneGroup(new DockablePaneGroup(pane, _rootGroup.rootGrid), group, relativeDock);
                            _rootGroup.rootGrid = dg;
                        }
                        else
                        {
                            DockablePaneGroup parentGroup = group.ParentGroup;
                            DockablePaneGroup newChildGroup = new DockablePaneGroup(new DockablePaneGroup(pane, _rootGroup.rootGrid), group, relativeDock);
                            parentGroup.ReplaceChildGroup(group, newChildGroup);
                        }
                    }

                    break;
                ////return new DockablePaneGroup(new DockablePaneGroup(pane), this, pane.Dock);
            }

            ////group.ChildGroup = new DockablePaneGroup(group.ChildGroup, pane, relativeDock);
            ArrangeLayout();
        }


        /// <summary>
        /// Overs the write existing group.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="relativePane">The relative pane.</param>
        protected internal void OverWriteExistingGroup(Window pane,Window relativePane)
        {
            
            AttachPaneEvents(pane);
            DockablePaneGroup group = GetPaneGroup(relativePane);
            DockablePaneGroup newGroup = (pane.DockManager.Children[0] as DockingGrid).GetPaneGroup(pane);
            //pane.DockState = DockState.Hidden;
            //(pane.DockManager.Children[0] as DockingGrid).ArrangeLayout();
            //pane.DockState = DockState.Dock;
            //DockablePaneGroup parentGroup = group.ParentGroup;
            //parentGroup.ReplaceChildGroup(group, newGroup);
            //pane.DockManager = relativePane.DockManager;
            //ArrangeLayout();
        }

        /// <summary>
        /// Adds the specified pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="relativePane">The relative pane.</param>
        /// <param name="relativeDock">The relative dock.</param>
        public void Add(Window pane, Window relativePane, Dock relativeDock)
        {
            if (relativePane != null)
            {
                Console.WriteLine("Add(...)");
                AttachPaneEvents(pane);

                if (pane.Parent != null && !_dockManager.IsOnApplyeTemplatefinish)
                {
                    if (pane.Parent.GetType() != typeof(Grid))
                    {
                        if (this._dockManager.Parent is WindowContainer)
                        {
                            if (!(this._dockManager.Parent as WindowContainer).handledLater)
                            {
                                ((Canvas)pane.DockingManager).Children.Remove(pane);
                            }
                        }
                        else
                        {
                            ((Canvas)pane.DockingManager).Children.Remove(pane);
                        }
                        ////pane.IsHidden = false;
                    }
                }

                DockablePaneGroup group = GetPaneGroup(relativePane);
                ////group.ParentGroup.ReplaceChildGroup(group, new DockablePaneGroup(group, new DockablePaneGroup(relativePane), relativeDock));
                if (group != null)
                {
                    AttachedpaneIntoGroup(relativeDock, pane, group);
                    if (relativePane._Caption == string.Empty)
                    {
                        pane.MoveWindowTargetName = "ClientArea";
                        pane.MoveDockPosition = relativeDock;
                    }
                    else
                    {
                        pane.MoveWindowTargetName = relativePane._Caption;
                        pane.MoveDockPosition = relativeDock;
                        pane.DockPosition = relativePane.DockPosition;
                    }
                }
                else
                {
                    if (relativePane.DockManager != null)
                    {
                        group = (relativePane.DockManager.Children[0] as DockingGrid).GetPaneGroup(relativePane);
                    }
                    else
                    {
                        group = null;
                    }
                    if (group != null)
                    {
                        (relativePane.DockManager.Children[0] as DockingGrid).MoveToWindow(pane, relativePane, relativeDock);//.AttachedpaneIntoGroup(relativeDock, pane, group);
                    }
                }
            }
        }

        /// <summary>
        /// Replaces the root window.
        /// </summary>
        /// <param name="pane">The pane.</param>
        protected internal void ReplaceRootWindow(Window pane)
        {         
            _rootGroup._attachedPane = pane;
        }

        /// <summary>
        /// Determines whether [is pane group presented] [the specified pane].
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns>
        /// 	<c>true</c> if [is pane group presented] [the specified pane]; otherwise, <c>false</c>.
        /// </returns>
        protected internal bool IsPaneGroupPresented(Window pane)
        {
            DockablePaneGroup paneGroup = GetPaneGroup(pane);
            if (paneGroup == null)
            {
                return false;
            }
            else
            {
                return true;
            }            
        }

        /// <summary>
        /// Gets the order of group.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns>The Pane Group Order.</returns>
        protected internal int GetOrderofGroup(Window pane)
        {
            DockablePaneGroup paneGroup = GetPaneGroup(pane);
            if (paneGroup != null)
            {
                return paneGroup.Order;
            }
            else
            {
                return 0;
            }            
        }

        /// <summary>
        /// Changes the order.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="relativePane">The relative pane.</param>
        protected internal void ChangeOrder(Window pane, Window relativePane)
        {
            DockablePaneGroup paneGroup = GetPaneGroup(pane);
            DockablePaneGroup relativePaneGroup = GetPaneGroup(relativePane);

            if (paneGroup != null && relativePaneGroup != null)
            {
                int order = paneGroup.Order;
                relativePaneGroup.Order = order;
            }   
        }

        /// <summary>
        /// Gets the pane group.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns>The Docking Pane Group.</returns>
        DockablePaneGroup GetPaneGroup(Window pane)
        {
            return _rootGroup.GetPaneGroup(pane);
        }

        /// <summary>
        /// Dumps the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="indent">The indent.</param>
        void Dump(DockablePaneGroup group, int indent)
        {
            if (indent == 0)
            {
                Console.WriteLine("Dump()");
            }

            for (int i = 0; i < indent; i++)
            {
                Console.Write("-");
            }

            Console.Write(">");

            if (group.AttachedPane == null)
            {
                Console.WriteLine(group.Dock);
                Dump(group.FirstChildGroup, indent + 4);
                Console.WriteLine();
                Dump(group.SecondChildGroup, indent + 4);
            }            
        }

        /// <summary>
        /// Gets the splitter.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The Custom Grid Splitter.</returns>
        protected internal CustomGridSplitter GetSplitter(Grid grid)
        {
            CustomGridSplitter csplitter = null;
            foreach (UIElement child in grid.Children)
            {
                if (child is CustomGridSplitter)
                {
                    csplitter = child as CustomGridSplitter; 
                }                
            }
            return csplitter;
        }

        /// <summary>
        /// Gets the first child group.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The Window.</returns>
        protected internal Window GetFirstChildGroup(Grid grid)
        {
            Window paretnWindow = null;
            if (paretnWindow == null)
            {
                foreach (UIElement child in grid.Children)
                {
                    if (paretnWindow == null)
                    {
                        if (child is Grid)
                        {
                            paretnWindow = GetFirstChildGroup(child as Grid);
                        }
                        else if (child is Window)
                        {
                            paretnWindow = child as Window;
                        }
                    }
                }
            }
            return paretnWindow;
        }

        /// <summary>
        /// Gets the grid location.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="relativeGrid">The relative grid.</param>
        /// <returns>The temp value.</returns>
        protected internal int GetGridLocation(Grid grid,Grid relativeGrid)
        {
            int i = 0,temp = 0;
            foreach (UIElement child in grid.Children)
            {
                i++;
                if (child is Grid)
                {
                    if (child as Grid == relativeGrid)
                    {
                        temp = i;
                    }                
                }
            }
            return temp;
        }

        /// <summary>
        /// Represents the Order Collection.
        /// </summary>
       protected internal List<string> orderCollection = new List<string>();

       /// <summary>
       /// Represents the Last Grid.
       /// </summary>
       Grid LastGrid = null;

       /// <summary>
       /// Orders the collection.
       /// </summary>
       /// <param name="grid">The grid.</param>
       /// <returns>The Ordered Collection.</returns>
        protected internal List<string> OrderCollection(Grid grid)
        {            
            foreach (UIElement child in grid.Children)
            {                
                if (child is Grid)
                {
                    OrderCollection(child as Grid);
                }
                else if (child is Window)
                {
                    //if (LastGrid == grid.Parent as Grid)
                    //{
                    //    orderCollection.Add(((Window)child)._Caption + " Move To " + ((Window)child).DockPosition);
                    //}
                    //else
                    //{
                        orderCollection.Add(((Window)child)._Caption);
                    //}
                    LastGrid = grid.Parent as Grid;
                }
            }
            return orderCollection;
        }

        /// <summary>
        /// Clears the specified grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void Clear(Grid grid)
        {
            if (grid != null)
            {
                foreach (UIElement child in grid.Children)
                {
                    if (child is Grid)
                    {
                        Clear(child as Grid);
                    }
                }

                grid.Children.Clear();
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
            }
        }

        /// <summary>
        /// Clears the dock window.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void ClearDockWindow(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    ClearDockWindow(child as Grid);
                }
                else if (child is Window)
                {
                    if (!colletion.Contains((Window)child))
                    {

                    }
                }
            }

            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
        }

        /// <summary>
        /// Gets or sets the colletion.
        /// </summary>
        /// <value>The colletion.</value>
        protected internal List<Window> colletion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the window element.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void GetWindowElement(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    GetWindowElement(child as Grid);
                }
                else if (child is Window)
                {
                    if (!colletion.Contains((Window)child))
                    {
                        if (rootWindow != (Window)child)
                        {                           
                            colletion.Add((Window)child);
                        }
                    }
                }
            }

            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
        }

        /// <summary>
        /// Gets all window element.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void GetAllWindowElement(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    GetAllWindowElement(child as Grid);
                }
                else if (child is Window)
                {
                    if (!colletion.Contains((Window)child))
                    { 
                            colletion.Add((Window)child); 
                    }
                }
            }

            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
        }


        /// <summary>
        /// Aranges the entire dock fill layout.
        /// </summary>
        protected internal void ArangeEntireDockFillLayout()
        {
            for (int i = 1; i <= rootWindow.DockManager.DockingParent.WindowCollection.Count; i++)
            {
                Window w = rootWindow.DockManager.DockingParent.WindowCollection[i];
                if (w.DockManager == rootWindow.DockManager)
                {
                    Remove(w);
                }
            }
            for (int i = 1; i <= rootWindow.DockManager.DockingParent.WindowCollection.Count; i++)
            {
                Window w = rootWindow.DockManager.DockingParent.WindowCollection[i];
                if (w.DockManager == rootWindow.DockManager)
                {
                    if (rootWindow != w)
                    {
                        if (i == 1)
                        {
                            ReplaceRootWindow(w);
                            rootWindow = w;
                            AttachPaneEvents(w);
                            ArrangeLayout();
                        }
                        else
                        {
                           // Remove((Window)w);
                            w.DockManager = _dockManager;
                            Add(w);
                            AttachPaneEvents(w);
                        }
                    }
                }
            }            
        }


        /// <summary>
        /// Aranges the entire un dock fill layout.
        /// </summary>
        protected internal void ArangeEntireUnDockFillLayout()
        {
            for (int i = 1; i <= rootWindow.DockManager.DockingParent.WindowCollection.Count; i++)
            {
                Window w = rootWindow.DockManager.DockingParent.WindowCollection[i];
                if (w.DockManager == rootWindow.DockManager)
                {
                    Remove(w);
                }
            }
            for (int i = 1; i <= rootWindow.DockManager.DockingParent.WindowCollection.Count; i++)
            {
                Window w = rootWindow.DockManager.DockingParent.WindowCollection[i];
                if (w.DockManager == rootWindow.DockManager)
                {
                    if (rootWindow != w)
                    {
                        //Remove((Window)w);                        
                        w.DockManager = _dockManager;
                        Add(w);
                        AttachPaneEvents(w);
                    }
                    else
                    {
                        ReplaceRootWindow(rootedWindow);
                        rootWindow = rootedWindow;
                        ArrangeLayout();
                        //Remove((Window)w);
                        w.DockManager = _dockManager;
                        Add(w);
                        AttachPaneEvents(w);
                    }
                }
            }
        }

        /// <summary>
        /// Aranges the entire layout.
        /// </summary>
        protected internal void ArangeEntireLayout()
        {
            for (int i = 1; i <= rootWindow.DockManager.DockingParent.WindowCollection.Count; i++)
            {
                Window w = rootWindow.DockManager.DockingParent.WindowCollection[i];
                if (w.DockManager == rootWindow.DockManager)
                {
                    //foreach(Window w in colletion)
                    //{
                    if (rootWindow != w)
                    {
                        Remove((Window)w);
                        w.DockManager = _dockManager;
                        Add(w);
                        AttachPaneEvents(w);
                    }
                    //}
                }
            }
        }

        /// <summary>
        /// Clears the width of the column.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void ClearColumnWidth(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    ClearColumnWidth(child as Grid);
                }
            }

            foreach (ColumnDefinition cd in grid.ColumnDefinitions)
            {
                cd.Width = new GridLength(1, GridUnitType.Auto);
            }
        }

        /// <summary>
        /// Clears the height of the row.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void ClearRowHeight(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    ClearRowHeight(child as Grid);
                }
            }

            foreach (RowDefinition cd in grid.RowDefinitions)
            {
                cd.Height = new GridLength(1, GridUnitType.Star);
            }
        }

        /// <summary>
        /// Clears the width of the column MAX.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void ClearColumnMAXWidth(Grid grid)
        {
            grid.ColumnDefinitions[0].MaxWidth = double.PositiveInfinity;
            grid.ColumnDefinitions[1].MaxWidth = double.PositiveInfinity;
        }

        /// <summary>
        /// Clears the height of the row MAX.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void ClearRowMAXHeight(Grid grid)
        {
            grid.RowDefinitions[0].MaxHeight = double.PositiveInfinity;
            grid.RowDefinitions[1].MaxHeight = double.PositiveInfinity;
        }

        /// <summary>
        /// Updatepanes the width of the heightand.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void UpdatepaneHeightandWidth(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    UpdatepaneHeightandWidth(child as Grid);
                }
                else if (child is Window)
                {
                    Window w = child as Window;
                    if (w.Parent is Grid)
                    {
                        if ((w.Parent as Grid).ActualHeight > 0)
                        {
                            w.PaneHeight = (w.Parent as Grid).ActualHeight;
                        }
                        if ((w.Parent as Grid).ActualWidth > 0)
                        {
                            w.PaneWidth = (w.Parent as Grid).ActualWidth;
                        }
                    }
                }
            }
        }
    }
}
