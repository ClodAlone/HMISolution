using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
#if !NET_STANDARD
using System.Windows.Controls;
using VFS;
using UFUAServerCMS;
using Utilities.WPF;
using UIMsgBoxAlertService.ComponentService;
using DriverSettingsInterfaces;
using SmartTagsControl.ComponentService;
using WPFUtilities.Extensions;
using UFUAEditor.Alarms;
using UFUAEditor.RemovedVariables;
using WPFUtilities.ImportExportHelpers;
#endif
using UFUAEditor.Controls;
using System.ComponentModel;
using ViewModelLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Diagnostics;
using Utilities;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Globalization;
using System.Windows;
using System.Xml;
using System.Text.RegularExpressions;
using OPCUAViewModel;
using Opc.Ua;
using DevExpress.Xpo.DB.Helpers;
using StringManager;
using System.Reflection;
using log4net;
using WPFUtilities;
using DevExpress.Data.Filtering;
using System.Threading;
using System.Xml.Linq;
using DataLoggerModel;
using XpoHelpers;
using UFUAModel;
using UFInterfaces;
#if !NET_STANDARD
using TempVarriables.ComponentService;
using Utilities.Xpo.UndoRedo;
using DevExpress.XtraRichEdit.Import.Rtf;
using CommandManager;
using UFUAEditor.Helpers;
#endif

namespace UFUAEditor.Document
{

    public class UFUAServerDocument : ViewModelBase,
#if !NET_STANDARD
        ICloneable, 
#endif
        IDocument, IXpoDocument
    {
        #region Declarations

        UnitOfWork uow;
        CachedUnitOfWork cachedUow;
        InMemoryDataStore InMemory;
        IDataLayer dl;
#if !NET_STANDARD
        //UnitOfWork uowCloner;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;
        IDataLayer dlClipboard;
#endif
        String defaultApplicationName;
        String connectionString;
        String fileBase;

#if !NET_STANDARD
        RemovedVariablesManager removedVariablesManager;
        ClipboardElements clipboardElements;

        UndoRedoXpoManager undoRedoHelper;
        IDataLayer dlUndoRedo;
        UnitOfWork uowUndoRedo;
#endif

        readonly CachedUnitOfWorks cachedUnitOfWorks;
        
        readonly Dictionary<Thread, String> cachedDefaultLocalEndpoint = new Dictionary<Thread, String>();
        readonly Dictionary<Thread, String> cachedDefaultAppName = new Dictionary<Thread, String>();
        readonly Dictionary<CachedUnitOfWork, CacheTags> cachedMapTags = new Dictionary<CachedUnitOfWork, CacheTags>();
        List<String> notFoundTagNames;

#if !NET_STANDARD
        internal static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);
#else
        internal static readonly ILog logGeneral = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif

        readonly static string[] transportOrderByRelevance = new string[]
        {
#if !NET_STANDARD
            Opc.Ua.Utils.UriSchemeNetPipe,
            Opc.Ua.Utils.UriSchemeNetTcp,
#endif
            Opc.Ua.Utils.UriSchemeOpcTcp,
            Opc.Ua.Utils.UriSchemeHttps,
#if !NET_STANDARD
            Opc.Ua.Utils.UriSchemeHttp,
            Opc.Ua.Utils.UriSchemeNoSecurityHttp
#endif
		};

#if !NET_STANDARD
        object tagsListLock = new object();
        List<string> listFlatFullTagNameCollection;
        List<string> listFlatFullTagNameCollectionAdded;
        List<string> listFlatFullTagNameCollectionRemoved;
        List<string> listFlatFullHistoricalTagNameCollection;
        List<string> listFlatFullHistoricalTagNameCollectionAdded;
        List<string> listFlatFullHistoricalTagNameCollectionRemoved;
        List<string> listFlatFullFolderNameCollection;
        List<string> listFlatFullFolderNameCollectionAdded;
        List<string> listFlatFullFolderNameCollectionRemoved;

        bool bInvalidateDriversDynamicSettingsOnSave;
#endif


        #endregion

        #region Constructors
        public UFUAServerDocument()
        {
            cachedUnitOfWorks = new CachedUnitOfWorks(this);
            cachedUnitOfWorks.CleaningUnitOfWorkEvent += (s, e) =>
            {
                lock (lockObject)
                {
                    if (cachedMapTags.ContainsKey(e.Task))
                        cachedMapTags.Remove(e.Task);
                }
            };
        }
        #endregion

        #region Methods

#if !NET_STANDARD
        internal void RenameReferences(Dictionary<string, string> nodeIdMap, UFInterfaces.Editors.CrossReferenceModel model)
        {
            string endpointUrl = GetDefaultLocalEndpoint(true);
            string applicationName = GetAplicationName(true);
            using (var cursor = new WaitCursor())
            {
                var list = new List<UFUAModel.TagEntityReference>();
                var opclist = new List<OPCUAEntityReference>();
                var mapTagChanged = new Dictionary<String, String>();
                UInt16 ns = CrossReferenceHelper.Helper.GetNSNumber();
                //var dlrsettings = doc.GetDataLoggerSettings().ToList();
                var dlrsettings = (from dlr in GetDataLoggerSettings()
                                   where
                                   dlr.EnableRecordingTag != null ||
                                   dlr.RecordingTag != null ||
                                   dlr.ResettingTag != null ||
                                   dlr.Columns.Count > 0
                                   select dlr).ToList();

                dlrsettings.ForEach(dlr =>
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;

                    if (dlr.EnableRecordingTag != null && !dlr.EnableRecordingTag.IsEmpty())
                        list.Add(dlr.EnableRecordingTag);
                    if (dlr.RecordingTag != null && !dlr.RecordingTag.IsEmpty())
                        list.Add(dlr.RecordingTag);
                    if (dlr.ResettingTag != null && !dlr.ResettingTag.IsEmpty())
                        list.Add(dlr.ResettingTag);
                    var dlrColumnsTag = (from c in dlr.Columns where c.IsValid && c.ColumnTag != null select c.ColumnTag).ToList();
                    list.AddRange(dlrColumnsTag);
                });

                var historicalSettings = (from historian in GetHistoricalSettings()
                                          where historian.EnableTag != null
                                          select historian).ToList();

                historicalSettings.ForEach(historian =>
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;

                    if (historian.EnableTag != null && !historian.EnableTag.IsEmpty())
                        list.Add(historian.EnableTag);
                });

                var driversettings = GetDrivers().ToList();
                List<TagEntityReference> drivermap = new List<TagEntityReference>();
                List<TagEntityReference> replacedTagMap = new List<TagEntityReference>();
                driversettings.ForEach(driver =>
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;

                    try
                    {
                        Dictionary<string, TagEntityReference> res = GetDriverSettingsTagList(driver, this);
                        drivermap.AddRange(res.Values);
                    }
                    catch (Exception ex)
                    {
                        logGeneral.Error(ex.Message);
                    }
                });

                drivermap = drivermap.Distinct().ToList();
                list.AddRange(drivermap);


                var alarmThresholds = (from alarmThreshold in GetAlarmThresholds(false)
                                       where
                                        alarmThreshold.EnableTag != null ||
                                        alarmThreshold.ActivationLowValueTag != null ||
                                        alarmThreshold.ActivationValueTag != null ||
                                        alarmThreshold.LowLimitTag != null ||
                                        alarmThreshold.LowLowLimitTag != null ||
                                        alarmThreshold.HighLimitTag != null ||
                                        alarmThreshold.HighHighLimitTag != null ||
                                        alarmThreshold.AliasTags.Count() > 0 ||
                                        alarmThreshold.HasCRCommands
                                       select alarmThreshold).ToList();

                bool bDirty = false;
                alarmThresholds.ForEach(alarmThreshold =>
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;
                    if (model.RenamedMap.Count > 0)
                    {
                        CommandManagerList command = alarmThreshold.CommandsAck?.FromXml<CommandManagerList>();
                        if (RenameCommand(command, model.RenamedMap, model))
                        {
                            bDirty = true;
                            alarmThreshold.CommandsAck = command.ToXml();
                        }
                        command = alarmThreshold.CommandsDbClick?.FromXml<CommandManagerList>();
                        if (RenameCommand(command, model.RenamedMap, model))
                        {
                            bDirty = true;
                            alarmThreshold.CommandsDbClick = command.ToXml();
                        }
                        command = alarmThreshold.CommandsOff?.FromXml<CommandManagerList>();
                        if (RenameCommand(command, model.RenamedMap, model))
                        {
                            bDirty = true;
                            alarmThreshold.CommandsOff = command.ToXml();
                        }
                        command = alarmThreshold.CommandsOn?.FromXml<CommandManagerList>();
                        if (RenameCommand(command, model.RenamedMap, model))
                        {
                            bDirty = true;
                            alarmThreshold.CommandsOn = command.ToXml();
                        }
                        command = alarmThreshold.CommandsReset?.FromXml<CommandManagerList>();
                        if (RenameCommand(command, model.RenamedMap, model))
                        {
                            bDirty = true;
                            alarmThreshold.CommandsReset = command.ToXml();
                        }
                    }
                    //if(getTags)
                    {
                        if (alarmThreshold.EnableTag != null && !alarmThreshold.EnableTag.IsEmpty())
                            list.Add(alarmThreshold.EnableTag);
                        if (alarmThreshold.ActivationLowValueTag != null && !alarmThreshold.ActivationLowValueTag.IsEmpty())
                            list.Add(alarmThreshold.ActivationLowValueTag);
                        if (alarmThreshold.ActivationValueTag != null && !alarmThreshold.ActivationValueTag.IsEmpty())
                            list.Add(alarmThreshold.ActivationValueTag);
                        if (alarmThreshold.LowLimitTag != null && !alarmThreshold.LowLimitTag.IsEmpty())
                            list.Add(alarmThreshold.LowLimitTag);
                        if (alarmThreshold.LowLowLimitTag != null && !alarmThreshold.LowLowLimitTag.IsEmpty())
                            list.Add(alarmThreshold.LowLowLimitTag);
                        if (alarmThreshold.HighLimitTag != null && !alarmThreshold.HighLimitTag.IsEmpty())
                            list.Add(alarmThreshold.HighLimitTag);
                        if (alarmThreshold.HighHighLimitTag != null && !alarmThreshold.HighHighLimitTag.IsEmpty())
                            list.Add(alarmThreshold.HighHighLimitTag);

                        for (int j = 0; j < alarmThreshold.AliasTags.Count(); j++)
                        {
                            var aliasTag = alarmThreshold.AliasTags[j];
                            if (IsTagReferenceValid(aliasTag.TagEntity))
                                list.Add(aliasTag.TagEntity);
                        }
                    }
                    if (alarmThreshold.HasCRCommands)
                    {
                        AddCommandsTag(alarmThreshold.CommandsAck, opclist);
                        AddCommandsTag(alarmThreshold.CommandsDbClick, opclist);
                        AddCommandsTag(alarmThreshold.CommandsOff, opclist);
                        AddCommandsTag(alarmThreshold.CommandsOn, opclist);
                        AddCommandsTag(alarmThreshold.CommandsReset, opclist);
                    }
                });

                var nodelist = (from c in list
                                let rnid = c.ResolveNodeId(ns)
                                where rnid != null
                                select rnid.ToString()).Distinct().ToList();
                var opcnodelist = (from c in opclist
                                   where c.ResolvedNodeId != null
                                   select c.ResolvedNodeId.ToString()).Distinct().ToList();
                var preMap = (from n in nodeIdMap
                              where nodelist.Contains(n.Key)
                              select n).ToDictionary(x => x.Key, x => x.Value);
                var toGetList = (from c in nodelist
                                 where !preMap.ContainsKey(c)
                                 select c).Distinct().ToList();
                var mapNodes = GetListNodeNames(toGetList);
                if (mapNodes == null)
                    return;
                mapNodes.ToList().ForEach(n => nodeIdMap.Add(n.Key, n.Value));
                preMap.ToList().ForEach(n => mapNodes.Add(n.Key, n.Value));
                if(list.Count > 0)
                foreach (var node in mapNodes.Keys)
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;

                    var found = (from c in list
                                 let rnid = c.ResolveNodeId(ns)
                                 where rnid != null &&
                                 rnid.ToString() == node
                                 select c).ToList();
                    found.ForEach(fitem =>
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return;

                        string nodeid = fitem.ResolveNodeId(ns).ToString();
                        var shortname = mapNodes[node];
                        var newName = String.Format("{0} ({1})", shortname, applicationName);
                        UFUAModel.TagEntityReference newEntity = null;
                        if (fitem.HumanReadable != newName)
                        {
                            fitem.HumanReadable = newName;
                            newEntity = new UFUAModel.TagEntityReference(fitem.Guid, CrossReferenceHelper.Helper.GetNewPath(shortname, fitem.Name), fitem.NodeId, newName);
                            fitem = newEntity;
                            bDirty = true;
                        }
                        if (newEntity != null)
                        {
                            dlrsettings.ForEach(dlr =>
                            {
                                if (dlr.EnableRecordingTag != null && dlr.EnableRecordingTag.ResolveNodeId(ns).ToString() == nodeid)
                                    dlr.EnableRecordingTag = newEntity;
                                if (dlr.RecordingTag != null && dlr.RecordingTag.ResolveNodeId(ns).ToString() == nodeid)
                                    dlr.RecordingTag = newEntity;
                                if (dlr.ResettingTag != null && dlr.ResettingTag.ResolveNodeId(ns).ToString() == nodeid)
                                    dlr.ResettingTag = newEntity;

                                var dlrColumnsTag = (from c in dlr.Columns where c.IsValid && c.ColumnTag != null && c.ColumnTag.ResolveNodeId(ns).ToString() == nodeid select c).ToList();
                                dlrColumnsTag.ForEach(c => c.ColumnTag = newEntity);
                                bDirty = true;
                            });

                            (from item in historicalSettings where item.EnableTag != null && item.EnableTag.ResolveNodeId(ns).ToString() == nodeid select item).ToList().ForEach(item =>
                            {
                                item.EnableTag = newEntity;
                                bDirty = true;
                            });
                            (from item in drivermap where item != null && item.ResolveNodeId(ns).ToString() == nodeid select item).ToList().ForEach(item =>
                            {
                                var _item = newEntity;
                                if (!replacedTagMap.Contains(_item))
                                    replacedTagMap.Add(_item);
                                bDirty = true;
                            });
                            (from item in alarmThresholds where item.EnableTag != null && item.EnableTag.ResolveNodeId(ns).ToString() == nodeid select item).ToList().ForEach(item =>
                            {
                                item.EnableTag = newEntity;
                                bDirty = true;
                            });
                            (from item in alarmThresholds where item.ActivationLowValueTag != null && item.ActivationLowValueTag.ResolveNodeId(ns).ToString() == nodeid select item).ToList().ForEach(item =>
                            {
                                item.ActivationLowValueTag = newEntity;
                                bDirty = true;
                            });
                            (from item in alarmThresholds where item.ActivationValueTag != null && item.ActivationValueTag.ResolveNodeId(ns).ToString() == nodeid select item).ToList().ForEach(item =>
                            {
                                item.ActivationValueTag = newEntity;
                                bDirty = true;
                            });
                            (from item in alarmThresholds where item.LowLimitTag != null && item.LowLimitTag.ResolveNodeId(ns).ToString() == nodeid select item).ToList().ForEach(item =>
                            {
                                item.LowLimitTag = newEntity;
                                bDirty = true;
                            });
                            (from item in alarmThresholds where item.LowLowLimitTag != null && item.LowLowLimitTag.ResolveNodeId(ns).ToString() == nodeid select item).ToList().ForEach(item =>
                            {
                                item.LowLowLimitTag = newEntity;
                                bDirty = true;
                            });
                            (from item in alarmThresholds where item.HighLimitTag != null && item.HighLimitTag.ResolveNodeId(ns).ToString() == nodeid select item).ToList().ForEach(item =>
                            {
                                item.HighLimitTag = newEntity;
                                bDirty = true;
                            });
                            (from item in alarmThresholds where item.HighHighLimitTag != null && item.HighHighLimitTag.ResolveNodeId(ns).ToString() == nodeid select item).ToList().ForEach(item =>
                            {
                                item.HighHighLimitTag = newEntity;
                                bDirty = true;
                            });
                            (from item in alarmThresholds where item.AliasTags.Count() > 0 select item).ToList().ForEach(item =>
                            {
                                (from alias in item.AliasTags where alias.TagEntity != null select alias).ToList().ForEach(alias =>
                                {
                                    if (alias.TagEntity.ResolveNodeId(ns).ToString() == nodeid)
                                        alias.TagEntity = newEntity;
                                    bDirty = true;
                                });
                            });
                        }
                    });
                }

                preMap = (from n in nodeIdMap
                              where nodelist.Contains(n.Key)
                              select n).ToDictionary(x => x.Key, x => x.Value);
                toGetList = (from c in opcnodelist
                             where !preMap.ContainsKey(c)
                                 select c).Distinct().ToList();
                mapNodes = GetListNodeNames(toGetList);
                if (mapNodes == null)
                    return;
                mapNodes.ToList().ForEach(n => nodeIdMap.Add(n.Key, n.Value));
                preMap.ToList().ForEach(n => mapNodes.Add(n.Key, n.Value));
                if(opclist.Count > 0)
                foreach (var node in mapNodes.Keys)
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;

                    var opcfound = (from c in opclist
                                    where c.ResolvedNodeId != null &&
                                    c.ResolvedNodeId.ToString() == node
                                    select c).ToList();
                    opcfound.ForEach(nitem =>
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return;

                        var shortname = mapNodes[node];
                        var newName = String.Format("{0} ({1})", shortname, applicationName);
                        if (nitem.HumanReadable != newName || nitem.AppName != applicationName || nitem.EndpointUrl != endpointUrl)
                        {
                            nitem.AppName = applicationName;
                            nitem.EndpointUrl = endpointUrl;
                            nitem.HumanReadable = newName;
                            nitem.ReadablePath = CrossReferenceHelper.Helper.GetNewPath(shortname, nitem.ReadablePath);
                            nitem.RelativePath = CrossReferenceHelper.Helper.GetNewPath(shortname, nitem.RelativePath);

                            (from item in alarmThresholds where !string.IsNullOrEmpty(item.CommandsAck) && item.CommandsAck.Contains(node) select item).ToList().ForEach(item =>
                            {
                                item.CommandsAck = RenameTag(item.CommandsAck, nitem, model);
                            });
                            (from item in alarmThresholds where !string.IsNullOrEmpty(item.CommandsDbClick) && item.CommandsDbClick.Contains(node) select item).ToList().ForEach(item =>
                            {
                                item.CommandsDbClick = RenameTag(item.CommandsDbClick, nitem, model);
                            });
                            (from item in alarmThresholds where !string.IsNullOrEmpty(item.CommandsOff) && item.CommandsOff.Contains(node) select item).ToList().ForEach(item =>
                            {
                                item.CommandsOff = RenameTag(item.CommandsOff, nitem, model);
                            });
                            (from item in alarmThresholds where !string.IsNullOrEmpty(item.CommandsOn) && item.CommandsOn.Contains(node) select item).ToList().ForEach(item =>
                            {
                                item.CommandsOn = RenameTag(item.CommandsOn, nitem, model);
                            });
                            (from item in alarmThresholds where !string.IsNullOrEmpty(item.CommandsReset) && item.CommandsReset.Contains(node) select item).ToList().ForEach(item =>
                            {
                                item.CommandsReset = RenameTag(item.CommandsReset, nitem, model);
                            });
                            bDirty = true;
                        }
                    });
                }

                if (bDirty)
                {
                    driversettings.ForEach(driver =>
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return;

                        SetDriverSettingsTagList(replacedTagMap, driver);
                    });
                    SaveToFile(bForceSave:true);
                }
            }
        }

        private bool RenameCommand(CommandManagerList commandList, Dictionary<String, String> renamed, UFInterfaces.Editors.CrossReferenceModel model)
        {
            bool bDirty = false;
            if(commandList != null)
                foreach (var command in commandList)
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return bDirty;
                    CommandManager.OpenScreenCommand screencommand = command as CommandManager.OpenScreenCommand;
                    var path = screencommand?.ScreenName.GetPathString();
                    if (screencommand != null && !string.IsNullOrEmpty(path))
                    {
                        if (renamed.ContainsKey(path))
                        {
                            screencommand.ScreenName = new Uri(renamed[path], UriKind.RelativeOrAbsolute);
                            bDirty = true;
                        }
                    }
                }
            return bDirty;
        }

        private void SetDriverSettingsTagList(List<TagEntityReference> replacedTagMap, UFUAModel.UFUACommunicationDriver driver)
        {
            if (driver == null)
                return;

            var dll = String.Format("{0}\\{1}", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), driver.AssemblyName);
            var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(dll);

            DriverSettingsInterfaces.ICrossReferenceAware driverWpfEditing = null;
            try
            {
                var types = Assembly.LoadFile(dll).GetTypes();
                var list = (from t in types/*.AsParallel()*/
                            where !t.IsAbstract && typeof(DriverSettingsInterfaces.ICrossReferenceAware).IsAssignableFrom(t)
                            select (DriverSettingsInterfaces.ICrossReferenceAware)Activator.CreateInstance(t)).ToList();
                if(list.Count > 0)
                    driverWpfEditing = list[0];

                if (driverWpfEditing == null)
                    return;
            }
            catch (Exception ex)
            {
                logGeneral.Error(String.Format(Properties.Resources.CommDriverNotFound, uidll));
                return;
            }

            try
            {
                var settingsContext = new ComunicationSettingsContext2()
                {
                    ConnectionString = ConnectionString,
                    bProtected = Protected,
                    protectionCode = Id,
                    listTag = GetFlatFullTagNameNodeIdCollection()
                };

                List<string> list = (from tag in replacedTagMap select tag.ToXml()).ToList();
                driverWpfEditing.SetTagList(list, settingsContext);
                return;
            }
            catch (DriverBaseInterfaces.ValidatingDocumentException ex)
            {
                logGeneral.Error(String.Format(Properties.Resources.ErrorValidatingDocument, ex.FilePath));
            }
            return;
        }

        internal Dictionary<string, UFUAModel.TagEntityReference> GetDriverSettingsTagList(UFUAModel.UFUACommunicationDriver driver, UFUAServerDocument doc)
        {
            Dictionary<string, UFUAModel.TagEntityReference> result = new Dictionary<string, UFUAModel.TagEntityReference>();

            if (driver == null)
                return result;

            var dll = String.Format("{0}\\{1}", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), driver.AssemblyName);
            var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(dll);

            DriverSettingsInterfaces.ICrossReferenceAware driverWpfEditing = null;
            Type[] types;
            try
            {
               types = Assembly.LoadFile(dll).GetTypes();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}: {dll}");
            }
            
            var list = (from t in types/*.AsParallel()*/
                        where !t.IsAbstract && typeof(DriverSettingsInterfaces.ICrossReferenceAware).IsAssignableFrom(t)
                        select (DriverSettingsInterfaces.ICrossReferenceAware)Activator.CreateInstance(t)).ToList();
            if (list.Count == 0)
                return result;

            driverWpfEditing = list[0];

            if (driverWpfEditing == null)
                return result;

            var settingsContext = new ComunicationSettingsContext2()
            {
                ConnectionString = ConnectionString,
                bProtected = Protected,
                protectionCode = Id,
                listTag = GetFlatFullTagNameNodeIdCollection()
            };

            var map = driverWpfEditing.GetTagList(settingsContext);
            (from key in map.Keys select key).ToList().ForEach(k =>
            {
                result.Add(k, map[k].FromXml<TagEntityReference>());
            });
            return result;
        }

        private void AddCommandsTag(string commands, List<OPCUAEntityReference> list)
        {
            if (!string.IsNullOrEmpty(commands))
            {
                var commandList = commands.FromXml<CommandManager.CommandManagerList>();
                for (int i = 0; i < commandList.Count(); i++)
                {
                    var command = commandList[i];
                    command.ListTags.ForEach(tag =>
                    {
                        list.Add(tag);
                    });
                }
            }
        }
        private string RenameTag(string commands, OPCUAEntityReference newTag, UFInterfaces.Editors.CrossReferenceModel model)
        {
            if (!string.IsNullOrEmpty(commands))
            {
                bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
                bool getScreen = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Screens);
                Dictionary<String, String> renamed = model.RenamedMap;
                var commandList = commands.FromXml<CommandManager.CommandManagerList>();
                for (int i = 0; i < commandList.Count(); i++)
                {
                    var command = commandList[i];
                    //if(getTags)
                    {
                        command.ListTags.ForEach(tag =>
                        {
                            if (newTag.ResolvedNodeId == tag.ResolvedNodeId && (tag.HumanReadable != newTag.HumanReadable || newTag.EndpointUrl != tag.EndpointUrl))
                            {
                                tag.HumanReadable = newTag.HumanReadable;
                                tag.ReadablePath = newTag.ReadablePath;
                                tag.RelativePath = newTag.RelativePath;
                                tag.AppName = newTag.AppName;
                                tag.EndpointUrl = newTag.EndpointUrl;
                            }
                        });
                        if (command is CommandManager.ReportCommand)
                        {
                            var repCommand = command as CommandManager.ReportCommand;
                            if (repCommand.Parameters != null)
                                (from p in repCommand.Parameters where p.TagRef != null && newTag.ResolvedNodeId == p.TagRef.ResolvedNodeId select p).ToList().ForEach(p =>
                                {
                                    p.TagRefXml = newTag.ToXml();
                                });
                        }
                    }
                    //if(getScreen)
                    {
                        CommandManager.OpenScreenCommand scommand = command as CommandManager.OpenScreenCommand;
                        var path = scommand?.ScreenName.GetPathString();
                        if (scommand != null && !string.IsNullOrEmpty(path))
                        {
                            if (renamed.ContainsKey(path))
                                scommand.ScreenName = new Uri(renamed[path], UriKind.RelativeOrAbsolute);
                        }
                    }
                }
                commands = commandList.ToXml();
            }
            return commands;
        }
        private bool IsTagReferenceValid(UFUAModel.TagEntityReference tagReference)
        {
            return tagReference != null && !tagReference.IsEmpty();
        }
#endif

        public IDataLayer GetDataLayer()
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(UFUAModel.UFUATag).Assembly, typeof(DataLoggerModel.DataLoggerSettings).Assembly);
            dict.GetDataStoreSchema(typeof(XpoHelpers.ProtectionFile));

            if (String.IsNullOrEmpty(fileBase))
            {
                // dl = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                var store = DevExpress.Xpo.XpoDefault.GetConnectionProvider(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                var ret = new DevExpress.Xpo.ThreadSafeDataLayer(dict, store);

                return ret;
            }
            else
            {
                lock (lockObject)
                {
                    if (InMemory == null)
                    {
                        InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                        var fileInfo = new FileInfo(fileBase);
                        if (fileInfo.Exists && fileInfo.Length > 0)
                        {
                            if (Protected || !Utilities.IO.FileSystem.IsXmlFile(fileBase))
                            {
                                var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fileBase));
                                using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                                {
                                    var xmlreader = XmlReader.Create(reader);
                                    try
                                    {
                                        InMemory.ReadXml(xmlreader);
                                    }
                                    catch (Exception ex)
                                    {
                                        File.Copy(fileBase, String.Format("{0}.bak", fileBase), true);
                                        File.Delete(fileBase);
#if !NET_STANDARD
                                        var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                        if (uiMsgBox != null)
                                            uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorCorruptedDocument.Replace("-newline-", Environment.NewLine), fileBase));
#endif
                                        logGeneral.Error(String.Format(Properties.Resources.ErrorReadingDocument, fileBase), ex);
                                    }
                                }
                            }
                            else
                                try
                                {
                                    InMemory.ReadXml(fileBase);
                                }
                                catch (Exception ex)
                                {
                                    File.Copy(fileBase, String.Format("{0}.bak", fileBase), true);
                                    File.Delete(fileBase);
#if !NET_STANDARD
                                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                    if (uiMsgBox != null)
                                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorCorruptedDocument.Replace("-newline-", Environment.NewLine), fileBase));
#endif
                                    logGeneral.Error(String.Format(Properties.Resources.ErrorReadingDocument, fileBase), ex);
                                }
                        }
                    }
                }

                return new DevExpress.Xpo.ThreadSafeDataLayer(dict, InMemory);
            }
        }

#if !NET_STANDARD
        bool trackingChangesDisabled;
#endif
        void CreateDataLayer(bool bUseCacheUow)
        {
            dl = GetDataLayer();
            if (bUseCacheUow)
            {
                cachedUow = cachedUnitOfWorks.BeginUnitOfWork();
                cachedUow.KeepUnitOfWork = true;
                uow = cachedUow.UnitOfWork;
            }
            else
                uow = new UnitOfWork(dl);
#if !NET_STANDARD
            uow.ObjectChanged += (o, e) =>
                {
                    if (trackingChangesDisabled || !e.Session.TrackingChanges)
                        return;
                    NeedsSave = true;

                    if (e.PropertyName == "ApplicationName")
                    {
                        ConfigurationId = Guid.NewGuid();
                    }
                };

            // check if the address space need to reload because some treview item has been deleted
            uow.ObjectDeleting += (o, e) =>
            {
                if (trackingChangesDisabled || !e.Session.TrackingChanges)
                    return;
                NeedsSave = true;

                if (e.Object is UFUAModel.UFUAHistorianSettings)
                {
                    var hs = e.Object as UFUAModel.UFUAHistorianSettings;
                    RemoveHistoricalSettingsFromTags(hs.Name);
                }
                else if (e.Object is UFUAModel.UFUATagPrototype)
                {
                    var prototype = e.Object as UFUAModel.UFUATagPrototype;
                    ProcessRemoved(prototype);
                }
                else if (e.Object is UFUAModel.UFUATag)
                {
                    var ufuaTag = e.Object as UFUAModel.UFUATag;
                    ProcessRemoved(ufuaTag);
                }
                else if (e.Object is UFUAModel.UFUAEngineeringUnit)
                {
                    var eu = e.Object as UFUAModel.UFUAEngineeringUnit;
                    RemoveEngineeringUnitsFromTags(eu.Name);
                }
                else if (e.Object is UFUAModel.UFUAView)
                {
                    var hs = e.Object as UFUAModel.UFUAView;
                    RemoveViewFromTags(hs);
                }
            };

            uow.ObjectsSaved += (o, e) =>
            {
                if (trackingChangesDisabled)
                    return;
                NeedsSave = false;
            };

            //uowCloner = new UnitOfWork(dl);
            if (!bUseCacheUow)
            {
                InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                dlClipboard = new SimpleDataLayer(InMemoryClipboard);
                uowClipboard = new UnitOfWork(dlClipboard);
            }
#endif
        }

        bool IsBelongFromParent(IDocument parent)
        {
            var id = XpoHelpers.XpoHelper.GetProtectionCode(uow);
            return id == Guid.Empty || id == parent.Id;
        }

        public UnitOfWork GetSession()
        {
            return uow;
        }

#if !NET_STANDARD
        internal UserControl GetSmartTagsEditorObject(IXPSimpleObject source)
        {
            if (source is UFUAModel.UFUATag)
                return new NewTag(this, (source as UFUAModel.UFUATag).IsPrototypeMember);
            else if (source is UFUAModel.UFUAAlarmDefinition)
                return new NewAlarmDefinition(this);
            else if (source is UFUAModel.UFUAHistorianSettings)
                return new NewHistoricalSettings(this);

            return null;
        }

        internal bool GetFriendObjects(UFUAModel.UFUAAlarmThreshold data, UFInterfaces.GetFriendObjectsEventArgs e)
        {
            if (e.friendList == null)
                e.friendList = new List<Object>();

            e.friendList.Add(new AlarmCommandsEditObject(data, AlarmCommandsEventType.CommandsOn));
            e.friendList.Add(new AlarmCommandsEditObject(data, AlarmCommandsEventType.CommandsOff));
            e.friendList.Add(new AlarmCommandsEditObject(data, AlarmCommandsEventType.CommandsAck));
            e.friendList.Add(new AlarmCommandsEditObject(data, AlarmCommandsEventType.CommandsReset));
            e.friendList.Add(new AlarmCommandsEditObject(data, AlarmCommandsEventType.CommandsDbClick));

            return true;
        }

        internal void CreateUndoRedoHelper()
        {
            CreateUndoRedoHelper(Properties.Settings.Default.MaxUndoRedoActions);
        }

        internal void CreateUndoRedoHelper(short numactions)
        {
            if (undoRedoHelper != null)
                return;

            dlUndoRedo = XpoDefault.GetDataLayer(InMemoryDataStore.GetConnectionStringInMemory(true), DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            uowUndoRedo = new UnitOfWork(dlUndoRedo);
            undoRedoHelper = numactions > 0 ? new UndoRedoXpoManager(uow, uowUndoRedo, numactions) : new UndoRedoXpoManager(uow, uowUndoRedo);
        }
#endif

        void EnsureDefaultSettings(string title)
        {
            defaultApplicationName = String.Format("{0}_{1}", title, Properties.Settings.Default.AppNameSuffix);
            EnsureDefaultSettings();
        }

        void EnsureDefaultSettings()
        {
            var configuration = GetConfiguration();
            configuration.EnsureDefaultSettings(defaultApplicationName);
        }

        #region String helpers
#if !NET_STANDARD
        internal List<String> GetWholeStrings()
        {
            var ret = new List<String>();

            ret.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                          where !String.IsNullOrEmpty(p.AlarmText) && p.IsValid
                          select p.AlarmText).ToList());

            ret.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                          where (String.IsNullOrEmpty(p.AlarmText) || p.AddTagDescription) && p.IsValid
                          select p.ComposedAlarmText).ToList());

            return ret;
        }

        internal List<String> GetWholeStrings(UFUAModel.UFUATagPrototype prototype)
        {
            var ret = new List<String>();

            ret.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                          where !String.IsNullOrEmpty(p.AlarmText) && p.IsValid && p.UFUATagAss.IsMemberOf(prototype)
                          select p.AlarmText).ToList());

            ret.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                          where (String.IsNullOrEmpty(p.AlarmText) || p.AddTagDescription) && p.IsValid && p.UFUATagAss.IsMemberOf(prototype)
                          select p.ComposedAlarmText).ToList());

            return ret;
        }

        internal List<String> GetWholeStrings(UFUAModel.UFUATag tag)
        {
            var ret = new List<String>();

            ret.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                          where !String.IsNullOrEmpty(p.AlarmText) && p.IsValid && p.UFUATagAss == tag
                          select p.AlarmText).ToList());

            ret.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                          where (String.IsNullOrEmpty(p.AlarmText) || p.AddTagDescription) && p.IsValid && p.UFUATagAss == tag
                          select p.ComposedAlarmText).ToList());

            return ret;
        }

        internal List<String> GetWholeStrings(UFUAModel.UFUAFolder folder)
        {
            var ret = new List<String>();


            ret.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                          where !String.IsNullOrEmpty(p.AlarmText) && p.IsValid && p.UFUATagAss.IsMemberOf(folder)
                          select p.AlarmText).ToList());

            ret.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                          where String.IsNullOrEmpty(p.AlarmText) && p.IsValid && p.UFUATagAss.IsMemberOf(folder)
                          select p.ComposedAlarmText).ToList());

            return ret;
        }

        internal List<String> GetWholeStrings(UFUAModel.UFUAAlarmThreshold threshold)
        {
            var ret = new List<String>();

            if (!String.IsNullOrEmpty(threshold.AlarmText) && !(threshold.AddTagDescription))
                ret.Add(threshold.AlarmText);
            else
                ret.Add(threshold.ComposedAlarmText);

            return ret;
        }
        internal IList<UFUAModel.UFUAAlarmThreshold> GetThresholdsList()
        {
            return (from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                    select p).ToList();
        }

        internal IList<UFUAModel.UFUAAlarmThreshold> GetThresholdsList(UFUAModel.UFUAArea area)
        {
            var ret = new List<UFUAModel.UFUAAlarmThreshold>();
            foreach (var ufuaarea in area.UFUAAreas)
                ret.AddRange(GetThresholdsList(ufuaarea));
            foreach (var ufuasource in area.UFUAAlarmSources)
                ret.AddRange(GetThresholdsList(ufuasource));

            return ret;
        }

        internal IList<UFUAModel.UFUAAlarmThreshold> GetThresholdsList(UFUAModel.UFUAAlarmSource source)
        {
            var ret = new List<UFUAModel.UFUAAlarmThreshold>();
            foreach (var alarmdef in source.UFUAAlarmDefinitions)
                ret.AddRange(alarmdef.UFUAAlarmThresholds);

            return ret;
        }
#endif
        #endregion

        #region Views
#if !NET_STANDARD
        internal IList<UFUAModel.UFUAView> GetViewsList()
        {
            return (from p in new XPQuery<UFUAModel.UFUAView>(uow, true).AsParallel()
                    orderby p.Name
                    select p).ToList();
        }

        internal UFUAModel.UFUAView FindViewByNodeId(String nodeId)
        {
            var list = (from view in new XPQuery<UFUAModel.UFUAView>(uow, true).AsParallel()
                        where view.NodeId.ToString() == nodeId
                        select view).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal UFUAModel.UFUAView GetView(String name)
        {
            var list = (from hs in new XPQuery<UFUAModel.UFUAView>(uow, true).AsParallel()
                        where hs.Name == name
                        select hs).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }

        internal IList<UFUAModel.UFUATag> GetViewTags(UFUAModel.UFUAView view)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true)/*.AsParallel()*/
                    where tag.UFUAViews.Contains(view)
                    orderby tag.Name
                    select tag).ToList().AsParallel().Where(tag =>
                    !tag.IsSubPrototypeMember || !tag.UseShared.Value).ToList();
        }

        internal IList<String> GetViewsNameList()
        {
            return (from hs in new XPQuery<UFUAModel.UFUAView>(uow, true).AsParallel()
                    orderby hs.Name
                    select hs.Name).ToList();
        }

        String NewViewName(String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultViewName;

            if (listname == null)
                listname = GetViewsNameList();

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool ViewNameExists(String name)
        {
            return (from hs in new XPQuery<UFUAModel.UFUAView>(uow, true).AsParallel()
                    where hs.Name == name
                    select hs).ToList().Count > 0;
        }

        internal UFUAModel.UFUAView AddNewView()
        {
            var hs = new UFUAModel.UFUAView(uow) { Name = NewViewName(), NodeId = Guid.NewGuid() };
            return hs;
        }

        internal bool IsViewNameUsed(String name)
        {
            return (from tag in new XPQuery<UFUAModel.UFUAView>(uow, true)/*.AsParallel()*/
                    where tag.Name == name && tag.UFUATags.Count > 0
                    select tag).ToList().Count > 0;
        }

        internal void AssignViews(UFUAModel.UFUATag tag, string views)
        {
            if (string.IsNullOrEmpty(views))
            {
                (from v in new XPQuery<UFUAModel.UFUAView>(uow, true)//.AsParallel() Cross-thread operation detected.
                 where v.UFUATags.Contains(tag)
                 select v).ToList().ForEach(x => x.UFUATags.Remove(tag));

                return;
            }

            var viewlist = views.Split('|').ToList();

            (from v in new XPQuery<UFUAModel.UFUAView>(uow, true)//.AsParallel() Cross-thread operation detected.
             where v.UFUATags.Contains(tag) && !viewlist.Contains(v.Name)
             select v).ToList().ForEach(x => x.UFUATags.Remove(tag));

            if (string.IsNullOrEmpty(views))
                return;

            viewlist.ForEach(p =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(p))
                    {
                        var view = (from v in new XPQuery<UFUAModel.UFUAView>(uow, true).AsParallel()
                                    where v.Name == p
                                    select v).FirstOrDefault();
                        if (view == null)
                        {
                            view = new UFUAModel.UFUAView(uow) { Name = p, NodeId = Guid.NewGuid() };
                        }

                        if (!view.UFUATags.Contains(tag))
                        {
                            view.UFUATags.Add(tag);
                            view.NotifyPropertyChanged("UFUATags");
                        }
                    }
                }
                catch (Exception)
                {
                }

            });
        }
#endif
        #endregion

        #region Historical Settings
        internal IList<UFUAModel.UFUAHistorianSettings> GetHistoricalSettings()
        {
            return (from p in new XPQuery<UFUAModel.UFUAHistorianSettings>(uow, true).AsParallel()
                    orderby p.Name
                    select p).ToList();
        }

        internal UFUAModel.UFUAHistorianSettings GetHistoricalSetting(string name)
        {
            return (from p in new XPQuery<UFUAModel.UFUAHistorianSettings>(uow, true).AsParallel()
                    where p.Name == name
                    select p).FirstOrDefault();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<String> GetHistoricalSettingsNameList(bool inExecution)
        {
            if (inExecution)
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    return (from p in new XPQuery<UFUAModel.UFUAHistorianSettings>(task.UnitOfWork, true).AsParallel()
                            select p.Name).ToList();
                }
            }
            else
            {
                return (from p in new XPQuery<UFUAModel.UFUAHistorianSettings>(uow, true).AsParallel()
                        select p.Name).ToList();
            }
        }

        internal IList<String> GetAuditTraceTagNameList(bool inExecution)
        {
            if (inExecution)
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    return (from p in new XPQuery<UFUAModel.UFUATag>(task.UnitOfWork, true).AsParallel()
                            where p.AuditTraceEnabled
                            select p.GetFullName()).ToList();
                }
            }
            else
            {
                return (from p in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where p.AuditTraceEnabled
                        select p.GetFullName()).ToList();
            }
        }

        internal String NewHistoricalSettingsName(String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultHistoricalSettingsName;

            if (listname == null)
                listname = GetHistoricalSettingsNameList(inExecution: false);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        internal bool HistoricalSettingsNameExists(String name)
        {
            return (from hs in new XPQuery<UFUAModel.UFUAHistorianSettings>(uow, true).AsParallel()
                    where hs.Name == name
                    select hs).ToList().Count > 0;
        }

        public UFUAModel.UFUAHistorianSettings AddNewHistoricalSettings()
        {
            return new UFUAModel.UFUAHistorianSettings(uow) { Name = NewHistoricalSettingsName() };
        }

#if !NET_STANDARD
        internal bool IsHistoricalSettingsNameUsed(String name)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    where tag.HistorianSettings == name
                    select tag).ToList().Count > 0;
        }
#endif

        public void RemoveHistoricalSettingsFromTags(String name)
        {
            var tags = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.HistorianSettings == name
                        select tag).ToList();
            if (tags.Count > 0)
            {
                foreach (var tag in tags)
                    tag.HistorianSettings = string.Empty;
            }
        }

#if !NET_STANDARD
        public void RemoveEngineeringUnitsFromTags(String name)
        {
            var tags = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.UFUAEngineeringUnit == name
                        select tag).ToList();
            if (tags.Count > 0)
            {
                foreach (var tag in tags)
                    tag.UFUAEngineeringUnit = null;
            }
        }

        public void RemoveViewFromTags(UFUAModel.UFUAView view)
        {
            var tags = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true)/*.AsParallel()*/
                        where tag.UFUAViews.Contains(view)
                        select tag).ToList();
            if (tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    tag.UFUAViews.Remove(view);
                    tag.NotifyPropertyChanged("UFUAViews");
                    tag.NotifyPropertyChanged("Views");
                    view.UFUATags.Remove(tag);
                    view.NotifyPropertyChanged("UFUATags");
                }
            }
        }

        internal IList<UFUAModel.UFUATag> GetHistoricalSettingsTags(String name)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    where tag.HistorianSettings == name
                    orderby tag.Name
                    select tag).ToList().AsParallel().Where(tag => 
                    !tag.IsSubPrototypeMember || !tag.UseShared.Value).ToList();
        }

        internal void HistoricalSettingsNameReplace(String oldName, String newName)
        {
            var list = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.HistorianSettings == oldName
                        select tag).ToList();
            list.ForEach((tag) => tag.HistorianSettings = newName);
        }
#endif

        public UFUAModel.UFUAHistorianSettings GetHistoricalSettings(String name, bool inExecution = false)
        {
            if (inExecution)
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from hs in new XPQuery<UFUAModel.UFUAHistorianSettings>(task.UnitOfWork, true).AsParallel()
                                where hs.Name == name
                                select hs).ToList();
                    if (list.Count > 0)
                        return list[0];
                }
            }
            else
            {
                var list = (from hs in new XPQuery<UFUAModel.UFUAHistorianSettings>(uow, true).AsParallel()
                            where hs.Name == name
                            select hs).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            return null;
        }

#if !NET_STANDARD
        private void AssignHistorian(UFUAModel.UFUATag tag, string historian)
        {
            try
            {
                if (!string.IsNullOrEmpty(historian))
                {
                    var view = (from v in new XPQuery<UFUAModel.UFUAHistorianSettings>(uow, true).AsParallel()
                                where v.Name == historian
                                select v).FirstOrDefault();
                    if (view == null)
                        view = new UFUAModel.UFUAHistorianSettings(uow) { Name = historian };

                    tag.HistorianSettings = historian;
                }
                else
                    tag.HistorianSettings = string.Empty;
            }
            catch (Exception)
            {
            }

        }
#endif
        #endregion

        #region Data Loggers
        internal IList<List<string>> GetDataLoggerSettingsList()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                return (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                        orderby p.Name
                        select new List<string>() { p.Name, p.TableName, p.UtcTimeColumnName, p.LocalTimeColumnName, p.MillisecondsColumnName, p.UserColumnName, p.ReasonColumnName }).ToList();
            }
        }

        public DataLoggerModel.DataLoggerSettings GetDataLogger(string name, bool inExecution = false)
        {
            if (inExecution)
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                                where p.Name == name
                                select p).ToList();
                    if (list.Count > 0)
                    {
                        task.KeepUnitOfWork = true;
                        return list[0];
                    }
                }
            }
            else
            {
                var list = (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(uow, true).AsParallel()
                            where p.Name == name
                            select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            return null;
        }

#if !NET_STANDARD
        internal IList<DataLoggerModel.DataLoggerSettings> GetDataLoggerSettings()
        {
            return (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(uow, true).AsParallel()
                    orderby p.Name
                    select p).ToList();
        }

        internal OPCUAEntityReference GetDataLoggerColumnReference(String rootName, String columnName)
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                DataLoggerModel.DataLoggerSettings dlr = (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                                                          where p.Name == rootName
                                                          select p).FirstOrDefault();
                UFUAModel.TagEntityReference ColumnTag = (from p in new XPQuery<DataLoggerModel.DataLoggerColumn>(task.UnitOfWork, true).AsParallel()
                                                          where p.DataLoggerReference == dlr && p.Name == columnName
                                                          select p.ColumnTag).FirstOrDefault();
                if (ColumnTag != null && !ColumnTag.IsEmpty())
                {
                    NodeId nodeid = null;
                    if (!NodeId.IsNull(ColumnTag.NodeId))
                        nodeid = ColumnTag.NodeId;
                    else
                        nodeid = new Opc.Ua.NodeId(ColumnTag.Guid, TagPathHelper.GetNS());

                    var applicationName = GetAplicationName();
                    return new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                                    ColumnTag.Name, nodeid, ColumnTag.HumanReadable, null, ColumnTag.Name);
                }
            }

            return null;
        }
#endif

        internal String GetHistorianName(object resolvednodeid)
        {
            if (resolvednodeid is NodeId)
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    try
                    {
                        UFUAModel.UFUATag member;
                        var tagfound = FindTagByNodeId(resolvednodeid as NodeId, task.UnitOfWork, out member);
                        if (member != null)
                            return member.HistorianSettings;
                        else if (tagfound != null)
                            return tagfound.HistorianSettings;
                    }
                    catch
                    { }
                }
            }

            return string.Empty;
        }

        internal IList<string> GetDataLoggerColumnList(string rootName)
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                DataLoggerModel.DataLoggerSettings dlr = (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                                                          where p.Name == rootName
                                                          select p).FirstOrDefault();
                return (from p in new XPQuery<DataLoggerModel.DataLoggerColumn>(task.UnitOfWork, true).AsParallel()
                        where p.DataLoggerReference == dlr
                        orderby p.ColumnName
                        select p.Name).ToList();
            }
        }

        internal bool UsesAggreagatedTables(string rootName)
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                DataLoggerModel.DataLoggerSettings dlr = (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                                                          where p.Name == rootName
                                                          select p).FirstOrDefault();
                return dlr != null ? dlr.UseAggregatedTables : false;
            }
        }

        internal IList<List<string>> GetDataLoggerColumnSettingList(string rootName)
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                DataLoggerModel.DataLoggerSettings dlr = (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                                                          where p.Name == rootName
                                                          select p).FirstOrDefault();
                var list = (from p in new XPQuery<DataLoggerModel.DataLoggerColumn>(task.UnitOfWork, true)/*.AsParallel()*/
                            where p.DataLoggerReference == dlr
                            select p).ToList();
                List<List<string>> ret = new List<List<string>>();
                list.ForEach(p =>
                {
                    ret.Add(new List<string>() { p.Name, p.SourceTimeStampSuffixColumnName, p.AddSourceTimeStampColumn.ToString(), p.ColumnTag?.Name, p.ColumnTag?.Guid.ToString() });
                });

                return ret;
            }
        }

#if !NET_STANDARD
        internal IList<DataLoggerModel.DataLoggerColumn> GetDataLoggerColumns(DataLoggerModel.DataLoggerSettings root = null)
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                return (from p in new XPQuery<DataLoggerModel.DataLoggerColumn>(task.UnitOfWork, true).AsParallel()
                        where p.DataLoggerReference == root
                        orderby p.ColumnName
                        select p).ToList();
            }
        }

        internal IList<DataLoggerModel.DataLoggerColumn> GetDataLoggerColumnSettings(DataLoggerModel.DataLoggerSettings root = null, bool allSettings = false)
        {
            return (from p in new XPQuery<DataLoggerModel.DataLoggerColumn>(uow, true).AsParallel()
                    where !allSettings && p.DataLoggerReference == root || allSettings
                    orderby p.DataLoggerReference
                    select p).ToList();
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<String> GetDataLoggerSettingsNames()
        {
            return (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(uow, true).AsParallel()
                    select p.Name.ToString()).ToList();
        }

        internal IList<String> GetDataLoggerSettingsNameList(bool inExecution = false)
        {
            if (inExecution)
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    return (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                            where p.Name != null
                            orderby p.Name
                            select p.Name.ToString()).ToList();
                }
            }
            else
            {
                return (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(uow, true).AsParallel()
                        where p.Name != null
                        orderby p.Name
                        select p.Name.ToString()).ToList();
            }
        }

        internal String NewDataLoggerSettingsName(String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultDataLoggerSettingsName;

            if (listname == null)
                listname = GetDataLoggerSettingsNameList();

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool DataLoggerSettingsNameExists(String name)
        {
            return (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(uow, true).AsParallel()
                    where p.Name == name
                    select p).ToList().Count > 0;
        }

        internal IList<String> GetDataLoggerColumnNameList(DataLoggerModel.DataLoggerSettings root)
        {
            if (root == null)
            {
                return (from p in new XPQuery<DataLoggerModel.DataLoggerColumn>(uow, true).AsParallel()
                        where p.DataLoggerReference == null && p.ColumnName != null
                        select p.ColumnName.ToString()).ToList();
            }

            return (from c in root.Columns.AsParallel() where c.ColumnName != null select c.ColumnName).ToList();
        }

        internal String NewDataLoggerColumnName(DataLoggerModel.DataLoggerSettings root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}", bool bRemoveEndsNumbers = true)
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultDataLoggerColumnName;

            if (listname == null)
                listname = GetDataLoggerColumnNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format, bRemoveEndsNumbers);
        }

        bool DataLoggerColumnNameExists(String name, DataLoggerModel.DataLoggerSettings root)
        {
            if (root == null)
            {
                return (from p in new XPQuery<DataLoggerModel.DataLoggerColumn>(uow, true).AsParallel()
                        where p.DataLoggerReference == null && p.ColumnName == name
                        select p).ToList().Count > 0;
            }

            return (from c in root.Columns.AsParallel() where c.ColumnName == name select c).ToList().Count > 0;
        }

        public DataLoggerModel.DataLoggerSettings AddNewDataLoggerSettings()
        {
            return new DataLoggerModel.DataLoggerSettings(uow) { Name = NewDataLoggerSettingsName() };
        }

        public DataLoggerModel.DataLoggerColumn AddNewDataLoggerColumn(DataLoggerModel.DataLoggerSettings root)
        {
            var column = new DataLoggerModel.DataLoggerColumn(uow) { ColumnName = NewDataLoggerColumnName(root) };
            if (root != null)
                root.Columns.Add(column);
            return column;
        }
        #endregion

        #region EngineeringUnit
#if !NET_STANDARD
        internal IList<String> GetEngineeringUnitNames()
        {
            return (from p in new XPQuery<UFUAModel.UFUAEngineeringUnit>(uow, true).AsParallel()
                    orderby p.Name
                    select p.Name).ToList();
        }

        internal IList<UFUAModel.UFUAEngineeringUnit> GetEngineeringUnits()
        {
            return (from p in new XPQuery<UFUAModel.UFUAEngineeringUnit>(uow, true).AsParallel()
                    orderby p.Name
                    select p).ToList();
        }
#endif

        public string GetTagEngineeringUnit(string guid)
        {
            try
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var searchGuid = new Guid(guid);
                    var tag = (from p in new XPQuery<UFUAModel.UFUATag>(task.UnitOfWork, true)/*.AsParallel()*/
                               where p.NodeId == searchGuid
                               select p).FirstOrDefault();
                    UFUAModel.UFUAEngineeringUnit eu = GetEngineeringUnit(tag.UFUAEngineeringUnit, inExecution: true);
                    return $"{eu.UnitName};{eu.EURangeLow};{eu.EURangeHigh}";
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public Dictionary<string,string> GetTagsEngineeringUnit()
        {
            try
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var eunits = (from p in new XPQuery<UFUAModel.UFUAEngineeringUnit>(task.UnitOfWork, true)/*.AsParallel()*/
                     select p).ToList();

                    Dictionary<string, string> unitMap = new Dictionary<string, string>();
                    eunits.ForEach(eu => unitMap.Add(eu.Name, $"{eu.UnitName};{eu.EURangeLow};{eu.EURangeHigh}"));

                    var tags = (from p in new XPQuery<UFUAModel.UFUATag>(task.UnitOfWork, true)/*.AsParallel()*/
                               where !string.IsNullOrEmpty(p.UFUAEngineeringUnit)
                               select p).ToList();
                    Dictionary<string, string> tagMap = new Dictionary<string, string>();
                    tags.ForEach(tag =>
                    {
                        if(unitMap.ContainsKey(tag.UFUAEngineeringUnit))
                            tagMap.Add(tag.NodeId.ToString(), unitMap[tag.UFUAEngineeringUnit]);
                    });
                    return tagMap;
                }
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }

        public UFUAModel.UFUAEngineeringUnit GetEngineeringUnit(string euname, bool inExecution = false)
        {
            if (inExecution)
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    return (from p in new XPQuery<UFUAModel.UFUAEngineeringUnit>(task.UnitOfWork, true)/*.AsParallel()*/
                            where p.Name == euname
                            select p).FirstOrDefault();
                }
            }
            else
            {
                return (from p in new XPQuery<UFUAModel.UFUAEngineeringUnit>(uow, true).AsParallel()
                        where p.Name == euname
                        select p).FirstOrDefault();
            }
        }

        IList<String> GetEngineeringUnitsNameList()
        {
            return (from p in new XPQuery<UFUAModel.UFUAEngineeringUnit>(uow, true).AsParallel()
                    select p.Name).ToList();
        }

        internal String NewEngineeringUnitsName(String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultEngineeringUnitsName;

            if (listname == null)
                listname = GetEngineeringUnitsNameList();

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool EngineeringUnitsNameExists(String name)
        {
            return (from hs in new XPQuery<UFUAModel.UFUAEngineeringUnit>(uow, true).AsParallel()
                    where hs.Name == name
                    select hs).ToList().Count > 0;
        }

        public UFUAModel.UFUAEngineeringUnit AddNewEngineeringUnits()
        {
            var hs = new UFUAModel.UFUAEngineeringUnit(uow) { Name = NewEngineeringUnitsName() };
            return hs;
        }

#if !NET_STANDARD
        internal bool IsEngineeringUnitsNameUsed(String name)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    where tag.UFUAEngineeringUnit == name
                    select tag).ToList().Count > 0;
        }

        internal IList<UFUAModel.UFUATag> GetEngineeringUnitsTags(String name)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    where tag.UFUAEngineeringUnit == name
                    orderby tag.Name
                    select tag).ToList().AsParallel().Where(tag =>
                    !tag.IsSubPrototypeMember || !tag.UseShared.Value).ToList();
        }

        internal void EngineeringUnitsNameReplace(String oldName, String newName)
        {
            var list = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.UFUAEngineeringUnit == oldName
                        select tag).ToList();
            list.ForEach((tag) => tag.UFUAEngineeringUnit = newName);
        }

        internal UFUAModel.UFUAEngineeringUnit GetEngineeringUnits(String name)
        {
            var list = (from hs in new XPQuery<UFUAModel.UFUAEngineeringUnit>(uow, true).AsParallel()
                        where hs.Name == name
                        select hs).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }
#endif
        #endregion

        #region Folder
        internal IList<String> GetFoldersNameList(UFUAModel.UFUAFolder root)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                        where folder.UFUAFolderAss == null && folder.UFUATagPrototype == null
                        select folder.Name).ToList();
            }

            return (from c in root.UFUAFolders.AsParallel() select c.Name).ToList();
        }

        internal IList<String> GetFoldersNameList(UFUAModel.UFUATagPrototype prototype)
        {
            return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                    where folder.UFUATagPrototype == prototype
                    select folder.Name).ToList();
        }

        internal String NewFolderName(UFUAModel.UFUAFolder root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultFolderName;

            if (listname == null)
                listname = GetFoldersNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        public UFUAModel.UFUAFolder GetFolder(string name, UFUAModel.UFUAFolder folder = null)
        {
            List<UFUAModel.UFUAFolder> list;
            if (folder == null)
            {
                list = (from f in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                        where f.UFUAFolderAss == null && f.UFUATagPrototype == null && f.Name == name
                        select f).ToList();
            }
            else
                list = (from c in folder.UFUAFolders.AsParallel() where c.Name == name select c).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }
        bool FolderNameExists(String name, UFUAModel.UFUAFolder root)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                        where folder.UFUAFolderAss == null && folder.UFUATagPrototype == null && folder.Name == name
                        select folder).ToList().Count > 0;
            }

            return (from c in root.UFUAFolders.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }

        internal String NewFolderName(UFUAModel.UFUATagPrototype prototype, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultFolderName;

            if (listname == null)
                listname = GetFoldersNameList(prototype);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        //bool FolderNameExists(String name, UFUAModel.UFUATagPrototype prototype)
        //{
        //    return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
        //            where folder.UFUATagPrototype == prototype && folder.Name == name
        //            select folder).ToList().Count > 0;
        //}

#if !NET_STANDARD
        internal UFUAModel.UFUAFolder AddNewFolder(string path, UnitOfWork uow, UFUAModel.UFUATagPrototype prototype, List<IXPSimpleObject> addedObjects)
        {
            UFUAModel.UFUAFolder _lfolder;

            if (prototype != null)
                _lfolder = (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                            where folder.GetRelativeName() == path && folder.PrototypeReference == prototype
                            select folder).FirstOrDefault();
            else
                _lfolder = (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                            where folder.GetRelativeName() == path && folder.PrototypeReference == null
                            select folder).FirstOrDefault();

            if (_lfolder != null)
                return _lfolder;

            var parent = System.IO.Path.GetDirectoryName(path);
            UFUAModel.UFUAFolder parentfolder = null;
            if (!string.IsNullOrEmpty(parent))
                parentfolder = AddNewFolder(parent.Replace("\\", "/"), uow, prototype, addedObjects);

            var foldername = System.IO.Path.GetFileName(path);
            if (parentfolder == null && prototype != null)
            {
                var _folder = new UFUAModel.UFUAFolder(uow) { Name = NewFolderName(prototype, foldername), NodeId = Guid.NewGuid(), MemberOrderId = -1, UFUATagPrototype = prototype };
                if (_folder is IXPSimpleObject)
                    addedObjects.Add(_folder as IXPSimpleObject);
                return _folder;
            }
            else
            {
                var _folder = new UFUAModel.UFUAFolder(uow) { Name = NewFolderName(parentfolder, foldername), NodeId = Guid.NewGuid(), MemberOrderId = -1 };
                if (_folder is IXPSimpleObject)
                    addedObjects.Add(_folder as IXPSimpleObject);
                if (parentfolder != null)
                    parentfolder.UFUAFolders.Add(_folder);
                return _folder;
            }

        }
#endif

        public UFUAModel.UFUAFolder AddNewFolder(UFUAModel.UFUAFolder root, int orderid = -1)
        {
            var folder = new UFUAModel.UFUAFolder(uow) { Name = NewFolderName(root), NodeId = Guid.NewGuid(), MemberOrderId = orderid };
            if (root != null)
                root.UFUAFolders.Add(folder);
            return folder;
        }

        internal UFUAModel.UFUAFolder AddNewFolder(UFUAModel.UFUATagPrototype prototype, int orderid = -1)
        {
            var folder = new UFUAModel.UFUAFolder(uow) { Name = NewFolderName(prototype), NodeId = Guid.NewGuid(), MemberOrderId = orderid };
            if (prototype != null)
                prototype.Folders.Add(folder);
            return folder;
        }

        internal IList<object> GetGenericFolderCollection(object root = null)
        {
            return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                    where folder.UFUAFolderAss == (UFUAModel.UFUAFolder)root &&
                    ((UFUAModel.UFUAFolder)root != null || folder.UFUATagPrototype == null)
                    orderby folder.Name ascending
                    select (object)folder).ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<UFUAModel.UFUAFolder> GetFolderCollection(UFUAModel.UFUAFolder root = null, bool sortByName = false)
        {
            if (root != null && root.IsPrototypeMember)
            {
                if (sortByName)
                {
                    return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                            where folder.UFUAFolderAss == root &&
                            (root != null || folder.UFUATagPrototype == null)
                            orderby folder.Name ascending
                            select folder).ToList();
                }
                else
                {
                    return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                            where folder.UFUAFolderAss == root &&
                            (root != null || folder.UFUATagPrototype == null)
                            orderby folder.MemberOrderId ascending
                            select folder).ToList();
                }
            }

            return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                    where folder.UFUAFolderAss == root &&
                    (root != null || folder.UFUATagPrototype == null)
                    orderby folder.Name ascending
                    select folder).ToList();
        }

#if !NET_STANDARD
        internal UFUAModel.UFUAFolder FindFolderByNodeId(Guid guid)
        {
            return FindFolderByNodeId(guid, null);
        }

        internal UFUAModel.UFUAFolder FindFolderByNodeId(Guid guid, Guid prototypeId)
        {
            var prototype = (from item in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                             where item.NodeId == prototypeId && item.UFUATagOwner == null
                             select item).FirstOrDefault();

            return FindFolderByNodeId(guid, prototype);
        }

        UFUAModel.UFUAFolder FindFolderByNodeId(Guid guid, UFUAModel.UFUATagPrototype prototype = null)
        {
            return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                    where folder.NodeId == guid && folder.PrototypeReference == prototype
                    select folder).FirstOrDefault();
        }

        internal UFUAModel.UFUAFolder FindSubPrototypeFolderByNodeId(Guid guid, Guid prototypeId, Guid ownerId)
        {
            var prototype = (from item in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                             where item.NodeId == prototypeId && item.UFUATagOwner != null && item.UFUATagOwner.NodeId == ownerId
                             select item).FirstOrDefault();

            if (prototype == null)
                return null;
            return FindFolderByNodeId(guid, prototype);
        }

        internal List<UFUAModel.UFUAFolder> FindSubPrototypeFoldersByNodeId(Guid guid)
        {
            return (from tag in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                    where tag.NodeId == guid 
                    select tag).ToList().AsParallel().Where(tag => tag.IsSubPrototypeMember).ToList();
        }
#endif
        #endregion

        #region BaseAddresses
        public bool RemoveBaseAddress(string transport)
        {
            var list = (from b in GetConfiguration().BaseAddresses
                        where b.Transport == transport
                        select b).ToList();
            if (list.Count > 0)
            {
                while(list.Count > 0)
                {
                    var ba = list[0] as UFUABaseAddress;
                    if (ba != null)
                        ba.Delete();
                    list.RemoveAt(0);
                }
                return true;
            }
            
            return false;
        }
        public UFUAModel.UFUABaseAddress AddNewBaseAddress(string transport)
        {
            var ba = new UFUAModel.UFUABaseAddress(uow)
                {
                    Enabled = true,
                    Transport = transport,
                    Server = "localhost",
                    Port = GetConfiguration().GetDefaultPort(transport)
                };

            GetConfiguration().BaseAddresses.Add(ba);

            return ba;
        }
        public List<UFUABaseAddress> GetBaseAddressList()
        {
            return (from address in new XPQuery<UFUAModel.UFUABaseAddress>(uow, true).AsParallel()
                        select address).ToList();
        }
        public UFUAModel.UFUABaseAddress GetBaseAddress(string transport)
        {
            var list = (from address in new XPQuery<UFUAModel.UFUABaseAddress>(uow, true).AsParallel()
                        where address.Transport == transport
                        select address).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }

#if !NET_STANDARD
        private bool AddHttpAccessRules(bool silent)
        {
            var urls = (from address in new XPQuery<UFUAModel.UFUABaseAddress>(uow, true).AsParallel()
                        select address.Path).ToArray();
            try
            {
                var httpAccessRules = new HTTPAccessRules(urls);
                httpAccessRules.CheckUrlsAndAddAccessRules();
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    var message = Properties.Resources.HttpRegistrationFailed;
                    message = message.Replace("'newline'", Environment.NewLine);
                    message = String.Format(message, ex.Message);
                    if (editorManagerComponent.UIInterface != null)
                    {
                        return editorManagerComponent.UIInterface.ShowYesNo(message, CustomDialogIcons.Warning) == CustomDialogResults.Yes;
                    }
                    else
                    {
                        return MessageBox.Show(message, Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                    }
                }

                return false;
            }

            return true;
        }
#endif
        #endregion

        #region Drivers
        public UFUAModel.UFUACommunicationDriver AddNewDriver()
        {
            return new UFUAModel.UFUACommunicationDriver(uow);
        }

        public bool IsDriverNameUsed(String name)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    where tag.DynamicSettings != null && tag.DynamicSettings.ToLower().Contains(String.Format("{0}.", name.ToLower()))
                    select tag).ToList().Count > 0;
        }

#if !NET_STANDARD
        internal IList<UFUAModel.UFUACommunicationDriver> GetDrivers()
        {
            return (from p in new XPQuery<UFUAModel.UFUACommunicationDriver>(uow, true).AsParallel()
                    orderby p.FriendlyName
                    select p).ToList();
        }

        IList<String> GetDriversNameList()
        {
            return (from hs in new XPQuery<UFUAModel.UFUACommunicationDriver>(uow, true).AsParallel()
                    select hs.Name).ToList();
        }

        internal static void EditDriverSettings(UFUAModel.UFUACommunicationDriver driver, UFUAServerDocument doc)
        {
            if (driver == null)
                return;

            var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(String.Format("{0}\\{1}", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), driver.AssemblyName));

            DriverSettingsInterfaces.ICommunicationDriverWpfEditing driverWpfEditing = null;
            try
            {
                var types = Assembly.LoadFile(uidll).GetTypes();
                var list = (from t in types/*.AsParallel()*/
                            where !t.IsAbstract && typeof(DriverSettingsInterfaces.ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                            select (DriverSettingsInterfaces.ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).ToList();
                driverWpfEditing = list[0];

                if (driverWpfEditing == null || driverWpfEditing.GeneralSettingsEditor == null)
                    return;
            }
            catch (Exception ex)
            {
                if (doc.EditorManagerComponent.UIInterface != null)
                    doc.EditorManagerComponent.UIInterface.ShowInformation(String.Format(Properties.Resources.CommDriverNotFound, uidll));
                return;
            }

            UserControl control = null;
            try
            {
                if (doc.EditorManagerComponent.Workspace != null)
                    doc.EditorManagerComponent.Workspace.ResetBusy();

                control = driverWpfEditing.GeneralSettingsEditor;
                var settingsContext = new ComunicationSettingsContext2()
                {
                    ConnectionString = doc.ConnectionString,
                    bProtected = doc.Protected,
                    protectionCode = doc.Id,
                    listTag = doc.GetFlatFullTagNameNodeIdCollection()
                };
                control.DataContext = settingsContext;
                //List<string> lista = new List<string>() { doc.ConnectionString, doc.Protected.ToString() };
                //control.DataContext = lista;// Document.ConnectionString;
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = driver.FriendlyName,
                    Owner = Application.Current.Windows.Count > 0 ? Application.Current.Windows[0] : Application.Current.MainWindow
                };
                using (new ResetCursor())
                {
                    if (Dialog.ShowDialog() == true)
                    {
                        driverWpfEditing.SaveSettings(control);
                        doc.InvalidateDynamicSettings(driver);
                    }
                }
            }
            catch (DriverBaseInterfaces.ValidatingDocumentException ex)
            {
                var uiMsgBox = doc.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, ex.FilePath));
            }
            finally
            {
                if (control is IDisposable)
                    (control as IDisposable).Dispose();

                if (doc.EditorManagerComponent.Workspace != null)
                    doc.EditorManagerComponent.Workspace.RestoreBusy();
            }
        }

        void InvalidateAllDriversDynamicSettings()
        {
            if (!bInvalidateDriversDynamicSettingsOnSave)
                return;

            bInvalidateDriversDynamicSettingsOnSave = false;
            var driverlist = GetConfiguration().ComunicationDrivers;
            if (driverlist.Count > 0)
            {
                foreach (var driver in driverlist)
                    InvalidateDynamicSettings(driver);
            }
        }

        void InvalidateDynamicSettings(UFUACommunicationDriver driver)
        {
            var drvDll = String.Format("{0}\\{1}", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), driver.AssemblyName);

            try
            {
                var types = Assembly.LoadFile(drvDll).GetTypes();
                var list = (from t in types.AsParallel()
                            where !t.IsAbstract && typeof(DriverBaseInterfaces.IInvalidateDynJobs).IsAssignableFrom(t)
                            select (DriverBaseInterfaces.IInvalidateDynJobs)Activator.CreateInstance(t)).ToList();

                if (list.Count > 0)
                    list[0].InvalidateDynamicSettings(ConnectionString);
            }
            catch (Exception ex)
            {
                logGeneral.ErrorFormat(Properties.Resources.CommDriverNotFound, drvDll);
            }
        }
#endif
        #endregion

        #region Alarms
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<UFUAModel.UFUAAlarmDefinition> GetAlarmDefinitions()
        {
            return (from p in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uow, true).AsParallel()
                    orderby p.Name
                    select p).ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<UFUAModel.UFUAAlarmDefinition> GetAlarmDefinitions(UFUAModel.UFUAAlarmSource root)
        {
            return (from p in root.UFUAAlarmDefinitions.AsParallel()
                    orderby p.Name
                    select p).ToList();
        }

        internal IList<UFUAModel.UFUAAlarmThreshold> GetAlarmThresholds(bool inExecution = true)
        {
            if (!inExecution)
                return (from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                        where p.IsValid
                        orderby p.Name
                        select p).ToList();

            else
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    return (from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(task.UnitOfWork, true).AsParallel()
                            where p.IsValid
                            orderby p.Name
                            select p).ToList();
                }
        }

        internal IDictionary<String, String> GetAlarmCommandsOnDbClick()
        {
            return (from c in GetAlarmThresholds()
                    where !string.IsNullOrEmpty(c.CommandsDbClick)
                    select c).ToDictionary(c => c.GetUniqueConditionName(), c => c.CommandsDbClick);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<UFUAModel.UFUAArea> GetAlarmAreas(UFUAModel.UFUAArea root = null)
        {
            return (from p in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                    where p.UFUAAreaAss == root
                    orderby p.Name
                    select p).ToList();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<UFUAModel.UFUAAlarmSource> GetSourceCollection(UFUAModel.UFUAArea root = null)
        {
            if (root == null)
            {
                return (from p in new XPQuery<UFUAModel.UFUAAlarmSource>(uow, true).AsParallel()
                        where p.UFUAArea == null
                        orderby p.Name
                        select p).ToList();
            }

            var areas = (from area in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                         //where folder.Oid == root.Oid
                         where area.NodeId == root.NodeId
                         select area).ToList();
            if (areas.Count == 0)
                return new List<UFUAModel.UFUAAlarmSource>();
            else
                return (from c in areas[0].UFUAAlarmSources.AsParallel() orderby c.Name select c).ToList();
        }

        internal IList<String> GetAlarmDefinitionsNameList(UFUAModel.UFUAAlarmSource root)
        {
            if (root == null)
            {
                return (from alarmPrototype in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uow, true).AsParallel()
                        where alarmPrototype.UFUAAlarmDefinitions == null
                        select alarmPrototype.Name).ToList();
            }

            return (from c in root.UFUAAlarmDefinitions.AsParallel() select c.Name).ToList();
        }

        IList<String> GetAlarmAreasNameList(UFUAModel.UFUAArea root)
        {
            if (root == null)
            {
                return (from area in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                        where area.UFUAAreaAss == null
                        select area.Name).ToList();
            }

            return (from c in root.UFUAAreas.AsParallel() select c.Name).ToList();
        }

        IList<String> GetAlarmSourcesNameList(UFUAModel.UFUAArea root)
        {
            if (root == null)
            {
                return (from alarmSource in new XPQuery<UFUAModel.UFUAAlarmSource>(uow, true).AsParallel()
                        where alarmSource.UFUAArea == null
                        select alarmSource.Name).ToList();
            }

            return (from c in root.UFUAAlarmSources.AsParallel() select c.Name).ToList();
        }

#if !NET_STANDARD
        internal UFUAModel.UFUAArea FindAlarmAreaByNodeId(Guid nodeId)
        {
            var list = (from area in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                        where area.NodeId == nodeId
                        select area).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal UFUAModel.UFUAAlarmSource FindAlarmSourceAreaByNodeId(Guid nodeId)
        {
            var list = (from source in new XPQuery<UFUAModel.UFUAAlarmSource>(uow, true).AsParallel()
                        where source.NodeId == nodeId
                        select source).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal UFUAModel.UFUAAlarmDefinition FindAlarmDefinitionByNodeId(Guid nodeId)
        {
            var list = (from definition in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uow, true).AsParallel()
                        where definition.NodeId == nodeId
                        select definition).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }
#endif

        internal String NewAlarmAreaName(UFUAModel.UFUAArea root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultAlarmAreaName;

            if (listname == null)
                listname = GetAlarmAreasNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool AlarmAreaNameExists(String name, UFUAModel.UFUAArea root)
        {
            if (root == null)
            {
                return (from area in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                        where area.UFUAAreaAss == null && area.Name == name
                        select area).ToList().Count > 0;
            }

            return (from c in root.UFUAAreas.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }

        internal String NewAlarmSourceName(UFUAModel.UFUAArea root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultAlarmSourceName;

            if (listname == null)
                listname = GetAlarmSourcesNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool AlarmSourceNameExists(String name, UFUAModel.UFUAArea root)
        {
            if (root == null)
            {
                return (from alarmSource in new XPQuery<UFUAModel.UFUAAlarmSource>(uow, true).AsParallel()
                        where alarmSource.UFUAArea == null && alarmSource.Name == name
                        select alarmSource).ToList().Count > 0;
            }

            return (from c in root.UFUAAlarmSources.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }

        internal String NewAlarmPrototypeName(UFUAModel.UFUAAlarmSource root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultAlarmPrototypeName;

            if (listname == null)
                listname = GetAlarmDefinitionsNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool AlarmPrototypeNameExists(String name, UFUAModel.UFUAAlarmSource root)
        {
            if (root == null)
            {
                return (from alarmPrototype in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uow, true).AsParallel()
                        where alarmPrototype.UFUAAlarmDefinitions == null && alarmPrototype.Name == name
                        select alarmPrototype).ToList().Count > 0;
            }

            return (from c in root.UFUAAlarmDefinitions.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }

        public UFUAModel.UFUAArea AddNewAlarmArea(UFUAModel.UFUAArea root = null)
        {
            var folder = new UFUAModel.UFUAArea(uow) { Name = NewAlarmAreaName(root), NodeId = Guid.NewGuid() };
            if (root != null)
                root.UFUAAreas.Add(folder);
            return folder;
        }

        internal UFUAModel.UFUAArea AddNewAlarmArea(UFUAModel.UFUAArea root = null, string name = null)
        {
            var folder = new UFUAModel.UFUAArea(uow) { Name = NewAlarmAreaName(root, name), NodeId = Guid.NewGuid() };
            if (root != null)
                root.UFUAAreas.Add(folder);
            return folder;
        }

        public UFUAModel.UFUAAlarmSource AddNewAlarmSource(UFUAModel.UFUAArea root, String name = null)
        {
            var folder = new UFUAModel.UFUAAlarmSource(uow) { Name = NewAlarmSourceName(root, name), NodeId = Guid.NewGuid() };
            if (root != null)
                root.UFUAAlarmSources.Add(folder);
            return folder;
        }

#if !NET_STANDARD
        internal UFUAModel.UFUAAlarmSource AddNewAlarmSource(string path, UnitOfWork uow, List<IXPSimpleObject> addedObjects)
        {
            UFUAModel.UFUAAlarmSource asource = (from source in new XPQuery<UFUAModel.UFUAAlarmSource>(uow, true).AsParallel()
                                                 where source.GetRelativeName() == path
                                                 select source).FirstOrDefault();
            if (asource == null)
            {
                var sourcename = System.IO.Path.GetFileName(path);
                path = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                UFUAModel.UFUAArea aarea = AddNewAlarmArea(path, uow, null);
                if (aarea is IXPSimpleObject)
                    addedObjects.Add(aarea as IXPSimpleObject);
                asource = AddNewAlarmSource(aarea, sourcename);
                if (asource is IXPSimpleObject)
                    addedObjects.Add(asource as IXPSimpleObject);
            }

            return asource;
        }
        internal DataLoggerModel.DataLoggerSettings AddNewDatalogger(string name, UnitOfWork uow)
        {
            DataLoggerModel.DataLoggerSettings dlrs = (from source in new XPQuery<DataLoggerModel.DataLoggerSettings>(uow, true).AsParallel()
                                                       where source.GetDataloggerName() == name
                                                       select source).FirstOrDefault();
            if (dlrs == null)
            {
                dlrs = AddNewDataLoggerSettings();
                dlrs.Name = name;
            }

            return dlrs;
        }
        internal UFUAModel.UFUAArea AddNewAlarmArea(string path, UnitOfWork uow, UFUAModel.UFUAArea aarea)
        {
            UFUAModel.UFUAArea _larea;

            if (aarea != null)
                _larea = (from area in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                          where area.GetRelativeName() == path && area == aarea.UFUAAreaAss
                          select area).FirstOrDefault();
            else
                _larea = (from area in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                          where area.GetRelativeName() == path
                          select area).FirstOrDefault();

            if (_larea != null)
                return _larea;

            var parent = System.IO.Path.GetDirectoryName(path);
            UFUAModel.UFUAArea parentarea = null;
            if (!string.IsNullOrEmpty(parent))
                parentarea = AddNewAlarmArea(parent, uow, aarea);

            var areaname = System.IO.Path.GetFileName(path);

            return new UFUAModel.UFUAArea(uow) { Name = NewAlarmAreaName(parentarea, areaname), NodeId = Guid.NewGuid(), UFUAAreaAss = parentarea };
        }
#endif

        public UFUAModel.UFUAAlarmDefinition GetAlarmPrototype(string name, UFUAModel.UFUAAlarmSource parent)
        {
            if (parent != null)
            {
                List<UFUAModel.UFUAAlarmDefinition> list = (from a in parent.UFUAAlarmDefinitions where a.Name == name select a).ToList();
                if (list.Count > 0)
                    return list[0];
            }
            return null;
        }
        internal UFUAModel.UFUAAlarmDefinition AddNewAlarmPrototype(UFUAModel.UFUAAlarmSource root, string name = null)
        {
            var folder = new UFUAModel.UFUAAlarmDefinition(uow) { Name = NewAlarmPrototypeName(root, name), NodeId = Guid.NewGuid() };
            if (root != null)
                root.UFUAAlarmDefinitions.Add(folder);
            return folder;
        }
        public UFUAModel.UFUAAlarmDefinition AddNewAlarmPrototype(UFUAModel.UFUAAlarmSource root)
        {
            var folder = new UFUAModel.UFUAAlarmDefinition(uow) { Name = NewAlarmPrototypeName(root), NodeId = Guid.NewGuid() };
            if (root != null)
                root.UFUAAlarmDefinitions.Add(folder);
            return folder;
        }

        internal UFUAModel.UFUAAlarmThreshold AddNewAlarmThreshold(UFUAModel.UFUAAlarmDefinition parent)
        {
            return new UFUAModel.UFUAAlarmThreshold(parent, uow);
        }

#if !NET_STANDARD
        internal bool IsAlarmDefinitionUsed(UFUAModel.UFUAAlarmDefinition alarm)
        {
            var list = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.UFUAAlarmThresholds != null
                        select tag).ToList();
            foreach (var tag in list)
            {
                var found = (from c in tag.UFUAAlarmThresholds.AsParallel()
                             where c.UFUAAlarmDefinitionRef == alarm
                             select c).ToList();
                if (found.Count > 0)
                    return true;
            }

            return false;
        }

        internal List<UFUAModel.UFUAAlarmThreshold> GetNewThresholdList(UFUAModel.UFUAAlarmDefinition hs, AssignAlarmViewModel model, UFUAModel.UFUATag tag)
        {
            return GetNewThresholdList(hs, tag, model.AssignAlarmType, (model.SetAlarmText ? model.PrefixAlarmText : ""));
        }
#endif

        public List<UFUAModel.UFUAAlarmThreshold> GetNewThresholdList(UFUAModel.UFUAAlarmDefinition hs, UFUAModel.UFUATag tag, AssignAlarmType alrType, string alrText)
        {
            var ret = new List<UFUAModel.UFUAAlarmThreshold>();
            if (hs.StatisticData == StatDef.StatProps.None && alrType == AssignAlarmType.AnyTagBit)
            {
                if (tag.ArrayDimension > 0)
                {
                    for (uint cc = 0; cc < tag.ArrayDimension; cc++)
                    {
                        for (byte ii = 0; ii < tag.GetBitsNumber(); ii++)
                        {
                            var thr = AddNewAlarmThreshold(hs);
                            thr.Expression = String.Format("[{0}].{1}", cc, ii);
                            ret.Add(thr);
                        }
                    }
                }
                else if (tag.GetBitsNumber() > 0)
                {
                    for (int ii = 0; ii < tag.GetBitsNumber(); ii++)
                    {
                        var thr = AddNewAlarmThreshold(hs);
                        thr.Expression = String.Format(".{0}", ii);
                        ret.Add(thr);
                    }
                }
                else
                {
                    var thr = AddNewAlarmThreshold(hs);
                    ret.Add(thr);
                }
            }
            else if (hs.StatisticData == StatDef.StatProps.None && alrType == AssignAlarmType.AnyTagElement)
            {
                if (tag.ArrayDimension > 0)
                {
                    for (uint cc = 0; cc < tag.ArrayDimension; cc++)
                    {
                        var thr = AddNewAlarmThreshold(hs);
                        thr.Expression = String.Format("[{0}]", cc);
                        ret.Add(thr);
                    }
                }
                else
                {
                    var thr = AddNewAlarmThreshold(hs);
                    ret.Add(thr);
                }
            }
            else
            {
                var thr = AddNewAlarmThreshold(hs);
                ret.Add(thr);
            }

            if (/*model.SetAlarmText*/string.IsNullOrEmpty(alrText))
            {
                uint counter = 0;
                ret.ForEach((alr) =>
                {
                    if (/*model.PrefixAlarmText*/alrText == null || String.IsNullOrEmpty(/*model.PrefixAlarmText*/alrText.Trim()))
                    {
                        alr.UFUATagAss = tag;
                        alr.AlarmText = alr.ComposedAlarmText;
                    }
                    else
                    {
                        alr.AlarmText = String.Format("{0}.{1}", /*model.PrefixAlarmText*/alrText, ++counter);
                    }
                });
            }

            return ret;
        }

#if !NET_STANDARD
        internal UFUAModel.UFUAAlarmDefinition GetAlarmDefinitioInSource(string adefname, UFUAModel.UFUAAlarmSource source, UnitOfWork uow)
        {
            return (from adef in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uow, true).AsParallel()
                    where adef.UFUAAlarmDefinitions == source && adef.Name == adefname
                    select adef).FirstOrDefault();
        }

        internal UFUAModel.UFUAAlarmDefinition GetAlarmDefinition(Guid nodeid, UnitOfWork uow)
        {
            return (from adef in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uow, true).AsParallel()
                    where adef.NodeId == nodeid
                    select adef).FirstOrDefault();
        }

        bool CleanInvalidAlarmThreshold(UnitOfWork uow)
        {
            var thresholds = (from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uow, true).AsParallel()
                              where !p.IsValid
                              select p).ToList();

            thresholds.ForEach((threshold) => threshold.Delete());
            return thresholds.Count > 0;
        }
#endif

        public UFUAModel.UFUAArea GetAlarmArea(string name, UFUAModel.UFUAArea parent = null, UnitOfWork uow = null)
        {
            if (uow == null)
                uow = this.uow;

            List<UFUAModel.UFUAArea> list;
            if (parent != null)
            {
                list = (from a in parent.UFUAAreas where a.Name == name select a).ToList();
            }
            else
            {
                list = (from a in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                        where a.UFUAAreaAss == null && a.Name == name
                        select a).ToList();
            }

            if (list.Count > 0)
                return list[0];
            return null;
        }

        internal UFUAModel.UFUAAlarmSource GetAlarmSource(string sourcePath)
        {
            UFUAModel.UFUAAlarmSource source;
            source = (from s in new XPQuery<UFUAModel.UFUAAlarmSource>(uow, true).AsParallel()
                      select s).ToList().AsParallel().Where(s => s.GetRelativeName() == sourcePath).Select(s => s).FirstOrDefault();
            return source;
        }

        internal UFUAModel.UFUAArea GetAlarmArea(string areaPath)
        {
            UFUAModel.UFUAArea area;
            area = (from s in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                      select s).ToList().AsParallel().Where(s => s.GetRelativeName() == areaPath).Select(s => s).FirstOrDefault();
            return area;
        }

        public UFUAModel.UFUAAlarmSource GetAlarmSource(string name, UFUAModel.UFUAArea parent, UnitOfWork uow = null)
        {
            if (uow == null)
                uow = this.uow;

            List<UFUAModel.UFUAAlarmSource> list;
            if (parent != null)
            {
                list = (from a in parent.UFUAAlarmSources where a.Name == name select a).ToList();
                if (list.Count > 0)
                    return list[0];
            }
            return null;
        }

        public OPCUAEntityReference GetAlarmsSourceOPCUAEntityReference(string sourcePath, bool inExecution = false)
        {
            NodeId nodeid = null;
            UFUAModel.UFUAArea area = null;

            StringBuilder rel = new StringBuilder();
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);

            CachedUnitOfWork task = null;
            if (inExecution)
                task = cachedUnitOfWorks.BeginUnitOfWork();

            try
            {
                var sources = sourcePath.Split(UFUAModel.UFUAArea.AreaSeparator);
                for (int ii = 0; ii < sources.Length; ii++)
                {
                    var parent = area;
                    area = GetAlarmArea(sources[ii], area, task?.UnitOfWork);
                    if (area == null)
                    {
                        var source = GetAlarmSource(sources[ii], parent, task?.UnitOfWork);
                        if (source != null)
                        {
                            nodeid = new Opc.Ua.NodeId(source.NodeId, ns);
                            rel.AppendFormat("/{1}:{0}", sources[ii], ns);
                            break;
                        }
                    }
                    else
                        rel.AppendFormat("/{1}:{0}", sources[ii], ns);
                }
            }
            finally
            {
                if (task != null)
                    task.Dispose();
            }

            if (nodeid == null && area != null)
                nodeid = new Opc.Ua.NodeId(area.NodeId, ns);
            else if (nodeid == null)
                return null;

            var relativepath = String.Format("/{1}:{2}{0}", rel.ToString(), ns, UFUAServerInfo.UFUAServerInfo.GetAlarmRootName());
            var applicationName = GetAplicationName();
            return new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                            relativepath, nodeid, string.Format("{0} ({1})", sourcePath, applicationName), null, relativepath);
        }
        #endregion

        #region Tags and Prototypes
        internal IList<String> GetPrototypesNames()
        {
            return (from p in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                    where p.UFUATagOwner == null
                    orderby p.Name
                    select p.Name).ToList();
        }

        internal IList<UFUAModel.UFUATagPrototype> GetPrototypes()
        {
            return (from p in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                    where p.UFUATagOwner == null
                    orderby p.Name
                    select p).ToList();
        }

#if !NET_STANDARD
        internal UFUAModel.UFUATagPrototype GetPrototype(string name, string tagOwnerPath = null)
        {
            return GetPrototype(name, uow, tagOwnerPath);
        }

        internal UFUAModel.UFUATagPrototype GetPrototype(string name, UnitOfWork _uow, string tagOwnerPath = null)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            if (tagOwnerPath != null)
                return (from p in new XPQuery<UFUAModel.UFUATagPrototype>(_uow, true).AsParallel()
                        where p.TagOwnerPath == tagOwnerPath && p.Name == name
                        select p).FirstOrDefault();
            else
                return (from p in new XPQuery<UFUAModel.UFUATagPrototype>(_uow, true).AsParallel()
                        where p.UFUATagOwner == null && p.Name == name
                        select p).FirstOrDefault();
        }

        public UFUAModel.UFUATagPrototype CreateSubPrototype(UFUAModel.UFUATag tag, UnitOfWork _uow = null)
        {
            UnitOfWork luow = _uow ?? uow;
            if (tag.ModelType != UFUAModel.ModelType.ObjectType)
                return null;
            var prototypeName = tag.PrototypeName;
            var prototyperef = (from p in new XPQuery<UFUAModel.UFUATagPrototype>(luow, true).AsParallel()
                                where p.Name == prototypeName && p.UFUATagOwner == null
                                select p).FirstOrDefault();

            if (prototyperef == null)
                return null;

            var prototype = (from p in new XPQuery<UFUAModel.UFUATagPrototype>(luow, true).AsParallel()
                             where p.Name == prototypeName && p.UFUATagOwner == tag
                             select p).FirstOrDefault();

            try
            {
                trackingChangesDisabled = true;

                if (prototype == null)
                {
                    var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(luow, luow, checkattributes: false, copyaggregated: true, copyassociation: true);
                    prototype = cloneHelper.Clone(prototyperef, false);
                    var helper = new UFUAModel.Helpers.AssociationHelper(luow);
                    helper.CopyAssociationReferences(prototyperef, prototype);
                    ClearSubPrototypeMembers(tag);
                    tag.SubPrototypeMembers.Add(prototype);
                }
                else if (!tag.IsSubPrototypeMembersCreated)
                {
                    var helper = new UFUAModel.Helpers.AssociationHelper(luow);
                    var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(luow, luow, checkattributes: false, copyaggregated: true, copyassociation: true);
                    var copytags = (from c in prototype.GetTagMembers().AsParallel() where !c.UseShared.Value select c).ToList();
                    prototype = cloneHelper.Clone(prototyperef, false);
                    helper.CopyAssociationReferences(prototyperef, prototype);
                    var targettags = prototype.GetTagMembers();
                    copytags.ForEach((copytag) =>
                    {
                        EnsureValidNodeId(copytag);
                        var targettag = (from c in targettags.AsParallel() where c.NodeId == copytag.NodeId select c).FirstOrDefault();
                        if (targettag != null)
                        {
                            using (new UFUAModel.Helpers.KeepReadOnlyProperties(targettag))
                            {
                                helper.ClearAssociationReferences(targettag);
                                cloneHelper.Clone(copytag, false, targettag);
                                helper.CopyAssociationReferences(copytag, targettag);
                            }
                        }
                    });
                    ClearSubPrototypeMembers(tag);
                    tag.SubPrototypeMembers.Add(prototype);
                }
            }
            finally
            {
                trackingChangesDisabled = false;
                tag.IsSubPrototypeMembersCreated = true;
            }

            return prototype;
        }

        void ClearSubPrototypeMembers(UFUAModel.UFUATag tag)
        {
            while (tag.SubPrototypeMembers.Count > 0)
            {
                var member = tag.SubPrototypeMembers[0];
                tag.SubPrototypeMembers.Remove(member);
                member.Delete();
            }
        }

        bool CleanSubPrototypeMembers(UnitOfWork _uow = null)
        {
            UnitOfWork luow = _uow ?? uow;

            var prototypes = (from p in new XPQuery<UFUAModel.UFUATagPrototype>(luow, true).AsParallel()
                              where p.UFUATagOwner != null
                              select p).ToList();

            if (prototypes.Count == 0)
                return false;

            bool bClean = false;
            var toDelete = new List<UFUAModel.UFUATagPrototype>();
            foreach (var prototype in prototypes)
            {
                if (prototype.IsDeleted)
                    continue;

                if (prototype.UFUATagOwner.PrototypeName != prototype.Name)
                    toDelete.Add(prototype);
                else
                {
                    bClean = CleanSubPrototypeMembers(prototype);
                    if (prototype.Folders.Count == 0 && prototype.Members.Count == 0)
                        toDelete.Add(prototype);
                }
            }
            toDelete.ForEach((prototype) => prototype.Delete());

            return bClean || toDelete.Count > 0;
        }

        bool CleanSubPrototypeMembers(UFUAModel.UFUATagPrototype prototype)
        {
            var currentMembers = new List<Guid>();
            var currentPrototype = FindPrototypeByName(prototype.Name);
            if (currentPrototype != null)
            {
                foreach (var member in currentPrototype.Members)
                    currentMembers.Add(member.NodeId);
            }

            var members = (from c in prototype.Members where c.UseShared.Value || !currentMembers.Contains(c.NodeId) select c).ToList();
            members.ForEach((item) => item.Delete());

            bool bClean = members.Count > 0;
            var toDelete = new List<UFUAModel.UFUAFolder>();
            foreach (var folder in prototype.Folders)
            {
                bClean = CleanSubPrototypeMembers(folder);
                if (folder.UFUAFolders.Count == 0 && folder.UFUATags.Count == 0)
                    toDelete.Add(folder);
            }
            toDelete.ForEach((folder) => folder.Delete());

            return bClean || toDelete.Count > 0;
        }

        bool CleanSubPrototypeMembers(UFUAModel.UFUAFolder root)
        {
            var members = (from c in root.UFUATags where c.UseShared.Value select c).ToList();
            members.ForEach((item) => item.Delete());

            bool bClean = members.Count > 0;
            var toDelete = new List<UFUAModel.UFUAFolder>();
            foreach (var folder in root.UFUAFolders)
            {
                bClean = CleanSubPrototypeMembers(folder);
                if (folder.UFUAFolders.Count == 0 && folder.UFUATags.Count == 0)
                    toDelete.Add(folder);
            }
            toDelete.ForEach((folder) => folder.Delete());

            return bClean || toDelete.Count > 0;
        }
#endif

        String NewPrototypeName(String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultPrototypeName;

            if (listname == null)
                listname = GetPrototypesNames();

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

#if !NET_STANDARD
        internal List<UFUAModel.UFUAEnumString> GetEnums()
        {
            return (from c in new XPQuery<UFUAModel.UFUAEnumString>(uow).AsParallel()
                    select c).ToList();
        }
        bool PrototypeNameExists(String name)
        {
            return (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                    where prototype.Name == name && prototype.UFUATagOwner == null
                    select prototype).ToList().Count > 0;
        }
#endif

        public UFUAModel.UFUATagPrototype FindPrototypeByName(String name)
        {
            return (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                    where prototype.Name == name && prototype.UFUATagOwner == null
                    select prototype).FirstOrDefault();
        }

#if !NET_STANDARD
        internal void DropChanges()
        {
            uow.DropChanges();
        }

        internal UFUAModel.UFUATagPrototype FindPrototypeByNodeId(Guid guid)
        {
            return (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                    where prototype.NodeId == guid && prototype.UFUATagOwner == null
                    select prototype).FirstOrDefault();
        }

        internal UFUAModel.UFUATagPrototype FindSubPrototypeByNodeId(Guid guid, Guid ownerId)
        {
            return (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                    where prototype.NodeId == guid && prototype.UFUATagOwner != null && prototype.UFUATagOwner.NodeId == ownerId
                    select prototype).FirstOrDefault();
        }
#endif

        public UFUAModel.UFUATagPrototype AddNewPrototype()
        {
            return new UFUAModel.UFUATagPrototype(uow) { Name = NewPrototypeName(), NodeId = Guid.NewGuid(), CreateDate = DateTime.UtcNow };
        }

#if !NET_STANDARD
        public int NewMemberOrderId(UFUAModel.UFUAFolder protofolder)
        {
            List<UFUAModel.UFUATag> last = new List<UFUAModel.UFUATag>();
            last.AddRange(protofolder.UFUATags.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));

            if (last.Count > 0)
            {
                return last[last.Count - 1].MemberOrderId.Value + 1;
            }
            return 0;
        }
        public int NewMemberOrderId(UFUAModel.UFUATagPrototype prototype)
        {
            List<UFUAModel.UFUATag> last = new List<UFUAModel.UFUATag>();

            prototype.EnsureUniqueMembersOrderId();
            last.AddRange(prototype.Members.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));
            if (last.Count > 0)
            {
                return last[last.Count - 1].MemberOrderId.Value + 1;
            }
            return 0;
        }
        public int NewFolderOrderId(UFUAModel.UFUAFolder prototype)
        {
            List<UFUAModel.UFUAFolder> last = new List<UFUAModel.UFUAFolder>();
            last.AddRange(prototype.UFUAFolders.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));
            if (last.Count > 0)
            {
                return last[last.Count - 1].MemberOrderId.Value + 1;
            }
            return 0;
        }
        public int NewFolderOrderId(UFUAModel.UFUATagPrototype prototype)
        {
            List<UFUAModel.UFUAFolder> last = new List<UFUAModel.UFUAFolder>();
            last.AddRange(prototype.Folders.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));
            if (last.Count > 0)
            {
                return last[last.Count - 1].MemberOrderId.Value + 1;
            }
            return 0;
        }

        internal bool MoveMemberDown(UFUAModel.UFUAFolder folder)
        {
            List<UFUAModel.UFUAFolder> last = new List<UFUAModel.UFUAFolder>();
            if (folder.UFUAFolderAss != null)
                last.AddRange(folder.UFUAFolderAss.UFUAFolders.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));
            else if (folder.UFUATagPrototype != null)
                last.AddRange(folder.UFUATagPrototype.Folders.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));
            if (last.Count > 0)
            {
                int current = last.IndexOf(folder);
                if (current < last.Count - 1)
                {
                    int forward = last[current + 1].MemberOrderId.Value;
                    last[current + 1].MemberOrderId = last[current].MemberOrderId;
                    last[current].MemberOrderId = forward;
                    return true;
                }
            }
            return false;
        }

        internal bool MoveMemberUp(UFUAModel.UFUAFolder folder)
        {
            List<UFUAModel.UFUAFolder> last = new List<UFUAModel.UFUAFolder>();
            if (folder.UFUAFolderAss != null)
                last.AddRange(folder.UFUAFolderAss.UFUAFolders.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));
            else if (folder.UFUATagPrototype != null)
                last.AddRange(folder.UFUATagPrototype.Folders.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));
            if (last.Count > 0)
            {
                int current = last.IndexOf(folder);
                if (current > 0)
                {
                    int backward = last[current - 1].MemberOrderId.Value;
                    last[current - 1].MemberOrderId = last[current].MemberOrderId;
                    last[current].MemberOrderId = backward;
                    return true;
                }
            }
            return false;
        }

        internal bool MoveMemberUp(UFUAModel.UFUATag tag)
        {
            if (tag.PrototypeReference == null)
                return false;
            List<UFUAModel.UFUATag> last = new List<UFUAModel.UFUATag>();
            if (tag.UFUAFolder != null)
                last.AddRange(tag.UFUAFolder.UFUATags.OrderBy(o => o.MemberOrderId));
            else
            {
                tag.PrototypeReference.EnsureUniqueMembersOrderId();
                last.AddRange(tag.PrototypeReference.Members.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));
            }
            if (last.Count > 0)
            {
                int current = last.IndexOf(tag);
                if (current > 0)
                {
                    int backward = last[current - 1].MemberOrderId.Value;
                    last[current - 1].MemberOrderId = last[current].MemberOrderId;
                    last[current].MemberOrderId = backward;
                    return true;
                }
            }
            return false;
        }

        internal bool MoveMemberDown(UFUAModel.UFUATag tag)
        {
            if (tag.PrototypeReference == null)
                return false;
            List<UFUAModel.UFUATag> last = new List<UFUAModel.UFUATag>();
            if (tag.UFUAFolder != null)
                last.AddRange(tag.UFUAFolder.UFUATags.OrderBy(o => o.MemberOrderId));
            else
            {
                tag.PrototypeReference.EnsureUniqueMembersOrderId();
                last.AddRange(tag.PrototypeReference.Members.OrderBy(o => o.MemberOrderId).Where(o => o.MemberOrderId.HasValue));
            }
            if (last.Count > 0)
            {
                int current = last.IndexOf(tag);
                if (current < last.Count - 1)
                {
                    int forward = last[current + 1].MemberOrderId.Value;
                    last[current + 1].MemberOrderId = last[current].MemberOrderId;
                    last[current].MemberOrderId = forward;
                    return true;
                }
            }
            return false;
        }

        internal bool SetMemberPosition(UFUAModel.UFUAFolder folder, int newpos)
        {
            if (folder.UFUATagPrototype == null || newpos < 0)
                return false;
            List<UFUAModel.UFUAFolder> last = new List<UFUAModel.UFUAFolder>();
            if (folder.UFUAFolderAss != null)
                last.AddRange(folder.UFUAFolderAss.UFUAFolders.OrderBy(o => o.MemberOrderId));
            else if (folder.UFUATagPrototype != null)
                last.AddRange(folder.UFUATagPrototype.Folders.OrderBy(o => o.MemberOrderId));
            int current = last.IndexOf(folder);
            if (current != -1 && current != newpos &&
                last.Count > 0 && last.Count > newpos)
            {
                last.Remove(folder);
                last.Insert(newpos, folder);
                for (int ii = 0; ii < last.Count; ii++)
                    last[ii].MemberOrderId = ii;
                return true;
            }
            return false;
        }

        internal bool SetMemberPosition(UFUAModel.UFUATag tag, int newpos)
        {
            if (tag.PrototypeReference == null || newpos < 0)
                return false;
            List<UFUAModel.UFUATag> last = new List<UFUAModel.UFUATag>();
            if (tag.UFUAFolder != null)
                last.AddRange(tag.UFUAFolder.UFUATags.OrderBy(o => o.MemberOrderId));
            else
            {
                tag.PrototypeReference.EnsureUniqueMembersOrderId();
                last.AddRange(tag.PrototypeReference.Members.OrderBy(o => o.MemberOrderId));
            }
            int current = last.IndexOf(tag);
            if (current != -1 && current != newpos &&
                last.Count > 0 && last.Count > newpos)
            {
                last.Remove(tag);
                last.Insert(newpos, tag);
                for (int ii = 0; ii < last.Count; ii++)
                    last[ii].MemberOrderId = ii;
                return true;
            }
            return false;
        }

        internal IList<String> GetTagsMemberNameList(UFUAModel.UFUAFolder root, UFUAModel.UFUATagPrototype prototype)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.UFUAFolder == null
                        select tag).ToList().AsParallel().Where(tag =>
                        tag.PrototypeReference == prototype).Select(tag =>
                        tag.Name).ToList();
            }

            return (from c in root.UFUATags.AsParallel() select c.Name).ToList();
        }
#endif

        internal IList<String> GetTagsNameList(UFUAModel.UFUAFolder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.UFUAFolder == null && tag.UFUATagPrototype == null
                        select tag.Name).ToList();
            }

            return (from c in root.UFUATags.AsParallel() select c.Name).ToList();
        }

        internal IList<String> GetTagsNameList(UFUAModel.UFUATagPrototype prototype, UFUAModel.UFUAFolder root = null)
        {
            if (root == null)
                return (from c in prototype.Members.AsParallel() select c.Name).ToList();
            else
                return (from c in root.UFUATags.AsParallel() select c.Name).ToList();
        }

#if !NET_STANDARD
        internal IList<UFUAModel.UFUATag> GetTagsList(UFUAModel.UFUATagPrototype prototype, UFUAModel.UFUAFolder root = null)
        {
            if (root == null)
                return (from c in prototype.Members.AsParallel() select c).ToList();
            else
                return (from c in root.UFUATags.AsParallel() select c).ToList();
        }
#endif

        public UFUAModel.UFUATag AddNewTag(UFUAModel.UFUAFolder root, bool prototypemember = false, int orderid = -1)
        {
            var name = String.Empty;
            if (prototypemember)
                name = Properties.Settings.Default.DefaultMemberName;
            var tag = new UFUAModel.UFUATag(uow) { Name = NewTagName(root, name), NodeId = Guid.NewGuid(), CreateDate = DateTime.UtcNow, MemberOrderId = orderid };
            if (root != null)
                root.UFUATags.Add(tag);
            return tag;
        }

        internal String NewTagName(UFUAModel.UFUAFolder root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultTagName;

            if (listname == null)
                listname = GetTagsNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

#if !NET_STANDARD
        internal String NewImportedPrototypeName(String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultPrototypeName;

            ulong counter;
            string fmtzero = "0";
            var baseName = name;
            if (mapcounter == null || !mapcounter.TryGetValue(baseName, out counter))
                counter = 1;

            if (listname == null)
                listname = GetPrototypesNames();

            while (listname.Contains(name))
                name = String.Format(format, baseName, (counter++).ToString(fmtzero));
            listname.Add(name);

            if (mapcounter != null)
                mapcounter[baseName] = counter;

            return name;
        }
        internal String NewImportedTagName(UFUAModel.UFUAFolder root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}", UFUAModel.UFUATagPrototype prototype = null)
        {
            if (String.IsNullOrEmpty(name))
                name = prototype != null ? Properties.Settings.Default.DefaultMemberName : Properties.Settings.Default.DefaultTagName;

            ulong counter;
            string fmtzero = "0";
            var baseName = name;
            if (mapcounter == null || !mapcounter.TryGetValue(baseName, out counter))
                counter = 1;

            if (listname == null)
                listname = prototype == null ? GetTagsNameList(root) : GetTagsNameList(prototype, root);

            while (listname.Contains(name))
                name = String.Format(format, baseName, (counter++).ToString(fmtzero));
            listname.Add(name);

            if (mapcounter != null)
                mapcounter[baseName] = counter;

            return name;
        }

        UFUAModel.UFUATag TagExists(String guid, UnitOfWork uow)
        {
            var searchGuid = new Guid(guid);
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    where tag.NodeId == searchGuid
                    select tag).FirstOrDefault();
        }

        UFUAModel.UFUATag TagNameInFolderExists(String name, UFUAModel.UFUAFolder root, UnitOfWork uow)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    where tag.UFUAFolder == root && tag.UFUATagPrototype == null && tag.Name == name && tag.IsPrototypeMember == false
                    select tag).FirstOrDefault();
        }
#endif

        public UFUAModel.UFUATag GetTag(string name, UFUAModel.UFUAFolder folder = null)
        {
            List<UFUAModel.UFUATag> list;
            if (folder == null)
            {
                list = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.UFUAFolder == null && tag.UFUATagPrototype == null && tag.Name == name
                        select tag).ToList();
            }
            else
                list = (from c in folder.UFUATags.AsParallel() where c.Name == name select c).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }

        public bool TagNameExists(String name, UFUAModel.UFUAFolder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.UFUAFolder == null && tag.UFUATagPrototype == null && tag.Name == name
                        select tag).ToList().Count > 0;
            }

            return (from c in root.UFUATags.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }

        internal String NewTagName(UFUAModel.UFUATagPrototype prototype, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultMemberName;

            if (listname == null)
                listname = GetTagsNameList(prototype);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

#if !NET_STANDARD
        static bool TagNameExists(String name, UFUAModel.UFUATagPrototype prototype)
        {
            return (from c in prototype.Members.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }
#endif

        public UFUAModel.UFUATag AddNewTag(UFUAModel.UFUATagPrototype prototype, int orderid = -1)
        {
            var tag = new UFUAModel.UFUATag(uow) { Name = NewTagName(prototype), NodeId = Guid.NewGuid(), MemberOrderId = orderid };

            prototype.Members.Add(tag);
            return tag;
        }

#if !NET_STANDARD
        void ValidateRenamedVariables()
        {
            if (removedVariablesManager == null)
                return;

            var guidHelper = new GuidHelper(uow, TypeObject.Tag);
            guidHelper.Guids.ForEach((guid) => 
            { 
                if (removedVariablesManager.ExistGuid(guid))
                {
                    var key = removedVariablesManager.FindGuid(guid);
                    removedVariablesManager.RemoveMember(key);
                    removedVariablesManager.RemoveVariable(key);
                }
            });
        }

        internal void EnsureValidNodeId(UFUAModel.UFUATagPrototype prototype)
        {
            if (removedVariablesManager == null)
                return;

            var tags = prototype.GetTagMembers();
            tags.ForEach((tag) => EnsureValidNodeId(tag));

            if (removedVariablesManager.ExistMember(prototype.Name))
            {
                prototype.NodeId = removedVariablesManager.FindMember(prototype.Name);
                removedVariablesManager.RemoveMember(prototype.Name);
            }
            else if (removedVariablesManager.ExistGuid(prototype.NodeId))
            {
                var key = removedVariablesManager.FindGuid(prototype.NodeId);
                removedVariablesManager.RemoveMember(key);
            }
        }

        internal void EnsureValidNodeId(UFUAModel.UFUAFolder folder)
        {
            if (removedVariablesManager == null)
                return;

            var tags = folder.GetTagMembers();
            tags.ForEach((tag) => EnsureValidNodeId(tag));
        }

        internal void EnsureValidNodeId(UFUAModel.UFUATag ufuaTag)
        {
            if (removedVariablesManager == null)
                return;

            if (!ufuaTag.IsPrototypeMember)
            {
                var key = ufuaTag.GetRelativeName();
                if (removedVariablesManager.ExistVariable(key))
                {
                    ufuaTag.NodeId = removedVariablesManager.FindVariable(key);
                    removedVariablesManager.RemoveVariable(key);
                }
                else if (removedVariablesManager.ExistGuid(ufuaTag.NodeId))
                {
                    key = removedVariablesManager.FindGuid(ufuaTag.NodeId);
                    removedVariablesManager.RemoveVariable(key);
                }
            }
            else
            {
                var key = String.Format("{0}:{1}", ufuaTag.PrototypeReference.Name, ufuaTag.GetRelativeName());
                if (removedVariablesManager.ExistMember(key))
                {
                    ufuaTag.NodeId = removedVariablesManager.FindMember(key);
                    removedVariablesManager.RemoveMember(key);
                }
                else if (removedVariablesManager.ExistGuid(ufuaTag.NodeId))
                {
                    key = removedVariablesManager.FindGuid(ufuaTag.NodeId);
                    removedVariablesManager.RemoveMember(key);
                }
            }
        }

        void ProcessRemoved(UFUAModel.UFUATagPrototype prototype)
        {
            if (removedVariablesManager == null)
                return;

            var tags = prototype.GetTagMembers();
            tags.ForEach((tag) => ProcessRemoved(tag));

            removedVariablesManager.RemoveMember(prototype.Name);
            removedVariablesManager.AddMember(prototype.Name, prototype.NodeId);
        }

        void ProcessRemoved(UFUAModel.UFUATag tag)
        {
            if (removedVariablesManager == null || tag == null || tag.IsSubPrototypeMember)
                return;

            if (!tag.IsPrototypeMember)
            {
                var key = tag.GetRelativeName();
                removedVariablesManager.RemoveVariable(key);
                removedVariablesManager.AddVariable(key, tag.NodeId);
            }
            else
            {
                var key = String.Format("{0}:{1}", tag.PrototypeReference.Name, tag.GetRelativeName());
                removedVariablesManager.RemoveMember(key);
                removedVariablesManager.AddMember(key, tag.NodeId);
            }
        }

        internal List<UFUAModel.UFUATag> GetObjectTagCollection(UFUAModel.UFUATagPrototype prototype)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    where tag.ModelType == ModelType.ObjectType && 
                    tag.PrototypeModel == prototype.Name || tag.PrototypeModel == prototype.NodeId.ToString()
                    select tag).ToList().AsParallel().Where(tag =>
                    !tag.IsPrototypeMember).ToList();
        }
#endif

        internal IList<UFUAModel.UFUATag> GetFlatTagCollection()
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    orderby tag.Oid
                    select tag).ToList().AsParallel().Where(tag =>
                    !tag.IsPrototypeMember).ToList();
        }

#if !NET_STANDARD
        internal IList<UFInterfaces.Editors.CrossReferenceResultModel> GetCRFlatTagCollection(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTexts = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
            if (!getTexts && !getTags && !getConnections)
                return result;
            //using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                List<UFUAModel.UFUATag> fulllist = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                                    select tag).ToList().AsParallel().Where(tag => tag.PrototypeReference == null).ToList();

                List<UFUAModel.UFUATag> prototypeModelList = fulllist.AsParallel().Where(tag => (!string.IsNullOrEmpty(tag.PrototypeModel) && tag.ModelType != UFUAModel.ModelType.Method)).ToList();
                List<UFUAModel.UFUATag> uFUAAlarmThresholdList = fulllist/*.AsParallel()*/.Where(tag => tag.UFUAAlarmThresholds.Count > 0).ToList();

                List<UFUAModel.UFUATag> _list = fulllist.ToList().AsParallel().Where(tag =>
                                                      !tag.IsPrototypeMember).ToList();

                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();

                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                if (model.QuitEvent.IsCancellationRequested)
                    return result;
                string applicationName = GetAplicationName(true);
                List<UFUATag> protList = new List<UFUATag>();
                Dictionary<UFUATag, String> tagToRelativeMap = new Dictionary<UFUATag, String>();
                Dictionary<String, String> relativeToNameMap = new Dictionary<String, String>();
                string auditDefConnection = GetAuditTraceDefaultConnection();
                string endpointUrl = GetDefaultLocalEndpoint(true);
                if (getTags || getConnections || getTexts)
                {

                    var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                    dsInterface.SetDocumentParent(this);
                    var localTag = getTags ? dsInterface.GetVariables() : new List<string>();
                    if (getTags)
                        foreach (var tag in localTag)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                //loopState.Break();
                                break;
                            var typeDefinition = "UFUASVariableSmall";
                            bool hasPrototypeModel = false;
                            string relativePath = tag.Replace('&', '\\');
                            var name = relativePath.Split('\\').LastOrDefault();
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                TypeDefinition = typeDefinition,
                                HasPrototypeModel = hasPrototypeModel,
                                RelativePath = relativePath,
                                Name = name,
                                AppName = dsInterface.DataSynkName,
                                ReferencedNodeId = null,
                                EndpointUrl = null,
                                CReferenceType = CrossReferenceType.Tags,
                                Description = string.Format("{0}", Properties.Resources.CrossReferenceServerTagDefined),
                                Settings = string.Format("{0}|{1}", DocManagerType.UFUAServer, rootBase),
                                ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                IconType = DocManagerType.UFUAServer.ToString()
                            });
                            bool? isInError = (from t in result where $"{t.RelativePath}" == $"{relativePath}" select t).ToList()?.Count > 1;
                            if(isInError.HasValue && isInError.Value)
                                model.ErrorMessages.Add(string.Format(Properties.Resources.CrossReferenceServerTagMultiDefinitions, $"{relativePath}"));
                        }//);

                    //Parallel.ForEach(_list, (tag, loopState) =>
                    foreach (var tag in _list)
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            //loopState.Break();
                            break;
                        var typeDefinition = UFUAEditorManagerComponent.GetTagBitmapImageName(tag);
                        bool hasPrototypeModel = !string.IsNullOrEmpty(tag.PrototypeModel) && tag.ModelType != UFUAModel.ModelType.Method;
                        string relativePath = tag.GetRelativePath(ns);
                        var name = relativePath.Split('/').LastOrDefault();
                        string referencedNodeId = tag.NodeId.ToString();
                        //lock (tagToRelativeMap)
                        {
                            if (!tagToRelativeMap.ContainsKey(tag))
                            {
                                tagToRelativeMap.Add(tag, relativePath);
                            }
                            if (!relativeToNameMap.ContainsKey(relativePath))
                            {
                                relativeToNameMap.Add(relativePath, name);
                            }
                        }
                        if(getTexts)
                        {
                            if(!string.IsNullOrEmpty(tag.Description))
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = tag.Description,
                                    Name = tag.Description,
                                    AppName = applicationName,
                                    CReferenceType = CrossReferenceType.Strings,
                                    Description = string.Format("{0} ({1})", relativePath, Properties.Resources.CRTagDescription),
                                    Settings = string.Format("{0}|{1}", DocManagerType.UFUAServer, rootBase),
                                    ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                    IconType = DocManagerType.UFUAServer.ToString()
                                });
                            if (tag.EnumStrings.Count > 0)
                                tag.EnumStrings.ToList().ForEach(enums =>
                                {
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = enums.Data,
                                        Name = enums.Data,
                                        AppName = applicationName,
                                        CReferenceType = CrossReferenceType.Strings,
                                        Description = string.Format("{0} ({1})", relativePath, Properties.Resources.CRTagEnums),
                                        Settings = string.Format("{0}|{1}", DocManagerType.UFUAServer, rootBase),
                                        ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                        IconType = DocManagerType.UFUAServer.ToString()
                                    });
                                });
                        }
                        //if(getTags)
                        {
                            if (getConnections && tag.AuditTraceEnabled)
                                //lock (result)
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = auditDefConnection,
                                    Name = Properties.Resources.AuditDefConnectionString,
                                    AppName = applicationName,
                                    CReferenceType = CrossReferenceType.Connections,
                                    Description = tag.GetRelativeName(),
                                    Settings = string.Format("{0}|{1}|{2}", DocManagerType.AuditingConn.ToString(), rootBase, Title),
                                    ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                    IconType = typeDefinition
                                });
                            if (getTags && !string.IsNullOrEmpty(tag.HistorianSettings))
                                //lock (result)
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    TypeDefinition = typeDefinition,
                                    HasPrototypeModel = hasPrototypeModel,
                                    RelativePath = relativePath,
                                    Name = name,
                                    AppName = applicationName,
                                    ReferencedNodeId = referencedNodeId,
                                    EndpointUrl = endpointUrl,
                                    CReferenceType = CrossReferenceType.Tags,
                                    Description = string.Format("{0} ({1})", tag.HistorianSettings, Properties.Resources.CrossReferenceServerTagHistorianSettings),
                                    Settings = string.Format("{0}|{1}", DocManagerType.HistoricalPrototypes, rootBase),
                                    ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                    IconType = DocManagerType.HistoricalPrototypes.ToString()
                                });
                            if (getTags && !string.IsNullOrEmpty(tag.ScriptCode))
                                //lock (result)
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    TypeDefinition = typeDefinition,
                                    HasPrototypeModel = hasPrototypeModel,
                                    RelativePath = relativePath,
                                    Name = name,
                                    AppName = applicationName,
                                    ReferencedNodeId = referencedNodeId,
                                    EndpointUrl = endpointUrl,
                                    CReferenceType = CrossReferenceType.Tags,
                                    Description = string.Format("{0}", Properties.Resources.CrossReferenceServerTagScript),
                                    Settings = string.Format("{0}|{1}", DocManagerType.AddressSpaceScriptCode, rootBase),
                                    ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                    IconType = DocManagerType.AddressSpaceScriptCode.ToString()
                                });

                            //lock (result)
                        if(getTags)
                            {
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    TypeDefinition = typeDefinition,
                                    HasPrototypeModel = hasPrototypeModel,
                                    RelativePath = relativePath,
                                    Name = name,
                                    AppName = applicationName,
                                    ReferencedNodeId = tag.NodeId.ToString(),
                                    EndpointUrl = endpointUrl,
                                    CReferenceType = CrossReferenceType.Tags,
                                    Description = string.Format("{0}", Properties.Resources.CrossReferenceServerTagDefined),
                                    Settings = string.Format("{0}|{1}", DocManagerType.UFUAServer, rootBase),
                                    ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                    IconType = DocManagerType.UFUAServer.ToString()
                                });
                                bool? isInError = (from t in result where $"{t.RelativePath}" == $"{relativePath}" && t.Description == Properties.Resources.CrossReferenceServerTagDefined select t).ToList()?.Count > 1;
                                if (isInError.HasValue && isInError.Value)
                                    model.ErrorMessages.Add(string.Format(Properties.Resources.CrossReferenceServerTagMultiDefinitions, $"{tag.GetFullName()}"));
                            }
                        }
                    }//);
                    var mapList = new Dictionary<UFUAModel.UFUATag, List<UFUAModel.UFUATag>>();
                    prototypeModelList.ToList().ForEach(tag =>
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return;
                        string relativePath = string.Empty;
                        var name = string.Empty;
                        if (tagToRelativeMap.ContainsKey(tag))
                        {
                            relativePath = tagToRelativeMap[tag];
                            name = relativeToNameMap[relativePath];
                        }
                        else
                        {
                            relativePath = tag.GetRelativePath(ns);
                            name = relativePath.Split('/').LastOrDefault();
                            tagToRelativeMap.Add(tag, relativePath);
                            relativeToNameMap.Add(relativePath, name);
                        }


                        var typeDefinition = UFUAEditorManagerComponent.GetTagBitmapImageName(tag);
                        var refList = GetCRPFlatTagCollection(mapList, auditDefConnection, protList, applicationName, endpointUrl, name, tag, relativePath, model);
                        if (refList.Count > 0)
                        {
                            //lock (protList)
                                protList.Add(tag);
                            result.AddRange(refList);
                        }
                    });

                }

                uFUAAlarmThresholdList.ToList().ForEach(tag =>
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;
                    var typeDefinition = UFUAEditorManagerComponent.GetTagBitmapImageName(tag);
                    bool hasPrototypeModel = !string.IsNullOrEmpty(tag.PrototypeModel) && tag.ModelType != UFUAModel.ModelType.Method;
                    string relativePath = string.Empty;
                    var name = string.Empty;
                    if(tagToRelativeMap.ContainsKey(tag))
                    {
                        relativePath = tagToRelativeMap[tag];
                        name = relativeToNameMap[relativePath];
                    }
                    else
                    {
                        relativePath = tag.GetRelativePath(ns);
                        name =  relativePath.Split('/').LastOrDefault();
                        tagToRelativeMap.Add(tag, relativePath);
                        relativeToNameMap[relativePath] = name;
                    }
                    string docType = DocManagerType.AlarmPrototype.ToString();
                    foreach (var threshold in tag.UFUAAlarmThresholds)
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return;
                        if (threshold.UFUAAlarmDefinitionRef == null)
                            continue;
                        if (getTexts)
                        {
                            var details = threshold.AlarmText;
                            if (!String.IsNullOrEmpty(details))
                            {
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = details,
                                    Name = details,
                                    AppName = applicationName,
                                    CReferenceType = CrossReferenceType.Strings,
                                    Description = string.Format("{0}\\{1} ({2})", Title, details, Properties.Resources.ThresholdText),
                                    Settings = string.Format("{0}|{1}", docType, rootBase),
                                    ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                    IconType = docType
                                });
                            }
                        }

                        if (getTags)
                        {
                            string referencedNodeId = tag.NodeId.ToString();
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                TypeDefinition = typeDefinition,
                                HasPrototypeModel = hasPrototypeModel,
                                RelativePath = relativePath,
                                Name = name,
                                AppName = applicationName,
                                ReferencedNodeId = referencedNodeId,
                                EndpointUrl = endpointUrl,
                                CReferenceType = CrossReferenceType.Tags,
                                Description = string.Format("{0}\\{1} ({2})", threshold.UFUAAlarmDefinitionRef.SourcePath.Replace('/', '\\'), threshold.UFUAAlarmDefinitionRef.Name, Properties.Resources.CrossReferenceServerTagAlarm),
                                Settings = string.Format("{0}|{1}", docType, rootBase),
                                ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                IconType = docType
                            });
                        }
                    }
                });
                return result;
            }
        }
        internal IList<UFInterfaces.Editors.CrossReferenceResultModel> GetCRPFlatTagCollection(Dictionary<UFUAModel.UFUATag, List<UFUAModel.UFUATag>> mapList, string auditDefConnection, List<UFUATag> protList, string applicationName, string endpointUrl, String name, UFUAModel.UFUATag stag, String relativePath, UFInterfaces.Editors.CrossReferenceModel model)
        {
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getTexts = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            //using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                var mainPrototype = GetPrototype(stag.PrototypeName);
                var prototype = stag.SubPrototypeMembers.Count > 0 ? stag.SubPrototypeMembers[0] : null;//CreateSubPrototype(stag);
                if (mainPrototype == null)
                    return result;
                Dictionary<string, UFUAModel.UFUATag> ptaglist = prototype?.GetTagMembers().ToDictionary(x => x.NodeId.ToString(), x => x);
                Dictionary<string, UFUAModel.UFUATag> maintaglist = mainPrototype.GetTagMembers().ToDictionary(x => x.NodeId.ToString(), x => x);
                if (ptaglist == null)
                    ptaglist = maintaglist;
                else
                {
                    maintaglist.Keys.ToList().ForEach(guid =>
                    {
                        if (!ptaglist.ContainsKey(guid))
                            ptaglist.Add(guid, maintaglist[guid]);
                    });
                }

                string historicalSettingsDocType = DocManagerType.HistoricalPrototypes.ToString();
                string prototypeScriptCodeDocType = DocManagerType.PrototypeScriptCode.ToString();
                string MessagePrototypeDocType = DocManagerType.MessagePrototype.ToString();
                string alarmPrototypeDocType = DocManagerType.AlarmPrototype.ToString();
                //Parallel.ForEach(ptaglist, (ptag, loopState) =>
                foreach (var ptag in ptaglist.Values)
                {
                    //lock (protList)
                    protList.Add(ptag);
                    if (!mapList.ContainsKey(ptag))
                        mapList.Add(ptag, new List<UFUAModel.UFUATag>());
                    mapList[ptag].Add(stag);
                    var entity = GetTagOPCUAEntityReference(ptag, mapList[ptag]);
                    string referencedNodeId = entity.ResolvedNodeId.Identifier.ToString();
                    if (model.QuitEvent.IsCancellationRequested)
                        //loopState.Break();
                        break;
                    var typeDefinition = UFUAEditorManagerComponent.GetTagBitmapImageName(ptag);
                    bool hasPrototypeModel = !string.IsNullOrEmpty(ptag.PrototypeModel) && ptag.ModelType != UFUAModel.ModelType.Method;
                    var relpath = ptag.GetRelativeName(false);
                    relpath = $"{relativePath}/{relpath}";
                    //lock (result)
                    if(getTags)
                        result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                        {
                            TypeDefinition = typeDefinition,
                            HasPrototypeModel = hasPrototypeModel,
                            RelativePath = relpath,
                            Name = name,
                            AppName = applicationName,
                            ReferencedNodeId = referencedNodeId,
                            EndpointUrl = endpointUrl,
                            CReferenceType = CrossReferenceType.Tags,
                            Description = string.Format("{0}", Properties.Resources.CrossReferenceServerTagDefined),
                            Settings = string.Format("{0}|{1}", DocManagerType.UFUAServer, rootBase),
                            ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                            IconType = DocManagerType.UFUAServer.ToString()
                        });

                    if (getTexts)
                    {
                        if (!string.IsNullOrEmpty(ptag.Description))
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = ptag.Description,
                                Name = ptag.Description,
                                AppName = applicationName,
                                CReferenceType = CrossReferenceType.Strings,
                                Description = string.Format("{0} ({1})", relpath, Properties.Resources.CRTagDescription),
                                Settings = string.Format("{0}|{1}", DocManagerType.UFUAServer, rootBase),
                                ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                IconType = DocManagerType.UFUAServer.ToString()
                            });
                        if (ptag.EnumStrings.Count > 0)
                            ptag.EnumStrings.ToList().ForEach(enums =>
                            {
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = enums.Data,
                                    Name = enums.Data,
                                    AppName = applicationName,
                                    CReferenceType = CrossReferenceType.Strings,
                                    Description = string.Format("{0} ({1})", relpath, Properties.Resources.CRTagEnums),
                                    Settings = string.Format("{0}|{1}", DocManagerType.UFUAServer, rootBase),
                                    ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                    IconType = DocManagerType.UFUAServer.ToString()
                                });
                            });
                    }

                    if (getConnections && ptag.AuditTraceEnabled)
                        //lock (result)
                        result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                        {
                            RelativePath = auditDefConnection,
                            Name = Properties.Resources.AuditDefConnectionString,
                            AppName = applicationName,
                            CReferenceType = CrossReferenceType.Connections,
                            Description = relpath,
                            Settings = string.Format("{0}|{1}|{2}", DocManagerType.AuditingConn.ToString(), rootBase, Title),
                            ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                            IconType = typeDefinition
                        });
                    if (getTags && !string.IsNullOrEmpty(ptag.HistorianSettings))
                    {
                        string descr = $"{ptag.HistorianSettings}";
                        if (!ptag.UseShared.Value)
                            descr = $"{descr} {Properties.Resources.CrossReferenceServerTagMemberHistorianSettings}";
                        else
                            descr = $"{descr} {Properties.Resources.CrossReferenceServerPrototypeMemberHistorianSettings}";
                        //lock(result)
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                TypeDefinition = typeDefinition,
                                RelativePath = relpath,
                                Name = name,
                                AppName = applicationName,
                                EndpointUrl = endpointUrl,
                                ReferencedNodeId = referencedNodeId,
                                CReferenceType = CrossReferenceType.Tags,
                                Description = string.Format("{2}{1} ({0})", descr, relpath, stag.Name),
                                Settings = string.Format("{0}|{1}", DocManagerType.HistoricalPrototypes, rootBase),
                                ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                IconType = historicalSettingsDocType
                            });
                    }
                    var useShared = ptag.UseShared.Value;
                    if (getTags && !string.IsNullOrEmpty(ptag.ScriptCode))
                    {
                        string descr = Properties.Resources.CrossReferenceServerPrototypeMemberScript;
                        if (!ptag.UseShared.Value)
                            descr = Properties.Resources.CrossReferenceServerTagMemberScript;
                        //lock (result)
                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                TypeDefinition = typeDefinition,
                                HasPrototypeModel = hasPrototypeModel,
                                RelativePath = relpath,
                                Name = name,
                                AppName = applicationName,
                                EndpointUrl = endpointUrl,
                                ReferencedNodeId = referencedNodeId,
                                CReferenceType = CrossReferenceType.Tags,
                                Description = string.Format("{2}{1} ({0})", descr, relpath, stag.Name),
                                Settings = string.Format("{0}|{1}", prototypeScriptCodeDocType, rootBase),
                                ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                IconType = prototypeScriptCodeDocType
                            });
                    }

                    if (getTags && !string.IsNullOrEmpty(ptag.Alarms))
                    {
                        foreach (var threshold in ptag.UFUAAlarmThresholds)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                //loopState.Break();
                                break;
                            string descr = $"{threshold.UFUAAlarmDefinitionRef.SourcePath.Replace('/', '\\')}\\{threshold.UFUAAlarmDefinitionRef.Name}";
                            if (!ptag.UseShared.Value)
                                descr = $"{descr} {Properties.Resources.CrossReferenceServerTagMemberAlarm}";
                            else
                                descr = $"{descr} {Properties.Resources.CrossReferenceServerPrototypeMemberAlarm}";

                            //lock (result)
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    TypeDefinition = typeDefinition,
                                    RelativePath = relativePath,
                                    Name = name,
                                    AppName = applicationName,
                                    EndpointUrl = endpointUrl,
                                    ReferencedNodeId = referencedNodeId,
                                    CReferenceType = CrossReferenceType.Tags,
                                    Description = string.Format("{2}{1} ({0})", descr, relpath, stag.Name),
                                    Settings = string.Format("{0}|{1}", DocManagerType.AlarmPrototype, rootBase),
                                    ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                    IconType = threshold.UFUAAlarmDefinitionRef.Severity == 0 ? MessagePrototypeDocType : alarmPrototypeDocType
                                });
                        }
                    }

                    if (hasPrototypeModel)
                        result.AddRange(GetCRPFlatTagCollection(mapList, auditDefConnection, protList, applicationName, endpointUrl, name, ptag, relpath, model));
                }//);

                return result;
            }
        }
#endif

        internal IList<String> GetFlatTagNameCollection()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var listTag = (from tag in new XPQuery<UFUAModel.UFUATag>(task.UnitOfWork, true).AsParallel()
                               where String.IsNullOrEmpty(tag.PrototypeModel) && tag.ModelType != UFUAModel.ModelType.Method
                               select tag).ToList().AsParallel().Where(tag =>
                               !tag.IsPrototypeMember).ToList();
                return (from tag in listTag.AsParallel()
                        orderby tag.Oid
                        select String.IsNullOrEmpty(tag.FolderPath) ?
                        String.Format("{0}", tag.Name) :
                        String.Format("{0}\\{1}", tag.FolderPath, tag.Name)
                        ).ToList();
            }
        }

#if !NET_STANDARD
        internal UFUAModel.UFUATag FindTagByEntityReference(UFUAModel.TagEntityReference reference)
        {
            if (reference.IsEmpty())
                return null;
            else if (!NodeId.IsNull(reference.NodeId))
            {
                UFUAModel.UFUATag member;
                var tag = FindTagByNodeId(reference.NodeId, out member);
                if (member != null)
                    return member;
                else
                    return tag;
            }
            else 
                return FindTagByNodeId(reference.Guid);
        }

        internal UFUAModel.UFUATag FindTagByNodeId(NodeId nodeId)
        {
            UFUAModel.UFUATag member;
            return FindTagByNodeId(nodeId, uow, out member);
        }

        internal UFUAModel.UFUATag FindTagByNodeId(Guid guid)
        {
            return FindTagByNodeId(guid, uow);
        }

        internal UFUAModel.UFUATag FindTagByNodeId(Guid guid, Guid prototypeId)
        {
            var prototype = (from item in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                             where item.NodeId == prototypeId && item.UFUATagOwner == null
                             select item).FirstOrDefault();

            return FindTagByNodeId(guid, uow, prototype);
        }

        internal UFUAModel.UFUATag FindTagByNodeId(NodeId nodeId, Session session)
        {
            UFUAModel.UFUATag member;
            return FindTagByNodeId(nodeId, session, out member);
        }

        internal UFUAModel.UFUATag FindTagByNodeId(NodeId nodeId, out UFUAModel.UFUATag member)
        {
            return FindTagByNodeId(nodeId, uow, out member);
        }

        internal UFUAModel.UFUATag FindSubPrototypeMemberByNodeId(Guid guid, Guid prototypeId, Guid ownerId)
        {
            var prototype = (from item in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                             where item.NodeId == prototypeId && item.UFUATagOwner != null && item.UFUATagOwner.NodeId == ownerId
                             select item).FirstOrDefault();

            if (prototype == null)
                return null;
            return FindTagByNodeId(guid, uow, prototype);
        }

        internal List<UFUAModel.UFUATag> FindSubPrototypeMembersByNodeId(Guid guid)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    where tag.NodeId == guid 
                    select tag).ToList().AsParallel().Where(tag => tag.IsSubPrototypeMember).ToList();
        }
#endif

        public UFUAModel.UFUATag FindTagByNodeId(NodeId nodeId, Session session, out UFUAModel.UFUATag member)
        {
            Guid tagGuid;
            Guid memberGuid;
            UFUAModel.UFUATag instance = null;
            member = null;

            var identifier = nodeId.Identifier.ToString();
            if (identifier.Contains('?'))
            {
                string[] parentnodeid = identifier.ToString().Split('?');
                if (parentnodeid.Count<string>() >= 2)
                {
                    if (Guid.TryParse(parentnodeid[0], out tagGuid) &&
                        Guid.TryParse(parentnodeid[1].Split('/').LastOrDefault(), out memberGuid))
                    {
                        instance = (from p in new XPQuery<UFUAModel.UFUATag>(session, true).AsParallel()
                                    where p.NodeId == tagGuid
                                    select p).FirstOrDefault();
                        if (instance != null)
                        {
                            if (instance.SubPrototypeMembers != null && instance.SubPrototypeMembers.Count > 0)
                            {
                                var members = (from p in new XPQuery<UFUAModel.UFUATag>(session, true).AsParallel()
                                               where p.NodeId == memberGuid && !p.UseShared.Value
                                               select p).ToList().AsParallel().Where(p =>
                                               p.IsSubPrototypeMember).ToList();
                                foreach (var tag in members)
                                {
                                    var instanceTag = tag.PrototypeReference.UFUATagOwner;
                                    while (instanceTag.IsSubPrototypeMember)
                                        instanceTag = instanceTag.PrototypeReference.UFUATagOwner;

                                    if (instanceTag == instance)
                                    {
                                        member = tag;
                                        break;
                                    }
                                };
                            }

                            if (member == null)
                            {
                                member = (from p in new XPQuery<UFUAModel.UFUATag>(session, true).AsParallel()
                                          where p.NodeId == memberGuid
                                          select p).ToList().AsParallel().Where(p =>
                                          !p.IsSubPrototypeMember).FirstOrDefault();
                            }
                        }
                    }
                }
            }
            else if (Guid.TryParse(identifier, out tagGuid))
            {
                instance = FindTagByNodeId(tagGuid, session);

                if (instance == null)
                {
                    member = (from p in new XPQuery<UFUAModel.UFUATag>(session, true).AsParallel()
                              where p.NodeId == tagGuid
                              select p).ToList().AsParallel().Where(p =>
                              !p.IsSubPrototypeMember).FirstOrDefault();
                }
            }

            return instance;
        }

        UFUAModel.UFUATag FindTagByNodeId(Guid guid, Session session, UFUAModel.UFUATagPrototype prototype = null)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(session, true).AsParallel()
                    where tag.NodeId == guid && tag.PrototypeReference == prototype
                    select tag).FirstOrDefault();
        }

#if !NET_STANDARD
        internal bool CheckVariable(string name, Dictionary<String, List<String>> nestedMemberList, out string refTagFound, bool getTag = false, bool clearCache = false)
        {
            refTagFound = null;
            bool result = true;
            if (string.IsNullOrEmpty(name))
                return true;

            using (CachedUnitOfWork task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                CacheTags mapTags = null;
                lock (lockObject)
                {
                    if (cachedMapTags.ContainsKey(task))
                    {
                        if (!clearCache)
                            mapTags = cachedMapTags[task];
                        else
                            cachedMapTags.Remove(task);
                    }
                }
                var tagString = name;
                if (mapTags == null)
                {
                    mapTags = InitCacheTags(task.UnitOfWork);
                    lock (lockObject)
                    {
                        task.KeepUnitOfWork = true;
                        cachedMapTags.Add(task, mapTags);
                    }
                }
                UFUAModel.UFUATag tagfound = null;
                if (mapTags.BackslashTags.ContainsKey(tagString))
                    tagfound = mapTags.BackslashTags[tagString];
                
                bool hasFoundWildChar = false;
                if (tagfound == null && tagString.EndsWith(NamespaceTableConverter.WholeFolderWildChar))
                {
                    var index = tagString.LastIndexOf(NamespaceTableConverter.WholeFolderWildChar);
                    if (index == 0 && mapTags.BackslashTags.Count > 0)
                        hasFoundWildChar = true;
                    else
                    {
                        var key = tagString.Substring(0, index);
                        var folder = (from b in mapTags.BackslashTags.Keys where b.StartsWith(key) select b).FirstOrDefault();
                        if (!string.IsNullOrEmpty(folder))
                            hasFoundWildChar = true;
                    }
                }
                if(tagfound != null && getTag)
                    refTagFound = GetTagOPCUAEntityReference(tagfound, null, false)?.ToXml();
                if (tagfound != null || hasFoundWildChar)
                    result = false;
                else
                {
                    string tagFoundPath = string.Empty;
                    var splits = tagString.Split('\\');
                    for (int i = splits.Length - 1; i > 0; --i)
                    {
                        String strToFind = String.Empty;
                        for (int j = 0; j < i; ++j)
                        {
                            if (String.IsNullOrEmpty(strToFind))
                                strToFind = splits[j];
                            else
                                strToFind = String.Format("{0}\\{1}", strToFind, splits[j]);
                        }

                        if (mapTags.BackslashTags.ContainsKey(strToFind))
                        {
                            tagFoundPath = strToFind;
                            tagfound = mapTags.BackslashTags[strToFind];
                            break;
                        }
                    }

                    if (tagfound != null)
                    {
                        if (tagfound.IsObjectType && tagfound.ModelType != UFUAModel.ModelType.Method)
                        {
                            if (nestedMemberList == null)
                            {
                                nestedMemberList = new Dictionary<String, List<String>>();

                                var plist = (from tag in new XPQuery<UFUAModel.UFUATagPrototype>(task.UnitOfWork, true).AsParallel()
                                             orderby tag.Oid
                                             select tag).ToList();
                                plist.ForEach(x =>
                                {
                                    if (!nestedMemberList.ContainsKey(x.Name))
                                        nestedMemberList.Add(x.Name, GetNestedMemberPath(x, nestedMemberList, task.UnitOfWork).ToList());
                                });
                            }

                            if (nestedMemberList.ContainsKey(tagfound.PrototypeName))
                            {
                                string relativePath = String.IsNullOrEmpty(tagfound.FolderPath) ? String.Format("{0}", tagfound.Name) : String.Format("{0}\\{1}", tagfound.FolderPath, tagfound.Name);
                                var ret = (from subPath in nestedMemberList[tagfound.PrototypeName]
                                 let newPath = relativePath + "\\" + subPath
                                 where newPath == name
                                 select newPath).FirstOrDefault();
                                if (!string.IsNullOrEmpty(ret) && getTag)
                                    refTagFound = GetTagOPCUAEntityReference(tagString.Substring(tagFoundPath.Length), relativePath, false)?.ToXml();
                                result = string.IsNullOrEmpty(ret);
                            }
                        }
                        else if (getTag)
                            refTagFound = GetTagOPCUAEntityReference(tagfound, null, false)?.ToXml();
                    }
                }
            }
            return result;
        }

        private IList<string> GetNestedMemberPath(UFUAModel.UFUATagPrototype p, Dictionary<String, List<String>> nlist, UnitOfWork uow)
        {
            List<string> list = new List<string>();
            //list.AddRange((from tag in p.Members select
            //               String.IsNullOrEmpty(tag.FolderPath) ? String.Format("{0}", tag.Name) : String.Format("{0}\\{1}", tag.FolderPath, tag.Name)).ToList());
            (from tag in p.Members
             select tag).ToList().ForEach(t =>
            {
                string relativepath = String.IsNullOrEmpty(t.FolderPath) ? String.Format("{0}", t.Name) : String.Format("{0}\\{1}", t.FolderPath, t.Name);
                list.Add(relativepath);
                if (t.IsObjectType && t.ModelType != UFUAModel.ModelType.Method)
                {
                    var prototypeName = t.PrototypeName;
                    var prototype = (from item in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true)
                                     where item.Name == prototypeName && item.UFUATagOwner == null
                                     select item).FirstOrDefault();
                    if (prototype != null)
                    {
                        if (nlist.ContainsKey(prototype.Name))
                        {
                            list.AddRange(
                                (from path in nlist[prototype.Name]
                                 select String.Format("{0}\\{1}", relativepath, path)).ToList()
                                 );
                        }
                        else
                        {
                            nlist.Add(prototype.Name, GetNestedMemberPath(prototype, nlist, uow).ToList());
                            list.AddRange(
                                (from path in nlist[prototype.Name]
                                 select String.Format("{0}\\{1}", relativepath, path)).ToList()
                                 );
                        }
                    }
                }
            });

            p.Folders.ToList().ForEach(folder => { list.AddRange(GetNestedMemberPath(folder, nlist, uow).ToList()); });
            return list;
        }

        private IList<string> GetNestedMemberPath(UFUAModel.UFUAFolder f, Dictionary<String, List<String>> nlist, UnitOfWork uow)
        {
            List<string> list = new List<string>();
            //list.AddRange((from tag in f.UFUATags select
            //               String.IsNullOrEmpty(tag.FolderPath) ? String.Format("{0}", tag.Name) : String.Format("{0}\\{1}", tag.FolderPath, tag.Name)).ToList());
            (from tag in f.UFUATags
             select tag).ToList().ForEach(t =>
             {
                 string relativepath = String.IsNullOrEmpty(t.FolderPath) ? String.Format("{0}", t.Name) : String.Format("{0}\\{1}", t.FolderPath, t.Name);
                 list.Add(relativepath);
                 if (t.IsObjectType && t.ModelType != UFUAModel.ModelType.Method)
                 {
                     var prototypeName = t.PrototypeName;
                     var prototype = (from item in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true)
                                      where item.Name == prototypeName && item.UFUATagOwner == null
                                      select item).FirstOrDefault();
                     if (prototype != null)
                     {
                         if (nlist.ContainsKey(prototype.Name))
                             list.AddRange(
                                 (from path in nlist[prototype.Name]
                                  select String.Format("{0}\\{1}", relativepath, path)).ToList()
                                  );
                         else
                         {
                             nlist.Add(prototype.Name, GetNestedMemberPath(prototype, nlist, uow).ToList());
                             list.AddRange((from path in nlist[prototype.Name] select String.Format("{0}\\{1}", relativepath, path)).ToList());
                         }
                     }
                 }
             });

            f.UFUAFolders.ToList().ForEach(folder => { list.AddRange(GetNestedMemberPath(folder, nlist, uow)); });
            return list;
        }

        internal IList<String> GetFlatListAlarmSources(UFUAModel.UFUAArea root = null)
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ret = new List<String>();
                var areas = GetAlarmAreas(root);
                foreach (var area in areas)
                {
                    var name = area.GetRelativeName();
                    ret.Add(name);

                    var sources = (from c in area.UFUAAlarmSources.AsParallel()
                                   orderby c.Name
                                   select c.Name).ToList();
                    foreach (var source in area.UFUAAlarmSources)
                        ret.Add(source.GetRelativeName());
                    ret.AddRange(GetFlatListAlarmSources(area));
                }

                return ret;
            }
        }

        internal IList<String> GetFlatFullTagNameCollectionOrderByName(bool onlyHistorical = false, bool bForceRefresh = false)
        {
            lock (tagsListLock)
            {
                var cachedList = onlyHistorical ? listFlatFullHistoricalTagNameCollection : listFlatFullTagNameCollection;
                if (cachedList == null || bForceRefresh)
                {
                    using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                    {
                        List<UFUAModel.UFUATag> _list = (from tag in new XPQuery<UFUAModel.UFUATag>(task.UnitOfWork, true).AsParallel()
                                                         select tag).ToList().AsParallel().Where(tag =>
                                                         !tag.IsPrototypeMember).ToList();
                        List<string> resultH = new List<string>();
                        List<string> result = new List<string>();
                        _list.ForEach(tag =>
                        {
                            string relativePath = String.IsNullOrEmpty(tag.FolderPath) ? String.Format("{0}", tag.Name) : String.Format("{0}\\{1}", tag.FolderPath, tag.Name);
                            result.Add(relativePath);
                            if (!String.IsNullOrEmpty(tag.HistorianSettings))
                                resultH.Add(relativePath);
                            if (!String.IsNullOrEmpty(tag.PrototypeModel) && tag.ModelType != UFUAModel.ModelType.Method)
                            {
                                result.AddRange(GetFlatFullTagNameCollection(tag.PrototypeModel, relativePath, true, false, true));
                                resultH.AddRange(GetFlatFullTagNameCollection(tag.PrototypeModel, relativePath, true, true, true));
                            }
                        });

                        result.Sort();
                        resultH.Sort();

                        if (listFlatFullTagNameCollection == null)
                            listFlatFullTagNameCollection = new List<string>();
                        else
                            listFlatFullTagNameCollection.Clear();
                        listFlatFullTagNameCollection.AddRange(result);

                        if (listFlatFullHistoricalTagNameCollection == null)
                            listFlatFullHistoricalTagNameCollection = new List<string>();
                        else
                            listFlatFullHistoricalTagNameCollection.Clear();
                        listFlatFullHistoricalTagNameCollection.AddRange(resultH);
                    }
                }

                if (listFlatFullTagNameCollectionAdded != null || listFlatFullTagNameCollectionRemoved != null)
                {
                    if (listFlatFullTagNameCollectionAdded != null)
                    {
                        listFlatFullTagNameCollectionAdded.ForEach((tagName) =>
                        {
                            if (!listFlatFullTagNameCollection.Contains(tagName))
                                listFlatFullTagNameCollection.Add(tagName);
                        });
                    }
                    if (listFlatFullTagNameCollectionRemoved != null)
                        listFlatFullTagNameCollectionRemoved.ForEach((tagName) => listFlatFullTagNameCollection.Remove(tagName));

                    listFlatFullTagNameCollectionAdded = null;
                    listFlatFullTagNameCollectionRemoved = null;
                    listFlatFullTagNameCollection.Sort();
                }

                if (listFlatFullHistoricalTagNameCollectionAdded != null || listFlatFullHistoricalTagNameCollectionRemoved != null)
                {
                    if (listFlatFullHistoricalTagNameCollectionAdded != null)
                    {
                        listFlatFullHistoricalTagNameCollectionAdded.ForEach((tagName) =>
                        {
                            if (!listFlatFullHistoricalTagNameCollection.Contains(tagName))
                                listFlatFullHistoricalTagNameCollection.Add(tagName);
                        });
                    }
                    if (listFlatFullHistoricalTagNameCollectionRemoved != null)
                        listFlatFullHistoricalTagNameCollectionRemoved.ForEach((tagName) => listFlatFullHistoricalTagNameCollection.Remove(tagName));

                    listFlatFullHistoricalTagNameCollectionAdded = null;
                    listFlatFullHistoricalTagNameCollectionRemoved = null;
                    listFlatFullHistoricalTagNameCollection.Sort();
                }

                return onlyHistorical ? listFlatFullHistoricalTagNameCollection : listFlatFullTagNameCollection;
            }
        }

        internal IList<String> GetFlatFullFolderNameCollectionOrderByName(bool bForceRefresh = false)
        {
            lock (tagsListLock)
            {
                if (listFlatFullFolderNameCollection == null || bForceRefresh)
                {
                    using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                    {
                        List<UFUAModel.UFUAFolder> _list = (from folder in new XPQuery<UFUAModel.UFUAFolder>(task.UnitOfWork, true).AsParallel()
                                                            select folder).ToList().AsParallel().Where(folder =>
                                                            !folder.IsPrototypeMember && !String.IsNullOrEmpty(folder.Name)).ToList();
                        if (listFlatFullFolderNameCollection == null)
                            listFlatFullFolderNameCollection = new List<string>();
                        else
                            listFlatFullFolderNameCollection.Clear();
                        _list.ForEach(folder =>
                        {
                            if (!listFlatFullFolderNameCollection.Contains(folder.PathIdentifier))
                                listFlatFullFolderNameCollection.Add(folder.PathIdentifier);
                        });
                        listFlatFullFolderNameCollection.Sort();
                    }
                }

                if (listFlatFullFolderNameCollectionAdded != null || listFlatFullFolderNameCollectionRemoved != null)
                {
                    if (listFlatFullFolderNameCollectionAdded != null)
                    {
                        listFlatFullFolderNameCollectionAdded.ForEach((tagName) =>
                        {
                            if (!listFlatFullFolderNameCollection.Contains(tagName))
                                listFlatFullFolderNameCollection.Add(tagName);
                        });
                    }
                    if (listFlatFullFolderNameCollectionRemoved != null)
                        listFlatFullFolderNameCollectionRemoved.ForEach((tagName) => listFlatFullFolderNameCollection.Remove(tagName));

                    listFlatFullFolderNameCollectionAdded = null;
                    listFlatFullFolderNameCollectionRemoved = null;
                    listFlatFullFolderNameCollection.Sort();
                }

                return listFlatFullFolderNameCollection;
            }
        }

        protected NodeId FromGuidToNodeId(Guid from)
        {
            string relativepath = string.Empty;
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);

            return new NodeId(from, ns);
        }

        internal IDictionary<String, NodeId> GetFlatFullTagNameNodeIdCollection()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var _list = (from tag in new XPQuery<UFUAModel.UFUATag>(task.UnitOfWork, true).AsParallel()
                             orderby tag.Oid
                             select tag).ToList().AsParallel().Where(tag =>
                             !tag.IsPrototypeMember).ToList();

                var result = new Dictionary<String, NodeId>();
                _list.ForEach(tag =>
                {
                    string relativePath = String.IsNullOrEmpty(tag.FolderPath) ? String.Format("{0}", tag.Name) : String.Format("{0}\\{1}", tag.FolderPath, tag.Name);
                    if (!result.ContainsKey(relativePath))
                        result.Add(relativePath, FromGuidToNodeId(tag.NodeId));
                });
                return result;
            }
        }
#endif

        internal IList<String> GetFlatFullTagNameCollection(String stag, String _relativePath, bool first = false, bool onlyHistorical = false, bool inExecution = false)
        {
            CachedUnitOfWork task = null;
            if (inExecution)
                task = cachedUnitOfWorks.BeginUnitOfWork();
            try
            {
                var _uow = task != null ? task.UnitOfWork : uow;
                List<UFUAModel.UFUATag> ptaglist = (from tag in new XPQuery<UFUAModel.UFUATag>(_uow, true).AsParallel()
                                                    orderby tag.Oid
                                                    select tag).ToList().AsParallel().Where(tag =>
                                                    tag.IsPrototypeMember && !tag.IsSubPrototypeMember &&
                                                    tag.PrototypeReference.NodeId.ToString() == stag
                                                    ).ToList();
                List<string> result = new List<string>();

                ptaglist.ForEach(ptag =>
                {
                    try
                    {
                        string _cpath = String.IsNullOrEmpty(ptag.FolderPath) ? String.Format("{0}", ptag.Name) : String.Format("{0}\\{1}", ptag.FolderPath, ptag.Name);
                        string relativePath = _relativePath + (first ? ":" : "\\") + _cpath;
                        if (!onlyHistorical || !String.IsNullOrEmpty(ptag.HistorianSettings))
                        {
                            result.Add(relativePath);
                        }

                        if (!String.IsNullOrEmpty(ptag.PrototypeModel) && ptag.ModelType != UFUAModel.ModelType.Method)
                            result.AddRange(GetFlatFullTagNameCollection(ptag.PrototypeModel, relativePath, false, onlyHistorical, inExecution));
                    }
                    catch (Exception)
                    {
                    }
                });

                return result;
            }
            finally
            {
                if (task != null)
                    task.Dispose();
            }
        }

        internal IDictionary<String, IList<String>> GetFlatListPrototypes()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var map = new Dictionary<String, IList<String>>();
                (from tag in new XPQuery<UFUAModel.UFUATagPrototype>(task.UnitOfWork, true).AsParallel()
                 where tag.UFUATagOwner == null
                 orderby tag.Oid
                 select tag).ToList().ForEach(prototype =>
                 {
                     var members = (from tag in prototype.Members select tag.Name).ToList();
                     if (!map.ContainsKey(prototype.Name))
                         map.Add(prototype.Name, members);
                 });

                return map;
            }
        }

#if !NET_STANDARD
        internal IList<String> GetFlatListPrototypes(string name)
        {
            List<String> map = new List<String>();
            var prototype = (from tag in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                             where tag.Name == name && tag.UFUATagOwner == null
                             select tag).FirstOrDefault();

            if (prototype != null)
            {
                var members = (from tag in prototype.Members select tag.Name).ToList();
                return members;
            }
            else
                return null;
        }

        internal IDictionary<String, String> GetListNodeNames(IList<String> nodes)
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var map = new Dictionary<String, String>();
                var listTags = (from tag in new XPQuery<UFUAModel.UFUATag>(task.UnitOfWork, true).AsParallel()
                                select tag).ToList().AsParallel().Where(tag => tag.PrototypeReference == null).ToList();
                foreach (var _node in nodes)
                {
                    bool addInMap = false;
                    var node = _node;
                    var node1 = string.Empty;
                    var sub_node1 = string.Empty;
                    string ns = string.Empty;
                    int p = node.IndexOf("?");
                    if (p != -1)
                    {
                        node = _node.Substring(0, p);
                        node = node.Replace(";s=", ";g=");
                        sub_node1 = $"{_node.Substring(p + 1)}";
                        ns = node.Substring(0, node.LastIndexOf('=') + 1);
                    }

                    NodeId nodeid = null;
                    try
                    {
                        nodeid = NodeId.Parse(node);
                    }
                    catch
                    { }
                    if (nodeid == null || nodeid.IdType != IdType.Guid)
                        continue;
                    var guid = (Guid)nodeid.Identifier;
                    var list = (from tag in listTags.AsParallel()
                                where tag.NodeId == guid
                                select tag).ToList();
                    if (list.Count > 0 && !map.ContainsKey(_node))
                    {
                        var relPath = list[0].GetRelativeName();
                        if (!string.IsNullOrEmpty(sub_node1))
                        {
                            if (sub_node1.Contains('/'))
                            {
                                addInMap = TryUpdateRelativePath(ref relPath, ns, sub_node1, task.UnitOfWork);
                            }
                            else
                            {
                                node1 = $"{ns}{sub_node1}";
                                var mainPrototype = GetPrototype(list[0].PrototypeName, task.UnitOfWork);
                                if (mainPrototype == null)
                                    continue;
                                var proto = list[0].SubPrototypeMembers.Count > 0 ? list[0].SubPrototypeMembers[0] : null;//CreateSubPrototype(list[0], _uow: task.UnitOfWork);
                                Dictionary<string, UFUAModel.UFUATag> ptaglist = proto?.GetTagMembers().ToDictionary(x => x.NodeId.ToString(), x => x);
                                Dictionary<string, UFUAModel.UFUATag> maintaglist = mainPrototype.GetTagMembers().ToDictionary(x => x.NodeId.ToString(), x => x);
                                if (ptaglist == null)
                                    ptaglist = maintaglist;
                                else
                                {
                                    maintaglist.Keys.ToList().ForEach(id =>
                                    {
                                        if (!ptaglist.ContainsKey(id))
                                            ptaglist.Add(id, maintaglist[id]);
                                    });
                                }
                                if (IsValidNodeID(node1))
                                {
                                    var list1 = (from tag in ptaglist.Values.AsParallel()
                                                    where tag.NodeId.ToString() == sub_node1
                                                    select tag).ToList();
                                    if (list1.Count > 0)
                                    {
                                        relPath = $"{relPath}:{list1[0].Name}"; 
                                        addInMap = true;
                                    }
                                }
                            }
                        }
                        else
                            addInMap = true;

                        if (addInMap)
                            map.Add(_node, relPath.Replace("/", "\\"));
                    }
                }

                return map;
            }
        }

        private bool IsValidNodeID(string node1)
        {
            try
            {
                var nodeid1 = (Guid)NodeId.Parse(node1).Identifier;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryUpdateRelativePath(ref string relPath, string ns, string sub_node1, UnitOfWork uow)
        {
            string lastnode = sub_node1;
            if (sub_node1.Contains('/'))
            {
                var p1 = sub_node1.LastIndexOf("/");
                lastnode = $"{sub_node1.Substring(p1 + 1)}";
                relPath = $"{relPath}\\{sub_node1.Substring(0, p1)}";
                var node1 = $"{ns}{lastnode}";
                if (IsValidNodeID(node1))
                {
                    UFUAModel.UFUATag tag = (from t in new XPQuery<UFUAModel.UFUATag>(uow, true)/*.AsParallel()*/
                                             where t.NodeId.ToString() == lastnode
                                             select t).FirstOrDefault();
                    if (tag != null)
                    {
                        relPath = $"{relPath}\\{tag.Name}";
                        return true;
                    }
                }
                else
                {
                    relPath = $"{relPath}\\{lastnode}";
                    return true;
                }
            }
            else
            {
                relPath = $"{relPath}\\{lastnode}";
                return true;
            }
            return false;
        }
#endif

        internal IDictionary<String, String> GetFlatListPrototypeInstances()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var listTag = (from tag in new XPQuery<UFUAModel.UFUATag>(task.UnitOfWork, true).AsParallel() select tag).ToList();
                var map = new Dictionary<String, String>();
                Parallel.ForEach(listTag, tag =>
                    {
                        if (!tag.IsPrototypeMember && tag.ModelType != UFUAModel.ModelType.Method)
                        {
                            var name = String.IsNullOrEmpty(tag.FolderPath) ?
                                String.Format("{0}", tag.Name) :
                                String.Format("{0}\\{1}", tag.FolderPath, tag.Name);
                            lock (map)
                            {
                                if (!map.ContainsKey(name))
                                    map.Add(name, tag.PrototypeName);
                            }
                        }
                    });




                return map;
            }
        }

#if !NET_STANDARD
        internal IList<UFUAModel.UFUATag> GetFlatPrototypeMemberCollection()
        {
            List<UFUAModel.UFUATag> list = new List<UFUAModel.UFUATag>();
            var plist = (from tag in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                         where tag.UFUATagOwner == null
                         orderby tag.Oid
                         select tag).ToList();
            plist.ForEach(x =>
            {
                list.AddRange(GetNestedMembers(x));
            });
            return list;
        }

        private IList<UFUAModel.UFUATag> GetNestedMembers(UFUAModel.UFUATagPrototype p)
        {
            List<UFUAModel.UFUATag> list = new List<UFUAModel.UFUATag>();
            list.AddRange((from tag in p.Members select tag).ToList());
            p.Folders.ToList().ForEach(folder => { list.AddRange(GetNestedMembers(folder)); });
            return list;
        }

        private IList<UFUAModel.UFUATag> GetNestedMembers(UFUAModel.UFUAFolder f)
        {
            List<UFUAModel.UFUATag> list = new List<UFUAModel.UFUATag>();
            list.AddRange(f.UFUATags.ToList());
            f.UFUAFolders.ToList().ForEach(folder => { list.AddRange(GetNestedMembers(folder)); });
            return list;
        }
#endif

        void SetDefaultLocalEndpoint(String str)
        {
            lock (lockObject)
            {
                if (cachedDefaultLocalEndpoint.ContainsKey(Thread.CurrentThread))
                    cachedDefaultLocalEndpoint.Remove(Thread.CurrentThread);
                cachedDefaultLocalEndpoint.Add(Thread.CurrentThread, str);
            }
        }

        internal List<string> GetEndpoints()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true)/*.AsParallel()*/ select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFUAModel.UFUAConfiguration(task.UnitOfWork);
                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);

                var endpoints = (from ba in ufuaConfiguration.BaseAddresses/*.AsParallel()*/
                                 where ba.Enabled == true
                                 select ba.Path).ToList();

                return endpoints;
            }
        }

        internal string GetDefaultLocalEndpoint(bool refresh = false)
        {
            if(!refresh)
                lock (lockObject)
                {
                    if (cachedDefaultLocalEndpoint.ContainsKey(Thread.CurrentThread))
                        return cachedDefaultLocalEndpoint[Thread.CurrentThread];
                }

            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true)/*.AsParallel()*/ select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFUAModel.UFUAConfiguration(task.UnitOfWork);
                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);

                var endpoints = (from ba in ufuaConfiguration.BaseAddresses/*.AsParallel()*/
                                 where ba.Enabled == true
                                 select ba).ToList();

                if (endpoints.Count > 0)
                {
                    foreach (var transport in transportOrderByRelevance)
                    {
                        var endpoint = (from ba in endpoints
                                        where ba.Transport == transport
                                        select ba).ToList();

                        if (endpoint.Count > 0)
                        {
                            SetDefaultLocalEndpoint(endpoint[0].Path);
                            return endpoint[0].Path;
                        }
                    }

                    if (endpoints.Count > 0)
                    {
                        SetDefaultLocalEndpoint(endpoints[0].Path);
                        return endpoints[0].Path;
                    }
                }

                var listadd = UFUAServerInfo.UFUAServerInfo.GetCurrentApplicationBaseAddresses();
                if (listadd != null && listadd.Count > 0)
                {
                    SetDefaultLocalEndpoint(listadd[0]);
                    return listadd[0];
                }

                return string.Empty;
            }
        }

        internal string GetRelativePath(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                
                var find = String.Format("{0}:{1}\\", ns, UFUAServerInfo.UFUAServerInfo.GetTagRootName());
                var ret = value;
                if (ret.StartsWith(find, true, CultureInfo.CurrentCulture))
                    ret = ret.Substring(find.Length);
                find = String.Format("{0}:{1}/", ns, UFUAServerInfo.UFUAServerInfo.GetTagRootName());
                if (ret.StartsWith(find, true, CultureInfo.CurrentCulture))
                    ret = ret.Substring(find.Length);

                string oldChars = string.Format("{0}:", ns);
                ret = ret.Replace(oldChars, "");
                ret = ret.Replace("/", "\\");
                return ret;
            }

            return String.Empty;
        }

        internal OPCUAEntityReference GetTagOPCUAEntityReference(UFUATag tagFound, IList<UFUATag> parentTags = null, bool inExecution = false, bool refresh = false)
        {
            var applicationName = GetAplicationName(refresh);
            var tc = UFUAModel.Helpers.TagComponentsHelper.GetUFUATagComponents(applicationName, tagFound, parentTags);
            var ns = Utilities.TagPathHelper.GetNS();
            OPCUAEntityReference entityreference = null;
            entityreference = new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(refresh),
                                tc.RelativePath, tc.NodeId, tc.HumanReadable, null, tc.RelativePath, bAllowAutoRedundancy: inExecution ? tagFound.IsRedundancyEnabled.Value : true);

            if (tc.TagParent != null && !String.IsNullOrEmpty(tc.TagParent.PrototypeModel))
            {
                var foundPrototypeName = tc.TagParent.PrototypeName;
                CachedUnitOfWork task = cachedUow;
                if (inExecution)
                    task = cachedUnitOfWorks.BeginUnitOfWork();
                try
                {
                    UnitOfWork uow = task != null ? task.UnitOfWork : this.uow;
                    var prototypeFound = (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                                          where prototype.Name == foundPrototypeName && prototype.UFUATagOwner == null
                                          select prototype).ToList();
                    if (prototypeFound.Count > 0)
                    {
                        entityreference.ParentNodeId = new Opc.Ua.NodeId(prototypeFound[0].NodeId, ns);
                        entityreference.ParentTypeDefinitionNameList = new List<String>() { foundPrototypeName };
                        entityreference.TypeDefinitionNodeId = new Opc.Ua.NodeId(prototypeFound[0].NodeId, ns);
                        entityreference.TypeDefinitionName = foundPrototypeName;

                        entityreference.ParentTypeDefinitionName = tc.TagParent.PrototypeModel;
                    }
                }
                finally
                {
                    if (task != null && task != cachedUow)
                        task.Dispose();
                }
            }
            else if (tagFound != null && !String.IsNullOrEmpty(tagFound.PrototypeModel))
            {
                var foundPrototypeName = tagFound.PrototypeName;
                CachedUnitOfWork task = cachedUow;
                if (inExecution)
                    task = cachedUnitOfWorks.BeginUnitOfWork();
                try
                {
                    UnitOfWork uow = task != null ? task.UnitOfWork : this.uow;
                    var prototypeFound = (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                                          where prototype.Name == foundPrototypeName && prototype.UFUATagOwner == null
                                          select prototype).ToList();
                    if (prototypeFound.Count > 0)
                    {
                        entityreference.TypeDefinitionNodeId = new Opc.Ua.NodeId(prototypeFound[0].NodeId, ns);
                        entityreference.TypeDefinitionName = foundPrototypeName;

                        entityreference.ParentTypeDefinitionName = tagFound.PrototypeModel;
                    }
                }
                finally
                {
                    if (task != null && task != cachedUow)
                        task.Dispose();
                }
            }
            return entityreference;
        }

        public OPCUAEntityReference GetTagOPCUAEntityReference(UFUAModel.UFUATag tagFound, UFUAModel.UFUATag member = null)
        {
            String relativepath = null;
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);

            Opc.Ua.NodeId nodeid;
            var humanReadable = tagFound.GetRelativeName().Replace('/', '\\');
            if (member == null)
            {
                nodeid = new Opc.Ua.NodeId(tagFound.NodeId, ns);
                relativepath = tagFound.GetRelativePath(ns);
            }
            else
            {
                if (String.IsNullOrEmpty(member.FolderPath))
                    nodeid = new Opc.Ua.NodeId(String.Format("{0}?{1}", tagFound.NodeId, member.NodeId), ns);
                else
                    nodeid = new Opc.Ua.NodeId(String.Format("{0}?{1}/{2}", tagFound.NodeId, member.FolderPath, member.NodeId), ns);

                //relativepath = String.Format("/{1}:{0}", member.NodeId, ns);
                relativepath = String.Format("{1}/{0}", member.GetRelativePath(ns).Substring(string.Format("{0}:{1}/", ns, UFUAServerInfo.UFUAServerInfo.GetTagRootName()).Length), tagFound.GetRelativePath(ns));

                humanReadable = String.Format("{0}:{1}", humanReadable, member.GetRelativeName().Replace('/', '\\'));
            }

            var applicationName = GetAplicationName();
            OPCUAEntityReference entityreference = null;
            entityreference = new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                            relativepath, nodeid, string.Format("{0} ({1})", humanReadable, applicationName), null, relativepath);
            if (!String.IsNullOrEmpty(tagFound.PrototypeModel))
            {
                var foundPrototypeName = tagFound.PrototypeName;
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var prototypeFound = (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(task.UnitOfWork, true).AsParallel()
                                          where prototype.Name == foundPrototypeName && prototype.UFUATagOwner == null
                                          select prototype).ToList();
                    if (prototypeFound.Count > 0)
                    {
                        entityreference.ParentNodeId = new Opc.Ua.NodeId(prototypeFound[0].NodeId, ns);
                        entityreference.ParentTypeDefinitionNameList = new List<String>() { foundPrototypeName };
                        entityreference.TypeDefinitionNodeId = new Opc.Ua.NodeId(prototypeFound[0].NodeId, ns);
                        entityreference.TypeDefinitionName = foundPrototypeName;
                    }
                }
            }

            return entityreference;
        }

        public UFUAModel.UFUATag GetUFUATag(string tagname, string membername, bool bForceSubPrototypeCreation)
        {
            var tagString = tagname;
            var instance = string.Empty;
            if (!string.IsNullOrEmpty(membername))
            {
                tagString = membername;
                instance = tagname;
            }
            return GetUFUATag(ref tagString, ref instance, inExecution: false, bForceSubPrototypeCreation: bForceSubPrototypeCreation);
        }

        private UFUATag GetUFUATag(ref String tagName, ref String instance, bool inExecution, List<UFUAModel.UFUATag> list = null, bool bForceSubPrototypeCreation = false)
        {
            tagName = GetRelativePath(tagName);

            if (inExecution)
            {
                lock (lockObject)
                {
                    var composedName = String.Format("{0}:{1}", instance ?? String.Empty, tagName);
                    if (notFoundTagNames == null)
                        notFoundTagNames = new List<String>();
                    if (notFoundTagNames.Contains(composedName))
                        return null;
                }
            }

            var applicationName = GetAplicationName();
            CachedUnitOfWork task = cachedUow;
            if (inExecution)
                task = cachedUnitOfWorks.BeginUnitOfWork();
            try
            {

                var tagString = tagName;
                String member = null;

                if (!String.IsNullOrEmpty(instance))
                {
                    tagString = instance;
                    member = tagName;
                }

                UFUAModel.UFUATag tagfound = null;
                if (task != null)
                {
                    CacheTags mapTags = null;
                    lock (lockObject)
                    {
                        if (cachedMapTags.ContainsKey(task))
                            mapTags = cachedMapTags[task];
                    }

                    if (mapTags == null)
                    {
                        mapTags = InitCacheTags(task.UnitOfWork);
                        lock (lockObject)
                        {
                            task.KeepUnitOfWork = true;
                            cachedMapTags.Add(task, mapTags);
                        }
                    }

                    if (mapTags.BackslashTags.ContainsKey(tagString))
                        tagfound = mapTags.BackslashTags[tagString];
                    else if (tagString.Contains('\\'))
                    {
                        var splits = tagString.Split('\\');
                        for (int i = splits.Length - 1; i > 0; --i)
                        {
                            String strToFind = String.Empty;
                            for (int j = 0; j < i; ++j)
                            {
                                if (String.IsNullOrEmpty(strToFind))
                                    strToFind = splits[j];
                                else
                                    strToFind = String.Format("{0}\\{1}", strToFind, splits[j]);
                            }

                            if (mapTags.BackslashTags.ContainsKey(strToFind))
                            {
                                tagfound = mapTags.BackslashTags[strToFind];
                                instance = strToFind;
                                member = tagString.Replace(String.Format("{0}\\", strToFind), "");
                                break;
                            }
                        }

                        //var instanceCheck = tagString.Substring(0, tagString.LastIndexOf("\\"));
                        //if (mapTags.BackslashTags.ContainsKey(instanceCheck))
                        //{
                        //    tagfound = mapTags.BackslashTags[instanceCheck];
                        //    instance = instanceCheck;
                        //    member = tagString.Replace(String.Format("{0}\\", instanceCheck), "");
                        //}
                    }
                    else if (tagString.Contains('_'))
                    {
                        if (mapTags.UnderscoreTags.ContainsKey(tagString))
                            tagfound = mapTags.UnderscoreTags[tagString];
                    }
                }

                UnitOfWork uow = task != null ? task.UnitOfWork : this.uow;
                if (tagfound == null)
                {
                    List<UFUAModel.UFUATag> listtagfound = null;
                    if (tagString.Contains('\\'))
                    {
                        listtagfound = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                        select tag).ToList().AsParallel().Where(tag =>
                                        String.Compare(String.Format("{0}\\{1}", tag.FolderPath, tag.Name), tagString, true) == 0 &&
                                        !tag.IsPrototypeMember).ToList();
                        if (listtagfound.Count > 0)
                            tagfound = listtagfound[0];
                        else
                        {
                            listtagfound = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                            select tag).ToList().AsParallel().Where(tag =>
                                            (String.Compare(String.Format("{0}\\{1}", tag.FolderPath, tag.OriginalName), tagString, true) == 0 ||
                                            String.Compare(String.Format("{0}\\{1}", tag.OriginalFolderPath, tag.Name), tagString, true) == 0 ||
                                            String.Compare(String.Format("{0}\\{1}", tag.OriginalFolderPath, tag.OriginalName), tagString, true) == 0) &&
                                            !tag.IsPrototypeMember).ToList();
                            if (listtagfound.Count > 0)
                                tagfound = listtagfound[0];
                        }
                    }
                    else
                    {
                        listtagfound = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                        where String.Compare(tag.Name, tagString, true) == 0
                                        select tag).ToList().AsParallel().Where(tag =>
                                        String.IsNullOrEmpty(tag.FolderPath) &&
                                        !tag.IsPrototypeMember).ToList();
                        if (listtagfound.Count > 0)
                            tagfound = listtagfound[0];
                        else
                        {
                            listtagfound = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                            where String.Compare(tag.OriginalName, tagString, true) == 0
                                            select tag).ToList().AsParallel().Where(tag =>
                                            String.IsNullOrEmpty(tag.FolderPath) &&
                                            !tag.IsPrototypeMember).ToList();
                            if (listtagfound.Count > 0)
                                tagfound = listtagfound[0];
                        }
                    }
                }

                if (tagfound == null && tagString.Contains('_'))
                {
                    List<UFUAModel.UFUATag> listtagfound = null;
                    listtagfound = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                    select tag).ToList().AsParallel().Where(tag =>
                                    String.Compare(String.Format("{0}\\{1}", tag.FolderPath, tag.Name).Replace('\\', '_'), tagString, true) == 0 &&
                                    !tag.IsPrototypeMember).ToList();

                    if (listtagfound.Count > 0)
                        tagfound = listtagfound[0];
                    else
                    {
                        listtagfound = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                        select tag).ToList().AsParallel().Where(tag =>
                                        (String.Compare(String.Format("{0}\\{1}", tag.FolderPath, tag.OriginalName).Replace('\\', '_'), tagString, true) == 0 ||
                                        String.Compare(String.Format("{0}\\{1}", tag.OriginalFolderPath, tag.Name).Replace('\\', '_'), tagString, true) == 0 ||
                                        String.Compare(String.Format("{0}\\{1}", tag.OriginalFolderPath, tag.OriginalName).Replace('\\', '_'), tagString, true) == 0) &&
                                        !tag.IsPrototypeMember).ToList();
                        if (listtagfound.Count > 0)
                            tagfound = listtagfound[0];
                    }
                }

                if (tagfound != null)
                {
                    tagfound = GetInstanceTag(tagfound, member, instance, list, uow, inExecution, bForceSubPrototypeCreation);
                    if (tagfound != null)
                    {
                        return tagfound;
                    }
                }
            }
            finally
            {
                if (task != null && task != cachedUow)
                    task.Dispose();
            }

            if (inExecution)
            {
                lock (lockObject)
                {
                    var composedName = String.Format("{0}:{1}", instance ?? String.Empty, tagName);
                    if (notFoundTagNames == null)
                        notFoundTagNames = new List<String>();
                    if (!notFoundTagNames.Contains(composedName))
                        notFoundTagNames.Add(composedName);
                }
            }

            return null;
        }

        internal CacheTags InitCacheTags(UnitOfWork _uow = null)
        {
            var mapTags = new CacheTags();
            UnitOfWork tuow = _uow != null ? _uow : uow;
            foreach (var tag in new XPQuery<UFUAModel.UFUATag>(tuow, true))
            {
                if (tag.IsPrototypeMember)
                    continue;

                if (!String.IsNullOrEmpty(tag.FolderPath))
                {
                    var s = String.Format("{0}\\{1}", tag.FolderPath, tag.Name);
                    if (!mapTags.BackslashTags.ContainsKey(s))
                        mapTags.BackslashTags.Add(s, tag);
                    s = s.Replace('\\', '_');
                    if (!mapTags.UnderscoreTags.ContainsKey(s))
                        mapTags.UnderscoreTags.Add(s, tag);
                }
                else if (!mapTags.BackslashTags.ContainsKey(tag.Name))
                {
                    mapTags.BackslashTags.Add(tag.Name, tag);
                }

                if (!String.IsNullOrEmpty(tag.OriginalFolderPath))
                {
                    var s = String.Format("{0}\\{1}", tag.OriginalFolderPath, tag.Name);
                    if (!mapTags.BackslashTags.ContainsKey(s))
                        mapTags.BackslashTags.Add(s, tag);
                    s = s.Replace('\\', '_');
                    if (!mapTags.UnderscoreTags.ContainsKey(s))
                        mapTags.UnderscoreTags.Add(s, tag);
                    if (!String.IsNullOrEmpty(tag.OriginalName))
                    {
                        s = String.Format("{0}\\{1}", tag.OriginalFolderPath, tag.OriginalName);
                        if (!mapTags.BackslashTags.ContainsKey(s))
                            mapTags.BackslashTags.Add(s, tag);
                        s = s.Replace('\\', '_');
                        if (!mapTags.UnderscoreTags.ContainsKey(s))
                            mapTags.UnderscoreTags.Add(s, tag);
                    }
                }
            }
            return mapTags;
        }

        internal UFUATag GetInstanceTag(UFUATag tagfound, string memberName, string instance, List<UFUATag> list = null, UnitOfWork _uow = null, bool inExecution = true, bool bForceSubPrototypeCreation = false)
        {
            if (String.IsNullOrEmpty(instance))
                return tagfound;
            UnitOfWork tuow = _uow != null ? _uow : uow;
            if (list == null)
                list = new List<UFUAModel.UFUATag>();
            if (!string.IsNullOrEmpty(memberName))
            {
                list.Insert(0, tagfound);
                var steps = memberName.Split('\\');
                UFUAModel.UFUAFolder currFolder = null;
                foreach (var step in steps)
                {
                    if (tagfound != null && tagfound.ModelType == UFUAModel.ModelType.ObjectType &&
                        !String.IsNullOrEmpty(tagfound.PrototypeName))
                    {
                        UFUATagPrototype tagPrototypeFound = null;
#if !NET_STANDARD
                        if (cachedUow != null || (bForceSubPrototypeCreation && tuow != null))
                        {
                            if (tagfound.IsSubPrototypeMembersCreated)
                                tagPrototypeFound = tagfound.SubPrototypeMembers[0]; 
                            else
                                tagPrototypeFound = CreateSubPrototype(tagfound, _uow: tuow);
                        }
#endif
                        List<UFUAModel.UFUATagPrototype> prototypeList = null;
                        if (tagPrototypeFound != null)
                        {
                            prototypeList = new List<UFUATagPrototype>();
                            prototypeList.Add(tagPrototypeFound);
                        }
                        else
                        {
                            var prototypeName = tagfound.PrototypeName;
                            prototypeList = (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(tuow, true).AsParallel()
                                             where prototype.Name == prototypeName && prototype.UFUATagOwner == null
                                             select prototype).ToList();
                        }
                        if (prototypeList.Count > 0)
                        {
                            var prototypeFound = prototypeList[0];
                            if (prototypeFound != null)
                            {
                                prototypeFound.EnsureUniqueMembersOrderId();

                                var MemberList = (from c in prototypeFound.Members
                                                      // where c.Name == step
                                                  where String.Compare(c.Name, step, true) == 0
                                                  select c).ToList();
                                if (MemberList.Count > 0)
                                {
                                    tagfound = MemberList[0];
                                    list.Insert(0, tagfound);
                                    continue;
                                }
                                var FolderList = (from c in prototypeFound.Folders
                                                      // where c.Name == step
                                                  where String.Compare(c.Name, step, true) == 0
                                                  select c).ToList();
                                if (FolderList.Count > 0)
                                {
                                    currFolder = FolderList[0];
                                    tagfound = null;
                                    continue;
                                }
                            }
                        }
                    }
                    if (currFolder != null)
                    {
                        var memList = (from m in currFolder.UFUATags
                                           // where m.Name == step
                                       where String.Compare(m.Name, step, true) == 0
                                       select m).ToList();
                        var FolderList = (from c in currFolder.UFUAFolders
                                              // where c.Name == step
                                          where String.Compare(c.Name, step, true) == 0
                                          select c).ToList();
                        currFolder = null;
                        if (memList.Count > 0)
                        {
                            tagfound = memList[0];
                            list.Insert(0, tagfound);
                            continue;
                        }
                        if (FolderList.Count > 0)
                        {
                            currFolder = FolderList[0];
                        }

                    }
                }
                //end foreach
                //ufuatag = selected.Tag as UFUAModel.UFUATag;
                if (tagfound != null)
                    list.Remove(tagfound);
            }
            return tagfound;
        }

        public OPCUAEntityReference GetTagOPCUAEntityReference(String tagName, String instance, bool inExecution)
        {
            String member = null;

            if (!String.IsNullOrEmpty(instance))
            {
                member = tagName;
            }

            List<UFUAModel.UFUATag> list = new List<UFUATag>();
            UFUAModel.UFUATag tagfound = GetUFUATag(ref tagName, ref instance, inExecution, list);
            if (tagfound != null)
            {
                if (String.IsNullOrEmpty(instance))
                    return GetTagOPCUAEntityReference(tagfound, (IList<UFUAModel.UFUATag>)null, inExecution);

                return GetTagOPCUAEntityReference(tagfound, list, inExecution);
            }


            if (inExecution)
            {
                lock (lockObject)
                {
                    var composedName = String.Format("{0}:{1}", instance ?? String.Empty, tagName);
                    if (notFoundTagNames == null)
                        notFoundTagNames = new List<String>();
                    if (!notFoundTagNames.Contains(composedName))
                        notFoundTagNames.Add(composedName);
                }
            }

            return null;
        }

        List<UFUATagPrototype> GetListPrototypes(String name)
        {
            var ret = new List<UFUATagPrototype>();
            var prototypeFound = (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true)//.AsParallel()
                                  where prototype.Name == name && prototype.UFUATagOwner == null
                                  select prototype).ToList();
            if (prototypeFound.Count > 0)
            {
                ret.Add(prototypeFound[0]);

                var sublist = (from member in prototypeFound[0].GetTagMembers().AsParallel()
                               where !String.IsNullOrEmpty(member.PrototypeName) &&
                               member.PrototypeName != name
                               select member.PrototypeName).ToList();
                foreach (var subitem in sublist)
                {
                    var retsub = GetListPrototypes(subitem);
                    ret.AddRange(retsub);
                }
            }

            return ret;
        }

#if !NET_STANDARD
        public event EventHandler<VariableEventArgs> CreatingVariable;
        virtual public void OnCreatingVariable(Object sender, VariableEventArgs args)
        {
            var t = CreatingVariable;
            if (t != null)
                t(sender, args);
        }

        public event EventHandler<VariableEventArgs> VariableCreated;
        virtual public void OnVariableCreated(Object sender, VariableEventArgs args)
        {
            var t = VariableCreated;
            if (t != null)
                t(sender, args);
        }

        static char flatSeparator = ':';
        internal Dictionary<String, String> CheckAndUpdateVariableListSettingsFlat(String flat)
        {
            var editView = ActiveView as UFUAEditorControl;

            var renamed = new Dictionary<String, String>();
            var InMemorylocal = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            var dllocal = new SimpleDataLayer(InMemorylocal);
            var uowlocal = new UnitOfWork(dllocal);
            var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowlocal, uow, true, false, false);
            var cloneHelperPrototype = new XpoHelpers.CloneIXPSimpleObjectHelper(uowlocal, uow, true, true, true);

            using (var xml = new StringReader(flat))
            {
                using (var xmlTextReader = new XmlTextReader(xml))
                {
                    try
                    {
                        InMemorylocal.ReadXml(xmlTextReader);
                    }
                    catch
                    {
                    }
                }
            }

            var mapPrototypesNodeId = new Dictionary<String, String>();
            var ret = new List<UFUAModel.UFUATag>();
            var listToCopyPrototype = (from c in new XPQuery<UFUAModel.UFUATagPrototype>(uowlocal).AsParallel()
                                       select c).ToList();
            if (listToCopyPrototype.Count > 0)
            {
                var listundo = new List<IXPSimpleObject>();
                listToCopyPrototype.ForEach(prototype =>
                {
                    var prototypeexists = (from c in new XPQuery<UFUAModel.UFUATagPrototype>(uow, true).AsParallel()
                                           where c.Name == prototype.Name && c.UFUATagOwner == null
                                           select c).ToList();
                    bool bAdd = true;
                    if (prototypeexists.Count > 0)
                    {
                        var uiMsgBox = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (uiMsgBox != null)
                        {
                            var answer = uiMsgBox.ShowYesNo(String.Format(Properties.Resources.PrototypeExistsOverwrite, prototype.Name), CustomDialogIcons.Question);
                            bAdd = answer == CustomDialogResults.Yes;
                        }

                        if (bAdd)
                        {
                            if (editView != null)
                                editView.RemovePrototype(prototypeexists[0]);
                            prototypeexists[0].Delete();
                        }
                    }

                    if (bAdd)
                    {
                        var newprototype = cloneHelperPrototype.Clone(prototype, false);

                        listundo.Add(newprototype);
                        EnsureValidNodeId(newprototype);
                        if (prototype.NodeId != newprototype.NodeId)
                            mapPrototypesNodeId[prototype.NodeId.ToString()] = newprototype.NodeId.ToString();
                    }
                    else if (prototypeexists.Count > 0)
                    {
                        if (prototype.NodeId != prototypeexists[0].NodeId)
                            mapPrototypesNodeId[prototype.NodeId.ToString()] = prototypeexists[0].NodeId.ToString();
                    }
                });

                if (editView != null)
                    editView.AddPrototypeList(listundo);
            }

            var listToCopy = (from c in new XPQuery<UFUAModel.UFUATag>(uowlocal).AsParallel()
                              select c).ToList().AsParallel().Where(c => c.PrototypeReference == null).ToList();
            if (listToCopy.Count > 0)
            {
                var listundo = new List<IXPSimpleObject>();
                listToCopy.ForEach(tag =>
                {
                    var listFolderToCreate = new List<UFUAFolder>();
                    UFUAFolder folder = tag.UFUAFolder;
                    while (folder != null)
                    {
                        listFolderToCreate.Insert(0, folder);
                        folder = folder.UFUAFolderAss;
                    }
                    UFUAFolder rootFolder = null;
                    foreach (var f in listFolderToCreate)
                    {
                        var folderexists = (from c in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                                            where c.UFUAFolderAss == rootFolder && c.UFUATagPrototype == null && 
                                            c.Name == f.Name
                                            select c).ToList();
                        if (folderexists.Count == 0)
                        {
                            var foldernewName = NewFolderName(f, f.Name);
                            var newfolder = cloneHelper.Clone(f, false);

                            newfolder.Name = foldernewName;
                            if (rootFolder != null)
                                rootFolder.UFUAFolders.Add(newfolder);
                            rootFolder = newfolder;
                            listundo.Add(newfolder);
                            EnsureValidNodeId(newfolder);
                        }
                        else
                            rootFolder = folderexists[0];
                    }

                    var newName = NewTagName(rootFolder, tag.Name);
                    var args = new VariableEventArgs() { Name = newName };
                    OnCreatingVariable(this, args);
                    if (!args.Cancel)
                    {
                        if (args.Name != newName)
                            newName = NewTagName(rootFolder, args.Name);

                        var newtag = cloneHelper.Clone(tag, false);

                        newtag.Name = newName;
                        if (rootFolder != null)
                            rootFolder.UFUATags.Add(newtag);
                        ret.Add(newtag);
                        listundo.Add(newtag);
                        EnsureValidNodeId(newtag);

                        if (newtag.ModelType == UFUAModel.ModelType.ObjectType &&
                            !String.IsNullOrEmpty(newtag.PrototypeModel) &&
                            mapPrototypesNodeId.ContainsKey(newtag.PrototypeModel))
                        {
                            newtag.PrototypeModel = mapPrototypesNodeId[newtag.PrototypeModel];
                        }

                        args.Name = newName;
                        OnVariableCreated(this, args);

                        //if (tag.Name != newtag.Name)
                        {
                            List<UFUAModel.UFUATag> listSource = new List<UFUATag>();
                            List<UFUAModel.UFUATag> listDest = new List<UFUATag>();
                            renamed.Add(GetTagOPCUAEntityReference(tag, listSource).ToXml(),
                                        GetTagOPCUAEntityReference(newtag, listDest).ToXml());

                            if (newtag.ModelType == UFUAModel.ModelType.ObjectType &&
                                !String.IsNullOrEmpty(newtag.PrototypeName))
                            {
                                var prototypeFoundSource = CreateSubPrototype(tag, _uow: uowlocal);
                                var prototypeFoundDest = CreateSubPrototype(newtag);
                                if (prototypeFoundSource != null && prototypeFoundDest != null)
                                {
                                    prototypeFoundSource.EnsureUniqueMembersOrderId();
                                    prototypeFoundDest.EnsureUniqueMembersOrderId();

                                    listSource.Add(tag);
                                    listDest.Add(newtag);
                                    for (int i = 0; i < prototypeFoundSource.Members.Count; ++i)
                                    {
                                        renamed.Add(GetTagOPCUAEntityReference(prototypeFoundSource.Members[i], listSource).ToXml(),
                                                    GetTagOPCUAEntityReference(prototypeFoundDest.Members[i], listDest).ToXml());
                                    }

                                    GetRenameSubFolderPrototype(prototypeFoundSource.Folders,
                                        prototypeFoundDest.Folders, renamed, listSource, listDest);
                                }
                            }
                        }
                    }
                });

                if (editView != null)
                    editView.AddTagFolderList(listundo);
            }

            return renamed;
        }

        void GetRenameSubFolderPrototype(XPCollection<UFUAFolder> folderSource, XPCollection<UFUAFolder> folderDest, 
                                         Dictionary<String, String> renamed, List<UFUAModel.UFUATag> listSource,
                                         List<UFUAModel.UFUATag> listDest)
        {
            for (int i = 0; i < folderSource.Count; ++i)
            {
                GetRenameSubFolderPrototype(folderSource[i].UFUAFolders, folderDest[i].UFUAFolders,
                                            renamed, listSource, listDest);

                for (int j = 0; j < folderSource[i].UFUATags.Count; ++j)
                {
                    renamed.Add(GetTagOPCUAEntityReference(folderSource[i].UFUATags[j], listSource).ToXml(),
                                GetTagOPCUAEntityReference(folderDest[i].UFUATags[j], listDest).ToXml());
                }
            }
        }

        internal Tuple<String, String> GetVariableListSettingsFlat(List<object> list)
        {
            var InMemorylocal = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            var dllocal = new SimpleDataLayer(InMemorylocal);
            var uowlocal = new UnitOfWork(dllocal);
            var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uow, uowlocal, false, false, false);
            var cloneHelperPrototypes = new XpoHelpers.CloneIXPSimpleObjectHelper(uow, uowlocal, false, true, true);
            var tempVariables = new Dictionary<String, String>();
            var bIsUowLocalEmpty = true;
            foreach (var reference in list.OfType<OPCUAEntityReference>())
            {
                if (reference.AppName == SysVariables.SysNames.dataSynkName ||
                    reference.ParentTypeDefinitionNodeId != null ||
                    reference.ParentTypeDefinitionIdList != null && reference.ParentTypeDefinitionIdList.Count > 0 ||
                    reference.ResolvedNodeId != null && 
                    reference.ResolvedNodeId.ToString().Contains(UFUAServerInfo.Guids.SystemTagsGuid.ToString()))
                    continue;

                var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface(reference.AppName);
                if (dsInterface != null)
                {
                    var monitoredItemViewModel = dsInterface.GetVariable(reference.RelativePath, this);
                    if (monitoredItemViewModel != null)
                    {
                        var name = String.Format("{0}{1}{2}", reference.AppName, flatSeparator, reference.RelativePath);
                        if (!tempVariables.ContainsKey(name))
                            tempVariables.Add(name, monitoredItemViewModel.DataValue.Value.GetType().Name);
                    }
                    else
                    {
#if !NET_STANDARD
                        var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (uiMsgBox != null)
                            uiMsgBox.ShowError(String.Format(Properties.Resources.CannotFindVariableInformation, reference.RelativePath.Replace('&', '\\')));
#else
                        logGeneral.ErrorFormat(Properties.Resources.CannotFindVariableInformation, reference.RelativePath.Replace('&', '\\'));
#endif
                    }
                }
                else
                {
                    var path = TagPathHelper.GetTagPath(reference.HumanReadable, reference.ReadablePath);
                    var split = path.Split(':');
                    var name = split[0];
                    var tagfound = GetUFUATag(name, null, false);
                    if (tagfound != null)
                    {
                        bIsUowLocalEmpty = false;
                        var bFound = (from tag in new XPQuery<UFUAModel.UFUATag>(uowlocal, true)
                                        select tag).ToList().AsParallel().Where(tag => 
                                        tag.GetFullName() == tagfound.GetFullName()).FirstOrDefault() != null;
                        if (!bFound)
                        {
                            var newtag = cloneHelper.Clone(tagfound, false);
                            if (tagfound.UFUAFolder != null)
                            {
                                var folderFound = (from folder in new XPQuery<UFUAModel.UFUAFolder>(uowlocal, true)
                                                    select folder).ToList().AsParallel().Where(folder => 
                                                    folder.GetFullName() == tagfound.UFUAFolder.GetFullName()).FirstOrDefault();
                                if (folderFound == null)
                                    folderFound = cloneHelper.Clone(tagfound.UFUAFolder, false);
                                var localFolder = newtag.UFUAFolder = folderFound;
                                var parentFolder = tagfound.UFUAFolder.UFUAFolderAss;
                                while (parentFolder != null)
                                {
                                    folderFound = (from folder in new XPQuery<UFUAModel.UFUAFolder>(uowlocal, true)
                                                    select folder).ToList().AsParallel().Where(folder => 
                                                    folder.GetFullName() == parentFolder.GetFullName()).FirstOrDefault();
                                    if (folderFound == null)
                                        folderFound = cloneHelper.Clone(parentFolder, false);
                                    localFolder = localFolder.UFUAFolderAss = folderFound;
                                    parentFolder = parentFolder.UFUAFolderAss;
                                }
                            }
                        }
                            
                        if (!String.IsNullOrEmpty(tagfound.PrototypeName))
                        {
                            var prototypeName = tagfound.PrototypeName;
                            var prototypeAlreadyExists = (from prototype in new XPQuery<UFUAModel.UFUATagPrototype>(uowlocal, true)// .AsParallel()
                                                            where prototype.Name == prototypeName && prototype.UFUATagOwner == null
                                                            select prototype).ToList();

                            if (prototypeAlreadyExists.Count == 0)
                            {
                                var prototypeFound = GetListPrototypes(tagfound.PrototypeName);
                                foreach (var prototype in prototypeFound)
                                    cloneHelperPrototypes.Clone(prototype, false);
                            }
                        }
                    }
                    else
                    {
#if !NET_STANDARD
                        var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (uiMsgBox != null)
                            uiMsgBox.ShowError(String.Format(Properties.Resources.CannotFindVariableInformation, reference.RelativePath));
#else
                        logGeneral.ErrorFormat(Properties.Resources.CannotFindVariableInformation, reference.RelativePath);
#endif
                    }
                }
            }
            uowlocal.CommitChanges();

            var ret = String.Empty;
            if (!bIsUowLocalEmpty)
            {
                using (var xml = new StringWriter())
                {
                    using (var xmlTextWriter = new XmlTextWriter(xml) { Formatting = System.Xml.Formatting.Indented })
                    {
                        InMemorylocal.WriteXml(xmlTextWriter);
                        xmlTextWriter.Flush();
                        xmlTextWriter.Close();
                    }

                    ret = xml.ToString();
                }
            }

            uowlocal.Disconnect();
            uowlocal.Dispose();
            dllocal.Dispose();

            return new Tuple<String, String>(ret, tempVariables.Count > 0 ? tempVariables.ToXml() : String.Empty);
        }
#endif

        internal OPCUAEntityReference GetNodeIdOPCUAEntityReference(String tagName, String nodeID)
        {
            String relativepath = null;
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);
            String tag = null;

            Opc.Ua.NodeId nodeid = new Opc.Ua.NodeId(String.Format("{0}?{1}", nodeID, tagName), ns);
            if (tagName.Contains('/'))
            {
                StringBuilder rel = new StringBuilder();
                foreach (String s in tagName.Split('/').ToList())
                {
                    rel.AppendFormat("/{1}:{0}", s, ns);
                }
                relativepath = String.Format("/{1}:{0}", rel.ToString(), ns);
            }
            else
            {
                tag = tagName;
                relativepath = String.Format("/{1}:{0}", tagName, ns);
            }


            var applicationName = GetAplicationName();
            OPCUAEntityReference entityreference = null;
            entityreference = new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                            relativepath, nodeid, string.Format("{0} ({1})", tag, applicationName), null, relativepath);
            return entityreference;
        }

        void SetDefaultAppName(String str)
        {
            lock (lockObject)
            {
                if (cachedDefaultAppName.ContainsKey(Thread.CurrentThread))
                    cachedDefaultAppName.Remove(Thread.CurrentThread);
                cachedDefaultAppName.Add(Thread.CurrentThread, str);
            }
        }

        internal String GetAplicationName(bool refresh = false)
        {
            if(!refresh)
                lock (lockObject)
                {
                    if (cachedDefaultAppName.ContainsKey(Thread.CurrentThread))
                        return cachedDefaultAppName[Thread.CurrentThread];
                }

            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true)/*.AsParallel()*/ select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFUAModel.UFUAConfiguration(task.UnitOfWork);

                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);
                var ret = ufuaConfiguration.ApplicationName;
                SetDefaultAppName(ret);

                return ret;
            }
        }

        internal String GetHistorianDefaultConnection()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFUAModel.UFUAConfiguration(task.UnitOfWork);

                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);
                ufuaConfiguration.NormalizeConnectionStrings(rootBase);
                return ufuaConfiguration.HistorianDefaultConnection;
            }
        }

        internal String GetEventDefaultConnection()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFUAModel.UFUAConfiguration(task.UnitOfWork);

                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);
                ufuaConfiguration.NormalizeConnectionStrings(rootBase);
                return ufuaConfiguration.EventDefaultConnection;
            }
        }

        internal bool IsAtLeastOneAuditTraceEnabled()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                return (from tag in new XPQuery<UFUAModel.UFUATag>(task.UnitOfWork, true).AsParallel()
                        where tag.AuditTraceEnabled
                        select tag).FirstOrDefault() != null;
            }
        }

#if !NET_STANDARD
        internal bool NeedToRunAsCFR21UserIndentity()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFUAModel.UFUAConfiguration(task.UnitOfWork);

                if (ufuaConfiguration.EnableEventDataProtection)
                    return true;

                var lisths = (from hs in new XPQuery<UFUAModel.UFUAHistorianSettings>(task.UnitOfWork, true).AsParallel()
                              where hs.EnableDataProtection && hs.Enabled.Value
                              select hs).ToList();
                if (lisths.Count > 0)
                    return true;

                var listdl = (from dl in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                              where dl.EnableDataProtection && dl.Enable.Value
                              select dl).ToList();

                var validDl = (from c in listdl/*.AsParallel()*/ // AsParallel cannot be used in this case for avoing thread-safe exception when "c.IsValid" method is called.
                               where c.IsValid
                               select c).ToList();

                if (validDl.Count > 0)
                    return true;

                return false;
            }
        }
#endif

        internal String GetAuditTraceDefaultConnection()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFUAModel.UFUAConfiguration(task.UnitOfWork);

                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);
                ufuaConfiguration.NormalizeConnectionStrings(rootBase);
                return ufuaConfiguration.AuditTraceDefaultConnection;
            }
        }

        internal String GetHistorianConnection(String historian)
        {
            var ret = string.Empty;
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var found = (from tag in new XPQuery<UFUAModel.UFUAHistorianSettings>(task.UnitOfWork, true).AsParallel()
                             where tag.Name == historian
                             select tag).FirstOrDefault();
                if (found != null)
                {
                    ret = XpoHelpers.XpoHelper.NormalizeConnectionString(found.ConnectionSettings, rootBase);
                    if (!String.IsNullOrWhiteSpace(found.TableName))
                    {   if (String.IsNullOrWhiteSpace(ret))
                            ret = GetHistorianDefaultConnection();
                        ret = UFUAHistorianModel.Helpers.HistorianHelper.SetTableName<UFUAHistorianModel.UFUAAuditDataItem>(ret, found.TableName);
                    }
                }
            }

            return ret;
        }

        internal IDictionary<String, String> GetHistorianConnections(bool usedefault = true)
        {
            var ret = new Dictionary<String, String>();
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var list = (from tag in new XPQuery<UFUAModel.UFUAHistorianSettings>(task.UnitOfWork, true).AsParallel()
                            //where !String.IsNullOrEmpty(tag.ConnectionSettings)
                            select tag).ToList();

                if (list.Count > 0)
                {
                    var defconn = usedefault ? GetHistorianDefaultConnection() : string.Empty;
                    foreach (var item in list)
                    {
                        if (String.IsNullOrEmpty(item.ConnectionSettings))
                            ret.Add(item.Name, defconn);
                        else
                            ret.Add(item.Name, XpoHelpers.XpoHelper.NormalizeConnectionString(item.ConnectionSettings, rootBase));
                    }
                }
            }

            return ret;
        }

        internal String[] GetServerUriArray()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).ToList();
                if (list.Count > 0 && !String.IsNullOrEmpty(list[0].ListRedundancyServers))
                    return list[0].ListRedundancyServers.Split(new Char[] { ',' });
                return null;
            }
        }

        internal IDictionary<String, String> GetDataLoggerConnections(bool usedefault = true)
        {
            var ret = new Dictionary<String, String>();
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var list = (from tag in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                            //where !String.IsNullOrEmpty(tag.ReadableConnectionString)
                            select tag).ToList();

                if (list.Count > 0)
                {
                    var defconn = usedefault ? GetHistorianDefaultConnection() : string.Empty;
                    foreach (var item in list)
                    {
                        if (String.IsNullOrEmpty(item.ReadableConnectionString))
                            ret.Add(item.Name, defconn);
                        else
                            ret.Add(item.Name, String.Format("DataProvider={0};{1}", item.ConnectionSettings.DataProvider, item.ConnectionSettings.Connection));
                    }
                }
            }

            return ret;
        }

        internal String GetDataLoggerConnection(String datalogger)
        {
            var ret = string.Empty;
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var dataloggersettings = (from tag in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                                          where tag.Name == datalogger
                                          select tag).FirstOrDefault();
                if (dataloggersettings != null && !String.IsNullOrEmpty(dataloggersettings.ReadableConnectionString))
                {
                    ret = String.Format("DataProvider={0};{1}", dataloggersettings.ConnectionSettings.DataProvider, dataloggersettings.ConnectionSettings.Connection);
                }
            }

            return ret;
        }

        internal String GetDataLoggerTableName(String name)
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                return (from tag in new XPQuery<DataLoggerModel.DataLoggerSettings>(task.UnitOfWork, true).AsParallel()
                        where tag.Name == name
                        select !String.IsNullOrEmpty(tag.TableName) ? tag.TableName : tag.Name).Single();
            }
        }

        internal OPCUAEntityReference GetWriteValuesOPCUAEntityReference(String driverName)
        {
            string relativepath = string.Empty;
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);

            var applicationName = GetAplicationName();
            var nodeId = new NodeId(String.Format("{0}.WriteValues", driverName), ns); // Second parameter should be replaced with 'NamespaceIndex'
            return new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                            null, nodeId, string.Format("{0}.WriteValues ({1})", driverName, applicationName), null, null);
        }


        internal OPCUAEntityReference GetReadValuesOPCUAEntityReference(String driverName)
        {
            string relativepath = string.Empty;
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);

            var applicationName = GetAplicationName();
            var nodeId = new NodeId(String.Format("{0}.ReadValues", driverName), ns); // Second parameter should be replaced with 'NamespaceIndex'
            return new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                            null, nodeId, string.Format("{0}.ReadValues ({1})", driverName, applicationName), null, null);
        }

#if !NET_STANDARD
        internal IList<UFUAModel.UFUATag> GetTagMemberCollection(UFUAModel.UFUAFolder root = null, string prototypeName = null, string tagOwnerPath = null)
        {
            if (root == null)
            {
                if (string.IsNullOrEmpty(prototypeName))
                    return new List<UFUAModel.UFUATag>();
                if (!string.IsNullOrEmpty(tagOwnerPath))
                    return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                            where tag.UFUAFolder == null
                            orderby tag.Name ascending
                            select tag).ToList().AsParallel().Where(tag =>
                            tag.IsSubPrototypeMember &&
                            tag.TagOwnerPath == tagOwnerPath).ToList();
                else
                    return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                            where tag.UFUAFolder == null
                            orderby tag.Name ascending
                            select tag).ToList().AsParallel().Where(tag =>
                            tag.IsPrototypeMember && string.IsNullOrEmpty(tag.TagOwnerPath) &&
                            tag.PrototypeReferenceName == prototypeName).ToList();
            }
            List<UFUAModel.UFUAFolder> folders;
            if (!string.IsNullOrEmpty(tagOwnerPath))
                folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                           where folder.NodeId == root.NodeId
                           orderby folder.Name ascending
                           select folder).ToList().AsParallel().Where(folder =>
                           folder.IsSubPrototypeMember &&
                           folder.PrototypeReference.TagOwnerPath == tagOwnerPath).ToList();
            else
                folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                           where folder.NodeId == root.NodeId
                           orderby folder.Name ascending
                           select folder).ToList().AsParallel().Where(folder =>
                           folder.IsPrototypeMember && string.IsNullOrEmpty(folder.PrototypeReference.TagOwnerPath) &&
                           folder.PrototypeReference.Name == prototypeName).ToList();


            if (folders.Count == 0)
                return new List<UFUAModel.UFUATag>();
            else
                return (from c in folders[0].UFUATags.AsParallel()
                        orderby c.Name ascending
                        select c).ToList().AsParallel().Where(c =>
                        c.IsPrototypeMember).ToList();
        }

        internal IList<UFUAModel.UFUAFolder> GetFolderPrototypeCollection(UFUAModel.UFUAFolder root = null, string prototypeName = null, string tagOwnerPath = null)
        {
            if (root == null)
            {
                if (string.IsNullOrEmpty(prototypeName))
                    return new List<UFUAModel.UFUAFolder>();
                if (!string.IsNullOrEmpty(tagOwnerPath))
                    return (from tag in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                            where tag.UFUAFolderAss == null
                            orderby tag.Name ascending
                            select tag).ToList().AsParallel().Where(tag =>
                            tag.IsSubPrototypeMember &&
                            tag.PrototypeReference.TagOwnerPath == tagOwnerPath).ToList();
                else
                    return (from tag in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                            where tag.UFUAFolderAss == null
                            orderby tag.Name ascending
                            select tag).ToList().AsParallel().Where(tag =>
                            tag.IsPrototypeMember && string.IsNullOrEmpty(tag.PrototypeReference.TagOwnerPath) &&
                            tag.PrototypeReference.Name == prototypeName).ToList();
            }

            if (!string.IsNullOrEmpty(tagOwnerPath))
                return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                        where folder.NodeId == root.NodeId
                        orderby folder.Name ascending
                        select folder).ToList().AsParallel().Where(folder =>
                        folder.IsSubPrototypeMember &&
                        folder.PrototypeReference.TagOwnerPath == tagOwnerPath).ToList();
            else
                return (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                        where folder.NodeId == root.NodeId
                        orderby folder.Name ascending
                        select folder).ToList().AsParallel().Where(folder =>
                        folder.IsPrototypeMember && string.IsNullOrEmpty(folder.PrototypeReference.TagOwnerPath) &&
                        folder.PrototypeReference.Name == prototypeName).ToList();
        }

        internal IList<string> GetTagMemberCollectionNames(UFUAModel.UFUAFolder root = null, string prototypeName = null)
        {
            if (root == null)
            {
                if (string.IsNullOrEmpty(prototypeName))
                    return new List<string>();
                return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.UFUAFolder == null
                        orderby tag.Name ascending
                        select tag).ToList().AsParallel().Where(tag => tag.IsPrototypeMember == true
                        && tag.PrototypeReferenceName == prototypeName).AsParallel().Select(t => t.Name).ToList();
            }

            var folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                               //where folder.Oid == root.Oid
                           where folder.NodeId == root.NodeId
                           orderby folder.Name ascending
                           select folder).ToList();
            if (folders.Count == 0)
                return new List<string>();
            else
                return (from c in folders[0].UFUATags.AsParallel() where c.IsPrototypeMember == true orderby c.Name ascending select c.Name).ToList();
        }

        internal IList<UFUAModel.UFUATag> GetFullTagMemberCollection()
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                    orderby tag.Name ascending
                    select tag).ToList().AsParallel().Where(tag =>
                    tag.IsPrototypeMember).ToList();
        }

        internal IList<UFUAModel.UFUATag> GetFullTagMemberImportExportCollection(bool useShared = true)
        {
            if (useShared)
                return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        orderby tag.Name ascending
                        select tag).ToList().AsParallel().Where(tag =>
                        tag.IsPrototypeMember && !tag.IsSubPrototypeMember).ToList();
            else
                return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        orderby tag.Name ascending
                        select tag).ToList().AsParallel().Where(tag =>
                        tag.IsSubPrototypeMember).ToList();
        }

        internal UFUAModel.UFUATag GetUFUATag(string path)
        {
            if (path == null)
                return null;
            UFUAModel.UFUATag ret = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                     orderby tag.Name ascending
                                     select tag).ToList().AsParallel().Where(tag =>
                                     tag.GetFullName() == path).FirstOrDefault();
            return ret;
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<UFUAModel.UFUATag> GetTagCollection(UFUAModel.UFUAFolder root = null, bool sortByName = false)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.UFUAFolder == null && tag.UFUATagPrototype == null
                        orderby tag.Name ascending
                        select tag).ToList();
            }

            var prototypeReference = root.PrototypeReference;
            var folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(uow, true).AsParallel()
                           where folder.NodeId == root.NodeId
                           orderby folder.Name ascending
                           select folder).ToList().AsParallel().Where(folder => folder.PrototypeReference == prototypeReference).ToList();
            if (folders.Count == 0)
                return new List<UFUAModel.UFUATag>();
            else
            {
                if (folders[0].IsPrototypeMember)
                {
                    if (sortByName)
                        return (from c in folders[0].UFUATags.AsParallel() orderby c.Name ascending select c).ToList();
                    else
                        return (from c in folders[0].UFUATags.AsParallel() orderby c.MemberOrderId ascending select c).ToList();
                }
                else
                    return (from c in folders[0].UFUATags.AsParallel() orderby c.Name ascending select c).ToList();
            }
        }

#if !NET_STANDARD
        internal List<UFUAModel.UFUATag> AddImportedTag(List<UFUAModel.ImportTag> itags, UFUAModel.UFUAFolder root, ref CustomDialogResults dialogRetValue)
        {
            var ret = new List<UFUAModel.UFUATag>();

            var listTags = GetTagCollection(root);
            var listNames = GetTagsNameList(root);
            var mapStartCounter = new Dictionary<string, ulong>();

            foreach (var itag in itags)
            {
                string tempName = UFUAModel.Helpers.NameValidator.EnsureValidName(itag.Name);
                if (!string.IsNullOrEmpty(tempName))
                {
                    int index = 0;
                    for (index = 0; index < tempName.Length; index++)
                    {
                        // search for first letter character from input string
                        if (Char.IsLetter(tempName[index]))
                            break;
                    }
                    // remove any non letter character from input string
                    tempName = tempName.Substring(index);
                }

                if (string.IsNullOrEmpty(tempName))
                {
                    tempName = "Variable";
                }

                itag.Name = tempName;

                UFUAModel.UFUATag tag = (from c in listTags.AsParallel() where c.Name == itag.Name select c).FirstOrDefault();
                if (tag != null)
                {
                    if (dialogRetValue != CustomDialogResults.YesAll &&
                        dialogRetValue != CustomDialogResults.NoAll &&
                        dialogRetValue != CustomDialogResults.Cancel)
                    {
                        dialogRetValue = AskCreateVariable(string.Format(Properties.Resources.ImportTagNameExist, itag.Name), Properties.Resources.ImportTagInProgress);
                    }

                    if (dialogRetValue == CustomDialogResults.Cancel)
                    {
                        return ret;
                    }
                    else if (dialogRetValue == CustomDialogResults.No || dialogRetValue == CustomDialogResults.NoAll)
                    {
                        tag.DataType = itag.DataType;
                        tag.Description = itag.Description;
                        tag.DynamicSettings = itag.DynSettings;
                        tag.ModelType = itag.ModelType;
                        tag.PrototypeName = itag.PrototypeModel;
                        tag.ArrayDimension = itag.ArrayDimension;
                        if (tag.ModelType == UFUAModel.ModelType.Enumerated && itag.EnumsString != null)
                        {
                            if (tag.EnumStrings != null)
                            {
                                while (tag.EnumStrings.Count > 0)
                                    tag.EnumStrings[0].Delete();
                                foreach (var x in itag.EnumsString)
                                {
                                    tag.EnumStrings.Add(new UFUAModel.UFUAEnumString(tag.Session) { Data = x });
                                }
                            }
                        }
                        continue;
                    }
                }

                if (itag != null)
                    itag.Name = NewTagName(root, itag.Name, mapStartCounter, listNames);
                if ((itag.ModelType == UFUAModel.ModelType.Enumerated) && (itag.EnumsString != null))
                {

                    tag = new UFUAModel.UFUATag(uow)
                    {
                        Name = itag.Name,
                        NodeId = Guid.NewGuid(),
                        CreateDate = DateTime.UtcNow,
                        MemberOrderId = -1,
                        DataType = itag.DataType,
                        Description = itag.Description,
                        DynamicSettings = itag.DynSettings,
                        ModelType = itag.ModelType,
                        PrototypeName = itag.PrototypeModel,
                        ArrayDimension = itag.ArrayDimension
                    };
                    foreach (var item in itag.EnumsString)
                        tag.EnumStrings.Add(new UFUAModel.UFUAEnumString(tag.Session) { Data = item });
                }
                else
                    tag = new UFUAModel.UFUATag(uow)
                    {
                        Name = itag.Name,
                        NodeId = Guid.NewGuid(),
                        CreateDate = DateTime.UtcNow,
                        MemberOrderId = -1,
                        DataType = itag.DataType,
                        Description = itag.Description,
                        DynamicSettings = itag.DynSettings,
                        ModelType = itag.ModelType,
                        PrototypeName = itag.PrototypeModel,
                        ArrayDimension = itag.ArrayDimension
                    };

                if (root != null)
                    root.UFUATags.Add(tag);

                EnsureValidNodeId(tag);

                ret.Add(tag);

                listTags.Add(tag);
                listNames.Add(tag.Name);
            }

            return ret;
        }

        // FOGBUGZ 11407
        internal CustomDialogResults AskCreateVariable(string dialogcontent, string dialogTitle)
        {
            YesNoAllCancelControl userCont = new YesNoAllCancelControl(dialogcontent);
            userCont.ClearValue(FrameworkElement.WidthProperty);
            userCont.ClearValue(FrameworkElement.WidthProperty);
            var Dialog = new GeneralDialogContent(userCont, GeneralDialogButtons.None)
            {
                Title = dialogTitle,
                HelpLink = "AskCreateVariable"
            };
            Dialog.ShowDialog();
            return (userCont.ClickedButton);
        }

        internal UFUAModel.UFUATagPrototype AddImportedPrototype(UFUAModel.ImportPrototype iproto)
        {
            var editView = ActiveView as UFUAEditorControl;
            string protoname = UFUAModel.Helpers.NameValidator.EnsureValidName(iproto.Name);

            if (!string.IsNullOrEmpty(protoname))
            {
                int index = 0;
                for (index = 0; index < protoname.Length; index++)
                {
                    // search for first letter character from input string
                    if (Char.IsLetter(protoname[index]))
                        break;
                }
                // remove any non letter character from input string
                protoname = protoname.Substring(index);
            }

            if (string.IsNullOrEmpty(protoname))
            {
                protoname = "Prototype";
            }

            var prototype = FindPrototypeByName(protoname);
            if (prototype != null)
            {
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox == null ||
                    uiMsgBox.ShowYesNo(string.Format(Properties.Resources.ImportPrototypeNameExist, protoname), CustomDialogIcons.Question) == CustomDialogResults.No)
                {
                    if (editView != null)
                        editView.RemovePrototype(prototype);
                    prototype.Delete();
                }
                else
                    protoname = NewPrototypeName(protoname);
            }

            prototype = new UFUAModel.UFUATagPrototype(uow) { Name = protoname, NodeId = Guid.NewGuid(), CreateDate = DateTime.UtcNow, Description = iproto.Description };

            for (int i = 0; i < iproto.Elements.Count; i++)
            {
                UFUAModel.UFUAFolder root = null;
                if (!String.IsNullOrEmpty(iproto.Elements[i].Folder))
                {
                    var folders = iproto.Elements[i].Folder.Split('/');
                    foreach (var folder in folders)
                    {
                        var listFolders = new List<UFUAModel.UFUAFolder>();
                        var folderName = UFUAModel.Helpers.NameValidator.EnsureValidName(folder);
                        if (root != null)
                            listFolders.AddRange(GetFolderCollection(root));
                        else
                            listFolders.AddRange(prototype.Folders);

                        var parentFolder = root;
                        root = (from c in listFolders.AsParallel()
                                where c.Name == folderName
                                select c).FirstOrDefault();

                        if (root == null)
                        {
                            if (parentFolder != null)
                                root = AddNewFolder(parentFolder);
                            else
                                root = AddNewFolder(prototype);
                            root.Name = folderName;
                        }
                    }
                }

                var protomodel = UFUAModel.Helpers.NameValidator.EnsureValidName(iproto.Elements[i].PrototypeModel);
                var tag = new UFUAModel.UFUATag(uow)
                {
                    Name = iproto.Elements[i].Name,
                    ModelType = iproto.Elements[i].ModelType,
                    PrototypeModel = protomodel,
                    ArrayDimension = iproto.Elements[i].ArrayDimension,
                    DataType = iproto.Elements[i].DataType,
                    Description = iproto.Elements[i].Description,
                    NodeId = Guid.NewGuid(),
                    MemberOrderId = i
                };
                if (iproto.Elements[i].ModelType == UFUAModel.ModelType.Enumerated)
                    foreach (var item in iproto.Elements[i].EnumsString)
                        tag.EnumStrings.Add(new UFUAModel.UFUAEnumString(tag.Session) { Data = item });

                if (root == null)
                    prototype.Members.Add(tag);
                else
                    root.UFUATags.Add(tag);

                EnsureValidNodeId(tag);
            }

            EnsureValidNodeId(prototype);

            if (editView != null)
                editView.AddPrototypeList(new List<IXPSimpleObject>() { prototype });

            return prototype;
        }

        internal ImportExportResult ServerImportExport<T>()
        {
            if (EditorManagerComponent == null)
                return new ImportExportResult(ResultType.Failed);
            //string rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            DocumentImportExportHelper<T> docHelper = new DocumentImportExportHelper<T>(this);
            trackingChangesDisabled = true;
            var result = docHelper.ImportExport();
            NeedsSave = result.Result == ResultType.Successfully && 
                        (result.AddedObjects != null && result.AddedObjects.Count > 0 || 
                        result.ChangedObjects != null && result.ChangedObjects.Count > 0);
            trackingChangesDisabled = false;
            return result;
        }
#endif
        #endregion

        #region WinClipboard
#if !NET_STANDARD
        internal void CopyInMemoryDataToWinClipboard()
        {
            Clipboard.Clear();
            using (var xml = new StringWriter())
            {
                using (var xmlTextWriter = new XmlTextWriter(xml) { Formatting = System.Xml.Formatting.Indented })
                {
                    InMemoryClipboard.WriteXml(xmlTextWriter);
                    xmlTextWriter.Flush();
                    xmlTextWriter.Close();
                }

                LastClipboardUnicodeText = xml.ToString();
                Clipboard.SetText(LastClipboardUnicodeText);
            }
        }

        string LastClipboardUnicodeText = String.Empty;
        internal void CopyWinClipboardToInMemoryData()
        {
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.UnicodeText) &&
                    Clipboard.GetText(TextDataFormat.UnicodeText) != LastClipboardUnicodeText)
                {
                    CleanClipbaord();
                    LastClipboardUnicodeText = Clipboard.GetText(TextDataFormat.UnicodeText);
                    using (var xml = new StringReader(Clipboard.GetText(TextDataFormat.UnicodeText)))
                    {
                        using (var xmlTextReader = new XmlTextReader(xml))
                        {
                            var tempds = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                            //using (var tempdl = new SimpleDataLayer(tempds))
                            {
                                bool bValid = true;
                                try
                                {
                                    tempds.ReadXml(xmlTextReader);
                                }
                                catch
                                {
                                    bValid = false;
                                }

                                if (bValid)
                                {
                                    InMemoryClipboard.ReadFromInMemoryDataStore(tempds);
                                }
                            }
                        }
                    }
                    CheckClipbaord();
                }
            }
            catch
            { }
        }

        internal void CleanClipbaord()
        {
            if (uowClipboard == null)
                return;

            uowClipboard.ClearDatabase();
            clipboardElements.Clear();
        }

        internal void CheckClipbaord()
        {
            if (uowClipboard == null)
                return;

            clipboardElements.Check(uowClipboard);
        }
#endif
        #endregion

        #region Drivers (Clipboard)
#if !NET_STANDARD
        internal void CopyDriversToClipbaord(List<UFUAModel.UFUACommunicationDriver> list)
        {
            list.ForEach(driver =>
            {
                CloneToClipboard(driver, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsDrivers()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsDrivers;
        }

        internal List<UFUAModel.UFUACommunicationDriver> PasteClipboardDrivers()
        {
            var ret = new List<UFUAModel.UFUACommunicationDriver>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUACommunicationDriver>(uowClipboard).AsParallel()
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var listName = GetDriversNameList();
            listToCopy.ForEach(driver =>
            {
                if (!listName.Contains(driver.Name))
                {
                    var newdriver = CloneFromClipboard(driver, false) as UFUAModel.UFUACommunicationDriver;
                    ret.Add(newdriver);
                }
            });
            return ret;
        }
#endif
        #endregion

        #region Tags and Prototypes (Clipboard)
#if !NET_STANDARD
        internal void CopyListFoldersToClipbaord(List<UFUAModel.UFUAFolder> list)
        {
            if (EditorManagerComponent.Workspace == null)
                return;
            EditorManagerComponent.Workspace.UpdateProgressState(0,
                                                    list.Count(),
                                                   $"{Properties.Resources.WorkInProgress}",
                                                   TaskbarItemProgressState.Normal);
            list.ForEach(folder =>
            {
                CloneToClipboard(folder, false);
                EditorManagerComponent.Workspace.IncrementProgressState();
            });
            uowClipboard.CommitChanges();
        }

        internal void CopyListTagsToClipbaord(List<UFUAModel.UFUATag> list)
        {
            if (EditorManagerComponent.Workspace == null)
                return;
            EditorManagerComponent.Workspace.UpdateProgressState(0,
                                                    list.Count(),
                                                   $"{Properties.Resources.WorkInProgress}",
                                                   TaskbarItemProgressState.Normal);
            list.ForEach(tag =>
                {
                    CloneToClipboard(tag, false);
                    EditorManagerComponent.Workspace.IncrementProgressState();
                });
            uowClipboard.CommitChanges();
        }

        internal void CopyListAlarmThresholdsToClipbaord(List<UFUAModel.UFUAAlarmThreshold> list)
        {
            if (EditorManagerComponent.Workspace == null)
                return;
            EditorManagerComponent.Workspace.UpdateProgressState(0,
                                                    list.Count(),
                                                   $"{Properties.Resources.WorkInProgress}",
                                                   TaskbarItemProgressState.Normal);
            list.ForEach(alr =>
            {
                CloneToClipboard(alr, false);
                EditorManagerComponent.Workspace.IncrementProgressState();
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsFolders()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsFolders;
        }

        internal bool ClipboardContainsTags()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsTags;
        }

        internal bool ClipboardContainsAlarmThresholds()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsAlarmThresholds;
        }

        internal List<XPObject> PasteClipboardTempFoldersAndTags(UFUAModel.UFUAFolder folder, TempVariablesModel.Folder parent = null)
        {
            var ret = new List<XPObject>();
            var listTempFolderToCopy = (from c in new XPQuery<TempVariablesModel.Folder>(uowClipboard).AsParallel()
                                        where c.FolderAss == parent
                                        //orderby c.Oid
                                        select c).ToList();
            if (listTempFolderToCopy.Count > 0)
            {
                var mapStartCounter = new Dictionary<string, ulong>();
                var listName = GetFoldersNameList(folder);
                listTempFolderToCopy.ForEach(dir =>
                {
                    var newName = NewFolderName(folder, dir.Name, mapStartCounter, listName);
                    var newfolder = AddNewFolder(folder);
                    newfolder.Name = newName;
                    if (dir.Folders.Count > 0)
                        dir.Folders.ToList().ForEach(f => PasteClipboardTempFoldersAndTags(newfolder, dir));

                    if (newfolder != null)
                    {
                        if (folder != null)
                            folder.UFUAFolders.Add(newfolder);
                        ret.Add(newfolder);

                        var listUFUATagToCopy = (from c in new XPQuery<TempVariablesModel.Variable>(uowClipboard).AsParallel()
                                                 where c.Folder == dir
                                                 //orderby c.Oid
                                                 select c).ToList();
                        if (listUFUATagToCopy.Count > 0)
                        {
                            var mapStartTagsCounter = new Dictionary<string, ulong>();
                            var listTagsName = GetTagsNameList(folder);
                            listUFUATagToCopy.ForEach(tag =>
                            {
                                var newTagName = NewTagName(newfolder, tag.Name, mapStartTagsCounter, listTagsName);
                                var newtag = AddNewTag(newfolder);
                                newtag.Name = newTagName;
                                try
                                {
                                    newtag.Description = tag.Description;
                                    newtag.DataType = tag.DataType;
                                    newtag.InitialValue = tag.InitialValue;
                                    newtag.ArrayDimension = (uint)tag.ArrayDimension;
                                }
                                catch (Exception)
                                {
                                }

                                if (newfolder != null)
                                    newfolder.UFUATags.Add(newtag);
                            });
                        }
                    }
                });
            }
            if (parent == null)
            {
                var listmainUFUATagToCopy = (from c in new XPQuery<TempVariablesModel.Variable>(uowClipboard).AsParallel()
                                             where c.Folder == null
                                             //orderby c.Oid
                                             select c).ToList();
                if (listmainUFUATagToCopy.Count > 0)
                {
                    var mapStartTagsCounter = new Dictionary<string, ulong>();
                    var listTagsName = GetTagsNameList(folder);
                    listmainUFUATagToCopy.ForEach(tag =>
                    {
                        var newTagName = NewTagName(folder, tag.Name, mapStartTagsCounter, listTagsName);
                        var newtag = AddNewTag(folder);
                        newtag.Name = newTagName;
                        try
                        {
                            newtag.Description = tag.Description;
                            newtag.DataType = tag.DataType;
                            newtag.InitialValue = tag.InitialValue;
                            newtag.ArrayDimension = (uint)tag.ArrayDimension;
                        }
                        catch (Exception)
                        {
                        }

                        if (folder != null)
                            folder.UFUATags.Add(newtag);

                        ret.Add(newtag);
                    });
                }
            }
            return ret;
        }

        internal List<UFUAModel.UFUAFolder> PasteClipboardFolders(UFUAModel.UFUAFolder folder, UFUAModel.UFUAFolder parent = null)
        {
            var ret = new List<UFUAModel.UFUAFolder>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUAFolder>(uowClipboard).AsParallel()
                              where c.UFUAFolderAss == parent && c.UFUATagPrototype == null
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new GuidHelper(uow, TypeObject.Folder);
            var listName = GetFoldersNameList(folder);
            var helper = new UFUAModel.Helpers.AssociationHelper(uow);
            listToCopy.ForEach(dir =>
            {
                var exist = guidHelper.Contains(dir.NodeId);
                var newName = NewFolderName(folder, dir.Name, mapStartCounter, listName);
                var newfolder = CloneFromClipboard(dir, exist) as UFUAModel.UFUAFolder;
                helper.CopyAssociationReferences(dir, newfolder); // Must be called before changing the name of the cloned object.
                newfolder.Name = newName;
                if (folder != null)
                    folder.UFUAFolders.Add(newfolder);
                ret.Add(newfolder);
                EnsureValidNodeId(newfolder);
                if (!exist)
                    guidHelper.EnsureValidGuid(newfolder);
            });
            return ret;
        }

        internal List<UFUAModel.UFUAFolder> PasteClipboardFolders(UFUAModel.UFUATagPrototype prototype, UFUAModel.UFUATagPrototype parent = null)
        {
            var ret = new List<UFUAModel.UFUAFolder>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUAFolder>(uowClipboard).AsParallel()
                              where c.UFUATagPrototype == parent && c.UFUAFolderAss == null
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new GuidHelper(uow, TypeObject.Folder);
            var listName = GetFoldersNameList(prototype);
            var helper = new UFUAModel.Helpers.AssociationHelper(uow);
            listToCopy.ForEach(dir =>
            {
                var exist = guidHelper.Contains(dir.NodeId);
                var newName = NewFolderName(prototype, dir.Name, mapStartCounter, listName);
                var newfolder = CloneFromClipboard(dir, exist) as UFUAModel.UFUAFolder;
                helper.CopyAssociationReferences(dir, newfolder); // Must be called before changing the name of the cloned object.
                newfolder.Name = newName;
                if (prototype != null)
                    prototype.Folders.Add(newfolder);
                ret.Add(newfolder);
                EnsureValidNodeId(newfolder);
                if (!exist)
                    guidHelper.EnsureValidGuid(newfolder);
            });
            return ret;
        }

        internal List<UFUAModel.UFUATag> PasteClipboardTags(UFUAModel.UFUAFolder folder, UFUAModel.UFUAFolder parent = null)
        {
#if DEBUG
            var watcher = Stopwatch.StartNew();
#endif
            var ret = new List<UFUAModel.UFUATag>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUATag>(uowClipboard).AsParallel()
                              where c.UFUAFolder == parent
                              //orderby c.Oid
                              select c).ToList().AsParallel().Where(c =>
                              !c.IsSubPrototypeMember).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new GuidHelper(uow, TypeObject.Tag);
            var listName = GetTagsNameList(folder);
            var helper = new UFUAModel.Helpers.AssociationHelper(uow);
            listToCopy.ForEach(tag =>
            {
                var exist = guidHelper.Contains(tag.NodeId);
                var newName = NewTagName(folder, tag.Name, mapStartCounter, listName);
                var newtag = CloneFromClipboard(tag, exist) as UFUAModel.UFUATag;
                helper.CopyAssociationReferences(tag, newtag); // Must be called before changing the name of the cloned object.
                newtag.Name = newName;
                if (folder != null)
                    folder.UFUATags.Add(newtag);
                ret.Add(newtag);
                EnsureValidNodeId(newtag);
            });
#if DEBUG
            Debug.WriteLine("PasteClipboardTags: Total Copied = {0}, Total Elapsed = {1}", listToCopy.Count, watcher.ElapsedMilliseconds);
#endif
            return ret;
        }

        internal List<UFUAModel.UFUATag> PasteClipboardTags(UFUAModel.UFUATagPrototype prototype, UFUAModel.UFUAFolder parent = null)
        {
            var ret = new List<UFUAModel.UFUATag>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUATag>(uowClipboard).AsParallel()
                              where c.UFUAFolder == parent
                              //orderby c.Oid
                              select c).ToList().AsParallel().Where(c =>
                              !c.IsSubPrototypeMember).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new GuidHelper(uow, TypeObject.Tag);
            var listName = GetTagsNameList(prototype);
            var helper = new UFUAModel.Helpers.AssociationHelper(uow);
            listToCopy.ForEach(tag =>
            {
                var exist = guidHelper.Contains(tag.NodeId);
                var newName = NewTagName(prototype, tag.Name, mapStartCounter, listName);
                var newtag = CloneFromClipboard(tag, exist) as UFUAModel.UFUATag;
                helper.CopyAssociationReferences(tag, newtag); // Must be called before changing the name of the cloned object.
                newtag.Name = newName;
                if (prototype != null)
                    prototype.Members.Add(newtag);
                ret.Add(newtag);
                EnsureValidNodeId(newtag);
            });
            return ret;
        }

        internal List<UFUAModel.UFUAAlarmThreshold> PasteClipboardAlarmThresholds(UFUAModel.UFUATag tag, UFUAModel.UFUATag parent = null)
        {
            var ret = new List<UFUAModel.UFUAAlarmThreshold>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUAAlarmThreshold>(uowClipboard).AsParallel()
                              where c.UFUATagAss == parent
                              //orderby c.Oid
                              select c).ToList();
            listToCopy.ForEach(alr =>
            {
                var newalr = CloneFromClipboard(alr, false) as UFUAModel.UFUAAlarmThreshold;
                if (!NodeId.IsNull(newalr.UFUAAlarmDefinitionNodeIdRef))
                    newalr.UFUAAlarmDefinitionRef = FindAlarmDefinitionByNodeId(newalr.UFUAAlarmDefinitionNodeIdRef);
                tag.UFUAAlarmThresholds.Add(newalr);
                tag.NotifyPropertyChanged("UFUAAlarmThresholds");
                ret.Add(newalr);
            });
            return ret;
        }

        internal void CopyListPrototypesToClipbaord(List<UFUAModel.UFUATagPrototype> list)
        {
            list.ForEach(tag =>
            {
                CloneToClipboard(tag, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsPrototypes()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsPrototypes;
        }

        internal List<UFUAModel.UFUATagPrototype> PasteClipboardPrototypes()
        {
            var ret = new List<UFUAModel.UFUATagPrototype>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUATagPrototype>(uowClipboard).AsParallel()
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new GuidHelper(uow, TypeObject.Prototype);
            var listName = GetPrototypesNames();
            var helper = new UFUAModel.Helpers.AssociationHelper(uow);
            listToCopy.ForEach(tag =>
            {
                var exist = guidHelper.Contains(tag.NodeId);
                var newName = NewPrototypeName(tag.Name, mapStartCounter, listName);
                var newtag = CloneFromClipboard(tag, exist) as UFUAModel.UFUATagPrototype;
                helper.CopyAssociationReferences(tag, newtag); // Must be called before changing the name of the cloned object.
                newtag.Name = newName;
                ret.Add(newtag);
                EnsureValidNodeId(newtag);
                if (!exist)
                    guidHelper.EnsureValidGuid(newtag);
            });
            return ret;
        }
#endif
        #endregion

        #region Views (Clipboard)
#if !NET_STANDARD
        internal void CopyListViewsToClipbaord(List<UFUAModel.UFUAView> list)
        {
            list.ForEach(view =>
            {
                CloneToClipboard(view, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsViews()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsViews;
        }

        internal List<UFUAModel.UFUAView> PasteClipboardViews()
        {
            var ret = new List<UFUAModel.UFUAView>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUAView>(uowClipboard).AsParallel()
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new GuidHelper(uow, TypeObject.View);
            var listName = GetViewsNameList();
            listToCopy.ForEach(view =>
            {
                var exist = guidHelper.Contains(view.NodeId);
                var newName = NewViewName(view.Name, mapStartCounter, listName);
                var newview = CloneFromClipboard(view, exist) as UFUAModel.UFUAView;
                newview.Name = newName;
                ret.Add(newview);
            });
            return ret;
        }
#endif
        #endregion

        #region Historians (Clipboard)
#if !NET_STANDARD
        internal void CopyListHistoriansToClipbaord(List<UFUAModel.UFUAHistorianSettings> list)
        {
            list.ForEach(hs =>
            {
                CloneToClipboard(hs, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsHistorians()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsHistorians;
        }

        internal List<UFUAModel.UFUAHistorianSettings> PasteClipboardHistorians()
        {
            var ret = new List<UFUAModel.UFUAHistorianSettings>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUAHistorianSettings>(uowClipboard).AsParallel()
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var listName = GetHistoricalSettingsNameList(inExecution: false);
            listToCopy.ForEach(hs =>
            {
                var newName = NewHistoricalSettingsName(hs.Name, mapStartCounter, listName);
                var newhs = CloneFromClipboard(hs, hs.Name != newName) as UFUAModel.UFUAHistorianSettings;
                newhs.Name = newName;
                ret.Add(newhs);
            });
            return ret;
        }
#endif
        #endregion

        #region Data Loggers (Clipboard)
#if !NET_STANDARD
        internal void CopyListDataLoggerSettingsToClipbaord(List<DataLoggerModel.DataLoggerSettings> list)
        {
            list.ForEach(area =>
            {
                CloneToClipboard(area, false);
            });
            uowClipboard.CommitChanges();
        }

        internal void CopyListDataLoggerColumnToClipbaord(List<DataLoggerModel.DataLoggerColumn> list)
        {
            list.ForEach(src =>
            {
                CloneToClipboard(src, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsDataLoggerSettings()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsDataLoggerSettings;
        }

        internal bool ClipboardContainsDataLoggerColumn()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsDataLoggerColumn;
        }

        internal List<DataLoggerModel.DataLoggerSettings> PasteClipboardDataLoggerSettings()
        {
            var ret = new List<DataLoggerModel.DataLoggerSettings>();
            var listToCopy = (from c in new XPQuery<DataLoggerModel.DataLoggerSettings>(uowClipboard).AsParallel()
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var listName = GetDataLoggerSettingsNames();
            listToCopy.ForEach(datalogger =>
            {
                var exist = listName.Contains(datalogger.Name);
                var newName = NewDataLoggerSettingsName(datalogger.Name, mapStartCounter, listName);
                var newDataLogger = CloneFromClipboard(datalogger, exist) as DataLoggerModel.DataLoggerSettings;
                newDataLogger.Name = newName;
                ret.Add(newDataLogger);
            });
            return ret;
        }

        internal List<DataLoggerModel.DataLoggerColumn> PasteClipboardDataLoggerColumn(DataLoggerModel.DataLoggerSettings datalogger, DataLoggerModel.DataLoggerSettings parent = null)
        {
            var ret = new List<DataLoggerModel.DataLoggerColumn>();
            var listToCopy = (from c in new XPQuery<DataLoggerModel.DataLoggerColumn>(uowClipboard).AsParallel()
                              where c.DataLoggerReference == parent
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var listName = GetDataLoggerColumnNameList(datalogger);
            listToCopy.ForEach(column =>
            {
                var exist = listName.Contains(column.ColumnName);
                var newName = NewDataLoggerColumnName(datalogger, column.ColumnName, mapStartCounter, listName);
                var newColumn = CloneFromClipboard(column, exist) as DataLoggerModel.DataLoggerColumn;
                newColumn.ColumnName = newName;
                if (datalogger != null)
                    datalogger.Columns.Add(newColumn);
                ret.Add(newColumn);
            });
            mapStartCounter.Clear();

            return ret;
        }
#endif
        #endregion

        #region EnginneringUnits (Clipboard)
#if !NET_STANDARD
        internal void CopyListEngineeringUnitsToClipbaord(List<UFUAModel.UFUAEngineeringUnit> list)
        {
            list.ForEach(eu =>
            {
                CloneToClipboard(eu, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsEngineeringUnits()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsEngineeringUnits;
        }

        internal List<UFUAModel.UFUAEngineeringUnit> PasteClipboardEngineeringUnits()
        {
            var ret = new List<UFUAModel.UFUAEngineeringUnit>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUAEngineeringUnit>(uowClipboard).AsParallel()
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var listName = GetEngineeringUnitsNameList();
            listToCopy.ForEach(eu =>
            {
                var newName = NewEngineeringUnitsName(eu.Name, mapStartCounter, listName);
                var neweu = CloneFromClipboard(eu, eu.Name != newName) as UFUAModel.UFUAEngineeringUnit;
                neweu.Name = newName;
                ret.Add(neweu);
            });
            return ret;
        }
#endif
        #endregion

        #region Alarms (Clipboard)
#if !NET_STANDARD
        internal void CopyListAlarmAreasToClipbaord(List<UFUAModel.UFUAArea> list)
        {
            list.ForEach(area =>
            {
                CloneToClipboard(area, false);
            });
            uowClipboard.CommitChanges();
        }

        internal void CopyListAlarmSourcesToClipbaord(List<UFUAModel.UFUAAlarmSource> list)
        {
            list.ForEach(src =>
            {
                CloneToClipboard(src, false);
            });
            uowClipboard.CommitChanges();
        }

        internal void CopyListAlarmDefinitionsToClipbaord(List<UFUAModel.UFUAAlarmDefinition> list)
        {
            list.ForEach(alr =>
            {
                CloneToClipboard(alr, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsAlarmAreas()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsAlarmAreas;
        }

        internal bool ClipboardContainsAlarmSources()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsAlarmSources;
        }

        internal bool ClipboardContainsAlarmDefinitions()
        {
            if (uowClipboard == null)
                return false;

            return clipboardElements.ContainsAlarmDefinitions;
        }

        internal List<UFUAModel.UFUAArea> PasteClipboardAlarmAreas(UFUAModel.UFUAArea source, UFUAModel.UFUAArea parent = null)
        {
            var ret = new List<UFUAModel.UFUAArea>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUAArea>(uowClipboard).AsParallel()
                              where c.UFUAAreaAss == parent
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new GuidHelper(uow, TypeObject.AlarmArea);
            var listName = GetAlarmAreasNameList(source);
            listToCopy.ForEach(area =>
            {
                var exist = guidHelper.Contains(area.NodeId);
                var newName = NewAlarmAreaName(source, area.Name, mapStartCounter, listName);
                var newarea = CloneFromClipboard(area, exist) as UFUAModel.UFUAArea;
                newarea.Name = newName;
                if (source != null)
                    source.UFUAAreas.Add(newarea);
                ret.Add(newarea);
                if (!exist)
                    guidHelper.EnsureValidGuid(newarea);
            });
            return ret;
        }

        internal List<UFUAModel.UFUAAlarmSource> PasteClipboardAlarmSources(UFUAModel.UFUAArea source, UFUAModel.UFUAArea parent = null)
        {
            var ret = new List<UFUAModel.UFUAAlarmSource>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUAAlarmSource>(uowClipboard).AsParallel()
                              where c.UFUAArea == parent
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new GuidHelper(uow, TypeObject.AlarmSource);
            var listName = GetAlarmSourcesNameList(source);
            listToCopy.ForEach(src =>
            {
                var exist = guidHelper.Contains(src.NodeId);
                var newName = NewAlarmSourceName(source, src.Name, mapStartCounter, listName);
                var newsrc = CloneFromClipboard(src, exist) as UFUAModel.UFUAAlarmSource;
                newsrc.Name = newName;
                if (source != null)
                    source.UFUAAlarmSources.Add(newsrc);
                ret.Add(newsrc);
                if (!exist)
                    guidHelper.EnsureValidGuid(newsrc);
            });
            mapStartCounter.Clear();

            return ret;
        }

        internal List<UFUAModel.UFUAAlarmDefinition> PasteClipboardAlarmDefinitions(UFUAModel.UFUAAlarmSource source, UFUAModel.UFUAAlarmSource parent = null)
        {
            var ret = new List<UFUAModel.UFUAAlarmDefinition>();
            var listToCopy = (from c in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uowClipboard).AsParallel()
                              where c.UFUAAlarmDefinitions == parent
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new GuidHelper(uow, TypeObject.AlarmDefinition);
            var listName = GetAlarmDefinitionsNameList(source);
            listToCopy.ForEach(alr =>
            {
                var exist = guidHelper.Contains(alr.NodeId);
                var newName = NewAlarmPrototypeName(source, alr.Name, mapStartCounter, listName);
                var newalr = CloneFromClipboard(alr, exist) as UFUAModel.UFUAAlarmDefinition;
                newalr.Name = newName;
                if (source != null)
                    source.UFUAAlarmDefinitions.Add(newalr);
                ret.Add(newalr);
            });
            return ret;
        }
#endif
        #endregion

        #region OnDocumentChanged Event
#if !NET_STANDARD
        public event EventHandler<ChangedDocumentEvent> ChangedDocument;
        internal void OnChangedDocument(object source, ChangedType type, System.Collections.ICollection changedObjects)
        {
            OnChangedDocument(source, type, changedObjects, null);
        }

        internal void OnChangedDocument(object source, ChangedType type, System.Collections.ICollection changedObjects, System.Collections.ICollection originalObjects)
        {
            var tags = new List<UFUAModel.UFUATag>();
            var prototypes = new List<UFUAModel.UFUATagPrototype>();
            foreach (var changed in changedObjects)
            {
                if (changed is UFUAModel.UFUAFolder ||
                    changed is UFUAModel.UFUATag ||
                    changed is UFUAModel.UFUATagPrototype ||
                    changed is UFUAModel.UFUACommunicationDriver)
                {
                    bInvalidateDriversDynamicSettingsOnSave = true;
                }

                UFUATagPrototype prototype = null;
                if (changed is UFUAModel.UFUAFolder)
                {
                    var folder = changed as UFUAModel.UFUAFolder;
                    prototype = folder.PrototypeReference;
                }
                else if (changed is UFUAModel.UFUATag)
                {
                    var tag = changed as UFUAModel.UFUATag;
                    prototype = tag.PrototypeReference;
                }

                if (prototype != null && prototype.UFUATagOwner == null && !prototypes.Contains(prototype))
                {
                    prototypes.Add(prototype);
                    prototype.InvalidateMembersOrderId();
                    var prototypeName = prototype.Name;
                    var prototypeNodeId = prototype.NodeId.ToString();
                    tags.AddRange((from c in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                   where c.ModelType == ModelType.ObjectType && 
                                   (c.PrototypeModel == prototypeName || c.PrototypeModel == prototypeNodeId)
                                   select c).ToList().AsParallel().Where(c => !c.IsPrototypeMember || c.IsSubPrototypeMember));
                }

                if (changed is UFUAModel.UFUATag)
                {
                    var tag = changed as UFUAModel.UFUATag;
                    if (!tag.IsPrototypeMember)
                    {
                        string relativePath = String.IsNullOrEmpty(tag.FolderPath) ? String.Format("{0}", tag.Name) : String.Format("{0}\\{1}", tag.FolderPath, tag.Name);
                        if (type == ChangedType.changed)
                        {
                            UFUAModel.UFUATag nestedObject = null;
                            if (lastNestedUnitOfWork != null && lastNestedUnitOfWork.InTransaction)
                                nestedObject = lastNestedUnitOfWork.GetNestedObject(changed) as UFUAModel.UFUATag;
                            if (nestedObject == null && originalObjects != null)
                            {
                                foreach (var original in originalObjects)
                                {
                                    if (original is UFUAModel.UFUATag && (original as UFUAModel.UFUATag).NodeId == tag.NodeId)
                                    {
                                        nestedObject = tag;
                                        tag = original as UFUAModel.UFUATag;
                                        relativePath = String.IsNullOrEmpty(tag.FolderPath) ? String.Format("{0}", tag.Name) : String.Format("{0}\\{1}", tag.FolderPath, tag.Name);
                                        break;
                                    }
                                }
                            }
                            if (nestedObject != null)
                            {
                                string newPath = String.IsNullOrEmpty(nestedObject.FolderPath) ? String.Format("{0}", nestedObject.Name) : String.Format("{0}\\{1}", nestedObject.FolderPath, nestedObject.Name);
                                if (nestedObject.Name != tag.Name)
                                {
                                    lock (tagsListLock)
                                    {
                                        if (listFlatFullTagNameCollectionAdded == null)
                                            listFlatFullTagNameCollectionAdded = new List<string>();
                                        listFlatFullTagNameCollectionAdded.Remove(relativePath);
                                        if (!listFlatFullTagNameCollectionAdded.Contains(newPath))
                                            listFlatFullTagNameCollectionAdded.Add(newPath);

                                        if (listFlatFullTagNameCollectionRemoved == null)
                                            listFlatFullTagNameCollectionRemoved = new List<string>();
                                        listFlatFullTagNameCollectionRemoved.Remove(newPath);
                                        if (!listFlatFullTagNameCollectionRemoved.Contains(relativePath))
                                            listFlatFullTagNameCollectionRemoved.Add(relativePath);


                                        if (!String.IsNullOrEmpty(nestedObject.HistorianSettings))
                                        {
                                            if (listFlatFullHistoricalTagNameCollectionAdded == null)
                                                listFlatFullHistoricalTagNameCollectionAdded = new List<string>();
                                            listFlatFullHistoricalTagNameCollectionAdded.Remove(relativePath);
                                            if (!listFlatFullHistoricalTagNameCollectionAdded.Contains(newPath))
                                                listFlatFullHistoricalTagNameCollectionAdded.Add(newPath);

                                            if (listFlatFullHistoricalTagNameCollectionRemoved == null)
                                                listFlatFullHistoricalTagNameCollectionRemoved = new List<string>();
                                            listFlatFullHistoricalTagNameCollectionRemoved.Remove(newPath);
                                            if (!listFlatFullHistoricalTagNameCollectionRemoved.Contains(relativePath))
                                                listFlatFullHistoricalTagNameCollectionRemoved.Add(relativePath);
                                        }
                                    }
                                }

                                if (!String.IsNullOrEmpty(tag.PrototypeModel))
                                {
                                    var oldPaths = GetFlatFullTagNameCollection(tag.PrototypeModel, relativePath, true, false);
                                    foreach (var oldPath in oldPaths)
                                    {
                                        if (listFlatFullTagNameCollectionRemoved == null)
                                            listFlatFullTagNameCollectionRemoved = new List<string>();
                                        if (!listFlatFullTagNameCollectionRemoved.Contains(oldPath))
                                            listFlatFullTagNameCollectionRemoved.Add(oldPath);
                                        if (listFlatFullTagNameCollectionAdded != null && listFlatFullTagNameCollectionAdded.Contains(oldPath))
                                            listFlatFullTagNameCollectionAdded.Remove(oldPath);
                                    }

                                    oldPaths = GetFlatFullTagNameCollection(tag.PrototypeModel, relativePath, true, true);
                                    foreach (var oldPath in oldPaths)
                                    {
                                        if (listFlatFullHistoricalTagNameCollectionRemoved == null)
                                            listFlatFullHistoricalTagNameCollectionRemoved = new List<string>();
                                        if (!listFlatFullHistoricalTagNameCollectionRemoved.Contains(oldPath))
                                            listFlatFullHistoricalTagNameCollectionRemoved.Add(oldPath);
                                        if (listFlatFullHistoricalTagNameCollectionAdded != null && listFlatFullHistoricalTagNameCollectionAdded.Contains(oldPath))
                                            listFlatFullHistoricalTagNameCollectionAdded.Remove(oldPath);
                                    }
                                }

                                if (!String.IsNullOrEmpty(nestedObject.PrototypeModel))
                                {
                                    var newPaths = GetFlatFullTagNameCollection(nestedObject.PrototypeModel, newPath, true, false);
                                    foreach (var newPath2 in newPaths)
                                    {
                                        if (listFlatFullTagNameCollectionAdded == null)
                                            listFlatFullTagNameCollectionAdded = new List<string>();
                                        if (!listFlatFullTagNameCollectionAdded.Contains(newPath2))
                                            listFlatFullTagNameCollectionAdded.Add(newPath2);
                                        if (listFlatFullTagNameCollectionRemoved != null)
                                            listFlatFullTagNameCollectionRemoved.Remove(newPath2);
                                    }

                                    newPaths = GetFlatFullTagNameCollection(nestedObject.PrototypeModel, newPath, true, true);
                                    foreach (var newPath2 in newPaths)
                                    {
                                        if (listFlatFullHistoricalTagNameCollectionAdded == null)
                                            listFlatFullHistoricalTagNameCollectionAdded = new List<string>();
                                        if (!listFlatFullHistoricalTagNameCollectionAdded.Contains(newPath2))
                                            listFlatFullHistoricalTagNameCollectionAdded.Add(newPath2);
                                        if (listFlatFullHistoricalTagNameCollectionRemoved != null)
                                            listFlatFullHistoricalTagNameCollectionRemoved.Remove(newPath2);
                                    }
                                }                                

                                if (!String.IsNullOrEmpty(nestedObject.HistorianSettings) != !String.IsNullOrEmpty(tag.HistorianSettings))
                                {
                                    if (!String.IsNullOrEmpty(tag.HistorianSettings))
                                    {
                                        lock (tagsListLock)
                                        {
                                            if (listFlatFullHistoricalTagNameCollectionAdded != null)
                                                listFlatFullHistoricalTagNameCollectionAdded.Remove(relativePath);
                                            if (listFlatFullHistoricalTagNameCollectionRemoved == null)
                                                listFlatFullHistoricalTagNameCollectionRemoved = new List<string>();
                                            listFlatFullHistoricalTagNameCollectionRemoved.Remove(newPath);
                                            if (!listFlatFullHistoricalTagNameCollectionRemoved.Contains(relativePath))
                                                listFlatFullHistoricalTagNameCollectionRemoved.Add(relativePath);
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(nestedObject.HistorianSettings))
                                    {
                                        lock (tagsListLock)
                                        {
                                            if (listFlatFullHistoricalTagNameCollectionAdded == null)
                                                listFlatFullHistoricalTagNameCollectionAdded = new List<string>();
                                            if (!listFlatFullHistoricalTagNameCollectionAdded.Contains(newPath))
                                                listFlatFullHistoricalTagNameCollectionAdded.Add(newPath);
                                        }
                                    }
                                }
                            }
                        }
                        else if (type == ChangedType.added)
                        {
                            lock (tagsListLock)
                            {
                                if (listFlatFullTagNameCollectionAdded == null)
                                    listFlatFullTagNameCollectionAdded = new List<string>();
                                if (!listFlatFullTagNameCollectionAdded.Contains(relativePath))
                                    listFlatFullTagNameCollectionAdded.Add(relativePath);
                                if (listFlatFullTagNameCollectionRemoved != null)
                                    listFlatFullTagNameCollectionRemoved.Remove(relativePath);
                                if (!String.IsNullOrEmpty(tag.HistorianSettings))
                                {
                                    if (listFlatFullHistoricalTagNameCollectionAdded == null)
                                        listFlatFullHistoricalTagNameCollectionAdded = new List<string>();
                                    if (!listFlatFullHistoricalTagNameCollectionAdded.Contains(relativePath))
                                        listFlatFullHistoricalTagNameCollectionAdded.Add(relativePath);
                                    if (listFlatFullHistoricalTagNameCollectionRemoved != null)
                                        listFlatFullHistoricalTagNameCollectionRemoved.Remove(relativePath);
                                }
                                if (tag.ModelType == ModelType.ObjectType && !String.IsNullOrEmpty(tag.PrototypeModel))
                                {
                                    var newPaths = GetFlatFullTagNameCollection(tag.PrototypeModel, relativePath, true, false);
                                    foreach (var newPath2 in newPaths)
                                    {
                                        if (listFlatFullTagNameCollectionAdded == null)
                                            listFlatFullTagNameCollectionAdded = new List<string>();
                                        if (!listFlatFullTagNameCollectionAdded.Contains(newPath2))
                                            listFlatFullTagNameCollectionAdded.Add(newPath2);
                                        if (listFlatFullTagNameCollectionRemoved != null)
                                            listFlatFullTagNameCollectionRemoved.Remove(newPath2);
                                    }

                                    newPaths = GetFlatFullTagNameCollection(tag.PrototypeModel, relativePath, true, true);
                                    foreach (var newPath2 in newPaths)
                                    {
                                        if (listFlatFullHistoricalTagNameCollectionAdded == null)
                                            listFlatFullHistoricalTagNameCollectionAdded = new List<string>();
                                        if (!listFlatFullHistoricalTagNameCollectionAdded.Contains(newPath2))
                                            listFlatFullHistoricalTagNameCollectionAdded.Add(newPath2);
                                        if (listFlatFullHistoricalTagNameCollectionRemoved != null)
                                            listFlatFullHistoricalTagNameCollectionRemoved.Remove(newPath2);
                                    }
                                }
                            }
                        }
                        else if (type == ChangedType.removed)
                        {
                            lock (tagsListLock)
                            {
                                if (listFlatFullTagNameCollectionRemoved == null)
                                    listFlatFullTagNameCollectionRemoved = new List<string>();
                                if (!listFlatFullTagNameCollectionRemoved.Contains(relativePath))
                                    listFlatFullTagNameCollectionRemoved.Add(relativePath);
                                if (listFlatFullTagNameCollectionAdded != null && listFlatFullTagNameCollectionAdded.Contains(relativePath))
                                    listFlatFullTagNameCollectionAdded.Remove(relativePath);
                                if (!String.IsNullOrEmpty(tag.HistorianSettings))
                                {
                                    if (listFlatFullHistoricalTagNameCollectionRemoved == null)
                                        listFlatFullHistoricalTagNameCollectionRemoved = new List<string>();
                                    if (!listFlatFullHistoricalTagNameCollectionRemoved.Contains(relativePath))
                                        listFlatFullHistoricalTagNameCollectionRemoved.Add(relativePath);
                                    if (listFlatFullHistoricalTagNameCollectionAdded != null && listFlatFullHistoricalTagNameCollectionAdded.Contains(relativePath))
                                        listFlatFullHistoricalTagNameCollectionAdded.Remove(relativePath);

                                }
                                if (tag.ModelType == ModelType.ObjectType && !String.IsNullOrEmpty(tag.PrototypeModel))
                                {
                                    var oldPaths = GetFlatFullTagNameCollection(tag.PrototypeModel, relativePath, true, false);
                                    foreach (var oldPath in oldPaths)
                                    {
                                        if (listFlatFullTagNameCollectionRemoved == null)
                                            listFlatFullTagNameCollectionRemoved = new List<string>();
                                        if (!listFlatFullTagNameCollectionRemoved.Contains(oldPath))
                                            listFlatFullTagNameCollectionRemoved.Add(oldPath);
                                        if (listFlatFullTagNameCollectionAdded != null && listFlatFullTagNameCollectionAdded.Contains(oldPath))
                                            listFlatFullTagNameCollectionAdded.Remove(oldPath);
                                    }

                                    oldPaths = GetFlatFullTagNameCollection(tag.PrototypeModel, relativePath, true, true);
                                    foreach (var oldPath in oldPaths)
                                    {
                                        if (listFlatFullHistoricalTagNameCollectionRemoved == null)
                                            listFlatFullHistoricalTagNameCollectionRemoved = new List<string>();
                                        if (!listFlatFullHistoricalTagNameCollectionRemoved.Contains(oldPath))
                                            listFlatFullHistoricalTagNameCollectionRemoved.Add(oldPath);
                                        if (listFlatFullHistoricalTagNameCollectionAdded != null && listFlatFullHistoricalTagNameCollectionAdded.Contains(oldPath))
                                            listFlatFullHistoricalTagNameCollectionAdded.Remove(oldPath);
                                    }
                                }
                            }
                        }
                    }
                    else if (!tag.IsSubPrototypeMember)
                    {
                        string relativePath = String.IsNullOrEmpty(tag.FolderPath) ? String.Format(":{0}", tag.Name) : String.Format(":{0}\\{1}", tag.FolderPath, tag.Name);
                        if (type == ChangedType.changed)
                        {
                            UFUAModel.UFUATag nestedObject = null;
                            if (lastNestedUnitOfWork != null && lastNestedUnitOfWork.InTransaction)
                                nestedObject = lastNestedUnitOfWork.GetNestedObject(changed) as UFUAModel.UFUATag;
                            if (nestedObject == null && originalObjects != null)
                            {
                                foreach (var original in originalObjects)
                                {
                                    if (original is UFUAModel.UFUATag && (original as UFUAModel.UFUATag).NodeId == tag.NodeId)
                                    {
                                        nestedObject = tag;
                                        tag = original as UFUAModel.UFUATag;
                                        relativePath = String.IsNullOrEmpty(tag.FolderPath) ? String.Format(":{0}", tag.Name) : String.Format(":{0}\\{1}", tag.FolderPath, tag.Name);
                                        break;
                                    }
                                }
                            }
                            if (nestedObject != null)
                            {
                                string newPath = String.IsNullOrEmpty(nestedObject.FolderPath) ? String.Format(":{0}", nestedObject.Name) : String.Format(":{0}\\{1}", nestedObject.FolderPath, nestedObject.Name);
                                if (nestedObject.Name != tag.Name)
                                {
                                    lock (tagsListLock)
                                    {
                                        if (listFlatFullTagNameCollectionAdded != null)
                                        {
                                            var foundPaths = (from c in listFlatFullTagNameCollectionAdded.AsParallel()
                                                              where c.EndsWith(relativePath)
                                                              select c).ToList();

                                            foreach (var pathToRemove in foundPaths)
                                            {
                                                var pathToAdd = String.Format("{0}{1}", pathToRemove.Substring(0, pathToRemove.IndexOf(':')), newPath);
                                                listFlatFullTagNameCollectionAdded.Remove(pathToRemove);
                                                if (!listFlatFullTagNameCollectionAdded.Contains(pathToAdd))
                                                    listFlatFullTagNameCollectionAdded.Add(pathToAdd);
                                            }
                                        }

                                        if (listFlatFullTagNameCollectionRemoved != null)
                                        {
                                            var foundPaths = (from c in listFlatFullTagNameCollectionRemoved.AsParallel()
                                                              where c.EndsWith(newPath)
                                                              select c).ToList();

                                            foreach (var pathToRemove in foundPaths)
                                            {
                                                var pathToAdd = String.Format("{0}{1}", pathToRemove.Substring(0, pathToRemove.IndexOf(':')), relativePath);
                                                listFlatFullTagNameCollectionRemoved.Remove(pathToRemove);
                                                if (!listFlatFullTagNameCollectionRemoved.Contains(pathToAdd))
                                                    listFlatFullTagNameCollectionRemoved.Add(pathToAdd);
                                            }
                                        }

                                        if (!String.IsNullOrEmpty(nestedObject.HistorianSettings))
                                        {
                                            if (listFlatFullHistoricalTagNameCollectionAdded != null)
                                            {
                                                var foundPaths = (from c in listFlatFullHistoricalTagNameCollectionAdded.AsParallel()
                                                                  where c.EndsWith(relativePath)
                                                                  select c).ToList();

                                                foreach (var pathToRemove in foundPaths)
                                                {
                                                    var pathToAdd = String.Format("{0}{1}", pathToRemove.Substring(0, pathToRemove.IndexOf(':')), newPath);
                                                    listFlatFullHistoricalTagNameCollectionAdded.Remove(pathToRemove);
                                                    if (!listFlatFullHistoricalTagNameCollectionAdded.Contains(pathToAdd))
                                                        listFlatFullHistoricalTagNameCollectionAdded.Add(pathToAdd);
                                                }
                                            }

                                            if (listFlatFullHistoricalTagNameCollectionRemoved != null)
                                            {
                                                var foundPaths = (from c in listFlatFullHistoricalTagNameCollectionRemoved.AsParallel()
                                                                  where c.EndsWith(newPath)
                                                                  select c).ToList();

                                                foreach (var pathToRemove in foundPaths)
                                                {
                                                    var pathToAdd = String.Format("{0}{1}", pathToRemove.Substring(0, pathToRemove.IndexOf(':')), relativePath);
                                                    listFlatFullHistoricalTagNameCollectionRemoved.Remove(pathToRemove);
                                                    if (!listFlatFullHistoricalTagNameCollectionRemoved.Contains(pathToAdd))
                                                        listFlatFullHistoricalTagNameCollectionRemoved.Add(pathToAdd);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!String.IsNullOrEmpty(nestedObject.HistorianSettings) != !String.IsNullOrEmpty(tag.HistorianSettings))
                                {
                                    var prototypeName = prototype.Name;
                                    var prototypeNodeId = prototype.NodeId.ToString();
                                    var instances = (from c in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                                     where c.ModelType == ModelType.ObjectType &&
                                                     (c.PrototypeModel == prototypeName || c.PrototypeModel == prototypeNodeId)
                                                     select c).ToList().AsParallel().Where(c => !c.IsPrototypeMember);

                                    if (!String.IsNullOrEmpty(tag.HistorianSettings))
                                    {
                                        lock (tagsListLock)
                                        {
                                            foreach (var instance in instances)
                                            {
                                                string pathToRemove = String.IsNullOrEmpty(instance.FolderPath) ? String.Format("{0}", instance.Name) : String.Format("{0}\\{1}", instance.FolderPath, instance.Name);
                                                pathToRemove = String.Format("{0}{1}", pathToRemove, relativePath);

                                                if (listFlatFullHistoricalTagNameCollectionRemoved == null)
                                                    listFlatFullHistoricalTagNameCollectionRemoved = new List<string>();
                                                if (!listFlatFullHistoricalTagNameCollectionRemoved.Contains(pathToRemove))
                                                    listFlatFullHistoricalTagNameCollectionRemoved.Add(pathToRemove);
                                                if (listFlatFullHistoricalTagNameCollectionAdded != null && listFlatFullHistoricalTagNameCollectionAdded.Contains(pathToRemove))
                                                    listFlatFullHistoricalTagNameCollectionAdded.Remove(pathToRemove);
                                            }
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(nestedObject.HistorianSettings))
                                    {
                                        lock (tagsListLock)
                                        {
                                            foreach (var instance in instances)
                                            {
                                                string pathToAdd = String.IsNullOrEmpty(instance.FolderPath) ? String.Format("{0}", instance.Name) : String.Format("{0}\\{1}", instance.FolderPath, instance.Name);
                                                pathToAdd = String.Format("{0}{1}", pathToAdd, newPath);

                                                if (listFlatFullHistoricalTagNameCollectionAdded == null)
                                                    listFlatFullHistoricalTagNameCollectionAdded = new List<string>();
                                                if (!listFlatFullHistoricalTagNameCollectionAdded.Contains(pathToAdd))
                                                    listFlatFullHistoricalTagNameCollectionAdded.Add(pathToAdd);
                                                if (listFlatFullHistoricalTagNameCollectionRemoved != null && listFlatFullHistoricalTagNameCollectionRemoved.Contains(pathToAdd))
                                                    listFlatFullHistoricalTagNameCollectionRemoved.Remove(pathToAdd);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (type == ChangedType.added)
                        {
                            var prototypeName = prototype.Name;
                            var prototypeNodeId = prototype.NodeId.ToString();
                            var instances = (from c in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                             where c.ModelType == ModelType.ObjectType &&
                                             (c.PrototypeModel == prototypeName || c.PrototypeModel == prototypeNodeId)
                                             select c).ToList().AsParallel().Where(c => !c.IsPrototypeMember);

                            lock (tagsListLock)
                            {
                                foreach (var instance in instances)
                                {
                                    string newPath = String.IsNullOrEmpty(instance.FolderPath) ? String.Format("{0}", instance.Name) : String.Format("{0}\\{1}", instance.FolderPath, instance.Name);
                                    newPath = String.Format("{0}{1}", newPath, relativePath);

                                    lock (tagsListLock)
                                    {
                                        if (listFlatFullTagNameCollectionAdded == null)
                                            listFlatFullTagNameCollectionAdded = new List<string>();
                                        if (!listFlatFullTagNameCollectionAdded.Contains(newPath))
                                            listFlatFullTagNameCollectionAdded.Add(newPath);
                                        if (listFlatFullTagNameCollectionRemoved != null)
                                            listFlatFullTagNameCollectionRemoved.Remove(newPath);
                                        if (!String.IsNullOrEmpty(tag.HistorianSettings))
                                        {
                                            if (listFlatFullHistoricalTagNameCollectionAdded == null)
                                                listFlatFullHistoricalTagNameCollectionAdded = new List<string>();
                                            if (!listFlatFullHistoricalTagNameCollectionAdded.Contains(newPath))
                                                listFlatFullHistoricalTagNameCollectionAdded.Add(newPath);
                                            if (listFlatFullHistoricalTagNameCollectionRemoved != null)
                                                listFlatFullHistoricalTagNameCollectionRemoved.Remove(newPath);
                                        }
                                    }
                                }
                            }
                        }
                        else if (type == ChangedType.removed)
                        {
                            var prototypeName = prototype.Name;
                            var prototypeNodeId = prototype.NodeId.ToString();
                            var instances = (from c in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                                             where c.ModelType == ModelType.ObjectType &&
                                             (c.PrototypeModel == prototypeName || c.PrototypeModel == prototypeNodeId)
                                             select c).ToList().AsParallel().Where(c => !c.IsPrototypeMember);

                            lock (tagsListLock)
                            {
                                foreach (var instance in instances)
                                {
                                    string newPath = String.IsNullOrEmpty(instance.FolderPath) ? String.Format("{0}", instance.Name) : String.Format("{0}\\{1}", instance.FolderPath, instance.Name);
                                    newPath = String.Format("{0}{1}", newPath, relativePath);

                                    lock (tagsListLock)
                                    {
                                        if (listFlatFullTagNameCollectionRemoved == null)
                                            listFlatFullTagNameCollectionRemoved = new List<string>();
                                        if (!listFlatFullTagNameCollectionRemoved.Contains(newPath))
                                            listFlatFullTagNameCollectionRemoved.Add(newPath);
                                        if (listFlatFullTagNameCollectionAdded != null && listFlatFullTagNameCollectionAdded.Contains(newPath))
                                            listFlatFullTagNameCollectionAdded.Remove(newPath);
                                        if (!String.IsNullOrEmpty(tag.HistorianSettings))
                                        {
                                            if (listFlatFullHistoricalTagNameCollectionRemoved == null)
                                                listFlatFullHistoricalTagNameCollectionRemoved = new List<string>();
                                            if (!listFlatFullHistoricalTagNameCollectionRemoved.Contains(newPath))
                                                listFlatFullHistoricalTagNameCollectionRemoved.Add(newPath);
                                            if (listFlatFullHistoricalTagNameCollectionAdded != null && listFlatFullHistoricalTagNameCollectionAdded.Contains(newPath))
                                                listFlatFullHistoricalTagNameCollectionAdded.Remove(newPath);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (changed is UFUAModel.UFUAFolder)
                {
                    var folder = changed as UFUAModel.UFUAFolder;
                    if (!folder.IsPrototypeMember)
                    {
                        if (type == ChangedType.changed)
                        {
                            UFUAModel.UFUAFolder nestedObject = null;
                            if (lastNestedUnitOfWork != null && lastNestedUnitOfWork.InTransaction)
                                nestedObject = lastNestedUnitOfWork.GetNestedObject(changed) as UFUAModel.UFUAFolder;
                            if (nestedObject != null && !nestedObject.IsPrototypeMember)
                            {
                                lock (tagsListLock)
                                {
                                    //Folder
                                    if (listFlatFullFolderNameCollectionAdded == null)
                                        listFlatFullFolderNameCollectionAdded = new List<string>();
                                    listFlatFullFolderNameCollectionAdded.Remove(folder.PathIdentifier);
                                    if (!listFlatFullFolderNameCollectionAdded.Contains(nestedObject.PathIdentifier))
                                        listFlatFullFolderNameCollectionAdded.Add(nestedObject.PathIdentifier);

                                    if (listFlatFullFolderNameCollectionRemoved == null)
                                        listFlatFullFolderNameCollectionRemoved = new List<string>();
                                    listFlatFullFolderNameCollectionRemoved.Remove(folder.PathIdentifier);
                                    listFlatFullFolderNameCollectionRemoved.Remove(nestedObject.PathIdentifier);
                                }
                            }
                        }
                        else if (type == ChangedType.added )
                        {
                            lock (tagsListLock)
                            {
                                if (listFlatFullFolderNameCollectionAdded == null)
                                    listFlatFullFolderNameCollectionAdded = new List<string>();
                                listFlatFullFolderNameCollectionAdded.Remove(folder.PathIdentifier);
                                if (!listFlatFullFolderNameCollectionAdded.Contains(folder.PathIdentifier))
                                    listFlatFullFolderNameCollectionAdded.Add(folder.PathIdentifier);

                                if (listFlatFullFolderNameCollectionRemoved != null && listFlatFullFolderNameCollectionRemoved.Contains(folder.PathIdentifier))
                                    listFlatFullFolderNameCollectionRemoved.Remove(folder.PathIdentifier);
                            }
                        }
                        else if (type == ChangedType.removed )
                        {
                            lock (tagsListLock)
                            {
                                if (listFlatFullFolderNameCollectionAdded != null && listFlatFullFolderNameCollectionAdded.Contains(folder.PathIdentifier))
                                    listFlatFullFolderNameCollectionAdded.Remove(folder.PathIdentifier);

                                if (listFlatFullFolderNameCollectionRemoved == null)
                                    listFlatFullFolderNameCollectionRemoved = new List<string>();
                                listFlatFullFolderNameCollectionRemoved.Remove(folder.PathIdentifier);
                                if (!listFlatFullFolderNameCollectionRemoved.Contains(folder.PathIdentifier))
                                    listFlatFullFolderNameCollectionRemoved.Add(folder.PathIdentifier);
                            }
                        }
                    }
                }
            }

            if (tags.Count > 0)
                tags.ForEach((c) => c.IsSubPrototypeMembersCreated = false);

            ChangedDocument?.Invoke(this, new ChangedDocumentEvent(source, type, changedObjects));
            editorManagerComponent.OnChangedDocument(this, type, changedObjects);
        }
#endif
        #endregion

        #region Undo/Redo
#if !NET_STANDARD
        internal void AddUndoAction(UserControl owner, IXPSimpleObject source, UndoRedoAction action)
        {
            AddUndoAction(owner, new UndoRedoXpoData(source, owner), action);
        }

        internal void AddUndoAction(UserControl owner, IList<IXPSimpleObject> list, UndoRedoAction action)
        {
            var data = new UndoRedoXpoDataCollection();
            foreach (var source in list)
                data.Add(new UndoRedoXpoData(source, owner));
            AddUndoAction(owner, data, action);
        }

        internal void AddUndoAction(UserControl owner, UndoRedoXpoDataCollection list, UndoRedoAction action)
        {
            if (undoRedoHelper == null)
                return;

            undoRedoHelper.AddUndoAction(list, action);

            if (list.Count > 0)
            {
                switch (action)
                {
                    case UndoRedoAction.Added:
                        OnChangedDocument(owner, ChangedType.added, list.Sources);
                        break;

                    case UndoRedoAction.Changed:
                    case UndoRedoAction.Replaced:
                        OnChangedDocument(owner, ChangedType.changed, list.Sources);
                        break;

                    case UndoRedoAction.Removed:
                        OnChangedDocument(owner, ChangedType.removed, list.Sources);
                        break;
                }
            }
        }

        internal void AddUndoAction(UserControl owner, UndoRedoXpoData obj, UndoRedoAction action)
        {
            if (undoRedoHelper == null)
                return;

            undoRedoHelper.AddUndoAction(obj, action);

            switch (action)
            {
                case UndoRedoAction.Added:

                    OnChangedDocument(owner, ChangedType.added, new List<object>() { obj.Source });
                    break;

                case UndoRedoAction.Changed:
                case UndoRedoAction.Replaced:

                    OnChangedDocument(owner, ChangedType.changed, new List<object>() { obj.Source });
                    break;

                case UndoRedoAction.Removed:

                    OnChangedDocument(owner, ChangedType.removed, new List<object>() { obj.Source });
                    break;
            }
        }

        internal void AddRedoAction(UserControl owner, IXPSimpleObject source, UndoRedoAction action)
        {
            AddRedoAction(owner, new UndoRedoXpoData(source, owner), action);
        }

        internal void AddRedoAction(UserControl owner, IList<IXPSimpleObject> list, UndoRedoAction action)
        {
            var data = new UndoRedoXpoDataCollection();
            foreach (var source in list)
                data.Add(new UndoRedoXpoData(source, owner));
            AddRedoAction(owner, data, action);
        }

        internal void AddRedoAction(UserControl owner, UndoRedoXpoDataCollection list, UndoRedoAction action)
        {
            if (undoRedoHelper == null)
                return;

            undoRedoHelper.AddRedoAction(list, action);
        }

        internal void AddRedoAction(UserControl owner, UndoRedoXpoData obj, UndoRedoAction action)
        {
            if (undoRedoHelper == null)
                return;

            undoRedoHelper.AddRedoAction(obj, action);
        }

        internal bool UndoContainsSomething()
        {
            if (bObjectDisposed)
                return false;

            return undoRedoHelper != null && undoRedoHelper.CanUndo();
        }

        internal bool RedoContainsSomething()
        {
            if (bObjectDisposed)
                return false;

            return undoRedoHelper != null && undoRedoHelper.CanRedo();
        }

        internal String GetNextUndoOwner()
        {
            if (bObjectDisposed)
                return null;

            if (undoRedoHelper != null)
            {
                var data = undoRedoHelper.PeekNextUndo();
                if (data != null && data.Count > 0)
                    return data[0].OwnerTypeName;
            }
            return null;
        }

        internal String GetNextRedoOwner()
        {
            if (bObjectDisposed)
                return null;

            if (undoRedoHelper != null)
            {
                var data = undoRedoHelper.PeekNextRedo();
                if (data != null && data.Count > 0)
                    return data[0].OwnerTypeName;
            }
            return null;
        }

        internal UndoRedoXpoDataCollection UndoAction(UserControl owner, out UndoRedoAction action)
        {
            if (undoRedoHelper == null)
            {
                action = UndoRedoAction.None;
                return new UndoRedoXpoDataCollection();
            }

            if (undoRedoHelper.CanUndo())
            {
                var ret = undoRedoHelper.Undo(out action);
                if (action != UndoRedoAction.Replaced)
                {
                    var helper = new UFUAModel.Helpers.AssociationHelper(uow);
                    helper.CopyAssociationReferences(ret.Originals, ret.Sources);
                }
                undoRedoHelper.PurgeUndoActions();

                if (ret.Count > 0)
                {
                    switch (action)
                    {
                        case UndoRedoAction.Added:
                            OnChangedDocument(owner, ChangedType.removed, ret.Sources);
                            break;

                        case UndoRedoAction.Changed:
                        case UndoRedoAction.Replaced:
                            OnChangedDocument(owner, ChangedType.changed, ret.Sources, undoRedoHelper.PeekNextRedo()?.Sources);
                            break;

                        case UndoRedoAction.Removed:
                            foreach (var obj in ret)
                                AddExistingObject(obj);
                            OnChangedDocument(owner, ChangedType.added, ret.Sources);
                            break;
                    }
                }

                return ret;
            }

            action = UndoRedoAction.None;
            return null;
        }

        internal UndoRedoXpoDataCollection RedoAction(UserControl owner, out UndoRedoAction action)
        {
            if (undoRedoHelper == null)
            {
                action = UndoRedoAction.None;
                return new UndoRedoXpoDataCollection();
            }

            if (undoRedoHelper.CanRedo())
            {
                var ret = undoRedoHelper.Redo(out action);
                if (action != UndoRedoAction.Replaced)
                {
                    var helper = new UFUAModel.Helpers.AssociationHelper(uow);
                    helper.CopyAssociationReferences(ret.Originals, ret.Sources);
                }
                undoRedoHelper.PurgeRedoActions();

                if (ret.Count > 0)
                {
                    switch (action)
                    {
                        case UndoRedoAction.Added:
                            foreach (var obj in ret)
                                AddExistingObject(obj);
                            OnChangedDocument(owner, ChangedType.added, ret.Sources);
                            break;

                        case UndoRedoAction.Changed:
                        case UndoRedoAction.Replaced:
                            OnChangedDocument(owner, ChangedType.changed, ret.Sources, undoRedoHelper.PeekNextUndo()?.Sources);
                            break;

                        case UndoRedoAction.Removed:
                            OnChangedDocument(owner, ChangedType.removed, ret.Sources);
                            break;
                    }
                }

                return ret;
            }

            action = UndoRedoAction.None;
            return null;
        }

        internal XPObject GetParentObject(UndoRedoXpoData data)
        {
            if (data.Source is UFUAModel.UFUATag)
            {
                if (!String.IsNullOrEmpty(data.ParentIdentifier))
                {
                    if (!String.IsNullOrEmpty(data.OwnerIdentifier))
                    {
                        var ownerIds = data.OwnerIdentifier.Split('|');
                        if (ownerIds.Length > 1)
                            return FindSubPrototypeFolderByNodeId(Guid.Parse(data.ParentIdentifier), Guid.Parse(ownerIds[0]), Guid.Parse(ownerIds[1]));
                        else
                            return FindFolderByNodeId(Guid.Parse(data.ParentIdentifier), Guid.Parse(ownerIds[0]));
                    }
                    else
                        return FindFolderByNodeId(Guid.Parse(data.ParentIdentifier));
                }
                else if (!String.IsNullOrEmpty(data.OwnerIdentifier))
                {
                    var ownerIds = data.OwnerIdentifier.Split('|');
                    if (ownerIds.Length > 1)
                        return FindSubPrototypeByNodeId(Guid.Parse(ownerIds[0]), Guid.Parse(ownerIds[1]));
                    else
                        return FindPrototypeByNodeId(Guid.Parse(ownerIds[0]));
                }
            }
            else if (data.Source is UFUAModel.UFUAFolder)
            {
                if (!String.IsNullOrEmpty(data.ParentIdentifier))
                {
                    if (!String.IsNullOrEmpty(data.OwnerIdentifier))
                    {
                        var ownerIds = data.OwnerIdentifier.Split('|');
                        if (ownerIds.Length > 1)
                            return FindSubPrototypeFolderByNodeId(Guid.Parse(data.ParentIdentifier), Guid.Parse(ownerIds[0]), Guid.Parse(ownerIds[1]));
                        else
                            return FindFolderByNodeId(Guid.Parse(data.ParentIdentifier), Guid.Parse(ownerIds[0]));
                    }
                    else
                        return FindFolderByNodeId(Guid.Parse(data.ParentIdentifier));
                }
                else if (!String.IsNullOrEmpty(data.OwnerIdentifier))
                {
                    var ownerIds = data.OwnerIdentifier.Split('|');
                    if (ownerIds.Length > 1)
                        return FindSubPrototypeByNodeId(Guid.Parse(ownerIds[0]), Guid.Parse(ownerIds[1]));
                    else
                        return FindPrototypeByNodeId(Guid.Parse(ownerIds[0]));
                }
            }
            else if (data.Source is UFUAModel.UFUAArea)
            {
                if (!String.IsNullOrEmpty(data.ParentIdentifier))
                {
                    return FindAlarmAreaByNodeId(Guid.Parse(data.ParentIdentifier));
                }
            }
            else if (data.Source is UFUAModel.UFUAAlarmSource)
            {
                if (!String.IsNullOrEmpty(data.ParentIdentifier))
                {
                    return FindAlarmAreaByNodeId(Guid.Parse(data.ParentIdentifier));
                }
            }
            else if (data.Source is UFUAModel.UFUAAlarmDefinition)
            {
                if (!String.IsNullOrEmpty(data.ParentIdentifier))
                {
                    return FindAlarmSourceAreaByNodeId(Guid.Parse(data.ParentIdentifier));
                }
            }
            else if (data.Source is UFUAModel.UFUAAlarmThreshold)
            {
                if (!String.IsNullOrEmpty(data.ParentIdentifier))
                {
                    if (!String.IsNullOrEmpty(data.OwnerIdentifier))
                    {
                        var ownerIds = data.OwnerIdentifier.Split('|');
                        if (ownerIds.Length > 1)
                            return FindSubPrototypeMemberByNodeId(Guid.Parse(data.ParentIdentifier), Guid.Parse(ownerIds[0]), Guid.Parse(ownerIds[1]));
                        else
                            return FindTagByNodeId(Guid.Parse(data.ParentIdentifier), Guid.Parse(ownerIds[0]));
                    }
                    else
                        return FindTagByNodeId(Guid.Parse(data.ParentIdentifier));
                }
            }
            else if (data.Source is DataLoggerModel.DataLoggerColumn)
            {
                if (!String.IsNullOrEmpty(data.ParentIdentifier))
                {
                    return GetDataLogger(data.ParentIdentifier);
                }
            }

            return null;
        }

        void AddExistingObject(UndoRedoXpoData source)
        {
            AddExistingObject(source.Source, GetParentObject(source));
        }

        void AddExistingObject(XPObject obj, XPObject parent)
        {
            if (obj.IsDeleted)
                obj.SetMemberValue("GCRecord", null);

            if (obj is UFUAModel.UFUATag)
            {
                var tag = obj as UFUAModel.UFUATag;
                if (parent is UFUAModel.UFUAFolder)
                    AddExistingObject(tag, parent as UFUAFolder);
                else if (parent is UFUAModel.UFUATagPrototype)
                    AddExistingObject(tag, parent as UFUAModel.UFUATagPrototype);

                EnsureValidNodeId(tag);
            }
            else if (obj is UFUAModel.UFUATagPrototype)
            {
                var prototype = obj as UFUAModel.UFUATagPrototype;
                EnsureValidNodeId(prototype);
            }
            else if (obj is UFUAModel.UFUAFolder)
            {
                var folder = obj as UFUAModel.UFUAFolder;
                if (parent is UFUAModel.UFUAFolder)
                {
                    AddExistingObject(folder, parent as UFUAFolder);
                }
                else if (parent is UFUAModel.UFUATagPrototype)
                {
                    AddExistingObject(folder, parent as UFUAModel.UFUATagPrototype);
                }

                EnsureValidNodeId(folder);
            }
            else if (obj is UFUAModel.UFUAArea)
            {
                var area = obj as UFUAModel.UFUAArea;
                if (parent is UFUAModel.UFUAArea)
                {
                    AddExistingObject(area, parent as UFUAModel.UFUAArea);
                }
            }
            else if (obj is UFUAModel.UFUAAlarmSource)
            {
                var source = obj as UFUAModel.UFUAAlarmSource;
                if (parent is UFUAModel.UFUAArea)
                {
                    AddExistingObject(source, parent as UFUAModel.UFUAArea);
                }
            }
            else if (obj is UFUAModel.UFUAAlarmDefinition)
            {
                var def = obj as UFUAModel.UFUAAlarmDefinition;
                if (parent is UFUAModel.UFUAAlarmSource)
                {
                    AddExistingObject(def, parent as UFUAModel.UFUAAlarmSource);
                }
            }
            else if (obj is UFUAModel.UFUAAlarmThreshold)
            {
                var def = obj as UFUAModel.UFUAAlarmThreshold;
                if (parent is UFUAModel.UFUATag)
                {
                    AddExistingObject(def, parent as UFUAModel.UFUATag);
                }
            }
            else if (obj is DataLoggerModel.DataLoggerColumn)
            {
                var column = obj as DataLoggerModel.DataLoggerColumn;
                if (parent is DataLoggerModel.DataLoggerSettings)
                {
                    AddExistingObject(column, parent as DataLoggerModel.DataLoggerSettings);
                }
            }
        }

        void AddExistingObject(UFUAModel.UFUATag tag, UFUAModel.UFUAFolder root)
        {
            if (root != null)
                root.UFUATags.Add(tag);
        }

        void AddExistingObject(UFUAModel.UFUATag tag, UFUAModel.UFUATagPrototype root)
        {
            if (root != null)
                root.Members.Add(tag);
        }

        void AddExistingObject(UFUAModel.UFUAFolder folder, UFUAModel.UFUAFolder root)
        {
            if (root != null)
                root.UFUAFolders.Add(folder);
        }

        void AddExistingObject(UFUAModel.UFUAFolder folder, UFUAModel.UFUATagPrototype root)
        {
            if (root != null)
                root.Folders.Add(folder);
        }

        void AddExistingObject(UFUAModel.UFUAArea area, UFUAModel.UFUAArea root)
        {
            if (root != null)
                root.UFUAAreas.Add(area);
        }

        void AddExistingObject(UFUAModel.UFUAAlarmSource source, UFUAModel.UFUAArea root)
        {
            if (root != null)
                root.UFUAAlarmSources.Add(source);
        }

        void AddExistingObject(UFUAModel.UFUAAlarmDefinition definition, UFUAModel.UFUAAlarmSource root)
        {
            if (root != null)
                root.UFUAAlarmDefinitions.Add(definition);
        }

        void AddExistingObject(UFUAModel.UFUAAlarmThreshold threshold, UFUAModel.UFUATag root)
        {
            if (root != null)
            {
                threshold.UFUAAlarmDefinitionRef = FindAlarmDefinitionByNodeId(threshold.UFUAAlarmDefinitionNodeIdRef);
                root.UFUAAlarmThresholds.Add(threshold);
                root.NotifyPropertyChanged("UFUAAlarmThresholds");
            }
        }

        void AddExistingObject(DataLoggerModel.DataLoggerColumn column, DataLoggerModel.DataLoggerSettings root)
        {
            if (root != null)
                root.Columns.Add(column);
        }
#endif
        #endregion

        public List<UFUAModel.UFUATag> GetFlatListFromNodeID(List<string> nodeIdList)
        {
            var list = (from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel()
                        where tag.NodeId != null
                        select tag).ToList().AsParallel().Where(tag => nodeIdList.Contains(tag.NodeId.ToString()) /*&& !tag.IsPrototypeMember*/).ToList();
            return list;
        }

        public UFUAModel.UFUAConfiguration GetConfiguration(bool inExecution = false)
        {
            if (inExecution)
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).ToList();
                    if (list.Count == 0)
                        return new UFUAModel.UFUAConfiguration(task.UnitOfWork);
                    return list[0];
                }
            }
            else
            {
                var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(uow, true).AsParallel() select tag).ToList();
                if (list.Count == 0)
                    return new UFUAModel.UFUAConfiguration(uow);
                return list[0];
            }
        }

        internal String GetServiceName()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var serverName = UFUAServerInfo.UFUAServerInfo.GetServerName();
                var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).ToList();
                if (list.Count == 0)
                    return serverName;

                return String.Format("{0} ({1})", serverName, list[0].ApplicationName);
            }
        }

#if !NET_STANDARD
        internal XPObject CloneToClipboard(XPObject obj, bool checkattributes)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uow, uowClipboard, checkattributes, true, true);
            return cloneHelper.Clone(obj, false);
        }

        internal XPObject CloneFromClipboard(XPObject obj, bool checkattributes)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowClipboard, uow, checkattributes);
            return cloneHelper.Clone(obj, false);
        }

        NestedUnitOfWork lastNestedUnitOfWork;
        internal NestedUnitOfWork BeginNestedUnitOfWork()
        {
            lastNestedUnitOfWork = uow.BeginNestedUnitOfWork();
            lastNestedUnitOfWork.Disposed += (s, e) => { lastNestedUnitOfWork = null; };
            return lastNestedUnitOfWork;
        }

        internal XPObject GetNestedObject(object obj)
        {
            if (obj == null || !(obj is XPObject) || (obj as XPObject).IsLoading || (obj as XPObject).IsDeleted)
                return null;

            UowContext = BeginNestedUnitOfWork();
            return UowContext.GetNestedObject(obj as XPObject);
        }

        internal List<XPObject> GetNestedObjects(System.Collections.IList objects)
        {
            var list = (from c in objects.OfType<XPObject>() where !c.IsLoading && !c.IsDeleted select c).ToList();
            if (list.Count == 0)
                return null;

            UowContext = BeginNestedUnitOfWork();

            var ret = new List<XPObject>(list.Count);
            list.ForEach(obj =>
            {
                ret.Add(UowContext.GetNestedObject(obj));
            });

            return ret;
        }

        List<XPObject> GetParentObjects(System.Collections.IList objects)
        {
            var list = (from c in objects.OfType<XPObject>() where !c.IsLoading && !c.IsDeleted select c).ToList();
            if (list.Count == 0 || UowContext == null)
                return null;

            var ret = new List<XPObject>(list.Count);
            list.ForEach(obj =>
            {
                if (XpoHelpers.XpoHelper.IsSessionObject(obj, UowContext))
                    ret.Add(UowContext.GetParentObject(obj));
            });

            return ret;
        }
#endif

        static String GetConnectionString(String path
#if !NET_STANDARD
            , FileSystemProviderBase vfs
#endif
            )
        {
            String connString = null;
#if !NET_STANDARD
            if (vfs != null && vfs is DataSourceFileSystemProvider)
            {
                connString = (vfs as DataSourceFileSystemProvider).ConnectionString;
            }
            else
#endif
            {
                var xmlfile = String.Format("{0}/{1}/{2}{3}", path,
                                    Properties.Settings.Default.TypeLabel,
                                    Properties.Settings.Default.DefaultProjectName,
                                    Properties.Settings.Default.DefaultFileExt);

                connString = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", xmlfile));
            }

            return connString;
        }

        static String GetBaseFilename(String path
#if !NET_STANDARD
            , FileSystemProviderBase vfs
#endif
            )
        {
#if !NET_STANDARD
            if (vfs != null)
                return null;
#endif

            return String.Format("{0}/{1}/{2}{3}", path,
                                Properties.Settings.Default.TypeLabel,
                                Properties.Settings.Default.DefaultProjectName,
                                Properties.Settings.Default.DefaultFileExt);
        }

#if !NET_STANDARD
        public static void CopyFile(String fullPath, String newPath, bool bCopy,
            IDocument parent, UFInterfaces.IWorkspace work = null, IDocumentManager manager = null)
        {
            string ret = string.Empty;
            using (var sourceDoc = FromFile(fullPath, manager, parent, false, false))
            {
                if (sourceDoc == null)
                    return;

                var targetConn = newPath;
                if (!XpoHelpers.XpoHelper.IsDataSource(targetConn))
                    targetConn = GetConnectionString(newPath, null);
                using (var dlTarget = XpoDefault.GetDataLayer(targetConn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var uowTarget = new UnitOfWork(dlTarget))
                    {
                        var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceDoc.GetSession(), uowTarget, false, true, true);

                        ////////////////////////////////////////////////////////////////////////////
                        // delete all first
                        var cursor = new XPCursor(uowTarget, typeof(UFUAModel.UFUAFolder),
                            new GroupOperator(GroupOperatorType.And,
                                new NullOperator("UFUATagPrototype"),
                                new NullOperator("UFUAFolderAss")));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        cursor = new XPCursor(uowTarget, typeof(UFUAModel.UFUATag),
                            new GroupOperator(GroupOperatorType.And,
                                new NullOperator("UFUATagPrototype"),
                                new NullOperator("UFUAFolder")));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        cursor = new XPCursor(uowTarget, typeof(UFUAModel.UFUATagPrototype));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        cursor = new XPCursor(uowTarget, typeof(UFUAModel.UFUAArea),
                            new NullOperator("UFUAAreaAss"));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        cursor = new XPCursor(uowTarget, typeof(UFUAModel.UFUAEngineeringUnit));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        cursor = new XPCursor(uowTarget, typeof(UFUAModel.UFUAHistorianSettings));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        cursor = new XPCursor(uowTarget, typeof(DataLoggerModel.DataLoggerSettings));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        cursor = new XPCursor(uowTarget, typeof(UFUAModel.UFUAView));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        cursor = new XPCursor(uowTarget, typeof(UFUAModel.UFUAConfiguration));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        uowTarget.CommitChangesAndFreeMemory();
                        ////////////////////////////////////////////////////////////////////////////

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(UFUAModel.UFUAFolder),
                            new GroupOperator(GroupOperatorType.And,
                                new NullOperator("UFUATagPrototype"),
                                new NullOperator("UFUAFolderAss")));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(UFUAModel.UFUATag),
                            new GroupOperator(GroupOperatorType.And,
                                new NullOperator("UFUATagPrototype"),
                                new NullOperator("UFUAFolder")));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(UFUAModel.UFUATagPrototype));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(UFUAModel.UFUAArea),
                            new NullOperator("UFUAAreaAss"));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(UFUAModel.UFUAEngineeringUnit));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(UFUAModel.UFUAHistorianSettings));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(DataLoggerModel.DataLoggerSettings));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(UFUAModel.UFUAView));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        var cloneHelperConf = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceDoc.GetSession(), uowTarget, true, true, true);
                        cloneHelperConf.Clone(sourceDoc.GetConfiguration(), false);

                        uowTarget.CommitChangesAndFreeMemory();
                    }
                }

                var driverlist = sourceDoc.GetConfiguration().ComunicationDrivers;
                if (driverlist.Count > 0)
                {
                    foreach (var driver in driverlist)
                    {
                        var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(String.Format("{0}\\{1}", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), driver.AssemblyName));

                        try
                        {
                            var types = Assembly.LoadFile(uidll).GetTypes();
                            var list = (from t in types.AsParallel()
                                        where !t.IsAbstract && typeof(ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                                        select (ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).ToList();

                            list[0].CopyFile(sourceDoc.ConnectionString, targetConn);
                        }
                        catch (Exception e)
                        {
                            if (ret.Length > 0)
                            {
                                StringBuilder ss = new StringBuilder(ret);
                                ss.Append('\r');
                                ss.Append(string.Format(Properties.Resources.SaveAsDriverNotInstalled, driver.Name, uidll));
                            }
                            else
                                ret = string.Format(Properties.Resources.SaveAsDriverNotInstalled, driver.Name, uidll);
                        }
                    }
                }

                if (sourceDoc.removedVariablesManager != null)
                {
                    DataSourceFileSystemProvider fileSystemProvider = null;
                    try
                    {
                        var filePath = newPath;
                        if (XpoHelpers.XpoHelper.IsDataSource(newPath))
                        {
                            filePath = fullPath.Replace(parent.rootBase, parent.rootBaseDB);
                            fileSystemProvider = new DataSourceFileSystemProvider("")
                            {
                                ConnectionString = newPath
                            };
                        }

                        var targetRemovedVariables = new RemovedVariablesManager(filePath, fileSystemProvider, logGeneral);
                        targetRemovedVariables.Delete();
                        targetRemovedVariables.CopyFrom(sourceDoc.removedVariablesManager);
                        targetRemovedVariables.Save();
                    }
                    finally
                    {
                        if (fileSystemProvider != null)
                            fileSystemProvider.Dispose();
                    }
                }

            }

            if (ret.Length > 0 && work != null)
            {
                bool wasbusy = work.IsBusy;
                if (wasbusy)
                    work.IsBusy = false;
                MessageBox.Show(ret, Properties.Resources.SavaAsCaption, MessageBoxButton.OK);
                if (wasbusy)
                    work.IsBusy = true;
            }

            if (!bCopy)
                RemoveFile(fullPath, parent);
            return;
        }

        public static void RenameFile(String fullPath, String oldName, String newName,
            FileSystemProviderBase fileSystemProvider = null)
        {
        }

        public static void RemoveFile(string fullPath, IDocument parent, IDocumentManager manager = null)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            var file = GetBaseFilename(fullPath, fileSystemProvider);
            if (!String.IsNullOrEmpty(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                using (var sourceDoc = FromFile(fullPath, manager, parent, false, false))
                {
                    if (sourceDoc == null)
                        return;
                    var tags = sourceDoc.GetFlatTagCollection();
                    foreach (var tag in tags)
                    {
                        tag.Delete();
                    }
                    var prototypes = sourceDoc.GetPrototypes();
                    foreach (var prototype in prototypes)
                    {
                        prototype.Delete();
                    }
                    var alarms = sourceDoc.GetAlarmDefinitions();
                    foreach (var alarm in alarms)
                    {
                        alarm.Delete();
                    }
                    var areas = sourceDoc.GetAlarmAreas();
                    foreach (var area in areas)
                    {
                        area.Delete();
                    }
                    var units = sourceDoc.GetEngineeringUnits();
                    foreach (var unit in units)
                    {
                        unit.Delete();
                    }

                    sourceDoc.GetConfiguration().Delete();
                    sourceDoc.GetSession().CommitChanges();
                }
            }

            var removedVariables = new RemovedVariablesManager(fullPath, fileSystemProvider, logGeneral);
            removedVariables.Delete();
        }
#endif

        public static UFUAServerDocument FromFile(String path, IDocumentManager c, IDocument parent,
            bool bCreateNew = true, bool bCheckEmpty = true, bool bUseCacheUow = false)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    return null;

#if !NET_STANDARD
                FileSystemProviderBase vfs = parent.fileSystemProviderBase;
#endif
                String connString = GetConnectionString(path
#if !NET_STANDARD
                    , vfs
#endif
                    );
                String xmlfile = GetBaseFilename(path
#if !NET_STANDARD
                    , vfs
#endif
                    );

#if NET_STANDARD
                String fileCore = xmlfile + UFUAServerInfo.UFUAServerInfo.GetServerCoreExtension();
                try
                {
                    if (!File.Exists(fileCore))
                        File.Copy(xmlfile, fileCore);
                    else
                    {
                        FileInfo infoFileCore = new FileInfo(fileCore);
                        FileInfo infoXmlFile = new FileInfo(xmlfile);
                        if (infoXmlFile.LastWriteTime > infoFileCore.LastWriteTime)
                        {
                            File.Delete(fileCore);
                            File.Copy(xmlfile, fileCore);
                        } 
                    }

                    xmlfile = fileCore;
                }
                catch {}
#endif

                if (!bCreateNew && !String.IsNullOrEmpty(xmlfile) && !File.Exists(xmlfile))
                    return null;

                var serverDoc = new UFUAServerDocument()
                {
                    connectionString = connString,
#if !NET_STANDARD
                    EditorManagerComponent = c as UFUAEditorManagerComponent,
#endif
                    fileBase = xmlfile,
                    Parent = parent
                };

                serverDoc.CreateDataLayer(bUseCacheUow);
                if (!bCreateNew && bCheckEmpty && serverDoc.IsEmpty)
                {
                    serverDoc.Dispose();
                    return null;
                }
                else if (
#if !NET_STANDARD
                    vfs == null &&
#endif
                    !serverDoc.IsBelongFromParent(parent))
                {
                    serverDoc.Dispose();
#if !NET_STANDARD
                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiMsgBox != null)
                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, path));
#else
                    logGeneral.ErrorFormat(Properties.Resources.ErrorValidatingDocument, path);
#endif
                    return null;
                }

                var title = Path.GetFileNameWithoutExtension(path);
#if !NET_STANDARD
                if (vfs != null)
                    title = XpoHelpers.XpoHelper.GetDataSourceTitle(connString, true);
#endif
                serverDoc.EnsureDefaultSettings(title);

#if !NET_STANDARD
                serverDoc.NeedsSave = false;
                serverDoc.removedVariablesManager = new RemovedVariablesManager(path, vfs, logGeneral);
                serverDoc.ValidateRenamedVariables();
#endif
                return serverDoc;
            }
            catch
            {
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, path));
#else
                logGeneral.ErrorFormat(Properties.Resources.ErrorReadingDocument, path);
#endif
                return null;
            }
        }

#if !NET_STANDARD
        public bool CanClose()
        {
            if (ServerCMSHelperSync.IsServerStartedManually)
            {
                if (EditorManagerComponent.UIInterface != null)
                {
                    var res = EditorManagerComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.StopServerStartedManually,
                        UFUAServerInfo.UFUAServerInfo.GetServerName()), CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                        return false;
                    else if (res == CustomDialogResults.Yes)
                        ServerCMSHelperSync.StopServer();
                }
            }

            return true;
        }

        public string GetLocalizedStringID()
        {
            string got = string.Empty;
            if (EditorManagerComponent.StringEditor != null)
            {
                var se = EditorManagerComponent.StringEditor.GetStringEditor(this);
                GeneralDialogContent Dialog = new GeneralDialogContent(se)
                {
                    DialogKeepContent = true,
                    HelpLink = "StringEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    got = se.DataContext as string;

                }
            }
            return got;
        }

        internal void RemoveTagList(List<string> resolvedNodeIds)
        {
            List<UFUAModel.UFUATag> tagList = GetFlatListFromNodeID(resolvedNodeIds);
            OnChangedDocument(this, ChangedType.removed, tagList.ToList<object>());
            tagList.ForEach(tag =>
            {
                tag.Delete();
            });
        }
#endif

        public bool SaveToFile(bool discargechanges = false, bool bForceSave = false, bool forceEncryption = false)
        {
            if (uow == null)
                return false;

#if !NET_STANDARD
            List<XPObject> parentObjects = null;
            if (ActiveView != null && EditorManagerComponent.Workspace != null &&
                EditorManagerComponent.Workspace.ActiveWindow == ActiveView)
            {
                EditorManagerComponent.Workspace.UpdateContextNow();
                var objects = EditorManagerComponent.Workspace.ContextObjects;
                if (EditorManagerComponent.Workspace.ContextObject != null)
                {
                    if (objects == null)
                        objects = new List<object>();
                    objects.Add(EditorManagerComponent.Workspace.ContextObject);
                }

                if (objects != null)
                    parentObjects = GetParentObjects(objects);
            }
#endif

            try
            {
#if !NET_STANDARD
                if (discargechanges)
                {
                    if (uow.TryPurgeDeletedObjects(logGeneral) > 0)
                        bForceSave = true;

                    var bCommitChanges = CleanSubPrototypeMembers(uow);
                    bCommitChanges |= CleanInvalidAlarmThreshold(uow);
                    if (bCommitChanges)
                    {
                        bForceSave = true;
                        uow.CommitChanges();
                        uow.TryPurgeDeletedObjects();
                    }
                }

                if (!bForceSave && !NeedsSave)
                    return false;

                if (bInvalidateDriversDynamicSettingsOnSave)
                    InvalidateAllDriversDynamicSettings();
#endif

                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (forceEncryption || Protected)
                        XpoHelpers.XpoHelper.AddProtectionCode(uow, Id);
                    else
                        XpoHelpers.XpoHelper.RemoveProtectionCode(uow);
                }

                uow.CommitChanges();

                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (File.Exists(fileBase))
                    {
                        try
                        {
                            File.Delete(fileBase);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

#if !NET_STANDARD
                    {
                        var dList = GetDrivers();
                        foreach (var dr in dList)
                        {
                            var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(String.Format("{0}\\{1}", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), dr.AssemblyName));

                            DriverSettingsInterfaces.ICommunicationDriverWpfEditing driverWpfEditing = null;
                            try
                            {
                                var types = Assembly.LoadFile(uidll).GetTypes();
                                var list = (from t in types/*.AsParallel()*/
                                            where !t.IsAbstract && typeof(DriverSettingsInterfaces.ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                                            select (DriverSettingsInterfaces.ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).ToList();
                                driverWpfEditing = list[0];

                                UserControl control = null;
                                try
                                {
                                    control = driverWpfEditing?.GeneralSettingsEditor;
                                }
                                catch
                                { }

                                if (control != null)
                                {
                                    var settingsContext = new ComunicationSettingsContext2()
                                    {
                                        ConnectionString = ConnectionString,
                                        bProtected = false,
                                        protectionCode = Id,
                                        listTag = GetFlatFullTagNameNodeIdCollection()
                                    };
                                    control.DataContext = settingsContext;

                                    settingsContext.bProtected = forceEncryption || Protected;
                                    driverWpfEditing.SaveSettings(control);

                                    if (control is IDisposable)
                                        (control as IDisposable).Dispose();
                                }
                            }
                            catch (DriverBaseInterfaces.ValidatingDocumentException ex)
                            {
                                if (EditorManagerComponent.UIInterface != null)
                                    EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, ex.FilePath));
                            }
                            catch (Exception ex)
                            {
                                if (EditorManagerComponent.UIInterface != null)
                                    EditorManagerComponent.UIInterface.ShowInformation(String.Format(Properties.Resources.CommDriverNotFound, uidll));
                            }
                        }
                    }
#endif

                    if (forceEncryption || Protected)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var writer = XmlWriter.Create(memoryStream);
                            InMemory.WriteXml(writer);
                            writer.Flush();
                            writer.Close();
                            var str = Convert.ToBase64String(memoryStream.ToArray());
                            var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                            File.WriteAllText(fileBase, toWrite);
                        }
                    }
                    else
                        InMemory.WriteXml(fileBase);
                }

#if !NET_STANDARD
                if (!discargechanges && removedVariablesManager != null)
                    removedVariablesManager.Save();
#endif

            }
            catch (Exception ex)
            {
#if !NET_STANDARD
                MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
#endif
                return false;
            }

#if !NET_STANDARD
            EditorManagerComponent.RefreshHiddenDocuments(Parent);

            if (parentObjects != null && parentObjects.Count > 0)
            {
                if (parentObjects.Count == 1)
                    EditorManagerComponent.Workspace.ContextObject = GetNestedObject(parentObjects[0]);
                else
                    EditorManagerComponent.Workspace.ContextObject = GetNestedObjects(parentObjects);
            }
#endif
            return true;
        }

#if !NET_STANDARD
        public void CertificateChecker()
        {
            // /SUFSolution /EE:\PRIVATE\12-0-Drivers\UFSolution\bin\Debug\UFSolution.Config.xml /C client
            // /SUFUAServer /EE:\PRIVATE\12-0-Drivers\UFSolution\bin\Debug\UFUAServer.UAServer.Config.xml
            string arguments = string.Format(Properties.Settings.Default.CertificateCheckerArgs/*""/SPlatform.NExT IOServer" "/E{0}" /I"*/,
                UFUAServerInfo.UFUAServerInfo.GetServerConfigFile(), GetAplicationName(), ApplicationPropertiesHelper.GetProperty("CurrentSkin"));
            string path = Properties.Settings.Default.CertificateChecker/*"CertificateChecker.exe"*/;
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);
            Process.Start(path, arguments);
        }

        public void ServiceManager()
        {
            if (editorManagerComponent.UserEditor != null)
                editorManagerComponent.UserEditor.EnsureCredentialProvider(this);
            AddHttpAccessRules(true);

            string serverConn = String.Empty;
            string stringConn = String.Empty;
            string userConn = String.Empty;
            if (fileBase != null)
            {
                serverConn = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", fileBase));
                if (editorManagerComponent.StringEditor != null)
                    stringConn = editorManagerComponent.StringEditor.GetConnectionStringFromFile(rootBase);
                if (editorManagerComponent.UserEditor != null)
                    userConn = editorManagerComponent.UserEditor.GetConnectionStringFromFile(rootBase);
            }
            else
                serverConn = userConn = stringConn = ConnectionString;

            var docPath = GetSpecialFolder(SpecialFolders.Documents).GetPathString();
            docPath = docPath.Trim('\\', '/');

            var applicationName = GetAplicationName();
            if (NeedToRunAsCFR21UserIndentity())
            {
                var username = UFUAServerInfo.UFUAServerInfo.GetCFR21UserName();
                var domainName = UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName();
                if (!String.IsNullOrEmpty(domainName))
                    username = String.Format("{0}\\{1}", domainName, username);
                var password = "ie4HZN8u9uW4NJOdnRNFbMcs9wfWnAaGICcU3qs6fcW8IpbPVp273NEpPMaAlV8B"/* "{45F928C5_1EF2_48D1_9505_14ACf7AD6AF8}" */;
                UFUAServerCMS.UFUAServerCSMHelpers.OpenServiceManagerWithLogInInformation(applicationName, username, password, serverConn, stringConn, userConn, docPath);
            }
            else
                UFUAServerCMS.UFUAServerCSMHelpers.OpenServiceManager(applicationName, serverConn, stringConn, userConn, docPath);
        }
#endif

        #region Script Remote Debugging
#if !NET_STANDARD
        internal bool EnableScriptDebugging(Guid id, bool bEnable)
        {
            if (!ServerCMSHelperAsync.IsServerRunning)
                return false;

            return ServerCMSHelperAsync.EnableScriptDebugging(id, bEnable);
        }

        internal List<string> ScriptSynchronizing(Guid id, List<string> data)
        {
            if (!ServerCMSHelperAsync.IsServerRunning)
                return null;

            return ServerCMSHelperAsync.ScriptSynchronizing(id, data);
        }
#endif
        #endregion

#if !NET_STANDARD
        public bool StartServer(bool bSave = true, bool manually = false)
        {
            return StartServer(null, null, bSave, manually);
        }
        public bool StartServer(TextBlock textBlock, ScrollViewer scroll, bool bSave = true, bool manually = false)
        {
            if (bSave && NeedsSave)
            {
                if (EditorManagerComponent.UIInterface != null)
                {
                    var res = EditorManagerComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                        UFUAServerInfo.UFUAServerInfo.GetServerName()), CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                        return false;
                }

                SaveToFile();
            }

            if (!AddHttpAccessRules(false))
                return false;

            string serverConn = String.Empty;
            string stringConn = String.Empty;
            string userConn = String.Empty;
            if (fileBase != null)
            {
                serverConn = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", fileBase));
                if (editorManagerComponent.StringEditor != null)
                    stringConn = editorManagerComponent.StringEditor.GetConnectionStringFromFile(rootBase);
                if (editorManagerComponent.UserEditor != null)
                    userConn = editorManagerComponent.UserEditor.GetConnectionStringFromFile(rootBase);
            }
            else
                serverConn = userConn = stringConn = ConnectionString;

            var docPath = GetSpecialFolder(SpecialFolders.Documents).GetPathString();
            docPath = docPath.Trim('\\', '/');

            bool suspended = false;
#if !DEBUG
            if(MSZ.MSZView.IsSuspended())
                suspended = true;
#endif
            var needToRunAsCFR21UserIndentity = NeedToRunAsCFR21UserIndentity();
            if (needToRunAsCFR21UserIndentity && FilePath != null)
            {
                try
                {
                    var rootPath = FilePath;
                    if (Parent != null)
                        rootPath = Parent.FilePath;

                    DirectoryInfo dInfo = new DirectoryInfo(Path.GetDirectoryName(rootPath));
                    var accessRule = new System.Security.AccessControl.FileSystemAccessRule(
                        UFUAServerInfo.UFUAServerInfo.GetCFR21UserNameSetting(),
                        System.Security.AccessControl.FileSystemRights.Read |
                        System.Security.AccessControl.FileSystemRights.Write |
                        System.Security.AccessControl.FileSystemRights.Delete |
                        System.Security.AccessControl.FileSystemRights.Modify,
                        System.Security.AccessControl.InheritanceFlags.ContainerInherit |
                        System.Security.AccessControl.InheritanceFlags.ObjectInherit,
                        System.Security.AccessControl.PropagationFlags.InheritOnly,
                        System.Security.AccessControl.AccessControlType.Allow);

                    System.Security.AccessControl.DirectorySecurity dSecurity = dInfo.GetAccessControl();
                    dSecurity.RemoveAccessRuleAll(accessRule);
                    dSecurity.AddAccessRule(accessRule);
                    dInfo.SetAccessControl(dSecurity);
                }
                catch { }
            }

            var serverCMSHelper = manually ? ServerCMSHelperAsync : ServerCMSHelperSync;
            return serverCMSHelper.StartServer(textBlock, scroll, suspended, manually, needToRunAsCFR21UserIndentity, serverConn, stringConn, userConn, docPath);
        }
#endif

        public OPCUAEntityReference GetServerOPCUAEntityReference()
        {
            var applicationName = GetAplicationName();
            return new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                            null, ObjectIds.Server, ObjectIds.Server.ToString(), null, null);
        }
        #endregion

        #region Properties
#if !NET_STANDARD
        [Browsable(false)]
        bool needsSave;
        public bool NeedsSave
        {
            get
            {
                return needsSave;
            }
            set
            {
                if (needsSave == value)
                    return;

                needsSave = value;
                if (!needsSave)
                {
                    lock (tagsListLock)
                    {
                        listFlatFullTagNameCollection = null;
                        listFlatFullTagNameCollectionAdded = null;
                        listFlatFullTagNameCollectionRemoved = null;
                        listFlatFullHistoricalTagNameCollection = null;
                        listFlatFullHistoricalTagNameCollectionAdded = null;
                        listFlatFullHistoricalTagNameCollectionRemoved = null;
                        //Folder
                        listFlatFullFolderNameCollection = null;
                        listFlatFullFolderNameCollectionAdded = null;
                        listFlatFullFolderNameCollectionRemoved = null;
                    }
                }
                OnPropertyChanged("NeedsSave");
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public uint TotalDynamicTags
        {
            get
            {
                bool bArrayOneSize = false;
#if !DEBUG
                bArrayOneSize = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx3ItKbxy9EMJoUOWvtWCzGQ=="/* AR1 */);
#endif
                return UFUAModel.Helpers.DynamicTagsCounter.GetDynamicTagsCounter(uow, bArrayOneSize);
            }
        }
#endif

        [Browsable(false)]
        public bool IsDisposed
        {
            get { return bObjectDisposed; }
        }

#if !NET_STANDARD
        NestedUnitOfWork uowContext;
        [Browsable(false)]
        internal NestedUnitOfWork UowContext
        {
            get
            {
                return uowContext;
            }
            set
            {
                if (uowContext == value)
                    return;

                uowContext = value;
                OnPropertyChanged("UowContext");
            }
        }

        UFUAEditorManagerComponent editorManagerComponent;
        [Browsable(false)]
        public UFUAEditorManagerComponent EditorManagerComponent
        {
            get
            {
                return editorManagerComponent;
            }
            private set
            {
                editorManagerComponent = value;
            }
        }
#endif

        [Browsable(false)]
        public String ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        Guid configurationId;
        [Browsable(false)]
        Guid ConfigurationId
        {
            get
            {
                if (configurationId == Guid.Empty)
                {
                    using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                    {
                        var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).ToList();
                        if (list.Count == 0)
                            return Guid.Empty;

                        configurationId = list[0].ConfigurationId;
                    }
                }

                return configurationId;
            }
            set
            {
                if (configurationId == value)
                    return;

                var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(uow, true).AsParallel() select tag).ToList();
                if (list.Count > 0)
                {
                    list[0].ConfigurationId = value;
                    configurationId = value;
                }
            }
        }

#if !NET_STANDARD
        UFUAServerCSMHelpers serverCMSHelperSync;
        [Browsable(false)]
        internal UFUAServerCSMHelpers ServerCMSHelperSync
        {
            get
            {
                if (serverCMSHelperSync != null &&
                    !serverCMSHelperSync.IsServerStartedManually &&
                    serverCMSHelperSync.InstanceId != ConfigurationId.ToString())
                {
                    serverCMSHelperSync.Dispose();
                    serverCMSHelperSync = null;
                }

                if (serverCMSHelperSync == null)
                    serverCMSHelperSync = new UFUAServerCSMHelpers(ConfigurationId.ToString(), logGeneral);

                return serverCMSHelperSync;
            }
        }

        UFUAServerCSMHelpers serverCMSHelperAsync;
        [Browsable(false)]
        internal UFUAServerCSMHelpers ServerCMSHelperAsync
        {
            get
            {
                if (serverCMSHelperAsync != null &&
                    !serverCMSHelperAsync.IsServerStartedManually &&
                    serverCMSHelperAsync.InstanceId != ConfigurationId.ToString())
                {
                    serverCMSHelperAsync.Dispose();
                    serverCMSHelperAsync = null;
                }

                if (serverCMSHelperAsync == null)
                {
                    serverCMSHelperAsync = new UFUAServerCSMHelpers(ConfigurationId.ToString(), logGeneral);
                    serverCMSHelperAsync.StartServerStatusInBackground();
                }

                return serverCMSHelperAsync;
            }
        }
#endif
        #endregion

        #region ICloneable Members

        UFUAServerDocument(UFUAServerDocument template)
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }

#if !NET_STANDARD
        public object Clone()
        {
            return new UFUAServerDocument(this);
        }
#endif
        #endregion

        #region IDocument

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            var temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

#if !NET_STANDARD
        UserControl activeView;
        [Browsable(false)]
        public UserControl ActiveView
        {
            get
            {
                return activeView;
            }
            set
            {
                activeView = value;
            }
        }

        [Browsable(false)]
        public UserControl View
        {
            get
            {
                return activeView;
            }
        }
#endif

        IDocument parent;
        [Browsable(false)]
        public IDocument Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
                if (parent != null && uow != null && GetConfiguration().ParentApplicationName != parent.Title)
                {
                    GetConfiguration().ParentApplicationName = parent.Title;
                }
            }
        }

        [Browsable(false)]
        public IList<IDocument> Childs
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String ProjectType
        {
            get
            {
                if (Parent != null)
                    return Parent.ProjectType;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public String Theme
        {
            get
            {
                if (Parent != null)
                    return Parent.Theme;
                return String.Empty;
            }
        }

        public Uri GetSpecialFolder(SpecialFolders specialFolder)
        {
            if (Parent != null)
                return Parent.GetSpecialFolder(specialFolder);
            return null;
        }

        public Uri MakeAbosoluteUri(Uri relative)
        {
            if (relative == null)
                return null;

            if (Parent != null)
                return Parent.MakeAbosoluteUri(relative);
            if (relative.IsAbsoluteUri)
                return relative;

            // return new Uri(Path.GetDirectoryName(FullPath) + "\\").MakeRelativeUri(absolute);
            return null;
        }

        public IDocument UpdateParentFromUri(Uri relative)
        {
            if (Parent != null)
                return Parent.UpdateParentFromUri(relative);
            return this;
        }

        public Uri MakeRelativeUri(Uri absolute)
        {
            if (absolute == null)
                return null;

            if (Parent != null)
                return Parent.MakeRelativeUri(absolute);
            if (!absolute.IsAbsoluteUri)
                return absolute;

            // return absolute.MakeRelativeUri(new Uri(Path.GetDirectoryName(FullPath) + "\\"));
            return null;
        }

#if !NET_STANDARD
        [Browsable(false)]
        public FileSystemProviderBase fileSystemProviderBase
        {
            get
            {
                if (Parent != null)
                    return Parent.fileSystemProviderBase;
                return new PhysicalFileSystemProvider("");
            }
        }
#endif

        [Browsable(false)]
        public String rootBase
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBase;
                return Path.GetDirectoryName(fileBase);
            }
        }

        [Browsable(false)]
        public String rootBaseDB
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBaseDB;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public bool Protected
        {
            get
            {
                if (Parent != null)
                    return Parent.Protected;
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                return Guid.Empty;
            }
        }

        public String Title
        {
            get
            {
                return Path.GetFileNameWithoutExtension(fileBase);
            }
        }

        [Browsable(false)]
        public String FilePath
        {
            get
            {
                return fileBase;
            }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return GetFlatTagCollection().Count == 0 && GetConfiguration().ComunicationDrivers.Count == 0;
            }
        }

        [Browsable(false)]
        public bool IsRoot
        {
            get
            {
                return false;
            }
        }

        public Object GetService(Type type)
        {
            if (Parent != null)
                return Parent.GetService(type);
            return null;
        }

        #endregion

        #region IDisposable

        protected bool bObjectDisposed;
        protected override void OnDispose()
        {
            if (bObjectDisposed)
                return;
            bObjectDisposed = true;

            OnDisposing(this);

            base.OnDispose();

#if !NET_STANDARD
            // EditorManagerComponent.Workspace.ContextObject = null;

            if (uowContext != null)
            {
                uowContext.Dispose();
                uowContext = null;
            }
#endif

            if (uow != null)
            {
                uow.Disconnect();
                uow.Dispose();
                uow = null;
            }
#if !NET_STANDARD
            //if (uowCloner != null)
            //{
            //    uowCloner.Disconnect();
            //    uowCloner.Dispose();
            //    uowCloner = null;
            //}
            if (uowClipboard != null)
            {
                uowClipboard.Disconnect();
                uowClipboard.Dispose();
                uowClipboard = null;
            }
#endif

            if (dl != null)
            {
                dl.Dispose();
                dl = null;
            }

#if !NET_STANDARD
            if (dlClipboard != null)
            {
                dlClipboard.Dispose();
                dlClipboard = null;
            }
#endif

            if (cachedUnitOfWorks != null)
                cachedUnitOfWorks.Dispose();

            lock (lockObject)
            {
                cachedDefaultLocalEndpoint.Clear();
                cachedDefaultAppName.Clear();
                cachedMapTags.Clear();
                if (notFoundTagNames != null)
                    notFoundTagNames.Clear();
            }

#if !NET_STANDARD
            if (undoRedoHelper != null)
            {
                undoRedoHelper.PurgeUndoActions();
                undoRedoHelper.PurgeRedoActions();
                undoRedoHelper = null;
            }

            if (uowUndoRedo != null)
            {
                uowUndoRedo.Disconnect();
                uowUndoRedo.Dispose();
                uowUndoRedo = null;
            }

            if (dlUndoRedo != null)
            {
                dlUndoRedo.Dispose();
                dlUndoRedo = null;
            }

            if (serverCMSHelperAsync != null)
            {
                serverCMSHelperAsync.Dispose();
            }

            if (serverCMSHelperSync != null)
            {
                serverCMSHelperSync.Dispose();
            }

            lock (tagsListLock)
            {
                listFlatFullTagNameCollection = null;
                listFlatFullTagNameCollectionAdded = null;
                listFlatFullTagNameCollectionRemoved = null;
                listFlatFullHistoricalTagNameCollection = null;
                listFlatFullHistoricalTagNameCollectionAdded = null;
                listFlatFullHistoricalTagNameCollectionRemoved = null;
                //Folder
                listFlatFullFolderNameCollection = null;
                listFlatFullFolderNameCollectionAdded = null;
                listFlatFullFolderNameCollectionRemoved = null;
            }
#endif
        }

        #endregion
    }
}
