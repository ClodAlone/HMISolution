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
using System.Drawing;
using System;
using System.Collections;
using System.Globalization;
using System.Xml;

using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a polygon shape.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  public class PolygonShape
    : Shape,
    IWidget
  {
    #region Class members
    /// <summary>
    /// Points of the polygon.
    /// </summary>
    private ArrayList m_points;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets X points of polygon.
    /// </summary>
    public PointF[] Points
    {
      get
      {
        return ( PointF[] )m_points.ToArray( typeof( PointF ) );
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="canvas"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public PolygonShape( Canvas canvas )
      : base( canvas )
    {
      m_points = new ArrayList();
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Adds point to the polygon.
    /// </summary>
    /// <param name="point">New point for the polygon.</param>
    public void Add( PointF point )
    {
      m_points.Add( point );
    }
    /// <summary>
    /// Adds points to the polygon.
    /// </summary>
    /// <param name="points">Array of points.</param>
    public void AddRange( PointF[] points )
    {
      m_points.AddRange( points );
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
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      base.WriteXmlAttributes( writer );
      
      writer.WriteValue( PropertyNames.Type, ShapeType.Polygon );
    }
    /// <summary>
    /// Writes points to XML.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlContent( IXDLSContentWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      base.WriteXmlContent( writer );

      if( m_points != null && m_points.Count > 0 )
      {
        XmlWriter xmlWriter = writer.InnerWriter;

        xmlWriter.WriteStartElement( PropertyNames.Points );

        // Write points to XML.
        for( int i = 0, len = m_points.Count; i < len; i++ )
        {
          xmlWriter.WriteStartElement( PropertyNames.Point );

          PointF curPoint = ( PointF )m_points[ i ];
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
    /// Reads element from XML.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    /// <returns>True.</returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override bool ReadXmlContent( IXDLSContentReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );
      
      bool result = base.ReadXmlContent( reader );

      if( reader.TagName == PropertyNames.Points )
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

            if( xmlReader.LocalName == PropertyNames.Point )
            {
              ReadPoint( reader as XDLSReader );
              xmlReader.Read();
            }
          }
        }
        
        result = ( m_points.Count > 0 );
      }

      return result;

      // Read collection of points and types.
      /*XmlReader xmlReader = reader.InnerReader;
      
      bool hasChildElements = !xmlReader.IsEmptyElement;
      int currDepth = xmlReader.Depth;

      if( hasChildElements )
      {
        m_points.Clear();
        
        xmlReader.Read();
        xmlReader.ReadStartElement();
        
        while( xmlReader.Depth > currDepth && !xmlReader.EOF )
        {
          // Skips if node is not Element type or it's just holder of items.
          if( xmlReader.NodeType != XmlNodeType.Element || 
            xmlReader.LocalName == PropertyNames.Points )
          {
            xmlReader.Read();
            continue;
          }

          if( xmlReader.LocalName == PropertyNames.Point )
          {
            ReadPoint( reader as XDLSReader );
          }

          xmlReader.Read();
        }

        // Exit from Items node.
        xmlReader.Read();
      }

      return true;*/
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
      ( cg as DLSGraphics ).DrawPolygonShape( this, ltWidget );
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
      XmlReader xmlReader = reader.InnerReader;

      if( xmlReader.LocalName == PropertyNames.Point )
      {
        float x = reader.ReadFloat( PropertyNames.X );
        float y = reader.ReadFloat( PropertyNames.Y );
        PointF point = new PointF( x, y );

        // Add to collection.
        m_points.Add( point );
      }
    }
    #endregion
	}
}
