using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace UnitConverterManager
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
        private static GeneralCommand _RenameLocale = new GeneralCommand(
         Properties.UICommandResource.RenameLocaleName,
         Properties.UICommandResource.RenameLocaleText,
         Properties.UICommandResource.RenameLocaleGestures,
         Properties.UICommandResource.RenameLocaleGesturesDisplayText,
         Properties.UICommandResource.RenameLocaleTooltip,
         Properties.UICommandResource.RenameLocaleDescription,
         typeof(UIGeneralCommands));

        public static GeneralCommand RenameLocale
        {
            get { return _RenameLocale; }
        }

        private static GeneralCommand _RemoveConverter = new GeneralCommand(
          Properties.UICommandResource.RemoveConverterName,
          Properties.UICommandResource.RemoveConverterText,
          Properties.UICommandResource.RemoveConverterGestures,
          Properties.UICommandResource.RemoveConverterGesturesDisplayText,
          Properties.UICommandResource.RemoveConverterTooltip,
          Properties.UICommandResource.RemoveConverterDescription,
          typeof(UIGeneralCommands));

        public static GeneralCommand RemoveConverter
        {
            get { return _RemoveConverter; }
        }

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

        public static GeneralCommand AddNewConverter
        {
            get { return _AddNewConverter; }
        }

        private static GeneralCommand _AddNewConverter = new GeneralCommand(
            Properties.UICommandResource.AddNewConverterName,
            Properties.UICommandResource.AddNewConverterText,
            Properties.UICommandResource.AddNewConverterGestures,
            Properties.UICommandResource.AddNewConverterGesturesDisplayText,
            Properties.UICommandResource.AddNewConverterTooltip,
            Properties.UICommandResource.AddNewConverterDescription,
            typeof(UIGeneralCommands));

     
        private static GeneralCommand _ImportConverters = new GeneralCommand(
            Properties.UICommandResource.ImportConvertersName,
            Properties.UICommandResource.ImportConvertersText,
            Properties.UICommandResource.ImportConvertersGestures,
            Properties.UICommandResource.ImportConvertersGesturesDisplayText,
            Properties.UICommandResource.ImportConvertersTooltip,
            Properties.UICommandResource.ImportConvertersDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ImportConverters
        {
            get { return _ImportConverters; }
        }

        private static GeneralCommand _ExportConverters = new GeneralCommand(
            Properties.UICommandResource.ExportConvertersName,
            Properties.UICommandResource.ExportConvertersText,
            Properties.UICommandResource.ExportConvertersGestures,
            Properties.UICommandResource.ExportConvertersGesturesDisplayText,
            Properties.UICommandResource.ExportConvertersTooltip,
            Properties.UICommandResource.ExportConvertersDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ExportConverters
        {
            get { return _ExportConverters; }
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
