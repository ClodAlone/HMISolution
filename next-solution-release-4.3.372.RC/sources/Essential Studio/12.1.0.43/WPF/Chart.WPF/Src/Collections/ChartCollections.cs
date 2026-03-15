// <copyright file="ChartCollections.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Markup;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Collections;
using System.Windows.Controls;

    /// <summary>
    /// Represents chart series comparer.
    /// </summary>
    /// <seealso cref="ChartSeries"/>
    /// <exclude/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartSeriesComparerByY : IComparer<ChartSeries>
    {
        #region Implementation
        /// <summary>
        /// Compares the specified s1 with the specified s2.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Zero if ranges of x == y;
        /// <para/>
        /// -1 if ranges of x &lt; y;
        /// <para/>
        /// 1 if ranges of x &gt; y;
        /// </returns>
        public int Compare(ChartSeries x, ChartSeries y)
        {
            return x == y ? 0 :
              x.YRange.End > y.YRange.End ? 1 : -1;
        }
        #endregion
    }


        #region ChartIndexedDataPointByXComparer
        /// <summary>
        /// Class that provides comparison abilities for indexed data points.
        /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
        public class ChartIndexedDataPointByXComparer : Comparer<ChartIndexedDataPoint>
        {
            #region Members
            /// <summary>
            /// Initializes diff
            /// </summary>
            private double diff;
            #endregion

            #region Implementation
            /// <summary>
            /// Compares the specified p1 with the specified p2.
            /// </summary>
            /// <param name="point1">The point1.</param>
            /// <param name="point2">The point2.</param>
            /// <returns>
            /// negative value if point1 &lt; point2
            /// <para>
            /// zero if point1 = point2.
            /// </para>
            /// <para>
            /// positive value if point1 &gt; point2
            /// </para>
            /// </returns>
            public override int Compare(ChartIndexedDataPoint point1, ChartIndexedDataPoint point2)
            {
                diff = point1.DataPoint.X - point2.DataPoint.X;
                if (diff == 0)
                {
                    return 0;
                }

                return diff < 0 ? -1 : 1;
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Represents ChartDoubleComparer
        /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
        public class ChartDoubleComparer : IComparer<double>
        {
            #region Members
            /// <summary>
            /// Initializes m_inversed
            /// </summary>
            private bool m_inversed;
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartDoubleComparer"/> class.
            /// </summary>
            /// <param name="inversed">if set to <c>true</c> comparing is inversed.</param>
            public ChartDoubleComparer(bool inversed)
            {
                m_inversed = inversed;
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// Value Condition Less than zero x is less than y.Zero x equals y.Greater than zero x is greater than y.
            /// </returns>
            public int Compare(double x, double y)
            {
                if (x > y)
                {
                    return m_inversed ? -1 : 1;
                }
                else if (x < y)
                {
                    return m_inversed ? 1 : -1;
                }

                return 0;
            }
            #endregion
        }
   

    /// <summary>
    /// Represents AnnotationsCollection. Annotations at specific X-Y coordinates can be added to the chart programmatically.
    /// </summary>
    /// <example>
    /// XAML:
    /// <code language="XAML">
    ///  &lt;sfchart:ChartSeries Name="series1" Label="Series1" Type="Area"
    /// Interior="LightSkyBlue"&gt;
    /// &lt;sfchart:ChartSeries.Annotations&gt;
    /// &lt;sfchart:AnnotationsCollection LineColor="White" x:Uid="Annot"&gt;
    ///             &lt;!-- Here we define the look and feel of the annotation. --&gt;
    ///              &lt;sfchart:AnnotationsCollection.AnnotationsTemplate&gt;
    ///                  &lt;DataTemplate&gt;
    ///                      &lt;Button Content="{Binding Y}" ToolTip="{Binding
    /// Description}" Background="LightGray" Name="Button1" Click="Button_Click" /&gt;
    ///                  &lt;/DataTemplate&gt;
    ///              &lt;/sfchart:AnnotationsCollection.AnnotationsTemplate&gt;
    ///          &lt;/sfchart:AnnotationsCollection&gt;
    ///         &lt;!-- The annotations are added to this collection in code-behind --&gt;
    ///      &lt;/sfchart:ChartSeries.Annotations&gt;
    ///  &lt;/sfchart:ChartSeries&gt;
    /// </code>
    /// C#:
    /// <code language="C#">
    /// // Series1 annotations
    /// ChartSeriesAnnotation ser1LowPoint = new ChartSeriesAnnotation() { X = 1, Y =
    /// 20, Description = "Series 1 Low Point" }; 
    /// ChartSeriesAnnotation ser1HighPoint = new ChartSeriesAnnotation() { X = 7, Y =
    /// 56, Description = "Series 1 High Point" }; 
    /// this.Chart1.Areas[0].Series[0].Annotations.Items.Add(ser1LowPoint); 
    /// this.Chart1.Areas[0].Series[0].Annotations.Items.Add(ser1HighPoint);
    /// </code>
    /// </example>
    /// <seealso cref="ChartAnnotationLabel"/>
    [ContentProperty("Items")]
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class AnnotationsCollection : DependencyObject, INotifyPropertyChanged
    {
        #region Members
        /// <summary>
        /// Declares m_annotationsCollection
        /// </summary>
        public ObservableCollection<ChartSeriesAnnotation> m_annotationsCollection;
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies the AnnotationsTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty AnnotationsTemplateProperty =
            DependencyProperty.Register("AnnotationsTemplate", typeof(DataTemplate), typeof(AnnotationsCollection), new UIPropertyMetadata(null));

        /// <summary>       
        /// Identifies the LineColor dependency property.
        /// </summary>
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(AnnotationsCollection), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the IsRelative dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRelativeProperty =
            DependencyProperty.Register("IsRelative", typeof(bool), typeof(AnnotationsCollection), new UIPropertyMetadata(true));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether IsRelative value. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if annotations should be placed relatively to axis units; otherwise, <c>false</c>.
        /// </value>
        public bool IsRelative
        {
            get { return (bool)GetValue(IsRelativeProperty); }
            set { SetValue(IsRelativeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the LineColor. This is a dependency property.
        /// </summary>
        /// <value>The LineColor.</value>
        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the AnnotationsTemplate. This is a dependency property.
        /// </summary>
        /// <value>The AnnotationsTemplate.</value>
        public DataTemplate AnnotationsTemplate
        {
            get { return (DataTemplate)GetValue(AnnotationsTemplateProperty); }
            set { SetValue(AnnotationsTemplateProperty, value); }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_annotationsCollection.Count;
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]        
        public ObservableCollection<ChartSeriesAnnotation> Items
        {
            get
            {
                return m_annotationsCollection;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationsCollection"/> class.
        /// </summary>
        public AnnotationsCollection()
        {
            m_annotationsCollection = new ObservableCollection<ChartSeriesAnnotation>();
        }
        #endregion

        #region Implemantation

        internal void Dispose()
        {
            if (this.m_annotationsCollection != null)
            {
                this.m_annotationsCollection.Clear();
                this.m_annotationsCollection = null;
            }
            this.AnnotationsTemplate = null;
        }


        /// <summary>
        /// Called when the property changed.
        /// </summary>    
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(e.Property.Name));
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Initializes PropertyChanged
        /// </summary>    
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    /// <summary>
    /// Represents class implementation for IndicatorCollection
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
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
            DependencyProperty.Register("IndicatorTemplate", typeof(DataTemplate), typeof(IndicatorCollection), new UIPropertyMetadata(null));

        
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
        /// Called when instance created for IndicatorCollection Class
        /// </summary>
        public IndicatorCollection()
        {
            m_IndicatorCollection = new ObservableCollection<ChartTechnicalIndicator>();
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


        /// <summary>
        /// Called when the property changed.
        /// </summary>    
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(e.Property.Name));
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Initializes PropertyChanged
        /// </summary>    
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    /// <summary>
    /// Represents the chart striplines collection.
    /// </summary>
    /// <seealso cref="ChartArea"/>
    /// <exclude/>
    public class ChartStripLinesCollection : ObservableCollection<ChartStripLine>
    {
    }

    /// <summary>
    /// Represents the <see cref="ChartAxis">chart axes</see> collection.
    /// </summary>
    /// <seealso cref="ChartAxis"/>
    /// <exclude/>
    public class ChartAxesCollection : ObservableCollection<ChartAxis>
    {
    }

    /// <summary>
    /// Represents the <see cref="ChartToolBar"> ToolBar</see> collection.
    /// </summary>
    /// <seealso cref="ChartToolBar"/>
    public class ChartToolBarCollection : ObservableCollection<ChartToolBar>
    {
    }

    /// <summary>
    /// Represents the <see cref="ChartAxis">chart axis</see> labels collection. 
    /// </summary>
    /// <seealso cref="ChartAxis"/>
    /// <seealso cref="ChartSeries"/>
    /// <exclude/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAxisLabelsCollection : ObservableCollection<ChartAxisLabel>
    {
        internal ChartAxis m_chartAxis = null;

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert.</param>
        protected override void InsertItem(int index, ChartAxisLabel item)
        {
            item.Axis = m_chartAxis;
            base.InsertItem(index, item);
        }
        /// <summary>
        /// ChartAxisLabelsCollection Clear Items
        /// </summary>    
        /// <seealso cref="ChartAxisLabelsCollection"/>
        protected override void ClearItems()
        {
            foreach (ChartAxisLabel label in this)
            {
                if (label != null && label.m_isCustomLabel == false)
                {
                    label.Axis = null;
                    label.Dispose();
                }
            }
            //for (int i = 0; i < this.Count; i++)
            //{
            //    if(this[i] != null && this[i].m_isCustomLabel == false)
            //    {
            //        this[i].Axis = null;
            //        this[i].Dispose();
            //        //this[i] = null;                    
            //    }
            //}
            this.m_chartAxis = null;
            base.ClearItems();
        }
    }

    /// <summary>
    /// Represents the <see cref="ChartArea">chart areas</see> collection.  
    /// </summary>
    /// <seealso cref="ChartArea"/>
    /// <see cref="Chart"/>
    /// <exclude/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAreasCollection : ObservableCollection<ChartArea>
    {
        #region Constants
        /// <summary>
        /// Initializes C_emptyIndex
        /// </summary>
        private const int C_emptyIndex = -1;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Chart.ChartArea"/> with the specified name.
        /// </summary>
        /// <param name="name">The ChartArea aame</param>
        /// <returns>the index value</returns>
        public ChartArea this[string name]
        {
            get
            {
                int index = this.IndexOf(name);
                return index != C_emptyIndex ? this[index] : null;
            }

            set
            {
                int index = this.IndexOf(name);

                if (index != C_emptyIndex)
                {
                    this[index] = value;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Get index of <see cref="ChartSeries"/> by name.
        /// </summary>
        /// <param name="name">Series name</param>
        /// <returns>Index of series in collection.</returns>
        public int IndexOf(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }
        #endregion

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (Items[i].Series != null)
                {

                    int count = Items[i].Series.Count;
                    while (count > 0)
                    {
                        if (Items[i].Series[0] != null)
                        {
                            //Items[i].Legend.
                            //Items[i].Series.RemoveAt(0);
                             Items[i].Series.Clear();
                            count = Items[i].Series.Count;
                        }
                    }
                }
            }
            for (int i = 0; i < this.Items.Count; i++)
                {
                  Items.Clear();
                }
           // }
            base.ClearItems();
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }
    }

    /// <summary>
    /// Represents the <see cref="ChartLegend">chart legends</see> collection. 
    /// </summary>
    /// <seealso cref="Chart"/>
    /// <seealso cref="ChartArea"/>
    /// <seealso cref="ChartSeries"/>
    /// <seealso cref="ChartLegend"/>
    /// <exclude/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartLegendsCollection : ObservableCollection<ChartLegend>
    {
        #region Constants
        /// <summary>
        /// Initializes c_emptyIndex
        /// </summary>
        private const int C_emptyIndex = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="ChartLegend"/> by name.
        /// </summary>
        /// <param name="name">Legend's name.</param>
        /// <returns><see cref="ChartLegend"/>.</returns>
        public ChartLegend this[string name]
        {
            get
            {
                int index = this.IndexOf(name);
                return index != C_emptyIndex ? this[index] : null;
            }

            set
            {
                int index = this.IndexOf(name);

                if (index != C_emptyIndex)
                {
                    this[index] = value;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets index of <see cref="ChartLegend"/> by name.
        /// </summary>
        /// <param name="name">Legend's name.</param>
        /// <returns>Index of specified legend.</returns>
        public int IndexOf(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }
        #endregion
    }

    /// <summary>
    /// Represents the collection of <see cref="ChartSeries"/>.
    /// </summary>
    /// <seealso cref="ChartSeries"/>
    /// <seealso cref="ChartArea"/>
    /// <exclude/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartSeriesCollection : ObservableCollection<ChartSeries>
    {
        #region Constants
        /// <summary>
        /// Initializes c_emptyIndex
        /// </summary>
        private const int C_emptyIndex = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets <see cref="ChartSeries"/> by the name.
        /// </summary>
        /// <param name="name">Name of series.</param>
        /// <returns>-1 if legend's name cannoot be retrieved, otherwise, instance of <see cref="ChartSeries"/>.</returns>
        public ChartSeries this[string name]
        {
            get
            {
                int index = this.IndexOf(name);
                return index != C_emptyIndex ? this[index] : null;
            }

            set
            {
                int index = this.IndexOf(name);

                if (index != C_emptyIndex)
                {
                    this[index] = value;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets index of <see cref="ChartSeries"/> by name.
        /// </summary>
        /// <param name="name">Name of series</param>
        /// <returns>Index of specified series.</returns>
        public int IndexOf(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Adds range of <see cref="ChartSeries"/> to collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <seealso cref="ChartSeriesCollection"/>
        public void AddRange(IEnumerable<ChartSeries> collection)
        {
            foreach (ChartSeries series in collection)
            {
                this.Items.Add(series);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, series));
            }
        }
        #endregion

        #region Implementation

        internal void Dispose()
        {
            if (this.Items != null)
            {
                this.Items.Clear();
                this.ClearItems();
            }
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <seealso cref="ChartSeriesCollection"/>
        protected override void ClearItems()
        {
            foreach (ChartSeries series in this.Items)
            {
                ////Checking if collection is a member of area.
                if (series.Area != null && series.Area.Series == this)
                {
                    ////Clearing all segments of series.
                    series.Dispose();
                }
            }

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (Items[i].Area != null && Items[i].Area.Series == this)
                {
                    //Items[i].Dispose();
                    //Items[i] = null;
                    Items.Clear();
                }
            }

            base.ClearItems();

            //base.ClearItems();
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            //this.Items[index].Dispose();
            //SD14564- The below conditions are added to remove the axis(XAxis/YAxis) of corresponding series, if those axis is not Primary/Secondary axis of the Series's ChartArea
            if (index < this.Items.Count)
            {
                if (!this.Items[index].Area.SecondaryAxis.Equals(this.Items[index].YAxis))
                {
                    this.Items[index].Area.Axes.Remove(this.Items[index].YAxis);
                }
                if (!this.Items[index].Area.PrimaryAxis.Equals(this.Items[index].XAxis))
                {
                    this.Items[index].Area.Axes.Remove(this.Items[index].XAxis);
                }
            }
            base.RemoveItem(index);
        }

        #endregion
    }

    /// <summary>
    /// Class represents collection that is used as wrapper for <see cref="ChartArea.Series"/> 
    /// and contains currently visible series.
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class VisibleSeriesCollection : ObservableCollection<ChartSeries>
    {
        #region Members
        /// <summary>
        /// Declares m_series
        /// </summary>
        private ChartSeriesCollection m_series;

        /// <summary>
        /// Declares m_firstSeries
        /// </summary>
        private ChartSeries m_firstSeries;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleSeriesCollection"/> class.
        /// </summary>
        /// <param name="wrappedSeriesCollection">The wrapped series collection.</param>
        public VisibleSeriesCollection(ChartSeriesCollection wrappedSeriesCollection)
        {
            m_series = wrappedSeriesCollection;
            wrappedSeriesCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(SeriesCollectionChanged);
            Refresh();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Refreshes Visible series.
        /// </summary>
        public void Refresh()
        {
            ////TODO SMART REFRESH
            while (Count > 0)
            {
                RemoveAt(Count - 1);
            }
            double[] ZorderIndex_vals=new double[m_series.Count];
            List<double> ZorderIndex_vals_temp=new List<double>();            
            int i=0;
            foreach(ChartSeries series in m_series)
            {
                if (series != null)
                {
                    ZorderIndex_vals[i] = series.ZOrder;
                    ZorderIndex_vals_temp.Add(series.ZOrder);
                    i++;
                }
                              
            }
            m_firstSeries = null;            
            Array.Sort(ZorderIndex_vals); 
            List<ChartSeries> addedItems = new List<ChartSeries>();
            ////Here goes series filtering logic. 
            //foreach (ChartSeries series in m_series)
            for (int k = 0; k < m_series.Count;k++ )
                {
                    int temp = 0;
                    temp = ZorderIndex_vals_temp.IndexOf(ZorderIndex_vals[k]);                    
                    ZorderIndex_vals_temp[temp] = -1;
                   
                    ////Checking series for visibility.
                    if (m_series[temp].IsVisible)
                    {

                        if (m_firstSeries == null)
                        {
                            m_firstSeries = m_series[temp];
                            Add(m_series[temp]);                           
                        }
                        else
                        {
                            ////Checking series type for compatibility.
                            if (m_firstSeries.ChartType != null)
                            {
                                if ((m_firstSeries.ChartType.IsCompatible(m_series[temp].ChartType) && m_series[temp].ChartType.IsCompatible(m_firstSeries.ChartType)) || m_series[temp].Type == ChartTypes.Pie || m_series[temp].Type == ChartTypes.Doughnut)
                                {
                                    Add(m_series[temp]);
                                }
                            }
                        }
                    }                    
                }
             
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Series the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void SeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Determines whether the specified series is visible.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>
        /// <c>true</c> if the specified series is visible; otherwise, <c>false</c>.
        /// </returns>
        private bool IsVisible(ChartSeries series)
        {
            return m_firstSeries != null && series.IsVisible && m_firstSeries.ChartType.IsCompatible(series.ChartType) &&
                    series.ChartType.IsCompatible(m_firstSeries.ChartType);
        }
        #endregion
    }

    /// <summary>
    /// Represents the Chart Adornments collection class
    /// </summary>
    /// <seealso cref="ChartAdornment"/>
    /// <seealso cref="ChartAdornmentInfo"/>
    public class ChartAdornmentsCollection : ObservableCollection<ChartAdornment>
    {
    }

    /// <summary>
    /// Represents Chart Annotation Labels collection class. Annotation labels are used to add custom labels on Chart
    /// </summary>
    /// <seealso cref="ChartAnnotationLabel"/>
    public class ChartAnnotationLabelsCollection : ObservableCollection<ChartAnnotationLabel>
    {
        internal void Dispose()
        {
            foreach (ChartAnnotationLabel item in this)
            {
                item.Dispose();
            }
            if (this.Items != null)
            {
                this.Items.Clear();
            }
            this.ClearItems();
        }
    }

    /// <summary>
    /// Class implementation for TabItemCollection
    /// </summary>
    public class TabItemCollection : ObservableCollection<TabItem>
    {
        /// <summary>
        /// Constructor for TabItemCollection Class 
        /// </summary>
        public TabItemCollection()
        { 
        }
    }

    /// <summary>
    /// Represents FastSegmnetPropertiesCollection implementation
    /// </summary>
    public class FastSegmnetPropertiesCollection : ObservableCollection<FastSegmnetProperties>
    {
        internal bool IsUpdated { get; set; }

        internal int DataCount { get; set; }

    }

    /// <summary>
    /// Represents the InteractiveCursor Collection class
    /// </summary>
    /// <seealso cref="ChartAdornment"/>
    /// <seealso cref="ChartAdornmentInfo"/>
    public class InteractiveCursorCollection : ObservableCollection<InteractiveCursor>
    {

    }

    /// <summary>
    /// Represents SyncInteractiveCursor Collection class
    /// </summary>
    /// <seealso cref="ChartAdornment"/>
    /// <seealso cref="ChartAdornmentInfo"/>
    public class SyncInteractiveCursorCollection : ObservableCollection<SyncInteractiveCursor>
    {
    }
}
