using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace MSEditor
{
    /// <summary>
    /// Our commands class. This contains all commands we would like to use in our application.
    /// Each command has properties for name, text (using an _ for mnemonics), gestures (like Ctrl+N),
    /// and gesture display text. Fill out strings in the application resources for each of these values
    /// and expose a get property for each command in use.
    /// 
    /// The gesture strings are a ; delimted collection of items parseable by the KeyGestureConverter
    /// class. The gesture display strings are what is displayed to the user in a menu item (for example)
    /// for a particular gesture. Odds are that the gesture display strings will be duplicates
    /// of the gesture strings, but this is how Microsoft does it under the hood.
    /// </summary>
    public static class UIGeneralCommands
    {

        private static GeneralCommand _AddNewEvent = new GeneralCommand(
            Properties.UICommandResource.AddNewEventName,
            Properties.UICommandResource.AddNewEventText,
            Properties.UICommandResource.AddNewEventGestures,
            Properties.UICommandResource.AddNewEventGesturesDisplayText,
            Properties.UICommandResource.AddNewEventTooltip,
            Properties.UICommandResource.AddNewEventDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewEvent
        {
            get { return _AddNewEvent; }
        }

        private static GeneralCommand _AddNewFolder = new GeneralCommand(
            Properties.UICommandResource.AddNewFolderName,
            Properties.UICommandResource.AddNewFolderText,
            Properties.UICommandResource.AddNewFolderGestures,
            Properties.UICommandResource.AddNewFolderGesturesDisplayText,
            Properties.UICommandResource.AddNewFolderTooltip,
            Properties.UICommandResource.AddNewFolderDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewFolder
        {
            get { return _AddNewFolder; }
        }

        private static GeneralCommand _StartServer = new GeneralCommand(
            Properties.UICommandResource.StartServerName,
            Properties.UICommandResource.StartServerText,
            Properties.UICommandResource.StartServerGestures,
            Properties.UICommandResource.StartServerGesturesDisplayText,
            Properties.UICommandResource.StartServerTooltip,
            Properties.UICommandResource.StartServerDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StartServer
        {
            get { return _StartServer; }
        }

        private static GeneralCommand _StopServer = new GeneralCommand(
            Properties.UICommandResource.StopServerName,
            Properties.UICommandResource.StopServerText,
            Properties.UICommandResource.StopServerGestures,
            Properties.UICommandResource.StopServerGesturesDisplayText,
            Properties.UICommandResource.StopServerTooltip,
            Properties.UICommandResource.StopServerDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StopServer
        {
            get { return _StopServer; }
        }

        private static GeneralCommand _ShowToolbar = new GeneralCommand(
             Properties.UICommandResource.ShowToolbarName,
             Properties.UICommandResource.ShowToolbarText,
             Properties.UICommandResource.ShowToolbarGestures,
             Properties.UICommandResource.ShowToolbarGesturesDisplayText,
             Properties.UICommandResource.ShowToolbarTooltip,
             Properties.UICommandResource.ShowToolbarDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand ShowToolbar
        {
            get { return _ShowToolbar; }
        }

        private static GeneralCommand _ServiceManager = new GeneralCommand(
            Properties.UICommandResource.ServiceManagerName,
            Properties.UICommandResource.ServiceManagerText,
            Properties.UICommandResource.ServiceManagerGestures,
            Properties.UICommandResource.ServiceManagerGesturesDisplayText,
            Properties.UICommandResource.ServiceManagerTooltip,
            Properties.UICommandResource.ServiceManagerDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ServiceManager
        {
            get { return _ServiceManager; }
        }
    }
}