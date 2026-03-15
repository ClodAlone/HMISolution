#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.XlsIO.Drawing;

namespace Syncfusion.XlsIO.Implementation.Shapes
{
    /// <summary>
    /// Represents the Autoshape.
    /// </summary>
    public class AutoShapeImpl : ShapeImpl
    {

        #region Members
        /// <summary>
        /// Extended the ShapeImpl class to support AutoShape.
        /// </summary>
        private ShapeImplExt m_shapeExt;
        /// <summary>
        /// Gets or set a value indicating whether shape can move with cell
        /// </summary>
        private bool m_isMoveWithCell;
        /// <summary>
        /// Gets or sets a value indicating whether shape can size with cell.
        /// </summary>
        private bool m_isSizeWithCell;
        #endregion

        #region Intialization
        internal AutoShapeImpl(IApplication application, object parent)
            : base(application, parent)
        {
            this.ShapeType = ExcelShapeType.AutoShape;
            base.m_bSupportOptions = true;
            m_isMoveWithCell = true;
            m_isSizeWithCell = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Extended ShapeImpl class to support AutoShape.
        /// </summary>
        internal ShapeImplExt ShapeExt
        {
            get
            {
                return m_shapeExt;
            }
        }

        /// <summary>
        /// Represents the Text Frame.
        /// </summary>
        public override ITextFrame TextFrame
        {
            get
            {
                return this.m_shapeExt.TextFrame;
            }
        }
        /// <summary>
        /// Represents the TextFrame for Internal purpose.
        /// TODO: Remove this property when provided the complete support (.xls & .xlsx).
        /// </summary>
        internal TextFrame TextFrameInternal
        {
            get
            {
                return this.m_shapeExt.TextFrame;
            }
        }
        /// <summary>
        /// Represents the Shape's Description.
        /// </summary>
        public override string AlternativeText
        {
            get
            {
                return this.m_shapeExt.Description;
            }
            set
            {
                this.m_shapeExt.Description = value;
            }
        }
        /// <summary>
        /// Represents the Shape's unique id.
        /// </summary>
        public override int Id
        {
            get
            {
                return m_shapeExt.ShapeID;
            }

        }
        /// <summary>
        /// Represents the name of the Shape.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.m_shapeExt.Name;
            }
            set
            {
                this.m_shapeExt.Name = value;
            }
        }
        /// <summary>
        /// Represents the BottomRow position of the Shape.
        /// </summary>
        public override int BottomRow
        {
            get
            {

                return this.m_shapeExt.ClientAnchor.BottomRow + 1;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.BottomRow = value - 1;
            }
        }
        /// <summary>
        /// Represents the BottomRowOffset of the Shape.
        /// </summary>
        public override int BottomRowOffset
        {
            get
            {
                return this.m_shapeExt.ClientAnchor.BottomRowOffset;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.BottomRowOffset = value;
            }
        }
        /// <summary>
        /// Represents the Height of the shape.
        /// </summary>
        public override int Height
        {
            get
            {
                return this.m_shapeExt.ClientAnchor.Height;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.Height = value;
            }
        }
        /// <summary>
        /// Represents the Left position of the shape.
        /// </summary>
        public override int Left
        {
            // TODO: Implement this property to calculate the 
            // TODO: image position form Left Corner of the sheet.
            get
            {
                return this.m_shapeExt.ClientAnchor.Left;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.Left = value;
            }
        }
        /// <summary>
        /// Represents the Left position of the shape.
        /// </summary>
        public override int LeftColumn
        {
            get
            {
                return this.m_shapeExt.ClientAnchor.LeftColumn + 1;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.LeftColumn = value - 1;
            }
        }
        /// <summary>
        /// Represents the Left column offset of the shape.
        /// </summary>
        public override int LeftColumnOffset
        {
            get
            {
                return this.m_shapeExt.ClientAnchor.LeftColumnOffset;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.LeftColumnOffset = value;
            }
        }
        /// <summary>
        /// Represents the RightColumn of the Shape.
        /// </summary>
        public override int RightColumn
        {
            get
            {
                return this.m_shapeExt.ClientAnchor.RightColumn + 1;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.RightColumn = value - 1;
            }
        }
        /// <summary>
        /// Represent the Right Column Offset of the shape.
        /// </summary>
        public override int RightColumnOffset
        {
            get
            {
                return this.m_shapeExt.ClientAnchor.RightColumnOffset;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.RightColumnOffset = value;
            }
        }
        /// <summary>
        /// Represents the Top position of the shape.
        /// </summary>
        public override int Top
        {
            // TODO: Implement this property to calculate the 
            // TODO: image position form Left Corner of the sheet.
            get
            {
                return this.m_shapeExt.ClientAnchor.Top;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.Top = value;
            }
        }
        /// <summary>
        /// Represents the Top row of the shape.
        /// </summary>
        public override int TopRow
        {
            get
            {
                return this.m_shapeExt.ClientAnchor.TopRow + 1;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.TopRow = value - 1;
            }
        }
        /// <summary>
        /// Represents of the top row offset of the shape.
        /// </summary>
        public override int TopRowOffset
        {
            get
            {
                return this.m_shapeExt.ClientAnchor.TopRowOffset;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.TopRowOffset = value;
            }
        }
        /// <summary>
        /// Represents the Shape width.
        /// </summary>
        public override int Width
        {
            get
            {
                return this.m_shapeExt.ClientAnchor.Width;
            }
            set
            {
                this.m_shapeExt.ClientAnchor.Width = value;
            }
        }
        /// <summary>
        /// Represents the Shape Rotation.
        /// </summary>
        public override int ShapeRotation
        {
            get
            {
                return base.ShapeRotation;
            }
            set
            {
                base.ShapeRotation = value;
            }
        }
        /// <summary>
        /// Represents the fill.
        /// </summary>
        public override IFill Fill
        {
            get
            {
                return m_shapeExt.Fill;
            }
        }
        /// <summary>
        /// Represents the Line Format.
        /// </summary>
        public override IShapeLineFormat Line
        {
            get
            {
                return m_shapeExt.Line;
            }
        }
        /// <summary>
        /// Indicates whether the shape is hidden.
        /// </summary>
        public bool IsHidden
        {
            get
            {
                return this.m_shapeExt.IsHidden;
            }
            set
            {
                this.m_shapeExt.IsHidden= value;
            }
        }
        /// <summary>
        /// Represents the shape title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.m_shapeExt.Title;
            }
            set
            {
                this.m_shapeExt.Title = value;
            }
        }
        /// <summary>
        /// Gets or set a value indicating whether shape can move with cell
        /// </summary>
        public override bool IsMoveWithCell
        {
            get
            {
                return m_isMoveWithCell;
            }
            set
            {
                m_isMoveWithCell = value;
                SetPlacementValue();
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether shape can size with cell.
        /// </summary>
        public override bool IsSizeWithCell
        {
            get
            {
                return m_isSizeWithCell;
            }
            set
            {
                m_isSizeWithCell = value;
                SetPlacementValue();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Instantiates the ShapeExt with the Shape Type.
        /// </summary>
        /// <param name="type">Type of the AutoShape.</param>
        /// <param name="sheetImpl">Represents the Worksheet object.</param>
        internal void CreateShape(AutoShapeType type, WorksheetImpl sheetImpl)
        {
            m_shapeExt = new ShapeImplExt(type, sheetImpl);
        }
        /// <summary>
        /// Updates the shape id.
        /// </summary>
        /// <param name="shapeId"></param>
        internal void SetShapeID(int shapeId)
        {
            this.m_shapeExt.ShapeID = shapeId;
        }
        /// <summary>
        /// Updates the Placement Value for the shape.
        /// </summary>
        private void SetPlacementValue()
        {
            if (this.m_isMoveWithCell && this.m_isSizeWithCell)
                this.m_shapeExt.ClientAnchor.Placement = PlacementType.MoveAndSize;
            if (this.m_isMoveWithCell && !this.m_isSizeWithCell)
                this.m_shapeExt.ClientAnchor.Placement = PlacementType.Move;
            if (!this.m_isMoveWithCell && !this.m_isSizeWithCell)
                this.m_shapeExt.ClientAnchor.Placement = PlacementType.FreeFloating;
        }
        #endregion
    }
}
