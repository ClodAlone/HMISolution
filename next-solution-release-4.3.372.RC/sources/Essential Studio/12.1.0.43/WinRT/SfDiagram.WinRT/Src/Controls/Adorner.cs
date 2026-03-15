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
        #region Dependency Properties
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(Adorner), new PropertyMetadata(1d, CurrentZoomChanged));


        private static void CurrentZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Adorner).UpdateSelectionPreviews();
            (d as Adorner).InvalidateMeasure();
            (d as Adorner)._scaleChanged = true;
        }

        #endregion

        bool _scaleChanged = false;
        TranslateTransform AdornerTransform;
        SharedData _mSharedData;
        //IInternalSelector _mWrapper;

        #region Adorner

        public Adorner()
        {
            _mConnectorSelectionBin = new Stack<Path>();
            _mNodeSelectionBin = new Stack<Rectangle>();
            InitializeConnectionEditor();
            InitializeConnectionIndicator();
            _mGuidelines = new List<UIElement>();
            _mSelectionIndicatorBrush = new SolidColorBrush(new Color() { A = 255, R = 0, G = 93, B = 145 });
            this.RenderTransform = AdornerTransform = new TranslateTransform();
        }

        public void UpdateAdornerTransform()
        {
            AdornerTransform.X = _mSharedData.ScrollViewer.ZoomPanTransform.TranslateX;
            AdornerTransform.Y = _mSharedData.ScrollViewer.ZoomPanTransform.TranslateY;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                if (child is Selector && _scaleChanged)
                {
                    child.InvalidateMeasure();
                }
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_mSelectedConnector != null)
            {
                _mSelectedConnector.UpdateThums();
                UpdateEndPointion();
            }
            foreach (UIElement child in Children)
            {
                Size desired = child.DesiredSize;
                if (child is INodePort && _scaleChanged)
                {
                    (child as NodePort).InvalidateArrange();
                }
                else if (child is DiagramThumb)
                {
                    DiagramThumb thumb = child as DiagramThumb;
                    (thumb.RenderTransform as TranslateTransform).X = thumb.OffsetX * Scale - thumb.DesiredSize.Width / 2;
                    (thumb.RenderTransform as TranslateTransform).Y = thumb.OffsetY * Scale - thumb.DesiredSize.Height / 2;
                }
                else if (child is Line)
                {
                    Line line = child as Line;
                    desired.Width=desired.Width > 0 ? desired.Width : Math.Abs(line.X2 - line.X1);
                    desired.Height = desired.Height > 0 ? desired.Height : Math.Abs(line.Y2 - line.Y1);
                }
                child.Arrange(new Rect(0, 0, desired.Width, desired.Height));
            }
            if (_scaleChanged)
                _scaleChanged = false;
            return new Size(0, 0);
        }

        public void Init(SharedData shared)
        {
            _mSharedData = shared;
            _mSharedData.Selected.Subscribe(Selected);
            _mSharedData.UnSelected.Subscribe(UnSelected);

        }

        public void Dispose()
        {
        }

        internal void PrepareAdorner()
        {
            Binding zoom = new Binding()
            {
                Path = new PropertyPath("CurrentZoom"),
                Source = _mSharedData.ScrollViewer
            };

            SetBinding(ScaleProperty, zoom);
            //_mWrapper = _mSharedData.Graph.InternalSelectedItems;
        }

        #endregion

    }

    public class RunTimeConnectionIndicator : ContentControl
    {
        internal RunTimeConnectionIndicator()
        {
            this.DefaultStyleKey = typeof(RunTimeConnectionIndicator);
        }

        private void GotoState(string state)
        {
            VisualStateManager.GoToState(this, state, true);
        }
        private NodePort _dragOverPort;
        private Node _dragOverNode;
        public NodePort DragOverPort
        {
            get { return _dragOverPort; }
            internal set
            {
                if (_dragOverPort != null)
                {
                    _dragOverPort.IsConnecting = false;
                }
                _dragOverPort = value;

                if (_dragOverPort != null)
                {
                    _dragOverPort.IsConnecting = true;
                    GotoState("Connecting");
                }
                else if (_dragOverNode == null)
                {
                    GotoState("Normal");
                }
            }
        }

        public Node DragOverNode
        {
            get { return _dragOverNode; }
            internal set
            {
                if (_dragOverNode != null)
                {
                    _dragOverNode.IsConnecting = false;
                }

                _dragOverNode = value;

                if (_dragOverNode != null)
                {
                    _dragOverNode.IsConnecting = true;
                    GotoState("Connecting");
                }
                else if (_dragOverPort == null)
                {
                    GotoState("Normal");
                }
            }
        }
    }
}
