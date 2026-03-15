//-------------------------------------------------------------------------------------------------
// <copyright file="NamedSet.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#if !SILVERLIGHT
namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Represents a Named Set within a cube.
    /// </summary>
    [Serializable]
    public class NamedSet : IDisposable
    {
        [NonSerialized]
#else
using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents a Named Set within a cube. 
    /// </summary>
    [DataContract]
    public class NamedSet : IDisposable
    {
        private string _uniqueName;
#endif
        private PropertyCollection _Properties;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedSet"/> class.
        /// </summary>
        public NamedSet()
        {
            this.Properties = new PropertyCollection();
            this.Name = string.Empty;
            this.Expression = string.Empty;
            this.ParentHierarchyName = string.Empty;
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Properties != null)
            {
                Properties.Clear();
            }

            Properties = null;
        }

        #endregion

        #region Public Properties
#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [XmlIgnore]
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

            private set
            {
                _Properties = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent cube schema.
        /// </summary>
        /// <value>parent cube schema.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), Description("Gets or sets the parent cube schema."), DefaultValue((string)null)]
        public CubeSchema ParentCubeSchema { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        public string UniqueName 
        {
            get
            {
                return Syncfusion.Olap.Common.Utils.QuoteIdentifier(this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the parent hierarchy name.
        /// </summary>
        /// <value>The name of the parent hierarchy.</value>
        public string ParentHierarchyName { get; set; }
        /// <summary>
        /// Gets or sets the parent dimension name.
        /// </summary>
        /// <value>The name of the parent dimension.</value>
        public string ParentDimensionName { get; set; }

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        [XmlIgnore]
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlIgnore]
        public string Description { get; set; }

        /// <summary>
        /// Gets or Sets the DisplayFolder
        /// </summary>
        [XmlIgnore]
        public string DisplayFolder { get; set; }
#else
        [DataMember]
        [XmlIgnore]
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
            set
            {
                _Properties = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent cube schema.
        /// </summary>
        /// <value>parent cube schema.</value>
        [Browsable(false), Description("Gets or sets the parent cube schema."), DefaultValue((string)null)]
        [DataMember]
        public CubeSchema ParentCubeSchema { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string UniqueName
        {
            get
            {
                return Syncfusion.OlapSilverlight.Common.Utils.QuoteIdentifier(this.Name);
            }
            set
            {
                _uniqueName = value;
            }
        }

        [DataMember]
        public string ParentHierarchyName { get; set; }

        [DataMember]
        public string ParentDimensionName { get; set; }

        [DataMember]
        [XmlIgnore]
        public string Expression { get; set; }

        [DataMember]
        [XmlIgnore]
        public string Description { get; set; }

         /// <summary>
        /// Gets or Sets the DisplayFolder
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public string DisplayFolder { get; set; }
#endif

        #endregion
    }
}
