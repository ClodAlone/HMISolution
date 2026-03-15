#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.Models
{
    using Syncfusion.JavaScript.Shared.Serializer;
    public class ToolBar<T> where T:class
    {
        private bool allowToolBar = false;
        private List<String> defaultToolbarItems = new List<String>();
        private List<Object> customToolbarItems = new List<Object>();

        //Properties

        [JsonProperty("allowToolBar")]
        [DefaultValue(false)]
        public bool AllowToolbar
        {
            get { return this.allowToolBar; }
            set { this.allowToolBar = value; }
        }

        [JsonProperty("toolBarItems")]       
        public List<String> ToolBarItems
        {
            get { return this.defaultToolbarItems; }
            set { this.defaultToolbarItems = value; }
        }

        [JsonProperty("customToolbarItems")]       
        public List<Object> CustomToolbarItems
        {
            get { return this.customToolbarItems; }
            set { this.customToolbarItems = value; }
        }

        #region ShouldSerialize Methods

        public bool ShouldSerializeToolBarItems()
        {
            if (ToolBarItems.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeCustomToolbarItems()
        {
            if (CustomToolbarItems.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
