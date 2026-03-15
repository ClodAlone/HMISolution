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
using Syncfusion.Reports.Base.DOM;

namespace Syncfusion.RDL.DOM
{
    public class StateIndicator : GaugePanelItem
    {

        #region Members

        private GaugeStateIndicatorStyles indicatorStyle;

        private int scaleFactor;

        private int angle;

        private TransformationType transformationType;

        private GaugeInputValue maximumValue;

        private GaugeInputValue minimumValue;

        private GaugeInputValue gaugeInputValue;

        private StateIndicatorIconsSet iconsSet;

        private List<IndicatorState> indicatorStates;

        private Style style;

        #endregion

        #region Public Properties

        public GaugeStateIndicatorStyles IndicatorStyle
        {
            get { return indicatorStyle; }

            set { indicatorStyle = value; }
        }

        public int Angle
        {
            get { return angle; }

            set { angle = value; }
        }

        public int ScaleFactor
        {
            get { return scaleFactor; }

            set { scaleFactor = value; }
        }

        public TransformationType TransformationType
        {
            get { return transformationType; }

            set { transformationType = value; }
        }

        public GaugeInputValue MaximumValue
        {
            get { return maximumValue; }

            set { maximumValue = value; }
        }

        public GaugeInputValue MinimumValue
        {
            get { return minimumValue; }

            set { minimumValue = value; }
        }

        public GaugeInputValue GaugeInputValue
        {
            get { return gaugeInputValue; }

            set { gaugeInputValue = value; }
        }

        public StateIndicatorIconsSet IconsSet
        {
            get { return iconsSet; }

            set { iconsSet = value; }
        }


        public List<IndicatorState> IndicatorStates
        {
            get { return indicatorStates; }

            set { indicatorStates = value; }
        }

        public Style Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }


        #endregion

        public StateIndicator()
        {

        }
    }   
}
