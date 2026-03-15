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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;
using System.Data;
using System.Text;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// The information needed for setting the attributes of a column in the drop down 
	/// list of the <see cref="AutoComplete"/> control.
	/// </summary>
	/// <remarks> 
	/// The <see cref="AutoComplete"/> control supports displaying multiple columns
	/// of data for probable matching items. This class specifies the appearance
	/// and behavior of each column that should be visible.
	/// <para>
	/// In the case that the <see cref="AutoComplete.DataSource"/> is an external 
	/// <see cref="DataTable"/> the <see cref="DataColumn"/> objects in the DataTable
	/// will be mirrored in the <see cref="AutoComplete.Columns"/> property which
	/// is a collection of these objects.
	/// </para>
	/// <para>
	/// The <see cref="ColumnHeaderText"/> , <see cref="MinColumnWidth"/>, 
	/// <see cref="ColumnName"/> and <see cref="Visible"/>
	/// properties specify the appearance of the column at runtime.
	/// </para>
	///	<para>
	///	The <see cref="MatchingColumn"/> and <see cref="ImageColumn"/> properties
	///	specify how the column is to be treated at runtime. If a Column is set to
	///	be the MatchingColumn (the <see cref="MatchingColumn"/> property is set to true)
	///	that column will be used for matching against the current text of the target
	///	edit control of the <see cref="AutoComplete"/> control.
	///	</para>
	/// </remarks>
	[
		TypeConverter(typeof(Syncfusion.Windows.Forms.Tools.Design.DataColumnInfoConverter)),
	
	DesignTimeVisible(false),
	ToolboxItem(false)
	]
	public sealed class AutoCompleteDataColumnInfo: Component
	{
		/// <summary>
		/// The text of the column header.
		/// </summary>
		private string	columnHeaderTextValue;

		/// <summary>
		/// The minimum width for the column header.
		/// </summary>
		private int		minColumnWidthValue;

		/// <summary>
		/// 
		/// </summary>
		private bool visible;

		/// <summary>
		/// 
		/// </summary>
		private bool matchingColumn;

		/// <summary>
		/// 
		/// </summary>
		private bool imageColumn;

		/// <summary>
		/// Used internally even if the column display text is changed.
		/// </summary>
		private string columnName;

		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public AutoCompleteDataColumnInfoCollection columnsCollection;

		/// <summary>
		/// Initializes an object of type AutoCompleteDataColumnInfo. Overloaded.
		/// </summary>
		/// <remarks>
		/// The AutoCompleteDataColumnInfo class holds the information needed to intialize one
		/// column in the drop down list view of the <see cref="AutoComplete"/> control.
		/// The initial values for the column name and the default header width
		/// are set.
		/// </remarks>
		public AutoCompleteDataColumnInfo()
		{
			this.columnHeaderTextValue = String.Empty;
			this.minColumnWidthValue = AutoComplete.MinColumnWidth;
			this.visible = true;
			this.columnName = String.Empty;
		}

		/// <summary>
		/// Initializes an object of type AutoCompleteDataColumnInfo.
		/// </summary>
		/// <param name="headerText">The text for the column header.</param>
		/// <param name="width">The width of the column.</param>
		/// <param name="visible">Indicates whether the column is to be visible.</param>
		/// <remarks>
		/// This constructor takes the header text, the minimum width and the visible value of the
		/// column as parameters to the constructor. You can also use the default constructor and 
		/// then set the column header using the <see cref="ColumnHeaderText"/>
		/// property and the minimum width by setting <see cref="MinColumnWidth"/> property.
		/// </remarks>
		public AutoCompleteDataColumnInfo(string headerText, int width, bool visible)
		{
			this.columnHeaderTextValue = headerText;
			this.minColumnWidthValue  = width;
			this.visible = visible;
			this.columnName = headerText;
		}

	
		/// <summary>
		/// 
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		private bool ShouldSerializeMinColumnWidth()
		{
			return this.MinColumnWidth != AutoComplete.MinColumnWidth;
		}

		/// <summary>
		/// 
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)] 
		private void ResetMinColumnWidth()
		{
			this.MinColumnWidth = AutoComplete.MinColumnWidth;
		}
		
		/// <summary>
		/// PropertyChanged event handler.
		/// </summary>
		[ 
		Browsable(false),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;	

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			if(this.PropertyChanged != null)
			{
				try
				{
					PropertyChanged(this, args);
				}
				catch(Exception)
				{
				}
			}
		}

		/// <summary>
		/// Gets / sets the minimum column width for the column to be inserted 
		/// in the ListView of the drop down list in a <see cref="AutoComplete"/> control.
		/// </summary>
		/// <remarks>The MinColumnWidth value will be used when inserting 
		/// columns into the drop down list view of the <see cref="AutoComplete"/> control.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance")
		]
		public int MinColumnWidth
		{
			get
			{
				return this.minColumnWidthValue;
			}

			set
			{
				if(value >0)
					this.minColumnWidthValue = value;
			}
		}

		/// <summary>
		/// Gets / sets the column header text for the column to be inserted in 
		/// the <see cref="AutoComplete"/> class.
		/// </summary>
		/// <remarks>
		/// The default value is set to a empty string. The <see cref="ColumnName"/>
		/// property is a read only property and its initial value is set to be the
		/// same as the ColumnHeaderText.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance")
		]
		public string ColumnHeaderText
		{
			get
			{
				return this.columnHeaderTextValue;
			}

			set
			{
				this.columnHeaderTextValue = value;
				if(this.columnName == String.Empty)
					this.columnName = value;
			}
		}


		/// <summary>
		/// Sets the <see cref="ColumnName"/> property.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <remarks>
		/// The <see cref="ColumnName"/> property is kept as a 
		/// read only property and the initial value of the <see cref="ColumnName"/>
		/// property is set to be the <see cref="ColumnHeaderText"/> property.
		/// <para>
		/// This method is provided in case there is a need for changing
		/// the <see cref="ColumnName"/> after being set from a data source
		/// or through the <see cref="ColumnHeaderText"/> property.
		/// </para>
		/// </remarks>
		public void SetColumnName(string columnName)
		{
			this.columnName = columnName;
		}

		/// <summary>
		/// Returns the name of the column.
		/// </summary>
		/// <remarks>
		/// This is a read only property and the initial value is
		/// either set from the column name of a <see cref="DataColumn"/>
		/// that this column represents or the <see cref="ColumnHeaderText"/>
		/// that is set initially for this column.
		/// <para>
		/// The <see cref="ColumnHeaderText"/> and ColumnName properties
		/// can have different values.
		/// </para>
		/// <para>
		/// The ColumnName is important in the matching process when the
		/// data source is a external <see cref="DataTable"/> as the 
		/// filtering for the probable match list is performed based on
		/// ColumnName.
		/// </para>
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance")
		]
		public string ColumnName
		{
			get
			{
				return this.columnName;
			}
		}

		/// <summary>
		/// Indicates whether this column is to be visible.
		/// </summary>
		/// <remarks>
		/// This property affects the visibility of this column at runtime.
		/// Any column that is set to be the matching column cannot be set to
		/// be invisible.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance")
		]
		public bool Visible
		{
			get
			{
				return this.visible;
			}

			set
			{
				if(this.matchingColumn == true && value == false)
				{
					//Matching column has to be visible
				}
				else
					this.visible = value;
			}
		}

		/// <summary>
		/// Indicates whether the column that this item represents is to be treated
		/// as the matching column.
		/// </summary>
		/// <remarks>
		/// When the value is set to true and this <see cref="AutoCompleteDataColumnInfo"/>
		/// item belongs to a collection all other items are set to non matching columns.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance")
		]
		public bool MatchingColumn
		{
			get
			{
				return this.matchingColumn;
			}

			set
			{
				bool oldValue = this.matchingColumn;
				this.matchingColumn = value;

				//Set the other entries
				if(this.columnsCollection != null)
				{
					if(this.matchingColumn != oldValue && value == true)
					{
						this.visible = true;
						foreach( AutoCompleteDataColumnInfo info in this.columnsCollection)
						{
							if(info != this)
								info.MatchingColumn = false;
						}
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the column that this item represents is to be treated
		/// as the image column.
		/// </summary>
		/// <remarks>
		/// When the value is set to true and this <see cref="AutoCompleteDataColumnInfo"/>
		/// item belongs to a collection all other items are set to non image columns.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance")
		]
		public bool ImageColumn
		{
			get
			{
				return this.imageColumn;
			}

			set
			{
				bool oldValue = this.imageColumn;
				this.imageColumn = value;

				if(this.columnsCollection != null)
				{
					//Set the other entries
					if(this.imageColumn != oldValue && value == true)
					{
						foreach(AutoCompleteDataColumnInfo info in this.columnsCollection)
						{
							if(info != this)
								info.ImageColumn = false;
						}
					}
				}
			}
		}
	}
}