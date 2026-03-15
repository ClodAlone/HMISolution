using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using System.IO;
using Opc.Ua;
using System.Threading;
using System.Text.RegularExpressions;
using System.Globalization;
using XpoHelpers;
using log4net;

namespace UFUATagLogger
{
    public class UFUATagLogger : IDisposable
    {
        #region Declarations
        readonly Object lockObject = new Object();
        readonly Dictionary<String, UFUATagLogEntity> pendingEntries = new Dictionary<String, UFUATagLogEntity>();
        readonly Dictionary<String, DateTime> lastSavedTimes = new Dictionary<String, DateTime>();
        readonly List<String> corruptFileNames = new List<String>();

#if !NET_STANDARD
        static readonly ILog logServer = LogManager.GetLogger(Properties.Resources.Server);
#else
        static readonly ILog logServer = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.Server);
#endif

        Timer saveRetentiveTagsExecuter;
        bool saveRetentiveTagsExecuting;
        bool ExitMode;
        readonly String defaultSettings;
        bool bDisposed;
#endregion

        public UFUATagLogger(String defSettings)
        {
            defaultSettings = defSettings;
        }

#region Persistence

        IDataLayer CreateDataLayer(UFUATagLogEntity uFUATagLogEntity, out IDisposable[] objectsToDisposeOnDisconnect, bool retry = true, string filename = null)
        {
            string conn = null;
            string file = null;
            try
            {
                conn = XpoHelper.GetConnectionString(defaultSettings, Properties.Settings.Default.TypeLabel, filename ?? uFUATagLogEntity.TagName, Properties.Settings.Default.DefaultFileExt);
                file = XpoHelper.GetDataSourceFilePath(conn);
                if (file != null && corruptFileNames.Contains(file))
                {
                    corruptFileNames.Remove(file);
                    if (File.Exists(file))
                    {
                        File.Copy(file, file + ".bak", true);
                        File.Delete(file);
                    }
                }

                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                dict.GetDataStoreSchema(typeof(TagPersistence).Assembly);
                return XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema, out objectsToDisposeOnDisconnect);
                /* http://www.devexpress.com/Support/Center/Question/Details/Q535243
                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                var store = XpoDefault.GetConnectionProvider(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                dict.GetDataStoreSchema(typeof(TagPersistence).Assembly);
                return new ThreadSafeDataLayer(dict, store);
                */
            }
            catch (PathTooLongException e)
            {
                if (filename == null && uFUATagLogEntity.TagName != null)
                {
                    var index = uFUATagLogEntity.TagName.LastIndexOf('.');
                    if (index != -1)
                        return CreateDataLayer(uFUATagLogEntity, out objectsToDisposeOnDisconnect, filename: uFUATagLogEntity.TagName.Substring(index + 1));
                }
                else
                {
                    var message = String.Format(Properties.Resources.ErrorCreatingDataLayer, uFUATagLogEntity.TagName, e.Message);
                    var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                    Console.WriteLine(logMessage);

                    logServer.Error(message);
                }
            }
            catch (Exception e)
            {
                if (file != null && File.Exists(file))
                {
                    try
                    {
                        if (e.InnerException != null && e.InnerException is System.UnauthorizedAccessException)
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            if (retry)
                                return CreateDataLayer(uFUATagLogEntity, out objectsToDisposeOnDisconnect, false);
                        }
                        File.Copy(file, file + ".bak", true);
                        File.Delete(file);
                    }
                    catch
                    {
                        corruptFileNames.Add(file);
                    }
                }

                var message = String.Format(Properties.Resources.ErrorCreatingDataLayer, uFUATagLogEntity.TagName, e.InnerException != null ? e.InnerException.Message : e.Message);
                var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                Console.WriteLine(logMessage);

                logServer.Error(message);
            }

            objectsToDisposeOnDisconnect = new IDisposable[0];

            return null;
        }

#endregion

#region Implementation

        public bool GetDataValue(UFUATagLogEntity entity)
        {
            lock (lockObject)
            {
                IDisposable[] objectsToDisposeOnDisconnect;
                var idl = CreateDataLayer(entity, out objectsToDisposeOnDisconnect);
                if (idl == null)
                    return false;

                try
                {
                    using (var ufw = new UnitOfWork(idl))
                    {
                        try
                        {
                            var entry = (from c in new XPQuery<TagPersistence>(ufw)/*.AsParallel()*/
                                         where c.NodeId == entity.NodeId
                                         select c).Single();

                            try
                            {
                                entity.Value.Value = ChangeType(entry.Value, entity.builtinType, entity.arraySizeOneDimension);
                            }
                            catch
                            { }
                            entity.Value.SourceTimestamp = entry.ServerTimeStamp;
                            entity.Value.SourcePicoseconds = entry.SourcePicoseconds;
                            entity.Value.ServerTimestamp = entry.ServerTimeStamp;
                            entity.Value.ServerPicoseconds = entry.ServerPicoseconds;
                            entity.Value.StatusCode = entry.StatusCode;

                            entity.min = entry.Min;
                            entity.max = entry.Max;
                            entity.totAverage = entry.TotAverage;
                            entity.countUpdates = entry.CountUpdates;
                            entity.totalTimeOn = entry.TotalTimeOn;
                            entity.lastTotalTimeOn = entry.LastTotalTimeOn;
                            entity.lastDoubleValue = entry.LastDoubleValue;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
                finally
                {
                    idl.Dispose();
                    foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                        obj.Dispose();
                }

                return true;
            }
        }

        static object ChangeType(Object v, BuiltInType builtinType, uint arraySizeOneDimension = 0)
        {
            object value = v;
            if (arraySizeOneDimension > 0)
            {
                if (value is Array)
                {
                    var values = value as Array;
                    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)arraySizeOneDimension);
                    for (int ii = 0; ii < values.Length && ii < array.Length; ii++)
                    {
                        array.SetValue(ChangeType(values.GetValue(ii), builtinType), ii);
                    }

                    return array;
                }
                else if (value is String &&
                    (value as String).Length > 2 && (value as String)[0] == '{' &&
                    (value as String)[(value as String).Length - 1] == '}')
                {
                    var values = (value as String).Substring(1, (value as String).Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)arraySizeOneDimension);
                    for (int ii = 0; ii < values.Length && ii < array.Length; ii++)
                    {
                        array.SetValue(ChangeType(values[ii], builtinType), ii);
                    }

                    return array;
                }
                else if (value is String &&
                    builtinType == BuiltInType.Byte &&
                    (value as String).Length >= (arraySizeOneDimension * 2))
                {
                    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)arraySizeOneDimension);
                    System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
                    for (int ii = 0; ii < array.Length; ii++)
                    {
                        array.SetValue(Byte.Parse((value as String).Substring(ii * 2, 2), System.Globalization.NumberStyles.HexNumber, info), ii);
                    }

                    return array;
                }
                else
                    throw new InvalidCastException(String.Format("Cannot cast the value '{0}' to type {1}({2})", v, builtinType, arraySizeOneDimension));
            }

            try
            {
                if (value is String && builtinType != BuiltInType.String)
                {
                    if (String.Compare(value as String, "True", true) == 0)
                        v = 1;
                    else if (String.Compare(value as String, "False", true) == 0)
                        v = 0;
                    else
                    {
                        System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                        v = Convert.ToDouble(v, info);
                    }
                }
            }
            catch { }

            switch (builtinType)
            {
                case BuiltInType.Boolean: value = Convert.ToBoolean(v); break;
                case BuiltInType.SByte: value = Convert.ToSByte(v); break;
                case BuiltInType.Byte: value = Convert.ToByte(v); break;
                case BuiltInType.Int16: value = Convert.ToInt16(v); break;
                case BuiltInType.UInt16: value = Convert.ToUInt16(v); break;
                case BuiltInType.Int32: value = Convert.ToInt32(v); break;
                case BuiltInType.UInt32: value = Convert.ToUInt32(v); break;
                case BuiltInType.Int64: value = Convert.ToInt64(v); break;
                case BuiltInType.UInt64: value = Convert.ToUInt64(v); break;
                case BuiltInType.Float: value = Convert.ToSingle(v); break;
                case BuiltInType.Double: value = Convert.ToDouble(v); break;
            }

            return value;
        }

        public void AddLogEntity(UFUATagLogEntity entity)
        {
            if (bDisposed || ExitMode)
                return;

            lock (lockObject)
            {
                pendingEntries[entity.NodeId] = entity;
            }

            var delay = Properties.Settings.Default.DelaySavePersistence;
            StartFlushing(TimeSpan.FromMilliseconds(delay));
        }

        static void SetTagPersistenceEntityData(TagPersistence item, UFUATagLogEntity entity)
        {
            item.NodeId = entity.NodeId;
            item.FriendlyName = entity.TagName;
            item.Value = String.Format(CultureInfo.InvariantCulture, "{0}", entity.Value.WrappedValue).TrimEnd(Char.MinValue);
            item.SourceTimeStamp = entity.Value.SourceTimestamp;
            item.SourcePicoseconds = entity.Value.SourcePicoseconds;
            item.ServerTimeStamp = entity.Value.ServerTimestamp;
            item.ServerPicoseconds = entity.Value.ServerPicoseconds;
            item.StatusCode = entity.Value.StatusCode.Code;
            item.Status = String.Format("{0}", entity.Value.StatusCode);

            item.Min = entity.min;
            item.Max = entity.max;
            item.TotAverage = entity.totAverage;
            item.CountUpdates = entity.countUpdates;
            item.TotalTimeOn = entity.totalTimeOn;
            item.LastTotalTimeOn = entity.lastTotalTimeOn;
            item.LastDoubleValue = entity.lastDoubleValue;
        }

        void StartFlushing(TimeSpan delayTime)
        {
            lock (lockObject)
            {
                if (saveRetentiveTagsExecuting || saveRetentiveTagsExecuter != null)
                    return;
                
                saveRetentiveTagsExecuter = new Timer((o) =>
                {
                    try
                    {
                        var entities = new List<UFUATagLogEntity>();
                        lock (lockObject)
                        {
                            if (saveRetentiveTagsExecuter != null)
                            {
                                saveRetentiveTagsExecuter.Dispose();
                                saveRetentiveTagsExecuter = null;
                            }

                            saveRetentiveTagsExecuting = true;
                            entities.AddRange(pendingEntries.Values);
                            pendingEntries.Clear();
                        }
#if DEBUG
                        var startingTime = DateTime.UtcNow;
                        var watcher = new System.Diagnostics.Stopwatch();
                        watcher.Start();
#endif
                        SaveRetentiveTags(entities, delayTime);
#if DEBUG
                        watcher.Stop();
                        System.Diagnostics.Debug.WriteLine(String.Format("UFUATagLogger - Saved Retentive Tags, started {0}, executed in {1}", startingTime, watcher.Elapsed));
#endif
                    }
                    finally
                    {
                        lock (lockObject)
                        {
                            saveRetentiveTagsExecuting = false;
                            if (pendingEntries.Values.Count > 0)
                                StartFlushing(delayTime);
                        }
                    }
                }, null, delayTime, TimeSpan.FromMilliseconds(-1));
            }
        }

        void SaveRetentiveTags(List<UFUATagLogEntity> entities, TimeSpan delayTime)
        {
            var unsavedEntities = new List<UFUATagLogEntity>();
            while (entities.Count > 0)
            {
                var entity = entities[0];
                entities.RemoveAt(0);

                if (delayTime == TimeSpan.Zero ||
                    !lastSavedTimes.ContainsKey(entity.NodeId) ||
                    (DateTime.UtcNow - lastSavedTimes[entity.NodeId] > delayTime))
                {
                    IDisposable[] objectsToDisposeOnDisconnect;
                    var idl = CreateDataLayer(entity, out objectsToDisposeOnDisconnect);
                    if (idl != null)
                    {
                        try
                        {
                            using (var ufw = new UnitOfWork(idl))
                            {
                                var entriesToDelete = (from entry in new XPQuery<TagPersistence>(ufw)/*.AsParallel()*/
                                                       where entry.NodeId == entity.NodeId
                                                       select entry).ToList();

                                ufw.Delete(entriesToDelete);

                                TagPersistence item = new TagPersistence(ufw);
                                SetTagPersistenceEntityData(item, entity);

                                ufw.CommitChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            var message = String.Format(Properties.Resources.ErrorOnSaving, entity.TagName, ex.Message);
                            var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                            Console.WriteLine(logMessage);

                            logServer.Error(message);
                        }
                        finally
                        {
                            idl.Dispose();
                            foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                                obj.Dispose();
                        }
                    }

                    lastSavedTimes[entity.NodeId] = DateTime.UtcNow;
                }
                else
                    unsavedEntities.Add(entity);
            }

            if (unsavedEntities.Count > 0)
            {
                lock (lockObject)
                {
                    while (unsavedEntities.Count > 0)
                    {
                        var tagLogEntity = unsavedEntities[0];
                        unsavedEntities.RemoveAt(0);

                        if (!pendingEntries.ContainsKey(tagLogEntity.NodeId))
                            pendingEntries.Add(tagLogEntity.NodeId, tagLogEntity);
                    }
                }
            }
        }

        void ExitPendingJobs()
        {
            ExitMode = true;

            AutoResetEvent waitHandle = null;
            lock (lockObject)
            {
                if (saveRetentiveTagsExecuter != null)
                {
                    waitHandle = new AutoResetEvent(false);
                    saveRetentiveTagsExecuter.Change(0, System.Threading.Timeout.Infinite);
                    saveRetentiveTagsExecuter.Dispose(waitHandle);
                }
            }
            if (waitHandle != null)
            {
                waitHandle.WaitOne();
                waitHandle.Dispose();
            }

            lock (lockObject)
            { 
                SaveRetentiveTags(pendingEntries.Values.ToList(), TimeSpan.Zero);
                pendingEntries.Clear();
            }
        }

#endregion
        
#region IDisposable Members

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            ExitPendingJobs();
        }

#endregion
    }
}
