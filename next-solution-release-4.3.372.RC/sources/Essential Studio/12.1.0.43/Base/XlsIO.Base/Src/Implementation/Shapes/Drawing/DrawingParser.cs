#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Shapes;
using System.IO;
namespace Syncfusion.XlsIO.Drawing
{
    internal class DrawingParser
    {
        internal AutoShapeType autoShapeType = AutoShapeType.Unknown;
        internal bool isHyperLink;
        internal bool isGroupShape;
        internal double topX;
        internal double topY;
        internal double bottomX;
        internal double bottomY;
        internal AutoShapeConstant autoShapeConstant = AutoShapeConstant.Index_187;
        internal int leftColumn;
        internal int leftColumnOffset;
        internal int posX;
        internal int posY;
        internal int extCX;
        internal int extCY;
        internal int topRow;
        internal int topRowOffset;
        internal int rightColumn;
        internal int rightColumnOffset;
        internal int bottomRow;
        internal int bottomRowOffset;
        internal int cx;
        internal int cy;
        //private MsoDrawingType msoDrawingType_0 = MsoDrawingType.CellsDrawing;
        internal ClientAnchor clientAnchor;
        internal string placement;
        internal string relationID;
        internal string anchorName;
        internal int id;
        internal string name;
        internal string descr;
        internal string tittle;
        public double shapeRotation;
        public Stream CustGeomStream;
        public bool IsHidden;
        public bool FlipVertical;
        public bool FlipHorizontal;
        public string preFix = "xdr";
        public string shapeType;

        internal void AddShape(AutoShapeImpl autoShapeImpl,WorksheetImpl sheet)
        {
            autoShapeImpl.CreateShape(this.autoShapeType,sheet);
            autoShapeImpl.ShapeExt.AnchorType = Helper.GetAnchorType(this.anchorName);

            if (this.shapeType == "cxnSp")
                autoShapeImpl.ShapeExt.ShapeType = ExcelAutoShapeType.cxnSp;
            else
                autoShapeImpl.ShapeExt.ShapeType = ExcelAutoShapeType.sp;

            clientAnchor = autoShapeImpl.ShapeExt.ClientAnchor;
            if (this.anchorName == "absoluteAnchor")
            {

                this.clientAnchor.SetAnchor(this.posX, this.posY, this.cx, this.cy);

            }
            else if (this.anchorName == "oneCellAnchor")
            {
                this.clientAnchor.SetAnchor(this.topRow, this.topRowOffset, this.leftColumn, this.leftColumnOffset, this.cy, this.cx);
            }
            else if (this.anchorName == "freeFloating")
            {

                int num = 96;
                this.clientAnchor.SetAnchor(Helper.ConvertEmuToOffset(this.posX, num), Helper.ConvertEmuToOffset(this.posY, num), Helper.ConvertEmuToOffset(this.extCX, num), Helper.ConvertEmuToOffset(this.extCY, num));

            }
            else if (this.anchorName == "relSizeAnchor")
            {

                this.clientAnchor.SetAnchor((int)(this.topX * 4000.0), (int)(this.topY * 4000.0), (int)((this.bottomX - this.topX) * 4000.0), (int)((this.bottomY - this.topY) * 4000.0));

            }
            else
            {
                this.clientAnchor.SetAnchor(this.topRow, this.topRowOffset, this.leftColumn, this.leftColumnOffset, this.bottomRow, this.bottomRowOffset, this.rightColumn, this.rightColumnOffset);
            }
            if ((this.placement != null) && (this.placement.Length != 0))
            {
                this.clientAnchor.Placement = Helper.GetPlacementType(this.placement);
            }

            autoShapeImpl.SetShapeID(this.id);
            autoShapeImpl.Name = this.name;
            autoShapeImpl.AlternativeText = this.descr;
            autoShapeImpl.IsHidden = this.IsHidden;
            autoShapeImpl.Title = this.tittle;
            autoShapeImpl.ShapeExt.Rotation = this.shapeRotation;
            autoShapeImpl.ShapeExt.FlipVertical = this.FlipVertical;
            autoShapeImpl.ShapeExt.FlipHorizontal = this.FlipHorizontal;

            if (this.CustGeomStream != null && this.CustGeomStream.Length > 0)
            {
                autoShapeImpl.ShapeExt.PreservedElements.Add("avLst", this.CustGeomStream);
            }
        }
        
        
    }
}
