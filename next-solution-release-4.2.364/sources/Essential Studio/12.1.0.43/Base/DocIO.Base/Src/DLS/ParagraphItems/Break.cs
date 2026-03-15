#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region File using directives
using System;
using Syncfusion.DocIO.DLS;
using System.Windows;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.Layouting;
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
using System.Drawing;
#endif

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represent a break special char. Can be a page break, column break or line break.
    /// </summary>
    public class Break : ParagraphItem
#if !SILVERLIGHT && !WP
      ,ILeafWidget
#endif
    {
        #region Fields
        /// <summary>
        /// Type of break. 
        /// </summary>
        private BreakType m_breakType;
        /// <summary>
        /// Line break text.
        /// </summary>
        private WTextRange m_lineBreakText;
#if (!SILVERLIGHT && !WP) || WINRT
        internal HtmlToDocLayoutInfo m_htmlToDocLayoutInfo = new HtmlToDocLayoutInfo();
#endif
        #endregion

        #region Properties
#if (!SILVERLIGHT && !WP) || WINRT
        /// <summary>
        /// Gets Html to Doc layout info
        /// </summary>
        internal HtmlToDocLayoutInfo HtmlToDocLayoutInfo
        {
            get
            {
                return m_htmlToDocLayoutInfo;
            }
        }
#endif
    
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.Break;
            }
        }
        /// <summary>
        /// Gets the type of the break.
        /// </summary>
        /// <value>The type of the break.</value>
        public BreakType BreakType
        {
            get
            {
                return m_breakType;
            }
        }
        /// <summary>
        /// Gets/sets text range of line break.
        /// </summary>
        internal WTextRange TextRange
        {
            get
            {
                return m_lineBreakText;
            }
            set
            {
                m_lineBreakText = value;
            }
        }
        /// <summary>
        /// Gets the end pos.
        /// </summary>
        /// <value>The end pos.</value>
        internal override int EndPos
        {
            get
            {
                return base.EndPos + m_lineBreakText.Text.Length;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Break"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public Break(IWordDocument doc)
            : this(doc, BreakType.LineBreak)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Break"/> class.
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="breakType">Break type</param>
        public Break(IWordDocument doc, BreakType breakType)
            : base((WordDocument)doc)
        {
            m_breakType = breakType;
            m_lineBreakText = new WTextRange(doc);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Attaches to paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="itemPos">The item pos.</param>
        internal override void Attach(WParagraph paragraph, int itemPos)
        {
            base.Attach(paragraph, itemPos);

            if (OwnerParagraph != null && m_breakType == BreakType.LineBreak)
            {
                OwnerParagraph.UpdateText(this, 0, m_lineBreakText.Text);
            }
        }
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal override void Detach()
        {
            base.Detach();

            if (OwnerParagraph != null && m_breakType == BreakType.LineBreak)
            {
                OwnerParagraph.UpdateText(this, m_lineBreakText.Text.Length, string.Empty);
            }
        }
        #endregion

        #region Implementation / xml
//#if !SILVERLIGHT
        /// <summary>
        /// Writes object data as xml attributes.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, ParagraphItemType.Break);
            writer.WriteValue(XDLSConstants.BreakTypeAttr, BreakType);
        }
        /// <summary>
        /// Reads object data from xml attributes.
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);
            m_breakType = (BreakType)reader.ReadEnum(XDLSConstants.BreakTypeAttr, typeof(BreakType));
        }
        /// <summary>
        /// Registers child objects in XDSL holder.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.TextRangeTag, m_lineBreakText);
        }
//#endif
        #endregion

        #region Implementation / layout
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            switch (m_breakType)
            {
                case BreakType.PageBreak:
                    m_layoutInfo = new ParagraphLayoutInfo(ChildrenLayoutDirection.Vertical, false);
                    m_layoutInfo.IsPageBreakItem = true;
                    break;

                case BreakType.LineBreak:
                    m_layoutInfo = new ParagraphLayoutInfo(ChildrenLayoutDirection.Vertical, false);
                    m_layoutInfo.IsLineBreak = true;
                    break;

                case BreakType.ColumnBreak:
                    m_layoutInfo = new ParagraphLayoutInfo(ChildrenLayoutDirection.Vertical, false);
                    m_layoutInfo.IsPageBreakItem = true;
                    break;

                default:
                    break;
            }
            if (this.TextRange.CharacterFormat.Hidden)//Set the IsSkip property true when characterformat contains hidden is true
                m_layoutInfo.IsSkip = true;
        }
        /// <summary>
        /// Measures self size.
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns></returns>
        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            SizeF size = new SizeF();
            WParagraph paragraph = this.OwnerParagraph;
            if (Owner is SDTInlineContent)
                paragraph = GetOwnerParagraph();
            ParagraphItemCollection paraItems = paragraph.Items;
            if (paragraph.m_bHasSDTInlineItem)
                paraItems = paragraph.GetParagraphItems();
            WTextRange txtRange = null;
            int index = paraItems.IndexOf(this);

            if (index > 0 && (paraItems[index - 1] is Break || paraItems.Count == index + 1))
            {
                Break prevBreak = paraItems[index - 1] as Break;
                if ((prevBreak != null && prevBreak.BreakType == BreakType.LineBreak) || paraItems.Count == index + 1)
                {
                    for (int i = index; i >= 0; i--)
                    {
                        if (paraItems[i] is WTextRange)
                        {
                            txtRange = paraItems[i] as WTextRange;
                            break;
                        }
                    }
                    if (txtRange != null)
                    {
                        size = dc.MeasureTextRange(txtRange, ".");
                    }
                    else
                    {
                        size.Height = dc.MeasureString(" ", this.TextRange.CharacterFormat.Font, null).Height;
                    }
                }
            }
            //Calculating height for the line break to layout
            if ((this is Break) && (this as Break).BreakType == BreakType.LineBreak && index < paraItems.Count - 1 && (index == 0 || (paraItems[index - 1] is Break) || paraItems[index + 1] is ShapeObject))
            {
                size.Height = dc.MeasureString(" ", this.TextRange.CharacterFormat.Font, null).Height;
            }
            if ((this is Break) && (this as Break).BreakType != BreakType.LineBreak)
                return SizeF.Empty;
            return size;
        }
#endif
        #endregion
    }
}
