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
#include "COpcXmlAttribute.h"
#include "COpcVariant.h"

//==============================================================================
// COpcXmlAttribute

// Constructor
COpcXmlAttribute::COpcXmlAttribute(IUnknown* ipUnknown)
{
    m_ipAttribute = NULL;
    *this = ipUnknown;
}

// Copy Constructor
COpcXmlAttribute::COpcXmlAttribute(const COpcXmlAttribute& cAttribute)
{
    m_ipAttribute = NULL;
    *this = cAttribute.m_ipAttribute;
}

// Destructor
COpcXmlAttribute::~COpcXmlAttribute()
{
    if (m_ipAttribute != NULL)
    {
        m_ipAttribute->Release();
        m_ipAttribute = NULL;
    }
}

// Assignment
COpcXmlAttribute& COpcXmlAttribute::operator=(IUnknown* ipUnknown)
{
    if (m_ipAttribute != NULL)
    {
        m_ipAttribute->Release();
        m_ipAttribute = NULL;
    }

    if (ipUnknown != NULL)
    {
        HRESULT hResult = ipUnknown->QueryInterface(__uuidof(IXMLDOMAttribute), (void**)&m_ipAttribute);

        if (FAILED(hResult))
        {
            m_ipAttribute = NULL;
        }
    }

    return *this;
}

// Assignment
COpcXmlAttribute& COpcXmlAttribute::operator=(const COpcXmlAttribute& cAttribute)
{
    if (this == &cAttribute)
    {
        return *this;
    }

    *this = cAttribute.m_ipAttribute;
    return *this;
}

// GetName
COpcString COpcXmlAttribute::GetName()
{
	if (m_ipAttribute != NULL)
	{
		BSTR bstrName = NULL;

		HRESULT hResult = m_ipAttribute->get_name(&bstrName);
		OPC_ASSERT(SUCCEEDED(hResult));

		COpcString cName = bstrName;
		SysFreeString(bstrName);

		return cName;
	}

	return (LPCWSTR)NULL;
}

// GetPrefix
COpcString COpcXmlAttribute::GetPrefix()
{
	if (m_ipAttribute != NULL)
	{
		BSTR bstrName = NULL;

		HRESULT hResult = m_ipAttribute->get_prefix(&bstrName);
		OPC_ASSERT(SUCCEEDED(hResult));

		COpcString cName = bstrName;
		SysFreeString(bstrName);

		return cName;
	}

	return (LPCWSTR)NULL;
}

// GetQualifiedName
OpcXml::QName COpcXmlAttribute::GetQualifiedName()
{
	OpcXml::QName cQName;

	if (m_ipAttribute != NULL)
	{
		BSTR bstrName = NULL;

		HRESULT hResult = m_ipAttribute->get_baseName(&bstrName);
		OPC_ASSERT(SUCCEEDED(hResult));

		cQName.SetName(bstrName);
		cQName.SetNamespace(GetNamespace());
		
		SysFreeString(bstrName);
	}

	return cQName;
}

// GetNamespace
COpcString COpcXmlAttribute::GetNamespace()
{
	if (m_ipAttribute != NULL)
	{
		BSTR bstrName = NULL;

		HRESULT hResult = m_ipAttribute->get_namespaceURI(&bstrName);
		OPC_ASSERT(SUCCEEDED(hResult));

		COpcString cName = bstrName;
		SysFreeString(bstrName);

		return cName;
	}

	return (LPCWSTR)NULL;
}

// GetValue
COpcString COpcXmlAttribute::GetValue()
{
    VARIANT cVariant; OpcVariantInit(&cVariant);

    if (FAILED(m_ipAttribute->get_value(&cVariant))) { OPC_ASSERT(false); }

    COpcString cValue = cVariant.bstrVal;
    OpcVariantClear(&cVariant);

    return cValue;
}
