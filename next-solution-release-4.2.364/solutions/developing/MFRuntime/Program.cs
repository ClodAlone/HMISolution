using System;

using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Hardware;

namespace MFRuntime
{
    public class Program : Microsoft.SPOT.Application
    {
        static public Font NinaBFont;
        static public Font SmallFont;

        private static Program myApplication;
        static private Window mainWindow;

        public static void Main()
        {
            myApplication = new Program();

            // Load the fonts.
            NinaBFont = Resources.GetFont(Resources.FontResources.NinaB);
            SmallFont = Resources.GetFont(Resources.FontResources.small);

            mainWindow = new UI.MainMenuWindow(myApplication);

            // Create the object that configures the GPIO pins to buttons.
            GPIOButtonInputProvider inputProvider = new GPIOButtonInputProvider(null);

            // Start the application
            myApplication.Run(mainWindow);
        }

        public void GoHome()
        {
            Buttons.Focus(Program.mainWindow); // Set focus back to the main window
        }
    }
}
