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
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Diagram
{
    internal class CustomCursor
    {

        private bool _MouseEnter = false;
        private bool _MouseDown = false;
        private bool _IsPortDraggging = false;
        private FrameworkElement _source = null;
        internal UIElement _Rotate;
        internal UIElement _Move;
        internal Canvas _Overlay;
        
        /// <summary>
        /// Stores a reference to the current Popup.
        /// </summary>
        private Popup _popup;

        /// <summary>
        /// Stores a reference to the current overlay.
        /// </summary>
        public UIElement Content
        {
            //get { return _popup.Child; }
            //set { _popup.Child = value; }
            get { return _Overlay.Children[0]; }
            set { _Overlay.Children[0] = value; }
        }

        public static CustomCursor GetCustomCursor(DependencyObject obj)
        {
            return (CustomCursor)obj.GetValue(CustomCursorProperty);
        }

        public static void SetCustomCursor(DependencyObject obj, CustomCursor value)
        {
            obj.SetValue(CustomCursorProperty, value);
        }

        // Using a DependencyProperty as the backing store for CustomCursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomCursorProperty =
            DependencyProperty.RegisterAttached("CustomCursor", typeof(CustomCursor), typeof(CustomCursor), new PropertyMetadata(null, OnCustomCursorPropertyChanged));

        public static void OnCustomCursorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs evtArgs)
        {
            if (evtArgs.OldValue != null)
            {
                CustomCursor oldValue = evtArgs.OldValue as CustomCursor;
                if (d is FrameworkElement)
                {
                    FrameworkElement CursorHost = d as FrameworkElement;
                    CursorHost.ClearValue(FrameworkElement.CursorProperty);
                    CursorHost.Loaded -= new RoutedEventHandler(oldValue.CursorHost_Loaded);
                    CursorHost.Unloaded -= new RoutedEventHandler(oldValue.CursorHost_Unloaded);
                    oldValue._popup.IsOpen = false;
                    oldValue._popup.Child = null;
                    oldValue._Overlay.Children.Clear();
                    CursorHost.MouseMove -= new MouseEventHandler(oldValue.newValue_MouseMove);
                    CursorHost.MouseEnter -= new MouseEventHandler(oldValue.newValue_MouseEnter);
                    CursorHost.MouseLeave -= new MouseEventHandler(oldValue.newValue_MouseLeave);
                    CursorHost.RemoveHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(oldValue.newValue_MouseLeftButtonDown));
                    CursorHost.RemoveHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(oldValue.newValue_MouseLeftButtonUp));
                }
            }
            if (evtArgs.NewValue != null)
            {
                CustomCursor newValue = evtArgs.NewValue as CustomCursor;
                if (d is FrameworkElement)
                {
                    FrameworkElement CursorHost = d as FrameworkElement;
                    newValue._source = CursorHost;
                    CursorHost.Loaded += new RoutedEventHandler(newValue.CursorHost_Loaded);
                    CursorHost.Unloaded += new RoutedEventHandler(newValue.CursorHost_Unloaded);
                    CursorHost.MouseMove += new MouseEventHandler(newValue.newValue_MouseMove);
                    CursorHost.MouseEnter += new MouseEventHandler(newValue.newValue_MouseEnter);
                    CursorHost.MouseLeave += new MouseEventHandler(newValue.newValue_MouseLeave);
                    CursorHost.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(newValue.newValue_MouseLeftButtonDown), true);
                    CursorHost.AddHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(newValue.newValue_MouseLeftButtonUp), true);
                }
            }
        }

        void CursorHost_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_popup != null)
            {
                _popup.Visibility = Visibility.Collapsed;
            }
        }

        void CursorHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (_popup != null)
            {
                _popup.Visibility = Visibility.Visible;
            }
        }
        
        void newValue_MouseMove(object sender, MouseEventArgs e)
        {
            UpdatePopup(e.GetPosition(null), e);
        }

        void newValue_MouseEnter(object sender, MouseEventArgs e)
        {
            _MouseEnter = true;
            UpdatePopup(e.GetPosition(null), e);
        }

        void newValue_MouseLeave(object sender, MouseEventArgs e)
        {
            _MouseEnter = false;
            UpdatePopup(e.GetPosition(null), e);
        }

        void newValue_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _MouseDown = false;
            UpdatePopup(e.GetPosition(null), e);
            _IsPortDraggging = false;
        }

        void newValue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (UIElement ele in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), _source))
            {
                if ((ele is Resizer) || (ele is ConnectionPort) ||
                    ((ele is TextBox) && ((ele as TextBox).Name == "PART_TextBox"))
                    )
                {
                    _MouseDown = false;
                    _IsPortDraggging = true;
                    return;
                }
            }
            _MouseDown = true;
        }
        
        public CustomCursor(UIElement Content)
        {
            _popup = new Popup();
            _Overlay = new Canvas();
            _popup.Child = _Overlay;
            _Overlay.Children.Add(new Canvas());
            this.Content = Content;
        }
        
        internal void UpdatePopup(Point Position, MouseEventArgs e)
        {
            bool setNone = true;
            if ((_source as Node).dview !=null && !(_source as Node).dview.IsPageEditable)
            {
                setNone = false;
            }
            else if (!_MouseDown)
            {
                foreach (UIElement ele in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), _source))
                {
                    if (((ele is FrameworkElement) && !(ele is Node) && ((ele as FrameworkElement).Cursor != null)) ||
                        ((ele is Resizer) || (ele is ConnectionPort)))
                    {
                        setNone = false;
                        break;
                    }
                    else if ((ele is Thumb) && ((ele as Thumb).Name == "PART_Rotator"))
                    {
                        //_popup.Child = _Rotate;
                        _Overlay.Children[0] = _Rotate;
                        break;
                    }
                    //else if (_popup.Child != _Move)
                    else if (_Overlay.Children[0] != _Move)
                    {
                        //_popup.Child = _Move;
                        _Overlay.Children[0] = _Move;
                    }
                }
            }
            if (e.OriginalSource == null || _IsPortDraggging)
            {
                setNone = false;
            }

            if (setNone && (_MouseEnter || _MouseDown))
            {
                //Canvas.SetLeft(_popup, Position.X);
                //Canvas.SetTop(_popup, Position.Y);
                Canvas.SetLeft(_Overlay.Children[0] as UIElement, Position.X);
                Canvas.SetTop(_Overlay.Children[0] as UIElement, Position.Y);
                _source.Cursor = Cursors.None;
                //_popup.IsOpen = false;
                _popup.IsOpen = true;
            }
            else
            {
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _source.ClearValue(FrameworkElement.CursorProperty);
                }
            }
        }
    }
}
