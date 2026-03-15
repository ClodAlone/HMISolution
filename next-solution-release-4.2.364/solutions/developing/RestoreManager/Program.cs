using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExpress.Xpo;
using XpoHelpers;
using System.Threading;
using DataReader;
using UFUAHistorianModel;
using DataReader.SchemaInfo;
using UFUAHistorianModel.Helpers;
using Utilities;
using DevExpress.Xpo.DB;
using System.IO;
using System.Xml;

namespace RestoreManager
{
    class Program
    {
        #region Declarations
        static bool bExitMode;
        static bool isRunningAsCFR21UserName;
        #endregion

        static void Main(string[] args)
        {
#if !NET_CORE
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
#if DEBUG
            //if (!System.Diagnostics.Debugger.IsAttached &&
            //    Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
            //        "DebugMe - RestoreManager", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            //    System.Diagnostics.Debugger.Launch();
#endif
#else
#if DEBUG
            //Console.WriteLine("if you would like to attach a debugger now is the right moment !");
            //Console.WriteLine("Press enter to continue...");
            //Console.ReadLine();
#endif
#endif

            if (args.Length > 0)
            {
                CommandLineOptions cl = new CommandLineOptions(args);
                if (cl.IsValid)
                {
                    if (cl.CallingProcessId > 0)
                    {
                        try
                        {
                            var process = Process.GetProcessById(cl.CallingProcessId);
                            if (process != null)
                            {
                                process.EnableRaisingEvents = true;
                                process.Exited += (o, e) =>
                                {
                                    bExitMode = true;
                                };
                            }
                        }
                        catch { }
                    }

                    try
                    {
                        isRunningAsCFR21UserName = CurrentUser.IsEqualTo(UFUAServerInfo.UFUAServerInfo.GetCFR21UserName(), UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName());
                    }
                    catch
                    { }

                    switch (cl.Operation)
                    {
                        case Operations.OpSection:
                            {
                                RestoreData(cl.Option, cl.Source, cl.Destination, cl.DLROptions, cl.Redundancy);
                            }
                            break;
                    }
                }
                else
                {
                    Console.WriteLine(Properties.Resources.InvalidOptions);
                    return;
                }

            }
            else
                Console.WriteLine(Properties.Resources.InvalidOptionNumber);
        }

        #region Methods
        private static void RestoreData(int option, string source, string destination, string dlroptions, bool redundancy)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination))
            {
                Console.WriteLine(Properties.Resources.InvalidOptions);
                return;
            }

            var datasource = option == 2 ? System.IO.Path.GetFileNameWithoutExtension(source) : XpoHelper.GetDataSourceLockName(source);
            using (var Mutex = new Mutex(false, datasource))
            {
                try
                {
                    Mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    Console.WriteLine(ex.Message.Replace(Environment.NewLine, " "));
                }

                if (bExitMode)
                    return;

                try
                {
                    switch (option)
                    {
                        case 0:
                            RestoreHistorians(source, destination, redundancy);
                            break;
                        case 1:
                            RestoreEvents(source, destination, redundancy);
                            break;
                        case 2:
                            RestoreDatalogger(source, destination, dlroptions, redundancy);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message.Replace(Environment.NewLine, " "));
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }

        private static Dictionary<string, object> GetElements(string filepath, string descendantid, bool p2, string name, bool ditinct = false)
        {
            Dictionary<string, object> mapTag = new Dictionary<string, object>();
            var keyexpandolist = GetElementsFromXml(filepath, descendantid, name, p2, ditinct);
            if (keyexpandolist.Count() != 0)
            {
                keyexpandolist.ToList().ForEach(e =>
                {
                    var regkeydictionary = e as IDictionary<string, object>;
                    regkeydictionary.ToList().ForEach(r =>
                    {
                        try
                        {
                            mapTag.Add(r.Key, r.Value);
                        }
                        catch
                        {
                            Console.Error.WriteLine(Properties.Resources.InvalidXamlSchema);
                        }
                    });
                });
            }

            return mapTag;
        }

        public static IEnumerable<dynamic> GetElementsFromXml(string file, string descendantid, string descendantname, bool fromcode = false, bool distinct = false)
        {
            var expandoFromXml = new List<dynamic>();
            int index = 0;
            XDocument doc = new XDocument();
            if (fromcode)
            {
                doc = XDocument.Parse(file);
            }
            else
            {
                doc = XDocument.Load(file);
            }
            foreach (var element in doc.Descendants(descendantid)/*.AsParallel()*/)
            {
                dynamic expandoObject = new ExpandoObject();
                var dictionary = expandoObject as IDictionary<string, object>;
                foreach (var child in element.Descendants())
                {
                    if (child.Name.Namespace == "" && child.Name == descendantname && (!distinct || (distinct && child.Parent.Name == descendantid)))
                        lock (dictionary)
                        {
                            //dictionary[child.Name.ToString()] = child.Value.Trim();
                            dictionary[string.Format("item{0}", index++)] = child;
                        }
                }
                yield return expandoObject;
            }

        }

        #region DataLogger
        private static void RestoreDatalogger(string source, string destination, string dlroptions, bool redundancy)
        {
            if (bExitMode)
                return;

            DataLoggerModel.Helpers.DataLoggerInfo dataloggerinfo = null;
            try
            {
                if (System.IO.File.Exists(dlroptions))
                {
                    var stream = System.IO.File.ReadAllText(dlroptions);
                    dataloggerinfo = stream.FromXml<DataLoggerModel.Helpers.DataLoggerInfo>();
                }
            }
            catch
            { }

            if (string.IsNullOrEmpty(source) || 
                string.IsNullOrEmpty(destination) || 
                dataloggerinfo == null || !dataloggerinfo.IsValid() ||
                !System.IO.File.Exists(source))
            {
                Console.WriteLine(Properties.Resources.InvalidOptions);
                return;
            }

            try
            {
                using (var writer = new DataWriter.DataSetWriter(dataloggerinfo.DataProvider, dataloggerinfo.Connection))
                {
                    var dbCommandBuilder = DataReader.DataReader.CreateDbCommandBuilder(dataloggerinfo.DataProvider);

                    DataTable dataTable = null;
                    if (dataloggerinfo.TableSchema != null)
                        dataTable = dataloggerinfo.TableSchema;
                    else
                    {
                        string select = string.Format("SELECT * FROM {0}{1}{2}", dbCommandBuilder.QuotePrefix, dataloggerinfo.TableName, dbCommandBuilder.QuoteSuffix);
                        dataTable = DataReader.DataReader.GetDataSetSqlData(dataloggerinfo.DataProvider,
                            dataloggerinfo.Connection, select, schema: DataSchemaType.SourceOnlySchema);
                    }
                    if (dataTable != null)
                    {
                        dataTable.TableName = dataloggerinfo.TableName;
                        dataTable.Constraints.Clear();

                        if (isRunningAsCFR21UserName)
                        {
                            var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(source));
                            using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                            {
                                dataTable.ReadXml(reader);
                            }
                        }
                        else
                            dataTable.ReadXml(source);

                        var copyDataView = new DataView(dataTable.Clone());
                        var invalidRows = dataTable.Clone();
                        while (dataTable.Rows.Count > 0)
                        {
                            DataRow row = dataTable.Rows[0];
                            if (redundancy)
                                row[dataloggerinfo.RedundancyColumnName] = DateTime.UtcNow;
                            copyDataView.Table.ImportRow(row);

                            try
                            {
                                writer.InsertRows(copyDataView);
                                writer.Commit();
                            }
                            catch (Exception ex)
                            {
                                invalidRows.ImportRow(row);
                                Console.WriteLine(string.Format(Properties.Resources.ErrorRestoringSourceRecord, row[dataloggerinfo.UtcTimeColumnName], ex.Message.Replace(Environment.NewLine, " ")));
                            }

                            dataTable.Rows.Remove(row);
                            copyDataView.Delete(0);
                        }

                        if (invalidRows.Rows.Count > 0)
                        {
                            try
                            {
                                var file = Path.ChangeExtension(source, Properties.Settings.Default.RejectedFileExt);
                                invalidRows.WriteXml(file);
                            }
                            catch
                            { }
                        }
                    }
                }

                RemoveRestoringFile(source);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(string.Format(Properties.Resources.ErrorRestoringSourceFile, e.Message.Replace(Environment.NewLine, " ")));
            }
        }
        #endregion

        #region EventLog
        private static void RestoreEvents(string source, string destination, bool redundancy)
        {
            if (bExitMode)
                return;

            var inMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            var fileBase = XpoHelper.GetDataSourceFilePath(source);
            if (string.IsNullOrEmpty(fileBase) ||
                string.IsNullOrEmpty(destination) ||
                !System.IO.File.Exists(fileBase))
            {
                Console.WriteLine(Properties.Resources.InvalidOptions);
                return;
            }

            try
            {
                if (isRunningAsCFR21UserName)
                {
                    var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fileBase));
                    using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                    {
                        var xmlreader = XmlReader.Create(reader);
                        inMemory.ReadXml(xmlreader);
                    }
                }
                else
                    inMemory.ReadXml(fileBase);

                using (var idlSafely = new SimpleDataLayer(inMemory))
                {
                    using (var ufwSafely = new UnitOfWork(idlSafely))
                    {
                        List<UFUAAuditLogItem> restoring = (from entry in new XPQuery<UFUAAuditLogItem>(ufwSafely, true)/*.AsParallel()*/
                                                            select entry).ToList();

                        if (restoring.Count > 0)
                        {
                            using (var ridlSafely = HistorianHelper.CreateSimpleDataLayer<UFUAAuditLogItem>(destination, DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists))
                            {
                                using (var rufwSafely = new UnitOfWork(ridlSafely))
                                {
                                    try
                                    {
                                        rufwSafely.LockingOption = LockingOption.None;

                                        try
                                        {
                                            rufwSafely.ExplicitBeginTransaction(IsolationLevel.ReadUncommitted);
                                        }
                                        catch
                                        {
                                            rufwSafely.ExplicitBeginTransaction();
                                        }

                                        var copyEntities = restoring.ToList();
                                        copyEntities.ForEach(entity =>
                                        {
                                            //var bNewRecord = (from entry in new XPQuery<UFUAAuditLogItem>(rufwSafely)/*.AsParallel()*/
                                            //                  where entry.EventId == entity.EventId
                                            //                  select entry).Take(1).ToList().Count == 0;

                                            //System.Diagnostics.Debug.Assert(bNewRecord, "Warning",
                                            //            "Found the duplicated EventId '{0}' resotring event log from '{1}' to '{2}'",
                                            //            entity.EventId, source, destination);

                                            //if (bNewRecord)
                                            {
                                                UFUAAuditLogItem item = new UFUAAuditLogItem(rufwSafely, entity);

                                                if (redundancy)
                                                    item.RedundancySyncTime = DateTime.UtcNow;
                                            }

                                            try
                                            {
                                                rufwSafely.CommitChanges();
                                                restoring.Remove(entity);
                                                entity.Delete();
                                            }
                                            catch (Exception ex)
                                            {
                                                rufwSafely.RollbackTransaction();
                                                Console.WriteLine(string.Format(Properties.Resources.ErrorRestoringSourceRecord, entity.EventId, ex.Message.Replace(Environment.NewLine, " ")));
                                            }
                                        });

                                        rufwSafely.ExplicitCommitTransaction();
                                    }
                                    catch
                                    {
                                        rufwSafely.ExplicitRollbackTransaction();
                                        throw;
                                    }
                                }
                            }

                            if (restoring.Count > 0)
                                ufwSafely.TryCommitChanges();
                        }

                        if (restoring.Count > 0)
                        {
                            try
                            {
                                var file = Path.ChangeExtension(fileBase, Properties.Settings.Default.RejectedFileExt);
                                inMemory.WriteXml(file);
                            }
                            catch
                            { }
                        }
                    }
                }

                RemoveRestoringFile(source);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format(Properties.Resources.ErrorRestoringSourceFile, ex.Message.Replace(Environment.NewLine, " ")));
            }
        }
        #endregion

        #region Historian
        static Dictionary<int, UFUAAuditDataLog> mapDataLogRef = new Dictionary<int, UFUAAuditDataLog>();
        private static void RestoreHistorians(string source, string destination, bool redundancy)
        {
            if (bExitMode)
                return;

            var inMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            var fileBase = XpoHelper.GetDataSourceFilePath(source);
            if (string.IsNullOrEmpty(fileBase) ||
                string.IsNullOrEmpty(destination) ||
                !System.IO.File.Exists(fileBase))
            {
                Console.WriteLine(Properties.Resources.InvalidOptions);
                return;
            }

            try
            {
                if (isRunningAsCFR21UserName)
                {
                    var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fileBase));
                    using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                    {
                        var xmlreader = XmlReader.Create(reader);
                        inMemory.ReadXml(xmlreader);
                    }
                }
                else
                    inMemory.ReadXml(fileBase);

                using (var idlSafely = new SimpleDataLayer(inMemory))
                {
                    using (var ufwSafely = new UnitOfWork(idlSafely))
                    {

                        var restoringDataLog = (from entry in new XPQuery<UFUAAuditDataLog>(ufwSafely, true)/*.AsParallel()*/
                                                select entry).ToList();

                        var restoringDataItem = (from entry in new XPQuery<UFUAAuditDataItem>(ufwSafely, true)/*.AsParallel()*/
                                                 select entry).ToList();


                        if (restoringDataLog.Count > 0 || restoringDataItem.Count > 0)
                        {
                            using (var ridlSafely = HistorianHelper.CreateSimpleDataLayer<UFUAAuditDataItem>(destination, DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists))
                            {
                                using (var rufwSafely = new UnitOfWork(ridlSafely))
                                {
                                    try
                                    {
                                        rufwSafely.LockingOption = LockingOption.None;

                                        try
                                        {
                                            rufwSafely.ExplicitBeginTransaction(IsolationLevel.ReadUncommitted);
                                        }
                                        catch
                                        {
                                            rufwSafely.ExplicitBeginTransaction();
                                        }

                                        var copyDataLogItems = restoringDataLog.ToList();
                                        copyDataLogItems.ForEach(dataLogItem =>
                                        {
                                            UFUAAuditDataLog dataLog = (from entry in new XPQuery<UFUAAuditDataLog>(rufwSafely)/*.AsParallel()*/
                                                                        where entry.NodeId == dataLogItem.NodeId
                                                                        select entry).FirstOrDefault();
                                            if (dataLog == null)
                                            {
                                                dataLog = new UFUAAuditDataLog(rufwSafely)
                                                {
                                                    Name = dataLogItem.Name,
                                                    Description = dataLogItem.Description,
                                                    HistoricalName = dataLogItem.HistoricalName,
                                                    NodeId = dataLogItem.NodeId
                                                };
                                            }

                                            try
                                            {
                                                rufwSafely.CommitChanges();
                                                restoringDataLog.Remove(dataLogItem);

                                                if (!mapDataLogRef.ContainsKey(dataLogItem.Oid))
                                                    mapDataLogRef.Add(dataLogItem.Oid, dataLog);
                                                dataLogItem.Delete();
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(string.Format(Properties.Resources.ErrorRestoringSourceRecord, dataLogItem.Oid, ex.Message.Replace(Environment.NewLine, " ")));
                                            }
                                        });

                                        var copyDataItems = restoringDataItem.ToList();
                                        copyDataItems.ForEach(entity =>
                                        {
                                            if (!mapDataLogRef.ContainsKey(entity.DataLogRef))
                                                return;

                                            UFUAAuditDataLog dataLog = mapDataLogRef[entity.DataLogRef];
                                            //var bNewRecord = (from entry in new XPQuery<UFUAAuditDataItem>(rufwSafely)/*.AsParallel()*/
                                            //                  where entry.EventId == entity.EventId
                                            //                  select entry).Take(1).ToList().Count == 0;

                                            //System.Diagnostics.Debug.Assert(bNewRecord, "Warning",
                                            //    "Found the duplicated EventId '{0}' resotring historian from '{1}' to '{2}'",
                                            //    entity.EventId, source, destination);

                                            //if (bNewRecord)
                                            {
                                                UFUAAuditDataItem item = new UFUAAuditDataItem(rufwSafely, entity)
                                                {
                                                    DataLogRef = dataLog.Oid
                                                };

                                                if (redundancy)
                                                    item.RedundancySyncTime = DateTime.UtcNow;
                                            }

                                            try
                                            {
                                                rufwSafely.CommitChanges();
                                                restoringDataItem.Remove(entity);
                                                entity.Delete();
                                            }
                                            catch (Exception ex)
                                            {
                                                rufwSafely.RollbackTransaction();
                                                Console.WriteLine(string.Format(Properties.Resources.ErrorRestoringSourceRecord, entity.Oid, ex.Message.Replace(Environment.NewLine, " ")));
                                            }
                                        });

                                        rufwSafely.ExplicitCommitTransaction();
                                    }
                                    catch
                                    {
                                        rufwSafely.ExplicitRollbackTransaction();
                                        throw;
                                    }
                                }
                            }

                            if (restoringDataLog.Count > 0 || restoringDataItem.Count > 0)
                                ufwSafely.TryCommitChanges();
                        }

                        if (restoringDataLog.Count > 0 || restoringDataItem.Count > 0)
                        {
                            try
                            {
                                var file = Path.ChangeExtension(fileBase, Properties.Settings.Default.RejectedFileExt);
                                inMemory.WriteXml(file);
                            }
                            catch
                            { }
                        }
                    }
                }

                RemoveRestoringFile(source);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format(Properties.Resources.ErrorRestoringSourceFile, ex.Message.Replace(Environment.NewLine, " ")));
            }
        }
        #endregion

        private static void RemoveRestoringFile(String datasource)
        {
            try
            {
                String filepath = XpoHelper.IsDataSource(datasource) ? XpoHelper.GetDataSourceFilePath(datasource) : datasource;
                if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(filepath))
                    System.IO.File.Delete(filepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Properties.Resources.ErrorDeletingSourceFile, ex.Message.Replace(Environment.NewLine, " ")));
            }
        }
        #endregion

    }
}
