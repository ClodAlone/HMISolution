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

#ifndef _COpcXmlAttribute_H_
#define _COpcXmlAttribute_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "COpcString.h"
#include "COpcArray.h"
#include "OpcXmlType.h"

//==============================================================================
// CLASS:   COpcXmlAttribute
// PURPOSE  Represents an XML attribute.

class OPCUTILS_API COpcXmlAttribute 
{
    OPC_CLASS_NEW_DELETE_ARRAY();

public:

    //==========================================================================
    // Public Operators

    // Constructor
    COpcXmlAttribute(IUnknown* ipUnknown = NULL);

    // Copy Constructor
    COpcXmlAttribute(const COpcXmlAttribute& cAttribute);
            
    // Destructor
    ~COpcXmlAttribute();

    // Assignment
    COpcXmlAttribute& operator=(IUnknown* ipUnknown);
    COpcXmlAttribute& operator=(const COpcXmlAttribute& cAttribute);

    // Accessor
    operator IXMLDOMAttribute*() const { return m_ipAttribute; }

    //==========================================================================
    // Public Methods
    
    // GetName
    COpcString GetName();
    	
	// Prefix
    COpcString GetPrefix();   
        
	// Namespace
	COpcString GetNamespace();

	// GetQualifiedName
	OpcXml::QName GetQualifiedName();

    // GetValue
    COpcString GetValue();
   
protected:

    //==========================================================================
    // Private Members

    IXMLDOMAttribute* m_ipAttribute;
};

//==============================================================================
// TYPE:    COpcXmlAttributeList
// PURPOSE: A list of elements.

typedef COpcArray<COpcXmlAttribute> COpcXmlAttributeList;

#endif // _COpcXmlAttribute_H_
