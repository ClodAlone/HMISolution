#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// A collection of <see cref="Property"/>.
    /// </summary>
    [CollectionDataContract]
    public class PropertyCollection : Collection<Property>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCollection"/> class.
        /// </summary>
        public PropertyCollection()
        {

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Property"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Property this[int index]
        {
            get { return base.Items[index] as Property; }
            set { base.Items[index] = value; }
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name of the property Collection</param>
        /// <param name="value">index of thed next item</param>
        /// <returns>index of the property in the current collection</returns>
        public void Add(string name, object value)
        {
            base.Items.Add(new Property(name, value));
        }

        /// <summary>
        /// Adds the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>the index of the curren property in the collection</returns>
        public void Add(Property property)
        {
            base.Items.Add(property);
        }

        /// <summary>
        /// Determines whether [contains] [the specified property].
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified property]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Property property)
        {
            return base.Items.Contains(property);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="propertyArray">The property array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Property[] propertyArray, int index)
        {
            base.Items.CopyTo(propertyArray, index);
        }

        /// <summary>
        /// Finds the Property by its name
        /// </summary>
        /// <param name="name">The name of the property to be found.</param>
        /// <returns>Property from the collection</returns>
        public Property FindByName(string name)
        {
            foreach (Property property in base.Items)
            {
                if (property.Name == name)
                {
                    return property;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<Property> GetEnumerator()
        {
            foreach (Property propertyObj in base.Items)
            {
                yield return propertyObj;
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>the index of the propety class in the collection</returns>
        public int IndexOf(Property property)
        {
            return base.Items.IndexOf(property);
        }

        /// <summary>
        /// Inserts the property to the specified index
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="property">The property.</param>
        public void Insert(int index, Property property)
        {
            base.Items.Insert(index, property);
        }

        /// <summary>
        /// Removes the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void Remove(Property property)
        {
            base.Items.Remove(property);
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
            base.Items.RemoveAt(index);
        }
        #endregion
    }
}
