//-------------------------------------------------------------------------------------------------
// <copyright file="CubeInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

#if !SILVERLIGHT
namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// A CubeInfo extracts the Cube attribute informations after a successful connection has been
    /// made with the local or offline cube.
    /// </summary>
    /// <remarks>
    /// A CubeInfo is created in AdomdProvider while browsing through all the cubes available in AdomdProvider
    /// with the current connection.
    /// </remarks>
        [Serializable]
    public class CubeInfo
    {
#else
using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Data;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// A CubeInfo extracts the Cube attribute information after a successful connection has been
    /// made with the local or offline cube.
    /// </summary>
    /// <remarks>
    /// A CubeInfo is created in AdomdProvider while browsing through all the cubes available in AdomdProvider
    /// with the current connection.
    /// </remarks>
        [DataContract]
    public class CubeInfo
    {
#endif
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CubeInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the Cube</param>
        /// <param name="caption">The caption of the cube</param>
        /// <param name="description">The description of the cube</param>
        public CubeInfo(string name, string caption, string description)
        {
            Name = name;
            Caption = caption;
            Description = description;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the Cube caption.
        /// </summary>
        /// <value>Caption of the Cube</value>
        [Description("Gets or sets a cube caption."), DefaultValue("")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>contains the Cube Description</value>
        [Description("Gets or sets a cube description."), DefaultValue("")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Cube name.
        /// </summary>
        /// <value>Contains the name of the Cube</value>
        [Description("Gets or sets a cube name."), DefaultValue("")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Name { get; set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>Properties Collection</value>
#if SILVERLIGHT
            [DataMember]
        [Description("Gets cube collection."), DefaultValue((string)null)]
        public PropertyCollection Properties { get; set; }
#else
        [Description("Gets cube collection."), DefaultValue((string)null)]
        public PropertyCollection Properties { get; private set; }
#endif


            /// <summary>
        /// Gets or sets a value indicating whether this Cube is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a value indicating whether this Cube is visible."), DefaultValue(true)]
#if SILVERLIGHT
        [DataMember]
#endif
        public bool Visible { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            if (Caption.Length > 0)
            {
                return Caption;
            }

            return Name;
        }
        #endregion
    }
}
