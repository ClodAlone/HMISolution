 /**
 * <interfacename>CmpChannelClient</interfacename>
 * <description>
 *	Interface for the channel client.
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPCHANNELCLIENTITF_H_
#define _CMPCHANNELCLIENTITF_H_

#include "CmpStd.h"

 

 




#include "CmpChannelMgrItf.h"
#include "CmpChannelClientAppItf.h"

#ifdef __EXTERNAL_DECL
#	undef __EXTERNAL_DECL
#endif

#ifdef CM_EXTERNAL_USE_DEFINITION
#	define __EXTERNAL_DECL extern
#else
#	define __EXTERNAL_DECL
#endif


/* -- Functions exported to the application -- */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 *   Request a new channel to node addrReceiver . This function is executed asynchronously.
 *   To get the result of this call use NetClientOpenChannelResult.
 * </description>
 * <param name="addrReceiver" type="IN">  Open a channel to this node </param>
 * <param name="dwCommBufferSize" type="IN">  Requested size of the communication buffer </param>
 * <param name="pdwReqId" type="OUT"> 
 *   Set to a unique request id. Use this id in NetClientOpenChannelResult to 
 *   determine the result of this call.
 * </param>
 * <result>
 *   <ul>
 *     <li>ERR_OK, if the channel has been established</li>
 *     <li>ERR_PENDING, if the function hasn't completed yet. </li>
 *     <li>ERR_NOBUFFER, if we're low on memory.</li>
 *     <li>ERR_CHC_NUMCHANNELS, 
 *         if buffers for an additional channel are not available. Possibly a smaller 
 *		   communication buffer could work. Also, this error condition might be 
 *         temporarily and could change if memory is freed elsewhere.
 *     </li>
 *   </ul>
 * </result>
*/
int CDECL NetClientOpenChannel(PEERADDRESS addrReceiver, RTS_UI32 dwCommBufferSize, RTS_UI32 *pdwReqId);
typedef int (CDECL * PFNETCLIENTOPENCHANNEL) (PEERADDRESS addrReceiver, RTS_UI32 dwCommBufferSize, RTS_UI32 *pdwReqId);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTOPENCHANNEL_NOTIMPLEMENTED)
	#define USE_NetClientOpenChannel
	#define EXT_NetClientOpenChannel
	#define GET_NetClientOpenChannel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientOpenChannel(p0,p1,p2)  (int)ERR_NOTIMPLEMENTED
	#define CHK_NetClientOpenChannel  FALSE
	#define EXP_NetClientOpenChannel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientOpenChannel
	#define EXT_NetClientOpenChannel
	#define GET_NetClientOpenChannel(fl)  CAL_CMGETAPI( "NetClientOpenChannel" ) 
	#define CAL_NetClientOpenChannel  NetClientOpenChannel
	#define CHK_NetClientOpenChannel  TRUE
	#define EXP_NetClientOpenChannel  CAL_CMEXPAPI( "NetClientOpenChannel" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientOpenChannel
	#define EXT_NetClientOpenChannel
	#define GET_NetClientOpenChannel(fl)  CAL_CMGETAPI( "NetClientOpenChannel" ) 
	#define CAL_NetClientOpenChannel  NetClientOpenChannel
	#define CHK_NetClientOpenChannel  TRUE
	#define EXP_NetClientOpenChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientOpenChannel", (RTS_UINTPTR)NetClientOpenChannel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientOpenChannel
	#define EXT_CmpChannelClientNetClientOpenChannel
	#define GET_CmpChannelClientNetClientOpenChannel  ERR_OK
	#define CAL_CmpChannelClientNetClientOpenChannel pICmpChannelClient->INetClientOpenChannel
	#define CHK_CmpChannelClientNetClientOpenChannel (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientOpenChannel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientOpenChannel
	#define EXT_NetClientOpenChannel
	#define GET_NetClientOpenChannel(fl)  CAL_CMGETAPI( "NetClientOpenChannel" ) 
	#define CAL_NetClientOpenChannel pICmpChannelClient->INetClientOpenChannel
	#define CHK_NetClientOpenChannel (pICmpChannelClient != NULL)
	#define EXP_NetClientOpenChannel  CAL_CMEXPAPI( "NetClientOpenChannel" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientOpenChannel  PFNETCLIENTOPENCHANNEL pfNetClientOpenChannel;
	#define EXT_NetClientOpenChannel  extern PFNETCLIENTOPENCHANNEL pfNetClientOpenChannel;
	#define GET_NetClientOpenChannel(fl)  s_pfCMGetAPI2( "NetClientOpenChannel", (RTS_VOID_FCTPTR *)&pfNetClientOpenChannel, (fl), 0, 0)
	#define CAL_NetClientOpenChannel  pfNetClientOpenChannel
	#define CHK_NetClientOpenChannel  (pfNetClientOpenChannel != NULL)
	#define EXP_NetClientOpenChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientOpenChannel", (RTS_UINTPTR)NetClientOpenChannel, 0, 0) 
#endif




/** 
 * <description>
 *   Get the result of a NetClientOpenChannel request. 
 * </description>
 *
 * <param name="dwRequestId" type="IN">  The request id assigned by the NetClientOpenChannel request.</param>
 * <param name="pwChannelHandle" type="OUT"> Identifies the newly created channel.</param>
 * <param name="pdwCommBufferSize" type="OUT"> The communication buffer used for this channel.</param>
 * <param name="pbBigEndianByteOrder" type="OUT">
 *		True if the target system uses big endian byte order. Since that is typically used by motorola processors 
 *      (PowerPC etc.), it sometimes is also  referred to as "Motorola Byteorder" 
 * </param>
 *
 * <result>
 *  ERR_OK,      channel has been established
 * 	ERR_PENDING, is still in progress
 *	ERR_FAILED,  channel could not be created
 * </result>
*/
int CDECL NetClientOpenChannelResult(RTS_UI32 dwRequestId, unsigned short *pwChannelHandle, RTS_UI32 *pdwCommBufferSize, RTS_BOOL *pbBigEndianByteOrder);
typedef int (CDECL * PFNETCLIENTOPENCHANNELRESULT) (RTS_UI32 dwRequestId, unsigned short *pwChannelHandle, RTS_UI32 *pdwCommBufferSize, RTS_BOOL *pbBigEndianByteOrder);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTOPENCHANNELRESULT_NOTIMPLEMENTED)
	#define USE_NetClientOpenChannelResult
	#define EXT_NetClientOpenChannelResult
	#define GET_NetClientOpenChannelResult(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientOpenChannelResult(p0,p1,p2,p3)  (int)ERR_NOTIMPLEMENTED
	#define CHK_NetClientOpenChannelResult  FALSE
	#define EXP_NetClientOpenChannelResult  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientOpenChannelResult
	#define EXT_NetClientOpenChannelResult
	#define GET_NetClientOpenChannelResult(fl)  CAL_CMGETAPI( "NetClientOpenChannelResult" ) 
	#define CAL_NetClientOpenChannelResult  NetClientOpenChannelResult
	#define CHK_NetClientOpenChannelResult  TRUE
	#define EXP_NetClientOpenChannelResult  CAL_CMEXPAPI( "NetClientOpenChannelResult" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientOpenChannelResult
	#define EXT_NetClientOpenChannelResult
	#define GET_NetClientOpenChannelResult(fl)  CAL_CMGETAPI( "NetClientOpenChannelResult" ) 
	#define CAL_NetClientOpenChannelResult  NetClientOpenChannelResult
	#define CHK_NetClientOpenChannelResult  TRUE
	#define EXP_NetClientOpenChannelResult  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientOpenChannelResult", (RTS_UINTPTR)NetClientOpenChannelResult, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientOpenChannelResult
	#define EXT_CmpChannelClientNetClientOpenChannelResult
	#define GET_CmpChannelClientNetClientOpenChannelResult  ERR_OK
	#define CAL_CmpChannelClientNetClientOpenChannelResult pICmpChannelClient->INetClientOpenChannelResult
	#define CHK_CmpChannelClientNetClientOpenChannelResult (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientOpenChannelResult  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientOpenChannelResult
	#define EXT_NetClientOpenChannelResult
	#define GET_NetClientOpenChannelResult(fl)  CAL_CMGETAPI( "NetClientOpenChannelResult" ) 
	#define CAL_NetClientOpenChannelResult pICmpChannelClient->INetClientOpenChannelResult
	#define CHK_NetClientOpenChannelResult (pICmpChannelClient != NULL)
	#define EXP_NetClientOpenChannelResult  CAL_CMEXPAPI( "NetClientOpenChannelResult" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientOpenChannelResult  PFNETCLIENTOPENCHANNELRESULT pfNetClientOpenChannelResult;
	#define EXT_NetClientOpenChannelResult  extern PFNETCLIENTOPENCHANNELRESULT pfNetClientOpenChannelResult;
	#define GET_NetClientOpenChannelResult(fl)  s_pfCMGetAPI2( "NetClientOpenChannelResult", (RTS_VOID_FCTPTR *)&pfNetClientOpenChannelResult, (fl), 0, 0)
	#define CAL_NetClientOpenChannelResult  pfNetClientOpenChannelResult
	#define CHK_NetClientOpenChannelResult  (pfNetClientOpenChannelResult != NULL)
	#define EXP_NetClientOpenChannelResult  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientOpenChannelResult", (RTS_UINTPTR)NetClientOpenChannelResult, 0, 0) 
#endif




/**
 * <description>
 *   Close a channel. This function is executed synchronously 
 * </description>
 * <param name="wChannelHandle" type="IN"> Handle to the channel </param>
 * <result>
 *   An error code, if the handle could not be closed.
 * </result>
*/
int CDECL NetClientCloseChannel(unsigned short wChannelHandle);
typedef int (CDECL * PFNETCLIENTCLOSECHANNEL) (unsigned short wChannelHandle);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTCLOSECHANNEL_NOTIMPLEMENTED)
	#define USE_NetClientCloseChannel
	#define EXT_NetClientCloseChannel
	#define GET_NetClientCloseChannel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientCloseChannel(p0)  (int)ERR_NOTIMPLEMENTED
	#define CHK_NetClientCloseChannel  FALSE
	#define EXP_NetClientCloseChannel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientCloseChannel
	#define EXT_NetClientCloseChannel
	#define GET_NetClientCloseChannel(fl)  CAL_CMGETAPI( "NetClientCloseChannel" ) 
	#define CAL_NetClientCloseChannel  NetClientCloseChannel
	#define CHK_NetClientCloseChannel  TRUE
	#define EXP_NetClientCloseChannel  CAL_CMEXPAPI( "NetClientCloseChannel" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientCloseChannel
	#define EXT_NetClientCloseChannel
	#define GET_NetClientCloseChannel(fl)  CAL_CMGETAPI( "NetClientCloseChannel" ) 
	#define CAL_NetClientCloseChannel  NetClientCloseChannel
	#define CHK_NetClientCloseChannel  TRUE
	#define EXP_NetClientCloseChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientCloseChannel", (RTS_UINTPTR)NetClientCloseChannel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientCloseChannel
	#define EXT_CmpChannelClientNetClientCloseChannel
	#define GET_CmpChannelClientNetClientCloseChannel  ERR_OK
	#define CAL_CmpChannelClientNetClientCloseChannel pICmpChannelClient->INetClientCloseChannel
	#define CHK_CmpChannelClientNetClientCloseChannel (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientCloseChannel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientCloseChannel
	#define EXT_NetClientCloseChannel
	#define GET_NetClientCloseChannel(fl)  CAL_CMGETAPI( "NetClientCloseChannel" ) 
	#define CAL_NetClientCloseChannel pICmpChannelClient->INetClientCloseChannel
	#define CHK_NetClientCloseChannel (pICmpChannelClient != NULL)
	#define EXP_NetClientCloseChannel  CAL_CMEXPAPI( "NetClientCloseChannel" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientCloseChannel  PFNETCLIENTCLOSECHANNEL pfNetClientCloseChannel;
	#define EXT_NetClientCloseChannel  extern PFNETCLIENTCLOSECHANNEL pfNetClientCloseChannel;
	#define GET_NetClientCloseChannel(fl)  s_pfCMGetAPI2( "NetClientCloseChannel", (RTS_VOID_FCTPTR *)&pfNetClientCloseChannel, (fl), 0, 0)
	#define CAL_NetClientCloseChannel  pfNetClientCloseChannel
	#define CHK_NetClientCloseChannel  (pfNetClientCloseChannel != NULL)
	#define EXP_NetClientCloseChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientCloseChannel", (RTS_UINTPTR)NetClientCloseChannel, 0, 0) 
#endif




/**
 *  <description>
 *    Send a message.
 *    This call will fail, if
 *     - the channel is not in send mode
 *	   - the channel is already sending
 *     - the size of the data is greater then the commbuffersize returned by NetClientOpenChannel
 *  </description>
 *
 *	<param name="wChannelHandle" type="IN"> Handle to a channel as returned by NetClientOpenChannel</param>
 *  <param name="pduData" type="IN"> 
 *    The data to be sent. The buffer pointed to by pduData may not be changed
 *    until the package has been sent completely.
 *  </param>
*/ 
int CDECL NetClientSend(unsigned short wChannelHandle, PROTOCOL_DATA_UNIT pduData);
typedef int (CDECL * PFNETCLIENTSEND) (unsigned short wChannelHandle, PROTOCOL_DATA_UNIT pduData);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTSEND_NOTIMPLEMENTED)
	#define USE_NetClientSend
	#define EXT_NetClientSend
	#define GET_NetClientSend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientSend(p0,p1)  (int)ERR_NOTIMPLEMENTED
	#define CHK_NetClientSend  FALSE
	#define EXP_NetClientSend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientSend
	#define EXT_NetClientSend
	#define GET_NetClientSend(fl)  CAL_CMGETAPI( "NetClientSend" ) 
	#define CAL_NetClientSend  NetClientSend
	#define CHK_NetClientSend  TRUE
	#define EXP_NetClientSend  CAL_CMEXPAPI( "NetClientSend" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientSend
	#define EXT_NetClientSend
	#define GET_NetClientSend(fl)  CAL_CMGETAPI( "NetClientSend" ) 
	#define CAL_NetClientSend  NetClientSend
	#define CHK_NetClientSend  TRUE
	#define EXP_NetClientSend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientSend", (RTS_UINTPTR)NetClientSend, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientSend
	#define EXT_CmpChannelClientNetClientSend
	#define GET_CmpChannelClientNetClientSend  ERR_OK
	#define CAL_CmpChannelClientNetClientSend pICmpChannelClient->INetClientSend
	#define CHK_CmpChannelClientNetClientSend (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientSend  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientSend
	#define EXT_NetClientSend
	#define GET_NetClientSend(fl)  CAL_CMGETAPI( "NetClientSend" ) 
	#define CAL_NetClientSend pICmpChannelClient->INetClientSend
	#define CHK_NetClientSend (pICmpChannelClient != NULL)
	#define EXP_NetClientSend  CAL_CMEXPAPI( "NetClientSend" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientSend  PFNETCLIENTSEND pfNetClientSend;
	#define EXT_NetClientSend  extern PFNETCLIENTSEND pfNetClientSend;
	#define GET_NetClientSend(fl)  s_pfCMGetAPI2( "NetClientSend", (RTS_VOID_FCTPTR *)&pfNetClientSend, (fl), 0, 0)
	#define CAL_NetClientSend  pfNetClientSend
	#define CHK_NetClientSend  (pfNetClientSend != NULL)
	#define EXP_NetClientSend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientSend", (RTS_UINTPTR)NetClientSend, 0, 0) 
#endif




/**
 *	<description>
 *		Get the current status of an active channel.
 *	</description>
 *	<param name="wChannelHandle" type="IN"> 
 *		Handle to a channel as returned by NetClientOpenChannel
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
RTS_RESULT CDECL NetClientGetStatus(unsigned short wChannelHandle, RTS_UI16 *pusStatus, RTS_UI8 *pbyScalingFactor, RTS_I32 *pnItemsComplete, RTS_I32 *pnTotalItems);
typedef RTS_RESULT (CDECL * PFNETCLIENTGETSTATUS) (unsigned short wChannelHandle, RTS_UI16 *pusStatus, RTS_UI8 *pbyScalingFactor, RTS_I32 *pnItemsComplete, RTS_I32 *pnTotalItems);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTGETSTATUS_NOTIMPLEMENTED)
	#define USE_NetClientGetStatus
	#define EXT_NetClientGetStatus
	#define GET_NetClientGetStatus(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientGetStatus(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_NetClientGetStatus  FALSE
	#define EXP_NetClientGetStatus  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientGetStatus
	#define EXT_NetClientGetStatus
	#define GET_NetClientGetStatus(fl)  CAL_CMGETAPI( "NetClientGetStatus" ) 
	#define CAL_NetClientGetStatus  NetClientGetStatus
	#define CHK_NetClientGetStatus  TRUE
	#define EXP_NetClientGetStatus  CAL_CMEXPAPI( "NetClientGetStatus" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientGetStatus
	#define EXT_NetClientGetStatus
	#define GET_NetClientGetStatus(fl)  CAL_CMGETAPI( "NetClientGetStatus" ) 
	#define CAL_NetClientGetStatus  NetClientGetStatus
	#define CHK_NetClientGetStatus  TRUE
	#define EXP_NetClientGetStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientGetStatus", (RTS_UINTPTR)NetClientGetStatus, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientGetStatus
	#define EXT_CmpChannelClientNetClientGetStatus
	#define GET_CmpChannelClientNetClientGetStatus  ERR_OK
	#define CAL_CmpChannelClientNetClientGetStatus pICmpChannelClient->INetClientGetStatus
	#define CHK_CmpChannelClientNetClientGetStatus (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientGetStatus  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientGetStatus
	#define EXT_NetClientGetStatus
	#define GET_NetClientGetStatus(fl)  CAL_CMGETAPI( "NetClientGetStatus" ) 
	#define CAL_NetClientGetStatus pICmpChannelClient->INetClientGetStatus
	#define CHK_NetClientGetStatus (pICmpChannelClient != NULL)
	#define EXP_NetClientGetStatus  CAL_CMEXPAPI( "NetClientGetStatus" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientGetStatus  PFNETCLIENTGETSTATUS pfNetClientGetStatus;
	#define EXT_NetClientGetStatus  extern PFNETCLIENTGETSTATUS pfNetClientGetStatus;
	#define GET_NetClientGetStatus(fl)  s_pfCMGetAPI2( "NetClientGetStatus", (RTS_VOID_FCTPTR *)&pfNetClientGetStatus, (fl), 0, 0)
	#define CAL_NetClientGetStatus  pfNetClientGetStatus
	#define CHK_NetClientGetStatus  (pfNetClientGetStatus != NULL)
	#define EXP_NetClientGetStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientGetStatus", (RTS_UINTPTR)NetClientGetStatus, 0, 0) 
#endif




/* -- Functions exported to the L4Base network component -- */

/* <description>
 *   Get the buffer for the specified channel id. 
 *
 *   NOTE:
 *   After usage the channelbuffer MUST be released by
 *   calling NetServerReleaseChannel. Failing to do so will prevent the channel
 *   from being closed and the server will eventually run out of channels.
 *   Nevertheless, this function DOES NOT provide exclusive access to the channel.
 *   The L4Base component must use appropriate semaphores to ensure exclusive access.
 * </description>
 * <param name="addrPeer" type="IN"> The second endpoint of the channel. </param>
 * <param name="wChannelId" type="IN"> The id of the channel. </param>
 * <param name="ppChBuffer" type="OUT"> Is set to the channelbuffer, if the channel exists. </param>
 **/
int CDECL NetClientGetChannel(PEERADDRESS addrPeer, unsigned short wChannelId, CHANNELBUFFER **ppChBuffer);
typedef int (CDECL * PFNETCLIENTGETCHANNEL) (PEERADDRESS addrPeer, unsigned short wChannelId, CHANNELBUFFER **ppChBuffer);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTGETCHANNEL_NOTIMPLEMENTED)
	#define USE_NetClientGetChannel
	#define EXT_NetClientGetChannel
	#define GET_NetClientGetChannel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientGetChannel(p0,p1,p2)  (int)ERR_NOTIMPLEMENTED
	#define CHK_NetClientGetChannel  FALSE
	#define EXP_NetClientGetChannel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientGetChannel
	#define EXT_NetClientGetChannel
	#define GET_NetClientGetChannel(fl)  CAL_CMGETAPI( "NetClientGetChannel" ) 
	#define CAL_NetClientGetChannel  NetClientGetChannel
	#define CHK_NetClientGetChannel  TRUE
	#define EXP_NetClientGetChannel  CAL_CMEXPAPI( "NetClientGetChannel" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientGetChannel
	#define EXT_NetClientGetChannel
	#define GET_NetClientGetChannel(fl)  CAL_CMGETAPI( "NetClientGetChannel" ) 
	#define CAL_NetClientGetChannel  NetClientGetChannel
	#define CHK_NetClientGetChannel  TRUE
	#define EXP_NetClientGetChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientGetChannel", (RTS_UINTPTR)NetClientGetChannel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientGetChannel
	#define EXT_CmpChannelClientNetClientGetChannel
	#define GET_CmpChannelClientNetClientGetChannel  ERR_OK
	#define CAL_CmpChannelClientNetClientGetChannel pICmpChannelClient->INetClientGetChannel
	#define CHK_CmpChannelClientNetClientGetChannel (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientGetChannel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientGetChannel
	#define EXT_NetClientGetChannel
	#define GET_NetClientGetChannel(fl)  CAL_CMGETAPI( "NetClientGetChannel" ) 
	#define CAL_NetClientGetChannel pICmpChannelClient->INetClientGetChannel
	#define CHK_NetClientGetChannel (pICmpChannelClient != NULL)
	#define EXP_NetClientGetChannel  CAL_CMEXPAPI( "NetClientGetChannel" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientGetChannel  PFNETCLIENTGETCHANNEL pfNetClientGetChannel;
	#define EXT_NetClientGetChannel  extern PFNETCLIENTGETCHANNEL pfNetClientGetChannel;
	#define GET_NetClientGetChannel(fl)  s_pfCMGetAPI2( "NetClientGetChannel", (RTS_VOID_FCTPTR *)&pfNetClientGetChannel, (fl), 0, 0)
	#define CAL_NetClientGetChannel  pfNetClientGetChannel
	#define CHK_NetClientGetChannel  (pfNetClientGetChannel != NULL)
	#define EXP_NetClientGetChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientGetChannel", (RTS_UINTPTR)NetClientGetChannel, 0, 0) 
#endif




/**
 * <description>
 *   Release a channel buffer returned by NetServerGetChannel. This buffer may not be used
 *   after calling this function. Use NetServerGetChannel to acquire access to this channel again.
 * </description>
 *
 * <param name="pChBuffer" type="IN">The channel buffer to release</param>
*/
int CDECL NetClientReleaseChannel(CHANNELBUFFER *pChBuffer);
typedef int (CDECL * PFNETCLIENTRELEASECHANNEL) (CHANNELBUFFER *pChBuffer);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTRELEASECHANNEL_NOTIMPLEMENTED)
	#define USE_NetClientReleaseChannel
	#define EXT_NetClientReleaseChannel
	#define GET_NetClientReleaseChannel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientReleaseChannel(p0)  (int)ERR_NOTIMPLEMENTED
	#define CHK_NetClientReleaseChannel  FALSE
	#define EXP_NetClientReleaseChannel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientReleaseChannel
	#define EXT_NetClientReleaseChannel
	#define GET_NetClientReleaseChannel(fl)  CAL_CMGETAPI( "NetClientReleaseChannel" ) 
	#define CAL_NetClientReleaseChannel  NetClientReleaseChannel
	#define CHK_NetClientReleaseChannel  TRUE
	#define EXP_NetClientReleaseChannel  CAL_CMEXPAPI( "NetClientReleaseChannel" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientReleaseChannel
	#define EXT_NetClientReleaseChannel
	#define GET_NetClientReleaseChannel(fl)  CAL_CMGETAPI( "NetClientReleaseChannel" ) 
	#define CAL_NetClientReleaseChannel  NetClientReleaseChannel
	#define CHK_NetClientReleaseChannel  TRUE
	#define EXP_NetClientReleaseChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientReleaseChannel", (RTS_UINTPTR)NetClientReleaseChannel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientReleaseChannel
	#define EXT_CmpChannelClientNetClientReleaseChannel
	#define GET_CmpChannelClientNetClientReleaseChannel  ERR_OK
	#define CAL_CmpChannelClientNetClientReleaseChannel pICmpChannelClient->INetClientReleaseChannel
	#define CHK_CmpChannelClientNetClientReleaseChannel (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientReleaseChannel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientReleaseChannel
	#define EXT_NetClientReleaseChannel
	#define GET_NetClientReleaseChannel(fl)  CAL_CMGETAPI( "NetClientReleaseChannel" ) 
	#define CAL_NetClientReleaseChannel pICmpChannelClient->INetClientReleaseChannel
	#define CHK_NetClientReleaseChannel (pICmpChannelClient != NULL)
	#define EXP_NetClientReleaseChannel  CAL_CMEXPAPI( "NetClientReleaseChannel" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientReleaseChannel  PFNETCLIENTRELEASECHANNEL pfNetClientReleaseChannel;
	#define EXT_NetClientReleaseChannel  extern PFNETCLIENTRELEASECHANNEL pfNetClientReleaseChannel;
	#define GET_NetClientReleaseChannel(fl)  s_pfCMGetAPI2( "NetClientReleaseChannel", (RTS_VOID_FCTPTR *)&pfNetClientReleaseChannel, (fl), 0, 0)
	#define CAL_NetClientReleaseChannel  pfNetClientReleaseChannel
	#define CHK_NetClientReleaseChannel  (pfNetClientReleaseChannel != NULL)
	#define EXP_NetClientReleaseChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientReleaseChannel", (RTS_UINTPTR)NetClientReleaseChannel, 0, 0) 
#endif




/**
 * <description>
 *   Called by the L4Base component each time a meta-response arrives. 
 * </description>
 * <param name="addrSender" type="IN">The sender of the response</param>
 * <param name="pduData" type="IN">The response itself</param>
*/
int CDECL NetClientHandleMetaResponse(RTS_HANDLE hRouter, PEERADDRESS addrSender, PROTOCOL_DATA_UNIT pduData);
typedef int (CDECL * PFNETCLIENTHANDLEMETARESPONSE) (RTS_HANDLE hRouter, PEERADDRESS addrSender, PROTOCOL_DATA_UNIT pduData);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTHANDLEMETARESPONSE_NOTIMPLEMENTED)
	#define USE_NetClientHandleMetaResponse
	#define EXT_NetClientHandleMetaResponse
	#define GET_NetClientHandleMetaResponse(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientHandleMetaResponse(p0,p1,p2)  (int)ERR_NOTIMPLEMENTED
	#define CHK_NetClientHandleMetaResponse  FALSE
	#define EXP_NetClientHandleMetaResponse  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientHandleMetaResponse
	#define EXT_NetClientHandleMetaResponse
	#define GET_NetClientHandleMetaResponse(fl)  CAL_CMGETAPI( "NetClientHandleMetaResponse" ) 
	#define CAL_NetClientHandleMetaResponse  NetClientHandleMetaResponse
	#define CHK_NetClientHandleMetaResponse  TRUE
	#define EXP_NetClientHandleMetaResponse  CAL_CMEXPAPI( "NetClientHandleMetaResponse" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientHandleMetaResponse
	#define EXT_NetClientHandleMetaResponse
	#define GET_NetClientHandleMetaResponse(fl)  CAL_CMGETAPI( "NetClientHandleMetaResponse" ) 
	#define CAL_NetClientHandleMetaResponse  NetClientHandleMetaResponse
	#define CHK_NetClientHandleMetaResponse  TRUE
	#define EXP_NetClientHandleMetaResponse  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientHandleMetaResponse", (RTS_UINTPTR)NetClientHandleMetaResponse, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientHandleMetaResponse
	#define EXT_CmpChannelClientNetClientHandleMetaResponse
	#define GET_CmpChannelClientNetClientHandleMetaResponse  ERR_OK
	#define CAL_CmpChannelClientNetClientHandleMetaResponse pICmpChannelClient->INetClientHandleMetaResponse
	#define CHK_CmpChannelClientNetClientHandleMetaResponse (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientHandleMetaResponse  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientHandleMetaResponse
	#define EXT_NetClientHandleMetaResponse
	#define GET_NetClientHandleMetaResponse(fl)  CAL_CMGETAPI( "NetClientHandleMetaResponse" ) 
	#define CAL_NetClientHandleMetaResponse pICmpChannelClient->INetClientHandleMetaResponse
	#define CHK_NetClientHandleMetaResponse (pICmpChannelClient != NULL)
	#define EXP_NetClientHandleMetaResponse  CAL_CMEXPAPI( "NetClientHandleMetaResponse" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientHandleMetaResponse  PFNETCLIENTHANDLEMETARESPONSE pfNetClientHandleMetaResponse;
	#define EXT_NetClientHandleMetaResponse  extern PFNETCLIENTHANDLEMETARESPONSE pfNetClientHandleMetaResponse;
	#define GET_NetClientHandleMetaResponse(fl)  s_pfCMGetAPI2( "NetClientHandleMetaResponse", (RTS_VOID_FCTPTR *)&pfNetClientHandleMetaResponse, (fl), 0, 0)
	#define CAL_NetClientHandleMetaResponse  pfNetClientHandleMetaResponse
	#define CHK_NetClientHandleMetaResponse  (pfNetClientHandleMetaResponse != NULL)
	#define EXP_NetClientHandleMetaResponse  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientHandleMetaResponse", (RTS_UINTPTR)NetClientHandleMetaResponse, 0, 0) 
#endif




/**
 * <description>
 *   Calls the pfChannelHandler once for each active server channel 
 * </description>
 * <param name="pfChannelHandler" type="IN"> Function to be called for each channel </param>
 * <param name="pParam" type="INOUT"> This param is passed to pfChannelHandler. </param>
*/
void CDECL NetClientForEachChannel(PFCHANNELHANDLER pfChannelHandler, void * pParam);
typedef void (CDECL * PFNETCLIENTFOREACHCHANNEL) (PFCHANNELHANDLER pfChannelHandler, void * pParam);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTFOREACHCHANNEL_NOTIMPLEMENTED)
	#define USE_NetClientForEachChannel
	#define EXT_NetClientForEachChannel
	#define GET_NetClientForEachChannel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientForEachChannel(p0,p1)  
	#define CHK_NetClientForEachChannel  FALSE
	#define EXP_NetClientForEachChannel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientForEachChannel
	#define EXT_NetClientForEachChannel
	#define GET_NetClientForEachChannel(fl)  CAL_CMGETAPI( "NetClientForEachChannel" ) 
	#define CAL_NetClientForEachChannel  NetClientForEachChannel
	#define CHK_NetClientForEachChannel  TRUE
	#define EXP_NetClientForEachChannel  CAL_CMEXPAPI( "NetClientForEachChannel" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientForEachChannel
	#define EXT_NetClientForEachChannel
	#define GET_NetClientForEachChannel(fl)  CAL_CMGETAPI( "NetClientForEachChannel" ) 
	#define CAL_NetClientForEachChannel  NetClientForEachChannel
	#define CHK_NetClientForEachChannel  TRUE
	#define EXP_NetClientForEachChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientForEachChannel", (RTS_UINTPTR)NetClientForEachChannel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientForEachChannel
	#define EXT_CmpChannelClientNetClientForEachChannel
	#define GET_CmpChannelClientNetClientForEachChannel  ERR_OK
	#define CAL_CmpChannelClientNetClientForEachChannel pICmpChannelClient->INetClientForEachChannel
	#define CHK_CmpChannelClientNetClientForEachChannel (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientForEachChannel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientForEachChannel
	#define EXT_NetClientForEachChannel
	#define GET_NetClientForEachChannel(fl)  CAL_CMGETAPI( "NetClientForEachChannel" ) 
	#define CAL_NetClientForEachChannel pICmpChannelClient->INetClientForEachChannel
	#define CHK_NetClientForEachChannel (pICmpChannelClient != NULL)
	#define EXP_NetClientForEachChannel  CAL_CMEXPAPI( "NetClientForEachChannel" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientForEachChannel  PFNETCLIENTFOREACHCHANNEL pfNetClientForEachChannel;
	#define EXT_NetClientForEachChannel  extern PFNETCLIENTFOREACHCHANNEL pfNetClientForEachChannel;
	#define GET_NetClientForEachChannel(fl)  s_pfCMGetAPI2( "NetClientForEachChannel", (RTS_VOID_FCTPTR *)&pfNetClientForEachChannel, (fl), 0, 0)
	#define CAL_NetClientForEachChannel  pfNetClientForEachChannel
	#define CHK_NetClientForEachChannel  (pfNetClientForEachChannel != NULL)
	#define EXP_NetClientForEachChannel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientForEachChannel", (RTS_UINTPTR)NetClientForEachChannel, 0, 0) 
#endif




/**
 * <description>
 *   Called by the L4Base component whenever a complete L7 message arrived on a client channel 
 * </description>
 *  <param name="wChannelId" type="IN"></param> 
 *  <param name="addrSender" type="IN">Sender of the message</param>
 *   <param name="pduData" type="IN">Content of the message. The client must copy the contents of the message
 *                     before sending the next message, since the data pointed to by pData will
 *                     be valid only until the next message has been sent or the channel is closed.</param>
 */
int CDECL NetClientMessageReceived(CHANNELBUFFER *pChBuffer, PROTOCOL_DATA_UNIT pduData);
typedef int (CDECL * PFNETCLIENTMESSAGERECEIVED) (CHANNELBUFFER *pChBuffer, PROTOCOL_DATA_UNIT pduData);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTMESSAGERECEIVED_NOTIMPLEMENTED)
	#define USE_NetClientMessageReceived
	#define EXT_NetClientMessageReceived
	#define GET_NetClientMessageReceived(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientMessageReceived(p0,p1)  (int)ERR_NOTIMPLEMENTED
	#define CHK_NetClientMessageReceived  FALSE
	#define EXP_NetClientMessageReceived  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientMessageReceived
	#define EXT_NetClientMessageReceived
	#define GET_NetClientMessageReceived(fl)  CAL_CMGETAPI( "NetClientMessageReceived" ) 
	#define CAL_NetClientMessageReceived  NetClientMessageReceived
	#define CHK_NetClientMessageReceived  TRUE
	#define EXP_NetClientMessageReceived  CAL_CMEXPAPI( "NetClientMessageReceived" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientMessageReceived
	#define EXT_NetClientMessageReceived
	#define GET_NetClientMessageReceived(fl)  CAL_CMGETAPI( "NetClientMessageReceived" ) 
	#define CAL_NetClientMessageReceived  NetClientMessageReceived
	#define CHK_NetClientMessageReceived  TRUE
	#define EXP_NetClientMessageReceived  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientMessageReceived", (RTS_UINTPTR)NetClientMessageReceived, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientMessageReceived
	#define EXT_CmpChannelClientNetClientMessageReceived
	#define GET_CmpChannelClientNetClientMessageReceived  ERR_OK
	#define CAL_CmpChannelClientNetClientMessageReceived pICmpChannelClient->INetClientMessageReceived
	#define CHK_CmpChannelClientNetClientMessageReceived (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientMessageReceived  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientMessageReceived
	#define EXT_NetClientMessageReceived
	#define GET_NetClientMessageReceived(fl)  CAL_CMGETAPI( "NetClientMessageReceived" ) 
	#define CAL_NetClientMessageReceived pICmpChannelClient->INetClientMessageReceived
	#define CHK_NetClientMessageReceived (pICmpChannelClient != NULL)
	#define EXP_NetClientMessageReceived  CAL_CMEXPAPI( "NetClientMessageReceived" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientMessageReceived  PFNETCLIENTMESSAGERECEIVED pfNetClientMessageReceived;
	#define EXT_NetClientMessageReceived  extern PFNETCLIENTMESSAGERECEIVED pfNetClientMessageReceived;
	#define GET_NetClientMessageReceived(fl)  s_pfCMGetAPI2( "NetClientMessageReceived", (RTS_VOID_FCTPTR *)&pfNetClientMessageReceived, (fl), 0, 0)
	#define CAL_NetClientMessageReceived  pfNetClientMessageReceived
	#define CHK_NetClientMessageReceived  (pfNetClientMessageReceived != NULL)
	#define EXP_NetClientMessageReceived  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientMessageReceived", (RTS_UINTPTR)NetClientMessageReceived, 0, 0) 
#endif




/**
 * <description>
 *   Called when a unrecoverable error occurs for pfChBuffer, eg. ERR_CHANNEL_BROKEN.
 *   The channel must be closed by the net client.
 * </description>
 *	<param name="pChBuffer" type="IN"></param>
 *	<param name="nError" type="IN">The error</param>
*/
int CDECL NetClientChannelError(CHANNELBUFFER *pChBuffer, int nError);
typedef int (CDECL * PFNETCLIENTCHANNELERROR) (CHANNELBUFFER *pChBuffer, int nError);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTCHANNELERROR_NOTIMPLEMENTED)
	#define USE_NetClientChannelError
	#define EXT_NetClientChannelError
	#define GET_NetClientChannelError(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientChannelError(p0,p1)  (int)ERR_NOTIMPLEMENTED
	#define CHK_NetClientChannelError  FALSE
	#define EXP_NetClientChannelError  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientChannelError
	#define EXT_NetClientChannelError
	#define GET_NetClientChannelError(fl)  CAL_CMGETAPI( "NetClientChannelError" ) 
	#define CAL_NetClientChannelError  NetClientChannelError
	#define CHK_NetClientChannelError  TRUE
	#define EXP_NetClientChannelError  CAL_CMEXPAPI( "NetClientChannelError" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientChannelError
	#define EXT_NetClientChannelError
	#define GET_NetClientChannelError(fl)  CAL_CMGETAPI( "NetClientChannelError" ) 
	#define CAL_NetClientChannelError  NetClientChannelError
	#define CHK_NetClientChannelError  TRUE
	#define EXP_NetClientChannelError  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientChannelError", (RTS_UINTPTR)NetClientChannelError, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientChannelError
	#define EXT_CmpChannelClientNetClientChannelError
	#define GET_CmpChannelClientNetClientChannelError  ERR_OK
	#define CAL_CmpChannelClientNetClientChannelError pICmpChannelClient->INetClientChannelError
	#define CHK_CmpChannelClientNetClientChannelError (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientChannelError  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientChannelError
	#define EXT_NetClientChannelError
	#define GET_NetClientChannelError(fl)  CAL_CMGETAPI( "NetClientChannelError" ) 
	#define CAL_NetClientChannelError pICmpChannelClient->INetClientChannelError
	#define CHK_NetClientChannelError (pICmpChannelClient != NULL)
	#define EXP_NetClientChannelError  CAL_CMEXPAPI( "NetClientChannelError" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientChannelError  PFNETCLIENTCHANNELERROR pfNetClientChannelError;
	#define EXT_NetClientChannelError  extern PFNETCLIENTCHANNELERROR pfNetClientChannelError;
	#define GET_NetClientChannelError(fl)  s_pfCMGetAPI2( "NetClientChannelError", (RTS_VOID_FCTPTR *)&pfNetClientChannelError, (fl), 0, 0)
	#define CAL_NetClientChannelError  pfNetClientChannelError
	#define CHK_NetClientChannelError  (pfNetClientChannelError != NULL)
	#define EXP_NetClientChannelError  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientChannelError", (RTS_UINTPTR)NetClientChannelError, 0, 0) 
#endif




/**
 * <description>Function to register a handler interface</description>
 * <param name="wChannelHandle" type="IN">Channel handle that was retrieved by NetClientOpenChannel</param>
 * <param name="pIChannelClientApp" type="IN">Interface pointer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL NetClientRegisterAppInterface(unsigned short wChannelHandle, ICmpChannelClientApp *pIChannelClientApp);
typedef RTS_RESULT (CDECL * PFNETCLIENTREGISTERAPPINTERFACE) (unsigned short wChannelHandle, ICmpChannelClientApp *pIChannelClientApp);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTREGISTERAPPINTERFACE_NOTIMPLEMENTED)
	#define USE_NetClientRegisterAppInterface
	#define EXT_NetClientRegisterAppInterface
	#define GET_NetClientRegisterAppInterface(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientRegisterAppInterface(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_NetClientRegisterAppInterface  FALSE
	#define EXP_NetClientRegisterAppInterface  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientRegisterAppInterface
	#define EXT_NetClientRegisterAppInterface
	#define GET_NetClientRegisterAppInterface(fl)  CAL_CMGETAPI( "NetClientRegisterAppInterface" ) 
	#define CAL_NetClientRegisterAppInterface  NetClientRegisterAppInterface
	#define CHK_NetClientRegisterAppInterface  TRUE
	#define EXP_NetClientRegisterAppInterface  CAL_CMEXPAPI( "NetClientRegisterAppInterface" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientRegisterAppInterface
	#define EXT_NetClientRegisterAppInterface
	#define GET_NetClientRegisterAppInterface(fl)  CAL_CMGETAPI( "NetClientRegisterAppInterface" ) 
	#define CAL_NetClientRegisterAppInterface  NetClientRegisterAppInterface
	#define CHK_NetClientRegisterAppInterface  TRUE
	#define EXP_NetClientRegisterAppInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientRegisterAppInterface", (RTS_UINTPTR)NetClientRegisterAppInterface, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientRegisterAppInterface
	#define EXT_CmpChannelClientNetClientRegisterAppInterface
	#define GET_CmpChannelClientNetClientRegisterAppInterface  ERR_OK
	#define CAL_CmpChannelClientNetClientRegisterAppInterface pICmpChannelClient->INetClientRegisterAppInterface
	#define CHK_CmpChannelClientNetClientRegisterAppInterface (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientRegisterAppInterface  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientRegisterAppInterface
	#define EXT_NetClientRegisterAppInterface
	#define GET_NetClientRegisterAppInterface(fl)  CAL_CMGETAPI( "NetClientRegisterAppInterface" ) 
	#define CAL_NetClientRegisterAppInterface pICmpChannelClient->INetClientRegisterAppInterface
	#define CHK_NetClientRegisterAppInterface (pICmpChannelClient != NULL)
	#define EXP_NetClientRegisterAppInterface  CAL_CMEXPAPI( "NetClientRegisterAppInterface" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientRegisterAppInterface  PFNETCLIENTREGISTERAPPINTERFACE pfNetClientRegisterAppInterface;
	#define EXT_NetClientRegisterAppInterface  extern PFNETCLIENTREGISTERAPPINTERFACE pfNetClientRegisterAppInterface;
	#define GET_NetClientRegisterAppInterface(fl)  s_pfCMGetAPI2( "NetClientRegisterAppInterface", (RTS_VOID_FCTPTR *)&pfNetClientRegisterAppInterface, (fl), 0, 0)
	#define CAL_NetClientRegisterAppInterface  pfNetClientRegisterAppInterface
	#define CHK_NetClientRegisterAppInterface  (pfNetClientRegisterAppInterface != NULL)
	#define EXP_NetClientRegisterAppInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientRegisterAppInterface", (RTS_UINTPTR)NetClientRegisterAppInterface, 0, 0) 
#endif




/**
 * <description>Function to register a handler interface</description>
 * <param name="wChannelHandle" type="IN">Channel handle that was retrieved by NetClientOpenChannel</param>
 * <param name="pIChannelClientApp" type="IN">Interface pointer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL NetClientUnregisterAppInterface(unsigned short wChannelHandle, ICmpChannelClientApp *pIChannelClientApp);
typedef RTS_RESULT (CDECL * PFNETCLIENTUNREGISTERAPPINTERFACE) (unsigned short wChannelHandle, ICmpChannelClientApp *pIChannelClientApp);
#if defined(CMPCHANNELCLIENT_NOTIMPLEMENTED) || defined(NETCLIENTUNREGISTERAPPINTERFACE_NOTIMPLEMENTED)
	#define USE_NetClientUnregisterAppInterface
	#define EXT_NetClientUnregisterAppInterface
	#define GET_NetClientUnregisterAppInterface(fl)  ERR_NOTIMPLEMENTED
	#define CAL_NetClientUnregisterAppInterface(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_NetClientUnregisterAppInterface  FALSE
	#define EXP_NetClientUnregisterAppInterface  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_NetClientUnregisterAppInterface
	#define EXT_NetClientUnregisterAppInterface
	#define GET_NetClientUnregisterAppInterface(fl)  CAL_CMGETAPI( "NetClientUnregisterAppInterface" ) 
	#define CAL_NetClientUnregisterAppInterface  NetClientUnregisterAppInterface
	#define CHK_NetClientUnregisterAppInterface  TRUE
	#define EXP_NetClientUnregisterAppInterface  CAL_CMEXPAPI( "NetClientUnregisterAppInterface" ) 
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENT_EXTERNAL)
	#define USE_NetClientUnregisterAppInterface
	#define EXT_NetClientUnregisterAppInterface
	#define GET_NetClientUnregisterAppInterface(fl)  CAL_CMGETAPI( "NetClientUnregisterAppInterface" ) 
	#define CAL_NetClientUnregisterAppInterface  NetClientUnregisterAppInterface
	#define CHK_NetClientUnregisterAppInterface  TRUE
	#define EXP_NetClientUnregisterAppInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientUnregisterAppInterface", (RTS_UINTPTR)NetClientUnregisterAppInterface, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientNetClientUnregisterAppInterface
	#define EXT_CmpChannelClientNetClientUnregisterAppInterface
	#define GET_CmpChannelClientNetClientUnregisterAppInterface  ERR_OK
	#define CAL_CmpChannelClientNetClientUnregisterAppInterface pICmpChannelClient->INetClientUnregisterAppInterface
	#define CHK_CmpChannelClientNetClientUnregisterAppInterface (pICmpChannelClient != NULL)
	#define EXP_CmpChannelClientNetClientUnregisterAppInterface  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_NetClientUnregisterAppInterface
	#define EXT_NetClientUnregisterAppInterface
	#define GET_NetClientUnregisterAppInterface(fl)  CAL_CMGETAPI( "NetClientUnregisterAppInterface" ) 
	#define CAL_NetClientUnregisterAppInterface pICmpChannelClient->INetClientUnregisterAppInterface
	#define CHK_NetClientUnregisterAppInterface (pICmpChannelClient != NULL)
	#define EXP_NetClientUnregisterAppInterface  CAL_CMEXPAPI( "NetClientUnregisterAppInterface" ) 
#else /* DYNAMIC_LINK */
	#define USE_NetClientUnregisterAppInterface  PFNETCLIENTUNREGISTERAPPINTERFACE pfNetClientUnregisterAppInterface;
	#define EXT_NetClientUnregisterAppInterface  extern PFNETCLIENTUNREGISTERAPPINTERFACE pfNetClientUnregisterAppInterface;
	#define GET_NetClientUnregisterAppInterface(fl)  s_pfCMGetAPI2( "NetClientUnregisterAppInterface", (RTS_VOID_FCTPTR *)&pfNetClientUnregisterAppInterface, (fl), 0, 0)
	#define CAL_NetClientUnregisterAppInterface  pfNetClientUnregisterAppInterface
	#define CHK_NetClientUnregisterAppInterface  (pfNetClientUnregisterAppInterface != NULL)
	#define EXP_NetClientUnregisterAppInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"NetClientUnregisterAppInterface", (RTS_UINTPTR)NetClientUnregisterAppInterface, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFNETCLIENTOPENCHANNEL INetClientOpenChannel;
 	PFNETCLIENTOPENCHANNELRESULT INetClientOpenChannelResult;
 	PFNETCLIENTCLOSECHANNEL INetClientCloseChannel;
 	PFNETCLIENTSEND INetClientSend;
 	PFNETCLIENTGETSTATUS INetClientGetStatus;
 	PFNETCLIENTGETCHANNEL INetClientGetChannel;
 	PFNETCLIENTRELEASECHANNEL INetClientReleaseChannel;
 	PFNETCLIENTHANDLEMETARESPONSE INetClientHandleMetaResponse;
 	PFNETCLIENTFOREACHCHANNEL INetClientForEachChannel;
 	PFNETCLIENTMESSAGERECEIVED INetClientMessageReceived;
 	PFNETCLIENTCHANNELERROR INetClientChannelError;
 	PFNETCLIENTREGISTERAPPINTERFACE INetClientRegisterAppInterface;
 	PFNETCLIENTUNREGISTERAPPINTERFACE INetClientUnregisterAppInterface;
 } ICmpChannelClient_C;

#ifdef CPLUSPLUS
class ICmpChannelClient : public IBase
{
	public:
		virtual int CDECL INetClientOpenChannel(PEERADDRESS addrReceiver, RTS_UI32 dwCommBufferSize, RTS_UI32 *pdwReqId) =0;
		virtual int CDECL INetClientOpenChannelResult(RTS_UI32 dwRequestId, unsigned short *pwChannelHandle, RTS_UI32 *pdwCommBufferSize, RTS_BOOL *pbBigEndianByteOrder) =0;
		virtual int CDECL INetClientCloseChannel(unsigned short wChannelHandle) =0;
		virtual int CDECL INetClientSend(unsigned short wChannelHandle, PROTOCOL_DATA_UNIT pduData) =0;
		virtual RTS_RESULT CDECL INetClientGetStatus(unsigned short wChannelHandle, RTS_UI16 *pusStatus, RTS_UI8 *pbyScalingFactor, RTS_I32 *pnItemsComplete, RTS_I32 *pnTotalItems) =0;
		virtual int CDECL INetClientGetChannel(PEERADDRESS addrPeer, unsigned short wChannelId, CHANNELBUFFER **ppChBuffer) =0;
		virtual int CDECL INetClientReleaseChannel(CHANNELBUFFER *pChBuffer) =0;
		virtual int CDECL INetClientHandleMetaResponse(RTS_HANDLE hRouter, PEERADDRESS addrSender, PROTOCOL_DATA_UNIT pduData) =0;
		virtual void CDECL INetClientForEachChannel(PFCHANNELHANDLER pfChannelHandler, void * pParam) =0;
		virtual int CDECL INetClientMessageReceived(CHANNELBUFFER *pChBuffer, PROTOCOL_DATA_UNIT pduData) =0;
		virtual int CDECL INetClientChannelError(CHANNELBUFFER *pChBuffer, int nError) =0;
		virtual RTS_RESULT CDECL INetClientRegisterAppInterface(unsigned short wChannelHandle, ICmpChannelClientApp *pIChannelClientApp) =0;
		virtual RTS_RESULT CDECL INetClientUnregisterAppInterface(unsigned short wChannelHandle, ICmpChannelClientApp *pIChannelClientApp) =0;
};
	#ifndef ITF_CmpChannelClient
		#define ITF_CmpChannelClient static ICmpChannelClient *pICmpChannelClient = NULL;
	#endif
	#define EXTITF_CmpChannelClient
#else	/*CPLUSPLUS*/
	typedef ICmpChannelClient_C		ICmpChannelClient;
	#ifndef ITF_CmpChannelClient
		#define ITF_CmpChannelClient
	#endif
	#define EXTITF_CmpChannelClient
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPCHANNELCLIENTITF_H_*/
