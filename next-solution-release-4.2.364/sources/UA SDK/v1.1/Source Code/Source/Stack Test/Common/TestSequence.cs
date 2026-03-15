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
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Opc.Ua.StackTest
{ 
    /// <summary>
    /// A sequence of test cases to run.
    /// </summary>
    public partial class TestSequence 
    {                
        #region Static Methods
        /// <summary>
        /// Loads a test sequence from a file.
        /// </summary>
        public static TestSequence Load(string filepath)
        {
            return Load(File.OpenRead(filepath));
        }
        
        /// <summary>
        /// Loads a test sequence from a stream.
        /// </summary>
        public static TestSequence Load(Stream istrm)
        {
			XmlTextReader reader = new XmlTextReader(istrm);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TestSequence));
                
                TestSequence sequence = serializer.Deserialize(reader) as TestSequence;
                                
                uint lastId = 1;

                foreach (TestCase testcase in sequence.TestCase)
                {
                    if (testcase.TestId == 0)
                    {
                        testcase.TestId = lastId++;
                    }
                    else
                    {
                        if (testcase.TestId < lastId)
                        {
                            testcase.TestId = lastId++;
                        }
                        else
                        {
                            lastId = testcase.TestId+1;
                        }
                    }
                }

                return sequence;
            }
            finally
            {
                reader.Close();
            }
        }
        
        /// <summary>
        /// Saves a test sequence to a file.
        /// </summary>
        public static void Save(string filepath, TestSequence sequence)
        {
            Save(File.Open(filepath, FileMode.Create), sequence);
        }
        
        /// <summary>
        /// Saves a test sequence to a stream.
        /// </summary>
        public static void Save(Stream ostrm, TestSequence sequence)
        {
			XmlTextWriter writer = new XmlTextWriter(ostrm, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TestSequence));
                serializer.Serialize(writer, sequence);
            }
            finally
            {
                writer.Close();
            }
        }
        #endregion
    }
    
    /// <summary>
    /// Bits that indicate what level of detail to include in the logs.
    /// </summary>
    [Flags]
    public enum TestLogDetailMasks : int
    {
        /// <summary>
        /// Log all errors.
        /// </summary>
        Errors = 0x01,

        /// <summary>
        /// Log an event when starting the first iteration for a test case.
        /// </summary>
        FirstStart = 0x02,

        /// <summary>
        /// Log an event when starting the all iterations for a test case.
        /// </summary>
        AllsStarts = 0x04,

        /// <summary>
        /// Log an event after completing the last iteration for a test case.
        /// </summary>
        LastEnd = 0x08,

        /// <summary>
        /// Log an event after completing each iterations for a test case.
        /// </summary>
        AllsEnds = 0x10,

        /// <summary>
        /// Log first 24 bytes of random data used to create the request/response data.
        /// </summary>
        RandomData = 0x20        
    }
}
