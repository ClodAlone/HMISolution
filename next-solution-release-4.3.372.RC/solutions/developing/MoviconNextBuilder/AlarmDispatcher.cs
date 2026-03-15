using ADEditor;
using ADEditor.ComponentService;
using ADEditor.Document;
using ADModel;
using ADServerInfo;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UFUAModel;
using Utilities;

namespace MoviconNextBuilder
{
    public enum ADReturnCode : int
    {
        Exist = 1,
        Ok = 0,
        NoDocument = -1,
        ObjectUsed = -2,
        NoObject = -3
    }

    public class AlarmDispatcher : IDisposable
    {
        #region Ctor
        public AlarmDispatcher(IDocument parent, ADEditorManagerComponent editormanager)
        {
            adComponent = editormanager;
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            document = ADEditorDocument.FromFile(uri.GetPathString(), adComponent, parent);
        }
        #endregion Ctor

        #region data
        ADEditorDocument document;

        
        #endregion data

        #region Properties
        ADEditorManagerComponent adComponent;
        public ADEditorManagerComponent ADComponent
        {
            get { return adComponent; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds an available plugin to the project configuration
        /// </summary>
        /// <param name="name">name of the plugin</param>
        /// <param name="assemblyname">assembly name of the plugin (i.e. ADSmtp.dll)</param>
        /// <param name="description">Description of the plugin (optional)</param>
        /// <returns>0 for successful insert, 1 if the plugin is already present, -1 if the project is not opened</returns>
        public ADReturnCode AddPlugin(string name, string assemblyname, String description=null)
        {
            if (document == null)
                return ADReturnCode.NoDocument;
             
            var list = (from plug in document.GetConfiguration().ADPlugins where plug.Name == name && plug.AssemblyName == assemblyname select plug).ToList();
            if (list.Count == 0)
            {
                var plugin = document.AddNewPlugin();
                plugin.NodeId = Guid.NewGuid();
                plugin.Name = name;
                plugin.AssemblyName = assemblyname;
                if(description != null)
                    plugin.Description = description;

                document.GetConfiguration().ADPlugins.Add(plugin);//restore row, commentata per case 9139
                return ADReturnCode.Ok;
            }
            else
                return ADReturnCode.Exist;
        }

        /// <summary>
        /// Remove a plugin from the project configuration
        /// </summary>
        /// <param name="name">name of the pluign (as set when added to the project configuration)</param>
        /// <returns>0 upon removal, -2 if the plugin is in use by notifications, -3 if the plugin do not exists, in project configuration, -1 if the project is not opened</returns>
        public ADReturnCode RemovePlugin(string name)
        {
            if(document == null)
                return ADReturnCode.NoDocument;
            if (document.IsPluginNameUsed(name))
            {
                return ADReturnCode.ObjectUsed;
            }
            var list = (from plug in document.GetConfiguration().ADPlugins where plug.Name == name select plug).ToList();
            if(list.Count > 0)
            {
                list[0].Delete();
                return ADReturnCode.Ok;
            }
            return ADReturnCode.NoObject;
            
        }

        /// <summary>
        /// retuns list of available plugin, for the current installation
        /// </summary>
        /// <returns>
        /// a list of pluginDesc object, containing Name, AssemblyName and Description for the available plugins.
        /// </returns>
        public List<pluginDesc> GetAvailablePlugins()
        {
            try
            {
                var doc = XElement.Load(ADServerInfo.ADServerInfo.GetPluginListFile());
                return (from item in doc.Descendants("Plugin")
                        select new pluginDesc
                        {
                            Name = item.Attribute("FriendlyName").Value,
                            Description = item.Attribute("Description").Value,
                            AssemblyName = item.Attribute("AssemblyName").Value
                        }).ToList();
            }
            catch(Exception e)
            { }
            return null;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"> name of the Notification</param>
        /// <param name="type"> type of the notification [NotificationTypes.Local, NotificationTypes.Server]</param>
        /// <param name="pluginname">name of the plugin</param>
        /// <param name="usr">recipient, sholud be a UFUser or a UFRole object</param>
        /// <param name="tag">OPCUAEntityReference tag, only for Local Notification (optional)</param>
        /// <param name="area">UFUAArea to reference, only for Server Notification (optional)</param>
        /// <param name="folder">ADFolder object into add the Notification (optional)</param>
        /// <returns>the added ADNotification object.
        /// If the type is NotificationTypes.Server area has to be valued, or the method returns null.
        /// The method returns null if the project is not opened
        /// </returns>
        public ADNotification AddNotification(string name, NotificationTypes type, String pluginname, object usr, OPCUAEntityReference tag = null, UFUAArea area = null, ADFolder folder = null)
        {
            if (document != null && pluginname != null && usr != null)
            {
                if (!(usr is UFUserModel.UFUser) && !(usr is UFUserModel.UFRole))
                    return null;

                var notification = document.AddNewNotification(folder);
                notification.Name = name;

                notification.NotificationType = type;
                if (type == NotificationTypes.Server && area != null)
                    notification.AlarmName = ADEditorDocument.GetCompleteAreaName(area);
                if (tag != null)
                {
                    notification.NotificationItem = tag.ToXml();
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    string oldChars = string.Format("{0}:", ns);
                    if (!string.IsNullOrEmpty(notification.AlarmName))
                        notification.NotificationItemName = string.Format("{0} ({1})", (tag.EndpointUrl).Replace(oldChars, ""), tag.AppName)/*HumanReadable*/;
                    else
                        notification.NotificationItemName = string.Format("{0} ({1})", (tag.ReadablePath).Replace(oldChars, ""), tag.AppName)/*HumanReadable*/;

                }
                
                if(pluginname != null)
                {
                    var list = (from plug in document.GetConfiguration().ADPlugins where plug.Name == pluginname select plug).ToList();
                    if(list.Count > 0)
                    {
                        notification.PluginID = list[0].NodeId;
                        notification.PluginName = list[0].Name;
                    }
                }
                notification.MultiRecipientID += "g=" + (usr is UFUserModel.UFUser ? (usr as UFUserModel.UFUser).NodeId : (usr as UFUserModel.UFRole).NodeId) + ";";
                notification.Recipient = (usr is UFUserModel.UFUser ? (usr as UFUserModel.UFUser).Name : (usr as UFUserModel.UFRole).Name); 
                return notification;
            }
            return null;
        }

        /// <summary>
        /// returns an existing Notification
        /// </summary>
        /// <param name="name">Notification name</param>
        /// <param name="folder">folder containing the notification</param>
        /// <returns>returns the Notification, if exists, otherwise null</returns>
        public ADNotification GetNotification(string name, ADFolder folder = null)
        {
            if (document != null)
                return document.GetNotification(name, folder);
            return null;
        }

        /// <summary>
        /// Delete a Notification
        /// </summary>
        /// <param name="name">Notification name</param>
        /// <param name="folder">folder containing the notification</param>
        /// <returns>True upon deletion, false otherwise</returns>
        public bool DeleteNotification(string name, ADFolder folder = null)
        {
            if(document != null)
            {
                var notification = document.GetNotification(name, folder);
                if(notification != null)
                {
                    notification.Delete();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Add a new folder for Notifications
        /// </summary>
        /// <param name="name">Name of the folder to add</param>
        /// <param name="folder">existing folder into which create the new one (optional)</param>
        /// <returns>The new folder object, null if the project is not opened</returns>
        public ADFolder AddFolder(string name, ADFolder folder = null)
        {
            if(document != null)
            {
                var newfolder = document.GetFolder(name, folder);
                if (newfolder != null)
                    return newfolder;
                newfolder = document.AddNewFolder(folder);
                newfolder.Name = name;
                return newfolder;
            }
            return null;
        }

        /// <summary>
        /// gets an existing folder
        /// </summary>
        /// <param name="name">name of the folder</param>
        /// <param name="folder">folder containing the searched one (optional)</param>
        /// <returns>The folder object, null if the project is not opened</returns>
        public ADFolder GetFolder(string name, ADFolder folder = null)
        {
            if(document != null)
                return document.GetFolder(name, folder);
            return null;
        }
        
        /// <summary>
        /// delete a folder
        /// </summary>
        /// <param name="name">name of the folder to delete</param>
        /// <param name="folder">folder containing the folder to delete (optional)</param>
        /// <returns>true upon deletion, false otherwise</returns>
        public bool DeleteFolder(string name, ADFolder folder = null)
        {
            if(document != null)
            {
                var foldertodelete = document.GetFolder(name, folder);
                if(foldertodelete != null)
                {
                    foldertodelete.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the server configuration
        /// </summary>
        /// <returns>an ADGeneralSettings object, containing the server settings, null if the project is not opened</returns>
        public ADGeneralSettings GetServerConfiguration()
        {
            if (document != null)
                return document.GetConfiguration();
            return null;
        }
        /// <summary>
        /// Add a new base address to the server configuration
        /// </summary>
        /// <param name="transport"></param>
        /// <returns>Possible values are: Opc.Ua.Utils.UriSchemeNetPipe, Opc.Ua.Utils.UriSchemeHttp, Opc.Ua.Utils.UriSchemeHttps, Opc.Ua.Utils.UriSchemeNoSecurityHttp, Opc.Ua.Utils.UriSchemeOpcTcp, Opc.Ua.Utils.UriSchemeNetTcp</returns>
        public ADBaseAddress AddBaseAddress(string transport = Opc.Ua.Utils.UriSchemeNetPipe)
        {
            if (document != null)
                return document.AddNewBaseAddress(transport);
            return null;
        }

        /// <summary>
        /// return a base address corresponding to the transport parameter
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public ADBaseAddress GetBaseAddress(string transport)
        {
            if (document != null)
                return document.GetBaseAddress(transport);
            return null;
        }

        public void Save()
        {
            if (document == null)
                return;

            if (document != null && document.NeedsSave)
                document.SaveToFile();
        }

        public void Dispose()
        {
            if (document != null)
            {
                document.Dispose();
                document = null;
            }
        }
        #endregion Methods
    }
}
