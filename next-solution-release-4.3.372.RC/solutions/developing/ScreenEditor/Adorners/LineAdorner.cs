using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Globalization;
using System.ComponentModel;
using WPFUtilities;

namespace ScreenManager.Adorners
{
    class LineAdorner : BasicPointAdorner
    {
        public LineAdorner(UIElement parent, Line element, bool canMove, IGridViewInfoService gridViewInfoService)
            : base(parent, element, canMove, gridViewInfoService)
        {
        }

        PropertyChangeNotifier X1, X2, Y1, Y2;
        public override void Activate()
        {
            base.Activate();

            var line = Element as Line;
            var propDesc = DependencyPropertyDescriptor.FromProperty(Line.X1Property, typeof(Line));
            if (X1 == null)
            {
                X1 = new PropertyChangeNotifier(line, propDesc.Name);
                X1.ValueChanged += (o, e) =>
                {
                    if (currentThumb == null)
                        points[0] = new Point(line.X1, line.Y1);
                };
            }
            propDesc = DependencyPropertyDescriptor.FromProperty(Line.X2Property, typeof(Line));
            if (X2 == null)
            {
                X2 = new PropertyChangeNotifier(line, propDesc.Name);
                X2.ValueChanged += (o, e) =>
                {
                    if (currentThumb == null)
                        points[1] = new Point(line.X2, line.Y2);
                };
            }
            propDesc = DependencyPropertyDescriptor.FromProperty(Line.Y1Property, typeof(Line));
            if (Y1 == null)
            {
                Y1 = new PropertyChangeNotifier(line, propDesc.Name);
                Y1.ValueChanged += (o, e) =>
                {
                    if (currentThumb == null)
                        points[0] = new Point(line.X1, line.Y1);
                };
            }
            propDesc = DependencyPropertyDescriptor.FromProperty(Line.Y2Property, typeof(Line));
            if (Y2 == null)
            {
                Y2 = new PropertyChangeNotifier(line, propDesc.Name);
                Y2.ValueChanged += (o, e) =>
                {
                    if (currentThumb == null)
                        points[1] = new Point(line.X2, line.Y2);
                };
            }
        }

        public override void Deactivate()
        {
            if (X1 != null)
            {
                X1.Dispose();
                X1 = null;
            }
            if (X2 != null)
            {
                X2.Dispose();
                X2 = null;
            }
            if (Y1 != null)
            {
                Y1.Dispose();
                Y1 = null;
            }
            if (Y2 != null)
            {
                Y2.Dispose();
                Y2 = null;
            }

            base.Deactivate();
        }

        PointCollection points;
        protected override PointCollection Points 
        {
            get
            {
                if (points == null)
                {
                    points = new PointCollection();
                    Line line = Element as Line;

                    points.Add(new Point(line.X1, line.Y1));
                    points.Add(new Point(line.X2, line.Y2));
                }

                return points;
            }

            set
            {
                Line line = Element as Line;

                try
                {
                    line.X1 = value[0].X;
                    line.Y1 = value[0].Y;
                    line.X2 = value[1].X;
                    line.Y2 = value[1].Y;
                }
                catch { }
            }
        }

        protected override void UpdatePoints()
        {
            Points = points;
        }

        protected override bool CanAddPoints 
        {
            get
            {
                return false;
            }
        }

    }
}
