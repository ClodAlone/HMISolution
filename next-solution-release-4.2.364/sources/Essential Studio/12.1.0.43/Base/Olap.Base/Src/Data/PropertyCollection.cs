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
using Syncfusion.Olap.Common;
using System.Collections.ObjectModel;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// A Collection of properties
    /// </summary>
    [Serializable]
    public class PropertyCollection : List<Property>, ICloneable<PropertyCollection>
    {
        #region Public Methods
        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name of the property Collection</param>
        /// <param name="value">index of thed next item</param>
        /// <returns>index of the property in the current collection</returns>
        public int Add(string name, object value)
        {
            this.Add(new Property(name, value));
            return this.Count - 1;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="PropertyCollection"/>.</returns>
        public PropertyCollection Clone()
        {
            PropertyCollection propertyCollection = new PropertyCollection();
            foreach (Property property in this)
            {
                propertyCollection.Add(property.Clone());
            }

            return propertyCollection;
        }

        /// <summary>
        /// Finds the Property by its name
        /// </summary>
        /// <param name="name">The name of the property to be found.</param>
        /// <returns>Property from the collection</returns>
        public Property FindByName(string name)
        {
            foreach (Property property in this)
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
