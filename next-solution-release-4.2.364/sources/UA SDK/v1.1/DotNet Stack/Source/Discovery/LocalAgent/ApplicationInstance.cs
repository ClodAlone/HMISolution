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

namespace Opc.Ua.GdsLocalAgent
{
    internal class ApplicationInstance : Opc.Ua.Configuration.ApplicationInstance
    {
        /// <summary>
        /// Creates the default access templates.
        /// </summary>
        protected override void Install(bool silent, Dictionary<string, string> args)
        {
            base.Install(silent, args);
                        
            // install the default roles for the GDS agent.
            string accessTemplatePath = Utils.GetAbsoluteDirectoryPath("%CommonApplicationData%\\OPC Foundation\\GDS\\Access Templates", false, false, true);

            if (accessTemplatePath != null)
            {
                AccessTemplateManager.CreateTemplate(accessTemplatePath, "Administrators");
                AccessTemplateManager.CreateTemplate(accessTemplatePath, "Users", WellKnownSidType.BuiltinUsersSid);
                AccessTemplateManager.CreateTemplate(accessTemplatePath, "System", WellKnownSidType.LocalSystemSid);
                AccessTemplateManager.CreateTemplate(accessTemplatePath, "SystemOrUsers", WellKnownSidType.BuiltinUsersSid, WellKnownSidType.LocalSystemSid);
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
                // install the default roles for the GDS agent.
                string accessTemplatePath = Utils.GetAbsoluteDirectoryPath("%CommonApplicationData%\\OPC Foundation\\GDS\\Access Templates", false, false, true);

                if (accessTemplatePath != null)
                {
                    AccessTemplateManager.DeleteTemplate(accessTemplatePath, "Administrators");
                    AccessTemplateManager.DeleteTemplate(accessTemplatePath, "Users");
                    AccessTemplateManager.DeleteTemplate(accessTemplatePath, "System");
                    AccessTemplateManager.DeleteTemplate(accessTemplatePath, "SystemOrUsers");
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error deleting user role files.");
            }
        }
    }
}
