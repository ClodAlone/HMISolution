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
#include "COpcEnumStringWrapper.h"
#include "COpcUaProxyUtils.h"

using namespace System;

/// <summary>
/// Dumps the current state.
/// </summary>
static void TraceState(String^ context, ... array<Object^>^ args)
{
    #ifdef TRACESTATE
    COpcUaProxyUtils::TraceState("COpcEnumStringWrapper", context, args);
	#endif
}

//============================================================================
// COpcEnumStringWrapper

COpcEnumStringWrapper::COpcEnumStringWrapper()
{
	TraceState("COpcEnumStringWrapper");
	
    m_ipUnknown = NULL;
}

// Constructor
COpcEnumStringWrapper::COpcEnumStringWrapper(IUnknown* ipUnknown)
{
	TraceState("COpcEnumStringWrapper");
	
    m_ipUnknown = ipUnknown;
	m_ipUnknown->AddRef();
}

// Destructor 
COpcEnumStringWrapper::~COpcEnumStringWrapper()
{
	TraceState("~COpcEnumStringWrapper");

	if (m_ipUnknown != NULL)
	{
		m_ipUnknown->Release();
		m_ipUnknown = NULL;
	}
}

//============================================================================
// IEnumString
   
// Next
HRESULT COpcEnumStringWrapper::Next(
	ULONG     celt,
	LPOLESTR* rgelt,
	ULONG*    pceltFetched
)
{
	TraceState("Next");

	COpcLock cLock(*this);

	// check inner server.
	if (m_ipUnknown == NULL)
	{
		return E_FAIL;
	}

	// fetch required interface.
	IEnumString* ipInterface = NULL;

	if (FAILED(m_ipUnknown->QueryInterface(IID_IEnumString, (void**)&ipInterface)))
	{
		return E_NOTIMPL;
	}

	// invoke method.
	HRESULT hResult = ipInterface->Next(
		celt,
		rgelt,
		pceltFetched
	);

	// release interface.
	ipInterface->Release();

	if (hResult == S_OK)
	{
		if (celt > 0 && *pceltFetched == 0)
		{
			return S_FALSE;
		}
	}

	return hResult;
}

// Skip
HRESULT COpcEnumStringWrapper::Skip(ULONG celt)
{
	TraceState("Skip");

	COpcLock cLock(*this);

	// check inner server.
	if (m_ipUnknown == NULL)
	{
		return E_FAIL;
	}

	// fetch required interface.
	IEnumString* ipInterface = NULL;

	if (FAILED(m_ipUnknown->QueryInterface(IID_IEnumString, (void**)&ipInterface)))
	{
		return E_NOTIMPL;
	}

	// invoke method.
	HRESULT hResult = ipInterface->Skip(
		celt
	);

	// release interface.
	ipInterface->Release();

	return hResult;
}

// Reset
HRESULT COpcEnumStringWrapper::Reset()
{
	TraceState("Reset");

	COpcLock cLock(*this);

	// check inner server.
	if (m_ipUnknown == NULL)
	{
		return E_FAIL;
	}

	// fetch required interface.
	IEnumString* ipInterface = NULL;

	if (FAILED(m_ipUnknown->QueryInterface(IID_IEnumString, (void**)&ipInterface)))
	{
		return E_NOTIMPL;
	}

	// invoke method.
	HRESULT hResult = ipInterface->Reset();

	// release interface.
	ipInterface->Release();

	return hResult;
}

// Clone
HRESULT COpcEnumStringWrapper::Clone(IEnumString** ppEnum)
{
	TraceState("Clone");

	COpcLock cLock(*this);

    // check for invalid arguments.
    if (ppEnum == NULL)
    {
        return E_INVALIDARG;
    }

	// check inner server.
	if (m_ipUnknown == NULL)
	{
		return E_FAIL;
	}

	// fetch required interface.
	IEnumString* ipInterface = NULL;

	if (FAILED(m_ipUnknown->QueryInterface(IID_IEnumString, (void**)&ipInterface)))
	{
		return E_NOTIMPL;
	}

	// invoke method.
	IEnumString* ipEnum = NULL;

	HRESULT hResult = ipInterface->Clone(&ipEnum);

	// release interface.
	ipInterface->Release();

	// create wrapper.
	COpcEnumStringWrapper* pEnum = new COpcEnumStringWrapper(ipEnum);

	// release local reference.
	ipEnum->Release();

	// query for interface.
    hResult = pEnum->QueryInterface(IID_IEnumString, (void**)ppEnum);

    // release local reference.
    pEnum->Release();

    return hResult;
}
