using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace UFUserEditor
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

        private static GeneralCommand _AddNewUser = new GeneralCommand(
            Properties.UICommandResource.AddNewUserName,
            Properties.UICommandResource.AddNewUserText,
            Properties.UICommandResource.AddNewUserGestures,
            Properties.UICommandResource.AddNewUserGesturesDisplayText,
            Properties.UICommandResource.AddNewUserTooltip,
            Properties.UICommandResource.AddNewUserDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewUser
        {
            get { return _AddNewUser; }
        }

        private static GeneralCommand _AddNewRole = new GeneralCommand(
            Properties.UICommandResource.AddNewRoleName,
            Properties.UICommandResource.AddNewRoleText,
            Properties.UICommandResource.AddNewRoleGestures,
            Properties.UICommandResource.AddNewRoleGesturesDisplayText,
            Properties.UICommandResource.AddNewRoleTooltip,
            Properties.UICommandResource.AddNewRoleDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewRole
        {
            get { return _AddNewRole; }
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