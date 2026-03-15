#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Xml;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;

namespace Syncfusion.XlsIO.Drawing
{
   
    internal class ShapeStyle
    {
        private string attribute;
        private ShapeImplExt shape;

        public ShapeStyle(ShapeImplExt shape, string arrtibute)
        {
            this.shape = shape;
            this.attribute = arrtibute;
        }

        private void EffectRef(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("a", "effectRef", Drawings.ANamespace);
            xmlTextWriter.WriteAttributeString("idx", "0");
            xmlTextWriter.WriteStartElement("a", "schemeClr", null);
            xmlTextWriter.WriteAttributeString("val", "accent1");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
        }

        private void FillRef(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("a", "fillRef", Drawings.ANamespace);
            xmlTextWriter.WriteAttributeString("idx", "1");
            xmlTextWriter.WriteStartElement("a", "schemeClr", null);
            xmlTextWriter.WriteAttributeString("val", "accent1");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
        }

        private void FontRef(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("a", "fontRef", Drawings.ANamespace);
            xmlTextWriter.WriteAttributeString("idx", "minor");
            xmlTextWriter.WriteStartElement("a", "schemeClr", null);
            xmlTextWriter.WriteAttributeString("val", "lt1");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
        }

        private void lnRef(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("a", "lnRef", Drawings.ANamespace);
            xmlTextWriter.WriteAttributeString("idx", "2");
            xmlTextWriter.WriteStartElement("a", "schemeClr", null);
            xmlTextWriter.WriteAttributeString("val", "accent1");
            xmlTextWriter.WriteStartElement("a", "shade", Drawings.ANamespace);
            xmlTextWriter.WriteAttributeString("val", "50000");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
        }

        internal void Write(XmlWriter xmlTextWriter)
        {
           
            Stream preservedStream;
            if (this.shape.PreservedElements.TryGetValue("Style", out preservedStream))
            {
                if (preservedStream != null && preservedStream.Length > 0)
                {
                    preservedStream.Position = 0;
                    ShapeParser.WriteNodeFromStream(xmlTextWriter, preservedStream);
                }
            }
            else if(this.shape.IsCreated)
            {
                xmlTextWriter.WriteStartElement(this.attribute, "style", Drawings.XdrNamespace);
                this.lnRef(xmlTextWriter);
                this.FillRef(xmlTextWriter);
                this.EffectRef(xmlTextWriter);
                this.FontRef(xmlTextWriter);
                xmlTextWriter.WriteEndElement();
            }
           
        }
    }
}

