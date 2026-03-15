//-------------------------------------------------------------------------------------------------
// <copyright file="PropertyCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Manager;

namespace Syncfusion.Olap.Reports
#else
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Reports
#endif
{
    /// <summary>
    /// A Collection of properties.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
    public class MemberPropertyCollection : CollectionBase, IEnumerable<MemberProperty>, ICloneable<MemberPropertyCollection>
#else
    [CollectionDataContract]
    public class MemberPropertyCollection : Collection<MemberProperty>
#endif
    {
        #region Public Methods

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Property"/> at the specified index.
        /// </summary>
        /// <value><see cref="MemberProperty"/></value>
        public MemberProperty this[int index]
        {
            get 
            { 
                return (MemberProperty)base.List[index]; 
            }
            set 
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name of the property Collection</param>
        /// <param name="uniqueName">Name of the unique.</param>
        /// <returns>
        /// index of the property in the current collection
        /// </returns>
        public int Add(string name, string uniqueName)
        {
            return base.List.Add(new MemberProperty(name, uniqueName));
        }

        /// <summary>
        /// Adds the specified property.
        /// </summary>
        /// <param name="memberProperty">The member property.</param>
        /// <returns>
        /// the index of the current property in the collection
        /// </returns>
        public int Add(MemberProperty memberProperty)
        {
            return base.List.Add(memberProperty);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="MemberPropertyCollection"/>.</returns>
        public MemberPropertyCollection Clone()
        {
            MemberPropertyCollection propertyCollection = new MemberPropertyCollection();
            foreach (MemberProperty property in base.List)
            {
                propertyCollection.Add(property.Clone());
            }

            return propertyCollection;
        }

        /// <summary>
        /// Determines whether [contains] [the specified property].
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified property]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(MemberProperty property)
        {
            return base.List.Contains(property);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="propertyArray">The property array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(MemberProperty[] propertyArray, int index)
        {
            base.List.CopyTo(propertyArray, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<MemberProperty> GetEnumerator()
        {
            foreach (MemberProperty propertyObj in this.List)
            {
                yield return propertyObj;
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>the index of the propety class in the collection</returns>
        public int IndexOf(MemberProperty property)
        {
            return base.List.IndexOf(property);
        }

        /// <summary>
        /// Inserts the property to the specified index
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="property">The property.</param>
        public void Insert(int index, MemberProperty property)
        {
            base.List.Insert(index, property);
        }

        /// <summary>
        /// Removes the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void Remove(MemberProperty property)
        {
            base.List.Remove(property);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.CollectionBase"/> instance. This method is not overridable.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.
        /// -or-
        /// <paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.CollectionBase.Count"/>.
        /// </exception>
        public new void RemoveAt(int index)
        {
            base.List.RemoveAt(index);
        }
#else
        public void Add(string name, string uniqueName)
        {
            base.Items.Add(new MemberProperty(name, uniqueName));

        }
#endif
        /// <summary>
        /// Finds the Property by its name
        /// </summary>
        /// <param name="name">The name of the property to be found.</param>
        /// <returns>Property from the collection</returns>
        public MemberProperty FindByName(string name)
        {
#if !SILVERLIGHT
            foreach (MemberProperty property in base.List)
#else
            foreach (MemberProperty property in base.Items)
#endif
            {
                if (property.Name == name)
                {
                    return property;
                }
            }

            return null;
        }
        #endregion
    }
}
