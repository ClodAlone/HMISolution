//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace UFWebServerApp.Web
{
    public class HistorianData : DataStoreService {
        public static IDataStore DataStore;
        static HistorianData() {
            DataStore = XpoDefault.GetConnectionProvider(Global.ConnectionStringData, AutoCreateOption.DatabaseAndSchema);
        }
        public HistorianData()
            : base(DataStore) {
        }
    }
}
