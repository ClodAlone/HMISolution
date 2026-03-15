using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Windows;
using OPCUAViewModel;
using System.ComponentModel;
using System.Windows.Threading;
#if !WINDOWS_UWP
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI;
#endif
using System.Collections.ObjectModel;
using Utilities;

namespace Display
{
    public class ContentCollection : ObservableCollection<FrameworkElement>
    {
    }

    public enum ContentMappingMode
    {
        Absolute,
        RelativeToGaugeSize
    }

    public enum SizingMode
    {
        Scaled,
        Explicit
    }

    public class Gauge : Control, IDisposable
    {
        protected FrameworkElement canvasBackground;
        protected FrameworkElement canvasCover;
        protected Canvas canvasElements;
        protected FrameworkElement canvasFrame;
        protected Canvas canvasInnerContent;
        protected Canvas canvasRoot;
        protected bool templateApplied;
        protected MonitoredItemViewModel monitoredItemViewModel;

        Opc.Ua.DataValue lastDataValue;
        DispatcherOperation dpUpdateWarning;
        object lockObject = new object();

        public static readonly DependencyProperty BackgroundVisibleProperty = DependencyProperty.Register("BackgroundVisible", typeof(Visibility), typeof(Gauge), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(Gauge.OnBackgroundVisibleChanged)));
        public static readonly DependencyProperty CoverVisibleProperty = DependencyProperty.Register("CoverVisible", typeof(Visibility), typeof(Gauge), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(Gauge.OnCoverVisibleChanged)));
        public static readonly DependencyProperty EnableAnimationProperty = DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(Gauge), new PropertyMetadata(true, new PropertyChangedCallback(Gauge.OnEnableAnimationChanged)));
        public static readonly DependencyProperty FrameVisibleProperty = DependencyProperty.Register("FrameVisible", typeof(Visibility), typeof(Gauge), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(Gauge.OnFrameVisibleChanged)));
        public static readonly DependencyProperty InnerContentMappingModeProperty = DependencyProperty.RegisterAttached("InnerContentMappingMode", typeof(ContentMappingMode), typeof(Gauge), new PropertyMetadata(ContentMappingMode.Absolute));
        protected ContentCollection innerElements = new ContentCollection();
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(double), typeof(Gauge), new PropertyMetadata(0.0));
        public static readonly DependencyProperty SizingMethodProperty = DependencyProperty.RegisterAttached("SizingMethod", typeof(SizingMode), typeof(Gauge), new PropertyMetadata(SizingMode.Scaled));
        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached("Top", typeof(double), typeof(Gauge), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(Gauge), new PropertyMetadata(0.0, new PropertyChangedCallback(Gauge.OnValueChanged)));

        public Gauge()
        {
            this.innerElements.CollectionChanged += new NotifyCollectionChangedEventHandler(this.innerElements_CollectionChanged);
            base.SizeChanged += new SizeChangedEventHandler(this.Gauge_SizeChanged);

            DataContextChanged += (o, e) =>
            {
                if (DataContext is MonitoredItemViewModel && !DesignerProperties.GetIsInDesignMode(this))
                {
                    if (monitoredItemViewModel != null)
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    if (monitoredItemViewModel != null)
                    {
                        monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                        monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                    }
                }
            };
        }

        private void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue")
            {
                var m = (MonitoredItemViewModel)sender;

                if (m.IsReadOnly)
                    return;

                bool bForceDispatcherOperation = false;
                lock (lockObject)
                {
                    bForceDispatcherOperation = lastDataValue == null;
                    lastDataValue = m.DataValue;
                }

                if (dpUpdateWarning == null || bForceDispatcherOperation || 
                    dpUpdateWarning.Status == DispatcherOperationStatus.Completed ||
                    dpUpdateWarning.Status == DispatcherOperationStatus.Aborted)
                {
                    dpUpdateWarning = Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(this, () =>
                    {
                        if (bDispose)
                            return;

                        ManageWarning();
                    });
                }
            }
        }

        protected virtual void ArrangeElement(FrameworkElement element)
        {
            Size layoutSize = this.GetLayoutSize(this);
            double num = this.ControlWidth / this.GetLayoutSize(element).Width;
            double num2 = this.ControlHeight / this.GetLayoutSize(element).Height;
            double num3 = Math.Min(num, num2);
            ScaleTransform transform = new ScaleTransform
            {
                ScaleX = num3,
                ScaleY = num3
            };
            element.RenderTransform = transform;
            if (num < num2)
            {
                element.SetValue(Canvas.TopProperty, Math.Abs((double)((this.GetLayoutSize(element).Height * num3) - layoutSize.Height)) / 2.0);
                element.SetValue(Canvas.LeftProperty, 0.0);
            }
            else
            {
                element.SetValue(Canvas.TopProperty, 0.0);
                element.SetValue(Canvas.LeftProperty, Math.Abs((double)((this.GetLayoutSize(element).Width * num3) - layoutSize.Width)) / 2.0);
            }
        }

        protected virtual void ArrangeInnerElement(FrameworkElement element)
        {
            double num3;
            double num4;
            double top = GetTop(element);
            double left = GetLeft(element);
            if (GetInnerContentMappingMode(element) == ContentMappingMode.RelativeToGaugeSize)
            {
                num3 = (left * base.Width) + this.GetHorizontalOffset(element);
                num4 = (top * base.Height) + this.GetVerticalOffset(element);
            }
            else
            {
                num3 = left + this.GetHorizontalOffset(element);
                num4 = top + this.GetVerticalOffset(element);
            }
            element.SetValue(Canvas.TopProperty, num4);
            element.SetValue(Canvas.LeftProperty, num3);
        }

        protected virtual void Draw()
        {
            if (this.canvasBackground != null)
            {
                this.canvasBackground.Visibility = this.BackgroundVisible;
                this.ArrangeElement(this.canvasBackground);
            }
            if (this.canvasFrame != null)
            {
                this.canvasFrame.Visibility = this.FrameVisible;
                this.ArrangeElement(this.canvasFrame);
                //if (this.canvasFrame is Border)
                //{
                //    (this.canvasFrame as Border).BorderThickness = this.ReadLocalValue(BorderThicknessProperty) != DependencyProperty.UnsetValue ? new Thickness(4) : this.BorderThickness;
                //    this.ArrangeElement(this.canvasFrame);
                //    (this.canvasFrame as Border).BorderBrush = this.ReadLocalValue(BorderBrushProperty) != DependencyProperty.UnsetValue ? new SolidColorBrush(Colors.Black) : this.BorderBrush;
                //    this.ArrangeElement(this.canvasFrame);
                //}
            }
            if (this.canvasCover != null)
            {
                this.canvasCover.Visibility = this.CoverVisible;
                this.ArrangeElement(this.canvasCover);
            }
            foreach (FrameworkElement element in this.canvasInnerContent.Children)
            {
                this.ArrangeInnerElement(element);
            }
        }

        private void element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ArrangeInnerElement(sender as FrameworkElement);
        }

        private void Gauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Draw();
        }

        protected double GetHorizontalOffset(FrameworkElement fe)
        {
            double actualWidth = fe.ActualWidth;
            if (double.IsNaN(actualWidth) || (actualWidth == 0.0))
            {
                actualWidth = fe.Width;
            }
            if (double.IsNaN(actualWidth))
            {
                actualWidth = 0.0;
            }
            if (fe.HorizontalAlignment == HorizontalAlignment.Center)
            {
                return ((actualWidth / 2.0) * -1.0);
            }
            if ((fe.HorizontalAlignment != HorizontalAlignment.Left) && (fe.HorizontalAlignment == HorizontalAlignment.Right))
            {
                return (actualWidth * -1.0);
            }
            return 0.0;
        }

        public static ContentMappingMode GetInnerContentMappingMode(DependencyObject o)
        {
            return (ContentMappingMode)o.GetValue(InnerContentMappingModeProperty);
        }

        protected Size GetLayoutSize(FrameworkElement fe)
        {
            double width;
            double height;
            if (!double.IsNaN(fe.Width) && !double.IsInfinity(fe.Width))
            {
                width = fe.Width;
            }
            else if ((fe.ActualWidth == 0.0) && (fe.DesiredSize.Width > 0.0))
            {
                width = fe.DesiredSize.Width;
            }
            else
            {
                width = fe.ActualWidth;
            }
            if (!double.IsNaN(fe.Height) && !double.IsInfinity(fe.Height))
            {
                height = fe.Height;
            }
            else if ((fe.ActualHeight == 0.0) && (fe.DesiredSize.Height > 0.0))
            {
                height = fe.DesiredSize.Height;
            }
            else
            {
                height = fe.ActualHeight;
            }
            return new Size(width, height);
        }

        public static double GetLeft(DependencyObject o)
        {
            return (double)o.GetValue(LeftProperty);
        }

        public static SizingMode GetSizingMethod(DependencyObject o)
        {
            return (SizingMode)o.GetValue(SizingMethodProperty);
        }

        public static double GetTop(DependencyObject o)
        {
            return (double)o.GetValue(TopProperty);
        }

        protected double GetVerticalOffset(FrameworkElement fe)
        {
            double actualHeight = fe.ActualHeight;
            if (double.IsNaN(actualHeight) || (actualHeight == 0.0))
            {
                actualHeight = fe.Height;
            }
            if (double.IsNaN(actualHeight))
            {
                actualHeight = 0.0;
            }
            if (fe.VerticalAlignment == VerticalAlignment.Center)
            {
                return ((actualHeight / 2.0) * -1.0);
            }
            if ((fe.VerticalAlignment != VerticalAlignment.Top) && (fe.VerticalAlignment == VerticalAlignment.Bottom))
            {
                return (actualHeight * -1.0);
            }
            return 0.0;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
#if !WINDOWS_UWP
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.canvasRoot = base.GetTemplateChild("LayoutRoot") as Canvas;
            this.canvasBackground = base.GetTemplateChild("Background") as FrameworkElement;
            this.canvasFrame = base.GetTemplateChild("Frame") as FrameworkElement;
            this.canvasCover = base.GetTemplateChild("Cover") as FrameworkElement;
            this.canvasElements = base.GetTemplateChild("Elements") as Canvas;
            this.canvasInnerContent = base.GetTemplateChild("AdditionalElements") as Canvas;
            if (this.canvasRoot != null)
            {
                foreach (FrameworkElement element in this.innerElements)
                {
                    DetachFromVisualTree(element);
                    this.canvasInnerContent.Children.Add(element);
                }
                this.templateApplied = true;
            }
        }

        public static void DetachFromVisualTree(FrameworkElement element)
        {
            if (element != null)
            {
                FrameworkElement parent = element.Parent as FrameworkElement;
                if (parent == null)
                {
                    parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
                }
                if (parent != null)
                {
                    if (parent is Panel)
                    {
                        (parent as Panel).Children.Remove(element);
                    }
                    else if (parent is Border)
                    {
                        (parent as Border).Child = null;
                    }
                    else if (parent is ContentPresenter)
                    {
                        (parent as ContentPresenter).Content = null;
                    }
                    else if (parent is ContentControl)
                    {
                        (parent as ContentControl).Content = null;
                    }
                }
            }
        }

        private void innerElements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (FrameworkElement element in e.OldItems)
                {
                    element.SizeChanged -= new SizeChangedEventHandler(this.element_SizeChanged);
                    if (this.canvasInnerContent != null)
                    {
                        this.canvasInnerContent.Children.Remove(element);
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (FrameworkElement element2 in e.NewItems)
                {
                    element2.SizeChanged += new SizeChangedEventHandler(this.element_SizeChanged);
                    if (this.canvasInnerContent != null)
                    {
                        this.canvasInnerContent.Children.Add(element2);
                    }
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.canvasFrame == null)
            {
                return base.MeasureOverride(availableSize);
            }
            Size layoutSize = this.GetLayoutSize(this.canvasFrame);
            double num = (layoutSize.Height == 0.0) ? 0.0 : (layoutSize.Width / layoutSize.Height);
            if (num == 0.0)
            {
                return new Size(0.0, 0.0);
            }
            double width = availableSize.Width;
            double height = availableSize.Height;
            if (double.IsNaN(base.Width) && (base.HorizontalAlignment != HorizontalAlignment.Stretch))
            {
                double num4 = !double.IsNaN(base.Height) ? base.Height : availableSize.Height;
                width = ((num4 * num) <= availableSize.Width) ? (num4 * num) : availableSize.Width;
            }
            if (double.IsNaN(base.Height) && (base.VerticalAlignment != VerticalAlignment.Stretch))
            {
                double num5 = !double.IsNaN(base.Width) ? base.Width : availableSize.Width;
                height = ((num5 / num) <= availableSize.Height) ? (num5 / num) : availableSize.Height;
            }
            return new Size(width, height);
        }

        private static void OnCoverVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Gauge)o).OnCoverVisibleChanged((Visibility)e.OldValue, (Visibility)e.NewValue);
        }

        protected virtual void OnCoverVisibleChanged(Visibility oldValue, Visibility newValue)
        {
            if (this.canvasCover != null)
            {
                this.canvasCover.Visibility = newValue;
            }
        }

        private static void OnBackgroundVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Gauge)o).OnBackgroundVisibleChanged((Visibility)e.OldValue, (Visibility)e.NewValue);
        }
        protected virtual void OnBackgroundVisibleChanged(Visibility oldValue, Visibility newValue)
        {
            if (this.canvasBackground != null)
            {
                this.canvasBackground.Visibility = this.BackgroundVisible;
                this.ArrangeElement(this.canvasBackground);
            }
        }
        protected virtual void OnEnableAnimationChanged(bool oldValue, bool newValue)
        {
        }

        private static void OnEnableAnimationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Gauge)o).OnEnableAnimationChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnEnablePopupsChanged(bool oldValue, bool newValue)
        {
        }

        private static void OnEnablePopupsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Gauge)o).OnEnablePopupsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private static void OnFrameVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Gauge)o).OnFrameVisibleChanged((Visibility)e.OldValue, (Visibility)e.NewValue);
        }

        protected virtual void OnFrameVisibleChanged(Visibility oldValue, Visibility newValue)
        {
            if (this.canvasFrame != null)
            {
                this.canvasFrame.Visibility = newValue;
            }
        }

        protected virtual void OnValueChanged(double oldValue, double newValue)
        { }

        private void ManageWarning()
        {
            if (this.canvasRoot != null)
            {
                Opc.Ua.DataValue dataValue = null;
                lock (lockObject)
                {
                    dataValue = lastDataValue;
                    lastDataValue = null;
                }

                if (dataValue != null && Opc.Ua.StatusCode.IsBad(dataValue.StatusCode))
                {
                    this.canvasRoot.ToolTip = Properties.Resources.NullValueWarning;
                    this.canvasElements.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.canvasRoot.ToolTip = null;
                    this.canvasElements.Visibility = Visibility.Visible;
                }
            }
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Gauge)o).OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        public static void SetInnerContentMappingMode(DependencyObject o, ContentMappingMode value)
        {
            o.SetValue(InnerContentMappingModeProperty, value);
        }

        public static void SetLeft(DependencyObject o, double value)
        {
            o.SetValue(LeftProperty, value);
        }

        public static void SetSizingMethod(DependencyObject o, SizingMode value)
        {
            o.SetValue(SizingMethodProperty, value);
        }

        public static void SetTop(DependencyObject o, double value)
        {
            o.SetValue(TopProperty, value);
        }

        #region IDisposable
        bool bDispose;
        public virtual void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;

            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            if (dpUpdateWarning != null &&
                dpUpdateWarning.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateWarning.Status != DispatcherOperationStatus.Completed)
                dpUpdateWarning.Abort();
        }
        #endregion

        public Visibility BackgroundVisible
        {
            get
            {
                return (Visibility)base.GetValue(BackgroundVisibleProperty);
            }
            set
            {
                base.SetValue(BackgroundVisibleProperty, value);
            }
        }

        internal double ControlHeight
        {
            get
            {
                if (!double.IsNaN(base.Height))
                {
                    return base.Height;
                }
                return base.ActualHeight;
            }
        }

        internal double ControlWidth
        {
            get
            {
                if (!double.IsNaN(base.Width))
                {
                    return base.Width;
                }
                return base.ActualWidth;
            }
        }

        public Visibility CoverVisible
        {
            get
            {
                return (Visibility)base.GetValue(CoverVisibleProperty);
            }
            set
            {
                base.SetValue(CoverVisibleProperty, value);
            }
        }

        public bool EnableAnimation
        {
            get
            {
                return (bool)base.GetValue(EnableAnimationProperty);
            }
            set
            {
                base.SetValue(EnableAnimationProperty, value);
            }
        }

        public Visibility FrameVisible
        {
            get
            {
                return (Visibility)base.GetValue(FrameVisibleProperty);
            }
            set
            {
                base.SetValue(FrameVisibleProperty, value);
            }
        }

        internal double GaugeHeight { get; set; }

        internal double GaugeWidth { get; set; }

        public ContentCollection InnerContent
        {
            get
            {
                return this.innerElements;
            }
        }

        public double Value
        {
            get
            {
                return (double)base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }
    }
}
