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
#include "COpcCPContainer.h"
#include "COpcEnumCPs.h"

//==============================================================================
// COpcCPContainer

// Constructor
COpcCPContainer::COpcCPContainer()
{
}

// Destructor 
COpcCPContainer::~COpcCPContainer()
{
    // release the connection points.
    OPC_POS pos = m_cCPs.GetHeadPosition();

    while (pos != NULL)
    {
        m_cCPs.GetNext(pos)->Release();
    }
}

// RegisterInterface
void COpcCPContainer::RegisterInterface(const IID& tInterface)
{
    // constructor adds one reference.
    COpcConnectionPoint* pCP = new COpcConnectionPoint(tInterface, this);
    m_cCPs.AddTail(pCP);
}

// UnregisterInterface
void COpcCPContainer::UnregisterInterface(const IID& tInterface)
{
    OPC_POS pos = m_cCPs.GetHeadPosition();

    while (pos != NULL)
    {
        COpcConnectionPoint* pCP = m_cCPs[pos];

        if (pCP->GetInterface() == tInterface)
        {
            m_cCPs.RemoveAt(pos);
            pCP->Delete();
            break;
        }

        m_cCPs.GetNext(pos);
    }
}

// GetCallback
HRESULT COpcCPContainer::GetCallback(const IID& tInterface, IUnknown** ippCallback)
{
    COpcConnectionPoint* pCP = NULL;

    OPC_POS pos = m_cCPs.GetHeadPosition();

    while (pos != NULL)
    {
        pCP = m_cCPs.GetNext(pos);

        if (pCP->GetInterface() == tInterface)
        {
            IUnknown* ipUnknown = pCP->GetCallback();
            
            if (ipUnknown != NULL)
            {
                return ipUnknown->QueryInterface(tInterface, (void**)ippCallback);
            }
        }
    }

    return E_FAIL;
}

// IsConnected
bool COpcCPContainer::IsConnected(const IID& tInterface)
{
    COpcConnectionPoint* pCP = NULL;

    OPC_POS pos = m_cCPs.GetHeadPosition();

    while (pos != NULL)
    {
        pCP = m_cCPs.GetNext(pos);

        if (pCP->GetInterface() == tInterface)
        {
            return pCP->IsConnected();
        }
    }

    return false;
}

//==============================================================================
// IConnectionPointContainer

// EnumConnectionPoints
HRESULT COpcCPContainer::EnumConnectionPoints(IEnumConnectionPoints** ppEnum)
{
    // invalid arguments.
    if (ppEnum == NULL)
    {
        return E_POINTER;
    }

    // create enumeration object.
    COpcEnumCPs* pEnumCPs = new COpcEnumCPs(m_cCPs);

    if (pEnumCPs == NULL)
    {
        return E_OUTOFMEMORY;
    }

    // query for enumeration interface.
    HRESULT hResult = pEnumCPs->QueryInterface(IID_IEnumConnectionPoints, (void**)ppEnum);

    // release local reference.
    pEnumCPs->Release();

    return hResult;
}

// FindConnectionPoint
HRESULT COpcCPContainer::FindConnectionPoint(REFIID riid, IConnectionPoint** ppCP)
{
    // invalid arguments.
    if (ppCP == NULL)
    {
        return E_POINTER;
    }

    // search for connection point.
    OPC_POS pos = m_cCPs.GetHeadPosition();

    while (pos != NULL)
    {
        COpcConnectionPoint* pCP = m_cCPs.GetNext(pos);

        if (pCP->GetInterface() == riid)
        {
            return pCP->QueryInterface(IID_IConnectionPoint, (void**)ppCP);
        }
    }

    // connection point not found.
    return CONNECT_E_NOCONNECTION;
}
