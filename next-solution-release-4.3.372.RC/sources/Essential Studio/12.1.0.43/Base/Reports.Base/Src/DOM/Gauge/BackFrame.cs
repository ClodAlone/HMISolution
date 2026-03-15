//-------------------------------------------------------------------------------------------------
// <copyright file="BackFrame.cs" company="syncfusion">
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
    public class BackFrame
    {
        #region Members

        private FrameBackground frameBackGround = null;

        private FrameImage frameImage = null;

        private FrameShape frameShape = FrameShape.Default;

        private FrameStyle frameStyle = FrameStyle.None;

        private float frameWidth = 8;

        private GlassEffect glassEffect = GlassEffect.None;

        private Style style = null;

        #endregion

        #region Constructor

        public BackFrame()
        {
        }

        #endregion

        #region Public Properties

        public FrameBackground FrameBackground
        {
            get { return frameBackGround; }
            set { frameBackGround = value; }
        }

        public FrameImage FrameImage
        {
            get { return frameImage; }
            set { frameImage = value; }
        }

        public FrameShape FrameShape
        {
            get { return frameShape; }
            set { frameShape = value; }
        }

        public FrameStyle FrameStyle
        {
            get { return frameStyle; }

            set { frameStyle = value; }
        }

        public float FrameWidth
        {
            get 
            { 
                return frameWidth; 
            }

            set
            {
                if (value >= 0 && value <= 50)
                {
                    frameWidth = value;
                }
                else
                {
                    throw new Exception("Gauge - BackFrame - FrameWidth Must be between 0 and 50.");
                }
            }
        }
    
        public GlassEffect GlassEffect
        {
            get { return glassEffect; }
            set { glassEffect = value; }
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
    }
}
