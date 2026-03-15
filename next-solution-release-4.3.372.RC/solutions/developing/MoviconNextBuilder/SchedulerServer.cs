using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using MSEditor.ComponentService;
using MSModel;
using MSSchedulerSettings.Document;
using OPCUAViewModel;
using Utilities;

namespace MoviconNextBuilder
{
    public class SchedulerServer : IDisposable
    {
        #region Ctor
        public SchedulerServer(IDocument parent,  SchedulerEditorManagerComponent eventmanager)
        {
            eventEditorComponent = eventmanager;
            var uri = new System.Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            document = SchedulerEditorDocument.FromFile(uri.GetPathString(), eventEditorComponent, parent);
        }
        #endregion


        #region data
        SchedulerEditorDocument document;
        #endregion data

        #region Properties
        SchedulerEditorManagerComponent eventEditorComponent;
        public SchedulerEditorManagerComponent EventEditorComponent
        {
            get
            {
                return eventEditorComponent;
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Add a new Scheduler
        /// </summary>
        /// <param name="name">name of the scheduler</param>
        /// <param name="type">type of the scheduler (optional)</param>
        /// <param name="tag">OPCUAEntityReference representing the scheduler tag (optional)</param>
        /// <param name="enabletag">OPCUAEntityReference representing the enable tag (optional)</param>
        /// <param name="folder">folder containing the scheduler (optional)</param>
        /// <returns> An MSScheduledAction object representing the scheduler, null il the project is not open</returns>
        public MSScheduledAction AddScheduler(string name, ScheduleType type = ScheduleType.everyMinute, OPCUAEntityReference tag = null, OPCUAEntityReference enabletag = null, MSFolder folder = null)
        {
            if(document != null)
            {
                var action = document.AddNewEvent(folder);
                action.Name = name;
                action.Type = type;
                if(tag != null)
                {
                    action.ScheduleItem = tag.ToXml();
                }

                if (enabletag != null)
                {
                    action.EnableVariable = enabletag.ToXml();
                }
                return action;
            }
            return null;
        }
        /// <summary>
        /// returns an existing Scheduler
        /// </summary>
        /// <param name="name">Scheduler name</param>
        /// <param name="folder">folder containing the Scheduler</param>
        /// <returns>returns the Scheduler, if exists, otherwise null</returns>
        public MSScheduledAction GetScheduler(string name, MSFolder folder = null)
        {
            if (document != null)
                return document.GetEvent(name, folder);
            return null;
        }

        /// <summary>
        /// Delete a Scheduler
        /// </summary>
        /// <param name="name">Scheduler name</param>
        /// <param name="folder">folder containing the Scheduler</param>
        /// <returns>True upon deletion, false otherwise</returns>
        public bool DeleteScheduler(string name, MSFolder folder = null)
        {
            if (document != null)
            {
                var action = document.GetEvent(name, folder);
                if (action != null)
                {
                    action.Delete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add a CalendarItem to the Scheduler
        /// </summary>
        /// <param name="actionname">name of the Scheduler</param>
        /// <param name="folder">folder containing the Scheduler</param>
        /// <returns>a CalendarItem, bond to the Scheduler, null if the project is not open
        /// or the Event do not exists</returns>
        public CalendarItem AddCalendarItem(string actionname, MSFolder folder = null)
        {
            if(document != null)
            {
                var action = document.GetEvent(actionname, folder);
                if (action != null)
                {
                    var calitem = document.AddNewCalendarItem(action);
                    action.Calendar.Add(calitem);
                    return calitem;
                }
            }
            return null;
        }

        /// <summary>
        /// Add an ExceptionsCalendarItem to the Scheduler
        /// </summary>
        /// <param name="actionname">name of the Scheduler</param>
        /// <param name="folder">folder containing the Scheduler</param>
        /// <returns>an ExceptionsCalendarItem, bond to the Scheduler, null if the project is not open
        /// or the Event do not exists</returns>
        public ExceptionsCalendarItem AddExceptionCalendarItem(string actionname, MSFolder folder = null)
        {
            if (document != null)
            {
                var action = document.GetEvent(actionname, folder);
                if (action != null)
                {
                    var calitem = document.AddNewExceptionCalendarItem(action);
                    action.ExceptionsCalendar.Add(calitem);
                    return calitem;
                }
            }
            return null;
        }

        /// <summary>
        /// Add a WeeklyCalendarItem to the Scheduler
        /// </summary>
        /// <param name="actionname">name of the Scheduler</param>
        /// <param name="folder">folder containing the Scheduler</param>
        /// <returns>a WeeklyCalendarItem, bond to the Scheduler, null if the project is not open
        /// or the Scheduler do not exists</returns>
        public WeeklyCalendarItem AddWeeklyCalendarItem(string actionname, MSFolder folder = null)
        {
            if (document != null)
            {
                var action = document.GetEvent(actionname, folder);
                if (action != null)
                {
                    var calitem = document.AddNewWeeklyCalendarItem(action);
                    action.WeeklyCalendar.Add(calitem);
                    return calitem;
                }
            }
            return null;
        }

        /// <summary>
        /// Add a new folder for Scheduler
        /// </summary>
        /// <param name="name">Name of the folder to add</param>
        /// <param name="folder">existing folder into which create the new one (optional)</param>
        /// <returns>The new folder object, null if the project is not opened</returns>
        public MSFolder AddFolder(string name, MSFolder folder = null)
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
        public MSFolder GetFolder(string name, MSFolder folder = null)
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
        public bool DeleteFolder(string name, MSFolder folder = null)
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
        /// Gets the server configuration
        /// </summary>
        /// <returns>an MSGeneralSettings object, containing the server settings, null if the project is not opened</returns>
        public MSGeneralSettings GetServerConfiguration()
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
        public MSBaseAddress AddBaseAddress(string transport = Opc.Ua.Utils.UriSchemeNetPipe)
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
        public MSBaseAddress GetBaseAddress(string transport)
        {
            if (document != null)
                return document.GetBaseAddress(transport);
            return null;
        }

        /// <summary>
        /// saves the settings, if some have been modified in the current working session.
        /// </summary>
        public void Save()
        {
            if (document == null)
                return;

            if (document.NeedsSave)
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
        #endregion
    }
}
