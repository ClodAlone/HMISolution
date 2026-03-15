//-------------------------------------------------------------------------------------------------
// <copyright file="Measure.cs" company="syncfusion">
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
    /// Represents a measure within a cube or measure group. 
    /// </summary>
    /// <remarks>
    /// A measure represents the data within a fact table that is being organized and aggregated by the cube.
    /// In ADOMD.NET, the Measure represents the metadata for a measure within a cube. A measure is 
    /// not explicitly identified in a cellset, because a measure is treated as a member for querying purposes 
    /// and is represented by a Measure within the Measures collection of a CubeDef.
    /// </remarks>
    [Serializable]
    public class Measure : IDisposable
    {
#else
using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents a measure within a cube or measure group. 
    /// </summary>
    /// <remarks>
    /// A measure represents the data within a fact table that is being organized and aggregated by the cube.
    /// In ADOMD.NET, the Measure represents the metadata for a measure within a cube. A measure is 
    /// not explicitly identified in a cellset, because a measure is treated as a member for querying purposes 
    /// and is represented by a Measure within the Measures collection of a CubeDef.
    /// </remarks>
    [DataContract]
    public class Measure : IDisposable
    {

#endif
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Measure"/> class.
        /// </summary>
        public Measure()
        {
            this.Caption = string.Empty;
            this.Description = string.Empty;
            this.GroupName = string.Empty;
            this.DisplayFolder = string.Empty;
            this.Name = string.Empty;
            this.ParentCubeName = string.Empty;
            this.UniqueName = string.Empty;
            this.Units = string.Empty;
            this.Visible = true;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>Caption of type string</value>
        [DefaultValue(""), Description("Gets or sets a Measure caption.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>Description of type string</value>
        [DefaultValue(""), Description("Gets or sets the Measure description.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Group name
        /// </summary>
        /// <value>Group name of type string</value>
        [Description("Gets or sets the Measure group name."), DefaultValue("")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or Sets the Display Folder name
        /// </summary>
        /// <value>Display Folder name of type string</value>
        [Description("Gets or Sets the Display Folder name")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string DisplayFolder { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>Measure name of type string</value>
        [DefaultValue(""), Description("Gets or sets the Measure name.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the numeric precision.
        /// </summary>
        /// <value>numeric precision.</value>
        [Description("Gets or sets numeric precision."), DefaultValue(0)]
#if SILVERLIGHT
        [DataMember]
#endif
        public int NumericPrecision { get; set; }

        /// <summary>
        /// Gets or sets the numeric scale.
        /// </summary>
        /// <value>numeric scale.</value>
        [DefaultValue(0), Description("Gets or sets numeric scale.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public int NumericScale { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent cube.
        /// </summary>
        /// <value>The name of the parent cube.</value>
        [Description("Gets or sets the parent cube name."), DefaultValue("")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string ParentCubeName { get; set; }

        /// <summary>
        /// Gets or sets the parent cube schema.
        /// </summary>
        /// <value>The parent cube schema.</value>
        [Description("Gets or sets the parent cubeschema."), DefaultValue((string)null)]
#if SILVERLIGHT
        [DataMember]
#endif
        public CubeSchema ParentCubeSchema { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the measure aggregator.
        /// </summary>
        /// <value>The measure aggregator.</value>
        public int MeasureAggregator { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
#if !SILVERLIGHT
        [NonSerialized]
#endif
        PropertyCollection _Properties;

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [Description("Gets property collection."), DefaultValue((string)null)]
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
#else
        [DataMember]
        [Description("Gets property collection."), DefaultValue((string)null)]
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

#endif
        /// <summary>
        /// Gets or sets the Measure unique name
        /// </summary>
        /// <value>Measure unique name of type string.</value>
        [DefaultValue(""), Description("Gets or sets the unique name.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>The units.</value>
        [DefaultValue(""), Description("Gets or sets the units.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Units { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Measure"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
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
            if (this.Properties != null)
            {
                this.Properties.Clear();
            }

            this.ParentCubeSchema = null;
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
                return this.Caption;
            }

            return Name;
        }
        #endregion
    }
}
