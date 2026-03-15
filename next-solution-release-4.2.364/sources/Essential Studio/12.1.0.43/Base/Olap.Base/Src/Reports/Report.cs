//-------------------------------------------------------------------------------------------------
// <copyright file="Report.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#if !SILVERLIGHT
namespace Syncfusion.Olap.Reports
#else
namespace Syncfusion.OlapSilverlight.Reports
#endif
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

#if !SILVERLIGHT
    using Syncfusion.Olap.Common;
    using Syncfusion.Olap.Manager;
    using Syncfusion.Olap.Engine;
    /// <summary>
    /// Represents the report information like axis elements, visibility of expanders, paging options, drill functionalities, etc.,.
    /// </summary>
    [Serializable]
    public class OlapReport : ICloneable<OlapReport>
#else
    using System.Runtime.Serialization;
    using Syncfusion.OlapSilverlight.Manager;
    using Syncfusion.OlapSilverlight.Common;
    using Syncfusion.OlapSilverlight.Engine;
    /// <summary>
    /// Represents the report information like axis elements, visibility of expanders, paging options, drill functionalities, etc.,.
    /// </summary>
    [DataContract]
    public class OlapReport
#endif
    {
        #region Private Variables
        private Items _categoricalElements;

        private ChartAppearanceSettings _chartSettings;
        private GridAppearanceSettings _gridSettings; 

        private string _currentCubeName;

        private bool _useWhereClauseForSlicing = true;

        private string _connectionString;

        private Items _filterElements;

        private Items _seriesElements;

        private bool _showExpanders=true;

        ////private bool _ShowGrandTotal;

        private Items _slicerElements;

        //private Items _calculatedMember;

        private Items _calculatedMembers;

        private Items _virtualKpis;

        private bool _togglePivot;

        private PagerOptions pagerOptions = null;

        private SerializableDictionary<string, HeaderPositionsInfo> drilledCells;

        private List<SlicerRangeFiltersInfo> _filterSlicerElement = null;
        #endregion

#if !SILVERLIGHT
        //#region Internal Variables
        //internal OlapDataManager Model;
        //#endregion
#endif

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapReport"/> class.
        /// </summary>
        public OlapReport()
        {
            this.Name = string.Empty;
            this._currentCubeName = string.Empty;
            this.UseWhereClauseForSlicing = true;
            this._connectionString = string.Empty;
            this.ShowEmptyRowData = false;
            this.ShowEmptyColumnData = false;
            this.ShowExpanders = true;
            ////this.ShowGrandTotal = false;
            this.TogglePivot = false;
            this.CategoricalElements = new Items();
            this.SeriesElements = new Items();
            this.SlicerElements = new Items();
            this.FilterElements = new Items();
            this.CalculatedMembers = new Items();
            this.EnablePaging = false;
            this.VisualTotalVisibility = true;
            this.UseDefaultMember = true;
            this._filterSlicerElement = new List<SlicerRangeFiltersInfo>();
            this.ChartSettings = new ChartAppearanceSettings();
            this.GridSettings = new GridAppearanceSettings();
            this.VirtualKpiElements = new Items();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapReport"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public OlapReport(string name)
        {
            this.Name = name;
            this._currentCubeName = string.Empty;
            this._connectionString = string.Empty;
            this.ShowEmptyRowData = false;
            this.ShowEmptyColumnData = false;
            this.ShowExpanders = true;
            ////this.ShowGrandTotal = false;
            this.TogglePivot = false;
            this.CategoricalElements = new Items();
            this.SeriesElements = new Items();
            this.SlicerElements = new Items();
            this.FilterElements = new Items();
            this.CalculatedMembers = new Items();
            this.EnablePaging = false;
            this.VisualTotalVisibility = true;
            this.UseDefaultMember = true;
            this._filterSlicerElement = new List<SlicerRangeFiltersInfo>();
            this.ChartSettings = new ChartAppearanceSettings();
            this.GridSettings = new GridAppearanceSettings();
            this.VirtualKpiElements = new Items();
        }
        #endregion

        #region Public Methods 
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the categorical elements.
        /// </summary>
        /// <value>The categorical elements.</value>
        public Items CategoricalElements
        {
            get
            {
                return _categoricalElements;
            }

            set
            {
                _categoricalElements = value;
////#if !SILVERLIGHT
////                if (Model != null)
////                {
////                    Model.NotifyElementModified(AxisPosition.Categorical);
////                }
////#endif
            }
        }

        /// <summary>
        /// Gets or sets the engine version.
        /// </summary>
        /// <value>The QueryBuilder Engine version</value>
        public QueryBuilderEngineVersions EngineVersion { get; set; }

        /// <summary>
        /// Gets or sets the chart settings.
        /// </summary>
        /// <value>The chart settings.</value>
        public ChartAppearanceSettings ChartSettings
        {
            get
            {
                return _chartSettings;
            }

            set
            {
                _chartSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the grid settings.
        /// </summary>
        /// <value>The grid settings.</value>
        public GridAppearanceSettings GridSettings
        {
            get { return _gridSettings; }
            set { _gridSettings = value; }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the name of the current cube.
        /// </summary>
        /// <value>The name of the current cube.</value>
        public string CurrentCubeName
        {
            get
            {
                return _currentCubeName;
            }

            set
            {
                _currentCubeName = value;
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }

            set
            {
                _connectionString = value;
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the filter elements.
        /// </summary>
        /// <value>The filter elements.</value>
        public Items FilterElements
        {
            get
            {
                return _filterElements;
            }

            set
            {
                _filterElements = value;
////#if !SILVERLIGHT
////                if (Model != null)
////                {
////                    Model.NotifyElementModified();
////                }
////#endif
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
            set;
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the series elements.
        /// </summary>
        /// <value>The series elements.</value>
        public Items SeriesElements
        {
            get
            {
                return _seriesElements;
            }

            set
            {
                _seriesElements = value;
////#if !SILVERLIGHT
////                if (Model != null)
////                {
////                    this.Model.NotifyElementModified(AxisPosition.Series);
////                }
////#endif
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether [show empty column data].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show empty column data]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowEmptyColumnData { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether [show empty row data].
        /// </summary>
        /// <value><c>true</c> if [show empty row data]; otherwise, <c>false</c>.</value>
        public bool ShowEmptyRowData { get; set; }


#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or set UseWhereClauseForSlicing
        /// </summary>
        public bool UseWhereClauseForSlicing
        {
            get { return _useWhereClauseForSlicing; }
            set { _useWhereClauseForSlicing = value; }
        }
            

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether [show expanders].
        /// </summary>
        /// <value><c>true</c> if [show expanders]; otherwise, <c>false</c>.</value>
        public bool ShowExpanders
        {
            get
            {
                return _showExpanders;
            }

            set
            {
                _showExpanders = value;
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the slicer elements.
        /// </summary>
        /// <value>The slicer elements.</value>
        public Items SlicerElements
        {
            get
            {
                return _slicerElements;
            }

            set
            {
                _slicerElements = value;
////#if !SILVERLIGHT
////                if (this.Model != null)
////                {
////                    this.Model.NotifyElementModified();
////                }
////#endif
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether [toggle pivot].
        /// </summary>
        /// <value><c>true</c> if [toggle pivot]; otherwise, <c>false</c>.</value>
        public bool TogglePivot
        {
            get
            {
                return _togglePivot;
            }

            set
            {
                _togglePivot = value;
////#if !SILVERLIGHT
////                if (Model != null)
////                {
////                    this.Model.NotifyElementModified();
////                }
////#endif
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the calculated members.
        /// </summary>
        /// <value>The calculated members.</value>
        public Items CalculatedMembers
        {
            get 
            {
                return _calculatedMembers;
            }

            set
            {
                _calculatedMembers = value;
            }
        }


#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the Virtual Kpi elements
        /// </summary>
        public Items VirtualKpiElements
        {
            get { return _virtualKpis; }
            set { _virtualKpis = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is paging enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is paging enabled; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
#if SILVERLIGHT
        [DataMember]
#endif
        public bool EnablePaging { get; set; }

        /// <summary>
        /// Gets or sets the type of the drill.
        /// </summary>
        /// <value>The type of the drill.</value>
        [DefaultValue(DrillType.DrillMember)]
#if SILVERLIGHT
        [DataMember]
#endif
        public DrillType DrillType { get; set; }
         
        /// <summary>
        /// Gets or sets the visibility for VisualTotals
        /// </summary>
        [DefaultValue(true)]
#if SILVERLIGHT
        [DataMember]
#endif
        public bool VisualTotalVisibility { get; set; }

        /// <summary>
        /// Gets or sets whether default member can be used or not(to avoid the results with its All member)
        /// </summary>
        [DefaultValue(true)]
#if SILVERLIGHT
        [DataMember]
#endif
        public bool UseDefaultMember { get; set; }


        /// <summary>
        /// Gets the pager options.
        /// </summary>
        /// <value>The pager options.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public PagerOptions PagerOptions
        {
            get
            {
                if (pagerOptions == null)
                {
                    pagerOptions = new PagerOptions();
                }
                return pagerOptions;
            }
            set
            {
                pagerOptions = value;
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the drilled cells.
        /// </summary>
        /// <value>The drilled cells.</value>
        public SerializableDictionary<string, HeaderPositionsInfo> DrilledCells
        {
            get
            {
                if (drilledCells == null)
                {
                    drilledCells = new SerializableDictionary<string, HeaderPositionsInfo>();
                    drilledCells.Add("ColumnHeader", null);
                    drilledCells.Add("RowHeader", null);
                }
                return drilledCells;
            }
            set
            {
                drilledCells = value;
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Filtering the slicer element by range using the "dimension name:hierarchy name:level name:start value:end value" format. Unique names can also be passed in those fields.
        /// </summary>
        /// <value>The name.</value>
        public List<SlicerRangeFiltersInfo> SlicerRangeFilters
        {
            get
            {
                return _filterSlicerElement;
            }
            set
            {
                _filterSlicerElement = value;
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the current MDX query.
        /// </summary>
        /// <value>The current MDX query.</value>
        [System.Xml.Serialization.XmlIgnore()]
        public string CurrentMdxQuery { get; set; }

//#if SILVERLIGHT
//        [DataMember]
//#endif
//        public bool ExpandAll
//        {
//            get;
//            set;
//        }

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="OlapReport"/>.</returns>
        public OlapReport Clone()
        {
            OlapReport report = new OlapReport();
            foreach (Item item in this.CategoricalElements)
            {
                if (item.ElementValue != null)
                    report.CategoricalElements.Add(item.Clone());
            }

            foreach (Item item in this.SeriesElements)
            {
                if (item.ElementValue != null)
                    report.SeriesElements.Add(item.Clone());
            }

            foreach (Item item in this.SlicerElements)
            {
                report.SlicerElements.Add(item.Clone());
            }

            foreach (Item item in this.FilterElements)
            {
                report.FilterElements.Add(item.Clone());
            }

            if (this.ChartSettings != null)
            {
                report.ChartSettings = this.ChartSettings.Clone();
            }

            if (this.GridSettings != null)
            {
                report.GridSettings = this.GridSettings.Clone();
            }

            ////Updating Categorical Items property
            report.CategoricalElements.IsFilterOrSortOn = this.CategoricalElements.IsFilterOrSortOn;
            report.CategoricalElements.SubSetElement = this.CategoricalElements.SubSetElement;
            ////Updating Series Items property
            report.SeriesElements.IsFilterOrSortOn = this.SeriesElements.IsFilterOrSortOn;
            report.SeriesElements.SubSetElement = this.SeriesElements.SubSetElement;
            ////Updating Slicer Items property
            report.SeriesElements.IsFilterOrSortOn = this.SeriesElements.IsFilterOrSortOn;

            report.CurrentCubeName = this.CurrentCubeName;
            report.ConnectionString = this.ConnectionString;
            report.Name = this.Name;
            report.ShowEmptyColumnData = this.ShowEmptyColumnData;
            report.ShowEmptyRowData = this.ShowEmptyRowData;
            report.ShowExpanders = this.ShowExpanders;
            report.EngineVersion = this.EngineVersion;
            report.CalculatedMembers = this.CalculatedMembers;
            report.VirtualKpiElements = this.VirtualKpiElements;
            ////report.ShowGrandTotal = this.ShowGrandTotal;
            report.DrillType = this.DrillType;
            report.TogglePivot = this.TogglePivot;
            report.VisualTotalVisibility = this.VisualTotalVisibility;
            report.EnablePaging = this.EnablePaging;
            this.PagerOptions.CopyTo(ref report.pagerOptions);
            return report;
        }
#endif
        #endregion

    }

    /// <summary>
    /// To filter the dimensions available in slicer field based on the value range.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
     public class SlicerRangeFiltersInfo
#else
    [DataContract]
    public class SlicerRangeFiltersInfo
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlicerRangeFiltersInfo"/> class. It's used to filter values from one range to another. Also  it's been recommended to use the unique name of the member element for start value and end value. The name of the member element can be used for start value and end value if and only if they build the unique name, which normally occurs for some dimensions and not all.
        /// </summary>
        public SlicerRangeFiltersInfo()
        { 
        
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SlicerRangeFiltersInfo"/> class. It's used to filter values from one range to another. Also  it's been recommended to use the unique name of the member element for start value and end value. The name of the member element can be used for start value and end value if and only if they build the unique name, which normally occurs for some dimensions and not all.
        /// </summary>
        /// <param name="startValueUniqueName">Start name of the value unique.</param>
        /// <param name="endValueUniqueName">End name of the value unique.</param>
        public SlicerRangeFiltersInfo(string startValueUniqueName, string endValueUniqueName)
        {
            this.StartValue = startValueUniqueName;
            this.EndValue = endValueUniqueName;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SlicerRangeFiltersInfo"/> class. It's used to filter values from one range to another. Also  it's been recommended to use the unique name of the member element for start value and end value. The name of the member element can be used for start value and end value if and only if they build the unique name, which normally occurs for some dimensions and not all.
        /// </summary>
        /// <param name="dimensionName">Name of the dimension.</param>
        /// <param name="hierarchyName">Name of the hierarchy.</param>
        /// <param name="levelName">Name of the level.</param>
        /// <param name="startValueName">Start name of the value.</param>
        /// <param name="endValueName">End name of the value.</param>
        public SlicerRangeFiltersInfo(string dimensionName, string hierarchyName, string levelName, string startValueName, string endValueName)
        {
            this.DimensionName = dimensionName;
            this.HierarchyName = hierarchyName;
            this.LevelName = levelName;
            this.StartValue = startValueName;
            this.EndValue = endValueName;
        }
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the name of the dimension.
        /// </summary>
        /// <value>The name of the dimension.</value>
        public string DimensionName
        {
            get;
            set;
        }
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the name of the hierarchy.
        /// </summary>
        /// <value>The name of the hierarchy.</value>
        public string HierarchyName
        {
            get;
            set;
        }
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the name of the level.
        /// </summary>
        /// <value>The name of the level.</value>
        public string LevelName
        {
            get;
            set;
        }
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the start value.
        /// </summary>
        /// <value>The start value.</value>
        public string StartValue
        {
            get;
            set;
        }
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the end value.
        /// </summary>
        /// <value>The end value.</value>
        public string EndValue
        {
            get;
            set;
        }
    }
}
