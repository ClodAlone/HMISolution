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
using System.ComponentModel;

using Syncfusion.Grouping.Sorting;

namespace Syncfusion.Grouping
{
#if false
	/// <summary>
	/// EngineFactory creates <see cref="ElementBase"/> objects to be used in a <see cref="Grouping"/>.
	/// </summary>
	public class EngineFactory
	{
		static EngineFactoryBase factory;
		public static event EventHandler FactoryChanged;

		public static EngineFactoryBase Factory
		{
			get
			{
				if (factory == null)
					factory = new EngineFactoryBase(true);
				return factory;
			}
			set
			{
				if (factory != value)
				{
					factory = value;
					if (FactoryChanged != null)
						FactoryChanged(typeof(EngineFactory), EventArgs.Empty);
				}
			}
		}


		public static AddNewRecord CreateAddNewRecord()
		{
			return Factory.CreateAddNewRecord();
		}

		public static AddNewRecordSection CreateAddNewRecordSection()
		{
			return Factory.CreateAddNewRecordSection();
		}

		public static CaptionSection CreateCaptionSection()
		{
			return Factory.CreateCaptionSection();
		}

		public static ChildTable CreateChildTable()
		{
			return Factory.CreateChildTable();
		}

		public static ColumnHeaderRow CreateColumnHeaderRow()
		{
			return Factory.CreateColumnHeaderRow();
		}

		public static EmptySection CreateEmptySection()
		{
			return Factory.CreateEmptySection();
		}

		public static Engine CreateEngine()
		{
			return Factory.CreateEngine();
		}

		public static ExpressionFieldDescriptor CreateExpressionFieldDescriptor()
		{
			return Factory.CreateExpressionFieldDescriptor();
		}

		public static ExpressionFieldEvaluator CreateExpressionFieldEvaluator()
		{
			return Factory.CreateExpressionFieldEvaluator();
		}

		public static FieldDescriptor CreateFieldDescriptor()
		{
			return Factory.CreateFieldDescriptor();
		}

		public static FilterBarSection CreateFilterBarSection()
		{
			return Factory.CreateFilterBarSection();
		}

		public static Group CreateGroup()
		{
			return Factory.CreateGroup();
		}

		public static GroupsDetails CreateGroupsDetails()
		{
			return Factory.CreateGroupsDetails();
		}

		public static NestedTable CreateNestedTable()
		{
			return Factory.CreateNestedTable();
		}

		public static Record CreateRecord()
		{
			return Factory.CreateRecord();
		}

		public static RecordNestedTablesPart CreateRecordNestedTablesPart()
		{
			return Factory.CreateRecordNestedTablesPart();
		}

		public static RecordRow CreateRecordRow()
		{
			return Factory.CreateRecordRow();
		}

		public static RecordRowsPart CreateRecordRowsPart()
		{
			return Factory.CreateRecordRowsPart();
		}

		public static RecordsDetails CreateRecordsDetails()
		{
			return Factory.CreateRecordsDetails();
		}

		public static RelationDescriptor CreateRelationDescriptor()
		{
			return Factory.CreateRelationDescriptor();
		}

		public static RowElementsSection CreateRowElementsSection()
		{
			return Factory.CreateRowElementsSection();
		}

		public static SummarySection CreateSummarySection()
		{
			return Factory.CreateSummarySection();
		}

		public static Table CreateTable()
		{
			return Factory.CreateTable();
		}

		public static TableDescriptor CreateTableDescriptor()
		{
			return Factory.CreateTableDescriptor();
		}
	}
#endif

	public class EngineFactoryBase
	{
		/// <summary>
		/// Initializes an <see cref="EngineFactoryBase"/>.
		/// </summary>
		public EngineFactoryBase()
		{
			isDefault = false;
		}

		/// <summary>
		/// Initializes an <see cref="EngineFactoryBase"/> and optionally marks it as "Default"
		/// allowing the grid to replace it with a derived factory at any time.
		/// </summary>
		/// <param name="isDefault"></param>
		public EngineFactoryBase(bool isDefault)
		{
			this.isDefault = isDefault;
		}

		bool isDefault;

		/// <summary>
		/// Returns True when this factory has been auto-initialized by ElementBase.
		/// </summary>
		public virtual bool IsDefault
		{
			get
			{
				return isDefault;
			}
		}


		// Factory methods

		public virtual AddNewRecord CreateAddNewRecord()
		{
			return new AddNewRecord();
		}

		public virtual AddNewRecordSection CreateAddNewRecordSection()
		{
			return new AddNewRecordSection();
		}

		public virtual CaptionSection CreateCaptionSection()
		{
			return new CaptionSection();
		}

		public virtual ChildTable CreateChildTable()
		{
			return new ChildTable();
		}

		public virtual ColumnHeaderRow CreateColumnHeaderRow()
		{
			return new ColumnHeaderRow();
		}

		public virtual ColumnHeaderSection CreateColumnHeaderSection()
		{
			return new ColumnHeaderSection();
		}

		public virtual EmptySection CreateEmptySection()
		{
			return new EmptySection();
		}

		public virtual Engine CreateEngine()
		{
			this.
			return new Engine(this);
		}

//		public virtual ExpressionFieldDescriptor CreateExpressionFieldDescriptor()
//		{
//			return new ExpressionFieldDescriptor();
//		}

		public virtual ExpressionFieldEvaluator CreateExpressionFieldEvaluator()
		{
			return new ExpressionFieldEvaluator();
		}

//		public virtual FieldDescriptor CreateFieldDescriptor()
//		{
//			return new FieldDescriptor();
//		}

		public virtual FilterBarSection CreateFilterBarSection()
		{
			return new FilterBarSection();
		}

		public virtual Group CreateGroup()
		{
			return new Group();
		}

		public virtual GroupsDetails CreateGroupsDetails()
		{
			return new GroupsDetails();
		}

		public virtual NestedTable CreateNestedTable()
		{
			return new NestedTable();
		}

		public virtual Record CreateRecord()
		{
			return new Record();
		}

		public virtual RecordNestedTablesPart CreateRecordNestedTablesPart()
		{
			return new RecordNestedTablesPart();
		}

		public virtual RecordRow CreateRecordRow()
		{
			return new RecordRow();
		}

		public virtual RecordRowsPart CreateRecordRowsPart()
		{
			return new RecordRowsPart();
		}

		public virtual RecordsDetails CreateRecordsDetails()
		{
			return new RecordsDetails();
		}

//		public virtual RelationDescriptor CreateRelationDescriptor()
//		{
//			return new RelationDescriptor();
//		}

		public virtual RowElementsSection CreateRowElementsSection()
		{
			return new RowElementsSection();
		}

		public virtual SummarySection CreateSummarySection()
		{
			return new SummarySection();
		}

		public virtual Table CreateTable()
		{
			return new Table();
		}

		public virtual TableDescriptor CreateTableDescriptor()
		{
			return new TableDescriptor();
		}

	}
}
