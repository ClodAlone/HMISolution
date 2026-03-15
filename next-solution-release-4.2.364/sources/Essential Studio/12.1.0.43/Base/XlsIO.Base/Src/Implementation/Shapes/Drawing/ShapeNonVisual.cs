#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Xml;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;

namespace Syncfusion.XlsIO.Drawing
{    
    internal class ShapeNonVisual
    {
        private string attribute;
        private string drawingPros = "cNvPr";
        private string drawingShapeProps;
        private string nonVisual;
        private ShapeImplExt shape;

        public ShapeNonVisual(ShapeImplExt shape, string attribute)
        {
            this.shape = shape;
            this.attribute = attribute;
        }

        private void InitializeShapeType(ExcelAutoShapeType shapeType)
        {
            switch (shapeType)
            {
                case ExcelAutoShapeType.sp:
                    this.nonVisual = "nvSpPr";
                    this.drawingShapeProps = "cNvSpPr";
                    break;

                case ExcelAutoShapeType.grpSp:
                    this.nonVisual = "nvGrpSpPr";
                    this.drawingShapeProps = "cNvGrpSpPr";
                    break;

                case ExcelAutoShapeType.graphicFrame:
                    this.nonVisual = "nvGraphicFramePr";
                    this.drawingShapeProps = "cNvGraphicFramePr";
                    break;

                case ExcelAutoShapeType.cxnSp:
                    this.nonVisual = "nvCxnSpPr";
                    this.drawingShapeProps = "cNvCxnSpPr";
                    break;

                case ExcelAutoShapeType.pic:
                    this.nonVisual = "nvPicPr";
                    this.drawingShapeProps = "cNvPicPr";
                    break;
            }
        }

        private void SerializeNonVisualDrawingProps(XmlWriter xmlTextWriter)
        {
            int shapeID = this.shape.ShapeID;
            xmlTextWriter.WriteStartElement(this.attribute , this.drawingPros, Drawings.XdrNamespace);
            xmlTextWriter.WriteAttributeString("id", shapeID.ToString());

            if (this.shape.Name != null && this.shape.Name.Length > 0)
                xmlTextWriter.WriteAttributeString("name", this.shape.Name);
            else
                xmlTextWriter.WriteAttributeString("name", string.Format("{0} {1}", this.shape.AutoShapeType.ToString(), shapeID));

            if (this.shape.Description != null && this.shape.Description.Length > 0)
                xmlTextWriter.WriteAttributeString("descr", this.shape.Description);

            if (this.shape.IsHidden)
                xmlTextWriter.WriteAttributeString("hidden", "1");

            if (this.shape.Title != null && this.shape.Title.Length > 0)
                xmlTextWriter.WriteAttributeString("title", this.shape.Title);

            xmlTextWriter.WriteEndElement();
        }

        private void SerializeNonVisualDrawingShapeProps(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement(this.attribute, this.drawingShapeProps, Drawings.XdrNamespace);
            //if (this.shape.IsTextBox)
            //{
            //    xmlTextWriter.WriteAttributeString("txBox", "1");
            //}
            xmlTextWriter.WriteEndElement();
        }

        internal void Write(XmlWriter xmlTextWriter)
        {
            ExcelAutoShapeType shapeType = this.shape.ShapeType;
            this.InitializeShapeType(shapeType);
            xmlTextWriter.WriteStartElement(this.attribute, this.nonVisual, Drawings.XdrNamespace);
            this.SerializeNonVisualDrawingProps(xmlTextWriter);
            this.SerializeNonVisualDrawingShapeProps(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
        }
    }
}

