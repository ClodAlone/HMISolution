//-------------------------------------------------------------------------------------------------
// <copyright file="KPI.cs" company="syncfusion">
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
    /// Represents a KPI within a cube. 
    /// </summary>
    [Serializable]
    public class Kpi : IDisposable
    {
#else
using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents a KPI within a cube. 
    /// </summary>
    [DataContract]
    public class Kpi : IDisposable
    {
#endif
        #region internal Variables
#if !SILVERLIGHT
        [NonSerialized]
#endif
        PropertyCollection _Properties;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Kpi"/> class.
        /// </summary>
        public Kpi()
        {
            this.Caption = string.Empty;
            this.Description = string.Empty;
            this.DisplayFolder = string.Empty;
            this.UniqueName = string.Empty;
            this.Name = string.Empty;
            this.Properties = new PropertyCollection();
            this.StatusGraphic = string.Empty;
            this.TrendGraphic = string.Empty;
#if !SILVERLIGHT
            this.ParentKpi = null;
#endif
        }
        #endregion

        #region Public Properties
        //// Properties
#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display folder.
        /// </summary>
        /// <value>The display folder as string.</value>
        public string DisplayFolder { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name as string.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the status graphic.
        /// </summary>
        /// <value>The status graphic as string.</value>
        public string StatusGraphic { get; set; }

        /// <summary>
        /// Gets or sets the trend graphic.
        /// </summary>
        /// <value>The trend graphic as string.</value>
        public string TrendGraphic { get; set; }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        /// <value>The unique name as string.</value>
        public string UniqueName { get; set; }

        [NonSerialized]
        Microsoft.AnalysisServices.AdomdClient.Kpi _ParentKpi;
        /// <summary>
        /// Gets or sets the parent KPI.
        /// </summary>
        /// <value>The parent KPI.</value>
        [XmlIgnore]
        public Microsoft.AnalysisServices.AdomdClient.Kpi ParentKpi
        {
            get
            {
                return _ParentKpi;
            }

             set
            {
                _ParentKpi = value;
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
        /// Gets the properties.
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
#else
        [DataMember]
        public string Caption { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string DisplayFolder { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string StatusGraphic { get; set; }
        [DataMember]
        public string TrendGraphic { get; set; }
        [DataMember]
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the parent cube schema.
        /// </summary>
        /// <value>parent cube schema.</value>
        [DataMember]
        [Browsable(false), Description("Gets or sets the parent cube schema."), DefaultValue((string)null)]
        public CubeSchema ParentCubeSchema { get; set; }


        /// <summary>
        /// Gets the proerties.
        /// </summary>
        /// <value>The properties.</value>
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
#endif

        #endregion

        #region Public Methods
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

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion
    }
}

