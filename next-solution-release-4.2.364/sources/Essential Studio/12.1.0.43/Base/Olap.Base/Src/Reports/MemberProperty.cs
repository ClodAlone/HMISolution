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

#if !SILVERLIGHT
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
#else
using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Reports
#endif
{
    /// <summary>
    /// Represents a property of various objects
    /// </summary>
#if SILVERLIGHT
    [DataContract]
    public class MemberProperty
#else
    [Serializable]

    public class MemberProperty : ICloneable<MemberProperty>
#endif
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberProperty"/> class.
        /// </summary>
        public MemberProperty()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="uniqueName">The unique name.</param>
        public MemberProperty(string name, string uniqueName)
        {
            Name = name;
            UniqueName = uniqueName;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name of the property</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public string UniqueName { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="MemberProperty"/>.</returns>
        public MemberProperty Clone()
        {
            return this as MemberProperty;
        }
#endif
        #endregion
    }
}
