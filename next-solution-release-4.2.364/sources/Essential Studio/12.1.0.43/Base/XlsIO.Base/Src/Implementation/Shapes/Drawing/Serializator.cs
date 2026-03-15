#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.IO;
using System.Text;
using System.Xml;

namespace Syncfusion.XlsIO.Drawing
{
    
    internal class Serializator
    {
        internal Serializator()
        {

        }


        internal void AddShape(ShapeImplExt shape, XmlWriter xmlTextwriter)
        {
            new AutoShapeSerializator(shape).Write(xmlTextwriter);
        }

        private void WriteHeader(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartDocument(true);
            xmlTextWriter.WriteStartElement("xdr:wsDr");
            xmlTextWriter.WriteAttributeString("xmlns", "xdr", null, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
            xmlTextWriter.WriteAttributeString("xmlns", "a", null, "http://schemas.openxmlformats.org/drawingml/2006/main");
            xmlTextWriter.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        }
    }
}

