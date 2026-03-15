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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class ShapeFileLayerBuilder
    {
        public ShapeFileLayer shapeFileLayer;

        public MapProperties mapProperties;

        public List<ShapeFileLayer> layers;

        public ShapeFileLayerBuilder(ShapeFileLayer shapeFileLayer, MapProperties mapProperties)
        {
            this.mapProperties = mapProperties;
            this.shapeFileLayer = shapeFileLayer;

            if (this.layers == null)
            {
                this.layers = new List<ShapeFileLayer>();
            }

            this.mapProperties.Layers = this.layers;
        }

        public ShapeFileLayerBuilder(ShapeFileLayer shapeFileLayer, ShapeFileLayer sublayer)
        {
            this.shapeFileLayer = shapeFileLayer;

            if (this.layers == null)
            {
                this.layers = new List<ShapeFileLayer>();
            }

            sublayer.SubshapeFileLayers = this.layers;
        }

        public void Add()
        {
            if (this.layers != null)
            {
                this.layers.Add(this.shapeFileLayer);
                this.shapeFileLayer =new ShapeFileLayer();
            }
        }

        public ShapeFileLayerBuilder EnableSelection(bool enableSelection)
        {

            shapeFileLayer.EnableSelection = enableSelection;
            return this;
        }

        public ShapeFileLayerBuilder EnableMouseHover(bool enableMouseHover)
        {
            shapeFileLayer.EnableMouseHover = enableMouseHover;
            return this;
        }
        
        public ShapeFileLayerBuilder ShowToolTip(bool tooltipVisibility)
        {
            shapeFileLayer.ShowToolTip = tooltipVisibility;
            return this;
        }

        public ShapeFileLayerBuilder EnableAnimation(bool enableAnimation)
        {
            shapeFileLayer.EnableAnimation = enableAnimation;
            return this;
        }

        public ShapeFileLayerBuilder ShowMapItems(bool mapItemsVisibility)
        {
            shapeFileLayer.ShowMapItems = mapItemsVisibility;
            return this;
        }
        
        public ShapeFileLayerBuilder Data(object data)
        {
            shapeFileLayer.Data = data;
            return this;
        }

        public ShapeFileLayerBuilder DataSource(object dataSource)
        {
            shapeFileLayer.DataSource = dataSource;
            return this;
        }

        public ShapeFileLayerBuilder AnnotationDataSource(object annotationDataSource)
        {
            shapeFileLayer.AnnotationDataSource = annotationDataSource;
            return this;
        }
             

        public ShapeFileLayerBuilder AnnotationTemplate(string annotationTemplate)
        {
            shapeFileLayer.AnnotationTemplate = annotationTemplate;
            return this;
        }

        public ShapeFileLayerBuilder TooltipTemplate(string tooltipTemplate)
        {
            shapeFileLayer.TooltipTemplate = tooltipTemplate;
            return this;
        }

        public ShapeFileLayerBuilder MapItemsTemplate(string mapItemsTemplate)
        {
            shapeFileLayer.MapItemsTemplate = mapItemsTemplate;
            return this;
        }

        public ShapeFileLayerBuilder ShapeIdTableField(string shapeIdTableField)
        {
            shapeFileLayer.ShapeIdTableField = shapeIdTableField;
            return this;
        }

        public ShapeFileLayerBuilder ShapeIDPath(string shapeIdPath)
        {
            shapeFileLayer.ShapeIdPath = shapeIdPath;
            return this;
        }

        public ShapeFileLayerBuilder Annotations(Action<MapAnnotationBuilder> annotations)
        {
            var obj = new MapAnnotations();
            obj.MapAnnotation = new MapAnnotation();
            var builder = new MapAnnotationBuilder(obj, shapeFileLayer);
            if (annotations != null)
                annotations.Invoke(builder);
            return this;
        }

        public ShapeFileLayerBuilder SubshapeFileLayers(Action<ShapeFileLayerBuilder> sublayers)
        {
            var obj = new ShapeFileLayer();
            var builder = new ShapeFileLayerBuilder(obj, shapeFileLayer);
            if (sublayers != null)
                sublayers.Invoke(builder);
            return this;
        }

        public ShapeFileLayerBuilder ShapeSetting(Action<ShapeSettingBuilder> shapeSetting)
        {
            var obj = new ShapeSetting();

            var builder = new ShapeSettingBuilder(obj, shapeFileLayer);
            if (shapeSetting != null)
                shapeSetting.Invoke(builder);
            return this;
        }

        public ShapeFileLayerBuilder BubbleSetting(Action<BubbleSettingBuilder> bubbleSetting)
        {
            var obj = new BubbleSetting();

            var builder = new BubbleSettingBuilder(obj, shapeFileLayer);
            if (bubbleSetting != null)
                bubbleSetting.Invoke(builder);
            return this;
        }

        public ShapeFileLayerBuilder LabelSetting(Action<MapLabelSettingBuilder> labelSetting)
        {
            var obj = new MapLabelSetting();

            var builder = new MapLabelSettingBuilder(obj,this.shapeFileLayer);
            if (labelSetting != null)
                labelSetting.Invoke(builder);
            return this;
        }

        public ShapeFileLayerBuilder LegendSetting(Action<LegendSettingBuilder> legendSetting)
        {
            var obj = new MapLegendSetting();

            var builder = new LegendSettingBuilder(obj,this.shapeFileLayer);
            if (legendSetting != null)
                legendSetting.Invoke(builder);
            return this;
        }


        public ShapeFileLayerBuilder MapType(MapType mapType)
        {
            shapeFileLayer.MapType = mapType;
            return this;
        }

    }
}
