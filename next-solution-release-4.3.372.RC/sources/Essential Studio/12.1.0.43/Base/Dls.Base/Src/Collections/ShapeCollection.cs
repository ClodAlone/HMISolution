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

using System;

using Syncfusion.DLS.XML;

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Represents shape collection
  /// </summary>
  public class ShapeCollection : EntityCollectionBase
  {
    #region Class properties
    /// <summary>
    /// Get Shape by index
    /// </summary>
    public Shape this[ int index ]
    {
      get
      {
        return ( Shape )List[ index ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="canvas"></param>
    protected internal ShapeCollection( Canvas canvas )
      : base( canvas )
    {
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds Shape to collection
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public int Add( Shape shape )
    {
      return List.Add( shape );
    }
    /// <summary>
    /// Checks if shape already exists in the collection.
    /// </summary>
    /// <param name="shape">Shape object.</param>
    /// <returns>True - if shape already exists in the collection, False otherwise.</returns>
    public bool Contains( Shape shape )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      return List.Contains( shape );
    }
    /// <summary>
    /// Removes shape from the collection.
    /// </summary>
    /// <param name="shape">Shape object.</param>
    public void Remove( Shape shape )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      List.Remove( shape );
    }
    /// <summary>
    /// Inserts shape into collection at the specified index.
    /// </summary>
    /// <param name="index">Index of the shape in the collection.</param>
    /// <param name="shape">Shape to be inserted in the collection.</param>
    public void Insert( int index, Shape shape )
    {
      if( index < 0 || index > List.Count )
        throw new ArgumentOutOfRangeException( "index", index, "Value can not be less 0 and greater Count" );
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      List.Insert( index, shape );
    }
    /// <summary>
    /// Returns index of the shape object in the collection.
    /// </summary>
    /// <param name="shape">Shape object.</param>
    /// <returns>Index of the shape object in the collection.</returns>
    public int IndexOf( Shape shape )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      return List.IndexOf( shape );
    }
    #endregion

    #region IDLSXmlEntityCollection implements
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object CreateItem( IXDLSContentReader reader )
    {
      string shapeType = reader.GetAttributeValue( PropertyNames.Type );
      object shape = Enum.Parse( typeof( ShapeType ), shapeType, true );

      return Document.CreateShape( ( ShapeType )shape, Owner as Canvas );
    }
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.ShapeItemTag;
      }
    }
    #endregion
  }
}