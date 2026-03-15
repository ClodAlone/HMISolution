using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Globalization;

namespace ScreenManager.Adorners
{
    class PolygonAdorner : BasicPointAdorner
    {
        public PolygonAdorner(UIElement parent, Polygon element, bool canMove, IGridViewInfoService gridViewInfoService)
            : base(parent, element, canMove, gridViewInfoService)
        {
        }

        protected override PointCollection Points 
        {
            get
            {
                return (Element as Polygon).Points;
            }

            set
            {
                try
                {
                    (Element as Polygon).Points = value;
                }
                catch { }
            }
        }

        protected override bool CanAddPoints 
        {
            get
            {
                return true;
            }
        }

    }
}
