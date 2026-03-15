#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Represents worksheet custom property.
	/// </summary>
	public class WorksheetCustomProperty
    : ICustomProperty
    , ICloneable
	{
    #region Class members
    /// <summary>
    /// Low level record.
    /// </summary>
    private CustomPropertyRecord m_record;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    private WorksheetCustomProperty()
    {
    }
    /// <summary>
    /// Creates new instance of the class.
    /// </summary>
    /// <param name="strName">Name of the new property.</param>
    public WorksheetCustomProperty( string strName )
    {
      m_record = ( CustomPropertyRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.CustomProperty );

      m_record.Name = strName;
    }
    /// <summary>
    /// Creates new instance of the class.
    /// </summary>
    /// <param name="property">Low level property data.</param>
    [ CLSCompliant( false ) ]
    public WorksheetCustomProperty( CustomPropertyRecord property )
    {
      if( property == null )
        throw new ArgumentNullException( "property" );

      m_record = property;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns name of the property. Read-only.
    /// </summary>
    public string Name
    {
      get
      {
        return m_record.Name;
      }
    }
    /// <summary>
    /// Gets / sets value of the property.
    /// </summary>
    public string Value
    {
      get
      {
        return m_record.Value;
      }
      set
      {
        m_record.Value = value;
      }
    }
    #endregion

    #region Class parse / serialization methods
    /// <summary>
    /// Serializes property into list of Biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( m_record );
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public object Clone()
    {
      WorksheetCustomProperty result = ( WorksheetCustomProperty )MemberwiseClone();
      result.m_record = ( CustomPropertyRecord )CloneUtils.CloneCloneable( m_record );

      return result;
    }

    #endregion
  }
}
