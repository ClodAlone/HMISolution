// <copyright file="ChartAreaPresenter.cs" company="Syncfusion">
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
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Collections;
    using System.Linq;

    /// <summary>
    /// Class is used for template selection during changing axes type.
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAreaTemplateSelector : DataTemplateSelector
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Cartesian template.
        /// </summary>
        /// <value>The Cartesian template.</value>
        public DataTemplate CartesianTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the polar template.
        /// </summary>
        /// <value>The polar template.</value>
        public DataTemplate PolarTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the none template.
        /// </summary>
        /// <value>The none template.</value>
        public DataTemplate NoneTemplate
        {
            get;
            set;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"></see> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"></see> or null. The default value is null.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ChartArea area = item as ChartArea;
            DataTemplate result = null;
            if (area != null)
            {
                if (area.View3DMode == true)
                {

                    SharedResourceDictionary baseRD = new SharedResourceDictionary()
                    {
                        Source = new Uri("/Syncfusion.Chart.Wpf;component/3DChart/Chart3DGrid.xaml", UriKind.RelativeOrAbsolute)
                    };
                    if (baseRD != null)
                    {
                        result = (DataTemplate)baseRD["Chart3DTemplateKey"];
                    }

                }
                else
                {
                    switch (area.AreaType)
                    {
                        case ChartAxesType.None:
                            result = this.NoneTemplate;
                            break;
                        case ChartAxesType.CartesianAxes:
                            result = this.CartesianTemplate;
                            break;
                        case ChartAxesType.PolarAxes:
                            result = this.PolarTemplate;
                            break;
                    }
                }

            }
            return result;
        }
        #endregion
    }

    /// <summary>
    /// Class is a container for <see cref="ChartArea">chart areas</see>.
    /// </summary>
    /// <seealso cref="ChartAreasCollection"/>
    /// <seealso cref="ChartArea"/>
    /// <seealso cref="ChartAxis"/>
    /// <exclude/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [TemplatePart(Name = "PART_GridElement", Type = typeof(FrameworkElement)),
      TemplatePart(Name = "PART_WatermarkElement", Type = typeof(FrameworkElement)),
      TemplatePart(Name = "PART_AxesContainer", Type = typeof(ItemsControl)),
      TemplatePart(Name = "PART_SeriesContainer", Type = typeof(ItemsControl)),
      TemplatePart(Name = "PART_ScaleBreakElement", Type = typeof(FrameworkElement))]
    public class ChartAreaPresenter : ContentPresenter, IDisposable
    {
        #region Constants
        /// <summary>
        /// Declares c_axesContainerName
        /// </summary>
        private const string C_axesContainerName = "PART_AxesContainer";

        /// <summary>
        /// Declares c_seriesContainerName
        /// </summary>
        private const string C_seriesContainerName = "PART_SeriesContainer";

        /// <summary>
        /// Declares c_gridElementName
        /// </summary>
        private const string C_gridElementName = "PART_GridElement";

        /// <summary>
        /// Declares C_watermarkElementName
        /// </summary>
        private const string C_watermarkElementName = "PART_WatermarkElement";

        /// <summary>
        /// Declares C_scaleBreakElementName 
        /// </summary>
        private const string C_scaleBreakElementName = "PART_ScaleBreakElement";
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_owner
        /// </summary>
        private ChartArea m_owner;

        /// <summary>
        /// Initializes m_axesContainer
        /// </summary>
        private ItemsControl m_axesContainer;

        /// <summary>
        /// Initializes m_seriesContainer
        /// </summary>
        internal ItemsControl m_seriesContainer;

        /// <summary>
        /// Initializes m_gridElement
        /// </summary>
        private FrameworkElement m_gridElement;
        private FrameworkElement m_watermarkElement;
        private FrameworkElement m_scaleBreakElement;
        internal Canvas m_splitter;
        internal ItemsControl m_cursor;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the axes container.
        /// </summary>
        /// <value>The axes container.</value>
        public ItemsControl AxesContainer
        {
            get { return m_axesContainer; }
        }

        /// <summary>
        /// Gets the series container.
        /// </summary>
        /// <value>The series container.</value>
        public ItemsControl SeriesContainer
        {
            get
            {
                return m_seriesContainer;
            }
        }

        /// <summary>
        /// Get and Set CLR GridElement property
        /// </summary>
        public FrameworkElement GridElement
        {
            get
            {
                return m_gridElement;
            }

        }
        #endregion

        /// <summary>
        /// Called when Instance created for ChartAreaPresenter
        /// </summary>
        public ChartAreaPresenter()
        {
            this.Loaded += new RoutedEventHandler(ChartAreaPresenter_Loaded);
        }

        void ChartAreaPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_owner != null)// && m_owner.m_areaPresenter == null)
            {
                m_owner.SetAreaPresenter(this);
            }
        }


        #region Public methods
        /// <summary>
        /// Invalidates the children.
        /// </summary>
        public void InvalidateChildren()
        {
            if (m_gridElement != null)
            {
                m_gridElement.InvalidateVisual();
            }

            if (m_watermarkElement != null)
            {
                m_watermarkElement.InvalidateVisual();
            }

            if (m_seriesContainer != null && m_owner != null )
            {
                if (m_owner.VisibleSeries != null)
                {
                    foreach (ChartSeries series in m_owner.VisibleSeries)
                    {
                        DependencyObject dobj = m_seriesContainer.ItemContainerGenerator.ContainerFromItem(series);

                        if (dobj is UIElement)
                        {
                            ((UIElement)dobj).InvalidateMeasure();
                        }
                    }
                }
            }

            if (m_axesContainer != null)
            {
                m_axesContainer.InvalidateArrange();
            }
            ////Invalidating ChartSeriesAnnotations adorner.
            ////Retrieving AnnotationsAdorner to invalidate
            if (SeriesContainer != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    Adorner[] adorners = adornerLayer.GetAdorners(this.SeriesContainer);
                    if (adorners != null && adorners[0] != null)
                    {
                        adorners[0].InvalidateVisual();
                    }
                }
            }
            if (m_scaleBreakElement != null)
            {
                m_scaleBreakElement.InvalidateVisual();
            }
        }
   
        /// <summary>
        /// Invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"></see>.
        /// </summary>
        /// <seealso cref="ChartAreaPresenter"/>
        public override void OnApplyTemplate()
        {
            m_axesContainer = this.GetTemplateChild(C_axesContainerName) as ItemsControl;
            m_seriesContainer = this.GetTemplateChild(C_seriesContainerName) as ItemsControl;
            m_gridElement = this.GetTemplateChild(C_gridElementName) as FrameworkElement;
            m_watermarkElement = this.GetTemplateChild(C_watermarkElementName) as FrameworkElement;
            m_scaleBreakElement = this.GetTemplateChild(C_scaleBreakElementName) as FrameworkElement;
            //DependencyObject obj = VisualTreeHelper.GetParent(this);
            //obj = VisualTreeHelper.GetChild(obj, 0);
            //if (obj is Canvas)
            //{
            //    m_splitter = obj as Canvas;
                
            //}
                
            m_cursor = this.GetTemplateChild("interactivecursor") as ItemsControl;
            m_owner = this.TemplatedParent as ChartArea;

            if (m_owner != null)
            {
                m_owner.SetAreaPresenter(this);
                  //  m_owner.UpdateArea();
            }

            base.OnApplyTemplate();
        }

        
        /// <summary>
        /// Update current template or template selector.
        /// </summary>
        internal void InvalidateTemplate()
        {
            if (this.ContentTemplate != null)
            {
                DataTemplate template = this.ContentTemplate;
                this.ContentTemplate = null;
                this.ContentTemplate = template;
            }

            if (this.ContentTemplateSelector != null)
            {
                DataTemplateSelector templateSelector = this.ContentTemplateSelector;
                this.ContentTemplateSelector = null;
                this.ContentTemplateSelector = templateSelector;
            }
        }
        #endregion


        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Loaded -= ChartAreaPresenter_Loaded;
            this.m_watermarkElement = null;
            if (m_axesContainer != null)
            {
                m_axesContainer.ItemsSource = null;
                m_axesContainer.ClearValue(ItemsControl.ItemsSourceProperty);
                m_axesContainer.ItemTemplate = null;
                m_axesContainer.ItemContainerStyle = null;
                m_axesContainer = null;
            }

            if (m_seriesContainer != null)
            {
                m_seriesContainer.ItemsSource = null;
                m_seriesContainer.ItemTemplate = null;
                m_seriesContainer.ClearValue(ItemsControl.ItemsSourceProperty);
                m_seriesContainer.ItemContainerStyle = null;
                m_seriesContainer = null;
            }

            if (m_gridElement is ChartCartesianAreaGrid)
            {
                ((ChartCartesianAreaGrid)m_gridElement).Dispose();
                m_gridElement = null;
            }

            if (m_splitter != null)
            {
                m_splitter.Children.Clear();
                m_splitter = null;
            }

            m_owner = null;
            //this.DataContext = null;
            this.ContentTemplate = null;
            this.ContentTemplateSelector = null;

        }

        #endregion

    }

   
}
