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
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Class describing one instance of ColumnGuide.
	/// </summary>
	[TypeConverter( typeof( ColumnGuideItem.ColumnGuideItemConverter ) )]
	[Serializable]
	public class ColumnGuideItem
	{
		#region Internal Classes
		/// <summary>
		/// Class used for work with property grid.
		/// </summary>
		internal class ColumnGuideItemConverter
			: ExpandableObjectConverter
		{
			/// <summary>
			/// Disables instantination.
			/// </summary>
			/// <param name="context">Current context, does not matter.</param>
			/// <returns>False.</returns>
			public override bool GetCreateInstanceSupported( ITypeDescriptorContext context )
			{
				return false;
			}
		}
		#endregion

		#region Class Members
		/// <summary>
		/// Guide column.
		/// </summary>
		private int m_iColumn = 0;
		/// <summary>
		/// Column guide line color.
		/// </summary>
		private Color m_color = Color.LightBlue;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets guide column.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies guide column" )]
		public int Column
		{
			get
			{
				return m_iColumn;
			}
			set
			{
				if( value < 0 )
					throw new ArgumentOutOfRangeException( "Column" );

				m_iColumn = value;
			}
		}
		/// <summary>
		/// Gets or sets column guide line color.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( typeof( Color ), "LightBlue" )]
		[Description( "Specifies column guide line color." )]
		public Color Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates new instance of ColumnGuideItem.
		/// </summary>
		public ColumnGuideItem()
		{
		}
		/// <summary>
		/// Creates new instance of ColumnGuideItem.
		/// </summary>
		/// <param name="column">Guide column.</param>
		public ColumnGuideItem( int column )
			: this( column, Color.LightBlue )
		{
		}
		/// <summary>
		/// Creates new instance of ColumnGuideItem.
		/// </summary>
		/// <param name="column">Guide column.</param>
		/// <param name="color">Column guide line color.</param>
		public ColumnGuideItem( int column, Color color )
		{
			m_iColumn = column;
			m_color = color;
		}
		#endregion
	}
}
