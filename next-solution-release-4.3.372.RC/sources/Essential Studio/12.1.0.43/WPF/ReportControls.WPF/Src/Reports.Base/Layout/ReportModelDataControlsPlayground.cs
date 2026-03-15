//-------------------------------------------------------------------------------------------------
// <copyright file="PageModelDataControlsPlayground.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Globalization;
using Syncfusion.RDL.Data;

namespace Syncfusion.RDL.Layout
{
    #region Helper Class

    internal class ReportItemContiner
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the control.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        public IReportItemModeler ReportItem
        {
            get;
            set;
        }

        public List<ReportItemContiner> LeftOrder
        {
            get;
            set;
        }

        public List<ReportItemContiner> TopOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Left gap between two controls.
        /// </summary>
        /// <value>The L gap.</value>
        public Dictionary<string, double> LGap
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Top gap between two controls.
        /// </summary>
        /// <value>The T gap.</value>
        public Dictionary<string, double> TGap
        {
            get;
            set;
        }

        public Dictionary<string, double> BGap
        {
            get;
            set;
        }

        public List<string> PreviousTopControlNames
        {
            get;
            set;
        }

        public List<string> PreviousLeftControlNames
        {
            get;
            set;
        }

        #endregion
    }

    #endregion
}
