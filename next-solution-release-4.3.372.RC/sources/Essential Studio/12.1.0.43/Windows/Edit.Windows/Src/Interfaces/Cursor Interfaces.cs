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
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Virtual coordinates of the cursor.
  /// Collapsings can change virtual coordinates.
  /// </summary>
  public interface ICursorVirtualCoordinates
  {
    /// <summary>
    /// Gets or sets cursor's line index.
    /// </summary>
    int Line{ get; set; }
    /// <summary>
		/// Gets or sets cursor's column index.
    /// </summary>
    int Column{ get; set; }
    /// <summary>
		/// Gets or sets cursor's position.
    /// </summary>
    Point Position{ get; set; }
    /// <summary>
    /// Event, raised when position of the cursor was changed.
    /// </summary>
    event EventHandler CoordinatesChanged;
  }
  /// <summary>
  /// Physical coordinates of the cursor in text.
  /// </summary>
  public interface ICursorPhysicalCoordinates
  {
    /// <summary>
		/// Gets or sets cursor's line index.
    /// </summary>
    int Line{ get; }
    /// <summary>
		/// Gets or sets cursor's column index.
    /// </summary>
    int Column{ get; }
    /// <summary>
		/// Gets or sets cursor's position.
    /// </summary>
    IParsePoint Position{ get; set; }
  }

  /// <summary>
  /// Graphical coordinates of the cursor.
  /// </summary>
  public interface ICursorGraphicalCoordinates
  {
    /// <summary>
		/// Gets or sets left-top point of the cursor in client coordinates.
    /// </summary>
    Point LeftTop{ get; set; }
    /// <summary>
		/// Gets size of the cursor.
    /// </summary>
    Size Size{ get; set; }
    /// <summary>
		/// Gets rectangle, occupied by cursor.
    /// </summary>
    Rectangle Rectangle{ get; }
  }
  /// <summary>
  /// Manager of the cursor.
  /// </summary>
  public interface ICursorManager
  {
    /// <summary>
		/// Gets or sets visibility of the cursor.
    /// </summary>
    bool Visible{ get; set; }
    /// <summary>
		/// Gets control, that is the owner of the cursor and control's it's visibility.
    /// </summary>
    Control Owner{ get; }
    /// <summary>
    /// Updates cursor's parameters.
    /// </summary>
    void Update();
    /// <summary>
		/// Gets virtual coordinates of the cursor.
    /// </summary>
    ICursorVirtualCoordinates CursorVirtualCoordinates{ get; }
    /// <summary>
		/// Gets physical coordinates of the cursor. 
    /// </summary>
    ICursorPhysicalCoordinates CursorPhysicalCoordinates{ get; }
    /// <summary>
		/// Gets graphical coordinates of the cursor.
    /// </summary>
    ICursorGraphicalCoordinates CursorGraphicalCoordinates{ get; }
    /// <summary>
    /// Converter of the positions.
    /// </summary>
    IPositionConverter PositionConverter{ get; }
  }

  /// <summary>
  /// Converter of the positions.
  /// </summary>
  public interface IPositionConverter
  {
    /// <summary>
    /// Converts virtual position to physical.
    /// </summary>
    /// <param name="point">Virtual position.</param>
    /// <returns>Physical position.</returns>
    IParsePoint VirtualToPhysical( Point point );
    /// <summary>
    /// Converts physical position to virtual.
    /// </summary>
    /// <remarks>
    /// Note for implementing: if given physical coordinates can not be directly 
    /// mapped to virtual, remapping of the coordinates must be done 
    /// to make this mapping possible. Example: if coordinates 
    /// belongs to collapsed region, it must be uncollapsed.
    /// </remarks>
    /// <param name="point">Physical position.</param>
    /// <returns>Virtual position.</returns>
    Point PhysicalToVirtual( IParsePoint point );
    /// <summary>
    /// Converts graphical point to virtual.
    /// </summary>
    /// <param name="point">Graphical point.</param>
    /// <returns>Virtual point.</returns>
    Point GraphicalToVirtual( Point point );
		/// <summary>
		/// Converts graphical point to virtual.
		/// </summary>
		/// <param name="point">Graphical point.</param>
		/// <param name="allowWhiteSpace">Specifies if whitespace after last character in line should be treated like regular characters.</param>
		/// <returns>Virtual point.</returns>
		Point GraphicalToVirtual( Point point, bool allowWhiteSpace );
		/// <summary>
    /// Converts virtual point to graphical cursor coordinates.
    /// </summary>
    /// <param name="point">Virtual point.</param>
    /// <returns>Rectangle of the cursor.</returns>
    RectangleF VirtualToGraphical( Point point );
    /// <summary>
    /// Corrects virtual coordinates.
    /// </summary>
    /// <param name="point">Virtual coordinates to be corrected.</param>
    /// <param name="virtualSpaceEnabled">Specifies whether virtual space is enabled.</param>
    /// <returns>Virtual point with correct coordinates.</returns>
    Point CorrectVirtual( Point point, bool virtualSpaceEnabled );
  } 
}