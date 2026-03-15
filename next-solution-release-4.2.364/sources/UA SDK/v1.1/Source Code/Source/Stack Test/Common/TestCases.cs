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
using System.Configuration;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Reflection;
using System.IO;

namespace Opc.Ua.StackTest
{
    /// <summary>
    /// A class that defines constants for the test cases supported by the test harness.
    /// </summary>
    public class TestCases
    {
        /// <summary>
        /// Interoperability between the ANSI C and the .NET Serializers.
        /// </summary>
        public const string SerializerDirect = "SerializerDirect";

        /// <summary>
        /// Interoperability between the ANSI C and the .NET Serializers.
        /// </summary>
        public const string SerializerDirectEx = "SerializerDirectEx";

        /// <summary>
        /// Basic message exchange with scalar values.
        /// </summary>
        public const string ScalarValues = "ScalarValues";

        /// <summary>
        /// Basic message exchange with array values.
        /// </summary>
        public const string ArrayValues = "ArrayValues";

        /// <summary>
        /// Basic message exchange with Extension Object values.
        /// </summary>
        public const string ExtensionObjectValues = "ExtensionObjectValues";

        /// <summary>
        /// Basic message exchange with all Built-In types.
        /// </summary>
        public const string BuiltInTypes = "BuiltInTypes";

        /// <summary>
        /// Tests using large messages that exceeed the limits set by the sender/receiver.
        /// </summary>
        public const string LargeMessages = "LargeMessages";

        /// <summary>
        /// Protocol test: Multiple Channels Test
        /// </summary>
        public const string MultipleChannels = "MultipleChannels";

        /// <summary>
        /// Sends a series of requests that take time to complete.
        /// </summary>
        public const string AutoReconnect = "AutoReconnect";     

        /// <summary>
        /// Basic message exchange with Server Fault.
        /// </summary>
        public const string ServerFault = "ServerFault";

        /// <summary>
        /// Basic message exchange with Server Fault.
        /// </summary>
        public const string ServerTimeout = "ServerTimeout";

        /// <summary>
        /// Maximum length of the a string
        /// </summary>
        public const string MaxStringLength = "MaxStringLength";

        /// <summary>
        /// Maximum length of an array
        /// </summary>
        public const string MaxArrayLength = "MaxArrayLength";

        /// <summary>
        /// Maximum length of a message allowed by the server.
        /// </summary>
        public const string ServerMaxMessageSize = "ServerMaxMessageSize";

        /// <summary>
        /// Maximum length of a message allowed by the client.
        /// </summary>
        public const string ClientMaxMessageSize = "ClientMaxMessageSize";

        /// <summary>
        /// Maximum depth of the nesting
        /// </summary>
        public const string MaxDepth = "MaxDepth";

        /// <summary>
        /// Iteration that is used to initialize a test case
        /// </summary>
        public const int TestSetupIteration = -1;

        /// <summary>
        /// Iteration that is used to windup a test case
        /// </summary>
        public const int TestCleanupIteration = Int32.MaxValue;

        /// <summary>
        /// Maximum timeout value
        /// </summary>
        public const string MaxTimeout = "MaxTimeout";

        /// <summary>
        /// Minimum timeout value
        /// </summary>
        public const string MinTimeout = "MinTimeout";

        /// <summary>
        /// Maximum service execution
        /// </summary>
        public const string MaxTransportDelay = "MaxTransportDelay";

        /// <summary>
        /// Max response delay
        /// </summary>
        public const string MaxResponseDelay = "MaxResponseDelay";

        /// <summary>
        /// The iterval between tests.
        /// </summary>
        public const string RequestInterval = "RequestInterval";

        /// <summary>
        /// The type of stack events.
        /// </summary>
        public const string StackEventType = "StackEventType";
        
        /// <summary>
        /// Frequency of stack actions
        /// </summary>
        public const string StackEventFrequency = "StackEventFrequency";
        
        /// <summary>
        /// The maximum error recovery time.
        /// </summary>
        public const string MaxRecoveryTime = "MaxRecoveryTime";
        
        /// <summary>
        /// Number of channels for the multiple channel test case.
        /// </summary>
        public const string ChannelsPerServer = "ChannelsPerServer";

        /// <summary>
        /// Server details for the multiple channel test case.
        /// </summary>
        public const string ServerDetails = "ServerDetails";   

        /// <summary>
        /// Whether to verify the extension object lengths during the serializer direct tests.
        /// </summary>
        public const string AlwaysCheckSizes = "AlwaysCheckSizes";        
    }
}
