//-------------------------------------------------------------------------------------------------
// <copyright file="Property.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Represents a property of various objects
    /// </summary>
    [Serializable]
    public class Property : ICloneable<Property>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value.</param>
        public Property(string name, object value)
        {
            Name = name;
            Value = value;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name of the property</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Property Clone()
        {
            return this as Property;
        }
        #endregion
    }
}
