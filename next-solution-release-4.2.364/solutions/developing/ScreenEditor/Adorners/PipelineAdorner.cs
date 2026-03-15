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
    class PipelineAdorner : BasicPointAdorner
    {
        public PipelineAdorner(UIElement parent, Pipeline.Pipeline element, bool canMove, IGridViewInfoService gridViewInfoService)
            : base(parent, element, canMove, gridViewInfoService)
        {
        }

        protected override PointCollection Points 
        {
            get
            {
                return (Element as Pipeline.Pipeline).Points;
            }

            set
            {
                (Element as Pipeline.Pipeline).Points = value;
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
