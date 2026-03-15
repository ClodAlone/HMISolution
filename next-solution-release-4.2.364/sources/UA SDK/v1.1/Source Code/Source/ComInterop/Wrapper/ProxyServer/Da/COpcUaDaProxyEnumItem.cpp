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

#include "StdAfx.h"
#include "COpcUaDaProxyEnumItem.h"
#include "COpcUaProxyUtils.h"

using namespace System;

/// <summary>
/// Dumps the current state.
/// </summary>
static void TraceState(String^ context, ... array<Object^>^ args)
{
    #ifdef TRACESTATE
    COpcUaProxyUtils::TraceState("COpcUaDaProxyEnumItem", context, args);
	#endif
}

//============================================================================
// COpcUaDaProxyEnumItem

COpcUaDaProxyEnumItem::COpcUaDaProxyEnumItem()
{
	TraceState("COpcUaDaProxyEnumItem");
	
    m_uIndex = 0;
    m_uCount = 0;
    m_pItems = NULL;	
}

// Constructor
COpcUaDaProxyEnumItem::COpcUaDaProxyEnumItem(UINT uCount, OPCITEMATTRIBUTES* pItems)
{
	TraceState("COpcUaDaProxyEnumItem");

    m_uIndex = 0;
    m_uCount = uCount;
    m_pItems = pItems;
}

// Destructor 
COpcUaDaProxyEnumItem::~COpcUaDaProxyEnumItem()
{
	TraceState("~COpcUaDaProxyEnumItem");
    COpcLock cLock(*this);

    for (UINT ii = 0; ii < m_uCount; ii++)
    {
        Clear(m_pItems[ii]);
    }

    OpcFree(m_pItems);
}

// Init
void COpcUaDaProxyEnumItem::Init(OPCITEMATTRIBUTES& cAttributes)
{
	memset(&cAttributes, 0, sizeof(OPCITEMATTRIBUTES));
}

// Clear
void COpcUaDaProxyEnumItem::Clear(OPCITEMATTRIBUTES& cAttributes)
{
	OpcFree(cAttributes.szAccessPath);
	OpcFree(cAttributes.szItemID);
	OpcFree(cAttributes.pBlob);
	OpcVariantClear(&cAttributes.vEUInfo);
}

// Copy
void COpcUaDaProxyEnumItem::Copy(OPCITEMATTRIBUTES& cDst, OPCITEMATTRIBUTES& cSrc)
{
	cDst.szAccessPath   = OpcStrDup(cSrc.szAccessPath);
	cDst.szItemID       = OpcStrDup(cSrc.szItemID);
	cDst.bActive        = cSrc.bActive;
	cDst.hClient        = cSrc.hClient;
	cDst.hServer        = cSrc.hServer;
	cDst.dwAccessRights = cSrc.dwAccessRights;
	cDst.dwBlobSize     = cSrc.dwBlobSize;
  
	if (cSrc.dwBlobSize > 0)
	{
		cDst.pBlob = (BYTE*)OpcAlloc(cSrc.dwBlobSize);
		memcpy(cDst.pBlob, cSrc.pBlob, cSrc.dwBlobSize);
	}

	
	cDst.vtRequestedDataType = cSrc.vtRequestedDataType;
	cDst.vtCanonicalDataType = cSrc.vtCanonicalDataType;
	cDst.dwEUType            = cSrc.dwEUType;

	OpcVariantCopy(&cDst.vEUInfo, &cSrc.vEUInfo);
}

//============================================================================
// IEnumOPCItemAttributes
   
// Next
HRESULT COpcUaDaProxyEnumItem::Next(
	ULONG               celt,
	OPCITEMATTRIBUTES** ppItemArray,
	ULONG*              pceltFetched
)
{
	TraceState("Next");	
    COpcLock cLock(*this);

    // check for invalid arguments.
    if (ppItemArray == NULL || pceltFetched == NULL)
    {
        return E_INVALIDARG;
    }

    *pceltFetched = 0;

    // all items already returned.
    if (m_uIndex >= m_uCount)
    {
        return S_FALSE;
    }

    // copy items.
    *ppItemArray = (OPCITEMATTRIBUTES*)OpcArrayAlloc(OPCITEMATTRIBUTES, celt);
    memset(*ppItemArray, 0, celt*sizeof(OPCITEMATTRIBUTES));

	UINT ii = 0;

    for (ii = m_uIndex; ii < m_uCount && *pceltFetched < celt; ii++)
    {
        Copy((*ppItemArray)[*pceltFetched], m_pItems[ii]);
        (*pceltFetched)++;
    }

    // no enough strings left.
    if (*pceltFetched < celt)
    {
        m_uIndex = m_uCount;
        return S_FALSE;
    }

    m_uIndex = ii;
    return S_OK;
}

// Skip
HRESULT COpcUaDaProxyEnumItem::Skip(ULONG celt)
{
	TraceState("Skip");	
    COpcLock cLock(*this);

    if (m_uIndex + celt > m_uCount)
    {
        m_uIndex = m_uCount;
        return S_FALSE;
    }

    m_uIndex += celt;
    return S_OK;
}

// Reset
HRESULT COpcUaDaProxyEnumItem::Reset()
{
	TraceState("Reset");	
    COpcLock cLock(*this);

    m_uIndex = 0;
    return S_OK;
}

// Clone
HRESULT COpcUaDaProxyEnumItem::Clone(IEnumOPCItemAttributes** ppEnum)
{
	TraceState("Clone");	
    COpcLock cLock(*this);

    // check for invalid arguments.
    if (ppEnum == NULL)
    {
        return E_INVALIDARG;
    }

    // allocate enumerator.
    COpcUaDaProxyEnumItem* pEnum = new COpcUaDaProxyEnumItem();

	if (m_uCount > 0)
	{
		// copy items.
		OPCITEMATTRIBUTES* pItems = OpcArrayAlloc(OPCITEMATTRIBUTES, m_uCount);

		for (UINT ii = 0; ii < m_uCount; ii++)
		{
			Copy(pItems[ii], m_pItems[ii]);
		}

		// set new enumerator state.
		pEnum->m_pItems = pItems;
		pEnum->m_uCount = m_uCount;
		pEnum->m_uIndex = m_uIndex;
	}

	// query for interface.
    HRESULT hResult = pEnum->QueryInterface(IID_IEnumOPCItemAttributes, (void**)ppEnum);

    // release local reference.
    pEnum->Release();

    return hResult;
}
