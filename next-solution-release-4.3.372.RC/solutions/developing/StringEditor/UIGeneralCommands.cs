using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace StringManager
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
        private static GeneralCommand _RemoveLocale = new GeneralCommand(
            Properties.UICommandResource.RemoveLocaleName,
            Properties.UICommandResource.RemoveLocaleText,
            Properties.UICommandResource.RemoveLocaleGestures,
            Properties.UICommandResource.RemoveLocaleGesturesDisplayText,
            Properties.UICommandResource.RemoveLocaleTooltip,
            Properties.UICommandResource.RemoveLocaleDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand RemoveLocale
        {
            get { return _RemoveLocale; }
        }

        private static GeneralCommand _AddNewLocale = new GeneralCommand(
            Properties.UICommandResource.AddNewLocaleName,
            Properties.UICommandResource.AddNewLocaleText,
            Properties.UICommandResource.AddNewLocaleGestures,
            Properties.UICommandResource.AddNewLocaleGesturesDisplayText,
            Properties.UICommandResource.AddNewLocaleTooltip,
            Properties.UICommandResource.AddNewLocaleDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewLocale
        {
            get { return _AddNewLocale; }
        }

        private static GeneralCommand _AddNewLocaleText = new GeneralCommand(
            Properties.UICommandResource.AddNewLocaleTextName,
            Properties.UICommandResource.AddNewLocaleTextText,
            Properties.UICommandResource.AddNewLocaleTextGestures,
            Properties.UICommandResource.AddNewLocaleTextGesturesDisplayText,
            Properties.UICommandResource.AddNewLocaleTextTooltip,
            Properties.UICommandResource.AddNewLocaleTextDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewLocaleText
        {
            get { return _AddNewLocaleText; }
        }

        private static GeneralCommand _EnableAutoTranslate = new GeneralCommand(
            Properties.UICommandResource.EnableAutoTranslateName,
            Properties.UICommandResource.EnableAutoTranslateText,
            Properties.UICommandResource.EnableAutoTranslateGestures,
            Properties.UICommandResource.EnableAutoTranslateGesturesDisplayText,
            Properties.UICommandResource.EnableAutoTranslateTooltip,
            Properties.UICommandResource.EnableAutoTranslateDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EnableAutoTranslate
        {
            get { return _EnableAutoTranslate; }
        }

        private static GeneralCommand _TranslateColumns = new GeneralCommand(
            Properties.UICommandResource.TranslateColumnsName,
            Properties.UICommandResource.TranslateColumnsText,
            Properties.UICommandResource.TranslateColumnsGestures,
            Properties.UICommandResource.TranslateColumnsGesturesDisplayText,
            Properties.UICommandResource.TranslateColumnsTooltip,
            Properties.UICommandResource.TranslateColumnsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand TranslateColumns
        {
            get { return _TranslateColumns; }
        }

        private static GeneralCommand _ImportStrings = new GeneralCommand(
            Properties.UICommandResource.ImportStringsName,
            Properties.UICommandResource.ImportStringsText,
            Properties.UICommandResource.ImportStringsGestures,
            Properties.UICommandResource.ImportStringsGesturesDisplayText,
            Properties.UICommandResource.ImportStringsTooltip,
            Properties.UICommandResource.ImportStringsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ImportStrings
        {
            get { return _ImportStrings; }
        }

        private static GeneralCommand _ExportStrings = new GeneralCommand(
            Properties.UICommandResource.ExportStringsName,
            Properties.UICommandResource.ExportStringsText,
            Properties.UICommandResource.ExportStringsGestures,
            Properties.UICommandResource.ExportStringsGesturesDisplayText,
            Properties.UICommandResource.ExportStringsTooltip,
            Properties.UICommandResource.ExportStringsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ExportStrings
        {
            get { return _ExportStrings; }
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
    }
}