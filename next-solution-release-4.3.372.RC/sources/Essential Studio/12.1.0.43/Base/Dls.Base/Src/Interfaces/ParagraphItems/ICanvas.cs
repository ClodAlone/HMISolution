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

using System.Drawing;

using Syncfusion.DLS.Collections;

namespace Syncfusion.DLS
{
  /// <summary>
  /// Interface publishes Canvas functionality
  /// </summary>
  public interface ICanvas : IParagraphItem
  {
    /// <summary>
    /// Gets/sets border color.
    /// </summary>
    Color BorderColor{ get; set; }
    /// <summary>
    /// Gets/sets width of the border.
    /// </summary>
    float Width{ get; set; }
    /// <summary>
    /// Gets/sets height of canvas.
    /// </summary>
    float Height{ get; set; }
    /// <summary>
    /// Gets / sets size of the canvas.
    /// </summary>
    SizeF Size{ get; set; }
    /// <summary>
    /// Gets shapes of the canvas.
    /// </summary>
    ShapeCollection Shapes{ get; }
    /// <summary>
    /// Creates shape object by it's type and adds it to collection.
    /// </summary>
    /// <returns>Created shape object.</returns>
    /// <remarks>You have to set coordinates of this shape to corrrectly
    /// display this shape on the canvas.</remarks>
    Shape AddShape( ShapeType shapeType );
    /// <summary>
    /// Creates Arc shape and adds it to collection.
    /// </summary>
    /// <returns> Created Arc shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    ArcShape AddArc();
    /// <summary>
    /// Creates Bezier curve shape and adds it to collection.
    /// </summary>
    /// <returns> Created Bezier curve shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    BezierShape AddBezier();
    /// <summary>
    /// Creates Ellipse shape and adds it to collection.
    /// </summary>
    /// <returns> Created Ellipse shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    EllipseShape AddEllipse();
    /// <summary>
    /// Creates Image shape and adds it to collection.
    /// </summary>
    /// <param name="image">Image object.</param>
    /// <returns> Created Image shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    ImageShape AddImage( Image image );
    /// <summary>
    /// Creates Line shape and adds it to collection.
    /// </summary>
    /// <returns> Created Line shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    LineShape AddLine();
    /// <summary>
    /// Creates Path shape and adds it to collection.
    /// </summary>
    /// <returns> Created Path shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    PathShape AddPath();
    /// <summary>
    /// Creates Pie shape and adds it to collection.
    /// </summary>
    /// <returns> Created Pie shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    PieShape AddPie();
    /// <summary>
    /// Creates Polygon shape and adds it to collection.
    /// </summary>
    /// <returns> Created Polygon shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    PolygonShape AddPolygon();
    /// <summary>
    /// Creates Rectangle shape and adds it to collection.
    /// </summary>
    /// <returns> Created Rectangle shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    RectangleShape AddRectangle();
    /// <summary>
    /// Creates Text shape and adds it to collection.
    /// </summary>
    /// <param name="text">Text data of the shape.</param>
    /// <returns> Created Text shape.</returns>
		/// <remarks>You have to set coordinates of this shape to corrrectly
		/// display this shape on the canvas.</remarks>
    TextShape AddText( string text );
  }
}