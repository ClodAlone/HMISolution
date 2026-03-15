//-------------------------------------------------------------------------------------------------
// <copyright file="Dimension.cs" company="syncfusion">
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
using System.Xml.Serialization;

#if !SILVERLIGHT
namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Represents a dimension within a cube. 
    /// </summary>
    /// <remarks>
    /// A dimension is created in UpdateDimension method of AdomdProvider, while
    /// browsing the current cube in the current connection.
    /// </remarks>
    [Serializable]
    public class Dimension : IDisposable
    {
#else
using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents a dimension within a cube. 
    /// </summary>
    /// <remarks>
    /// A dimension is created in UpdateDimension method of AdomdProvider, while
    /// browsing the current cube in the current connection.
    /// </remarks>
    [KnownType(typeof(HierarchyCollection))]
    [KnownType(typeof(PropertyCollection))]
    [DataContract]
    public class Dimension : IDisposable
    {
#endif
        #region Private Variables
        /// <summary>
        /// Gets or sets the parent cube schema.
        /// </summary>
        /// <value>parent cube schema.</value>
#if !SILVERLIGHT
        [NonSerialized]
#endif
        private CubeSchema _ParentCubeSchema;


#if !SILVERLIGHT
        [NonSerialized]
#endif
        PropertyCollection _Properties;

        /// <summary>
        /// Gets or sets the name of the unique.
        /// </summary>
        /// <value>Dimension unique name.</value>
        string _uniqueName;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Dimension"/> class.
        /// </summary>
        public Dimension()
        {
            this.Caption = string.Empty;
            this.DefaultHierarchyName = string.Empty;
            this.Description = string.Empty;
            this.Name = string.Empty;
            this.UniqueName = string.Empty;
            this.Visible = true;
            this.Hierarchies = new HierarchyCollection(this);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>Dimension caption of type string</value>
        [Description("Gets or sets the caption."), DefaultValue("")]
#if SILVERLIGHT
       [DataMember]
#endif
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the default name of the hierarchy.
        /// </summary>
        /// <value>The default name of the hierarchy type of string.</value>
        [Description("Gets or sets the default name of the hierarchy."), DefaultValue("")]
#if SILVERLIGHT
       [DataMember]
#endif
        public string DefaultHierarchyName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>Dimension description of type string.</value>
        [Description("Gets or sets the description."), DefaultValue("")]
#if SILVERLIGHT
       [DataMember]
#endif
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the dimension.
        /// </summary>
        /// <value>The type of the dimension.</value>
        [DefaultValue(0), Description("Gets the dimension Type.")]
#if SILVERLIGHT
       [DataMember]
#endif
        public DimensionTypeEnum DimensionType { get; set; }

        /// <summary>
        /// Gets or sets the hierarchies.
        /// </summary>
        /// <value>A collection Hierarchy objects.</value>
        [DefaultValue((string)null), Description("Gets or sets the hierarchies.")]
#if SILVERLIGHT
       [DataMember]
#endif
        public HierarchyCollection Hierarchies { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>Dimension name of type string</value>
        [Description("Gets or sets the name."), DefaultValue("")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Name { get; set; }

#if !SILVERLIGHT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#else
        [IgnoreDataMember]
#endif
        /// <summary>
        /// Gets or sets the parent cube schema.
        /// </summary>
        /// <value>The parent cube schema.</value>
        [Browsable(false), XmlIgnore, Description("Gets or sets the parent cube schema."), DefaultValue((string)null)]
        public CubeSchema ParentCubeSchema
        {
            get
            {
                return _ParentCubeSchema;
            }

            set
            {
                _ParentCubeSchema = value;
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>Gets the properties.</value>
        [DefaultValue((string)null), Description("Gets the dimension collection properties.")]
        [XmlIgnore]
#if SILVERLIGHT
       [DataMember]
#endif
        public PropertyCollection Properties
        {
            get
            {
                if (_Properties == null)
                {
                    this._Properties = new PropertyCollection();
                }

                return _Properties;
            }
#if !SILVERLIGHT
            private set
            {
                _Properties = value;
            }
#else
            set
            {
                _Properties = value;
            }
#endif
        }

        /// <summary>
        /// Gets or sets the unique name of the dimension.
        /// </summary>
        /// <value>The Unique name of the Dimension</value>
        [DefaultValue(""), Description("Gets or sets unique name.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string UniqueName
        {
            get
            {
                return _uniqueName;
            }

            set
            {
                _uniqueName = value.Replace(".[(All)]", string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Dimension"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a value indicating whether this is visible."), DefaultValue(true)]
#if SILVERLIGHT
        [DataMember]
#endif
        public bool Visible { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (Hierarchy hierarchy in Hierarchies)
            {
                hierarchy.Dispose();
            }

            if (Properties != null)
            {
                Properties.Clear();
            }

            Properties = null;
        }

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
