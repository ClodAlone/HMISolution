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
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
	/// <summary>
	/// Summary description for RichTextStringGroup.
	/// </summary>
	public class RichTextStringGroup
    : CommonObject
    , IRichTextString
	{
    #region Class members
    /// <summary>
    /// Parent group of ranges.
    /// </summary>
    private RangeGroup m_rangeGroup;
    /// <summary>
    /// Represents an RTF string.
    /// </summary>
    private string m_rtfText;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    public RichTextStringGroup( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_rangeGroup = FindParent( typeof( RangeGroup ) ) as RangeGroup;

      if( m_rangeGroup == null )
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent range group." );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single RichTextString from the group. Read-only.
    /// </summary>
    public IRichTextString this[ int index ]
    {
      get
      {
//        if( index < 0 || index > Count - 1 )
//          throw new ArgumentOutOfRangeException( "index", index, "Value cannot be less than 0 and greater than Count - 1" );
        return m_rangeGroup[ index ].RichText;
      }
    }

    /// <summary>
    /// Returns number of elements in the group. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        return m_rangeGroup.Count;
      }
    }
    #endregion

    #region IRichTextString Members
    /// <summary>
    /// Returns font which is applied to character at the specified position.
    /// </summary>
    /// <param name="iPosition">Character index.</param>
    /// <returns>Font which is applied to character at the specified position.</returns>
    public IFont GetFont( int iPosition )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Sets font for range of characters.
    /// </summary>
    /// <param name="iStartPos">First character of the range.</param>
    /// <param name="iEndPos">Last character of the range.</param>
    /// <param name="font">Font to set.</param>
    public void SetFont( int iStartPos, int iEndPos, IFont font )
    {
      for( int i = 0, iCount = Count; i < iCount; i++ )
      {
        this[ i ].SetFont( iStartPos, iEndPos, font );
      }
    }
    /// <summary>
    /// Clears string formatting.
    /// </summary>
    public void ClearFormatting()
    {
      for( int i = 0, iCount = Count; i < iCount; i++ )
      {
        this[ i ].ClearFormatting();
      }
    }
    /// <summary>
    /// Appends rich text string with specified text and font.
    /// </summary>
    /// <param name="text">Text to append.</param>
    /// <param name="font">Font to use.</param>
    public void Append( string text, IFont font )
    {
      for( int i = 0, iCount = Count; i < iCount; i++ )
      {
        this[ i ].Append( text, font );
      }
    }
    /// <summary>
    /// Clears text and formatting.
    /// </summary>
    public void Clear()
    {
      for( int i = 0, iCount = Count; i < iCount; i++ )
      {
        this[ i ].Clear();
      }
    }
    /// <summary>
    /// Gets / sets text of the string.
    /// </summary>
    public string Text
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return null;

        string result = this[ 0 ].Text;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Text ) return null;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Text = value;
        }
      }
    }
    /// <summary>
    /// Returns text in rtf format. Read-only.
    /// </summary>
    public string RtfText
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return null;

        m_rtfText = this[ 0 ].RtfText;

        for( int i = 1; i < iCount; i++ )
        {
          if( m_rtfText != this[ i ].RtfText ) return null;
        }

        return m_rtfText;
      }
        set
        {
            m_rtfText = value;
        }
    }
    /// <summary>
    /// Indicates whether rich text string has formatting runs. Read-only.
    /// </summary>
    public bool IsFormatted
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IsFormatted;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IsFormatted ) return false;
        }

        return result;
      }
    }
    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// 
    /// </summary>
    public void BeginUpdate()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public void EndUpdate()
    {
    }
    #endregion
  }
}
