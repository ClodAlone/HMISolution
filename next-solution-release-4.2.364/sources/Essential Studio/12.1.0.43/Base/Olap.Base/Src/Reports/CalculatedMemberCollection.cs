//-------------------------------------------------------------------------------------------------
// <copyright file="HierarchyElementCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents a collection of calculated member.
    /// </summary>
    [Serializable]
    public class CalculatedMemberCollection : CollectionBase, ICloneable<CalculatedMemberCollection>
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Reports
{
    [CollectionDataContract]
    public class CalculatedMemberCollection : Collection<CalculatedMember>
#endif
    {
        #region Public Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedMemberCollection"/> class.
        /// </summary>
        public CalculatedMemberCollection()
        {

        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Reports.CalculatedMember"/> at the specified index.
        /// </summary>
        /// <value><see cref="CalculatedMember"/></value>
        public CalculatedMember this[int index]
        {
            get
            {
                return (CalculatedMember)base.List[index];
            }

            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified calculated member.
        /// </summary>
        /// <param name="calculatedMember">The calculated member.</param>
        /// <returns>The position into which the new element was inserted, or -1 to indicate that
        /// the item was not inserted into the collection.</returns>
        public int Add(CalculatedMember calculatedMember)
        {
            return base.List.Add(calculatedMember);
        }

        /// <summary>
        /// Removes the specified calculated member.
        /// </summary>
        /// <param name="calculatedMember">The calculated member.</param>
        public void Remove(CalculatedMember calculatedMember)
        {
            base.List.Remove(calculatedMember);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Copy of calculated member collection.</returns>
        public CalculatedMemberCollection Clone()
        {
            CalculatedMemberCollection calculatedMemberCollection = new CalculatedMemberCollection();
            foreach (CalculatedMember calculatedMember in base.List)
            {
                calculatedMemberCollection.Add(calculatedMember.Clone());
            }

            return calculatedMemberCollection;
        }
#endif
        /// <summary>
        /// Gets the <see cref="Syncfusion.Olap.Reports.CalculatedMember"/> with the specified name.
        /// </summary>
        /// <value><see cref="CalculatedMember"/></value>
        public CalculatedMember this[string name]
        {
            get
            {
                return this.FindCalculatedMemberByName(name);
            }
        }

        /// <summary>
        /// Finds the calculated name of the member by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public CalculatedMember FindCalculatedMemberByName(string name)
        {
#if !SILVERLIGHT
            foreach (CalculatedMember calculatedMember in this.List)
#else
            foreach (CalculatedMember calculatedMember in this.Items)
#endif
            {
                if (calculatedMember.Name == name)
                {
                    return calculatedMember;
                }
            }

            return null;
        }
      
        #endregion
    }
}
