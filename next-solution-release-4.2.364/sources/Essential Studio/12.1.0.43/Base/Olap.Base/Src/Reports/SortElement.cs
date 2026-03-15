//-------------------------------------------------------------------------------------------------
// <copyright file="SortElement.cs" company="syncfusion">
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
    /// Represents the sort element information.
    /// </summary>
    [Serializable]
    public class SortElement : Element, ICloneable<SortElement>
#else

using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class SortElement : Element
#endif
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SortElement"/> class.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="isSortOn">if set to <c>true</c> [is sort on].</param>
        public SortElement(AxisPosition axis, SortOrder orderBy, bool isSortOn)
        {
            this.Axis = axis;
            this.SortOrder = orderBy;
            this.IsSortOn = isSortOn;
            this.Element = new MeasureElement();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortElement"/> class.
        /// </summary>
        public SortElement()
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
        /// Gets or sets the element.
        /// </summary>
        /// <value>The element.</value>
        public MeasureElement Element { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether this instance is sort on.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is sort on; otherwise, <c>false</c>.
        /// </value>
        public bool IsSortOn { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public SortOrder SortOrder { get; set; }
        #endregion

#if !SILVERLIGHT
        #region Public Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="SortElement"/>.</returns>
        public new SortElement Clone()
        {
            SortElement sortElement = new SortElement(this.Axis, this.SortOrder, this.IsSortOn);
            sortElement.Element = this.Element.Clone();
            return sortElement;
        }
        #endregion
#endif
    }
}
