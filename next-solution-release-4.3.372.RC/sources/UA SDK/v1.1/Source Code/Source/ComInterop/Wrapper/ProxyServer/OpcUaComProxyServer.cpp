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
#include "Da/COpcUaDaProxyServer.h"
#include "Ae/COpcUaAe2ProxyServer.h"
#include "Hda/COpcUaHdaProxyServer.h"

#pragma warning(disable:4192)

//================================================================================
// COM Module Declarations

OPC_DECLARE_APPLICATION(OpcUa, OpcUaComProxyServer, "UA COM Proxy Server", TRUE)

OPC_BEGIN_CLASS_TABLE()
    OPC_CLASS_TABLE_ENTRY(COpcUaDaProxyServer, ComDaProxyServer, 1, "UA COM DA Proxy Server")
    OPC_CLASS_TABLE_ENTRY(COpcUaAe2ProxyServer, ComAe2ProxyServer, 1, "UA COM AE Proxy Server")
    // OPC_CLASS_TABLE_ENTRY(COpcUaAeProxyServer, ComAeProxyServer, 1, "UA COM AE Proxy Server")
    OPC_CLASS_TABLE_ENTRY(COpcUaHdaProxyServer, ComHdaProxyServer, 1, "UA COM HDA Proxy Server")
OPC_END_CLASS_TABLE()

OPC_BEGIN_CATEGORY_TABLE()

    OPC_CATEGORY_TABLE_ENTRY(ComDaProxyServer, CATID_OPCDAServer20, OPC_CATEGORY_DESCRIPTION_DA20)

#ifndef OPCUA_NO_DA3_SUPPORT
    OPC_CATEGORY_TABLE_ENTRY(ComDaProxyServer, CATID_OPCDAServer30, OPC_CATEGORY_DESCRIPTION_DA30)
#endif

	// OPC_CATEGORY_TABLE_ENTRY(ComAeProxyServer, CATID_OPCAEServer10, OPC_CATEGORY_DESCRIPTION_AE10)
	OPC_CATEGORY_TABLE_ENTRY(ComAe2ProxyServer, CATID_OPCAEServer10, OPC_CATEGORY_DESCRIPTION_AE10)
    OPC_CATEGORY_TABLE_ENTRY(ComHdaProxyServer, CATID_OPCHDAServer10, OPC_CATEGORY_DESCRIPTION_HDA10)

OPC_END_CATEGORY_TABLE()

#ifndef _USRDLL

// {037D6665-27F3-462f-9E8F-F8C146D28669}
OPC_IMPLEMENT_LOCAL_SERVER(0x37d6665, 0x27f3, 0x462f, 0x9e, 0x8f, 0xf8, 0xc1, 0x46, 0xd2, 0x86, 0x69);

//================================================================================
// WinMain

extern "C" int WINAPI _tWinMain(
    HINSTANCE hInstance, 
	HINSTANCE hPrevInstance, 
    LPTSTR    lpCmdLine, 
    int       nShowCmd
)
{
    OPC_START_LOCAL_SERVER_EX(hInstance, lpCmdLine);
    return 0;
}

#else

OPC_IMPLEMENT_INPROC_SERVER();

//==============================================================================
// DllMain

extern "C"
BOOL WINAPI DllMain( 
    HINSTANCE hModule, 
    DWORD     dwReason, 
    LPVOID    lpReserved)
{
    OPC_START_INPROC_SERVER(hModule, dwReason);
    return TRUE;
}

#endif // _USRDLL
