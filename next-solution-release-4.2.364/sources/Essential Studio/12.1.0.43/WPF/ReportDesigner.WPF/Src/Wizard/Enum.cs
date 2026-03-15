#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
	/// <summary>
	/// Specifies the type of the meta tree node
	/// </summary>
	internal enum NodeType
	{
		/// <summary>
		/// Default the node type is Folder
		/// </summary>
		Folder,

		/// <summary>
		/// Node type is Table
		/// </summary>
		Table,

		/// <summary>
		/// Node type is View
		/// </summary>
		View,

		/// <summary>
		/// Node type is StoredProcedure
		/// </summary>
		StoredProcedure,

		/// <summary>
		/// Node type is TableValuedFunction
		/// </summary>
		TableValuedFunction,

		/// <summary>
		/// Node type is TableColumn
		/// </summary>
		TableColumn,
	}

	internal enum ControlPropertyType
	{
		/// <summary>
		/// Default the Control Property type is General
		/// </summary>
		General,

		/// <summary>
		/// Control Property type is Visibility
		/// </summary>
		Visibility,

		///// <summary>
		///// Control Property type is Border
		///// </summary>
		//Border,

		/// <summary>
		/// Control Property type is Fill
		/// </summary>
		Fill,

		/// <summary>
		/// Control Property type is Size
		/// </summary>
		Size,

		/// <summary>
		/// Control Property type is Font
		/// </summary>
		Font,

		/// <summary>
		/// Control Property type is Alignment
		/// </summary>
		Alignment,     

		/// <summary>
		/// Available Values of the Given Parameter(s)
		/// </summary>
		AvailableValues,

		/// <summary>
		/// Control property type is Action
		/// </summary>
		Action,

		/// <summary>
		/// Control property type is Code
		/// </summary>
		Code,

		/// <summary>
		/// Control property type is Reference
		/// </summary>
		Reference,

		/// <summary>
		/// Control property type is Variables
		/// </summary>
		Variable,

		/// <summary>
		/// Control property type is sorting
		/// </summary>
		Sorting,

		/// <summary>
		/// Control property type is filter
		/// </summary>
		Filter,

		/// <summary>
		/// Defaul Values of the Given Parameter(s)
		/// </summary>
		DefaultValues,

		/// <summary>
		/// Advanced Values of the Given Parameter(s)
		/// </summary>
		AdvancedValues,

		/// <summary>
		/// Control Property type is PageSetup
		/// </summary>
		PageSetup,
	
		/// <summary>
		/// Control Property type is PageSetup
		/// </summary>
		Parameters,

		/// <summary>
		/// Control Property type is Label
		/// </summary>
		Label,

		/// <summary>
		/// Control Property type is MajorTick
		/// </summary>
		MajorTick,

		/// <summary>
		/// Control Property type is MinorTick
		/// </summary>
		MinorTick,       

		/// <summary>
		/// Control Property type is Border
		/// </summary>
		Border,

		/// <summary>
		/// Control Property type is Frame
		/// </summary>
		Frame,

		/// <summary>
		/// Control Property type is FrameFill
		/// </summary>
		FrameFill,

		/// <summary>
		/// Control Property type is FrameFill
		/// </summary>
		AreaColor,
		
		/// <summary>
		/// Control Property type is FrameShadow
		/// </summary>
		FrameShadow,

		/// <summary>
		/// Control Property type is Tick
		/// </summary>
		Options3D,

		/// <summary>
		/// Control Property type is Tick
		/// </summary>
		Tick,

		/// <summary>
		/// Control Property type is Style
		/// </summary>
		Style,

		/// <summary>
		/// Control Property type is Axes
		/// </summary>
		Axes
	}

	internal enum ImageSourceType
	{
		/// <summary>
		/// Default the Image Source type is External
		/// </summary>
		//External,

		/// <summary>
		/// Default the Image Source type is Embedded
		/// </summary>
		Embedded,
		Database
	}

	internal enum PageSizeType
	{
		/// <summary>
		/// Default the Page Size type is Letter
		/// </summary>
		Letter
	}

	internal enum HorizontalAlignmentType
	{
		/// <summary>
		/// Default the Alignment type is Default
		/// </summary>
		Default,

		/// <summary>
		/// Default the Alignment type is General
		/// </summary>
		General,

		/// <summary>
		/// Default the Alignment type is Left
		/// </summary>
		Left,

		/// <summary>
		/// Default the Alignment type is Center
		/// </summary>
		Center,

		/// <summary>
		/// Default the Alignment type is Right
		/// </summary>
		Right,
	}

	internal enum VerticalAlignmentType
	{
		/// <summary>
		/// Default the Vertical Alignment type is Default
		/// </summary>
		Default,

		/// <summary>
		/// Default the Vertical Alignment type is Top
		/// </summary>
		Top,

		/// <summary>
		/// Default the Vertical Alignment type is Middle
		/// </summary>
		Middle,

		/// <summary>
		/// Default the Vertical Alignment type is Bottom
		/// </summary>
		Bottom
	}

	internal enum EffectsType
	{
		/// <summary>
		/// Default the Font Effects type is Default
		/// </summary>
		Default,

		/// <summary>
		/// Default the Font Effects type is Top
		/// </summary>
		None,

		/// <summary>
		/// Default the Font Effects type is Middle
		/// </summary>
		Underline,

		/// <summary>
		/// Default the Font Effects type is Bottom
		/// </summary>
		Overline,

		/// <summary>
		/// Default the Font Effects type is Bottom
		/// </summary>
		Strikethrough
	}

	internal enum FontSizeType
	{
		/// <summary>
		/// Default the Font Effects type is Default
		/// </summary>
		Default,

		/// <summary>
		/// Default the Font Effects type is Top
		/// </summary>
		None,

		/// <summary>
		/// Default the Font Effects type is Middle
		/// </summary>
		Underline,

		/// <summary>
		/// Default the Font Effects type is Bottom
		/// </summary>
		Overline,

		/// <summary>
		/// Default the Font Effects type is Bottom
		/// </summary>
		Strikethrough
	}

	internal enum TableJoinType
	{
		/// <summary>
		/// Default the Table Join type is InnerJoin
		/// </summary>
		InnerJoin,

		/// <summary>
		/// Default the Table Join type is LeftOuterJoin
		/// </summary>
		LeftOuterJoin,

		/// <summary>
		/// Default the Table Join type is RightOuterJoin
		/// </summary>
		RightOuterJoin,

		/// <summary>
		/// Default the Table Join type is FullOuterJoin
		/// </summary>
		FullOuterJoin
	}

	internal enum TextBoxPropertyType
	{
		/// <summary>
		/// TextBox Propery
		/// </summary>
		
		TextBoxProperty,

		/// <summary>
		/// Creating a Place Holder
		/// </summary>
		CreatePlaceHolder,

		/// <summary>
		/// Place Holder Property
		/// </summary>
		PlaceHolderProperty,

		TextProperty


	}

	internal enum TablixRegion
	{        
		TablixCorner,

		TablixRowHierarchy,

		TablixColumnHierarchy,

		TablixBody
	}

	internal enum TablixHierarchyType
	{
		TablixRowHierarchy,
		TablixColumnHierarchy
	}

	internal enum TablixRegionBorderType
	{
		Left,
		Right,
		Top,
		Bottom
	}

	internal enum InsertAt
	{
		Left,
		Right,
		Above,
		Below
	}

	internal enum PlaceHolderType
	{
		Field,
		Sum,
		Count,
		First,
		Avg,
		Expression
	}

	internal enum TablixPropertyType
	{
		Tablix,
		Column
	}
}
