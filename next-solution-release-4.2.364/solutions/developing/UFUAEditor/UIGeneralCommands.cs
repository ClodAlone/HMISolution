using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace UFUAEditor
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
        //private static GeneralCommand _AddNewLocalTag = new GeneralCommand(
        //Properties.UICommandResource.AddNewLocalTagName,
        //Properties.UICommandResource.AddNewLocalTagText,
        //Properties.UICommandResource.AddNewLocalTagGestures,
        //Properties.UICommandResource.AddNewLocalTagGesturesDisplayText,
        //Properties.UICommandResource.AddNewLocalTagTooltip,
        //Properties.UICommandResource.AddNewLocalTagDescription,
        //typeof(UIGeneralCommands));

        //public static GeneralCommand AddNewLocalTag
        //{
        //    get { return _AddNewLocalTag; }
        //}

        //private static GeneralCommand _AddNewLocalFolder = new GeneralCommand(
        //    Properties.UICommandResource.AddNewLocalFolderName,
        //    Properties.UICommandResource.AddNewLocalFolderText,
        //    Properties.UICommandResource.AddNewLocalFolderGestures,
        //    Properties.UICommandResource.AddNewLocalFolderGesturesDisplayText,
        //    Properties.UICommandResource.AddNewLocalFolderTooltip,
        //    Properties.UICommandResource.AddNewLocalFolderDescription,
        //    typeof(UIGeneralCommands));

        //public static GeneralCommand AddNewLocalFolder
        //{
        //    get { return _AddNewLocalFolder; }
        //}

        private static GeneralCommand _AddNewTag = new GeneralCommand(
            Properties.UICommandResource.AddNewTagName,
            Properties.UICommandResource.AddNewTagText,
            Properties.UICommandResource.AddNewTagGestures,
            Properties.UICommandResource.AddNewTagGesturesDisplayText,
            Properties.UICommandResource.AddNewTagTooltip,
            Properties.UICommandResource.AddNewTagDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewTag
        {
            get { return _AddNewTag; }
        }

        //private static GeneralCommand _AddNewGeneralTag = new GeneralCommand(
        //           Properties.UICommandResource.AddNewGeneralTagName,
        //           Properties.UICommandResource.AddNewGeneralTagText,
        //           Properties.UICommandResource.AddNewGeneralTagGestures,
        //           Properties.UICommandResource.AddNewGeneralTagGesturesDisplayText,
        //           Properties.UICommandResource.AddNewGeneralTagTooltip,
        //           Properties.UICommandResource.AddNewGeneralTagDescription,
        //           typeof(UIGeneralCommands));

        //public static GeneralCommand AddNewGeneralTag
        //{
        //    get { return _AddNewGeneralTag; }
        //}

        //private static GeneralCommand _AddNewGeneralFolder = new GeneralCommand(
        //           Properties.UICommandResource.AddNewGeneralFolderName,
        //           Properties.UICommandResource.AddNewGeneralFolderText,
        //           Properties.UICommandResource.AddNewGeneralFolderGestures,
        //           Properties.UICommandResource.AddNewGeneralFolderGesturesDisplayText,
        //           Properties.UICommandResource.AddNewGeneralFolderTooltip,
        //           Properties.UICommandResource.AddNewGeneralFolderDescription,
        //           typeof(UIGeneralCommands));

        //public static GeneralCommand AddNewGeneralFolder
        //{
        //    get { return _AddNewGeneralFolder; }
        //}

        private static GeneralCommand _ImportExportTags = new GeneralCommand(
        Properties.UICommandResource.ImportExportTagsName,
        Properties.UICommandResource.ImportExportTagsText,
        Properties.UICommandResource.ImportExportTagsGestures,
        Properties.UICommandResource.ImportExportTagsGesturesDisplayText,
        Properties.UICommandResource.ImportExportTagsTooltip,
        Properties.UICommandResource.ImportExportTagsDescription,
        typeof(UIGeneralCommands));

        public static GeneralCommand ImportExportTags
        {
            get { return _ImportExportTags; }
        }

        private static GeneralCommand _ImportExportAddressSpace = new GeneralCommand(
        Properties.UICommandResource.ImportExportAddressSpaceName,
        Properties.UICommandResource.ImportExportAddressSpaceText,
        Properties.UICommandResource.ImportExportAddressSpaceGestures,
        Properties.UICommandResource.ImportExportAddressSpaceGesturesDisplayText,
        Properties.UICommandResource.ImportExportAddressSpaceTooltip,
        Properties.UICommandResource.ImportExportAddressSpaceDescription,
        typeof(UIGeneralCommands));

        public static GeneralCommand ImportExportAddressSpace
        {
            get { return _ImportExportAddressSpace; }
        }

        private static GeneralCommand _ImportExportAlarms = new GeneralCommand(
        Properties.UICommandResource.ImportExportAlarmsName,
        Properties.UICommandResource.ImportExportAlarmsText,
        Properties.UICommandResource.ImportExportAlarmsGestures,
        Properties.UICommandResource.ImportExportAlarmsGesturesDisplayText,
        Properties.UICommandResource.ImportExportAlarmsTooltip,
        Properties.UICommandResource.ImportExportAlarmsDescription,
        typeof(UIGeneralCommands));

        public static GeneralCommand ImportExportAlarms
        {
            get { return _ImportExportAlarms; }
        }

        private static GeneralCommand _ImportExportHistoricals = new GeneralCommand(
        Properties.UICommandResource.ImportExportHistoricalsName,
        Properties.UICommandResource.ImportExportHistoricalsText,
        Properties.UICommandResource.ImportExportHistoricalsGestures,
        Properties.UICommandResource.ImportExportHistoricalsGesturesDisplayText,
        Properties.UICommandResource.ImportExportHistoricalsTooltip,
        Properties.UICommandResource.ImportExportHistoricalsDescription,
        typeof(UIGeneralCommands));

        public static GeneralCommand ImportExportHistoricals
        {
            get { return _ImportExportHistoricals; }
        }

        private static GeneralCommand _ImportExportDataloggers = new GeneralCommand(
         Properties.UICommandResource.ImportExportDataloggersName,
         Properties.UICommandResource.ImportExportDataloggersText,
         Properties.UICommandResource.ImportExportDataloggersGestures,
         Properties.UICommandResource.ImportExportDataloggersGesturesDisplayText,
         Properties.UICommandResource.ImportExportDataloggersTooltip,
         Properties.UICommandResource.ImportExportDataloggersDescription,
         typeof(UIGeneralCommands));

        public static GeneralCommand ImportExportDataloggers
        {
            get { return _ImportExportDataloggers; }
        }

        private static GeneralCommand _ImportExportEUnits = new GeneralCommand(
        Properties.UICommandResource.ImportExportEUnitsName,
        Properties.UICommandResource.ImportExportEUnitsText,
        Properties.UICommandResource.ImportExportEUnitsGestures,
        Properties.UICommandResource.ImportExportEUnitsGesturesDisplayText,
        Properties.UICommandResource.ImportExportEUnitsTooltip,
        Properties.UICommandResource.ImportExportEUnitsDescription,
        typeof(UIGeneralCommands));

        public static GeneralCommand ImportExportEUnits
        {
            get { return _ImportExportEUnits; }
        }

        private static GeneralCommand _ImportExportPrototypes = new GeneralCommand(
        Properties.UICommandResource.ImportExportPrototypesName,
        Properties.UICommandResource.ImportExportPrototypesText,
        Properties.UICommandResource.ImportExportPrototypesGestures,
        Properties.UICommandResource.ImportExportPrototypesGesturesDisplayText,
        Properties.UICommandResource.ImportExportPrototypesTooltip,
        Properties.UICommandResource.ImportExportPrototypesDescription,
        typeof(UIGeneralCommands));

        public static GeneralCommand ImportExportPrototypes
        {
            get { return _ImportExportPrototypes; }
        }


        private static GeneralCommand _AddNewMember = new GeneralCommand(
            Properties.UICommandResource.AddNewMemberName,
            Properties.UICommandResource.AddNewMemberText,
            Properties.UICommandResource.AddNewMemberGestures,
            Properties.UICommandResource.AddNewMemberGesturesDisplayText,
            Properties.UICommandResource.AddNewMemberTooltip,
            Properties.UICommandResource.AddNewMemberDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewMember
        {
            get { return _AddNewMember; }
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

        private static GeneralCommand _AddNewPrototype = new GeneralCommand(
            Properties.UICommandResource.AddNewPrototypeName,
            Properties.UICommandResource.AddNewPrototypeText,
            Properties.UICommandResource.AddNewPrototypeGestures,
            Properties.UICommandResource.AddNewPrototypeGesturesDisplayText,
            Properties.UICommandResource.AddNewPrototypeTooltip,
            Properties.UICommandResource.AddNewPrototypeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewPrototype
        {
            get { return _AddNewPrototype; }
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

        private static GeneralCommand _CertificateChecker = new GeneralCommand(
            Properties.UICommandResource.CertificateCheckerName,
            Properties.UICommandResource.CertificateCheckerText,
            Properties.UICommandResource.CertificateCheckerGestures,
            Properties.UICommandResource.CertificateCheckerGesturesDisplayText,
            Properties.UICommandResource.CertificateCheckerTooltip,
            Properties.UICommandResource.CertificateCheckerDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CertificateChecker
        {
            get { return _CertificateChecker; }
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

        private static GeneralCommand _AddNewDriver = new GeneralCommand(
            Properties.UICommandResource.AddNewDriverName,
            Properties.UICommandResource.AddNewDriverText,
            Properties.UICommandResource.AddNewDriverGestures,
            Properties.UICommandResource.AddNewDriverGesturesDisplayText,
            Properties.UICommandResource.AddNewDriverTooltip,
            Properties.UICommandResource.AddNewDriverDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewDriver
        {
            get { return _AddNewDriver; }
        }

        private static GeneralCommand _OpenDriverSettings = new GeneralCommand(
            Properties.UICommandResource.OpenDriverSettingsName,
            Properties.UICommandResource.OpenDriverSettingsText,
            Properties.UICommandResource.OpenDriverSettingsGestures,
            Properties.UICommandResource.OpenDriverSettingsGesturesDisplayText,
            Properties.UICommandResource.OpenDriverSettingsTooltip,
            Properties.UICommandResource.OpenDriverSettingsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand OpenDriverSettings
        {
            get { return _OpenDriverSettings; }
        }

        private static GeneralCommand _ImportTagsDriver = new GeneralCommand(
            Properties.UICommandResource.ImportTagsDriverName,
            Properties.UICommandResource.ImportTagsDriverText,
            Properties.UICommandResource.ImportTagsDriverGestures,
            Properties.UICommandResource.ImportTagsDriverGesturesDisplayText,
            Properties.UICommandResource.ImportTagsDriverTooltip,
            Properties.UICommandResource.ImportTagsDriverDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ImportTagsDriver
        {
            get { return _ImportTagsDriver; }
        }

        private static GeneralCommand _AddNewHistoricalSettings = new GeneralCommand(
            Properties.UICommandResource.AddNewHistoricalSettingsName,
            Properties.UICommandResource.AddNewHistoricalSettingsText,
            Properties.UICommandResource.AddNewHistoricalSettingsGestures,
            Properties.UICommandResource.AddNewHistoricalSettingsGesturesDisplayText,
            Properties.UICommandResource.AddNewHistoricalSettingsTooltip,
            Properties.UICommandResource.AddNewHistoricalSettingsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewHistoricalSettings
        {
            get { return _AddNewHistoricalSettings; }
        }

        private static GeneralCommand _AssignHistoricalSettings = new GeneralCommand(
            Properties.UICommandResource.AssignHistoricalSettingsName,
            Properties.UICommandResource.AssignHistoricalSettingsText,
            Properties.UICommandResource.AssignHistoricalSettingsGestures,
            Properties.UICommandResource.AssignHistoricalSettingsGesturesDisplayText,
            Properties.UICommandResource.AssignHistoricalSettingsTooltip,
            Properties.UICommandResource.AssignHistoricalSettingsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AssignHistoricalSettings
        {
            get { return _AssignHistoricalSettings; }
        }

        private static GeneralCommand _ManteinanceHistoricalSettings = new GeneralCommand(
            Properties.UICommandResource.ManteinanceHistoricalSettingsName,
            Properties.UICommandResource.ManteinanceHistoricalSettingsText,
            Properties.UICommandResource.ManteinanceHistoricalSettingsGestures,
            Properties.UICommandResource.ManteinanceHistoricalSettingsGesturesDisplayText,
            Properties.UICommandResource.ManteinanceHistoricalSettingsTooltip,
            Properties.UICommandResource.ManteinanceHistoricalSettingsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ManteinanceHistoricalSettings
        {
            get { return _ManteinanceHistoricalSettings; }
        }

        private static GeneralCommand _AddNewDataLoggerSettings = new GeneralCommand(
            Properties.UICommandResource.AddNewDataLoggerSettingsName,
            Properties.UICommandResource.AddNewDataLoggerSettingsText,
            Properties.UICommandResource.AddNewDataLoggerSettingsGestures,
            Properties.UICommandResource.AddNewDataLoggerSettingsGesturesDisplayText,
            Properties.UICommandResource.AddNewDataLoggerSettingsTooltip,
            Properties.UICommandResource.AddNewDataLoggerSettingsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewDataLoggerSettings
        {
            get { return _AddNewDataLoggerSettings; }
        }

        private static GeneralCommand _AddNewAlarmArea = new GeneralCommand(
            Properties.UICommandResource.AddNewAlarmAreaName,
            Properties.UICommandResource.AddNewAlarmAreaText,
            Properties.UICommandResource.AddNewAlarmAreaGestures,
            Properties.UICommandResource.AddNewAlarmAreaGesturesDisplayText,
            Properties.UICommandResource.AddNewAlarmAreaTooltip,
            Properties.UICommandResource.AddNewAlarmAreaDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewAlarmArea
        {
            get { return _AddNewAlarmArea; }
        }

        private static GeneralCommand _AddNewAlarmSource = new GeneralCommand(
            Properties.UICommandResource.AddNewAlarmSourceName,
            Properties.UICommandResource.AddNewAlarmSourceText,
            Properties.UICommandResource.AddNewAlarmSourceGestures,
            Properties.UICommandResource.AddNewAlarmSourceGesturesDisplayText,
            Properties.UICommandResource.AddNewAlarmSourceTooltip,
            Properties.UICommandResource.AddNewAlarmSourceDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewAlarmSource
        {
            get { return _AddNewAlarmSource; }
        }

        private static GeneralCommand _AddNewAlarmDefinition = new GeneralCommand(
            Properties.UICommandResource.AddNewAlarmDefinitionName,
            Properties.UICommandResource.AddNewAlarmDefinitionText,
            Properties.UICommandResource.AddNewAlarmDefinitionGestures,
            Properties.UICommandResource.AddNewAlarmDefinitionGesturesDisplayText,
            Properties.UICommandResource.AddNewAlarmDefinitionTooltip,
            Properties.UICommandResource.AddNewAlarmDefinitionDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewAlarmDefinition
        {
            get { return _AddNewAlarmDefinition; }
        }

        private static GeneralCommand _AddNewMessageDefinition = new GeneralCommand(
            Properties.UICommandResource.AddNewMessageDefinitionName,
            Properties.UICommandResource.AddNewMessageDefinitionText,
            Properties.UICommandResource.AddNewMessageDefinitionGestures,
            Properties.UICommandResource.AddNewMessageDefinitionGesturesDisplayText,
            Properties.UICommandResource.AddNewMessageDefinitionTooltip,
            Properties.UICommandResource.AddNewMessageDefinitionDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewMessageDefinition
        {
            get { return _AddNewMessageDefinition; }
        }

        private static GeneralCommand _AssignAlarmDefinition = new GeneralCommand(
            Properties.UICommandResource.AssignAlarmDefinitionName,
            Properties.UICommandResource.AssignAlarmDefinitionText,
            Properties.UICommandResource.AssignAlarmDefinitionGestures,
            Properties.UICommandResource.AssignAlarmDefinitionGesturesDisplayText,
            Properties.UICommandResource.AssignAlarmDefinitionTooltip,
            Properties.UICommandResource.AssignAlarmDefinitionDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AssignAlarmDefinition
        {
            get { return _AssignAlarmDefinition; }
        }

        private static GeneralCommand _AssignTag = new GeneralCommand(
            Properties.UICommandResource.AssignTagName,
            Properties.UICommandResource.AssignTagText,
            Properties.UICommandResource.AssignTagGestures,
            Properties.UICommandResource.AssignTagGesturesDisplayText,
            Properties.UICommandResource.AssignTagTooltip,
            Properties.UICommandResource.AssignTagDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AssignTag
        {
            get { return _AssignTag; }
        }

        private static GeneralCommand _AddNewEngineeringUnits = new GeneralCommand(
            Properties.UICommandResource.AddNewEngineeringUnitsName,
            Properties.UICommandResource.AddNewEngineeringUnitsText,
            Properties.UICommandResource.AddNewEngineeringUnitsGestures,
            Properties.UICommandResource.AddNewEngineeringUnitsGesturesDisplayText,
            Properties.UICommandResource.AddNewEngineeringUnitsTooltip,
            Properties.UICommandResource.AddNewEngineeringUnitsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewEngineeringUnits
        {
            get { return _AddNewEngineeringUnits; }
        }


        private static GeneralCommand _AddNewView = new GeneralCommand(
            Properties.UICommandResource.AddNewViewName,
            Properties.UICommandResource.AddNewViewText,
            Properties.UICommandResource.AddNewViewGestures,
            Properties.UICommandResource.AddNewViewGesturesDisplayText,
            Properties.UICommandResource.AddNewViewTooltip,
            Properties.UICommandResource.AddNewViewDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewView
        {
            get { return _AddNewView; }
        }

        private static GeneralCommand _AssociateView = new GeneralCommand(
            Properties.UICommandResource.AssociateViewName,
            Properties.UICommandResource.AssociateViewText,
            Properties.UICommandResource.AssociateViewGestures,
            Properties.UICommandResource.AssociateViewGesturesDisplayText,
            Properties.UICommandResource.AssociateViewTooltip,
            Properties.UICommandResource.AssociateViewDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AssociateView
        {
            get { return _AssociateView; }
        }

        private static GeneralCommand _AttachDebugger = new GeneralCommand(
            Properties.UICommandResource.AttachDebuggerName,
            Properties.UICommandResource.AttachDebuggerText,
            Properties.UICommandResource.AttachDebuggerGestures,
            Properties.UICommandResource.AttachDebuggerGesturesDisplayText,
            Properties.UICommandResource.AttachDebuggerTooltip,
            Properties.UICommandResource.AttachDebuggerDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AttachDebugger
        {
            get { return _AttachDebugger; }
        }

        private static GeneralCommand _MoveMemberUp = new GeneralCommand(
            Properties.UICommandResource.MoveMemberUpName,
            Properties.UICommandResource.MoveMemberUpText,
            Properties.UICommandResource.MoveMemberUpGestures,
            Properties.UICommandResource.MoveMemberUpGesturesDisplayText,
            Properties.UICommandResource.MoveMemberUpTooltip,
            Properties.UICommandResource.MoveMemberUpDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveMemberUp
        {
            get { return _MoveMemberUp; }
        }

        private static GeneralCommand _MoveMemberDown = new GeneralCommand(
            Properties.UICommandResource.MoveMemberDownName,
            Properties.UICommandResource.MoveMemberDownText,
            Properties.UICommandResource.MoveMemberDownGestures,
            Properties.UICommandResource.MoveMemberDownGesturesDisplayText,
            Properties.UICommandResource.MoveMemberDownTooltip,
            Properties.UICommandResource.MoveMemberDownDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveMemberDown
        {
            get { return _MoveMemberDown; }
        }

        private static GeneralCommand _SetMemberPosition = new GeneralCommand(
            Properties.UICommandResource.SetMemberPositionName,
            Properties.UICommandResource.SetMemberPositionText,
            Properties.UICommandResource.SetMemberPositionGestures,
            Properties.UICommandResource.SetMemberPositionGesturesDisplayText,
            Properties.UICommandResource.SetMemberPositionTooltip,
            Properties.UICommandResource.SetMemberPositionDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SetMemberPosition
        {
            get { return _SetMemberPosition; }
        }

        private static GeneralCommand _AddWholeStringId = new GeneralCommand(
            TranslatableMenu.Properties.Resources.AddStringIdName,
            TranslatableMenu.Properties.Resources.AddStringIdText,
            Properties.UICommandResource.AddStringIdGestures,
            Properties.UICommandResource.AddStringIdGesturesDisplayText,
            Properties.UICommandResource.AddStringIdTooltip,
            Properties.UICommandResource.AddStringIdDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddWholeStringId
        {
            get { return _AddWholeStringId; }
        }

        private static GeneralCommand _AddAddressSpaceStringId = new GeneralCommand(
            TranslatableMenu.Properties.Resources.AddStringIdName,
            TranslatableMenu.Properties.Resources.AddStringIdText,
            Properties.UICommandResource.AddStringIdGestures,
            Properties.UICommandResource.AddStringIdGesturesDisplayText,
            Properties.UICommandResource.AddStringIdTooltip,
            Properties.UICommandResource.AddStringIdDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddAddressSpaceStringId
        {
            get { return _AddAddressSpaceStringId; }
        }

        private static GeneralCommand _AddPrototypesStringId = new GeneralCommand(
            TranslatableMenu.Properties.Resources.AddStringIdName,
            TranslatableMenu.Properties.Resources.AddStringIdText,
            Properties.UICommandResource.AddStringIdGestures,
            Properties.UICommandResource.AddStringIdGesturesDisplayText,
            Properties.UICommandResource.AddStringIdTooltip,
            Properties.UICommandResource.AddStringIdDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddPrototypesStringId
        {
            get { return _AddPrototypesStringId; }
        }

        private static GeneralCommand _AggregateTables = new GeneralCommand(
            Properties.UICommandResource.AggregateTablesName,
            Properties.UICommandResource.AggregateTablesText,
            Properties.UICommandResource.AggregateTablesGestures,
            Properties.UICommandResource.AggregateTablesDisplayText,
            Properties.UICommandResource.AggregateTablesTooltip,
            Properties.UICommandResource.AggregateTablesDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AggregateTables
        {
            get { return _AggregateTables; }
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

        private static GeneralCommand _RemoveAlarmDefinitions = new GeneralCommand(
            Properties.UICommandResource.RemoveAlarmDefinitionsName,
            Properties.UICommandResource.RemoveAlarmDefinitionsText,
            Properties.UICommandResource.RemoveAlarmDefinitionsGestures,
            Properties.UICommandResource.RemoveAlarmDefinitionsGesturesDisplayText,
            Properties.UICommandResource.RemoveAlarmDefinitionsTooltip,
            Properties.UICommandResource.RemoveAlarmDefinitionsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand RemoveAlarmDefinitions
        {
            get { return _RemoveAlarmDefinitions; }
        }
    }
}