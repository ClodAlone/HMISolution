//-------------------------------------------------------------------------------------------------
// <copyright file="SSASDataProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Reports;

namespace Syncfusion.Windows.Reports.Data
{
    internal class SSASDataProvider
    {

        public SSASDataProvider()
        {

        }

        public OlapData GetData(string connectionString, string query, string dataSetName)
        {
            OlapData dataSet = new OlapData();
            OlapDataManager dataManager;
            OlapReport dataReport = new OlapReport();
            dataManager = GetDataManager(connectionString, query);

            dataSet.DataManager = dataManager;
            dataSet.Nane = dataSetName;
            return dataSet;
        }

        private OlapDataManager GetDataManager(string connectionString, string query)
        {
            OlapDataManager olapDataManager;

            olapDataManager = new OlapDataManager(connectionString);
            //olapDataManager.ExecuteCellSet(query);
            return olapDataManager;
        }
    }
}
