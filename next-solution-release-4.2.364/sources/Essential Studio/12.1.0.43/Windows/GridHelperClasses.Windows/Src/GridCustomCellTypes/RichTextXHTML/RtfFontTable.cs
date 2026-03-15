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
using System.Windows.Forms;

namespace Syncfusion.GridHelperClasses
{
	/// <summary>
	/// Summary description for RtfFontTable.
	/// </summary>
	public class RtfFontTable
	{
		private int numberOfFonts = 0;
		private string fonttbl;
		private Hashtable loadedFonts = new Hashtable();
        /// <summary>
        /// Font table holding different fonts
        /// </summary>
		public RtfFontTable()
		{
			//
			// TODO: Add constructor logic here
			//

			fonttbl = "{\\fonttbl{\\f0\\froman Times New Roman;}";
		}
        /// <summary>
        /// Uses the specified font
        /// </summary>
        /// <param name="fontName">string</param>
        /// <returns>int</returns>
		public int UseFont(string fontName)
		{
			if (loadedFonts.Contains(fontName))
			{
				return (int)loadedFonts[fontName];
			}
			else
			{
				fonttbl += "{\\f" + (++numberOfFonts) + "\\fnil " + fontName + ";}";
				loadedFonts.Add(fontName, numberOfFonts);
				return numberOfFonts;
			}
		}

        /// <summary>
        /// Converts the data to string
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return fonttbl + "}";
		}

	}
}
