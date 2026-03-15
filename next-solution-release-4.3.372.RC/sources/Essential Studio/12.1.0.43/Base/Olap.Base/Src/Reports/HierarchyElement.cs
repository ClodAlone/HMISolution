//-------------------------------------------------------------------------------------------------
// <copyright file="HierarchyElement.cs" company="syncfusion">
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
    /// Represents the hierarchy element.
    /// </summary>
    [Serializable]
    public class HierarchyElement : Element, ICloneable<HierarchyElement>
#else
using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Common;
namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class HierarchyElement : Element
#endif
    {
        #region Private Properties
        DimensionElement _parentDimension;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyElement"/> class.
        /// </summary>
        /// <param name="parentDimensionElement">The parent dimension element.</param>
        public HierarchyElement(DimensionElement parentDimensionElement)
        {
            this.Name = string.Empty;
            this.LevelElements = new LevelElementCollection(this);
#if !SILVERLIGHT
            this.Properties = new PropertyCollection();
#endif
            this.ParentDimension = parentDimensionElement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyElement"/> class.
        /// </summary>
        public HierarchyElement()
        {
            this.Name = string.Empty;
            this.LevelElements = new LevelElementCollection(this);
#if !SILVERLIGHT
            this.Properties = new PropertyCollection();
#endif
        }
        #endregion

        #region Public Properties
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the level elements.
        /// </summary>
        /// <value>The level elements.</value>
        public LevelElementCollection LevelElements { get; set; }
        #endregion

        #region Internal Properties
        [XmlIgnoreAttribute(), DefaultValue((string)null)]
        internal DimensionElement ParentDimension 
        { 
            get 
            { 
                return _parentDimension; 
            } 
            
            set 
            { 
                _parentDimension = value; 
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        [XmlIgnoreAttribute()]
        public string UniqueName
        {
            get
            {
                if (this.ParentDimension != null)
                {
                    return this.ParentDimension.UniqueName + "." + Utils.QuoteIdentifier(this.Name);
                }

                return string.Empty;
            }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(false)]
        public bool IsAttributeHierarchy { get; set; }

        /// <summary>
        /// Adds the specified level name.
        /// </summary>
        /// <param name="levelName">Name of the level.</param>
        /// <returns>true if successfully added, else other wise.</returns>
        public bool Add(string levelName)
        {
            LevelElement levelElement = this.LevelElements[levelName];
            if (levelElement == null)
            {
                this.LevelElements.Add(new LevelElement(this) { Name = levelName });
                return true;
            }

            return false;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="HierarchyElement"/>.</returns>
        public new HierarchyElement Clone()
        {
            HierarchyElement hierarchyElement = new HierarchyElement(this.ParentDimension);
            hierarchyElement.Name = this.Name;
            hierarchyElement.LevelElements = this.LevelElements.Clone();
            hierarchyElement.Properties = this.Properties.Clone();
            return hierarchyElement;
        }
#endif
        #endregion
    }
}
