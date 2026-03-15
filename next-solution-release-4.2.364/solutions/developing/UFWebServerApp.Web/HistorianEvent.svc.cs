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
    public class HistorianEvent : DataStoreService {
        public static IDataStore DataStore;
        static HistorianEvent() {
            DataStore = XpoDefault.GetConnectionProvider(Global.ConnectionStringEvent, AutoCreateOption.DatabaseAndSchema);
        }
        public HistorianEvent()
            : base(DataStore) {
        }
    }
}
