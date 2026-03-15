 /**
 * <interfacename>CmpSecureChannel</interfacename>
 * <description> 
 *	Interface for a secure channel implementation to provide a secure (TLS based) channel on top of a communication channel.
 *  In the communication stack of the CODESYS runtime system the implementation of this interface
 *  can be optionally called between CmpChannelServer and CmpSrv.
 * </description>
 *
 *  SECURITY-NOTE:
 *           The security manager must be linked statically to the runtime system core to prevent, that it cannot be removed or replaced by a malware!
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPSECURECHANNELITF_H_
#define _CMPSECURECHANNELITF_H_

#include "CmpStd.h"

 

 




#include "CmpCommunicationLibItf.h"
#include "CmpSecurityManagerItf.h"

#ifndef PLC_HANDLER
	#ifdef CMPSECURECHANNEL_EXTERNAL
		#error CmpSecureChannel must not be over loadable so defining CMPSECURECHANNEL_EXTERNAL is forbidden because of security requirements!
	#endif
#endif
	
/*
 * Header used for transmitting TLS related packets on the channel
 */

typedef struct
{
	RTS_UI16		ui16HeaderTag;		/* Header indicating protocol type */
	RTS_UI16		ui16HeaderLength;	/* Length of the header beginning at ulServiceLength */
	RTS_UI16		ui16ServiceGroup;	/* Not Used -> Set to 0 */
	RTS_UI16		ui16Service;		/* Not Used -> Set to 0 */
	RTS_UI32		ui32SessionID;		/* Not Used -> Set to 0 */
	RTS_UI32		ui32ServiceLength;	/* Length of the data following after this header */
	RTS_UI32		ui32NumBlocks;		/* Number of TLS record protocol blocks following in this message */
	RTS_UI8			ui8MajorVersion;	/* Major Version of TLS protocol inside CDS */
	RTS_UI8			ui8MinorVersion;	/* Minor version of TLS protocol inside CDS */
	RTS_UI16		ui16Reserved;		/* Reserved have to be 0 */
} HEADER_TAG_CRYPT;


/**
 * <category>SecuritySettings</category>
 * <type>String</type>
 * <description>Communication mode to select, which kind of communication is possible.
 *	NOTE:
 *		Security modes must be ordered in an descending sorting, that the setting with the highest security level is found at the beginning of the list!
 * </description>
 */
#define CMPSECCHANNEL_KEY_STRING_ONLINECOMMMODE						CMPSETTINGS_CATEGORY_SECURITY ".CommunicationMode"
#define CMPSECCHANNEL_KEY_STRING_ONLINECOMMMODE_DESC				"Communication mode for online communication"

#define CMPSECCHANNEL_VALUE_ONLINECOMMMODE_ONLY_ENCRYPTED			"ONLY_ENCRYPTED"
#define CMPSECCHANNEL_VALUE_ONLINECOMMMODE_PLAIN_AND_ENCRYPTED		"PLAIN_AND_ENCRYPTED"
#define CMPSECCHANNEL_VALUE_ONLINECOMMMODE_ONLY_PLAIN				"ONLY_PLAIN"

#ifndef CMPSECCHANNEL_VALUE_ONLINECOMMMODE_DEFAULT
	#define CMPSECCHANNEL_VALUE_ONLINECOMMMODE_DEFAULT				CMPSECCHANNEL_VALUE_ONLINECOMMMODE_PLAIN_AND_ENCRYPTED
#endif

/**
 * <description>Security level for the programming system communication</description>
 * <element name="OnlineCommMode_ONLY_PLAIN" type="OUT">Only unencrypted communication supported</element>
 * <element name="OnlineCommMode_PLAIN_AND_ENCRYPTED" type="OUT">Encrypted and unencrypted communication supported. Can be decided by the online user</element>
 * <element name="OnlineCommMode_ONLY_ENCRYPTED" type="OUT">Only encrypted communication allowed</element>
 */
typedef enum
{
	OnlineCommMode_ONLY_PLAIN,
	OnlineCommMode_PLAIN_AND_ENCRYPTED,
	OnlineCommMode_ONLY_ENCRYPTED
} CMPSECURECHANNEL_ONLINECOMMMODE_IDS;

#define CMPSECCHANNEL_VALUE_ONLINECOMMMODE_LIST		{\
														{(RTS_I32)OnlineCommMode_ONLY_PLAIN, CMPSECMAN_SFLAGS_NONE, CMPSECCHANNEL_VALUE_ONLINECOMMMODE_ONLY_PLAIN, "Only unencrypted communication allowed"},\
														{(RTS_I32)OnlineCommMode_PLAIN_AND_ENCRYPTED, CMPSECMAN_SFLAGS_NONE, CMPSECCHANNEL_VALUE_ONLINECOMMMODE_PLAIN_AND_ENCRYPTED, "Encrypted and unencrypted communication supported. Can be decided by the online user"},\
														{(RTS_I32)OnlineCommMode_ONLY_ENCRYPTED, CMPSECMAN_SFLAGS_NONE, CMPSECCHANNEL_VALUE_ONLINECOMMMODE_ONLY_ENCRYPTED, "Only encrypted communication supported"}\
													}

			
/**
 * <category>SecuritySettings</category>
 * <description>Security setting to specify, if a self-signed cert should be created by default if not available</description>
 */
#define CMPSECCHANNEL_KEY_STRING_CREATE_SELFSIGNED_CERT				SECURITY_KEY_STRING_CREATE_SELFSIGNED_CERT
#define CMPSECCHANNEL_KEY_STRING_CREATE_SELFSIGNED_CERT_DESC		SECURITY_KEY_STRING_CREATE_SELFSIGNED_CERT_DESC

#ifndef CMPSECCHANNEL_VALUE_CREATE_SELFSIGNED_CERT_DEFAULT
	#define CMPSECCHANNEL_VALUE_CREATE_SELFSIGNED_CERT_DEFAULT		SECURITY_VALUE_CREATE_SELFSIGNED_CERT_YES
#endif

#define CMPSECCHANNEL_VALUE_CREATE_SELFSIGNED_CERT_LIST				SECURITY_VALUE_CREATE_SELFSIGNED_CERT_LIST
																
/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Hash of the last used certificate. This is used to restore an expired certificate after the systems reboot.
 * This setting is managed by the CmpSecureChannel and must not be set by hand.</description>
 */
#define CMPSECCHANNEL_KEY_STRING_CERTHASH			"CertificateHash"
#ifndef CMPSECCHANNEL_VALUE_CERTHASH_DEFAULT	
	#define CMPSECCHANNEL_VALUE_CERTHASH_DEFAULT	""
#endif

#ifdef __cplusplus
extern "C" {
#endif


/* -- Functions exported to the layer 4 network component. Only to be called by CmpChannelServer. -- */

/**
 * <description>Handle one service request from the communication layer below (channel server)</description>
 * <param name="ui32ChannelHandle" type="IN">Id of the channel on which the request arrived</param>
 * <param name="pduRequest" type="IN">Pointer to the request</param>
 * <param name="pduReply" type="OUT">Pointer to the request reply buffer</param>
 * <param name="bFirstCall" type="IN">0: Tells the function, if it was already called for the same request before (0) or not (1).</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Received service was completely handled.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PENDING">Received service will be handled asynchronously, 
 * but the caller has not to take care about this anymore.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_CALL_AGAIN">Received service will be handled asynchronously. 
 * To progress this function have to be called again for the same received service data with bFirstCall=0.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_...">Another error occurred, channel should be closed.</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SecChServerHandleRequest(RTS_UI32 ui32ChannelHandle, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply, RTS_UI32 bFirstCall);
typedef RTS_RESULT (CDECL * PFSECCHSERVERHANDLEREQUEST) (RTS_UI32 ui32ChannelHandle, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply, RTS_UI32 bFirstCall);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHSERVERHANDLEREQUEST_NOTIMPLEMENTED)
	#define USE_SecChServerHandleRequest
	#define EXT_SecChServerHandleRequest
	#define GET_SecChServerHandleRequest(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChServerHandleRequest(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecChServerHandleRequest  FALSE
	#define EXP_SecChServerHandleRequest  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChServerHandleRequest
	#define EXT_SecChServerHandleRequest
	#define GET_SecChServerHandleRequest(fl)  CAL_CMGETAPI( "SecChServerHandleRequest" ) 
	#define CAL_SecChServerHandleRequest  SecChServerHandleRequest
	#define CHK_SecChServerHandleRequest  TRUE
	#define EXP_SecChServerHandleRequest  CAL_CMEXPAPI( "SecChServerHandleRequest" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChServerHandleRequest
	#define EXT_SecChServerHandleRequest
	#define GET_SecChServerHandleRequest(fl)  CAL_CMGETAPI( "SecChServerHandleRequest" ) 
	#define CAL_SecChServerHandleRequest  SecChServerHandleRequest
	#define CHK_SecChServerHandleRequest  TRUE
	#define EXP_SecChServerHandleRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerHandleRequest", (RTS_UINTPTR)SecChServerHandleRequest, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChServerHandleRequest
	#define EXT_CmpSecureChannelSecChServerHandleRequest
	#define GET_CmpSecureChannelSecChServerHandleRequest  ERR_OK
	#define CAL_CmpSecureChannelSecChServerHandleRequest pICmpSecureChannel->ISecChServerHandleRequest
	#define CHK_CmpSecureChannelSecChServerHandleRequest (pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChServerHandleRequest  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChServerHandleRequest
	#define EXT_SecChServerHandleRequest
	#define GET_SecChServerHandleRequest(fl)  CAL_CMGETAPI( "SecChServerHandleRequest" ) 
	#define CAL_SecChServerHandleRequest pICmpSecureChannel->ISecChServerHandleRequest
	#define CHK_SecChServerHandleRequest (pICmpSecureChannel != NULL)
	#define EXP_SecChServerHandleRequest  CAL_CMEXPAPI( "SecChServerHandleRequest" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChServerHandleRequest  PFSECCHSERVERHANDLEREQUEST pfSecChServerHandleRequest;
	#define EXT_SecChServerHandleRequest  extern PFSECCHSERVERHANDLEREQUEST pfSecChServerHandleRequest;
	#define GET_SecChServerHandleRequest(fl)  s_pfCMGetAPI2( "SecChServerHandleRequest", (RTS_VOID_FCTPTR *)&pfSecChServerHandleRequest, (fl), 0, 0)
	#define CAL_SecChServerHandleRequest  pfSecChServerHandleRequest
	#define CHK_SecChServerHandleRequest  (pfSecChServerHandleRequest != NULL)
	#define EXP_SecChServerHandleRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerHandleRequest", (RTS_UINTPTR)SecChServerHandleRequest, 0, 0) 
#endif






/* -- Functions exported to the layer 6 network component. Only to be called by CmpSrv. -- */

/**
* <description>
*   Retrieves the number of available server channels.
*   This is equal to the max. number of clients, which can be connected at the same time.
* </description>
* <param name="pui16MaxChannels" type="OUT">Number of channels.</param>
* <result>error code</result>
*/
RTS_RESULT CDECL SecChServerGetMaxChannels(RTS_UI16 *pui16MaxChannels);
typedef RTS_RESULT (CDECL * PFSECCHSERVERGETMAXCHANNELS) (RTS_UI16 *pui16MaxChannels);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHSERVERGETMAXCHANNELS_NOTIMPLEMENTED)
	#define USE_SecChServerGetMaxChannels
	#define EXT_SecChServerGetMaxChannels
	#define GET_SecChServerGetMaxChannels(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChServerGetMaxChannels(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecChServerGetMaxChannels  FALSE
	#define EXP_SecChServerGetMaxChannels  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChServerGetMaxChannels
	#define EXT_SecChServerGetMaxChannels
	#define GET_SecChServerGetMaxChannels(fl)  CAL_CMGETAPI( "SecChServerGetMaxChannels" ) 
	#define CAL_SecChServerGetMaxChannels  SecChServerGetMaxChannels
	#define CHK_SecChServerGetMaxChannels  TRUE
	#define EXP_SecChServerGetMaxChannels  CAL_CMEXPAPI( "SecChServerGetMaxChannels" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChServerGetMaxChannels
	#define EXT_SecChServerGetMaxChannels
	#define GET_SecChServerGetMaxChannels(fl)  CAL_CMGETAPI( "SecChServerGetMaxChannels" ) 
	#define CAL_SecChServerGetMaxChannels  SecChServerGetMaxChannels
	#define CHK_SecChServerGetMaxChannels  TRUE
	#define EXP_SecChServerGetMaxChannels  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerGetMaxChannels", (RTS_UINTPTR)SecChServerGetMaxChannels, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChServerGetMaxChannels
	#define EXT_CmpSecureChannelSecChServerGetMaxChannels
	#define GET_CmpSecureChannelSecChServerGetMaxChannels  ERR_OK
	#define CAL_CmpSecureChannelSecChServerGetMaxChannels pICmpSecureChannel->ISecChServerGetMaxChannels
	#define CHK_CmpSecureChannelSecChServerGetMaxChannels (pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChServerGetMaxChannels  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChServerGetMaxChannels
	#define EXT_SecChServerGetMaxChannels
	#define GET_SecChServerGetMaxChannels(fl)  CAL_CMGETAPI( "SecChServerGetMaxChannels" ) 
	#define CAL_SecChServerGetMaxChannels pICmpSecureChannel->ISecChServerGetMaxChannels
	#define CHK_SecChServerGetMaxChannels (pICmpSecureChannel != NULL)
	#define EXP_SecChServerGetMaxChannels  CAL_CMEXPAPI( "SecChServerGetMaxChannels" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChServerGetMaxChannels  PFSECCHSERVERGETMAXCHANNELS pfSecChServerGetMaxChannels;
	#define EXT_SecChServerGetMaxChannels  extern PFSECCHSERVERGETMAXCHANNELS pfSecChServerGetMaxChannels;
	#define GET_SecChServerGetMaxChannels(fl)  s_pfCMGetAPI2( "SecChServerGetMaxChannels", (RTS_VOID_FCTPTR *)&pfSecChServerGetMaxChannels, (fl), 0, 0)
	#define CAL_SecChServerGetMaxChannels  pfSecChServerGetMaxChannels
	#define CHK_SecChServerGetMaxChannels  (pfSecChServerGetMaxChannels != NULL)
	#define EXP_SecChServerGetMaxChannels  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerGetMaxChannels", (RTS_UINTPTR)SecChServerGetMaxChannels, 0, 0) 
#endif




/**
* <description>
*   Retrieves general information for the specified server channel. This function is intended for information purposes only.
* </description>
* <param name="ui16ChannelIndex" type="IN">Index of the channel. Allowed range: 0..MaxChannels-1.</param>
* <param name="pui32ServerState" type="OUT">State of the server channel, see category "channel server state" for CSSTATE_ values in CmpCommunicationLibItf.</param>
* <param name="pChInfoBuffer" type="INOUT">Caller allocated buffer, which is filled by the CHANNELINFO structure. If the state is CSSTATE_FREE, no structure is returned.</param>
* <param name="psiBufferLen" type="INOUT">Pointer to the size of the buffer in bytes, returns the number of copied bytes.</param>
* <result>error code</result>
*/
RTS_RESULT CDECL SecChServerGetChannelInfoByIndex(RTS_UI16 ui16ChannelIndex, RTS_UI32 *pui32ServerState, CHANNELINFO *pChInfoBuffer, RTS_SIZE *psiBufferLen);
typedef RTS_RESULT (CDECL * PFSECCHSERVERGETCHANNELINFOBYINDEX) (RTS_UI16 ui16ChannelIndex, RTS_UI32 *pui32ServerState, CHANNELINFO *pChInfoBuffer, RTS_SIZE *psiBufferLen);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHSERVERGETCHANNELINFOBYINDEX_NOTIMPLEMENTED)
	#define USE_SecChServerGetChannelInfoByIndex
	#define EXT_SecChServerGetChannelInfoByIndex
	#define GET_SecChServerGetChannelInfoByIndex(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChServerGetChannelInfoByIndex(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecChServerGetChannelInfoByIndex  FALSE
	#define EXP_SecChServerGetChannelInfoByIndex  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChServerGetChannelInfoByIndex
	#define EXT_SecChServerGetChannelInfoByIndex
	#define GET_SecChServerGetChannelInfoByIndex(fl)  CAL_CMGETAPI( "SecChServerGetChannelInfoByIndex" ) 
	#define CAL_SecChServerGetChannelInfoByIndex  SecChServerGetChannelInfoByIndex
	#define CHK_SecChServerGetChannelInfoByIndex  TRUE
	#define EXP_SecChServerGetChannelInfoByIndex  CAL_CMEXPAPI( "SecChServerGetChannelInfoByIndex" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChServerGetChannelInfoByIndex
	#define EXT_SecChServerGetChannelInfoByIndex
	#define GET_SecChServerGetChannelInfoByIndex(fl)  CAL_CMGETAPI( "SecChServerGetChannelInfoByIndex" ) 
	#define CAL_SecChServerGetChannelInfoByIndex  SecChServerGetChannelInfoByIndex
	#define CHK_SecChServerGetChannelInfoByIndex  TRUE
	#define EXP_SecChServerGetChannelInfoByIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerGetChannelInfoByIndex", (RTS_UINTPTR)SecChServerGetChannelInfoByIndex, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChServerGetChannelInfoByIndex
	#define EXT_CmpSecureChannelSecChServerGetChannelInfoByIndex
	#define GET_CmpSecureChannelSecChServerGetChannelInfoByIndex  ERR_OK
	#define CAL_CmpSecureChannelSecChServerGetChannelInfoByIndex pICmpSecureChannel->ISecChServerGetChannelInfoByIndex
	#define CHK_CmpSecureChannelSecChServerGetChannelInfoByIndex (pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChServerGetChannelInfoByIndex  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChServerGetChannelInfoByIndex
	#define EXT_SecChServerGetChannelInfoByIndex
	#define GET_SecChServerGetChannelInfoByIndex(fl)  CAL_CMGETAPI( "SecChServerGetChannelInfoByIndex" ) 
	#define CAL_SecChServerGetChannelInfoByIndex pICmpSecureChannel->ISecChServerGetChannelInfoByIndex
	#define CHK_SecChServerGetChannelInfoByIndex (pICmpSecureChannel != NULL)
	#define EXP_SecChServerGetChannelInfoByIndex  CAL_CMEXPAPI( "SecChServerGetChannelInfoByIndex" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChServerGetChannelInfoByIndex  PFSECCHSERVERGETCHANNELINFOBYINDEX pfSecChServerGetChannelInfoByIndex;
	#define EXT_SecChServerGetChannelInfoByIndex  extern PFSECCHSERVERGETCHANNELINFOBYINDEX pfSecChServerGetChannelInfoByIndex;
	#define GET_SecChServerGetChannelInfoByIndex(fl)  s_pfCMGetAPI2( "SecChServerGetChannelInfoByIndex", (RTS_VOID_FCTPTR *)&pfSecChServerGetChannelInfoByIndex, (fl), 0, 0)
	#define CAL_SecChServerGetChannelInfoByIndex  pfSecChServerGetChannelInfoByIndex
	#define CHK_SecChServerGetChannelInfoByIndex  (pfSecChServerGetChannelInfoByIndex != NULL)
	#define EXP_SecChServerGetChannelInfoByIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerGetChannelInfoByIndex", (RTS_UINTPTR)SecChServerGetChannelInfoByIndex, 0, 0) 
#endif




/**
*	<description>
*		Get the current status of an active channel.
*	</description>
*	<param name="ui32ChannelHandle" type="IN">
*		Id of the channel
*	</param>
*	<param name="pui16Status" type="OUT">
*		Is set to the current progress state. The PROGRESS_xxx constants define valid values.
*	</param>
*  <param name="pui8ScalingFactor" type="OUT">
*		Provides the scaling factor for pnItemsComplete and pnTotalItems. These values have been scaled
*		down by dividing them through 2^ScalingFactor
*		(i.e. they have been right shifted by ScalingFactor bits).
*  </param>
*	<param name="pi32ItemsComplete" type="OUT">
*		Number of items completed (e.g. the number of bytes transfered).
*  </param>
* 	<param name="pi32TotalItems" type="OUT">
*		Total number of item. Is set to -1 if unknown.
*  </param>
*  <result>error code</result>
*/
RTS_RESULT CDECL SecChServerGetStatus(RTS_UI32 ui32ChannelHandle, RTS_UI16 *pui16Status, RTS_UI8 *pui8ScalingFactor, RTS_I32 *pi32ItemsComplete, RTS_I32 *pi32TotalItems);
typedef RTS_RESULT (CDECL * PFSECCHSERVERGETSTATUS) (RTS_UI32 ui32ChannelHandle, RTS_UI16 *pui16Status, RTS_UI8 *pui8ScalingFactor, RTS_I32 *pi32ItemsComplete, RTS_I32 *pi32TotalItems);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHSERVERGETSTATUS_NOTIMPLEMENTED)
	#define USE_SecChServerGetStatus
	#define EXT_SecChServerGetStatus
	#define GET_SecChServerGetStatus(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChServerGetStatus(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecChServerGetStatus  FALSE
	#define EXP_SecChServerGetStatus  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChServerGetStatus
	#define EXT_SecChServerGetStatus
	#define GET_SecChServerGetStatus(fl)  CAL_CMGETAPI( "SecChServerGetStatus" ) 
	#define CAL_SecChServerGetStatus  SecChServerGetStatus
	#define CHK_SecChServerGetStatus  TRUE
	#define EXP_SecChServerGetStatus  CAL_CMEXPAPI( "SecChServerGetStatus" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChServerGetStatus
	#define EXT_SecChServerGetStatus
	#define GET_SecChServerGetStatus(fl)  CAL_CMGETAPI( "SecChServerGetStatus" ) 
	#define CAL_SecChServerGetStatus  SecChServerGetStatus
	#define CHK_SecChServerGetStatus  TRUE
	#define EXP_SecChServerGetStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerGetStatus", (RTS_UINTPTR)SecChServerGetStatus, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChServerGetStatus
	#define EXT_CmpSecureChannelSecChServerGetStatus
	#define GET_CmpSecureChannelSecChServerGetStatus  ERR_OK
	#define CAL_CmpSecureChannelSecChServerGetStatus pICmpSecureChannel->ISecChServerGetStatus
	#define CHK_CmpSecureChannelSecChServerGetStatus (pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChServerGetStatus  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChServerGetStatus
	#define EXT_SecChServerGetStatus
	#define GET_SecChServerGetStatus(fl)  CAL_CMGETAPI( "SecChServerGetStatus" ) 
	#define CAL_SecChServerGetStatus pICmpSecureChannel->ISecChServerGetStatus
	#define CHK_SecChServerGetStatus (pICmpSecureChannel != NULL)
	#define EXP_SecChServerGetStatus  CAL_CMEXPAPI( "SecChServerGetStatus" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChServerGetStatus  PFSECCHSERVERGETSTATUS pfSecChServerGetStatus;
	#define EXT_SecChServerGetStatus  extern PFSECCHSERVERGETSTATUS pfSecChServerGetStatus;
	#define GET_SecChServerGetStatus(fl)  s_pfCMGetAPI2( "SecChServerGetStatus", (RTS_VOID_FCTPTR *)&pfSecChServerGetStatus, (fl), 0, 0)
	#define CAL_SecChServerGetStatus  pfSecChServerGetStatus
	#define CHK_SecChServerGetStatus  (pfSecChServerGetStatus != NULL)
	#define EXP_SecChServerGetStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerGetStatus", (RTS_UINTPTR)SecChServerGetStatus, 0, 0) 
#endif




/**
* <description>
*   Retrieves the current request and reply buffer.
* </description>
* <param name="ui32ChannelHandle" type="IN">Id of the channel for which the session id should be read.</param>
* <param name="ppduRequest" type="INOUT">Caller allocated PDU to be filled with pointer and length of the received request.</param>
* <param name="ppduReplyBuffer" type="INOUT">Caller allocated PDU to be filled with pointer and length of the reply.</param>
* <result>error code</result>
*/
RTS_RESULT CDECL SecChServerGetRequest(RTS_UI32 ui32ChannelHandle, PROTOCOL_DATA_UNIT *ppduRequest, PROTOCOL_DATA_UNIT *ppduReplyBuffer);
typedef RTS_RESULT (CDECL * PFSECCHSERVERGETREQUEST) (RTS_UI32 ui32ChannelHandle, PROTOCOL_DATA_UNIT *ppduRequest, PROTOCOL_DATA_UNIT *ppduReplyBuffer);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHSERVERGETREQUEST_NOTIMPLEMENTED)
	#define USE_SecChServerGetRequest
	#define EXT_SecChServerGetRequest
	#define GET_SecChServerGetRequest(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChServerGetRequest(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecChServerGetRequest  FALSE
	#define EXP_SecChServerGetRequest  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChServerGetRequest
	#define EXT_SecChServerGetRequest
	#define GET_SecChServerGetRequest(fl)  CAL_CMGETAPI( "SecChServerGetRequest" ) 
	#define CAL_SecChServerGetRequest  SecChServerGetRequest
	#define CHK_SecChServerGetRequest  TRUE
	#define EXP_SecChServerGetRequest  CAL_CMEXPAPI( "SecChServerGetRequest" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChServerGetRequest
	#define EXT_SecChServerGetRequest
	#define GET_SecChServerGetRequest(fl)  CAL_CMGETAPI( "SecChServerGetRequest" ) 
	#define CAL_SecChServerGetRequest  SecChServerGetRequest
	#define CHK_SecChServerGetRequest  TRUE
	#define EXP_SecChServerGetRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerGetRequest", (RTS_UINTPTR)SecChServerGetRequest, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChServerGetRequest
	#define EXT_CmpSecureChannelSecChServerGetRequest
	#define GET_CmpSecureChannelSecChServerGetRequest  ERR_OK
	#define CAL_CmpSecureChannelSecChServerGetRequest pICmpSecureChannel->ISecChServerGetRequest
	#define CHK_CmpSecureChannelSecChServerGetRequest (pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChServerGetRequest  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChServerGetRequest
	#define EXT_SecChServerGetRequest
	#define GET_SecChServerGetRequest(fl)  CAL_CMGETAPI( "SecChServerGetRequest" ) 
	#define CAL_SecChServerGetRequest pICmpSecureChannel->ISecChServerGetRequest
	#define CHK_SecChServerGetRequest (pICmpSecureChannel != NULL)
	#define EXP_SecChServerGetRequest  CAL_CMEXPAPI( "SecChServerGetRequest" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChServerGetRequest  PFSECCHSERVERGETREQUEST pfSecChServerGetRequest;
	#define EXT_SecChServerGetRequest  extern PFSECCHSERVERGETREQUEST pfSecChServerGetRequest;
	#define GET_SecChServerGetRequest(fl)  s_pfCMGetAPI2( "SecChServerGetRequest", (RTS_VOID_FCTPTR *)&pfSecChServerGetRequest, (fl), 0, 0)
	#define CAL_SecChServerGetRequest  pfSecChServerGetRequest
	#define CHK_SecChServerGetRequest  (pfSecChServerGetRequest != NULL)
	#define EXP_SecChServerGetRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerGetRequest", (RTS_UINTPTR)SecChServerGetRequest, 0, 0) 
#endif




/**
* <description>
*   Commits the filled reply buffer to the channel and triggers sending the reply.
* </description>
* <param name="ui32ChannelHandle" type="IN">Id of the channel for which the session id should be read.</param>
* <param name="pduData" type="IN">Pointer to the reply buffer</param>
* <param name="closeChannel" type="IN">If set to TRUE the channel will be closed after sending the pdu.</param>
* <param name="closeReason" type="IN">The error code to send when closing the channel.</param>
* <result>error code</result>
*/
RTS_RESULT CDECL SecChServerFinishRequest(RTS_UI32 ui32ChannelHandle, PROTOCOL_DATA_UNIT pduData, RTS_BOOL closeChannel, RTS_RESULT closeReason);
typedef RTS_RESULT (CDECL * PFSECCHSERVERFINISHREQUEST) (RTS_UI32 ui32ChannelHandle, PROTOCOL_DATA_UNIT pduData, RTS_BOOL closeChannel, RTS_RESULT closeReason);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHSERVERFINISHREQUEST_NOTIMPLEMENTED)
	#define USE_SecChServerFinishRequest
	#define EXT_SecChServerFinishRequest
	#define GET_SecChServerFinishRequest(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChServerFinishRequest(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecChServerFinishRequest  FALSE
	#define EXP_SecChServerFinishRequest  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChServerFinishRequest
	#define EXT_SecChServerFinishRequest
	#define GET_SecChServerFinishRequest(fl)  CAL_CMGETAPI( "SecChServerFinishRequest" ) 
	#define CAL_SecChServerFinishRequest  SecChServerFinishRequest
	#define CHK_SecChServerFinishRequest  TRUE
	#define EXP_SecChServerFinishRequest  CAL_CMEXPAPI( "SecChServerFinishRequest" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChServerFinishRequest
	#define EXT_SecChServerFinishRequest
	#define GET_SecChServerFinishRequest(fl)  CAL_CMGETAPI( "SecChServerFinishRequest" ) 
	#define CAL_SecChServerFinishRequest  SecChServerFinishRequest
	#define CHK_SecChServerFinishRequest  TRUE
	#define EXP_SecChServerFinishRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerFinishRequest", (RTS_UINTPTR)SecChServerFinishRequest, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChServerFinishRequest
	#define EXT_CmpSecureChannelSecChServerFinishRequest
	#define GET_CmpSecureChannelSecChServerFinishRequest  ERR_OK
	#define CAL_CmpSecureChannelSecChServerFinishRequest pICmpSecureChannel->ISecChServerFinishRequest
	#define CHK_CmpSecureChannelSecChServerFinishRequest (pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChServerFinishRequest  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChServerFinishRequest
	#define EXT_SecChServerFinishRequest
	#define GET_SecChServerFinishRequest(fl)  CAL_CMGETAPI( "SecChServerFinishRequest" ) 
	#define CAL_SecChServerFinishRequest pICmpSecureChannel->ISecChServerFinishRequest
	#define CHK_SecChServerFinishRequest (pICmpSecureChannel != NULL)
	#define EXP_SecChServerFinishRequest  CAL_CMEXPAPI( "SecChServerFinishRequest" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChServerFinishRequest  PFSECCHSERVERFINISHREQUEST pfSecChServerFinishRequest;
	#define EXT_SecChServerFinishRequest  extern PFSECCHSERVERFINISHREQUEST pfSecChServerFinishRequest;
	#define GET_SecChServerFinishRequest(fl)  s_pfCMGetAPI2( "SecChServerFinishRequest", (RTS_VOID_FCTPTR *)&pfSecChServerFinishRequest, (fl), 0, 0)
	#define CAL_SecChServerFinishRequest  pfSecChServerFinishRequest
	#define CHK_SecChServerFinishRequest  (pfSecChServerFinishRequest != NULL)
	#define EXP_SecChServerFinishRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChServerFinishRequest", (RTS_UINTPTR)SecChServerFinishRequest, 0, 0) 
#endif




/**
 * <description>
 *  Called by the CmpSecureChannel if data needs to be sent to the network.
 * </description>
 * <param name="pduData" type="IN">Data that has to be sent by the transport layer.</param>
 * <param name="pSendUserData" type="IN">User data given by SecChClientInit.</param>
 */
typedef RTS_RESULT(CDECL *PFSENDDATA)(PROTOCOL_DATA_UNIT pduData, void* pSendUserData);

/**
 * <description>
 *  Called by the CmpSecureChannel if decrypted data is available for processing.
 * </description>
 * <param name="pduData" type="IN">Decrypted data to be processed by the client.</param>
 * <param name="pReceiveUserData" type="IN">User data given by SecChClientInit.</param>
 */
typedef RTS_RESULT(CDECL *PFDATARECEIVED)(PROTOCOL_DATA_UNIT pduData, void* pReceiveUserData);

/**
 * <description>Initialize a new secure channel.</description>
 * <param name="pSendPDU" type="IN">Function used by the secure channel to send data.</param>
 * <param name="pSendUserData" type="IN">Parameter passed by the secure channel to the send function.</param>
 * <param name="pfDataReceived" type="IN">Function used by the secure channel to send decrypted data to the client.</param>
 * <param name="pReceiveUserData" type="IN">Parameter passed by the secure channel to the client when data was decrypted.</param>
 * <param name="hTlsContext" type="IN">TLS Context used to establish the TLS connection. See CmpTlsCreateContext.</param>
 * <param name="ui32ChannelSize" type="IN">Size of the communication channel</param>
 * <param name="bPLCBigEndian" type="IN">Flag whether the PLC has Motorola byte order or not.</param>
 * <param name="pResult" type="OUT">error code</param>
 */
RTS_HANDLE CDECL SecChClientInit(PFSENDDATA pSendPDU, void *pSendUserData, PFDATARECEIVED pfDataReceived, void *pReceiveUserData, RTS_HANDLE hTlsContext, RTS_UI32 ui32ChannelSize, RTS_BOOL bPLCBigEndian, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSECCHCLIENTINIT) (PFSENDDATA pSendPDU, void *pSendUserData, PFDATARECEIVED pfDataReceived, void *pReceiveUserData, RTS_HANDLE hTlsContext, RTS_UI32 ui32ChannelSize, RTS_BOOL bPLCBigEndian, RTS_RESULT *pResult);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHCLIENTINIT_NOTIMPLEMENTED)
	#define USE_SecChClientInit
	#define EXT_SecChClientInit
	#define GET_SecChClientInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChClientInit(p0,p1,p2,p3,p4,p5,p6,p7)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SecChClientInit  FALSE
	#define EXP_SecChClientInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChClientInit
	#define EXT_SecChClientInit
	#define GET_SecChClientInit(fl)  CAL_CMGETAPI( "SecChClientInit" ) 
	#define CAL_SecChClientInit  SecChClientInit
	#define CHK_SecChClientInit  TRUE
	#define EXP_SecChClientInit  CAL_CMEXPAPI( "SecChClientInit" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChClientInit
	#define EXT_SecChClientInit
	#define GET_SecChClientInit(fl)  CAL_CMGETAPI( "SecChClientInit" ) 
	#define CAL_SecChClientInit  SecChClientInit
	#define CHK_SecChClientInit  TRUE
	#define EXP_SecChClientInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChClientInit", (RTS_UINTPTR)SecChClientInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChClientInit
	#define EXT_CmpSecureChannelSecChClientInit
	#define GET_CmpSecureChannelSecChClientInit  ERR_OK
	#define CAL_CmpSecureChannelSecChClientInit  ((ICmpSecureChannel*)s_pfCMCreateInstance(CLASSID_CCmpSecureChannel, NULL))->ISecChClientInit
	#define CHK_CmpSecureChannelSecChClientInit	(s_pfCMCreateInstance != NULL && pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChClientInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChClientInit
	#define EXT_SecChClientInit
	#define GET_SecChClientInit(fl)  CAL_CMGETAPI( "SecChClientInit" ) 
	#define CAL_SecChClientInit  ((ICmpSecureChannel*)s_pfCMCreateInstance(CLASSID_CCmpSecureChannel, NULL))->ISecChClientInit
	#define CHK_SecChClientInit	(s_pfCMCreateInstance != NULL && pICmpSecureChannel != NULL)
	#define EXP_SecChClientInit  CAL_CMEXPAPI( "SecChClientInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChClientInit  PFSECCHCLIENTINIT pfSecChClientInit;
	#define EXT_SecChClientInit  extern PFSECCHCLIENTINIT pfSecChClientInit;
	#define GET_SecChClientInit(fl)  s_pfCMGetAPI2( "SecChClientInit", (RTS_VOID_FCTPTR *)&pfSecChClientInit, (fl), 0, 0)
	#define CAL_SecChClientInit  pfSecChClientInit
	#define CHK_SecChClientInit  (pfSecChClientInit != NULL)
	#define EXP_SecChClientInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChClientInit", (RTS_UINTPTR)SecChClientInit, 0, 0) 
#endif




/**
 * <description>Cleanup the SecureChannel Client.</description>
 * <param name="hSecureChannel" type="IN">Id of the channel.</param>
 */
RTS_RESULT CDECL SecChClientCleanup(RTS_HANDLE hSecureChannel);
typedef RTS_RESULT (CDECL * PFSECCHCLIENTCLEANUP) (RTS_HANDLE hSecureChannel);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHCLIENTCLEANUP_NOTIMPLEMENTED)
	#define USE_SecChClientCleanup
	#define EXT_SecChClientCleanup
	#define GET_SecChClientCleanup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChClientCleanup(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecChClientCleanup  FALSE
	#define EXP_SecChClientCleanup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChClientCleanup
	#define EXT_SecChClientCleanup
	#define GET_SecChClientCleanup(fl)  CAL_CMGETAPI( "SecChClientCleanup" ) 
	#define CAL_SecChClientCleanup  SecChClientCleanup
	#define CHK_SecChClientCleanup  TRUE
	#define EXP_SecChClientCleanup  CAL_CMEXPAPI( "SecChClientCleanup" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChClientCleanup
	#define EXT_SecChClientCleanup
	#define GET_SecChClientCleanup(fl)  CAL_CMGETAPI( "SecChClientCleanup" ) 
	#define CAL_SecChClientCleanup  SecChClientCleanup
	#define CHK_SecChClientCleanup  TRUE
	#define EXP_SecChClientCleanup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChClientCleanup", (RTS_UINTPTR)SecChClientCleanup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChClientCleanup
	#define EXT_CmpSecureChannelSecChClientCleanup
	#define GET_CmpSecureChannelSecChClientCleanup  ERR_OK
	#define CAL_CmpSecureChannelSecChClientCleanup(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ICmpSecureChannel*)p0)->ISecChClientCleanup())
	#define CHK_CmpSecureChannelSecChClientCleanup  TRUE
	#define EXP_CmpSecureChannelSecChClientCleanup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChClientCleanup
	#define EXT_SecChClientCleanup
	#define GET_SecChClientCleanup(fl)  CAL_CMGETAPI( "SecChClientCleanup" ) 
	#define CAL_SecChClientCleanup(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ICmpSecureChannel*)p0)->ISecChClientCleanup())
	#define CHK_SecChClientCleanup  TRUE
	#define EXP_SecChClientCleanup  CAL_CMEXPAPI( "SecChClientCleanup" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChClientCleanup  PFSECCHCLIENTCLEANUP pfSecChClientCleanup;
	#define EXT_SecChClientCleanup  extern PFSECCHCLIENTCLEANUP pfSecChClientCleanup;
	#define GET_SecChClientCleanup(fl)  s_pfCMGetAPI2( "SecChClientCleanup", (RTS_VOID_FCTPTR *)&pfSecChClientCleanup, (fl), 0, 0)
	#define CAL_SecChClientCleanup  pfSecChClientCleanup
	#define CHK_SecChClientCleanup  (pfSecChClientCleanup != NULL)
	#define EXP_SecChClientCleanup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChClientCleanup", (RTS_UINTPTR)SecChClientCleanup, 0, 0) 
#endif




/**
 * <description>Send a complete L7 service.</description>
 * <param name="hSecureChannel" type="IN">Id of the channel.</param>
 * <param name="pduData" type="IN">Service to send. Must be a complete L7 service.</param>
 */
RTS_RESULT CDECL SecChClientSendData(RTS_HANDLE hSecureChannel, PROTOCOL_DATA_UNIT pduData);
typedef RTS_RESULT (CDECL * PFSECCHCLIENTSENDDATA) (RTS_HANDLE hSecureChannel, PROTOCOL_DATA_UNIT pduData);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHCLIENTSENDDATA_NOTIMPLEMENTED)
	#define USE_SecChClientSendData
	#define EXT_SecChClientSendData
	#define GET_SecChClientSendData(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChClientSendData(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecChClientSendData  FALSE
	#define EXP_SecChClientSendData  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChClientSendData
	#define EXT_SecChClientSendData
	#define GET_SecChClientSendData(fl)  CAL_CMGETAPI( "SecChClientSendData" ) 
	#define CAL_SecChClientSendData  SecChClientSendData
	#define CHK_SecChClientSendData  TRUE
	#define EXP_SecChClientSendData  CAL_CMEXPAPI( "SecChClientSendData" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChClientSendData
	#define EXT_SecChClientSendData
	#define GET_SecChClientSendData(fl)  CAL_CMGETAPI( "SecChClientSendData" ) 
	#define CAL_SecChClientSendData  SecChClientSendData
	#define CHK_SecChClientSendData  TRUE
	#define EXP_SecChClientSendData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChClientSendData", (RTS_UINTPTR)SecChClientSendData, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChClientSendData
	#define EXT_CmpSecureChannelSecChClientSendData
	#define GET_CmpSecureChannelSecChClientSendData  ERR_OK
	#define CAL_CmpSecureChannelSecChClientSendData(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pICmpSecureChannel->ISecChClientSendData(p1) : ((ICmpSecureChannel*)p0)->ISecChClientSendData(p1))
	#define CHK_CmpSecureChannelSecChClientSendData  (pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChClientSendData  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChClientSendData
	#define EXT_SecChClientSendData
	#define GET_SecChClientSendData(fl)  CAL_CMGETAPI( "SecChClientSendData" ) 
	#define CAL_SecChClientSendData(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pICmpSecureChannel->ISecChClientSendData(p1) : ((ICmpSecureChannel*)p0)->ISecChClientSendData(p1))
	#define CHK_SecChClientSendData  (pICmpSecureChannel != NULL)
	#define EXP_SecChClientSendData  CAL_CMEXPAPI( "SecChClientSendData" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChClientSendData  PFSECCHCLIENTSENDDATA pfSecChClientSendData;
	#define EXT_SecChClientSendData  extern PFSECCHCLIENTSENDDATA pfSecChClientSendData;
	#define GET_SecChClientSendData(fl)  s_pfCMGetAPI2( "SecChClientSendData", (RTS_VOID_FCTPTR *)&pfSecChClientSendData, (fl), 0, 0)
	#define CAL_SecChClientSendData  pfSecChClientSendData
	#define CHK_SecChClientSendData  (pfSecChClientSendData != NULL)
	#define EXP_SecChClientSendData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChClientSendData", (RTS_UINTPTR)SecChClientSendData, 0, 0) 
#endif




/**
 * <description>Function that is called when a new response arrived. This function will decrypt the data an then call pfDataReceived.
 * If there is the need to do some handshaking within the TLS connection this will be handled by this function. The data is sent
 * using pSendPDU in this case.</description>
 * <param name="hSecureChannel" type="IN">Id of the channel.</param>
 * <param name="pduData" type="IN">Service received from the network.</param>
 */
RTS_RESULT CDECL SecChClientDataReceived(RTS_HANDLE hSecureChannel, PROTOCOL_DATA_UNIT pduData);
typedef RTS_RESULT (CDECL * PFSECCHCLIENTDATARECEIVED) (RTS_HANDLE hSecureChannel, PROTOCOL_DATA_UNIT pduData);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHCLIENTDATARECEIVED_NOTIMPLEMENTED)
	#define USE_SecChClientDataReceived
	#define EXT_SecChClientDataReceived
	#define GET_SecChClientDataReceived(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChClientDataReceived(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecChClientDataReceived  FALSE
	#define EXP_SecChClientDataReceived  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChClientDataReceived
	#define EXT_SecChClientDataReceived
	#define GET_SecChClientDataReceived(fl)  CAL_CMGETAPI( "SecChClientDataReceived" ) 
	#define CAL_SecChClientDataReceived  SecChClientDataReceived
	#define CHK_SecChClientDataReceived  TRUE
	#define EXP_SecChClientDataReceived  CAL_CMEXPAPI( "SecChClientDataReceived" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChClientDataReceived
	#define EXT_SecChClientDataReceived
	#define GET_SecChClientDataReceived(fl)  CAL_CMGETAPI( "SecChClientDataReceived" ) 
	#define CAL_SecChClientDataReceived  SecChClientDataReceived
	#define CHK_SecChClientDataReceived  TRUE
	#define EXP_SecChClientDataReceived  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChClientDataReceived", (RTS_UINTPTR)SecChClientDataReceived, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChClientDataReceived
	#define EXT_CmpSecureChannelSecChClientDataReceived
	#define GET_CmpSecureChannelSecChClientDataReceived  ERR_OK
	#define CAL_CmpSecureChannelSecChClientDataReceived(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pICmpSecureChannel->ISecChClientDataReceived(p1) : ((ICmpSecureChannel*)p0)->ISecChClientDataReceived(p1))
	#define CHK_CmpSecureChannelSecChClientDataReceived  (pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChClientDataReceived  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChClientDataReceived
	#define EXT_SecChClientDataReceived
	#define GET_SecChClientDataReceived(fl)  CAL_CMGETAPI( "SecChClientDataReceived" ) 
	#define CAL_SecChClientDataReceived(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pICmpSecureChannel->ISecChClientDataReceived(p1) : ((ICmpSecureChannel*)p0)->ISecChClientDataReceived(p1))
	#define CHK_SecChClientDataReceived  (pICmpSecureChannel != NULL)
	#define EXP_SecChClientDataReceived  CAL_CMEXPAPI( "SecChClientDataReceived" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChClientDataReceived  PFSECCHCLIENTDATARECEIVED pfSecChClientDataReceived;
	#define EXT_SecChClientDataReceived  extern PFSECCHCLIENTDATARECEIVED pfSecChClientDataReceived;
	#define GET_SecChClientDataReceived(fl)  s_pfCMGetAPI2( "SecChClientDataReceived", (RTS_VOID_FCTPTR *)&pfSecChClientDataReceived, (fl), 0, 0)
	#define CAL_SecChClientDataReceived  pfSecChClientDataReceived
	#define CHK_SecChClientDataReceived  (pfSecChClientDataReceived != NULL)
	#define EXP_SecChClientDataReceived  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChClientDataReceived", (RTS_UINTPTR)SecChClientDataReceived, 0, 0) 
#endif




/**
 * <description>This function calculates the maximum secure channel size from the network channel size.</description>
 * <param name="ui32ChannelSize" type="IN">Size of the network channel.</param>
 */
RTS_UI32 CDECL SecChGetSecureChannelSize(RTS_UI32 ui32ChannelSize);
typedef RTS_UI32 (CDECL * PFSECCHGETSECURECHANNELSIZE) (RTS_UI32 ui32ChannelSize);
#if defined(CMPSECURECHANNEL_NOTIMPLEMENTED) || defined(SECCHGETSECURECHANNELSIZE_NOTIMPLEMENTED)
	#define USE_SecChGetSecureChannelSize
	#define EXT_SecChGetSecureChannelSize
	#define GET_SecChGetSecureChannelSize(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecChGetSecureChannelSize(p0)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SecChGetSecureChannelSize  FALSE
	#define EXP_SecChGetSecureChannelSize  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecChGetSecureChannelSize
	#define EXT_SecChGetSecureChannelSize
	#define GET_SecChGetSecureChannelSize(fl)  CAL_CMGETAPI( "SecChGetSecureChannelSize" ) 
	#define CAL_SecChGetSecureChannelSize  SecChGetSecureChannelSize
	#define CHK_SecChGetSecureChannelSize  TRUE
	#define EXP_SecChGetSecureChannelSize  CAL_CMEXPAPI( "SecChGetSecureChannelSize" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURECHANNEL_EXTERNAL)
	#define USE_SecChGetSecureChannelSize
	#define EXT_SecChGetSecureChannelSize
	#define GET_SecChGetSecureChannelSize(fl)  CAL_CMGETAPI( "SecChGetSecureChannelSize" ) 
	#define CAL_SecChGetSecureChannelSize  SecChGetSecureChannelSize
	#define CHK_SecChGetSecureChannelSize  TRUE
	#define EXP_SecChGetSecureChannelSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChGetSecureChannelSize", (RTS_UINTPTR)SecChGetSecureChannelSize, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecureChannelSecChGetSecureChannelSize
	#define EXT_CmpSecureChannelSecChGetSecureChannelSize
	#define GET_CmpSecureChannelSecChGetSecureChannelSize  ERR_OK
	#define CAL_CmpSecureChannelSecChGetSecureChannelSize pICmpSecureChannel->ISecChGetSecureChannelSize
	#define CHK_CmpSecureChannelSecChGetSecureChannelSize (pICmpSecureChannel != NULL)
	#define EXP_CmpSecureChannelSecChGetSecureChannelSize  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecChGetSecureChannelSize
	#define EXT_SecChGetSecureChannelSize
	#define GET_SecChGetSecureChannelSize(fl)  CAL_CMGETAPI( "SecChGetSecureChannelSize" ) 
	#define CAL_SecChGetSecureChannelSize pICmpSecureChannel->ISecChGetSecureChannelSize
	#define CHK_SecChGetSecureChannelSize (pICmpSecureChannel != NULL)
	#define EXP_SecChGetSecureChannelSize  CAL_CMEXPAPI( "SecChGetSecureChannelSize" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecChGetSecureChannelSize  PFSECCHGETSECURECHANNELSIZE pfSecChGetSecureChannelSize;
	#define EXT_SecChGetSecureChannelSize  extern PFSECCHGETSECURECHANNELSIZE pfSecChGetSecureChannelSize;
	#define GET_SecChGetSecureChannelSize(fl)  s_pfCMGetAPI2( "SecChGetSecureChannelSize", (RTS_VOID_FCTPTR *)&pfSecChGetSecureChannelSize, (fl), 0, 0)
	#define CAL_SecChGetSecureChannelSize  pfSecChGetSecureChannelSize
	#define CHK_SecChGetSecureChannelSize  (pfSecChGetSecureChannelSize != NULL)
	#define EXP_SecChGetSecureChannelSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecChGetSecureChannelSize", (RTS_UINTPTR)SecChGetSecureChannelSize, 0, 0) 
#endif





#ifdef __cplusplus
}
#endif




typedef struct
{
	IBase_C *pBase;
	PFSECCHSERVERHANDLEREQUEST ISecChServerHandleRequest;
 	PFSECCHSERVERGETMAXCHANNELS ISecChServerGetMaxChannels;
 	PFSECCHSERVERGETCHANNELINFOBYINDEX ISecChServerGetChannelInfoByIndex;
 	PFSECCHSERVERGETSTATUS ISecChServerGetStatus;
 	PFSECCHSERVERGETREQUEST ISecChServerGetRequest;
 	PFSECCHSERVERFINISHREQUEST ISecChServerFinishRequest;
 	PFSECCHCLIENTINIT ISecChClientInit;
 	PFSECCHCLIENTCLEANUP ISecChClientCleanup;
 	PFSECCHCLIENTSENDDATA ISecChClientSendData;
 	PFSECCHCLIENTDATARECEIVED ISecChClientDataReceived;
 	PFSECCHGETSECURECHANNELSIZE ISecChGetSecureChannelSize;
 } ICmpSecureChannel_C;

#ifdef CPLUSPLUS
class ICmpSecureChannel : public IBase
{
	public:
		virtual RTS_RESULT CDECL ISecChServerHandleRequest(RTS_UI32 ui32ChannelHandle, PROTOCOL_DATA_UNIT pduRequest, PROTOCOL_DATA_UNIT pduReply, RTS_UI32 bFirstCall) =0;
		virtual RTS_RESULT CDECL ISecChServerGetMaxChannels(RTS_UI16 *pui16MaxChannels) =0;
		virtual RTS_RESULT CDECL ISecChServerGetChannelInfoByIndex(RTS_UI16 ui16ChannelIndex, RTS_UI32 *pui32ServerState, CHANNELINFO *pChInfoBuffer, RTS_SIZE *psiBufferLen) =0;
		virtual RTS_RESULT CDECL ISecChServerGetStatus(RTS_UI32 ui32ChannelHandle, RTS_UI16 *pui16Status, RTS_UI8 *pui8ScalingFactor, RTS_I32 *pi32ItemsComplete, RTS_I32 *pi32TotalItems) =0;
		virtual RTS_RESULT CDECL ISecChServerGetRequest(RTS_UI32 ui32ChannelHandle, PROTOCOL_DATA_UNIT *ppduRequest, PROTOCOL_DATA_UNIT *ppduReplyBuffer) =0;
		virtual RTS_RESULT CDECL ISecChServerFinishRequest(RTS_UI32 ui32ChannelHandle, PROTOCOL_DATA_UNIT pduData, RTS_BOOL closeChannel, RTS_RESULT closeReason) =0;
		virtual RTS_HANDLE CDECL ISecChClientInit(PFSENDDATA pSendPDU, void *pSendUserData, PFDATARECEIVED pfDataReceived, void *pReceiveUserData, RTS_HANDLE hTlsContext, RTS_UI32 ui32ChannelSize, RTS_BOOL bPLCBigEndian, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISecChClientCleanup(void) =0;
		virtual RTS_RESULT CDECL ISecChClientSendData(PROTOCOL_DATA_UNIT pduData) =0;
		virtual RTS_RESULT CDECL ISecChClientDataReceived(PROTOCOL_DATA_UNIT pduData) =0;
		virtual RTS_UI32 CDECL ISecChGetSecureChannelSize(RTS_UI32 ui32ChannelSize) =0;
};
	#ifndef ITF_CmpSecureChannel
		#define ITF_CmpSecureChannel static ICmpSecureChannel *pICmpSecureChannel = NULL;
	#endif
	#define EXTITF_CmpSecureChannel
#else	/*CPLUSPLUS*/
	typedef ICmpSecureChannel_C		ICmpSecureChannel;
	#ifndef ITF_CmpSecureChannel
		#define ITF_CmpSecureChannel
	#endif
	#define EXTITF_CmpSecureChannel
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPSECURECHANNELITF_H_*/
