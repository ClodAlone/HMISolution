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
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Interface for DynamicFormatsLayer.
	/// </summary>
	public interface IDynamicFormatsLayer
    : ICollection
    , IEnumerable
	{
    /// <summary>
    /// Gets value that specifies whether this layer is hidden.
    /// </summary>
    bool Hidden{ get; }
    /// <summary>
    /// Creates new dynamic format object and adds it to the list.
    /// All existing dynamic formatting in specified range will be deleted or trimmed.
    /// </summary>
    /// <param name="start">Starting positions.</param>
    /// <param name="end">End positions.</param>
    /// <param name="format">Format to be added.</param>
    /// <returns>Newly created format.</returns>
    IDynamicFormat Add( CoordinatePoint start, CoordinatePoint end, ISnippetFormat format );
    /// <summary>
    /// Removes all formatting in specified range.
    /// </summary>
    /// <param name="start">Start of the range.</param>
    /// <param name="end">End of the range.</param>
    void Remove( CoordinatePoint start, CoordinatePoint end );
    /// <summary>
    /// Removes given formatting.
    /// </summary>
    /// <param name="format">Formatting to be deleted.</param>
    void Remove( IDynamicFormat format );
    /// <summary>
    /// Removes formatting by given index.
    /// </summary>
    /// <param name="index">Index of the formatting to be removed.</param>
    void RemoveAt( int index );
    /// <summary>
    /// Returns index of the dynamic formatting.
    /// </summary>
    /// <param name="format">Formatting to be found.</param>
    /// <returns>Index of the formatting in the internal list.</returns>
    int IndexOf( IDynamicFormat format );
		/// <summary>
		/// Checks whether coordinate point belongs to current layer.
		/// </summary>
		/// <param name="point">Point to check.</param>
		/// <param name="bIncludeAfter">If true, point at the beginning of region is considered as belonging to layer.</param>
		/// <param name="bIncludeBefore">If true, point at the end of region is considered as belonging to layer.</param>
		/// <returns>Bool indicating whether given point belongs to current layer.</returns>
		bool PointInLayer( CoordinatePoint point, bool bIncludeBefore, bool bIncludeAfter );
    /// <summary>
    /// GET list of dynamic formatting, that intersepts with given range.
    /// </summary>
    IList this[ CoordinatePoint start, CoordinatePoint end ]{ get; }
    /// <summary>
    /// GET dynamic formatting by ParsePoint.
    /// </summary>
    IDynamicFormat this[ CoordinatePoint point ]{ get; }
    /// <summary>
    /// GET formatting by index.
    /// </summary>
    IDynamicFormat this[ int index ]{ get; }
    /// <summary>
    /// Event that is raised when data somethins is changed whithin the layer.
    /// </summary>
    event EventHandler DataChanged;
		/// <summary>
		/// Clears all formatting in layer.
		/// </summary>
		void Clear();
  }
}
