using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFUAModel;
using DocumentManager.ComponentService;
using UFUAEditor.Document;
using Utilities;
using UFUAEditor.Controls;
using UFUAEditor.ComponentService;
using OPCUAViewModel;
using System.ComponentModel;
using System.Reflection;
using DriverBaseInterfaces;
#if !NET_STANDARD
using TempVariablesModel;
using TempVariablesManager;
#endif

namespace MoviconNextBuilder
{
    /* Loads the data relative to the I/O Data Server of the project
     * 
    */
    public class IODataServer : IDisposable
    {
        #region Ctor
        public IODataServer(IDocument parent, UFUAEditorManagerComponent editormanager)
        {
            ufuaEditorComponent = editormanager;
            currentParent = parent;
            currentUri = new System.Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            LoadDocument();
        }

        ~IODataServer()
        { }
        #endregion Ctor

        #region data
        UFUAServerDocument document;
#if !NET_STANDARD
        TempVariables tempVar;
        TempVariablesManager.Document.TempVariablesPersistence tempDocument;
#endif
        readonly IDocument currentParent;
        readonly Uri currentUri;
        #endregion data

        #region Properties
        UFUAEditorManagerComponent ufuaEditorComponent;
        public UFUAEditor.ComponentService.UFUAEditorManagerComponent UFUAEditorComponent
        {
            get
            {
                return ufuaEditorComponent;
            }
        }
        #endregion Properties

        #region Methods
        void LoadDocument()
        {
            //loading tags from the file if it exists
            document = UFUAServerDocument.FromFile(currentUri.GetPathString(), ufuaEditorComponent, currentParent, bUseCacheUow: true);
#if !NET_STANDARD        
            if (document != null)
            {
                var tempVariablesInt = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                if (tempVariablesInt != null)
                {
                    tempVar = (TempVariables)tempVariablesInt;
                    tempDocument = tempVar.GetDocument(document);
                }
            }
#endif
        }

        void CleanDocument()
        {
            if (dictEditingDrivers.Count > 0)
            {
                foreach (var d in dictEditingDrivers.Values)
                {
                    d.Dispose();
                }
                dictEditingDrivers.Clear();
            }
            if (document != null)
            {
                document.Dispose();
                document = null;
            }
#if !NET_STANDARD
            if (tempDocument != null)
            {
                if (tempVar.CurrentDocument != tempDocument)
                    tempDocument.Dispose();
                tempDocument = null;
            }
#endif
        }
        /// <summary>
        /// Adds a prototype to the Address space
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public UFUATagPrototype AddPrototype(string name, bool overwrite = false)
        {
            if(document != null)
            {
                var proto = document.FindPrototypeByName(name);
                if(proto != null)
                {
                    if (!overwrite)
                        return proto;
                    proto.Delete();
                }
                proto = document.AddNewPrototype();
                proto.Name = name;
                return proto;
            }
            return null;
        }

        /// <summary>
        /// Adda member tag to a prorotype.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="proto"></param>
        /// <param name="type"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public UFUATag AddPrototypeMember(string name, UFUATagPrototype proto, DataType type = DataType.Byte, bool overwrite = false)
        {
            if (document != null && proto != null)
            {
                var members = (from m in proto.Members where m.Name == name select m).ToList();
                if (members.Count > 0)
                {
                    if (!overwrite)
                        return members[0];
                    members[0].Delete();
                }

                var tag = document.AddNewTag(proto);
                tag.Name = name;
                tag.DataType = type;
                return tag;
            }
            return null;
        }

        /// <summary>
        /// returns a prototype by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UFUATagPrototype GetPrototype(string name)
        {
            if(document != null)
                return document.FindPrototypeByName(name);
            return null;
        }

        /// <summary>
        /// delete apropttype by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeletePrototype(string name)
        {
            if(document != null)
            {
                var prototypetodelete = document.FindPrototypeByName(name);
                if(prototypetodelete != null)
                {
                    prototypetodelete.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add a folder to a structure
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="proto"></param>
        /// <param name="overwrite"></param>
        /// <returns>UFUAFolder</returns>
        public UFUAFolder AddPrototypeFolder(string folderName, UFUATagPrototype proto, bool overwrite = false)
        {
            UFUAFolder newFolder = null;

            if (document != null && proto != null)
            {
                var session = document.GetSession();

                newFolder = new UFUAFolder(session);
                newFolder.Name = folderName;

                var existingFolder = proto.Folders?.FirstOrDefault(folder => folder.Name == folderName);

                if (existingFolder != null)
                {
                    if (overwrite == false)
                    {
                        return existingFolder;
                    }
                    else
                    {
                        existingFolder.Delete();
                    }
                }
                proto.Folders.Add(newFolder);
            }

            return newFolder;
        }

        /// <summary>
        /// Delete a folder from a structure 
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="proto"></param>
        /// <returns>Bool, true if the selected structure's folder has been deleted</returns>
        public bool DeletePrototypeFolder(string folderName, UFUATagPrototype proto)
        {
            var folderDeleted = false;

            if (proto != null)
            {
                var prototypeFolder = proto.Folders?.FirstOrDefault(folder => folder.Name == folderName);

                if (prototypeFolder != null)
                {
                    prototypeFolder.Delete();
                    folderDeleted = true;
                }
            }

            return folderDeleted;
        }

        /// <summary>
        /// Get a folder from a structure 
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="proto"></param>
        /// <returns>UFUAFolder, null if the structure has no folders or the folder name is not found</returns>
        public UFUAFolder GetPrototypeFolder(string folderName, UFUATagPrototype proto)
        {
            UFUAFolder prototypeFolder = null;

            if (proto != null)
            {
                prototypeFolder = proto.Folders?.FirstOrDefault(folder => folder.Name == folderName);
            }

            return prototypeFolder;
        }

        /// <summary>
        /// Get the structure's folders list 
        /// </summary>
        /// <param name="proto"></param>
        /// <returns>List<UFUAFolder>, empty list if the structure has no folders</returns>
        public List<UFUAFolder> GetPrototypeFoldersList(UFUATagPrototype proto)
        {
            var prototypeFoldersList = new List<UFUAFolder>();

            if (proto != null && proto.Folders?.Count > 0)
            {
                prototypeFoldersList = proto.Folders.ToList();
            }

            return prototypeFoldersList;
        }

        /// <summary>
        /// create a new Tag in the server address space. If folder is null, refer to the Address Space root.
        /// The new Tag is returned on succesful creation, otherwise returns null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <param name="type"></param>
        /// <param name="overwrite"></param>
        /// <returns>UFUATag object, null if no tag has been created</returns>
        public UFUATag AddTag(string name, UFUAFolder folder = null, DataType type = DataType.Byte, bool overwrite = false)
        {
            if(document == null)
                return null;
            var tag = document.GetTag(name, folder);
            if(tag != null)
            {
                //variable exists with that name
                if (!overwrite)
                    return null;

                tag.Delete();
            }

            tag = document.AddNewTag(folder);
            tag.Name = name;
            tag.DataType = type;
            return tag;
        }

        /// <summary>
        /// return a list of Tag, contained in the parameter folder. If folder is null, refer to the Address Space root.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public List<UFUATag> GetTagList(UFUAFolder folder = null)
        {
            List<UFUATag> tagList = new List<UFUATag>();
            if (document != null)
                tagList.AddRange(document.GetTagCollection(folder));
            return tagList;
        }
        /// <summary>
        /// return an existing Tag. If folder is null, refer to the Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns>UFUATag object existing</returns>
        public UFUATag GetTag(string name, UFUAFolder folder = null)
        {
            if (document != null)
                return document.GetTag(name, folder);
            return null;
        }
        /// <summary>
        /// delete a tag by its name. If folder is null, refer to the Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool DeleteTag(string name, UFUAFolder folder = null)
        {
            if (document != null)
            {
                var tag = document.GetTag(name, folder);
                if (tag != null)
                {
                    tag.Delete();
                    return true;
                }
            }
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFUATag GetUFATag(string tagname, string membername = null)
        {
            return GetUFUATag(tagname, membername);
        }

        public UFUATag GetUFUATag(string tagname, string membername = null)
        {
            if (document != null)
                return document.GetUFUATag(tagname, membername, false);
            return null;
        }


        public List<UFUATag> GetTagMembers(UFUATag tag)
        {
            List<UFUATag> tagList = new List<UFUATag>();
#if !NET_STANDARD
            if (document != null && tag != null && !string.IsNullOrEmpty(tag.PrototypeName))
            {
                document.CreateSubPrototype(tag);
                if(tag.SubPrototypeMembers != null && tag.SubPrototypeMembers.Count > 0)
                {
                    tagList.AddRange(tag.SubPrototypeMembers[0].Members);
                }
            }
#endif
            return tagList;
        }


        /// <summary>
        /// create a new folder. If the foder already exeists, return the existing one.
        /// if folder argument is null, the new folder is created under the Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public UFUAFolder AddFolder(string name, UFUAFolder folder = null)
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
        /// return an existing folder. If folder parameter is null, the search refer to the Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public UFUAFolder GetFolder(string name, UFUAFolder folder = null)
        {
            if (document != null)
                return document.GetFolder(name, folder);
            return null;
        }


        /// <summary>
        /// returns a list of folder, contained in the folder passed as parameter. If folder parameter is null, the search refer to the Address Space root.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public List<UFUAFolder> GetFolderList(UFUAFolder folder = null)
        {
            List<UFUAFolder> list = new List<UFUAFolder>();
            if (document != null)
                list.AddRange(document.GetFolderCollection(folder));
            return list;
        }

        /// <summary>
        /// delete a folder by its name. if folder parameter is null, refer to the Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool DeleteFolder(string name, UFUAFolder folder = null)
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
        /// Create a new datalogger. If creation is not succesful, returns null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public DataLoggerModel.DataLoggerSettings AddDatalogger(string name, bool overwrite = false)
        {
            if (document != null)
            {
                var datalogger = document.GetDataLogger(name);
                if(datalogger != null)
                {
                    //datalogger already exists!!
                    if (!overwrite)
                        return null;
                    datalogger.Delete();
                }
                datalogger = document.AddNewDataLoggerSettings();
                datalogger.Name = name;
                return datalogger;
            }
            return null;
        }

        /// <summary>
        /// return an existing Datalogger.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DataLoggerModel.DataLoggerSettings GetDatalogger(string name)
        {
            if (document != null)
                return document.GetDataLogger(name);
            return null;
        }

        /// <summary>
        /// delete a datalogger by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteDatalogger(string name)
        {
            if(document != null)
            {
                var datalogger = document.GetDataLogger(name);
                if(datalogger != null)
                {
                    datalogger.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// returns a list of the datalogger prenesn in the project
        /// </summary>
        /// <returns></returns>
        public List<string> GetDataloggerList()
        {
            List<string> dlList = new List<string>();
            if (document != null)
                dlList.AddRange(document.GetDataLoggerSettingsNames() as List<string>);
            return dlList;
        }
        /// <summary>
        /// Create a new column, associated to the datalogger specified with the datalogger parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="datalogger"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public DataLoggerModel.DataLoggerColumn AddDataLoggerColumn(string name, DataLoggerModel.DataLoggerSettings datalogger, bool overwrite = false)
        {
            if(document != null && datalogger != null)
            {
                var list = (from c in datalogger.Columns where c.Name == name select c).ToList();
                if(list.Count > 0)
                {
                    //column already exists!!
                    if (!overwrite)
                        return null;
                    list[0].Delete();
                }
                var column = document.AddNewDataLoggerColumn(datalogger);
                column.ColumnName = name;
                return column;
            }
            return null;
        }

        public DataLoggerModel.DataLoggerColumn GetDataloggerColumn(string name, DataLoggerModel.DataLoggerSettings datalogger)
        {
            if (document != null && datalogger != null)
            {
                var list = (from c in datalogger.Columns where c.Name == name select c).ToList();
                if (list.Count > 0)
                    return list[0];
            }
            return null;
        }
        public bool DeleteDataloggerColumn(string name, DataLoggerModel.DataLoggerSettings datalogger)
        {
            var deletecolumn = GetDataloggerColumn(name, datalogger);
            if (deletecolumn != null)
            {
                deletecolumn.Delete();
                return true;
            }

            return false;
        }

        public OPCUAEntityReference GetServerEntityReference()
        {
            if(document != null)
            {
                return document.GetServerOPCUAEntityReference();
            }

            return null;
        }
        /// <summary>
        /// Create a new Alarm Area. If the Area already exists, return the existing one.
        /// If parent is null, the new Area is created under the root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public UFUAArea AddAlarmArea(string name, UFUAArea parent = null)
        {
            if(document != null)
            {
                var area = document.GetAlarmArea(name, parent);
                if (area != null)
                    return area;
                area = document.AddNewAlarmArea(parent);
                area.Name = name;
                return area;
            }
            return null;
        }

        /// <summary>
        /// returns an existing Alarm Area. If parent is null, the search refer to the root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public UFUAArea GetAlarmArea(string name, UFUAArea parent = null)
        {
            if (document != null)
                return document.GetAlarmArea(name, parent);
            return null;
        }

        /// <summary>
        /// delete an Alarm Area by its name. If parent is null, refer to the root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool DeleteAlarmArea(string name, UFUAArea parent = null)
        {
            if(document != null)
            {
                var areatodelete = document.GetAlarmArea(name, parent);
                if(areatodelete != null)
                {
                    areatodelete.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Create a new Alarm Source. If the Alarm Source already exists, return the existing one.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public UFUAAlarmSource AddAlarmSource(string name, UFUAArea parent)
        {
            if(document != null && parent != null)
            {
                var source = document.GetAlarmSource(name, parent);
                if (source == null)
                    source = document.AddNewAlarmSource(parent, name);
                return source;
            }
            return null;
        }

        /// <summary>
        /// returns an existing Alarm Source.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public UFUAAlarmSource GetAlarmSource(string name, UFUAArea parent)
        {
            if (document != null && parent != null)
                return document.GetAlarmSource(name, parent);
            return null;
        }

        /// <summary>
        /// delete an Alarm Source by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool DeleteAlarmSource(string name, UFUAArea parent)
        {
            if(document != null && parent != null)
            {
                var sourcetodelete = document.GetAlarmSource(name, parent);
                if(sourcetodelete != null)
                {
                    sourcetodelete.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// create a new Alarm Definition.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public UFUAAlarmDefinition AddAlarmDefinition(string name, UFUAAlarmSource parent, bool overwrite = false)
        {
            if(document != null && parent != null)
            {
                var alarmdef = document.GetAlarmPrototype(name, parent);
                if(alarmdef != null)
                {
                    if (!overwrite)
                        return null;
                    alarmdef.Delete();
                }
                alarmdef = document.AddNewAlarmPrototype(parent);
                alarmdef.Name = name;
                return alarmdef;
            }
            return null;
        }

        /// <summary>
        /// returns an exixting Alarm Definition
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public UFUAAlarmDefinition GetAlarmDefinition(string name, UFUAAlarmSource parent)
        {
            if(document != null && parent != null)
                return document.GetAlarmPrototype(name, parent);
            return null;
        }

        /// <summary>
        /// deletes an alarm definition by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool DeleteAlarmDefinition(string name, UFUAAlarmSource parent)
        {
            if(document != null && parent != null)
            {
                var deftodelete = document.GetAlarmPrototype(name, parent);
                if (deftodelete != null)
                {
                    deftodelete.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Assign an alarm to a tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="alarm"></param>
        /// <param name="type">0 = Single, 1 = AnyTagBit, 2 = AnyTagElement</param>
        /// <param name="text">Alarm text prefix </param>
        /// <returns></returns>
        public bool AssignAlarmToTag(UFUATag tag, UFUAAlarmDefinition alarm, int type = 0, string text = "")
        {

            if (document != null && tag != null && alarm != null)
            {
                var list = document.GetNewThresholdList(alarm, tag, (type == 2 ? AssignAlarmType.AnyTagElement : (type == 1 ? AssignAlarmType.AnyTagBit : AssignAlarmType.Single)), text, !string.IsNullOrEmpty(text));
                if(list.Count > 0)
                {
                    tag.UFUAAlarmThresholds.AddRange(list);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove an alarm definition from a tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="alarm"></param>
        /// <returns></returns>
        public bool RemoveAlarmFromTag(UFUATag tag, UFUAAlarmDefinition alarm)
        {
            if(tag != null && alarm != null)
                return tag.RemoveAlarmDefinition(alarm);
            return false;
        }


        /// <summary>
        /// returns the list of Alarm areas
        /// </summary>
        /// <param name="parent">UFUAArea from which extract its UFUAAreas, null to get from the whole project</param>
        /// <returns>List of UFUAArea from the project</returns>
        public List<UFUAArea> GetAlarmAreaList(UFUAArea parent = null)
        {
            if (document != null)
            {
                List<UFUAArea> areaList = new List<UFUAArea>();
                areaList.AddRange(document.GetAlarmAreas(parent));
                return areaList;
            }
            return null;
        }

        /// <summary>
        /// returns a list of the Alarm Sources from an Alarm Area
        /// </summary>
        /// <param name="area">UFUAArea to search into, null for the whole project</param>
        /// <returns>List of UFUAAlarmSource from the project</returns>
        public List<UFUAAlarmSource> GetAlarmSourceList(UFUAArea area)
        {
            if (document != null)
            {
                List<UFUAAlarmSource> sourceList = new List<UFUAAlarmSource>();
                sourceList.AddRange(document.GetSourceCollection(area));
                return sourceList;
            }
            return null;
        }
        /// <summary>
        /// returns the list of Alarm Definitions of an Alarm Area
        /// </summary>
        /// <param name="source">UFUAAlarmSource to search into</param>
        /// <returns>List of UFUAAlarmDefinition for the source.</returns>
        public List<UFUAAlarmDefinition> GetAlarmDefinitionList(UFUAAlarmSource source)
        {
            if (document != null)
            {
                List<UFUAAlarmDefinition> definitionList = new List<UFUAAlarmDefinition>();
                if (source != null)
                    definitionList.AddRange(document.GetAlarmDefinitions(source));
                else
                    definitionList.AddRange(document.GetAlarmDefinitions());
                return definitionList;
            }
            return null;
        }


        /// <summary>
        /// adds a new Engineering unit
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public UFUAEngineeringUnit AddEngineeringUnit(string name, bool overwrite = false)
        {
            if(document != null)
            {
                var eu = document.GetEngineeringUnit(name);
                if(eu != null)
                {
                    if (!overwrite)
                        return null;
                    eu.Delete();
                }
                eu = document.AddNewEngineeringUnits();
                eu.Name = name;
                return eu;
            }
            return null;
        }

        /// <summary>
        /// returns an engineering Units by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UFUAEngineeringUnit GetEngineeringUnit(string name)
        {
            if(document != null)
                return document.GetEngineeringUnit(name);
            return null;
        }

        /// <summary>
        /// delete an Engineering Unit by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteEngineeringUnit(string name)
        {
            if(document != null)
            {
                var eu = document.GetEngineeringUnit(name);
                if(eu != null)
                {
                    eu.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// returns the current server general configuration
        /// </summary>
        /// <returns></returns>
        public UFUAConfiguration GetServerConfiguration()
        {
            if(document != null)
                return document.GetConfiguration();
            return null;
        }

        public bool RemoveBaseAddress(string transport)
        {
            if (document != null)
                return document.RemoveBaseAddress(transport);
            return false;
        }

        public List<UFUABaseAddress> GetBaseAddressList()
        {
            if (document != null)
                return document.GetBaseAddressList();
            return null;
        }
        /// <summary>
        /// Add a new base address to the server configuration
        /// </summary>
        /// <param name="transport"></param>
        /// <returns>Possible values are: Opc.Ua.Utils.UriSchemeNetPipe, Opc.Ua.Utils.UriSchemeHttp, Opc.Ua.Utils.UriSchemeHttps, Opc.Ua.Utils.UriSchemeNoSecurityHttp, Opc.Ua.Utils.UriSchemeOpcTcp, Opc.Ua.Utils.UriSchemeNetTcp</returns>
        public UFUABaseAddress AddBaseAddress(string transport =
#if !NET_STANDARD
            Opc.Ua.Utils.UriSchemeNetPipe
#else
            Opc.Ua.Utils.UriSchemeOpcTcp
#endif
            )
        {
            if (
#if !NET_STANDARD
                transport != Opc.Ua.Utils.UriSchemeHttp &&
#endif
                transport != Opc.Ua.Utils.UriSchemeHttps &&
#if !NET_STANDARD
                transport != Opc.Ua.Utils.UriSchemeNetPipe &&
                transport != Opc.Ua.Utils.UriSchemeNetTcp &&
                transport != Opc.Ua.Utils.UriSchemeNoSecurityHttp &&
#endif
                transport != Opc.Ua.Utils.UriSchemeOpcTcp)
                return null;

            UFUABaseAddress ret = null;
            if (document != null)
            {
                ret = document.GetBaseAddress(transport);
                if (ret == null)
                    ret = document.AddNewBaseAddress(transport);
            }

            return ret;
        }

        /// <summary>
        /// return a base address corresponding to the transport parameter
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public UFUABaseAddress GetBaseAddress(string transport)
        {
            if(document != null)
                return document.GetBaseAddress(transport);
            return null;
        }

        /// <summary>
        /// Add a new driver
        /// </summary>
        /// <param name="assemblyname">name of the assembly es. "ModbusTCP.dll"</param>
        /// <param name="friendlyname"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public bool AddNewDriver(string assemblyname, string friendlyname, string factory)
        {
            if(document != null && assemblyname.Length > 4)
            {
                var list = (from d in document.GetConfiguration().ComunicationDrivers where d.AssemblyName == assemblyname select d).ToList();
                if (list.Count > 0)
                    return false;

                var driver = document.AddNewDriver();
                driver.Factory = factory;
                driver.FriendlyName = friendlyname;
                driver.Name = assemblyname.Substring(0, assemblyname.Length - 4);
                driver.AssemblyName = assemblyname;

                document.GetConfiguration().ComunicationDrivers.Add(driver);
                return true;
            }
            return false;
        }


        public bool RemoveDriver(string assemblyname, bool force = false)
        {
            if (document != null && assemblyname.Length > 4)
            {
                UFUACommunicationDriver driver = null;
                string serchassemblyname = assemblyname.ToLower();
                var lstDrv = (from d in document.GetConfiguration().ComunicationDrivers where d.AssemblyName.ToLower() == serchassemblyname select d).ToList();
                if (lstDrv.Count > 0)
                    driver = lstDrv[0];
                int nPoint = assemblyname.LastIndexOf('.');
                if(nPoint != -1)
                    assemblyname = assemblyname.Substring(0, nPoint);

                if(driver != null && (!document.IsDriverNameUsed(assemblyname) || force))
                {
                    driver.Delete();
                    if (dictEditingDrivers.ContainsKey(serchassemblyname))
                        dictEditingDrivers.Remove(serchassemblyname);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Gets a OPCUAEntityReference relative to the named Tag.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public OPCUAEntityReference GetTagEntityReference(string name, string instance = null)
        {
            if(document != null)
                return document.GetTagOPCUAEntityReference(name, instance, inExecution: false);
            return null;
        }
        public OPCUAEntityReference GetTagOPCUAEntityReference(string tagname, string membername = null)
        {
            if (string.IsNullOrEmpty(membername))
                return GetTagEntityReference(tagname);
            else
                return GetTagEntityReference(membername, tagname);
        }

        public OPCUAEntityReference GetTagEntityReference(UFUAModel.UFUATag tag)
        {
            if (tag.UFUATagPrototype != null && tag.TagOwnerPath.Length > 0)
            {
                var ownerPath = tag.TagOwnerPath.Replace("/", "\\");
                return GetTagOPCUAEntityReference(ownerPath, tag.Name);
            }
            else
                return GetTagOPCUAEntityReference(string.IsNullOrEmpty(tag.PathIdentifier) ? tag.Name : tag.PathIdentifier);
        }

        public TagEntityReference GetVarTagEntityReference(UFUAModel.UFUATag tag)
        {
            if (tag.UFUATagPrototype != null && tag.TagOwnerPath.Length > 0)
            {
                var ownerPath = tag.TagOwnerPath.Replace("/", "\\");
                return GetVarTagEntityReference(ownerPath, tag.Name);
            }
            else
                return GetVarTagEntityReference(string.IsNullOrEmpty(tag.PathIdentifier) ? tag.Name : tag.PathIdentifier);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public TagEntityReference GetVarTagEntityReferencs(string tagname, string membername = null)
        {
            return GetVarTagEntityReference(tagname, membername);
        }

        public TagEntityReference GetVarTagEntityReference(string tagname, string membername = null)
        {
            if (document != null)
            {
                var entityReference = GetTagOPCUAEntityReference(tagname, membername);
                if (entityReference != null)
                {
                    Guid tagGuid = Guid.Empty;
                    if (entityReference.ResolvedNodeId.IdType == Opc.Ua.IdType.Guid)
                        tagGuid = (Guid)entityReference.ResolvedNodeId.Identifier;
                    else if (entityReference.ResolvedNodeId.IdType == Opc.Ua.IdType.String)
                    {
                        var identifier = entityReference.ResolvedNodeId.Identifier.ToString();
                        var index = identifier.LastIndexOf('?');
                        if (index != -1)
                            identifier = identifier.Substring(index + 1);
                        Guid.TryParse(identifier, out tagGuid);
                    }

                    return new UFUAModel.TagEntityReference(tagGuid, entityReference.RelativePath, entityReference.ResolvedNodeId, entityReference.HumanReadable);
                }
            }
            return null;
        }
        /// <summary>
        /// Adds a Historical Settings prototipe to the server
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public UFUAHistorianSettings AddHistorian(string name, bool overwrite = false)
        {
            if(document != null)
            {
                var hs = document.GetHistoricalSettings(name);
                if(hs != null)
                {
                    if (!overwrite)
                        return null;
                    hs.Delete();
                }
                hs = document.AddNewHistoricalSettings();
                if(hs != null)
                {
                    hs.Name = name;
                    return hs;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the named Historical Settings prototype, if present.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UFUAHistorianSettings GetHistorian(string name)
        {
            if (document != null)
                return document.GetHistoricalSettings(name);
            return null;
        }
        /// <summary>
        /// Delete a Historical Settings prototype
        /// </summary>
        /// <param name="name"></param>
        /// <param name="removefromtag">If True, remove the being deleted Historical Settings prototype reference from the associated tags (default: False)</param>
        /// <returns></returns>
        public bool DeleteHistorian(string name, bool removefromtag = false)
        {
            if(document != null)
            {
                var hs = document.GetHistoricalSettings(name);
                if (hs != null)
                {
                    if (removefromtag)
                        document.RemoveHistoricalSettingsFromTags(name);
                    hs.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the list of Historian present in the project.
        /// </summary>
        /// <returns></returns>
        public List<string> GetHistorianList()
        {
            List<string> hiList = new List<string>();
            if (document != null)
                hiList.AddRange(document.GetHistoricalSettingsNameList(false) as List<string>);
            return hiList;
        }

#if !NET_STANDARD
        /// <summary>
        /// create a new TempVariable folder. If the foder already exeists, return the existing one.
        /// if folder argument is null, the new folder is created under the Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Folder AddLocalFolder(string name, Folder folder = null)
        {
            if (tempDocument != null)
            {
                var newfolder = tempDocument.FindFolderByName(name, folder);
                if (newfolder != null)
                    return newfolder;
                newfolder = tempDocument.AddNewFolder(folder);
                newfolder.Name = name;
                return newfolder;
            }
            return null;
        }

        /// <summary>
        /// return an existing local folder. If folder parameter is null, the search refer to the Local Tag Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Folder GetLocalFolder(string name, Folder folder = null)
        {
            if (tempDocument != null)
                return tempDocument.FindFolderByName(name, folder);
            return null;
        }

        /// <summary>
        /// delete a Local folder by its name. if folder parameter is null, refer to the Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool DeleteLocalFolder(string name, Folder folder = null)
        {
            if (tempDocument != null)
            {
                var foldertodelete = tempDocument.FindFolderByName(name, folder);
                if (foldertodelete != null)
                {
                    foldertodelete.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// create a new Local Tag in the Local Tag address space. If folder is null, refer to the Local Tag Address Space root.
        /// The new Local Tag is returned on succesful creation, otherwise returns null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <param name="arraydimension"></param>
        /// <param name="initialvalue"></param>
        /// <param name="overwrite"></param>
        /// <returns>Variable object, null if no tag has been created</returns>
        public Variable AddLocalTag(string name, Folder folder = null, DataType type = DataType.Byte, string description = "", int arraydimension = 0, string initialvalue = "", bool overwrite = false)
        {
            if (tempDocument == null)
                return null;
            var tag = tempDocument.GetTag(name, folder);
            if (tag != null)
            {
                //variable exists with that name
                if (!overwrite)
                    return null;

                tag.Delete();
            }

            tag = tempDocument.AddNewTag(folder);
            tag.Name = name;
            tag.DataType = type;
            tag.ArrayDimension = arraydimension;
            tag.Description = description;
            tag.InitialValue = initialvalue;
            return tag;
        }

        /// <summary>
        /// return an existing Local Tag. If folder is null, refer to the Local Tag Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns>Variable object existing</returns>
        public Variable GetLocalTag(string name, Folder folder = null)
        {
            if (tempDocument != null)
                return tempDocument.GetTag(name, folder);

            return null;
        }
        /// <summary>
        /// delete a Local tag by its name. If folder is null, refer to the Local Tag Address Space root.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <returns>true upon deletion, false otherwise</returns>
        public bool DeleteLocalTag(string name, Folder folder = null)
        {
            if (tempDocument != null)
            {
                var tag = tempDocument.GetTag(name, folder);
                if (tag != null)
                {
                    tag.Delete();
                    return true;
                }
            }
            return false;
        }

        public OPCUAEntityReference GetLocalTagOPCUAEntityReference(string tagname)
        {
            if (tempVar == null)
                return null;

            string normalized = tagname.Replace('/', '\\');
            string[] parts = normalized.Split('\\');
            if(parts.Length == 1)
                return tempVar.GetReference(parts[0]);
            else if (parts.Length > 1)
            {
                if (tempDocument == null)
                    return null;

                Folder fContainer = null;
                Folder fTemp = null;
                for(int i = 0; i < parts.Length - 1; i++)
                {
                    fContainer = tempDocument.FindFolderByName(parts[i], fTemp);
                    if(fContainer != null)
                    {
                        fTemp = fContainer;
                    }
                }
                var lTag = tempDocument.GetTag(parts[parts.Length - 1], fContainer);
                if(lTag != null)
                    return tempVar.GetReference(lTag);
            }
                
            return null;
        }
#endif


        #region Drivers

        private Dictionary<string, IConfigurationEditor> dictEditingDrivers = new Dictionary<string, IConfigurationEditor>();

        bool GetDriver(string assemblyname)
        {
            if (document != null && assemblyname.Length > 4)
            {
                assemblyname = assemblyname.ToLower();
                if (dictEditingDrivers.ContainsKey(assemblyname))
                    return true;

                var lstDrv = (from d in document.GetConfiguration().ComunicationDrivers where d.AssemblyName.ToLower() == assemblyname select d).ToList();
                if (lstDrv.Count == 0)
                    return false;
                var driver = lstDrv[0];

                string rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                //var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(String.Format("{0}\\Drivers\\{1}", rootPath, driver.AssemblyName));
                var uidll = String.Format("{0}\\Drivers\\{1}", rootPath, driver.AssemblyName);

                try
                {
                    var a = Assembly.LoadFrom(uidll);
                    var drivername = a.GetName().Name;
                    drivername = drivername.Replace(".UI", "");
                    var types = a.GetTypes();


                    var list = (from t in types.AsParallel()
                                where !t.IsAbstract && typeof(IConfigurationEditor).IsAssignableFrom(t)
                                select (IConfigurationEditor)Activator.CreateInstance(t)).ToList();

                    if (list.Count > 0 )
                    {

                        var ConfEditor = list[0];
                        ConfEditor.Init(document.ConnectionString, drivername, document.Protected, document.Id);
                        if (ConfEditor.IsValid)
                        {
                            dictEditingDrivers[assemblyname] = ConfEditor;
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return false;
        }


        public object GetDriverConfiguration(string assemblyname)
        {
            if (GetDriver(assemblyname))
            {
                assemblyname = assemblyname.ToLower();
                return dictEditingDrivers[assemblyname].Configuration;
            }
            return null;
        }

        public object AddChannel(string assemblyname)
        {
            if (GetDriver(assemblyname))
            {
                assemblyname = assemblyname.ToLower();
                return dictEditingDrivers[assemblyname].AddChannel();
            }
            return null;
        }
        public object GetChannel(string assemblyname, string channelname)
        {
            if (GetDriver(assemblyname))
            {
                assemblyname = assemblyname.ToLower();
                return dictEditingDrivers[assemblyname].GetChannel(channelname);
            }
            return null;
        }
        public bool RemoveChannel(string assemblyname, string channelname, bool removestations = false)
        {
            if (GetDriver(assemblyname))
            {
                assemblyname = assemblyname.ToLower();
                return dictEditingDrivers[assemblyname].RemoveChannelSettings(channelname, removestations);
            }
            return false;
        }
        public List<object> GetChannelList(string assemblyname)
        {
            if (GetDriver(assemblyname))
            {
                assemblyname = assemblyname.ToLower();
                return dictEditingDrivers[assemblyname].GetChannelList();
            }
            return null;
        }
        public object AddStation(string assemblyname, string channelname)
        {
            if (GetDriver(assemblyname))
            {
                assemblyname = assemblyname.ToLower();
                return dictEditingDrivers[assemblyname].AddStation(channelname);
            }
            return null;
        }
        public object GetStation(string assemblyname, string channelname, string stationname)
        {
            if (GetDriver(assemblyname))
            {
                assemblyname = assemblyname.ToLower();
                return dictEditingDrivers[assemblyname].GetStation(channelname, stationname);
            }
            return null;
        }
        public bool RemoveStation(string assemblyname, string channelname, string stationname)
        {
            if (GetDriver(assemblyname))
            {
                assemblyname = assemblyname.ToLower();
                return dictEditingDrivers[assemblyname].RemoveStationSettings(channelname, stationname);
            }
            return false;
        }
        public List<object> GetStationList(string assemblyname)
        {
            if (GetDriver(assemblyname))
            {
                assemblyname = assemblyname.ToLower();
                return dictEditingDrivers[assemblyname].GetStationList();
            }
            return null;
        }
        public bool SaveDriver(string assemblyname)
        {
            bool ret = false;
            assemblyname = assemblyname.ToLower();
            if(dictEditingDrivers.ContainsKey(assemblyname))
            {
                var confEd = dictEditingDrivers[assemblyname];
                ret = confEd.Save();

                dictEditingDrivers.Remove(assemblyname);
                confEd.Dispose();
            }
            return ret;
        }

        //public bool LoadDriver(string assemblyname)
        //{
        //    bool ret = false;
        //    assemblyname = assemblyname.ToLower();
        //    if (dictEditingDrivers.ContainsKey(assemblyname))
        //    {
        //        var confEd = dictEditingDrivers[assemblyname];
        //        ret = confEd.Save();

        //        dictEditingDrivers.Remove(assemblyname);
        //        confEd.Dispose();
        //    }
        //    return ret;
        //}

        #endregion

        /// <summary>
        /// saves the settings, if some have been modified in the current working session.
        /// </summary>
        public void Save()
        {
            if (document == null)
                return;

#if !NET_STANDARD
            if (tempDocument != null && tempDocument.NeedsSave)
                tempVar.Save(document);
#endif

            document.SaveToFile(bForceSave: true);
            document.SaveToFile(discargechanges: true);
        }

        public void Dispose()
        {
            CleanDocument();
        }
        #endregion Methods
    }
}
