//-------------------------------------------------------------------------------------------------
// <copyright file="HierarchyElementCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    [CollectionDataContract]
    public class HierarchyElementCollection : Collection<HierarchyElement>
    {
        #region Private variables
        private DimensionElement _parentDimension;
        #endregion

        #region Public Methods
        public HierarchyElementCollection(DimensionElement parentDimesionElement)
        {
            this._parentDimension = parentDimesionElement;
        }

        public HierarchyElementCollection()
        {
        }

        public HierarchyElement this[int index]
        {
            get
            {
                return (HierarchyElement)base.Items[index];
            }

            set
            {
                base.Items[index] = value;
            }
        }

        public HierarchyElement this[string name]
        {
            get
            {
                return this.FindHierarchyElementByName(name);
            }
        }

        public void Add(HierarchyElement hierarchyElement)
        {
            base.Items.Add(hierarchyElement);
        }

        public HierarchyElement FindHierarchyElementByName(string name)
        {
            foreach (HierarchyElement hierarchyElement in this.Items)
            {
                if (hierarchyElement.Name == name)
                {
                    return hierarchyElement;
                }
            }

            return null;
        }

        public void Remove(HierarchyElement hierarchyElement)
        {
            base.Items.Remove(hierarchyElement);
        }
        #endregion

        #region Protected Methods
        //protected override void OnInsertComplete(int index, object value)
        //{
        //    this.UpdateParent(value);
        //}

        //protected override void OnSetComplete(int index, object oldValue, object newValue)
        //{
        //    this.UpdateParent(newValue);
        //}
        #endregion

        #region Private Methods
        void UpdateParent(object hierarchyObj)
        {
            if (hierarchyObj is HierarchyElement)
            {
                HierarchyElement hierarchyElement = (HierarchyElement)hierarchyObj;
                hierarchyElement.ParentDimension = _parentDimension;
            }
        }
        #endregion
    }
}
