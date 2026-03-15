using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace MultipleDocumentManager
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

        private static GeneralCommand _MyCommand = new GeneralCommand(
            Properties.UICommandResource.MyCommandName,
            Properties.UICommandResource.MyCommandText,
            Properties.UICommandResource.MyCommandGestures,
            Properties.UICommandResource.MyCommandGesturesDisplayText,
            Properties.UICommandResource.MyCommandTooltip,
            Properties.UICommandResource.MyCommandDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MyCommand
        {
            get { return _MyCommand; }
        }
    }
}