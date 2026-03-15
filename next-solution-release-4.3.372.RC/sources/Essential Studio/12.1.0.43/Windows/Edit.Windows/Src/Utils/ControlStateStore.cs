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
using System.Drawing.Drawing2D;
using System.Collections;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Text;

using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Class for storing control state.
	/// </summary>
	[Serializable]
	public class ControlStateStore
		: ISerializable
	{
		#region Internal Classes
		/// <summary>
		/// Information about one dynamic format.
		/// </summary>
		[Serializable]
		private struct FormatInfo
		{
			public long StartOffset;
			public long EndOffset;
			public TextFormatInfo TextFormatInfo;

			public FormatInfo( IDynamicFormat format )
			{
				StartOffset = format.Start.PhysicalPoint.Offset;
				EndOffset = format.End.PhysicalPoint.Offset;
				TextFormatInfo = new TextFormatInfo( ( Format )format.Format );
			}
		}
		#endregion

		#region Class Constants
		/// <summary>
		/// Used in formats naming.
		/// </summary>
		private const string DEF_FORMAT_NAME = "Format";
		#endregion

		#region Class Initialization
		/// <summary>
		/// Hides class constructor for external use.
		/// </summary>
		internal ControlStateStore()
		{
		}
		/// <summary>
		/// Used in deserialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ControlStateStore( SerializationInfo info, StreamingContext context )
		{
			m_hashFormatsInfo = ( Hashtable )info.GetValue( "Formattings", typeof( Hashtable ) );
			m_bookmarksOffsets = ( ArrayList )info.GetValue( "Bookmarks", typeof( ArrayList ) );
			m_strHashCode = ( string )info.GetValue( "HashCode", typeof( string ) );
		}
		#endregion

		#region Class Members
		/// <summary>
		/// Hashtable with info about formats.
		/// </summary>
		private Hashtable m_hashFormatsInfo = new Hashtable();
		/// <summary>
		/// Array of numbers of bookmarks lines.
		/// </summary>
		private ArrayList m_bookmarksOffsets = new ArrayList();
		/// <summary>
		/// Hashcode for current control text.
		/// </summary>
		private string m_strHashCode;
        /// <summary>
        /// Expand All initialized
        /// </summary>
        private bool m_expandAllIntialized = false;
		/// <summary>
        /// current line index
        /// </summary>
    	private int currentLine = 0;
		/// <summary>
        /// current column index
        /// </summary>
        private int currentColumn = 0;
        #endregion

		#region Class Static Members
		/// <summary>
		/// Static index for formats names.
		/// </summary>
		private static int _formatIndex;
		#endregion

       
        private IList m_serializeCollapsibleregion = new ArrayList();
        /// <summary>
        /// Gets/Sets collapsed region
        /// </summary>
        public IList SerializeCollapsibleRegion
        {
            get { return m_serializeCollapsibleregion; }
            set { m_serializeCollapsibleregion = value; }
        }
        /// <summary>
        /// To Store serialized Line index
        /// </summary>
        internal List<int> m_SerializedLineIndex = new List<int>();

		#region Class Public Methods
		/// <summary>
		/// Stores all needed info about control state.
		/// </summary>
		/// <param name="control">Control to store info about.</param>
		public void StoreData( StreamEditControl control )
		{
			if( control == null ) throw new ArgumentNullException( "control" );
			if (control.EnableMD5 )
			{
			MD5 cr = MD5.Create();
			byte[] hashcCode = cr.ComputeHash( Encoding.UTF8.GetBytes( control.Text ) );

			m_strHashCode = GetStringHashCode( hashcCode );

			}
			DynamicFormatManager formatManager = control.DynamicFormatManager;

			m_hashFormatsInfo.Clear();

			foreach( string layerName in formatManager.Names )
			{
				ArrayList formats = new ArrayList();
				IDynamicFormatsLayer layer = formatManager[ layerName ];

				foreach( IDynamicFormat format in layer )
				{
					FormatInfo formatInfo = new FormatInfo( format );
					formats.Add( formatInfo );
				}

				if( formats.Count > 0 ) m_hashFormatsInfo.Add( layerName, formats );
			}

			m_bookmarksOffsets.Clear();

			foreach( Bookmark bookmark in control.Bookmarks.Bookmarks )
			{
				m_bookmarksOffsets.Add( bookmark.Point.Offset );
			}

            SerializeCollapsibleRegion.Clear();

            SerializeCollapsibleRegion = control.m_parser.GetCollapsedRegionsList();

            for (int i = 0; i < SerializeCollapsibleRegion.Count; i++)
            {
                if (SerializeCollapsibleRegion[i] is CollapsableRegion)
                    m_SerializedLineIndex.Add((SerializeCollapsibleRegion[i] as CollapsableRegion).Start.Line);
            }

            if (SerializeCollapsibleRegion.Count > 0)
                m_expandAllIntialized = false;

            currentLine = control.InternalGetLine(control.CurrentLine, false).LineIndex;
            currentColumn = control.VisualColumn;
		}
		/// <summary>
		/// Restores saved data to control.
		/// </summary>
		/// <param name="control">Control to restore data to.</param>
		/// <param name="append">Specifies whether settings from the state should be applied without clearin currently used settings.</param>
		public void RestoreData( StreamEditControl control, bool append )
		{
			if( control == null ) throw new ArgumentNullException( "control" );

			if( !CanApply( control ) ) throw new Exception( "Control text has changed. The state can't be implemented." );

			DynamicFormatManager formatManager = control.DynamicFormatManager;

			if( !append )
			{
				foreach( IDynamicFormatsLayer layer in formatManager.Layers )
				{
					layer.Clear();
				}
			}

			foreach( string name in m_hashFormatsInfo.Keys )
			{
				IDynamicFormatsLayer layer = formatManager[ name ];

				if( null == layer ) layer = formatManager.RegisterLayer( name );

				//				while( !append && layer.Count > 0 ) 
				//					layer.RemoveAt( 0 );

				IList formats = ( IList )m_hashFormatsInfo[ name ];

				foreach( FormatInfo formatInfo in formats )
				{
					IParsePoint startParsePoint = control.Parser.BaseStream.GetParsePoint( formatInfo.StartOffset );
                    if (InputLanguage.CurrentInputLanguage.Culture.Name.Contains("ar"))
                        startParsePoint = control.Parser.BaseStream.GetParsePoint(formatInfo.StartOffset * 2 + 1);
					CoordinatePoint startPoint = control.Parser.GetCoordinatePoint( startParsePoint, true );
					startPoint.UpdatePhisicalCoordinates();

					IParsePoint endParsePoint = control.Parser.BaseStream.GetParsePoint( formatInfo.EndOffset );
                    if (InputLanguage.CurrentInputLanguage.Culture.Name.Contains("ar"))
                        endParsePoint = control.Parser.BaseStream.GetParsePoint(formatInfo.EndOffset * 2 + 2);
					CoordinatePoint endPoint = control.Parser.GetCoordinatePoint( endParsePoint, true );
					endPoint.UpdatePhisicalCoordinates();

					ISnippetFormat format = ( ( FormatManager )control.Language ).Add( GetFormatName() );

					formatInfo.TextFormatInfo.ApplyFormatInfo( ( Format )format );

					layer.Add( startPoint, endPoint, format );
				}
			}

			if( !append )
				control.Bookmarks.BookmarkClear( false );

			foreach( long offset in m_bookmarksOffsets )
			{
				IParsePoint parsePoint = control.Parser.BaseStream.GetParsePoint( offset );
				CoordinatePoint coordPoint = control.Parser.GetCoordinatePoint( parsePoint );
				control.Bookmarks.BookmarkAdd( coordPoint.VirtualLine );
			}

            foreach (CollapsableRegion item in SerializeCollapsibleRegion)
            {
                if (item.Start.IsValid)
                {
                    if (!m_expandAllIntialized)
                    {
                        control.ExpandAll();
                        m_expandAllIntialized = true;
                    }
                    control.GoToLine(item.Start.Line);
                    control.SerializeCollapse();
                }
            }

            foreach (int item in m_SerializedLineIndex)
            {
                if (!m_expandAllIntialized)
                {
                    control.ExpandAll();
                    m_expandAllIntialized = true;
                }
                control.GoToLine(item);
                control.SerializeCollapse();
            }

            control.CurrentLine = currentLine;
            control.VisualColumn = currentColumn;
		}
		/// <summary>
		/// Checks whether current control state store instance can be applied to certain stream edit control object.
		/// </summary>
		/// <param name="control">Control to check.</param>
		/// <returns>True if state can be applied, therwise false.</returns>
		public bool CanApply( StreamEditControl control )
		{
			if( control == null ) throw new ArgumentNullException( "control" );

			if (control.EnableMD5 )
			{
			MD5 cr = MD5.Create();
			byte[] hashCode = cr.ComputeHash( Encoding.UTF8.GetBytes( control.Text ) );

			return ( m_strHashCode == GetStringHashCode( hashCode ) );
			}
			else
			{
				return true;
			}
		}
		#endregion

		#region Class Private Methods
		/// <summary>
		/// Gets unique format name.
		/// </summary>
		/// <returns>Unique format name.</returns>
		private string GetFormatName()
		{
			return DEF_FORMAT_NAME + ( _formatIndex++ ).ToString();
		}
		/// <summary>
		/// Transforms array of bytes with hash code to string.
		/// </summary>
		/// <param name="hashCode">Array of bytes wth hash code.</param>
		/// <returns>Resulting string with hash code.</returns>
		private string GetStringHashCode( byte[] hashCode )
		{
			StringBuilder strBuilder = new StringBuilder();

			foreach( byte hashItem in hashCode )
			{
				strBuilder.Append( hashItem.ToString() );
			}

			return strBuilder.ToString();
		}
		#endregion

		#region ISerializable Members
		/// <summary>
		/// Creates data for serialization.
		/// </summary>
		/// <param name="info">SerializationInfo.</param>
		/// <param name="context">StreamingContext.</param>
		public void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			int n = m_hashFormatsInfo.Keys.Count;

			info.AddValue( "HashCode", m_strHashCode );
			info.AddValue( "Formattings", m_hashFormatsInfo );
			info.AddValue( "Bookmarks", m_bookmarksOffsets );
		}
		#endregion
	}
}