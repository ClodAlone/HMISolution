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
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using System.Xml;
using System.Runtime.Serialization;

using Opc.Ua.Client.Controls;
using Opc.Ua.Sample.Controls;

namespace Opc.Ua.StackTest
{
    static class Program
    {
        static byte[] Encode(TestStackRequest request1, ServiceMessageContext context1, bool binary)
        {
            if (binary)
            {
                return BinaryEncoder.EncodeMessage(request1, context1);
            }
            else
            {
                ServiceMessageContext.ThreadContext = context1;

                MemoryStream ostrm = new MemoryStream();

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = System.Text.Encoding.UTF8;

                using (XmlWriter writer = XmlWriter.Create(ostrm, settings))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(TestStackRequest));
                    serializer.WriteObject(writer, request1);
                }

                return ostrm.ToArray();
            }
        }

        static TestStackRequest Decode(byte[] message, ServiceMessageContext context1, bool binary)
        {
            if (binary)
            {
                return (TestStackRequest)BinaryDecoder.DecodeMessage(message, null, context1);
            }
            else
            {
                ServiceMessageContext.ThreadContext = context1;

                MemoryStream istrm = new MemoryStream(message);
                XmlReaderSettings settings = new XmlReaderSettings();

                using (XmlReader reader = XmlReader.Create(istrm, settings))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(TestStackRequest));
                    return (TestStackRequest)serializer.ReadObject(reader);
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception exception)
            {
				GuiUtils.HandleException("UA StackTest Client", MethodBase.GetCurrentMethod(), exception);
            }     
        }
    }
}
