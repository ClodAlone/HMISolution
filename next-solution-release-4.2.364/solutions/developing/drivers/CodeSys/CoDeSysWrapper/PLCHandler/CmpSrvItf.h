 /**
 * <interfacename>CmpSrv</interfacename>
 * <description> 
 *	Interface of the level 7 server.
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPSRVITF_H_
#define _CMPSRVITF_H_

#include "CmpStd.h"

 

 




#include "CmpBinTagUtilItf.h"
#include "CmpCommunicationLibItf.h"

/**
 * <description> 
 *	Defines the default CoDeSys layer 7 protocol. 
 * </description> 
 */
#define HEADERTAG_3S		0xCD55

/**
 * <description> 
 *	Defines the default CoDeSys encrypted layer 7 protocol. 
 * </description> 
 */
#define HEADERTAG_CRYPT		0x7557

/**
 * <description> 
 *	Defines the default safety communication layer 7 protocol.
 * </description> 
 */
#define HEADERTAG_SAFETY	0x5AF4

/**
 * <category>Service Groups</category>
 * <description> 
 *	Service groups for the layer 7 communication
 * </description> 
 */
#define SG_REPLY_PREFIX			0x0080

#define SG_INVALID				0x0000
#define SG_DEVICE				0x0001
#define SG_APPLICATION			0x0002
#define SG_MONITORING			0x0003
#define SG_VISUALISATION		0x0004
#define SG_LOG					0x0005
#define SG_SETTINGS				0x0006
#define SG_ETHERCAT				0x0007
#define SG_FILE_TRANSFER		0x0008
#define SG_IEC_VAR_ACCESS		0x0009
#define SG_COMPONENT_MANAGER	0x000A
#define SG_CMPIOMGR				0x000B
#define SG_CMPUSERMGR			0x000C
#define SG_ONLINEPARAMS			0x000D
#define SG_SOFTVISION			0x000E
#define SG_TRACE_MANAGER		0x000F
#define SG_FDT					0x0010
#define SG_PLCSHELL				0x0011
#define SG_APPLICATION_BP		0x0012
#define SG_APPLICATION_FORCE	0x0013
#define SG_REDUNDANCY			0x0014
#define	SG_VISU_AUTOTEST		0x0015
#define SG_IOLINK				0x0016
#define SG_ETCSRV				0x0017
#define SG_ALARM_MANAGER		0x0018
#define SG_PROFINET				0x0019
#define SG_SIL2					0x001A
#define SG_MONITORING2			0x001B
#define	SG_SUB_NODE_COMM		0x001C	/* e. g. used for communication to a SIL3 RTS via fieldbus */
#define	SG_CMPCODEMETER			0x001D	/* used for license transfer */
#define	SG_TREND_STORAGE		0x001E	/* access to the trend storage */
#define SG_COREDUMP				0x001F
#define SG_BACKUP				0x0020
#define SG_RUNTIME_TEST			0x0021  /* Used for test and verification of the runtime */
#define SG_X509_CERT            0x0022
#define SG_SECURITY_MANAGER		0x0023
#define SG_MAX_DEFINED			0x0023	/* NOTE: Must be adapted, if new service group was added! */
#define SG_MAX_RESERVED			0x00FF

/**
 * <description> 
 *	Defines some special values for session ids
 * </description> 
 */
#define SRV_SESSION_ID_EMPTY				0
#define SRV_SESSION_ID_INVALID				0x00000001
#define SRV_SESSION_ID_INITAL_REQUEST		0x00000011
#define SRV_SESSION_ID_REMOTE_VISU_CLIENT	0x00000815
#define SRV_SESSION_ID_WEBSERVER			0x0000ABCD
#define SRV_SESSION_ID_VALIDATION_BIT		0x80000000

/**
 * <category>Static defines</category>
 * <description>Number of static groups</description>
 */
 #ifndef SRV_NUM_OF_STATIC_GROUPS
	#define SRV_NUM_OF_STATIC_GROUPS		SG_MAX_DEFINED
#endif

/**
 * <category>Static defines</category>
 * <description>Max number of services that can be defined for synchronous execution</description>
 */
#ifndef SRV_NUM_OF_SYNC_SERVICES
	#define SRV_NUM_OF_SYNC_SERVICES		15
#endif

/**
 * <category>Service reply tags</category>
 * <description> 
 *	Global service reply tags
 * </description> 
 */
#define TAG_ERROR						0xFF7F
#define TAG_EXTERROR_INFO				0xFF7E
#define TAG_USER_NOTIFY					0xFF7D
#define TAG_SERVICE_RESULT				0x01


/**
 * <category>Event parameter</category>
 * <element name="ulChannelId" type="IN">Id of the channel on which the request arrived</element>
 * <element name="pHeaderTag" type="IN">Pointer to the header struct of the received request</element>
 * <element name="pduData" type="IN">References the request data (without header)</element>
 * <element name="pduSendBuffer" type="OUT">Contains the buffer (pointer and length) for the reply data</element>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Received service was completely handled.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_L7_UNKNOWNCMD">Service is unknown by the event handler.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_CALL_AGAIN">Received service will be handled asynchronously. 
 * To progress this event will be posted again for the same received service data with bFirstCall=0.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_...">Another error occured, caller will send this result in a generic error tag as reply (or close the channel).</errorcode>
 * <element name="Result" type="INOUT">IN: ERR_PENDING: Signals a repeated call for the same request. All other values represent the first call. OUT: See above.</element>
 */
typedef struct
{
	RTS_UI32 ulChannelId;
	HEADER_TAG *pHeaderTag;
	PROTOCOL_DATA_UNIT pduData;
	PROTOCOL_DATA_UNIT pduSendBuffer;
	RTS_RESULT Result;
} SERVICEHANDLER_PARAMETER;

/**
 * <category>Event parameter</category>
 * <element name="pServceHandlerParameter" type="IN">Pointer to service handler structure</element>
 */
typedef struct
{
	SERVICEHANDLER_PARAMETER *pServceHandlerParameter;
} EVTPARAM_CmpSrv;
#define EVTPARAMID_CmpSrv		0x0001
#define EVTVERSION_CmpSrv		0x0002

/* EVTVERSION_CmpSrv
0x0001: First version
0x0002: Supports now result code ERR_CALL_AGAIN for asynchronous service handlers. Event paramter structure is still the same.
*/


/**
 * <category>Events</category>
 * <description>
 *	NOTE: Every service group can be opened, as it is still provided by the server component! The corresponding
 *	event will created implicitly by the server component, if EventOpen() is called.
 *	Example:
 *		Client calls:				hEvent = CAL_EventOpen([Service group], CMPID_CmpSrv, NULL);
 *		CmpSrv created the event:	hEvent = CAL_EventCreate2([Service group], CMPID_CmpSrv, 1, NULL);	
 *
 *	ATTENTION: This feature is only available right after CH_INIT3 step!
 *
 * </description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpSrv</param>
 */

#define EVT_ExecuteOnlineService					MAKE_EVENTID(EVTCLASS_INFO, 1)

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 *	Servers register this callback function to process services of a service group. The reply to 
 *	such a request must be sent using FinishRequest.
 * </description>
 * <param name="ulChannelId" type="IN">Id of the channel on which the request arrived</param>
 * <param name="pHeaderTag" type="IN">Pointer to the header struct of the message</param>
 * <param name="pduData" type="IN">References the request data (without header)</param>
 * <param name="pduSendBuffer" type="IN">Contains the length of the buffer for the reply data, and the buffer for the replay data</param>
 * <result>
 *	Should return ERR_OK (succeeded immediatly) or ERR_PENDING (service is handled asynchronously).
 *	All other results abort the request.
 * </result>
 */
typedef RTS_RESULT (CDECL *PFServiceHandler)(RTS_UI32 ulChannelId, HEADER_TAG *pHeaderTag, PROTOCOL_DATA_UNIT pduData, PROTOCOL_DATA_UNIT pduSendBuffer);

/**
 * <description>
 * <p>Obsolete: Use PFServiceHandler (above) instead. Will be removed in future versions!</p>
 *	<p>Servers register this callback function to process services of a service group. The reply to 
 *	such a request must be sent using FinishRequest.</p>
 * </description>
 * <param name="hRouter" type="IN">Obsolete parameter, should be set to RTS_INVALID_HANDLE.</param>
 * <param name="ulChannelId" type="IN">Id of the channel on which the request arrived</param>
 * <param name="pHeaderTag" type="IN">Pointer to the header struct of the message</param>
 * <param name="pduData" type="IN">References the request data (without header)</param>
 * <param name="pduSendBuffer" type="IN">Contains the length of the buffer for the reply data, and the buffer for the replay data</param>
 * <result>
 *	Should return ERR_OK (succeeded immediatly) or ERR_PENDING (service is handled asynchronously).
 *	All other results abort the request.
 * </result>
 */
typedef RTS_RESULT (CDECL *PFServiceHandler2)(RTS_HANDLE hRouter, RTS_UI32 ulChannelId, HEADER_TAG *pHeaderTag, PROTOCOL_DATA_UNIT pduData, PROTOCOL_DATA_UNIT pduSendBuffer);


/**
 * <description>Obsolete: Use ServerAppHandleRequest3 instead. Will be removed in future versions!</description>
 */
RTS_RESULT CDECL ServerAppHandleRequest(RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply);
typedef RTS_RESULT (CDECL * PFSERVERAPPHANDLEREQUEST) (RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERAPPHANDLEREQUEST_NOTIMPLEMENTED)
	#define USE_ServerAppHandleRequest
	#define EXT_ServerAppHandleRequest
	#define GET_ServerAppHandleRequest(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerAppHandleRequest(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerAppHandleRequest  FALSE
	#define EXP_ServerAppHandleRequest  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerAppHandleRequest
	#define EXT_ServerAppHandleRequest
	#define GET_ServerAppHandleRequest(fl)  CAL_CMGETAPI( "ServerAppHandleRequest" ) 
	#define CAL_ServerAppHandleRequest  ServerAppHandleRequest
	#define CHK_ServerAppHandleRequest  TRUE
	#define EXP_ServerAppHandleRequest  CAL_CMEXPAPI( "ServerAppHandleRequest" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerAppHandleRequest
	#define EXT_ServerAppHandleRequest
	#define GET_ServerAppHandleRequest(fl)  CAL_CMGETAPI( "ServerAppHandleRequest" ) 
	#define CAL_ServerAppHandleRequest  ServerAppHandleRequest
	#define CHK_ServerAppHandleRequest  TRUE
	#define EXP_ServerAppHandleRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerAppHandleRequest", (RTS_UINTPTR)ServerAppHandleRequest, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerAppHandleRequest
	#define EXT_CmpSrvServerAppHandleRequest
	#define GET_CmpSrvServerAppHandleRequest  ERR_OK
	#define CAL_CmpSrvServerAppHandleRequest pICmpSrv->IServerAppHandleRequest
	#define CHK_CmpSrvServerAppHandleRequest (pICmpSrv != NULL)
	#define EXP_CmpSrvServerAppHandleRequest  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerAppHandleRequest
	#define EXT_ServerAppHandleRequest
	#define GET_ServerAppHandleRequest(fl)  CAL_CMGETAPI( "ServerAppHandleRequest" ) 
	#define CAL_ServerAppHandleRequest pICmpSrv->IServerAppHandleRequest
	#define CHK_ServerAppHandleRequest (pICmpSrv != NULL)
	#define EXP_ServerAppHandleRequest  CAL_CMEXPAPI( "ServerAppHandleRequest" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerAppHandleRequest  PFSERVERAPPHANDLEREQUEST pfServerAppHandleRequest;
	#define EXT_ServerAppHandleRequest  extern PFSERVERAPPHANDLEREQUEST pfServerAppHandleRequest;
	#define GET_ServerAppHandleRequest(fl)  s_pfCMGetAPI2( "ServerAppHandleRequest", (RTS_VOID_FCTPTR *)&pfServerAppHandleRequest, (fl), 0, 0)
	#define CAL_ServerAppHandleRequest  pfServerAppHandleRequest
	#define CHK_ServerAppHandleRequest  (pfServerAppHandleRequest != NULL)
	#define EXP_ServerAppHandleRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerAppHandleRequest", (RTS_UINTPTR)ServerAppHandleRequest, 0, 0) 
#endif




/**
 * <description>Obsolete: Use ServerAppHandleRequest3 instead. Will be removed in future versions!</description>
 */
RTS_RESULT CDECL ServerAppHandleRequest2(RTS_HANDLE hRouter, RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply);
typedef RTS_RESULT (CDECL * PFSERVERAPPHANDLEREQUEST2) (RTS_HANDLE hRouter, RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERAPPHANDLEREQUEST2_NOTIMPLEMENTED)
	#define USE_ServerAppHandleRequest2
	#define EXT_ServerAppHandleRequest2
	#define GET_ServerAppHandleRequest2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerAppHandleRequest2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerAppHandleRequest2  FALSE
	#define EXP_ServerAppHandleRequest2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerAppHandleRequest2
	#define EXT_ServerAppHandleRequest2
	#define GET_ServerAppHandleRequest2(fl)  CAL_CMGETAPI( "ServerAppHandleRequest2" ) 
	#define CAL_ServerAppHandleRequest2  ServerAppHandleRequest2
	#define CHK_ServerAppHandleRequest2  TRUE
	#define EXP_ServerAppHandleRequest2  CAL_CMEXPAPI( "ServerAppHandleRequest2" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerAppHandleRequest2
	#define EXT_ServerAppHandleRequest2
	#define GET_ServerAppHandleRequest2(fl)  CAL_CMGETAPI( "ServerAppHandleRequest2" ) 
	#define CAL_ServerAppHandleRequest2  ServerAppHandleRequest2
	#define CHK_ServerAppHandleRequest2  TRUE
	#define EXP_ServerAppHandleRequest2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerAppHandleRequest2", (RTS_UINTPTR)ServerAppHandleRequest2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerAppHandleRequest2
	#define EXT_CmpSrvServerAppHandleRequest2
	#define GET_CmpSrvServerAppHandleRequest2  ERR_OK
	#define CAL_CmpSrvServerAppHandleRequest2 pICmpSrv->IServerAppHandleRequest2
	#define CHK_CmpSrvServerAppHandleRequest2 (pICmpSrv != NULL)
	#define EXP_CmpSrvServerAppHandleRequest2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerAppHandleRequest2
	#define EXT_ServerAppHandleRequest2
	#define GET_ServerAppHandleRequest2(fl)  CAL_CMGETAPI( "ServerAppHandleRequest2" ) 
	#define CAL_ServerAppHandleRequest2 pICmpSrv->IServerAppHandleRequest2
	#define CHK_ServerAppHandleRequest2 (pICmpSrv != NULL)
	#define EXP_ServerAppHandleRequest2  CAL_CMEXPAPI( "ServerAppHandleRequest2" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerAppHandleRequest2  PFSERVERAPPHANDLEREQUEST2 pfServerAppHandleRequest2;
	#define EXT_ServerAppHandleRequest2  extern PFSERVERAPPHANDLEREQUEST2 pfServerAppHandleRequest2;
	#define GET_ServerAppHandleRequest2(fl)  s_pfCMGetAPI2( "ServerAppHandleRequest2", (RTS_VOID_FCTPTR *)&pfServerAppHandleRequest2, (fl), 0, 0)
	#define CAL_ServerAppHandleRequest2  pfServerAppHandleRequest2
	#define CHK_ServerAppHandleRequest2  (pfServerAppHandleRequest2 != NULL)
	#define EXP_ServerAppHandleRequest2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerAppHandleRequest2", (RTS_UINTPTR)ServerAppHandleRequest2, 0, 0) 
#endif




/**
 * <description>Handle one sevice request from the communication layer below (channel server)</description>
 * <param name="ulChannelId" type="IN">Id of the channel on which the request arrived</param>
 * <param name="pduRequest" type="IN">Pointer to the request</param>
 * <param name="pduReply" type="OUT">Pointer to the request reply buffer</param>
 * <param name="bFirstCall" type="IN">0: Tells the function, if it was already called for the same request before (0) or not (1).</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Received service was completely handeled.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PENDING">Received service will be handled asynchronously, 
 * but the caller has not to take care about this anymore.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_CALL_AGAIN">Received service will be handled asynchronously. 
 * To progress this function have to be called again for the same received service data with bFirstCall=0.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_...">Another error occured, channel should be closed.</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL ServerAppHandleRequest3(RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply, RTS_UI32 bFirstCall);
typedef RTS_RESULT (CDECL * PFSERVERAPPHANDLEREQUEST3) (RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply, RTS_UI32 bFirstCall);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERAPPHANDLEREQUEST3_NOTIMPLEMENTED)
	#define USE_ServerAppHandleRequest3
	#define EXT_ServerAppHandleRequest3
	#define GET_ServerAppHandleRequest3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerAppHandleRequest3(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerAppHandleRequest3  FALSE
	#define EXP_ServerAppHandleRequest3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerAppHandleRequest3
	#define EXT_ServerAppHandleRequest3
	#define GET_ServerAppHandleRequest3(fl)  CAL_CMGETAPI( "ServerAppHandleRequest3" ) 
	#define CAL_ServerAppHandleRequest3  ServerAppHandleRequest3
	#define CHK_ServerAppHandleRequest3  TRUE
	#define EXP_ServerAppHandleRequest3  CAL_CMEXPAPI( "ServerAppHandleRequest3" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerAppHandleRequest3
	#define EXT_ServerAppHandleRequest3
	#define GET_ServerAppHandleRequest3(fl)  CAL_CMGETAPI( "ServerAppHandleRequest3" ) 
	#define CAL_ServerAppHandleRequest3  ServerAppHandleRequest3
	#define CHK_ServerAppHandleRequest3  TRUE
	#define EXP_ServerAppHandleRequest3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerAppHandleRequest3", (RTS_UINTPTR)ServerAppHandleRequest3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerAppHandleRequest3
	#define EXT_CmpSrvServerAppHandleRequest3
	#define GET_CmpSrvServerAppHandleRequest3  ERR_OK
	#define CAL_CmpSrvServerAppHandleRequest3 pICmpSrv->IServerAppHandleRequest3
	#define CHK_CmpSrvServerAppHandleRequest3 (pICmpSrv != NULL)
	#define EXP_CmpSrvServerAppHandleRequest3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerAppHandleRequest3
	#define EXT_ServerAppHandleRequest3
	#define GET_ServerAppHandleRequest3(fl)  CAL_CMGETAPI( "ServerAppHandleRequest3" ) 
	#define CAL_ServerAppHandleRequest3 pICmpSrv->IServerAppHandleRequest3
	#define CHK_ServerAppHandleRequest3 (pICmpSrv != NULL)
	#define EXP_ServerAppHandleRequest3  CAL_CMEXPAPI( "ServerAppHandleRequest3" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerAppHandleRequest3  PFSERVERAPPHANDLEREQUEST3 pfServerAppHandleRequest3;
	#define EXT_ServerAppHandleRequest3  extern PFSERVERAPPHANDLEREQUEST3 pfServerAppHandleRequest3;
	#define GET_ServerAppHandleRequest3(fl)  s_pfCMGetAPI2( "ServerAppHandleRequest3", (RTS_VOID_FCTPTR *)&pfServerAppHandleRequest3, (fl), 0, 0)
	#define CAL_ServerAppHandleRequest3  pfServerAppHandleRequest3
	#define CHK_ServerAppHandleRequest3  (pfServerAppHandleRequest3 != NULL)
	#define EXP_ServerAppHandleRequest3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerAppHandleRequest3", (RTS_UINTPTR)ServerAppHandleRequest3, 0, 0) 
#endif




/**
 * <description>Register a handler for requests to a specific service group</description>
 * <param name="ulServiceGroup" type="IN">The service group which is handled by the provided function</param>
 * <param name="pfServiceHandler" type="IN">A handler function.</param>
 */
RTS_RESULT CDECL ServerRegisterServiceHandler(RTS_UI32 ulServiceGroup, PFServiceHandler pfServiceHandler);
typedef RTS_RESULT (CDECL * PFSERVERREGISTERSERVICEHANDLER) (RTS_UI32 ulServiceGroup, PFServiceHandler pfServiceHandler);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERREGISTERSERVICEHANDLER_NOTIMPLEMENTED)
	#define USE_ServerRegisterServiceHandler
	#define EXT_ServerRegisterServiceHandler
	#define GET_ServerRegisterServiceHandler(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerRegisterServiceHandler(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerRegisterServiceHandler  FALSE
	#define EXP_ServerRegisterServiceHandler  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerRegisterServiceHandler
	#define EXT_ServerRegisterServiceHandler
	#define GET_ServerRegisterServiceHandler(fl)  CAL_CMGETAPI( "ServerRegisterServiceHandler" ) 
	#define CAL_ServerRegisterServiceHandler  ServerRegisterServiceHandler
	#define CHK_ServerRegisterServiceHandler  TRUE
	#define EXP_ServerRegisterServiceHandler  CAL_CMEXPAPI( "ServerRegisterServiceHandler" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerRegisterServiceHandler
	#define EXT_ServerRegisterServiceHandler
	#define GET_ServerRegisterServiceHandler(fl)  CAL_CMGETAPI( "ServerRegisterServiceHandler" ) 
	#define CAL_ServerRegisterServiceHandler  ServerRegisterServiceHandler
	#define CHK_ServerRegisterServiceHandler  TRUE
	#define EXP_ServerRegisterServiceHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterServiceHandler", (RTS_UINTPTR)ServerRegisterServiceHandler, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerRegisterServiceHandler
	#define EXT_CmpSrvServerRegisterServiceHandler
	#define GET_CmpSrvServerRegisterServiceHandler  ERR_OK
	#define CAL_CmpSrvServerRegisterServiceHandler pICmpSrv->IServerRegisterServiceHandler
	#define CHK_CmpSrvServerRegisterServiceHandler (pICmpSrv != NULL)
	#define EXP_CmpSrvServerRegisterServiceHandler  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerRegisterServiceHandler
	#define EXT_ServerRegisterServiceHandler
	#define GET_ServerRegisterServiceHandler(fl)  CAL_CMGETAPI( "ServerRegisterServiceHandler" ) 
	#define CAL_ServerRegisterServiceHandler pICmpSrv->IServerRegisterServiceHandler
	#define CHK_ServerRegisterServiceHandler (pICmpSrv != NULL)
	#define EXP_ServerRegisterServiceHandler  CAL_CMEXPAPI( "ServerRegisterServiceHandler" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerRegisterServiceHandler  PFSERVERREGISTERSERVICEHANDLER pfServerRegisterServiceHandler;
	#define EXT_ServerRegisterServiceHandler  extern PFSERVERREGISTERSERVICEHANDLER pfServerRegisterServiceHandler;
	#define GET_ServerRegisterServiceHandler(fl)  s_pfCMGetAPI2( "ServerRegisterServiceHandler", (RTS_VOID_FCTPTR *)&pfServerRegisterServiceHandler, (fl), 0, 0)
	#define CAL_ServerRegisterServiceHandler  pfServerRegisterServiceHandler
	#define CHK_ServerRegisterServiceHandler  (pfServerRegisterServiceHandler != NULL)
	#define EXP_ServerRegisterServiceHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterServiceHandler", (RTS_UINTPTR)ServerRegisterServiceHandler, 0, 0) 
#endif




/**
 * <description>
 * <p>Obsolete: Use ServerRegisterServiceHandler instead. Will be removed in future versions!</p>
 * <p>Register a handler for requests to a specific service group</p></description>
 * <param name="ulServiceGroup" type="IN">The service group which is handled by the provided function</param>
 * <param name="pfServiceHandler" type="IN">A handler function.</param>
 * <param name="pszRouter" type="IN">Obsolete parameter, should be set to NULL.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL ServerRegisterServiceHandler2(RTS_UI32 ulServiceGroup, PFServiceHandler pfServiceHandler, char *pszRouter);
typedef RTS_RESULT (CDECL * PFSERVERREGISTERSERVICEHANDLER2) (RTS_UI32 ulServiceGroup, PFServiceHandler pfServiceHandler, char *pszRouter);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERREGISTERSERVICEHANDLER2_NOTIMPLEMENTED)
	#define USE_ServerRegisterServiceHandler2
	#define EXT_ServerRegisterServiceHandler2
	#define GET_ServerRegisterServiceHandler2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerRegisterServiceHandler2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerRegisterServiceHandler2  FALSE
	#define EXP_ServerRegisterServiceHandler2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerRegisterServiceHandler2
	#define EXT_ServerRegisterServiceHandler2
	#define GET_ServerRegisterServiceHandler2(fl)  CAL_CMGETAPI( "ServerRegisterServiceHandler2" ) 
	#define CAL_ServerRegisterServiceHandler2  ServerRegisterServiceHandler2
	#define CHK_ServerRegisterServiceHandler2  TRUE
	#define EXP_ServerRegisterServiceHandler2  CAL_CMEXPAPI( "ServerRegisterServiceHandler2" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerRegisterServiceHandler2
	#define EXT_ServerRegisterServiceHandler2
	#define GET_ServerRegisterServiceHandler2(fl)  CAL_CMGETAPI( "ServerRegisterServiceHandler2" ) 
	#define CAL_ServerRegisterServiceHandler2  ServerRegisterServiceHandler2
	#define CHK_ServerRegisterServiceHandler2  TRUE
	#define EXP_ServerRegisterServiceHandler2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterServiceHandler2", (RTS_UINTPTR)ServerRegisterServiceHandler2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerRegisterServiceHandler2
	#define EXT_CmpSrvServerRegisterServiceHandler2
	#define GET_CmpSrvServerRegisterServiceHandler2  ERR_OK
	#define CAL_CmpSrvServerRegisterServiceHandler2 pICmpSrv->IServerRegisterServiceHandler2
	#define CHK_CmpSrvServerRegisterServiceHandler2 (pICmpSrv != NULL)
	#define EXP_CmpSrvServerRegisterServiceHandler2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerRegisterServiceHandler2
	#define EXT_ServerRegisterServiceHandler2
	#define GET_ServerRegisterServiceHandler2(fl)  CAL_CMGETAPI( "ServerRegisterServiceHandler2" ) 
	#define CAL_ServerRegisterServiceHandler2 pICmpSrv->IServerRegisterServiceHandler2
	#define CHK_ServerRegisterServiceHandler2 (pICmpSrv != NULL)
	#define EXP_ServerRegisterServiceHandler2  CAL_CMEXPAPI( "ServerRegisterServiceHandler2" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerRegisterServiceHandler2  PFSERVERREGISTERSERVICEHANDLER2 pfServerRegisterServiceHandler2;
	#define EXT_ServerRegisterServiceHandler2  extern PFSERVERREGISTERSERVICEHANDLER2 pfServerRegisterServiceHandler2;
	#define GET_ServerRegisterServiceHandler2(fl)  s_pfCMGetAPI2( "ServerRegisterServiceHandler2", (RTS_VOID_FCTPTR *)&pfServerRegisterServiceHandler2, (fl), 0, 0)
	#define CAL_ServerRegisterServiceHandler2  pfServerRegisterServiceHandler2
	#define CHK_ServerRegisterServiceHandler2  (pfServerRegisterServiceHandler2 != NULL)
	#define EXP_ServerRegisterServiceHandler2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterServiceHandler2", (RTS_UINTPTR)ServerRegisterServiceHandler2, 0, 0) 
#endif




/**
 * <description>
 * <p>Obsolete: Use ServerRegisterServiceHandler instead. Will be removed in future versions!</p>
 * <p>Register a handler for requests to a specific service group</p></description>
 * <param name="ulServiceGroup" type="IN">The service group which is handled by the provided function</param>
 * <param name="pfServiceHandler2" type="IN">A handler function.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL ServerRegisterServiceHandler3(RTS_UI32 ulServiceGroup, PFServiceHandler2 pfServiceHandler2);
typedef RTS_RESULT (CDECL * PFSERVERREGISTERSERVICEHANDLER3) (RTS_UI32 ulServiceGroup, PFServiceHandler2 pfServiceHandler2);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERREGISTERSERVICEHANDLER3_NOTIMPLEMENTED)
	#define USE_ServerRegisterServiceHandler3
	#define EXT_ServerRegisterServiceHandler3
	#define GET_ServerRegisterServiceHandler3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerRegisterServiceHandler3(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerRegisterServiceHandler3  FALSE
	#define EXP_ServerRegisterServiceHandler3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerRegisterServiceHandler3
	#define EXT_ServerRegisterServiceHandler3
	#define GET_ServerRegisterServiceHandler3(fl)  CAL_CMGETAPI( "ServerRegisterServiceHandler3" ) 
	#define CAL_ServerRegisterServiceHandler3  ServerRegisterServiceHandler3
	#define CHK_ServerRegisterServiceHandler3  TRUE
	#define EXP_ServerRegisterServiceHandler3  CAL_CMEXPAPI( "ServerRegisterServiceHandler3" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerRegisterServiceHandler3
	#define EXT_ServerRegisterServiceHandler3
	#define GET_ServerRegisterServiceHandler3(fl)  CAL_CMGETAPI( "ServerRegisterServiceHandler3" ) 
	#define CAL_ServerRegisterServiceHandler3  ServerRegisterServiceHandler3
	#define CHK_ServerRegisterServiceHandler3  TRUE
	#define EXP_ServerRegisterServiceHandler3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterServiceHandler3", (RTS_UINTPTR)ServerRegisterServiceHandler3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerRegisterServiceHandler3
	#define EXT_CmpSrvServerRegisterServiceHandler3
	#define GET_CmpSrvServerRegisterServiceHandler3  ERR_OK
	#define CAL_CmpSrvServerRegisterServiceHandler3 pICmpSrv->IServerRegisterServiceHandler3
	#define CHK_CmpSrvServerRegisterServiceHandler3 (pICmpSrv != NULL)
	#define EXP_CmpSrvServerRegisterServiceHandler3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerRegisterServiceHandler3
	#define EXT_ServerRegisterServiceHandler3
	#define GET_ServerRegisterServiceHandler3(fl)  CAL_CMGETAPI( "ServerRegisterServiceHandler3" ) 
	#define CAL_ServerRegisterServiceHandler3 pICmpSrv->IServerRegisterServiceHandler3
	#define CHK_ServerRegisterServiceHandler3 (pICmpSrv != NULL)
	#define EXP_ServerRegisterServiceHandler3  CAL_CMEXPAPI( "ServerRegisterServiceHandler3" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerRegisterServiceHandler3  PFSERVERREGISTERSERVICEHANDLER3 pfServerRegisterServiceHandler3;
	#define EXT_ServerRegisterServiceHandler3  extern PFSERVERREGISTERSERVICEHANDLER3 pfServerRegisterServiceHandler3;
	#define GET_ServerRegisterServiceHandler3(fl)  s_pfCMGetAPI2( "ServerRegisterServiceHandler3", (RTS_VOID_FCTPTR *)&pfServerRegisterServiceHandler3, (fl), 0, 0)
	#define CAL_ServerRegisterServiceHandler3  pfServerRegisterServiceHandler3
	#define CHK_ServerRegisterServiceHandler3  (pfServerRegisterServiceHandler3 != NULL)
	#define EXP_ServerRegisterServiceHandler3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterServiceHandler3", (RTS_UINTPTR)ServerRegisterServiceHandler3, 0, 0) 
#endif




/**
 * <description>Unregister a handler for requests to a specific service group</description>
 * <param name="ulServiceGroup" type="IN">The service group which is handled by the provided function</param>
 * <param name="pfServiceHandler" type="IN">A handler function.</param>
 */
RTS_RESULT CDECL ServerUnRegisterServiceHandler(RTS_UI32 ulServiceGroup, PFServiceHandler pfServiceHandler);
typedef RTS_RESULT (CDECL * PFSERVERUNREGISTERSERVICEHANDLER) (RTS_UI32 ulServiceGroup, PFServiceHandler pfServiceHandler);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERUNREGISTERSERVICEHANDLER_NOTIMPLEMENTED)
	#define USE_ServerUnRegisterServiceHandler
	#define EXT_ServerUnRegisterServiceHandler
	#define GET_ServerUnRegisterServiceHandler(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerUnRegisterServiceHandler(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerUnRegisterServiceHandler  FALSE
	#define EXP_ServerUnRegisterServiceHandler  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerUnRegisterServiceHandler
	#define EXT_ServerUnRegisterServiceHandler
	#define GET_ServerUnRegisterServiceHandler(fl)  CAL_CMGETAPI( "ServerUnRegisterServiceHandler" ) 
	#define CAL_ServerUnRegisterServiceHandler  ServerUnRegisterServiceHandler
	#define CHK_ServerUnRegisterServiceHandler  TRUE
	#define EXP_ServerUnRegisterServiceHandler  CAL_CMEXPAPI( "ServerUnRegisterServiceHandler" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerUnRegisterServiceHandler
	#define EXT_ServerUnRegisterServiceHandler
	#define GET_ServerUnRegisterServiceHandler(fl)  CAL_CMGETAPI( "ServerUnRegisterServiceHandler" ) 
	#define CAL_ServerUnRegisterServiceHandler  ServerUnRegisterServiceHandler
	#define CHK_ServerUnRegisterServiceHandler  TRUE
	#define EXP_ServerUnRegisterServiceHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerUnRegisterServiceHandler", (RTS_UINTPTR)ServerUnRegisterServiceHandler, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerUnRegisterServiceHandler
	#define EXT_CmpSrvServerUnRegisterServiceHandler
	#define GET_CmpSrvServerUnRegisterServiceHandler  ERR_OK
	#define CAL_CmpSrvServerUnRegisterServiceHandler pICmpSrv->IServerUnRegisterServiceHandler
	#define CHK_CmpSrvServerUnRegisterServiceHandler (pICmpSrv != NULL)
	#define EXP_CmpSrvServerUnRegisterServiceHandler  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerUnRegisterServiceHandler
	#define EXT_ServerUnRegisterServiceHandler
	#define GET_ServerUnRegisterServiceHandler(fl)  CAL_CMGETAPI( "ServerUnRegisterServiceHandler" ) 
	#define CAL_ServerUnRegisterServiceHandler pICmpSrv->IServerUnRegisterServiceHandler
	#define CHK_ServerUnRegisterServiceHandler (pICmpSrv != NULL)
	#define EXP_ServerUnRegisterServiceHandler  CAL_CMEXPAPI( "ServerUnRegisterServiceHandler" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerUnRegisterServiceHandler  PFSERVERUNREGISTERSERVICEHANDLER pfServerUnRegisterServiceHandler;
	#define EXT_ServerUnRegisterServiceHandler  extern PFSERVERUNREGISTERSERVICEHANDLER pfServerUnRegisterServiceHandler;
	#define GET_ServerUnRegisterServiceHandler(fl)  s_pfCMGetAPI2( "ServerUnRegisterServiceHandler", (RTS_VOID_FCTPTR *)&pfServerUnRegisterServiceHandler, (fl), 0, 0)
	#define CAL_ServerUnRegisterServiceHandler  pfServerUnRegisterServiceHandler
	#define CHK_ServerUnRegisterServiceHandler  (pfServerUnRegisterServiceHandler != NULL)
	#define EXP_ServerUnRegisterServiceHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerUnRegisterServiceHandler", (RTS_UINTPTR)ServerUnRegisterServiceHandler, 0, 0) 
#endif




/**
 * <description>
 *	 Register a handler for requests of a specific protocol other then HEADERTAG_3S. All requests with that protocol
 *   will be sent to this handler.
 * </description>
 * <param name="usProtocolId" type="IN">The protocol id which is handled by the provided function</param>
 * <param name="pfServiceHandler" type="IN">A handler function.</param>
 */
RTS_RESULT CDECL ServerRegisterProtocolHandler(RTS_UI16 usProtocolId, PFServiceHandler pfServiceHandler);
typedef RTS_RESULT (CDECL * PFSERVERREGISTERPROTOCOLHANDLER) (RTS_UI16 usProtocolId, PFServiceHandler pfServiceHandler);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERREGISTERPROTOCOLHANDLER_NOTIMPLEMENTED)
	#define USE_ServerRegisterProtocolHandler
	#define EXT_ServerRegisterProtocolHandler
	#define GET_ServerRegisterProtocolHandler(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerRegisterProtocolHandler(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerRegisterProtocolHandler  FALSE
	#define EXP_ServerRegisterProtocolHandler  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerRegisterProtocolHandler
	#define EXT_ServerRegisterProtocolHandler
	#define GET_ServerRegisterProtocolHandler(fl)  CAL_CMGETAPI( "ServerRegisterProtocolHandler" ) 
	#define CAL_ServerRegisterProtocolHandler  ServerRegisterProtocolHandler
	#define CHK_ServerRegisterProtocolHandler  TRUE
	#define EXP_ServerRegisterProtocolHandler  CAL_CMEXPAPI( "ServerRegisterProtocolHandler" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerRegisterProtocolHandler
	#define EXT_ServerRegisterProtocolHandler
	#define GET_ServerRegisterProtocolHandler(fl)  CAL_CMGETAPI( "ServerRegisterProtocolHandler" ) 
	#define CAL_ServerRegisterProtocolHandler  ServerRegisterProtocolHandler
	#define CHK_ServerRegisterProtocolHandler  TRUE
	#define EXP_ServerRegisterProtocolHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterProtocolHandler", (RTS_UINTPTR)ServerRegisterProtocolHandler, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerRegisterProtocolHandler
	#define EXT_CmpSrvServerRegisterProtocolHandler
	#define GET_CmpSrvServerRegisterProtocolHandler  ERR_OK
	#define CAL_CmpSrvServerRegisterProtocolHandler pICmpSrv->IServerRegisterProtocolHandler
	#define CHK_CmpSrvServerRegisterProtocolHandler (pICmpSrv != NULL)
	#define EXP_CmpSrvServerRegisterProtocolHandler  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerRegisterProtocolHandler
	#define EXT_ServerRegisterProtocolHandler
	#define GET_ServerRegisterProtocolHandler(fl)  CAL_CMGETAPI( "ServerRegisterProtocolHandler" ) 
	#define CAL_ServerRegisterProtocolHandler pICmpSrv->IServerRegisterProtocolHandler
	#define CHK_ServerRegisterProtocolHandler (pICmpSrv != NULL)
	#define EXP_ServerRegisterProtocolHandler  CAL_CMEXPAPI( "ServerRegisterProtocolHandler" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerRegisterProtocolHandler  PFSERVERREGISTERPROTOCOLHANDLER pfServerRegisterProtocolHandler;
	#define EXT_ServerRegisterProtocolHandler  extern PFSERVERREGISTERPROTOCOLHANDLER pfServerRegisterProtocolHandler;
	#define GET_ServerRegisterProtocolHandler(fl)  s_pfCMGetAPI2( "ServerRegisterProtocolHandler", (RTS_VOID_FCTPTR *)&pfServerRegisterProtocolHandler, (fl), 0, 0)
	#define CAL_ServerRegisterProtocolHandler  pfServerRegisterProtocolHandler
	#define CHK_ServerRegisterProtocolHandler  (pfServerRegisterProtocolHandler != NULL)
	#define EXP_ServerRegisterProtocolHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterProtocolHandler", (RTS_UINTPTR)ServerRegisterProtocolHandler, 0, 0) 
#endif




/**
 * <description>
 * <p>Obsolete: Use ServerRegisterProtocolHandler instead. Will be removed in future versions!</p>
 * <p> Register a handler for requests of a specific protocol other then HEADERTAG_3S. All requests with that protocol
 *   will be sent to this handler.</p>
 * </description>
 * <param name="usProtocolId" type="IN">The protocol id which is handled by the provided function</param>
 * <param name="pfServiceHandler" type="IN">A handler function.</param>
 * <param name="pszRouter" type="IN">Obsolete parameter, should be set to NULL.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL ServerRegisterProtocolHandler2(RTS_UI16 usProtocolId, PFServiceHandler pfServiceHandler, char *pszRouter);
typedef RTS_RESULT (CDECL * PFSERVERREGISTERPROTOCOLHANDLER2) (RTS_UI16 usProtocolId, PFServiceHandler pfServiceHandler, char *pszRouter);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERREGISTERPROTOCOLHANDLER2_NOTIMPLEMENTED)
	#define USE_ServerRegisterProtocolHandler2
	#define EXT_ServerRegisterProtocolHandler2
	#define GET_ServerRegisterProtocolHandler2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerRegisterProtocolHandler2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerRegisterProtocolHandler2  FALSE
	#define EXP_ServerRegisterProtocolHandler2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerRegisterProtocolHandler2
	#define EXT_ServerRegisterProtocolHandler2
	#define GET_ServerRegisterProtocolHandler2(fl)  CAL_CMGETAPI( "ServerRegisterProtocolHandler2" ) 
	#define CAL_ServerRegisterProtocolHandler2  ServerRegisterProtocolHandler2
	#define CHK_ServerRegisterProtocolHandler2  TRUE
	#define EXP_ServerRegisterProtocolHandler2  CAL_CMEXPAPI( "ServerRegisterProtocolHandler2" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerRegisterProtocolHandler2
	#define EXT_ServerRegisterProtocolHandler2
	#define GET_ServerRegisterProtocolHandler2(fl)  CAL_CMGETAPI( "ServerRegisterProtocolHandler2" ) 
	#define CAL_ServerRegisterProtocolHandler2  ServerRegisterProtocolHandler2
	#define CHK_ServerRegisterProtocolHandler2  TRUE
	#define EXP_ServerRegisterProtocolHandler2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterProtocolHandler2", (RTS_UINTPTR)ServerRegisterProtocolHandler2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerRegisterProtocolHandler2
	#define EXT_CmpSrvServerRegisterProtocolHandler2
	#define GET_CmpSrvServerRegisterProtocolHandler2  ERR_OK
	#define CAL_CmpSrvServerRegisterProtocolHandler2 pICmpSrv->IServerRegisterProtocolHandler2
	#define CHK_CmpSrvServerRegisterProtocolHandler2 (pICmpSrv != NULL)
	#define EXP_CmpSrvServerRegisterProtocolHandler2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerRegisterProtocolHandler2
	#define EXT_ServerRegisterProtocolHandler2
	#define GET_ServerRegisterProtocolHandler2(fl)  CAL_CMGETAPI( "ServerRegisterProtocolHandler2" ) 
	#define CAL_ServerRegisterProtocolHandler2 pICmpSrv->IServerRegisterProtocolHandler2
	#define CHK_ServerRegisterProtocolHandler2 (pICmpSrv != NULL)
	#define EXP_ServerRegisterProtocolHandler2  CAL_CMEXPAPI( "ServerRegisterProtocolHandler2" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerRegisterProtocolHandler2  PFSERVERREGISTERPROTOCOLHANDLER2 pfServerRegisterProtocolHandler2;
	#define EXT_ServerRegisterProtocolHandler2  extern PFSERVERREGISTERPROTOCOLHANDLER2 pfServerRegisterProtocolHandler2;
	#define GET_ServerRegisterProtocolHandler2(fl)  s_pfCMGetAPI2( "ServerRegisterProtocolHandler2", (RTS_VOID_FCTPTR *)&pfServerRegisterProtocolHandler2, (fl), 0, 0)
	#define CAL_ServerRegisterProtocolHandler2  pfServerRegisterProtocolHandler2
	#define CHK_ServerRegisterProtocolHandler2  (pfServerRegisterProtocolHandler2 != NULL)
	#define EXP_ServerRegisterProtocolHandler2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerRegisterProtocolHandler2", (RTS_UINTPTR)ServerRegisterProtocolHandler2, 0, 0) 
#endif




/**
 * <description>
 *	 Unregister a handler for requests of a specific protocol.
 * </description>
 * <param name="usProtocolId" type="IN">The protocol id which is handled by the provided function</param>
 * <param name="pfServiceHandler" type="IN">A handler function.</param>
 */
RTS_RESULT CDECL ServerUnRegisterProtocolHandler(RTS_UI16 usProtocolId, PFServiceHandler pfServiceHandler);
typedef RTS_RESULT (CDECL * PFSERVERUNREGISTERPROTOCOLHANDLER) (RTS_UI16 usProtocolId, PFServiceHandler pfServiceHandler);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERUNREGISTERPROTOCOLHANDLER_NOTIMPLEMENTED)
	#define USE_ServerUnRegisterProtocolHandler
	#define EXT_ServerUnRegisterProtocolHandler
	#define GET_ServerUnRegisterProtocolHandler(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerUnRegisterProtocolHandler(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerUnRegisterProtocolHandler  FALSE
	#define EXP_ServerUnRegisterProtocolHandler  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerUnRegisterProtocolHandler
	#define EXT_ServerUnRegisterProtocolHandler
	#define GET_ServerUnRegisterProtocolHandler(fl)  CAL_CMGETAPI( "ServerUnRegisterProtocolHandler" ) 
	#define CAL_ServerUnRegisterProtocolHandler  ServerUnRegisterProtocolHandler
	#define CHK_ServerUnRegisterProtocolHandler  TRUE
	#define EXP_ServerUnRegisterProtocolHandler  CAL_CMEXPAPI( "ServerUnRegisterProtocolHandler" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerUnRegisterProtocolHandler
	#define EXT_ServerUnRegisterProtocolHandler
	#define GET_ServerUnRegisterProtocolHandler(fl)  CAL_CMGETAPI( "ServerUnRegisterProtocolHandler" ) 
	#define CAL_ServerUnRegisterProtocolHandler  ServerUnRegisterProtocolHandler
	#define CHK_ServerUnRegisterProtocolHandler  TRUE
	#define EXP_ServerUnRegisterProtocolHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerUnRegisterProtocolHandler", (RTS_UINTPTR)ServerUnRegisterProtocolHandler, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerUnRegisterProtocolHandler
	#define EXT_CmpSrvServerUnRegisterProtocolHandler
	#define GET_CmpSrvServerUnRegisterProtocolHandler  ERR_OK
	#define CAL_CmpSrvServerUnRegisterProtocolHandler pICmpSrv->IServerUnRegisterProtocolHandler
	#define CHK_CmpSrvServerUnRegisterProtocolHandler (pICmpSrv != NULL)
	#define EXP_CmpSrvServerUnRegisterProtocolHandler  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerUnRegisterProtocolHandler
	#define EXT_ServerUnRegisterProtocolHandler
	#define GET_ServerUnRegisterProtocolHandler(fl)  CAL_CMGETAPI( "ServerUnRegisterProtocolHandler" ) 
	#define CAL_ServerUnRegisterProtocolHandler pICmpSrv->IServerUnRegisterProtocolHandler
	#define CHK_ServerUnRegisterProtocolHandler (pICmpSrv != NULL)
	#define EXP_ServerUnRegisterProtocolHandler  CAL_CMEXPAPI( "ServerUnRegisterProtocolHandler" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerUnRegisterProtocolHandler  PFSERVERUNREGISTERPROTOCOLHANDLER pfServerUnRegisterProtocolHandler;
	#define EXT_ServerUnRegisterProtocolHandler  extern PFSERVERUNREGISTERPROTOCOLHANDLER pfServerUnRegisterProtocolHandler;
	#define GET_ServerUnRegisterProtocolHandler(fl)  s_pfCMGetAPI2( "ServerUnRegisterProtocolHandler", (RTS_VOID_FCTPTR *)&pfServerUnRegisterProtocolHandler, (fl), 0, 0)
	#define CAL_ServerUnRegisterProtocolHandler  pfServerUnRegisterProtocolHandler
	#define CHK_ServerUnRegisterProtocolHandler  (pfServerUnRegisterProtocolHandler != NULL)
	#define EXP_ServerUnRegisterProtocolHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerUnRegisterProtocolHandler", (RTS_UINTPTR)ServerUnRegisterProtocolHandler, 0, 0) 
#endif




/**
 * <description>
 *   Send a reply to a request previously received by a service handler 
 * </description>
 * <param name="ulChannelId" type="IN">
 *   Id of the channel on which to answer a reply. Must be the same that was passed in to the service
 *   handler function.
 * </param>
 * <param name="pduData" type="IN"> 
 *   Contains the buffer and the length of the reply data. Buffer must be the same as the one passed in 
 *   to the service handler function, the length must not be greater then the maximum reply buffer length.
 * </param>
 */
RTS_RESULT CDECL ServerFinishRequest(RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduData);
typedef RTS_RESULT (CDECL * PFSERVERFINISHREQUEST) (RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduData);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERFINISHREQUEST_NOTIMPLEMENTED)
	#define USE_ServerFinishRequest
	#define EXT_ServerFinishRequest
	#define GET_ServerFinishRequest(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerFinishRequest(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerFinishRequest  FALSE
	#define EXP_ServerFinishRequest  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerFinishRequest
	#define EXT_ServerFinishRequest
	#define GET_ServerFinishRequest(fl)  CAL_CMGETAPI( "ServerFinishRequest" ) 
	#define CAL_ServerFinishRequest  ServerFinishRequest
	#define CHK_ServerFinishRequest  TRUE
	#define EXP_ServerFinishRequest  CAL_CMEXPAPI( "ServerFinishRequest" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerFinishRequest
	#define EXT_ServerFinishRequest
	#define GET_ServerFinishRequest(fl)  CAL_CMGETAPI( "ServerFinishRequest" ) 
	#define CAL_ServerFinishRequest  ServerFinishRequest
	#define CHK_ServerFinishRequest  TRUE
	#define EXP_ServerFinishRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerFinishRequest", (RTS_UINTPTR)ServerFinishRequest, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerFinishRequest
	#define EXT_CmpSrvServerFinishRequest
	#define GET_CmpSrvServerFinishRequest  ERR_OK
	#define CAL_CmpSrvServerFinishRequest pICmpSrv->IServerFinishRequest
	#define CHK_CmpSrvServerFinishRequest (pICmpSrv != NULL)
	#define EXP_CmpSrvServerFinishRequest  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerFinishRequest
	#define EXT_ServerFinishRequest
	#define GET_ServerFinishRequest(fl)  CAL_CMGETAPI( "ServerFinishRequest" ) 
	#define CAL_ServerFinishRequest pICmpSrv->IServerFinishRequest
	#define CHK_ServerFinishRequest (pICmpSrv != NULL)
	#define EXP_ServerFinishRequest  CAL_CMEXPAPI( "ServerFinishRequest" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerFinishRequest  PFSERVERFINISHREQUEST pfServerFinishRequest;
	#define EXT_ServerFinishRequest  extern PFSERVERFINISHREQUEST pfServerFinishRequest;
	#define GET_ServerFinishRequest(fl)  s_pfCMGetAPI2( "ServerFinishRequest", (RTS_VOID_FCTPTR *)&pfServerFinishRequest, (fl), 0, 0)
	#define CAL_ServerFinishRequest  pfServerFinishRequest
	#define CHK_ServerFinishRequest  (pfServerFinishRequest != NULL)
	#define EXP_ServerFinishRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerFinishRequest", (RTS_UINTPTR)ServerFinishRequest, 0, 0) 
#endif




/**
 * <description>Generates a unique session Id</description>
 * <param name="pulSessionId" type="OUT">Pointer to get back the generated session Id</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL ServerGenerateSessionId(RTS_UI32 *pulSessionId);
typedef RTS_RESULT (CDECL * PFSERVERGENERATESESSIONID) (RTS_UI32 *pulSessionId);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERGENERATESESSIONID_NOTIMPLEMENTED)
	#define USE_ServerGenerateSessionId
	#define EXT_ServerGenerateSessionId
	#define GET_ServerGenerateSessionId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerGenerateSessionId(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerGenerateSessionId  FALSE
	#define EXP_ServerGenerateSessionId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerGenerateSessionId
	#define EXT_ServerGenerateSessionId
	#define GET_ServerGenerateSessionId(fl)  CAL_CMGETAPI( "ServerGenerateSessionId" ) 
	#define CAL_ServerGenerateSessionId  ServerGenerateSessionId
	#define CHK_ServerGenerateSessionId  TRUE
	#define EXP_ServerGenerateSessionId  CAL_CMEXPAPI( "ServerGenerateSessionId" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerGenerateSessionId
	#define EXT_ServerGenerateSessionId
	#define GET_ServerGenerateSessionId(fl)  CAL_CMGETAPI( "ServerGenerateSessionId" ) 
	#define CAL_ServerGenerateSessionId  ServerGenerateSessionId
	#define CHK_ServerGenerateSessionId  TRUE
	#define EXP_ServerGenerateSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGenerateSessionId", (RTS_UINTPTR)ServerGenerateSessionId, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerGenerateSessionId
	#define EXT_CmpSrvServerGenerateSessionId
	#define GET_CmpSrvServerGenerateSessionId  ERR_OK
	#define CAL_CmpSrvServerGenerateSessionId pICmpSrv->IServerGenerateSessionId
	#define CHK_CmpSrvServerGenerateSessionId (pICmpSrv != NULL)
	#define EXP_CmpSrvServerGenerateSessionId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerGenerateSessionId
	#define EXT_ServerGenerateSessionId
	#define GET_ServerGenerateSessionId(fl)  CAL_CMGETAPI( "ServerGenerateSessionId" ) 
	#define CAL_ServerGenerateSessionId pICmpSrv->IServerGenerateSessionId
	#define CHK_ServerGenerateSessionId (pICmpSrv != NULL)
	#define EXP_ServerGenerateSessionId  CAL_CMEXPAPI( "ServerGenerateSessionId" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerGenerateSessionId  PFSERVERGENERATESESSIONID pfServerGenerateSessionId;
	#define EXT_ServerGenerateSessionId  extern PFSERVERGENERATESESSIONID pfServerGenerateSessionId;
	#define GET_ServerGenerateSessionId(fl)  s_pfCMGetAPI2( "ServerGenerateSessionId", (RTS_VOID_FCTPTR *)&pfServerGenerateSessionId, (fl), 0, 0)
	#define CAL_ServerGenerateSessionId  pfServerGenerateSessionId
	#define CHK_ServerGenerateSessionId  (pfServerGenerateSessionId != NULL)
	#define EXP_ServerGenerateSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGenerateSessionId", (RTS_UINTPTR)ServerGenerateSessionId, 0, 0) 
#endif




/** 
 * <description>
 *   Stores the session id in the channel instance.  
 * </description>
 * <param name="ulChannelHandle" type="IN">Id of the channel for which the session id should be set.</param>
 * <param name="ulSessionId" type="IN">New session id fo the channel.</param>
 * <result>error code</result>
*/
RTS_RESULT CDECL ServerSetSessionId(RTS_UI32 ulChannelHandle, RTS_UI32 ulSessionId);
typedef RTS_RESULT (CDECL * PFSERVERSETSESSIONID) (RTS_UI32 ulChannelHandle, RTS_UI32 ulSessionId);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERSETSESSIONID_NOTIMPLEMENTED)
	#define USE_ServerSetSessionId
	#define EXT_ServerSetSessionId
	#define GET_ServerSetSessionId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerSetSessionId(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerSetSessionId  FALSE
	#define EXP_ServerSetSessionId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerSetSessionId
	#define EXT_ServerSetSessionId
	#define GET_ServerSetSessionId(fl)  CAL_CMGETAPI( "ServerSetSessionId" ) 
	#define CAL_ServerSetSessionId  ServerSetSessionId
	#define CHK_ServerSetSessionId  TRUE
	#define EXP_ServerSetSessionId  CAL_CMEXPAPI( "ServerSetSessionId" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerSetSessionId
	#define EXT_ServerSetSessionId
	#define GET_ServerSetSessionId(fl)  CAL_CMGETAPI( "ServerSetSessionId" ) 
	#define CAL_ServerSetSessionId  ServerSetSessionId
	#define CHK_ServerSetSessionId  TRUE
	#define EXP_ServerSetSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerSetSessionId", (RTS_UINTPTR)ServerSetSessionId, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerSetSessionId
	#define EXT_CmpSrvServerSetSessionId
	#define GET_CmpSrvServerSetSessionId  ERR_OK
	#define CAL_CmpSrvServerSetSessionId pICmpSrv->IServerSetSessionId
	#define CHK_CmpSrvServerSetSessionId (pICmpSrv != NULL)
	#define EXP_CmpSrvServerSetSessionId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerSetSessionId
	#define EXT_ServerSetSessionId
	#define GET_ServerSetSessionId(fl)  CAL_CMGETAPI( "ServerSetSessionId" ) 
	#define CAL_ServerSetSessionId pICmpSrv->IServerSetSessionId
	#define CHK_ServerSetSessionId (pICmpSrv != NULL)
	#define EXP_ServerSetSessionId  CAL_CMEXPAPI( "ServerSetSessionId" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerSetSessionId  PFSERVERSETSESSIONID pfServerSetSessionId;
	#define EXT_ServerSetSessionId  extern PFSERVERSETSESSIONID pfServerSetSessionId;
	#define GET_ServerSetSessionId(fl)  s_pfCMGetAPI2( "ServerSetSessionId", (RTS_VOID_FCTPTR *)&pfServerSetSessionId, (fl), 0, 0)
	#define CAL_ServerSetSessionId  pfServerSetSessionId
	#define CHK_ServerSetSessionId  (pfServerSetSessionId != NULL)
	#define EXP_ServerSetSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerSetSessionId", (RTS_UINTPTR)ServerSetSessionId, 0, 0) 
#endif




/** 
 * <description>
 *   Retrieves the stored session id from the channel instance.  
 * </description>
 * <param name="ulChannelHandle" type="IN">Id of the channel for which the session id should be read.</param>
 * <param name="pulSessionId" type="OUT">Pointer to return the session id.</param>
 * <result>error code</result>
*/
RTS_RESULT CDECL ServerGetSessionId(RTS_UI32 ulChannelHandle, RTS_UI32 *pulSessionId);
typedef RTS_RESULT (CDECL * PFSERVERGETSESSIONID) (RTS_UI32 ulChannelHandle, RTS_UI32 *pulSessionId);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERGETSESSIONID_NOTIMPLEMENTED)
	#define USE_ServerGetSessionId
	#define EXT_ServerGetSessionId
	#define GET_ServerGetSessionId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerGetSessionId(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerGetSessionId  FALSE
	#define EXP_ServerGetSessionId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerGetSessionId
	#define EXT_ServerGetSessionId
	#define GET_ServerGetSessionId(fl)  CAL_CMGETAPI( "ServerGetSessionId" ) 
	#define CAL_ServerGetSessionId  ServerGetSessionId
	#define CHK_ServerGetSessionId  TRUE
	#define EXP_ServerGetSessionId  CAL_CMEXPAPI( "ServerGetSessionId" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerGetSessionId
	#define EXT_ServerGetSessionId
	#define GET_ServerGetSessionId(fl)  CAL_CMGETAPI( "ServerGetSessionId" ) 
	#define CAL_ServerGetSessionId  ServerGetSessionId
	#define CHK_ServerGetSessionId  TRUE
	#define EXP_ServerGetSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGetSessionId", (RTS_UINTPTR)ServerGetSessionId, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerGetSessionId
	#define EXT_CmpSrvServerGetSessionId
	#define GET_CmpSrvServerGetSessionId  ERR_OK
	#define CAL_CmpSrvServerGetSessionId pICmpSrv->IServerGetSessionId
	#define CHK_CmpSrvServerGetSessionId (pICmpSrv != NULL)
	#define EXP_CmpSrvServerGetSessionId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerGetSessionId
	#define EXT_ServerGetSessionId
	#define GET_ServerGetSessionId(fl)  CAL_CMGETAPI( "ServerGetSessionId" ) 
	#define CAL_ServerGetSessionId pICmpSrv->IServerGetSessionId
	#define CHK_ServerGetSessionId (pICmpSrv != NULL)
	#define EXP_ServerGetSessionId  CAL_CMEXPAPI( "ServerGetSessionId" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerGetSessionId  PFSERVERGETSESSIONID pfServerGetSessionId;
	#define EXT_ServerGetSessionId  extern PFSERVERGETSESSIONID pfServerGetSessionId;
	#define GET_ServerGetSessionId(fl)  s_pfCMGetAPI2( "ServerGetSessionId", (RTS_VOID_FCTPTR *)&pfServerGetSessionId, (fl), 0, 0)
	#define CAL_ServerGetSessionId  pfServerGetSessionId
	#define CHK_ServerGetSessionId  (pfServerGetSessionId != NULL)
	#define EXP_ServerGetSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGetSessionId", (RTS_UINTPTR)ServerGetSessionId, 0, 0) 
#endif




/**
* <description>
*   Retrieves the number of available server channels.
*   This is equal to the max. number of clients, which can be connected at the same time.
* </description>
* <param name="pwMaxChannels" type="OUT">Number of of channels.</param>
* <result>error code</result>
*/
RTS_RESULT CDECL ServerGetMaxChannels(RTS_UI16 *pwMaxChannels);
typedef RTS_RESULT (CDECL * PFSERVERGETMAXCHANNELS) (RTS_UI16 *pwMaxChannels);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERGETMAXCHANNELS_NOTIMPLEMENTED)
	#define USE_ServerGetMaxChannels
	#define EXT_ServerGetMaxChannels
	#define GET_ServerGetMaxChannels(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerGetMaxChannels(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerGetMaxChannels  FALSE
	#define EXP_ServerGetMaxChannels  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerGetMaxChannels
	#define EXT_ServerGetMaxChannels
	#define GET_ServerGetMaxChannels(fl)  CAL_CMGETAPI( "ServerGetMaxChannels" ) 
	#define CAL_ServerGetMaxChannels  ServerGetMaxChannels
	#define CHK_ServerGetMaxChannels  TRUE
	#define EXP_ServerGetMaxChannels  CAL_CMEXPAPI( "ServerGetMaxChannels" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerGetMaxChannels
	#define EXT_ServerGetMaxChannels
	#define GET_ServerGetMaxChannels(fl)  CAL_CMGETAPI( "ServerGetMaxChannels" ) 
	#define CAL_ServerGetMaxChannels  ServerGetMaxChannels
	#define CHK_ServerGetMaxChannels  TRUE
	#define EXP_ServerGetMaxChannels  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGetMaxChannels", (RTS_UINTPTR)ServerGetMaxChannels, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerGetMaxChannels
	#define EXT_CmpSrvServerGetMaxChannels
	#define GET_CmpSrvServerGetMaxChannels  ERR_OK
	#define CAL_CmpSrvServerGetMaxChannels pICmpSrv->IServerGetMaxChannels
	#define CHK_CmpSrvServerGetMaxChannels (pICmpSrv != NULL)
	#define EXP_CmpSrvServerGetMaxChannels  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerGetMaxChannels
	#define EXT_ServerGetMaxChannels
	#define GET_ServerGetMaxChannels(fl)  CAL_CMGETAPI( "ServerGetMaxChannels" ) 
	#define CAL_ServerGetMaxChannels pICmpSrv->IServerGetMaxChannels
	#define CHK_ServerGetMaxChannels (pICmpSrv != NULL)
	#define EXP_ServerGetMaxChannels  CAL_CMEXPAPI( "ServerGetMaxChannels" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerGetMaxChannels  PFSERVERGETMAXCHANNELS pfServerGetMaxChannels;
	#define EXT_ServerGetMaxChannels  extern PFSERVERGETMAXCHANNELS pfServerGetMaxChannels;
	#define GET_ServerGetMaxChannels(fl)  s_pfCMGetAPI2( "ServerGetMaxChannels", (RTS_VOID_FCTPTR *)&pfServerGetMaxChannels, (fl), 0, 0)
	#define CAL_ServerGetMaxChannels  pfServerGetMaxChannels
	#define CHK_ServerGetMaxChannels  (pfServerGetMaxChannels != NULL)
	#define EXP_ServerGetMaxChannels  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGetMaxChannels", (RTS_UINTPTR)ServerGetMaxChannels, 0, 0) 
#endif




/**
* <description>
*   Retrieves general information for the specified server channel. This function is intended for information purpoeses only.
* </description>
* <param name="ui16ChannelIndex" type="IN">Index of the channel. Allowed range: 0..MaxChannels-1.</param>
* <param name="pui32ServerState" type="OUT">State of the server channel, see category "channel server state" for CSSTATE_ values in CmpCommunicationLibItf.</param>
* <param name="pChInfoBuffer" type="INOUT">Caller allocated buffer, which is filled by the CHANNELINFO structure. If the the state is CSSTATE_FREE, no structure is returned.</param>
* <param name="psiBufferLen" type="INOUT">Pointer to the size of the buffer in bytes, returns the number of copied bytes.</param>
* <result>error code</result>
*/
RTS_RESULT CDECL ServerGetChannelInfoByIndex(RTS_UI16 ui16ChannelIndex, RTS_UI32 *pui32ServerState, CHANNELINFO *pChInfoBuffer, RTS_SIZE *psiBufferLen);
typedef RTS_RESULT (CDECL * PFSERVERGETCHANNELINFOBYINDEX) (RTS_UI16 ui16ChannelIndex, RTS_UI32 *pui32ServerState, CHANNELINFO *pChInfoBuffer, RTS_SIZE *psiBufferLen);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERGETCHANNELINFOBYINDEX_NOTIMPLEMENTED)
	#define USE_ServerGetChannelInfoByIndex
	#define EXT_ServerGetChannelInfoByIndex
	#define GET_ServerGetChannelInfoByIndex(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerGetChannelInfoByIndex(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerGetChannelInfoByIndex  FALSE
	#define EXP_ServerGetChannelInfoByIndex  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerGetChannelInfoByIndex
	#define EXT_ServerGetChannelInfoByIndex
	#define GET_ServerGetChannelInfoByIndex(fl)  CAL_CMGETAPI( "ServerGetChannelInfoByIndex" ) 
	#define CAL_ServerGetChannelInfoByIndex  ServerGetChannelInfoByIndex
	#define CHK_ServerGetChannelInfoByIndex  TRUE
	#define EXP_ServerGetChannelInfoByIndex  CAL_CMEXPAPI( "ServerGetChannelInfoByIndex" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerGetChannelInfoByIndex
	#define EXT_ServerGetChannelInfoByIndex
	#define GET_ServerGetChannelInfoByIndex(fl)  CAL_CMGETAPI( "ServerGetChannelInfoByIndex" ) 
	#define CAL_ServerGetChannelInfoByIndex  ServerGetChannelInfoByIndex
	#define CHK_ServerGetChannelInfoByIndex  TRUE
	#define EXP_ServerGetChannelInfoByIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGetChannelInfoByIndex", (RTS_UINTPTR)ServerGetChannelInfoByIndex, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerGetChannelInfoByIndex
	#define EXT_CmpSrvServerGetChannelInfoByIndex
	#define GET_CmpSrvServerGetChannelInfoByIndex  ERR_OK
	#define CAL_CmpSrvServerGetChannelInfoByIndex pICmpSrv->IServerGetChannelInfoByIndex
	#define CHK_CmpSrvServerGetChannelInfoByIndex (pICmpSrv != NULL)
	#define EXP_CmpSrvServerGetChannelInfoByIndex  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerGetChannelInfoByIndex
	#define EXT_ServerGetChannelInfoByIndex
	#define GET_ServerGetChannelInfoByIndex(fl)  CAL_CMGETAPI( "ServerGetChannelInfoByIndex" ) 
	#define CAL_ServerGetChannelInfoByIndex pICmpSrv->IServerGetChannelInfoByIndex
	#define CHK_ServerGetChannelInfoByIndex (pICmpSrv != NULL)
	#define EXP_ServerGetChannelInfoByIndex  CAL_CMEXPAPI( "ServerGetChannelInfoByIndex" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerGetChannelInfoByIndex  PFSERVERGETCHANNELINFOBYINDEX pfServerGetChannelInfoByIndex;
	#define EXT_ServerGetChannelInfoByIndex  extern PFSERVERGETCHANNELINFOBYINDEX pfServerGetChannelInfoByIndex;
	#define GET_ServerGetChannelInfoByIndex(fl)  s_pfCMGetAPI2( "ServerGetChannelInfoByIndex", (RTS_VOID_FCTPTR *)&pfServerGetChannelInfoByIndex, (fl), 0, 0)
	#define CAL_ServerGetChannelInfoByIndex  pfServerGetChannelInfoByIndex
	#define CHK_ServerGetChannelInfoByIndex  (pfServerGetChannelInfoByIndex != NULL)
	#define EXP_ServerGetChannelInfoByIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGetChannelInfoByIndex", (RTS_UINTPTR)ServerGetChannelInfoByIndex, 0, 0) 
#endif




/**
*	<description>
*		Get the current status of an active channel.
*	</description>
*	<param name="wChannelHandle" type="IN">
*		Id of the channel
*	</param>
*	<param name="pusStatus" type="OUT">
*		Is set to the current progress state. The PROGRESS_xxx constants define valied values.
*	</param>
*  <param name="pbyScalingFactor" type="OUT">
*		Provides the scaling factor for pnItemsComplete and pnTotalItems. These values have been scaled
*		down by dividing them through 2^ScalingFactor
*		(i.e. they have been right shifted by ScalingFactor bits).
*  </param>
*	<param name="pnItemsComplete" type="OUT">
*		Number of items completed (eg. the number of bytes transfered).
*  </param>
* 	<param name="pnTotalItems" type="OUT">
*		Total number of item. Is set to -1 if unknown.
*  </param>
*  <result>error code</result>
*/
RTS_RESULT CDECL ServerGetStatus(RTS_UI32 ulChannelHandle, RTS_UI16 *pusStatus, RTS_UI8 *pbyScalingFactor, RTS_I32 *pnItemsComplete, RTS_I32 *pnTotalItems);
typedef RTS_RESULT (CDECL * PFSERVERGETSTATUS) (RTS_UI32 ulChannelHandle, RTS_UI16 *pusStatus, RTS_UI8 *pbyScalingFactor, RTS_I32 *pnItemsComplete, RTS_I32 *pnTotalItems);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERGETSTATUS_NOTIMPLEMENTED)
	#define USE_ServerGetStatus
	#define EXT_ServerGetStatus
	#define GET_ServerGetStatus(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerGetStatus(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerGetStatus  FALSE
	#define EXP_ServerGetStatus  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerGetStatus
	#define EXT_ServerGetStatus
	#define GET_ServerGetStatus(fl)  CAL_CMGETAPI( "ServerGetStatus" ) 
	#define CAL_ServerGetStatus  ServerGetStatus
	#define CHK_ServerGetStatus  TRUE
	#define EXP_ServerGetStatus  CAL_CMEXPAPI( "ServerGetStatus" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerGetStatus
	#define EXT_ServerGetStatus
	#define GET_ServerGetStatus(fl)  CAL_CMGETAPI( "ServerGetStatus" ) 
	#define CAL_ServerGetStatus  ServerGetStatus
	#define CHK_ServerGetStatus  TRUE
	#define EXP_ServerGetStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGetStatus", (RTS_UINTPTR)ServerGetStatus, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerGetStatus
	#define EXT_CmpSrvServerGetStatus
	#define GET_CmpSrvServerGetStatus  ERR_OK
	#define CAL_CmpSrvServerGetStatus pICmpSrv->IServerGetStatus
	#define CHK_CmpSrvServerGetStatus (pICmpSrv != NULL)
	#define EXP_CmpSrvServerGetStatus  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerGetStatus
	#define EXT_ServerGetStatus
	#define GET_ServerGetStatus(fl)  CAL_CMGETAPI( "ServerGetStatus" ) 
	#define CAL_ServerGetStatus pICmpSrv->IServerGetStatus
	#define CHK_ServerGetStatus (pICmpSrv != NULL)
	#define EXP_ServerGetStatus  CAL_CMEXPAPI( "ServerGetStatus" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerGetStatus  PFSERVERGETSTATUS pfServerGetStatus;
	#define EXT_ServerGetStatus  extern PFSERVERGETSTATUS pfServerGetStatus;
	#define GET_ServerGetStatus(fl)  s_pfCMGetAPI2( "ServerGetStatus", (RTS_VOID_FCTPTR *)&pfServerGetStatus, (fl), 0, 0)
	#define CAL_ServerGetStatus  pfServerGetStatus
	#define CHK_ServerGetStatus  (pfServerGetStatus != NULL)
	#define EXP_ServerGetStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerGetStatus", (RTS_UINTPTR)ServerGetStatus, 0, 0) 
#endif




/**
 * <description>Set redundancy mode to enable synchronous execution of services (called by CmpRedundancy)</description>
 * <param name="bRedundant" type="IN"></param>
 * <result>error code</result>
 */
RTS_RESULT CDECL ServerSetRedundancyMode(RTS_BOOL bRedundant);
typedef RTS_RESULT (CDECL * PFSERVERSETREDUNDANCYMODE) (RTS_BOOL bRedundant);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERSETREDUNDANCYMODE_NOTIMPLEMENTED)
	#define USE_ServerSetRedundancyMode
	#define EXT_ServerSetRedundancyMode
	#define GET_ServerSetRedundancyMode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerSetRedundancyMode(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerSetRedundancyMode  FALSE
	#define EXP_ServerSetRedundancyMode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerSetRedundancyMode
	#define EXT_ServerSetRedundancyMode
	#define GET_ServerSetRedundancyMode(fl)  CAL_CMGETAPI( "ServerSetRedundancyMode" ) 
	#define CAL_ServerSetRedundancyMode  ServerSetRedundancyMode
	#define CHK_ServerSetRedundancyMode  TRUE
	#define EXP_ServerSetRedundancyMode  CAL_CMEXPAPI( "ServerSetRedundancyMode" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerSetRedundancyMode
	#define EXT_ServerSetRedundancyMode
	#define GET_ServerSetRedundancyMode(fl)  CAL_CMGETAPI( "ServerSetRedundancyMode" ) 
	#define CAL_ServerSetRedundancyMode  ServerSetRedundancyMode
	#define CHK_ServerSetRedundancyMode  TRUE
	#define EXP_ServerSetRedundancyMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerSetRedundancyMode", (RTS_UINTPTR)ServerSetRedundancyMode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerSetRedundancyMode
	#define EXT_CmpSrvServerSetRedundancyMode
	#define GET_CmpSrvServerSetRedundancyMode  ERR_OK
	#define CAL_CmpSrvServerSetRedundancyMode pICmpSrv->IServerSetRedundancyMode
	#define CHK_CmpSrvServerSetRedundancyMode (pICmpSrv != NULL)
	#define EXP_CmpSrvServerSetRedundancyMode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerSetRedundancyMode
	#define EXT_ServerSetRedundancyMode
	#define GET_ServerSetRedundancyMode(fl)  CAL_CMGETAPI( "ServerSetRedundancyMode" ) 
	#define CAL_ServerSetRedundancyMode pICmpSrv->IServerSetRedundancyMode
	#define CHK_ServerSetRedundancyMode (pICmpSrv != NULL)
	#define EXP_ServerSetRedundancyMode  CAL_CMEXPAPI( "ServerSetRedundancyMode" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerSetRedundancyMode  PFSERVERSETREDUNDANCYMODE pfServerSetRedundancyMode;
	#define EXT_ServerSetRedundancyMode  extern PFSERVERSETREDUNDANCYMODE pfServerSetRedundancyMode;
	#define GET_ServerSetRedundancyMode(fl)  s_pfCMGetAPI2( "ServerSetRedundancyMode", (RTS_VOID_FCTPTR *)&pfServerSetRedundancyMode, (fl), 0, 0)
	#define CAL_ServerSetRedundancyMode  pfServerSetRedundancyMode
	#define CHK_ServerSetRedundancyMode  (pfServerSetRedundancyMode != NULL)
	#define EXP_ServerSetRedundancyMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerSetRedundancyMode", (RTS_UINTPTR)ServerSetRedundancyMode, 0, 0) 
#endif




/**
 * <description>Set redundancy mode standby to enable session id handling for standby controller (called by CmpRedundancy)</description>
 * <param name="bRedundanyStandby" type="IN"></param>
 * <result>error code</result>
 */
RTS_RESULT CDECL ServerSetRedundancyModeStandby(RTS_BOOL bRedundanyStandby);
typedef RTS_RESULT (CDECL * PFSERVERSETREDUNDANCYMODESTANDBY) (RTS_BOOL bRedundanyStandby);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERSETREDUNDANCYMODESTANDBY_NOTIMPLEMENTED)
	#define USE_ServerSetRedundancyModeStandby
	#define EXT_ServerSetRedundancyModeStandby
	#define GET_ServerSetRedundancyModeStandby(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerSetRedundancyModeStandby(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerSetRedundancyModeStandby  FALSE
	#define EXP_ServerSetRedundancyModeStandby  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerSetRedundancyModeStandby
	#define EXT_ServerSetRedundancyModeStandby
	#define GET_ServerSetRedundancyModeStandby(fl)  CAL_CMGETAPI( "ServerSetRedundancyModeStandby" ) 
	#define CAL_ServerSetRedundancyModeStandby  ServerSetRedundancyModeStandby
	#define CHK_ServerSetRedundancyModeStandby  TRUE
	#define EXP_ServerSetRedundancyModeStandby  CAL_CMEXPAPI( "ServerSetRedundancyModeStandby" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerSetRedundancyModeStandby
	#define EXT_ServerSetRedundancyModeStandby
	#define GET_ServerSetRedundancyModeStandby(fl)  CAL_CMGETAPI( "ServerSetRedundancyModeStandby" ) 
	#define CAL_ServerSetRedundancyModeStandby  ServerSetRedundancyModeStandby
	#define CHK_ServerSetRedundancyModeStandby  TRUE
	#define EXP_ServerSetRedundancyModeStandby  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerSetRedundancyModeStandby", (RTS_UINTPTR)ServerSetRedundancyModeStandby, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerSetRedundancyModeStandby
	#define EXT_CmpSrvServerSetRedundancyModeStandby
	#define GET_CmpSrvServerSetRedundancyModeStandby  ERR_OK
	#define CAL_CmpSrvServerSetRedundancyModeStandby pICmpSrv->IServerSetRedundancyModeStandby
	#define CHK_CmpSrvServerSetRedundancyModeStandby (pICmpSrv != NULL)
	#define EXP_CmpSrvServerSetRedundancyModeStandby  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerSetRedundancyModeStandby
	#define EXT_ServerSetRedundancyModeStandby
	#define GET_ServerSetRedundancyModeStandby(fl)  CAL_CMGETAPI( "ServerSetRedundancyModeStandby" ) 
	#define CAL_ServerSetRedundancyModeStandby pICmpSrv->IServerSetRedundancyModeStandby
	#define CHK_ServerSetRedundancyModeStandby (pICmpSrv != NULL)
	#define EXP_ServerSetRedundancyModeStandby  CAL_CMEXPAPI( "ServerSetRedundancyModeStandby" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerSetRedundancyModeStandby  PFSERVERSETREDUNDANCYMODESTANDBY pfServerSetRedundancyModeStandby;
	#define EXT_ServerSetRedundancyModeStandby  extern PFSERVERSETREDUNDANCYMODESTANDBY pfServerSetRedundancyModeStandby;
	#define GET_ServerSetRedundancyModeStandby(fl)  s_pfCMGetAPI2( "ServerSetRedundancyModeStandby", (RTS_VOID_FCTPTR *)&pfServerSetRedundancyModeStandby, (fl), 0, 0)
	#define CAL_ServerSetRedundancyModeStandby  pfServerSetRedundancyModeStandby
	#define CHK_ServerSetRedundancyModeStandby  (pfServerSetRedundancyModeStandby != NULL)
	#define EXP_ServerSetRedundancyModeStandby  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerSetRedundancyModeStandby", (RTS_UINTPTR)ServerSetRedundancyModeStandby, 0, 0) 
#endif




/**
 * <description>Set flag to execute online service (called by CmpRedundancy)</description>
 * <result>error code</result>
 */
RTS_RESULT CDECL ServerExecuteOnlineService(void);
typedef RTS_RESULT (CDECL * PFSERVEREXECUTEONLINESERVICE) (void);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVEREXECUTEONLINESERVICE_NOTIMPLEMENTED)
	#define USE_ServerExecuteOnlineService
	#define EXT_ServerExecuteOnlineService
	#define GET_ServerExecuteOnlineService(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerExecuteOnlineService()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerExecuteOnlineService  FALSE
	#define EXP_ServerExecuteOnlineService  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerExecuteOnlineService
	#define EXT_ServerExecuteOnlineService
	#define GET_ServerExecuteOnlineService(fl)  CAL_CMGETAPI( "ServerExecuteOnlineService" ) 
	#define CAL_ServerExecuteOnlineService  ServerExecuteOnlineService
	#define CHK_ServerExecuteOnlineService  TRUE
	#define EXP_ServerExecuteOnlineService  CAL_CMEXPAPI( "ServerExecuteOnlineService" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerExecuteOnlineService
	#define EXT_ServerExecuteOnlineService
	#define GET_ServerExecuteOnlineService(fl)  CAL_CMGETAPI( "ServerExecuteOnlineService" ) 
	#define CAL_ServerExecuteOnlineService  ServerExecuteOnlineService
	#define CHK_ServerExecuteOnlineService  TRUE
	#define EXP_ServerExecuteOnlineService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerExecuteOnlineService", (RTS_UINTPTR)ServerExecuteOnlineService, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerExecuteOnlineService
	#define EXT_CmpSrvServerExecuteOnlineService
	#define GET_CmpSrvServerExecuteOnlineService  ERR_OK
	#define CAL_CmpSrvServerExecuteOnlineService pICmpSrv->IServerExecuteOnlineService
	#define CHK_CmpSrvServerExecuteOnlineService (pICmpSrv != NULL)
	#define EXP_CmpSrvServerExecuteOnlineService  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerExecuteOnlineService
	#define EXT_ServerExecuteOnlineService
	#define GET_ServerExecuteOnlineService(fl)  CAL_CMGETAPI( "ServerExecuteOnlineService" ) 
	#define CAL_ServerExecuteOnlineService pICmpSrv->IServerExecuteOnlineService
	#define CHK_ServerExecuteOnlineService (pICmpSrv != NULL)
	#define EXP_ServerExecuteOnlineService  CAL_CMEXPAPI( "ServerExecuteOnlineService" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerExecuteOnlineService  PFSERVEREXECUTEONLINESERVICE pfServerExecuteOnlineService;
	#define EXT_ServerExecuteOnlineService  extern PFSERVEREXECUTEONLINESERVICE pfServerExecuteOnlineService;
	#define GET_ServerExecuteOnlineService(fl)  s_pfCMGetAPI2( "ServerExecuteOnlineService", (RTS_VOID_FCTPTR *)&pfServerExecuteOnlineService, (fl), 0, 0)
	#define CAL_ServerExecuteOnlineService  pfServerExecuteOnlineService
	#define CHK_ServerExecuteOnlineService  (pfServerExecuteOnlineService != NULL)
	#define EXP_ServerExecuteOnlineService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerExecuteOnlineService", (RTS_UINTPTR)ServerExecuteOnlineService, 0, 0) 
#endif




/**
 * <description>Handle one request (called by CmpRedundancy)</description> 
 * <param name="ulChannelId" type="IN">Id of the channel on which the request arrived</param>
 * <param name="pduRequest" type="IN">Pointer to the request</param>
 * <param name="pduReply" type="OUT">Pointer to the request reply</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL ServerHandleRequest(RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply);
typedef RTS_RESULT (CDECL * PFSERVERHANDLEREQUEST) (RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SERVERHANDLEREQUEST_NOTIMPLEMENTED)
	#define USE_ServerHandleRequest
	#define EXT_ServerHandleRequest
	#define GET_ServerHandleRequest(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ServerHandleRequest(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ServerHandleRequest  FALSE
	#define EXP_ServerHandleRequest  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ServerHandleRequest
	#define EXT_ServerHandleRequest
	#define GET_ServerHandleRequest(fl)  CAL_CMGETAPI( "ServerHandleRequest" ) 
	#define CAL_ServerHandleRequest  ServerHandleRequest
	#define CHK_ServerHandleRequest  TRUE
	#define EXP_ServerHandleRequest  CAL_CMEXPAPI( "ServerHandleRequest" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_ServerHandleRequest
	#define EXT_ServerHandleRequest
	#define GET_ServerHandleRequest(fl)  CAL_CMGETAPI( "ServerHandleRequest" ) 
	#define CAL_ServerHandleRequest  ServerHandleRequest
	#define CHK_ServerHandleRequest  TRUE
	#define EXP_ServerHandleRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerHandleRequest", (RTS_UINTPTR)ServerHandleRequest, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvServerHandleRequest
	#define EXT_CmpSrvServerHandleRequest
	#define GET_CmpSrvServerHandleRequest  ERR_OK
	#define CAL_CmpSrvServerHandleRequest pICmpSrv->IServerHandleRequest
	#define CHK_CmpSrvServerHandleRequest (pICmpSrv != NULL)
	#define EXP_CmpSrvServerHandleRequest  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ServerHandleRequest
	#define EXT_ServerHandleRequest
	#define GET_ServerHandleRequest(fl)  CAL_CMGETAPI( "ServerHandleRequest" ) 
	#define CAL_ServerHandleRequest pICmpSrv->IServerHandleRequest
	#define CHK_ServerHandleRequest (pICmpSrv != NULL)
	#define EXP_ServerHandleRequest  CAL_CMEXPAPI( "ServerHandleRequest" ) 
#else /* DYNAMIC_LINK */
	#define USE_ServerHandleRequest  PFSERVERHANDLEREQUEST pfServerHandleRequest;
	#define EXT_ServerHandleRequest  extern PFSERVERHANDLEREQUEST pfServerHandleRequest;
	#define GET_ServerHandleRequest(fl)  s_pfCMGetAPI2( "ServerHandleRequest", (RTS_VOID_FCTPTR *)&pfServerHandleRequest, (fl), 0, 0)
	#define CAL_ServerHandleRequest  pfServerHandleRequest
	#define CHK_ServerHandleRequest  (pfServerHandleRequest != NULL)
	#define EXP_ServerHandleRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ServerHandleRequest", (RTS_UINTPTR)ServerHandleRequest, 0, 0) 
#endif




/**
 * <description>Get the last log entry of class LOG_USER_NOTIFY as a toplevel online service tag</description>
 * <param name="pWriter" type="IN">Pointer to the bintag writer to get the service tag</param>
 * <param name="pduSendBuffer" type="IN">Pointer to the send buffer to reset the content of the bintag writer</param>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_OK: There is still an unread log entry of the type LOG_USER_NOTIFY</li>
 *		<li>ERR_NO_OBJECT: No pending log entry of the type LOG_USER_NOTIFY</li>
 *		<li>ERR_NOT_SUPPORTED: Service not supported</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL SrvGetUserNotificationService(BINTAGWRITER *pWriter, PROTOCOL_DATA_UNIT *pduSendBuffer);
typedef RTS_RESULT (CDECL * PFSRVGETUSERNOTIFICATIONSERVICE) (BINTAGWRITER *pWriter, PROTOCOL_DATA_UNIT *pduSendBuffer);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SRVGETUSERNOTIFICATIONSERVICE_NOTIMPLEMENTED)
	#define USE_SrvGetUserNotificationService
	#define EXT_SrvGetUserNotificationService
	#define GET_SrvGetUserNotificationService(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SrvGetUserNotificationService(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SrvGetUserNotificationService  FALSE
	#define EXP_SrvGetUserNotificationService  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SrvGetUserNotificationService
	#define EXT_SrvGetUserNotificationService
	#define GET_SrvGetUserNotificationService(fl)  CAL_CMGETAPI( "SrvGetUserNotificationService" ) 
	#define CAL_SrvGetUserNotificationService  SrvGetUserNotificationService
	#define CHK_SrvGetUserNotificationService  TRUE
	#define EXP_SrvGetUserNotificationService  CAL_CMEXPAPI( "SrvGetUserNotificationService" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_SrvGetUserNotificationService
	#define EXT_SrvGetUserNotificationService
	#define GET_SrvGetUserNotificationService(fl)  CAL_CMGETAPI( "SrvGetUserNotificationService" ) 
	#define CAL_SrvGetUserNotificationService  SrvGetUserNotificationService
	#define CHK_SrvGetUserNotificationService  TRUE
	#define EXP_SrvGetUserNotificationService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SrvGetUserNotificationService", (RTS_UINTPTR)SrvGetUserNotificationService, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvSrvGetUserNotificationService
	#define EXT_CmpSrvSrvGetUserNotificationService
	#define GET_CmpSrvSrvGetUserNotificationService  ERR_OK
	#define CAL_CmpSrvSrvGetUserNotificationService pICmpSrv->ISrvGetUserNotificationService
	#define CHK_CmpSrvSrvGetUserNotificationService (pICmpSrv != NULL)
	#define EXP_CmpSrvSrvGetUserNotificationService  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SrvGetUserNotificationService
	#define EXT_SrvGetUserNotificationService
	#define GET_SrvGetUserNotificationService(fl)  CAL_CMGETAPI( "SrvGetUserNotificationService" ) 
	#define CAL_SrvGetUserNotificationService pICmpSrv->ISrvGetUserNotificationService
	#define CHK_SrvGetUserNotificationService (pICmpSrv != NULL)
	#define EXP_SrvGetUserNotificationService  CAL_CMEXPAPI( "SrvGetUserNotificationService" ) 
#else /* DYNAMIC_LINK */
	#define USE_SrvGetUserNotificationService  PFSRVGETUSERNOTIFICATIONSERVICE pfSrvGetUserNotificationService;
	#define EXT_SrvGetUserNotificationService  extern PFSRVGETUSERNOTIFICATIONSERVICE pfSrvGetUserNotificationService;
	#define GET_SrvGetUserNotificationService(fl)  s_pfCMGetAPI2( "SrvGetUserNotificationService", (RTS_VOID_FCTPTR *)&pfSrvGetUserNotificationService, (fl), 0, 0)
	#define CAL_SrvGetUserNotificationService  pfSrvGetUserNotificationService
	#define CHK_SrvGetUserNotificationService  (pfSrvGetUserNotificationService != NULL)
	#define EXP_SrvGetUserNotificationService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SrvGetUserNotificationService", (RTS_UINTPTR)SrvGetUserNotificationService, 0, 0) 
#endif




/**
 * <description>Get the last log entry of class LOG_USER_NOTIFY as a toplevel online service tag</description>
 * <param name="pWriter" type="IN">Pointer to the bintag writer to get the service tag</param>
 * <param name="pduSendBuffer" type="IN">Pointer to the send buffer to reset the content of the bintag writer</param>
 * <param name="ulTagId" type="IN">TagId to send user notify info</param>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_OK: There is still an unread log entry of the type LOG_USER_NOTIFY</li>
 *		<li>ERR_NO_OBJECT: No pending log entry of the type LOG_USER_NOTIFY</li>
 *		<li>ERR_NOT_SUPPORTED: Service not supported</li>
 *	</ul>
 * </result>cmpsrv
 */
RTS_RESULT CDECL SrvGetUserNotificationService2(BINTAGWRITER *pWriter, PROTOCOL_DATA_UNIT *pduSendBuffer, unsigned long ulTagId);
typedef RTS_RESULT (CDECL * PFSRVGETUSERNOTIFICATIONSERVICE2) (BINTAGWRITER *pWriter, PROTOCOL_DATA_UNIT *pduSendBuffer, unsigned long ulTagId);
#if defined(CMPSRV_NOTIMPLEMENTED) || defined(SRVGETUSERNOTIFICATIONSERVICE2_NOTIMPLEMENTED)
	#define USE_SrvGetUserNotificationService2
	#define EXT_SrvGetUserNotificationService2
	#define GET_SrvGetUserNotificationService2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SrvGetUserNotificationService2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SrvGetUserNotificationService2  FALSE
	#define EXP_SrvGetUserNotificationService2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SrvGetUserNotificationService2
	#define EXT_SrvGetUserNotificationService2
	#define GET_SrvGetUserNotificationService2(fl)  CAL_CMGETAPI( "SrvGetUserNotificationService2" ) 
	#define CAL_SrvGetUserNotificationService2  SrvGetUserNotificationService2
	#define CHK_SrvGetUserNotificationService2  TRUE
	#define EXP_SrvGetUserNotificationService2  CAL_CMEXPAPI( "SrvGetUserNotificationService2" ) 
#elif defined(MIXED_LINK) && !defined(CMPSRV_EXTERNAL)
	#define USE_SrvGetUserNotificationService2
	#define EXT_SrvGetUserNotificationService2
	#define GET_SrvGetUserNotificationService2(fl)  CAL_CMGETAPI( "SrvGetUserNotificationService2" ) 
	#define CAL_SrvGetUserNotificationService2  SrvGetUserNotificationService2
	#define CHK_SrvGetUserNotificationService2  TRUE
	#define EXP_SrvGetUserNotificationService2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SrvGetUserNotificationService2", (RTS_UINTPTR)SrvGetUserNotificationService2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSrvSrvGetUserNotificationService2
	#define EXT_CmpSrvSrvGetUserNotificationService2
	#define GET_CmpSrvSrvGetUserNotificationService2  ERR_OK
	#define CAL_CmpSrvSrvGetUserNotificationService2 pICmpSrv->ISrvGetUserNotificationService2
	#define CHK_CmpSrvSrvGetUserNotificationService2 (pICmpSrv != NULL)
	#define EXP_CmpSrvSrvGetUserNotificationService2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SrvGetUserNotificationService2
	#define EXT_SrvGetUserNotificationService2
	#define GET_SrvGetUserNotificationService2(fl)  CAL_CMGETAPI( "SrvGetUserNotificationService2" ) 
	#define CAL_SrvGetUserNotificationService2 pICmpSrv->ISrvGetUserNotificationService2
	#define CHK_SrvGetUserNotificationService2 (pICmpSrv != NULL)
	#define EXP_SrvGetUserNotificationService2  CAL_CMEXPAPI( "SrvGetUserNotificationService2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SrvGetUserNotificationService2  PFSRVGETUSERNOTIFICATIONSERVICE2 pfSrvGetUserNotificationService2;
	#define EXT_SrvGetUserNotificationService2  extern PFSRVGETUSERNOTIFICATIONSERVICE2 pfSrvGetUserNotificationService2;
	#define GET_SrvGetUserNotificationService2(fl)  s_pfCMGetAPI2( "SrvGetUserNotificationService2", (RTS_VOID_FCTPTR *)&pfSrvGetUserNotificationService2, (fl), 0, 0)
	#define CAL_SrvGetUserNotificationService2  pfSrvGetUserNotificationService2
	#define CHK_SrvGetUserNotificationService2  (pfSrvGetUserNotificationService2 != NULL)
	#define EXP_SrvGetUserNotificationService2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SrvGetUserNotificationService2", (RTS_UINTPTR)SrvGetUserNotificationService2, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSERVERAPPHANDLEREQUEST IServerAppHandleRequest;
 	PFSERVERAPPHANDLEREQUEST2 IServerAppHandleRequest2;
 	PFSERVERAPPHANDLEREQUEST3 IServerAppHandleRequest3;
 	PFSERVERREGISTERSERVICEHANDLER IServerRegisterServiceHandler;
 	PFSERVERREGISTERSERVICEHANDLER2 IServerRegisterServiceHandler2;
 	PFSERVERREGISTERSERVICEHANDLER3 IServerRegisterServiceHandler3;
 	PFSERVERUNREGISTERSERVICEHANDLER IServerUnRegisterServiceHandler;
 	PFSERVERREGISTERPROTOCOLHANDLER IServerRegisterProtocolHandler;
 	PFSERVERREGISTERPROTOCOLHANDLER2 IServerRegisterProtocolHandler2;
 	PFSERVERUNREGISTERPROTOCOLHANDLER IServerUnRegisterProtocolHandler;
 	PFSERVERFINISHREQUEST IServerFinishRequest;
 	PFSERVERGENERATESESSIONID IServerGenerateSessionId;
 	PFSERVERSETSESSIONID IServerSetSessionId;
 	PFSERVERGETSESSIONID IServerGetSessionId;
 	PFSERVERGETMAXCHANNELS IServerGetMaxChannels;
 	PFSERVERGETCHANNELINFOBYINDEX IServerGetChannelInfoByIndex;
 	PFSERVERGETSTATUS IServerGetStatus;
 	PFSERVERSETREDUNDANCYMODE IServerSetRedundancyMode;
 	PFSERVERSETREDUNDANCYMODESTANDBY IServerSetRedundancyModeStandby;
 	PFSERVEREXECUTEONLINESERVICE IServerExecuteOnlineService;
 	PFSERVERHANDLEREQUEST IServerHandleRequest;
 	PFSRVGETUSERNOTIFICATIONSERVICE ISrvGetUserNotificationService;
 	PFSRVGETUSERNOTIFICATIONSERVICE2 ISrvGetUserNotificationService2;
 } ICmpSrv_C;

#ifdef CPLUSPLUS
class ICmpSrv : public IBase
{
	public:
		virtual RTS_RESULT CDECL IServerAppHandleRequest(RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply) =0;
		virtual RTS_RESULT CDECL IServerAppHandleRequest2(RTS_HANDLE hRouter, RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply) =0;
		virtual RTS_RESULT CDECL IServerAppHandleRequest3(RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply, RTS_UI32 bFirstCall) =0;
		virtual RTS_RESULT CDECL IServerRegisterServiceHandler(RTS_UI32 ulServiceGroup, PFServiceHandler pfServiceHandler) =0;
		virtual RTS_RESULT CDECL IServerRegisterServiceHandler2(RTS_UI32 ulServiceGroup, PFServiceHandler pfServiceHandler, char *pszRouter) =0;
		virtual RTS_RESULT CDECL IServerRegisterServiceHandler3(RTS_UI32 ulServiceGroup, PFServiceHandler2 pfServiceHandler2) =0;
		virtual RTS_RESULT CDECL IServerUnRegisterServiceHandler(RTS_UI32 ulServiceGroup, PFServiceHandler pfServiceHandler) =0;
		virtual RTS_RESULT CDECL IServerRegisterProtocolHandler(RTS_UI16 usProtocolId, PFServiceHandler pfServiceHandler) =0;
		virtual RTS_RESULT CDECL IServerRegisterProtocolHandler2(RTS_UI16 usProtocolId, PFServiceHandler pfServiceHandler, char *pszRouter) =0;
		virtual RTS_RESULT CDECL IServerUnRegisterProtocolHandler(RTS_UI16 usProtocolId, PFServiceHandler pfServiceHandler) =0;
		virtual RTS_RESULT CDECL IServerFinishRequest(RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduData) =0;
		virtual RTS_RESULT CDECL IServerGenerateSessionId(RTS_UI32 *pulSessionId) =0;
		virtual RTS_RESULT CDECL IServerSetSessionId(RTS_UI32 ulChannelHandle, RTS_UI32 ulSessionId) =0;
		virtual RTS_RESULT CDECL IServerGetSessionId(RTS_UI32 ulChannelHandle, RTS_UI32 *pulSessionId) =0;
		virtual RTS_RESULT CDECL IServerGetMaxChannels(RTS_UI16 *pwMaxChannels) =0;
		virtual RTS_RESULT CDECL IServerGetChannelInfoByIndex(RTS_UI16 ui16ChannelIndex, RTS_UI32 *pui32ServerState, CHANNELINFO *pChInfoBuffer, RTS_SIZE *psiBufferLen) =0;
		virtual RTS_RESULT CDECL IServerGetStatus(RTS_UI32 ulChannelHandle, RTS_UI16 *pusStatus, RTS_UI8 *pbyScalingFactor, RTS_I32 *pnItemsComplete, RTS_I32 *pnTotalItems) =0;
		virtual RTS_RESULT CDECL IServerSetRedundancyMode(RTS_BOOL bRedundant) =0;
		virtual RTS_RESULT CDECL IServerSetRedundancyModeStandby(RTS_BOOL bRedundanyStandby) =0;
		virtual RTS_RESULT CDECL IServerExecuteOnlineService(void) =0;
		virtual RTS_RESULT CDECL IServerHandleRequest(RTS_UI32 ulChannelId, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply) =0;
		virtual RTS_RESULT CDECL ISrvGetUserNotificationService(BINTAGWRITER *pWriter, PROTOCOL_DATA_UNIT *pduSendBuffer) =0;
		virtual RTS_RESULT CDECL ISrvGetUserNotificationService2(BINTAGWRITER *pWriter, PROTOCOL_DATA_UNIT *pduSendBuffer, unsigned long ulTagId) =0;
};
	#ifndef ITF_CmpSrv
		#define ITF_CmpSrv static ICmpSrv *pICmpSrv = NULL;
	#endif
	#define EXTITF_CmpSrv
#else	/*CPLUSPLUS*/
	typedef ICmpSrv_C		ICmpSrv;
	#ifndef ITF_CmpSrv
		#define ITF_CmpSrv
	#endif
	#define EXTITF_CmpSrv
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPSRVITF_H_*/
