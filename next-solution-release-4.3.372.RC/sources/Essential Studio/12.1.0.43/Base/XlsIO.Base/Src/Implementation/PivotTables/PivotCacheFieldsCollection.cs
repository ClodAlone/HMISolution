#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for PivotCacheFieldsCollection.
  /// </summary>
  public class PivotCacheFieldsCollection : CollectionBase<PivotCacheFieldImpl>
  {
      #region Properties
      public PivotCacheFieldImpl this[string name]
      {
          get
          {
              foreach (PivotCacheFieldImpl table in InnerList)
              {
                  if (table.Name == name)
                  {
                      return table;
                  }
              }
              return null;
          }
      }
      #endregion
      #region Class Initialize/Finalize methods
      /// <summary>
    /// Initializes new instance of the pivot cache fields collection.
    /// </summary>
    public PivotCacheFieldsCollection()
    {
    }
    #endregion

    #region Class serialization / parse methods
    /// <summary>
    /// Reads collection records from the BiffReader.
    /// </summary>
    /// <param name="reader">BiffReader to get records from.</param>
    /// <param name="iFieldsNumber">Number of fields to read.</param>
    [ CLSCompliant( false ) ]
    public void Parse( BiffReader reader, int iFieldsNumber )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      for( int i = 0; i < iFieldsNumber; i++ )
      {
        TBIFFRecord recordType = reader.PeekRecordType();

        if( recordType != TBIFFRecord.PivotField )
          throw new UnexpectedRecordException();

        PivotCacheFieldImpl field = new PivotCacheFieldImpl( reader );
        Add( field );
      }
    }
    /// <summary>
    /// Saves collection as Biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList that should get all collection's records.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      for( int i = 0, len = Count; i < len; i++ )
      {
        PivotCacheFieldImpl field = this[ i ];
        field.Serialize( records );
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds new field to the collection.
    /// </summary>
    /// <param name="field">Field to add.</param>
    /// <returns>Index of the newly added field.</returns>
    public int Add( PivotCacheFieldImpl field )
    {
      if( field == null )
        throw new ArgumentNullException( "field" );

      base.Add( field );
      int result = Count - 1;
      field.Index = result;
      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strName"></param>
    /// <returns></returns>
    public PivotCacheFieldImpl AddNewField( string strName )
    {
      if( strName == null )
        throw new ArgumentNullException( "strName" );

      if( strName.Length == 0 )
        throw new ArgumentException( "strName - string cannot be empty" );

      PivotCacheFieldImpl field = new PivotCacheFieldImpl();
      field.Name = strName;
      Add( field );
      return field;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strName"></param>
    /// <returns></returns>
    public PivotCacheFieldImpl AddNewField(string strName,string formula)
    {
        if (strName == null)
            throw new ArgumentNullException("strName");

        if (strName.Length == 0)
            throw new ArgumentException("strName - string cannot be empty");

        PivotCacheFieldImpl field = new PivotCacheFieldImpl();
        field.Name = strName;
        //TODO: Validate the formula
        field.Formula = formula;
        Add(field);
        return field;
    }
    public int GetOrdinaryFieldCount()
    {
        int ifieldCount = 0;
        foreach (PivotCacheFieldImpl field in InnerList)
        {
            bool countable = true;

            if (field.IsFieldGroup)
                countable = (field.FieldGroup.IsDiscrete)?false:true;
            
            if (!field.IsFormulaField && countable)
                ifieldCount++;
        }
        return ifieldCount;
    }
    #endregion
  }
}
