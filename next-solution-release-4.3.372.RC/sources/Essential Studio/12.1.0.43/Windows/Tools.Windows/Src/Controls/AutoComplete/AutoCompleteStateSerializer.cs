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
using System.Drawing;
using System.Runtime.Serialization;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary></summary>
	[ Serializable ]
	public class AutoCompleteStateSerializer : ISerializable
	{
		#region Class constants
		/// <summary></summary>
		private const string c_strDROP_DOWN_HEIGHT = "DropDownHeight";
		/// <summary></summary>
		private const string c_strDROP_DOWN_WIDTH = "DropDownWidth";
		#endregion

		#region Class members
		/// <summary></summary>
		private int m_nDropDownHeight;
		/// <summary></summary>
		private int m_nDropDownWidth;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets the height of the drop down.
		/// </summary>
		public int DropDownHeight
		{
			get
			{
				return m_nDropDownHeight;
			}
			set
			{
				if( m_nDropDownHeight != value )
				{
					m_nDropDownHeight = value;
				}
			}
		}

		/// <summary></summary>
		public int DropDownWidth
		{
			get
			{
				return m_nDropDownWidth;
			}
			set
			{
				if( m_nDropDownWidth != value )
				{
					m_nDropDownWidth = value;
				}
			}
		}
		#endregion

		#region Class initialize/finalize methods
		/// <summary></summary>
		/// <param name="acSrc"/>
		public AutoCompleteStateSerializer( AutoComplete acSrc )
		{
			PopupHost puDropDown = acSrc.AutoCompletePopup.PopupHost as PopupHost;
			Size size = ( ( SizablePopupHost )puDropDown ).LastSize;

			m_nDropDownHeight = size.Height;
			m_nDropDownWidth = size.Width;
		}

		/// <summary></summary>
		/// <param name="info"/>
		/// <param name="context"/>
		protected AutoCompleteStateSerializer( SerializationInfo info, StreamingContext context )
		{
			m_nDropDownHeight = info.GetInt32( c_strDROP_DOWN_HEIGHT );
			m_nDropDownWidth = info.GetInt32( c_strDROP_DOWN_WIDTH );
		}
		/// <summary></summary>
		/// <param name="info"/>
		/// <param name="context"/>
		public void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( c_strDROP_DOWN_HEIGHT, this.DropDownHeight );
			info.AddValue( c_strDROP_DOWN_WIDTH, this.DropDownWidth );
		}
		#endregion
	}

}