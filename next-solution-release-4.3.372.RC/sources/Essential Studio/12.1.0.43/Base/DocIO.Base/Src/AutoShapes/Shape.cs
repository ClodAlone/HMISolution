#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
using System.IO;
using System;
using Syncfusion.DocIO;
using System.Collections;
#if WINRT || WP
#else
using System.Drawing;
#endif
#if !(SILVERLIGHT || WP)
using Syncfusion.Layouting;
using Syncfusion.DocIO.Rendering;
#endif
namespace Syncfusion.DocIO.DLS
{
    public partial class Shape : ParagraphItem, IEntity
                               #if !SILVERLIGHT && !WP
                               , ILeafWidget
                               #endif
    {
        #region Commented Fields
        //private bool m_HorizontalFlip;
        //private Hyperlink m_hyperlink;
        //private Adjustments m_Adjustments;
        //private BackgroundStyle m_BackgroundStyle;
        //private Callout m_Callout;
        //private Chart m_chart;
        //private GlowFormat m_GlowFormat;
        //private GroupShapes m_GroupShapes;
        //private bool m_HasChart;
        //private bool m_HasSmartArt;
        //private LinkFormat m_LinkFormat;
        //private List<ShapeNodes> m_ShapeNodes;
        //private OLEFormat m_OLEFormat;
        //private Shape m_OwnerShape;
        //private ReflectionFormat m_ReflectionFormat;
        //private float m_Rotation;
        //private ShadowFormat m_ShadowFormat;
        //private ShapeStyle m_ShapeStyle;
        //private SmartArt m_SmartArt;
        //private SoftEdgeFormat m_SoftEdgeFormat;
        //private TextEffect m_TextEffect;
        //private TextFrame2 m_TextFrame2;
        //private ThreeDFormat m_ThreeDFormat;
        //private bool m_VerticalFlip;
        //private Vertices m_Vertices;
        //private CanvasShapes m_CanvasItems;
        //private Script m_Script;
        //private float m_Top;
        //private RelativeHorizontalPosition m_RelativeHorizontalPosition;
        //private RelativeSize m_RelativeHorizontalSize;
        //private RelativeVerticalPosition m_RelativeVerticalPosition;
        //private RelativeSize m_RelativeVerticalSize ;
        //private float m_TopRelative;
        //private float m_WidthRelative;
        //private float m_HeightRelative;
        //private float m_LeftRelative;
        //private bool m_isChild;
        //private FillFormat m_FillFormat;
        //private LineFormat m_LineFormat;
        //private float m_Left;
        //private PictureFormat m_PictureFormat;
        #endregion
        #region fields
        //private WCharacterFormat m_charFormat;
        private string m_AlternateText;
        //private WordDocument m_Document;
        private AutoShapeType m_AutoShapeType;
        private long m_ID;
        //private bool m_LockAspectRatio;
        private string m_Name;
        //private Entity m_Owner;
        private WTextBody m_TextBody;
        private string m_Title;
        //private bool m_Visible;
        private bool m_isHeader;
        //private WParagraph m_OwnerParagraph; //Anchor - range
        private int m_ZOrderPosition = int.MaxValue;
        private bool m_bLayoutInTableCell = true;
        private bool m_LockAnchor;
        private HorizontalOrigin m_HorizontalOrigin = HorizontalOrigin.Margin;
        private VerticalOrigin m_VerticalOrigin = VerticalOrigin.Margin;
        private WrapFormat m_WrapFormat;
        private bool m_isCloned;
        internal Dictionary<string, Stream> m_docxProps = new Dictionary<string, Stream>();
        private bool m_isBelowText = false;
        private float m_Height;
        private float m_Width;
        private float m_widthScale = 100f;
        private float m_heightScale = 100f;
        private ShapeHorizontalAlignment m_horAlignment = ShapeHorizontalAlignment.None;
        private ShapeVerticalAlignment m_vertAlignment = ShapeVerticalAlignment.None;
        private ShapePosition m_shapePosition = ShapePosition.Static;
        private float m_horizPosition;
        private float m_vertPosition;
        private FillFormat m_FillFormat;
        private LineFormat m_LineFormat;
        internal bool Is2007Shape;
        List<string> m_styleProps;
        public string ShapeTypeID;
        private Dictionary<string, DictionaryEntry> m_relations;
        private TextFrame m_TextFrame;
        private string m_Adjustments;
        private bool m_IsHorizontalRule;
        private double m_arcSize;
        private bool m_UseStandardColorHR;
        private bool m_UseNoShadeHR;
        private bool m_fliphorizantal;
        private bool m_flipvertical;
        //private bool m_IsSimplePos;
        #endregion
        #region Properties
        internal bool UseStandardColorHR
        {
            get { return m_UseStandardColorHR; }
            set { m_UseStandardColorHR = value; }
        }
        internal bool UseNoShadeHR
        {
            get { return m_UseNoShadeHR; }
            set { m_UseNoShadeHR = value; }
        }
        internal double ArcSize
        {
            get { return m_arcSize; }
            set { m_arcSize = value; }
        }
        internal bool IsHorizontalRule
        {
            get { return m_IsHorizontalRule; }
            set { m_IsHorizontalRule = value; }
        }
        internal string Adjustments
        {
            get { return m_Adjustments; }
            set { m_Adjustments = value; }
        }
        public TextFrame TextFrame
        {
            get
            {
                if (m_TextFrame == null)
                    m_TextFrame = new TextFrame(this);
                return m_TextFrame;
            }
            set { m_TextFrame = value; }
        }


        /// <summary>
        /// Gets the docx style properties.
        /// </summary>
        /// <value>The docx style props.</value>
        internal List<String> DocxStyleProps
        {
            get
            {
                if (m_styleProps == null)
                    m_styleProps = new List<String>();
                return m_styleProps;
            }
        }
        Dictionary<string, Stream> m_docx2007Props;
        internal Dictionary<string, Stream> Docx2007Props
        {
            get
            {
                if (m_docx2007Props == null)
                    m_docx2007Props = new Dictionary<string, Stream>();
                return m_docx2007Props;
            }
            set
            {
                m_docx2007Props = value;
            }
        }


        private Dictionary<string, ImageRecord> m_imageRelations;
        private Dictionary<string, string> m_shapeGuide;

        /// <summary>
        /// Gets the image relations.
        /// </summary>
        /// <value>The image relations.</value>
        internal Dictionary<string, ImageRecord> ImageRelations
        {
            get
            {
                if (m_imageRelations == null)
                {
                    m_imageRelations = new Dictionary<string, ImageRecord>();
                }
                return m_imageRelations;
            }
        }
        /// <summary>
        /// Gets the shape guide
        /// </summary>
        /// <value>The shape guide.</value>
        internal Dictionary<string, string> ShapeGuide
        {
            get
            {
                if (m_shapeGuide == null)
                {
                    m_shapeGuide = new Dictionary<string, string>();
                }
                return m_shapeGuide;
            }
        }
        /// <summary>
        /// Gets the relations.
        /// </summary>
        /// <value>The relations.</value>
        internal Dictionary<string, DictionaryEntry> Relations
        {
            get
            {
                if (m_relations == null)
                {
                    m_relations = new Dictionary<string, DictionaryEntry>();
                }
                return m_relations;
            }
        }
        //internal bool IsSimplePos
        //{
        //    get { return m_IsSimplePos; }
        //    set { m_IsSimplePos = value; }
        //}
        public HorizontalOrigin HorizontalOrigin
        {
            get
            {
                return m_HorizontalOrigin;
            }
            set
            {
                m_HorizontalOrigin = value;
            }
        }

        public ShapeHorizontalAlignment HorizontalAlignment
        {
            get { return m_horAlignment; }
            set { m_horAlignment = value; }
        }

        public float HorizontalPosition
        {
            get { return m_horizPosition; }
            set { m_horizPosition = value; }
        }

        public VerticalOrigin VerticalOrigin
        {
            get { return m_VerticalOrigin; }
            set { m_VerticalOrigin = value; }
        }

        public ShapeVerticalAlignment VerticalAlignment
        {
            get { return m_vertAlignment; }
            set { m_vertAlignment = value; }
        }

        public float VerticalPosition
        {
            get { return m_vertPosition; }
            set { m_vertPosition = value; }
        }

        public bool IsBelowText
        {
            get { return m_isBelowText; }
            set { m_isBelowText = value; }
        }
        /// <summary>
        /// Gets / sets picture height.
        /// </summary>
        public float Height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
            }
        }
        /// <summary>
        /// Gets / sets picture width.
        /// </summary>
        public float Width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
            }
        }
        /// <summary>
        /// Gets / sets picture height scale factor in percent.
        /// </summary>
        public float HeightScale
        {
            get
            {
                return m_heightScale;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Scale factor must be greater than 0");
                }
                m_heightScale = value;
            }
        }
        /// <summary>
        /// Gets / sets picture width scale factor in percent.
        /// </summary>
        public float WidthScale
        {
            get
            {
                return m_widthScale;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Scale factor must be greater than 0");
                }
                m_widthScale = value;
            }
        }
        /// <summary>
        /// Returns or sets the alternative text associated with a shape
        /// </summary>
        public string AlternativeText
        {
            get { return m_AlternateText; }
            set { m_AlternateText = value; }
        }
        internal bool LayoutInCell
        {
            get
            {
                return m_bLayoutInTableCell;
            }
            set
            {
                m_bLayoutInTableCell = value;
            }
        }
        internal Dictionary<string, Stream> DocxProps
        {
            get
            {
                if (m_docxProps == null)
                {
                    m_docxProps = new Dictionary<string, Stream>();
                }
                return m_docxProps;
            }
        }
        //public WParagraph OwnerParagraph
        //{
        //    get { return m_OwnerParagraph; }
        //}

        //public WordDocument WordDocument
        //{
        //    get { return m_Document; }
        //    set { m_Document = value; }
        //}
        /// <summary>
        /// Returns or sets the shape type for the specified Shape object, which must represent an AutoShape other than a line or freeform drawing.
        /// </summary>
        public AutoShapeType AutoShapeType
        {
            get { return m_AutoShapeType; }
            internal set { m_AutoShapeType = value; }
        }

        internal long ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        //public bool LockAspectRatio
        //{
        //    get { return m_LockAspectRatio; }
        //    set { m_LockAspectRatio = value; }
        //}

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        //public Entity Owner
        //{
        //    get { return m_Owner; }
        //    set { m_Owner = value; }
        //}


        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        //public bool Visible
        //{
        //    get { return m_Visible; }
        //    set { m_Visible = value; }
        //}
        internal int ZOrderPosition
        {
            get { return m_ZOrderPosition; }
            set { m_ZOrderPosition = value; }
        }

        public bool LockAnchor
        {
            get { return m_LockAnchor; }
            set { m_LockAnchor = value; }
        }

        public WTextBody TextBody
        {
            get
            {
                if (m_TextBody == null)
                    m_TextBody = new WTextBody(this.Document, this);
                return m_TextBody;
            }
            set { m_TextBody = value; }
        }
        public WrapFormat WrapFormat
        {
            get
            {
                if (m_WrapFormat == null)
                    m_WrapFormat = new WrapFormat();
                return m_WrapFormat;
            }
            set { m_WrapFormat = value; }
        }
        public LineFormat LineFormat
        {
            get
            {
                if (m_LineFormat == null)
                    m_LineFormat = new LineFormat(this);
                return m_LineFormat;
            }
            set { m_LineFormat = value; }
        }
        public FillFormat FillFormat
        {
            get
            {
                if (m_FillFormat == null)
                    m_FillFormat = new FillFormat(this);
                return m_FillFormat;
            }
            set { m_FillFormat = value; }
        }
        ///// <summary>
        ///// Gets  character format( font properties ).
        ///// </summary>
        //public WCharacterFormat CharacterFormat
        //{
        //    get
        //    {
        //        return m_charFormat;
        //    }
        //    internal set
        //    {
        //        m_charFormat = value;
        //    }
        //}
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.AutoShape;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [flip horizantal].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [flip horizantal]; otherwise, <c>false</c>.
        /// </value>
        internal bool FlipHorizantal
        {
            get { return m_fliphorizantal; }
            set { m_fliphorizantal = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [flip vertical].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [flip vertical]; otherwise, <c>false</c>.
        /// </value>
        internal bool FlipVertical
        {
            get { return m_flipvertical; }
            set { m_flipvertical = value; }
        }
        #endregion
        #region CommentedPropertes
        //        public BackgroundStyle BackgroundStyle
        //        {
        //            get { return m_BackgroundStyle; }
        //            set { m_BackgroundStyle = value; }
        //        }

        //        internal Callout Callout
        //        {
        //            get { return m_Callout; }
        //            set { m_Callout = value; }
        //        }

        //        internal Chart Chart
        //        {
        //            get { return m_chart; }
        //            set { Chart = value; }
        //        }
        //        public bool IsChild
        //        {
        //            get { return m_isChild; }
        //            set { m_isChild = value; }
        //        }
        //        internal GlowFormat GlowFormat
        //        {
        //            get { return m_GlowFormat; }
        //            set { m_GlowFormat = value; }
        //        }
        //        internal GroupShapes GroupShapes
        //        {
        //            get { return m_GroupShapes; }
        //            set { m_GroupShapes = value; }
        //        }
        //        public bool HasChart
        //        {
        //            get { return m_HasChart; }
        //            set { m_HasChart = value; }
        //        }
        //        public bool HasSmartArt
        //        {
        //            get { return m_HasSmartArt; }
        //            set { m_HasSmartArt = value; }
        //        }public bool HorizontalFlip
        //        {
        //            get { return m_HorizontalFlip; }
        //            set { m_HorizontalFlip = value; }
        //        }
        //        internal Hyperlink Hyperlink
        //        {
        //            get { return m_hyperlink; }
        //            set { m_hyperlink = value; }
        //        }
        //        internal LinkFormat LinkFormat
        //        {
        //            get { return m_LinkFormat; }
        //            set { m_LinkFormat = value; }
        //        } public float Left
        //        {
        //            get { return m_Left; }
        //            set { m_Left = value; }
        //        }
        //internal List<ShapeNodes> ShapeNodes
        //        {
        //            get { return m_ShapeNodes; }
        //            set { m_ShapeNodes = value; }
        //        }internal OLEFormat OLEFormat
        //        {
        //            get { return m_OLEFormat; }
        //            set { m_OLEFormat = value; }
        //        }
        //        public Shape OwnerShape
        //        {
        //            get { return m_OwnerShape; }
        //            set { m_OwnerShape = value; }
        //        }
        //        public PictureFormat PictureFormat
        //        {
        //            get { return m_PictureFormat; }
        //            set { m_PictureFormat = value; }
        //        }public float Top
        //        {
        //            get { return m_Top; }
        //            set { m_Top = value; }
        //        }
        //        public bool VerticalFlip
        //        {
        //            get { return m_VerticalFlip; }
        //            set { m_VerticalFlip = value; }
        //        }
        //        internal Vertices Vertices
        //        {
        //            get { return m_Vertices; }
        //            set { m_Vertices = value; }
        //        }
        //internal CanvasShapes CanvasItems
        //        {
        //            get { return m_CanvasItems; }
        //            set { m_CanvasItems = value; }
        //        }
        //        public float HeightRelative
        //        {
        //            get { return m_HeightRelative; }
        //            set { m_HeightRelative = value; }
        //        }
        //        public bool LayoutInCell
        //        {
        //            get { return m_LayoutInCell; }
        //            set { m_LayoutInCell = value; }
        //        }
        //        public float LeftRelative
        //        {
        //            get { return m_LeftRelative; }
        //            set { m_LeftRelative = value; }
        //        } public RelativeHorizontalPosition RelativeHorizontalPosition
        //        {
        //            get { return m_RelativeHorizontalPosition; }
        //            set { m_RelativeHorizontalPosition = value; }
        //        }
        //        public RelativeSize RelativeHorizontalSize
        //        {
        //            get { return m_RelativeHorizontalSize; }
        //            set { m_RelativeHorizontalSize = value; }
        //        }
        //        public RelativeVerticalPosition RelativeVerticalPosition
        //        {
        //            get { return m_RelativeVerticalPosition; }
        //            set { m_RelativeVerticalPosition = value; }
        //        }
        //        public RelativeSize RelativeVerticalSize
        //        {
        //            get { return m_RelativeVerticalSize; }
        //            set { m_RelativeVerticalSize = value; }
        //        }
        //        internal Script Script
        //        {
        //            get { return m_Script; }
        //            set { m_Script = value; }
        //        }
        //        public float TopRelative
        //        {
        //            get { return m_TopRelative; }
        //            set { m_TopRelative = value; }
        //        }
        //        public float WidthRelative
        //        {
        //            get { return m_WidthRelative; }
        //            set { m_WidthRelative = value; }
        //        }
        //        internal Adjustments Adjustments
        //        {
        //            get { return m_Adjustments; }
        //            set { m_Adjustments = value; }
        //        }
        //internal ReflectionFormat ReflectionFormat
        //{
        //    get { return m_ReflectionFormat; }
        //    set { m_ReflectionFormat = value; }
        //}
        //public float Rotation
        //{
        //    get { return m_Rotation; }
        //    set { m_Rotation = value; }
        //}
        //internal ShadowFormat ShadowFormat
        //{
        //    get { return m_ShadowFormat; }
        //    set { m_ShadowFormat = value; }
        //}
        //internal ShapeStyle ShapeStyle
        //{
        //    get { return m_ShapeStyle; }
        //    set { m_ShapeStyle = value; }
        //}
        //internal SmartArt SmartArt
        //{
        //    get { return m_SmartArt; }
        //    set { m_SmartArt = value; }
        //}
        //internal SoftEdgeFormat SoftEdgeFormat
        //{
        //    get { return m_SoftEdgeFormat; }
        //    set { m_SoftEdgeFormat = value; }
        //}
        //internal TextEffect TextEffect
        //{
        //    get { return m_TextEffect; }
        //    set { m_TextEffect = value; }
        //}
        ////internal TextFrame TextFrame
        ////{
        ////    get { return m_TextFrame; }
        ////    set { m_TextFrame = value; }
        ////}
        //internal TextFrame2 TextFrame2
        //{
        //    get { return m_TextFrame2; }
        //    set { m_TextFrame2 = value; }
        //}
        //internal ThreeDFormat ThreeDFormat
        //{
        //    get { return m_ThreeDFormat; }
        //    set { m_ThreeDFormat = value; }
        //}
        #endregion
        internal Shape(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_charFormat = new WCharacterFormat(Document);
            m_charFormat.SetOwner(this);

            WrapFormat.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
            FillFormat.Color = Color.White;
            FillFormat.Fill = true;
            FillFormat.FillType = FillType.FillSolid;

            LineFormat.Color = Color.Black;
            LineFormat.DashStyle = LineDashing.Solid;
            LineFormat.Line = true;
            LineFormat.Style = LineStyle.Single;
            LineFormat.Transparency = 0f;
            LineFormat.Weight = 1;

            m_HorizontalOrigin = HorizontalOrigin.Column;
            m_VerticalOrigin = VerticalOrigin.Paragraph;
            
            m_horAlignment = ShapeHorizontalAlignment.None;
            m_vertAlignment = ShapeVerticalAlignment.None;
            TextFrame.TextVerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Top;
        }
        public Shape(IWordDocument doc, AutoShapeType autoShapeType)
            :this((WordDocument)doc)
        {
            this.m_AutoShapeType = autoShapeType;
        }
        /// <summary>
        /// Attaches to paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="itemPos">The item pos.</param>
        internal override void Attach(WParagraph paragraph, int itemPos)
        {
            base.Attach(paragraph, itemPos);
            if (!DeepDetached)
            {
                Document.AutoShapeCollection.Add(this);
                m_isCloned = false;
            }
            else
            {
                m_isCloned = true;
            }
        }
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal override void Detach()
        {
            base.Detach();

            if (!DeepDetached)
            {
                Document.AutoShapeCollection.Remove(this);
            }
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        internal override void CloneCommit()
        {
            if (m_isCloned)
            {
                Document.AutoShapeCollection.Add(this);
                m_isCloned = false;
            }
            this.TextBody.CloneCommit();
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            Shape shape = (Shape)base.CloneImpl();
            shape.m_isCloned = true;
            return shape;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            base.CloneRelationsTo(doc, nextOwner);
        }
        /// <summary>
        /// Sets the character format.
        /// </summary>
        /// <param name="charFormat">The character format.</param>
        public void ApplyCharacterFormat(WCharacterFormat charFormat)
        {
            if (charFormat != null)
            {
                m_charFormat = charFormat.CloneInt() as WCharacterFormat;
            }
        }
#if !SILVERLIGHT && !WP

        #region IWidget/ILeafWidget implement
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            dc.DrawShape(this, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            return new SizeF(this.Width, this.Height);
        }
        #endregion
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new ShapeLayoutInfo(ChildrenLayoutDirection.Horizontal);
            WParagraph ownerParagraph = this.OwnerParagraph;
            if (this.Owner is SDTInlineContent)
                ownerParagraph = GetOwnerParagraph();
            if (ownerParagraph.IsInCell && ((ownerParagraph as IWidget).LayoutInfo.IsClipped))
                m_layoutInfo.IsClipped = true;
            if (this.TextFrame.TextDirection != TextDirection.Horizontal)
                m_layoutInfo.IsVerticalText = true;
            if (this.WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                m_layoutInfo.IsSkipBottomAlign = true;
            if (this.ParaItemCharFormat.HasValue(WCharacterFormat.HiddenKey))
                m_layoutInfo.IsSkip = true;
        }
#endif
    }
    #if !SILVERLIGHT && !WP
    internal class ShapeLayoutInfo : LayoutInfo
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private RectangleF m_textLayoutingBounds;
        #endregion
        #region Properties
        /// <summary>
        /// Get/Set Footnote/Endnote TextBody  height
        /// </summary>
        internal RectangleF TextLayoutingBounds
        {
            get
            {
                return m_textLayoutingBounds;
            }
            set
            {
                m_textLayoutingBounds = value;
            }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutParagraphInfo"/> class.
        /// </summary>
        /// <param name="childLayoutDirection">The child layout direction.</param>
        internal ShapeLayoutInfo(ChildrenLayoutDirection childLayoutDirection)
            : base(childLayoutDirection)
        { }
        #endregion
    }
#endif
}