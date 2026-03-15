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

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for WPStylesData.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WPStylesData
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private WPTablesData m_tablesData;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializing constructor
        /// </summary>
        /// <param name="tables"></param>
        [CLSCompliant(false)]
        internal WPStylesData(WPTablesData tables)
        {
            m_tablesData = tables;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets stylesheet information
        /// </summary>
        [CLSCompliant(false)]
        internal StyleSheetInfoRecord StyleSheetInfo
        {
            get
            {
                return m_tablesData.StyleSheetInfo;
            }
        }

        /// <summary>
        /// Gets stylesheet definitions
        /// </summary>
        [CLSCompliant(false)]
        internal StyleDefinitionRecord[] StyleDefinitions
        {
            get
            {
                return m_tablesData.StyleDefinitions;
            }
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Gets style defintion record by the specified style id
        /// </summary>
        /// <param name="styleID"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        internal StyleDefinitionRecord GetStyleRecordByID(int styleID)
        {
            for (int i = 0; i < StyleDefinitions.Length; i++)
            {
                StyleDefinitionRecord record = StyleDefinitions[i];
                if (record.StyleId == styleID)
                    return record;

            }

            return null;
        }
        #endregion internal methods
    }
}