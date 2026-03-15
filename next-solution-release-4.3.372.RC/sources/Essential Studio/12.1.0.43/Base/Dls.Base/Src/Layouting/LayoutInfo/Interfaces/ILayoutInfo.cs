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

namespace Syncfusion.Layouting
{
	/// <summary>
	/// Summary description for ILayoutInfo.
	/// </summary>
	public interface ILayoutInfo
	  : ILayoutSpacingsInfo
	{
    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether this instance is clipped.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is clipped; otherwise, <c>false</c>.
    /// </value>
    bool IsClippedVertical
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is clipped horizontal.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is clipped horizontal; otherwise, <c>false</c>.
    /// </value>
    bool IsClippedHorizontal
    {
      get;
      set;
    }
    /// <summary>
    /// Gets a value indicating whether this instance is skip.
    /// </summary>
    /// <value><c>true</c> if this instance is skip; otherwise, <c>false</c>.</value>
    bool IsSkip
    {
      get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is skip bottom align.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is skip bottom align; otherwise, <c>false</c>.
    /// </value>
    bool IsSkipBottomAlign
    {
      get;
      set;
    }
    /// <summary>
    /// Gets a value indicating whether this instance is line container.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is line container; otherwise, <c>false</c>.
    /// </value>
    bool IsLineContainer
    {
      get;
    }
    /// <summary>
    /// Gets the children layout direction.
    /// </summary>
    /// <value>The children layout direction.</value>
    ChildrenLayoutDirection ChildrenLayoutDirection
    {
      get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is line break.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is line break; otherwise, <c>false</c>.
    /// </value>
    bool IsLineBreak
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets a value indicating whether [text wrap].
    /// </summary>
    /// <value><c>true</c> if [text wrap]; otherwise, <c>false</c>.</value>
    bool TextWrap
    {
      get;
      set;
    }
    /// <summary>
    /// 
    /// </summary>
    bool IsPageBreakItem
    {
      get;
      set;
    }
    #endregion
	}
}
