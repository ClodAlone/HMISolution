#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.Shared;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class RangeNavigatorProperties
    {
        #region Field

        private string m_theme = null;
        private string m_padding =null;
        private bool m_canResize = false;
        private bool m_snappingMode = false;
        private bool m_deferredUpdate = true;
        private bool m_isasync = false;
        private int m_asyncInterval = 100;
        private string m_selectedData=null;

        private object m_size = null;
        private object m_tooltip = null;
        private object m_range = null;
        private object m_selectedRange = null;
        private object m_zoomCordinates = null;
        private object m_navigatorStyle = null;
        private object valueAxisSettings =null;
        private object m_labelSetting = null;
        private object m_dataSource = new NavigatorDataSource();
        private List<Series> m_Series = new List<Series>();
        private object m_seriesopt = null;

        //Event
        private string loaded = null;
        private string created = null;
        private string rangeChanged = null;
        private string m_rangePadding = null;
        private string m_valueType = null;
        private bool m_isRTL = false;
        private string m_localization = null;

         #endregion

        #region Property
        [JsonProperty("loaded")]
        [DefaultValue(null)]
        public String Loaded
        {
            get { return this.loaded; }
            set { this.loaded = value; }
        }
        [JsonProperty("load")]
        [DefaultValue(null)]
        public String Load
        {
            get { return this.created; }
            set { this.created = value; }
        }
        [JsonProperty("rangeChanged")]
        [DefaultValue(null)]
        public String RangeChanged
        {
            get { return this.rangeChanged; }
            set { this.rangeChanged = value; }
        }
        [JsonProperty("theme")]
        [DefaultValue(null)]
        public string Theme
        {
            get { return this.m_theme; }
            set { this.m_theme = value; }
        }
        [JsonProperty("valueType")]
        [DefaultValue(null)]
        public string ValueType
        {
            get { return this.m_valueType; }
            set { this.m_valueType = value; }
        }
        [JsonProperty("rangePadding")]
        [DefaultValue(null)]
        public string RangePadding
        {
            get { return this.m_rangePadding; }
            set { this.m_rangePadding = value; }
        }
        [JsonProperty("localization")]
        [DefaultValue(null)]
        public string Localization
        {
            get { return this.m_localization; }
            set { this.m_localization = value; }
        }
        [JsonProperty("isRTL")]
        [DefaultValue(false)]
        public bool IsRTL
        {
            get { return this.m_isRTL; }
            set { this.m_isRTL = value; }
        }
        [JsonProperty("padding")]
        [DefaultValue(null)]
        public string Padding
        {
            get { return this.m_padding; }
            set { this.m_padding = value; }
        }

        [JsonProperty("canResize")]
        [DefaultValue(false)]
        public bool CanResize
        {
            get { return this.m_canResize; }
            set { this.m_canResize = value; }
        }
        [JsonProperty("snappingMode")]
        [DefaultValue(false)]
        public bool SnappingMode
        {
            get { return this.m_snappingMode; }
            set { this.m_snappingMode = value; }
        }
        [JsonProperty("isasync")]
        [DefaultValue(false)]
        public bool IsAsync
        {
            get { return this.m_isasync; }
            set { this.m_isasync = value; }
        }
        [JsonProperty("deferredUpdate")]
        [DefaultValue(true)]
        public bool DeferredUpdate
        {
            get { return this.m_deferredUpdate; }
            set { this.m_deferredUpdate = value; }
        }
        
        [JsonProperty("asyncInterval")]
        [DefaultValue(100)]
        public int AsyncInterval
        {
            get { return this.m_asyncInterval; }
            set { this.m_asyncInterval = value; }
        }
        [JsonProperty("selectedData")]
        [DefaultValue(null)]
        public string SelectedData
        {
            get { return this.m_selectedData; }
            set { this.m_selectedData = value; }
        }

        [JsonProperty("size")]
        public object Size
        {
            get { return this.m_size; }
            set { this.m_size = value; }
        }
        [JsonProperty("tooltip")]
        public object Tooltip
        {
            get { return this.m_tooltip; }
            set { this.m_tooltip = value; }
        }
        [JsonProperty("range")]
        public object Range
        {
            get { return this.m_range; }
            set { this.m_range = value; }
        }
        [JsonProperty("selectedRange")]
        public object SelectedRange
        {
            get { return this.m_selectedRange; }
            set { this.m_selectedRange = value; }
        }
        [JsonProperty("zoomCordinates")]
        public object ZoomCordinates
        {
            get { return this.m_zoomCordinates; }
            set { this.m_zoomCordinates = value; }
        }
         [JsonProperty("navigatorStyle")]
        public object NavigatorStyle
        {
            get { return this.m_navigatorStyle; }
            set { this.m_navigatorStyle = value; }
        }
         [JsonProperty("valueAxisSettings")]
         public object ValueAxisSettings
         {
             get { return this.valueAxisSettings; }
             set { this.valueAxisSettings = value; }
         }
         [JsonProperty("labelSettings")]
         public object LabelSetting
         {
             get { return this.m_labelSetting; }
             set { this.m_labelSetting = value; }
         }

         [JsonProperty("dataSource")]
         public object DataSource
         {
             get { return this.m_dataSource; }
             set { this.m_dataSource = value; }
         }
         [JsonProperty("series")]
       
         public List<Series> Series
         {
             get { return this.m_Series; }
             set { this.m_Series = value; }
         }
         [JsonProperty("seriesSettings")]
         public object SeriesSetting
         {
             get
             {

                 return this.m_seriesopt;
             }
             set
             {
                 this.m_seriesopt = value;
             }
         }
        #endregion

        #region ShouldSerialize
        public bool ShouldSerializeSize()
        {
            if (Utils.PropertyCompare(Size, new NavigatorSize()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeValueAxisSettings()
        {
            if (Utils.PropertyCompare(ValueAxisSettings, new Axis()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeTooltip()
        {
            if (Utils.PropertyCompare(Tooltip, new Tooltip()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeRange()
        {
            if (Utils.PropertyCompare(Range, new Range()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeSelectedRange()
        {
            if (Utils.PropertyCompare(SelectedRange, new SelectedRange()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeZoomCordinates()
        {
            if (Utils.PropertyCompare(ZoomCordinates, new ZoomCordinates()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeNavigatorStyle()
        {
            if (Utils.PropertyCompare(NavigatorStyle, new NavigatorStyle()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeLabelSetting()
        {
            if (Utils.PropertyCompare(LabelSetting, new LabelSetting()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeDataSource()
        {
            if (Utils.PropertyCompare(DataSource, new NavigatorDataSource()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeSeries()
        {
            if (Series.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeSeriesSetting()
        {
            if (Utils.PropertyCompare(SeriesSetting, new CommonSeriesOptions()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
