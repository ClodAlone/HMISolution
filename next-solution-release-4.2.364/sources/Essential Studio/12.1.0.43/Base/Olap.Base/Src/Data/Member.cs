//-------------------------------------------------------------------------------------------------
// <copyright file="Member.cs" company="syncfusion">
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
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Represents a single member within a hierarchy, tuple, level, or member. 
    /// </summary>
    /// <remarks>
    /// A member is an item in a hierarchy that represents one or more records in the underlying 
    /// relational database. A member is the lowest level of reference used when accessing 
    /// cell data in a cube.
    /// <para>
    /// Members are used to construct tuples, which in turn are used to construct sets. Members are 
    /// organized hierarchically; a member can have other members associated with it. For example, in 
    /// a time dimension that contains three levels named Year, Month, and Day, the members of the 
    /// Day level are leaf members because they have no child members. The members in the Year and 
    /// Month levels are nonleaf members, because each member in the Month level has at least 28 child 
    /// members from the Day level and each member in the Year level has 12 child members from the 
    /// Month level.
    /// </para>
    /// <para>
    /// The Member encapsulates the information necessary to describe a member, including a collection of 
    /// Member objects that contain child members, if applicable.
    /// </para>
    /// <para>
    /// The information available to a Member depends on the parent of the Members collection from which the 
    /// Member was retrieved. While the Members collection externally represents a collection of Member 
    /// objects for a specified Hierarchy, Tuple, Level, or Member, the collection is internally loaded and 
    /// managed in one of two ways, depending on the parent of the Hierarchy, Tuple, Level, or Member:
    /// </para>
    /// <para>
    /// * If the parent object was referenced through a CubeSchema in order to retrieve metadata from the server, 
    /// the collection represents the members that are defined for the parent object.
    /// </para>
    /// <para>
    /// * If the parent object was referenced through a CellSet in order to retrieve metadata from a query, the 
    /// collection represents the members that are retrieved for the set (or axis) that contains the parent object.
    /// </para>
    /// </remarks>
    [Serializable]
    public class Member : IDisposable
    {
        #region Private and Internal Variables
        internal MemberCollection _ChildMembers;
        bool _IsParentMemberLoaded;
        #endregion

        #region Private Properties
        /// <summary>
        /// Gets or sets the name of the level unique.
        /// </summary>
        /// <value>The name of the level unique.</value>
        [DefaultValue(""), Description("Gets or sets the member level's unique name.")]
        string _LevelUniqueName;

        [Browsable(false), DefaultValue((string)null), Description("Gets or sets the parent member"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal Member _ParentMember;

        [NonSerialized]
        PropertyCollection _Properties;

        PropertyCollection _MemberProperties;

        /// <summary>
        /// Gets or sets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        [Description("Gets or sets the unique name."), DefaultValue("")]
        string _UniqueName;

        /// <summary>
        /// Gets or sets the caption of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        [Description("Gets or sets the caption."), DefaultValue("")]
        private string _Caption;

        /// <summary>
        /// Gets or sets the caption of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        [Description("Gets or sets the caption."), DefaultValue("")]
        private string _parentUniqueName;

        /// <summary>
        /// Gets or sets the caption of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        [Description("Gets or sets the caption."), DefaultValue("")]
        private string _parentCaption;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Member"/> class.
        /// </summary>
        public Member()
        {
            this._ChildMembers = new MemberCollection();
            this.Caption = string.Empty;
            this.Description = string.Empty;
            this.LevelUniqueName = string.Empty;
            this.Name = string.Empty;
            this.UniqueName = string.Empty;
            this.ParentCaption = string.Empty;
            this.ParentUniqueName = string.Empty;
            this.LevelDepth = -1;
            this.Visible = true;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        [DefaultValue((string)""), Description("Gets or sets the member caption.")]
        public string Caption
        {
            get
            {
                return _Caption;
            }

            set
            {
                _Caption = value;
            }
        }

        /// <summary>
        /// Gets the child members.
        /// </summary>
        /// <value>The child members.</value>
        public MemberCollection ChildMembers
        {
            get
            {
                if (this._ChildMembers == null || this._ChildMembers.Count == 0)
                {
                    if (this.CubeSchema != null)
                    {
                        try
                        {
                            (CubeSchema.DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider).CurrentCubeName = CubeSchema.CubeInfo.Name;
                        }
                        catch
                        {}
                        this._ChildMembers = CubeSchema.DataProvider.GetChildMembers(this, false);
                    }
                    else
                    {
                        this._ChildMembers = new MemberCollection();
                    }
                }

                return this._ChildMembers;
            }
        }

        /// <summary>
        /// Gets or sets the cube schema.
        /// </summary>
        /// <value>The cube schema.</value>
        public CubeSchema CubeSchema { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Description("Gets or sets the member description."), DefaultValue("")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [drilled down].
        /// </summary>
        /// <value><c>true</c> if [drilled down]; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Description("Gets or sets a weather a member is drilled down "), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DrilledDown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has child members.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has child members; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets members has child members."), DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasChildMembers { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a value indicating whether this instance is member loaded on demand.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is member loaded on demand; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue((string)null), Description("Gets or sets a flag that indicates that child member collection is pre-populated with members.")]
        public bool IsMemberLoadedOnDemand
        {
            get
            {
                return this._ChildMembers != null;
            }
        }

        /// <summary>
        /// Gets or sets the KPI status graphic.
        /// </summary>
        /// <value>The KPI status graphic.</value>
        [DefaultValue("")]
        public string KPIStatusGraphic { get; set; }

        /// <summary>
        /// Gets or sets the KPI trend graphic.
        /// </summary>
        /// <value>The KPI trend graphic.</value>
        [DefaultValue("")]
        public string KPITrendGraphic { get; set; }

        /// <summary>
        /// Gets or sets the type of the KPI.
        /// </summary>
        /// <value>The type of the KPI.</value>
        [DefaultValue(KpiTypeEnum.Kpi_None)]
        public KpiTypeEnum KPIType { get; set; }

        /// <summary>
        /// Gets or sets the level depth.
        /// </summary>
        /// <value>The level depth.</value>
        [Description("Gets or sets the member leveldepth"), DefaultValue(true)]
        public int LevelDepth { get; set; }
        public string LevelUniqueName
        {
            get
            {
                return
                    _LevelUniqueName;
            }

            set
            {
                _LevelUniqueName = value.Replace(".[(All)]", string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the Member object</value>
        [Description("Gets or sets the member name."), DefaultValue("")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique name of the parent object.
        /// </summary>
        /// <value>The parent unique name as string.</value>
        public string ParentUniqueName
        {
            get
            {
                return _parentUniqueName;
            }

            set
            {
                _parentUniqueName = value;
            }
        }

        /// <summary>
        /// Gets or sets the caption of the parent object.
        /// </summary>
        /// <value>The parent caption as string.</value>
        public string ParentCaption
        {
            get
            {
                return _parentCaption;
            }

            set
            {
                _parentCaption = value;
            }
        }

        /// <summary>
        /// Gets the custom unique name of the parent object.
        /// </summary>
        /// <value>The name of the parent custom unique.</value>
        public string ParentCustomUniqueName
        {
            get
            {
                if (_parentUniqueName != string.Empty && this._parentCaption != string.Empty && _parentUniqueName !=null && this._parentCaption !=null )
                {
                    string[] tempString = _parentUniqueName.Split('.');
                    if (tempString.Length > 2)
                        return tempString[0] + "." + tempString[1] + "." + tempString[2] + "." + Utils.QuoteIdentifier(this._parentCaption);
                    if(tempString.Length>1)
                        return tempString[0] + "." + tempString[1] + "." + Utils.QuoteIdentifier(this._parentCaption);
                    return tempString[0] + "." + Utils.QuoteIdentifier(this._parentCaption);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent level.
        /// </summary>
        /// <value>The parent level.</value>
        [Description("Gets or sets Parent level."), DefaultValue((string)null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Level ParentLevel { get; set; }

        /// <summary>
        /// Gets or sets the parent member.
        /// </summary>
        /// <value>The parent member.</value>
        public Member ParentMember
        {
            get
            {
                if (this._ParentMember == null && !this._IsParentMemberLoaded)
                {
                    Dimension dimension = this.GetDimension();
                    if (dimension != null && dimension.ParentCubeSchema != null && dimension.ParentCubeSchema.DataProvider != null)
                    {
                        this._IsParentMemberLoaded = true;
                        this._ParentMember = dimension.ParentCubeSchema.DataProvider.GetParentMember(this);
                        if (this._ParentMember != null)
                        {
                            Member member = dimension.ParentCubeSchema.GetMemberByUniqueName(this._ParentMember.UniqueName, true);
                            if (member != null)
                            {
                                this._ParentMember = member;
                            }
                        }
                    }
                }

                return _ParentMember;
            }

            set
            {
                _ParentMember = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent hierarchy.
        /// </summary>
        /// <value>The parent hierarchy.</value>
        public string ParentHierarchy { get; set; }
        /// <summary>
        /// Gets or sets the parent dimension.
        /// </summary>
        /// <value>The parent dimension.</value>
        public string ParentDimension { get; set; }
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [DefaultValue((string)null), Description("Gets property collection")]
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
        /// Gets or sets the member properties.
        /// </summary>
        /// <value>The member properties.</value>
        public PropertyCollection MemberProperties 
        {
            get
            {
                if (_MemberProperties == null)
                {
                    this._MemberProperties = new PropertyCollection();
                }

                return _MemberProperties;
            }
            set
            {
                _MemberProperties = value;
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DefaultValue(0)]
        public MemberTypeEnum Type { get; set; }
        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        /// <value>The unique name as string.</value>
        public string UniqueName
        {
            get
            {
                return
                    _UniqueName;
            }

            set
            {
                _UniqueName = value.Replace(".[(All)]", string.Empty);
            }
        }

        /// <summary>
        /// Gets the custom unique name.
        /// </summary>
        /// <value>The custom unique name as string.</value>
        public string CustomUniqueName
        {
            get
            {
                if (_UniqueName != string.Empty)
                {
                    string[] tempString = _UniqueName.Split('.');
                    if (tempString.Length < 3)
                        return _UniqueName;
                    else
                        return tempString[0] + "." + tempString[1] + "." + (tempString[2].StartsWith("&") ? tempString[1] : tempString[2]) + "." + Utils.QuoteIdentifier(this._Caption);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Member"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Visible { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Properties != null)
            {
                this.Properties.Clear();
            }
        }

        /// <summary>
        /// Gets the dimension.
        /// </summary>
        /// <param name="cubeSchema">The cube schema.</param>
        /// <returns>returns the dimension object if matches else returns null</returns>
        public Dimension GetDimension(CubeSchema cubeSchema)
        {
            if (LevelUniqueName.Length > 0 || this.ParentLevel != null)
            {
                if (ParentLevel == null && cubeSchema != null)
                {
                    this.ParentLevel = cubeSchema.GetLevelByUniqueName(LevelUniqueName);
                }

                if (this.ParentLevel != null)
                {
                    if (this.ParentLevel.ParentHierarchy != null)
                    {
                        if (this.ParentLevel.ParentHierarchy.ParentDimension != null)
                        {
                            return this.ParentLevel.ParentHierarchy.ParentDimension;
                        }
                    }
                }
            }

            if (this.ParentLevel != null)
            {
                this.GetDimension();
            }

            return null;
        }

        /// <summary>
        /// Gets the dimension.
        /// </summary>
        /// <returns>returns the Dimension object</returns>
        public Dimension GetDimension()
        {
            return GetDimension(this.CubeSchema);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            if (this.Caption.Length > 0)
            {
                return this.Caption;
            }

            return this.Name;
        }
        #endregion
    }
}
