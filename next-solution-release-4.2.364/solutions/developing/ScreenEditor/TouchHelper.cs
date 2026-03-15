using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI;
using DevExpress.Xpf.Editors.Flyout.Native;
using Utilities;

namespace ScreenManager
{
    public class TouchHelper : Behavior<AppBar>
    {
        public TouchPoint touchStart;
        readonly int SwipeGestureTreshold = 18;

        public double TouchAreaHeight
        {
            get { return (double)GetValue(TouchAreaHeightProperty); }
            set { SetValue(TouchAreaHeightProperty, value); }
        }
        public static readonly DependencyProperty TouchAreaHeightProperty =
            DependencyProperty.Register("TouchAreaHeight", typeof(double), typeof(TouchHelper), new PropertyMetadata(100d));

        protected override void OnAttached()
        {
            base.OnAttached();
            Touch.FrameReported += OnTouchFrameReported;

            if (AssociatedObject.IsLoaded)
            {
                DetachDefaultHandler();
                this.SubscribeEvents();
            }
            else
                AssociatedObject.Loaded += OnAssociatedObjectLoaded;

        }

        protected void UnsubscribeEvents()
        {
            if (this.frameworkElement != null)
            {
                this.frameworkElement.ContextMenuOpening -= AssociatedObject_ContextMenuOpening;
                this.frameworkElement.MouseLeftButtonUp -= frameworkElement_MouseLeftButtonUp;
                frameworkElement.TouchDown -= FrameworkElement_TouchDown;
            }
        }

        protected FrameworkElement frameworkElement;

        protected void SubscribeEvents()
        {
            UIElement uiElement = DevExpress.Xpf.Core.Native.LayoutHelper.FindRoot((DependencyObject)this.AssociatedObject, false) as UIElement;
            if (uiElement == null)
                return;
            frameworkElement = uiElement as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.ContextMenuOpening += AssociatedObject_ContextMenuOpening;
                frameworkElement.MouseLeftButtonUp += frameworkElement_MouseLeftButtonUp;
                frameworkElement.TouchDown += FrameworkElement_TouchDown;
            }
        }

        private void FrameworkElement_TouchDown(object sender, TouchEventArgs e)
        {
            DependencyObject dependencyObject = e.OriginalSource as DependencyObject;
            if (dependencyObject != null && !(dependencyObject is Visual))
                dependencyObject = LogicalTreeHelper.GetParent(dependencyObject);
            if (dependencyObject == null || DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<AppBar>(dependencyObject) != null || (FlyoutBase.GetFlyout(e.OriginalSource as DependencyObject) != null || this.AssociatedObject.HideMode != AppBarHideMode.Default) || this.AssociatedObject.HideMode == AppBarHideMode.Sticky)
                return;

            this.AssociatedObject.IsOpen = false;
        }

        void frameworkElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dependencyObject = e.OriginalSource as DependencyObject;
            if (dependencyObject != null && !(dependencyObject is Visual))
                dependencyObject = LogicalTreeHelper.GetParent(dependencyObject);
            if (dependencyObject == null || DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<AppBar>(dependencyObject) != null || (FlyoutBase.GetFlyout(e.OriginalSource as DependencyObject) != null || this.AssociatedObject.HideMode != AppBarHideMode.Default) || this.AssociatedObject.HideMode == AppBarHideMode.Sticky)
                return;

            this.AssociatedObject.IsOpen = false;
        }

        void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            DetachDefaultHandler();
            this.SubscribeEvents();
        }

        void AssociatedObject_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.AssociatedObject.HideMode == AppBarHideMode.AlwaysVisible && this.AssociatedObject.IsOpen)
                return;
            if (e.OriginalSource is Canvas)
                this.AssociatedObject.IsOpen = !this.AssociatedObject.IsOpen;
        }

        protected void DetachDefaultHandler()
        {
            FrameworkElement root = Window.GetWindow(AssociatedObject);
            if (root == null)
                root = DevExpress.Xpf.Core.Native.LayoutHelper.FindLayoutOrVisualParentObject<NavigationPage>(AssociatedObject, true);
            if (root == null)
                root = DevExpress.Xpf.Core.Native.LayoutHelper.FindLayoutOrVisualParentObject<UserControl>(AssociatedObject, true);

            if (root == null) return;
            var parent = DevExpress.Xpf.Core.Native.LayoutHelper.FindLayoutOrVisualParentObject(AssociatedObject, typeof(NavigationPage)) as UIElement ?? VisualTreeHelper.GetParent(AssociatedObject) as UIElement;
            if (parent == null) return;

            Type typeUIEventService = typeof(AppBar).Assembly.GetTypes().FirstOrDefault(t => t.Name == "UIEventService");
            var fieldOwnerMap = typeUIEventService.GetField("ownerMap", BindingFlags.NonPublic | BindingFlags.Static);
            var ownerMap = (IDictionary)fieldOwnerMap.GetValue(null);
            var serviceInstance = ownerMap[parent];
            if (serviceInstance == null) return;
            serviceInstance.GetType().GetMethod("RemoveListener").Invoke(serviceInstance, new object[] { AssociatedObject });
        }

        protected void UnsubscribeAppBar()
        {
            FrameworkElement root = Window.GetWindow(AssociatedObject);
            if (root == null)
                root = DevExpress.Xpf.Core.Native.LayoutHelper.FindLayoutOrVisualParentObject<NavigationPage>(AssociatedObject, true);
            if (root == null)
                root = DevExpress.Xpf.Core.Native.LayoutHelper.FindLayoutOrVisualParentObject<UserControl>(AssociatedObject, true);

            if (root == null) return;
            var parent = DevExpress.Xpf.Core.Native.LayoutHelper.FindLayoutOrVisualParentObject(AssociatedObject, typeof(NavigationPage)) as UIElement ?? VisualTreeHelper.GetParent(AssociatedObject) as UIElement;
            if (parent == null) return;

            Type typeUIEventService = typeof(AppBar).Assembly.GetTypes().FirstOrDefault(t => t.Name == "UIEventService");
            var methodUnsubscribe = typeUIEventService.GetMethod("Unsubscribe", BindingFlags.Public | BindingFlags.Static);
            methodUnsubscribe.Invoke(typeUIEventService, new object[] { AssociatedObject, parent });
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
            Touch.FrameReported -= OnTouchFrameReported;
            UnsubscribeEvents();
            UnsubscribeAppBar();
            base.OnDetaching();
        }

        protected virtual void OnTouchFrameReported(object sender, TouchFrameEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (AssociatedObject == null) return;

                var root = DevExpress.Xpf.Core.Native.LayoutHelper.GetRoot(AssociatedObject);
                if (root == null)
                    root = AssociatedObject;

                TouchPoint tp = e.GetPrimaryTouchPoint(root);
                if (tp == null) return;
                switch (tp.Action)
                {
                    case TouchAction.Down:
                        if (AssociatedObject.IsOpen) return;

                        var touch = tp;
                        switch (AssociatedObject.Alignment)
                        {
                            case AppBarAlignment.Top:
                                if (touch.Position.Y > 0 && touch.Position.Y < TouchAreaHeight)
                                {
                                    touchStart = touch;
                                    e.SuspendMousePromotionUntilTouchUp();
                                }
                                break;
                            case AppBarAlignment.Bottom:
                                if (touch.Position.Y > root.RenderSize.Height - TouchAreaHeight && touch.Position.Y < root.RenderSize.Height)
                                {
                                    touchStart = touch;
                                    e.SuspendMousePromotionUntilTouchUp();
                                }
                                break;
                            case AppBarAlignment.Left:
                                if (touch.Position.X > 0 && touch.Position.X < TouchAreaHeight)
                                {
                                    touchStart = touch;
                                    e.SuspendMousePromotionUntilTouchUp();
                                }
                                break;
                            case AppBarAlignment.Right:
                                if (touch.Position.X > root.RenderSize.Width - TouchAreaHeight && touch.Position.X < root.RenderSize.Width)
                                {
                                    touchStart = touch;
                                    e.SuspendMousePromotionUntilTouchUp();
                                }
                                break;
                        }

                        break;

                    case TouchAction.Move:
                        var Touch = tp;
                        if (touchStart != null)
                        {
                            switch (AssociatedObject.Alignment)
                            {
                                case AppBarAlignment.Top:
                                case AppBarAlignment.Bottom:
                                    {
                                        var move = touchStart.Position.Y - Touch.Position.Y;
                                        if (move * ((AssociatedObject.Alignment == AppBarAlignment.Top) ? -1 : 1) >= SwipeGestureTreshold)
                                            AssociatedObject.SetCurrentValue(AppBar.IsOpenProperty, true);
                                        break;
                                    }
                                case AppBarAlignment.Left:
                                case AppBarAlignment.Right:
                                    {
                                        var move = touchStart.Position.X - Touch.Position.X;
                                        if (move * ((AssociatedObject.Alignment == AppBarAlignment.Left) ? -1 : 1) >= SwipeGestureTreshold)
                                            AssociatedObject.SetCurrentValue(AppBar.IsOpenProperty, true);
                                        break;
                                    }
                            }
                        }
                        break;

                    case TouchAction.Up:
                        if (touchStart != null && AssociatedObject.IsOpen)
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                AssociatedObject.SetCurrentValue(AppBar.IsOpenProperty, true);
                            }), DispatcherPriority.Render);
                        //else
                        //    AssociatedObject.SetCurrentValue(AppBar.IsOpenProperty, false);
                        touchStart = null;

                        break;
                }
            }));
        }
    }
}
