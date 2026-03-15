using System;
using System.Windows.Input;
using Utilities;

namespace WizardPluginHelpers
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
        private static GeneralCommand _prevStep = new GeneralCommand(
                        Properties.Resources.PreviousBtn,
                        Properties.Resources.PreviousBtn,
                        Properties.Resources.PreviousBtn,
                        Properties.Resources.PreviousBtn,
                        Properties.Resources.PreviousBtn,
                        Properties.Resources.PreviousBtn,
                        typeof(UIGeneralCommands));

        public static GeneralCommand PrevStep
        {
            get { return _prevStep; }
        }

        private static GeneralCommand _nextStep = new GeneralCommand(
                       Properties.Resources.NextBtn,
                       Properties.Resources.NextBtn,
                       Properties.Resources.NextBtn,
                       Properties.Resources.NextBtn,
                       Properties.Resources.NextBtn,
                       Properties.Resources.NextBtn,
                       typeof(UIGeneralCommands));

        public static GeneralCommand NextStep
        {
            get { return _nextStep; }
        }

        private static GeneralCommand _finish = new GeneralCommand(
                       Properties.Resources.Finish,
                       Properties.Resources.Finish,
                       Properties.Resources.Finish,
                       Properties.Resources.Finish,
                       Properties.Resources.Finish,
                       Properties.Resources.Finish,
                       typeof(UIGeneralCommands));

        public static GeneralCommand FinishCommand
        {
            get { return _finish; }
        }

        private static GeneralCommand _cancel = new GeneralCommand(
                       Properties.Resources.Cancel,
                       Properties.Resources.Cancel,
                       Properties.Resources.Cancel,
                       Properties.Resources.Cancel,
                       Properties.Resources.Cancel,
                       Properties.Resources.Cancel,
                       typeof(UIGeneralCommands));

        public static GeneralCommand CancelCommand
        {
            get { return _cancel; }
        }

        private static GeneralCommand _help = new GeneralCommand(
                       Properties.Resources.Help,
                       Properties.Resources.Help,
                       Properties.Resources.Help,
                       Properties.Resources.Help,
                       Properties.Resources.Help,
                       Properties.Resources.Help,
                       typeof(UIGeneralCommands));

        public static GeneralCommand HelpCommand
        {
            get { return _help; }
        }
    }
}