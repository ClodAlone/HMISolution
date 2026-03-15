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

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.FormatParser;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Represents number format. Responsible for reading, writing,
	/// parsing, applying, checking and other operations with number formats.
	/// </summary>
	public class FormatImpl
    : CommonObject
    , INumberFormat
    , ICloneParent
	{
    #region Class members
    /// <summary>
    /// Format record that contains low-level information about format.
    /// </summary>
    private FormatRecord m_format;
    /// <summary>
    /// Parsed format.
    /// </summary>
    private FormatSectionCollection m_parsedFormat;
    /// <summary>
    /// Reference to the format parser.
    /// </summary>
    private FormatParserImpl m_parser;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the format.
    /// </summary>
    /// <param name="application">Application object for the new format.</param>
    /// <param name="parent">Parent object for the new format.</param>
    protected FormatImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    /// <summary>
    /// Initializes new instance of the format.
    /// </summary>
    /// <param name="application">Application object for the new format.</param>
    /// <param name="parent">Parent object for the new format.</param>
    /// <param name="format">Format record that contains low-level information about format.</param>
    [ CLSCompliant( false ) ]
    public FormatImpl( IApplication application, object parent, FormatRecord format )
      : this( application, parent )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      m_format = ( FormatRecord )format.Clone();
    }
    /// <summary>
    /// Initializes new instance of the format.
    /// </summary>
    /// <param name="application">Application object for the new format.</param>
    /// <param name="parent">Parent object for the new format.</param>
    /// <param name="index">Format index.</param>
    /// <param name="strFormat">Format string.</param>
    public FormatImpl( IApplication application, object parent, int index, string strFormat )
      : this( application, parent )
    {
      if( strFormat == null )
        throw new ArgumentNullException( "strFormat" );

      if( strFormat.Length == 0 )
        throw new ArgumentException( "strFormat - string cannot be empty." );

      m_format = ( FormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Format );
      m_format.Index = index;
      m_format.FormatString = strFormat;
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      FormatsCollection formats = FindParent( typeof( FormatsCollection ) ) as FormatsCollection;

      if( formats == null )
        throw new ArgumentNullException( "Parent", "Can't find parent collection of formats." );

      m_parser = formats.Parser;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns format index. Read-only.
    /// </summary>
    public int Index
    {
      get
      {
        return m_format.Index;
      }
    }
    /// <summary>
    /// Returns format string. Read-only.
    /// </summary>
    public string FormatString
    {
      get
      {
        return m_format.FormatString;
      }
    }
    /// <summary>
    /// Returns format record that contains low-level information about format. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public FormatRecord Record
    {
      get
      {
        return m_format;
      }
    }

    /// <summary>
    /// Returns format type of the first section of this number format. Read-only.
    /// </summary>
    public ExcelFormatType FormatType
    {
      get
      {
        PrepareFormat();

        return m_parsedFormat[ 0 ].FormatType;
      }
    }
    /// <summary>
    /// Indicates whether the first section of this number format contains fraction sign. Read-only.
    /// </summary>
    public bool IsFraction
    {
      get
      {
        PrepareFormat();

        return m_parsedFormat[ 0 ].IsFraction;
      }
    }
    /// <summary>
    /// Indicates whether first section of this number format contains E/E+
    /// or E- signs in format string. Read-only.
    /// </summary>
    public bool IsScientific
    {
      get
      {
        PrepareFormat();

        return m_parsedFormat[ 0 ].IsScientific;
      }
    }
    /// <summary>
    /// Indicates whether thousand separator is present in the first section
    /// of this number format. Read-only.
    /// </summary>
    public bool IsThousandSeparator
    {
      get
      {
        PrepareFormat();

        return m_parsedFormat[ 0 ].IsThousandSeparator;
      }
    }
    /// <summary>
    /// Number of digits after "." sign in the first section of this number format. Read-only.
    /// </summary>
    public int DecimalPlaces
    {
      get
      {
        PrepareFormat();

        return m_parsedFormat[ 0 ].DecimalNumber;
      }
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Serializes format into OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_format == null )
        throw new ApplicationException( "Format was not initialized propertly." );

      records.Add( m_format );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Checks whether format is already parsed, if it isn't than parses it.
    /// </summary>
    private void PrepareFormat()
    {
      if( m_parsedFormat != null ) return;

      string formatString = FormatString;
      if (this.Parent != null && (this.Parent as FormatsCollection) != null)
          if (Array.IndexOf(((this.Parent as FormatsCollection).DEF_FORMAT_STRING), formatString) >= 0)
#if ( WINRT )
              formatString = formatString.Replace(FormatsCollection.Currency, System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol.ToString());
#else
              formatString = formatString.Replace(FormatsCollection.Currency, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol.ToString());
#endif  


      m_parsedFormat = m_parser.Parse(formatString);
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
      PrepareFormat();
      return m_parsedFormat.GetFormatType( value );
    }
    /// <summary>
    /// Returns format type for a specified value.
    /// </summary>
    /// <param name="value">Value to get format type for.</param>
    /// <returns>Format type for the specified value.</returns>
    public ExcelFormatType GetFormatType( string value )
    {
      PrepareFormat();
      return m_parsedFormat.GetFormatType( value );
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <returns>String representation of the value according to the number format.</returns>
    public string ApplyFormat( double value )
    {
      return ApplyFormat( value, false );
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to show hidden symbols.</param>
    /// <returns>String representation of the value according to the number format.</returns>
    public string ApplyFormat( double value, bool bShowHiddenSymbols )
    {
      PrepareFormat();
      return m_parsedFormat.ApplyFormat( value, bShowHiddenSymbols );
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <returns>String representation of the value according to the number format.</returns>
    public string ApplyFormat( string value )
    {
      return ApplyFormat( value, false );
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to apply format to.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to show hidden symbols.</param>
    /// <returns>String representation of the value according to the number format.</returns>
    public string ApplyFormat( string value, bool bShowHiddenSymbols )
    {
      PrepareFormat();
      return m_parsedFormat.ApplyFormat( value, bShowHiddenSymbols );
    }
    public bool IsTimeFormat( double value )
    {
      return m_parsedFormat.IsTimeFormat( value );
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent )
    {
      FormatImpl result = ( FormatImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.m_format = ( FormatRecord )CloneUtils.CloneCloneable( m_format );

      if( m_parsedFormat != null )
        result.m_parsedFormat = ( FormatSectionCollection )m_parsedFormat.Clone( result );

      return result;
    }

    #endregion

    internal void Clear()
    {
        m_parser.Clear();
        if (m_parsedFormat != null)
        {
            m_parsedFormat.Dispose();

            m_parsedFormat.Clear();
        }
        m_format = null;
        m_parsedFormat = null;
        this.Dispose();
    }
    }
}
