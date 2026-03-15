//-------------------------------------------------------------------------------------------------
// <copyright file="FrameImage.cs" company="syncfusion">
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
    public class FrameImage : BaseGaugeImage
    {
        private bool clipImage;

        private string hueColor;

        private float transparency;

        public FrameImage()
        {
        }

        public bool ClipImage
        {
            get { return clipImage; }

            set { clipImage = value; }
        }

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
    }
}
