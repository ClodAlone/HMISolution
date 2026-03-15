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

namespace Syncfusion.RDL.ItemModel
{
    internal class ImagePropertiesExp
    {
        public BorderExp Border { get; set; }
        public ImageActionInfoExp ImageActionInfo { get; set; }
        public string MIMEType { get; set; }
        public string ToolTip { get; set; }
        public string Value { get; set; }
        public string BookMark { get; set; }
        public string DocumentMapLabel { get; set; }
        public string ZIndex { get; set; }
        public ThicknessExp Padding { get; set; }
        public string Hidden { get; set; }
    }

    internal class ImageActionInfoExp
    {
        public string Hyperlink { get; set; }
        public string BookmarkLink { get; set; }
        public string ReportName { get; set; }
        public List<ImageParameterExp> Parameters { get; set; }
    }

    internal class ImageParameterExp
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Omit { get; set; }
    }
}
