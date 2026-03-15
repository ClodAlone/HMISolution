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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    public class MemberElement : Element
    {
        #region Private Variables
        LevelElement _ParentLevelElement;

        MemberElement _ParentMemberElement;

        private string _UniqueName;
        #endregion

        #region Constructor
        public MemberElement(LevelElement parentLevelElement)
        {
            this.ParentLevelElement = parentLevelElement;
            this._UniqueName = string.Empty;
            this.DimensionName = string.Empty;
            this.ChildMemberElements = new MemberElementCollection(parentLevelElement);
            this.IsParentLevel = true;
            this.IsSelectedChildMembers = false;
        }

        public MemberElement(MemberElement parentMemberElement)
        {
            this.ParentMemberElement = parentMemberElement;
            this._UniqueName = string.Empty;
            this.DimensionName = string.Empty;
            this.ChildMemberElements = new MemberElementCollection(parentMemberElement);
            this.IsParentLevel = false;
            this.IsSelectedChildMembers = false;
        }

        public MemberElement()
        {
            this.Name = string.Empty;
            this.Visible = true;
            this._UniqueName = string.Empty;
            this.DimensionName = string.Empty;
            this.ChildMemberElements = new MemberElementCollection(this);
            this.IsSelectedChildMembers = false;
        }
        #endregion

        #region Public Properties
        [DefaultValue((string)null)]
        public MemberElementCollection ChildMemberElements { get; set; }

        /// <summary>
        /// Used only to display the the Caption in AxisElementBuilder
        /// </summary>
        public string DimensionName { get; set; }

        [DefaultValue(false)]
        public bool IsParentLevel { get; set; }

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
                    if (ParentLevelElement == null)
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
        [DefaultValue(false)]
        public bool ShowChildMembers { get; set; }
        #endregion

        #region Internal Properties
        [XmlIgnoreAttribute(), DefaultValue((string)null)]
        internal LevelElement ParentLevelElement
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

        [XmlIgnoreAttribute(), DefaultValue((string)null)]
        internal MemberElement ParentMemberElement
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
        public void Add(string memberName)
        {
            this.ChildMemberElements.Add(new MemberElement(this) { Name = memberName });
        }

        public void Add(MemberElement memberElement)
        {
            this.ChildMemberElements.Add(memberElement);
        }

        #endregion
    }
}
