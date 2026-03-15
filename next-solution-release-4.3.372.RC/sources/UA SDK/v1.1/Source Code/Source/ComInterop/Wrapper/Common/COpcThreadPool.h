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

#ifndef _COpcThreadPool_H_
#define _COpcThreadPool_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"
#include "COpcList.h"
#include "COpcCriticalSection.h"

class COpcMessage;

//==============================================================================
// INTERFACE: IOpcMessageCallback
// PURPOSE:   A interface to an object that processes messages.

interface IOpcMessageCallback : public IUnknown
{
	// ProcessMessage
	virtual void ProcessMessage(COpcMessage& cMsg) = 0;
};

//==============================================================================
// CLASS:   COpcMessage
// PURPOSE: A base class for a message.

class OPCUTILS_API COpcMessage
{
    OPC_CLASS_NEW_DELETE();

public:

    //==========================================================================
    // Public Operators

    // Constructor
    COpcMessage(UINT uType, IOpcMessageCallback* ipCallback);

    // Copy Constructor
    COpcMessage(const COpcMessage& cMessage);

	// Destructor
    virtual ~COpcMessage();

    //==========================================================================
    // Public Methods

	// Process
	virtual void Process()
	{
		if (m_ipCallback != NULL)
		{
			m_ipCallback->ProcessMessage(*this);
		}
	}

	// GetID
	UINT GetID() { return m_uID; }

	// GetType
	UINT GetType() { return m_uType; }

protected:

    //==========================================================================
    // Protected Operators

	UINT                 m_uID;
	UINT                 m_uType;
	IOpcMessageCallback* m_ipCallback;
};

//==============================================================================
// CLASS:   COpcThreadPool
// PURPOSE: Manages a pool of threads that process queued messages.

class OPCUTILS_API COpcThreadPool : public COpcSynchObject
{
    OPC_CLASS_NEW_DELETE();

public:

    //==========================================================================
    // Public Operators

    // Constructor
    COpcThreadPool();

	// Destructor
    ~COpcThreadPool();

    //==========================================================================
    // Public Methods
     
	// Start
	bool Start();

	// Stop
	void Stop();

	// Run
	void Run();

    // QueueMessage
	bool QueueMessage(COpcMessage* pMsg);

	// SetSize
	void SetSize(UINT uMinThreads, UINT uMaxThreads);

private:

    //==========================================================================
    // Private Members

	HANDLE                  m_hEvent;
	COpcList<COpcMessage*>  m_cQueue;

	UINT                    m_uTotalThreads;
	UINT                    m_uWaitingThreads;
	UINT                    m_uMinThreads;
	UINT                    m_uMaxThreads;
};

#endif // _COpcThreadPool_H_
