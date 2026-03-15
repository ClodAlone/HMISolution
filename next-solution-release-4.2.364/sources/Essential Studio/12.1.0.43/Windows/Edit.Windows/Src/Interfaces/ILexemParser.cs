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
using System.Text;

using Syncfusion.IO;
using Syncfusion.Windows.Forms.Edit;

using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Utils;
#endregion

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface declare base functionality of each Lexem parser for our control.
  /// </summary>
  public interface ILexemParser
  {
    /// <summary>
    /// Collection of availabe formats. Reference on base collection of formats.
    /// </summary>
    IFormatManager Formats{ get; }
    /// <summary>
    /// Reference on base stream which used as source for Parser
    /// </summary>
    StreamsWrapper BaseStream{ get; }
    /// <summary>
    /// Quantity of lines in input stream
    /// </summary>
    int TotalLines{ get; }
    /// <summary>
    /// Current line index. This property can be used for fast move to needed
    /// line in file.
    /// </summary>
    int CurrentLine{ get; set; }
    /// <summary>
    /// Read next line from input stream and set CurrentLine index to new value
    /// </summary>
    /// <returns>array of lexems in line, null if end of stream reached</returns>
    IList NextLine();
    /// <summary>
    /// Read previous line from input stream and set CurrentLine index to new value.
    /// When method reach start of stream it will return first line lexems only and
    /// will not change CurrentLine property to new value.
    /// </summary>
    /// <returns>array of lexems</returns>
    IList PreviousLine();
    /// <summary>
    /// Get current line lexems.
    /// </summary>
    /// <returns>lexems array of current line</returns>
    IList GetLine();
    /// <summary>
    /// Gets <see cref="CoordinatePoint"/> class instance, that represents
    /// some coordinates in stream.
    /// </summary>
    /// <param name="iLine">Virtual line.</param>
    /// <param name="iColumn">Virtual column.</param>
    /// <returns><see cref="CoordinatePoint"/> class instance.</returns>
    CoordinatePoint GetCoordinatePoint( int iLine, int iColumn );
    /// <summary>
    /// Gets <see cref="CoordinatePoint"/> by given
    ///  <see cref="IParsePoint"/>.
    /// </summary>
    /// <param name="point"><see cref="IParsePoint"/> that points
    /// to physical position in stream.</param>
    /// <param name="bRedirectToStart"></param>
    /// <returns><see cref="CoordinatePoint"/> that points to the given position.</returns>
    CoordinatePoint GetCoordinatePoint( IParsePoint point, bool bRedirectToStart );
    /// <summary>
    /// Gets <see cref="CoordinatePoint"/> by given
    ///  <see cref="IParsePoint"/>.
    /// </summary>
    /// <param name="point"><see cref="IParsePoint"/> that points
    /// to physical position in stream.</param>
    /// <returns><see cref="CoordinatePoint"/> that points to the given position.</returns>
    CoordinatePoint GetCoordinatePoint( IParsePoint point );
		/// <summary>
		/// Gets <see cref="CoordinatePoint"/> by given
		///  <see cref="IParsePoint"/>.
		/// </summary>
		/// <param name="point"><see cref="IParsePoint"/> that points
		/// to physical position in stream.</param>
		/// <param name="bRedirectToStart"></param>
		/// <param name="bTrackPosition">Specifies whether coordinate point should track position.</param>
		/// <returns><see cref="CoordinatePoint"/> that points to the given position.</returns>
		CoordinatePoint GetCoordinatePoint( IParsePoint point, bool bRedirectToStart, bool bTrackPosition );
		/// <summary>
    /// Searches for the <see cref="Syncfusion.Windows.Forms.Edit.Interfaces.IParsePoint"/>
    /// at the given position.
    /// </summary>
    /// <remarks>
    /// If it can not be found (it is in virtual space), then
    /// you will get parse point, pointing to the beginning of the
    /// next line. If it can not be done, ParsePoint, pointing to
    /// the end of current line will be returned.
    /// </remarks>
    /// <param name="iLine">Line index, the ParsePoint is needed for.</param>
    /// <param name="iColumn">Column index, the ParsePoint is needed for. Can be 0.</param>
    /// <returns>ParsePoint to the given position.</returns>
    CoordinatePoint GetNearestParsePointRight( int iLine, int iColumn );
    /// <summary>
    /// Searches for the <see cref="Syncfusion.Windows.Forms.Edit.Interfaces.IParsePoint"/>
    /// at the given position.
    /// </summary>
    /// <remarks>
    /// If it can not be found ( or column is 0), and 
    /// if it is in virtual space, then you will get parse point
    /// to the end of given line; If column is 0, then you will get
    /// parse point to the end of the previous line( if it is one ).
    /// </remarks>
    /// <param name="iLine">Line index the ParsePoint is needed for.</param>
    /// <param name="iColumn">Column index the ParsePoint is needed for. Can be 0.</param>
    /// <returns>ParsePoint to the given position.</returns>
    CoordinatePoint GetNearestParsePointLeft( int iLine, int iColumn );
    /// <summary>
    /// Creates enumerator of lexems.
    /// </summary>
    /// <param name="stack">Stack for the current position.</param>>
    /// <returns></returns>
    IEnumerator GetEnumerator( ConfigStack stack );
    /// <summary>
    /// Creates enumerator of lexems.
    /// </summary>
    /// <param name="stack">Stack for the current position.</param>>
    /// <param name="point">New current position.</param>
    /// <returns></returns>
    IEnumerator GetEnumerator( ConfigStack stack, IParsePoint point );
    /// <summary>
    /// Creates enumerator of the lexem lines.
    /// </summary>
    /// <param name="line">Starting line.</param>
    /// <returns>Enumerator.</returns>
    IEnumerator GetLineEnumerator( ILexemLine line );
    /// <summary>
    /// Creates enumerator of the lexem lines.
    /// </summary>
    /// <returns>Enumerator.</returns>
    IEnumerator GetLineEnumerator();
  }
}