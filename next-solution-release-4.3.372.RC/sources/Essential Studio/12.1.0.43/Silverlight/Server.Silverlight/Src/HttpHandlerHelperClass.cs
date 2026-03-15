#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

/// <summary>
/// Main class of the control.
/// </summary>
namespace Syncfusion.Silverlight.Server.WebHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Represents the class that handles the Request and Response for uploading
    /// </summary>
    public class HttpHandlerHelperClass
    {
        /// <summary>
        /// Method that handles the Request for uploading process
        /// </summary>
        /// <param name="httpcontext">Represents the HttpContext</param>
        /// <param name="uploadfilepath">Represents the path of the file</param>
        public void HandleRequest(HttpContext httpcontext, string uploadfilepath)
        {
            string remove = string.Empty;
            string filepath = string.Empty;
            bool status;
            bool havebytes;
            long bytestoupload;
            string filename = httpcontext.Request.QueryString["filename"];

            if (httpcontext.Request.QueryString["status"] == null)
            {
                status = true;
            }
            else
            {
                status = bool.Parse(httpcontext.Request.QueryString["status"]);
            }

            if (httpcontext.Request.QueryString["havebytes"] == null)
            {
                havebytes = false;
            }
            else
            {
                havebytes = bool.Parse(httpcontext.Request.QueryString["havebytes"]);
            }

            if (httpcontext.Request.QueryString["bytestoupload"] == null)
            {
                bytestoupload = 0;
            }
            else
            {
                bytestoupload = long.Parse(httpcontext.Request.QueryString["bytestoupload"]);
            }

            if (httpcontext.Request.QueryString["Remove"] == null)
            {
                remove = string.Empty;
            }
            else
            {
                remove = httpcontext.Request.QueryString["Remove"];
            }

            filepath = Path.Combine(uploadfilepath, filename);

            if (remove == "Remove")
            {
                string[] fileslist = Directory.GetFiles(uploadfilepath);
                foreach (string file in fileslist)
                {
                    FileInfo fileinfo = new FileInfo(file);
                    if (fileinfo.Name == filename)
                    {
                        fileinfo.Delete();
                    }
                }
            }
            else
            {
                if (havebytes)
                {
                    FileInfo fi = null;
                    try
                    {
                        fi = new FileInfo(filepath);
                    }
                    catch
                    {
                        throw new Exception("File Path is too long to handle");
                    }

                    if (!fi.Exists)
                    {
                        httpcontext.Response.Write("0");
                    }
                    else
                    {
                        httpcontext.Response.Write(fi.Length.ToString());
                    }

                    httpcontext.Response.Flush();
                    return;
                }
                else
                {
                    if (bytestoupload > 0 && File.Exists(filepath))
                    {
                        using (FileStream fs = File.Open(filepath, FileMode.Append))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesreceived = 0;
                            while ((bytesreceived = httpcontext.Request.InputStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                fs.Write(buffer, 0, bytesreceived);
                            }

                            fs.Close();
                        }
                    }
                    else
                    {
                        try
                        {
                            using (FileStream fs = File.Create(filepath))
                            {
                                byte[] buffer = new byte[4096];
                                int bytesread = 0;
                                while ((bytesread = httpcontext.Request.InputStream.Read(buffer, 0, buffer.Length)) != 0)
                                {
                                    fs.Write(buffer, 0, bytesread);
                                }

                                fs.Close();
                            }
                        }
                        catch
                        {
                            throw new Exception("File Path is too long to handle");
                        }
                    }
                }
            }
        }
    }
}