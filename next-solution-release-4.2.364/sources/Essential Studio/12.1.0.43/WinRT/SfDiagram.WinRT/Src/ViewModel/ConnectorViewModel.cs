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
using System.Windows;

#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes; 
#else
using System.Windows.Media;
using System.Windows.Shapes; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public partial class ConnectorViewModel :
        GroupableViewModel,
        IConnector
    {
        static ConnectorViewModel()
        {
            SetDefultConnectorGeometryStyle();
            SetDefaultTargetDecoratorStyle();
        }

        private static Style _mStaticGeometryStyle;
        private static Style _mStaticTargetDecoratorStyle;

        //private Style _dConnectorGeometryStyle;
        //private Geometry _dSourceDecorator;
        //private Geometry _dTargetDecorator;
        //private Style _dSourceDecoratorStyle;
        //private Style _dTargetDecoratorStyle;

        private static void SetDefultConnectorGeometryStyle()
        {
            _mStaticGeometryStyle = new Style()
                {
                    TargetType = typeof(Shape),
                };
            _mStaticGeometryStyle.Setters.Add(
                new Setter()
                    {
                        Property = Shape.StrokeProperty,
                        Value = new SolidColorBrush(new Color() {A = 0xff, R = 0x77, G = 0x7e, B = 0x84}) // "#FF777E84"
                    });
            _mStaticGeometryStyle.Setters.Add(
                new Setter()
                {
                    Property = Shape.StrokeThicknessProperty,
                    Value = 1d
                });
        }

        private void SetDefauletTargetDecorator()
        {
            //_mTargetDecorator = new PathGeometry()
            //    {
            //        Figures = new PathFigureCollection()
            //            {
            //                new PathFigure()
            //                    {
            //                        StartPoint = new Point(0, 0),
            //                        Segments = new PathSegmentCollection()
            //                            {
            //                                new PolyLineSegment()
            //                                    {
            //                                        Points = new PointCollection()
            //                                            {
            //                                                new Point(10, 5),
            //                                                new Point(0, 10),
            //                                                new Point(0,0)
            //                                            }
            //                                    }
            //                            }
            //                    }
            //            }
            //    };
        }

        private static void SetDefaultTargetDecoratorStyle()
        {
            _mStaticTargetDecoratorStyle = new Style()
            {
                TargetType = typeof(Shape),
            };
            _mStaticTargetDecoratorStyle.Setters.Add(
                new Setter()
                {
                    Property = Shape.StrokeProperty,
                    Value = new SolidColorBrush(new Color() {A = 0xff, R = 0x77, G = 0x7e, B = 0x84}) // "#FF777E84"
                });
            _mStaticTargetDecoratorStyle.Setters.Add(
                new Setter()
                {
                    Property = Shape.FillProperty,
                    Value = new SolidColorBrush(new Color() { A = 0xff, R = 0x77, G = 0x7e, B = 0x84 }) //"#FF777E84"
                });
            _mStaticTargetDecoratorStyle.Setters.Add(
                new Setter()
                {
                    Property = Shape.StrokeThicknessProperty,
                    Value = 1d
                });
            _mStaticTargetDecoratorStyle.Setters.Add(
                new Setter()
                {
                    Property = Shape.StretchProperty,
                    Value = Stretch.Fill
                });
            _mStaticTargetDecoratorStyle.Setters.Add(
                new Setter()
                {
                    Property = Shape.WidthProperty,
                    Value = 10d
                });
            _mStaticTargetDecoratorStyle.Setters.Add(
                new Setter()
                {
                    Property = Shape.HeightProperty,
                    Value = 10d
                });
        }

        private void SetDefaultSourceDecoratorStyle()
        {
            SetDefaultTargetDecoratorStyle();
            _mSourceDecoratorStyle = _mTargetDecoratorStyle;
        }

        public ConnectorViewModel()
        {
            _mConnectorGeometryStyle = _mStaticGeometryStyle;
            _mTargetDecoratorStyle = _mStaticTargetDecoratorStyle;
            //SetDefultConnectorGeometryStyle();
            //SetDefauletTargetDecorator();
            //SetDefaultTargetDecoratorStyle();
        }
    }
}
