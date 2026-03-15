 /**
 * <interfacename>CmpGwClient</interfacename>
 * <description> 
 *	Interface of the gateway client as an entry point in the PLC network.
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPGWCLIENTITF_H_
#define _CMPGWCLIENTITF_H_

#include "CmpStd.h"

 

 




#include "CmpCommunicationLibItf.h"

#ifdef __cplusplus
	extern "C" {
#endif 


/**
* <category>Settings</category>
* <type>Int</type>
* <description>
*	Setting to modify the default value of the Gateway inactivity timeout in ms.
*	This default value is only used, if the Gateway connect is started by BeginConnectToGateway() instead of BeginConnectToGateway2().
*	If no piece of data (also no keep alive) is received or sent within this time, the GwClient closes the connection to the Gateway.
* </description>
*/
#define GWCLIENTKEY_INT_INACTIVITY_TIMEOUT					"InactivityTimeoutMs"
#ifndef GWCLIENTKEY_INT_INACTIVITY_TIMEOUT_DEFAULT
	#define GWCLIENTKEY_INT_INACTIVITY_TIMEOUT_DEFAULT		30000
#endif


enum 
{
	PT_CHAR = 17,  /* char */
	PT_BYTE = 18,	/* unsigned char */
	PT_INT16 = 19,
	PT_UINT16 = 20,
	PT_INT32 = 21,
	PT_UINT32 = 22,
	PT_INT64 = 23,
	PT_UINT64 = 24,  
	PT_WCHAR = 25,	 /* RTS_WCHAR (2-byte UNICODE char) */
	PT_FLOAT = 26,   /* "float" (32 bit floating point value) */
	PT_DOUBLE = 27,  /* "double" (64 bit floating point value) */
	PT_BOOLEAN = 28, /* an int which is interpreted as a boolean value (0 or != 0) */
	PT_STRING = 29,
	PT_WSTRING = 30
};

typedef struct
{
	ADDRESSCOMPONENT *pAddress;
	RTS_UI16 nLength;
}NODEADDRESS_OLD;

#define MAX_CHANNEL_NAME 19

typedef struct 
{
	RTS_UI32 dwCommBuffer;
	RTS_WCHAR wszName[MAX_CHANNEL_NAME + 1];
}CHANNELQOS;


#define MAX_PARAM_NAME 19

/* This structure is used to describe a parameter */
typedef struct
{
	RTS_WCHAR pwszName[MAX_PARAM_NAME+1];
	RTS_UI32 dwParamId;
	RTS_UI32 dwType; /* PT_xxx */
	void *pDefaultValue; /* points to the default value of this parameter. May be null */
}PARAMETERDEFINITION;

/* This structure is used to pass a parameter.
   pValue is a pointer to the value of the parameter
   ( e.g. a pointer to an int (PT_INT) or a pointer to a wchar-string (PT_WSTRING)).
*/
typedef struct
{
	RTS_UI32 dwParamId;
	RTS_UI32 type;
	void * pValue; /* Pointer to the value */
}PARAMETER;

typedef struct
{
	RTS_I32 nNumParams;
	PARAMETERDEFINITION *pParam; /* An array of nNumParams parameter definitions*/
}PARAMDEFLIST;

typedef struct
{
	RTS_I32 nNumParams;
	PARAMETER *pParam; /* An array of nNumParams parameters */
}PARAMLIST;

/* Describe a communication driver */
typedef struct
{
	RTS_UI32 dwStructLength;	/* Must be set to the size of this structure */
	RTS_HANDLE hDriver;			/* Identifies the driver during application execution. May change in different instances of an application */
	RTS_GUID guid;              /* Use to identify a well known communication driver */
	RTS_WCHAR *pwszName;
	PARAMDEFLIST params;       /* E.g. a TCP/IP driver uses 2 parameters: IP-address and port */
	RTS_I32 nMinPingInterval;  /* The number of seconds to wait, between pinging a gateway using this driver. Set to 0 to disable polling at all.*/
}COMMDRVINFO;

/* 
  Application provided function to be called when an asynchronous operation
  completes. pAsyncRes may not point to the same object as provided in the
  BeginXXX function, but is an exact copy of this object 
*/
struct tagASYNCRESULT;
typedef void (STDCALL *PFASYNCCALLBACK)(struct tagASYNCRESULT *pAsyncRes);

/* Use this structure to call one of the BeginXXX functions asynchronously. 
   Passing a NULL pointer for this structure will result in a synchronous call.
   When using this structure the application has to provide the pUser and pfCallback
   field, the others are set by the asynchronous function. An application may set
   both of those - pUser and pfCallback - to NULL. In the case of pfCallback 
   no callback will be triggered when the operation completes. pUser is not 
   interpreted by the asynchronous function anyways, it simply serves as an 
   identifier of the call for the application.
*/
typedef struct tagASYNCRESULT
{
	void *pUser; 
		/* Application supplied value to identify the asynchronous call.*/
	PFASYNCCALLBACK pfCallback;
		/* Application supplied function to be called when the asynchronous call completes.*/
	RTS_HANDLE hEvent;
		/* This event will be set when the asynchronous call completes. 
		   Is set by the BeginXXX function
		*/
	RTS_UI32 ulRequestId;
		/* Internally used. To be ignored by the application */
}ASYNCRESULT;

/* -- Functions exported by communication drivers to the Gateway-Client -- */

/* Send data on a connection. If not all of the data could be sent, the number of bytes 
   sent is returned in pdwSent and the function has to be called again later with the remaining
   bytes. 
   NOTE: This function must not block and it must not call any functions of the 
         gateway, since this may lead to deadlocks.
   hConnHandle    IN  identifies the connection.
   data           IN  The data to be sent
   pdwSent        OUT The number of bytes the could actually be sent.
*/
typedef RTS_RESULT (CDECL *PFCOMMDRVSEND)(RTS_HANDLE hConnHandle, PROTOCOL_DATA_UNIT data, RTS_UI32 *pdwSent);

/* Receive data from an existent connection. 
   NOTE: This function must not block and it must not call any functions of the 
         gateway, since this may lead to deadlocks.
   hConnHandle  IN      identifies the connection
   pData        OUT  The data to be received.
   dwReceive    IN   Number of bytes to receive.
*/
typedef RTS_RESULT (CDECL *PFCOMMDRVRECEIVE)(RTS_HANDLE hConnHandle, PROTOCOL_DATA_UNIT *pData, RTS_UI32 dwReceive);

/* Open a connection as described by pParams
   pParams        IN  
   phConnHandle   OUT    If successful, phConnHandle is set to the handle for the connection.
   pAsyncRes      IN/OUT Asynchronous result.
 */
typedef RTS_RESULT (CDECL *PFCOMMDRVBEGINCONNECT)( PARAMLIST *pParams, RTS_HANDLE *phConnHandle, ASYNCRESULT *pAsyncRes);

/* Wait until the BeginConnect call identified by pAsyncRes has completed. 
   pAsyncRes     IN   Identifies the matching BeginConnect call.
   phConnHandle  OUT  If the call was successful, then this parameter is set to the 
                      handle for the newly created connection.
*/
typedef RTS_RESULT (CDECL *PFCOMMDRVENDCONNECT) ( ASYNCRESULT *pAsyncRes, RTS_HANDLE *phConnHandle);

/* Close a connection 
   NOTE: This function must not block and it must not call any functions of the 
         gateway client, since this may lead to deadlocks.
*/
typedef RTS_RESULT (CDECL *PFCOMMDRVCLOSE)(RTS_HANDLE hConnHandle);

typedef struct 
{
	PFCOMMDRVBEGINCONNECT   pfBeginConnect;
	PFCOMMDRVENDCONNECT     pfEndConnect;
	PFCOMMDRVSEND           pfSend;
	PFCOMMDRVRECEIVE        pfReceive;
	PFCOMMDRVCLOSE          pfClose;
}COMMDRVITF;

/* ByteOrders of the answering node used only for GWCLIENTNODEINFO.byIntelByteOrder. */
#define GWCLIENT_NI_BYTEORDER_MOTOROLA		0
#define GWCLIENT_NI_BYTEORDER_INTEL			1
#define GWCLIENT_NI_BYTEORDER_UNKNOWN		255

typedef struct
{
	RTS_UI32	dwRtsMaxChannels;		/* Number of communication channels */
	RTS_UI32	dwTargetType;			/* Target type, e. g. SYSTARGET_TYPE_PROGRAMMABLE */
	RTS_UI32	dwTargetId;				/* Target id, consists of SYSTARGET_VENDOR_ID and SYSTARGET_DEVICE_ID */
	RTS_UI32	dwTargetVersion;		/* (OEM) version */
	RTS_UI32	dwFlags;				/* See name service package flags NSPF_ - unused flags have to be set to 0 */
	RTS_UI8		byIntelByteOrder;		/* Byte order: 0="Motorola", 1="Intel", 255=Unknown, see defines GWCLIENT_NI_BYTEORDER_xxx */
	RTS_UI8		byByte;					/* Reserved, always 0 */
	RTS_UI16	wRtsBlkDrvType;			/* Type of block driver instance on runtime on which this node info was sent, e.g. RTS_BLK_DRV_TYPE_UDP */
	RTS_UI16	wGwBlkDrvType;			/* Type of block driver instance on Gateway on which this node info was received, e.g. RTS_BLK_DRV_TYPE_UDP */
	RTS_UI16	wGwRtsNsVersion;		/* Name service version used between runtime system and Gateway */
	RTS_UI32	dwGwClientGwVersion;	/* Name service version used between GwClient and Gateway */
}GWCLIENTNODEINFO;

/* -- Functions exported by the Gateway-Client to the communication drivers -- */

/**
 * <description>
 *  Register a commdriver with the gateway client. 
 * </description>
 * <param name="pItf" type="IN">The interface functions of the comm driver.</param>
 * <param name="pDrvInfo" type="IN">
 *  Information about the driver. The hDriver field is unused in this call.
 * </param>
 * <param name="phDriverHandle" type="OUT">
 *  Will be set with the handle assigned to the communication-driver.
 *  The driver must use this handle in all callbacks (eg. GWClientConnectionReady).
 * </param>
*/
RTS_RESULT CDECL GWClientRegisterCommDrv(COMMDRVITF *pItf, COMMDRVINFO *pDrvInfo, RTS_HANDLE *phDriverHandle);
typedef RTS_RESULT (CDECL * PFGWCLIENTREGISTERCOMMDRV) (COMMDRVITF *pItf, COMMDRVINFO *pDrvInfo, RTS_HANDLE *phDriverHandle);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTREGISTERCOMMDRV_NOTIMPLEMENTED)
	#define USE_GWClientRegisterCommDrv
	#define EXT_GWClientRegisterCommDrv
	#define GET_GWClientRegisterCommDrv(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientRegisterCommDrv(p0,p1,p2) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientRegisterCommDrv  FALSE
	#define EXP_GWClientRegisterCommDrv  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientRegisterCommDrv
	#define EXT_GWClientRegisterCommDrv
	#define GET_GWClientRegisterCommDrv(fl)  CAL_CMGETAPI( "GWClientRegisterCommDrv" ) 
	#define CAL_GWClientRegisterCommDrv  GWClientRegisterCommDrv
	#define CHK_GWClientRegisterCommDrv  TRUE
	#define EXP_GWClientRegisterCommDrv  CAL_CMEXPAPI( "GWClientRegisterCommDrv" ) 
	#define EXTREF_ITF_GWClientRegisterCommDrv {(RTS_VOID_FCTPTR)GWClientRegisterCommDrv, "GWClientRegisterCommDrv", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientRegisterCommDrv
	#define EXT_GWClientRegisterCommDrv
	#define GET_GWClientRegisterCommDrv(fl)  CAL_CMGETAPI( "GWClientRegisterCommDrv" ) 
	#define CAL_GWClientRegisterCommDrv  GWClientRegisterCommDrv
	#define CHK_GWClientRegisterCommDrv  TRUE
	#define EXP_GWClientRegisterCommDrv  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientRegisterCommDrv", (RTS_UINTPTR)GWClientRegisterCommDrv, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientRegisterCommDrv
	#define EXT_CmpGwClientGWClientRegisterCommDrv
	#define GET_CmpGwClientGWClientRegisterCommDrv  ERR_OK
	#define CAL_CmpGwClientGWClientRegisterCommDrv  GWClientRegisterCommDrv
	#define CHK_CmpGwClientGWClientRegisterCommDrv  TRUE
	#define EXP_CmpGwClientGWClientRegisterCommDrv  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientRegisterCommDrv
	#define EXT_GWClientRegisterCommDrv
	#define GET_GWClientRegisterCommDrv(fl)  CAL_CMGETAPI( "GWClientRegisterCommDrv" ) 
	#define CAL_GWClientRegisterCommDrv  GWClientRegisterCommDrv
	#define CHK_GWClientRegisterCommDrv  TRUE
	#define EXP_GWClientRegisterCommDrv  CAL_CMEXPAPI( "GWClientRegisterCommDrv" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientRegisterCommDrv  PFGWCLIENTREGISTERCOMMDRV pfGWClientRegisterCommDrv;
	#define EXT_GWClientRegisterCommDrv  extern PFGWCLIENTREGISTERCOMMDRV pfGWClientRegisterCommDrv;
	#define GET_GWClientRegisterCommDrv(fl)  s_pfCMGetAPI2( "GWClientRegisterCommDrv", (RTS_VOID_FCTPTR *)&pfGWClientRegisterCommDrv, (fl), 0, 0)
	#define CAL_GWClientRegisterCommDrv  pfGWClientRegisterCommDrv
	#define CHK_GWClientRegisterCommDrv  (pfGWClientRegisterCommDrv != NULL)
	#define EXP_GWClientRegisterCommDrv   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientRegisterCommDrv", (RTS_UINTPTR)GWClientRegisterCommDrv, 0, 0) 
#endif


enum
{
	COMMDRV_ACTION_SEND = 0,
	COMMDRV_ACTION_RECEIVE = 1
};

/**
 * <description>
 *  May be called by a commdriver if a connection is ready to send/receive.
 *  This function shall not signal a condition more then once without the 
 *  gateway client reacting to that signal.
 *  E.g. if the commdriver signals that a connection is ready to send it shall not
 *  signal this situation again before the gateway client actually called "send".
 *  The gwclient must not rely on this function to be called but may use 
 *  it to increase the performance of the connection.
 * </description>
 * <param name="hDriverHandle" type="IN">
 *  The driver handle returned by GWClientRegisterCommDrv.
 * </param>
 * <param name="hConnHandle" type="IN">
 *  The handle to the connection.
 * </param>
 * <param name="nAction" type="IN">
 *  The action that is ready on this connection - eg. COMMDRV_ACTION_SEND, COMMDRV_ACTION_RECEIVE
 * </param>
 */
RTS_RESULT CDECL GWClientConnectionReady(RTS_HANDLE hDriverHandle, RTS_HANDLE hConnHandle, RTS_I32 nAction);
typedef RTS_RESULT (CDECL * PFGWCLIENTCONNECTIONREADY) (RTS_HANDLE hDriverHandle, RTS_HANDLE hConnHandle, RTS_I32 nAction);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTCONNECTIONREADY_NOTIMPLEMENTED)
	#define USE_GWClientConnectionReady
	#define EXT_GWClientConnectionReady
	#define GET_GWClientConnectionReady(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientConnectionReady(p0,p1,p2) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientConnectionReady  FALSE
	#define EXP_GWClientConnectionReady  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientConnectionReady
	#define EXT_GWClientConnectionReady
	#define GET_GWClientConnectionReady(fl)  CAL_CMGETAPI( "GWClientConnectionReady" ) 
	#define CAL_GWClientConnectionReady  GWClientConnectionReady
	#define CHK_GWClientConnectionReady  TRUE
	#define EXP_GWClientConnectionReady  CAL_CMEXPAPI( "GWClientConnectionReady" ) 
	#define EXTREF_ITF_GWClientConnectionReady {(RTS_VOID_FCTPTR)GWClientConnectionReady, "GWClientConnectionReady", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientConnectionReady
	#define EXT_GWClientConnectionReady
	#define GET_GWClientConnectionReady(fl)  CAL_CMGETAPI( "GWClientConnectionReady" ) 
	#define CAL_GWClientConnectionReady  GWClientConnectionReady
	#define CHK_GWClientConnectionReady  TRUE
	#define EXP_GWClientConnectionReady  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientConnectionReady", (RTS_UINTPTR)GWClientConnectionReady, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientConnectionReady
	#define EXT_CmpGwClientGWClientConnectionReady
	#define GET_CmpGwClientGWClientConnectionReady  ERR_OK
	#define CAL_CmpGwClientGWClientConnectionReady  GWClientConnectionReady
	#define CHK_CmpGwClientGWClientConnectionReady  TRUE
	#define EXP_CmpGwClientGWClientConnectionReady  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientConnectionReady
	#define EXT_GWClientConnectionReady
	#define GET_GWClientConnectionReady(fl)  CAL_CMGETAPI( "GWClientConnectionReady" ) 
	#define CAL_GWClientConnectionReady  GWClientConnectionReady
	#define CHK_GWClientConnectionReady  TRUE
	#define EXP_GWClientConnectionReady  CAL_CMEXPAPI( "GWClientConnectionReady" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientConnectionReady  PFGWCLIENTCONNECTIONREADY pfGWClientConnectionReady;
	#define EXT_GWClientConnectionReady  extern PFGWCLIENTCONNECTIONREADY pfGWClientConnectionReady;
	#define GET_GWClientConnectionReady(fl)  s_pfCMGetAPI2( "GWClientConnectionReady", (RTS_VOID_FCTPTR *)&pfGWClientConnectionReady, (fl), 0, 0)
	#define CAL_GWClientConnectionReady  pfGWClientConnectionReady
	#define CHK_GWClientConnectionReady  (pfGWClientConnectionReady != NULL)
	#define EXP_GWClientConnectionReady   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientConnectionReady", (RTS_UINTPTR)GWClientConnectionReady, 0, 0) 
#endif


typedef void (STDCALL *PFENUMCOMMDRVCALLBACK)(RTS_HANDLE hDriver, RTS_GUID *guid, RTS_WCHAR *pwszName, PARAMDEFLIST *pParams, RTS_I8 bLast, RTS_I32 nMinPingInterval);
typedef void (STDCALL *PFENUMCOMMDRVCALLBACK2)(RTS_UINTPTR dwUser, RTS_HANDLE hDriver, RTS_GUID *guid, RTS_WCHAR *pwszName, PARAMDEFLIST *pParams, RTS_I8 bLast, RTS_I32 nMinPingInterval);

/* This callback is called by the name service functions (e.g. ResolveAllNodes) for each 
   answering node.
   dwUser       IN  user defined value simply passed to this function
   addrNode     IN  The address of the answering node
   addrParent   IN  The address of its parent node (length zero if node is top level)
   nMaxChannels IN  Maximum number of channels that the runtime system is able to deal with.
   dwTargetId   IN  Target identifier
   wszNodeName  IN  The name of the node.
   wszTargetName IN The description of the target
*/
typedef void (STDCALL *PFNODEINFOCALLBACK)(RTS_UINTPTR dwUser, NODEADDRESS_OLD addrNode, NODEADDRESS_OLD addrParent, RTS_I32 nMaxChannels, RTS_UI32 dwTargetId, RTS_WCHAR *wszNodeName, RTS_WCHAR *wszTargetName);

/* This callback is called by the name service functions (e.g. ResolveAllNodes) for each 
   answering node.
   dwUser       IN  user defined value simply passed to this function
   addrNode     IN  The address of the answering node
   addrParent   IN  The address of its parent node (length zero if node is top level)
   nMaxChannels IN  Maximum number of channels that the runtime system is able to deal with.
   dwTargetType IN  Target type
   dwTargetId	IN  Target identification number
   dwTargetVersion IN  Target version
   wszNodeName  IN  The name of the node.
   wszDeviceName IN The name of the device
   wszVendorName IN The name of the vendor
*/
typedef void (STDCALL *PFNODEINFOCALLBACK2)(RTS_UINTPTR dwUser, NODEADDRESS_OLD addrNode, NODEADDRESS_OLD addrParent, RTS_I32 nMaxChannels, RTS_UI32 dwTargetType, RTS_UI32 dwTargetId, RTS_UI32 dwTargetVersion, RTS_WCHAR *wszNodeName, RTS_WCHAR *wszDeviceName, RTS_WCHAR *wszVendorName);

/* This callback is called by the name service functions (e.g. ResolveAllNodes) for each
answering node.
dwUser				IN  User defined value simply passed to this function
addrNode			IN  The address of the answering node
addrParent			IN  The address of its parent node (length zero if node is top level)
GWCLIENTNODEINFO	IN  Detailed node information 
wszNodeName			IN  Node name
wszDeviceName		IN  Device name
wszVendorName;		IN  Vendor name, may be an empty string
pszSerialNumber		IN  Serial number, may be an empty string
pbyOemData;			IN  OEM data; may be NULL, if dwOemDataLength==0
dwOemDataLength		IN  Length of OEM specific data
*/
typedef void (STDCALL *PFNODEINFOCALLBACK3)(RTS_UINTPTR dwUser, NODEADDRESS_OLD addrNode, NODEADDRESS_OLD addrParent, GWCLIENTNODEINFO *pGwClientNodeInfo, RTS_WCHAR *wszNodeName, RTS_WCHAR *wszDeviceName, RTS_WCHAR *wszVendorName, RTS_CSTRING *pszSerialNumber, RTS_UI8 *pbyOemData, RTS_UI32 dwOemDataLength);


RTS_RESULT CDECL GWClientGetInterfaceVersion(RTS_UI16 *pwMajor, RTS_UI16 *pwMinor);
typedef RTS_RESULT (CDECL * PFGWCLIENTGETINTERFACEVERSION) (RTS_UI16 *pwMajor, RTS_UI16 *pwMinor);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTGETINTERFACEVERSION_NOTIMPLEMENTED)
	#define USE_GWClientGetInterfaceVersion
	#define EXT_GWClientGetInterfaceVersion
	#define GET_GWClientGetInterfaceVersion(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientGetInterfaceVersion(p0,p1) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientGetInterfaceVersion  FALSE
	#define EXP_GWClientGetInterfaceVersion  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientGetInterfaceVersion
	#define EXT_GWClientGetInterfaceVersion
	#define GET_GWClientGetInterfaceVersion(fl)  CAL_CMGETAPI( "GWClientGetInterfaceVersion" ) 
	#define CAL_GWClientGetInterfaceVersion  GWClientGetInterfaceVersion
	#define CHK_GWClientGetInterfaceVersion  TRUE
	#define EXP_GWClientGetInterfaceVersion  CAL_CMEXPAPI( "GWClientGetInterfaceVersion" ) 
	#define EXTREF_ITF_GWClientGetInterfaceVersion {(RTS_VOID_FCTPTR)GWClientGetInterfaceVersion, "GWClientGetInterfaceVersion", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientGetInterfaceVersion
	#define EXT_GWClientGetInterfaceVersion
	#define GET_GWClientGetInterfaceVersion(fl)  CAL_CMGETAPI( "GWClientGetInterfaceVersion" ) 
	#define CAL_GWClientGetInterfaceVersion  GWClientGetInterfaceVersion
	#define CHK_GWClientGetInterfaceVersion  TRUE
	#define EXP_GWClientGetInterfaceVersion  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientGetInterfaceVersion", (RTS_UINTPTR)GWClientGetInterfaceVersion, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientGetInterfaceVersion
	#define EXT_CmpGwClientGWClientGetInterfaceVersion
	#define GET_CmpGwClientGWClientGetInterfaceVersion  ERR_OK
	#define CAL_CmpGwClientGWClientGetInterfaceVersion  GWClientGetInterfaceVersion
	#define CHK_CmpGwClientGWClientGetInterfaceVersion  TRUE
	#define EXP_CmpGwClientGWClientGetInterfaceVersion  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientGetInterfaceVersion
	#define EXT_GWClientGetInterfaceVersion
	#define GET_GWClientGetInterfaceVersion(fl)  CAL_CMGETAPI( "GWClientGetInterfaceVersion" ) 
	#define CAL_GWClientGetInterfaceVersion  GWClientGetInterfaceVersion
	#define CHK_GWClientGetInterfaceVersion  TRUE
	#define EXP_GWClientGetInterfaceVersion  CAL_CMEXPAPI( "GWClientGetInterfaceVersion" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientGetInterfaceVersion  PFGWCLIENTGETINTERFACEVERSION pfGWClientGetInterfaceVersion;
	#define EXT_GWClientGetInterfaceVersion  extern PFGWCLIENTGETINTERFACEVERSION pfGWClientGetInterfaceVersion;
	#define GET_GWClientGetInterfaceVersion(fl)  s_pfCMGetAPI2( "GWClientGetInterfaceVersion", (RTS_VOID_FCTPTR *)&pfGWClientGetInterfaceVersion, (fl), 0, 0)
	#define CAL_GWClientGetInterfaceVersion  pfGWClientGetInterfaceVersion
	#define CHK_GWClientGetInterfaceVersion  (pfGWClientGetInterfaceVersion != NULL)
	#define EXP_GWClientGetInterfaceVersion   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientGetInterfaceVersion", (RTS_UINTPTR)GWClientGetInterfaceVersion, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEnumCommDrivers(PFENUMCOMMDRVCALLBACK pfCallback);
typedef RTS_RESULT (CDECL * PFGWCLIENTENUMCOMMDRIVERS) (PFENUMCOMMDRVCALLBACK pfCallback);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENUMCOMMDRIVERS_NOTIMPLEMENTED)
	#define USE_GWClientEnumCommDrivers
	#define EXT_GWClientEnumCommDrivers
	#define GET_GWClientEnumCommDrivers(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEnumCommDrivers(p0) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEnumCommDrivers  FALSE
	#define EXP_GWClientEnumCommDrivers  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEnumCommDrivers
	#define EXT_GWClientEnumCommDrivers
	#define GET_GWClientEnumCommDrivers(fl)  CAL_CMGETAPI( "GWClientEnumCommDrivers" ) 
	#define CAL_GWClientEnumCommDrivers  GWClientEnumCommDrivers
	#define CHK_GWClientEnumCommDrivers  TRUE
	#define EXP_GWClientEnumCommDrivers  CAL_CMEXPAPI( "GWClientEnumCommDrivers" ) 
	#define EXTREF_ITF_GWClientEnumCommDrivers {(RTS_VOID_FCTPTR)GWClientEnumCommDrivers, "GWClientEnumCommDrivers", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEnumCommDrivers
	#define EXT_GWClientEnumCommDrivers
	#define GET_GWClientEnumCommDrivers(fl)  CAL_CMGETAPI( "GWClientEnumCommDrivers" ) 
	#define CAL_GWClientEnumCommDrivers  GWClientEnumCommDrivers
	#define CHK_GWClientEnumCommDrivers  TRUE
	#define EXP_GWClientEnumCommDrivers  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEnumCommDrivers", (RTS_UINTPTR)GWClientEnumCommDrivers, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEnumCommDrivers
	#define EXT_CmpGwClientGWClientEnumCommDrivers
	#define GET_CmpGwClientGWClientEnumCommDrivers  ERR_OK
	#define CAL_CmpGwClientGWClientEnumCommDrivers  GWClientEnumCommDrivers
	#define CHK_CmpGwClientGWClientEnumCommDrivers  TRUE
	#define EXP_CmpGwClientGWClientEnumCommDrivers  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEnumCommDrivers
	#define EXT_GWClientEnumCommDrivers
	#define GET_GWClientEnumCommDrivers(fl)  CAL_CMGETAPI( "GWClientEnumCommDrivers" ) 
	#define CAL_GWClientEnumCommDrivers  GWClientEnumCommDrivers
	#define CHK_GWClientEnumCommDrivers  TRUE
	#define EXP_GWClientEnumCommDrivers  CAL_CMEXPAPI( "GWClientEnumCommDrivers" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEnumCommDrivers  PFGWCLIENTENUMCOMMDRIVERS pfGWClientEnumCommDrivers;
	#define EXT_GWClientEnumCommDrivers  extern PFGWCLIENTENUMCOMMDRIVERS pfGWClientEnumCommDrivers;
	#define GET_GWClientEnumCommDrivers(fl)  s_pfCMGetAPI2( "GWClientEnumCommDrivers", (RTS_VOID_FCTPTR *)&pfGWClientEnumCommDrivers, (fl), 0, 0)
	#define CAL_GWClientEnumCommDrivers  pfGWClientEnumCommDrivers
	#define CHK_GWClientEnumCommDrivers  (pfGWClientEnumCommDrivers != NULL)
	#define EXP_GWClientEnumCommDrivers   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEnumCommDrivers", (RTS_UINTPTR)GWClientEnumCommDrivers, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEnumCommDrivers2(PFENUMCOMMDRVCALLBACK2 pfCallback, RTS_UINTPTR dwUser);
typedef RTS_RESULT (CDECL * PFGWCLIENTENUMCOMMDRIVERS2) (PFENUMCOMMDRVCALLBACK2 pfCallback, RTS_UINTPTR dwUser);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENUMCOMMDRIVERS2_NOTIMPLEMENTED)
	#define USE_GWClientEnumCommDrivers2
	#define EXT_GWClientEnumCommDrivers2
	#define GET_GWClientEnumCommDrivers2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEnumCommDrivers2(p0,p1) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEnumCommDrivers2  FALSE
	#define EXP_GWClientEnumCommDrivers2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEnumCommDrivers2
	#define EXT_GWClientEnumCommDrivers2
	#define GET_GWClientEnumCommDrivers2(fl)  CAL_CMGETAPI( "GWClientEnumCommDrivers2" ) 
	#define CAL_GWClientEnumCommDrivers2  GWClientEnumCommDrivers2
	#define CHK_GWClientEnumCommDrivers2  TRUE
	#define EXP_GWClientEnumCommDrivers2  CAL_CMEXPAPI( "GWClientEnumCommDrivers2" ) 
	#define EXTREF_ITF_GWClientEnumCommDrivers2 {(RTS_VOID_FCTPTR)GWClientEnumCommDrivers2, "GWClientEnumCommDrivers2", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEnumCommDrivers2
	#define EXT_GWClientEnumCommDrivers2
	#define GET_GWClientEnumCommDrivers2(fl)  CAL_CMGETAPI( "GWClientEnumCommDrivers2" ) 
	#define CAL_GWClientEnumCommDrivers2  GWClientEnumCommDrivers2
	#define CHK_GWClientEnumCommDrivers2  TRUE
	#define EXP_GWClientEnumCommDrivers2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEnumCommDrivers2", (RTS_UINTPTR)GWClientEnumCommDrivers2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEnumCommDrivers2
	#define EXT_CmpGwClientGWClientEnumCommDrivers2
	#define GET_CmpGwClientGWClientEnumCommDrivers2  ERR_OK
	#define CAL_CmpGwClientGWClientEnumCommDrivers2  GWClientEnumCommDrivers2
	#define CHK_CmpGwClientGWClientEnumCommDrivers2  TRUE
	#define EXP_CmpGwClientGWClientEnumCommDrivers2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEnumCommDrivers2
	#define EXT_GWClientEnumCommDrivers2
	#define GET_GWClientEnumCommDrivers2(fl)  CAL_CMGETAPI( "GWClientEnumCommDrivers2" ) 
	#define CAL_GWClientEnumCommDrivers2  GWClientEnumCommDrivers2
	#define CHK_GWClientEnumCommDrivers2  TRUE
	#define EXP_GWClientEnumCommDrivers2  CAL_CMEXPAPI( "GWClientEnumCommDrivers2" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEnumCommDrivers2  PFGWCLIENTENUMCOMMDRIVERS2 pfGWClientEnumCommDrivers2;
	#define EXT_GWClientEnumCommDrivers2  extern PFGWCLIENTENUMCOMMDRIVERS2 pfGWClientEnumCommDrivers2;
	#define GET_GWClientEnumCommDrivers2(fl)  s_pfCMGetAPI2( "GWClientEnumCommDrivers2", (RTS_VOID_FCTPTR *)&pfGWClientEnumCommDrivers2, (fl), 0, 0)
	#define CAL_GWClientEnumCommDrivers2  pfGWClientEnumCommDrivers2
	#define CHK_GWClientEnumCommDrivers2  (pfGWClientEnumCommDrivers2 != NULL)
	#define EXP_GWClientEnumCommDrivers2   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEnumCommDrivers2", (RTS_UINTPTR)GWClientEnumCommDrivers2, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginConnectToGateway(RTS_HANDLE hDriver, RTS_I32 nNumParams, PARAMETER *pParams, RTS_HANDLE *phGateway, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINCONNECTTOGATEWAY) (RTS_HANDLE hDriver, RTS_I32 nNumParams, PARAMETER *pParams, RTS_HANDLE *phGateway, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINCONNECTTOGATEWAY_NOTIMPLEMENTED)
	#define USE_GWClientBeginConnectToGateway
	#define EXT_GWClientBeginConnectToGateway
	#define GET_GWClientBeginConnectToGateway(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginConnectToGateway(p0,p1,p2,p3,p4) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginConnectToGateway  FALSE
	#define EXP_GWClientBeginConnectToGateway  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginConnectToGateway
	#define EXT_GWClientBeginConnectToGateway
	#define GET_GWClientBeginConnectToGateway(fl)  CAL_CMGETAPI( "GWClientBeginConnectToGateway" ) 
	#define CAL_GWClientBeginConnectToGateway  GWClientBeginConnectToGateway
	#define CHK_GWClientBeginConnectToGateway  TRUE
	#define EXP_GWClientBeginConnectToGateway  CAL_CMEXPAPI( "GWClientBeginConnectToGateway" ) 
	#define EXTREF_ITF_GWClientBeginConnectToGateway {(RTS_VOID_FCTPTR)GWClientBeginConnectToGateway, "GWClientBeginConnectToGateway", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginConnectToGateway
	#define EXT_GWClientBeginConnectToGateway
	#define GET_GWClientBeginConnectToGateway(fl)  CAL_CMGETAPI( "GWClientBeginConnectToGateway" ) 
	#define CAL_GWClientBeginConnectToGateway  GWClientBeginConnectToGateway
	#define CHK_GWClientBeginConnectToGateway  TRUE
	#define EXP_GWClientBeginConnectToGateway  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginConnectToGateway", (RTS_UINTPTR)GWClientBeginConnectToGateway, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginConnectToGateway
	#define EXT_CmpGwClientGWClientBeginConnectToGateway
	#define GET_CmpGwClientGWClientBeginConnectToGateway  ERR_OK
	#define CAL_CmpGwClientGWClientBeginConnectToGateway  GWClientBeginConnectToGateway
	#define CHK_CmpGwClientGWClientBeginConnectToGateway  TRUE
	#define EXP_CmpGwClientGWClientBeginConnectToGateway  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginConnectToGateway
	#define EXT_GWClientBeginConnectToGateway
	#define GET_GWClientBeginConnectToGateway(fl)  CAL_CMGETAPI( "GWClientBeginConnectToGateway" ) 
	#define CAL_GWClientBeginConnectToGateway  GWClientBeginConnectToGateway
	#define CHK_GWClientBeginConnectToGateway  TRUE
	#define EXP_GWClientBeginConnectToGateway  CAL_CMEXPAPI( "GWClientBeginConnectToGateway" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginConnectToGateway  PFGWCLIENTBEGINCONNECTTOGATEWAY pfGWClientBeginConnectToGateway;
	#define EXT_GWClientBeginConnectToGateway  extern PFGWCLIENTBEGINCONNECTTOGATEWAY pfGWClientBeginConnectToGateway;
	#define GET_GWClientBeginConnectToGateway(fl)  s_pfCMGetAPI2( "GWClientBeginConnectToGateway", (RTS_VOID_FCTPTR *)&pfGWClientBeginConnectToGateway, (fl), 0, 0)
	#define CAL_GWClientBeginConnectToGateway  pfGWClientBeginConnectToGateway
	#define CHK_GWClientBeginConnectToGateway  (pfGWClientBeginConnectToGateway != NULL)
	#define EXP_GWClientBeginConnectToGateway   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginConnectToGateway", (RTS_UINTPTR)GWClientBeginConnectToGateway, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginConnectToGateway2(RTS_HANDLE hDriver, RTS_I32 nNumParams, PARAMETER *pParams, RTS_UI32 ui32GwInactivityTimeout, RTS_HANDLE *phGateway, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINCONNECTTOGATEWAY2) (RTS_HANDLE hDriver, RTS_I32 nNumParams, PARAMETER *pParams, RTS_UI32 ui32GwInactivityTimeout, RTS_HANDLE *phGateway, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINCONNECTTOGATEWAY2_NOTIMPLEMENTED)
	#define USE_GWClientBeginConnectToGateway2
	#define EXT_GWClientBeginConnectToGateway2
	#define GET_GWClientBeginConnectToGateway2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginConnectToGateway2(p0,p1,p2,p3,p4,p5) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginConnectToGateway2  FALSE
	#define EXP_GWClientBeginConnectToGateway2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginConnectToGateway2
	#define EXT_GWClientBeginConnectToGateway2
	#define GET_GWClientBeginConnectToGateway2(fl)  CAL_CMGETAPI( "GWClientBeginConnectToGateway2" ) 
	#define CAL_GWClientBeginConnectToGateway2  GWClientBeginConnectToGateway2
	#define CHK_GWClientBeginConnectToGateway2  TRUE
	#define EXP_GWClientBeginConnectToGateway2  CAL_CMEXPAPI( "GWClientBeginConnectToGateway2" ) 
	#define EXTREF_ITF_GWClientBeginConnectToGateway2 {(RTS_VOID_FCTPTR)GWClientBeginConnectToGateway2, "GWClientBeginConnectToGateway2", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginConnectToGateway2
	#define EXT_GWClientBeginConnectToGateway2
	#define GET_GWClientBeginConnectToGateway2(fl)  CAL_CMGETAPI( "GWClientBeginConnectToGateway2" ) 
	#define CAL_GWClientBeginConnectToGateway2  GWClientBeginConnectToGateway2
	#define CHK_GWClientBeginConnectToGateway2  TRUE
	#define EXP_GWClientBeginConnectToGateway2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginConnectToGateway2", (RTS_UINTPTR)GWClientBeginConnectToGateway2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginConnectToGateway2
	#define EXT_CmpGwClientGWClientBeginConnectToGateway2
	#define GET_CmpGwClientGWClientBeginConnectToGateway2  ERR_OK
	#define CAL_CmpGwClientGWClientBeginConnectToGateway2  GWClientBeginConnectToGateway2
	#define CHK_CmpGwClientGWClientBeginConnectToGateway2  TRUE
	#define EXP_CmpGwClientGWClientBeginConnectToGateway2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginConnectToGateway2
	#define EXT_GWClientBeginConnectToGateway2
	#define GET_GWClientBeginConnectToGateway2(fl)  CAL_CMGETAPI( "GWClientBeginConnectToGateway2" ) 
	#define CAL_GWClientBeginConnectToGateway2  GWClientBeginConnectToGateway2
	#define CHK_GWClientBeginConnectToGateway2  TRUE
	#define EXP_GWClientBeginConnectToGateway2  CAL_CMEXPAPI( "GWClientBeginConnectToGateway2" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginConnectToGateway2  PFGWCLIENTBEGINCONNECTTOGATEWAY2 pfGWClientBeginConnectToGateway2;
	#define EXT_GWClientBeginConnectToGateway2  extern PFGWCLIENTBEGINCONNECTTOGATEWAY2 pfGWClientBeginConnectToGateway2;
	#define GET_GWClientBeginConnectToGateway2(fl)  s_pfCMGetAPI2( "GWClientBeginConnectToGateway2", (RTS_VOID_FCTPTR *)&pfGWClientBeginConnectToGateway2, (fl), 0, 0)
	#define CAL_GWClientBeginConnectToGateway2  pfGWClientBeginConnectToGateway2
	#define CHK_GWClientBeginConnectToGateway2  (pfGWClientBeginConnectToGateway2 != NULL)
	#define EXP_GWClientBeginConnectToGateway2   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginConnectToGateway2", (RTS_UINTPTR)GWClientBeginConnectToGateway2, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEndConnectToGateway(ASYNCRESULT *pAsyncRes, RTS_HANDLE *phGateway);
typedef RTS_RESULT (CDECL * PFGWCLIENTENDCONNECTTOGATEWAY) (ASYNCRESULT *pAsyncRes, RTS_HANDLE *phGateway);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENDCONNECTTOGATEWAY_NOTIMPLEMENTED)
	#define USE_GWClientEndConnectToGateway
	#define EXT_GWClientEndConnectToGateway
	#define GET_GWClientEndConnectToGateway(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEndConnectToGateway(p0,p1) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEndConnectToGateway  FALSE
	#define EXP_GWClientEndConnectToGateway  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEndConnectToGateway
	#define EXT_GWClientEndConnectToGateway
	#define GET_GWClientEndConnectToGateway(fl)  CAL_CMGETAPI( "GWClientEndConnectToGateway" ) 
	#define CAL_GWClientEndConnectToGateway  GWClientEndConnectToGateway
	#define CHK_GWClientEndConnectToGateway  TRUE
	#define EXP_GWClientEndConnectToGateway  CAL_CMEXPAPI( "GWClientEndConnectToGateway" ) 
	#define EXTREF_ITF_GWClientEndConnectToGateway {(RTS_VOID_FCTPTR)GWClientEndConnectToGateway, "GWClientEndConnectToGateway", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEndConnectToGateway
	#define EXT_GWClientEndConnectToGateway
	#define GET_GWClientEndConnectToGateway(fl)  CAL_CMGETAPI( "GWClientEndConnectToGateway" ) 
	#define CAL_GWClientEndConnectToGateway  GWClientEndConnectToGateway
	#define CHK_GWClientEndConnectToGateway  TRUE
	#define EXP_GWClientEndConnectToGateway  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndConnectToGateway", (RTS_UINTPTR)GWClientEndConnectToGateway, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEndConnectToGateway
	#define EXT_CmpGwClientGWClientEndConnectToGateway
	#define GET_CmpGwClientGWClientEndConnectToGateway  ERR_OK
	#define CAL_CmpGwClientGWClientEndConnectToGateway  GWClientEndConnectToGateway
	#define CHK_CmpGwClientGWClientEndConnectToGateway  TRUE
	#define EXP_CmpGwClientGWClientEndConnectToGateway  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEndConnectToGateway
	#define EXT_GWClientEndConnectToGateway
	#define GET_GWClientEndConnectToGateway(fl)  CAL_CMGETAPI( "GWClientEndConnectToGateway" ) 
	#define CAL_GWClientEndConnectToGateway  GWClientEndConnectToGateway
	#define CHK_GWClientEndConnectToGateway  TRUE
	#define EXP_GWClientEndConnectToGateway  CAL_CMEXPAPI( "GWClientEndConnectToGateway" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEndConnectToGateway  PFGWCLIENTENDCONNECTTOGATEWAY pfGWClientEndConnectToGateway;
	#define EXT_GWClientEndConnectToGateway  extern PFGWCLIENTENDCONNECTTOGATEWAY pfGWClientEndConnectToGateway;
	#define GET_GWClientEndConnectToGateway(fl)  s_pfCMGetAPI2( "GWClientEndConnectToGateway", (RTS_VOID_FCTPTR *)&pfGWClientEndConnectToGateway, (fl), 0, 0)
	#define CAL_GWClientEndConnectToGateway  pfGWClientEndConnectToGateway
	#define CHK_GWClientEndConnectToGateway  (pfGWClientEndConnectToGateway != NULL)
	#define EXP_GWClientEndConnectToGateway   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndConnectToGateway", (RTS_UINTPTR)GWClientEndConnectToGateway, 0, 0) 
#endif

RTS_RESULT CDECL GWClientDisconnectFromGateway(RTS_HANDLE hGateway);
typedef RTS_RESULT (CDECL * PFGWCLIENTDISCONNECTFROMGATEWAY) (RTS_HANDLE hGateway);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTDISCONNECTFROMGATEWAY_NOTIMPLEMENTED)
	#define USE_GWClientDisconnectFromGateway
	#define EXT_GWClientDisconnectFromGateway
	#define GET_GWClientDisconnectFromGateway(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientDisconnectFromGateway(p0) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientDisconnectFromGateway  FALSE
	#define EXP_GWClientDisconnectFromGateway  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientDisconnectFromGateway
	#define EXT_GWClientDisconnectFromGateway
	#define GET_GWClientDisconnectFromGateway(fl)  CAL_CMGETAPI( "GWClientDisconnectFromGateway" ) 
	#define CAL_GWClientDisconnectFromGateway  GWClientDisconnectFromGateway
	#define CHK_GWClientDisconnectFromGateway  TRUE
	#define EXP_GWClientDisconnectFromGateway  CAL_CMEXPAPI( "GWClientDisconnectFromGateway" ) 
	#define EXTREF_ITF_GWClientDisconnectFromGateway {(RTS_VOID_FCTPTR)GWClientDisconnectFromGateway, "GWClientDisconnectFromGateway", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientDisconnectFromGateway
	#define EXT_GWClientDisconnectFromGateway
	#define GET_GWClientDisconnectFromGateway(fl)  CAL_CMGETAPI( "GWClientDisconnectFromGateway" ) 
	#define CAL_GWClientDisconnectFromGateway  GWClientDisconnectFromGateway
	#define CHK_GWClientDisconnectFromGateway  TRUE
	#define EXP_GWClientDisconnectFromGateway  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientDisconnectFromGateway", (RTS_UINTPTR)GWClientDisconnectFromGateway, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientDisconnectFromGateway
	#define EXT_CmpGwClientGWClientDisconnectFromGateway
	#define GET_CmpGwClientGWClientDisconnectFromGateway  ERR_OK
	#define CAL_CmpGwClientGWClientDisconnectFromGateway  GWClientDisconnectFromGateway
	#define CHK_CmpGwClientGWClientDisconnectFromGateway  TRUE
	#define EXP_CmpGwClientGWClientDisconnectFromGateway  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientDisconnectFromGateway
	#define EXT_GWClientDisconnectFromGateway
	#define GET_GWClientDisconnectFromGateway(fl)  CAL_CMGETAPI( "GWClientDisconnectFromGateway" ) 
	#define CAL_GWClientDisconnectFromGateway  GWClientDisconnectFromGateway
	#define CHK_GWClientDisconnectFromGateway  TRUE
	#define EXP_GWClientDisconnectFromGateway  CAL_CMEXPAPI( "GWClientDisconnectFromGateway" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientDisconnectFromGateway  PFGWCLIENTDISCONNECTFROMGATEWAY pfGWClientDisconnectFromGateway;
	#define EXT_GWClientDisconnectFromGateway  extern PFGWCLIENTDISCONNECTFROMGATEWAY pfGWClientDisconnectFromGateway;
	#define GET_GWClientDisconnectFromGateway(fl)  s_pfCMGetAPI2( "GWClientDisconnectFromGateway", (RTS_VOID_FCTPTR *)&pfGWClientDisconnectFromGateway, (fl), 0, 0)
	#define CAL_GWClientDisconnectFromGateway  pfGWClientDisconnectFromGateway
	#define CHK_GWClientDisconnectFromGateway  (pfGWClientDisconnectFromGateway != NULL)
	#define EXP_GWClientDisconnectFromGateway   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientDisconnectFromGateway", (RTS_UINTPTR)GWClientDisconnectFromGateway, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginIncrementalResolveAllNodes(RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK pfCallback, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGININCREMENTALRESOLVEALLNODES) (RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK pfCallback, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGININCREMENTALRESOLVEALLNODES_NOTIMPLEMENTED)
	#define USE_GWClientBeginIncrementalResolveAllNodes
	#define EXT_GWClientBeginIncrementalResolveAllNodes
	#define GET_GWClientBeginIncrementalResolveAllNodes(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginIncrementalResolveAllNodes(p0,p1,p2,p3) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginIncrementalResolveAllNodes  FALSE
	#define EXP_GWClientBeginIncrementalResolveAllNodes  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginIncrementalResolveAllNodes
	#define EXT_GWClientBeginIncrementalResolveAllNodes
	#define GET_GWClientBeginIncrementalResolveAllNodes(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveAllNodes" ) 
	#define CAL_GWClientBeginIncrementalResolveAllNodes  GWClientBeginIncrementalResolveAllNodes
	#define CHK_GWClientBeginIncrementalResolveAllNodes  TRUE
	#define EXP_GWClientBeginIncrementalResolveAllNodes  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveAllNodes" ) 
	#define EXTREF_ITF_GWClientBeginIncrementalResolveAllNodes {(RTS_VOID_FCTPTR)GWClientBeginIncrementalResolveAllNodes, "GWClientBeginIncrementalResolveAllNodes", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginIncrementalResolveAllNodes
	#define EXT_GWClientBeginIncrementalResolveAllNodes
	#define GET_GWClientBeginIncrementalResolveAllNodes(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveAllNodes" ) 
	#define CAL_GWClientBeginIncrementalResolveAllNodes  GWClientBeginIncrementalResolveAllNodes
	#define CHK_GWClientBeginIncrementalResolveAllNodes  TRUE
	#define EXP_GWClientBeginIncrementalResolveAllNodes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveAllNodes", (RTS_UINTPTR)GWClientBeginIncrementalResolveAllNodes, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginIncrementalResolveAllNodes
	#define EXT_CmpGwClientGWClientBeginIncrementalResolveAllNodes
	#define GET_CmpGwClientGWClientBeginIncrementalResolveAllNodes  ERR_OK
	#define CAL_CmpGwClientGWClientBeginIncrementalResolveAllNodes  GWClientBeginIncrementalResolveAllNodes
	#define CHK_CmpGwClientGWClientBeginIncrementalResolveAllNodes  TRUE
	#define EXP_CmpGwClientGWClientBeginIncrementalResolveAllNodes  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginIncrementalResolveAllNodes
	#define EXT_GWClientBeginIncrementalResolveAllNodes
	#define GET_GWClientBeginIncrementalResolveAllNodes(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveAllNodes" ) 
	#define CAL_GWClientBeginIncrementalResolveAllNodes  GWClientBeginIncrementalResolveAllNodes
	#define CHK_GWClientBeginIncrementalResolveAllNodes  TRUE
	#define EXP_GWClientBeginIncrementalResolveAllNodes  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveAllNodes" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginIncrementalResolveAllNodes  PFGWCLIENTBEGININCREMENTALRESOLVEALLNODES pfGWClientBeginIncrementalResolveAllNodes;
	#define EXT_GWClientBeginIncrementalResolveAllNodes  extern PFGWCLIENTBEGININCREMENTALRESOLVEALLNODES pfGWClientBeginIncrementalResolveAllNodes;
	#define GET_GWClientBeginIncrementalResolveAllNodes(fl)  s_pfCMGetAPI2( "GWClientBeginIncrementalResolveAllNodes", (RTS_VOID_FCTPTR *)&pfGWClientBeginIncrementalResolveAllNodes, (fl), 0, 0)
	#define CAL_GWClientBeginIncrementalResolveAllNodes  pfGWClientBeginIncrementalResolveAllNodes
	#define CHK_GWClientBeginIncrementalResolveAllNodes  (pfGWClientBeginIncrementalResolveAllNodes != NULL)
	#define EXP_GWClientBeginIncrementalResolveAllNodes   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveAllNodes", (RTS_UINTPTR)GWClientBeginIncrementalResolveAllNodes, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginIncrementalResolveAllNodes2(RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK2 pfCallback2, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGININCREMENTALRESOLVEALLNODES2) (RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK2 pfCallback2, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGININCREMENTALRESOLVEALLNODES2_NOTIMPLEMENTED)
	#define USE_GWClientBeginIncrementalResolveAllNodes2
	#define EXT_GWClientBeginIncrementalResolveAllNodes2
	#define GET_GWClientBeginIncrementalResolveAllNodes2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginIncrementalResolveAllNodes2(p0,p1,p2,p3) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginIncrementalResolveAllNodes2  FALSE
	#define EXP_GWClientBeginIncrementalResolveAllNodes2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginIncrementalResolveAllNodes2
	#define EXT_GWClientBeginIncrementalResolveAllNodes2
	#define GET_GWClientBeginIncrementalResolveAllNodes2(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveAllNodes2" ) 
	#define CAL_GWClientBeginIncrementalResolveAllNodes2  GWClientBeginIncrementalResolveAllNodes2
	#define CHK_GWClientBeginIncrementalResolveAllNodes2  TRUE
	#define EXP_GWClientBeginIncrementalResolveAllNodes2  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveAllNodes2" ) 
	#define EXTREF_ITF_GWClientBeginIncrementalResolveAllNodes2 {(RTS_VOID_FCTPTR)GWClientBeginIncrementalResolveAllNodes2, "GWClientBeginIncrementalResolveAllNodes2", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginIncrementalResolveAllNodes2
	#define EXT_GWClientBeginIncrementalResolveAllNodes2
	#define GET_GWClientBeginIncrementalResolveAllNodes2(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveAllNodes2" ) 
	#define CAL_GWClientBeginIncrementalResolveAllNodes2  GWClientBeginIncrementalResolveAllNodes2
	#define CHK_GWClientBeginIncrementalResolveAllNodes2  TRUE
	#define EXP_GWClientBeginIncrementalResolveAllNodes2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveAllNodes2", (RTS_UINTPTR)GWClientBeginIncrementalResolveAllNodes2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginIncrementalResolveAllNodes2
	#define EXT_CmpGwClientGWClientBeginIncrementalResolveAllNodes2
	#define GET_CmpGwClientGWClientBeginIncrementalResolveAllNodes2  ERR_OK
	#define CAL_CmpGwClientGWClientBeginIncrementalResolveAllNodes2  GWClientBeginIncrementalResolveAllNodes2
	#define CHK_CmpGwClientGWClientBeginIncrementalResolveAllNodes2  TRUE
	#define EXP_CmpGwClientGWClientBeginIncrementalResolveAllNodes2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginIncrementalResolveAllNodes2
	#define EXT_GWClientBeginIncrementalResolveAllNodes2
	#define GET_GWClientBeginIncrementalResolveAllNodes2(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveAllNodes2" ) 
	#define CAL_GWClientBeginIncrementalResolveAllNodes2  GWClientBeginIncrementalResolveAllNodes2
	#define CHK_GWClientBeginIncrementalResolveAllNodes2  TRUE
	#define EXP_GWClientBeginIncrementalResolveAllNodes2  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveAllNodes2" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginIncrementalResolveAllNodes2  PFGWCLIENTBEGININCREMENTALRESOLVEALLNODES2 pfGWClientBeginIncrementalResolveAllNodes2;
	#define EXT_GWClientBeginIncrementalResolveAllNodes2  extern PFGWCLIENTBEGININCREMENTALRESOLVEALLNODES2 pfGWClientBeginIncrementalResolveAllNodes2;
	#define GET_GWClientBeginIncrementalResolveAllNodes2(fl)  s_pfCMGetAPI2( "GWClientBeginIncrementalResolveAllNodes2", (RTS_VOID_FCTPTR *)&pfGWClientBeginIncrementalResolveAllNodes2, (fl), 0, 0)
	#define CAL_GWClientBeginIncrementalResolveAllNodes2  pfGWClientBeginIncrementalResolveAllNodes2
	#define CHK_GWClientBeginIncrementalResolveAllNodes2  (pfGWClientBeginIncrementalResolveAllNodes2 != NULL)
	#define EXP_GWClientBeginIncrementalResolveAllNodes2   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveAllNodes2", (RTS_UINTPTR)GWClientBeginIncrementalResolveAllNodes2, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginIncrementalResolveAllNodes3(RTS_HANDLE hGateway, RTS_UI32 dwTimeout, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK3 pfCallback3, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGININCREMENTALRESOLVEALLNODES3) (RTS_HANDLE hGateway, RTS_UI32 dwTimeout, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK3 pfCallback3, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGININCREMENTALRESOLVEALLNODES3_NOTIMPLEMENTED)
	#define USE_GWClientBeginIncrementalResolveAllNodes3
	#define EXT_GWClientBeginIncrementalResolveAllNodes3
	#define GET_GWClientBeginIncrementalResolveAllNodes3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginIncrementalResolveAllNodes3(p0,p1,p2,p3,p4) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginIncrementalResolveAllNodes3  FALSE
	#define EXP_GWClientBeginIncrementalResolveAllNodes3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginIncrementalResolveAllNodes3
	#define EXT_GWClientBeginIncrementalResolveAllNodes3
	#define GET_GWClientBeginIncrementalResolveAllNodes3(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveAllNodes3" ) 
	#define CAL_GWClientBeginIncrementalResolveAllNodes3  GWClientBeginIncrementalResolveAllNodes3
	#define CHK_GWClientBeginIncrementalResolveAllNodes3  TRUE
	#define EXP_GWClientBeginIncrementalResolveAllNodes3  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveAllNodes3" ) 
	#define EXTREF_ITF_GWClientBeginIncrementalResolveAllNodes3 {(RTS_VOID_FCTPTR)GWClientBeginIncrementalResolveAllNodes3, "GWClientBeginIncrementalResolveAllNodes3", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginIncrementalResolveAllNodes3
	#define EXT_GWClientBeginIncrementalResolveAllNodes3
	#define GET_GWClientBeginIncrementalResolveAllNodes3(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveAllNodes3" ) 
	#define CAL_GWClientBeginIncrementalResolveAllNodes3  GWClientBeginIncrementalResolveAllNodes3
	#define CHK_GWClientBeginIncrementalResolveAllNodes3  TRUE
	#define EXP_GWClientBeginIncrementalResolveAllNodes3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveAllNodes3", (RTS_UINTPTR)GWClientBeginIncrementalResolveAllNodes3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginIncrementalResolveAllNodes3
	#define EXT_CmpGwClientGWClientBeginIncrementalResolveAllNodes3
	#define GET_CmpGwClientGWClientBeginIncrementalResolveAllNodes3  ERR_OK
	#define CAL_CmpGwClientGWClientBeginIncrementalResolveAllNodes3  GWClientBeginIncrementalResolveAllNodes3
	#define CHK_CmpGwClientGWClientBeginIncrementalResolveAllNodes3  TRUE
	#define EXP_CmpGwClientGWClientBeginIncrementalResolveAllNodes3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginIncrementalResolveAllNodes3
	#define EXT_GWClientBeginIncrementalResolveAllNodes3
	#define GET_GWClientBeginIncrementalResolveAllNodes3(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveAllNodes3" ) 
	#define CAL_GWClientBeginIncrementalResolveAllNodes3  GWClientBeginIncrementalResolveAllNodes3
	#define CHK_GWClientBeginIncrementalResolveAllNodes3  TRUE
	#define EXP_GWClientBeginIncrementalResolveAllNodes3  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveAllNodes3" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginIncrementalResolveAllNodes3  PFGWCLIENTBEGININCREMENTALRESOLVEALLNODES3 pfGWClientBeginIncrementalResolveAllNodes3;
	#define EXT_GWClientBeginIncrementalResolveAllNodes3  extern PFGWCLIENTBEGININCREMENTALRESOLVEALLNODES3 pfGWClientBeginIncrementalResolveAllNodes3;
	#define GET_GWClientBeginIncrementalResolveAllNodes3(fl)  s_pfCMGetAPI2( "GWClientBeginIncrementalResolveAllNodes3", (RTS_VOID_FCTPTR *)&pfGWClientBeginIncrementalResolveAllNodes3, (fl), 0, 0)
	#define CAL_GWClientBeginIncrementalResolveAllNodes3  pfGWClientBeginIncrementalResolveAllNodes3
	#define CHK_GWClientBeginIncrementalResolveAllNodes3  (pfGWClientBeginIncrementalResolveAllNodes3 != NULL)
	#define EXP_GWClientBeginIncrementalResolveAllNodes3   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveAllNodes3", (RTS_UINTPTR)GWClientBeginIncrementalResolveAllNodes3, 0, 0) 
#endif

RTS_RESULT CDECL GWClientAdjustIncrementalResolveAllNodesTimeout(RTS_UI32 dwRemainingTimeout, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTADJUSTINCREMENTALRESOLVEALLNODESTIMEOUT) (RTS_UI32 dwRemainingTimeout, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTADJUSTINCREMENTALRESOLVEALLNODESTIMEOUT_NOTIMPLEMENTED)
	#define USE_GWClientAdjustIncrementalResolveAllNodesTimeout
	#define EXT_GWClientAdjustIncrementalResolveAllNodesTimeout
	#define GET_GWClientAdjustIncrementalResolveAllNodesTimeout(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientAdjustIncrementalResolveAllNodesTimeout(p0,p1) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientAdjustIncrementalResolveAllNodesTimeout  FALSE
	#define EXP_GWClientAdjustIncrementalResolveAllNodesTimeout  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientAdjustIncrementalResolveAllNodesTimeout
	#define EXT_GWClientAdjustIncrementalResolveAllNodesTimeout
	#define GET_GWClientAdjustIncrementalResolveAllNodesTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustIncrementalResolveAllNodesTimeout" ) 
	#define CAL_GWClientAdjustIncrementalResolveAllNodesTimeout  GWClientAdjustIncrementalResolveAllNodesTimeout
	#define CHK_GWClientAdjustIncrementalResolveAllNodesTimeout  TRUE
	#define EXP_GWClientAdjustIncrementalResolveAllNodesTimeout  CAL_CMEXPAPI( "GWClientAdjustIncrementalResolveAllNodesTimeout" ) 
	#define EXTREF_ITF_GWClientAdjustIncrementalResolveAllNodesTimeout {(RTS_VOID_FCTPTR)GWClientAdjustIncrementalResolveAllNodesTimeout, "GWClientAdjustIncrementalResolveAllNodesTimeout", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientAdjustIncrementalResolveAllNodesTimeout
	#define EXT_GWClientAdjustIncrementalResolveAllNodesTimeout
	#define GET_GWClientAdjustIncrementalResolveAllNodesTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustIncrementalResolveAllNodesTimeout" ) 
	#define CAL_GWClientAdjustIncrementalResolveAllNodesTimeout  GWClientAdjustIncrementalResolveAllNodesTimeout
	#define CHK_GWClientAdjustIncrementalResolveAllNodesTimeout  TRUE
	#define EXP_GWClientAdjustIncrementalResolveAllNodesTimeout  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientAdjustIncrementalResolveAllNodesTimeout", (RTS_UINTPTR)GWClientAdjustIncrementalResolveAllNodesTimeout, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientAdjustIncrementalResolveAllNodesTimeout
	#define EXT_CmpGwClientGWClientAdjustIncrementalResolveAllNodesTimeout
	#define GET_CmpGwClientGWClientAdjustIncrementalResolveAllNodesTimeout  ERR_OK
	#define CAL_CmpGwClientGWClientAdjustIncrementalResolveAllNodesTimeout  GWClientAdjustIncrementalResolveAllNodesTimeout
	#define CHK_CmpGwClientGWClientAdjustIncrementalResolveAllNodesTimeout  TRUE
	#define EXP_CmpGwClientGWClientAdjustIncrementalResolveAllNodesTimeout  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientAdjustIncrementalResolveAllNodesTimeout
	#define EXT_GWClientAdjustIncrementalResolveAllNodesTimeout
	#define GET_GWClientAdjustIncrementalResolveAllNodesTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustIncrementalResolveAllNodesTimeout" ) 
	#define CAL_GWClientAdjustIncrementalResolveAllNodesTimeout  GWClientAdjustIncrementalResolveAllNodesTimeout
	#define CHK_GWClientAdjustIncrementalResolveAllNodesTimeout  TRUE
	#define EXP_GWClientAdjustIncrementalResolveAllNodesTimeout  CAL_CMEXPAPI( "GWClientAdjustIncrementalResolveAllNodesTimeout" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientAdjustIncrementalResolveAllNodesTimeout  PFGWCLIENTADJUSTINCREMENTALRESOLVEALLNODESTIMEOUT pfGWClientAdjustIncrementalResolveAllNodesTimeout;
	#define EXT_GWClientAdjustIncrementalResolveAllNodesTimeout  extern PFGWCLIENTADJUSTINCREMENTALRESOLVEALLNODESTIMEOUT pfGWClientAdjustIncrementalResolveAllNodesTimeout;
	#define GET_GWClientAdjustIncrementalResolveAllNodesTimeout(fl)  s_pfCMGetAPI2( "GWClientAdjustIncrementalResolveAllNodesTimeout", (RTS_VOID_FCTPTR *)&pfGWClientAdjustIncrementalResolveAllNodesTimeout, (fl), 0, 0)
	#define CAL_GWClientAdjustIncrementalResolveAllNodesTimeout  pfGWClientAdjustIncrementalResolveAllNodesTimeout
	#define CHK_GWClientAdjustIncrementalResolveAllNodesTimeout  (pfGWClientAdjustIncrementalResolveAllNodesTimeout != NULL)
	#define EXP_GWClientAdjustIncrementalResolveAllNodesTimeout   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientAdjustIncrementalResolveAllNodesTimeout", (RTS_UINTPTR)GWClientAdjustIncrementalResolveAllNodesTimeout, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEndIncrementalResolveAllNodes(ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTENDINCREMENTALRESOLVEALLNODES) (ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENDINCREMENTALRESOLVEALLNODES_NOTIMPLEMENTED)
	#define USE_GWClientEndIncrementalResolveAllNodes
	#define EXT_GWClientEndIncrementalResolveAllNodes
	#define GET_GWClientEndIncrementalResolveAllNodes(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEndIncrementalResolveAllNodes(p0) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEndIncrementalResolveAllNodes  FALSE
	#define EXP_GWClientEndIncrementalResolveAllNodes  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEndIncrementalResolveAllNodes
	#define EXT_GWClientEndIncrementalResolveAllNodes
	#define GET_GWClientEndIncrementalResolveAllNodes(fl)  CAL_CMGETAPI( "GWClientEndIncrementalResolveAllNodes" ) 
	#define CAL_GWClientEndIncrementalResolveAllNodes  GWClientEndIncrementalResolveAllNodes
	#define CHK_GWClientEndIncrementalResolveAllNodes  TRUE
	#define EXP_GWClientEndIncrementalResolveAllNodes  CAL_CMEXPAPI( "GWClientEndIncrementalResolveAllNodes" ) 
	#define EXTREF_ITF_GWClientEndIncrementalResolveAllNodes {(RTS_VOID_FCTPTR)GWClientEndIncrementalResolveAllNodes, "GWClientEndIncrementalResolveAllNodes", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEndIncrementalResolveAllNodes
	#define EXT_GWClientEndIncrementalResolveAllNodes
	#define GET_GWClientEndIncrementalResolveAllNodes(fl)  CAL_CMGETAPI( "GWClientEndIncrementalResolveAllNodes" ) 
	#define CAL_GWClientEndIncrementalResolveAllNodes  GWClientEndIncrementalResolveAllNodes
	#define CHK_GWClientEndIncrementalResolveAllNodes  TRUE
	#define EXP_GWClientEndIncrementalResolveAllNodes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndIncrementalResolveAllNodes", (RTS_UINTPTR)GWClientEndIncrementalResolveAllNodes, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEndIncrementalResolveAllNodes
	#define EXT_CmpGwClientGWClientEndIncrementalResolveAllNodes
	#define GET_CmpGwClientGWClientEndIncrementalResolveAllNodes  ERR_OK
	#define CAL_CmpGwClientGWClientEndIncrementalResolveAllNodes  GWClientEndIncrementalResolveAllNodes
	#define CHK_CmpGwClientGWClientEndIncrementalResolveAllNodes  TRUE
	#define EXP_CmpGwClientGWClientEndIncrementalResolveAllNodes  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEndIncrementalResolveAllNodes
	#define EXT_GWClientEndIncrementalResolveAllNodes
	#define GET_GWClientEndIncrementalResolveAllNodes(fl)  CAL_CMGETAPI( "GWClientEndIncrementalResolveAllNodes" ) 
	#define CAL_GWClientEndIncrementalResolveAllNodes  GWClientEndIncrementalResolveAllNodes
	#define CHK_GWClientEndIncrementalResolveAllNodes  TRUE
	#define EXP_GWClientEndIncrementalResolveAllNodes  CAL_CMEXPAPI( "GWClientEndIncrementalResolveAllNodes" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEndIncrementalResolveAllNodes  PFGWCLIENTENDINCREMENTALRESOLVEALLNODES pfGWClientEndIncrementalResolveAllNodes;
	#define EXT_GWClientEndIncrementalResolveAllNodes  extern PFGWCLIENTENDINCREMENTALRESOLVEALLNODES pfGWClientEndIncrementalResolveAllNodes;
	#define GET_GWClientEndIncrementalResolveAllNodes(fl)  s_pfCMGetAPI2( "GWClientEndIncrementalResolveAllNodes", (RTS_VOID_FCTPTR *)&pfGWClientEndIncrementalResolveAllNodes, (fl), 0, 0)
	#define CAL_GWClientEndIncrementalResolveAllNodes  pfGWClientEndIncrementalResolveAllNodes
	#define CHK_GWClientEndIncrementalResolveAllNodes  (pfGWClientEndIncrementalResolveAllNodes != NULL)
	#define EXP_GWClientEndIncrementalResolveAllNodes   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndIncrementalResolveAllNodes", (RTS_UINTPTR)GWClientEndIncrementalResolveAllNodes, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginIncrementalResolveName(const RTS_WCHAR *pwszNodeName, RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK pfCallback, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGININCREMENTALRESOLVENAME) (const RTS_WCHAR *pwszNodeName, RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK pfCallback, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGININCREMENTALRESOLVENAME_NOTIMPLEMENTED)
	#define USE_GWClientBeginIncrementalResolveName
	#define EXT_GWClientBeginIncrementalResolveName
	#define GET_GWClientBeginIncrementalResolveName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginIncrementalResolveName(p0,p1,p2,p3,p4) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginIncrementalResolveName  FALSE
	#define EXP_GWClientBeginIncrementalResolveName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginIncrementalResolveName
	#define EXT_GWClientBeginIncrementalResolveName
	#define GET_GWClientBeginIncrementalResolveName(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveName" ) 
	#define CAL_GWClientBeginIncrementalResolveName  GWClientBeginIncrementalResolveName
	#define CHK_GWClientBeginIncrementalResolveName  TRUE
	#define EXP_GWClientBeginIncrementalResolveName  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveName" ) 
	#define EXTREF_ITF_GWClientBeginIncrementalResolveName {(RTS_VOID_FCTPTR)GWClientBeginIncrementalResolveName, "GWClientBeginIncrementalResolveName", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginIncrementalResolveName
	#define EXT_GWClientBeginIncrementalResolveName
	#define GET_GWClientBeginIncrementalResolveName(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveName" ) 
	#define CAL_GWClientBeginIncrementalResolveName  GWClientBeginIncrementalResolveName
	#define CHK_GWClientBeginIncrementalResolveName  TRUE
	#define EXP_GWClientBeginIncrementalResolveName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveName", (RTS_UINTPTR)GWClientBeginIncrementalResolveName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginIncrementalResolveName
	#define EXT_CmpGwClientGWClientBeginIncrementalResolveName
	#define GET_CmpGwClientGWClientBeginIncrementalResolveName  ERR_OK
	#define CAL_CmpGwClientGWClientBeginIncrementalResolveName  GWClientBeginIncrementalResolveName
	#define CHK_CmpGwClientGWClientBeginIncrementalResolveName  TRUE
	#define EXP_CmpGwClientGWClientBeginIncrementalResolveName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginIncrementalResolveName
	#define EXT_GWClientBeginIncrementalResolveName
	#define GET_GWClientBeginIncrementalResolveName(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveName" ) 
	#define CAL_GWClientBeginIncrementalResolveName  GWClientBeginIncrementalResolveName
	#define CHK_GWClientBeginIncrementalResolveName  TRUE
	#define EXP_GWClientBeginIncrementalResolveName  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveName" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginIncrementalResolveName  PFGWCLIENTBEGININCREMENTALRESOLVENAME pfGWClientBeginIncrementalResolveName;
	#define EXT_GWClientBeginIncrementalResolveName  extern PFGWCLIENTBEGININCREMENTALRESOLVENAME pfGWClientBeginIncrementalResolveName;
	#define GET_GWClientBeginIncrementalResolveName(fl)  s_pfCMGetAPI2( "GWClientBeginIncrementalResolveName", (RTS_VOID_FCTPTR *)&pfGWClientBeginIncrementalResolveName, (fl), 0, 0)
	#define CAL_GWClientBeginIncrementalResolveName  pfGWClientBeginIncrementalResolveName
	#define CHK_GWClientBeginIncrementalResolveName  (pfGWClientBeginIncrementalResolveName != NULL)
	#define EXP_GWClientBeginIncrementalResolveName   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveName", (RTS_UINTPTR)GWClientBeginIncrementalResolveName, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginIncrementalResolveName3(const RTS_WCHAR *pwszNodeName, RTS_HANDLE hGateway, RTS_UI32 dwTimeout, RTS_BOOL bFinishWithFirstNode, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK3 pfCallback3, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGININCREMENTALRESOLVENAME3) (const RTS_WCHAR *pwszNodeName, RTS_HANDLE hGateway, RTS_UI32 dwTimeout, RTS_BOOL bFinishWithFirstNode, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK3 pfCallback3, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGININCREMENTALRESOLVENAME3_NOTIMPLEMENTED)
	#define USE_GWClientBeginIncrementalResolveName3
	#define EXT_GWClientBeginIncrementalResolveName3
	#define GET_GWClientBeginIncrementalResolveName3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginIncrementalResolveName3(p0,p1,p2,p3,p4,p5,p6) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginIncrementalResolveName3  FALSE
	#define EXP_GWClientBeginIncrementalResolveName3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginIncrementalResolveName3
	#define EXT_GWClientBeginIncrementalResolveName3
	#define GET_GWClientBeginIncrementalResolveName3(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveName3" ) 
	#define CAL_GWClientBeginIncrementalResolveName3  GWClientBeginIncrementalResolveName3
	#define CHK_GWClientBeginIncrementalResolveName3  TRUE
	#define EXP_GWClientBeginIncrementalResolveName3  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveName3" ) 
	#define EXTREF_ITF_GWClientBeginIncrementalResolveName3 {(RTS_VOID_FCTPTR)GWClientBeginIncrementalResolveName3, "GWClientBeginIncrementalResolveName3", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginIncrementalResolveName3
	#define EXT_GWClientBeginIncrementalResolveName3
	#define GET_GWClientBeginIncrementalResolveName3(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveName3" ) 
	#define CAL_GWClientBeginIncrementalResolveName3  GWClientBeginIncrementalResolveName3
	#define CHK_GWClientBeginIncrementalResolveName3  TRUE
	#define EXP_GWClientBeginIncrementalResolveName3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveName3", (RTS_UINTPTR)GWClientBeginIncrementalResolveName3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginIncrementalResolveName3
	#define EXT_CmpGwClientGWClientBeginIncrementalResolveName3
	#define GET_CmpGwClientGWClientBeginIncrementalResolveName3  ERR_OK
	#define CAL_CmpGwClientGWClientBeginIncrementalResolveName3  GWClientBeginIncrementalResolveName3
	#define CHK_CmpGwClientGWClientBeginIncrementalResolveName3  TRUE
	#define EXP_CmpGwClientGWClientBeginIncrementalResolveName3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginIncrementalResolveName3
	#define EXT_GWClientBeginIncrementalResolveName3
	#define GET_GWClientBeginIncrementalResolveName3(fl)  CAL_CMGETAPI( "GWClientBeginIncrementalResolveName3" ) 
	#define CAL_GWClientBeginIncrementalResolveName3  GWClientBeginIncrementalResolveName3
	#define CHK_GWClientBeginIncrementalResolveName3  TRUE
	#define EXP_GWClientBeginIncrementalResolveName3  CAL_CMEXPAPI( "GWClientBeginIncrementalResolveName3" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginIncrementalResolveName3  PFGWCLIENTBEGININCREMENTALRESOLVENAME3 pfGWClientBeginIncrementalResolveName3;
	#define EXT_GWClientBeginIncrementalResolveName3  extern PFGWCLIENTBEGININCREMENTALRESOLVENAME3 pfGWClientBeginIncrementalResolveName3;
	#define GET_GWClientBeginIncrementalResolveName3(fl)  s_pfCMGetAPI2( "GWClientBeginIncrementalResolveName3", (RTS_VOID_FCTPTR *)&pfGWClientBeginIncrementalResolveName3, (fl), 0, 0)
	#define CAL_GWClientBeginIncrementalResolveName3  pfGWClientBeginIncrementalResolveName3
	#define CHK_GWClientBeginIncrementalResolveName3  (pfGWClientBeginIncrementalResolveName3 != NULL)
	#define EXP_GWClientBeginIncrementalResolveName3   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginIncrementalResolveName3", (RTS_UINTPTR)GWClientBeginIncrementalResolveName3, 0, 0) 
#endif

RTS_RESULT CDECL GWClientAdjustIncrementalResolveNameTimeout(RTS_UI32 dwRemainingTimeout, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTADJUSTINCREMENTALRESOLVENAMETIMEOUT) (RTS_UI32 dwRemainingTimeout, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTADJUSTINCREMENTALRESOLVENAMETIMEOUT_NOTIMPLEMENTED)
	#define USE_GWClientAdjustIncrementalResolveNameTimeout
	#define EXT_GWClientAdjustIncrementalResolveNameTimeout
	#define GET_GWClientAdjustIncrementalResolveNameTimeout(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientAdjustIncrementalResolveNameTimeout(p0,p1) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientAdjustIncrementalResolveNameTimeout  FALSE
	#define EXP_GWClientAdjustIncrementalResolveNameTimeout  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientAdjustIncrementalResolveNameTimeout
	#define EXT_GWClientAdjustIncrementalResolveNameTimeout
	#define GET_GWClientAdjustIncrementalResolveNameTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustIncrementalResolveNameTimeout" ) 
	#define CAL_GWClientAdjustIncrementalResolveNameTimeout  GWClientAdjustIncrementalResolveNameTimeout
	#define CHK_GWClientAdjustIncrementalResolveNameTimeout  TRUE
	#define EXP_GWClientAdjustIncrementalResolveNameTimeout  CAL_CMEXPAPI( "GWClientAdjustIncrementalResolveNameTimeout" ) 
	#define EXTREF_ITF_GWClientAdjustIncrementalResolveNameTimeout {(RTS_VOID_FCTPTR)GWClientAdjustIncrementalResolveNameTimeout, "GWClientAdjustIncrementalResolveNameTimeout", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientAdjustIncrementalResolveNameTimeout
	#define EXT_GWClientAdjustIncrementalResolveNameTimeout
	#define GET_GWClientAdjustIncrementalResolveNameTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustIncrementalResolveNameTimeout" ) 
	#define CAL_GWClientAdjustIncrementalResolveNameTimeout  GWClientAdjustIncrementalResolveNameTimeout
	#define CHK_GWClientAdjustIncrementalResolveNameTimeout  TRUE
	#define EXP_GWClientAdjustIncrementalResolveNameTimeout  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientAdjustIncrementalResolveNameTimeout", (RTS_UINTPTR)GWClientAdjustIncrementalResolveNameTimeout, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientAdjustIncrementalResolveNameTimeout
	#define EXT_CmpGwClientGWClientAdjustIncrementalResolveNameTimeout
	#define GET_CmpGwClientGWClientAdjustIncrementalResolveNameTimeout  ERR_OK
	#define CAL_CmpGwClientGWClientAdjustIncrementalResolveNameTimeout  GWClientAdjustIncrementalResolveNameTimeout
	#define CHK_CmpGwClientGWClientAdjustIncrementalResolveNameTimeout  TRUE
	#define EXP_CmpGwClientGWClientAdjustIncrementalResolveNameTimeout  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientAdjustIncrementalResolveNameTimeout
	#define EXT_GWClientAdjustIncrementalResolveNameTimeout
	#define GET_GWClientAdjustIncrementalResolveNameTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustIncrementalResolveNameTimeout" ) 
	#define CAL_GWClientAdjustIncrementalResolveNameTimeout  GWClientAdjustIncrementalResolveNameTimeout
	#define CHK_GWClientAdjustIncrementalResolveNameTimeout  TRUE
	#define EXP_GWClientAdjustIncrementalResolveNameTimeout  CAL_CMEXPAPI( "GWClientAdjustIncrementalResolveNameTimeout" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientAdjustIncrementalResolveNameTimeout  PFGWCLIENTADJUSTINCREMENTALRESOLVENAMETIMEOUT pfGWClientAdjustIncrementalResolveNameTimeout;
	#define EXT_GWClientAdjustIncrementalResolveNameTimeout  extern PFGWCLIENTADJUSTINCREMENTALRESOLVENAMETIMEOUT pfGWClientAdjustIncrementalResolveNameTimeout;
	#define GET_GWClientAdjustIncrementalResolveNameTimeout(fl)  s_pfCMGetAPI2( "GWClientAdjustIncrementalResolveNameTimeout", (RTS_VOID_FCTPTR *)&pfGWClientAdjustIncrementalResolveNameTimeout, (fl), 0, 0)
	#define CAL_GWClientAdjustIncrementalResolveNameTimeout  pfGWClientAdjustIncrementalResolveNameTimeout
	#define CHK_GWClientAdjustIncrementalResolveNameTimeout  (pfGWClientAdjustIncrementalResolveNameTimeout != NULL)
	#define EXP_GWClientAdjustIncrementalResolveNameTimeout   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientAdjustIncrementalResolveNameTimeout", (RTS_UINTPTR)GWClientAdjustIncrementalResolveNameTimeout, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEndIncrementalResolveName(ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTENDINCREMENTALRESOLVENAME) (ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENDINCREMENTALRESOLVENAME_NOTIMPLEMENTED)
	#define USE_GWClientEndIncrementalResolveName
	#define EXT_GWClientEndIncrementalResolveName
	#define GET_GWClientEndIncrementalResolveName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEndIncrementalResolveName(p0) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEndIncrementalResolveName  FALSE
	#define EXP_GWClientEndIncrementalResolveName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEndIncrementalResolveName
	#define EXT_GWClientEndIncrementalResolveName
	#define GET_GWClientEndIncrementalResolveName(fl)  CAL_CMGETAPI( "GWClientEndIncrementalResolveName" ) 
	#define CAL_GWClientEndIncrementalResolveName  GWClientEndIncrementalResolveName
	#define CHK_GWClientEndIncrementalResolveName  TRUE
	#define EXP_GWClientEndIncrementalResolveName  CAL_CMEXPAPI( "GWClientEndIncrementalResolveName" ) 
	#define EXTREF_ITF_GWClientEndIncrementalResolveName {(RTS_VOID_FCTPTR)GWClientEndIncrementalResolveName, "GWClientEndIncrementalResolveName", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEndIncrementalResolveName
	#define EXT_GWClientEndIncrementalResolveName
	#define GET_GWClientEndIncrementalResolveName(fl)  CAL_CMGETAPI( "GWClientEndIncrementalResolveName" ) 
	#define CAL_GWClientEndIncrementalResolveName  GWClientEndIncrementalResolveName
	#define CHK_GWClientEndIncrementalResolveName  TRUE
	#define EXP_GWClientEndIncrementalResolveName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndIncrementalResolveName", (RTS_UINTPTR)GWClientEndIncrementalResolveName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEndIncrementalResolveName
	#define EXT_CmpGwClientGWClientEndIncrementalResolveName
	#define GET_CmpGwClientGWClientEndIncrementalResolveName  ERR_OK
	#define CAL_CmpGwClientGWClientEndIncrementalResolveName  GWClientEndIncrementalResolveName
	#define CHK_CmpGwClientGWClientEndIncrementalResolveName  TRUE
	#define EXP_CmpGwClientGWClientEndIncrementalResolveName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEndIncrementalResolveName
	#define EXT_GWClientEndIncrementalResolveName
	#define GET_GWClientEndIncrementalResolveName(fl)  CAL_CMGETAPI( "GWClientEndIncrementalResolveName" ) 
	#define CAL_GWClientEndIncrementalResolveName  GWClientEndIncrementalResolveName
	#define CHK_GWClientEndIncrementalResolveName  TRUE
	#define EXP_GWClientEndIncrementalResolveName  CAL_CMEXPAPI( "GWClientEndIncrementalResolveName" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEndIncrementalResolveName  PFGWCLIENTENDINCREMENTALRESOLVENAME pfGWClientEndIncrementalResolveName;
	#define EXT_GWClientEndIncrementalResolveName  extern PFGWCLIENTENDINCREMENTALRESOLVENAME pfGWClientEndIncrementalResolveName;
	#define GET_GWClientEndIncrementalResolveName(fl)  s_pfCMGetAPI2( "GWClientEndIncrementalResolveName", (RTS_VOID_FCTPTR *)&pfGWClientEndIncrementalResolveName, (fl), 0, 0)
	#define CAL_GWClientEndIncrementalResolveName  pfGWClientEndIncrementalResolveName
	#define CHK_GWClientEndIncrementalResolveName  (pfGWClientEndIncrementalResolveName != NULL)
	#define EXP_GWClientEndIncrementalResolveName   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndIncrementalResolveName", (RTS_UINTPTR)GWClientEndIncrementalResolveName, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginResolveBlockDriverAddress(RTS_UI8 byBlkDrvType, RTS_UI8 byBlkDrvFlags, RTS_UI8 byAddrBitLength, RTS_UI8 *pbyAddress, RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK2 pfCallback, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINRESOLVEBLOCKDRIVERADDRESS) (RTS_UI8 byBlkDrvType, RTS_UI8 byBlkDrvFlags, RTS_UI8 byAddrBitLength, RTS_UI8 *pbyAddress, RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK2 pfCallback, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINRESOLVEBLOCKDRIVERADDRESS_NOTIMPLEMENTED)
	#define USE_GWClientBeginResolveBlockDriverAddress
	#define EXT_GWClientBeginResolveBlockDriverAddress
	#define GET_GWClientBeginResolveBlockDriverAddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginResolveBlockDriverAddress(p0,p1,p2,p3,p4,p5,p6,p7) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginResolveBlockDriverAddress  FALSE
	#define EXP_GWClientBeginResolveBlockDriverAddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginResolveBlockDriverAddress
	#define EXT_GWClientBeginResolveBlockDriverAddress
	#define GET_GWClientBeginResolveBlockDriverAddress(fl)  CAL_CMGETAPI( "GWClientBeginResolveBlockDriverAddress" ) 
	#define CAL_GWClientBeginResolveBlockDriverAddress  GWClientBeginResolveBlockDriverAddress
	#define CHK_GWClientBeginResolveBlockDriverAddress  TRUE
	#define EXP_GWClientBeginResolveBlockDriverAddress  CAL_CMEXPAPI( "GWClientBeginResolveBlockDriverAddress" ) 
	#define EXTREF_ITF_GWClientBeginResolveBlockDriverAddress {(RTS_VOID_FCTPTR)GWClientBeginResolveBlockDriverAddress, "GWClientBeginResolveBlockDriverAddress", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginResolveBlockDriverAddress
	#define EXT_GWClientBeginResolveBlockDriverAddress
	#define GET_GWClientBeginResolveBlockDriverAddress(fl)  CAL_CMGETAPI( "GWClientBeginResolveBlockDriverAddress" ) 
	#define CAL_GWClientBeginResolveBlockDriverAddress  GWClientBeginResolveBlockDriverAddress
	#define CHK_GWClientBeginResolveBlockDriverAddress  TRUE
	#define EXP_GWClientBeginResolveBlockDriverAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginResolveBlockDriverAddress", (RTS_UINTPTR)GWClientBeginResolveBlockDriverAddress, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginResolveBlockDriverAddress
	#define EXT_CmpGwClientGWClientBeginResolveBlockDriverAddress
	#define GET_CmpGwClientGWClientBeginResolveBlockDriverAddress  ERR_OK
	#define CAL_CmpGwClientGWClientBeginResolveBlockDriverAddress  GWClientBeginResolveBlockDriverAddress
	#define CHK_CmpGwClientGWClientBeginResolveBlockDriverAddress  TRUE
	#define EXP_CmpGwClientGWClientBeginResolveBlockDriverAddress  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginResolveBlockDriverAddress
	#define EXT_GWClientBeginResolveBlockDriverAddress
	#define GET_GWClientBeginResolveBlockDriverAddress(fl)  CAL_CMGETAPI( "GWClientBeginResolveBlockDriverAddress" ) 
	#define CAL_GWClientBeginResolveBlockDriverAddress  GWClientBeginResolveBlockDriverAddress
	#define CHK_GWClientBeginResolveBlockDriverAddress  TRUE
	#define EXP_GWClientBeginResolveBlockDriverAddress  CAL_CMEXPAPI( "GWClientBeginResolveBlockDriverAddress" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginResolveBlockDriverAddress  PFGWCLIENTBEGINRESOLVEBLOCKDRIVERADDRESS pfGWClientBeginResolveBlockDriverAddress;
	#define EXT_GWClientBeginResolveBlockDriverAddress  extern PFGWCLIENTBEGINRESOLVEBLOCKDRIVERADDRESS pfGWClientBeginResolveBlockDriverAddress;
	#define GET_GWClientBeginResolveBlockDriverAddress(fl)  s_pfCMGetAPI2( "GWClientBeginResolveBlockDriverAddress", (RTS_VOID_FCTPTR *)&pfGWClientBeginResolveBlockDriverAddress, (fl), 0, 0)
	#define CAL_GWClientBeginResolveBlockDriverAddress  pfGWClientBeginResolveBlockDriverAddress
	#define CHK_GWClientBeginResolveBlockDriverAddress  (pfGWClientBeginResolveBlockDriverAddress != NULL)
	#define EXP_GWClientBeginResolveBlockDriverAddress   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginResolveBlockDriverAddress", (RTS_UINTPTR)GWClientBeginResolveBlockDriverAddress, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginResolveBlockDriverAddress3(RTS_UI8 byBlkDrvType, RTS_UI8 byBlkDrvFlags, RTS_UI8 byAddrBitLength, RTS_UI8 *pbyAddress, RTS_HANDLE hGateway, RTS_UI32 dwTimeout, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK3 pfCallback3, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINRESOLVEBLOCKDRIVERADDRESS3) (RTS_UI8 byBlkDrvType, RTS_UI8 byBlkDrvFlags, RTS_UI8 byAddrBitLength, RTS_UI8 *pbyAddress, RTS_HANDLE hGateway, RTS_UI32 dwTimeout, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK3 pfCallback3, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINRESOLVEBLOCKDRIVERADDRESS3_NOTIMPLEMENTED)
	#define USE_GWClientBeginResolveBlockDriverAddress3
	#define EXT_GWClientBeginResolveBlockDriverAddress3
	#define GET_GWClientBeginResolveBlockDriverAddress3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginResolveBlockDriverAddress3(p0,p1,p2,p3,p4,p5,p6,p7,p8) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginResolveBlockDriverAddress3  FALSE
	#define EXP_GWClientBeginResolveBlockDriverAddress3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginResolveBlockDriverAddress3
	#define EXT_GWClientBeginResolveBlockDriverAddress3
	#define GET_GWClientBeginResolveBlockDriverAddress3(fl)  CAL_CMGETAPI( "GWClientBeginResolveBlockDriverAddress3" ) 
	#define CAL_GWClientBeginResolveBlockDriverAddress3  GWClientBeginResolveBlockDriverAddress3
	#define CHK_GWClientBeginResolveBlockDriverAddress3  TRUE
	#define EXP_GWClientBeginResolveBlockDriverAddress3  CAL_CMEXPAPI( "GWClientBeginResolveBlockDriverAddress3" ) 
	#define EXTREF_ITF_GWClientBeginResolveBlockDriverAddress3 {(RTS_VOID_FCTPTR)GWClientBeginResolveBlockDriverAddress3, "GWClientBeginResolveBlockDriverAddress3", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginResolveBlockDriverAddress3
	#define EXT_GWClientBeginResolveBlockDriverAddress3
	#define GET_GWClientBeginResolveBlockDriverAddress3(fl)  CAL_CMGETAPI( "GWClientBeginResolveBlockDriverAddress3" ) 
	#define CAL_GWClientBeginResolveBlockDriverAddress3  GWClientBeginResolveBlockDriverAddress3
	#define CHK_GWClientBeginResolveBlockDriverAddress3  TRUE
	#define EXP_GWClientBeginResolveBlockDriverAddress3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginResolveBlockDriverAddress3", (RTS_UINTPTR)GWClientBeginResolveBlockDriverAddress3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginResolveBlockDriverAddress3
	#define EXT_CmpGwClientGWClientBeginResolveBlockDriverAddress3
	#define GET_CmpGwClientGWClientBeginResolveBlockDriverAddress3  ERR_OK
	#define CAL_CmpGwClientGWClientBeginResolveBlockDriverAddress3  GWClientBeginResolveBlockDriverAddress3
	#define CHK_CmpGwClientGWClientBeginResolveBlockDriverAddress3  TRUE
	#define EXP_CmpGwClientGWClientBeginResolveBlockDriverAddress3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginResolveBlockDriverAddress3
	#define EXT_GWClientBeginResolveBlockDriverAddress3
	#define GET_GWClientBeginResolveBlockDriverAddress3(fl)  CAL_CMGETAPI( "GWClientBeginResolveBlockDriverAddress3" ) 
	#define CAL_GWClientBeginResolveBlockDriverAddress3  GWClientBeginResolveBlockDriverAddress3
	#define CHK_GWClientBeginResolveBlockDriverAddress3  TRUE
	#define EXP_GWClientBeginResolveBlockDriverAddress3  CAL_CMEXPAPI( "GWClientBeginResolveBlockDriverAddress3" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginResolveBlockDriverAddress3  PFGWCLIENTBEGINRESOLVEBLOCKDRIVERADDRESS3 pfGWClientBeginResolveBlockDriverAddress3;
	#define EXT_GWClientBeginResolveBlockDriverAddress3  extern PFGWCLIENTBEGINRESOLVEBLOCKDRIVERADDRESS3 pfGWClientBeginResolveBlockDriverAddress3;
	#define GET_GWClientBeginResolveBlockDriverAddress3(fl)  s_pfCMGetAPI2( "GWClientBeginResolveBlockDriverAddress3", (RTS_VOID_FCTPTR *)&pfGWClientBeginResolveBlockDriverAddress3, (fl), 0, 0)
	#define CAL_GWClientBeginResolveBlockDriverAddress3  pfGWClientBeginResolveBlockDriverAddress3
	#define CHK_GWClientBeginResolveBlockDriverAddress3  (pfGWClientBeginResolveBlockDriverAddress3 != NULL)
	#define EXP_GWClientBeginResolveBlockDriverAddress3   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginResolveBlockDriverAddress3", (RTS_UINTPTR)GWClientBeginResolveBlockDriverAddress3, 0, 0) 
#endif

RTS_RESULT CDECL GWClientAdjustResolveBlockDriverAddressTimeout(RTS_UI32 dwRemainingTimeout, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTADJUSTRESOLVEBLOCKDRIVERADDRESSTIMEOUT) (RTS_UI32 dwRemainingTimeout, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTADJUSTRESOLVEBLOCKDRIVERADDRESSTIMEOUT_NOTIMPLEMENTED)
	#define USE_GWClientAdjustResolveBlockDriverAddressTimeout
	#define EXT_GWClientAdjustResolveBlockDriverAddressTimeout
	#define GET_GWClientAdjustResolveBlockDriverAddressTimeout(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientAdjustResolveBlockDriverAddressTimeout(p0,p1) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientAdjustResolveBlockDriverAddressTimeout  FALSE
	#define EXP_GWClientAdjustResolveBlockDriverAddressTimeout  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientAdjustResolveBlockDriverAddressTimeout
	#define EXT_GWClientAdjustResolveBlockDriverAddressTimeout
	#define GET_GWClientAdjustResolveBlockDriverAddressTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustResolveBlockDriverAddressTimeout" ) 
	#define CAL_GWClientAdjustResolveBlockDriverAddressTimeout  GWClientAdjustResolveBlockDriverAddressTimeout
	#define CHK_GWClientAdjustResolveBlockDriverAddressTimeout  TRUE
	#define EXP_GWClientAdjustResolveBlockDriverAddressTimeout  CAL_CMEXPAPI( "GWClientAdjustResolveBlockDriverAddressTimeout" ) 
	#define EXTREF_ITF_GWClientAdjustResolveBlockDriverAddressTimeout {(RTS_VOID_FCTPTR)GWClientAdjustResolveBlockDriverAddressTimeout, "GWClientAdjustResolveBlockDriverAddressTimeout", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientAdjustResolveBlockDriverAddressTimeout
	#define EXT_GWClientAdjustResolveBlockDriverAddressTimeout
	#define GET_GWClientAdjustResolveBlockDriverAddressTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustResolveBlockDriverAddressTimeout" ) 
	#define CAL_GWClientAdjustResolveBlockDriverAddressTimeout  GWClientAdjustResolveBlockDriverAddressTimeout
	#define CHK_GWClientAdjustResolveBlockDriverAddressTimeout  TRUE
	#define EXP_GWClientAdjustResolveBlockDriverAddressTimeout  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientAdjustResolveBlockDriverAddressTimeout", (RTS_UINTPTR)GWClientAdjustResolveBlockDriverAddressTimeout, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientAdjustResolveBlockDriverAddressTimeout
	#define EXT_CmpGwClientGWClientAdjustResolveBlockDriverAddressTimeout
	#define GET_CmpGwClientGWClientAdjustResolveBlockDriverAddressTimeout  ERR_OK
	#define CAL_CmpGwClientGWClientAdjustResolveBlockDriverAddressTimeout  GWClientAdjustResolveBlockDriverAddressTimeout
	#define CHK_CmpGwClientGWClientAdjustResolveBlockDriverAddressTimeout  TRUE
	#define EXP_CmpGwClientGWClientAdjustResolveBlockDriverAddressTimeout  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientAdjustResolveBlockDriverAddressTimeout
	#define EXT_GWClientAdjustResolveBlockDriverAddressTimeout
	#define GET_GWClientAdjustResolveBlockDriverAddressTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustResolveBlockDriverAddressTimeout" ) 
	#define CAL_GWClientAdjustResolveBlockDriverAddressTimeout  GWClientAdjustResolveBlockDriverAddressTimeout
	#define CHK_GWClientAdjustResolveBlockDriverAddressTimeout  TRUE
	#define EXP_GWClientAdjustResolveBlockDriverAddressTimeout  CAL_CMEXPAPI( "GWClientAdjustResolveBlockDriverAddressTimeout" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientAdjustResolveBlockDriverAddressTimeout  PFGWCLIENTADJUSTRESOLVEBLOCKDRIVERADDRESSTIMEOUT pfGWClientAdjustResolveBlockDriverAddressTimeout;
	#define EXT_GWClientAdjustResolveBlockDriverAddressTimeout  extern PFGWCLIENTADJUSTRESOLVEBLOCKDRIVERADDRESSTIMEOUT pfGWClientAdjustResolveBlockDriverAddressTimeout;
	#define GET_GWClientAdjustResolveBlockDriverAddressTimeout(fl)  s_pfCMGetAPI2( "GWClientAdjustResolveBlockDriverAddressTimeout", (RTS_VOID_FCTPTR *)&pfGWClientAdjustResolveBlockDriverAddressTimeout, (fl), 0, 0)
	#define CAL_GWClientAdjustResolveBlockDriverAddressTimeout  pfGWClientAdjustResolveBlockDriverAddressTimeout
	#define CHK_GWClientAdjustResolveBlockDriverAddressTimeout  (pfGWClientAdjustResolveBlockDriverAddressTimeout != NULL)
	#define EXP_GWClientAdjustResolveBlockDriverAddressTimeout   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientAdjustResolveBlockDriverAddressTimeout", (RTS_UINTPTR)GWClientAdjustResolveBlockDriverAddressTimeout, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEndResolveBlockDriverAddress(ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTENDRESOLVEBLOCKDRIVERADDRESS) (ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENDRESOLVEBLOCKDRIVERADDRESS_NOTIMPLEMENTED)
	#define USE_GWClientEndResolveBlockDriverAddress
	#define EXT_GWClientEndResolveBlockDriverAddress
	#define GET_GWClientEndResolveBlockDriverAddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEndResolveBlockDriverAddress(p0) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEndResolveBlockDriverAddress  FALSE
	#define EXP_GWClientEndResolveBlockDriverAddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEndResolveBlockDriverAddress
	#define EXT_GWClientEndResolveBlockDriverAddress
	#define GET_GWClientEndResolveBlockDriverAddress(fl)  CAL_CMGETAPI( "GWClientEndResolveBlockDriverAddress" ) 
	#define CAL_GWClientEndResolveBlockDriverAddress  GWClientEndResolveBlockDriverAddress
	#define CHK_GWClientEndResolveBlockDriverAddress  TRUE
	#define EXP_GWClientEndResolveBlockDriverAddress  CAL_CMEXPAPI( "GWClientEndResolveBlockDriverAddress" ) 
	#define EXTREF_ITF_GWClientEndResolveBlockDriverAddress {(RTS_VOID_FCTPTR)GWClientEndResolveBlockDriverAddress, "GWClientEndResolveBlockDriverAddress", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEndResolveBlockDriverAddress
	#define EXT_GWClientEndResolveBlockDriverAddress
	#define GET_GWClientEndResolveBlockDriverAddress(fl)  CAL_CMGETAPI( "GWClientEndResolveBlockDriverAddress" ) 
	#define CAL_GWClientEndResolveBlockDriverAddress  GWClientEndResolveBlockDriverAddress
	#define CHK_GWClientEndResolveBlockDriverAddress  TRUE
	#define EXP_GWClientEndResolveBlockDriverAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndResolveBlockDriverAddress", (RTS_UINTPTR)GWClientEndResolveBlockDriverAddress, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEndResolveBlockDriverAddress
	#define EXT_CmpGwClientGWClientEndResolveBlockDriverAddress
	#define GET_CmpGwClientGWClientEndResolveBlockDriverAddress  ERR_OK
	#define CAL_CmpGwClientGWClientEndResolveBlockDriverAddress  GWClientEndResolveBlockDriverAddress
	#define CHK_CmpGwClientGWClientEndResolveBlockDriverAddress  TRUE
	#define EXP_CmpGwClientGWClientEndResolveBlockDriverAddress  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEndResolveBlockDriverAddress
	#define EXT_GWClientEndResolveBlockDriverAddress
	#define GET_GWClientEndResolveBlockDriverAddress(fl)  CAL_CMGETAPI( "GWClientEndResolveBlockDriverAddress" ) 
	#define CAL_GWClientEndResolveBlockDriverAddress  GWClientEndResolveBlockDriverAddress
	#define CHK_GWClientEndResolveBlockDriverAddress  TRUE
	#define EXP_GWClientEndResolveBlockDriverAddress  CAL_CMEXPAPI( "GWClientEndResolveBlockDriverAddress" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEndResolveBlockDriverAddress  PFGWCLIENTENDRESOLVEBLOCKDRIVERADDRESS pfGWClientEndResolveBlockDriverAddress;
	#define EXT_GWClientEndResolveBlockDriverAddress  extern PFGWCLIENTENDRESOLVEBLOCKDRIVERADDRESS pfGWClientEndResolveBlockDriverAddress;
	#define GET_GWClientEndResolveBlockDriverAddress(fl)  s_pfCMGetAPI2( "GWClientEndResolveBlockDriverAddress", (RTS_VOID_FCTPTR *)&pfGWClientEndResolveBlockDriverAddress, (fl), 0, 0)
	#define CAL_GWClientEndResolveBlockDriverAddress  pfGWClientEndResolveBlockDriverAddress
	#define CHK_GWClientEndResolveBlockDriverAddress  (pfGWClientEndResolveBlockDriverAddress != NULL)
	#define EXP_GWClientEndResolveBlockDriverAddress   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndResolveBlockDriverAddress", (RTS_UINTPTR)GWClientEndResolveBlockDriverAddress, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginResolveAddress(RTS_UI8 byAddrComponentLength, RTS_UI8 *pbyAddress, RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK2 pfCallback, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINRESOLVEADDRESS) (RTS_UI8 byAddrComponentLength, RTS_UI8 *pbyAddress, RTS_HANDLE hGateway, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK2 pfCallback, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINRESOLVEADDRESS_NOTIMPLEMENTED)
	#define USE_GWClientBeginResolveAddress
	#define EXT_GWClientBeginResolveAddress
	#define GET_GWClientBeginResolveAddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginResolveAddress(p0,p1,p2,p3,p4,p5) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginResolveAddress  FALSE
	#define EXP_GWClientBeginResolveAddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginResolveAddress
	#define EXT_GWClientBeginResolveAddress
	#define GET_GWClientBeginResolveAddress(fl)  CAL_CMGETAPI( "GWClientBeginResolveAddress" ) 
	#define CAL_GWClientBeginResolveAddress  GWClientBeginResolveAddress
	#define CHK_GWClientBeginResolveAddress  TRUE
	#define EXP_GWClientBeginResolveAddress  CAL_CMEXPAPI( "GWClientBeginResolveAddress" ) 
	#define EXTREF_ITF_GWClientBeginResolveAddress {(RTS_VOID_FCTPTR)GWClientBeginResolveAddress, "GWClientBeginResolveAddress", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginResolveAddress
	#define EXT_GWClientBeginResolveAddress
	#define GET_GWClientBeginResolveAddress(fl)  CAL_CMGETAPI( "GWClientBeginResolveAddress" ) 
	#define CAL_GWClientBeginResolveAddress  GWClientBeginResolveAddress
	#define CHK_GWClientBeginResolveAddress  TRUE
	#define EXP_GWClientBeginResolveAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginResolveAddress", (RTS_UINTPTR)GWClientBeginResolveAddress, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginResolveAddress
	#define EXT_CmpGwClientGWClientBeginResolveAddress
	#define GET_CmpGwClientGWClientBeginResolveAddress  ERR_OK
	#define CAL_CmpGwClientGWClientBeginResolveAddress  GWClientBeginResolveAddress
	#define CHK_CmpGwClientGWClientBeginResolveAddress  TRUE
	#define EXP_CmpGwClientGWClientBeginResolveAddress  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginResolveAddress
	#define EXT_GWClientBeginResolveAddress
	#define GET_GWClientBeginResolveAddress(fl)  CAL_CMGETAPI( "GWClientBeginResolveAddress" ) 
	#define CAL_GWClientBeginResolveAddress  GWClientBeginResolveAddress
	#define CHK_GWClientBeginResolveAddress  TRUE
	#define EXP_GWClientBeginResolveAddress  CAL_CMEXPAPI( "GWClientBeginResolveAddress" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginResolveAddress  PFGWCLIENTBEGINRESOLVEADDRESS pfGWClientBeginResolveAddress;
	#define EXT_GWClientBeginResolveAddress  extern PFGWCLIENTBEGINRESOLVEADDRESS pfGWClientBeginResolveAddress;
	#define GET_GWClientBeginResolveAddress(fl)  s_pfCMGetAPI2( "GWClientBeginResolveAddress", (RTS_VOID_FCTPTR *)&pfGWClientBeginResolveAddress, (fl), 0, 0)
	#define CAL_GWClientBeginResolveAddress  pfGWClientBeginResolveAddress
	#define CHK_GWClientBeginResolveAddress  (pfGWClientBeginResolveAddress != NULL)
	#define EXP_GWClientBeginResolveAddress   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginResolveAddress", (RTS_UINTPTR)GWClientBeginResolveAddress, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginResolveAddress3(RTS_UI8 byAddrComponentLength, RTS_UI8 *pbyAddress, RTS_HANDLE hGateway, RTS_UI32 dwTimeout, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK3 pfCallback3, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINRESOLVEADDRESS3) (RTS_UI8 byAddrComponentLength, RTS_UI8 *pbyAddress, RTS_HANDLE hGateway, RTS_UI32 dwTimeout, RTS_UINTPTR dwUser, PFNODEINFOCALLBACK3 pfCallback3, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINRESOLVEADDRESS3_NOTIMPLEMENTED)
	#define USE_GWClientBeginResolveAddress3
	#define EXT_GWClientBeginResolveAddress3
	#define GET_GWClientBeginResolveAddress3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginResolveAddress3(p0,p1,p2,p3,p4,p5,p6) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginResolveAddress3  FALSE
	#define EXP_GWClientBeginResolveAddress3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginResolveAddress3
	#define EXT_GWClientBeginResolveAddress3
	#define GET_GWClientBeginResolveAddress3(fl)  CAL_CMGETAPI( "GWClientBeginResolveAddress3" ) 
	#define CAL_GWClientBeginResolveAddress3  GWClientBeginResolveAddress3
	#define CHK_GWClientBeginResolveAddress3  TRUE
	#define EXP_GWClientBeginResolveAddress3  CAL_CMEXPAPI( "GWClientBeginResolveAddress3" ) 
	#define EXTREF_ITF_GWClientBeginResolveAddress3 {(RTS_VOID_FCTPTR)GWClientBeginResolveAddress3, "GWClientBeginResolveAddress3", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginResolveAddress3
	#define EXT_GWClientBeginResolveAddress3
	#define GET_GWClientBeginResolveAddress3(fl)  CAL_CMGETAPI( "GWClientBeginResolveAddress3" ) 
	#define CAL_GWClientBeginResolveAddress3  GWClientBeginResolveAddress3
	#define CHK_GWClientBeginResolveAddress3  TRUE
	#define EXP_GWClientBeginResolveAddress3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginResolveAddress3", (RTS_UINTPTR)GWClientBeginResolveAddress3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginResolveAddress3
	#define EXT_CmpGwClientGWClientBeginResolveAddress3
	#define GET_CmpGwClientGWClientBeginResolveAddress3  ERR_OK
	#define CAL_CmpGwClientGWClientBeginResolveAddress3  GWClientBeginResolveAddress3
	#define CHK_CmpGwClientGWClientBeginResolveAddress3  TRUE
	#define EXP_CmpGwClientGWClientBeginResolveAddress3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginResolveAddress3
	#define EXT_GWClientBeginResolveAddress3
	#define GET_GWClientBeginResolveAddress3(fl)  CAL_CMGETAPI( "GWClientBeginResolveAddress3" ) 
	#define CAL_GWClientBeginResolveAddress3  GWClientBeginResolveAddress3
	#define CHK_GWClientBeginResolveAddress3  TRUE
	#define EXP_GWClientBeginResolveAddress3  CAL_CMEXPAPI( "GWClientBeginResolveAddress3" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginResolveAddress3  PFGWCLIENTBEGINRESOLVEADDRESS3 pfGWClientBeginResolveAddress3;
	#define EXT_GWClientBeginResolveAddress3  extern PFGWCLIENTBEGINRESOLVEADDRESS3 pfGWClientBeginResolveAddress3;
	#define GET_GWClientBeginResolveAddress3(fl)  s_pfCMGetAPI2( "GWClientBeginResolveAddress3", (RTS_VOID_FCTPTR *)&pfGWClientBeginResolveAddress3, (fl), 0, 0)
	#define CAL_GWClientBeginResolveAddress3  pfGWClientBeginResolveAddress3
	#define CHK_GWClientBeginResolveAddress3  (pfGWClientBeginResolveAddress3 != NULL)
	#define EXP_GWClientBeginResolveAddress3   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginResolveAddress3", (RTS_UINTPTR)GWClientBeginResolveAddress3, 0, 0) 
#endif

RTS_RESULT CDECL GWClientAdjustResolveAddressTimeout(RTS_UI32 dwRemainingTimeout, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTADJUSTRESOLVEADDRESSTIMEOUT) (RTS_UI32 dwRemainingTimeout, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTADJUSTRESOLVEADDRESSTIMEOUT_NOTIMPLEMENTED)
	#define USE_GWClientAdjustResolveAddressTimeout
	#define EXT_GWClientAdjustResolveAddressTimeout
	#define GET_GWClientAdjustResolveAddressTimeout(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientAdjustResolveAddressTimeout(p0,p1) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientAdjustResolveAddressTimeout  FALSE
	#define EXP_GWClientAdjustResolveAddressTimeout  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientAdjustResolveAddressTimeout
	#define EXT_GWClientAdjustResolveAddressTimeout
	#define GET_GWClientAdjustResolveAddressTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustResolveAddressTimeout" ) 
	#define CAL_GWClientAdjustResolveAddressTimeout  GWClientAdjustResolveAddressTimeout
	#define CHK_GWClientAdjustResolveAddressTimeout  TRUE
	#define EXP_GWClientAdjustResolveAddressTimeout  CAL_CMEXPAPI( "GWClientAdjustResolveAddressTimeout" ) 
	#define EXTREF_ITF_GWClientAdjustResolveAddressTimeout {(RTS_VOID_FCTPTR)GWClientAdjustResolveAddressTimeout, "GWClientAdjustResolveAddressTimeout", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientAdjustResolveAddressTimeout
	#define EXT_GWClientAdjustResolveAddressTimeout
	#define GET_GWClientAdjustResolveAddressTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustResolveAddressTimeout" ) 
	#define CAL_GWClientAdjustResolveAddressTimeout  GWClientAdjustResolveAddressTimeout
	#define CHK_GWClientAdjustResolveAddressTimeout  TRUE
	#define EXP_GWClientAdjustResolveAddressTimeout  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientAdjustResolveAddressTimeout", (RTS_UINTPTR)GWClientAdjustResolveAddressTimeout, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientAdjustResolveAddressTimeout
	#define EXT_CmpGwClientGWClientAdjustResolveAddressTimeout
	#define GET_CmpGwClientGWClientAdjustResolveAddressTimeout  ERR_OK
	#define CAL_CmpGwClientGWClientAdjustResolveAddressTimeout  GWClientAdjustResolveAddressTimeout
	#define CHK_CmpGwClientGWClientAdjustResolveAddressTimeout  TRUE
	#define EXP_CmpGwClientGWClientAdjustResolveAddressTimeout  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientAdjustResolveAddressTimeout
	#define EXT_GWClientAdjustResolveAddressTimeout
	#define GET_GWClientAdjustResolveAddressTimeout(fl)  CAL_CMGETAPI( "GWClientAdjustResolveAddressTimeout" ) 
	#define CAL_GWClientAdjustResolveAddressTimeout  GWClientAdjustResolveAddressTimeout
	#define CHK_GWClientAdjustResolveAddressTimeout  TRUE
	#define EXP_GWClientAdjustResolveAddressTimeout  CAL_CMEXPAPI( "GWClientAdjustResolveAddressTimeout" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientAdjustResolveAddressTimeout  PFGWCLIENTADJUSTRESOLVEADDRESSTIMEOUT pfGWClientAdjustResolveAddressTimeout;
	#define EXT_GWClientAdjustResolveAddressTimeout  extern PFGWCLIENTADJUSTRESOLVEADDRESSTIMEOUT pfGWClientAdjustResolveAddressTimeout;
	#define GET_GWClientAdjustResolveAddressTimeout(fl)  s_pfCMGetAPI2( "GWClientAdjustResolveAddressTimeout", (RTS_VOID_FCTPTR *)&pfGWClientAdjustResolveAddressTimeout, (fl), 0, 0)
	#define CAL_GWClientAdjustResolveAddressTimeout  pfGWClientAdjustResolveAddressTimeout
	#define CHK_GWClientAdjustResolveAddressTimeout  (pfGWClientAdjustResolveAddressTimeout != NULL)
	#define EXP_GWClientAdjustResolveAddressTimeout   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientAdjustResolveAddressTimeout", (RTS_UINTPTR)GWClientAdjustResolveAddressTimeout, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEndResolveAddress(ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTENDRESOLVEADDRESS) (ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENDRESOLVEADDRESS_NOTIMPLEMENTED)
	#define USE_GWClientEndResolveAddress
	#define EXT_GWClientEndResolveAddress
	#define GET_GWClientEndResolveAddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEndResolveAddress(p0) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEndResolveAddress  FALSE
	#define EXP_GWClientEndResolveAddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEndResolveAddress
	#define EXT_GWClientEndResolveAddress
	#define GET_GWClientEndResolveAddress(fl)  CAL_CMGETAPI( "GWClientEndResolveAddress" ) 
	#define CAL_GWClientEndResolveAddress  GWClientEndResolveAddress
	#define CHK_GWClientEndResolveAddress  TRUE
	#define EXP_GWClientEndResolveAddress  CAL_CMEXPAPI( "GWClientEndResolveAddress" ) 
	#define EXTREF_ITF_GWClientEndResolveAddress {(RTS_VOID_FCTPTR)GWClientEndResolveAddress, "GWClientEndResolveAddress", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEndResolveAddress
	#define EXT_GWClientEndResolveAddress
	#define GET_GWClientEndResolveAddress(fl)  CAL_CMGETAPI( "GWClientEndResolveAddress" ) 
	#define CAL_GWClientEndResolveAddress  GWClientEndResolveAddress
	#define CHK_GWClientEndResolveAddress  TRUE
	#define EXP_GWClientEndResolveAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndResolveAddress", (RTS_UINTPTR)GWClientEndResolveAddress, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEndResolveAddress
	#define EXT_CmpGwClientGWClientEndResolveAddress
	#define GET_CmpGwClientGWClientEndResolveAddress  ERR_OK
	#define CAL_CmpGwClientGWClientEndResolveAddress  GWClientEndResolveAddress
	#define CHK_CmpGwClientGWClientEndResolveAddress  TRUE
	#define EXP_CmpGwClientGWClientEndResolveAddress  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEndResolveAddress
	#define EXT_GWClientEndResolveAddress
	#define GET_GWClientEndResolveAddress(fl)  CAL_CMGETAPI( "GWClientEndResolveAddress" ) 
	#define CAL_GWClientEndResolveAddress  GWClientEndResolveAddress
	#define CHK_GWClientEndResolveAddress  TRUE
	#define EXP_GWClientEndResolveAddress  CAL_CMEXPAPI( "GWClientEndResolveAddress" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEndResolveAddress  PFGWCLIENTENDRESOLVEADDRESS pfGWClientEndResolveAddress;
	#define EXT_GWClientEndResolveAddress  extern PFGWCLIENTENDRESOLVEADDRESS pfGWClientEndResolveAddress;
	#define GET_GWClientEndResolveAddress(fl)  s_pfCMGetAPI2( "GWClientEndResolveAddress", (RTS_VOID_FCTPTR *)&pfGWClientEndResolveAddress, (fl), 0, 0)
	#define CAL_GWClientEndResolveAddress  pfGWClientEndResolveAddress
	#define CHK_GWClientEndResolveAddress  (pfGWClientEndResolveAddress != NULL)
	#define EXP_GWClientEndResolveAddress   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndResolveAddress", (RTS_UINTPTR)GWClientEndResolveAddress, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginSendRequest(RTS_HANDLE hGateway, RTS_HANDLE hChannel, PROTOCOL_DATA_UNIT pduReq, PROTOCOL_DATA_UNIT *ppduResp, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINSENDREQUEST) (RTS_HANDLE hGateway, RTS_HANDLE hChannel, PROTOCOL_DATA_UNIT pduReq, PROTOCOL_DATA_UNIT *ppduResp, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINSENDREQUEST_NOTIMPLEMENTED)
	#define USE_GWClientBeginSendRequest
	#define EXT_GWClientBeginSendRequest
	#define GET_GWClientBeginSendRequest(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginSendRequest(p0,p1,p2,p3,p4) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginSendRequest  FALSE
	#define EXP_GWClientBeginSendRequest  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginSendRequest
	#define EXT_GWClientBeginSendRequest
	#define GET_GWClientBeginSendRequest(fl)  CAL_CMGETAPI( "GWClientBeginSendRequest" ) 
	#define CAL_GWClientBeginSendRequest  GWClientBeginSendRequest
	#define CHK_GWClientBeginSendRequest  TRUE
	#define EXP_GWClientBeginSendRequest  CAL_CMEXPAPI( "GWClientBeginSendRequest" ) 
	#define EXTREF_ITF_GWClientBeginSendRequest {(RTS_VOID_FCTPTR)GWClientBeginSendRequest, "GWClientBeginSendRequest", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginSendRequest
	#define EXT_GWClientBeginSendRequest
	#define GET_GWClientBeginSendRequest(fl)  CAL_CMGETAPI( "GWClientBeginSendRequest" ) 
	#define CAL_GWClientBeginSendRequest  GWClientBeginSendRequest
	#define CHK_GWClientBeginSendRequest  TRUE
	#define EXP_GWClientBeginSendRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginSendRequest", (RTS_UINTPTR)GWClientBeginSendRequest, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginSendRequest
	#define EXT_CmpGwClientGWClientBeginSendRequest
	#define GET_CmpGwClientGWClientBeginSendRequest  ERR_OK
	#define CAL_CmpGwClientGWClientBeginSendRequest  GWClientBeginSendRequest
	#define CHK_CmpGwClientGWClientBeginSendRequest  TRUE
	#define EXP_CmpGwClientGWClientBeginSendRequest  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginSendRequest
	#define EXT_GWClientBeginSendRequest
	#define GET_GWClientBeginSendRequest(fl)  CAL_CMGETAPI( "GWClientBeginSendRequest" ) 
	#define CAL_GWClientBeginSendRequest  GWClientBeginSendRequest
	#define CHK_GWClientBeginSendRequest  TRUE
	#define EXP_GWClientBeginSendRequest  CAL_CMEXPAPI( "GWClientBeginSendRequest" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginSendRequest  PFGWCLIENTBEGINSENDREQUEST pfGWClientBeginSendRequest;
	#define EXT_GWClientBeginSendRequest  extern PFGWCLIENTBEGINSENDREQUEST pfGWClientBeginSendRequest;
	#define GET_GWClientBeginSendRequest(fl)  s_pfCMGetAPI2( "GWClientBeginSendRequest", (RTS_VOID_FCTPTR *)&pfGWClientBeginSendRequest, (fl), 0, 0)
	#define CAL_GWClientBeginSendRequest  pfGWClientBeginSendRequest
	#define CHK_GWClientBeginSendRequest  (pfGWClientBeginSendRequest != NULL)
	#define EXP_GWClientBeginSendRequest   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginSendRequest", (RTS_UINTPTR)GWClientBeginSendRequest, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEndSendRequest(ASYNCRESULT *pAsyncRes, PROTOCOL_DATA_UNIT *ppduResp);
typedef RTS_RESULT (CDECL * PFGWCLIENTENDSENDREQUEST) (ASYNCRESULT *pAsyncRes, PROTOCOL_DATA_UNIT *ppduResp);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENDSENDREQUEST_NOTIMPLEMENTED)
	#define USE_GWClientEndSendRequest
	#define EXT_GWClientEndSendRequest
	#define GET_GWClientEndSendRequest(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEndSendRequest(p0,p1) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEndSendRequest  FALSE
	#define EXP_GWClientEndSendRequest  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEndSendRequest
	#define EXT_GWClientEndSendRequest
	#define GET_GWClientEndSendRequest(fl)  CAL_CMGETAPI( "GWClientEndSendRequest" ) 
	#define CAL_GWClientEndSendRequest  GWClientEndSendRequest
	#define CHK_GWClientEndSendRequest  TRUE
	#define EXP_GWClientEndSendRequest  CAL_CMEXPAPI( "GWClientEndSendRequest" ) 
	#define EXTREF_ITF_GWClientEndSendRequest {(RTS_VOID_FCTPTR)GWClientEndSendRequest, "GWClientEndSendRequest", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEndSendRequest
	#define EXT_GWClientEndSendRequest
	#define GET_GWClientEndSendRequest(fl)  CAL_CMGETAPI( "GWClientEndSendRequest" ) 
	#define CAL_GWClientEndSendRequest  GWClientEndSendRequest
	#define CHK_GWClientEndSendRequest  TRUE
	#define EXP_GWClientEndSendRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndSendRequest", (RTS_UINTPTR)GWClientEndSendRequest, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEndSendRequest
	#define EXT_CmpGwClientGWClientEndSendRequest
	#define GET_CmpGwClientGWClientEndSendRequest  ERR_OK
	#define CAL_CmpGwClientGWClientEndSendRequest  GWClientEndSendRequest
	#define CHK_CmpGwClientGWClientEndSendRequest  TRUE
	#define EXP_CmpGwClientGWClientEndSendRequest  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEndSendRequest
	#define EXT_GWClientEndSendRequest
	#define GET_GWClientEndSendRequest(fl)  CAL_CMGETAPI( "GWClientEndSendRequest" ) 
	#define CAL_GWClientEndSendRequest  GWClientEndSendRequest
	#define CHK_GWClientEndSendRequest  TRUE
	#define EXP_GWClientEndSendRequest  CAL_CMEXPAPI( "GWClientEndSendRequest" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEndSendRequest  PFGWCLIENTENDSENDREQUEST pfGWClientEndSendRequest;
	#define EXT_GWClientEndSendRequest  extern PFGWCLIENTENDSENDREQUEST pfGWClientEndSendRequest;
	#define GET_GWClientEndSendRequest(fl)  s_pfCMGetAPI2( "GWClientEndSendRequest", (RTS_VOID_FCTPTR *)&pfGWClientEndSendRequest, (fl), 0, 0)
	#define CAL_GWClientEndSendRequest  pfGWClientEndSendRequest
	#define CHK_GWClientEndSendRequest  (pfGWClientEndSendRequest != NULL)
	#define EXP_GWClientEndSendRequest   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndSendRequest", (RTS_UINTPTR)GWClientEndSendRequest, 0, 0) 
#endif

RTS_RESULT CDECL GWClientGetRequestStatus(ASYNCRESULT *pAsyncRes, RTS_UI16 *pwStatus, RTS_UI8 *pbyScaling, RTS_I32 *pnItemsComplete, RTS_I32 *pnTotalItems);
typedef RTS_RESULT (CDECL * PFGWCLIENTGETREQUESTSTATUS) (ASYNCRESULT *pAsyncRes, RTS_UI16 *pwStatus, RTS_UI8 *pbyScaling, RTS_I32 *pnItemsComplete, RTS_I32 *pnTotalItems);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTGETREQUESTSTATUS_NOTIMPLEMENTED)
	#define USE_GWClientGetRequestStatus
	#define EXT_GWClientGetRequestStatus
	#define GET_GWClientGetRequestStatus(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientGetRequestStatus(p0,p1,p2,p3,p4) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientGetRequestStatus  FALSE
	#define EXP_GWClientGetRequestStatus  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientGetRequestStatus
	#define EXT_GWClientGetRequestStatus
	#define GET_GWClientGetRequestStatus(fl)  CAL_CMGETAPI( "GWClientGetRequestStatus" ) 
	#define CAL_GWClientGetRequestStatus  GWClientGetRequestStatus
	#define CHK_GWClientGetRequestStatus  TRUE
	#define EXP_GWClientGetRequestStatus  CAL_CMEXPAPI( "GWClientGetRequestStatus" ) 
	#define EXTREF_ITF_GWClientGetRequestStatus {(RTS_VOID_FCTPTR)GWClientGetRequestStatus, "GWClientGetRequestStatus", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientGetRequestStatus
	#define EXT_GWClientGetRequestStatus
	#define GET_GWClientGetRequestStatus(fl)  CAL_CMGETAPI( "GWClientGetRequestStatus" ) 
	#define CAL_GWClientGetRequestStatus  GWClientGetRequestStatus
	#define CHK_GWClientGetRequestStatus  TRUE
	#define EXP_GWClientGetRequestStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientGetRequestStatus", (RTS_UINTPTR)GWClientGetRequestStatus, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientGetRequestStatus
	#define EXT_CmpGwClientGWClientGetRequestStatus
	#define GET_CmpGwClientGWClientGetRequestStatus  ERR_OK
	#define CAL_CmpGwClientGWClientGetRequestStatus  GWClientGetRequestStatus
	#define CHK_CmpGwClientGWClientGetRequestStatus  TRUE
	#define EXP_CmpGwClientGWClientGetRequestStatus  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientGetRequestStatus
	#define EXT_GWClientGetRequestStatus
	#define GET_GWClientGetRequestStatus(fl)  CAL_CMGETAPI( "GWClientGetRequestStatus" ) 
	#define CAL_GWClientGetRequestStatus  GWClientGetRequestStatus
	#define CHK_GWClientGetRequestStatus  TRUE
	#define EXP_GWClientGetRequestStatus  CAL_CMEXPAPI( "GWClientGetRequestStatus" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientGetRequestStatus  PFGWCLIENTGETREQUESTSTATUS pfGWClientGetRequestStatus;
	#define EXT_GWClientGetRequestStatus  extern PFGWCLIENTGETREQUESTSTATUS pfGWClientGetRequestStatus;
	#define GET_GWClientGetRequestStatus(fl)  s_pfCMGetAPI2( "GWClientGetRequestStatus", (RTS_VOID_FCTPTR *)&pfGWClientGetRequestStatus, (fl), 0, 0)
	#define CAL_GWClientGetRequestStatus  pfGWClientGetRequestStatus
	#define CHK_GWClientGetRequestStatus  (pfGWClientGetRequestStatus != NULL)
	#define EXP_GWClientGetRequestStatus   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientGetRequestStatus", (RTS_UINTPTR)GWClientGetRequestStatus, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginOpenChannel(RTS_HANDLE hGateway, NODEADDRESS_OLD addrTo, CHANNELQOS qosChannel, RTS_HANDLE *phChannel, RTS_UI32 *pdwCommBufferSize, RTS_UI8 *pbBigEndianTarget, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINOPENCHANNEL) (RTS_HANDLE hGateway, NODEADDRESS_OLD addrTo, CHANNELQOS qosChannel, RTS_HANDLE *phChannel, RTS_UI32 *pdwCommBufferSize, RTS_UI8 *pbBigEndianTarget, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINOPENCHANNEL_NOTIMPLEMENTED)
	#define USE_GWClientBeginOpenChannel
	#define EXT_GWClientBeginOpenChannel
	#define GET_GWClientBeginOpenChannel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginOpenChannel(p0,p1,p2,p3,p4,p5,p6) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginOpenChannel  FALSE
	#define EXP_GWClientBeginOpenChannel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginOpenChannel
	#define EXT_GWClientBeginOpenChannel
	#define GET_GWClientBeginOpenChannel(fl)  CAL_CMGETAPI( "GWClientBeginOpenChannel" ) 
	#define CAL_GWClientBeginOpenChannel  GWClientBeginOpenChannel
	#define CHK_GWClientBeginOpenChannel  TRUE
	#define EXP_GWClientBeginOpenChannel  CAL_CMEXPAPI( "GWClientBeginOpenChannel" ) 
	#define EXTREF_ITF_GWClientBeginOpenChannel {(RTS_VOID_FCTPTR)GWClientBeginOpenChannel, "GWClientBeginOpenChannel", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginOpenChannel
	#define EXT_GWClientBeginOpenChannel
	#define GET_GWClientBeginOpenChannel(fl)  CAL_CMGETAPI( "GWClientBeginOpenChannel" ) 
	#define CAL_GWClientBeginOpenChannel  GWClientBeginOpenChannel
	#define CHK_GWClientBeginOpenChannel  TRUE
	#define EXP_GWClientBeginOpenChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginOpenChannel", (RTS_UINTPTR)GWClientBeginOpenChannel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginOpenChannel
	#define EXT_CmpGwClientGWClientBeginOpenChannel
	#define GET_CmpGwClientGWClientBeginOpenChannel  ERR_OK
	#define CAL_CmpGwClientGWClientBeginOpenChannel  GWClientBeginOpenChannel
	#define CHK_CmpGwClientGWClientBeginOpenChannel  TRUE
	#define EXP_CmpGwClientGWClientBeginOpenChannel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginOpenChannel
	#define EXT_GWClientBeginOpenChannel
	#define GET_GWClientBeginOpenChannel(fl)  CAL_CMGETAPI( "GWClientBeginOpenChannel" ) 
	#define CAL_GWClientBeginOpenChannel  GWClientBeginOpenChannel
	#define CHK_GWClientBeginOpenChannel  TRUE
	#define EXP_GWClientBeginOpenChannel  CAL_CMEXPAPI( "GWClientBeginOpenChannel" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginOpenChannel  PFGWCLIENTBEGINOPENCHANNEL pfGWClientBeginOpenChannel;
	#define EXT_GWClientBeginOpenChannel  extern PFGWCLIENTBEGINOPENCHANNEL pfGWClientBeginOpenChannel;
	#define GET_GWClientBeginOpenChannel(fl)  s_pfCMGetAPI2( "GWClientBeginOpenChannel", (RTS_VOID_FCTPTR *)&pfGWClientBeginOpenChannel, (fl), 0, 0)
	#define CAL_GWClientBeginOpenChannel  pfGWClientBeginOpenChannel
	#define CHK_GWClientBeginOpenChannel  (pfGWClientBeginOpenChannel != NULL)
	#define EXP_GWClientBeginOpenChannel   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginOpenChannel", (RTS_UINTPTR)GWClientBeginOpenChannel, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginOpenChannel2(RTS_HANDLE hGateway, PEERADDRESS addrTo, CHANNELQOS qosChannel, RTS_HANDLE *phChannel, RTS_UI32 *pdwCommBufferSize, RTS_UI8 *pbBigEndianTarget, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINOPENCHANNEL2) (RTS_HANDLE hGateway, PEERADDRESS addrTo, CHANNELQOS qosChannel, RTS_HANDLE *phChannel, RTS_UI32 *pdwCommBufferSize, RTS_UI8 *pbBigEndianTarget, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINOPENCHANNEL2_NOTIMPLEMENTED)
	#define USE_GWClientBeginOpenChannel2
	#define EXT_GWClientBeginOpenChannel2
	#define GET_GWClientBeginOpenChannel2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginOpenChannel2(p0,p1,p2,p3,p4,p5,p6) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginOpenChannel2  FALSE
	#define EXP_GWClientBeginOpenChannel2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginOpenChannel2
	#define EXT_GWClientBeginOpenChannel2
	#define GET_GWClientBeginOpenChannel2(fl)  CAL_CMGETAPI( "GWClientBeginOpenChannel2" ) 
	#define CAL_GWClientBeginOpenChannel2  GWClientBeginOpenChannel2
	#define CHK_GWClientBeginOpenChannel2  TRUE
	#define EXP_GWClientBeginOpenChannel2  CAL_CMEXPAPI( "GWClientBeginOpenChannel2" ) 
	#define EXTREF_ITF_GWClientBeginOpenChannel2 {(RTS_VOID_FCTPTR)GWClientBeginOpenChannel2, "GWClientBeginOpenChannel2", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginOpenChannel2
	#define EXT_GWClientBeginOpenChannel2
	#define GET_GWClientBeginOpenChannel2(fl)  CAL_CMGETAPI( "GWClientBeginOpenChannel2" ) 
	#define CAL_GWClientBeginOpenChannel2  GWClientBeginOpenChannel2
	#define CHK_GWClientBeginOpenChannel2  TRUE
	#define EXP_GWClientBeginOpenChannel2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginOpenChannel2", (RTS_UINTPTR)GWClientBeginOpenChannel2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginOpenChannel2
	#define EXT_CmpGwClientGWClientBeginOpenChannel2
	#define GET_CmpGwClientGWClientBeginOpenChannel2  ERR_OK
	#define CAL_CmpGwClientGWClientBeginOpenChannel2  GWClientBeginOpenChannel2
	#define CHK_CmpGwClientGWClientBeginOpenChannel2  TRUE
	#define EXP_CmpGwClientGWClientBeginOpenChannel2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginOpenChannel2
	#define EXT_GWClientBeginOpenChannel2
	#define GET_GWClientBeginOpenChannel2(fl)  CAL_CMGETAPI( "GWClientBeginOpenChannel2" ) 
	#define CAL_GWClientBeginOpenChannel2  GWClientBeginOpenChannel2
	#define CHK_GWClientBeginOpenChannel2  TRUE
	#define EXP_GWClientBeginOpenChannel2  CAL_CMEXPAPI( "GWClientBeginOpenChannel2" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginOpenChannel2  PFGWCLIENTBEGINOPENCHANNEL2 pfGWClientBeginOpenChannel2;
	#define EXT_GWClientBeginOpenChannel2  extern PFGWCLIENTBEGINOPENCHANNEL2 pfGWClientBeginOpenChannel2;
	#define GET_GWClientBeginOpenChannel2(fl)  s_pfCMGetAPI2( "GWClientBeginOpenChannel2", (RTS_VOID_FCTPTR *)&pfGWClientBeginOpenChannel2, (fl), 0, 0)
	#define CAL_GWClientBeginOpenChannel2  pfGWClientBeginOpenChannel2
	#define CHK_GWClientBeginOpenChannel2  (pfGWClientBeginOpenChannel2 != NULL)
	#define EXP_GWClientBeginOpenChannel2   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginOpenChannel2", (RTS_UINTPTR)GWClientBeginOpenChannel2, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEndOpenChannel(ASYNCRESULT *pAsyncRes, RTS_HANDLE *phChannel, RTS_UI32 *pdwCommBufferSize, RTS_UI8 *pbBigEndianTarget);
typedef RTS_RESULT (CDECL * PFGWCLIENTENDOPENCHANNEL) (ASYNCRESULT *pAsyncRes, RTS_HANDLE *phChannel, RTS_UI32 *pdwCommBufferSize, RTS_UI8 *pbBigEndianTarget);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENDOPENCHANNEL_NOTIMPLEMENTED)
	#define USE_GWClientEndOpenChannel
	#define EXT_GWClientEndOpenChannel
	#define GET_GWClientEndOpenChannel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEndOpenChannel(p0,p1,p2,p3) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEndOpenChannel  FALSE
	#define EXP_GWClientEndOpenChannel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEndOpenChannel
	#define EXT_GWClientEndOpenChannel
	#define GET_GWClientEndOpenChannel(fl)  CAL_CMGETAPI( "GWClientEndOpenChannel" ) 
	#define CAL_GWClientEndOpenChannel  GWClientEndOpenChannel
	#define CHK_GWClientEndOpenChannel  TRUE
	#define EXP_GWClientEndOpenChannel  CAL_CMEXPAPI( "GWClientEndOpenChannel" ) 
	#define EXTREF_ITF_GWClientEndOpenChannel {(RTS_VOID_FCTPTR)GWClientEndOpenChannel, "GWClientEndOpenChannel", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEndOpenChannel
	#define EXT_GWClientEndOpenChannel
	#define GET_GWClientEndOpenChannel(fl)  CAL_CMGETAPI( "GWClientEndOpenChannel" ) 
	#define CAL_GWClientEndOpenChannel  GWClientEndOpenChannel
	#define CHK_GWClientEndOpenChannel  TRUE
	#define EXP_GWClientEndOpenChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndOpenChannel", (RTS_UINTPTR)GWClientEndOpenChannel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEndOpenChannel
	#define EXT_CmpGwClientGWClientEndOpenChannel
	#define GET_CmpGwClientGWClientEndOpenChannel  ERR_OK
	#define CAL_CmpGwClientGWClientEndOpenChannel  GWClientEndOpenChannel
	#define CHK_CmpGwClientGWClientEndOpenChannel  TRUE
	#define EXP_CmpGwClientGWClientEndOpenChannel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEndOpenChannel
	#define EXT_GWClientEndOpenChannel
	#define GET_GWClientEndOpenChannel(fl)  CAL_CMGETAPI( "GWClientEndOpenChannel" ) 
	#define CAL_GWClientEndOpenChannel  GWClientEndOpenChannel
	#define CHK_GWClientEndOpenChannel  TRUE
	#define EXP_GWClientEndOpenChannel  CAL_CMEXPAPI( "GWClientEndOpenChannel" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEndOpenChannel  PFGWCLIENTENDOPENCHANNEL pfGWClientEndOpenChannel;
	#define EXT_GWClientEndOpenChannel  extern PFGWCLIENTENDOPENCHANNEL pfGWClientEndOpenChannel;
	#define GET_GWClientEndOpenChannel(fl)  s_pfCMGetAPI2( "GWClientEndOpenChannel", (RTS_VOID_FCTPTR *)&pfGWClientEndOpenChannel, (fl), 0, 0)
	#define CAL_GWClientEndOpenChannel  pfGWClientEndOpenChannel
	#define CHK_GWClientEndOpenChannel  (pfGWClientEndOpenChannel != NULL)
	#define EXP_GWClientEndOpenChannel   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndOpenChannel", (RTS_UINTPTR)GWClientEndOpenChannel, 0, 0) 
#endif

RTS_RESULT CDECL GWClientBeginCloseChannel(RTS_HANDLE hGateway, RTS_HANDLE hChannel, ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTBEGINCLOSECHANNEL) (RTS_HANDLE hGateway, RTS_HANDLE hChannel, ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTBEGINCLOSECHANNEL_NOTIMPLEMENTED)
	#define USE_GWClientBeginCloseChannel
	#define EXT_GWClientBeginCloseChannel
	#define GET_GWClientBeginCloseChannel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientBeginCloseChannel(p0,p1,p2) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientBeginCloseChannel  FALSE
	#define EXP_GWClientBeginCloseChannel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientBeginCloseChannel
	#define EXT_GWClientBeginCloseChannel
	#define GET_GWClientBeginCloseChannel(fl)  CAL_CMGETAPI( "GWClientBeginCloseChannel" ) 
	#define CAL_GWClientBeginCloseChannel  GWClientBeginCloseChannel
	#define CHK_GWClientBeginCloseChannel  TRUE
	#define EXP_GWClientBeginCloseChannel  CAL_CMEXPAPI( "GWClientBeginCloseChannel" ) 
	#define EXTREF_ITF_GWClientBeginCloseChannel {(RTS_VOID_FCTPTR)GWClientBeginCloseChannel, "GWClientBeginCloseChannel", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientBeginCloseChannel
	#define EXT_GWClientBeginCloseChannel
	#define GET_GWClientBeginCloseChannel(fl)  CAL_CMGETAPI( "GWClientBeginCloseChannel" ) 
	#define CAL_GWClientBeginCloseChannel  GWClientBeginCloseChannel
	#define CHK_GWClientBeginCloseChannel  TRUE
	#define EXP_GWClientBeginCloseChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginCloseChannel", (RTS_UINTPTR)GWClientBeginCloseChannel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientBeginCloseChannel
	#define EXT_CmpGwClientGWClientBeginCloseChannel
	#define GET_CmpGwClientGWClientBeginCloseChannel  ERR_OK
	#define CAL_CmpGwClientGWClientBeginCloseChannel  GWClientBeginCloseChannel
	#define CHK_CmpGwClientGWClientBeginCloseChannel  TRUE
	#define EXP_CmpGwClientGWClientBeginCloseChannel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientBeginCloseChannel
	#define EXT_GWClientBeginCloseChannel
	#define GET_GWClientBeginCloseChannel(fl)  CAL_CMGETAPI( "GWClientBeginCloseChannel" ) 
	#define CAL_GWClientBeginCloseChannel  GWClientBeginCloseChannel
	#define CHK_GWClientBeginCloseChannel  TRUE
	#define EXP_GWClientBeginCloseChannel  CAL_CMEXPAPI( "GWClientBeginCloseChannel" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientBeginCloseChannel  PFGWCLIENTBEGINCLOSECHANNEL pfGWClientBeginCloseChannel;
	#define EXT_GWClientBeginCloseChannel  extern PFGWCLIENTBEGINCLOSECHANNEL pfGWClientBeginCloseChannel;
	#define GET_GWClientBeginCloseChannel(fl)  s_pfCMGetAPI2( "GWClientBeginCloseChannel", (RTS_VOID_FCTPTR *)&pfGWClientBeginCloseChannel, (fl), 0, 0)
	#define CAL_GWClientBeginCloseChannel  pfGWClientBeginCloseChannel
	#define CHK_GWClientBeginCloseChannel  (pfGWClientBeginCloseChannel != NULL)
	#define EXP_GWClientBeginCloseChannel   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientBeginCloseChannel", (RTS_UINTPTR)GWClientBeginCloseChannel, 0, 0) 
#endif

RTS_RESULT CDECL GWClientEndCloseChannel(ASYNCRESULT *pAsyncRes);
typedef RTS_RESULT (CDECL * PFGWCLIENTENDCLOSECHANNEL) (ASYNCRESULT *pAsyncRes);
#if defined(CMPGWCLIENT_NOTIMPLEMENTED) || defined(GWCLIENTENDCLOSECHANNEL_NOTIMPLEMENTED)
	#define USE_GWClientEndCloseChannel
	#define EXT_GWClientEndCloseChannel
	#define GET_GWClientEndCloseChannel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_GWClientEndCloseChannel(p0) (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_GWClientEndCloseChannel  FALSE
	#define EXP_GWClientEndCloseChannel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_GWClientEndCloseChannel
	#define EXT_GWClientEndCloseChannel
	#define GET_GWClientEndCloseChannel(fl)  CAL_CMGETAPI( "GWClientEndCloseChannel" ) 
	#define CAL_GWClientEndCloseChannel  GWClientEndCloseChannel
	#define CHK_GWClientEndCloseChannel  TRUE
	#define EXP_GWClientEndCloseChannel  CAL_CMEXPAPI( "GWClientEndCloseChannel" ) 
	#define EXTREF_ITF_GWClientEndCloseChannel {(RTS_VOID_FCTPTR)GWClientEndCloseChannel, "GWClientEndCloseChannel", 0, 0}
#elif defined(MIXED_LINK) && !defined(CMPGWCLIENT_EXTERNAL)
	#define USE_GWClientEndCloseChannel
	#define EXT_GWClientEndCloseChannel
	#define GET_GWClientEndCloseChannel(fl)  CAL_CMGETAPI( "GWClientEndCloseChannel" ) 
	#define CAL_GWClientEndCloseChannel  GWClientEndCloseChannel
	#define CHK_GWClientEndCloseChannel  TRUE
	#define EXP_GWClientEndCloseChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndCloseChannel", (RTS_UINTPTR)GWClientEndCloseChannel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpGwClientGWClientEndCloseChannel
	#define EXT_CmpGwClientGWClientEndCloseChannel
	#define GET_CmpGwClientGWClientEndCloseChannel  ERR_OK
	#define CAL_CmpGwClientGWClientEndCloseChannel  GWClientEndCloseChannel
	#define CHK_CmpGwClientGWClientEndCloseChannel  TRUE
	#define EXP_CmpGwClientGWClientEndCloseChannel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_GWClientEndCloseChannel
	#define EXT_GWClientEndCloseChannel
	#define GET_GWClientEndCloseChannel(fl)  CAL_CMGETAPI( "GWClientEndCloseChannel" ) 
	#define CAL_GWClientEndCloseChannel  GWClientEndCloseChannel
	#define CHK_GWClientEndCloseChannel  TRUE
	#define EXP_GWClientEndCloseChannel  CAL_CMEXPAPI( "GWClientEndCloseChannel" ) 
#else /* DYNAMIC_LINK */
	#define USE_GWClientEndCloseChannel  PFGWCLIENTENDCLOSECHANNEL pfGWClientEndCloseChannel;
	#define EXT_GWClientEndCloseChannel  extern PFGWCLIENTENDCLOSECHANNEL pfGWClientEndCloseChannel;
	#define GET_GWClientEndCloseChannel(fl)  s_pfCMGetAPI2( "GWClientEndCloseChannel", (RTS_VOID_FCTPTR *)&pfGWClientEndCloseChannel, (fl), 0, 0)
	#define CAL_GWClientEndCloseChannel  pfGWClientEndCloseChannel
	#define CHK_GWClientEndCloseChannel  (pfGWClientEndCloseChannel != NULL)
	#define EXP_GWClientEndCloseChannel   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"GWClientEndCloseChannel", (RTS_UINTPTR)GWClientEndCloseChannel, 0, 0) 
#endif


#ifdef __cplusplus
	}
#endif 



typedef struct
{
	IBase_C *pBase;
} ICmpGwClient_C;

#ifdef CPLUSPLUS
class ICmpGwClient : public IBase
{
	public:
};
	#ifndef ITF_CmpGwClient
		#define ITF_CmpGwClient static ICmpGwClient *pICmpGwClient = NULL;
	#endif
	#define EXTITF_CmpGwClient
#else	/*CPLUSPLUS*/
	typedef ICmpGwClient_C		ICmpGwClient;
	#ifndef ITF_CmpGwClient
		#define ITF_CmpGwClient
	#endif
	#define EXTITF_CmpGwClient
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPGWCLIENTITF_H_*/
