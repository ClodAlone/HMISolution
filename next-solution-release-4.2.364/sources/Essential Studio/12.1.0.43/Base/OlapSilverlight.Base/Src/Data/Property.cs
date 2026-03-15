#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents the properties.
    /// </summary>
    [DataContract]
    public class Property
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        public Property()
        {

        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name of the property</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [DataMember]
        public object Value { get; set; }
        #endregion
    }
}
