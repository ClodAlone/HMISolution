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

#pragma once

#include "COpcUaDaProxyGroup.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace Opc::Ua;
using namespace Opc::Ua::Com;

/// <summary>
/// Used to dispatch callbacks.
/// </summary>>
public ref class COpcUaDaProxyGroupCallback : IComDaGroupCallback
{
public:

	/// <summary>
	/// Creates a new callback,
	/// </summary>
	COpcUaDaProxyGroupCallback(IOPCDataCallback* ipCallback);

	/// <summary>
	/// Releases all resources used by the callback.
	/// </summary>
	~COpcUaDaProxyGroupCallback();

    /// <summary>
    /// The finializer implementation.
    /// </summary>
    !COpcUaDaProxyGroupCallback();

	// ReadCompleted
	virtual void ReadCompleted(
		int groupHandle,
		bool isRefresh,
		int cancelId,
		int transactionId,
		array<int>^ clientHandles,
		array<DaValue^>^ values);

	// WriteCompleted
	virtual void WriteCompleted(
		int groupHandle,
		int transactionId,
		array<int>^ clientHandles,
		array<int>^ errors);

	// CancelSucceeded
	virtual void CancelSucceeded(
		int groupHandle,
		int transactionId);

private:

	/// <summary>
	/// An unmanaged container for unmanaged data stored in the channel.
	/// </summary>
	IOPCDataCallback* m_ipCallback;

	/// <summary>
	/// A synchronization object for the object.
	/// </summary>
	Object^ m_lock;
};
