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
using System.Xml.Serialization;
using Syncfusion.RDL.DOM;

namespace Syncfusion.Reports.Base.DOM
{
    public class IndicatorState
    {
        private string name;

        private GaugeInputValue startValue;

        private GaugeInputValue endValue;

        private string color;

        private double scaleFactor;

        private GaugeStateIndicatorStyles indicatorStyle;

        private IndicatorImage indicatorImage;

        private ResizeModes resizeMode;

        [XmlAttribute()]
        public string Name
        {
            get { return name; }

            set { name = Name; }
        }

        public GaugeInputValue StartValue
        {
            get { return startValue; }

            set { startValue = value; }
        }

        public GaugeInputValue EndValue
        {
            get { return endValue; }

            set { endValue = value; }
        }

        public string Color
        {
            get { return color; }

            set { color = value; }
        }

        public ResizeModes ResizeMode
        {
            get { return resizeMode; }

            set { resizeMode = value; }
        }

        public double ScaleFactor
        {
            get { return scaleFactor; }

            set { scaleFactor = value; }
        }

        public GaugeStateIndicatorStyles IndicatorStyle
        {
            get { return indicatorStyle; }

            set { indicatorStyle = value; }
        }

        public IndicatorImage IndicatorImage
        {
            get { return indicatorImage; }

            set { indicatorImage = value; }
        }

        public IndicatorState()
        {

        }
    }
}
