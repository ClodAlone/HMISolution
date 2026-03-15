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

#ifndef _COpcEnumUnknowns_H_
#define _COpcEnumUnknowns_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"
#include "COpcComObject.h"
#include "COpcList.h"
#include "COpcString.h"

//==============================================================================
// CLASS:   COpcEnumUnknown
// PURPOSE: A class to implement the IEnumUnknown interface.
// NOTES:

class OPCUTILS_API COpcEnumUnknown 
:
    public COpcComObject,
    public IEnumUnknown
{     
    OPC_BEGIN_INTERFACE_TABLE(COpcEnumUnknown)
        OPC_INTERFACE_ENTRY(IEnumUnknown)
    OPC_END_INTERFACE_TABLE()

    OPC_CLASS_NEW_DELETE()

public:

    //==========================================================================
    // Operators

    // Constructor
    COpcEnumUnknown();

    // Constructor
    COpcEnumUnknown(UINT uCount, IUnknown**& pUnknowns);

    // Destructor 
    ~COpcEnumUnknown();

    //==========================================================================
    // IEnumConnectionPoints
       
    // Next
    STDMETHODIMP Next(
        ULONG      celt,          
        IUnknown** rgelt,   
        ULONG*     pceltFetched
    );

    // Skip
    STDMETHODIMP Skip(ULONG celt);

    // Reset
    STDMETHODIMP Reset();

    // Clone
    STDMETHODIMP Clone(IEnumUnknown** ppEnum);

private:

    //==========================================================================
    // Private Members

    UINT       m_uIndex;
    UINT       m_uCount;
    IUnknown** m_pUnknowns;
};

#endif // _COpcEnumUnknowns_H_
