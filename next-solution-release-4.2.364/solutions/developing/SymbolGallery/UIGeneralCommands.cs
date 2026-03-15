using System;
using System.Windows.Input;
using Utilities;

namespace SymbolGallery
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
        private static GeneralCommand _zoomIn = new GeneralCommand(
                        Properties.Resources.ZoomIn,
                        Properties.Resources.ZoomIn,
                        Properties.Resources.ZoomIn,
                        Properties.Resources.ZoomIn,
                        Properties.Resources.ZoomIn,
                        Properties.Resources.ZoomIn,
                        typeof(UIGeneralCommands));

        public static GeneralCommand ZoomIn
        {
            get { return _zoomIn; }
        }

        private static GeneralCommand _zoomOut = new GeneralCommand(
                       Properties.Resources.ZoomOut,
                       Properties.Resources.ZoomOut,
                       Properties.Resources.ZoomOut,
                       Properties.Resources.ZoomOut,
                       Properties.Resources.ZoomOut,
                       Properties.Resources.ZoomOut,
                       typeof(UIGeneralCommands));

        public static GeneralCommand ZoomOut
        {
            get { return _zoomOut; }
        }
    }
}