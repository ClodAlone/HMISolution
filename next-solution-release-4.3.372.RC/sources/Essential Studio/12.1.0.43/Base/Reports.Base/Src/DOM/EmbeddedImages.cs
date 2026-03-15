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
using System.Xml;
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class EmbeddedImages : List<EmbeddedImage>
    {
    }

    public class EmbeddedImage
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public string MIMEType { get; set; }
        public string ImageData { get; set; }
    }
}
