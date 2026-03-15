#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Xml;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;

namespace Syncfusion.XlsIO.Drawing
{
    internal class AutoShapeSerializator
    {
        private AnchorType anchorType;
        private string attribute;
        private string chartNameSpace = "http://schemas.openxmlformats.org/drawingml/2006/chartDrawing";
        private ShapeImplExt shape;
        private int resolution;

        internal AutoShapeSerializator(ShapeImplExt shape)
        {
            this.shape = shape;
            this.attribute = "xdr";
            this.anchorType = shape.AnchorType;
            this.resolution = shape.Worksheet.AppImplementation.GetdpiX();
        }

        private void SerializeAbsoluteAnchor(XmlWriter xmlTextWriter)
        {

            int width = this.shape.ClientAnchor.Width;
            int height = this.shape.ClientAnchor.Height;
            if (width < 0x38f)
            {
                width = 900;
                height = 600;
            }

            width = Helper.ConvertOffsetToEMU(width, this.resolution);
            height = Helper.ConvertOffsetToEMU(height, this.resolution);
            xmlTextWriter.WriteStartElement("xdr","pos",Drawings.XdrNamespace);
            xmlTextWriter.WriteAttributeString("x", "0");
            xmlTextWriter.WriteAttributeString("y", "0");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "ext", Drawings.XdrNamespace);
            xmlTextWriter.WriteAttributeString("cx", Helper.ToString(width));
            xmlTextWriter.WriteAttributeString("cy", Helper.ToString(height));
            xmlTextWriter.WriteEndElement();
        }

        private void SerializeAnchorPosition(XmlWriter xmlTextWriter)
        {
            switch (this.anchorType)
            {
                case AnchorType.Absolute:
                    this.SerializeAbsoluteAnchor(xmlTextWriter);
                    break;

                case AnchorType.RelSize:
                    this.SerializeRelSizeAnchor(xmlTextWriter);
                    break;

                case AnchorType.OneCell:
                    this.SerializeOneCellAnchor(xmlTextWriter);
                    break;

                default:
                    this.SerializeTwoCellAnchor(xmlTextWriter);
                    break;
            }
        }

        private void SerializeClientData(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("xdr", "clientData", Drawings.XdrNamespace);
            xmlTextWriter.WriteEndElement();
        }

        private void SerializeConnector(XmlWriter xmlTextWriter)
        {
            new ShapePropertiesSerializor(this.shape, this.attribute).Write(xmlTextWriter);
            new ShapeStyle(this.shape, this.attribute).Write(xmlTextWriter);
        }

        private void SerializeGenralShapes(XmlWriter xmlTextWriter)
        {
            new ShapePropertiesSerializor(this.shape, this.attribute).Write(xmlTextWriter);
            new ShapeStyle(this.shape, this.attribute).Write(xmlTextWriter);
            new TextBody(this.shape, this.attribute).Write(xmlTextWriter);
        }

        private void SerializeGraphicFrame(XmlWriter xmlTextWriter)
        {
            throw new NotImplementedException();
        }

        private void SerializeGroupShapes(XmlWriter xmlTextWriter)
        {
            throw new NotImplementedException();
        }

        private void SerializeOneCellAnchor(XmlWriter xmlTextWriter)
        {
            int leftColumn = this.shape.ClientAnchor.LeftColumn;
            int leftColumnOffset = this.shape.ClientAnchor.LeftColumnOffset;
            leftColumnOffset = this.shape.ClientAnchor.CalculateColumnOffset(leftColumn, 0, leftColumn, leftColumnOffset);

            int topRow = this.shape.ClientAnchor.TopRow;
            int topRowOffset = this.shape.ClientAnchor.TopRowOffset;
            topRowOffset = this.shape.ClientAnchor.CalculateRowOffset(topRow, 0, topRow, topRowOffset);

            int width = this.shape.ClientAnchor.Width;
            int height = this.shape.ClientAnchor.Height;
            width = Helper.ConvertOffsetToEMU(width, this.resolution);
            height = Helper.ConvertOffsetToEMU(height, this.resolution);

            xmlTextWriter.WriteStartElement("xdr", "from", Drawings.XdrNamespace);
            xmlTextWriter.WriteStartElement("xdr", "col", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(leftColumn));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "colOff", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(Helper.ConvertOffsetToEMU(leftColumnOffset, this.resolution)));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "row", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(topRow));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "rowOff", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(Helper.ConvertOffsetToEMU(topRowOffset, this.resolution)));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "ext", Drawings.XdrNamespace);
            xmlTextWriter.WriteAttributeString("cx", Helper.ToString(width));
            xmlTextWriter.WriteAttributeString("cy", Helper.ToString(height));
            xmlTextWriter.WriteEndElement();
        }

        private void SerializePicture(XmlWriter xmlTextWriter)
        {
            new ShapePropertiesSerializor(this.shape, this.attribute).Write(xmlTextWriter);
            new ShapeStyle(this.shape, this.attribute).Write(xmlTextWriter);
        }

        private void SerializeRelSizeAnchor(XmlWriter xmlTextWriter)
        {
            throw new NotImplementedException();
        }

        private void SerializeShapeChoices(XmlWriter xmlTextWriter)
        {
            ExcelAutoShapeType shapeType = this.shape.ShapeType;
            xmlTextWriter.WriteStartElement(this.attribute, shapeType.ToString(), Drawings.XdrNamespace);
            if (shapeType != ExcelAutoShapeType.sp)
            {
                this.WriteShapeAttributes(xmlTextWriter, false);
            }
            else
            {
                this.WriteShapeAttributes(xmlTextWriter, true);
            }
            this.SerializeShapeNonVisual(xmlTextWriter);
            switch (shapeType)
            {
                case ExcelAutoShapeType.sp:
                    this.SerializeGenralShapes(xmlTextWriter);
                    break;

                case ExcelAutoShapeType.grpSp:
                    this.SerializeGroupShapes(xmlTextWriter);
                    break;

                case ExcelAutoShapeType.graphicFrame:
                    this.SerializeGraphicFrame(xmlTextWriter);
                    break;

                case ExcelAutoShapeType.cxnSp:
                    this.SerializeConnector(xmlTextWriter);
                    break;

                case ExcelAutoShapeType.pic:
                    this.SerializePicture(xmlTextWriter);
                    break;
            }
            xmlTextWriter.WriteEndElement();
        }

        private void SerializeShapeNonVisual(XmlWriter xmlTextWriter)
        {
            new ShapeNonVisual(this.shape, this.attribute).Write(xmlTextWriter);
        }

        private void SerializeTwoCellAnchor(XmlWriter xmlTextWriter)
        {
            int leftColumn = this.shape.ClientAnchor.LeftColumn;
            int leftColumnOffset = this.shape.ClientAnchor.LeftColumnOffset;
            leftColumnOffset = this.shape.ClientAnchor.CalculateColumnOffset(leftColumn, 0, leftColumn, leftColumnOffset);

            int topRow = this.shape.ClientAnchor.TopRow;
            int topRowOffset = this.shape.ClientAnchor.TopRowOffset;
            topRowOffset = this.shape.ClientAnchor.CalculateRowOffset(topRow, 0, topRow, topRowOffset);

            int rightColumn = this.shape.ClientAnchor.RightColumn;
            int rightColumnOffset = this.shape.ClientAnchor.RightColumnOffset;
            rightColumnOffset = this.shape.ClientAnchor.CalculateColumnOffset(rightColumn, 0, rightColumn, rightColumnOffset);

            int bottomRow = this.shape.ClientAnchor.BottomRow;
            int bottomRowOffset = this.shape.ClientAnchor.BottomRowOffset;
            bottomRowOffset = this.shape.ClientAnchor.CalculateRowOffset(bottomRow, 0, bottomRow, bottomRowOffset);

            xmlTextWriter.WriteStartElement("xdr", "from", Drawings.XdrNamespace);
            xmlTextWriter.WriteStartElement("xdr", "col", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(leftColumn));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "colOff", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(Helper.ConvertOffsetToEMU(leftColumnOffset, this.resolution)));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "row", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(topRow));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "rowOff", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(Helper.ConvertOffsetToEMU(topRowOffset, this.resolution)));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "to", Drawings.XdrNamespace);
            xmlTextWriter.WriteStartElement("xdr", "col", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(rightColumn));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "colOff", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(Helper.ConvertOffsetToEMU(rightColumnOffset, this.resolution)));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "row", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(bottomRow));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("xdr", "rowOff", Drawings.XdrNamespace);
            xmlTextWriter.WriteString(Helper.ToString(Helper.ConvertOffsetToEMU(bottomRowOffset, this.resolution)));
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
        }

        internal void Write(XmlWriter xmlTextWriter)
        {
            switch (this.anchorType)
            {
                case AnchorType.Absolute:
                    xmlTextWriter.WriteStartElement("xdr", "absoluteAnchor", Drawings.XdrNamespace);
                    break;

                case AnchorType.RelSize:
                    xmlTextWriter.WriteStartElement("cdr:relSizeAnchor");
                    xmlTextWriter.WriteAttributeString("xmlns:cdr", this.chartNameSpace);
                    break;

                case AnchorType.OneCell:
                    xmlTextWriter.WriteStartElement("xdr", "oneCellAnchor", Drawings.XdrNamespace);
                    break;

                default:
                    xmlTextWriter.WriteStartElement("xdr","twoCellAnchor",Drawings.XdrNamespace);
                    break;
            }
            if (this.shape.ClientAnchor.Placement != PlacementType.MoveAndSize)
            {
                string placementType = Helper.GetPlacementType(this.shape.ClientAnchor.Placement);
                xmlTextWriter.WriteAttributeString("editAs", placementType);
            }
            this.SerializeAnchorPosition(xmlTextWriter);
            this.SerializeShapeChoices(xmlTextWriter);
            this.SerializeClientData(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
        }

        private void WriteShapeAttributes(XmlWriter xmlTextWriter, bool isGeneralShape)
        {
            string macro = this.shape.Macro;
            if (macro != null)
            {
                xmlTextWriter.WriteAttributeString("macro", macro);
            }
            if (this.shape.Published)
            {
                xmlTextWriter.WriteAttributeString("fPublished", "1");
            }
            if (isGeneralShape)
            {
                string textLink = this.shape.TextLink;
                if (textLink != null)
                {
                    xmlTextWriter.WriteAttributeString("textlink", textLink);
                }
            }
        }
    }
}

