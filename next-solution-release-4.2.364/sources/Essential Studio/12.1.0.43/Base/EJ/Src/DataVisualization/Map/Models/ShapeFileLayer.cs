#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class ShapePoint
    {
        public ShapePoint()
        {
        }

        public ShapePoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        [JsonProperty("x")]
        [DefaultValue(0)]
        public double X { get; set; }
        [JsonProperty("y")]
        [DefaultValue(0)]
        public double Y { get; set; }
    }

    public class ShapeFileLayer
    {
        #region Fields

        private bool enableSelection = true;
        private bool enableMouseHover = false;
        private bool showToolTip = false;
        private bool enableAnimation = false;
        private bool showMapItems = true;
        private object data = new object();
        private object dataSource = new object();
        private object annotationDataSource = new object();
        private string annotationTemplate = null;
        private string tooltipTemplate = null;
        private string mapItemsTemplate = null;
        private string shapeIdTableField = null;
        private string shapeIdPath = null;
        private List<MapAnnotations> annotations = null;
        private List<ShapeFileLayer> subshapeFileLayers = null;
        private ShapeSetting shapeSetting = null;
        private BubbleSetting bubbleSetting = null;
        private MapLabelSetting labelSetting = null;
        private MapLegendSetting legendSetting = null;
        private MapType mapType = MapType.Geomtery;

        #endregion

        public ShapeFileLayer()
        {
            this.annotations = new List<MapAnnotations>();
            this.subshapeFileLayers = new List<ShapeFileLayer>();
            this.shapeSetting = new ShapeSetting();
            this.bubbleSetting = new BubbleSetting();
        }

        #region Properties

        [JsonProperty("legendSetting")]
        [DefaultValue(null)]
        public MapLegendSetting LegendSetting
        {
            get { return this.legendSetting; }
            set { this.legendSetting = value; }
        }

        [JsonProperty("labelSetting")]
        [DefaultValue(null)]
        public MapLabelSetting LabelSetting
        {
            get { return this.labelSetting; }
            set { this.labelSetting = value; }
        }

        [JsonProperty("mapType")]
        [DefaultValue(MapType.Geomtery)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MapType MapType
        {
            get { return this.mapType; }
            set { this.mapType = value; }
        }

        [JsonProperty("enableSelection")]
        [DefaultValue(true)]
        public bool EnableSelection
        {
            get { return this.enableSelection; }
            set { this.enableSelection = value; }
        }

        [JsonProperty("enableMouseHover")]
        [DefaultValue(false)]
        public bool EnableMouseHover
        {
            get { return this.enableMouseHover; }
            set { this.enableMouseHover = value; }
        }

        [JsonProperty("showToolTip")]
        [DefaultValue(false)]
        public bool ShowToolTip
        {
            get { return this.showToolTip; }
            set { this.showToolTip = value; }
        }


        [JsonProperty("enableAnimation")]
        [DefaultValue(false)]
        public bool EnableAnimation
        {
            get { return this.enableAnimation; }
            set { this.enableAnimation = value; }
        }

        [JsonProperty("showMapItems")]
        [DefaultValue(false)]
        public bool ShowMapItems
        {
            get { return this.showMapItems; }
            set { this.showMapItems = value; }
        }

        [JsonProperty("data")]
        [JsonConverter(typeof(Syncfusion.JavaScript.DataVisualization.MapDataConvertor))]
        public object Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        [JsonProperty("dataSource")]
        [JsonConverter(typeof (DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }

        [JsonProperty("annotationDataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object AnnotationDataSource
        {
            get { return this.annotationDataSource; }
            set { this.annotationDataSource = value; }
        }
        [JsonProperty("annotationTemplate")]
        [DefaultValue(null)]
        public string AnnotationTemplate
        {
            get { return this.annotationTemplate; }
            set { this.annotationTemplate = value; }
        }

        [JsonProperty("tooltipTemplate")]
        [DefaultValue(null)]
        public string TooltipTemplate
        {
            get { return this.tooltipTemplate; }
            set { this.tooltipTemplate = value; }
        }

        [JsonProperty("mapItemsTemplate")]
        [DefaultValue(null)]
        public string MapItemsTemplate
        {
            get { return this.mapItemsTemplate; }
            set { this.mapItemsTemplate = value; }
        }

        [JsonProperty("annotations")]
        public List<MapAnnotations> Annotations
        {
            get { return this.annotations; }
            set { this.annotations = value; }
        }

        [JsonProperty("subshapeFileLayers")]
        public List<ShapeFileLayer> SubshapeFileLayers
        {
            get { return this.subshapeFileLayers; }
            set { this.subshapeFileLayers = value; }
        }

        [JsonProperty("shapeSetting")]
        public ShapeSetting ShapeSetting
        {
            get { return this.shapeSetting; }
            set { this.shapeSetting = value; }
        }

        [JsonProperty("bubbleSetting")]
        public BubbleSetting BubbleSetting
        {
            get { return this.bubbleSetting; }
            set { this.bubbleSetting = value; }
        }

        [JsonProperty("shapeIdTableField")]
        [DefaultValue(null)]
        public string ShapeIdTableField
        {
            get { return this.shapeIdTableField; }
            set { this.shapeIdTableField = value; }
        }

        [JsonProperty("shapeIdPath")]
        [DefaultValue(null)]
        public string ShapeIdPath
        {
            get { return this.shapeIdPath; }
            set { this.shapeIdPath = value; }
        }


        #endregion

        public bool ShouldSerializeDataSource()
        {
            if (typeof (DataSource).IsAssignableFrom(this.DataSource.GetType()))
            {
                if (Utils.PropertyCompare(DataSource, new DataSource()))
                    return true;
                else
                    return false;
            }
            else if (this.DataSource is IEnumerable)
            {
                ICollection data = DataSource as ICollection;
                if (data.Count != 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public bool ShouldSerializeData()
        {
            if (typeof (DataSource).IsAssignableFrom(this.Data.GetType()))
            {
                if (Utils.PropertyCompare(Data, new DataSource()))
                    return true;
                else
                    return false;
            }
            else if (this.Data is IEnumerable)
            {
                ICollection data = this.Data as ICollection;
                if (data.Count != 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }



        public bool ShouldSerializeAnnotationDataSource()
        {
            if (typeof (DataSource).IsAssignableFrom(this.AnnotationDataSource.GetType()))
            {
                if (Utils.PropertyCompare(DataSource, new DataSource()))
                    return true;
                else
                    return false;
            }
            else if (this.AnnotationDataSource is IEnumerable)
            {
                ICollection data = AnnotationDataSource as ICollection;
                if (data.Count != 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
   
