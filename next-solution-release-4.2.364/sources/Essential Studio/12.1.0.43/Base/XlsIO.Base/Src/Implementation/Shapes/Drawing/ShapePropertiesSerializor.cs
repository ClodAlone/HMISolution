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
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;

namespace Syncfusion.XlsIO.Drawing
{    
    internal class ShapePropertiesSerializor
    {
        private string attribute;
        private ShapeImplExt shape;
        private  int dpiX;
        private int dpiY;
        public ShapePropertiesSerializor(ShapeImplExt shape, string attribute)
        {
            this.shape = shape;
            this.attribute = attribute;
            this.dpiX = shape.Worksheet.AppImplementation.GetdpiX();
            this.dpiY = shape.Worksheet.AppImplementation.GetdpiY();
        }

        private void SerializeEffectProperties(XmlWriter xmlTextWriter)
        {
            Stream preservedStream;
            if (this.shape.PreservedElements.TryGetValue("Effect", out preservedStream))
            {

                if (preservedStream != null && preservedStream.Length > 0)
                {
                    preservedStream.Position = 0;
                    ShapeParser.WriteNodeFromStream(xmlTextWriter, preservedStream);
                }
            }
        }

        private void SerializeFillProperties(XmlWriter xmlTextWriter)
        {
            if (!this.shape.Fill.Visible)
            {
                xmlTextWriter.WriteStartElement("a", "noFill", Drawings.ANamespace);
                xmlTextWriter.WriteEndElement();
            }
            else if (this.shape.Logger.GetPreservedItem(PreservedFlag.Fill))
            {
                IInternalFill fill = (IInternalFill)this.shape.Fill;
                FileDataHolder dataHolder = this.shape.Worksheet.DataHolder.ParentHolder;
                Syncfusion.XlsIO.Implementation.XmlSerialization.Charts.ChartSerializatorCommon.SerializeFill(xmlTextWriter, fill, dataHolder, this.shape.Relations);
            }
            else
            {
                Stream preservedStream;
                if (this.shape.PreservedElements.TryGetValue("Fill", out preservedStream))
                {
                    if (preservedStream != null && preservedStream.Length > 0)
                    {
                        preservedStream.Position = 0;
                        ShapeParser.WriteNodeFromStream(xmlTextWriter, preservedStream);
                    }
                }
            }
        }

        private void SerializeGemoerty(XmlWriter xmlTextWriter)
        {
            Stream preservedStream;
            xmlTextWriter.WriteStartElement("a", "prstGeom", Drawings.ANamespace);
            AutoShapeConstant autoShapeConstant = AutoShapeHelper.GetAutoShapeConstant(this.shape.AutoShapeType);
            if (autoShapeConstant != AutoShapeConstant.Index_187)
            {
                xmlTextWriter.WriteAttributeString("prst", AutoShapeHelper.GetAutoShapeString(autoShapeConstant));
            }
            else
            {
                xmlTextWriter.WriteAttributeString("prst", "rect");
            }
            if (this.shape.PreservedElements.TryGetValue("avLst", out preservedStream))
            {
                if (preservedStream != null && preservedStream.Length > 0)
                {
                    preservedStream.Position = 0;
                    ShapeParser.WriteNodeFromStream(xmlTextWriter, preservedStream);
                }
            }
            else
            {
                xmlTextWriter.WriteStartElement("a", "avLst", Drawings.ANamespace);
                xmlTextWriter.WriteEndElement();
            }
            xmlTextWriter.WriteEndElement();
        }

        private void SerializeLineProperties(XmlWriter xmlTextWriter)
        {
            
            if (this.shape.Logger.GetPreservedItem(PreservedFlag.Line))
            {
                IShapeLineFormat lineFormat = shape.Line;
                DrawingShapeSerializator.SerializeLineSettings(xmlTextWriter, lineFormat, this.shape.Worksheet.Workbook);
            }
            else
            {
                Stream preservedStream;
                if (this.shape.PreservedElements.TryGetValue("Line", out preservedStream))
                {

                    if (preservedStream != null && preservedStream.Length > 0)
                    {
                        preservedStream.Position = 0;
                        ShapeParser.WriteNodeFromStream(xmlTextWriter, preservedStream);
                    }
                }
            }
        }

        private void SerializeScence3d(XmlWriter xmlTextWriter)
        {
            Stream preservedStream;
            if (this.shape.PreservedElements.TryGetValue("Scene3d", out preservedStream))
            {

                if (preservedStream != null && preservedStream.Length > 0)
                {
                    preservedStream.Position = 0;
                    ShapeParser.WriteNodeFromStream(xmlTextWriter, preservedStream);
                }
            }
        }

        private void SerializeShape3d(XmlWriter xmlTextWriter)
        {
            Stream preservedStream;
            if (this.shape.PreservedElements.TryGetValue("Sp3d", out preservedStream))
            {

                if (preservedStream != null && preservedStream.Length > 0)
                {
                    preservedStream.Position = 0;
                    ShapeParser.WriteNodeFromStream(xmlTextWriter, preservedStream);
                }
            }
        }

        private void SerializeTransformation(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("a", "xfrm", Drawings.ANamespace);
            if (this.shape.Rotation != 0 && this.shape.Rotation > 0)
                xmlTextWriter.WriteAttributeString("rot", Helper.ToString(this.shape.Rotation * 60000.0));
            if (this.shape.FlipHorizontal)
                xmlTextWriter.WriteAttributeString("flipH", "1");
            if(this.shape.FlipVertical)
                xmlTextWriter.WriteAttributeString("flipV", "1");

            xmlTextWriter.WriteStartElement("a", "off", Drawings.ANamespace);
            xmlTextWriter.WriteAttributeString("x", "0");
            xmlTextWriter.WriteAttributeString("y", "0");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("a", "ext", Drawings.ANamespace);
            xmlTextWriter.WriteAttributeString("cx", "0");
            xmlTextWriter.WriteAttributeString("cy", "0");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
        }

        internal void Write(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement(this.attribute, "spPr", Drawings.XdrNamespace);
            this.SerializeTransformation(xmlTextWriter);
            this.SerializeGemoerty(xmlTextWriter);
            this.SerializeFillProperties(xmlTextWriter);
            this.SerializeLineProperties(xmlTextWriter);
            this.SerializeEffectProperties(xmlTextWriter);
            this.SerializeScence3d(xmlTextWriter);
            this.SerializeShape3d(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
        }
    }
}

