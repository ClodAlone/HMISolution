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

#ifndef _COpcUaAe2ProxyBrowser_H_
#define _COpcUaAe2ProxyBrowser_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

using namespace Opc::Ua;
using namespace Opc::Ua::Com;
using namespace Opc::Ua::Com::Server;

class COpcUaAe2ProxyBrowser :
    public COpcComObject,
    public IOPCEventAreaBrowser,
    public COpcSynchObject
{
    OPC_CLASS_NEW_DELETE()

    OPC_BEGIN_INTERFACE_TABLE(COpcUaAe2ProxyBrowser)
        OPC_INTERFACE_ENTRY(IOPCEventAreaBrowser)
    OPC_END_INTERFACE_TABLE()

public:

	//=========================================================================
    // Operators

    // Constructor
    COpcUaAe2ProxyBrowser();
    COpcUaAe2ProxyBrowser(ComAe2Browser^ browser);

    // Destructor 
    ~COpcUaAe2ProxyBrowser();

	//=========================================================================
	// IOPCEventAreaBrowser

	STDMETHODIMP ChangeBrowsePosition( 
		/* [in] */ OPCAEBROWSEDIRECTION dwBrowseDirection,
		/* [string][in] */ LPCWSTR szString);

	STDMETHODIMP BrowseOPCAreas( 
		/* [in] */ OPCAEBROWSETYPE dwBrowseFilterType,
		/* [string][in] */ LPCWSTR szFilterCriteria,
		/* [out] */ LPENUMSTRING __RPC_FAR *ppIEnumString);

	STDMETHODIMP GetQualifiedAreaName( 
		/* [in] */ LPCWSTR szAreaName,
		/* [string][out] */ LPWSTR __RPC_FAR *pszQualifiedAreaName);

	STDMETHODIMP GetQualifiedSourceName( 
		/* [in] */ LPCWSTR szSourceName,
		/* [string][out] */ LPWSTR __RPC_FAR *pszQualifiedSourceName);

private:

	// GetInnerBrowser
	ComAe2Browser^ GetInnerBrowser();
	void* m_pInnerBrowser;
};

#endif // _COpcUaAe2ProxyBrowser_H_
