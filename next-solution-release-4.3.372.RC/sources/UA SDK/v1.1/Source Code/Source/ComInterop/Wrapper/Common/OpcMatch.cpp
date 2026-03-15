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
#include "OpcMatch.h"

//==============================================================================
// Local Functions

// ConvertCase
static inline int ConvertCase(int c, bool bCaseSensitive)
{
	return (bCaseSensitive)?c:toupper(c);
}

//==============================================================================
// OpcMatchPattern

bool OpcMatchPattern(
	LPCTSTR szString, 
	LPCTSTR szPattern, 
	bool bCaseSensitive
)
{
	// an empty pattern always matches.
	if (szPattern == NULL)
	{
		return true;
	}

	// an empty string never matches.
	if (szString == NULL)
	{
		return false;
	}

    TCHAR c, p, l;

    for (;;)
    {
        switch (p = ConvertCase(*szPattern++, bCaseSensitive))
        {
			// end of pattern.
			case 0:                            
			{
				return (*szString)?false:true; // if end of string true
			}

			// match zero or more char.
			case _T('*'):
			{
				while (*szString) 
				{   
					if (OpcMatchPattern(szString++, szPattern, bCaseSensitive))
					{
						return true;
					}
				}
			
				return OpcMatchPattern(szString, szPattern, bCaseSensitive);
			}

			// match any one char.
			case _T('?'):
			{
				if (*szString++ == 0) 
				{
					return false;  // not end of string 
				}

				break;
			}

			// match char set 
			case _T('['): 
			{
				if ((c = ConvertCase(*szString++, bCaseSensitive)) == 0)
				{
					return false; // syntax 
				}

				l = 0; 

				// match a char if NOT in set []
				if (*szPattern == _T('!')) 
				{
					++szPattern;

					while ((p = ConvertCase(*szPattern++, bCaseSensitive)) != _T('\0')) 
					{
						if (p == _T(']')) // if end of char set, then 
						{
							break; // no match found 
						}

						if (p == _T('-')) 
						{
							// check a range of chars? 
							p = ConvertCase( *szPattern, bCaseSensitive );

							// get high limit of range 
							if (p == 0  ||  p == _T(']'))
							{
								return false; // syntax 
							}

							if (c >= l  &&  c <= p) 
							{
								return false; // if in range, return false
							}
						} 

						l = p;
						
						if (c == p) // if char matches this element 
						{
							return false; // return false 
						}
					} 
				}

				// match if char is in set []
				else 
				{
					while ((p = ConvertCase(*szPattern++, bCaseSensitive)) != _T('\0')) 
					{
						if (p == _T(']')) // if end of char set, then no match found 
						{
							return false;
						}

						if (p == _T('-')) 
						{   
							// check a range of chars? 
							p = ConvertCase( *szPattern, bCaseSensitive );
							
							// get high limit of range 
							if (p == 0  ||  p == _T(']'))
							{
								return false; // syntax 
							}

							if (c >= l  &&  c <= p) 
							{
								break; // if in range, move on 
							}
						} 

						l = p;
						
						if (c == p) // if char matches this element move on 
						{
							break;           
						}
					} 

					while (p  &&  p != _T(']')) // got a match in char set skip to end of set
					{
						p = *szPattern++;             
					}
				}

				break; 
			}

			// match digit.
			case _T('#'):
			{
				c = *szString++; 

				if (!_istdigit(c))
				{
					return false; // not a digit
				}

				break;
			}

			// match exact char.
			default: 
			{
				c = ConvertCase(*szString++, bCaseSensitive); 
				
				if (c != p) // check for exact char
				{
					return false; // not a match
				}

				break;
			}
        } 
    } 

	return false;
}
