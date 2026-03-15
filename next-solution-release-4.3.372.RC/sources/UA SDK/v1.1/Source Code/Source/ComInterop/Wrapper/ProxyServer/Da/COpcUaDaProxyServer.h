/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community License ("RCL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community License ("RCL") Version 1.00, or subsequent versions as 
 * allowed by the RCL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCL.
 * 
 * All software distributed under the RCL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCL for specific 
 * language governing rights and limitations under the RCL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCL/1.00/
 * ======================================================================*/

#ifndef _COpcUaDaProxyServer_H_
#define _COpcUaDaProxyServer_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcUaComProxyServer.h"

class COpcUaDaProxyGroup;

using namespace Opc::Ua;
using namespace Opc::Ua::Com::Server;

//============================================================================
// CLASS:   COpcUaDaProxyServer
// PURPOSE: A class that implements the IOPCServer interface.

class COpcUaDaProxyServer :
    public COpcComObject,
    public COpcCPContainer,
    public IOPCCommon,
    public IOPCBrowseServerAddressSpace,
    public IOPCItemProperties,
    public IOPCServer,

#ifndef OPCUA_NO_DA3_SUPPORT
    public IOPCBrowse,
    public IOPCItemIO,
#endif
    
	public COpcSynchObject
{
    OPC_CLASS_NEW_DELETE()

    OPC_BEGIN_INTERFACE_TABLE(COpcUaDaProxyServer)
        OPC_INTERFACE_ENTRY(IOPCCommon)
        OPC_INTERFACE_ENTRY(IConnectionPointContainer)
        OPC_INTERFACE_ENTRY(IOPCServer)
        OPC_INTERFACE_ENTRY(IOPCBrowseServerAddressSpace)
        OPC_INTERFACE_ENTRY(IOPCItemProperties)

#ifndef OPCUA_NO_DA3_SUPPORT
        OPC_INTERFACE_ENTRY(IOPCBrowse)
        OPC_INTERFACE_ENTRY(IOPCItemIO)
#endif

    OPC_END_INTERFACE_TABLE()

public:

    //=========================================================================
    // Operators

    // Constructor
    COpcUaDaProxyServer();

    // Destructor 
    ~COpcUaDaProxyServer();

    //=========================================================================
    // Public Methods

    // FinalConstruct
    virtual HRESULT FinalConstruct();

    // FinalRelease
    virtual bool FinalRelease();
	
	//==========================================================================
    // IOPCCommon

    // SetLocaleID
    STDMETHODIMP SetLocaleID(LCID dwLcid);

    // GetLocaleID
    STDMETHODIMP GetLocaleID(LCID *pdwLcid);

    // QueryAvailableLocaleIDs
    STDMETHODIMP QueryAvailableLocaleIDs(DWORD* pdwCount, LCID** pdwLcid);

    // GetErrorString
    STDMETHODIMP GetErrorString(HRESULT dwError, LPWSTR* ppString);

    // SetClientName
    STDMETHODIMP SetClientName(LPCWSTR szName);

    //=========================================================================
    // IOPCServer

    // AddGroup
    STDMETHODIMP AddGroup(
        LPCWSTR    szName,
        BOOL       bActive,
        DWORD      dwRequestedUpdateRate,
        OPCHANDLE  hClientGroup,
        LONG*      pTimeBias,
        FLOAT*     pPercentDeadband,
        DWORD      dwLCID,
        OPCHANDLE* phServerGroup,
        DWORD*     pRevisedUpdateRate,
        REFIID     riid,
        LPUNKNOWN* ppUnk
    );

    // GetErrorString
    STDMETHODIMP GetErrorString( 
        HRESULT dwError,
        LCID    dwLocale,
        LPWSTR* ppString
    );

    // GetGroupByName
    STDMETHODIMP GetGroupByName(
        LPCWSTR    szName,
        REFIID     riid,
        LPUNKNOWN* ppUnk
    );

    // GetStatus
    STDMETHODIMP GetStatus( 
        OPCSERVERSTATUS** ppServerStatus
    );

    // RemoveGroup
    STDMETHODIMP RemoveGroup(
        OPCHANDLE hServerGroup,
        BOOL      bForce
    );

    // CreateGroupEnumerator
    STDMETHODIMP CreateGroupEnumerator(
        OPCENUMSCOPE dwScope, 
        REFIID       riid, 
        LPUNKNOWN*   ppUnk
    );

    //=========================================================================
    // IOPCBrowseServerAddressSpace
    
    // QueryOrganization
    STDMETHODIMP QueryOrganization(OPCNAMESPACETYPE* pNameSpaceType);
    
    // ChangeBrowsePosition
    STDMETHODIMP ChangeBrowsePosition(
        OPCBROWSEDIRECTION dwBrowseDirection,  
        LPCWSTR            szString
    );

    // BrowseOPCItemIDs
    STDMETHODIMP BrowseOPCItemIDs(
        OPCBROWSETYPE   dwBrowseFilterType,
        LPCWSTR         szFilterCriteria,  
        VARTYPE         vtDataTypeFilter,     
        DWORD           dwAccessRightsFilter,
        LPENUMSTRING*   ppIEnumString
    );

    // GetItemID
    STDMETHODIMP GetItemID(
        LPWSTR  wszItemName,
        LPWSTR* pszItemID
    );

    // BrowseAccessPaths
    STDMETHODIMP BrowseAccessPaths(
        LPCWSTR       szItemID,  
        LPENUMSTRING* ppIEnumString
    );

    //=========================================================================
    // IOPCItemProperties

    // QueryAvailableProperties
    STDMETHODIMP QueryAvailableProperties( 
        LPWSTR     szItemID,
        DWORD    * pdwCount,
        DWORD   ** ppPropertyIDs,
        LPWSTR  ** ppDescriptions,
        VARTYPE ** ppvtDataTypes
    );

    // GetItemProperties
    STDMETHODIMP GetItemProperties ( 
        LPWSTR     szItemID,
        DWORD      dwCount,
        DWORD    * pdwPropertyIDs,
        VARIANT ** ppvData,
        HRESULT ** ppErrors
    );

    // LookupItemIDs
    STDMETHODIMP LookupItemIDs( 
        LPWSTR     szItemID,
        DWORD      dwCount,
        DWORD    * pdwPropertyIDs,
        LPWSTR  ** ppszNewItemIDs,
        HRESULT ** ppErrors
    );

    //=========================================================================
    // IOPCBrowse

    // GetProperties
    STDMETHODIMP GetProperties( 
        DWORD		        dwItemCount,
        LPWSTR*             pszItemIDs,
        BOOL		        bReturnPropertyValues,
        DWORD		        dwPropertyCount,
        DWORD*              pdwPropertyIDs,
        OPCITEMPROPERTIES** ppItemProperties 
    );

    // Browse
    STDMETHODIMP Browse(
	    LPWSTR	           szItemName,
	    LPWSTR*	           pszContinuationPoint,
	    DWORD              dwMaxElementsReturned,
		OPCBROWSEFILTER    dwFilter,
	    LPWSTR             szElementNameFilter,
	    LPWSTR             szVendorFilter,
	    BOOL               bReturnAllProperties,
	    BOOL               bReturnPropertyValues,
	    DWORD              dwPropertyCount,
	    DWORD*             pdwPropertyIDs,
	    BOOL*              pbMoreElements,
	    DWORD*	           pdwCount,
	    OPCBROWSEELEMENT** ppBrowseElements
    );
    
    //=========================================================================
    // IOPCItemIO

    // Read
    STDMETHODIMP Read(
        DWORD       dwCount, 
        LPCWSTR   * pszItemIDs,
        DWORD     * pdwMaxAge,
        VARIANT  ** ppvValues,
        WORD     ** ppwQualities,
        FILETIME ** ppftTimeStamps,
        HRESULT  ** ppErrors
    );

    // WriteVQT
    STDMETHODIMP WriteVQT(
        DWORD         dwCount, 
        LPCWSTR    *  pszItemIDs,
        OPCITEMVQT *  pItemVQT,
        HRESULT    ** ppErrors
    );

private:
        
	// Returns the wrapped server instance.
	ComDaProxy^ GetInnerServer();

    //==========================================================================
    // Private Members

	void* m_pInnerServer;
	LPWSTR m_pClientName;
	OPCSERVERSTATUS m_tServerStatus;
};

#endif // _COpcUaDaProxyServer_H_
