using System;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.UI.Interactivity;
using System.Runtime.Serialization;
using System.IO;
using DocumentManager.ComponentService;
using VFS;
using UIMsgBoxAlertService.ComponentService;
using System.Xml;
using System.Linq;
using log4net;
using StringManager.ComponentService;
using UFProjectManager.ComponentService;

namespace GridLayout
{
    public class GridLayoutHelper : Object
    {
        public static string DesignSettingName { get { return Properties.Settings.Default.DesignSettingName; } }
        public static string DesignSettingBaseName { get { return Properties.Settings.Default.DesignSettingBaseName; } }
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.GridLayoutHelper);
        public static MemorySettings LoadMemoryMap(IDocument Document, string controlName, string username)
        {
            return StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, controlName, username);
        }
        public static Setting InitDesign(Setting setting, IDocument Document, string controlName, out MemorySettings memories, string username, bool bKeepOptionValue = false)
        {
            string defaultTagSetting = GridLayoutHelper.DesignSettingName;
            setting.Name = defaultTagSetting;
            setting.ReadOnly = true;

            memories = LoadMemoryMap(Document, controlName, username);
            if (memories != null)
            {
                var defaultsetting = (from m in memories where m.Name.Equals(defaultTagSetting) select m).FirstOrDefault();
                if (defaultsetting != null)
                {
                    memories.Remove(defaultsetting);
                    if (bKeepOptionValue)
                        setting.Option1 = defaultsetting.Option1;
                }
                memories.Insert(0, setting);
            }
            else
            {
                memories = new MemorySettings();
                memories.Add(setting);
            }

            return setting;
        }
        public static string InitDesign(string GridLayout, IDocument Document, string controlName, out MemorySettings memories, string username)
        {
            string designGridLayout = GridLayout;
            string defaultTagSetting = GridLayoutHelper.DesignSettingName;
            try
            {
                memories = LoadMemoryMap(Document, controlName, username);
                var defaultsetting = (from m in memories where m.Name.Equals(defaultTagSetting) select m).FirstOrDefault();
                if (defaultsetting == null)
                    memories.Add(new Setting() { Name = defaultTagSetting, GridLayout = designGridLayout, ReadOnly = true });
                else
                {
                    defaultsetting.ReadOnly = true;
                    defaultsetting.GridLayout = designGridLayout;
                }
            }
            catch (Exception ex)
            {
                memories = new MemorySettings();
                memories.Add(new Setting() { Name = defaultTagSetting, GridLayout = designGridLayout, ReadOnly = true });
                IUFProjectManager iUFProjectManager = Document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                log.Error(Properties.Resources.InitMemoryError, ex);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.GridLayoutHelper,
                      DateTime.UtcNow, $"{Properties.Resources.InitMemoryError}: {ex.Message}",
                      System.Diagnostics.EventLogEntryType.Error);
            }

            return designGridLayout;
        }
        //public static void SaveMemoryMap(MemorySettings memories, IDocument Document, string controlName)
        //{
        //    StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(memories,Document, controlName);
        //}
    }
}
