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
#include "COpcThread.h"

//==============================================================================
// Local Functions

static DWORD WINAPI ThreadProc(LPVOID lpParameter)
{
    return ((COpcThread*)lpParameter)->Run();
}

//==============================================================================
// COpcThread

// Constructor
COpcThread::COpcThread()
{
    m_dwID            = NULL;
    m_hThread         = NULL;
    m_hEvent          = NULL;
    m_pfnControl      = NULL;
    m_pData           = NULL;
    m_bWaitingForStop = false;
}

// Destructor
COpcThread::~COpcThread() 
{
    if (m_hThread != NULL) CloseHandle(m_hThread);
    if (m_hEvent != NULL)  CloseHandle(m_hEvent);
}

//==============================================================================
// Public Methods

// Start
bool COpcThread::Start(
    PfnOpcThreadControl pfnControl, 
    void*               pData, 
    DWORD               dwTimeout,
	int                 iPriority)
{
    m_pfnControl = pfnControl;
    m_pData      = pData;

    // no control procedure specified.
    if (m_pfnControl == NULL)
    {
        return false;
    }

    // thread already running.
    if (m_hThread != NULL)
    {
        return false;
    }

    // create thread started event.
    m_hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

    if (m_hEvent == NULL)
    {
        return false;
    }

    // start thread.
    m_hThread = CreateThread(
        NULL,
        NULL,
        ThreadProc,
        (void*)this,
        NULL,
        &m_dwID);

    if (m_hThread == NULL)
    {
        return false;
    }

	// set thread priority.
	SetThreadPriority(m_hThread, iPriority);

    // wait for thread start.
    DWORD dwResult = WaitForSingleObject(m_hEvent, dwTimeout);

    if (dwResult != WAIT_OBJECT_0)
    {
        return false;
    }

    // reset thread event.
    ResetEvent(m_hEvent);

    return true;
}

// Stop
void COpcThread::Stop(DWORD dwTimeout)
{
    // check if thread is not stopping itself and if thread is running.
    if (m_dwID != GetCurrentThreadId() && m_hThread != NULL)
    {
        m_bWaitingForStop = true;

        // post quit message.
        PostThreadMessage(m_dwID, WM_QUIT, 0, 0);

        // call thread procedure for user defined stop message.
        m_pfnControl(NULL, true);

        // wait for thread stopped event.
        WaitForSingleObject(m_hEvent, dwTimeout);
    }

    // close handles.
    if (m_hThread != NULL) CloseHandle(m_hThread);
    if (m_hEvent != NULL)  CloseHandle(m_hEvent);

    // intialize state.
    m_dwID            = NULL;
    m_hThread         = NULL;
    m_hEvent          = NULL;
    m_pfnControl      = NULL;
    m_pData           = NULL;
    m_bWaitingForStop = false;
}

// Run
DWORD COpcThread::Run()
{
    // initialize message queue.
    MSG cMsg; memset(&cMsg, 0, sizeof(cMsg));
    PeekMessage(&cMsg, NULL, 0, 0, PM_NOREMOVE);

    // signal that the thread started.
    SetEvent(m_hEvent);

    // call thread procedure.
    m_pfnControl(m_pData, false);

    // signal that the thread stopped.
    CloseHandle(m_hThread);
    InterlockedExchange((LONG*)&m_hThread, NULL);

    SetEvent(m_hEvent);

    return 0;
}

// PostMessage
bool COpcThread::PostMessage(UINT uMsgID, WPARAM wParam, LPARAM lParam)
{
    return (PostThreadMessage(m_dwID, uMsgID, wParam, lParam) != 0);
}
