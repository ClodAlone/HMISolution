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
using System.Collections;

using Syncfusion.XlsIO.FormatParser.FormatTokens;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.FormatParser
{
  /// <summary>
  /// Class used for Section Collection.
  /// </summary>
  public class FormatSectionCollection
    : CollectionBaseEx<FormatSection>
  {
    #region Class constants
    /// <summary>
    /// Two many sections error message.
    /// </summary>
    private const string DEF_TWO_MANY_SECTIONS_MESSAGE = "Two many sections in format.";
    /// <summary>
    /// Maximum number of sections in "conditional mode".
    /// </summary>
    private const int DEF_CONDITION_MAX_COUNT = 3;
    /// <summary>
    /// Maximum number of secionts in "non-conditional mode";
    /// </summary>
    private const int DEF_NONCONDITION_MAX_COUNT = 4;
    /// <summary>
    /// Index of section with positive number format.
    /// </summary>
    private const int DEF_POSITIVE_SECTION = 0;
    /// <summary>
    /// Index of section with negative number format.
    /// </summary>
    private const int DEF_NEGATIVE_SECTION = 1;
    /// <summary>
    /// Index of section with positive number format.
    /// </summary>
    private const int DEF_ZERO_SECTION = 2;
    /// <summary>
    /// Index of section with positive number format.
    /// </summary>
    private const int DEF_TEXT_SECTION = 3;
    #endregion

    #region Class members
    /// <summary>
    /// Indicates whether format contains conditions.
    /// </summary>
    private bool m_bConditionalFormat;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the FormatSectionCollection class to prevent creation without arguments.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    private FormatSectionCollection( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// Initializes a new instance of the FormatSectionCollection class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="arrTokens">List to parse.</param>
    public FormatSectionCollection( IApplication application, object parent, List<FormatTokenBase> arrTokens )
      : base( application, parent )
    {
      if( arrTokens == null )
        throw new ArgumentNullException( "arrTokens" );

      Parse( arrTokens );
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Returns format type for a specified value.
    /// </summary>
    /// <param name="value">Value to get format type for.</param>
    /// <returns>Format type for the specified value.</returns>
    public ExcelFormatType GetFormatType( double value )
    {
      FormatSection section = GetSection( value );

      if( section == null )
        throw new FormatException( "Can't find required format section." );

      return section.FormatType;
    }
    /// <summary>
    /// Returns format type for a specified value.
    /// </summary>
    /// <param name="value">Value to get format type for.</param>
    /// <returns>Format type for the specified value.</returns>
    public ExcelFormatType GetFormatType( string value )
    {
      // TODO: check whether it is correct in all situations.
      return (HasDateTimeFormat())? ExcelFormatType.DateTime: GetSection( 3 ).FormatType;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Splits array of tokens by SectionSeparator.
    /// </summary>
    /// <param name="arrTokens">List to parse.</param>
    private void Parse( List<FormatTokenBase> arrTokens )
    {
      if( arrTokens == null )
        throw new ArgumentNullException( "arrTokens" );

      List<FormatTokenBase> arrCurrentSection = new List<FormatTokenBase>();

      for( int i = 0, len = arrTokens.Count; i < len; i++ )
      {
        FormatTokenBase token = arrTokens[ i ];

        if( token.TokenType == TokenType.Section )
        {
          InnerList.Add( new FormatSection( Application, this, arrCurrentSection ) );
          arrCurrentSection = new List<FormatTokenBase>();
        }
        else
        {
          arrCurrentSection.Add( token );
        }
      }

      InnerList.Add( new FormatSection( Application, this, arrCurrentSection ) );

      if( this[ 0 ].HasCondition )
      {
          int conditionCount = 0;
          for (int i = 0; i < this.Count ; i++)
          {
              if( this[i].HasCondition )
                conditionCount++;
          }
          if ( conditionCount > DEF_CONDITION_MAX_COUNT)
            throw new FormatException( DEF_TWO_MANY_SECTIONS_MESSAGE );

        m_bConditionalFormat = true;
      }
      else if( Count > DEF_NONCONDITION_MAX_COUNT )
      {
        throw new FormatException( DEF_TWO_MANY_SECTIONS_MESSAGE );
      }
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <param name="bShowReservedSymbols">Indicates whether to show reserved symbols.</param>
    /// <returns>String representation of the value.</returns>
    public string ApplyFormat( double value, bool bShowReservedSymbols )
    {
      FormatSection section = GetSection( value );

      if( section != null )
      {
        if( !m_bConditionalFormat && value < 0 && Count > 1 ) value = -value;

        return section.ApplyFormat( value, bShowReservedSymbols );
      }

      throw new FormatException( "Can't locate correct section." );
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <param name="bShowReservedSymbols">Indicates whether to show reserved symbols.</param>
    /// <returns>String representation of the value.</returns>
    public string ApplyFormat( string value, bool bShowReservedSymbols )
    {
      FormatSection section = GetTextSection();
      return ( section != null )
        ? section.ApplyFormat( value, bShowReservedSymbols )
        : value;
    }
    /// <summary>
    /// Returns section for formatting with specified index.
    /// </summary>
    /// <param name="iSectionIndex">Section index.</param>
    /// <returns>Format section that should be to the value.</returns>
    private FormatSection GetSection( int iSectionIndex )
    {
      return this[ iSectionIndex % Count ];
    }
    /// <summary>
    /// Returns section that corresponds to the specified value.
    /// </summary>
    /// <param name="value">Value to search section for.</param>
    /// <returns>If section is found then returns it, otherwise returns Null.</returns>
    private FormatSection GetSection( double value )
    {
      FormatSection result = null;

      if( m_bConditionalFormat )
      {
        int iCount = Count;
        FormatSection section;

        for( int i = 0; i < iCount; i++ )
        {
          section = this[ i ];

          bool bCondition = section.HasCondition;

          if( !bCondition || bCondition && section.CheckCondition( value ) )
          {
            result = section;
            break;
          }
        }
      }
      else
      {
        if( value > 0 )
        {
          result = GetSection( DEF_POSITIVE_SECTION );
        }
        else if( value < 0 )
        {
          result = GetSection( DEF_NEGATIVE_SECTION );
        }
        else
        {
          result = GetZeroSection();//GetSection( DEF_ZERO_SECTION );
        }
      }

      return result;
    }
    /// <summary>
    /// Searches for section that should be used for zero number formatting.
    /// </summary>
    /// <returns>Section that should be used for zero number formatting.</returns>
    private FormatSection GetZeroSection()
    {
      if( m_bConditionalFormat )
        throw new NotSupportedException( "This method is not supported for number formats with conditions." );

      int iSectionsCount = InnerList.Count;
      int iLastSection = iSectionsCount - 1;
      FormatSection result = null;

      if( iLastSection < DEF_ZERO_SECTION )
      {
        result = this[ DEF_POSITIVE_SECTION ];
      }
      else if( iLastSection > DEF_ZERO_SECTION )
      {
        result = this[ DEF_ZERO_SECTION ];
      }
      else
      {
        result = this[ DEF_ZERO_SECTION ];

        if( result.FormatType == ExcelFormatType.Text )
          result = this[ DEF_POSITIVE_SECTION ];
      }

      return result;
    }
    /// <summary>
    /// Returns text section.
    /// </summary>
    /// <returns>Text format section.</returns>
    private FormatSection GetTextSection()
    {
      //return ( m_bConditionalFormat ) ? null : GetSection( DEF_TEXT_SECTION );
      FormatSection result = null;

      if( !m_bConditionalFormat )
      {
        int iSectionsCount = InnerList.Count;
        int iLastSection = iSectionsCount - 1;

        if( iLastSection >= DEF_TEXT_SECTION )
        {
          result = this[ DEF_TEXT_SECTION ];
        }
        else
        {
          result = this[ iLastSection ];

          if( result.FormatType != ExcelFormatType.Text )
            result = this[ DEF_POSITIVE_SECTION ];
        }
      }

      return result;
    }
    public bool IsTimeFormat(double value)
    {
        if( value < 0 )
          return false;

        return GetSection( value ).IsTimeFormat;
    }
    private bool HasDateTimeFormat()
    {
        foreach (FormatSection section in this)
        {
            if(section.FormatType== ExcelFormatType.DateTime)
                    return true;
        }
        return false;
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Represents parent object.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      FormatSectionCollection result = new FormatSectionCollection( Application, parent );
      List<FormatSection> listSource = InnerList;
      List<FormatSection> listDest = result.InnerList;

      for( int i = 0, len = Count; i < len; i++ )
      {
        FormatSection section = listSource[ i ];
        section = ( FormatSection )section.Clone( result );
        listDest.Add( section );
      }

      return result;
    }

    #endregion

    internal void Dispose()
    {
        int count = this.InnerList.Count;

        for (int i = 0; i < count; i++)
        {
            this.InnerList[i].Clear();
            this.InnerList[i] = null;
        }

    }
  }
}
