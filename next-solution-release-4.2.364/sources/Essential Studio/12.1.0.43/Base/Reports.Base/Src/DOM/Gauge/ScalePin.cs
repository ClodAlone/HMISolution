//-------------------------------------------------------------------------------------------------
// <copyright file="ScalePin.cs" company="syncfusion">
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
    public class ScalePin
    {
        #region Members

        private float location;

        private bool enable;

        private PinLabel pinLabel = null;

        #endregion

        #region Public Properties

        public float Location
        {
            get { return location; }

            set { location = value; }
        }

        public bool Enable
        {
            get { return enable; }

            set { enable = value; }
        }

        public PinLabel PinLabel
        {
            get { return pinLabel; }

            set { pinLabel = value; }
        }

        #endregion

        #region Constructors
        public ScalePin()
        {
        }
        #endregion
    }
}
