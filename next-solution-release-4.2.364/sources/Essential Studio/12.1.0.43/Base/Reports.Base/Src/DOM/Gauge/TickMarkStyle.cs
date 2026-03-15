//-------------------------------------------------------------------------------------------------
// <copyright file="TickMarkStyle.cs" company="syncfusion">
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
    public class TickMarkStyle
    {
        #region private properties

        private float gradientDensity;

        #endregion

        #region public properties
        public Style Style
        {
            get;
            set;
        }

        public float DistanceFromScale
        {
            get;
            set;
        }

        public Placement Placement
        {
            get;
            set;
        }

        public bool EnableGradient
        {
            get;
            set;
        }

        public float GradientDensity
        {
            get { return gradientDensity; }
            set 
            {
                if (value >= 0 && value <= 100)
                {
                    gradientDensity = value;
                }
                else
                {
                    throw new Exception("Gradient Density value should be with in range of 0 to 100");
                }
            }
        }

        public TickMarkImage TickMarkImage
        {
            get;
            set;
        }

        public float Length
        {
            get;
            set;
        }

        public float Width
        {
            get;
            set;
        }

        public Shape Shape
        {
            get;
            set;
        }

        public bool Hidden
        {
            get;
            set;
        }

        #endregion

        public TickMarkStyle()
        {
        }

    }
}