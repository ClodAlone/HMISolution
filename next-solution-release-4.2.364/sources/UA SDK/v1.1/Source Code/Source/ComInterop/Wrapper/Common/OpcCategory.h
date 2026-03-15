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

#ifndef _OpcCategory_H_
#define _OpcCategory_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"
#include "COpcComObject.h"
#include "COpcList.h"

//==============================================================================
// FUNCTION: OpcEnumServers
// PURPOSE:  Enumerates servers in the specified category on the host.
 
// OpcEnumServers
OPCUTILS_API HRESULT OpcEnumServersInCategory(
    LPCTSTR          tsHostName,
    const CATID&     tCategory,
    COpcList<CLSID>* pServers 
);

//==============================================================================
// FUNCTION: RegisterClsidInCategory
// PURPOSE:  Registers a CLSID as belonging to a component category. 
 
HRESULT RegisterClsidInCategory(REFCLSID clsid, CATID catid, LPCWSTR szDescription) ;

//==============================================================================
// FUNCTION: UnregisterClsidInCategory
// PURPOSE:  Unregisters a CLSID as belonging to a component category. 
HRESULT UnregisterClsidInCategory(REFCLSID clsid, CATID catid);

//==============================================================================
// STRUCT:  TClassCategories
// PURPOSE: Associates a clsid with a component category. 

struct TClassCategories 
{
    const CLSID* pClsid;
    const CATID* pCategory;
	const TCHAR* szDescription;
};

//==============================================================================
// MACRO:   OPC_BEGIN_CATEGORY_TABLE
// PURPOSE: Begins the module class category table.

#define OPC_BEGIN_CATEGORY_TABLE() static const TClassCategories g_pCategoryTable[] = {

//==============================================================================
// MACRO:   OPC_CATEGORY_TABLE_ENTRY
// PURPOSE: An entry in the module class category table.

#define OPC_CATEGORY_TABLE_ENTRY(xClsid, xCatid, xDescription) {&(__uuidof(xClsid)), &(xCatid), (xDescription)},

//==============================================================================
// MACRO:   OPC_END_CATEGORY_TABLE
// PURPOSE: Ends the module class category table.

#define OPC_END_CATEGORY_TABLE() {NULL, NULL, NULL}};

#endif // _OpcCategory_H_
