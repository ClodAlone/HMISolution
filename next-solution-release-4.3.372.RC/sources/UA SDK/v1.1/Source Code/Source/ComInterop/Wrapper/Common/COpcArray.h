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

#ifndef _COpcArray_H
#define _COpcArray_H

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "COpcString.h"

//==============================================================================
// CLASS:   COpcList<TYPE>
// PURPOSE: Defines a indexable array template class.

template<class TYPE>
class COpcArray 
{
    OPC_CLASS_NEW_DELETE_ARRAY()

    public:

    //==========================================================================
    // Constructor
    COpcArray(UINT uSize = 0)
    :
        m_pData(NULL),
        m_uSize(0)
    {
        SetSize(uSize);
    }

    //==========================================================================
    // Copy Constructor
    COpcArray(const COpcArray& cArray)
    :
        m_pData(NULL),
        m_uSize(0)
    {
        *this = cArray;
    }  

    //==========================================================================
    // Destructor
    ~COpcArray()
    {
        RemoveAll();
    }

    //==========================================================================
    // Assignment
    COpcArray& operator=(const COpcArray& cArray)
    {
        SetSize(cArray.m_uSize);

        for (UINT ii = 0; ii < cArray.m_uSize; ii++)
        {
            m_pData[ii] = cArray[ii];
        }

        return *this;
    }

    //==========================================================================
    // GetSize
    UINT GetSize() const
    {
        return m_uSize;
    }

    //==========================================================================
    // GetData
    TYPE* GetData() const
    {
        return m_pData;
    }

    //==========================================================================
    // SetSize
    void SetSize(UINT uNewSize)
    {
        if (uNewSize == 0)
        {
            RemoveAll();
            return;
        }

        TYPE* pData = new TYPE[uNewSize];

        for (UINT ii = 0; ii < uNewSize && ii < m_uSize; ii++)
        {
            pData[ii] = m_pData[ii];
        }

        if (m_pData != NULL)
        {
            delete [] m_pData;
        }

        m_pData = pData;
        m_uSize = uNewSize;
    }

    //==========================================================================
    // RemoveAll	
    void RemoveAll()
    {
        if (m_pData != NULL)
        {
            delete [] m_pData;
        }

        m_uSize = 0;
        m_pData = NULL;
    }

    //==========================================================================
    // operator[]	
    TYPE& operator[](UINT uIndex)
    {
        OPC_ASSERT(uIndex < m_uSize);
        return m_pData[uIndex];
    }

    const TYPE& operator[](UINT uIndex) const
    {
        OPC_ASSERT(uIndex < m_uSize);
        return m_pData[uIndex];
    }

    //==========================================================================
    // SetAtGrow
    void SetAtGrow(UINT uIndex, const TYPE& newElement)
    {
        if (uIndex+1 > m_uSize)
        {
            SetSize(uIndex+1);
        }

        m_pData[uIndex] = newElement;
    }

    //==========================================================================
    // Append
    void Append(const TYPE& newElement)
    {
        SetAtGrow(m_uSize, newElement);
    }

    //==========================================================================
    // InsertAt
    void InsertAt(UINT uIndex, const TYPE& newElement, UINT uCount = 1)
    {
        OPC_ASSERT(uIndex < m_uSize);

        UINT uNewSize = m_uSize+uCount;
        TYPE* pData = new TYPE[uNewSize];

		UINT ii = 0;

        for (ii = 0; ii < uIndex; ii++)
        {
            pData[ii] = m_pData[ii];
        }

        for (ii = uIndex; ii < uCount; ii++)
        {
            pData[ii] = newElement;
        }

        for (ii = uIndex+uCount; ii < uNewSize; ii++)
        {
            pData[ii] = m_pData[ii-uCount];
        }

        delete [] m_pData;
        m_pData = pData;
        m_uSize = uNewSize;
    }

    //==========================================================================
    // RemoveAt
    void RemoveAt(UINT uIndex, UINT uCount = 1)
    {
        OPC_ASSERT(uIndex < m_uSize);

        UINT uNewSize = m_uSize-uCount;
        TYPE* pData = new TYPE[uNewSize];

		UINT ii = 0;

        for (ii = 0; ii < uIndex; ii++)
        {
            pData[ii] = m_pData[ii];
        }

        for (ii = uIndex+uCount; ii < m_uSize; ii++)
        {
            pData[ii-uCount] = m_pData[ii];
        }

        delete [] m_pData;
        m_pData = pData;
        m_uSize = uNewSize;
    }

private:

    TYPE* m_pData;
    UINT  m_uSize;
};

//==============================================================================
// TYPE:    COpcStringArray
// PURPOSE: An array of strings.

typedef COpcArray<COpcString> COpcStringArray;

#ifndef OPCUTILS_EXPORTS
template class OPCUTILS_API COpcArray<COpcString>;
#endif

#endif //ndef _COpcArray_H
