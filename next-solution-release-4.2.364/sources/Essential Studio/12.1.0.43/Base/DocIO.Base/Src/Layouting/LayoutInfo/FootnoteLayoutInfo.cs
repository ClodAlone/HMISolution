#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;

namespace Syncfusion.Layouting
{
    internal class FootnoteLayoutInfo : LayoutInfo
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private string m_footnoteID;
        /// <summary>
        /// 
        /// </summary>
        private WTextRange m_textRange;
        /// <summary>
        /// 
        /// </summary>
        private float m_height;
        /// <summary>
        /// Holds the height of the footnote
        /// </summary>
        private float m_endnoteheight;
        #endregion
        #region Properties
        /// <summary>
        /// Get/Set Footnote/Endnote TextBody  height
        /// </summary>
        internal float FootnoteHeight
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }
        /// <summary>
        /// Get/Set Endnote TextBody  height
        /// </summary>
        internal float Endnoteheight
        {
            get
            {
                return m_endnoteheight;
            }
            set
            {
                m_endnoteheight = value;
            }
        }
        /// <summary>
        /// Get/Set Footnote/Endnote ID.
        /// </summary>
        internal string FootnoteID
        {
            get
            {
                return m_footnoteID;
            }
            set
            {
                m_footnoteID = value;
            }
        }
        /// <summary>
        /// Get/Set the TextRange 
        /// </summary>
        internal WTextRange TextRange
        {
            get
            {
                return m_textRange;
            }
            set
            {
                m_textRange = value;
            }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutParagraphInfo"/> class.
        /// </summary>
        /// <param name="childLayoutDirection">The child layout direction.</param>
        internal FootnoteLayoutInfo(ChildrenLayoutDirection childLayoutDirection)
            : base(childLayoutDirection)
        { }
        #endregion
        #region Implementation
        /// Get Footnote ID based on number format
        /// </summary>
        /// <param name="footnote"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal string GetFootnoteID(WFootnote footnote, int id)
        {
            WSection section = GetBaseEntity(footnote) as WSection;
            string text = id.ToString();
            if (section != null)
            {
                FootEndNoteNumberFormat numberformat = section.PageSetup.FootnoteNumberFormat;
                //if the current item is endnote means get the end note number format
                if (footnote.FootnoteType == FootnoteType.Endnote)
                    numberformat = section.PageSetup.EndnoteNumberFormat;

                //Get foot note or end note ID based on Corresponding numberformat
                text = section.PageSetup.GetNumberFormatValue((byte)numberformat, id);
                if (footnote.CustomMarkerIsSymbol)
                {
                    text = Char.ConvertFromUtf32(footnote.SymbolCode);
                }
                else if (footnote.CustomMarker != string.Empty)
                {
                    text = footnote.CustomMarker;
                }
            }
            return text;
        }
        /// <summary>
        /// Get Base Entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        internal Entity GetBaseEntity(Entity entity)
        {
            Entity ent = entity;
            while (!(ent is WSection))
            {
                if (ent.Owner == null)
                    break;
                else
                    ent = ent.Owner as Entity;
            }
            return ent;
        }
        #endregion
    }
}
#endif