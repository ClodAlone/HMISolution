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

namespace Syncfusion.RDL.DOM
{
    public class Paragraphs : List<Paragraph>
    {

    }


    public class Paragraph
    {
        public TextRuns TextRuns { get; set; }
        public string LeftIndent { get; set; }
        public string RightIndent { get; set; }
        public string HangingIndent { get; set; }
        public Style Style { get; set; }
        public ListStyle ListStyle { get; set; }
        public int ListLevel { get; set; }
        public Size SpaceBefore { get; set; }
        public Size SpaceAfter { get; set; }

        public bool ShouldSerializeListLevel()
        {
            return this.ListLevel != 0;
        }

        public void RestListLevel()
        {
            this.ListLevel = 0;
        }

        public bool ShouldSerializeListStyle()
        {
            return this.ListStyle != ListStyle.None;
        }

        public void RestListStyle()
        {
            this.ListStyle = ListStyle.None;
        }
    }
}
