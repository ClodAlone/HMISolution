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

namespace Syncfusion.Windows.Forms.Chart.PostScript
{
	/// <summary>
	/// Represents the post scripf radial shading dictionary.
	/// </summary>
	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class RadialShadingDictionary : PostScriptDictionary
	{
		#region Members

		private const string BaseName = "RadialShdng";

		#endregion
		/// <summary>
		/// Creates instance of the RadialShadingDictionary.
		/// </summary>
		/// <param name="point1"></param>
		/// <param name="r1"></param>
		/// <param name="point2"></param>
		/// <param name="r2"></param>
		/// <param name="funcName"></param>
		public RadialShadingDictionary(PointF point1, float r1, PointF point2, float r2, string funcName)
		{
			InternalTable.Add("/ShadingType", 3);
			InternalTable.Add("/ColorSpace", "/DeviceRGB");
			InternalTable.Add("/Coords", "[ " + point1.X.ToString().Replace(",", ".") + " " +
																					point1.Y.ToString().Replace(",", ".") + " " +
																					r1.ToString().Replace(",", ".") + " " +
																					point2.X.ToString().Replace(",", ".") + " " +
																					point2.Y.ToString().Replace(",", ".") + " " +
																					r2.ToString().Replace(",", ".") + " ]");
			InternalTable.Add("/Function", funcName);
			InternalTable.Add("/Extend", "[ true true ]");
		}

		/// <summary>
		/// Overridden.
		/// </summary>
		public override string Name
		{
			get
			{
				return BaseName + base.Name;
			}
		}
	}
}
