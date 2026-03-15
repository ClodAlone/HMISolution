#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
    public class MapProperties
    {
        #region Fields

        private double minZoom = 1;
        private double maxZoom = 100;
        private double zoomFactor = 1;
        private double zoomLevel = 1;
        private bool enablePan = true;
        private bool enableZoom = true;
        private int baseMapIndex = 1;
        private string zoomedIn = null;
        private string zoomedOut = null;
        private string panning = null;
        private string panned = null;
        private string shapeSelected = null;
        private bool enableAnimation = false;
        private List<ShapeFileLayer> layers=null;
        private string mapBackground = null;
        private string onRenderComplete = null;
        private bool canResize = false;
        private bool enableLayerChangeAnimation = false;
        private bool zoomOnSelection = false;
        private NavigationControl navigationControl = null;
        private ShapePoint centerPosition = new ShapePoint();
        private object dataSource = new object();
        private string shapeUnHovered = null;
        private string shapeHovered = null;
        private string annotationSelected = null;
        #endregion

        #region Properties
              

        [JsonProperty("minZoom")]
        [DefaultValue(1)]
        public double MinZoom
        {
            get { return this.minZoom; }
            set { this.minZoom = value; }
        }

        [JsonProperty("maxZoom")]
        [DefaultValue(100)]
        public double MaxZoom
        {
            get { return this.maxZoom; }
            set { this.maxZoom = value; }
        }

        [JsonProperty("zoomFactor")]
        [DefaultValue(1)]
        public double ZoomFactor
        {
            get { return this.zoomFactor; }
            set { this.zoomFactor = value; }
        }

        [JsonProperty("zoomLevel")]
        [DefaultValue(1)]
        public double ZoomLevel
        {
            get { return this.zoomLevel; }
            set { this.zoomLevel = value; }
        }

        

        [JsonProperty("baseMapIndex")]
        [DefaultValue(0)]
        public int BaseMapIndex
        {
            get { return this.baseMapIndex; }
            set { this.baseMapIndex = value; }
        }

        [JsonProperty("enablePan")]
        [DefaultValue(true)]
        public bool EnablePan
        {
            get { return this.enablePan; }
            set { this.enablePan = value; }
        }

        [JsonProperty("enableZoom")]
        [DefaultValue(true)]
        public bool EnableZoom
        {
            get { return this.enableZoom; }
            set { this.enableZoom = value; }
        }

        [JsonProperty("canResize")]
        [DefaultValue(true)]
        public bool CanResize
        {
            get { return this.canResize; }
            set { this.canResize = value; }
        }

        [JsonProperty("enableLayerChangeAnimation")]
        [DefaultValue(false)]
        public bool EnableLayerChangeAnimation
        {
            get { return this.enableLayerChangeAnimation; }
            set { this.enableLayerChangeAnimation = value; }
        }

        [JsonProperty("zoomOnSelection")]
        [DefaultValue(false)]
        public bool ZoomOnSelection
        {
            get { return this.zoomOnSelection; }
            set { this.zoomOnSelection = value; }
        }

        [JsonProperty("navigationControl")]
        public NavigationControl NavigationControl
        {
            get { return this.navigationControl; }
            set { this.navigationControl = value; }
        }

        [JsonProperty("enableAnimation")]
        [DefaultValue(false)]
        public bool EnableAnimation
        {
            get { return this.enableAnimation; }
            set { this.enableAnimation = value; }
        }

        [JsonProperty("layers")]
        public List<ShapeFileLayer> Layers
        {
            get { return this.layers; }
            set { this.layers = value; }
        }

        [JsonProperty("zoomedIn")]
        [DefaultValue("")]
        public string ZoomedIn
        {
            get { return this.zoomedIn; }
            set { this.zoomedIn = value; }
        }

        [JsonProperty("zoomedOut")]
        [DefaultValue("")]
        public string ZoomedOut
        {
            get { return this.zoomedOut; }
            set { this.zoomedOut = value; }
        }

        [JsonProperty("panning")]
        [DefaultValue("")]
        public string Panning
        {
            get { return this.panning; }
            set { this.panning = value; }
        }

        [JsonProperty("panned")]
        [DefaultValue("")]
        public string Panned
        {
            get { return this.panned; }
            set { this.panned = value; }
        }

        [JsonProperty("shapeSelected")]
        [DefaultValue("")]
        public string ShapeSelected
        {
            get { return this.shapeSelected; }
            set { this.shapeSelected = value; }
        }

        [JsonProperty("mapBackground")]
        [DefaultValue("white")]
        public string MapBackground
        {
            get { return this.mapBackground; }
            set { this.mapBackground = value; }
        }

        [JsonProperty("onRenderComplete")]
        [DefaultValue("")]
        public string OnRenderComplete
        {
            get { return this.onRenderComplete; }
            set { this.onRenderComplete = value; }
        }

        [JsonProperty("shapeHovered")]
        [DefaultValue("")]
        public string ShapeHovered
        {
            get { return this.shapeHovered; }
            set { this.shapeHovered = value; }
        }

        [JsonProperty("shapeUnHovered")]
        [DefaultValue("")]
        public string ShapeUnHovered
        {
            get { return this.shapeUnHovered; }
            set { this.shapeUnHovered = value; }
        }

        [JsonProperty("annotationSelected")]
        [DefaultValue("")]
        public string AnnotationSelected
        {
            get { return this.annotationSelected; }
            set { this.annotationSelected = value; }
        }

        [JsonProperty("centerPosition")]
        public ShapePoint CenterPosition
        {
            get { return this.centerPosition; }
            set { this.centerPosition = value; }
        }

        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }
        #endregion

        public bool ShouldSerializeLayers()
        {
            return this.layers != null && layers.Count > 0;
        }

        
    }
}
