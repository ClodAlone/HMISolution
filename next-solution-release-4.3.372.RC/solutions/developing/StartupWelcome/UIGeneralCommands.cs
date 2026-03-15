using System;
using System.Windows.Input;
using Utilities;

namespace StartupWelcome
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
        private static GeneralCommand _DeployServerService = new GeneralCommand(
            Properties.UICommandResource.DeployServerServiceName,
            Properties.UICommandResource.DeployServerServiceText,
            Properties.UICommandResource.DeployServerServiceGestures,
            Properties.UICommandResource.DeployServerServiceGesturesDisplayText,
            Properties.UICommandResource.DeployServerServiceTooltip,
            Properties.UICommandResource.DeployServerServiceDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand DeployServerService
        {
            get { return _DeployServerService; }
        }

        private static GeneralCommand _DongleService = new GeneralCommand(
            Properties.UICommandResource.DongleServiceName,
            Properties.UICommandResource.DongleServiceText,
            Properties.UICommandResource.DongleServiceGestures,
            Properties.UICommandResource.DongleServiceGesturesDisplayText,
            Properties.UICommandResource.DongleServiceTooltip,
            Properties.UICommandResource.DongleServiceDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand DongleService
        {
            get { return _DongleService; }
        }


        private static GeneralCommand _GetKey = new GeneralCommand(
            Properties.UICommandResource.GetKeyName,
            Properties.UICommandResource.GetKeyText,
            Properties.UICommandResource.GetKeyGestures,
            Properties.UICommandResource.GetKeyGesturesDisplayText,
            Properties.UICommandResource.GetKeyTooltip,
            Properties.UICommandResource.GetKeyDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand GetKey
        {
            get { return _GetKey; }
        }

        private static GeneralCommand _StartUpPage = new GeneralCommand(
            Properties.UICommandResource.StartupPageName,
            Properties.UICommandResource.StartupPageText,
            Properties.UICommandResource.StartupPageGestures,
            Properties.UICommandResource.StartupPageGesturesDisplayText,
            Properties.UICommandResource.StartupPageTooltip,
            Properties.UICommandResource.StartupPageDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StartUpPage
        {
            get { return _StartUpPage; }
        }
        private static GeneralCommand _DongleOptions = new GeneralCommand(
            Properties.UICommandResource.DongleOptionsName,
            Properties.UICommandResource.DongleOptionsText,
            Properties.UICommandResource.DongleOptionsGestures,
            Properties.UICommandResource.DongleOptionsGesturesDisplayText,
            Properties.UICommandResource.DongleOptionsTooltip,
            Properties.UICommandResource.DongleOptionsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand DongleOptions
        {
            get { return _DongleOptions; }
        }

        private static GeneralCommand _DongleRequired = new GeneralCommand(
            Properties.UICommandResource.DongleRequiredName,
            Properties.UICommandResource.DongleRequiredText,
            Properties.UICommandResource.DongleRequiredGestures,
            Properties.UICommandResource.DongleRequiredGesturesDisplayText,
            Properties.UICommandResource.DongleRequiredTooltip,
            Properties.UICommandResource.DongleRequiredDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand DongleRequired
        {
            get { return _DongleRequired; }
        }

        private static GeneralCommand _UseLayoutToFile = new GeneralCommand(
            Properties.UICommandResource.UseLayoutToFileName,
            Properties.UICommandResource.UseLayoutToFileText,
            Properties.UICommandResource.UseLayoutToFileGestures,
            Properties.UICommandResource.UseLayoutToFileGesturesDisplayText,
            Properties.UICommandResource.UseLayoutToFileTooltip,
            Properties.UICommandResource.UseLayoutToFileDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand UseLayoutToFile
        {
            get { return _UseLayoutToFile; }
        }

        private static GeneralCommand _ManageLayouts = new GeneralCommand(
            Properties.UICommandResource.ManageLayoutsName,
            Properties.UICommandResource.ManageLayoutsText,
            Properties.UICommandResource.ManageLayoutsGestures,
            Properties.UICommandResource.ManageLayoutsGesturesDisplayText,
            Properties.UICommandResource.ManageLayoutsTooltip,
            Properties.UICommandResource.ManageLayoutsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ManageLayouts
        {
            get { return _ManageLayouts; }
        }
    }
}