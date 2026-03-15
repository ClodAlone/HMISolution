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
#include "COpcEnumString.h"

//==============================================================================
// COpcEnumString

COpcEnumString::COpcEnumString()
{
    m_uIndex   = 0;
    m_uCount   = 0;
    m_pStrings = NULL;
}

// Constructor
COpcEnumString::COpcEnumString(UINT uCount, LPWSTR*& pStrings)
{
    m_uIndex   = 0;
    m_uCount   = uCount;
    m_pStrings = pStrings;

    // take ownership of memory.
    pStrings = NULL;
}

// Destructor 
COpcEnumString::~COpcEnumString()
{
    for (UINT ii = 0; ii < m_uCount; ii++)
    {
        OpcFree(m_pStrings[ii]);
    }

    OpcFree(m_pStrings);
}

//==============================================================================
// IEnumString
   
// Next
HRESULT COpcEnumString::Next(
    ULONG     celt,
    LPOLESTR* rgelt,
    ULONG*    pceltFetched)
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
        rgelt[*pceltFetched] = (LPWSTR)CoTaskMemAlloc((wcslen(m_pStrings[ii])+1)*sizeof(WCHAR));

        if (m_pStrings[ii] == NULL)
        {
            rgelt[*pceltFetched] = NULL;
        }
        else
        {
            wcscpy(rgelt[*pceltFetched], m_pStrings[ii]);
        }

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
HRESULT COpcEnumString::Skip(ULONG celt)
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
HRESULT COpcEnumString::Reset()
{
    m_uIndex = 0;
    return S_OK;
}

// Clone
HRESULT COpcEnumString::Clone(IEnumString** ppEnum)
{
    // check for invalid arguments.
    if (ppEnum == NULL)
    {
        return E_INVALIDARG;
    }

    // allocate enumerator.
    COpcEnumString* pEnum = new COpcEnumString();
 
    // copy strings.
    pEnum->m_pStrings = OpcArrayAlloc(LPWSTR, m_uCount);

    for (UINT ii = 0; ii < m_uCount; ii++)
    {
        pEnum->m_pStrings[ii] = OpcArrayAlloc(WCHAR, wcslen(m_pStrings[ii])+1);
        wcscpy(pEnum->m_pStrings[ii], m_pStrings[ii]);
    }

    // set index.
    pEnum->m_uIndex = m_uIndex;
    pEnum->m_uCount = m_uCount;

    HRESULT hResult = pEnum->QueryInterface(IID_IEnumString, (void**)ppEnum);

    // release local reference.
    pEnum->Release();

    return hResult;
}
