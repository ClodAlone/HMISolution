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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Class Dockable Pane Group.
    /// </summary>
    class DockablePaneGroup
    {
        /// <summary>
        /// The Attached Pane Window.
        /// </summary>
        protected internal Window _attachedPane;

        /// <summary>
        /// 
        /// </summary>
        protected internal Window resizingWindow1;

        /// <summary>
        /// 
        /// </summary>
        protected internal Window resizingWindow2;

        /// <summary>
        /// 
        /// </summary>
        protected internal ColumnDefinition columnDefinition1;

        /// <summary>
        /// 
        /// </summary>
        protected internal ColumnDefinition columnDefinition2;

        /// <summary>
        /// 
        /// </summary>
        protected internal Grid resizingGrid1;

        /// <summary>
        /// 
        /// </summary>
        protected internal Grid resizingGrid2;

        /// <summary>
        /// 
        /// </summary>
        protected internal RowDefinition rowDefinition1;

        /// <summary>
        /// 
        /// </summary>
        protected internal RowDefinition rowDefinition2;

        /// <summary>
        /// 
        /// </summary>
        protected internal Window resizingRowWindow1;

        /// <summary>
        /// 
        /// </summary>
        protected internal Window resizingRowWindow2;

        /// <summary>
        /// 
        /// </summary>
        protected internal Grid resizingRowGrid1;

        /// <summary>
        /// 
        /// </summary>
        protected internal Grid resizingRowGrid2;

        /// <summary>
        /// Gets the attached pane.
        /// </summary>
        /// <value>The attached pane.</value>
        public Window AttachedPane
        {
            get
            {
                return _attachedPane;
            }
        }

        /// <summary>
        /// The first Child group.
        /// </summary>
        protected internal DockablePaneGroup _firstChildGroup;

        /// <summary>
        /// Gets or sets the Dockable PaneGroup .
        /// </summary>
        /// <value>The first child group.</value>
        public DockablePaneGroup FirstChildGroup
        {
            get
            {
                return _firstChildGroup;
            }

            set
            {
                _firstChildGroup = value;
                value._parentGroup = this;
            }
        }

        /// <summary>
        /// The Second Child Group.
        /// </summary>
        protected internal DockablePaneGroup _secondChildGroup;

        /// <summary>
        /// Gets or sets the Dockable pane Group's first Element .
        /// </summary>
        /// <value>The first.</value>
        protected internal DockablePaneGroup First
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Dockable pane Group's Second Element.
        /// </summary>
        /// <value>The second.</value>
        protected internal DockablePaneGroup second
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second child group.
        /// </summary>
        /// <value>The second child group.</value>
        public DockablePaneGroup SecondChildGroup
        {
            get
            {
                return _secondChildGroup;
            }

            set
            {
                _secondChildGroup = value;
                value._parentGroup = this;
            }
        }

        /// <summary>
        /// Represents the Parent Group.
        /// </summary>
        DockablePaneGroup _parentGroup;

        /// <summary>
        /// Gets or sets the parent group.
        /// </summary>
        /// <value>The parent group.</value>
        public DockablePaneGroup ParentGroup
        {
            get
            {
                return _parentGroup;
            }

            internal set
            {
                _parentGroup = value;
            }
        }

        /// <summary>
        /// Represents the dock.
        /// </summary>
        protected internal Dock _dock;

        /// <summary>
        /// Gets the dock.
        /// </summary>
        /// <value>The dock.</value>
        public Dock Dock
        {
            get
            {
                return _dock;
            }
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
        /// Initializes a new instance of the <see cref="DockablePaneGroup"/> class.
        /// </summary>
        public DockablePaneGroup()
        {
            Order = ++s;
        }

        /// <summary>
        /// Represents the value s.
        /// </summary>
        public static int s = 0;


        /// <summary>
        /// Initializes a new instance of the <see cref="DockablePaneGroup"/> class.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="rootgrid">The rootgrid.</param>
        public DockablePaneGroup(Window pane, DockingGrid rootgrid)
        {
            _attachedPane = pane;
            Order = ++s;
            rootGrid = rootgrid;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DockablePaneGroup"/> class.
        /// </summary>
        /// <param name="firstGroup">The first group.</param>
        /// <param name="secondGroup">The second group.</param>
        /// <param name="groupDock">The group dock.</param>
        public DockablePaneGroup(DockablePaneGroup firstGroup, DockablePaneGroup secondGroup, Dock groupDock)
        {
            FirstChildGroup = firstGroup;
            SecondChildGroup = secondGroup;
            _dock = groupDock;
        }

        /// <summary>
        /// Gets the width of the group.
        /// </summary>
        /// <value>The width of the group.</value>
        GridLength GroupWidth
        {
            get
            {
                if (AttachedPane != null)
                {
                    if (AttachedPane.PaneWidth > 4)
                    {
                        return new GridLength(AttachedPane.PaneWidth, GridUnitType.Pixel);
                    }
                    else
                    {
                        return new GridLength(5, GridUnitType.Pixel);
                    }
                }
                else
                {
                    if (Dock == Dock.Left || Dock == Dock.Right)
                    {
                        return new GridLength(FirstChildGroup.GroupWidth.Value + SecondChildGroup.GroupWidth.Value + 4, GridUnitType.Pixel);
                    }
                    else
                    {
                        if (FirstChildGroup.GroupWidth.Value > SecondChildGroup.GroupWidth.Value)
                            return FirstChildGroup.GroupWidth;
                        else
                            return SecondChildGroup.GroupWidth;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the height of the group.
        /// </summary>
        /// <value>The height of the group.</value>
        GridLength GroupHeight
        {
            get
            {
                if (AttachedPane != null)
                {
                    if (AttachedPane.PaneHeight > 4.0)
                    {
                        return new GridLength(AttachedPane.PaneHeight, GridUnitType.Pixel);
                    }
                    else
                    {
                        return new GridLength(5, GridUnitType.Pixel);
                    }
                }
                else
                {
                    if (Dock == Dock.Top || Dock == Dock.Bottom)
                    {
                        return new GridLength(FirstChildGroup.GroupHeight.Value + SecondChildGroup.GroupHeight.Value + 4, GridUnitType.Pixel);
                    }
                    else
                    {
                        if (FirstChildGroup.GroupHeight.Value > SecondChildGroup.GroupHeight.Value)
                            return FirstChildGroup.GroupHeight;
                        else
                            return SecondChildGroup.GroupHeight;
                    }
                }
            }
        }


        /// <summary>
        /// Gets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value>Returns<c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden
        {
            get
            {
                if (AttachedPane != null)
                {
                    if (!AttachedPane.IsHidden)
                    {
                        if (AttachedPane.DockManager.Children[0] as DockingGrid == rootGrid)
                        {
                            if (AttachedPane.AlreadyExisting)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return AttachedPane.IsHidden;
                    }
                }

                return FirstChildGroup.IsHidden && SecondChildGroup.IsHidden;
            }
        }

        /// <summary>
        /// Gets or sets the color of the splitter back ground.
        /// </summary>
        /// <value>The color of the splitter back ground.</value>
        private Brush SplitterBackGroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the Root Grid.
        /// </summary>
        protected internal DockingGrid rootGrid = null;

        /// <summary>
        /// Arranges the specified grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="dg">The dg.</param>
        public void Arrange(Grid grid, DockingGrid dg)
        {
            if (AttachedPane != null)
            {
                bool allowme = false;
                if (AttachedPane.DockState == DockState.Dock)
                {
                    allowme = true;
                }
                else
                {
                    if (AttachedPane.DockState == DockState.Float && AttachedPane.DockManager != null && AttachedPane.DockManager.Parent is WindowContainer)
                    {
                        //if (DockingManager.GetTargetNameInFloatingMode(AttachedPane.WindowChildElement) != string.Empty || AttachedPane.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(AttachedPane.WindowChildElement)) != null)
                        //{
                        allowme = true;
                        //}
                    }
                }
                ////AttachedPane.IsHidden)
                if (allowme && AttachedPane.DockManager.Children[0] as DockingGrid == dg)
                {
                    Canvas.SetLeft(AttachedPane, 0);
                    Canvas.SetTop(AttachedPane, 0);
                    Canvas.SetZIndex(AttachedPane, 1);

                    if (AttachedPane.Parent != null)
                    {
                        if (AttachedPane.Parent is DockingManager && AttachedPane.GetType() == typeof(Window))
                        {
                            if (dg._dockManager.Parent is WindowContainer)
                            {
                                if (!(dg._dockManager.Parent as WindowContainer).handledLater)
                                {
                                    try
                                    {
                                        ((Canvas)AttachedPane.DockingManager).Children.Remove(AttachedPane);
                                        if (AttachedPane.DockState == DockState.Dock && AttachedPane.DockingManager != null)
                                        {
                                            AttachedPane.DockingManager.ShowDockbutton(AttachedPane);
                                        }
                                        grid.Children.Add(AttachedPane);
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    if ((dg._dockManager.Parent as WindowContainer).handledLater)
                                    {
                                        Canvas.SetLeft(AttachedPane, AttachedPane.LeftPosition);
                                        Canvas.SetTop(AttachedPane, AttachedPane.TopPosition);
                                        Canvas.SetZIndex(AttachedPane, AttachedPane.ZindexOrder);
                                        AttachedPane.Height = AttachedPane.DesiredHeightInFloatMode;
                                        AttachedPane.Width = AttachedPane.DesiredWidthInFloatMode;
                                        AttachedPane.ApplyBorderForFloatWindow();
                                    }
                                }
                            }
                            else
                            {
                                ((Canvas)AttachedPane.DockingManager).Children.Remove(AttachedPane);
                                AttachedPane.ApplyDockStyle();
                                if (AttachedPane.DockState == DockState.Dock && AttachedPane.DockingManager != null)
                                {
                                    AttachedPane.DockingManager.ShowDockbutton(AttachedPane);
                                }
                                grid.Children.Add(AttachedPane);
                            }
                        }
                    }
                    else if (grid != null)
                    {
                        if (AttachedPane.DockState == DockState.Dock && AttachedPane.DockingManager != null)
                        {
                            AttachedPane.DockingManager.ShowDockbutton(AttachedPane);
                        }
                        grid.Children.Add(AttachedPane);
                        if (AttachedPane.DockingManager != null)
                        {
                            if (AttachedPane.DockingManager.SplitterBackGroundColor != null)
                            {
                                SplitterBackGroundColor = AttachedPane.DockingManager.SplitterBackGroundColor;
                            }
                        }
                    }
                }
            }
            else if (FirstChildGroup.IsHidden && !SecondChildGroup.IsHidden)
            {
                SecondChildGroup.Arrange(grid, dg);
            }
            else if (!FirstChildGroup.IsHidden && SecondChildGroup.IsHidden)
            {
                FirstChildGroup.Arrange(grid, dg);
            }
            else
            {
                if (grid != null)
                {
                    if (Dock == Dock.Left || Dock == Dock.Right)
                    {
                        grid.RowDefinitions.Add(new RowDefinition());
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        ////grid.ColumnDefinitions[0].Width = (Dock == Dock.Left) ? new GridLength(AttachedPane.PaneWidth) : new GridLength(1, GridUnitType.Star);
                        ////grid.ColumnDefinitions[1].Width = (Dock == Dock.Right) ? new GridLength(AttachedPane.PaneWidth) : new GridLength(1, GridUnitType.Star);
                        grid.ColumnDefinitions[0].Width = (Dock == Dock.Left) ? FirstChildGroup.GroupWidth : new GridLength(1, GridUnitType.Star);
                        grid.ColumnDefinitions[1].Width = (Dock == Dock.Right) ? SecondChildGroup.GroupWidth : new GridLength(1, GridUnitType.Star);
                        grid.ColumnDefinitions[0].MinWidth = 20;
                        grid.ColumnDefinitions[1].MinWidth = 20;
                        Grid firstChildGrid = new Grid();
                        ////firstChildGrid.ShowGridLines = true;
                        ////firstChildGrid.Background = new SolidColorBrush(Colors.Red);
                        firstChildGrid.SetValue(Grid.ColumnProperty, 0);
                        firstChildGrid.Margin = new Thickness(0, 0, 4, 0);
                        FirstChildGroup.Arrange(firstChildGrid, dg);
                        grid.Children.Add(firstChildGrid);
                        Grid secondChildGrid = new Grid();
                        ////secondChildGrid.ShowGridLines = true;
                        ////secondChildGrid.Background = new SolidColorBrush(Colors.Red);
                        secondChildGrid.SetValue(Grid.ColumnProperty, 1);
                        ////secondChildGrid.Margin = (Dock == Dock.Right) ? new Thickness(0, 0, 4, 0) : new Thickness();
                        SecondChildGroup.Arrange(secondChildGrid, dg);
                        grid.Children.Add(secondChildGrid);
                        ////AttachedPane.SetValue(Grid.ColumnProperty, (Dock == Dock.Right) ? 1 : 0);
                        ////AttachedPane.Margin = (Dock == Dock.Left) ? new Thickness(0, 0, 4, 0) : new Thickness();
                        ////grid.Children.Add(AttachedPane);
                        CustomGridSplitter splitter = new CustomGridSplitter();
                        splitter.MouseLeftButtonDown += new MouseButtonEventHandler(Widthsplitter_MouseLeftButtonDown);
                        splitter.MouseLeftButtonUp += new MouseButtonEventHandler(Widthsplitter_MouseLeftButtonUp);
                        splitter.MouseMove += new MouseEventHandler(Widthsplitter_MouseMove);
                        splitter.Width = 4;
                        splitter.HorizontalAlignment = HorizontalAlignment.Right;
                        splitter.VerticalAlignment = VerticalAlignment.Stretch;
                        splitter.Background = new SolidColorBrush(Colors.Transparent);
                        splitter.BorderBrush = new SolidColorBrush(Colors.Transparent);
                        if (dg._dockManager.DockingParent.FreezeLayout)
                        {
                            splitter.IsEnabled = false;
                        }
                        else
                        {
                            if (firstChildGrid.Children.Count == 1 && firstChildGrid.Children[0] is Window)
                            {
                                if ((firstChildGrid.Children[0] as Window).WindowChildElement != null)
                                {
                                    if (!DockingManager.GetCanResize((firstChildGrid.Children[0] as Window).WindowChildElement))
                                        splitter.IsEnabled = false;
                                }
                            }
                            if (secondChildGrid.Children.Count == 1 && secondChildGrid.Children[0] is Window)
                            {
                                if ((secondChildGrid.Children[0] as Window).WindowChildElement != null)
                                {
                                    if (!DockingManager.GetCanResize((secondChildGrid.Children[0] as Window).WindowChildElement))
                                        splitter.IsEnabled = false;
                                }
                            }
                            if (firstChildGrid.Children.Count == 1 && firstChildGrid.Children[0] is Window && secondChildGrid.Children.Count == 1 && secondChildGrid.Children[0] is Window)
                            {
                                if ((firstChildGrid.Children[0] as Window).WindowChildElement != null && (secondChildGrid.Children[0] as Window).WindowChildElement != null)
                                {
                                    if (DockingManager.GetCanResize((firstChildGrid.Children[0] as Window).WindowChildElement) && DockingManager.GetCanResize((secondChildGrid.Children[0] as Window).WindowChildElement))
                                    {
                                        if (!splitter.IsEnabled)
                                            splitter.IsEnabled = true;
                                    }
                                }
                            }
                            if (firstChildGrid.Children.Count == 3)
                            {
                                if (!(firstChildGrid.Children[2] as GridSplitter).IsEnabled)
                                {
                                    for (int i = 0; i <= 1; i++)
                                    {
                                        if (firstChildGrid.Children[i] is Grid)
                                        {
                                            Grid grd = firstChildGrid.Children[i] as Grid;
                                            if (grd.Children[0] is Window)
                                            {
                                                if ((grd.Children[0] as Window).WindowChildElement != null)
                                                {
                                                    if (!DockingManager.GetCanResize((grd.Children[0] as Window).WindowChildElement))
                                                        splitter.IsEnabled = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (secondChildGrid.Children.Count == 3)
                            {
                                if (!(secondChildGrid.Children[2] as GridSplitter).IsEnabled)
                                {
                                    for (int i = 0; i <= 1; i++)
                                    {
                                        if (secondChildGrid.Children[i] is Grid)
                                        {
                                            Grid grd = secondChildGrid.Children[i] as Grid;
                                            if (grd.Children[0] is Window && (grd.Children[0] as Window).WindowChildElement != null)
                                            {
                                                if (!DockingManager.GetCanResize((grd.Children[0] as Window).WindowChildElement))
                                                    splitter.IsEnabled = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        grid.Children.Add(splitter);
                    }
                    else
                    {
                        //// if (Dock == Dock.Top || Dock == Dock.Bottom)
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        grid.RowDefinitions.Add(new RowDefinition());
                        grid.RowDefinitions.Add(new RowDefinition());
                        ////grid.RowDefinitions[0].Height = (Dock == Dock.Top) ? new GridLength(AttachedPane.PaneHeight) : new GridLength(1, GridUnitType.Star);
                        ////grid.RowDefinitions[1].Height = (Dock == Dock.Bottom) ? new GridLength(AttachedPane.PaneHeight) : new GridLength(1, GridUnitType.Star);
                        grid.RowDefinitions[0].Height = (Dock == Dock.Top) ? FirstChildGroup.GroupHeight : new GridLength(1, GridUnitType.Star);
                        grid.RowDefinitions[1].Height = (Dock == Dock.Bottom) ? SecondChildGroup.GroupHeight : new GridLength(1, GridUnitType.Star);
                        grid.RowDefinitions[0].MinHeight = 20;
                        grid.RowDefinitions[1].MinHeight = 20;
                        Grid firstChildGrid = new Grid();
                        ////firstChildGrid.SetValue(Grid.RowProperty, (Dock == Dock.Bottom) ? 1 : 0);
                        firstChildGrid.SetValue(Grid.RowProperty, 0);
                        ////firstChildGrid.ShowGridLines = true;
                        ////firstChildGrid.Background = new SolidColorBrush(Colors.Red);
                        ////firstChildGrid.Margin = (Dock == Dock.Bottom) ? new Thickness(0, 0, 0, 4) : new Thickness();
                        firstChildGrid.Margin = new Thickness(0, 0, 0, 4);
                        FirstChildGroup.Arrange(firstChildGrid, dg);
                        grid.Children.Add(firstChildGrid);
                        Grid secondChildGrid = new Grid();
                        ////secondChildGrid.ShowGridLines = true;
                        ////secondChildGrid.Background = new SolidColorBrush(Colors.Red);
                        ////secondChildGrid.SetValue(Grid.RowProperty, (Dock == Dock.Top) ? 1 : 0);
                        secondChildGrid.SetValue(Grid.RowProperty, 1);
                        ////secondChildGrid.Margin = (Dock == Dock.Bottom) ? new Thickness(0, 0, 0, 4) : new Thickness();
                        SecondChildGroup.Arrange(secondChildGrid, dg);
                        grid.Children.Add(secondChildGrid);
                        ////AttachedPane.SetValue(Grid.RowProperty, (Dock == Dock.Bottom) ? 1 : 0);
                        ////AttachedPane.Margin = (Dock == Dock.Top) ? new Thickness(0, 0, 0, 4) : new Thickness();
                        ////grid.Children.Add(AttachedPane);
                        CustomGridSplitter splitter = new CustomGridSplitter();
                        splitter.MouseLeftButtonDown += new MouseButtonEventHandler(Heightsplitter_MouseLeftButtonDown);
                        splitter.MouseLeftButtonUp += new MouseButtonEventHandler(Heightsplitter_MouseLeftButtonUp);
                        splitter.MouseMove += new MouseEventHandler(Heightsplitter_MouseMove);
                        splitter.Height = 4;
                        splitter.Background = new SolidColorBrush(Colors.Transparent);
                        splitter.BorderBrush = new SolidColorBrush(Colors.Transparent);
                        splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                        splitter.VerticalAlignment = VerticalAlignment.Bottom;
                        if (dg._dockManager.DockingParent.FreezeLayout)
                        {
                            splitter.IsEnabled = false;
                        }
                        else
                        {
                            if (firstChildGrid.Children.Count == 1 && firstChildGrid.Children[0] is Window)
                            {
                                if ((firstChildGrid.Children[0] as Window).WindowChildElement != null)
                                {
                                    if (!DockingManager.GetCanResize((firstChildGrid.Children[0] as Window).WindowChildElement))
                                        splitter.IsEnabled = false;
                                }
                            }
                            if (secondChildGrid.Children.Count == 1 && secondChildGrid.Children[0] is Window)
                            {
                                if ((secondChildGrid.Children[0] as Window).WindowChildElement != null)
                                {
                                    if (!DockingManager.GetCanResize((secondChildGrid.Children[0] as Window).WindowChildElement))
                                        splitter.IsEnabled = false;
                                }
                            }
                            if (firstChildGrid.Children.Count == 1 && firstChildGrid.Children[0] is Window && secondChildGrid.Children.Count == 1 && secondChildGrid.Children[0] is Window)
                            {
                                if ((firstChildGrid.Children[0] as Window).WindowChildElement != null && (secondChildGrid.Children[0] as Window).WindowChildElement != null)
                                {
                                    if (DockingManager.GetCanResize((firstChildGrid.Children[0] as Window).WindowChildElement) && DockingManager.GetCanResize((secondChildGrid.Children[0] as Window).WindowChildElement))
                                    {
                                        if (!splitter.IsEnabled)
                                            splitter.IsEnabled = true;
                                    }
                                }
                            }
                            if (firstChildGrid.Children.Count == 3)
                            {
                                if (!(firstChildGrid.Children[2] as GridSplitter).IsEnabled)
                                {
                                    for (int i = 0; i <= 1; i++)
                                    {
                                        if (firstChildGrid.Children[i] is Grid)
                                        {
                                            Grid grd = firstChildGrid.Children[i] as Grid;
                                            if (grd.Children[0] is Window && (grd.Children[0] as Window).WindowChildElement != null)
                                            {
                                                if (!DockingManager.GetCanResize((grd.Children[0] as Window).WindowChildElement))
                                                    splitter.IsEnabled = false;
                                            }
                                        }
                                    }
                                }
                            }
                            if (secondChildGrid.Children.Count == 3)
                            {
                                if (!(secondChildGrid.Children[2] as GridSplitter).IsEnabled)
                                {
                                    for (int i = 0; i <= 1; i++)
                                    {
                                        if (secondChildGrid.Children[i] is Grid)
                                        {
                                            Grid grd = secondChildGrid.Children[i] as Grid;
                                            if (grd.Children[0] is Window && (grd.Children[0] as Window).WindowChildElement != null)
                                            {
                                                if (!DockingManager.GetCanResize((grd.Children[0] as Window).WindowChildElement))
                                                    splitter.IsEnabled = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        grid.Children.Add(splitter);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the docking grid.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns>The Dock Manager.</returns>
        protected internal DockManager GetDockingGrid(UIElement elem)
        {
            if (elem != null)
            {
                UIElement tempDockingGrid = (UIElement)VisualTreeHelper.GetParent(elem);
                if (tempDockingGrid != null && tempDockingGrid.GetType() == typeof(DockManager))
                {
                    return (DockManager)tempDockingGrid;
                }
                else
                {
                    tempDockingGrid = GetDockingGrid(tempDockingGrid);
                    return (DockManager)tempDockingGrid;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Updatepanes the height.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void UpdatepaneHeight(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    UpdatepaneHeight(child as Grid);
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
                    }
                }
            }
        }

        /// <summary>
        /// Updatepanes the width.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void UpdatepaneWidth(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    UpdatepaneWidth(child as Grid);
                }
                else if (child is Window)
                {
                    Window w = child as Window;
                    if (w.Parent is Grid)
                    {
                        if ((w.Parent as Grid).ActualWidth > 0)
                        {
                            w.PaneWidth = (w.Parent as Grid).ActualWidth;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Heightsplitter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void Heightsplitter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)((GridSplitter)sender).Parent;
            DockManager dm = GetDockingGrid(parentGrid);
            UpdatepaneHeight(dm.gridDocking.gridDocking);
            ClearMaxColumnWidthandRow(dm.gridDocking.gridDocking);

            if (rowDefinition1 != null)
            {
                rowDefinition1.MinHeight = 20;
                rowDefinition1 = null;
            }

            if(rowDefinition2 != null)
            {
                rowDefinition2.MinHeight = 20;
                rowDefinition2 = null;
            }

            if(resizingRowWindow2 != null)
            {
                resizingRowWindow2.SizeChanged -= new SizeChangedEventHandler(resizingRowWindow2_SizeChanged);
                resizingRowWindow2 = null;
            }

            if (resizingRowWindow1 != null)
            {
                resizingRowWindow1.SizeChanged -= new SizeChangedEventHandler(resizingRowWindow1_SizeChanged);
                resizingRowWindow1 = null;
            }

            if(resizingRowGrid1 != null)
            {
                resizingRowGrid1.SizeChanged -= new SizeChangedEventHandler(resizingRowGrid1_SizeChanged);
                resizingRowGrid1 = null;
            }

            if (resizingRowGrid2 != null)
            {
                resizingRowGrid2.SizeChanged -= new SizeChangedEventHandler(resizingRowGrid2_SizeChanged);
                resizingRowGrid2 = null;
            }

            (sender as GridSplitter).Background = new SolidColorBrush(Colors.Transparent);
            (sender as GridSplitter).BorderBrush = new SolidColorBrush(Colors.Transparent);
            (sender as GridSplitter).BorderThickness = new Thickness(0);
        }

        /// <summary>
        /// Value indicating whether loaded or not.
        /// </summary>
        private bool _loaded = false;

        /// <summary>
        /// Handles the MouseMove event of the Widthsplitter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void Widthsplitter_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && !_loaded)
            {
                Grid parentGrid = (Grid)((GridSplitter)sender).Parent;
                if ((parentGrid.Children[1] as Grid).Children.Count > 0)
                {
                    if ((parentGrid.Children[1] as Grid).Children[0].GetType() == typeof(Window))
                    {
                        Window temp = (parentGrid.Children[1] as Grid).Children[0] as Window;
                        if (temp.DockManager != null)
                        {
                            //(temp.DockManager.Children[0] as DockingGrid).ClearColumnWidth(parentGrid);
                        }
                        _loaded = true;
                        isDragging = false;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the Heightsplitter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void Heightsplitter_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && !_loaded)
            {
                Grid parentGrid = (Grid)((GridSplitter)sender).Parent;
                if ((parentGrid.Children[1] as Grid).Children.Count > 0)
                {
                    if ((parentGrid.Children[1] as Grid).Children[0].GetType() == typeof(Window))
                    {
                        Window temp = (parentGrid.Children[1] as Grid).Children[0] as Window;
                        if (temp.DockManager != null)
                        {
                            //(temp.DockManager.Children[0] as DockingGrid).ClearRowHeight(parentGrid);
                        }
                        _loaded = true;
                        isDragging = false;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the max column widthand row.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void ClearMaxColumnWidthandRow(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    ClearMaxColumnWidthandRow(child as Grid);
                }
            }

            foreach (ColumnDefinition cd in grid.ColumnDefinitions)
            {
                cd.MaxWidth = double.PositiveInfinity;
            }
            foreach (RowDefinition cd in grid.RowDefinitions)
            {
                cd.MaxHeight = double.PositiveInfinity;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Widthsplitter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void Widthsplitter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)((GridSplitter)sender).Parent;
            DockManager dm = GetDockingGrid(parentGrid);
            UpdatepaneWidth(dm.gridDocking.gridDocking);
            ClearMaxColumnWidthandRow(dm.gridDocking.gridDocking);

            if (resizingWindow1 != null)
            {
                resizingWindow1.SizeChanged -= new SizeChangedEventHandler(resizingWindow1_SizeChanged);
                resizingWindow1 = null;
            }

            if (resizingWindow2 != null)
            {
                resizingWindow2.SizeChanged -= new SizeChangedEventHandler(resizingWindow2_SizeChanged);
                resizingWindow2 = null;
            }

            if (columnDefinition1 != null)
            {
                columnDefinition1.MinWidth = 20;
                columnDefinition1 = null;
            }

            if(columnDefinition2 != null)
            {
                columnDefinition2.MinWidth = 20;
                columnDefinition2 = null;
            }

            if (resizingGrid1 != null)
            {
                resizingGrid1.SizeChanged -= new SizeChangedEventHandler(resizingGrid1_SizeChanged);
                resizingGrid1 = null;
            }

            if (resizingGrid2 != null)
            {
                resizingGrid2.SizeChanged -= new SizeChangedEventHandler(resizingGrid2_SizeChanged);
                resizingGrid2 = null;
            }

            isDragging = false;
            _loaded = false;
            (sender as GridSplitter).Background = new SolidColorBrush(Colors.Transparent);
            (sender as GridSplitter).BorderBrush = new SolidColorBrush(Colors.Transparent);
            (sender as GridSplitter).BorderThickness = new Thickness(0);
        }

        /// <summary>
        /// Value indicating whether dragging is true or not.
        /// </summary>
        bool isDragging = false;

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
                cd.Width = new GridLength(1, GridUnitType.Star);
            }
        }

        /// <summary>
        /// Clears the width of the max column.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void ClearMaxColumnWidth(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    ClearMaxColumnWidth(child as Grid);
                }
            }

            foreach (ColumnDefinition cd in grid.ColumnDefinitions)
            {
                cd.MaxWidth = double.PositiveInfinity;
            }
        }

        /// <summary>
        /// Sets height for columns
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="dock"></param>
        void SetWidthForColumn(Grid grid, Dock dock)
        {
            if(dock == Dock.Left)
            {
                if (grid.ColumnDefinitions[0].Width == new GridLength(1, GridUnitType.Star))
                    grid.ColumnDefinitions[0].Width = new GridLength(((Grid) grid).ColumnDefinitions[0].ActualWidth,GridUnitType.Pixel);
                grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

                if (((Grid)grid.Children[1]).Children.Count > 1 && ((Grid)grid.Children[1]).ColumnDefinitions.Count > 1)
                {
                    SetWidthForColumn((Grid) grid.Children[1], Dock.Left);
                }
                else if(((Grid)grid.Children[1]).Children.Count > 1 && ((Grid)grid.Children[1]).RowDefinitions.Count > 1)
                {
                    resizingGrid1 = (Grid)grid.Children[1];
                    resizingGrid1.SizeChanged += new SizeChangedEventHandler(resizingGrid1_SizeChanged);              
                }
                else if (((Grid)grid.Children[1]).Children.Count == 1 && ((Grid)grid.Children[1]).Children[0].GetType() == typeof(Window))
                {
                    resizingWindow1 = ((Grid) grid.Children[1]).Children[0] as Window;
                    resizingWindow1.SizeChanged += new SizeChangedEventHandler(resizingWindow1_SizeChanged);
                }
            }
            else if (dock == Dock.Right)
            {
                grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                if (grid.ColumnDefinitions[1].Width == new GridLength(1, GridUnitType.Star))
                    grid.ColumnDefinitions[1].Width = new GridLength(((Grid)grid).ColumnDefinitions[1].ActualWidth, GridUnitType.Pixel);

                if (((Grid)grid.Children[0]).Children.Count > 1 && ((Grid)grid.Children[0]).ColumnDefinitions.Count > 1)
                {
                    SetWidthForColumn((Grid) grid.Children[0], Dock.Right);
                }
                else if(((Grid)grid.Children[0]).Children.Count > 1 && ((Grid)grid.Children[0]).RowDefinitions.Count > 1)
                {
                    resizingGrid2 = (Grid) grid.Children[0];
                    resizingGrid2.SizeChanged += new SizeChangedEventHandler(resizingGrid2_SizeChanged);
                }
                else if (((Grid)grid.Children[0]).Children.Count == 1 && ((Grid)grid.Children[0]).Children[0].GetType() == typeof(Window))
                {
                    resizingWindow2 = ((Grid) grid.Children[0]).Children[0] as Window;
                    resizingWindow2.SizeChanged += new SizeChangedEventHandler(resizingWindow2_SizeChanged);
                }
            }
        }

        void resizingGrid2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (resizingGrid2 != null && resizingGrid2.ActualWidth <= 20)
            {
                if (columnDefinition2 != null)
                {
                    columnDefinition2.MinWidth = columnDefinition2.ActualWidth;
                    if (columnDefinition1 != null)
                        columnDefinition1.MaxWidth = columnDefinition1.ActualWidth;
                }
            }
        }

        void resizingGrid1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(resizingGrid1 != null && resizingGrid1.ActualWidth <= 20)
            {
                if (columnDefinition1 != null)
                {
                    columnDefinition1.MinWidth = columnDefinition1.ActualWidth;
                    if (columnDefinition2 != null)
                        columnDefinition2.MaxWidth = columnDefinition2.ActualWidth;
                }
            }
        }

        void resizingWindow2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
            if (resizingWindow2 != null && resizingWindow2.ActualWidth <= 20 )
            {
                if (columnDefinition2 != null)
                {
                    columnDefinition2.MinWidth = columnDefinition2.ActualWidth;
                    if (columnDefinition1 != null)
                        columnDefinition1.MaxWidth = columnDefinition1.ActualWidth;
                }
            }
        }

        void resizingWindow1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(resizingWindow1 != null && resizingWindow1.ActualWidth<=20)
            {
                if(columnDefinition1 != null)
                {
                    columnDefinition1.MinWidth = columnDefinition1.ActualWidth;
                    if (columnDefinition2 != null)
                        columnDefinition2.MaxWidth = columnDefinition2.ActualWidth;
                }
            }
        }

        /// <summary>
        /// Sets the height for rows
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="dock"></param>
        void SetHeightForRow(Grid grid, Dock dock)
        {
            if(dock == Dock.Top)
            {
                if(grid.RowDefinitions[0].Height == new GridLength(1, GridUnitType.Star))
                    grid.RowDefinitions[0].Height = new GridLength(((Grid)grid).RowDefinitions[0].ActualHeight, GridUnitType.Pixel);
                grid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

                if (((Grid)grid.Children[1]).Children.Count > 1 && ((Grid)grid.Children[1]).RowDefinitions.Count > 1)
                    SetHeightForRow((Grid) grid.Children[1], Dock.Top);
                else if (((Grid)grid.Children[1]).Children.Count > 1 && ((Grid)grid.Children[1]).ColumnDefinitions.Count > 1)
                {
                    resizingRowGrid1 = (Grid)grid.Children[0];
                    resizingRowGrid1.SizeChanged += new SizeChangedEventHandler(resizingRowGrid1_SizeChanged);
                }
                else if (((Grid)grid.Children[1]).Children.Count == 1 && ((Grid)grid.Children[1]).Children[0].GetType() == typeof(Window))
                {
                    resizingRowWindow1 = ((Grid)grid.Children[1]).Children[0] as Window;
                    resizingRowWindow1.SizeChanged += new SizeChangedEventHandler(resizingRowWindow1_SizeChanged);
                }
            }
            else if(dock == Dock.Bottom)
            {
                grid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                if(grid.RowDefinitions[1].Height == new GridLength(1,GridUnitType.Star))
                    grid.RowDefinitions[1].Height = new GridLength(((Grid)grid).RowDefinitions[1].ActualHeight, GridUnitType.Pixel);

                if(((Grid)grid.Children[0]).Children.Count > 1 && ((Grid) grid.Children[0]).RowDefinitions.Count > 1)
                    SetHeightForRow((Grid)grid.Children[0], Dock.Bottom);
                else if (((Grid)grid.Children[0]).Children.Count > 1 && ((Grid)grid.Children[0]).ColumnDefinitions.Count > 1)
                {
                    resizingRowGrid2 = (Grid)grid.Children[0];
                    resizingRowGrid2.SizeChanged += new SizeChangedEventHandler(resizingRowGrid2_SizeChanged);
                }
                else if (((Grid)grid.Children[0]).Children.Count == 1 && ((Grid)grid.Children[0]).Children[0].GetType() == typeof(Window))
                {
                    resizingRowWindow2 = ((Grid)grid.Children[0]).Children[0] as Window;
                    resizingRowWindow2.SizeChanged += new SizeChangedEventHandler(resizingRowWindow2_SizeChanged);
                }
            }
        }

        void resizingRowWindow1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           if(resizingRowWindow1 != null && resizingRowWindow1.ActualHeight <= 20)
           {
               if (rowDefinition1 != null)
               {
                   rowDefinition1.MinHeight = rowDefinition1.ActualHeight;
                   if (rowDefinition2 != null)
                        rowDefinition2.MaxHeight = rowDefinition2.ActualHeight;
               }
           }
        }

        void resizingRowGrid1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(resizingRowGrid1 != null && resizingRowGrid1.ActualHeight<= 20)
            {
                if (rowDefinition1 != null)
                {
                    rowDefinition1.MinHeight = rowDefinition1.ActualHeight;
                    if (rowDefinition2 != null)
                        rowDefinition2.MaxHeight = rowDefinition2.ActualHeight;
                }
            }
        }

        void resizingRowGrid2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (resizingRowGrid2 != null && resizingRowGrid2.ActualHeight <= 20)
            {
                if (rowDefinition2 != null)
                {
                    rowDefinition2.MinHeight = rowDefinition2.ActualHeight;
                    if (rowDefinition1 != null)
                        rowDefinition1.MaxHeight = rowDefinition1.ActualHeight;
                }
            }
        }

        void resizingRowWindow2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (resizingRowWindow2 != null && resizingRowWindow2.ActualHeight <= 20)
            {
                if (rowDefinition2 != null)
                {
                    rowDefinition2.MinHeight = rowDefinition2.ActualHeight;
                    if (rowDefinition1 != null)
                        rowDefinition1.MaxHeight = rowDefinition1.ActualHeight;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Widthsplitter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void Widthsplitter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _loaded = false;
            isDragging = true;
            Grid parentGrid = (Grid)((GridSplitter)sender).Parent;
            ClearMaxColumnWidth(parentGrid);
            parentGrid.ColumnDefinitions[0].MinWidth = 20;
            parentGrid.ColumnDefinitions[1].MinWidth = 20;
            parentGrid.ColumnDefinitions[0].MaxWidth = parentGrid.ActualWidth - 20;
            parentGrid.ColumnDefinitions[1].MaxWidth = parentGrid.ActualWidth - 20;

            columnDefinition1 = parentGrid.ColumnDefinitions[0];
            columnDefinition2 = parentGrid.ColumnDefinitions[1];

            if ((parentGrid.Children[0] as Grid).Children.Count > 1 && (parentGrid.Children[0] as Grid).ColumnDefinitions.Count > 1)
            {
                SetWidthForColumn(parentGrid.Children[0] as Grid, Dock.Left);
            }

            if ((parentGrid.Children[1] as Grid).Children.Count > 1 && (parentGrid.Children[1] as Grid).ColumnDefinitions.Count > 1)
            {
                SetWidthForColumn(parentGrid.Children[1] as Grid, Dock.Right);
            }

            try
            {
                if ((parentGrid.Children[0] as Grid).Children[0].GetType() == typeof(Window))
                {
                    Window temp = (parentGrid.Children[0] as Grid).Children[0] as Window;
                    (sender as GridSplitter).Background = temp.DockingManager.SplitterBackGroundColor;
                    if (temp.DockManager != null)
                    {
                        //if (temp.DockManager.Parent.GetType() != typeof(WindowContainer))
                        //{
                        parentGrid.ColumnDefinitions[0].MaxWidth = parentGrid.ActualWidth - 20;
                        parentGrid.ColumnDefinitions[1].MaxWidth = parentGrid.ActualWidth - 20;
                        //(temp.DockManager.Children[0] as DockingGrid).ClearRowColumnWidthHeight((temp.DockManager.Children[0] as DockingGrid).gridDocking);
                        //}
                    }
                    else
                    {
                        parentGrid.ColumnDefinitions[0].MaxWidth = parentGrid.ActualWidth - 20;
                        parentGrid.ColumnDefinitions[1].MaxWidth = parentGrid.ActualWidth - 20;
                    }
                }
                else if ((parentGrid.Children[1] as Grid).Children.Count > 0)
                {
                    if ((parentGrid.Children[1] as Grid).Children[0].GetType() == typeof(Window))
                    {
                        Window temp = (parentGrid.Children[1] as Grid).Children[0] as Window;
                        (sender as GridSplitter).Background = temp.DockingManager.SplitterBackGroundColor;
                        if (temp.DockManager != null)
                        {
                            //if (temp.DockManager.Parent.GetType() != typeof(WindowContainer))
                            //{
                            parentGrid.ColumnDefinitions[0].MaxWidth = parentGrid.ActualWidth - 20;
                            parentGrid.ColumnDefinitions[1].MaxWidth = parentGrid.ActualWidth - 20;
                            //(temp.DockManager.Children[0] as DockingGrid).ClearRowColumnWidthHeight((temp.DockManager.Children[0] as DockingGrid).gridDocking);
                            //}
                        }
                        else
                        {
                            parentGrid.ColumnDefinitions[0].MaxWidth = parentGrid.ActualWidth - 20;
                            parentGrid.ColumnDefinitions[1].MaxWidth = parentGrid.ActualWidth - 20;
                        }

                        //(temp.DockManager.Children[0] as DockingGrid).ClearColumnWidth(parentGrid);                     
                    }
                }
                //ClearColumnWidth(parentGrid.Children[0] as Grid);
                //ClearColumnWidth(parentGrid.Children[1] as Grid);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Heightsplitter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void Heightsplitter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)((GridSplitter)sender).Parent;

            parentGrid.RowDefinitions[0].MinHeight = 20;
            parentGrid.RowDefinitions[1].MinHeight = 20;

            rowDefinition1 = parentGrid.RowDefinitions[0];
            rowDefinition2 = parentGrid.RowDefinitions[1];

            if ((parentGrid.Children[0] as Grid).Children.Count > 1 && (parentGrid.Children[0] as Grid).RowDefinitions.Count > 1)
            {
                SetHeightForRow(parentGrid.Children[0] as Grid, Dock.Top);
            }

            if ((parentGrid.Children[1] as Grid).Children.Count > 1 && (parentGrid.Children[1] as Grid).RowDefinitions.Count > 1)
            {
                SetHeightForRow(parentGrid.Children[1] as Grid, Dock.Bottom);
            }

            if ((parentGrid.Children[0] as Grid).Children[0].GetType() == typeof(Window))
            {
                Window temp = (parentGrid.Children[0] as Grid).Children[0] as Window;
                (sender as GridSplitter).Background = temp.DockingManager.SplitterBackGroundColor;
                if (temp.DockManager != null)
                {
                    //if (temp.DockManager.Parent.GetType() != typeof(WindowContainer))
                    //{
                    parentGrid.RowDefinitions[0].MaxHeight = parentGrid.ActualHeight - 20;
                    parentGrid.RowDefinitions[1].MaxHeight = parentGrid.ActualHeight - 20;
                    //(temp.DockManager.Children[0] as DockingGrid).ClearRowColumnWidthHeight((temp.DockManager.Children[0] as DockingGrid).gridDocking);
                    //}
                }
                else
                {
                    parentGrid.RowDefinitions[0].MaxHeight = parentGrid.ActualHeight - 20;
                    parentGrid.RowDefinitions[1].MaxHeight = parentGrid.ActualHeight - 20;
                }
            }
            else if ((parentGrid.Children[1] as Grid).Children[0].GetType() == typeof(Window))
            {
                Window temp = (parentGrid.Children[1] as Grid).Children[0] as Window;
                (sender as GridSplitter).Background = temp.DockingManager.SplitterBackGroundColor;
                if (temp.DockManager != null)
                {
                    //if (temp.DockManager.Parent.GetType() != typeof(WindowContainer))
                    //{
                    parentGrid.RowDefinitions[0].MaxHeight = parentGrid.ActualHeight - 20;
                    parentGrid.RowDefinitions[1].MaxHeight = parentGrid.ActualHeight - 20;
                    //}
                }
                else
                {
                    parentGrid.RowDefinitions[0].MaxHeight = parentGrid.ActualHeight - 20;
                    parentGrid.RowDefinitions[1].MaxHeight = parentGrid.ActualHeight - 20;
                }

                // (temp.DockManager.Children[0] as DockingGrid).ClearRowHeight(parentGrid);        
            }
        }

        /// <summary>
        /// Replaces the child.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void ReplaceChild(Window pane)
        {
            _attachedPane = pane;
        }

        /// <summary>
        /// Replaces the child group.
        /// </summary>
        /// <param name="groupToFind">The group to find.</param>
        /// <param name="groupToReplace">The group to replace.</param>
        public void ReplaceChildGroup(DockablePaneGroup groupToFind, DockablePaneGroup groupToReplace)
        {
            if (FirstChildGroup == groupToFind)
            {
                FirstChildGroup = groupToReplace;
                groupToReplace.rootGrid = groupToFind.rootGrid;
            }
            else if (SecondChildGroup == groupToFind)
            {
                SecondChildGroup = groupToReplace;
                groupToReplace.rootGrid = groupToFind.rootGrid;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "Dockable PaneGroup class");
            }
        }

        ////public void SaveChildPanesSize()
        ////{
        ////    if (AttachedPane != null && ParentGroup!=null)
        ////        AttachedPane.SaveSize(ParentGroup.Dock);
        ////    else
        ////    {
        ////        FirstChildGroup.SaveChildPanesSize();
        ////        SecondChildGroup.SaveChildPanesSize();
        ////    }

        ////}

        /// <summary>
        /// Adds the existing.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns>The Docking Panel Group.</returns>
        public DockablePaneGroup AddExisting(Window pane)
        {

            switch (pane.DockPosition)
            {
                case Dock.Right:
                case Dock.Bottom:
                    return new DockablePaneGroup(this, new DockablePaneGroup(pane, rootGrid), pane.DockPosition);

                case Dock.Left:
                case Dock.Top:
                    return new DockablePaneGroup(new DockablePaneGroup(pane, rootGrid), this, pane.DockPosition);
            }

            return null;
        }

        /// <summary>
        /// Adds the pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns>The Docking Manager.</returns>
        public DockablePaneGroup AddPane(Window pane)
        {
            switch (pane.DockPosition)
            {
                case Dock.Right:
                case Dock.Bottom:
                    {
                        DockablePaneGroup dpg = new DockablePaneGroup(pane, rootGrid);
                        DockablePaneGroup temp = new DockablePaneGroup(this, dpg, pane.DockPosition);
                        dpg.First = temp;
                        return temp;
                    }

                case Dock.Left:
                case Dock.Top:
                    return new DockablePaneGroup(new DockablePaneGroup(pane, rootGrid), this, pane.DockPosition);
            }

            return null;
            ////DockablePaneGroup resGroup = null;

            ////if (AttachedPane != null)
            ////{
            ////    DockablePaneGroup newChildGroup = new DockablePaneGroup(AttachedPane);
            ////    switch (pane.Dock)
            ////    {
            ////        case Dock.Left:
            ////            resGroup = new DockablePaneGroup(new DockablePaneGroup(pane), newChildGroup, SplitOrientation.Vertical);
            ////            break;
            ////        case Dock.Right:
            ////            resGroup = new DockablePaneGroup(newChildGroup, new DockablePaneGroup(pane), SplitOrientation.Vertical);
            ////            break;
            ////        case Dock.Top:
            ////            resGroup = new DockablePaneGroup(new DockablePaneGroup(pane), newChildGroup, SplitOrientation.Horizontal);
            ////            break;
            ////        case Dock.Bottom:
            ////            resGroup = new DockablePaneGroup(newChildGroup, new DockablePaneGroup(pane), SplitOrientation.Horizontal);
            ////            break;
            ////    }
            ////}
            ////else
            ////{
            ////    if (SplitOrientation == SplitOrientation.Vertical)
            ////    {
            ////        if (pane.Dock == Dock.Left)
            ////        {
            ////            ChildGroups.Insert(0, new DockablePaneGroup(pane));
            ////            resGroup = this;
            ////        }
            ////        else if (pane.Dock == Dock.Right)
            ////        {
            ////            int index = 0; 
            ////            for (int i = 0; i < ChildGroups.Count;i++)
            ////                if (ChildGroups[i].do

            ////            ChildGroups.Add(new DockablePaneGroup(pane));
            ////            resGroup = this;
            ////        }
            ////        else if (pane.Dock == Dock.Bottom)
            ////            resGroup = new DockablePaneGroup(this, new DockablePaneGroup(pane), SplitOrientation.Horizontal);
            ////        else if (pane.Dock == Dock.Top)
            ////            resGroup = new DockablePaneGroup(new DockablePaneGroup(pane), this, SplitOrientation.Horizontal);
            ////    }
            ////    else //if (SplitOrientation == SplitOrientation.Horizontal)
            ////    {
            ////        if (pane.Dock == Dock.Top)
            ////        {
            ////            ChildGroups.Insert(0, new DockablePaneGroup(pane));
            ////            resGroup = this;
            ////        }
            ////        else if (pane.Dock == Dock.Bottom)
            ////        {
            ////            ChildGroups.Add(new DockablePaneGroup(pane));
            ////            resGroup = this;
            ////        }
            ////        else if (pane.Dock == Dock.Right)
            ////            resGroup = new DockablePaneGroup(this, new DockablePaneGroup(pane), SplitOrientation.Vertical);
            ////        else if (pane.Dock == Dock.Left)
            ////            resGroup = new DockablePaneGroup(new DockablePaneGroup(pane), this, SplitOrientation.Vertical);
            ////    }
            ////}

            ////return resGroup;
        }

        /// <summary>
        /// Removes the pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns>The Docking Pane Group.</returns>
        public DockablePaneGroup RemovePane(Window pane)
        {
            if (AttachedPane != null)
            {
                return null;
            }

            if (FirstChildGroup.AttachedPane == pane)
            {
                return SecondChildGroup;
            }
            else if (SecondChildGroup.AttachedPane == pane)
            {
                return FirstChildGroup;
            }
            else
            {
                DockablePaneGroup group = FirstChildGroup.RemovePane(pane);
                if (group != null)
                {
                    FirstChildGroup = group;
                    group._parentGroup = this;
                    return null;
                }

                group = SecondChildGroup.RemovePane(pane);
                if (group != null)
                {
                    SecondChildGroup = group;
                    group._parentGroup = this;
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the pane group.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns>The Docking panel Group.</returns>
        public DockablePaneGroup GetPaneGroup(Window pane)
        {
            if (AttachedPane == pane)
            {
                return this;
            }

            if (FirstChildGroup != null)
            {
                DockablePaneGroup paneGroup = FirstChildGroup.GetPaneGroup(pane);
                if (paneGroup != null)
                {
                    return paneGroup;
                }
            }

            if (SecondChildGroup != null)
            {
                DockablePaneGroup paneGroup = SecondChildGroup.GetPaneGroup(pane);
                if (paneGroup != null)
                {
                    return paneGroup;
                }
            }

            return null;
        }

        /// <summary>
        /// Arranges the order.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="dg">The dg.</param>
        public void ArrangeOrder(Grid grid, DockingGrid dg)
        {
            if (AttachedPane != null)
            {
                ////AttachedPane.IsHidden)
                if (AttachedPane.DockState == DockState.Dock && AttachedPane.DockManager.Children[0] as DockingGrid == dg)
                {
                    Canvas.SetLeft(AttachedPane, 0);
                    Canvas.SetTop(AttachedPane, 0);
                    Canvas.SetZIndex(AttachedPane, 1);

                    if (AttachedPane.Parent != null)
                    {
                        if (AttachedPane.Parent is DockingManager && AttachedPane.GetType() == typeof(Window))
                        {
                            ((Canvas)AttachedPane.DockingManager).Children.Remove(AttachedPane);
                            // grid.Children.Add(AttachedPane);
                        }
                    }
                    else
                    {
                        //grid.Children.Add(AttachedPane);
                        if (AttachedPane.DockingManager != null)
                        {
                            if (AttachedPane.DockingManager.SplitterBackGroundColor != null)
                            {
                                SplitterBackGroundColor = AttachedPane.DockingManager.SplitterBackGroundColor;
                            }
                        }
                    }
                }
            }
            else if (FirstChildGroup.IsHidden && !SecondChildGroup.IsHidden)
            {
                SecondChildGroup.ArrangeOrder(grid, dg);
            }
            else if (!FirstChildGroup.IsHidden && SecondChildGroup.IsHidden)
            {
                FirstChildGroup.ArrangeOrder(grid, dg);
            }
            else
            {
                if (Dock == Dock.Left || Dock == Dock.Right)
                {
                    Grid firstChildGrid = new Grid();
                    FirstChildGroup.ArrangeOrder(firstChildGrid, dg);

                    Grid secondChildGrid = new Grid();
                    SecondChildGroup.ArrangeOrder(secondChildGrid, dg);
                    //grid.Children.Add(secondChildGrid);

                    CustomGridSplitter splitter = new CustomGridSplitter();
                    splitter.MouseLeftButtonDown += new MouseButtonEventHandler(Widthsplitter_MouseLeftButtonDown);
                    splitter.MouseLeftButtonUp += new MouseButtonEventHandler(Widthsplitter_MouseLeftButtonUp);
                    splitter.MouseMove += new MouseEventHandler(Widthsplitter_MouseMove);
                    splitter.Width = 4;
                    splitter.HorizontalAlignment = HorizontalAlignment.Right;
                    splitter.VerticalAlignment = VerticalAlignment.Stretch;
                    splitter.Background = new SolidColorBrush(Colors.Transparent);
                    splitter.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    //grid.Children.Add(splitter);
                }
                else
                {

                    Grid firstChildGrid = new Grid();
                    FirstChildGroup.ArrangeOrder(firstChildGrid, dg);

                    //grid.Children.Add(firstChildGrid);
                    Grid secondChildGrid = new Grid();
                    SecondChildGroup.ArrangeOrder(secondChildGrid, dg);
                    //grid.Children.Add(secondChildGrid);

                    CustomGridSplitter splitter = new CustomGridSplitter();
                    splitter.MouseLeftButtonDown += new MouseButtonEventHandler(Heightsplitter_MouseLeftButtonDown);
                    splitter.MouseLeftButtonUp += new MouseButtonEventHandler(Heightsplitter_MouseLeftButtonUp);
                    splitter.MouseMove += new MouseEventHandler(Heightsplitter_MouseMove);
                    splitter.Height = 4;
                    splitter.Background = new SolidColorBrush(Colors.Transparent);
                    splitter.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    splitter.VerticalAlignment = VerticalAlignment.Bottom;
                    //grid.Children.Add(splitter);
                }
            }
        }

    }
}
