 /**
 * <interfacename>CmpUserDB</interfacename>
 * <description> 
 *	Interface for the user management database. Two different kinds of user management items must be stored:
 *	
 *	1. Users + Groups:
 *	   Here you have to store all users with their credentials and their property flags, additionally all user groups with all assigned users and the usergroup property flags.
 *
 *	   Example:
 *			- User: Name = "MyUser"; Password = [password in a safe hash]; Property = 0x....
 *			- UserGroup: Name = "Administrator"; UserMembers = "MyUser", "..."; Property = 0x....
 *
 *	2. User Rights:
 *	   Here you have to store the tree of all objects of the runtime system with their assigned usergroups and the configured rights on it.
 *
 *	   Example:
 *			- Object: Name = "Device"; UserGroup = "Administator" | Rights = USERDB_RIGHT_ALL | DeniedRights = USERDB_RIGHT_NONE; 
 *									   UserGroup = "..." | Rights = ... | DeniedRights = ...;
 *									   ...
 *			- Object: Name = "Device.MyObject"; UserGroup = "Administator" | Rights = USERDB_RIGHT_ALL | DeniedRights = USERDB_RIGHT_NONE; 
 *												UserGroup = "..." | Rights = ... | DeniedRights = ...;
 *												...
 *
 *	For details see CmpUserMgrItf.h.
 *
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved. GmbH
 * </copyright>
 */


	
	
#ifndef _CMPUSERDBITF_H_
#define _CMPUSERDBITF_H_

#include "CmpStd.h"

 

 




#include "CmpItf.h"
#include "CmpCryptMD5Itf.h"

/**
 * <category>Static defines</category>
 * <description>Number of users at startup</description>
 */
#ifndef USERDB_NUM_OF_STATIC_USERS
	#define USERDB_NUM_OF_STATIC_USERS		8
#endif

/**
 * <category>Static defines</category>
 * <description>Number of usergroups at startup</description>
 */
#ifndef USERDB_NUM_OF_STATIC_GROUPS
	#define USERDB_NUM_OF_STATIC_GROUPS		4
#endif

/**
 * <category>Static defines</category>
 * <description>Number of user and group handles at startup</description>
 */
#ifndef USERDB_NUM_OF_STATIC_HANDLES
	#define USERDB_NUM_OF_STATIC_HANDLES	12
#endif

/**
 * <category>UserDB properties</category>
 * <description>Properties for a user or usergroup</description>
 */
#define USERDB_PROP_NONE					UINT32_C(0x00000000)
#define USERDB_PROP_EDITABLE				UINT32_C(0x00000001)
#define USERDB_PROP_ADD_MEMBER				UINT32_C(0x00000002)
#define USERDB_PROP_REMOVE_MEMBER			UINT32_C(0x00000004)
#define USERDB_PROP_CREATE					UINT32_C(0x00000008)
#define USERDB_PROP_DELETE					UINT32_C(0x00000010)
#define USERDB_PROP_REMOVE_ALL_MEMBERS		UINT32_C(0x00000020)
#define USERDB_PROP_EDIT_RIGHTS				UINT32_C(0x00000040)
#define USERDB_PROP_OWNER					UINT32_C(0x00000080)
#define USERDB_PROP_NAME_EDITABLE			UINT32_C(0x00000100)
#define USERDB_PROP_PASSWORD_UPTODATE		UINT32_C(0x00001000)
#define USERDB_PROP_ENABLE_CHANGE_PASSWORD	UINT32_C(0x00002000)
#define USERDB_PROP_ALL					(UINT32_C(0x0FFFFFFF) & ~USERDB_PROP_OWNER)

#define USERDB_PROP_GROUP_ADMINISTRATOR_DEFAULT	(USERDB_PROP_EDITABLE | USERDB_PROP_ADD_MEMBER | USERDB_PROP_REMOVE_MEMBER | USERDB_PROP_CREATE | USERDB_PROP_OWNER)
#define USERDB_PROP_USER_ADMINISTRATOR_DEFAULT	(UINT32_C(0x0FFFFFFF) & ~USERDB_PROP_NAME_EDITABLE)
#define USERDB_PROP_GROUP_EVERYONE_DEFAULT		(USERDB_PROP_ADD_MEMBER | USERDB_PROP_REMOVE_MEMBER | USERDB_PROP_CREATE | USERDB_PROP_EDIT_RIGHTS )

/**
 * <category>Static defines</category>
 * <description>Passwort types</description>
 */
#define USERDB_PASSWORD_TYPE_STRING		UINT32_C(0x00000001)
#define USERDB_PASSWORD_TYPE_MD5		UINT32_C(0x00000002)

/**
 * <category>Union to store the cleartext password or a password hash optional</category>
 * <description>
 *	Password entry.
 * </description>
 *	<element name="psz" type="IN">Cleartext password</element>
 *	<element name="abyMD5" type="IN">Buffer to store the MD5 hash</element>
 */
typedef union
{
	char *psz;			
	unsigned char abyMD5[CRYPTMD5_KEY_LENGTH];
} UserDBPassword;

/**
 * <category>User database entry</category>
 * <description>
 *	User database entry to hold all informations of a user.
 * </description>
 *	<element name="pszUser" type="IN">User name</element>
 *	<element name="ulPasswordType" type="IN">Password type</element>
 *	<element name="Password" type="IN">Stored password credential</element>
 *	<element name="ulProperty" type="IN">User properties. See category "UserDB properties"</element>
 */
typedef struct
{
	char *pszUser;
	RTS_UI32 ulPasswordType;
	UserDBPassword Password;
	RTS_UI32 ulProperty;
} UserDBEntry;

/**
 * <category>User group database entry</category>
 * <description>
 *	User group database entry to hold all informations of a usergroup.
 * </description>
 *	<element name="pszGroup" type="IN">Group name</element>
 *	<element name="hUserList" type="IN">Handle to the mempool user list assigned to the group</element>
 *	<element name="hGroupList" type="IN">Handle to the mempool group list from which this group is derived</element>
 *	<element name="ulProperty" type="IN">User group properties. See category "UserDB properties"</element>
 */
typedef struct
{
	char *pszGroup;
	RTS_HANDLE hUserList;
	RTS_HANDLE hGroupList;
	RTS_UI32 ulProperty;
} GroupDBEntry;


/**
 * <category>Predefined user groups</category>
 * <description>Predefined user groups</description>
 * <element name="USERDB_GROUP_ADMINISTRATOR" type="IN">Administrator group. A member of this groups has all access rights on all objects!</element>
 * <element name="USERDB_GROUP_DEVELOPER" type="IN">Developer group. A member of this groups has all access to the IEC applications an all related stuff.</element>
 * <element name="USERDB_GROUP_SERVICE" type="IN">Service group. A member of this groups has all access to object for maintenance and service operations.</element>
 * <element name="USERDB_GROUP_WATCH" type="IN">Watch group. A member of this groups has read-only access to the objects.</element>
 * <element name="USERDB_GROUP_EVERYONE" type="IN">Everyone group. Every user is automatically member of this group!
 *					NOTE:
 *					By default, this group has read-only access to the objects. But this can be deactivated, so every user must be a member of one of the groups above and have
 *					no access right by default!
 * </element>
 */
#define USERDB_GROUP_ADMINISTRATOR		"Administrator"
#define USERDB_GROUP_DEVELOPER			"Developer"
#define USERDB_GROUP_SERVICE			"Service"
#define USERDB_GROUP_WATCH				"Watch"
#define USERDB_GROUP_EVERYONE			"Everyone"

/**
 * <category>Predefined users</category>
 * <description>Predefined users that are created for the default user management</description>
 */
#ifndef USERDB_USER_ADMINISTRATOR
	#define USERDB_USER_ADMINISTRATOR								"Administrator"
#endif
#ifndef USERDB_USER_ADMINISTRATOR_INITIAL_PASSWORD
	#define USERDB_USER_ADMINISTRATOR_INITIAL_PASSWORD				USERDB_USER_ADMINISTRATOR
#endif
#ifndef USERDB_USER_ANONYMOUS
	#define USERDB_USER_ANONYMOUS									"Anonymous"
#endif


/**
 * <category>User rights</category>
 * <description>Defined rights that can fully control the access to an object</description>
 * <element name="USERDB_RIGHT_NONE" type="IN">Access denied</element>
 * <element name="USERDB_RIGHT_VIEW" type="IN">Only view rights on the object. Object cannot be modified in any way!</element>
 * <element name="USERDB_RIGHT_MODIFY" type="IN">Object can be viewed and modified</element>
 * <element name="USERDB_RIGHT_EXECUTE" type="IN">Object can be executed</element>
 * <element name="USERDB_RIGHT_ADD_REMOVE" type="IN">It is allowed to add and remove subobjects</element>
 * <element name="USERDB_RIGHT_OEM" type="IN">The upper 8 bits of the access rights are reserved for OEM specific access rights</element>
 * <element name="USERDB_RIGHT_ALL" type="IN">All rights allowed on the object</element>
 */
#define USERDB_RIGHT_NONE					UINT32_C(0x00000000)
#define USERDB_RIGHT_VIEW					UINT32_C(0x00000001)
#define USERDB_RIGHT_MODIFY					UINT32_C(0x00000002)
#define USERDB_RIGHT_EXECUTE				UINT32_C(0x00000004)
#define USERDB_RIGHT_ADD_REMOVE				UINT32_C(0x00000008)
#define USERDB_RIGHT_OEM					UINT32_C(0xFF000000)
#define USERDB_RIGHT_ALL					UINT32_C(0xFFFFFFFF)

/**
 * <category>Event parameter</category>
 * <element name="bUserManagementChanged" type="IN">TRUE: Usermanagement database changed; FALSE: Not changed</element>
 * <element name="bRightsManagementChanged" type="IN">TRUE: Rightsmanagement database changed; FALSE: Not changed</element>
 */
typedef struct
{
	RTS_HANDLE hGroup;
} EVTPARAM_UserDbRemoveGroupFromRightsDB;

#define EVTPARAMID_UserDbRemoveGroupFromRightsDB	0x0001
#define EVTVERSION_UserDbRemoveGroupFromRightsDB	0x0001

/**
 * <category>Events</category>
 * <description>Event is sent every time the usermanagement database or the userrights database was changed</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpUserMgrDatabaseChanged</param>
 */
#define EVT_UserDbRemoveGroupFromRightsDB		MAKE_EVENTID(EVTCLASS_INFO, 1)


#ifdef __cplusplus
extern "C" {
#endif

/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ */
/*                    Users and UserGroups management																														*/
/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ */

/* --------- User management configuration ---------------------------------- */ 
/* This section of functions is OPTIONAL and must only be implemented, if the */
/* Users + Groups configuration in CODESYS is used!							  */

/**
 * <description>Function to check if users and usergroups database is configurable (not fix compiled)</description>
 * <param name="pulPropertyUsers" type="OUT">Pointer to get the users property: USERDB_PROP_ALL = Users are configurable, USERDB_PROP_NONE = Users are not configurable</param>
 * <param name="pulPropertyGroups" type="OUT">Pointer to get the user groups property: USERDB_PROP_ALL = Groups are configurable, USERDB_PROP_NONE = Groups are not configurable</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Could not be retrieved</errorcode>
 */
RTS_RESULT CDECL UserDBIsConfigurable(RTS_UI32 *pulPropertyUsers, RTS_UI32 *pulPropertyGroups);
typedef RTS_RESULT (CDECL * PFUSERDBISCONFIGURABLE) (RTS_UI32 *pulPropertyUsers, RTS_UI32 *pulPropertyGroups);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBISCONFIGURABLE_NOTIMPLEMENTED)
	#define USE_UserDBIsConfigurable
	#define EXT_UserDBIsConfigurable
	#define GET_UserDBIsConfigurable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBIsConfigurable(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBIsConfigurable  FALSE
	#define EXP_UserDBIsConfigurable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBIsConfigurable
	#define EXT_UserDBIsConfigurable
	#define GET_UserDBIsConfigurable(fl)  CAL_CMGETAPI( "UserDBIsConfigurable" ) 
	#define CAL_UserDBIsConfigurable  UserDBIsConfigurable
	#define CHK_UserDBIsConfigurable  TRUE
	#define EXP_UserDBIsConfigurable  CAL_CMEXPAPI( "UserDBIsConfigurable" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBIsConfigurable
	#define EXT_UserDBIsConfigurable
	#define GET_UserDBIsConfigurable(fl)  CAL_CMGETAPI( "UserDBIsConfigurable" ) 
	#define CAL_UserDBIsConfigurable  UserDBIsConfigurable
	#define CHK_UserDBIsConfigurable  TRUE
	#define EXP_UserDBIsConfigurable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBIsConfigurable", (RTS_UINTPTR)UserDBIsConfigurable, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBIsConfigurable
	#define EXT_CmpUserDBUserDBIsConfigurable
	#define GET_CmpUserDBUserDBIsConfigurable  ERR_OK
	#define CAL_CmpUserDBUserDBIsConfigurable pICmpUserDB->IUserDBIsConfigurable
	#define CHK_CmpUserDBUserDBIsConfigurable (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBIsConfigurable  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBIsConfigurable
	#define EXT_UserDBIsConfigurable
	#define GET_UserDBIsConfigurable(fl)  CAL_CMGETAPI( "UserDBIsConfigurable" ) 
	#define CAL_UserDBIsConfigurable pICmpUserDB->IUserDBIsConfigurable
	#define CHK_UserDBIsConfigurable (pICmpUserDB != NULL)
	#define EXP_UserDBIsConfigurable  CAL_CMEXPAPI( "UserDBIsConfigurable" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBIsConfigurable  PFUSERDBISCONFIGURABLE pfUserDBIsConfigurable;
	#define EXT_UserDBIsConfigurable  extern PFUSERDBISCONFIGURABLE pfUserDBIsConfigurable;
	#define GET_UserDBIsConfigurable(fl)  s_pfCMGetAPI2( "UserDBIsConfigurable", (RTS_VOID_FCTPTR *)&pfUserDBIsConfigurable, (fl), 0, 0)
	#define CAL_UserDBIsConfigurable  pfUserDBIsConfigurable
	#define CHK_UserDBIsConfigurable  (pfUserDBIsConfigurable != NULL)
	#define EXP_UserDBIsConfigurable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBIsConfigurable", (RTS_UINTPTR)UserDBIsConfigurable, 0, 0) 
#endif




/**
 * <description>Load the user database into memory</description>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Error loading user database</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">Is not implemented perhaps the UserDB is fix</errorcode>
 */
RTS_RESULT CDECL UserDBLoad(void);
typedef RTS_RESULT (CDECL * PFUSERDBLOAD) (void);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBLOAD_NOTIMPLEMENTED)
	#define USE_UserDBLoad
	#define EXT_UserDBLoad
	#define GET_UserDBLoad(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBLoad()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBLoad  FALSE
	#define EXP_UserDBLoad  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBLoad
	#define EXT_UserDBLoad
	#define GET_UserDBLoad(fl)  CAL_CMGETAPI( "UserDBLoad" ) 
	#define CAL_UserDBLoad  UserDBLoad
	#define CHK_UserDBLoad  TRUE
	#define EXP_UserDBLoad  CAL_CMEXPAPI( "UserDBLoad" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBLoad
	#define EXT_UserDBLoad
	#define GET_UserDBLoad(fl)  CAL_CMGETAPI( "UserDBLoad" ) 
	#define CAL_UserDBLoad  UserDBLoad
	#define CHK_UserDBLoad  TRUE
	#define EXP_UserDBLoad  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBLoad", (RTS_UINTPTR)UserDBLoad, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBLoad
	#define EXT_CmpUserDBUserDBLoad
	#define GET_CmpUserDBUserDBLoad  ERR_OK
	#define CAL_CmpUserDBUserDBLoad pICmpUserDB->IUserDBLoad
	#define CHK_CmpUserDBUserDBLoad (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBLoad  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBLoad
	#define EXT_UserDBLoad
	#define GET_UserDBLoad(fl)  CAL_CMGETAPI( "UserDBLoad" ) 
	#define CAL_UserDBLoad pICmpUserDB->IUserDBLoad
	#define CHK_UserDBLoad (pICmpUserDB != NULL)
	#define EXP_UserDBLoad  CAL_CMEXPAPI( "UserDBLoad" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBLoad  PFUSERDBLOAD pfUserDBLoad;
	#define EXT_UserDBLoad  extern PFUSERDBLOAD pfUserDBLoad;
	#define GET_UserDBLoad(fl)  s_pfCMGetAPI2( "UserDBLoad", (RTS_VOID_FCTPTR *)&pfUserDBLoad, (fl), 0, 0)
	#define CAL_UserDBLoad  pfUserDBLoad
	#define CHK_UserDBLoad  (pfUserDBLoad != NULL)
	#define EXP_UserDBLoad  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBLoad", (RTS_UINTPTR)UserDBLoad, 0, 0) 
#endif




/**
 * <description>Storing the current user management into permanent database file</description>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Error storing user database</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">Is not implemented perhaps the UserDB is fix</errorcode>
 */
RTS_RESULT CDECL UserDBStore(void);
typedef RTS_RESULT (CDECL * PFUSERDBSTORE) (void);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBSTORE_NOTIMPLEMENTED)
	#define USE_UserDBStore
	#define EXT_UserDBStore
	#define GET_UserDBStore(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBStore()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBStore  FALSE
	#define EXP_UserDBStore  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBStore
	#define EXT_UserDBStore
	#define GET_UserDBStore(fl)  CAL_CMGETAPI( "UserDBStore" ) 
	#define CAL_UserDBStore  UserDBStore
	#define CHK_UserDBStore  TRUE
	#define EXP_UserDBStore  CAL_CMEXPAPI( "UserDBStore" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBStore
	#define EXT_UserDBStore
	#define GET_UserDBStore(fl)  CAL_CMGETAPI( "UserDBStore" ) 
	#define CAL_UserDBStore  UserDBStore
	#define CHK_UserDBStore  TRUE
	#define EXP_UserDBStore  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBStore", (RTS_UINTPTR)UserDBStore, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBStore
	#define EXT_CmpUserDBUserDBStore
	#define GET_CmpUserDBUserDBStore  ERR_OK
	#define CAL_CmpUserDBUserDBStore pICmpUserDB->IUserDBStore
	#define CHK_CmpUserDBUserDBStore (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBStore  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBStore
	#define EXT_UserDBStore
	#define GET_UserDBStore(fl)  CAL_CMGETAPI( "UserDBStore" ) 
	#define CAL_UserDBStore pICmpUserDB->IUserDBStore
	#define CHK_UserDBStore (pICmpUserDB != NULL)
	#define EXP_UserDBStore  CAL_CMEXPAPI( "UserDBStore" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBStore  PFUSERDBSTORE pfUserDBStore;
	#define EXT_UserDBStore  extern PFUSERDBSTORE pfUserDBStore;
	#define GET_UserDBStore(fl)  s_pfCMGetAPI2( "UserDBStore", (RTS_VOID_FCTPTR *)&pfUserDBStore, (fl), 0, 0)
	#define CAL_UserDBStore  pfUserDBStore
	#define CHK_UserDBStore  (pfUserDBStore != NULL)
	#define EXP_UserDBStore  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBStore", (RTS_UINTPTR)UserDBStore, 0, 0) 
#endif




/**
 * <description>Release user management in memory. NOTE: The database file is not removed!</description>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Failed releasing user management</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">Is not implemented perhaps the UserDB is fix</errorcode>
 */
RTS_RESULT CDECL UserDBRelease(void);
typedef RTS_RESULT (CDECL * PFUSERDBRELEASE) (void);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBRELEASE_NOTIMPLEMENTED)
	#define USE_UserDBRelease
	#define EXT_UserDBRelease
	#define GET_UserDBRelease(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBRelease()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBRelease  FALSE
	#define EXP_UserDBRelease  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBRelease
	#define EXT_UserDBRelease
	#define GET_UserDBRelease(fl)  CAL_CMGETAPI( "UserDBRelease" ) 
	#define CAL_UserDBRelease  UserDBRelease
	#define CHK_UserDBRelease  TRUE
	#define EXP_UserDBRelease  CAL_CMEXPAPI( "UserDBRelease" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBRelease
	#define EXT_UserDBRelease
	#define GET_UserDBRelease(fl)  CAL_CMGETAPI( "UserDBRelease" ) 
	#define CAL_UserDBRelease  UserDBRelease
	#define CHK_UserDBRelease  TRUE
	#define EXP_UserDBRelease  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBRelease", (RTS_UINTPTR)UserDBRelease, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBRelease
	#define EXT_CmpUserDBUserDBRelease
	#define GET_CmpUserDBUserDBRelease  ERR_OK
	#define CAL_CmpUserDBUserDBRelease pICmpUserDB->IUserDBRelease
	#define CHK_CmpUserDBUserDBRelease (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBRelease  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBRelease
	#define EXT_UserDBRelease
	#define GET_UserDBRelease(fl)  CAL_CMGETAPI( "UserDBRelease" ) 
	#define CAL_UserDBRelease pICmpUserDB->IUserDBRelease
	#define CHK_UserDBRelease (pICmpUserDB != NULL)
	#define EXP_UserDBRelease  CAL_CMEXPAPI( "UserDBRelease" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBRelease  PFUSERDBRELEASE pfUserDBRelease;
	#define EXT_UserDBRelease  extern PFUSERDBRELEASE pfUserDBRelease;
	#define GET_UserDBRelease(fl)  s_pfCMGetAPI2( "UserDBRelease", (RTS_VOID_FCTPTR *)&pfUserDBRelease, (fl), 0, 0)
	#define CAL_UserDBRelease  pfUserDBRelease
	#define CHK_UserDBRelease  (pfUserDBRelease != NULL)
	#define EXP_UserDBRelease  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBRelease", (RTS_UINTPTR)UserDBRelease, 0, 0) 
#endif




/* Single user configuration */

/**
 * <description>Add a new user to the user database</description>
 * <param name="pszUser" type="IN">User name</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">User already exists</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter. pszUser = NULL or empty</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory available to store new user</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <result>Handle to the added user or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBAdd(char *pszUser, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBADD) (char *pszUser, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBADD_NOTIMPLEMENTED)
	#define USE_UserDBAdd
	#define EXT_UserDBAdd
	#define GET_UserDBAdd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBAdd(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBAdd  FALSE
	#define EXP_UserDBAdd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBAdd
	#define EXT_UserDBAdd
	#define GET_UserDBAdd(fl)  CAL_CMGETAPI( "UserDBAdd" ) 
	#define CAL_UserDBAdd  UserDBAdd
	#define CHK_UserDBAdd  TRUE
	#define EXP_UserDBAdd  CAL_CMEXPAPI( "UserDBAdd" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBAdd
	#define EXT_UserDBAdd
	#define GET_UserDBAdd(fl)  CAL_CMGETAPI( "UserDBAdd" ) 
	#define CAL_UserDBAdd  UserDBAdd
	#define CHK_UserDBAdd  TRUE
	#define EXP_UserDBAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBAdd", (RTS_UINTPTR)UserDBAdd, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBAdd
	#define EXT_CmpUserDBUserDBAdd
	#define GET_CmpUserDBUserDBAdd  ERR_OK
	#define CAL_CmpUserDBUserDBAdd pICmpUserDB->IUserDBAdd
	#define CHK_CmpUserDBUserDBAdd (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBAdd  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBAdd
	#define EXT_UserDBAdd
	#define GET_UserDBAdd(fl)  CAL_CMGETAPI( "UserDBAdd" ) 
	#define CAL_UserDBAdd pICmpUserDB->IUserDBAdd
	#define CHK_UserDBAdd (pICmpUserDB != NULL)
	#define EXP_UserDBAdd  CAL_CMEXPAPI( "UserDBAdd" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBAdd  PFUSERDBADD pfUserDBAdd;
	#define EXT_UserDBAdd  extern PFUSERDBADD pfUserDBAdd;
	#define GET_UserDBAdd(fl)  s_pfCMGetAPI2( "UserDBAdd", (RTS_VOID_FCTPTR *)&pfUserDBAdd, (fl), 0, 0)
	#define CAL_UserDBAdd  pfUserDBAdd
	#define CHK_UserDBAdd  (pfUserDBAdd != NULL)
	#define EXP_UserDBAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBAdd", (RTS_UINTPTR)UserDBAdd, 0, 0) 
#endif




/**
 * <description>Remove user from the user database</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle</errorcode>
 */
RTS_RESULT CDECL UserDBRemove(RTS_HANDLE hUser);
typedef RTS_RESULT (CDECL * PFUSERDBREMOVE) (RTS_HANDLE hUser);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBREMOVE_NOTIMPLEMENTED)
	#define USE_UserDBRemove
	#define EXT_UserDBRemove
	#define GET_UserDBRemove(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBRemove(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBRemove  FALSE
	#define EXP_UserDBRemove  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBRemove
	#define EXT_UserDBRemove
	#define GET_UserDBRemove(fl)  CAL_CMGETAPI( "UserDBRemove" ) 
	#define CAL_UserDBRemove  UserDBRemove
	#define CHK_UserDBRemove  TRUE
	#define EXP_UserDBRemove  CAL_CMEXPAPI( "UserDBRemove" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBRemove
	#define EXT_UserDBRemove
	#define GET_UserDBRemove(fl)  CAL_CMGETAPI( "UserDBRemove" ) 
	#define CAL_UserDBRemove  UserDBRemove
	#define CHK_UserDBRemove  TRUE
	#define EXP_UserDBRemove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBRemove", (RTS_UINTPTR)UserDBRemove, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBRemove
	#define EXT_CmpUserDBUserDBRemove
	#define GET_CmpUserDBUserDBRemove  ERR_OK
	#define CAL_CmpUserDBUserDBRemove pICmpUserDB->IUserDBRemove
	#define CHK_CmpUserDBUserDBRemove (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBRemove  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBRemove
	#define EXT_UserDBRemove
	#define GET_UserDBRemove(fl)  CAL_CMGETAPI( "UserDBRemove" ) 
	#define CAL_UserDBRemove pICmpUserDB->IUserDBRemove
	#define CHK_UserDBRemove (pICmpUserDB != NULL)
	#define EXP_UserDBRemove  CAL_CMEXPAPI( "UserDBRemove" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBRemove  PFUSERDBREMOVE pfUserDBRemove;
	#define EXT_UserDBRemove  extern PFUSERDBREMOVE pfUserDBRemove;
	#define GET_UserDBRemove(fl)  s_pfCMGetAPI2( "UserDBRemove", (RTS_VOID_FCTPTR *)&pfUserDBRemove, (fl), 0, 0)
	#define CAL_UserDBRemove  pfUserDBRemove
	#define CHK_UserDBRemove  (pfUserDBRemove != NULL)
	#define EXP_UserDBRemove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBRemove", (RTS_UINTPTR)UserDBRemove, 0, 0) 
#endif




/**
 * <description>Set or overwrite the password credential of the user</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pszPassword" type="IN">Pointer to password in cleartext</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Password could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle or password = NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">The new and the old password are the same</errorcode>
 */
RTS_RESULT CDECL UserDBSetPassword(RTS_HANDLE hUser, char *pszPassword);
typedef RTS_RESULT (CDECL * PFUSERDBSETPASSWORD) (RTS_HANDLE hUser, char *pszPassword);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBSETPASSWORD_NOTIMPLEMENTED)
	#define USE_UserDBSetPassword
	#define EXT_UserDBSetPassword
	#define GET_UserDBSetPassword(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBSetPassword(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBSetPassword  FALSE
	#define EXP_UserDBSetPassword  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBSetPassword
	#define EXT_UserDBSetPassword
	#define GET_UserDBSetPassword(fl)  CAL_CMGETAPI( "UserDBSetPassword" ) 
	#define CAL_UserDBSetPassword  UserDBSetPassword
	#define CHK_UserDBSetPassword  TRUE
	#define EXP_UserDBSetPassword  CAL_CMEXPAPI( "UserDBSetPassword" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBSetPassword
	#define EXT_UserDBSetPassword
	#define GET_UserDBSetPassword(fl)  CAL_CMGETAPI( "UserDBSetPassword" ) 
	#define CAL_UserDBSetPassword  UserDBSetPassword
	#define CHK_UserDBSetPassword  TRUE
	#define EXP_UserDBSetPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBSetPassword", (RTS_UINTPTR)UserDBSetPassword, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBSetPassword
	#define EXT_CmpUserDBUserDBSetPassword
	#define GET_CmpUserDBUserDBSetPassword  ERR_OK
	#define CAL_CmpUserDBUserDBSetPassword pICmpUserDB->IUserDBSetPassword
	#define CHK_CmpUserDBUserDBSetPassword (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBSetPassword  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBSetPassword
	#define EXT_UserDBSetPassword
	#define GET_UserDBSetPassword(fl)  CAL_CMGETAPI( "UserDBSetPassword" ) 
	#define CAL_UserDBSetPassword pICmpUserDB->IUserDBSetPassword
	#define CHK_UserDBSetPassword (pICmpUserDB != NULL)
	#define EXP_UserDBSetPassword  CAL_CMEXPAPI( "UserDBSetPassword" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBSetPassword  PFUSERDBSETPASSWORD pfUserDBSetPassword;
	#define EXT_UserDBSetPassword  extern PFUSERDBSETPASSWORD pfUserDBSetPassword;
	#define GET_UserDBSetPassword(fl)  s_pfCMGetAPI2( "UserDBSetPassword", (RTS_VOID_FCTPTR *)&pfUserDBSetPassword, (fl), 0, 0)
	#define CAL_UserDBSetPassword  pfUserDBSetPassword
	#define CHK_UserDBSetPassword  (pfUserDBSetPassword != NULL)
	#define EXP_UserDBSetPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBSetPassword", (RTS_UINTPTR)UserDBSetPassword, 0, 0) 
#endif




/**
 * <description>Get the password credential of the user</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pbyPasswordMD5" type="IN">Pointer to MD5 hash of the password</param>
 * <param name="piMaxLen" type="INOUT">Pointer to password MD5 hash length. If it is too short, needed length is returned if error code is ERR_BUFFERSIZE</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Password could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle or piMaxLen = NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">If password buffer is too short. Needed length is returned in *piMaxLen </errorcode>
 */
RTS_RESULT CDECL UserDBGetPasswordMD5(RTS_HANDLE hUser, unsigned char *pbyPasswordMD5, int *piMaxLen);
typedef RTS_RESULT (CDECL * PFUSERDBGETPASSWORDMD5) (RTS_HANDLE hUser, unsigned char *pbyPasswordMD5, int *piMaxLen);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGETPASSWORDMD5_NOTIMPLEMENTED)
	#define USE_UserDBGetPasswordMD5
	#define EXT_UserDBGetPasswordMD5
	#define GET_UserDBGetPasswordMD5(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGetPasswordMD5(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGetPasswordMD5  FALSE
	#define EXP_UserDBGetPasswordMD5  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGetPasswordMD5
	#define EXT_UserDBGetPasswordMD5
	#define GET_UserDBGetPasswordMD5(fl)  CAL_CMGETAPI( "UserDBGetPasswordMD5" ) 
	#define CAL_UserDBGetPasswordMD5  UserDBGetPasswordMD5
	#define CHK_UserDBGetPasswordMD5  TRUE
	#define EXP_UserDBGetPasswordMD5  CAL_CMEXPAPI( "UserDBGetPasswordMD5" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGetPasswordMD5
	#define EXT_UserDBGetPasswordMD5
	#define GET_UserDBGetPasswordMD5(fl)  CAL_CMGETAPI( "UserDBGetPasswordMD5" ) 
	#define CAL_UserDBGetPasswordMD5  UserDBGetPasswordMD5
	#define CHK_UserDBGetPasswordMD5  TRUE
	#define EXP_UserDBGetPasswordMD5  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetPasswordMD5", (RTS_UINTPTR)UserDBGetPasswordMD5, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGetPasswordMD5
	#define EXT_CmpUserDBUserDBGetPasswordMD5
	#define GET_CmpUserDBUserDBGetPasswordMD5  ERR_OK
	#define CAL_CmpUserDBUserDBGetPasswordMD5 pICmpUserDB->IUserDBGetPasswordMD5
	#define CHK_CmpUserDBUserDBGetPasswordMD5 (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGetPasswordMD5  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGetPasswordMD5
	#define EXT_UserDBGetPasswordMD5
	#define GET_UserDBGetPasswordMD5(fl)  CAL_CMGETAPI( "UserDBGetPasswordMD5" ) 
	#define CAL_UserDBGetPasswordMD5 pICmpUserDB->IUserDBGetPasswordMD5
	#define CHK_UserDBGetPasswordMD5 (pICmpUserDB != NULL)
	#define EXP_UserDBGetPasswordMD5  CAL_CMEXPAPI( "UserDBGetPasswordMD5" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGetPasswordMD5  PFUSERDBGETPASSWORDMD5 pfUserDBGetPasswordMD5;
	#define EXT_UserDBGetPasswordMD5  extern PFUSERDBGETPASSWORDMD5 pfUserDBGetPasswordMD5;
	#define GET_UserDBGetPasswordMD5(fl)  s_pfCMGetAPI2( "UserDBGetPasswordMD5", (RTS_VOID_FCTPTR *)&pfUserDBGetPasswordMD5, (fl), 0, 0)
	#define CAL_UserDBGetPasswordMD5  pfUserDBGetPasswordMD5
	#define CHK_UserDBGetPasswordMD5  (pfUserDBGetPasswordMD5 != NULL)
	#define EXP_UserDBGetPasswordMD5  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetPasswordMD5", (RTS_UINTPTR)UserDBGetPasswordMD5, 0, 0) 
#endif




/**
 * <description>Set or overwrite the password credential of the user</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pbyPasswordMD5" type="IN">Pointer to MD5 hash of the password</param>
 * <param name="piMaxLen" type="INOUT">Pointer to password MD5 hash length. If it is too short, needed length is returned if error code is ERR_BUFFERSIZE</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Password could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle or piMaxLen = NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">If password is too short. Needed length is returned in *piMaxLen </errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">The new and the old password are the same</errorcode>
 */
RTS_RESULT CDECL UserDBSetPasswordMD5(RTS_HANDLE hUser, unsigned char *pbyPasswordMD5, int *piMaxLen);
typedef RTS_RESULT (CDECL * PFUSERDBSETPASSWORDMD5) (RTS_HANDLE hUser, unsigned char *pbyPasswordMD5, int *piMaxLen);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBSETPASSWORDMD5_NOTIMPLEMENTED)
	#define USE_UserDBSetPasswordMD5
	#define EXT_UserDBSetPasswordMD5
	#define GET_UserDBSetPasswordMD5(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBSetPasswordMD5(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBSetPasswordMD5  FALSE
	#define EXP_UserDBSetPasswordMD5  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBSetPasswordMD5
	#define EXT_UserDBSetPasswordMD5
	#define GET_UserDBSetPasswordMD5(fl)  CAL_CMGETAPI( "UserDBSetPasswordMD5" ) 
	#define CAL_UserDBSetPasswordMD5  UserDBSetPasswordMD5
	#define CHK_UserDBSetPasswordMD5  TRUE
	#define EXP_UserDBSetPasswordMD5  CAL_CMEXPAPI( "UserDBSetPasswordMD5" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBSetPasswordMD5
	#define EXT_UserDBSetPasswordMD5
	#define GET_UserDBSetPasswordMD5(fl)  CAL_CMGETAPI( "UserDBSetPasswordMD5" ) 
	#define CAL_UserDBSetPasswordMD5  UserDBSetPasswordMD5
	#define CHK_UserDBSetPasswordMD5  TRUE
	#define EXP_UserDBSetPasswordMD5  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBSetPasswordMD5", (RTS_UINTPTR)UserDBSetPasswordMD5, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBSetPasswordMD5
	#define EXT_CmpUserDBUserDBSetPasswordMD5
	#define GET_CmpUserDBUserDBSetPasswordMD5  ERR_OK
	#define CAL_CmpUserDBUserDBSetPasswordMD5 pICmpUserDB->IUserDBSetPasswordMD5
	#define CHK_CmpUserDBUserDBSetPasswordMD5 (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBSetPasswordMD5  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBSetPasswordMD5
	#define EXT_UserDBSetPasswordMD5
	#define GET_UserDBSetPasswordMD5(fl)  CAL_CMGETAPI( "UserDBSetPasswordMD5" ) 
	#define CAL_UserDBSetPasswordMD5 pICmpUserDB->IUserDBSetPasswordMD5
	#define CHK_UserDBSetPasswordMD5 (pICmpUserDB != NULL)
	#define EXP_UserDBSetPasswordMD5  CAL_CMEXPAPI( "UserDBSetPasswordMD5" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBSetPasswordMD5  PFUSERDBSETPASSWORDMD5 pfUserDBSetPasswordMD5;
	#define EXT_UserDBSetPasswordMD5  extern PFUSERDBSETPASSWORDMD5 pfUserDBSetPasswordMD5;
	#define GET_UserDBSetPasswordMD5(fl)  s_pfCMGetAPI2( "UserDBSetPasswordMD5", (RTS_VOID_FCTPTR *)&pfUserDBSetPasswordMD5, (fl), 0, 0)
	#define CAL_UserDBSetPasswordMD5  pfUserDBSetPasswordMD5
	#define CHK_UserDBSetPasswordMD5  (pfUserDBSetPasswordMD5 != NULL)
	#define EXP_UserDBSetPasswordMD5  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBSetPasswordMD5", (RTS_UINTPTR)UserDBSetPasswordMD5, 0, 0) 
#endif




/**
 * <description>Set properties of a user</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pulProperty" type="IN">Pointer to set the properties. For details see category "UserDB properties"</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Properties could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle or property pointer = NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Invalid combination of property flags (password change not allowed but password not up to date)</errorcode>
 */
RTS_RESULT CDECL UserDBSetProperty(RTS_HANDLE hUser, RTS_UI32 ulProperty);
typedef RTS_RESULT (CDECL * PFUSERDBSETPROPERTY) (RTS_HANDLE hUser, RTS_UI32 ulProperty);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBSETPROPERTY_NOTIMPLEMENTED)
	#define USE_UserDBSetProperty
	#define EXT_UserDBSetProperty
	#define GET_UserDBSetProperty(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBSetProperty(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBSetProperty  FALSE
	#define EXP_UserDBSetProperty  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBSetProperty
	#define EXT_UserDBSetProperty
	#define GET_UserDBSetProperty(fl)  CAL_CMGETAPI( "UserDBSetProperty" ) 
	#define CAL_UserDBSetProperty  UserDBSetProperty
	#define CHK_UserDBSetProperty  TRUE
	#define EXP_UserDBSetProperty  CAL_CMEXPAPI( "UserDBSetProperty" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBSetProperty
	#define EXT_UserDBSetProperty
	#define GET_UserDBSetProperty(fl)  CAL_CMGETAPI( "UserDBSetProperty" ) 
	#define CAL_UserDBSetProperty  UserDBSetProperty
	#define CHK_UserDBSetProperty  TRUE
	#define EXP_UserDBSetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBSetProperty", (RTS_UINTPTR)UserDBSetProperty, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBSetProperty
	#define EXT_CmpUserDBUserDBSetProperty
	#define GET_CmpUserDBUserDBSetProperty  ERR_OK
	#define CAL_CmpUserDBUserDBSetProperty pICmpUserDB->IUserDBSetProperty
	#define CHK_CmpUserDBUserDBSetProperty (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBSetProperty  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBSetProperty
	#define EXT_UserDBSetProperty
	#define GET_UserDBSetProperty(fl)  CAL_CMGETAPI( "UserDBSetProperty" ) 
	#define CAL_UserDBSetProperty pICmpUserDB->IUserDBSetProperty
	#define CHK_UserDBSetProperty (pICmpUserDB != NULL)
	#define EXP_UserDBSetProperty  CAL_CMEXPAPI( "UserDBSetProperty" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBSetProperty  PFUSERDBSETPROPERTY pfUserDBSetProperty;
	#define EXT_UserDBSetProperty  extern PFUSERDBSETPROPERTY pfUserDBSetProperty;
	#define GET_UserDBSetProperty(fl)  s_pfCMGetAPI2( "UserDBSetProperty", (RTS_VOID_FCTPTR *)&pfUserDBSetProperty, (fl), 0, 0)
	#define CAL_UserDBSetProperty  pfUserDBSetProperty
	#define CHK_UserDBSetProperty  (pfUserDBSetProperty != NULL)
	#define EXP_UserDBSetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBSetProperty", (RTS_UINTPTR)UserDBSetProperty, 0, 0) 
#endif





/* --------- User management ------------------------------------------------- */
/* This section of functions is MANDATORY for the user management!             */

/* User iteration */

/**
 * <description>Iteration interface to get the first configured user</description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First user available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No user configured</errorcode>
 * <result>Handle to the first user or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGetFirst(RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGETFIRST) (RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGETFIRST_NOTIMPLEMENTED)
	#define USE_UserDBGetFirst
	#define EXT_UserDBGetFirst
	#define GET_UserDBGetFirst(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGetFirst(p0)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGetFirst  FALSE
	#define EXP_UserDBGetFirst  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGetFirst
	#define EXT_UserDBGetFirst
	#define GET_UserDBGetFirst(fl)  CAL_CMGETAPI( "UserDBGetFirst" ) 
	#define CAL_UserDBGetFirst  UserDBGetFirst
	#define CHK_UserDBGetFirst  TRUE
	#define EXP_UserDBGetFirst  CAL_CMEXPAPI( "UserDBGetFirst" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGetFirst
	#define EXT_UserDBGetFirst
	#define GET_UserDBGetFirst(fl)  CAL_CMGETAPI( "UserDBGetFirst" ) 
	#define CAL_UserDBGetFirst  UserDBGetFirst
	#define CHK_UserDBGetFirst  TRUE
	#define EXP_UserDBGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetFirst", (RTS_UINTPTR)UserDBGetFirst, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGetFirst
	#define EXT_CmpUserDBUserDBGetFirst
	#define GET_CmpUserDBUserDBGetFirst  ERR_OK
	#define CAL_CmpUserDBUserDBGetFirst pICmpUserDB->IUserDBGetFirst
	#define CHK_CmpUserDBUserDBGetFirst (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGetFirst  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGetFirst
	#define EXT_UserDBGetFirst
	#define GET_UserDBGetFirst(fl)  CAL_CMGETAPI( "UserDBGetFirst" ) 
	#define CAL_UserDBGetFirst pICmpUserDB->IUserDBGetFirst
	#define CHK_UserDBGetFirst (pICmpUserDB != NULL)
	#define EXP_UserDBGetFirst  CAL_CMEXPAPI( "UserDBGetFirst" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGetFirst  PFUSERDBGETFIRST pfUserDBGetFirst;
	#define EXT_UserDBGetFirst  extern PFUSERDBGETFIRST pfUserDBGetFirst;
	#define GET_UserDBGetFirst(fl)  s_pfCMGetAPI2( "UserDBGetFirst", (RTS_VOID_FCTPTR *)&pfUserDBGetFirst, (fl), 0, 0)
	#define CAL_UserDBGetFirst  pfUserDBGetFirst
	#define CHK_UserDBGetFirst  (pfUserDBGetFirst != NULL)
	#define EXP_UserDBGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetFirst", (RTS_UINTPTR)UserDBGetFirst, 0, 0) 
#endif




/**
 * <description>Iteration interface to get next configured user. Must be started with UserDBGetFirst()</description>
 * <param name="hPrevUser" type="IN">Handle to the previous user retrieved via UserDBGetFirst() or subsequent calls of UserDBGetNext()</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next user available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next user available</errorcode>
 * <result>Handle to the next user or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGetNext(RTS_HANDLE hPrevUser, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGETNEXT) (RTS_HANDLE hPrevUser, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGETNEXT_NOTIMPLEMENTED)
	#define USE_UserDBGetNext
	#define EXT_UserDBGetNext
	#define GET_UserDBGetNext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGetNext(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGetNext  FALSE
	#define EXP_UserDBGetNext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGetNext
	#define EXT_UserDBGetNext
	#define GET_UserDBGetNext(fl)  CAL_CMGETAPI( "UserDBGetNext" ) 
	#define CAL_UserDBGetNext  UserDBGetNext
	#define CHK_UserDBGetNext  TRUE
	#define EXP_UserDBGetNext  CAL_CMEXPAPI( "UserDBGetNext" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGetNext
	#define EXT_UserDBGetNext
	#define GET_UserDBGetNext(fl)  CAL_CMGETAPI( "UserDBGetNext" ) 
	#define CAL_UserDBGetNext  UserDBGetNext
	#define CHK_UserDBGetNext  TRUE
	#define EXP_UserDBGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetNext", (RTS_UINTPTR)UserDBGetNext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGetNext
	#define EXT_CmpUserDBUserDBGetNext
	#define GET_CmpUserDBUserDBGetNext  ERR_OK
	#define CAL_CmpUserDBUserDBGetNext pICmpUserDB->IUserDBGetNext
	#define CHK_CmpUserDBUserDBGetNext (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGetNext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGetNext
	#define EXT_UserDBGetNext
	#define GET_UserDBGetNext(fl)  CAL_CMGETAPI( "UserDBGetNext" ) 
	#define CAL_UserDBGetNext pICmpUserDB->IUserDBGetNext
	#define CHK_UserDBGetNext (pICmpUserDB != NULL)
	#define EXP_UserDBGetNext  CAL_CMEXPAPI( "UserDBGetNext" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGetNext  PFUSERDBGETNEXT pfUserDBGetNext;
	#define EXT_UserDBGetNext  extern PFUSERDBGETNEXT pfUserDBGetNext;
	#define GET_UserDBGetNext(fl)  s_pfCMGetAPI2( "UserDBGetNext", (RTS_VOID_FCTPTR *)&pfUserDBGetNext, (fl), 0, 0)
	#define CAL_UserDBGetNext  pfUserDBGetNext
	#define CHK_UserDBGetNext  (pfUserDBGetNext != NULL)
	#define EXP_UserDBGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetNext", (RTS_UINTPTR)UserDBGetNext, 0, 0) 
#endif




/* Single user access */

/**
 * <description>Open user specified by its name</description>
 * <param name="pszUser" type="IN">User name</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be opened</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter. pszUser = NULL or empty</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <result>Handle to the user or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBOpen(char *pszUser, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBOPEN) (char *pszUser, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBOPEN_NOTIMPLEMENTED)
	#define USE_UserDBOpen
	#define EXT_UserDBOpen
	#define GET_UserDBOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBOpen(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBOpen  FALSE
	#define EXP_UserDBOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBOpen
	#define EXT_UserDBOpen
	#define GET_UserDBOpen(fl)  CAL_CMGETAPI( "UserDBOpen" ) 
	#define CAL_UserDBOpen  UserDBOpen
	#define CHK_UserDBOpen  TRUE
	#define EXP_UserDBOpen  CAL_CMEXPAPI( "UserDBOpen" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBOpen
	#define EXT_UserDBOpen
	#define GET_UserDBOpen(fl)  CAL_CMGETAPI( "UserDBOpen" ) 
	#define CAL_UserDBOpen  UserDBOpen
	#define CHK_UserDBOpen  TRUE
	#define EXP_UserDBOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBOpen", (RTS_UINTPTR)UserDBOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBOpen
	#define EXT_CmpUserDBUserDBOpen
	#define GET_CmpUserDBUserDBOpen  ERR_OK
	#define CAL_CmpUserDBUserDBOpen pICmpUserDB->IUserDBOpen
	#define CHK_CmpUserDBUserDBOpen (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBOpen
	#define EXT_UserDBOpen
	#define GET_UserDBOpen(fl)  CAL_CMGETAPI( "UserDBOpen" ) 
	#define CAL_UserDBOpen pICmpUserDB->IUserDBOpen
	#define CHK_UserDBOpen (pICmpUserDB != NULL)
	#define EXP_UserDBOpen  CAL_CMEXPAPI( "UserDBOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBOpen  PFUSERDBOPEN pfUserDBOpen;
	#define EXT_UserDBOpen  extern PFUSERDBOPEN pfUserDBOpen;
	#define GET_UserDBOpen(fl)  s_pfCMGetAPI2( "UserDBOpen", (RTS_VOID_FCTPTR *)&pfUserDBOpen, (fl), 0, 0)
	#define CAL_UserDBOpen  pfUserDBOpen
	#define CHK_UserDBOpen  (pfUserDBOpen != NULL)
	#define EXP_UserDBOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBOpen", (RTS_UINTPTR)UserDBOpen, 0, 0) 
#endif




/**
 * <description>Close a user</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be closed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle</errorcode>
 */
RTS_RESULT CDECL UserDBClose(RTS_HANDLE hUser);
typedef RTS_RESULT (CDECL * PFUSERDBCLOSE) (RTS_HANDLE hUser);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBCLOSE_NOTIMPLEMENTED)
	#define USE_UserDBClose
	#define EXT_UserDBClose
	#define GET_UserDBClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBClose  FALSE
	#define EXP_UserDBClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBClose
	#define EXT_UserDBClose
	#define GET_UserDBClose(fl)  CAL_CMGETAPI( "UserDBClose" ) 
	#define CAL_UserDBClose  UserDBClose
	#define CHK_UserDBClose  TRUE
	#define EXP_UserDBClose  CAL_CMEXPAPI( "UserDBClose" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBClose
	#define EXT_UserDBClose
	#define GET_UserDBClose(fl)  CAL_CMGETAPI( "UserDBClose" ) 
	#define CAL_UserDBClose  UserDBClose
	#define CHK_UserDBClose  TRUE
	#define EXP_UserDBClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBClose", (RTS_UINTPTR)UserDBClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBClose
	#define EXT_CmpUserDBUserDBClose
	#define GET_CmpUserDBUserDBClose  ERR_OK
	#define CAL_CmpUserDBUserDBClose pICmpUserDB->IUserDBClose
	#define CHK_CmpUserDBUserDBClose (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBClose
	#define EXT_UserDBClose
	#define GET_UserDBClose(fl)  CAL_CMGETAPI( "UserDBClose" ) 
	#define CAL_UserDBClose pICmpUserDB->IUserDBClose
	#define CHK_UserDBClose (pICmpUserDB != NULL)
	#define EXP_UserDBClose  CAL_CMEXPAPI( "UserDBClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBClose  PFUSERDBCLOSE pfUserDBClose;
	#define EXT_UserDBClose  extern PFUSERDBCLOSE pfUserDBClose;
	#define GET_UserDBClose(fl)  s_pfCMGetAPI2( "UserDBClose", (RTS_VOID_FCTPTR *)&pfUserDBClose, (fl), 0, 0)
	#define CAL_UserDBClose  pfUserDBClose
	#define CHK_UserDBClose  (pfUserDBClose != NULL)
	#define EXP_UserDBClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBClose", (RTS_UINTPTR)UserDBClose, 0, 0) 
#endif




/**
 * <description>Get user name</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pszUser" type="OUT">Pointer to get user name</param>
 * <param name="piMaxLen" type="INOUT">Pointer to length of buffer in pszUser. If pszUser = NULL, only the length of the user name is returned in *piMaxLen</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User name is returned</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle or pszUser = NULL and piMaxLen = NULL</errorcode>
 */
RTS_RESULT CDECL UserDBGetName(RTS_HANDLE hUser, char *pszUser, int *piMaxLen);
typedef RTS_RESULT (CDECL * PFUSERDBGETNAME) (RTS_HANDLE hUser, char *pszUser, int *piMaxLen);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGETNAME_NOTIMPLEMENTED)
	#define USE_UserDBGetName
	#define EXT_UserDBGetName
	#define GET_UserDBGetName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGetName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGetName  FALSE
	#define EXP_UserDBGetName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGetName
	#define EXT_UserDBGetName
	#define GET_UserDBGetName(fl)  CAL_CMGETAPI( "UserDBGetName" ) 
	#define CAL_UserDBGetName  UserDBGetName
	#define CHK_UserDBGetName  TRUE
	#define EXP_UserDBGetName  CAL_CMEXPAPI( "UserDBGetName" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGetName
	#define EXT_UserDBGetName
	#define GET_UserDBGetName(fl)  CAL_CMGETAPI( "UserDBGetName" ) 
	#define CAL_UserDBGetName  UserDBGetName
	#define CHK_UserDBGetName  TRUE
	#define EXP_UserDBGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetName", (RTS_UINTPTR)UserDBGetName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGetName
	#define EXT_CmpUserDBUserDBGetName
	#define GET_CmpUserDBUserDBGetName  ERR_OK
	#define CAL_CmpUserDBUserDBGetName pICmpUserDB->IUserDBGetName
	#define CHK_CmpUserDBUserDBGetName (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGetName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGetName
	#define EXT_UserDBGetName
	#define GET_UserDBGetName(fl)  CAL_CMGETAPI( "UserDBGetName" ) 
	#define CAL_UserDBGetName pICmpUserDB->IUserDBGetName
	#define CHK_UserDBGetName (pICmpUserDB != NULL)
	#define EXP_UserDBGetName  CAL_CMEXPAPI( "UserDBGetName" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGetName  PFUSERDBGETNAME pfUserDBGetName;
	#define EXT_UserDBGetName  extern PFUSERDBGETNAME pfUserDBGetName;
	#define GET_UserDBGetName(fl)  s_pfCMGetAPI2( "UserDBGetName", (RTS_VOID_FCTPTR *)&pfUserDBGetName, (fl), 0, 0)
	#define CAL_UserDBGetName  pfUserDBGetName
	#define CHK_UserDBGetName  (pfUserDBGetName != NULL)
	#define EXP_UserDBGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetName", (RTS_UINTPTR)UserDBGetName, 0, 0) 
#endif




/**
 * <description>Get properties of a user</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pulProperty" type="OUT">Pointer to return the properties. For details see category "UserDB properties"</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Properties could be retrieved</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle or property pointer = NULL</errorcode>
 */
RTS_RESULT CDECL UserDBGetProperty(RTS_HANDLE hUser, RTS_UI32 *pulProperty);
typedef RTS_RESULT (CDECL * PFUSERDBGETPROPERTY) (RTS_HANDLE hUser, RTS_UI32 *pulProperty);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGETPROPERTY_NOTIMPLEMENTED)
	#define USE_UserDBGetProperty
	#define EXT_UserDBGetProperty
	#define GET_UserDBGetProperty(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGetProperty(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGetProperty  FALSE
	#define EXP_UserDBGetProperty  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGetProperty
	#define EXT_UserDBGetProperty
	#define GET_UserDBGetProperty(fl)  CAL_CMGETAPI( "UserDBGetProperty" ) 
	#define CAL_UserDBGetProperty  UserDBGetProperty
	#define CHK_UserDBGetProperty  TRUE
	#define EXP_UserDBGetProperty  CAL_CMEXPAPI( "UserDBGetProperty" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGetProperty
	#define EXT_UserDBGetProperty
	#define GET_UserDBGetProperty(fl)  CAL_CMGETAPI( "UserDBGetProperty" ) 
	#define CAL_UserDBGetProperty  UserDBGetProperty
	#define CHK_UserDBGetProperty  TRUE
	#define EXP_UserDBGetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetProperty", (RTS_UINTPTR)UserDBGetProperty, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGetProperty
	#define EXT_CmpUserDBUserDBGetProperty
	#define GET_CmpUserDBUserDBGetProperty  ERR_OK
	#define CAL_CmpUserDBUserDBGetProperty pICmpUserDB->IUserDBGetProperty
	#define CHK_CmpUserDBUserDBGetProperty (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGetProperty  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGetProperty
	#define EXT_UserDBGetProperty
	#define GET_UserDBGetProperty(fl)  CAL_CMGETAPI( "UserDBGetProperty" ) 
	#define CAL_UserDBGetProperty pICmpUserDB->IUserDBGetProperty
	#define CHK_UserDBGetProperty (pICmpUserDB != NULL)
	#define EXP_UserDBGetProperty  CAL_CMEXPAPI( "UserDBGetProperty" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGetProperty  PFUSERDBGETPROPERTY pfUserDBGetProperty;
	#define EXT_UserDBGetProperty  extern PFUSERDBGETPROPERTY pfUserDBGetProperty;
	#define GET_UserDBGetProperty(fl)  s_pfCMGetAPI2( "UserDBGetProperty", (RTS_VOID_FCTPTR *)&pfUserDBGetProperty, (fl), 0, 0)
	#define CAL_UserDBGetProperty  pfUserDBGetProperty
	#define CHK_UserDBGetProperty  (pfUserDBGetProperty != NULL)
	#define EXP_UserDBGetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetProperty", (RTS_UINTPTR)UserDBGetProperty, 0, 0) 
#endif




/**
 * <description>Check user credentials (authentication)</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pszPassword" type="IN">Pointer to password in cleartext</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Password credential matched!</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle or password = NULL so password credential does not match!</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Password credential does not match!</errorcode>
 */
RTS_RESULT CDECL UserDBCheckPassword(RTS_HANDLE hUser, char *pszPassword);
typedef RTS_RESULT (CDECL * PFUSERDBCHECKPASSWORD) (RTS_HANDLE hUser, char *pszPassword);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBCHECKPASSWORD_NOTIMPLEMENTED)
	#define USE_UserDBCheckPassword
	#define EXT_UserDBCheckPassword
	#define GET_UserDBCheckPassword(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBCheckPassword(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBCheckPassword  FALSE
	#define EXP_UserDBCheckPassword  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBCheckPassword
	#define EXT_UserDBCheckPassword
	#define GET_UserDBCheckPassword(fl)  CAL_CMGETAPI( "UserDBCheckPassword" ) 
	#define CAL_UserDBCheckPassword  UserDBCheckPassword
	#define CHK_UserDBCheckPassword  TRUE
	#define EXP_UserDBCheckPassword  CAL_CMEXPAPI( "UserDBCheckPassword" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBCheckPassword
	#define EXT_UserDBCheckPassword
	#define GET_UserDBCheckPassword(fl)  CAL_CMGETAPI( "UserDBCheckPassword" ) 
	#define CAL_UserDBCheckPassword  UserDBCheckPassword
	#define CHK_UserDBCheckPassword  TRUE
	#define EXP_UserDBCheckPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBCheckPassword", (RTS_UINTPTR)UserDBCheckPassword, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBCheckPassword
	#define EXT_CmpUserDBUserDBCheckPassword
	#define GET_CmpUserDBUserDBCheckPassword  ERR_OK
	#define CAL_CmpUserDBUserDBCheckPassword pICmpUserDB->IUserDBCheckPassword
	#define CHK_CmpUserDBUserDBCheckPassword (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBCheckPassword  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBCheckPassword
	#define EXT_UserDBCheckPassword
	#define GET_UserDBCheckPassword(fl)  CAL_CMGETAPI( "UserDBCheckPassword" ) 
	#define CAL_UserDBCheckPassword pICmpUserDB->IUserDBCheckPassword
	#define CHK_UserDBCheckPassword (pICmpUserDB != NULL)
	#define EXP_UserDBCheckPassword  CAL_CMEXPAPI( "UserDBCheckPassword" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBCheckPassword  PFUSERDBCHECKPASSWORD pfUserDBCheckPassword;
	#define EXT_UserDBCheckPassword  extern PFUSERDBCHECKPASSWORD pfUserDBCheckPassword;
	#define GET_UserDBCheckPassword(fl)  s_pfCMGetAPI2( "UserDBCheckPassword", (RTS_VOID_FCTPTR *)&pfUserDBCheckPassword, (fl), 0, 0)
	#define CAL_UserDBCheckPassword  pfUserDBCheckPassword
	#define CHK_UserDBCheckPassword  (pfUserDBCheckPassword != NULL)
	#define EXP_UserDBCheckPassword  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBCheckPassword", (RTS_UINTPTR)UserDBCheckPassword, 0, 0) 
#endif




/* Iteration over all groups to which the user is assigned */

/**
 * <description>Iteration interface to get the first group to which the user is assigned</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First user group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user handle</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No user group configured</errorcode>
 * <result>Handle to the first user group or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGetFirstGroup(RTS_HANDLE hUser, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGETFIRSTGROUP) (RTS_HANDLE hUser, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGETFIRSTGROUP_NOTIMPLEMENTED)
	#define USE_UserDBGetFirstGroup
	#define EXT_UserDBGetFirstGroup
	#define GET_UserDBGetFirstGroup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGetFirstGroup(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGetFirstGroup  FALSE
	#define EXP_UserDBGetFirstGroup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGetFirstGroup
	#define EXT_UserDBGetFirstGroup
	#define GET_UserDBGetFirstGroup(fl)  CAL_CMGETAPI( "UserDBGetFirstGroup" ) 
	#define CAL_UserDBGetFirstGroup  UserDBGetFirstGroup
	#define CHK_UserDBGetFirstGroup  TRUE
	#define EXP_UserDBGetFirstGroup  CAL_CMEXPAPI( "UserDBGetFirstGroup" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGetFirstGroup
	#define EXT_UserDBGetFirstGroup
	#define GET_UserDBGetFirstGroup(fl)  CAL_CMGETAPI( "UserDBGetFirstGroup" ) 
	#define CAL_UserDBGetFirstGroup  UserDBGetFirstGroup
	#define CHK_UserDBGetFirstGroup  TRUE
	#define EXP_UserDBGetFirstGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetFirstGroup", (RTS_UINTPTR)UserDBGetFirstGroup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGetFirstGroup
	#define EXT_CmpUserDBUserDBGetFirstGroup
	#define GET_CmpUserDBUserDBGetFirstGroup  ERR_OK
	#define CAL_CmpUserDBUserDBGetFirstGroup pICmpUserDB->IUserDBGetFirstGroup
	#define CHK_CmpUserDBUserDBGetFirstGroup (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGetFirstGroup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGetFirstGroup
	#define EXT_UserDBGetFirstGroup
	#define GET_UserDBGetFirstGroup(fl)  CAL_CMGETAPI( "UserDBGetFirstGroup" ) 
	#define CAL_UserDBGetFirstGroup pICmpUserDB->IUserDBGetFirstGroup
	#define CHK_UserDBGetFirstGroup (pICmpUserDB != NULL)
	#define EXP_UserDBGetFirstGroup  CAL_CMEXPAPI( "UserDBGetFirstGroup" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGetFirstGroup  PFUSERDBGETFIRSTGROUP pfUserDBGetFirstGroup;
	#define EXT_UserDBGetFirstGroup  extern PFUSERDBGETFIRSTGROUP pfUserDBGetFirstGroup;
	#define GET_UserDBGetFirstGroup(fl)  s_pfCMGetAPI2( "UserDBGetFirstGroup", (RTS_VOID_FCTPTR *)&pfUserDBGetFirstGroup, (fl), 0, 0)
	#define CAL_UserDBGetFirstGroup  pfUserDBGetFirstGroup
	#define CHK_UserDBGetFirstGroup  (pfUserDBGetFirstGroup != NULL)
	#define EXP_UserDBGetFirstGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetFirstGroup", (RTS_UINTPTR)UserDBGetFirstGroup, 0, 0) 
#endif




/**
 * <description>Iteration interface to get next configured user group to which the user is assigned. Must be started with UserDBGetFirstGroup()</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="hPrevGroup" type="IN">Handle to the previous user group retrieved via UserDBGetFirstGroup() or subsequent calls of UserDBGetNextGroup()</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next user group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next user group available</errorcode>
 * <result>Handle to the next user group or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGetNextGroup(RTS_HANDLE hUser, RTS_HANDLE hPrevGroup, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGETNEXTGROUP) (RTS_HANDLE hUser, RTS_HANDLE hPrevGroup, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGETNEXTGROUP_NOTIMPLEMENTED)
	#define USE_UserDBGetNextGroup
	#define EXT_UserDBGetNextGroup
	#define GET_UserDBGetNextGroup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGetNextGroup(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGetNextGroup  FALSE
	#define EXP_UserDBGetNextGroup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGetNextGroup
	#define EXT_UserDBGetNextGroup
	#define GET_UserDBGetNextGroup(fl)  CAL_CMGETAPI( "UserDBGetNextGroup" ) 
	#define CAL_UserDBGetNextGroup  UserDBGetNextGroup
	#define CHK_UserDBGetNextGroup  TRUE
	#define EXP_UserDBGetNextGroup  CAL_CMEXPAPI( "UserDBGetNextGroup" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGetNextGroup
	#define EXT_UserDBGetNextGroup
	#define GET_UserDBGetNextGroup(fl)  CAL_CMGETAPI( "UserDBGetNextGroup" ) 
	#define CAL_UserDBGetNextGroup  UserDBGetNextGroup
	#define CHK_UserDBGetNextGroup  TRUE
	#define EXP_UserDBGetNextGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetNextGroup", (RTS_UINTPTR)UserDBGetNextGroup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGetNextGroup
	#define EXT_CmpUserDBUserDBGetNextGroup
	#define GET_CmpUserDBUserDBGetNextGroup  ERR_OK
	#define CAL_CmpUserDBUserDBGetNextGroup pICmpUserDB->IUserDBGetNextGroup
	#define CHK_CmpUserDBUserDBGetNextGroup (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGetNextGroup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGetNextGroup
	#define EXT_UserDBGetNextGroup
	#define GET_UserDBGetNextGroup(fl)  CAL_CMGETAPI( "UserDBGetNextGroup" ) 
	#define CAL_UserDBGetNextGroup pICmpUserDB->IUserDBGetNextGroup
	#define CHK_UserDBGetNextGroup (pICmpUserDB != NULL)
	#define EXP_UserDBGetNextGroup  CAL_CMEXPAPI( "UserDBGetNextGroup" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGetNextGroup  PFUSERDBGETNEXTGROUP pfUserDBGetNextGroup;
	#define EXT_UserDBGetNextGroup  extern PFUSERDBGETNEXTGROUP pfUserDBGetNextGroup;
	#define GET_UserDBGetNextGroup(fl)  s_pfCMGetAPI2( "UserDBGetNextGroup", (RTS_VOID_FCTPTR *)&pfUserDBGetNextGroup, (fl), 0, 0)
	#define CAL_UserDBGetNextGroup  pfUserDBGetNextGroup
	#define CHK_UserDBGetNextGroup  (pfUserDBGetNextGroup != NULL)
	#define EXP_UserDBGetNextGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGetNextGroup", (RTS_UINTPTR)UserDBGetNextGroup, 0, 0) 
#endif





/* --------- Group management configuration --------------------------------- */
/* This section of functions is OPTIONAL and must only be implemented, if the */
/* Users + Groups configuration in CODESYS is used!							  */

/**
 * <description>Add a new user group to the user database</description>
 * <param name="pszGroup" type="IN">Group name</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group could be added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">Group already exists</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter. pszGroup = NULL or empty</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory available to store new group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <result>Handle to the added group or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGroupAdd(char *pszGroup, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGROUPADD) (char *pszGroup, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPADD_NOTIMPLEMENTED)
	#define USE_UserDBGroupAdd
	#define EXT_UserDBGroupAdd
	#define GET_UserDBGroupAdd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupAdd(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGroupAdd  FALSE
	#define EXP_UserDBGroupAdd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupAdd
	#define EXT_UserDBGroupAdd
	#define GET_UserDBGroupAdd(fl)  CAL_CMGETAPI( "UserDBGroupAdd" ) 
	#define CAL_UserDBGroupAdd  UserDBGroupAdd
	#define CHK_UserDBGroupAdd  TRUE
	#define EXP_UserDBGroupAdd  CAL_CMEXPAPI( "UserDBGroupAdd" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupAdd
	#define EXT_UserDBGroupAdd
	#define GET_UserDBGroupAdd(fl)  CAL_CMGETAPI( "UserDBGroupAdd" ) 
	#define CAL_UserDBGroupAdd  UserDBGroupAdd
	#define CHK_UserDBGroupAdd  TRUE
	#define EXP_UserDBGroupAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupAdd", (RTS_UINTPTR)UserDBGroupAdd, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupAdd
	#define EXT_CmpUserDBUserDBGroupAdd
	#define GET_CmpUserDBUserDBGroupAdd  ERR_OK
	#define CAL_CmpUserDBUserDBGroupAdd pICmpUserDB->IUserDBGroupAdd
	#define CHK_CmpUserDBUserDBGroupAdd (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupAdd  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupAdd
	#define EXT_UserDBGroupAdd
	#define GET_UserDBGroupAdd(fl)  CAL_CMGETAPI( "UserDBGroupAdd" ) 
	#define CAL_UserDBGroupAdd pICmpUserDB->IUserDBGroupAdd
	#define CHK_UserDBGroupAdd (pICmpUserDB != NULL)
	#define EXP_UserDBGroupAdd  CAL_CMEXPAPI( "UserDBGroupAdd" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupAdd  PFUSERDBGROUPADD pfUserDBGroupAdd;
	#define EXT_UserDBGroupAdd  extern PFUSERDBGROUPADD pfUserDBGroupAdd;
	#define GET_UserDBGroupAdd(fl)  s_pfCMGetAPI2( "UserDBGroupAdd", (RTS_VOID_FCTPTR *)&pfUserDBGroupAdd, (fl), 0, 0)
	#define CAL_UserDBGroupAdd  pfUserDBGroupAdd
	#define CHK_UserDBGroupAdd  (pfUserDBGroupAdd != NULL)
	#define EXP_UserDBGroupAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupAdd", (RTS_UINTPTR)UserDBGroupAdd, 0, 0) 
#endif




/**
 * <description>Remove user group from the user database</description>
 * <param name="hGroup" type="IN">Handle to the group</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group could be removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid group handle</errorcode>
 */
RTS_RESULT CDECL UserDBGroupRemove(RTS_HANDLE hGroup);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPREMOVE) (RTS_HANDLE hGroup);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPREMOVE_NOTIMPLEMENTED)
	#define USE_UserDBGroupRemove
	#define EXT_UserDBGroupRemove
	#define GET_UserDBGroupRemove(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupRemove(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupRemove  FALSE
	#define EXP_UserDBGroupRemove  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupRemove
	#define EXT_UserDBGroupRemove
	#define GET_UserDBGroupRemove(fl)  CAL_CMGETAPI( "UserDBGroupRemove" ) 
	#define CAL_UserDBGroupRemove  UserDBGroupRemove
	#define CHK_UserDBGroupRemove  TRUE
	#define EXP_UserDBGroupRemove  CAL_CMEXPAPI( "UserDBGroupRemove" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupRemove
	#define EXT_UserDBGroupRemove
	#define GET_UserDBGroupRemove(fl)  CAL_CMGETAPI( "UserDBGroupRemove" ) 
	#define CAL_UserDBGroupRemove  UserDBGroupRemove
	#define CHK_UserDBGroupRemove  TRUE
	#define EXP_UserDBGroupRemove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupRemove", (RTS_UINTPTR)UserDBGroupRemove, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupRemove
	#define EXT_CmpUserDBUserDBGroupRemove
	#define GET_CmpUserDBUserDBGroupRemove  ERR_OK
	#define CAL_CmpUserDBUserDBGroupRemove pICmpUserDB->IUserDBGroupRemove
	#define CHK_CmpUserDBUserDBGroupRemove (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupRemove  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupRemove
	#define EXT_UserDBGroupRemove
	#define GET_UserDBGroupRemove(fl)  CAL_CMGETAPI( "UserDBGroupRemove" ) 
	#define CAL_UserDBGroupRemove pICmpUserDB->IUserDBGroupRemove
	#define CHK_UserDBGroupRemove (pICmpUserDB != NULL)
	#define EXP_UserDBGroupRemove  CAL_CMEXPAPI( "UserDBGroupRemove" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupRemove  PFUSERDBGROUPREMOVE pfUserDBGroupRemove;
	#define EXT_UserDBGroupRemove  extern PFUSERDBGROUPREMOVE pfUserDBGroupRemove;
	#define GET_UserDBGroupRemove(fl)  s_pfCMGetAPI2( "UserDBGroupRemove", (RTS_VOID_FCTPTR *)&pfUserDBGroupRemove, (fl), 0, 0)
	#define CAL_UserDBGroupRemove  pfUserDBGroupRemove
	#define CHK_UserDBGroupRemove  (pfUserDBGroupRemove != NULL)
	#define EXP_UserDBGroupRemove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupRemove", (RTS_UINTPTR)UserDBGroupRemove, 0, 0) 
#endif




/**
 * <description>Add a new user to a group</description>
 * <param name="hGroup" type="IN">Handle to the group</param>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be added or is already added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter. hGroup or hUser invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory available to add user to group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 */
RTS_RESULT CDECL UserDBGroupAddUser(RTS_HANDLE hGroup, RTS_HANDLE hUser);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPADDUSER) (RTS_HANDLE hGroup, RTS_HANDLE hUser);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPADDUSER_NOTIMPLEMENTED)
	#define USE_UserDBGroupAddUser
	#define EXT_UserDBGroupAddUser
	#define GET_UserDBGroupAddUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupAddUser(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupAddUser  FALSE
	#define EXP_UserDBGroupAddUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupAddUser
	#define EXT_UserDBGroupAddUser
	#define GET_UserDBGroupAddUser(fl)  CAL_CMGETAPI( "UserDBGroupAddUser" ) 
	#define CAL_UserDBGroupAddUser  UserDBGroupAddUser
	#define CHK_UserDBGroupAddUser  TRUE
	#define EXP_UserDBGroupAddUser  CAL_CMEXPAPI( "UserDBGroupAddUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupAddUser
	#define EXT_UserDBGroupAddUser
	#define GET_UserDBGroupAddUser(fl)  CAL_CMGETAPI( "UserDBGroupAddUser" ) 
	#define CAL_UserDBGroupAddUser  UserDBGroupAddUser
	#define CHK_UserDBGroupAddUser  TRUE
	#define EXP_UserDBGroupAddUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupAddUser", (RTS_UINTPTR)UserDBGroupAddUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupAddUser
	#define EXT_CmpUserDBUserDBGroupAddUser
	#define GET_CmpUserDBUserDBGroupAddUser  ERR_OK
	#define CAL_CmpUserDBUserDBGroupAddUser pICmpUserDB->IUserDBGroupAddUser
	#define CHK_CmpUserDBUserDBGroupAddUser (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupAddUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupAddUser
	#define EXT_UserDBGroupAddUser
	#define GET_UserDBGroupAddUser(fl)  CAL_CMGETAPI( "UserDBGroupAddUser" ) 
	#define CAL_UserDBGroupAddUser pICmpUserDB->IUserDBGroupAddUser
	#define CHK_UserDBGroupAddUser (pICmpUserDB != NULL)
	#define EXP_UserDBGroupAddUser  CAL_CMEXPAPI( "UserDBGroupAddUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupAddUser  PFUSERDBGROUPADDUSER pfUserDBGroupAddUser;
	#define EXT_UserDBGroupAddUser  extern PFUSERDBGROUPADDUSER pfUserDBGroupAddUser;
	#define GET_UserDBGroupAddUser(fl)  s_pfCMGetAPI2( "UserDBGroupAddUser", (RTS_VOID_FCTPTR *)&pfUserDBGroupAddUser, (fl), 0, 0)
	#define CAL_UserDBGroupAddUser  pfUserDBGroupAddUser
	#define CHK_UserDBGroupAddUser  (pfUserDBGroupAddUser != NULL)
	#define EXP_UserDBGroupAddUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupAddUser", (RTS_UINTPTR)UserDBGroupAddUser, 0, 0) 
#endif




/**
 * <description>Remove a user from a group</description>
 * <param name="hGroup" type="IN">Handle to the group</param>
 * <param name="hUser" type="IN">Handle to the user</param>
  * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter. hGroup or hUser invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 */
RTS_RESULT CDECL UserDBGroupRemoveUser(RTS_HANDLE hGroup, RTS_HANDLE hUser);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPREMOVEUSER) (RTS_HANDLE hGroup, RTS_HANDLE hUser);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPREMOVEUSER_NOTIMPLEMENTED)
	#define USE_UserDBGroupRemoveUser
	#define EXT_UserDBGroupRemoveUser
	#define GET_UserDBGroupRemoveUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupRemoveUser(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupRemoveUser  FALSE
	#define EXP_UserDBGroupRemoveUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupRemoveUser
	#define EXT_UserDBGroupRemoveUser
	#define GET_UserDBGroupRemoveUser(fl)  CAL_CMGETAPI( "UserDBGroupRemoveUser" ) 
	#define CAL_UserDBGroupRemoveUser  UserDBGroupRemoveUser
	#define CHK_UserDBGroupRemoveUser  TRUE
	#define EXP_UserDBGroupRemoveUser  CAL_CMEXPAPI( "UserDBGroupRemoveUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupRemoveUser
	#define EXT_UserDBGroupRemoveUser
	#define GET_UserDBGroupRemoveUser(fl)  CAL_CMGETAPI( "UserDBGroupRemoveUser" ) 
	#define CAL_UserDBGroupRemoveUser  UserDBGroupRemoveUser
	#define CHK_UserDBGroupRemoveUser  TRUE
	#define EXP_UserDBGroupRemoveUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupRemoveUser", (RTS_UINTPTR)UserDBGroupRemoveUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupRemoveUser
	#define EXT_CmpUserDBUserDBGroupRemoveUser
	#define GET_CmpUserDBUserDBGroupRemoveUser  ERR_OK
	#define CAL_CmpUserDBUserDBGroupRemoveUser pICmpUserDB->IUserDBGroupRemoveUser
	#define CHK_CmpUserDBUserDBGroupRemoveUser (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupRemoveUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupRemoveUser
	#define EXT_UserDBGroupRemoveUser
	#define GET_UserDBGroupRemoveUser(fl)  CAL_CMGETAPI( "UserDBGroupRemoveUser" ) 
	#define CAL_UserDBGroupRemoveUser pICmpUserDB->IUserDBGroupRemoveUser
	#define CHK_UserDBGroupRemoveUser (pICmpUserDB != NULL)
	#define EXP_UserDBGroupRemoveUser  CAL_CMEXPAPI( "UserDBGroupRemoveUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupRemoveUser  PFUSERDBGROUPREMOVEUSER pfUserDBGroupRemoveUser;
	#define EXT_UserDBGroupRemoveUser  extern PFUSERDBGROUPREMOVEUSER pfUserDBGroupRemoveUser;
	#define GET_UserDBGroupRemoveUser(fl)  s_pfCMGetAPI2( "UserDBGroupRemoveUser", (RTS_VOID_FCTPTR *)&pfUserDBGroupRemoveUser, (fl), 0, 0)
	#define CAL_UserDBGroupRemoveUser  pfUserDBGroupRemoveUser
	#define CHK_UserDBGroupRemoveUser  (pfUserDBGroupRemoveUser != NULL)
	#define EXP_UserDBGroupRemoveUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupRemoveUser", (RTS_UINTPTR)UserDBGroupRemoveUser, 0, 0) 
#endif




/**
 * <description>Set properties of a group</description>
 * <param name="hGroup" type="IN">Handle to the group</param>
 * <param name="pulProperty" type="IN">Pointer to set the properties. For details see category "UserDB properties"</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Properties could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user group or property pointer = NULL</errorcode>
 */
RTS_RESULT CDECL UserDBGroupSetProperty(RTS_HANDLE hGroup, RTS_UI32 ulProperty);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPSETPROPERTY) (RTS_HANDLE hGroup, RTS_UI32 ulProperty);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPSETPROPERTY_NOTIMPLEMENTED)
	#define USE_UserDBGroupSetProperty
	#define EXT_UserDBGroupSetProperty
	#define GET_UserDBGroupSetProperty(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupSetProperty(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupSetProperty  FALSE
	#define EXP_UserDBGroupSetProperty  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupSetProperty
	#define EXT_UserDBGroupSetProperty
	#define GET_UserDBGroupSetProperty(fl)  CAL_CMGETAPI( "UserDBGroupSetProperty" ) 
	#define CAL_UserDBGroupSetProperty  UserDBGroupSetProperty
	#define CHK_UserDBGroupSetProperty  TRUE
	#define EXP_UserDBGroupSetProperty  CAL_CMEXPAPI( "UserDBGroupSetProperty" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupSetProperty
	#define EXT_UserDBGroupSetProperty
	#define GET_UserDBGroupSetProperty(fl)  CAL_CMGETAPI( "UserDBGroupSetProperty" ) 
	#define CAL_UserDBGroupSetProperty  UserDBGroupSetProperty
	#define CHK_UserDBGroupSetProperty  TRUE
	#define EXP_UserDBGroupSetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupSetProperty", (RTS_UINTPTR)UserDBGroupSetProperty, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupSetProperty
	#define EXT_CmpUserDBUserDBGroupSetProperty
	#define GET_CmpUserDBUserDBGroupSetProperty  ERR_OK
	#define CAL_CmpUserDBUserDBGroupSetProperty pICmpUserDB->IUserDBGroupSetProperty
	#define CHK_CmpUserDBUserDBGroupSetProperty (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupSetProperty  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupSetProperty
	#define EXT_UserDBGroupSetProperty
	#define GET_UserDBGroupSetProperty(fl)  CAL_CMGETAPI( "UserDBGroupSetProperty" ) 
	#define CAL_UserDBGroupSetProperty pICmpUserDB->IUserDBGroupSetProperty
	#define CHK_UserDBGroupSetProperty (pICmpUserDB != NULL)
	#define EXP_UserDBGroupSetProperty  CAL_CMEXPAPI( "UserDBGroupSetProperty" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupSetProperty  PFUSERDBGROUPSETPROPERTY pfUserDBGroupSetProperty;
	#define EXT_UserDBGroupSetProperty  extern PFUSERDBGROUPSETPROPERTY pfUserDBGroupSetProperty;
	#define GET_UserDBGroupSetProperty(fl)  s_pfCMGetAPI2( "UserDBGroupSetProperty", (RTS_VOID_FCTPTR *)&pfUserDBGroupSetProperty, (fl), 0, 0)
	#define CAL_UserDBGroupSetProperty  pfUserDBGroupSetProperty
	#define CHK_UserDBGroupSetProperty  (pfUserDBGroupSetProperty != NULL)
	#define EXP_UserDBGroupSetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupSetProperty", (RTS_UINTPTR)UserDBGroupSetProperty, 0, 0) 
#endif




/**
 * <description>Add a group as a member to another group. So that group is derived from the first one</description>
 * <param name="hGroup" type="IN">First group to derive from</param>
 * <param name="hGroupMember" type="IN">New member group which is derived from the first group (hGroup) after this operation</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group could be added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">Group is already added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter. hGroup or hGroupMember invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory available to add group to another group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 */
RTS_RESULT CDECL UserDBGroupAddMember(RTS_HANDLE hGroup, RTS_HANDLE hGroupMember);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPADDMEMBER) (RTS_HANDLE hGroup, RTS_HANDLE hGroupMember);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPADDMEMBER_NOTIMPLEMENTED)
	#define USE_UserDBGroupAddMember
	#define EXT_UserDBGroupAddMember
	#define GET_UserDBGroupAddMember(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupAddMember(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupAddMember  FALSE
	#define EXP_UserDBGroupAddMember  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupAddMember
	#define EXT_UserDBGroupAddMember
	#define GET_UserDBGroupAddMember(fl)  CAL_CMGETAPI( "UserDBGroupAddMember" ) 
	#define CAL_UserDBGroupAddMember  UserDBGroupAddMember
	#define CHK_UserDBGroupAddMember  TRUE
	#define EXP_UserDBGroupAddMember  CAL_CMEXPAPI( "UserDBGroupAddMember" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupAddMember
	#define EXT_UserDBGroupAddMember
	#define GET_UserDBGroupAddMember(fl)  CAL_CMGETAPI( "UserDBGroupAddMember" ) 
	#define CAL_UserDBGroupAddMember  UserDBGroupAddMember
	#define CHK_UserDBGroupAddMember  TRUE
	#define EXP_UserDBGroupAddMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupAddMember", (RTS_UINTPTR)UserDBGroupAddMember, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupAddMember
	#define EXT_CmpUserDBUserDBGroupAddMember
	#define GET_CmpUserDBUserDBGroupAddMember  ERR_OK
	#define CAL_CmpUserDBUserDBGroupAddMember pICmpUserDB->IUserDBGroupAddMember
	#define CHK_CmpUserDBUserDBGroupAddMember (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupAddMember  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupAddMember
	#define EXT_UserDBGroupAddMember
	#define GET_UserDBGroupAddMember(fl)  CAL_CMGETAPI( "UserDBGroupAddMember" ) 
	#define CAL_UserDBGroupAddMember pICmpUserDB->IUserDBGroupAddMember
	#define CHK_UserDBGroupAddMember (pICmpUserDB != NULL)
	#define EXP_UserDBGroupAddMember  CAL_CMEXPAPI( "UserDBGroupAddMember" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupAddMember  PFUSERDBGROUPADDMEMBER pfUserDBGroupAddMember;
	#define EXT_UserDBGroupAddMember  extern PFUSERDBGROUPADDMEMBER pfUserDBGroupAddMember;
	#define GET_UserDBGroupAddMember(fl)  s_pfCMGetAPI2( "UserDBGroupAddMember", (RTS_VOID_FCTPTR *)&pfUserDBGroupAddMember, (fl), 0, 0)
	#define CAL_UserDBGroupAddMember  pfUserDBGroupAddMember
	#define CHK_UserDBGroupAddMember  (pfUserDBGroupAddMember != NULL)
	#define EXP_UserDBGroupAddMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupAddMember", (RTS_UINTPTR)UserDBGroupAddMember, 0, 0) 
#endif




/**
 * <description>Remove a group as a member from another group. So the group is no longer derived from the first one</description>
 * <param name="hGroup" type="IN">First group to remove the derived group from</param>
 * <param name="hGroupMember" type="IN">Member group which is no longer derived from the first group (hGroup) after this operation</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group could be removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter. hGroup or hGroupMember invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 */
 RTS_RESULT CDECL UserDBGroupRemoveMember(RTS_HANDLE hGroup, RTS_HANDLE hGroupMember);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPREMOVEMEMBER) (RTS_HANDLE hGroup, RTS_HANDLE hGroupMember);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPREMOVEMEMBER_NOTIMPLEMENTED)
	#define USE_UserDBGroupRemoveMember
	#define EXT_UserDBGroupRemoveMember
	#define GET_UserDBGroupRemoveMember(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupRemoveMember(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupRemoveMember  FALSE
	#define EXP_UserDBGroupRemoveMember  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupRemoveMember
	#define EXT_UserDBGroupRemoveMember
	#define GET_UserDBGroupRemoveMember(fl)  CAL_CMGETAPI( "UserDBGroupRemoveMember" ) 
	#define CAL_UserDBGroupRemoveMember  UserDBGroupRemoveMember
	#define CHK_UserDBGroupRemoveMember  TRUE
	#define EXP_UserDBGroupRemoveMember  CAL_CMEXPAPI( "UserDBGroupRemoveMember" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupRemoveMember
	#define EXT_UserDBGroupRemoveMember
	#define GET_UserDBGroupRemoveMember(fl)  CAL_CMGETAPI( "UserDBGroupRemoveMember" ) 
	#define CAL_UserDBGroupRemoveMember  UserDBGroupRemoveMember
	#define CHK_UserDBGroupRemoveMember  TRUE
	#define EXP_UserDBGroupRemoveMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupRemoveMember", (RTS_UINTPTR)UserDBGroupRemoveMember, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupRemoveMember
	#define EXT_CmpUserDBUserDBGroupRemoveMember
	#define GET_CmpUserDBUserDBGroupRemoveMember  ERR_OK
	#define CAL_CmpUserDBUserDBGroupRemoveMember pICmpUserDB->IUserDBGroupRemoveMember
	#define CHK_CmpUserDBUserDBGroupRemoveMember (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupRemoveMember  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupRemoveMember
	#define EXT_UserDBGroupRemoveMember
	#define GET_UserDBGroupRemoveMember(fl)  CAL_CMGETAPI( "UserDBGroupRemoveMember" ) 
	#define CAL_UserDBGroupRemoveMember pICmpUserDB->IUserDBGroupRemoveMember
	#define CHK_UserDBGroupRemoveMember (pICmpUserDB != NULL)
	#define EXP_UserDBGroupRemoveMember  CAL_CMEXPAPI( "UserDBGroupRemoveMember" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupRemoveMember  PFUSERDBGROUPREMOVEMEMBER pfUserDBGroupRemoveMember;
	#define EXT_UserDBGroupRemoveMember  extern PFUSERDBGROUPREMOVEMEMBER pfUserDBGroupRemoveMember;
	#define GET_UserDBGroupRemoveMember(fl)  s_pfCMGetAPI2( "UserDBGroupRemoveMember", (RTS_VOID_FCTPTR *)&pfUserDBGroupRemoveMember, (fl), 0, 0)
	#define CAL_UserDBGroupRemoveMember  pfUserDBGroupRemoveMember
	#define CHK_UserDBGroupRemoveMember  (pfUserDBGroupRemoveMember != NULL)
	#define EXP_UserDBGroupRemoveMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupRemoveMember", (RTS_UINTPTR)UserDBGroupRemoveMember, 0, 0) 
#endif




/**
 * <description>Explicitly enforce the usage of user authentication: no anonymous login is possible, login with user and password is required
 * NOTE: The UserDB is enforced implicitly when a DB file is loaded or configured via CODESYS.
 * </description>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User and password login is required</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Anonymous login is possible</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 */
 RTS_RESULT CDECL UserDBEnforce(void);
typedef RTS_RESULT (CDECL * PFUSERDBENFORCE) (void);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBENFORCE_NOTIMPLEMENTED)
	#define USE_UserDBEnforce
	#define EXT_UserDBEnforce
	#define GET_UserDBEnforce(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBEnforce()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBEnforce  FALSE
	#define EXP_UserDBEnforce  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBEnforce
	#define EXT_UserDBEnforce
	#define GET_UserDBEnforce(fl)  CAL_CMGETAPI( "UserDBEnforce" ) 
	#define CAL_UserDBEnforce  UserDBEnforce
	#define CHK_UserDBEnforce  TRUE
	#define EXP_UserDBEnforce  CAL_CMEXPAPI( "UserDBEnforce" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBEnforce
	#define EXT_UserDBEnforce
	#define GET_UserDBEnforce(fl)  CAL_CMGETAPI( "UserDBEnforce" ) 
	#define CAL_UserDBEnforce  UserDBEnforce
	#define CHK_UserDBEnforce  TRUE
	#define EXP_UserDBEnforce  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBEnforce", (RTS_UINTPTR)UserDBEnforce, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBEnforce
	#define EXT_CmpUserDBUserDBEnforce
	#define GET_CmpUserDBUserDBEnforce  ERR_OK
	#define CAL_CmpUserDBUserDBEnforce pICmpUserDB->IUserDBEnforce
	#define CHK_CmpUserDBUserDBEnforce (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBEnforce  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBEnforce
	#define EXT_UserDBEnforce
	#define GET_UserDBEnforce(fl)  CAL_CMGETAPI( "UserDBEnforce" ) 
	#define CAL_UserDBEnforce pICmpUserDB->IUserDBEnforce
	#define CHK_UserDBEnforce (pICmpUserDB != NULL)
	#define EXP_UserDBEnforce  CAL_CMEXPAPI( "UserDBEnforce" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBEnforce  PFUSERDBENFORCE pfUserDBEnforce;
	#define EXT_UserDBEnforce  extern PFUSERDBENFORCE pfUserDBEnforce;
	#define GET_UserDBEnforce(fl)  s_pfCMGetAPI2( "UserDBEnforce", (RTS_VOID_FCTPTR *)&pfUserDBEnforce, (fl), 0, 0)
	#define CAL_UserDBEnforce  pfUserDBEnforce
	#define CHK_UserDBEnforce  (pfUserDBEnforce != NULL)
	#define EXP_UserDBEnforce  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBEnforce", (RTS_UINTPTR)UserDBEnforce, 0, 0) 
#endif




/* --------- Group management ------------------------------------------------ */
/* This section of functions is MANDATORY for the user management!             */

/* Group iteration */

/**
 * <description>Iteration interface to get the first configured user group</description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First user group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No user group configured</errorcode>
 * <result>Handle to the first user group or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGroupGetFirst(RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGROUPGETFIRST) (RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPGETFIRST_NOTIMPLEMENTED)
	#define USE_UserDBGroupGetFirst
	#define EXT_UserDBGroupGetFirst
	#define GET_UserDBGroupGetFirst(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupGetFirst(p0)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGroupGetFirst  FALSE
	#define EXP_UserDBGroupGetFirst  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupGetFirst
	#define EXT_UserDBGroupGetFirst
	#define GET_UserDBGroupGetFirst(fl)  CAL_CMGETAPI( "UserDBGroupGetFirst" ) 
	#define CAL_UserDBGroupGetFirst  UserDBGroupGetFirst
	#define CHK_UserDBGroupGetFirst  TRUE
	#define EXP_UserDBGroupGetFirst  CAL_CMEXPAPI( "UserDBGroupGetFirst" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupGetFirst
	#define EXT_UserDBGroupGetFirst
	#define GET_UserDBGroupGetFirst(fl)  CAL_CMGETAPI( "UserDBGroupGetFirst" ) 
	#define CAL_UserDBGroupGetFirst  UserDBGroupGetFirst
	#define CHK_UserDBGroupGetFirst  TRUE
	#define EXP_UserDBGroupGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetFirst", (RTS_UINTPTR)UserDBGroupGetFirst, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupGetFirst
	#define EXT_CmpUserDBUserDBGroupGetFirst
	#define GET_CmpUserDBUserDBGroupGetFirst  ERR_OK
	#define CAL_CmpUserDBUserDBGroupGetFirst pICmpUserDB->IUserDBGroupGetFirst
	#define CHK_CmpUserDBUserDBGroupGetFirst (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupGetFirst  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupGetFirst
	#define EXT_UserDBGroupGetFirst
	#define GET_UserDBGroupGetFirst(fl)  CAL_CMGETAPI( "UserDBGroupGetFirst" ) 
	#define CAL_UserDBGroupGetFirst pICmpUserDB->IUserDBGroupGetFirst
	#define CHK_UserDBGroupGetFirst (pICmpUserDB != NULL)
	#define EXP_UserDBGroupGetFirst  CAL_CMEXPAPI( "UserDBGroupGetFirst" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupGetFirst  PFUSERDBGROUPGETFIRST pfUserDBGroupGetFirst;
	#define EXT_UserDBGroupGetFirst  extern PFUSERDBGROUPGETFIRST pfUserDBGroupGetFirst;
	#define GET_UserDBGroupGetFirst(fl)  s_pfCMGetAPI2( "UserDBGroupGetFirst", (RTS_VOID_FCTPTR *)&pfUserDBGroupGetFirst, (fl), 0, 0)
	#define CAL_UserDBGroupGetFirst  pfUserDBGroupGetFirst
	#define CHK_UserDBGroupGetFirst  (pfUserDBGroupGetFirst != NULL)
	#define EXP_UserDBGroupGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetFirst", (RTS_UINTPTR)UserDBGroupGetFirst, 0, 0) 
#endif




/**
 * <description>Iteration interface to get next configured user group. Must be started with UserDBGroupGetFirst()</description>
 * <param name="hPrevGroup" type="IN">Handle to the previous user group retrieved via UserDBGroupGetFirst() or subsequent calls of UserDBGroupGetNext()</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next user group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next user group available</errorcode>
 * <result>Handle to the next user group or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGroupGetNext(RTS_HANDLE hPrevGroup, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGROUPGETNEXT) (RTS_HANDLE hPrevGroup, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPGETNEXT_NOTIMPLEMENTED)
	#define USE_UserDBGroupGetNext
	#define EXT_UserDBGroupGetNext
	#define GET_UserDBGroupGetNext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupGetNext(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGroupGetNext  FALSE
	#define EXP_UserDBGroupGetNext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupGetNext
	#define EXT_UserDBGroupGetNext
	#define GET_UserDBGroupGetNext(fl)  CAL_CMGETAPI( "UserDBGroupGetNext" ) 
	#define CAL_UserDBGroupGetNext  UserDBGroupGetNext
	#define CHK_UserDBGroupGetNext  TRUE
	#define EXP_UserDBGroupGetNext  CAL_CMEXPAPI( "UserDBGroupGetNext" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupGetNext
	#define EXT_UserDBGroupGetNext
	#define GET_UserDBGroupGetNext(fl)  CAL_CMGETAPI( "UserDBGroupGetNext" ) 
	#define CAL_UserDBGroupGetNext  UserDBGroupGetNext
	#define CHK_UserDBGroupGetNext  TRUE
	#define EXP_UserDBGroupGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetNext", (RTS_UINTPTR)UserDBGroupGetNext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupGetNext
	#define EXT_CmpUserDBUserDBGroupGetNext
	#define GET_CmpUserDBUserDBGroupGetNext  ERR_OK
	#define CAL_CmpUserDBUserDBGroupGetNext pICmpUserDB->IUserDBGroupGetNext
	#define CHK_CmpUserDBUserDBGroupGetNext (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupGetNext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupGetNext
	#define EXT_UserDBGroupGetNext
	#define GET_UserDBGroupGetNext(fl)  CAL_CMGETAPI( "UserDBGroupGetNext" ) 
	#define CAL_UserDBGroupGetNext pICmpUserDB->IUserDBGroupGetNext
	#define CHK_UserDBGroupGetNext (pICmpUserDB != NULL)
	#define EXP_UserDBGroupGetNext  CAL_CMEXPAPI( "UserDBGroupGetNext" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupGetNext  PFUSERDBGROUPGETNEXT pfUserDBGroupGetNext;
	#define EXT_UserDBGroupGetNext  extern PFUSERDBGROUPGETNEXT pfUserDBGroupGetNext;
	#define GET_UserDBGroupGetNext(fl)  s_pfCMGetAPI2( "UserDBGroupGetNext", (RTS_VOID_FCTPTR *)&pfUserDBGroupGetNext, (fl), 0, 0)
	#define CAL_UserDBGroupGetNext  pfUserDBGroupGetNext
	#define CHK_UserDBGroupGetNext  (pfUserDBGroupGetNext != NULL)
	#define EXP_UserDBGroupGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetNext", (RTS_UINTPTR)UserDBGroupGetNext, 0, 0) 
#endif




/* Single group access */

/**
 * <description>Open user group specified by its name</description>
 * <param name="pszGroup" type="IN">User group name</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User group could be opened</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter. pszGroup = NULL or empty</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <result>Handle to the user group or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGroupOpen(char *pszGroup, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGROUPOPEN) (char *pszGroup, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPOPEN_NOTIMPLEMENTED)
	#define USE_UserDBGroupOpen
	#define EXT_UserDBGroupOpen
	#define GET_UserDBGroupOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupOpen(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGroupOpen  FALSE
	#define EXP_UserDBGroupOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupOpen
	#define EXT_UserDBGroupOpen
	#define GET_UserDBGroupOpen(fl)  CAL_CMGETAPI( "UserDBGroupOpen" ) 
	#define CAL_UserDBGroupOpen  UserDBGroupOpen
	#define CHK_UserDBGroupOpen  TRUE
	#define EXP_UserDBGroupOpen  CAL_CMEXPAPI( "UserDBGroupOpen" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupOpen
	#define EXT_UserDBGroupOpen
	#define GET_UserDBGroupOpen(fl)  CAL_CMGETAPI( "UserDBGroupOpen" ) 
	#define CAL_UserDBGroupOpen  UserDBGroupOpen
	#define CHK_UserDBGroupOpen  TRUE
	#define EXP_UserDBGroupOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupOpen", (RTS_UINTPTR)UserDBGroupOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupOpen
	#define EXT_CmpUserDBUserDBGroupOpen
	#define GET_CmpUserDBUserDBGroupOpen  ERR_OK
	#define CAL_CmpUserDBUserDBGroupOpen pICmpUserDB->IUserDBGroupOpen
	#define CHK_CmpUserDBUserDBGroupOpen (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupOpen
	#define EXT_UserDBGroupOpen
	#define GET_UserDBGroupOpen(fl)  CAL_CMGETAPI( "UserDBGroupOpen" ) 
	#define CAL_UserDBGroupOpen pICmpUserDB->IUserDBGroupOpen
	#define CHK_UserDBGroupOpen (pICmpUserDB != NULL)
	#define EXP_UserDBGroupOpen  CAL_CMEXPAPI( "UserDBGroupOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupOpen  PFUSERDBGROUPOPEN pfUserDBGroupOpen;
	#define EXT_UserDBGroupOpen  extern PFUSERDBGROUPOPEN pfUserDBGroupOpen;
	#define GET_UserDBGroupOpen(fl)  s_pfCMGetAPI2( "UserDBGroupOpen", (RTS_VOID_FCTPTR *)&pfUserDBGroupOpen, (fl), 0, 0)
	#define CAL_UserDBGroupOpen  pfUserDBGroupOpen
	#define CHK_UserDBGroupOpen  (pfUserDBGroupOpen != NULL)
	#define EXP_UserDBGroupOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupOpen", (RTS_UINTPTR)UserDBGroupOpen, 0, 0) 
#endif




/**
 * <description>Close a user group</description>
 * <param name="hGroup" type="IN">Handle to the user group</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User group could be closed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user group handle</errorcode>
 */
RTS_RESULT CDECL UserDBGroupClose(RTS_HANDLE hGroup);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPCLOSE) (RTS_HANDLE hGroup);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPCLOSE_NOTIMPLEMENTED)
	#define USE_UserDBGroupClose
	#define EXT_UserDBGroupClose
	#define GET_UserDBGroupClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupClose  FALSE
	#define EXP_UserDBGroupClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupClose
	#define EXT_UserDBGroupClose
	#define GET_UserDBGroupClose(fl)  CAL_CMGETAPI( "UserDBGroupClose" ) 
	#define CAL_UserDBGroupClose  UserDBGroupClose
	#define CHK_UserDBGroupClose  TRUE
	#define EXP_UserDBGroupClose  CAL_CMEXPAPI( "UserDBGroupClose" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupClose
	#define EXT_UserDBGroupClose
	#define GET_UserDBGroupClose(fl)  CAL_CMGETAPI( "UserDBGroupClose" ) 
	#define CAL_UserDBGroupClose  UserDBGroupClose
	#define CHK_UserDBGroupClose  TRUE
	#define EXP_UserDBGroupClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupClose", (RTS_UINTPTR)UserDBGroupClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupClose
	#define EXT_CmpUserDBUserDBGroupClose
	#define GET_CmpUserDBUserDBGroupClose  ERR_OK
	#define CAL_CmpUserDBUserDBGroupClose pICmpUserDB->IUserDBGroupClose
	#define CHK_CmpUserDBUserDBGroupClose (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupClose
	#define EXT_UserDBGroupClose
	#define GET_UserDBGroupClose(fl)  CAL_CMGETAPI( "UserDBGroupClose" ) 
	#define CAL_UserDBGroupClose pICmpUserDB->IUserDBGroupClose
	#define CHK_UserDBGroupClose (pICmpUserDB != NULL)
	#define EXP_UserDBGroupClose  CAL_CMEXPAPI( "UserDBGroupClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupClose  PFUSERDBGROUPCLOSE pfUserDBGroupClose;
	#define EXT_UserDBGroupClose  extern PFUSERDBGROUPCLOSE pfUserDBGroupClose;
	#define GET_UserDBGroupClose(fl)  s_pfCMGetAPI2( "UserDBGroupClose", (RTS_VOID_FCTPTR *)&pfUserDBGroupClose, (fl), 0, 0)
	#define CAL_UserDBGroupClose  pfUserDBGroupClose
	#define CHK_UserDBGroupClose  (pfUserDBGroupClose != NULL)
	#define EXP_UserDBGroupClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupClose", (RTS_UINTPTR)UserDBGroupClose, 0, 0) 
#endif




/**
 * <description>Get user group name</description>
 * <param name="hGroup" type="IN">Handle to the user group</param>
 * <param name="pszGroup" type="OUT">Pointer to get user group name</param>
 * <param name="piMaxLen" type="INOUT">Pointer to length of buffer in pszGroup. If pszGroup = NULL, only the length of the user group name is returned in *piMaxLen</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User group name is returned</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user group handle or pszGroup = NULL and piMaxLen = NULL</errorcode>
 */
RTS_RESULT CDECL UserDBGroupGetName(RTS_HANDLE hGroup, char *pszGroup, int *piMaxLen);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPGETNAME) (RTS_HANDLE hGroup, char *pszGroup, int *piMaxLen);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPGETNAME_NOTIMPLEMENTED)
	#define USE_UserDBGroupGetName
	#define EXT_UserDBGroupGetName
	#define GET_UserDBGroupGetName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupGetName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupGetName  FALSE
	#define EXP_UserDBGroupGetName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupGetName
	#define EXT_UserDBGroupGetName
	#define GET_UserDBGroupGetName(fl)  CAL_CMGETAPI( "UserDBGroupGetName" ) 
	#define CAL_UserDBGroupGetName  UserDBGroupGetName
	#define CHK_UserDBGroupGetName  TRUE
	#define EXP_UserDBGroupGetName  CAL_CMEXPAPI( "UserDBGroupGetName" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupGetName
	#define EXT_UserDBGroupGetName
	#define GET_UserDBGroupGetName(fl)  CAL_CMGETAPI( "UserDBGroupGetName" ) 
	#define CAL_UserDBGroupGetName  UserDBGroupGetName
	#define CHK_UserDBGroupGetName  TRUE
	#define EXP_UserDBGroupGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetName", (RTS_UINTPTR)UserDBGroupGetName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupGetName
	#define EXT_CmpUserDBUserDBGroupGetName
	#define GET_CmpUserDBUserDBGroupGetName  ERR_OK
	#define CAL_CmpUserDBUserDBGroupGetName pICmpUserDB->IUserDBGroupGetName
	#define CHK_CmpUserDBUserDBGroupGetName (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupGetName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupGetName
	#define EXT_UserDBGroupGetName
	#define GET_UserDBGroupGetName(fl)  CAL_CMGETAPI( "UserDBGroupGetName" ) 
	#define CAL_UserDBGroupGetName pICmpUserDB->IUserDBGroupGetName
	#define CHK_UserDBGroupGetName (pICmpUserDB != NULL)
	#define EXP_UserDBGroupGetName  CAL_CMEXPAPI( "UserDBGroupGetName" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupGetName  PFUSERDBGROUPGETNAME pfUserDBGroupGetName;
	#define EXT_UserDBGroupGetName  extern PFUSERDBGROUPGETNAME pfUserDBGroupGetName;
	#define GET_UserDBGroupGetName(fl)  s_pfCMGetAPI2( "UserDBGroupGetName", (RTS_VOID_FCTPTR *)&pfUserDBGroupGetName, (fl), 0, 0)
	#define CAL_UserDBGroupGetName  pfUserDBGroupGetName
	#define CHK_UserDBGroupGetName  (pfUserDBGroupGetName != NULL)
	#define EXP_UserDBGroupGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetName", (RTS_UINTPTR)UserDBGroupGetName, 0, 0) 
#endif




/**
 * <description>Get properties of a group</description>
 * <param name="hGroup" type="IN">Handle to the group</param>
 * <param name="pulProperty" type="OUT">Pointer to return the properties. For details see category "UserDB properties"</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Properties could be retrieved</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user group or property pointer = NULL</errorcode>
 */
RTS_RESULT CDECL UserDBGroupGetProperty(RTS_HANDLE hGroup, RTS_UI32 *pulProperty);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPGETPROPERTY) (RTS_HANDLE hGroup, RTS_UI32 *pulProperty);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPGETPROPERTY_NOTIMPLEMENTED)
	#define USE_UserDBGroupGetProperty
	#define EXT_UserDBGroupGetProperty
	#define GET_UserDBGroupGetProperty(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupGetProperty(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupGetProperty  FALSE
	#define EXP_UserDBGroupGetProperty  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupGetProperty
	#define EXT_UserDBGroupGetProperty
	#define GET_UserDBGroupGetProperty(fl)  CAL_CMGETAPI( "UserDBGroupGetProperty" ) 
	#define CAL_UserDBGroupGetProperty  UserDBGroupGetProperty
	#define CHK_UserDBGroupGetProperty  TRUE
	#define EXP_UserDBGroupGetProperty  CAL_CMEXPAPI( "UserDBGroupGetProperty" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupGetProperty
	#define EXT_UserDBGroupGetProperty
	#define GET_UserDBGroupGetProperty(fl)  CAL_CMGETAPI( "UserDBGroupGetProperty" ) 
	#define CAL_UserDBGroupGetProperty  UserDBGroupGetProperty
	#define CHK_UserDBGroupGetProperty  TRUE
	#define EXP_UserDBGroupGetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetProperty", (RTS_UINTPTR)UserDBGroupGetProperty, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupGetProperty
	#define EXT_CmpUserDBUserDBGroupGetProperty
	#define GET_CmpUserDBUserDBGroupGetProperty  ERR_OK
	#define CAL_CmpUserDBUserDBGroupGetProperty pICmpUserDB->IUserDBGroupGetProperty
	#define CHK_CmpUserDBUserDBGroupGetProperty (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupGetProperty  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupGetProperty
	#define EXT_UserDBGroupGetProperty
	#define GET_UserDBGroupGetProperty(fl)  CAL_CMGETAPI( "UserDBGroupGetProperty" ) 
	#define CAL_UserDBGroupGetProperty pICmpUserDB->IUserDBGroupGetProperty
	#define CHK_UserDBGroupGetProperty (pICmpUserDB != NULL)
	#define EXP_UserDBGroupGetProperty  CAL_CMEXPAPI( "UserDBGroupGetProperty" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupGetProperty  PFUSERDBGROUPGETPROPERTY pfUserDBGroupGetProperty;
	#define EXT_UserDBGroupGetProperty  extern PFUSERDBGROUPGETPROPERTY pfUserDBGroupGetProperty;
	#define GET_UserDBGroupGetProperty(fl)  s_pfCMGetAPI2( "UserDBGroupGetProperty", (RTS_VOID_FCTPTR *)&pfUserDBGroupGetProperty, (fl), 0, 0)
	#define CAL_UserDBGroupGetProperty  pfUserDBGroupGetProperty
	#define CHK_UserDBGroupGetProperty  (pfUserDBGroupGetProperty != NULL)
	#define EXP_UserDBGroupGetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetProperty", (RTS_UINTPTR)UserDBGroupGetProperty, 0, 0) 
#endif




/**
 * <description>Check if the user is a member of the group</description>
 * <param name="hGroup" type="IN">Handle to the user group</param>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User is a member of the group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user or user group handle</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">User is no member of the specified user group</errorcode>
 */
RTS_RESULT CDECL UserDBGroupHasUser(RTS_HANDLE hGroup, RTS_HANDLE hUser);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPHASUSER) (RTS_HANDLE hGroup, RTS_HANDLE hUser);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPHASUSER_NOTIMPLEMENTED)
	#define USE_UserDBGroupHasUser
	#define EXT_UserDBGroupHasUser
	#define GET_UserDBGroupHasUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupHasUser(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupHasUser  FALSE
	#define EXP_UserDBGroupHasUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupHasUser
	#define EXT_UserDBGroupHasUser
	#define GET_UserDBGroupHasUser(fl)  CAL_CMGETAPI( "UserDBGroupHasUser" ) 
	#define CAL_UserDBGroupHasUser  UserDBGroupHasUser
	#define CHK_UserDBGroupHasUser  TRUE
	#define EXP_UserDBGroupHasUser  CAL_CMEXPAPI( "UserDBGroupHasUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupHasUser
	#define EXT_UserDBGroupHasUser
	#define GET_UserDBGroupHasUser(fl)  CAL_CMGETAPI( "UserDBGroupHasUser" ) 
	#define CAL_UserDBGroupHasUser  UserDBGroupHasUser
	#define CHK_UserDBGroupHasUser  TRUE
	#define EXP_UserDBGroupHasUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupHasUser", (RTS_UINTPTR)UserDBGroupHasUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupHasUser
	#define EXT_CmpUserDBUserDBGroupHasUser
	#define GET_CmpUserDBUserDBGroupHasUser  ERR_OK
	#define CAL_CmpUserDBUserDBGroupHasUser pICmpUserDB->IUserDBGroupHasUser
	#define CHK_CmpUserDBUserDBGroupHasUser (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupHasUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupHasUser
	#define EXT_UserDBGroupHasUser
	#define GET_UserDBGroupHasUser(fl)  CAL_CMGETAPI( "UserDBGroupHasUser" ) 
	#define CAL_UserDBGroupHasUser pICmpUserDB->IUserDBGroupHasUser
	#define CHK_UserDBGroupHasUser (pICmpUserDB != NULL)
	#define EXP_UserDBGroupHasUser  CAL_CMEXPAPI( "UserDBGroupHasUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupHasUser  PFUSERDBGROUPHASUSER pfUserDBGroupHasUser;
	#define EXT_UserDBGroupHasUser  extern PFUSERDBGROUPHASUSER pfUserDBGroupHasUser;
	#define GET_UserDBGroupHasUser(fl)  s_pfCMGetAPI2( "UserDBGroupHasUser", (RTS_VOID_FCTPTR *)&pfUserDBGroupHasUser, (fl), 0, 0)
	#define CAL_UserDBGroupHasUser  pfUserDBGroupHasUser
	#define CHK_UserDBGroupHasUser  (pfUserDBGroupHasUser != NULL)
	#define EXP_UserDBGroupHasUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupHasUser", (RTS_UINTPTR)UserDBGroupHasUser, 0, 0) 
#endif




/**
 * <description>Iteration interface to get the first user of the user group</description>
 * <param name="hGroup" type="IN">Handle to the user group</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First user in the group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No user configured in the user group</errorcode>
 * <result>Handle to the first user in the user group or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGroupGetFirstUser(RTS_HANDLE hGroup, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGROUPGETFIRSTUSER) (RTS_HANDLE hGroup, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPGETFIRSTUSER_NOTIMPLEMENTED)
	#define USE_UserDBGroupGetFirstUser
	#define EXT_UserDBGroupGetFirstUser
	#define GET_UserDBGroupGetFirstUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupGetFirstUser(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGroupGetFirstUser  FALSE
	#define EXP_UserDBGroupGetFirstUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupGetFirstUser
	#define EXT_UserDBGroupGetFirstUser
	#define GET_UserDBGroupGetFirstUser(fl)  CAL_CMGETAPI( "UserDBGroupGetFirstUser" ) 
	#define CAL_UserDBGroupGetFirstUser  UserDBGroupGetFirstUser
	#define CHK_UserDBGroupGetFirstUser  TRUE
	#define EXP_UserDBGroupGetFirstUser  CAL_CMEXPAPI( "UserDBGroupGetFirstUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupGetFirstUser
	#define EXT_UserDBGroupGetFirstUser
	#define GET_UserDBGroupGetFirstUser(fl)  CAL_CMGETAPI( "UserDBGroupGetFirstUser" ) 
	#define CAL_UserDBGroupGetFirstUser  UserDBGroupGetFirstUser
	#define CHK_UserDBGroupGetFirstUser  TRUE
	#define EXP_UserDBGroupGetFirstUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetFirstUser", (RTS_UINTPTR)UserDBGroupGetFirstUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupGetFirstUser
	#define EXT_CmpUserDBUserDBGroupGetFirstUser
	#define GET_CmpUserDBUserDBGroupGetFirstUser  ERR_OK
	#define CAL_CmpUserDBUserDBGroupGetFirstUser pICmpUserDB->IUserDBGroupGetFirstUser
	#define CHK_CmpUserDBUserDBGroupGetFirstUser (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupGetFirstUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupGetFirstUser
	#define EXT_UserDBGroupGetFirstUser
	#define GET_UserDBGroupGetFirstUser(fl)  CAL_CMGETAPI( "UserDBGroupGetFirstUser" ) 
	#define CAL_UserDBGroupGetFirstUser pICmpUserDB->IUserDBGroupGetFirstUser
	#define CHK_UserDBGroupGetFirstUser (pICmpUserDB != NULL)
	#define EXP_UserDBGroupGetFirstUser  CAL_CMEXPAPI( "UserDBGroupGetFirstUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupGetFirstUser  PFUSERDBGROUPGETFIRSTUSER pfUserDBGroupGetFirstUser;
	#define EXT_UserDBGroupGetFirstUser  extern PFUSERDBGROUPGETFIRSTUSER pfUserDBGroupGetFirstUser;
	#define GET_UserDBGroupGetFirstUser(fl)  s_pfCMGetAPI2( "UserDBGroupGetFirstUser", (RTS_VOID_FCTPTR *)&pfUserDBGroupGetFirstUser, (fl), 0, 0)
	#define CAL_UserDBGroupGetFirstUser  pfUserDBGroupGetFirstUser
	#define CHK_UserDBGroupGetFirstUser  (pfUserDBGroupGetFirstUser != NULL)
	#define EXP_UserDBGroupGetFirstUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetFirstUser", (RTS_UINTPTR)UserDBGroupGetFirstUser, 0, 0) 
#endif




/**
 * <description>Iteration interface to get next configured user of the user group. Must be started with UserDBGroupGetFirstUser()</description>
 * <param name="hGroup" type="IN">Handle to the user group</param>
 * <param name="hPrevUser" type="IN">Handle to the previous user retrieved via UserDBGroupGetFirstUser() or subsequent calls of UserDBGroupGetNextUser()</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next user available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next user available</errorcode>
 * <result>Handle to the next user or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGroupGetNextUser(RTS_HANDLE hGroup, RTS_HANDLE hPrevUser, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGROUPGETNEXTUSER) (RTS_HANDLE hGroup, RTS_HANDLE hPrevUser, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPGETNEXTUSER_NOTIMPLEMENTED)
	#define USE_UserDBGroupGetNextUser
	#define EXT_UserDBGroupGetNextUser
	#define GET_UserDBGroupGetNextUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupGetNextUser(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGroupGetNextUser  FALSE
	#define EXP_UserDBGroupGetNextUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupGetNextUser
	#define EXT_UserDBGroupGetNextUser
	#define GET_UserDBGroupGetNextUser(fl)  CAL_CMGETAPI( "UserDBGroupGetNextUser" ) 
	#define CAL_UserDBGroupGetNextUser  UserDBGroupGetNextUser
	#define CHK_UserDBGroupGetNextUser  TRUE
	#define EXP_UserDBGroupGetNextUser  CAL_CMEXPAPI( "UserDBGroupGetNextUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupGetNextUser
	#define EXT_UserDBGroupGetNextUser
	#define GET_UserDBGroupGetNextUser(fl)  CAL_CMGETAPI( "UserDBGroupGetNextUser" ) 
	#define CAL_UserDBGroupGetNextUser  UserDBGroupGetNextUser
	#define CHK_UserDBGroupGetNextUser  TRUE
	#define EXP_UserDBGroupGetNextUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetNextUser", (RTS_UINTPTR)UserDBGroupGetNextUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupGetNextUser
	#define EXT_CmpUserDBUserDBGroupGetNextUser
	#define GET_CmpUserDBUserDBGroupGetNextUser  ERR_OK
	#define CAL_CmpUserDBUserDBGroupGetNextUser pICmpUserDB->IUserDBGroupGetNextUser
	#define CHK_CmpUserDBUserDBGroupGetNextUser (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupGetNextUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupGetNextUser
	#define EXT_UserDBGroupGetNextUser
	#define GET_UserDBGroupGetNextUser(fl)  CAL_CMGETAPI( "UserDBGroupGetNextUser" ) 
	#define CAL_UserDBGroupGetNextUser pICmpUserDB->IUserDBGroupGetNextUser
	#define CHK_UserDBGroupGetNextUser (pICmpUserDB != NULL)
	#define EXP_UserDBGroupGetNextUser  CAL_CMEXPAPI( "UserDBGroupGetNextUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupGetNextUser  PFUSERDBGROUPGETNEXTUSER pfUserDBGroupGetNextUser;
	#define EXT_UserDBGroupGetNextUser  extern PFUSERDBGROUPGETNEXTUSER pfUserDBGroupGetNextUser;
	#define GET_UserDBGroupGetNextUser(fl)  s_pfCMGetAPI2( "UserDBGroupGetNextUser", (RTS_VOID_FCTPTR *)&pfUserDBGroupGetNextUser, (fl), 0, 0)
	#define CAL_UserDBGroupGetNextUser  pfUserDBGroupGetNextUser
	#define CHK_UserDBGroupGetNextUser  (pfUserDBGroupGetNextUser != NULL)
	#define EXP_UserDBGroupGetNextUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetNextUser", (RTS_UINTPTR)UserDBGroupGetNextUser, 0, 0) 
#endif




/**
 * <description>Check if a user group is a member of the first specified group</description>
 * <param name="hGroup" type="IN">Handle to the first user group</param>
 * <param name="hGroupMember" type="IN">Handle to the user group to check if it is a member of hGroup</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User group is a member of the first group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user group handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">User group is no member of the specified user group</errorcode>
 */
RTS_RESULT CDECL UserDBGroupHasMember(RTS_HANDLE hGroup, RTS_HANDLE hGroupMember);
typedef RTS_RESULT (CDECL * PFUSERDBGROUPHASMEMBER) (RTS_HANDLE hGroup, RTS_HANDLE hGroupMember);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPHASMEMBER_NOTIMPLEMENTED)
	#define USE_UserDBGroupHasMember
	#define EXT_UserDBGroupHasMember
	#define GET_UserDBGroupHasMember(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupHasMember(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGroupHasMember  FALSE
	#define EXP_UserDBGroupHasMember  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupHasMember
	#define EXT_UserDBGroupHasMember
	#define GET_UserDBGroupHasMember(fl)  CAL_CMGETAPI( "UserDBGroupHasMember" ) 
	#define CAL_UserDBGroupHasMember  UserDBGroupHasMember
	#define CHK_UserDBGroupHasMember  TRUE
	#define EXP_UserDBGroupHasMember  CAL_CMEXPAPI( "UserDBGroupHasMember" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupHasMember
	#define EXT_UserDBGroupHasMember
	#define GET_UserDBGroupHasMember(fl)  CAL_CMGETAPI( "UserDBGroupHasMember" ) 
	#define CAL_UserDBGroupHasMember  UserDBGroupHasMember
	#define CHK_UserDBGroupHasMember  TRUE
	#define EXP_UserDBGroupHasMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupHasMember", (RTS_UINTPTR)UserDBGroupHasMember, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupHasMember
	#define EXT_CmpUserDBUserDBGroupHasMember
	#define GET_CmpUserDBUserDBGroupHasMember  ERR_OK
	#define CAL_CmpUserDBUserDBGroupHasMember pICmpUserDB->IUserDBGroupHasMember
	#define CHK_CmpUserDBUserDBGroupHasMember (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupHasMember  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupHasMember
	#define EXT_UserDBGroupHasMember
	#define GET_UserDBGroupHasMember(fl)  CAL_CMGETAPI( "UserDBGroupHasMember" ) 
	#define CAL_UserDBGroupHasMember pICmpUserDB->IUserDBGroupHasMember
	#define CHK_UserDBGroupHasMember (pICmpUserDB != NULL)
	#define EXP_UserDBGroupHasMember  CAL_CMEXPAPI( "UserDBGroupHasMember" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupHasMember  PFUSERDBGROUPHASMEMBER pfUserDBGroupHasMember;
	#define EXT_UserDBGroupHasMember  extern PFUSERDBGROUPHASMEMBER pfUserDBGroupHasMember;
	#define GET_UserDBGroupHasMember(fl)  s_pfCMGetAPI2( "UserDBGroupHasMember", (RTS_VOID_FCTPTR *)&pfUserDBGroupHasMember, (fl), 0, 0)
	#define CAL_UserDBGroupHasMember  pfUserDBGroupHasMember
	#define CHK_UserDBGroupHasMember  (pfUserDBGroupHasMember != NULL)
	#define EXP_UserDBGroupHasMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupHasMember", (RTS_UINTPTR)UserDBGroupHasMember, 0, 0) 
#endif




/**
 * <description>Iteration interface to get the first user group as member of the specified group</description>
 * <param name="hGroup" type="IN">Handle to the user group</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First user group member available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user group handle</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No user group configured</errorcode>
 * <result>Handle to the first user group member or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGroupGetFirstMember(RTS_HANDLE hGroup, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGROUPGETFIRSTMEMBER) (RTS_HANDLE hGroup, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPGETFIRSTMEMBER_NOTIMPLEMENTED)
	#define USE_UserDBGroupGetFirstMember
	#define EXT_UserDBGroupGetFirstMember
	#define GET_UserDBGroupGetFirstMember(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupGetFirstMember(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGroupGetFirstMember  FALSE
	#define EXP_UserDBGroupGetFirstMember  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupGetFirstMember
	#define EXT_UserDBGroupGetFirstMember
	#define GET_UserDBGroupGetFirstMember(fl)  CAL_CMGETAPI( "UserDBGroupGetFirstMember" ) 
	#define CAL_UserDBGroupGetFirstMember  UserDBGroupGetFirstMember
	#define CHK_UserDBGroupGetFirstMember  TRUE
	#define EXP_UserDBGroupGetFirstMember  CAL_CMEXPAPI( "UserDBGroupGetFirstMember" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupGetFirstMember
	#define EXT_UserDBGroupGetFirstMember
	#define GET_UserDBGroupGetFirstMember(fl)  CAL_CMGETAPI( "UserDBGroupGetFirstMember" ) 
	#define CAL_UserDBGroupGetFirstMember  UserDBGroupGetFirstMember
	#define CHK_UserDBGroupGetFirstMember  TRUE
	#define EXP_UserDBGroupGetFirstMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetFirstMember", (RTS_UINTPTR)UserDBGroupGetFirstMember, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupGetFirstMember
	#define EXT_CmpUserDBUserDBGroupGetFirstMember
	#define GET_CmpUserDBUserDBGroupGetFirstMember  ERR_OK
	#define CAL_CmpUserDBUserDBGroupGetFirstMember pICmpUserDB->IUserDBGroupGetFirstMember
	#define CHK_CmpUserDBUserDBGroupGetFirstMember (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupGetFirstMember  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupGetFirstMember
	#define EXT_UserDBGroupGetFirstMember
	#define GET_UserDBGroupGetFirstMember(fl)  CAL_CMGETAPI( "UserDBGroupGetFirstMember" ) 
	#define CAL_UserDBGroupGetFirstMember pICmpUserDB->IUserDBGroupGetFirstMember
	#define CHK_UserDBGroupGetFirstMember (pICmpUserDB != NULL)
	#define EXP_UserDBGroupGetFirstMember  CAL_CMEXPAPI( "UserDBGroupGetFirstMember" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupGetFirstMember  PFUSERDBGROUPGETFIRSTMEMBER pfUserDBGroupGetFirstMember;
	#define EXT_UserDBGroupGetFirstMember  extern PFUSERDBGROUPGETFIRSTMEMBER pfUserDBGroupGetFirstMember;
	#define GET_UserDBGroupGetFirstMember(fl)  s_pfCMGetAPI2( "UserDBGroupGetFirstMember", (RTS_VOID_FCTPTR *)&pfUserDBGroupGetFirstMember, (fl), 0, 0)
	#define CAL_UserDBGroupGetFirstMember  pfUserDBGroupGetFirstMember
	#define CHK_UserDBGroupGetFirstMember  (pfUserDBGroupGetFirstMember != NULL)
	#define EXP_UserDBGroupGetFirstMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetFirstMember", (RTS_UINTPTR)UserDBGroupGetFirstMember, 0, 0) 
#endif




/**
 * <description>Iteration interface to get next configured user group as member of the specified group. Must be started with UserDBGroupGetFirstMember()</description>
 * <param name="hGroup" type="IN">Handle to the user group</param>
 * <param name="hPrevGroupMember" type="IN">Handle to the previous user group retrieved via UserDBGroupGetFirstMember() or subsequent calls of UserDBGroupGetNextMember()</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next user group member available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next user group member available</errorcode>
 * <result>Handle to the next user group member or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserDBGroupGetNextMember(RTS_HANDLE hGroup, RTS_HANDLE hPrevGroupMember, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERDBGROUPGETNEXTMEMBER) (RTS_HANDLE hGroup, RTS_HANDLE hPrevGroupMember, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGROUPGETNEXTMEMBER_NOTIMPLEMENTED)
	#define USE_UserDBGroupGetNextMember
	#define EXT_UserDBGroupGetNextMember
	#define GET_UserDBGroupGetNextMember(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGroupGetNextMember(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserDBGroupGetNextMember  FALSE
	#define EXP_UserDBGroupGetNextMember  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGroupGetNextMember
	#define EXT_UserDBGroupGetNextMember
	#define GET_UserDBGroupGetNextMember(fl)  CAL_CMGETAPI( "UserDBGroupGetNextMember" ) 
	#define CAL_UserDBGroupGetNextMember  UserDBGroupGetNextMember
	#define CHK_UserDBGroupGetNextMember  TRUE
	#define EXP_UserDBGroupGetNextMember  CAL_CMEXPAPI( "UserDBGroupGetNextMember" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGroupGetNextMember
	#define EXT_UserDBGroupGetNextMember
	#define GET_UserDBGroupGetNextMember(fl)  CAL_CMGETAPI( "UserDBGroupGetNextMember" ) 
	#define CAL_UserDBGroupGetNextMember  UserDBGroupGetNextMember
	#define CHK_UserDBGroupGetNextMember  TRUE
	#define EXP_UserDBGroupGetNextMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetNextMember", (RTS_UINTPTR)UserDBGroupGetNextMember, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGroupGetNextMember
	#define EXT_CmpUserDBUserDBGroupGetNextMember
	#define GET_CmpUserDBUserDBGroupGetNextMember  ERR_OK
	#define CAL_CmpUserDBUserDBGroupGetNextMember pICmpUserDB->IUserDBGroupGetNextMember
	#define CHK_CmpUserDBUserDBGroupGetNextMember (pICmpUserDB != NULL)
	#define EXP_CmpUserDBUserDBGroupGetNextMember  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGroupGetNextMember
	#define EXT_UserDBGroupGetNextMember
	#define GET_UserDBGroupGetNextMember(fl)  CAL_CMGETAPI( "UserDBGroupGetNextMember" ) 
	#define CAL_UserDBGroupGetNextMember pICmpUserDB->IUserDBGroupGetNextMember
	#define CHK_UserDBGroupGetNextMember (pICmpUserDB != NULL)
	#define EXP_UserDBGroupGetNextMember  CAL_CMEXPAPI( "UserDBGroupGetNextMember" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserDBGroupGetNextMember  PFUSERDBGROUPGETNEXTMEMBER pfUserDBGroupGetNextMember;
	#define EXT_UserDBGroupGetNextMember  extern PFUSERDBGROUPGETNEXTMEMBER pfUserDBGroupGetNextMember;
	#define GET_UserDBGroupGetNextMember(fl)  s_pfCMGetAPI2( "UserDBGroupGetNextMember", (RTS_VOID_FCTPTR *)&pfUserDBGroupGetNextMember, (fl), 0, 0)
	#define CAL_UserDBGroupGetNextMember  pfUserDBGroupGetNextMember
	#define CHK_UserDBGroupGetNextMember  (pfUserDBGroupGetNextMember != NULL)
	#define EXP_UserDBGroupGetNextMember  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserDBGroupGetNextMember", (RTS_UINTPTR)UserDBGroupGetNextMember, 0, 0) 
#endif





#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFUSERDBISCONFIGURABLE IUserDBIsConfigurable;
 	PFUSERDBLOAD IUserDBLoad;
 	PFUSERDBSTORE IUserDBStore;
 	PFUSERDBRELEASE IUserDBRelease;
 	PFUSERDBADD IUserDBAdd;
 	PFUSERDBREMOVE IUserDBRemove;
 	PFUSERDBSETPASSWORD IUserDBSetPassword;
 	PFUSERDBGETPASSWORDMD5 IUserDBGetPasswordMD5;
 	PFUSERDBSETPASSWORDMD5 IUserDBSetPasswordMD5;
 	PFUSERDBSETPROPERTY IUserDBSetProperty;
 	PFUSERDBGETFIRST IUserDBGetFirst;
 	PFUSERDBGETNEXT IUserDBGetNext;
 	PFUSERDBOPEN IUserDBOpen;
 	PFUSERDBCLOSE IUserDBClose;
 	PFUSERDBGETNAME IUserDBGetName;
 	PFUSERDBGETPROPERTY IUserDBGetProperty;
 	PFUSERDBCHECKPASSWORD IUserDBCheckPassword;
 	PFUSERDBGETFIRSTGROUP IUserDBGetFirstGroup;
 	PFUSERDBGETNEXTGROUP IUserDBGetNextGroup;
 	PFUSERDBGROUPADD IUserDBGroupAdd;
 	PFUSERDBGROUPREMOVE IUserDBGroupRemove;
 	PFUSERDBGROUPADDUSER IUserDBGroupAddUser;
 	PFUSERDBGROUPREMOVEUSER IUserDBGroupRemoveUser;
 	PFUSERDBGROUPSETPROPERTY IUserDBGroupSetProperty;
 	PFUSERDBGROUPADDMEMBER IUserDBGroupAddMember;
 	PFUSERDBGROUPREMOVEMEMBER IUserDBGroupRemoveMember;
 	PFUSERDBENFORCE IUserDBEnforce;
 	PFUSERDBGROUPGETFIRST IUserDBGroupGetFirst;
 	PFUSERDBGROUPGETNEXT IUserDBGroupGetNext;
 	PFUSERDBGROUPOPEN IUserDBGroupOpen;
 	PFUSERDBGROUPCLOSE IUserDBGroupClose;
 	PFUSERDBGROUPGETNAME IUserDBGroupGetName;
 	PFUSERDBGROUPGETPROPERTY IUserDBGroupGetProperty;
 	PFUSERDBGROUPHASUSER IUserDBGroupHasUser;
 	PFUSERDBGROUPGETFIRSTUSER IUserDBGroupGetFirstUser;
 	PFUSERDBGROUPGETNEXTUSER IUserDBGroupGetNextUser;
 	PFUSERDBGROUPHASMEMBER IUserDBGroupHasMember;
 	PFUSERDBGROUPGETFIRSTMEMBER IUserDBGroupGetFirstMember;
 	PFUSERDBGROUPGETNEXTMEMBER IUserDBGroupGetNextMember;
 } ICmpUserDB_C;

#ifdef CPLUSPLUS
class ICmpUserDB : public IBase
{
	public:
		virtual RTS_RESULT CDECL IUserDBIsConfigurable(RTS_UI32 *pulPropertyUsers, RTS_UI32 *pulPropertyGroups) =0;
		virtual RTS_RESULT CDECL IUserDBLoad(void) =0;
		virtual RTS_RESULT CDECL IUserDBStore(void) =0;
		virtual RTS_RESULT CDECL IUserDBRelease(void) =0;
		virtual RTS_HANDLE CDECL IUserDBAdd(char *pszUser, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserDBRemove(RTS_HANDLE hUser) =0;
		virtual RTS_RESULT CDECL IUserDBSetPassword(RTS_HANDLE hUser, char *pszPassword) =0;
		virtual RTS_RESULT CDECL IUserDBGetPasswordMD5(RTS_HANDLE hUser, unsigned char *pbyPasswordMD5, int *piMaxLen) =0;
		virtual RTS_RESULT CDECL IUserDBSetPasswordMD5(RTS_HANDLE hUser, unsigned char *pbyPasswordMD5, int *piMaxLen) =0;
		virtual RTS_RESULT CDECL IUserDBSetProperty(RTS_HANDLE hUser, RTS_UI32 ulProperty) =0;
		virtual RTS_HANDLE CDECL IUserDBGetFirst(RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserDBGetNext(RTS_HANDLE hPrevUser, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserDBOpen(char *pszUser, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserDBClose(RTS_HANDLE hUser) =0;
		virtual RTS_RESULT CDECL IUserDBGetName(RTS_HANDLE hUser, char *pszUser, int *piMaxLen) =0;
		virtual RTS_RESULT CDECL IUserDBGetProperty(RTS_HANDLE hUser, RTS_UI32 *pulProperty) =0;
		virtual RTS_RESULT CDECL IUserDBCheckPassword(RTS_HANDLE hUser, char *pszPassword) =0;
		virtual RTS_HANDLE CDECL IUserDBGetFirstGroup(RTS_HANDLE hUser, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserDBGetNextGroup(RTS_HANDLE hUser, RTS_HANDLE hPrevGroup, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserDBGroupAdd(char *pszGroup, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserDBGroupRemove(RTS_HANDLE hGroup) =0;
		virtual RTS_RESULT CDECL IUserDBGroupAddUser(RTS_HANDLE hGroup, RTS_HANDLE hUser) =0;
		virtual RTS_RESULT CDECL IUserDBGroupRemoveUser(RTS_HANDLE hGroup, RTS_HANDLE hUser) =0;
		virtual RTS_RESULT CDECL IUserDBGroupSetProperty(RTS_HANDLE hGroup, RTS_UI32 ulProperty) =0;
		virtual RTS_RESULT CDECL IUserDBGroupAddMember(RTS_HANDLE hGroup, RTS_HANDLE hGroupMember) =0;
		virtual RTS_RESULT CDECL IUserDBGroupRemoveMember(RTS_HANDLE hGroup, RTS_HANDLE hGroupMember) =0;
		virtual RTS_RESULT CDECL IUserDBEnforce(void) =0;
		virtual RTS_HANDLE CDECL IUserDBGroupGetFirst(RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserDBGroupGetNext(RTS_HANDLE hPrevGroup, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserDBGroupOpen(char *pszGroup, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserDBGroupClose(RTS_HANDLE hGroup) =0;
		virtual RTS_RESULT CDECL IUserDBGroupGetName(RTS_HANDLE hGroup, char *pszGroup, int *piMaxLen) =0;
		virtual RTS_RESULT CDECL IUserDBGroupGetProperty(RTS_HANDLE hGroup, RTS_UI32 *pulProperty) =0;
		virtual RTS_RESULT CDECL IUserDBGroupHasUser(RTS_HANDLE hGroup, RTS_HANDLE hUser) =0;
		virtual RTS_HANDLE CDECL IUserDBGroupGetFirstUser(RTS_HANDLE hGroup, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserDBGroupGetNextUser(RTS_HANDLE hGroup, RTS_HANDLE hPrevUser, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserDBGroupHasMember(RTS_HANDLE hGroup, RTS_HANDLE hGroupMember) =0;
		virtual RTS_HANDLE CDECL IUserDBGroupGetFirstMember(RTS_HANDLE hGroup, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserDBGroupGetNextMember(RTS_HANDLE hGroup, RTS_HANDLE hPrevGroupMember, RTS_RESULT *pResult) =0;
};
	#ifndef ITF_CmpUserDB
		#define ITF_CmpUserDB static ICmpUserDB *pICmpUserDB = NULL;
	#endif
	#define EXTITF_CmpUserDB
#else	/*CPLUSPLUS*/
	typedef ICmpUserDB_C		ICmpUserDB;
	#ifndef ITF_CmpUserDB
		#define ITF_CmpUserDB
	#endif
	#define EXTITF_CmpUserDB
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPUSERDBITF_H_*/
