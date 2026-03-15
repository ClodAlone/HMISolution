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
using System.Runtime.Serialization;
using Opc.Ua.Security;

namespace Opc.Ua.GdsLocalAgent
{
    [DataContract(Namespace=Namespaces.OpcUaGds)]
    public class GdsAgentApplication
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string UniqueId { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public ConfigurationType ConfigurationType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public string MachineName { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 4)]
        public string SubjectName { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 6)]
        public bool RegisteredWithGds { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 6)]
        public bool GdsSynchronizationEnabled { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 7)]
        public NodeId LastCertificateRequestId { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 8)]
        public DateTime LastSynchronizationTime { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 11)]
        public string ImportExportUtility { get; set; }
        
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 12)]
        public string ImportArguments { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 13)]
        public string ExportArguments { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 14)]
        public string PublicKeyFilePath { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 15)]
        public string PrivateKeyFilePath { get; set; }

        /// <summary>
        /// The name of the application.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                if (SecuritySettings != null)
                {
                    return SecuritySettings.ApplicationName;
                }

                return null;
            }

            set
            {
                if (SecuritySettings == null)
                {
                    SecuritySettings = new SecuredApplication();
                }

                SecuritySettings.ApplicationName = value;
            }
        }

        /// <summary>
        /// The configuration file for the application.
        /// </summary>
        public string ConfigurationFile
        {
            get
            {
                if (SecuritySettings != null)
                {
                    return SecuritySettings.ConfigurationFile;
                }

                return null;
            }

            set
            {
                if (SecuritySettings == null)
                {
                    SecuritySettings = new SecuredApplication();
                }

                SecuritySettings.ConfigurationFile = value;
            }
        }

        /// <summary>
        /// The executable file for the application.
        /// </summary>
        public string ExecutableFile
        {
            get
            {
                if (SecuritySettings != null)
                {
                    return SecuritySettings.ExecutableFile;
                }

                return null;
            }

            set
            {
                if (SecuritySettings == null)
                {
                    SecuritySettings = new SecuredApplication();
                }

                SecuritySettings.ExecutableFile = value;
            }
        }

        /// <summary>
        /// The security settings for the application.
        /// </summary>
        public SecuredApplication SecuritySettings
        {
            get
            {
                return m_securitySettings;
            }

            set
            {
                if (value != null && m_securitySettings != null)
                {
                    if (String.IsNullOrEmpty(value.ConfigurationFile))
                    {
                        value.ConfigurationFile = m_securitySettings.ConfigurationFile;
                    }

                    if (String.IsNullOrEmpty(value.ExecutableFile))
                    {
                        value.ExecutableFile = m_securitySettings.ExecutableFile;
                    }
                }

                m_securitySettings = value;
            }
        }

        private SecuredApplication m_securitySettings;
    }

    [DataContract(Namespace = Namespaces.OpcUaGds)]
    public enum ConfigurationType
    {
        [EnumMember()]
        Xml,
        
        [EnumMember()]
        ImportExport,

        [EnumMember()]
        Manual
    }
}
