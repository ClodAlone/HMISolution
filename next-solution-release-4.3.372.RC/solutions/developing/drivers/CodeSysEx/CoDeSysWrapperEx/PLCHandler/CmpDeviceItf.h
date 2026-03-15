 /**
 * <interfacename>CmpDevice</interfacename>
 * <description> 
 *	<p>Interface for the device component. This is the first component, that contacts a client to get
 *	online access to the target.
 *	Here for example the target identification is checked.</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPDEVICEITF_H_
#define _CMPDEVICEITF_H_

#include "CmpStd.h"

 

 



#include "CmpCommunicationLibItf.h"

#include "CmpBinTagUtilItf.h"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  Setting to store the current operation mode of the PLC.
 *  For possible values see category Device operation mode.
 *  Not a configuration option - only to be set/changed by the runtime system.
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
 *  Setting to store some additional information for some operation modes, to be checked at startup.
 *  Not a configuration option - only to be set/changed/removed by the runtime system.
 * </description>
 */
#define DEVKEY_INT_OPERATION_MODE_CHECK					"OperationModeCheck"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  Global session timeout in seconds.
 *  Default: 120 seconds.
 *  Can be overwritten per SRV_DEV_SESSION_CREATE service with the TAG_DEV_CLIENT_TIMEOUT_MS tag.
 * </description>
 */
#define DEVKEY_INT_SESSION_TIMEOUT_SEC						"SessionTimeoutSec"
#ifndef DEVVALUE_INT_SESSION_TIMEOUT_SEC_DEFAULT
	#define DEVVALUE_INT_SESSION_TIMEOUT_SEC_DEFAULT			120
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Maximum number of sessions per channel.
 * The service requests  SRV_DEV_SESSION_CREATE and SRV_DEV_LOGIN will fail if this limit is reached.
 * </description>
 */
#define DEVKEY_INT_MAX_NUM_CHANNEL_SESSIONS						"MaxNumChannelSessions"
#ifndef DEVVALUE_INT_MAX_NUM_CHANNEL_SESSIONS_DEFAULT
	#define DEVVALUE_MAX_NUM_CHANNEL_SESSIONS_DEFAULT			256
#endif


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
 *	in the section "runtime_features" (see description of the target settings in the runtime documentation).
 *	With this enabled target settings, the end user can specify the operation mode in the programming system with the menu commands "Online" / "Operating Mode".
 *
 *	NOTE:
 *	To change the operation mode online on the controller, the online user need the USERDB_RIGHT_MODIFY right on the device node (typically Administrator rights)!
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
 *	<ul>
 *		<li>DEV_ILC_NONE: Unspecified login command. Only used for pre-initialization, not in a service.</li>
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
 *	<ul>
 *		<li>DEV_ILMF_NONE: Unspecified login mode. Only used by the event for all login commands except DEV_ILC_LOGIN_REQUEST</li>
 *		<li>DEV_ILMF_ID: Data (string) to be compared on the device.</li>
 *		<li>DEV_ILMF_KEY_PRESS: Key press is requested. This service have to be answered with ERR_PENDING until the user presses the key on the device.</li>
 *		<li>DEV_ILMF_BLINK: The device should send a signal to its user interface (e. g. beep, blinking LED, ...) to identify it. </li>
 *		<li>DEV_ILMF_VERIFY_ID: Id verification was triggered in the background without any user interaction. 
 *			If this fails, the request is immediately repeated on the same session with a user provided id string.</li>
 *	</ul>
 * </description>
 */
#define DEV_ILMF_NONE				UINT32_C(0x00000000)
#define DEV_ILMF_ID					UINT32_C(0x00000001)
#define DEV_ILMF_KEY_PRESS			UINT32_C(0x00000002)
#define DEV_ILMF_BLINK				UINT32_C(0x00000004)
#define DEV_ILMF_VERIFY_ID			UINT32_C(0x80000000)

/**
* <category>DevSrvSession flags</category>
* <description>
*	<ul>
*		<li>RTS_DEV_SRV_SESSION_FLAG_NONE: No flag set</li>
*		<li>RTS_DEV_SRV_SESSION_FLAG_IMPLICIT: DevSrvSession was created implicit at login, no client information</li>
*		<li>RTS_DEV_SRV_SESSION_FLAG_EXPLICIT: DevSrvSession was created explicitly with client information</li>
*		<li>RTS_DEV_SRV_SESSION_LOGGED_IN: DevSrvSession is in logged in state</li>
*	</ul>
* </description>
*/
#define RTS_DEV_SRV_SESSION_FLAG_NONE		0x00000000
#define RTS_DEV_SRV_SESSION_FLAG_IMPLICIT	0x00000001
#define RTS_DEV_SRV_SESSION_FLAG_EXPLICIT	0x00000002
#define RTS_DEV_SRV_SESSION_LOGGED_IN		0x00000004

/**
 * <description> 
 *	Defines some special values for session ids
 * </description> 
 */
#define DEV_SRV_SESSION_ID_EMPTY				0x00000000
#define DEV_SRV_SESSION_ID_INVALID				0x00000001
#define DEV_SRV_SESSION_ID_INITAL_REQUEST		0x00000011
#define DEV_SRV_SESSION_ID_REMOTE_VISU_CLIENT	0x00000815
#define DEV_SRV_SESSION_ID_WEBSERVER			0x0000ABCD


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

#define TAG_DEV_REPLY_SETTINGS				0x24  /* further tags used by SRV_DEV_LOGIN service are defined in CmpUserMgrItf */

#define TAG_DEV_RESET_ORIGIN_CONFIG				0x31
#define TAG_DEV_RESET_ORIGIN_CONFIG_OPTIONS		0x32
#define TAG_DEV_RESET_ORIGIN_RESULTS			0x33
#define TAG_DEV_RESET_ORIGIN_CONFIG_RIGHTS		0x34
#define TAG_DEV_RESET_ORIGIN_CONFIG_DESCS		0x84
#define TAG_DEV_RESET_ORIGIN_CONFIG_NUM_DESCS	0x35
#define TAG_DEV_RESET_ORIGIN_CONFIG_DESC		0x36

#define TAG_DEV_REPLY_LOGOUT				0x00

#define TAG_DEV_SESSIONID					0x01

#define TAG_DEV_ECHO_REQUEST_DATA_LEN		0x51
#define TAG_DEV_ECHO_REPLY_DATA_LEN			0x52
#define TAG_DEV_ECHO_DATA					0x53
#define TAG_DEV_OPERATION_MODE				0x54
#define TAG_DEV_INTERACTIVE_LOGIN_MODE		0x55
#define TAG_DEV_INTERACTIVE_LOGIN_DATA		0x56
#define TAG_DEV_INTERACTIVE_LOGIN_COMMAND	0x57
#define TAG_DEV_NODE_NAME					0x58

#define TAG_DEV_CLIENT_IDENTIFIER			0x83
#define TAG_DEV_CLIENT_TYPE					0x40
#define TAG_DEV_CLIENT_NAME					0x41
#define TAG_DEV_CLIENT_VENDOR				0x42
#define TAG_DEV_CLIENT_HOSTNAME				0x43
#define TAG_DEV_CLIENT_VERSION				0x44
#define TAG_DEV_CLIENT_CDS_VERSION			0x45
#define TAG_DEV_CLIENT_AUTH_TYPE			0x46
#define TAG_DEV_CLIENT_TIMEOUT_MS			0x47


/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_GET_TARGET_IDENT" name="Get target identity"
 *  description="">
 *	<request>
 *		<tag id="TAG_DEV_REPLY_IDENTIFICATION" name="Identification" cardinality="1..?" type=""
 *		 description="TODO"/>
 *	</request>
 *	<response>
 *		<tag id="TAG_DEV_REPLY_IDENTIFICATION" name="Identification" cardinality="0..?" type=""
 *		 description="TODO"/>
 *		<tag id="TAG_DEV_REPLY_DEVICE_NAME" name="Device name" cardinality="0..?" type=""
 *		 description="TODO"/>
 *		<tag id="TAG_DEV_REPLY_VENDOR_NAME" name="Vendor name" cardinality="0..?" type=""
 *		 description="TODO"/>
 *		<tag id="TAG_DEV_REPLY_NODE_NAME" name="Node name" cardinality="0..?" type=""
 *		 description="TODO"/>
 *		<tag id="TAG_DEV_REPLY" name="Reply" cardinality="1..?" type=""
 *		 description="TODO"/>
 *		<tag id="TAG_DEV_REPLY_RTS_VERSION" name="RTS version" cardinality="0..?" type=""
 *		 description="TODO"/>
 *		<tag id="TAG_DEV_REPLY_FLAGS" name="Flags" cardinality="0..?" type=""
 *		 description="TODO"/>
 *		<tag id="TAG_DEV_REPLY_NUM_CHANNELS" name="Number of channels" cardinality="0..?" type=""
 *		 description="TODO"/>
 *		<tag id="TAG_DEV_REPLY_SERIAL_NUMBER" name="Serial number" cardinality="0..?" type=""
 *		 description="TODO"/>
 *		<tag id="TAG_DEV_REPLY_IP_ADDR_INFO" name="IP address info" cardinality="0..?" type=""
 *		 description="TODO"/>
 *	</response>
 * </service>
 */
#define SRV_DEV_GET_TARGET_IDENT 0x01

/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_LOGIN" name="Login">
 *	<description>
 *		Login service. Login as user on a session explicitly created by SRV_DEV_SESSION_CREATE. For old clients, which do not send the SRV_DEV_SESSION_CREATE service,
 *		the session is implicitly created by this service. Depending on the used authentication type, this service have to be sent several times.
 *	</description>
 *	<request>
 *		<tag id="TAG_USERMGR_CRYPT_TYPE" name="Crypt type" cardinality="1..?" type="RTS_UI32"
 *		 description="Authentication type. Must match to the type negotiated by SRV_DEV_SESSION_CREATE."/>
 *		<tag id="TAG_USERMGR_CRYPT_STEP" name="" cardinality="0..?" type="RTS_UI32"
 *		 description="Current step of login sequence. Not used for authentication type USERMGR_CRYPT_XOR."/>
 *		<tag id="TAG_USERMGR_CRYPT_CHALLENGE" name="Crypt challenge" cardinality="0..?" type="char*"
 *		 description="Challenge used for password encryption. Only used for authentication type USERMGR_CRYPT_XOR."/>
 *		<complextag id="TAG_USERMGR_CREDENTIALS" name="Credentials" cardinality="0..?"
 *		 description="Top level tag. Contains the user credentials.">
 *			<tag id="TAG_USERMGR_USER" name="User" cardinality="0..?" type="char*"
 *			 description="User name."/>
 *			<tag id="TAG_USERMGR_PASSWORD" name="Password" cardinality="0..?" type="char*"
 *			 description="Encrypted password."/>
 *		</complextag>
 *	</request>
 *	<response>
 *		<tag id="TAG_USERMGR_CRYPT_PUB_KEY" name="" cardinality="0..?" type="char*"
 *		 description="Public key used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<tag id="TAG_USERMGR_CRYPT_NONCE" name="" cardinality="0..?" type="char*"
 *		 description="Nonce used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<complextag id="TAG_ONLINE_ACCESS_RESULT" name="" cardinality="1..?"
 *		 description="Top level tag. Contains the service result, if access rights are missing for the requested service.">
 *			<tag id="TAG_USRMGR_REPLY_LOGIN_RESULT" name="" cardinality="1..?" type="RTS_UI16">
 *				<description>
 *					Result code of online operation.
 *					Important result codes, which should be handled by the client: ERR_OK, ERR_PENDING, ERR_TYPE_MISMATCH, ERR_CHANGE_PASSWORD, ERR_NOT_READY_YET, ERR_NO_ACCESS_RIGHTS
 *				</description>
 *			</tag>
 *		</complextag>
 *		<complextag id="TAG_ONLINE_ACCESS_OBJECT" name="" cardinality="1..?"
 *		 description="Top level tag. Describes the denied access.">
 *			<tag id="TAG_NAME" name="" cardinality="1..?" type="char*"
 *			 description="Object which should be accessed"/>
 *			<tag id="TAG_RIGHTS" name="" cardinality="1..?" type="RTS_UI32"
 *			 description="Needed rights to execute the requested access."/>
 *		</complextag>
 *		<tag id="TAG_USERMGR_CRYPT_TYPE" name="" cardinality="1..?" type="RTS_UI32"
 *		 description="Authentication type. Matches to the type negotiated by SRV_DEV_SESSION_CREATE."/>
 *		<tag id="TAG_USERMGR_CRYPT_CHALLENGE" name="" cardinality="0..?" type="char*"
 *		 description="Challenge used for password encryption. Only used for authentication type USERMGR_CRYPT_XOR."/>
 *		<complextag id="TAG_USRMGR_REPLY_LOGIN" name="Login reply" cardinality="1..?"
 *		 description="Top level tag. Contains the service result, if access rights are missing for the requested service.">
 *			<tag id="TAG_USRMGR_REPLY_LOGIN_RESULT" name="Login result" cardinality="1..?" type="RTS_UI16"
 *			 description="Result code of online operation."/>
 *			<tag id="TAG_DEV_REPLY_SETTINGS" name="Settings" cardinality="0..?" type="RTS_UI32"
 *			 description="Feature flags of the runtime system. Only sent, if the login was completed successfully."/>
 *			<tag id="TAG_DEV_REPLY_LOGIN_SESSIONID" name="Login session id" cardinality="0..?" type="RTS_UI32"
 *			 description="SessionID which has to be used for all further online services."/>
 *		</complextag>
 *		<complextag id="TAG_OBJECT" name="" cardinality="1..?"
 *		 description="Top level tag. Describes the denied access.">
 *			<tag id="TAG_NAME" name="" cardinality="1..?" type="char*"
 *			 description="Object which should be accessed"/>
 *			<tag id="TAG_RIGHTS" name="" cardinality="1..?" type="RTS_UI32"
 *			 description="Needed rights to execute the requested access."/>
 *		</complextag>
 *		<tag id="TAG_ERROR" name="" cardinality="1" type="RTS_UI16">
 *			<value define="ERR_INVALID_SESSION_ID" description="SessionID or session invalid." />
 *		</tag>
 *	</response>
 * </service>
 */
#define SRV_DEV_LOGIN 0x02

/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_LOGOUT" name="Logout"
 *  description="">
 *	<request>
 *		<tag id="TODO_0x01" name="" cardinality="" type=""
 *		 description="TODO"/>
 *	</request>
 *	<response>
 *		<tag id="TAG_DEV_REPLY_LOGOUT" name="Logout reply" cardinality="" type=""
 *		 description="TODO"/>
 *	</response>
 * </service>
 */
#define SRV_DEV_LOGOUT 0x03

/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_RESET_ORIGIN" name="Reset origin">
 *	<description>
 *		Reset Origin [Device]. Reset the device to factory settings.
 *		Compatibility note: if no tag TAG_DEV_RESET_ORIGIN_CONFIG_OPTIONS is received, the reset will affect all objects for which the current user has the needed rights (except objects specified by TAG_DEV_RESET_ORIGIN_CONFIG).
 *	</description>
 *	<request>
 *		<tag id="TAG_DEV_RESET_ORIGIN_CONFIG_OPTIONS" name="Reset origin config" cardinality="0..?" type="RTS_UI32">
 *			<description>
 *				See category "Reset Origin Configuration". Flag field for objects that shall not be handled during reset.
 *			</description>
 *		</tag>
 *		<tag id="TAG_DEV_RESET_ORIGIN_CONFIG" name="Reset origin config" cardinality="0..?" type="RTS_UI32">
 *			<description>
 *				Obsolete: use TAG_DEV_RESET_ORIGIN_CONFIG_OPTIONS instead.
 *			</description>
 *		</tag>
 *	</request>
 *	<response>
 *		<tag id="TAG_DEV_RESET_ORIGIN_RESULTS" name="Reset origin results" cardinality="0..1" type="RTS_UI32">
 *			<description>
 *				Only if an error occured during deletion of the configured objects: bit vector indicating the results for the requested configuration (bit=1: error deleting the corresponding object, bit=0: success).
 *			</description>
 *		</tag>
 *		<tag id="TAG_SERVICE_RESULT" name="Result" cardinality="" type=""
 *		 description="TODO"/>
 *	</response>
 * </service>
 */
#define SRV_DEV_RESET_ORIGIN 0x04

/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_ECHO" name="Echo">
 *	<description>
 *		Echo service. Receives services and send replies with dummy bytes as payload. If the tag TAG_DEV_ECHO_REPLY_DATA_LEN is omitted in
 *		the request, the server sends an empty service reply without any tag. If the requested reply length in TAG_DEV_ECHO_REPLY_DATA_LEN
 *		exceeds the communication buffer, then the length is limited to a value, which completely fills the communication buffer. If the requested
 *		reply length is greater than the request length, then dummy bytes are generated by the server. If not, the payload is copied starting from
 *		the beginning of the request payload in TAG_DEV_ECHO_DATA.
 *	</description>
 *	<request>
 *		<tag id="TAG_DEV_ECHO_REQUEST_DATA_LEN" name="Request data length" cardinality="0..?" type="RTS_UI32"
 *		 description="Length of tag content in TAG_DEV_ECHO_DATA."/>
 *		<tag id="TAG_DEV_ECHO_REPLY_DATA_LEN" name="Reply data length" cardinality="0..?" type="RTS_UI32"
 *		 description="Requested length of reply content in TAG_DEV_ECHO_DATA."/>
 *		<tag id="TAG_DEV_ECHO_DATA" name="Data" cardinality="0..?" type="char*"
 *		 description="Dummy bytes as payload."/>
 *	</request>
 *	<response>
 *		<tag id="TAG_SERVICE_RESULT" name="Result" cardinality="0..?" type="RTS_UI16"
 *		 description="Result code of online operation"/>
 *		<tag id="TAG_DEV_ECHO_REPLY_DATA_LEN" name="Reply data length" cardinality="0..?" type="RTS_UI32"
 *		 description="Length of tag content in TAG_DEV_ECHO_DATA"/>
 *		<tag id="TAG_DEV_ECHO_DATA" name="Data" cardinality="0..?" type="char*"
 *		 description="Dummy bytes as payload."/>
 *	</response>
 * </service>
 */
#define SRV_DEV_ECHO 0x05

/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_SET_OPERATION_MODE" name="Set operation mode"
 *  description="Sets one of the device operation modes of the PLC (see category: Device operation mode). Changing the operation mode may be denied by the PLC.">
 *	<request>
 *		<tag id="TAG_DEV_OPERATION_MODE" name="Operation mode" cardinality="0..?" type="RTS_UI32"
 *		 description="Operation mode to set."/>
 *	</request>
 *	<response>
 *		<tag id="TAG_SERVICE_RESULT" name="Result" cardinality="1..?" type="RTS_UI6"
 *		 description="Result code of online operation"/>
 *		<tag id="TAG_USER_NOTIFY" name="" cardinality="0..?" type="LogEntry"
 *		 description="User notification message, contains the component, which has denied the service"/>
 *	</response>
 * </service>
 */
#define SRV_DEV_SET_OPERATION_MODE 0x06

/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_GET_OPERATION_MODE" name="Get operation mode"
 *  description="Retrieves the device operation modes of the PLC (see category: Device operation mode).">
 *	<request/>
 *	<response>
 *		<tag id="TAG_DEV_OPERATION_MODE" name="Operation mode" cardinality="1..?" type="RTS_UI32"
 *		 description="Current operation mode of the device."/>
 *	</response>
 * </service>
 */
#define SRV_DEV_GET_OPERATION_MODE 0x07

/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_INTERACTIVE_LOGIN" name="Interactive login"
 *  description="Interactive login service. Helps to identify a PLC before login. ">
 *	<request>
 *		<tag id="TAG_DEV_INTERACTIVE_LOGIN_COMMAND" name="Command" cardinality="1..?" type="RTS_UI32">
 *			<description>
 *				See category "Interactive login commands".
 *			</description>
 *		</tag>
 *		<tag id="TAG_DEV_INTERACTIVE_LOGIN_MODE" name="Mode" cardinality="0..?" type="RTS_UI32"
 *		 description="See category Interactive login mode flags. Only used for command DEV_ILC_LOGIN_REQUEST."/>
 *		<tag id="TAG_DEV_INTERACTIVE_LOGIN_DATA" name="Data" cardinality="0..?" type="RTS_UI8*"
 *		 description="Login information to be checked by the runtime system. Only used for mode DEV_ILM_STRING"/>
 *	</request>
 *	<response>
 *		<tag id="TAG_SERVICE_RESULT" name="Result" cardinality="1..?" type="RTS_UI16">
 *			<description>
 *				Result code of online operation. Supported results: 
 *				ERR_OK (interactive login successful), 
 *				ERR_FAILED (generic error), 
 *				ERR_TIMEOUT (timeout elapsed on device side), 
 *				ERR_PENDING (device still waits for user action, e. g. key press), 
 *				ERR_NOT_SUPPORTED (requested login mode is not supported by the device) 
 *				ERR_ID_MISMATCH (login information does not match).
 *			</description>
 *		</tag>
 *		<tag id="TAG_DEV_REPLY_LOGIN_SESSIONID" name="Session id" cardinality="0..?" type="RTS_UI32">
 *			<description>
 *				SessionID which have to be used for all further online services on this channel. 
 *				Only returned for command DEV_ILC_LOGIN_REQUEST
 *			</description>
 *		</tag>
 *	</response>
 * </service>
 */
#define SRV_DEV_INTERACTIVE_LOGIN 0x08

/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_SET_NODE_NAME" name="Set node name"
 *  description="Sets a new UTF-16 node name for the device.">
 *	<request>
 *		<tag id="TAG_DEV_NODE_NAME" name="Node name" cardinality="1..?" type="RTS_UI32">
 *			<description>
 *				New UTF-16 node name. By passing an empty string, the node name settings are
 *				deleted and so the default node name is set again.
 *			</description>
 *		</tag>
 *	</request>
 *	<response>
 *		<tag id="TAG_SERVICE_RESULT" name="Result" cardinality="1..?" type="RTS_UI6"
 *		 description="Result code of online operation.
 *       Possible reasons for failure:
 *       - There is an invalid UTF-16 character in the node name.
 *       - Error when setting the node name. See description of SysTargetSetNodeName for details."/>
 *	</response>
 * </service>
 */
#define SRV_DEV_SET_NODE_NAME 0x09

/**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_SESSION_CREATE" name=""
 *  description="Service to create a session and identify the client. ">
 *	<request>
 *		<complextag id="TAG_DEV_CLIENT_IDENTIFIER" name="" cardinality="1"
 *		 description="Top level tag. Contains the client identification.">
 *			<tag id="TAG_DEV_CLIENT_TYPE" name="" cardinality="1" type="RTS_UI32"
 *			 description="The type of the client. See Client type in CmpSessionInformationItf."/>
 *			<tag id="TAG_DEV_CLIENT_NAME" name="" cardinality="0..1" type="char[]"
 *			 description="UTF-8 encoded name of the client."/>
 *			<tag id="TAG_DEV_CLIENT_VENDOR" name="" cardinality="0..1" type="char[]"
 *			 description="UTF-8 encoded vendor name of the client."/>
 *			<tag id="TAG_DEV_CLIENT_HOSTNAME" name="" cardinality="0..1" type="char[]"
 *			 description="UTF-8 encoded fully qualified host name (FQHN) of the system where the client is running."/>
 *			<tag id="TAG_DEV_CLIENT_VERSION" name="" cardinality="0..1" type="char[]"
 *			 description="UTF-8 encoded client version number."/>
 *			<tag id="TAG_DEV_CLIENT_CDS_VERSION" name="" cardinality="0..1" type="char[]"
 *			 description="UTF-8 encoded fully CODESYS version number."/>
 *		</complextag>
 *		<tag id="TAG_DEV_CLIENT_AUTH_TYPE" name="" cardinality="1" type=""
 *		 description="Authentication types supported by the client. See Crypt type in CmpUserMgrItf."/>
 *		<tag id="TAG_DEV_CLIENT_TIMEOUT_MS" name="" cardinality="0..1" type="RTS_UI32"
 *		 description="Session timeout in milliseconds."/>
 *	</request>
 *	<response>
 *		<tag id="TAG_DEV_REPLY_LOGIN_SESSIONID" name="" cardinality="0..1" type="RTS_UI32"
 *		 description="SessionID which has to be used for all further online services of this client."/>
 *		<tag id="TAG_DEV_CLIENT_AUTH_TYPE" name="" cardinality="1" type="RTS_UI32"
 *		 description="Effective authentication type selected by the runtime"/>
 *		<tag id="TAG_ERROR" name="" cardinality="0..1" type="RTS_UI16">
 *			<value define="ERR_DUPLICATE" description="An existing session is usually treated as error." />
 *			<value define="ERR_OUT_OF_LIMITS" description="Channel sessions limit reached." />
 *		</tag>
 *	</response>
 * </service>
 */
#define SRV_DEV_SESSION_CREATE 0x0A

 /**
 * <category>Online services</category>
 * <service group="SG_DEVICE" id="SRV_DEV_RESET_ORIGIN_GET_CONFIG" name="Reset origin get configuration"
 *  description="Get Reset Origin [Device] configuration. Is used to retrieve information about objects that are handled by the reset.">
 *	<request>
 *	</request>
 *	<response>
 *		<tag id="TAG_DEV_RESET_ORIGIN_CONFIG_OPTIONS" name="Reset origin config" cardinality="1" type="RTS_UI32">
 *			<description>
 *				See category "Reset Origin Configuration". Each bit represents an object that is handled.
 *			</description>
 *		</tag>
 *		<tag id="TAG_DEV_RESET_ORIGIN_CONFIG_RIGHTS" name="Reset origin config rights" cardinality="1" type="RTS_UI32">
 *			<description>
 *				Indicates the user access rights for the configuration. Each bit represents the right for the corresponding bit in TAG_DEV_RESET_ORIGIN_CONFIG_OPTIONS
 *				(bit=0: the current user is allowed to delete the corresponding object, bit=1: the user is not allowed to delete the corresponding object).
 *			</description>
 *		</tag>
 *		<complextag id="TAG_DEV_RESET_ORIGIN_CONFIG_DESCS" name="" cardinality="1"
 *		 description="Top level tag. Contains the descriptions of the objects that are handled in ascending order corresponding to the bits in TAG_DEV_RESET_ORIGIN_CONFIG_OPTIONS">
 *			<tag id="TAG_DEV_RESET_ORIGIN_CONFIG_NUM_DESCS" name="" cardinality="1" type="RTS_UI16"
 *			 description="Number of following descriptions"/>
 *			<tag id="TAG_DEV_RESET_ORIGIN_CONFIG_DESC" name="" cardinality="1..?" type="RTS_UTF8STRING"
 *			 description="Description of the object"/>
 *		</complextag>
 *		<tag id="TAG_SERVICE_RESULT" name="Result" cardinality="1" type="RTS_UI16">
 *			<description>
 *				Result code of online operation. Supported results: 
 *				ERR_OK (configuration successfully retrieved), 
 *				ERR_FAILED (generic error).
 *			</description>
 *		</tag>
 *	</response>
 * </service>
 */
#define SRV_DEV_RESET_ORIGIN_GET_CONFIG 0x0B

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
 * <element name="ui32ResetOriginConfig" type="IN">Configuration bit vector. See category "Reset Origin Configuration".</element>
 * <element name="hUser" type="IN">User handle of the current user.</element>
 * <element name="ui32ResetOriginResults" type="OUT">Result bit vector corresponding to ui32ResetOriginConfig (bit=0: success, bit=1: failed).</element>
 */
typedef struct
{
	RTS_UI32 ui32ResetOriginConfig;
	RTS_HANDLE hUser;
	RTS_UI32 ui32ResetOriginResults;
} EVTPARAM_CmpDevice_ResetOrigin;
#define EVTPARAMID_CmpDevice_ResetOrigin	0x0001
#define EVTVERSION_CmpDevice_ResetOrigin	0x0002

/**
 * <category>Event parameter</category>
 * <element name="ui32OperationMode" type="IN">New operation mode of the device. See category Device operation mode for possible values.</element>
 */
typedef struct
{
	RTS_UI32 ui32OperationMode;
} EVTPARAM_CmpDevice_SetOperationMode;
#define EVTPARAMID_CmpDevice_SetOperationMode	0x0001
#define EVTVERSION_CmpDevice_SetOperationMode	0x0001

/**
 * <category>Event parameter</category>
 * <element name="hUser" type="IN">User handle of the current user.</element>
 * <element name="ui32ResetOriginConfigOptions" type="OUT">
 *		Configuration bit vector representing the possible objects that are handled. See category "Reset Origin Configuration".
 *		The callback has to set the bit for the object it is responsible for.
 * </element>
 * <element name="ui32ResetOriginConfigRights" type="OUT">
 *		Bit vector indicating the access rights for the specified user to delete the objects represented in ui32ResetOriginConfigOptions
 *		(bit=0: the current user is allowed to delete the corresponding object, bit=1: the user is not allowed to delete).
 * </element>
 */
typedef struct
{
	RTS_HANDLE hUser;
	RTS_UI32 ui32ResetOriginConfigOptions;
	RTS_UI32 ui32ResetOriginConfigRights;
} EVTPARAM_CmpDevice_ResetOriginGetConfig;
#define EVTPARAMID_CmpDevice_ResetOriginGetConfig	0x0002
#define EVTVERSION_CmpDevice_ResetOriginGetConfig	0x0001

/**
 * <category>Event parameter</category>
 * <element name="clientType" type="IN">The type client type. See "Client types" of CmpSessionInformationItf.</element>
 * <element name="pszClientName" type="IN">The name of the client.</element>
 * <element name="pszClientVendorName" type="IN">The vendor name of the client application.</element>
 * <element name="pszClientHostName" type="IN">The hostname where the client is running.</element>
 * <element name="pszClientVersion" type="IN">The version of the client.</element>
 * <element name="pszClientCdsVersion" type="IN">The CODESYS version the client is based on.</element>
 * <element name="bDeny" type="OUT">TRUE=Login is denied, FALSE=Login will be executed.</element>
 */
typedef struct
{
	const RTS_UI32			clientType;
	const RTS_UTF8STRING	* const pszClientName;
	const RTS_UTF8STRING	* const pszClientVendorName;
	const RTS_UTF8STRING	* const pszClientHostName;
	const RTS_UTF8STRING	* const pszClientVersion;
	const RTS_UTF8STRING	* const pszClientCdsVersion;
    RTS_BOOL bDeny;
} EVTPARAM_CmpDevS_BeforeCreateSession;
#define EVTPARAMID_CmpDevS_BeforeCreateSession 0x0005
#define EVTVERSION_CmpDevS_BeforeCreateSession 0x0001

/**
 * <category>Event parameter</category>
 * <element name="ui32ChannelId" type="IN">
 *   The internal ID of the channel the session belonged to.
 * </element>
 * <element name="ui32SessionId" type="IN">
 *   The internal session ID.
 * </element>
 * <element name="errReason" type="IN">
 *	  An error code describing the reason for closing the session:
 *      - ERR_OK: User gracefully logged out.
 *      - ERR_DEVS_LOGINFAILED: User login failed.
 *      - ERR_DEVS_SESSIONTIMEOUT: Inactive session deleted due to timeout.
 *      - ERR_DEVS_CHANNELCLOSED: The channel of the session has been closed.
 * </element>
 */
typedef struct
{
	RTS_UI32   ui32ChannelId;
	RTS_UI32   ui32SessionId;
	RTS_RESULT  errReason;
} EVTPARAM_CmpDevS_SessionDeleted;
#define EVTPARAMID_CmpDevS_SessionDeleted 0x0006
#define EVTVERSION_CmpDevS_SessionDeleted 0x0001

/**
 * <category>Reset Origin Configuration</category>
 * <description>Configuration of the Reset Origin service &amp; event.
 *	NOTE: 
 *	If this list is extended, a corresponding description has to be added to the "Reset Origin Configuration Descriptions".
 *	Also, the list in the IDE should be extended including the localization of the description.
 * </description>
 */
#define DEV_ROC_RESET_ALL	0x00000000
#define DEV_ROC_USERMGMT	0x00000001
#define DEV_ROC_KEEP_USERDB	DEV_ROC_USERMGMT
#define DEV_ROC_PLCLOGIC	0x00000002
#define DEV_ROC_CERTS		0x00000004

/**
 * <category>Reset Origin Configuration Descriptions</category>
 * <description>Descriptions of the objects defined in category "Reset Origin Configuration".</description>
 */
#define DEV_ROC_DESCS	{\
							{ DEV_ROC_USERMGMT,	"User Management" }, \
							{ DEV_ROC_PLCLOGIC,	"PlcLogic" }, \
							{ DEV_ROC_CERTS,	"Certificates" }, \
						}

/* Structure to handle a mapping of a reset origin configuration flag and a description */
typedef struct RTS_ROC_TO_DESC_
{
	RTS_UI32 ui32Flag;
	RTS_UTF8STRING *pDesc;
} RTS_ROC_TO_DESC;

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
 * <description>Event is sent at reset origin device.</description>
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
 * <category>Events</category>
 * <description>Event is sent when the reset origin configuration is requested. Each responsible component has to add its information to the event parameter in the callback.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevice_ResetOriginGetConfig</param>
 */
#define EVT_CmpDevice_ResetOriginGetConfig		MAKE_EVENTID(EVTCLASS_INFO, 4)

/**
 * <category>Events</category>
 * <description>
 * Allows suppressing the creation (deny) of a session to allow only certain clients.
 * No callbacks in IEC possible.
 * </description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevS_BeforeCreateSession</param>
 */
#define EVT_DevSBeforeCreateSession	MAKE_EVENTID(EVTCLASS_INFO, 5)

/**
 * <category>Events</category>
 * <description>Event is sent when a session is deleted.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevS_SessionDeleted</param>
 */
#define EVT_DevSSessionDeleted	MAKE_EVENTID(EVTCLASS_INFO, 6)

/**
 * <category>Settings flags</category>
 * <description>Settings flags that re returned by the runtime system</description>
 */
#define SETTINGS_FLAG_KILLTASKONRESET			 0x01
#define SETTINGS_FLAG_COMPLEXMONITORING			 0x02
#define SETTINGS_FLAG_MONITORING2				 0x04
#define SETTINGS_FLAG_TRACE_SUPPORTS_MONITORING2 0x08
#define SETTINGS_FLAG_EDGE_GATEWAY				 0x10
#define SETTINGS_FLAG_EDGE_GATEWAY_CONFIGURABLE	 0x20

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
 *	<element name="nMaxDeviceNameLen" type="IN">Maximum length of the device name buffer in UNICODE characters (not bytes!)</element>
 *	<element name="pwszVendorName" type="IN">Pointer to vendor name</element>
 *	<element name="nMaxVendorNameLen" type="IN">Maximum length of the vendor name buffer in UNICODE characters (not bytes!)</element>
 *	<element name="pwszNodeName" type="IN">Pointer to node name</element>
 *	<element name="nMaxNodeNameLen" type="IN">Maximum length of the node name buffer in UNICODE characters (not bytes!)</element>
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
RTS_RESULT CDECL DevCheckTargetIdent(const TargetIdent *pTargetIdentReq, const TargetIdent *pTargetIdent);
typedef RTS_RESULT (CDECL * PFDEVCHECKTARGETIDENT) (const TargetIdent *pTargetIdentReq, const TargetIdent *pTargetIdent);
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
 *  Function to set the operation mode of the device. Usually this is called by an online service, received from the development system.
 *  Switching to the new operation mode may be denied by another component.
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




/** 
 * <description>
 *   Checks whether the channel has a session with the supplied ID.  
 * </description>
 * <param name="ui32ChannelId" type="IN">ID of the channel for which the session ID should be checked.</param>
 * <param name="ui32SessionId" type="OUT">Session ID.</param>
 * <result>
 *	<ul>
 *		<li>TRUE: The channel has a session with that ID.</li>
 *		<li>FALSE: The channel doesn't have a session with that ID.</li>
 *	</ul>
 * </result>
*/
RTS_BOOL CDECL DevSrvIsSessionValid(RTS_UI32 ui32ChannelId, RTS_UI32 ui32SessionId);
typedef RTS_BOOL (CDECL * PFDEVSRVISSESSIONVALID) (RTS_UI32 ui32ChannelId, RTS_UI32 ui32SessionId);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVSRVISSESSIONVALID_NOTIMPLEMENTED)
	#define USE_DevSrvIsSessionValid
	#define EXT_DevSrvIsSessionValid
	#define GET_DevSrvIsSessionValid(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevSrvIsSessionValid(p0,p1)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_DevSrvIsSessionValid  FALSE
	#define EXP_DevSrvIsSessionValid  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevSrvIsSessionValid
	#define EXT_DevSrvIsSessionValid
	#define GET_DevSrvIsSessionValid(fl)  CAL_CMGETAPI( "DevSrvIsSessionValid" ) 
	#define CAL_DevSrvIsSessionValid  DevSrvIsSessionValid
	#define CHK_DevSrvIsSessionValid  TRUE
	#define EXP_DevSrvIsSessionValid  CAL_CMEXPAPI( "DevSrvIsSessionValid" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevSrvIsSessionValid
	#define EXT_DevSrvIsSessionValid
	#define GET_DevSrvIsSessionValid(fl)  CAL_CMGETAPI( "DevSrvIsSessionValid" ) 
	#define CAL_DevSrvIsSessionValid  DevSrvIsSessionValid
	#define CHK_DevSrvIsSessionValid  TRUE
	#define EXP_DevSrvIsSessionValid  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevSrvIsSessionValid", (RTS_UINTPTR)DevSrvIsSessionValid, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevSrvIsSessionValid
	#define EXT_CmpDeviceDevSrvIsSessionValid
	#define GET_CmpDeviceDevSrvIsSessionValid  ERR_OK
	#define CAL_CmpDeviceDevSrvIsSessionValid pICmpDevice->IDevSrvIsSessionValid
	#define CHK_CmpDeviceDevSrvIsSessionValid (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevSrvIsSessionValid  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevSrvIsSessionValid
	#define EXT_DevSrvIsSessionValid
	#define GET_DevSrvIsSessionValid(fl)  CAL_CMGETAPI( "DevSrvIsSessionValid" ) 
	#define CAL_DevSrvIsSessionValid pICmpDevice->IDevSrvIsSessionValid
	#define CHK_DevSrvIsSessionValid (pICmpDevice != NULL)
	#define EXP_DevSrvIsSessionValid  CAL_CMEXPAPI( "DevSrvIsSessionValid" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevSrvIsSessionValid  PFDEVSRVISSESSIONVALID pfDevSrvIsSessionValid;
	#define EXT_DevSrvIsSessionValid  extern PFDEVSRVISSESSIONVALID pfDevSrvIsSessionValid;
	#define GET_DevSrvIsSessionValid(fl)  s_pfCMGetAPI2( "DevSrvIsSessionValid", (RTS_VOID_FCTPTR *)&pfDevSrvIsSessionValid, (fl), 0, 0)
	#define CAL_DevSrvIsSessionValid  pfDevSrvIsSessionValid
	#define CHK_DevSrvIsSessionValid  (pfDevSrvIsSessionValid != NULL)
	#define EXP_DevSrvIsSessionValid  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevSrvIsSessionValid", (RTS_UINTPTR)DevSrvIsSessionValid, 0, 0) 
#endif




/** 
 * <description>
 *   Retrieves the user handle assigned to the session.  
 * </description>
 * <param name="ui32SessionId" type="IN">Session id of which the user handle should be read.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User is available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The session management is not initialized</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">The session is not available</errorcode>
 * <result>Handle of the user or RTS_INVALID_HANDLE if not available</result>
*/
RTS_HANDLE CDECL DevGetSessionUser(RTS_UI32 ui32SessionId, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFDEVGETSESSIONUSER) (RTS_UI32 ui32SessionId, RTS_RESULT *pResult);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVGETSESSIONUSER_NOTIMPLEMENTED)
	#define USE_DevGetSessionUser
	#define EXT_DevGetSessionUser
	#define GET_DevGetSessionUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevGetSessionUser(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_DevGetSessionUser  FALSE
	#define EXP_DevGetSessionUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevGetSessionUser
	#define EXT_DevGetSessionUser
	#define GET_DevGetSessionUser(fl)  CAL_CMGETAPI( "DevGetSessionUser" ) 
	#define CAL_DevGetSessionUser  DevGetSessionUser
	#define CHK_DevGetSessionUser  TRUE
	#define EXP_DevGetSessionUser  CAL_CMEXPAPI( "DevGetSessionUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevGetSessionUser
	#define EXT_DevGetSessionUser
	#define GET_DevGetSessionUser(fl)  CAL_CMGETAPI( "DevGetSessionUser" ) 
	#define CAL_DevGetSessionUser  DevGetSessionUser
	#define CHK_DevGetSessionUser  TRUE
	#define EXP_DevGetSessionUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetSessionUser", (RTS_UINTPTR)DevGetSessionUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevGetSessionUser
	#define EXT_CmpDeviceDevGetSessionUser
	#define GET_CmpDeviceDevGetSessionUser  ERR_OK
	#define CAL_CmpDeviceDevGetSessionUser pICmpDevice->IDevGetSessionUser
	#define CHK_CmpDeviceDevGetSessionUser (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevGetSessionUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevGetSessionUser
	#define EXT_DevGetSessionUser
	#define GET_DevGetSessionUser(fl)  CAL_CMGETAPI( "DevGetSessionUser" ) 
	#define CAL_DevGetSessionUser pICmpDevice->IDevGetSessionUser
	#define CHK_DevGetSessionUser (pICmpDevice != NULL)
	#define EXP_DevGetSessionUser  CAL_CMEXPAPI( "DevGetSessionUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevGetSessionUser  PFDEVGETSESSIONUSER pfDevGetSessionUser;
	#define EXT_DevGetSessionUser  extern PFDEVGETSESSIONUSER pfDevGetSessionUser;
	#define GET_DevGetSessionUser(fl)  s_pfCMGetAPI2( "DevGetSessionUser", (RTS_VOID_FCTPTR *)&pfDevGetSessionUser, (fl), 0, 0)
	#define CAL_DevGetSessionUser  pfDevGetSessionUser
	#define CHK_DevGetSessionUser  (pfDevGetSessionUser != NULL)
	#define EXP_DevGetSessionUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetSessionUser", (RTS_UINTPTR)DevGetSessionUser, 0, 0) 
#endif




/** 
 * <description>
 *   Retrieves the authentication type used for the session.  
 * </description>
 * <param name="ui32SessionId" type="IN">Session id of which the authentication type should be read.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Authentication type was read successfully</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The session management is not initialized</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">The session is not available</errorcode>
 * <result>Crypt type (see CmpUserMgrItf) or USERMGR_CRYPT_INVALID if not available</result>
*/
RTS_UI32 CDECL DevGetSessionAuthType(RTS_UI32 ui32SessionId, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFDEVGETSESSIONAUTHTYPE) (RTS_UI32 ui32SessionId, RTS_RESULT *pResult);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVGETSESSIONAUTHTYPE_NOTIMPLEMENTED)
	#define USE_DevGetSessionAuthType
	#define EXT_DevGetSessionAuthType
	#define GET_DevGetSessionAuthType(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevGetSessionAuthType(p0,p1)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_DevGetSessionAuthType  FALSE
	#define EXP_DevGetSessionAuthType  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevGetSessionAuthType
	#define EXT_DevGetSessionAuthType
	#define GET_DevGetSessionAuthType(fl)  CAL_CMGETAPI( "DevGetSessionAuthType" ) 
	#define CAL_DevGetSessionAuthType  DevGetSessionAuthType
	#define CHK_DevGetSessionAuthType  TRUE
	#define EXP_DevGetSessionAuthType  CAL_CMEXPAPI( "DevGetSessionAuthType" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevGetSessionAuthType
	#define EXT_DevGetSessionAuthType
	#define GET_DevGetSessionAuthType(fl)  CAL_CMGETAPI( "DevGetSessionAuthType" ) 
	#define CAL_DevGetSessionAuthType  DevGetSessionAuthType
	#define CHK_DevGetSessionAuthType  TRUE
	#define EXP_DevGetSessionAuthType  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetSessionAuthType", (RTS_UINTPTR)DevGetSessionAuthType, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevGetSessionAuthType
	#define EXT_CmpDeviceDevGetSessionAuthType
	#define GET_CmpDeviceDevGetSessionAuthType  ERR_OK
	#define CAL_CmpDeviceDevGetSessionAuthType pICmpDevice->IDevGetSessionAuthType
	#define CHK_CmpDeviceDevGetSessionAuthType (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevGetSessionAuthType  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevGetSessionAuthType
	#define EXT_DevGetSessionAuthType
	#define GET_DevGetSessionAuthType(fl)  CAL_CMGETAPI( "DevGetSessionAuthType" ) 
	#define CAL_DevGetSessionAuthType pICmpDevice->IDevGetSessionAuthType
	#define CHK_DevGetSessionAuthType (pICmpDevice != NULL)
	#define EXP_DevGetSessionAuthType  CAL_CMEXPAPI( "DevGetSessionAuthType" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevGetSessionAuthType  PFDEVGETSESSIONAUTHTYPE pfDevGetSessionAuthType;
	#define EXT_DevGetSessionAuthType  extern PFDEVGETSESSIONAUTHTYPE pfDevGetSessionAuthType;
	#define GET_DevGetSessionAuthType(fl)  s_pfCMGetAPI2( "DevGetSessionAuthType", (RTS_VOID_FCTPTR *)&pfDevGetSessionAuthType, (fl), 0, 0)
	#define CAL_DevGetSessionAuthType  pfDevGetSessionAuthType
	#define CHK_DevGetSessionAuthType  (pfDevGetSessionAuthType != NULL)
	#define EXP_DevGetSessionAuthType  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetSessionAuthType", (RTS_UINTPTR)DevGetSessionAuthType, 0, 0) 
#endif




/** 
 * <description>
 *   Retrieves the specific flags describing the session.  
 * </description>
 * <param name="ui32SessionId" type="IN">Session id of which the session flags should be read.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Flags were read successfully</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The session management is not initialized</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">The session is not available</errorcode>
 * <result>Session flags</result>
*/
RTS_UI32 CDECL DevGetSessionFlags(RTS_UI32 ui32SessionId, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFDEVGETSESSIONFLAGS) (RTS_UI32 ui32SessionId, RTS_RESULT *pResult);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVGETSESSIONFLAGS_NOTIMPLEMENTED)
	#define USE_DevGetSessionFlags
	#define EXT_DevGetSessionFlags
	#define GET_DevGetSessionFlags(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevGetSessionFlags(p0,p1)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_DevGetSessionFlags  FALSE
	#define EXP_DevGetSessionFlags  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevGetSessionFlags
	#define EXT_DevGetSessionFlags
	#define GET_DevGetSessionFlags(fl)  CAL_CMGETAPI( "DevGetSessionFlags" ) 
	#define CAL_DevGetSessionFlags  DevGetSessionFlags
	#define CHK_DevGetSessionFlags  TRUE
	#define EXP_DevGetSessionFlags  CAL_CMEXPAPI( "DevGetSessionFlags" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevGetSessionFlags
	#define EXT_DevGetSessionFlags
	#define GET_DevGetSessionFlags(fl)  CAL_CMGETAPI( "DevGetSessionFlags" ) 
	#define CAL_DevGetSessionFlags  DevGetSessionFlags
	#define CHK_DevGetSessionFlags  TRUE
	#define EXP_DevGetSessionFlags  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetSessionFlags", (RTS_UINTPTR)DevGetSessionFlags, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevGetSessionFlags
	#define EXT_CmpDeviceDevGetSessionFlags
	#define GET_CmpDeviceDevGetSessionFlags  ERR_OK
	#define CAL_CmpDeviceDevGetSessionFlags pICmpDevice->IDevGetSessionFlags
	#define CHK_CmpDeviceDevGetSessionFlags (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevGetSessionFlags  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevGetSessionFlags
	#define EXT_DevGetSessionFlags
	#define GET_DevGetSessionFlags(fl)  CAL_CMGETAPI( "DevGetSessionFlags" ) 
	#define CAL_DevGetSessionFlags pICmpDevice->IDevGetSessionFlags
	#define CHK_DevGetSessionFlags (pICmpDevice != NULL)
	#define EXP_DevGetSessionFlags  CAL_CMEXPAPI( "DevGetSessionFlags" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevGetSessionFlags  PFDEVGETSESSIONFLAGS pfDevGetSessionFlags;
	#define EXT_DevGetSessionFlags  extern PFDEVGETSESSIONFLAGS pfDevGetSessionFlags;
	#define GET_DevGetSessionFlags(fl)  s_pfCMGetAPI2( "DevGetSessionFlags", (RTS_VOID_FCTPTR *)&pfDevGetSessionFlags, (fl), 0, 0)
	#define CAL_DevGetSessionFlags  pfDevGetSessionFlags
	#define CHK_DevGetSessionFlags  (pfDevGetSessionFlags != NULL)
	#define EXP_DevGetSessionFlags  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevGetSessionFlags", (RTS_UINTPTR)DevGetSessionFlags, 0, 0) 
#endif




/** 
 * <description>
 *   Checks if a request is valid.
 * </description>
 * <param name="ulChannelHandle" type="IN">Id of the channel on which the request was received.</param>
 * <param name="pHeaderTag" type="IN">Pointer to the header of the request to check.</param>
 * <result>error code</result>
*/
RTS_RESULT CDECL DevIsValidRequest(RTS_UI32 ulChannelHandle, HEADER_TAG_EXT *pHeaderTag, PROTOCOL_DATA_UNIT *pPduReply);
typedef RTS_RESULT (CDECL * PFDEVISVALIDREQUEST) (RTS_UI32 ulChannelHandle, HEADER_TAG_EXT *pHeaderTag, PROTOCOL_DATA_UNIT *pPduReply);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVISVALIDREQUEST_NOTIMPLEMENTED)
	#define USE_DevIsValidRequest
	#define EXT_DevIsValidRequest
	#define GET_DevIsValidRequest(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevIsValidRequest(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_DevIsValidRequest  FALSE
	#define EXP_DevIsValidRequest  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevIsValidRequest
	#define EXT_DevIsValidRequest
	#define GET_DevIsValidRequest(fl)  CAL_CMGETAPI( "DevIsValidRequest" ) 
	#define CAL_DevIsValidRequest  DevIsValidRequest
	#define CHK_DevIsValidRequest  TRUE
	#define EXP_DevIsValidRequest  CAL_CMEXPAPI( "DevIsValidRequest" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevIsValidRequest
	#define EXT_DevIsValidRequest
	#define GET_DevIsValidRequest(fl)  CAL_CMGETAPI( "DevIsValidRequest" ) 
	#define CAL_DevIsValidRequest  DevIsValidRequest
	#define CHK_DevIsValidRequest  TRUE
	#define EXP_DevIsValidRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevIsValidRequest", (RTS_UINTPTR)DevIsValidRequest, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevIsValidRequest
	#define EXT_CmpDeviceDevIsValidRequest
	#define GET_CmpDeviceDevIsValidRequest  ERR_OK
	#define CAL_CmpDeviceDevIsValidRequest pICmpDevice->IDevIsValidRequest
	#define CHK_CmpDeviceDevIsValidRequest (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevIsValidRequest  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevIsValidRequest
	#define EXT_DevIsValidRequest
	#define GET_DevIsValidRequest(fl)  CAL_CMGETAPI( "DevIsValidRequest" ) 
	#define CAL_DevIsValidRequest pICmpDevice->IDevIsValidRequest
	#define CHK_DevIsValidRequest (pICmpDevice != NULL)
	#define EXP_DevIsValidRequest  CAL_CMEXPAPI( "DevIsValidRequest" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevIsValidRequest  PFDEVISVALIDREQUEST pfDevIsValidRequest;
	#define EXT_DevIsValidRequest  extern PFDEVISVALIDREQUEST pfDevIsValidRequest;
	#define GET_DevIsValidRequest(fl)  s_pfCMGetAPI2( "DevIsValidRequest", (RTS_VOID_FCTPTR *)&pfDevIsValidRequest, (fl), 0, 0)
	#define CAL_DevIsValidRequest  pfDevIsValidRequest
	#define CHK_DevIsValidRequest  (pfDevIsValidRequest != NULL)
	#define EXP_DevIsValidRequest  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevIsValidRequest", (RTS_UINTPTR)DevIsValidRequest, 0, 0) 
#endif




/** 
 * <description>
 *   Reset origin device. Only supported if CmpEventMgr is available.
 * </description>
 * <param name="hUser" type="IN">User handle of the current user.</param>
 * <param name="ui32ResetOriginConfig" type="IN">Configuration bit vector. See category "Reset Origin Configuration".</param>
 * <param name="pui32ResetOriginResults" type="OUT">Optional: pointer to the bit vector indicating the results for the requested configuration
 *		(bit=1: error deleting the corresponding object, bit=0: success).</param>
 * <result>
 *	<ul>
 *		<li>ERR_OK: Reset origin device successful</li>
 *		<li>ERR_FAILED: At least one object could not be deleted. Check pui32Results for more information.</li>
 *	</ul>
 * </result>
*/
RTS_RESULT CDECL DevResetOrigin(RTS_HANDLE hUser, RTS_UI32 ui32ResetOriginConfig, RTS_UI32 *pui32ResetOriginResults);
typedef RTS_RESULT (CDECL * PFDEVRESETORIGIN) (RTS_HANDLE hUser, RTS_UI32 ui32ResetOriginConfig, RTS_UI32 *pui32ResetOriginResults);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVRESETORIGIN_NOTIMPLEMENTED)
	#define USE_DevResetOrigin
	#define EXT_DevResetOrigin
	#define GET_DevResetOrigin(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevResetOrigin(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_DevResetOrigin  FALSE
	#define EXP_DevResetOrigin  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevResetOrigin
	#define EXT_DevResetOrigin
	#define GET_DevResetOrigin(fl)  CAL_CMGETAPI( "DevResetOrigin" ) 
	#define CAL_DevResetOrigin  DevResetOrigin
	#define CHK_DevResetOrigin  TRUE
	#define EXP_DevResetOrigin  CAL_CMEXPAPI( "DevResetOrigin" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevResetOrigin
	#define EXT_DevResetOrigin
	#define GET_DevResetOrigin(fl)  CAL_CMGETAPI( "DevResetOrigin" ) 
	#define CAL_DevResetOrigin  DevResetOrigin
	#define CHK_DevResetOrigin  TRUE
	#define EXP_DevResetOrigin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevResetOrigin", (RTS_UINTPTR)DevResetOrigin, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevResetOrigin
	#define EXT_CmpDeviceDevResetOrigin
	#define GET_CmpDeviceDevResetOrigin  ERR_OK
	#define CAL_CmpDeviceDevResetOrigin pICmpDevice->IDevResetOrigin
	#define CHK_CmpDeviceDevResetOrigin (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevResetOrigin  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevResetOrigin
	#define EXT_DevResetOrigin
	#define GET_DevResetOrigin(fl)  CAL_CMGETAPI( "DevResetOrigin" ) 
	#define CAL_DevResetOrigin pICmpDevice->IDevResetOrigin
	#define CHK_DevResetOrigin (pICmpDevice != NULL)
	#define EXP_DevResetOrigin  CAL_CMEXPAPI( "DevResetOrigin" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevResetOrigin  PFDEVRESETORIGIN pfDevResetOrigin;
	#define EXT_DevResetOrigin  extern PFDEVRESETORIGIN pfDevResetOrigin;
	#define GET_DevResetOrigin(fl)  s_pfCMGetAPI2( "DevResetOrigin", (RTS_VOID_FCTPTR *)&pfDevResetOrigin, (fl), 0, 0)
	#define CAL_DevResetOrigin  pfDevResetOrigin
	#define CHK_DevResetOrigin  (pfDevResetOrigin != NULL)
	#define EXP_DevResetOrigin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevResetOrigin", (RTS_UINTPTR)DevResetOrigin, 0, 0) 
#endif




/** 
 * <description>
 *   Get configuration for reset origin device. Returns the bit vector for objects that are handled by the reset and the corresponding rights for the specified user.
 * </description>
 * <param name="hUser" type="IN">User handle of the current user.</param>
 * <param name="pui32ResetOriginConfigOptions" type="OUT">Required: Pointer to the Configuration bit vector representing the possible objects that are handled. See category "Reset Origin Configuration".</param>
 * <param name="pui32ResetOriginConfigRights" type="OUT">Optional: pointer to the bit vector indicating the access rights for the specified user to delete the objects represented in pui32ResetOriginConfigOptions
 *		(bit=0: the current user is allowed to delete the corresponding object, bit=1: the user is not allowed to delete the corresponding object).</param>
 * <result>
 *	<ul>
 *		<li>ERR_OK: Configuration retrieved successfully</li>
 *		<li>ERR_PARAMETER: pui32ResetOriginConfigOptions is NULL</li>
 *	</ul>
 * </result>
*/
RTS_RESULT CDECL DevResetOriginGetConfig(RTS_HANDLE hUser, RTS_UI32 *pui32ResetOriginConfigOptions, RTS_UI32 *pui32ResetOriginConfigRights);
typedef RTS_RESULT (CDECL * PFDEVRESETORIGINGETCONFIG) (RTS_HANDLE hUser, RTS_UI32 *pui32ResetOriginConfigOptions, RTS_UI32 *pui32ResetOriginConfigRights);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVRESETORIGINGETCONFIG_NOTIMPLEMENTED)
	#define USE_DevResetOriginGetConfig
	#define EXT_DevResetOriginGetConfig
	#define GET_DevResetOriginGetConfig(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevResetOriginGetConfig(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_DevResetOriginGetConfig  FALSE
	#define EXP_DevResetOriginGetConfig  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevResetOriginGetConfig
	#define EXT_DevResetOriginGetConfig
	#define GET_DevResetOriginGetConfig(fl)  CAL_CMGETAPI( "DevResetOriginGetConfig" ) 
	#define CAL_DevResetOriginGetConfig  DevResetOriginGetConfig
	#define CHK_DevResetOriginGetConfig  TRUE
	#define EXP_DevResetOriginGetConfig  CAL_CMEXPAPI( "DevResetOriginGetConfig" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevResetOriginGetConfig
	#define EXT_DevResetOriginGetConfig
	#define GET_DevResetOriginGetConfig(fl)  CAL_CMGETAPI( "DevResetOriginGetConfig" ) 
	#define CAL_DevResetOriginGetConfig  DevResetOriginGetConfig
	#define CHK_DevResetOriginGetConfig  TRUE
	#define EXP_DevResetOriginGetConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevResetOriginGetConfig", (RTS_UINTPTR)DevResetOriginGetConfig, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevResetOriginGetConfig
	#define EXT_CmpDeviceDevResetOriginGetConfig
	#define GET_CmpDeviceDevResetOriginGetConfig  ERR_OK
	#define CAL_CmpDeviceDevResetOriginGetConfig pICmpDevice->IDevResetOriginGetConfig
	#define CHK_CmpDeviceDevResetOriginGetConfig (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevResetOriginGetConfig  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevResetOriginGetConfig
	#define EXT_DevResetOriginGetConfig
	#define GET_DevResetOriginGetConfig(fl)  CAL_CMGETAPI( "DevResetOriginGetConfig" ) 
	#define CAL_DevResetOriginGetConfig pICmpDevice->IDevResetOriginGetConfig
	#define CHK_DevResetOriginGetConfig (pICmpDevice != NULL)
	#define EXP_DevResetOriginGetConfig  CAL_CMEXPAPI( "DevResetOriginGetConfig" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevResetOriginGetConfig  PFDEVRESETORIGINGETCONFIG pfDevResetOriginGetConfig;
	#define EXT_DevResetOriginGetConfig  extern PFDEVRESETORIGINGETCONFIG pfDevResetOriginGetConfig;
	#define GET_DevResetOriginGetConfig(fl)  s_pfCMGetAPI2( "DevResetOriginGetConfig", (RTS_VOID_FCTPTR *)&pfDevResetOriginGetConfig, (fl), 0, 0)
	#define CAL_DevResetOriginGetConfig  pfDevResetOriginGetConfig
	#define CHK_DevResetOriginGetConfig  (pfDevResetOriginGetConfig != NULL)
	#define EXP_DevResetOriginGetConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevResetOriginGetConfig", (RTS_UINTPTR)DevResetOriginGetConfig, 0, 0) 
#endif




/** 
 * <description>
 *   Are there any other session on this channel, except this one.  
 * </description>
 * <param name="ui32ChannelId" type="IN">ID of the channel to be checked.</param>
 * <param name="ui32SessionId" type="OUT">Session ID.</param>
 * <result>
 *	<ul>
 *		<li>ERR_OK: The channel has other session than the given one.</li>
 *		<li>ERR_NO_OBJECT: The channel doesn't have any other session, except the given one.</li>
 *		<li>ERR_NOTINITIALIZED: The session management is not initialized.</li>
 *	</ul>
 * </result>
*/
RTS_RESULT CDECL DevSrvAnyOtherChannelSessions(RTS_UI32 ui32ChannelId, RTS_UI32 ui32SessionId);
typedef RTS_RESULT (CDECL * PFDEVSRVANYOTHERCHANNELSESSIONS) (RTS_UI32 ui32ChannelId, RTS_UI32 ui32SessionId);
#if defined(CMPDEVICE_NOTIMPLEMENTED) || defined(DEVSRVANYOTHERCHANNELSESSIONS_NOTIMPLEMENTED)
	#define USE_DevSrvAnyOtherChannelSessions
	#define EXT_DevSrvAnyOtherChannelSessions
	#define GET_DevSrvAnyOtherChannelSessions(fl)  ERR_NOTIMPLEMENTED
	#define CAL_DevSrvAnyOtherChannelSessions(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_DevSrvAnyOtherChannelSessions  FALSE
	#define EXP_DevSrvAnyOtherChannelSessions  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_DevSrvAnyOtherChannelSessions
	#define EXT_DevSrvAnyOtherChannelSessions
	#define GET_DevSrvAnyOtherChannelSessions(fl)  CAL_CMGETAPI( "DevSrvAnyOtherChannelSessions" ) 
	#define CAL_DevSrvAnyOtherChannelSessions  DevSrvAnyOtherChannelSessions
	#define CHK_DevSrvAnyOtherChannelSessions  TRUE
	#define EXP_DevSrvAnyOtherChannelSessions  CAL_CMEXPAPI( "DevSrvAnyOtherChannelSessions" ) 
#elif defined(MIXED_LINK) && !defined(CMPDEVICE_EXTERNAL)
	#define USE_DevSrvAnyOtherChannelSessions
	#define EXT_DevSrvAnyOtherChannelSessions
	#define GET_DevSrvAnyOtherChannelSessions(fl)  CAL_CMGETAPI( "DevSrvAnyOtherChannelSessions" ) 
	#define CAL_DevSrvAnyOtherChannelSessions  DevSrvAnyOtherChannelSessions
	#define CHK_DevSrvAnyOtherChannelSessions  TRUE
	#define EXP_DevSrvAnyOtherChannelSessions  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevSrvAnyOtherChannelSessions", (RTS_UINTPTR)DevSrvAnyOtherChannelSessions, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpDeviceDevSrvAnyOtherChannelSessions
	#define EXT_CmpDeviceDevSrvAnyOtherChannelSessions
	#define GET_CmpDeviceDevSrvAnyOtherChannelSessions  ERR_OK
	#define CAL_CmpDeviceDevSrvAnyOtherChannelSessions pICmpDevice->IDevSrvAnyOtherChannelSessions
	#define CHK_CmpDeviceDevSrvAnyOtherChannelSessions (pICmpDevice != NULL)
	#define EXP_CmpDeviceDevSrvAnyOtherChannelSessions  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_DevSrvAnyOtherChannelSessions
	#define EXT_DevSrvAnyOtherChannelSessions
	#define GET_DevSrvAnyOtherChannelSessions(fl)  CAL_CMGETAPI( "DevSrvAnyOtherChannelSessions" ) 
	#define CAL_DevSrvAnyOtherChannelSessions pICmpDevice->IDevSrvAnyOtherChannelSessions
	#define CHK_DevSrvAnyOtherChannelSessions (pICmpDevice != NULL)
	#define EXP_DevSrvAnyOtherChannelSessions  CAL_CMEXPAPI( "DevSrvAnyOtherChannelSessions" ) 
#else /* DYNAMIC_LINK */
	#define USE_DevSrvAnyOtherChannelSessions  PFDEVSRVANYOTHERCHANNELSESSIONS pfDevSrvAnyOtherChannelSessions;
	#define EXT_DevSrvAnyOtherChannelSessions  extern PFDEVSRVANYOTHERCHANNELSESSIONS pfDevSrvAnyOtherChannelSessions;
	#define GET_DevSrvAnyOtherChannelSessions(fl)  s_pfCMGetAPI2( "DevSrvAnyOtherChannelSessions", (RTS_VOID_FCTPTR *)&pfDevSrvAnyOtherChannelSessions, (fl), 0, 0)
	#define CAL_DevSrvAnyOtherChannelSessions  pfDevSrvAnyOtherChannelSessions
	#define CHK_DevSrvAnyOtherChannelSessions  (pfDevSrvAnyOtherChannelSessions != NULL)
	#define EXP_DevSrvAnyOtherChannelSessions  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"DevSrvAnyOtherChannelSessions", (RTS_UINTPTR)DevSrvAnyOtherChannelSessions, 0, 0) 
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
 	PFDEVSRVISSESSIONVALID IDevSrvIsSessionValid;
 	PFDEVGETSESSIONUSER IDevGetSessionUser;
 	PFDEVGETSESSIONAUTHTYPE IDevGetSessionAuthType;
 	PFDEVGETSESSIONFLAGS IDevGetSessionFlags;
 	PFDEVISVALIDREQUEST IDevIsValidRequest;
 	PFDEVRESETORIGIN IDevResetOrigin;
 	PFDEVRESETORIGINGETCONFIG IDevResetOriginGetConfig;
 	PFDEVSRVANYOTHERCHANNELSESSIONS IDevSrvAnyOtherChannelSessions;
 } ICmpDevice_C;

#ifdef CPLUSPLUS
class ICmpDevice : public IBase
{
	public:
		virtual RTS_RESULT CDECL IDevGetIdent(DeviceIdentification *pDevIdent) =0;
		virtual RTS_RESULT CDECL IDevCheckTargetIdent(const TargetIdent *pTargetIdentReq, const TargetIdent *pTargetIdent) =0;
		virtual RTS_RESULT CDECL IDevSetOperationMode(RTS_UI32 ui32OperationMode, CMPID *pCmpIdDisabled) =0;
		virtual RTS_RESULT CDECL IDevGetOperationMode(RTS_UI32 *pui32OperationMode) =0;
		virtual RTS_BOOL CDECL IDevSrvIsSessionValid(RTS_UI32 ui32ChannelId, RTS_UI32 ui32SessionId) =0;
		virtual RTS_HANDLE CDECL IDevGetSessionUser(RTS_UI32 ui32SessionId, RTS_RESULT *pResult) =0;
		virtual RTS_UI32 CDECL IDevGetSessionAuthType(RTS_UI32 ui32SessionId, RTS_RESULT *pResult) =0;
		virtual RTS_UI32 CDECL IDevGetSessionFlags(RTS_UI32 ui32SessionId, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IDevIsValidRequest(RTS_UI32 ulChannelHandle, HEADER_TAG_EXT *pHeaderTag, PROTOCOL_DATA_UNIT *pPduReply) =0;
		virtual RTS_RESULT CDECL IDevResetOrigin(RTS_HANDLE hUser, RTS_UI32 ui32ResetOriginConfig, RTS_UI32 *pui32ResetOriginResults) =0;
		virtual RTS_RESULT CDECL IDevResetOriginGetConfig(RTS_HANDLE hUser, RTS_UI32 *pui32ResetOriginConfigOptions, RTS_UI32 *pui32ResetOriginConfigRights) =0;
		virtual RTS_RESULT CDECL IDevSrvAnyOtherChannelSessions(RTS_UI32 ui32ChannelId, RTS_UI32 ui32SessionId) =0;
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
