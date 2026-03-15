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
using System.Text;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace Opc.Ua.Gds
{
    #region RegisterApplicationMethodState Class
    #if (!OPCUA_EXCLUDE_RegisterApplicationMethodState)
    /// <summary>
    /// Stores an instance of the RegisterApplicationMethodType Method.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class RegisterApplicationMethodState : MethodState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public RegisterApplicationMethodState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Constructs an instance of a node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The new node.</returns>
        public new static NodeState Construct(NodeState parent)
        {
            return new RegisterApplicationMethodState(parent);
        }
        
        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYYIKBAAAAAEAHQAAAFJl" +
           "Z2lzdGVyQXBwbGljYXRpb25NZXRob2RUeXBlAQFfAgAvAQFfAl8CAAABAf////8CAAAAFWCpCgIAAAAA" +
           "AA4AAABJbnB1dEFyZ3VtZW50cwEBYAIALgBEYAIAAJYHAAAAAQAqAQFzAAAADgAAAEFwcGxpY2F0aW9u" +
           "VXJpAAz/////AAAAAAMAAAAATgAAAFRoZSBnbG9iYWxseSB1bmlxdWUgaWRlbnRpZmllciBmb3IgdGhl" +
           "IGFwcGxpY2F0aW9uIChhc3NpZ25lZCBpZiBub3QgcHJvdmlkZWQpLgEAKgEBcwAAAAsAAABNYWNoaW5l" +
           "TmFtZQAM/////wAAAAADAAAAAFEAAABUaGUgRE5TIG5hbWUgb3IgSVAgYWRkcmVzcyBvZiB0aGUgbWFj" +
           "aGluZSB3aGVyZSB0aGUgYXBwbGljYXRpb24gcnVucyAobWFuZGF0b3J5KS4BACoBAU4AAAAPAAAAQXBw" +
           "bGljYXRpb25OYW1lAAz/////AAAAAAMAAAAAKAAAAFRoZSBuYW1lIG9mIHRoZSBhcHBsaWNhdGlvbiAo" +
           "bWFuZGF0b3J5KS4BACoBAUwAAAAPAAAAQXBwbGljYXRpb25UeXBlAQAzAf////8AAAAAAwAAAAAkAAAA" +
           "VGhlIHR5cGUgb2YgYXBwbGljYXRpb24gKG1hbmRhdG9yeSkuAQAqAQFQAAAACgAAAFByb2R1Y3RVcmkA" +
           "DP////8AAAAAAwAAAAAvAAAAVGhlIGdsb2JhbGx5IHVuaXF1ZSBpZGVudGlmaWVyIGZvciB0aGUgcHJv" +
           "ZHVjdC4BACoBAVYAAAAQAAAAR2F0ZXdheVNlcnZlclVyaQAM/////wAAAAADAAAAAC8AAABUaGUgZ2xv" +
           "YmFsbHkgdW5pcXVlIGlkZW50aWZpZXIgZm9yIHRoZSBwcm9kdWN0LgEAKgEBcAAAAA0AAABEaXNjb3Zl" +
           "cnlVcmxzAAwBAAAAAAAAAAMAAAAATAAAAFRoZSBlbmRwb2ludHMgd2hpY2ggdGhlIGFwcGxpY2F0aW9u" +
           "IGlzIGNvbmZpZ3VyZWQgdG8gdXNlIChmb3Igc2VydmVycyBvbmx5KS4BACgBAQAAAAEB/////wAAAAAV" +
           "YKkKAgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEBYQIALgBEYQIAAJYBAAAAAQAqAQFVAAAAFQAAAFJl" +
           "dmlzZWRBcHBsaWNhdGlvblVyaQAM/////wAAAAADAAAAACkAAABUaGUgaWQgYXNzaWduZWQgYnkgdGhl" +
           "IGRpcmVjdG9yeSBzZXJ2aWNlLgEAKAEBAAAAAQH/////AAAAAA==";
        #endregion
        #endif
        #endregion
        
        #region Event Callbacks
        /// <summary>
        /// Raised when the the method is called.
        /// </summary>
        public RegisterApplicationMethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Invokes the method, returns the result and output argument.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="inputArguments">The input arguments which have been already validated.</param>
        /// <param name="outputArguments">The output arguments which have initialized with thier default values.</param>
        /// <returns></returns>
        protected override ServiceResult Call(
            ISystemContext context,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(context, objectId, inputArguments, outputArguments);
            }

            ServiceResult result = null;
            
            string applicationUri = (string)inputArguments[0];
            string machineName = (string)inputArguments[1];
            string applicationName = (string)inputArguments[2];
            ApplicationType applicationType = (ApplicationType)inputArguments[3];
            string productUri = (string)inputArguments[4];
            string gatewayServerUri = (string)inputArguments[5];
            string[] discoveryUrls = (string[])inputArguments[6];
            
            string revisedApplicationUri = (string)outputArguments[0];

            if (OnCall != null)
            {
                result = OnCall(
                    context,
                    this,
                    objectId,
                    applicationUri,
                    machineName,
                    applicationName,
                    applicationType,
                    productUri,
                    gatewayServerUri,
                    discoveryUrls,
                    ref revisedApplicationUri);
            }
            
            outputArguments[0] = revisedApplicationUri;

            return result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <summary>
    /// Used to receive notifications when the method is called.
    /// </summary>
    /// <exclude />
    public delegate ServiceResult RegisterApplicationMethodStateMethodCallHandler(
        ISystemContext context,
        MethodState method,
        NodeId objectId,
        string applicationUri,
        string machineName,
        string applicationName,
        ApplicationType applicationType,
        string productUri,
        string gatewayServerUri,
        string[] discoveryUrls,
        ref string revisedApplicationUri);
    #endif
    #endregion

    #region RequestCertificateMethodState Class
    #if (!OPCUA_EXCLUDE_RequestCertificateMethodState)
    /// <summary>
    /// Stores an instance of the RequestCertificateMethodType Method.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class RequestCertificateMethodState : MethodState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public RequestCertificateMethodState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Constructs an instance of a node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The new node.</returns>
        public new static NodeState Construct(NodeState parent)
        {
            return new RequestCertificateMethodState(parent);
        }
        
        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYYIKBAAAAAEAHAAAAFJl" +
           "cXVlc3RDZXJ0aWZpY2F0ZU1ldGhvZFR5cGUBAbkBAC8BAbkBuQEAAAEB/////wIAAAAVYKkKAgAAAAAA" +
           "DgAAAElucHV0QXJndW1lbnRzAQG6AQAuAES6AQAAlgYAAAABACoBAVAAAAAOAAAAQXBwbGljYXRpb25V" +
           "cmkADP////8AAAAAAwAAAAArAAAAVGhlIGdsb2JhbGx5IHVuaXF1ZSBpZCBmb3IgdGhlIGFwcGxpY2F0" +
           "aW9uLgEAKgEBUAAAAAsAAABTdWJqZWN0TmFtZQAM/////wAAAAADAAAAAC4AAABUaGUgdGhlIHN1Ympl" +
           "Y3QgbmFtZSB0byB1c2UgaW4gdGhlIGNldGlmaWNhdGUuAQAqAQFMAAAACwAAAERvbWFpbk5hbWVzAAwB" +
           "AAAAAAAAAAMAAAAAKgAAAFRoZSBkb21haW5zIHdoaWNoIHRoZSBhcHBsaWNhdGlvbiBydW5zIG9uLgEA" +
           "KgEBVwAAABAAAABQcml2YXRlS2V5Rm9ybWF0AAz/////AAAAAAMAAAAAMAAAAFRoZSBmb3JtYXQgZm9y" +
           "IHRoZSBwcml2YXRlIGtleSAoJ1BFTScgb3IgJ1BGWCcpLgEAKgEBSgAAABIAAABQcml2YXRlS2V5UGFz" +
           "c3dvcmQADP////8AAAAAAwAAAAAhAAAAVGhlIHBhc3N3b3JkIGZvciB0aGUgcHJpdmF0ZSBrZXkuAQAq" +
           "AQFUAAAAFgAAAENyZWF0ZUh0dHBzQ2VydGlmaWNhdGUAAf////8AAAAAAwAAAAAnAAAAV2hldGhlciB0" +
           "byBjcmVhdGUgYW4gSFRUUFMgY2VydGlmaWNhdGUuAQAoAQEAAAABAf////8AAAAAFWCpCgIAAAAAAA8A" +
           "AABPdXRwdXRBcmd1bWVudHMBAbsBAC4ARLsBAACWAQAAAAEAKgEBYwAAAAkAAABSZXF1ZXN0SWQAEf//" +
           "//8AAAAAAwAAAABDAAAAVGhlIGlkZW50aWZpZXIgYXNzaWduZWQgdGhlIHJlcXVlc3QgKHVzZWQgdG8g" +
           "cmV0cmlldmUgdGhlIHJlc3VsdHMpLgEAKAEBAAAAAQH/////AAAAAA==";
        #endregion
        #endif
        #endregion
        
        #region Event Callbacks
        /// <summary>
        /// Raised when the the method is called.
        /// </summary>
        public RequestCertificateMethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Invokes the method, returns the result and output argument.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="inputArguments">The input arguments which have been already validated.</param>
        /// <param name="outputArguments">The output arguments which have initialized with thier default values.</param>
        /// <returns></returns>
        protected override ServiceResult Call(
            ISystemContext context,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(context, objectId, inputArguments, outputArguments);
            }

            ServiceResult result = null;
            
            string applicationUri = (string)inputArguments[0];
            string subjectName = (string)inputArguments[1];
            string[] domainNames = (string[])inputArguments[2];
            string privateKeyFormat = (string)inputArguments[3];
            string privateKeyPassword = (string)inputArguments[4];
            bool createHttpsCertificate = (bool)inputArguments[5];
            
            NodeId requestId = (NodeId)outputArguments[0];

            if (OnCall != null)
            {
                result = OnCall(
                    context,
                    this,
                    objectId,
                    applicationUri,
                    subjectName,
                    domainNames,
                    privateKeyFormat,
                    privateKeyPassword,
                    createHttpsCertificate,
                    ref requestId);
            }
            
            outputArguments[0] = requestId;

            return result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <summary>
    /// Used to receive notifications when the method is called.
    /// </summary>
    /// <exclude />
    public delegate ServiceResult RequestCertificateMethodStateMethodCallHandler(
        ISystemContext context,
        MethodState method,
        NodeId objectId,
        string applicationUri,
        string subjectName,
        string[] domainNames,
        string privateKeyFormat,
        string privateKeyPassword,
        bool createHttpsCertificate,
        ref NodeId requestId);
    #endif
    #endregion

    #region RenewCertificateMethodState Class
    #if (!OPCUA_EXCLUDE_RenewCertificateMethodState)
    /// <summary>
    /// Stores an instance of the RenewCertificateMethodType Method.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class RenewCertificateMethodState : MethodState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public RenewCertificateMethodState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Constructs an instance of a node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The new node.</returns>
        public new static NodeState Construct(NodeState parent)
        {
            return new RenewCertificateMethodState(parent);
        }
        
        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYYIKBAAAAAEAGgAAAFJl" +
           "bmV3Q2VydGlmaWNhdGVNZXRob2RUeXBlAQEZAgAvAQEZAhkCAAABAf////8CAAAAFWCpCgIAAAAAAA4A" +
           "AABJbnB1dEFyZ3VtZW50cwEBGgIALgBEGgIAAJYDAAAAAQAqAQFuAAAADgAAAEFwcGxpY2F0aW9uVXJp" +
           "AAz/////AAAAAAMAAAAASQAAAFRoZSBnbG9iYWxseSB1bmlxdWUgaWQgZm9yIHRoZSBhcHBsaWNhdGlv" +
           "biB3aXRoIHRoZSBjZXJ0aWZpY2F0ZSB0byByZW5ldy4BACoBAVcAAAAQAAAAUHJpdmF0ZUtleUZvcm1h" +
           "dAAM/////wAAAAADAAAAADAAAABUaGUgZm9ybWF0IGZvciB0aGUgcHJpdmF0ZSBrZXkgKCdQRU0nIG9y" +
           "ICdQRlgnKS4BACoBAUoAAAASAAAAUHJpdmF0ZUtleVBhc3N3b3JkAAz/////AAAAAAMAAAAAIQAAAFRo" +
           "ZSBwYXNzd29yZCBmb3IgdGhlIHByaXZhdGUga2V5LgEAKAEBAAAAAQH/////AAAAABVgqQoCAAAAAAAP" +
           "AAAAT3V0cHV0QXJndW1lbnRzAQEbAgAuAEQbAgAAlgEAAAABACoBAWMAAAAJAAAAUmVxdWVzdElkABH/" +
           "////AAAAAAMAAAAAQwAAAFRoZSBpZGVudGlmaWVyIGFzc2lnbmVkIHRoZSByZXF1ZXN0ICh1c2VkIHRv" +
           "IHJldHJpZXZlIHRoZSByZXN1bHRzKS4BACgBAQAAAAEB/////wAAAAA=";
        #endregion
        #endif
        #endregion
        
        #region Event Callbacks
        /// <summary>
        /// Raised when the the method is called.
        /// </summary>
        public RenewCertificateMethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Invokes the method, returns the result and output argument.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="inputArguments">The input arguments which have been already validated.</param>
        /// <param name="outputArguments">The output arguments which have initialized with thier default values.</param>
        /// <returns></returns>
        protected override ServiceResult Call(
            ISystemContext context,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(context, objectId, inputArguments, outputArguments);
            }

            ServiceResult result = null;
            
            string applicationUri = (string)inputArguments[0];
            string privateKeyFormat = (string)inputArguments[1];
            string privateKeyPassword = (string)inputArguments[2];
            
            NodeId requestId = (NodeId)outputArguments[0];

            if (OnCall != null)
            {
                result = OnCall(
                    context,
                    this,
                    objectId,
                    applicationUri,
                    privateKeyFormat,
                    privateKeyPassword,
                    ref requestId);
            }
            
            outputArguments[0] = requestId;

            return result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <summary>
    /// Used to receive notifications when the method is called.
    /// </summary>
    /// <exclude />
    public delegate ServiceResult RenewCertificateMethodStateMethodCallHandler(
        ISystemContext context,
        MethodState method,
        NodeId objectId,
        string applicationUri,
        string privateKeyFormat,
        string privateKeyPassword,
        ref NodeId requestId);
    #endif
    #endregion

    #region RevokeCertificateMethodState Class
    #if (!OPCUA_EXCLUDE_RevokeCertificateMethodState)
    /// <summary>
    /// Stores an instance of the RevokeCertificateMethodType Method.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class RevokeCertificateMethodState : MethodState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public RevokeCertificateMethodState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Constructs an instance of a node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The new node.</returns>
        public new static NodeState Construct(NodeState parent)
        {
            return new RevokeCertificateMethodState(parent);
        }
        
        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYYIKBAAAAAEAGwAAAFJl" +
           "dm9rZUNlcnRpZmljYXRlTWV0aG9kVHlwZQEBHAIALwEBHAIcAgAAAQH/////AQAAABVgqQoCAAAAAAAO" +
           "AAAASW5wdXRBcmd1bWVudHMBAR0CAC4ARB0CAACWAQAAAAEAKgEBPAAAAAsAAABDZXJ0aWZpY2F0ZQAM" +
           "/////wAAAAADAAAAABoAAABUaGUgY2VydGlmaWNhdGUgdG8gcmV2b2tlLgEAKAEBAAAAAQH/////AAAA" +
           "AA==";
        #endregion
        #endif
        #endregion
        
        #region Event Callbacks
        /// <summary>
        /// Raised when the the method is called.
        /// </summary>
        public RevokeCertificateMethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Invokes the method, returns the result and output argument.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="inputArguments">The input arguments which have been already validated.</param>
        /// <param name="outputArguments">The output arguments which have initialized with thier default values.</param>
        /// <returns></returns>
        protected override ServiceResult Call(
            ISystemContext context,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(context, objectId, inputArguments, outputArguments);
            }

            ServiceResult result = null;
            
            string certificate = (string)inputArguments[0];

            if (OnCall != null)
            {
                result = OnCall(
                    context,
                    this,
                    objectId,
                    certificate);
            }

            return result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <summary>
    /// Used to receive notifications when the method is called.
    /// </summary>
    /// <exclude />
    public delegate ServiceResult RevokeCertificateMethodStateMethodCallHandler(
        ISystemContext context,
        MethodState method,
        NodeId objectId,
        string certificate);
    #endif
    #endregion

    #region CheckRequestStatusMethodState Class
    #if (!OPCUA_EXCLUDE_CheckRequestStatusMethodState)
    /// <summary>
    /// Stores an instance of the CheckRequestStatusMethodType Method.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class CheckRequestStatusMethodState : MethodState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public CheckRequestStatusMethodState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Constructs an instance of a node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The new node.</returns>
        public new static NodeState Construct(NodeState parent)
        {
            return new CheckRequestStatusMethodState(parent);
        }
        
        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYYIKBAAAAAEAHAAAAENo" +
           "ZWNrUmVxdWVzdFN0YXR1c01ldGhvZFR5cGUBAWICAC8BAWICYgIAAAEB/////wIAAAAVYKkKAgAAAAAA" +
           "DgAAAElucHV0QXJndW1lbnRzAQFjAgAuAERjAgAAlgEAAAABACoBAUQAAAAJAAAAUmVxdWVzdElkABH/" +
           "////AAAAAAMAAAAAJAAAAFRoZSBpZGVudGlmaWVyIGFzc2lnbmVkIHRoZSByZXF1ZXN0LgEAKAEBAAAA" +
           "AQH/////AAAAABVgqQoCAAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQFkAgAuAERkAgAAlgMAAAABACoB" +
           "AUIAAAALAAAAQ2VydGlmaWNhdGUAD/////8AAAAAAwAAAAAgAAAAVGhlIG5ldyBhcHBsaWNhdGlvbiBj" +
           "ZXJ0aWZpY2F0ZS4BACoBAVwAAAAKAAAAUHJpdmF0ZUtleQAP/////wAAAAADAAAAADsAAABUaGUgbmV3" +
           "IGFwcGxpY2F0aW9uIHByaXZhdGUga2V5IHRvIGdvIHdpdGggdGhlIGNlcnRpZmljYXRlLgEAKgEBQQAA" +
           "ABIAAABJc3N1ZXJDZXJ0aWZpY2F0ZXMADwEAAAAAAAAAAwAAAAAYAAAAVGhlIGlzc3VlciBjZXJ0aWZp" +
           "Y2F0ZXMuAQAoAQEAAAABAf////8AAAAA";
        #endregion
        #endif
        #endregion
        
        #region Event Callbacks
        /// <summary>
        /// Raised when the the method is called.
        /// </summary>
        public CheckRequestStatusMethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Invokes the method, returns the result and output argument.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="inputArguments">The input arguments which have been already validated.</param>
        /// <param name="outputArguments">The output arguments which have initialized with thier default values.</param>
        /// <returns></returns>
        protected override ServiceResult Call(
            ISystemContext context,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(context, objectId, inputArguments, outputArguments);
            }

            ServiceResult result = null;
            
            NodeId requestId = (NodeId)inputArguments[0];
            
            byte[] certificate = (byte[])outputArguments[0];
            byte[] privateKey = (byte[])outputArguments[1];
            byte[][] issuerCertificates = (byte[][])outputArguments[2];

            if (OnCall != null)
            {
                result = OnCall(
                    context,
                    this,
                    objectId,
                    requestId,
                    ref certificate,
                    ref privateKey,
                    ref issuerCertificates);
            }
            
            outputArguments[0] = certificate;
            outputArguments[1] = privateKey;
            outputArguments[2] = issuerCertificates;

            return result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <summary>
    /// Used to receive notifications when the method is called.
    /// </summary>
    /// <exclude />
    public delegate ServiceResult CheckRequestStatusMethodStateMethodCallHandler(
        ISystemContext context,
        MethodState method,
        NodeId objectId,
        NodeId requestId,
        ref byte[] certificate,
        ref byte[] privateKey,
        ref byte[][] issuerCertificates);
    #endif
    #endregion

    #region GetTrustListMethodState Class
    #if (!OPCUA_EXCLUDE_GetTrustListMethodState)
    /// <summary>
    /// Stores an instance of the GetTrustListMethodType Method.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class GetTrustListMethodState : MethodState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public GetTrustListMethodState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Constructs an instance of a node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The new node.</returns>
        public new static NodeState Construct(NodeState parent)
        {
            return new GetTrustListMethodState(parent);
        }
        
        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYYIKBAAAAAEAFgAAAEdl" +
           "dFRydXN0TGlzdE1ldGhvZFR5cGUBAXECAC8BAXECcQIAAAEB/////wIAAAAVYKkKAgAAAAAADgAAAElu" +
           "cHV0QXJndW1lbnRzAQFyAgAuAERyAgAAlgIAAAABACoBAVAAAAAOAAAAQXBwbGljYXRpb25VcmkADP//" +
           "//8AAAAAAwAAAAArAAAAVGhlIGdsb2JhbGx5IHVuaXF1ZSBpZCBmb3IgdGhlIGFwcGxpY2F0aW9uLgEA" +
           "KgEBUwAAABAAAABSZXR1cm5IdHRwc0xpc3RzAAH/////AAAAAAMAAAAALAAAAFdoZXRoZXIgdG8gcmV0" +
           "dXJuIHRoZSB0cnVzdCBsaXN0cyBmb3IgSFRUUFMuAQAoAQEAAAABAf////8AAAAAFWCpCgIAAAAAAA8A" +
           "AABPdXRwdXRBcmd1bWVudHMBAXMCAC4ARHMCAACWBQAAAAEAKgEBSQAAAAsAAABUcnVzdExpc3RJZAAR" +
           "/////wAAAAADAAAAACcAAABBIHVuaXF1ZSBpZGVudGlmaWVyIGZvciB0aGUgdHJ1c3QgbGlzdC4BACoB" +
           "AVYAAAATAAAAVHJ1c3RlZENlcnRpZmljYXRlcwAPAQAAAAAAAAADAAAAACwAAABUaGUgY2VydGlmaWNh" +
           "dGVzIHRydXN0ZWQgYnkgdGhlIGFwcGxpY2F0aW9uLgEAKgEBdgAAACEAAABUcnVzdGVkQ2VydGlmaWNh" +
           "dGVSZXZvY2F0aW9uTGlzdHMADwEAAAAAAAAAAwAAAAA+AAAAQW55IHJldm9jYXRpb24gbGlzdHMgYXNz" +
           "b2NpYXRlZCB3aXRoIHRoZSB0cnVzdGVkIGNlcnRpZmljYXRlcy4BACoBAVsAAAASAAAASXNzdWVyQ2Vy" +
           "dGlmaWNhdGVzAA8BAAAAAAAAAAMAAAAAMgAAAFRoZSBpc3N1ZXIgY2VydGlmaWNhdGVzIG5lZWRlZCBi" +
           "eSB0aGUgYXBwbGljYXRpb24uAQAqAQF0AAAAIAAAAElzc3VlckNlcnRpZmljYXRlUmV2b2NhdGlvbkxp" +
           "c3RzAA8BAAAAAAAAAAMAAAAAPQAAAEFueSByZXZvY2F0aW9uIGxpc3RzIGFzc29jaWF0ZWQgd2l0aCB0" +
           "aGUgaXNzdWVyIGNlcnRpZmljYXRlcy4BACgBAQAAAAEB/////wAAAAA=";
        #endregion
        #endif
        #endregion
        
        #region Event Callbacks
        /// <summary>
        /// Raised when the the method is called.
        /// </summary>
        public GetTrustListMethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Invokes the method, returns the result and output argument.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="inputArguments">The input arguments which have been already validated.</param>
        /// <param name="outputArguments">The output arguments which have initialized with thier default values.</param>
        /// <returns></returns>
        protected override ServiceResult Call(
            ISystemContext context,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(context, objectId, inputArguments, outputArguments);
            }

            ServiceResult result = null;
            
            string applicationUri = (string)inputArguments[0];
            bool returnHttpsLists = (bool)inputArguments[1];
            
            NodeId trustListId = (NodeId)outputArguments[0];
            byte[][] trustedCertificates = (byte[][])outputArguments[1];
            byte[][] trustedCertificateRevocationLists = (byte[][])outputArguments[2];
            byte[][] issuerCertificates = (byte[][])outputArguments[3];
            byte[][] issuerCertificateRevocationLists = (byte[][])outputArguments[4];

            if (OnCall != null)
            {
                result = OnCall(
                    context,
                    this,
                    objectId,
                    applicationUri,
                    returnHttpsLists,
                    ref trustListId,
                    ref trustedCertificates,
                    ref trustedCertificateRevocationLists,
                    ref issuerCertificates,
                    ref issuerCertificateRevocationLists);
            }
            
            outputArguments[0] = trustListId;
            outputArguments[1] = trustedCertificates;
            outputArguments[2] = trustedCertificateRevocationLists;
            outputArguments[3] = issuerCertificates;
            outputArguments[4] = issuerCertificateRevocationLists;

            return result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <summary>
    /// Used to receive notifications when the method is called.
    /// </summary>
    /// <exclude />
    public delegate ServiceResult GetTrustListMethodStateMethodCallHandler(
        ISystemContext context,
        MethodState method,
        NodeId objectId,
        string applicationUri,
        bool returnHttpsLists,
        ref NodeId trustListId,
        ref byte[][] trustedCertificates,
        ref byte[][] trustedCertificateRevocationLists,
        ref byte[][] issuerCertificates,
        ref byte[][] issuerCertificateRevocationLists);
    #endif
    #endregion

    #region QueryServersMethodState Class
    #if (!OPCUA_EXCLUDE_QueryServersMethodState)
    /// <summary>
    /// Stores an instance of the QueryServersMethodType Method.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class QueryServersMethodState : MethodState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public QueryServersMethodState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Constructs an instance of a node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The new node.</returns>
        public new static NodeState Construct(NodeState parent)
        {
            return new QueryServersMethodState(parent);
        }
        
        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYYIKBAAAAAEAFgAAAFF1" +
           "ZXJ5U2VydmVyc01ldGhvZFR5cGUBAR4CAC8BAR4CHgIAAAEB/////wIAAAAVYKkKAgAAAAAADgAAAElu" +
           "cHV0QXJndW1lbnRzAQEfAgAuAEQfAgAAlgUAAAABACoBAUoAAAAJAAAARWxlbWVudElkABH/////AAAA" +
           "AAMAAAAAKgAAAFRoZSBOb2RlSWQgb2YgdGhlIFN5c3RlbUVsZW1lbnQgdG8gc2VhcmNoLgEAKgEBYgAA" +
           "AA8AAABBcHBsaWNhdGlvbk5hbWUADP////8AAAAAAwAAAAA8AAAAQSBwYXR0ZXJuIHVzZWQgdG8gbWF0" +
           "Y2ggdGhlIEFwcGxpY2F0aW9uTmFtZXMgb2YgdGhlIFNlcnZlcnMuAQAqAQFaAAAACwAAAE1hY2hpbmVO" +
           "YW1lAAz/////AAAAAAMAAAAAOAAAAEEgcGF0dGVybiB1c2VkIHRvIG1hdGNoIHRoZSBNYWNoaW5lTmFt" +
           "ZXMgb2YgdGhlIFNlcnZlcnMuAQAqAQFgAAAADgAAAEFwcGxpY2F0aW9uVXJpAAz/////AAAAAAMAAAAA" +
           "OwAAAEEgcGF0dGVybiB1c2VkIHRvIG1hdGNoIHRoZSBBcHBsaWNhdGlvblVyaXMgb2YgdGhlIFNlcnZl" +
           "cnMuAQAqAQFYAAAACgAAAFByb2R1Y3RVcmkADP////8AAAAAAwAAAAA3AAAAQSBwYXR0ZXJuIHVzZWQg" +
           "dG8gbWF0Y2ggdGhlIFByb2R1Y3RVcmlzIG9mIHRoZSBTZXJ2ZXJzLgEAKAEBAAAAAQH/////AAAAABVg" +
           "qQoCAAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQEgAgAuAEQgAgAAlgEAAAABACoBAVIAAAAHAAAAU2Vy" +
           "dmVycwEANAEBAAAAAAAAAAMAAAAAMgAAAFRoZSBsaXN0IG9mIFNlcnZlcnMgdGhhdCBtZWV0IHRoZSBz" +
           "ZWFyY2ggY3JpdGVyaWEuAQAoAQEAAAABAf////8AAAAA";
        #endregion
        #endif
        #endregion
        
        #region Event Callbacks
        /// <summary>
        /// Raised when the the method is called.
        /// </summary>
        public QueryServersMethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Invokes the method, returns the result and output argument.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="inputArguments">The input arguments which have been already validated.</param>
        /// <param name="outputArguments">The output arguments which have initialized with thier default values.</param>
        /// <returns></returns>
        protected override ServiceResult Call(
            ISystemContext context,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(context, objectId, inputArguments, outputArguments);
            }

            ServiceResult result = null;
            
            NodeId elementId = (NodeId)inputArguments[0];
            string applicationName = (string)inputArguments[1];
            string machineName = (string)inputArguments[2];
            string applicationUri = (string)inputArguments[3];
            string productUri = (string)inputArguments[4];
            
            ApplicationDescription[] servers = (ApplicationDescription[])outputArguments[0];

            if (OnCall != null)
            {
                result = OnCall(
                    context,
                    this,
                    objectId,
                    elementId,
                    applicationName,
                    machineName,
                    applicationUri,
                    productUri,
                    ref servers);
            }
            
            outputArguments[0] = servers;

            return result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <summary>
    /// Used to receive notifications when the method is called.
    /// </summary>
    /// <exclude />
    public delegate ServiceResult QueryServersMethodStateMethodCallHandler(
        ISystemContext context,
        MethodState method,
        NodeId objectId,
        NodeId elementId,
        string applicationName,
        string machineName,
        string applicationUri,
        string productUri,
        ref ApplicationDescription[] servers);
    #endif
    #endregion

    #region CertificateRequestState Class
    #if (!OPCUA_EXCLUDE_CertificateRequestState)
    /// <summary>
    /// Stores an instance of the CertificateRequestType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class CertificateRequestState : DialogConditionState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public CertificateRequestState(NodeState parent) : base(parent)
        {
        }
        
        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(Opc.Ua.Gds.ObjectTypes.CertificateRequestType, Opc.Ua.Gds.Namespaces.OpcUaGds, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYIAAAQAAAAEAHgAAAENl" +
           "cnRpZmljYXRlUmVxdWVzdFR5cGVJbnN0YW5jZQEBwgEBAcIB/////yQAAAA1YIkKAgAAAAAABwAAAEV2" +
           "ZW50SWQBAcMBAwAAAAArAAAAQSBnbG9iYWxseSB1bmlxdWUgaWRlbnRpZmllciBmb3IgdGhlIGV2ZW50" +
           "LgAuAETDAQAAAA//////AQH/////AAAAADVgiQoCAAAAAAAJAAAARXZlbnRUeXBlAQHEAQMAAAAAIgAA" +
           "AFRoZSBpZGVudGlmaWVyIGZvciB0aGUgZXZlbnQgdHlwZS4ALgBExAEAAAAR/////wEB/////wAAAAA1" +
           "YIkKAgAAAAAACgAAAFNvdXJjZU5vZGUBAcUBAwAAAAAYAAAAVGhlIHNvdXJjZSBvZiB0aGUgZXZlbnQu" +
           "AC4ARMUBAAAAEf////8BAf////8AAAAANWCJCgIAAAAAAAoAAABTb3VyY2VOYW1lAQHGAQMAAAAAKQAA" +
           "AEEgZGVzY3JpcHRpb24gb2YgdGhlIHNvdXJjZSBvZiB0aGUgZXZlbnQuAC4ARMYBAAAADP////8BAf//" +
           "//8AAAAANWCJCgIAAAAAAAQAAABUaW1lAQHHAQMAAAAAGAAAAFdoZW4gdGhlIGV2ZW50IG9jY3VycmVk" +
           "LgAuAETHAQAAAQAmAf////8BAf////8AAAAANWCJCgIAAAAAAAsAAABSZWNlaXZlVGltZQEByAEDAAAA" +
           "AD4AAABXaGVuIHRoZSBzZXJ2ZXIgcmVjZWl2ZWQgdGhlIGV2ZW50IGZyb20gdGhlIHVuZGVybHlpbmcg" +
           "c3lzdGVtLgAuAETIAQAAAQAmAf////8BAf////8AAAAANWCJCgIAAAAAAAkAAABMb2NhbFRpbWUBAckB" +
           "AwAAAAA8AAAASW5mb3JtYXRpb24gYWJvdXQgdGhlIGxvY2FsIHRpbWUgd2hlcmUgdGhlIGV2ZW50IG9y" +
           "aWdpbmF0ZWQuAC4ARMkBAAABANAi/////wEB/////wAAAAA1YIkKAgAAAAAABwAAAE1lc3NhZ2UBAcoB" +
           "AwAAAAAlAAAAQSBsb2NhbGl6ZWQgZGVzY3JpcHRpb24gb2YgdGhlIGV2ZW50LgAuAETKAQAAABX/////" +
           "AQH/////AAAAADVgiQoCAAAAAAAIAAAAU2V2ZXJpdHkBAcsBAwAAAAAhAAAASW5kaWNhdGVzIGhvdyB1" +
           "cmdlbnQgYW4gZXZlbnQgaXMuAC4ARMsBAAAABf////8BAf////8AAAAAFWCJCgIAAAAAABAAAABDb25k" +
           "aXRpb25DbGFzc0lkAQHMAQAuAETMAQAAABH/////AQH/////AAAAABVgiQoCAAAAAAASAAAAQ29uZGl0" +
           "aW9uQ2xhc3NOYW1lAQHNAQAuAETNAQAAABX/////AQH/////AAAAABVgiQoCAAAAAAANAAAAQ29uZGl0" +
           "aW9uTmFtZQEBzgEALgBEzgEAAAAM/////wEB/////wAAAAAVYIkKAgAAAAAACAAAAEJyYW5jaElkAQHP" +
           "AQAuAETPAQAAABH/////AQH/////AAAAABVgiQoCAAAAAAAGAAAAUmV0YWluAQHQAQAuAETQAQAAAAH/" +
           "////AQH/////AAAAABVgiQoCAAAAAAAMAAAARW5hYmxlZFN0YXRlAQHRAQAvAQAjI9EBAAAAFf////8B" +
           "AQEAAAABACwjAAEB5wEBAAAAFWCJCgIAAAAAAAIAAABJZAEB0gEALgBE0gEAAAAB/////wEB/////wAA" +
           "AAAVYIkKAgAAAAAABwAAAFF1YWxpdHkBAdoBAC8BACoj2gEAAAAT/////wEB/////wEAAAAVYIkKAgAA" +
           "AAAADwAAAFNvdXJjZVRpbWVzdGFtcAEB2wEALgBE2wEAAAEAJgH/////AQH/////AAAAABVgiQoCAAAA" +
           "AAAMAAAATGFzdFNldmVyaXR5AQHcAQAvAQAqI9wBAAAABf////8BAf////8BAAAAFWCJCgIAAAAAAA8A" +
           "AABTb3VyY2VUaW1lc3RhbXABAd0BAC4ARN0BAAABACYB/////wEB/////wAAAAAVYIkKAgAAAAAABwAA" +
           "AENvbW1lbnQBAd4BAC8BACoj3gEAAAAV/////wEB/////wEAAAAVYIkKAgAAAAAADwAAAFNvdXJjZVRp" +
           "bWVzdGFtcAEB3wEALgBE3wEAAAEAJgH/////AQH/////AAAAABVgiQoCAAAAAAAMAAAAQ2xpZW50VXNl" +
           "cklkAQHgAQAuAETgAQAAAAz/////AQH/////AAAAAARhggoEAAAAAAAGAAAARW5hYmxlAQHhAQAvAQBD" +
           "I+EBAAABAQEAAAABAPkLAAEA8woAAAAABGGCCgQAAAAAAAcAAABEaXNhYmxlAQHiAQAvAQBEI+IBAAAB" +
           "AQEAAAABAPkLAAEA8woAAAAAFWCJCgIAAAAAAAsAAABEaWFsb2dTdGF0ZQEB5wEALwEAIyPnAQAAABX/" +
           "////AQEBAAAAAQAsIwEBAdEBAQAAABVgiQoCAAAAAAACAAAASWQBAegBAC4AROgBAAAAAf////8BAf//" +
           "//8AAAAAFWCJCgIAAAAAAAYAAABQcm9tcHQBAfABAC4ARPABAAAAFf////8BAf////8AAAAAFWCJCgIA" +
           "AAAAABEAAABSZXNwb25zZU9wdGlvblNldAEB8QEALgBE8QEAAAAVAQAAAAEB/////wAAAAAVYIkKAgAA" +
           "AAAADwAAAERlZmF1bHRSZXNwb25zZQEB8gEALgBE8gEAAAAG/////wEB/////wAAAAAVYIkKAgAAAAAA" +
           "CgAAAE9rUmVzcG9uc2UBAfMBAC4ARPMBAAAABv////8BAf////8AAAAAFWCJCgIAAAAAAA4AAABDYW5j" +
           "ZWxSZXNwb25zZQEB9AEALgBE9AEAAAAG/////wEB/////wAAAAAVYIkKAgAAAAAADAAAAExhc3RSZXNw" +
           "b25zZQEB9QEALgBE9QEAAAAG/////wEB/////wAAAAAEYYIKBAAAAAAABwAAAFJlc3BvbmQBAfYBAC8B" +
           "AG0j9gEAAAEBAQAAAAEA+QsAAQDfIgEAAAAVYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQH3AQAu" +
           "AET3AQAAlgEAAAABACoBAUwAAAAQAAAAU2VsZWN0ZWRSZXNwb25zZQAG/////wAAAAADAAAAACUAAABU" +
           "aGUgcmVzcG9uc2UgdG8gdGhlIGRpYWxvZyBjb25kaXRpb24uAQAoAQEAAAABAf////8AAAAAFWCJCgIA" +
           "AAABAA4AAABBcHBsaWNhdGlvblVyaQEB+AEALgBE+AEAAAAM/////wEB/////wAAAAAVYIkKAgAAAAEA" +
           "CgAAAFByb2R1Y3RVcmkBAXQCAC4ARHQCAAAADP////8BAf////8AAAAAFWCJCgIAAAABAA8AAABBcHBs" +
           "aWNhdGlvblR5cGUBAfkBAC4ARPkBAAAADP////8BAf////8AAAAAFWCJCgIAAAABAAsAAABNYWNoaW5l" +
           "TmFtZQEBRQIALgBERQIAAAAM/////wEB/////wAAAAAVYIkKAgAAAAEACwAAAFN1YmplY3ROYW1lAQH7" +
           "AQAuAET7AQAAAAz/////AQH/////AAAAABVgiQoCAAAAAQALAAAARG9tYWluTmFtZXMBAfwBAC4ARPwB" +
           "AAAADAEAAAABAf////8AAAAAFWCJCgIAAAABABIAAABJc0h0dHBzQ2VydGlmaWNhdGUBAUYCAC4AREYC" +
           "AAAAAf////8BAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the ApplicationUri Property.
        /// </summary>
        public PropertyState<string> ApplicationUri
        {
            get
            { 
                return m_applicationUri;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_applicationUri, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_applicationUri = value;
            }
        }

        /// <summary>
        /// A description for the ProductUri Property.
        /// </summary>
        public PropertyState<string> ProductUri
        {
            get
            { 
                return m_productUri;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_productUri, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_productUri = value;
            }
        }

        /// <summary>
        /// A description for the ApplicationType Property.
        /// </summary>
        public PropertyState<string> ApplicationType
        {
            get
            { 
                return m_applicationType;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_applicationType, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_applicationType = value;
            }
        }

        /// <summary>
        /// A description for the MachineName Property.
        /// </summary>
        public PropertyState<string> MachineName
        {
            get
            { 
                return m_machineName;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_machineName, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_machineName = value;
            }
        }

        /// <summary>
        /// A description for the SubjectName Property.
        /// </summary>
        public PropertyState<string> SubjectName
        {
            get
            { 
                return m_subjectName;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_subjectName, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_subjectName = value;
            }
        }

        /// <summary>
        /// A description for the DomainNames Property.
        /// </summary>
        public PropertyState<string[]> DomainNames
        {
            get
            { 
                return m_domainNames;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_domainNames, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_domainNames = value;
            }
        }

        /// <summary>
        /// A description for the IsHttpsCertificate Property.
        /// </summary>
        public PropertyState<bool> IsHttpsCertificate
        {
            get
            { 
                return m_isHttpsCertificate;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_isHttpsCertificate, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_isHttpsCertificate = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_applicationUri != null)
            {
                children.Add(m_applicationUri);
            }

            if (m_productUri != null)
            {
                children.Add(m_productUri);
            }

            if (m_applicationType != null)
            {
                children.Add(m_applicationType);
            }

            if (m_machineName != null)
            {
                children.Add(m_machineName);
            }

            if (m_subjectName != null)
            {
                children.Add(m_subjectName);
            }

            if (m_domainNames != null)
            {
                children.Add(m_domainNames);
            }

            if (m_isHttpsCertificate != null)
            {
                children.Add(m_isHttpsCertificate);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case Opc.Ua.Gds.BrowseNames.ApplicationUri:
                {
                    if (createOrReplace)
                    {
                        if (ApplicationUri == null)
                        {
                            if (replacement == null)
                            {
                                ApplicationUri = new PropertyState<string>(this);
                            }
                            else
                            {
                                ApplicationUri = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = ApplicationUri;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.ProductUri:
                {
                    if (createOrReplace)
                    {
                        if (ProductUri == null)
                        {
                            if (replacement == null)
                            {
                                ProductUri = new PropertyState<string>(this);
                            }
                            else
                            {
                                ProductUri = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = ProductUri;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.ApplicationType:
                {
                    if (createOrReplace)
                    {
                        if (ApplicationType == null)
                        {
                            if (replacement == null)
                            {
                                ApplicationType = new PropertyState<string>(this);
                            }
                            else
                            {
                                ApplicationType = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = ApplicationType;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.MachineName:
                {
                    if (createOrReplace)
                    {
                        if (MachineName == null)
                        {
                            if (replacement == null)
                            {
                                MachineName = new PropertyState<string>(this);
                            }
                            else
                            {
                                MachineName = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = MachineName;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.SubjectName:
                {
                    if (createOrReplace)
                    {
                        if (SubjectName == null)
                        {
                            if (replacement == null)
                            {
                                SubjectName = new PropertyState<string>(this);
                            }
                            else
                            {
                                SubjectName = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = SubjectName;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.DomainNames:
                {
                    if (createOrReplace)
                    {
                        if (DomainNames == null)
                        {
                            if (replacement == null)
                            {
                                DomainNames = new PropertyState<string[]>(this);
                            }
                            else
                            {
                                DomainNames = (PropertyState<string[]>)replacement;
                            }
                        }
                    }

                    instance = DomainNames;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.IsHttpsCertificate:
                {
                    if (createOrReplace)
                    {
                        if (IsHttpsCertificate == null)
                        {
                            if (replacement == null)
                            {
                                IsHttpsCertificate = new PropertyState<bool>(this);
                            }
                            else
                            {
                                IsHttpsCertificate = (PropertyState<bool>)replacement;
                            }
                        }
                    }

                    instance = IsHttpsCertificate;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private PropertyState<string> m_applicationUri;
        private PropertyState<string> m_productUri;
        private PropertyState<string> m_applicationType;
        private PropertyState<string> m_machineName;
        private PropertyState<string> m_subjectName;
        private PropertyState<string[]> m_domainNames;
        private PropertyState<bool> m_isHttpsCertificate;
        #endregion
    }
    #endif
    #endregion

    #region RootDirectoryEntryState Class
    #if (!OPCUA_EXCLUDE_RootDirectoryEntryState)
    /// <summary>
    /// Stores an instance of the RootDirectoryEntryType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class RootDirectoryEntryState : FolderState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public RootDirectoryEntryState(NodeState parent) : base(parent)
        {
        }
        
        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(Opc.Ua.Gds.ObjectTypes.RootDirectoryEntryType, Opc.Ua.Gds.Namespaces.OpcUaGds, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYIAAAQAAAAEAHgAAAFJv" +
           "b3REaXJlY3RvcnlFbnRyeVR5cGVJbnN0YW5jZQEB/QEBAf0B/////wkAAAAEYIAKAQAAAAEAEwAAAENl" +
           "cnRpZmljYXRlUmVxdWVzdHMBAf4BACMAPf4BAAD/////AAAAAARggAoBAAAAAQAMAAAAQXBwbGljYXRp" +
           "b25zAQFBAgAjAD1BAgAA/////wAAAAAEYYIKBAAAAAEAEwAAAFJlZ2lzdGVyQXBwbGljYXRpb24BAWUC" +
           "AC8BAWUCZQIAAAEB/////wIAAAAVYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQFmAgAuAERmAgAA" +
           "lgcAAAABACoBAXMAAAAOAAAAQXBwbGljYXRpb25VcmkADP////8AAAAAAwAAAABOAAAAVGhlIGdsb2Jh" +
           "bGx5IHVuaXF1ZSBpZGVudGlmaWVyIGZvciB0aGUgYXBwbGljYXRpb24gKGFzc2lnbmVkIGlmIG5vdCBw" +
           "cm92aWRlZCkuAQAqAQFzAAAACwAAAE1hY2hpbmVOYW1lAAz/////AAAAAAMAAAAAUQAAAFRoZSBETlMg" +
           "bmFtZSBvciBJUCBhZGRyZXNzIG9mIHRoZSBtYWNoaW5lIHdoZXJlIHRoZSBhcHBsaWNhdGlvbiBydW5z" +
           "IChtYW5kYXRvcnkpLgEAKgEBTgAAAA8AAABBcHBsaWNhdGlvbk5hbWUADP////8AAAAAAwAAAAAoAAAA" +
           "VGhlIG5hbWUgb2YgdGhlIGFwcGxpY2F0aW9uIChtYW5kYXRvcnkpLgEAKgEBTAAAAA8AAABBcHBsaWNh" +
           "dGlvblR5cGUBADMB/////wAAAAADAAAAACQAAABUaGUgdHlwZSBvZiBhcHBsaWNhdGlvbiAobWFuZGF0" +
           "b3J5KS4BACoBAVAAAAAKAAAAUHJvZHVjdFVyaQAM/////wAAAAADAAAAAC8AAABUaGUgZ2xvYmFsbHkg" +
           "dW5pcXVlIGlkZW50aWZpZXIgZm9yIHRoZSBwcm9kdWN0LgEAKgEBVgAAABAAAABHYXRld2F5U2VydmVy" +
           "VXJpAAz/////AAAAAAMAAAAALwAAAFRoZSBnbG9iYWxseSB1bmlxdWUgaWRlbnRpZmllciBmb3IgdGhl" +
           "IHByb2R1Y3QuAQAqAQFwAAAADQAAAERpc2NvdmVyeVVybHMADAEAAAAAAAAAAwAAAABMAAAAVGhlIGVu" +
           "ZHBvaW50cyB3aGljaCB0aGUgYXBwbGljYXRpb24gaXMgY29uZmlndXJlZCB0byB1c2UgKGZvciBzZXJ2" +
           "ZXJzIG9ubHkpLgEAKAEBAAAAAQH/////AAAAABVgqQoCAAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQFn" +
           "AgAuAERnAgAAlgEAAAABACoBAVUAAAAVAAAAUmV2aXNlZEFwcGxpY2F0aW9uVXJpAAz/////AAAAAAMA" +
           "AAAAKQAAAFRoZSBpZCBhc3NpZ25lZCBieSB0aGUgZGlyZWN0b3J5IHNlcnZpY2UuAQAoAQEAAAABAf//" +
           "//8AAAAABGGCCgQAAAABABIAAABSZXF1ZXN0Q2VydGlmaWNhdGUBAQICAC8BAQICAgIAAAEB/////wIA" +
           "AAAVYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQEDAgAuAEQDAgAAlgYAAAABACoBAVAAAAAOAAAA" +
           "QXBwbGljYXRpb25VcmkADP////8AAAAAAwAAAAArAAAAVGhlIGdsb2JhbGx5IHVuaXF1ZSBpZCBmb3Ig" +
           "dGhlIGFwcGxpY2F0aW9uLgEAKgEBUAAAAAsAAABTdWJqZWN0TmFtZQAM/////wAAAAADAAAAAC4AAABU" +
           "aGUgdGhlIHN1YmplY3QgbmFtZSB0byB1c2UgaW4gdGhlIGNldGlmaWNhdGUuAQAqAQFMAAAACwAAAERv" +
           "bWFpbk5hbWVzAAwBAAAAAAAAAAMAAAAAKgAAAFRoZSBkb21haW5zIHdoaWNoIHRoZSBhcHBsaWNhdGlv" +
           "biBydW5zIG9uLgEAKgEBVwAAABAAAABQcml2YXRlS2V5Rm9ybWF0AAz/////AAAAAAMAAAAAMAAAAFRo" +
           "ZSBmb3JtYXQgZm9yIHRoZSBwcml2YXRlIGtleSAoJ1BFTScgb3IgJ1BGWCcpLgEAKgEBSgAAABIAAABQ" +
           "cml2YXRlS2V5UGFzc3dvcmQADP////8AAAAAAwAAAAAhAAAAVGhlIHBhc3N3b3JkIGZvciB0aGUgcHJp" +
           "dmF0ZSBrZXkuAQAqAQFUAAAAFgAAAENyZWF0ZUh0dHBzQ2VydGlmaWNhdGUAAf////8AAAAAAwAAAAAn" +
           "AAAAV2hldGhlciB0byBjcmVhdGUgYW4gSFRUUFMgY2VydGlmaWNhdGUuAQAoAQEAAAABAf////8AAAAA" +
           "FWCpCgIAAAAAAA8AAABPdXRwdXRBcmd1bWVudHMBAQQCAC4ARAQCAACWAQAAAAEAKgEBYwAAAAkAAABS" +
           "ZXF1ZXN0SWQAEf////8AAAAAAwAAAABDAAAAVGhlIGlkZW50aWZpZXIgYXNzaWduZWQgdGhlIHJlcXVl" +
           "c3QgKHVzZWQgdG8gcmV0cmlldmUgdGhlIHJlc3VsdHMpLgEAKAEBAAAAAQH/////AAAAAARhggoEAAAA" +
           "AQASAAAAQ2hlY2tSZXF1ZXN0U3RhdHVzAQFoAgAvAQFoAmgCAAABAf////8CAAAAFWCpCgIAAAAAAA4A" +
           "AABJbnB1dEFyZ3VtZW50cwEBaQIALgBEaQIAAJYBAAAAAQAqAQFEAAAACQAAAFJlcXVlc3RJZAAR////" +
           "/wAAAAADAAAAACQAAABUaGUgaWRlbnRpZmllciBhc3NpZ25lZCB0aGUgcmVxdWVzdC4BACgBAQAAAAEB" +
           "/////wAAAAAVYKkKAgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEBagIALgBEagIAAJYDAAAAAQAqAQFC" +
           "AAAACwAAAENlcnRpZmljYXRlAA//////AAAAAAMAAAAAIAAAAFRoZSBuZXcgYXBwbGljYXRpb24gY2Vy" +
           "dGlmaWNhdGUuAQAqAQFcAAAACgAAAFByaXZhdGVLZXkAD/////8AAAAAAwAAAAA7AAAAVGhlIG5ldyBh" +
           "cHBsaWNhdGlvbiBwcml2YXRlIGtleSB0byBnbyB3aXRoIHRoZSBjZXJ0aWZpY2F0ZS4BACoBAUEAAAAS" +
           "AAAASXNzdWVyQ2VydGlmaWNhdGVzAA8BAAAAAAAAAAMAAAAAGAAAAFRoZSBpc3N1ZXIgY2VydGlmaWNh" +
           "dGVzLgEAKAEBAAAAAQH/////AAAAAARhggoEAAAAAQAMAAAAR2V0VHJ1c3RMaXN0AQF1AgAvAQF1AnUC" +
           "AAABAf////8CAAAAFWCpCgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEBdgIALgBEdgIAAJYCAAAAAQAq" +
           "AQFQAAAADgAAAEFwcGxpY2F0aW9uVXJpAAz/////AAAAAAMAAAAAKwAAAFRoZSBnbG9iYWxseSB1bmlx" +
           "dWUgaWQgZm9yIHRoZSBhcHBsaWNhdGlvbi4BACoBAVMAAAAQAAAAUmV0dXJuSHR0cHNMaXN0cwAB////" +
           "/wAAAAADAAAAACwAAABXaGV0aGVyIHRvIHJldHVybiB0aGUgdHJ1c3QgbGlzdHMgZm9yIEhUVFBTLgEA" +
           "KAEBAAAAAQH/////AAAAABVgqQoCAAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQF3AgAuAER3AgAAlgUA" +
           "AAABACoBAUkAAAALAAAAVHJ1c3RMaXN0SWQAEf////8AAAAAAwAAAAAnAAAAQSB1bmlxdWUgaWRlbnRp" +
           "ZmllciBmb3IgdGhlIHRydXN0IGxpc3QuAQAqAQFWAAAAEwAAAFRydXN0ZWRDZXJ0aWZpY2F0ZXMADwEA" +
           "AAAAAAAAAwAAAAAsAAAAVGhlIGNlcnRpZmljYXRlcyB0cnVzdGVkIGJ5IHRoZSBhcHBsaWNhdGlvbi4B" +
           "ACoBAXYAAAAhAAAAVHJ1c3RlZENlcnRpZmljYXRlUmV2b2NhdGlvbkxpc3RzAA8BAAAAAAAAAAMAAAAA" +
           "PgAAAEFueSByZXZvY2F0aW9uIGxpc3RzIGFzc29jaWF0ZWQgd2l0aCB0aGUgdHJ1c3RlZCBjZXJ0aWZp" +
           "Y2F0ZXMuAQAqAQFbAAAAEgAAAElzc3VlckNlcnRpZmljYXRlcwAPAQAAAAAAAAADAAAAADIAAABUaGUg" +
           "aXNzdWVyIGNlcnRpZmljYXRlcyBuZWVkZWQgYnkgdGhlIGFwcGxpY2F0aW9uLgEAKgEBdAAAACAAAABJ" +
           "c3N1ZXJDZXJ0aWZpY2F0ZVJldm9jYXRpb25MaXN0cwAPAQAAAAAAAAADAAAAAD0AAABBbnkgcmV2b2Nh" +
           "dGlvbiBsaXN0cyBhc3NvY2lhdGVkIHdpdGggdGhlIGlzc3VlciBjZXJ0aWZpY2F0ZXMuAQAoAQEAAAAB" +
           "Af////8AAAAABGGCCgQAAAABABAAAABSZW5ld0NlcnRpZmljYXRlAQEhAgAvAQEhAiECAAABAf////8C" +
           "AAAAFWCpCgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEBIgIALgBEIgIAAJYDAAAAAQAqAQFuAAAADgAA" +
           "AEFwcGxpY2F0aW9uVXJpAAz/////AAAAAAMAAAAASQAAAFRoZSBnbG9iYWxseSB1bmlxdWUgaWQgZm9y" +
           "IHRoZSBhcHBsaWNhdGlvbiB3aXRoIHRoZSBjZXJ0aWZpY2F0ZSB0byByZW5ldy4BACoBAVcAAAAQAAAA" +
           "UHJpdmF0ZUtleUZvcm1hdAAM/////wAAAAADAAAAADAAAABUaGUgZm9ybWF0IGZvciB0aGUgcHJpdmF0" +
           "ZSBrZXkgKCdQRU0nIG9yICdQRlgnKS4BACoBAUoAAAASAAAAUHJpdmF0ZUtleVBhc3N3b3JkAAz/////" +
           "AAAAAAMAAAAAIQAAAFRoZSBwYXNzd29yZCBmb3IgdGhlIHByaXZhdGUga2V5LgEAKAEBAAAAAQH/////" +
           "AAAAABVgqQoCAAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQEjAgAuAEQjAgAAlgEAAAABACoBAWMAAAAJ" +
           "AAAAUmVxdWVzdElkABH/////AAAAAAMAAAAAQwAAAFRoZSBpZGVudGlmaWVyIGFzc2lnbmVkIHRoZSBy" +
           "ZXF1ZXN0ICh1c2VkIHRvIHJldHJpZXZlIHRoZSByZXN1bHRzKS4BACgBAQAAAAEB/////wAAAAAEYYIK" +
           "BAAAAAEAEQAAAFJldm9rZUNlcnRpZmljYXRlAQEkAgAvAQEkAiQCAAABAf////8BAAAAFWCpCgIAAAAA" +
           "AA4AAABJbnB1dEFyZ3VtZW50cwEBJQIALgBEJQIAAJYBAAAAAQAqAQE8AAAACwAAAENlcnRpZmljYXRl" +
           "AAz/////AAAAAAMAAAAAGgAAAFRoZSBjZXJ0aWZpY2F0ZSB0byByZXZva2UuAQAoAQEAAAABAf////8A" +
           "AAAABGGCCgQAAAABAAwAAABRdWVyeVNlcnZlcnMBASYCAC8BASYCJgIAAAEB/////wIAAAAVYKkKAgAA" +
           "AAAADgAAAElucHV0QXJndW1lbnRzAQEnAgAuAEQnAgAAlgUAAAABACoBAUoAAAAJAAAARWxlbWVudElk" +
           "ABH/////AAAAAAMAAAAAKgAAAFRoZSBOb2RlSWQgb2YgdGhlIFN5c3RlbUVsZW1lbnQgdG8gc2VhcmNo" +
           "LgEAKgEBYgAAAA8AAABBcHBsaWNhdGlvbk5hbWUADP////8AAAAAAwAAAAA8AAAAQSBwYXR0ZXJuIHVz" +
           "ZWQgdG8gbWF0Y2ggdGhlIEFwcGxpY2F0aW9uTmFtZXMgb2YgdGhlIFNlcnZlcnMuAQAqAQFaAAAACwAA" +
           "AE1hY2hpbmVOYW1lAAz/////AAAAAAMAAAAAOAAAAEEgcGF0dGVybiB1c2VkIHRvIG1hdGNoIHRoZSBN" +
           "YWNoaW5lTmFtZXMgb2YgdGhlIFNlcnZlcnMuAQAqAQFgAAAADgAAAEFwcGxpY2F0aW9uVXJpAAz/////" +
           "AAAAAAMAAAAAOwAAAEEgcGF0dGVybiB1c2VkIHRvIG1hdGNoIHRoZSBBcHBsaWNhdGlvblVyaXMgb2Yg" +
           "dGhlIFNlcnZlcnMuAQAqAQFYAAAACgAAAFByb2R1Y3RVcmkADP////8AAAAAAwAAAAA3AAAAQSBwYXR0" +
           "ZXJuIHVzZWQgdG8gbWF0Y2ggdGhlIFByb2R1Y3RVcmlzIG9mIHRoZSBTZXJ2ZXJzLgEAKAEBAAAAAQH/" +
           "////AAAAABVgqQoCAAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQEoAgAuAEQoAgAAlgEAAAABACoBAVIA" +
           "AAAHAAAAU2VydmVycwEANAEBAAAAAAAAAAMAAAAAMgAAAFRoZSBsaXN0IG9mIFNlcnZlcnMgdGhhdCBt" +
           "ZWV0IHRoZSBzZWFyY2ggY3JpdGVyaWEuAQAoAQEAAAABAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the CertificateRequests Object.
        /// </summary>
        public FolderState CertificateRequests
        {
            get
            { 
                return m_certificateRequests;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_certificateRequests, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_certificateRequests = value;
            }
        }

        /// <summary>
        /// A description for the Applications Object.
        /// </summary>
        public FolderState Applications
        {
            get
            { 
                return m_applications;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_applications, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_applications = value;
            }
        }

        /// <summary>
        /// A description for the RegisterApplicationMethodType Method.
        /// </summary>
        public RegisterApplicationMethodState RegisterApplication
        {
            get
            { 
                return m_registerApplicationMethod;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_registerApplicationMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_registerApplicationMethod = value;
            }
        }

        /// <summary>
        /// A description for the RequestCertificateMethodType Method.
        /// </summary>
        public RequestCertificateMethodState RequestCertificate
        {
            get
            { 
                return m_requestCertificateMethod;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_requestCertificateMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_requestCertificateMethod = value;
            }
        }

        /// <summary>
        /// A description for the CheckRequestStatusMethodType Method.
        /// </summary>
        public CheckRequestStatusMethodState CheckRequestStatus
        {
            get
            { 
                return m_checkRequestStatusMethod;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_checkRequestStatusMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_checkRequestStatusMethod = value;
            }
        }

        /// <summary>
        /// A description for the GetTrustListMethodType Method.
        /// </summary>
        public GetTrustListMethodState GetTrustList
        {
            get
            { 
                return m_getTrustListMethod;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_getTrustListMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_getTrustListMethod = value;
            }
        }

        /// <summary>
        /// A description for the RenewCertificateMethodType Method.
        /// </summary>
        public RenewCertificateMethodState RenewCertificate
        {
            get
            { 
                return m_renewCertificateMethod;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_renewCertificateMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_renewCertificateMethod = value;
            }
        }

        /// <summary>
        /// A description for the RevokeCertificateMethodType Method.
        /// </summary>
        public RevokeCertificateMethodState RevokeCertificate
        {
            get
            { 
                return m_revokeCertificateMethod;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_revokeCertificateMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_revokeCertificateMethod = value;
            }
        }

        /// <summary>
        /// A description for the QueryServersMethodType Method.
        /// </summary>
        public QueryServersMethodState QueryServers
        {
            get
            { 
                return m_queryServersMethod;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_queryServersMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_queryServersMethod = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_certificateRequests != null)
            {
                children.Add(m_certificateRequests);
            }

            if (m_applications != null)
            {
                children.Add(m_applications);
            }

            if (m_registerApplicationMethod != null)
            {
                children.Add(m_registerApplicationMethod);
            }

            if (m_requestCertificateMethod != null)
            {
                children.Add(m_requestCertificateMethod);
            }

            if (m_checkRequestStatusMethod != null)
            {
                children.Add(m_checkRequestStatusMethod);
            }

            if (m_getTrustListMethod != null)
            {
                children.Add(m_getTrustListMethod);
            }

            if (m_renewCertificateMethod != null)
            {
                children.Add(m_renewCertificateMethod);
            }

            if (m_revokeCertificateMethod != null)
            {
                children.Add(m_revokeCertificateMethod);
            }

            if (m_queryServersMethod != null)
            {
                children.Add(m_queryServersMethod);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case Opc.Ua.Gds.BrowseNames.CertificateRequests:
                {
                    if (createOrReplace)
                    {
                        if (CertificateRequests == null)
                        {
                            if (replacement == null)
                            {
                                CertificateRequests = new FolderState(this);
                            }
                            else
                            {
                                CertificateRequests = (FolderState)replacement;
                            }
                        }
                    }

                    instance = CertificateRequests;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.Applications:
                {
                    if (createOrReplace)
                    {
                        if (Applications == null)
                        {
                            if (replacement == null)
                            {
                                Applications = new FolderState(this);
                            }
                            else
                            {
                                Applications = (FolderState)replacement;
                            }
                        }
                    }

                    instance = Applications;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.RegisterApplication:
                {
                    if (createOrReplace)
                    {
                        if (RegisterApplication == null)
                        {
                            if (replacement == null)
                            {
                                RegisterApplication = new RegisterApplicationMethodState(this);
                            }
                            else
                            {
                                RegisterApplication = (RegisterApplicationMethodState)replacement;
                            }
                        }
                    }

                    instance = RegisterApplication;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.RequestCertificate:
                {
                    if (createOrReplace)
                    {
                        if (RequestCertificate == null)
                        {
                            if (replacement == null)
                            {
                                RequestCertificate = new RequestCertificateMethodState(this);
                            }
                            else
                            {
                                RequestCertificate = (RequestCertificateMethodState)replacement;
                            }
                        }
                    }

                    instance = RequestCertificate;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.CheckRequestStatus:
                {
                    if (createOrReplace)
                    {
                        if (CheckRequestStatus == null)
                        {
                            if (replacement == null)
                            {
                                CheckRequestStatus = new CheckRequestStatusMethodState(this);
                            }
                            else
                            {
                                CheckRequestStatus = (CheckRequestStatusMethodState)replacement;
                            }
                        }
                    }

                    instance = CheckRequestStatus;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.GetTrustList:
                {
                    if (createOrReplace)
                    {
                        if (GetTrustList == null)
                        {
                            if (replacement == null)
                            {
                                GetTrustList = new GetTrustListMethodState(this);
                            }
                            else
                            {
                                GetTrustList = (GetTrustListMethodState)replacement;
                            }
                        }
                    }

                    instance = GetTrustList;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.RenewCertificate:
                {
                    if (createOrReplace)
                    {
                        if (RenewCertificate == null)
                        {
                            if (replacement == null)
                            {
                                RenewCertificate = new RenewCertificateMethodState(this);
                            }
                            else
                            {
                                RenewCertificate = (RenewCertificateMethodState)replacement;
                            }
                        }
                    }

                    instance = RenewCertificate;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.RevokeCertificate:
                {
                    if (createOrReplace)
                    {
                        if (RevokeCertificate == null)
                        {
                            if (replacement == null)
                            {
                                RevokeCertificate = new RevokeCertificateMethodState(this);
                            }
                            else
                            {
                                RevokeCertificate = (RevokeCertificateMethodState)replacement;
                            }
                        }
                    }

                    instance = RevokeCertificate;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.QueryServers:
                {
                    if (createOrReplace)
                    {
                        if (QueryServers == null)
                        {
                            if (replacement == null)
                            {
                                QueryServers = new QueryServersMethodState(this);
                            }
                            else
                            {
                                QueryServers = (QueryServersMethodState)replacement;
                            }
                        }
                    }

                    instance = QueryServers;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private FolderState m_certificateRequests;
        private FolderState m_applications;
        private RegisterApplicationMethodState m_registerApplicationMethod;
        private RequestCertificateMethodState m_requestCertificateMethod;
        private CheckRequestStatusMethodState m_checkRequestStatusMethod;
        private GetTrustListMethodState m_getTrustListMethod;
        private RenewCertificateMethodState m_renewCertificateMethod;
        private RevokeCertificateMethodState m_revokeCertificateMethod;
        private QueryServersMethodState m_queryServersMethod;
        #endregion
    }
    #endif
    #endregion

    #region SystemElementState Class
    #if (!OPCUA_EXCLUDE_SystemElementState)
    /// <summary>
    /// Stores an instance of the SystemElementType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class SystemElementState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public SystemElementState(NodeState parent) : base(parent)
        {
        }
        
        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(Opc.Ua.Gds.ObjectTypes.SystemElementType, Opc.Ua.Gds.Namespaces.OpcUaGds, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYIAAAQAAAAEAGQAAAFN5" +
           "c3RlbUVsZW1lbnRUeXBlSW5zdGFuY2UBAToCAQE6Av////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        #endregion

        #region Private Fields
        #endregion
    }
    #endif
    #endregion

    #region AddressableElementState Class
    #if (!OPCUA_EXCLUDE_AddressableElementState)
    /// <summary>
    /// Stores an instance of the AddressableElementType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class AddressableElementState : SystemElementState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public AddressableElementState(NodeState parent) : base(parent)
        {
        }
        
        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(Opc.Ua.Gds.ObjectTypes.AddressableElementType, Opc.Ua.Gds.Namespaces.OpcUaGds, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYIAAAQAAAAEAHgAAAEFk" +
           "ZHJlc3NhYmxlRWxlbWVudFR5cGVJbnN0YW5jZQEBOwIBATsC/////wEAAAAVYIkKAgAAAAEABwAAAERu" +
           "c05hbWUBAXgCAC4ARHgCAAAADP////8BAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the DnsName Property.
        /// </summary>
        public PropertyState<string> DnsName
        {
            get
            { 
                return m_dnsName;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_dnsName, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_dnsName = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_dnsName != null)
            {
                children.Add(m_dnsName);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case Opc.Ua.Gds.BrowseNames.DnsName:
                {
                    if (createOrReplace)
                    {
                        if (DnsName == null)
                        {
                            if (replacement == null)
                            {
                                DnsName = new PropertyState<string>(this);
                            }
                            else
                            {
                                DnsName = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = DnsName;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private PropertyState<string> m_dnsName;
        #endregion
    }
    #endif
    #endregion

    #region ApplicationElementState Class
    #if (!OPCUA_EXCLUDE_ApplicationElementState)
    /// <summary>
    /// Stores an instance of the ApplicationElementType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ApplicationElementState : SystemElementState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public ApplicationElementState(NodeState parent) : base(parent)
        {
        }
        
        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(Opc.Ua.Gds.ObjectTypes.ApplicationElementType, Opc.Ua.Gds.Namespaces.OpcUaGds, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString = 
           "AQAAACAAAABodHRwOi8vb3BjZm91bmRhdGlvbi5vcmcvVUEvR0RTL/////8EYIAAAQAAAAEAHgAAAEFw" +
           "cGxpY2F0aW9uRWxlbWVudFR5cGVJbnN0YW5jZQEBPAIBATwC/////wUAAAAVYIkKAgAAAAEADgAAAEFw" +
           "cGxpY2F0aW9uVXJpAQE9AgAuAEQ9AgAAAAz/////AQH/////AAAAABVgiQoCAAAAAQAKAAAAUHJvZHVj" +
           "dFVyaQEBeQIALgBEeQIAAAAM/////wEB/////wAAAAAVYIkKAgAAAAEADwAAAEFwcGxpY2F0aW9uVHlw" +
           "ZQEBPgIALgBEPgIAAAAM/////wEB/////wAAAAAVYIkKAgAAAAEACwAAAE1hY2hpbmVOYW1lAQFEAgAu" +
           "AEREAgAAAAz/////AQH/////AAAAABVgiQoCAAAAAQANAAAARGlzY292ZXJ5VXJscwEBPwIALgBEPwIA" +
           "AAAMAQAAAAEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the ApplicationUri Property.
        /// </summary>
        public PropertyState<string> ApplicationUri
        {
            get
            { 
                return m_applicationUri;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_applicationUri, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_applicationUri = value;
            }
        }

        /// <summary>
        /// A description for the ProductUri Property.
        /// </summary>
        public PropertyState<string> ProductUri
        {
            get
            { 
                return m_productUri;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_productUri, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_productUri = value;
            }
        }

        /// <summary>
        /// A description for the ApplicationType Property.
        /// </summary>
        public PropertyState<string> ApplicationType
        {
            get
            { 
                return m_applicationType;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_applicationType, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_applicationType = value;
            }
        }

        /// <summary>
        /// A description for the MachineName Property.
        /// </summary>
        public PropertyState<string> MachineName
        {
            get
            { 
                return m_machineName;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_machineName, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_machineName = value;
            }
        }

        /// <summary>
        /// A description for the DiscoveryUrls Property.
        /// </summary>
        public PropertyState<string[]> DiscoveryUrls
        {
            get
            { 
                return m_discoveryUrls;  
            }
            
            set
            {
                if (!Object.ReferenceEquals(m_discoveryUrls, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_discoveryUrls = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_applicationUri != null)
            {
                children.Add(m_applicationUri);
            }

            if (m_productUri != null)
            {
                children.Add(m_productUri);
            }

            if (m_applicationType != null)
            {
                children.Add(m_applicationType);
            }

            if (m_machineName != null)
            {
                children.Add(m_machineName);
            }

            if (m_discoveryUrls != null)
            {
                children.Add(m_discoveryUrls);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case Opc.Ua.Gds.BrowseNames.ApplicationUri:
                {
                    if (createOrReplace)
                    {
                        if (ApplicationUri == null)
                        {
                            if (replacement == null)
                            {
                                ApplicationUri = new PropertyState<string>(this);
                            }
                            else
                            {
                                ApplicationUri = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = ApplicationUri;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.ProductUri:
                {
                    if (createOrReplace)
                    {
                        if (ProductUri == null)
                        {
                            if (replacement == null)
                            {
                                ProductUri = new PropertyState<string>(this);
                            }
                            else
                            {
                                ProductUri = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = ProductUri;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.ApplicationType:
                {
                    if (createOrReplace)
                    {
                        if (ApplicationType == null)
                        {
                            if (replacement == null)
                            {
                                ApplicationType = new PropertyState<string>(this);
                            }
                            else
                            {
                                ApplicationType = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = ApplicationType;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.MachineName:
                {
                    if (createOrReplace)
                    {
                        if (MachineName == null)
                        {
                            if (replacement == null)
                            {
                                MachineName = new PropertyState<string>(this);
                            }
                            else
                            {
                                MachineName = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = MachineName;
                    break;
                }

                case Opc.Ua.Gds.BrowseNames.DiscoveryUrls:
                {
                    if (createOrReplace)
                    {
                        if (DiscoveryUrls == null)
                        {
                            if (replacement == null)
                            {
                                DiscoveryUrls = new PropertyState<string[]>(this);
                            }
                            else
                            {
                                DiscoveryUrls = (PropertyState<string[]>)replacement;
                            }
                        }
                    }

                    instance = DiscoveryUrls;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private PropertyState<string> m_applicationUri;
        private PropertyState<string> m_productUri;
        private PropertyState<string> m_applicationType;
        private PropertyState<string> m_machineName;
        private PropertyState<string[]> m_discoveryUrls;
        #endregion
    }
    #endif
    #endregion
}
