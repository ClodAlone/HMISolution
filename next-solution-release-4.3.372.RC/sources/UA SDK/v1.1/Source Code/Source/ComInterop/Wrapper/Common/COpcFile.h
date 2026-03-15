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

#ifndef _COpcFile_H_
#define _COpcFile_H_

#include "COpcString.h"

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

//==============================================================================
// CLASS:   COpcFile
// PURPOSE  Facilitiates manipulation of XML Elements,

class COpcFile
{
    OPC_CLASS_NEW_DELETE();

public:

    //==========================================================================
    // Public Operators

    // Constructor
    COpcFile();
            
    // Destructor
    ~COpcFile();

    //==========================================================================
    // Public Methods

	// Create
	bool Create(const COpcString& cFileName);

	// Open
	bool Open(const COpcString& cFileName, bool bReadOnly = true);

	// Close
	void Close();

	// Read
	UINT Read(BYTE* pBuffer, UINT uSize);

	// Write
	UINT Write(BYTE* pBuffer, UINT uSize);
	
	// GetFileSize
	UINT GetFileSize();

	// GetLastModified
	FILETIME GetLastModified();

	// GetMemoryMapping
	BYTE* GetMemoryMapping();

private:

    //==========================================================================
    // Private Members

	HANDLE m_hFile;
	HANDLE m_hMapping;
	BYTE*  m_pView;
	bool   m_bReadOnly;
};

#endif // _COpcFile_H_
