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
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Summary description for ParagraphStyle.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class ParagraphStyle
    : Style,
      IParagraphStyle
  {
    #region Class members
    /// <summary>
    /// The character format object
    /// </summary>
    protected CharacterFormat m_charFormat;
    /// <summary>
    /// The paragraph format object
    /// </summary>
    protected ParagraphFormat m_paraFormat;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets character format (font properties).
    /// </summary>
    public CharacterFormat CharacterFormat
    {
      get
      {
        return m_charFormat;
      }
    }
    /// <summary>
    /// Get paragraph format.
    /// </summary>
    public ParagraphFormat ParagraphFormat
    {
      get
      {
        return m_paraFormat;
      }
    }
    /// <summary>
    /// Gets base style
    /// </summary>
    public ParagraphStyle BaseStyle
    {
      get
      {
        return Document.Styles.FindByName( m_baseStyleName ) as ParagraphStyle;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="doc"></param>
    public ParagraphStyle( IDocument doc )
      : base( doc )
    {
      CreateFormats();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="style"></param>
    /// <param name="doc"></param>
    protected internal ParagraphStyle( IParagraphStyle style, IDocument doc )
      : base( style, doc )
    {
      CreateFormats();
      CharacterFormat.ImportContainer( style.CharacterFormat );
      ParagraphFormat.ImportContainer( style.ParagraphFormat );
//      ( style as ParagraphStyle ).m_baseStyle = BaseStyle.Clone( doc );
      ParagraphStyle baseStyle = ( style as ParagraphStyle ).BaseStyle;
      if( baseStyle != null )
      {
        m_baseStyleName = baseStyle.ImportStyleTo( doc as Document );
//        if( doc.Styles.FindByName( baseStyle.Name ) == null )
//        {
//          doc.Styles.Add( baseStyle.Clone( doc ) );
//        }
//        ApplyBaseStyle( m_baseStyleName );
      }
      
      foreach( Tab tab in style.ParagraphFormat.Tabs )
      {
        ParagraphFormat.Tabs.AddTab( (Tab)tab.Clone() );
      }
    }
    #endregion
    
    #region Class public methods
    /// <summary>
    /// Apply base style for current style
    /// </summary>
    /// <param name="styleName"></param>
    public ParagraphStyle ApplyBaseStyle( string styleName )
    {
      ParagraphStyle style = Document.Styles.FindByName( styleName ) as ParagraphStyle;
      if( style == null )
      {
        throw new ArgumentException( "No such base style ( \"" + styleName + "\" ) in document" );
      }
      
      m_baseStyle = style;
      m_baseStyleName = styleName;
      
      CharacterFormat.ApplyBase( style.CharacterFormat );
      ParagraphFormat.ApplyBase( style.ParagraphFormat );
      
      return m_baseStyle as ParagraphStyle;
    }
    /// <summary>
    /// Imports the style to specified document .
    /// </summary>
    /// <param name="destDoc">The destination doc.</param>
    /// <returns></returns>
    internal string ImportStyleTo( Document destDoc )
    {
      ParagraphStyle foundStyle = ( ParagraphStyle )destDoc.Styles.FindByName( this.Name );
      if( foundStyle == null  )
      {
        IStyle newStyle = this.Clone( destDoc );
        destDoc.Styles.Add( newStyle );
        if( destDoc.CurClonedSection != null )
        {
          destDoc.CurClonedSection.OldParaStylesHolder.Add( Name, Name );
        }
        return Name;
      }
      else
      {
        return this.ApplyOrImportStyleTo( destDoc );
      }
    }
    /// <summary>
    /// Applies the or import style to document.
    /// </summary>
    /// <param name="destDoc">The destination doc.</param>
    /// <returns></returns>
    internal string ApplyOrImportStyleTo( Document destDoc )
    {
      string changedStyleName = null;
      if( destDoc.CurClonedSection != null )
      {
        changedStyleName = ( string ) destDoc.CurClonedSection.OldParaStylesHolder[ this.Name ];
      }

      if( changedStyleName != null )
      {
        return changedStyleName;
      }
      else
      {
        return AddNewStyle( this, destDoc );
      }
    }
    /// <summary>
    /// Adds new style based on "sourceStyle" to the document.
    /// </summary>
    /// <param name="sourceStyle">The source style.</param>
    /// <param name="destDoc">The dest doc.</param>
    /// <returns></returns>
    internal string AddNewStyle( ParagraphStyle sourceStyle, Document destDoc )
    {
      ParagraphStyle newStyle = null;
      if( sourceStyle != null )
      {
        newStyle = ( ParagraphStyle )sourceStyle.Clone( destDoc );
        newStyle.Name = newStyle.Name + "_" + Guid.NewGuid().ToString();
        destDoc.Styles.Add( newStyle );
        if( destDoc.CurClonedSection != null )
        {
          destDoc.CurClonedSection.OldParaStylesHolder.Add( sourceStyle.Name, newStyle.Name );
        }
        return newStyle.Name;
      }
      return string.Empty;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    protected virtual void CreateFormats()
    {
      Document docEx = Document as Document;

      if( docEx != null )
      {
        m_charFormat = docEx.CreateCharacterFormatImpl();
        m_paraFormat = docEx.CreateParagraphFormatImpl();
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public override IStyle Clone( IDocument document )
    {
      return CloneImpl( document );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    protected virtual IParagraphStyle CloneImpl( IDocument document )
    {
      return new ParagraphStyle( this, document );
    }
    #endregion

    #region Class XDLSSerializable overrides
    /// <summary>
    /// 
    /// </summary>
    protected override void InitXDLSHolder()
    {
      base.InitXDLSHolder();
      XDLSHolder.AddElement( XDLSConstants.CharacterFormatTag, CharacterFormat );
      XDLSHolder.AddElement( XDLSConstants.ParagraphFormatTag, ParagraphFormat );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
//      if( reader.HasAttribute( XDLSConstants.StyleBaseNameAttr ))
//      {
//        m_baseStyleName = reader.ReadString( XDLSConstants.StyleBaseNameAttr );
//      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      writer.WriteValue( XDLSConstants.TypeTag, ( Enum )StyleType.ParagraphStyle );
//      if( m_baseStyle != null )
//      {
//        writer.WriteValue( XDLSConstants.StyleBaseNameAttr, m_baseStyle.Name );
//      }
    }
    protected override void RestoreReference(string name, int index)
    {
      if( m_baseStyleName != null && m_baseStyleName.Length != 0 )
      {
        ApplyBaseStyle( m_baseStyleName );
      }
    }

    #endregion
  }
}