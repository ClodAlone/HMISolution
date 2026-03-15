using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Util;

namespace ADFirebase
{
    internal class FCMClient
    {
        #region fields
        private readonly ADFirebaseNotificationSender _context;
        #endregion

        #region ctor
        public FCMClient(ADFirebaseNotificationSender context, string jsonFileOnDB)
        {
            _context = context;
            
            if (FirebaseApp.DefaultInstance is null)
            {
                var isDbProject = XpoHelpers.XpoHelper.IsSQlDataProvider(_context.conn);

                if (!File.Exists(_context.serviceAccountKeyFilePath) && !isDbProject)
                {
                    throw new Exception("Service Account Key file missing.");
                }

                GoogleCredential credential;

                try
                {
                    if(isDbProject)
                    {
                        credential = GoogleCredential.FromJson(jsonFileOnDB);
                    }
                    else
                        credential = GoogleCredential.FromFile(_context.serviceAccountKeyFilePath);
                }
                catch (Exception e)
                {
                    if ((e.InnerException != null) && (e.InnerException.InnerException != null) &&
                        e.InnerException.InnerException.Message.Contains("Newtonsoft"))
                        e = e.InnerException.InnerException;

                    throw new Exception($"Error on creating Google credential: {e.Message}");
                }

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = credential
                });
            }

            if (_context.UseAccessToken)
            {
                if ((_context.TokenResponse is null) || _context.TokenResponse.IsExpired(SystemClock.Default))
                {
                    var initializer = new ServiceAccountCredential.Initializer(_context.credentialParameters["client_email"])
                    {
                        ProjectId = _context.credentialParameters["project_id"],
                        KeyId = _context.credentialParameters["private_key_id"]
                    };

                    initializer.Scopes = Settings.Default.scopes.Split('|');

                    var serviceAccountCredential = new ServiceAccountCredential(initializer.FromPrivateKey(_context.credentialParameters["private_key"]));
                    _ = serviceAccountCredential.GetAccessTokenForRequestAsync(_context.credentialParameters["auth_uri"]).Result;
                    _context.TokenResponse = serviceAccountCredential.Token;
                }
            }
        }
        #endregion

        #region Methods
        public async Task SendTextMessageAsync(string fcmTokenPath, string text, Dictionary<string, string> data)
        {
            var receiver = new FCMReceiver(_context, fcmTokenPath);
            if (!receiver.IsValid) throw new TokenException(String.Format(Properties.Resources.InvalidTokenPath,receiver.PathToToken));

            var (Tokens, Errors) = RetrieveTokens(new List<FCMReceiver>() { receiver });

            foreach (string error in Errors)
            {
                _context.OnSystemEvent(null, error, Opc.Ua.EventSeverity.High);
            }

            if (Tokens.Any())
            {
                var message = new Message()
                {
                    Data = data,
                    Token = Tokens.Single(),
                    Notification = new Notification()
                    {
                        Body = text,
                    },
                };

                if (!string.IsNullOrEmpty(_context.NotificationsTitle))
                {
                    message.Notification.Title = _context.NotificationsTitle;
                }
                _ = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
        }

        public async Task SendTextMulticastMessageAsync(List<string> fcmTokenPaths, string text, Dictionary<string, string> data)
        {
            var receivers = fcmTokenPaths.Select(t => new FCMReceiver(_context, t)).ToList();
            foreach (var invalidFCMReceiver in receivers.Where(r => !r.IsValid))
            {
                _context.OnSystemEvent(null, String.Format(Properties.Resources.InvalidTokenPath, invalidFCMReceiver.PathToToken), Opc.Ua.EventSeverity.High);
            }

            var (Tokens, Errors) = RetrieveTokens(receivers.Where(r => r.IsValid).ToList());

            foreach (string error in Errors)
            {
                _context.OnSystemEvent(null, error, Opc.Ua.EventSeverity.High);
            }

            if (Tokens.Any())
            {
                var message = new MulticastMessage()
                {
                    Data = data,
                    Tokens = Tokens,
                    Notification = new Notification()
                    {
                        Body = text,
                    },
                };

                if (!string.IsNullOrEmpty(_context.NotificationsTitle))
                {
                    message.Notification.Title = _context.NotificationsTitle;
                }

                _ = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            }
        }

        public (List<string> Tokens, List<string> Errors) RetrieveTokens(List<FCMReceiver> receivers)
        {
            if (!receivers.Any(r => !string.IsNullOrEmpty(r.PathToToken)))
                throw new TokenException("No notification receivers");

            var tokens = new List<string>();
            var errors = new List<string>();

            try
            {
                var responses = Task.WhenAll(receivers.Select(r => _context.httpClient.GetAsync(r.Url)).ToArray()).Result;
                errors.AddRange(responses.Where(r => !r.IsSuccessStatusCode).Select((r, i) => string.Format(Properties.Resources.BadResponsePath,receivers[i].PathToToken,r.StatusCode)));
                var results = Task.WhenAll(responses.Where(r => r.IsSuccessStatusCode).Select((r, i) => receivers[i].TryDeserializeHttpContent(r.Content))).Result;
                errors.AddRange(results.Where(r => !r.Success).Select(r => r.Error));
                tokens.AddRange(results.Where(r => r.Success).Select(r => r.Token));
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
            }

            if (!tokens.Any())
                throw new TokenException("Zero token retrieved");

            return (tokens, errors);
        }

        #endregion
    }

    class TokenException : Exception
    {
        public TokenException(string message) : base(message) { }
    }

}
