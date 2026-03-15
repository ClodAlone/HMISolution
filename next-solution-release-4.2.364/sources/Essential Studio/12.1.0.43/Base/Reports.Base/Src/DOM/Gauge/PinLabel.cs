//-------------------------------------------------------------------------------------------------
// <copyright file="PinLabel.cs" company="syncfusion">
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
    public class PinLabel
    {
        #region Members

        private Style style = null;

        private Placement placement = Placement.Inside;

        private string text = string.Empty;

        private bool allowUpsideDown;

        private float distanceFromScale;

        private float fontAngle;

        private bool rotateLabel;

        private bool useFontPercent;

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

        public Placement Placement
        {
            get { return placement; }

            set { placement = value; }
        }

        public string Text
        {
            get { return text; }

            set { text = value; }
        }

        public bool AllowUpsideDown
        {
            get { return allowUpsideDown; }

            set { allowUpsideDown = value; }
        }

        public float DistanceFromScale
        {
            get { return distanceFromScale; }

            set { distanceFromScale = value; }
        }

        public float FontAngle
        {
            get { return fontAngle; }

            set { fontAngle = value; }
        }

        public bool RotateLabel
        {
            get { return rotateLabel; }

            set { rotateLabel = value; }
        }

        public bool UseFontPercent
        {
            get { return useFontPercent; }

            set { useFontPercent = value; }
        }

        #endregion

        #region Constructors
        public PinLabel()
        {
        }
        #endregion
    }
}
