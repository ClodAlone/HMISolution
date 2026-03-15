//-------------------------------------------------------------------------------------------------
// <copyright file="LinearPointer.cs" company="syncfusion">
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
    public class LinearPointer : GaugePointer
    {
        #region Members

        private Thermometer thermometer = null;

        private LinearPointerType type = LinearPointerType.Marker;

        #endregion

        #region Public Properties

        public Thermometer Thermometer
        {
            get { return thermometer; }

            set { thermometer = value; }
        }

        public LinearPointerType Type
        {
            get { return type; }

            set { type = value; }
        }

        #endregion

        #region Constructors
        public LinearPointer()
        {
        }
        #endregion
    }
}
