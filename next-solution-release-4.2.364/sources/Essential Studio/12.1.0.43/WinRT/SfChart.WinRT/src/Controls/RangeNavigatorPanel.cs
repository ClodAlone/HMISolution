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
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class RangeNavigatorPanel : Panel
    {
        private RangeNavigatorRowDefinitions rowDefinitions;

        [ClassReference(IsReviewed = false)]
        public RangeNavigatorRowDefinitions RowDefinitions
        {
            get
            {
                if (rowDefinitions == null)
                    rowDefinitions = new RangeNavigatorRowDefinitions();

                return rowDefinitions;
            }
            set
            {
                rowDefinitions = value;
            }
        }



        [ClassReference(IsReviewed = false)]
        public static int GetRow(UIElement obj)
        {
            return (int)obj.GetValue(RowProperty);
        }

        [ClassReference(IsReviewed = false)]
        public static void SetRow(UIElement obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }

        // Using a DependencyProperty as the backing store for Row.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached("Row", typeof(int), typeof(RangeNavigatorPanel), new PropertyMetadata(0));

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in this.Children)
            {
                int rowIndex = GetRow(element);
                this.RowDefinitions[rowIndex].Element.Add(element);
            }

            
            return base.MeasureOverride(availableSize);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            double rowTop = 0;
            double usedHeight = 0;
            double rowStarCount = this.RowDefinitions.Sum((rowDef)
                =>
            {
                if (rowDef.Unit == ChartUnitType.Star)
                    return rowDef.Height;
                return 0;
            });

            double rowFixedHeight = this.RowDefinitions.Sum((rowDef)
                =>
            {
                if (rowDef.Unit == ChartUnitType.Pixels)
                    return rowDef.Height;
                return 0;
            });

            double remainingHeight = Math.Max(0, finalSize.Height - rowFixedHeight);
            double singleStarHeight = remainingHeight / rowStarCount;

            for (int i =0;i< this.RowDefinitions.Count;  i++)
            {
                RangeNavigatorRowDefinition row = this.RowDefinitions[i];
                double remainingSize = finalSize.Height - usedHeight;
                double height = 0;
                if (row.Unit == ChartUnitType.Star)
                {
                    height = Math.Min(remainingSize, row.Height * singleStarHeight);
                }
                else
                {
                    height = Math.Min(remainingSize, row.Height);
                }
                row.Height = double.IsNaN(height) ? 1d : height;
                RowDefinitions[i].Arrange(finalSize, rowTop);
                usedHeight += double.IsNaN(height) ? 1d : height;
                row.RowTop = rowTop;
                rowTop += double.IsNaN(height) ? 1d : height;
            }
            return base.ArrangeOverride(finalSize);
        }       

    }

    [ClassReference(IsReviewed = false)]
    public class RangeNavigatorRowDefinitions : ObservableCollection<RangeNavigatorRowDefinition>
    {
        public RangeNavigatorRowDefinitions()
        {

        }
    }

    public class RangeNavigatorRowDefinition : DependencyObject
    {
        private List<UIElement> element;

        public RangeNavigatorRowDefinition()
        {
            element = new List<UIElement>();
        }

        public double RowTop
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or Sets height of this row.
        /// </summary>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(RangeNavigatorRowDefinition), new PropertyMetadata(1d));

        /// <summary>
        /// Gets or Sets unit of the value specified in Height.
        /// </summary>
        public ChartUnitType Unit
        {
            get { return (ChartUnitType)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(ChartUnitType), typeof(RangeNavigatorRowDefinition), new PropertyMetadata(ChartUnitType.Star));

        /// <summary>
        /// Gets or Sets thickness of the border.
        /// </summary>
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SplitterLineThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(double), typeof(RangeNavigatorRowDefinition), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or Sets border stroke.
        /// </summary>
        public Brush BorderStroke
        {
            get { return (Brush)GetValue(BorderStrokeProperty); }
            set { SetValue(BorderStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SplitterStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderStrokeProperty =
            DependencyProperty.Register("BorderStroke", typeof(Brush), typeof(RangeNavigatorRowDefinition), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        internal Line BorderLine = new Line();

        internal List<UIElement> Element
        {
            get
            {
                return element;
            }
            set
            {
                element = value;
            }
        }

        Rect[] ElementBounds;

        internal void Measure(Size size,int rowIndex,double rowHeight)
        {
            int count=0;
            ElementBounds = new Rect[Element.Count];
            foreach (UIElement content in element)
            {

                if (Unit == ChartUnitType.Pixels)
                {
                    ElementBounds[count].Height = Height;
                }
                //else
                //{
                //    if(!(content is ResizableScrollBar))
                //    Height = rowHeight;
                //}
                count++;
            }
            
        }

        internal void Arrange(Size AvailableSize,double top)
        {

            foreach (UIElement content in Element)
            {
                double newheight = Height < 0 ? 1 : Height;
                content.Measure(new Size(AvailableSize.Width, newheight));
                content.Arrange(new Rect(0, top, AvailableSize.Width, newheight));
            }
        }

        private void RenderBorderLine(double top)
        {
            //if (Element != null && this.Element.Count > 0)
            //{
            //    UIElement borderelement = this.Element.FirstOrDefault();
            //    BorderLine.Stroke = this.BorderStroke;
            //    BorderLine.StrokeThickness = this.BorderThickness;
            //    BorderLine.X1 = 0;
            //    //BorderLine.X2 = borderelement.Area.SeriesClipRect.Width;
            //    //BorderLine.Y1 = BorderLine.Y2 = top - borderelement.Area.SeriesClipRect.Top;
            //}
        }
    }
}
