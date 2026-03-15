//-------------------------------------------------------------------------------------------------
// <copyright file="Item.cs" company="syncfusion">
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

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Parameters of select command
    /// </summary>
    [Serializable]
    public class Item : ICloneable<Item>
#else

using System.Runtime.Serialization;


namespace Syncfusion.OlapSilverlight.Reports
{
    /// <summary>
    /// Parameters of select command
    /// </summary>
    [DataContract]
    public class Item 
#endif
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item()
        {
            this.IsFilterOrSortOn = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="elementValue">The element value.</param>
        public Item(Element elementValue)
        {
            this.ElementValue = elementValue;
            this.IsFilterOrSortOn = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="elementValue">The element value.</param>
        /// <param name="excludedElementValue">The excluded element value.</param>
        public Item(Element elementValue, Element excludedElementValue)
        {
            this.ElementValue = elementValue;
            this.ExcludedElementValue = excludedElementValue;
            this.IsFilterOrSortOn = false;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        /// <value>The axis position</value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(0)]
        public AxisPosition Axis { get; set; }

        /// <summary>
        /// Gets or sets the element value.
        /// </summary>
        /// <value>The element value of Item</value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue((string)null)]
        public Element ElementValue { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter or sort on.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is filter or sort on; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool IsFilterOrSortOn { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the excluded element value.
        /// </summary>
        /// <value>The excluded element value.</value>
        [DefaultValue((string)null)]
        public Element ExcludedElementValue { get; set; }

        /// <summary>
        /// Gets or sets the ItemName.
        /// </summary>
        /// <value>Name of the Item</value>
        /// <remarks>
        /// Its only valid for Non-OLAP data(Flat table pivot data population)
        /// </remarks>
        public string ItemName { get; set; }

        #endregion

#if !SILVERLIGHT
        #region Public Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="Item"/>.</returns>
        public Item Clone()
        {
            Item item = new Item();
            item.Axis = this.Axis;
            if (this.ExcludedElementValue != null)
            {
                item.ExcludedElementValue = this.ExcludedElementValue.Clone();
            }

            item.ElementValue = this.ElementValue.Clone();
            return item;
        }
        #endregion
#endif
    }
}
