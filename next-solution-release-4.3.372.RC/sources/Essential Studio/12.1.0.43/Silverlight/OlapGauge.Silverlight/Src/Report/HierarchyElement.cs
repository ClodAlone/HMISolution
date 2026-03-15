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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    [DataContract]
    public class HierarchyElement : Element
    {
        #region Private Properties
        DimensionElement _ParentDimension;
        #endregion

        #region Constructor
        public HierarchyElement(DimensionElement parentDimensionElement)
        {
            this.Name = string.Empty;
            this.LevelElements = new LevelElementCollection(this);
            this.ParentDimension = parentDimensionElement;
        }

        public HierarchyElement()
        {
            this.Name = string.Empty;
            this.LevelElements = new LevelElementCollection(this);
        }
        #endregion

        #region Public Properties
        [DataMember]
        public LevelElementCollection LevelElements { get; set; }
        #endregion

        #region Internal Properties
        [XmlIgnoreAttribute(), DefaultValue((string)null)]
        internal DimensionElement ParentDimension 
        { 
            get 
            { 
                return _ParentDimension; 
            } 
            
            set 
            { 
                _ParentDimension = value; 
            }
        }
        #endregion

        #region Public Properties
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

        #endregion
    }
}
