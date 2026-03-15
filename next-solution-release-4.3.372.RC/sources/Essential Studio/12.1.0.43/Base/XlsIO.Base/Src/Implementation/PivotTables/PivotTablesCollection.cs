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

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Summary description for PivotTablesCollection.
    /// </summary>
    public class PivotTableCollection
      : CollectionBaseEx<object>
      , ICloneParent
      , IEnumerable<PivotTableImpl>
      , IPivotTables
    {
        #region Properties
        /// <summary>
        /// Gets single entry from the collection.
        /// </summary>
        /// <param name="index">Zero-based index of the item to get.</param>
        /// <returns>Single entry from the collection.</returns>
        public IPivotTable this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index");

                return (IPivotTable)InnerList[index];
            }
        }
        /// <summary>
        /// Gets single entry from the collection.
        /// </summary>
        /// <param name="index">Zero-based index of the item to get.</param>
        /// <returns>Single entry from the collection.</returns>
        public IPivotTable this[string name]
        {
            get
            {
                IPivotTable result = null;

                foreach (IPivotTable table in InnerList)
                {
                    if (table.Name == name)
                    {
                        result = table;
                        break;
                    }
                }

                return result;
            }
        }
        public WorksheetImpl ParentWorksheet
        {
            get
            {
                return FindParent(typeof(WorksheetImpl)) as WorksheetImpl;
            }
        }

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Creates collection and sets its Application and Parent values.
        /// </summary>
        /// <param name="application">
        /// Application object that represents the Excel application.
        /// </param>
        /// <param name="parent">Parent object of this collection.</param>
        public PivotTableCollection(IApplication application, object parent)
            : base(application, parent)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parses collection of pivot tables.
        /// </summary>
        /// <param name="data">Records with pivot table data.</param>
        /// <param name="iPos">Offset to the first pivot table record.</param>
        /// <returns>Offset to the record after</returns>
        public int Parse(IList data, int iPos)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (iPos < 0 || iPos > data.Count - 1)
                throw new ArgumentOutOfRangeException("iPos", "Value cannot be less than 0 and greater than data.Length - 1");

            BiffRecordRaw record = (BiffRecordRaw)data[iPos];

            while (record.TypeCode == PivotTableImpl.DEF_FIRSTRECORD_CODE)
            {
                PivotTableImpl table = new PivotTableImpl(Application, this);
                iPos = table.Parse(data, iPos);
                record = (BiffRecordRaw)data[iPos];
                base.Add(table);
            }

            return iPos;
        }
        /// <summary>
        /// Saves collection of pivot tables into OffsetArrayList.
        /// </summary>
        /// <param name="records">OffsetArrayList that will get all collection's records.</param>
        [CLSCompliant(false)]
        public void Serialize(OffsetArrayList records)
        {
            if (records == null)
                throw new ArgumentNullException("records");

            for (int i = 0, len = Count; i < len; i++)
            {
                PivotTableImpl table = (PivotTableImpl)InnerList[i];
                table.Serialize(records);
            }
        }
        /// <summary>
        /// Adds specified table to the collection.
        /// </summary>
        /// <param name="table">Table to add.</param>
        public void Add(PivotTableImpl table)
        {
            base.Add(table);
        }
        public IPivotTable Add(string name, IRange location, IPivotCache cache)
        {
            PivotTableImpl table = new PivotTableImpl(Application, this, cache.Index, location);
            table.Name = name;
            table.Cache.IsRefreshOnLoad = true;
            table.Cache.IsSaveData = true;
            PivotTableOptions settings = table.Options as PivotTableOptions;
            settings.IsWHAutoFormat = true;
            Add(table);
            return table;
        }
        /// <summary>
        /// Creates copy of the current collection.
        /// </summary>
        /// <param name="worksheet">Parent worksheet for the new collection.</param>
        /// <returns>Created collection.</returns>
        public PivotTableCollection Clone(WorksheetImpl worksheet, Dictionary<string, string> hashWorksheetNames)
        {
            PivotTableCollection result = new PivotTableCollection(worksheet.Application, worksheet);
            WorkbookImpl book = worksheet.ParentWorkbook;

            for (int i = 0, len = Count; i < len; i++)
            {
                PivotTableImpl table = (PivotTableImpl)this[i];
                table = table.Clone(result, hashWorksheetNames);
                //table.Index = ++book.MaxTableIndex;
                result.Add(table);
            }

            return result;
        }
        /// <summary>
        /// Removes the pivot table from the collection
        /// </summary>
        /// <param name="name">pivot table name to remove.</param>
        public void Remove(string name)
        {
            int i = 0;
            PivotTableImpl pivotTable = null;
            foreach (PivotTableImpl table in InnerList)
            {
                if (table.Name.Equals(name))
                {
                    pivotTable = table;
                    break;
                }
                i++;
            }
            if (pivotTable != null)
            {
                InnerList.RemoveAt(i);
                CleanAfterRemove(pivotTable);
            }
        }
        public void RemoveAt(int index)
        {
            string name = this[index].Name;
            Remove(name);
        }
        /// <summary>
        /// Cleans the cache and worksheet data before remove the Pivot Table
        /// </summary>
        /// <param name="pivotTable">pivot Table to remove</param>
        private void CleanAfterRemove(PivotTableImpl pivotTable)
        {
            WorkbookImpl book = pivotTable.Workbook as WorkbookImpl;
            PivotCacheCollection pivotCaches = book.PivotCaches;
            book.RemoveCache(pivotTable.CacheIndex);

            pivotTable.ClearPivotRange();
        }
        /// <summary>
        /// Clears the pivot table without checks the pivot caches.
        /// </summary>
        internal void ClearWithoutCheck()
        {
            base.Clear();
        }

        #endregion
        #region IEnumerable<PivotCacheImpl> Members

        public IEnumerator<PivotTableImpl> GetEnumerator()
        {
            foreach (PivotTableImpl table in InnerList)
            {
                yield return table;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (PivotTableImpl table in InnerList)
            {
                yield return table;
            }
        }

        #endregion
    }
}
