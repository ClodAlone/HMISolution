// ---------------------------------------------------------------------------
// Campari Software
//
// InternetInformationServicesComponent.cs
//
//
// ---------------------------------------------------------------------------
// Copyright (C) 2006-2007 Campari Software
// All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
// ---------------------------------------------------------------------------
using System;

namespace Campari.Software
{
    #region enum InternetInformationServicesComponent
    /// <summary>
    /// Specifies the Internet Information Services (IIS) versions
    /// </summary>
    public enum InternetInformationServicesComponent
    {
        /// <summary>
        /// Internet Information Services Common Files
        /// </summary>
        Common,

        /// <summary>
        /// Active Server Pages (ASP) for Internet Information Services
        /// </summary>
        ASP,

        /// <summary>
        /// File Transfer Protocol (FTP) service
        /// </summary>
        FTP,

        /// <summary>
        /// Internet Information Services Manager
        /// </summary>
        InetMgr,

        /// <summary>
        /// Internet Data Connector
        /// </summary>
        InternetDataConnector,

        /// <summary>
        /// Network News Transfer Protocl (NNTP) service
        /// </summary>
        NNTP,

        /// <summary>
        /// Server-Side Includes
        /// </summary>
        ServerSideIncludes,

        /// <summary>
        /// Simple Mail Transfer Protocol (SMTP) service
        /// </summary>
        SMTP,

        /// <summary>
        /// Web Distributed Authoring and Versioning (WebDAV) publishing
        /// </summary>
        WebDAV,

        /// <summary>
        /// World Wide Web (WWW) service
        /// </summary>
        WWW,

        /// <summary>
        /// Remote administration (HTML)
        /// </summary>
        RemoteAdmin,

        /// <summary>
        /// Internet Server Application Programming Interface (ISAPI) for
        /// Background Intelligent Transfer Service (BITS) server extensions
        /// </summary>
        BitsISAPI,

        /// <summary>
        /// Background Intelligent Transfer Service (BITS) server extensions
        /// </summary>
        Bits,

        /// <summary>
        /// FrontPage server extensions
        /// </summary>
        FrontPageExtensions,

        /// <summary>
        /// Internet printing
        /// </summary>
        InternetPrinting,

        /// <summary>
        /// ActiveX control and sample pages for hosting Terminal Services
        /// client connections over the web
        /// </summary>
        TSWebClient,
    }
    #endregion
}
