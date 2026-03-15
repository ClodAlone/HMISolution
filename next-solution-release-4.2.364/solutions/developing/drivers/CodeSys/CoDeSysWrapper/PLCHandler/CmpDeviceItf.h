 /**
 * <interfacename>CmpDevice</interfacename>
 * <description> 
 *	<p>Interface for the device component. This is the first component, that contacts a client to get
 *	online access to the target.
 *	Here for example the target identification is checked.</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPDEVICEITF_H_
#define _CMPDEVICEITF_H_

#include "CmpStd.h"

 

 




/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  Setting to store the current operation mode of the PLC.
 *  For possible values see category Device operation mode.
 *  Not a configuration option - only to be set/changend by the runtime system.
 * </description>
 */
#define DEVKEY_INT_OPERATION_MODE						"OperationMode"
#ifndef DEVVALUE_INT_OPERATION_MODE_DEFAULT
	#define DEVVALUE_INT_OPERATION_MODE_DEFAULT			DEV_OM_DEBUG
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  Setting to store some addtional information for some operation modes, to be checked at startup.
 *  Not a configuration option - only to be set/changed/removed by the runtime system.
 * </description>
 */
#define DEVKEY_INT_OPERATION_MODE_CHECK					"OperationModeCheck"


/**
 * <category>Operations</category>
 * <description>
 *  Operations of the device component. Can be disabled with the event CMPID_CmpMgr::EVT_CmpMgr_DisableOperation!
 *	<ul>
 *		<li>OP_DEVICE_SET_OPERATION_MODE: Changes the operation mode of the PLC (see category: Device operation mode). </li>
 *	</ul>
 * </description>
 */
#define OP_DEVICE_SET_OPERATION_MODE				1


/**
 * <category>Device operation mode</category>
 * <description>
 *  The operation mode describes the global operation mode of a PLC. By default a PLC is in debug mode. In this mode all online actions like download, 
 *  debugging and so on are allowed. Other modes restrict some online services to prevent unintended changes by the user on a running machine.
 *
 *	NOTE:
 *	This feature must be activated for each mode in the device description of the target with the following target settings:
 *		- "locked_mode_supported": for DEV_OM_LOCKED
 *		- "operational_mode_supported": for DEV_OM_OPERATIONAL
 *	in the section "runtime_features" (see description of the target settings in the runtiuem documentation).
 *	With this enabled target settings, the end user can specify the operation mode in the programming system with the menu commands "Online" / "Operating Mode".
 *
 *	NOTE:
 *	To change the operation mode online on the controller, the onlineuser need the USERDB_RIGHT_MODIFY right on the device node (typically Administrator rights)!
 *
 *	Here the available operation modes:
 *	<ul>
 *		<li>DEV_OM_NONE: Unspecified operation mode</li>
 *		<li>DEV_OM_DEBUG: Device is in debug mode: All operations are allowed</li>
 *		<li>DEV_OM_LOCKED: Device is locked: Previously set breakpoints and forced variables are further active, (file-)download and OnlineChange is blocked.
 *			NOTE: Target setting "locked_mode_supported" must be enabled in the device description of the target to use this feature!
 *		</li>
 *		<li>DEV_OM_OPERATIONAL: Device is operational: Debug, force, (file-)download and OnlineChange actions are blocked. This should be the operation mode of a machine in the 
 *		production process!
 *			NOTE: Target setting "operational_mode_supported" must be enabled in the device description of the target to use this feature!
 *		</li>
 *	</ul>
 * </description>
 */
#define DEV_OM_NONE				UINT32_C(0x00000000)
#define DEV_OM_DEBUG			UINT32_C(0x00000001)
#define DEV_OM_LOCKED			UINT32_C(0x00000002)
#define DEV_OM_OPERATIONAL		UINT32_C(0x00000003)

/**
 * <category>Interactive login commands</category>
 * <description>
 * 	
 *	<ul>
 *		<li>DEV_ILC_NONE: Unspecified login command. Only used for preinitialization, not in a service.</li>
 *		<li>DEV_ILC_IDENTIFY: The device should send a signal to its user interface (e. g. beep, blinking LED, ...) to identify it.</li>
 *		<li>DEV_ILC_LOGIN_REQUEST: Starts an interactive login. The mode is specified by the category Interactive login mode flags.</li>
 *		<li>DEV_ILC_LOGIN_POLL_RESULT: Polls the result of the interactive login, if it was not synchronously handled during request. 
 *          This is repeated until the device sends another result as ERR_PENDING or the user cancels the login </li>
 *		<li>DEV_ILC_LOGIN_CANCEL: Login was canceled by the user </li>
 *	</ul>
 * </description>
 */
#define DEV_ILC_NONE				UINT32_C(0x00000000)
#define DEV_ILC_IDENTIFY			UINT32_C(0x00000001)
#define DEV_ILC_LOGIN_REQUEST		UINT32_C(0x00000002)
#define DEV_ILC_LOGIN_POLL_RESULT	UINT32_C(0x00000003)
#define DEV_ILC_LOGIN_CANCEL		UINT32_C(0x00000004)

/**
 * <category>Interactive login mode flags</category>
 * <description>
 * 	
 *	<ul>
 *		<li>DEV_ILMF_NONE: Unspecified login mode. Only used by the event for all login commands except DEV_ILC_LOGIN_REQUEST</li>
 *		<li>DEV_ILMF_ID: Data (string) to be compared on the device.</li>
 *		<li>DEV_ILMF_KEY_PRESS: Key press is requested. This service have to be answered with ERR_PENDING until the the user presses the key on the device.</li>
 *		<li>DEV_ILMF_BLINK: The device should send a signal to its user interface (e. g. beep, blinking LED, ...) to identify it. </li>
 *		<li>DEV_ILMF_VERIFY_ID: Id verification was triggered in the background without any user interaction. 
 *			If this fails, the request is immediatly repeated on the same session with a user provided id string.</li>
 *	</ul>
 * </description>
 */
#define DEV_ILMF_NONE				UINT32_C(0x00000000)
#define DEV_ILMF_ID					UINT32_C(0x00000001)
#define DEV_ILMF_KEY_PRESS			UINT32_C(0x00000002)
#define DEV_ILMF_BLINK				UINT32_C(0x00000004)
#define DEV_ILMF_VERIFY_ID			UINT32_C(0x80000000)

/* Crypt types */
#define DEVICE_CRYPT_INVALID				0x00000000
#define DEVICE_CRYPT_XOR					0x00000001

/**
 * <category>Online services</category>
 */
#define SRV_DEV_GET_TARGET_IDENT			0x01
#define SRV_DEV_LOGIN						0x02
#define SRV_DEV_LOGOUT						0x03
#define SRV_DEV_RESET_ORIGIN				0x04
#define SRV_DEV_ECHO						0x05
#define SRV_DEV_SET_OPERATION_MODE			0x06
#define SRV_DEV_GET_OPERATION_MODE			0x07
#define SRV_DEV_INTERACTIVE_LOGIN			0x08
#define SRV_DEV_SET_NODE_NAME				0x09

/**
 * <category>Online service tags</category>
 */
#define TAG_DEV_REPLY_IDENTIFICATION		0x01
#define TAG_DEV_REPLY_DEVICE_NAME			0x02
#define TAG_DEV_REPLY_VENDOR_NAME			0x03
#define TAG_DEV_REPLY_NODE_NAME				0x04
#define TAG_DEV_REPLY						0x05
#define TAG_DEV_REPLY_RTS_VERSION			0x06
#define TAG_DEV_REPLY_FLAGS					0x07
#define TAG_DEV_REPLY_NUM_CHANNELS			0x08
#define TAG_DEV_REPLY_SERIAL_NUMBER			0x09
#define TAG_DEV_REPLY_IP_ADDR_INFO			0x0A

#define TAG_DEV_REPLY_LOGIN					0x82
#define TAG_DEV_REPLY_LOGIN_RESULT			0x20
#define TAG_DEV_REPLY_LOGIN_SESSIONID		0x21

#define TAG_DEV_CRYPT_TYPE					0x22
#define TAG_DEV_CRYPT_CHALLENGE				0x23
#define TAG_DEV_REPLY_SETTINGS				0x24

#define TAG_DEV_RESET_ORIGIN_CONFIG			0x31

#define TAG_DEV_REPLY_LOGOUT				0x00

#define TAG_DEV_CREDENTIALS					0x81
#define TAG_DEV_USER						0x10
#define TAG_DEV_PASSWORD					0x11

#define TAG_DEV_SESSIONID					0x01

#define TAG_DEV_ECHO_REQUEST_DATA_LEN		0x51
#define TAG_DEV_ECHO_REPLY_DATA_LEN			0x52
#define TAG_DEV_ECHO_DATA					0x53
#define TAG_DEV_OPERATION_MODE				0x54
#define TAG_DEV_INTERACTIVE_LOGIN_MODE		0x55
#define TAG_DEV_INTERACTIVE_LOGIN_DATA		0x56
#define TAG_DEV_INTERACTIVE_LOGIN_COMMAND	0x57
#define TAG_DEV_NODE_NAME					0x58


/**
 * <category>Online services</category>
 * <Description>
 *	Echo service. Receives services and send replys with dummy bytes as payload. If the tag TAG_DEV_ECHO_REPLY_DATA_LEN is omitted in
 *  the request, the server sends an empty service reply without any tag. If the requested reply length in TAG_DEV_ECHO_REPLY_DATA_LEN
 *  exceeds the communciation buffer, then the length is limited to a value, which completely fills the communciation buffer. If the requested
 *  reply length is greater than the request length, then dummy bytes are generated by the server. If not, the payload is copied starting from
 *  the beginning of the request payload in TAG_DEV_ECHO_DATA.
 * </Description>
 * <service name="SRV_DEV_ECHO">
 *	<Request>
 *		<tag name="TAG_DEV_ECHO_REQUEST_DATA_LEN" required="optional">[RTS_UI32]: Length of tag content in TAG_DEV_ECHO_DATA.</tag>
 *		<tag name="TAG_DEV_ECHO_REPLY_DATA_LEN" required="optional">[RTS_UI32]: Requested length of reply content in TAG_DEV_ECHO_DATA.</tag>
 *		<tag name="TAG_DEV_ECHO_DATA" required="optional">[char*]: Dummy bytes as payload. </tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_SERVICE_RESULT" required="optional">[RTS_UI16]: Result code of online operation</tag>
 *		<tag name="TAG_DEV_ECHO_REPLY_DATA_LEN" required="optional">[RTS_UI32]: Length of tag content in TAG_DEV_ECHO_DATA </tag>
 *		<tag name="TAG_DEV_ECHO_DATA" required="optional">[char*]: Dummy bytes as payload. </tag>
 *	</Response>
 * </service>
 */

/**
 * <category>Online services</category>
 * <Description>
 *	Sets one of the device operation modes of the PLC (see category: Device operation mode). Changing the operation mode may be denied by the PLC.
 * </Description>
 * <service name="SRV_DEV_SET_OPERATION_MODE">
 *	<Request>
 *		<tag name="TAG_DEV_OPERATION_MODE" required="optional">[RTS_UI32]: Operation mode to set.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_SERVICE_RESULT" required="mandatory">[RTS_UI6]: Result code of online operation</tag>
 *		<tag name="TAG_USER_NOTIFY" required="optional">[LogEntry]: User notification message, contains the component, which has denied the service</tag>
 *	</Response>
 * </service>
 */

/**
 * <category>Online services</category>
 * <Description>
 *	Retrieves the device operation modes of the PLC (see category: Device operation mode).
 * </Description>
 * <service name="SRV_DEV_GET_OPERATION_MODE">
 *	<Request>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_DEV_OPERATION_MODE" required="mandatory">[RTS_UI32]: Current operation mode of the device.</tag>
 *	</Response>
 * </service>
 */

/**
 * <category>Online services</category>
 * <Description>
 *  Interactive login service. Helps to identify a PLC before login. 
 * </Description>
 * <service name="SRV_DEV_INTERACTIVE_LOGIN">
 *	<Request>
 *		<tag name="TAG_DEV_INTERACTIVE_LOGIN_COMMAND" required="mandatory">[RTS_UI32]: See category "Interactive login commands".</tag>
 *		<tag name="TAG_DEV_INTERACTIVE_LOGIN_MODE" required="optional">[RTS_UI32]: See category Interactive login mode flags. Only used for command DEV_ILC_LOGIN_REQUEST.</tag>
 *		<tag name="TAG_DEV_INTERACTIVE_LOGIN_DATA" required="optional">[RTS_UI8*]: Login information to be checked by the runtime system. Only used for mode DEV_ILM_STRING</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_SERVICE_RESULT" required="mandatory">[RTS_UI16]: Result code of online operation. Supported results: 
 *			ERR_OK (interactive login successful), 
 *			ERR_FAILED (generic error), 
 *			ERR_TIMEOUT (timeout elapsed on device side), 
 *			ERR_PENDING (device still waits for user action, e. g. key press), 
 *			ERR_NOT_SUPPORTED (requested login mode is not supported by the device) 
 *			ERR_ID_MISMATCH (login information does not match).</tag>
 *		<tag name="TAG_DEV_REPLY_LOGIN_SESSIONID" required="optional">[RTS_UI32]: SessionID which have to be used for all further online services on this channel. 
 *			Only returned for command DEV_ILC_LOGIN_REQUEST</tag>
 *	</Response>
 * </service>
 */
 
 /**
 * <category>Online services</category>
 * <Description>
 *	Sets a new UTF-16 node name for the device.
 * </Description>
 * <service name="SRV_DEV_SET_NODE_NAME">
 *	<Request>
 *		<tag name="TAG_DEV_NODE_NAME" required="mandatory">[RTS_UI32]: New UTF-16 node name. By passing an empty string, the node name settings are
 *      deleted and so the default node name is set again.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_SERVICE_RESULT" required="mandatory">[RTS_UI6]: Result code of online operation. See description of SysTargetSetNodeName for details.</tag>
 *	</Response>
 * </service>
 */

 /**
 * <category>Online services</category>
 * <Description>
 *  Reset Origin [Device]. Reset the device to factory settings.
 * </Description>
 * <service name="SRV_DEV_RESET_ORIGIN">
 *	<Request>
 *		<tag name="TAG_DEV_RESET_ORIGIN_CONFIG" required="optional">[RTS_UI32]: See category "Reset Origin Configuration". If this tag is omitted DEV_ROC_RESET_ALL is assumed.</tag>
 *	</Request>
 *	<Response>
 *		None
 *	</Response>
 * </service>
 */

/**
 * <category>Event parameter</category>
 * <element name="ui32LoginCommand" type="IN">Login command. See category "Interactive login commands" (look for DEV_ILC_xxx defines).</element>
 * <element name="ui32LoginMode" type="IN">Login mode. See category "Interactive login mode flags" (look for DEV_ILMF_xxx defines). Only used for command DEV_ILC_LOGIN_REQUEST.</element>
 * <element name="pui8LoginData" type="IN">Login information to be checked by the runtime system. Only used for mode DEV_ILM_STRING</element>
 * <element name="rLoginResult" type="OUT">Error code of event handler:
 *		<ul>
 *			<li>ERR_OK: Operation succeeded</li>
 *			<li>ERR_ID_MISMATCH: Is returned at DEV_ILMF_ID, if the ID does not match (e.g. the entered serial number is not identical to the target)</li>
 *			<li>ERR_PENDING: Is returned at DEV_ILMF_KEY_PRESS, if we wait for pressing the key</li>
 *		</ul>
 * </element>
 */
typedef struct
{
	RTS_UI32 ui32LoginCommand; 
	RTS_UI32 ui32LoginMode;
	RTS_UI8* pui8LoginData;
	RTS_RESULT rLoginResult;
} EVTPARAM_CmpDevice_InteractiveLogin;
#define EVTPARAMID_CmpDevice_InteractiveLogin	0x0001
#define EVTVERSION_CmpDevice_InteractiveLogin	0x0001

/**
 * <category>Event parameter</category>
 * <element name="ui32ResetOriginConfig" type="IN">Configuration bit vector. See category "Reset Origin Configuration"</element>
 */
typedef struct
{
	RTS_UI32 ui32ResetOriginConfig;
} EVTPARAM_CmpDevice_ResetOrigin;
#define EVTPARAMID_CmpDevice_ResetOrigin	0x0001
#define EVTVERSION_CmpDevice_ResetOrigin	0x0001

/**
 * <category>Event parameter</category>
 * <element name="ui32OperationMode" type="IN">New operation mode of the device. See category Device operation mode for possible values."</element>
 */
typedef struct
{
	RTS_UI32 ui32OperationMode;
} EVTPARAM_CmpDevice_SetOperationMode;
#define EVTPARAMID_CmpDevice_SetOperationMode	0x0001
#define EVTVERSION_CmpDevice_SetOperationMode	0x0001


/**
 * <category>Reset Origin Configuration</category>
 * <description>Configuration of the Reset Origin service &amp; event</description>
 */
#define DEV_ROC_RESET_ALL		 0x00000000
#define DEV_ROC_KEEP_USERDB		 0x00000001

/**
 * <category>Events</category>
 * <description>Event is sent to operate the interactive login or the wink feature.
 *	1. Wink:
 *	   The user can select a device in the Gateway dialog, and let it "blink". This allows to identify a device in the list of devices found in the scan. This may be expecially neccessary while PLC don't have unique or meaningful names yet.
 *	2. Secure Login:
 *     During login, it can be guaranteed that the user is connecting to the correct PLC.
 * </description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevice_InteractiveLogin</param>
 */
#define EVT_CmpDevice_InteractiveLogin			MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Events</category>
 * <description>Event is sent at reset origin device is executed online from a user in CODESYS</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevice_ResetOrigin</param>
 */
#define EVT_CmpDevice_ResetOrigin 				MAKE_EVENTID(EVTCLASS_INFO, 2)

/**
 * <category>Events</category>
 * <description>Event is sent after operation mode has been set</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevice_SetOperationMode</param>
 */
#define EVT_CmpDevice_SetOperationMode 			MAKE_EVENTID(EVTCLASS_INFO, 3)

/**
 * <category>Settings flags</category>
 * <description>Setings flags that re returned by the runtime system</description>
 */
#define SETTINGS_FLAG_KILLTASKONRESET			 0x01
#define SETTINGS_FLAG_COMPLEXMONITORING			 0x02
#define SETTINGS_FLAG_MONITORING2				 0x04
#define SETTINGS_FLAG_TRACE_SUPPORTS_MONITORING2 0x08

/**
 * <category>Static defines</category>
 * <description>Predefined objects in the runtime</description>
 */
#define USERDB_OBJECT_DEVICE		"Device"

typedef struct
{
	unsigned long ulSessionId;
	RTS_HANDLE hUser;
} DeviceSession;

/**
 * <category>Target identification</category>
 * <description>
 *	These values identifies a target completely unique!
 * </description>
 *	<element name="ulTargetType" type="IN">Target type. See category "Device Types" in SysTargetItf.h.</element>
 *	<element name="ulTargetId" type="IN">ID: HiWord = VendorID, LowWord = ID</element>
 *	<element name="ulTargetVersion" type="IN">Version of the target</element>
 */
typedef struct
{
	RTS_UI32 ulTargetType;
	RTS_UI32 ulTargetId;
	RTS_UI32 ulTargetVersion;
} TargetIdent;


/**
 * <category>Device description</category>
 * <description>
 * </description>
 *	<element name="targetIdent" type="IN">Target identification</element>
 *	<element name="pwszDeviceName" type="IN">Pointer to device name</element>
 *	<element name="nMaxDeviceNameLen" type="IN">Maximum length of the device name buffer in unicode characters (not bytes!)</element>
 *	<element name="pwszVendorName" type="IN">Pointer to vendor name</element>
 *	<element name="nMaxVendorNameLen" type="IN">Maximum length of the vendor name buffer in unicode characters (not bytes!)</element>
 *	<element name="pwszNodeName" type="IN">Pointer to node name</element>
 *	<element name="nMaxNodeNameLen" type="IN">Maximum length of the node name buffer in unicode characters (not bytes!)</element>
 */
typedef struct
{
	TargetIdent targetIdent;
	RTS_WCHAR *pwszDeviceName;
	RTS_SIZE nMaxDeviceNameLen;
	RTS_WCHAR *pwszVendorName;
	RTS_SIZE nMaxVendorNameLen;
	RTS_WCHAR *pwszNodeName;
	RTS_SIZE nMaxNodeNameLen;
} DeviceIdentification;


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Retrieves the target identification</description>
 * <param name="pDevIdent" type="OUT">Pointer to identification. Is filled by the function. If string pointer are NULL,
 *	the real size of the strings is returned.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL DevGetIdent(DeviceIdentification *pDevIdent);
typedef RTS_RESULT (CDECL * PFDEVGETIDENT) (DeviceIdentification *pDevIdent);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVGETIDENT_NOTIMPLEMENTED)
	#define USE_DevGetIdent
	#define EXT_DevGetIdent
	#define GET_DevGetIdent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevGetIdent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_DevGetIdent  FALSE
	#define EXP_DevGetIdent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevGetIdent
	#define EXT_DevGetIdent
	#define GET_DevGetIdent(fl)  CAL_CMGETAPI( "DevGetIdent" ) 
	#define CAL_DevGetIdent  DevGetIdent
	#define CHK_DevGetIdent  TRUE
	#define EXP_DevGetIdent  CAL_CMEXPAPI( "DevGetIdent" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevGetIdent
	#define EXT_DevGetIdent
	#define GET_DevGetIdent(fl)  CAL_CMGETAPI( "DevGetIdent" ) 
	#define CAL_DevGetIdent  DevGetIdent
	#define CHK_DevGetIdent  TRUE
	#define EXP_DevGetIdent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetIdent", (RTS_UINTPTR)DevGetIdent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevGetIdent
	#define EXT_CmpDeviceDevGetIdent
	#define GET_CmpDeviceDevGetIdent  ERR_OK
	#define CAL_CmpDeviceDevGetIdent pICmpDevice->IDevGetIdent
	#define CHK_CmpDeviceDevGetIdent (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevGetIdent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevGetIdent
	#define EXT_DevGetIdent
	#define GET_DevGetIdent(fl)  CAL_CMGETAPI( "DevGetIdent" ) 
	#define CAL_DevGetIdent pICmpDevice->IDevGetIdent
	#define CHK_DevGetIdent (pICmpDevice != NULL)
	#define EXP_DevGetIdent  CAL_CMEXPAPI( "DevGetIdent" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevGetIdent  PFDEVGETIDENT pfDevGetIdent;
	#define EXT_DevGetIdent  extern PFDEVGETIDENT pfDevGetIdent;
	#define GET_DevGetIdent(fl)  s_pfCMGetAPI2( "DevGetIdent", (RTS_VOID_FCTPTR *)&pfDevGetIdent, (fl), 0, 0)
	#define CAL_DevGetIdent  pfDevGetIdent
	#define CHK_DevGetIdent  (pfDevGetIdent != NULL)
	#define EXP_DevGetIdent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetIdent", (RTS_UINTPTR)DevGetIdent, 0, 0) 
#endif




/**
 * <description>Checks the compatibility between a requested identification and the target identification</description>
 * <param name="pTargetIdentReq" type="IN">Requested target identification to check. Must not be NULL!</param>
 * <param name="pTargetIdent" type="IN">Own target identity. Can be NULL, then the built in target identification is used</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Target identifications matched</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pTargetIdentReq is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_TYPE_MISMATCH">Type mismatch</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_ID_MISMATCH">ID mismatch</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_VERSION_MISMATCH">Version mismatch. Versions not compatible!</errorcode>
 */
RTS_RESULT CDECL DevCheckTargetIdent(TargetIdent *pTargetIdentReq, TargetIdent *pTargetIdent);
typedef RTS_RESULT (CDECL * PFDEVCHECKTARGETIDENT) (TargetIdent *pTargetIdentReq, TargetIdent *pTargetIdent);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVCHECKTARGETIDENT_NOTIMPLEMENTED)
	#define USE_DevCheckTargetIdent
	#define EXT_DevCheckTargetIdent
	#define GET_DevCheckTargetIdent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevCheckTargetIdent(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_DevCheckTargetIdent  FALSE
	#define EXP_DevCheckTargetIdent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevCheckTargetIdent
	#define EXT_DevCheckTargetIdent
	#define GET_DevCheckTargetIdent(fl)  CAL_CMGETAPI( "DevCheckTargetIdent" ) 
	#define CAL_DevCheckTargetIdent  DevCheckTargetIdent
	#define CHK_DevCheckTargetIdent  TRUE
	#define EXP_DevCheckTargetIdent  CAL_CMEXPAPI( "DevCheckTargetIdent" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevCheckTargetIdent
	#define EXT_DevCheckTargetIdent
	#define GET_DevCheckTargetIdent(fl)  CAL_CMGETAPI( "DevCheckTargetIdent" ) 
	#define CAL_DevCheckTargetIdent  DevCheckTargetIdent
	#define CHK_DevCheckTargetIdent  TRUE
	#define EXP_DevCheckTargetIdent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevCheckTargetIdent", (RTS_UINTPTR)DevCheckTargetIdent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevCheckTargetIdent
	#define EXT_CmpDeviceDevCheckTargetIdent
	#define GET_CmpDeviceDevCheckTargetIdent  ERR_OK
	#define CAL_CmpDeviceDevCheckTargetIdent pICmpDevice->IDevCheckTargetIdent
	#define CHK_CmpDeviceDevCheckTargetIdent (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevCheckTargetIdent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevCheckTargetIdent
	#define EXT_DevCheckTargetIdent
	#define GET_DevCheckTargetIdent(fl)  CAL_CMGETAPI( "DevCheckTargetIdent" ) 
	#define CAL_DevCheckTargetIdent pICmpDevice->IDevCheckTargetIdent
	#define CHK_DevCheckTargetIdent (pICmpDevice != NULL)
	#define EXP_DevCheckTargetIdent  CAL_CMEXPAPI( "DevCheckTargetIdent" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevCheckTargetIdent  PFDEVCHECKTARGETIDENT pfDevCheckTargetIdent;
	#define EXT_DevCheckTargetIdent  extern PFDEVCHECKTARGETIDENT pfDevCheckTargetIdent;
	#define GET_DevCheckTargetIdent(fl)  s_pfCMGetAPI2( "DevCheckTargetIdent", (RTS_VOID_FCTPTR *)&pfDevCheckTargetIdent, (fl), 0, 0)
	#define CAL_DevCheckTargetIdent  pfDevCheckTargetIdent
	#define CHK_DevCheckTargetIdent  (pfDevCheckTargetIdent != NULL)
	#define EXP_DevCheckTargetIdent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevCheckTargetIdent", (RTS_UINTPTR)DevCheckTargetIdent, 0, 0) 
#endif




/**
 * <description> 
 *  Function to set the operation mode of the device. Usually this is called by an online service, received from the developemnt system.
 *  Switching to the new oepration mode may be denied by another component.
 * </description>
 * <element name="ui32Operation" type="IN">New operation mode of the device. See category Device operation mode for possible values.</element>
 * <element name="pCmpIdDisabled" type="OUT">Pointer to ComponentID to get the component, which disabled the operation. Can be NULL.</element>
 * <result>
 *	<ul>
 *		<li>ERR_OK: New operation mode was successfully set</li>
 *		<li>ERR_OPERATION_DENIED: The component returned in pCmpIdDisabled has denied the new operation mode</li>
 *		<li>ERR_FAILED: Operation mode could not be set because of another error</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL DevSetOperationMode(RTS_UI32 ui32OperationMode, CMPID *pCmpIdDisabled);
typedef RTS_RESULT (CDECL * PFDEVSETOPERATIONMODE) (RTS_UI32 ui32OperationMode, CMPID *pCmpIdDisabled);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVSETOPERATIONMODE_NOTIMPLEMENTED)
	#define USE_DevSetOperationMode
	#define EXT_DevSetOperationMode
	#define GET_DevSetOperationMode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevSetOperationMode(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_DevSetOperationMode  FALSE
	#define EXP_DevSetOperationMode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevSetOperationMode
	#define EXT_DevSetOperationMode
	#define GET_DevSetOperationMode(fl)  CAL_CMGETAPI( "DevSetOperationMode" ) 
	#define CAL_DevSetOperationMode  DevSetOperationMode
	#define CHK_DevSetOperationMode  TRUE
	#define EXP_DevSetOperationMode  CAL_CMEXPAPI( "DevSetOperationMode" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevSetOperationMode
	#define EXT_DevSetOperationMode
	#define GET_DevSetOperationMode(fl)  CAL_CMGETAPI( "DevSetOperationMode" ) 
	#define CAL_DevSetOperationMode  DevSetOperationMode
	#define CHK_DevSetOperationMode  TRUE
	#define EXP_DevSetOperationMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevSetOperationMode", (RTS_UINTPTR)DevSetOperationMode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevSetOperationMode
	#define EXT_CmpDeviceDevSetOperationMode
	#define GET_CmpDeviceDevSetOperationMode  ERR_OK
	#define CAL_CmpDeviceDevSetOperationMode pICmpDevice->IDevSetOperationMode
	#define CHK_CmpDeviceDevSetOperationMode (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevSetOperationMode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevSetOperationMode
	#define EXT_DevSetOperationMode
	#define GET_DevSetOperationMode(fl)  CAL_CMGETAPI( "DevSetOperationMode" ) 
	#define CAL_DevSetOperationMode pICmpDevice->IDevSetOperationMode
	#define CHK_DevSetOperationMode (pICmpDevice != NULL)
	#define EXP_DevSetOperationMode  CAL_CMEXPAPI( "DevSetOperationMode" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevSetOperationMode  PFDEVSETOPERATIONMODE pfDevSetOperationMode;
	#define EXT_DevSetOperationMode  extern PFDEVSETOPERATIONMODE pfDevSetOperationMode;
	#define GET_DevSetOperationMode(fl)  s_pfCMGetAPI2( "DevSetOperationMode", (RTS_VOID_FCTPTR *)&pfDevSetOperationMode, (fl), 0, 0)
	#define CAL_DevSetOperationMode  pfDevSetOperationMode
	#define CHK_DevSetOperationMode  (pfDevSetOperationMode != NULL)
	#define EXP_DevSetOperationMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevSetOperationMode", (RTS_UINTPTR)DevSetOperationMode, 0, 0) 
#endif




/**
 * <description> 
 *  Function to get the current operation mode of the device.
 * </description>
 * <element name="pui32Operation" type="OUT">Pointer to return the current operation mode.</element>
 * <result>
 *	<ul>
 *		<li>ERR_OK: Current operation mode was returned in pui32Operation. See category Device operation mode for possible values.</li>
 *		<li>ERR_PARAMETER: pui32Operation was a pointer to NULL.</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL DevGetOperationMode(RTS_UI32 *pui32OperationMode);
typedef RTS_RESULT (CDECL * PFDEVGETOPERATIONMODE) (RTS_UI32 *pui32OperationMode);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVGETOPERATIONMODE_NOTIMPLEMENTED)
	#define USE_DevGetOperationMode
	#define EXT_DevGetOperationMode
	#define GET_DevGetOperationMode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevGetOperationMode(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_DevGetOperationMode  FALSE
	#define EXP_DevGetOperationMode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevGetOperationMode
	#define EXT_DevGetOperationMode
	#define GET_DevGetOperationMode(fl)  CAL_CMGETAPI( "DevGetOperationMode" ) 
	#define CAL_DevGetOperationMode  DevGetOperationMode
	#define CHK_DevGetOperationMode  TRUE
	#define EXP_DevGetOperationMode  CAL_CMEXPAPI( "DevGetOperationMode" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevGetOperationMode
	#define EXT_DevGetOperationMode
	#define GET_DevGetOperationMode(fl)  CAL_CMGETAPI( "DevGetOperationMode" ) 
	#define CAL_DevGetOperationMode  DevGetOperationMode
	#define CHK_DevGetOperationMode  TRUE
	#define EXP_DevGetOperationMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetOperationMode", (RTS_UINTPTR)DevGetOperationMode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevGetOperationMode
	#define EXT_CmpDeviceDevGetOperationMode
	#define GET_CmpDeviceDevGetOperationMode  ERR_OK
	#define CAL_CmpDeviceDevGetOperationMode pICmpDevice->IDevGetOperationMode
	#define CHK_CmpDeviceDevGetOperationMode (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevGetOperationMode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevGetOperationMode
	#define EXT_DevGetOperationMode
	#define GET_DevGetOperationMode(fl)  CAL_CMGETAPI( "DevGetOperationMode" ) 
	#define CAL_DevGetOperationMode pICmpDevice->IDevGetOperationMode
	#define CHK_DevGetOperationMode (pICmpDevice != NULL)
	#define EXP_DevGetOperationMode  CAL_CMEXPAPI( "DevGetOperationMode" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevGetOperationMode  PFDEVGETOPERATIONMODE pfDevGetOperationMode;
	#define EXT_DevGetOperationMode  extern PFDEVGETOPERATIONMODE pfDevGetOperationMode;
	#define GET_DevGetOperationMode(fl)  s_pfCMGetAPI2( "DevGetOperationMode", (RTS_VOID_FCTPTR *)&pfDevGetOperationMode, (fl), 0, 0)
	#define CAL_DevGetOperationMode  pfDevGetOperationMode
	#define CHK_DevGetOperationMode  (pfDevGetOperationMode != NULL)
	#define EXP_DevGetOperationMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetOperationMode", (RTS_UINTPTR)DevGetOperationMode, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFDEVGETIDENT IDevGetIdent;
 	PFDEVCHECKTARGETIDENT IDevCheckTargetIdent;
 	PFDEVSETOPERATIONMODE IDevSetOperationMode;
 	PFDEVGETOPERATIONMODE IDevGetOperationMode;
 } ICmpDevice_C;

#ifdef CPLUSPLUS
class ICmpDevice : public IBase
{
	public:
		virtual RTS_RESULT CDECL IDevGetIdent(DeviceIdentification *pDevIdent) =0;
		virtual RTS_RESULT CDECL IDevCheckTargetIdent(TargetIdent *pTargetIdentReq, TargetIdent *pTargetIdent) =0;
		virtual RTS_RESULT CDECL IDevSetOperationMode(RTS_UI32 ui32OperationMode, CMPID *pCmpIdDisabled) =0;
		virtual RTS_RESULT CDECL IDevGetOperationMode(RTS_UI32 *pui32OperationMode) =0;
};
	#ifndef ITF_CmpDevice
		#define ITF_CmpDevice static ICmpDevice *pICmpDevice = NULL;
	#endif
	#define EXTITF_CmpDevice
#else	/*CPLUSPLUS*/
	typedef ICmpDevice_C		ICmpDevice;
	#ifndef ITF_CmpDevice
		#define ITF_CmpDevice
	#endif
	#define EXTITF_CmpDevice
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPDEVICEITF_H_*/
