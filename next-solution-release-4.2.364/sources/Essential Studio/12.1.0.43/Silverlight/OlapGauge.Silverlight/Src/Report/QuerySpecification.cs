//-------------------------------------------------------------------------------------------------
// <copyright file="QuerySpecification.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    /// <summary>
    /// A Class that specifies the specification for building a query
    /// TODO : On request additional feature need to be included
    /// Created for scalability
    /// </summary>
    public class QuerySpecification
    {
        #region Private Variables
        Select _Select;
        #endregion

        #region Public Methods
        public Where Filter { get; set; }

        public Select Select
        {
            get
            {
                return _Select;
            }

            set
            {
                _Select = value;
            }
        }
        #endregion
    }
}
