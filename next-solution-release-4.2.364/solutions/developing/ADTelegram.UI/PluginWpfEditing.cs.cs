using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADPluginSettingsInterface;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;

namespace ADTelegram.UI
{
    public class PluginWpfEditing : IPluginWpfEditing
    {
        GeneralSettingsEditor generalSettingsEditor;
        PluginTestEditor pluginTestEditor;

        #region IPluginWpfEditing Members

        public System.Windows.Controls.UserControl GeneralSettingsEditor
        {
            get
            {
                if (generalSettingsEditor == null)
                    generalSettingsEditor = new GeneralSettingsEditor();
                return generalSettingsEditor;
            }
        }

        public bool SaveSettings(System.Windows.Controls.UserControl control)
        {
            ((GeneralSettingsEditor)control).SavePluginSettings();

            return true;
        }

        public bool CopyFile(string sourceconn, string targetconn)
        {
            string pluginname = ADPluginBase.Helpers.PluginInfo.GetPluginName(this).Replace(".UI", "");

            var sourceDL = ADPluginBase.PluginBase.GetPluginDataLayer(sourceconn, pluginname);
            string toConn = ADPluginBase.PluginBase.GetConnectionString(targetconn, null, pluginname);
            var targetDL = XpoDefault.GetDataLayer(toConn, AutoCreateOption.DatabaseAndSchema);

            if (sourceDL == null || targetDL == null)
                return false;

            using (UnitOfWork sourceufw = new UnitOfWork(sourceDL))
            {
                using (UnitOfWork targetufw = new UnitOfWork(targetDL))
                {
                    var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceufw, targetufw, false);

                    ////////////////////////////////////////////////////////////////////////////
                    // delete all first
                    (from p in new XPQuery<PluginSettings>(targetufw, true).AsParallel()
                     select p).ToList().ForEach(tag => tag.Delete());
                    ////////////////////////////////////////////////////////////////////////////

                    var configuration = (from tag in new XPQuery<PluginSettings>(sourceufw).AsParallel() select tag).ToList();
                    if (configuration.Count > 0)
                        cloneHelper.Clone(configuration[0], false);
                    targetufw.CommitChanges();
                }
            }

            return true;
        }

        public System.Windows.Controls.UserControl PluginTestEditor
        {
            get
            {
                if (pluginTestEditor == null)
                    pluginTestEditor = new PluginTestEditor();
                return pluginTestEditor;
            }
        }

        #endregion
    }
}
