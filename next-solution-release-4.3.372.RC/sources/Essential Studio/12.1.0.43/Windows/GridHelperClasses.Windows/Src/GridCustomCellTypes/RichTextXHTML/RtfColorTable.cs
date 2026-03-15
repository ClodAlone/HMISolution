#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Any infringement will be prosecuted under
//  applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.Drawing;

namespace Syncfusion.GridHelperClasses
{
	/// <summary>
	/// Summary description for RtfColorTable.
	/// </summary>
	public class RtfColorTable
	{
		private int numberOfColors = 0;
		private string colortbl;
		private Hashtable loadedColors = new Hashtable();
        /// <summary>
        /// Color table holding different colors
        /// </summary>
		public RtfColorTable()
		{
			//
			// TODO: Add constructor logic here
			//
			colortbl = "{\\colortbl;";
		}
        /// <summary>
        /// Used to set the color
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public int UseColor(Color key)
		{
			if (loadedColors.ContainsKey(key))
			{
				return (int)loadedColors[key];
			}
			else
			{
				colortbl += "\\red" + key.R + "\\green" + key.G + "\\blue" + key.B + ";";
				loadedColors.Add(key, ++numberOfColors);
				return numberOfColors;
			}
		}
        /// <summary>
        /// Converts the data to string
        /// </summary>
        /// <returns>string</returns>
		public override string ToString()
		{
			return colortbl + "}";
		}
	}
}
