#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Data
{
    [KnownType(typeof(MemberCollection))]
    [DataContract]
    public class Member
    {
        #region Private and Internal Variables
        internal MemberCollection _ChildMembers;
        private PropertyCollection _MemberProperties;
        PropertyCollection _Properties;
        string _customUniqueName = string.Empty;
        string _customParentUName = string.Empty;
        bool _IsParentMemberLoaded;
        #endregion

        #region Private Properties
        /// <summary>
        /// Gets or sets the name of the level unique.
        /// </summary>
        /// <value>The name of the level unique.</value>
        [DefaultValue(""), Description("Gets or sets the member level's unique name.")]
        [DataMember]
        public string _LevelUniqueName;

        //[Browsable(false), DefaultValue((string)null), Description("Gets or sets the parent member"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DataMember]
        public Member _ParentMember;

        /// <summary>
        /// Gets or sets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        [Description("Gets or sets the unique name."), DefaultValue("")]
        [DataMember]
        public string _UniqueName;

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
            this.LevelDepth = -1;
            this.Visible = true;
            this.Kpi_Name = string.Empty;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        [DefaultValue((string)""), Description("Gets or sets the member caption.")]
        [DataMember]
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
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Description("Gets or sets the member description."), DefaultValue("")]
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [drilled down].
        /// </summary>
        /// <value><c>true</c> if [drilled down]; otherwise, <c>false</c>.</value>
        //[DefaultValue(false), Description("Gets or sets a weather a member is drilled down "), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DataMember]
        public bool DrilledDown { get; set; }

      
        /// <summary>
        /// Gets or sets a value indicating whether this instance has child members.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has child members; otherwise, <c>false</c>.
        /// </value>
        //[Description("Gets or sets members has child members."), DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DataMember]
        public bool HasChildMembers { get; set; }

        [Browsable(false), DefaultValue((string)null), Description("Gets or sets a flag that indicates that child member collection is pre-populated with members.")]
        [DataMember]
        public bool IsMemberLoadedOnDemand
        {
            get
            {
                return this._ChildMembers != null;
            }
            set
            {
                bool child = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the kpi.
        /// </summary>
        /// <value>The name of the kpi.</value>
        [DataMember]
        public string Kpi_Name { get; set; }
        #endregion

        #region Public Methods
      
        [DefaultValue("")]
        [DataMember]
        public string KPIStatusGraphic { get; set; }

        [DefaultValue("")]
        [DataMember]
        public string KPITrendGraphic { get; set; }

        [DefaultValue(KpiTypeEnum.Kpi_None)]
        [DataMember]
        public KpiTypeEnum KPIType { get; set; }

        /// <summary>
        /// Gets or sets the level depth.
        /// </summary>
        /// <value>The level depth.</value>
        [Description("Gets or sets the member leveldepth"), DefaultValue(true)]
        [DataMember]
        public int LevelDepth { get; set; }

        [DataMember]
        public string LevelUniqueName
        {
            get;
            set;
        }

        [DataMember]
        public MemberTypeEnum Type { get; set; }

        [DataMember]
        public CubeSchema CubeSchema { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the Member object</value>
        [Description("Gets or sets the member name."), DefaultValue("")]
        [DataMember]
        public string Name { get; set; }

        [DataMember]
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

        [DataMember]
        public string ParentHierarchy { get; set; }

        [DataMember]
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

        public string ParentCustomUniqueName
        {
            get
            {
                if (!string.IsNullOrEmpty(_parentUniqueName) && this._parentCaption != string.Empty)
                {
                    string[] tempString = _parentUniqueName.Split('.');
                    if (tempString.Length > 2)
                        return tempString[0] + "." + tempString[1] + "." + tempString[2] + "." + Utils.QuoteIdentifier(this._parentCaption);
                    if (tempString.Length > 1)
                        return tempString[0] + "." + tempString[1] + "." + Utils.QuoteIdentifier(this._parentCaption);
                    return tempString[0] + "." + Utils.QuoteIdentifier(this._parentCaption);
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                _parentUniqueName = value;   
            }
        }

        /// <summary>
        /// Gets or sets the member properties.
        /// </summary>
        /// <value>The member properties.</value>
        [DataMember]
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
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [DefaultValue((string)null), Description("Gets property collection")]
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

        [Description("Gets or sets Parent level."), DefaultValue((string)null), Browsable(false)]
        [DataMember]
        public Level ParentLevel { get; set; }

        /// <summary>
        /// Gets or sets the parent member.
        /// </summary>
        /// <value>The parent member.</value>
        [DataMember]
        public Member ParentMember
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the child members.
        /// </summary>
        /// <value>The child members.</value>
        [DataMember]
        public MemberCollection ChildMembers
        {
            get
            {
                if (this._ChildMembers == null || this._ChildMembers.Count == 0)
                {    
                 this._ChildMembers = new MemberCollection();
                }

                return this._ChildMembers;
            }
            set
            {
                this._ChildMembers = value;
            }
        }

        //[DefaultValue(0)]
        //public MemberTypeEnum Type { get; set; }
        [DataMember]
        public string UniqueName
        {
            get
            {
                return this._UniqueName;
            }
            set
            {
                this._UniqueName = value;
            }
        }

        //[DataMember]
        public string CustomUniqueName
        {
            get
            {
                if (!string.IsNullOrEmpty(_UniqueName))
                {
                    string[] tempString = _UniqueName.Split('.');
                    if (tempString.Length < 3)
                        return _UniqueName;
                    return tempString[0] + "." + tempString[1] + "." + tempString[2] + "." + Utils.QuoteIdentifier(this._Caption);
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
        [DataMember]
        public bool Visible { get; set; }

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
        #endregion
    }
}
