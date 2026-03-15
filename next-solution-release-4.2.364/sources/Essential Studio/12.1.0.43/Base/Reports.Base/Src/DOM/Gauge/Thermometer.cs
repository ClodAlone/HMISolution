//-------------------------------------------------------------------------------------------------
// <copyright file="Thermometer.cs" company="syncfusion">
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
    public class Thermometer
    {
        #region Members

        private Style style = null;

        private ThermometerStyle thermometerStyle = ThermometerStyle.Standard;

        private float bulbOffset = 5;

        private float bulbSize = 50;

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

        public ThermometerStyle ThermometerStyle
        {
            get { return thermometerStyle; }

            set { thermometerStyle = value; }
        }

        public float BulbOffset
        {
            get { return bulbOffset; }

            set { bulbOffset = value; }
        }

        public float BulbSize
        {
            get { return bulbSize; }

            set { bulbSize = value; }
        }

        #endregion

        #region Constructors
        public Thermometer()
        {
        }
        #endregion
    }
}
