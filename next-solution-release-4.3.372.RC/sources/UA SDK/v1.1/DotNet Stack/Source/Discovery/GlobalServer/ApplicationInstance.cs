/* ========================================================================
 * Copyright (c) 2005-2011 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Opc.Ua.GdsServer
{
    internal class ApplicationInstance : Opc.Ua.Configuration.ApplicationInstance
    {
        /// <summary>
        /// Creates the default access templates.
        /// </summary>
        protected override void OnBeforeInstallService()
        {
            string protectedData = Utils.GetAbsoluteDirectoryPath(@"%CommonApplicationData%\OPC Foundation\GDS\ProtectedData\", false, false, true);

            if (protectedData != null)
            {
                AccessTemplateManager.SetPermissions(new DirectoryInfo(protectedData), FileSystemRights.ReadAndExecute | FileSystemRights.Write, true, WellKnownSidType.LocalSystemSid);
            }

            if (!String.IsNullOrEmpty(ApplicationConfiguration.SecurityConfiguration.UserRoleDirectory))
            {
                string directoryPath = Utils.GetAbsoluteDirectoryPath(ApplicationConfiguration.SecurityConfiguration.UserRoleDirectory, false, false, true);

                if (directoryPath != null)
                {
                    UserRoleManager.CreateRole(directoryPath, "GdsAdministrator");
                    UserRoleManager.CreateRole(directoryPath, "ApplicationAdministrator", WellKnownSidType.BuiltinPowerUsersSid);
                }
            }
        }

        /// <summary>
        /// Removes the default access templates.
        /// </summary>
        protected override void Uninstall(bool silent, Dictionary<string, string> args)
        {
            base.Uninstall(silent, args);

            try
            {
                if (ApplicationConfiguration != null)
                {
                    if (!String.IsNullOrEmpty(ApplicationConfiguration.SecurityConfiguration.UserRoleDirectory))
                    {
                        string directoryPath = Utils.GetAbsoluteDirectoryPath(ApplicationConfiguration.SecurityConfiguration.UserRoleDirectory, false, false, false);

                        if (directoryPath != null)
                        {
                            UserRoleManager.DeleteRole(directoryPath, "GdsAdministrator");
                            UserRoleManager.DeleteRole(directoryPath, "ApplicationAdministrator");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error deleting user role files.");
            }
        }
    }
}
