#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Resources;
using System.IO;
using System.Threading;


namespace Syncfusion.Windows.Shared.Controls

{
    /// <summary>
    /// 
    /// </summary>
    public static class DragAndDropManager 
    {
        static DispatcherTimer myDispatcherTimer = null;
        static bool isPopup = false;
        static DragAndDropManager()
        {
           _dragdropeventargs = new DragDropEventArgs();
           if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
           myDispatcherTimer = new DispatcherTimer();
           myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
           myDispatcherTimer.Tick += new EventHandler(myDispatcherTimer_Tick);
        }

        static void myDispatcherTimer_Tick(object sender, EventArgs e)
        {
            CancelDrag();
            myDispatcherTimer.Stop();
        }
        #region Private Members

        private static object DraggingItem=null;

        private static DragArrow dragArrow;

        private static DragMode _dragMode = DragMode.Move;

        internal static Popup _dragPopup = new Popup();

        private static Storyboard _returnAnimation;

        private static Popup _dragArrow;

        private static Popup _dragiconpoup;

        private static Popup _descriptionpopup;

        private static double _dragiconheight;

        private static double _dragiconwidth;

        private static bool _ismousepressed;

        private static bool _dropCompleted;

        private static DragDropEventArgs _dragdropeventargs = null;

        private static DragDecorator _dragDecorator;

        private static Point _mouseposition;

        private static Storyboard scrollstory = new Storyboard();

        private static WeakReference rootVisual = new WeakReference(null);

        private static bool IsKeyPressProcessed = false;

        private static bool IsKeyReleaseProcessed = false;

        private static bool IsCancelOnEscape = true;

        private static Visibility IsDropDescriptionVisibile = Visibility.Visible;

        private static ControlTemplate dragArrowTemplate = null;

        private static bool IsReturnAnimationEnabled = true;

        internal static FrameworkElement RootVisual
        {
            get
            {
                return (rootVisual.Target as FrameworkElement);
               
            }

            set
            {
                if (rootVisual.Target != null && rootVisual.IsAlive)
                {
                    RemoveRootHandlers(rootVisual.Target as FrameworkElement);
                }
                FrameworkElement element = value as FrameworkElement;
                if (element != null)
                {
                    rootVisual = new WeakReference(element);
                    AddRootHandlers(element);
                    
                }
            }
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// 
        /// </summary>
        public static event DragDropEventHandler DragStarted;
        /// <summary>
        /// 
        /// </summary>
        public static event DragDropEventHandler Drag;
        /// <summary>
        /// 
        /// </summary>
        public static event DragDropEventHandler DropEnter;
        /// <summary>
        /// 
        /// </summary>
        public static event DragDropEventHandler Drop;
        /// <summary>
        /// 
        /// </summary>
        public static event DragDropEventHandler DropLeave;
        /// <summary>
        /// 
        /// </summary>
        public static event DragDropEventHandler DropCancelled;
        /// <summary>
        /// 
        /// </summary>
        public static event DragDropEventHandler QueryContinueDrag;

       #endregion

        #region Implementation
        private static void OnDragStarted(object sender, MouseEventArgs e)
        {
            _dragdropeventargs.MouseEventArgs = e;

            if (_dragdropeventargs.Status == Status.DragStarted)
            {
                _descriptionpopup.IsOpen = true;
                //if (IsDropDescriptionVisibile == Visibility.Visible && _dragDecorator.Descriptiontext!=null)                    
                //    _dragDecorator.Descriptiontext.Visibility = Visibility.Visible;
                _dragdropeventargs.OriginalSource = e.OriginalSource;
            
                ItemsControl itemsDragSource = ItemsControl.ItemsControlFromItemContainer(sender as DependencyObject);
                if (itemsDragSource != null)
                {
                    _dragdropeventargs.DragSource = itemsDragSource;
                }
                else
                {
                    if (sender is FrameworkElement)
                    {
                        _dragdropeventargs.DragSource = ((FrameworkElement)sender).Parent;
                     }
                }

                if (DragStarted != null)
                {
                    DragStarted(sender, _dragdropeventargs);
                }

                if (_dragdropeventargs.DragIcon != null)
                {
                    _dragdropeventargs.DragIcon.IsHitTestVisible = false;
                    GetDragImage(e, _dragdropeventargs.DragIcon);
                }
                else
                {
                    object payLoad = _dragdropeventargs.PayLoad;
                    if (payLoad is UIElement)
                    {
                        Image img = null;
                       ImageSource source = new WriteableBitmap(payLoad as UIElement, new TranslateTransform());
                        img = new Image() { Source = source, IsHitTestVisible = false };
                                    
                        if (payLoad != null && img != null)
                        {
                            GetDragImage(e, img);
                        }
                    }
                }
                if (_dragdropeventargs.IsDragArrowEnabled)
                {
                    GetDragArrow(e.GetPosition(null));
                }
                _dragdropeventargs.Status = Status.DragInProgress;
             }
        }

        private static void OnDrag(object sender, MouseEventArgs e)
        {
            _dragdropeventargs.MouseEventArgs = e;
            if (_dragdropeventargs.Status == Status.DragInProgress || _dragdropeventargs.Status == Status.Impossible)
            {
                _dragdropeventargs.OriginalSource = e.OriginalSource;
                if (_dragiconpoup != null && _dragiconpoup.IsOpen)
                {

                    _dragiconpoup.HorizontalOffset = e.GetPosition(null).X - (_dragiconwidth / 2);
                    _dragiconpoup.VerticalOffset = e.GetPosition(null).Y - (_dragiconheight - 12);

                }
                if (_descriptionpopup != null && _descriptionpopup.IsOpen)
                {
                    _descriptionpopup.HorizontalOffset = e.GetPosition(null).X + 10;
                    _descriptionpopup.VerticalOffset = e.GetPosition(null).Y + 10;
                }
                if (_dragArrow != null && _dragArrow.IsOpen)
                {
                    TransformGroup group1 = new TransformGroup
                    {
                        Children = { new ScaleTransform(), new RotateTransform() }
                    };
                    dragArrow.RenderTransform = group1;

                    double horizontalDif = _mouseposition.X - e.GetPosition(null).X;
                    double verticalDif = _mouseposition.Y - e.GetPosition(null).Y;
                    double distance = Math.Sqrt((horizontalDif * horizontalDif) + (verticalDif * verticalDif));

                    TransformGroup group = dragArrow.RenderTransform as TransformGroup;
                    ScaleTransform scale = group.Children[0] as ScaleTransform;
                    RotateTransform rotate = group.Children[1] as RotateTransform;
                    dragArrow.Width = distance;

                    if (horizontalDif != 0.0)
                    {
                        rotate.Angle = (Math.Atan(verticalDif / horizontalDif) * 180.0) / 3.1415926535897931;
                    }
                    else
                    {
                        rotate.Angle = (verticalDif < 0.0) ? ((double)90) : ((double)(-90));
                    }
                    if (horizontalDif > 0.0)
                    {
                        rotate.Angle += 180.0;
                        scale.ScaleY = -1.0;
                    }
                    else
                    {
                        scale.ScaleY = 1.0;
                    }
                }
               
                if (Drag != null)
                {
                    Drag(sender, _dragdropeventargs);
                }
            }
        }

        private static void OnDropEnter(object sender, MouseEventArgs e)
        {
            _dragdropeventargs.MouseEventArgs = e;
            if (_dragdropeventargs.Status == Status.DragInProgress)
            {
                          
                _dragdropeventargs.OriginalSource = e.OriginalSource;
                _dragdropeventargs.DropTarget = sender;

                if (DropEnter != null)
                {
                    DropEnter(sender, _dragdropeventargs);
                 }

                if (_dragdropeventargs.Status != Status.Impossible)
                {
                    if (String.IsNullOrEmpty(_dragdropeventargs.DropDescription))
                    {              
                        string moveText = String.Empty;
                        if (_dragMode == DragMode.Move)
                        {
                            moveText = "Move To ";
                        }
                        else
                        {
                            moveText = "Copy To ";
                        }
                        if (!String.IsNullOrEmpty(((FrameworkElement)_dragdropeventargs.DropTarget).Name))
                        {
                            _dragDecorator.DropDescription = moveText + ((FrameworkElement)_dragdropeventargs.DropTarget).Name;
                        }
                        else
                        {
                            _dragDecorator.DropDescription = moveText + _dragdropeventargs.DropTarget.ToString();
                        }

                    }
                    else
                    {
                        _dragDecorator.DropDescription = _dragdropeventargs.DropDescription;
                    }

                    if (_dragDecorator.BackBorder != null && IsDropDescriptionVisibile == Visibility.Visible)
                    {
                        _dragDecorator.BackBorder.Visibility = Visibility.Visible;
                    }
                    
                    if (_dragDecorator.ImpossiblePath != null)
                    {
                        _dragDecorator.ImpossiblePath.Visibility = Visibility.Collapsed;
                    }
                    if (_dragDecorator.Descriptiontext != null && IsDropDescriptionVisibile==Visibility.Visible)
                    {
                        _dragDecorator.Descriptiontext.Visibility = Visibility.Visible;
                    }

                    if (_dragMode == DragMode.Move)
                    {
                        if (_dragDecorator.Descriptiontext != null)
                        {
                            _dragDecorator.CopyPath.Visibility = Visibility.Collapsed;
                        }
                        if (_dragDecorator.MovePath != null && IsDropDescriptionVisibile ==Visibility.Visible)
                        {
                            _dragDecorator.MovePath.Visibility = Visibility.Visible;
                            _dragDecorator.Descriptiontext.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        if (_dragDecorator.Descriptiontext != null && IsDropDescriptionVisibile==Visibility.Visible)
                        {
                            _dragDecorator.CopyPath.Visibility = Visibility.Visible;
                            _dragDecorator.Descriptiontext.Visibility = Visibility.Visible;
                        }
                        if (_dragDecorator.MovePath != null)
                        {
                            _dragDecorator.MovePath.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                else
                {
                    if (_dragDecorator.BackBorder != null)
                    {
                        _dragDecorator.BackBorder.Visibility = Visibility.Collapsed;
                    }
                    if (_dragDecorator.MovePath != null)
                    {
                        _dragDecorator.MovePath.Visibility = Visibility.Collapsed;
                    }
                    if (_dragDecorator.ImpossiblePath != null)
                    {
                        _dragDecorator.ImpossiblePath.Visibility = Visibility.Visible;
                    }
                    if (_dragDecorator.Descriptiontext != null)
                    {
                        _dragDecorator.Descriptiontext.Visibility = Visibility.Collapsed;
                    }
                    if (_dragDecorator.Descriptiontext != null)
                    {
                        _dragDecorator.CopyPath.Visibility = Visibility.Collapsed;
                    }
                }

                _descriptionpopup.HorizontalOffset = e.GetPosition(null).X + 10;
                _descriptionpopup.VerticalOffset = e.GetPosition(null).Y + 10;
            }
        }

        private static void OnDropLeave(object sender, MouseEventArgs e)
        {
            _dragdropeventargs.OriginalSource = e.OriginalSource;
            _dragdropeventargs.MouseEventArgs = e;
            if (_dragdropeventargs.Status == Status.Impossible)
            {
                _dragdropeventargs.Status = Status.DragInProgress;
            }

            if (_dragdropeventargs.Status == Status.DragInProgress)
            {
               if (_descriptionpopup.IsOpen)
                {
                    if (_dragDecorator.BackBorder != null)
                    {
                        _dragDecorator.BackBorder.Visibility = Visibility.Collapsed;
                    }
                    if (_dragDecorator.MovePath != null)
                    {
                        _dragDecorator.MovePath.Visibility = Visibility.Collapsed;
                    }
                    if (_dragDecorator.ImpossiblePath != null)
                    {
                        _dragDecorator.ImpossiblePath.Visibility = Visibility.Collapsed;
                    }
                    if (_dragDecorator.Descriptiontext != null)
                    {
                       _dragDecorator.Descriptiontext.Visibility = Visibility.Collapsed;
                    }
                    if (_dragDecorator.Descriptiontext != null)
                    {
                        _dragDecorator.CopyPath.Visibility = Visibility.Collapsed;
                    }
                }

               if (DropLeave != null)
                {
                    DropLeave(sender, _dragdropeventargs);
                   
                }
            }
        }

        internal static void ClearDragDropEventArgs()
        {
            if (_dragdropeventargs != null)
            {
                _dragdropeventargs.DropTarget = null;
                _dragdropeventargs.DragIcon = null;
                _dragdropeventargs.DragSource = null;
                _dragdropeventargs.DropDescription = null;
                _dragdropeventargs.MouseEventArgs = null;
                _dragdropeventargs.OriginalSource = null;
                _dragdropeventargs.PayLoad = null;
            }
       }
        private static void OnDrop(object sender, MouseEventArgs e)
        {
            _dragdropeventargs.OriginalSource = e.OriginalSource;
            _dragdropeventargs.MouseEventArgs = e;
            if (_dragdropeventargs.Status == Status.DragInProgress)
            {
                _dragiconpoup.IsOpen = false;
                _dragArrow.IsOpen = false;
                _descriptionpopup.IsOpen = false;
                _dragdropeventargs.DropTarget = sender;
                
                if (!_dropCompleted)
                {
                    _dropCompleted = true;
                }

                if (_dragdropeventargs.Status != Status.Impossible)
                {
                    _dragdropeventargs.Status = Status.DropSuccess;

                    if (Drop != null)
                    {
                        Drop(sender, _dragdropeventargs);
                    }
                }
                else
                {
                    _dragdropeventargs.Status = Status.Cancel;

                    if (DropCancelled != null)
                    {
                        DropCancelled(_dragdropeventargs.DropTarget, _dragdropeventargs);
                    }
                }
            }
            ClearDragDropEventArgs();
        }
        /// <summary>
        /// 
        /// </summary>
        public static void CancelDrag()
        {
            _ismousepressed = false;
            if (_dragdropeventargs.Status == Status.DragInProgress || _dragdropeventargs.Status == Status.Impossible)
            {
                if (_dragiconpoup != null && _dragiconpoup.IsOpen && IsReturnAnimationEnabled)
                {
                    if (_returnAnimation != null)
                    {
                        _returnAnimation.Stop();
                        DoubleAnimationUsingKeyFrames keyFrames = _returnAnimation.Children[2] as DoubleAnimationUsingKeyFrames;
                        Storyboard.SetTarget(keyFrames, _dragiconpoup.Child);
                       _returnAnimation.Begin();
                    }

                }
                
                _descriptionpopup.IsOpen = false;
                if(!IsReturnAnimationEnabled)
                _dragiconpoup.IsOpen = false;
                _dropCompleted = true;
                dragArrow = null;
                _dragArrow.IsOpen = false;
                _dragdropeventargs.Status = Status.Cancel;
                if (DropCancelled != null)
                {
                    DropCancelled(_dragdropeventargs.DropTarget, _dragdropeventargs);
                }
                ClearDragDropEventArgs();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetText(Uri uri)
        {
            string returnString = String.Empty;
            StreamResourceInfo resourceStream = Application.GetResourceStream(uri);
            if (resourceStream != null && resourceStream.Stream != null)
            {
                using (StreamReader stream = new StreamReader(resourceStream.Stream))
                {
                    returnString = stream.ReadToEnd();
                    stream.Close();
                }
            }
            return returnString;
        }
#endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetAllowDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowDragProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetAllowDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowDragProperty, value);
        }

        // Using a DependencyProperty as the backing store for AllowDrag.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AllowDragProperty =
            DependencyProperty.RegisterAttached("AllowDrag", typeof(bool), typeof(DragAndDropManager), new PropertyMetadata(false, OnAllowDragChanged));

      /// <summary>
      /// 
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
        public static bool GetAllowDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowDropProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetAllowDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowDropProperty, value);
        }

        // Using a DependencyProperty as the backing store for AllowDrop.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AllowDropProperty =
            DependencyProperty.RegisterAttached("AllowDrop", typeof(bool), typeof(DragAndDropManager), new PropertyMetadata(false, new PropertyChangedCallback(OnAllowDropChanged)));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetDragThreshold(DependencyObject obj)
        {
            return (double)obj.GetValue(DragThresholdProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetDragThreshold(DependencyObject obj, double value)
        {
            obj.SetValue(DragThresholdProperty, value);
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for DragThreshold.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DragThresholdProperty =
            DependencyProperty.RegisterAttached("DragThreshold", typeof(double), typeof(DragAndDropManager), new PropertyMetadata(2.0));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DragMode GetDragMode(DependencyObject obj)
        {
            return (DragMode)obj.GetValue(DragModeProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetDragMode(DependencyObject obj, DragMode value)
        {
            obj.SetValue(DragModeProperty, value);
        }
         
        /// <summary>
        /// Using a DependencyProperty as the backing store for DragMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DragModeProperty =
            DependencyProperty.RegisterAttached("DragMode", typeof(DragMode), typeof(DragAndDropManager), new PropertyMetadata(DragMode.Move, new PropertyChangedCallback(OnDragModeChanged)));



        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ControlTemplate GetDragArrowTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(DragArrowTemplateProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetDragArrowTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(DragArrowTemplateProperty,value);
            dragArrowTemplate = value;
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DragArrowTemplateProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DragArrowTemplateProperty =
            DependencyProperty.RegisterAttached("DragArrowTemplate", typeof(ControlTemplate), typeof(DragAndDropManager), new PropertyMetadata(null));

        
     

        /// <summary>
        /// The boolean value that determines whether Drag opertaion is cancelled or not on pressing Esc key
        /// </summary>
        public static readonly DependencyProperty CancelOnEscapeProperty =
            DependencyProperty.RegisterAttached("CancelOnEscape", typeof(bool), typeof(DragAndDropManager), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetCancelOnEscape(DependencyObject obj)
        {
            return (bool)obj.GetValue(CancelOnEscapeProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetCancelOnEscape(DependencyObject obj, bool value)
        {
            obj.SetValue(CancelOnEscapeProperty, value);
            IsCancelOnEscape = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetEnableReturnAnimation(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableReturnAnimationProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetEnableReturnAnimation(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableReturnAnimationProperty, value);
            IsReturnAnimationEnabled = value;
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for etEnableReturnAnimation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableReturnAnimationProperty =
            DependencyProperty.RegisterAttached("EnableReturnAnimation", typeof(bool), typeof(DragAndDropManager), new PropertyMetadata(true));

        

        /// <summary>
        /// The value determines whether DropDescription is vibile or not
        /// </summary>
        public static readonly DependencyProperty DropDescriptionVisibilityProperty =
            DependencyProperty.RegisterAttached("DropDescriptionVisibility", typeof(Visibility), typeof(DragAndDropManager), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Visibility GetDropDescriptionVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(DropDescriptionVisibilityProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetDropDescriptionVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(DropDescriptionVisibilityProperty, value);
            IsDropDescriptionVisibile = value;
                       
        }
        #endregion

      
        private static void OnAllowDragChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement dragsource = sender as FrameworkElement;
            if ((bool)dragsource.GetValue(DragAndDropManager.AllowDragProperty))
            {

                dragsource.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(dragsource_MouseLeftButtonDown), true);
                dragsource.AddHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(dragsource_MouseLeftButtonUp), true);
                dragsource.MouseMove += new MouseEventHandler(dragsource_MouseMove);
                dragsource.Loaded+=new RoutedEventHandler(OnManagerLoaded);
             
            }
            else
            {
                dragsource.RemoveHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(dragsource_MouseLeftButtonDown));
                dragsource.MouseMove -= new MouseEventHandler(dragsource_MouseMove);
           }
            InitializeManager();
        }

        private static void OnDragModeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement dragsource = sender as FrameworkElement;

            if (dragsource != null)
            {
                
                if ((DragMode)dragsource.GetValue(DragAndDropManager.DragModeProperty) == DragMode.Move)
                {
                    if (_dragDecorator != null)
                    {
                        if (_dragDecorator.Descriptiontext != null)
                        {
                           _dragDecorator.CopyPath.Visibility = Visibility.Collapsed;
                        }
                       
                        if (_dragDecorator.MovePath != null )
                        {
                                _dragDecorator.MovePath.Visibility = Visibility.Visible;
                                _dragDecorator.DropDescription = "Move To " + ((FrameworkElement)_dragdropeventargs.DragSource).Name;
                                _dragDecorator.Descriptiontext.Visibility = Visibility.Visible;
                                _descriptionpopup.Visibility = Visibility.Visible;
         
                        }

                        if (String.IsNullOrEmpty(_dragdropeventargs.DropDescription))
                        {
                            if(_dragdropeventargs.DropTarget != null)

                            if (!String.IsNullOrEmpty(((FrameworkElement)_dragdropeventargs.DropTarget).Name))
                            {
                                _dragDecorator.DropDescription = "Move To " + ((FrameworkElement)_dragdropeventargs.DropTarget).Name;
                            }
                            else
                            {
                                _dragDecorator.DropDescription = "Move To " + _dragdropeventargs.DropTarget.ToString();
                            }

                        }
                        else
                        {
                            _dragDecorator.DropDescription = _dragdropeventargs.DropDescription;
                        }

                    }
                    _dragMode = DragMode.Move;
                }
                else
                {
                    if (_dragDecorator != null)
                    {
                        if (_dragDecorator.Descriptiontext != null)
                        {
                            _dragDecorator.CopyPath.Visibility = Visibility.Visible;
                            _dragDecorator.DropDescription = "Copy To " + ((FrameworkElement)_dragdropeventargs.DragSource).Name;
                            _dragDecorator.Descriptiontext.Visibility = Visibility.Visible;
                            _descriptionpopup.Visibility = Visibility.Visible;
         
                        }
                        if (_dragDecorator.MovePath != null)
                        {
                            _dragDecorator.MovePath.Visibility = Visibility.Collapsed;
                        }

                        if (String.IsNullOrEmpty(_dragdropeventargs.DropDescription))
                        {
                            if (_dragdropeventargs.DropTarget != null)
                            {
                                if (!String.IsNullOrEmpty(((FrameworkElement)_dragdropeventargs.DropTarget).Name))
                                {
                                    _dragDecorator.DropDescription = "Copy To " + ((FrameworkElement)_dragdropeventargs.DropTarget).Name;
                                }
                                else
                                {
                                    _dragDecorator.DropDescription = "Copy To " + _dragdropeventargs.DropTarget.ToString();
                                }
                            }
                        }
                        else
                        {
                            _dragDecorator.DropDescription = _dragdropeventargs.DropDescription;
                        }

                    }
                    _dragMode = DragMode.Copy;
                }
            }
        }

        private static void OnManagerLoaded(object sender, RoutedEventArgs e)
        {
            if (RootVisual == null)
            {
                RootVisual = Application.Current.RootVisual as FrameworkElement;
           }
        }

        private static void InitializeManager()
        {
             RootVisual = Application.Current.RootVisual as FrameworkElement;
            _dragArrow = new Popup();
            _dragiconpoup = new Popup();
            _descriptionpopup = new Popup();
            _dragDecorator = new DragDecorator();
            DoubleAnimation scrollanimation = new DoubleAnimation();
            scrollstory.Children.Add(scrollanimation);
             CreateReturnAnimation();
            _descriptionpopup.Child = _dragDecorator;
           
        }

        private static void AddRootHandlers(FrameworkElement root)
        {
            if (root != null)
            {

                root.AddHandler(FrameworkElement.KeyDownEvent, new KeyEventHandler(RootKeyDown), true); 
                root.AddHandler(FrameworkElement.KeyUpEvent, new KeyEventHandler(RootKeyUp), true);
                root.MouseMove += new MouseEventHandler(RootVisual_MouseMove);
                root.AddHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftUp), true);
                root.MouseLeave += new MouseEventHandler(RootVisual_MouseLeave);
            }
        }

          
      private static void RemoveRootHandlers(FrameworkElement root)
        {
            if (root != null)
            {
                root.MouseMove -= new MouseEventHandler(RootVisual_MouseMove);
                root.MouseLeave -= new MouseEventHandler(RootVisual_MouseLeave);
                root.RemoveHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftUp));
            }
        }

        private static void RootKeyDown(object sender, KeyEventArgs e)
            {
             
            IsKeyReleaseProcessed = false;
            
            _dragdropeventargs.DragKey = e.Key;

            if (QueryContinueDrag != null && _dragdropeventargs.Status == Status.DragInProgress && !IsKeyPressProcessed)
            {
               QueryContinueDrag(sender, _dragdropeventargs);
             }

           if (IsCancelOnEscape && e.Key == Key.Escape)
            {
                CancelDrag();
            }
           
            IsKeyPressProcessed = true;
           
        }

        private static void RootKeyUp(object sender, KeyEventArgs e)
       {
            IsKeyPressProcessed = false;

            _dragdropeventargs.DragKey = Key.None;

            if (QueryContinueDrag != null && _dragdropeventargs.Status == Status.DragInProgress && !IsKeyReleaseProcessed)
            {
               QueryContinueDrag(sender, _dragdropeventargs);
            }

            IsKeyReleaseProcessed = true;
        }

        static void RootVisual_MouseLeave(object sender, MouseEventArgs e)
        {
           
            if (isPopup)
                myDispatcherTimer.Start();
            else
                CancelDrag();
        }

        private static void CreateReturnAnimation()
        {
            _returnAnimation = new Storyboard();
            _returnAnimation.Completed += new EventHandler(_returnAnimation_Completed);
            DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();

            ExponentialEase ease = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 5 };

            Storyboard.SetTarget(keyFrames, _dragiconpoup);
            Storyboard.SetTargetProperty(keyFrames, new PropertyPath(Popup.HorizontalOffsetProperty));
            EasingDoubleKeyFrame easingFrame2 = new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0.7), EasingFunction = ease };
            keyFrames.KeyFrames.Add(easingFrame2);
            _returnAnimation.Children.Add(keyFrames);

            DoubleAnimationUsingKeyFrames keyFrames1 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(keyFrames1, _dragiconpoup);
            Storyboard.SetTargetProperty(keyFrames1, new PropertyPath(Popup.VerticalOffsetProperty));
            EasingDoubleKeyFrame _easingFrame2 = new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0.7), EasingFunction = ease };
            keyFrames1.KeyFrames.Add(_easingFrame2);
            _returnAnimation.Children.Add(keyFrames1);

            DoubleAnimationUsingKeyFrames keyFrames2 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(keyFrames2, new PropertyPath(Popup.OpacityProperty));
            EasingDoubleKeyFrame _easingFrame3 = new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0.7), EasingFunction = ease, Value = 0.0 };
            keyFrames2.KeyFrames.Add(_easingFrame3);
            _returnAnimation.Children.Add(keyFrames2);
        }

        static void _returnAnimation_Completed(object sender, EventArgs e)
        {
            _dragiconpoup.Child = null;
            _dragiconpoup.IsOpen = false;
        }

        private static void dragsource_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (_ismousepressed && (_dragdropeventargs.Status == Status.Cancel || _dragdropeventargs.Status == Status.DropSuccess))
            {
                Point currentPosition = e.GetPosition(null);
                double threshold = GetDragThreshold(sender as DependencyObject);
                if (!(((Math.Abs((double)(currentPosition.X - _mouseposition.X)) < threshold) && (Math.Abs((double)(currentPosition.Y - _mouseposition.Y)) < threshold))))
                {
                    _dragdropeventargs.Status = Status.DragStarted;
                    OnDragStarted(DraggingItem, e);
                    //_ismousepressed = false;
                }
            }
        }

        private static void SetAnimationValue(Point point)
        {
            DoubleAnimationUsingKeyFrames keyFrames = _returnAnimation.Children[0] as DoubleAnimationUsingKeyFrames;
            DoubleAnimationUsingKeyFrames keyFrames1 = _returnAnimation.Children[1] as DoubleAnimationUsingKeyFrames;

            EasingDoubleKeyFrame easingFrame1 = keyFrames.KeyFrames[0] as EasingDoubleKeyFrame;
            EasingDoubleKeyFrame easingFrame2 = keyFrames1.KeyFrames[0] as EasingDoubleKeyFrame;

            easingFrame1.Value = point.X;
            easingFrame2.Value = point.Y;
        }

        private static void OnAllowDropChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement droptarget = sender as FrameworkElement;
                       
            if ((bool)droptarget.GetValue(DragAndDropManager.AllowDropProperty))
            {

                droptarget.AddHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(droptarget_MouseLeftButtonUp), true);
                droptarget.MouseMove += new MouseEventHandler(droptarget_MouseMove);
                droptarget.MouseEnter += new MouseEventHandler(droptarget_MouseEnter);
                droptarget.MouseLeave += new MouseEventHandler(droptarget_MouseLeave);
             }
            else
            {
               droptarget.MouseEnter -= new MouseEventHandler(droptarget_MouseEnter);
               droptarget.MouseLeave -= new MouseEventHandler(droptarget_MouseLeave);
               droptarget.RemoveHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(droptarget_MouseLeftButtonUp));
               droptarget.MouseMove -= new MouseEventHandler(droptarget_MouseMove);
            }
            if (droptarget is Popup)
                isPopup = true;
            
        }

        private static void droptarget_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragdropeventargs.Status == Status.DragInProgress)
            {
                ScrollViewer scroller = GetScroller(sender as UIElement);
                AdjustScrollViewer(scroller, e.GetPosition(null));
                RootVisual_MouseMove(sender, e);
                           
            }
        }

        private static ScrollViewer GetScroller(UIElement parent)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            if (count > 0)
            {
                for (int i = 0; i < count; )
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                    i++;
                    if (child.GetType() != typeof(ScrollViewer))
                    {
                        return GetScroller(child);
                    }
                    else
                    {
                        return child as ScrollViewer;
                    }
                }
            }
            return null;
        }
        
        private static void dragsource_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ismousepressed = true;
            _mouseposition = e.GetPosition(null);
            DraggingItem = sender;
        }

        private static void dragsource_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
          _ismousepressed = false;
          DraggingItem = null;
        }

        private static void GetDragArrow(Point point)
        {

            dragArrow = new DragArrow();
            if (dragArrowTemplate != null)
                dragArrow.Template = dragArrowTemplate;

            _dragArrow.Child = dragArrow;
            
            _dragArrow.HorizontalOffset = point.X;
            _dragArrow.VerticalOffset = point.Y;
            _dragArrow.IsOpen = true;
        }

        private static void OnMouseLeftUp(object sender, MouseButtonEventArgs args)
        {
            _ismousepressed = false;
            CancelDrag();
        }

        static void droptarget_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_ismousepressed == true)
            {
                OnDropLeave(sender, e);
            }
        }

        static void droptarget_MouseEnter(object sender, MouseEventArgs e)
        {
            myDispatcherTimer.Stop();
            if (_ismousepressed == true)
            {
                OnDropEnter(sender, e);
            }
        }

        private static void GetDragImage(MouseEventArgs e, UIElement icon)
        {
            _dragiconpoup.Child = icon;
            _dragiconpoup.IsOpen = true;
            icon.UpdateLayout();
           icon.InvalidateArrange();
            _dragiconwidth = icon.RenderSize.Width;
           _dragiconheight = icon.RenderSize.Height;
            _dragiconpoup.HorizontalOffset = e.GetPosition(null).X - (_dragiconwidth / 2);
            _dragiconpoup.VerticalOffset = e.GetPosition(null).Y - (_dragiconheight - 12);
            if(IsReturnAnimationEnabled)
            SetAnimationValue(new Point(_dragiconpoup.HorizontalOffset, _dragiconpoup.VerticalOffset));
            
        }

        static void RootVisual_MouseMove(object sender, MouseEventArgs e)
        {
            if (_ismousepressed == true)
            {
                OnDrag(sender, e);
            }
        }

        private static void AdjustScrollViewer(ScrollViewer viewer, Point currentPoint)
        {
            
            if (viewer != null && _dragdropeventargs.Status == Status.DragInProgress)
            {
                Point p = currentPoint;
                Point topLeft = viewer.TransformToVisual(null).Transform(new Point(0.0, 0.0));
                Point relative = new Point(p.X - topLeft.X, p.Y - topLeft.Y);
                if (relative.Y > 0.0 && relative.Y < 30.0)
                {
                    viewer.ScrollToVerticalOffset(viewer.VerticalOffset - (20.0 * ((40.0 - relative.Y) / 40.0)));
                }
                if ((relative.Y > (viewer.ActualHeight - 40.0)) && (relative.Y < viewer.ActualHeight))
                {
                    viewer.ScrollToVerticalOffset(viewer.VerticalOffset + (20.0 * ((40.0 - (viewer.ActualHeight - relative.Y)) / 40.0)));
                }
                if ((relative.X > 0.0) && (relative.X < 40.0))
                {
                    viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset - (20.0 * ((40.0 - relative.X) / 40.0)));
                }
                if ((relative.X > (viewer.ActualWidth - 40.0)) && (relative.X < viewer.ActualWidth))
                {
                    viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset + (20.0 * ((40.0 - (viewer.ActualWidth - relative.X)) / 40.0)));
                }
            }
        }

        static void droptarget_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            _ismousepressed = false;
            OnDrop(sender, args);
        }

        //protected virtual static void Dispose()
        //{
        //    if (((FrameworkElement)_dragdropeventargs.DropTarget) != null)
        //    {
        //        ((FrameworkElement)_dragdropeventargs.DropTarget).RemoveHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(droptarget_MouseLeftButtonUp));
        //        ((FrameworkElement)_dragdropeventargs.DropTarget).MouseEnter -= new MouseEventHandler(droptarget_MouseEnter);
        //        ((FrameworkElement)_dragdropeventargs.DropTarget).MouseLeave -= new MouseEventHandler(droptarget_MouseLeave);
        //        ((FrameworkElement)_dragdropeventargs.DropTarget).MouseMove -= new MouseEventHandler(droptarget_MouseMove);
        //    }

        //    if (((FrameworkElement)_dragdropeventargs.DragSource) != null)
        //    {
        //        ((FrameworkElement)_dragdropeventargs.DragSource).RemoveHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(dragsource_MouseLeftButtonDown));
        //        ((FrameworkElement)_dragdropeventargs.DragSource).MouseMove -= new MouseEventHandler(dragsource_MouseMove);
        //    }
            
        //    _returnAnimation.Completed += new EventHandler(_returnAnimation_Completed);
        //}

    }
    /// <summary>
    /// 
    /// </summary>
    public enum DragMode
    {
        /// <summary>
        /// 
        /// </summary>
        Move,
        /// <summary>
        /// 
        /// </summary>
        Copy
    }

}
