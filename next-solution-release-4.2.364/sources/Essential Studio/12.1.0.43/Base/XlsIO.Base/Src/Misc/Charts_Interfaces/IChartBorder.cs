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

namespace Syncfusion.XlsIO.Interfaces.Charts
{
	/// <summary>
	/// Summary description for IChartBorder.
	/// </summary>
	public interface IChartBorder
	{
    ExcelAutoType         BorderType { get; set; } // auto, custom, none
    ExcelChartLinePattern LineStyle { get; set; }
    Color                 LineColor { get; set; }
    ExcelChartLineWeight   LineWeight { get; set; }
	}
}
