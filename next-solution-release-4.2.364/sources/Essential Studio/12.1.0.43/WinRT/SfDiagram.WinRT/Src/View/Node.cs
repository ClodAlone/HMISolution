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
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#else
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
#endif
using System.Reflection;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Panels;

namespace Syncfusion.UI.Xaml.Diagram
{
#if !WINRT
    [DesignTimeVisible(false)]
#endif
    public partial class Node :
        ContentControl,
        INode,
        IDiagramElement,
        INodeView,
        ISharedData
    {
        internal SharedData _mSharedData;
        internal AnnotationPanel AnnotationHost { get; private set; }

        //public Node()
        //{

        //    //Binding bind = new Binding();
        //    //bind.Path = new PropertyPath("NodeTransform");
        //    //bind.RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self };
        //    //this.SetBinding(RenderTransformProperty, bind);

        //    //bind = new Binding();
        //    //bind.Path = new PropertyPath("Pivot");
        //    //bind.RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self };            
        //    //this.SetBinding(RenderTransformOriginProperty , bind);

        //}

        internal CompositeTransform ArrangeTransform;
        private NodeDragStartingEvent _mDragStarting;
        private NodeDragEvent _mDragEvent;

#if WPF
        static Node()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Node), new FrameworkPropertyMetadata(typeof(Node)));
        }
#endif

        public Node()
        {
#if !WPF
            this.DefaultStyleKey = typeof(Node);
#endif
            this.SizeChanged += Node_SizeChanged;
#if TOUCH
            this.ManipulationMode = ManipulationModes.None;
#endif

            ArrangeTransform = new CompositeTransform();
#if WPF
            this.RenderTransform = ArrangeTransform.Transform;
#else
            this.RenderTransform = ArrangeTransform;
#endif

            Binding bind = new Binding();
            bind = new Binding();
            bind.Path = new PropertyPath("Pivot");
            bind.RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self };
            this.SetBinding(RenderTransformOriginProperty, bind);

#if WINRT
            this.Tapped += (s, e) => Wrapper.Tap(true);
            this.DoubleTapped += (s, e) => Wrapper.DoubleTap(true);
            this.PointerPressed += Node_PointerPressed;
            this.PointerReleased += Node_PointerReleased;
#elif WPF
            //this.Click += (s, e) => Wrapper.Tap(true);
            this.MouseDoubleClick += (s, e) => Wrapper.DoubleTap(true);
            this.MouseLeftButtonDown += Node_PointerPressed;
            this.MouseMove += Node_PointerMoved;
            this.MouseLeftButtonUp += Node_PointerReleased;
#elif SILVERLIGHT
            this.Tap += (s, e) => Wrapper.Tap(true);
            this.DoubleTap += (s, e) => Wrapper.DoubleTap(true);
            this.MouseLeftButtonDown += Node_PointerPressed;
            this.MouseMove += Node_PointerMoved;
            this.MouseLeftButtonUp += Node_PointerReleased;
#endif
            ID = Guid.NewGuid();
            //if (!(this is Selector))
            //{
            //    _mDragProvider = new DragProvider(this);
            //}

            this.Loaded += Node_Loaded;
        }

        void Node_PointerReleased(object sender, MouseButtonEventArgs e)
        {
            if (_mSharedData.UndoRedoController != null)
                _mSharedData.UndoRedoController.EndComposite();
            _mInitialLocation = null;
            _mSharedData.Adorner.ClearGuidelines();
#if WINRT
            this.PointerMoved -= Node_PointerMoved;
#else
            this.MouseMove -= Node_PointerMoved;
#endif
            this.ReleasePointerCapture(e);
        }

        Point? _mInitialLocation;

        internal void SetInitialLocation(Point p)
        {
            _mInitialLocation = p;
            _mDragStarting.Publish(null);
        }

        bool _isDragStarted = false;

        void Node_PointerMoved(object sender, MouseEventArgs e)
        {
            if (!(this is Selector) && !(Constraints.Contains(NodeConstraints.AllowPan))
                && this.CapturePointer(e))
            {
                if (_mInitialLocation == null)
                {
                    _mInitialLocation = e.GetCurrentPoint(_mSharedData.Page).Position;
                    _mDragStarting.Publish(e);
                    _isDragStarted = true;
                }

                Point pt = e.GetCurrentPoint(_mSharedData.Page).Position;
                if (_isDragStarted && _mInitialLocation.Value != pt)
                {
                    if (_mSharedData.Graph.Tool.Contains(Tool.DrawOnce) || _mSharedData.Graph.Tool.Contains(Tool.ContinuesDraw))
                    {
                        this.ReleasePointerCapture(e);
                        return;
                    }
                    Wrapper.PointerDragStarted();
                    _mDragStarting.Publish(e);
                    _isDragStarted = false;

                }
                _mDragEvent.Publish(new NodeDragDeltaEventArgs(this,
                    new ManipulationDragDelta(pt.X - _mInitialLocation.Value.X, pt.Y - _mInitialLocation.Value.Y, 0, 0)));
            }
        }

        void Node_PointerPressed(object sender, MouseButtonEventArgs e)
        {
            if (!(Constraints.Contains(NodeConstraints.AllowPan)) && !(this is Selector)
               && this.CapturePointer(e))
            {
                if (_mSharedData.UndoRedoController != null)
                    _mSharedData.UndoRedoController.BeginComposite();
                _isDragStarted = true;
                _mDragStarting.Publish(e);
#if WINRT
                this.PointerMoved += Node_PointerMoved;
#else
                this.MouseMove += Node_PointerMoved;
#endif
            }
        }

        internal void Manual_PointerPressed(MouseEventArgs e)
        {
#if WINRT
                this.PointerMoved += Node_PointerMoved;
#else
            this.MouseMove += Node_PointerMoved;
#endif
        }

        private void CleanPortAnnot(IProtectedNode wrapper)
        {
            if (wrapper.InternalPorts != null)
            {
                foreach (var internalPort in wrapper.InternalPorts)
                {
                    _mSharedData.Adorner.Children.Remove(internalPort.View);
                }
            }
            if (AnnotationHost != null)
            {
                if (wrapper.InternalAnnotations != null)
                {
                    foreach (var internalAnnot in wrapper.InternalAnnotations)
                    {
                        AnnotationHost.Children.Remove(internalAnnot.View);
                    }
                }
            }
        }

        void Node_Loaded(object sender, RoutedEventArgs e)
        {
            if (Wrapper != null)
            {
                if (Wrapper.InternalPorts != null)
                {
                    foreach (var internalPort in Wrapper.InternalPorts)
                    {
                        if (!_mSharedData.Adorner.Children.Contains(internalPort.View))
                        {
                            _mSharedData.Adorner.Children.Add(internalPort.View);
                        }
                    }
                }
            }

            if (AnnotationHost != null)
            {
                if (Wrapper.InternalAnnotations != null)
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
        }


        void ISharedData.Init(SharedData shared)
        {
            if (_mSharedData == null)
            {
                _mSharedData = shared;
                if (!(this is Selector))
                {
                    _mDragStarting = _mSharedData.EventAggregator.GetEvent<NodeDragStartingEvent>();
                    _mDragEvent = _mSharedData.EventAggregator.GetEvent<NodeDragEvent>();
                }
            }
        }

        private bool _mMeasureing = false;
        //private bool _mArranging = false;

        internal void ForceInvalidateMeasure()
        {
            if (!_mMeasureing)
            {
                InvalidateMeasure();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _mMeasureing = true;
            Size desiredSize = base.MeasureOverride(availableSize);
            if (AnnotationHost != null)
            {
                Size size = desiredSize.Max(AnnotationHost._mDesiredSize);
                if (double.IsNaN(Width))
                {
                    desiredSize.Width = size.Width;
                }
                if (double.IsNaN(Height))
                {
                    desiredSize.Height = size.Height;
                }
            }
            if (Wrapper.DesiredSize != desiredSize)
            {
                if (Wrapper.KnownParentGroup != null)
                {
                    (Wrapper.KnownParentGroup as IProtectedNode).Invalidate();
                }
            }
            Wrapper.SetDesiredSize(desiredSize);
            Wrapper.UpdateCompositeTrans();
            _mMeasureing = false;
            return desiredSize;
            //return new Size(0,0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size actualSize = base.ArrangeOverride(finalSize);
            if (AnnotationHost != null)
            {
                Size size = actualSize.Max(AnnotationHost._mActualSize);
                if (double.IsNaN(Width))
                {
                    actualSize.Width = size.Width;
                }
                if (double.IsNaN(Height))
                {
                    actualSize.Height = size.Height;
                }
            }
            //Size actualSize = new Size(0,0);
            //if (Wrapper.DesiredSize == new Size(0, 0))
            //{
            //    actualSize = base.ArrangeOverride(finalSize);
            //}
            //else
            //{
            //    actualSize = base.ArrangeOverride(Wrapper.DesiredSize);
            //}
            Wrapper.SetActualSize(actualSize);
            Wrapper.UpdateCompositeTrans();
            //if (Wrapper.InternalPorts != null)
            //{
            //    foreach (var nodePort in Wrapper.InternalPorts)
            //    {
            //        if (nodePort.View != null)
            //        {
            //            nodePort.View.Arrange();
            //        }
            //    }
            //}
            return actualSize;
            //return new Size(0, 0);
        }

        private IProtectedNode _mWrapper;

        internal IProtectedNode Wrapper
        {
            get
            {
                return _mWrapper;
            }
        }

        internal virtual void SetWrapper(IProtectedNode value)
        {
            if (value == null && _mWrapper != null)
            {
                CleanPortAnnot(_mWrapper);
            }
            _mWrapper = value;
            if (_mWrapper != null)
            {
                Width = _mWrapper.UnitWidth;
                Height = _mWrapper.UnitHeight;
            }
        }


        internal CompositeTransform ContentTransform = null;

        private void DoApplyTemplate()
        {
            AnnotationHost = GetTemplateChild("PART_Annotations") as AnnotationPanel;
            if (!(this is Selector))
            {
                ContentTransform = new CompositeTransform();
                ContentPresenter content = GetTemplateChild("ContentPresenter") as ContentPresenter;
                Path path = GetTemplateChild("Path") as Path;
#if WPF
                if (content != null)
                {
                    content.RenderTransform = ContentTransform.Transform;
                }
                if (path != null)
                {
                    path.RenderTransform = ContentTransform.Transform;
                }
#else
                if (content != null)
                {
                    content.RenderTransform = ContentTransform;
                }
                if (path != null)
                {
                    path.RenderTransform = ContentTransform;
                }
#endif
            }
        }

        private void OnOffsetXChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.OffsetX);
            InvalidateArrange();
        }
        private void OnOffsetYChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.OffsetY);
            InvalidateArrange();
        }
        private void OnUnitWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_mSharedData != null)
            {
                Width = _mSharedData.Unit.ToPixel(UnitWidth);
            }
            OnPropertyChanged(NodeConstants.UnitWidth);
        }

        private void OnUnitHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_mSharedData != null)
            {
                Height = _mSharedData.Unit.ToPixel(UnitHeight);
            }
            OnPropertyChanged(NodeConstants.UnitHeight);
        }

        private void OnRotateAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.RotateAngle);
            InvalidateArrange();
        }

        private void OnPivotChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.Pivot);
        }
        private void OnAutoBindChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.AutoBind);
        }
        private void OnIDChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.ID);
        }
        private void OnKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.Key);
        }
        private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.IsSelected);
        }

        private void OnParentGroupChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.ParentGroup);
        }

        private void OnPortsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.Ports);
        }

        private void OnAnnotationsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.Annotations);
        }

        private void OnZIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            int newValue = (int)e.NewValue;
            Canvas.SetZIndex(this, newValue);
            OnPropertyChanged(NodeConstants.ZIndex);
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Bounds

        public Rect Bounds
        {
            get { return (Rect)GetValue(BoundsProperty); }
        }

        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.Register("Bounds", typeof(Rect), typeof(Node), new PropertyMetadata(Rect.Empty, OnBoundsChanged));

        private static void OnBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnBoundsChanged(e);
        }

        #endregion

        void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Wrapper != null
                && Wrapper.KnownParentGroup != null
                && Wrapper.KnownParentGroup.View != null)
            {
                (Wrapper.KnownParentGroup.View as UIElement).InvalidateMeasure();
            }

            if (IsSelected)
            {
                (_mSharedData.Graph.InternalSelectedItems.View as UIElement).InvalidateMeasure();
            }
        }

        //private void OnOffsetXChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    //this.NodeTransform.TranslateX = (double)e.NewValue;
        //    //this.GetType().GetRuntimeProperty("OffsetX").GetCustomAttributes(true);

        //    //this.OffsetY = this.OffsetX;
        //    //object o = this.DataContext;
        //    //if (o != null)
        //    //{
        //    //    PropertyInfo info = o.GetType().GetRuntimeProperty("Y");
        //    //    if (info != null)
        //    //    {
        //    //        info.SetValue(o, this.OffsetX);
        //    //    }
        //    //}
        //    OnPropertyChanged(NodeConstants.OffsetX);
        //}

        //private void OnOffsetYChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    //this.NodeTransform.TranslateY = (double)e.NewValue;
        //    OnPropertyChanged(NodeConstants.OffsetY);
        //}
        //private void OnRotateAngleChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    double oldValue = (double)e.OldValue;
        //    double newValue = (double)e.NewValue;
        //}
        //private void OnMinWidthChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    double oldValue = (double)e.OldValue;
        //    double newValue = (double)e.NewValue;
        //}
        //private void OnMaxWidthChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    double oldValue = (double)e.OldValue;
        //    double newValue = (double)e.NewValue;
        //}
        //private void OnWidthChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    double oldValue = (double)e.OldValue;
        //    double newValue = (double)e.NewValue;
        //}
        //private void OnMinHeightChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    double oldValue = (double)e.OldValue;
        //    double newValue = (double)e.NewValue;
        //}
        //private void OnMaxHeightChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    double oldValue = (double)e.OldValue;
        //    double newValue = (double)e.NewValue;
        //}
        //private void OnHeightChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    double oldValue = (double)e.OldValue;
        //    double newValue = (double)e.NewValue;
        //}
        //private void OnMatrixChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    MatrixExt oldValue = (MatrixExt)e.OldValue;
        //    MatrixExt newValue = (MatrixExt)e.NewValue;
        //}
        private void OnShapeChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.Shape);
        }
        private void OnShapeStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.ShapeStyle);
        }

        private void OnBoundsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Wrapper != null && Wrapper.InternalPorts != null)
            {
                foreach (var nodePort in Wrapper.InternalPorts)
                {
                    if (nodePort.View != null)
                    {
                        nodePort.View.InvalidateArrange();
                    }
                }
            }
        }

        private void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(NodeConstants.IsExpanded);
        }

        private void BindToINode(INode source)
        {
            if (source != null)
            {
                Binding bind;
                foreach (var property in NodeConstants.NodeProperties)
                {
                    bind = new Binding();
                    bind.Path = new PropertyPath(property.Item2);
                    bind.Mode = BindingMode.TwoWay;
                    //bind.Source = source;
                    if (ReadLocalValue(property.Item1) == DependencyProperty.UnsetValue)
                    {
                        this.SetBinding(property.Item1, bind);
                    }
                }
            }
        }

        private object BusinessObject { get; set; }

        void IView.SetBusinessObject(object item)
        {
            this.BusinessObject = item;
            if (AutoBind && BusinessObject is INode)
            {
                BindToINode(BusinessObject as INode);
            }
        }

        public bool IsConnecting { get; set; }

        private void OnSnapToObjectChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void OnFlipChanged(DependencyPropertyChangedEventArgs e)
        {
            //if ((Flip)e.NewValue != Flip.None)
                OnPropertyChanged(NodeConstants.Flip);
                InvalidateArrange();
        }

        private void OnConstraintsChanged(DependencyPropertyChangedEventArgs e)
        {
# if WINRT
            if (Constraints.Contains(NodeConstraints.AllowPan))
            {
                this.ManipulationMode =
      ManipulationModes.TranslateX |
      ManipulationModes.TranslateY |
      ManipulationModes.Scale |
      ManipulationModes.Rotate;
            }
            else
            {
                this.ManipulationMode = ManipulationModes.None;
            }
            //if (Constraints.Contains(NodeConstraints.Resizable))
            //{
            //    this.ManipulationMode |= ManipulationModes.Scale;
            //}
            //else
            //{
            //    this.ManipulationMode &= ~ManipulationModes.Scale;
            //}


            //if (Constraints.Contains(NodeConstraints.Rotatable))
            //{
            //    this.ManipulationMode |= ManipulationModes.Rotate;
            //}
            //else
            //{
            //    this.ManipulationMode &= ~ManipulationModes.Rotate;
            //}

            //if (Constraints.Contains(NodeConstraints.Draggable))
            //{
            //    this.ManipulationMode |= (ManipulationModes.TranslateX | ManipulationModes.TranslateY);
            //}
            //else
            //{
            //    this.ManipulationMode &= ~(ManipulationModes.TranslateX | ManipulationModes.TranslateY);
            //}

            //if (!ManipulationMode.Contains(ManipulationModes.TranslateX | ManipulationModes.TranslateY |
            //                                 ManipulationModes.Scale | ManipulationModes.Rotate)
            //    && Constraints.Contains(NodeConstraints.AllowPan))
            //{
            //    ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY |
            //                       ManipulationModes.Rotate | ManipulationModes.Scale;
            //}
# endif
        }

        /// <summary>
        /// Invoked when target objects have been found to snap
        /// </summary>
        /// <param name="snapParameters">Collection of SnapParameters.</param>
        /// <param name="snapAccepted">Whether the object can be snapped or not</param>
        public virtual void OnSnap(List<SnapParameter> snapParameters, out bool snapAccepted)
        {
            snapAccepted = true;
        }

        public object Info { get; set; }

        void IDisposable.Dispose()
        {
        }
    }
}
