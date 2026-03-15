#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Olap.Manager;
using System.Windows.Threading;
using Syncfusion.Windows.Tools.Olap;
using Syncfusion.Olap.Reports;
using System.ComponentModel;

namespace Syncfusion.OlapChart.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CubeConfiguration
        : Window
    {
        #region Members

        private OlapDataManager olapDataManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CubeConfiguration"/> class.
        /// </summary>
        public CubeConfiguration()
        {
            InitializeComponent();
            this.Closed += (s, a) =>
            {
                DesignerProperties.SetIsInDesignMode(this.OlapChartControl, true);
            };
            this.Loaded += delegate(object sender, RoutedEventArgs e)
                {
                    this.DataBind();
                };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the olap data manager.
        /// </summary>
        /// <value>The olap data manager.</value>
        public OlapDataManager OlapDataManager
        {
            get { return this.olapDataManager; }
            set { this.olapDataManager = value; }
        }

        /// <summary>
        /// Gets or sets the cloned olap data manager.
        /// </summary>
        /// <value>The cloned olap data manager.</value>
        public OlapDataManager ClonedOlapDataManager 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the cube selector.
        /// </summary>
        /// <value>The cube selector.</value>
        public CubeSelector CubeSelector
        {
            get
            {
                return this.PART_CubeSelector;
            }
        }

        /// <summary>
        /// Gets the cube dimension browser.
        /// </summary>
        /// <value>The cube dimension browser.</value>
        public CubeDimensionBrowser CubeDimensionBrowser
        {
            get
            {
                return this.PART_CubeDimensionBrowser;
            }

        }

        /// <summary>
        /// Gets the axis builder column.
        /// </summary>
        /// <value>The axis builder column.</value>
        public AxisElementBuilder AxisBuilderColumn
        {
            get
            {
                return this.PART_AxisBuilderColumn;
            }
        }

        /// <summary>
        /// Gets the axis builder row.
        /// </summary>
        /// <value>The axis builder row.</value>
        public AxisElementBuilder AxisBuilderRow
        {
            get
            {
                return this.PART_AxisBuilderRow;
            }
        }

        /// <summary>
        /// Gets the axis builder slicer.
        /// </summary>
        /// <value>The axis builder slicer.</value>
        public AxisElementBuilder AxisBuilderSlicer
        {
            get
            {
                return this.PART_AxisBuilderSlicer;
            }
        }

        #endregion

        #region Events

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            //// Store.
            OlapReport olapReport = this.ClonedOlapDataManager.CurrentReport;
            this.OlapDataManager.SetCurrentReport(olapReport);
            this.DialogResult = true;
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            //// Role-back.
            this.DialogResult = false;
            this.Close();
        }

        #endregion

        #region Cube configuration helper

        /// <summary>
        /// Binds the manager.
        /// </summary>
        /// <param name="olapDataManager">The olap data manager.</param>
        public void BindManager(OlapDataManager olapDataManager)
        {
            if (this.CubeSelector != null)
                this.CubeSelector.OlapDataManager = olapDataManager;

            if (this.CubeDimensionBrowser != null)
            {
                ResourceDictionary rs = new ResourceDictionary();
                rs.Source = new Uri("/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                if (this.CubeDimensionBrowser.Style == null)
                    this.CubeDimensionBrowser.Style = rs["DefaultCubeDimensionBrowser"] as Style;
                this.CubeDimensionBrowser.OlapDataManager = olapDataManager;
            }

            if (this.AxisBuilderColumn != null)
            {
                this.AxisBuilderColumn.OlapDataManager = olapDataManager;
            }

            if (this.AxisBuilderRow != null)
            {
                this.AxisBuilderRow.OlapDataManager = olapDataManager;
            }
            
            if (this.AxisBuilderSlicer != null)
            {
                this.AxisBuilderSlicer.OlapDataManager = olapDataManager;
            }

            if (this.OlapChartControl != null)
            {
                DesignerProperties.SetIsInDesignMode(this.OlapChartControl, false);
                this.OlapChartControl.OlapDataManager = olapDataManager;
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        public void DataBind()
        {
            this.ClonedOlapDataManager = new OlapDataManager();
            this.ClonedOlapDataManager.ConnectionString = this.OlapDataManager.ConnectionString;
            this.ClonedOlapDataManager.DataProvider = this.olapDataManager.DataProvider;

            Syncfusion.Olap.Reports.OlapReport olapReport = this.olapDataManager.CurrentReport.Clone();
            this.BindManager(this.ClonedOlapDataManager);

            if (olapDataManager.CurrentCubeName != null && olapDataManager.CurrentCubeName != string.Empty)
            {
                this.ClonedOlapDataManager.SetCurrentReport(olapReport);
            }
        }

        #endregion
    }
}

