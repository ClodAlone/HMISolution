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
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Models
{
    public class ToolBarOptions
    {
        private List<string> toolBarItems =new List<string>();
        private bool allowToolBar = false;

        //Properties

        [JsonProperty("allowToolBar")]
        [DefaultValue(false)]
        public bool AllowToolbar
        {
            get { return this.allowToolBar; }
            set { this.allowToolBar = value; }
        }
        [JsonProperty("toolBarItems")]
        [DefaultValue(null)]

        public List<String> ToolBarItems
        {
            get { return this.toolBarItems; }
            set { this.toolBarItems = value; }
        }

        #region ShouldSerialize Methods

        public bool ShouldSerializeToolBarItems()
        {
            if (ToolBarItems.Count != 0)
                return true;
            else
                return false;
        }

        #endregion
    }
}