namespace UFWebClient
{
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using System.ServiceModel;
    using System.Windows;
    using UFWebClient.UFProjectManagerServiceReference;
    using System.ServiceModel.Channels;
    using System;
    using ComponentArt.UFSolution.Demos;

    /// <summary>
    /// Home page for the application.
    /// </summary>
    public partial class Home : Page
    {
        UFProjectManagerClient client;

        /// <summary>
        /// Creates a new <see cref="Home"/> instance.
        /// </summary>
        public Home()
        {
            InitializeComponent();

            this.Title = ApplicationStrings.HomePageTitle;
        }

        private void Initialize()
        {
            busyIndicator.IsBusy = true;
            busyIndicator.BusyContent = ApplicationStrings.BusyIndicatorSearchingServers;

            CustomBinding binding = new CustomBinding(
                            //new PollingDuplexBindingElement()
                            //            {
                            //                DuplexMode = PollingDuplexMode.MultipleMessagesPerPoll,
                            //                MaxPendingMessagesPerSession = 2147483647
                            //            },
                            new BinaryMessageEncodingBindingElement(),
                            new HttpTransportBindingElement()
                                        {
                                            MaxReceivedMessageSize = 2147483647,
                                            MaxBufferSize = 2147483647,
                                            TransferMode = TransferMode.StreamedResponse
                                        });

            client = new UFProjectManagerClient(binding, GetEndPoint());
            
            client.ListActiveProjectsAsync();
            client.ListActiveProjectsCompleted += (o, e) =>
                {
                    busyIndicator.IsBusy = false;

                    if (e.Error != null)
                    {
                        ErrorWindow.CreateNew(e.Error);
                    }
                    else
                        listRunningProjects.ItemsSource = e.Result;
                };
        }

        /// <summary>
        /// Executes when the user navigates to this page.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Initialize();
        }

        private void listRunningProjects_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            busyIndicator.IsBusy = true;
            busyIndicator.BusyContent = ApplicationStrings.BusyIndicatorSearchingServers;

            var item = listRunningProjects.SelectedItem as String;

            client.GetStartupScreenAsync(item);
            client.GetStartupScreenCompleted += (o, ev) =>
                {
                    busyIndicator.IsBusy = false;

                    if (ev.Error != null)
                    {
                        ErrorWindow.CreateNew(ev.Error);
                    }
                    else
                    {
                        if (App.Current.Resources.Contains("requestedUri"))
                            App.Current.Resources.Remove("requestedUri");
                        App.Current.Resources.Add("requestedUri", ev.Result);
                        NavigationService.Navigate(new Uri("/ScreenContainer", UriKind.Relative));
                    }
                };
        }

        private static EndpointAddress GetEndPoint()
        {
            return new EndpointAddress(
                "http://" +
                Application.Current.Host.Source.DnsSafeHost +
                ":10002/UFProjectManager");
        }
    }
}