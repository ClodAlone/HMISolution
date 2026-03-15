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
#include "COpcEnumUnknown.h"

//==============================================================================
// COpcEnumUnknown

COpcEnumUnknown::COpcEnumUnknown()
{
    m_uIndex    = 0;
    m_uCount    = 0;
    m_pUnknowns = NULL;
}

// Constructor
COpcEnumUnknown::COpcEnumUnknown(UINT uCount, IUnknown**& pUnknowns)
{
    m_uIndex   = 0;
    m_uCount   = uCount;
    m_pUnknowns = pUnknowns;

    // take ownership of memory.
    pUnknowns = NULL;
}

// Destructor 
COpcEnumUnknown::~COpcEnumUnknown()
{
    for (UINT ii = 0; ii < m_uCount; ii++)
    {
        if (m_pUnknowns[ii] != NULL) m_pUnknowns[ii]->Release();
    }

    OpcFree(m_pUnknowns);
}

//==============================================================================
// IEnumUnknown
   
// Next
HRESULT COpcEnumUnknown::Next(
    ULONG      celt,          
    IUnknown** rgelt,   
    ULONG*     pceltFetched
)
{
    // check for invalid arguments.
    if (rgelt == NULL || pceltFetched == NULL)
    {
        return E_INVALIDARG;
    }

    *pceltFetched = NULL;

    // all strings already returned.
    if (m_uIndex >= m_uCount)
    {
        return S_FALSE;
    }

    // copy strings.
    for (UINT ii = m_uIndex; ii < m_uCount && *pceltFetched < celt; ii++)
    {
        rgelt[*pceltFetched] = m_pUnknowns[ii];
        if (m_pUnknowns[ii] != NULL) m_pUnknowns[ii]->AddRef();
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
HRESULT COpcEnumUnknown::Skip(ULONG celt)
{
    if (m_uIndex + celt > m_uCount)
    {
        m_uIndex = m_uCount;
        return S_FALSE;
    }

    m_uIndex += celt;
    return S_OK;
}

// Reset
HRESULT COpcEnumUnknown::Reset()
{
    m_uIndex = 0;
    return S_OK;
}

// Clone
HRESULT COpcEnumUnknown::Clone(IEnumUnknown** ppEnum)
{
    // check for invalid arguments.
    if (ppEnum == NULL)
    {
        return E_INVALIDARG;
    }

    // allocate enumerator.
    COpcEnumUnknown* pEnum = new COpcEnumUnknown();
 
    // copy strings.
    pEnum->m_pUnknowns = OpcArrayAlloc(IUnknown*, m_uCount);

    for (UINT ii = 0; ii < m_uCount; ii++)
    {
        pEnum->m_pUnknowns[ii] = m_pUnknowns[ii];
        if (m_pUnknowns[ii] != NULL) m_pUnknowns[ii]->AddRef();
    }

    // set index.
    pEnum->m_uIndex = m_uIndex;
    pEnum->m_uCount = m_uCount;

    HRESULT hResult = pEnum->QueryInterface(IID_IEnumUnknown, (void**)ppEnum);

    // release local reference.
    pEnum->Release();

    return hResult;
}
