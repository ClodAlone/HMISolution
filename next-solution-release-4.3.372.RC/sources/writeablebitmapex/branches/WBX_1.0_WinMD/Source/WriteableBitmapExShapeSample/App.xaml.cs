#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       Sample for the WriteableBitmap extension methods.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2011-12-16 19:50:22 +0100 (Fr, 16 Dez 2011) $
//   Changed in:        $Revision: 83935 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/branches/WBX_1.0_BitmapContext/Source/WriteableBitmapExShapeSample/App.xaml.cs $
//   Id:                $Id: App.xaml.cs 83935 2011-12-16 18:50:22Z unknown $
//
//
//   Copyright © 2009-2011 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WriteableBitmapExShapeSample
{
   public partial class App : Application
   {

      public App()
      {
         this.Startup += this.Application_Startup;
         this.Exit += this.Application_Exit;
         this.UnhandledException += this.Application_UnhandledException;

         InitializeComponent();
      }

      private void Application_Startup(object sender, StartupEventArgs e)
      {
         this.RootVisual = new MainPage();
      }

      private void Application_Exit(object sender, EventArgs e)
      {

      }
      private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
      {
         // If the app is running outside of the debugger then report the exception using
         // the browser's exception mechanism. On IE this will display it a yellow alert 
         // icon in the status bar and Firefox will display a script error.
         if (!System.Diagnostics.Debugger.IsAttached)
         {

            // NOTE: This will allow the application to continue running after an exception has been thrown
            // but not handled. 
            // For production applications this error handling should be replaced with something that will 
            // report the error to the website and stop the application.
            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
         }
      }
      private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
      {
         try
         {
            string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
            errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

            System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
         }
         catch (Exception)
         {
         }
      }
   }
}
