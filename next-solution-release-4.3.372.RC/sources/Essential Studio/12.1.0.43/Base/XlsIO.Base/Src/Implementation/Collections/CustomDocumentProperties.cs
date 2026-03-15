#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Collections;

//using Syncfusion.XlsIO.IO.Stream.Win32;
using Syncfusion.CompoundFile.XlsIO.Native;
using Syncfusion.CompoundFile.XlsIO;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.CompoundFile.XlsIO.Net;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
	/// <summary>
	/// Summary description for CustomDocumentProperties.
	/// </summary>
	public class CustomDocumentProperties
    : CollectionBaseEx<DocumentPropertyImpl>
    , ICustomDocumentProperties
  {
    #region Class constants
    /// <summary>
    /// Custom guid string.
    /// </summary>
    public const string CustomGuidString = "D5CDD505-2E9C-101B-9397-08002B2CF9AE";
    #endregion

    #region Class static members
    /// <summary>
    /// Guid used for parsing/serialization of custom properties.
    /// </summary>
    public static readonly Guid GuidCustom = new Guid( CustomGuidString );
    #endregion

    #region Class members
    /// <summary>
    /// Dictionary with document properties, key - property name/id, value - property value.
    /// </summary>
    private Dictionary<string, DocumentPropertyImpl> m_propertiesHash = new Dictionary<string, DocumentPropertyImpl>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CustomDocumentProperties( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    public IDocumentProperty this[ string strName ]
    {
      get
      {
        IDocumentProperty property = GetProperty( strName );

        return ( property != null ) ? property : Add( strName );
      }
    }
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    IDocumentProperty ICustomDocumentProperties.this[ int iIndex ]
    {
      get
      {
        if( iIndex < 0 || iIndex > Count - 1 )
        {
          throw new ArgumentOutOfRangeException( "iIndex",
            "Value cannot be less than 0 and greater than than Count - 1." );
        }

        return InnerList[ iIndex ];
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Returns custom property by name.
    /// </summary>
    /// <param name="strName">Custom property.</param>
    /// <returns>Custom property.</returns>
    public IDocumentProperty GetProperty( string strName )
    {
      DocumentPropertyImpl result;
      m_propertiesHash.TryGetValue( strName, out result );

      return result;
    }
    /// <summary>
    /// Removes specified object from the collection.
    /// </summary>
    /// <param name="strName">Property name.</param>
    public void Remove( string strName )
    {
      DocumentPropertyImpl result;
      if( m_propertiesHash.TryGetValue( strName, out result ) )
      {
        m_propertiesHash.Remove( strName );
        base.Remove( result );
      }
    }
    /// <summary>
    /// Adds element to the collection.
    /// </summary>
    /// <param name="strName">Property name to add.</param>
    /// <returns>Newly created property.</returns>
    public IDocumentProperty Add( string strName )
    {
      DocumentPropertyImpl property = new DocumentPropertyImpl( strName, null );
      m_propertiesHash.Add( strName, property );
      base.Add( property );
      return property;
    }
    /// <summary>
    /// Checks whether collection contains property with specified name.
    /// </summary>
    /// <param name="strName">Name to check.</param>
    /// <returns>True if property is contained by collection; false otherwise.</returns>
    public bool Contains( string strName )
    {
      return m_propertiesHash.ContainsKey( strName );
    }
    /// <summary>
    /// Serializes built-in properties.
    /// </summary>
    /// <param name="setProp">IPropertySetStorage to serialize into.</param>
    [CLSCompliant( false )]
    public void Serialize( PropertySection section )
    {
      BuiltInDocumentProperties.WriteProperties( section, m_propertiesHash.Values );
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Serializes built-in properties.
    /// </summary>
    /// <param name="setProp">IPropertySetStorage to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( IPropertySetStorage setProp )
    {
      BuiltInDocumentProperties.WriteProperties( setProp, GuidCustom, m_propertiesHash.Values );
    }
    /// <summary>
    /// Extract built-in document properties.
    /// </summary>
    /// <param name="setProp">IPropertySetStorage to extract properties from.</param>
    [ CLSCompliant( false ) ]
    public void Parse( IPropertySetStorage setProp )
    {
      BuiltInDocumentProperties.ReadProperties( setProp, GuidCustom, m_propertiesHash, List, true, false );
    }
#endif
    /// <summary>
    /// Extract custom document properties.
    /// </summary>
    /// <param name="setProp">Collection to extract properties from.</param>
    [CLSCompliant( false )]
    public void Parse( DocumentPropertyCollection properties )
    {
      List<PropertySection> lstSections = properties.Sections;

      for( int i = 0, len = lstSections.Count; i < len; i++ )
      {
        PropertySection section = lstSections[ i ];

        if( section.Id == GuidCustom )
        {
          BuiltInDocumentProperties.ReadProperties( section, m_propertiesHash, InnerList, true, false );
        }
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// OnClear is invoked after Clear behavior.
    /// </summary>
    protected override void OnClearComplete()
    {
      m_propertiesHash.Clear();
    }

    #endregion
  }
}
