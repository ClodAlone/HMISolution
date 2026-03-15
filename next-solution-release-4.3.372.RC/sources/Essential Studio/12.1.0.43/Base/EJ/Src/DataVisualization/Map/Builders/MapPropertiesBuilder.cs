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
using System.Web;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class MapPropertiesBuilder
    {
        public Map map;

        public MapPropertiesBuilder(Map map)
        {
            this.map = new Map(map.ID, map.MapModel);
        }    

       
        public MapPropertiesBuilder MinZoom(double minZoom)
        {
            map.MapModel.MinZoom = minZoom;
            return this;
        }
        
        public MapPropertiesBuilder MaxZoom(double maxZoom)
        {
            map.MapModel.MaxZoom = maxZoom;
            return this;
        }

        public MapPropertiesBuilder ZoomFactor(double zoomFactor)
        {
            map.MapModel.ZoomFactor = zoomFactor;
            return this;
        }

        public MapPropertiesBuilder ZoomLevel(double zoomLevel)
        {
            map.MapModel.ZoomLevel = zoomLevel;
            return this;
        }

        public MapPropertiesBuilder EnablePan(bool enabelPan)
        {
            map.MapModel.EnablePan = enabelPan;
            return this;
        }

        public MapPropertiesBuilder EnableZoom(bool enableZoom)
        {
            map.MapModel.EnableZoom = enableZoom;
            return this;
        }

        public MapPropertiesBuilder BaseMapIndex(int baseMapIndex)
        {
            map.MapModel.BaseMapIndex = baseMapIndex;
            return this;
        }

        public MapPropertiesBuilder ZoomedIn(string zoomedIn)
        {
            map.MapModel.ZoomedIn = zoomedIn;
            return this;
        }  

        public MapPropertiesBuilder ZoomedOut(string zoomedOut)
        {
            map.MapModel.ZoomedOut = zoomedOut;
            return this;
        }   

        public MapPropertiesBuilder Panning(string panning)
        {
            map.MapModel.Panning = panning;
            return this;
        }

        public MapPropertiesBuilder Panned(string panned)
        {
            map.MapModel.Panned = panned;
            return this;
        }

        public MapPropertiesBuilder ShapeSelected(string shapeSelected)
        {
            map.MapModel.ShapeSelected = shapeSelected;
            return this;
        }

        public MapPropertiesBuilder EnableAnimation(bool enableAnimation)
        {
            map.MapModel.EnableAnimation = enableAnimation;
            return this;
        }

        public MapPropertiesBuilder Layers(Action<ShapeFileLayerBuilder> layers)
        {
            var obj = new ShapeFileLayer();
            var builder = new ShapeFileLayerBuilder(obj, map.MapModel);
            if (layers != null)
                layers.Invoke(builder);
            return this;
        }

        public MapPropertiesBuilder MapBackground(string mapBackground)
        {
            map.MapModel.MapBackground = mapBackground;
            return this;
        }

        public MapPropertiesBuilder OnRenderComplete(string onRenderComplete)
        {
            map.MapModel.OnRenderComplete = onRenderComplete;
            return this;
        }

        public MapPropertiesBuilder ShapeHovered(string shapehover)
        {
            map.MapModel.ShapeHovered = shapehover;
            return this;
        }

        public MapPropertiesBuilder ShapeUnHovered(string shapeunhover)
        {
            map.MapModel.ShapeUnHovered = shapeunhover;
            return this;
        }

        public MapPropertiesBuilder AnnotationSelected(string annotationselected)
        {
            map.MapModel.AnnotationSelected = annotationselected;
            return this;
        }

        public MapPropertiesBuilder CanResize(bool canResize)
        {
            map.MapModel.CanResize = canResize;
            return this;
        }

        public MapPropertiesBuilder EnableLayerChangeAnimation(bool enableLayerChangeAnimation)
        {
            map.MapModel.EnableLayerChangeAnimation = enableLayerChangeAnimation;
            return this;
        }

        public MapPropertiesBuilder ZoomOnSelection(bool zoomOnSelection)
        {
            map.MapModel.ZoomOnSelection = zoomOnSelection;
            return this;
        }


        public MapPropertiesBuilder NavigationControl(Action<NavigationControlBuilder> navigationControl)
        {
            var obj = new NavigationControl();
            var builder = new NavigationControlBuilder(obj, map.MapModel);
            if (navigationControl != null)
                navigationControl.Invoke(builder);
            return this;
        }

        public MapPropertiesBuilder CenterPosition(ShapePoint centerPosition)
        {
            map.MapModel.CenterPosition = centerPosition;
            return this;
        }

        public MapPropertiesBuilder DataSource(object DataSource)
        {
            map.MapModel.DataSource = DataSource;
            return this;
        }

        public HtmlString Render()
        {
            return new HtmlString(map.Render().ToString());
        }

        public override String ToString()
        {
            return Render().ToString();
        }
    }
}
