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
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation; 
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Controller;

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface IGroupable : IDiagramElement
    {
        bool IsSelected { get; set; }
        int ZIndex { get; set; }
        object Annotations { get; set; }
        object ParentGroup { get; set; }
        object Info { get; set; }
    }

    public interface IGroupableInfo
    {
        bool IsGrouped { get; }
        Rect Bounds { get; }
        IGraph Graph { get; }
    }
    

    internal interface IInternalGroupable :
        IGroupable, IGroupableInfo, IInternalDiagramElement, IWrapper, IDisposable
    {
        RectCorners? Corners { get; }
        Point Center { get; }
        Quad Quad { get; set; }
        UIElement SelectionPreview { get; set; }

        bool CanVirtualize { get; }
        VirtualizationState VirtualizationState { get; }

        void SetVirtualizationState(VirtualizationState value);
        IView View { get; set; }
        event SimpleEventHandler BoundsChanged;
        //Rect ParentBounds { get; }
        //event Action<IInternalGroupable> ParentBoundsChanged;
        IInternalGroup KnownParentGroup { get; set; }
        ObservableElements<IAnnotation, AnnotationEditorWrapper> InternalAnnotations { get; set; }
        bool UserInteracting { get; set; }
        void Tap(bool handled);
        void DoubleTap(bool handled);
        void PointerDragStarted();
        void RubberbandSelect();
    }

    internal delegate void SimpleEventHandler(IInternalGroupable sender);

    internal struct RectCorners
    {

        //double oneX, twoX, threeX, oneY, twoY, threeY;
        //private Rect outerRect;
        private Point _topLeft;
        private Point _top;
        private Point _topRight;
        private Point _left;
        //private Point _center;
        private Point _right;
        private Point _bottomLeft;
        private Point _bottom;
        private Point _bottomRight;
        private bool _isActual;
        private Rect _outerBounds;
        private MatrixExt _matrix;

        private double _origX;
        private double _origY;
        //private double _origW;
        //private double _origH;

        public RectCorners(Rect rect, bool actual, Point pivot, out Point _center, double angle)
        {
            _isActual = actual;

            _origX = rect.Left;
            _origY = rect.Top;
            double _origW = rect.Width;
            double _origH = rect.Height;

            double w = _origW;
            _origX = _origX - pivot.X * w;
            double twoX = _origX + w / 2;
            double threeX = _origX + w;

            double h = _origH;
            _origY = _origY - pivot.Y * h;
            double twoY = _origY + h / 2;
            double threeY = _origY + h;

            _topLeft = new Point(_origX, _origY);
            _top = new Point(twoX, _origY);
            _topRight = new Point(threeX, _origY);

            _left = new Point(_origX, twoY);
            _center = new Point(twoX, twoY);
            _right = new Point(threeX, twoY);

            _bottomLeft = new Point(_origX, threeY);
            _bottom = new Point(twoX, threeY);
            _bottomRight = new Point(threeX, threeY);

            _matrix = MatrixExt.Identity;

            //Rect outerRect = rect;
            if (angle != 0)
            {
                _matrix.RotateAt(angle, rect.Left, rect.Top);
                _topLeft = _matrix.Transform(_topLeft);
                _top = _matrix.Transform(_top);
                _topRight = _matrix.Transform(_topRight);
                _left = _matrix.Transform(_left);
                _center = _matrix.Transform(_center);
                _right = _matrix.Transform(_right);
                _bottomLeft = _matrix.Transform(_bottomLeft);
                _bottom = _matrix.Transform(_bottom);
                _bottomRight = _matrix.Transform(_bottomRight);

                if (actual)
                {
                }
                else
                {
                    angle = angle%360;
                    if (angle < 0)
                    {
                        angle += 360;
                    }
                    if (angle > 315 || angle < 45)
                    {
                        //_outerBounds = rect;
                        //_topLeft = _topLeft;
                        //_top = _top;
                        //_topRight = _topRight;
                        //_left = _left;
                        //_center = _center;
                        //_right = _right;
                        //_bottomLeft = _bottomLeft;
                        //_bottom = _bottom;
                        //_bottomRight = _bottomRight;
                    }
                    else if (angle < 135)
                    {
                        Point temp = _topRight;
                        _topRight = _topLeft;
                        _topLeft = _bottomLeft;
                        _bottomLeft = _bottomRight;
                        _bottomRight = temp;

                        temp = _top;
                        _top = _left;
                        _left = _bottom;
                        _bottom = _right;
                        _right = temp;

                        //_center = _center;
                    }
                    else if (angle < 225)
                    {
                        Point temp = _topLeft;
                        _topLeft = _bottomRight;
                        _bottomRight = temp;

                        temp = _top;
                        _top = _bottom;
                        _bottom = temp;

                        temp = _topRight;
                        _topRight = _bottomLeft;
                        _bottomLeft = temp;

                        temp = _left;
                        _left = _right;
                        _right = temp;

                        //_center = _center;
                    }
                    else
                    {
                        Point temp = _topLeft;
                        _topLeft = _topRight;
                        _topRight = _bottomRight;
                        _bottomRight = _bottomLeft;
                        _bottomLeft = temp;

                        temp = _top;
                        _top = _right;
                        _left = _top;
                        _bottom = _left;
                        _right = temp;

                        //_center = _center;
                    }
                }

                double minX = double.PositiveInfinity;
                double minY = double.PositiveInfinity;
                double maxX = double.NegativeInfinity;
                double maxY = double.NegativeInfinity;

                foreach (var current in
                    new Point[]
                        {
                            _topLeft, _top, _topRight,
                            _left, _right,
                            _bottomLeft, _bottom, _bottomRight
                        })
                {
                    minX = Math.Min(minX, current.X);
                    minY = Math.Min(minY, current.Y);
                    maxX = Math.Max(maxX, current.X);
                    maxY = Math.Max(maxY, current.Y);
                }
                _outerBounds = new Rect(minX, minY, maxX - minX, maxY - minY);
            }
            else
            {
                _outerBounds = new Rect(_topLeft, _bottomRight);
            }
        }

        //internal Point Update(Rect rect, Point pivot, double angle = 0)
        //{
        //    double oneX = rect.Left;
        //    double w = rect.Width;
        //    oneX = oneX - pivot.X * w;
        //    double twoX = oneX + w / 2;
        //    double threeX = oneX + w;

        //    double oneY = rect.Top;
        //    double h = rect.Height;
        //    oneY = oneY - pivot.Y * h;
        //    double twoY = oneY + h / 2;
        //    double threeY = oneY + h;

        //    _topLeft = new Point(oneX, oneY);
        //    _top = new Point(twoX, oneY);
        //    _topRight = new Point(threeX, oneY);

        //    _left = new Point(oneX, twoY);
        //    Point _center = new Point(twoX, twoY);
        //    _right = new Point(threeX, twoY);

        //    _bottomLeft = new Point(oneX, threeY);
        //    _bottom = new Point(twoX, threeY);
        //    _bottomRight = new Point(threeX, threeY);

        //    //Rect outerRect = rect;
        //    if (angle != 0)
        //    {
        //        MatrixExt matrix = MatrixExt.Identity;
        //        matrix.RotateAt(angle, oneX, oneY);

        //        if (_isActual)
        //        {
        //            _topLeft = matrix.Transform(_topLeft);
        //            _top = matrix.Transform(_top);
        //            _topRight = matrix.Transform(_topRight);
        //            _left = matrix.Transform(_left);
        //            _center = matrix.Transform(_center);
        //            _right = matrix.Transform(_right);
        //            _bottomLeft = matrix.Transform(_bottomLeft);
        //            _bottom = matrix.Transform(_bottom);
        //            _bottomRight = matrix.Transform(_bottomRight);
        //        }
        //        else
        //        {
        //            angle = angle % 360;
        //            if (angle < 0)
        //            {
        //                angle += 360;
        //            }
        //            if (angle > 315 || angle < 45)
        //            {
        //                _outerBounds = rect;
        //                return _center;
        //                //_topLeft = _topLeft;
        //                //_top = _top;
        //                //_topRight = _topRight;
        //                //_left = _left;
        //                //_center = _center;
        //                //_right = _right;
        //                //_bottomLeft = _bottomLeft;
        //                //_bottom = _bottom;
        //                //_bottomRight = _bottomRight;
        //            }
        //            else if (angle < 135)
        //            {
        //                _topLeft = _bottomLeft;
        //                _top = _left;
        //                _topRight = _topLeft;
        //                _left = _bottom;
        //                //_center = _center;
        //                _right = _top;
        //                _bottomLeft = _bottomRight;
        //                _bottom = _right;
        //                _bottomRight = _topRight;
        //            }
        //            else if (angle < 225)
        //            {
        //                _topLeft = _bottomRight;
        //                _top = _bottom;
        //                _topRight = _bottomLeft;
        //                _left = _right;
        //                //_center = _center;
        //                _right = _left;
        //                _bottomLeft = _topRight;
        //                _bottom = _top;
        //                _bottomRight = _topLeft;
        //            }
        //            else
        //            {
        //                _topLeft = _topRight;
        //                _top = _right;
        //                _topRight = _bottomRight;
        //                _left = _top;
        //                //_center = _center;
        //                _right = _bottom;
        //                _bottomLeft = _topLeft;
        //                _bottom = _left;
        //                _bottomRight = _bottomLeft;
        //            }

        //            double minX = double.NegativeInfinity;
        //            double minY = double.NegativeInfinity;
        //            double maxX = double.PositiveInfinity;
        //            double maxY = double.PositiveInfinity;

        //            foreach (var current in 
        //                    new Point[] {
        //                        _topLeft,_top,_topRight,
        //                        _left, _right,
        //                        _bottomLeft, _bottom, _bottomRight
        //                    })
        //            {
        //                minX = Math.Min(minX, current.X);
        //                minY = Math.Min(minY, current.Y);
        //                maxX = Math.Max(maxX, current.X);
        //                maxY = Math.Max(maxY, current.Y);
        //            }
        //            _outerBounds = new Rect(minX, minY, maxX - minX, maxY - minY);
        //        }
        //        _outerBounds = rect;
        //    }
        //    else
        //    {
        //        _outerBounds = rect;
        //    }
        //    return _center;
        //}

        public Point Transform(Point point)
        {
            return _matrix.Transform(new Point(_origX + point.X, _origY + point.Y));
        }

        public Point TopLeft
        {
            get { return _topLeft; }
            //get { return new Point(oneX, oneY); }
        }

        public Point Top
        {
            //get { return new Point(twoX, oneY); }
            get { return _top; }
        }

        public Point TopRight
        {
            //get { return new Point(threeX, oneY); }
            get { return _topRight; }
        }

        public Point Left
        {
            //get { return new Point(oneX, twoY); }
            get { return _left; }
        }

        //public Point Center
        //{
        //    //get { return new Point(twoX, twoY); }
        //    get { return _center; }
        //}

        public Point Right
        {
            //get { return new Point(threeX, twoY); }
            get { return _right; }
        }

        public Point BottomLeft
        {
            //get { return new Point(oneX, threeY); }
            get { return _bottomLeft; }
        }

        public Point Bottom
        {
            //get { return new Point(twoX, threeY); }
            get { return _bottom; }
        }

        public Point BottomRight
        {
            //get { return new Point(threeX, threeY); }
            get { return _bottomRight; }
        }

        internal bool IsActual
        {
            get { return _isActual; }
        }

        public Rect OuterBounds
        {
            //get { return outerRect; }
            get { return _outerBounds; }
        }

        internal static Rect GetTransformBounds(
           Point pivot,
           double x,
           double y,
           double width,
           double height,
           MatrixExt matrix,
           ref Point[] corners)
        {
            if (corners == null)
            {
                corners = new Point[9]
                    {
                        matrix.Transform(new Point(x, y)),
                        matrix.Transform(new Point(x + width/2, y)),
                        matrix.Transform(new Point(x + width, y)),

                        matrix.Transform(new Point(x, y + height/2)),
                        matrix.Transform(new Point(x + width/2, y + height/2)),
                        matrix.Transform(new Point(x + width, y + height/2)),

                        matrix.Transform(new Point(x, y + height)),
                        matrix.Transform(new Point(x + width/2, y + height)),
                        matrix.Transform(new Point(x + width, y + height))
                    };
            }
            else
            {
                for (int i = 0; i < corners.Length; i++)
                {
                    var corner = corners[i];
                    Point trans = matrix.Transform(corner);
                    corners[i] = trans;
                }
            }
            double left = double.PositiveInfinity;
            double top = double.PositiveInfinity;
            double right = double.NegativeInfinity;
            double bottom = double.NegativeInfinity;

            foreach (Point corner in corners)
            {
                left = corner.X.Min(left, false);
                top = corner.Y.Min(top, false);
                right = corner.X.Max(right, false);
                bottom = corner.Y.Max(bottom, false);
            }
            Rect outterRect = new Rect(new Point(left, top), new Point(right, bottom));
            return outterRect;
        }

    }
}