 /**
 * <interfacename>CmpSessionInformation</interfacename>
 *
 * <description> 
 *	 
 *	<p>Interface of the session information management. In order to store and handle information about the connected clients,
 *	the session information is collected during the login process. It is used as part of the client identification management
 *	to identify all connected clients with corresponding information.</p>	 
 *	
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */
 
 /* Change the default char type from signed char to unsigned char */
 #define _CHAR_UNSIGNED


	
	
#ifndef _CMPSESSIONINFORMATIONITF_H_
#define _CMPSESSIONINFORMATIONITF_H_

#include "CmpStd.h"

 

 




#include "CmpItf.h"

/**
 * <category>String Memory Sizes</category>
 * <description>Number of bytes of stings including the \0 terminator.</description>
 * <element name="SESS_INFO_MEMSIZE_USER_NAME">Memory size of the string for the user logged in on the session.</element>
 * <element name="SESS_INFO_MEMSIZE_CLIENT_NAME">Memory size of the string for the name of the client application that created that session.</element>
 * <element name="SESS_INFO_MEMSIZE_CLIENT_VENDOR_NAME">Memory size of the string for the name of the client application's vendor.</element>
 * <element name="SESS_INFO_MEMSIZE_CLIENT_HOST_NAME">Memory size of the string for the host name where client is running.</element>
 * <element name="SESS_INFO_MEMSIZE_CLIENT_VERSION">Memory size of the string for the client version.</element>
 * <element name="SESS_INFO_MEMSIZE_CLIENT_CDS_VERSION">Memory size of the string for the CODESYS version the client is based on.</element>
 */
#define SESS_INFO_MEMSIZE_USER_NAME					UINT16_C(64)
#define SESS_INFO_MEMSIZE_CLIENT_NAME				UINT16_C(256)
#define SESS_INFO_MEMSIZE_CLIENT_VENDOR_NAME		UINT16_C(256)
#define SESS_INFO_MEMSIZE_CLIENT_HOST_NAME			UINT16_C(256)
#define SESS_INFO_MEMSIZE_CLIENT_VERSION			UINT16_C(256)
#define SESS_INFO_MEMSIZE_CLIENT_CDS_VERSION		UINT16_C(256)

/**
 * <category>Client types</category>
 * <description>These are different types of clients that can be connected to a Runtime system.</description>
 * <element name="SESS_INFO_CLIENT_TYPE_NO_INFO">This is an old fashioned client that does not provide any useful information.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PROG">This is a HighWord that represents a general client that can program the PLC.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PROGRAMMING_SYSTEM">This client is used to program the PLC.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PROG_SYSTEM_VIA_CAS">Programming System connected via CODESYS Automation Server.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PROG_SYSTEM_GO">This client is used to program the PLC via CODESYS go!.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PROG_SYSTEM_GO_VIA_CAS">Programming System CODESYS go! connected via CODESYS Automation Server.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_OPCUA_CLIENT">This is OPC UA client connected to the PLC.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_WEBVISU">Web visu - CODESYS HMI for web browser.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_REMOTE_TARGET_VISU">This is remote target visu connected to the PLC.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_DATA_SOURCES">Datasources using the ChannelClient for PLC communication.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_DATA_SERVER">Legacy Dataserver or other client based on PlcServices.library.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PLCHANDLER">This is a unspecific PLCHandler based client, most likely a customer product.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PLCH_OPC">CODESY OPC Server V3 based on the PLCHandler.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PLCH_EDGE">CODESYS Edge Gateway based on PLCHandler.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PLCH_DATA_SOURCES">Datasources using PLCHandler for PLC communication.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_PLCH_IECVARACC_BROWSE">CODESYS IecVarAccessClient based on PLCHandler.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_LEGACY_PROGRAMMING_SYSTEM">Probably CODESYS before version V3.5.16.0.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_LEGACY_WEBVISU">Probably WebVisualization before version V3.5.16.0.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_LEGACY_REMOTE_TARGET_VISU">Probably Remote TargetVisu before version V3.5.16.0.</element>
 * <element name="SESS_INFO_CLIENT_TYPE_LEGACY_PLCHANDLER">Probably PLCHandler or similar before version V3.5.16.0.</element>
 */
#define SESS_INFO_CLIENT_TYPE_NO_INFO						0x00000000
#define SESS_INFO_CLIENT_TYPE_PROG							0xC0DE0000
#define SESS_INFO_CLIENT_TYPE_PROGRAMMING_SYSTEM			(SESS_INFO_CLIENT_TYPE_PROG + 0x5150)
#define SESS_INFO_CLIENT_TYPE_PROG_SYSTEM_VIA_CAS			(SESS_INFO_CLIENT_TYPE_PROG + 0x0CA5)
#define SESS_INFO_CLIENT_TYPE_PROG_SYSTEM_GO				(SESS_INFO_CLIENT_TYPE_PROG + 0x6000)
#define SESS_INFO_CLIENT_TYPE_PROG_SYSTEM_GO_VIA_CAS		(SESS_INFO_CLIENT_TYPE_PROG + 0x60CA) 
#define SESS_INFO_CLIENT_TYPE_OPC_UA_CLIENT					0x09C02000
#define SESS_INFO_CLIENT_TYPE_WEBVISU						0xABCDABCD
#define SESS_INFO_CLIENT_TYPE_REMOTE_TARGET_VISU			0x08150815
#define SESS_INFO_CLIENT_TYPE_DATA_SOURCES					0xD520D520
#define SESS_INFO_CLIENT_TYPE_DATA_SERVER					0xD510D510
#define SESS_INFO_CLIENT_TYPE_PLCHANDLER					0x59500000
#define SESS_INFO_CLIENT_TYPE_PLCH_OPC						(SESS_INFO_CLIENT_TYPE_PLCHANDLER + 0x09C0)
#define SESS_INFO_CLIENT_TYPE_PLCH_EDGE						(SESS_INFO_CLIENT_TYPE_PLCHANDLER + 0xED6E)
#define SESS_INFO_CLIENT_TYPE_PLCH_DATA_SOURCES				(SESS_INFO_CLIENT_TYPE_PLCHANDLER + 0xD520)
#define SESS_INFO_CLIENT_TYPE_PLCH_IECVARACC_BROWSE			(SESS_INFO_CLIENT_TYPE_PLCHANDLER + 0x1ECB)
#define SESS_INFO_CLIENT_TYPE_LEGACY_PROGRAMMING_SYSTEM		0x01D0C0DE
#define SESS_INFO_CLIENT_TYPE_LEGACY_WEBVISU				0x01D0ABCD
#define SESS_INFO_CLIENT_TYPE_LEGACY_REMOTE_TARGET_VISU		0x01D00815
#define SESS_INFO_CLIENT_TYPE_LEGACY_PLCHANDLER				0x01D05950

/**
 * <category>Session information structure</category>
 * <description>This structure holds the information that is stored for each client. See description of SessionInfoAddSession for element descriptions.</description>
 */
typedef struct SessionInfo_
{
	RTS_UI32		ui32SessionId;
	RTS_UI32		ui32ChannelId;
	RTS_UI32		ui32ClientType;
	RTS_UTF8STRING	szUserName[SESS_INFO_MEMSIZE_USER_NAME];
	RTS_UTF8STRING	szClientName[SESS_INFO_MEMSIZE_CLIENT_NAME];
	RTS_UTF8STRING	szClientVendorName[SESS_INFO_MEMSIZE_CLIENT_VENDOR_NAME];
	RTS_UTF8STRING	szClientHostName[SESS_INFO_MEMSIZE_CLIENT_HOST_NAME];
	RTS_UTF8STRING	szClientVersion[SESS_INFO_MEMSIZE_CLIENT_VERSION];
	RTS_UTF8STRING	szClientCdsVersion[SESS_INFO_MEMSIZE_CLIENT_CDS_VERSION];
} SessionInfo;

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 * Add a session information record to the session info pool.
 * All String contents are copied into the own memory of the CmpSessionInformation module and therefore do not have to be retained by the caller afterwards.  
 * </description>
 * <param name="ui32ClientType" type="IN">The type of the client that created this session. See Client types.</param>
 * <param name="ui32SessionId" type="IN">SessionId used for the CODESYS online communication, not used for OPC UA clients.</param>
 * <param name="ui32ChannelId" type="IN">ChannelId used for the CODESYS online communication, not used for OPC UA clients.</param>
 * <param name="pszUserName" type="IN">UTF-8 encoded user name known by the user management. Max. mem. size: SESS_INFO_MEMSIZE_USER_NAME.</param>
 * <param name="pszClientName" type="IN">UTF-8 encoded name of the client application that created that session. Max. mem. size: SESS_INFO_MEMSIZE_CLIENT_NAME.</param>
 * <param name="pszClientVendorName" type="IN">UTF-8 encoded name of the client application's vendor. Max. mem. size: SESS_INFO_MEMSIZE_CLIENT_VENDOR_NAME.</param>
 * <param name="pszClientHostName" type="IN">UTF-8 encoded host name where client is running. Max. mem. size: SESS_INFO_MEMSIZE_CLIENT_HOST_NAME.</param>
 * <param name="pszClientVersion" type="IN">UTF-8 encoded client version. Max. mem. size: SESS_INFO_MEMSIZE_CLIENT_VERSION.</param>
 * <param name="pszClientCdsVersion" type="IN">UTF-8 encoded CODESYS version the client is based on. Max. mem. size: SESS_INFO_MEMSIZE_CLIENT_CDS_VERSION.</param>
 * <param name="prResult" type="OUT">Result of the operation.</param>
 * <result>Handle to the session information.</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Session info successfully stored.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_MEMORY">No more memory to store the session info.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Something else went wrong.</errorcode>
 */
RTS_HANDLE CDECL SessionInfoAddSession(RTS_UI32 ui32ClientType, RTS_UI32 ui32SessionId, RTS_UI32 ui32ChannelId, RTS_UTF8STRING const* pszUserName, RTS_UTF8STRING const* pszClientName, RTS_UTF8STRING const* pszClientVendorName, RTS_UTF8STRING const* pszClientHostName, RTS_UTF8STRING const* pszClientVersion, RTS_UTF8STRING const* pszClientCdsVersion, RTS_RESULT * prResult);
typedef RTS_HANDLE (CDECL * PFSESSIONINFOADDSESSION) (RTS_UI32 ui32ClientType, RTS_UI32 ui32SessionId, RTS_UI32 ui32ChannelId, RTS_UTF8STRING const* pszUserName, RTS_UTF8STRING const* pszClientName, RTS_UTF8STRING const* pszClientVendorName, RTS_UTF8STRING const* pszClientHostName, RTS_UTF8STRING const* pszClientVersion, RTS_UTF8STRING const* pszClientCdsVersion, RTS_RESULT * prResult);
#if defined(CMPSESSIONINFORMATION_NOTIMPLEMENTED) || defined(SESSIONINFOADDSESSION_NOTIMPLEMENTED)
	#define USE_SessionInfoAddSession
	#define EXT_SessionInfoAddSession
	#define GET_SessionInfoAddSession(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SessionInfoAddSession(p0,p1,p2,p3,p4,p5,p6,p7,p8,p9)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SessionInfoAddSession  FALSE
	#define EXP_SessionInfoAddSession  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SessionInfoAddSession
	#define EXT_SessionInfoAddSession
	#define GET_SessionInfoAddSession(fl)  CAL_CMGETAPI( "SessionInfoAddSession" ) 
	#define CAL_SessionInfoAddSession  SessionInfoAddSession
	#define CHK_SessionInfoAddSession  TRUE
	#define EXP_SessionInfoAddSession  CAL_CMEXPAPI( "SessionInfoAddSession" ) 
#elif defined(MIXED_LINK) && !defined(CMPSESSIONINFORMATION_EXTERNAL)
	#define USE_SessionInfoAddSession
	#define EXT_SessionInfoAddSession
	#define GET_SessionInfoAddSession(fl)  CAL_CMGETAPI( "SessionInfoAddSession" ) 
	#define CAL_SessionInfoAddSession  SessionInfoAddSession
	#define CHK_SessionInfoAddSession  TRUE
	#define EXP_SessionInfoAddSession  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SessionInfoAddSession", (RTS_UINTPTR)SessionInfoAddSession, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSessionInformationSessionInfoAddSession
	#define EXT_CmpSessionInformationSessionInfoAddSession
	#define GET_CmpSessionInformationSessionInfoAddSession  ERR_OK
	#define CAL_CmpSessionInformationSessionInfoAddSession pICmpSessionInformation->ISessionInfoAddSession
	#define CHK_CmpSessionInformationSessionInfoAddSession (pICmpSessionInformation != NULL)
	#define EXP_CmpSessionInformationSessionInfoAddSession  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SessionInfoAddSession
	#define EXT_SessionInfoAddSession
	#define GET_SessionInfoAddSession(fl)  CAL_CMGETAPI( "SessionInfoAddSession" ) 
	#define CAL_SessionInfoAddSession pICmpSessionInformation->ISessionInfoAddSession
	#define CHK_SessionInfoAddSession (pICmpSessionInformation != NULL)
	#define EXP_SessionInfoAddSession  CAL_CMEXPAPI( "SessionInfoAddSession" ) 
#else /* DYNAMIC_LINK */
	#define USE_SessionInfoAddSession  PFSESSIONINFOADDSESSION pfSessionInfoAddSession;
	#define EXT_SessionInfoAddSession  extern PFSESSIONINFOADDSESSION pfSessionInfoAddSession;
	#define GET_SessionInfoAddSession(fl)  s_pfCMGetAPI2( "SessionInfoAddSession", (RTS_VOID_FCTPTR *)&pfSessionInfoAddSession, (fl), 0, 0)
	#define CAL_SessionInfoAddSession  pfSessionInfoAddSession
	#define CHK_SessionInfoAddSession  (pfSessionInfoAddSession != NULL)
	#define EXP_SessionInfoAddSession  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SessionInfoAddSession", (RTS_UINTPTR)SessionInfoAddSession, 0, 0) 
#endif




/**
 * <description>Update the user handle of an existing session information record. This must be called after 
 * 	a user has upgraded the credentials to handle specific operations.</description>
 * <param name="hSessionInfo" type="IN">Handle to the session information.</param>
 * <param name="pszUserName" type="IN">UTF-8 encoded user name known by the user management. Max. mem. size: SESS_INFO_MEMSIZE_USER_NAME.</param>
 * <result>Error Code.</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Session info successfully updated.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hSessionInfo was invalid.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Something else went wrong.</errorcode>
 */
RTS_RESULT CDECL SessionInfoUpdateUser(RTS_HANDLE hSessionInfo, RTS_UTF8STRING const* pszUserName);
typedef RTS_RESULT (CDECL * PFSESSIONINFOUPDATEUSER) (RTS_HANDLE hSessionInfo, RTS_UTF8STRING const* pszUserName);
#if defined(CMPSESSIONINFORMATION_NOTIMPLEMENTED) || defined(SESSIONINFOUPDATEUSER_NOTIMPLEMENTED)
	#define USE_SessionInfoUpdateUser
	#define EXT_SessionInfoUpdateUser
	#define GET_SessionInfoUpdateUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SessionInfoUpdateUser(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SessionInfoUpdateUser  FALSE
	#define EXP_SessionInfoUpdateUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SessionInfoUpdateUser
	#define EXT_SessionInfoUpdateUser
	#define GET_SessionInfoUpdateUser(fl)  CAL_CMGETAPI( "SessionInfoUpdateUser" ) 
	#define CAL_SessionInfoUpdateUser  SessionInfoUpdateUser
	#define CHK_SessionInfoUpdateUser  TRUE
	#define EXP_SessionInfoUpdateUser  CAL_CMEXPAPI( "SessionInfoUpdateUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPSESSIONINFORMATION_EXTERNAL)
	#define USE_SessionInfoUpdateUser
	#define EXT_SessionInfoUpdateUser
	#define GET_SessionInfoUpdateUser(fl)  CAL_CMGETAPI( "SessionInfoUpdateUser" ) 
	#define CAL_SessionInfoUpdateUser  SessionInfoUpdateUser
	#define CHK_SessionInfoUpdateUser  TRUE
	#define EXP_SessionInfoUpdateUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SessionInfoUpdateUser", (RTS_UINTPTR)SessionInfoUpdateUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSessionInformationSessionInfoUpdateUser
	#define EXT_CmpSessionInformationSessionInfoUpdateUser
	#define GET_CmpSessionInformationSessionInfoUpdateUser  ERR_OK
	#define CAL_CmpSessionInformationSessionInfoUpdateUser pICmpSessionInformation->ISessionInfoUpdateUser
	#define CHK_CmpSessionInformationSessionInfoUpdateUser (pICmpSessionInformation != NULL)
	#define EXP_CmpSessionInformationSessionInfoUpdateUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SessionInfoUpdateUser
	#define EXT_SessionInfoUpdateUser
	#define GET_SessionInfoUpdateUser(fl)  CAL_CMGETAPI( "SessionInfoUpdateUser" ) 
	#define CAL_SessionInfoUpdateUser pICmpSessionInformation->ISessionInfoUpdateUser
	#define CHK_SessionInfoUpdateUser (pICmpSessionInformation != NULL)
	#define EXP_SessionInfoUpdateUser  CAL_CMEXPAPI( "SessionInfoUpdateUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_SessionInfoUpdateUser  PFSESSIONINFOUPDATEUSER pfSessionInfoUpdateUser;
	#define EXT_SessionInfoUpdateUser  extern PFSESSIONINFOUPDATEUSER pfSessionInfoUpdateUser;
	#define GET_SessionInfoUpdateUser(fl)  s_pfCMGetAPI2( "SessionInfoUpdateUser", (RTS_VOID_FCTPTR *)&pfSessionInfoUpdateUser, (fl), 0, 0)
	#define CAL_SessionInfoUpdateUser  pfSessionInfoUpdateUser
	#define CHK_SessionInfoUpdateUser  (pfSessionInfoUpdateUser != NULL)
	#define EXP_SessionInfoUpdateUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SessionInfoUpdateUser", (RTS_UINTPTR)SessionInfoUpdateUser, 0, 0) 
#endif




/**
 * <description>Delete the session information. Used when a client closes the session.</description>
 * <param name="hSessionInfo" type="IN">Handle to the session information.</param>
 * <result>Error Code.</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Session info successfully removed.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hSessionInfo was invalid.</errorcode>
 */
RTS_RESULT CDECL SessionInfoRemoveSession(RTS_HANDLE hSessionInfo);
typedef RTS_RESULT (CDECL * PFSESSIONINFOREMOVESESSION) (RTS_HANDLE hSessionInfo);
#if defined(CMPSESSIONINFORMATION_NOTIMPLEMENTED) || defined(SESSIONINFOREMOVESESSION_NOTIMPLEMENTED)
	#define USE_SessionInfoRemoveSession
	#define EXT_SessionInfoRemoveSession
	#define GET_SessionInfoRemoveSession(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SessionInfoRemoveSession(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SessionInfoRemoveSession  FALSE
	#define EXP_SessionInfoRemoveSession  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SessionInfoRemoveSession
	#define EXT_SessionInfoRemoveSession
	#define GET_SessionInfoRemoveSession(fl)  CAL_CMGETAPI( "SessionInfoRemoveSession" ) 
	#define CAL_SessionInfoRemoveSession  SessionInfoRemoveSession
	#define CHK_SessionInfoRemoveSession  TRUE
	#define EXP_SessionInfoRemoveSession  CAL_CMEXPAPI( "SessionInfoRemoveSession" ) 
#elif defined(MIXED_LINK) && !defined(CMPSESSIONINFORMATION_EXTERNAL)
	#define USE_SessionInfoRemoveSession
	#define EXT_SessionInfoRemoveSession
	#define GET_SessionInfoRemoveSession(fl)  CAL_CMGETAPI( "SessionInfoRemoveSession" ) 
	#define CAL_SessionInfoRemoveSession  SessionInfoRemoveSession
	#define CHK_SessionInfoRemoveSession  TRUE
	#define EXP_SessionInfoRemoveSession  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SessionInfoRemoveSession", (RTS_UINTPTR)SessionInfoRemoveSession, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSessionInformationSessionInfoRemoveSession
	#define EXT_CmpSessionInformationSessionInfoRemoveSession
	#define GET_CmpSessionInformationSessionInfoRemoveSession  ERR_OK
	#define CAL_CmpSessionInformationSessionInfoRemoveSession pICmpSessionInformation->ISessionInfoRemoveSession
	#define CHK_CmpSessionInformationSessionInfoRemoveSession (pICmpSessionInformation != NULL)
	#define EXP_CmpSessionInformationSessionInfoRemoveSession  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SessionInfoRemoveSession
	#define EXT_SessionInfoRemoveSession
	#define GET_SessionInfoRemoveSession(fl)  CAL_CMGETAPI( "SessionInfoRemoveSession" ) 
	#define CAL_SessionInfoRemoveSession pICmpSessionInformation->ISessionInfoRemoveSession
	#define CHK_SessionInfoRemoveSession (pICmpSessionInformation != NULL)
	#define EXP_SessionInfoRemoveSession  CAL_CMEXPAPI( "SessionInfoRemoveSession" ) 
#else /* DYNAMIC_LINK */
	#define USE_SessionInfoRemoveSession  PFSESSIONINFOREMOVESESSION pfSessionInfoRemoveSession;
	#define EXT_SessionInfoRemoveSession  extern PFSESSIONINFOREMOVESESSION pfSessionInfoRemoveSession;
	#define GET_SessionInfoRemoveSession(fl)  s_pfCMGetAPI2( "SessionInfoRemoveSession", (RTS_VOID_FCTPTR *)&pfSessionInfoRemoveSession, (fl), 0, 0)
	#define CAL_SessionInfoRemoveSession  pfSessionInfoRemoveSession
	#define CHK_SessionInfoRemoveSession  (pfSessionInfoRemoveSession != NULL)
	#define EXP_SessionInfoRemoveSession  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SessionInfoRemoveSession", (RTS_UINTPTR)SessionInfoRemoveSession, 0, 0) 
#endif




/**
 * <description>
 * <description>Retrieves the session information for a given session Id.</description>
 * <param name="ui32SessionId" type="IN">Session Id for which the session information is requested.</param>
 * <param name="pResult" type="OUT">Pointer to error code.</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Session information retrieved successfully.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_OBJECT">No session information found for the given session Id.</errorcode>
 * <result>Pointer to the session information.</result>
 */
SessionInfo* CDECL SessionInfoGetById(RTS_UI32 ui32SessionId, RTS_RESULT *pResult);
typedef SessionInfo* (CDECL * PFSESSIONINFOGETBYID) (RTS_UI32 ui32SessionId, RTS_RESULT *pResult);
#if defined(CMPSESSIONINFORMATION_NOTIMPLEMENTED) || defined(SESSIONINFOGETBYID_NOTIMPLEMENTED)
	#define USE_SessionInfoGetById
	#define EXT_SessionInfoGetById
	#define GET_SessionInfoGetById(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SessionInfoGetById(p0,p1)  (SessionInfo*)ERR_NOTIMPLEMENTED
	#define CHK_SessionInfoGetById  FALSE
	#define EXP_SessionInfoGetById  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SessionInfoGetById
	#define EXT_SessionInfoGetById
	#define GET_SessionInfoGetById(fl)  CAL_CMGETAPI( "SessionInfoGetById" ) 
	#define CAL_SessionInfoGetById  SessionInfoGetById
	#define CHK_SessionInfoGetById  TRUE
	#define EXP_SessionInfoGetById  CAL_CMEXPAPI( "SessionInfoGetById" ) 
#elif defined(MIXED_LINK) && !defined(CMPSESSIONINFORMATION_EXTERNAL)
	#define USE_SessionInfoGetById
	#define EXT_SessionInfoGetById
	#define GET_SessionInfoGetById(fl)  CAL_CMGETAPI( "SessionInfoGetById" ) 
	#define CAL_SessionInfoGetById  SessionInfoGetById
	#define CHK_SessionInfoGetById  TRUE
	#define EXP_SessionInfoGetById  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SessionInfoGetById", (RTS_UINTPTR)SessionInfoGetById, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSessionInformationSessionInfoGetById
	#define EXT_CmpSessionInformationSessionInfoGetById
	#define GET_CmpSessionInformationSessionInfoGetById  ERR_OK
	#define CAL_CmpSessionInformationSessionInfoGetById pICmpSessionInformation->ISessionInfoGetById
	#define CHK_CmpSessionInformationSessionInfoGetById (pICmpSessionInformation != NULL)
	#define EXP_CmpSessionInformationSessionInfoGetById  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SessionInfoGetById
	#define EXT_SessionInfoGetById
	#define GET_SessionInfoGetById(fl)  CAL_CMGETAPI( "SessionInfoGetById" ) 
	#define CAL_SessionInfoGetById pICmpSessionInformation->ISessionInfoGetById
	#define CHK_SessionInfoGetById (pICmpSessionInformation != NULL)
	#define EXP_SessionInfoGetById  CAL_CMEXPAPI( "SessionInfoGetById" ) 
#else /* DYNAMIC_LINK */
	#define USE_SessionInfoGetById  PFSESSIONINFOGETBYID pfSessionInfoGetById;
	#define EXT_SessionInfoGetById  extern PFSESSIONINFOGETBYID pfSessionInfoGetById;
	#define GET_SessionInfoGetById(fl)  s_pfCMGetAPI2( "SessionInfoGetById", (RTS_VOID_FCTPTR *)&pfSessionInfoGetById, (fl), 0, 0)
	#define CAL_SessionInfoGetById  pfSessionInfoGetById
	#define CHK_SessionInfoGetById  (pfSessionInfoGetById != NULL)
	#define EXP_SessionInfoGetById  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SessionInfoGetById", (RTS_UINTPTR)SessionInfoGetById, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSESSIONINFOADDSESSION ISessionInfoAddSession;
 	PFSESSIONINFOUPDATEUSER ISessionInfoUpdateUser;
 	PFSESSIONINFOREMOVESESSION ISessionInfoRemoveSession;
 	PFSESSIONINFOGETBYID ISessionInfoGetById;
 } ICmpSessionInformation_C;

#ifdef CPLUSPLUS
class ICmpSessionInformation : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISessionInfoAddSession(RTS_UI32 ui32ClientType, RTS_UI32 ui32SessionId, RTS_UI32 ui32ChannelId, RTS_UTF8STRING const* pszUserName, RTS_UTF8STRING const* pszClientName, RTS_UTF8STRING const* pszClientVendorName, RTS_UTF8STRING const* pszClientHostName, RTS_UTF8STRING const* pszClientVersion, RTS_UTF8STRING const* pszClientCdsVersion, RTS_RESULT * prResult) =0;
		virtual RTS_RESULT CDECL ISessionInfoUpdateUser(RTS_HANDLE hSessionInfo, RTS_UTF8STRING const* pszUserName) =0;
		virtual RTS_RESULT CDECL ISessionInfoRemoveSession(RTS_HANDLE hSessionInfo) =0;
		virtual SessionInfo* CDECL ISessionInfoGetById(RTS_UI32 ui32SessionId, RTS_RESULT *pResult) =0;
};
	#ifndef ITF_CmpSessionInformation
		#define ITF_CmpSessionInformation static ICmpSessionInformation *pICmpSessionInformation = NULL;
	#endif
	#define EXTITF_CmpSessionInformation
#else	/*CPLUSPLUS*/
	typedef ICmpSessionInformation_C		ICmpSessionInformation;
	#ifndef ITF_CmpSessionInformation
		#define ITF_CmpSessionInformation
	#endif
	#define EXTITF_CmpSessionInformation
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPSESSIONINFORMATIONITF_H_*/
