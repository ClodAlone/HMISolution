//-------------------------------------------------------------------------------------------------
// <copyright file="GaugeLabel.cs" company="syncfusion">
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
    public class GaugeLabel : GaugePanelItem
    {
        private float angle;

        private ResizeMode resizeMode = ResizeMode.AutoFit;

        private Style style = null;

        private string text = string.Empty;

        private Size textShadowOffset;

        private bool useFontPercent;

        public GaugeLabel()
        {
        }

        public float Angle
        {
            get { return angle; }

            set { angle = value; }
        }

        public ResizeMode ResizeMode
        {
            get { return resizeMode; }

            set { resizeMode = value; }
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

        public string Text
        {
            get { return text; }

            set { text = value; }
        }

        public Size TextShadowOffset
        {
            get { return textShadowOffset; }

            set { textShadowOffset = value; }
        }

        public bool UseFontPercent
        {
            get { return useFontPercent; }

            set { useFontPercent = value; }
        }
    }
}
