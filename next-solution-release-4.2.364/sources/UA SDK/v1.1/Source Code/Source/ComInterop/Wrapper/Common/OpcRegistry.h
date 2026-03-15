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

#ifndef _OpcRegistry_H_
#define _OpcRegistry_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"

//==============================================================================
// FUNCTION: OpcRegGetValue
// PURPOSE:  Gets a string value from the registry.

bool OPCUTILS_API OpcRegGetValue(
	HKEY    hBaseKey,
	LPCTSTR tsSubKey,
	LPCTSTR tsValueName,
	LPTSTR* ptsValue
);

//==============================================================================
// FUNCTION: OpcRegGetValue
// PURPOSE:  Gets a DWORD value from the registry.

bool OPCUTILS_API OpcRegGetValue(
	HKEY    hBaseKey,
	LPCTSTR tsSubKey,
	LPCTSTR tsValueName,
	DWORD*  pdwValue
);

//==============================================================================
// FUNCTION: OpcRegGetValue
// PURPOSE:  Gets a DWORD value from the registry.

bool OPCUTILS_API OpcRegGetValue(
	HKEY    hBaseKey,
	LPCTSTR tsSubKey,
	LPCTSTR tsValueName,
	BYTE**  ppValue,
	DWORD*  pdwLength
);

//==============================================================================
// FUNCTION: OpcRegSetValue
// PURPOSE:  Sets a string value in the registry.

bool OPCUTILS_API OpcRegSetValue(
	HKEY    hBaseKey,
	LPCTSTR tsSubKey,
	LPCTSTR tsValueName,
	LPCTSTR tsValue
);

//==============================================================================
// FUNCTION: OpcRegSetValue
// PURPOSE:  Gets a DWORD value from the registry.

bool OPCUTILS_API OpcRegSetValue(
	HKEY    hBaseKey,
	LPCTSTR tsSubKey,
	LPCTSTR tsValueName,
	DWORD   dwValue
);

//==============================================================================
// FUNCTION: OpcRegSetValue
// PURPOSE:  Sets a string value in the registry.

bool OPCUTILS_API OpcRegSetValue(
	HKEY    hBaseKey,
	LPCTSTR tsSubKey,
	LPCTSTR tsValueName,
	BYTE*   pValue,
	DWORD   dwLength
);

//==============================================================================
// FUNCTION: OpcRegDeleteKey
// PURPOSE:  Recursively deletes a key and all sub keys.
// NOTES:

bool OPCUTILS_API OpcRegDeleteKey(
	HKEY    hBaseKey,
	LPCTSTR tsSubKey
);

#endif //ndef _OpcRegistry_H_
