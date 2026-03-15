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

#ifndef _COpcTextReader_H
#define _COpcTextReader_H

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"
#include "COpcText.h"

//==============================================================================
// CLASS:   COpcTextReader
// PURPOSE: Extracts tokens from a stream.

class OPCUTILS_API COpcTextReader
{
    OPC_CLASS_NEW_DELETE();

public:

    //==========================================================================
    // Operators

    // Constructor
    COpcTextReader(const COpcString& cBuffer);  
    COpcTextReader(LPCSTR szBuffer, UINT uLength = -1);  
    COpcTextReader(LPCWSTR szBuffer, UINT uLength = -1);  
 
    // Destructor
    ~COpcTextReader(); 

    //==========================================================================
    // Public Methods
  
    // GetNext
    bool GetNext(COpcText& cText);

    // GetBuf
    LPCWSTR GetBuf() const { return m_szBuf; }

private:

    //==========================================================================
    // Private Methods

    // ReadData
    bool ReadData();

    // FindToken
    bool FindToken(COpcText& cText);

    // FindLiteral
    bool FindLiteral(COpcText& cText);

    // FindNonWhitespace
    bool FindNonWhitespace(COpcText& cText);

    // FindWhitespace
    bool FindWhitespace(COpcText& cText);
    
    // FindDelimited
    bool FindDelimited(COpcText& cText);

    // FindEnclosed
    bool FindEnclosed(COpcText& cText);

    // CheckForHalt
    bool CheckForHalt(COpcText& cText, UINT uIndex);
    
    // CheckForDelim
    bool CheckForDelim(COpcText& cText, UINT uIndex);

    // SkipWhitespace
    UINT SkipWhitespace(COpcText& cText);

    // CopyData
    void CopyData(COpcText& cText, UINT uStart, UINT uEnd);

    //==========================================================================
    // Private Members

    LPWSTR m_szBuf;
    UINT   m_uLength;
    UINT   m_uEndOfData;
};

#endif //ndef _COpcTextReader_H
