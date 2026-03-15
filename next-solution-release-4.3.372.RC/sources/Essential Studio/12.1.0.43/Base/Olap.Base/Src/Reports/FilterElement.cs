//-------------------------------------------------------------------------------------------------
// <copyright file="FilterElements.cs" company="syncfusion">
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

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the filtered element item.
    /// </summary>
    [Serializable]
    public class FilterElement : Element, ICloneable<FilterElement>
#else
using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class FilterElement : Element
#endif
    {
        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterElement"/> class.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public FilterElement(AxisPosition axis)
        {
            this.Elements = new ElementCollection();
            this.FilterValue = new ElementCollection();
            this.Axis = axis;
            this.IsFilterCondition = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterElement"/> class.
        /// </summary>
        public FilterElement()
        {
            this.Elements = new ElementCollection();
            this.FilterValue = new ElementCollection();
            this.Axis = AxisPosition.Series;
            this.IsFilterCondition = false;
        }

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
        /// Gets or sets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public ElementCollection Elements { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the filter case.
        /// </summary>
        /// <value>The filter case.</value>
        public FilterCase FilterCase { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the filter value.
        /// </summary>
        /// <value>The filter value.</value>
        public ElementCollection FilterValue { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter condition.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is filter condition; otherwise, <c>false</c>.
        /// </value>
        public bool IsFilterCondition { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public new FilterElement Clone()
        {
            FilterElement filterElements = new FilterElement(this.Axis);
            filterElements.Elements = this.Elements.Clone();
            filterElements.FilterValue = this.FilterValue.Clone();
            return filterElements;
        }
#endif
        #endregion
    }
}
