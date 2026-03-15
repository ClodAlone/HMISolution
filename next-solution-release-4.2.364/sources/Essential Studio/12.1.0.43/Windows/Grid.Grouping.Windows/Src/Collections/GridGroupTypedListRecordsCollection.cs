//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupTypedListRecordsCollection.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Data;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms.Grid;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class GridGroupTypedListRecordsCollection : GroupTypedListRecordsCollection, IGridListControlSource
    {
        GridTableDescriptor td;
        GridTable table;
        PropertyDescriptorCollection properties = null;
        int propertiesVersion = -1;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridGroupTypedListRecordsCollection(Group group)
            : base(group)
        {
            group.ParentTableDescriptor.PropertyChanged += new DescriptorPropertyChangedEventHandler(this.TableDescriptor_PropertyChanged);
            this.td = (GridTableDescriptor) Group.ParentTableDescriptor;
            this.table = (GridTable) Group.ParentTable;
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                return this.td;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridTable Table
        {
            get
            {
                return this.table;
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="listAccessors">The listAccessors</param>
        /// <returns>returns the PropertyDescriptorCollection</returns>
        /// <internalonly/>
        public override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (this.properties == null || propertiesVersion != td.VisibleColumns.Version)
            {
                ArrayList pds = new ArrayList();

                // Visible columns
                foreach (GridVisibleColumnDescriptor visibleColumn in this.td.VisibleColumns)
                {
                    FieldDescriptor field = this.td.Fields[visibleColumn.Name];
                    if (field != null)
                    {
                        pds.Add(new TableFieldPropertyDescriptor(field));
                    }
                }

                // All other fields (so that listbox logic with Display/Value member is not affected)
                foreach (FieldDescriptor field in this.td.Fields)
                {
                    if (!td.VisibleColumns.Contains(field.Name))
                    {
                        pds.Add(new TableFieldPropertyDescriptor(field));
                    }
                }

                pds.Add(new TableRecordDataPropertyDescriptor(typeof(object)));   // __data__ to GetData()
                pds.Add(new TableRecordIndexPropertyDescriptor());  // __index__ RecordIndex.

                this.properties = new PropertyDescriptorCollection((PropertyDescriptor[]) pds.ToArray(typeof(PropertyDescriptor)));
                this.propertiesVersion = td.VisibleColumns.Version;
            }

            return this.properties;
        }

        #region IGridListControlSource Members

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns the visible column count</returns>
        /// <internalonly/>
        public int GetVisibleColumnCount()
        {
            return this.td.VisibleColumns.Count;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="columnIndex">The column index</param>
        /// <returns>returns the width</returns>
        /// <internalonly/>
        public int GetWidth(int columnIndex)
        {
            GridVisibleColumnDescriptor field = this.td.VisibleColumns[columnIndex];
            GridColumnDescriptor column = this.td.Columns[field.Name];
#if ASPNET
#else
            if (column.Width == -1)
            {
                if (this.table.TableModel == null)
                {
                    this.table.TableModel = new GridTableModel();
                }

                this.table.TableModel.Table = this.table;
                this.table.TableModel.UpdateColumnWidths();
            }
#endif
            return column.Width;
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public void SetWidth(int columnIndex, int width)
        {
            GridVisibleColumnDescriptor field = this.td.VisibleColumns[columnIndex];
            this.td.Columns[field.Name].Width = width;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="recordNum">The recordNum</param>
        /// <param name="columnIndex">The columnIndex</param>
        /// <returns>returns the grid style info</returns>
        /// <internalonly/>
        public GridStyleInfo GetStyle(int recordNum, int columnIndex)
        {
            if (recordNum == -1)
            {
                //// Headers
                int rowIndex = this.table.GetRangeOfColumnHeaderSection().Top;
                return this.table.CreateTableCellStyle(this.table.DisplayElements[rowIndex], rowIndex, columnIndex + td.GroupedColumns.Count);
            }

            Record r = this[recordNum];
            return this.table.CreateTableCellStyle(r, this.table.DisplayElements.IndexOf(r), columnIndex + td.GroupedColumns.Count);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public event EventHandler ItemPropertiesChanged;

        private void TableDescriptor_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            this.properties = null;
            if (this.ItemPropertiesChanged != null)
            {
                this.ItemPropertiesChanged(this, EventArgs.Empty);
            }
        }
     
        #endregion
    }
}
