using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace ClientEditor
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

    }
}