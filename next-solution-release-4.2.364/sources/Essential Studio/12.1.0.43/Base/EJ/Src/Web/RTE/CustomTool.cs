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
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Models;


namespace Syncfusion.JavaScript.Models
{
    public class CustomTool
    {
        #region Fields
        private String name = null;
        private String tooltip = null;
        private String css = null;
        private String action = null;
        #endregion
        #region Properties
        [JsonProperty("name")]
        [DefaultValue(null)]
        public String Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        [JsonProperty("tooltip")]
        [DefaultValue(null)]
        public String Tooltip
        {
            get { return this.tooltip; }
            set { this.tooltip = value; }
        }
        [JsonProperty("css")]
        [DefaultValue(null)]
        public String Css
        {
            get { return this.css; }
            set { this.css = value; }
        }
        [JsonProperty("action")]
        [DefaultValue(null)]
        public String Action
        {
            get { return this.action; }
            set { this.action = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
          
        public class CustomToolBuilder
        {
            private CustomTool customTool = new CustomTool();
            public CustomToolBuilder(CustomTool customTool)
            {
                this.customTool = customTool;
            }
            public CustomToolBuilder Name(String name)
            {
                this.customTool.Name = name;
                return this;
            }
            public CustomToolBuilder Tooltip(String tooltip)
            {
                this.customTool.Tooltip = tooltip;
                return this;
            }
            public CustomToolBuilder Css(String css)
            {
                this.customTool.Css = css;
                return this;
            }
            public CustomToolBuilder Action(String action)
            {
                this.customTool.Action = action;
                return this;
            }
      }    
}
