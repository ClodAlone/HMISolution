#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks; 
#else
using System.Windows.Media; 
#endif
using Syncfusion.UI.Xaml.Diagram.Panels;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public partial class NodePort :
        PortBase,
        INodePort
    {
        readonly TranslateTransform _mTranslate = new TranslateTransform();
        internal IInternalNodePort Wrapper;

        public NodePort()
        {
            this.RenderTransform = _mTranslate;
            ID = Guid.NewGuid();
            //this.Shape = new PathGeometry
            //    {
            //        Figures = new PathFigureCollection
            //            {
            //                new PathFigure
            //                    {
            //                        StartPoint = new Point(0, 0),
            //                        Segments = new PathSegmentCollection
            //                            {
            //                                new LineSegment
            //                                    {
            //                                        Point = new Point(10, 10)
            //                                    }
            //                            }
            //                    },
            
            //                new PathFigure
            //                    {
            //                        StartPoint = new Point(10, 0),
            //                        Segments = new PathSegmentCollection
            //                            {
            //                                new LineSegment
            //                                    {
            //                                        Point = new Point(0, 10)
            //                                    }
            //                            }
            //                    }
            //            }
            //    };
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdatePosition();
            return base.ArrangeOverride(finalSize);
        }

        private void UpdatePosition()
        {
            if (Wrapper != null && Wrapper.KnownNode != null)
            {
                Point newPos = Wrapper.UpdatePosition();
                _mTranslate.X = newPos.X - DesiredSize.Width / 2;
                _mTranslate.Y = newPos.Y - DesiredSize.Height / 2;
            }
        }


        private void OnNodeOffsetXChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodePortConstants.NodeOffsetX);
            UpdatePosition();
        }
        private void OnNodeOffsetYChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodePortConstants.NodeOffsetY);
            UpdatePosition();
        }
        private void OnNodeChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodePortConstants.Node);
        }
        private void OnUnitModeChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodePortConstants.UnitMode);
        }

        public OrthogonalDirection GetDirection()
        {
            throw new NotImplementedException();
        }
    }
}
