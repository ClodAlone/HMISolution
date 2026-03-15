using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace ScreenManager.Adorners
{
    class GridAdorner : Adorner
    {
        internal double MagicSnapNumber;
        Brush gridBrush;

        public GridAdorner(UIElement uie, double magicSnapNumber) : base(uie)
        {
            MagicSnapNumber = magicSnapNumber;
            IsHitTestVisible = false;
        }

        internal void RefreshGridBrush()
        {
            gridBrush = CreateGridBrush();
            InvalidateVisual();
        }

        Brush CreateGridBrush()
        {
            Color c = Colors.Black;

            GeometryDrawing aDrawing = new GeometryDrawing();

            Rect r = new Rect(0, 0, MagicSnapNumber, MagicSnapNumber);
            RectangleGeometry rect1 = new RectangleGeometry(r);

            // Add the geometry to the drawing.
            aDrawing.Geometry = rect1;

            // Specify the drawing's fill.

            aDrawing.Brush = Brushes.Transparent;
            //((AdornedElement as Canvas).Background as SolidColorBrush);

            // Specify the drawing's stroke.
            Pen stroke = new Pen();
            stroke.Thickness = 0.5;
            stroke.DashStyle = DashStyles.Dot;
            stroke.Brush = new SolidColorBrush(c);
            aDrawing.Pen = stroke;

            // Create a DrawingBrush
            DrawingBrush myDrawingBrush = new DrawingBrush 
            { 
                Drawing = aDrawing, 
                Stretch = Stretch.None, 
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, MagicSnapNumber, MagicSnapNumber), 
                ViewportUnits = BrushMappingMode.Absolute, 
                Opacity = 0.5 
            };

            return myDrawingBrush;
        }
        /// <summary>
        /// OnRender overriding 
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc)
        {
            if (gridBrush == null)
                gridBrush = CreateGridBrush();
            dc.DrawRectangle(gridBrush, null, new Rect(AdornedElement.RenderSize));
        }
    }
}
