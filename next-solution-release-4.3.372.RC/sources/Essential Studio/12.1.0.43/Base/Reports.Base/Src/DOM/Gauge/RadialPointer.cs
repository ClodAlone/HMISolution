//-------------------------------------------------------------------------------------------------
// <copyright file="RadialPointer.cs" company="syncfusion">
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
    public class RadialPointer : GaugePointer
    {
        #region Members

        private PointerCap pointerCap = null;

        private RadialPointerType type = RadialPointerType.Needle;

        private NeedleStyleGauge needleStyle = NeedleStyleGauge.Triangular;

        #endregion

        #region Public Properties

        public PointerCap PointerCap
        {
            get { return pointerCap; }

            set { pointerCap = value; }
        }

        public RadialPointerType Type
        {
            get { return type; }

            set { type = value; }
        }

        public NeedleStyleGauge NeedleStyle
        {
            get { return needleStyle; }

            set { needleStyle = value; }
        }

        #endregion

        #region Constructors
        public RadialPointer()
        {
        }
        #endregion
    }
}