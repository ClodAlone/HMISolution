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
using System.ComponentModel;
using System.Xml.Serialization;
#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Data;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// DimensionElement represents a dimension object in cube
    /// </summary>
    /// <remarks>
    /// DimenisionElement is created in OlapReport object and added into the collection.
    /// </remarks>
    [Serializable]
    public class DimensionElement : Element, ICloneable<DimensionElement>
#else

using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Reports
{
    [KnownType(typeof(MemberProperty))]
    [DataContract]
    public class DimensionElement : Element
#endif
    {
        #region Private Members
        private MemberPropertyCollection _MemberPropertyCollection;
        private string m_drillUpDownLevel = string.Empty;
        private string m_drillUpDownMember = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionElement"/> class.
        /// </summary>
        public DimensionElement()
        {
            this.Name = string.Empty;
            this.HierarchyName = string.Empty;
            this.Hierarchy = null;
            this.MemberProperties = new MemberPropertyCollection();            
#if !SILVERLIGHT
            this.Properties = new PropertyCollection();
#endif

        }
        #endregion

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the hierarchy Element based on the Hierarchy Name.
        /// </summary>
        /// <value>The hierarchy.</value>
        public HierarchyElement Hierarchy
        {
            get;
            set;
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the drill state.
        /// </summary>
        public DrillState DrillState
        {
            get;
            set;
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the level name till which expand/collapse operation needs to be done. 
        /// NOTE: This would be enabled only when DrillState option is set to "ExpandToLevel"/"CollapseToLevel".
        /// </summary>
        public string DrillUpDownLevel
        {
            get
            {
                return m_drillUpDownLevel;
            }
            set
            {
                m_drillUpDownLevel = value;
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the member name from which expand/collapse operation needs to be done. 
        /// NOTE: This would be enabled only when DrillState option is set to "ExpandToLevel"/"CollapseToLevel" and "DrillUpDownLevel" pointing to which level the expand/collapse operation needs to be done.
        /// </summary>
        public string DrillUpDownMember
        {
            get
            {
                return m_drillUpDownMember;
            }
            set
            {
                m_drillUpDownMember = value;
            }
        }

        #region Public Properties
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the name of the hierarchy.
        /// </summary>
        /// <value>The name of the hierarchy.</value>
        [DefaultValue((string)null)]
        public string HierarchyName
        {
            get
            {
                if (this.Hierarchy != null)
                {
                    return this.Hierarchy.Name;
                }
                return string.Empty;
            }

            set
            {
                SetHierarchy(value);
            }
        }

        /// <summary>
        /// Gets the uniquename.
        /// </summary>
        /// <value>The Unique name of the dimension</value>
        [XmlIgnore()]
        public string UniqueName
        {
            get
            {
                return Utils.QuoteIdentifier(Name);
            }
        }
        
        /// <summary>
        /// Gets or sets the member properties.
        /// </summary>
        /// <value>The member properties.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public MemberPropertyCollection MemberProperties
        {
            get
            {
                return _MemberPropertyCollection;
            }
            set
            {
                _MemberPropertyCollection = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the level to the existing hierarchy.
        /// </summary>
        /// <param name="hierarchyName">Name of the hierarchy.</param>
        /// <param name="levelName">Name of the level.</param>
        /// <returns>returns false if level is not added in Hierarchy Element else returns true</returns>
        public bool AddLevel(string hierarchyName, string levelName)
        {
            if (String.IsNullOrEmpty(hierarchyName))
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

        /// <summary>
        /// Adds the level to the existing Hierarchy.
        /// </summary>
        /// <param name="levelName">Name of the level.</param>
        /// <returns>false if level is not added to the hierarchy else returns true.</returns>
        //public bool AddLevel(string levelName)
        //{
        //    if (String.IsNullOrEmpty(this.HierarchyName))
        //    {
        //        return false;
        //    }

        //    this.AddLevel(this.HierarchyName, levelName);
        //    return true;
        //}

        /// <summary>
        /// Adds the member to the specified level.
        /// </summary>
        /// <param name="levelName">Name of the level.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>false if member is not added to level</returns>
        public bool AddMember(string levelName, string memberName)
        {
            if ((String.IsNullOrEmpty(levelName)) ||
                (String.IsNullOrEmpty(memberName)))
            {
                return false;
            }

            ////Adding Level and check for null case
            if (this.Hierarchy.LevelElements[levelName] != null)
            {
                this.Hierarchy.LevelElements[levelName].Add(memberName, 0);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the array of member collection to the level
        /// </summary>
        /// <param name="levelName">Name of the level.</param>
        /// <param name="memberNames">The member names.</param>
        /// <returns>false if members are not added to the level else returns true</returns>
        public bool AddMembers(string levelName, params string[] memberNames)
        {
            if ((String.IsNullOrEmpty(levelName)) ||
                (memberNames == null || memberNames.Length < 0))
            {
                return false;
            }

            foreach (string memberName in memberNames)
            {
                ////Adding Level and check for null case
                if (this.Hierarchy.LevelElements[levelName] != null)
                {
                    this.Hierarchy.LevelElements[levelName].Add(memberName, 0);
                }
            }

            return true;
        }

        /// <summary>
        /// Adds the member to the levelElement.
        /// </summary>
        /// <param name="levelName">Name of the level.</param>
        /// <param name="memberElement">The member element.</param>
        /// <returns>true if added to the level or else returns false</returns>
        public bool AddMember(string levelName, MemberElement memberElement)
        {
            if (String.IsNullOrEmpty(levelName) || memberElement == null)
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

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>returns a new instance of Dimension Element</returns>
        public new DimensionElement Clone()
        {
            DimensionElement dimensionElement = new DimensionElement();
            dimensionElement.Name = this.Name;
            dimensionElement.Hierarchy = this.Hierarchy;
            dimensionElement.Properties = this.Properties.Clone();
            return dimensionElement;
        }
#endif
        #endregion

        #region Internal Methods
        /// <summary>
        /// Sets the hierarchy.
        /// </summary>
        /// <param name="hierarchyName">Name of the hierarchy.</param>
        /// <returns>Returns true if hierarchy is set else returns false</returns>
        internal bool SetHierarchy(string hierarchyName)
        {
            if (hierarchyName != string.Empty)
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
            }

            return false;
        }
        #endregion
    }
}
