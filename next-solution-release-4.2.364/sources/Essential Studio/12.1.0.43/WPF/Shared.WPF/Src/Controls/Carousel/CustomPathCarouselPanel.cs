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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Collections.ObjectModel;


namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomPathCarouselPanel : VirtualizingPanel
    {
        #region Properties

        private System.Windows.Shapes.Path _Path;

        internal Carousel Owner = null;


        internal CarouselItem removingItem = null;

        internal bool isRender = true;

        private bool isRefreshing = false;

        internal Size finalSize;

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public System.Windows.Shapes.Path Path
        {
            get { return _Path; }
            set { _Path = value; }
        }

        /// <summary>
        /// Gets or sets the items per page.
        /// </summary>
        /// <value>The items per page.</value>
        public int ItemsPerPage
        {
            get { return (int)GetValue(ItemsPerPageProperty); }
            set { SetValue(ItemsPerPageProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemsPerPageProperty =
            DependencyProperty.Register("ItemsPerPage", typeof(int), typeof(CustomPathCarouselPanel), new PropertyMetadata(-1, (s, a) => ((CustomPathCarouselPanel)s).OnItemsPerPageChanged(s)));

        /// <summary>
        /// Called when [items per page changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected void OnItemsPerPageChanged(DependencyObject obj)
        {
            CustomPathCarouselPanel panel = obj as CustomPathCarouselPanel;
            if (panel != null && !panel.isRefreshing)
            {
                panel.SetPaths();
                if (Owner != null && Owner.SelectedItem!=null && ItemsPerPage != Owner.Items.Count)
                {
                    if (ItemsPerPage == -1)
                        ItemsPerPage = Owner.Items.Count;
                    if ((panel.NewVirtualizingPanelHandler == null) && base.IsInitialized && Owner.SelectedItem is CarouselItem)
                        panel.NewVirtualizingPanelHandler = new VirtualizingPanelItemMoveHandler(CoerceDisplacement(GetMovementOffsetFromTopElement(Owner.SelectedItem as CarouselItem)), panel.CarouselPanelHelper);
                }
                panel.Invalidate(false);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double TopItemPosition
        {
            get { return (double)GetValue(TopItemPositionProperty); }
            set { SetValue(TopItemPositionProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TopItemPositionProperty =
            DependencyProperty.Register("TopItemPosition", typeof(double), typeof(CustomPathCarouselPanel), new PropertyMetadata(0.5, new PropertyChangedCallback(OnTopItemPositionChanged), new CoerceValueCallback(CoerceTopItemPosition)));

        private static void OnTopItemPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CustomPathCarouselPanel panel = (CustomPathCarouselPanel)obj;
            if (panel != null)
            {
                panel.Invalidate(false);
            }
        }

        private static object CoerceTopItemPosition(DependencyObject obj,object value)
        {
            double newVal = (double)value;
            if (newVal > 1 || newVal < 0)
            {
                newVal = 0.5;
            }
            return newVal;
        }
        
        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty PathFractionProperty;

        /// <summary>
        /// Initializes the <see cref="CarouselPanel"/> class.
        /// </summary>
        static CustomPathCarouselPanel()
        {
            PathFractionProperty = DependencyProperty.RegisterAttached("PathFraction", typeof(double), typeof(CustomPathCarouselPanel), new PropertyMetadata(-1.0));
            ItemPathFractionManagerProperty = DependencyProperty.RegisterAttached("ItemMovementAnimationDataFraction", typeof(PathFractionManager), typeof(CustomPathCarouselPanel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselPanel"/> class.
        /// </summary>
        private CarouselPanelHelper CarouselPanelHelper;
        /// <summary>
        /// 
        /// </summary>
        public CustomPathCarouselPanel()
        {
            CarouselPanelHelper = new CarouselPanelHelper(this);
            base.AddHandler(UIElement.MouseDownEvent, new RoutedEventHandler(this.SelectedItemChanged));
            this.Loaded += new RoutedEventHandler(CarouselPanel_Loaded);
        }

        void CarouselPanel_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetCustomPathWithItems(this.ItemsPerPage);
        }

        private bool ShouldLoadItems
        {
            get { return true; }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(0.0, 0.0);
            if (carouselPathHelper != null)
            {
                if (double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
                {
                    size.Width = carouselPathHelper.Geometry.Bounds.Right - carouselPathHelper.Geometry.Bounds.Left;
                    size.Height = availableSize.Height;
                    MeasurePanel(size);
                    return size;
                }
                else if (!double.IsInfinity(availableSize.Width) && double.IsInfinity(availableSize.Height))
                {
                    size.Width = availableSize.Width;
                    size.Height = carouselPathHelper.Geometry.Bounds.Bottom - carouselPathHelper.Geometry.Bounds.Top;
                    MeasurePanel(size);
                    return size;
                }
                else if (double.IsInfinity(availableSize.Width) && double.IsInfinity(availableSize.Height))
                {
                    size.Width = carouselPathHelper.Geometry.Bounds.Right - carouselPathHelper.Geometry.Bounds.Left;
                    size.Height = carouselPathHelper.Geometry.Bounds.Bottom - carouselPathHelper.Geometry.Bounds.Top;
                    MeasurePanel(size);
                    return size;
                }
                else
                {
                    MeasurePanel(availableSize);
                    return availableSize;
                }
            }
            return size;
        }

        internal void MeasurePanel(Size availableSize)
        {
            finalSize = availableSize;
            this.SetPaths();
            this.carouselPathHelper.UpdateCustomPath(availableSize, new Thickness(0, 0, 0, 0));
            this.CleanUpItems();
            this.InitializeItemMovement();
            this.UpdateVisibleItems();
            foreach (UIElement child in base.InternalChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            this.SetMaximumandViewPanelOffset();
            this.Start_ItemMovement();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicalParent"></param>
        /// <returns></returns>
        protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            ObservableUIElementCollection elementCollection = new ObservableUIElementCollection(this, logicalParent);
            //elementCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnChildrenCollectionChanged);
            return elementCollection;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //this.SetPaths();
            double childHeightStartPoint = 0.0;
            if (base.IsItemsHost)
            {
                foreach (UIElement child in base.InternalChildren)
                {                   
                    childHeightStartPoint = ArrangeVisibleChild(childHeightStartPoint, child);
                    this.RecalculatePosition(child);
                }
            }
            else
            {
                foreach (UIElement child in base.Children)
                {
                    Point newItemPosition;
                    Point newItemTangent;
                    double pathFraction = (double)child.GetValue(PathFractionProperty);
                    carouselPathHelper.Geometry.GetPointAtFractionLength(pathFraction, out newItemPosition, out newItemTangent);
                    TranslateTransform transform = new TranslateTransform(newItemPosition.X - child.DesiredSize.Width/2, newItemPosition.Y - child.DesiredSize.Height/2);
                    TransformGroup finalTransform = new TransformGroup();
                    finalTransform.Children.Add(transform);
                    finalTransform.Freeze();
                    child.RenderTransform = finalTransform;
                    child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                }
            }
            return finalSize;
        }

        private static double ArrangeVisibleChild(double childHeightStartPoint, UIElement child)
        {
            child.Arrange(new Rect(new Point(0.0, 0.0), child.DesiredSize));
            childHeightStartPoint += child.DesiredSize.Height;
            return childHeightStartPoint;
        }

        internal void RecalculatePosition(UIElement child)
        {
            MatrixTransform newTransform = null;
            if (this.CurrentVirtualizingPanelHandler == null)
            {
                newTransform = VirtualizingPanelHandler.RecalculateItemPosition(child, this.carouselPathHelper);
                this.ApplyOffsetTransform(child, newTransform);
            }
            this.UpdateVisualization();
        }

        private void ApplyOffsetTransform(UIElement item, MatrixTransform transform)
        {
            Point pathOffset = new Point(0, 0);

            Matrix matrix = transform.Matrix;
            matrix.Translate(pathOffset.X, pathOffset.Y);
            MatrixTransform newtransform = new MatrixTransform(matrix);
            item.RenderTransform = newtransform;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);
            int displacement = 1;
            if (this.Owner != null && args.Action != NotifyCollectionChangedAction.Reset)
            {
                if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    int selectedIndex = this.Owner.SelectedIndex;
                    if(selectedIndex < 0)
                        this.Owner.SelectedItem = null;
                    else if (!this.Owner.Items.Contains(this.Owner.SelectedItem))
                    {
                        if (this.Owner.Items.Count != 0)
                        {
                            for (int i = selectedIndex; i >= 0; i--)
                            {
                                if (i < this.Owner.Items.Count)
                                {
                                    if (selectedIndex > i)
                                        displacement = -1;
                                    this.Owner.SelectedItem = this.Owner.Items[i];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            this.Owner.SelectedItem = null;
                        }
                    }
                }
                Refresh(true, displacement);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);           
        }
        /// <summary>
        /// 
        /// </summary>
        public void RenderAgain()
        {
            for (int i = 0; i < base.InternalChildren.Count; i++)
            {
                UIElement child = base.InternalChildren[i];
                child.SetValue(PathFractionProperty, carouselPathHelper.PathFractions[i].PathFraction);
            }
        }
        #endregion
        
        #region Path

        internal Path DrawingPath;
        internal CarouselPathHelper carouselPathHelper;
        private void SetPaths()
        {
            if (this.Path == null)
            {
                this.SetDrawingPathFromPath(CarouselPanelHelperMethods.GetPath());
            }
            else
            {
                Path.Stretch = Stretch.Uniform;
                this.SetDrawingPathFromPath(this.Path);
            }          
        }

        private void SetDrawingPathFromPath(Path drawingPath)
        {
            this.DrawingPath = drawingPath;
            this.SetFractionPathDrawing(drawingPath);
        }

        private void SetFractionPathDrawing(Path drawingPath)
        {
            if (drawingPath != null)
            {
                this.carouselPathHelper = new CarouselPathHelper(drawingPath, ItemsPerPage);
                this.carouselPathHelper.SetTopElementPathFraction(new PathFractions(this.TopItemPosition));
            }
        }

        private void DrawPath(DrawingContext dc)
        {
            Path RenderedPath = this.Path;
            if (((dc != null) && (RenderedPath != null)))
            {
                Pen pathPen = new Pen();
                pathPen.Brush = new SolidColorBrush(Colors.Red); //RenderedPath.Stroke;
                pathPen.DashCap = RenderedPath.StrokeDashCap;
                pathPen.EndLineCap = RenderedPath.StrokeEndLineCap;
                pathPen.LineJoin = RenderedPath.StrokeLineJoin;
                pathPen.MiterLimit = RenderedPath.StrokeMiterLimit;
                pathPen.StartLineCap = RenderedPath.StrokeStartLineCap;
                pathPen.Thickness = 2;// RenderedPath.StrokeThickness;
                dc.DrawGeometry(RenderedPath.Fill, pathPen, this.carouselPathHelper.Geometry);
            }
        }
        #endregion

        private void SelectedItemChanged(object sender, RoutedEventArgs e)
        {            
        }

        private PathFractionRangeHandler m_PathFractionRangeHandler = new PathFractionRangeHandler();

        #region VirtualizingPanelHandler

        internal VisibleItemsHandler GetCurrentItemPathArrangement()
        {
            if (this.carouselPathHelper == null)
            {
                return null;
            }
            VisibleItemsHandler arrangement = new VisibleItemsHandler(this.carouselPathHelper.GetVisiblePathFractionCount());
            if (arrangement.Count > 0)
            {
                foreach (VisiblePanelItem pair in this.m_PathFractionRangeHandler)
                {
                    double pathFraction = GetPathFraction(pair.Child);
                    if (CarouselPathHelper.IsVisible(pathFraction))
                    {
                        int controlPointIndex = this.carouselPathHelper.GetPathFractionIndex(pathFraction);
                        if (controlPointIndex != -1)
                        {
                            arrangement.SetItemAtPosition(controlPointIndex - 1, pair);
                        }
                    }
                }
            }
            return arrangement;
        }

        internal void SetCustomPathWithItems(int numberOfItems)
        {
            if (((numberOfItems > 0) && this.ShouldLoadItems) && !this.m_PathFractionRangeHandler.HasVisibleItems)
            {
                int movementDisplacement = Math.Min(this.ItemsPerPage, numberOfItems);
                int itemsAfter = CarouselPanelHelperMethods.GetItemCountlater(this.m_PathFractionRangeHandler, this.CarouselPanelHelper.ItemsCount);
                int itemsBefore = CarouselPanelHelperMethods.GetItemCountBefore(this.m_PathFractionRangeHandler);
                if (itemsAfter >= movementDisplacement)
                {
                    this.MoveBy(movementDisplacement);
                }
                else if (itemsBefore >= movementDisplacement)
                {
                    this.MoveBy(-movementDisplacement);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="displacement"></param>
        public void MoveBy(int displacement)
        {
            if (displacement != 0)
            {
                this.MoveItemInternallyBy(displacement);
                this.UpdatePanelOffset(displacement);
            }
        }


        #endregion

        #region CleanUp and GenerateItems

        private void CleanUpItems()
        {
            if (base.IsItemsHost)
            {
                this.CleanGeneratedItems();
            }
            this.m_PathFractionRangeHandler.ClearCleanUp();
        }

        private void CleanGeneratedItems()
        {
            int count = base.InternalChildren.Count;
            for (int i = 0; i < count; i++)
            {
                if ((base.InternalChildren[i] as CarouselItem).DataContext != null)
                {
                    if (i < base.InternalChildren.Count && base.InternalChildren[i] is CarouselItem && (base.InternalChildren[i] as CarouselItem).DataContext.ToString() == "{DisconnectedItem}")
                    {
                        base.RemoveInternalChildRange(i, 1);
                    }
                }
            }
            UIElementCollection internalChildren = base.InternalChildren;
            IItemContainerGenerator generator = base.ItemContainerGenerator;
            foreach (VisiblePanelItem pair in this.m_PathFractionRangeHandler.ToCleanUp)
            {
                GeneratorPosition childGeneratorPos = generator.GeneratorPositionFromIndex(pair.Index);
                int index = base.InternalChildren.IndexOf(pair.Child);
                if (childGeneratorPos.Index >= 0 && index >=0)
                {
                    generator.Remove(childGeneratorPos, 1);                    
                    base.RemoveInternalChildRange(index, 1);
                }
            }
        }

        internal void UpdateVisibleItems()
        {
            if (base.IsItemsHost)
            {
                this.GenerateItems();
            }
            else
            {
                this.UpdateChildPairs();
            }
        }

        private void UpdateChildPairs()
        {
            if (!base.IsItemsHost)
            {
                foreach (VisiblePanelItem pair in this.m_PathFractionRangeHandler)
                {
                    pair.Child = base.InternalChildren[pair.Index];
                }
            }
        }

        private void GenerateItems()
        {
            if (base.IsItemsHost)
            {
                this.GenerateChildrenWithItemContainerGenerator();
            }
        }

        private void GenerateChildrenWithItemContainerGenerator()
        {
            if (this.m_PathFractionRangeHandler.HasVisibleItems)
            {
                UIElementCollection internalChildren = base.InternalChildren;
                IItemContainerGenerator generator = base.ItemContainerGenerator;
                foreach (VisiblePanelItem item in this.m_PathFractionRangeHandler)
                {
                    GeneratorPosition startPos = generator.GeneratorPositionFromIndex(item.Index);
                    using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
                    {
                        bool newlyRealized;
                        UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                        if (newlyRealized || item.Child == null)
                        {
                            item.Child = child;
                            try
                            {
                                base.InsertInternalChild(base.InternalChildren.Count, child);
                                generator.PrepareItemContainer(child);
                            }
                                catch (Exception)
                                {}
                        }
                        continue;
                    }
                }

                if (base.InternalChildren.Count > this.m_PathFractionRangeHandler.Count 
                    && this.NewVirtualizingPanelHandler is VirtualizingPanelItemMoveHandler)
                {
                    VirtualizingPanelItemMoveHandler handler = this.NewVirtualizingPanelHandler as VirtualizingPanelItemMoveHandler;
                    CleanUpItems(handler.Collection.FirstVisibleIndex, handler.Collection.LastVisibleIndex);
                }
            }
        }


        /// <summary>
        /// Cleans the Container of the Items that not in  Viewport.
        /// </summary>        
        private void CleanUpItems(int firstVisibleItemIndex, int lastVisibleItemIndex)
        {

            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            for (int i = children.Count - 1; i >= 0; i--)
            {

                // Map a child index to an item index by going through a generator position

                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);

                if (itemIndex < firstVisibleItemIndex || itemIndex > lastVisibleItemIndex)
                {
                    generator.Remove(childGeneratorPos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }
        #endregion

        #region UpdateVisualization

        SortedDictionary<double, UIElement> sorteditems = new SortedDictionary<double, UIElement>();

        private void UpdateVisualization()
        {
            if (this.m_PathFractionRangeHandler.HasVisibleItems)
            {
                this.UpdateZorder();
                foreach (VisiblePanelItem pair in this.m_PathFractionRangeHandler)
                {
                    ScaleTransform scaleTransform;
                    SkewTransform skewAngleXTransform;
                    SkewTransform skewAngleYTransform;
                    UIElement item = pair.Child;
                    item.RenderTransformOrigin = new Point(0.5, 0.5);
                    this.UpdateOpacityFractions(item);
                    this.UpdateSkewAngleXFractions(item, out skewAngleXTransform);
                    this.UpdateSkewAngleYFractions(item, out skewAngleYTransform);
                    this.UpdateScaleFractions(item, out scaleTransform);
                    TranslateTransform translateTransform = new TranslateTransform(item.RenderTransform.Value.OffsetX, item.RenderTransform.Value.OffsetY);
                    MatrixTransform newItemTransform = new MatrixTransform(((scaleTransform.Value * skewAngleXTransform.Value) * skewAngleYTransform.Value) * translateTransform.Value);
                    item.RenderTransform = newItemTransform;
                }
            }
        }

        #region ZOrder

        private void UpdateZorder()
        {
            UIElement child = this.FindClosestElementToPathFraction(this.TopItemPosition);
            //base.SetValue(TopContainerPropertyKey, child);
            if (child != null)
            {
                int childIndex = 0;
                for (int i = 0; i < this.carouselPathHelper.PathFractions.Count(); i++)
                {
                    if (this.carouselPathHelper.PathFractions[i].PathFraction == this.carouselPathHelper.topElementPathFraction.PathFraction)
                    {
                        childIndex = i;
                        break;
                    }
                }

                sorteditems.Clear();

                foreach (UIElement element in Children)
                {
                    if (!sorteditems.ContainsKey(GetPathFraction(element)))
                    {
                        sorteditems.Add(GetPathFraction(element), element);
                    }
                }

                int zIndexCounter = base.Children.Count - 1;
                for (int i = childIndex; i >= 0; i--)
                {
                    double fraction = carouselPathHelper.PathFractions[i].PathFraction;
                    if (sorteditems.ContainsKey(fraction))
                    {
                        Panel.SetZIndex(sorteditems[fraction], zIndexCounter);
                        zIndexCounter--;
                    }
                }
                for (int i = childIndex + 1; i < carouselPathHelper.PathFractions.Count(); i++)
                {
                    double fraction = carouselPathHelper.PathFractions[i].PathFraction;
                    if (sorteditems.ContainsKey(fraction))
                    {
                        Panel.SetZIndex(sorteditems[fraction], zIndexCounter);
                        zIndexCounter--;
                    }
                }
            }
        }

        internal UIElement FindClosestElementToPathFraction(double point)
        {
            double currentShortestDistance = double.PositiveInfinity;
            UIElement currentClosestElement = null;
            foreach (UIElement element in base.InternalChildren)
            {
                double distanceToPoint = Math.Abs((double)(GetPathFraction(element) - point));
                if (distanceToPoint < currentShortestDistance)
                {
                    currentShortestDistance = distanceToPoint;
                    currentClosestElement = element;
                }
            }
            if (currentClosestElement == null)
            {
                return null;
            }
            return currentClosestElement;
        }
        #endregion

        #region Opacity
        /// <summary>
        /// 
        /// </summary>
        public bool OpacityEnabled
        {
            get { return (bool)GetValue(OpacityEnabledProperty); }
            set { SetValue(OpacityEnabledProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OpacityEnabledProperty =
            DependencyProperty.Register("OpacityEnabled", typeof(bool), typeof(CustomPathCarouselPanel), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public PathFractionCollection OpacityFractions
        {
            get { return (PathFractionCollection)GetValue(OpacityFractionsProperty); }
            set { SetValue(OpacityFractionsProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OpacityFractionsProperty =
            DependencyProperty.Register("OpacityFractions", typeof(PathFractionCollection), typeof(CustomPathCarouselPanel), new PropertyMetadata(null));

        internal PathFractionCollection internalOpacityFractions;
        internal PathFractionCollection InternalOpacityFractions
        {
            get
            {
                if (CanResetPathFractionCollection(this.internalOpacityFractions, this.OpacityFractions))
                {
                    this.internalOpacityFractions = null;
                }
                if (this.internalOpacityFractions == null)
                {
                    this.internalOpacityFractions = this.OpacityFractions ?? CarouselPanelHelperMethods.GetOpacityFractionsCollection();
                }
                return this.internalOpacityFractions;
            }
        }

        private void UpdateOpacityFractions(UIElement item)
        {
            if ((!this.OpacityEnabled || (this.InternalOpacityFractions == null)) || (this.InternalOpacityFractions.Count <= 0))
            {
                item.Opacity = 1.0;
            }
            else
            {
                double currentOpacity = GetCurrentEffectValue(item, this.InternalOpacityFractions);
                item.Opacity = currentOpacity;
            }

            if (CustomPathCarouselPanel.GetPathFraction(item) == 0 || CustomPathCarouselPanel.GetPathFraction(item) == 1)
            {
                item.Opacity = 0;
            }
        }
        #endregion

        #region Scaling
        /// <summary>
        /// 
        /// </summary>
        public bool ScalingEnabled
        {
            get { return (bool)GetValue(ScalingEnabledProperty); }
            set { SetValue(ScalingEnabledProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ScalingEnabledProperty =
            DependencyProperty.Register("ScalingEnabled", typeof(bool), typeof(CarouselPanel), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public PathFractionCollection ScaleFractions
        {
            get { return (PathFractionCollection)GetValue(ScaleFractionsProperty); }
            set { SetValue(ScaleFractionsProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ScaleFractionsProperty =
            DependencyProperty.Register("ScaleFractions", typeof(PathFractionCollection), typeof(CarouselPanel), new PropertyMetadata(null));

        internal PathFractionCollection internalScaleFractions;
        internal PathFractionCollection InternalScaleFractions
        {
            get
            {
                if (CanResetPathFractionCollection(this.internalScaleFractions, this.OpacityFractions))
                {
                    this.internalScaleFractions = null; 
                }
                if (this.internalScaleFractions == null)
                {
                    this.internalScaleFractions = this.ScaleFractions ?? CarouselPanelHelperMethods.GetScaleFractionsCollection();
                }
                return this.internalScaleFractions;
            }
        }

        private void UpdateScaleFractions(UIElement item, out ScaleTransform transform)
        {
            transform = new ScaleTransform(1.0, 1.0);
            if ((!this.ScalingEnabled || (this.InternalScaleFractions == null)) || (this.InternalScaleFractions.Count <= 0))
            {
                transform = new ScaleTransform();
            }
            else
            {
                double currentScale = GetCurrentEffectValue(item, this.InternalScaleFractions);
                transform = new ScaleTransform(currentScale, currentScale);
            }
        }
        
        #endregion

        #region SkewAngleX
        /// <summary>
        /// 
        /// </summary>
        public bool SkewAngleXEnabled
        {
            get { return (bool)GetValue(SkewAngleXEnabledProperty); }
            set { SetValue(SkewAngleXEnabledProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SkewAngleXEnabledProperty =
            DependencyProperty.Register("SkewAngleXEnabled", typeof(bool), typeof(CarouselPanel), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public PathFractionCollection SkewAngleXFractions
        {
            get { return (PathFractionCollection)GetValue(SkewAngleXFractionsProperty); }
            set { SetValue(SkewAngleXFractionsProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SkewAngleXFractionsProperty =
            DependencyProperty.Register("SkewAngleXFractions", typeof(PathFractionCollection), typeof(CarouselPanel), new PropertyMetadata(null));

        internal PathFractionCollection internalSkewAngleXFractions;
        internal PathFractionCollection InternalSkewAngleXFractions
        {
            get
            {
                if (CanResetPathFractionCollection(this.internalSkewAngleXFractions, this.SkewAngleXFractions))
                {
                    this.internalSkewAngleXFractions = null;
                }
                if (this.internalSkewAngleXFractions == null)
                {
                    this.internalSkewAngleXFractions = this.SkewAngleXFractions ?? CarouselPanelHelperMethods.GetSkewAngleXFractionsCollection();
                }
                return this.internalSkewAngleXFractions;
            }
        }

        private void UpdateSkewAngleXFractions(UIElement item, out SkewTransform transform)
        {
            if ((!this.SkewAngleXEnabled || (this.InternalSkewAngleXFractions == null)) || (this.InternalSkewAngleXFractions.Count <= 0))
            {
                transform = new SkewTransform();
            }
            else
            {
                double currentSkewAngleX = GetCurrentEffectValue(item, this.InternalSkewAngleXFractions);
                transform = new SkewTransform(currentSkewAngleX, 0.0);
            }
        }
        #endregion

        #region SkewAngleY
        /// <summary>
        /// 
        /// </summary>
        public bool SkewAngleYEnabled
        {
            get { return (bool)GetValue(SkewAngleYEnabledProperty); }
            set { SetValue(SkewAngleYEnabledProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SkewAngleYEnabledProperty =
            DependencyProperty.Register("SkewAngleYEnabled", typeof(bool), typeof(CarouselPanel), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public PathFractionCollection SkewAngleYFractions
        {
            get { return (PathFractionCollection)GetValue(SkewAngleYFractionsProperty); }
            set { SetValue(SkewAngleYFractionsProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SkewAngleYFractionsProperty =
            DependencyProperty.Register("SkewAngleYFractions", typeof(PathFractionCollection), typeof(CarouselPanel), new PropertyMetadata(null));

        internal PathFractionCollection internalSkewAngleYFractions;
        internal PathFractionCollection InternalSkewAngleYFractions
        {
            get
            {
                if (CanResetPathFractionCollection(this.internalSkewAngleYFractions, this.SkewAngleYFractions))
                {
                    this.internalSkewAngleYFractions = null;
                }
                if (this.internalSkewAngleYFractions == null)
                {
                    this.internalSkewAngleYFractions = this.SkewAngleYFractions ?? CarouselPanelHelperMethods.GetSkewAngleYFractionsCollection();
                }
                return this.internalSkewAngleYFractions;
            }
        }

        private void UpdateSkewAngleYFractions(UIElement item, out SkewTransform transform)
        {
            if ((!this.SkewAngleYEnabled || (this.InternalSkewAngleYFractions == null)) || (this.InternalSkewAngleYFractions.Count <= 0))
            {
                transform = new SkewTransform();
            }
            else
            {
                double currentSkewAngleY = GetCurrentEffectValue(item, this.InternalSkewAngleYFractions);
                transform = new SkewTransform(currentSkewAngleY, 0.0);
            }
        }
        #endregion

        internal static bool CanResetPathFractionCollection(PathFractionCollection internalStops, PathFractionCollection publicStops)
        {
            return (((internalStops != null) && (publicStops != null)) && (internalStops != publicStops));
        }

        private static double GetCurrentEffectValue(UIElement item, PathFractionCollection effectCollection)
        {
            double currentPathFraction = GetPathFraction(item);
            FractionValue leftPoint = null;
            FractionValue rightPoint = null;
            effectCollection.FindNearestPoints(currentPathFraction, out leftPoint, out rightPoint);
            if (leftPoint == null)
            {
                return rightPoint.Value;
            }
            if (rightPoint == null)
            {
                return leftPoint.Value;
            }
            return CalculateChange(currentPathFraction, leftPoint, rightPoint);
        }

        internal static double CalculateChange(double currentPathFraction, FractionValue stop1, FractionValue stop2)
        {
            FractionValue biggerPoint;
            FractionValue smallerPoint;
            if (stop1.Value > stop2.Value)
            {
                biggerPoint = stop1;
                smallerPoint = stop2;
            }
            else
            {
                biggerPoint = stop2;
                smallerPoint = stop1;
            }
            double bigSide = biggerPoint.Value - smallerPoint.Value;
            double smallSide = ((currentPathFraction - smallerPoint.Fraction) * bigSide) / (biggerPoint.Fraction - smallerPoint.Fraction);
            return (smallerPoint.Value + smallSide);
        }
        #endregion

        internal int MaxPanelOffset = 0;
        internal int ViewPanelOffset = 0;
        internal int PanelOffset;

        internal void UpdatePanelOffset(int displacement)
        {
            if (displacement != 0)
            {
                int minOffset = 1;
                int maxOffset = (this.MaxPanelOffset - this.ViewPanelOffset) - 1;

                int Offset = CarouselPanelHelperMethods.CoerceRangeValues(this.PanelOffset + displacement, minOffset, maxOffset);
                this.PanelOffset = Offset;
            }
        }

        private void CheckPanelOffset()
        {
            this.SetMaximumandViewPanelOffset();
            int newOffset = GetOffsetFromCurrentArrangement(this.GetCurrentItemPathArrangement());
            this.PanelOffset = newOffset;
        }

        private void SetMaximumandViewPanelOffset()
        {
            int maxPanelOffset = this.CalculateMaximumOffset();

            if (this.MaxPanelOffset != maxPanelOffset)
            {
                this.MaxPanelOffset = maxPanelOffset;
            }
            if (this.ItemsPerPage != this.ViewPanelOffset)
            {
                this.ViewPanelOffset = this.ItemsPerPage;
            }
        }

        private int CalculateMaximumOffset()
        {
            int maxPanelOffset = 0;
            if (this.CarouselPanelHelper.ItemsCount > 0)
            {
                maxPanelOffset = (this.CarouselPanelHelper.ItemsCount + this.ItemsPerPage) + this.ItemsPerPage;
            }
            return maxPanelOffset;
        }

        private static int GetOffsetFromCurrentArrangement(VisibleItemsHandler arrangement)
        {
            int offset = 0;
            if (arrangement!=null && arrangement.GetUsedPositions() > 0)
            {
                int largestIndex = arrangement.GetLargestItemIndex();
                int freePositionsOfTheLeft = arrangement.GetFreePositionsLeft();
                offset = (largestIndex + freePositionsOfTheLeft) + 1;
            }
            return (int)offset;
        }

        private static readonly DependencyProperty ItemPathFractionManagerProperty;
        internal static PathFractionManager GetPathFractionManager(UIElement element)
        {
            return (PathFractionManager)element.GetValue(ItemPathFractionManagerProperty);
        }

        internal static void SetPathFractionManager(UIElement element, PathFractionManager value)
        {
            element.SetValue(ItemPathFractionManagerProperty, value);
        }

        internal static void SetPathFraction(UIElement element, double value)
        {
            element.SetValue(PathFractionProperty, value);
        }

        internal static double GetPathFraction(UIElement element)
        {
            if (element == null)
            {
                return -1.0;
            }
            return (double)element.GetValue(PathFractionProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isItemSelected"></param>
        public void BringItemIntoView(UIElement item, bool isItemSelected)
        {
            if (base.Children.Contains(item))
            {             
                int moveBy = this.GetMovementOffsetFromTopElement(item);
                if (isItemSelected)
                {
                    
                }
                this.MoveItemInternallyBy(moveBy);
            }
        }

        private int GetMovementOffsetFromTopElement(UIElement element)
        {
            double pathFraction = GetPathFraction(element);
            if (pathFraction == -1.0)
            {
                return 0;
            }
            int itemPosition = this.carouselPathHelper.GetPathFractionIndex(pathFraction);
            return (this.carouselPathHelper.TopElementPathFractionIndex - itemPosition);
        }
        /// <summary>
        /// Calls when Selected Item Out Of Page
        /// </summary>
        /// <param name="displacement"></param>
        internal void MoveItemInternallyforoutofrange(int displacement)
        {
            if (this.CurrentVirtualizingPanelHandler != null)
            {
                VirtualizingPanelItemMoveHandler _VirtualizingPanelHandler = this.CurrentVirtualizingPanelHandler as VirtualizingPanelItemMoveHandler;
                if ((_VirtualizingPanelHandler != null) && _VirtualizingPanelHandler.IsOpposite(displacement))
                {
                    _VirtualizingPanelHandler.Reverse();
                    return;
                }
            }
            else
            {
               this.FinishItemMovements();
                if (displacement != 0)
                {
                    if ((this.NewVirtualizingPanelHandler == null) && base.IsInitialized)
                    {
                        this.NewVirtualizingPanelHandler = new VirtualizingPanelItemMoveHandler(displacement, this.CarouselPanelHelper);
                        this.Invalidate(false);
                    }
                }
            }           
        }

        VirtualizingPanelHandler CurrentVirtualizingPanelHandler;
        VirtualizingPanelHandler OldVirtualizingPanelHandler;
       internal VirtualizingPanelHandler NewVirtualizingPanelHandler;
      internal  VirtualizingPanelHandler tempVirtualizingPanelHandler;

        internal void MoveItemInternallyBy(int displacement)
        {
            if (this.CurrentVirtualizingPanelHandler != null)
            {
                VirtualizingPanelItemMoveHandler _VirtualizingPanelHandler = this.CurrentVirtualizingPanelHandler as VirtualizingPanelItemMoveHandler;
                if ((_VirtualizingPanelHandler != null) && _VirtualizingPanelHandler.IsOpposite(displacement))
                {
                    _VirtualizingPanelHandler.Reverse();
                    return;
                }
            }
            else
            {
                this.FinishItemMovements();
                int newDisplacement = this.CoerceDisplacement(displacement);
                if (newDisplacement != 0)
                {
                    if ((this.NewVirtualizingPanelHandler == null) && base.IsInitialized)
                    {
                        this.NewVirtualizingPanelHandler = new VirtualizingPanelItemMoveHandler(newDisplacement, this.CarouselPanelHelper);
                        this.Invalidate(false);
                    }
                }
            }
        }

        internal void Invalidate(bool invalidateVisual)
        {
            if (invalidateVisual)
            {
                base.InvalidateVisual();
            }
            else
            {
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        private void Start_ItemMovement()
        {
            if (this.NewVirtualizingPanelHandler != null)
            {
                this.PrepareItemsToMove(this.NewVirtualizingPanelHandler);
                if (this.CurrentVirtualizingPanelHandler == null)
                {
                    CompositionTarget.Rendering += new EventHandler(this.TargetRendering);
                }
                this.OldVirtualizingPanelHandler = this.NewVirtualizingPanelHandler;
                this.NewVirtualizingPanelHandler = null;
            }
        }

        private void PrepareItemsToMove(VirtualizingPanelHandler _VirtualizingPanelHandler)
        {
            List<VisiblePanelItem> itemsToAnimate = this.m_PathFractionRangeHandler.ToList<VisiblePanelItem>();
            VirtualizingPanelItemMoveHandler _VirtualizingPanelItemMoveHandler = _VirtualizingPanelHandler as VirtualizingPanelItemMoveHandler;
            if ((_VirtualizingPanelItemMoveHandler != null) && (_VirtualizingPanelItemMoveHandler.PathDisplacement < 0))
            {
                itemsToAnimate.Reverse();
            }
            _VirtualizingPanelHandler.Duration = new TimeSpan(0, 0, 0, 0, 300);// this.ItemsMovementAnimationDuration;
            foreach (VisiblePanelItem currentElement in itemsToAnimate)
            {
                currentElement.Child.RenderTransformOrigin = new Point(0.5, 0.5);
                _VirtualizingPanelHandler.AddItemToMove(currentElement);
            }
        }

        private int CoerceDisplacement(int displacement)
        {
            int newDisplacement = displacement;
            VisibleItemsHandler arrangement = this.GetCurrentItemPathArrangement();
            int itemsAfter = GetItemCountLater(this.m_PathFractionRangeHandler, this.CarouselPanelHelper.ItemsCount);
            int itemsBefore = GetItemCountBefore(this.m_PathFractionRangeHandler);
            if (arrangement == null)
            {
                return 0;
            }
            if (displacement < 0)
            {
                int used = arrangement.GetUsedPositions();
                int fromLeft = arrangement.GetFreePositionsLeft();
                int max = ((used + fromLeft) - 1) + itemsBefore;
                return -Math.Min(max, -displacement);
            }
            if (displacement > 0)
            {
                int used = arrangement.GetUsedPositions();
                int fromRight = arrangement.GetFreePositionsRight();
                int max = ((used + fromRight) - 1) + itemsAfter;
                newDisplacement = Math.Min(max, displacement);
            }
            return newDisplacement;
        }

        internal static int GetItemCountLater(PathFractionRangeHandler range, int itemCount)
        {
            if (range.LastVisibleItemIndex >= itemCount)
            {
                return 0;
            }
            return ((itemCount - range.LastVisibleItemIndex) - 1);
        }

        internal static int GetItemCountBefore(PathFractionRangeHandler range)
        {
            if (range.FirstVisibleItemIndex < 0)
            {
                return 0;
            }
            return range.FirstVisibleItemIndex;
        }

        private void InitializeItemMovement()
        {
            if (this.NewVirtualizingPanelHandler != null)
            {
                tempVirtualizingPanelHandler = this.NewVirtualizingPanelHandler;
                VisibleRangeAction action;
                LinkedList<VisiblePanelItem> pairs;
                this.NewVirtualizingPanelHandler.Initialize(this.carouselPathHelper, this.GetCurrentItemPathArrangement());
                this.NewVirtualizingPanelHandler.CalculateItemsToAdd(out action, out pairs);
                this.m_PathFractionRangeHandler.UpdateVisibleRange(action, pairs);
            }
        }

        internal void FinishItemMovements()
        {
            if (this.CurrentVirtualizingPanelHandler != null)
            {
                this.UpdateVisualization();
                CompositionTarget.Rendering -= new EventHandler(this.TargetRendering);
                this.CurrentVirtualizingPanelHandler.EndItemMovement();
                IList<VisiblePanelItem> itemsToRemove = this.CurrentVirtualizingPanelHandler.GetItemsToRemoveEndofArrangeOverride();
                if ((itemsToRemove != null) && (itemsToRemove.Count > 0))
                {
                    foreach (VisiblePanelItem currentElement in itemsToRemove)
                    {
                        SetPathFraction(currentElement.Child, -1.0);
                    }
                }
                this.m_PathFractionRangeHandler.ScheduleClean(itemsToRemove);
                this.CurrentVirtualizingPanelHandler = null;               
                this.CheckPanelOffset();
                this.Invalidate(false);
            }
        }

        /// <summary>
        /// Checks the selected item.
        /// </summary>
        private void CheckSelectedItem()
        {
            if (Owner != null)
            {
                UIElement element = FindClosestElementToPathFraction(Owner.TopItemPosition);
                if (element!=null)
                {
                    if (Owner.SelectedItem == null && element is CarouselItem)
                    {
                        int index = Owner.Items.IndexOf((element as CarouselItem).DataContext);
                        if (index == -1)
                        {
                            index = Owner.Items.IndexOf(element as CarouselItem);
                        }
                        Owner.SelectedIndex = index;
                    }
                    else if(Owner.SelectedItem != null && element is CarouselItem)
                    {
                        if (!element.Equals(Owner.SelectedItem) && Owner.SelectedItem is CarouselItem)
                        {
                            this.BringItemIntoView(Owner.SelectedItem as CarouselItem, true);
                        }
                    }
                }
            }
        }

        private void TargetRendering(object sender, EventArgs e)
        {
            RenderingEventArgs renderArgs = (RenderingEventArgs)e;

            if (this.OldVirtualizingPanelHandler != null)
            {
                this.CurrentVirtualizingPanelHandler = this.OldVirtualizingPanelHandler;
                this.OldVirtualizingPanelHandler = null;
                this.CurrentVirtualizingPanelHandler.BeginItemMovement(renderArgs.RenderingTime);              
            }

            if (this.CurrentVirtualizingPanelHandler != null)
            {
                this.CurrentVirtualizingPanelHandler.Update(renderArgs.RenderingTime);
                this.UpdateVisualization();

                if (this.CurrentVirtualizingPanelHandler.State == ItemMovementState.Finished)
                {
                    this.FinishItemMovements();
                    CheckSelectedItem();
                }
            }
        }

        internal void Refresh(bool isCollectionModified, int pathDisplacement)
        {
            isRefreshing = true;
            CarouselPanelHelper = new CarouselPanelHelper(this);
            this.CurrentVirtualizingPanelHandler = null;
            if (this.Owner.ItemsPerPage == -1 || (this.Owner.Items.Count <= this.Owner.ItemsPerPage && this.Owner.Items.Count > 0))
            {
                ItemsPerPage = this.Owner.Items.Count;
            }
            else
            {
                ItemsPerPage = this.Owner.ItemsPerPage;
            }
            int displacement = CoerceDisplacement(this.ItemsPerPage);
            m_PathFractionRangeHandler = new PathFractionRangeHandler();
            this.CheckPanelOffset();
            this.NewVirtualizingPanelHandler = null;
            if (!isCollectionModified)
            {
                MoveBy(displacement);
            }
            else
            {
                SetPaths();
                displacement = this.Owner.SelectedIndex + this.carouselPathHelper.TopElementPathFractionIndex;
                if (pathDisplacement < 0)
                {
                    this.PanelOffset = displacement + 1;
                    displacement = pathDisplacement;
                }
                MoveBy(displacement);
            }
            isRefreshing = false;
        }
    }
}
