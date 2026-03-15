//==============================================================================
// TITLE: COpcHdaTime.h
//
// CONTENTS:
// 
// Functions used to resolve relative times as defined in the HDA specification.
//
// (c) Copyright 2003-2004 The OPC Foundation
// ALL RIGHTS RESERVED.
//
// DISCLAIMER:
//  This code is provided by the OPC Foundation solely to assist in 
//  understanding and use of the appropriate OPC Specification(s) and may be 
//  used as set forth in the License Grant section of the OPC Specification.
//  This code is provided as-is and without warranty or support of any sort
//  and is subject to the Warranty and Liability Disclaimers which appear
//  in the printed OPC Specification.
//
// MODIFICATION LOG:
//
// Date       By    Notes
// ---------- ---   -----
// 2003/12/17 RSA   Initial implementation.

#ifndef _COpcHdaTime_H_
#define _COpcHdaTime_H_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

//==============================================================================
// FUNCTION: OpcHdaResolveTime
// PURPOSE:  Converts a relative time to an absolute UTC time.

LONGLONG OpcHdaResolveTime(OPCHDA_TIME& cTime);

#endif // _COpcHdaTime_H_
