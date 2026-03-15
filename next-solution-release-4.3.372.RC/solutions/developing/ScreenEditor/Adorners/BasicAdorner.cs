using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Utilities.WPF;
using Utilities.Animations;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using ScreenManager.SpecialObjects;
using _3DTools;
using Utilities;
using System.Collections.Generic;
using ScreenManager.Popups;
using WPFUtilities;
using Pipeline;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;

namespace ScreenManager.Adorners
{
    class BasicAdorner : Adorner
    {
        public static BasicAdorner CreateAdorner(UIElement parent, UIElement uie, 
                bool canMove, IGridViewInfoService GridViewInfoService, bool bMultiSelected = false)
        {
            if (uie is Line)
                return new LineAdorner(parent, uie as Line, canMove, GridViewInfoService);
            else if (uie is Polygon)
                return new PolygonAdorner(parent, uie as Polygon, canMove, GridViewInfoService);
            else if (uie is Polyline)
                return new PolylineAdorner(parent, uie as Polyline, canMove, GridViewInfoService);
            else if (uie is Pipeline.Pipeline)
                return new PipelineAdorner(parent, uie as Pipeline.Pipeline, canMove, GridViewInfoService);
            else if (uie is PolyBezier.PolyBezier)
                return new PolyBezierAdorner(parent, uie as PolyBezier.PolyBezier, canMove, GridViewInfoService);

            if (bMultiSelected)
                return new BasicAdorner(parent, uie, canMove, GridViewInfoService, true);

            return new GenericElementAdorner(parent, uie, canMove, GridViewInfoService);
        }

        #region Declarations

        protected VisualCollection visualChildren;
        protected IGridViewInfoService GridViewInfoService;
        protected bool bEnableFireChanging = true;

        protected EditingAdornerControl editControl;
        protected DragThumb dragControl;
        protected DragThumbSelected dragControlSelected;
        protected DragThumbMultiSelected dragControlMultiSelected;
        protected UIElement adornedElement;
        protected bool bCanMove;
        protected bool isMultiSelection;
        #endregion

        #region Constructors

        static ThumbsResources tr = new ThumbsResources();
        static BasicAdorner()
        {
            tr.InitializeComponent();
        }

        protected BasicAdorner(UIElement parent, UIElement element, bool canMove,
            IGridViewInfoService gridViewInfoService, bool multiSelection = false)
            : base(parent)
        {
            // Focusable = false;
            isMultiSelection = multiSelection;
            adornedElement = element;
            bCanMove = canMove;
            GridViewInfoService = gridViewInfoService;

            dragControl = new DragThumb(parent, GridViewInfoService, this is BasicPointAdorner);
            dragControl.VerticalAlignment = VerticalAlignment.Stretch;
            dragControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            dragControl.Cursor = Cursors.SizeAll;

            if (!isMultiSelection)
            {
                // editControl = new EditingAdornerControl(element);
                // editControl.Focusable = false;
                // editControl.DataContext = element;

                dragControlSelected = new DragThumbSelected();
                dragControlSelected.Visibility = Visibility.Hidden;
                dragControlSelected.VerticalAlignment = VerticalAlignment.Stretch;
                dragControlSelected.HorizontalAlignment = HorizontalAlignment.Stretch;
                dragControlSelected.Cursor = Cursors.Cross;

                // editControl.HorizontalAlignment = HorizontalAlignment.Right;
                // editControl.VerticalAlignment = VerticalAlignment.Center;
            }

            dragControlMultiSelected = new DragThumbMultiSelected();
            dragControlMultiSelected.Visibility = Visibility.Hidden;
            dragControlMultiSelected.VerticalAlignment = VerticalAlignment.Stretch;
            dragControlMultiSelected.HorizontalAlignment = HorizontalAlignment.Stretch;
            dragControlMultiSelected.Cursor = Cursors.Cross;

            Resources = tr;
            visualChildren = new VisualCollection(this);

            ClipToBounds = false;
            
            dragControl.DataContext = element;
            if (isMultiSelection)
                Activate();
        }

        #endregion

        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

        internal void SetLockMovement(bool bSet)
        {
            for(int i = 0; i < VisualChildrenCount; ++i)
            {
                var el = GetVisualChild(i) as UIElement;
                if (el == null || el == editControl)
                    continue;
                el.IsHitTestVisible = bSet ? false : true;
            }
        }

        internal bool SetCacheMode(bool bSet)
        {
            if (adornedElement.CacheMode == null && bSet)
                adornedElement.CacheMode = new BitmapCache() { EnableClearType = true };
            else if (adornedElement.CacheMode != null && !bSet)
                adornedElement.CacheMode = null;

            return adornedElement.CacheMode != null;
        }

        protected bool HasPopupOpened()
        {
            if (editControl != null && (editControl.popup.IsOpen || editControl.popupSmart.IsOpen))
                return true;
            return false;
        }

        protected double ElementWidth
        {
            get
            {
                if (FindParentCanvas(adornedElement).Children.Contains(adornedElement))
                {
                    double dWidth = adornedElement.DesiredSize.Width;
                    if (dWidth == 0.0)
                    {
                        FrameworkElement element = adornedElement as FrameworkElement;
                        if (element != null)
                            dWidth = element.ActualWidth;
                    }

                    return dWidth;
                }
                else
                {
                    var boundrect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { adornedElement },
                                                                    FindParentCanvas(adornedElement));
                    return boundrect.Width;
                }
            }
        }

        protected double ElementTop
        {
            get
            {
                if (FindParentCanvas(adornedElement).Children.Contains(adornedElement))
                {
                    double top = Canvas.GetTop(adornedElement);
                    if (Double.IsNaN(top))
                    {
                        var boundrect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { adornedElement },
                                                                        FindParentCanvas(adornedElement));
                        top = boundrect.Top;
                        if (Double.IsNaN(top) || Double.IsInfinity(top))
                            top = 0;
                    }

                    return top;
                }
                else
                {
                    var boundrect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { adornedElement },
                                                                    FindParentCanvas(adornedElement));
                    return boundrect.TopLeft.Y;
                    //var boundrect = VisualTreeHelper.GetContentBounds(adornedElement);
                    //try
                    //{
                    //    Point p = adornedElement.TransformToVisual(FindParentCanvas(adornedElement)).Transform(boundrect.TopLeft);
                    //    if (Double.IsNaN(p.Y) || Double.IsInfinity(p.Y))
                    //        p.Y = 0;
                    //    return p.Y;
                    //}
                    //catch (Exception ex)
                    //{
                    //    return boundrect.Top;
                    //}
                }
            }
        }

        protected double ElementLeft
        {
            get
            {
                if (FindParentCanvas(adornedElement).Children.Contains(adornedElement))
                {
                    double left = Canvas.GetLeft(adornedElement);
                    if (Double.IsNaN(left))
                    {
                        var boundrect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { adornedElement },
                                                                        FindParentCanvas(adornedElement));
                        left = boundrect.Left;
                        if (Double.IsNaN(left) || Double.IsInfinity(left))
                            left = 0;
                    }
                    return left;
                }
                else
                {
                    var boundrect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { adornedElement },
                                                                    FindParentCanvas(adornedElement));
                    return boundrect.TopLeft.X;
                    //var boundrect = VisualTreeHelper.GetContentBounds(adornedElement);
                    //try
                    //{
                    //    Point p = adornedElement.TransformToVisual(FindParentCanvas(adornedElement)).Transform(boundrect.TopLeft);
                    //    if (Double.IsNaN(p.X) || Double.IsInfinity(p.X))
                    //        p.X = 0;
                    //    return p.X;
                    //}
                    //catch (Exception ex)
                    //{
                    //    return boundrect.Left;
                    //}
                }
            }
        }

        protected double ElementHeight
        {
            get
            {
                if (FindParentCanvas(adornedElement).Children.Contains(adornedElement))
                {
                    double dHeight = adornedElement.DesiredSize.Height;
                    if (dHeight == 0.0)
                    {
                        FrameworkElement element = adornedElement as FrameworkElement;
                        if (element != null)
                            dHeight = element.ActualHeight;
                    }

                    return dHeight;
                }
                else
                {
                    var boundrect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { adornedElement },
                                                                    FindParentCanvas(adornedElement));
                    return boundrect.Height;
                }
            }
        }

        protected Thumb currentThumb;
        protected bool bChanged;
        protected void control_DragDelta(object sender, DragDeltaEventArgs e)
        {
            currentThumb = sender as Thumb;
            bChanged = true;

            OnFireChanging(this);
            bEnableFireChanging = false;
            InvalidateArrange();
            FindParentCanvas(adornedElement).InvalidateArrange();
            AdornerLayer.GetAdornerLayer(AdornedElement).Update();
        }

        protected void control_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender == dragControl)
                SetThumbVisibility(true);

            if (dragControlSelected != null)
                dragControlSelected.Visibility = lastDragSelectedVisibility;
            //if (editControl != null)
            //    editControl.Visibility = Visibility.Visible;

            currentThumb = null;

            InvalidateArrange();
            if (bChanged)
                OnFireChanged(this);
            bEnableFireChanging = true;
            FindParentCanvas(adornedElement).InvalidateArrange();

            AdornerLayer.GetAdornerLayer(AdornedElement).Update();
        }

        protected virtual void SetThumbVisibility(bool bVisible)
        {
        }

        protected Visibility lastDragSelectedVisibility;
        protected void control_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (sender == dragControl)
                SetThumbVisibility(false);

            if (dragControlSelected != null)
            {
                lastDragSelectedVisibility = dragControlSelected.Visibility;
                dragControlSelected.Visibility = Visibility.Hidden;
            }
            if (editControl != null)
                editControl.Visibility = Visibility.Collapsed;

            currentThumb = sender as Thumb;
            bChanged = false;

            InvalidateArrange();
            FindParentCanvas(adornedElement).InvalidateArrange();

            AdornerLayer.GetAdornerLayer(AdornedElement).Update();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var c in visualChildren)
            {
                if (c is FrameworkElement)
                    ArrangeControl(c);
            }
            return finalSize;
        }

        protected void ArrangeControl(Visual c)
        {
            if (c is FrameworkElement)
            {
                Rect ro = new Rect(0, 0, ElementWidth, ElementHeight);

                FrameworkElement control = c as FrameworkElement;
                Rect aligmentRect = new Rect
                {
                    Width = control.Width,
                    Height = control.Height,
                    Y = ro.Y,
                    X = ro.X
                };
                if (adornedElement is Shape && (Double.IsNaN(aligmentRect.Width) || Double.IsNaN(aligmentRect.Height)))
                {
                    var boundrect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { adornedElement },
                                                                    FindParentCanvas(adornedElement));
                    if (!boundrect.IsEmpty)
                    {
                        aligmentRect.Width = boundrect.Width;
                        aligmentRect.Height = boundrect.Height;
                    }
                }

                if (!(control is RotateThumb))
                {
                    Type t = adornedElement.GetType();
                    var propertyPoints = t.GetProperty("Points");

                    if (propertyPoints != null | 
                        adornedElement is Polygon ||
                        adornedElement is Polyline ||
                        adornedElement is Line)
                    {
                        var boundrect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { adornedElement },
                                                                        FindParentCanvas(adornedElement));
                        if (FindParentCanvas(adornedElement).Children.Contains(adornedElement))
                        {
                            aligmentRect.X = boundrect.Left;
                            aligmentRect.Y = boundrect.Top;
                        }
                        else
                        {
                            Point po = adornedElement.TransformToVisual(adornedElement).Transform(boundrect.TopLeft);
                            aligmentRect.X = po.X;
                            aligmentRect.Y = po.Y;
                        }
                        ro.Width = boundrect.Width;
                        ro.Height = boundrect.Height;
                    }
                    else
                    {
                        aligmentRect.X = ElementLeft;
                        aligmentRect.Y = ElementTop;
                    }
                }

                if (control is TransformOriginThumb)
                {
                    Point ptOrigin = adornedElement.RenderTransformOrigin;

                    aligmentRect.Y = ElementTop + ElementHeight * ptOrigin.Y;
                    aligmentRect.X = ElementLeft + ElementWidth * ptOrigin.X;
                }
                else
                {
                    switch (control.VerticalAlignment)
                    {
                        case VerticalAlignment.Top: aligmentRect.Y += 0;
                            break;
                        case VerticalAlignment.Bottom: aligmentRect.Y += ro.Height;
                            break;
                        case VerticalAlignment.Center: aligmentRect.Y += ro.Height / 2;
                            break;
                        case VerticalAlignment.Stretch: aligmentRect.Height = ro.Height;// *Math.Abs((AdornedElement.RenderTransform as TransformGroup).Children[0].Value.M22);
                            break;
                    }
                    switch (control.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left: aligmentRect.X += 0;
                            break;
                        case HorizontalAlignment.Right: 
                            if (control != editControl)
                                aligmentRect.X += ro.Width;
                            break;
                        case HorizontalAlignment.Center: aligmentRect.X += ro.Width / 2;
                            break;
                        case HorizontalAlignment.Stretch: aligmentRect.Width = ro.Width;// *Math.Abs((AdornedElement.RenderTransform as TransformGroup).Children[0].Value.M11);
                            break;
                    }
                }

                Point p = adornedElement.TransformToVisual(adornedElement).Transform(new Point(aligmentRect.X, aligmentRect.Y));
                if (control.RenderTransform != null)
                {
                    p.X -= control.RenderTransform.Value.OffsetX;
                    p.Y -= control.RenderTransform.Value.OffsetY;
                }

                aligmentRect.X = p.X - (double.IsNaN(control.Width) ? 0 : control.Width) / 2;
                aligmentRect.Y = p.Y - (double.IsNaN(control.Height) ? 0 : control.Height) / 2;

                control.Arrange(aligmentRect);
            }
        }

        #region Virtuals

        PropertyChangeNotifier notifier; 
        public virtual void Activate()
        {
            if (!isMultiSelection)
            {
                DependencyPropertyDescriptor propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsManipulationEnabledProperty, typeof(UIElement));
                if (notifier == null)
                {
                    notifier = new PropertyChangeNotifier(adornedElement, propDesc.Name);
                    notifier.ValueChanged += (o, e) =>
                        {
                            if (!adornedElement.IsManipulationEnabled)
                                dragControl.Visibility = Visibility.Visible;
                            else
                            {
                                dragControl.Visibility = Visibility.Collapsed;
                                adornedElement.RenderTransformOrigin = new Point(0.5, 0.5);
                            }
                        };
                }
                if (adornedElement.IsManipulationEnabled)
                {
                    dragControl.Visibility = Visibility.Collapsed;
                    adornedElement.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                dragControl.MouseDoubleClick += dragControl_MouseDoubleClick;

                visualChildren.Insert(0, dragControlSelected);
                visualChildren.Insert(1, dragControl);
            }
            else
            {
                visualChildren.Insert(0, dragControl);

                dragControl.DragStarted += control_DragStarted;
                dragControl.DragCompleted += control_DragCompleted;
                dragControl.DragDelta += control_DragDelta;
            }

            if (dragControl != null)
                dragControl.Activate();

            visualChildren.Insert(0, dragControlMultiSelected);

            if (editControl != null)
            {
                visualChildren.Add(editControl);

                editControl.btnEnableRotationThumbs.Click += (o, e) =>
                    {
                        ShowHideRotationThumbs();
                        editControl.popup.IsOpen = false;
                    };
                mapTrackballs.Clear();
                mapTrackballTranforms.Clear();
                //editControl.btnBitmapCache.Click += (o, e) =>
                //{
                //    bool bSet = adornedElement.CacheMode != null;
                //    SetCacheMode(!bSet);
                //};

                //var listModel3Ds = adornedElement.GetChildrenOfType<Viewport3D>().ToList();
                //if (listModel3Ds.Count == 0 && adornedElement is Viewport3D)
                //    listModel3Ds.Add(adornedElement as Viewport3D);
                //editControl.btn3DEdit.Visibility = listModel3Ds.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                //if (editControl.btn3DEdit.Visibility == Visibility.Visible)
                //{
                //    editControl.btn3DEdit.Click += (o, e) =>
                //        {
                //            dragControl.Visibility = editControl.btn3DEdit.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;
                //            dragControlSelected.Visibility = editControl.btn3DEdit.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;
                //            dragControlMultiSelected.Visibility = editControl.btn3DEdit.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;

                //            editControl.btn3DCamera.Visibility = editControl.btn3DEdit.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                //        };

                //    var mapTrackballs = new Dictionary<Viewport3D, Trackball>();
                //    var mapTrackballTranforms = new Dictionary<Viewport3D, Transform3D>();
                //    editControl.btn3DCameraPositions.Click += (o, e) =>
                //        {
                //            var cameraPositionEditor = new CameraTransformsEditor(mapTrackballs.Values.First(), DataContext);
                //            var Dialog = new GeneralDialogContent(cameraPositionEditor)
                //            {
                //                Owner = this.FindParent<Window>(),
                //                Title = Properties.Resources.CameraPositionEditor,
                //                HelpLink = "CameraPositionEditor"
                //            };

                //            // BeginEdit();
                //            if (Dialog.ShowDialog() != true)
                //            {
                //                // CancelEdit();

                //                return;
                //            }

                //            OnFireChanged(this);

                //            // EndEdit();
                //        };

                //    editControl.btn3DCamera.Click += (o, e) =>
                //    {
                //        editControl.btn3DCameraPositions.Visibility = editControl.btn3DCamera.IsChecked == true ?
                //            Visibility.Visible : Visibility.Collapsed;
                //        foreach (var el in listModel3Ds)
                //        {
                //            if (editControl.btn3DCamera.IsChecked == true)
                //            {
                //                el.IsManipulationEnabled = true;
                //                if (!mapTrackballs.ContainsKey(el))
                //                {
                //                    var trackball = new Trackball { EventSource = el };
                //                    mapTrackballs.Add(el, trackball);
                //                }
                //                else
                //                    mapTrackballs[el].EventSource = el;

                //                if (mapTrackballTranforms.ContainsKey(el))
                //                    mapTrackballTranforms.Remove(el);
                //                if (el.Camera.Transform != null)
                //                    mapTrackballTranforms.Add(el, el.Camera.Transform);

                //                el.Camera.Transform = mapTrackballs[el].Transform;
                //                //if (LayoutHelper.HasTouchInput())
                //                //    mapTrackballs[el].SetTouchDevice(true);
                //            }
                //            else
                //            {
                //                el.IsManipulationEnabled = false;
                //                if (mapTrackballTranforms.ContainsKey(el))
                //                    el.Camera.Transform = mapTrackballTranforms[el];
                //                else
                //                    el.Camera.Transform = null;

                //                if (mapTrackballs.ContainsKey(el))
                //                    mapTrackballs[el].EventSource = null;
                //            }
                //        }
                //    };
                //}
            }

            GridViewInfoService.PrepareControlRectangleForSnapLines(adornedElement);

            bEnableFireChanging = true;

            Visibility = Visibility.Visible;
            InvalidateMeasure();
            InvalidateArrange();
            var ret = FindParentCanvas(adornedElement);
            if (ret != null)
                ret.InvalidateArrange();

            // AdornedElement.Blink(1000, 0.5, 1, new SineEase() { EasingMode = EasingMode.EaseIn });
        }

        internal virtual void ResetTransormOriginSettings()
        {
            adornedElement.RenderTransformOrigin = new Point(0.5, 0.5);
            foreach (FrameworkElement control in visualChildren)
            {
                if (control is TransformOriginThumb)
                {
                    ArrangeControl(control);
                }
            }
        }

        internal virtual bool IsTransformOriginChanged()
        {
            return adornedElement.RenderTransformOrigin.X != 0.5 || adornedElement.RenderTransformOrigin.Y != 0.5;
        }

        internal virtual void ShowHideRotationThumbs()
        {
            foreach (FrameworkElement control in visualChildren)
            {
                if (control is RotateThumb)
                {
                    control.Visibility = control.Visibility == System.Windows.Visibility.Visible ?
                        System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                }
                else if (control is TransformOriginThumb)
                {
                    control.Visibility = control.Visibility == System.Windows.Visibility.Visible ?
                        System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                }
            }
        }

        public void Update3DragControl(bool bShow)
        {
            dragControl.Visibility = bShow ? Visibility.Collapsed : Visibility.Visible;
            dragControlSelected.Visibility = bShow ? Visibility.Collapsed : Visibility.Visible;
            dragControlMultiSelected.Visibility = bShow ? Visibility.Collapsed : Visibility.Visible;
        }

        public void Update3DCameraPosition()
        {
            if (DataContext == null)
                return;
            var cameraPositionEditor = new CameraTransformsEditor(mapTrackballs.Values.First(), DataContext);
            var Dialog = new GeneralDialogContent(cameraPositionEditor)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.CameraPositionEditor,
                HelpLink = "CameraPositionEditor"
            };

            if (Dialog.ShowDialog() != true)
            {
                return;
            }

            OnFireChanged(this);
        }

        Dictionary<Viewport3D, Trackball> mapTrackballs = new Dictionary<Viewport3D, Trackball>();
        Dictionary<Viewport3D, Transform3D> mapTrackballTranforms = new Dictionary<Viewport3D, Transform3D>();

        //initialIsHitTestVisibleStatus this is used to restore initial isHitTestVisible property value, before enabling camera
        //Item1: selected object, Item2: IsHitTestVisible of selected object, Item3: IsHitTestVisible of selected object parent 
        List<Tuple<Viewport3D, bool, bool>> initialIsHitTestVisibleStatus = new List<Tuple<Viewport3D, bool, bool>>();
        public void Update3DCamera(bool bShow)
        {
            var listModel3Ds = adornedElement.GetChildrenOfType<Viewport3D>().ToList();
            if (listModel3Ds.Count == 0 && adornedElement is Viewport3D)
                listModel3Ds.Add(adornedElement as Viewport3D);
            foreach (var el in listModel3Ds)
            {
                if (bShow)
                {
                    el.IsManipulationEnabled = true;                    
                    if(el.Parent is Viewbox)
                        initialIsHitTestVisibleStatus.Add(new Tuple<Viewport3D, bool, bool>(el, el.IsHitTestVisible, (el.Parent as Viewbox).IsHitTestVisible));
                    else
                        initialIsHitTestVisibleStatus.Add(new Tuple<Viewport3D, bool, bool>(el, el.IsHitTestVisible, false));

                    EnableObjectInteraction(true);
                    
                    if (!mapTrackballs.ContainsKey(el))
                    {
                        var trackball = new Trackball { EventSource = el };

                        trackball.TrackCompleted += trackball_TrackCompleted;
                        mapTrackballs.Add(el, trackball);
                    }
                    else
                        mapTrackballs[el].EventSource = el;

                    if (mapTrackballTranforms.ContainsKey(el))
                        mapTrackballTranforms.Remove(el);
                    if (el.Camera.Transform != null)
                        mapTrackballTranforms.Add(el, el.Camera.Transform);

                    el.Camera.Transform = mapTrackballs[el].Transform;
                    //if (LayoutHelper.HasTouchInput())
                    //    mapTrackballs[el].SetTouchDevice(true);
                }
                else
                {
                    el.IsManipulationEnabled = false;
                    EnableObjectInteraction(false);

                    if (mapTrackballTranforms.ContainsKey(el))
                        el.Camera.Transform = mapTrackballTranforms[el];
                    else
                        el.Camera.Transform = null;

                    if (mapTrackballs.ContainsKey(el))
                        mapTrackballs[el].EventSource = null;
                }
            }
        }

        private void trackball_TrackCompleted(object sender, EventArgs e)
        {
            OnFireChanged(this);
        }

        void dragControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                //editControl.OpcuaEntityReferenceBtn.Command.Execute(null);
                if (UIGeneralCommands.EditDataContextItem.CanExecute(null, null))
                    UIGeneralCommands.EditDataContextItem.Execute(null, null);
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (UIGeneralCommands.ShowInlineAnimations.CanExecute(null, null))
                    UIGeneralCommands.ShowInlineAnimations.Execute(null, null);
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (UIGeneralCommands.ShowInlineCommands.CanExecute(null, null))
                    UIGeneralCommands.ShowInlineCommands.Execute(null, null);
            }
            else
                // editControl.ShowInlinePropBtn.Command.Execute(null);
                GridViewInfoService.ActivateProperty();
        }

        protected Canvas FindParentCanvas(DependencyObject dObj)
        {
            if (AdornedElement is Canvas)
                return AdornedElement as Canvas;

            var ret = dObj.FindParent<Canvas>();
            if (ret != null)
            {
                var first = ret.FindParent<Canvas>();
                while(first != null)
                {
                    ret = first;
                    first = first.FindParent<Canvas>();
                }
                return ret;
            }
            return AdornedElement as Canvas;
        }

        /*
        public virtual void RestorePosition(bool bSet)
        {
        }
        */

        public virtual void Deactivate()
        {
            foreach(var key in mapTrackballs.Keys)
            {
                mapTrackballs[key].TrackCompleted -= trackball_TrackCompleted;
            }

            mapTrackballs.Clear();
            mapTrackballTranforms.Clear();

            // AdornedElement.Blink(0, 0.5, 1, new BackEase() { EasingMode = EasingMode.EaseOut });
            if (notifier != null)
            {
                notifier.Dispose();
                notifier = null;
            }

            if (isMultiSelection)
            {
                dragControl.DragStarted -= control_DragStarted;
                dragControl.DragCompleted -= control_DragCompleted;
                dragControl.DragDelta -= control_DragDelta;
            }

            if (dragControl != null)
                dragControl.Deactivate();

            dragControl.MouseDoubleClick -= dragControl_MouseDoubleClick;

            bEnableFireChanging = false;

            InvalidateMeasure();
            InvalidateArrange();
            var ret = FindParentCanvas(adornedElement);
            if (ret != null)
                ret.InvalidateArrange();

            visualChildren.Clear();
            //currentThumb = null;
            adornedElement.UpdateLayout();

            //EnableObjectInteraction(false);
            RestorePreviousObjectInteractionStatus();
            initialIsHitTestVisibleStatus.Clear();
            Visibility = Visibility.Collapsed;
        }


        private void EnableObjectInteraction(bool bEnable)
        {
            var listModel3Ds = adornedElement.GetChildrenOfType<Viewport3D>().ToList();
            if (listModel3Ds.Count == 0 && adornedElement is Viewport3D)
                listModel3Ds.Add(adornedElement as Viewport3D);
            foreach (var el in listModel3Ds)
            {
                el.IsHitTestVisible = bEnable;
                if (el.Parent is Viewbox)
                {
                    (el.Parent as Viewbox).IsHitTestVisible = bEnable;
                }
            }
        }

        private void RestorePreviousObjectInteractionStatus()
        {
            foreach(var tuple in initialIsHitTestVisibleStatus)
            {
                tuple.Item1.IsHitTestVisible = tuple.Item2;
                if (tuple.Item1.Parent is Viewbox)
                {
                    (tuple.Item1.Parent as Viewbox).IsHitTestVisible = tuple.Item3;
                }
            }
        }

        public virtual bool IsSelactable(UIElement el)
        {
            return true;
        }

        public virtual void SetIsActive(bool bSet, bool isMultiselection = false)
        {
            if (bSet && isMultiselection)
            {
                dragControlMultiSelected.Visibility = System.Windows.Visibility.Visible;
            }
            else
                dragControlMultiSelected.Visibility = System.Windows.Visibility.Hidden;

            if (dragControlSelected == null)
                return;
            dragControlSelected.Visibility = bCanMove && bSet ? Visibility.Visible : Visibility.Hidden;
        }

        public virtual void SetDraggable(bool bSet, bool bInside = false, bool bForce = false)
        {
            if (editControl != null /*&& editControl.btn3DEdit.IsChecked == true*/)
                return;
            dragControl.Visibility = isMultiSelection || bCanMove && bSet ? Visibility.Visible : Visibility.Hidden;
            if (bInside && dragControlSelected != null)
            {
                dragControlSelected.Visibility = Visibility.Hidden;
            }
            if (bInside && dragControlMultiSelected != null)
            {
                dragControlMultiSelected.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        internal void ShowExpander(bool showAdornerExpander)
        {
            if (editControl == null)
                return;
            editControl.expander.IsExpanded = showAdornerExpander;
        }

        #endregion

        #region Events

        public event EventHandler changed;
        public void OnFireChanged(Object sender, AdornerOperationEventArgs e = null)
        {
            EventHandler temp = changed;
            if (temp != null)
                temp(sender, e ?? EventArgs.Empty);
        }

        public event EventHandler changing;
        public void OnFireChanging(Object sender)
        {
            if (!bEnableFireChanging)
                return;

            EventHandler temp = changing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        #endregion
    }
}