 /**
 * <interfacename>SysSocket2</interfacename>
 * <description></description>
 *
 * <copyright></copyright>
 */


	
	
#ifndef _SYSSOCKET2EXTERNALITF_H_
#define _SYSSOCKET2EXTERNALITF_H_

#include "CmpStd.h"

 

 




/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Enum: SysSocket2_Type</description>
 */
#define SYSSOCKET2_TYPE_STD    RTS_IEC_INT_C(0x1)	
#define SYSSOCKET2_TYPE_TLS    RTS_IEC_INT_C(0x2)	
/* Typed enum definition */
#define SYSSOCKET2_TYPE    RTS_IEC_INT

/**
 * <description>SysSocket2_StdSockets</description>
 */
typedef struct tagSysSocket2_StdSockets
{
	RTS_IEC_INT i32AddressFamily;		
	RTS_IEC_DINT i32Type;		
	RTS_IEC_DINT i32Protocol;		
} SysSocket2_StdSockets;

/**
 * <description>SysSocket2_TlsSockets</description>
 */
typedef struct tagSysSocket2_TlsSockets
{
	RTS_IEC_HANDLE hTlsContext;		
} SysSocket2_TlsSockets;

/**
 * <description>SysSocket2_SpecificParameter</description>
 */
typedef union
{
	SysSocket2_StdSockets stdSockets;		
	SysSocket2_TlsSockets tlsSockets;		
} SysSocket2_SpecificParameter;

/**
 * This structure represents the parameters to create a specific socket type
 * Set the parameterType to your desired socket type. By now Std or TLS sockets
 * are available.
 * The specifica member contains the parameters needed to create this socket type
 */
typedef struct tagSysSocket2_Parameter
{
	RTS_IEC_INT parameterType;		
	SysSocket2_SpecificParameter specific;		
} SysSocket2_Parameter;

/**
 * | Accept the next incoming TCP connection.
 * | This functions calls the function accept of the operating system, which can accept a request to connect
 * | to a socket. A new descriptor (handle) for the socket is returned. The original socket is reset to the
 * | "listening" state (see SysSockListen).
 *
 * .. note::
 * 
 *		For TLS Sockets the blocking mode will be set to **nonblocking** for the new client sockets. 
 *		If a blocking behavior is needed use |SysSock2Ioctl| to set the socket to blocking mode. This
 *		reflects the behavior of |SysSock2Create| for TLS sockets.
 *
 * :return: Handle to the new accepted socket or RTS_INVALID_HANDLE if failed.
 */
typedef struct tagsyssock2accept_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address of the client, who is connected */
	RTS_IEC_DINT *pdiSockAddrSize;		/* VAR_INPUT */	/* Pointer to size of socket address structure */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_HANDLE SysSock2Accept;		/* VAR_OUTPUT */	
} syssock2accept_struct;

void CDECL CDECL_EXT syssock2accept(syssock2accept_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2ACCEPT_IEC) (syssock2accept_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2ACCEPT_NOTIMPLEMENTED)
	#define USE_syssock2accept
	#define EXT_syssock2accept
	#define GET_syssock2accept(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2accept(p0) 
	#define CHK_syssock2accept  FALSE
	#define EXP_syssock2accept  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2accept
	#define EXT_syssock2accept
	#define GET_syssock2accept(fl)  CAL_CMGETAPI( "syssock2accept" ) 
	#define CAL_syssock2accept  syssock2accept
	#define CHK_syssock2accept  TRUE
	#define EXP_syssock2accept  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2accept", (RTS_UINTPTR)syssock2accept, 1, 0x17C31CE6, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2accept
	#define EXT_syssock2accept
	#define GET_syssock2accept(fl)  CAL_CMGETAPI( "syssock2accept" ) 
	#define CAL_syssock2accept  syssock2accept
	#define CHK_syssock2accept  TRUE
	#define EXP_syssock2accept  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2accept", (RTS_UINTPTR)syssock2accept, 1, 0x17C31CE6, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2accept
	#define EXT_SysSocket2externalsyssock2accept
	#define GET_SysSocket2externalsyssock2accept  ERR_OK
	#define CAL_SysSocket2externalsyssock2accept  syssock2accept
	#define CHK_SysSocket2externalsyssock2accept  TRUE
	#define EXP_SysSocket2externalsyssock2accept  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2accept", (RTS_UINTPTR)syssock2accept, 1, 0x17C31CE6, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2accept
	#define EXT_syssock2accept
	#define GET_syssock2accept(fl)  CAL_CMGETAPI( "syssock2accept" ) 
	#define CAL_syssock2accept  syssock2accept
	#define CHK_syssock2accept  TRUE
	#define EXP_syssock2accept  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2accept", (RTS_UINTPTR)syssock2accept, 1, 0x17C31CE6, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2accept  PFSYSSOCK2ACCEPT_IEC pfsyssock2accept;
	#define EXT_syssock2accept  extern PFSYSSOCK2ACCEPT_IEC pfsyssock2accept;
	#define GET_syssock2accept(fl)  s_pfCMGetAPI2( "syssock2accept", (RTS_VOID_FCTPTR *)&pfsyssock2accept, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x17C31CE6, 0x03050E00)
	#define CAL_syssock2accept  pfsyssock2accept
	#define CHK_syssock2accept  (pfsyssock2accept != NULL)
	#define EXP_syssock2accept   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2accept", (RTS_UINTPTR)syssock2accept, 1, 0x17C31CE6, 0x03050E00) 
#endif


/**
 * | Bind a socket to a socket address and port number.
 * | This functions calls the function bind of the operating system, which will assign a local address
 *   to a socket that has already been allocated to an address by |SysSock2Create|, but not yet fixed uniquely.
 *   This is usually done previous to a call to functions like |SysSock2Listen| or |SysSock2Accept|.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2bind_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Spcket address */
	RTS_IEC_DINT diSockAddrSize;		/* VAR_INPUT */	/* Size of the socket address structure */
	RTS_IEC_RESULT SysSock2Bind;		/* VAR_OUTPUT */	
} syssock2bind_struct;

void CDECL CDECL_EXT syssock2bind(syssock2bind_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2BIND_IEC) (syssock2bind_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2BIND_NOTIMPLEMENTED)
	#define USE_syssock2bind
	#define EXT_syssock2bind
	#define GET_syssock2bind(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2bind(p0) 
	#define CHK_syssock2bind  FALSE
	#define EXP_syssock2bind  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2bind
	#define EXT_syssock2bind
	#define GET_syssock2bind(fl)  CAL_CMGETAPI( "syssock2bind" ) 
	#define CAL_syssock2bind  syssock2bind
	#define CHK_syssock2bind  TRUE
	#define EXP_syssock2bind  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2bind", (RTS_UINTPTR)syssock2bind, 1, 0x1AA54880, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2bind
	#define EXT_syssock2bind
	#define GET_syssock2bind(fl)  CAL_CMGETAPI( "syssock2bind" ) 
	#define CAL_syssock2bind  syssock2bind
	#define CHK_syssock2bind  TRUE
	#define EXP_syssock2bind  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2bind", (RTS_UINTPTR)syssock2bind, 1, 0x1AA54880, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2bind
	#define EXT_SysSocket2externalsyssock2bind
	#define GET_SysSocket2externalsyssock2bind  ERR_OK
	#define CAL_SysSocket2externalsyssock2bind  syssock2bind
	#define CHK_SysSocket2externalsyssock2bind  TRUE
	#define EXP_SysSocket2externalsyssock2bind  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2bind", (RTS_UINTPTR)syssock2bind, 1, 0x1AA54880, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2bind
	#define EXT_syssock2bind
	#define GET_syssock2bind(fl)  CAL_CMGETAPI( "syssock2bind" ) 
	#define CAL_syssock2bind  syssock2bind
	#define CHK_syssock2bind  TRUE
	#define EXP_syssock2bind  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2bind", (RTS_UINTPTR)syssock2bind, 1, 0x1AA54880, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2bind  PFSYSSOCK2BIND_IEC pfsyssock2bind;
	#define EXT_syssock2bind  extern PFSYSSOCK2BIND_IEC pfsyssock2bind;
	#define GET_syssock2bind(fl)  s_pfCMGetAPI2( "syssock2bind", (RTS_VOID_FCTPTR *)&pfsyssock2bind, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x1AA54880, 0x03050E00)
	#define CAL_syssock2bind  pfsyssock2bind
	#define CHK_syssock2bind  (pfsyssock2bind != NULL)
	#define EXP_syssock2bind   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2bind", (RTS_UINTPTR)syssock2bind, 1, 0x1AA54880, 0x03050E00) 
#endif


/**
 *  | Close a socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2close_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_RESULT SysSock2Close;		/* VAR_OUTPUT */	
} syssock2close_struct;

void CDECL CDECL_EXT syssock2close(syssock2close_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2CLOSE_IEC) (syssock2close_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2CLOSE_NOTIMPLEMENTED)
	#define USE_syssock2close
	#define EXT_syssock2close
	#define GET_syssock2close(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2close(p0) 
	#define CHK_syssock2close  FALSE
	#define EXP_syssock2close  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2close
	#define EXT_syssock2close
	#define GET_syssock2close(fl)  CAL_CMGETAPI( "syssock2close" ) 
	#define CAL_syssock2close  syssock2close
	#define CHK_syssock2close  TRUE
	#define EXP_syssock2close  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2close", (RTS_UINTPTR)syssock2close, 1, 0xEE4C98E7, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2close
	#define EXT_syssock2close
	#define GET_syssock2close(fl)  CAL_CMGETAPI( "syssock2close" ) 
	#define CAL_syssock2close  syssock2close
	#define CHK_syssock2close  TRUE
	#define EXP_syssock2close  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2close", (RTS_UINTPTR)syssock2close, 1, 0xEE4C98E7, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2close
	#define EXT_SysSocket2externalsyssock2close
	#define GET_SysSocket2externalsyssock2close  ERR_OK
	#define CAL_SysSocket2externalsyssock2close  syssock2close
	#define CHK_SysSocket2externalsyssock2close  TRUE
	#define EXP_SysSocket2externalsyssock2close  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2close", (RTS_UINTPTR)syssock2close, 1, 0xEE4C98E7, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2close
	#define EXT_syssock2close
	#define GET_syssock2close(fl)  CAL_CMGETAPI( "syssock2close" ) 
	#define CAL_syssock2close  syssock2close
	#define CHK_syssock2close  TRUE
	#define EXP_syssock2close  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2close", (RTS_UINTPTR)syssock2close, 1, 0xEE4C98E7, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2close  PFSYSSOCK2CLOSE_IEC pfsyssock2close;
	#define EXT_syssock2close  extern PFSYSSOCK2CLOSE_IEC pfsyssock2close;
	#define GET_syssock2close(fl)  s_pfCMGetAPI2( "syssock2close", (RTS_VOID_FCTPTR *)&pfsyssock2close, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xEE4C98E7, 0x03050E00)
	#define CAL_syssock2close  pfsyssock2close
	#define CHK_syssock2close  (pfsyssock2close != NULL)
	#define EXP_syssock2close   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2close", (RTS_UINTPTR)syssock2close, 1, 0xEE4C98E7, 0x03050E00) 
#endif


/**
 * | Connect as a client to a TCP server.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2connect_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address of the server to connect to */
	RTS_IEC_DINT diSockAddrSize;		/* VAR_INPUT */	/* Size of socket address structure */
	RTS_IEC_RESULT SysSock2Connect;		/* VAR_OUTPUT */	
} syssock2connect_struct;

void CDECL CDECL_EXT syssock2connect(syssock2connect_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2CONNECT_IEC) (syssock2connect_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2CONNECT_NOTIMPLEMENTED)
	#define USE_syssock2connect
	#define EXT_syssock2connect
	#define GET_syssock2connect(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2connect(p0) 
	#define CHK_syssock2connect  FALSE
	#define EXP_syssock2connect  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2connect
	#define EXT_syssock2connect
	#define GET_syssock2connect(fl)  CAL_CMGETAPI( "syssock2connect" ) 
	#define CAL_syssock2connect  syssock2connect
	#define CHK_syssock2connect  TRUE
	#define EXP_syssock2connect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2connect", (RTS_UINTPTR)syssock2connect, 1, 0x482EC7EC, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2connect
	#define EXT_syssock2connect
	#define GET_syssock2connect(fl)  CAL_CMGETAPI( "syssock2connect" ) 
	#define CAL_syssock2connect  syssock2connect
	#define CHK_syssock2connect  TRUE
	#define EXP_syssock2connect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2connect", (RTS_UINTPTR)syssock2connect, 1, 0x482EC7EC, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2connect
	#define EXT_SysSocket2externalsyssock2connect
	#define GET_SysSocket2externalsyssock2connect  ERR_OK
	#define CAL_SysSocket2externalsyssock2connect  syssock2connect
	#define CHK_SysSocket2externalsyssock2connect  TRUE
	#define EXP_SysSocket2externalsyssock2connect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2connect", (RTS_UINTPTR)syssock2connect, 1, 0x482EC7EC, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2connect
	#define EXT_syssock2connect
	#define GET_syssock2connect(fl)  CAL_CMGETAPI( "syssock2connect" ) 
	#define CAL_syssock2connect  syssock2connect
	#define CHK_syssock2connect  TRUE
	#define EXP_syssock2connect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2connect", (RTS_UINTPTR)syssock2connect, 1, 0x482EC7EC, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2connect  PFSYSSOCK2CONNECT_IEC pfsyssock2connect;
	#define EXT_syssock2connect  extern PFSYSSOCK2CONNECT_IEC pfsyssock2connect;
	#define GET_syssock2connect(fl)  s_pfCMGetAPI2( "syssock2connect", (RTS_VOID_FCTPTR *)&pfsyssock2connect, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x482EC7EC, 0x03050E00)
	#define CAL_syssock2connect  pfsyssock2connect
	#define CHK_syssock2connect  (pfsyssock2connect != NULL)
	#define EXP_syssock2connect   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2connect", (RTS_UINTPTR)syssock2connect, 1, 0x482EC7EC, 0x03050E00) 
#endif


/**
 * Create a new socket and return the socket handle.
 * By now STD or TLS sockets are available. Using a STD socket results in the default
 * socket behavior.
 *
 * .. note::
 *
 *     Using the TLS socket results in an IPv4 TCP socket where the TLS protocol is used. 
 *     Since additional handshaking has to be done, connection establishment takes 
 *     some time longer. By default, these sockets are created as **nonblocking**. 
 *     If a blocking behavior is needed this **has to be set explicitly**. Be aware that 
 *     blocking TLS sockets can lead to deadlock situations if the peer does not send 
 *     TLS data. This is a potential security risk, typically for server implementations.
 *     An attacker can create a connection and then stop sending TLS data, resulting in 
 *     a denial of service attack. This situation is avoided with nonblocking sockets.
 *
 *     To avoid this potential risk by default TLS based sockets are created as 
 *     **nonblocking** by default. This applies to handles created by |SysSock2Create| 
 *     and |SysSock2Accept|. If this has to be changed use the |SysSock2Ioctl| to 
 *     setup the desired behavior.
 *     
 *
 * :return: 
 *     A handle to the created socket or INVALID_HANDLE if something went wrong.  
 *     The return is a descriptor (handle) of the new socket, which is required as input parameter by other 
 *     library functions like |SysSock2Bind|, |SysSock2Connect| etc.
 */
typedef struct tagsyssock2create_struct
{
	SysSocket2_Parameter *pParameter;	/* VAR_INPUT */	/* Class specific parameters. See SysSock_Parameter for details */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_HANDLE SysSock2Create;		/* VAR_OUTPUT */	
} syssock2create_struct;

void CDECL CDECL_EXT syssock2create(syssock2create_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2CREATE_IEC) (syssock2create_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2CREATE_NOTIMPLEMENTED)
	#define USE_syssock2create
	#define EXT_syssock2create
	#define GET_syssock2create(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2create(p0) 
	#define CHK_syssock2create  FALSE
	#define EXP_syssock2create  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2create
	#define EXT_syssock2create
	#define GET_syssock2create(fl)  CAL_CMGETAPI( "syssock2create" ) 
	#define CAL_syssock2create  syssock2create
	#define CHK_syssock2create  TRUE
	#define EXP_syssock2create  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2create", (RTS_UINTPTR)syssock2create, 1, 0xF631E049, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2create
	#define EXT_syssock2create
	#define GET_syssock2create(fl)  CAL_CMGETAPI( "syssock2create" ) 
	#define CAL_syssock2create  syssock2create
	#define CHK_syssock2create  TRUE
	#define EXP_syssock2create  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2create", (RTS_UINTPTR)syssock2create, 1, 0xF631E049, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2create
	#define EXT_SysSocket2externalsyssock2create
	#define GET_SysSocket2externalsyssock2create  ERR_OK
	#define CAL_SysSocket2externalsyssock2create  syssock2create
	#define CHK_SysSocket2externalsyssock2create  TRUE
	#define EXP_SysSocket2externalsyssock2create  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2create", (RTS_UINTPTR)syssock2create, 1, 0xF631E049, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2create
	#define EXT_syssock2create
	#define GET_syssock2create(fl)  CAL_CMGETAPI( "syssock2create" ) 
	#define CAL_syssock2create  syssock2create
	#define CHK_syssock2create  TRUE
	#define EXP_syssock2create  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2create", (RTS_UINTPTR)syssock2create, 1, 0xF631E049, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2create  PFSYSSOCK2CREATE_IEC pfsyssock2create;
	#define EXT_syssock2create  extern PFSYSSOCK2CREATE_IEC pfsyssock2create;
	#define GET_syssock2create(fl)  s_pfCMGetAPI2( "syssock2create", (RTS_VOID_FCTPTR *)&pfsyssock2create, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xF631E049, 0x03050E00)
	#define CAL_syssock2create  pfsyssock2create
	#define CHK_syssock2create  (pfsyssock2create != NULL)
	#define EXP_syssock2create   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2create", (RTS_UINTPTR)syssock2create, 1, 0xF631E049, 0x03050E00) 
#endif


/**
 * | Add a socket to a socket set.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2fdinit_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Socket to add */
	SOCKET_FD_SET *pfs;					/* VAR_INPUT */	/* Socket Set */
	RTS_IEC_RESULT SysSock2FdInit;		/* VAR_OUTPUT */	
} syssock2fdinit_struct;

void CDECL CDECL_EXT syssock2fdinit(syssock2fdinit_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2FDINIT_IEC) (syssock2fdinit_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2FDINIT_NOTIMPLEMENTED)
	#define USE_syssock2fdinit
	#define EXT_syssock2fdinit
	#define GET_syssock2fdinit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2fdinit(p0) 
	#define CHK_syssock2fdinit  FALSE
	#define EXP_syssock2fdinit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2fdinit
	#define EXT_syssock2fdinit
	#define GET_syssock2fdinit(fl)  CAL_CMGETAPI( "syssock2fdinit" ) 
	#define CAL_syssock2fdinit  syssock2fdinit
	#define CHK_syssock2fdinit  TRUE
	#define EXP_syssock2fdinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdinit", (RTS_UINTPTR)syssock2fdinit, 1, 0xAD038A02, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2fdinit
	#define EXT_syssock2fdinit
	#define GET_syssock2fdinit(fl)  CAL_CMGETAPI( "syssock2fdinit" ) 
	#define CAL_syssock2fdinit  syssock2fdinit
	#define CHK_syssock2fdinit  TRUE
	#define EXP_syssock2fdinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdinit", (RTS_UINTPTR)syssock2fdinit, 1, 0xAD038A02, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2fdinit
	#define EXT_SysSocket2externalsyssock2fdinit
	#define GET_SysSocket2externalsyssock2fdinit  ERR_OK
	#define CAL_SysSocket2externalsyssock2fdinit  syssock2fdinit
	#define CHK_SysSocket2externalsyssock2fdinit  TRUE
	#define EXP_SysSocket2externalsyssock2fdinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdinit", (RTS_UINTPTR)syssock2fdinit, 1, 0xAD038A02, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2fdinit
	#define EXT_syssock2fdinit
	#define GET_syssock2fdinit(fl)  CAL_CMGETAPI( "syssock2fdinit" ) 
	#define CAL_syssock2fdinit  syssock2fdinit
	#define CHK_syssock2fdinit  TRUE
	#define EXP_syssock2fdinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdinit", (RTS_UINTPTR)syssock2fdinit, 1, 0xAD038A02, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2fdinit  PFSYSSOCK2FDINIT_IEC pfsyssock2fdinit;
	#define EXT_syssock2fdinit  extern PFSYSSOCK2FDINIT_IEC pfsyssock2fdinit;
	#define GET_syssock2fdinit(fl)  s_pfCMGetAPI2( "syssock2fdinit", (RTS_VOID_FCTPTR *)&pfsyssock2fdinit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xAD038A02, 0x03050E00)
	#define CAL_syssock2fdinit  pfsyssock2fdinit
	#define CHK_syssock2fdinit  (pfsyssock2fdinit != NULL)
	#define EXP_syssock2fdinit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdinit", (RTS_UINTPTR)syssock2fdinit, 1, 0xAD038A02, 0x03050E00) 
#endif


/**
 * | Check if a socket is inside of a set.
 * :return: TRUE if it is inside the set, FALSE if not.
 */
typedef struct tagsyssock2fdisset_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Socket to check */
	SOCKET_FD_SET *pfs;					/* VAR_INPUT */	/* Socket Set */
	RTS_IEC_BOOL SysSock2FdIsset;		/* VAR_OUTPUT */	
} syssock2fdisset_struct;

void CDECL CDECL_EXT syssock2fdisset(syssock2fdisset_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2FDISSET_IEC) (syssock2fdisset_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2FDISSET_NOTIMPLEMENTED)
	#define USE_syssock2fdisset
	#define EXT_syssock2fdisset
	#define GET_syssock2fdisset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2fdisset(p0) 
	#define CHK_syssock2fdisset  FALSE
	#define EXP_syssock2fdisset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2fdisset
	#define EXT_syssock2fdisset
	#define GET_syssock2fdisset(fl)  CAL_CMGETAPI( "syssock2fdisset" ) 
	#define CAL_syssock2fdisset  syssock2fdisset
	#define CHK_syssock2fdisset  TRUE
	#define EXP_syssock2fdisset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdisset", (RTS_UINTPTR)syssock2fdisset, 1, 0x838C8AA3, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2fdisset
	#define EXT_syssock2fdisset
	#define GET_syssock2fdisset(fl)  CAL_CMGETAPI( "syssock2fdisset" ) 
	#define CAL_syssock2fdisset  syssock2fdisset
	#define CHK_syssock2fdisset  TRUE
	#define EXP_syssock2fdisset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdisset", (RTS_UINTPTR)syssock2fdisset, 1, 0x838C8AA3, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2fdisset
	#define EXT_SysSocket2externalsyssock2fdisset
	#define GET_SysSocket2externalsyssock2fdisset  ERR_OK
	#define CAL_SysSocket2externalsyssock2fdisset  syssock2fdisset
	#define CHK_SysSocket2externalsyssock2fdisset  TRUE
	#define EXP_SysSocket2externalsyssock2fdisset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdisset", (RTS_UINTPTR)syssock2fdisset, 1, 0x838C8AA3, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2fdisset
	#define EXT_syssock2fdisset
	#define GET_syssock2fdisset(fl)  CAL_CMGETAPI( "syssock2fdisset" ) 
	#define CAL_syssock2fdisset  syssock2fdisset
	#define CHK_syssock2fdisset  TRUE
	#define EXP_syssock2fdisset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdisset", (RTS_UINTPTR)syssock2fdisset, 1, 0x838C8AA3, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2fdisset  PFSYSSOCK2FDISSET_IEC pfsyssock2fdisset;
	#define EXT_syssock2fdisset  extern PFSYSSOCK2FDISSET_IEC pfsyssock2fdisset;
	#define GET_syssock2fdisset(fl)  s_pfCMGetAPI2( "syssock2fdisset", (RTS_VOID_FCTPTR *)&pfsyssock2fdisset, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x838C8AA3, 0x03050E00)
	#define CAL_syssock2fdisset  pfsyssock2fdisset
	#define CHK_syssock2fdisset  (pfsyssock2fdisset != NULL)
	#define EXP_syssock2fdisset   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2fdisset", (RTS_UINTPTR)syssock2fdisset, 1, 0x838C8AA3, 0x03050E00) 
#endif


/**
 * | Set options of a specified socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2getoption_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diLevel;				/* VAR_INPUT */	/* Level of the socket */
	RTS_IEC_DINT diOption;				/* VAR_INPUT */	/* Socket option command */
	RTS_IEC_BYTE *pdiOptionValue;		/* VAR_INPUT */	/* Pointer to the option value */
	RTS_IEC_DINT *pdiOptionLen;			/* VAR_INPUT */	/* Lenght of option value */
	RTS_IEC_RESULT SysSock2GetOption;	/* VAR_OUTPUT */	
} syssock2getoption_struct;

void CDECL CDECL_EXT syssock2getoption(syssock2getoption_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2GETOPTION_IEC) (syssock2getoption_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2GETOPTION_NOTIMPLEMENTED)
	#define USE_syssock2getoption
	#define EXT_syssock2getoption
	#define GET_syssock2getoption(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2getoption(p0) 
	#define CHK_syssock2getoption  FALSE
	#define EXP_syssock2getoption  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2getoption
	#define EXT_syssock2getoption
	#define GET_syssock2getoption(fl)  CAL_CMGETAPI( "syssock2getoption" ) 
	#define CAL_syssock2getoption  syssock2getoption
	#define CHK_syssock2getoption  TRUE
	#define EXP_syssock2getoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getoption", (RTS_UINTPTR)syssock2getoption, 1, 0xE48066F9, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2getoption
	#define EXT_syssock2getoption
	#define GET_syssock2getoption(fl)  CAL_CMGETAPI( "syssock2getoption" ) 
	#define CAL_syssock2getoption  syssock2getoption
	#define CHK_syssock2getoption  TRUE
	#define EXP_syssock2getoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getoption", (RTS_UINTPTR)syssock2getoption, 1, 0xE48066F9, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2getoption
	#define EXT_SysSocket2externalsyssock2getoption
	#define GET_SysSocket2externalsyssock2getoption  ERR_OK
	#define CAL_SysSocket2externalsyssock2getoption  syssock2getoption
	#define CHK_SysSocket2externalsyssock2getoption  TRUE
	#define EXP_SysSocket2externalsyssock2getoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getoption", (RTS_UINTPTR)syssock2getoption, 1, 0xE48066F9, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2getoption
	#define EXT_syssock2getoption
	#define GET_syssock2getoption(fl)  CAL_CMGETAPI( "syssock2getoption" ) 
	#define CAL_syssock2getoption  syssock2getoption
	#define CHK_syssock2getoption  TRUE
	#define EXP_syssock2getoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getoption", (RTS_UINTPTR)syssock2getoption, 1, 0xE48066F9, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2getoption  PFSYSSOCK2GETOPTION_IEC pfsyssock2getoption;
	#define EXT_syssock2getoption  extern PFSYSSOCK2GETOPTION_IEC pfsyssock2getoption;
	#define GET_syssock2getoption(fl)  s_pfCMGetAPI2( "syssock2getoption", (RTS_VOID_FCTPTR *)&pfsyssock2getoption, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xE48066F9, 0x03050E00)
	#define CAL_syssock2getoption  pfsyssock2getoption
	#define CHK_syssock2getoption  (pfsyssock2getoption != NULL)
	#define EXP_syssock2getoption   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getoption", (RTS_UINTPTR)syssock2getoption, 1, 0xE48066F9, 0x03050E00) 
#endif


/**
 * | Returns the socket address of the peer to which a socket is connected.
 * | The SysSockGetPeerName function can be used only on a connected socket. 
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2getpeername_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address of the peer */
	RTS_IEC_DINT *pdiSockAddrSize;		/* VAR_INPUT */	/* Pointer to size of socket address structure */
	RTS_IEC_RESULT SysSock2GetPeerName;	/* VAR_OUTPUT */	
} syssock2getpeername_struct;

void CDECL CDECL_EXT syssock2getpeername(syssock2getpeername_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2GETPEERNAME_IEC) (syssock2getpeername_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2GETPEERNAME_NOTIMPLEMENTED)
	#define USE_syssock2getpeername
	#define EXT_syssock2getpeername
	#define GET_syssock2getpeername(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2getpeername(p0) 
	#define CHK_syssock2getpeername  FALSE
	#define EXP_syssock2getpeername  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2getpeername
	#define EXT_syssock2getpeername
	#define GET_syssock2getpeername(fl)  CAL_CMGETAPI( "syssock2getpeername" ) 
	#define CAL_syssock2getpeername  syssock2getpeername
	#define CHK_syssock2getpeername  TRUE
	#define EXP_syssock2getpeername  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getpeername", (RTS_UINTPTR)syssock2getpeername, 1, 0x4D1F4B79, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2getpeername
	#define EXT_syssock2getpeername
	#define GET_syssock2getpeername(fl)  CAL_CMGETAPI( "syssock2getpeername" ) 
	#define CAL_syssock2getpeername  syssock2getpeername
	#define CHK_syssock2getpeername  TRUE
	#define EXP_syssock2getpeername  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getpeername", (RTS_UINTPTR)syssock2getpeername, 1, 0x4D1F4B79, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2getpeername
	#define EXT_SysSocket2externalsyssock2getpeername
	#define GET_SysSocket2externalsyssock2getpeername  ERR_OK
	#define CAL_SysSocket2externalsyssock2getpeername  syssock2getpeername
	#define CHK_SysSocket2externalsyssock2getpeername  TRUE
	#define EXP_SysSocket2externalsyssock2getpeername  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getpeername", (RTS_UINTPTR)syssock2getpeername, 1, 0x4D1F4B79, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2getpeername
	#define EXT_syssock2getpeername
	#define GET_syssock2getpeername(fl)  CAL_CMGETAPI( "syssock2getpeername" ) 
	#define CAL_syssock2getpeername  syssock2getpeername
	#define CHK_syssock2getpeername  TRUE
	#define EXP_syssock2getpeername  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getpeername", (RTS_UINTPTR)syssock2getpeername, 1, 0x4D1F4B79, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2getpeername  PFSYSSOCK2GETPEERNAME_IEC pfsyssock2getpeername;
	#define EXT_syssock2getpeername  extern PFSYSSOCK2GETPEERNAME_IEC pfsyssock2getpeername;
	#define GET_syssock2getpeername(fl)  s_pfCMGetAPI2( "syssock2getpeername", (RTS_VOID_FCTPTR *)&pfsyssock2getpeername, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x4D1F4B79, 0x03050E00)
	#define CAL_syssock2getpeername  pfsyssock2getpeername
	#define CHK_syssock2getpeername  (pfsyssock2getpeername != NULL)
	#define EXP_syssock2getpeername   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getpeername", (RTS_UINTPTR)syssock2getpeername, 1, 0x4D1F4B79, 0x03050E00) 
#endif


/**
 * | Returns the socket address of the local socket.
 * | Usally this function is called to retrieve the local socket address on multihomed hosts or to get 
 *   the local port number in client implemntations. If the socket is neither connected nor locally bound to an address,
 *   the result of the function and the value stored in the object pointed to by pSockAddress is unspecified. 
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2getsockname_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address of the local socket */
	RTS_IEC_DINT *pdiSockAddrSize;		/* VAR_INPUT */	/* Pointer to size of socket address structure */
	RTS_IEC_RESULT SysSock2GetSockName;	/* VAR_OUTPUT */	
} syssock2getsockname_struct;

void CDECL CDECL_EXT syssock2getsockname(syssock2getsockname_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2GETSOCKNAME_IEC) (syssock2getsockname_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2GETSOCKNAME_NOTIMPLEMENTED)
	#define USE_syssock2getsockname
	#define EXT_syssock2getsockname
	#define GET_syssock2getsockname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2getsockname(p0) 
	#define CHK_syssock2getsockname  FALSE
	#define EXP_syssock2getsockname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2getsockname
	#define EXT_syssock2getsockname
	#define GET_syssock2getsockname(fl)  CAL_CMGETAPI( "syssock2getsockname" ) 
	#define CAL_syssock2getsockname  syssock2getsockname
	#define CHK_syssock2getsockname  TRUE
	#define EXP_syssock2getsockname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getsockname", (RTS_UINTPTR)syssock2getsockname, 1, 0xCD3D4AC3, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2getsockname
	#define EXT_syssock2getsockname
	#define GET_syssock2getsockname(fl)  CAL_CMGETAPI( "syssock2getsockname" ) 
	#define CAL_syssock2getsockname  syssock2getsockname
	#define CHK_syssock2getsockname  TRUE
	#define EXP_syssock2getsockname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getsockname", (RTS_UINTPTR)syssock2getsockname, 1, 0xCD3D4AC3, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2getsockname
	#define EXT_SysSocket2externalsyssock2getsockname
	#define GET_SysSocket2externalsyssock2getsockname  ERR_OK
	#define CAL_SysSocket2externalsyssock2getsockname  syssock2getsockname
	#define CHK_SysSocket2externalsyssock2getsockname  TRUE
	#define EXP_SysSocket2externalsyssock2getsockname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getsockname", (RTS_UINTPTR)syssock2getsockname, 1, 0xCD3D4AC3, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2getsockname
	#define EXT_syssock2getsockname
	#define GET_syssock2getsockname(fl)  CAL_CMGETAPI( "syssock2getsockname" ) 
	#define CAL_syssock2getsockname  syssock2getsockname
	#define CHK_syssock2getsockname  TRUE
	#define EXP_syssock2getsockname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getsockname", (RTS_UINTPTR)syssock2getsockname, 1, 0xCD3D4AC3, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2getsockname  PFSYSSOCK2GETSOCKNAME_IEC pfsyssock2getsockname;
	#define EXT_syssock2getsockname  extern PFSYSSOCK2GETSOCKNAME_IEC pfsyssock2getsockname;
	#define GET_syssock2getsockname(fl)  s_pfCMGetAPI2( "syssock2getsockname", (RTS_VOID_FCTPTR *)&pfsyssock2getsockname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xCD3D4AC3, 0x03050E00)
	#define CAL_syssock2getsockname  pfsyssock2getsockname
	#define CHK_syssock2getsockname  (pfsyssock2getsockname != NULL)
	#define EXP_syssock2getsockname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2getsockname", (RTS_UINTPTR)syssock2getsockname, 1, 0xCD3D4AC3, 0x03050E00) 
#endif


/**
 * | Io-control of a socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2ioctl_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diCommand;				/* VAR_INPUT */	/* Io-control command */
	RTS_IEC_DINT *pdiParameter;			/* VAR_INPUT */	/* Parameter value of the command */
	RTS_IEC_RESULT SysSock2Ioctl;		/* VAR_OUTPUT */	
} syssock2ioctl_struct;

void CDECL CDECL_EXT syssock2ioctl(syssock2ioctl_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2IOCTL_IEC) (syssock2ioctl_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2IOCTL_NOTIMPLEMENTED)
	#define USE_syssock2ioctl
	#define EXT_syssock2ioctl
	#define GET_syssock2ioctl(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2ioctl(p0) 
	#define CHK_syssock2ioctl  FALSE
	#define EXP_syssock2ioctl  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2ioctl
	#define EXT_syssock2ioctl
	#define GET_syssock2ioctl(fl)  CAL_CMGETAPI( "syssock2ioctl" ) 
	#define CAL_syssock2ioctl  syssock2ioctl
	#define CHK_syssock2ioctl  TRUE
	#define EXP_syssock2ioctl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2ioctl", (RTS_UINTPTR)syssock2ioctl, 1, 0xCF1E8764, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2ioctl
	#define EXT_syssock2ioctl
	#define GET_syssock2ioctl(fl)  CAL_CMGETAPI( "syssock2ioctl" ) 
	#define CAL_syssock2ioctl  syssock2ioctl
	#define CHK_syssock2ioctl  TRUE
	#define EXP_syssock2ioctl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2ioctl", (RTS_UINTPTR)syssock2ioctl, 1, 0xCF1E8764, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2ioctl
	#define EXT_SysSocket2externalsyssock2ioctl
	#define GET_SysSocket2externalsyssock2ioctl  ERR_OK
	#define CAL_SysSocket2externalsyssock2ioctl  syssock2ioctl
	#define CHK_SysSocket2externalsyssock2ioctl  TRUE
	#define EXP_SysSocket2externalsyssock2ioctl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2ioctl", (RTS_UINTPTR)syssock2ioctl, 1, 0xCF1E8764, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2ioctl
	#define EXT_syssock2ioctl
	#define GET_syssock2ioctl(fl)  CAL_CMGETAPI( "syssock2ioctl" ) 
	#define CAL_syssock2ioctl  syssock2ioctl
	#define CHK_syssock2ioctl  TRUE
	#define EXP_syssock2ioctl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2ioctl", (RTS_UINTPTR)syssock2ioctl, 1, 0xCF1E8764, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2ioctl  PFSYSSOCK2IOCTL_IEC pfsyssock2ioctl;
	#define EXT_syssock2ioctl  extern PFSYSSOCK2IOCTL_IEC pfsyssock2ioctl;
	#define GET_syssock2ioctl(fl)  s_pfCMGetAPI2( "syssock2ioctl", (RTS_VOID_FCTPTR *)&pfsyssock2ioctl, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xCF1E8764, 0x03050E00)
	#define CAL_syssock2ioctl  pfsyssock2ioctl
	#define CHK_syssock2ioctl  (pfsyssock2ioctl != NULL)
	#define EXP_syssock2ioctl   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2ioctl", (RTS_UINTPTR)syssock2ioctl, 1, 0xCF1E8764, 0x03050E00) 
#endif


/**
 * | Listen on a TCP server socket for new connection.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2listen_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diMaxConnections;		/* VAR_INPUT */	/* Maximum number of connections allowed */
	RTS_IEC_RESULT SysSock2Listen;		/* VAR_OUTPUT */	
} syssock2listen_struct;

void CDECL CDECL_EXT syssock2listen(syssock2listen_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2LISTEN_IEC) (syssock2listen_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2LISTEN_NOTIMPLEMENTED)
	#define USE_syssock2listen
	#define EXT_syssock2listen
	#define GET_syssock2listen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2listen(p0) 
	#define CHK_syssock2listen  FALSE
	#define EXP_syssock2listen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2listen
	#define EXT_syssock2listen
	#define GET_syssock2listen(fl)  CAL_CMGETAPI( "syssock2listen" ) 
	#define CAL_syssock2listen  syssock2listen
	#define CHK_syssock2listen  TRUE
	#define EXP_syssock2listen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2listen", (RTS_UINTPTR)syssock2listen, 1, 0xC76C27A4, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2listen
	#define EXT_syssock2listen
	#define GET_syssock2listen(fl)  CAL_CMGETAPI( "syssock2listen" ) 
	#define CAL_syssock2listen  syssock2listen
	#define CHK_syssock2listen  TRUE
	#define EXP_syssock2listen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2listen", (RTS_UINTPTR)syssock2listen, 1, 0xC76C27A4, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2listen
	#define EXT_SysSocket2externalsyssock2listen
	#define GET_SysSocket2externalsyssock2listen  ERR_OK
	#define CAL_SysSocket2externalsyssock2listen  syssock2listen
	#define CHK_SysSocket2externalsyssock2listen  TRUE
	#define EXP_SysSocket2externalsyssock2listen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2listen", (RTS_UINTPTR)syssock2listen, 1, 0xC76C27A4, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2listen
	#define EXT_syssock2listen
	#define GET_syssock2listen(fl)  CAL_CMGETAPI( "syssock2listen" ) 
	#define CAL_syssock2listen  syssock2listen
	#define CHK_syssock2listen  TRUE
	#define EXP_syssock2listen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2listen", (RTS_UINTPTR)syssock2listen, 1, 0xC76C27A4, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2listen  PFSYSSOCK2LISTEN_IEC pfsyssock2listen;
	#define EXT_syssock2listen  extern PFSYSSOCK2LISTEN_IEC pfsyssock2listen;
	#define GET_syssock2listen(fl)  s_pfCMGetAPI2( "syssock2listen", (RTS_VOID_FCTPTR *)&pfsyssock2listen, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xC76C27A4, 0x03050E00)
	#define CAL_syssock2listen  pfsyssock2listen
	#define CHK_syssock2listen  (pfsyssock2listen != NULL)
	#define EXP_syssock2listen   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2listen", (RTS_UINTPTR)syssock2listen, 1, 0xC76C27A4, 0x03050E00) 
#endif


/**
 * | Receive data from a TCP socket.
 * :return: Number of bytes received. 0 if failed.
 */
typedef struct tagsyssock2recv_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Buffer to read data from the socket */
	RTS_IEC_XINT diBufferSize;			/* VAR_INPUT */	/* Maximum length of the buffer */
	RTS_IEC_DINT diFlags;				/* VAR_INPUT */	/* | The flags parameter can be used to influence the behavior of the 
	  function beyond the options specified for the associated socket. The semantics of this function 
	  are determined by the socket options and the flags parameter. The latter is constructed by using the
	  bitwise OR operator with any of the SOCKET_MSG values. See category TCP flags. */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSock2Recv;			/* VAR_OUTPUT */	
} syssock2recv_struct;

void CDECL CDECL_EXT syssock2recv(syssock2recv_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2RECV_IEC) (syssock2recv_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2RECV_NOTIMPLEMENTED)
	#define USE_syssock2recv
	#define EXT_syssock2recv
	#define GET_syssock2recv(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2recv(p0) 
	#define CHK_syssock2recv  FALSE
	#define EXP_syssock2recv  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2recv
	#define EXT_syssock2recv
	#define GET_syssock2recv(fl)  CAL_CMGETAPI( "syssock2recv" ) 
	#define CAL_syssock2recv  syssock2recv
	#define CHK_syssock2recv  TRUE
	#define EXP_syssock2recv  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recv", (RTS_UINTPTR)syssock2recv, 1, RTSITF_GET_SIGNATURE(0xB660C00E, 0xC68B1F2F), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2recv
	#define EXT_syssock2recv
	#define GET_syssock2recv(fl)  CAL_CMGETAPI( "syssock2recv" ) 
	#define CAL_syssock2recv  syssock2recv
	#define CHK_syssock2recv  TRUE
	#define EXP_syssock2recv  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recv", (RTS_UINTPTR)syssock2recv, 1, RTSITF_GET_SIGNATURE(0xB660C00E, 0xC68B1F2F), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2recv
	#define EXT_SysSocket2externalsyssock2recv
	#define GET_SysSocket2externalsyssock2recv  ERR_OK
	#define CAL_SysSocket2externalsyssock2recv  syssock2recv
	#define CHK_SysSocket2externalsyssock2recv  TRUE
	#define EXP_SysSocket2externalsyssock2recv  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recv", (RTS_UINTPTR)syssock2recv, 1, RTSITF_GET_SIGNATURE(0xB660C00E, 0xC68B1F2F), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2recv
	#define EXT_syssock2recv
	#define GET_syssock2recv(fl)  CAL_CMGETAPI( "syssock2recv" ) 
	#define CAL_syssock2recv  syssock2recv
	#define CHK_syssock2recv  TRUE
	#define EXP_syssock2recv  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recv", (RTS_UINTPTR)syssock2recv, 1, RTSITF_GET_SIGNATURE(0xB660C00E, 0xC68B1F2F), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2recv  PFSYSSOCK2RECV_IEC pfsyssock2recv;
	#define EXT_syssock2recv  extern PFSYSSOCK2RECV_IEC pfsyssock2recv;
	#define GET_syssock2recv(fl)  s_pfCMGetAPI2( "syssock2recv", (RTS_VOID_FCTPTR *)&pfsyssock2recv, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xB660C00E, 0xC68B1F2F), 0x03050E00)
	#define CAL_syssock2recv  pfsyssock2recv
	#define CHK_syssock2recv  (pfsyssock2recv != NULL)
	#define EXP_syssock2recv   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recv", (RTS_UINTPTR)syssock2recv, 1, RTSITF_GET_SIGNATURE(0xB660C00E, 0xC68B1F2F), 0x03050E00) 
#endif


/**
 * | Receive a message from a connectionless socket (UDP).
 * :return: Number of bytes received
 */
typedef struct tagsyssock2recvfrom_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Buffer to read data from the socket */
	RTS_IEC_XINT diBufferSize;			/* VAR_INPUT */	/* Maximum length of the buffer */
	RTS_IEC_DINT diFlags;				/* VAR_INPUT */	/* | The flags parameter can be used to influence the behavior of the 
	  function beyond the options specified for the associated socket. The semantics of this function 
	  are determined by the socket options and the flags parameter. The latter is constructed by using the
	  bitwise OR operator with any of the SOCKET_MSG values. */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address and port to receive data from */
	RTS_IEC_DINT diSockAddrSize;		/* VAR_INPUT */	/* Size of socket address structure */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSock2RecvFrom;		/* VAR_OUTPUT */	
} syssock2recvfrom_struct;

void CDECL CDECL_EXT syssock2recvfrom(syssock2recvfrom_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2RECVFROM_IEC) (syssock2recvfrom_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2RECVFROM_NOTIMPLEMENTED)
	#define USE_syssock2recvfrom
	#define EXT_syssock2recvfrom
	#define GET_syssock2recvfrom(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2recvfrom(p0) 
	#define CHK_syssock2recvfrom  FALSE
	#define EXP_syssock2recvfrom  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2recvfrom
	#define EXT_syssock2recvfrom
	#define GET_syssock2recvfrom(fl)  CAL_CMGETAPI( "syssock2recvfrom" ) 
	#define CAL_syssock2recvfrom  syssock2recvfrom
	#define CHK_syssock2recvfrom  TRUE
	#define EXP_syssock2recvfrom  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recvfrom", (RTS_UINTPTR)syssock2recvfrom, 1, RTSITF_GET_SIGNATURE(0x1109A6B6, 0xE3F1F6D3), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2recvfrom
	#define EXT_syssock2recvfrom
	#define GET_syssock2recvfrom(fl)  CAL_CMGETAPI( "syssock2recvfrom" ) 
	#define CAL_syssock2recvfrom  syssock2recvfrom
	#define CHK_syssock2recvfrom  TRUE
	#define EXP_syssock2recvfrom  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recvfrom", (RTS_UINTPTR)syssock2recvfrom, 1, RTSITF_GET_SIGNATURE(0x1109A6B6, 0xE3F1F6D3), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2recvfrom
	#define EXT_SysSocket2externalsyssock2recvfrom
	#define GET_SysSocket2externalsyssock2recvfrom  ERR_OK
	#define CAL_SysSocket2externalsyssock2recvfrom  syssock2recvfrom
	#define CHK_SysSocket2externalsyssock2recvfrom  TRUE
	#define EXP_SysSocket2externalsyssock2recvfrom  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recvfrom", (RTS_UINTPTR)syssock2recvfrom, 1, RTSITF_GET_SIGNATURE(0x1109A6B6, 0xE3F1F6D3), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2recvfrom
	#define EXT_syssock2recvfrom
	#define GET_syssock2recvfrom(fl)  CAL_CMGETAPI( "syssock2recvfrom" ) 
	#define CAL_syssock2recvfrom  syssock2recvfrom
	#define CHK_syssock2recvfrom  TRUE
	#define EXP_syssock2recvfrom  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recvfrom", (RTS_UINTPTR)syssock2recvfrom, 1, RTSITF_GET_SIGNATURE(0x1109A6B6, 0xE3F1F6D3), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2recvfrom  PFSYSSOCK2RECVFROM_IEC pfsyssock2recvfrom;
	#define EXT_syssock2recvfrom  extern PFSYSSOCK2RECVFROM_IEC pfsyssock2recvfrom;
	#define GET_syssock2recvfrom(fl)  s_pfCMGetAPI2( "syssock2recvfrom", (RTS_VOID_FCTPTR *)&pfsyssock2recvfrom, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x1109A6B6, 0xE3F1F6D3), 0x03050E00)
	#define CAL_syssock2recvfrom  pfsyssock2recvfrom
	#define CHK_syssock2recvfrom  (pfsyssock2recvfrom != NULL)
	#define EXP_syssock2recvfrom   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2recvfrom", (RTS_UINTPTR)syssock2recvfrom, 1, RTSITF_GET_SIGNATURE(0x1109A6B6, 0xE3F1F6D3), 0x03050E00) 
#endif


/**
 * | Check a number of sockets for activity. All sockets added to the FD_SETs have to be of the same type (Standard or TLS)
 * :return: Runtime system error code (see CmpErrors.library):
 *		| ERR_OK
 *		| ERR_SOCK_TIMEDOUT, if timeout expired
 */
typedef struct tagsyssock2select_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to one of the socket handles added to the FD Sets. */
	RTS_IEC_DINT diWidth;				/* VAR_INPUT */	/* Number of sockets in the |SOCKET_FD_SET| structure, so |SOCKET_FD_SETSIZE| must be used here. */
	SOCKET_FD_SET *pfdRead;				/* VAR_INPUT */	/* Optional pointer to the structure |SOCKET_FD_SET| defining the socket set
 to be checked for reading. It is also possible to set this parameter to 0. */
	SOCKET_FD_SET *pfdWrite;			/* VAR_INPUT */	/* Optional pointer to the structure |SOCKET_FD_SET| defining the socket set
 to be checked for writing. It is also possible to set this parameter to 0. */
	SOCKET_FD_SET *pfdExcept;			/* VAR_INPUT */	/* Optional pointer to structure |SOCKET_FD_SET| that defines the socket set
 the error state has to be checked. It is also possible to set this parameter to 0. */
	SOCKET_TIMEVAL *ptvTimeout;			/* VAR_INPUT */	/* Pointer to maximum timespan which the function SysSockSelect waits for a response:
 	| ptvTimeout=NULL:	Infinite wait
 	| ptvTimeout->tv_sec=-1, ptvTimeout->tv_usec=-1:	Infinite wait
 	| ptvTimeout->tv_sec=0, ptvTimeout->tv_usec=0:	No wait */
	RTS_IEC_DINT *pdiReady;				/* VAR_INPUT */	/* Number of sockets that are ready for IO */
	RTS_IEC_RESULT SysSock2Select;		/* VAR_OUTPUT */	
} syssock2select_struct;

void CDECL CDECL_EXT syssock2select(syssock2select_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2SELECT_IEC) (syssock2select_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2SELECT_NOTIMPLEMENTED)
	#define USE_syssock2select
	#define EXT_syssock2select
	#define GET_syssock2select(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2select(p0) 
	#define CHK_syssock2select  FALSE
	#define EXP_syssock2select  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2select
	#define EXT_syssock2select
	#define GET_syssock2select(fl)  CAL_CMGETAPI( "syssock2select" ) 
	#define CAL_syssock2select  syssock2select
	#define CHK_syssock2select  TRUE
	#define EXP_syssock2select  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2select", (RTS_UINTPTR)syssock2select, 1, 0x027528F5, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2select
	#define EXT_syssock2select
	#define GET_syssock2select(fl)  CAL_CMGETAPI( "syssock2select" ) 
	#define CAL_syssock2select  syssock2select
	#define CHK_syssock2select  TRUE
	#define EXP_syssock2select  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2select", (RTS_UINTPTR)syssock2select, 1, 0x027528F5, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2select
	#define EXT_SysSocket2externalsyssock2select
	#define GET_SysSocket2externalsyssock2select  ERR_OK
	#define CAL_SysSocket2externalsyssock2select  syssock2select
	#define CHK_SysSocket2externalsyssock2select  TRUE
	#define EXP_SysSocket2externalsyssock2select  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2select", (RTS_UINTPTR)syssock2select, 1, 0x027528F5, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2select
	#define EXT_syssock2select
	#define GET_syssock2select(fl)  CAL_CMGETAPI( "syssock2select" ) 
	#define CAL_syssock2select  syssock2select
	#define CHK_syssock2select  TRUE
	#define EXP_syssock2select  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2select", (RTS_UINTPTR)syssock2select, 1, 0x027528F5, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2select  PFSYSSOCK2SELECT_IEC pfsyssock2select;
	#define EXT_syssock2select  extern PFSYSSOCK2SELECT_IEC pfsyssock2select;
	#define GET_syssock2select(fl)  s_pfCMGetAPI2( "syssock2select", (RTS_VOID_FCTPTR *)&pfsyssock2select, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x027528F5, 0x03050E00)
	#define CAL_syssock2select  pfsyssock2select
	#define CHK_syssock2select  (pfsyssock2select != NULL)
	#define EXP_syssock2select   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2select", (RTS_UINTPTR)syssock2select, 1, 0x027528F5, 0x03050E00) 
#endif


/**
 * | Sent data to a TCP socket.
 * :return: Number of sent bytes. 0 if failed.
 */
typedef struct tagsyssock2send_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Buffer with data to sent */
	RTS_IEC_XINT diBufferSize;			/* VAR_INPUT */	/* Maximum length of the buffer */
	RTS_IEC_DINT diFlags;				/* VAR_INPUT */	/* | The flags parameter can be used to influence the behavior of the 
	  function beyond the options specified for the associated socket. The semantics of this function 
	  are determined by the socket options and the flags parameter. The latter is constructed by using the
	  bitwise OR operator with any of the SOCKET_MSG values. */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSock2Send;			/* VAR_OUTPUT */	
} syssock2send_struct;

void CDECL CDECL_EXT syssock2send(syssock2send_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2SEND_IEC) (syssock2send_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2SEND_NOTIMPLEMENTED)
	#define USE_syssock2send
	#define EXT_syssock2send
	#define GET_syssock2send(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2send(p0) 
	#define CHK_syssock2send  FALSE
	#define EXP_syssock2send  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2send
	#define EXT_syssock2send
	#define GET_syssock2send(fl)  CAL_CMGETAPI( "syssock2send" ) 
	#define CAL_syssock2send  syssock2send
	#define CHK_syssock2send  TRUE
	#define EXP_syssock2send  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2send", (RTS_UINTPTR)syssock2send, 1, RTSITF_GET_SIGNATURE(0x92166851, 0xE2FDB770), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2send
	#define EXT_syssock2send
	#define GET_syssock2send(fl)  CAL_CMGETAPI( "syssock2send" ) 
	#define CAL_syssock2send  syssock2send
	#define CHK_syssock2send  TRUE
	#define EXP_syssock2send  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2send", (RTS_UINTPTR)syssock2send, 1, RTSITF_GET_SIGNATURE(0x92166851, 0xE2FDB770), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2send
	#define EXT_SysSocket2externalsyssock2send
	#define GET_SysSocket2externalsyssock2send  ERR_OK
	#define CAL_SysSocket2externalsyssock2send  syssock2send
	#define CHK_SysSocket2externalsyssock2send  TRUE
	#define EXP_SysSocket2externalsyssock2send  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2send", (RTS_UINTPTR)syssock2send, 1, RTSITF_GET_SIGNATURE(0x92166851, 0xE2FDB770), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2send
	#define EXT_syssock2send
	#define GET_syssock2send(fl)  CAL_CMGETAPI( "syssock2send" ) 
	#define CAL_syssock2send  syssock2send
	#define CHK_syssock2send  TRUE
	#define EXP_syssock2send  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2send", (RTS_UINTPTR)syssock2send, 1, RTSITF_GET_SIGNATURE(0x92166851, 0xE2FDB770), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2send  PFSYSSOCK2SEND_IEC pfsyssock2send;
	#define EXT_syssock2send  extern PFSYSSOCK2SEND_IEC pfsyssock2send;
	#define GET_syssock2send(fl)  s_pfCMGetAPI2( "syssock2send", (RTS_VOID_FCTPTR *)&pfsyssock2send, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x92166851, 0xE2FDB770), 0x03050E00)
	#define CAL_syssock2send  pfsyssock2send
	#define CHK_syssock2send  (pfsyssock2send != NULL)
	#define EXP_syssock2send   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2send", (RTS_UINTPTR)syssock2send, 1, RTSITF_GET_SIGNATURE(0x92166851, 0xE2FDB770), 0x03050E00) 
#endif


/**
 * | Send a message over a connectionless socket (UDP).
 * :return: Number of bytes received.
 */
typedef struct tagsyssock2sendto_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Buffer with send data */
	RTS_IEC_XINT diBufferSize;			/* VAR_INPUT */	/* Length of data to send. If diBufferSize = 0 ERR_PARAMETER is returned */
	RTS_IEC_DINT diFlags;				/* VAR_INPUT */	/* | The flags parameter can be used to influence the behavior of the 
	  function beyond the options specified for the associated socket. The semantics of this function 
	  are determined by the socket options and the flags parameter. The latter is constructed by using the
	  bitwise OR operator with any of the SOCKET_MSG values. */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address and port to sent data to */
	RTS_IEC_DINT diSockAddrSize;		/* VAR_INPUT */	/* Size of socket address structure */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSock2SendTo;		/* VAR_OUTPUT */	
} syssock2sendto_struct;

void CDECL CDECL_EXT syssock2sendto(syssock2sendto_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2SENDTO_IEC) (syssock2sendto_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2SENDTO_NOTIMPLEMENTED)
	#define USE_syssock2sendto
	#define EXT_syssock2sendto
	#define GET_syssock2sendto(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2sendto(p0) 
	#define CHK_syssock2sendto  FALSE
	#define EXP_syssock2sendto  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2sendto
	#define EXT_syssock2sendto
	#define GET_syssock2sendto(fl)  CAL_CMGETAPI( "syssock2sendto" ) 
	#define CAL_syssock2sendto  syssock2sendto
	#define CHK_syssock2sendto  TRUE
	#define EXP_syssock2sendto  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2sendto", (RTS_UINTPTR)syssock2sendto, 1, RTSITF_GET_SIGNATURE(0xBC837B5E, 0x5311AA5A), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2sendto
	#define EXT_syssock2sendto
	#define GET_syssock2sendto(fl)  CAL_CMGETAPI( "syssock2sendto" ) 
	#define CAL_syssock2sendto  syssock2sendto
	#define CHK_syssock2sendto  TRUE
	#define EXP_syssock2sendto  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2sendto", (RTS_UINTPTR)syssock2sendto, 1, RTSITF_GET_SIGNATURE(0xBC837B5E, 0x5311AA5A), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2sendto
	#define EXT_SysSocket2externalsyssock2sendto
	#define GET_SysSocket2externalsyssock2sendto  ERR_OK
	#define CAL_SysSocket2externalsyssock2sendto  syssock2sendto
	#define CHK_SysSocket2externalsyssock2sendto  TRUE
	#define EXP_SysSocket2externalsyssock2sendto  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2sendto", (RTS_UINTPTR)syssock2sendto, 1, RTSITF_GET_SIGNATURE(0xBC837B5E, 0x5311AA5A), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2sendto
	#define EXT_syssock2sendto
	#define GET_syssock2sendto(fl)  CAL_CMGETAPI( "syssock2sendto" ) 
	#define CAL_syssock2sendto  syssock2sendto
	#define CHK_syssock2sendto  TRUE
	#define EXP_syssock2sendto  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2sendto", (RTS_UINTPTR)syssock2sendto, 1, RTSITF_GET_SIGNATURE(0xBC837B5E, 0x5311AA5A), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2sendto  PFSYSSOCK2SENDTO_IEC pfsyssock2sendto;
	#define EXT_syssock2sendto  extern PFSYSSOCK2SENDTO_IEC pfsyssock2sendto;
	#define GET_syssock2sendto(fl)  s_pfCMGetAPI2( "syssock2sendto", (RTS_VOID_FCTPTR *)&pfsyssock2sendto, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xBC837B5E, 0x5311AA5A), 0x03050E00)
	#define CAL_syssock2sendto  pfsyssock2sendto
	#define CHK_syssock2sendto  (pfsyssock2sendto != NULL)
	#define EXP_syssock2sendto   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2sendto", (RTS_UINTPTR)syssock2sendto, 1, RTSITF_GET_SIGNATURE(0xBC837B5E, 0x5311AA5A), 0x03050E00) 
#endif


/**
 * | Set options of a specified socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2setoption_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diLevel;				/* VAR_INPUT */	/* Level of the socket */
	RTS_IEC_DINT diOption;				/* VAR_INPUT */	/* Socket option command */
	RTS_IEC_BYTE *pdiOptionValue;		/* VAR_INPUT */	/* Pointer to the option value */
	RTS_IEC_DINT diOptionLen;			/* VAR_INPUT */	/* Lenght of option value */
	RTS_IEC_RESULT SysSock2SetOption;	/* VAR_OUTPUT */	
} syssock2setoption_struct;

void CDECL CDECL_EXT syssock2setoption(syssock2setoption_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2SETOPTION_IEC) (syssock2setoption_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2SETOPTION_NOTIMPLEMENTED)
	#define USE_syssock2setoption
	#define EXT_syssock2setoption
	#define GET_syssock2setoption(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2setoption(p0) 
	#define CHK_syssock2setoption  FALSE
	#define EXP_syssock2setoption  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2setoption
	#define EXT_syssock2setoption
	#define GET_syssock2setoption(fl)  CAL_CMGETAPI( "syssock2setoption" ) 
	#define CAL_syssock2setoption  syssock2setoption
	#define CHK_syssock2setoption  TRUE
	#define EXP_syssock2setoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2setoption", (RTS_UINTPTR)syssock2setoption, 1, 0x442C7F14, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2setoption
	#define EXT_syssock2setoption
	#define GET_syssock2setoption(fl)  CAL_CMGETAPI( "syssock2setoption" ) 
	#define CAL_syssock2setoption  syssock2setoption
	#define CHK_syssock2setoption  TRUE
	#define EXP_syssock2setoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2setoption", (RTS_UINTPTR)syssock2setoption, 1, 0x442C7F14, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2setoption
	#define EXT_SysSocket2externalsyssock2setoption
	#define GET_SysSocket2externalsyssock2setoption  ERR_OK
	#define CAL_SysSocket2externalsyssock2setoption  syssock2setoption
	#define CHK_SysSocket2externalsyssock2setoption  TRUE
	#define EXP_SysSocket2externalsyssock2setoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2setoption", (RTS_UINTPTR)syssock2setoption, 1, 0x442C7F14, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2setoption
	#define EXT_syssock2setoption
	#define GET_syssock2setoption(fl)  CAL_CMGETAPI( "syssock2setoption" ) 
	#define CAL_syssock2setoption  syssock2setoption
	#define CHK_syssock2setoption  TRUE
	#define EXP_syssock2setoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2setoption", (RTS_UINTPTR)syssock2setoption, 1, 0x442C7F14, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2setoption  PFSYSSOCK2SETOPTION_IEC pfsyssock2setoption;
	#define EXT_syssock2setoption  extern PFSYSSOCK2SETOPTION_IEC pfsyssock2setoption;
	#define GET_syssock2setoption(fl)  s_pfCMGetAPI2( "syssock2setoption", (RTS_VOID_FCTPTR *)&pfsyssock2setoption, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x442C7F14, 0x03050E00)
	#define CAL_syssock2setoption  pfsyssock2setoption
	#define CHK_syssock2setoption  (pfsyssock2setoption != NULL)
	#define EXP_syssock2setoption   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2setoption", (RTS_UINTPTR)syssock2setoption, 1, 0x442C7F14, 0x03050E00) 
#endif


/**
 * | Shutdown a socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssock2shutdown_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diHow;					/* VAR_INPUT */	/* Specifies, which operations are no longer be allowed. See shutdown flags. */
	RTS_IEC_RESULT SysSock2Shutdown;	/* VAR_OUTPUT */	
} syssock2shutdown_struct;

void CDECL CDECL_EXT syssock2shutdown(syssock2shutdown_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCK2SHUTDOWN_IEC) (syssock2shutdown_struct *p);
#if defined(SYSSOCKET2EXTERNAL_NOTIMPLEMENTED) || defined(SYSSOCK2SHUTDOWN_NOTIMPLEMENTED)
	#define USE_syssock2shutdown
	#define EXT_syssock2shutdown
	#define GET_syssock2shutdown(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssock2shutdown(p0) 
	#define CHK_syssock2shutdown  FALSE
	#define EXP_syssock2shutdown  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssock2shutdown
	#define EXT_syssock2shutdown
	#define GET_syssock2shutdown(fl)  CAL_CMGETAPI( "syssock2shutdown" ) 
	#define CAL_syssock2shutdown  syssock2shutdown
	#define CHK_syssock2shutdown  TRUE
	#define EXP_syssock2shutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2shutdown", (RTS_UINTPTR)syssock2shutdown, 1, 0x775B849E, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET2EXTERNAL_EXTERNAL)
	#define USE_syssock2shutdown
	#define EXT_syssock2shutdown
	#define GET_syssock2shutdown(fl)  CAL_CMGETAPI( "syssock2shutdown" ) 
	#define CAL_syssock2shutdown  syssock2shutdown
	#define CHK_syssock2shutdown  TRUE
	#define EXP_syssock2shutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2shutdown", (RTS_UINTPTR)syssock2shutdown, 1, 0x775B849E, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocket2externalsyssock2shutdown
	#define EXT_SysSocket2externalsyssock2shutdown
	#define GET_SysSocket2externalsyssock2shutdown  ERR_OK
	#define CAL_SysSocket2externalsyssock2shutdown  syssock2shutdown
	#define CHK_SysSocket2externalsyssock2shutdown  TRUE
	#define EXP_SysSocket2externalsyssock2shutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2shutdown", (RTS_UINTPTR)syssock2shutdown, 1, 0x775B849E, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssock2shutdown
	#define EXT_syssock2shutdown
	#define GET_syssock2shutdown(fl)  CAL_CMGETAPI( "syssock2shutdown" ) 
	#define CAL_syssock2shutdown  syssock2shutdown
	#define CHK_syssock2shutdown  TRUE
	#define EXP_syssock2shutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2shutdown", (RTS_UINTPTR)syssock2shutdown, 1, 0x775B849E, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssock2shutdown  PFSYSSOCK2SHUTDOWN_IEC pfsyssock2shutdown;
	#define EXT_syssock2shutdown  extern PFSYSSOCK2SHUTDOWN_IEC pfsyssock2shutdown;
	#define GET_syssock2shutdown(fl)  s_pfCMGetAPI2( "syssock2shutdown", (RTS_VOID_FCTPTR *)&pfsyssock2shutdown, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x775B849E, 0x03050E00)
	#define CAL_syssock2shutdown  pfsyssock2shutdown
	#define CHK_syssock2shutdown  (pfsyssock2shutdown != NULL)
	#define EXP_syssock2shutdown   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssock2shutdown", (RTS_UINTPTR)syssock2shutdown, 1, 0x775B849E, 0x03050E00) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/




typedef struct
{
	IBase_C *pBase;
} ISysSocket2external_C;

#ifdef CPLUSPLUS
class ISysSocket2external : public IBase
{
	public:
};
	#ifndef ITF_SysSocket2external
		#define ITF_SysSocket2external static ISysSocket2external *pISysSocket2external = NULL;
	#endif
	#define EXTITF_SysSocket2external
#else	/*CPLUSPLUS*/
	typedef ISysSocket2external_C		ISysSocket2external;
	#ifndef ITF_SysSocket2external
		#define ITF_SysSocket2external
	#endif
	#define EXTITF_SysSocket2external
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSSOCKET2EXTERNALITF_H_*/
