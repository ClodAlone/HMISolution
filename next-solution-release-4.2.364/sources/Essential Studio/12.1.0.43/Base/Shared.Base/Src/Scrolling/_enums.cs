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
using System.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary></summary>
	public enum ScrollBarCustomDrawStyles
	{
		/// <summary></summary>
		Classic,
		/// <summary></summary>
		WindowsXP,
		/// <summary></summary>
		Office2007,
        /// <summary></summary>
        Office2007Generic,
        /// <summary>Retrieves Office2010 scroll bars</summary>
        Office2010,
        /// <summary>
        /// Metro style
        /// </summary>
        Metro
	};
	/// <summary>
	/// Characterize zone which contain mouse down position.
	/// Order of enum's elements can not be changed, because ScrollBarCustomDraw 
	/// uses enum's indexes. 
	/// </summary>
	public enum PressedZone
	{
		/// <summary></summary>
		[ Browsable( false) ]
		None = -1,
		/// <summary></summary>
		MinButton,
		/// <summary></summary>
		MaxButton,
		/// <summary></summary>
		Thumb,
		/// <summary></summary>
		ThumbLeftZone,
		/// <summary></summary>
		ThumbRightZone,
		/// <summary></summary>
		[ Browsable( false) ]
		Length
	};
	/// <summary>
	/// 
	/// </summary>
	public enum Office2007ColorScheme
	{
		/// <summary></summary>
		Blue = 0,
        /// <summary></summary>
        Silver,
		/// <summary></summary>
		Black,
		/// <summary></summary>
		Managed = -1
	}
    /// <summary>
    /// 
    /// </summary>
    public enum MetroColorScheme
    {
        /// <summary></summary>
        Magenta = 0,
        /// <summary></summary>
        Purple,
        /// <summary></summary>
        Teal,
        /// <summary></summary>
        Lime,
        /// <summary></summary>
        Brown,
        /// <summary></summary>       
        Pink,
        /// <summary></summary>
        Orange,
        /// <summary></summary>
        Blue,
        /// <summary></summary>
        Red,
        /// <summary></summary>
        Green,
        /// <summary></summary>
        Managed = -1
    }
    /// <summary>
    /// Characterize Office2010 scroll bar color scheme.
    /// </summary>
    public enum Office2010ColorScheme
    {
        /// <summary>Office2010 blue.</summary>
        Blue = 0,
        /// <summary>Office2010 silver.</summary>
        Silver,
        /// <summary>Office2010 black.</summary>
        Black,
        /// <summary>Office2010 default color.</summary>
        Managed = -1
    }

    /// <summary>
    /// Characterize MS-Office scroll bars.
    /// </summary>
    public enum OfficeScrollBars
    {
        /// <summary>Office2007 Scrollbars.</summary>
        Office2007,
        /// <summary>Office2010 Scrollbars.</summary>
        Office2010,
        /// <summary>Metro Scrollbars.</summary>
        Metro,
        /// <summary>Ordinary Scrollbars.</summary>
        None
    }

	/// <summary>
	/// Characterize zone which contain mouse position.
	/// Order of enum's elements can not be changed, because ScrollBarCustomDraw 
	/// uses enum's indexes. 
	/// </summary>
	public enum MovedZone
	{   
		/// <summary></summary>
		MinButton,
		/// <summary></summary>
		MaxButton,
		/// <summary></summary>
		Thumb,
		/// <summary></summary>
		ThumbLeftZone,
		/// <summary></summary>
		ThumbRightZone,
		/// <summary></summary>
		Out   
	};

	/// <summary>
	/// Specifies the layout and colors for scrollbars.
	/// </summary>
	public enum WindowsXPColorsScheme
	{
		/// <summary></summary>
		DefaultBlue = 1,
		/// <summary></summary>
		OliveGreen = 2,
		/// <summary></summary>
		Silver = 3
	}
  
  /// <summary>
  /// Specifies behaviour of size gripper for scrollable frame controls.
  /// </summary>
  public enum SizeGripperVisibility
  {
    /// <summary>
    /// Gripper is visible when both horizontal and vertical scrollbars are visible.
    /// </summary>
    Auto,
    /// <summary>
    /// Gripper is always visible.
    /// </summary>
    Visible,
    /// <summary>
    /// Gripper is always hidden.
    /// </summary>
    Hidden
  }
}