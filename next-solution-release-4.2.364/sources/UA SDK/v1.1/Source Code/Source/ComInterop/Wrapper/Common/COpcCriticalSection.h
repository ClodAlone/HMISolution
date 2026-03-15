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

#ifndef _COpcCriticalSection_H_
#define _COpcCriticalSection_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"

//==============================================================================
// TITLE:   COpcCriticalSection.h
// PURPOSE: Implements a wrapper for a critical section.
// NOTES:

class OPCUTILS_API COpcCriticalSection
{
    OPC_CLASS_NEW_DELETE();

public:

    //==========================================================================
    // Operators

    // Constructor
    inline COpcCriticalSection()
    {
        m_ulLocks  = 0;
        m_dwThread = -1;

        InitializeCriticalSection(&m_csLock);
    }

    // Destructor
    inline ~COpcCriticalSection()
    {
        DeleteCriticalSection(&m_csLock);
    }

    //==========================================================================
    // Public Methods

    // Lock
    inline void Lock()
    {
        EnterCriticalSection(&m_csLock);

        if (m_dwThread == -1)
        {
            m_dwThread = GetCurrentThreadId();
        }

        OPC_ASSERT(m_dwThread == GetCurrentThreadId());

        m_ulLocks++;
    }

    // Unlock
    inline void Unlock()
    {
        OPC_ASSERT(m_dwThread == GetCurrentThreadId());
        OPC_ASSERT(m_ulLocks > 0);

        m_ulLocks--;

        if (m_ulLocks == 0)
        {
            m_dwThread = -1;
        }

        LeaveCriticalSection(&m_csLock);
    }

    // HasLock
    inline bool HasLock()
	{
		return (m_dwThread == GetCurrentThreadId());
	}

private: 
   
   //===========================================================================
   // Private Members

   CRITICAL_SECTION m_csLock;
   DWORD            m_dwThread;
   ULONG            m_ulLocks;
};

//==============================================================================
// TITLE:   COpcLock.h
// PURPOSE: Implements a class that leaves a critical section when destroyed.
// NOTES:

class COpcLock
{
public:

    //==========================================================================
    // Operators

    // Constructor
    inline COpcLock(const COpcCriticalSection& cLock)
    :
        m_pLock(NULL)
    {
        m_pLock = (COpcCriticalSection*)&cLock;
        m_pLock->Lock();
        m_uLocks = 1;
    }

    // Destructor
    inline ~COpcLock()
    {
        while (m_uLocks > 0) Unlock();
    }

    //==========================================================================
    // Public Methods

    inline void Unlock()
    {
        OPC_ASSERT(m_uLocks > 0);

        m_uLocks--;

        if (m_uLocks == 0)
        {
            m_pLock->Unlock();
        }
    }

    inline void Lock()
    {
        if (m_uLocks == 0)
        {
            m_pLock->Lock();
        }

        m_uLocks++;
    }

private:

    UINT                 m_uLocks;
    COpcCriticalSection* m_pLock;       
};

//==============================================================================
// TITLE:   COpcSynchObject.h
// PURPOSE: A base class that adds a critical section to a class.

class COpcSynchObject
{

public:

    // Cast
    operator COpcCriticalSection&() { return m_cLock; }
    operator const COpcCriticalSection&() const { return m_cLock; }

	// HasLock
	bool HasLock() { return m_cLock.HasLock(); }

private:

    COpcCriticalSection m_cLock;
};

#endif // _COpcCriticalSection_H_
