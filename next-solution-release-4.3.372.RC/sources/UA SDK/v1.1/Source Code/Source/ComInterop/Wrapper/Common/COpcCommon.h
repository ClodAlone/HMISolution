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

#ifndef _COpcCommon_H_
#define _COpcCommon_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "opccomn.h"

#include "OpcDefs.h"
#include "COpcString.h"

//==============================================================================
// FUNCTION: OPCXX_MESSAGE_MODULE_NAME
// PURPOSE:  Names for modules that contain text for standard OPC messages.

#define OPC_MESSAGE_MODULE_NAME_AE    _T("opc_aeps")
#define OPC_MESSAGE_MODULE_NAME_BATCH _T("opcbc_ps")
#define OPC_MESSAGE_MODULE_NAME_DA    _T("opcproxy")
#define OPC_MESSAGE_MODULE_NAME_DX    _T("opcdxps")
#define OPC_MESSAGE_MODULE_NAME_HDA   _T("opchda_ps")
#define OPC_MESSAGE_MODULE_NAME_SEC   _T("opcsec_ps")
#define OPC_MESSAGE_MODULE_NAME_CMD   _T("opccmdps")

//==============================================================================
// CLASS:   COpcCommon
// PURPOSE: Implements the IOPCCommon interface.
// NOTES:

class OPCUTILS_API COpcCommon : public IOPCCommon
{
public:

    //==========================================================================
    // Operators

    // Constructor
    COpcCommon();

    // Destructor 
    ~COpcCommon();

	//==========================================================================
    // Public Methods
    
	// GetErrorString
    static COpcString GetErrorString(
		const COpcString& cModuleName,
        HRESULT           hResult
    );

    // GetErrorString
    static STDMETHODIMP GetErrorString( 
		LPCTSTR szModuleName,
        HRESULT dwError,
        LCID    dwLocale,
        LPWSTR* ppString
    );

	//==========================================================================
    // IOPCCommon

    // SetLocaleID
    STDMETHODIMP SetLocaleID(LCID dwLcid);

    // GetLocaleID
    STDMETHODIMP GetLocaleID(LCID *pdwLcid);

    // QueryAvailableLocaleIDs
    STDMETHODIMP QueryAvailableLocaleIDs(DWORD* pdwCount, LCID** pdwLcid);

    // GetErrorString
    STDMETHODIMP GetErrorString(HRESULT dwError, LPWSTR* ppString);

    // SetClientName
    STDMETHODIMP SetClientName(LPCWSTR szName);

protected:

    //==========================================================================
    // Protected Methods

    // GetLocaleID
    LCID GetLocaleID() const { return m_dwLcid; }

    // GetClientName
    const COpcString& GetClientName() const { return m_cClientName; }

    // GetAvailableLocaleIDs
    virtual const LCID* GetAvailableLocaleIDs() { return NULL; }
	
	// GetErrorString
    virtual STDMETHODIMP GetErrorString(HRESULT dwError, LCID dwLocale, LPWSTR* ppString) = 0;

private:

    //==========================================================================
    // Private Members

    LCID       m_dwLcid;
    COpcString m_cClientName;
};

#endif // _COpcCommon_H_
