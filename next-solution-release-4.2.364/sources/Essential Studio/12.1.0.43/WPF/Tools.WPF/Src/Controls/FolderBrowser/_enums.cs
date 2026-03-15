// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Enum of CSIDLs identifying standard shell folders.
    /// </summary>
    
    public enum LocationID
    {
        /// <summary>
        /// The virtual folder representing the Windows desktop, the root of the namespace.
        /// </summary>
        Desktop = 0x0000,
        
        /// <summary>
        /// The virtual folder containing installed printers.
        /// </summary>
        Printers = 0x0004,
        
        /// <summary>
        /// The virtual folder representing the My Documents desktop item.
        /// </summary>
        MyDocuments = 0x0005,
        
        /// <summary>
        /// The file system directory that serves as a common repository for favorite items common to all users. Valid only for Windows NT systems.
        /// </summary>
        Favorites = 0x0006,
        
        /// <summary>
        /// The file system directory that serves as a common repository for the user's favorite items.
        /// </summary>
        Recent = 0x0008,
        
        /// <summary>
        /// The file system directory that contains Send To menu items. 
        /// </summary>
        SendTo = 0x0009,
        
        /// <summary>
        /// The file system directory containing Start menu items. 
        /// </summary>
        StartMenu = 0x000b,
        
        /// <summary>
        /// The virtual folder representing My Computer, containing everything on the local computer: storage devices, printers, and Control Panel. The folder may also contain mapped network drives.
        /// </summary>
        MyComputer = 0x0011,
        
        /// <summary>
        /// A virtual folder representing Network Neighborhood, the root of the network namespace hierarchy.
        /// </summary>
        NetworkNeighborhood = 0x0012,
        
        /// <summary>
        /// The file system directory that serves as a common repository for document templates.
        /// </summary>
        Templates = 0x0015,
        
        /// <summary>
        /// The file system directory that serves as a common repository for image files. 
        /// </summary>
        MyPictures = 0x0027,
        
        /// <summary>
        /// The virtual folder representing Network Connections, containing network and dial-up connections. 
        /// </summary>
        NetAndDialUpConnections = 0x0031,
        
        /// <summary>
        /// The Program Files folder.
        /// </summary>
        ProgramFiles = 0x0026,
        
        /// <summary>
        /// The Windows directory or SYSROOT. This corresponds to the %windir% or %SYSTEMROOT% environment variables.
        /// </summary>
        Windows = 0x0024,
        
        /// <summary>
        /// If set to Custom RootPath should be specified.
        /// </summary>
        Custom
    }
}
