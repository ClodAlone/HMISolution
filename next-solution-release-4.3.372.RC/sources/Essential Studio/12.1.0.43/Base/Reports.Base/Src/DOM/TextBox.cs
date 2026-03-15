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
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class TextBox : ReportItem
    {
        public bool CanGrow { get; set; }
        public bool CanShrink { get; set; }
        public DataElementStyle DataElementStyle { get; set; }
        public string HideDuplicates { get; set; }
        public bool KeepTogether { get; set; }
        public Paragraphs Paragraphs { get; set; }
        public ToggleImage ToggleImage { get; set; }
        public UserSort UserSort { get; set; }
        [XmlIgnore]
        public string Text { get; set; }

        public bool ShouldSerializeKeepTogether()
        {
            return this.KeepTogether != false;
        }

        public void RestKeepTogether()
        {
            this.KeepTogether = false;
        }

        public bool ShouldSerializeCanGrow()
        {
            return this.CanGrow != false;
        }

        public void RestCanGrow()
        {
            this.CanGrow = false;
        }

        public bool ShouldSerializeCanShrink()
        {
            return this.CanShrink != false;
        }

        public void RestCanShrink()
        {
            this.CanShrink = false;
        }

        public bool ShouldSerializeDataElementStyle()
        {
            return this.DataElementStyle != DataElementStyle.Auto;
        }

        public void RestDataElementStyle()
        {
            this.DataElementStyle = DataElementStyle.Auto;
        }
    }
}
