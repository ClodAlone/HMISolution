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

#ifndef _COpcXmlDocument_H_
#define _COpcXmlDocument_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "COpcMap.h"
#include "OpcXmlType.h"
#include "COpcXmlElement.h"

//==============================================================================
// CLASS:   COpcXmlDocument
// PURPOSE  Facilitiates manipulation of XML documents,

class OPCUTILS_API COpcXmlDocument 
{
    OPC_CLASS_NEW_DELETE()

public:

    //==========================================================================
    // Public Operators

    // Constructor
    COpcXmlDocument(IXMLDOMDocument* ipUnknown = NULL);

    // Copy Constructor
    COpcXmlDocument(const COpcXmlDocument& cDocument);
            
    // Destructor
    ~COpcXmlDocument();

    // Assignment
    COpcXmlDocument& operator=(IUnknown* ipUnknown);
    COpcXmlDocument& operator=(const COpcXmlDocument& cDocument);

    // Accessor
    operator IXMLDOMDocument*() const { return m_ipDocument; }

    //==========================================================================
    // Public Methods
    
    // Init
    virtual bool Init();

    // Clear
    virtual void Clear();

	// New
    virtual bool New();

    // New
    virtual bool New(const COpcString& cRoot, const COpcString& cDefaultNamespace);

	// New
    virtual bool New(IXMLDOMElement* ipElement);

    // Init
    virtual bool LoadXml(LPCWSTR szXml);

    // Load
    virtual bool Load(const COpcString& cFilePath = OPC_EMPTY_STRING);

    // Save
    virtual bool Save(const COpcString& cFilePath = OPC_EMPTY_STRING);

    // GetRoot
    COpcXmlElement GetRoot() const;

	// GetXml
	bool GetXml(COpcString& cXml) const;

    // GetDefaultNamespace
    COpcString GetDefaultNamespace();

    // AddNamespace
    bool AddNamespace(const COpcString& cPrefix, const COpcString& cNamespace);

	// GetNamespaces
	void GetNamespaces(COpcStringMap& cNamespaces);

	// GetNamespacePrefix
	COpcString GetNamespacePrefix(const COpcString& cNamespace);

	// FindElement
	COpcXmlElement FindElement(const COpcString& cXPath);
	
	// FindElements
	UINT FindElements(const COpcString& cXPath, COpcXmlElementList& cElements);

protected:
    
    //==========================================================================
    // Protected Methods

    // GetFilePath
    const COpcString& GetFilePath() const { return m_cFilePath; }

    // SetFilePath
    void SetFilePath(const COpcString& cFilePath) { m_cFilePath = cFilePath; }

private:

    //==========================================================================
    // Private Members

    COpcString       m_cFilePath;
    IXMLDOMDocument* m_ipDocument;
};

#endif // _COpcXmlDocument_H_
