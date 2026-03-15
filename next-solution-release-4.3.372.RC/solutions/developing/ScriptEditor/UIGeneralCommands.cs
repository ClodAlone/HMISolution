using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace ScriptManager
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

        private static GeneralCommand _DebugStepInto = new GeneralCommand(
            Properties.UICommandResource.DebugStepIntoName,
            Properties.UICommandResource.DebugStepIntoText,
            Properties.UICommandResource.DebugStepIntoGestures,
            Properties.UICommandResource.DebugStepIntoGesturesDisplayText,
            Properties.UICommandResource.DebugStepIntoTooltip,
            Properties.UICommandResource.DebugStepIntoDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand DebugStepInto
        {
            get { return _DebugStepInto; }
        }

        private static GeneralCommand _EditReferences = new GeneralCommand(
            Properties.UICommandResource.EditReferencesName,
            Properties.UICommandResource.EditReferencesText,
            Properties.UICommandResource.EditReferencesGestures,
            Properties.UICommandResource.EditReferencesGesturesDisplayText,
            Properties.UICommandResource.EditReferencesTooltip,
            Properties.UICommandResource.EditReferencesDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EditReferences
        {
            get { return _EditReferences; }
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