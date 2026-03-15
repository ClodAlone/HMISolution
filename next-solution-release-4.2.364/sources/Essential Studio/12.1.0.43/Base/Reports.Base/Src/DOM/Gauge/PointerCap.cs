//-------------------------------------------------------------------------------------------------
// <copyright file="PointerCap.cs" company="syncfusion">
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
    public class PointerCap
    {
        #region Members

        private Style style = null;

        private CapImage capImage = null;

        private CapStyle capStyle = CapStyle.RoundedDark;

        private bool onTop;

        private bool reflection;

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

        public CapImage CapImage
        {
            get { return capImage; }

            set { capImage = value; }
        }

        public CapStyle CapStyle
        {
            get { return capStyle; }

            set { capStyle = value; }
        }

        public bool OnTop
        {
            get { return onTop; }

            set { onTop = value; }
        }

        public bool Reflection
        {
            get { return reflection; }

            set { reflection = value; }
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
        public PointerCap()
        {
        }
        #endregion
    }
}
