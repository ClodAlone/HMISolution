using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using Utilities;
using System.Security.Cryptography.X509Certificates;
using UFInterfaces;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IdentityModel.Tokens;
using System.IdentityModel.Claims;
using OPCUAViewModelService.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using System.Security.Cryptography;
using System.Windows.Threading;
#endif
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.IO;

namespace OPCUAViewModel
{
    public class SessionViewModel : TreeViewItemViewModel, IEntityReference
    {
#region Members
        Session session;
        String sessionName;
        UserIdentity userIdentity;

        static ApplicationConfiguration globalConfiguration;
        public static SafeObservableCollection<SessionViewModel> listActiveSessions = new SafeObservableCollection<SessionViewModel>();
        static Dictionary<SessionTitleModel, SessionViewModel> mapActiveSessions = new Dictionary<SessionTitleModel, SessionViewModel>();

#endregion

#region Constructor
        public SessionViewModel(String sessionname, TreeViewItemViewModel parent)
            : base(parent, false)
        {
            if (parent.Parent is ApplicationDescriptionViewModel)
                Title = parent.Parent.Title;
            else
                Title = parent.Title;

            String globaName;
#if !WINDOWS_UWP
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
                globaName = assembly.GetName().Name;
            else
                globaName = "UFSolution";
#else
            globaName = "NExTIoT";
#endif                        
            var hostname = System.Net.Dns.GetHostName();
            Name = String.Format("{0} ({1}) - {2} - {3}", globaName, hostname, sessionname, Title);
            sessionName = sessionname;
            CreateUniqueName();

            lock (mapActiveSessions)
            {
                if (!listActiveSessions.Contains(this))
                    listActiveSessions.Add(this);

                mapActiveSessions.Add(GetComposedTitle(), this);
            }
        }
#endregion

#region events
        /// <summary>
        ///  Raised when the session user identity changes.
        /// </summary>
        public EventHandler<UserIdentityChangedEventArgs> UserIdentityChanged;

        void OnUserIdentityChanged(IUserIdentity identity)
        {
            var handler = UserIdentityChanged;
            if (handler != null)
            {
                handler(this, new UserIdentityChangedEventArgs(identity));
            }
        }
#endregion

#region Commands
        RelayCommand _browserCommand;
        public ICommand BrowserCommand
        {
            get
            {
                if (_browserCommand == null)
                {
                    _browserCommand = new RelayCommand(
                        param => CreateBrowser(),
                        param => CanCreateBrowser
                        );
                }
                return _browserCommand;
            }
        }

        bool CanCreateBrowser
        {
            get { return session != null; }
        }

        public BrowserViewModel CreateBrowser()
        {
            return CreateBrowser(BrowseDirection.Forward, true, 0, false, 
                                 ReferenceTypes.HierarchicalReferences);
        }

        public BrowserViewModel CreateBrowser(NodeId rootId, bool bContinueUntilDone)
        {
            BrowserViewModel browser = CreateBrowser();

            if (rootId != null)
                browser.rootId = rootId;
            return browser;
        }

        public BrowserViewModel CreateAreaBrowser(NodeId rootId)
        {
            BrowserViewModel browser = CreateBrowser(BrowseDirection.Forward, true, 0, false,
                                        ReferenceTypeIds.HasEventSource);
            if (rootId == null)
                rootId = ObjectIds.Server;
            browser.rootId = rootId;
            return browser;
        }

        public BrowserViewModel CreateBrowser(BrowseDirection browseDir, bool bIncludeSubTypes, 
                                              int NodeClassMask, bool ContinueUntilDone, 
                                              NodeId referTypeId)
        {
            Browser browser = new Browser(session) 
            { 
                BrowseDirection = browseDir, 
                IncludeSubtypes = bIncludeSubTypes, 
                NodeClassMask = NodeClassMask, 
                ContinueUntilDone = ContinueUntilDone, 
                ReferenceTypeId = referTypeId 
            };

            return new BrowserViewModel(browser, this);
        }


        RelayCommand _subscriptionCommand;
        public ICommand SubscriptionCommand
        {
            get
            {
                if (_subscriptionCommand == null)
                {
                    _subscriptionCommand = new RelayCommand(
                        param => CreateSubscription(String.Empty),
                        param => CanCreateSubscription
                        );
                }
                return _subscriptionCommand;
            }
        }

        public bool CanCreateSubscription
        {
            get { return session != null; }
        }

        public SubscriptionViewModel CreateSubscription(String Name, bool bUsePollingRead = false)
        {
            bool hasChildren = HasChildren;

            lock (lockObject)
            {
                if (hasChildren)
                {
                    var found = (from c in Children.OfType<SubscriptionViewModel>()
                                 where c.subscription != null && c.subscription.DisplayName == Name
                                 select c).ToList();
                    if (found.Count > 0)
                        return found[0];
                }
            }

            SubscriptionViewModel svm = null;
            Subscription subscription = null;
            try
            {
                subscription = new Subscription(session.DefaultSubscription);
                subscription.PublishingInterval = 1000;
                subscription.KeepAliveCount = 10;
                subscription.LifetimeCount = 10;
                subscription.MaxNotificationsPerPublish = 1000;
                subscription.Priority = 100;

                if (!String.IsNullOrEmpty(Name))
                    subscription.DisplayName = Name;
                session.AddSubscription(subscription);

                bool bErrorSubscription = false;
                try
                {
                    if (!bUsePollingRead)
                        subscription.Create();
                }
                catch
                {
                    bErrorSubscription = true;
                }

                svm = new SubscriptionViewModel(subscription, this, bErrorSubscription || bUsePollingRead);

                lock (lockObject)
                {
                    Children.Add(svm);
                }
            }
            catch (Exception ex)
            {
                if (svm != null)
                {
                    RemoveSubscription(svm);
                    svm = null;
                }
                else if (subscription != null)
                {
                    session.RemoveSubscription(subscription);
                    subscription.Dispose();
                }

                LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUASubscriptionFailed, ex.Message);
                Utils.Trace(ex, Properties.Resource.OPCUASubscriptionFailed);
            }
            return svm;
        }

        public void RemoveSubscription(SubscriptionViewModel svm)
        {
            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                if (hasChildren && Children.Contains(svm))
                    Children.Remove(svm);
            }

            svm.Dispose();
        }

        long maxNodesPerTranslateBrowsePathsToNodeIds = 0;
        internal void SetMaxNodesPerTranslateBrowsePathsToNodeIds(long n)
        {
            maxNodesPerTranslateBrowsePathsToNodeIds = n;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        RelayCommand _connectCommand;
        public ICommand CommandCommand
        {
            get
            {
                if (_connectCommand == null)
                {
                    _connectCommand = new RelayCommand(
                        param =>
                        {
                            session = CreateSession(); 
                            if (session != null)
                                Connected = true;
                        },
                        param => CanCreateSession
                        );
                }
                return _connectCommand;
            }
        }
#endif

        bool CanCreateSession
        {
            get { return session != null && !Connected; }
        }

        void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            // LastMessage = String.Format("{0} - Untrusted Certificate : {0}", Title, e.Certificate.Subject);
            StringBuilder buffer = new StringBuilder();

            buffer.AppendFormat("Certificate could not validated: {0}\r\n\r\n", e.Error.StatusCode);
            buffer.AppendFormat("Subject: {0}\r\n", e.Certificate.Subject);
            buffer.AppendFormat("Issuer: {0}\r\n", (e.Certificate.Subject == e.Certificate.Issuer) ? "Self-signed" : e.Certificate.Issuer);
            buffer.AppendFormat("Valid From: {0}\r\n", e.Certificate.NotBefore);
            buffer.AppendFormat("Valid To: {0}\r\n", e.Certificate.NotAfter);
            buffer.AppendFormat("Thumbprint: {0}\r\n\r\n", e.Certificate.Thumbprint);

            buffer.AppendFormat("Accept anyways?");

            //if (MessageBox.Show(buffer.ToString(), caller.Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    e.Accept = true;
            //}
            e.Accept = true;

#if !WINDOWS_UWP && !NET_STANDARD
            LastMessage = buffer.ToString();
            if (!AcceptUntrustedCertificates && 
                !configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates && 
                OPCUAViewModelComponent.uiInterfaceAvailable &&
                OPCUAViewModelComponent.uiInterface.ShowYesNo(buffer.ToString(), CustomDialogIcons.Warning) != CustomDialogResults.Yes)
                e.Accept = false;
#endif
        }

        /// <summary>
        /// Creates a user identity for the policy.
        /// </summary>
        bool bDoNotAskUserAgain;
        IUserIdentity CreateUserIdentity(UserTokenType TokenType)
        {
            if (TokenType == UserTokenType.Anonymous)
                return new UserIdentity(new AnonymousIdentityToken());

            //if (TokenType == UserTokenType.Anonymous)
            //{
            //    if (!OPCUAViewModelComponent.uiInterfaceAvailable || bPromptUserLoginHasAlreadyBeenAsked ||
            //        OPCUAViewModelComponent.uiInterface.ShowYesNo("Would you like to connect with a user name ?", CustomDialogIcons.Question) != CustomDialogResults.Yes)
            //    {
            //        bPromptUserLoginHasAlreadyBeenAsked = true;
            //        return null;
            //    }
            //    bPromptUserLoginHasAlreadyBeenAsked = true;
            //    TokenType = UserTokenType.UserName;
            //}

            if (TokenType == UserTokenType.UserName)
            {
                if (userIdentity == null)
                {
                    var settings = RealTimeConnectionManagerViewModel.GetSession(sessionName);
                    if (!String.IsNullOrEmpty(settings.UserName) && !String.IsNullOrEmpty(settings.Password))
                        userIdentity = new UserIdentity(settings.UserName, settings.Password);
                }
#if !WINDOWS_UWP && !NET_STANDARD
                if (userIdentity == null)
                {
                    ShowCredentialOptions options = new ShowCredentialOptions();
                    options.WindowTitle = Title;
                    options.MainInstruction = Properties.Resource.ShowCredentialMainInstruction;
                    options.Content = Properties.Resource.ShowCredentialContent;
                    options.SavedCredentialsBucket = Title;
                    if (!bDoNotAskUserAgain && OPCUAViewModelComponent.uiInterfaceAvailable)
                    {
                        bDoNotAskUserAgain = true;
                        ShowCredentialResults res = OPCUAViewModelComponent.uiInterface.ShowCredentialDialog(options);
                        if (res.result == CustomDialogResults.OK)
                            userIdentity = new UserIdentity(res.UserName, res.Password);
                        else
                            throw ServiceResultException.Create(StatusCodes.BadSecurityPolicyRejected, "User token policy is not supported.");
                    }
                }
#endif
                return userIdentity;
            }

#if !WINDOWS_UWP
            if (TokenType == UserTokenType.Certificate)
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

                try
                {
                    foreach (X509Certificate2 certificate in store.Certificates)
                    {
                        if (certificate.HasPrivateKey)
                        {
                            return new UserIdentity(certificate);
                        }
                    }

                    return null;
                }
                finally
                {
                    store.Close();
                }
            }

//#if !NET_STANDARD
//            if (TokenType == UserTokenType.IssuedToken)
//                return CreateSAMLToken("someone@somewhere.com");
//#endif
#endif

            throw ServiceResultException.Create(StatusCodes.BadSecurityPolicyRejected, "User token policy is not supported.");
        }

//#if !WINDOWS_UWP && !NET_STANDARD
//        public static UserIdentity CreateSAMLToken(string emailAddress)
//        {
//            // Normally this would be done by a server that is capable of verifying that
//            // the user is a legimate holder of email address. Using a local certficate to
//            // signed the SAML token is a short cut that would never be done in a real system.
//            CertificateIdentifier userid = new CertificateIdentifier();

//            userid.StoreType = CertificateStoreType.Windows;
//            userid.StorePath = "LocalMachine\\My";
//            userid.SubjectName = "UA Sample Client";

//            X509Certificate2 certificate = userid.Find();
//            X509SecurityToken signingToken = new X509SecurityToken(certificate);

//            // Create list of confirmation strings
//            List<string> confirmations = new List<string>();

//            // Add holder-of-key string to list of confirmation strings
//            confirmations.Add("urn:oasis:names:tc:SAML:1.0:cm:bearer");

//            // Create SAML subject statement based on issuer member variable, confirmation string collection 
//            // local variable and proof key identifier parameter
//            SamlSubject subject = new SamlSubject("urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress", null, emailAddress);

//            // Create a list of SAML attributes
//            List<SamlAttribute> attributes = new List<SamlAttribute>();
//            Claim claim = Claim.CreateNameClaim(emailAddress);
//            attributes.Add(new SamlAttribute(claim));

//            // Create list of SAML statements
//            List<SamlStatement> statements = new List<SamlStatement>();

//            // Add a SAML attribute statement to the list of statements. Attribute statement is based on 
//            // subject statement and SAML attributes resulting from claims
//            statements.Add(new SamlAttributeStatement(subject, attributes));

//            // Create a valid from/until condition
//            DateTime validFrom = DateTime.UtcNow;
//            DateTime validTo = DateTime.UtcNow.AddHours(12);

//            SamlConditions conditions = new SamlConditions(validFrom, validTo);

//            // Create the SAML assertion
//            SamlAssertion assertion = new SamlAssertion(
//                "_" + Guid.NewGuid().ToString(),
//                signingToken.Certificate.Subject,
//                validFrom,
//                conditions,
//                null,
//                statements);

//            SecurityKey signingKey = new System.IdentityModel.Tokens.RsaSecurityKey((RSA)signingToken.Certificate.PrivateKey);

//            // Set the signing credentials for the SAML assertion
//            assertion.SigningCredentials = new SigningCredentials(
//                signingKey,
//                System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha1Signature,
//                System.IdentityModel.Tokens.SecurityAlgorithms.Sha1Digest,
//                new SecurityKeyIdentifier(signingToken.CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause>()));

//            return new UserIdentity(new SamlSecurityToken(assertion));
//        }
//#endif
        bool bRequeryValue;
        public void RenewUserIdentity(IUserIdentity identity, StringCollection preferredLocales)
        {
#if !WINDOWS_UWP
            if (preferredLocales.Count == 0)
                preferredLocales.Add(System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
#endif
            lock (lockObject)
            {
                if (session != null)
                    session.UpdateSession(identity, preferredLocales);
            }
            if (identity != null && identity.TokenType == UserTokenType.UserName)
            { 
                userIdentity = (UserIdentity)identity;
                UserTokenType = identity.TokenType;
            }
            
            lock (lockObject)
            {
                bRequeryValue = true;
            }
            PromoteIdleExecution(1000);
            OnUserIdentityChanged(identity);
        }

        void RequeryValues()
        {
            var list = new List<SubscriptionViewModel>();
            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                if (hasChildren)
                {
                    foreach (var v in Children)
                        list.Add(v as SubscriptionViewModel);
                }
            }
            list.ForEach(svm =>
            {
                svm.RequeryNotConnectedItems();
            });
        }

#if WINDOWS_UWP || NET_STANDARD
        async Task<Session>
#else
        Session 
#endif
        CreateSession(bool reconnect = false)
        {
            Session s = null;

            if (configuration == null)
            {
                if (Parent is EndpointDescriptionViewModel)
                {
                    if ((Parent as EndpointDescriptionViewModel).configuration != null)
                        configuration = (Parent as EndpointDescriptionViewModel).configuration;
                    else if (Parent.Parent is ApplicationDescriptionViewModel)
                        configuration = (Parent.Parent as ApplicationDescriptionViewModel).configuration;
                    else if (Parent.Parent is OPCUADiscoveryViewModel)
                        configuration = (Parent.Parent as OPCUADiscoveryViewModel).configuration;
#if !WINDOWS_UWP && !NET_STANDARD
                    if (configuration == null)
                    {
                        lock (mapActiveSessions)
                        {
                            if (globalConfiguration == null)
                            {
                                globalConfiguration = Helpers.CreateClientConfiguration();
                                // m_configuration = ApplicationConfiguration.Load("SampleConfiguration", ApplicationType.ClientAndServer);
                                // Helpers.CheckApplicationInstanceCertificate(globalConfiguration);
                                try
                                { 
                                    X509Certificate2 certificate = Helpers.CheckApplicationInstanceCertificate(globalConfiguration, 2048, true, true);
                                    if (certificate != null)
                                    {
                                        // ensure the application uri matches the certificate.
                                        string applicationUri = Utils.GetApplicationUriFromCertficate(certificate);

                                        if (applicationUri != null)
                                        {
                                            globalConfiguration.ApplicationUri = applicationUri;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, Title, MessageBoxButton.OK,
                                                MessageBoxImage.Error, MessageBoxResult.OK,
                                                MessageBoxOptions.ServiceNotification);
                                }

                                globalConfiguration.CertificateValidator.CertificateValidation += CertificateValidator_CertificateValidation;
                            }
                        }

                        configuration = globalConfiguration;
                    }
#else
                    if (configuration == null)
                        configuration = Helpers.CreateClientConfiguration();
#endif
                    //configuration.ApplicationUri = Utils.ReplaceLocalhost(configuration.ApplicationUri);

                    endPoint = new ConfiguredEndpoint(null,
                        (Parent as EndpointDescriptionViewModel).endpointDescription,
                        EndpointConfiguration.Create(configuration));
                }
            }

            /*
            // create the binding factory if it has not been created yet.
            if (bindingFactory == null)
                bindingFactory = BindingFactory.Create(configuration);

            // update from server.
            if (endPoint.UpdateBeforeConnect)
            {
                try
                {
                    endPoint.UpdateFromServer(bindingFactory);
                }
                finally
                {
                }
            }

            // the client cannot connect unless it trusts the server certificate.
            // for simplicity the server certificate is automatically added to the trust list, 
            // however, this should never be done in a real application unless it has been 
            // confirmed by the administrator of the application.
            configuration.SecurityConfiguration.AddTrustedPeer(endPoint.Description.ServerCertificate);
            */               
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);

            // find the client certificate.
            X509Certificate2 clientCertificate =
#if WINDOWS_UWP || NET_STANDARD
                await
#endif
                configuration.SecurityConfiguration.ApplicationCertificate.Find();

            /*
            // create the channel.            
            channel = SessionChannel.Create(
                configuration,
                endPoint.Description,
                endPoint.Configuration,
                bindingFactory,
                clientCertificate,
                null);
            */

            var userpolicy = endPoint.Description.FindUserTokenPolicy(UserTokenType, (string)null);
            if (userpolicy == null)
            {
                UserTokenType = UserTokenType.UserName;
                userpolicy = endPoint.Description.FindUserTokenPolicy(UserTokenType, (string)null);
                if (userpolicy == null)
                {
                    UserTokenType = UserTokenType.Certificate;
                    userpolicy = endPoint.Description.FindUserTokenPolicy(UserTokenType, (string)null);
                }
            }

            /*
            ServiceMessageContext context = new ServiceMessageContext 
            { 
                MaxStringLength = configuration.TransportQuotas.MaxStringLength, 
                MaxByteStringLength = configuration.TransportQuotas.MaxByteStringLength, 
                MaxArrayLength = configuration.TransportQuotas.MaxArrayLength, 
                MaxMessageSize = configuration.TransportQuotas.MaxMessageSize, 
                NamespaceUris = new NamespaceTable(), 
                ServerUris = new StringTable(), 
                Factory = configuration.MessageContext.Factory 
            };

            messageContext = context;

            // create the channel object used to connect to the server.
            SessionChannel channel = SessionChannel.Create(
                configuration,
                endPoint.Description,
                endpointConfiguration,
                BindingFactory.Create(configuration, messageContext),
                clientCertificate,
                null);
            */

            lock (lockObject)
            {
                if (!reconnect && (session != null || Connected))
                    return null;

                LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUASessionConnecting);


                ITransportChannel channel = null;
                // create the session.
                try
                {
                    // create the channel object used to connect to the server.
                    channel = SessionChannel.Create(
                        configuration,
                        endPoint.Description,
                        endpointConfiguration,
                        clientCertificate,
                        configuration.CreateMessageContext());

                    // create the session.
                    s = new Session(channel, configuration, endPoint, clientCertificate);
                    s.ReturnDiagnostics = EnableDiagnostic ? DiagnosticsMasks.All : DiagnosticsMasks.None;

                    // open the session.
                    var checkDomain = (configuration.SecurityConfiguration.ApplicationCertificate.ValidationOptions & CertificateValidationOptions.SuppressHostNameInvalid) == 0;
                    s.Open(Name, 0, CreateUserIdentity(UserTokenType), null, checkDomain);
                }
                catch (Exception ex)
                {
                    LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUASessionFailed, ex.Message);

                    Utils.Trace(ex, Properties.Resource.OPCUASessionFailed);

                    if (channel != null)
                    {
                        channel.Close();
                        channel.Dispose();
                    }

                    if (s != null)
                    {
                        s.Close();
                        s.Dispose();
                        s = null;
                    }
                }

                if (s != null)
                {
                    s.KeepAlive += session_KeepAlive;
                    session_KeepAlive(s, null);

                    // s.Notification += session_Notification;
                    s.PublishError += session_PublishError;
                    s.SessionClosing += session_SessionClosing;
                    s.SubscriptionsChanged += session_SubscriptionsChanged;
                    s.Handle = this;

                    LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUASessionCreated);
                }
            }

            return s;
        }
#endregion

#region Methods

        void CreateUniqueName()
        {
            lock (mapActiveSessions)
            {
                String originalTitle = Title;
                int i = 1;
                while (mapActiveSessions.ContainsKey(GetComposedTitle()))
                    Title = Name = String.Format("{0} {1}", originalTitle, i++);
            }
        }

        static SessionTitleModel GetComposedTitle(String sessionname, ApplicationDescriptionViewModel ap, EndpointDescriptionViewModel ep)
        {
            return new SessionTitleModel(sessionname, ap.Workstation, ap.Title, ep.DisplayTitle);
        }

        public SessionTitleModel GetComposedTitle()
        {
            ApplicationDescriptionViewModel ap = Parent.Parent as ApplicationDescriptionViewModel;
            EndpointDescriptionViewModel ep = Parent as EndpointDescriptionViewModel;
            if (ap != null && !String.IsNullOrEmpty(ap.Workstation) &&
                String.Compare(ap.Workstation, Properties.Settings.Default.localhost, true) != 0 &&
                String.Compare(ap.Workstation, Properties.Settings.Default.localhostip, true) != 0)
                return new SessionTitleModel(sessionName, ap.Workstation, Title, ep.DisplayTitle);

            return new SessionTitleModel(sessionName, Title, ep.Title);
        }

        public static SessionViewModel GetSessionViewModel(String sessionname, String name, String endpoint)
        {
            SessionViewModel ret = null;

            lock (mapActiveSessions)
            {
                mapActiveSessions.TryGetValue(new SessionTitleModel(sessionname, name, endpoint), out ret);
            }

            return ret;
        }

        public static SessionViewModel FindOrCreate(String sessioname, EndpointDescriptionViewModel epdvm)
        {
            SessionViewModel ret = null;

            lock (mapActiveSessions)
            {
                if (epdvm.Parent is ApplicationDescriptionViewModel)
                {
                    ApplicationDescriptionViewModel advm = epdvm.Parent as ApplicationDescriptionViewModel;
                    if (!mapActiveSessions.TryGetValue(GetComposedTitle(sessioname, advm, epdvm), out ret))
                        ret = new SessionViewModel(sessioname, epdvm);
                    else if (!ret.Connected)
                    {
                        EndpointDescriptionViewModel dpd = EndpointDescriptionViewModel.SelectEndpoint(advm.Workstation, advm.Title, true);
                        if (dpd != null && dpd != epdvm)
                            ret = new SessionViewModel(sessioname, dpd);
                    }
                }
                else
                {
                    ret = GetSessionViewModel(sessioname, epdvm.Title, epdvm.DisplayTitle);
                    if (ret == null)
                        ret = new SessionViewModel(sessioname, epdvm);
                }
            }

            return ret;
        }

        static Dictionary<String, EndpointDescriptionViewModel> mapDiscoveredEndpoints = new Dictionary<string, EndpointDescriptionViewModel>();
        static Dictionary<String, Task> mapDiscoveredEndpointPendings = new Dictionary<string, Task>();
        public static SessionViewModel FindOrCreate(String sessionname, String hostname, String appname, String endpointUrl, 
                                                    bool useSecurity, bool bDiscoverSynchro = false)
        {
            SessionViewModel ret = null;

            List<SessionTitleModel> list = new List<SessionTitleModel>();
            var sessionTitle = new SessionTitleModel(sessionname, hostname, appname, endPoint: null);
            var titleToFind = sessionTitle.ToString();
            lock (mapActiveSessions)
            {
                list = (from c in mapActiveSessions.Keys// .AsParallel()
                        where c == sessionTitle // && mapActiveSessions[c].Connected == true
                        select c).ToList();
            }

            if (list.Count == 0)
            {
                EndpointDescriptionViewModel dpd = null;
                if (!String.IsNullOrEmpty(endpointUrl))
                {
                    var e = EndpointDescriptionViewModel.SelectEndpoint(endpointUrl, useSecurity);
                    if (e != null)
                    {
                        var ap = new ApplicationDescription();
                        ap.ApplicationName = appname;

                        var a = new ApplicationDescriptionViewModel(ap, hostname, null, null);
                        dpd = new EndpointDescriptionViewModel(e, a);
                    }
                    else
                        dpd = EndpointDescriptionViewModel.SelectEndpoint(hostname, appname, useSecurity);
                }
                else
                {
                    if (bDiscoverSynchro)
                    {
                        dpd = EndpointDescriptionViewModel.SelectEndpoint(hostname, appname, useSecurity);
                    }
                    else
                    {
                        lock (mapActiveSessions)
                        {
                            if (mapDiscoveredEndpoints.ContainsKey(titleToFind))
                            {
                                dpd = mapDiscoveredEndpoints[titleToFind];
                                mapDiscoveredEndpoints.Remove(titleToFind);
                            }
                            if (dpd == null)
                            {
                                if (!mapDiscoveredEndpointPendings.ContainsKey(titleToFind))
                                {
                                    var task = Task.Factory.StartNew(() =>
                                    {
                                        try
                                        {
                                            dpd = EndpointDescriptionViewModel.SelectEndpoint(hostname, appname, useSecurity);
                                        }
                                        finally
                                        {
                                            lock (mapActiveSessions)
                                            {
                                                if (mapDiscoveredEndpoints.ContainsKey(titleToFind))
                                                    mapDiscoveredEndpoints.Remove(titleToFind);
                                                if (dpd != null)
                                                    mapDiscoveredEndpoints.Add(titleToFind, dpd);

                                                mapDiscoveredEndpointPendings.Remove(titleToFind);
                                            }
                                        }
                                    }, TaskCreationOptions.LongRunning);
                                    mapDiscoveredEndpointPendings.Add(titleToFind, task);
                                }
                            }
                        }
                    }
                }
                if (dpd != null)
                    ret = new SessionViewModel(sessionname, dpd);
            }
            else
            {
                lock (mapActiveSessions)
                {
                    ret = mapActiveSessions[list[0]];
                }
            }

            return ret;
        }

        void  session_SubscriptionsChanged(object sender, EventArgs e)
        {
            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUASessionSubscriptionChanged);
        } 

        void  session_SessionClosing(object sender, EventArgs e)
        {
            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUASessionClosing);
        } 

        void  session_PublishError(
#if !NET_STANDARD
            Session s, 
#else
            ISession s,
#endif
            PublishErrorEventArgs e)
        {
            // check for events from discarded sessions.
            if (!Object.ReferenceEquals(s, session))
                return;

            LastMessage = String.Format("{0} - {1} - {2}", Title, Properties.Resource.OPCUASessionPublishError, e.Status);

            PromoteIdleExecution(1000);

            /*
            var list = new List<SubscriptionViewModel>();
            lock (lockObject)
            {
                foreach (var v in Children)
                    list.Add(v as SubscriptionViewModel);
            }
            list.ForEach(svm =>
            {
                svm.UpdateMonitoredItemNotConnected(StatusCodes.Uncertain);
            });
            */
        }

        void  session_Notification(Session s, NotificationEventArgs e)
        {
            // check for events from discarded sessions.
            if (!Object.ReferenceEquals(s, session))
                return;

            LastMessage = String.Format("{0} - {1} - {2}", Title, Properties.Resource.OPCUASessionNotification, e.NotificationMessage);
        }

        void session_KeepAlive(
#if !NET_STANDARD
            Session s, 
#else
            ISession s,
#endif
            KeepAliveEventArgs e)
        {
            // check for events from discarded sessions.
            // lock (lockObject)
            {
                if (bConnecting)
                {
                    if (Connected)
                        bConnecting = false;
                    else
                    {
                        AutoConnect = true;
                        PromoteIdleExecution(1000);
                        return;
                    }
                }

                if (session == null)
                {
                    Connected = false;
                    PromoteIdleExecution(1000);
                }

                if (!Object.ReferenceEquals(s, session))
                    return;

                PromoteIdleExecution(1000);

                if (e != null)
                {
                    // LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUASessionKeepAlive);
                    LastCurrentTime = e.CurrentTime;

                    CurrentState = e.CurrentState;
                    if (e.Status == null || !ServiceResult.IsBad(e.Status))
                    {
                        AutoConnect = true;
                        return;
                    }

                    LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUASessionKeepAliveBad);

                    Connected = false;
                    PromoteIdleExecution(1000);
                }
            }
        }

        bool bFailedToReconnect = false;
        bool bConnecting = false;
        protected
#if WINDOWS_UWP || NET_STANDARD
        async
#endif
        override void IdleExecution()
        {
            PromoteIdleExecution(2000);

            bool failedToReconnect = false;
#if WINDOWS_UWP || NET_STANDARD
            bool bTotalRecreation = false;
#endif
            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                if (hasChildren)
                {
                    foreach (var v in Children)
                    {
                        var svm = v as SubscriptionViewModel;
                        if (!svm.IsNanoServer && svm.m_publishLateCount >= Properties.Settings.Default.PublishLateCountBeforeReconnecting)
                        {
                            Connected = false;
                            AutoConnect = true;
                            break;
                        }
                    }
                }

                if (AutoConnect && !Connected)
                {
                    bConnecting = true;

                    if (session != null)
                    {
                        if (!bFailedToReconnect)
                        {
                            try
                            {
                                session.Reconnect();
                                Connected = true;
                                if (hasChildren)
                                {
                                    foreach (var v in Children)
                                    {
                                        SubscriptionViewModel svm = v as SubscriptionViewModel;
                                        svm.RecreateMonitoredItems(session);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Connected = false;
                                AutoConnect = true;

                                LastMessage = String.Format("{0} - {1} : {2}", Title, Properties.Resource.OPCUASessionReconnectedFailed, ex.Message);
                                Utils.Trace(ex, Properties.Resource.OPCUASessionReconnectedFailed);

                                bool recreateNow = false;

                                // recreate the session if it has been closed.
                                ServiceResultException sre = ex as ServiceResultException;

                                if (sre != null && (sre.StatusCode == StatusCodes.BadSessionClosed/* || sre.StatusCode == StatusCodes.BadSessionIdInvalid*/))
                                {
                                    recreateNow = true;
                                }
                                else
                                {
                                    // check if reconnecting is still an option.
                                    if (session != null && session.LastKeepAliveTime.AddMilliseconds(2000/*session.SessionTimeout*/) > DateTime.UtcNow)
                                    {
                                        // PromoteIdleExecution(2000);
                                        return;
                                    }
                                }

                                bFailedToReconnect = true;

                                // must re-create the subscription when the server comes back.
                                if (!recreateNow)
                                {
                                    // PromoteIdleExecution(2000);
                                    return;
                                }

                                failedToReconnect = true;
                                // PromoteIdleExecution(2000);
                            }
                        }
                        else
                            failedToReconnect = true;

                        if (failedToReconnect)
                        {
#if !WINDOWS_UWP && !NET_STANDARD
                            // re-create the session.
                            //try
                            //{
                            //    Session newsession = Session.Recreate(session);
                            //    session.Close();
                            //    session.Dispose();
                            //    session = newsession;

                            //    Connected = true;
                            //    bFailedToReconnect = false;

                            //    lock (lockObject)
                            //    {
                            //        foreach (var v in Children)
                            //        {
                            //            SubscriptionViewModel svm = v as SubscriptionViewModel;
                            //            svm.RecreateMonitoredItems(session);
                            //        }
                            //    }

                            //}
                            //catch (Exception exception)
                            //{
                            //    PromoteIdleExecution(2000);

                            //    Utils.Trace(exception, Properties.Resource.OPCUASessionReconnectedFailed);
                            //}

                            try
                            {
                                Connected = false;
                                Session newsession = CreateSession(true);
                                if (newsession != null)
                                {
                                    session.KeepAlive -= session_KeepAlive;
                                    // session.Notification -= session_Notification;
                                    session.PublishError -= session_PublishError;
                                    session.SessionClosing -= session_SessionClosing;
                                    session.SubscriptionsChanged -= session_SubscriptionsChanged;

                                    session.Close();
                                    session.Dispose();
                                    session = newsession;

                                    Connected = true;
                                    bFailedToReconnect = false;

                                    if (hasChildren)
                                    {
                                        foreach (var v in Children)
                                        {
                                            SubscriptionViewModel svm = v as SubscriptionViewModel;
                                            svm.RecreateMonitoredItems(session);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Connected = false;
                                AutoConnect = true;
                            	
                                // PromoteIdleExecution(DispatcherPriority.Invalid);
                                Utils.Trace(ex, Properties.Resource.OPCUASessionReconnectedFailed);
                            }
#else
                            bTotalRecreation = true;
#endif
                        }
                        else
                            Connected = true;
                    }
                    else
                    {
#if !WINDOWS_UWP && !NET_STANDARD
                        Session newsession = CreateSession(true);
                        if (newsession != null)
                        {
                            session = newsession;

                            Connected = true;
                            bFailedToReconnect = false;
                        }
#else
                        bTotalRecreation = true;
#endif
                    }
                }
            }

#if WINDOWS_UWP || NET_STANDARD
            if (bTotalRecreation)
            {
                Session newsession = await CreateSession(true);
                if (newsession != null)
                {
                    lock (lockObject)
                    {
                        session = newsession;

                        Connected = true;
                        bFailedToReconnect = false;
                    }
                }
            }
            else if (failedToReconnect)
            {
                // re-create the session.
                //try
                //{
                //    Session newsession = Session.Recreate(session);
                //    session.Close();
                //    session.Dispose();
                //    session = newsession;

                //    Connected = true;
                //    bFailedToReconnect = false;

                //    lock (lockObject)
                //    {
                //        foreach (var v in Children)
                //        {
                //            SubscriptionViewModel svm = v as SubscriptionViewModel;
                //            svm.RecreateMonitoredItems(session);
                //        }
                //    }

                //}
                //catch (Exception exception)
                //{
                //    PromoteIdleExecution(2000);

                //    Utils.Trace(exception, Properties.Resource.OPCUASessionReconnectedFailed);
                //}

                try
                {
                    Connected = false;
                    Session newsession = await CreateSession(true);
                    if (newsession != null)
                    {
                        lock (lockObject)
                        {
                            session.KeepAlive -= session_KeepAlive;
                            // session.Notification -= session_Notification;
                            session.PublishError -= session_PublishError;
                            session.SessionClosing -= session_SessionClosing;
                            session.SubscriptionsChanged -= session_SubscriptionsChanged;

                            session.Close();
                            session.Dispose();
                            session = newsession;

                            Connected = true;
                            bFailedToReconnect = false;

                            foreach (var v in Children)
                            {
                                SubscriptionViewModel svm = v as SubscriptionViewModel;
                                svm.RecreateMonitoredItems(session);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (lockObject)
                    {
                        Connected = false;
                        AutoConnect = true;
                    }
                    
                    // PromoteIdleExecution(DispatcherPriority.Invalid);
                    Utils.Trace(ex, Properties.Resource.OPCUASessionReconnectedFailed);
                }
            }
            else
            {
                lock (lockObject)
                {
                    Connected = true;
                }
            }
#endif

            var bRequery = false;
            lock (lockObject)
            {
                if (bRequeryValue)
                {
                    bRequeryValue = false;
                    bRequery = true;
                }
            }
            if (bRequery)
                RequeryValues();
        }

        protected override void OnDispose()
        {
            bDisposed = true;

            base.OnDispose();

            AutoConnect = false;
            lock (lockObject)
            {
                if (session != null)
                {
                    session.KeepAlive -= session_KeepAlive;
                    // session.Notification -= session_Notification;
                    session.PublishError -= session_PublishError;
                    session.SessionClosing -= session_SessionClosing;
                    session.SubscriptionsChanged -= session_SubscriptionsChanged;

                    session.Close();
                    session.Dispose();
                    session = null;
                }
            }

            _Connected = false;

            lock (mapActiveSessions)
            {
                if (listActiveSessions.Contains(this))
                    listActiveSessions.Remove(this);

                mapActiveSessions.Remove(GetComposedTitle());
            }
        }

        public Dictionary<String, ExpandedNodeId> GetNodeIds(
            ExpandedNodeId startNodeId,
            NamespaceTable namespacesUris,
            params String[] relativePaths)
        {
            if (session == null)
                throw new ArgumentNullException("session not connected");

            if (relativePaths == null)
                throw new ArgumentNullException("relativePaths null");

            // build the list of browse paths to follow by parsing the relative paths.
            BrowsePathCollection browsePaths = new BrowsePathCollection();
            for (int ii = 0; ii < relativePaths.Length; ii++)
            {
                BrowsePath browsePath = new BrowsePath();

                // The relative paths used indexes in the namespacesUris table. These must be 
                // converted to indexes used by the server. An error occurs if the relative path
                // refers to a namespaceUri that the server does not recognize.

                // The relative paths may refer to ReferenceType by their BrowseName. The TypeTree object
                // allows the parser to look up the server's NodeId for the ReferenceType.

                try
                {
                    browsePath.RelativePath = RelativePath.Parse(
                        relativePaths[ii],
                        session.TypeTree,
                        namespacesUris,
                        session.NamespaceUris);
                }
                catch (Exception ex)
                {
                    LastMessage = String.Format("{0} - {1} : {2} {3}", Title, Properties.Resource.OPCUASessionPathNotValid, relativePaths[ii], ex.Message);
                }

                browsePath.StartingNode = ExpandedNodeId.ToNodeId(startNodeId, Session.NamespaceUris);

                browsePaths.Add(browsePath);
            }

            Dictionary<String, ExpandedNodeId> nodes = new Dictionary<String, ExpandedNodeId>();

            if (maxNodesPerTranslateBrowsePathsToNodeIds > 0 && browsePaths.Count > maxNodesPerTranslateBrowsePathsToNodeIds)
            {
                for (long i = 0; i < browsePaths.Count; i += maxNodesPerTranslateBrowsePathsToNodeIds)
                {
                    var browsePathsRange = new BrowsePathCollection(browsePaths.GetRange((int)i, Math.Min((int)maxNodesPerTranslateBrowsePathsToNodeIds, (int)(browsePaths.Count - i))));
                    var relativePathsRange = new List<String>(relativePaths).GetRange((int)i, Math.Min((int)maxNodesPerTranslateBrowsePathsToNodeIds, (int)(relativePaths.Length - i)));

                    // make the call to the server.
                    BrowsePathResultCollection results;
                    DiagnosticInfoCollection diagnosticInfos;

                    try
                    {
                        ResponseHeader responseHeader = session.TranslateBrowsePathsToNodeIds(
                            null,
                            browsePathsRange,
                            out results,
                            out diagnosticInfos);

                        // ensure that the server returned valid results.
                        Session.ValidateResponse(results, browsePathsRange);
                        Session.ValidateDiagnosticInfos(diagnosticInfos, browsePathsRange);

                        // collect the list of node ids found.
                        for (int ii = 0; ii < results.Count; ii++)
                        {
                            // check if the start node actually exists.
                            if (StatusCode.IsBad(results[ii].StatusCode))
                            {
                                ServiceResult error = new ServiceResult(
                                    results.Count > ii ? results[ii].StatusCode : StatusCodes.BadUnexpectedError,
                                    diagnosticInfos.Count > ii ? diagnosticInfos[ii] : null,
                                    responseHeader.StringTable);

                                LastMessage = String.Format("{0} - {1} : {2} {3}", Title, Properties.Resource.OPCUASessionPathNotValid, relativePathsRange[ii], error);
                                continue;
                            }

                            // an empty list is returned if no node was found.
                            if (results[ii].Targets.Count == 0)
                            {
                                LastMessage = String.Format("{0} - {1} : {2}", Title, Properties.Resource.OPCUASessionPathDoesNotExist, relativePathsRange[ii]);
                                continue;
                            }

                            // Multiple matches are possible, however, the node that matches the type model is the
                            // one we are interested in here. The rest can be ignored.
                            BrowsePathTarget target = results[ii].Targets[0];

                            if (target.RemainingPathIndex != UInt32.MaxValue)
                            {
                                LastMessage = String.Format("{0} - {1} : {2}", Title, Properties.Resource.OPCUASessionPathOtherServer, relativePathsRange[ii]);
                                continue;
                            }

                            nodes.Add(relativePathsRange[ii], target.TargetId);
                        }
                    }
                    catch (Exception ex)
                    {
                        LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUATraslateBrowsePathFailed, ex.Message);
                    }
                }
            }
            else
            {
                // make the call to the server.
                BrowsePathResultCollection results;
                DiagnosticInfoCollection diagnosticInfos;

                try
                {
                    ResponseHeader responseHeader = session.TranslateBrowsePathsToNodeIds(
                        null,
                        browsePaths,
                        out results,
                        out diagnosticInfos);

                    // ensure that the server returned valid results.
                    Session.ValidateResponse(results, browsePaths);
                    Session.ValidateDiagnosticInfos(diagnosticInfos, browsePaths);

                    // collect the list of node ids found.
                    for (int ii = 0; ii < results.Count; ii++)
                    {
                        // check if the start node actually exists.
                        if (StatusCode.IsBad(results[ii].StatusCode))
                        {
                            ServiceResult error = new ServiceResult(
                                results.Count > ii ? results[ii].StatusCode : StatusCodes.BadUnexpectedError,
                                diagnosticInfos.Count > ii ? diagnosticInfos[ii] : null,
                                responseHeader.StringTable);

                            LastMessage = String.Format("{0} - {1} : {2} {3}", Title, Properties.Resource.OPCUASessionPathNotValid, relativePaths[ii], error);
                            continue;
                        }

                        // an empty list is returned if no node was found.
                        if (results[ii].Targets.Count == 0)
                        {
                            LastMessage = String.Format("{0} - {1} : {2}", Title, Properties.Resource.OPCUASessionPathDoesNotExist, relativePaths[ii]);
                            continue;
                        }

                        // Multiple matches are possible, however, the node that matches the type model is the
                        // one we are interested in here. The rest can be ignored.
                        BrowsePathTarget target = results[ii].Targets[0];

                        if (target.RemainingPathIndex != UInt32.MaxValue)
                        {
                            LastMessage = String.Format("{0} - {1} : {2}", Title, Properties.Resource.OPCUASessionPathOtherServer, relativePaths[ii]);
                            continue;
                        }

                        nodes.Add(relativePaths[ii], target.TargetId);
                    }
                }
                catch (Exception ex)
                {
                    LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUATraslateBrowsePathFailed, ex.Message);
                }
            }

            // return whatever was found.
            return nodes;
        }
#endregion

#region Properties

        public String AppTitle
        {
            get
            {
                //ApplicationDescriptionViewModel ap = Parent.Parent as ApplicationDescriptionViewModel;
                //if (ap != null)
                //    return ap.ApplicationName.ToString();

                try
                {
                    var epd = Parent as EndpointDescriptionViewModel;
                    if (epd != null)
                        return String.Format("{0} {1}",
                            epd.endpointDescription.Server.ApplicationName.ToString(),
                            epd.DisplayTitle);
                }
                catch
                {

                }

                return Title;
            }
        }

        public String HostName
        {
            get
            {
                var ap = Parent.Parent as ApplicationDescriptionViewModel;
                if (ap != null)
                    return ap.Workstation;
                return String.Empty;
            }
        }

        public String AppName
        {
            get
            {
                var ap = Parent.Parent as ApplicationDescriptionViewModel;
                if (ap != null)
                    return ap.Title;
                var ep = Parent as EndpointDescriptionViewModel;
                if (ep != null)
                    return ep.endpointDescription.Server.ApplicationName.ToString();

                return Title;
            }
        }

        UserTokenType _userTokenType = UserTokenType.Anonymous;
        public UserTokenType UserTokenType
        {
            get
            {
                return _userTokenType;
            }

            set
            {
                if (value == _userTokenType)
                    return;

                _userTokenType = value;
                OnPropertyChanged("UserTokenType");
            }
        }

        bool _acceptUntrustedCertificates = true;
        public bool AcceptUntrustedCertificates
        {
            get
            {
                return _acceptUntrustedCertificates;
            }
            set
            {
                if (_acceptUntrustedCertificates == value)
                    return;
                _acceptUntrustedCertificates = value;
            }
        }

        private bool _Connected;
        public bool Connected
        {
            get
            {
                return _Connected;
            }
            internal set
            {
                if (value == _Connected || bDisposed)
                    return;

                if (value == true)
                    bConnecting = false;
                else
                    AutoConnect = false;

                if (value == true && DataTypeFetched == false)
                    PromoteIdleExecution(2000);
                if (value == false && DataTypeFetched == true)
                    DataTypeFetched = false;

                var list = new List<SubscriptionViewModel>();
                bool hasChildren = HasChildren;
                lock (lockObject)
                {
                    if (hasChildren)
                    {
                        foreach (var v in Children)
                            list.Add(v as SubscriptionViewModel);
                    }
                }
                list.ForEach(svm =>
                    {
                        svm.UpdateMonitoredItemNotConnected(value ? StatusCodes.UncertainLastUsableValue : StatusCodes.BadNotConnected);
                    });

                bool bFetchDataType = false;
                lock (lockObject)
                {
                    if (value == true && !_DataTypeFetching && session != null)
                    {
                        bFetchDataType = true;
                        _DataTypeFetching = true;
                    }
                }

                if (bFetchDataType)
                {
                    LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUASessionFetchingReferencesTypeTree);

                    // need to fetch the references in order to parse relative paths.
                    session.FetchTypeTree(ReferenceTypes.References);
                    _DataTypeFetching = false;
                    DataTypeFetched = true;
                }

                _Connected = value;
                OnPropertyChanged("Connected");

                // Mediator.NotifyColleagues<SessionViewModel>("SessionConnectionChanged", this);
            }
        }

        bool _DataTypeFetching;
        bool _DataTypeFetched;
        public bool DataTypeFetched
        {
            get
            {
                return _DataTypeFetched;
            }
            set
            {
                if (value == _DataTypeFetched)
                    return;

                _DataTypeFetched = true;
                OnPropertyChanged("DataTypeFetched");
            }
        }

        bool _EnableDiagnostic;
        public bool EnableDiagnostic
        {
            get
            {
                return _EnableDiagnostic;
            }
            set
            {
                if (value == _EnableDiagnostic)
                    return;

                _EnableDiagnostic = true;
                OnPropertyChanged("EnableDiagnostic");
            }
        }

        private bool _AutoConnect = false;
        public bool AutoConnect
        {
            get
            {
                return _AutoConnect;
            }
            set
            {
                if (value == _AutoConnect)
                    return;

                _AutoConnect = value;
                OnPropertyChanged("AutoConnect");
            }
        }

        private ServerState _CurrentState = ServerState.Unknown;
        public ServerState CurrentState
        {
            get
            {
                return _CurrentState;
            }
            set
            {
                if (value == _CurrentState)
                    return;

                _CurrentState = value;
                OnPropertyChanged("CurrentState");
            }
        }

        private ConfiguredEndpoint _endPoint;
        public ConfiguredEndpoint endPoint
        {
            get
            {
                return _endPoint;
            }
            set
            {
                if (value == _endPoint)
                    return;

                _endPoint = value;
                OnPropertyChanged("endPoint");
            }
        }

#if !NET_STANDARD
        private BindingFactory _bindingFactory;
        public BindingFactory bindingFactory
        {
            get
            {
                return _bindingFactory;
            }
            set
            {
                if (value == _bindingFactory)
                    return;

                _bindingFactory = value;
                OnPropertyChanged("bindingFactory");
            }
        }
#endif

        private ApplicationConfiguration _configuration;
        public ApplicationConfiguration configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                if (value == _configuration)
                    return;

                _configuration = value;
                OnPropertyChanged("configuration");
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (String.Compare(value, _Name, false) == 0)
                    return;

                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public DiagnosticsMasks ReturnDiagnostics
        {
            get
            {
                return session.ReturnDiagnostics;
            }
            set
            {
                if (value == session.ReturnDiagnostics)
                    return;

                session.ReturnDiagnostics = value;
                OnPropertyChanged("ReturnDiagnostics");
            }
        }

        public Session Session
        {
            get
            {
                return session;
            }
        }

        public Subscription DefaultSubscription
        {
            get
            {
                return session.DefaultSubscription;
            }
            set
            {
                if (value == session.DefaultSubscription)
                    return;

                session.DefaultSubscription = value;
                OnPropertyChanged("DefaultSubscription");
            }
        }

        public int DefunctRequestCount
        {
            get
            {
                return session.DefunctRequestCount;
            }
        }

        public int GoodPublishRequestCount
        {
            get
            {
                return session.GoodPublishRequestCount;
            }
        }

        public int KeepAliveInterval
        {
            get
            {
                return session.KeepAliveInterval;
            }
            set
            {
                if (value == session.KeepAliveInterval)
                    return;

                session.KeepAliveInterval = value;
                OnPropertyChanged("KeepAliveInterval");
            }
        }

        public DateTime _lastCurrentTime;
        public DateTime LastCurrentTime
        {
            get
            {
                return _lastCurrentTime;
            }
            set
            {
                if (value == _lastCurrentTime)
                    return;

                _lastCurrentTime = value;
                OnPropertyChanged("LastCurrentTime");
            }
        }

        public bool KeepAliveStopped
        {
            get
            {
                return session.KeepAliveStopped;
            }
        }

        public int OutstandingRequestCount
        {
            get
            {
                return session.OutstandingRequestCount;
            }
        }

        public StringCollection PreferredLocales
        {
            get
            {
                return session.PreferredLocales;
            }
        }

        public StringTable ServerUris
        {
            get
            {
                return session.ServerUris;
            }
        }

        public NamespaceTable NamespaceUris
        {
            get
            {
                return session.NamespaceUris;
            }
        }

        public String SessionName
        {
            get
            {
                return session.SessionName;
            }
        }

        public Double SessionTimeout
        {
            get
            {
                return session.SessionTimeout;
            }
        }

        public int SubscriptionCount
        {
            get
            {
                return session.SubscriptionCount;
            }
        }

        bool bDisposed;
        public bool Disposed
        {
            get 
            { 
                return bDisposed; 
            }
        }

        ReferenceDescriptionCollection _viewDescriptions;
        public ReferenceDescriptionCollection ViewDescriptions
        {
            get
            {
                BrowseDescription nodeToBrowse = new BrowseDescription();

                nodeToBrowse.NodeId = Opc.Ua.ObjectIds.ViewsFolder;
                nodeToBrowse.ReferenceTypeId = Opc.Ua.ReferenceTypeIds.HierarchicalReferences;
                nodeToBrowse.IncludeSubtypes = true;
                nodeToBrowse.BrowseDirection = BrowseDirection.Forward;
                nodeToBrowse.NodeClassMask = 0;
                nodeToBrowse.ResultMask = (uint)BrowseResultMask.All;

                return Browse(Session, nodeToBrowse, false);
            }
        }

#endregion

#region Browse Helpers

        /// <summary>
        /// Browses the address space and returns the references found.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="nodeToBrowse">The NodeId for the starting node.</param>
        /// <param name="throwOnError">if set to <c>true</c> a exception will be thrown on an error.</param>
        /// <returns>
        /// The references found. Null if an error occurred.
        /// </returns>
        public static ReferenceDescriptionCollection Browse(Session session, BrowseDescription nodeToBrowse, bool throwOnError)
        {
            return Browse(session, null, nodeToBrowse, throwOnError);
        }

        /// <summary>
        /// Browses the address space and returns the references found.
        /// </summary>
        public static ReferenceDescriptionCollection Browse(Session session, ViewDescription view, BrowseDescription nodeToBrowse, bool throwOnError)
        {
            try
            {
                ReferenceDescriptionCollection references = new ReferenceDescriptionCollection();

                // construct browse request.
                BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
                nodesToBrowse.Add(nodeToBrowse);

                // start the browse operation.
                BrowseResultCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                session.Browse(
                    null,
                    view,
                    0,
                    nodesToBrowse,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, nodesToBrowse);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);

                do
                {
                    // check for error.
                    if (StatusCode.IsBad(results[0].StatusCode))
                    {
                        throw new ServiceResultException(results[0].StatusCode);
                    }

                    // process results.
                    for (int ii = 0; ii < results[0].References.Count; ii++)
                    {
                        references.Add(results[0].References[ii]);
                    }

                    // check if all references have been fetched.
                    if (results[0].References.Count == 0 || results[0].ContinuationPoint == null)
                    {
                        break;
                    }

                    // continue browse operation.
                    ByteStringCollection continuationPoints = new ByteStringCollection();
                    continuationPoints.Add(results[0].ContinuationPoint);

                    session.BrowseNext(
                        null,
                        false,
                        continuationPoints,
                        out results,
                        out diagnosticInfos);

                    ClientBase.ValidateResponse(results, continuationPoints);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);
                }
                while (true);

                //return complete list.
                return references;
            }
            catch (Exception exception)
            {
                if (throwOnError)
                {
                    throw new ServiceResultException(exception, StatusCodes.BadUnexpectedError);
                }

                return null;
            }
        }

        /// <summary>
        /// Browses the address space and returns all of the supertypes of the specified type node.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="typeId">The NodeId for a type node in the address space.</param>
        /// <param name="throwOnError">if set to <c>true</c> a exception will be thrown on an error.</param>
        /// <returns>
        /// The references found. Null if an error occurred.
        /// </returns>
        public static ReferenceDescriptionCollection BrowseSuperTypes(Session session, NodeId typeId, bool throwOnError)
        {
            ReferenceDescriptionCollection supertypes = new ReferenceDescriptionCollection();

            try
            {
                // find all of the children of the field.
                BrowseDescription nodeToBrowse = new BrowseDescription();

                nodeToBrowse.NodeId = typeId;
                nodeToBrowse.BrowseDirection = BrowseDirection.Inverse;
                nodeToBrowse.ReferenceTypeId = ReferenceTypeIds.HasSubtype;
                nodeToBrowse.IncludeSubtypes = false; // more efficient to use IncludeSubtypes=False when possible.
                nodeToBrowse.NodeClassMask = 0; // the HasSubtype reference already restricts the targets to Types. 
                nodeToBrowse.ResultMask = (uint)BrowseResultMask.All;

                ReferenceDescriptionCollection references = Browse(session, nodeToBrowse, throwOnError);

                while (references != null && references.Count > 0)
                {
                    // should never be more than one supertype.
                    supertypes.Add(references[0]);

                    // only follow references within this server.
                    if (references[0].NodeId.ServerIndex > 0)
                    {
                        break;
                    }

                    // get the references for the next level up.
                    nodeToBrowse.NodeId = (NodeId)references[0].NodeId;
                    references = Browse(session, nodeToBrowse, throwOnError);
                }

                // return complete list.
                return supertypes;
            }
            catch (Exception exception)
            {
                if (throwOnError)
                {
                    throw new ServiceResultException(exception, StatusCodes.BadUnexpectedError);
                }

                return null;
            }
        }

        #endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region Validations

        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }
#endregion
#endif

#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                //BitmapImage bm = new BitmapImage();
                //bm.BeginInit();
                //Assembly assembly = Assembly.GetExecutingAssembly();

                //String str = String.Format("pack://application:,,,/{0};component/Images/effects_16x16.png",
                //    Path.GetFileNameWithoutExtension(assembly.Location));
                //bm.UriSource = new Uri(str);
                //bm.EndInit();
                var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, $"OPCUAVMEffects", false);

                return bm;
#else
                return null;
#endif
            }
        }

        public Object Tooltip
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                return new OPCUAViewModel.UserControls.SessionViewModel();
#else
                return null;
#endif
            }
        }

        public ImageSource ExpandedImageSource { get { return null; } }
        public ContextMenu contextMenu { get { return null; } }

        public object ContainedObject
        {
            get
            {
                return session;
            }
        }

        public object EntityParent
        {
            get
            {
                return Parent;
            }
        }

        public String TypeDefinitionString
        {
            get { return null; }
        }
#endregion
    }
}
