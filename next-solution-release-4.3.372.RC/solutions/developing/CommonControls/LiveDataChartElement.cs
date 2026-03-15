using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CommonControls
{
    public class LiveDataChartElement : FrameworkElement
    {
        const int BarWidth = 4;
        const int BarSpace = 1;
        const int SegmentHeight = 2;
        const int SegmentSpace = 1;
        const double MaxBarHeight = 19;
        public static readonly DependencyProperty CpuUsageHistoryProperty;
        public static readonly DependencyProperty BarBrushesProperty;
        static LiveDataChartElement()
        {
            CpuUsageHistoryProperty = DependencyProperty.Register("CpuUsageHistory", typeof(CpuUsageHistory), typeof(LiveDataChartElement), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCpuUsageHistoryChanged)));
            BarBrushesProperty = DependencyProperty.Register("BarBrushes", typeof(IList), typeof(LiveDataChartElement), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        }
        static void OnCpuUsageHistoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LiveDataChartElement)d).OnCpuUsageHistoryChanged((CpuUsageHistory)e.OldValue);
        }
        public CpuUsageHistory CpuUsageHistory
        {
            get { return (CpuUsageHistory)GetValue(CpuUsageHistoryProperty); }
            set { SetValue(CpuUsageHistoryProperty, value); }
        }
        public IList BarBrushes
        {
            get { return (IList)GetValue(BarBrushesProperty); }
            set { SetValue(BarBrushesProperty, value); }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            //drawingContext.DrawRectangle(Brushes.Gray, null, new Rect(0, 0, ActualWidth, ActualHeight));
            if (CpuUsageHistory == null || BarBrushes == null)
                return;
            int barCount = (int)Math.Floor((ActualWidth - BarWidth) / (BarWidth + BarSpace) + 1);
            int startHitoryIndex = Math.Max(0, CpuUsageHistory.Count - barCount);
            for (int i = startHitoryIndex; i < CpuUsageHistory.Count; i++)
            {
                int placeIndex = i - startHitoryIndex;
                double height = Math.Round(MaxBarHeight * (CpuUsageHistory[i] / 100d));
                int segmentCount = (int)Math.Floor((height - SegmentHeight) / (SegmentHeight + SegmentSpace) + 1);
                for (int j = 0; j < segmentCount; j++)
                {
                    Rect rect = new Rect(placeIndex * (BarWidth + BarSpace), ActualHeight - j * (SegmentHeight + SegmentSpace), BarWidth, SegmentHeight);
                    drawingContext.DrawRectangle((Brush)BarBrushes[j], null, rect);
                }
            }
        }
        void OnCpuUsageHistoryChanged(CpuUsageHistory oldValue)
        {
            if (oldValue != null)
                oldValue.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCpuUsageHistoryCollectionChanged);
            if (CpuUsageHistory != null)
                CpuUsageHistory.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCpuUsageHistoryCollectionChanged);
            InvalidateVisual();
        }

        void OnCpuUsageHistoryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateVisual();
        }
    }
    public class CpuUsageHistory : ObservableCollection<double>
    {
        public void AddHistoryValue(double value)
        {
            Add(value);
            if (Count > 100)
                RemoveAt(0);
        }
    }
}
