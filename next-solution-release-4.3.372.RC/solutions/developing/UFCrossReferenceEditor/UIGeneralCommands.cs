using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace UFCrossReferenceEditor
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

        private static GeneralCommand _UpdateCRDoc = new GeneralCommand(
            Properties.UICommandResource.UpdateCRDocName,
            Properties.UICommandResource.UpdateCRDocText,
            Properties.UICommandResource.UpdateCRDocGestures,
            Properties.UICommandResource.UpdateCRDocGesturesDisplayText,
            Properties.UICommandResource.UpdateCRDocTooltip,
            Properties.UICommandResource.UpdateCRDocDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand UpdateCRDoc
        {
            get { return _UpdateCRDoc; }
        }

        private static GeneralCommand _AddStringId = new GeneralCommand(
            Properties.UICommandResource.AddStringIdName,
            Properties.UICommandResource.AddStringIdText,
            Properties.UICommandResource.AddStringIdGestures,
            Properties.UICommandResource.AddStringIdGesturesDisplayText,
            Properties.UICommandResource.AddStringIdTooltip,
            Properties.UICommandResource.AddStringIdDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddStringId
        {
            get { return _AddStringId; }
        }

        private static GeneralCommand _RemoveStringId = new GeneralCommand(
            Properties.UICommandResource.RemoveStringIdName,
            Properties.UICommandResource.RemoveStringIdText,
            Properties.UICommandResource.RemoveStringIdGestures,
            Properties.UICommandResource.RemoveStringIdGesturesDisplayText,
            Properties.UICommandResource.RemoveStringIdTooltip,
            Properties.UICommandResource.RemoveStringIdDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand RemoveStringId
        {
            get { return _RemoveStringId; }
        }

        private static GeneralCommand _RemoveTags = new GeneralCommand(
            Properties.UICommandResource.RemoveTagsName,
            Properties.UICommandResource.RemoveTagsText,
            Properties.UICommandResource.RemoveTagsGestures,
            Properties.UICommandResource.RemoveTagsGesturesDisplayText,
            Properties.UICommandResource.RemoveTagsTooltip,
            Properties.UICommandResource.RemoveTagsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand RemoveTags
        {
            get { return _RemoveTags; }
        }

        private static GeneralCommand _ClearCRDoc = new GeneralCommand(
            Properties.UICommandResource.ClearCRDocName,
            Properties.UICommandResource.ClearCRDocText,
            Properties.UICommandResource.ClearCRDocGestures,
            Properties.UICommandResource.ClearCRDocGesturesDisplayText,
            Properties.UICommandResource.ClearCRDocTooltip,
            Properties.UICommandResource.ClearCRDocDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ClearCRDoc
        {
            get { return _ClearCRDoc; }
        }

        private static GeneralCommand _RenameReferences = new GeneralCommand(
            Properties.UICommandResource.RenameReferencesName,
            Properties.UICommandResource.RenameReferencesText,
            Properties.UICommandResource.RenameReferencesGestures,
            Properties.UICommandResource.RenameReferencesGesturesDisplayText,
            Properties.UICommandResource.RenameReferencesTooltip,
            Properties.UICommandResource.RenameReferencesDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand RenameReferences
        {
            get { return _RenameReferences; }
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