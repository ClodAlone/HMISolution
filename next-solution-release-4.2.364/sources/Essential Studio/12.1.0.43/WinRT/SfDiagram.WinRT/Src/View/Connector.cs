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
using System.Windows;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes; 
#else
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes; 
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Diagram
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public partial class Connector :
        ContentControl, 
        IConnector,
        IConnectorView,
        ISharedData
    {
        private SharedData _mSharedData = null;
        internal Panel AnnotationHost { get; private set; }

#if WPF
        static Connector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Connector), new FrameworkPropertyMetadata(typeof(Connector)));
        }
        
#endif
        public Connector()
        {
#if !WPF
            DefaultStyleKey = typeof(Connector); 
#endif
            //TargetDecorator = new PathGeometry()
            //{
            //    Figures = new PathFigureCollection()
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
            //};
#if WINRT
            this.Tapped += (s, e) => Wrapper.Tap(true);
            this.DoubleTapped += (s, e) => Wrapper.DoubleTap(true); 
#elif WPF
            //this.Click += (s, e) => Wrapper.Tap(true);
            this.MouseDoubleClick += (s, e) => Wrapper.DoubleTap(true); 
#elif SILVERLIGHT
            this.Tap += (s, e) => Wrapper.Tap(true);
            this.DoubleTap += (s, e) => Wrapper.DoubleTap(true); 
#endif
            this.Loaded += Connector_Loaded;
            _mInternalSegments = new PathSegmentCollection();
            _mPathFigure = new PathFigure() { Segments = _mInternalSegments };
            _mInternalSegmentsTrans = new PathSegmentCollection();
            _mPathFigureTrans = new PathFigure() { Segments = _mInternalSegmentsTrans };

            this.Geometry = new PathGeometry()
                {
                    Figures = new PathFigureCollection()
                        {
                            _mPathFigure
                        }
                };

            this.TransparentGeometry = new PathGeometry()
            {
                Figures = new PathFigureCollection()
                        {
                            _mPathFigureTrans
                        }
            };
        }

        void CleanAnnot(IInternalConnector wrapper)
        {
            if (wrapper.InternalAnnotations != null)
            {
                foreach (var internalAnnot in wrapper.InternalAnnotations)
                {
                    AnnotationHost.Children.Remove(internalAnnot.View);
                }
            }
        }

        void Connector_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Wrapper.InternalAnnotations != null)
            //{
            //    foreach (var internalAnnot in Wrapper.InternalAnnotations)
            //    {
            //        if (!AnnotationHost.Children.Contains(internalAnnot.View))
            //        {
            //            AnnotationHost.Children.Add(internalAnnot.View);
            //        }
            //    }
            //}
        }
        
        internal Geometry TransparentGeometry
        {
            get { return (Geometry)GetValue(TransparentGeometryProperty); }
            set { SetValue(TransparentGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TransparentGeometry.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TransparentGeometryProperty =
            DependencyProperty.Register("TransparentGeometry", typeof(Geometry), typeof(Connector), new PropertyMetadata(null));

        void ISharedData.Init(SharedData shared)
        {
            _mSharedData = shared;
        }

        internal PathSegmentCollection _mInternalSegments;
        internal PathFigure _mPathFigure;

        private PathSegmentCollection _mInternalSegmentsTrans;
        private PathFigure _mPathFigureTrans;

        private IInternalConnector _mWrapper;

        internal IInternalConnector Wrapper
        {
            get
            {
                return _mWrapper;
            }
            set
            {
                _mWrapper = value;
                if (value == null && _mWrapper!=null)
                {
                    CleanAnnot(_mWrapper);
                }
                //if (_mWrapper != null)
                //{
                //    this.Geometry = new PathGeometry()
                //        {
                //            Figures = new PathFigureCollection()
                //                {
                //                    Wrapper.PathFigure
                //                }
                //        };
                //}
                //else
                //{
                //    Geometry.Figures.Clear();
                //    Geometry = null;
                //}
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            return new Size(0,0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            UpdateGeometry();
            UpdateDecorator();
            UpdateAnnotations();
            Wrapper.UpdateRouting();
            Wrapper.UpdateBridging();
            CloneTrans();
            return new Size(0, 0);
        }

        private void CloneTrans()
        {
            _mPathFigureTrans.StartPoint = _mPathFigure.StartPoint;
            _mInternalSegments.Clone(_mInternalSegmentsTrans);
        }

        private void UpdateAnnotations()
        {
            if (Wrapper.InternalAnnotations != null)
            {
                foreach (AnnotationEditorWrapper annotation in Wrapper.InternalAnnotations)
                {
                    Point one = Wrapper.SourcePoint;
                    Point two = Wrapper.TargetPoint;
                    //HorizontalAlignment align = HorizontalAlignment.Stretch;
                    IConnectorSegment first = Wrapper.Segments.FirstOrDefault();
                    switch (annotation.Alignment)
                    {
                        case ConnectorAnnotationAlignment.Source:
                            annotation.View.AnnotateAt = one = SourcePoint;
                            annotation.View.AnotationAngle = one.FindAngle(two);
                            if (first is ILineSegment)
                            {
                            }
                            else if (first is IOrthogonalSegment)
                            {
                                PathSegmentCollection seg = Wrapper.InternalSegments;
                                IOrthogonalSegment ortho = Wrapper.Segments.Take(2).Last() as IOrthogonalSegment;
                                if (seg.Count > 2)
                                {
                                    annotation.View.AnnotateAt =
                                        one = (Wrapper.InternalSegments[0] as LineSegment).Point;
                                    two = (Wrapper.InternalSegments[1] as LineSegment).Point;
                                    annotation.View.AnotationAngle = one.FindAngle(two);
                                }
                            }
                            break;
                        case ConnectorAnnotationAlignment.Target:
                            annotation.View.AnnotateAt = one = TargetPoint;
                            two = SourcePoint;
                            annotation.View.AnotationAngle = two.FindAngle(one);
                            if (first is ILineSegment)
                            {
                            }
                            else if (first is IOrthogonalSegment)
                            {
                                PathSegmentCollection seg = Wrapper.InternalSegments;
                                IOrthogonalSegment ortho = Wrapper.Segments.Take(2).Last() as IOrthogonalSegment;
                                if (seg.Count > 2)
                                {
                                    annotation.View.AnnotateAt =
                                        one = (Wrapper.InternalSegments[1] as LineSegment).Point;
                                    two = (Wrapper.InternalSegments[0] as LineSegment).Point;
                                    annotation.View.AnotationAngle = one.FindAngle(two);
                                }
                            }
                            annotation.View.AnotationAngle += 180;
                            break;
                        case ConnectorAnnotationAlignment.Center:
                            Rect rect = new Rect(one, two);
                            annotation.View.AnnotateAt = new Point(rect.Left + rect.Width/2, rect.Top + rect.Height/2);
                            annotation.View.AnotationAngle = one.FindAngle(two);

                            if (first is ILineSegment)
                            {
                            }
                            else if (first is IOrthogonalSegment)
                            {
                                PathSegmentCollection seg = Wrapper.InternalSegments;
                                IOrthogonalSegment ortho = Wrapper.Segments.Take(2).Last() as IOrthogonalSegment;
                                if (seg.Count > 2)
                                {
                                    one = (Wrapper.InternalSegments[1] as LineSegment).Point;
                                    two = (Wrapper.InternalSegments[0] as LineSegment).Point;
                                    rect = new Rect(one, two);
                                    annotation.View.AnnotateAt = new Point(rect.Left + rect.Width/2,
                                                                           rect.Top + rect.Height/2);
                                    annotation.View.AnotationAngle = one.FindAngle(two);
                                }
                            }

                            break;
                    }
                    //switch (annotation.Orientation)
                    //{
                    //    case ConnectorAnnotationOrientation.Horizontal:
                    //        break;
                    //    case ConnectorAnnotationOrientation.Vertical:
                    //        break;
                    //    case ConnectorAnnotationOrientation.Auto:
                    //        break;
                    //}

                    double angle = annotation.View.AnotationAngle;
                    angle = angle%360;

                    if (angle > 0 && angle < 45)
                    {
                        annotation.View.HorizontalAlignment = HorizontalAlignment.Left;
                        annotation.View.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                    else if (angle > 45 && angle < 90)
                    {
                        annotation.View.HorizontalAlignment = HorizontalAlignment.Right;
                        annotation.View.VerticalAlignment = VerticalAlignment.Top;
                    }
                    else if (angle > 90 && angle < 137)
                    {
                        annotation.View.HorizontalAlignment = HorizontalAlignment.Left;
                        annotation.View.VerticalAlignment = VerticalAlignment.Top;
                    }
                    else if (angle > 137 && angle < 180)
                    {
                        annotation.View.HorizontalAlignment = HorizontalAlignment.Right;
                        annotation.View.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                    else if (angle > 180 && angle < 225)
                    {
                        annotation.View.HorizontalAlignment = HorizontalAlignment.Right;
                        annotation.View.VerticalAlignment = VerticalAlignment.Top;
                    }
                    else if (angle > 225 && angle < 270)
                    {
                        annotation.View.HorizontalAlignment = HorizontalAlignment.Left;
                        annotation.View.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                    else if (angle > 270 && angle < 315)
                    {
                        annotation.View.HorizontalAlignment = HorizontalAlignment.Right;
                        annotation.View.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                    else if (angle > 315 && angle < 360)
                    {
                        annotation.View.HorizontalAlignment = HorizontalAlignment.Left;
                        annotation.View.VerticalAlignment = VerticalAlignment.Top;
                    }

                    if (annotation.Alignment == ConnectorAnnotationAlignment.Center)
                    {
                        annotation.View.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                }
            }
        }

        CompositeTransform SourceDecoratorTransform = new CompositeTransform();
        CompositeTransform TargetDecoratorTransform = new CompositeTransform();
            Path PART_SourceDecorator;
            Path PART_TargetDecorator;

        private void DoApplyTemplate()
        {
            PART_SourceDecorator = GetTemplateChild("PART_SourceDecorator") as Path;
            PART_TargetDecorator = GetTemplateChild("PART_TargetDecorator") as Path;
            PART_SourceDecorator.RenderTransformOrigin = new Point(1, 0.5);
            PART_TargetDecorator.RenderTransformOrigin = new Point(1, 0.5);
#if WPF
            PART_SourceDecorator.RenderTransform = SourceDecoratorTransform.Transform;
            PART_TargetDecorator.RenderTransform = TargetDecoratorTransform.Transform; 
#else
            PART_SourceDecorator.RenderTransform = SourceDecoratorTransform;
            PART_TargetDecorator.RenderTransform = TargetDecoratorTransform;
#endif
            AnnotationHost = GetTemplateChild("PART_Annotations") as Panel;

            if (Wrapper != null && Wrapper.InternalAnnotations != null)
            {
                foreach (var internalAnnot in Wrapper.InternalAnnotations)
                {
                    if (!AnnotationHost.Children.Contains(internalAnnot.View))
                    {
                        AnnotationHost.Children.Add(internalAnnot.View);
                    }
                }
            }
        }

        protected virtual void UpdateGeometry()
        {
            Wrapper.UpdateGeometry();
        }

        internal virtual void UpdateDecorator()
        {
            SourceDecoratorTransform.TranslateX = Wrapper.SourcePoint.X - PART_SourceDecorator.ActualWidth;
            SourceDecoratorTransform.TranslateY = Wrapper.SourcePoint.Y - PART_SourceDecorator.ActualHeight / 2;
            TargetDecoratorTransform.TranslateX = Wrapper.TargetPoint.X - PART_TargetDecorator.ActualWidth;
            TargetDecoratorTransform.TranslateY = Wrapper.TargetPoint.Y - PART_TargetDecorator.ActualHeight / 2;

            PathSegment first = Wrapper.InternalSegments.First();
            PathSegment lastAdj = null;
            if (Wrapper.InternalSegments.Count > 1)
            {
                lastAdj = Wrapper.InternalSegments[Wrapper.InternalSegments.Count - 2];
            }
            PathSegment last = Wrapper.InternalSegments.Last();
            Point firstPoint = Wrapper.PathFigure.StartPoint;
            Point? secondPoint = null;
            Point? lastPoint = null;
            Point? lastAdjPoint = null;

            if (first != null)
            {
                if (first is LineSegment)
                {
                    secondPoint = (first as LineSegment).Point;
                }
                else if (first is ArcSegment)
                {
                    secondPoint = (first as ArcSegment).Point;
                }
                else if (first is BezierSegment)
                {
                    secondPoint = (first as BezierSegment).Point1;
                }
                else if (first is PolyBezierSegment)
                {
                    secondPoint = (first as PolyBezierSegment).Points.First();
                }
                else if (first is PolyLineSegment)
                {
                    secondPoint = (first as PolyLineSegment).Points.First();
                }
                else if (first is PolyQuadraticBezierSegment)
                {
                    secondPoint = (first as PolyQuadraticBezierSegment).Points.First();
                }
                else if (first is QuadraticBezierSegment)
                {
                    secondPoint = (first as QuadraticBezierSegment).Point1;
                }
                else
                {
                    throw new InvalidOperationException("Invalid segment");
                }
            }
            else
            {
                secondPoint = TargetPoint;
            }
            if (last != null)
            {
                lastPoint = last.GetEndPoint();

                if (last is LineSegment)
                {
                }
                else if (last is ArcSegment)
                {
                }
                else if (last is BezierSegment)
                {
                    lastAdjPoint = (last as BezierSegment).Point2;
                }
                else if (last is PolyBezierSegment)
                {
                    lastAdjPoint = (last as PolyBezierSegment).Points[(last as PolyBezierSegment).Points.Count - 2];
                }
                else if (last is PolyLineSegment)
                {
                    lastAdjPoint = (last as PolyLineSegment).Points[(last as PolyLineSegment).Points.Count - 2];
                }
                else if (last is PolyQuadraticBezierSegment)
                {
                    lastAdjPoint = (last as PolyQuadraticBezierSegment).Points[(last as PolyQuadraticBezierSegment).Points.Count - 2];
                }
                else if (last is QuadraticBezierSegment)
                {
                    lastAdjPoint = (last as QuadraticBezierSegment).Point1;
                }

                if (lastAdjPoint == null && lastAdj != null)
                {
                    lastAdjPoint = lastAdj.GetEndPoint();
                }
                if (lastAdjPoint == null)
                {
                    lastAdjPoint = firstPoint;
                }
            }
            else
            {
                lastAdjPoint = firstPoint;
                lastPoint = TargetPoint;
            }
            SourceDecoratorTransform.Rotation = secondPoint.Value.FindAngle(firstPoint);
            TargetDecoratorTransform.Rotation = lastAdjPoint.Value.FindAngle(lastPoint.Value);
        }

        public PathGeometry Geometry
        {
            get { return (PathGeometry)GetValue(GeometryProperty); }
            private set { SetValue(GeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Geometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(PathGeometry), typeof(Connector), new PropertyMetadata(null, OnGeometryChanged));

        private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnGeometryChanged(e);
        }

        private void OnGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void BindToINode(IConnector source)
        {
            if (source != null)
            {
                Binding bind;
                foreach (var property in ConnectorConstants.ConnectorProperties)
                {
                    bind = new Binding();
                    bind.Path = new PropertyPath(property.Item2);
                    bind.Mode = BindingMode.TwoWay;
                    //bind.Source = source;
                    if (ReadLocalValue(property.Item1) == DependencyProperty.UnsetValue)
                    {
                        this.SetBinding(property.Item1, bind);
                    }
                    else
                    {

                    }
                }
            }
        }

        private object BusinessObject { get; set; }

        void IView.SetBusinessObject(object item)
        {
            this.BusinessObject = item;
            if (AutoBind && BusinessObject is IConnector)
            {
                BindToINode(BusinessObject as IConnector);
            }
        }

        private void OnAutoBindChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void OnSourceNodeChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.SourceNode);
        }
        private void OnTargetNodeChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.TargetNode);
        }
        private void OnSourcePointChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.SourcePoint);
            InvalidateArrange();
        }
        private void OnTargetPointChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.TargetPoint);
            InvalidateArrange();            
        }
        private void OnIDChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.ID);
        }
        private void OnKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.Key);
        }
        private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.IsSelected); 
        }

        private void OnAnnotationsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged("Annotations");
        }

        private void OnZIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            int oldValue = (int)e.OldValue;
            int newValue = (int)e.NewValue;
            Canvas.SetZIndex(this, newValue);
            OnPropertyChanged("ZIndex");
        }
        private void OnBridgeSpaceChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.BridgeSpace);
        }

        private void OnParentGroupChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.ParentGroup);
        }

        private void OnSegmentsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.Segments);
        }
        private void OnSourcePortChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.SourcePort);
        }
        private void OnTargetPortChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.TargetPort);
        }

        private void OnConnectorGeometryStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.ConnectorGeometryStyle);
        }

        private void OnSourceDecoratorChanged(DependencyPropertyChangedEventArgs e)
        {
            InvalidateArrange();
            OnPropertyChanged(ConnectorConstants.SourceDecorator);
        }

        private void OnTargetDecoratorChanged(DependencyPropertyChangedEventArgs e)
        {
            InvalidateArrange();
            OnPropertyChanged(ConnectorConstants.TargetDecorator);
        }

        private void OnSourceDecoratorStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.SourceDecoratorStyle);
        }
        private void OnTargetDecoratorStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(ConnectorConstants.TargetDecoratorStyle);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property));
        }

        private void OnConstraintsChanged(DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void OnBezierSmoothnessChanged(DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        public object Info { get; set; }

        void IDisposable.Dispose()
        {
        }

        internal IEnumerable<PathSegment> Internal_CreateSegments(Point start, Point end, double angle)
        {
            foreach (PathSegment segment in CreateSegments(start, end, angle))
            yield return segment;
        }

        protected virtual IEnumerable<PathSegment> CreateSegments(Point start, Point end, double angle)
        {
            SweepDirection sd;
            if (angle > 0 && angle < 180)
            {
                sd = SweepDirection.Clockwise;
            }
            else
            {
                sd = SweepDirection.Counterclockwise;
            }
            ArcSegment arc = new ArcSegment()
                {
                    Point = end,
                    Size = new Size(1, 1),
                    SweepDirection = sd
                };
            yield return arc;
        }
    }
}
