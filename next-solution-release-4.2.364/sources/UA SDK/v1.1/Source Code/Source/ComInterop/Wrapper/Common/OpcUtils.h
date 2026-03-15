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

#ifndef _OpcUtils_H_
#define _OpcUtils_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "opccomn.h"

#define OPCUTILS_API

#include "OpcDefs.h"
#include "COpcString.h"
#include "COpcFile.h"
#include "OpcMatch.h"
#include "COpcCriticalSection.h"
#include "COpcArray.h"
#include "COpcList.h"
#include "COpcMap.h"
#include "COpcSortedArray.h"
#include "COpcText.h"
#include "COpcTextReader.h"
#include "COpcThread.h"
#include "OpcCategory.h"
#include "OpcRegistry.h"
#include "OpcXmlType.h"
#include "COpcXmlAnyType.h"
#include "COpcXmlElement.h"
#include "COpcXmlDocument.h"
#include "COpcVariant.h"
#include "COpcComObject.h"
#include "COpcClassFactory.h"
#include "COpcComModule.h"
#include "COpcCommon.h"
#include "COpcConnectionPoint.h"
#include "COpcCPContainer.h"
#include "COpcEnumCPs.h"
#include "COpcEnumString.h"
#include "COpcEnumUnknown.h"
#include "COpcSecurity.h"
#include "COpcThreadPool.h"
#include "COpcBrowseElement.h"

#endif // _OpcUtils_H_
