//-------------------------------------------------------------------------------------------------
// <copyright file="MemberElement.cs" company="syncfusion">
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
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Data;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the member element information.
    /// </summary>
    [Serializable]
    public class MemberElement : Element, ICloneable<MemberElement>
#else

using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class MemberElement : Element
#endif
    {
        #region Private Variables
        LevelElement _ParentLevelElement;

        MemberElement _ParentMemberElement;

        private string _UniqueName;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberElement"/> class.
        /// </summary>
        /// <param name="parentLevelElement">The parent level element.</param>
        public MemberElement(LevelElement parentLevelElement)
        {
            this.ParentLevelElement = parentLevelElement;
            this._UniqueName = string.Empty;
            this.DimensionName = string.Empty;
            this.ChildMemberElements = new MemberElementCollection(this);
#if !SILVERLIGHT
            this.Properties = new PropertyCollection();
#endif
            this.IsParentLevel = true;
            this.IsSelectedChildMembers = false;
            this.ShowChildMembers = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberElement"/> class.
        /// </summary>
        /// <param name="parentMemberElement">The parent member element.</param>
        public MemberElement(MemberElement parentMemberElement)
        {
            this.ParentMemberElement = parentMemberElement;
            this._UniqueName = string.Empty;
            this.DimensionName = string.Empty;
            this.ChildMemberElements = new MemberElementCollection(this);
#if !SILVERLIGHT
            this.Properties = new PropertyCollection();
#endif
            this.IsParentLevel = false;
            this.IsSelectedChildMembers = false;
            this.ShowChildMembers = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberElement"/> class.
        /// </summary>
        public MemberElement()
        {
            this.Name = string.Empty;
            this.Visible = true;
            this._UniqueName = string.Empty;
            this.DimensionName = string.Empty;
            this.ChildMemberElements = new MemberElementCollection(this);
#if !SILVERLIGHT
            this.Properties = new PropertyCollection();
#endif
            this.IsSelectedChildMembers = false;
            this.ShowChildMembers = false;
        }
        #endregion

        #region Public Properties
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the child member elements.
        /// </summary>
        /// <value>The child member elements.</value>
        [DefaultValue((string)null)]
        public MemberElementCollection ChildMemberElements { get; set; }

        /// <summary>
        /// Used only to display the the Caption in AxisElementBuilder
        /// </summary>
        public string DimensionName { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public int Level
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is parent level.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is parent level; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool IsParentLevel { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        /// <value>The unique name of <see cref="MemberElement"/>.</value>
        [DefaultValue("")]
        public string UniqueName
        {
            get
            {
                if (_UniqueName != string.Empty)
                {
                    return _UniqueName;
                }

                if (IsParentLevel)
                {
                    if (ParentLevelElement == null)
                    {
                        return this.Name;
                    }

                    return ParentLevelElement.UniqueName + "." + Utils.QuoteIdentifier(this.Name);
                }
                else
                {
                    if (ParentMemberElement == null)
                    {
                        return this.Name;
                    }

                    return ParentMemberElement.UniqueName + "." + Utils.QuoteIdentifier(this.Name);
                }
            }

            set
            {
                _UniqueName = value;
            }
        }

        /// <summary>
        /// Gets or sets the is selected members.
        /// </summary>
        /// <value>Contains whether the child nodes have explicitly selected members</value>
        [DefaultValue(false)]
        public bool IsSelectedChildMembers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display the child Members
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(false)]
        public bool ShowChildMembers { get; set; }

        /// <summary>
        /// Gets or sets the parent level element.
        /// </summary>
        /// <value>The parent level element.</value>
        [XmlIgnoreAttribute(), DefaultValue((string)null)]
        public LevelElement ParentLevelElement
        {
            get
            {
                return _ParentLevelElement;
            }

            set
            {
                _ParentLevelElement = value;
                IsParentLevel = true;
            }
        }

        /// <summary>
        /// Gets or sets the parent member element.
        /// </summary>
        /// <value>The parent member element.</value>
        [XmlIgnoreAttribute(), DefaultValue((string)null)]
        public MemberElement ParentMemberElement
        {
            get
            {
                return _ParentMemberElement;
            }

            set
            {
                _ParentMemberElement = value;
                IsParentLevel = false;
            }
        }
        #endregion

        #region Public Methods
#if !SILVERLIGHT
        /// <summary>
        /// Adds the specified member name.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>The index of the memberElement</returns>
        public int Add(string memberName)
        {
            return this.ChildMemberElements.Add(new MemberElement(this) { Name = memberName });
        }

        /// <summary>
        /// Adds the specified member element.
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        /// <returns>The index of the memberElement</returns>
        public int Add(MemberElement memberElement)
        {
            return this.ChildMemberElements.Add(memberElement);
        }
#else
        public void Add(string memberName)
        {
            this.ChildMemberElements.Add(new MemberElement(this) { Name = memberName });
        }

        public void Add(MemberElement memberElement)
        {
            this.ChildMemberElements.Add(memberElement);
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="MemberElement"/>.</returns>
        public new MemberElement Clone()
        {
            MemberElement memberElement;
            if (this.IsParentLevel)
            {
                memberElement = new MemberElement(this.ParentLevelElement);
            }
            else
            {
                memberElement = new MemberElement(this.ParentMemberElement);
            }

            memberElement.Name = this.Name;
            memberElement.ChildMemberElements = this.ChildMemberElements.Clone();
            memberElement.Properties = this.Properties.Clone();
            return memberElement;
        }
#endif 
        #endregion
    }
}
