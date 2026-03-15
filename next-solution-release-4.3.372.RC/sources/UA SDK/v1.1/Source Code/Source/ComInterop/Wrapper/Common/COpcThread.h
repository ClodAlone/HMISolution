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

#ifndef _COpcThread_H_
#define _COpcThread_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"
#include "COpcString.h"

//==============================================================================
// TYPEDEF: PfnOpcThreadControl
// PURPOSE: Pointer to a function that controls a thread.

typedef void (WINAPI *FnOpcThreadControl)(void* pData, bool bStopThread);
typedef FnOpcThreadControl PfnOpcThreadControl;

//==============================================================================
// CLASS:   COpcThread
// PURPOSE: Manages startup and shutdown of a thread.

class OPCUTILS_API COpcThread
{
    OPC_CLASS_NEW_DELETE()

public:

    //==========================================================================
    // Public Operators

    // Constructor
    COpcThread();

    // Destructor
    ~COpcThread();

    //==========================================================================
    // Public Methods

    // Start
    bool Start(
        PfnOpcThreadControl pfnStartProc, 
        void*               pData, 
        DWORD               dwTimeout = INFINITE,
		int                 iPriority = THREAD_PRIORITY_NORMAL);

    // Stop
    void Stop(DWORD dwTimeout = INFINITE);

    // WaitingForStop
    bool WaitingForStop() { return m_bWaitingForStop; }

    // Run
    DWORD Run();

    // PostMessage
    bool PostMessage(UINT uMsgID, WPARAM wParam, LPARAM lParam);

private:

    //==========================================================================
    // Private Members

    DWORD               m_dwID;
    HANDLE              m_hThread;
    HANDLE              m_hEvent;
    bool                m_bWaitingForStop;

    PfnOpcThreadControl m_pfnControl;
    void*               m_pData;
};

#endif // _COpcThread_H_
