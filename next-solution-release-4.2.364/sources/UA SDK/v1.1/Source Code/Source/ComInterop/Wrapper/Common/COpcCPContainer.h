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

#ifndef _COpcCPContainer_H_
#define _COpcCPContainer_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"
#include "COpcList.h"
#include "COpcConnectionPoint.h"

//==============================================================================
// CLASS:   COpcConnectionPointList
// PURPOSE: Stores a list of connection points.

typedef COpcList<COpcConnectionPoint*> COpcConnectionPointList;
template class OPCUTILS_API COpcList<COpcConnectionPoint*>;

//==============================================================================
// CLASS:   COpcCPContainer
// PURPOSE: Implements the IConnectionPointContainer interface.
// NOTES:

class OPCUTILS_API COpcCPContainer : public IConnectionPointContainer
{
public:

    //==========================================================================
    // Operators

    // Constructor
    COpcCPContainer();

    // Destructor 
    ~COpcCPContainer();

    //==========================================================================
    // IConnectionPointContainer

    // EnumConnectionPoints
    STDMETHODIMP EnumConnectionPoints(IEnumConnectionPoints** ppEnum);

    // FindConnectionPoint
    STDMETHODIMP FindConnectionPoint(REFIID riid, IConnectionPoint** ppCP);

    //==========================================================================
    // Public Methods

	// OnAdvise
	virtual void OnAdvise(REFIID riid, DWORD dwCookie) {}

	// OnUnadvise
	virtual void OnUnadvise(REFIID riid, DWORD dwCookie) {}

protected:

    //==========================================================================
    // Protected Methods

    // RegisterInterface
    void RegisterInterface(const IID& tInterface);

    // UnregisterInterface
    void UnregisterInterface(const IID& tInterface);

    // GetCallback
    HRESULT GetCallback(const IID& tInterface, IUnknown** ippCallback);

    // IsConnected
    bool IsConnected(const IID& tInterface);

    //==========================================================================
    // Protected Members

    COpcConnectionPointList m_cCPs;
};

#endif // _COpcCPContainer_H_
