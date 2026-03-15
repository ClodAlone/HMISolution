#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Reflection;
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Syncfusion.UI.Xaml.Diagram.Utility;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal partial class NodePortWrapper :
        DiagramElementWrapper,
        IInternalNodePort
    {
        //private INodePort _mKnownSource;
        public object InternalID { get; set; }

        public NodePortWrapper(INodePort source,
                                 SharedData sharedData): base(sharedData)
        {
            Source = source;
        }

        protected override void SharedDataInitialized()
        {
        }

        protected override void SourceChanged()
        {
            if (_mSource is NodePort)
            {
                View = _mSource as NodePort;
            }
            else
            {
                View = new NodePort();
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            
        }

        //public double NodeOffsetX
        //{
        //    get { return Source.NodeOffsetX; }
        //    set
        //    {
        //        if (Source.NodeOffsetX != value)
        //        {
        //            Source.NodeOffsetX = value;
        //            OnPropertyChanged(NodePortConstants.NodeOffsetX);
        //        }
        //    }
        //}

        //public double NodeOffsetY
        //{
        //    get { return Source.NodeOffsetY; }
        //    set
        //    {
        //        if (Source.NodeOffsetY != value)
        //        {
        //            Source.NodeOffsetY = value;
        //            OnPropertyChanged(NodePortConstants.NodeOffsetY);
        //        }
        //    }
        //}

        //public TNode Node
        //{
        //    get { return Source.Node; }
        //    set {
        //        if (Source.Node == null || !Source.Node.Equals(value))
        //        {
        //            Source.Node = value;
        //            OnPropertyChanged(NodePortConstants.Node);
        //        }
        //    }
        //}
        //public UnitMode UnitMode
        //{
        //    get { return Source.UnitMode; }
        //    set
        //    {
        //        if (Source.UnitMode != value)
        //        {
        //            Source.UnitMode = value;
        //            OnPropertyChanged(NodePortConstants.UnitMode);
        //        }
        //    }
        //}

        //public object Key
        //{
        //    get { return Source.Key; }
        //    set { Source.Key = value; }
        //}

        //public TID ID
        //{
        //    get { return Source.ID; }
        //    set { Source.ID = value; }
        //}

        public OrthogonalDirection GetDirection()
        {
            return KnownNode.GetDirection(new Point(OffsetX, OffsetY));
        }

        //private object _mSource;

        //public object Source
        //{
        //    get { return _mKnownSource; }
        //    private set
        //    {
        //        _mKnownSource = (INodePort)value;
        //        if (_mKnownSource != null)
        //        {
        //            _mKnownSource.PropertyChanged += KnownSource_PropertyChanged;
        //        }
        //        if (_mKnownSource is NodePort)
        //        {
        //            View = _mKnownSource as NodePort;
        //        }
        //        else
        //        {
        //            View = new NodePort();
        //        }
        //    }
        //}

        //private void KnownSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    OnPropertyChanged(e.PropertyName);
        //}

        public double OffsetX
        {
            get { return _offsetX; }
            set
            {
                _offsetX = value;
                if (PositionChanged != null)
                {
                    PositionChanged.Invoke();
                }
            }
        }

        public double OffsetY
        {
            get { return _offsetY; }
            set
            {
                _offsetY = value;
                if (PositionChanged != null)
                {
                    PositionChanged.Invoke();
                }
            }
        }

        public event Action PositionChanged;

        //public Geometry Shape
        //{
        //    get { return _mKnownSource.Shape; }
        //    set { _mKnownSource.Shape = value; }
        //}
        //public Style ShapeStyle
        //{
        //    get { return _mKnownSource.ShapeStyle; }
        //    set { _mKnownSource.ShapeStyle = value; }
        //}
        //public PortConstraints Constraints
        //{
        //    get { return _mKnownSource.Constraints; }
        //    set { _mKnownSource.Constraints = value; }
        //}

        private NodePort _mView;

        public NodePort View
        {
            get { return _mView; }
            set
            {
                _mView = value;
                if (_mView != null)
                {
                    _mView.Wrapper = this;
                }
            }
        }

        private IInternalNode _mKnownNode;
        private double _offsetX;
        private double _offsetY;

        public IInternalNode KnownNode
        {
            get { return _mKnownNode; }
            set
            {
                if (_mKnownNode != value)
                {
                    var old = _mKnownNode;
                    _mKnownNode = value;
                    OnKnownNodeChanged(old, value);
                }
            }
        }

        private void OnKnownNodeChanged(IInternalNode oldNode, IInternalNode newNode)
        {
            if (oldNode != null)
            {
                oldNode.BoundsChanged -= knownNode_BoundsChanged;
            }
            if (newNode != null)
            {
                newNode.BoundsChanged += knownNode_BoundsChanged;
            }
        }

        void knownNode_BoundsChanged(IInternalGroupable obj)
        {
            UpdatePosition();
            if (View != null)
            {
                View.InvalidateArrange();
            }
        }


        public Point UpdatePosition()
        {
            IInternalNode node = KnownNode;
            Point absPoint = new Point(UnitMode == UnitMode.Absolute ? NodeOffsetX : node.DesiredSize.Width*NodeOffsetX,
                                       UnitMode == UnitMode.Absolute ? NodeOffsetY : node.DesiredSize.Height*NodeOffsetY);

            //_mTranslate.X = pixelX - DesiredSize.Width/2;
            //_mTranslate.Y = pixelY - DesiredSize.Height/2;
            if (KnownNode.Corners != null)
            {
                var corner = KnownNode.Corners.Value;
                Point pagePosition = corner.Transform(absPoint);
                //TransformToVisual(this.FindVisualParent<DiagramPage>())
                //    .TransformPoint(new Point(ActualWidth / 2, ActualHeight / 2));
                OffsetX = pagePosition.X;
                OffsetY = pagePosition.Y;
            }
            double x=OffsetX, y = OffsetY;
            if (SharedData != null && SharedData.ScrollViewer!=null)
            {
                double currentZoom = SharedData.ScrollViewer.CurrentZoom;
                x =  x * currentZoom;
                y =  y * currentZoom;
            }
            return new Point(x,y);
        }
    }
}
