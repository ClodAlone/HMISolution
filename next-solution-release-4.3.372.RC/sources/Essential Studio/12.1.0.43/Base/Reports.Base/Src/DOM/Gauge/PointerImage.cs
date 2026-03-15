//-------------------------------------------------------------------------------------------------
// <copyright file="PointerImage.cs" company="syncfusion">
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
    public class PointerImage : BaseGaugeImage
    {
        #region Members

        private string hueColor;

        private float transparency;

        private Size offsetX;

        private Size offsetY;

        #endregion

        #region Public Properties

        public string HueColor
        {
            get { return hueColor; }

            set { hueColor = value; }
        }

        public float Transparency
        {
            get { return transparency; }

            set { transparency = value; }
        }

        public Size OffsetX
        {
            get { return offsetX; }

            set { offsetX = value; }
        }

        public Size OffsetY
        {
            get { return offsetY; }

            set { offsetY = value; }
        }

        #endregion

        #region Constructors
        public PointerImage()
        {
        }
        #endregion
    }
}
