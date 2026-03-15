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
#include "COpcEnumCPs.h"

// Constructor
COpcEnumCPs::COpcEnumCPs()
{
    Reset();
}

// Constructor
COpcEnumCPs::COpcEnumCPs(const COpcConnectionPointList& cCPs)
{
    // copy connection points.
    OPC_POS pos = cCPs.GetHeadPosition();

    while (pos != NULL)
    {
        COpcConnectionPoint* pCP = cCPs.GetNext(pos);

        m_cCPs.AddTail(pCP);
        pCP->AddRef();
    }      

    // set pointer to start of list.
    Reset();
}

// Destructor 
COpcEnumCPs::~COpcEnumCPs()
{
    // release connection points.
    OPC_POS pos = m_cCPs.GetHeadPosition();

    while (pos != NULL)
    {
        m_cCPs.GetNext(pos)->Release();
    }
}

//==============================================================================
// IEnumConnectionPoints

// Next
HRESULT COpcEnumCPs::Next(
    ULONG              cConnections,
    LPCONNECTIONPOINT* ppCP,
    ULONG*             pcFetched
)
{
    // invalid arguments - return error.
    if (pcFetched == NULL)
    {
        return E_INVALIDARG;
    }
        
    *pcFetched = 0;

    // trivial case - return nothing.
    if (cConnections == 0)
    {
        return S_OK;
    }
    
    // read connection points.
    for (ULONG ii = 0; ii < cConnections; ii++)
    {
        // end of list reached before count reached.
        if (m_pos == NULL)
        {
            *pcFetched = ii;
            return S_FALSE;
        }

        ppCP[ii] = m_cCPs.GetNext(m_pos);
        
        // client must release the reference.
        ppCP[ii]->AddRef();
    } 

    *pcFetched = ii;
    return S_OK;
}

// Skip
HRESULT COpcEnumCPs::Skip(ULONG cConnections)
{
    // skip connection points.
    OPC_POS pos = m_cCPs.GetHeadPosition();

    for (ULONG ii = 0; ii < cConnections; ii++)
    {
        // end of list reached before count reached.
        if (m_pos == NULL)
        {
            return S_FALSE;
        }

        m_cCPs.GetNext(m_pos);
    } 

    return S_OK;
}

// Reset
HRESULT COpcEnumCPs::Reset()
{
    m_pos = m_cCPs.GetHeadPosition();
    return S_OK;
}

// Clone
HRESULT COpcEnumCPs::Clone(IEnumConnectionPoints** ppEnum)
{
    // create a new enumeration object.
    COpcEnumCPs* ipEnum = new COpcEnumCPs();

    // copy connection points.
    OPC_POS pos = m_cCPs.GetHeadPosition();

    while (pos != NULL)
    {
        COpcConnectionPoint* pCP = m_cCPs[pos];

        ipEnum->m_cCPs.AddTail(pCP);
    
        // clone must release the reference.
        pCP->AddRef();

        // save the current location.
        if (pos == m_pos)
        {
            ipEnum->m_pos = ipEnum->m_cCPs.GetTailPosition();   
        }

        m_cCPs.GetNext(pos);
    }      

    // query interface.
    HRESULT hResult = ipEnum->QueryInterface(IID_IEnumConnectionPoints, (void**)ppEnum);

    // release local reference.
    ipEnum->Release();

    return hResult;
}
