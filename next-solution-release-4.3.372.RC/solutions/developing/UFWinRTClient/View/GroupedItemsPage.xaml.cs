using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using DevExpress.UI.Xaml.Layout;

namespace UFWinRTClient.View
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : DXPage
    {
        public GroupedItemsPage()
        {
            this.InitializeComponent();
        }

        ServiceGateway.ServiceGatewayClient someSvc;
        private async void Button_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            var binaryMessageEncoding = new BinaryMessageEncodingBindingElement();
            var httpTransport = new HttpTransportBindingElement() 
                                            { 
                                                MaxBufferSize = int.MaxValue, 
                                                MaxReceivedMessageSize = int.MaxValue
                                            };
            // var security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            // add the binding elements into a Custom Binding
            var binding = new CustomBinding(binaryMessageEncoding, httpTransport);
            // var binding = new BasicHttpBinding();
            // binding.Security.Mode = BasicHttpSecurityMode.Transport;
            // binding.Security.Mode = BasicHttpSecurityMode.None; 
            // binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            var endpoint = new EndpointAddress("http://localhost:52878/ServiceGateway.svc");


            someSvc = new ServiceGateway.ServiceGatewayClient(binding, endpoint);
            // var cred = CredentialCache.DefaultNetworkCredentials;
            // someSvc.ClientCredentials.Windows.ClientCredential = new NetworkCredential("pippo", "ohibo");
            // someSvc.ClientCredentials.Windows.ClientCredential.Password
            someSvc.ClientCredentials.UserName.UserName = "user1";
            someSvc.ClientCredentials.UserName.Password = "userpassword1";
            
            /*
            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            var endpoint = new EndpointAddress("https://claudio-pc:44300/ServiceGateway.svc");


            someSvc = new ServiceGateway.ServiceGatewayClient(binding, endpoint);
            //var cred = CredentialCache.DefaultNetworkCredentials;
            //someSvc.ClientCredentials.Windows.ClientCredential = cred;
            someSvc.ClientCredentials.UserName.UserName = "user1";
            someSvc.ClientCredentials.UserName.Password = "userpassword1";
             * */
            await someSvc.RegisterAsync(100, 200);
        }
    }
}
