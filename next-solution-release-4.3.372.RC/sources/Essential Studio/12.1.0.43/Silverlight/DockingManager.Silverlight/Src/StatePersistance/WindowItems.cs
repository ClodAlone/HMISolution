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
    /// Represents the Window items class.
    /// </summary>
    public class WindowItems
    {
        /// <summary>
        /// Gets or sets the _ caption.
        /// </summary>
        /// <value>The _ caption.</value>
        public string _Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the state of the dockable.
        /// </summary>
        /// <value>The state of the dockable.</value>
        public DockableState DockableState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the zindex for float.
        /// </summary>
        /// <value>The zindex for float.</value>
        public int ZindexForFloat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [parent is window container].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [parent is window container]; otherwise, <c>false</c>.
        /// </value>
        public bool ParentIsWindowContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current state main.
        /// </summary>
        /// <value>The current state main.</value>
        public StateMaintanance CurrentStateMain
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the previous state main.
        /// </summary>
        /// <value>The previous state main.</value>
        public StateMaintanance PreviousStateMain
        {
            get;
            set;
        }
       
        private List<string> _tarGetNameCollection;

        /// <summary>
        /// Gets or sets the Collection of CustomTab Items.
        /// </summary>
        /// <value>The target name collection.</value>
        public List<string> TargetNameCollection
        {
            get
            {
                return _tarGetNameCollection;
            }
            set
            {
                _tarGetNameCollection = value;
            }
        }


        /// <summary>
        /// Gets or sets the LeftPosition if the Float Window.
        /// </summary>
        /// <value>The left position.</value>
        public double LeftPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the TopPosition of the Float window .
        /// </summary>
        /// <value>The top position.</value>
        public double TopPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the Maximized state of a window
        /// </summary>
        public MaximizedState MaximizedState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the previous width of the window
        /// </summary>
        public double PreviousWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the previous height of the window
        /// </summary>
        public double PreviousHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the previous floating width of the window
        /// </summary>
        public double PreviousFloatWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the previous floating height of the window
        /// </summary>
        public double PreviousFloatHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the previous left location of the window
        /// </summary>
        public double PreviousLeftLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the previous top location of the window
        /// </summary>
        public double PreviousTopLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the TargetName of the Tabbed Window .
        /// </summary>
        /// <value>The target name in floating mode.</value>
        public string TargetNameInFloatingMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target name in docked mode.
        /// </summary>
        /// <value>The target name in docked mode.</value>
        public string TargetNameInDockedMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the side in docked mode.
        /// </summary>
        /// <value>The side in docked mode.</value>
        public Dock SideInDockedMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the side in float mode.
        /// </summary>
        /// <value>The side in float mode.</value>
        public Dock SideInFloatMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Left difference between the Grid .
        /// </summary>
        /// <value>The left diff.</value>
        public double LeftDiff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets  the Topdifference between the Grid.
        /// </summary>
        /// <value>The top diff.</value>
        public double TopDiff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Animation Width for the Float window.
        /// </summary>
        /// <value>The width of the animation.</value>
        public double AnimationWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Animation height of the Float Window.
        /// </summary>
        /// <value>The height of the animation.</value>
        public double AnimationHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can dock.
        /// </summary>
        /// <value><c>true</c> if this instance can dock; otherwise, <c>false</c>.</value>
        public bool CanDock
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// Stores the boolean value for Window can able to float
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool CanFloat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// Stores the Boolean value for Window can able to Close
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool CanClose
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool CanAutoHide
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// Stores the Boolean value for Window can able to Drag
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool CanDrag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Dock State .
        /// </summary>
        /// <value>The state of the dock.</value>
        public DockState DockState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dock position.
        /// </summary>
        /// <value>The dock position.</value>
        public Dock DockPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowItems"/> class.
        /// </summary>
        public WindowItems()
        {
        }



        /// <summary>
        /// Gets or sets the window container collection.
        /// </summary>
        /// <value>The window container collection.</value>
        public List<string> WindowContainerCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [window container presented].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [window container presented]; otherwise, <c>false</c>.
        /// </value>
        public bool WindowContainerPresented
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired height in docked mode.
        /// </summary>
        /// <value>The desired height in docked mode.</value>
        public double DesiredHeightInDockedMode//PaneHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired width in docked mode.
        /// </summary>
        /// <value>The desired width in docked mode.</value>
        public double DesiredWidthInDockedMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired height in float mode.
        /// </summary>
        /// <value>The desired height in float mode.</value>
        public double DesiredHeightInFloatMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired width in float mode.
        /// </summary>
        /// <value>The desired width in float mode.</value>
        public double DesiredWidthInFloatMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        /// <value>The name of the container.</value>
        public string ContainerName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets element type
        /// </summary>
        public string ElementType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the Caption
        /// </summary>
        public string Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the container left.
        /// </summary>
        /// <value>The container left.</value>
        public double ContainerLeft
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the container.
        /// </summary>
        /// <value>The height of the container.</value>
        public double ContainerHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the container.
        /// </summary>
        /// <value>The width of the container.</value>
        public double ContainerWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the container top.
        /// </summary>
        /// <value>The container top.</value>
        public double ContainerTop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the move window target.
        /// </summary>
        /// <value>The name of the move window target.</value>
        public string MoveWindowTargetName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the move dock position.
        /// </summary>
        /// <value>The move dock position.</value>
        public Dock MoveDockPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the container order.
        /// </summary>
        /// <value>The container order.</value>
        public int ContainerOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tar get name in float mode.
        /// </summary>
        /// <value>The tar get name in float mode.</value>
        public string TarGetNameInFloatMode
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the index of the container collection.
        /// </summary>
        /// <value>The index of the container collection.</value>
        public int ContainerCollectionIndex
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the dock tab order.
        /// </summary>
        /// <value>The dock tab order.</value>
        public int DockTabOrder
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the float tab order.
        /// </summary>
        /// <value>The float tab order.</value>
        public int FloatTabOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selected tab item.
        /// </summary>
        /// <value>The selected tab item.</value>
        public string SelectedTabItem
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowItems"/> class.
        /// </summary>
        /// <param name="w">The w.</param>
        public WindowItems(Window w)
        {
            _tarGetNameCollection = new List<string>();
            this.WindowContainerCollection = new List<string>();
            this.ContainerName = string.Empty;
            this.AnimationHeight = w.AnimationHeight;
            this.AnimationWidth = w.AnimationWidth;
            this.CanAutoHide = w.CanAutoHide;
            this.CanClose = w.CanClose;
            this.CanDock = w.CanDock;
            this.MaximizedState = w.MaximizedState;
            this.PreviousHeight = w.PreviousHeight;
            this.PreviousWidth = w.PreviousWidth;
            this.PreviousFloatWidth = w.PreviousFloatWidth;
            this.PreviousFloatHeight = w.PreviousFloatHeight;
            this.PreviousLeftLocation = w.PreviousLeftLocation;
            this.PreviousTopLocation = w.PreviousTopLocation;
            if (w.WindowChildElement != null)
            {
                this.Caption = w.Caption;
                this.ElementType = w.WindowChildElement.GetType().ToString();
            }

            if (w.Height.ToString() == "NaN" || w.Height == 0.0 || w.DockState == DockState.AutoHidden || w.DockState == DockState.Hidden)
           {
               if (w.DockingManager != null && w.WindowChildElement != null)
               {
                   bool allowSizeContent = DockingManager.GetSizeToContent(w.DockingManager);
                   if (allowSizeContent)
                   {
                       allowSizeContent = DockingManager.GetSizeToContent(w.WindowChildElement);
                   }
                   if (!allowSizeContent)
                   {
                       if (DockingManager.GetFloatingWindowRect(w.WindowChildElement) != Rect.Empty)
                       {
                           Rect floatingRect = DockingManager.GetFloatingWindowRect(w.WindowChildElement);
                           if (floatingRect.Height > 0 && floatingRect.Width > 0)
                           {
                               w.FloatHeight = floatingRect.Height;
                               w.FloatWidth = floatingRect.Width;
                           }
                           else
                           {
                               w.FloatHeight = 200;
                               w.FloatWidth = 200;
                           }
                           w.LeftPosition = floatingRect.Left;
                           w.TopPosition = floatingRect.Top;
                       }
                       else
                       {
                           //w.FloatHeight = 0;
                           //w.FloatWidth = 0;
                       }
                   }
                   else
                   { 
                       if ((w.WindowChildElement as FrameworkElement).Height != 0.0 && (w.WindowChildElement as FrameworkElement).Height.ToString() != "NaN")
                       {
                           w.FloatHeight = (w.WindowChildElement as FrameworkElement).Height;
                       }
                       else
                       {
                           w.FloatHeight = 200;
                       }
                       if ((w.WindowChildElement as FrameworkElement).Width != 0.0 && (w.WindowChildElement as FrameworkElement).Width.ToString() != "NaN")
                       {
                           w.FloatWidth = (w.WindowChildElement as FrameworkElement).Width;
                       }
                       else
                       {
                           w.FloatWidth = 200;
                       }
                   }
               }
                this.DesiredHeightInFloatMode = w.FloatHeight;
                this.DesiredWidthInFloatMode = w.FloatWidth;
            }
            else
            {
                this.DesiredHeightInFloatMode = w.Height;
                this.DesiredWidthInFloatMode = w.Width;
            }
            
            this.CanDrag = w.CanDrag;
            this.CanFloat = w.CanFloat;            
            this.CurrentStateMain = w.CurrentStateMain;
            if ((w.DockState == DockState.Dock || w.DockState == DockState.Float) && !((Canvas)w.DockingManager).Children.Contains(w) && w.Visibility == Visibility.Visible)
            {
                if (w.Parent is Grid)
                {
                    this.DesiredHeightInDockedMode = (w.Parent as Grid).ActualHeight;
                    this.DesiredWidthInDockedMode = (w.Parent as Grid).ActualWidth;
                }
                else
                {
                    this.DesiredHeightInDockedMode = w.PaneHeight;
                    this.DesiredWidthInDockedMode = w.PaneWidth;
                }
            }
            else
            {
                this.DesiredHeightInDockedMode = w.PaneHeight;
                this.DesiredWidthInDockedMode = w.PaneWidth;
            }
            this.DesiredHeightInDockedMode = w.PaneHeight;
            this.DesiredWidthInDockedMode = w.PaneWidth;
            this._Caption = w._Caption;
            this.DockableState = w.DockableState;
            DockManager dm = null;
            if (w.DockManager.Parent is WindowContainer)
            {
                dm = w.DockManager;
            }
            else
            {
                if (w.OldValueDockManager != null)
                {
                    if (w.OldValueDockManager.Parent is WindowContainer)
                    {
                        dm = w.OldValueDockManager;
                    }
                }
            }
            if (dm != null)
            {
                Window _w = (dm.Parent as WindowContainer)._window;
                
                    WindowContainerPresented = true;

                    this.ContainerName = (dm.Parent as WindowContainer).ContainerName;
                    this.ContainerLeft = Canvas.GetLeft(_w);
                    this.ContainerTop = Canvas.GetTop(_w);
                    ContainerHeight = _w.Height;
                    ContainerWidth = _w.Width;
                    if (_w.WindowCollection.Contains(w))
                    {
                        this.ContainerCollectionIndex = _w.WindowCollection.IndexOf(w);
                    }
                    for (int i = 0; i < _w.WindowCollection.Count; i++)
                    {
                        this.WindowContainerCollection.Add(_w.WindowCollection[i]._Caption);
                    }
            }     
            this.DockPosition = w.DockPosition;
            this.DockState = DockingManager.GetDockState(w.WindowChildElement);             
            this.TargetNameInDockedMode = DockingManager.GetTargetNameInDockedMode(w.WindowChildElement);
            this.TargetNameInFloatingMode = DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement);
            this.SideInDockedMode = DockingManager.GetSideInDockedMode(w.WindowChildElement);
            CustomTabItem cstabItem = null;
            CustomTabControl csTabControl = null;
            if ((w.WindowChildElement as FrameworkElement).Parent is CustomTabItem)
            {
                cstabItem = (w.WindowChildElement as FrameworkElement).Parent as CustomTabItem;
                if (cstabItem.Parent is CustomTabControl)
                {
                    csTabControl = cstabItem.Parent as CustomTabControl;
                }
            }
            if (cstabItem != null && csTabControl != null && csTabControl.Items.Contains(cstabItem))
            {
                if (this.DockState != DockState.Float)
                {
                    DockTabOrder = csTabControl.Items.IndexOf(cstabItem);
                }
                else
                {
                    FloatTabOrder = csTabControl.Items.IndexOf(cstabItem);
                }
            }
            this.SideInFloatMode = DockingManager.GetSideInFloatMode(w.WindowChildElement);
            if (w.DockState == DockState.Float && ((Canvas)w.DockingManager).Children.Contains(w))
            {
                this.ZindexForFloat = Canvas.GetZIndex(w);
                this.TopPosition = Canvas.GetTop(w);
                this.LeftPosition = Canvas.GetLeft(w);
            }
            else if (((Canvas)w.DockingManager).Children.Contains(w) && w.Visibility == Visibility.Visible)
            {
                if (w.CustomTabControl != null)
                {
                    if (w.CustomTabControl.Items.Count > 0)
                    {
                        //this.DockState = DockState.Float;
                        this.ZindexForFloat = Canvas.GetZIndex(w);
                        this.TopPosition = Canvas.GetTop(w);
                        this.LeftPosition = Canvas.GetLeft(w);
                    }
                }
            }
            else
            {
                this.TopPosition = w.TopPosition;
                this.LeftPosition = w.LeftPosition;
            }
                                
            this.MoveDockPosition = w.MoveDockPosition;
            this.MoveWindowTargetName = w.MoveWindowTargetName;            
            DockingGrid dg = w.DockingManager.GetParentDockManager();
            this.Order = (w.DockManager.Children[0] as DockingGrid).GetOrderofGroup(w);
            if (w.DockManager.Parent is WindowContainer)
            {
                this.ContainerOrder = (w.DockManager.Children[0] as DockingGrid).GetOrderofGroup(w);
            }
            else if (w.OldValueDockManager != null)
            {
                if (w.OldValueDockManager.Parent is WindowContainer)
                {
                    this.ContainerOrder = (w.OldValueDockManager.Children[0] as DockingGrid).GetOrderofGroup(w);
                }
            }
            if (w.CustomTabControl != null)
            {
                if (w.CustomTabControl.Items.Count > 1)
                {
                    SelectedTabItem = (w.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow._Caption;
                }
            }
        }
    }
}
