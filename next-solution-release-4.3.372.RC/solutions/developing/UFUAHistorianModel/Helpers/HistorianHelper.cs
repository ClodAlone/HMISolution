using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace UFUAHistorianModel.Helpers
{
    public class AuditDataLogInfo
    {
        public int Oid;
    }

    public class HistorianHelper
    {
        static Dictionary<Type, Dictionary<String, Type>> encryptionFieldsCache = new Dictionary<Type, Dictionary<string, Type>>();
        static object objMutex = new object();
        public static Dictionary<String, Type> GetEncryptionFields<T>()
        {
            lock (encryptionFieldsCache)
            {
                var type = typeof(T);
                if (!encryptionFieldsCache.ContainsKey(type))
                {
                    encryptionFieldsCache.Add(type, new Dictionary<String, Type>());
                    var properties = TypeDescriptor.GetProperties(type, new Attribute[] { new EncryptionAttribute() });
                    foreach (PropertyDescriptor prop in properties)
                        encryptionFieldsCache[type].Add(prop.Name, prop.PropertyType);

                }

                return encryptionFieldsCache[type];
            }
        }

        public static UFUAAuditDataLog GetOrCreateHistorianAuditDataLog(UFUAHistorianLogEntity entity, UnitOfWork ufw)
        {
            var list = (from entry in new XPQuery<UFUAAuditDataLog>(ufw)/*.AsParallel()*/
                        where entry.NodeId == entity.NodeId
                        select entry).ToList();

            if (list.Count > 0)
                return list[0];

            return new UFUAAuditDataLog(ufw)
            {
                Name = entity.Name,
                Description = entity.Description,
                HistoricalName = entity.HistoricalName,
                NodeId = entity.NodeId
            };
        }

        static Dictionary<String, AuditDataLogInfo> mapAuditDataLogInfo = new Dictionary<String, AuditDataLogInfo>();
        public static AuditDataLogInfo GetAuditDataLogInfo(String nodeId, UnitOfWork ufw)
        {
            lock (mapAuditDataLogInfo)
            {
                if (!mapAuditDataLogInfo.ContainsKey(nodeId))
                {
                    var item = (from entry in new XPQuery<UFUAAuditDataLog>(ufw)/*.AsParallel()*/
                                where entry.NodeId == nodeId
                                select entry).FirstOrDefault();

                    if (item == null)
                        return null;

                    mapAuditDataLogInfo.Add(nodeId, new AuditDataLogInfo() { Oid = item.Oid });
                }

                return mapAuditDataLogInfo[nodeId];
            }
        }

        public static int GetFirstOid<T>(UnitOfWork ufw) where T : XPObject
        {
            return (from entry in new XPQuery<T>(ufw)/*.AsParallel()*/
                    orderby entry.Oid ascending
                    select entry.Oid).FirstOrDefault();
        }

        public static T GetFirstEntry<T>(UnitOfWork ufw, int dataLogRef = -1) where T : XPObject
        {
            if (typeof(T) == typeof(UFUAAuditDataItem))
            {
                return (from entry in new XPQuery<UFUAAuditDataItem>(ufw)/*.AsParallel()*/
                        where entry.DataLogRef == dataLogRef
                        orderby entry.Oid ascending
                        select entry).FirstOrDefault() as T;
            }
            else
            {
                return (from entry in new XPQuery<T>(ufw)/*.AsParallel()*/
                        orderby entry.Oid ascending
                        select entry).FirstOrDefault();
            }

        }

        public static int GetLastOid<T>(UnitOfWork ufw) where T :  XPObject
        {
            return (from entry in new XPQuery<T>(ufw)/*.AsParallel()*/
                    orderby entry.Oid descending
                    select entry.Oid).FirstOrDefault();
        }

        public static T GetLastEntry<T>(UnitOfWork ufw, int dataLogRef = -1) where T : XPObject
        {
            if (typeof(T) == typeof(UFUAAuditDataItem))
            {
                return (from entry in new XPQuery<UFUAAuditDataItem>(ufw)/*.AsParallel()*/
                        where entry.DataLogRef == dataLogRef
                        orderby entry.Oid descending
                        select entry).FirstOrDefault() as T;
            }
            else
            {
                return (from entry in new XPQuery<T>(ufw)/*.AsParallel()*/
                        orderby entry.Oid descending
                        select entry).FirstOrDefault();
            }
        }

        public static bool ExistOid<T>(UnitOfWork ufw, int oid) where T : XPObject
        {
            var list = (from entry in new XPQuery<T>(ufw)/*.AsParallel()*/
                        where entry.Oid == oid
                        select entry.Oid).ToList();

            return list.Count > 0;
        }

        public static bool TryUpdateSchema<T>(String settings, params Type[] extendedTypes) where T : XPObject
        {
            try
            {
                UpdateSchema<T>(settings, extendedTypes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Failed to update database schema : {0}", ex.Message));
                return false;
            }

            return true;
        }

        static List<string> schemaUpdated = new List<string>();
        public static void UpdateSchema<T>(String settings, params Type[] extendedTypes) where T : XPObject
        {
            var name = XpoHelpers.XpoHelper.GetDataSourceLockName(settings);
            if (name == null)
                name = Assembly.GetExecutingAssembly().GetName().Name;

            using (var Mutex = new Mutex(false, name))
            {
                var signal = Mutex.WaitOne(Properties.Settings.Default.TimeoutUpdateSchema);

                if (!signal)
                    throw new UpdateSchemaTimeoutException("Timeout occured waiting for previous update schema operation to complete");

                try
                {
                    lock (schemaUpdated)
                    {
                        if (schemaUpdated.Contains(settings))
                            return;

                        using (var dl = CreateDataLayer<T>(settings))
                        {
                            using (var uow = new UnitOfWork(dl))
                            {
                                uow.UpdateSchema(typeof(T));
                                uow.CreateObjectTypeRecords(typeof(T));

                                if (extendedTypes != null && extendedTypes.Length > 0)
                                {
                                    uow.UpdateSchema(extendedTypes);
                                    uow.CreateObjectTypeRecords(extendedTypes);
                                }
                            }
                        }

                        schemaUpdated.Add(settings);
                    }
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }

        public static bool TestConnection<T>(UnitOfWork ufw) where T : XPObject
        {
            try
            {
                var oid = (from entry in new XPQuery<T>(ufw)
                           select entry.Oid).Take(1).SingleOrDefault();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static IDataLayer CreateDataLayer<T>(String settings, int commandTimeout = 0) where T : XPObject
        {
#if !NET_STANDARD
            if (XpoHelpers.XpoHelper.IsMSSQlCEDataProvider(settings) && !MSSqlCEHelper.Utilities.IsV40Installed())
            {
                throw new InvalidOperationException("SQL Server Compact 4.0 is not properly installed.");
            }
#endif
            XpoHelpers.XpoHelper.CreateDirectoryIfNotExists(settings);
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var tableName = GetTableName<T>(settings);
            IDataStore store = null;
            lock (objMutex)
            {
                store = XpoDefault.GetConnectionProvider(RemoveTableName(settings), DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            }
            dict.GetDataStoreSchema(typeof(T).Assembly);
            if (!String.IsNullOrEmpty(tableName))
                dict.ChangeTableName<T>(tableName);
            return new CustomThreadSafeDataLayer(commandTimeout, dict, store);
        }

        public static IDataLayer CreateSimpleDataLayer<T>(String settings, DevExpress.Xpo.DB.AutoCreateOption autoCreateOption = DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema) where T : XPObject
        {
#if !NET_STANDARD
            if (XpoHelpers.XpoHelper.IsMSSQlCEDataProvider(settings) && !MSSqlCEHelper.Utilities.IsV40Installed())
            {
                throw new InvalidOperationException("SQL Server Compact 4.0 is not properly installed.");
            }
#endif
            XpoHelpers.XpoHelper.CreateDirectoryIfNotExists(settings);
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var tableName = GetTableName<T>(settings);
            IDataStore store = null;
            lock (objMutex)
            {
                store = XpoDefault.GetConnectionProvider(RemoveTableName(settings), autoCreateOption);
            }
            dict.GetDataStoreSchema(typeof(T).Assembly);
            if (!String.IsNullOrEmpty(tableName))
                dict.ChangeTableName<T>(tableName);
            return new SimpleDataLayer(dict, store);
        }

        static readonly String CatalogSourceKeyword = "initial catalog";
        public static void ApplyCFR21Requirements(String settings, UnitOfWork ufw)
        {
            if (!XpoHelpers.XpoHelper.IsMSSQlDataProvider(settings))
                return;

            var helper = new ConnectionStringParser(settings);
            if (helper.PartExists(CatalogSourceKeyword))
            {
                var dbName = helper.GetPartByName(CatalogSourceKeyword);
                ufw.ExecuteNonQuery(String.Format("ALTER DATABASE [{0}] SET RECOVERY FULL", dbName));
                bool isFullBackupExists = (int)ufw.ExecuteScalar(String.Format(Properties.Settings.Default.MSSQLFullBackupExistsQuery, dbName)) > 0;
                if (!isFullBackupExists)
                {
                    var data = ufw.ExecuteQuery(Properties.Settings.Default.MSSQLBackupDirectoryQuery);
                    if (data.ResultSet != null && 
                        data.ResultSet.Length > 0 && data.ResultSet[0].Rows.Length > 0 && 
                        data.ResultSet[0].Rows[0].Values.Length > 1)
                    {
                        var backupDirectory = (string)data.ResultSet[0].Rows[0].Values[1];
                        if (backupDirectory != null)
                            ufw.ExecuteNonQuery(String.Format("BACKUP DATABASE [{0}] To DISK = N'{1}\\{0}.bak' WITH INIT, NAME = N'{0}-Full Database Backup'", dbName, backupDirectory));
                    }
                }
            }
        }

        #region TableName Helpers
        private static readonly String TableNameHeader = "table name";
        private static readonly String TableNameSeparator = "@";

        public static String GetTableName<T>(String conn) where T : XPObject
        {
            try
            {
                if (XpoHelpers.XpoHelper.IsDataSource(conn))
                {
                    var helper = new ConnectionStringParser(conn);
                    var tableName = helper.GetPartByName(TableNameHeader);
                    if (!String.IsNullOrEmpty(tableName))
                    {
                        var index = tableName.IndexOf(TableNameSeparator);
                        if (index > 0 && tableName.Substring(0, index) == typeof(T).Name)
                            return tableName.Substring(index + 1);
                    }
                }
            }
            catch (Exception ex)
            { }

            return null;
        }

        public static String SetTableName<T>(String conn, String name) where T : XPObject
        {
            var ret = conn;
            try
            {
                if (XpoHelpers.XpoHelper.IsDataSource(conn))
                {
                    var helper = new ConnectionStringParser(conn);
                    if (helper.PartExists(TableNameHeader))
                        helper.RemovePartByName(TableNameHeader);
                    helper.AddPart(TableNameHeader, String.Format("{0}{1}{2}", typeof(T).Name, TableNameSeparator, name));
                    ret = helper.GetConnectionString();
                }
            }
            catch (Exception ex)
            { }

            return ret;
        }

        public static String RemoveTableName(String conn)
        {
            var ret = conn;
            try
            {
                if (XpoHelpers.XpoHelper.IsDataSource(conn))
                {
                    var helper = new ConnectionStringParser(conn);
                    helper.RemovePartByName(TableNameHeader);
                    ret = helper.GetConnectionString();
                }
            }
            catch (Exception ex)
            { }

            return ret;
        }
        #endregion
    }
}
