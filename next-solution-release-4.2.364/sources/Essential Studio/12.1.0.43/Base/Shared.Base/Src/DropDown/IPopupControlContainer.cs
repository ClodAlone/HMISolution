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
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface INeedKeyboardMessages
	{
		bool KeyboardMessage(ref Message m);
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface INeedMouseMoveMessages
	{
		bool MouseMessage(ref Message m);
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IDelegateFocusToPrevWindow
	{
		Control PreviousWindow{get;}
	}
	/// <summary>
	/// Specifies the relative alignment of a popup child about its parent.
	/// </summary>
	public enum PopupRelativeAlignment
	{
		/// <summary>
		/// The child will be aligned to the parent's top-left corner
		/// and drawn upwards and to the right of the parent's left border.
		/// </summary>
		TopLeft,
		/// <summary>
		/// The child will be aligned to the parent's top-right corner
		/// and drawn upwards and to the left of the parent's right border.
		/// </summary>
		TopRight,
		/// <summary>
		/// The child will be aligned to the parent's top-right corner
		/// and drawn downwards and to the right of the parent.
		/// </summary>
		RightTop,
		/// <summary>
		/// The child will be aligned to the parent's bottom-right corner
		/// and drawn upwards and to the right of the parent.
		/// </summary>
		RightBottom,
		/// <summary>
		/// The child will be aligned to the parent's bottom-left corner
		/// and drawn downwards and to the right of the parent's left border.
		/// </summary>
		BottomLeft,
		/// <summary>
		/// The child will be aligned to the parent's bottom-right corner
		/// and drawn downwards and to the left of the parent's right border.
		/// </summary>
		BottomRight,
		/// <summary>
		/// The child will be aligned to the parent's top-left corner
		/// and drawn downwards and to the left of the parent.
		/// </summary>
		LeftTop,
		/// <summary>
		/// The child will be aligned to the parent's bottom-left corner
		/// and drawn downwards and to the right of the parent.
		/// </summary>
		LeftBottom,
		/// <summary>
		/// Does not indicate any of the above alignments.
		/// </summary>
		Default
	}

	/// <summary>
	/// A generic interface for any control that wants to participate 
	/// in the Popup framework.
	/// </summary>
	public interface IPopupItem
	{
		/// <summary>
		/// Returns the popup's control parent.
		/// </summary>
		/// <remarks>
		/// If such a control exists, then the Popup framework will use
		/// it in its popup activation logic.
		/// </remarks>
		Control GetPopupParentControl();
		/// <summary>
		/// Indicates whether a specified control is part of the
		/// popup hierarchy.
		/// </summary>
		/// <param name="control">A control instance.</param>
		/// <param name="askPopupParent">True indicates this query should
		/// be passed to the IPopupParent, if any; False indicates you 
		/// should not query the popup parent.</param>
		/// <returns>True if the control is part of the Popup hierarchy;
		/// False otherwise.</returns>
		bool IsRelatedControl(Control control, bool askPopupParent);
	}

	/// <summary>
	/// Specifies the way in which a popup was closed.
	/// </summary>
	/// <remarks>
	/// This information is usually provided in a 
	/// PopupControlContainer's <see cref="E:Syncfusion.Windows.Forms.PopupControlContainer.CloseUp"/> event.
	/// You can use it to determine, in some cases, whether or not
	/// to use the updated data in a popup.
	/// </remarks>
	public enum PopupCloseType
	{
		/// <summary>
		/// The user wants the changes made in the popup to be applied.
		/// </summary>
		Done,
		/// <summary>
		/// The user canceled the popup and expects the changes, if any, to be ignored.
		/// </summary>
		Canceled,
		/// <summary>
		/// The popup was deactivated due to the user clicking in some
		/// other window, a different application getting focus, etc.
		/// </summary>
		Deactivated
	}

	/// <summary>
	/// A generic interface that defines a popup parent, that will
	/// control the alignment of a popup, etc.
	/// </summary>
	/// <remarks><para>Any object / control that wants to act as a popup's parent
	/// and participate in the popup framework should implement this interface.</para>
	/// <para>Take a look at the PopupsInDepth sample under the Tools\Samples\Editors Package\PopupControlContainer\Advanced
	/// folder for an implementation of this interface.</para></remarks>
	public interface IPopupParent : IPopupItem
	{
		/// <summary>
		/// Will be called to indicate that the popup child was closed
		/// in the specified mode.
		/// </summary>
		/// <param name="childUI">The child that was closed.</param>
		/// <param name="popupCloseType">A <see cref="PopupCloseType"/> value.</param>
		void ChildClosing(IPopupChild childUI, PopupCloseType popupCloseType);

		/// <summary>
		/// Returns the location for popup, given the preferred relative
		/// alignments.
		/// </summary>
		/// <remarks>
		/// <para>This method allows you to provide 8 different preferred positions for
		/// your popup, in any order.</para>
		/// <para>Different preferred positions are necessary because some positions
		/// may not be ideal for the popup as there may not be enough screen space
		/// for the popup when shown in that position.</para>
		/// <para>The popup will first call this method with prevAlignment set to
		/// <see cref="PopupRelativeAlignment.Default"/>. You should then return a location and
		/// designate this location as one of the 8 positions in the <see cref="PopupRelativeAlignment"/>
		/// enumeration using the newAlignment reference. </para>
		/// <para>The popup will then check if there is enough space in the screen
		/// to draw at this location. If not, it will call this method again with
		/// prevAlignment set to newAlignment from the previous call.</para>
		/// <para>
		/// This goes on until you return a location that the popup finds acceptable.
		/// However, if you run out of locations before the popup can find an acceptable location,
		/// then you should set newAlignment to <see cref="PopupRelativeAlignment.Default"/>. The popup will then assume
		/// that you don't have any more positions to supply and do the best it can
		/// with the supplied location.
		/// </para>
		/// </remarks>
		/// <param name="prevAlignment">The previous alignment.</param>
		/// <param name="newAlignment">The new alignment designated for the returned location.</param>
		/// <returns>
		/// An ideal location for popup designated by one of the <see cref="PopupRelativeAlignment"/> values in
		/// newAlignment. Or returns a Point.Empty and set newAlignment to <see cref="PopupRelativeAlignment.Default"/>
		/// if you do not intend to provide multiple preferred locations.
		/// </returns>
		Point GetLocationForPopupAlignment(PopupRelativeAlignment prevAlignment, 
			out PopupRelativeAlignment newAlignment);

		/// <summary>
		/// Returns a line (defined by 2 points) in screen co-ordinates that indicates the
		/// border overlap between the child and parent.
		/// </summary>
		/// <param name="relativeAlignment">The preferred alignment selected using the
		/// <see cref="GetLocationForPopupAlignment"/> method.</param>
		/// <remarks>
		/// If a valid line is provided, the popup child in that
		/// area will be drawn in an overlap style.
		/// </remarks>
		/// <returns>
		/// An array of 2 points representing a line (in screen co-ordinates)
		/// where the overlap takes place. NULL if no overlap is desired.
		/// </returns>
		Point[] GetBorderOverlapCue(PopupRelativeAlignment relativeAlignment);

		/// <summary>
		/// Indicates whether control's elements should be rendered right-to-left.
		/// </summary>
		bool IsRightToLeft{get;}
	}

	/// <summary>
	/// Defines a generic interface which when implemented will allow
	/// that object / control to participate in the popup framework.
	/// </summary>
	/// <remarks>
	/// This interface should be implemented by an object / control that 
	/// acts like a popup window and wants to participate in the popup framework.
	/// </remarks>
	public interface IPopupChild : IPopupItem, INeedKeyboardMessages, INeedMouseMoveMessages,
		IMouseHookHLProcClient, IKeyboardProcHookClient
	{
		/// <summary>
		/// Hides the popup window.
		/// </summary>
		/// <param name="popupCloseType">The PopupCloseType value that indicates
		/// the mode in which this popup should be closed.</param>
		void HidePopup(PopupCloseType popupCloseType);
		/// <summary>
		/// Indicates whether a popup is currently active / open.
		/// </summary>
		/// <returns>True if it is open; False otherwise.</returns>
		bool IsShowing();
		/// <summary>
		/// Returns the <see cref="IPopupParent"/> parent.
		/// </summary>
		/// <value>An instance of the <see cref="IPopupParent"/> interface.</value>
		/// <remarks>
		/// The Popup framework can handle a hierarchy of popups (like
		/// in a menu) for which it requires each popup child to provide
		/// a reference to its popup parent.
		/// </remarks>
		IPopupParent PopupParent{get;}
	}


	/// <summary>
	/// Represents the method that will handle the <see cref="E:Syncfusion.Windows.Forms.PopupControlContainer.CloseUp"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A PopupClosedEventArgs that contains the event data.</param>
	public delegate void PopupClosedEventHandler(object sender, PopupClosedEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="Syncfusion.Windows.Forms.PopupControlContainer.CloseUp"/> event.
	/// </summary>
	public sealed class PopupClosedEventArgs : SyncfusionEventArgs 
	{
		PopupCloseType popupCloseType;
	
		/// <summary>
		/// Creates an instance of the PopupClosedEventArgs class.
		/// </summary>
		/// <param name="popupCloseType">A PopupCloseType value.</param>
		public PopupClosedEventArgs(PopupCloseType popupCloseType) 
		{
			this.popupCloseType = popupCloseType;
		}
	
		/// <summary>
		/// Returns the PopupCloseType value indicating the way in which 
		/// the popup was closed.
		/// </summary>
		[TraceProperty(true)]
		public PopupCloseType PopupCloseType
		{
			get
			{
				return popupCloseType;
			}
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class UtilFuncs
	{
		// Methods
		internal const int SWP_NOSIZE = 1; // 0x0001 
		internal const int SWP_NOMOVE = 2; // 0x0002 
		internal const int SWP_NOZORDER = 4; // 0x0004 
		internal const int SWP_NOACTIVATE = 16; // 0x0010 
		internal const int SWP_SHOWWINDOW = 64; // 0x0040 
		internal const int SWP_HIDEWINDOW = 128; // 0x0080 
		internal const int SWP_DRAWFRAME = 32; // 0x0020 

		public static void ShowWindowTopMost(IntPtr handle, Point location, Size size)  
		{
			NativeMethods.SetWindowPos(handle, 
				(IntPtr)NativeMethods.HWND_TOPMOST, location.X, location.Y, size.Width, size.Height, 
				SWP_NOACTIVATE|SWP_SHOWWINDOW);
		}

		public static void SetVisibleNoActivate(Control control, bool visible)
		{
			if( null != control )
			{
				if( visible )
				{
					ShowWindowTopMost(control.Handle, control.Location, control.Size);
					control.Visible = true;
				}
				else
				{
					control.Visible = false;
				}
			}
		}
	}

}


