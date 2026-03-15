/* ========================================================================
 * Copyright (c) 2005-2009 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community Binary License ("RCBL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community Binary License ("RCBL") Version 1.00, or subsequent versions 
 * as allowed by the RCBL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCBL.
 * 
 * All software distributed under the RCBL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCBL for specific 
 * language governing rights and limitations under the RCBL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCBL/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ComponentModel;

using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;

namespace Opc.Ua.Configuration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
                
            // check if running in command line mode.
            string[] args = Environment.GetCommandLineArgs();
            
            try
            {
                if (args.Length > 1)
                {
                    if (ConfigUtils.ProcessCommandLine())
                    {
                        return;
                    }
                }

                ApplicationConfiguration configuration = GuiUtils.DoStartupChecks(
                    "Opc.Ua.ConfigurationTool", 
                    ApplicationType.Client,
                    "%CommonApplicationData%\\OPC Foundation\\Config\\Opc.Ua.ConfigurationTool.Config.xml",
                    false);

                if (configuration != null)
                {
                    Application.Run(new MainForm(configuration));
                }
            }
            catch (Exception e)
            {
                GuiUtils.HandleException(Utils.Format(
                    "UA Certificate Tool: {0} {1}", 
                    (args.Length > 1)?args[1]:null,                    
                    (args.Length > 2)?args[2]:null), 
                    null,
                    e);
            }
        }
    }
}
