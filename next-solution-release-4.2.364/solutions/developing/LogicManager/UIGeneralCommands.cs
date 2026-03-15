using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace LogicManager
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
        private static GeneralCommand _StartTestCommand = new GeneralCommand(
            Properties.UICommandResource.StartTestCommandName,
            Properties.UICommandResource.StartTestCommandText,
            Properties.UICommandResource.StartTestCommandGestures,
            Properties.UICommandResource.StartTestCommandGesturesDisplayText,
            Properties.UICommandResource.StartTestCommandTooltip,
            Properties.UICommandResource.StartTestCommandDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StartTestCommand
        {
            get { return _StartTestCommand; }
        }

        private static GeneralCommand _StopTestCommand = new GeneralCommand(
            Properties.UICommandResource.StopTestCommandName,
            Properties.UICommandResource.StopTestCommandText,
            Properties.UICommandResource.StopTestCommandGestures,
            Properties.UICommandResource.StopTestCommandGesturesDisplayText,
            Properties.UICommandResource.StopTestCommandTooltip,
            Properties.UICommandResource.StopTestCommandDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StopTestCommand
        {
            get { return _StopTestCommand; }
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