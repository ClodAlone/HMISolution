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
    #region Method Identifiers
    /// <summary>
    /// A class that declares constants for all Methods in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Methods
    {
        /// <summary>
        /// The identifier for the RegisterApplicationMethodType Method.
        /// </summary>
        public const uint RegisterApplicationMethodType = 607;

        /// <summary>
        /// The identifier for the RequestCertificateMethodType Method.
        /// </summary>
        public const uint RequestCertificateMethodType = 441;

        /// <summary>
        /// The identifier for the RenewCertificateMethodType Method.
        /// </summary>
        public const uint RenewCertificateMethodType = 537;

        /// <summary>
        /// The identifier for the RevokeCertificateMethodType Method.
        /// </summary>
        public const uint RevokeCertificateMethodType = 540;

        /// <summary>
        /// The identifier for the CheckRequestStatusMethodType Method.
        /// </summary>
        public const uint CheckRequestStatusMethodType = 610;

        /// <summary>
        /// The identifier for the GetTrustListMethodType Method.
        /// </summary>
        public const uint GetTrustListMethodType = 625;

        /// <summary>
        /// The identifier for the QueryServersMethodType Method.
        /// </summary>
        public const uint QueryServersMethodType = 542;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RegisterApplication Method.
        /// </summary>
        public const uint RootDirectoryEntryType_RegisterApplication = 613;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RequestCertificate Method.
        /// </summary>
        public const uint RootDirectoryEntryType_RequestCertificate = 514;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_CheckRequestStatus Method.
        /// </summary>
        public const uint RootDirectoryEntryType_CheckRequestStatus = 616;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_GetTrustList Method.
        /// </summary>
        public const uint RootDirectoryEntryType_GetTrustList = 629;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RenewCertificate Method.
        /// </summary>
        public const uint RootDirectoryEntryType_RenewCertificate = 545;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RevokeCertificate Method.
        /// </summary>
        public const uint RootDirectoryEntryType_RevokeCertificate = 548;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_QueryServers Method.
        /// </summary>
        public const uint RootDirectoryEntryType_QueryServers = 550;

        /// <summary>
        /// The identifier for the Directory_RegisterApplication Method.
        /// </summary>
        public const uint Directory_RegisterApplication = 619;

        /// <summary>
        /// The identifier for the Directory_RequestCertificate Method.
        /// </summary>
        public const uint Directory_RequestCertificate = 590;

        /// <summary>
        /// The identifier for the Directory_CheckRequestStatus Method.
        /// </summary>
        public const uint Directory_CheckRequestStatus = 622;

        /// <summary>
        /// The identifier for the Directory_GetTrustList Method.
        /// </summary>
        public const uint Directory_GetTrustList = 634;

        /// <summary>
        /// The identifier for the Directory_RenewCertificate Method.
        /// </summary>
        public const uint Directory_RenewCertificate = 599;

        /// <summary>
        /// The identifier for the Directory_RevokeCertificate Method.
        /// </summary>
        public const uint Directory_RevokeCertificate = 602;

        /// <summary>
        /// The identifier for the Directory_QueryServers Method.
        /// </summary>
        public const uint Directory_QueryServers = 604;
    }
    #endregion

    #region Object Identifiers
    /// <summary>
    /// A class that declares constants for all Objects in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Objects
    {
        /// <summary>
        /// The identifier for the RootDirectoryEntryType_CertificateRequests Object.
        /// </summary>
        public const uint RootDirectoryEntryType_CertificateRequests = 510;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_Applications Object.
        /// </summary>
        public const uint RootDirectoryEntryType_Applications = 577;

        /// <summary>
        /// The identifier for the Directory Object.
        /// </summary>
        public const uint Directory = 584;

        /// <summary>
        /// The identifier for the Directory_CertificateRequests Object.
        /// </summary>
        public const uint Directory_CertificateRequests = 585;

        /// <summary>
        /// The identifier for the Directory_Applications Object.
        /// </summary>
        public const uint Directory_Applications = 586;
    }
    #endregion

    #region ObjectType Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypes
    {
        /// <summary>
        /// The identifier for the CertificateRequestType ObjectType.
        /// </summary>
        public const uint CertificateRequestType = 450;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType ObjectType.
        /// </summary>
        public const uint RootDirectoryEntryType = 509;

        /// <summary>
        /// The identifier for the SystemElementType ObjectType.
        /// </summary>
        public const uint SystemElementType = 570;

        /// <summary>
        /// The identifier for the AddressableElementType ObjectType.
        /// </summary>
        public const uint AddressableElementType = 571;

        /// <summary>
        /// The identifier for the ApplicationElementType ObjectType.
        /// </summary>
        public const uint ApplicationElementType = 572;
    }
    #endregion

    #region Variable Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Variables
    {
        /// <summary>
        /// The identifier for the RegisterApplicationMethodType_InputArguments Variable.
        /// </summary>
        public const uint RegisterApplicationMethodType_InputArguments = 608;

        /// <summary>
        /// The identifier for the RegisterApplicationMethodType_OutputArguments Variable.
        /// </summary>
        public const uint RegisterApplicationMethodType_OutputArguments = 609;

        /// <summary>
        /// The identifier for the RequestCertificateMethodType_InputArguments Variable.
        /// </summary>
        public const uint RequestCertificateMethodType_InputArguments = 442;

        /// <summary>
        /// The identifier for the RequestCertificateMethodType_OutputArguments Variable.
        /// </summary>
        public const uint RequestCertificateMethodType_OutputArguments = 443;

        /// <summary>
        /// The identifier for the RenewCertificateMethodType_InputArguments Variable.
        /// </summary>
        public const uint RenewCertificateMethodType_InputArguments = 538;

        /// <summary>
        /// The identifier for the RenewCertificateMethodType_OutputArguments Variable.
        /// </summary>
        public const uint RenewCertificateMethodType_OutputArguments = 539;

        /// <summary>
        /// The identifier for the RevokeCertificateMethodType_InputArguments Variable.
        /// </summary>
        public const uint RevokeCertificateMethodType_InputArguments = 541;

        /// <summary>
        /// The identifier for the CheckRequestStatusMethodType_InputArguments Variable.
        /// </summary>
        public const uint CheckRequestStatusMethodType_InputArguments = 611;

        /// <summary>
        /// The identifier for the CheckRequestStatusMethodType_OutputArguments Variable.
        /// </summary>
        public const uint CheckRequestStatusMethodType_OutputArguments = 612;

        /// <summary>
        /// The identifier for the GetTrustListMethodType_InputArguments Variable.
        /// </summary>
        public const uint GetTrustListMethodType_InputArguments = 626;

        /// <summary>
        /// The identifier for the GetTrustListMethodType_OutputArguments Variable.
        /// </summary>
        public const uint GetTrustListMethodType_OutputArguments = 627;

        /// <summary>
        /// The identifier for the QueryServersMethodType_InputArguments Variable.
        /// </summary>
        public const uint QueryServersMethodType_InputArguments = 543;

        /// <summary>
        /// The identifier for the QueryServersMethodType_OutputArguments Variable.
        /// </summary>
        public const uint QueryServersMethodType_OutputArguments = 544;

        /// <summary>
        /// The identifier for the CertificateRequestType_ApplicationUri Variable.
        /// </summary>
        public const uint CertificateRequestType_ApplicationUri = 504;

        /// <summary>
        /// The identifier for the CertificateRequestType_ProductUri Variable.
        /// </summary>
        public const uint CertificateRequestType_ProductUri = 628;

        /// <summary>
        /// The identifier for the CertificateRequestType_ApplicationType Variable.
        /// </summary>
        public const uint CertificateRequestType_ApplicationType = 505;

        /// <summary>
        /// The identifier for the CertificateRequestType_MachineName Variable.
        /// </summary>
        public const uint CertificateRequestType_MachineName = 581;

        /// <summary>
        /// The identifier for the CertificateRequestType_SubjectName Variable.
        /// </summary>
        public const uint CertificateRequestType_SubjectName = 507;

        /// <summary>
        /// The identifier for the CertificateRequestType_DomainNames Variable.
        /// </summary>
        public const uint CertificateRequestType_DomainNames = 508;

        /// <summary>
        /// The identifier for the CertificateRequestType_IsHttpsCertificate Variable.
        /// </summary>
        public const uint CertificateRequestType_IsHttpsCertificate = 582;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RegisterApplication_InputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_RegisterApplication_InputArguments = 614;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RegisterApplication_OutputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_RegisterApplication_OutputArguments = 615;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RequestCertificate_InputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_RequestCertificate_InputArguments = 515;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RequestCertificate_OutputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_RequestCertificate_OutputArguments = 516;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_CheckRequestStatus_InputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_CheckRequestStatus_InputArguments = 617;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_CheckRequestStatus_OutputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_CheckRequestStatus_OutputArguments = 618;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_GetTrustList_InputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_GetTrustList_InputArguments = 630;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_GetTrustList_OutputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_GetTrustList_OutputArguments = 631;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RenewCertificate_InputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_RenewCertificate_InputArguments = 546;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RenewCertificate_OutputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_RenewCertificate_OutputArguments = 547;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RevokeCertificate_InputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_RevokeCertificate_InputArguments = 549;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_QueryServers_InputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_QueryServers_InputArguments = 551;

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_QueryServers_OutputArguments Variable.
        /// </summary>
        public const uint RootDirectoryEntryType_QueryServers_OutputArguments = 552;

        /// <summary>
        /// The identifier for the AddressableElementType_DnsName Variable.
        /// </summary>
        public const uint AddressableElementType_DnsName = 632;

        /// <summary>
        /// The identifier for the ApplicationElementType_ApplicationUri Variable.
        /// </summary>
        public const uint ApplicationElementType_ApplicationUri = 573;

        /// <summary>
        /// The identifier for the ApplicationElementType_ProductUri Variable.
        /// </summary>
        public const uint ApplicationElementType_ProductUri = 633;

        /// <summary>
        /// The identifier for the ApplicationElementType_ApplicationType Variable.
        /// </summary>
        public const uint ApplicationElementType_ApplicationType = 574;

        /// <summary>
        /// The identifier for the ApplicationElementType_MachineName Variable.
        /// </summary>
        public const uint ApplicationElementType_MachineName = 580;

        /// <summary>
        /// The identifier for the ApplicationElementType_DiscoveryUrls Variable.
        /// </summary>
        public const uint ApplicationElementType_DiscoveryUrls = 575;

        /// <summary>
        /// The identifier for the Directory_RegisterApplication_InputArguments Variable.
        /// </summary>
        public const uint Directory_RegisterApplication_InputArguments = 620;

        /// <summary>
        /// The identifier for the Directory_RegisterApplication_OutputArguments Variable.
        /// </summary>
        public const uint Directory_RegisterApplication_OutputArguments = 621;

        /// <summary>
        /// The identifier for the Directory_RequestCertificate_InputArguments Variable.
        /// </summary>
        public const uint Directory_RequestCertificate_InputArguments = 591;

        /// <summary>
        /// The identifier for the Directory_RequestCertificate_OutputArguments Variable.
        /// </summary>
        public const uint Directory_RequestCertificate_OutputArguments = 592;

        /// <summary>
        /// The identifier for the Directory_CheckRequestStatus_InputArguments Variable.
        /// </summary>
        public const uint Directory_CheckRequestStatus_InputArguments = 623;

        /// <summary>
        /// The identifier for the Directory_CheckRequestStatus_OutputArguments Variable.
        /// </summary>
        public const uint Directory_CheckRequestStatus_OutputArguments = 624;

        /// <summary>
        /// The identifier for the Directory_GetTrustList_InputArguments Variable.
        /// </summary>
        public const uint Directory_GetTrustList_InputArguments = 635;

        /// <summary>
        /// The identifier for the Directory_GetTrustList_OutputArguments Variable.
        /// </summary>
        public const uint Directory_GetTrustList_OutputArguments = 636;

        /// <summary>
        /// The identifier for the Directory_RenewCertificate_InputArguments Variable.
        /// </summary>
        public const uint Directory_RenewCertificate_InputArguments = 600;

        /// <summary>
        /// The identifier for the Directory_RenewCertificate_OutputArguments Variable.
        /// </summary>
        public const uint Directory_RenewCertificate_OutputArguments = 601;

        /// <summary>
        /// The identifier for the Directory_RevokeCertificate_InputArguments Variable.
        /// </summary>
        public const uint Directory_RevokeCertificate_InputArguments = 603;

        /// <summary>
        /// The identifier for the Directory_QueryServers_InputArguments Variable.
        /// </summary>
        public const uint Directory_QueryServers_InputArguments = 605;

        /// <summary>
        /// The identifier for the Directory_QueryServers_OutputArguments Variable.
        /// </summary>
        public const uint Directory_QueryServers_OutputArguments = 606;
    }
    #endregion

    #region Method Node Identifiers
    /// <summary>
    /// A class that declares constants for all Methods in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class MethodIds
    {
        /// <summary>
        /// The identifier for the RegisterApplicationMethodType Method.
        /// </summary>
        public static readonly ExpandedNodeId RegisterApplicationMethodType = new ExpandedNodeId(Opc.Ua.Gds.Methods.RegisterApplicationMethodType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RequestCertificateMethodType Method.
        /// </summary>
        public static readonly ExpandedNodeId RequestCertificateMethodType = new ExpandedNodeId(Opc.Ua.Gds.Methods.RequestCertificateMethodType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RenewCertificateMethodType Method.
        /// </summary>
        public static readonly ExpandedNodeId RenewCertificateMethodType = new ExpandedNodeId(Opc.Ua.Gds.Methods.RenewCertificateMethodType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RevokeCertificateMethodType Method.
        /// </summary>
        public static readonly ExpandedNodeId RevokeCertificateMethodType = new ExpandedNodeId(Opc.Ua.Gds.Methods.RevokeCertificateMethodType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CheckRequestStatusMethodType Method.
        /// </summary>
        public static readonly ExpandedNodeId CheckRequestStatusMethodType = new ExpandedNodeId(Opc.Ua.Gds.Methods.CheckRequestStatusMethodType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the GetTrustListMethodType Method.
        /// </summary>
        public static readonly ExpandedNodeId GetTrustListMethodType = new ExpandedNodeId(Opc.Ua.Gds.Methods.GetTrustListMethodType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the QueryServersMethodType Method.
        /// </summary>
        public static readonly ExpandedNodeId QueryServersMethodType = new ExpandedNodeId(Opc.Ua.Gds.Methods.QueryServersMethodType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RegisterApplication Method.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RegisterApplication = new ExpandedNodeId(Opc.Ua.Gds.Methods.RootDirectoryEntryType_RegisterApplication, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RequestCertificate Method.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RequestCertificate = new ExpandedNodeId(Opc.Ua.Gds.Methods.RootDirectoryEntryType_RequestCertificate, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_CheckRequestStatus Method.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_CheckRequestStatus = new ExpandedNodeId(Opc.Ua.Gds.Methods.RootDirectoryEntryType_CheckRequestStatus, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_GetTrustList Method.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_GetTrustList = new ExpandedNodeId(Opc.Ua.Gds.Methods.RootDirectoryEntryType_GetTrustList, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RenewCertificate Method.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RenewCertificate = new ExpandedNodeId(Opc.Ua.Gds.Methods.RootDirectoryEntryType_RenewCertificate, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RevokeCertificate Method.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RevokeCertificate = new ExpandedNodeId(Opc.Ua.Gds.Methods.RootDirectoryEntryType_RevokeCertificate, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_QueryServers Method.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_QueryServers = new ExpandedNodeId(Opc.Ua.Gds.Methods.RootDirectoryEntryType_QueryServers, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RegisterApplication Method.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RegisterApplication = new ExpandedNodeId(Opc.Ua.Gds.Methods.Directory_RegisterApplication, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RequestCertificate Method.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RequestCertificate = new ExpandedNodeId(Opc.Ua.Gds.Methods.Directory_RequestCertificate, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_CheckRequestStatus Method.
        /// </summary>
        public static readonly ExpandedNodeId Directory_CheckRequestStatus = new ExpandedNodeId(Opc.Ua.Gds.Methods.Directory_CheckRequestStatus, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_GetTrustList Method.
        /// </summary>
        public static readonly ExpandedNodeId Directory_GetTrustList = new ExpandedNodeId(Opc.Ua.Gds.Methods.Directory_GetTrustList, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RenewCertificate Method.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RenewCertificate = new ExpandedNodeId(Opc.Ua.Gds.Methods.Directory_RenewCertificate, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RevokeCertificate Method.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RevokeCertificate = new ExpandedNodeId(Opc.Ua.Gds.Methods.Directory_RevokeCertificate, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_QueryServers Method.
        /// </summary>
        public static readonly ExpandedNodeId Directory_QueryServers = new ExpandedNodeId(Opc.Ua.Gds.Methods.Directory_QueryServers, Opc.Ua.Gds.Namespaces.OpcUaGds);
    }
    #endregion

    #region Object Node Identifiers
    /// <summary>
    /// A class that declares constants for all Objects in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectIds
    {
        /// <summary>
        /// The identifier for the RootDirectoryEntryType_CertificateRequests Object.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_CertificateRequests = new ExpandedNodeId(Opc.Ua.Gds.Objects.RootDirectoryEntryType_CertificateRequests, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_Applications Object.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_Applications = new ExpandedNodeId(Opc.Ua.Gds.Objects.RootDirectoryEntryType_Applications, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory Object.
        /// </summary>
        public static readonly ExpandedNodeId Directory = new ExpandedNodeId(Opc.Ua.Gds.Objects.Directory, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_CertificateRequests Object.
        /// </summary>
        public static readonly ExpandedNodeId Directory_CertificateRequests = new ExpandedNodeId(Opc.Ua.Gds.Objects.Directory_CertificateRequests, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_Applications Object.
        /// </summary>
        public static readonly ExpandedNodeId Directory_Applications = new ExpandedNodeId(Opc.Ua.Gds.Objects.Directory_Applications, Opc.Ua.Gds.Namespaces.OpcUaGds);
    }
    #endregion

    #region ObjectType Node Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypeIds
    {
        /// <summary>
        /// The identifier for the CertificateRequestType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId CertificateRequestType = new ExpandedNodeId(Opc.Ua.Gds.ObjectTypes.CertificateRequestType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType = new ExpandedNodeId(Opc.Ua.Gds.ObjectTypes.RootDirectoryEntryType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the SystemElementType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId SystemElementType = new ExpandedNodeId(Opc.Ua.Gds.ObjectTypes.SystemElementType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the AddressableElementType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId AddressableElementType = new ExpandedNodeId(Opc.Ua.Gds.ObjectTypes.AddressableElementType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the ApplicationElementType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId ApplicationElementType = new ExpandedNodeId(Opc.Ua.Gds.ObjectTypes.ApplicationElementType, Opc.Ua.Gds.Namespaces.OpcUaGds);
    }
    #endregion

    #region Variable Node Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableIds
    {
        /// <summary>
        /// The identifier for the RegisterApplicationMethodType_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RegisterApplicationMethodType_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RegisterApplicationMethodType_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RegisterApplicationMethodType_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RegisterApplicationMethodType_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RegisterApplicationMethodType_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RequestCertificateMethodType_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RequestCertificateMethodType_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RequestCertificateMethodType_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RequestCertificateMethodType_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RequestCertificateMethodType_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RequestCertificateMethodType_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RenewCertificateMethodType_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RenewCertificateMethodType_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RenewCertificateMethodType_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RenewCertificateMethodType_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RenewCertificateMethodType_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RenewCertificateMethodType_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RevokeCertificateMethodType_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RevokeCertificateMethodType_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RevokeCertificateMethodType_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CheckRequestStatusMethodType_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId CheckRequestStatusMethodType_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.CheckRequestStatusMethodType_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CheckRequestStatusMethodType_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId CheckRequestStatusMethodType_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.CheckRequestStatusMethodType_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the GetTrustListMethodType_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId GetTrustListMethodType_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.GetTrustListMethodType_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the GetTrustListMethodType_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId GetTrustListMethodType_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.GetTrustListMethodType_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the QueryServersMethodType_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId QueryServersMethodType_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.QueryServersMethodType_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the QueryServersMethodType_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId QueryServersMethodType_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.QueryServersMethodType_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CertificateRequestType_ApplicationUri Variable.
        /// </summary>
        public static readonly ExpandedNodeId CertificateRequestType_ApplicationUri = new ExpandedNodeId(Opc.Ua.Gds.Variables.CertificateRequestType_ApplicationUri, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CertificateRequestType_ProductUri Variable.
        /// </summary>
        public static readonly ExpandedNodeId CertificateRequestType_ProductUri = new ExpandedNodeId(Opc.Ua.Gds.Variables.CertificateRequestType_ProductUri, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CertificateRequestType_ApplicationType Variable.
        /// </summary>
        public static readonly ExpandedNodeId CertificateRequestType_ApplicationType = new ExpandedNodeId(Opc.Ua.Gds.Variables.CertificateRequestType_ApplicationType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CertificateRequestType_MachineName Variable.
        /// </summary>
        public static readonly ExpandedNodeId CertificateRequestType_MachineName = new ExpandedNodeId(Opc.Ua.Gds.Variables.CertificateRequestType_MachineName, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CertificateRequestType_SubjectName Variable.
        /// </summary>
        public static readonly ExpandedNodeId CertificateRequestType_SubjectName = new ExpandedNodeId(Opc.Ua.Gds.Variables.CertificateRequestType_SubjectName, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CertificateRequestType_DomainNames Variable.
        /// </summary>
        public static readonly ExpandedNodeId CertificateRequestType_DomainNames = new ExpandedNodeId(Opc.Ua.Gds.Variables.CertificateRequestType_DomainNames, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the CertificateRequestType_IsHttpsCertificate Variable.
        /// </summary>
        public static readonly ExpandedNodeId CertificateRequestType_IsHttpsCertificate = new ExpandedNodeId(Opc.Ua.Gds.Variables.CertificateRequestType_IsHttpsCertificate, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RegisterApplication_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RegisterApplication_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_RegisterApplication_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RegisterApplication_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RegisterApplication_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_RegisterApplication_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RequestCertificate_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RequestCertificate_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_RequestCertificate_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RequestCertificate_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RequestCertificate_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_RequestCertificate_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_CheckRequestStatus_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_CheckRequestStatus_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_CheckRequestStatus_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_CheckRequestStatus_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_CheckRequestStatus_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_CheckRequestStatus_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_GetTrustList_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_GetTrustList_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_GetTrustList_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_GetTrustList_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_GetTrustList_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_GetTrustList_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RenewCertificate_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RenewCertificate_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_RenewCertificate_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RenewCertificate_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RenewCertificate_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_RenewCertificate_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_RevokeCertificate_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_RevokeCertificate_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_RevokeCertificate_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_QueryServers_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_QueryServers_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_QueryServers_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the RootDirectoryEntryType_QueryServers_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId RootDirectoryEntryType_QueryServers_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.RootDirectoryEntryType_QueryServers_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the AddressableElementType_DnsName Variable.
        /// </summary>
        public static readonly ExpandedNodeId AddressableElementType_DnsName = new ExpandedNodeId(Opc.Ua.Gds.Variables.AddressableElementType_DnsName, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the ApplicationElementType_ApplicationUri Variable.
        /// </summary>
        public static readonly ExpandedNodeId ApplicationElementType_ApplicationUri = new ExpandedNodeId(Opc.Ua.Gds.Variables.ApplicationElementType_ApplicationUri, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the ApplicationElementType_ProductUri Variable.
        /// </summary>
        public static readonly ExpandedNodeId ApplicationElementType_ProductUri = new ExpandedNodeId(Opc.Ua.Gds.Variables.ApplicationElementType_ProductUri, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the ApplicationElementType_ApplicationType Variable.
        /// </summary>
        public static readonly ExpandedNodeId ApplicationElementType_ApplicationType = new ExpandedNodeId(Opc.Ua.Gds.Variables.ApplicationElementType_ApplicationType, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the ApplicationElementType_MachineName Variable.
        /// </summary>
        public static readonly ExpandedNodeId ApplicationElementType_MachineName = new ExpandedNodeId(Opc.Ua.Gds.Variables.ApplicationElementType_MachineName, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the ApplicationElementType_DiscoveryUrls Variable.
        /// </summary>
        public static readonly ExpandedNodeId ApplicationElementType_DiscoveryUrls = new ExpandedNodeId(Opc.Ua.Gds.Variables.ApplicationElementType_DiscoveryUrls, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RegisterApplication_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RegisterApplication_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_RegisterApplication_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RegisterApplication_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RegisterApplication_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_RegisterApplication_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RequestCertificate_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RequestCertificate_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_RequestCertificate_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RequestCertificate_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RequestCertificate_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_RequestCertificate_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_CheckRequestStatus_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_CheckRequestStatus_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_CheckRequestStatus_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_CheckRequestStatus_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_CheckRequestStatus_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_CheckRequestStatus_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_GetTrustList_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_GetTrustList_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_GetTrustList_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_GetTrustList_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_GetTrustList_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_GetTrustList_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RenewCertificate_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RenewCertificate_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_RenewCertificate_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RenewCertificate_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RenewCertificate_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_RenewCertificate_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_RevokeCertificate_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_RevokeCertificate_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_RevokeCertificate_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_QueryServers_InputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_QueryServers_InputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_QueryServers_InputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);

        /// <summary>
        /// The identifier for the Directory_QueryServers_OutputArguments Variable.
        /// </summary>
        public static readonly ExpandedNodeId Directory_QueryServers_OutputArguments = new ExpandedNodeId(Opc.Ua.Gds.Variables.Directory_QueryServers_OutputArguments, Opc.Ua.Gds.Namespaces.OpcUaGds);
    }
    #endregion

    #region BrowseName Declarations
    /// <summary>
    /// Declares all of the BrowseNames used in the Model Design.
    /// </summary>
    public static partial class BrowseNames
    {
        /// <summary>
        /// The BrowseName for the AddressableElementType component.
        /// </summary>
        public const string AddressableElementType = "AddressableElementType";

        /// <summary>
        /// The BrowseName for the ApplicationElementType component.
        /// </summary>
        public const string ApplicationElementType = "ApplicationElementType";

        /// <summary>
        /// The BrowseName for the Applications component.
        /// </summary>
        public const string Applications = "Applications";

        /// <summary>
        /// The BrowseName for the ApplicationType component.
        /// </summary>
        public const string ApplicationType = "ApplicationType";

        /// <summary>
        /// The BrowseName for the ApplicationUri component.
        /// </summary>
        public const string ApplicationUri = "ApplicationUri";

        /// <summary>
        /// The BrowseName for the CertificateRequests component.
        /// </summary>
        public const string CertificateRequests = "CertificateRequests";

        /// <summary>
        /// The BrowseName for the CertificateRequestType component.
        /// </summary>
        public const string CertificateRequestType = "CertificateRequestType";

        /// <summary>
        /// The BrowseName for the CheckRequestStatus component.
        /// </summary>
        public const string CheckRequestStatus = "CheckRequestStatus";

        /// <summary>
        /// The BrowseName for the CheckRequestStatusMethodType component.
        /// </summary>
        public const string CheckRequestStatusMethodType = "CheckRequestStatusMethodType";

        /// <summary>
        /// The BrowseName for the Directory component.
        /// </summary>
        public const string Directory = "Directory";

        /// <summary>
        /// The BrowseName for the DiscoveryUrls component.
        /// </summary>
        public const string DiscoveryUrls = "DiscoveryUrls";

        /// <summary>
        /// The BrowseName for the DnsName component.
        /// </summary>
        public const string DnsName = "DnsName";

        /// <summary>
        /// The BrowseName for the DomainNames component.
        /// </summary>
        public const string DomainNames = "DomainNames";

        /// <summary>
        /// The BrowseName for the GetTrustList component.
        /// </summary>
        public const string GetTrustList = "GetTrustList";

        /// <summary>
        /// The BrowseName for the GetTrustListMethodType component.
        /// </summary>
        public const string GetTrustListMethodType = "GetTrustListMethodType";

        /// <summary>
        /// The BrowseName for the IsHttpsCertificate component.
        /// </summary>
        public const string IsHttpsCertificate = "IsHttpsCertificate";

        /// <summary>
        /// The BrowseName for the MachineName component.
        /// </summary>
        public const string MachineName = "MachineName";

        /// <summary>
        /// The BrowseName for the ProductUri component.
        /// </summary>
        public const string ProductUri = "ProductUri";

        /// <summary>
        /// The BrowseName for the QueryServers component.
        /// </summary>
        public const string QueryServers = "QueryServers";

        /// <summary>
        /// The BrowseName for the QueryServersMethodType component.
        /// </summary>
        public const string QueryServersMethodType = "QueryServersMethodType";

        /// <summary>
        /// The BrowseName for the RegisterApplication component.
        /// </summary>
        public const string RegisterApplication = "RegisterApplication";

        /// <summary>
        /// The BrowseName for the RegisterApplicationMethodType component.
        /// </summary>
        public const string RegisterApplicationMethodType = "RegisterApplicationMethodType";

        /// <summary>
        /// The BrowseName for the RenewCertificate component.
        /// </summary>
        public const string RenewCertificate = "RenewCertificate";

        /// <summary>
        /// The BrowseName for the RenewCertificateMethodType component.
        /// </summary>
        public const string RenewCertificateMethodType = "RenewCertificateMethodType";

        /// <summary>
        /// The BrowseName for the RequestCertificate component.
        /// </summary>
        public const string RequestCertificate = "RequestCertificate";

        /// <summary>
        /// The BrowseName for the RequestCertificateMethodType component.
        /// </summary>
        public const string RequestCertificateMethodType = "RequestCertificateMethodType";

        /// <summary>
        /// The BrowseName for the RevokeCertificate component.
        /// </summary>
        public const string RevokeCertificate = "RevokeCertificate";

        /// <summary>
        /// The BrowseName for the RevokeCertificateMethodType component.
        /// </summary>
        public const string RevokeCertificateMethodType = "RevokeCertificateMethodType";

        /// <summary>
        /// The BrowseName for the RootDirectoryEntryType component.
        /// </summary>
        public const string RootDirectoryEntryType = "RootDirectoryEntryType";

        /// <summary>
        /// The BrowseName for the SubjectName component.
        /// </summary>
        public const string SubjectName = "SubjectName";

        /// <summary>
        /// The BrowseName for the SystemElementType component.
        /// </summary>
        public const string SystemElementType = "SystemElementType";
    }
    #endregion

    #region Namespace Declarations
    /// <summary>
    /// Defines constants for all namespaces referenced by the model design.
    /// </summary>
    public static partial class Namespaces
    {
        /// <summary>
        /// The URI for the OpcUa namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUa = "http://opcfoundation.org/UA/";

        /// <summary>
        /// The URI for the OpcUaXsd namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUaXsd = "http://opcfoundation.org/UA/2008/02/Types.xsd";

        /// <summary>
        /// The URI for the OpcUaGds namespace (.NET code namespace is 'Opc.Ua.Gds').
        /// </summary>
        public const string OpcUaGds = "http://opcfoundation.org/UA/GDS/";

        /// <summary>
        /// Returns a namespace table with all of the URIs defined.
        /// </summary>
        /// <remarks>
        /// This table is was used to create any relative paths in the model design.
        /// </remarks>
        public static NamespaceTable GetNamespaceTable()
        {
            FieldInfo[] fields = typeof(Namespaces).GetFields(BindingFlags.Public | BindingFlags.Static);

            NamespaceTable namespaceTable = new NamespaceTable();

            foreach (FieldInfo field in fields)
            {
                string namespaceUri = (string)field.GetValue(typeof(Namespaces));

                if (namespaceTable.GetIndex(namespaceUri) == -1)
                {
                    namespaceTable.Append(namespaceUri);
                }
            }

            return namespaceTable;
        }
    }
    #endregion
}
