#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Text;
using System.IO;

namespace Syncfusion.Pdf.Security
{
    public class TimeStampServer
    {
        #region Fields
        private Uri m_server;
        private string m_username;
        private string m_password;
        private int m_timeOut;
        #endregion

        #region Properties
        public Uri Server
        {
            get
            {
                return m_server;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Server");

                m_server = value;
            }
        }

        public string UserName
        {
            get
            {
                return m_username;
            }
            set
            {
                m_username = value;
            }
        }

        public string Password
        {
            get
            {
                return m_password;
            }
            set
            {
                m_password = value;
            }
        }

        public int TimeOut
        {
            get
            {
                return m_timeOut;
            }
            set
            {
                m_timeOut = value;
            }
        }
        #endregion

        #region Constructors
        public TimeStampServer(Uri server)
        {
            if (server == null)
                throw new ArgumentNullException("Sever");

            m_server = server;
        }

        public TimeStampServer(Uri server, string username, string password)
            : this(server)
        {
            m_username = username;
            m_password = password;
        }

        public TimeStampServer(Uri server, string username, string password, int timeOut)
            : this(server, username, password)
        {
            m_timeOut = timeOut;
        }
        #endregion

        #region Implementation
        internal byte[] GetTimeStampResponse(byte[] request)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(m_server.ToString());
            webRequest.ContentLength = request.Length;
            webRequest.ContentType = "application/timestamp-query";
            webRequest.Method = "POST";
            if(!string.IsNullOrEmpty(m_username))
            {
                string authInfo = m_username + ":" + m_password;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                webRequest.Headers["Authorization"] = "Basic " + authInfo;
            }

            Stream outStream = webRequest.GetRequestStream();
            outStream.Write(request, 0, request.Length);
            outStream.Close();

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Server returned unexpected response code : "+response.StatusCode.ToString());

            Stream inputStream = response.GetResponseStream();
            MemoryStream stream = new MemoryStream();

            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, bytesRead);
            }

            inputStream.Close();
            response.Close();
            
            byte[] respBytes = stream.ToArray();

            String encoding = response.ContentEncoding;
            if (!string.IsNullOrEmpty(response.ContentEncoding) && response.ContentEncoding.Equals("base64", StringComparison.InvariantCultureIgnoreCase))
            {
                respBytes = Convert.FromBase64String(Encoding.ASCII.GetString(respBytes));
            }
            return respBytes;
        }
        #endregion
    }
}
