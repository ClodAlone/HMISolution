using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ADFirebase
{
    internal class FCMReceiver
    {
        const string RTDB_PREFIX = "RTDB:";
        const string FSDB_PREFIX = "FSDB:";

        public string PathToToken { get; private set; }

        public string Url { get; private set; }

        public bool IsRealtime { get; private set; } = false;

        public bool IsFirestore { get; private set; } = false;

        public bool IsValid { get; private set; } = true;

        public FCMReceiver(ADFirebaseNotificationSender context, string s)
        {
            if (s.StartsWith(RTDB_PREFIX))
            {
                PathToToken = s.Substring(RTDB_PREFIX.Length);
                IsRealtime = true;
                Url = $"{context.RealtimeDatabaseUrl}/{PathToToken}/token.json";
                if (context.UseAccessToken)
                    Url += $"?access_token={context.TokenResponse.AccessToken}";
            }
            else if (s.StartsWith(FSDB_PREFIX))
            {
                PathToToken = s.Substring(FSDB_PREFIX.Length);
                IsFirestore = true;
                Url = $"https://firestore.googleapis.com/v1/projects/{context.ProjectID}/databases/(default)/documents/{PathToToken}?mask.fieldPaths=token";
                if (context.UseAccessToken)
                    Url += $"&access_token={context.TokenResponse.AccessToken}";
            }
            else
            {
                PathToToken = s;
                IsValid = false;
            }
        }

        public async Task<(bool Success, string Token, string Error)> TryDeserializeHttpContent(HttpContent httpContent)
        {
            var responseBody = await httpContent.ReadAsStringAsync();

            try
            {
                if (IsRealtime)
                {
                    var token = Newtonsoft.Json.JsonConvert.DeserializeObject<String>(responseBody);
                    if (string.IsNullOrEmpty(token))
                        return (false, null, String.Format(Properties.Resources.ErrorTokenNotFound, PathToToken));
                    else
                        return (true, token, null);
                }
                else if (IsFirestore)
                {
                    var doc = Newtonsoft.Json.JsonConvert.DeserializeObject<FSDoc>(responseBody);
                    if (!(doc is null) && !(doc.fields is null) && !(doc.fields.token is null) && !(doc.fields.token.stringValue is null))
                        return (true, doc.fields.token.stringValue, null);
                    else
                        throw new Exception("response wrong format");
                }
                else
                {
                    return (false, null, null);
                }
            }
            catch (Exception e)
            {
                return (false, null, $"Error: {e.Message}. Body: {responseBody}");
            }
        }
    }

    class FSDoc
    {
#pragma warning disable IDE1006 // Naming Styles
        public FSFields fields { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    class FSFields
    {
#pragma warning disable IDE1006 // Naming Styles
        public FSString token { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    class FSString
    {
#pragma warning disable IDE1006 // Naming Styles
        public string stringValue { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
