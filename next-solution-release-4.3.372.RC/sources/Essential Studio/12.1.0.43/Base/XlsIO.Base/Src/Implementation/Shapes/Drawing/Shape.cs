#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.IO;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization;

namespace Syncfusion.XlsIO.Drawing
{
    /// <summary>
    /// Extended the ShapeImp to add support for the Autoshape.
    /// </summary>
    internal class ShapeImplExt
    {
        #region Members
        private string m_textlink;
        private bool m_fLocksText;
        private bool m_fPublished;
        private ClientAnchor m_clientAnchor;
        private ExcelAutoShapeType m_shapeType;
        private AutoShapeType m_autoShapeType;
        private ShapeDrawingType m_shapeDrawingType;
        private int m_shapeID;
        private TextFrame m_textframe;
        private Dictionary<string, Stream> m_preservedElements;
        private string m_decription;
        private string m_name;
        private bool m_isHidden = false;
        private bool m_flipVertical;
        private bool m_flipHorizontal;
        private string m_title;
        private WorksheetImpl m_worksheet;
        private AnchorType m_anchorType;
        private string m_macro;
        private string m_text;
        private bool m_lockText;
        private bool m_published;
        private bool m_isCreated = false;
        private double m_shapeRotation;
        private ShapeFillImpl m_fill;
        private ShapeLineFormatImpl m_line;
        private RelationCollection m_relations;
        private PreservationLogger m_logger;
        #endregion

        #region Instantiate
        public ShapeImplExt(AutoShapeType autoShapeType, WorksheetImpl worksheetImpl)
        {
            this.m_worksheet = worksheetImpl;
            this.m_autoShapeType = autoShapeType;
            this.m_anchorType = AnchorType.TwoCell;
            this.m_logger = new PreservationLogger();
            CreateShapeType(autoShapeType);
        }

        private void CreateShapeType(AutoShapeType autoShapeType)
        {
            switch (autoShapeType)
            {
                case AutoShapeType.Line:
                case AutoShapeType.StraightConnector:
                case AutoShapeType.ElbowConnector:
                case AutoShapeType.BentConnector2:
                case AutoShapeType.BentConnector4:
                case AutoShapeType.BentConnector5:
                case AutoShapeType.CurvedConnector:
                case AutoShapeType.CurvedConnector2:
                case AutoShapeType.CurvedConnector4:
                case AutoShapeType.CurvedConnector5:
                    this.m_shapeType = ExcelAutoShapeType.cxnSp;
                    break;
                default:
                    this.m_shapeType = ExcelAutoShapeType.sp;
                    break;
            }
        }
        #endregion

        #region Properties
        internal double Rotation
        {
            get
            {
                return m_shapeRotation;
            }
            set
            {
                m_shapeRotation = value;
            }
        }
        internal WorksheetImpl Worksheet
        {
            get
            {
                return m_worksheet;
            }
            set
            {
                m_worksheet = value;
            }
        }
        internal string Description
        {
            get
            {
                return m_decription;
            }
            set
            {
                m_decription = value;
            }
        }
        internal Dictionary<string, Stream> PreservedElements
        {
            get
            {
                if (m_preservedElements == null)
                    m_preservedElements = new Dictionary<string, Stream>();
                return m_preservedElements;
            }
        }
        internal string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
            }
        }
        internal bool IsCreated
        {
            get
            {
                return m_isCreated;
            }
            set
            {
                m_isCreated = value;
            }
        }
        public TextFrame TextFrame
        {
            get
            {
                if (this.m_textframe == null)
                {
                    this.m_textframe = new TextFrame(this);
                }
                return this.m_textframe;
            }
        }
        public ClientAnchor ClientAnchor
        {
            get
            {
                if (this.m_clientAnchor == null)
                    this.m_clientAnchor = new ClientAnchor(this.m_worksheet);
                return this.m_clientAnchor;
            }
        }
        public ShapeFillImpl Fill
        {
            get
            {
                if (this.m_fill == null)
                    this.m_fill = new ShapeFillImpl(this.m_worksheet.AppImplementation, this.m_worksheet, ExcelFillType.SolidColor,this.m_logger);
                return m_fill;
            }
        }
        public ShapeLineFormatImpl Line
        {
            get
            {
                if (this.m_line == null)
                    this.m_line = new ShapeLineFormatImpl(this.m_worksheet.AppImplementation, this.m_worksheet,this.m_logger);
                return m_line;
            }
        }


        public int ShapeID
        {
            get
            {
                return m_shapeID;
            }
            set
            {
                m_shapeID = value;
            }
        }

        public ExcelAutoShapeType ShapeType
        {
            get
            {
                return m_shapeType;
            }
            set
            {
                m_shapeType = value;
            }
        }
        public AutoShapeType AutoShapeType
        {
            get
            {
                return this.m_autoShapeType;
            }
        }

        public string Macro
        {
            get { return this.m_macro; }
            set { this.m_macro = value; }
        }

        public string TextLink
        {
            get { return this.m_textlink; }
            set { this.m_textlink = value; }
        }

        public bool LocksText
        {
            get { return this.m_lockText; }
            set { this.m_lockText = value; }
        }

        public bool Published
        {
            get { return this.m_published; }
            set { this.m_published = value; }
        }
        public AnchorType AnchorType
        {
            get
            {
                return m_anchorType;
            }
            set
            {
                m_anchorType = value;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        public bool IsHidden
        {
            get
            {
                return m_isHidden;
            }
            set
            {
                m_isHidden = value;
            }
        }
        public RelationCollection Relations
        {
            get
            {
                if (m_relations == null)
                    m_relations = new RelationCollection();

                return m_relations;
            }
        }
        public PreservationLogger Logger
        {
            get { return m_logger; }
        }
        public bool FlipVertical
        {
            get
            {
                return m_flipVertical;
            }
            set
            {
                m_flipVertical = value;
            }
        }

        public bool FlipHorizontal
        {
            get
            {
                return m_flipHorizontal;
            }
            set
            {
                m_flipHorizontal = value;
            }
        }
        #endregion

        
    }
    internal class PreservationLogger
    {
        private PreservedFlag m_flag;


        internal PreservationLogger()
        {
            this.m_flag = 0L;
        }

        internal bool CheckFlag(PreservedFlag flag)
        {
            return ((this.m_flag & flag) != 0);
        }

        internal void SetFlag(PreservedFlag flag)
        {
            this.m_flag |= flag;
        }

        internal void ResetFlag()
        {
            this.m_flag = 0L;
        }

        internal bool GetPreservedItem(PreservedFlag flag)
        {
            switch (flag)
            {
                case PreservedFlag.Fill:
                    {
                        return this.CheckFlag(PreservedFlag.Fill);
                        break;
                    }
                case PreservedFlag.Line:
                    {
                        return this.CheckFlag(PreservedFlag.Line);
                        break;
                    }
                case PreservedFlag.RichText:
                    {
                        return this.CheckFlag(PreservedFlag.RichText);
                        break;
                    }
            }
            return false;
        }
    }

    internal enum PreservedFlag : int
    {
        Fill = 1,
        Line = 2,
        RichText = 4,
    }



}
