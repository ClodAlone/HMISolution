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
using System.Collections;

using Syncfusion.Windows.Forms.Edit.Implementation.Parser;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface for lexem line.
  /// </summary>
  public interface ILexemLine
  {
		#region Properties
    /// <summary>
    /// ParsePoint at the beginning of the line.
    /// </summary>
    IParsePoint LineStartPoint{ get; }
    /// <summary>
    /// ParsePoint at the end of the line.
    /// </summary>
    IParsePoint LineEndPoint{ get; }
    /// <summary>
    /// Stack at the beginning of the line.
    /// </summary>
    ConfigStack LineStartStack{ get; }
    /// <summary>
    /// Stack at the end of the line. If line was not parsed, it will be reparsed.
    /// </summary>
    ConfigStack LineEndStack{ get; }
    /// <summary>
    /// Collection of all lexems, that belong to current line. If line was not parsed, it will be reparsed.
    /// </summary>
    IList LineLexems{ get; }
    /// <summary>
    /// Flag that determines, whether line is parsed. If line is parsed, than LineEndStack property contains Stack for the end of the line
    /// and LineLexems collection contains all lexems that belong to current line. If line was changed, than Parsed will be set to false.
    /// </summary>
    bool Parsed{ get; set; }
    /// <summary>
    /// Checks validity of the line. If line was already disposed, it is no longer valid.
    /// </summary>
    bool IsValid{ get; }
    /// <summary>
    /// Parser, the line belongs to.
    /// </summary>
    ILexemParser Parser{ get; }
		/// <summary>
		/// Gets length of the line.
		/// </summary>
		int LineLength{ get; }
		/// <summary>
		/// Index of the line.
		/// </summary>
		/// <remarks>
		/// It can be different from the one, stored in m_point because it also includes data from collapsing.
		/// </remarks>
		int LineIndex{ get; }
		#endregion

		#region Events
    /// <summary>
    /// Event that is raised when line is deleted. If position of the LineStartPoint is changed, than line is considered to be invalid
    /// and must be deleted. Or LineStartPoint was deleted.
    /// </summary>
    event EventHandler LineDeleted;
		#endregion

		#region Methods
    /// <summary>
    /// Deletes self. Raises LineDeleted.
    /// </summary>
    void DeleteSelf();
    /// <summary>
    /// Searches lexem, that contains given column index.
    /// </summary>
    /// <param name="column">Needed column.</param>
    /// <returns>Found lexem, or null if needed column is in virtual space.</returns>
    ILexem FindLexemByColumn( int column );
    /// <summary>
    /// Gets stack copy for the lexem at the specified column.
    /// </summary>
    /// <param name="column">Needed column.</param>
    /// <returns>Copy of the stack.</returns>
    ConfigStack GetStackByColumn( int column );
		#endregion
  }
}
