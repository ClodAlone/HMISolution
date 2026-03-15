#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class PivotFieldItem : IPivotFieldItem
    {
        #region Members
        /// <summary>
        /// Member for setting the visibility of each field item.
        /// </summary>
        private bool b_Visible = true;

        /// <summary>
        /// Parent(field) of item .
        /// </summary>
        private PivotFieldImpl m_Parent;

        /// <summary>
        /// member for setting/getting the item value
        /// </summary>
        private string str_Text;

        #endregion

        #region Property

        /// <summary>
        /// Get/set the item based on text of field items.
        /// </summary>
        public string Text
        {
            get
            {
                return str_Text;
            }
            set
            {
                str_Text = value;
            }
        }

        /// <summary>
        /// Get/set the parent of the items (ie. Field)
        /// </summary>
        public PivotFieldImpl Parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                m_Parent = value;
            }
        }

        /// <summary>
        /// Get /set the visibity of each item of field
        /// </summary>
        public bool Visible
        {
            get
            {
                return b_Visible;
            }
            set
            {
                if (!value)
                    this.Parent.m_iItemInvisibleCount++;

                if (this.Parent.m_iItemInvisibleCount >= this.Parent.Items.Count)
                    throw new ArgumentNullException("All the items cannot be set invisible");

                Dictionary<int, PivotItemOptions> items = this.Parent.ItemOptions;
                foreach (KeyValuePair<int, PivotItemOptions> item in items)
                {
                    if (item.Value != null)
                        item.Value.IsHidden = false;
                }
                
                this.Parent.IsMultiSelected = true;
                b_Visible = value;
            }
        }
        #endregion

    }
}
