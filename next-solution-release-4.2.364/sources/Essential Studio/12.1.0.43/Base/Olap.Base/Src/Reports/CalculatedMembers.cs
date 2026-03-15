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
    /// Represents the Calculated members.
    /// </summary>
    [Serializable]
    public class CalculatedMembers : Element, ICloneable<CalculatedMembers>
#else
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class CalculatedMembers : Element
#endif
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedMembers"/> class.
        /// </summary>
        public CalculatedMembers()
        {
            Elements = new CalculatedMemberCollection();
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
        public CalculatedMemberCollection Elements { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the specified calculated member.
        /// </summary>
        /// <param name="calculatedMember">The calculated member.</param>
        public void Add(CalculatedMember calculatedMember)
        {
#if !SILVERLIGHT
            this.Elements.Add(calculatedMember.Clone());
#else
            this.Elements.Add(calculatedMember);
#endif
        }

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="CalculatedMembers"/>.</returns>
        public new CalculatedMembers Clone()
        {
            CalculatedMembers calculatedMembers = new CalculatedMembers();
            calculatedMembers.Elements = this.Elements.Clone();
            return calculatedMembers;
        }
#endif
        #endregion
    }
}
