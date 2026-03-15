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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Globalization;

using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents path shape.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  public class PathShape
    : Shape,
    IWidget
	{
    #region Class members
    /// <summary>
    /// Array of points in the path.
    /// </summary>
    private ArrayList m_points;
    /// <summary>
    /// Types of the points.
    /// </summary>
    private ArrayList m_types;
    /// <summary>
    /// Determines how the interiors of shapes in this GraphicsPath object are filled.
    /// </summary>
    private FillMode m_fillMode;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets the points in the path.
    /// </summary>
    public PointF[] PathPoints
    {
      get
      {
        return ( PointF[] )m_points.ToArray( typeof( PointF ) );
      }
    }
    /// <summary>
    /// Gets the types of the corresponding points in the PathPoints array.
    /// </summary>
    public PathPointType[] PathTypes
    {
      get
      {
        return ( PathPointType[] )m_types.ToArray( typeof( PathPointType ) );
      }
    }
    /// <summary>
    /// Gets / sets FillMode enumeration that determines how the interiors
    ///  of shapes in this GraphicsPath object are filled.
    /// </summary>
    public FillMode FillMode
    {
      get
      {
        return m_fillMode;
      }
      set
      {
        if( m_fillMode != value )
        {
          m_fillMode = value;
        }
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates a new PathShape object.
    /// </summary>
    /// <param name="canvas">Parent canvas.</param>
    public PathShape( Canvas canvas )
      : base( canvas )
    {
      m_points = new ArrayList();
      m_types  = new ArrayList();
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Starts path shape.
    /// </summary>
    /// <param name="startPoint">Starting point.</param>
		/// <remarks>Use this method before filling path shape except in the case of
		/// importing data from GraphicsPath object.</remarks>
    public void Start( PointF startPoint  )
    {
      AddPoint( startPoint, PathPointType.Start );
    }
    /// <summary>
    /// Closes current figure inside the shape.
    /// </summary>
    /// <remarks>Use this method after filling path shape except in the case of
    /// importing data from GraphicsPath object.</remarks>
    public void CloseFigure()
    {
      if( m_types.Count > 0 )
      {
        PathPointType type = ( PathPointType )m_types[ m_types.Count - 1 ];
        type |= PathPointType.CloseSubpath;
        m_types[ m_types.Count - 1 ] = type;
      }
    }
    /// <summary>
    /// Adds point to the path shape.
    /// </summary>
    /// <param name="point">New point inside path.</param>
    /// <param name="pointType">Type of the point.</param>
    public void AddPoint( PointF point, PathPointType pointType )
    {
      m_points.Add( point );
      m_types.Add( pointType );
    }
    /// <summary>
    /// Creates GraphicsPath object.
    /// </summary>
    /// <param name="points">Array of points in the path.</param>
    /// <param name="types">Types of the points.</param>
    /// <param name="fillMode">Fill mode of the path.</param>
    /// <returns>Created graphics path object.</returns>
    public GraphicsPath ToGraphicsPath( PointF[] points, PathPointType[] types, FillMode fillMode )
    {
      if( points == null )
        throw new ArgumentNullException( "points" );
      if( types == null )
        throw new ArgumentNullException( "types" );

      GraphicsPath path = null;
      
      if( points.Length == types.Length && points.Length > 0 )
      {
        byte[] typesBytes = ToBytes( types );
        path = new GraphicsPath( points, typesBytes, fillMode );
      }

      return path;
    }
    /// <summary>
    /// Creates GraphicsPath object.
    /// </summary>
    public GraphicsPath ToGraphicsPath()
    {
      return ToGraphicsPath( this.PathPoints, this.PathTypes, this.FillMode );
    }
    /// <summary>
    /// Converts graphics path object to PathShape.
    /// </summary>
    /// <param name="path">Graphics Path object.</param>
    /// <remarks>All data of the PathShape, stored before will be lost.</remarks>
    public void FromGraphicsPath( GraphicsPath path )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );

      // Clear data.
      m_points.Clear();
      m_types.Clear();

      this.FillMode = path.FillMode;
      
      m_points.AddRange( path.PathPoints );

      PathPointType[] types = FromBytes( path.PathTypes );
      m_types.AddRange( types );
    }
    #endregion

    #region XML serialization overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      
      writer.WriteValue( PropertyNames.Type, ShapeType.Path );
      writer.WriteValue( PropertyNames.FillMode, this.FillMode );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      
      this.FillMode = ( FillMode ) reader.ReadEnum( PropertyNames.FillMode,
        typeof( FillMode ) );
    }
    /// <summary>
    /// Overloaded. Writes image to XML.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlContent( IXDLSContentWriter writer )
    {
      base.WriteXmlContent( writer );

      if( m_points.Count == m_types.Count && m_points.Count > 0 )
      {
        XmlWriter xmlWriter = writer.InnerWriter;
        xmlWriter.WriteStartElement( PropertyNames.Items );

        // Write collection of points and their types.
        for( int i = 0, len = m_points.Count; i < len; i++ )
        {
          PointF curPoint = ( PointF )m_points[ i ];
          PathPointType curType = ( PathPointType )m_types[ i ];

          xmlWriter.WriteStartElement( PropertyNames.Item );

          string typeVal = curType.ToString();
          xmlWriter.WriteAttributeString( PropertyNames.Type, typeVal );
          
          string numVal = curPoint.X.ToString( CultureInfo.InvariantCulture );
          xmlWriter.WriteAttributeString( PropertyNames.X, numVal );

          numVal = curPoint.Y.ToString( CultureInfo.InvariantCulture );
          xmlWriter.WriteAttributeString( PropertyNames.Y, numVal );
          
          xmlWriter.WriteEndElement();
        }

        xmlWriter.WriteEndElement();
      }
    }
    /// <summary>
    /// Overloaded. Reads image from the XML.
    /// </summary>
    /// <param name="reader">Rader object.</param>
    /// <returns>True always.</returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override bool ReadXmlContent( IXDLSContentReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      bool result = base.ReadXmlContent( reader );

      if( reader.TagName == PropertyNames.Items )
      {
        XmlReader xmlReader = reader.InnerReader;
        bool isEmptyItems = xmlReader.IsEmptyElement;
        xmlReader.ReadStartElement();
        int depth = xmlReader.Depth;
        
        if( !isEmptyItems )
        {
          while( depth == xmlReader.Depth )
          {
            if( xmlReader.NodeType != XmlNodeType.Element )
            {
              xmlReader.Read();
              continue;
            }

            if( xmlReader.LocalName == PropertyNames.Item )
            {
              ReadPoint( reader as XDLSReader );
              xmlReader.Read();
            }
          }
        }
        
        result = ( m_points.Count > 0 );
      }

      return result;
    }
    #endregion
    
    #region WidgetBase override
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      ApplyTransform( cg );
      ( cg as DLSGraphics ).DrawPathShape( this, ltWidget );
      ResetTransform( cg );
    }
    #endregion

    #region Class utility methods
    /// <summary>
    /// Reads point and it's type from XML.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private void ReadPoint( XDLSReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      // Read collection of points and types.
      if( reader.TagName == PropertyNames.Item )
      {
        PathPointType pointType = ( PathPointType )reader.ReadEnum( PropertyNames.Type,
          typeof( PathPointType ) );

        float x = reader.ReadFloat( PropertyNames.X );
        float y = reader.ReadFloat( PropertyNames.Y );
        PointF point = new PointF( x, y );

        // Add to collection.
        m_points.Add( point );
        m_types.Add( pointType );
      }
    }
    /// <summary>
    /// Converts array of point types to byte array.
    /// </summary>
    /// <param name="types">Array of point types.</param>
    /// <returns>Array of bytes.</returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private byte[] ToBytes( PathPointType[] types )
    {
      if( types == null )
        throw new ArgumentNullException( "types" );
      
      byte[] typesByte = new byte[ types.Length ];
      
      for( int i = 0, len = types.Length; i < len; i++ )
      {
        typesByte[ i ] = ( byte )types[ i ];
      }

      return typesByte;
    }
    /// <summary>
    /// Converts array of point types from byte array.
    /// </summary>
    /// <param name="typesByte">Array of bytes.</param>
    /// <returns>Array of point types.</returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private PathPointType[] FromBytes( byte[] typesByte )
    {
      if( typesByte == null )
        throw new ArgumentNullException( "typesByte" );
      
      PathPointType[] types = new PathPointType[ typesByte.Length ];
      
      for( int i = 0, len = typesByte.Length; i < len; i++ )
      {
        types[ i ] = ( PathPointType )typesByte[ i ];
      }

      return types;
    }
    #endregion
	}
}
