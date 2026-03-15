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

#region file using directives
using System;
using System.Collections.Generic;
using System.Text;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
    /// <summary>
    /// Represents rich text string in the workbook.
    /// </summary>
    public class ChartRichTextString
      : CommonWrapper
      , IChartRichTextString
      , IOptimizedUpdate
    {
        #region Class members
        /// <summary>
        /// Low level text object.
        /// </summary>
        private string m_text;
        /// <summary>
        /// Parent workbook.
        /// </summary>
        protected WorkbookImpl m_book;
        /// <summary>
        /// Indicates whether string is read-only.
        /// </summary>
        private bool m_bIsReadOnly;
        /// <summary>
        /// Represents the parent object
        /// </summary>
        private object m_parent;
        /// <summary>
        /// Represents the chart text area
        /// </summary>
        private ChartTextAreaImpl m_textArea;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the formatting runs of rich-text
        /// </summary>
        public ChartAlrunsRecord.TRuns[] FormattingRuns
        {
            get
            {
                if (TextArea != null && TextArea.ChartAlRuns != null)
                    return TextArea.ChartAlRuns.Runs;

                return null;
            }
        }
        /// <summary>
        /// Gets or sets the chart text area
        /// </summary>
        private ChartTextAreaImpl TextArea
        {
            get
            {
                return m_textArea;
            }
            set
            {
                m_textArea = value;
            }
        }
        /// <summary>
        /// Gets the text
        /// </summary>
        public string Text
        {
            get
            {
                Charts.ChartTextAreaImpl textArea = (Parent as Implementation.Charts.ChartTextAreaImpl);
                if (textArea != null)
                    m_text = textArea.Text;

                return m_text;
            }
        }
        #endregion

        #region IParentApplication members
        /// <summary>
        /// Returns parent object. Read-only.
        /// </summary>
        public object Parent
        {
            get
            {
                return m_parent;
            }
        }
        /// <summary>
        /// Returns parent application object. Read-only.
        /// </summary>
        public IApplication Application
        {
            get
            {
                return m_book.Application;
            }
        }
        #endregion
            
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes new instance of the RichTextString.
        /// </summary>
        /// <param name="application">Application object for the RichTextString.</param>
        /// <param name="parent">Parent object for the RichTextString.</param>
        public ChartRichTextString( IApplication application, object parent )
          //: base() application, parent )
        {
          if( parent == null )
            throw new ArgumentNullException( "parent" );

          m_parent = parent;
          SetParents();

          if ((m_parent as ChartTextAreaImpl) != null)
              m_textArea = (m_parent as ChartTextAreaImpl);
        }
        /// <summary>
        /// Initializes new instance of the RichTextString.
        /// </summary>
        /// <param name="application">Application object for the RichTextString.</param>
        /// <param name="parent">Parent object for the RichTextString.</param>
        /// <param name="isReadOnly">Indicates whether string is read-only.</param>
        public ChartRichTextString( IApplication application, object parent, bool isReadOnly )
          : this( application, parent, isReadOnly, false )
        {
        }
        /// <summary>
        /// Initializes new instance of the RichTextString.
        /// </summary>
        /// <param name="application">Application object for the RichTextString.</param>
        /// <param name="parent">Parent object for the RichTextString.</param>
        /// <param name="isReadOnly">Indicates whether string is read-only.</param>
        /// <param name="bCreateText">Indicates whether to create inner TextWithFormat.</param>
        public ChartRichTextString( IApplication application, object parent, bool isReadOnly, bool bCreateText )
          : this( application, parent )
        {
          m_bIsReadOnly = isReadOnly;

          if( bCreateText )
          {
            m_text = new TextWithFormat();
          }
        }
        /// <summary>
        /// Initializes new instance of the RichTextString.
        /// </summary>
        /// <param name="application">Application object for the RichTextString.</param>
        /// <param name="parent">Parent object for the RichTextString.</param>
        /// <param name="text">Text to wrap.</param>
        public ChartRichTextString( IApplication application, object parent, TextWithFormat text )
          : this( application, parent )
        {
          m_text = text;
        }
        /// <summary>
        /// Searches for all necessary parent objects.
        /// </summary>
        protected virtual void SetParents()
        {
          m_book = CommonObject.FindParent( m_parent, typeof( WorkbookImpl ) ) as WorkbookImpl;

          if( m_book == null )
            throw new ArgumentNullException( "Can't find parent workbook." );
        }
        #endregion

        #region Rich-text string methods
        /// <summary>
        /// Sets font for range of characters.
        /// </summary>
        /// <param name="iStartPos">First character of the range.</param>
        /// <param name="iEndPos">Last character of the range.</param>
        /// <param name="font">Font to set.</param>
        public void SetFont(int iStartPos, int iEndPos, IFont font)
        {
            BeginUpdate();

            ushort iFontIndex = (ushort)AddFont(font);
            TextArea = (Parent as Implementation.Charts.ChartTextAreaImpl);

            if (TextArea == null)
                throw new ArgumentNullException("textArea");

            if (TextArea.Text == null)
                throw new ArgumentNullException("Does not support rich-text for empty string");

            if (TextArea.Text != null && TextArea.Text.Length > 0)
            {
                if (TextArea.ChartAlRuns != null)
                {
                    List<ChartAlrunsRecord.TRuns> tRuns = new List<ChartAlrunsRecord.TRuns>(TextArea.ChartAlRuns.Runs);
                    bool isUpdated = false;

                    for (int i = 0; i < tRuns.Count; i++)
                    {
                        if ((tRuns[i].FirstCharIndex == iStartPos) || (isUpdated && tRuns[i].FirstCharIndex <= iEndPos))
                        {
                            tRuns[i].FontIndex = iFontIndex;
                            TextArea.ChartAlRuns.Runs = tRuns.ToArray();
                            isUpdated = true;
                        }
                    }

                    if (!isUpdated)
                    {
                        if (iStartPos > 0)
                        {
                            for (int i = 0; i < iStartPos; i++)
                            {
                                tRuns.Add(new ChartAlrunsRecord.TRuns((ushort)i, 0));
                                TextArea.ChartAlRuns.Runs = tRuns.ToArray();
                            }
                        }

                        for (int i = iStartPos; i <= iEndPos; i++)
                        {
                            tRuns.Add(new ChartAlrunsRecord.TRuns((ushort)i, (ushort)iFontIndex));
                            TextArea.ChartAlRuns.Runs = tRuns.ToArray();
                        }

                        for (int i = iEndPos + 1; i <= TextArea.Text.Length; i++)
                        {
                            tRuns.Add(new ChartAlrunsRecord.TRuns((ushort)i, 0));
                            TextArea.ChartAlRuns.Runs = tRuns.ToArray();
                        }
                    }

                    TextArea.m_chartText.IsAutoColor = false;
                }
            }

            EndUpdate();
        }
        /// <summary>
        /// Gets font for the specified formatting run.
        /// </summary>
        /// <param name="tRuns">Formatting run to return its font</param>
        public IFont GetFont(ChartAlrunsRecord.TRuns tRuns)
        {
            Collections.FontsCollection fonts = m_book.InnerFonts;
            IFont result = null;

            foreach(FontImpl font in fonts)
            {
                if (font.Index == tRuns.FontIndex)
                    result = font;
            }

            return result;
        }
        /// <summary>
        /// Adds font to all required collections..
        /// </summary>
        /// <param name="font">Font to add.</param>
        /// <returns>Font index in the collection.</returns>
        protected virtual int AddFont(IFont font)
        {
            IInternalFont fontInternal = (IInternalFont)font;

            FontImpl fontImpl = fontInternal.Font;
            fontImpl = m_book.InnerFonts.Add(fontImpl) as FontImpl;

            return fontImpl.Index;
        }
        #endregion
    }
}
