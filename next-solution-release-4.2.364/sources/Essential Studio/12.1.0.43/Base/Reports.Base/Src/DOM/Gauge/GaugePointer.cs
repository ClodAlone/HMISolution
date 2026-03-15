//-------------------------------------------------------------------------------------------------
// <copyright file="GaugePointer.cs" company="syncfusion">
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
    public class GaugePointer
    {
        #region Members

        private Style style = null;

        private GaugeInputValue gaugeInputValue = null;

        private PointerImage pointerImage = null;

        private ActionInfo actionInfo = null;

        private BarStart barStart = BarStart.ScaleStart;

        private MarkerStyle markerStyle = MarkerStyle.Triangle;

        private Placement placement;

        private string name;

        private float distanceFromScale;

        private float markerLength;

        private bool snappingEnabled;

        private float snappingInterval;

        private string toolTip;

        private bool hidden;

        private float width;

        #endregion

        #region Public Properties

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

        public GaugeInputValue GaugeInputValue
        {
            get { return gaugeInputValue; }

            set { gaugeInputValue = value; }
        }

        public PointerImage PointerImage
        {
            get { return pointerImage; }

            set { pointerImage = value; }
        }

        public ActionInfo ActionInfo
        {
            get { return actionInfo; }

            set { actionInfo = value; }
        }

        public BarStart BarStart
        {
            get { return barStart; }

            set { barStart = value; }
        }

        public MarkerStyle MarkerStyle
        {
            get { return markerStyle; }

            set { markerStyle = value; }
        }

        public Placement Placement
        {
            get { return placement; }

            set { placement = value; }
        }

        [XmlAttribute()]
        public string Name
        {
            get { return name; }

            set { name = value; }
        }

        public float DistanceFromScale
        {
            get { return distanceFromScale; }

            set { distanceFromScale = value; }
        }

        public float MarkerLength
        {
            get { return markerLength; }

            set { markerLength = value; }
        }

        public bool SnappingEnabled
        {
            get { return snappingEnabled; }

            set { snappingEnabled = value; }
        }

        public float SnappingInterval
        {
            get { return snappingInterval; }

            set { snappingInterval = value; }
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

        public GaugePointer()
        {
        }

        #endregion
    }
}
