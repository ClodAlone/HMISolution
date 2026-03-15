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
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    
    //public class SelectedEventArgs<T>
    //{
    //    public T Item { get; private set; }

    //    public SelectedEventArgs(T item)
    //    {
    //        Item = item;
    //    }
    //}

    //public class UnSelectedEventArgs<T>
    //{
    //    public T Item { get; private set; }

    //    public UnSelectedEventArgs(T item)
    //    {
    //        Item = item;
    //    }
    //}
    
    public class DiagramEventArgs
    {
        public object Item { get; private set; }

        public DiagramEventArgs(object item)
        {
            Item = item;
        }
    }

    public class DiagramPreviewEventArgs:DiagramEventArgs
    {
        private bool _mcancel = false;
        public bool Cancel
        {
            get
            {
                return _mcancel;
            }
            set
            {
                if (_mcancel != value)
                {
                    _mcancel = value;
                }
            }
        }

        public DiagramPreviewEventArgs(object item):base(item)
        {
           
        }
    }

    public enum ItemSource
    {
        UnKnown,
        Stencil,
        DrawingTool
    }

    public class ItemAddedEventArgs : DiagramEventArgs
    {
        public ItemSource ItemSource { get; private set; }
        public ItemAddedEventArgs(object item, ItemSource source) : base(item)
        {
            ItemSource = source;
        }
    }

    

    public class ChangeEventArgs<TSource, TChange> : DiagramEventArgs
    {
        public TChange OldValue { get; private set; }
        public TChange NewValue { get; private set; }

        public ChangeEventArgs( TSource item,ref TChange oldValue,ref TChange newValue)
            : base(item)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class SymbolDroppingEventArgs
    {
        private SymbolDropMode _symbolDropMode;

        public SymbolDropMode SymbolDropMode
        {
            get { return _symbolDropMode; }
            set { _symbolDropMode = value; }
        }
        
    }

    public struct ConnectorChangedEventArgs
    {
        private IInternalNodePort _port;
        private IInternalGroup _group;
        private IInternalNode _node;
        private Point _point;
        private bool _isSource;

        private readonly ConnectorChangedAction _change;
        private DragState _mDragState;

        internal IInternalNode InternalNode
        {
            get { return _node; }
            set
            {
                if (_node != value)
                {
                    var old = this;
                    _node = value;
                    FireChange(ref old);
                }
            }
        }

        internal IInternalGroup InternalGroup
        {
            get { return _group; }
            set
            {
                if (_group != value)
                {
                    var old = this;
                    _group = value;
                    FireChange(ref old);
                }
            }
        }

        internal bool IsSource
        {
            get { return _isSource; }
            set { _isSource = value; }
        }

        internal IInternalNodePort InternalPort
        {
            get { return _port; }
            set
            {
                if (_port != value)
                {
                    var old = this;
                    _port = value;
                    FireChange(ref old);
                }
            }
        }
        
        public Point Point
        {
            get { return _point; }
            internal set
            {
                if (_point != value)
                {
                    var old = this;
                    _point = value;
                    FireChange(ref old);
                }
            }
        }

        public object Node
        {
            get
            {
                if (_node != null)
                    return _node.Source;
                return null;
            }
        }

        public object Group
        {
            get
            {
                if (_group != null)
                    return _group.Source;
                return null;
            }
        }

        public IPort Port
        {
            get
            {
                if (_port != null)
                    return _port.Source as IPort;
                return null;
            }
        }

        public DragState DragState
        {
            get { return _mDragState; }
            internal set
            {
                if (_mDragState != value)
                {
                    var old = this;
                    _mDragState = value;
                    FireChange(ref old);
                }
            }
        }
        
        private void FireChange(ref ConnectorChangedEventArgs old)
        {
            _change(ref old, ref this);
        }

        internal ConnectorChangedEventArgs(ConnectorChangedAction notify,
                                        bool isSource,
                                        Point? point = null,
                                        IInternalNode node = null,
                                        IInternalGroup group = null, 
                                        IInternalNodePort port = null)
        {
            _point = point ?? new Point(0,0);
            _node = node;
            _group = group;
            _port = port;
            _isSource = isSource;
            _change = notify;
            _mDragState = DragState.None;
        }

        public static bool operator ==(ConnectorChangedEventArgs d1, ConnectorChangedEventArgs d2)
        {
            return
                d1._point == d2._point &&
                d1._node == d2._node &&
                d1._group == d2._group &&
                d1._port == d2._port;
        }

        public static bool operator !=(ConnectorChangedEventArgs d1, ConnectorChangedEventArgs d2)
        {
            return
                d1._point != d2._point ||
                d1._node != d2._node ||
                d1._group != d2._group ||
                d1._port != d2._port;
        }

        public override bool Equals(object obj)
        {
            if (obj is ConnectorChangedEventArgs)
            {
                return this == (ConnectorChangedEventArgs) obj;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
   
    public enum DragState
    {
        None,
        Starting,
        Started,
        Dragging,
        Completed,
        //Canceled
    }
   
    internal delegate void ConnectorChangedAction(ref ConnectorChangedEventArgs arg1, ref ConnectorChangedEventArgs arg2);
    internal delegate void NodeChangedAction(ref NodeChangedEventArgs arg1, ref NodeChangedEventArgs arg2);
    public struct NodeChangedEventArgs
    {
        private double _offsetX;
        private double _offsetY;
        private double _rotateAngle;
        private double _width;
        private double _height;
        private Point _pivot;
        private double _scaleX;
        private double _scaleY;
        private NodeChangedAction _action;

        public double OffsetX
        {
            get { return _offsetX; }
            internal set
            {
                if (_offsetX != value)
                {
                    var old = this;
                    _offsetX = value;
                    Fire(ref old);
                }
            }
        }

        public double OffsetY
        {
            get { return _offsetY; }
            internal set
            {
                if (_offsetY != value)
                {
                    var old = this;
                    _offsetY = value;
                    Fire(ref old);
                }
            }
        }

        public double RotateAngle
        {
            get { return _rotateAngle; }
            internal set
            {
                if (_rotateAngle != value)
                {
                    var old = this;
                    _rotateAngle = value;
                    Fire(ref old);
                }
            }
        }

        public double Width
        {
            get { return _width; }
            internal set
            {
                if (_width != value)
                {
                    var old = this;
                    _width = value;
                    Fire(ref old);
                }
            }
        }

        public double Height
        {
            get { return _height; }
            internal set
            {
                if (_height != value)
                {
                    var old = this;
                    _height = value;
                    Fire(ref old);
                }
            }
        }

        public Point Pivot
        {
            get { return _pivot; }
            internal set
            {
                if (_pivot != value)
                {
                    var old = this;
                    _pivot = value;
                    Fire(ref old);
                }
            }
        }


        public double ScaleX
        {
            get { return _scaleX; }
            internal set
            {
                if (_scaleX != value)
                {
                    var old = this;
                    _scaleX = value;
                    Fire(ref old);
                }
            }
        }


        public double ScaleY
        {
            get { return _scaleY; }
            internal set
            {
                if (_scaleY != value)
                {
                    var old = this;
                    _scaleY = value;
                    Fire(ref old);
                }
            }
        }

        internal NodeChangedEventArgs(
            NodeChangedAction action,
            //object item, 
            double x = 0, double y = 0,
            double angle = 0,
            double width = 0, double height = 0,
            Point? pivot = null,
            double scaleX = 1,
            double scaleY = 1)
        {
            _action = action;
            //_item = item;
            _offsetX = x;
            _offsetY = y;
            _rotateAngle = angle;
            _width = width;
            _height = height;
            _pivot = pivot ?? new Point(0.5, 0.5);
            _scaleX = scaleX;
            _scaleY = scaleY;
        }

        private void Fire(ref NodeChangedEventArgs oldValue)
        {
            _action.Invoke(ref oldValue, ref this);
        }
    }

    public struct ScrollChanged
    {
        private Rect _mPageBounds;
        public Rect PageBounds
        {
            get
            {
                return _mPageBounds;
            }
            internal set
            {
                _mPageBounds = value;
            }
        }


        private Rect _mViewPort;
        public Rect ViewPort
        {
            get
            {
                return _mViewPort;
            }
            internal set
            {
                _mViewPort = value;
            }
        }

        private Rect _mContentBounds;

        public Rect ContentBounds
        {
            get { return _mContentBounds; }
            internal set { _mContentBounds = value; }
        }

        private double _mCurrentZoom;

        public double CurrentZoom
        {
            get { return _mCurrentZoom; }
            internal set { _mCurrentZoom = value; }
        }

        private double _mMinZoom;

        public double MinZoom
        {
            get { return _mMinZoom; }
            internal set { _mMinZoom = value; }
        }

        private double _mMaxZoom;

        public double MaxZoom
        {
            get { return _mMaxZoom; }
            internal set { _mMaxZoom = value; }
        }

        private double _mZoomFactor;

        public double ZoomFactor
        {
            get { return _mZoomFactor; }
            internal set { _mZoomFactor = value; }
        }

        private double? _mScrollFactor;

        public double? ScrollFactor
        {
            get { return _mScrollFactor; }
            internal set { _mScrollFactor = value; }
        }

        internal ScrollChanged(Rect viewport, Rect pageBounds, Rect contentBounds, double currentZoom, double minZoom, double maxZoom, double zoomFactor, double? scrollFactor)
        {
            _mViewPort = viewport;
            _mPageBounds = pageBounds;
            _mContentBounds = contentBounds;
            _mCurrentZoom = currentZoom;
            _mMinZoom = minZoom;
            _mMaxZoom = maxZoom;
            _mZoomFactor = zoomFactor;
            _mScrollFactor = scrollFactor;
        }
    }
    //public class TransformStartingEventArgs
    //{
    //    public TransformEventArgs InitialValue { get; private set; }

    //    public TransformStartingEventArgs(TransformEventArgs init)
    //    {
    //        InitialValue = init;
    //    }
    //}
    //public class TransformStartedEventArgs
    //{
    //    public TransformEventArgs CumulativeDelta { get; private set; }

    //    public TransformStartedEventArgs(TransformEventArgs cum)
    //    {
    //        CumulativeDelta = cum;
    //    }
    //}
    //public class TransformDeltaEventArgs
    //{
    //    public TransformEventArgs CumulativeDelta { get; private set; }
    //    public TransformEventArgs Delta { get; private set; }

    //    public TransformDeltaEventArgs(TransformEventArgs cum, TransformEventArgs delta)
    //    {
    //        CumulativeDelta = cum;
    //        Delta = delta;
    //    }
    //}
    //public class TransformCompletedEventArgs
    //{
    //    public TransformEventArgs CumulativeDelta { get; private set; }

    //    public TransformCompletedEventArgs(TransformEventArgs cum)
    //    {
    //        CumulativeDelta = cum;
    //    }
    //}
    //public class TransformCanceledEventArgs
    //{
    //    public TransformEventArgs InitialValue { get; private set; }

    //    public TransformCanceledEventArgs(TransformEventArgs init)
    //    {
    //        InitialValue = init;
    //    }
    //}

    public delegate void SelectedEventHandler(object sender, DiagramEventArgs args);
    public delegate void UnSelectedEventHandler(object sender, DiagramEventArgs args);
    public delegate void ItemTappedEventHandler(object sender, DiagramEventArgs args);
    public delegate void ItemDoubleTappedEventHandler(object sender, DiagramEventArgs args);
    public delegate void ViewPortChangedEventHandler(object sender, ChangeEventArgs<object,ScrollChanged> args);
    public delegate void SymbolDroppingEventHandler(object sender,SymbolDroppingEventArgs args);
    //public delegate void TransformStartingEventHandler(object sender, TransformStartingEventArgs args);
    //public delegate void TransformStartedEventHandler(object sender, TransformStartedEventArgs args);
    //public delegate void TransformDeltaEventHandler(object sender, TransformDeltaEventArgs args);
    //public delegate void TransformCompletedEventHandler(object sender, TransformCompletedEventArgs args);
    //public delegate void TransformCanceledEventHandler(object sender, TransformCanceledEventArgs args); 
    public delegate void ItemAddedEventHandler(object sender, ItemAddedEventArgs args);
    public delegate void ItemDeletedEventHandler(object sender, DiagramEventArgs args);
    public delegate void NodeChangedEventHandler(object sender, ChangeEventArgs<object, NodeChangedEventArgs> args);
    public delegate void ConnectorSourceChangedEventHandler(object sender, ChangeEventArgs<object, ConnectorChangedEventArgs> args);
    public delegate void ConnectorTargetChangedEventHandler(object sender, ChangeEventArgs<object, ConnectorChangedEventArgs> args);


    public delegate void SelectingEventHandler(object sender,DiagramPreviewEventArgs args);
    public delegate void UnSelectingEventHandler(object sender,DiagramPreviewEventArgs args);
    public delegate void ItemDeletingEventHandler(object sender,DiagramPreviewEventArgs args);   

}
