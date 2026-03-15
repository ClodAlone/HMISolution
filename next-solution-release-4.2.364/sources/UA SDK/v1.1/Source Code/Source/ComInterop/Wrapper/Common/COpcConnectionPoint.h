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

#ifndef _COpcConnectionPoint_H_
#define _COpcConnectionPoint_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "ocidl.h"

#include "OpcDefs.h"
#include "COpcComObject.h"
#include "COpcCriticalSection.h"

class COpcCPContainer;

//==============================================================================
// CLASS:   COpcConnectionPoint
// PURPOSE: Implements the IConnectionPoint interface.
// NOTES:

class OPCUTILS_API COpcConnectionPoint
: 
    public COpcComObject,
    public COpcSynchObject,
    public IConnectionPoint
{
    OPC_BEGIN_INTERFACE_TABLE(COpcConnectionPoint)
        OPC_INTERFACE_ENTRY(IConnectionPoint)
    OPC_END_INTERFACE_TABLE()

    OPC_CLASS_NEW_DELETE()

public:

    //==========================================================================
    // Operators

    // Constructor
    COpcConnectionPoint();

    // Constructor
    COpcConnectionPoint(const IID& tIid, COpcCPContainer* pContainer);

    // Destructor 
    ~COpcConnectionPoint();

    //==========================================================================
    // IConnectionPoint

    // GetConnectionInterface
    STDMETHODIMP GetConnectionInterface(IID* pIID);

    // GetConnectionPointContainer
    STDMETHODIMP GetConnectionPointContainer(IConnectionPointContainer** ppCPC);

    // Advise
    STDMETHODIMP Advise(IUnknown* pUnkSink, DWORD* pdwCookie);

    // Unadvise
    STDMETHODIMP Unadvise(DWORD dwCookie);

    // EnumConnections
    STDMETHODIMP EnumConnections(IEnumConnections** ppEnum);

    //==========================================================================
    // Public Methods

    // GetCallback
    IUnknown* GetCallback() { return m_ipCallback; }

    // GetInterface
    const IID& GetInterface() { return m_tInterface; }

    // Delete
    bool Delete();

    // IsConnected
    bool IsConnected() { return (m_dwCookie != NULL); }
    
private:

    //==========================================================================
    // Private Members

    IID              m_tInterface;
    COpcCPContainer* m_pContainer;
    IUnknown*        m_ipCallback;
    DWORD            m_dwCookie;
    bool             m_bFetched;
};

//==============================================================================
// FUNCTION: OpcConnect
// PURPOSE:  Establishes a connection to the server.

OPCUTILS_API HRESULT OpcConnect(
    IUnknown* ipSource, 
    IUnknown* ipSink, 
    REFIID    riid, 
    DWORD*    pdwConnection);

//==============================================================================
// FUNCTION: OpcDisconnect
// PURPOSE:  Closes a connection to the server.

OPCUTILS_API HRESULT OpcDisconnect(
    IUnknown* ipSource, 
    REFIID    riid, 
    DWORD     dwConnection);

#endif // _COpcConnectionPoint_H_
