//-------------------------------------------------------------------------------------------------
// <copyright file="GaugeScale.cs" company="syncfusion">
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
    public class GaugeScale
    {
        #region Members

        private GaugePointers gaugePointers = null;

        private ScaleRanges scaleRanges = null;

        private Style style = null;

        private CustomLabels customLabels = null;

        private MaximumValue maximumValue = null;

        private MinimumValue minimumValue = null;

        private GaugeMajorTickMarks gaugeMajorTickMarks = null;

        private GaugeMinorTickMarks gaugeMinorTickMarks = null;

        private MaximumPin maximumPin = null;     

        private MinimumPin minimumPin = null;   

        private ScaleLabels scaleLabels = null;

        private ActionInfo actionInfo = null;

        private string name;

        private float interval;

        private float intervalOffset;

        private bool logarithmic;

        private float logarithmicBase = 10;

        private float multiplier;

        private bool reversed;

        private bool tickMarksOnTop;

        private string toolTip = string.Empty;

        private bool hidden;

        private float width;

        #endregion

        #region Public Properties

        [XmlArrayItem("RadialPointer", typeof(RadialPointer))]
        [XmlArrayItem("LinearPointer", typeof(LinearPointer))]
        public GaugePointers GaugePointers
        {
            get { return gaugePointers; }

            set { gaugePointers = value; }
        }

        public ScaleRanges ScaleRanges
        {
            get { return scaleRanges; }

            set { scaleRanges = value; }
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

        public CustomLabels CustomLabels
        {
            get { return customLabels; }
            set { customLabels = value; }
        }

        public MaximumValue MaximumValue
        {
            get { return maximumValue; }
            set { maximumValue = value; }
        }

        public MinimumValue MinimumValue
        {
            get { return minimumValue; }
            set { minimumValue = value; }
        }

        public GaugeMajorTickMarks GaugeMajorTickMarks
        {
            get { return gaugeMajorTickMarks; }
            set { gaugeMajorTickMarks = value; }
        }

        public GaugeMinorTickMarks GaugeMinorTickMarks
        {
            get { return gaugeMinorTickMarks; }
            set { gaugeMinorTickMarks = value; }
        }

        public MaximumPin MaximumPin
        {
            get { return maximumPin; }
            set { maximumPin = value; }
        }

        public MinimumPin MinimumPin
        {
            get { return minimumPin; }
            set { minimumPin = value; }
        }

        public ScaleLabels ScaleLabels
        {
            get { return scaleLabels; }
            set { scaleLabels = value; }
        }

        public ActionInfo ActionInfo
        {
            get { return actionInfo; }
            set { actionInfo = value; }
        }

        [XmlAttribute()]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public float Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public float IntervalOffset
        {
            get { return intervalOffset; }
            set { intervalOffset = value; }
        }

        public bool Logarithmic
        {
            get { return logarithmic; }
            set { logarithmic = value; }
        }

        public float LogarithmicBase
        {
            get { return logarithmicBase; }
            set { logarithmicBase = value; }
        }

        public float Multiplier
        {
            get { return multiplier; }
            set { multiplier = value; }
        }

        public bool Reversed
        {
            get { return reversed; }
            set { reversed = value; }
        }

        public bool TickMarksOnTop
        {
            get { return tickMarksOnTop; }
            set { tickMarksOnTop = value; }
        }

        public string ToolTip
        {
            get { return toolTip; }
            set { toolTip = value; }
        }

        public bool Hidden
        {
            get { return hidden; }
            set { hidden = value; }
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        #endregion

        #region Constructors
        public GaugeScale()
        {
        }
        #endregion

        public bool ShouldSerializeGaugePointers()
        {
            return GaugePointers != null && GaugePointers.Count > 0;
        }

        public void ResetGaugePointers()
        {
            this.GaugePointers = new GaugePointers();
        }

        public bool ShouldSerializeScaleRanges()
        {
            return ScaleRanges != null && ScaleRanges.Count > 0;
        }

        public void ResetScaleRanges()
        {
            this.ScaleRanges = new ScaleRanges();
        }
    }
}
