using System;
using System.Collections.Generic;
using System.Windows.Input;
using Utilities;

namespace UFSolutionNext
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
        public static string FileMenuText
        {
            get
            {
                return Properties.UICommandResource.FileMenuText;
            }
        }
        public static string ViewMenuText
        {
            get
            {
                return Properties.UICommandResource.ViewMenuText;
            }
        }
        public static string StatusbarText
        {
            get
            {
                return Properties.UICommandResource.Statusbar;
            }
        }
        public static string MagnifierToolText
        {
            get
            {
                return Properties.UICommandResource.MagnifierToolText;
            }
        }

        private static GeneralCommand _ExitCommand = new GeneralCommand(
            Properties.UICommandResource.ExitCommandName,
            Properties.UICommandResource.ExitCommandText,
            Properties.UICommandResource.ExitCommandGestures,
            Properties.UICommandResource.ExitCommandGesturesDisplayText,
            Properties.UICommandResource.ExitCommandTooltip,
            Properties.UICommandResource.ExitCommandDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ExitCommand
        {
            get { return _ExitCommand; }
        }

        private static GeneralCommand _exit = new GeneralCommand(
            Properties.UICommandResource.ExitName,
            Properties.UICommandResource.ExitText,
            Properties.UICommandResource.ExitGestures,
            Properties.UICommandResource.ExitGesturesDisplayText,
            Properties.UICommandResource.ExitTooltip,
            Properties.UICommandResource.ExitDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand Exit
        {
            get { return _exit; }
        }

        private static GeneralCommand _fullScreen = new GeneralCommand(
            Properties.UICommandResource.FullScreenName,
            Properties.UICommandResource.FullScreenText,
            Properties.UICommandResource.FullScreenGestures,
            Properties.UICommandResource.FullScreenGesturesDisplayText,
            Properties.UICommandResource.FullScreenTooltip,
            Properties.UICommandResource.FullScreenDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand FullScreen
        {
            get { return _fullScreen; }
        }

        private static GeneralCommand _LanguagePreferences = new GeneralCommand(
            Properties.UICommandResource.LanguagePreferencesName,
            Properties.UICommandResource.LanguagePreferencesText,
            Properties.UICommandResource.LanguagePreferencesGestures,
            Properties.UICommandResource.LanguagePreferencesGesturesDisplayText,
            Properties.UICommandResource.LanguagePreferencesTooltip,
            Properties.UICommandResource.LanguagePreferencesDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand LanguagePreferences
        {
            get { return _LanguagePreferences; }
        }

        private static GeneralCommand _EasyMode = new GeneralCommand(
            Properties.UICommandResource.EasyModeName,
            Properties.UICommandResource.EasyModeText,
            Properties.UICommandResource.EasyModeGestures,
            Properties.UICommandResource.EasyModeGesturesDisplayText,
            Properties.UICommandResource.EasyModeTooltip,
            Properties.UICommandResource.EasyModeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EasyMode
        {
            get { return _EasyMode; }
        }

        private static GeneralCommand _CheckCertificates = new GeneralCommand(
           Properties.UICommandResource.CheckCertificatesName,
           Properties.UICommandResource.CheckCertificatesText,
           Properties.UICommandResource.CheckCertificatesGestures,
           Properties.UICommandResource.CheckCertificatesGesturesDisplayText,
           Properties.UICommandResource.CheckCertificatesTooltip,
           Properties.UICommandResource.CheckCertificatesDescription,
           typeof(UIGeneralCommands));

        public static GeneralCommand CheckCertificates
        {
            get { return _CheckCertificates; }
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

        private static GeneralCommand _ChangeApplicationTheme = new GeneralCommand(
            Properties.UICommandResource.ChangeApplicationThemeName,
            Properties.UICommandResource.ChangeApplicationThemeText,
            Properties.UICommandResource.ChangeApplicationThemeGestures,
            Properties.UICommandResource.ChangeApplicationThemeGesturesDisplayText,
            Properties.UICommandResource.ChangeApplicationThemeTooltip,
            Properties.UICommandResource.ChangeApplicationThemeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ChangeApplicationTheme
        {
            get { return _ChangeApplicationTheme; }
        }

        private static GeneralCommand _CheckForUpdates = new GeneralCommand(
            Properties.UICommandResource.CheckForUpdatesName,
            Properties.UICommandResource.CheckForUpdatesText,
            Properties.UICommandResource.CheckForUpdatesGestures,
            Properties.UICommandResource.CheckForUpdatesGesturesDisplayText,
            Properties.UICommandResource.CheckForUpdatesTooltip,
            Properties.UICommandResource.CheckForUpdatesDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CheckForUpdates
        {
            get { return _CheckForUpdates; }
        }
    }
}