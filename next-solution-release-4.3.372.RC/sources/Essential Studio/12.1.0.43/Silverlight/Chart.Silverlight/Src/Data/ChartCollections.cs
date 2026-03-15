#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartScaleBreakCollection
    /// </summary>
    public class ChartScaleBreaksCollection : ObservableCollection<ChartScaleBreak>
    {
    }

    /// <summary>
    /// Class implementation for ChartStripLinesCollection 
    /// </summary>
    public class ChartStripLinesCollection : ObservableCollection<ChartStripLine>
    {
    }

    /// <summary>
    /// Class implementation for AnnotationsCollection
    /// </summary>
    public class AnnotationsCollection : ObservableCollection<ChartSeriesAnnotation>
    {

    }

    /// <summary>
    /// Class implementation for ChartAnnotationLabelsCollection
    /// </summary>
    public class ChartAnnotationLabelsCollection : ObservableCollection<ChartAnnotationLabel>
    {
    }
  

    /// <summary>
    /// Class implementation for areasCollection
    /// </summary>
    public class AreasCollection : ObservableCollection<ChartArea>
    {
        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The method is being called in a <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.PropertyChanged"/> or <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event handler. </exception>
        protected override void ClearItems()
        {
            if (this != null)
            {
                foreach (ChartArea area in this)
                {
                    if (area != null)
                    {
                        if (area.stripLinePanel != null)
                        {
                            if (area.stripLinePanel.Area != null && area.stripLinePanel.Area.InteractiveCursors != null)
                            {
                                area.stripLinePanel.Area.InteractiveCursors.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(area.stripLinePanel.Area.InteractiveCursorCollections_CollectionChanged);
                                area.stripLinePanel.Area.InteractiveCursors.Clear();
                                area.stripLinePanel.Area.InteractiveCursors = null;
                            }
                        }
                        if (area.InteractiveCursors != null)
                        {
                            area.InteractiveCursors.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(area.InteractiveCursorCollections_CollectionChanged);
                            area.InteractiveCursors.Clear();
                            area.InteractiveCursors = null;
                        }
                    }
                }
            }
            base.ClearItems();
        }
    }

    /// <summary>
    /// Called when instance created for InteractiveCursorCollection
    /// </summary>
    public class InteractiveCursorCollection : ObservableCollection<InteractiveCursor>
    {
    }

    /// <summary>
    /// Class implementation for Series Collection
    /// </summary>
    public class SeriesCollection : ObservableCollection<ChartSeries>
    {
        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided event data.
        /// </summary>
        /// <param name="e">The event data to report in the event.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The method is being called in a <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.PropertyChanged"/> or <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event handler. </exception>
        protected override void ClearItems()
        {
            foreach (ChartSeries series in this)
            {
                if (series.XAxis != null && series.XAxis.axisBindedSeriesList != null)
                {
                    series.XAxis.axisBindedSeriesList.Remove(series);
                }

                if (series.YAxis != null && series.YAxis.axisBindedSeriesList != null)
                {
                    series.YAxis.axisBindedSeriesList.Remove(series);
                }
            }
            base.ClearItems();
        }
    }
    /// <summary>
    /// Class implementation for AxesCollection 
    /// </summary>
    public class AxesCollection : ObservableCollection<ChartAxis>
    {
        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided event data.
        /// </summary>
        /// <param name="e">The event data to report in the event.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }
    }

    /// <summary>
    /// Class implementation  for ChartPointsCollection Class
    /// </summary>
    [TypeConverter(typeof(ChartListDataConveter))]
    public class ChartPointsCollection : ObservableCollection<ChartPoint>
    {
        internal ChartSeries series = null;

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert.</param><exception cref="T:System.InvalidOperationException">The method is being called in a <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.PropertyChanged"/> or <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event handler.</exception>
        protected override void InsertItem(int index, ChartPoint item)
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
            }

            if (series!=null && series.isdatasourcedata == false && series.DataSource != null)
            {
                return;
            }

            base.InsertItem(index, item);
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (series != null && series.Area != null)
            {
                 series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided event data.
        /// </summary>
        /// <param name="e">The event data to report in the event.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                if (series != null && series.Area != null)
                {
                    series.Area.LoadArea();
                }
            }
        }
    }

    /// <summary>
    /// Class implementation for SegmentsCollection
    /// </summary>
    public class SegmentsCollection : ObservableCollection<Segment>
    {
    }
    /// <summary>
    /// Class implementation  for ChartAreaTemplates
    /// </summary>
    public class ChartAreaTemplates : ObservableCollection<ControlTemplate>
    {
    }

    /// <summary>
    /// Class implementation for ChartAxisLabelsCollection
    /// </summary>
    public class ChartAxisLabelsCollection : ObservableCollection<ChartAxisLabel>
    {
        /// <summary>
        /// ChartAxisLabelsCollection Clear Items
        /// </summary>    
        protected override void ClearItems()
        {
            foreach (ChartAxisLabel label in this)
            {
                label.Dispose();
            }

            base.ClearItems();
        }
    }
    /// <summary>
    /// Class implementation for indicatorCollection
    /// </summary>
    public class IndicatorCollection : DependencyObject, INotifyPropertyChanged
    {
        #region Members
        /// <summary>
        /// Declares m_annotationsCollection
        /// </summary>
        internal ObservableCollection<ChartTechnicalIndicator> m_IndicatorCollection;
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies the AnnotationsTemplate dependency property.
        /// </summary>
        internal static readonly DependencyProperty IndicatorTemplateProperty =
            DependencyProperty.Register("IndicatorTemplate", typeof(DataTemplate), typeof(IndicatorCollection), new PropertyMetadata(null));


        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the AnnotationsTemplate. This is a dependency property.
        /// </summary>
        /// <value>The AnnotationsTemplate.</value>
        internal DataTemplate IndicatorTemplate
        {
            get { return (DataTemplate)GetValue(IndicatorTemplateProperty); }
            set { SetValue(IndicatorTemplateProperty, value); }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_IndicatorCollection.Count;
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>

        public ObservableCollection<ChartTechnicalIndicator> Items
        {
            get
            {
                return m_IndicatorCollection;
            }
        }
        #endregion

        /// <summary>
        /// Called when instance created for IndicatorCollection
        /// </summary>
        public IndicatorCollection()
        {
            m_IndicatorCollection = new ObservableCollection<ChartTechnicalIndicator>();
            m_IndicatorCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(m_IndicatorCollection_CollectionChanged);
        }

        void m_IndicatorCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
        


        #region Implemantation

        internal void Dispose()
        {
            if (this.m_IndicatorCollection != null)
            {
                this.m_IndicatorCollection.Clear();
                this.m_IndicatorCollection = null;
            }
            this.IndicatorTemplate = null;
        }
        
        
        #endregion
#pragma warning disable 0067
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Initializes PropertyChanged
        /// </summary>    
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
#pragma warning restore 0067
    }

}
