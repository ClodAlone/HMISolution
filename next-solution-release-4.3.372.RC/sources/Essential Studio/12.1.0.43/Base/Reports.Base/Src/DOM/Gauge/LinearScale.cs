//-------------------------------------------------------------------------------------------------
// <copyright file="LinearScale.cs" company="syncfusion">
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
    public class LinearScale : GaugeScale
    {
        #region Members

        private float startMargin = 0;

        private float endMargin = 0;

        private float position = 0;

        #endregion

        #region Public Properties

        public float StartMargin
        {
            get { return startMargin; }

            set { startMargin = value; }
        }

        public float EndMargin
        {
            get { return endMargin; }

            set { endMargin = value; }
        }

        public float Position
        {
            get { return position; }

            set { position = value; }
        }

        #endregion

        #region Constructors
        public LinearScale()
        {
        }
        #endregion
    }
}
