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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Diagram
{
    public class VerticalRuler : Ruler
    {
        public VerticalRuler()
            : base(string.Empty)
        {
            this.DefaultStyleKey = typeof(VerticalRuler);
            this.Orientation = Orientation.Vertical;
            this.Loaded += new RoutedEventHandler(VerticalRuler_Loaded);
            this.MarkerUp = false;
        }

        public VerticalRuler(string name)
            : base(name)
        {
            this.DefaultStyleKey = typeof(VerticalRuler);
            this.Orientation = Orientation.Vertical;
            this.Loaded += new RoutedEventHandler(VerticalRuler_Loaded);
            this.MarkerUp = false;
        }

        void VerticalRuler_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= VerticalRuler_Loaded;
            if (sv != null)
            {
                this.sv.MouseMove += new MouseEventHandler(sv_MouseMove);
            }
        }

        void sv_MouseMove(object sender, MouseEventArgs e)
        {
            this.MarkerPosition = this.dv.ScrollGrid.ScrollOwner.VerticalOffset+e.GetPosition(this.dv.ScrollGrid).Y;
        }
    }

    internal class LayoutTransforUtil : ContentPresenter
    {
        internal double angle = 90;
        public LayoutTransforUtil()
        {
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (angle == 90)
            {
                double width = 0;
                double height = 0;
                UIElement child = Content as UIElement;
                child.Measure(new Size(Double.PositiveInfinity, availableSize.Width));
                height += child.DesiredSize.Width;
                width = Math.Max(width, child.DesiredSize.Height);
                return new Size(width, height);
            }
            else
            {
                return base.MeasureOverride(availableSize);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (angle == 90)
            {
                UIElement child = Content as UIElement;
                Rect arrangeRect = new Rect();
                arrangeRect.X = 0;
                arrangeRect.Y = 0;
                arrangeRect.Width = child.DesiredSize.Width;
                arrangeRect.Height = finalSize.Width;
                child.Arrange(arrangeRect);

                RotateTransform rotateTransform = new RotateTransform
                {
                    Angle = angle,
                };

                TranslateTransform translateTransform = new TranslateTransform();
                //{
                translateTransform.X = finalSize.Width;
                if ((this.Content as Ruler).Orientation == Orientation.Vertical)
                {
                    //translateTransform.X -= (this.Content as Ruler).Margin.Left;
                    //translateTransform.Y += (this.Content as Ruler).Margin.Left;
                    translateTransform.Y += (this.Content as Ruler).OffsetY;
                }
                //};

                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(rotateTransform);
                transformGroup.Children.Add(translateTransform);

                child.RenderTransform = transformGroup;
                return finalSize;
            }
            else
            {
                return base.ArrangeOverride(finalSize);
            }
        }
    }

   
}
