#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
//using System;
//using System.Net;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System;
using System.Collections.Specialized;
//using System.Windows.Media.Media3D;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// 
    /// </summary>
    public class CarouselPanel : VirtualizingPanel
    {
        private static readonly DependencyProperty NormalizedOffsetProperty;
        private static readonly DependencyProperty PlaneProjectionProperty;
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
            DependencyProperty.Register("ItemsPerPage", typeof(int), typeof(CarouselPanel), new PropertyMetadata(7));

        /// <summary>
        /// 
        /// </summary>
        public double GlobalOffsetX
        {
            get { return (double)GetValue(GlobalOffsetXProperty); }
            set { SetValue(GlobalOffsetXProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GlobalOffsetXProperty =
            DependencyProperty.Register("GlobalOffsetX", typeof(double), typeof(CarouselPanel), new PropertyMetadata(0.4));

        /// <summary>
        /// Gets or sets the global offset Y.
        /// </summary>
        /// <value>The global offset Y.</value>
        public double GlobalOffsetY
        {
            get { return (double)GetValue(GlobalOffsetYProperty); }
            set { SetValue(GlobalOffsetYProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GlobalOffsetYProperty =
            DependencyProperty.Register("GlobalOffsetY", typeof(double), typeof(CarouselPanel), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the global offset Z.
        /// </summary>
        /// <value>The global offset Z.</value>
        public double GlobalOffsetZ
        {
            get { return (double)GetValue(GlobalOffsetZProperty); }
            set { SetValue(GlobalOffsetZProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GlobalOffsetZProperty =
            DependencyProperty.Register("GlobalOffsetZ", typeof(double), typeof(CarouselPanel), new PropertyMetadata(0.5));

        private double _layoutOffset;
        private int FirstVisibleIndex, LastVisibleIndex;

        /// <summary>
        /// Get or set the Layout offset
        /// </summary>
        public double LayoutOffset
        {
            get { return _layoutOffset; }
            set
            {
                _layoutOffset = value;
                InvalidateMeasure();
            }
        }

        private bool _cyclicMode = true;

        /// <summary>
        /// Get or set the Cyclic mode
        /// </summary>
        public bool CyclicMode
        {
            get { return _cyclicMode; }
            set
            {
                _cyclicMode = value;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Get or set the total item count
        /// </summary>
        public int ItemCount
        {
            get { return this.GetItemsCount(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetItemsCount()
        {
            int children = this.Children.Count;
            if (this.IsItemsHost)
            {
                children = this.GetParentItemsControl().Items.Count;
            }
            return children;
        }

        private ItemsControl GetParentItemsControl()
        {
            ItemsControl parent = null;
            if (this.IsItemsHost)
            {
                parent = ItemsControl.GetItemsOwner(this);
            }
            return parent;
        }

        private double _consideredItems;

        /// <summary>
        /// Get or set the number of items to consider in a layout pass
        /// </summary>
        public double ConsideredItems
        {
            get { return _consideredItems; }
            set { _consideredItems = value; }
        }

        static CarouselPanel()
        {
            NormalizedOffsetProperty = DependencyProperty.RegisterAttached("NormalizedOffset", typeof(double), typeof(CarouselPanel), new PropertyMetadata(0.0));
            PlaneProjectionProperty = DependencyProperty.RegisterAttached("PlaneProjection", typeof(PlaneProjection), typeof(CarouselPanel), new PropertyMetadata(new PlaneProjection()));
        }

        /// <summary>
        /// 
        /// </summary>
        public CarouselPanel()
        {

        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsPositiveInfinity(availableSize.Width) || double.IsPositiveInfinity(availableSize.Height) || ItemCount == 0)
            {
                return new Size(0, 0);
            }

            CalculateIndices();
            UpdateVisibleRanges();


            if (ItemsControl.GetItemsOwner(this).Items.Count == 0)
                return availableSize;

            for (int i = 0; i < _usedRanges; i++)
            {
                IsChildCreated(VisibleRangeHandler[i * 2], VisibleRangeHandler[i * 2 + 1], availableSize);
            }


            for (int i = Children.Count - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPosition = new GeneratorPosition(i, 0);
                int itemIndex = ItemContainerGenerator.IndexFromGeneratorPosition(childGeneratorPosition);
                if (!IsInVisibleRange(itemIndex))
                {
                    ItemContainerGenerator.Remove(childGeneratorPosition, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }

            UpdateZOrder(Children.OfType<UIElement>());

            return availableSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
            }
        }

        private void IsChildCreated(int firstVisibleIndex, int lastVisibleIndex, Size availableSize)
        {
            var children = Children;
            IItemContainerGenerator generator = ItemContainerGenerator;
            GeneratorPosition start = generator.GeneratorPositionFromIndex(firstVisibleIndex);
            int childIndex = (start.Offset == 0) ? start.Index : start.Index + 1;
            using (generator.StartAt(start, GeneratorDirection.Forward, true))
            {
                for (int i = firstVisibleIndex; i <= lastVisibleIndex; ++i, ++childIndex)
                {
                    bool isNewlyRealized;

                    UIElement child = generator.GenerateNext(out isNewlyRealized) as UIElement;
                    if (isNewlyRealized)
                    {
                        if (childIndex >= children.Count)
                        {
                            base.AddInternalChild(child);
                        }
                        else
                        {
                            base.InsertInternalChild(childIndex, child);
                        }
                        generator.PrepareItemContainer(child);
                    }
                    else
                    {
                        Debug.Assert(child == children[childIndex]);
                    }

                    child.Measure(availableSize);
                    double relativeItemIndex = CalculateRelativeIndex(ItemsControl.GetItemsOwner(this).ItemContainerGenerator.IndexFromContainer(child));


                    UpdateVisualization(relativeItemIndex, child, (int)_consideredItems, availableSize);
                }
            }
        }

        private void UpdateZOrder(IEnumerable<UIElement> children)
        {
            int zindex = 1;
            foreach (var child in children.Cast<CarouselItem>().OrderBy(c => CarouselPanel.GetPlaneProjectionProperty(c).GlobalOffsetZ))
            {
                Canvas.SetZIndex(child, zindex++);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {

                ArrangeChild(finalSize, child);
            }
            return finalSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <param name="child"></param>
        protected virtual void ArrangeChild(Size finalSize, UIElement child)
        {
            //if (child.DesiredSize.Height == 0.0 || child.DesiredSize.Width == 0.0)
            //    return;
            //child.Arrange(new Rect(finalSize.Width * 0.5 - child.DesiredSize.Width * 0.5, finalSize.Height * 0.5 - child.DesiredSize.Height * 0.5, child.DesiredSize.Width, child.DesiredSize.Height));

            if (child.DesiredSize.Height == 0.0 || child.DesiredSize.Width == 0.0)
                return;
            //child.Arrange(new Rect(
            //    finalSize.Width * 0.5 - child.DesiredSize.Width * 0.5, 
            //    finalSize.Height * 0.5 - child.DesiredSize.Height * 0.5,
            //    child.DesiredSize.Width,
            //    child.DesiredSize.Height));

            child.Projection = CarouselPanel.GetPlaneProjectionProperty(child);

            child.Arrange(new Rect(
                finalSize.Width * 0.5 - child.DesiredSize.Width * 0.5,
                (finalSize.Height * 0.5 - child.DesiredSize.Height * 0.5) / 2,
                child.DesiredSize.Width,
                child.DesiredSize.Height));
        }

        private void UpdateVisualization(double normalizedIndex, UIElement element, int consideredItemsCount, Size viewportsize)
        {
            if (element != null)
            {
                CarouselPanel.SetNormalizedOffsetProperty(element, normalizedIndex);
                ApplyPlanProjectionForChild(element, normalizedIndex, viewportsize);
            }
        }

        private void ApplyPlanProjectionForChild(UIElement child, double offset, Size viewportsize)
        {
            var piFactor = offset * Math.PI;
            PlaneProjection _PlaneProjection = new PlaneProjection();
            _PlaneProjection.GlobalOffsetX = viewportsize.Width * GlobalOffsetX * Math.Sin(piFactor);
            _PlaneProjection.GlobalOffsetY = GlobalOffsetY * (Math.Cos(piFactor) - 1) * viewportsize.Height + 0.5 * viewportsize.Height;
            _PlaneProjection.GlobalOffsetZ = viewportsize.Width * GlobalOffsetZ * (Math.Cos(piFactor) - 1);

            //item.Transform3D.GlobalOffsetX = _availableSize.Width * .4 * Math.Sin(piFactor);
            //item.Transform3D.GlobalOffsetZ = _availableSize.Width * .6 * (Math.Cos(piFactor) - 1);
            //item.Transform3D.GlobalOffsetY = 0.0 * (Math.Cos(piFactor) - 1) * _availableSize.Height + 0.2 * _availableSize.Height;

            CarouselPanel.SetPlaneProjectionProperty(child, _PlaneProjection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public static double GetNormalizedOffsetProperty(UIElement child)
        {
            return (double)child.GetValue(NormalizedOffsetProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="value"></param>
        public static void SetNormalizedOffsetProperty(UIElement child, double value)
        {
            child.SetValue(NormalizedOffsetProperty, value);
        }

        double CalculateRelativeIndex(int itemIndex)
        {
            double relativeIndex = itemIndex - _layoutOffset;
            if (relativeIndex < 0)
                relativeIndex += ItemCount;
            if (Math.Abs(relativeIndex - ItemCount) < Math.Abs(relativeIndex))
                relativeIndex = relativeIndex - ItemCount;
            return relativeIndex * 2.0 / _consideredItems;
        }

        private int _usedRanges;
        int[] VisibleRangeHandler = new int[6];

        private bool IsInVisibleRange(int index)
        {
            for (int i = 0; i < _usedRanges; i++)
            {
                if (index >= VisibleRangeHandler[i * 2] && index <= VisibleRangeHandler[i * 2 + 1])
                    return true;
            }
            return false;
        }

        private void UpdateVisibleRanges()
        {
            _usedRanges = 0;
            if (FirstVisibleIndex < 0)
            {
                ++_usedRanges;
                VisibleRangeHandler[0] = ItemCount + FirstVisibleIndex;
                VisibleRangeHandler[1] = ItemCount - 1;
            }
            VisibleRangeHandler[2 * _usedRanges] = FirstVisibleIndex > 0 ? FirstVisibleIndex : 0;
            VisibleRangeHandler[2 * _usedRanges + 1] = LastVisibleIndex >= ItemCount ? ItemCount - 1 : LastVisibleIndex;
            ++_usedRanges;
            if (LastVisibleIndex >= ItemCount)
            {
                VisibleRangeHandler[2 * _usedRanges] = 0;
                VisibleRangeHandler[2 * _usedRanges + 1] = LastVisibleIndex - ItemCount;
                _usedRanges++;
            }
        }

        private void CalculateIndices()
        {
            int layoutMax = ItemsPerPage;
            if (layoutMax == -1)
                return;

            var totalItems = Math.Min(layoutMax, ItemCount);
            _consideredItems = totalItems;
            double halfTotal = 0.5 * totalItems;
            FirstVisibleIndex = (int)Math.Ceiling(_layoutOffset - halfTotal);
            LastVisibleIndex = (int)Math.Floor(_layoutOffset + halfTotal);

            if (CyclicMode)
            {
                if (LastVisibleIndex - FirstVisibleIndex < _consideredItems)
                    LastVisibleIndex++;
            }
            if (LastVisibleIndex - FirstVisibleIndex >= _consideredItems)
                LastVisibleIndex--;
            _consideredItems = totalItems;
            while (FirstVisibleIndex <= -ItemCount)
            {
                FirstVisibleIndex += ItemCount;
                LastVisibleIndex += ItemCount;
            }
            while (FirstVisibleIndex >= ItemCount)
            {
                FirstVisibleIndex -= ItemCount;
                LastVisibleIndex -= ItemCount;
            }
            if (!_cyclicMode)
            {

                if (LastVisibleIndex >= ItemCount - 1)
                    LastVisibleIndex = ItemCount - 1;
                if (FirstVisibleIndex < 0)
                    FirstVisibleIndex = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public static PlaneProjection GetPlaneProjectionProperty(UIElement child)
        {
            return (PlaneProjection)child.GetValue(PlaneProjectionProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="value"></param>
        public static void SetPlaneProjectionProperty(UIElement child, PlaneProjection value)
        {
            child.SetValue(PlaneProjectionProperty, value);
        }
    }
}