//-------------------------------------------------------------------------------------------------
// <copyright file="Items.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.OlapSilverlight.Base.Report
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    
    /// <summary>
    /// Collection of item objects
    /// </summary>
    [CollectionDataContract]
    public class Items : Collection<Item>
    {
        #region Private Variables
        bool _IsFilterOrSortOn;
        #endregion

        #region Constructor
        public Items(List<Item> items)
        {
            foreach (var item in items)
            {
                base.Add(item);
            }
        }

        public Items()
        {
            _IsFilterOrSortOn = false;
        }
        #endregion

        #region Public Properties
        [DataMember,DefaultValue(false)]
        public bool IsFilterOrSortOn
        {
            get
            {
                return _IsFilterOrSortOn;
            }

            set
            {
                _IsFilterOrSortOn = value;
                UpdateFilterOrSortingStatus(_IsFilterOrSortOn);
            }
        }

        #endregion

        #region Private Methods
        private void UpdateFilterOrSortingStatus(bool value)
        {
            foreach (Item item in this)
            {
                item.IsFilterOrSortOn = value;
            }
        }
        #endregion
    }
}
