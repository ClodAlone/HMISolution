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
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections;
using System.Globalization;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    internal class CarouselPanelHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private CustomPathCarouselPanel carouselPanel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselPanelHelper"/> class.
        /// </summary>
        /// <param name="panel">The panel.</param>
        public CarouselPanelHelper(CustomPathCarouselPanel panel)
        {
            this.carouselPanel = panel;
        }

        /// <summary>
        /// Gets the items count.
        /// </summary>
        /// <returns></returns>
        public int ItemCount()
        {
            int children = this.carouselPanel.Children.Count;
            if (this.carouselPanel.IsItemsHost)
            {
                children = this.ParentItem().Items.Count;
            }
            return children;
        }

        /// <summary>
        /// Gets the parent items control.
        /// </summary>
        /// <returns></returns>
        private ItemsControl ParentItem()
        {
            ItemsControl parent = null;
            if (this.carouselPanel.IsItemsHost)
            {
                parent = ItemsControl.GetItemsOwner(this.carouselPanel);
            }
            return parent;
        }

        /// <summary>
        /// Gets the items count.
        /// </summary>
        /// <value>The items count.</value>
        public int ItemsCount
        {
            get
            {
                return this.ItemCount();
            }
        }

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get
            {
                return this.carouselPanel.ItemsPerPage;
            }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public int Position
        {
            get
            {
                return (int)this.carouselPanel.PanelOffset;
            }
        }
    }

    internal static class CarouselPanelHelperMethods
    {
        /// <summary>
        /// Gets the default path.
        /// </summary>
        /// <returns></returns>
        public static Path GetPath()
        {
            System.Windows.Shapes.Path newPath = new System.Windows.Shapes.Path();
            object geometryFigures = new PathFigureCollectionConverter().ConvertFromString("M639,-115.5 C702,-106.5 666.49972,-35 491.49972,-35 300.4994,-35 293.49973,-116 343.50004,-116");
            PathGeometry newGeometry = new PathGeometry();
            newPath.Stretch = Stretch.Fill;
            BrushConverter brushConverter = new BrushConverter();
            newPath.Stroke = (Brush)brushConverter.ConvertFromString("#FF0998f8");
            newPath.StrokeThickness = 2.0;
            newGeometry.Figures = (PathFigureCollection)geometryFigures;
            newPath.Data = newGeometry;
            return newPath;

        }

        /// <summary>
        /// Gets the default opacity fractions collection.
        /// </summary>
        /// <returns></returns>
        public static PathFractionCollection GetOpacityFractionsCollection()
        {
            PathFractionCollection opacityFractions = new PathFractionCollection();
            opacityFractions.Add(new FractionValue() { Fraction = 0.0, Value = 0.5 });
            opacityFractions.Add(new FractionValue() { Fraction = 0.5, Value = 1 });
            opacityFractions.Add(new FractionValue() { Fraction = 1, Value = 0.5 });
            return opacityFractions;
        }

        /// <summary>
        /// Gets the default scale fractions collection.
        /// </summary>
        /// <returns></returns>
        public static PathFractionCollection GetScaleFractionsCollection()
        {
            PathFractionCollection scaleFractions = new PathFractionCollection();
            scaleFractions.Add(new FractionValue() { Fraction = 0.0, Value = 0.3 });
            scaleFractions.Add(new FractionValue() { Fraction = 0.5, Value = 1 });
            scaleFractions.Add(new FractionValue() { Fraction = 1, Value = 0.3 });
            return scaleFractions;
        }

        /// <summary>
        /// Gets the default skew angle X fractions collection.
        /// </summary>
        /// <returns></returns>
        public static PathFractionCollection GetSkewAngleXFractionsCollection()
        {
            return new PathFractionCollection();
        }

        /// <summary>
        /// Gets the default skew angle Y fractions collection.
        /// </summary>
        /// <returns></returns>
        public static PathFractionCollection GetSkewAngleYFractionsCollection()
        {
            return new PathFractionCollection();
        }

        /// <summary>
        /// Gets the item count after.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="itemCount">The item count.</param>
        /// <returns></returns>
        internal static int GetItemCountlater(PathFractionRangeHandler range, int itemCount)
        {
            if (range.LastVisibleItemIndex >= itemCount)
            {
                return 0;
            }
            return ((itemCount - range.LastVisibleItemIndex) - 1);
        }

        /// <summary>
        /// Gets the item count before.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        internal static int GetItemCountBefore(PathFractionRangeHandler range)
        {
            if (range.FirstVisibleItemIndex < 0)
            {
                return 0;
            }
            return range.FirstVisibleItemIndex;
        }

        /// <summary>
        /// Determines whether [is in range] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>
        /// 	<c>true</c> if [is in range] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInRange(int value, int min, int max)
        {
            return ((value >= min) && (value <= max));
        }

        /// <summary>
        /// Coerces the value between range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        public static int CoerceRangeValues(int value, int min, int max)
        {
            int newValue = 0;
            if (value > max)
            {
                newValue = max;
            }
            else
            {
                newValue = value;
            }
            if (newValue < min)
            {
                newValue = min;
            }
            return newValue;
        }
    }
    internal class CarouselPathHelper
    {

        #region Privatemembers

        /// <summary>
        /// 
        /// </summary>
        private Path _CarouselPath;
        /// <summary>
        /// 
        /// </summary>
        private PathGeometry _Geometry;
        /// <summary>
        /// 
        /// </summary>
        private PathFractions[] _PathFractions;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselPathHelper"/> class.
        /// </summary>
        /// <param name="Path">The path.</param>
        /// <param name="ItemsPerPage">The items per page.</param>
        public CarouselPathHelper(Path Path, int ItemsPerPage)
        {
            if (Path.Data != null)
                this.Geometry = PathGeometry.CreateFromGeometry(Path.Data);
            else
                this.Geometry = PathGeometry.CreateFromGeometry(System.Windows.Media.Geometry.Empty);

            this.CarouselPath = Path;
            this._PathFractions = CarouselPathHelper.PathFraction(ItemsPerPage);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the carousel path.
        /// </summary>
        /// <value>The carousel path.</value>
        public Path CarouselPath
        {
            get { return _CarouselPath; }
            set { _CarouselPath = value; }
        }

        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>The geometry.</value>
        public PathGeometry Geometry
        {
            get { return _Geometry; }
            set { _Geometry = value; }
        }

        /// <summary>
        /// Gets the path fractions.
        /// </summary>
        /// <value>The path fractions.</value>
        public PathFractions[] PathFractions
        {
            get { return _PathFractions; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal PathFractions topElementPathFraction;
        /// <summary>
        /// Gets the index of the top element path fraction.
        /// </summary>
        /// <value>The index of the top element path fraction.</value>
        public int TopElementPathFractionIndex
        {
            get
            {
                return this.GetPathFractionIndex(this.topElementPathFraction.PathFraction);
            }
        }

        /// <summary>
        /// Gets the top element path fraction.
        /// </summary>
        /// <value>The top element path fraction.</value>
        public double TopElementPathFraction
        {
            get
            {
                return this.topElementPathFraction.PathFraction;
            }
        }

        /// <summary>
        /// Sets the top element path fraction.
        /// </summary>
        /// <param name="desiredPathFraction">The desired path fraction.</param>
        public void SetTopElementPathFraction(PathFractions desiredPathFraction)
        {
            desiredPathFraction.PathFraction = this.PathFractions[this.PathFractions.Count() / 2].PathFraction;
            this.topElementPathFraction = new PathFractions(desiredPathFraction.PathFraction);
            if (!this.IsPathFractionDifferentFromStartAndEndFractions(desiredPathFraction))
            {
                PathFractions leftPoint = this.NearestPathFractionAtLeft(desiredPathFraction.PathFraction);
                PathFractions rightPoint = this.NearestPathFractionAtRight(desiredPathFraction.PathFraction);
                if ((leftPoint == null) || (leftPoint.PathFraction == 0.0))
                {
                    this.topElementPathFraction = rightPoint;
                }
                else if ((rightPoint == null) || (rightPoint.PathFraction == 1.0))
                {
                    this.topElementPathFraction = leftPoint;
                }
                else if (Math.Abs((double)(leftPoint.PathFraction - desiredPathFraction.PathFraction)) <= Math.Abs((double)(rightPoint.PathFraction - desiredPathFraction.PathFraction)))
                {
                    this.topElementPathFraction = leftPoint;
                }
                else
                {
                    this.topElementPathFraction = rightPoint;
                }
            }
        }

        /// <summary>
        /// Finds the left nearest path fraction.
        /// </summary>
        /// <param name="pathFraction">The path fraction.</param>
        /// <returns></returns>
        public PathFractions NearestPathFractionAtLeft(double pathFraction)
        {
            int controlPointIndex = NearestPathFractionAtLeft(this.PathFractions, pathFraction);
            if (controlPointIndex != -1)
            {
                return this.PathFractions[controlPointIndex];
            }
            return null;
        }

        /// <summary>
        /// Finds the left nearest path fraction.
        /// </summary>
        /// <param name="_pathfractions">The _pathfractions.</param>
        /// <param name="pathFraction">The path fraction.</param>
        /// <returns></returns>
        public static int NearestPathFractionAtLeft(PathFractions[] _pathfractions, double pathFraction)
        {
            int leftNearestIndex = -1;
            int foundIndex = FindNearestPathFractionIndex(pathFraction, _pathfractions);
            if (foundIndex < 0)
            {
                int firstLargerIndex = Math.Abs(foundIndex) - 1;
                if ((firstLargerIndex - 1) >= 0)
                {
                    leftNearestIndex = firstLargerIndex - 1;
                }
                return leftNearestIndex;
            }
            if ((foundIndex - 1) >= 0)
            {
                leftNearestIndex = foundIndex - 1;
            }
            return leftNearestIndex;
        }

        /// <summary>
        /// Finds the index of the nearest path fraction.
        /// </summary>
        /// <param name="pathFraction">The path fraction.</param>
        /// <param name="PathFractions">The path fractions.</param>
        /// <returns></returns>
        private static int FindNearestPathFractionIndex(double pathFraction, PathFractions[] PathFractions)
        {
            PathFractions _pathfraction = new PathFractions(pathFraction);

            int i = 0;
            foreach (PathFractions item in PathFractions)
            {
                if (item.PathFraction >= pathFraction)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Finds the right nearest path fraction.
        /// </summary>
        /// <param name="pathFraction">The path fraction.</param>
        /// <returns></returns>
        public PathFractions NearestPathFractionAtRight(double pathFraction)
        {
            int pathFractionIndex = NearestPathFractionAtRight(this.PathFractions, pathFraction);
            if (pathFractionIndex != -1)
            {
                return this.PathFractions[pathFractionIndex];
            }
            return null;
        }

        /// <summary>
        /// Finds the right nearest path fraction.
        /// </summary>
        /// <param name="_pathFractions">The _path fractions.</param>
        /// <param name="pathFraction">The path fraction.</param>
        /// <returns></returns>
        public static int NearestPathFractionAtRight(PathFractions[] _pathFractions, double pathFraction)
        {
            int rightNearestIndex = -1;
            int foundIndex = FindNearestPathFractionIndex(pathFraction, _pathFractions);
            if (foundIndex < 0)
            {
                int firstLargerIndex = Math.Abs(foundIndex) - 1;
                if (firstLargerIndex < _pathFractions.Length)
                {
                    rightNearestIndex = firstLargerIndex;
                }
                return rightNearestIndex;
            }
            if ((foundIndex + 1) < _pathFractions.Length)
            {
                rightNearestIndex = foundIndex + 1;
            }
            return rightNearestIndex;
        }

        /// <summary>
        /// Determines whether [is path fraction different from start and end fractions] [the specified _path fraction].
        /// </summary>
        /// <param name="_pathFraction">The _path fraction.</param>
        /// <returns>
        /// 	<c>true</c> if [is path fraction different from start and end fractions] [the specified _path fraction]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPathFractionDifferentFromStartAndEndFractions(PathFractions _pathFraction)
        {
            return ((this.IsPathFraction(_pathFraction.PathFraction) && (_pathFraction.PathFraction != 0.0)) && (_pathFraction.PathFraction != 1.0));
        }

        /// <summary>
        /// Determines whether [is path fraction] [the specified path fraction].
        /// </summary>
        /// <param name="pathFraction">The path fraction.</param>
        /// <returns>
        /// 	<c>true</c> if [is path fraction] [the specified path fraction]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPathFraction(double pathFraction)
        {
            return (this.GetPathFractionIndex(pathFraction) >= 0);
        }
        #endregion

        /// <summary>
        /// Splits the Path based on no of items in the page.
        /// </summary>
        /// <param name="ItemsPerPage">The items per page.</param>
        /// <returns></returns>
        public static PathFractions[] PathFraction(int ItemsPerPage)
        {
            int NoOfFractions = 0;
            if (ItemsPerPage%2 == 0)
            {
                NoOfFractions = ItemsPerPage + 3;
            }

            else
            {
                NoOfFractions = ItemsPerPage + 2;
                
            }

            PathFractions[] PathFractions = new PathFractions[NoOfFractions];
                double distanceRatio = Math.Round((double) 1/(NoOfFractions - 1), 3);

                for (int i = 0; i < NoOfFractions; i++)
                {
                    double Fraction = Math.Round(distanceRatio*i, 3);
                    PathFractions.SetValue(new PathFractions(Fraction), i);
                }
           
            return PathFractions;
        }

        /// <summary>
        /// Updating Path positions based on the available size.
        /// </summary>
        /// <param name="availablesize">The availablesize.</param>
        /// <param name="padding">The padding.</param>
        public void UpdateCustomPath(Size availablesize, Thickness padding)
        {
            if (this.Geometry != null)
            {
                this.Geometry.Transform = null;
                Rect ViewPort = new Rect(padding.Left, padding.Top, (double)availablesize.Width - (padding.Left + padding.Right), (double)availablesize.Height - (padding.Top + padding.Bottom));
                ScaleTransform scaleTransform = ScaleCustomPathWithAvailableSize(ViewPort, this.Geometry.Bounds, this.CarouselPath.Stretch);
                Rect TransformedGeometryBounds = ChangeGeometryBounds(this.Geometry.Bounds, scaleTransform.Value);
                TranslateTransform translateTransform =ChangeCustomPathToAvailableSize(ViewPort, TransformedGeometryBounds);

                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleTransform);
                transformGroup.Children.Add(translateTransform);
                translateTransform.Freeze();

                this.Geometry.Transform = transformGroup;
            }
        }

        /// <summary>
        /// Scales the size of the path to AvailableSize
        /// </summary>
        /// <param name="ViewPort">The view port.</param>
        /// <param name="GeometryBounds">The geometry bounds.</param>
        /// <param name="Stretch">The stretch.</param>
        /// <returns></returns>
        private ScaleTransform ScaleCustomPathWithAvailableSize(Rect ViewPort, Rect GeometryBounds, Stretch Stretch)
        {
#if new

            //----
            double PathViewPortWidth = double.IsNaN(GeometryBounds.Width) ? ViewPort.Width : GeometryBounds.Width;
            double PathViewPortHeight = double.IsNaN(GeometryBounds.Height) ? ViewPort.Height : GeometryBounds.Height;
            //----
            double ScaleX = GeometryBounds.Width == 0 ? PathViewPortWidth : ViewPort.Width / GeometryBounds.Width;
            double ScaleY = GeometryBounds.Height == 0 ? PathViewPortHeight : ViewPort.Height / GeometryBounds.Height;
            //----
            switch (Stretch)
            {
                case Stretch.None:
                    ScaleX = ScaleY = 1;
                    break;
                case Stretch.Fill:
                    break;
                case Stretch.Uniform:
                    ScaleX = ScaleY = Math.Min(ScaleX, ScaleY);
                    break;
                case Stretch.UniformToFill:
                    ScaleX = ScaleY = Math.Max(ScaleX, ScaleY);
                    break;
            }
            //----
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = ScaleX;
            scaleTransform.ScaleY = ScaleY;
            return scaleTransform;

#endif

#if old
#endif

            //----
            double PathViewPortWidth = double.IsNaN(this.CarouselPath.Width) ? ViewPort.Width : this.CarouselPath.Width;
            double PathViewPortHeight = double.IsNaN(this.CarouselPath.Height) ? ViewPort.Height : this.CarouselPath.Height;
            //----
            double ScaleX = GeometryBounds.Width == 0 ? PathViewPortWidth : PathViewPortWidth / GeometryBounds.Width;
            double ScaleY = GeometryBounds.Height == 0 ? PathViewPortHeight : PathViewPortHeight / GeometryBounds.Height;

            //----
            switch (Stretch)
            {
                case Stretch.None:
                    ScaleX = ScaleY = 1;
                    break;
                case Stretch.Fill:
                    break;
                case Stretch.Uniform:
                    ScaleX = ScaleY = Math.Min(ScaleX, ScaleY);
                    break;
                case Stretch.UniformToFill:
                    ScaleX = ScaleY = Math.Max(ScaleX, ScaleY);
                    break;
            }
            //----
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = ScaleX;
            scaleTransform.ScaleY = ScaleY;
            return scaleTransform;
        }

        /// <summary>
        /// Changes the geometry bounds based on the ScaleTransform
        /// </summary>
        /// <param name="CurrentGeometryBounds">The current geometry bounds.</param>
        /// <param name="Transformation">The transformation.</param>
        /// <returns></returns>
        private static Rect ChangeGeometryBounds(Rect CurrentGeometryBounds, Matrix Transformation)
        {
            Rect TransformedBounds = Rect.Transform(CurrentGeometryBounds, Transformation);
            TransformedBounds.Width = TransformedBounds.Width == 0 ? 0.5 : TransformedBounds.Width;
            TransformedBounds.Height = TransformedBounds.Height == 0 ? 0.5 : TransformedBounds.Height;
            return TransformedBounds;
        }

        /// <summary>
        /// Translates the size of the path based on AvailableSize.
        /// </summary>
        /// <param name="ViewPort">The view port.</param>
        /// <param name="GeometryBounds">The geometry bounds.</param>
        /// <returns></returns>
        private TranslateTransform ChangeCustomPathToAvailableSize(Rect ViewPort, Rect GeometryBounds)
        {
#if Old
#endif

            double HorizontalOffset = ViewPort.Left;
            double VerticalOffset = ViewPort.Top;
            //Path Width and Height
            double PathWidth = double.IsNaN(this.CarouselPath.Width) ? GeometryBounds.Width : this.CarouselPath.Width;
            double PathHeight = double.IsNaN(this.CarouselPath.Height) ? GeometryBounds.Height : this.CarouselPath.Height;

            //Remaining Width and Height
            double RemainingWidth = Math.Max(0.0, ViewPort.Width - PathWidth);
            double RemainingHeight = Math.Max(0.0, ViewPort.Height - PathHeight);

            //Calculating Transformations
            HorizontalOffset += CalculateHorizontalTransformation(this.CarouselPath.HorizontalAlignment, RemainingWidth);
            VerticalOffset += CalculateVerticalTransformation(this.CarouselPath.VerticalAlignment, RemainingHeight);
            double TranX = HorizontalOffset - GeometryBounds.Left;
            double TranY = RemainingHeight - GeometryBounds.Top;

            return new TranslateTransform(TranX, TranY);

#if New

            double HorizontalOffset = ViewPort.Left;
            double VerticalOffset = ViewPort.Top;
            //Path Width and Height
            //double PathWidth = double.IsNaN(this.CarouselPath.Width) ? GeometryBounds.Width : this.CarouselPath.Width;
            //double PathHeight = double.IsNaN(this.CarouselPath.Height) ? GeometryBounds.Height : this.CarouselPath.Height;
            
            //Remaining Width and Height
            double RemainingWidth = Math.Max(0.0, ViewPort.Width - GeometryBounds.Width);
            double RemainingHeight = Math.Max(0.0, ViewPort.Height - GeometryBounds.Height);

            //Calculating Transformations
            HorizontalOffset += CalculateHorizontalTransformation(this.CarouselPath.HorizontalAlignment, RemainingWidth);
            VerticalOffset += CalculateVerticalTransformation(this.CarouselPath.VerticalAlignment, RemainingHeight);
            double TranX = HorizontalOffset - GeometryBounds.Left;
            double TranY = RemainingHeight - GeometryBounds.Top;
            
            return new TranslateTransform(TranX, TranY);
#endif

        }

        private static double CalculateHorizontalTransformation(HorizontalAlignment Alignment, double RemainingWidth)
        {
            switch (Alignment)
            {
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    return Math.Max(0.0, RemainingWidth / 2);
                case HorizontalAlignment.Right:
                    return Math.Max(0.0, RemainingWidth);
            }
            return 0.0;
        }

        private static double CalculateVerticalTransformation(VerticalAlignment Alignment, double RemainHeight)
        {
            switch (Alignment)
            {
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    return Math.Max(0.0, RemainHeight / 2);
                case VerticalAlignment.Bottom:
                    return Math.Max(0.0, RemainHeight);
            }
            return 0.0;
        }

        #region SupportingMethods

        public int GetPathFractionIndex(double pathFraction)
        {
            int i = 0;
            foreach (PathFractions item in this.PathFractions)
            {
                if (item.PathFraction >= pathFraction)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        public int GetVisiblePathFractionCount()
        {
            return (this.PathFractions.Length);
            //return (this.PathFractions.Length - 2);
        }
        #endregion

        #region HelperMethods

        public static bool IsVisible(double pathFraction)
        {
            bool visible = ((pathFraction == -1.0) || (pathFraction == 0.0)) || (pathFraction == 1.0);
            return !visible;
        }

        public int CompareCustomPathFractions(PathFractions x, PathFractions y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            return x.PathFraction.CompareTo(y.PathFraction);
        }
        #endregion

        public PathFractions GetVisiblePathFraction(int index)
        {
            if ((index >= 0) && (index != this.GetVisiblePathFractionCount()))
            {
                return this.PathFractions[index + 1];
            }
            return null;
        }
    }
   
    /// <summary>
    /// 
    /// </summary>
    public class PathFractionCollection : ObservableCollection<FractionValue>
    {
        /// <summary>
        /// Finds the nearest points.
        /// </summary>
        /// <param name="currentStopPointPathFraction">The current stop point path fraction.</param>
        /// <param name="LeftNearestStopPint">The left nearest stop pint.</param>
        /// <param name="RightNearestStopPint">The right nearest stop pint.</param>
        internal void FindNearestPoints(double currentStopPointPathFraction, out FractionValue LeftNearestStopPint, out FractionValue RightNearestStopPint)
        {
            double leftClosestDistance = -1.0;
            double rightClosestDistance = -1.0;

            FractionValue leftitem = new FractionValue();
            FractionValue rightitem = new FractionValue();

            if (this.Items.Count <= 0)
            {
                throw new NotImplementedException();
            }

            foreach (FractionValue stopPoint in this.Items)
            {
                if ((stopPoint.Fraction >= leftClosestDistance) && (stopPoint.Fraction <= currentStopPointPathFraction))
                {
                    leftClosestDistance = stopPoint.Fraction;
                    leftitem = stopPoint;
                }
                else
                {
                    rightClosestDistance = stopPoint.Fraction;
                    rightitem = stopPoint;
                    break;
                }
            }
            if (leftClosestDistance == -1.0)
            {
                LeftNearestStopPint = null;
            }
            else
            {
                LeftNearestStopPint = leftitem;               
            }
            if (rightClosestDistance == -1.0)
            {
                RightNearestStopPint = null;
            }
            else
            {
                RightNearestStopPint = rightitem;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FractionValue : DependencyObject
    {
        /// <summary>
        /// Gets or sets the fraction.
        /// </summary>
        /// <value>The fraction.</value>
        public double Fraction
        {
            get { return (double)GetValue(FractionProperty); }
            set { SetValue(FractionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FractionProperty =
            DependencyProperty.Register("Fraction", typeof(double), typeof(FractionValue), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(FractionValue), new PropertyMetadata(0.0));
    }
    /// <summary>
    /// 
    /// </summary>
    internal class PathFractionManager
    {
        private double newPathFraction;
        private double currentPathFraction;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFractionManager"/> class.
        /// </summary>
        /// <param name="newVal">The new val.</param>
        /// <param name="oldVal">The old val.</param>
        public PathFractionManager(double newVal, double oldVal)
        {
            if (newVal < 0.0)
            {
                throw new Exception();
            }
            this.newPathFraction = newVal;
            this.currentPathFraction = oldVal;
        }

        /// <summary>
        /// Gets the new path fraction.
        /// </summary>
        /// <value>The new path fraction.</value>
        public double NewPathFraction
        {
            get
            {
                return this.newPathFraction;
            }
        }

        /// <summary>
        /// Gets the current path fraction.
        /// </summary>
        /// <value>The current path fraction.</value>
        public double CurrentPathFraction
        {
            get
            {
                return this.currentPathFraction;
            }
        }
    }
    internal class PathFractionRangeHandler : IEnumerable<VisiblePanelItem>, IEnumerable
    {
        private LinkedList<VisiblePanelItem> _ChildIndexPair;
        private int firstVisibleItemIndex;
        private int lastVisibleItemIndex;
        private const int MinimumIndexValue = -1;
        private LinkedList<VisiblePanelItem> visibleItems;

        public PathFractionRangeHandler()
        {
            this.firstVisibleItemIndex = -1;
            this.lastVisibleItemIndex = -1;
            this.visibleItems = new LinkedList<VisiblePanelItem>();
            this.ToCleanUp = new LinkedList<VisiblePanelItem>();
        }

        internal PathFractionRangeHandler(LinkedList<VisiblePanelItem> visibleItems)
            : this()
        {
            this.visibleItems = visibleItems;
        }

        public void AddFirst(LinkedList<VisiblePanelItem> pairs)
        {
            if (pairs == null)
            {
                throw new ArgumentNullException("pairs");
            }
            if (pairs.Count > 0)
            {
                for (LinkedListNode<VisiblePanelItem> pair = pairs.Last; pair != null; pair = pair.Previous)
                {
                    this.AddFirst(pair.Value);
                }
            }
        }

        public void AddFirst(VisiblePanelItem pair)
        {
            if (pair == null)
            {
                throw new ArgumentNullException("pair");
            }
            this.visibleItems.AddFirst(pair);
        }

        public void AddLast(LinkedList<VisiblePanelItem> pairs)
        {
            if (pairs == null)
            {
                throw new ArgumentNullException("pairs");
            }
            if (pairs.Count > 0)
            {
                foreach (VisiblePanelItem pair in pairs)
                {
                    this.AddLast(pair);
                }
            }
        }

        public void AddLast(VisiblePanelItem pair)
        {
            if (pair == null)
            {
                throw new ArgumentNullException("pair");
            }
            this.visibleItems.AddLast(pair);
        }

        public void Clear()
        {
            this.visibleItems.Clear();
            this.ToCleanUp.Clear();
        }

        public void ClearCleanUp()
        {
            this.ToCleanUp.Clear();
        }

        public IEnumerator<VisiblePanelItem> GetEnumerator()
        {
            return this.visibleItems.GetEnumerator();
        }

        public int GetVisibleItemsCount()
        {
            if (this.HasVisibleItems)
            {
                return ((this.lastVisibleItemIndex - this.firstVisibleItemIndex) + 1);
            }
            return 0;
        }

        public bool IsInVisibleRange(int index)
        {
            if ((index < 0) || !this.HasVisibleItems)
            {
                return false;
            }
            return ((index >= this.firstVisibleItemIndex) && (index <= this.lastVisibleItemIndex));
        }

        public void Remove(VisiblePanelItem pair)
        {
            this.visibleItems.Remove(pair);
        }

        public void ScheduleClean(IList<VisiblePanelItem> pairs)
        {
            foreach (VisiblePanelItem pair in pairs)
            {
                if (this.visibleItems.Remove(pair))
                {
                    this.ToCleanUp.AddFirst(pair);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.visibleItems.GetEnumerator();
        }

        public void UpdateVisibleRange(VisibleRangeAction action, LinkedList<VisiblePanelItem> pairs)
        {
            LinkedList<VisiblePanelItem> newPairs = new LinkedList<VisiblePanelItem>();
            foreach (VisiblePanelItem pair in pairs)
            {
                if (!this.visibleItems.Contains(pair))
                {
                    newPairs.AddFirst(pair);
                }
            }
            if (action == VisibleRangeAction.AddFromEnd)
            {
                this.AddLast(newPairs);
            }
            if (action == VisibleRangeAction.AddFromStart)
            {
                this.AddFirst(newPairs);
            }
            if ((action == VisibleRangeAction.RemoveFromEnd) || (action == VisibleRangeAction.RemoveFromStart))
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                return this.visibleItems.Count;
            }
        }

        public VisiblePanelItem First
        {
            get
            {
                if (this.visibleItems.First != null)
                {
                    return this.visibleItems.First.Value;
                }
                return null;
            }
        }

        public int FirstVisibleItemIndex
        {
            get
            {
                if (this.First == null)
                {
                    return -1;
                }
                return this.First.Index;
            }
        }

        public bool HasVisibleItems
        {
            get
            {
                return (this.visibleItems.Count > 0);
            }
        }

        public VisiblePanelItem Last
        {
            get
            {
                if (this.visibleItems.Last != null)
                {
                    return this.visibleItems.Last.Value;
                }
                return null;
            }
        }

        public int LastVisibleItemIndex
        {
            get
            {
                if (this.Last == null)
                {
                    return -1;
                }
                return this.Last.Index;
            }
        }

        public LinkedList<VisiblePanelItem> ToCleanUp
        {
            get
            {
                return this._ChildIndexPair;
            }
            private set
            {
                this._ChildIndexPair = value;
            }
        }
    }

    internal class VisiblePanelItem
    {
        private UIElement _Child;
        private int _Index;

        public VisiblePanelItem(UIElement child, int index)
        {
            if (index < 0)
            {
                return;
            }
            this.Child = child;
            this.Index = index;
        }

        public override bool Equals(object obj)
        {
            VisiblePanelItem otherPair = obj as VisiblePanelItem;
            return ((otherPair != null) && (this.Index == otherPair.Index));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public UIElement Child
        {
            get
            {
                return this._Child;
            }
            set
            {
                this._Child = value;
            }
        }

        public int Index
        {
            get
            {
                return this._Index;
            }
            set
            {
                this._Index = value;
            }
        }
    }

    internal enum VisibleRangeAction
    {
        RemoveFromStart,
        RemoveFromEnd,
        AddFromStart,
        AddFromEnd
    }
    /// <summary>
    /// 
    /// </summary>
    public class PathFractions : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathFractions"/> class.
        /// </summary>
        public PathFractions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFractions"/> class.
        /// </summary>
        /// <param name="pathFraction">The path fraction.</param>
        public PathFractions(double pathFraction)
        {
            this.PathFraction = pathFraction;
        }

        /// <summary>
        /// Gets or sets the path fraction.
        /// </summary>
        /// <value>The path fraction.</value>
        public double PathFraction
        {
            get { return (double)GetValue(PathFractionProperty); }
            set { SetValue(PathFractionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PathFractionProperty =
            DependencyProperty.Register("PathFraction", typeof(double), typeof(PathFractions), new PropertyMetadata(0.0, null, new CoerceValueCallback(CoercePathFraction)));

        /// <summary>
        /// Coerces the path fraction.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoercePathFraction(DependencyObject d, object baseValue)
        {
            double value = (double)baseValue;
            if (value < 0.0)
                value = 0.0;

            if (value > 1.0)
                value = 1.0;
            return value;
        }


    }
    internal class VirtualizingItemsCollection
    {
        private int _ItemsCount;
        private int _ItemsPerPage;
        private int firstVisibleIndex;
        private int lastVisibleIndex;
        private int offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingItemsCollection"/> class.
        /// </summary>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="itemsCount">The items count.</param>
        public VirtualizingItemsCollection(int itemsPerPage, int itemsCount)
        {
            this.Offset = 0;
            this.ItemsPerPage = itemsPerPage;
            this.ItemsCount = itemsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingItemsCollection"/> class.
        /// </summary>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="itemsCount">The items count.</param>
        /// <param name="offset">The offset.</param>
        public VirtualizingItemsCollection(int itemsPerPage, int itemsCount, int offset)
        {
            this.ItemsPerPage = itemsPerPage;
            this.ItemsCount = itemsCount;
            this.Offset = offset;
        }

        /// <summary>
        /// Adjusts the offset after item added.
        /// </summary>
        /// <param name="newIndexPosition">The new index position.</param>
        /// <param name="count">The count.</param>
        public void ModifyOffsetAfterItemAdded(int newIndexPosition, int count)
        {
            if (newIndexPosition <= this.GetFirstVisibleIndex())
            {
                this.Offset += count;
            }
            this.ItemsCount += count;
            this.Offset = this.Offset;
        }

        /// <summary>
        /// Adjusts the offset after item removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public void ModifyOffsetAfterItemRemoved(int index, int count)
        {
            if (index <= this.GetFirstVisibleIndex())
            {
                this.Offset -= count;
            }
            this.ItemsCount -= count;
            this.Offset = this.Offset;
        }

        /// <summary>
        /// Gets the first index of the visible.
        /// </summary>
        /// <returns></returns>
        private int GetFirstVisibleIndex()
        {
            for (int i = this.ItemsPerPage; i > 0; i--)
            {
                int index = this.Offset - i;
                if ((index >= 0) && (index < this.ItemsCount))
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the last index of the visible.
        /// </summary>
        /// <returns></returns>
        private int GetLastVisibleIndex()
        {
            for (int i = 1; i <= this.ItemsPerPage; i++)
            {
                int index = this.Offset - i;
                if (index < this.ItemsCount)
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int GetPosition(int index)
        {
            if ((index >= 0) && CarouselPanelHelperMethods.IsInRange(index, this.FirstVisibleIndex, this.LastVisibleIndex))
            {
                return Math.Abs((int)(index - (this.Offset - 1)));
            }
            return -1;
        }

        /// <summary>
        /// Determines whether [is before visible range] [the specified index].
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// 	<c>true</c> if [is before visible range] [the specified index]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsBeforeVisibleRange(int index)
        {
            int firstIndex = this.Offset - this.ItemsPerPage;
            return (index < firstIndex);
        }

        /// <summary>
        /// Moves the specified displacement.
        /// </summary>
        /// <param name="displacement">The displacement.</param>
        public void Move(int displacement)
        {
            if (displacement > 0)
            {
                this.MoveRight(displacement);
            }
            else
            {
                this.MoveLeft(displacement);
            }
        }

        /// <summary>
        /// Moves the left.
        /// </summary>
        /// <param name="displacement">The displacement.</param>
        public void MoveLeft(int displacement)
        {
            int absDisplacement = Math.Abs(displacement);
            int newOffset = this.Offset - absDisplacement;
            this.Offset = CarouselPanelHelperMethods.CoerceRangeValues(newOffset, 0, this.ItemsPerPage + this.ItemsCount);
        }

        /// <summary>
        /// Moves the right.
        /// </summary>
        /// <param name="displacement">The displacement.</param>
        public void MoveRight(int displacement)
        {
            Math.Abs(displacement);
            int newOffset = this.Offset + displacement;
            this.Offset = CarouselPanelHelperMethods.CoerceRangeValues(newOffset, 0, this.ItemsPerPage + this.ItemsCount);
        }

        /// <summary>
        /// Tries the fill.
        /// </summary>
        public void TryFill()
        {
            if (!this.PageFull)
            {
                int countBefore = this.CountBefore;
                int countAfter = this.CountAfter;
                if (countAfter >= countBefore)
                {
                    this.Offset += Math.Min(Math.Max(this.FreePosition, countAfter), this.FreePosition);
                }
                else
                {
                    this.Offset -= Math.Min(Math.Max(this.FreePosition, countBefore), this.FreePosition);
                }
            }
        }

        /// <summary>
        /// Gets the count after.
        /// </summary>
        /// <value>The count after.</value>
        public int CountAfter
        {
            get
            {
                if (this.LastVisibleIndex >= 0)
                {
                    return ((this.ItemsCount - this.LastVisibleIndex) - 1);
                }
                if (this.Offset == 0)
                {
                    return this.ItemsCount;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the count before.
        /// </summary>
        /// <value>The count before.</value>
        public int CountBefore
        {
            get
            {
                if (this.FirstVisibleIndex > 0)
                {
                    return this.FirstVisibleIndex;
                }
                if (this.OffsetIsMaximum)
                {
                    return this.ItemsPerPage;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the first index of the visible.
        /// </summary>
        /// <value>The first index of the visible.</value>
        public int FirstVisibleIndex
        {
            get
            {
                return this.firstVisibleIndex;
            }
        }

        /// <summary>
        /// Gets the free position.
        /// </summary>
        /// <value>The free position.</value>
        public int FreePosition
        {
            get
            {
                return (this.ItemsPerPage - this.VisibleItemsCount);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has more items.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has more items; otherwise, <c>false</c>.
        /// </value>
        public bool HasMoreItems
        {
            get
            {
                return (this.VisibleItemsCount < this.ItemsCount);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has reached end.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has reached end; otherwise, <c>false</c>.
        /// </value>
        public bool HasReachedEnd
        {
            get
            {
                return (this.Offset >= (this.ItemsCount + this.ItemsPerPage));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has visible items.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has visible items; otherwise, <c>false</c>.
        /// </value>
        public bool HasVisibleItems
        {
            get
            {
                return ((this.LastVisibleIndex >= 0) && (this.FirstVisibleIndex >= 0));
            }
        }

        /// <summary>
        /// Gets or sets the items count.
        /// </summary>
        /// <value>The items count.</value>
        public int ItemsCount
        {
            get
            {
                return this._ItemsCount;
            }
            private set
            {
                this._ItemsCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the items per page.
        /// </summary>
        /// <value>The items per page.</value>
        public int ItemsPerPage
        {
            get
            {
                return this._ItemsPerPage;
            }
            private set
            {
                this._ItemsPerPage = value;
            }
        }

        /// <summary>
        /// Gets the last index of the visible.
        /// </summary>
        /// <value>The last index of the visible.</value>
        public int LastVisibleIndex
        {
            get
            {
                return this.lastVisibleIndex;
            }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public int Offset
        {
            get
            {
                return this.offset;
            }
            private set
            {
                this.offset = value;
                this.firstVisibleIndex = this.GetFirstVisibleIndex();
                this.lastVisibleIndex = this.GetLastVisibleIndex();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [offset is maximum].
        /// </summary>
        /// <value><c>true</c> if [offset is maximum]; otherwise, <c>false</c>.</value>
        public bool OffsetIsMaximum
        {
            get
            {
                return (this.Offset == (this.ItemsCount + this.ItemsPerPage));
            }
        }

        /// <summary>
        /// Gets a value indicating whether [page full].
        /// </summary>
        /// <value><c>true</c> if [page full]; otherwise, <c>false</c>.</value>
        public bool PageFull
        {
            get
            {
                return (this.VisibleItemsCount == this.ItemsPerPage);
            }
        }

        /// <summary>
        /// Gets the visible items count.
        /// </summary>
        /// <value>The visible items count.</value>
        public int VisibleItemsCount
        {
            get
            {
                if (this.HasVisibleItems)
                {
                    return ((this.LastVisibleIndex - this.FirstVisibleIndex) + 1);
                }
                return 0;
            }
        }


    }

    internal abstract class VirtualizingPanelHandler
    {
        private VisibleItemsHandler initialPathPositionStates;
        private bool isInitialized;

        /// <summary>
        /// Begins the animation.
        /// </summary>
        /// <param name="beginTime">The begin time.</param>
        public virtual void BeginItemMovement(TimeSpan beginTime)
        {
            if (!this.isInitialized)
            {
                return;
            }
            if (this.state != ItemMovementState.NotStarted)
            {
                return;
            }
            this.lastRender = beginTime;
            this.state = ItemMovementState.Started;
        }

        public VirtualizingPanelHandler()
        {
        }

        public void Initialize(CarouselPathHelper PathHelper, VisibleItemsHandler positionStates)
        {
            if (PathHelper == null)
            {
                throw new ArgumentNullException("path");
            }
            if (positionStates == null)
            {
                throw new ArgumentNullException("positionStates");
            }

            this.CarouselPathHelper = PathHelper;
            if (this.initialPathPositionStates == null)
            {
                this.initialPathPositionStates = positionStates;
            }
            this.Initialize();
            this.isInitialized = true;
        }

        protected virtual void Initialize()
        {
        }

        public abstract void CalculateItemsToAdd(out VisibleRangeAction action, out LinkedList<VisiblePanelItem> itemsToAdd);
        public abstract void AddItemToMove(VisiblePanelItem item);
        protected abstract void Animate(double percentageDone);
        public abstract void EndItemMovement();
        public abstract void Reverse();
        public abstract IList<VisiblePanelItem> GetItemsToRemoveEndofArrangeOverride();

        private static Point CalculateNewPosition(UIElement item, CarouselPathHelper carouselPathHelper)
        {
            Point newItemPosition;
            Point newItemTangent;
            double pathFraction = CustomPathCarouselPanel.GetPathFraction(item);
            try
            {
                carouselPathHelper.Geometry.GetPointAtFractionLength(pathFraction, out newItemPosition, out newItemTangent);
                return newItemPosition;
            }
            catch
            {
                newItemPosition = new Point();
                return newItemPosition;
            }
        }

        public static MatrixTransform RecalculateItemPosition(UIElement item, CarouselPathHelper carouselPathHelper)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (carouselPathHelper == null)
            {
                throw new ArgumentNullException("animationPath");
            }
            Matrix newMaxtrix = Matrix.Identity;

            Point newItemPosition = CalculateNewPosition(item, carouselPathHelper);
            newMaxtrix.Translate(newItemPosition.X, newItemPosition.Y);
            CustomPathCenterItem(item, ref newMaxtrix);
            return new MatrixTransform(newMaxtrix);
        }

        private static void CustomPathCenterItem(UIElement item, ref Matrix itemTransform)
        {
            Size itemSize = item.RenderSize;
            itemTransform.Translate(-(itemSize.Width / 2.0), -(itemSize.Height / 2.0));
        }

        /// <summary>
        /// Sets the animation data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startPathFraction">The start path fraction.</param>
        /// <param name="endPathFraction">The end path fraction.</param>
        protected static void SetPathFractionManagerForItem(UIElement item, double startPathFraction, double endPathFraction)
        {
            if ((startPathFraction == 0 && endPathFraction == 1) || ((startPathFraction == 1 && endPathFraction == 0)))
            {
                endPathFraction = startPathFraction;
                //Debug.WriteLine(startPathFraction.ToString(), endPathFraction.ToString());
            }
            PathFractionManager newAnimationData = new PathFractionManager(endPathFraction, startPathFraction);
            if(newAnimationData != null)
            CustomPathCarouselPanel.SetPathFractionManager(item, newAnimationData);
        }

        protected static void EndItemMovement(List<VisiblePanelItem> collection, CarouselPathHelper animationPath)
        {
            foreach (VisiblePanelItem currentElement in collection)
            {
                PathFractionManager data = CustomPathCarouselPanel.GetPathFractionManager(currentElement.Child);
                if(data != null)
                CustomPathCarouselPanel.SetPathFraction(currentElement.Child, data.NewPathFraction);
                RecalculateItemPosition(currentElement.Child, animationPath);
            }
        }

        public void Update(TimeSpan currentTime)
        {
            if (this.state == ItemMovementState.Started)
            {
                TimeSpan deltaTime = currentTime.Subtract(this.lastRender);
                this.totalRunningTime = this.totalRunningTime.Add(deltaTime);
                this.lastRender = currentTime;
                if (this.totalRunningTime < this.duration)
                {
                    this.Animate(this.GetPercentageDone());
                }
                else
                {
                    this.EndItemMovement();
                    this.state = ItemMovementState.Finished;
                }
            }
        }

        public double GetPercentageDone()
        {
            return (this.totalRunningTime.TotalSeconds / this.duration.TotalSeconds);
        }

        public TimeSpan GetTimeLeft()
        {
            if (this.duration <= this.totalRunningTime)
            {
                return TimeSpan.Zero;
            }
            return this.duration.Subtract(this.totalRunningTime);
        }

        protected static void ReverseAnimationdata(List<VisiblePanelItem> collection)
        {
            foreach (VisiblePanelItem currentElement in collection)
            {
                PathFractionManager data = CustomPathCarouselPanel.GetPathFractionManager(currentElement.Child);
                if (data != null)
                {
                    PathFractionManager newData = new PathFractionManager(data.CurrentPathFraction, data.NewPathFraction);
                    CustomPathCarouselPanel.SetPathFractionManager(currentElement.Child, newData);
                }
            }
        }

        protected static void UpdateCustomPathItemFraction(UIElement item, double animationPercentageDone)
        {
            PathFractionManager data = CustomPathCarouselPanel.GetPathFractionManager(item);
            if (data != null)
            {
                double distance = data.NewPathFraction - data.CurrentPathFraction;
                CustomPathCarouselPanel.SetPathFraction(item, data.CurrentPathFraction + (distance * animationPercentageDone));
            }
            
               
        
        }

        protected void UpdateItemTransformation(UIElement item)
        {
            Matrix matrix = RecalculateItemPosition(item, this.CarouselPathHelper).Matrix;
            matrix.Translate(0.0, 0.0);
            item.RenderTransform = new MatrixTransform(matrix);
        }

        private CarouselPathHelper carouselPathHelper;
        /// <summary>
        /// Gets or sets the carousel path helper.
        /// </summary>
        /// <value>The carousel path helper.</value>
        public CarouselPathHelper CarouselPathHelper
        {
            get
            {
                return this.carouselPathHelper;
            }
            protected set
            {
                this.carouselPathHelper = value;
            }
        }

        private TimeSpan duration = new TimeSpan(0, 0, 0, 0, 300);
        private TimeSpan lastRender = new TimeSpan(0, 0, 0);
        private TimeSpan totalRunningTime = new TimeSpan(0, 0, 0);
        private ItemMovementState state = ItemMovementState.NotStarted;

        public TimeSpan Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
                if ((this.duration <= this.totalRunningTime) && (this.state == ItemMovementState.Started))
                {
                    this.EndItemMovement();
                    this.state = ItemMovementState.Finished;
                }
            }
        }

        public VisibleItemsHandler InitialPathPositionStates
        {
            get
            {
                return this.initialPathPositionStates;
            }
            set
            {
                this.initialPathPositionStates = value;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return this.isInitialized;
            }
        }

        public TimeSpan LastRender
        {
            get
            {
                return this.lastRender;
            }
            protected set
            {
                this.lastRender = value;
            }
        }

        public ItemMovementState State
        {
            get
            {
                return this.state;
            }
            protected set
            {
                this.state = value;
            }
        }

        public TimeSpan TotalRunningTime
        {
            get
            {
                return this.totalRunningTime;
            }
            protected set
            {
                this.totalRunningTime = value;
            }
        }
    }

    internal enum ItemMovementState
    {
        NotStarted,
        Started,
        Finished
    }

    internal class VirtualizingPanelItemMoveHandler : VirtualizingPanelHandler
    {
        public VirtualizingPanelItemMoveHandler()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private VirtualizingItemsCollection _VirtualizingItemsCollection;
        private List<VisiblePanelItem> newItemsToExit;
        private List<VisiblePanelItem> newItemsToStay;
        private List<VisiblePanelItem> oldItemsToExit;
        private List<VisiblePanelItem> oldItemsToStay;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingPanelItemMoveHandler"/> class.
        /// </summary>
        /// <param name="displacement">The displacement.</param>
        /// <param name="infoProvider">The info provider.</param>
        public VirtualizingPanelItemMoveHandler(int displacement, CarouselPanelHelper infoProvider)
        {
            this.pathDisplacement = displacement;
            this.oldItemsToExit = new List<VisiblePanelItem>();
            this.newItemsToExit = new List<VisiblePanelItem>();
            this.oldItemsToStay = new List<VisiblePanelItem>();
            this.newItemsToStay = new List<VisiblePanelItem>();
            this.Collection = new VirtualizingItemsCollection(infoProvider.PageSize, infoProvider.ItemsCount, infoProvider.Position);
        }

        public override void AddItemToMove(VisiblePanelItem item)
        {
            if (item != null)
            {
                if (CustomPathCarouselPanel.GetPathFraction(item.Child) < 0.0)
                {
                    this.AddNewItem(item);
                }
                else
                {
                    this.AddExistingItem(item);
                }
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.Collection.Move(this.pathDisplacement);
        }

        #region AddExistingItem

        private void AddExistingItem(VisiblePanelItem pair)
        {
            UIElement item = pair.Child;
            double currentPathFraction = CustomPathCarouselPanel.GetPathFraction(item);
            double targetFraction = this.CalculateNextPathFraction(pair.Index);
            VirtualizingPanelHandler.SetPathFractionManagerForItem(item, currentPathFraction, targetFraction);
            CustomPathCarouselPanel.SetPathFraction(item, targetFraction);
           
            switch (targetFraction.ToString())
            {
                case "0":
                case "1":
                    this.oldItemsToExit.Add(pair);
                    return;
            }
            if ((pair.Child) is CarouselItem)
            {
                if ((pair.Child as CarouselItem).DataContext != null)
                {
                    if ((pair.Child as CarouselItem).DataContext.ToString() == "{DisconnectedItem}")
                    {
                        this.oldItemsToExit.Add(pair);
                    }
                    else
                        this.oldItemsToStay.Add(pair);
                }
            }
            else
            this.oldItemsToStay.Add(pair);
        }

        public double CalculateNextPathFraction(int index)
        {
            int viewrangePosition = this.Collection.GetPosition(index);
            if (viewrangePosition == -1)
            {
                return this.GetExitPathFraction();
            }
            return base.CarouselPathHelper.GetVisiblePathFraction(viewrangePosition).PathFraction;
        }

        public double GetExitPathFraction()
        {
            if (this.PathDisplacement >= 0)
            {
                return 1.0;
            }
            return 0.0;
        }
        #endregion

        #region AddNewItem

        private void AddNewItem(VisiblePanelItem pair)
        {
            UIElement item = pair.Child;
            this.SetStartingPathFraction(item);
            this.CalculateNewItemAnimation(pair);
            if ((pair.Child) is CarouselItem)
            {
                if ((pair.Child as CarouselItem).DataContext != null)
                {
                    if ((pair.Child as CarouselItem).DataContext.ToString() == "{DisconnectedItem}")
                    {
                        this.newItemsToExit.Add(pair);
                    }
                    else
                        this.newItemsToStay.Add(pair);
                }
            }
            else
                this.newItemsToStay.Add(pair);
        }

        private void SetStartingPathFraction(UIElement item)
        {
            double startingPathFraction = -1.0;
            if (this.pathDisplacement <= 0)
            {
                startingPathFraction = 1.0;
            }
            else if (this.pathDisplacement > 0)
            {
                startingPathFraction = 0.0;
            }
            CustomPathCarouselPanel.SetPathFraction(item, startingPathFraction);
        }

        private void CalculateNewItemAnimation(VisiblePanelItem pair)
        {
            double currentPathFraction = CustomPathCarouselPanel.GetPathFraction(pair.Child);
            double nextFraction = this.CalculateNextPathFraction(pair.Index);
            VirtualizingPanelHandler.SetPathFractionManagerForItem(pair.Child, currentPathFraction, nextFraction);
            CustomPathCarouselPanel.SetPathFraction(pair.Child, nextFraction);
        }
        #endregion

        /// <summary>
        /// Calculates the items to add.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="itemsToAdd">The items to add.</param>
        public override void CalculateItemsToAdd(out VisibleRangeAction action, out LinkedList<VisiblePanelItem> itemsToAdd)
        {
            itemsToAdd = new LinkedList<VisiblePanelItem>();
            if (this.PathDisplacement >= 0)
            {
                action = VisibleRangeAction.AddFromEnd;
                for (int i = 0; i < this.Collection.VisibleItemsCount; i++)
                {
                    VisiblePanelItem pair = new VisiblePanelItem(null, this.Collection.FirstVisibleIndex + i);
                    itemsToAdd.AddFirst(pair);
                }
            }
            else
            {
                action = VisibleRangeAction.AddFromStart;
                for (int i = 0; i < this.Collection.VisibleItemsCount; i++)
                {
                    VisiblePanelItem pair = new VisiblePanelItem(null, this.Collection.FirstVisibleIndex + i);
                    itemsToAdd.AddFirst(pair);
                }
            }
        }

        public override IList<VisiblePanelItem> GetItemsToRemoveEndofArrangeOverride()
        {
            return this.oldItemsToExit;
        }

        protected override void Animate(double percentageDone)
        {
            this.Animate(this.oldItemsToExit, percentageDone);
            this.Animate(this.oldItemsToStay, percentageDone);
            this.Animate(this.newItemsToExit, percentageDone);
            this.Animate(this.newItemsToStay, percentageDone);
        }

        private void Animate(List<VisiblePanelItem> collection, double percentageDone)
        {
            foreach (VisiblePanelItem currentElement in collection)
            {
                VirtualizingPanelHandler.UpdateCustomPathItemFraction(currentElement.Child, percentageDone);
                base.UpdateItemTransformation(currentElement.Child);
            }
        }

        public override void EndItemMovement()
        {
            VirtualizingPanelHandler.EndItemMovement(this.oldItemsToExit, base.CarouselPathHelper);
            VirtualizingPanelHandler.EndItemMovement(this.oldItemsToStay, base.CarouselPathHelper);
            VirtualizingPanelHandler.EndItemMovement(this.newItemsToExit, base.CarouselPathHelper);
            VirtualizingPanelHandler.EndItemMovement(this.newItemsToStay, base.CarouselPathHelper);
        }

        public bool IsOpposite(int displacement)
        {
            return (this.PathDisplacement == -displacement);
        }

        public override void Reverse()
        {
            this.pathDisplacement = -this.pathDisplacement;
            base.TotalRunningTime = base.Duration.Subtract(base.TotalRunningTime);
            VirtualizingPanelHandler.ReverseAnimationdata(this.oldItemsToExit);
            VirtualizingPanelHandler.ReverseAnimationdata(this.oldItemsToStay);
            VirtualizingPanelHandler.ReverseAnimationdata(this.newItemsToExit);
            VirtualizingPanelHandler.ReverseAnimationdata(this.newItemsToStay);
            this.ReverseOldAndNewItems();
        }

        private void ReverseOldAndNewItems()
        {
            List<VisiblePanelItem> refHolder = this.oldItemsToExit;
            this.oldItemsToExit = new List<VisiblePanelItem>(this.newItemsToExit);
            this.oldItemsToExit.AddRange(this.newItemsToStay);
            this.newItemsToExit.Clear();
            this.newItemsToStay = new List<VisiblePanelItem>(refHolder);
        }

        public VirtualizingItemsCollection Collection
        {
            get
            {
                return this._VirtualizingItemsCollection;
            }
            set
            {
                this._VirtualizingItemsCollection = value;
            }
        }

        private int pathDisplacement;
        public int PathDisplacement
        {
            get
            {
                return this.pathDisplacement;
            }
        }
    }

    internal class VisibleItemsHandler
    {
        private const int AvailablePositionDigit = -1;
        private VisiblePanelItem[] positions;

        public VisibleItemsHandler(int visiblePositions)
        {
            this.positions = new VisiblePanelItem[visiblePositions];
        }

        public int GetFreePositionsLeft()
        {
            int freePositions = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] != null)
                {
                    return freePositions;
                }
                freePositions++;
            }
            return freePositions;
        }

        public int GetFreePositionsRight()
        {
            int freePositions = 0;
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (this[i] != null)
                {
                    return freePositions;
                }
                freePositions++;
            }
            return freePositions;
        }

        public VisiblePanelItem GetItemAtPosition(int positionIndex)
        {
            return this.positions[positionIndex];
        }

        public int GetLargestItemIndex()
        {
            int index = -1;
            for (int i = 0; i < this.Count; i++)
            {
                if ((this[i] != null) && (this[i].Index > index))
                {
                    index = this[i].Index;
                }
            }
            return index;
        }

        public int GetUsedPositions()
        {
            int usedPositions = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] != null)
                {
                    usedPositions++;
                }
            }
            return usedPositions;
        }

        public void SetItemAtPosition(int positionIndex, VisiblePanelItem item)
        {
            this.positions.SetValue(item, positionIndex);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(this.Count);
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == null)
                {
                    //builder.Append(-1.ToString(CultureInfo.InvariantCulture));
                    builder.Append(AvailablePositionDigit.ToString(CultureInfo.InstalledUICulture));
                }
                else
                {
                    builder.Append(this[i].Index);
                }
            }
            return builder.ToString();
        }

        public int Count
        {
            get
            {
                return this.positions.Length;
            }
        }

        public VisiblePanelItem this[int positionIndex]
        {
            get
            {
                return this.positions[positionIndex];
            }
            set
            {
                this.positions.SetValue(value, positionIndex);
            }
        }
    }
}
