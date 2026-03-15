 /**
 * <interfacename>CmpUserMgr</interfacename>
 * <description> 
 *  Interface for the user management component.
 *
 *  The user management in the runtime system is based on ACL (Access Control Lists). Here we have Users and UserGroups
 *  and access rights on objects.
 *
 *  Here are the principles of the user management:
 *
 *  1. There are users which are identified by its user name 
 *  2. A User can be a member of one or several UserGroups. But at least the user is a member of the group "Everyone".
 *  3. There are some predefined UserGroups, which covers the most typical use cases: 
 *     - "Administrator": Administrator group. A member of this groups has all access rights on all objects!
 *     - "Developer": Developer group. A member of this groups has all access to the IEC applications an all related stuff.
 *     - "Service": Service group. A member of this groups has all access to object for maintenance and service operations.
 *     - "Watch": Watch group. A member of this groups has read-only access to the objects.
 *     - "Everyone": Everyone group. A member of this groups has read-only access to the objects.
 *
 *  4. A UserGroup can be derived from another UserGroup an inherits all rights from the base group
 *  5. Every object in the runtime system, that must be protected with access rights is registered dynamically during startup of the runtime by the corresponding
 *     component under the appropriate folder.
 *     The objects in the runtime system are organized as a tree with 2 root nodes:
 *     - Runtime system object:
 *          "Device"
 *              |----- "PlcLogic"
 *              |----------- "Application"
 *              |----- "Logger"
 *              |----- "Settings"
 *              |----- "UserManagement"	
 *
 *     - Filesystem objects:
 *             "/"
 *              |----- "Temp"
 *              |----------- "Test.txt"
 *
 *  6. There are 5 different access rights that can be specified at every group for each object:
 *     - USERDB_RIGHT_NONE: Access denied
 *     - USERDB_RIGHT_VIEW: Only view rights on the object. Object cannot be modified in any case!
 *     - USERDB_RIGHT_MODIFY: Object can be viewed and modified
 *     - USERDB_RIGHT_EXECUTE: Object can be executed
 *     - USERDB_RIGHT_ADD_REMOVE: It is allowed to add and remove objects or subobjects
 *     - USERDB_RIGHT_ALL: All rights allowed on the object
 *
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPUSERMGRITF_H_
#define _CMPUSERMGRITF_H_

#include "CmpStd.h"

 

 




#include "CmpUserDBItf.h"
#include "CmpBinTagUtilItf.h"
#include "CmpDeviceItf.h"

#include "CmpItf.h"


/**
 * <category>Static defines</category>
 * <description>Number of predefined static session entries</description>
 */
#ifndef USERMGR_NUM_OF_STATIC_SESSIONS
	#define USERMGR_NUM_OF_STATIC_SESSIONS			8
#endif


/**
 * <category>Online services</category>
 * <description>
 *	Online service to read the user management.
 * </description>
 * <service name="SRV_READ_USER_MGMT">
 *	<Request>
 *		<No further tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_PROPERTY_USERS" required="mandatory">[RTS_UI32]: User property</tag>
 *		<tag name="TAG_PROPERTY_GROUPS" required="mandatory">[RTS_UI32]: Group property</tag>
 *		<tag name="TAG_USER" required="mandatory">Top level tag may contain the following sub tags</tag>
 *			<tag name="TAG_NAME" required="optional">[char*]: NUL terminated name of the user</tag>
 *			<tag name="TAG_PASSWORD" required="optional">[char*]: Password as MD5 hash</tag>
 *			<tag name="TAG_PROPERTY" required="optional">[RTS_UI32]: User property</tag>
 *		<tag name="TAG_GROUP" required="mandatory">Top level tag may contain the following sub tags</tag>
 *			<tag name="TAG_NAME" required="optional">[char*]: NUL terminated name of the user</tag>
 *			<tag name="TAG_PASSWORD" required="optional">[char*]: Password as MD5 hash</tag>
 *			<tag name="TAG_PROPERTY" required="optional">[RTS_UI32]: User property</tag>
 *	</Response>
 * </service>
 */
#define SRV_READ_USER_MGMT							0x01

/**
 * <category>Online services</category>
 * <description>Online service to write the user management</description>
 */
#define SRV_WRITE_USER_MGMT							0x02

/**
 * <category>Online services</category>
 * <description>Online service to read the user rights management</description>
 */
#define SRV_READ_USER_RIGHTS_MGMT					0x03

/**
 * <category>Online services</category>
 * <description>Online service to write the user rights management</description>
 */
#define SRV_WRITE_USER_RIGHTS_MGMT					0x04

/**
 * <category>Online services</category>
 * <description>Online service to add a single user</description>
 */
#define SRV_USER_ADD								0x05

/**
 * <category>Online services</category>
 * <description>Online service to add a single user</description>
 */
#define SRV_USER_DELETE								0x06

/**
 * <category>Online services</category>
 * <description>Online service to add a single user</description>
 */
#define SRV_USER_CHANGE_PASSWORD					0x07


/**
 * <category>Online tags</category>
 * <description>Property of the database for users</description>
 */
#define TAG_PROPERTY_USERS							0x01

/**
 * <category>Online tags</category>
 * <description>Property of the database for groups</description>
 */
#define TAG_PROPERTY_GROUPS							0x02

/**
 * <category>Online tags</category>
 * <description>Name tag</description>
 */
#define TAG_NAME									0x03

/**
 * <category>Online tags</category>
 * <description>Property of an entry</description>
 */
#define TAG_PROPERTY								0x04

/**
 * <category>Online tags</category>
 * <description>Property of an entry</description>
 */
#define TAG_PASSWORD								0x05

/**
 * <category>Online tags</category>
 * <description>Group member</description>
 */
#define TAG_GROUP_MEMBER							0x06

/**
 * <category>Online tags</category>
 * <description>User member</description>
 */
#define TAG_USER_MEMBER								0x07

/**
 * <category>Online tags</category>
 * <description>Property of the database for groups</description>
 */
#define TAG_RIGHTS									0x08

/**
 * <category>Online tags</category>
 * <description>Property of the database for groups</description>
 */
#define TAG_DENIED_RIGHTS							0x09

/**
 * <category>Online tags</category>
 * <description>ID of an object</description>
 */
#define TAG_ID										0x0A

/**
 * <category>Online tags</category>
 * <description>Parent ID of an object</description>
 */
#define TAG_PARENT_ID								0x0B

/**
 * <category>Online tags</category>
 * <description>Object handle</description>
 */
#define TAG_OBJECT_HANDLE							0x0C

/**
 * <category>Online tags</category>
 * <description>Group handle</description>
 */
#define TAG_GROUP_HANDLE							0x0D

/**
 * <category>Online tags</category>
 * <description>Used rights</description>
 */
#define TAG_USED_RIGHTS								0x0E

/**
 * <category>Online tags</category>
 * <description>Version of Online User Manager</description>
 */
#define TAG_USRMGR_VERSION							0x0F

/**
 * <category>Online tags</category>
 * <description>Contains user information</description>
 */
#define TAG_USER									0x81

/**
 * <category>Online tags</category>
 * <description>Contains group information:
 *                     - group name
 *                     - list of users in this group
 *                     - list of child groups derived from this group
 * </description>
 */
#define TAG_GROUP									0x82

/**
 * <category>Online tags</category>
 * <description>Contains object information</description>
 */
#define TAG_OBJECT									0x83


/**
 * <category>User management session entry</category>
 * <description>
 *	Contains the correlation between the SessionID of the connection and the logged in User.
 * </description>
 * <element name="ulSessionId" type="IN">SessionID of the connection</element>
 * <element name="hUser" type="IN">Handle of the logged in user on the SessionID</element>
 */
typedef struct
{
	RTS_UI32 ulSessionId;
	RTS_HANDLE hUser;
} UserMgrSessionEntry;


/**
 * <category>Crypt types</category>
 * <description>Crypt types to chiffre the password</description>
 * <element name="USERMGR_CRYPT_INVALID" type="IN">Invalid of unknown crypt type</element>
 * <element name="USERMGR_CRYPT_XOR" type="IN">Disguise password with an XOR algorithm</element>
 */
#define USERMGR_CRYPT_INVALID				0x00000000
#define USERMGR_CRYPT_XOR					0x00000001

/**
 * <category>Static defines</category>
 * <description>Predefined objects in the runtime</description>
 */
#define USERDB_OBJECT_USERMGMT		"Device.UserManagement"


/**
 * <category>Event parameter</category>
 * <element name="bUserManagementChanged" type="IN">TRUE: Usermanagement database changed; FALSE: Not changed</element>
 * <element name="bRightsManagementChanged" type="IN">TRUE: Rightsmanagement database changed; FALSE: Not changed</element>
 */
typedef struct
{
	RTS_BOOL bUserManagementChanged;
	RTS_BOOL bRightsManagementChanged;
} EVTPARAM_CmpUserMgrDatabaseChanged;

#define EVTPARAMID_CmpUserMgrDatabaseChanged	0x0001
#define EVTVERSION_CmpUserMgrDatabaseChanged	0x0001

/**
 * <category>Events</category>
 * <description>Event is sent every time the usermanagement database or the userrights database was changed</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpUserMgrDatabaseChanged</param>
 */
#define EVT_UserMgrDatabaseChanged		MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Static defines</category>
 * <description>Number of unsuccessful login retries to limit failed user authentication
 * In case USERMGR_NUM_OF_LOGIN_RETRIES is set to 0 this security feature is disabled.
 * </description>
 */
#ifndef USERMGR_NUM_OF_LOGIN_RETRIES
	#define USERMGR_NUM_OF_LOGIN_RETRIES			3
#endif

/**
 * <category>Static defines</category>
 * <description>Timeout after USERMGR_NUM_OF_LOGIN_RETRIES unsuccessful login tries in seconds, i.e. the user is temporarily excluded
 * The maximal timeout value for user exclusion is 4233600 seconds which is equivalent to 49 days. But this is not persistent, i.e. a restart of the runtime resets the exclusion.
 * </description>
 */
#ifdef USERMGR_TIMEOUT_OF_LOGIN_RETRIES
	#define USERMGR_USERLOCK_TIME				USERMGR_TIMEOUT_OF_LOGIN_RETRIES
#endif
#ifndef USERMGR_USERLOCK_TIME
	#define USERMGR_USERLOCK_TIME				60
#endif


/**
 * <category>SecuritySettings</category>
 * <description>Security setting of the behavior, if login authentication fails. Can be selected in CmpSecurityManager.
 *	NOTE:
 *		Security modes must be ordered in an descending sorting, i.e. the setting with the highest security level is found at the beginning of the list!
 * </description>
 */
#define CMPUSERMGR_KEY_STRING_LOGIN				CMPSETTINGS_CATEGORY_SECURITY ".UserLogin_ErrorBehavior"
#define CMPUSERMGR_KEY_STRING_LOGIN_DESC		"Behavior on user login errors"

#define CMPUSERMGR_VALUE_LOGIN_EXCLUSION		"EXCLUSION"
#define CMPUSERMGR_VALUE_LOGIN_TIMEOUT			"TIMEOUT"
#define CMPUSERMGR_VALUE_LOGIN_NOEXCLUSION		"NOEXCLUSION"

#ifndef CMPUSERMGR_VALUE_LOGIN_DEFAULT
	#define CMPUSERMGR_VALUE_LOGIN_DEFAULT		CMPUSERMGR_VALUE_LOGIN_TIMEOUT
#endif


/**
 * <category>SecuritySettings</category>
 * <description>Security settings of the behavior when login authentication fails.
 * </description>
 */
typedef enum
{
	UserMgrLoginErr_NoExclusion,
	UserMgrLoginErr_Timeout,
	UserMgrLoginErr_Exclusion
} USERMGR_LOGIN_ERROR;

#define CMPUSERMGR_VALUE_LOGIN_LIST				{ \
													{(RTS_I32)UserMgrLoginErr_Exclusion, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_LOGIN_EXCLUSION, "User is excluded permanently"},\
													{(RTS_I32)UserMgrLoginErr_Timeout, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_LOGIN_TIMEOUT, "User is excluded temporarily. See setting [UserLockTime] which specifies the locked time."},\
													{(RTS_I32)UserMgrLoginErr_NoExclusion, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_LOGIN_NOEXCLUSION, "No user exclusion"},\
												}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: Maximal number of failed login retries
 * </description>
 */
#define CMPUSERMGR_KEY_INT_MAXRETRIES				CMPSETTINGS_CATEGORY_SECURITY ".UserLogin_MaxRetries"
#define CMPUSERMGR_KEY_INT_MAXRETRIES_DESC			"Maximal number of failed login retries"

#ifndef CMPUSERMGR_VALUE_MAXRETRIES_DEFAULT
	#define CMPUSERMGR_VALUE_MAXRETRIES_DEFAULT		USERMGR_NUM_OF_LOGIN_RETRIES
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: Exclusion timeout in seconds in case the maximal number of failed login retries is reached
 * </description>
 */
#define CMPUSERMGR_KEY_INT_LOCKTIME					CMPSETTINGS_CATEGORY_SECURITY ".User_LockTime"
#define CMPUSERMGR_KEY_INT_LOCKTIME_DESC			"Time in [s] which the user is locked after a maximum number of failed login"

#ifndef CMPUSERMGR_VALUE_LOCKTIME_DEFAULT
	#define CMPUSERMGR_VALUE_LOCKTIME_DEFAULT		USERMGR_USERLOCK_TIME
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings to enforce the usage of the user management.
 *	NOTE:
 *		Security modes must be ordered in an descending sorting, i.e. the setting with the highest security level is found at the beginning of the list!
 * </description>
 */
#define CMPUSERMGR_KEY_STRING_ENFORCE				CMPSETTINGS_CATEGORY_SECURITY ".UserMgmtEnforce"
#define CMPUSERMGR_KEY_STRING_ENFORCE_DESC			"Setting to enforce user management in general!"

#define CMPUSERMGR_VALUE_ENFORCE_YES				"YES"
#define CMPUSERMGR_VALUE_ENFORCE_NO					"NO"

#ifndef CMPUSERMGR_VALUE_ENFORCE_DEFAULT
	#define CMPUSERMGR_VALUE_ENFORCE_DEFAULT		CMPUSERMGR_VALUE_ENFORCE_NO
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings to enforce the usage of the user management.
 * </description>
 */
typedef enum
{
	UserMgmtEnforce_Yes,
	UserMgmtEnforce_No
} SECURITY_USERMGR_ENFORCE;

#define CMPUSERMGR_VALUE_ENFORCE_LIST			{\
													{(RTS_I32)UserMgmtEnforce_Yes, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_ENFORCE_YES, "User management is enforced, i.e. no anonymous login possible"},\
													{(RTS_I32)UserMgmtEnforce_No, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_ENFORCE_NO, "User management is optional"}\
												}



/**
 * <category>User state</category>
 * <description>The bit definitions for UserMgrGetState
 * </description>
 * <element name="USERMGR_US_NONE" type="The user is known</element>
 * <element name="USERMGR_US_LOGGEDIN" type="OUT">The user has successfully logged in</element>
 * <element name="USERMGR_US_AUTHENTICATED" type="OUT">The user is authenticated e.g. by password verification</element>
 */
#define USERMGR_US_NONE				UINT32_C(0x00000000)
#define USERMGR_US_LOGGEDIN			UINT32_C(0x00000001)
#define USERMGR_US_AUTHENTICATED	UINT32_C(0x00000002)


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Login user on the usermanagement
 * NOTE: The returned handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="pszUser" type="IN">Name of the user</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User is available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If pszUser is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT or ERR_PARAMETER">The user is not available in the user management</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">Could not store user information</errorcode>
 * <result>Handle to the user or RTS_INVALID_HANDLE if not available</result>
 */
RTS_HANDLE CDECL UserMgrLogin(char *pszUser, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGRLOGIN) (char *pszUser, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRLOGIN_NOTIMPLEMENTED)
	#define USE_UserMgrLogin
	#define EXT_UserMgrLogin
	#define GET_UserMgrLogin(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrLogin(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrLogin  FALSE
	#define EXP_UserMgrLogin  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrLogin
	#define EXT_UserMgrLogin
	#define GET_UserMgrLogin(fl)  CAL_CMGETAPI( "UserMgrLogin" ) 
	#define CAL_UserMgrLogin  UserMgrLogin
	#define CHK_UserMgrLogin  TRUE
	#define EXP_UserMgrLogin  CAL_CMEXPAPI( "UserMgrLogin" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrLogin
	#define EXT_UserMgrLogin
	#define GET_UserMgrLogin(fl)  CAL_CMGETAPI( "UserMgrLogin" ) 
	#define CAL_UserMgrLogin  UserMgrLogin
	#define CHK_UserMgrLogin  TRUE
	#define EXP_UserMgrLogin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrLogin", (RTS_UINTPTR)UserMgrLogin, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrLogin
	#define EXT_CmpUserMgrUserMgrLogin
	#define GET_CmpUserMgrUserMgrLogin  ERR_OK
	#define CAL_CmpUserMgrUserMgrLogin pICmpUserMgr->IUserMgrLogin
	#define CHK_CmpUserMgrUserMgrLogin (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrLogin  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrLogin
	#define EXT_UserMgrLogin
	#define GET_UserMgrLogin(fl)  CAL_CMGETAPI( "UserMgrLogin" ) 
	#define CAL_UserMgrLogin pICmpUserMgr->IUserMgrLogin
	#define CHK_UserMgrLogin (pICmpUserMgr != NULL)
	#define EXP_UserMgrLogin  CAL_CMEXPAPI( "UserMgrLogin" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrLogin  PFUSERMGRLOGIN pfUserMgrLogin;
	#define EXT_UserMgrLogin  extern PFUSERMGRLOGIN pfUserMgrLogin;
	#define GET_UserMgrLogin(fl)  s_pfCMGetAPI2( "UserMgrLogin", (RTS_VOID_FCTPTR *)&pfUserMgrLogin, (fl), 0, 0)
	#define CAL_UserMgrLogin  pfUserMgrLogin
	#define CHK_UserMgrLogin  (pfUserMgrLogin != NULL)
	#define EXP_UserMgrLogin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrLogin", (RTS_UINTPTR)UserMgrLogin, 0, 0) 
#endif




/**
 * <description>Logout specified by User
 * NOTE: The requred user handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User is available and logout succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If user is not avilable</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser is invalid or unknown</errorcode>
 */
RTS_RESULT CDECL UserMgrLogout(RTS_HANDLE hUser);
typedef RTS_RESULT (CDECL * PFUSERMGRLOGOUT) (RTS_HANDLE hUser);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRLOGOUT_NOTIMPLEMENTED)
	#define USE_UserMgrLogout
	#define EXT_UserMgrLogout
	#define GET_UserMgrLogout(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrLogout(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrLogout  FALSE
	#define EXP_UserMgrLogout  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrLogout
	#define EXT_UserMgrLogout
	#define GET_UserMgrLogout(fl)  CAL_CMGETAPI( "UserMgrLogout" ) 
	#define CAL_UserMgrLogout  UserMgrLogout
	#define CHK_UserMgrLogout  TRUE
	#define EXP_UserMgrLogout  CAL_CMEXPAPI( "UserMgrLogout" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrLogout
	#define EXT_UserMgrLogout
	#define GET_UserMgrLogout(fl)  CAL_CMGETAPI( "UserMgrLogout" ) 
	#define CAL_UserMgrLogout  UserMgrLogout
	#define CHK_UserMgrLogout  TRUE
	#define EXP_UserMgrLogout  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrLogout", (RTS_UINTPTR)UserMgrLogout, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrLogout
	#define EXT_CmpUserMgrUserMgrLogout
	#define GET_CmpUserMgrUserMgrLogout  ERR_OK
	#define CAL_CmpUserMgrUserMgrLogout pICmpUserMgr->IUserMgrLogout
	#define CHK_CmpUserMgrUserMgrLogout (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrLogout  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrLogout
	#define EXT_UserMgrLogout
	#define GET_UserMgrLogout(fl)  CAL_CMGETAPI( "UserMgrLogout" ) 
	#define CAL_UserMgrLogout pICmpUserMgr->IUserMgrLogout
	#define CHK_UserMgrLogout (pICmpUserMgr != NULL)
	#define EXP_UserMgrLogout  CAL_CMEXPAPI( "UserMgrLogout" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrLogout  PFUSERMGRLOGOUT pfUserMgrLogout;
	#define EXT_UserMgrLogout  extern PFUSERMGRLOGOUT pfUserMgrLogout;
	#define GET_UserMgrLogout(fl)  s_pfCMGetAPI2( "UserMgrLogout", (RTS_VOID_FCTPTR *)&pfUserMgrLogout, (fl), 0, 0)
	#define CAL_UserMgrLogout  pfUserMgrLogout
	#define CHK_UserMgrLogout  (pfUserMgrLogout != NULL)
	#define EXP_UserMgrLogout  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrLogout", (RTS_UINTPTR)UserMgrLogout, 0, 0) 
#endif




/**
 * <description>Logout specified by SessionID</description>
 * <param name="ulSessionId" type="IN">SessionID of the device online connection (see CmpDeviceItf.h)</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">SessionID or user is available and logout succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If SessionID or user is not avilable</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">Related user handle of the session is invalid or unknown</errorcode>
 */
 RTS_RESULT CDECL UserMgrLogoutBySessionId(RTS_UI32 ulSessionId);
typedef RTS_RESULT (CDECL * PFUSERMGRLOGOUTBYSESSIONID) (RTS_UI32 ulSessionId);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRLOGOUTBYSESSIONID_NOTIMPLEMENTED)
	#define USE_UserMgrLogoutBySessionId
	#define EXT_UserMgrLogoutBySessionId
	#define GET_UserMgrLogoutBySessionId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrLogoutBySessionId(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrLogoutBySessionId  FALSE
	#define EXP_UserMgrLogoutBySessionId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrLogoutBySessionId
	#define EXT_UserMgrLogoutBySessionId
	#define GET_UserMgrLogoutBySessionId(fl)  CAL_CMGETAPI( "UserMgrLogoutBySessionId" ) 
	#define CAL_UserMgrLogoutBySessionId  UserMgrLogoutBySessionId
	#define CHK_UserMgrLogoutBySessionId  TRUE
	#define EXP_UserMgrLogoutBySessionId  CAL_CMEXPAPI( "UserMgrLogoutBySessionId" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrLogoutBySessionId
	#define EXT_UserMgrLogoutBySessionId
	#define GET_UserMgrLogoutBySessionId(fl)  CAL_CMGETAPI( "UserMgrLogoutBySessionId" ) 
	#define CAL_UserMgrLogoutBySessionId  UserMgrLogoutBySessionId
	#define CHK_UserMgrLogoutBySessionId  TRUE
	#define EXP_UserMgrLogoutBySessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrLogoutBySessionId", (RTS_UINTPTR)UserMgrLogoutBySessionId, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrLogoutBySessionId
	#define EXT_CmpUserMgrUserMgrLogoutBySessionId
	#define GET_CmpUserMgrUserMgrLogoutBySessionId  ERR_OK
	#define CAL_CmpUserMgrUserMgrLogoutBySessionId pICmpUserMgr->IUserMgrLogoutBySessionId
	#define CHK_CmpUserMgrUserMgrLogoutBySessionId (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrLogoutBySessionId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrLogoutBySessionId
	#define EXT_UserMgrLogoutBySessionId
	#define GET_UserMgrLogoutBySessionId(fl)  CAL_CMGETAPI( "UserMgrLogoutBySessionId" ) 
	#define CAL_UserMgrLogoutBySessionId pICmpUserMgr->IUserMgrLogoutBySessionId
	#define CHK_UserMgrLogoutBySessionId (pICmpUserMgr != NULL)
	#define EXP_UserMgrLogoutBySessionId  CAL_CMEXPAPI( "UserMgrLogoutBySessionId" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrLogoutBySessionId  PFUSERMGRLOGOUTBYSESSIONID pfUserMgrLogoutBySessionId;
	#define EXT_UserMgrLogoutBySessionId  extern PFUSERMGRLOGOUTBYSESSIONID pfUserMgrLogoutBySessionId;
	#define GET_UserMgrLogoutBySessionId(fl)  s_pfCMGetAPI2( "UserMgrLogoutBySessionId", (RTS_VOID_FCTPTR *)&pfUserMgrLogoutBySessionId, (fl), 0, 0)
	#define CAL_UserMgrLogoutBySessionId  pfUserMgrLogoutBySessionId
	#define CHK_UserMgrLogoutBySessionId  (pfUserMgrLogoutBySessionId != NULL)
	#define EXP_UserMgrLogoutBySessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrLogoutBySessionId", (RTS_UINTPTR)UserMgrLogoutBySessionId, 0, 0) 
#endif




/**
 * <description>Check user password against credentials (authentication)!
 * NOTE: The requred user handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pszPassword" type="IN">Pointer to the cleartext password!</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User is available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If user is not avilable</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">If user password does not match credentials</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser is invalid or unknown</errorcode>
 */
RTS_RESULT CDECL UserMgrCheckPassword(RTS_HANDLE hUser, char *pszPassword);
typedef RTS_RESULT (CDECL * PFUSERMGRCHECKPASSWORD) (RTS_HANDLE hUser, char *pszPassword);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRCHECKPASSWORD_NOTIMPLEMENTED)
	#define USE_UserMgrCheckPassword
	#define EXT_UserMgrCheckPassword
	#define GET_UserMgrCheckPassword(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrCheckPassword(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrCheckPassword  FALSE
	#define EXP_UserMgrCheckPassword  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrCheckPassword
	#define EXT_UserMgrCheckPassword
	#define GET_UserMgrCheckPassword(fl)  CAL_CMGETAPI( "UserMgrCheckPassword" ) 
	#define CAL_UserMgrCheckPassword  UserMgrCheckPassword
	#define CHK_UserMgrCheckPassword  TRUE
	#define EXP_UserMgrCheckPassword  CAL_CMEXPAPI( "UserMgrCheckPassword" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrCheckPassword
	#define EXT_UserMgrCheckPassword
	#define GET_UserMgrCheckPassword(fl)  CAL_CMGETAPI( "UserMgrCheckPassword" ) 
	#define CAL_UserMgrCheckPassword  UserMgrCheckPassword
	#define CHK_UserMgrCheckPassword  TRUE
	#define EXP_UserMgrCheckPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrCheckPassword", (RTS_UINTPTR)UserMgrCheckPassword, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrCheckPassword
	#define EXT_CmpUserMgrUserMgrCheckPassword
	#define GET_CmpUserMgrUserMgrCheckPassword  ERR_OK
	#define CAL_CmpUserMgrUserMgrCheckPassword pICmpUserMgr->IUserMgrCheckPassword
	#define CHK_CmpUserMgrUserMgrCheckPassword (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrCheckPassword  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrCheckPassword
	#define EXT_UserMgrCheckPassword
	#define GET_UserMgrCheckPassword(fl)  CAL_CMGETAPI( "UserMgrCheckPassword" ) 
	#define CAL_UserMgrCheckPassword pICmpUserMgr->IUserMgrCheckPassword
	#define CHK_UserMgrCheckPassword (pICmpUserMgr != NULL)
	#define EXP_UserMgrCheckPassword  CAL_CMEXPAPI( "UserMgrCheckPassword" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrCheckPassword  PFUSERMGRCHECKPASSWORD pfUserMgrCheckPassword;
	#define EXT_UserMgrCheckPassword  extern PFUSERMGRCHECKPASSWORD pfUserMgrCheckPassword;
	#define GET_UserMgrCheckPassword(fl)  s_pfCMGetAPI2( "UserMgrCheckPassword", (RTS_VOID_FCTPTR *)&pfUserMgrCheckPassword, (fl), 0, 0)
	#define CAL_UserMgrCheckPassword  pfUserMgrCheckPassword
	#define CHK_UserMgrCheckPassword  (pfUserMgrCheckPassword != NULL)
	#define EXP_UserMgrCheckPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrCheckPassword", (RTS_UINTPTR)UserMgrCheckPassword, 0, 0) 
#endif




/**
 * <description>Get properties of a user
 * NOTE: The requred user handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pulProperty" type="OUT">Pointer to return the properties. For details see category "UserDB properties"</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Properties could be retrieved</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Property pointer = NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser is invalid or unknown</errorcode>
 */
RTS_RESULT CDECL UserMgrGetProperty(RTS_HANDLE hUser, RTS_UI32 *pulProperty);
typedef RTS_RESULT (CDECL * PFUSERMGRGETPROPERTY) (RTS_HANDLE hUser, RTS_UI32 *pulProperty);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGETPROPERTY_NOTIMPLEMENTED)
	#define USE_UserMgrGetProperty
	#define EXT_UserMgrGetProperty
	#define GET_UserMgrGetProperty(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGetProperty(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGetProperty  FALSE
	#define EXP_UserMgrGetProperty  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGetProperty
	#define EXT_UserMgrGetProperty
	#define GET_UserMgrGetProperty(fl)  CAL_CMGETAPI( "UserMgrGetProperty" ) 
	#define CAL_UserMgrGetProperty  UserMgrGetProperty
	#define CHK_UserMgrGetProperty  TRUE
	#define EXP_UserMgrGetProperty  CAL_CMEXPAPI( "UserMgrGetProperty" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGetProperty
	#define EXT_UserMgrGetProperty
	#define GET_UserMgrGetProperty(fl)  CAL_CMGETAPI( "UserMgrGetProperty" ) 
	#define CAL_UserMgrGetProperty  UserMgrGetProperty
	#define CHK_UserMgrGetProperty  TRUE
	#define EXP_UserMgrGetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetProperty", (RTS_UINTPTR)UserMgrGetProperty, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGetProperty
	#define EXT_CmpUserMgrUserMgrGetProperty
	#define GET_CmpUserMgrUserMgrGetProperty  ERR_OK
	#define CAL_CmpUserMgrUserMgrGetProperty pICmpUserMgr->IUserMgrGetProperty
	#define CHK_CmpUserMgrUserMgrGetProperty (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGetProperty  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGetProperty
	#define EXT_UserMgrGetProperty
	#define GET_UserMgrGetProperty(fl)  CAL_CMGETAPI( "UserMgrGetProperty" ) 
	#define CAL_UserMgrGetProperty pICmpUserMgr->IUserMgrGetProperty
	#define CHK_UserMgrGetProperty (pICmpUserMgr != NULL)
	#define EXP_UserMgrGetProperty  CAL_CMEXPAPI( "UserMgrGetProperty" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGetProperty  PFUSERMGRGETPROPERTY pfUserMgrGetProperty;
	#define EXT_UserMgrGetProperty  extern PFUSERMGRGETPROPERTY pfUserMgrGetProperty;
	#define GET_UserMgrGetProperty(fl)  s_pfCMGetAPI2( "UserMgrGetProperty", (RTS_VOID_FCTPTR *)&pfUserMgrGetProperty, (fl), 0, 0)
	#define CAL_UserMgrGetProperty  pfUserMgrGetProperty
	#define CHK_UserMgrGetProperty  (pfUserMgrGetProperty != NULL)
	#define EXP_UserMgrGetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetProperty", (RTS_UINTPTR)UserMgrGetProperty, 0, 0) 
#endif




/**
 * <description>Check if the a user is a member of the given group
 * NOTE: The requred user handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="pszGroup" type="IN">Group name</param>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">The user is a member of the given group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">The user is not a member of the given group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser is invalid or unknown</errorcode>
 */
RTS_RESULT CDECL UserMgrIsGroupMember(char *pszGroup, RTS_HANDLE hUser);
typedef RTS_RESULT (CDECL * PFUSERMGRISGROUPMEMBER) (char *pszGroup, RTS_HANDLE hUser);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRISGROUPMEMBER_NOTIMPLEMENTED)
	#define USE_UserMgrIsGroupMember
	#define EXT_UserMgrIsGroupMember
	#define GET_UserMgrIsGroupMember(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrIsGroupMember(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrIsGroupMember  FALSE
	#define EXP_UserMgrIsGroupMember  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrIsGroupMember
	#define EXT_UserMgrIsGroupMember
	#define GET_UserMgrIsGroupMember(fl)  CAL_CMGETAPI( "UserMgrIsGroupMember" ) 
	#define CAL_UserMgrIsGroupMember  UserMgrIsGroupMember
	#define CHK_UserMgrIsGroupMember  TRUE
	#define EXP_UserMgrIsGroupMember  CAL_CMEXPAPI( "UserMgrIsGroupMember" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrIsGroupMember
	#define EXT_UserMgrIsGroupMember
	#define GET_UserMgrIsGroupMember(fl)  CAL_CMGETAPI( "UserMgrIsGroupMember" ) 
	#define CAL_UserMgrIsGroupMember  UserMgrIsGroupMember
	#define CHK_UserMgrIsGroupMember  TRUE
	#define EXP_UserMgrIsGroupMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsGroupMember", (RTS_UINTPTR)UserMgrIsGroupMember, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrIsGroupMember
	#define EXT_CmpUserMgrUserMgrIsGroupMember
	#define GET_CmpUserMgrUserMgrIsGroupMember  ERR_OK
	#define CAL_CmpUserMgrUserMgrIsGroupMember pICmpUserMgr->IUserMgrIsGroupMember
	#define CHK_CmpUserMgrUserMgrIsGroupMember (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrIsGroupMember  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrIsGroupMember
	#define EXT_UserMgrIsGroupMember
	#define GET_UserMgrIsGroupMember(fl)  CAL_CMGETAPI( "UserMgrIsGroupMember" ) 
	#define CAL_UserMgrIsGroupMember pICmpUserMgr->IUserMgrIsGroupMember
	#define CHK_UserMgrIsGroupMember (pICmpUserMgr != NULL)
	#define EXP_UserMgrIsGroupMember  CAL_CMEXPAPI( "UserMgrIsGroupMember" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrIsGroupMember  PFUSERMGRISGROUPMEMBER pfUserMgrIsGroupMember;
	#define EXT_UserMgrIsGroupMember  extern PFUSERMGRISGROUPMEMBER pfUserMgrIsGroupMember;
	#define GET_UserMgrIsGroupMember(fl)  s_pfCMGetAPI2( "UserMgrIsGroupMember", (RTS_VOID_FCTPTR *)&pfUserMgrIsGroupMember, (fl), 0, 0)
	#define CAL_UserMgrIsGroupMember  pfUserMgrIsGroupMember
	#define CHK_UserMgrIsGroupMember  (pfUserMgrIsGroupMember != NULL)
	#define EXP_UserMgrIsGroupMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsGroupMember", (RTS_UINTPTR)UserMgrIsGroupMember, 0, 0) 
#endif




/**
 * <description>Returns the user state
 * The returned bitfield shows whether the user is logged in and is authenticated (see category User state)
 * NOTE: The requred user handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>User state</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Success</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser is invalid or unknown</errorcode>
 */
RTS_UI32 CDECL UserMgrGetState(RTS_HANDLE hUser, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFUSERMGRGETSTATE) (RTS_HANDLE hUser, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGETSTATE_NOTIMPLEMENTED)
	#define USE_UserMgrGetState
	#define EXT_UserMgrGetState
	#define GET_UserMgrGetState(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGetState(p0,p1)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGetState  FALSE
	#define EXP_UserMgrGetState  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGetState
	#define EXT_UserMgrGetState
	#define GET_UserMgrGetState(fl)  CAL_CMGETAPI( "UserMgrGetState" ) 
	#define CAL_UserMgrGetState  UserMgrGetState
	#define CHK_UserMgrGetState  TRUE
	#define EXP_UserMgrGetState  CAL_CMEXPAPI( "UserMgrGetState" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGetState
	#define EXT_UserMgrGetState
	#define GET_UserMgrGetState(fl)  CAL_CMGETAPI( "UserMgrGetState" ) 
	#define CAL_UserMgrGetState  UserMgrGetState
	#define CHK_UserMgrGetState  TRUE
	#define EXP_UserMgrGetState  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetState", (RTS_UINTPTR)UserMgrGetState, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGetState
	#define EXT_CmpUserMgrUserMgrGetState
	#define GET_CmpUserMgrUserMgrGetState  ERR_OK
	#define CAL_CmpUserMgrUserMgrGetState pICmpUserMgr->IUserMgrGetState
	#define CHK_CmpUserMgrUserMgrGetState (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGetState  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGetState
	#define EXT_UserMgrGetState
	#define GET_UserMgrGetState(fl)  CAL_CMGETAPI( "UserMgrGetState" ) 
	#define CAL_UserMgrGetState pICmpUserMgr->IUserMgrGetState
	#define CHK_UserMgrGetState (pICmpUserMgr != NULL)
	#define EXP_UserMgrGetState  CAL_CMEXPAPI( "UserMgrGetState" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGetState  PFUSERMGRGETSTATE pfUserMgrGetState;
	#define EXT_UserMgrGetState  extern PFUSERMGRGETSTATE pfUserMgrGetState;
	#define GET_UserMgrGetState(fl)  s_pfCMGetAPI2( "UserMgrGetState", (RTS_VOID_FCTPTR *)&pfUserMgrGetState, (fl), 0, 0)
	#define CAL_UserMgrGetState  pfUserMgrGetState
	#define CHK_UserMgrGetState  (pfUserMgrGetState != NULL)
	#define EXP_UserMgrGetState  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetState", (RTS_UINTPTR)UserMgrGetState, 0, 0) 
#endif




/**
 * <description>Check user access rights on the specified object (authorization)!</description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="ulSessionId" type="IN">SessionID of the device online connection (see CmpDeviceItf.h)</param>
 * <param name="ulRequestedRights" type="IN">Requested rights on that object</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">If user has the requested rights on the object or if no user management is configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If SessionID or user is not avilable or the object does not exist</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">If something failed retrieving the access rights</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">Explicitly denied rights</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">Related user handle of the session is invalid or unknown</errorcode>
 */
RTS_RESULT CDECL UserMgrHasAccessRights(char *pszObject, RTS_UI32 ulSessionId, RTS_UI32 ulRequestedRights);
typedef RTS_RESULT (CDECL * PFUSERMGRHASACCESSRIGHTS) (char *pszObject, RTS_UI32 ulSessionId, RTS_UI32 ulRequestedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRHASACCESSRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrHasAccessRights
	#define EXT_UserMgrHasAccessRights
	#define GET_UserMgrHasAccessRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrHasAccessRights(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrHasAccessRights  FALSE
	#define EXP_UserMgrHasAccessRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrHasAccessRights
	#define EXT_UserMgrHasAccessRights
	#define GET_UserMgrHasAccessRights(fl)  CAL_CMGETAPI( "UserMgrHasAccessRights" ) 
	#define CAL_UserMgrHasAccessRights  UserMgrHasAccessRights
	#define CHK_UserMgrHasAccessRights  TRUE
	#define EXP_UserMgrHasAccessRights  CAL_CMEXPAPI( "UserMgrHasAccessRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrHasAccessRights
	#define EXT_UserMgrHasAccessRights
	#define GET_UserMgrHasAccessRights(fl)  CAL_CMGETAPI( "UserMgrHasAccessRights" ) 
	#define CAL_UserMgrHasAccessRights  UserMgrHasAccessRights
	#define CHK_UserMgrHasAccessRights  TRUE
	#define EXP_UserMgrHasAccessRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrHasAccessRights", (RTS_UINTPTR)UserMgrHasAccessRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrHasAccessRights
	#define EXT_CmpUserMgrUserMgrHasAccessRights
	#define GET_CmpUserMgrUserMgrHasAccessRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrHasAccessRights pICmpUserMgr->IUserMgrHasAccessRights
	#define CHK_CmpUserMgrUserMgrHasAccessRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrHasAccessRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrHasAccessRights
	#define EXT_UserMgrHasAccessRights
	#define GET_UserMgrHasAccessRights(fl)  CAL_CMGETAPI( "UserMgrHasAccessRights" ) 
	#define CAL_UserMgrHasAccessRights pICmpUserMgr->IUserMgrHasAccessRights
	#define CHK_UserMgrHasAccessRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrHasAccessRights  CAL_CMEXPAPI( "UserMgrHasAccessRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrHasAccessRights  PFUSERMGRHASACCESSRIGHTS pfUserMgrHasAccessRights;
	#define EXT_UserMgrHasAccessRights  extern PFUSERMGRHASACCESSRIGHTS pfUserMgrHasAccessRights;
	#define GET_UserMgrHasAccessRights(fl)  s_pfCMGetAPI2( "UserMgrHasAccessRights", (RTS_VOID_FCTPTR *)&pfUserMgrHasAccessRights, (fl), 0, 0)
	#define CAL_UserMgrHasAccessRights  pfUserMgrHasAccessRights
	#define CHK_UserMgrHasAccessRights  (pfUserMgrHasAccessRights != NULL)
	#define EXP_UserMgrHasAccessRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrHasAccessRights", (RTS_UINTPTR)UserMgrHasAccessRights, 0, 0) 
#endif




/**
 * <description>Get user access rights on the specified object (authorization)!</description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="ulSessionId" type="IN">SessionID of the device online connection (see CmpDeviceItf.h)</param>
 * <param name="pulRights" type="IN">Pointer to get admitted rights</param>
 * <param name="pulDeniedRights" type="IN">Pointer to get denied rights.
 *	 NOTE:
 *	 If the required right is denied _and_ admitted on the specified object, the denied right is more significant and so the access must be denied !!</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">If user has the requested rights on the object or if no user management is configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If SessionID or user is not avilable or the object does not exist</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">If something failed retrieving the access rights</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">Explicitly denied rights</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">Related user handle of the session is invalid or unknown</errorcode>
 */
RTS_RESULT CDECL UserMgrGetAccessRights(char *pszObject, RTS_UI32 ulSessionId, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights);
typedef RTS_RESULT (CDECL * PFUSERMGRGETACCESSRIGHTS) (char *pszObject, RTS_UI32 ulSessionId, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGETACCESSRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrGetAccessRights
	#define EXT_UserMgrGetAccessRights
	#define GET_UserMgrGetAccessRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGetAccessRights(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGetAccessRights  FALSE
	#define EXP_UserMgrGetAccessRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGetAccessRights
	#define EXT_UserMgrGetAccessRights
	#define GET_UserMgrGetAccessRights(fl)  CAL_CMGETAPI( "UserMgrGetAccessRights" ) 
	#define CAL_UserMgrGetAccessRights  UserMgrGetAccessRights
	#define CHK_UserMgrGetAccessRights  TRUE
	#define EXP_UserMgrGetAccessRights  CAL_CMEXPAPI( "UserMgrGetAccessRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGetAccessRights
	#define EXT_UserMgrGetAccessRights
	#define GET_UserMgrGetAccessRights(fl)  CAL_CMGETAPI( "UserMgrGetAccessRights" ) 
	#define CAL_UserMgrGetAccessRights  UserMgrGetAccessRights
	#define CHK_UserMgrGetAccessRights  TRUE
	#define EXP_UserMgrGetAccessRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetAccessRights", (RTS_UINTPTR)UserMgrGetAccessRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGetAccessRights
	#define EXT_CmpUserMgrUserMgrGetAccessRights
	#define GET_CmpUserMgrUserMgrGetAccessRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrGetAccessRights pICmpUserMgr->IUserMgrGetAccessRights
	#define CHK_CmpUserMgrUserMgrGetAccessRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGetAccessRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGetAccessRights
	#define EXT_UserMgrGetAccessRights
	#define GET_UserMgrGetAccessRights(fl)  CAL_CMGETAPI( "UserMgrGetAccessRights" ) 
	#define CAL_UserMgrGetAccessRights pICmpUserMgr->IUserMgrGetAccessRights
	#define CHK_UserMgrGetAccessRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrGetAccessRights  CAL_CMEXPAPI( "UserMgrGetAccessRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGetAccessRights  PFUSERMGRGETACCESSRIGHTS pfUserMgrGetAccessRights;
	#define EXT_UserMgrGetAccessRights  extern PFUSERMGRGETACCESSRIGHTS pfUserMgrGetAccessRights;
	#define GET_UserMgrGetAccessRights(fl)  s_pfCMGetAPI2( "UserMgrGetAccessRights", (RTS_VOID_FCTPTR *)&pfUserMgrGetAccessRights, (fl), 0, 0)
	#define CAL_UserMgrGetAccessRights  pfUserMgrGetAccessRights
	#define CHK_UserMgrGetAccessRights  (pfUserMgrGetAccessRights != NULL)
	#define EXP_UserMgrGetAccessRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetAccessRights", (RTS_UINTPTR)UserMgrGetAccessRights, 0, 0) 
#endif




/**
 * <description>Check user access rights on the specified object (authorization)!
 * NOTE: The requred user handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="ulRequestedRights" type="IN">Requested rights on that object</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">If user has the requested rights on the object or if no user management is configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If user is not avilable or the object does not exist</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">If something failed retrieving the access rights</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">Explicitly denied rights</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser is invalid or unknown</errorcode>
 */
RTS_RESULT CDECL UserMgrHasUserAccessRights(char *pszObject, RTS_HANDLE hUser, RTS_UI32 ulRequestedRights);
typedef RTS_RESULT (CDECL * PFUSERMGRHASUSERACCESSRIGHTS) (char *pszObject, RTS_HANDLE hUser, RTS_UI32 ulRequestedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRHASUSERACCESSRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrHasUserAccessRights
	#define EXT_UserMgrHasUserAccessRights
	#define GET_UserMgrHasUserAccessRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrHasUserAccessRights(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrHasUserAccessRights  FALSE
	#define EXP_UserMgrHasUserAccessRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrHasUserAccessRights
	#define EXT_UserMgrHasUserAccessRights
	#define GET_UserMgrHasUserAccessRights(fl)  CAL_CMGETAPI( "UserMgrHasUserAccessRights" ) 
	#define CAL_UserMgrHasUserAccessRights  UserMgrHasUserAccessRights
	#define CHK_UserMgrHasUserAccessRights  TRUE
	#define EXP_UserMgrHasUserAccessRights  CAL_CMEXPAPI( "UserMgrHasUserAccessRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrHasUserAccessRights
	#define EXT_UserMgrHasUserAccessRights
	#define GET_UserMgrHasUserAccessRights(fl)  CAL_CMGETAPI( "UserMgrHasUserAccessRights" ) 
	#define CAL_UserMgrHasUserAccessRights  UserMgrHasUserAccessRights
	#define CHK_UserMgrHasUserAccessRights  TRUE
	#define EXP_UserMgrHasUserAccessRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrHasUserAccessRights", (RTS_UINTPTR)UserMgrHasUserAccessRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrHasUserAccessRights
	#define EXT_CmpUserMgrUserMgrHasUserAccessRights
	#define GET_CmpUserMgrUserMgrHasUserAccessRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrHasUserAccessRights pICmpUserMgr->IUserMgrHasUserAccessRights
	#define CHK_CmpUserMgrUserMgrHasUserAccessRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrHasUserAccessRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrHasUserAccessRights
	#define EXT_UserMgrHasUserAccessRights
	#define GET_UserMgrHasUserAccessRights(fl)  CAL_CMGETAPI( "UserMgrHasUserAccessRights" ) 
	#define CAL_UserMgrHasUserAccessRights pICmpUserMgr->IUserMgrHasUserAccessRights
	#define CHK_UserMgrHasUserAccessRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrHasUserAccessRights  CAL_CMEXPAPI( "UserMgrHasUserAccessRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrHasUserAccessRights  PFUSERMGRHASUSERACCESSRIGHTS pfUserMgrHasUserAccessRights;
	#define EXT_UserMgrHasUserAccessRights  extern PFUSERMGRHASUSERACCESSRIGHTS pfUserMgrHasUserAccessRights;
	#define GET_UserMgrHasUserAccessRights(fl)  s_pfCMGetAPI2( "UserMgrHasUserAccessRights", (RTS_VOID_FCTPTR *)&pfUserMgrHasUserAccessRights, (fl), 0, 0)
	#define CAL_UserMgrHasUserAccessRights  pfUserMgrHasUserAccessRights
	#define CHK_UserMgrHasUserAccessRights  (pfUserMgrHasUserAccessRights != NULL)
	#define EXP_UserMgrHasUserAccessRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrHasUserAccessRights", (RTS_UINTPTR)UserMgrHasUserAccessRights, 0, 0) 
#endif




/**
 * <description>Get user access rights on the specified object (authorization)!
 * NOTE: The requred user handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pulRights" type="IN">Pointer to get admitted rights</param>
 * <param name="pulDeniedRights" type="IN">Pointer to get denied rights.
 *	 NOTE:
 *	 If the required right is denied _and_ admitted on the specified object, the denied right is more significant and so the access must be denied !!</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">If user has the requested rights on the object or if no user management is configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If user is not avilable or the object does not exist</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">If something failed retrieving the access rights</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">Explicitly denied rights</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser is invalid or unknown</errorcode>
 */
RTS_RESULT CDECL UserMgrGetUserAccessRights(char *pszObject, RTS_HANDLE hUser, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights);
typedef RTS_RESULT (CDECL * PFUSERMGRGETUSERACCESSRIGHTS) (char *pszObject, RTS_HANDLE hUser, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGETUSERACCESSRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrGetUserAccessRights
	#define EXT_UserMgrGetUserAccessRights
	#define GET_UserMgrGetUserAccessRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGetUserAccessRights(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGetUserAccessRights  FALSE
	#define EXP_UserMgrGetUserAccessRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGetUserAccessRights
	#define EXT_UserMgrGetUserAccessRights
	#define GET_UserMgrGetUserAccessRights(fl)  CAL_CMGETAPI( "UserMgrGetUserAccessRights" ) 
	#define CAL_UserMgrGetUserAccessRights  UserMgrGetUserAccessRights
	#define CHK_UserMgrGetUserAccessRights  TRUE
	#define EXP_UserMgrGetUserAccessRights  CAL_CMEXPAPI( "UserMgrGetUserAccessRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGetUserAccessRights
	#define EXT_UserMgrGetUserAccessRights
	#define GET_UserMgrGetUserAccessRights(fl)  CAL_CMGETAPI( "UserMgrGetUserAccessRights" ) 
	#define CAL_UserMgrGetUserAccessRights  UserMgrGetUserAccessRights
	#define CHK_UserMgrGetUserAccessRights  TRUE
	#define EXP_UserMgrGetUserAccessRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetUserAccessRights", (RTS_UINTPTR)UserMgrGetUserAccessRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGetUserAccessRights
	#define EXT_CmpUserMgrUserMgrGetUserAccessRights
	#define GET_CmpUserMgrUserMgrGetUserAccessRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrGetUserAccessRights pICmpUserMgr->IUserMgrGetUserAccessRights
	#define CHK_CmpUserMgrUserMgrGetUserAccessRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGetUserAccessRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGetUserAccessRights
	#define EXT_UserMgrGetUserAccessRights
	#define GET_UserMgrGetUserAccessRights(fl)  CAL_CMGETAPI( "UserMgrGetUserAccessRights" ) 
	#define CAL_UserMgrGetUserAccessRights pICmpUserMgr->IUserMgrGetUserAccessRights
	#define CHK_UserMgrGetUserAccessRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrGetUserAccessRights  CAL_CMEXPAPI( "UserMgrGetUserAccessRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGetUserAccessRights  PFUSERMGRGETUSERACCESSRIGHTS pfUserMgrGetUserAccessRights;
	#define EXT_UserMgrGetUserAccessRights  extern PFUSERMGRGETUSERACCESSRIGHTS pfUserMgrGetUserAccessRights;
	#define GET_UserMgrGetUserAccessRights(fl)  s_pfCMGetAPI2( "UserMgrGetUserAccessRights", (RTS_VOID_FCTPTR *)&pfUserMgrGetUserAccessRights, (fl), 0, 0)
	#define CAL_UserMgrGetUserAccessRights  pfUserMgrGetUserAccessRights
	#define CHK_UserMgrGetUserAccessRights  (pfUserMgrGetUserAccessRights != NULL)
	#define EXP_UserMgrGetUserAccessRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetUserAccessRights", (RTS_UINTPTR)UserMgrGetUserAccessRights, 0, 0) 
#endif




/**
 * <description>Add online SessionID to the user (bind the user at this session)
 * NOTE: The requred user handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="ulSessionId" type="IN">SessionID of the device online connection (see CmpDeviceItf.h)</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">If user has the requested rights on the object or if no user management is configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">If CmpUserMgr component is still not initialized</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">Could not store SessionID</errorcode>
 */
RTS_RESULT CDECL UserMgrAddSessionId(RTS_HANDLE hUser, RTS_UI32 *pulSessionId);
typedef RTS_RESULT (CDECL * PFUSERMGRADDSESSIONID) (RTS_HANDLE hUser, RTS_UI32 *pulSessionId);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRADDSESSIONID_NOTIMPLEMENTED)
	#define USE_UserMgrAddSessionId
	#define EXT_UserMgrAddSessionId
	#define GET_UserMgrAddSessionId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrAddSessionId(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrAddSessionId  FALSE
	#define EXP_UserMgrAddSessionId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrAddSessionId
	#define EXT_UserMgrAddSessionId
	#define GET_UserMgrAddSessionId(fl)  CAL_CMGETAPI( "UserMgrAddSessionId" ) 
	#define CAL_UserMgrAddSessionId  UserMgrAddSessionId
	#define CHK_UserMgrAddSessionId  TRUE
	#define EXP_UserMgrAddSessionId  CAL_CMEXPAPI( "UserMgrAddSessionId" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrAddSessionId
	#define EXT_UserMgrAddSessionId
	#define GET_UserMgrAddSessionId(fl)  CAL_CMGETAPI( "UserMgrAddSessionId" ) 
	#define CAL_UserMgrAddSessionId  UserMgrAddSessionId
	#define CHK_UserMgrAddSessionId  TRUE
	#define EXP_UserMgrAddSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddSessionId", (RTS_UINTPTR)UserMgrAddSessionId, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrAddSessionId
	#define EXT_CmpUserMgrUserMgrAddSessionId
	#define GET_CmpUserMgrUserMgrAddSessionId  ERR_OK
	#define CAL_CmpUserMgrUserMgrAddSessionId pICmpUserMgr->IUserMgrAddSessionId
	#define CHK_CmpUserMgrUserMgrAddSessionId (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrAddSessionId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrAddSessionId
	#define EXT_UserMgrAddSessionId
	#define GET_UserMgrAddSessionId(fl)  CAL_CMGETAPI( "UserMgrAddSessionId" ) 
	#define CAL_UserMgrAddSessionId pICmpUserMgr->IUserMgrAddSessionId
	#define CHK_UserMgrAddSessionId (pICmpUserMgr != NULL)
	#define EXP_UserMgrAddSessionId  CAL_CMEXPAPI( "UserMgrAddSessionId" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrAddSessionId  PFUSERMGRADDSESSIONID pfUserMgrAddSessionId;
	#define EXT_UserMgrAddSessionId  extern PFUSERMGRADDSESSIONID pfUserMgrAddSessionId;
	#define GET_UserMgrAddSessionId(fl)  s_pfCMGetAPI2( "UserMgrAddSessionId", (RTS_VOID_FCTPTR *)&pfUserMgrAddSessionId, (fl), 0, 0)
	#define CAL_UserMgrAddSessionId  pfUserMgrAddSessionId
	#define CHK_UserMgrAddSessionId  (pfUserMgrAddSessionId != NULL)
	#define EXP_UserMgrAddSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddSessionId", (RTS_UINTPTR)UserMgrAddSessionId, 0, 0) 
#endif




/**
 * <description>Remove online SessionID from the user (unbind the user from this session)</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="ulSessionId" type="IN">SessionID of the device online connection (see CmpDeviceItf.h)</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">If user has the requested rights on the object or if no user management is configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">If CmpUserMgr component is still not initialized</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">Could not store SessionID</errorcode>
 */
RTS_RESULT CDECL UserMgrRemoveSessionId(RTS_UI32 ulSessionId);
typedef RTS_RESULT (CDECL * PFUSERMGRREMOVESESSIONID) (RTS_UI32 ulSessionId);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREMOVESESSIONID_NOTIMPLEMENTED)
	#define USE_UserMgrRemoveSessionId
	#define EXT_UserMgrRemoveSessionId
	#define GET_UserMgrRemoveSessionId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRemoveSessionId(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRemoveSessionId  FALSE
	#define EXP_UserMgrRemoveSessionId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRemoveSessionId
	#define EXT_UserMgrRemoveSessionId
	#define GET_UserMgrRemoveSessionId(fl)  CAL_CMGETAPI( "UserMgrRemoveSessionId" ) 
	#define CAL_UserMgrRemoveSessionId  UserMgrRemoveSessionId
	#define CHK_UserMgrRemoveSessionId  TRUE
	#define EXP_UserMgrRemoveSessionId  CAL_CMEXPAPI( "UserMgrRemoveSessionId" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRemoveSessionId
	#define EXT_UserMgrRemoveSessionId
	#define GET_UserMgrRemoveSessionId(fl)  CAL_CMGETAPI( "UserMgrRemoveSessionId" ) 
	#define CAL_UserMgrRemoveSessionId  UserMgrRemoveSessionId
	#define CHK_UserMgrRemoveSessionId  TRUE
	#define EXP_UserMgrRemoveSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveSessionId", (RTS_UINTPTR)UserMgrRemoveSessionId, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRemoveSessionId
	#define EXT_CmpUserMgrUserMgrRemoveSessionId
	#define GET_CmpUserMgrUserMgrRemoveSessionId  ERR_OK
	#define CAL_CmpUserMgrUserMgrRemoveSessionId pICmpUserMgr->IUserMgrRemoveSessionId
	#define CHK_CmpUserMgrUserMgrRemoveSessionId (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRemoveSessionId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRemoveSessionId
	#define EXT_UserMgrRemoveSessionId
	#define GET_UserMgrRemoveSessionId(fl)  CAL_CMGETAPI( "UserMgrRemoveSessionId" ) 
	#define CAL_UserMgrRemoveSessionId pICmpUserMgr->IUserMgrRemoveSessionId
	#define CHK_UserMgrRemoveSessionId (pICmpUserMgr != NULL)
	#define EXP_UserMgrRemoveSessionId  CAL_CMEXPAPI( "UserMgrRemoveSessionId" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRemoveSessionId  PFUSERMGRREMOVESESSIONID pfUserMgrRemoveSessionId;
	#define EXT_UserMgrRemoveSessionId  extern PFUSERMGRREMOVESESSIONID pfUserMgrRemoveSessionId;
	#define GET_UserMgrRemoveSessionId(fl)  s_pfCMGetAPI2( "UserMgrRemoveSessionId", (RTS_VOID_FCTPTR *)&pfUserMgrRemoveSessionId, (fl), 0, 0)
	#define CAL_UserMgrRemoveSessionId  pfUserMgrRemoveSessionId
	#define CHK_UserMgrRemoveSessionId  (pfUserMgrRemoveSessionId != NULL)
	#define EXP_UserMgrRemoveSessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveSessionId", (RTS_UINTPTR)UserMgrRemoveSessionId, 0, 0) 
#endif




/**
 * <description>Returns the user bound at the SessionID
 * NOTE: The returned handle is not compatible with a handle from the UserDB, and these handles must not be mixed.
 * </description>
 * <param name="ulSessionId" type="IN">SessionID of the device online connection (see CmpDeviceItf.h)</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User is available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If SessionID is invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT or ERR_PARAMETER">The user is not available in the user management</errorcode>
 * <result>Handle to the user or RTS_INVALID_HANDLE if not available</result>
 */
RTS_HANDLE CDECL UserMgrFindUserBySessionId(RTS_UI32 ulSessionId, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGRFINDUSERBYSESSIONID) (RTS_UI32 ulSessionId, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRFINDUSERBYSESSIONID_NOTIMPLEMENTED)
	#define USE_UserMgrFindUserBySessionId
	#define EXT_UserMgrFindUserBySessionId
	#define GET_UserMgrFindUserBySessionId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrFindUserBySessionId(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrFindUserBySessionId  FALSE
	#define EXP_UserMgrFindUserBySessionId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrFindUserBySessionId
	#define EXT_UserMgrFindUserBySessionId
	#define GET_UserMgrFindUserBySessionId(fl)  CAL_CMGETAPI( "UserMgrFindUserBySessionId" ) 
	#define CAL_UserMgrFindUserBySessionId  UserMgrFindUserBySessionId
	#define CHK_UserMgrFindUserBySessionId  TRUE
	#define EXP_UserMgrFindUserBySessionId  CAL_CMEXPAPI( "UserMgrFindUserBySessionId" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrFindUserBySessionId
	#define EXT_UserMgrFindUserBySessionId
	#define GET_UserMgrFindUserBySessionId(fl)  CAL_CMGETAPI( "UserMgrFindUserBySessionId" ) 
	#define CAL_UserMgrFindUserBySessionId  UserMgrFindUserBySessionId
	#define CHK_UserMgrFindUserBySessionId  TRUE
	#define EXP_UserMgrFindUserBySessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrFindUserBySessionId", (RTS_UINTPTR)UserMgrFindUserBySessionId, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrFindUserBySessionId
	#define EXT_CmpUserMgrUserMgrFindUserBySessionId
	#define GET_CmpUserMgrUserMgrFindUserBySessionId  ERR_OK
	#define CAL_CmpUserMgrUserMgrFindUserBySessionId pICmpUserMgr->IUserMgrFindUserBySessionId
	#define CHK_CmpUserMgrUserMgrFindUserBySessionId (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrFindUserBySessionId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrFindUserBySessionId
	#define EXT_UserMgrFindUserBySessionId
	#define GET_UserMgrFindUserBySessionId(fl)  CAL_CMGETAPI( "UserMgrFindUserBySessionId" ) 
	#define CAL_UserMgrFindUserBySessionId pICmpUserMgr->IUserMgrFindUserBySessionId
	#define CHK_UserMgrFindUserBySessionId (pICmpUserMgr != NULL)
	#define EXP_UserMgrFindUserBySessionId  CAL_CMEXPAPI( "UserMgrFindUserBySessionId" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrFindUserBySessionId  PFUSERMGRFINDUSERBYSESSIONID pfUserMgrFindUserBySessionId;
	#define EXT_UserMgrFindUserBySessionId  extern PFUSERMGRFINDUSERBYSESSIONID pfUserMgrFindUserBySessionId;
	#define GET_UserMgrFindUserBySessionId(fl)  s_pfCMGetAPI2( "UserMgrFindUserBySessionId", (RTS_VOID_FCTPTR *)&pfUserMgrFindUserBySessionId, (fl), 0, 0)
	#define CAL_UserMgrFindUserBySessionId  pfUserMgrFindUserBySessionId
	#define CHK_UserMgrFindUserBySessionId  (pfUserMgrFindUserBySessionId != NULL)
	#define EXP_UserMgrFindUserBySessionId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrFindUserBySessionId", (RTS_UINTPTR)UserMgrFindUserBySessionId, 0, 0) 
#endif




/**
 * <description>Adds an value to an logged in user. This can be used as some kind of chache that exists as long as the user is logged in.</description>
 * <param name="hUser" type="IN">Handle to the user.</param>
 * <param name="key" type="IN">Access key for the chace value. This has to be used at UserMgrGetInfoOfUser to access the value.</param>
 * <param name="pValue" type="IN">Value to cache at the user.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Value cached</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">The same key has been used already</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">The caching failed</errorcode>
 */
RTS_RESULT CDECL UserMgrAddInfoToUser(RTS_HANDLE hUser, RTS_SIZE key, RTS_TYPEDVALUE *pValue);
typedef RTS_RESULT (CDECL * PFUSERMGRADDINFOTOUSER) (RTS_HANDLE hUser, RTS_SIZE key, RTS_TYPEDVALUE *pValue);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRADDINFOTOUSER_NOTIMPLEMENTED)
	#define USE_UserMgrAddInfoToUser
	#define EXT_UserMgrAddInfoToUser
	#define GET_UserMgrAddInfoToUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrAddInfoToUser(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrAddInfoToUser  FALSE
	#define EXP_UserMgrAddInfoToUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrAddInfoToUser
	#define EXT_UserMgrAddInfoToUser
	#define GET_UserMgrAddInfoToUser(fl)  CAL_CMGETAPI( "UserMgrAddInfoToUser" ) 
	#define CAL_UserMgrAddInfoToUser  UserMgrAddInfoToUser
	#define CHK_UserMgrAddInfoToUser  TRUE
	#define EXP_UserMgrAddInfoToUser  CAL_CMEXPAPI( "UserMgrAddInfoToUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrAddInfoToUser
	#define EXT_UserMgrAddInfoToUser
	#define GET_UserMgrAddInfoToUser(fl)  CAL_CMGETAPI( "UserMgrAddInfoToUser" ) 
	#define CAL_UserMgrAddInfoToUser  UserMgrAddInfoToUser
	#define CHK_UserMgrAddInfoToUser  TRUE
	#define EXP_UserMgrAddInfoToUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddInfoToUser", (RTS_UINTPTR)UserMgrAddInfoToUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrAddInfoToUser
	#define EXT_CmpUserMgrUserMgrAddInfoToUser
	#define GET_CmpUserMgrUserMgrAddInfoToUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrAddInfoToUser pICmpUserMgr->IUserMgrAddInfoToUser
	#define CHK_CmpUserMgrUserMgrAddInfoToUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrAddInfoToUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrAddInfoToUser
	#define EXT_UserMgrAddInfoToUser
	#define GET_UserMgrAddInfoToUser(fl)  CAL_CMGETAPI( "UserMgrAddInfoToUser" ) 
	#define CAL_UserMgrAddInfoToUser pICmpUserMgr->IUserMgrAddInfoToUser
	#define CHK_UserMgrAddInfoToUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrAddInfoToUser  CAL_CMEXPAPI( "UserMgrAddInfoToUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrAddInfoToUser  PFUSERMGRADDINFOTOUSER pfUserMgrAddInfoToUser;
	#define EXT_UserMgrAddInfoToUser  extern PFUSERMGRADDINFOTOUSER pfUserMgrAddInfoToUser;
	#define GET_UserMgrAddInfoToUser(fl)  s_pfCMGetAPI2( "UserMgrAddInfoToUser", (RTS_VOID_FCTPTR *)&pfUserMgrAddInfoToUser, (fl), 0, 0)
	#define CAL_UserMgrAddInfoToUser  pfUserMgrAddInfoToUser
	#define CHK_UserMgrAddInfoToUser  (pfUserMgrAddInfoToUser != NULL)
	#define EXP_UserMgrAddInfoToUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddInfoToUser", (RTS_UINTPTR)UserMgrAddInfoToUser, 0, 0) 
#endif




/**
 * <description>Removes a specific chace value from an user.</description>
 * <param name="hUser" type="IN">Handle to the user.</param>
 * <param name="key" type="IN">Key of the cache value.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Removing cache value successfull</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No cache value with this key available.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Removing cache value failed</errorcode>
 */
RTS_RESULT CDECL UserMgrRemoveInfoFromUser(RTS_HANDLE hUser, RTS_SIZE key);
typedef RTS_RESULT (CDECL * PFUSERMGRREMOVEINFOFROMUSER) (RTS_HANDLE hUser, RTS_SIZE key);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREMOVEINFOFROMUSER_NOTIMPLEMENTED)
	#define USE_UserMgrRemoveInfoFromUser
	#define EXT_UserMgrRemoveInfoFromUser
	#define GET_UserMgrRemoveInfoFromUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRemoveInfoFromUser(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRemoveInfoFromUser  FALSE
	#define EXP_UserMgrRemoveInfoFromUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRemoveInfoFromUser
	#define EXT_UserMgrRemoveInfoFromUser
	#define GET_UserMgrRemoveInfoFromUser(fl)  CAL_CMGETAPI( "UserMgrRemoveInfoFromUser" ) 
	#define CAL_UserMgrRemoveInfoFromUser  UserMgrRemoveInfoFromUser
	#define CHK_UserMgrRemoveInfoFromUser  TRUE
	#define EXP_UserMgrRemoveInfoFromUser  CAL_CMEXPAPI( "UserMgrRemoveInfoFromUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRemoveInfoFromUser
	#define EXT_UserMgrRemoveInfoFromUser
	#define GET_UserMgrRemoveInfoFromUser(fl)  CAL_CMGETAPI( "UserMgrRemoveInfoFromUser" ) 
	#define CAL_UserMgrRemoveInfoFromUser  UserMgrRemoveInfoFromUser
	#define CHK_UserMgrRemoveInfoFromUser  TRUE
	#define EXP_UserMgrRemoveInfoFromUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveInfoFromUser", (RTS_UINTPTR)UserMgrRemoveInfoFromUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRemoveInfoFromUser
	#define EXT_CmpUserMgrUserMgrRemoveInfoFromUser
	#define GET_CmpUserMgrUserMgrRemoveInfoFromUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrRemoveInfoFromUser pICmpUserMgr->IUserMgrRemoveInfoFromUser
	#define CHK_CmpUserMgrUserMgrRemoveInfoFromUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRemoveInfoFromUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRemoveInfoFromUser
	#define EXT_UserMgrRemoveInfoFromUser
	#define GET_UserMgrRemoveInfoFromUser(fl)  CAL_CMGETAPI( "UserMgrRemoveInfoFromUser" ) 
	#define CAL_UserMgrRemoveInfoFromUser pICmpUserMgr->IUserMgrRemoveInfoFromUser
	#define CHK_UserMgrRemoveInfoFromUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrRemoveInfoFromUser  CAL_CMEXPAPI( "UserMgrRemoveInfoFromUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRemoveInfoFromUser  PFUSERMGRREMOVEINFOFROMUSER pfUserMgrRemoveInfoFromUser;
	#define EXT_UserMgrRemoveInfoFromUser  extern PFUSERMGRREMOVEINFOFROMUSER pfUserMgrRemoveInfoFromUser;
	#define GET_UserMgrRemoveInfoFromUser(fl)  s_pfCMGetAPI2( "UserMgrRemoveInfoFromUser", (RTS_VOID_FCTPTR *)&pfUserMgrRemoveInfoFromUser, (fl), 0, 0)
	#define CAL_UserMgrRemoveInfoFromUser  pfUserMgrRemoveInfoFromUser
	#define CHK_UserMgrRemoveInfoFromUser  (pfUserMgrRemoveInfoFromUser != NULL)
	#define EXP_UserMgrRemoveInfoFromUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveInfoFromUser", (RTS_UINTPTR)UserMgrRemoveInfoFromUser, 0, 0) 
#endif




/**
 * <description>Removes a specific chace value from all logged in users.</description>
 * <param name="key" type="IN">Key of the cache value.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Removing cache value successfull</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Removing cache value failed</errorcode>
 */
RTS_RESULT CDECL UserMgrRemoveInfoFromAllUsers(RTS_SIZE key);
typedef RTS_RESULT (CDECL * PFUSERMGRREMOVEINFOFROMALLUSERS) (RTS_SIZE key);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREMOVEINFOFROMALLUSERS_NOTIMPLEMENTED)
	#define USE_UserMgrRemoveInfoFromAllUsers
	#define EXT_UserMgrRemoveInfoFromAllUsers
	#define GET_UserMgrRemoveInfoFromAllUsers(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRemoveInfoFromAllUsers(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRemoveInfoFromAllUsers  FALSE
	#define EXP_UserMgrRemoveInfoFromAllUsers  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRemoveInfoFromAllUsers
	#define EXT_UserMgrRemoveInfoFromAllUsers
	#define GET_UserMgrRemoveInfoFromAllUsers(fl)  CAL_CMGETAPI( "UserMgrRemoveInfoFromAllUsers" ) 
	#define CAL_UserMgrRemoveInfoFromAllUsers  UserMgrRemoveInfoFromAllUsers
	#define CHK_UserMgrRemoveInfoFromAllUsers  TRUE
	#define EXP_UserMgrRemoveInfoFromAllUsers  CAL_CMEXPAPI( "UserMgrRemoveInfoFromAllUsers" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRemoveInfoFromAllUsers
	#define EXT_UserMgrRemoveInfoFromAllUsers
	#define GET_UserMgrRemoveInfoFromAllUsers(fl)  CAL_CMGETAPI( "UserMgrRemoveInfoFromAllUsers" ) 
	#define CAL_UserMgrRemoveInfoFromAllUsers  UserMgrRemoveInfoFromAllUsers
	#define CHK_UserMgrRemoveInfoFromAllUsers  TRUE
	#define EXP_UserMgrRemoveInfoFromAllUsers  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveInfoFromAllUsers", (RTS_UINTPTR)UserMgrRemoveInfoFromAllUsers, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRemoveInfoFromAllUsers
	#define EXT_CmpUserMgrUserMgrRemoveInfoFromAllUsers
	#define GET_CmpUserMgrUserMgrRemoveInfoFromAllUsers  ERR_OK
	#define CAL_CmpUserMgrUserMgrRemoveInfoFromAllUsers pICmpUserMgr->IUserMgrRemoveInfoFromAllUsers
	#define CHK_CmpUserMgrUserMgrRemoveInfoFromAllUsers (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRemoveInfoFromAllUsers  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRemoveInfoFromAllUsers
	#define EXT_UserMgrRemoveInfoFromAllUsers
	#define GET_UserMgrRemoveInfoFromAllUsers(fl)  CAL_CMGETAPI( "UserMgrRemoveInfoFromAllUsers" ) 
	#define CAL_UserMgrRemoveInfoFromAllUsers pICmpUserMgr->IUserMgrRemoveInfoFromAllUsers
	#define CHK_UserMgrRemoveInfoFromAllUsers (pICmpUserMgr != NULL)
	#define EXP_UserMgrRemoveInfoFromAllUsers  CAL_CMEXPAPI( "UserMgrRemoveInfoFromAllUsers" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRemoveInfoFromAllUsers  PFUSERMGRREMOVEINFOFROMALLUSERS pfUserMgrRemoveInfoFromAllUsers;
	#define EXT_UserMgrRemoveInfoFromAllUsers  extern PFUSERMGRREMOVEINFOFROMALLUSERS pfUserMgrRemoveInfoFromAllUsers;
	#define GET_UserMgrRemoveInfoFromAllUsers(fl)  s_pfCMGetAPI2( "UserMgrRemoveInfoFromAllUsers", (RTS_VOID_FCTPTR *)&pfUserMgrRemoveInfoFromAllUsers, (fl), 0, 0)
	#define CAL_UserMgrRemoveInfoFromAllUsers  pfUserMgrRemoveInfoFromAllUsers
	#define CHK_UserMgrRemoveInfoFromAllUsers  (pfUserMgrRemoveInfoFromAllUsers != NULL)
	#define EXP_UserMgrRemoveInfoFromAllUsers  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveInfoFromAllUsers", (RTS_UINTPTR)UserMgrRemoveInfoFromAllUsers, 0, 0) 
#endif




/**
 * <description>Retrieves the value cached using UserMgrAddInfoToUser.</description>
 * <param name="hUser" type="IN">Handle to the user.</param>
 * <param name="key" type="IN">Key of the cache value.</param>
 * <param name="pValue" type="IN">Value to cache at the user.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Retrieving cache value successfull. Value copied to pValue.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Some parameter is invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No cache value with this key available.</errorcode>
 */
RTS_RESULT CDECL UserMgrGetInfoOfUser(RTS_HANDLE hUser, RTS_SIZE key, RTS_TYPEDVALUE *pValue);
typedef RTS_RESULT (CDECL * PFUSERMGRGETINFOOFUSER) (RTS_HANDLE hUser, RTS_SIZE key, RTS_TYPEDVALUE *pValue);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGETINFOOFUSER_NOTIMPLEMENTED)
	#define USE_UserMgrGetInfoOfUser
	#define EXT_UserMgrGetInfoOfUser
	#define GET_UserMgrGetInfoOfUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGetInfoOfUser(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGetInfoOfUser  FALSE
	#define EXP_UserMgrGetInfoOfUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGetInfoOfUser
	#define EXT_UserMgrGetInfoOfUser
	#define GET_UserMgrGetInfoOfUser(fl)  CAL_CMGETAPI( "UserMgrGetInfoOfUser" ) 
	#define CAL_UserMgrGetInfoOfUser  UserMgrGetInfoOfUser
	#define CHK_UserMgrGetInfoOfUser  TRUE
	#define EXP_UserMgrGetInfoOfUser  CAL_CMEXPAPI( "UserMgrGetInfoOfUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGetInfoOfUser
	#define EXT_UserMgrGetInfoOfUser
	#define GET_UserMgrGetInfoOfUser(fl)  CAL_CMGETAPI( "UserMgrGetInfoOfUser" ) 
	#define CAL_UserMgrGetInfoOfUser  UserMgrGetInfoOfUser
	#define CHK_UserMgrGetInfoOfUser  TRUE
	#define EXP_UserMgrGetInfoOfUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetInfoOfUser", (RTS_UINTPTR)UserMgrGetInfoOfUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGetInfoOfUser
	#define EXT_CmpUserMgrUserMgrGetInfoOfUser
	#define GET_CmpUserMgrUserMgrGetInfoOfUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrGetInfoOfUser pICmpUserMgr->IUserMgrGetInfoOfUser
	#define CHK_CmpUserMgrUserMgrGetInfoOfUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGetInfoOfUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGetInfoOfUser
	#define EXT_UserMgrGetInfoOfUser
	#define GET_UserMgrGetInfoOfUser(fl)  CAL_CMGETAPI( "UserMgrGetInfoOfUser" ) 
	#define CAL_UserMgrGetInfoOfUser pICmpUserMgr->IUserMgrGetInfoOfUser
	#define CHK_UserMgrGetInfoOfUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrGetInfoOfUser  CAL_CMEXPAPI( "UserMgrGetInfoOfUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGetInfoOfUser  PFUSERMGRGETINFOOFUSER pfUserMgrGetInfoOfUser;
	#define EXT_UserMgrGetInfoOfUser  extern PFUSERMGRGETINFOOFUSER pfUserMgrGetInfoOfUser;
	#define GET_UserMgrGetInfoOfUser(fl)  s_pfCMGetAPI2( "UserMgrGetInfoOfUser", (RTS_VOID_FCTPTR *)&pfUserMgrGetInfoOfUser, (fl), 0, 0)
	#define CAL_UserMgrGetInfoOfUser  pfUserMgrGetInfoOfUser
	#define CHK_UserMgrGetInfoOfUser  (pfUserMgrGetInfoOfUser != NULL)
	#define EXP_UserMgrGetInfoOfUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetInfoOfUser", (RTS_UINTPTR)UserMgrGetInfoOfUser, 0, 0) 
#endif




/**
 * <description>Add the error tag to the reply stream of an online service, if the service is denied because of a failed access right (authorization error)!</description>
 * <param name="pWriter" type="IN">Pointer the writer stream</param>
 * <param name="Result" type="IN">Result code of the operation</param>
 * <param name="pszObject" type="IN">Pointer to name of the object, whose access was denied</param>
 * <param name="ulRequestedRights" type="IN">Requested rights on the object, which are not enabled</param>
 * <param name="ulSessionId" type="IN">SessionID of the device online connection (see CmpDeviceItf.h)</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Reply tag could be created</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Reply tag could not be created</errorcode>
 */
RTS_RESULT CDECL UserMgrAddOnlineAccessError(BINTAGWRITER *pWriter, RTS_RESULT Result, char *pszObject, RTS_UI32 ulRequestedRights, RTS_UI32 ulSessionID);
typedef RTS_RESULT (CDECL * PFUSERMGRADDONLINEACCESSERROR) (BINTAGWRITER *pWriter, RTS_RESULT Result, char *pszObject, RTS_UI32 ulRequestedRights, RTS_UI32 ulSessionID);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRADDONLINEACCESSERROR_NOTIMPLEMENTED)
	#define USE_UserMgrAddOnlineAccessError
	#define EXT_UserMgrAddOnlineAccessError
	#define GET_UserMgrAddOnlineAccessError(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrAddOnlineAccessError(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrAddOnlineAccessError  FALSE
	#define EXP_UserMgrAddOnlineAccessError  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrAddOnlineAccessError
	#define EXT_UserMgrAddOnlineAccessError
	#define GET_UserMgrAddOnlineAccessError(fl)  CAL_CMGETAPI( "UserMgrAddOnlineAccessError" ) 
	#define CAL_UserMgrAddOnlineAccessError  UserMgrAddOnlineAccessError
	#define CHK_UserMgrAddOnlineAccessError  TRUE
	#define EXP_UserMgrAddOnlineAccessError  CAL_CMEXPAPI( "UserMgrAddOnlineAccessError" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrAddOnlineAccessError
	#define EXT_UserMgrAddOnlineAccessError
	#define GET_UserMgrAddOnlineAccessError(fl)  CAL_CMGETAPI( "UserMgrAddOnlineAccessError" ) 
	#define CAL_UserMgrAddOnlineAccessError  UserMgrAddOnlineAccessError
	#define CHK_UserMgrAddOnlineAccessError  TRUE
	#define EXP_UserMgrAddOnlineAccessError  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddOnlineAccessError", (RTS_UINTPTR)UserMgrAddOnlineAccessError, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrAddOnlineAccessError
	#define EXT_CmpUserMgrUserMgrAddOnlineAccessError
	#define GET_CmpUserMgrUserMgrAddOnlineAccessError  ERR_OK
	#define CAL_CmpUserMgrUserMgrAddOnlineAccessError pICmpUserMgr->IUserMgrAddOnlineAccessError
	#define CHK_CmpUserMgrUserMgrAddOnlineAccessError (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrAddOnlineAccessError  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrAddOnlineAccessError
	#define EXT_UserMgrAddOnlineAccessError
	#define GET_UserMgrAddOnlineAccessError(fl)  CAL_CMGETAPI( "UserMgrAddOnlineAccessError" ) 
	#define CAL_UserMgrAddOnlineAccessError pICmpUserMgr->IUserMgrAddOnlineAccessError
	#define CHK_UserMgrAddOnlineAccessError (pICmpUserMgr != NULL)
	#define EXP_UserMgrAddOnlineAccessError  CAL_CMEXPAPI( "UserMgrAddOnlineAccessError" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrAddOnlineAccessError  PFUSERMGRADDONLINEACCESSERROR pfUserMgrAddOnlineAccessError;
	#define EXT_UserMgrAddOnlineAccessError  extern PFUSERMGRADDONLINEACCESSERROR pfUserMgrAddOnlineAccessError;
	#define GET_UserMgrAddOnlineAccessError(fl)  s_pfCMGetAPI2( "UserMgrAddOnlineAccessError", (RTS_VOID_FCTPTR *)&pfUserMgrAddOnlineAccessError, (fl), 0, 0)
	#define CAL_UserMgrAddOnlineAccessError  pfUserMgrAddOnlineAccessError
	#define CHK_UserMgrAddOnlineAccessError  (pfUserMgrAddOnlineAccessError != NULL)
	#define EXP_UserMgrAddOnlineAccessError  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddOnlineAccessError", (RTS_UINTPTR)UserMgrAddOnlineAccessError, 0, 0) 
#endif




/**
 * <description>
 *	Add a new object to the user management, so the access rights can be configured on that object.
 *	NOTE:
 *	The name of the object must include the full namespace with "Device" as the root node, e.g. "Device.MyObject" or if it's a filesystem object with "/" as the root node.
 * </description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Object could be added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Object invalid = NULL or empty</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">If memory could not be allocated to store this object</errorcode>
 * <result>Handle to the object or RTS_INVALID_HANDLE if it could not be added</result>
 */
RTS_HANDLE CDECL UserMgrObjectAdd(char *pszObject, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTADD) (char *pszObject, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTADD_NOTIMPLEMENTED)
	#define USE_UserMgrObjectAdd
	#define EXT_UserMgrObjectAdd
	#define GET_UserMgrObjectAdd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectAdd(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectAdd  FALSE
	#define EXP_UserMgrObjectAdd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectAdd
	#define EXT_UserMgrObjectAdd
	#define GET_UserMgrObjectAdd(fl)  CAL_CMGETAPI( "UserMgrObjectAdd" ) 
	#define CAL_UserMgrObjectAdd  UserMgrObjectAdd
	#define CHK_UserMgrObjectAdd  TRUE
	#define EXP_UserMgrObjectAdd  CAL_CMEXPAPI( "UserMgrObjectAdd" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectAdd
	#define EXT_UserMgrObjectAdd
	#define GET_UserMgrObjectAdd(fl)  CAL_CMGETAPI( "UserMgrObjectAdd" ) 
	#define CAL_UserMgrObjectAdd  UserMgrObjectAdd
	#define CHK_UserMgrObjectAdd  TRUE
	#define EXP_UserMgrObjectAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAdd", (RTS_UINTPTR)UserMgrObjectAdd, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectAdd
	#define EXT_CmpUserMgrUserMgrObjectAdd
	#define GET_CmpUserMgrUserMgrObjectAdd  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectAdd pICmpUserMgr->IUserMgrObjectAdd
	#define CHK_CmpUserMgrUserMgrObjectAdd (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectAdd  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectAdd
	#define EXT_UserMgrObjectAdd
	#define GET_UserMgrObjectAdd(fl)  CAL_CMGETAPI( "UserMgrObjectAdd" ) 
	#define CAL_UserMgrObjectAdd pICmpUserMgr->IUserMgrObjectAdd
	#define CHK_UserMgrObjectAdd (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectAdd  CAL_CMEXPAPI( "UserMgrObjectAdd" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectAdd  PFUSERMGROBJECTADD pfUserMgrObjectAdd;
	#define EXT_UserMgrObjectAdd  extern PFUSERMGROBJECTADD pfUserMgrObjectAdd;
	#define GET_UserMgrObjectAdd(fl)  s_pfCMGetAPI2( "UserMgrObjectAdd", (RTS_VOID_FCTPTR *)&pfUserMgrObjectAdd, (fl), 0, 0)
	#define CAL_UserMgrObjectAdd  pfUserMgrObjectAdd
	#define CHK_UserMgrObjectAdd  (pfUserMgrObjectAdd != NULL)
	#define EXP_UserMgrObjectAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAdd", (RTS_UINTPTR)UserMgrObjectAdd, 0, 0) 
#endif




/**
 * <description>Remove an object from the user management and all its children</description>
 * <param name="hObject" type="IN">Handle to object</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Object could be removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid object handle</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectRemove(RTS_HANDLE hObject);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTREMOVE) (RTS_HANDLE hObject);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTREMOVE_NOTIMPLEMENTED)
	#define USE_UserMgrObjectRemove
	#define EXT_UserMgrObjectRemove
	#define GET_UserMgrObjectRemove(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectRemove(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectRemove  FALSE
	#define EXP_UserMgrObjectRemove  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectRemove
	#define EXT_UserMgrObjectRemove
	#define GET_UserMgrObjectRemove(fl)  CAL_CMGETAPI( "UserMgrObjectRemove" ) 
	#define CAL_UserMgrObjectRemove  UserMgrObjectRemove
	#define CHK_UserMgrObjectRemove  TRUE
	#define EXP_UserMgrObjectRemove  CAL_CMEXPAPI( "UserMgrObjectRemove" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectRemove
	#define EXT_UserMgrObjectRemove
	#define GET_UserMgrObjectRemove(fl)  CAL_CMGETAPI( "UserMgrObjectRemove" ) 
	#define CAL_UserMgrObjectRemove  UserMgrObjectRemove
	#define CHK_UserMgrObjectRemove  TRUE
	#define EXP_UserMgrObjectRemove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectRemove", (RTS_UINTPTR)UserMgrObjectRemove, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectRemove
	#define EXT_CmpUserMgrUserMgrObjectRemove
	#define GET_CmpUserMgrUserMgrObjectRemove  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectRemove pICmpUserMgr->IUserMgrObjectRemove
	#define CHK_CmpUserMgrUserMgrObjectRemove (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectRemove  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectRemove
	#define EXT_UserMgrObjectRemove
	#define GET_UserMgrObjectRemove(fl)  CAL_CMGETAPI( "UserMgrObjectRemove" ) 
	#define CAL_UserMgrObjectRemove pICmpUserMgr->IUserMgrObjectRemove
	#define CHK_UserMgrObjectRemove (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectRemove  CAL_CMEXPAPI( "UserMgrObjectRemove" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectRemove  PFUSERMGROBJECTREMOVE pfUserMgrObjectRemove;
	#define EXT_UserMgrObjectRemove  extern PFUSERMGROBJECTREMOVE pfUserMgrObjectRemove;
	#define GET_UserMgrObjectRemove(fl)  s_pfCMGetAPI2( "UserMgrObjectRemove", (RTS_VOID_FCTPTR *)&pfUserMgrObjectRemove, (fl), 0, 0)
	#define CAL_UserMgrObjectRemove  pfUserMgrObjectRemove
	#define CHK_UserMgrObjectRemove  (pfUserMgrObjectRemove != NULL)
	#define EXP_UserMgrObjectRemove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectRemove", (RTS_UINTPTR)UserMgrObjectRemove, 0, 0) 
#endif




/**
 * <description>
 *	Add a new object under a father object specified by its name to the user management, so the access rights can be configured on that object.
 * </description>
 * <param name="hFatherObject" type="IN">Handle to the father object, under which the object will be added</param>
 * <param name="pszObject" type="IN">Object name (see object tree)</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Object could be added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Object invalid = NULL or empty</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">If memory could not be allocated to store this object</errorcode>
 * <result>Handle to the object or RTS_INVALID_HANDLE if it could not be added</result>
 */
RTS_HANDLE CDECL UserMgrObjectAddChild(RTS_HANDLE hFatherObject, char *pszObject, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTADDCHILD) (RTS_HANDLE hFatherObject, char *pszObject, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTADDCHILD_NOTIMPLEMENTED)
	#define USE_UserMgrObjectAddChild
	#define EXT_UserMgrObjectAddChild
	#define GET_UserMgrObjectAddChild(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectAddChild(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectAddChild  FALSE
	#define EXP_UserMgrObjectAddChild  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectAddChild
	#define EXT_UserMgrObjectAddChild
	#define GET_UserMgrObjectAddChild(fl)  CAL_CMGETAPI( "UserMgrObjectAddChild" ) 
	#define CAL_UserMgrObjectAddChild  UserMgrObjectAddChild
	#define CHK_UserMgrObjectAddChild  TRUE
	#define EXP_UserMgrObjectAddChild  CAL_CMEXPAPI( "UserMgrObjectAddChild" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectAddChild
	#define EXT_UserMgrObjectAddChild
	#define GET_UserMgrObjectAddChild(fl)  CAL_CMGETAPI( "UserMgrObjectAddChild" ) 
	#define CAL_UserMgrObjectAddChild  UserMgrObjectAddChild
	#define CHK_UserMgrObjectAddChild  TRUE
	#define EXP_UserMgrObjectAddChild  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAddChild", (RTS_UINTPTR)UserMgrObjectAddChild, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectAddChild
	#define EXT_CmpUserMgrUserMgrObjectAddChild
	#define GET_CmpUserMgrUserMgrObjectAddChild  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectAddChild pICmpUserMgr->IUserMgrObjectAddChild
	#define CHK_CmpUserMgrUserMgrObjectAddChild (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectAddChild  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectAddChild
	#define EXT_UserMgrObjectAddChild
	#define GET_UserMgrObjectAddChild(fl)  CAL_CMGETAPI( "UserMgrObjectAddChild" ) 
	#define CAL_UserMgrObjectAddChild pICmpUserMgr->IUserMgrObjectAddChild
	#define CHK_UserMgrObjectAddChild (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectAddChild  CAL_CMEXPAPI( "UserMgrObjectAddChild" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectAddChild  PFUSERMGROBJECTADDCHILD pfUserMgrObjectAddChild;
	#define EXT_UserMgrObjectAddChild  extern PFUSERMGROBJECTADDCHILD pfUserMgrObjectAddChild;
	#define GET_UserMgrObjectAddChild(fl)  s_pfCMGetAPI2( "UserMgrObjectAddChild", (RTS_VOID_FCTPTR *)&pfUserMgrObjectAddChild, (fl), 0, 0)
	#define CAL_UserMgrObjectAddChild  pfUserMgrObjectAddChild
	#define CHK_UserMgrObjectAddChild  (pfUserMgrObjectAddChild != NULL)
	#define EXP_UserMgrObjectAddChild  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAddChild", (RTS_UINTPTR)UserMgrObjectAddChild, 0, 0) 
#endif




/**
 * <description>
 *	Set the used access rights for the object specified by handle.
 *	The rights are additionally stored in an internal list. In case USERDB_RIGHT_NONE is passed for ulUsedRights the previously stored rights are retrieved for this object.
 * </description>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="ulUsedRights" type="IN">Used access rights or USERDB_RIGHT_NONE for retrieval</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Rights are successfully set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid object handle</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">Object was not found for rights retrieval</errorcode>
 * <result>Error code</result>
 */
RTS_RESULT CDECL UserMgrObjectSetUsedRights(RTS_HANDLE hObject, RTS_UI32 ulUsedRights);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTSETUSEDRIGHTS) (RTS_HANDLE hObject, RTS_UI32 ulUsedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTSETUSEDRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrObjectSetUsedRights
	#define EXT_UserMgrObjectSetUsedRights
	#define GET_UserMgrObjectSetUsedRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectSetUsedRights(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectSetUsedRights  FALSE
	#define EXP_UserMgrObjectSetUsedRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectSetUsedRights
	#define EXT_UserMgrObjectSetUsedRights
	#define GET_UserMgrObjectSetUsedRights(fl)  CAL_CMGETAPI( "UserMgrObjectSetUsedRights" ) 
	#define CAL_UserMgrObjectSetUsedRights  UserMgrObjectSetUsedRights
	#define CHK_UserMgrObjectSetUsedRights  TRUE
	#define EXP_UserMgrObjectSetUsedRights  CAL_CMEXPAPI( "UserMgrObjectSetUsedRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectSetUsedRights
	#define EXT_UserMgrObjectSetUsedRights
	#define GET_UserMgrObjectSetUsedRights(fl)  CAL_CMGETAPI( "UserMgrObjectSetUsedRights" ) 
	#define CAL_UserMgrObjectSetUsedRights  UserMgrObjectSetUsedRights
	#define CHK_UserMgrObjectSetUsedRights  TRUE
	#define EXP_UserMgrObjectSetUsedRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetUsedRights", (RTS_UINTPTR)UserMgrObjectSetUsedRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectSetUsedRights
	#define EXT_CmpUserMgrUserMgrObjectSetUsedRights
	#define GET_CmpUserMgrUserMgrObjectSetUsedRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectSetUsedRights pICmpUserMgr->IUserMgrObjectSetUsedRights
	#define CHK_CmpUserMgrUserMgrObjectSetUsedRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectSetUsedRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectSetUsedRights
	#define EXT_UserMgrObjectSetUsedRights
	#define GET_UserMgrObjectSetUsedRights(fl)  CAL_CMGETAPI( "UserMgrObjectSetUsedRights" ) 
	#define CAL_UserMgrObjectSetUsedRights pICmpUserMgr->IUserMgrObjectSetUsedRights
	#define CHK_UserMgrObjectSetUsedRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectSetUsedRights  CAL_CMEXPAPI( "UserMgrObjectSetUsedRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectSetUsedRights  PFUSERMGROBJECTSETUSEDRIGHTS pfUserMgrObjectSetUsedRights;
	#define EXT_UserMgrObjectSetUsedRights  extern PFUSERMGROBJECTSETUSEDRIGHTS pfUserMgrObjectSetUsedRights;
	#define GET_UserMgrObjectSetUsedRights(fl)  s_pfCMGetAPI2( "UserMgrObjectSetUsedRights", (RTS_VOID_FCTPTR *)&pfUserMgrObjectSetUsedRights, (fl), 0, 0)
	#define CAL_UserMgrObjectSetUsedRights  pfUserMgrObjectSetUsedRights
	#define CHK_UserMgrObjectSetUsedRights  (pfUserMgrObjectSetUsedRights != NULL)
	#define EXP_UserMgrObjectSetUsedRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetUsedRights", (RTS_UINTPTR)UserMgrObjectSetUsedRights, 0, 0) 
#endif




/**
 * <description>
 *	Get the used access rights for the object specified by handle.
 *	NOTE: In case the used rights are not set the function returns USERDB_RIGHT_ALL.
 * </description>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="pulUsedRights" type="OUT">Pointer to used access rights</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Rights are successfully set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid object handle or invalid pointer</errorcode>
 * <result>Error code</result>
 */
RTS_RESULT CDECL UserMgrObjectGetUsedRights(RTS_HANDLE hObject, RTS_UI32 *pulUsedRights);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTGETUSEDRIGHTS) (RTS_HANDLE hObject, RTS_UI32 *pulUsedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETUSEDRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetUsedRights
	#define EXT_UserMgrObjectGetUsedRights
	#define GET_UserMgrObjectGetUsedRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetUsedRights(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectGetUsedRights  FALSE
	#define EXP_UserMgrObjectGetUsedRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetUsedRights
	#define EXT_UserMgrObjectGetUsedRights
	#define GET_UserMgrObjectGetUsedRights(fl)  CAL_CMGETAPI( "UserMgrObjectGetUsedRights" ) 
	#define CAL_UserMgrObjectGetUsedRights  UserMgrObjectGetUsedRights
	#define CHK_UserMgrObjectGetUsedRights  TRUE
	#define EXP_UserMgrObjectGetUsedRights  CAL_CMEXPAPI( "UserMgrObjectGetUsedRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetUsedRights
	#define EXT_UserMgrObjectGetUsedRights
	#define GET_UserMgrObjectGetUsedRights(fl)  CAL_CMGETAPI( "UserMgrObjectGetUsedRights" ) 
	#define CAL_UserMgrObjectGetUsedRights  UserMgrObjectGetUsedRights
	#define CHK_UserMgrObjectGetUsedRights  TRUE
	#define EXP_UserMgrObjectGetUsedRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetUsedRights", (RTS_UINTPTR)UserMgrObjectGetUsedRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetUsedRights
	#define EXT_CmpUserMgrUserMgrObjectGetUsedRights
	#define GET_CmpUserMgrUserMgrObjectGetUsedRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetUsedRights pICmpUserMgr->IUserMgrObjectGetUsedRights
	#define CHK_CmpUserMgrUserMgrObjectGetUsedRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetUsedRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetUsedRights
	#define EXT_UserMgrObjectGetUsedRights
	#define GET_UserMgrObjectGetUsedRights(fl)  CAL_CMGETAPI( "UserMgrObjectGetUsedRights" ) 
	#define CAL_UserMgrObjectGetUsedRights pICmpUserMgr->IUserMgrObjectGetUsedRights
	#define CHK_UserMgrObjectGetUsedRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetUsedRights  CAL_CMEXPAPI( "UserMgrObjectGetUsedRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetUsedRights  PFUSERMGROBJECTGETUSEDRIGHTS pfUserMgrObjectGetUsedRights;
	#define EXT_UserMgrObjectGetUsedRights  extern PFUSERMGROBJECTGETUSEDRIGHTS pfUserMgrObjectGetUsedRights;
	#define GET_UserMgrObjectGetUsedRights(fl)  s_pfCMGetAPI2( "UserMgrObjectGetUsedRights", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetUsedRights, (fl), 0, 0)
	#define CAL_UserMgrObjectGetUsedRights  pfUserMgrObjectGetUsedRights
	#define CHK_UserMgrObjectGetUsedRights  (pfUserMgrObjectGetUsedRights != NULL)
	#define EXP_UserMgrObjectGetUsedRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetUsedRights", (RTS_UINTPTR)UserMgrObjectGetUsedRights, 0, 0) 
#endif




/**
 * <description>
 *	Open an existing object.
 *	NOTE:
 *	The name of the object must include the full namespace with "Device" as the root node, e.g. "Device.MyObject" or if it's a filesystem object with "/" as the root node.
 * </description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Object could be opened</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Object not available of invalid</errorcode>
 * <result>Handle to the object or RTS_INVALID_HANDLE if it is not available</result>
 */
RTS_HANDLE CDECL UserMgrObjectOpen(char *pszObject, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTOPEN) (char *pszObject, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTOPEN_NOTIMPLEMENTED)
	#define USE_UserMgrObjectOpen
	#define EXT_UserMgrObjectOpen
	#define GET_UserMgrObjectOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectOpen(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectOpen  FALSE
	#define EXP_UserMgrObjectOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectOpen
	#define EXT_UserMgrObjectOpen
	#define GET_UserMgrObjectOpen(fl)  CAL_CMGETAPI( "UserMgrObjectOpen" ) 
	#define CAL_UserMgrObjectOpen  UserMgrObjectOpen
	#define CHK_UserMgrObjectOpen  TRUE
	#define EXP_UserMgrObjectOpen  CAL_CMEXPAPI( "UserMgrObjectOpen" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectOpen
	#define EXT_UserMgrObjectOpen
	#define GET_UserMgrObjectOpen(fl)  CAL_CMGETAPI( "UserMgrObjectOpen" ) 
	#define CAL_UserMgrObjectOpen  UserMgrObjectOpen
	#define CHK_UserMgrObjectOpen  TRUE
	#define EXP_UserMgrObjectOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectOpen", (RTS_UINTPTR)UserMgrObjectOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectOpen
	#define EXT_CmpUserMgrUserMgrObjectOpen
	#define GET_CmpUserMgrUserMgrObjectOpen  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectOpen pICmpUserMgr->IUserMgrObjectOpen
	#define CHK_CmpUserMgrUserMgrObjectOpen (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectOpen
	#define EXT_UserMgrObjectOpen
	#define GET_UserMgrObjectOpen(fl)  CAL_CMGETAPI( "UserMgrObjectOpen" ) 
	#define CAL_UserMgrObjectOpen pICmpUserMgr->IUserMgrObjectOpen
	#define CHK_UserMgrObjectOpen (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectOpen  CAL_CMEXPAPI( "UserMgrObjectOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectOpen  PFUSERMGROBJECTOPEN pfUserMgrObjectOpen;
	#define EXT_UserMgrObjectOpen  extern PFUSERMGROBJECTOPEN pfUserMgrObjectOpen;
	#define GET_UserMgrObjectOpen(fl)  s_pfCMGetAPI2( "UserMgrObjectOpen", (RTS_VOID_FCTPTR *)&pfUserMgrObjectOpen, (fl), 0, 0)
	#define CAL_UserMgrObjectOpen  pfUserMgrObjectOpen
	#define CHK_UserMgrObjectOpen  (pfUserMgrObjectOpen != NULL)
	#define EXP_UserMgrObjectOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectOpen", (RTS_UINTPTR)UserMgrObjectOpen, 0, 0) 
#endif




/**
 * <description>Opens an existing object. Checks if the user is member of at least one group that has access to this object.</description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="hUser" type="IN">Handle of the logged in user.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Object could be opened. User has access rights to the object.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Object not available of invalid.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User has no access rights to this object.</errorcode>
 * <result>Handle to the object or RTS_INVALID_HANDLE if it is not available or no access rights are existing.</result>
 */
RTS_HANDLE CDECL UserMgrObjectOpen2(char *pszObject, RTS_HANDLE hUser, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTOPEN2) (char *pszObject, RTS_HANDLE hUser, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTOPEN2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectOpen2
	#define EXT_UserMgrObjectOpen2
	#define GET_UserMgrObjectOpen2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectOpen2(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectOpen2  FALSE
	#define EXP_UserMgrObjectOpen2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectOpen2
	#define EXT_UserMgrObjectOpen2
	#define GET_UserMgrObjectOpen2(fl)  CAL_CMGETAPI( "UserMgrObjectOpen2" ) 
	#define CAL_UserMgrObjectOpen2  UserMgrObjectOpen2
	#define CHK_UserMgrObjectOpen2  TRUE
	#define EXP_UserMgrObjectOpen2  CAL_CMEXPAPI( "UserMgrObjectOpen2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectOpen2
	#define EXT_UserMgrObjectOpen2
	#define GET_UserMgrObjectOpen2(fl)  CAL_CMGETAPI( "UserMgrObjectOpen2" ) 
	#define CAL_UserMgrObjectOpen2  UserMgrObjectOpen2
	#define CHK_UserMgrObjectOpen2  TRUE
	#define EXP_UserMgrObjectOpen2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectOpen2", (RTS_UINTPTR)UserMgrObjectOpen2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectOpen2
	#define EXT_CmpUserMgrUserMgrObjectOpen2
	#define GET_CmpUserMgrUserMgrObjectOpen2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectOpen2 pICmpUserMgr->IUserMgrObjectOpen2
	#define CHK_CmpUserMgrUserMgrObjectOpen2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectOpen2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectOpen2
	#define EXT_UserMgrObjectOpen2
	#define GET_UserMgrObjectOpen2(fl)  CAL_CMGETAPI( "UserMgrObjectOpen2" ) 
	#define CAL_UserMgrObjectOpen2 pICmpUserMgr->IUserMgrObjectOpen2
	#define CHK_UserMgrObjectOpen2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectOpen2  CAL_CMEXPAPI( "UserMgrObjectOpen2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectOpen2  PFUSERMGROBJECTOPEN2 pfUserMgrObjectOpen2;
	#define EXT_UserMgrObjectOpen2  extern PFUSERMGROBJECTOPEN2 pfUserMgrObjectOpen2;
	#define GET_UserMgrObjectOpen2(fl)  s_pfCMGetAPI2( "UserMgrObjectOpen2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectOpen2, (fl), 0, 0)
	#define CAL_UserMgrObjectOpen2  pfUserMgrObjectOpen2
	#define CHK_UserMgrObjectOpen2  (pfUserMgrObjectOpen2 != NULL)
	#define EXP_UserMgrObjectOpen2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectOpen2", (RTS_UINTPTR)UserMgrObjectOpen2, 0, 0) 
#endif




/**
 * <description>Close an object</description>
 * <param name="hObject" type="IN">Handle to object</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Object could be closed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid object handle</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectClose(RTS_HANDLE hObject);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTCLOSE) (RTS_HANDLE hObject);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTCLOSE_NOTIMPLEMENTED)
	#define USE_UserMgrObjectClose
	#define EXT_UserMgrObjectClose
	#define GET_UserMgrObjectClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectClose  FALSE
	#define EXP_UserMgrObjectClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectClose
	#define EXT_UserMgrObjectClose
	#define GET_UserMgrObjectClose(fl)  CAL_CMGETAPI( "UserMgrObjectClose" ) 
	#define CAL_UserMgrObjectClose  UserMgrObjectClose
	#define CHK_UserMgrObjectClose  TRUE
	#define EXP_UserMgrObjectClose  CAL_CMEXPAPI( "UserMgrObjectClose" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectClose
	#define EXT_UserMgrObjectClose
	#define GET_UserMgrObjectClose(fl)  CAL_CMGETAPI( "UserMgrObjectClose" ) 
	#define CAL_UserMgrObjectClose  UserMgrObjectClose
	#define CHK_UserMgrObjectClose  TRUE
	#define EXP_UserMgrObjectClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectClose", (RTS_UINTPTR)UserMgrObjectClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectClose
	#define EXT_CmpUserMgrUserMgrObjectClose
	#define GET_CmpUserMgrUserMgrObjectClose  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectClose pICmpUserMgr->IUserMgrObjectClose
	#define CHK_CmpUserMgrUserMgrObjectClose (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectClose
	#define EXT_UserMgrObjectClose
	#define GET_UserMgrObjectClose(fl)  CAL_CMGETAPI( "UserMgrObjectClose" ) 
	#define CAL_UserMgrObjectClose pICmpUserMgr->IUserMgrObjectClose
	#define CHK_UserMgrObjectClose (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectClose  CAL_CMEXPAPI( "UserMgrObjectClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectClose  PFUSERMGROBJECTCLOSE pfUserMgrObjectClose;
	#define EXT_UserMgrObjectClose  extern PFUSERMGROBJECTCLOSE pfUserMgrObjectClose;
	#define GET_UserMgrObjectClose(fl)  s_pfCMGetAPI2( "UserMgrObjectClose", (RTS_VOID_FCTPTR *)&pfUserMgrObjectClose, (fl), 0, 0)
	#define CAL_UserMgrObjectClose  pfUserMgrObjectClose
	#define CHK_UserMgrObjectClose  (pfUserMgrObjectClose != NULL)
	#define EXP_UserMgrObjectClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectClose", (RTS_UINTPTR)UserMgrObjectClose, 0, 0) 
#endif




/**
 * <description>Get a new challenge value to encrypt the password</description>
 * <param name="pulChallenge" type="OUT">Pointer to challenge to get the value</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Challenge could be generated</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter: pulChallenge = NULL</errorcode>
 */
RTS_RESULT CDECL UserMgrGetChallenge(RTS_UI32 *pulChallenge);
typedef RTS_RESULT (CDECL * PFUSERMGRGETCHALLENGE) (RTS_UI32 *pulChallenge);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGETCHALLENGE_NOTIMPLEMENTED)
	#define USE_UserMgrGetChallenge
	#define EXT_UserMgrGetChallenge
	#define GET_UserMgrGetChallenge(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGetChallenge(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGetChallenge  FALSE
	#define EXP_UserMgrGetChallenge  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGetChallenge
	#define EXT_UserMgrGetChallenge
	#define GET_UserMgrGetChallenge(fl)  CAL_CMGETAPI( "UserMgrGetChallenge" ) 
	#define CAL_UserMgrGetChallenge  UserMgrGetChallenge
	#define CHK_UserMgrGetChallenge  TRUE
	#define EXP_UserMgrGetChallenge  CAL_CMEXPAPI( "UserMgrGetChallenge" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGetChallenge
	#define EXT_UserMgrGetChallenge
	#define GET_UserMgrGetChallenge(fl)  CAL_CMGETAPI( "UserMgrGetChallenge" ) 
	#define CAL_UserMgrGetChallenge  UserMgrGetChallenge
	#define CHK_UserMgrGetChallenge  TRUE
	#define EXP_UserMgrGetChallenge  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetChallenge", (RTS_UINTPTR)UserMgrGetChallenge, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGetChallenge
	#define EXT_CmpUserMgrUserMgrGetChallenge
	#define GET_CmpUserMgrUserMgrGetChallenge  ERR_OK
	#define CAL_CmpUserMgrUserMgrGetChallenge pICmpUserMgr->IUserMgrGetChallenge
	#define CHK_CmpUserMgrUserMgrGetChallenge (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGetChallenge  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGetChallenge
	#define EXT_UserMgrGetChallenge
	#define GET_UserMgrGetChallenge(fl)  CAL_CMGETAPI( "UserMgrGetChallenge" ) 
	#define CAL_UserMgrGetChallenge pICmpUserMgr->IUserMgrGetChallenge
	#define CHK_UserMgrGetChallenge (pICmpUserMgr != NULL)
	#define EXP_UserMgrGetChallenge  CAL_CMEXPAPI( "UserMgrGetChallenge" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGetChallenge  PFUSERMGRGETCHALLENGE pfUserMgrGetChallenge;
	#define EXT_UserMgrGetChallenge  extern PFUSERMGRGETCHALLENGE pfUserMgrGetChallenge;
	#define GET_UserMgrGetChallenge(fl)  s_pfCMGetAPI2( "UserMgrGetChallenge", (RTS_VOID_FCTPTR *)&pfUserMgrGetChallenge, (fl), 0, 0)
	#define CAL_UserMgrGetChallenge  pfUserMgrGetChallenge
	#define CHK_UserMgrGetChallenge  (pfUserMgrGetChallenge != NULL)
	#define EXP_UserMgrGetChallenge  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetChallenge", (RTS_UINTPTR)UserMgrGetChallenge, 0, 0) 
#endif




/**
 * <description>Encrypt the password specified by crypt type</description>
 * <param name="pszPassword" type="IN">Cleartext password</param>
 * <param name="pszPasswordCrypted" type="OUT">Pointer to get the encrypted password</param>
 * <param name="pnPasswordCryptedSize" type="INOUT">Pointer to transmit the buffer length of pszPasswordCrypted or to return the necessary encrypted password length if result code is ERR_BUFFERSIZE</param>
 * <param name="ulCryptType" type="IN">Crypt type. See category "Crypt types" for details</param>
 * <param name="ulChallenge" type="IN">Challenge value to encrypt the password</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Password could be encrypted</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">At least one parameter is invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOT_SUPPORTED">Unsupported encryption type</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">Buffer for pszPasswordCrypted is too short, so the requested length is returned in *pnPasswordCryptedSize</errorcode> 
 */
RTS_RESULT CDECL UserMgrCryptPassword(char *pszPassword, char *pszPasswordCrypted, int *pnPasswordCryptedSize, RTS_UI32 ulCryptType, RTS_UI32 ulChallenge);
typedef RTS_RESULT (CDECL * PFUSERMGRCRYPTPASSWORD) (char *pszPassword, char *pszPasswordCrypted, int *pnPasswordCryptedSize, RTS_UI32 ulCryptType, RTS_UI32 ulChallenge);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRCRYPTPASSWORD_NOTIMPLEMENTED)
	#define USE_UserMgrCryptPassword
	#define EXT_UserMgrCryptPassword
	#define GET_UserMgrCryptPassword(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrCryptPassword(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrCryptPassword  FALSE
	#define EXP_UserMgrCryptPassword  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrCryptPassword
	#define EXT_UserMgrCryptPassword
	#define GET_UserMgrCryptPassword(fl)  CAL_CMGETAPI( "UserMgrCryptPassword" ) 
	#define CAL_UserMgrCryptPassword  UserMgrCryptPassword
	#define CHK_UserMgrCryptPassword  TRUE
	#define EXP_UserMgrCryptPassword  CAL_CMEXPAPI( "UserMgrCryptPassword" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrCryptPassword
	#define EXT_UserMgrCryptPassword
	#define GET_UserMgrCryptPassword(fl)  CAL_CMGETAPI( "UserMgrCryptPassword" ) 
	#define CAL_UserMgrCryptPassword  UserMgrCryptPassword
	#define CHK_UserMgrCryptPassword  TRUE
	#define EXP_UserMgrCryptPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrCryptPassword", (RTS_UINTPTR)UserMgrCryptPassword, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrCryptPassword
	#define EXT_CmpUserMgrUserMgrCryptPassword
	#define GET_CmpUserMgrUserMgrCryptPassword  ERR_OK
	#define CAL_CmpUserMgrUserMgrCryptPassword pICmpUserMgr->IUserMgrCryptPassword
	#define CHK_CmpUserMgrUserMgrCryptPassword (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrCryptPassword  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrCryptPassword
	#define EXT_UserMgrCryptPassword
	#define GET_UserMgrCryptPassword(fl)  CAL_CMGETAPI( "UserMgrCryptPassword" ) 
	#define CAL_UserMgrCryptPassword pICmpUserMgr->IUserMgrCryptPassword
	#define CHK_UserMgrCryptPassword (pICmpUserMgr != NULL)
	#define EXP_UserMgrCryptPassword  CAL_CMEXPAPI( "UserMgrCryptPassword" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrCryptPassword  PFUSERMGRCRYPTPASSWORD pfUserMgrCryptPassword;
	#define EXT_UserMgrCryptPassword  extern PFUSERMGRCRYPTPASSWORD pfUserMgrCryptPassword;
	#define GET_UserMgrCryptPassword(fl)  s_pfCMGetAPI2( "UserMgrCryptPassword", (RTS_VOID_FCTPTR *)&pfUserMgrCryptPassword, (fl), 0, 0)
	#define CAL_UserMgrCryptPassword  pfUserMgrCryptPassword
	#define CHK_UserMgrCryptPassword  (pfUserMgrCryptPassword != NULL)
	#define EXP_UserMgrCryptPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrCryptPassword", (RTS_UINTPTR)UserMgrCryptPassword, 0, 0) 
#endif




/**
 * <description>Decrypt the password specified by crypt type</description>
 * <param name="pszPasswordCrypted" type="IN">Encrypted password</param>
 * <param name="nPasswordCryptedSize" type="IN">Lenght of the encrypted password</param>
 * <param name="ulCryptType" type="IN">Crypt type. See category "Crypt types" for details</param>
 * <param name="ulChallenge" type="IN">Challenge value to encrypt the password</param>
 * <param name="pszPassword" type="IN">Cleartext password</param>
 * <param name="pnMaxLenPassword" type="INOUT">Pointer to transmit the buffer length of pszPassword or to return the necessary decrypted password length if result code is ERR_BUFFERSIZE</param>
  * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Password could be decrypted</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">At least one parameter is invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOT_SUPPORTED">Unsupported decryption type</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">Buffer for pszPassword is too short, so the requested length is returned in *pnMaxLenPassword</errorcode> 
 */
RTS_RESULT CDECL UserMgrDecryptPassword(char *pszPasswordCrypted, int nPasswordCryptedSize, RTS_UI32 ulCryptType, RTS_UI32 ulChallenge, char *pszPassword, int *pnMaxLenPassword);
typedef RTS_RESULT (CDECL * PFUSERMGRDECRYPTPASSWORD) (char *pszPasswordCrypted, int nPasswordCryptedSize, RTS_UI32 ulCryptType, RTS_UI32 ulChallenge, char *pszPassword, int *pnMaxLenPassword);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRDECRYPTPASSWORD_NOTIMPLEMENTED)
	#define USE_UserMgrDecryptPassword
	#define EXT_UserMgrDecryptPassword
	#define GET_UserMgrDecryptPassword(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrDecryptPassword(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrDecryptPassword  FALSE
	#define EXP_UserMgrDecryptPassword  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrDecryptPassword
	#define EXT_UserMgrDecryptPassword
	#define GET_UserMgrDecryptPassword(fl)  CAL_CMGETAPI( "UserMgrDecryptPassword" ) 
	#define CAL_UserMgrDecryptPassword  UserMgrDecryptPassword
	#define CHK_UserMgrDecryptPassword  TRUE
	#define EXP_UserMgrDecryptPassword  CAL_CMEXPAPI( "UserMgrDecryptPassword" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrDecryptPassword
	#define EXT_UserMgrDecryptPassword
	#define GET_UserMgrDecryptPassword(fl)  CAL_CMGETAPI( "UserMgrDecryptPassword" ) 
	#define CAL_UserMgrDecryptPassword  UserMgrDecryptPassword
	#define CHK_UserMgrDecryptPassword  TRUE
	#define EXP_UserMgrDecryptPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrDecryptPassword", (RTS_UINTPTR)UserMgrDecryptPassword, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrDecryptPassword
	#define EXT_CmpUserMgrUserMgrDecryptPassword
	#define GET_CmpUserMgrUserMgrDecryptPassword  ERR_OK
	#define CAL_CmpUserMgrUserMgrDecryptPassword pICmpUserMgr->IUserMgrDecryptPassword
	#define CHK_CmpUserMgrUserMgrDecryptPassword (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrDecryptPassword  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrDecryptPassword
	#define EXT_UserMgrDecryptPassword
	#define GET_UserMgrDecryptPassword(fl)  CAL_CMGETAPI( "UserMgrDecryptPassword" ) 
	#define CAL_UserMgrDecryptPassword pICmpUserMgr->IUserMgrDecryptPassword
	#define CHK_UserMgrDecryptPassword (pICmpUserMgr != NULL)
	#define EXP_UserMgrDecryptPassword  CAL_CMEXPAPI( "UserMgrDecryptPassword" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrDecryptPassword  PFUSERMGRDECRYPTPASSWORD pfUserMgrDecryptPassword;
	#define EXT_UserMgrDecryptPassword  extern PFUSERMGRDECRYPTPASSWORD pfUserMgrDecryptPassword;
	#define GET_UserMgrDecryptPassword(fl)  s_pfCMGetAPI2( "UserMgrDecryptPassword", (RTS_VOID_FCTPTR *)&pfUserMgrDecryptPassword, (fl), 0, 0)
	#define CAL_UserMgrDecryptPassword  pfUserMgrDecryptPassword
	#define CHK_UserMgrDecryptPassword  (pfUserMgrDecryptPassword != NULL)
	#define EXP_UserMgrDecryptPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrDecryptPassword", (RTS_UINTPTR)UserMgrDecryptPassword, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFUSERMGRLOGIN IUserMgrLogin;
 	PFUSERMGRLOGOUT IUserMgrLogout;
 	PFUSERMGRLOGOUTBYSESSIONID IUserMgrLogoutBySessionId;
 	PFUSERMGRCHECKPASSWORD IUserMgrCheckPassword;
 	PFUSERMGRGETPROPERTY IUserMgrGetProperty;
 	PFUSERMGRISGROUPMEMBER IUserMgrIsGroupMember;
 	PFUSERMGRGETSTATE IUserMgrGetState;
 	PFUSERMGRHASACCESSRIGHTS IUserMgrHasAccessRights;
 	PFUSERMGRGETACCESSRIGHTS IUserMgrGetAccessRights;
 	PFUSERMGRHASUSERACCESSRIGHTS IUserMgrHasUserAccessRights;
 	PFUSERMGRGETUSERACCESSRIGHTS IUserMgrGetUserAccessRights;
 	PFUSERMGRADDSESSIONID IUserMgrAddSessionId;
 	PFUSERMGRREMOVESESSIONID IUserMgrRemoveSessionId;
 	PFUSERMGRFINDUSERBYSESSIONID IUserMgrFindUserBySessionId;
 	PFUSERMGRADDINFOTOUSER IUserMgrAddInfoToUser;
 	PFUSERMGRREMOVEINFOFROMUSER IUserMgrRemoveInfoFromUser;
 	PFUSERMGRREMOVEINFOFROMALLUSERS IUserMgrRemoveInfoFromAllUsers;
 	PFUSERMGRGETINFOOFUSER IUserMgrGetInfoOfUser;
 	PFUSERMGRADDONLINEACCESSERROR IUserMgrAddOnlineAccessError;
 	PFUSERMGROBJECTADD IUserMgrObjectAdd;
 	PFUSERMGROBJECTREMOVE IUserMgrObjectRemove;
 	PFUSERMGROBJECTADDCHILD IUserMgrObjectAddChild;
 	PFUSERMGROBJECTSETUSEDRIGHTS IUserMgrObjectSetUsedRights;
 	PFUSERMGROBJECTGETUSEDRIGHTS IUserMgrObjectGetUsedRights;
 	PFUSERMGROBJECTOPEN IUserMgrObjectOpen;
 	PFUSERMGROBJECTOPEN2 IUserMgrObjectOpen2;
 	PFUSERMGROBJECTCLOSE IUserMgrObjectClose;
 	PFUSERMGRGETCHALLENGE IUserMgrGetChallenge;
 	PFUSERMGRCRYPTPASSWORD IUserMgrCryptPassword;
 	PFUSERMGRDECRYPTPASSWORD IUserMgrDecryptPassword;
 } ICmpUserMgr_C;

#ifdef CPLUSPLUS
class ICmpUserMgr : public IBase
{
	public:
		virtual RTS_HANDLE CDECL IUserMgrLogin(char *pszUser, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrLogout(RTS_HANDLE hUser) =0;
		virtual RTS_RESULT CDECL IUserMgrLogoutBySessionId(RTS_UI32 ulSessionId) =0;
		virtual RTS_RESULT CDECL IUserMgrCheckPassword(RTS_HANDLE hUser, char *pszPassword) =0;
		virtual RTS_RESULT CDECL IUserMgrGetProperty(RTS_HANDLE hUser, RTS_UI32 *pulProperty) =0;
		virtual RTS_RESULT CDECL IUserMgrIsGroupMember(char *pszGroup, RTS_HANDLE hUser) =0;
		virtual RTS_UI32 CDECL IUserMgrGetState(RTS_HANDLE hUser, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrHasAccessRights(char *pszObject, RTS_UI32 ulSessionId, RTS_UI32 ulRequestedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrGetAccessRights(char *pszObject, RTS_UI32 ulSessionId, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrHasUserAccessRights(char *pszObject, RTS_HANDLE hUser, RTS_UI32 ulRequestedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrGetUserAccessRights(char *pszObject, RTS_HANDLE hUser, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrAddSessionId(RTS_HANDLE hUser, RTS_UI32 *pulSessionId) =0;
		virtual RTS_RESULT CDECL IUserMgrRemoveSessionId(RTS_UI32 ulSessionId) =0;
		virtual RTS_HANDLE CDECL IUserMgrFindUserBySessionId(RTS_UI32 ulSessionId, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrAddInfoToUser(RTS_HANDLE hUser, RTS_SIZE key, RTS_TYPEDVALUE *pValue) =0;
		virtual RTS_RESULT CDECL IUserMgrRemoveInfoFromUser(RTS_HANDLE hUser, RTS_SIZE key) =0;
		virtual RTS_RESULT CDECL IUserMgrRemoveInfoFromAllUsers(RTS_SIZE key) =0;
		virtual RTS_RESULT CDECL IUserMgrGetInfoOfUser(RTS_HANDLE hUser, RTS_SIZE key, RTS_TYPEDVALUE *pValue) =0;
		virtual RTS_RESULT CDECL IUserMgrAddOnlineAccessError(BINTAGWRITER *pWriter, RTS_RESULT Result, char *pszObject, RTS_UI32 ulRequestedRights, RTS_UI32 ulSessionID) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectAdd(char *pszObject, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectRemove(RTS_HANDLE hObject) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectAddChild(RTS_HANDLE hFatherObject, char *pszObject, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectSetUsedRights(RTS_HANDLE hObject, RTS_UI32 ulUsedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectGetUsedRights(RTS_HANDLE hObject, RTS_UI32 *pulUsedRights) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectOpen(char *pszObject, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectOpen2(char *pszObject, RTS_HANDLE hUser, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectClose(RTS_HANDLE hObject) =0;
		virtual RTS_RESULT CDECL IUserMgrGetChallenge(RTS_UI32 *pulChallenge) =0;
		virtual RTS_RESULT CDECL IUserMgrCryptPassword(char *pszPassword, char *pszPasswordCrypted, int *pnPasswordCryptedSize, RTS_UI32 ulCryptType, RTS_UI32 ulChallenge) =0;
		virtual RTS_RESULT CDECL IUserMgrDecryptPassword(char *pszPasswordCrypted, int nPasswordCryptedSize, RTS_UI32 ulCryptType, RTS_UI32 ulChallenge, char *pszPassword, int *pnMaxLenPassword) =0;
};
	#ifndef ITF_CmpUserMgr
		#define ITF_CmpUserMgr static ICmpUserMgr *pICmpUserMgr = NULL;
	#endif
	#define EXTITF_CmpUserMgr
#else	/*CPLUSPLUS*/
	typedef ICmpUserMgr_C		ICmpUserMgr;
	#ifndef ITF_CmpUserMgr
		#define ITF_CmpUserMgr
	#endif
	#define EXTITF_CmpUserMgr
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPUSERMGRITF_H_*/
