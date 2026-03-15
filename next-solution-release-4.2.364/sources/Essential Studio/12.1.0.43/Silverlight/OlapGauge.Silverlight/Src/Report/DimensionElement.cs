//-------------------------------------------------------------------------------------------------
// <copyright file="DimensionElement.cs" company="syncfusion">
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
    public class DimensionElement : Element
    {

        #region Private Variables

        private HierarchyElementCollection _Hierarchies;

        #endregion

        #region Constructor
        public DimensionElement()
        {
            this.Name = string.Empty;
            this.HierarchyName = string.Empty;
            this.Hierarchy = null;
        }
        #endregion

        [DataMember]
        public HierarchyElement Hierarchy
        {
            get;
            set;
        }

        #region Public Properties
        [DataMember, DefaultValue((string)null)]
        public string HierarchyName
        {
            get
            {
                //if (this.Hierarchy != null)
                //{
                return this.Hierarchy.Name;
                //}
                //return string.Empty;
            }
            set
            {
                SetHierarchy(value);
            }
        }

        [XmlIgnore()]
        public string UniqueName
        {
            get
            {
                return Utils.QuoteIdentifier(Name);
            }
        }
        #endregion

        #region Public Methods
        public bool AddLevel(string hierarchyName, string levelName)
        {
            if (hierarchyName == null && hierarchyName == string.Empty)
            {
                return false;
            }

            ////Adding hierarchy
            this.SetHierarchy(hierarchyName);
            if (Hierarchy != null)
            {
                Hierarchy.Add(levelName);
                return true;
            }

            return false;
        }

        public bool AddLevel(string levelName)
        {
            if (this.HierarchyName == string.Empty || this.HierarchyName == null)
            {
                return false;
            }

            this.AddLevel(this.HierarchyName, levelName);
            return true;
        }

        public bool AddMember(string levelName, string memberName)
        {
            if ((levelName == string.Empty || levelName == null) ||
                (memberName == string.Empty || memberName == null))
            {
                return false;
            }

            ////Adding Level and check for null case
            if (this.Hierarchy.LevelElements[levelName] != null)
            {
                this.Hierarchy.LevelElements[levelName].Add(memberName);
                return true;
            }

            return false;
        }

        public bool AddMembers(string levelName, params string[] memberNames)
        {
            if ((levelName == string.Empty || levelName == null) ||
                (memberNames == null || memberNames.Length < 0))
            {
                return false;
            }

            foreach (string memberName in memberNames)
            {
                ////Adding Level and check for null case
                if (this.Hierarchy.LevelElements[levelName] != null)
                {
                    this.Hierarchy.LevelElements[levelName].Add(memberName);
                }
            }
            return true;
        }

        public bool AddMember(string levelName, MemberElement memberElement)
        {
            if (levelName == string.Empty || levelName == null || memberElement == null)
            {
                return false;
            }

            if (this.Hierarchy.LevelElements[levelName] != null)
            {
                this.Hierarchy.LevelElements[levelName].Add(memberElement);
                return true;
            }
            return false;
        }

        #endregion

        #region Internal Methods
        internal bool SetHierarchy(string hierarchyName)
        {
            if (this.Hierarchy != null)
            {
                this.Hierarchy.Name = hierarchyName;
                this.Hierarchy.ParentDimension = this;
            }
            else
            {
                this.Hierarchy = new HierarchyElement(this) { Name = hierarchyName };
            }
            return false;
        }
        #endregion
    }
}
