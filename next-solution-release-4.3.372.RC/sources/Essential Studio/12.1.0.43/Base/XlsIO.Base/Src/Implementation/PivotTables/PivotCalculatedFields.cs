#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    public class PivotCalculatedFields:
        List<PivotFieldImpl>,
        IPivotCalculatedFields
    {
        #region members
        private PivotTableImpl m_pivotTable;
        #endregion
        #region Initializer
        public PivotCalculatedFields(PivotTableImpl pivotTable)
        {
            m_pivotTable = pivotTable;
        }

        #endregion
        #region Implementation Properties

        /// <summary>
        /// Add new calculated field to the pivot table.
        /// </summary>
        /// <param name="name">name of the new field. </param>
        /// <param name="formula">formula for the calculated field.</param>
        /// <returns>returns calculated field.</returns>
        IPivotField IPivotCalculatedFields.Add(string name, string formula)
        {
            PivotCacheImpl cache=m_pivotTable.Cache;
            PivotCacheFieldImpl cacheField= cache.CacheFields.AddNewField(name,formula);
            PivotFieldImpl field=new PivotFieldImpl(cacheField,m_pivotTable);
            m_pivotTable.AddPivotField(PivotAxisTypes.None, field,true);

            int count=m_pivotTable.DataFields.Count;
            string fieldName=PivotTableImpl.DefaultDataFieldStart+count;
            field.CanDragToColumn = false;
            field.CanDragToPage = false;
            field.CanDragToRow = false;
            field.Axis = PivotAxisTypes.None;
            m_pivotTable.SetChanged(true);
            return field;
        }
        
     
        

        /// <summary>
        /// Returns single entry from the collection.
        /// </summary>
        /// <param name="index">Item index to return.</param>
        /// <returns>Single entry from the collection.</returns>
        public IPivotField this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index");
                return base[index];
            }
        }
        /// <summary>
        /// Returns the single entry 
        /// </summary>
        /// <param name="name">name of the calculated field</param>
        /// <returns>calculated pivot field for the specified name</returns>
        IPivotField IPivotCalculatedFields.this[string name]
        {
            get
            {
                foreach (PivotFieldImpl field in this)
                {
                    if (field.Name == name)
                    {
                        return field;
                    }
                }
                return null;
            }
        }
        #endregion
    }
}
