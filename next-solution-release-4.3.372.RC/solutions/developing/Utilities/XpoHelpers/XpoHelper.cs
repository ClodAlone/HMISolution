using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Text.RegularExpressions;

namespace XpoHelpers
{
    #region CloneIXPSimpleObjectHelper Class

    public class CloneIXPSimpleObjectHelper
    {
        /// <summary>
        /// A dictionary containing objects from the source session as key and objects from the 
        /// target session as values
        /// </summary>
        /// <returns></returns>
        Dictionary<object, object> clonedObjects;
        Session sourceSession;
        Session targetSession;
        bool checkAttributes;
        bool copyAggregated;
        bool copyAssociation;
        bool copyOidKey;

        static SortingCollection sortCollectionByOid = new SortingCollection() { new SortProperty("Oid", SortingDirection.Ascending) };
        
        /// <summary>
        /// Initializes a new instance of the CloneIXPSimpleObjectHelper class.
        /// </summary>
        public CloneIXPSimpleObjectHelper(Session source, Session target, bool checkattributes = false, bool copyaggregated = true, bool copyassociation = false, bool copyOidKey = false)
        {
            this.clonedObjects = new Dictionary<object, object>();
            this.sourceSession = source;
            this.targetSession = target;
            this.checkAttributes = checkattributes;
            this.copyAggregated = copyaggregated;
            this.copyAssociation = copyassociation;
            this.copyOidKey = copyOidKey;
        }

        public T Clone<T>(T source) where T : IXPSimpleObject
        {
            return Clone<T>(source, targetSession, false, null, null);
        }
        public T Clone<T>(T source, bool synchronize, IXPSimpleObject target = null) where T : IXPSimpleObject
        {
            return (T)Clone(source as IXPSimpleObject, targetSession, synchronize, target);
        }

        public object Clone(IXPSimpleObject source)
        {
            return Clone(source, targetSession, false, null, null);
        }
        public object Clone(IXPSimpleObject source, bool synchronize, IXPSimpleObject target = null)
        {
            return Clone(source, targetSession, synchronize, target);
        }

        public T Clone<T>(T source, Session targetSession, bool synchronize, IXPSimpleObject target = null, IXPSimpleObject parent = null) where T : IXPSimpleObject
        {
            return (T)Clone(source as IXPSimpleObject, targetSession, synchronize, target, parent);
        }

        /// <summary>
        /// Clones and / or synchronizes the given IXPSimpleObject.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetSession"></param>
        /// <param name="synchronize">If set to true, reference properties are only cloned in case
        /// the reference object does not exist in the targetsession. Otherwise the exising object will be
        /// reused and synchronized with the source. Set this property to false when knowing at forehand 
        /// that the targetSession will not contain any of the objects of the source.</param>
        /// <returns></returns>
        public object Clone(IXPSimpleObject source, Session targetSession, bool synchronize, IXPSimpleObject target = null, IXPSimpleObject parent = null)
        {
            if (source == null)
                return null;
            if (clonedObjects.ContainsKey(source))
                return clonedObjects[source];
            XPClassInfo targetClassInfo = targetSession.GetClassInfo(source.GetType());
            object clone = target;
            if (clone == null && synchronize)
            {
                var key = source.Session.GetKeyValue(source);
                clone = targetSession.GetObjectByKey(targetClassInfo, key);
            }
            if (clone == null)
                clone = targetClassInfo.CreateNewObject(targetSession);
            clonedObjects.Add(source, clone);

            foreach (XPMemberInfo m in targetClassInfo.PersistentProperties)
            {
                if (m is DevExpress.Xpo.Metadata.Helpers.ServiceField || (!copyOidKey && m.IsKey))
                    continue;
                object val;
                if (m.ReferenceType != null)
                    continue;

                //{
                //    object createdByClone = m.GetValue(clone);
                //    if ((createdByClone != null) && synchronize == false)
                //        val = createdByClone;
                //    else
                //    {
                //         val = Clone((IXPSimpleObject)m.GetValue(source), targetSession, synchronize);
                //    }

                //}
                //else

                bool prevCheckAttributes = checkAttributes;
                try
                {
                    if (m.HasAttribute("DisableCheckCustomAttributes"))
                        checkAttributes = false;

                    if (checkAttributes && m.HasAttribute("Generate"))
                    {
                        var aInfo = (CustomAttribute)m.GetAttributeInfo("Generate");
                        switch (aInfo.Value)
                        {
                            case "Guid": val = Guid.NewGuid(); break;
                            case "DateTime": val = DateTime.UtcNow; break;
                            case "Null": val = null; break;
                            default: throw new InvalidEnumArgumentException(String.Format("Invalid value for the custom attribute {0} : value = {1}", aInfo.Name, aInfo.Value));
                        }
                    }
                    else
                    {
                        val = m.GetValue(source);
                    }
                    m.SetValue(clone, val);
                }
                finally
                {
                    checkAttributes = prevCheckAttributes;
                }
            }
            foreach (XPMemberInfo m in targetClassInfo.CollectionProperties)
            {
                bool prevCheckAttributes = checkAttributes;
                try
                {
                    if (m.HasAttribute("DisableCheckCustomAttributes"))
                        checkAttributes = false;

                    if (copyAggregated && m.HasAttribute(typeof(AggregatedAttribute)))
                    {
                        XPBaseCollection col = (XPBaseCollection)m.GetValue(clone);
                        XPBaseCollection colSource = (XPBaseCollection)m.GetValue(source);

                        var sorting = colSource.Sorting;
                        colSource.Sorting = sortCollectionByOid;

                        try
                        {
                            foreach (IXPSimpleObject obj in new ArrayList(colSource))
                                col.BaseAdd(Clone(obj, targetSession, synchronize));
                        }
                        finally
                        {
                            colSource.Sorting = sorting;
                        }
                    }
                    else if (copyAssociation && parent == null && m.HasAttribute(typeof(AssociationAttribute)))
                    {
                        XPBaseCollection col = (XPBaseCollection)m.GetValue(clone);
                        XPBaseCollection colSource = (XPBaseCollection)m.GetValue(source);
                        
                        var sorting = colSource.Sorting;
                        colSource.Sorting = sortCollectionByOid;

                        try
                        {
                            foreach (IXPSimpleObject obj in new ArrayList(colSource))
                                col.BaseAdd(Clone(obj, targetSession, true, null, source));
                        }
                        finally
                        {
                            colSource.Sorting = sorting;
                        }
                    }
                }
                finally
                {
                    checkAttributes = prevCheckAttributes;
                }
            }
            return clone;
        }
    }

    #endregion
    
    #region XpoHelper static Class

    static public class XpoHelper
    {
        #region Constants

        const uint MAX_PATH = 260;

        private static readonly String DataSourceHeader = "data source";
        private static readonly String DataSourceShortHeader = "datasource";
        private static readonly String CatalogSourceHeader = "initial catalog";
        private static readonly String ServerHeader = "server";
        private static readonly String DataBaseHeader = "database";
        private static readonly String TableNameHeader = "table name";
        private static readonly String UserHeader = "user id";
        private static readonly String PasswordHeader = "password";
        private static readonly String TrustedHeader = "integrated security";

        #endregion

        public static bool IsDataSource(String conn)
        {
            bool isDataSource = false;
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType))
                    isDataSource = true;
            }
            catch (Exception ex)
            {
            }

            return isDataSource;
        }

        public static bool IsSQlDataProvider(String conn)
        {
            bool isSQlDataProvider = false;
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType) &&
                    providerType != DataSetDataStore.XpoProviderTypeString &&
                    providerType != InMemoryDataStore.XpoProviderTypeString)
                    isSQlDataProvider = true;
            }
            catch (Exception ex)
            {
            }

            return isSQlDataProvider;
        }

        public static bool IsMSSQlDataProvider(String conn)
        {
            bool isSQlDataProvider = false;
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType) && providerType == MSSqlConnectionProvider.XpoProviderTypeString)
                    isSQlDataProvider = true;
            }
            catch (Exception ex)
            {
            }

            return isSQlDataProvider;
        }

#if !NET_STANDARD
        public static bool IsMSSQlCEDataProvider(String conn)
        {
            bool isSQlDataProvider = false;
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType) && providerType == MSSqlCEConnectionProvider.XpoProviderTypeString)
                    isSQlDataProvider = true;
            }
            catch (Exception ex)
            {
            }

            return isSQlDataProvider;
        }
#endif

        public static bool IsMySQlDataProvider(String conn)
        {
            bool isSQlDataProvider = false;
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType) && providerType == MySqlConnectionProvider.XpoProviderTypeString)
                    isSQlDataProvider = true;
            }
            catch (Exception ex)
            {
            }

            return isSQlDataProvider;
        }

        public static bool IsOracleSQlDataProvider(String conn)
        {
            bool isSQlDataProvider = false;
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType) &&
#if !NET_STANDARD
                    providerType == OracleConnectionProvider.XpoProviderTypeString
#else
                    providerType == ODPManagedConnectionProvider.XpoProviderTypeString
#endif
                    )
                    isSQlDataProvider = true;
            }
            catch (Exception ex)
            {
            }

            return isSQlDataProvider;
        }

        public static bool IsPostgreSQlDataProvider(String conn)
        {
            bool isSQlDataProvider = false;
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType) && providerType == PostgreSqlConnectionProvider.XpoProviderTypeString)
                    isSQlDataProvider = true;
            }
            catch (Exception ex)
            {
            }

            return isSQlDataProvider;
        }

        public static bool IsSessionObject(IXPSimpleObject source, Session session)
        {
            try
            {
                return source.Session == session;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static String GetDataSourceTitle(String conn, bool onlytitle = false)
        {
            try
            {
                if (conn == null) return conn;
                var connNoPassword = GetConnectionStringWithoutPassword(conn);
                var helper = new ConnectionStringParser(connNoPassword);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (providerType == InMemoryDataStore.XpoProviderTypeString)
                {
                    return Path.GetFileNameWithoutExtension(helper.GetPartByName(DataSourceHeader));
                }
#if !NET_STANDARD
                else if (providerType == AccessConnectionProvider.XpoProviderTypeString)
                {
                    return Path.GetFileNameWithoutExtension(helper.GetPartByName(DataSourceHeader));
                }
#endif
                else if (providerType == SQLiteConnectionProvider.XpoProviderTypeString)
                {
                    return Path.GetFileNameWithoutExtension(helper.GetPartByName(DataSourceHeader));
                }
                else if (providerType == MSSqlConnectionProvider.XpoProviderTypeString)
                {
                    if (onlytitle)
                    {
                        if (helper.PartExists(CatalogSourceHeader))
                            return helper.GetPartByName(CatalogSourceHeader);
                        else
                            return helper.GetPartByName(DataSourceHeader);
                    }
                    else
                        return String.Format("{0} - {1}", helper.GetPartByName(DataSourceHeader),
                            helper.GetPartByName(CatalogSourceHeader));
                }
                else if (providerType == MySqlConnectionProvider.XpoProviderTypeString)
                {
                    if (onlytitle)
                    {
                        if (helper.PartExists(DataBaseHeader))
                            return helper.GetPartByName(DataBaseHeader);
                        else
                            return helper.GetPartByName(ServerHeader);
                    }
                    else
                        return String.Format("{0} - {1}", helper.GetPartByName(ServerHeader),
                            helper.GetPartByName(DataBaseHeader));
                }

                return helper.GetConnectionString();
            }
            catch (Exception ex)
            {
            }

            return conn;
        }

        public static String GetDataSourceServer(String conn, bool checkprovidertype = true)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!checkprovidertype || providerType == MSSqlConnectionProvider.XpoProviderTypeString ||
#if !NET_STANDARD
                    providerType == MSSqlCEConnectionProvider.XpoProviderTypeString ||
#endif
                    providerType == MySqlConnectionProvider.XpoProviderTypeString  ||
#if !NET_STANDARD
                    providerType == OracleConnectionProvider.XpoProviderTypeString ||
#else
                    providerType == ODPManagedConnectionProvider.XpoProviderTypeString ||
#endif
                    providerType == PostgreSqlConnectionProvider.XpoProviderTypeString)
                {
                    return helper.GetPartByName(DataSourceHeader);
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        static string mutexNamePattern = string.Format("[{0}{1};]", Regex.Escape(new string(Path.GetInvalidFileNameChars())), Regex.Escape("\\"));
        public static String GetDataSourceLockName(String conn)
        {
            try
            {
                conn = conn.ToLower();
                var builder = new StringBuilder((int)MAX_PATH);
                var helper = new ConnectionStringParser(conn);
                if (helper.PartExists(DataStoreBase.XpoProviderTypeParameterName))
                    builder.Append(helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName));
                if (helper.PartExists(DataSourceHeader))
                    builder.Append(helper.GetPartByName(DataSourceHeader));
                if (helper.PartExists(CatalogSourceHeader))
                    builder.Append(helper.GetPartByName(CatalogSourceHeader));
                if (helper.PartExists(TableNameHeader))
                    builder.Append(helper.GetPartByName(TableNameHeader));

                return Regex.Replace(builder.ToString(), mutexNamePattern, ".");
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public static String SetDataSourceServer(String conn, String server, bool checkprovidertype = true)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!checkprovidertype || providerType == MSSqlConnectionProvider.XpoProviderTypeString ||
#if !NET_STANDARD
                    providerType == MSSqlCEConnectionProvider.XpoProviderTypeString ||
#endif
                    providerType == MySqlConnectionProvider.XpoProviderTypeString  ||
#if !NET_STANDARD
                    providerType == OracleConnectionProvider.XpoProviderTypeString ||
#else
                    providerType == ODPManagedConnectionProvider.XpoProviderTypeString ||
#endif
                    providerType == PostgreSqlConnectionProvider.XpoProviderTypeString)
                {
                    helper.UpdatePartByName(DataSourceHeader, server);
                    return helper.GetConnectionString();
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public static String GetDataSourceFilePath(String conn, string providerTypeString = InMemoryDataStore.XpoProviderTypeString)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (providerTypeString == null || providerType == providerTypeString)
                {
                    var datasource = helper.GetPartByName(DataSourceHeader);
                    if (String.IsNullOrEmpty(datasource))
                        datasource = helper.GetPartByName(DataSourceShortHeader);
#if !NET_STANDARD
                    return Path.GetFullPath(datasource);
#else
                    return Path.GetFullPath(datasource.Replace('\\', Path.DirectorySeparatorChar));
#endif
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        /// <summary>
        /// Normalize the connection string in order to allow remote connection from remote server.
        /// </summary>
        /// <param name="connection">
        /// The connection string to normalize.
        /// </param>
        /// <param name="checkprovidertype">
        /// Allow to decide to control for the presence of the 'XpoProviderTypeParameterName' constant for ensure a database connection.
        /// </param>
        /// <returns>
        /// Return a new connection string with DataSource parameter normalized (return the given connection if no any normalization occured).
        /// </returns>
        public static String NormalizeConnectionString(String connection, bool checkprovidertype = true)
        {
            connection = connection.Replace("(local)", "localhost");
            connection = connection.Replace(".\\", "localhost\\");
            var hostname = XpoHelpers.XpoHelper.GetDataSourceServer(connection, checkprovidertype);
            if (hostname != null)
            {
                String serverinstance = null;
                var index = hostname.IndexOf('\\');
                if (index != -1)
                {
                    serverinstance = hostname.Substring(index + 1);
                    hostname = hostname.Substring(0, index);
                }
                var newhostname = Utilities.LanExtensions.NormalizeHostname(hostname);
                if (newhostname != hostname)
                {
                    var newserver = new StringBuilder(newhostname);
                    if (!String.IsNullOrEmpty(serverinstance))
                        newserver.AppendFormat("\\{0}", serverinstance);
                    var ret = XpoHelpers.XpoHelper.SetDataSourceServer(connection, newserver.ToString(), checkprovidertype);
                    if (ret != null)
                        return ret;
                }
            }

            return connection;
        }

        public static String NormalizeConnectionString(String connection, String projectRoot)
        {
            if (!String.IsNullOrEmpty(connection))
            {
                if (String.IsNullOrEmpty(projectRoot) || !System.IO.Path.IsPathRooted(projectRoot))
                    projectRoot = GetSpecialFolder(Environment.SpecialFolder.LocalApplicationData);
                var rootPath = System.IO.Path.GetDirectoryName(projectRoot);
                connection = connection.Replace(Utilities.Properties.Settings.Default.ProjectRootPlaceholder, rootPath);
            }

#if NET_STANDARD
            if (connection != null)
            {
                var helper = new ConnectionStringParser(connection);
                string sourcePath = null;
                if (helper.PartExists(DataSourceHeader))
                    sourcePath = helper.GetPartByName(DataSourceHeader).Replace('\\', Path.DirectorySeparatorChar);
                else if (helper.PartExists(DataSourceShortHeader))
                    sourcePath = helper.GetPartByName(DataSourceShortHeader).Replace('\\', Path.DirectorySeparatorChar);
                if (sourcePath != null && Utilities.DirectoryHelper.IsValidFile(sourcePath, checkIfExist: false))
                {
                    if (helper.PartExists(DataSourceHeader))
                        helper.UpdatePartByName(DataSourceHeader, sourcePath);
                    else if (helper.PartExists(DataSourceShortHeader))
                        helper.UpdatePartByName(DataSourceShortHeader, sourcePath);
                    connection = helper.GetConnectionString();
                }
            }
#endif

            return connection;
        }

        public static String AddPlaceholderToConnectionString(String connection, String projectRoot)
        {
            if (!String.IsNullOrEmpty(connection))
            {
                if (String.IsNullOrEmpty(projectRoot) || !System.IO.Path.IsPathRooted(projectRoot))
                    projectRoot = GetSpecialFolder(Environment.SpecialFolder.LocalApplicationData);
                var rootPath = System.IO.Path.GetDirectoryName(projectRoot);
                connection = connection.Replace(rootPath, Utilities.Properties.Settings.Default.ProjectRootPlaceholder);
            }

            return connection;
        }

        public static String GetSpecialFolder(Environment.SpecialFolder specialFolder)
        {
            return String.Format("{0}{3}{1}{3}{2}{3}",
                Environment.GetFolderPath(specialFolder),
                Utilities.AssemblyInfo.Company,
                Utilities.AssemblyInfo.Product,
                System.IO.Path.DirectorySeparatorChar);
        }

        public static void CreateDirectoryIfNotExists(String connection)
        {
            String providerTypeString = null;
            try
            {
                var helper = new ConnectionStringParser(connection);
                providerTypeString = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            }
            catch (Exception ex)
            { }

            if (providerTypeString == null || 
                providerTypeString == MSSqlCEConnectionProvider.XpoProviderTypeString || 
                providerTypeString == SQLiteConnectionProvider.XpoProviderTypeString)
            {
                var filePath = XpoHelpers.XpoHelper.GetDataSourceFilePath(connection, null);
                if (!String.IsNullOrEmpty(filePath) && System.IO.Path.IsPathRooted(filePath))
                {
                    try
                    {
                        var file = new System.IO.FileInfo(filePath);
                        if (!file.Exists)
                        {
                            System.IO.Directory.CreateDirectory(file.DirectoryName);
                        }
                    }
                    catch
                    { }
                }
            }
        }

        public static String EnsureTrustedConnectionStrings(String connection)
        {
            try
            {
                var helper = new ConnectionStringParser(connection);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (providerType == MSSqlConnectionProvider.XpoProviderTypeString && !helper.PartExists(TrustedHeader))
                {
                    helper.RemovePartByName(UserHeader);
                    helper.RemovePartByName(PasswordHeader);
                    helper.AddPart(TrustedHeader, "SSPI");
                    return helper.GetConnectionString();
                }
            }
            catch (Exception ex)
            { }

            return connection;
        }

        public static String GetConnectionStringWithoutPassword(String conn)
        {
            try
            {
                if (conn == null) return null;
                var helper = new ConnectionStringParser(conn);
                helper.UpdatePartByName(PasswordHeader, "***");
                return helper.GetConnectionString();
            }
            catch (Exception ex)
            {
            }

            return conn;
        }

        public static String GetConnectionString(String activeconnection, String folder, String baseName, String xmlExt)
        {
            if (String.IsNullOrEmpty(activeconnection) || /*String.IsNullOrEmpty(folder) ||*/ String.IsNullOrEmpty(baseName) || String.IsNullOrEmpty(xmlExt))
                throw new ArgumentNullException("Parameters cannot be null or empty");

            string regex = string.Format("[{0};]", Regex.Escape(new string(Path.GetInvalidFileNameChars())));
            baseName = Regex.Replace(baseName, regex, ".");

            ConnectionStringParser helper = new ConnectionStringParser(activeconnection);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            if (providerType == InMemoryDataStore.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                ds = ds.Replace('/', '\\');
                int index = ds.LastIndexOf('\\');
                if (index != -1)
                {
                    //var path = String.Format("{0}\\{1}", ds.Substring(0, index), folder);
                    var path = ds.Substring(0, index);
                    if(!String.IsNullOrEmpty(folder))
                        path = String.Format("{0}\\{1}", path, folder);

#if !NET_STANDARD
                    Directory.CreateDirectory(path);
#else
                    Directory.CreateDirectory(path.Replace('\\', Path.DirectorySeparatorChar));
#endif
                    path = String.Format("{0}\\{1}{2}", path, baseName, xmlExt);
                    if (path.Length >= MAX_PATH)
                        throw new System.IO.PathTooLongException();
#if !NET_STANDARD
                    ds = path;
#else
                    ds = path.Replace('\\', Path.DirectorySeparatorChar);
#endif
                }

                helper.UpdatePartByName(DataSourceHeader, ds);

                return helper.GetConnectionString();
            }
            else
                return helper.GetConnectionString();
        }

        public static object Clone(IXPSimpleObject source, Session session)
        {
            if (source == null)
                return null;
            object clone = source.ClassInfo.CreateObject(session ?? source.Session);
            foreach (XPMemberInfo m in source.ClassInfo.PersistentProperties)
            {
                if (m is DevExpress.Xpo.Metadata.Helpers.ServiceField || m.IsKey)
                    continue;
                object val;
                if (m.ReferenceType != null && m.HasAttribute(typeof(AggregatedAttribute)))
                {
                    val = Clone((IXPSimpleObject)m.GetValue(source), session);
                }
                else
                {
                    val = m.GetValue(source);
                }
                m.SetValue(clone, val);
            }
            foreach (XPMemberInfo m in source.ClassInfo.CollectionProperties)
            {
                if (m.HasAttribute(typeof(AggregatedAttribute)))
                {
                    XPBaseCollection col = (XPBaseCollection)m.GetValue(clone);
                    foreach (IXPSimpleObject obj in new ArrayList((XPBaseCollection)m.GetValue(source)))
                        col.BaseAdd(Clone(obj, session));
                }
            }
            return clone;
        }

        public static object CloneObject(object obj)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
                binaryFormatter.Serialize(memStream, obj);
                memStream.Seek(0, SeekOrigin.Begin);
                return binaryFormatter.Deserialize(memStream);
            }
        }

        public static void AddProtectionCode(Session session, Guid guid)
        {
            var protection = (from tag in new XPQuery<ProtectionFile>(session, true).AsParallel() select tag).FirstOrDefault();
            if (protection == null)
                protection = new ProtectionFile(session);
            protection.Id = guid;
        }

        public static void RemoveProtectionCode(Session session)
        {
            var protections = (from tag in new XPQuery<ProtectionFile>(session, true).AsParallel() select tag).ToList();
            session.Delete(protections);
        }

        public static Guid GetProtectionCode(Session session)
        {
            var protection = (from tag in new XPQuery<ProtectionFile>(session, true).AsParallel() select tag).FirstOrDefault();
            if (protection != null)
                return protection.Id;

            return Guid.Empty;
        }

        /// <summary>
        /// Append new parameter to the current connection string
        /// </summary>
        /// <param name="serverConnection">Current connection string</param>
        /// <param name="parameterName">Name of the parameter to append</param>
        /// <param name="parameterValue">Value of the parameter to append</param>
        /// <returns></returns>
        public static string AddParameterToConnectionString(string serverConnection, string parameterName, string parameterValue)
        {
            return $"{serverConnection};{parameterName}={parameterValue}";
        }
        
        public static string GetUFUAServerUniqueStartupName(string assemblyName, string connectionString)
        {
            var dataSource = string.Empty;
            var uniqueDataSource = string.Empty;

            try
            {
                uniqueDataSource = GetDataSourceLockName(connectionString);
            }
            finally
            {
                if (string.IsNullOrEmpty(uniqueDataSource) == false)
                {
                    dataSource = uniqueDataSource;
                }
            }

            string uniqueStartupName = assemblyName + dataSource;
            
            if (uniqueStartupName.Length > (int)MAX_PATH)
            {
                uniqueStartupName = uniqueStartupName.Substring(0, (int)MAX_PATH - 1);
            }

            return uniqueStartupName;
        }

        public static string GetServiceUniqueStartupName(
            string assemblyName,
            string separator,
            string projectDocumentConfigId)
        {
            var uniqueStartupName = assemblyName + separator + projectDocumentConfigId;
            
            if (uniqueStartupName.Length > (int)MAX_PATH)
            {
                uniqueStartupName = uniqueStartupName.Substring(0, (int)MAX_PATH - 1);
            }        
                
            return uniqueStartupName;
        }
    }

#endregion
    
#region UndoRedoIXPSimpleObjectHelper Class
    
    public class UndoRedoIXPSimpleObjectHelper
    {
#region Declarations

        readonly Session sourceSession;
        readonly Session targetSession;

        readonly Object lockObject = new Object();
        readonly short MaxActionsToStore;
        readonly Stack<List<IXPSimpleObject>> UndoStack;
        readonly Stack<UndoRedoActions> UndoAction;
        readonly Dictionary<IXPSimpleObject, IXPSimpleObject> UndoToSource;

        readonly Stack<List<IXPSimpleObject>> RedoStack;
        readonly Stack<UndoRedoActions> RedoAction;
        readonly Dictionary<IXPSimpleObject, IXPSimpleObject> RedoToSource;

#endregion

#region Constructors

        public UndoRedoIXPSimpleObjectHelper(Session source, Session target) : 
            this (source, target, numactions: 0)
        { }

        public UndoRedoIXPSimpleObjectHelper(Session source, Session target, short numactions)
        {
            sourceSession = source;
            targetSession = target;
            MaxActionsToStore = numactions;
            UndoStack = new Stack<List<IXPSimpleObject>>(numactions);
            UndoAction = new Stack<UndoRedoActions>(numactions);
            UndoToSource = new Dictionary<IXPSimpleObject, IXPSimpleObject>();
            RedoStack = new Stack<List<IXPSimpleObject>>(numactions);
            RedoAction = new Stack<UndoRedoActions>(numactions);
            RedoToSource = new Dictionary<IXPSimpleObject, IXPSimpleObject>();
        }

#endregion

#region Members

        public void AddUndoAction(IList<IXPSimpleObject> sources, UndoRedoActions action)
        {
            AddUndoAction(sources, action, false);
        }

        public void AddRedoAction(IList<IXPSimpleObject> sources, UndoRedoActions action)
        {
            AddRedoAction(sources, action, false);
        }

        public void AddUndoAction(IXPSimpleObject source, UndoRedoActions action)
        {
            var list = new List<IXPSimpleObject>();
            list.Add(source);
            AddUndoAction(list, action);
        }

        public void AddRedoAction(IXPSimpleObject source, UndoRedoActions action)
        {
            var list = new List<IXPSimpleObject>();
            list.Add(source);
            AddRedoAction(list, action);
        }

        public List<XPObject> Undo(out UndoRedoActions action)
        {
            List<XPObject> objectsToDelete;
            var ret = Undo(out action, out objectsToDelete);
            objectsToDelete.ForEach((obj) => obj.Delete());
            PurgeUndoActions();
            return ret;
        }

        public List<XPObject> Undo(out UndoRedoActions action, out List<XPObject> stackObjects)
        {
            stackObjects = new List<XPObject>();
            var targets = new Dictionary<XPObject, IXPSimpleObject>();
            lock (lockObject)
            {
                action = UndoRedoActions.None;

                if (UndoStack.Count != UndoAction.Count)
                    throw new InvalidOperationException("Invalid memory stack for performing Undo command");

                var list = new List<IXPSimpleObject>();
                if (UndoStack.Count > 0)
                {
                    action = UndoAction.Pop();
                    list = UndoStack.Pop();
                }
            
                if (list.Count > 0)
                {
                    foreach (var entry in list/*.AsParallel()*/)
                    {
                        var obj = entry as XPObject;
                        if (UndoToSource.ContainsKey(obj))
                            targets[obj] = UndoToSource[obj];
                    }
                }
            }

            var ret = new List<XPObject>();
            if (targets.Keys.Count > 0)
            {
                AddRedoAction(targets.Values.ToList(), action, true);
                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(targetSession, sourceSession, copyaggregated: action != UndoRedoActions.Changed && action != UndoRedoActions.Replaced);
                foreach (var obj in targets.Keys)
                {
                    var target = targets[obj] as XPObject;
                    if (action == UndoRedoActions.Changed || 
                        action == UndoRedoActions.Replaced ||
                        action == UndoRedoActions.Added && !target.IsDeleted ||
                        action == UndoRedoActions.Removed && target.IsDeleted)
                        ret.Add(cloneHelper.Clone(obj, true, target));
                    UndoToSource.Remove(obj);
                    stackObjects.Add(obj);
                }
            }

            return ret;
        }

        public List<XPObject> Redo(out UndoRedoActions action)
        {
            List<XPObject> objectsToDelete;
            var ret = Redo(out action, out objectsToDelete);
            objectsToDelete.ForEach((obj) => obj.Delete());
            PurgeRedoActions();
            return ret;
        }

        public List<XPObject> Redo(out UndoRedoActions action, out List<XPObject> stackObjects)
        {
            stackObjects = new List<XPObject>();
            var targets = new Dictionary<XPObject, IXPSimpleObject>();
            lock (lockObject)
            {
                var list = new List<IXPSimpleObject>();
                action = UndoRedoActions.None;

                if (RedoStack.Count != RedoAction.Count)
                    throw new InvalidOperationException("Invalid memory stack for performing Redo command");

                if (RedoStack.Count > 0)
                {
                    action = RedoAction.Pop();
                    list = RedoStack.Pop();
                }

                if (list.Count > 0)
                {
                    foreach (var entry in list/*.AsParallel()*/)
                    {
                        var obj = entry as XPObject;
                        if (RedoToSource.ContainsKey(obj))
                            targets[obj] = RedoToSource[obj];
                    }
                }
            }

            var ret = new List<XPObject>();
            if (targets.Keys.Count > 0)
            {
                AddUndoAction(targets.Values.ToList(), action, true);
                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(targetSession, sourceSession, copyaggregated: action != UndoRedoActions.Changed && action != UndoRedoActions.Replaced);
                foreach (var obj in targets.Keys)
                {
                    var target = targets[obj] as XPObject;
                    if (action == UndoRedoActions.Changed || 
                        action == UndoRedoActions.Replaced ||
                        action == UndoRedoActions.Added && target.IsDeleted ||
                        action == UndoRedoActions.Removed && !target.IsDeleted)
                        ret.Add(cloneHelper.Clone(obj, true, target));
                    RedoToSource.Remove(obj);
                    stackObjects.Add(obj);
                }
            }

            return ret;
        }

        public bool CanUndo()
        {
            lock (lockObject)
            {
                return UndoStack.Count > 0;
            }
        }

        public bool CanRedo()
        {
            lock (lockObject)
            {
                return RedoStack.Count > 0;
            }
        }

        public void PurgeUndoActions(bool clean = false)
        {
            if (clean)
            {
                lock (lockObject)
                {
                    foreach (var list in UndoStack/*.AsParallel()*/)
                    {
                        list.ForEach(entry => 
                        {
                            if (entry is XPObject)
                                (entry as XPObject).Delete();
                        });
                    }

                    UndoStack.Clear();
                    UndoAction.Clear();
                }
            }

            targetSession.CommitTransaction();
            targetSession.PurgeDeletedObjects();
        }

        public void PurgeRedoActions(bool clean = false)
        {
            if (clean)
            {
                lock (lockObject)
                {
                    foreach (var list in RedoStack/*.AsParallel()*/)
                    {
                        list.ForEach(entry =>
                        {
                            if (entry is XPObject)
                                (entry as XPObject).Delete();
                        });
                    }

                    RedoStack.Clear();
                    RedoAction.Clear();
                }
            }

            targetSession.CommitTransaction();
            targetSession.PurgeDeletedObjects();
        }

#endregion

#region Interfaces
        public interface IUniqueIdentifier
        {
            String UniqueIdentifier { get; }
        }
#endregion

#region Enumerators

        public enum UndoRedoActions
        {
            None,
            Added,
            Changed,
            Replaced,
            Removed
        }

#endregion

#region Privates Members

        void AddUndoAction(IList<IXPSimpleObject> sources, UndoRedoActions action, bool checkDeleted)
        {
            lock (lockObject)
            {
                if (MaxActionsToStore > 0)
                    CheckMaxActionsToStore(UndoStack, UndoAction, MaxActionsToStore);

                var list = new List<IXPSimpleObject>();
                var listids = new List<String>();
                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceSession, targetSession, false, action != UndoRedoActions.Replaced, action != UndoRedoActions.Replaced);
                foreach (var source in sources/*.AsParallel()*/)
                {
                    var entry = source as XPObject;
                    if (!checkDeleted ||
                        action == UndoRedoActions.Changed || 
                        action == UndoRedoActions.Replaced ||
                        action == UndoRedoActions.Added && entry.IsDeleted ||
                        action == UndoRedoActions.Removed && !entry.IsDeleted)
                    {
                        var obj = cloneHelper.Clone(entry, false);
                        list.Add(obj);
                        UndoToSource[obj] = source;
                    }
                    if (!checkDeleted && action == UndoRedoActions.Added && entry is IUniqueIdentifier)
                    {
                        var id = (entry as IUniqueIdentifier).UniqueIdentifier;
                        if (!String.IsNullOrEmpty(id))
                            listids.Add(id);
                    }
                }

                if (list.Count > 0)
                {
                    UndoStack.Push(list);
                    UndoAction.Push(action);
                }

                if (listids.Count > 0)
                    RemoveActionsByIdentifiers(RedoStack, RedoAction, listids);
            }
        }

        void AddRedoAction(IList<IXPSimpleObject> sources, UndoRedoActions action, bool checkDeleted)
        {
            lock (lockObject)
            {
                if (MaxActionsToStore > 0)
                    CheckMaxActionsToStore(RedoStack, RedoAction, MaxActionsToStore);

                var list = new List<IXPSimpleObject>();
                var listids = new List<String>();
                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceSession, targetSession, false, action != UndoRedoActions.Replaced, action != UndoRedoActions.Replaced);
                foreach (var source in sources/*.AsParallel()*/)
                {
                    var entry = source as XPObject;
                    if (!checkDeleted ||
                        action == UndoRedoActions.Changed ||
                        action == UndoRedoActions.Replaced ||
                        action == UndoRedoActions.Added && !entry.IsDeleted ||
                        action == UndoRedoActions.Removed && entry.IsDeleted)
                    {
                        var obj = cloneHelper.Clone(entry, false);
                        list.Add(obj);
                        RedoToSource[obj] = source;
                    }
                    if (!checkDeleted && action == UndoRedoActions.Added && entry is IUniqueIdentifier)
                    {
                        var id = (entry as IUniqueIdentifier).UniqueIdentifier;
                        if (!String.IsNullOrEmpty(id))
                            listids.Add(id);
                    }
                }

                if (list.Count > 0)
                {
                    RedoStack.Push(list);
                    RedoAction.Push(action);
                }

                if (listids.Count > 0)
                    RemoveActionsByIdentifiers(UndoStack, UndoAction, listids);
            }
        }

        void CheckMaxActionsToStore(Stack<List<IXPSimpleObject>> listobjects, Stack<UndoRedoActions> listactions, short numactions)
        {
            if (listactions.Count >= MaxActionsToStore)
            {
                var commands = new Stack<List<IXPSimpleObject>>(numactions);
                // fill queue with the stack list
                while (listobjects.Count > 1)
                    commands.Push(listobjects.Pop());
                // remove the latest object
                foreach (var entry in listobjects.Pop())
                {
                    if (entry is XPObject)
                    {
                        var obj = entry as XPObject;
                        UndoToSource.Remove(obj);
                        obj.Delete();
                    }
                }
                // fill stack with the queue list - 1
                while (commands.Count > 0)
                    listobjects.Push(commands.Pop());

                var actions = new Stack<UndoRedoActions>(numactions);
                // fill queue with the stack list
                while (listactions.Count > 1)
                    actions.Push(listactions.Pop());
                // remove the latest object
                listactions.Pop();
                // fill stack with the queue list - 1
                while (actions.Count > 0)
                    listactions.Push(actions.Pop());

                commands.Clear();
                actions.Clear();
            }
        }

        void RemoveActionsByIdentifiers(Stack<List<IXPSimpleObject>> listobjects, Stack<UndoRedoActions> listactions, List<String> listids)
        {
            var commands = new Stack<List<IXPSimpleObject>>(listobjects.Count);
            var actions = new Stack<UndoRedoActions>(listactions.Count);
            while (listobjects.Count > 0)
            {
                var objs = listobjects.Pop();
                var acts = listactions.Pop();
                for (int ii = 0; ii < objs.Count; ii++)
                {
                    if (!(objs[ii] is IUniqueIdentifier))
                        continue;

                    var id = (objs[ii] as IUniqueIdentifier).UniqueIdentifier;
                    if (listids.Contains(id))
                    {
                        objs.RemoveAt(ii);
                        ii--;
                    }
                }

                if (objs.Count > 0)
                {
                    commands.Push(objs);
                    actions.Push(acts);
                }
            }

            while (commands.Count > 0)
                listobjects.Push(commands.Pop());
            while (actions.Count > 0)
                listactions.Push(actions.Pop());

            commands.Clear();
            actions.Clear();
        }

#endregion

    }

#endregion

#region Protection File Class
    [DeferredDeletion(false)]
    public class ProtectionFile :  XPObject
    {
#region Constructors
        internal ProtectionFile(Session session)
            : base(session)
        { }
#endregion

#region Properties
        private Guid id;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get
            {
                return id;
            }
            set
            {
                SetPropertyValue("Id", ref id, value);
            }
        }
#endregion
    }

#endregion
}
