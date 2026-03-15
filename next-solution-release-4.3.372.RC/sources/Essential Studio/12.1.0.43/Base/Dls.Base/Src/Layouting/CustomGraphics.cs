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
using System.Drawing;
using System.Drawing.Drawing2D;
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Summary description for CustumGraphics.
  /// </summary>
  public class CustomGraphics
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private Graphics m_graphics;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    protected internal Graphics Graphics
    {
      get
      {
        //m_graphics.PageUnit = GraphicsUnit.Point;
        return m_graphics;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public CustomGraphics()
    {}
    /// <summary>
    /// 
    /// </summary>
    public CustomGraphics( Graphics graphics )
    {
      m_graphics = graphics;
      m_graphics.PageUnit = GraphicsUnit.Point;
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <returns></returns>
    public CustomGraphics Update( Graphics graphics )
    {
      m_graphics = graphics;
      m_graphics.PageUnit = GraphicsUnit.Point;
      return this;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public virtual Matrix ApplyTransform( Matrix matrix )
    {
      if( Graphics == null )
      {
        throw new InvalidOperationException();
      }

      Matrix oldMatrix = Graphics.Transform;
      if( matrix != null )
      {
        Graphics.MultiplyTransform( matrix );
      }
      return oldMatrix;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="matrix"></param>
    public virtual void ResetTransform( Matrix matrix )
    {
      if( Graphics == null )
      {
        throw new InvalidOperationException();
      }

      Graphics.Transform = matrix;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="measurer"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public int GetSplitIndexByOffset( string text, ITextMeasurable measurer, double offset, bool bSplitByChar )
    {
      /*/
      int resIndex = -1;
      int spaceIndex = -1;

      if( offset > 0 )
      {
        for( int i = 0; i < text.Length; i++ )
        {
          SizeF size = measurer.Measure( this, text.Substring( 0, i + 1 ) );
          if( size.Width > offset )
          {
            if( spaceIndex > 0 )
              resIndex = spaceIndex;
            else  
              resIndex = i - 1; 
            break;
          }
          
          if( text[ i + 1 ] == ' ' )
            spaceIndex = i + 1;
        }
        if( resIndex < 0 )
        {
          resIndex = text.Length - 1;
        }
      }
      
      return resIndex;
     /*/
      return _GetSplitIndexByOffset( text, measurer, offset, bSplitByChar );
     //*/
    }
    /// <summary>
    /// Gets character index for paragraph text splitting.
    /// If first splitted part of text has a SPACE characters - gets last SPACE character index,
    /// otherwise gets index of last character. (last character in first splitted part has left 
    /// bound with offset lesser that specified OFFSET and right bound with greater offset )
    /// <remarks>
    /// NOTE:
    /// - If text.Length == 0 then (return -1)
    /// - If offset greater than right bound offset of last character 
    ///   then (return INDEX_OF_LAST_CHARACTER)
    /// </remarks>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="measurer"></param>
    /// <param name="offset"></param>
    /// <returns>Index of character for text splitting</returns>
    private int _GetSplitIndexByOffset( string text, ITextMeasurable measurer, double offset, bool bSplitByChar )
    {
      if( text == null )
        throw new ArgumentNullException( "text" );
      
      if( measurer == null )
        throw new ArgumentNullException( "strWidget" );

      if( offset < 0 )
        throw new ArgumentOutOfRangeException( "offset", offset, "Value can not be less 0" );

      int resIndex = -1;

      if( text.Length != 0 && offset > 0 )
      {
        /* Algorithm:
              1. Find WORD which consist OFFSET
              2. If WORD not fit find character that fit to OFFSET
            */
        int wordStartIndex = 0;
        int wordLength = 0;
        double prevWordsWidth = 0f;

        // Measuring by words
        while( ( wordLength = GetWordLength( text, wordStartIndex ) ) > -1 )
        {
          SizeF size = measurer.Measure( this, text.Substring( wordStartIndex, wordLength ) );

          if( size.Width + prevWordsWidth > offset )
          {
            break;
          }

          prevWordsWidth += size.Width;
          wordStartIndex += wordLength;
        }

        resIndex = wordStartIndex - 1;

        // If nothing word fit in specified offset
        if( resIndex < 0 || bSplitByChar )
        {
          for( int i = 0; i < text.Length; i++ )
          {
            SizeF size = measurer.Measure( this, text.Substring( 0, i + 1 ) );

            if( size.Width > offset )
            {
              resIndex = i - 1;
              break;
            }
          }
        }
        
        if( resIndex < 0 )
        {
          resIndex = text.Length - 1;
        }
      }

      return resIndex;
    }
    /// <summary>
    /// Gets length of WORD.
    /// NOTE:
    /// - WORD: text run that finished by last space letter 
    ///   (sample: "text  " or "  "; wrong sample: " text" or "text  text" )
    /// EXCLUSION:
    /// - If text working part have zero symbols return (-1)
    /// - If text working part don't consist SPACE letters return legth of 
    ///   text working part
    /// </summary>
    /// <param name="text"></param>
    /// <param name="startIndex">Index of word first letter</param>
    /// <returns>Length of found word</returns>
    private int GetWordLength( string text, int startIndex )
    {
      const char cSPACE = ' ';
      int resLength = -1;
      int wordMaxLength = text.Length - startIndex;

      if( wordMaxLength > 0 )
      {
        for( int i = startIndex, len = text.Length; i < len; i++ )
        {
          if( i == len - 1 ) // i == last letter
          {
            resLength = wordMaxLength;
            break;
          }
          else if( text[ i ] == cSPACE && text[ i + 1 ] != cSPACE )
          {
            resLength = i - startIndex + 1;
            break;
          }
        }
      }

      return resLength;
    }
    #endregion
  }
}