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

#ifndef _COpcUaHdaProxyBrowser_H_
#define _COpcUaHdaProxyBrowser_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "COpcUaHdaProxyBrowser.h"

using namespace Opc::Ua;
using namespace Opc::Ua::Com;
using namespace Opc::Ua::Com::Server;

class COpcUaHdaProxyBrowser :
    public COpcComObject,
    public IOPCHDA_Browser,
    public COpcSynchObject
{
    OPC_CLASS_NEW_DELETE()

    OPC_BEGIN_INTERFACE_TABLE(COpcUaHdaProxyBrowser)
        OPC_INTERFACE_ENTRY(IOPCHDA_Browser)
    OPC_END_INTERFACE_TABLE()

public:

	//=========================================================================
    // Operators

    // Constructor
    COpcUaHdaProxyBrowser();
    COpcUaHdaProxyBrowser(ComHdaBrowser^ browser);

    // Destructor 
    ~COpcUaHdaProxyBrowser();

    //=========================================================================
    // IOPCHDA_Browser

	STDMETHODIMP GetEnum(
		OPCHDA_BROWSETYPE dwBrowseType,
		LPENUMSTRING*     ppIEnumString
	);

	STDMETHODIMP ChangeBrowsePosition(
		OPCHDA_BROWSEDIRECTION dwBrowseDirection,
		LPCWSTR                szString
	);

	STDMETHODIMP GetItemID(
		LPCWSTR szNode,
		LPWSTR* pszItemID
	);


	STDMETHODIMP GetBranchPosition(
		LPWSTR* pszBranchPos
	);

private:

	// GetInnerBrowser
	ComHdaBrowser^ GetInnerBrowser();

	void* m_pInnerBrowser;

};

#endif // _COpcUaHdaProxyBrowser_H_
