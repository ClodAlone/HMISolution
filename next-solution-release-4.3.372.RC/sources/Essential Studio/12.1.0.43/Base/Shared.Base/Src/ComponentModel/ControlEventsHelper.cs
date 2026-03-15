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
using System.Reflection;
using System.Windows.Forms;
#endregion

namespace Syncfusion.ComponentModel
{
	/// <summary></summary>
	public class ControlEventsHelper
	{
		#region Event keys
		/// <summary></summary>
		public static readonly object EventBackColor;
		/// <summary></summary>
		public static readonly object EventBackgroundImage;
		/// <summary></summary>
		public static readonly object EventBindingContext;
		/// <summary></summary>
		public static readonly object EventCausesValidation;
		/// <summary></summary>
		public static readonly object EventChangeUICues;
		/// <summary></summary>
		public static readonly object EventClick;
		/// <summary></summary>
		public static readonly object EventContextMenu;
		/// <summary></summary>
		public static readonly object EventControlAdded;
		/// <summary></summary>
		public static readonly object EventControlRemoved;
		/// <summary></summary>
		public static readonly object EventCursor;
		/// <summary></summary>
		public static readonly object EventDock;
		/// <summary></summary>
		public static readonly object EventDoubleClick;
		/// <summary></summary>
		public static readonly object EventDragDrop;
		/// <summary></summary>
		public static readonly object EventDragEnter;
		/// <summary></summary>
		public static readonly object EventDragLeave;
		/// <summary></summary>
		public static readonly object EventDragOver;
		/// <summary></summary>
		public static readonly object EventEnabled;
		/// <summary></summary>
		public static readonly object EventEnabledChanged;
		/// <summary></summary>
		public static readonly object EventEnter;
		/// <summary></summary>
		public static readonly object EventFont;
		/// <summary></summary>
		public static readonly object EventForeColor;
		/// <summary></summary>
		public static readonly object EventGiveFeedback;
		/// <summary></summary>
		public static readonly object EventGotFocus;
		/// <summary></summary>
		public static readonly object EventHandleCreated;
		/// <summary></summary>
		public static readonly object EventHandleDestroyed;
		/// <summary></summary>
		public static readonly object EventHelpRequested;
		/// <summary></summary>
		public static readonly object EventImeModeChanged;
		/// <summary></summary>
		public static readonly object EventInvalidated;
		/// <summary></summary>
		public static readonly object EventKeyDown;
		/// <summary></summary>
		public static readonly object EventKeyPress;
		/// <summary></summary>
		public static readonly object EventKeyUp;
		/// <summary></summary>
		public static readonly object EventLayout;
		/// <summary></summary>
		public static readonly object EventLeave;
		/// <summary></summary>
		public static readonly object EventLocation;
		/// <summary></summary>
		public static readonly object EventLostFocus;
		/// <summary></summary>
		public static readonly object EventMouseDown;
		/// <summary></summary>
		public static readonly object EventMouseEnter;
		/// <summary></summary>
		public static readonly object EventMouseHover;
		/// <summary></summary>
		public static readonly object EventMouseLeave;
		/// <summary></summary>
		public static readonly object EventMouseMove;
		/// <summary></summary>
		public static readonly object EventMouseUp;
		/// <summary></summary>
		public static readonly object EventMouseWheel;
		/// <summary></summary>
		public static readonly object EventMove;
		/// <summary></summary>
		public static readonly object EventPaint;
		/// <summary></summary>
		public static readonly object EventParent;
		/// <summary></summary>
		public static readonly object EventQueryAccessibilityHelp;
		/// <summary></summary>
		public static readonly object EventQueryContinueDrag;
		/// <summary></summary>
		public static readonly object EventResize;
		/// <summary></summary>
		public static readonly object EventRightToLeft;
		/// <summary></summary>
		public static readonly object EventSize;
		/// <summary></summary>
		public static readonly object EventStyleChanged;
		/// <summary></summary>
		public static readonly object EventSystemColorsChanged;
		/// <summary></summary>
		public static readonly object EventTabIndex;
		/// <summary></summary>
		public static readonly object EventTabStop;
		/// <summary></summary>
		public static readonly object EventText;
		/// <summary></summary>
		public static readonly object EventValidated;
		/// <summary></summary>
		public static readonly object EventValidating;
		/// <summary></summary>
		public static readonly object EventVisible;
		/// <summary></summary>
		public static readonly object EventVisibleChanged;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		static ControlEventsHelper()
		{
			Type control = typeof( Control );
			const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetField;

			using( Control ctrInstance = new Control() )
			{
				// 00 - 09
				EventBackColor = control.GetField( "EventBackColor", flags ).GetValue( ctrInstance );
				EventBackgroundImage = control.GetField( "EventBackgroundImage", flags ).GetValue( ctrInstance );
				EventBindingContext = control.GetField( "EventBindingContext", flags ).GetValue( ctrInstance );
				EventCausesValidation = control.GetField( "EventCausesValidation", flags ).GetValue( ctrInstance );
				EventChangeUICues = control.GetField( "EventChangeUICues", flags ).GetValue( ctrInstance );
				EventClick = control.GetField( "EventClick", flags ).GetValue( ctrInstance );
				EventContextMenu = control.GetField( "EventContextMenu", flags ).GetValue( ctrInstance );
				EventControlAdded = control.GetField( "EventControlAdded", flags ).GetValue( ctrInstance );
				EventControlRemoved = control.GetField( "EventControlRemoved", flags ).GetValue( ctrInstance );
				// 10 - 19
				EventCursor = control.GetField( "EventCursor", flags ).GetValue( ctrInstance );
				EventDock = control.GetField( "EventDock", flags ).GetValue( ctrInstance );
				EventDoubleClick = control.GetField( "EventDoubleClick", flags ).GetValue( ctrInstance );
				EventDragDrop = control.GetField( "EventDragDrop", flags ).GetValue( ctrInstance );
				EventDragEnter = control.GetField( "EventDragEnter", flags ).GetValue( ctrInstance );
				EventDragLeave = control.GetField( "EventDragLeave", flags ).GetValue( ctrInstance );
				EventDragOver = control.GetField( "EventDragOver", flags ).GetValue( ctrInstance );
				EventEnabled = control.GetField( "EventEnabled", flags ).GetValue( ctrInstance );
				EventEnabledChanged = control.GetField( "EventEnabledChanged", flags ).GetValue( ctrInstance );
				// 20 - 29
				EventEnter = control.GetField( "EventEnter", flags ).GetValue( ctrInstance );
				EventFont = control.GetField( "EventFont", flags ).GetValue( ctrInstance );
				EventForeColor = control.GetField( "EventForeColor", flags ).GetValue( ctrInstance );
				EventGiveFeedback = control.GetField( "EventGiveFeedback", flags ).GetValue( ctrInstance );
				EventGotFocus = control.GetField( "EventGotFocus", flags ).GetValue( ctrInstance );
				EventHandleCreated = control.GetField( "EventHandleCreated", flags ).GetValue( ctrInstance );
				EventHandleDestroyed = control.GetField( "EventHandleDestroyed", flags ).GetValue( ctrInstance );
				EventHelpRequested = control.GetField( "EventHelpRequested", flags ).GetValue( ctrInstance );
				EventImeModeChanged = control.GetField( "EventImeModeChanged", flags ).GetValue( ctrInstance );
				EventInvalidated = control.GetField( "EventInvalidated", flags ).GetValue( ctrInstance );
				// 30 - 39
				EventKeyDown = control.GetField( "EventKeyDown", flags ).GetValue( ctrInstance );
				EventKeyPress = control.GetField( "EventKeyPress", flags ).GetValue( ctrInstance );
				EventKeyUp = control.GetField( "EventKeyUp", flags ).GetValue( ctrInstance );
				EventLayout = control.GetField( "EventLayout", flags ).GetValue( ctrInstance );
				EventLeave = control.GetField( "EventLeave", flags ).GetValue( ctrInstance );
				EventLocation = control.GetField( "EventLocation", flags ).GetValue( ctrInstance );
				EventLostFocus = control.GetField( "EventLostFocus", flags ).GetValue( ctrInstance );
				EventMouseDown = control.GetField( "EventMouseDown", flags ).GetValue( ctrInstance );
				EventMouseEnter = control.GetField( "EventMouseEnter", flags ).GetValue( ctrInstance );
				EventMouseHover = control.GetField( "EventMouseHover", flags ).GetValue( ctrInstance );
				// 40 - 49
				EventMouseLeave = control.GetField( "EventMouseLeave", flags ).GetValue( ctrInstance );
				EventMouseMove = control.GetField( "EventMouseMove", flags ).GetValue( ctrInstance );
				EventMouseUp = control.GetField( "EventMouseUp", flags ).GetValue( ctrInstance );
				EventMouseWheel = control.GetField( "EventMouseWheel", flags ).GetValue( ctrInstance );
				EventMove = control.GetField( "EventMove", flags ).GetValue( ctrInstance );
				EventPaint = control.GetField( "EventPaint", flags ).GetValue( ctrInstance );
				EventParent = control.GetField( "EventParent", flags ).GetValue( ctrInstance );
				EventQueryAccessibilityHelp = control.GetField( "EventQueryAccessibilityHelp", flags ).GetValue( ctrInstance );
				EventQueryContinueDrag = control.GetField( "EventQueryContinueDrag", flags ).GetValue( ctrInstance );
				// 50 - 59
				EventResize = control.GetField( "EventResize", flags ).GetValue( ctrInstance );
				EventRightToLeft = control.GetField( "EventRightToLeft", flags ).GetValue( ctrInstance );
				EventSize = control.GetField( "EventSize", flags ).GetValue( ctrInstance );
				EventStyleChanged = control.GetField( "EventStyleChanged", flags ).GetValue( ctrInstance );
				EventSystemColorsChanged = control.GetField( "EventSystemColorsChanged", flags ).GetValue( ctrInstance );
				EventTabIndex = control.GetField( "EventTabIndex", flags ).GetValue( ctrInstance );
				EventTabStop = control.GetField( "EventTabStop", flags ).GetValue( ctrInstance );
				EventText = control.GetField( "EventText", flags ).GetValue( ctrInstance );
				EventValidated = control.GetField( "EventValidated", flags ).GetValue( ctrInstance );
				// 60 - 63
				EventValidating = control.GetField( "EventValidating", flags ).GetValue( ctrInstance );
				EventVisible = control.GetField( "EventVisible", flags ).GetValue( ctrInstance );
				EventVisibleChanged = control.GetField( "EventVisibleChanged", flags ).GetValue( ctrInstance );
			}
		}
		#endregion
	}
}