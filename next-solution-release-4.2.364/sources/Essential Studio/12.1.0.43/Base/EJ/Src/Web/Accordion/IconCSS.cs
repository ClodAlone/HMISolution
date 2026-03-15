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


namespace Syncfusion.JavaScript
{
    public class IconCSS
    {
        #region Fields
        private String header = "e-collaps";
        private String selectedHeader = "e-expand";
        #endregion
        #region Properties
        [JsonProperty("header")]
        [DefaultValue("e-collaps")]
        public String Header
        {
            get { return this.header; }
            set { this.header = value; }
        }
        [JsonProperty("selectedHeader")]
        [DefaultValue("e-expand")]
        public String SelectedHeader
        {
            get { return this.selectedHeader; }
            set { this.selectedHeader = value; }
        }
        #endregion
    }
    public class IconCSSBuilder
    {
        private IconCSS iconCSS = new IconCSS();
        public IconCSSBuilder(IconCSS iconCSS)
        {
            this.iconCSS = iconCSS;
        }
        public IconCSSBuilder Header(String header)
        {
            this.iconCSS.Header = header;
            return this;
        }
        public IconCSSBuilder SelectedHeader(String selectedHeader)
        {
            this.iconCSS.SelectedHeader = selectedHeader;
            return this;
        }
    }
}
