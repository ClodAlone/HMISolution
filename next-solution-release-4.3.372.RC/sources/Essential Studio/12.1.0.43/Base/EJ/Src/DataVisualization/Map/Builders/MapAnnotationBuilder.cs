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
    public class MapAnnotationBuilder
    {
        public MapAnnotations mapAnnotation;

        public ShapeFileLayer shapeFileLayer;

        public List<MapAnnotations> MapAnnotationses;


        public MapAnnotationBuilder(MapAnnotations mapAnnotations,ShapeFileLayer layer)
        {
            this.mapAnnotation = mapAnnotations;
            this.shapeFileLayer = layer;

            if (this.MapAnnotationses == null)
            {
                this.MapAnnotationses = new List<MapAnnotations>();
            }

            this.shapeFileLayer.Annotations = this.MapAnnotationses;
        }

        public void Add()
        {
            if (this.MapAnnotationses != null)
            {
                this.MapAnnotationses.Add(this.mapAnnotation);
                this.mapAnnotation = new MapAnnotations();
                this.mapAnnotation.MapAnnotation = new MapAnnotation();
            }
        }

        public MapAnnotationBuilder AnnotationLabel(string annotationLabel)
        {
            this.mapAnnotation.MapAnnotation.AnnotationLabel = annotationLabel;
            return this;
        }

        public MapAnnotationBuilder AnnotationLabelFontSize(double annotationLabelFontSize)
        {
            this.mapAnnotation.MapAnnotation.AnnotationLabelFontSize = annotationLabelFontSize;
            return this;
        }

        public MapAnnotationBuilder AnnotationLabelForeground(string annotationLabelForeground)
        {
            this.mapAnnotation.MapAnnotation.AnnotationLabelForeground = annotationLabelForeground;
            return this;
        }

        public MapAnnotationBuilder Latitude(double latitude)
        {
            this.mapAnnotation.MapAnnotation.Latitude = latitude;
            return this;
        }

        public MapAnnotationBuilder Longitude(double longitude)
        {
            this.mapAnnotation.MapAnnotation.Longitude = longitude;
            return this;
        }
    }
} 
