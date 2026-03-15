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
using System.Collections;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Class for converting valid html to "HTML Clipboard Format".
	/// HTML Clipboard Format:
	/// http://msdn.microsoft.com/workshop/networking/clipboard/htmlclipboard.asp?frame=true
	/// </summary>
	public class ClipboardHTML
	{
		private struct FragmentPair
		{
			public int Start;
			public int End;
		}

		#region Class constants
		/// <summary>
		/// Common header for clipboard.
		/// </summary>
		private const string DEF_HTML_HEADER =
			"Version:1.0\n" +
			"StartHTML:aaaaaaaaaa\n" +
			"EndHTML:bbbbbbbbbb\n";

		/// <summary>
		/// Header of the fragment.
		/// </summary>
		private const string DEF_HTML_HEADER_FRAGMENT =
			"StartFragment:cccccccccc\n" +
			"EndFragment:dddddddddd\n";
		/// <summary>
		/// Start of the fragment.
		/// </summary>
		public const string DEF_HTML_FRAGMENT_START = "<!--StartFragment -->";
		/// <summary>
		/// End of the fragment.
		/// </summary>
		public const string DEF_HTML_FRAGMENT_END = "<!--EndFragment -->";
		#endregion

		/// <summary>
		/// Converts html to "HTML Clipboard Format".
		/// </summary>
		/// <remarks>
		/// Input HTML must have <!--StartFragment --> 
		/// and <!--EndFragment --> marks.
		/// </remarks>
		/// <param name="text">HTML to convert.</param>
		/// <returns>HTML with special headers.</returns>
		public static string GetHTMLForClipboard( string text )
		{
			string result = DEF_HTML_HEADER;
			IList fragments = new ArrayList();
			int lastIndex = 0;
			int iSizeHeader = DEF_HTML_HEADER.Length;
			int iSizeFragmentHeader = DEF_HTML_HEADER_FRAGMENT.Length;

			while( ( lastIndex = text.IndexOf( DEF_HTML_FRAGMENT_START, lastIndex ) ) > -1 )
			{
				FragmentPair pair = new FragmentPair();
				pair.Start = lastIndex;
				lastIndex = text.IndexOf( DEF_HTML_FRAGMENT_END, lastIndex );

				if( lastIndex == -1 )
					throw new ApplicationException( DEF_HTML_FRAGMENT_START + "found and " + DEF_HTML_FRAGMENT_END + " was not found." );

				pair.End = lastIndex;
				fragments.Add( pair );
			}

			if( fragments.Count == 0 )
				throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_39 );

			result = result.Replace( "aaaaaaaaaa", string.Format( "{0, 10}", iSizeHeader + iSizeFragmentHeader * fragments.Count ) );
			result = result.Replace( "bbbbbbbbbb", string.Format( "{0, 10}", iSizeHeader + iSizeFragmentHeader * fragments.Count + text.Length ) );

			for( int i = 0; i < fragments.Count; i++ )
			{
				FragmentPair pair = ( FragmentPair )fragments[ i ];
				string pairText = DEF_HTML_HEADER_FRAGMENT;
				pairText = pairText.Replace( "cccccccccc", string.Format( "{0, 10}", iSizeHeader + iSizeFragmentHeader * ( i + 1 ) + pair.Start ) );
				pairText = pairText.Replace( "dddddddddd", string.Format( "{0, 10}", iSizeHeader + iSizeFragmentHeader * ( i + 1 ) + pair.End ) );
				result += pairText;
			}

			result += text;
			return result;
		}

	}
}
