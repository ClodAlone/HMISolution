//-------------------------------------------------------------------------------------------------
// <copyright file="MeasureElement.cs" company="syncfusion">
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
using Syncfusion.OlapSilverlight.Base.Data;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    /// <summary>
    /// A Wrapper class of Measure, Contains only required informaiton of measure for generating MDXQuery
    /// Created for scalibility
    /// </summary>
    [DataContract]
    public class MeasureElement : Element
    {
        #region Private Variables
        /// <summary>
        /// Gets or sets the measure unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
        [DefaultValue("")]
        string _UniqueName;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureElement"/> class.
        /// </summary>
        public MeasureElement()
        {
            this.Name = string.Empty;
            this._UniqueName = string.Empty;
        }
        #endregion

        #region Public Properties
        [DataMember]
        public string UniqueName
        {
            get
            {
                if (_UniqueName != string.Empty)
                {
                    return _UniqueName;
                }

                return Utils.QuoteIdentifier(PropertyConstants.Measures) + "." + Utils.QuoteIdentifier(Name);
            }

            set
            {
                _UniqueName = value;
            }
        }
        #endregion

    }
}
