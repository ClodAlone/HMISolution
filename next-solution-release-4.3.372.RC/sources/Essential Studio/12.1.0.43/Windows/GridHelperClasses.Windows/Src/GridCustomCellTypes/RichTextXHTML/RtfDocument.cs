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
using System.Drawing;

namespace Syncfusion.GridHelperClasses
{
	/// <summary>
	/// Summary description for RtfDocument.
	/// </summary>
	public class RtfDocument
	{
		private RtfFontTable fonttbl;
		private RtfColorTable colortbl;
		private string header;
		private string document;
        /// <summary>
        /// Empty Construcor for RtfDocument class
        /// </summary>
		public RtfDocument()
		{
			//
			// TODO: Add constructor logic here
			//
			header = "{\\rtf1";
			fonttbl = new RtfFontTable();
			colortbl = new RtfColorTable();
		}
        /// <summary>
        /// converts the data to String Type
        /// </summary>
        /// <returns>string</returns>
		public override string ToString()
		{
			header += fonttbl.ToString() + colortbl.ToString();
			return header + "{" + document + "}}";
		}
        /// <summary>
        /// Appends teh text with with other text
        /// </summary>
        /// <param name="text">text</param>
		public void AppendText(string text)
		{
			document += text;
		}
        /// <summary>
        /// UseFont
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
		public int UseFont(string fontName)
		{
			return fonttbl.UseFont(fontName);
		}
        /// <summary>
        /// Method to use the color in grid
        /// </summary>
        /// <param name="fromArgb"></param>
        /// <returns></returns>
		public int UseColor(Color fromArgb)
		{
			return colortbl.UseColor(fromArgb);
		}
	}
}
