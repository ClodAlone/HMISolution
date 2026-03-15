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
using System.IO;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents TextConverter.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class TextConverter
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private StreamWriter m_writer;
    private string m_text = "";
    /// <summary>
    /// 
    /// </summary>
    private int m_curSectionIndex = 0;
    private bool m_bGetString = false;
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public TextConverter()
    {
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    public string GetText( Document document )
    {
      m_bGetString = true;
      
      foreach( Section section in document.Sections )
      {
        WriteParagraphs( section.Paragraphs, false );
        WriteSectionEnd( section );
      }
      m_bGetString = false;
      
      return m_text;
    }
    /// <summary>
    /// By means of StreamWriter writes WordDocument to TXT format.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    public void Write( StreamWriter writer, IDocument document )
    {
      m_writer = writer;

      WriteBody( document );
      //WriteHFBody( document );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="document"></param>
    public void Read( StreamReader reader, IDocument document )
    {
      string text = reader.ReadToEnd();
      string[] textlines = text.Split( "\n".ToCharArray() );

      if( document.LastParagraph == null )
      {
        if( document.LastSection == null )
        {
          document.EnsureMinimal();
        }
        else
        {
          document.LastSection.AddParagraph();
        }
      }

      foreach( string line in textlines )
      {
        line.Trim( "\r".ToCharArray() );
        document.LastParagraph.AppendText( line );
        document.LastSection.AddParagraph();
        if( line == "\r" )
        {
          document.LastSection.AddParagraph();
        }
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Writes document body.
    /// </summary>
    /// <param name="document"></param>
    protected void WriteBody( IDocument document )
    {
      foreach( Section section in document.Sections )
      {
        WriteParagraphs( section.Paragraphs, false );
        WriteSectionEnd( section );
      }
    }
    /// <summary>
    /// Writes document header/footer body.
    /// </summary>
    /// <param name="document"></param>
    protected void WriteHFBody( Document document )
    {
//      int secCount = m_wordDoc.Sections.Count;
//      
//      for( int i = 0; i < secCount; i++ )
//      {
//        
//        m_writer.WriteLine( "\r\n< " + i + " Section Headers Footers>" );
//        for( int j = 0; j < 6; j++ )
//        {
//          IParagraphCollection collection = m_wordDoc.Sections[i].HeadersFooters[ j ];
//        
//          if( collection.Count > 0)
//          {
//            m_writer.WriteLine( "<" + (HeaderType)j + ">" );
//            WriteParagraphs( collection, false );
//            m_writer.WriteLine( "</" + (HeaderType)j + ">" );
//          }
//        }
//        m_writer.WriteLine( "</ " + i + " Section Headers Footers>" );
//      }
    }
    /// <summary>
    /// Writes paragraphs.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="isTableBody"></param>
    protected void WriteParagraphs( IParagraphCollection collection, bool isTableBody )
    {
      foreach( IParagraph paragraph in collection )
      {
        WriteParagraph( paragraph );
      }
    }
    /// <summary>
    /// Writes paragraph.
    /// </summary>
    /// <param name="paragraph"></param>
    protected void WriteParagraph( IParagraph paragraph )
    {
      if( paragraph.ListFormat.CurrentListStyle != null
          && paragraph.ListFormat.CurrentListStyle.ListType == ListType.Bulleted )
      {
        m_writer.Write( "* " );
      }
      for( int i = 0, len = paragraph.ItemsCount; i < len; i++ )
      {
        IParagraphItem item = paragraph[ i ];
        ITextRange text = item as ITextRange;

        // For ITextRange items
        if( text != null )
        {
          WriteText( paragraph.CharacterFormat, text );
        }
        
        // For Tables
        else if( item is ITable )
        {
          WriteTable( item as ITable );
        }
      }

      if( m_bGetString )
      {
        m_text += "\r\n";
      }
      else
      {
        m_writer.WriteLine( "" );
      }
    }
    /// <summary>
    /// Writes table.
    /// </summary>
    /// <param name="table"></param>
    protected void WriteTable( ITable table )
    {
      foreach( TableRow row in table.Rows )
      {
        foreach( TableCell cell in row.Cells )
        {
          WriteParagraphs( cell.Paragraphs, true );
        }
      }
    }
    /// <summary>
    /// Writes end of section.
    /// </summary>
    /// <param name="section"></param>
    protected void WriteSectionEnd( ISection section )
    {
      if( m_bGetString )
      {
        m_text += "\r\n";
      }
      else
      {
        m_writer.WriteLine( "" );
      }
      m_curSectionIndex++;
    }
    /// <summary>
    /// Writes text.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="text"></param>
    protected void WriteText( CharacterFormat format, ITextRange text )
    {
      if( m_bGetString )
      {
        m_text += text.Text;
        
        if( text.CharacterFormat.LineBreak )
        {
          m_text += "\r\n";
        }
      }
      else
      {
        m_writer.Write( text.Text );
        
        if( text.CharacterFormat.LineBreak )
        {
          m_writer.WriteLine( "" );
        }
      }
    }
    #endregion

    
  }
}
