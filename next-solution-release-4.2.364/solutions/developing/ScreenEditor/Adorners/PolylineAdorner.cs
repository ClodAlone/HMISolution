using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Globalization;
using ScreenManager.SpecialObjects;

namespace ScreenManager.Adorners
{
    class PolylineAdorner : BasicPointAdorner
    {
        public PolylineAdorner(UIElement parent, Polyline element, bool canMove, IGridViewInfoService gridViewInfoService)
            : base(parent, element, canMove, gridViewInfoService)
        {
        }

        protected override PointCollection Points 
        {
            get
            {
                return (Element as Polyline).Points;
            }

            set
            {
                try
                {
                    (Element as Polyline).Points = value;
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
