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
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Shapes;
namespace Syncfusion.XlsIO.Drawing
{   

    internal class TextBody
    {
        private string attribute;
        private ShapeImplExt shape;

        public TextBody(ShapeImplExt shape, string attribute)
        {
            this.shape = shape;
            this.attribute = attribute;
        }

        private void TextBodyProperties(XmlWriter xmlTextWriter)
        {
            TextFrame textFrame=this.shape.TextFrame;
            xmlTextWriter.WriteStartElement("a", "bodyPr", Drawings.ANamespace);
            if (textFrame.TextVertOverflowType != TextVertOverflowType.OverFlow)
            {
                xmlTextWriter.WriteAttributeString("vertOverflow", Helper.GetVerticalFlowType(textFrame.TextVertOverflowType));
            }
            if (textFrame.TextHorzOverflowType != TextHorzOverflowType.OverFlow)
            {
                xmlTextWriter.WriteAttributeString("horzOverflow", Helper.GetHorizontalFlowType(textFrame.TextHorzOverflowType));
            }
            string str = "square";
            if (!textFrame.WrapTextInShape)
            {
                str = "none";
            }
            xmlTextWriter.WriteAttributeString("wrap", str);
            if (!textFrame.IsAutoMargins)
            {
                xmlTextWriter.WriteAttributeString("lIns", Helper.ToString(textFrame.GetLeftMargin()));
                xmlTextWriter.WriteAttributeString("tIns", Helper.ToString(textFrame.GetTopMargin()));
                xmlTextWriter.WriteAttributeString("rIns", Helper.ToString(textFrame.GetRightMargin()));
                xmlTextWriter.WriteAttributeString("bIns", Helper.ToString(textFrame.GetBottomMargin()));
            }
            string anchor="t";
            bool anchorCtr = textFrame.GetAnchorPosition(out anchor);

            if (textFrame.TextDirection != TextDirection.Horizontal)
            {
                string txtDirection = textFrame.GetTextDirection(textFrame.TextDirection);
                if (txtDirection != null)
                {
                    xmlTextWriter.WriteAttributeString("vert", txtDirection);
                }
            }
            xmlTextWriter.WriteAttributeString("anchor", anchor);
            if(anchorCtr)
                xmlTextWriter.WriteAttributeString("anchorCtr", "1");
            else
                xmlTextWriter.WriteAttributeString("anchorCtr", "0");
            if (textFrame.IsAutoSize)
            {
                xmlTextWriter.WriteElementString("a:spAutoFit", null);
            }

            if (textFrame.Columns.Number > 0)
                xmlTextWriter.WriteAttributeString("numCol", Helper.ToString(textFrame.Columns.Number));
            int columnSpacing = (int)((textFrame.Columns.SpacingPt * 12700.0) + 0.5);
            if(columnSpacing>0)
                xmlTextWriter.WriteAttributeString("spcCol", Helper.ToString(columnSpacing));
            xmlTextWriter.WriteEndElement();
        }

        private void TextParagraph(XmlWriter xmlTextWriter_0)
        {
            xmlTextWriter_0.WriteStartElement("a", "p", null);
            xmlTextWriter_0.WriteStartElement("a", "r", Drawings.ANamespace);
            xmlTextWriter_0.WriteStartElement("a", "rPr", Drawings.ANamespace);

            xmlTextWriter_0.WriteAttributeString("lang", "en-US");
            xmlTextWriter_0.WriteAttributeString("sz", "1100");
            xmlTextWriter_0.WriteEndElement();

            xmlTextWriter_0.WriteStartElement("a", "t", Drawings.ANamespace);
            string text = this.shape.TextFrame.TextRange.Text;
            xmlTextWriter_0.WriteString(text);
            xmlTextWriter_0.WriteEndElement();

            xmlTextWriter_0.WriteEndElement();
            xmlTextWriter_0.WriteEndElement();

        }

        internal void Write(XmlWriter xmlTextWriter)
        {
            if (this.shape.Logger.GetPreservedItem(PreservedFlag.RichText))
            {
                xmlTextWriter.WriteStartElement(this.attribute, "txBody", Drawings.XdrNamespace);
                ITextRange textRange = this.shape.TextFrame.TextRange;
                RichTextString textArea = (RichTextString)textRange.RichText;
                TextBodyProperties(xmlTextWriter);
                TextBoxSerializator.SerializeParagraphsAutoShapes(xmlTextWriter, textArea, this.shape.Worksheet.ParentWorkbook);

                xmlTextWriter.WriteEndElement();

            }
            else
            {
                Stream preservedStream;
                if (this.shape.PreservedElements.TryGetValue("TextBody", out preservedStream))
                {
                    if (preservedStream != null && preservedStream.Length > 0)
                    {
                        preservedStream.Position = 0;
                        ShapeParser.WriteNodeFromStream(xmlTextWriter, preservedStream);
                    }
                }
            }
        }
    }
}

