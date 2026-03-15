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
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Data;
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Controller;

namespace Syncfusion.UI.Xaml.Diagram.Controls
{
    internal sealed partial class Adorner : Panel, ISharedData
    {
        int _mSelectionCount;
        UIElement _mFirstItem;
        readonly Stack<Rectangle> _mNodeSelectionBin;
        readonly Stack<Path> _mConnectorSelectionBin;
        SolidColorBrush _mSelectionIndicatorBrush;
        List<UIElement> _mGuidelines;

        # region Selction preview

        private void Selected(SelectionArgs<IInternalGroupable> item)
        {
            if (item.Source.SelectionPreview == null)
            {
                IInternalNode node = item.Source as IInternalNode;
                if (node != null)
                {
                    CreateSelectionPreview(node);
                }
                else
                {
                    CreateSelectionPreview(item.Source as IInternalConnector);
                }
            }
        }

        private void UnSelected(SelectionArgs<IInternalGroupable> item)
        {
            IInternalNode node = item.Source as IInternalNode;
            if (node != null)
            {
                RemoveSelectionPreview(node);
            }
            else
            {
                RemoveSelectionPreview(item.Source as IInternalConnector);
            }
            if (_mSharedData.Graph.InternalSelectedItems.FirstSelectedItem != null &&
                node == _mSharedData.Graph.InternalSelectedItems.FirstSelectedItem)
            {
                _mSharedData.Graph.InternalSelectedItems.FirstSelectedItem = null;
            }
        }

        private void CreateSelectionPreview(IInternalNode node)
        {
            Rectangle preview = _mNodeSelectionBin.Count > 0 ? _mNodeSelectionBin.Pop() : null;
            if (preview == null)
            {
                preview = new Rectangle()
                {
                    Stroke = _mSelectionIndicatorBrush,
                    StrokeThickness = 1,
                    IsHitTestVisible = false
                };
            }
            preview.RenderTransformOrigin = new Point(0.5, 0.5);
#if !WPF
            preview.RenderTransform = new CompositeTransform();
#else 
            preview.RenderTransform=new CompositeTransform().Transform;
#endif
            node.SelectionPreview = preview;
            AddPreview(node, preview);
            UpdatePreview(node, preview);
        }

        private void CreateSelectionPreview(IInternalConnector connector)
        {
            Path preview = _mConnectorSelectionBin.Count > 0 ? _mConnectorSelectionBin.Pop() : null;
            if (preview == null)
            {
                preview = new Path()
                    {
                        Data = new PathGeometry()
                            {
                                Figures = new PathFigureCollection()
                                    {
                                        new PathFigure()
                                    }
                            },
                        Stroke = _mSelectionIndicatorBrush,
                        StrokeThickness = 1,
                        IsHitTestVisible = false,
                        RenderTransformOrigin = new Point(0, 0),
                        Stretch = Stretch.Fill,
                        RenderTransform = new TranslateTransform()
                    };
            }
            //preview.RenderTransform = new ScaleTransform();
            connector.SelectionPreview = preview;
            AddPreview(connector, preview);
            UpdatePreview(connector, preview);
        }

        private void RemoveSelectionPreview(IInternalNode node)
        {
            Rectangle element = node.SelectionPreview as Rectangle;
            RemovePreview(element);
            _mNodeSelectionBin.Push(element);
            node.SelectionPreview = null;
        }

        private void RemoveSelectionPreview(IInternalConnector connector)
        {
            Path element = connector.SelectionPreview as Path;
            RemovePreview(element);
            _mConnectorSelectionBin.Push(element);
            connector.SelectionPreview = null;
        }

        internal void UpdatePreview(IInternalNode node, Rectangle view = null)
        {
            if (view == null)
            {
                if (node.SelectionPreview == null)
                {
                    CreateSelectionPreview(node);
                }
                view = node.SelectionPreview as Rectangle;
            }
            if (node.View != null)
            {
                view.Width = node.ActualWidth * Scale;
                view.Height = node.ActualHeight * Scale;
#if WPF
                Transform transform = view.RenderTransform;
#else
                CompositeTransform transform = view.RenderTransform as CompositeTransform;
                 transform.TranslateX = (node.OffsetX - node.ActualWidth * node.Pivot.X) * Scale;
                transform.TranslateY = (node.OffsetY - node.ActualHeight * node.Pivot.Y) * Scale;
                transform.Rotation = node.RotateAngle;
#endif

            }
        }

        void UpdateSelectionPreviews()
        {
            foreach (IInternalNode node in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
            {
                UpdatePreview(node);
            }
            foreach (IInternalConnector conn in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
            {
                UpdatePreview(conn);
            }
        }

        internal void UpdatePreview(IInternalConnector conn, Path view = null)
        {
            if (view == null)
            {
                if (conn.SelectionPreview == null)
                {
                    CreateSelectionPreview(conn);
                }
                view = conn.SelectionPreview as Path;
            }
            PathFigure targetGeometry = (view.Data as PathGeometry).Figures[0];
            var connector = conn.View as Connector;
            if (connector != null)
            {
                PathFigure sourceGeometry = (connector.TransparentGeometry as PathGeometry).Figures[0];
                targetGeometry.StartPoint = sourceGeometry.StartPoint;
                sourceGeometry.Segments.Clone(targetGeometry.Segments);

                //Solution 1: Updating the preview by changing the points based on scale value

                //foreach (PathSegment segment in targetGeometry.Segments)
                //{
                //    if (segment is BezierSegment)
                //    {
                //        BezierSegment bezier = segment as BezierSegment;
                //        bezier.Point1 = new Point(bezier.Point1.X * zoom, bezier.Point1.Y * zoom);
                //        bezier.Point2 = new Point(bezier.Point2.X * zoom, bezier.Point2.Y * zoom);
                //        bezier.Point3 = new Point(bezier.Point3.X * zoom, bezier.Point3.Y * zoom);
                //    }
                //    else
                //    {
                //        Point pt = segment.GetEndPoint();
                //        segment.SetEndPoint(new Point(pt.X * zoom, pt.Y * zoom));
                //    }
                //}

                //Solution 2: Scaling the preview

                view.Width = connector.Geometry.Bounds.Width * Scale;
                view.Height = connector.Geometry.Bounds.Height * Scale;
                (view.RenderTransform as TranslateTransform).X = connector.Geometry.Bounds.Left * Scale;
                (view.RenderTransform as TranslateTransform).Y = connector.Geometry.Bounds.Top * Scale;
            }
        }

        private void AddPreview(IInternalGroupable node, UIElement element)
        {
            if (_mSelectionCount == 0)
            {
                _mFirstItem = element;
                _mSharedData.Graph.InternalSelectedItems.FirstSelectedItem = node;
            }
            else if (_mSelectionCount == 1)
            {
                Children.Insert(1, _mFirstItem);
                _mFirstItem = null;
                Children.Insert(2, element);
            }
            else
            {
                Children.Insert(_mSelectionCount + 1, element);
            }
            _mSelectionCount++;
        }

        private void RemovePreview(UIElement element)
        {
            Children.Remove(element);
            if (_mSelectionCount == 2)
            {
                _mFirstItem = Children[1];
                Children.RemoveAt(1);
            }
            _mSelectionCount--;

        }

        #endregion

        #region Snapping lines

        internal void AddGuideline(UIElement element)
        {
            Children.Add(element);
            _mGuidelines.Add(element);
        }

        internal void ClearGuidelines()
        {
            foreach (UIElement elemnt in _mGuidelines)
            {
                Children.Remove(elemnt);
            }
            _mGuidelines.Clear();
        }

        internal void DrawGuideLine(Point start, Point end,
                               string type)
        {
            start = new Point(start.X * Scale, start.Y * Scale);
            end = new Point(end.X * Scale, end.Y * Scale);
            this.DrawLine(start, end);
        }

        internal void DrawGuideLine(Point start, Point end)
        {
            Color color = new Color() { A = 255, R = 229, G = 30, B = 37 };

            start = new Point(start.X * Scale, start.Y * Scale);
            end = new Point(end.X * Scale, end.Y * Scale);
            if (start.Y == end.Y)
            {
                string data = "M5,0 L0,5 L5,10";
                Path path = new Path();
                path.UseLayoutRounding = true;
                path.Data = data.ParseGeometry();
                path.Stroke = new SolidColorBrush(color);
                path.StrokeThickness = 1;
                path.RenderTransform = new TranslateTransform() { X = start.X, Y = start.Y - 5 };
                //Canvas.SetLeft(path, start.X);
                //Canvas.SetTop(path, start.Y - 5);
                AddGuideline(path);
                Line line = new Line()
                {
                    X1 = start.X,
                    X2 = end.X,
                    Y1 = start.Y,
                    Y2 = end.Y,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(color),
                    StrokeStartLineCap = PenLineCap.Round,
                    UseLayoutRounding = true
                };
                AddGuideline(line);
                data = "M0,0 L5,5 L0,10";
                Path path2 = new Path();
                path2.UseLayoutRounding = true;
                path2.Data = data.ParseGeometry();
                path2.Stroke = new SolidColorBrush(color);
                path2.StrokeThickness = 1;
                path2.RenderTransform = new TranslateTransform() { X = end.X - 5, Y = end.Y - 5 };
                //Canvas.SetLeft(path2, end.X - 5);
                //Canvas.SetTop(path2, end.Y - 5);
                AddGuideline(path2);
            }
            else
            {
                if (start.X == end.X)
                {
                    string data = "M0,5 L5,0 L10,5";
                    Path path = new Path();
                    path.UseLayoutRounding = true;
                    path.Data = data.ParseGeometry();
                    path.Stroke = new SolidColorBrush(color);
                    path.StrokeThickness = 1;
                    path.RenderTransform = new TranslateTransform() { X = start.X - 5, Y = start.Y };
                    //Canvas.SetLeft(path, start.X - 5);
                    //Canvas.SetTop(path, start.Y);
                    AddGuideline(path);
                    Line line = new Line()
                    {
                        X1 = start.X,
                        X2 = end.X,
                        Y1 = start.Y,
                        Y2 = end.Y,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(color),
                        StrokeStartLineCap = PenLineCap.Round,
                        UseLayoutRounding = true
                    };
                    AddGuideline(line);
                    data = "M0,0 L5,5 L10,0";
                    Path path2 = new Path();
                    path2.UseLayoutRounding = true;
                    path2.Data = data.ParseGeometry();
                    path2.Stroke = new SolidColorBrush(color);
                    path2.StrokeThickness = 1;
                    path2.RenderTransform = new TranslateTransform() { X = end.X - 5, Y = end.Y - 5 };
                    //Canvas.SetLeft(path2, end.X - 5);
                    //Canvas.SetTop(path2, end.Y - 5);
                    AddGuideline(path2);
                }
            }
        }

        private void DrawLine(Point start, Point end)
        {
            Color color = new Color() { A = 255, R = 229, G = 30, B = 37 };
            Line line = new Line()
            {
                X1 = start.X,
                X2 = end.X,
                Y1 = start.Y,
                Y2 = end.Y,
                StrokeThickness = 1,
                UseLayoutRounding = true,
                Stroke = new SolidColorBrush(color)
            };
            AddGuideline(line);
        }

        #endregion
    }

}
