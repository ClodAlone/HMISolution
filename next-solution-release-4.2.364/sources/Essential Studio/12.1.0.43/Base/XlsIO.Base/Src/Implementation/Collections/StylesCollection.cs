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

#define MAP_COLLECTION

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Reflection;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;

#if  (SILVERLIGHT) || (WINRT) || (WP)
using Syncfusion.XlsIO.Interfaces;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// A collection of all the Style objects in the specified or active
  /// workbook. Each Style object represents a style description for a
  /// range. The Style object also contains all style attributes (font,
  /// number format, alignment, and so on) as properties. There are
  /// several built-in styles � including Normal, Currency, and Percent
  /// � which are listed in the Style name box in the Style dialog box
  /// (Format menu).
  /// </summary>
  public class StylesCollection
    : CollectionBaseEx<IStyle>
    , IStyles
  {
    #region Class constants
#if HASH_STYLE
    private string[] hashSkip = new string[]
    {
      "Application",
      "Parent",
    };
#endif
    #endregion

    #region Class members
#if MAP_COLLECTION
    /// <summary>
    /// RB-tree for quick style search operation.
    /// </summary>
    //private MapCollection m_map = new MapCollection();
    private Dictionary<StyleImpl, StyleImpl> m_map = new Dictionary<StyleImpl, StyleImpl>();
#endif
    /// <summary>
    /// Collection of all the styles in the workbook.
    /// </summary>
    private Dictionary<string, StyleImpl> m_dictStyles;
    /// <summary>
    /// Dictionary StyleXFIndex to StyleImpl.
    /// </summary>
    private Dictionary<int, StyleImpl> m_hashIndexToStyle;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_holder;

#if MAP_COLLECTION
    /// <summary>
    /// 
    /// </summary>
    private EventHandler m_beforeChange;
    /// <summary>
    /// 
    /// </summary>
    private EventHandler m_afterChange;
#endif

#if HASH_STYLE
    private Dictionary<int, StyleImpl> m_dictFastFind = new Dictionary<int, StyleImpl>();
#endif
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates the collection and sets its Application and Parent properties.
    /// </summary>
    /// <param name="application">The application object for this collection.</param>
    /// <param name="parent">The parent object for this collection.</param>
    public StylesCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_dictStyles = new Dictionary<string, StyleImpl>();

      object result = this.FindParent( typeof( WorkbookImpl ) );
      
      if( result == null )
        throw new ArgumentException( "Style collection must be in Workbook object tree." );

      m_holder = ( WorkbookImpl )result;
#if MAP_COLLECTION
      m_beforeChange = new EventHandler( OnStyleBeforeChange );
      m_afterChange = new EventHandler( OnStyleAfterChange );
#endif
    }
    #endregion

    #region IStyles Members
    /// <summary>
    /// Returns a single object from a collection. Read-only.
    /// </summary>
    public IStyle this[ string name ]
    {
      get
      {
        StyleImpl result;

        if( !m_dictStyles.TryGetValue( name, out result ) )
          throw new ArgumentException( "Style with specified name does not exist. Name: " + name, "value" );

        return result;
      }
    }
    /// <summary>
    /// Creates a new style and adds it to the list of styles that are
    /// available for the current workbook. Returns a Style object.
    /// </summary>
    /// <param name="name">Name of the newly created style.</param>
    /// <param name="BasedOn">Prototype for the style.</param>
    /// <returns>Newly created style.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If name is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When collection already contains style with specified name.
    /// </exception>
    public IStyle Add( string name, object BasedOn )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      if( m_dictStyles.ContainsKey( name ) )
        throw new ArgumentException( "Name of style must be unique." );

      IStyle style = null;

      //if( BasedOn == null || BasedOn.ToString() == "Normal" )
      if( BasedOn == null )
      {
        style = AppImplementation.CreateStyle( m_holder, name );//( Parent, name );
      }
      else if( BasedOn is string )
      {
        style = AppImplementation.CreateStyle( m_holder, name, ( StyleImpl )this[ ( string )BasedOn ] );
          //( Parent, name, ( StyleImpl )this[ ( string )BasedOn ] );
      }
      else if( BasedOn is StyleImpl )
      {
        style = AppImplementation.CreateStyle( m_holder, name, ( StyleImpl )BasedOn );
        //( Parent, name, ( StyleImpl )BasedOn );
      }

      if( style != null ) base.Add( style );

      return style;
    }

    /// <summary>
    /// Creates a new style and adds it to the list of styles that are
    /// available for the current workbook. Returns a Style object.
    /// </summary>
    /// <param name="name">Name of the newly created style.</param>
    /// <returns>Newly created style.</returns>
    public IStyle Add( string name )
    {
      return Add( name, null );
    }

    /// <summary>
    /// Merges the styles from another workbook into the Styles collection. 
    /// Keep only unique style in collection.
    /// </summary>
    /// <param name="Workbook">Workbook from which all styles will be added.</param>
    /// <returns>Merged collection.</returns>
    public IStyles Merge( object Workbook )
    {
      return Merge( Workbook, false );
    }
    /// <summary>
    /// Merges the styles from another workbook into the Styles collection.
    /// </summary>
    /// <param name="Workbook">Workbook from which all styles will be added.</param>
    /// <param name="overwrite">True to overwrite styles with the same names.</param>
    /// <returns>Merged collection.</returns>
    /// <exception cref="System.ArgumentException">
    /// If specified Workbook is not WorkbookImpl.
    /// </exception>
    public IStyles Merge( object Workbook, bool overwrite )
    {
      if( Workbook == null )
        throw new ArgumentNullException( "Workbook" );

      if( !( Workbook is WorkbookImpl ) )
        throw new ArgumentException( "Wrong argument type", "Workbook" );

      WorkbookImpl book = ( WorkbookImpl )Workbook;
      Merge( book, overwrite ? ExcelStyleMergeOptions.Replace : ExcelStyleMergeOptions.Leave );

      return this;
    }
    /// <summary>
    /// Removes style from the collection.
    /// </summary>
    /// <param name="styleName">Style to remove.</param>
    public void Remove( string styleName )
    {
      if( styleName == null || styleName.Length == 0 )
        return;

      StyleImpl style;
      m_dictStyles.TryGetValue( styleName, out style );

      if( style == null || style.BuiltIn )
        return;

      int iXFIndex = style.XFormatIndex;

      m_map.Remove( style );
      base.Remove( style );
      m_dictStyles.Remove( styleName );

      WorkbookImpl book = style.Workbook;
      book.RemoveExtenededFormatIndex( iXFIndex );
    }
    #endregion

    #region Recovery specific methods
    /// <summary>
    /// Adds specified style into this collection.
    /// </summary>
    /// <param name="style">Style that must be added.</param>
    public void Add( IStyle style )
    {
//      if( !m_holder.Loading && ContainsName( style.Name ) )

      string strStyleName = style.Name;

      if( ContainsName( strStyleName ) )
      {
        StyleImpl styleNew = ( StyleImpl )style;
        StyleImpl styleOld;
        m_dictStyles.TryGetValue( strStyleName, out styleOld );
        bool isEqual = styleNew.Index == styleOld.Index ?
                                                    true
                                                  : false;
        if( isEqual )
        {
            if(styleNew.BuiltIn == styleOld.BuiltIn)
          throw new ArgumentException(
            string.Format( "Collection already contains style with same names {0}.", style.Name ) );
        }
        if( styleNew.BuiltIn )
        {
          m_dictStyles[ strStyleName ] = styleNew;
        }
        else if( !styleOld.BuiltIn && !m_holder.Loading )
        {
          throw new ArgumentException( 
            string.Format( "Collection already contains style with same names {0}.", style.Name ) );
        }
      }

      base.Add( style );
    }
    /// <summary>
    /// Adds specified style to collection.
    /// </summary>
    /// <param name="style">Style to add.</param>
    /// <param name="bReplace">If true and ContainName then replace old name; otherwise add.</param>
    public void Add( IStyle style, bool bReplace )
    {
      if( ContainsName( style.Name ) )
      {
        if( bReplace )
        {
          //IStyle oldStyle = ( IStyle )m_dictStyles[ style.Name ];

          for( int i = 0, len = List.Count; i < len; i++ )
          {
            if( ( ( IStyle )List[ i ] ).Name == style.Name )
            {
              List[ i ] = style;//.Add( style );
              break;
            }
          }
        }
      }
      else
      {
        base.Add( style );
      }
    }
    /// <summary>
    /// Method returns True if collection contains style 
    /// with specified by user name.
    /// </summary>
    /// <param name="name">Name to check.</param>
    /// <returns>True if style exists; otherwise False.</returns>
    public bool  Contains( string name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      if( name.Length == 0 )
        throw new ArgumentException( "name - string cannot be empty." );

      return ContainsName( name );
    }
    #endregion

    #region Implementation methods
    /// <summary>
    /// Searches in the collection for style which is same as specified.
    /// </summary>
    /// <param name="style">Style to search.</param>
    /// <returns>Found style or NULL if no same style exists in list.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If specified style is NULL.
    /// </exception>
    public IStyle ContainsSameStyle( IStyle style )
    {
      if( style == null )
        throw new ArgumentNullException( "style" );

      StyleImpl destination = null;

//      if( style is StyleWrapper )
//      {
//        destination = (( StyleWrapper )style).Wrapped;
//      }
//      else
//      {
//        destination = style as StyleImpl;
//      }
      destination = style as StyleImpl;

#if HASH_STYLE
      int hash = HashCalculate.CalculateHash( destination, hashSkip );
      
      if( !m_dictFastFind.ContainsKey( hash ) )
      return null;

      return ( IStyle )m_dictFastFind[ hash ];
#else
      destination.NotCompareNames = true;
#if MAP_COLLECTION
      style = m_map[ destination ] as IStyle;
      destination.NotCompareNames = false;
      return style;
#else
      for( int i = 0, len = InnerList.Count; i < len; i++ )
      {
        StyleImpl stylesource = InnerList[ i ] as StyleImpl;

//        if( CompareStyles( stylesource, destination ) )
        if( stylesource.CompareTo( destination ) == 0  )
        {
          destination.NotCompareNames = false;
          return stylesource;
        }
      }

      destination.NotCompareNames = false;
      return null;
#endif
#endif
    }
    /// <summary>
    /// Compares two styles.
    /// </summary>
    /// <param name="source">The first style to compare.</param>
    /// <param name="destination">The second style to compare.</param>
    /// <returns>True if styles are the same; otherwise False.</returns>
    //public static bool CompareStyles( StyleImpl source, StyleImpl destination )
    public static bool CompareStyles( IStyle source, IStyle destination )
    {
      bool result = ( source.Color == destination.Color )
        && ( source.PatternColor == destination.PatternColor )
        && ( source.FillPattern == destination.FillPattern )
        && ( source.NumberFormat == destination.NumberFormat )
        && ( source.FormulaHidden == destination.FormulaHidden )
        && ( source.HorizontalAlignment == destination.HorizontalAlignment )
        && ( source.VerticalAlignment == destination.VerticalAlignment )
        && ( source.WrapText == destination.WrapText )
        && source.Font.Equals( destination.Font )//CompareFonts( source.Font, destination.Font )
        //*
        && ( source.IncludeAlignment == destination.IncludeAlignment )
        && ( source.IncludeBorder == destination.IncludeBorder )
        && ( source.IncludeFont == destination.IncludeFont )
        && ( source.IncludeNumberFormat == destination.IncludeNumberFormat )
        && ( source.IncludePatterns == destination.IncludePatterns )
        && ( source.IncludeProtection == destination.IncludeProtection )
        && ( source.IndentLevel == destination.IndentLevel )
        //*/
        && CompareBorders( source.Borders, destination.Borders )
        && ( source.Locked == destination.Locked )
        && ( source.ShrinkToFit == destination.ShrinkToFit );

      return result;
    }
    /// <summary>
    /// Compare all Border Collections items.
    /// </summary>
    /// <param name="source">First border to compare.</param>
    /// <param name="destination">Second border to compare.</param>
    /// <returns>True if borders are the same; otherwise False.</returns>
    public static bool CompareBorders( IBorders source, IBorders destination )
    {
      return CompareBorder( source[ ExcelBordersIndex.EdgeBottom ], destination[ ExcelBordersIndex.EdgeBottom ] )
      && CompareBorder( source[ ExcelBordersIndex.EdgeLeft ], destination[ ExcelBordersIndex.EdgeLeft ] )
      && CompareBorder( source[ ExcelBordersIndex.EdgeRight ], destination[ ExcelBordersIndex.EdgeRight ] )
      && CompareBorder( source[ ExcelBordersIndex.EdgeTop ], destination[ ExcelBordersIndex.EdgeTop ] )
      && CompareBorder( source[ ExcelBordersIndex.DiagonalDown ], destination[ ExcelBordersIndex.DiagonalDown ] )
      && CompareBorder( source[ ExcelBordersIndex.DiagonalUp ], destination[ ExcelBordersIndex.DiagonalUp ] );
    }

    /// <summary>
    /// Compare Border interfaces.
    /// </summary>
    /// <param name="source">First border to compare.</param>
    /// <param name="destination">Second border to compare.</param>
    /// <returns>True if borders are the same; otherwise False.</returns>
    public static bool CompareBorder( IBorder source, IBorder destination )
    {
      return ( source.ColorObject == destination.ColorObject )
      && ( source.LineStyle == destination.LineStyle )
      && ( source.ShowDiagonalLine == destination.ShowDiagonalLine );
    }
    /// <summary>
    /// Merges styles from specified workbook.
    /// </summary>
    /// <param name="workbook">Source workbook.</param>
    /// <param name="option">Merge options.</param>
    /// <returns>Dictionary with changed style names, key - old name, value - new name.</returns>
    public Dictionary<string, string> Merge( IWorkbook workbook, ExcelStyleMergeOptions option )
    {
      Dictionary<int, int> dicFonts;
      Dictionary<int, int> hashXFIndexes;

      return Merge( workbook, option, out dicFonts, out hashXFIndexes );
    }
    /// <summary>
    /// Merges styles from specified workbook.
    /// </summary>
    /// <param name="workbook">Source workbook.</param>
    /// <param name="option">Merge options.</param>
    /// <param name="dicFontIndexes">Returns dictionary with new font indexes.</param>
    /// <param name="hashExtFormatIndexes">Dictionary with new indexes of extended formats.</param>
    /// <returns>Dictionary with changed style names, key - old name, value - new name.</returns>
    public Dictionary<string, string> Merge( IWorkbook workbook, ExcelStyleMergeOptions option,
      out Dictionary<int, int> dicFontIndexes, out Dictionary<int, int> hashExtFormatIndexes )
    {
      if( !( workbook is WorkbookImpl ) )
        throw new ArgumentException( "Wrong argument type", "Workbook" );

      WorkbookImpl book = ( WorkbookImpl )workbook;
      hashExtFormatIndexes = null;
      dicFontIndexes = null;

      if( book == m_holder )
        return null;

      StylesCollection styles = book.InnerStyles;
      Dictionary<string, string> result = new Dictionary<string, string>();

      // We have to try to merge extended formats.
      hashExtFormatIndexes = m_holder.InnerExtFormats.Merge( book.InnerExtFormats, out dicFontIndexes );

      for (int i = 0, len = styles.Count; i < len; i++)
      {
          StyleImpl style = styles[i] as StyleImpl;
          string strStyleName = style.Name;
          bool bContains = m_dictStyles.ContainsKey(strStyleName);
          bool bisModified = style.IsBuiltInCustomized;
          bool isBuiltIn = style.BuiltIn;
          if ((!isBuiltIn)||(isBuiltIn && bisModified))
          {
              switch (option)
              {
                  case ExcelStyleMergeOptions.CreateDiffName:
                      strStyleName = GenerateDefaultName(this, strStyleName + "_");
                      result.Add(strStyleName, strStyleName);
                      break;

                  case ExcelStyleMergeOptions.Leave:
                      strStyleName = null;
                      break;

                  case ExcelStyleMergeOptions.Replace:
                      break;

                  default:
                      throw new ArgumentOutOfRangeException("option");
              }

              if (strStyleName != null)
              {
                  ICloneable record = (ICloneable)style.Record;
                  StyleRecord newRecord = (StyleRecord)record.Clone();
                  int iOldXFormat = newRecord.ExtendedFormatIndex;
                  newRecord.ExtendedFormatIndex = (ushort)(int)hashExtFormatIndexes[iOldXFormat];
                  newRecord.StyleName = strStyleName;
                  Add(newRecord);
              }
          }
          else if (isBuiltIn && !bContains)
          {
              result.Add(strStyleName, strStyleName);
              if (strStyleName != null)
              {
                  ICloneable record = (ICloneable)style.Record;
                  StyleRecord newRecord = (StyleRecord)record.Clone();
                  int iOldXFormat = newRecord.ExtendedFormatIndex;
                  newRecord.ExtendedFormatIndex = (ushort)(int)hashExtFormatIndexes[iOldXFormat];
                  newRecord.StyleName = strStyleName;
                  Add(newRecord);
              }
          }

        // TODO: create style here
      }

      return result;
    }
    /// <summary>
    /// Generates default name.
    /// </summary>
    /// <param name="strStart">Name prefix.</param>
    /// <returns>Automatically generated unique name.</returns>
    public string GenerateDefaultName( string strStart )
    {
      return GenerateDefaultName( strStart, m_dictStyles.Values );
    }
    /// <summary>
    /// Generates default name.
    /// </summary>
    /// <param name="strStart">Name prefix.</param>
    /// <param name="hashNamesInFile">Dictionary with used names.</param>
    /// <returns>Automatically generated unique name.</returns>
    public string GenerateDefaultName( string strStart, Dictionary<string, StyleRecord> hashNamesInFile )
    {
      return GenerateDefaultName( strStart, m_dictStyles.Values, hashNamesInFile.Keys );
    }
    /// <summary>
    /// Creates built in style.
    /// </summary>
    /// <param name="strName">Name of the style.</param>
    /// <returns>Created style.</returns>
    public StyleImpl CreateBuiltInStyle( string strName )
    {
      if( strName == null )
        throw new ArgumentNullException( "strName" );

      if( strName.Length == 0 )
        throw new ArgumentException( "strName - string cannot be empty" );

      StyleImpl result = AppImplementation.CreateStyle( m_holder, strName, true );
      //( this, strName, true );
      base.Add( result );
      return result;
    }
    /// <summary>
    /// Returns style corresponding to the index of extended format.
    /// </summary>
    /// <param name="index">Index of the extended format to find.</param>
    /// <returns>Found style.</returns>
    public StyleImpl GetByXFIndex( int index )
    {
      if( m_hashIndexToStyle != null && m_hashIndexToStyle.ContainsKey( index ) )
      {
        return m_hashIndexToStyle[ index ];
      }
      else
      {
        // Hash was cleared or wasn't build correctly.
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          StyleImpl style = this[ i ] as StyleImpl;
        
          if( style.Index == index )
          {
            return style;
          }
        }
      }

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Style was not found.", "Possible error: " );
      return null;
    }
    /// <summary>
    /// Updates style record of each style in the collection according to the xf indexes.
    /// </summary>
    public void UpdateStyleRecords()
    {
      List<IStyle> arrList = InnerList;

      for( int i = 0, len = arrList.Count; i < len; i++ )
      {
        StyleImpl style = ( StyleImpl )arrList[ i ];
        style.UpdateStyleRecord();
      }
    }
    /// <summary>
    /// Searches in collection for style named styleName.
    /// </summary>
    /// <param name="styleName">Style name to search.</param>
    /// <returns>Searched style if found; otherwise returns NULL.</returns>
    internal IStyle Find( string styleName )
    {
      return this[ styleName ];
    }
    /// <summary>
    /// Method checks whether the collection contains a style with the specified name.
    /// </summary>
    /// <param name="styleName">Name to check.</param>
    /// <returns>True if style exits; otherwise False.</returns>
    internal bool ContainsName( string styleName )
    {
      return m_dictStyles.ContainsKey( styleName );
    }
    /// <summary>
    /// Adds new style to the collection.
    /// </summary>
    /// <param name="style">Style to add.</param>
    [ CLSCompliant( false ) ]
    public void Add( StyleRecord style )
    {
      if( style == null )
        throw new ArgumentNullException( "style" );

      StyleImpl styleToAdd = AppImplementation.CreateStyle( m_holder, style );
      Add( styleToAdd );
    }
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns>Copy of the current instance.</returns>
    public override object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      StylesCollection result = new StylesCollection( Application, parent );

      List<IStyle> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        StyleImpl style = ( StyleImpl )list[ i ];
        style = ( StyleImpl )style.Clone( result );
        result.Add( style );
      }

      return result;
    }
    #endregion

    #region Implementation properties
#if MAP_COLLECTION
    /// <summary>
    /// 
    /// </summary>
    //public MapCollection Map
    public Dictionary<StyleImpl, StyleImpl> Map
    {
      get
      {
        return m_map;
      }
    }
#endif
    #endregion

    #region Keep in sync fast style by name finder
    /// <summary>
    /// Performs additional processes after clearing the collection.
    /// </summary>
    protected override void OnClearComplete()
    {
      m_dictStyles.Clear();
#if MAP_COLLECTION
      m_map.Clear();
#endif

#if HASH_STYLE
      m_dictFastFind.Clear();
#endif

      if( m_hashIndexToStyle != null )
        m_hashIndexToStyle.Clear();

      base.OnClearComplete();
    }

    /// <summary>
    /// Performs additional processes after inserting a new element into the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at the index.</param>
    protected override void OnInsertComplete( int index, IStyle value )
    {
      string strStyleName = value.Name;

      if( !m_dictStyles.ContainsKey( strStyleName ) )
        m_dictStyles[ value.Name ] = ( StyleImpl )value;

      StyleImpl style = ( StyleImpl )value;

      if( m_hashIndexToStyle != null )
      {
        m_hashIndexToStyle[ style.Index ] = style;
      }

#if MAP_COLLECTION
      m_map.Add( style, style );
      //m_map[ style ] = style;
      style.BeforeChange += m_beforeChange;
      style.AfterChange += m_afterChange;
#endif

#if HASH_STYLE
      int hash = HashCalculate.CalculateHash( value, hashSkip );
      m_dictFastFind[ hash ] = ( StyleImpl )value;
#endif

      base.OnInsertComplete( index, value );
    }

    /// <summary>
    /// Performs additional processes after removing an element from the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which value can be found.</param>
    /// <param name="value">The value of the element to remove from the index.</param>
    protected override void OnRemoveComplete( int index, IStyle value )
    {
      StyleImpl style = ( StyleImpl )value;
      m_dictStyles.Remove( style.Name );

      if( m_hashIndexToStyle != null )
      {
        m_hashIndexToStyle.Remove( style.Index );
      }

#if MAP_COLLECTION
      m_map.Remove( style );
      style.BeforeChange -= m_beforeChange;
      style.AfterChange -= m_afterChange;
#endif

#if HASH_STYLE
      int hash = HashCalculate.CalculateHash( value, hashSkip );
      m_dictFastFind.Remove( hash );
#endif

      base.OnRemoveComplete( index, value );
    }

    /// <summary>
    /// Performs additional processes after setting a value in the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which oldValue can be found.</param>
    /// <param name="oldValue">The value to replace with newValue.</param>
    /// <param name="newValue">The new value of the element at the index.</param>
    protected override void OnSetComplete( int index, IStyle oldValue, IStyle newValue )
    {
      StyleImpl styleOld = ( StyleImpl )oldValue;
      StyleImpl styleNew = ( StyleImpl )newValue;

      if( m_hashIndexToStyle != null )
      {
        m_hashIndexToStyle.Remove( styleOld.Index );
        m_hashIndexToStyle[ styleNew.Index ] = styleNew;
      }

      m_dictStyles.Remove( styleOld.Name );

#if MAP_COLLECTION
      m_map.Remove( styleOld );
      m_map.Add( styleNew, styleNew );
      //m_map[ styleNew ] = styleNew;
#endif

#if HASH_STYLE
      int hash = HashCalculate.CalculateHash( oldValue, hashSkip );
      m_dictFastFind.Remove( hash );
#endif

      if( m_dictStyles.ContainsKey( styleNew.Name ) )
        throw new ArgumentException( "Collection cannot contain two styles with same name." );

      m_dictStyles[ styleNew.Name ] = styleNew;
      
#if HASH_STYLE
      hash = HashCalculate.CalculateHash( newValue, hashSkip );
      m_dictFastFind[ hash ] = ( StyleImpl )newValue;
#endif

      base.OnSetComplete( index, oldValue, newValue );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnStyleBeforeChange( object sender, EventArgs args )
    {
      if( sender == null )
        throw new ArgumentNullException( "sender" );

      if( args == null )
        throw new ArgumentNullException( "args" );

      StyleImpl style = ( StyleImpl ) sender;

#if MAP_COLLECTION
      m_map.Remove( style );
      
      style.BeforeChange -= m_beforeChange;
      //style.AfterChange -= m_afterChange;
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnStyleAfterChange( object sender, EventArgs args )
    {
      if( sender == null )
        throw new ArgumentNullException( "sender" );

      if( args == null )
        throw new ArgumentNullException( "args" );

      StyleImpl style = ( StyleImpl ) sender;

#if MAP_COLLECTION
      m_map.Add( style, style );
      //m_map[ style ] = style;
      style.BeforeChange += m_beforeChange;
//      style.AfterChange += m_afterChange;
#endif
    }
    /// <summary>
    /// Clears hashIndexToStyle member.
    /// </summary>
    internal void ClearStylesHash()
    {
      m_hashIndexToStyle = null;
      m_map = null;
      m_dictStyles = null;
    }
    #endregion

    internal void Dispose()
    {
        foreach (StyleImpl style in this.InnerList)
        {
            style.Dispose();
        }
    }
  }
}
