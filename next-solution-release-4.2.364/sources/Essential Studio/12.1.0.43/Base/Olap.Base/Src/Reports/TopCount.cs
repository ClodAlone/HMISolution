//-------------------------------------------------------------------------------------------------
// <copyright file="TopCount.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Data;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the top-count information.
    /// </summary>
    [Serializable]
    public class TopCountElement : Element, ICloneable<TopCountElement>

#else

using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class TopCountElement : Element
#endif
    {
        #region Private Variables
        private string _measureName;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TopCountElement"/> class.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="fieldCount">The field count.</param>
        public TopCountElement(AxisPosition axis, int fieldCount)
        {
            this.Axis = axis;
            this.FieldCount = fieldCount;
            this.MeasureName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopCountElement"/> class.
        /// </summary>
        public TopCountElement()
        {
        }
        #endregion

        #region Public Properties
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        /// <value>The axis.</value>
        public AxisPosition Axis { get; set; }
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the field count.
        /// </summary>
        /// <value>The field count.</value>
        public int FieldCount { get; set; }
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the name of the measure.
        /// </summary>
        /// <value>The name of the measure.</value>
        public string MeasureName 
        {
            get
            {
                if (_measureName != string.Empty)
                {
                    if (_measureName.StartsWith(Utils.QuoteIdentifier(PropertyConstants.MeasrueNodeName) + "."))
                        return _measureName;

                    return Utils.QuoteIdentifier(PropertyConstants.MeasrueNodeName) + "." + Utils.QuoteIdentifier(_measureName);
                }
                return string.Empty;
            }

            set
            {
                _measureName = value;
            }
        }

        #endregion

#if !SILVERLIGHT
        #region Public Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="TopCountElement"/>.</returns>
        public new TopCountElement Clone()
        {
            TopCountElement topCount = new TopCountElement(this.Axis, this.FieldCount);
            topCount.MeasureName = this.MeasureName;
            return topCount;
        }
        #endregion
#endif
    }
}
