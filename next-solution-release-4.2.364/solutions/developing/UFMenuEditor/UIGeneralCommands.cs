using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace UFMenuEditor
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
        private static GeneralCommand _AddNewMenuItem = new GeneralCommand(
              Properties.UICommandResource.AddNewMenuItemName,
              Properties.UICommandResource.AddNewMenuItemText,
              Properties.UICommandResource.AddNewMenuItemGestures,
              Properties.UICommandResource.AddNewMenuItemGesturesDisplayText,
              Properties.UICommandResource.AddNewMenuItemTooltip,
              Properties.UICommandResource.AddNewMenuItemDescription,
              typeof(UIGeneralCommands));

        public static GeneralCommand AddNewMenuItem
        {
            get { return _AddNewMenuItem; }
        }

        private static GeneralCommand _MoveItemUp = new GeneralCommand(
            Properties.UICommandResource.MoveItemUpName,
            Properties.UICommandResource.MoveItemUpText,
            Properties.UICommandResource.MoveItemUpGestures,
            Properties.UICommandResource.MoveItemUpGesturesDisplayText,
            Properties.UICommandResource.MoveItemUpTooltip,
            Properties.UICommandResource.MoveItemUpDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveItemUp
        {
            get { return _MoveItemUp; }
        }
        private static GeneralCommand _MoveItemDown = new GeneralCommand(
            Properties.UICommandResource.MoveItemDownName,
            Properties.UICommandResource.MoveItemDownText,
            Properties.UICommandResource.MoveItemDownGestures,
            Properties.UICommandResource.MoveItemDownGesturesDisplayText,
            Properties.UICommandResource.MoveItemDownTooltip,
            Properties.UICommandResource.MoveItemDownDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveItemDown
        {
            get { return _MoveItemDown; }
        }

        private static GeneralCommand _TestMenu = new GeneralCommand(
            Properties.UICommandResource.TestMenuName,
            Properties.UICommandResource.TestMenuText,
            Properties.UICommandResource.TestMenuGestures,
            Properties.UICommandResource.TestMenuGesturesDisplayText,
            Properties.UICommandResource.TestMenuTooltip,
            Properties.UICommandResource.TestMenuDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand TestMenu
        {
            get { return _TestMenu; }
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
    }
}
