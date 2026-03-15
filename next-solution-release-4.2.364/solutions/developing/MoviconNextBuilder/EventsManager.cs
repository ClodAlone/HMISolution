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
        public EventsManager(IDocument parent, EventEditorManagerComponent eventmanager)
        {
            evComponent = eventmanager;
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            document = EventEditorDocument.FromFile(uri.GetPathString(), evComponent, parent);
        }
        #endregion Ctor

        #region data
        EventEditorDocument document;
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
