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

#ifndef _OpcMatch_H_
#define _OpcMatch_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "OpcDefs.h"
#include "COpcString.h"

//==============================================================================
// FUNCTION: OpcMatchPattern
// PURPOSE:  Provides the same functionality as the LIKE operator in Visual Basic.
//
// Return Value:
// 
// If string matches pattern, return is TRUE; if there is no match, return is FALSE. 
// If the pattern is empty, the return is always TRUE;
// If the string is empty and the pattern is not empty, the return is FALSE;
//
// Parameters:
//
//   szString       String to be compared with pattern.
//   szPattern      Any string conforming to the pattern-matching conventions described in Remarks.
//   bCaseSensitive TRUE if comparison should be case sensitive.
// 
// Remarks:
// 
// A versatile tool used to compare two strings. The pattern-matching features 
// allow you to use wildcard characters, character lists, or character ranges, in 
// any combination, to match strings. The following table shows the characters
// allowed in pattern and what they match:
//
// Characters in pattern	Matches in string
//			?				Any single character.
//			*				Zero or more characters.
//			#				Any single digit (0-9).
//		[charlist]			Any single character in charlist.
//		[!charlist]			Any single character not in charlist.
//		
// A group of one or more characters (charlist) enclosed in brackets ([])
// can be used to match any single character in string and can include almost 
// any charcter code, including digits. 
//
// 0Note To match the special characters left bracket ([), question mark (?), 
// number sign (#), and asterisk (*), enclose them in brackets. 
//
// The right bracket (]) can't be used within a group to match itself, but it 
// can be used outside a group as an individual character.
//
// By using a hyphen (-) to separate the upper and lower bounds of the range, 
// charlist can specify a range of characters. For example, [A-Z] results in
// a match if the corresponding character position in string contains any 
// uppercase letters in the range A-Z. Multiple ranges are included within 
// the brackets without delimiters. 
//
// Other important rules for pattern matching include the following:
//
// - An exclamation point (!) at the beginning of charlist means that 
//   a match is made if any character except the characters in charlist 
//   is found in string. When used outside brackets, the exclamation 
//   point matches itself.
// 
// - A hyphen (-) can appear either at the beginning (after an exclamation 
//   point if one is used) or at the end of charlist to match itself. In 
//   any other location, the hyphen is used to identify a range of characters. 
//
// - When a range of characters is specified, they must appear in ascending 
//   sort order (from lowest to highest). [A-Z] is a valid pattern, 
//   but [Z-A] is not. 
//
// - The character sequence [] is considered a zero-length string (""). 

OPCUTILS_API bool OpcMatchPattern(
	LPCTSTR szString, 
	LPCTSTR szPattern, 
	bool bCaseSensitive = false
);

#endif // _OpcMatch_H_
