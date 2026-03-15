//-------------------------------------------------------------------------------------------------
// <copyright file="RadialScale.cs" company="syncfusion">
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
    public class RadialScale : GaugeScale
    {
        #region Members

        private float radius = 37;

        private float startAngle = 20;

        private float sweepAngle = 320;

        #endregion

        #region Public Properties

        public float Radius
        {
            get { return radius; }

            set { radius = value; }
        }

        public float StartAngle
        {
            get 
            { 
                return startAngle; 
            }

            set
            {
                if (value >= 0 && value <= 360)
                {
                    startAngle = value;
                }
                else
                {
                    throw new Exception("The start angle of the scale in degrees should be within the range of (0-360).");
                }
            }
        }

        public float SweepAngle
        {
            get 
            { 
                return sweepAngle; 
            }

            set
            {
                if (value >= 0 && value <= 360)
                {
                    sweepAngle = value;
                }
                else
                {
                    throw new Exception("The start angle of the scale in degrees should be within the range of (0-360).");
                }
            }
        }

        #endregion

        #region Constructors
        public RadialScale()
        {
        }
        #endregion
    }
}
