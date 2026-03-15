using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace UFProjectManager
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
        private static GeneralCommand _SaveAll = new GeneralCommand(
            Properties.UICommandResource.SaveAllName,
            Properties.UICommandResource.SaveAllText,
            Properties.UICommandResource.SaveAllGestures,
            Properties.UICommandResource.SaveAllGesturesDisplayText,
            Properties.UICommandResource.SaveAllTooltip,
            Properties.UICommandResource.SaveAllDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SaveAll
        {
            get { return _SaveAll; }
        }

        private static GeneralCommand _CloseAll = new GeneralCommand(
            Properties.UICommandResource.CloseAllName,
            Properties.UICommandResource.CloseAllText,
            Properties.UICommandResource.CloseAllGestures,
            Properties.UICommandResource.CloseAllGesturesDisplayText,
            Properties.UICommandResource.CloseAllTooltip,
            Properties.UICommandResource.CloseAllDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CloseAll
        {
            get { return _CloseAll; }
        }

        private static GeneralCommand _ApplicationClose = new GeneralCommand(
            Properties.UICommandResource.ApplicationCloseName,
            Properties.UICommandResource.ApplicationCloseText,
            Properties.UICommandResource.ApplicationCloseGestures,
            Properties.UICommandResource.ApplicationCloseGesturesDisplayText,
            Properties.UICommandResource.ApplicationCloseTooltip,
            Properties.UICommandResource.ApplicationCloseDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ApplicationClose
        {
            get { return _ApplicationClose; }
        }

        private static GeneralCommand _CreateWebClientExpress = new GeneralCommand(
           Properties.UICommandResource.CreateWebClientExpressName,
           Properties.UICommandResource.CreateWebClientExpressText,
           Properties.UICommandResource.CreateWebClientExpressGestures,
           Properties.UICommandResource.CreateWebClientExpressGesturesDisplayText,
           Properties.UICommandResource.CreateWebClientExpressTooltip,
           Properties.UICommandResource.CreateWebClientExpressDescription,
           typeof(UIGeneralCommands));

        public static GeneralCommand CreateWebClientExpress
        {
            get { return _CreateWebClientExpress; }
        }

        private static GeneralCommand _DeployWebClientExpress = new GeneralCommand(
           Properties.UICommandResource.DeployWebClientExpressName,
           Properties.UICommandResource.DeployWebClientExpressText,
           Properties.UICommandResource.DeployWebClientExpressGestures,
           Properties.UICommandResource.DeployWebClientExpressGesturesDisplayText,
           Properties.UICommandResource.DeployWebClientExpressTooltip,
           Properties.UICommandResource.DeployWebClientExpressDescription,
           typeof(UIGeneralCommands));

        public static GeneralCommand DeployWebClientExpress
        {
            get { return _DeployWebClientExpress; }
        }

        private static GeneralCommand _CreateWebClientApp = new GeneralCommand(
            Properties.UICommandResource.CreateWebClientAppName,
            Properties.UICommandResource.CreateWebClientAppText,
            Properties.UICommandResource.CreateWebClientAppGestures,
            Properties.UICommandResource.CreateWebClientAppGesturesDisplayText,
            Properties.UICommandResource.CreateWebClientAppTooltip,
            Properties.UICommandResource.CreateWebClientAppDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateWebClientApp
        {
            get { return _CreateWebClientApp; }
        }

        private static GeneralCommand _UploadIoTApp = new GeneralCommand(
            Properties.UICommandResource.UploadIoTAppName,
            Properties.UICommandResource.UploadIoTAppText,
            Properties.UICommandResource.UploadIoTAppGestures,
            Properties.UICommandResource.UploadIoTAppGesturesDisplayText,
            Properties.UICommandResource.UploadIoTAppTooltip,
            Properties.UICommandResource.UploadIoTAppDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand UploadIoTApp
        {
            get { return _UploadIoTApp; }
        }

        private static GeneralCommand _StartupRuntime = new GeneralCommand(
            Properties.UICommandResource.StartupRuntimeName,
            Properties.UICommandResource.StartupRuntimeText,
            Properties.UICommandResource.StartupRuntimeGestures,
            Properties.UICommandResource.StartupRuntimeGesturesDisplayText,
            Properties.UICommandResource.StartupRuntimeTooltip,
            Properties.UICommandResource.StartupRuntimeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StartupRuntime
        {
            get { return _StartupRuntime; }
        }

        private static GeneralCommand _StopRuntime = new GeneralCommand(
            Properties.UICommandResource.StopRuntimeName,
            Properties.UICommandResource.StopRuntimeText,
            Properties.UICommandResource.StopRuntimeGestures,
            Properties.UICommandResource.StopRuntimeGesturesDisplayText,
            Properties.UICommandResource.StopRuntimeTooltip,
            Properties.UICommandResource.StopRuntimeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StopRuntime
        {
            get { return _StopRuntime; }
        }

        private static GeneralCommand _OpenServicesPanel = new GeneralCommand(
            Properties.UICommandResource.OpenServicesPanelName,
            Properties.UICommandResource.OpenServicesPanelText,
            Properties.UICommandResource.OpenServicesPanelGestures,
            Properties.UICommandResource.OpenServicesPanelGesturesDisplayText,
            Properties.UICommandResource.OpenServicesPanelTooltip,
            Properties.UICommandResource.OpenServicesPanelDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand OpenServicesPanel
        {
            get { return _OpenServicesPanel; }
        }

        private static GeneralCommand _AddStringId = new GeneralCommand(
            TranslatableMenu.Properties.Resources.AddStringIdName,
            TranslatableMenu.Properties.Resources.AddStringIdText,
            Properties.UICommandResource.AddStringIdGestures,
            Properties.UICommandResource.AddStringIdGesturesDisplayText,
            Properties.UICommandResource.AddStringIdTooltip,
            Properties.UICommandResource.AddStringIdDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddStringId
        {
            get { return _AddStringId; }
        }

        private static GeneralCommand _AddPasswordProject = new GeneralCommand(
            Properties.UICommandResource.AddPasswordProjectName,
            Properties.UICommandResource.AddPasswordProjectText,
            Properties.UICommandResource.AddPasswordProjectGestures,
            Properties.UICommandResource.AddPasswordProjectGesturesDisplayText,
            Properties.UICommandResource.AddPasswordProjectTooltip,
            Properties.UICommandResource.AddPasswordProjectDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddPasswordProject
        {
            get { return _AddPasswordProject; }
        }

        private static GeneralCommand _EditAppNameSettings = new GeneralCommand(
            Properties.UICommandResource.EditAppNameSettingsName,
            Properties.UICommandResource.EditAppNameSettingsText,
            Properties.UICommandResource.EditAppNameSettingsGestures,
            Properties.UICommandResource.EditAppNameSettingsGesturesDisplayText,
            Properties.UICommandResource.EditAppNameSettingsTooltip,
            Properties.UICommandResource.EditAppNameSettingsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EditAppNameSettings
        {
            get { return _EditAppNameSettings; }
        }

        private static GeneralCommand _AddGeoLocation = new GeneralCommand(
            Properties.UICommandResource.EditGeoLocationName,
            Properties.UICommandResource.EditGeoLocationText,
            Properties.UICommandResource.EditGeoLocationGestures,
            Properties.UICommandResource.EditGeoLocationGesturesDisplayText,
            Properties.UICommandResource.EditGeoLocationTooltip,
            Properties.UICommandResource.EditGeoLocationDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddGeoLocation
        {
            get { return _AddGeoLocation; }
        }

        private static GeneralCommand _CreateQRCode = new GeneralCommand(
            Properties.UICommandResource.CreateQRCodeName,
            Properties.UICommandResource.CreateQRCodeText,
            Properties.UICommandResource.CreateQRCodeGestures,
            Properties.UICommandResource.CreateQRCodeGesturesDisplayText,
            Properties.UICommandResource.CreateQRCodeTooltip,
            Properties.UICommandResource.CreateQRCodeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateQRCode
        {
            get { return _CreateQRCode; }
        }

        private static GeneralCommand _CreateRuntimeShortcut = new GeneralCommand(
            Properties.UICommandResource.CreateRuntimeShortcutName,
            Properties.UICommandResource.CreateRuntimeShortcutText,
            Properties.UICommandResource.CreateRuntimeShortcutGestures,
            Properties.UICommandResource.CreateRuntimeShortcutDisplayText,
            Properties.UICommandResource.CreateRuntimeShortcutTooltip,
            Properties.UICommandResource.CreateRuntimeShortcutDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateRuntimeShortcut
        {
            get { return _CreateRuntimeShortcut; }
        }

        /*
        private static GeneralCommand _EditAppNameChildProjectSettings = new GeneralCommand(
            Properties.UICommandResource.EditAppNameChildProjectSettingsName,
            Properties.UICommandResource.EditAppNameChildProjectSettingsText,
            Properties.UICommandResource.EditAppNameChildProjectSettingsGestures,
            Properties.UICommandResource.EditAppNameChildProjectSettingsGesturesDisplayText,
            Properties.UICommandResource.EditAppNameChildProjectSettingsTooltip,
            Properties.UICommandResource.EditAppNameChildProjectSettingsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EditAppNameChildProjectSettings
        {
            get { return _EditAppNameChildProjectSettings; }
        }
        */

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

        private static GeneralCommand _AddNewScreenFromTemplate = new GeneralCommand(
             Properties.UICommandResource.AddNewScreenFromTemplateName,
             Properties.UICommandResource.AddNewScreenFromTemplateText,
             Properties.UICommandResource.AddNewScreenFromTemplateGestures,
             Properties.UICommandResource.AddNewScreenFromTemplateGesturesDisplayText,
             Properties.UICommandResource.AddNewScreenFromTemplateTooltip,
             Properties.UICommandResource.AddNewScreenFromTemplateDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand AddNewScreenFromTemplate
        {
            get { return _AddNewScreenFromTemplate; }
        }
    }
}