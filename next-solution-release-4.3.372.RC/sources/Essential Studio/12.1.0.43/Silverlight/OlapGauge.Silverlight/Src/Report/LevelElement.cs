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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    [DataContract]
    public class LevelElement : Element
    {
        #region Private Properties
        HierarchyElement _ParentHierarchy;
        #endregion

        #region Constructor
        public LevelElement()
        {
            this.Name = string.Empty;
            this.DimensionName = string.Empty;
            this.MemberElements = new MemberElementCollection(this);
        }

        public LevelElement(HierarchyElement parentHierarchy)
        {
            this.Name = string.Empty;
            this.DimensionName = string.Empty;
            this.MemberElements = new MemberElementCollection(this);
            this.ParentHierarchy = parentHierarchy;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Used only to display the the Caption in AxisElementBuilder
        /// </summary>
        [DataMember]
        [XmlIgnore()]
        public string DimensionName { get; set; }

        [DataMember]
        [DefaultValue((string)null)]
        public MemberElementCollection MemberElements { get; set; }

        [XmlIgnoreAttribute(), DefaultValue((string)null)]
        public HierarchyElement ParentHierarchy 
        { 
            get 
            { 
                return _ParentHierarchy; 
            } 
            
            set 
            { 
                _ParentHierarchy = value; 
            } 
        }

        [XmlIgnore()]
        public string UniqueName
        {
            get
            {
                return this.ParentHierarchy.UniqueName + "." + Utils.QuoteIdentifier(this.Name);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is child members selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is child members selected; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore()]
        public bool IncludeAvailableMembers { get; set; }
        #endregion

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

        #region Private Methods
        public void Add(string memberName)
        {
            this.MemberElements.Add(new MemberElement(this) { Name = memberName });
        }

        public void Add(MemberElement memberElement)
        {
            this.MemberElements.Add(memberElement);
        }
        #endregion
    }
}
