using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UFInterfaces.CoreHostComponents;
using Utilities;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NExTIoTShell
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        #region Components
        ComponentHost componentHost = new ComponentHost();

        ScreenManager.ComponentService.ScreenManagerComponent screenComponent = new ScreenManager.ComponentService.ScreenManagerComponent();
        public ScreenManager.ComponentService.ScreenManagerComponent ScreenComponent
        {
            get
            {
                return screenComponent;
            }
        }

        UFProjectManager.ComponentService.UFProjectManagerComponent ufProjectManagerComponent = new UFProjectManager.ComponentService.UFProjectManagerComponent();
        public UFProjectManager.ComponentService.UFProjectManagerComponent UFProjectManagerComponent
        {
            get
            {
                return ufProjectManagerComponent;
            }
        }

        LogicManager.ComponentService.DocumentEditorManagerComponent ufLogicManagerComponent = new LogicManager.ComponentService.DocumentEditorManagerComponent();
        public LogicManager.ComponentService.DocumentEditorManagerComponent UFLogicManagerComponent
        {
            get
            {
                return ufLogicManagerComponent;
            }
        }

        UIMsgBoxAlertService.ComponentService.UIMsgBoxAlertServiceComponent uiMsgBoxAlertServiceComponent = new UIMsgBoxAlertService.ComponentService.UIMsgBoxAlertServiceComponent();
        public UIMsgBoxAlertService.ComponentService.UIMsgBoxAlertServiceComponent UIMsgBoxAlertServiceComponent
        {
            get
            {
                return uiMsgBoxAlertServiceComponent;
            }
        }

        UriResolver.ComponentService.UriResolverComponent uriRisolverComponent = new UriResolver.ComponentService.UriResolverComponent();
        public UriResolver.ComponentService.UriResolverComponent URIRisolverComponent
        {
            get
            {
                return uriRisolverComponent;
            }
        }

        SysVariables.SysVariables sysVariables = new SysVariables.SysVariables();
        GPIO.GPIO gpio = new GPIO.GPIO();

        #endregion
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            componentHost.Components.Add(UFProjectManagerComponent);
            componentHost.Components.Add(ScreenComponent);
            componentHost.Components.Add(UFLogicManagerComponent);
            componentHost.Components.Add(UIMsgBoxAlertServiceComponent);
            componentHost.Components.Add(URIRisolverComponent);

            URIRisolverComponent.RegisterDocumentManager(ScreenComponent, ScreenComponent.TypeScheme);
            URIRisolverComponent.RegisterDocumentManager(UFLogicManagerComponent, UFLogicManagerComponent.TypeScheme);

            sysVariables.Initialize();
            gpio.Initialize();
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
