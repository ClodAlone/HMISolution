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
#include "COpcText.h"

//==============================================================================
// COpcText

// Constructor
COpcText::COpcText()
{
   Reset();
}

// Reset
void COpcText::Reset()
{
   m_cData.Empty();

   // Search Crtieria
   m_eType = COpcText::NonWhitespace;
   m_cHaltChars.Empty();
   m_uMaxChars = 0;
   m_bNoExtract = false;
   m_cText.Empty();
   m_bSkipLeading = false;
   m_bSkipWhitespace = false;
   m_bIgnoreCase = false;
   m_bEofDelim = false;
   m_bNewLineDelim = false;
   m_cDelims.Empty();
   m_bLeaveDelim = true;
   m_zStart = L'"';
   m_zEnd = L'"';
   m_bAllowEscape = true;

   // Search Results
   m_uStart = 0;
   m_uEnd = 0;
   m_zHaltChar = 0;
   m_uHaltPos = 0;
   m_zDelimChar = 0;
   m_bEof = false;
   m_bNewLine = false;
}

// CopyData
void COpcText::CopyData(LPCWSTR szData, UINT uLength)
{
    m_cData.Empty();

    if (uLength > 0 && szData != NULL)
    {
        LPWSTR wszData = OpcArrayAlloc(WCHAR, uLength+1);
        wcsncpy(wszData, szData, uLength);
        wszData[uLength] = L'\0';
        
        m_cData = wszData;
        OpcFree(wszData);
    }
}

// SetType
void COpcText::SetType(COpcText::Type eType)
{
   Reset();

   m_eType = eType;

   switch (eType)
   {
      case Literal:
      {
         m_cText.Empty();
         m_bSkipLeading = false;
         m_bSkipWhitespace = true;
         m_bIgnoreCase = false;
         break;
      }

      case Whitespace:
      {
         m_bSkipLeading = false;
         m_bEofDelim = true;
         break;
      }

      case NonWhitespace:
      {
         m_bSkipWhitespace = true;
         m_bEofDelim = true;
         break;
      }

      case Delimited:
      {
         m_cDelims.Empty();
         m_bSkipWhitespace = false;
         m_bIgnoreCase = false;
         m_bEofDelim = false;
         m_bNewLineDelim = false;
         m_bLeaveDelim = false;
         break;
      }

      default:
      {
         break;
      }
   }
}
