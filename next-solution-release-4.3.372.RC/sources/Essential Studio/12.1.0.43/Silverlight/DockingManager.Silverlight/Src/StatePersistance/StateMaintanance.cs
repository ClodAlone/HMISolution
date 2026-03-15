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
using System.Linq;
namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the window class.
    /// </summary>
    public partial class Window
    {

        /// <summary>
        /// Gets the tar get name window with another tab window.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <returns></returns>
        protected internal Window GetTarGetNameWindowWithAnotherTabWindow(Window w)
        {            
            Window temp = w;
            if (DockingManager.GetSideInDockedMode(w.WindowChildElement) == Dock.Tabbed)
            {
                List<Window> windowCollection = new List<Window>(this.DockingManager.WindowCollection.Values);
                IEnumerable<Window> query1 = windowCollection.Where(tempwindow => ((Window)tempwindow)._Caption == DockingManager.GetTargetNameInDockedMode(w.WindowChildElement));
                if (query1.Count() > 0)
                {
                    Window loc = query1.ElementAt(0);
                    if (loc != temp)
                    {
                        temp = GetTarGetNameWindowWithAnotherTabWindow(query1.ElementAt(0));
                    }
                }
            }
            return temp;
            
        }

        /// <summary>
        /// Generats the float window.
        /// </summary>
        protected internal void GeneratFloatWindow()
        {
            if (((Canvas)DockingManager).Children.Contains(this))
            {
                if (this.DockState == DockState.Dock)
                {
                    ((Canvas)DockingManager).Children.Remove(this);
                }
            }
             
            SetFloatWindow();
            this.Visibility = Visibility.Visible;
            this.DockState = DockState.Float;
            if (!(this._Caption != string.Empty && !((Canvas)DockingManager).Children.Contains(this)))
            {
                Window target = this.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(this.WindowChildElement));
                if (((Canvas)DockingManager).Children.Contains(this))
                {
                    ((Canvas)DockingManager).Children.Remove(this);
                }
                if (target != null)
                {
                    this.FloatHeight = target.FloatHeight;
                    this.FloatWidth = target.FloatWidth;
                    this.LeftPosition = target.LeftPosition;
                    this.TopPosition = target.TopPosition;
                }
                if (this.FloatHeight == 0)
                {
                    this.FloatHeight = 200;
                    this.FloatWidth = 200;
                }
                Canvas.SetLeft(this, this.LeftPosition);
                Canvas.SetTop(this, this.TopPosition);
                this.Height = this.FloatHeight;
                this.Width = this.FloatWidth;
                Canvas.SetZIndex(this, ++Window.currentZIndex);
                ((Canvas)DockingManager).Children.Add(this);
                this.ApplyBorderForFloatWindow();
                this.Visibility = Visibility.Visible;
            }
            if (this.dockToggle != null)
            {
                this.dockToggle.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// States the trans.
        /// </summary>
        protected internal void StateTrans()
        {
            bool allowmeToTestIsFloatWindowPresent = false;
            string ssd = DockingManager.GetTargetNameInFloatingMode(this.WindowChildElement);
            if (this._Caption != string.Empty && this.DockState != DockState.AutoHidden && this.DockableState != DockableState.Floating)
            {
                this.isDragging = false;
                this.isResizing = false;
                if (this.DockState == DockState.Dock)
                {                     
                    if (DockingManager.GetTargetNameInFloatingMode(this.WindowChildElement) != string.Empty && DockingManager.GetSideInFloatMode(this.WindowChildElement) != Dock.Tabbed)
                    {
                        this.DockState = DockState.Float;
                        if (!(this.DockManager.Parent is WindowContainer))
                        {
                            if (this.OldValueDockManager != null)
                            {
                                if (this.OldValueDockManager.Parent is WindowContainer)
                                {
                                    DockManager old = this.DockManager;
                                    this.DockManager = this.OldValueDockManager;
                                    this.OldValueDockManager = old;
                                }
                            }
                        }

                        StateMaintananceWindowContainerDock(allowmeToTestIsFloatWindowPresent, null);
                    }
                    else if (DockingManager.GetTargetNameInFloatingMode(this.WindowChildElement) != string.Empty && DockingManager.GetSideInFloatMode(this.WindowChildElement) == Dock.Tabbed)
                    {
                        Window target = this.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(this.WindowChildElement));
                        if (target.Visibility == Visibility.Visible && target.DockState == DockState.Float)
                        {
                            if (target.CustomTabControl != null)
                            {
                                if (this.CustomTabControl != null)
                                {
                                   CustomTabItem cstabitem = GetExactTabWindowWithCuastomTabItem(this);
                                   if (cstabitem != null)
                                   {
                                       if (this.CustomTabControl.Items.Contains(cstabitem))
                                       {
                                           this.CustomTabControl.Items.Remove(cstabitem);
                                           if (!target.CustomTabControl.Items.Contains(cstabitem))
                                           {
                                               target.CustomTabControl.Items.Add(cstabitem);
                                               target.DockingManager.HideTabPanel(target);
                                               target.DockingManager.HideTabPanel(this);
                                               this.Visibility = Visibility.Collapsed;
                                               this.DockState = DockState.Float;
                                               this.Visibility = Visibility.Collapsed;
                                           }
                                       }
                                   }
                                }
                            }
                        }
                        else
                        {
                            if (this.OldValueDockManager != null)
                            {
                                if (this.OldValueDockManager.Parent is WindowContainer)
                                {
                                    this.DockState = DockState.Float;
                                    if (!(this.DockManager.Parent is WindowContainer))
                                    {
                                        if (this.OldValueDockManager != null)
                                        {
                                            if (this.OldValueDockManager.Parent is WindowContainer)
                                            {
                                                DockManager old = this.DockManager;
                                                this.DockManager = this.OldValueDockManager;
                                                this.OldValueDockManager = old;
                                            }
                                        }
                                    }
                                    //this.DockingManager.UpdateTargetNameForFloatWindow(target, this);
                                    StateMaintananceWindowContainerDock(true, target);
                                    
                                }
                                else
                                {
                                    this.DockState = DockState.Float;
                                    if (((Canvas)DockingManager).Children.Contains(this))
                                    {
                                        ((Canvas)DockingManager).Children.Remove(this);
                                    }
                                    this.FloatHeight = target.FloatHeight;
                                    this.FloatWidth = target.FloatWidth;
                                    this.LeftPosition = target.LeftPosition;
                                    this.TopPosition = target.TopPosition;
                                    Canvas.SetLeft(this, target.LeftPosition);
                                    Canvas.SetTop(this, target.TopPosition);
                                    Canvas.SetZIndex(this, ++Window.currentZIndex);
                                    ((Canvas)DockingManager).Children.Add(this);
                                    this.ApplyBorderForFloatWindow();
                                    this.Visibility = Visibility.Visible;
                                    this.DockingManager.UpdateTargetNameForFloatWindow(target, this);
                                }
                            }
                            else
                            {
                                this.DockState = DockState.Float;
                                if (((Canvas)DockingManager).Children.Contains(this))
                                {
                                    ((Canvas)DockingManager).Children.Remove(this);
                                }
                                this.FloatHeight = target.FloatHeight;
                                this.FloatWidth = target.FloatWidth;
                                this.LeftPosition = target.LeftPosition;
                                this.TopPosition = target.TopPosition;
                                Canvas.SetLeft(this, target.LeftPosition);
                                Canvas.SetTop(this, target.TopPosition);
                                Canvas.SetZIndex(this, ++Window.currentZIndex);
                                ((Canvas)DockingManager).Children.Add(this);
                                this.ApplyBorderForFloatWindow();
                                this.Visibility = Visibility.Visible;
                                this.DockingManager.UpdateTargetNameForFloatWindow(target, this);
                            }
                        }
                        
                    }
                    else
                    {
                        List<Window> windowCollection = new List<Window>(this.DockingManager.WindowCollection.Values);
                        IEnumerable<Window> query1 = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == this._Caption && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed);
                        if (query1.Count() > 0)
                        {
                            allowmeToTestIsFloatWindowPresent = true;
                            this.DockState = DockState.Float;
                            if (!(this.DockManager.Parent is WindowContainer))
                            {
                                if (this.OldValueDockManager != null)
                                {
                                    if (this.OldValueDockManager.Parent is WindowContainer)
                                    {
                                        DockManager old = this.DockManager;
                                        this.DockManager = this.OldValueDockManager;
                                        this.OldValueDockManager = old;
                                    }
                                }
                            }

                            StateMaintananceWindowContainerDock(allowmeToTestIsFloatWindowPresent, null);
                        }
                        else
                        {
                            if (this.OldValueDockManager != null)
                            {
                                if (this.OldValueDockManager.Parent is WindowContainer)
                                {
                                    this.DockState = DockState.Float;
                                    if (!(this.DockManager.Parent is WindowContainer))
                                    {
                                        if (this.OldValueDockManager != null)
                                        {
                                            if (this.OldValueDockManager.Parent is WindowContainer)
                                            {
                                                DockManager old = this.DockManager;
                                                this.DockManager = this.OldValueDockManager;
                                                this.OldValueDockManager = old;
                                            }
                                        }
                                    }

                                    StateMaintananceWindowContainerDock(true, null);
                                    //this.DockingManager.UpdateTargetNameForFloatWindow(target, this);
                                }
                                else
                                {
                                    GeneratFloatWindow();
                                }
                            }
                            else
                            {
                                GeneratFloatWindow();
                            }
                        }
                    }
                    
                }
                else if (this.DockState == DockState.Float)
                {
                    bool allowMe = false;
                    if (this.DockManager != null)
                    {
                        if (this.DockManager.Parent is WindowContainer)
                        {
                            if (this.CustomTabControl != null)
                            {
                                if (this.CustomTabControl.Items.Count == 1)
                                {
                                    string sds = DockingManager.GetTargetNameInDockedMode(this.WindowChildElement);
                                    SetFloatWindow();
                                }
                                else
                                {
                                    DockManager container = this.DockManager;
                                    allowMe = true;
                                    SetFloatWindow();
                                    DockManager currentvalue = this.DockManager;
                                    this.DockManager = container;
                                    Window windowcont = (this.DockManager.Parent as WindowContainer)._window;
                                    foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                                    {
                                        if (cstabitem.OwnWindow != null)
                                        {
                                            //this.DockingManager.UpdateMoveToTargetName(cstabitem.OwnWindow, true);
                                            if (windowcont.WindowCollection.Contains(cstabitem.OwnWindow))
                                            {
                                                windowcont.WindowCollection.Remove(cstabitem.OwnWindow);
                                                cstabitem.OwnWindow.OldValueDockManager = null;
                                            }
                                        }                                        
                                    }
                                    this.DockingManager.SetboolValueWithTargetName(this, string.Empty, DockState.Float);
                                    DockingManager.SetTargetNameInFloatingMode(this.WindowChildElement, string.Empty);
                                    this.DockManager = currentvalue;
                                    this.OldValueDockManager = null;
                                }
                            }
                            else
                            {
                                SetFloatWindow();
                            }
                        }
                    }
                    if (DockingManager.GetSideInDockedMode(this.WindowChildElement) == Dock.Tabbed && !allowMe)
                    {
                        List<Window> windowCollection = new List<Window>(this.DockingManager.WindowCollection.Values);
                        string sds = DockingManager.GetTargetNameInDockedMode(this.WindowChildElement);
                        IEnumerable<Window> query1 = windowCollection.Where(tempwindow => ((Window)tempwindow)._Caption == DockingManager.GetTargetNameInDockedMode(this.WindowChildElement));
                        if (query1.Count() > 0)
                        {
                            //Window w = query1.ElementAt(0);
                            Window w = GetTarGetNameWindowWithAnotherTabWindow(query1.ElementAt(0));

                            if ((w.DockState == DockState.Dock && w.Visibility == Visibility.Visible && !((Canvas)DockingManager).Children.Contains(w)) || w.DockState == DockState.AutoHidden)
                            {
                                if (w.CustomTabControl != null)
                                {
                                    if (this.CustomTabControl != null)
                                    {
                                        CustomTabItem cstabitem = GetExactTabWindowWithCuastomTabItem(this);
                                        if (cstabitem != null)
                                        {
                                            if (this.CustomTabControl.Items.Contains(cstabitem))
                                            {
                                                this.CustomTabControl.Items.Remove(cstabitem);
                                                if (!w.CustomTabControl.Items.Contains(cstabitem))
                                                {
                                                    w.CustomTabControl.Items.Add(cstabitem);
                                                    w.DockingManager.HideTabPanel(w);
                                                    w.DockingManager.HideTabPanel(this);
                                                    this.Visibility = Visibility.Collapsed;
                                                    this.DockState = DockState.Dock;

                                                    if (this.DockManager.Parent is WindowContainer)
                                                    {
                                                        if (this.OldValueDockManager != null)
                                                        {
                                                            DockManager swap = this.DockManager;
                                                            this.DockManager = this.OldValueDockManager;
                                                            this.OldValueDockManager = swap;
                                                        }
                                                        else
                                                        {
                                                            DockingGrid dockingGrid = this.DockingManager.GetParentDockManager();
                                                            if (dockingGrid.Parent != null)
                                                            {
                                                                this.DockManager = dockingGrid.Parent as DockManager;
                                                            }
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
                                if (this.DockManager.Parent is WindowContainer)
                                {
                                    if (this.OldValueDockManager != null)
                                    {
                                        DockManager swap = this.DockManager;
                                        this.DockManager = this.OldValueDockManager;
                                        this.OldValueDockManager = swap;
                                    }
                                    else
                                    {
                                        DockingGrid dockingGrid = this.DockingManager.GetParentDockManager();
                                        if (dockingGrid.Parent != null)
                                        {
                                            this.DockManager = dockingGrid.Parent as DockManager;
                                        }
                                    }
                                }
                                this.DockingManager.UpdateTargetName(w, this);
                               (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, w, w.DockPosition);
                                //(this.DockManager.Children[0] as DockingGrid).ChangeTabOrder(w, this);
                                this.Visibility = Visibility.Visible;
                                
                                if (this.DockManager.Parent is WindowContainer)
                                {
                                    this.DockingManager.RemoveDock(this);
                                }
                                else if (this.DockManager.Parent is DockingManager)
                                {
                                    this.DockingManager.ShowDockbutton(this);
                                }
                                this.DockingManager.HideTabPanel(this);
                                
                            }

                        }
                        else
                        {
                            this.Visibility = Visibility.Visible;
                            this.Height = double.NaN;
                            this.Width = double.NaN;
                            DockManager oldvalue = this.DockManager;
                            if (this.DockManager.Parent is WindowContainer)
                            {
                                this.OldValueDockManager = oldvalue;
                            }
                            else
                            {
                                this.OldValueDockManager = null;
                            }
                            this.DockState = DockState.Dock;
                            this.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                            this.DockManager.gridDocking.ArrangeLayout();
                            this.DockingManager.ShowDockbutton(this);
                            this.ApplyDockStyle();
                        }
                    }
                    else if(!allowMe)
                    {
                        this.Visibility = Visibility.Visible;
                        this.Height = double.NaN;
                        this.Width = double.NaN;                        
                        DockManager oldvalue = this.DockManager;
                        if (this.OldValueDockManager != null)
                        {
                            if (this.DockManager.Parent is WindowContainer)
                            {
                                this.OldValueDockManager = oldvalue;
                            }
                            else if(!(this.OldValueDockManager.Parent is WindowContainer))
                            {
                                this.OldValueDockManager = null;
                            }
                        }
                        this.DockState = DockState.Dock;
                        this.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                        int sds = this.DockManager.gridDocking.GetOrderofGroup(this);
                        this.DockManager.gridDocking.ArrangeLayout();
                        this.DockingManager.ShowDockbutton(this);
                        this.ApplyDockStyle();
                    }

                    if (this._headerEventFired && !allowMe)
                    {
                        if (this.CustomTabControl != null)
                        {
                            if (this.CustomTabControl.Items.Count > 0)
                            {
                                //List<CustomTabItem> cstabitemCollection = new List<CustomTabItem>();
                                //foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                                //{
                                //    if (cstabitem.OwnWindow != null)
                                //    {
                                //        if (!cstabitemCollection.Contains(cstabitem))
                                //        {
                                //            cstabitemCollection.Add(cstabitem);
                                //        }
                                //    }
                                //}
                                //foreach (CustomTabItem cstabitem in cstabitemCollection)
                                //{
                                //    if (cstabitem.OwnWindow != this)
                                //    {
                                //        if (this.CustomTabControl.Items.Contains(cstabitem))
                                //        {
                                //            this.CustomTabControl.Items.Remove(cstabitem);
                                //        }
                                //        if (!cstabitem.OwnWindow.CustomTabControl.Items.Contains(cstabitem))
                                //        {
                                //            cstabitem.OwnWindow.CustomTabControl.Items.Add(cstabitem);
                                //            if (cstabitem.OwnWindow.CustomTabControl.SelectedItem != null)
                                //            {
                                //                cstabitem.OwnWindow.Caption = cstabitem.Header.ToString();
                                //                cstabitem.OwnWindow.DockingManager.HideTabPanel(cstabitem.OwnWindow);
                                //            }
                                //            else if (cstabitem.OwnWindow.CustomTabControl.Items.Count > 0)
                                //            {
                                //                cstabitem.OwnWindow.Caption = ((CustomTabItem)cstabitem.OwnWindow.CustomTabControl.Items[0]).Header.ToString();
                                //                cstabitem.OwnWindow.DockingManager.HideTabPanel(cstabitem.OwnWindow);
                                //            }
                                //        }
                                //    }
                                //}
                                //foreach (CustomTabItem cstabitem in cstabitemCollection)
                                //{
                                //    if (cstabitem.OwnWindow != this)
                                //    {
                                //        cstabitem.OwnWindow.StateTrans();
                                //    }
                                //}
                                if (this.CustomTabControl.Items.Count <= 1)
                                {
                                    if (this.CustomTabControl.SelectedItem != null)
                                    {
                                        this.Caption = ((CustomTabItem)this.CustomTabControl.SelectedItem).Header.ToString();
                                    }
                                    else if (this.CustomTabControl.Items.Count > 0)
                                    {
                                        this.Caption = ((CustomTabItem)this.CustomTabControl.Items[0]).Header.ToString();
                                    }
                                    if (this.CustomTabControl.primitiveTabPanel != null)
                                    {
                                        this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                                        this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                                    }
                                }//this.DockingManager.HideTabPanel(this);
                            }
                        }
                    }
                }
            }
            if (this.DockState == DockState.Float)
            {
                if (this.dockToggle != null)
                {
                    this.dockToggle.Visibility = Visibility.Collapsed;
                }
            }
        }


        /// <summary>
        /// Gets the exact tab window with cuastom tab item.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <returns></returns>
        protected internal CustomTabItem GetExactTabWindowWithCuastomTabItem(Window w)
        {
            CustomTabItem cstab = null;
            foreach (CustomTabItem cstabitem in w.CustomTabControl.Items)
            {
                if (cstabitem.OwnWindow == this)
                {
                    cstab = cstabitem;
                }
            }
            return cstab;
        }

        /// <summary>
        /// Removes from docking manager.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void RemoveFromDockingManager(Window w)
        {
            if (((Canvas)w.DockingManager).Children.Contains(w))
            {
                ((Canvas)w.DockingManager).Children.Remove(w);
            }
        }

        /// <summary>
        /// Gets the float window from window container.
        /// </summary>
        /// <param name="windowContainer">The window container.</param>
        /// <returns></returns>
        protected internal Window GetFloatWindowFromWindowContainer(Window windowContainer)
        {
            Window w = null;
            List<Window> windowCollection = new List<Window>(this.DockingManager.WindowCollection.Values);
                        IEnumerable<Window> query1 = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == this._Caption && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed && ((Canvas)DockingManager).Children.Contains(((Window)tempwindow)) && windowContainer.WindowCollection.Contains(((Window)tempwindow)) && ((Window)tempwindow).Visibility == Visibility.Visible);
                        if (query1.Count() > 0)
                        {
                            w = query1.ElementAt(0);
                        }
                        return w;
        }

        /// <summary>
        /// Numberofs the float windowwith container.
        /// </summary>
        /// <param name="windowcontainer">The windowcontainer.</param>
        /// <returns></returns>
        protected internal int NumberofFloatWindowwithContainer(Window windowcontainer)
        {
            List<Window> windowCollection = new List<Window>(this.DockingManager.WindowCollection.Values);
            IEnumerable<Window> query1 = windowcontainer.WindowCollection.Where(tempwindow => ((Window)tempwindow).DockState == DockState.Float && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed && ((Canvas)DockingManager).Children.Contains(((Window)tempwindow)) && ((Window)tempwindow).Visibility == Visibility.Visible);
            return query1.Count();

        }
        /// <summary>
        /// States the maintanance window container dock.
        /// </summary>
        /// <param name="allowmeToTestIsFloatWindowPresent">if set to <c>true</c> [allowme to test is float window present].</param>
        /// <param name="target">The target.</param>
        protected internal void StateMaintananceWindowContainerDock(bool allowmeToTestIsFloatWindowPresent, Window target)
        {
            if (this.DockManager.Parent is WindowContainer)
            {
                Window w = (this.DockManager.Parent as WindowContainer)._window;
                int numberofChildren = NumberofChildren(w, w.WindowContainer.DockManager);
                int numberofFloatWindow = NumberofFloatChildren(w, w.WindowContainer.DockManager);
                //int numberofFloatWindow = NumberofFloatWindowwithContainer(w);
                if (numberofFloatWindow == 1)
                {
                    if (!w.WindowCollection.Contains(this))
                    {
                        w.WindowCollection.Add(this);
                    }
                    leastWindow = null;
                    CheckChildrenPresent(w, w.WindowContainer.DockManager);
                    Window _w = null;
                    //if (!allowmeToTestIsFloatWindowPresent)
                    //{
                        _w = CheckFloatWindowIsPresent(w, w.WindowContainer.DockManager);
                    //}
                    //else
                    //{
                    //    _w = GetFloatWindowFromWindowContainer(w);
                    //}
                    int order = w.WindowContainer.DockManager.gridDocking.GetOrderofGroup(_w);
                    if (_w != null && _w != this)//&& w.DockManager == _w.DockManager)
                    {
                        //if (order != 0)
                        //{
                            w.Visibility = Visibility.Visible;
                            (this.DockManager.Parent as WindowContainer).IsstateTransInvoke = true;
                            if (!((Canvas)DockingManager).Children.Contains(w))
                            {
                                ((Canvas)DockingManager).Children.Add(w);
                                w.ApplyBorderForFloatWindow();
                            }
                            RemoveFromDockingManager(this);
                            RemoveFromDockingManager(_w);
                            this.Width = double.NaN;
                            this.Height = double.NaN;
                            _w.Width = double.NaN;
                            _w.Height = double.NaN;
                            this.Visibility = Visibility.Visible;
                            _w.Visibility = Visibility.Visible;
                            _w.DockManager = this.DockManager;
                            _w.OldValueDockManager = this.OldValueDockManager;
                            if (_w.DockManager.gridDocking.GetOrderofGroup(_w) <= 0)
                            {
                                Window _target = _w.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(_w.WindowChildElement));
                                if (_target != null)
                                {
                                    if (this.DockManager.gridDocking.GetOrderofGroup(_target) <= 0)
                                    {
                                        if (this.CustomTabControl != null)
                                        {
                                            foreach (CustomTabItem cstabitem in _w.CustomTabControl.Items)
                                            {
                                                if (this.DockManager.gridDocking.GetOrderofGroup(cstabitem.OwnWindow) > 0)
                                                {
                                                    (this.DockManager.Children[0] as DockingGrid).ReplaceChild(_w, cstabitem.OwnWindow, DockingManager.GetSideInFloatMode(cstabitem.OwnWindow.WindowChildElement));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DockingManager.GetSideInFloatMode(_target.WindowChildElement) == Dock.Tabbed || DockingManager.GetSideInFloatMode(_w.WindowChildElement) == Dock.Tabbed)
                                        {
                                            this.DockingManager.UpdateTargetNameForFloatWindow(_target, _w);
                                        }
                                        (this.DockManager.Children[0] as DockingGrid).ReplaceChild(_w, _target, DockingManager.GetSideInFloatMode(_target.WindowChildElement));
                                       
                                    }
                                    //(this.DockManager.Children[0] as DockingGrid).ReplaceChild(_w, _target, DockingManager.GetSideInFloatMode(_target.WindowChildElement));
                                }
                                else
                                {
                                    if (_w.CustomTabControl != null)
                                    {
                                        foreach (CustomTabItem cstabitem in _w.CustomTabControl.Items)
                                        {
                                            if (_w.DockManager.gridDocking.GetOrderofGroup(cstabitem.OwnWindow) > 0)
                                            {
                                                (this.DockManager.Children[0] as DockingGrid).ReplaceChild(_w, cstabitem.OwnWindow, DockingManager.GetSideInFloatMode(cstabitem.OwnWindow.WindowChildElement));
                                                break;
                                            }
                                        }
                                    }
                                    //(_w.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                }
                            }
                            else
                            {
                                (_w.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            }
                            if (this.DockManager.gridDocking.GetOrderofGroup(this) <= 0)
                            {
                                Window _target = _w.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(this.WindowChildElement));
                                if (_target != null)
                                {
                                    if (this.DockManager.gridDocking.GetOrderofGroup(_target) <= 0)
                                    {
                                        if (this.CustomTabControl != null)
                                        {
                                            foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                                            {
                                                if (this.DockManager.gridDocking.GetOrderofGroup(cstabitem.OwnWindow) > 0)
                                                {
                                                    (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, cstabitem.OwnWindow, DockingManager.GetSideInFloatMode(cstabitem.OwnWindow.WindowChildElement));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, _target, DockingManager.GetSideInFloatMode(_target.WindowChildElement));
                                    }
                                    //(this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, _target, DockingManager.GetSideInFloatMode(_target.WindowChildElement));
                                }
                                else
                                {
                                    if (this.CustomTabControl != null)
                                    {
                                        foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                                        {
                                            if (_w.DockManager.gridDocking.GetOrderofGroup(cstabitem.OwnWindow) > 0)
                                            {
                                                (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, cstabitem.OwnWindow, DockingManager.GetSideInFloatMode(cstabitem.OwnWindow.WindowChildElement));
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                (_w.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            }
                            this.DockingManager.UpdateTargetNameForFloatWindow(target, this);
                            (_w.DockManager.Children[0] as DockingGrid).ArrangeLayout();                            
                            StateMaintanance st = this.CurrentStateMain;
                            this.CurrentStateMain = this.PreviousStateMain;
                            this.PreviousStateMain = st;
                            this.DockingManager.HideTabPanel(this);
                            this.DockingManager.HideTabPanel(_w);
                            this.ApplyDockStyle();
                            _w.ApplyDockStyle();
                            //st = _w.CurrentStateMain;
                            //_w.CurrentStateMain = _w.PreviousStateMain;
                            //_w.PreviousStateMain = st;
                        //}
                        //else
                        //{
                        //    StateMaintanance curr = this.CurrentStateMain;
                        //    StateMaintanance prev = this.PreviousStateMain;
                        //    StateMaintanance windowcurr = _w.CurrentStateMain;
                        //    StateMaintanance windowprev = _w.PreviousStateMain;
                        //    StateMaintainForContainer();
                        //    this.CurrentStateMain = prev;
                        //    this.PreviousStateMain = curr;
                        //    _w.CurrentStateMain = windowcurr;
                        //    _w.PreviousStateMain = windowprev;
                        //}
                    }
                    else
                    {
                        if (!((Canvas)DockingManager).Children.Contains(this))
                        {
                            Canvas.SetZIndex(this, ++currentZIndex);
                            //this.DockState = DockState.Float;
                            ((Canvas)DockingManager).Children.Add(this);
                            this.ApplyBorderForFloatWindow();
                            this.DockState = DockState.Float;
                            ApplyBorderForFloatWindow();
                            StateMaintanance st = this.CurrentStateMain;
                            this.CurrentStateMain = this.PreviousStateMain;
                            this.PreviousStateMain = st;
                        }
                    }

                }
                else if (numberofFloatWindow == 0 && w.Visibility == Visibility.Collapsed)
                {
                    if (this.Parent is Grid)
                    {
                        this.DockState = DockState.Float;
                    }
                    Canvas.SetLeft(this, Canvas.GetLeft(w));
                    Canvas.SetTop(this, Canvas.GetTop(w));
                    Canvas.SetZIndex(this, ++currentZIndex);
                    this.Height = w.Height;
                    this.Width = w.Width;
                    
                    if (!((Canvas)DockingManager).Children.Contains(this))
                    {
                        ((Canvas)DockingManager).Children.Add(this);
                        this.ApplyBorderForFloatWindow();
                        this.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.Visibility = Visibility.Visible;
                    }
                    if (this.CurrentStateMain == StateMaintanance.TabWithDock || this.CurrentStateMain == StateMaintanance.Dock)
                    {
                        StateMaintanance st = this.CurrentStateMain;
                        this.CurrentStateMain = this.PreviousStateMain;
                        this.PreviousStateMain = st;
                    }
                    ApplyBorderForFloatWindow();
                    target = this.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(this.WindowChildElement));
                    if (target != null)
                    {
                        if (DockingManager.GetSideInFloatMode(target.WindowChildElement) == Dock.Tabbed || DockingManager.GetSideInFloatMode(this.WindowChildElement) == Dock.Tabbed)
                        {
                            this.DockingManager.UpdateTargetNameForFloatWindow(target, this);
                        }
                    }
                }
                else if (numberofFloatWindow == 0 && w.Visibility == Visibility.Visible)
                {
                    if (!w.WindowCollection.Contains(this))
                    {
                        w.WindowCollection.Add(this);
                    }
                    this.DockState = DockState.Float;
                    RemoveFromDockingManager(this);
                    this.Width = double.NaN;
                    this.Height = double.NaN;
                    this.Visibility = Visibility.Visible;
                    ApplyDockStyle();
                    if (this.DockManager.gridDocking.GetOrderofGroup(this) <= 0)
                    {
                        Window _target = this.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(this.WindowChildElement));
                        if (_target != null)
                        {
                            if (this.DockManager.gridDocking.GetOrderofGroup(this) <= 0)
                            {
                                if (this.CustomTabControl != null && this.CustomTabControl.Items.Count > 1)
                                {
                                    foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                                    {
                                        if (this.DockManager.gridDocking.GetOrderofGroup(cstabitem.OwnWindow) > 0)
                                        {
                                            (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, cstabitem.OwnWindow, DockingManager.GetSideInFloatMode(cstabitem.OwnWindow.WindowChildElement));
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, _target, DockingManager.GetSideInFloatMode(_target.WindowChildElement));
                                }
                            }
                            else
                            {
                                (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, _target, DockingManager.GetSideInFloatMode(_target.WindowChildElement));
                            }
                        }
                        else
                        {
                            if (this.CustomTabControl != null)
                            {
                                foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                                {
                                    if (this.DockManager.gridDocking.GetOrderofGroup(cstabitem.OwnWindow) > 0)
                                    {
                                        (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, cstabitem.OwnWindow, DockingManager.GetSideInFloatMode(cstabitem.OwnWindow.WindowChildElement));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                    }
                    this.DockingManager.UpdateTargetNameForFloatWindow(target, this);
                    (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                    StateMaintanance st = this.CurrentStateMain;
                    this.CurrentStateMain = this.PreviousStateMain;
                    this.PreviousStateMain = st;
                }
            }
        }

        /// <summary>
        /// States the maintance for window container.
        /// </summary>
        protected internal void StateMaintanceForWindowContainer()
        {
            if (this._Caption != string.Empty && this.DockState != DockState.AutoHidden)
            {
                this.isDragging = false;
                this.isResizing = false;    
                if (this.PreviousStateMain == StateMaintanance.Float)
                {
                    this.Visibility = Visibility.Visible;
                    double leftVal = this.LeftPosition;
                    double topVal = this.TopPosition;
                    double floatwidth = this.FloatWidth;
                    double floatheight = this.FloatHeight;
                    DockState ds = this.DockState;
                    this.ChangeState(DockState.Float);
                    if (!((Canvas)DockingManager).Children.Contains(this))
                    {
                        ((Canvas)DockingManager).Children.Add(this);
                        this.ApplyBorderForFloatWindow();
                    }
                    if (floatheight > 0.0)
                    {
                        Canvas.SetLeft(this, leftVal);
                        Canvas.SetTop(this, topVal);
                        this.Width = floatwidth;
                        this.Height = floatheight;
                       // UpdateFloatSize();
                    }
                    if (this.CustomTabControl.Items.Count > 1)
                    {
                        for (int i = this.CustomTabControl.Items.Count - 1; i >= 0; i--)
                        {
                            CustomTabItem cstab = this.CustomTabControl.Items[i] as CustomTabItem;
                                //cstab.OwnWindow.StateMaintanceForTabItem();
                                StateMaintanance st = cstab.OwnWindow.CurrentStateMain;
                                if (this.DockManager != null)
                                {
                                    if (this.DockManager.Parent is WindowContainer)
                                    {
                                        cstab.OwnWindow.CurrentStateMain = StateMaintanance.TabWithContainer;
                                    }
                                    else if (this.DockManager.Parent is DockingManager)
                                    {
                                        cstab.OwnWindow.CurrentStateMain = StateMaintanance.TabWithFloat;
                                    }
                                }
                                if (st != StateMaintanance.TabWithFloat)
                                {
                                    cstab.OwnWindow.PreviousStateMain = st;
                                }
                        }
                    }
                    else
                    {
                        StateMaintanance st = this.CurrentStateMain;
                        this.CurrentStateMain = this.PreviousStateMain;
                        this.PreviousStateMain = st;
                    }
                    ApplyBorderForFloatWindow();
                }                
                else if (this.PreviousStateMain == StateMaintanance.WindowContainer)
                {
                    if (!(this.DockManager.Parent is WindowContainer))
                    {
                        this.DockState = DockState.Float;
                        if (this.OldValueDockManager != null)
                        {
                            if (this.OldValueDockManager.Parent is WindowContainer)
                            {
                                DockManager old = this.DockManager;
                                this.DockManager = this.OldValueDockManager;
                                this.OldValueDockManager = old;
                            }
                        }
                    }
                    if (this.DockManager.Parent is WindowContainer)
                    {
                        StateMaintananceWindowContainerDock(false, null);
                    }
                    else
                    {
                        if (this.OldValueDockManager != null)
                        {
                            StateMaintainForContainer();
                            //if (this.CustomTabControl.Items.Count == 1)
                            //{
                            //    DockManager swap = this.DockManager;
                            //    this.DockManager = this.OldValueDockManager;
                            //    this.OldValueDockManager = swap;
                            //    goto l;
                            //}
                            //else
                            //{

                            //}
                        }
                    }
                }
                else if (this.PreviousStateMain == StateMaintanance.Dock)
                {
                    this.Width = double.NaN;
                    this.Height = double.NaN;
                    if (this.dockToggle != null)
                    {
                        this.dockToggle.Visibility = Visibility.Visible;
                    }
                    if (this.DockManager.Parent is WindowContainer)
                    {
                        DockManager dm = this.DockManager;
                        this.DockState = DockState.Hidden;
                        this.DockManager.gridDocking.ArrangeLayout();
                        this.DockManager = this.OldValueDockManager;
                        this.OldValueDockManager = dm;
                        this.DockState = DockState.Dock;
                        this.DockManager.gridDocking.ArrangeLayout();
                        this.DockingManager.HideTabPanel(this);
                    }
                    else
                    {
                        if (this.DockManager.gridDocking.IsPaneGroupPresented(this))
                        {
                            this.Visibility = Visibility.Visible;
                            ChangeState(DockState.Dock);
                            this.DockingManager.HideTabPanel(this);
                        }
                        else
                        {
                            Window w = this.DockingManager.GetWindow(this.floatWindowTargetName);
                        }

                    }
                    StateMaintanance st = this.CurrentStateMain;
                    this.CurrentStateMain = this.PreviousStateMain;
                    this.PreviousStateMain = st;
                }
                else if (this.PreviousStateMain == StateMaintanance.TabWithFloat)
                {
                    StateMaintanance st;
                    Window w = this.DockingManager.GetWindow(this.floatWindowTargetName); //GetParent(this);
                    if (w != null)
                    {
                        if (((Canvas)DockingManager).Children.Contains(w) && w.Visibility == Visibility.Visible)
                        {
                            if (w.CustomTabControl != null)
                            {
                                if (this.DockState == DockState.Dock && this.DockManager.Parent is DockingManager)
                                {
                                    ChangeState(DockState.Float);
                                    if (((Canvas)DockingManager).Children.Contains(this))
                                    {
                                        ((Canvas)DockingManager).Children.Remove(this);
                                    }
                                }
                                CustomTabItem cstab = null;
                                if (this.CustomTabControl.Items.Count == 1)
                                {
                                    cstab = this.CustomTabControl.Items[0] as CustomTabItem;
                                    this.CustomTabControl.Items.Remove(cstab);
                                }
                                if (cstab != null)
                                {
                                    if (!w.CustomTabControl.Items.Contains(cstab))
                                    {
                                        w.CustomTabControl.Items.Add(cstab);
                                        w.Caption = cstab.Header.ToString();
                                        this.DockingManager.HideTabPanel(w);
                                        this.DockingManager.HideTabPanel(this);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.DockState == DockState.Dock)
                            {
                                ChangeState(DockState.Float);
                                if (((Canvas)DockingManager).Children.Contains(this))
                                {
                                    ((Canvas)DockingManager).Children.Remove(this);
                                }
                            }
                            Canvas.SetZIndex(this, ++currentZIndex);
                            Canvas.SetTop(this, Canvas.GetTop(w));
                            Canvas.SetLeft(this, Canvas.GetLeft(w));
                            this.Width = w.Width;
                            this.Height = w.Height;
                            this.Visibility = Visibility.Visible;                            
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                ((Canvas)DockingManager).Children.Add(this);

                                this.ApplyBorderForFloatWindow();
                            }
                            for (int i = 0; i < w.FloatWindowTargetNameCollection.Count; i++)
                            {
                                w.FloatWindowTargetNameCollection[i].OwnWindow.floatWindowTargetName = this._Caption;
                            }
                            ApplyBorderForFloatWindow();
                        }
                    }
                    else
                    {
                        this.Visibility = Visibility.Visible; double leftVal = this.LeftPosition;
                        double topVal = this.TopPosition;
                        double floatwidth = this.FloatWidth;
                        double floatheight = this.FloatHeight;
                        if (this.DockState == DockState.Dock)
                        {
                            this.ChangeState(DockState.Float);
                        }
                        if (!((Canvas)DockingManager).Children.Contains(this))
                        {
                            ((Canvas)DockingManager).Children.Add(this);
                            this.ApplyBorderForFloatWindow();
                        }
                        if (floatheight > 0.0)
                        {
                            Canvas.SetLeft(this, leftVal);
                            Canvas.SetTop(this, topVal);
                            this.Width = floatwidth;
                            this.Height = floatheight;
                            // UpdateFloatSize();
                        }
                        if (this.CustomTabControl.Items.Count > 1)
                        {
                            for (int i = this.CustomTabControl.Items.Count - 1; i >= 0; i--)
                            {
                                CustomTabItem cstab = this.CustomTabControl.Items[i] as CustomTabItem;
                                //cstab.OwnWindow.StateMaintanceForTabItem();
                                if (cstab.OwnWindow != this)
                                {
                                    st = cstab.OwnWindow.CurrentStateMain;
                                    if (this.DockManager != null)
                                    {
                                        if (this.DockManager.Parent is WindowContainer)
                                        {
                                            cstab.OwnWindow.CurrentStateMain = StateMaintanance.TabWithContainer;
                                        }
                                        else if (this.DockManager.Parent is DockingManager)
                                        {
                                            cstab.OwnWindow.CurrentStateMain = StateMaintanance.TabWithFloat;
                                        }
                                    }
                                    cstab.OwnWindow.PreviousStateMain = st;
                                }
                            }
                        }
                    }
                    ApplyBorderForFloatWindow();
                    st = this.CurrentStateMain;
                    this.CurrentStateMain = this.PreviousStateMain;
                    this.PreviousStateMain = st;
                }
                else if (this.PreviousStateMain == StateMaintanance.TabWithContainer)
                {

                    Window w = this.DockingManager.GetWindow(this.floatWindowTargetName);
                    if (this.DockManager.Parent is WindowContainer)
                    {
                        StateMaintainForContainer();
                    }
                    else if (this.DockManager.Parent is DockingManager)
                    {
                        if (this.OldValueDockManager != null)
                        {
                            this.DockState = DockState.Float;   
                            DockManager olddm = this.DockManager;
                            this.DockManager = this.OldValueDockManager;
                            this.OldValueDockManager = this.DockManager;
                            StateMaintainForContainer();
                        }
                    }
                    //if (w != null)
                    //{
                    //    if (w.DockManager.Parent is WindowContainer)
                    //    {
                    //        if (w.CustomTabControl != null)
                    //        {
                    //            CustomTabItem cstab = null;
                    //            if (this.CustomTabControl.Items.Count == 1)
                    //            {
                    //                cstab = this.CustomTabControl.Items[0] as CustomTabItem;
                    //                this.CustomTabControl.Items.Remove(cstab);
                    //            }
                    //            if (cstab != null)
                    //            {
                    //                if (!w.CustomTabControl.Items.Contains(cstab))
                    //                {
                    //                    w.CustomTabControl.Items.Add(cstab);
                    //                    w.Caption = cstab.Header.ToString();
                    //                    this.DockingManager.HideTabPanel(w);
                    //                    this.DockingManager.HideTabPanel(this);
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Canvas.SetZIndex(this, ++currentZIndex);
                    //        Canvas.SetTop(this, Canvas.GetTop(w));
                    //        Canvas.SetLeft(this, Canvas.GetLeft(w));
                    //        this.Width = w.Width;
                    //        this.Height = w.Height;
                    //        this.Visibility = Visibility.Visible;
                    //        if (!((Canvas)DockingManager).Children.Contains(this))
                    //        {
                    //            ((Canvas)DockingManager).Children.Add(this);
                    //        }
                    //        for (int i = 0; i < w.FloatWindowTargetNameCollection.Count; i++)
                    //        {
                    //            w.FloatWindowTargetNameCollection[i].OwnWindow.floatWindowTargetName = this._Caption;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (!((Canvas)DockingManager).Children.Contains(this))
                    //    {
                    //        ((Canvas)DockingManager).Children.Add(this);
                    //        this.Visibility = Visibility.Visible;
                    //    }
                    //}
                    StateMaintanance st = this.CurrentStateMain;
                    this.CurrentStateMain = this.PreviousStateMain;
                    this.PreviousStateMain = st;
                }
                else if (this.PreviousStateMain == StateMaintanance.WindowContainerToDock)
                {
                    StateMaintance();
                    StateMaintanance st = this.CurrentStateMain;
                    this.CurrentStateMain = this.PreviousStateMain;
                    this.PreviousStateMain = st;
                }
                else if (this.PreviousStateMain == StateMaintanance.DockToWindowContainer)
                {
                    Window w = null;
                    if (this.DockManager.Parent is WindowContainer)
                    {
                        w = (this.DockManager.Parent as WindowContainer)._window;
                    }                    
                    
                    int numberofChildren = 0;
                    if (w != null)
                    {
                        DockManager dm = this.DockManager;
                        ChangeStateandDockManager();
                        numberofChildren = NumberofChildren(w, this.DockManager);
                        if (numberofChildren > 1)
                        {
                            numberofChildren = NumberofChildren(w, dm);
                        }
                    }
                    else
                    {
                        this.Width = double.NaN;
                        this.Height = double.NaN;
                        ChangeState(DockState.Dock);
                        if (this.DockManager.Parent is DockingManager)
                        {
                            this.DockingManager.ShowDockbutton(this);
                        }
                        //if (this.OldValueDockManager.Parent is WindowContainer)
                        //{
                        //    w = (this.DockManager.Parent as WindowContainer)._window;
                        //}
                        //numberofChildren = NumberofChildren(w, this.OldValueDockManager);
                    }
                    if (numberofChildren <= 1 && numberofChildren != 0)
                    {
                        if (!w.WindowCollection.Contains(this))
                        {
                            w.WindowCollection.Add(this);
                        }
                        leastWindow = null;
                        if (!CheckChildrenPresent(w) && leastWindow != null)
                        {
                            w.Visibility = Visibility.Collapsed;
                            leastWindow.DockState = DockState.Hidden;
                            (leastWindow.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            leastWindow.DockManager.DetachPaneEvents(this);
                            Canvas.SetZIndex(leastWindow, ++currentZIndex);
                            Canvas.SetLeft(leastWindow, Canvas.GetLeft(w));
                            Canvas.SetTop(leastWindow, Canvas.GetTop(w));
                            leastWindow.Width = w.Width;
                            leastWindow.Height = w.Height;
                            if (!((Canvas)DockingManager).Children.Contains(leastWindow))
                            {
                                ((Canvas)DockingManager).Children.Add(leastWindow);
                                leastWindow.ApplyBorderForFloatWindow();
                                leastWindow.DockState = DockState.Float;
                            }
                        }
                    }
                    else
                    {
                        if (numberofChildren > 1 && NumberofChildren(w, this.OldValueDockManager) > 1)
                        {
                            w.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            if (w != null)
                            {
                                w.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                    StateMaintanance st = this.CurrentStateMain;
                    this.CurrentStateMain = this.PreviousStateMain;
                    this.PreviousStateMain = st;
                }
                else if (this.PreviousStateMain == StateMaintanance.TabWithDock)
                {
                    if (this.CurrentStateMain == StateMaintanance.WindowContainer || this.CurrentStateMain == StateMaintanance.TabWithContainer)
                    {
                        StateMaintanance st;
                        if (this.CustomTabControl.Items.Count == 1)
                        {

                            this.DockState = DockState.Float;
                            StateMaintanceForTabItem();
                            st = this.CurrentStateMain;
                            this.CurrentStateMain = this.PreviousStateMain;
                            this.PreviousStateMain = st;
                        }
                        else
                        {
                            if (this._headerEventFired)
                            {
                                if (this.DockManager.Parent is WindowContainer && !((Canvas)DockingManager).Children.Contains(this))
                                {
                                    this.Visibility = Visibility.Visible;
                                    double leftVal = this.LeftPosition;
                                    double topVal = this.TopPosition;
                                    double floatwidth = this.FloatWidth;
                                    double floatheight = this.FloatHeight;
                                    if (this.Parent is Grid)
                                    {
                                        floatheight = (this.Parent as Grid).ActualHeight;
                                        floatwidth = (this.Parent as Grid).ActualWidth;
                                    }
                                    DockState ds = this.DockState;
                                    this.ChangeState(DockState.Float);
                                    Canvas.SetZIndex(this, ++currentZIndex);
                                    if (!((Canvas)DockingManager).Children.Contains(this))
                                    {
                                        ((Canvas)DockingManager).Children.Add(this);
                                        this.ApplyBorderForFloatWindow();
                                    }
                                    if (floatheight > 0.0)
                                    {
                                        //Canvas.SetLeft(this, leftVal);
                                        //Canvas.SetTop(this, topVal);
                                        this.Width = floatwidth;
                                        this.Height = floatheight;
                                        // UpdateFloatSize();
                                    }
                                    if ((this.DockManager.Parent as WindowContainer)._window.WindowCollection.Contains(this))
                                    {
                                        (this.DockManager.Parent as WindowContainer)._window.WindowCollection.Remove(this);
                                    }
                                    for (int i = this.CustomTabControl.Items.Count - 1; i >= 0; i--)
                                    {
                                        CustomTabItem cstab = this.CustomTabControl.Items[i] as CustomTabItem;
                                        cstab.OwnWindow.CurrentStateMain = StateMaintanance.TabWithFloat;
                                        cstab.OwnWindow.floatWindowTargetName = this._Caption;
                                        cstab.OwnWindow.FloatWindowTargetNameCollection = this.FloatWindowTargetNameCollection;
                                        if ((this.DockManager.Parent as WindowContainer)._window.WindowCollection.Contains(cstab.OwnWindow))
                                        {
                                            (this.DockManager.Parent as WindowContainer)._window.WindowCollection.Remove(cstab.OwnWindow);
                                        }
                                    }
                                }
                                else if(((Canvas)DockingManager).Children.Contains(this))
                                {
                                    for (int i = this.CustomTabControl.Items.Count - 1; i >= 0; i--)
                                    {
                                        CustomTabItem cstabItem = this.CustomTabControl.Items[i] as CustomTabItem;
                                        if (cstabItem.OwnWindow != null)
                                        {                                            
                                            if (cstabItem.OwnWindow != this)
                                            {
                                                this.CustomTabControl.Items.Remove(cstabItem);
                                                cstabItem.OwnWindow.CustomTabControl.Items.Add(cstabItem);
                                                cstabItem.OwnWindow.StateMaintanceForWindowContainer();
                                            }
                                            
                                        }
                                    }
                                    StateMaintanceForWindowContainer();
                                }
                            }
                            else
                            {
                                StateMaintanceForTabItem();
                                st = this.CurrentStateMain;
                                this.CurrentStateMain = this.PreviousStateMain;
                                this.PreviousStateMain = st;
                                for (int i = this.CustomTabControl.Items.Count - 1; i >= 0; i--)
                                {
                                    CustomTabItem cstab = this.CustomTabControl.Items[i] as CustomTabItem;
                                    if (cstab.OwnWindow != this)
                                    {
                                        if (this.CustomTabControl.Items.Contains(cstab))
                                        {
                                            this.CustomTabControl.Items.Remove(cstab);
                                        }
                                        if (!cstab.OwnWindow.CustomTabControl.Items.Contains(cstab))
                                        {
                                            cstab.OwnWindow.CustomTabControl.Items.Add(cstab);
                                        }
                                        cstab.OwnWindow.DockState = DockState.Float;
                                        cstab.OwnWindow.StateMaintanceForTabItem();
                                        st = cstab.OwnWindow.CurrentStateMain;
                                        cstab.OwnWindow.CurrentStateMain = cstab.OwnWindow.PreviousStateMain;
                                        cstab.OwnWindow.PreviousStateMain = st;
                                        cstab.OwnWindow.FloatWindowTargetNameCollection = this.FloatWindowTargetNameCollection;
                                    }
                                }
                            }
                        }                       
                    }
                    else
                    {
                        StateMaintanance st;
                        if (this.CustomTabControl.Items.Count == 1)
                        {
                            StateMaintanceForTabItem();
                            st = this.CurrentStateMain;
                            this.CurrentStateMain = this.PreviousStateMain;
                            this.PreviousStateMain = st;
                        }
                        else
                        {
                            //for (int i = this.CustomTabControl.Items.Count - 1; i >= 0; i--)
                            //{
                            //    CustomTabItem cstab = this.CustomTabControl.Items[i] as CustomTabItem;
                            //    if (cstab.OwnWindow != this)
                            //    {
                            //     //   cstab.OwnWindow.StateMaintanceForTabItem();
                            //        st = cstab.OwnWindow.CurrentStateMain;
                            //        cstab.OwnWindow.CurrentStateMain = cstab.OwnWindow.PreviousStateMain;
                            //        cstab.OwnWindow.PreviousStateMain = st;
                            //    }
                            //}
                            StateMaintanceForTabItem();
                            st = this.CurrentStateMain;
                            this.CurrentStateMain = this.PreviousStateMain;
                            this.PreviousStateMain = st;
                            for (int i = this.CustomTabControl.Items.Count - 1; i >= 0; i--)
                            {
                                CustomTabItem cstab = this.CustomTabControl.Items[i] as CustomTabItem;
                                if (cstab.OwnWindow != this)
                                {
                                    if (this.CustomTabControl.Items.Contains(cstab))
                                    {
                                        this.CustomTabControl.Items.Remove(cstab);
                                    }
                                    if (!cstab.OwnWindow.CustomTabControl.Items.Contains(cstab))
                                    {
                                        cstab.OwnWindow.CustomTabControl.Items.Add(cstab);
                                    }

                                    cstab.OwnWindow.StateMaintanceForWindowContainer();
                                    st = cstab.OwnWindow.CurrentStateMain;
                                    cstab.OwnWindow.CurrentStateMain = cstab.OwnWindow.PreviousStateMain;
                                    cstab.OwnWindow.PreviousStateMain = st;
                                }
                            }
                            //StateMaintanceForTabItem();
                            //st = this.CurrentStateMain;
                            //this.CurrentStateMain = this.PreviousStateMain;
                            //this.PreviousStateMain = st;
                        }                        
                    }
                    
                }
            }
            #region OldContent


            //if (_Caption != string.Empty)
            //{
            //    if (this.PreviousState == DockState.Float)
            //    {
            //        if (this.CustomTabControl != null)
            //        {
            //        //    if (this.CustomTabControl.Items.Count > 1)
            //        //    {
            //        //        CustomTabItem tabitem = (CustomTabItem)this.CustomTabControl.SelectedItem;
            //        //        for (int i = 1; i < this.DockingManager.WindowCollection.Count; i++)
            //        //        {
            //        //            if (tabitem.Header.ToString() == this.DockingManager.WindowCollection[i]._Caption.ToString() && this.Caption != tabitem.Header.ToString())
            //        //            {
            //        //            }
            //        //        }
            //        //    }
            //        }

            //        if (this.CanFloat)
            //        {
            //            Window _tempWindow = CheckDockWindowPresented();
            //            if (_tempWindow == null)
            //            {
            //                //SetFloat();
            //                StateMaintance();
            //            }
            //            else
            //            {
            //                //if (this.CustomTabControl.Items.Count > 1)
            //                //{
            //                CustomTabItem custtab = (CustomTabItem)this.CustomTabControl.SelectedItem;
            //                if (custtab != null)
            //                {
            //                    this.CustomTabControl.Items.Remove(custtab);
            //                    _tempWindow.CustomTabControl.Items.Add(custtab);
            //                    if (this.CustomTabControl.Items.Count <= 1)
            //                    {
            //                        this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
            //                        this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
            //                        this.Caption = custtab.Header.ToString();
            //                    }
            //                    else
            //                    {
            //                        this.Caption = custtab.Header.ToString();
            //                    }
            //                    _tempWindow.CustomTabControl.TabPanelBackground = this.DockingManager.TabPanelBackground;
            //                    _tempWindow.Caption = custtab.Header.ToString();
            //                    this.ChangeState(DockState.Hidden);
            //                }
            //                //}
            //                //else
            //                //{
            //                //  //  SetFloat();
            //                //}
            //            }
            //        }
            //    }
            //    else if (this.PreviousState == DockState.Dock )//&& this.CanDock)
            //    {
            //        DockState ds = this.DockState;
            //        if (this.PreviousState == ds)
            //        {
            //            StateMaintance();
            //        }
            //        else
            //        {
            //            if (this.PreviousDockSide == Dock.Tabbed)
            //            {
            //                StateMaintanceForTabItem();
            //                if (ds == DockState.Hidden)
            //                {
            //                    this.PreviousState = DockState.Dock;
            //                }
            //                else
            //                {
            //                    this.PreviousState = ds;
            //                }
            //            }
            //            else ////if (this.PreviousDockSide == Dock.None)
            //            {
            //                Window w = CheckDockWindowPresented();
            //                if (w == null)
            //                {
            //                    this.Width = double.NaN;
            //                    this.Height = double.NaN;
            //                    if (((Canvas)DockingManager).Children.Contains(this))
            //                    {
            //                        ((Canvas)DockingManager).Children.Remove(this);
            //                        this.DockingManager.ShowDockbutton(this);
            //                    }
            //                    ChangeState(DockState.Dock);
            //                    if (this.CustomTabControl != null)
            //                    {
            //                        if (this.CustomTabControl.Items.Count <= 1 && this.CustomTabControl.TabPanelBorder != null)
            //                        {
            //                            this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
            //                            this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
            //                        }
            //                        else if (this.CustomTabControl.Items.Count > 1 && this.CustomTabControl.TabPanelBorder != null)
            //                        {
            //                            this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
            //                            this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
            //                        }
            //                    }
            //                    this.PreviousState = ds;
            //                }
            //                else
            //                {
            //                    //CheckTarGetNameCollection();
            //                    this.PreviousState = ds;
            //                }
            //            }
            //        }
            //    }
            //}

#endregion
        }

        /// <summary>
        /// Gets the next window from window container.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <returns></returns>
        protected internal Window GetNextWindowFromWindowContainer(Window w)
        {
            Window w1 = null;
            for (int i = 0; i < w.WindowCollection.Count; i++)
            {
                if (w.WindowCollection[i].CurrentStateMain == StateMaintanance.WindowContainer && w.WindowCollection[i] != this)
                {
                    w1 = w.WindowCollection[i];
                    break;
                }
            }
            return w1;
        }

        /// <summary>
        /// States the maintance for tab item.
        /// </summary>
        protected internal void StateMaintanceForTabItem()
        {
            Window window =  CheckDockWindowPresented();
            if (window == null)
            {
                Window w = GetWindow();
                if (w != null && w != this)
                {
                    if (this.CurrentStateMain == StateMaintanance.WindowContainer ||this.CurrentStateMain == StateMaintanance.TabWithContainer)
                    {
                        if (this.OldValueDockManager != null)
                        {
                            DockManager swap = this.DockManager;
                            this.DockManager = this.OldValueDockManager;
                            this.OldValueDockManager = swap;
                            //this.DockManager = this.DockingManager.GetParentDockManager().Parent as DockManager;
                        }
                    }
                    else if (this.DockManager.Parent is WindowContainer)
                    {
                        if (this.OldValueDockManager != null)
                        {
                            DockManager swap = this.DockManager;
                            this.DockManager = this.OldValueDockManager;
                            this.OldValueDockManager = swap;
                        }
                        else
                        {
                            DockingGrid dockingGrid = this.DockingManager.GetParentDockManager();
                            if (dockingGrid.Parent != null)
                            {
                                this.DockManager = dockingGrid.Parent as DockManager;
                            }
                        }
                    }
                   
                    (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, w, w.DockPosition);
                    //(this.DockManager.Children[0] as DockingGrid).ChangeTabOrder(w, this);
                    this.Visibility = Visibility.Visible;
                    this.DockingManager.UpdateTargetName(w, this);
                    if (this.DockManager.Parent is WindowContainer)
                    {
                        this.DockingManager.RemoveDock(this);
                    }
                    else if (this.DockManager.Parent is DockingManager)
                    {
                        this.DockingManager.ShowDockbutton(this);
                    }
                    this.DockingManager.HideTabPanel(this);
                    //StateMaintanance st = this.CurrentStateMain;
                    //this.CurrentStateMain = this.PreviousStateMain;
                    //this.PreviousStateMain = st;
                }
                else
                {
                    if (this.CurrentStateMain == StateMaintanance.WindowContainer || this.CurrentStateMain == StateMaintanance.TabWithContainer)
                    {
                        if (this.OldValueDockManager != null)
                        {
                            DockManager swap = this.DockManager;
                            this.DockManager = this.OldValueDockManager;
                            this.OldValueDockManager = swap;
                        }
                    }
                    else if (this.DockManager.Parent is WindowContainer)
                    {
                        if (this.OldValueDockManager != null)
                        {
                            DockManager swap = this.DockManager;
                            this.DockManager = this.OldValueDockManager;
                            this.OldValueDockManager = swap;
                        }
                        else
                        {
                            DockingGrid dockingGrid = this.DockingManager.GetParentDockManager();
                            if (dockingGrid.Parent != null)
                            {
                                this.DockManager = dockingGrid.Parent as DockManager;
                            }
                        }
                    }
                    this.Height = double.NaN;
                    this.Width = double.NaN;                 
                    ChangeState(DockState.Dock);
                    this.DockingManager.HideTabPanel(this);
                    if (this.DockManager.Parent is WindowContainer)
                    {
                        this.DockingManager.RemoveDock(this);
                    }
                    else
                    {
                        this.DockingManager.ShowDockbutton(this);
                    }
                }
            }
            else
            {
                if (this.DockManager.Parent is WindowContainer)
                {
                    if (this.OldValueDockManager != null)
                    {
                        DockManager swap = this.DockManager;
                        this.DockManager = this.OldValueDockManager;
                        this.OldValueDockManager = swap;
                    }
                }
            }
            //if (!CheckTarGetNameCollection())
            //{
                //Window w = GetWindow();
                //if (w != null)
                //{
                //    (this.DockManager.Children[0] as DockingGrid).ReplaceChild(w, this, this.DockPosition);
                //    //(this.DockManager.Children[0] as DockingGrid).ChangeTabOrder(w, this);
                //    this.Visibility = Visibility.Visible;
                //}
           // }
        }


        /// <summary>
        /// Determines whether [is float window target present] [the specified w].
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="cstabItem">The cstab item.</param>
        /// <returns>
        /// 	<c>true</c> if [is float window target present] [the specified w]; otherwise, <c>false</c>.
        /// </returns>
        protected internal bool IsFloatWindowTargetPresent(Window w ,CustomTabItem cstabItem)
        {
            bool ispresent = false;
            for (int i = 0; i < w.FloatWindowTargetNameCollection.Count; i++)
            {
                if (cstabItem == w.FloatWindowTargetNameCollection[i])
                {
                    ispresent =  true;
                    break;
                }
                else
                {
                    ispresent =  false;
                }
            }
            return ispresent;
        }

        /// <summary>
        /// States the maintain for container.
        /// </summary>
        protected internal void StateMaintainForContainer()
        {
            Window windowContainer = null;
            if (this.DockManager.Parent is WindowContainer)
            {
                windowContainer = (this.DockManager.Parent as WindowContainer)._window;
            }
            else
            {
                if (this.OldValueDockManager != null)
                {
                    this.DockState = DockState.Float;
                    (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                    if (this.OldValueDockManager.Parent is WindowContainer)
                    {
                        DockManager swap = this.DockManager;
                        this.DockManager = this.OldValueDockManager;
                        this.OldValueDockManager = swap;
                    }
                    windowContainer = (this.DockManager.Parent as WindowContainer)._window;
                }
            }
            int numberofChildren = 0;
            numberofChildren = NumberofFloatChildren(windowContainer, this.DockManager);
            if (numberofChildren == 0)
            {
                numberofChildren = NumberofFloatWithTarGetname(windowContainer, this.DockManager);
            }
            //Window w = this.DockingManager.GetWindow(this.floatWindowTargetName);
            if (numberofChildren == 1)
            {
                if (IsFloatWindowTargetPresent(this, this.CustomTabControl.Items[0] as CustomTabItem))
                {
                    Window w = this.DockingManager.GetWindow(this.floatWindowTargetName);
                    if (w != null && w.Visibility == Visibility.Visible)
                    {
                        if (this.CustomTabControl.Items.Count == 1)
                        {
                            CustomTabItem custab = this.CustomTabControl.Items[0] as CustomTabItem;
                            this.CustomTabControl.Items.Remove(custab);
                            if (!w.CustomTabControl.Items.Contains(custab))
                            {
                                w.CustomTabControl.Items.Add(custab);
                            }
                            this.DockingManager.HideTabPanel(w);
                            if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ((Canvas)DockingManager).Children.Remove(this);
                            }
                        }
                    }
                    else
                    {
                        windowContainer.Visibility = Visibility.Visible;
                        this.DockingManager.RemoveDock(windowContainer);
                        this.DockingManager.RemoveDock(this);
                        leastWindow = null;
                        Window temp = CheckFloatWindowIsPresent(windowContainer, this.DockManager);
                        //Window temp = leastWindow;
                        if (temp != null)
                        {
                            if (windowContainer.WindowCollection.Contains(this))
                            {
                                temp.Height = double.NaN;
                                temp.Width = double.NaN;
                                temp.DockState = DockState.Dock;
                                this.DockState = DockState.Dock;
                                temp.Height = double.NaN;
                                temp.Width = double.NaN;
                                this.Height = double.NaN;
                                this.Width = double.NaN;
                                bool ispresentOrder = false;
                                if (temp.DockManager.gridDocking.GetOrderofGroup(temp) == 0)
                                {
                                    w = this.DockingManager.GetWindow(temp.floatWindowTargetName);
                                    (temp.DockManager.Children[0] as DockingGrid).ReplaceChild(temp, w, w.DockPosition);
                                    ispresentOrder = true;
                                }
                                if (this.DockManager.gridDocking.GetOrderofGroup(this) == 0)
                                {
                                    w = this.DockingManager.GetWindow(this.floatWindowTargetName);
                                    (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, w, w.DockPosition);
                                    ispresentOrder = true;
                                }
                                if (!ispresentOrder)
                                {
                                    (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                }
                            }
                            else
                            {
                                w = this.DockingManager.GetWindow(this.floatWindowTargetName);
                                temp.Height = double.NaN;
                                temp.Width = double.NaN;
                                temp.DockState = DockState.Dock;
                                this.DockState = DockState.Dock;
                                (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, w, w.DockPosition);
                                for (int i = 0; i < w.FloatWindowTargetNameCollection.Count; i++)
                                {
                                    w.FloatWindowTargetNameCollection[i].OwnWindow.floatWindowTargetName = this._Caption;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Window floatWindow = GetFloatWindowWithTarGetname(windowContainer, this.DockManager);
                    //if (!windowContainer.WindowCollection.Contains(floatWindow))
                    //{
                    Window target = GetFloatWindowFromWindowContainer(windowContainer, floatWindow);
                    if (target != null && target != this)
                    {
                        //if (this.CurrentStateMain == StateMaintanance.WindowContainer || this.CurrentStateMain == StateMaintanance.TabWithContainer)
                        //{
                        if (floatWindow.DockManager != windowContainer.WindowContainer.DockManager)
                        {
                            if (floatWindow.OldValueDockManager != null)
                            {
                                DockManager swap = floatWindow.DockManager;
                                floatWindow.DockManager = floatWindow.OldValueDockManager;
                                floatWindow.OldValueDockManager = swap;
                            }
                        }
                        if (this.DockManager != windowContainer.WindowContainer.DockManager)
                        {
                            if (this.OldValueDockManager != null)
                            {
                                DockManager swap = this.DockManager;
                                this.DockManager = this.OldValueDockManager;
                                this.OldValueDockManager = swap;
                            }
                        }
                        //}
                        this.DockState = DockState.Dock;
                        if (floatWindow != target)
                        {
                            (floatWindow.DockManager.Children[0] as DockingGrid).ReplaceChild(floatWindow, target, target.DockPosition);
                        }
                        else
                        {
                            floatWindow.Height = double.NaN;
                            floatWindow.Width = double.NaN;
                            this.Height = double.NaN;
                            this.Width = double.NaN;
                            floatWindow.DockState = DockState.Dock;
                            if (floatWindow.DockManager.gridDocking.GetOrderofGroup(floatWindow) == 0)
                            {
                                target = this.DockingManager.GetWindow(floatWindow.floatWindowTargetName);
                                if (target != null)
                                {
                                    (floatWindow.DockManager.Children[0] as DockingGrid).ReplaceChild(floatWindow, target, target.DockPosition);
                                }
                            }
                            if (this.DockManager.gridDocking.GetOrderofGroup(floatWindow) == 0)
                            {
                                target = this.DockingManager.GetWindow(this.floatWindowTargetName);
                                if (target != null)
                                {
                                    (this.DockManager.Children[0] as DockingGrid).ReplaceChild(this, target, target.DockPosition);
                                }
                            }
                            (floatWindow.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            this.DockingManager.HideTabPanel(floatWindow);
                            this.DockingManager.HideTabPanel(this);

                        }

                        //(this.DockManager.Children[0] as DockingGrid).ChangeTabOrder(w, this);
                        this.Visibility = Visibility.Visible;
                        //this.DockingManager.UpdateTargetName(target,floatWindow);
                        StateMaintanance st = floatWindow.CurrentStateMain;
                        floatWindow.CurrentStateMain = floatWindow.PreviousStateMain;
                        floatWindow.PreviousStateMain = st;

                    }
                    else
                    {
                    }
                    //}
                    //else
                    //{
                    //    this.DockState = DockState.Dock;
                    //    (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                    //}
                }

                // (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
            }
            else if (NumberofChildren(windowContainer, this.DockManager) > 1)
            {
                Window w = this.DockingManager.GetWindow(this.floatWindowTargetName);
                if (this.CustomTabControl.Items.Count == 1 && w != null)
                {
                    CustomTabItem custab = this.CustomTabControl.Items[0] as CustomTabItem;
                    this.CustomTabControl.Items.Remove(custab);
                    if (!w.CustomTabControl.Items.Contains(custab))
                    {
                        w.CustomTabControl.Items.Add(custab);
                        if (((Canvas)DockingManager).Children.Contains(this))
                        {
                            ((Canvas)DockingManager).Children.Remove(this);
                        }
                    }
                    this.DockingManager.HideTabPanel(w);
                }
            }
            else
            {
                if (((Canvas)DockingManager).Children.Contains(this))
                {
                    ((Canvas)DockingManager).Children.Remove(this);
                }
                Canvas.SetLeft(this, Canvas.GetLeft(windowContainer));
                Canvas.SetTop(this, Canvas.GetTop(windowContainer));
                this.Width = windowContainer.Width;
                this.Height = windowContainer.Height;
                Canvas.SetZIndex(this, ++currentZIndex);
                this.DockState = DockState.Float;
                if (this.Parent == null)
                {
                    ((Canvas)DockingManager).Children.Add(this);
                    this.ApplyBorderForFloatWindow();
                }

                this.Visibility = Visibility.Visible;
                for (int i = 0; i < this.FloatWindowTargetNameCollection.Count; i++)
                {
                    if (this.FloatWindowTargetNameCollection[i].OwnWindow != this)
                    {
                        this.FloatWindowTargetNameCollection[i].OwnWindow.floatWindowTargetName = this._Caption;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the float window from window container.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        protected internal Window GetFloatWindowFromWindowContainer(Window _windows, Window target)
        {
            Window temp = null;
            for (int i = 0; i < _windows.WindowCollection.Count; i++)
                {
                    if (_windows.WindowCollection[i].FloatWindowTargetNameCollection == target.FloatWindowTargetNameCollection)                    
                    {
                        temp = _windows.WindowCollection[i];
                        break;
                    }
                }
            return temp;
        }

        /// <summary>
        /// States the maintance.
        /// </summary>
        protected internal void StateMaintance()
        {
            if (this.OldValueDockManager != null && this._Caption != string.Empty)
            {                
                DockManager oldValue = this.DockManager;
                if (this.OldValueDockManager.Parent is WindowContainer)
                    {
                        Window windowContainer = (this.OldValueDockManager.Parent as WindowContainer)._window;
                        int numberofChildren = 0;//NumberofChildren
                        numberofChildren = NumberofFloatChildren(windowContainer, this.DockManager);
                        this.DockState = DockState.Hidden;
                        if (numberofChildren == 1)
                        {   
                            this.DockManager.DetachPaneEvents(this);
                            this.OldValueDockManager.AttachPaneEvents(this);    
                                windowContainer.Visibility = Visibility.Visible;
                                this.DockingManager.RemoveDock(windowContainer);
                                this.DockingManager.RemoveDock(this);
                                leastWindow = null;
                                Window temp = CheckFloatWindowIsPresent(windowContainer, this.DockManager);
                                //Window temp = leastWindow;
                                if (temp != null)
                                {
                                    //temp.DockState = DockState.Float;
                                    (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                    temp.DockManager = this.OldValueDockManager;
                                    temp.OldValueDockManager = oldValue;
                                    temp.Height = double.NaN;
                                    temp.Width = double.NaN;
                                    temp.DockState = DockState.Dock;
                                }

                                this.DockManager = this.OldValueDockManager;
                                this.OldValueDockManager = oldValue;
                                this.DockState = DockState.Dock;
                                (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                this.DockingManager.HideTabPanel(this);
                                this.DockingManager.HideTabPanel(temp);
                        }
                        else if ((this.OldValueDockManager.Parent as WindowContainer)._window.Visibility == Visibility.Visible)
                        {
                            this.DockState = DockState.Hidden;
                            (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            this.DockingManager.RemoveDock((this.OldValueDockManager.Parent as WindowContainer)._window);
                            this.DockingManager.RemoveDock(this);
                            //if (!(this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Contains(this))
                            //{
                            //    (this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Add(this);
                            //}
                            this.DockManager = this.OldValueDockManager;
                            this.OldValueDockManager = oldValue;
                            this.DockState = DockState.Dock;
                            (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                        }
                        else
                        {
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                this.DockState = DockState.Hidden;
                                (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                this.DockingManager.RemoveDock(this);
                                this.Width = (this.OldValueDockManager.Parent as WindowContainer)._window.Width;
                                this.Height = (this.OldValueDockManager.Parent as WindowContainer)._window.Height;
                                Canvas.SetZIndex(this, ++currentZIndex);
                                Canvas.SetLeft(this, Canvas.GetLeft((this.OldValueDockManager.Parent as WindowContainer)._window));
                                Canvas.SetTop(this, Canvas.GetTop((this.OldValueDockManager.Parent as WindowContainer)._window));
                                ((Canvas)DockingManager).Children.Add(this);
                                this.DockState = DockState.Float;
                                this.ApplyBorderForFloatWindow();
                                if (!(this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Contains(this))
                                {
                                    (this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Add(this);
                                }
                            }
                        }
                    }
                                 
            }            
        }


        /// <summary>
        /// States the maintance between container.
        /// </summary>
        protected internal void StateMaintanceBetweenContainer()
        {
            if (this.DockManager.Parent is WindowContainer)
            {
                Window w = (this.DockManager.Parent as WindowContainer)._window;
                if (NumberofChildren(w) <= 1)
                {
                    if (!w.WindowCollection.Contains(this))
                    {
                        w.WindowCollection.Add(this);
                    }
                    Window _w = GetNextWindowFromWindowContainer(w);
                    if (_w != null)
                    {
                        w.Visibility = Visibility.Visible;
                        this.Width = double.NaN;
                        this.Height = double.NaN;
                        _w.Width = double.NaN;
                        _w.Height = double.NaN;
                        ChangeState(DockState.Dock);
                        _w.ChangeState(DockState.Dock);

                        StateMaintanance st = this.CurrentStateMain;
                        this.CurrentStateMain = this.PreviousStateMain;
                        this.PreviousStateMain = st;

                        //st = _w.CurrentStateMain;
                        //_w.CurrentStateMain = _w.PreviousStateMain;
                        //_w.PreviousStateMain = st;

                    }
                    else
                    {
                        if (!((Canvas)DockingManager).Children.Contains(this))
                        {
                            Canvas.SetZIndex(this, ++currentZIndex);
                            //this.DockState = DockState.Float;
                            ((Canvas)DockingManager).Children.Add(this);
                            this.DockState = DockState.Float;
                            this.ApplyBorderForFloatWindow();
                            StateMaintanance st = this.CurrentStateMain;
                            this.CurrentStateMain = this.PreviousStateMain;
                            this.PreviousStateMain = st;
                        }
                    }

                }
                else if (w.WindowCollection.Count > 1)
                {
                    if (!w.WindowCollection.Contains(this))
                    {
                        w.WindowCollection.Add(this);
                    }
                    this.Width = double.NaN;
                    this.Height = double.NaN;
                    this.ChangeState(DockState.Dock);
                    StateMaintanance st = this.CurrentStateMain;
                    this.CurrentStateMain = this.PreviousStateMain;
                    this.PreviousStateMain = st;
                    if (this.DockManager.Parent is DockingManager)
                    {
                        this.DockingManager.ShowDockbutton(this);
                    }
                }
            }


            if (this.OldValueDockManager != null && this._Caption != string.Empty)
            {
                this.DockState = DockState.Hidden;
                (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                DockManager oldValue = this.DockManager;
                this.DockManager.DetachPaneEvents(this);
                this.OldValueDockManager.AttachPaneEvents(this);
                if (this.OldValueDockManager.Parent is WindowContainer)
                {
                    if ((this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Count == 2)
                    {
                        (this.OldValueDockManager.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                        this.DockingManager.ShowDockbutton(this);
                        if ((this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Contains(this))
                        {
                            (this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Remove(this);
                        }
                        this.DockManager = this.OldValueDockManager;
                        this.OldValueDockManager = oldValue;
                        this.DockState = DockState.Dock;
                        (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                        if (this.DockManager.Parent is DockingManager)
                        {
                            this.DockingManager.ShowDockbutton(this);
                        }


                    }
                    else if ((this.OldValueDockManager.Parent as WindowContainer)._window.Visibility == Visibility.Visible)
                    {
                        this.DockingManager.RemoveDock((this.OldValueDockManager.Parent as WindowContainer)._window);
                        this.DockingManager.RemoveDock(this);
                        if (!(this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Contains(this))
                        {
                            (this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Add(this);
                        }
                        this.DockManager = this.OldValueDockManager;
                        this.OldValueDockManager = oldValue;
                        this.DockState = DockState.Dock;
                        (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                        if (this.DockManager.Parent is DockingManager)
                        {
                            this.DockingManager.ShowDockbutton(this);
                        }
                    }
                    else
                    {
                        if (!((Canvas)DockingManager).Children.Contains(this))
                        {
                            this.DockState = DockState.Float;
                            this.DockingManager.RemoveDock(this);
                            this.Width = (this.OldValueDockManager.Parent as WindowContainer)._window.Width;
                            this.Height = (this.OldValueDockManager.Parent as WindowContainer)._window.Height;
                            Canvas.SetZIndex(this, ++currentZIndex);
                            Canvas.SetLeft(this, Canvas.GetLeft((this.OldValueDockManager.Parent as WindowContainer)._window));
                            Canvas.SetTop(this, Canvas.GetTop((this.OldValueDockManager.Parent as WindowContainer)._window));
                            ((Canvas)DockingManager).Children.Add(this);
                            this.ApplyBorderForFloatWindow();
                            if (!(this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Contains(this))
                            {
                                (this.OldValueDockManager.Parent as WindowContainer)._window.DuplicateWindowCollection.Add(this);
                            }
                        }
                    }
                }

            }
        }


        /// <summary>
        /// Changes the stateand dock manager.
        /// </summary>
        protected internal void ChangeStateandDockManager()
        {
                this.DockState = DockState.Hidden;
                (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                DockManager oldValue = this.DockManager;
                this.DockManager.DetachPaneEvents(this);
                this.OldValueDockManager.AttachPaneEvents(this);
                if (this.DockManager.Parent is WindowContainer)
                {   
                        this.DockManager = this.OldValueDockManager;
                        this.OldValueDockManager = oldValue;
                        this.Width = double.NaN;
                        this.Height = double.NaN;
                        this.DockState = DockState.Dock;
                        (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                        if (this.DockManager.Parent is DockingManager)
                        {
                            this.DockingManager.ShowDockbutton(this);
                        }
                }
                    

        }

        /// <summary>
        /// Changings the state after loaded.
        /// </summary>
        protected internal void ChangingStateAfterLoaded()
        {
            if (this.PreviousDockSide == Dock.Tabbed)
            {
                StateMaintanceForTabItem();
            }
            else ////if (this.PreviousDockSide == Dock.None)
            {
                Window w = CheckDockWindowPresented();
                if (w == null)
                {
                    this.Width = double.NaN;
                    this.Height = double.NaN;
                    if (((Canvas)DockingManager).Children.Contains(this))
                    {
                        ((Canvas)DockingManager).Children.Remove(this);
                       // this.DockingManager.ShowDockbutton(this);
                    }
                    ChangeState(DockState.Dock);
                    if (this.DockManager.Parent is DockingManager)
                    {
                        this.DockingManager.ShowDockbutton(this);
                    }
                    if (this.CustomTabControl != null)
                    {
                        if (this.CustomTabControl.Items.Count <= 1 && this.CustomTabControl.TabPanelBorder != null)
                        {
                            this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                            this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                        }
                        else if (this.CustomTabControl.Items.Count > 1 && this.CustomTabControl.TabPanelBorder != null)
                        {
                            this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                            this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    CheckTarGetNameCollection();
                }
            }
        }
    }
}
