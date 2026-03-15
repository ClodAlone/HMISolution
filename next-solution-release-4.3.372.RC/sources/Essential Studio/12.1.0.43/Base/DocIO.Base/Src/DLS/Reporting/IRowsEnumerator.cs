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

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for IRowsEnumerator.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public interface IRowsEnumerator
    {
        /// <summary>
        /// 
        /// </summary>
        void Reset();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool NextRow();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        object GetCellValue(string columnName);
        /// <summary>
        /// 
        /// </summary>
        string[] ColumnNames { get; }
        /// <summary>
        /// 
        /// </summary>
        int RowsCount { get; }
        /// <summary>
        /// 
        /// </summary>
        int CurrentRowIndex { get; }
        /// <summary>
        /// 
        /// </summary>
        string TableName { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsEnd { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsLast { get; }
    }
}