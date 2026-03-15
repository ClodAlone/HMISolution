#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Syncfusion.UI.Xaml.Grid
{
    public delegate Visual CreateContentFunction();

    public class BackgroundVisualHost : FrameworkElement
    {
        #region Private Members
        private ThreadedVisualHelper threadHelper = null;
        private HostVisual hostVisual = null; 
        #endregion

        #region IsIndicatorShowingProperty
        /// <summary>
        /// Identifies the InIndicatorShowing dependency property.
        /// </summary>
        public static readonly DependencyProperty IsIndicatorShowingProperty = DependencyProperty.Register(
            "IsIndicatorShowing",
            typeof(bool),
            typeof(BackgroundVisualHost),
            new FrameworkPropertyMetadata(false, OnInIndicatorShowingChanged));

        /// <summary>
        /// Gets or sets if the content is being displayed.
        /// </summary>
        public bool IsIndicatorShowing
        {
            get { return (bool)GetValue(IsIndicatorShowingProperty); }
            set { SetValue(IsIndicatorShowingProperty, value); }
        }

        static void OnInIndicatorShowingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackgroundVisualHost bvh = (BackgroundVisualHost)d;

            if (bvh.CreateContent != null)
            {
                if ((bool)e.NewValue)
                {
                    bvh.CreateContentHelper();
                }
                else
                {
                    bvh.HideContentHelper();
                }
            }
        }
        #endregion

        #region CreateContent Property
        /// <summary>
        /// Identifies the CreateContent dependency property.
        /// </summary>
        public static readonly DependencyProperty CreateContentProperty = DependencyProperty.Register(
            "CreateContent",
            typeof(CreateContentFunction),
            typeof(BackgroundVisualHost),
            new FrameworkPropertyMetadata(OnCreateContentChanged));

        /// <summary>
        /// Gets or sets the function used to create the visual to display in a background thread.
        /// </summary>
        public CreateContentFunction CreateContent
        {
            get { return (CreateContentFunction)GetValue(CreateContentProperty); }
            set { SetValue(CreateContentProperty, value); }
        }

        static void OnCreateContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackgroundVisualHost bvh = (BackgroundVisualHost)d;

            if (bvh.IsIndicatorShowing)
            {
                bvh.HideContentHelper();
                if (e.NewValue != null)
                    bvh.CreateContentHelper();
            }
        } 
        #endregion

        protected override int VisualChildrenCount
        {
            get { return hostVisual != null ? 1 : 0; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (hostVisual != null && index == 0)
                return hostVisual;

            throw new IndexOutOfRangeException("index");
        }

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get 
            {
                if (hostVisual != null)
                    yield return hostVisual;
            }
        }

        private void CreateContentHelper()
        {
            threadHelper = new ThreadedVisualHelper(CreateContent, SafeInvalidateMeasure);
            hostVisual = threadHelper.HostVisual;
        }

        private void SafeInvalidateMeasure()
        {
            Dispatcher.BeginInvoke(new Action(InvalidateMeasure), DispatcherPriority.Loaded);
        }

        private void HideContentHelper()
        {
            if (threadHelper != null)
            {
                threadHelper.Exit();
                threadHelper = null;
                InvalidateMeasure();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (threadHelper != null)
                return threadHelper.DesiredSize;

            return base.MeasureOverride(availableSize);
        }

        private class ThreadedVisualHelper
        {
            private readonly HostVisual _hostVisual = null;
            private readonly AutoResetEvent _sync = 
                new AutoResetEvent(false);
            private readonly CreateContentFunction _createContent;
            private readonly Action _invalidateMeasure;

            public HostVisual HostVisual { get { return _hostVisual; } }
            public Size DesiredSize { get; private set; }
            private Dispatcher Dispatcher { get; set; }

            public ThreadedVisualHelper(
                CreateContentFunction createContent, 
                Action invalidateMeasure)
            {
                _hostVisual = new HostVisual();
                _createContent = createContent;
                _invalidateMeasure = invalidateMeasure;

                Thread backgroundUi = new Thread(CreateAndShowContent);
                backgroundUi.SetApartmentState(ApartmentState.STA);
                backgroundUi.Name = "BackgroundVisualHostThread";
                backgroundUi.IsBackground = true;
                backgroundUi.Start();

                _sync.WaitOne();
            }

            public void Exit()
            {
                Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
            }

            private void CreateAndShowContent()
            {
                Dispatcher = Dispatcher.CurrentDispatcher;
                VisualTargetPresentationSource source = 
                    new VisualTargetPresentationSource(_hostVisual);
                _sync.Set();
                source.RootVisual = _createContent();
                DesiredSize = source.DesiredSize;
                _invalidateMeasure();

                Dispatcher.Run();
                source.Dispose();
            }
        }
    }
}
