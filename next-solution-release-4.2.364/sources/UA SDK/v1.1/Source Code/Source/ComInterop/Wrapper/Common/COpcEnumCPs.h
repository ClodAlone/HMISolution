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

#ifndef _COpcEnumCPs_H_
#define _COpcEnumCPs_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"
#include "COpcComObject.h"
#include "COpcCPContainer.h"

//==============================================================================
// CLASS:   COpcEnumCPs
// PURPOSE: Implements the IEnumConnectionPoints interface.
// NOTES:

class OPCUTILS_API COpcEnumCPs 
:
    public COpcComObject,
    public IEnumConnectionPoints
{     
    OPC_BEGIN_INTERFACE_TABLE(COpcEnumCPs)
        OPC_INTERFACE_ENTRY(IEnumConnectionPoints)
    OPC_END_INTERFACE_TABLE()

    OPC_CLASS_NEW_DELETE()

public:

    //==========================================================================
    // Operators

    // Constructor
    COpcEnumCPs();
    
    // Constructor
    COpcEnumCPs(const COpcList<COpcConnectionPoint*>& cCPs);

    // Destructor 
    ~COpcEnumCPs();

    //==========================================================================
    // IEnumConnectionPoints

    // Next
    STDMETHODIMP Next(
        ULONG              cConnections,
        LPCONNECTIONPOINT* ppCP,
        ULONG*             pcFetched
    );

    // Skip
    STDMETHODIMP Skip(ULONG cConnections);

    // Reset
    STDMETHODIMP Reset();

    // Clone
    STDMETHODIMP Clone(IEnumConnectionPoints** ppEnum);

private:

    //==========================================================================
    // Private Members

    OPC_POS                 m_pos;
    COpcConnectionPointList m_cCPs;
};

#endif // _COpcEnumCPs_H_
