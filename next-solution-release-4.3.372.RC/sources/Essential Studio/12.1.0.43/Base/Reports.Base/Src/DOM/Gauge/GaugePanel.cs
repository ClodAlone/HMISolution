//-------------------------------------------------------------------------------------------------
// <copyright file="GaugePanel.cs" company="syncfusion">
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
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class GaugePanel : DataRegion
    {
        #region Members

        private LinearGauges linearGauges = null;

        private RadialGauges radialGauges = null;

        private StateIndicators stateIndicators = null;

        private GaugeLabels gaugeLabels = null;

        private GaugeMember gaugeMember = null;

        private BackFrame backFrame = null;

        private TopImage topImage = null;

        private AntiAliasing antiAliasing;

        private TextAntiAliasingQuality textAntiAliasingQuality;

        private bool autoLayout;

        private float shadowIntensity = 25;

        #endregion

        #region Public Properties

        public LinearGauges LinearGauges
        {
            get { return linearGauges; }

            set { linearGauges = value; }
        }

        public RadialGauges RadialGauges
        {
            get { return radialGauges; }

            set { radialGauges = value; }
        }

        public StateIndicators StateIndicators
        {
            get { return stateIndicators; }

            set { stateIndicators = value; }
        }

        public GaugeLabels GaugeLabels
        {
            get { return gaugeLabels; }

            set { gaugeLabels = value; }
        }

        public GaugeMember GaugeMember
        {
            get { return gaugeMember; }

            set { gaugeMember = value; }
        }

        public BackFrame BackFrame
        {
            get { return backFrame; }

            set { backFrame = value; }
        }

        public TopImage TopImage
        {
            get { return topImage; }

            set { topImage = value; }
        }

        public AntiAliasing AntiAliasing
        {
            get { return antiAliasing; }

            set { antiAliasing = value; }
        }

        public TextAntiAliasingQuality TextAntiAliasingQuality
        {
            get { return textAntiAliasingQuality; }

            set { textAntiAliasingQuality = value; }
        }

        public bool AutoLayout
        {
            get { return autoLayout; }

            set { autoLayout = value; }
        }

        public float ShadowIntensity
        {
            get { return shadowIntensity; }

            set { shadowIntensity = value; }
        }
     
        #endregion

        #region Constructor Methods

        public GaugePanel()
        {
        }

        #endregion

    }

}