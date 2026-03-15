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
using Pipeline;

namespace ScreenManager.Adorners
{
    class PolyBezierAdorner : BasicPointAdorner
    {
        public PolyBezierAdorner(UIElement parent, PolyBezier.PolyBezier element, bool canMove, IGridViewInfoService gridViewInfoService)
            : base(parent, element, canMove, gridViewInfoService)
        {
        }

        protected override PointCollection Points 
        {
            get
            {
                return (Element as PolyBezier.PolyBezier).Points;
            }

            set
            {
                try
                {
                    (Element as PolyBezier.PolyBezier).Points = value;
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
