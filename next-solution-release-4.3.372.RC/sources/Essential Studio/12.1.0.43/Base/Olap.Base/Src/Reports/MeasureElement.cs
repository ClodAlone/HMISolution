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
using System.ComponentModel;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Data;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// A Wrapper class of Measure, Contains only required information of measure for generating MDXQuery
    /// Created for scalability
    /// </summary>
    [Serializable]
    public class MeasureElement : Element, ICloneable<MeasureElement>
#else
using Syncfusion.OlapSilverlight.Data;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Reports
{
    /// <summary>
    /// A Wrapper class of Measure, Contains only required informaiton of measure for generating MDXQuery
    /// Created for scalibility
    /// </summary>
    [DataContract]
    public class MeasureElement : Element
#endif
    {
        #region Private Variables
        /// <summary>
        /// Gets or sets the measure unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
        [DefaultValue("")]
        string _uniqueName;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureElement"/> class.
        /// </summary>
        public MeasureElement()
        {
            this.Name = string.Empty;
            this._uniqueName = string.Empty;
            this.Properties = new PropertyCollection();
        }
        #endregion

        #region Public Properties
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        /// <value>The unique name of <see cref="MeasureElement"/>.</value>
        public string UniqueName
        {
            get
            {
                if (_uniqueName != string.Empty)
                {
                    return _uniqueName;
                }

                return Utils.QuoteIdentifier(PropertyConstants.MeasrueNodeName) + "." + Utils.QuoteIdentifier(Name);
            }

            set
            {
                _uniqueName = value;
            }
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="MeasureElement"/>.</returns>
        public new MeasureElement Clone()
        {
            MeasureElement measureElement = new MeasureElement();
            measureElement.Name = this.Name;
#if !SILVERLIGHT
            measureElement.Properties = this.Properties.Clone();
#endif
            return measureElement;
        }
        #endregion

    }
}
