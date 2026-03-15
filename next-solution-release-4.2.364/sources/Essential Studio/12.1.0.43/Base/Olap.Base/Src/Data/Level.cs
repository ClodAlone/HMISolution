//-------------------------------------------------------------------------------------------------
// <copyright file="Level.cs" company="syncfusion">
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
    /// Represents a level within a hierarchy.
    /// </summary>
    /// <remarks >
    /// Dimensions are complex structures in Microsoft SQL Server 2005 Analysis Services (SSAS), 
    /// containing members that are organized into levels, which in turn are organized into hierarchies. 
    /// In ADOMD.NET, levels are represented by the Level class.
    /// </remarks>
    [Serializable]
    public class Level : IDisposable
    {
#else
using System.Runtime.Serialization;
  
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents a level within a hierarchy.
    /// </summary>
    /// <remarks >
    /// Dimensions are complex structures in Microsoft SQL Server 2005 Analysis Services (SSAS), 
    /// containing members that are organized into levels, which in turn are organized into hierarchies. 
    /// In ADOMD.NET, levels are represented by the Level class.
    /// </remarks>
     //[KnownType(typeof(MemberCollection))]
    [DataContract]
    public class Level : IDisposable
    {
#endif
        #region  Private Variables

#if !SILVERLIGHT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        [Description("Gets a collection of the child members."), DefaultValue((string)null), Browsable(false)]
        private MemberCollection _Members;

        [Description("Gets property collection"), DefaultValue((string)null)]
#if !SILVERLIGHT
        [NonSerialized]
#endif
        PropertyCollection _Properties;

        /// <summary>
        /// Gets or sets the unique name
        /// </summary>
        /// <value>The name unique name.</value>
        [Description("Gets or sets the Level unique name."), DefaultValue("")]
        string _UniqueName;
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        public Level()
        {
            this.Caption = string.Empty;
            this.Description = string.Empty;
            this.Name = string.Empty;
            this.UniqueName = string.Empty;
            this.Visible = true;
            this._memberCount = -1;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        [DefaultValue(""), Description("Gets or sets the caption.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Description("Gets or sets the description."), DefaultValue("")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is member loaded on demand.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is member loaded on demand; otherwise, <c>false</c>.
        /// </value>
#if !SILVERLIGHT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#else
        [DataMember]
#endif
        [Browsable(false), DefaultValue((string)null)]
        public bool IsMemberLoadedOnDemand { get; set; }

        /// <summary>
        /// Gets the level depth.
        /// </summary>
        /// <value>The level depth.</value>
        [Description("Gets the level depth."), DefaultValue(0)]
#if SILVERLIGHT
        [DataMember]
        public int LevelDepth { get; set; }
#else
        public int LevelDepth { get; internal set; }
#endif


        /// <summary>
        /// Gets or sets the type of the level.
        /// </summary>
        /// <value>The type of the level.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public LevelTypeEnum LevelType { get; set; }

        private long _memberCount;
        /// <summary>
        /// Gets or sets the member count.
        /// </summary>
        /// <value>The member count.</value>
        [DefaultValue(0), Description("Gets or sets the level members count.")]
#if SILVERLIGHT
        [DataMember]
        public long MemberCount
        {
            get
            {
                if (this.Members != null)
                {
                    _memberCount = this.Members.Count;
                }

                return _memberCount;
            }
            set { _memberCount = value; }
        }
#else
        public long MemberCount
        {
            get
            {
                if (this.CubeSchema != null && this.CubeSchema.DataProvider != null)
                {
                    _memberCount = this.Members.Count;
                }
                return _memberCount;
            }
            set
            {
                _memberCount = value;
            }
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>The members.</value>
        public MemberCollection Members
        {
            get
            {
                if (this._Members == null)
                {

                    if (this.CubeSchema != null && this.CubeSchema.DataProvider != null)
                    {
                        this.IsMemberLoadedOnDemand = true;
                        this._Members = this.CubeSchema.DataProvider.GetLevelMembers(this);

                    }
                    else
                    {
                        this._Members = new MemberCollection();
                    }
                }

                return _Members;
            }

            private set
            {
                _Members = value;
            }
        }
#else 
        [DataMember]
        public MemberCollection Members
        {
            get { return _Members; }
            set { _Members = value; }
        }
#endif

#if SILVERLIGHT
       // [DataMember]
#endif
        /// <summary>
        /// Gets or sets the cube schema.
        /// </summary>
        /// <value>The cube schema.</value>
        public CubeSchema CubeSchema { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the Level</value>
        [DefaultValue(""), Description("Gets or sets the level name.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent hierarchy.
        /// </summary>
        /// <value>The parent hierarchy.</value>
#if !SILVERLIGHT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#else
        [IgnoreDataMember]
#endif
        [Description("Gets or sets the Parent hierarchy."), DefaultValue((string)null)]
        public Hierarchy ParentHierarchy { get; set; }

#if !SILVERLIGHT
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
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [XmlIgnore]
        [DataMember]
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



#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        /// <value>The unique name as string.</value>
        public string UniqueName
        {
            get
            {
                return _UniqueName;
            }

            set
            {
                _UniqueName = value.Replace(".[(All)]", string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Level"/> is visible.
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
            if (Members != null)
            {
                foreach (Member member in Members)
                {
                    member.Dispose();
                }

                this.Members.Clear();
                this.Members = null;
            }

            if (this.Properties != null)
            {
                this.Properties.Clear();
            }

            this.ParentHierarchy = null;
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
