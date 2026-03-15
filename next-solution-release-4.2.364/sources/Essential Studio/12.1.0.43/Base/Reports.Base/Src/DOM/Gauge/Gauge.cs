//-------------------------------------------------------------------------------------------------
// <copyright file="Gauge.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class Gauge : GaugePanelItem
    {
        private float aspectRatio;

        private BackFrame backFrame = null;

        private bool clipContent;

        private GaugeScales gaugeScales = null;

        private TopImage topImage = null;

        public Gauge()
        {
        }

        public float AspectRatio
        {
            get { return aspectRatio; }

            set { aspectRatio = value; }
        }

        public BackFrame BackFrame
        {
            get { return backFrame; }

            set { backFrame = value; }
        }

        public bool ClipContent
        {
            get { return clipContent; }

            set { clipContent = value; }
        }

        [XmlArrayItem("RadialScale", typeof(RadialScale))]
        [XmlArrayItem("LinearScale", typeof(LinearScale))]
        public GaugeScales GaugeScales
        {
            get { return gaugeScales; }

            set { gaugeScales = value; }
        }

        public TopImage TopImage
        {
            get { return topImage; }

            set { topImage = value; }
        }
    }
}
