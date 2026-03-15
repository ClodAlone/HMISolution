using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace UFRecipeEditor
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

        private static GeneralCommand _AddNewGroup = new GeneralCommand(
            Properties.UICommandResource.AddNewGroupName,
            Properties.UICommandResource.AddNewGroupText,
            Properties.UICommandResource.AddNewGroupGestures,
            Properties.UICommandResource.AddNewGroupGesturesDisplayText,
            Properties.UICommandResource.AddNewGroupTooltip,
            Properties.UICommandResource.AddNewGroupDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewGroup
        {
            get { return _AddNewGroup; }
        }

        private static GeneralCommand _AddNewDataValue = new GeneralCommand(
            Properties.UICommandResource.AddNewDataValueName,
            Properties.UICommandResource.AddNewDataValueText,
            Properties.UICommandResource.AddNewDataValueGestures,
            Properties.UICommandResource.AddNewDataValueGesturesDisplayText,
            Properties.UICommandResource.AddNewDataValueTooltip,
            Properties.UICommandResource.AddNewDataValueDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewDataValue
        {
            get { return _AddNewDataValue; }
        }

        private static GeneralCommand _MoveTagUp = new GeneralCommand(
            Properties.UICommandResource.MoveTagUpName,
            Properties.UICommandResource.MoveTagUpText,
            Properties.UICommandResource.MoveTagUpGestures,
            Properties.UICommandResource.MoveTagUpGesturesDisplayText,
            Properties.UICommandResource.MoveTagUpTooltip,
            Properties.UICommandResource.MoveTagUpDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveTagUp
        {
            get { return _MoveTagUp; }
        }
        private static GeneralCommand _MoveTagDown = new GeneralCommand(
            Properties.UICommandResource.MoveTagDownName,
            Properties.UICommandResource.MoveTagDownText,
            Properties.UICommandResource.MoveTagDownGestures,
            Properties.UICommandResource.MoveTagDownGesturesDisplayText,
            Properties.UICommandResource.MoveTagDownTooltip,
            Properties.UICommandResource.MoveTagDownDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveTagDown
        {
            get { return _MoveTagDown; }
        }

        private static GeneralCommand _ResetRecipeLayout = new GeneralCommand(
            Properties.UICommandResource.ResetRecipeLayoutName,
            Properties.UICommandResource.ResetRecipeLayoutText,
            Properties.UICommandResource.ResetRecipeLayoutGestures,
            Properties.UICommandResource.ResetRecipeLayoutDisplayText,
            Properties.UICommandResource.ResetRecipeLayoutTooltip,
            Properties.UICommandResource.ResetRecipeLayoutDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ResetRecipeLayout
        {
            get { return _ResetRecipeLayout; }
        }

        private static GeneralCommand _CreateRecipeDatabase = new GeneralCommand(
            Properties.UICommandResource.CreateRecipeDatabaseName,
            Properties.UICommandResource.CreateRecipeDatabaseText,
            Properties.UICommandResource.CreateRecipeDatabaseGestures,
            Properties.UICommandResource.CreateRecipeDatabaseDisplayText,
            Properties.UICommandResource.CreateRecipeDatabaseTooltip,
            Properties.UICommandResource.CreateRecipeDatabaseDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateRecipeDatabase
        {
            get { return _CreateRecipeDatabase; }
        }

        private static GeneralCommand _TestRecipeEditor = new GeneralCommand(
            Properties.UICommandResource.TestRecipeEditorName,
            Properties.UICommandResource.TestRecipeEditorText,
            Properties.UICommandResource.TestRecipeEditorGestures,
            Properties.UICommandResource.TestRecipeEditorDisplayText,
            Properties.UICommandResource.TestRecipeEditorTooltip,
            Properties.UICommandResource.TestRecipeEditorDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand TestRecipeEditor
        {
            get { return _TestRecipeEditor; }
        }

        private static GeneralCommand _ImportExportRecipe = new GeneralCommand(
            Properties.UICommandResource.ImportExportRecipeName,
            Properties.UICommandResource.ImportExportRecipeText,
            Properties.UICommandResource.ImportExportRecipeGestures,
            Properties.UICommandResource.ImportExportRecipeDisplayText,
            Properties.UICommandResource.ImportExportRecipeTooltip,
            Properties.UICommandResource.ImportExportRecipeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ImportExportRecipe
        {
            get { return _ImportExportRecipe; }
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

        #region Recipe OPC UA Editor Commands
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
        #endregion
    }
}