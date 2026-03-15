using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System.Configuration;

namespace UFUAWebEditor
{
    /// <summary>
    /// Summary description for XpoHelper
    /// </summary>
    public static class XpoHelper
    {
        public static Session GetNewSession()
        {
            var ret = new Session(DataLayer);

            // set XpoDefault.Session to null to prevent accidental use of XPO default session
            lock (lockObject)
            {
                if (XpoDefault.Session == null)
                    XpoDefault.Session = ret;
            }

            return ret;
        }

        public static UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }

        private readonly static object lockObject = new object();

        static IDataLayer fDataLayer;
        static IDataLayer DataLayer
        {
            get
            {
                if (fDataLayer == null)
                {
                    lock (lockObject)
                    {
                        fDataLayer = GetDataLayer();
                    }
                }
                return fDataLayer;
            }
        }

        private static IDataLayer GetDataLayer()
        {
            Configuration rootWebConfig =
                            System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");

            String conn = null;
            KeyValueConfigurationCollection settings = rootWebConfig.AppSettings.Settings;
            var server = settings["server"];
            var database = settings["database"];
            var updateschema = settings["updateschema"];
            if (server != null && database != null)
                conn = MSSqlConnectionProvider.GetConnectionString(server.Value, database.Value);
            else
            {
                ConnectionStringSettings connString;
                if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
                {
                    connString =
                        rootWebConfig.ConnectionStrings.ConnectionStrings["ApplicationServices"];
                    if (connString != null)
                        conn = connString.ConnectionString;
                }
                // conn = MSSqlConnectionProvider.GetConnectionString("(local)", "TestUFUAServer");
            }

            XPDictionary dict = new ReflectionDictionary();

            /* Note: The database schema must exactly match your persistent classes' definition. ASP.NET applications
             * should not update the database schema themselves; moreover, they often don't have enough permission
             * to read the database schema.  You may need to create a separate tool which manages your database.
             * On the one hand, we recommend using AutoCreateOption.SchemaAlreadyExists for the XPO data layer of your
             * Web site.  On the other hand, the ThreadSafeDataLayer intensively uses data which is stored in the XPO's
             * XPObjectType service table.  This sample project maps to the XpoWebTest database on your MS SQL Server.
             * This database is created via the DatabaseUpdater project, which must be launched before this Site is 
             * published. */

            IDataStore store = XpoDefault.GetConnectionProvider(conn, updateschema != null ? AutoCreateOption.DatabaseAndSchema : AutoCreateOption.SchemaAlreadyExists);

            // initialize the XPO dictionary
            dict.GetDataStoreSchema(typeof(UFUAModel.UFUATag).Assembly);

            // create a ThreadSafeDataLayer
            IDataLayer dl = new ThreadSafeDataLayer(dict, store);

            return dl;
        }
    }
}