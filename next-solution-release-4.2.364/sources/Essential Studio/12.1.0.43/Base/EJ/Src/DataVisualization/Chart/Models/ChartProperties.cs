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
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Shared;
using System.Drawing;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class ChartProperties{
        #region Fields
        
        private List<Series> m_Series = new List<Series>();
        private List<RowDefinitions> m_RowDefinitions = new List<RowDefinitions>();
        private List<ColumnDefinitions> m_ColumnDefinitions = new List<ColumnDefinitions>();

        private object m_seriesopt = null;
        private object m_crosshair = new CrossHair();
        private object m_margin = null;

        private string m_background=null;
        private object m_legend = new Legend();
        private object m_primaryXaxis = null; 
        private object m_primaryYaxis = null; 

        private object m_Size = null;

        private object m_title = null;
        private double m_ElementSpacing = 10;
        private object m_Border = null;
        private object m_ChartAreaBorder = null;
      
         
        private string m_backgroundimageurl = null;
       
        private object m_zooming = null;
        private bool m_canresize=false;
       
        private bool m_initSeriesRender=true;
        private ChartTheme m_theme = ChartTheme.FlatLight;

        //Event

        private String load = null;
        private String axesLabelRendering = null;
        private String axesRangeCalculate = null;
        private String chartAreaBoundsCalculate = null;
        private String legendItemRendering = null;
        private String lengendBoundsCalculate = null;
        private String preRender = null;
        private String seriesRendering = null;
        private String symbolRendering = null;
        private String titleRendering = null;
        private String axesLabelsInitialize = null;
        private String axesTitleRendering = null;
        private String pointRegionClick = null;
        private String pointRegionMouseMove = null;
        private String legendItemClick = null;
        private String legendItemMouseMove = null;
        private String displayTextRendering = null;
        private String toolTipInitialize = null;
        private String trackAxisToolTip = null;
        private String trackToolTip = null;
        private String animationComplete = null;
        private String destroy = null;
        private String create = null;
        private string m_localization = null;

        private List<Axis> m_axes = new List<Axis>();
        #endregion

        #region Properties

        [JsonProperty("series")]
        
        public List<Series> Series
        {
            get { return this.m_Series; }
            set { this.m_Series = value; }
        }

        [JsonProperty("rowDefinitions")]
        
        public List<RowDefinitions> RowDefinitions
        {
            get { return this.m_RowDefinitions; }
            set { this.m_RowDefinitions = value; }
        }

        [JsonProperty("columnDefinitions")]

        public List<ColumnDefinitions> ColumnDefinitions
        {
            get { return this.m_ColumnDefinitions; }
            set { this.m_ColumnDefinitions = value; }
        }

        [JsonProperty("commonSeriesOptions")]
        public object CommonSeriesOptions
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

        [JsonProperty("background")]
        [DefaultValue(null)]
        public string Background
        {
            get { return this.m_background; }
            set { this.m_background = value; }   
        }

        [JsonProperty("primaryXAxis")]
        
        public object PrimaryXAxis
        {
            get
            {
               
                return this.m_primaryXaxis;
            }
            set
            {
                this.m_primaryXaxis = value;
            }
        }

        [JsonProperty("primaryYAxis")]
        
        public object PrimaryYAxis
        {
            get
            {
             
                return this.m_primaryYaxis;
            }
            set
            {
                this.m_primaryYaxis = value;
            }
        }

        [JsonProperty("axes")]
        
        public List<Axis> Axes
        {
            get { return this.m_axes; }
            set
            {
                this.m_axes = value;
            }
        }
        [JsonProperty("size")]
        
        public object Size
        {
            get{ return this.m_Size;}
            set { this.m_Size = value;}
        }

        [JsonProperty("elementSpacing")]
        [DefaultValue(10)]
        public double ElementSpacing
        {
            get{ return this.m_ElementSpacing;}
            set { this.m_ElementSpacing = value;}
        }
        /// <summary>
        /// gets or sets Tilte
        /// </summary>
        [JsonProperty("title")]
        public object Title
        {
            get
            {
                return this.m_title;
            }
            set
            {
                m_title = value;
            }
        }


        [JsonProperty("canResize")]
        [DefaultValue(false)]
        public bool CanResize
        {
            get{ return this.m_canresize;}
            set { this.m_canresize = value;}
        }
        [JsonProperty("localization")]
        [DefaultValue(null)]
        public string Localization
        {
            get { return this.m_localization; }
            set { this.m_localization = value; }
        }
       [JsonProperty("initSeriesRender")]
        [DefaultValue(true)]
        public bool InitSeriesRender
        {
            get { return this.m_initSeriesRender; }
            set { this.m_initSeriesRender = value; }
        }
       
        [JsonProperty("theme")]
        [DefaultValue(ChartTheme.FlatLight)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartTheme Theme
        {
            get{ return this.m_theme;}
            set { this.m_theme = value;}
        }

         

        [JsonProperty("backGroundImageUrl")]
        [DefaultValue(null)]
        public string BackGroundImageUrl
        {
            get { return this.m_backgroundimageurl; }
            set { this.m_backgroundimageurl = value; }
        }

        

        [JsonProperty("border")]
        public object Border
        {
            get
            {
               
                return this.m_Border;
            }
            set
            {
                this.m_Border = value;
            }
        }
        
        [JsonProperty("chartArea")]
        public object ChartArea
        {
            get
            {
               
                return this.m_ChartAreaBorder;
            }
            set
            {
                this.m_ChartAreaBorder = value;
            }
        }

        [JsonProperty("crosshair")]
        public object CrossHair
        {
            get
            {
                
                return this.m_crosshair;
            }
            set
            {
                this.m_crosshair = value;
            }
        }

        [JsonProperty("margin")]
        public object Margin
        {
            get
            {

                return this.m_margin;
            }
            set
            {
                this.m_margin = value;
            }
        }

        [JsonProperty("zooming")]
        public object Zooming
        {
            get
            {
               
                return this.m_zooming;
            }
            set
            {
                this.m_zooming = value;
            }
        }

        [JsonProperty("legend")]
        public object Legend
        {
            get
            {
                
                return this.m_legend;
            }
            set
            {
                this.m_legend = value;
            }
        }

              
        [JsonProperty("load")]
        [DefaultValue(null)]
        public String Load
        {
            get { return this.load; }
            set { this.load = value; }
        }

        [JsonProperty("axesLabelRendering")]
        [DefaultValue(null)]
        public String AxesLabelRendering
        {
            get { return this.axesLabelRendering; }
            set { this.axesLabelRendering = value; }
        }

        [JsonProperty("axesRangeCalculate")]
        [DefaultValue(null)]
        public String AxesRangeCalculate
        {
            get { return this.axesRangeCalculate; }
            set { this.axesRangeCalculate = value; }
        }

        [JsonProperty("axesTitleRendering")]
        [DefaultValue(null)]
        public String AxesTitleRendering
        {
            get { return this.axesTitleRendering; }
            set { this.axesTitleRendering = value; }
        }

        [JsonProperty("chartAreaBoundsCalculate")]
        [DefaultValue(null)]
        public String ChartAreaBoundsCalculate
        {
            get { return this.chartAreaBoundsCalculate; }
            set { this.chartAreaBoundsCalculate = value; }
        }

        [JsonProperty("legendItemRendering")]
        [DefaultValue(null)]
        public String LegendItemRendering
        {
            get { return this.legendItemRendering; }
            set { this.legendItemRendering = value; }
        }

        [JsonProperty("lengendBoundsCalculate")]
        [DefaultValue(null)]
        public String LengendBoundsCalculate
        {
            get { return this.lengendBoundsCalculate; }
            set { this.lengendBoundsCalculate = value; }
        }

        [JsonProperty("preRender")]
        [DefaultValue(null)]
        public String PreRender
        {
            get { return this.preRender; }
            set { this.preRender = value; }
        }

        [JsonProperty("seriesRendering")]
        [DefaultValue(null)]
        public String SeriesRendering
        {
            get { return this.seriesRendering; }
            set { this.seriesRendering = value; }
        }

        [JsonProperty("symbolRendering")]
        [DefaultValue(null)]
        public String SymbolRendering
        {
            get { return this.symbolRendering; }
            set { this.symbolRendering = value; }
        }

        [JsonProperty("titleRendering")]
        [DefaultValue(null)]
        public String TitleRendering
        {
            get { return this.titleRendering; }
            set { this.titleRendering = value; }
        }

        [JsonProperty("axesLabelsInitialize")]
        [DefaultValue(null)]
        public String AxesLabelsInitialize
        {
            get { return this.axesLabelsInitialize; }
            set { this.axesLabelsInitialize = value; }
        }

        [JsonProperty("pointRegionClick")]
        [DefaultValue(null)]
        public String PointRegionClick
        {
            get { return this.pointRegionClick; }
            set { this.pointRegionClick = value; }
        }

        [JsonProperty("pointRegionMouseMove")]
        [DefaultValue(null)]
        public String PointRegionMouseMove
        {
            get { return this.pointRegionMouseMove; }
            set { this.pointRegionMouseMove = value; }
        }

        [JsonProperty("legendItemClick")]
        [DefaultValue(null)]
        public String LegendItemClick
        {
            get { return this.legendItemClick; }
            set { this.legendItemClick = value; }
        }

        [JsonProperty("legendItemMouseMove")]
        [DefaultValue(null)]
        public String LegendItemMouseMove
        {
            get { return this.legendItemMouseMove; }
            set { this.legendItemMouseMove = value; }
        }

        [JsonProperty("displayTextRendering")]
        [DefaultValue(null)]
        public String DisplayTextRendering
        {
            get { return this.displayTextRendering; }
            set { this.displayTextRendering = value; }
        }

        [JsonProperty("toolTipInitialize")]
        [DefaultValue(null)]
        public String ToolTipInitialize
        {
            get { return this.toolTipInitialize; }
            set { this.toolTipInitialize = value; }
        }

        [JsonProperty("trackAxisToolTip")]
        [DefaultValue(null)]
        public String TrackAxisToolTip
        {
            get { return this.trackAxisToolTip; }
            set { this.trackAxisToolTip = value; }
        }

        [JsonProperty("trackToolTip")]
        [DefaultValue(null)]
        public String TrackToolTip
        {
            get { return this.trackToolTip; }
            set { this.trackToolTip = value; }
        }

        [JsonProperty("animationComplete")]
        [DefaultValue(null)]
        public String AnimationComplete
        {
            get { return this.animationComplete; }
            set { this.animationComplete = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }



        #endregion

        #region ShouldSerialize Methods

        public bool ShouldSerializePrimaryXAxis()
        {
            if (Utils.PropertyCompare(PrimaryXAxis, new Axis()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializePrimaryYAxis()
        {
            if (Utils.PropertyCompare(PrimaryYAxis, new Axis()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeCommonSeriesOptions()
        {
            if (Utils.PropertyCompare(CommonSeriesOptions, new CommonSeriesOptions()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new LayoutBorder()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeMargin()
        {
            if (Utils.PropertyCompare(Margin, new Margin()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeChartArea()
        {
            if (Utils.PropertyCompare(ChartArea, new ChartArea()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeZooming()
        {
            if (Utils.PropertyCompare(Zooming, new Zooming()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeLegend()
        {
            if (Utils.PropertyCompare(Legend, new Legend()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeTitle()
        {
            if (Utils.PropertyCompare(Title, new Title()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeAxes()
        {
            if (Axes.Count != 0)
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
        public bool ShouldSerializeRowDefinitions()
        {
            if (RowDefinitions.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeColumnDefinitions()
        {
            if (ColumnDefinitions.Count != 0)
                return true;
            else
                return false;
        }
       public bool ShouldSerializeSize()
        {
            if (Utils.PropertyCompare(Size, new Size()))
                return true;
            else
                return false;
            
        }
        #endregion



    }
}
