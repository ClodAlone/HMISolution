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
using System.Diagnostics;

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Diagnostics
{
	/// <summary>
	///    Provides predefined switches for enabling / disabling trace
	///    output or code instrumentation in the Syncfusion shared library.
	/// </summary>
	[ DocumentationExclude() ]
	internal sealed class Switches
	{
		#region General Shared TraceSwitch
		/// <summary>General switch for Shared Library.</summary>
		private static TraceSwitch general;
		/// <summary>
		/// Returns the General Tracing level for the Grid Library.
		/// </summary>
		public static TraceSwitch General
		{
			get
			{
				if( general == null )
				{
					general = new TraceSwitch( "Shared.General", "Enable Tracing for the Syncfusion shared Library" );
				}

				return general;
			}
		}

		#endregion
		
		#region Known Trace Switches
		/// <summary>Enable Tracing for the Styles.</summary>
		public static readonly TraceSwitch Styles = new TraceSwitch( "Shared.Styles", "Debug Styles" );
		/// <summary>Enable Tracing for the Shared Serialization.</summary>
		public static readonly TraceSwitch Serialization = new TraceSwitch( "Shared.Serialization", "Debug Shared Serialization" );
		/// <summary>Enable Tracing for the ScrollControl.</summary>
		public static readonly TraceSwitch ScrollControl = new TraceSwitch( "Shared.ScrollControl", "Enable Tracing for the ScrollControl" );
		/// <summary>Enable Tracing for Timer Start and Stop</summary>
		public static readonly TraceSwitch Timers = new TraceSwitch( "Shared.Timers", "Enable Tracing for Timer Start and Stop" );
		/// <summary>Enable Tracing for Timer Start and Stop.</summary>
		public static readonly TraceSwitch Workbook = new TraceSwitch( "Shared.Workbook", "Enable Tracing for Workbooks" );
		/// <summary>Enable Tracing for BeginUpdate and EndUpdate methods calls</summary>
		public static readonly TraceSwitch BeginEndUpdate = new TraceSwitch( "Shared.BeginEndUpdate", "Enable Tracing for BeginUpdate and EndUpdate methods calls" );
		/// <summary>Enable Tracing for MouseController.</summary>
		public static readonly TraceSwitch MouseController = new TraceSwitch( "Shared.MouseController", "Enable Tracing for MouseController" );
		/// <summary>Enable Tracing for OperationFeedback.</summary>
		public static readonly TraceSwitch OperationFeedback = new TraceSwitch( "Shared.OperationFeedback", "Enable Tracing for OperationFeedback" );
		/// <summary>Enable Tracing for BrushPaint drawing methods.</summary>
		public static readonly TraceSwitch BrushPaint = new TraceSwitch( "Shared.BrushPaint", "Enable Tracing for BrushPaint drawing methods" );
		/// <summary>Enable Tracing for ArrowButtonBar events.</summary>
		public static readonly TraceSwitch ArrowButtonBarEvents = new TraceSwitch( "Shared.ArrowButtonBarEvents", "Enable Tracing for ArrowButtonBar events" );
		/// <summary>Enable Tracing for ButtonBar events.</summary>
		public static readonly TraceSwitch ButtonBarEvents = new TraceSwitch( "Shared.ButtonBarEvents", "Enable Tracing for ButtonBar events" );
		/// <summary>Enable Tracing for RecordNavigationBar events.</summary>
		public static readonly TraceSwitch RecordNavigationBarEvents = new TraceSwitch( "Shared.RecordNavigationBarEvents", "Enable Tracing for RecordNavigationBar events" );
		/// <summary>Enable Tracing for RecordNavigationControl events.</summary>
		public static readonly TraceSwitch RecordNavigationControlEvents = new TraceSwitch( "Shared.RecordNavigationControlEvents", "Enable Tracing for RecordNavigationControl events" );
		/// <summary>Enable Tracing for SplitterControl events.</summary>
		public static readonly TraceSwitch SplitterControlEvents = new TraceSwitch( "Shared.SplitterControlEvents", "Enable Tracing for SplitterControl events" );
		/// <summary>Enable Tracing for TabBar events.</summary>
		public static readonly TraceSwitch TabBarEvents = new TraceSwitch( "Shared.TabBarEvents", "Enable Tracing for TabBar events" );
		/// <summary>Enable Tracing for TabBarSplitterControl events.</summary>
		public static readonly TraceSwitch TabBarSplitterControlEvents = new TraceSwitch( "Shared.TabBarSplitterControlEvents", "Enable Tracing for TabBarSplitterControl events" );
		/// <summary>Enable Tracing for MouseControllerDispatcher events.</summary>
		public static readonly TraceSwitch MouseControllerDispatcherEvents = new TraceSwitch( "Shared.MouseControllerDispatcherEvents", "Enable Tracing for MouseControllerDispatcher events" );
		/// <summary>Enable Tracing for ScrollControl events.</summary>
		public static readonly TraceSwitch ScrollControlEvents = new TraceSwitch( "Shared.ScrollControlEvents", "Enable Tracing for ScrollControl events" );
		/// <summary>Enable Tracing for Focus events.</summary>
		public static readonly TraceSwitch ScrollControlFocus = new TraceSwitch( "Shared.ScrollControlFocus", "Enable Tracing for Focus events" );
		#endregion
	}
}