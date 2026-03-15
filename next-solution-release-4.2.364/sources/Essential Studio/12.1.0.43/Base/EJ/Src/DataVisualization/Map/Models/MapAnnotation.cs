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

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class MapAnnotation
    {
        #region Fields

        private string annotationLabel = "Annotation";
        private double annotationLabelFontSize=12;
        private string annotationLabelForeground = "black";
        private double latitude=  0;
        private double longitude= 0 ;

        #endregion


        #region Properties

        [JsonProperty("annotationLabel")]
        [DefaultValue("Annotation")]
        public string AnnotationLabel
        {
            get { return this.annotationLabel; }
            set { this.annotationLabel = value; }
        }

        [JsonProperty("annotationLabelForeground")]
        [DefaultValue("black")]
        public string AnnotationLabelForeground
        {
            get { return this.annotationLabelForeground; }
            set { this.annotationLabelForeground = value; }
        }


        [JsonProperty("latitude")]
        [DefaultValue(0)]
        public double Latitude
        {
            get { return this.latitude; }
            set { this.latitude = value; }
        }

        [JsonProperty("longitude")]
        [DefaultValue(0)]
        public double Longitude
        {
            get { return this.longitude; }
            set { this.longitude = value; }
        }

        [JsonProperty("annotationLabelFontSize")]
        [DefaultValue(12)]
        public double AnnotationLabelFontSize
        {
            get { return this.annotationLabelFontSize; }
            set { this.annotationLabelFontSize = value; }
        }

        #endregion
    }

    public class MapAnnotations
    {
        private MapAnnotation mapAnnotation = null;

        public MapAnnotations()
        {
            this.mapAnnotation = new MapAnnotation();
        }

        [JsonProperty("mapAnnotation")]
        public MapAnnotation MapAnnotation
        {
            get { return this.mapAnnotation; }
            set { this.mapAnnotation = value; }
        }
    }

}
