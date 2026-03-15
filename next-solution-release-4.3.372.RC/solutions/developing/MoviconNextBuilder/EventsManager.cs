using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandManager;
using DocumentManager.ComponentService;
using UFEventEditor.ComponentService;
using UFEventEditor.Document;
using UFEventModel;
using Utilities;

namespace MoviconNextBuilder
{
    public class EventsManager : IDisposable
    {
        #region Ctor
        public EventsManager(IDocument parent, EventEditorManagerComponent eventmanager, ProjectBuilder prj = null)
        {
            evComponent = eventmanager;
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            document = EventEditorDocument.FromFile(uri.GetPathString(), evComponent, parent);
            projectBuilder = prj;
        }
        #endregion Ctor

        #region data
        EventEditorDocument document;
        ProjectBuilder projectBuilder = null;
        #endregion data

        #region Properties
        EventEditorManagerComponent evComponent;
        public EventEditorManagerComponent EventComponent
        {
            get { return evComponent; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds an event
        /// </summary>
        /// <param name="name">name of the event to create</param>
        /// <param name="folder">folder containing the event (optional)</param>
        /// <returns>retruns the UFEventObject object relative to the event</returns>
        public UFEventObject AddEvent(string name, UFEventFolder folder = null)
        {
            if(document != null)
            {
                var evnt = document.GetEvent(name, folder);
                if (evnt != null)
                    return evnt;
                evnt = document.AddNewEvent(folder);
                evnt.Name = name;
                return evnt;
            }
            return null;
        }
        /// <summary>
        /// returns an existing Event
        /// </summary>
        /// <param name="name">Event name</param>
        /// <param name="folder">folder containing the Event</param>
        /// <returns>returns the Event, if exists, otherwise null</returns>
        public UFEventObject GetEvent(string name, UFEventFolder folder = null)
        {
            if (document != null)
                return document.GetEvent(name, folder);
            return null;
        }
        /// <summary>
        /// Delete an Event
        /// </summary>
        /// <param name="name">Event name</param>
        /// <param name="folder">folder containing the Event</param>
        /// <returns>True upon deletion, false otherwise</returns>
        public bool DeleteEvent(string name, UFEventFolder folder = null)
        {
            if (document != null)
            {
                var evnt = document.GetEvent(name, folder);
                if (evnt != null)
                {
                    evnt.Delete();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Adds a command to the event command list
        /// </summary>
        /// <param name="evnt">name of the event</param>
        /// <param name="command">command to add</param>
        /// <returns></returns>
        public bool AddCommand(UFEventObject evnt, CommandManager.CommandManager command)
        {
            if (evnt == null || command == null)
                return false;

            var currList = evnt.CommandList as CommandManagerList;
            var cmdlist = new CommandManagerList();
            if (currList.Count > 0)
                cmdlist.AddRange(currList);
            cmdlist.Add(command);

            evnt.CommandList = cmdlist;
            
            return true;
        }
        /// <summary>
        /// Add a new folder 
        /// </summary>
        /// <param name="name">Name of the folder to add</param>
        /// <param name="folder">existing folder into which create the new one (optional)</param>
        /// <returns>The new folder object, null if the project is not opened</returns>
        public UFEventFolder AddFolder(string name, UFEventFolder folder = null)
        {
            if (document != null)
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
        public UFEventFolder GetFolder(string name, UFEventFolder folder = null)
        {
            if (document != null)
                return document.GetFolder(name, folder);
            return null;
        }

        /// <summary>
        /// delete a folder
        /// </summary>
        /// <param name="name">name of the folder to delete</param>
        /// <param name="folder">folder containing the folder to delete (optional)</param>
        /// <returns>true upon deletion, false otherwise</returns>
        public bool DeleteFolder(string name, UFEventFolder folder = null)
        {
            if (document != null)
            {
                var foldertodelete = document.GetFolder(name, folder);
                if (foldertodelete != null)
                {
                    foldertodelete.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add the tag that rules the event firing
        /// </summary>
        /// <param name="ev">This is the UFEventObjet, where set the new Tag</param>
        /// <param name="tag">OPCUAEntityReference, representing the I/O server tag that will fire the event</param>
        public void AddTagEvent(UFEventObject ev, OPCUAViewModel.OPCUAEntityReference tag)
        {
            if (ev != null && tag != null)
                ev.Tag = tag.ToXml();
        }

        /// <summary>
        /// Add the tag that enables the event, in runtime
        /// </summary>
        /// <param name="ev">This is the UFEventObjet, where set the new Tag</param>
        /// <param name="tag">OPCUAEntityReference, representing the I/O server tag that enable the event</param>
        public void AddEnableTagEvent(UFEventObject ev, OPCUAViewModel.OPCUAEntityReference tag)
        {
            if (ev != null && tag != null)
                ev.EnableTag = tag.ToXml();
        }

        /// <summary>
        /// Add the tag that contains the activation value of the event
        /// </summary>
        /// <param name="ev">This is the UFEventObjet, where set the new Tag</param>
        /// <param name="tag">OPCUAEntityReference, representing the I/O server tag whose value will fire the event</param>
        public void AddValueTagEvent(UFEventObject ev, OPCUAViewModel.OPCUAEntityReference tag)
        {
            if (ev != null && tag != null)
                ev.ValueTag = tag.ToXml();
        }

        /// <summary>
        /// Add the tag that rules the event firing
        /// </summary>
        /// <param name="name">Name of the UFEventObject </param>
        /// <param name="tag">OPCUAEntityReference, representing the I/O server tag that will fire the event</param>
        /// <param name="folder">UFEventFolder, containing the UFEventObjet (Optional)</param>
        /// <returns></returns>
        public UFEventObject AddTagEvent(string name, OPCUAViewModel.OPCUAEntityReference tag, UFEventFolder folder = null)
        {
            var ev = GetEvent(name, folder);
            if (ev != null && tag != null)
                ev.Tag = tag.ToXml();
            return ev;
        }

        /// <summary>
        /// Add the tag that enables the event, in runtime
        /// </summary>
        /// <param name="name">Name of the UFEventObject</param>
        /// <param name="tag">OPCUAEntityReference, representing the I/O server tag that enable the event</param>
        /// <param name="folder">UFEventFolder, containing the UFEventObjet (Optional)</param>
        /// <returns></returns>
        public UFEventObject AddEnableTagEvent(string name, OPCUAViewModel.OPCUAEntityReference tag, UFEventFolder folder = null)
        {
            var ev = GetEvent(name, folder);
            if (ev != null && tag != null)
                ev.EnableTag = tag.ToXml();
            return ev;
        }

        /// <summary>
        /// Add the tag that contains the activation value of the event
        /// </summary>
        /// <param name="name">Name of the UFEventObject</param>
        /// <param name="tag">OPCUAEntityReference, representing the I/O server tag whose value will fire the event</param>
        /// <param name="folder">UFEventFolder, containing the UFEventObjet (Optional)</param>
        /// <returns></returns>
        public UFEventObject AddValueTagEvent(string name, OPCUAViewModel.OPCUAEntityReference tag, UFEventFolder folder = null)
        {
            var ev = GetEvent(name, folder);
            if (ev != null && tag != null)
                ev.ValueTag = tag.ToXml();
            return ev;
        }

        /// <summary>
        /// Add the tag that rules the event firing
        /// </summary>
        /// <param name="name">Name of the UFEventObject</param>
        /// <param name="tagname">Name of the I/O server tag that will fire the event</param>
        /// <param name="folder">UFEventFolder, containing the UFEventObjet (Optional)</param>
        /// <returns></returns>
        public UFEventObject AddTagEvent(string name, string tagname, UFEventFolder folder = null)
        {
            OPCUAViewModel.OPCUAEntityReference tag = null;
            if (projectBuilder == null)
                return null;
            var ev = GetEvent(name, folder);
            if (ev != null && projectBuilder.IODataServer != null)
            {
                tag = projectBuilder.IODataServer.GetTagOPCUAEntityReference(tagname);
                if (tag != null)
                    ev.Tag = tag.ToXml();
            }
            return ev;
        }

        /// <summary>
        /// Add the tag that enables the event, in runtime
        /// </summary>
        /// <param name="name">Name of the UFEventObject</param>
        /// <param name="tagname">Name of the I/O server tag that enable the event</param>
        /// <param name="folder">UFEventFolder, containing the UFEventObjet (Optional)</param>
        /// <returns></returns>
        public UFEventObject AddEnableTagEvent(string name, string tagname, UFEventFolder folder = null)
        {
            OPCUAViewModel.OPCUAEntityReference tag = null;
            if (projectBuilder == null)
                return null;
            var ev = GetEvent(name, folder);
            if (ev != null && projectBuilder.IODataServer != null)
            {
                tag = projectBuilder.IODataServer.GetTagOPCUAEntityReference(tagname);
                if (tag != null)
                    ev.EnableTag= tag.ToXml();
            }
            return ev;
        }

        /// <summary>
        /// Add the tag that contains the activation value of the event
        /// </summary>
        /// <param name="name">Name of the UFEventObject</param>
        /// <param name="tagname">Name of the I/O server tag whose value will fire the event</param>
        /// <param name="folder">UFEventFolder, containing the UFEventObjet (Optional)</param>
        /// <returns></returns>
        public UFEventObject AddValueTagEvent(string name, string tagname, UFEventFolder folder = null)
        {
            OPCUAViewModel.OPCUAEntityReference tag = null;
            if (projectBuilder == null)
                return null;
            var ev = GetEvent(name, folder);
            if (ev != null && projectBuilder.IODataServer != null)
            {
                tag = projectBuilder.IODataServer.GetTagOPCUAEntityReference(tagname);
                if (tag != null)
                    ev.ValueTag = tag.ToXml();
            }
            return ev;
        }

        public void Save()
        {
            if (document == null)
                return;

            if (document != null && document.NeedsSave)
            {
                document.SaveToFile();
                document.SaveToFile(discargechanges: true);
            }
                
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
