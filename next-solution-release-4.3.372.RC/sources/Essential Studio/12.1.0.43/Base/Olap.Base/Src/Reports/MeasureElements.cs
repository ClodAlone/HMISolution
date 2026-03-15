//-------------------------------------------------------------------------------------------------
// <copyright file="MeasureElements.cs" company="syncfusion">
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
    /// Represents the Measure Element information.
    /// </summary>
    [Serializable]
    public class MeasureElements : Element, ICloneable<MeasureElements>
#else
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class MeasureElements : Element
#endif
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureElements"/> class.
        /// </summary>
        public MeasureElements()
        {
            Elements = new MeasureElementCollection();
            ExcludedMeasures = new MeasureElementCollection();
        }

        #endregion

        #region Public Properties

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public MeasureElementCollection Elements { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the excluded measures.
        /// </summary>
        /// <value>The excluded measures.</value>
        public MeasureElementCollection ExcludedMeasures { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified measure name.
        /// </summary>
        /// <param name="measureName">Name of the measure.</param>
        public void Add(string measureName)
        {
            this.Elements.Add(new MeasureElement { Name = measureName });
        }

        /// <summary>
        /// Adds the specified measure element.
        /// </summary>
        /// <param name="measureElement">The measure element.</param>
        public void Add(MeasureElement measureElement)
        {
            if (measureElement != null)
            {
                this.Elements.Add(measureElement);
            }
            else
            {
                throw new Exception("MeasureElement is null");
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="MeasureElements"/>.</returns>
        public new MeasureElements Clone()
        {
            MeasureElements measureElements = new MeasureElements();
            measureElements.Elements = this.Elements.Clone();
            measureElements.ExcludedMeasures = this.ExcludedMeasures.Clone();
            return measureElements;
        }

        #endregion
    }
}
