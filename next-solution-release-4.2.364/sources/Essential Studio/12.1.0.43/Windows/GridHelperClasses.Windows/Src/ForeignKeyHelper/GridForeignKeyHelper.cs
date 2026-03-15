//-------------------------------------------------------------------------------------------------
// <copyright file="GridForeignKeyHelper.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data;
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using Syncfusion.Grouping;

    /// <summary>
    /// A helper class to set up foreign key relation. It displays a column
    /// named DisplayCol in the foreign table where a column named LookUpCol
    /// is located in the main table. LookUpCol holds keys values that match
    /// the values in a column named idCol in the foreign table.
    /// </summary>
    public class GridForeignKeyHelper
    {
        /// <summary>
        /// Initializes a new <see cref="GridForeignKeyHelper"/>
        /// </summary>
        public GridForeignKeyHelper()
            : base()
        {
        }

        /// <summary>
        /// Sets up a foreign key relation.
        /// </summary>
        /// <param name="grid">The grouping grid.</param>
        /// <param name="valueColumnInMainTable">Parent column.</param>
        /// <param name="foreignDataTable">Foreign table.</param>
        /// <param name="valueMemberInForeignTable">Child value column.</param>
        /// <param name="displayMemberInForeignTable">Child display column.</param>
        public static void SetupForeignTableLookUp(GridGroupingControl grid, string valueColumnInMainTable, DataTable foreignDataTable, string valueMemberInForeignTable, string displayMemberInForeignTable)
        {
            //// step 1. remember the location of lookupcol so it can be swapped out later
            GridTableDescriptor td = grid.TableDescriptor;

            //// step 2. add it to the grouping engine
            grid.Engine.SourceListSet.Add(foreignDataTable.TableName, foreignDataTable.DefaultView);

            //// step 3. Create and setup a RelationKind.ForeignKeyReference relation

            //// set up relation descriptor that defines mapping between main table and foreign table
            GridRelationDescriptor rd = new GridRelationDescriptor();
            rd.Name = string.Format("{0}", valueColumnInMainTable); ////just some unique name
            rd.RelationKind = RelationKind.ForeignKeyReference; ////foreign key look up
            rd.ChildTableName = foreignDataTable.TableName;  //// SourceListSet name for lookup

            //// get foreign key for col "idCol" in foreign table
            rd.RelationKeys.Add(valueColumnInMainTable, valueMemberInForeignTable); ////col in main table,  foreign key col

            //// step 4. Set any optional properties on the relation
            //// dropdown only shows DisplayCol
            rd.ChildTableDescriptor.VisibleColumns.Add(displayMemberInForeignTable); ////display column
            rd.ChildTableDescriptor.SortedColumns.Add(displayMemberInForeignTable); ////sort it for dropdown display
            rd.ChildTableDescriptor.AllowEdit = false; ////no editing of foreign table
            rd.ChildTableDescriptor.AllowNew = false;  ////no new items added to foreign table
            
            //// step 5. add relation descriptor to main tabledescriptor
            td.Relations.Add(rd);
        }
    }
}