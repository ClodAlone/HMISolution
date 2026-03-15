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
	/// Summary description for LayoutFieldInfo.
	/// </summary>
	public class LayoutFieldInfo
	  : LayoutInfo
	{
	  #region Members
	  /// <summary>
	  /// 
	  /// </summary>
	  private int m_fieldType = -1;
	  #endregion

	  #region Properties
	  /// <summary>
	  /// Gets or sets the type of the field.
	  /// </summary>
	  /// <value>The type of the field.</value>
	  public int FieldType
	  {
	    get
	    {
	      return m_fieldType;
	    }
	    set
	    {
	      m_fieldType = value;
	    }
	  }
	  #endregion

	  #region Constructors
	  /// <summary>
	  /// Initializes a new instance of the <see cref="LayoutFieldInfo"/> class.
	  /// </summary>
	  /// <param name="childLayoutDirection">if set to <c>true</c> [b top subtract area].</param>
	  public LayoutFieldInfo( ChildrenLayoutDirection childLayoutDirection )
	    : base( childLayoutDirection )
	  {}
	  #endregion
	}
}
