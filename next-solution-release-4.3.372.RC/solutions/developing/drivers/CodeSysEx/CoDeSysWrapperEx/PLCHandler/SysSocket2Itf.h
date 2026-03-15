 /**
 * <interfacename>SysSocket</interfacename>
 * <description> 
 *	<p>The SysSocket interface is projected to handle access to Ethernet socket layer.
 *	TCP, UDP and RAW sockets can be used.</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _SYSSOCKET2ITF_H_
#define _SYSSOCKET2ITF_H_

#include "CmpStd.h"

 

 




#include "SysSocketItf.h"

#define SYSSOCK_TYPE_STD		1
#define SYSSOCK_TYPE_TLS		2
#define SYSSOCK_TYPE_STARTTLS	3

#define USE_SYSSOCKET2_STMT					static PF_DELETEINSTANCE2 s_pfCMDeleteInstance2 = NULL;
#define INIT_SYSSOCKET2_STMT(pInitStruct)	{s_pfCMDeleteInstance2 = ((INIT_STRUCT *)pInitStruct)->pfCMDeleteInstance2;}

typedef struct SysSocket_StdSockets_
{
	RTS_I32 i32AddressFamily;
	RTS_I32 i32Type;
	RTS_I32 i32Protocol;
} SysSocket_StdSockets;

typedef struct SysSocket_TlsSockets_
{
	RTS_HANDLE hTlsContext;
} SysSocket_TlsSockets;

typedef union SysSock_SpecificParameter_
{
	SysSocket_StdSockets stdSockets;
	SysSocket_TlsSockets tlsSockets;
} SysSock_SpecificParameter;

typedef struct SysSock_Parameter_
{
	RTS_UI32 ui32Type;
	SysSock_SpecificParameter specific;
} SysSock_Parameter;

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 *	Create a socket of the specified class. 
 * </description>
 * <note>
 *  Using the TLS socket results in an IPv4 TCP socket where the TLS protocol is used. 
 *  Since additional handshaking has to be done, connection establishment takes 
 *  some time longer. By default, these sockets are created as **nonblocking**. 
 *  If a blocking behavior is needed this **has to be set explicitly**. Be aware that 
 *  blocking TLS sockets can lead to deadlock situations if the peer does not send 
 *  TLS data. This is a potential security risk, typically for server implementations.
 *  An attacker can create a connection and then stop sending TLS data, resulting in 
 *  a denial of service attack. This situation is avoided with nonblocking sockets.
 *  
 *  To avoid this potential risk by default TLS based sockets are created as 
 *  **nonblocking** by default. This applies to handles created by |SysSock2Create| 
 *  and |SysSock2Accept|. If this has to be changed use the |SysSock2Ioctl| to 
 *  setup the desired behavior.
 * </note>
 * <param name="hISysSocket" type="IN">Pointer to IBase. Is filled by the generate create interface</param>
 * <param name="ClassId" type="IN">ClassID of the SysSocket2 implementation</param>
 * <param name="pParameter" type="IN">Class specific parameters. See SysSock_Parameter for details</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the SysSocket2Itf interface or RTS_INVALID_HANDLE if failed or ClassID not available</result>
 */
STATICITF_DEF RTS_HANDLE CDECL SysSock2Create(RTS_HANDLE hISysSocket, CLASSID ClassId, SysSock_Parameter *pParameter, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSSOCK2CREATE) (RTS_HANDLE hISysSocket, CLASSID ClassId, SysSock_Parameter *pParameter, RTS_RESULT *pResult);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2CREATE_NOTIMPLEMENTED)
	#define USE_SysSock2Create
	#define EXT_SysSock2Create
	#define GET_SysSock2Create(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Create(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysSock2Create  FALSE
	#define EXP_SysSock2Create  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Create
	#define EXT_SysSock2Create
	#define GET_SysSock2Create(fl)  ERR_OK
	#define CAL_SysSock2Create(p0,p1,p2,p3)	\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									(((IBase *)p0)->ClassId = p1) == CLASSID_INVALID ? RTS_INVALID_HANDLE :\
									(\
										(((IBase *)p0)->hInstance = ((ISysSocket2*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, NULL))->ISysSock2Create(p0,p1,p2,p3)) == RTS_INVALID_HANDLE ? (s_pfCMDeleteInstance2(p1, (IBase *)p0) ? RTS_INVALID_HANDLE : RTS_INVALID_HANDLE) : \
										(\
											((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, NULL)\
										)\
									)\
								)\
							)
	#define CHK_SysSock2Create  TRUE
	#define EXP_SysSock2Create  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Create
	#define EXT_SysSock2Create
	#define GET_SysSock2Create(fl)  ERR_OK
	#define CAL_SysSock2Create(p0,p1,p2,p3)		\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									(((IBase *)p0)->ClassId = p1) == CLASSID_INVALID ? RTS_INVALID_HANDLE :\
									(\
										(((IBase *)p0)->hInstance = ((ISysSocket2*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, NULL))->ISysSock2Create(p0,p1,p2,p3)) == RTS_INVALID_HANDLE ? (s_pfCMDeleteInstance2(p1, (IBase *)p0) ? RTS_INVALID_HANDLE : RTS_INVALID_HANDLE) : \
										(\
											((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, NULL)\
										)\
									)\
								)\
							)
	#define CHK_SysSock2Create  TRUE
	#define EXP_SysSock2Create  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Create
	#define EXT_SysSocket2SysSock2Create
	#define GET_SysSocket2SysSock2Create  ERR_OK
	#define CAL_SysSocket2SysSock2Create(p0,p1,p2,p3)	\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									((ISysSocket2*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, (RTS_RESULT *)1))->ISysSock2Create(p0,p1,p2,p3)\
								)\
							)
	#define CHK_SysSocket2SysSock2Create  (s_pfCMCreateInstance != NULL)
	#define EXP_SysSocket2SysSock2Create  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Create
	#define EXT_SysSock2Create
	#define GET_SysSock2Create(fl)  ERR_OK
	#define CAL_SysSock2Create(p0,p1,p2,p3)\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									((ISysSocket2*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, (RTS_RESULT *)1))->ISysSock2Create(p0,p1,p2,p3)\
								)\
							)
	#define CHK_SysSock2Create  (s_pfCMCreateInstance != NULL)
	#define EXP_SysSock2Create  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Create
	#define EXT_SysSock2Create
	#define GET_SysSock2Create(fl)  ERR_OK
	#define CAL_SysSock2Create(p0,p1,p2,p3)		\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									(((IBase *)p0)->ClassId = p1) == CLASSID_INVALID ? RTS_INVALID_HANDLE :\
									(\
										(((IBase *)p0)->hInstance = ((ISysSocket2*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, NULL))->ISysSock2Create(p0,p1,p2,p3)) == RTS_INVALID_HANDLE ? (s_pfCMDeleteInstance2(p1, (IBase *)p0) ? RTS_INVALID_HANDLE : RTS_INVALID_HANDLE) : \
										(\
											((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, NULL)\
										)\
									)\
								)\
							)
	#define CHK_SysSock2Create  TRUE
	#define EXP_SysSock2Create  ERR_OK
#endif




/**
 * <description>
 *	Close a socket specified by a SysSocket2Itf handle.
 * </description>
 * <param name="hSocket" type="IN">Handle that was created via SysSockCreate2 API function!</param>
 * <result>Error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2Close(RTS_HANDLE hSocket);
typedef RTS_RESULT (CDECL * PFSYSSOCK2CLOSE) (RTS_HANDLE hSocket);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2CLOSE_NOTIMPLEMENTED)
	#define USE_SysSock2Close
	#define EXT_SysSock2Close
	#define GET_SysSock2Close(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Close(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2Close  FALSE
	#define EXP_SysSock2Close  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Close
	#define EXT_SysSock2Close
	#define GET_SysSock2Close(fl)  ERR_OK
	#define CAL_SysSock2Close(p0) 		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Close(((ISysSocket2*)p0)->pBase->hInstance) != ERR_OK ? \
														ERR_FAILED :\
														s_pfCMDeleteInstance2(((ISysSocket2*)p0)->pBase->ClassId, ((ISysSocket2*)p0)->pBase))
	#define CHK_SysSock2Close  TRUE
	#define EXP_SysSock2Close  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Close
	#define EXT_SysSock2Close
	#define GET_SysSock2Close(fl)  ERR_OK
	#define CAL_SysSock2Close(p0) 	 	(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Close(((ISysSocket2*)p0)->pBase->hInstance) != ERR_OK ? \
														ERR_FAILED :\
														s_pfCMDeleteInstance2(((ISysSocket2*)p0)->pBase->ClassId, ((ISysSocket2*)p0)->pBase))
	#define CHK_SysSock2Close  TRUE
	#define EXP_SysSock2Close  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Close
	#define EXT_SysSocket2SysSock2Close
	#define GET_SysSocket2SysSock2Close  ERR_OK
	#define CAL_SysSocket2SysSock2Close(p0)	 (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Close())
	#define CHK_SysSocket2SysSock2Close  TRUE
	#define EXP_SysSocket2SysSock2Close  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Close
	#define EXT_SysSock2Close
	#define GET_SysSock2Close(fl)  ERR_OK
	#define CAL_SysSock2Close(p0)	 (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Close())
	#define CHK_SysSock2Close  TRUE
	#define EXP_SysSock2Close  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Close
	#define EXT_SysSock2Close
	#define GET_SysSock2Close(fl)  ERR_OK
	#define CAL_SysSock2Close(p0)			(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Close(((ISysSocket2*)p0)->pBase->hInstance) != ERR_OK ? \
														ERR_FAILED :\
														s_pfCMDeleteInstance2(((ISysSocket2*)p0)->pBase->ClassId, ((ISysSocket2*)p0)->pBase))
	#define CHK_SysSock2Close  TRUE
	#define EXP_SysSock2Close  ERR_OK
#endif




/**
 * <description>
 *	Set options of a specified socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iLevel" type="IN">Level of the socket</param>
 * <param name="iOption" type="IN">Socket option command</param>
 * <param name="pcOptionValue" type="IN">Pointer to the option value</param>
 * <param name="iOptionLen" type="IN">Length of option value</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2SetOption(RTS_HANDLE hSocket, RTS_I32 iLevel, RTS_I32 iOption, char *pcOptionValue, RTS_I32 iOptionLen);
typedef RTS_RESULT (CDECL * PFSYSSOCK2SETOPTION) (RTS_HANDLE hSocket, RTS_I32 iLevel, RTS_I32 iOption, char *pcOptionValue, RTS_I32 iOptionLen);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2SETOPTION_NOTIMPLEMENTED)
	#define USE_SysSock2SetOption
	#define EXT_SysSock2SetOption
	#define GET_SysSock2SetOption(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2SetOption(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2SetOption  FALSE
	#define EXP_SysSock2SetOption  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2SetOption
	#define EXT_SysSock2SetOption
	#define GET_SysSock2SetOption(fl)  ERR_OK
	#define CAL_SysSock2SetOption(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2SetOption(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2SetOption  TRUE
	#define EXP_SysSock2SetOption  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2SetOption
	#define EXT_SysSock2SetOption
	#define GET_SysSock2SetOption(fl)  ERR_OK
	#define CAL_SysSock2SetOption(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2SetOption(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2SetOption  TRUE
	#define EXP_SysSock2SetOption  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2SetOption
	#define EXT_SysSocket2SysSock2SetOption
	#define GET_SysSocket2SysSock2SetOption  ERR_OK
	#define CAL_SysSocket2SysSock2SetOption(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2SetOption(p1,p2,p3,p4))
	#define CHK_SysSocket2SysSock2SetOption  TRUE
	#define EXP_SysSocket2SysSock2SetOption  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2SetOption
	#define EXT_SysSock2SetOption
	#define GET_SysSock2SetOption(fl)  ERR_OK
	#define CAL_SysSock2SetOption(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2SetOption(p1,p2,p3,p4))
	#define CHK_SysSock2SetOption  TRUE
	#define EXP_SysSock2SetOption  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2SetOption
	#define EXT_SysSock2SetOption
	#define GET_SysSock2SetOption(fl)  ERR_OK
	#define CAL_SysSock2SetOption(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2SetOption(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2SetOption  TRUE
	#define EXP_SysSock2SetOption  ERR_OK
#endif




/**
 * <description>
 *	Get options of a specified socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iLevel" type="IN">Level of the socket</param>
 * <param name="iOption" type="IN">Socket option command</param>
 * <param name="pcOptionValue" type="OUT">Pointer to get the option value</param>
 * <param name="piOptionLen" type="OUT">Pointer to the option length. Real length is returned</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2GetOption(RTS_HANDLE hSocket, RTS_I32 iLevel, RTS_I32 iOption, char *pcOptionValue, RTS_I32 *piOptionLen);
typedef RTS_RESULT (CDECL * PFSYSSOCK2GETOPTION) (RTS_HANDLE hSocket, RTS_I32 iLevel, RTS_I32 iOption, char *pcOptionValue, RTS_I32 *piOptionLen);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2GETOPTION_NOTIMPLEMENTED)
	#define USE_SysSock2GetOption
	#define EXT_SysSock2GetOption
	#define GET_SysSock2GetOption(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2GetOption(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2GetOption  FALSE
	#define EXP_SysSock2GetOption  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2GetOption
	#define EXT_SysSock2GetOption
	#define GET_SysSock2GetOption(fl)  ERR_OK
	#define CAL_SysSock2GetOption(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetOption(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2GetOption  TRUE
	#define EXP_SysSock2GetOption  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2GetOption
	#define EXT_SysSock2GetOption
	#define GET_SysSock2GetOption(fl)  ERR_OK
	#define CAL_SysSock2GetOption(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetOption(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2GetOption  TRUE
	#define EXP_SysSock2GetOption  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2GetOption
	#define EXT_SysSocket2SysSock2GetOption
	#define GET_SysSocket2SysSock2GetOption  ERR_OK
	#define CAL_SysSocket2SysSock2GetOption(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetOption(p1,p2,p3,p4))
	#define CHK_SysSocket2SysSock2GetOption  TRUE
	#define EXP_SysSocket2SysSock2GetOption  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2GetOption
	#define EXT_SysSock2GetOption
	#define GET_SysSock2GetOption(fl)  ERR_OK
	#define CAL_SysSock2GetOption(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetOption(p1,p2,p3,p4))
	#define CHK_SysSock2GetOption  TRUE
	#define EXP_SysSock2GetOption  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2GetOption
	#define EXT_SysSock2GetOption
	#define GET_SysSock2GetOption(fl)  ERR_OK
	#define CAL_SysSock2GetOption(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetOption(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2GetOption  TRUE
	#define EXP_SysSock2GetOption  ERR_OK
#endif




/**
 * <description>
 *	Bind a socket to a socket address and port number
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="IN">Socket address</param>
 * <param name="iSockAddrSize" type="IN">Size of the socket address structure</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2Bind(RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize);
typedef RTS_RESULT (CDECL * PFSYSSOCK2BIND) (RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2BIND_NOTIMPLEMENTED)
	#define USE_SysSock2Bind
	#define EXT_SysSock2Bind
	#define GET_SysSock2Bind(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Bind(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2Bind  FALSE
	#define EXP_SysSock2Bind  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Bind
	#define EXT_SysSock2Bind
	#define GET_SysSock2Bind(fl)  ERR_OK
	#define CAL_SysSock2Bind(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Bind(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2Bind  TRUE
	#define EXP_SysSock2Bind  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Bind
	#define EXT_SysSock2Bind
	#define GET_SysSock2Bind(fl)  ERR_OK
	#define CAL_SysSock2Bind(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Bind(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2Bind  TRUE
	#define EXP_SysSock2Bind  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Bind
	#define EXT_SysSocket2SysSock2Bind
	#define GET_SysSocket2SysSock2Bind  ERR_OK
	#define CAL_SysSocket2SysSock2Bind(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Bind(p1,p2))
	#define CHK_SysSocket2SysSock2Bind  TRUE
	#define EXP_SysSocket2SysSock2Bind  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Bind
	#define EXT_SysSock2Bind
	#define GET_SysSock2Bind(fl)  ERR_OK
	#define CAL_SysSock2Bind(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Bind(p1,p2))
	#define CHK_SysSock2Bind  TRUE
	#define EXP_SysSock2Bind  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Bind
	#define EXT_SysSock2Bind
	#define GET_SysSock2Bind(fl)  ERR_OK
	#define CAL_SysSock2Bind(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Bind(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2Bind  TRUE
	#define EXP_SysSock2Bind  ERR_OK
#endif




/**
 * <description>
 *	Listen on a TCP server socket for new connection
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iMaxConnections" type="IN">Maximum number of connections allowed</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2Listen(RTS_HANDLE hSocket, RTS_I32 iMaxConnections);
typedef RTS_RESULT (CDECL * PFSYSSOCK2LISTEN) (RTS_HANDLE hSocket, RTS_I32 iMaxConnections);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2LISTEN_NOTIMPLEMENTED)
	#define USE_SysSock2Listen
	#define EXT_SysSock2Listen
	#define GET_SysSock2Listen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Listen(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2Listen  FALSE
	#define EXP_SysSock2Listen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Listen
	#define EXT_SysSock2Listen
	#define GET_SysSock2Listen(fl)  ERR_OK
	#define CAL_SysSock2Listen(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Listen(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2Listen  TRUE
	#define EXP_SysSock2Listen  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Listen
	#define EXT_SysSock2Listen
	#define GET_SysSock2Listen(fl)  ERR_OK
	#define CAL_SysSock2Listen(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Listen(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2Listen  TRUE
	#define EXP_SysSock2Listen  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Listen
	#define EXT_SysSocket2SysSock2Listen
	#define GET_SysSocket2SysSock2Listen  ERR_OK
	#define CAL_SysSocket2SysSock2Listen(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Listen(p1))
	#define CHK_SysSocket2SysSock2Listen  TRUE
	#define EXP_SysSocket2SysSock2Listen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Listen
	#define EXT_SysSock2Listen
	#define GET_SysSock2Listen(fl)  ERR_OK
	#define CAL_SysSock2Listen(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Listen(p1))
	#define CHK_SysSock2Listen  TRUE
	#define EXP_SysSock2Listen  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Listen
	#define EXT_SysSock2Listen
	#define GET_SysSock2Listen(fl)  ERR_OK
	#define CAL_SysSock2Listen(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Listen(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2Listen  TRUE
	#define EXP_SysSock2Listen  ERR_OK
#endif




/**
 * <description>
 *	Accept the next incoming TCP connection. Returns the socket for the newly created
 *	connection or RTS_INVALID_HANDLE if failed.
 *  Note: New client sockets of type TLS are created in nonblocking mode. Use SysSock2Ioctl to set the desired blocking mode.
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="OUT">Socket address of the client, who is connected</param>
 * <param name="piSockAddrSize" type="INOUT">Pointer to socket address structure</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the new accepted socket</result>
 */
STATICITF_DEF RTS_HANDLE CDECL SysSock2Accept(RTS_HANDLE hISysSocket, CLASSID ClassId, RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSSOCK2ACCEPT) (RTS_HANDLE hISysSocket, CLASSID ClassId, RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize, RTS_RESULT *pResult);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2ACCEPT_NOTIMPLEMENTED)
	#define USE_SysSock2Accept
	#define EXT_SysSock2Accept
	#define GET_SysSock2Accept(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Accept(p0,p1,p2,p3,p4,p5)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysSock2Accept  FALSE
	#define EXP_SysSock2Accept  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Accept
	#define EXT_SysSock2Accept
	#define GET_SysSock2Accept(fl)  ERR_OK
	#define CAL_SysSock2Accept(p0,p1,p2,p3,p4,p5)	\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									(((IBase *)p0)->ClassId = p1) == 0 ? RTS_INVALID_HANDLE :\
									(\
										(((IBase *)p0)->hInstance = ((ISysSocket2*)p2)->ISysSock2Accept(p2, p1, ((ISysSocket2*)p2)->pBase->hInstance, p3,p4,p5)) == RTS_INVALID_HANDLE ? (s_pfCMDeleteInstance2(p1, (IBase *)p0) ? RTS_INVALID_HANDLE : RTS_INVALID_HANDLE) : \
										(\
											((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, NULL)\
										)\
									)\
								)\
							)
	#define CHK_SysSock2Accept  TRUE
	#define EXP_SysSock2Accept  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Accept
	#define EXT_SysSock2Accept
	#define GET_SysSock2Accept(fl)  ERR_OK
	#define CAL_SysSock2Accept(p0,p1,p2,p3,p4,p5)		\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									(((IBase *)p0)->ClassId = p1) == 0 ? RTS_INVALID_HANDLE :\
									(\
										(((IBase *)p0)->hInstance = ((ISysSocket2*)p2)->ISysSock2Accept(p2, p1, ((ISysSocket2*)p2)->pBase->hInstance, p3,p4,p5)) == RTS_INVALID_HANDLE ? (s_pfCMDeleteInstance2(p1, (IBase *)p0) ? RTS_INVALID_HANDLE : RTS_INVALID_HANDLE) : \
										(\
											((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, NULL)\
										)\
									)\
								)\
							)
	#define CHK_SysSock2Accept  TRUE
	#define EXP_SysSock2Accept  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Accept
	#define EXT_SysSocket2SysSock2Accept
	#define GET_SysSocket2SysSock2Accept  ERR_OK
	#define CAL_SysSocket2SysSock2Accept(p0,p1,p2,p3,p4,p5)		\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									((ISysSocket2*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, (RTS_RESULT *)1))->ISysSock2Accept(p0,p1,p2,p3,p4,p5)\
								)\
							)
	#define CHK_SysSocket2SysSock2Accept  (s_pfCMCreateInstance != NULL)
	#define EXP_SysSocket2SysSock2Accept  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Accept
	#define EXT_SysSock2Accept
	#define GET_SysSock2Accept(fl)  ERR_OK
	#define CAL_SysSock2Accept(p0,p1,p2,p3,p4,p5)		\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									((ISysSocket2*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, (RTS_RESULT *)1))->ISysSock2Accept(p0,p1,p2,p3,p4,p5)\
								)\
							)
	#define CHK_SysSock2Accept  (s_pfCMCreateInstance != NULL)
	#define EXP_SysSock2Accept  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Accept
	#define EXT_SysSock2Accept
	#define GET_SysSock2Accept(fl)  ERR_OK
	#define CAL_SysSock2Accept(p0,p1,p2,p3,p4,p5)		\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									(((IBase *)p0)->ClassId = p1) == 0 ? RTS_INVALID_HANDLE :\
									(\
										(((IBase *)p0)->hInstance = ((ISysSocket2*)p2)->ISysSock2Accept(p2, p1, ((ISysSocket2*)p2)->pBase->hInstance, p3,p4,p5)) == RTS_INVALID_HANDLE ? (s_pfCMDeleteInstance2(p1, (IBase *)p0) ? RTS_INVALID_HANDLE : RTS_INVALID_HANDLE) : \
										(\
											((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ISysSocket2, NULL)\
										)\
									)\
								)\
							)
	#define CHK_SysSock2Accept  TRUE
	#define EXP_SysSock2Accept  ERR_OK
#endif




/**
 * <description>
 *	Connect as a client to a TCP server.
 *  Note: If you are connecting to a TLS server that requires SNI (Server Name Indication) use SysSock2SetOption with 
 *  level SOCKET_IPPROTO_TLS and option SOCKET_SO_HOSTNAME to set the host-name.
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="IN">Socket address of the client, who is connected</param>
 * <param name="iSockAddrSize" type="IN">Size of socket address structure</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2Connect(RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize);
typedef RTS_RESULT (CDECL * PFSYSSOCK2CONNECT) (RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2CONNECT_NOTIMPLEMENTED)
	#define USE_SysSock2Connect
	#define EXT_SysSock2Connect
	#define GET_SysSock2Connect(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Connect(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2Connect  FALSE
	#define EXP_SysSock2Connect  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Connect
	#define EXT_SysSock2Connect
	#define GET_SysSock2Connect(fl)  ERR_OK
	#define CAL_SysSock2Connect(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Connect(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2Connect  TRUE
	#define EXP_SysSock2Connect  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Connect
	#define EXT_SysSock2Connect
	#define GET_SysSock2Connect(fl)  ERR_OK
	#define CAL_SysSock2Connect(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Connect(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2Connect  TRUE
	#define EXP_SysSock2Connect  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Connect
	#define EXT_SysSocket2SysSock2Connect
	#define GET_SysSocket2SysSock2Connect  ERR_OK
	#define CAL_SysSocket2SysSock2Connect(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Connect(p1,p2))
	#define CHK_SysSocket2SysSock2Connect  TRUE
	#define EXP_SysSocket2SysSock2Connect  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Connect
	#define EXT_SysSock2Connect
	#define GET_SysSock2Connect(fl)  ERR_OK
	#define CAL_SysSock2Connect(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Connect(p1,p2))
	#define CHK_SysSock2Connect  TRUE
	#define EXP_SysSock2Connect  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Connect
	#define EXT_SysSock2Connect
	#define GET_SysSock2Connect(fl)  ERR_OK
	#define CAL_SysSock2Connect(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Connect(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2Connect  TRUE
	#define EXP_SysSock2Connect  ERR_OK
#endif




/**
 * <description>
 *	Check a number of sockets for activity	
 * </description>
 * <param name="iWidth" type="IN">Number of sockets in the SOCKET_FD_SET structure, so SOCKET_FD_SETSIZE must be used here.</param>
 * <param name="fdRead" type="IN">Read socket</param>
 * <param name="fdWrite" type="IN">Write socket</param>
 * <param name="fdExcept" type="IN">Exception socket</param>
 * <param name="ptvTimeout" type="IN">Pointer to specify the timeout of the operation.
 *	ptvTimeout=NULL:	Infinite wait
 *	ptvTimeout->tv_sec=-1, ptvTimeout->tv_usec=-1:	Infinite wait
 *	ptvTimeout->tv_sec=0, ptvTimeout->tv_usec=0:	No wait</param>
 * <param name="pnReady" type="OUT">Number of sockets that are ready for IO</param>
 * <result>ERR_OK or ERR_SOCK_TIMEDOUT, if timeout expired</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2Select(RTS_HANDLE hSocket, RTS_I32 iWidth, SOCKET_FD_SET *fdRead, SOCKET_FD_SET *fdWrite, SOCKET_FD_SET *fdExcept, SOCKET_TIMEVAL *ptvTimeout, RTS_I32 *pnReady);
typedef RTS_RESULT (CDECL * PFSYSSOCK2SELECT) (RTS_HANDLE hSocket, RTS_I32 iWidth, SOCKET_FD_SET *fdRead, SOCKET_FD_SET *fdWrite, SOCKET_FD_SET *fdExcept, SOCKET_TIMEVAL *ptvTimeout, RTS_I32 *pnReady);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2SELECT_NOTIMPLEMENTED)
	#define USE_SysSock2Select
	#define EXT_SysSock2Select
	#define GET_SysSock2Select(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Select(p0,p1,p2,p3,p4,p5,p6)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2Select  FALSE
	#define EXP_SysSock2Select  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Select
	#define EXT_SysSock2Select
	#define GET_SysSock2Select(fl)  ERR_OK
	#define CAL_SysSock2Select(p0,p1,p2,p3,p4,p5,p6) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Select(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2Select  TRUE
	#define EXP_SysSock2Select  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Select
	#define EXT_SysSock2Select
	#define GET_SysSock2Select(fl)  ERR_OK
	#define CAL_SysSock2Select(p0,p1,p2,p3,p4,p5,p6) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Select(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2Select  TRUE
	#define EXP_SysSock2Select  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Select
	#define EXT_SysSocket2SysSock2Select
	#define GET_SysSocket2SysSock2Select  ERR_OK
	#define CAL_SysSocket2SysSock2Select(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Select(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSocket2SysSock2Select  TRUE
	#define EXP_SysSocket2SysSock2Select  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Select
	#define EXT_SysSock2Select
	#define GET_SysSock2Select(fl)  ERR_OK
	#define CAL_SysSock2Select(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Select(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2Select  TRUE
	#define EXP_SysSock2Select  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Select
	#define EXT_SysSock2Select
	#define GET_SysSock2Select(fl)  ERR_OK
	#define CAL_SysSock2Select(p0,p1,p2,p3,p4,p5,p6) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Select(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2Select  TRUE
	#define EXP_SysSock2Select  ERR_OK
#endif




/**
 * <description>
 *	Shutdown a socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iHow" type="IN">Specified, which operations are no longer be allowed. See category shutdown flags</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2Shutdown(RTS_HANDLE hSocket, RTS_I32 iHow);
typedef RTS_RESULT (CDECL * PFSYSSOCK2SHUTDOWN) (RTS_HANDLE hSocket, RTS_I32 iHow);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2SHUTDOWN_NOTIMPLEMENTED)
	#define USE_SysSock2Shutdown
	#define EXT_SysSock2Shutdown
	#define GET_SysSock2Shutdown(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Shutdown(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2Shutdown  FALSE
	#define EXP_SysSock2Shutdown  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Shutdown
	#define EXT_SysSock2Shutdown
	#define GET_SysSock2Shutdown(fl)  ERR_OK
	#define CAL_SysSock2Shutdown(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Shutdown(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2Shutdown  TRUE
	#define EXP_SysSock2Shutdown  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Shutdown
	#define EXT_SysSock2Shutdown
	#define GET_SysSock2Shutdown(fl)  ERR_OK
	#define CAL_SysSock2Shutdown(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Shutdown(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2Shutdown  TRUE
	#define EXP_SysSock2Shutdown  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Shutdown
	#define EXT_SysSocket2SysSock2Shutdown
	#define GET_SysSocket2SysSock2Shutdown  ERR_OK
	#define CAL_SysSocket2SysSock2Shutdown(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Shutdown(p1))
	#define CHK_SysSocket2SysSock2Shutdown  TRUE
	#define EXP_SysSocket2SysSock2Shutdown  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Shutdown
	#define EXT_SysSock2Shutdown
	#define GET_SysSock2Shutdown(fl)  ERR_OK
	#define CAL_SysSock2Shutdown(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Shutdown(p1))
	#define CHK_SysSock2Shutdown  TRUE
	#define EXP_SysSock2Shutdown  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Shutdown
	#define EXT_SysSock2Shutdown
	#define GET_SysSock2Shutdown(fl)  ERR_OK
	#define CAL_SysSock2Shutdown(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Shutdown(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2Shutdown  TRUE
	#define EXP_SysSock2Shutdown  ERR_OK
#endif




/**
 * <description>
 *	Io-control of a socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iCommand" type="IN">Io-control command</param>
 * <param name="piParameter" type="INOUT">Parameter value of the command</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2Ioctl(RTS_HANDLE hSocket, RTS_I32 iCommand, RTS_I32 *piParameter);
typedef RTS_RESULT (CDECL * PFSYSSOCK2IOCTL) (RTS_HANDLE hSocket, RTS_I32 iCommand, RTS_I32 *piParameter);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2IOCTL_NOTIMPLEMENTED)
	#define USE_SysSock2Ioctl
	#define EXT_SysSock2Ioctl
	#define GET_SysSock2Ioctl(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Ioctl(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2Ioctl  FALSE
	#define EXP_SysSock2Ioctl  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Ioctl
	#define EXT_SysSock2Ioctl
	#define GET_SysSock2Ioctl(fl)  ERR_OK
	#define CAL_SysSock2Ioctl(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Ioctl(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2Ioctl  TRUE
	#define EXP_SysSock2Ioctl  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Ioctl
	#define EXT_SysSock2Ioctl
	#define GET_SysSock2Ioctl(fl)  ERR_OK
	#define CAL_SysSock2Ioctl(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Ioctl(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2Ioctl  TRUE
	#define EXP_SysSock2Ioctl  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Ioctl
	#define EXT_SysSocket2SysSock2Ioctl
	#define GET_SysSocket2SysSock2Ioctl  ERR_OK
	#define CAL_SysSocket2SysSock2Ioctl(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Ioctl(p1,p2))
	#define CHK_SysSocket2SysSock2Ioctl  TRUE
	#define EXP_SysSocket2SysSock2Ioctl  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Ioctl
	#define EXT_SysSock2Ioctl
	#define GET_SysSock2Ioctl(fl)  ERR_OK
	#define CAL_SysSock2Ioctl(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Ioctl(p1,p2))
	#define CHK_SysSock2Ioctl  TRUE
	#define EXP_SysSock2Ioctl  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Ioctl
	#define EXT_SysSock2Ioctl
	#define GET_SysSock2Ioctl(fl)  ERR_OK
	#define CAL_SysSock2Ioctl(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2Ioctl(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2Ioctl  TRUE
	#define EXP_SysSock2Ioctl  ERR_OK
#endif




/**
 * <description>
 *	Check if a socket is inside of a set.
 * </description>
 * <param name="hSocket" type="IN">Socket to check. Can be RTS_INVALID_HANDLE to check for an invalid filled set.</param>
 * <param name="pfs" type="IN">Socket Set</param>
 * <result>TRUE if the specified socket is inside the set, FALSE if not</result>
 */
STATICITF_DEF RTS_BOOL CDECL SysSock2FdIsset(RTS_HANDLE hSocket, SOCKET_FD_SET *pfs);
typedef RTS_BOOL (CDECL * PFSYSSOCK2FDISSET) (RTS_HANDLE hSocket, SOCKET_FD_SET *pfs);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2FDISSET_NOTIMPLEMENTED)
	#define USE_SysSock2FdIsset
	#define EXT_SysSock2FdIsset
	#define GET_SysSock2FdIsset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2FdIsset(p0,p1)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2FdIsset  FALSE
	#define EXP_SysSock2FdIsset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2FdIsset
	#define EXT_SysSock2FdIsset
	#define GET_SysSock2FdIsset(fl)  ERR_OK
	#define CAL_SysSock2FdIsset(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2FdIsset(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2FdIsset  TRUE
	#define EXP_SysSock2FdIsset  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2FdIsset
	#define EXT_SysSock2FdIsset
	#define GET_SysSock2FdIsset(fl)  ERR_OK
	#define CAL_SysSock2FdIsset(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2FdIsset(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2FdIsset  TRUE
	#define EXP_SysSock2FdIsset  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2FdIsset
	#define EXT_SysSocket2SysSock2FdIsset
	#define GET_SysSocket2SysSock2FdIsset  ERR_OK
	#define CAL_SysSocket2SysSock2FdIsset(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2FdIsset(p1))
	#define CHK_SysSocket2SysSock2FdIsset  TRUE
	#define EXP_SysSocket2SysSock2FdIsset  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2FdIsset
	#define EXT_SysSock2FdIsset
	#define GET_SysSock2FdIsset(fl)  ERR_OK
	#define CAL_SysSock2FdIsset(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2FdIsset(p1))
	#define CHK_SysSock2FdIsset  TRUE
	#define EXP_SysSock2FdIsset  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2FdIsset
	#define EXT_SysSock2FdIsset
	#define GET_SysSock2FdIsset(fl)  ERR_OK
	#define CAL_SysSock2FdIsset(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2FdIsset(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2FdIsset  TRUE
	#define EXP_SysSock2FdIsset  ERR_OK
#endif




/**
 * <description>
 *	Add a socket to a socket set.
 * </description>
 * <param name="hSocket" type="IN">Socket to add.</param>
 * <param name="pfs" type="IN">Socket Set</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2FdInit(RTS_HANDLE hSocket, SOCKET_FD_SET *pfs);
typedef RTS_RESULT (CDECL * PFSYSSOCK2FDINIT) (RTS_HANDLE hSocket, SOCKET_FD_SET *pfs);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2FDINIT_NOTIMPLEMENTED)
	#define USE_SysSock2FdInit
	#define EXT_SysSock2FdInit
	#define GET_SysSock2FdInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2FdInit(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2FdInit  FALSE
	#define EXP_SysSock2FdInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2FdInit
	#define EXT_SysSock2FdInit
	#define GET_SysSock2FdInit(fl)  ERR_OK
	#define CAL_SysSock2FdInit(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2FdInit(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2FdInit  TRUE
	#define EXP_SysSock2FdInit  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2FdInit
	#define EXT_SysSock2FdInit
	#define GET_SysSock2FdInit(fl)  ERR_OK
	#define CAL_SysSock2FdInit(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2FdInit(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2FdInit  TRUE
	#define EXP_SysSock2FdInit  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2FdInit
	#define EXT_SysSocket2SysSock2FdInit
	#define GET_SysSocket2SysSock2FdInit  ERR_OK
	#define CAL_SysSocket2SysSock2FdInit(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2FdInit(p1))
	#define CHK_SysSocket2SysSock2FdInit  TRUE
	#define EXP_SysSocket2SysSock2FdInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2FdInit
	#define EXT_SysSock2FdInit
	#define GET_SysSock2FdInit(fl)  ERR_OK
	#define CAL_SysSock2FdInit(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2FdInit(p1))
	#define CHK_SysSock2FdInit  TRUE
	#define EXP_SysSock2FdInit  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2FdInit
	#define EXT_SysSock2FdInit
	#define GET_SysSock2FdInit(fl)  ERR_OK
	#define CAL_SysSock2FdInit(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2FdInit(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2FdInit  TRUE
	#define EXP_SysSock2FdInit  ERR_OK
#endif




/* ------------ TCP specific ------------ */

/**
 * <description>
 *	Receive data from a TCP socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pbyBuffer" type="OUT">Buffer to read data from the socket</param>
 * <param name="iBufferSize" type="IN">Maximum length of the buffer</param>
 * <param name="iFlags" type="IN">The flags parameter can be used to influence the behavior of the 
 *	function beyond the options specified for the associated socket. The semantics of this function 
 *	are determined by the socket options and the flags parameter. The latter is constructed by using the
 *	bitwise OR operator with any of the SOCKET_MSG values. See category TCP flags.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns number of bytes received. 0 if failed.</result>
 */
STATICITF_DEF RTS_SSIZE CDECL SysSock2Recv(RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCK2RECV) (RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, RTS_RESULT *pResult);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2RECV_NOTIMPLEMENTED)
	#define USE_SysSock2Recv
	#define EXT_SysSock2Recv
	#define GET_SysSock2Recv(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Recv(p0,p1,p2,p3,p4)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2Recv  FALSE
	#define EXP_SysSock2Recv  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Recv
	#define EXT_SysSock2Recv
	#define GET_SysSock2Recv(fl)  ERR_OK
	#define CAL_SysSock2Recv(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Recv(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2Recv  TRUE
	#define EXP_SysSock2Recv  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Recv
	#define EXT_SysSock2Recv
	#define GET_SysSock2Recv(fl)  ERR_OK
	#define CAL_SysSock2Recv(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Recv(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2Recv  TRUE
	#define EXP_SysSock2Recv  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Recv
	#define EXT_SysSocket2SysSock2Recv
	#define GET_SysSocket2SysSock2Recv  ERR_OK
	#define CAL_SysSocket2SysSock2Recv(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Recv(p1,p2,p3,p4))
	#define CHK_SysSocket2SysSock2Recv  TRUE
	#define EXP_SysSocket2SysSock2Recv  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Recv
	#define EXT_SysSock2Recv
	#define GET_SysSock2Recv(fl)  ERR_OK
	#define CAL_SysSock2Recv(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Recv(p1,p2,p3,p4))
	#define CHK_SysSock2Recv  TRUE
	#define EXP_SysSock2Recv  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Recv
	#define EXT_SysSock2Recv
	#define GET_SysSock2Recv(fl)  ERR_OK
	#define CAL_SysSock2Recv(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Recv(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2Recv  TRUE
	#define EXP_SysSock2Recv  ERR_OK
#endif




/**
 * <description>
 *	Receives a message from a socket.
 *
 *  NOTE: For querying the SOCKET_MSG_BCAST message flag the IP_PKTINFO socket option
 *  hast to be enabled on the socket, i.e. the IP_PKTINFO socket option has to be set.
 *
 *  NOTE: If no messages are available at the socket and SOCKET_FIONBIO is not set on
 *  the socket, SysSockRecvMsg() shall block until a message arrives.
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket.</param>
 * <param name="pMessage" type="OUT">Points to a SOCK_RECVMSG_MSG structure, containing both the
 *  buffer to store the source address and the buffer
 *  for the incoming message. The length and format of
 *  the address depend on the address family of the
 *  socket. The msgFlags and msgFlagsSupported members are ignored on input, but
 *  may contain meaningful values on output.
 *  See category SysSockRecvMsg.</param>
 * <param name="flags" type="IN">Specifies the type of message reception. Values of this
 *  argument are formed by logically OR'ing zero or more of
 *  the SOCKET_MSG values. See category TCP flags.</param>
 * <param name="pResult" type="OUT">Pointer to error code.
 *  See category TCP flags.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Completed successfully.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter value(s).</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOT_SUPPORTED">WSARecvMsg is not available.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">The platform system call failed [Windows: WSARecvMsg(), Linux: recvmsg()]. See log for details.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">The SysSockGetOption() call failed. See log for details.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_SOCK_WOULDBLOCK">The socket is marked as nonblocking
 *  and there are no messages available at the socket.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_SOCK_CLOSED">There are no messages available to be received
 *  and the peer has performed an orderly shutdown.</errorcode>
 * <result>Upon successful completion, SysSockRecvMsg() shall return the length of
 *  the message in bytes. If no messages are available to be received
 *  and the peer has performed an orderly shutdown, SysSockRecvMsg() shall
 *  return 0 and set *pResult to ERR_SOCK_CLOSED. Otherwise, the function shall return 0 and set *pResult
 *  to indicate the error.</result>
 */
STATICITF_DEF RTS_SSIZE CDECL SysSock2RecvMsg(RTS_HANDLE hSocket, SOCK_RECVMSG_MSG* pMessage, RTS_UI32 flags, RTS_RESULT* pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCK2RECVMSG) (RTS_HANDLE hSocket, SOCK_RECVMSG_MSG* pMessage, RTS_UI32 flags, RTS_RESULT* pResult);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2RECVMSG_NOTIMPLEMENTED)
	#define USE_SysSock2RecvMsg
	#define EXT_SysSock2RecvMsg
	#define GET_SysSock2RecvMsg(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2RecvMsg(p0,p1,p2,p3)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2RecvMsg  FALSE
	#define EXP_SysSock2RecvMsg  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2RecvMsg
	#define EXT_SysSock2RecvMsg
	#define GET_SysSock2RecvMsg(fl)  ERR_OK
	#define CAL_SysSock2RecvMsg(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvMsg(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_SysSock2RecvMsg  TRUE
	#define EXP_SysSock2RecvMsg  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2RecvMsg
	#define EXT_SysSock2RecvMsg
	#define GET_SysSock2RecvMsg(fl)  ERR_OK
	#define CAL_SysSock2RecvMsg(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvMsg(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_SysSock2RecvMsg  TRUE
	#define EXP_SysSock2RecvMsg  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2RecvMsg
	#define EXT_SysSocket2SysSock2RecvMsg
	#define GET_SysSocket2SysSock2RecvMsg  ERR_OK
	#define CAL_SysSocket2SysSock2RecvMsg(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvMsg(p1,p2,p3))
	#define CHK_SysSocket2SysSock2RecvMsg  TRUE
	#define EXP_SysSocket2SysSock2RecvMsg  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2RecvMsg
	#define EXT_SysSock2RecvMsg
	#define GET_SysSock2RecvMsg(fl)  ERR_OK
	#define CAL_SysSock2RecvMsg(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvMsg(p1,p2,p3))
	#define CHK_SysSock2RecvMsg  TRUE
	#define EXP_SysSock2RecvMsg  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2RecvMsg
	#define EXT_SysSock2RecvMsg
	#define GET_SysSock2RecvMsg(fl)  ERR_OK
	#define CAL_SysSock2RecvMsg(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvMsg(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_SysSock2RecvMsg  TRUE
	#define EXP_SysSock2RecvMsg  ERR_OK
#endif




/**
 * <description>
 *	Sent data to a TCP socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pbyBuffer" type="IN">Buffer with data to sent</param>
 * <param name="iBufferSize" type="IN">Maximum length of the buffer</param>
 * <param name="iFlags" type="IN">The flags parameter can be used to influence the behavior of the 
 *	function beyond the options specified for the associated socket. The semantics of this function 
 *	are determined by the socket options and the flags parameter. The latter is constructed by using the
 *	bitwise OR operator with any of the SOCKET_MSG values.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns number of sent bytes. 0 if failed.</result>
 */
STATICITF_DEF RTS_SSIZE CDECL SysSock2Send(RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCK2SEND) (RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, RTS_RESULT *pResult);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2SEND_NOTIMPLEMENTED)
	#define USE_SysSock2Send
	#define EXT_SysSock2Send
	#define GET_SysSock2Send(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2Send(p0,p1,p2,p3,p4)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2Send  FALSE
	#define EXP_SysSock2Send  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2Send
	#define EXT_SysSock2Send
	#define GET_SysSock2Send(fl)  ERR_OK
	#define CAL_SysSock2Send(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Send(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2Send  TRUE
	#define EXP_SysSock2Send  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2Send
	#define EXT_SysSock2Send
	#define GET_SysSock2Send(fl)  ERR_OK
	#define CAL_SysSock2Send(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Send(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2Send  TRUE
	#define EXP_SysSock2Send  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2Send
	#define EXT_SysSocket2SysSock2Send
	#define GET_SysSocket2SysSock2Send  ERR_OK
	#define CAL_SysSocket2SysSock2Send(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Send(p1,p2,p3,p4))
	#define CHK_SysSocket2SysSock2Send  TRUE
	#define EXP_SysSocket2SysSock2Send  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2Send
	#define EXT_SysSock2Send
	#define GET_SysSock2Send(fl)  ERR_OK
	#define CAL_SysSock2Send(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Send(p1,p2,p3,p4))
	#define CHK_SysSock2Send  TRUE
	#define EXP_SysSock2Send  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2Send
	#define EXT_SysSock2Send
	#define GET_SysSock2Send(fl)  ERR_OK
	#define CAL_SysSock2Send(p0,p1,p2,p3,p4) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2Send(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4))
	#define CHK_SysSock2Send  TRUE
	#define EXP_SysSock2Send  ERR_OK
#endif




/* ------------ UDP specific ------------ */

/**
 * <description>
 *	Receive a message from a connectionless socket (UDP)
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pbyBuffer" type="OUT">Buffer to read data from the socket</param>
 * <param name="iBufferSize" type="IN">Maximum length of the buffer</param>
 * <param name="iFlags" type="IN">The flags parameter can be used to influence the behavior of the 
 *	function beyond the options specified for the associated socket. The semantics of this function 
 *	are determined by the socket options and the flags parameter. The latter is constructed by using the
 *	bitwise OR operator with any of the SOCKET_MSG values.</param>
 * <param name="pSockAddr" type="IN">Socket address and port to receive data from</param>
 * <param name="iSockAddrSize" type="IN">Size of socket address structure</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns number of bytes received</result>
 */
STATICITF_DEF RTS_SSIZE CDECL SysSock2RecvFrom(RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCK2RECVFROM) (RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize, RTS_RESULT *pResult);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2RECVFROM_NOTIMPLEMENTED)
	#define USE_SysSock2RecvFrom
	#define EXT_SysSock2RecvFrom
	#define GET_SysSock2RecvFrom(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2RecvFrom(p0,p1,p2,p3,p4,p5,p6)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2RecvFrom  FALSE
	#define EXP_SysSock2RecvFrom  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2RecvFrom
	#define EXT_SysSock2RecvFrom
	#define GET_SysSock2RecvFrom(fl)  ERR_OK
	#define CAL_SysSock2RecvFrom(p0,p1,p2,p3,p4,p5,p6) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvFrom(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2RecvFrom  TRUE
	#define EXP_SysSock2RecvFrom  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2RecvFrom
	#define EXT_SysSock2RecvFrom
	#define GET_SysSock2RecvFrom(fl)  ERR_OK
	#define CAL_SysSock2RecvFrom(p0,p1,p2,p3,p4,p5,p6) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvFrom(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2RecvFrom  TRUE
	#define EXP_SysSock2RecvFrom  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2RecvFrom
	#define EXT_SysSocket2SysSock2RecvFrom
	#define GET_SysSocket2SysSock2RecvFrom  ERR_OK
	#define CAL_SysSocket2SysSock2RecvFrom(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvFrom(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSocket2SysSock2RecvFrom  TRUE
	#define EXP_SysSocket2SysSock2RecvFrom  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2RecvFrom
	#define EXT_SysSock2RecvFrom
	#define GET_SysSock2RecvFrom(fl)  ERR_OK
	#define CAL_SysSock2RecvFrom(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvFrom(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2RecvFrom  TRUE
	#define EXP_SysSock2RecvFrom  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2RecvFrom
	#define EXT_SysSock2RecvFrom
	#define GET_SysSock2RecvFrom(fl)  ERR_OK
	#define CAL_SysSock2RecvFrom(p0,p1,p2,p3,p4,p5,p6) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2RecvFrom(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2RecvFrom  TRUE
	#define EXP_SysSock2RecvFrom  ERR_OK
#endif




/**
 * <description>
 *	Send a message over a connectionless socket (UDP)
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pbyBuffer" type="IN">Buffer with send data</param>
 * <param name="iBufferSize" type="IN">Length of data to send</param>
 * <param name="iFlags" type="IN">The flags parameter can be used to influence the behavior of the 
 *	function beyond the options specified for the associated socket. The semantics of this function 
 *	are determined by the socket options and the flags parameter. The latter is constructed by using the
 *	bitwise OR operator with any of the SOCKET_MSG values.</param>
 * <param name="pSockAddr" type="IN">Socket address and port to sent data to</param>
 * <param name="iSockAddrSize" type="IN">Size of socket address structure</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns number of bytes sent</result>
 */
STATICITF_DEF RTS_SSIZE CDECL SysSock2SendTo(RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCK2SENDTO) (RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize, RTS_RESULT *pResult);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2SENDTO_NOTIMPLEMENTED)
	#define USE_SysSock2SendTo
	#define EXT_SysSock2SendTo
	#define GET_SysSock2SendTo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2SendTo(p0,p1,p2,p3,p4,p5,p6)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2SendTo  FALSE
	#define EXP_SysSock2SendTo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2SendTo
	#define EXT_SysSock2SendTo
	#define GET_SysSock2SendTo(fl)  ERR_OK
	#define CAL_SysSock2SendTo(p0,p1,p2,p3,p4,p5,p6) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2SendTo(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2SendTo  TRUE
	#define EXP_SysSock2SendTo  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2SendTo
	#define EXT_SysSock2SendTo
	#define GET_SysSock2SendTo(fl)  ERR_OK
	#define CAL_SysSock2SendTo(p0,p1,p2,p3,p4,p5,p6) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2SendTo(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2SendTo  TRUE
	#define EXP_SysSock2SendTo  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2SendTo
	#define EXT_SysSocket2SysSock2SendTo
	#define GET_SysSocket2SysSock2SendTo  ERR_OK
	#define CAL_SysSocket2SysSock2SendTo(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2SendTo(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSocket2SysSock2SendTo  TRUE
	#define EXP_SysSocket2SysSock2SendTo  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2SendTo
	#define EXT_SysSock2SendTo
	#define GET_SysSock2SendTo(fl)  ERR_OK
	#define CAL_SysSock2SendTo(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2SendTo(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2SendTo  TRUE
	#define EXP_SysSock2SendTo  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2SendTo
	#define EXT_SysSock2SendTo
	#define GET_SysSock2SendTo(fl)  ERR_OK
	#define CAL_SysSock2SendTo(p0,p1,p2,p3,p4,p5,p6) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2SendTo(((ISysSocket2*)p0)->pBase->hInstance,p1,p2,p3,p4,p5,p6))
	#define CHK_SysSock2SendTo  TRUE
	#define EXP_SysSock2SendTo  ERR_OK
#endif




/**
 * <description>
 *	Returns the socket address of the peer to which a socket is connected.
 *  The SysSockGetPeerName function can be used only on a connected socket. 
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="OUT">Socket address of the peer</param>
 * <param name="piSockAddrSize" type="INOUT">Pointer to size of socket address structure</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2GetPeerName(RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize);
typedef RTS_RESULT (CDECL * PFSYSSOCK2GETPEERNAME) (RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2GETPEERNAME_NOTIMPLEMENTED)
	#define USE_SysSock2GetPeerName
	#define EXT_SysSock2GetPeerName
	#define GET_SysSock2GetPeerName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2GetPeerName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2GetPeerName  FALSE
	#define EXP_SysSock2GetPeerName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2GetPeerName
	#define EXT_SysSock2GetPeerName
	#define GET_SysSock2GetPeerName(fl)  ERR_OK
	#define CAL_SysSock2GetPeerName(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetPeerName(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2GetPeerName  TRUE
	#define EXP_SysSock2GetPeerName  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2GetPeerName
	#define EXT_SysSock2GetPeerName
	#define GET_SysSock2GetPeerName(fl)  ERR_OK
	#define CAL_SysSock2GetPeerName(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetPeerName(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2GetPeerName  TRUE
	#define EXP_SysSock2GetPeerName  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2GetPeerName
	#define EXT_SysSocket2SysSock2GetPeerName
	#define GET_SysSocket2SysSock2GetPeerName  ERR_OK
	#define CAL_SysSocket2SysSock2GetPeerName(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetPeerName(p1,p2))
	#define CHK_SysSocket2SysSock2GetPeerName  TRUE
	#define EXP_SysSocket2SysSock2GetPeerName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2GetPeerName
	#define EXT_SysSock2GetPeerName
	#define GET_SysSock2GetPeerName(fl)  ERR_OK
	#define CAL_SysSock2GetPeerName(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetPeerName(p1,p2))
	#define CHK_SysSock2GetPeerName  TRUE
	#define EXP_SysSock2GetPeerName  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2GetPeerName
	#define EXT_SysSock2GetPeerName
	#define GET_SysSock2GetPeerName(fl)  ERR_OK
	#define CAL_SysSock2GetPeerName(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetPeerName(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2GetPeerName  TRUE
	#define EXP_SysSock2GetPeerName  ERR_OK
#endif




/**
 * <description>
 *	Returns the socket address of the local socket.
 *  Usually this function is called to retrieve the local socket address on multi-homed hosts or to get 
 *  the local port number in client implementations. If the socket is not locally bound to an address,
 *  the function will fail until the socket is connected.
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="OUT">Socket address of the local socket</param>
 * <param name="piSockAddrSize" type="INOUT">Pointer to size of socket address structure</param>
 * <result>error code</result>
 */
STATICITF_DEF RTS_RESULT CDECL SysSock2GetSockName(RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize);
typedef RTS_RESULT (CDECL * PFSYSSOCK2GETSOCKNAME) (RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2GETSOCKNAME_NOTIMPLEMENTED)
	#define USE_SysSock2GetSockName
	#define EXT_SysSock2GetSockName
	#define GET_SysSock2GetSockName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2GetSockName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2GetSockName  FALSE
	#define EXP_SysSock2GetSockName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2GetSockName
	#define EXT_SysSock2GetSockName
	#define GET_SysSock2GetSockName(fl)  ERR_OK
	#define CAL_SysSock2GetSockName(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetSockName(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2GetSockName  TRUE
	#define EXP_SysSock2GetSockName  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2GetSockName
	#define EXT_SysSock2GetSockName
	#define GET_SysSock2GetSockName(fl)  ERR_OK
	#define CAL_SysSock2GetSockName(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetSockName(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2GetSockName  TRUE
	#define EXP_SysSock2GetSockName  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2GetSockName
	#define EXT_SysSocket2SysSock2GetSockName
	#define GET_SysSocket2SysSock2GetSockName  ERR_OK
	#define CAL_SysSocket2SysSock2GetSockName(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetSockName(p1,p2))
	#define CHK_SysSocket2SysSock2GetSockName  TRUE
	#define EXP_SysSocket2SysSock2GetSockName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2GetSockName
	#define EXT_SysSock2GetSockName
	#define GET_SysSock2GetSockName(fl)  ERR_OK
	#define CAL_SysSock2GetSockName(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetSockName(p1,p2))
	#define CHK_SysSock2GetSockName  TRUE
	#define EXP_SysSock2GetSockName  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2GetSockName
	#define EXT_SysSock2GetSockName
	#define GET_SysSock2GetSockName(fl)  ERR_OK
	#define CAL_SysSock2GetSockName(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ISysSocket2*)p0)->ISysSock2GetSockName(((ISysSocket2*)p0)->pBase->hInstance,p1,p2))
	#define CHK_SysSock2GetSockName  TRUE
	#define EXP_SysSock2GetSockName  ERR_OK
#endif




/**
 * <description>
 *	Returns the CLASSID of the socket implementation. Is used to detect, which kind of socket implementation is behind the socket handle.
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>CLASSID of the SysSocket2 implementation</result>
 */
STATICITF_DEF CLASSID CDECL SysSock2GetClassId(RTS_HANDLE hSocket, RTS_RESULT *pResult);
typedef CLASSID (CDECL * PFSYSSOCK2GETCLASSID) (RTS_HANDLE hSocket, RTS_RESULT *pResult);
#if defined(SYSSOCKET2_NOTIMPLEMENTED) || defined(SYSSOCK2GETCLASSID_NOTIMPLEMENTED)
	#define USE_SysSock2GetClassId
	#define EXT_SysSock2GetClassId
	#define GET_SysSock2GetClassId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSock2GetClassId(p0,p1)  (CLASSID)ERR_NOTIMPLEMENTED
	#define CHK_SysSock2GetClassId  FALSE
	#define EXP_SysSock2GetClassId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSock2GetClassId
	#define EXT_SysSock2GetClassId
	#define GET_SysSock2GetClassId(fl)  ERR_OK
	#define CAL_SysSock2GetClassId(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2GetClassId(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2GetClassId  TRUE
	#define EXP_SysSock2GetClassId  ERR_OK
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2_EXTERNAL)
	#define USE_SysSock2GetClassId
	#define EXT_SysSock2GetClassId
	#define GET_SysSock2GetClassId(fl)  ERR_OK
	#define CAL_SysSock2GetClassId(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2GetClassId(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2GetClassId  TRUE
	#define EXP_SysSock2GetClassId  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2SysSock2GetClassId
	#define EXT_SysSocket2SysSock2GetClassId
	#define GET_SysSocket2SysSock2GetClassId  ERR_OK
	#define CAL_SysSocket2SysSock2GetClassId(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2GetClassId(p1))
	#define CHK_SysSocket2SysSock2GetClassId  TRUE
	#define EXP_SysSocket2SysSock2GetClassId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSock2GetClassId
	#define EXT_SysSock2GetClassId
	#define GET_SysSock2GetClassId(fl)  ERR_OK
	#define CAL_SysSock2GetClassId(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2GetClassId(p1))
	#define CHK_SysSock2GetClassId  TRUE
	#define EXP_SysSock2GetClassId  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_SysSock2GetClassId
	#define EXT_SysSock2GetClassId
	#define GET_SysSock2GetClassId(fl)  ERR_OK
	#define CAL_SysSock2GetClassId(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ISysSocket2*)p0)->ISysSock2GetClassId(((ISysSocket2*)p0)->pBase->hInstance,p1))
	#define CHK_SysSock2GetClassId  TRUE
	#define EXP_SysSock2GetClassId  ERR_OK
#endif




#ifdef __cplusplus
}
#endif

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 * Enumeration: SysSocket2_Type
 * </description>
 * <element name="SYSSOCKET2_TYPE_STD" type=OUT>Standard socket</element>
 * <element name="SYSSOCKET2_TYPE_TLS" type=OUT>TLS socket</element>
 * <element name="SYSSOCKET2_TYPE_STARTTLS" type=OUT>Standard socket with the option to enable TLS later via socket option SOCKET_SO_STARTTLS</element>
*/
#define SYSSOCKET2_TYPE_STD    RTS_IEC_INT_C(0x1)
#define SYSSOCKET2_TYPE_TLS    RTS_IEC_INT_C(0x2)
#define SYSSOCKET2_TYPE_STARTTLS    RTS_IEC_INT_C(0x3)
/* Typed enum definition */
#define SYSSOCKET2_TYPE    RTS_IEC_INT

/**
 * <description>
 * Structure: SysSocket2_StdSockets
 * </description>
 * <element name="i32AddressFamily" type=INOUT></element>
 * <element name="i32Type" type=INOUT></element>
 * <element name="i32Protocol" type=INOUT></element>
*/
typedef struct tagSysSocket2_StdSockets
{
	RTS_IEC_INT i32AddressFamily;
	RTS_IEC_DINT i32Type;
	RTS_IEC_DINT i32Protocol;
} SysSocket2_StdSockets;

/**
 * <description>
 * Structure: SysSocket2_TlsSockets
 * </description>
 * <element name="hTlsContext" type=INOUT></element>
*/
typedef struct tagSysSocket2_TlsSockets
{
	RTS_IEC_HANDLE hTlsContext;
} SysSocket2_TlsSockets;

/**
 * <description>
 * Structure: SysSocket2_SpecificParameter
 * </description>
 * <element name="stdSockets" type=INOUT></element>
 * <element name="tlsSockets" type=INOUT></element>
*/
typedef union
{
	SysSocket2_StdSockets stdSockets;
	SysSocket2_TlsSockets tlsSockets;
} SysSocket2_SpecificParameter;

/**
 * <description>
 * This structure represents the parameters to create a specific socket type
 * Set the parameterType to your desired socket type. By now Std or TLS sockets
 * are available.
 * The specifica member contains the parameters needed to create this socket type
 * </description>
 * <element name="parameterType" type=INOUT></element>
 * <element name="specific" type=INOUT></element>
*/
typedef struct tagSysSocket2_Parameter
{
	RTS_IEC_INT parameterType;
	SysSocket2_SpecificParameter specific;
} SysSocket2_Parameter;

#ifdef __cplusplus
}
#endif




typedef struct
{
	IBase_C *pBase;
	PFSYSSOCK2CREATE ISysSock2Create;
 	PFSYSSOCK2CLOSE ISysSock2Close;
 	PFSYSSOCK2SETOPTION ISysSock2SetOption;
 	PFSYSSOCK2GETOPTION ISysSock2GetOption;
 	PFSYSSOCK2BIND ISysSock2Bind;
 	PFSYSSOCK2LISTEN ISysSock2Listen;
 	PFSYSSOCK2ACCEPT ISysSock2Accept;
 	PFSYSSOCK2CONNECT ISysSock2Connect;
 	PFSYSSOCK2SELECT ISysSock2Select;
 	PFSYSSOCK2SHUTDOWN ISysSock2Shutdown;
 	PFSYSSOCK2IOCTL ISysSock2Ioctl;
 	PFSYSSOCK2FDISSET ISysSock2FdIsset;
 	PFSYSSOCK2FDINIT ISysSock2FdInit;
 	PFSYSSOCK2RECV ISysSock2Recv;
 	PFSYSSOCK2RECVMSG ISysSock2RecvMsg;
 	PFSYSSOCK2SEND ISysSock2Send;
 	PFSYSSOCK2RECVFROM ISysSock2RecvFrom;
 	PFSYSSOCK2SENDTO ISysSock2SendTo;
 	PFSYSSOCK2GETPEERNAME ISysSock2GetPeerName;
 	PFSYSSOCK2GETSOCKNAME ISysSock2GetSockName;
 	PFSYSSOCK2GETCLASSID ISysSock2GetClassId;
 } ISysSocket2_C;

#ifdef CPLUSPLUS
class ISysSocket2 : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISysSock2Create(RTS_HANDLE hISysSocket, CLASSID ClassId, SysSock_Parameter *pParameter, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysSock2Close(void) =0;
		virtual RTS_RESULT CDECL ISysSock2SetOption(RTS_I32 iLevel, RTS_I32 iOption, char *pcOptionValue, RTS_I32 iOptionLen) =0;
		virtual RTS_RESULT CDECL ISysSock2GetOption(RTS_I32 iLevel, RTS_I32 iOption, char *pcOptionValue, RTS_I32 *piOptionLen) =0;
		virtual RTS_RESULT CDECL ISysSock2Bind(SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize) =0;
		virtual RTS_RESULT CDECL ISysSock2Listen(RTS_I32 iMaxConnections) =0;
		virtual RTS_HANDLE CDECL ISysSock2Accept(RTS_HANDLE hISysSocket, CLASSID ClassId, RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysSock2Connect(SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize) =0;
		virtual RTS_RESULT CDECL ISysSock2Select(RTS_I32 iWidth, SOCKET_FD_SET *fdRead, SOCKET_FD_SET *fdWrite, SOCKET_FD_SET *fdExcept, SOCKET_TIMEVAL *ptvTimeout, RTS_I32 *pnReady) =0;
		virtual RTS_RESULT CDECL ISysSock2Shutdown(RTS_I32 iHow) =0;
		virtual RTS_RESULT CDECL ISysSock2Ioctl(RTS_I32 iCommand, RTS_I32 *piParameter) =0;
		virtual RTS_BOOL CDECL ISysSock2FdIsset(SOCKET_FD_SET *pfs) =0;
		virtual RTS_RESULT CDECL ISysSock2FdInit(SOCKET_FD_SET *pfs) =0;
		virtual RTS_SSIZE CDECL ISysSock2Recv(char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, RTS_RESULT *pResult) =0;
		virtual RTS_SSIZE CDECL ISysSock2RecvMsg(SOCK_RECVMSG_MSG* pMessage, RTS_UI32 flags, RTS_RESULT* pResult) =0;
		virtual RTS_SSIZE CDECL ISysSock2Send(char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, RTS_RESULT *pResult) =0;
		virtual RTS_SSIZE CDECL ISysSock2RecvFrom(char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize, RTS_RESULT *pResult) =0;
		virtual RTS_SSIZE CDECL ISysSock2SendTo(char *pbyBuffer, RTS_SSIZE iBufferSize, RTS_I32 iFlags, SOCKADDRESS *pSockAddr, RTS_I32 iSockAddrSize, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysSock2GetPeerName(SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize) =0;
		virtual RTS_RESULT CDECL ISysSock2GetSockName(SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize) =0;
		virtual CLASSID CDECL ISysSock2GetClassId(RTS_RESULT *pResult) =0;
};
	#ifndef ITF_SysSocket2
		#define ITF_SysSocket2 static ISysSocket2 *pISysSocket2 = NULL;
	#endif
	#define EXTITF_SysSocket2
#else	/*CPLUSPLUS*/
	typedef ISysSocket2_C		ISysSocket2;
	#ifndef ITF_SysSocket2
		#define ITF_SysSocket2
	#endif
	#define EXTITF_SysSocket2
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSSOCKET2ITF_H_*/
