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

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Represents the properties and events required by the <see cref="ChartDockingManager"/>.
	/// </summary>
	public interface IChartDockControl
	{
    /// <summary>
    /// Event Raised when the Location of the Legend is changing.
    /// </summary>
    event LocationEventHandler LocationChanging;
    /// <summary>
    /// Event Raised when the position of the Legend was changed.
    /// </summary>
    event EventHandler ChartDockChanged;
    /// <summary>
    /// Event Raised when the Alignment of the Legend was changed.
    /// </summary>
    event EventHandler ChartAlignmentChanged;
    /// <summary>
    /// Event Raised when the size of the Legend was changed.
    /// </summary>
    event EventHandler SizeChanged;
    /// <summary>
    /// Event Raised when the location of the Legend was changed.
    /// </summary>
    event EventHandler LocationChanged;
    /// <summary>
    /// Event Raised when the Visible of the Legend was changed.
    /// </summary>
    event EventHandler VisibleChanged;

    /// <summary>
    /// Get and set the position of the Legend in chartControl.
    /// </summary>
    ChartDock Position
    { 
      get; 
      set;
    }
    /// <summary>
    /// Get and set the Alignment of the Legend in chartControl.
    /// </summary>
    ChartAlignment Alignment
    { 
      get; 
      set;
    }
    /// <summary>
    /// Get and set the Orientation of the Legend in chartControl.
    /// </summary>
    ChartOrientation Orientation
    { 
      get; 
      set;
    }
    /// <summary>
    /// Get and set the docking as free.
    /// </summary>
	ChartDockingFlags Behavior
    { 
      get; 
      set;
    }
    /// <summary>
    /// Get and set the Location of the Legend.
    /// </summary>
    Point Location
    {
      get;
      set;
    }
    /// <summary>
    /// Get and set the Size of the Legend. Its works when the FloatingAutosize property is false.
    /// </summary>
    Size Size
    {
      get;
      set;
    }
    /// <summary>
    /// Get and set the Visibility of the Legend in chartControl.
    /// </summary>
    bool Visible
    {
      get;
      set;
    }
		/// <summary>
		/// Gets or sets a value indicating whether the control can respond to user interaction.
		/// </summary>
		bool Enabled { get; set; }
		/// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    SizeF Measure(SizeF size);
  }
}
