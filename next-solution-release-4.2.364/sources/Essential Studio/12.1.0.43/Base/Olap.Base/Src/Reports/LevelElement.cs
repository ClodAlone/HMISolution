//-------------------------------------------------------------------------------------------------
// <copyright file="LevelElement.cs" company="syncfusion">
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
    /// Represents the Level information.
    /// </summary>
    [Serializable]
    public class LevelElement : Element, ICloneable<LevelElement>
#else
using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Common;
namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class LevelElement : Element
#endif
    {
        #region Private Properties
        HierarchyElement _parentHierarchy;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelElement"/> class.
        /// </summary>
        public LevelElement()
        {
            this.Name = string.Empty;
            this.DimensionName = string.Empty;
            this.MemberElements = new MemberElementCollection(this);
#if !SILVERLIGHT
            this.Properties = new PropertyCollection();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelElement"/> class.
        /// </summary>
        /// <param name="parentHierarchy">The parent hierarchy.</param>
        public LevelElement(HierarchyElement parentHierarchy)
        {
            this.Name = string.Empty;
            this.DimensionName = string.Empty;
            this.MemberElements = new MemberElementCollection(this);
#if !SILVERLIGHT
            this.Properties = new PropertyCollection();
#endif
            this.ParentHierarchy = parentHierarchy;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Used only to display the the Caption in AxisElementBuilder
        /// </summary>
#if SILVERLIGHT
        [DataMember]
#endif
        [XmlIgnore()]
        public string DimensionName { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the member elements.
        /// </summary>
        /// <value>The member elements.</value>
        [DefaultValue((string)null)]
        public MemberElementCollection MemberElements { get; set; }

        /// <summary>
        /// Gets or sets the parent hierarchy.
        /// </summary>
        /// <value>The parent hierarchy.</value>
        [XmlIgnoreAttribute(), DefaultValue((string)null)]
        public HierarchyElement ParentHierarchy 
        { 
            get 
            { 
                return _parentHierarchy; 
            } 
            
            set 
            { 
                _parentHierarchy = value; 
            } 
        }

        /// <summary>
        /// Gets the unique name.
        /// </summary>
        /// <value>The unique name for level object.</value>
        [XmlIgnore()]
        public string UniqueName
        {
            get
            {
                if (this.ParentHierarchy != null)
                {
                    return this.ParentHierarchy.UniqueName + "." + Utils.QuoteIdentifier(this.Name);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is child members selected.
        /// </summary>
        /// <value>
        ///       <c>true</c> if this instance is child members selected; otherwise, <c>false</c>.
        /// </value>
#if SILVERLIGHT
        [DataMember]
#endif
        public bool IncludeAvailableMembers { get; set; }
        #endregion

#if !SILVERLIGHT
        #region Public Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy <see cref="LevelElement"/>.</returns>
        public new LevelElement Clone()
        {
            LevelElement levelElement = new LevelElement(this.ParentHierarchy);
            levelElement.Name = this.Name;
            levelElement.Visible = this.Visible;
            levelElement.MemberElements = this.MemberElements.Clone();
            levelElement.Properties = this.Properties.Clone();
            return levelElement;
        }

        #endregion
#endif

        #region Internal Methods
        internal bool Add(string memberName, string uniqueName)
        {
            MemberElement memberElement = this.MemberElements[memberName];
            if (memberElement == null)
            {
                this.MemberElements.Add(new MemberElement(this)
                {
                    Name = memberName,
                    UniqueName = uniqueName
                });
                return true;
            }

            return false;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified member name.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        public void Add(string memberName)
        {
            this.MemberElements.Add(new MemberElement(this) { Name = memberName });
        }

        /// <summary>
        /// Adds the specified member name.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="level">The level.</param>
        public void Add(string memberName, int level)
        {
            this.MemberElements.Add(new MemberElement(this) { Name = memberName, Level = level });
        }

        /// <summary>
        /// Adds the specified member caption.
        /// </summary>
        /// <param name="memberCaption">The member caption.</param>
        /// <param name="memberUniqueName">Name of the member unique.</param>
        /// <param name="level">The level.</param>
        public void Add(string memberCaption, string memberUniqueName, int level)
        {
            this.MemberElements.Add(new MemberElement(this) { Name = memberCaption, UniqueName = memberUniqueName, Level = level });
        }

        public void Add(string memberCaption,string parentCaption, string memberUniqueName, int level)
        {
            this.MemberElements.Add(new MemberElement(this) { Name = memberCaption, RootNodeCaption =parentCaption,  UniqueName = memberUniqueName, Level = level });
        }
        /// <summary>
        /// Adds the specified member element.
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        public void Add(MemberElement memberElement)
        {
            this.MemberElements.Add(memberElement);
        }
        #endregion
    }
}
