 /**
 * <interfacename>CmpUserMgr</interfacename>
 * <description> 
 *  Interface for the user management component.
 *
 *  The user management in the runtime system is based on ACL (Access Control Lists). Here we have Users and UserGroups
 *  and access rights on objects.
 *
 *  The user manager requires some other interfaces to be implemented for full functionality:
 *
 *  1. CmpUserDB: This interface contains the basic functions to authentica users and to upload users to the CODESYS IDE
 *			in order to manage Groups. This interface has only read access to the database. No functions that can changed
 *			a value are defined here. This is the first interface to start if there is an existing user management that
 *			should be used by the CODESYS runtime system.
 *
 *	2. CmpUserDBConfiguration: This interface is used by the UserManager to Change the content of the users database. If this is
 *			not implemented it is not possible to manage the users and the corresponding credentials using the CODESYS IDE.
 *
 *	3. CmpUserGroupsDB: This interface provides access to existing user groups and users inside groups. This interface is
 *			read only too. No changes to the database can be done using this interface.
 * 
 *  4. CmpUserGroupsDBConfiguration: This interface allows to configure the user groups and allows to assign members to groups.
 *
 * 	All of these four interfaces are defined as static interfaces. To allow the user manager to use these interfaces it is
 *	required that the implementing component / components register the desired backends to the CmpUserMgr. It is possible to
 *	replace only the User Authentication interface and to stay with the default user groups and right database. This allows
 *	easy adoption of the CODESYS runtime to existing user databases.
 * 
 *  5. CmpUserObjectsDB:
 *
 *     Every object in the runtime system, that must be protected with access rights, is registered dynamically during startup of the runtime by the corresponding
 *     component under the appropriate folder.
 *
 *     The objects in the runtime system are organized as a tree with 2 root nodes:
 *     - Runtime system object:
 *          "Device"
 *              |----- "PlcLogic"
 *              |----------- "Application"
 *              |----- "Logger"
 *              |----- "Settings"
 *              |----- "UserManagement"	
 *
 *     - File system objects:
 *             "/"
 *              |----- "Temp"
 *              |----------- "Test.txt"
 *
 *     On every object there are assigned all user groups. And for every user group you can specify the appropriate access rights.
 *          
 *     There are 5 different access rights that can be specified at every group for each object:
 *
 *     - USERDB_RIGHT_NONE: Access denied
 *
 *     - USERDB_RIGHT_VIEW: Only view rights on the object. Object cannot be modified in any case!
 *
 *     - USERDB_RIGHT_MODIFY: Object can be viewed and modified
 *
 *     - USERDB_RIGHT_EXECUTE: Object can be executed
 *
 *     - USERDB_RIGHT_ADD_REMOVE: It is allowed to add and remove objects or sub objects
 *
 *     - USERDB_RIGHT_ALL: All rights allowed on the object
 *
 *     Not every access right is retrieved by an object. Which access rights are used are specified by each object and are masked in the user rights editor in the 
 *     programming system!
 *
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPUSERMGRITF_H_
#define _CMPUSERMGRITF_H_

#include "CmpStd.h"

 

 




#include "CmpBinTagUtilItf.h"
#include "CmpDeviceItf.h"
#include "CmpCommunicationLibItf.h"

#include "CmpItf.h"


/**
 * <category>Static defines</category>
 * <description>Number of predefined static login credential contexts used for authentication</description>
 */
#ifndef USERMGR_NUM_OF_STATIC_CREDENTIAL_CONTEXTS
	#define USERMGR_NUM_OF_STATIC_CREDENTIAL_CONTEXTS	4
#endif

/**
 * <category>Static defines</category>
 * <description>Number of predefined static session entries</description>
 */
#ifndef USERMGR_NUM_OF_STATIC_SESSIONS
	#define USERMGR_NUM_OF_STATIC_SESSIONS			8
#endif

/**
 * <category>Static defines</category>
 * <description>Max. length of the passwords. This value is used as buffer size and includes the terminating NUL. </description>
 */
#define USERMGR_MAX_PASSWORD_LEN					60

/**
 * <category>Static defines</category>
 * <description>Max. length of the user names. This value is used as buffer size and includes the terminating NUL. </description>
 */
#define USERMGR_MAX_USERNAME_LEN					60

/**
 * <category>Static defines</category>
 * <description>Max. length of the user group names. This value is used as buffer size and includes the terminating NUL. </description>
 */
#define USERMGR_MAX_GROUPNAME_LEN					60


/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_READ_USER_MGMT" name="Read user management">
 * 	<description>
 *		Online service to read the user management.
 *
 *		This service requires a read right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 * 	</description>
 *	<request/>
 *	<response>
 *		<tag id="TAG_PROPERTY_USERS" name="UserDB Properties." cardinality="1" type="RTS_UI32" description="Properties of the user database."/>
 *		<tag id="TAG_PROPERTY_GROUPS" name="GroupDB Properties." cardinality="1" type="RTS_UI32" description="Properties of the user group database."/>
 *		<complextag id="TAG_USER" name="User" cardinality="0..n" description="This tag contains a user.">
 *			<tag id="TAG_NAME" name="UserName" cardinality="1" type="RTS_UTF8STRING" description="The user name."/>
 *			<tag id="TAG_PROPERTY" name="UserProperties" cardinality="1" type="RTS_UI32" description="The users properties"/>
 *		</complextag>
 *		<complextag id="TAG_GROUP" name="Group" cardinality="0..n" description="This tag contains a user group.">
 *			<tag id="TAG_NAME" name="GroupName" cardinality="1" type="RTS_UTF8STRING" description="The name of the user group name."/>
 *			<tag id="TAG_PROPERTY" name="GroupProperties" cardinality="1" type="RTS_UI32" description="The user groups properties"/>
 *			<tag id="TAG_USER_MEMBER" name="UserMember" cardinality="0..1" type="RTS_UTF8STRING" description="A user that is member of this group."/>
 *			<tag id="TAG_GROUP_MEMBER" name="GroupMember" cardinality="0..1" type="RTS_UTF8STRING" description="An other group that is member of this group./>
 *		</complextag>
 *	</response>
 * </service>
 */
#define SRV_READ_USER_MGMT							0x01

/**
 * <category>Online services</category>
 * <description>Online service to write the user management. This service is not supported since V3.5.16.0.</description>
 */
#define SRV_WRITE_USER_MGMT							0x02


/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_READ_USER_RIGHTS_MGMT" name="Read user rights management">
 *	<description>
 *		Online service to read the user rights management
 *
 *		This service requires a read right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<tag id="TAG_OBJECT_HANDLE" name="GroupDB Properties." cardinality="0..1" type="Binary" description="Handle of the last object received."/>
 *	</request>
 *	<response>
 *		<complextag id="TAG_OBJECT" name="Object" cardinality="0..n" description="An object of the user rights management.">
 *			<tag id="TAG_ERROR" name="Serviceresult" cardinality="0..1" type="RTS_UI16" description="Errorcode of the operation"/>
 *			<tag id="TAG_OBJECT_HANDLE" name="GroupDB Properties." cardinality="0..1" type="Binary" description="Handle of the last object transmitted. Send this within the next request to get the next bunch of objects."/>
 *			<tag id="TAG_NAME" name="ObjectName" cardinality="1" type="RTS_UTF8STRING" description="The name of the object."/>
 *			<tag id="TAG_ID" name="ID" cardinality="1" type="RTS_UI32" description="ID of the object."/>
 *			<tag id="TAG_PARENT_ID" name="ParentID" cardinality="1" type="RTS_UI32" description="ID of the parent object."/>
 *			<tag id="TAG_USED_RIGHTS" name="UsedRights" cardinality="0..1" type="RTS_UI32" description="The evaluated rights of this object."/>
 *			<tag id="TAG_PROPERTY" name="ObjectProperty" cardinality="0..1" type="RTS_UI32" description="The properties of the object"/>
 *			<complextag id="TAG_GROUP" name="Group" cardinality="0..n" description="A group that has configured rights on an object.">
 *				<tag id="TAG_NAME" name="GroupName" cardinality="1" type="RTS_UTF8STRING" description="The name of the group"/>
 *				<tag id="TAG_RIGHTS" name="GarntedRights" cardinality="0..1" type="RTS_UI32" description="The granted rights on that object."/>
 *				<tag id="TAG_DENIED_RIGHTS" name="DeniedRights" cardinality="0..1" type="RTS_UI32" description="The denied rights on that object."/>
 *			</complextag>
 *		</complextag>
 *		
 *	</response>
 * </service>
 */
#define SRV_READ_USER_RIGHTS_MGMT					0x03

 /**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_READ_USER_RIGHTS_MGMT" name="Read user rights management">
 *	<description>
 *		Online service to write the user rights management
 *
 *		This service requires a modify right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<complextag id="TAG_OBJECT" name="Object" cardinality="0..n" description="An object of the user rights management.">
 *			<tag id="TAG_NAME" name="ObjectName" cardinality="1" type="RTS_UTF8STRING" description="The name of the object."/>
 *			<tag id="TAG_ID" name="ID" cardinality="1" type="RTS_UI32" description="ID of the object."/>
 *			<tag id="TAG_PARENT_ID" name="ParentID" cardinality="1" type="RTS_UI32" description="ID of the parent object."/>
 *			<complextag id="TAG_GROUP" name="Group" cardinality="0..n" description="A group that has configured rights on an object.">
 *				<tag id="TAG_NAME" name="GroupName" cardinality="1" type="RTS_UTF8STRING" description="The name of the group"/>
 *				<tag id="TAG_RIGHTS" name="GarntedRights" cardinality="0..1" type="RTS_UI32" description="The granted rights on that object."/>
 *				<tag id="TAG_DENIED_RIGHTS" name="DeniedRights" cardinality="0..1" type="RTS_UI32" description="The denied rights on that object."/>
 *			</complextag>
 *		</complextag>
 *	</request>
 *	<response>
 *		<tag id="TAG_ERROR" name="Serviceresult" cardinality="0..1" type="RTS_UI16" description="Errorcode of the operation"/>
 *	</response>
 * </service>
 */
#define SRV_WRITE_USER_RIGHTS_MGMT					0x04

/**
 * <category>Online services</category>
 * <description>Online service to add a single user. This service is not supported since V3.5.16.0.</description>
 */
#define SRV_USER_ADD								0x05

/**
 * <category>Online services</category>
 * <description>Online service to delete a single user. This service is not supported since V3.5.16.0.</description>
 */
#define SRV_USER_DELETE								0x06

/**
 * <category>Online services</category>
 * <description>Online service to change the password of a single user. This service is not supported since V3.5.16.0.</description>
 */
#define SRV_USER_CHANGE_PASSWORD					0x07

/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_UPDATE_CURRENT_USER_CREDENTIALS" name="Update current credentials">
 *	 <description>Online service to change the credentials of the current user. This service is allowed to all authenticated users. This 
 *				service will only affect the user that has logged into the current session. This service requires that the user authenticates
 *				by using the existing credentials.</description>
 *	<request>
 *		<tag id="TAG_USERMGR_CRYPT_TYPE" name="LoginProtocolType" cardinality="1" type="RTS_UI32" description="Authentication type. Must match to the type negotiated by SRV_DEV_SESSION_CREATE."/>
 *		<tag id="TAG_USERMGR_CRYPT_STEP" name="CurrentLoginProtocolStep" cardinality="0..1" type="RTS_UI32" description="Current step of login sequence."/>
 *		<complextag id="TAG_USERMGR_CREDENTIALS" name="Credentials" cardinality="0..1" description="Top level tag. Contains the user credentials.">
 *			<tag id="TAG_USERMGR_PASSWORD" name="Password" cardinality="0..1" type="Binary" description="Encrypted old password."/>
 *			<tag id="TAG_USERMGR_NEW_PASSWORD" name="New Password" cardinality="0..1" type="Binary" description="Encrypted new password."/>
 *		</complextag>
 *	</request>
 *	<response>
 *		<tag id="TAG_USERMGR_CRYPT_PUB_KEY" name="PLC Public Key" cardinality="0..1" type="char*" description="Public key used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."></tag>
 *		<tag id="TAG_USERMGR_CRYPT_NONCE" name="Nonce" cardinality="0..1" type="char*" description="Nonce used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."></tag>
 *		<tag id="TAG_USERMGR_CRYPT_NONCE_NEW" name="NonceNew" cardinality="0..1" type="char*" description="Nonce used for encryption of the new password. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."></tag>
 *		<tag id="TAG_USERMGR_CRYPT_CHALLENGE" name="Challange" cardinality="0..1" type="char*" description="Challange used for password encryption. Only sent for authentication type USERMGR_CRYPT_XOR."></tag>
 *		<tag id="TAG_USERMGR_CRYPT_CHALLENGE_NEW" name="ChallangeNew" cardinality="0..1" type="char*" description="Challange used for encryption of the new password. Only sent for authentication type USERMGR_CRYPT_XOR."></tag>
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="Password changed successfully" />
 *			<value define="ERR_NOT_SUPPORTED" description="Changing the credentials is not possible." />
 *			<value define="ERR_OPERATION_DENIED" description="The database is not configurable." />
 *			<value define="ERR_FAILED" description="Password not changed successfully" />
 *		</result>
 *	</response>
 * </service>
 */
#define SRV_UPDATE_CURRENT_USER_CREDENTIALS			0x08

/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_ADD_USER" name="Add User">
 *	<description>
 *		Online service to add a new user. 
 *
 *		To add a user two steps are required:
 *		1. Request the runtime to add a new user. This service returns the required values depending on the login protocol used by the new user.
 *		2. Request contains the actual user that should be added.
 *
 *		This service requires a modify right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<tag id="TAG_USERMGR_CRYPT_TYPE" name="LoginProtocolType" cardinality="1" type="RTS_UI32" description="Authentication type. This defines the protocol type of the new user."/>
 *		<tag id="TAG_USERMGR_CRYPT_STEP" name="CurrentLoginProtocolStep" cardinality="0..1" type="RTS_UI32" description="Current step of login sequence."/>
 *		<tag id="TAG_USERMGR_USER" name="Username" cardinality="1" type="RTS_UTF8STRING" description="The name of the new user."/>
 *		<tag id="TAG_PROPERTY" name="User Property" cardinality="1" type="RTS_UI32" description="Properties of the new user."/>
 *		<tag id="TAG_USERMGR_CREDENTIALS" name="Credentials" cardinality="0..1" description="Top level tag. Contains the user credentials. This is transmitted in the second step.">
 *			<tag id="TAG_USERMGR_NEW_PASSWORD" name="Password" cardinality="0..1" type="Binary" description="The encrypted password of the new user."/>
 *		</tag>
 *	</request>
 *	<response>
 *		<tag id="TAG_USERMGR_CRYPT_PUB_KEY" name="PLC Public Key" cardinality="0..1" type="char*" description="Public key used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<tag id="TAG_USERMGR_CRYPT_NONCE_NEW" name="Nonce" cardinality="0..1" type="char*" description="Nonce used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<tag id="TAG_USERMGR_CRYPT_CHALLENGE_NEW" name="Nonce" cardinality="0..1" type="char*" description="challange used for password encryption. Only sent for authentication type USERMGR_CRYPT_XOR."/>
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="User added successfully" />
 *			<value define="ERR_DUPLICATE" description="The user is existing already. No changes made to the database." />
 *			<value define="ERR_NOT_SUPPORTED" description="The requested login protocol is not supported by this PLC." />
 *			<value define="ERR_OPERATION_DENIED" description="The database is not configurable." />
 *		</tag>
 *	</result>
 *	</response>
 * </service>
 */
#define SRV_ADD_USER 0x09

/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_REMOVE_USER" name="Remove User">
 *	<description>
 *		Remove a user from the database. Additional all group assignments of the user will be deleted. 
 *
 *		This service requires a modify right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<tag id="TAG_USERMGR_USER" name="Username" cardinality="1" type="RTS_UTF8STRING" description="The name of the user."/>
 *	</request>
 *	<response>
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="User removedsuccessfully" />
 *			<value define="ERR_NO_OBJECT" description="The user is not existing. No changes made to the database." />
 *			<value define="ERR_NOT_SUPPORTED" description="Deleting the last administrator user is not possible." />
 *			<value define="ERR_OPERATION_DENIED" description="The database is not configurable." />
 *		</tag>
 *	</result>
 *	</response>
 * </service>
 */
#define SRV_REMOVE_USER 0x0A


/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_SET_USER_PROPERTY" name="Set User Property">
 *	<description>
 *		Set the properies of a user.
 *
 *		This service requires a modify right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<tag id="TAG_USERMGR_USER" name="Username" cardinality="1" type="RTS_UTF8STRING" description="The name of the user."/>
 *		<tag id="TAG_PROPERTY" name="Properties" cardinality="1" type="RTS_UI32" description="The name of the user."/>
 *	</request>
 *	<response>
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="User removedsuccessfully" />
 *			<value define="ERR_NO_OBJECT" description="The user is not existing. No changes made to the database." />
 *			<value define="ERR_OPERATION_DENIED" description="The database is not configurable." />
 *		</tag>
 *	</result>
 *	</response>
 * </service>
 */
#define SRV_SET_USER_PROPERTY 0x0B

/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_SET_CREDENTIALS_USER" name="Set User Credentials">
 *	<description>
 *		Update the credentials of an existing user.  
 *
 *		This service requires a modify right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<tag id="TAG_USERMGR_CRYPT_TYPE" name="LoginProtocolType" cardinality="1" type="RTS_UI32" description="Authentication type. Must match to the type negotiated by SRV_DEV_SESSION_CREATE."/>
 *		<tag id="TAG_USERMGR_CRYPT_STEP" name="CurrentLoginProtocolStep" cardinality="0..1" type="RTS_UI32" description="Current step of login sequence."/>
 *		<complextag id="TAG_USERMGR_CREDENTIALS" name="Credentials" cardinality="1" description="Top level tag. Contains the user credentials.">
 *			<tag id="TAG_USERMGR_USER" name="Username" cardinality="1" type="RTS_UTF8STRING" description="The name of the user."/>
 *			<tag id="TAG_USERMGR_PASSWORD" name="Password" cardinality="0..1" type="Binary" description="Encrypted password."/>
 *		</complextag>
 *	</request>
 *	<response>
 *		<tag id="TAG_USERMGR_CRYPT_PUB_KEY" name="PLC Public Key" cardinality="0..1" type="RTS_CSTRING" description="Public key used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<tag id="TAG_USERMGR_CRYPT_NONCE_NEW" name="Nonce" cardinality="0..1" type="char*" description="Nonce used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<tag id="TAG_USERMGR_CRYPT_CHALLENGE_NEW" name="Nonce" cardinality="0..1" type="char*" description="challange used for password encryption. Only sent for authentication type USERMGR_CRYPT_XOR."/>
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="Credentials changed successfully" />
 *			<value define="ERR_NO_OBJECT" description="The user is not existing. No changes made to the database." />
 *			<value define="ERR_NOT_SUPPORTED" description="The requested login protocol is not supported by this PLC." />
 *			<value define="ERR_OPERATION_DENIED" description="The database is not configurable." />
 *		</result>
 *	</response>  
 * </service>
 */
#define SRV_SET_CREDENTIALS_USER 0x0C

/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_ADD_GROUP" name="Add a user group">
 *	<description>
 *		Add a new user group to the runtime system. 
 *
 *		This service requires a modify right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<tag id="TAG_NAME" name="Group Name" cardinality="1" type="RTS_UTF8STRING" description="The name of the new group."/>
 *		<tag id="TAG_GROUP_MEMBER" name="Group Name" cardinality="0..1" type="RTS_UTF8STRING" description="The name of other groups that should be added to this group. Tag can occur 0..n times"/>
 *		<tag id="TAG_USER_MEMBER" name="Group Name" cardinality="0..1" type="RTS_UTF8STRING" description="The name of users that should be added to this group. Tag can occur 0..n times."/>
 *	</request>
 *	<response>
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="Group added successfully" />
  *			<value define="ERR_DUPLICATE" description="A group with this name is already existing." />
 *			<value define="ERR_OPERATION_DENIED" description="The database is not configurable." />
 *		</result>
 *	</response>  
 * </service>
 */
#define SRV_ADD_GROUP 0x0D

/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_REMOVE_GROUP" name="Remove a user group">
 *	<description>
 *		Remove an existing user group from the runtime system. Additional this group is removed from all other groups where it was a member.
 *		
 *		This service requires a modify right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<tag id="TAG_NAME" name="Group Name" cardinality="1" type="RTS_UTF8STRING" description="The name of the new group."/>
 *	</request>
 *	<response>
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="The group was removed successfully" />
 *			<value define="ERR_NO_OBJECT" description="The Group is not existing. No changes made to the database." />
 *			<value define="ERR_NOT_SUPPORTED" description="The administrator group can not be delete." />
 *			<value define="ERR_OPERATION_DENIED" description="The database is not configurable." />
 *		</result>
 *	</response>  
 * </service>
 */
#define SRV_REMOVE_GROUP 0x0E


/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_SET_GROUP_CONFIG" name="Configure a user group">
 *	<description>
 *		Update the configuration of an existing user group. This service contains all users and groups that are member of the configured group.
 *		In a first step the configuration of the group will be cleared. As a second step the new configuration is stored and activated.
 *
 *		This service requires a modify right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<tag id="TAG_NAME" name="Group Name" cardinality="1" type="RTS_UTF8STRING" description="The name of the group to configure."/>
 *		<tag id="TAG_GROUP_MEMBER" name="Group Name" cardinality="0..1" type="RTS_UTF8STRING" description="The name of other groups that should be added to this group. Tag can occur 0..n times"/>
 *		<tag id="TAG_USER_MEMBER" name="Group Name" cardinality="0..1" type="RTS_UTF8STRING" description="The name of users that should be added to this group. Tag can occur 0..n times."/>
 *	</request>
 *	<response>
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="The group configuration has been successfully stored." />
 *			<value define="ERR_NO_OBJECT" description="The group is not existing. No changes made to the database." />
 *			<value define="ERR_OPERATION_DENIED" description="The database is not configurable." />
 *		</result>
 *	</response>  
 * </service>
 */
#define SRV_SET_GROUP_CONFIG 0x0F

/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_EXPORT_USER_MGMT" name="Export the user management.">
 *	<description>
 *		The service exports the user management into a backup file which can be uploaded by the CODESYS IDE.
 *		The backup contains the corresponding user database which can be restored to PLCs that contain the same
 *		backend components.
 *
 *		To complete the service call two messages two messages in each direction are neccesarry. In the first step
 *		the needed information to transmit the password which is used to secure the backup are collected from the device
 *		The second step transmit the encrypted password and returns the path of the backup file.
 *
 *		This service requires read rights on the User Management object.
 *	</description>
 *	<request>
 *		<tag id="TAG_USERMGR_CRYPT_TYPE" name="LoginProtocolType" cardinality="1" type="RTS_UI32" description="Authentication type. Must match to the type negotiated by SRV_DEV_SESSION_CREATE."/>
 *		<tag id="TAG_USERMGR_CRYPT_STEP" name="CurrentLoginProtocolStep" cardinality="0..1" type="RTS_UI32" description="Current step of login sequence."/>
 *		<complextag id="TAG_USERMGR_CREDENTIALS" name="Credentials" cardinality="1" description="Top level tag. Contains the password used to encrypt the backup file.">
 *			<tag id="TAG_USERMGR_PASSWORD" name="Password" cardinality="0..1" type="Binary" description="Encrypted password."/>
 *		</complextag>
 *	</request>
 *	<response>
 *		<tag id="TAG_USERMGR_CRYPT_PUB_KEY" name="PLC Public Key" cardinality="0..1" type="RTS_CSTRING" description="Public key used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<tag id="TAG_USERMGR_CRYPT_NONCE" name="Nonce" cardinality="0..1" type="char*" description="Nonce used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<tag id="TAG_USERMGR_CRYPT_CHALLENGE" name="Nonce" cardinality="0..1" type="char*" description="challange used for password encryption. Only sent for authentication type USERMGR_CRYPT_XOR."/>
 *		<tag id="TAG_USERMGR_BACKUP_FILE" cardinality="1" name="Backfile path" type="RTS_CSTRING" description="Path of the backup file on the PLC. Can be used by file transfer services to upload the file.">
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="The group configuration has been successfully stored." />
 *			<value define="ERR_PENDING" description="The password exchange has not finished yet." />
 *			<value define="ERR_OPERATION_DENIED" description="The database could not be exported." />
 *		</result>
 *	</response>  
 * </service>
 */
#define SRV_EXPORT_USER_MGMT 0x10

/**
 * <category>Online services</category>
 * <service group="SG_CMPUSERMGR" id="SRV_IMPORT_USER_MGMT" name="Import the user management.">
 *	<description>
 *		The service imports the user management from a backup file which can be downloaded by the CODESYS IDE.
 *		The backup contains the corresponding user database which can be restored to PLCs that contain the same
 *		backend components.
 *
 *		This service requires a modify right the user management. Additional the last authentication must not
 *		or than several minutes. It the last authentication is older than several minutes a OnlineAccessError with ERR_REAUTHENTICATE is returned.
 *	</description>
 *	<request>
 *		<tag id="TAG_USERMGR_CRYPT_TYPE" name="LoginProtocolType" cardinality="1" type="RTS_UI32" description="Authentication type. Must match to the type negotiated by SRV_DEV_SESSION_CREATE."/>
 *		<tag id="TAG_USERMGR_CRYPT_STEP" name="CurrentLoginProtocolStep" cardinality="0..1" type="RTS_UI32" description="Current step of login sequence."/>
 *		<complextag id="TAG_USERMGR_CREDENTIALS" name="Credentials" cardinality="1" description="Top level tag. Contains the password used to decrypt the backup file.">
 *			<tag id="TAG_USERMGR_PASSWORD" name="Password" cardinality="0..1" type="Binary" description="Encrypted password."/>
 *		</complextag>
 *		<tag id="TAG_USERMGR_BACKUP_FILE" cardinality="1" name="Backfile path" type="RTS_CSTRING" description="Path on the PLC to backup file to restore.">
 *	</request>
 *	<response>
 *		<tag id="TAG_USERMGR_CRYPT_PUB_KEY" name="PLC Public Key" cardinality="0..1" type="RTS_CSTRING" description="Public key used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<tag id="TAG_USERMGR_CRYPT_NONCE" name="Nonce" cardinality="0..1" type="char*" description="Nonce used for password encryption. Only sent for authentication type USERMGR_CRYPT_ASYMMETRIC."/>
 *		<tag id="TAG_USERMGR_CRYPT_CHALLENGE" name="Nonce" cardinality="0..1" type="char*" description="challange used for password encryption. Only sent for authentication type USERMGR_CRYPT_XOR."/>
 *		<tag id="TAG_ERROR" cardinality="1" result="true" name="Serviceresult" type="RTS_UI16">
 *			<value define="ERR_OK" description="The group configuration has been successfully stored." />
 *			<value define="ERR_OPERATION_DENIED" description="The database could not be imported." />
 *			<value define="ERR_PARAMETER" description="The file to be imported is not valid" />
 *			<value define="ERR_SECURITY_CHECKS_FAILED" description="The password of the import file was not valid." />
 *		</result>
 *	</response>  
 * </service>
 */
#define SRV_IMPORT_USER_MGMT 0x11


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
 * <category>Online tags</category>
 * <description>Tags for login service </description>
 */
#define TAG_USERMGR_CRYPT_TYPE						0x22 
#define TAG_USERMGR_CRYPT_CHALLENGE					0x23 
/* #define TAG_DEV_REPLY_SETTINGS					0x24  not used here, but reserve value --> defined in CmpDeviceItf */ 
#define TAG_USERMGR_CRYPT_STEP						0x25 
#define TAG_USERMGR_CRYPT_NONCE						0x26 
#define TAG_USERMGR_CRYPT_PUB_KEY					0x27 
#define TAG_USERMGR_CRYPT_NONCE_NEW					0x28
#define TAG_USERMGR_CRYPT_CHALLENGE_NEW					0x29 
#define TAG_USRMGR_REPLY_LOGIN						TAG_DEV_REPLY_LOGIN /* 0x82*/
#define TAG_USRMGR_REPLY_LOGIN_RESULT				TAG_DEV_REPLY_LOGIN_RESULT /* 0x20 */

#define TAG_USERMGR_CREDENTIALS						0x81
#define TAG_USERMGR_USER							0x10
#define TAG_USERMGR_PASSWORD						0x11
#define TAG_USERMGR_NEW_PASSWORD					0x12
#define TAG_USERMGR_BACKUP_FILE						0x13

/**
 * <category>Crypt types</category>
 * <description>Crypt types to cipher the password or alternative authentication algorithms. Values can be used as bit mask.</description>
 * <element name="USERMGR_CRYPT_INVALID" type="IN">Invalid of unknown crypt type</element>
 * <element name="USERMGR_CRYPT_XOR" type="IN">Disguise password with an XOR algorithm</element>
 * <element name="USERMGR_CRYPT_ASYMMETRIC" type="IN">Transport password with public key encryption</element>
 */
#define USERMGR_CRYPT_INVALID				0x00000000
#define USERMGR_CRYPT_XOR					0x00000001
#define USERMGR_CRYPT_ASYMMETRIC			0x00000002

/**
 * <category>Static defines</category>
 * <description>Predefined objects in the runtime</description>
 * <element name="USERDB_OBJECT_USERMGMT" type="IN">Toplevel usermanagement object</element>
 * <element name="USERDB_OBJECT_USERMGMT_GROUPS" type="IN">Subobject to manage access rights to groups</element>
 * <element name="USERDB_OBJECT_USERMGMT_USERS" type="IN">Subobject to manage access rights to users</element> 
 * <element name="USERDB_OBJECT_USERMGMT_ACCESSRIGHTS" type="IN">Subobject to manage access rights</element>
 */
#define USERDB_OBJECT_USERMGMT		        "Device.UserManagement"
#define USERDB_OBJECT_USERMGMT_GROUPS		USERDB_OBJECT_USERMGMT ".Groups"
#define USERDB_OBJECT_USERMGMT_USERS		USERDB_OBJECT_USERMGMT ".Users"
#define USERDB_OBJECT_USERMGMT_ACCESSRIGHTS	USERDB_OBJECT_USERMGMT ".Access Rights"


/**
 * <category>Old database announcements</category>
 * <description>A database announces an old database to the user management</description>
 * <element name="USERMGR_OLDDATABASE_ANNOUNCE" type="IN">An old database is found and converting is requested. This may be denied.</element>
 * <element name="USERMGR_OLDDATABASE_FINISH" type="IN">An old database is converted and adjustment may be processed now</element>
 */
#define USERMGR_OLDDATABASE_ANNOUNCE		0x00000001
#define USERMGR_OLDDATABASE_FINISH			0x00000002


/**
 * <category>Event parameter</category>
 * <element name="bUserManagementChanged" type="IN">TRUE: User management database changed; FALSE: Not changed</element>
 * <element name="bRightsManagementChanged" type="IN">TRUE: Rights management database changed; FALSE: Not changed</element>
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
 * <description>Event is sent every time the user management database or the user-rights database was changed</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpUserMgrDatabaseChanged</param>
 */
#define EVT_UserMgrDatabaseChanged		MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Event parameter</category>
 * <element name="hUser" type="IN">Handle of a temprary administrator user, only valid with this event</element>
 * <element name="Result" type="OUT">Result of the adjustment</element>
 */
typedef struct
{
	RTS_HANDLE hUser;
	RTS_RESULT Result;
} EVTPARAM_CmpUserMgrAdjustDatabase;

#define EVTPARAMID_CmpUserMgrAdjustDatabase		0x0002
#define EVTVERSION_CmpUserMgrAdjustDatabase		0x0001

/**
 * <category>Events</category>
 * <description>
 * Event is sent when a default user management is to be created, especially with user groups and object rights.
 * The Result is preset with ERR_NOTIMPLEMENTED and in this standard case the user management creates the default database with user groups and object rights.
 * A temporary administrator user handle is provided for operations that require this.
 * With the Result set to ERR_OK a user management was successfully created within this event, or to any error if failed. 
 * </description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpUserMgrAdjustDatabase</param>
 */
#define EVT_UserMgrCreateDefaultDatabase		MAKE_EVENTID(EVTCLASS_INFO, 2)

/**
 * <category>Events</category>
 * <description>
 * Event is sent when an old user database is to be converted.
 * The Result is preset with ERR_OK and in this standard case the user management is converted to the new format.
 * With the Result set to ERR_OPERATION_DENIED this conversion is denied, that is as if there is no user database.
 * hUser is not used in this case and preset with RTS_INVALID_HANDLE.
 * NOTE: This event must be registered in CH_INIT2 because the database is loaded in CH_PRE_INIT3 already.
 * </description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpUserMgrAdjustDatabase</param>
 */
#define EVT_UserMgrConvertOldDatabaseAnnounce	MAKE_EVENTID(EVTCLASS_INFO, 3)

/**
 * <category>Events</category>
 * <description>
 * Event is sent when an old user database was successfully converted to the new format. The user database can be adjusted during this event.
 * A temporary administrator user handle is provided for operations that require this.
 * The Result is preset with ERR_OK.
 * NOTE: This event is not send when EVT_UserMgrConvertOldDatabaseAnnounce is denied.
 * NOTE: This event must be registered in CH_INIT2 because loading of the database is finished in CH_INIT3 already.
 * </description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpUserMgrAdjustDatabase</param>
 */
#define EVT_UserMgrConvertOldDatabaseFinish		MAKE_EVENTID(EVTCLASS_INFO, 4)

/**
 * <category>Static defines</category>
 * <description>
 *  Defines the update interval in ms for the key pair used by the asymmetric authentication algorithm. Whenever
 *  this time elapses since the startup of the device or the last key generation, the CmpUserMgr generates
 *  a new pair of keys in the background.
 * </description>
 */
#ifndef USERMGR_ASYMMETRIC_KEY_UPDATE_INTERVAL
	#define USERMGR_ASYMMETRIC_KEY_UPDATE_INTERVAL			(24*60*60*1000)	/* 24h */		
#endif

/**
 * <category>Static defines</category>
 * <description>Subcategory for password policy security settings of the UserMgr.
 * </description>
 */
#define USERMGR_SECSETTINGS_SUBCAT_PWDPOLICY						".UserMgmt.PasswordPolicy"

/**
 * <category>Settings</category>
 * <type>INT</type>
 * <description>
 *  This setting defines the startup behavior of the CmpUserMgr regarding key generation:
 *  The CmpUserMgr provides a key pair for the asymmetric authentication algorithm. Depending on the CPU performance
 *  of the device, the generation of this key pair may take about 2..100 seconds. As long as these keys are not available, 
 *  no client (e. g. the CODESYS Development system) is allowed to login. 
 *  Defined values (all other values are reserved for future use): 
 *  0:  The key pair is never stored and must be newly generated on each start of the runtime system. The keys are only
 *      kept in RAM.
 *  1:  Fast startup: When ever a new key pair is generated, these are stored. During startup the stored keys are loaded 
 *      to let the runtime system communicate immediately. If no keys exist (initial startup), then logins are blocked
 *      until the keys are available.   
 *  Independently of the settings the keys are updated regularly in the background. 
 * </description>
 */
#define USERMGRKEY_INT_FAST_KEY_STARTUP						"FastKeyStartup"
#ifndef USERMGRKEY_INT_FAST_KEY_STARTUP_DEFAULT
	#define USERMGRKEY_INT_FAST_KEY_STARTUP_DEFAULT			1
#endif

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  Key identifier for asymmetric authentication. Used to handle stored keys for the asymmetric authentication algorithm.
 *  This setting is managed by the CmpUserMgr and must not be set by hand.
 * </description>
 */
#define USERMGR_KEY_STRING_ASYMMETRIC_AUTH_KEY				"AsymmetricAuthKey"

/**
 * <category>SecuritySettings</category>
 * <description>Security setting to select the authentication algorithm. Can be selected in CmpSecurityManager.
 *	NOTE:
 *		Security modes must be ordered in an descending sorting, i.e. the setting with the highest security level is found at the beginning of the list!
 * </description>
 */	
#define CMPUSERMGR_KEY_STRING_LOGIN_AUTH_TYPE			CMPSETTINGS_CATEGORY_SECURITY ".UserLogin_AuthenticationType"
#define CMPUSERMGR_KEY_STRING_LOGIN_AUTH_TYPE_DESC		"Authentication algorithm"

#define CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_ASYMM			"ONLY_ASYMMETRIC"
#define CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_AUTO			"AUTO_NEGOTIATE"
#define CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_LEGACY			"ONLY_LEGACY"

#ifndef CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_DEFAULT
	#define CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_DEFAULT	CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_AUTO
#endif


/**
 * <category>SecuritySettings</category>
 * <description>Security setting to select the authentication algorithm.
 * </description>
 */
typedef enum
{
	UserMgrLoginAuthType_Asymmetric,
	UserMgrLoginAuthType_Auto,
	UserMgrLoginAuthType_Legacy
} USERMGR_LOGIN_AUTH_TYPE;

#define CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_LIST		{ \
														{(RTS_I32)UserMgrLoginAuthType_Asymmetric, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_ASYMM, "Password, asymmetric encryption"},\
														{(RTS_I32)UserMgrLoginAuthType_Auto, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_AUTO, "Negotiate authentication algorithm"},\
														{(RTS_I32)UserMgrLoginAuthType_Legacy, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_LOGIN_AUTH_TYPE_LEGACY, "Password, legacy encryption"},\
													}


/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: RSA key length for asymmetric authentication algorithm. 
 *  Minimal allowed value: MIN(CMPUSERMGR_VALUE_LOGIN_RSALEN_MIN, CMPSETTINGS_CATEGORY_SECURITY ".RSAMinKeyLen")
 * </description>
 */
#define CMPUSERMGR_KEY_INT_LOGIN_RSALEN				CMPSETTINGS_CATEGORY_SECURITY ".UserLogin_RSAKeyLen"
#define CMPUSERMGR_KEY_INT_LOGIN_RSALEN_DESC		"RSA key length for asymmetric authentication algorithm"

#ifndef CMPUSERMGR_VALUE_LOGIN_RSALEN_DEFAULT
	#define CMPUSERMGR_VALUE_LOGIN_RSALEN_DEFAULT	2048
#endif

#define CMPUSERMGR_VALUE_LOGIN_RSALEN_MIN			1024

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
	#define CMPUSERMGR_VALUE_ENFORCE_DEFAULT		CMPUSERMGR_VALUE_ENFORCE_YES
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
													{(RTS_I32)UserMgmtEnforce_Yes, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_ENFORCE_YES, "User management is enforced, i.e. no anonymous login possible (only for excluded components when UserMgmtAllowAnonymous is set)"},\
													{(RTS_I32)UserMgmtEnforce_No, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_ENFORCE_NO, "User management is optional"}\
												}


/**
 * <category>SecuritySettings</category>
 * <description>Security settings to allow anonymous login for registered components also when user management is activated.</description>
 * <element name="UserMgmtAllowAnonymous_No" type="IN">No anonymous login allowed when user management is activated.</element>
 * <element name="UserMgmtAllowAnonymous_Yes" type="IN">Anonymous login allowed when user management is activated.</element>
 */
#define CMPUSERMGR_KEY_STRING_ALLOWANONYMOUS			CMPSETTINGS_CATEGORY_SECURITY ".UserMgmtAllowAnonymous"
#define CMPUSERMGR_KEY_STRING_ALLOWANONYMOUS_DESC		"Setting to allow anonymous login for registered components when user management is activated."

#define CMPUSERMGR_VALUE_ALLOWANONYMOUS_NO				"NO"
#define CMPUSERMGR_VALUE_ALLOWANONYMOUS_YES				"YES"

#ifndef CMPUSERMGR_VALUE_ALLOWANONYMOUS_DEFAULT
	#define CMPUSERMGR_VALUE_ALLOWANONYMOUS_DEFAULT		CMPUSERMGR_VALUE_ALLOWANONYMOUS_NO
#endif

typedef enum
{
	UserMgmtAllowAnonymous_No,
	UserMgmtAllowAnonymous_Yes
} SECURITY_USERMGR_ALLOWANONYMOUS;

#define CMPUSERMGR_VALUE_ALLOWANONYMOUS_LIST	{\
													{(RTS_I32)UserMgmtAllowAnonymous_No, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_ALLOWANONYMOUS_NO, "No anonymous login is allowed when user management is activated."},\
													{(RTS_I32)UserMgmtAllowAnonymous_Yes, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_ALLOWANONYMOUS_YES, "Anonymous login is possible for registered components."}\
												}
												
												
/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: Maximum time in [s] between subsequent services calls to edit the user management. If the time is expired the user has to authenticate again.
 * </description>
 */
#define CMPUSERMGR_KEY_INT_EDITTIMEOUT				CMPSETTINGS_CATEGORY_SECURITY ".UserMgrEditTimeout"
#define CMPUSERMGR_KEY_INT_EDITTIMEOUT_DESC			"Maximum time in [s] between subsequent services calls to edit the user management. If the time is expired the user has to authenticate again."

#ifndef CMPUSERMGR_VALUE_EDITTIMEOUT_DEFAULT
	#define CMPUSERMGR_VALUE_EDITTIMEOUT_DEFAULT	300
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: Enable/disable user management password policy. </description>
 */
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY				    CMPSETTINGS_CATEGORY_SECURITY USERMGR_SECSETTINGS_SUBCAT_PWDPOLICY
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_DESC			"Enable/disable user management password policy"

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_ENABLED				"ENABLED"
#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_DISABLED				"DISABLED"

#ifndef CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_DEFAULT
	#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_DEFAULT	        CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_DISABLED
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings to enable the user management password policy.
 * </description>
 */
typedef enum
{
	UserMgmtPwdPolicy_Enabled,
	UserMgmtPwdPolicy_Disabled
} SECURITY_USRMGMT_PWDPOLICY;

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_LIST	{\
															{(RTS_I32)UserMgmtPwdPolicy_Enabled, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_ENABLED, "User management password policy is enabled"},\
															{(RTS_I32)UserMgmtPwdPolicy_Disabled, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_DISABLED, "User management password policy is disabled"}\
														}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: User management passwords minimum length.</description>
 */
#define CMPUSERMGR_KEY_INT_USRMGMT_PWDPOLICY_LENGTH				CMPSETTINGS_CATEGORY_SECURITY USERMGR_SECSETTINGS_SUBCAT_PWDPOLICY ".Length"
#define CMPUSERMGR_KEY_INT_USRMGMT_PWDPOLICY_LENGTH_DESC		"Minimum length"

#ifndef CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_LENGTH_DEFAULT
	#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_LENGTH_DEFAULT	8
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: User management passwords minimum number of unique characters.</description>
 */
#define CMPUSERMGR_KEY_INT_USRMGMT_PWDPOLICY_UNIQUECHARS			CMPSETTINGS_CATEGORY_SECURITY USERMGR_SECSETTINGS_SUBCAT_PWDPOLICY ".UniqueCharacters"
#define CMPUSERMGR_KEY_INT_USRMGMT_PWDPOLICY_UNIQUECHARS_DESC		"Minimum number of unique characters"

#ifndef CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_UNIQUECHARS_DEFAULT
	#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_UNIQUECHARS_DEFAULT	4
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: User management passwords must contain lowercase letters. </description>
 */
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_LOWERCASE				CMPSETTINGS_CATEGORY_SECURITY USERMGR_SECSETTINGS_SUBCAT_PWDPOLICY ".Characters.Lowercase"
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_LOWERCASE_DESC		"Must contain lowercase letters"

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_LOWERCASE_YES				"YES"
#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_LOWERCASE_NO				"NO"

#ifndef CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_LOWERCASE_DEFAULT
	#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_LOWERCASE_DEFAULT	CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_LOWERCASE_YES
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings to require lowercase letters in user management passwords.
 * </description>
 */
typedef enum
{
	UserMgmtPwdPolicyCharsLowercase_Yes,
	UserMgmtPwdPolicyCharsLowercase_No
} SECURITY_USRMGMT_PWDPOLICY_CHARS_LOWERCASE;

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_LOWERCASE_LIST	{\
																	{(RTS_I32)UserMgmtPwdPolicyCharsLowercase_Yes, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_LOWERCASE_YES, "User management passwords have to contain lowercase letters."},\
																	{(RTS_I32)UserMgmtPwdPolicyCharsLowercase_No, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_LOWERCASE_NO, "User management passwords don't not have to contain lowercase letters."}\
																}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: User management passwords must contain uppercase letters. </description>
 */
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_UPPERCASE				CMPSETTINGS_CATEGORY_SECURITY USERMGR_SECSETTINGS_SUBCAT_PWDPOLICY ".Characters.Uppercase"
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_UPPERCASE_DESC		"Must contain uppercase letters"

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_UPPERCASE_YES				"YES"
#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_UPPERCASE_NO				"NO"

#ifndef CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_UPPERCASE_DEFAULT
	#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_UPPERCASE_DEFAULT	CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_UPPERCASE_YES
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings to require uppercase letters in user management passwords.
 * </description>
 */
typedef enum
{
	UserMgmtPwdPolicyCharsUppercase_Yes,
	UserMgmtPwdPolicyCharsUppercase_No
} SECURITY_USRMGMT_PWDPOLICY_CHARS_UPPERCASE;

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_UPPERCASE_LIST	{\
																	{(RTS_I32)UserMgmtPwdPolicyCharsUppercase_Yes, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_UPPERCASE_YES, "User management passwords have to contain uppercase letters."},\
																	{(RTS_I32)UserMgmtPwdPolicyCharsUppercase_No, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_UPPERCASE_NO, "User management passwords don't not have to contain uppercase letters."}\
																}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: User management passwords must contain digits. </description>
 */
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_DIGITS				CMPSETTINGS_CATEGORY_SECURITY USERMGR_SECSETTINGS_SUBCAT_PWDPOLICY ".Characters.Digits"
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_DIGITS_DESC			"Must contain digits"

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_DIGITS_YES					"YES"
#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_DIGITS_NO					"NO"

#ifndef CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_DIGITS_DEFAULT
	#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_DIGITS_DEFAULT	CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_DIGITS_YES
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings to require digits in user management passwords.
 * </description>
 */
typedef enum
{
	UserMgmtPwdPolicyCharsDigits_Yes,
	UserMgmtPwdPolicyCharsDigits_No
} SECURITY_USRMGMT_PWDPOLICY_CHARS_DIGITS;

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_DIGITS_LIST	{\
																	{(RTS_I32)UserMgmtPwdPolicyCharsDigits_Yes, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_DIGITS_YES, "User management passwords have to contain digits."},\
																	{(RTS_I32)UserMgmtPwdPolicyCharsDigits_No, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_DIGITS_NO, "User management passwords don't not have to contain digits."}\
																}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: User management passwords must contain special characters. </description>
 */
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_SPECIAL				CMPSETTINGS_CATEGORY_SECURITY USERMGR_SECSETTINGS_SUBCAT_PWDPOLICY ".Characters.Special"
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_SPECIAL_DESC			"Must contain special characters"

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_SPECIAL_YES				"YES"
#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_SPECIAL_NO					"NO"

#ifndef CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_SPECIAL_DEFAULT
	#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_SPECIAL_DEFAULT	CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_SPECIAL_YES
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings to require special characters in user management passwords.
 * </description>
 */
typedef enum
{
	UserMgmtPwdPolicyCharsSpecial_Yes,
	UserMgmtPwdPolicyCharsSpecial_No
} SECURITY_USRMGMT_PWDPOLICY_CHARS_SPECIAL;

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_SPECIAL_LIST	{\
																	{(RTS_I32)UserMgmtPwdPolicyCharsSpecial_Yes, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_SPECIAL_YES, "User management passwords have to contain special characters."},\
																	{(RTS_I32)UserMgmtPwdPolicyCharsSpecial_No, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_SPECIAL_NO, "User management passwords don't not have to contain special characters."}\
																}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: User management passwords must not contain the username. </description>
 */
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME				CMPSETTINGS_CATEGORY_SECURITY USERMGR_SECSETTINGS_SUBCAT_PWDPOLICY ".Characters.NoUsername"
#define CMPUSERMGR_KEY_STRING_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME_DESC			"Must not contain the username"

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME_YES					"YES"
#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME_NO					"NO"

#ifndef CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME_DEFAULT
	#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME_DEFAULT	CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME_YES
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings to disallow the username in user management passwords.
 * </description>
 */
typedef enum
{
	UserMgmtPwdPolicyCharsNoUsername_Yes,
	UserMgmtPwdPolicyCharsNoUsername_No
} SECURITY_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME;

#define CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME_LIST	{\
																		{(RTS_I32)UserMgmtPwdPolicyCharsNoUsername_Yes, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME_YES, "User management passwords must not contain the username."},\
																		{(RTS_I32)UserMgmtPwdPolicyCharsNoUsername_No, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_PWDPOLICY_CHARS_NOUSERNAME_NO, "User management passwords may contain the username."}\
																	}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: User locked when invalid credentials are given.
 * </description>
 */
#define CMPUSERMGR_KEY_STRING_USERLOGINLOCK				CMPSETTINGS_CATEGORY_SECURITY ".UserLogin.Lock"
#define CMPUSERMGR_KEY_STRING_USERLOGINLOCK_DESC			"User locked when invalid credentials are given"

#define CMPUSERMGR_VALUE_USERLOGINLOCK_ENABLED					"ENABLED"
#define CMPUSERMGR_VALUE_USERLOGINLOCK_DISABLED					"DISABLED"

#ifndef CMPUSERMGR_VALUE_USERLOGINLOCK_DEFAULT
	#define CMPUSERMGR_VALUE_USERLOGINLOCK_DEFAULT	CMPUSERMGR_VALUE_USERLOGINLOCK_ENABLED
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security setting: User locked when invalid credentials are given.
 * </description>
 */
typedef enum
{
	UserMgmtUserLoginLock_Enabled,
	UserMgmtUserLoginLock_Disabled
} SECURITY_USERLOGINLOCK;

#define CMPUSERMGR_VALUE_USERLOGINLOCK_LIST	{\
												{(RTS_I32)UserMgmtUserLoginLock_Enabled, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USERLOGINLOCK_ENABLED, "User login lock enabled."},\
												{(RTS_I32)UserMgmtUserLoginLock_Disabled, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USERLOGINLOCK_DISABLED, "User login lock disabled."}\
											}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: Scope; User login lock is checked for admins, non-admins or all.
 *</description>
 */
#define CMPUSERMGR_KEY_STRING_USERLOGINLOCK_SCOPE				CMPSETTINGS_CATEGORY_SECURITY ".UserLogin.Lock.Scope"
#define CMPUSERMGR_KEY_STRING_USERLOGINLOCK_SCOPE_DESC			"User login lock is checked for admins, non-admins or all"

#define CMPUSERMGR_VALUE_USRMGMT_LOGINLOCK_SCOPE_ADMINS					"ADMINS"
#define CMPUSERMGR_VALUE_USRMGMT_LOGINLOCK_SCOPE_NONADMINS				"NONADMINS"
#define CMPUSERMGR_VALUE_USRMGMT_LOGINLOCK_SCOPE_ALL					"ALL"

#ifndef CMPUSERMGR_VALUE_USERLOGINLOCK_SCOPE_DEFAULT
	#define CMPUSERMGR_VALUE_USERLOGINLOCK_SCOPE_DEFAULT	CMPUSERMGR_VALUE_USRMGMT_LOGINLOCK_SCOPE_ADMINS
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings: Scope; User login lock is checked for admins, non-admins or all.
 * </description>
 */
typedef enum
{
	UserMgmtLoginLockScope_Admins,
	UserMgmtLoginLockScope_NonAdmins,
	UserMgmtLoginLockScope_All
} SECURITY_USRMGMT_LOGINLOCK_SCOPE;

#define CMPUSERMGR_VALUE_LOGINLOCK_SCOPE_LIST	{\
													{(RTS_I32)UserMgmtLoginLockScope_Admins, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_LOGINLOCK_SCOPE_ADMINS, "User login lock scope for admins."},\
													{(RTS_I32)UserMgmtLoginLockScope_NonAdmins, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_LOGINLOCK_SCOPE_NONADMINS, "User login lock scope for non-admins."},\
													{(RTS_I32)UserMgmtLoginLockScope_All, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_USRMGMT_LOGINLOCK_SCOPE_ALL, "User login lock scope for all."}\
												}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: MaxRetries; After how many tries of invalid credentials the login user is locked.
 *</description>
 */
#define CMPUSERMGR_KEY_INT_USERLOGINLOCK_MAXRETRIES				CMPSETTINGS_CATEGORY_SECURITY ".UserLogin.Lock.MaxRetries"
#define CMPUSERMGR_KEY_INT_USERLOGINLOCK_MAXRETRIES_DESC			"After how many tries of invalid credentials the login user is locked"

#ifndef CMPUSERMGR_VALUE_USERLOGINLOCK_MAXRETRIES_DEFAULT
	#define CMPUSERMGR_VALUE_USERLOGINLOCK_MAXRETRIES_DEFAULT	5
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: TimeoutSec; After how many seconds a locked user is released.
 *</description>
 */
#define CMPUSERMGR_KEY_INT_USERLOGINLOCK_TIMEOUTSEC				CMPSETTINGS_CATEGORY_SECURITY ".UserLogin.Lock.TimeoutSec"
#define CMPUSERMGR_KEY_INT_USERLOGINLOCK_TIMEOUTSEC_DESC			"After how many seconds a locked user is released"

#ifndef CMPUSERMGR_VALUE_USERLOGINLOCK_TIMEOUTSEC_DEFAULT
	#define CMPUSERMGR_VALUE_USERLOGINLOCK_TIMEOUTSEC_DEFAULT	3600
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: Check password expiration.
 * </description>
 */
#define CMPUSERMGR_KEY_STRING_PASSWORD_EXPIRATION				CMPSETTINGS_CATEGORY_SECURITY ".Password.Expiration"
#define CMPUSERMGR_KEY_STRING_PASSWORD_EXPIRATION_DESC			"Check password expiration"

#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_ENABLED				"ENABLED"
#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_DISABLED			"DISABLED"

#ifndef CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_DEFAULT
	#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_DEFAULT	CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_DISABLED
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security setting: Check password expiration.
 * </description>
 */
typedef enum
{
	UserMgmtPasswordExpiration_Enabled,
	UserMgmtPasswordExpiration_Disabled
} SECURITY_USERMGR_PASSWORD_EXPIRATION;

#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_LIST	{\
												{(RTS_I32)UserMgmtPasswordExpiration_Enabled, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_ENABLED, "Check password expiration enabled."},\
												{(RTS_I32)UserMgmtPasswordExpiration_Disabled, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_DISABLED, "Check password expiration disabled."}\
											}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: Scope; User login password expiration is checked for admins, non-admins or all.
 *</description>
 */
#define CMPUSERMGR_KEY_STRING_PASSWORD_EXPIRATION_SCOPE					CMPSETTINGS_CATEGORY_SECURITY ".Password.Expiration.Scope"
#define CMPUSERMGR_KEY_STRING_PASSWORD_EXPIRATION_SCOPE_DESC			"Password expiration is checked for admins, non-admins or all"

#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_ADMINS				"ADMINS"
#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_NONADMINS			"NONADMINS"
#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_ALL					"ALL"

#ifndef CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_DEFAULT
	#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_DEFAULT	CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_ADMINS
#endif

/**
 * <category>SecuritySettings</category>
 * <description>Security settings: Scope; Password expiration is checked for admins, non-admins or all.
 * </description>
 */
typedef enum
{
	UserMgmtPasswordExpirationScope_Admins,
	UserMgmtPasswordExpirationScope_NonAdmins,
	UserMgmtPasswordExpirationScope_All
} SECURITY_USERMGR_PASSWORD_EXPIRATION_SCOPE;

#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_LIST	{\
													{(RTS_I32)UserMgmtPasswordExpirationScope_Admins, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_ADMINS, "Password expiration scope for admins."},\
													{(RTS_I32)UserMgmtPasswordExpirationScope_NonAdmins, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_NONADMINS, "Password expiration scope for non-admins."},\
													{(RTS_I32)UserMgmtPasswordExpirationScope_All, CMPSECMAN_SFLAGS_NONE, CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_SCOPE_ALL, "Password expiration scope for all."}\
												}

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: Password TimeoutDays; After how many days a new password is required [1..1000].
 *</description>
 */
#define CMPUSERMGR_KEY_INT_PASSWORD_EXPIRATION_TIMEOUTDAYS				CMPSETTINGS_CATEGORY_SECURITY ".Password.Expiration.TimeoutDays"
#define CMPUSERMGR_KEY_INT_PASSWORD_EXPIRATION_TIMEOUTDAYS_DESC			"After how many days a new password is required [1..1000]"

#ifndef CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_TIMEOUTDAYS_DEFAULT
	#define CMPUSERMGR_VALUE_PASSWORD_EXPIRATION_TIMEOUTDAYS_DEFAULT	100
#endif


/**
 * <category>Predefined user groups</category>
 * <description>Predefined user groups</description>
 * <element name="USERDB_GROUP_ADMINISTRATOR" type="IN">Administrator group. A member of this groups has all access rights on all objects!</element>
 * <element name="USERDB_GROUP_DEVELOPER" type="IN">Developer group. A member of this groups has all access to the IEC applications an all related stuff.</element>
 * <element name="USERDB_GROUP_SERVICE" type="IN">Service group. A member of this groups has all access to object for maintenance and service operations.</element>
 * <element name="USERDB_GROUP_WATCH" type="IN">Watch group. A member of this groups has read-only access to the objects.</element>
 * <element name="USERDB_GROUP_EVERYONE" type="IN">Everyone group. This group is reserved for special further purposes and not be added to the user management.</element>
 * <element name="USERDB_GROUP_ANONYMOUS_PREFIX" type="IN">Prefix for anonymous group (also used for user). This prefix is reserved for anonymous login.
 *	No group or user beginning with this prefix will be added to the user management.</element>
 * </element>
 */
#ifndef USERDB_GROUP_ADMINISTRATOR
	#define USERDB_GROUP_ADMINISTRATOR								"Administrator"
#endif
#ifndef USERDB_GROUP_DEVELOPER
	#define USERDB_GROUP_DEVELOPER									"Developer"
#endif
#ifndef USERDB_GROUP_SERVICE
	#define USERDB_GROUP_SERVICE									"Service"
#endif
#ifndef USERDB_GROUP_WATCH
	#define USERDB_GROUP_WATCH										"Watch"
#endif
#ifndef USERDB_GROUP_EVERYONE
	#define USERDB_GROUP_EVERYONE									"Everyone"
#endif
#ifndef USERDB_GROUP_ANONYMOUS_PREFIX
	#define USERDB_GROUP_ANONYMOUS_PREFIX							"Anonymous"
#endif

/**
 * <category>User rights</category>
 * <description>Defined rights that can fully control the access to an object</description>
 * <element name="USERDB_RIGHT_NONE" type="IN">Access denied</element>
 * <element name="USERDB_RIGHT_VIEW" type="IN">Only view rights on the object. Object cannot be modified in any way!</element>
 * <element name="USERDB_RIGHT_MODIFY" type="IN">Object can be viewed and modified</element>
 * <element name="USERDB_RIGHT_EXECUTE" type="IN">Object can be executed</element>
 * <element name="USERDB_RIGHT_ADD_REMOVE" type="IN">It is allowed to add and remove sub objects</element>
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
 * <category>Properties</category>
 * <description>These defines are used as properties of the UserDB Objects. Use as bit-mask.</description>
 * <element name="USERDB_HIDE_OBJECT">Hides the object from being uploaded to the IDE.</element>
 * <element name="USERDB_EXPLICIT_RIGHTS">Evaluates only the explicit configured rights on that object. Don't check the parent 
 *	objects if no rights are configured. If no explicit rights are found access is denied.</element>
 */
#define USERDB_HIDE_OBJECT             1
#define USERDB_EXPLICIT_RIGHTS         2

/**
 * <category>User and group properites</category>
 * <description>Properties for a user.</description>
 * <element name="USERMGR_ONLINE_PROP_PASSWORD_UP_TO_DATE" type="IN">The users password is up to date and must not be changed.</element>
 * <element name="USERMGR_ONLINE_PROP_PASSWORD_EDITABLE" type="IN">The user is allowed to change the password.</element>
 * <element name="USERMGR_ONLINE_PROP_ALL" type="IN">All properities set.</element>
 */
#define USERMGR_ONLINE_PROP_PASSWORD_UP_TO_DATE	UINT32_C(0x00001000)
#define USERMGR_ONLINE_PROP_PASSWORD_EDITABLE	UINT32_C(0x00002000)
#define USERMGR_ONLINE_PROP_ALL					UINT32_C(0xFFFFFF7F)

#define USERMGR_USER_PROP_PASSWORD_EXPIRED		UINT32_C(0x00001000)
#define USERMGR_USER_PROP_ALLOW_CHANGE_PASSWORD	UINT32_C(0x00002000)
#define USERMGR_USER_PROP_DEFAULT USERMGR_USER_PROP_ALLOW_CHANGE_PASSWORD | USERMGR_USER_PROP_PASSWORD_EXPIRED

#define USERMGR_PROP_ANONYMOUS					UINT32_C(0x00004000)

/**
 * <category>Login protocol types</category>
 * <description>
 *  There are different protocols that can be used to authenticae a user. The following list contains the supported 
 *	credential types of the runtime system. It is not required that a runtime implements all available type. The available
 *	types depend on the actual implementaion of different user authenticators.
 *	<ul>
 *		<li>UserMgrCredentials_Type_Password: This login protocol transmitts the users password in plaintext over a TLS authenticated connection. This method is suitable if the credentials have
 *			to be forwarded to other databases such as LDAP, OS, or an OEM user database.</li>
 *		<li>UserMgrCredentials_Type_AUCPACE: This login protocol checks the credentials without transmitting the users password to the PLC. 
 *			It is suitable for embedded devices that have the CmpCryptoEmbedded available. A secure session is generated during login which can be used to protect further data within the session.</li>
 *		<li>UserMgrCredentials_Type_X509: This login protocol uses client certificates to authenticate the user. 
 *			A Nonce is digitally signed by the client with the users private key. Only suitable for lager systems with an implementation of CmpCrypto and CmpX509Cert</li>
 *	</ul>
 * </description>
 */
#define UserMgrCredentials_Type_Password	UINT32_C(1)

typedef struct
{
	char *pszPassword;
} UserMgrCredentials_Password;

#if 0
typedef struct
{
	RtsByteString 	Nonce;
	RtsByteString 	SignedData;
	RTS_SIZE		noneOffset;
	RTS_HANDLE		hClientCertificate;
	RtsByteString 	ClientSignature;	
} CmpUserMgr_X509_Parameter;
#endif

typedef union
{
	UserMgrCredentials_Password pwd;
}UserMgrCredentials_Store;

typedef struct
{
	RTS_UI32 credentialsType;
	UserMgrCredentials_Store credential;
}UserMgrCredentials;

typedef struct
{
	char *pszObject;
	RTS_UI32 defaultRights;
	RTS_UI32 defaultDeniedRights;
}UserMgrAnonymousObjectRights;

#define USERMGR_DATAEXCHANGE_FOLDER "$UserMgr$"
#define USERMGR_USERGROUPS_EXCHANGE_FILE USERMGR_DATAEXCHANGE_FOLDER "/Users_Groups_export.dum2"
#define USERMGR_USERGROUPS_BACKUP_FILE USERMGR_DATAEXCHANGE_FOLDER "/Backup.dum2"

/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 * Enumeration: UserMgrTypeClass
 * </description>
 * <element name="USERMGRTYPECLASS_Type_I8" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_I16" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_I32" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_UI8" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_UI16" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_UI32" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_REAL32" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_REAL64" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_HANDLE" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_RESULT" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_SIZE" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_BOOL" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_UI64" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_I64" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_STRING" type=OUT></element>
 * <element name="USERMGRTYPECLASS_Type_WSTRING" type=OUT></element>
*/
#define USERMGRTYPECLASS_TYPE_I8    RTS_IEC_INT_C(0x0)
#define USERMGRTYPECLASS_TYPE_I16    RTS_IEC_INT_C(0x1)
#define USERMGRTYPECLASS_TYPE_I32    RTS_IEC_INT_C(0x2)
#define USERMGRTYPECLASS_TYPE_UI8    RTS_IEC_INT_C(0x3)
#define USERMGRTYPECLASS_TYPE_UI16    RTS_IEC_INT_C(0x4)
#define USERMGRTYPECLASS_TYPE_UI32    RTS_IEC_INT_C(0x5)
#define USERMGRTYPECLASS_TYPE_REAL32    RTS_IEC_INT_C(0x6)
#define USERMGRTYPECLASS_TYPE_REAL64    RTS_IEC_INT_C(0x7)
#define USERMGRTYPECLASS_TYPE_HANDLE    RTS_IEC_INT_C(0x8)
#define USERMGRTYPECLASS_TYPE_RESULT    RTS_IEC_INT_C(0x9)
#define USERMGRTYPECLASS_TYPE_SIZE    RTS_IEC_INT_C(0xA)
#define USERMGRTYPECLASS_TYPE_BOOL    RTS_IEC_INT_C(0xB)
#define USERMGRTYPECLASS_TYPE_UI64    RTS_IEC_INT_C(0xC)
#define USERMGRTYPECLASS_TYPE_I64    RTS_IEC_INT_C(0xD)
#define USERMGRTYPECLASS_TYPE_STRING    RTS_IEC_INT_C(0xE)
#define USERMGRTYPECLASS_TYPE_WSTRING    RTS_IEC_INT_C(0xF)
/* Typed enum definition */
#define USERMGRTYPECLASS    RTS_IEC_INT

/**
 * <description>
 * Structure: UserMgrValue
 * </description>
 * <element name="i8Value" type=INOUT></element>
 * <element name="i16Value" type=INOUT></element>
 * <element name="i32Value" type=INOUT></element>
 * <element name="ui8Value" type=INOUT></element>
 * <element name="ui16Value" type=INOUT></element>
 * <element name="ui32Value" type=INOUT></element>
 * <element name="real32Value" type=INOUT></element>
 * <element name="real64Value" type=INOUT></element>
 * <element name="handleValue" type=INOUT></element>
 * <element name="resultValue" type=INOUT></element>
 * <element name="sizeValue" type=INOUT></element>
 * <element name="boolValue" type=INOUT></element>
 * <element name="ui64Value" type=INOUT></element>
 * <element name="i64Value" type=INOUT></element>
 * <element name="stringValue" type=INOUT></element>
 * <element name="wstringValue" type=INOUT></element>
*/
typedef union
{
	RTS_IEC_SINT i8Value;
	RTS_IEC_INT i16Value;
	RTS_IEC_DINT i32Value;
	RTS_IEC_USINT ui8Value;
	RTS_IEC_UINT ui16Value;
	RTS_IEC_UDINT ui32Value;
	RTS_IEC_REAL real32Value;
	RTS_IEC_LREAL real64Value;
	RTS_IEC_HANDLE handleValue;
	RTS_IEC_RESULT resultValue;
	RTS_IEC_XWORD sizeValue;
	RTS_IEC_BOOL boolValue;
	RTS_IEC_ULINT ui64Value;
	RTS_IEC_LINT i64Value;
	RTS_IEC_STRING *stringValue;
	RTS_IEC_WSTRING *wstringValue;
} UserMgrValue;

/**
 * <description>
 * Structure: UserMgrCacheValue
 * </description>
 * <element name="typeClass" type=INOUT></element>
 * <element name="value" type=INOUT></element>
*/
typedef struct tagUserMgrCacheValue
{
	RTS_IEC_INT typeClass;
	UserMgrValue value;
} UserMgrCacheValue;

#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif



/**
 * <description>Register a user database to the user manager. Only one database can be registered. If not database is registred then 
 * the user management will be deactivated if allowed.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 * <param name="type" type="IN">The credentials type that the database supports.</param>
 */
RTS_RESULT CDECL UserMgrRegisterUserDB(RTS_HANDLE hDatabase, RTS_UI32 type);
typedef RTS_RESULT (CDECL * PFUSERMGRREGISTERUSERDB) (RTS_HANDLE hDatabase, RTS_UI32 type);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREGISTERUSERDB_NOTIMPLEMENTED)
	#define USE_UserMgrRegisterUserDB
	#define EXT_UserMgrRegisterUserDB
	#define GET_UserMgrRegisterUserDB(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRegisterUserDB(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRegisterUserDB  FALSE
	#define EXP_UserMgrRegisterUserDB  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRegisterUserDB
	#define EXT_UserMgrRegisterUserDB
	#define GET_UserMgrRegisterUserDB(fl)  CAL_CMGETAPI( "UserMgrRegisterUserDB" ) 
	#define CAL_UserMgrRegisterUserDB  UserMgrRegisterUserDB
	#define CHK_UserMgrRegisterUserDB  TRUE
	#define EXP_UserMgrRegisterUserDB  CAL_CMEXPAPI( "UserMgrRegisterUserDB" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRegisterUserDB
	#define EXT_UserMgrRegisterUserDB
	#define GET_UserMgrRegisterUserDB(fl)  CAL_CMGETAPI( "UserMgrRegisterUserDB" ) 
	#define CAL_UserMgrRegisterUserDB  UserMgrRegisterUserDB
	#define CHK_UserMgrRegisterUserDB  TRUE
	#define EXP_UserMgrRegisterUserDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserDB", (RTS_UINTPTR)UserMgrRegisterUserDB, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRegisterUserDB
	#define EXT_CmpUserMgrUserMgrRegisterUserDB
	#define GET_CmpUserMgrUserMgrRegisterUserDB  ERR_OK
	#define CAL_CmpUserMgrUserMgrRegisterUserDB pICmpUserMgr->IUserMgrRegisterUserDB
	#define CHK_CmpUserMgrUserMgrRegisterUserDB (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRegisterUserDB  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRegisterUserDB
	#define EXT_UserMgrRegisterUserDB
	#define GET_UserMgrRegisterUserDB(fl)  CAL_CMGETAPI( "UserMgrRegisterUserDB" ) 
	#define CAL_UserMgrRegisterUserDB pICmpUserMgr->IUserMgrRegisterUserDB
	#define CHK_UserMgrRegisterUserDB (pICmpUserMgr != NULL)
	#define EXP_UserMgrRegisterUserDB  CAL_CMEXPAPI( "UserMgrRegisterUserDB" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRegisterUserDB  PFUSERMGRREGISTERUSERDB pfUserMgrRegisterUserDB;
	#define EXT_UserMgrRegisterUserDB  extern PFUSERMGRREGISTERUSERDB pfUserMgrRegisterUserDB;
	#define GET_UserMgrRegisterUserDB(fl)  s_pfCMGetAPI2( "UserMgrRegisterUserDB", (RTS_VOID_FCTPTR *)&pfUserMgrRegisterUserDB, (fl), 0, 0)
	#define CAL_UserMgrRegisterUserDB  pfUserMgrRegisterUserDB
	#define CHK_UserMgrRegisterUserDB  (pfUserMgrRegisterUserDB != NULL)
	#define EXP_UserMgrRegisterUserDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserDB", (RTS_UINTPTR)UserMgrRegisterUserDB, 0, 0) 
#endif



/**
 * <description>Unregister a user database from the user manager.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 */
RTS_RESULT CDECL UserMgrUnregisterUserDB(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRUNREGISTERUSERDB) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUNREGISTERUSERDB_NOTIMPLEMENTED)
	#define USE_UserMgrUnregisterUserDB
	#define EXT_UserMgrUnregisterUserDB
	#define GET_UserMgrUnregisterUserDB(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUnregisterUserDB(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUnregisterUserDB  FALSE
	#define EXP_UserMgrUnregisterUserDB  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUnregisterUserDB
	#define EXT_UserMgrUnregisterUserDB
	#define GET_UserMgrUnregisterUserDB(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserDB" ) 
	#define CAL_UserMgrUnregisterUserDB  UserMgrUnregisterUserDB
	#define CHK_UserMgrUnregisterUserDB  TRUE
	#define EXP_UserMgrUnregisterUserDB  CAL_CMEXPAPI( "UserMgrUnregisterUserDB" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUnregisterUserDB
	#define EXT_UserMgrUnregisterUserDB
	#define GET_UserMgrUnregisterUserDB(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserDB" ) 
	#define CAL_UserMgrUnregisterUserDB  UserMgrUnregisterUserDB
	#define CHK_UserMgrUnregisterUserDB  TRUE
	#define EXP_UserMgrUnregisterUserDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserDB", (RTS_UINTPTR)UserMgrUnregisterUserDB, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUnregisterUserDB
	#define EXT_CmpUserMgrUserMgrUnregisterUserDB
	#define GET_CmpUserMgrUserMgrUnregisterUserDB  ERR_OK
	#define CAL_CmpUserMgrUserMgrUnregisterUserDB pICmpUserMgr->IUserMgrUnregisterUserDB
	#define CHK_CmpUserMgrUserMgrUnregisterUserDB (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUnregisterUserDB  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUnregisterUserDB
	#define EXT_UserMgrUnregisterUserDB
	#define GET_UserMgrUnregisterUserDB(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserDB" ) 
	#define CAL_UserMgrUnregisterUserDB pICmpUserMgr->IUserMgrUnregisterUserDB
	#define CHK_UserMgrUnregisterUserDB (pICmpUserMgr != NULL)
	#define EXP_UserMgrUnregisterUserDB  CAL_CMEXPAPI( "UserMgrUnregisterUserDB" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUnregisterUserDB  PFUSERMGRUNREGISTERUSERDB pfUserMgrUnregisterUserDB;
	#define EXT_UserMgrUnregisterUserDB  extern PFUSERMGRUNREGISTERUSERDB pfUserMgrUnregisterUserDB;
	#define GET_UserMgrUnregisterUserDB(fl)  s_pfCMGetAPI2( "UserMgrUnregisterUserDB", (RTS_VOID_FCTPTR *)&pfUserMgrUnregisterUserDB, (fl), 0, 0)
	#define CAL_UserMgrUnregisterUserDB  pfUserMgrUnregisterUserDB
	#define CHK_UserMgrUnregisterUserDB  (pfUserMgrUnregisterUserDB != NULL)
	#define EXP_UserMgrUnregisterUserDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserDB", (RTS_UINTPTR)UserMgrUnregisterUserDB, 0, 0) 
#endif




/**
 * <description>Register a configuration database. If this interface is registered the online services can change the user database.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 * <param name="type" type="IN">The credentials type that the database supports.</param>
 */
RTS_RESULT CDECL UserMgrRegisterUserDBConfig(RTS_HANDLE hDatabase, RTS_UI32 type);
typedef RTS_RESULT (CDECL * PFUSERMGRREGISTERUSERDBCONFIG) (RTS_HANDLE hDatabase, RTS_UI32 type);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREGISTERUSERDBCONFIG_NOTIMPLEMENTED)
	#define USE_UserMgrRegisterUserDBConfig
	#define EXT_UserMgrRegisterUserDBConfig
	#define GET_UserMgrRegisterUserDBConfig(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRegisterUserDBConfig(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRegisterUserDBConfig  FALSE
	#define EXP_UserMgrRegisterUserDBConfig  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRegisterUserDBConfig
	#define EXT_UserMgrRegisterUserDBConfig
	#define GET_UserMgrRegisterUserDBConfig(fl)  CAL_CMGETAPI( "UserMgrRegisterUserDBConfig" ) 
	#define CAL_UserMgrRegisterUserDBConfig  UserMgrRegisterUserDBConfig
	#define CHK_UserMgrRegisterUserDBConfig  TRUE
	#define EXP_UserMgrRegisterUserDBConfig  CAL_CMEXPAPI( "UserMgrRegisterUserDBConfig" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRegisterUserDBConfig
	#define EXT_UserMgrRegisterUserDBConfig
	#define GET_UserMgrRegisterUserDBConfig(fl)  CAL_CMGETAPI( "UserMgrRegisterUserDBConfig" ) 
	#define CAL_UserMgrRegisterUserDBConfig  UserMgrRegisterUserDBConfig
	#define CHK_UserMgrRegisterUserDBConfig  TRUE
	#define EXP_UserMgrRegisterUserDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserDBConfig", (RTS_UINTPTR)UserMgrRegisterUserDBConfig, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRegisterUserDBConfig
	#define EXT_CmpUserMgrUserMgrRegisterUserDBConfig
	#define GET_CmpUserMgrUserMgrRegisterUserDBConfig  ERR_OK
	#define CAL_CmpUserMgrUserMgrRegisterUserDBConfig pICmpUserMgr->IUserMgrRegisterUserDBConfig
	#define CHK_CmpUserMgrUserMgrRegisterUserDBConfig (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRegisterUserDBConfig  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRegisterUserDBConfig
	#define EXT_UserMgrRegisterUserDBConfig
	#define GET_UserMgrRegisterUserDBConfig(fl)  CAL_CMGETAPI( "UserMgrRegisterUserDBConfig" ) 
	#define CAL_UserMgrRegisterUserDBConfig pICmpUserMgr->IUserMgrRegisterUserDBConfig
	#define CHK_UserMgrRegisterUserDBConfig (pICmpUserMgr != NULL)
	#define EXP_UserMgrRegisterUserDBConfig  CAL_CMEXPAPI( "UserMgrRegisterUserDBConfig" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRegisterUserDBConfig  PFUSERMGRREGISTERUSERDBCONFIG pfUserMgrRegisterUserDBConfig;
	#define EXT_UserMgrRegisterUserDBConfig  extern PFUSERMGRREGISTERUSERDBCONFIG pfUserMgrRegisterUserDBConfig;
	#define GET_UserMgrRegisterUserDBConfig(fl)  s_pfCMGetAPI2( "UserMgrRegisterUserDBConfig", (RTS_VOID_FCTPTR *)&pfUserMgrRegisterUserDBConfig, (fl), 0, 0)
	#define CAL_UserMgrRegisterUserDBConfig  pfUserMgrRegisterUserDBConfig
	#define CHK_UserMgrRegisterUserDBConfig  (pfUserMgrRegisterUserDBConfig != NULL)
	#define EXP_UserMgrRegisterUserDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserDBConfig", (RTS_UINTPTR)UserMgrRegisterUserDBConfig, 0, 0) 
#endif




/**
 * <description>Unregister a configuration database.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 * <param name="type" type="IN">The credentials type that the database supports.</param>
 */
RTS_RESULT CDECL UserMgrUnregisterUserDBConfig(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRUNREGISTERUSERDBCONFIG) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUNREGISTERUSERDBCONFIG_NOTIMPLEMENTED)
	#define USE_UserMgrUnregisterUserDBConfig
	#define EXT_UserMgrUnregisterUserDBConfig
	#define GET_UserMgrUnregisterUserDBConfig(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUnregisterUserDBConfig(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUnregisterUserDBConfig  FALSE
	#define EXP_UserMgrUnregisterUserDBConfig  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUnregisterUserDBConfig
	#define EXT_UserMgrUnregisterUserDBConfig
	#define GET_UserMgrUnregisterUserDBConfig(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserDBConfig" ) 
	#define CAL_UserMgrUnregisterUserDBConfig  UserMgrUnregisterUserDBConfig
	#define CHK_UserMgrUnregisterUserDBConfig  TRUE
	#define EXP_UserMgrUnregisterUserDBConfig  CAL_CMEXPAPI( "UserMgrUnregisterUserDBConfig" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUnregisterUserDBConfig
	#define EXT_UserMgrUnregisterUserDBConfig
	#define GET_UserMgrUnregisterUserDBConfig(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserDBConfig" ) 
	#define CAL_UserMgrUnregisterUserDBConfig  UserMgrUnregisterUserDBConfig
	#define CHK_UserMgrUnregisterUserDBConfig  TRUE
	#define EXP_UserMgrUnregisterUserDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserDBConfig", (RTS_UINTPTR)UserMgrUnregisterUserDBConfig, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUnregisterUserDBConfig
	#define EXT_CmpUserMgrUserMgrUnregisterUserDBConfig
	#define GET_CmpUserMgrUserMgrUnregisterUserDBConfig  ERR_OK
	#define CAL_CmpUserMgrUserMgrUnregisterUserDBConfig pICmpUserMgr->IUserMgrUnregisterUserDBConfig
	#define CHK_CmpUserMgrUserMgrUnregisterUserDBConfig (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUnregisterUserDBConfig  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUnregisterUserDBConfig
	#define EXT_UserMgrUnregisterUserDBConfig
	#define GET_UserMgrUnregisterUserDBConfig(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserDBConfig" ) 
	#define CAL_UserMgrUnregisterUserDBConfig pICmpUserMgr->IUserMgrUnregisterUserDBConfig
	#define CHK_UserMgrUnregisterUserDBConfig (pICmpUserMgr != NULL)
	#define EXP_UserMgrUnregisterUserDBConfig  CAL_CMEXPAPI( "UserMgrUnregisterUserDBConfig" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUnregisterUserDBConfig  PFUSERMGRUNREGISTERUSERDBCONFIG pfUserMgrUnregisterUserDBConfig;
	#define EXT_UserMgrUnregisterUserDBConfig  extern PFUSERMGRUNREGISTERUSERDBCONFIG pfUserMgrUnregisterUserDBConfig;
	#define GET_UserMgrUnregisterUserDBConfig(fl)  s_pfCMGetAPI2( "UserMgrUnregisterUserDBConfig", (RTS_VOID_FCTPTR *)&pfUserMgrUnregisterUserDBConfig, (fl), 0, 0)
	#define CAL_UserMgrUnregisterUserDBConfig  pfUserMgrUnregisterUserDBConfig
	#define CHK_UserMgrUnregisterUserDBConfig  (pfUserMgrUnregisterUserDBConfig != NULL)
	#define EXP_UserMgrUnregisterUserDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserDBConfig", (RTS_UINTPTR)UserMgrUnregisterUserDBConfig, 0, 0) 
#endif




/**
 * <description>Register user group database.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 */
RTS_RESULT CDECL UserMgrRegisterUserGroupsDB(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRREGISTERUSERGROUPSDB) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREGISTERUSERGROUPSDB_NOTIMPLEMENTED)
	#define USE_UserMgrRegisterUserGroupsDB
	#define EXT_UserMgrRegisterUserGroupsDB
	#define GET_UserMgrRegisterUserGroupsDB(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRegisterUserGroupsDB(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRegisterUserGroupsDB  FALSE
	#define EXP_UserMgrRegisterUserGroupsDB  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRegisterUserGroupsDB
	#define EXT_UserMgrRegisterUserGroupsDB
	#define GET_UserMgrRegisterUserGroupsDB(fl)  CAL_CMGETAPI( "UserMgrRegisterUserGroupsDB" ) 
	#define CAL_UserMgrRegisterUserGroupsDB  UserMgrRegisterUserGroupsDB
	#define CHK_UserMgrRegisterUserGroupsDB  TRUE
	#define EXP_UserMgrRegisterUserGroupsDB  CAL_CMEXPAPI( "UserMgrRegisterUserGroupsDB" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRegisterUserGroupsDB
	#define EXT_UserMgrRegisterUserGroupsDB
	#define GET_UserMgrRegisterUserGroupsDB(fl)  CAL_CMGETAPI( "UserMgrRegisterUserGroupsDB" ) 
	#define CAL_UserMgrRegisterUserGroupsDB  UserMgrRegisterUserGroupsDB
	#define CHK_UserMgrRegisterUserGroupsDB  TRUE
	#define EXP_UserMgrRegisterUserGroupsDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserGroupsDB", (RTS_UINTPTR)UserMgrRegisterUserGroupsDB, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRegisterUserGroupsDB
	#define EXT_CmpUserMgrUserMgrRegisterUserGroupsDB
	#define GET_CmpUserMgrUserMgrRegisterUserGroupsDB  ERR_OK
	#define CAL_CmpUserMgrUserMgrRegisterUserGroupsDB pICmpUserMgr->IUserMgrRegisterUserGroupsDB
	#define CHK_CmpUserMgrUserMgrRegisterUserGroupsDB (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRegisterUserGroupsDB  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRegisterUserGroupsDB
	#define EXT_UserMgrRegisterUserGroupsDB
	#define GET_UserMgrRegisterUserGroupsDB(fl)  CAL_CMGETAPI( "UserMgrRegisterUserGroupsDB" ) 
	#define CAL_UserMgrRegisterUserGroupsDB pICmpUserMgr->IUserMgrRegisterUserGroupsDB
	#define CHK_UserMgrRegisterUserGroupsDB (pICmpUserMgr != NULL)
	#define EXP_UserMgrRegisterUserGroupsDB  CAL_CMEXPAPI( "UserMgrRegisterUserGroupsDB" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRegisterUserGroupsDB  PFUSERMGRREGISTERUSERGROUPSDB pfUserMgrRegisterUserGroupsDB;
	#define EXT_UserMgrRegisterUserGroupsDB  extern PFUSERMGRREGISTERUSERGROUPSDB pfUserMgrRegisterUserGroupsDB;
	#define GET_UserMgrRegisterUserGroupsDB(fl)  s_pfCMGetAPI2( "UserMgrRegisterUserGroupsDB", (RTS_VOID_FCTPTR *)&pfUserMgrRegisterUserGroupsDB, (fl), 0, 0)
	#define CAL_UserMgrRegisterUserGroupsDB  pfUserMgrRegisterUserGroupsDB
	#define CHK_UserMgrRegisterUserGroupsDB  (pfUserMgrRegisterUserGroupsDB != NULL)
	#define EXP_UserMgrRegisterUserGroupsDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserGroupsDB", (RTS_UINTPTR)UserMgrRegisterUserGroupsDB, 0, 0) 
#endif




/**
 * <description>Unregister user group database.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 */
RTS_RESULT CDECL UserMgrUnregisterUserGroupsDB(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRUNREGISTERUSERGROUPSDB) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUNREGISTERUSERGROUPSDB_NOTIMPLEMENTED)
	#define USE_UserMgrUnregisterUserGroupsDB
	#define EXT_UserMgrUnregisterUserGroupsDB
	#define GET_UserMgrUnregisterUserGroupsDB(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUnregisterUserGroupsDB(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUnregisterUserGroupsDB  FALSE
	#define EXP_UserMgrUnregisterUserGroupsDB  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUnregisterUserGroupsDB
	#define EXT_UserMgrUnregisterUserGroupsDB
	#define GET_UserMgrUnregisterUserGroupsDB(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserGroupsDB" ) 
	#define CAL_UserMgrUnregisterUserGroupsDB  UserMgrUnregisterUserGroupsDB
	#define CHK_UserMgrUnregisterUserGroupsDB  TRUE
	#define EXP_UserMgrUnregisterUserGroupsDB  CAL_CMEXPAPI( "UserMgrUnregisterUserGroupsDB" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUnregisterUserGroupsDB
	#define EXT_UserMgrUnregisterUserGroupsDB
	#define GET_UserMgrUnregisterUserGroupsDB(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserGroupsDB" ) 
	#define CAL_UserMgrUnregisterUserGroupsDB  UserMgrUnregisterUserGroupsDB
	#define CHK_UserMgrUnregisterUserGroupsDB  TRUE
	#define EXP_UserMgrUnregisterUserGroupsDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserGroupsDB", (RTS_UINTPTR)UserMgrUnregisterUserGroupsDB, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUnregisterUserGroupsDB
	#define EXT_CmpUserMgrUserMgrUnregisterUserGroupsDB
	#define GET_CmpUserMgrUserMgrUnregisterUserGroupsDB  ERR_OK
	#define CAL_CmpUserMgrUserMgrUnregisterUserGroupsDB pICmpUserMgr->IUserMgrUnregisterUserGroupsDB
	#define CHK_CmpUserMgrUserMgrUnregisterUserGroupsDB (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUnregisterUserGroupsDB  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUnregisterUserGroupsDB
	#define EXT_UserMgrUnregisterUserGroupsDB
	#define GET_UserMgrUnregisterUserGroupsDB(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserGroupsDB" ) 
	#define CAL_UserMgrUnregisterUserGroupsDB pICmpUserMgr->IUserMgrUnregisterUserGroupsDB
	#define CHK_UserMgrUnregisterUserGroupsDB (pICmpUserMgr != NULL)
	#define EXP_UserMgrUnregisterUserGroupsDB  CAL_CMEXPAPI( "UserMgrUnregisterUserGroupsDB" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUnregisterUserGroupsDB  PFUSERMGRUNREGISTERUSERGROUPSDB pfUserMgrUnregisterUserGroupsDB;
	#define EXT_UserMgrUnregisterUserGroupsDB  extern PFUSERMGRUNREGISTERUSERGROUPSDB pfUserMgrUnregisterUserGroupsDB;
	#define GET_UserMgrUnregisterUserGroupsDB(fl)  s_pfCMGetAPI2( "UserMgrUnregisterUserGroupsDB", (RTS_VOID_FCTPTR *)&pfUserMgrUnregisterUserGroupsDB, (fl), 0, 0)
	#define CAL_UserMgrUnregisterUserGroupsDB  pfUserMgrUnregisterUserGroupsDB
	#define CHK_UserMgrUnregisterUserGroupsDB  (pfUserMgrUnregisterUserGroupsDB != NULL)
	#define EXP_UserMgrUnregisterUserGroupsDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserGroupsDB", (RTS_UINTPTR)UserMgrUnregisterUserGroupsDB, 0, 0) 
#endif




/**
 * <description>Register user group configuration database.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 */
RTS_RESULT CDECL UserMgrRegisterUserGroupsDBConfig(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRREGISTERUSERGROUPSDBCONFIG) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREGISTERUSERGROUPSDBCONFIG_NOTIMPLEMENTED)
	#define USE_UserMgrRegisterUserGroupsDBConfig
	#define EXT_UserMgrRegisterUserGroupsDBConfig
	#define GET_UserMgrRegisterUserGroupsDBConfig(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRegisterUserGroupsDBConfig(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRegisterUserGroupsDBConfig  FALSE
	#define EXP_UserMgrRegisterUserGroupsDBConfig  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRegisterUserGroupsDBConfig
	#define EXT_UserMgrRegisterUserGroupsDBConfig
	#define GET_UserMgrRegisterUserGroupsDBConfig(fl)  CAL_CMGETAPI( "UserMgrRegisterUserGroupsDBConfig" ) 
	#define CAL_UserMgrRegisterUserGroupsDBConfig  UserMgrRegisterUserGroupsDBConfig
	#define CHK_UserMgrRegisterUserGroupsDBConfig  TRUE
	#define EXP_UserMgrRegisterUserGroupsDBConfig  CAL_CMEXPAPI( "UserMgrRegisterUserGroupsDBConfig" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRegisterUserGroupsDBConfig
	#define EXT_UserMgrRegisterUserGroupsDBConfig
	#define GET_UserMgrRegisterUserGroupsDBConfig(fl)  CAL_CMGETAPI( "UserMgrRegisterUserGroupsDBConfig" ) 
	#define CAL_UserMgrRegisterUserGroupsDBConfig  UserMgrRegisterUserGroupsDBConfig
	#define CHK_UserMgrRegisterUserGroupsDBConfig  TRUE
	#define EXP_UserMgrRegisterUserGroupsDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserGroupsDBConfig", (RTS_UINTPTR)UserMgrRegisterUserGroupsDBConfig, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRegisterUserGroupsDBConfig
	#define EXT_CmpUserMgrUserMgrRegisterUserGroupsDBConfig
	#define GET_CmpUserMgrUserMgrRegisterUserGroupsDBConfig  ERR_OK
	#define CAL_CmpUserMgrUserMgrRegisterUserGroupsDBConfig pICmpUserMgr->IUserMgrRegisterUserGroupsDBConfig
	#define CHK_CmpUserMgrUserMgrRegisterUserGroupsDBConfig (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRegisterUserGroupsDBConfig  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRegisterUserGroupsDBConfig
	#define EXT_UserMgrRegisterUserGroupsDBConfig
	#define GET_UserMgrRegisterUserGroupsDBConfig(fl)  CAL_CMGETAPI( "UserMgrRegisterUserGroupsDBConfig" ) 
	#define CAL_UserMgrRegisterUserGroupsDBConfig pICmpUserMgr->IUserMgrRegisterUserGroupsDBConfig
	#define CHK_UserMgrRegisterUserGroupsDBConfig (pICmpUserMgr != NULL)
	#define EXP_UserMgrRegisterUserGroupsDBConfig  CAL_CMEXPAPI( "UserMgrRegisterUserGroupsDBConfig" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRegisterUserGroupsDBConfig  PFUSERMGRREGISTERUSERGROUPSDBCONFIG pfUserMgrRegisterUserGroupsDBConfig;
	#define EXT_UserMgrRegisterUserGroupsDBConfig  extern PFUSERMGRREGISTERUSERGROUPSDBCONFIG pfUserMgrRegisterUserGroupsDBConfig;
	#define GET_UserMgrRegisterUserGroupsDBConfig(fl)  s_pfCMGetAPI2( "UserMgrRegisterUserGroupsDBConfig", (RTS_VOID_FCTPTR *)&pfUserMgrRegisterUserGroupsDBConfig, (fl), 0, 0)
	#define CAL_UserMgrRegisterUserGroupsDBConfig  pfUserMgrRegisterUserGroupsDBConfig
	#define CHK_UserMgrRegisterUserGroupsDBConfig  (pfUserMgrRegisterUserGroupsDBConfig != NULL)
	#define EXP_UserMgrRegisterUserGroupsDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserGroupsDBConfig", (RTS_UINTPTR)UserMgrRegisterUserGroupsDBConfig, 0, 0) 
#endif




/**
 * <description>Unregister user group configuration database.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 */
RTS_RESULT CDECL UserMgrUnregisterUserGroupsDBConfig(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRUNREGISTERUSERGROUPSDBCONFIG) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUNREGISTERUSERGROUPSDBCONFIG_NOTIMPLEMENTED)
	#define USE_UserMgrUnregisterUserGroupsDBConfig
	#define EXT_UserMgrUnregisterUserGroupsDBConfig
	#define GET_UserMgrUnregisterUserGroupsDBConfig(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUnregisterUserGroupsDBConfig(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUnregisterUserGroupsDBConfig  FALSE
	#define EXP_UserMgrUnregisterUserGroupsDBConfig  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUnregisterUserGroupsDBConfig
	#define EXT_UserMgrUnregisterUserGroupsDBConfig
	#define GET_UserMgrUnregisterUserGroupsDBConfig(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserGroupsDBConfig" ) 
	#define CAL_UserMgrUnregisterUserGroupsDBConfig  UserMgrUnregisterUserGroupsDBConfig
	#define CHK_UserMgrUnregisterUserGroupsDBConfig  TRUE
	#define EXP_UserMgrUnregisterUserGroupsDBConfig  CAL_CMEXPAPI( "UserMgrUnregisterUserGroupsDBConfig" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUnregisterUserGroupsDBConfig
	#define EXT_UserMgrUnregisterUserGroupsDBConfig
	#define GET_UserMgrUnregisterUserGroupsDBConfig(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserGroupsDBConfig" ) 
	#define CAL_UserMgrUnregisterUserGroupsDBConfig  UserMgrUnregisterUserGroupsDBConfig
	#define CHK_UserMgrUnregisterUserGroupsDBConfig  TRUE
	#define EXP_UserMgrUnregisterUserGroupsDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserGroupsDBConfig", (RTS_UINTPTR)UserMgrUnregisterUserGroupsDBConfig, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUnregisterUserGroupsDBConfig
	#define EXT_CmpUserMgrUserMgrUnregisterUserGroupsDBConfig
	#define GET_CmpUserMgrUserMgrUnregisterUserGroupsDBConfig  ERR_OK
	#define CAL_CmpUserMgrUserMgrUnregisterUserGroupsDBConfig pICmpUserMgr->IUserMgrUnregisterUserGroupsDBConfig
	#define CHK_CmpUserMgrUserMgrUnregisterUserGroupsDBConfig (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUnregisterUserGroupsDBConfig  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUnregisterUserGroupsDBConfig
	#define EXT_UserMgrUnregisterUserGroupsDBConfig
	#define GET_UserMgrUnregisterUserGroupsDBConfig(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserGroupsDBConfig" ) 
	#define CAL_UserMgrUnregisterUserGroupsDBConfig pICmpUserMgr->IUserMgrUnregisterUserGroupsDBConfig
	#define CHK_UserMgrUnregisterUserGroupsDBConfig (pICmpUserMgr != NULL)
	#define EXP_UserMgrUnregisterUserGroupsDBConfig  CAL_CMEXPAPI( "UserMgrUnregisterUserGroupsDBConfig" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUnregisterUserGroupsDBConfig  PFUSERMGRUNREGISTERUSERGROUPSDBCONFIG pfUserMgrUnregisterUserGroupsDBConfig;
	#define EXT_UserMgrUnregisterUserGroupsDBConfig  extern PFUSERMGRUNREGISTERUSERGROUPSDBCONFIG pfUserMgrUnregisterUserGroupsDBConfig;
	#define GET_UserMgrUnregisterUserGroupsDBConfig(fl)  s_pfCMGetAPI2( "UserMgrUnregisterUserGroupsDBConfig", (RTS_VOID_FCTPTR *)&pfUserMgrUnregisterUserGroupsDBConfig, (fl), 0, 0)
	#define CAL_UserMgrUnregisterUserGroupsDBConfig  pfUserMgrUnregisterUserGroupsDBConfig
	#define CHK_UserMgrUnregisterUserGroupsDBConfig  (pfUserMgrUnregisterUserGroupsDBConfig != NULL)
	#define EXP_UserMgrUnregisterUserGroupsDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserGroupsDBConfig", (RTS_UINTPTR)UserMgrUnregisterUserGroupsDBConfig, 0, 0) 
#endif




/**
 * <description>Register user objects database.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 */
RTS_RESULT CDECL UserMgrRegisterUserObjectsDB(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRREGISTERUSEROBJECTSDB) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREGISTERUSEROBJECTSDB_NOTIMPLEMENTED)
	#define USE_UserMgrRegisterUserObjectsDB
	#define EXT_UserMgrRegisterUserObjectsDB
	#define GET_UserMgrRegisterUserObjectsDB(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRegisterUserObjectsDB(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRegisterUserObjectsDB  FALSE
	#define EXP_UserMgrRegisterUserObjectsDB  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRegisterUserObjectsDB
	#define EXT_UserMgrRegisterUserObjectsDB
	#define GET_UserMgrRegisterUserObjectsDB(fl)  CAL_CMGETAPI( "UserMgrRegisterUserObjectsDB" ) 
	#define CAL_UserMgrRegisterUserObjectsDB  UserMgrRegisterUserObjectsDB
	#define CHK_UserMgrRegisterUserObjectsDB  TRUE
	#define EXP_UserMgrRegisterUserObjectsDB  CAL_CMEXPAPI( "UserMgrRegisterUserObjectsDB" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRegisterUserObjectsDB
	#define EXT_UserMgrRegisterUserObjectsDB
	#define GET_UserMgrRegisterUserObjectsDB(fl)  CAL_CMGETAPI( "UserMgrRegisterUserObjectsDB" ) 
	#define CAL_UserMgrRegisterUserObjectsDB  UserMgrRegisterUserObjectsDB
	#define CHK_UserMgrRegisterUserObjectsDB  TRUE
	#define EXP_UserMgrRegisterUserObjectsDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserObjectsDB", (RTS_UINTPTR)UserMgrRegisterUserObjectsDB, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRegisterUserObjectsDB
	#define EXT_CmpUserMgrUserMgrRegisterUserObjectsDB
	#define GET_CmpUserMgrUserMgrRegisterUserObjectsDB  ERR_OK
	#define CAL_CmpUserMgrUserMgrRegisterUserObjectsDB pICmpUserMgr->IUserMgrRegisterUserObjectsDB
	#define CHK_CmpUserMgrUserMgrRegisterUserObjectsDB (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRegisterUserObjectsDB  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRegisterUserObjectsDB
	#define EXT_UserMgrRegisterUserObjectsDB
	#define GET_UserMgrRegisterUserObjectsDB(fl)  CAL_CMGETAPI( "UserMgrRegisterUserObjectsDB" ) 
	#define CAL_UserMgrRegisterUserObjectsDB pICmpUserMgr->IUserMgrRegisterUserObjectsDB
	#define CHK_UserMgrRegisterUserObjectsDB (pICmpUserMgr != NULL)
	#define EXP_UserMgrRegisterUserObjectsDB  CAL_CMEXPAPI( "UserMgrRegisterUserObjectsDB" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRegisterUserObjectsDB  PFUSERMGRREGISTERUSEROBJECTSDB pfUserMgrRegisterUserObjectsDB;
	#define EXT_UserMgrRegisterUserObjectsDB  extern PFUSERMGRREGISTERUSEROBJECTSDB pfUserMgrRegisterUserObjectsDB;
	#define GET_UserMgrRegisterUserObjectsDB(fl)  s_pfCMGetAPI2( "UserMgrRegisterUserObjectsDB", (RTS_VOID_FCTPTR *)&pfUserMgrRegisterUserObjectsDB, (fl), 0, 0)
	#define CAL_UserMgrRegisterUserObjectsDB  pfUserMgrRegisterUserObjectsDB
	#define CHK_UserMgrRegisterUserObjectsDB  (pfUserMgrRegisterUserObjectsDB != NULL)
	#define EXP_UserMgrRegisterUserObjectsDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserObjectsDB", (RTS_UINTPTR)UserMgrRegisterUserObjectsDB, 0, 0) 
#endif




/**
 * <description>Unregister user objects database.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 */
RTS_RESULT CDECL UserMgrUnregisterUserObjectsDB(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRUNREGISTERUSEROBJECTSDB) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUNREGISTERUSEROBJECTSDB_NOTIMPLEMENTED)
	#define USE_UserMgrUnregisterUserObjectsDB
	#define EXT_UserMgrUnregisterUserObjectsDB
	#define GET_UserMgrUnregisterUserObjectsDB(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUnregisterUserObjectsDB(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUnregisterUserObjectsDB  FALSE
	#define EXP_UserMgrUnregisterUserObjectsDB  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUnregisterUserObjectsDB
	#define EXT_UserMgrUnregisterUserObjectsDB
	#define GET_UserMgrUnregisterUserObjectsDB(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserObjectsDB" ) 
	#define CAL_UserMgrUnregisterUserObjectsDB  UserMgrUnregisterUserObjectsDB
	#define CHK_UserMgrUnregisterUserObjectsDB  TRUE
	#define EXP_UserMgrUnregisterUserObjectsDB  CAL_CMEXPAPI( "UserMgrUnregisterUserObjectsDB" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUnregisterUserObjectsDB
	#define EXT_UserMgrUnregisterUserObjectsDB
	#define GET_UserMgrUnregisterUserObjectsDB(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserObjectsDB" ) 
	#define CAL_UserMgrUnregisterUserObjectsDB  UserMgrUnregisterUserObjectsDB
	#define CHK_UserMgrUnregisterUserObjectsDB  TRUE
	#define EXP_UserMgrUnregisterUserObjectsDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserObjectsDB", (RTS_UINTPTR)UserMgrUnregisterUserObjectsDB, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUnregisterUserObjectsDB
	#define EXT_CmpUserMgrUserMgrUnregisterUserObjectsDB
	#define GET_CmpUserMgrUserMgrUnregisterUserObjectsDB  ERR_OK
	#define CAL_CmpUserMgrUserMgrUnregisterUserObjectsDB pICmpUserMgr->IUserMgrUnregisterUserObjectsDB
	#define CHK_CmpUserMgrUserMgrUnregisterUserObjectsDB (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUnregisterUserObjectsDB  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUnregisterUserObjectsDB
	#define EXT_UserMgrUnregisterUserObjectsDB
	#define GET_UserMgrUnregisterUserObjectsDB(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserObjectsDB" ) 
	#define CAL_UserMgrUnregisterUserObjectsDB pICmpUserMgr->IUserMgrUnregisterUserObjectsDB
	#define CHK_UserMgrUnregisterUserObjectsDB (pICmpUserMgr != NULL)
	#define EXP_UserMgrUnregisterUserObjectsDB  CAL_CMEXPAPI( "UserMgrUnregisterUserObjectsDB" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUnregisterUserObjectsDB  PFUSERMGRUNREGISTERUSEROBJECTSDB pfUserMgrUnregisterUserObjectsDB;
	#define EXT_UserMgrUnregisterUserObjectsDB  extern PFUSERMGRUNREGISTERUSEROBJECTSDB pfUserMgrUnregisterUserObjectsDB;
	#define GET_UserMgrUnregisterUserObjectsDB(fl)  s_pfCMGetAPI2( "UserMgrUnregisterUserObjectsDB", (RTS_VOID_FCTPTR *)&pfUserMgrUnregisterUserObjectsDB, (fl), 0, 0)
	#define CAL_UserMgrUnregisterUserObjectsDB  pfUserMgrUnregisterUserObjectsDB
	#define CHK_UserMgrUnregisterUserObjectsDB  (pfUserMgrUnregisterUserObjectsDB != NULL)
	#define EXP_UserMgrUnregisterUserObjectsDB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserObjectsDB", (RTS_UINTPTR)UserMgrUnregisterUserObjectsDB, 0, 0) 
#endif




/**
 * <description>Register user objects database configuration.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 */
RTS_RESULT CDECL UserMgrRegisterUserObjectsDBConfig(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRREGISTERUSEROBJECTSDBCONFIG) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREGISTERUSEROBJECTSDBCONFIG_NOTIMPLEMENTED)
	#define USE_UserMgrRegisterUserObjectsDBConfig
	#define EXT_UserMgrRegisterUserObjectsDBConfig
	#define GET_UserMgrRegisterUserObjectsDBConfig(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRegisterUserObjectsDBConfig(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRegisterUserObjectsDBConfig  FALSE
	#define EXP_UserMgrRegisterUserObjectsDBConfig  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRegisterUserObjectsDBConfig
	#define EXT_UserMgrRegisterUserObjectsDBConfig
	#define GET_UserMgrRegisterUserObjectsDBConfig(fl)  CAL_CMGETAPI( "UserMgrRegisterUserObjectsDBConfig" ) 
	#define CAL_UserMgrRegisterUserObjectsDBConfig  UserMgrRegisterUserObjectsDBConfig
	#define CHK_UserMgrRegisterUserObjectsDBConfig  TRUE
	#define EXP_UserMgrRegisterUserObjectsDBConfig  CAL_CMEXPAPI( "UserMgrRegisterUserObjectsDBConfig" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRegisterUserObjectsDBConfig
	#define EXT_UserMgrRegisterUserObjectsDBConfig
	#define GET_UserMgrRegisterUserObjectsDBConfig(fl)  CAL_CMGETAPI( "UserMgrRegisterUserObjectsDBConfig" ) 
	#define CAL_UserMgrRegisterUserObjectsDBConfig  UserMgrRegisterUserObjectsDBConfig
	#define CHK_UserMgrRegisterUserObjectsDBConfig  TRUE
	#define EXP_UserMgrRegisterUserObjectsDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserObjectsDBConfig", (RTS_UINTPTR)UserMgrRegisterUserObjectsDBConfig, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRegisterUserObjectsDBConfig
	#define EXT_CmpUserMgrUserMgrRegisterUserObjectsDBConfig
	#define GET_CmpUserMgrUserMgrRegisterUserObjectsDBConfig  ERR_OK
	#define CAL_CmpUserMgrUserMgrRegisterUserObjectsDBConfig pICmpUserMgr->IUserMgrRegisterUserObjectsDBConfig
	#define CHK_CmpUserMgrUserMgrRegisterUserObjectsDBConfig (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRegisterUserObjectsDBConfig  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRegisterUserObjectsDBConfig
	#define EXT_UserMgrRegisterUserObjectsDBConfig
	#define GET_UserMgrRegisterUserObjectsDBConfig(fl)  CAL_CMGETAPI( "UserMgrRegisterUserObjectsDBConfig" ) 
	#define CAL_UserMgrRegisterUserObjectsDBConfig pICmpUserMgr->IUserMgrRegisterUserObjectsDBConfig
	#define CHK_UserMgrRegisterUserObjectsDBConfig (pICmpUserMgr != NULL)
	#define EXP_UserMgrRegisterUserObjectsDBConfig  CAL_CMEXPAPI( "UserMgrRegisterUserObjectsDBConfig" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRegisterUserObjectsDBConfig  PFUSERMGRREGISTERUSEROBJECTSDBCONFIG pfUserMgrRegisterUserObjectsDBConfig;
	#define EXT_UserMgrRegisterUserObjectsDBConfig  extern PFUSERMGRREGISTERUSEROBJECTSDBCONFIG pfUserMgrRegisterUserObjectsDBConfig;
	#define GET_UserMgrRegisterUserObjectsDBConfig(fl)  s_pfCMGetAPI2( "UserMgrRegisterUserObjectsDBConfig", (RTS_VOID_FCTPTR *)&pfUserMgrRegisterUserObjectsDBConfig, (fl), 0, 0)
	#define CAL_UserMgrRegisterUserObjectsDBConfig  pfUserMgrRegisterUserObjectsDBConfig
	#define CHK_UserMgrRegisterUserObjectsDBConfig  (pfUserMgrRegisterUserObjectsDBConfig != NULL)
	#define EXP_UserMgrRegisterUserObjectsDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterUserObjectsDBConfig", (RTS_UINTPTR)UserMgrRegisterUserObjectsDBConfig, 0, 0) 
#endif




/**
 * <description>Unregister user objects database configuration.</description>
 * <param name="hDatabase" type="IN">Handle to the database.</param>
 */
RTS_RESULT CDECL UserMgrUnregisterUserObjectsDBConfig(RTS_HANDLE hDatabase);
typedef RTS_RESULT (CDECL * PFUSERMGRUNREGISTERUSEROBJECTSDBCONFIG) (RTS_HANDLE hDatabase);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUNREGISTERUSEROBJECTSDBCONFIG_NOTIMPLEMENTED)
	#define USE_UserMgrUnregisterUserObjectsDBConfig
	#define EXT_UserMgrUnregisterUserObjectsDBConfig
	#define GET_UserMgrUnregisterUserObjectsDBConfig(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUnregisterUserObjectsDBConfig(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUnregisterUserObjectsDBConfig  FALSE
	#define EXP_UserMgrUnregisterUserObjectsDBConfig  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUnregisterUserObjectsDBConfig
	#define EXT_UserMgrUnregisterUserObjectsDBConfig
	#define GET_UserMgrUnregisterUserObjectsDBConfig(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserObjectsDBConfig" ) 
	#define CAL_UserMgrUnregisterUserObjectsDBConfig  UserMgrUnregisterUserObjectsDBConfig
	#define CHK_UserMgrUnregisterUserObjectsDBConfig  TRUE
	#define EXP_UserMgrUnregisterUserObjectsDBConfig  CAL_CMEXPAPI( "UserMgrUnregisterUserObjectsDBConfig" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUnregisterUserObjectsDBConfig
	#define EXT_UserMgrUnregisterUserObjectsDBConfig
	#define GET_UserMgrUnregisterUserObjectsDBConfig(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserObjectsDBConfig" ) 
	#define CAL_UserMgrUnregisterUserObjectsDBConfig  UserMgrUnregisterUserObjectsDBConfig
	#define CHK_UserMgrUnregisterUserObjectsDBConfig  TRUE
	#define EXP_UserMgrUnregisterUserObjectsDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserObjectsDBConfig", (RTS_UINTPTR)UserMgrUnregisterUserObjectsDBConfig, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUnregisterUserObjectsDBConfig
	#define EXT_CmpUserMgrUserMgrUnregisterUserObjectsDBConfig
	#define GET_CmpUserMgrUserMgrUnregisterUserObjectsDBConfig  ERR_OK
	#define CAL_CmpUserMgrUserMgrUnregisterUserObjectsDBConfig pICmpUserMgr->IUserMgrUnregisterUserObjectsDBConfig
	#define CHK_CmpUserMgrUserMgrUnregisterUserObjectsDBConfig (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUnregisterUserObjectsDBConfig  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUnregisterUserObjectsDBConfig
	#define EXT_UserMgrUnregisterUserObjectsDBConfig
	#define GET_UserMgrUnregisterUserObjectsDBConfig(fl)  CAL_CMGETAPI( "UserMgrUnregisterUserObjectsDBConfig" ) 
	#define CAL_UserMgrUnregisterUserObjectsDBConfig pICmpUserMgr->IUserMgrUnregisterUserObjectsDBConfig
	#define CHK_UserMgrUnregisterUserObjectsDBConfig (pICmpUserMgr != NULL)
	#define EXP_UserMgrUnregisterUserObjectsDBConfig  CAL_CMEXPAPI( "UserMgrUnregisterUserObjectsDBConfig" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUnregisterUserObjectsDBConfig  PFUSERMGRUNREGISTERUSEROBJECTSDBCONFIG pfUserMgrUnregisterUserObjectsDBConfig;
	#define EXT_UserMgrUnregisterUserObjectsDBConfig  extern PFUSERMGRUNREGISTERUSEROBJECTSDBCONFIG pfUserMgrUnregisterUserObjectsDBConfig;
	#define GET_UserMgrUnregisterUserObjectsDBConfig(fl)  s_pfCMGetAPI2( "UserMgrUnregisterUserObjectsDBConfig", (RTS_VOID_FCTPTR *)&pfUserMgrUnregisterUserObjectsDBConfig, (fl), 0, 0)
	#define CAL_UserMgrUnregisterUserObjectsDBConfig  pfUserMgrUnregisterUserObjectsDBConfig
	#define CHK_UserMgrUnregisterUserObjectsDBConfig  (pfUserMgrUnregisterUserObjectsDBConfig != NULL)
	#define EXP_UserMgrUnregisterUserObjectsDBConfig  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterUserObjectsDBConfig", (RTS_UINTPTR)UserMgrUnregisterUserObjectsDBConfig, 0, 0) 
#endif




/**
 * <description>Selects the authentication type out of the client proposals. Only to be called by CmpDevice.</description>
 * <param name="ui32ClientFlags" type="IN">Flags describing the clients behavior during login</param>
 * <param name="pui32AuthType" type="INOUT">In: Client proposed authentication type. Out: Selected authentication type.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Login was successful</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pui32AuthType is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_TYPE_MISMATCH">Non of the proposed authentication types matches the runtime requirements.</errorcode>
 */
RTS_RESULT CDECL UserMgrSelectAuthType(RTS_UI32 ui32ClientFlags, RTS_UI32 *pui32AuthType);
typedef RTS_RESULT (CDECL * PFUSERMGRSELECTAUTHTYPE) (RTS_UI32 ui32ClientFlags, RTS_UI32 *pui32AuthType);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRSELECTAUTHTYPE_NOTIMPLEMENTED)
	#define USE_UserMgrSelectAuthType
	#define EXT_UserMgrSelectAuthType
	#define GET_UserMgrSelectAuthType(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrSelectAuthType(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrSelectAuthType  FALSE
	#define EXP_UserMgrSelectAuthType  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrSelectAuthType
	#define EXT_UserMgrSelectAuthType
	#define GET_UserMgrSelectAuthType(fl)  CAL_CMGETAPI( "UserMgrSelectAuthType" ) 
	#define CAL_UserMgrSelectAuthType  UserMgrSelectAuthType
	#define CHK_UserMgrSelectAuthType  TRUE
	#define EXP_UserMgrSelectAuthType  CAL_CMEXPAPI( "UserMgrSelectAuthType" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrSelectAuthType
	#define EXT_UserMgrSelectAuthType
	#define GET_UserMgrSelectAuthType(fl)  CAL_CMGETAPI( "UserMgrSelectAuthType" ) 
	#define CAL_UserMgrSelectAuthType  UserMgrSelectAuthType
	#define CHK_UserMgrSelectAuthType  TRUE
	#define EXP_UserMgrSelectAuthType  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrSelectAuthType", (RTS_UINTPTR)UserMgrSelectAuthType, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrSelectAuthType
	#define EXT_CmpUserMgrUserMgrSelectAuthType
	#define GET_CmpUserMgrUserMgrSelectAuthType  ERR_OK
	#define CAL_CmpUserMgrUserMgrSelectAuthType pICmpUserMgr->IUserMgrSelectAuthType
	#define CHK_CmpUserMgrUserMgrSelectAuthType (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrSelectAuthType  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrSelectAuthType
	#define EXT_UserMgrSelectAuthType
	#define GET_UserMgrSelectAuthType(fl)  CAL_CMGETAPI( "UserMgrSelectAuthType" ) 
	#define CAL_UserMgrSelectAuthType pICmpUserMgr->IUserMgrSelectAuthType
	#define CHK_UserMgrSelectAuthType (pICmpUserMgr != NULL)
	#define EXP_UserMgrSelectAuthType  CAL_CMEXPAPI( "UserMgrSelectAuthType" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrSelectAuthType  PFUSERMGRSELECTAUTHTYPE pfUserMgrSelectAuthType;
	#define EXT_UserMgrSelectAuthType  extern PFUSERMGRSELECTAUTHTYPE pfUserMgrSelectAuthType;
	#define GET_UserMgrSelectAuthType(fl)  s_pfCMGetAPI2( "UserMgrSelectAuthType", (RTS_VOID_FCTPTR *)&pfUserMgrSelectAuthType, (fl), 0, 0)
	#define CAL_UserMgrSelectAuthType  pfUserMgrSelectAuthType
	#define CHK_UserMgrSelectAuthType  (pfUserMgrSelectAuthType != NULL)
	#define EXP_UserMgrSelectAuthType  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrSelectAuthType", (RTS_UINTPTR)UserMgrSelectAuthType, 0, 0) 
#endif




/**
 * <description>Handles a UserMgr login request received by CmpDevice. Only to be called by CmpDevice.</description>
 * <param name="SessionId" type="IN">SessionId of the device's online connection</param>
 * <param name="SessionAuthType" type="IN">Authentication algorithm used for the online session</param>
 * <param name="SessionFlags" type="IN">Flags describing the online session</param>
 * <param name="pWriter" type="IN">Pointer the writer stream</param>
 * <param name="pReader" type="IN">Pointer the reader stream</param>
 * <param name="phUser" type="OUT">Pointer to return the user handle</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Login was successful</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PENDING">Login is in progress, further online services needed.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">At least one of the parameters is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">Login was denied e. g. because of wrong user/password combination.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_TYPE_MISMATCH">Authentication type does not match the negotiated type</errorcode>
 */
RTS_RESULT CDECL UserMgrHandleLoginService(RTS_UI32 SessionId, RTS_UI32 SessionAuthType, RTS_UI32 SessionFlags, BINTAGWRITER *pWriter, BINTAGREADER *pReader, RTS_HANDLE *phUser);
typedef RTS_RESULT (CDECL * PFUSERMGRHANDLELOGINSERVICE) (RTS_UI32 SessionId, RTS_UI32 SessionAuthType, RTS_UI32 SessionFlags, BINTAGWRITER *pWriter, BINTAGREADER *pReader, RTS_HANDLE *phUser);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRHANDLELOGINSERVICE_NOTIMPLEMENTED)
	#define USE_UserMgrHandleLoginService
	#define EXT_UserMgrHandleLoginService
	#define GET_UserMgrHandleLoginService(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrHandleLoginService(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrHandleLoginService  FALSE
	#define EXP_UserMgrHandleLoginService  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrHandleLoginService
	#define EXT_UserMgrHandleLoginService
	#define GET_UserMgrHandleLoginService(fl)  CAL_CMGETAPI( "UserMgrHandleLoginService" ) 
	#define CAL_UserMgrHandleLoginService  UserMgrHandleLoginService
	#define CHK_UserMgrHandleLoginService  TRUE
	#define EXP_UserMgrHandleLoginService  CAL_CMEXPAPI( "UserMgrHandleLoginService" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrHandleLoginService
	#define EXT_UserMgrHandleLoginService
	#define GET_UserMgrHandleLoginService(fl)  CAL_CMGETAPI( "UserMgrHandleLoginService" ) 
	#define CAL_UserMgrHandleLoginService  UserMgrHandleLoginService
	#define CHK_UserMgrHandleLoginService  TRUE
	#define EXP_UserMgrHandleLoginService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrHandleLoginService", (RTS_UINTPTR)UserMgrHandleLoginService, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrHandleLoginService
	#define EXT_CmpUserMgrUserMgrHandleLoginService
	#define GET_CmpUserMgrUserMgrHandleLoginService  ERR_OK
	#define CAL_CmpUserMgrUserMgrHandleLoginService pICmpUserMgr->IUserMgrHandleLoginService
	#define CHK_CmpUserMgrUserMgrHandleLoginService (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrHandleLoginService  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrHandleLoginService
	#define EXT_UserMgrHandleLoginService
	#define GET_UserMgrHandleLoginService(fl)  CAL_CMGETAPI( "UserMgrHandleLoginService" ) 
	#define CAL_UserMgrHandleLoginService pICmpUserMgr->IUserMgrHandleLoginService
	#define CHK_UserMgrHandleLoginService (pICmpUserMgr != NULL)
	#define EXP_UserMgrHandleLoginService  CAL_CMEXPAPI( "UserMgrHandleLoginService" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrHandleLoginService  PFUSERMGRHANDLELOGINSERVICE pfUserMgrHandleLoginService;
	#define EXT_UserMgrHandleLoginService  extern PFUSERMGRHANDLELOGINSERVICE pfUserMgrHandleLoginService;
	#define GET_UserMgrHandleLoginService(fl)  s_pfCMGetAPI2( "UserMgrHandleLoginService", (RTS_VOID_FCTPTR *)&pfUserMgrHandleLoginService, (fl), 0, 0)
	#define CAL_UserMgrHandleLoginService  pfUserMgrHandleLoginService
	#define CHK_UserMgrHandleLoginService  (pfUserMgrHandleLoginService != NULL)
	#define EXP_UserMgrHandleLoginService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrHandleLoginService", (RTS_UINTPTR)UserMgrHandleLoginService, 0, 0) 
#endif




/**
 * <description>Handles the asymmetric login protocol of the UsrMgr</description>
 * <param name="sessionId" type="IN">SessionID of the device online connection (see CmpDeviceItf.h)</param>
 * <param name="pReader" type="IN">Pointer the reader stream</param>
 * <param name="pWriter" type="IN">Pointer the writer stream</param>
 * <param name="bChangePassword" type="IN">Replace password with new password</param>
 * <param name="pUserName" type="IN">User name</param>
 * <param name="userNameSize" type="IN">Buffer size of user name</param>
 * <param name="pPassword" type="IN">Password</param>
 * <param name="passwordSize" type="IN">Buffer size of password</param>
 * <param name="pNewPassword" type="IN">New password</param>
 * <param name="newPasswordSize" type="IN">Buffer size of new password</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Reply tag could be created</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PENDING">Login is in progress, further online services needed.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">At least one of the parameters is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Reply tag could not be created</errorcode>
 */
RTS_RESULT CDECL UserMgrHandleAsymmetricLoginProtocol(RTS_UI32 sessionId, BINTAGREADER *pReader, BINTAGWRITER *pWriter, RTS_BOOL bChangePassword, char *pUserName, RTS_UI32 userNameSize, char *pPassword, RTS_UI32 passwordSize, char *pNewPassword, RTS_UI32 newPasswordSize);
typedef RTS_RESULT (CDECL * PFUSERMGRHANDLEASYMMETRICLOGINPROTOCOL) (RTS_UI32 sessionId, BINTAGREADER *pReader, BINTAGWRITER *pWriter, RTS_BOOL bChangePassword, char *pUserName, RTS_UI32 userNameSize, char *pPassword, RTS_UI32 passwordSize, char *pNewPassword, RTS_UI32 newPasswordSize);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRHANDLEASYMMETRICLOGINPROTOCOL_NOTIMPLEMENTED)
	#define USE_UserMgrHandleAsymmetricLoginProtocol
	#define EXT_UserMgrHandleAsymmetricLoginProtocol
	#define GET_UserMgrHandleAsymmetricLoginProtocol(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrHandleAsymmetricLoginProtocol(p0,p1,p2,p3,p4,p5,p6,p7,p8,p9)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrHandleAsymmetricLoginProtocol  FALSE
	#define EXP_UserMgrHandleAsymmetricLoginProtocol  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrHandleAsymmetricLoginProtocol
	#define EXT_UserMgrHandleAsymmetricLoginProtocol
	#define GET_UserMgrHandleAsymmetricLoginProtocol(fl)  CAL_CMGETAPI( "UserMgrHandleAsymmetricLoginProtocol" ) 
	#define CAL_UserMgrHandleAsymmetricLoginProtocol  UserMgrHandleAsymmetricLoginProtocol
	#define CHK_UserMgrHandleAsymmetricLoginProtocol  TRUE
	#define EXP_UserMgrHandleAsymmetricLoginProtocol  CAL_CMEXPAPI( "UserMgrHandleAsymmetricLoginProtocol" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrHandleAsymmetricLoginProtocol
	#define EXT_UserMgrHandleAsymmetricLoginProtocol
	#define GET_UserMgrHandleAsymmetricLoginProtocol(fl)  CAL_CMGETAPI( "UserMgrHandleAsymmetricLoginProtocol" ) 
	#define CAL_UserMgrHandleAsymmetricLoginProtocol  UserMgrHandleAsymmetricLoginProtocol
	#define CHK_UserMgrHandleAsymmetricLoginProtocol  TRUE
	#define EXP_UserMgrHandleAsymmetricLoginProtocol  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrHandleAsymmetricLoginProtocol", (RTS_UINTPTR)UserMgrHandleAsymmetricLoginProtocol, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrHandleAsymmetricLoginProtocol
	#define EXT_CmpUserMgrUserMgrHandleAsymmetricLoginProtocol
	#define GET_CmpUserMgrUserMgrHandleAsymmetricLoginProtocol  ERR_OK
	#define CAL_CmpUserMgrUserMgrHandleAsymmetricLoginProtocol pICmpUserMgr->IUserMgrHandleAsymmetricLoginProtocol
	#define CHK_CmpUserMgrUserMgrHandleAsymmetricLoginProtocol (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrHandleAsymmetricLoginProtocol  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrHandleAsymmetricLoginProtocol
	#define EXT_UserMgrHandleAsymmetricLoginProtocol
	#define GET_UserMgrHandleAsymmetricLoginProtocol(fl)  CAL_CMGETAPI( "UserMgrHandleAsymmetricLoginProtocol" ) 
	#define CAL_UserMgrHandleAsymmetricLoginProtocol pICmpUserMgr->IUserMgrHandleAsymmetricLoginProtocol
	#define CHK_UserMgrHandleAsymmetricLoginProtocol (pICmpUserMgr != NULL)
	#define EXP_UserMgrHandleAsymmetricLoginProtocol  CAL_CMEXPAPI( "UserMgrHandleAsymmetricLoginProtocol" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrHandleAsymmetricLoginProtocol  PFUSERMGRHANDLEASYMMETRICLOGINPROTOCOL pfUserMgrHandleAsymmetricLoginProtocol;
	#define EXT_UserMgrHandleAsymmetricLoginProtocol  extern PFUSERMGRHANDLEASYMMETRICLOGINPROTOCOL pfUserMgrHandleAsymmetricLoginProtocol;
	#define GET_UserMgrHandleAsymmetricLoginProtocol(fl)  s_pfCMGetAPI2( "UserMgrHandleAsymmetricLoginProtocol", (RTS_VOID_FCTPTR *)&pfUserMgrHandleAsymmetricLoginProtocol, (fl), 0, 0)
	#define CAL_UserMgrHandleAsymmetricLoginProtocol  pfUserMgrHandleAsymmetricLoginProtocol
	#define CHK_UserMgrHandleAsymmetricLoginProtocol  (pfUserMgrHandleAsymmetricLoginProtocol != NULL)
	#define EXP_UserMgrHandleAsymmetricLoginProtocol  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrHandleAsymmetricLoginProtocol", (RTS_UINTPTR)UserMgrHandleAsymmetricLoginProtocol, 0, 0) 
#endif




RTS_RESULT CDECL UserMgrIsServiceAllowed(RTS_UI32 channelID, HEADER_TAG_EXT *pHeader, RTS_HANDLE hUser, PROTOCOL_DATA_UNIT *pPduReply);
typedef RTS_RESULT (CDECL * PFUSERMGRISSERVICEALLOWED) (RTS_UI32 channelID, HEADER_TAG_EXT *pHeader, RTS_HANDLE hUser, PROTOCOL_DATA_UNIT *pPduReply);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRISSERVICEALLOWED_NOTIMPLEMENTED)
	#define USE_UserMgrIsServiceAllowed
	#define EXT_UserMgrIsServiceAllowed
	#define GET_UserMgrIsServiceAllowed(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrIsServiceAllowed(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrIsServiceAllowed  FALSE
	#define EXP_UserMgrIsServiceAllowed  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrIsServiceAllowed
	#define EXT_UserMgrIsServiceAllowed
	#define GET_UserMgrIsServiceAllowed(fl)  CAL_CMGETAPI( "UserMgrIsServiceAllowed" ) 
	#define CAL_UserMgrIsServiceAllowed  UserMgrIsServiceAllowed
	#define CHK_UserMgrIsServiceAllowed  TRUE
	#define EXP_UserMgrIsServiceAllowed  CAL_CMEXPAPI( "UserMgrIsServiceAllowed" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrIsServiceAllowed
	#define EXT_UserMgrIsServiceAllowed
	#define GET_UserMgrIsServiceAllowed(fl)  CAL_CMGETAPI( "UserMgrIsServiceAllowed" ) 
	#define CAL_UserMgrIsServiceAllowed  UserMgrIsServiceAllowed
	#define CHK_UserMgrIsServiceAllowed  TRUE
	#define EXP_UserMgrIsServiceAllowed  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsServiceAllowed", (RTS_UINTPTR)UserMgrIsServiceAllowed, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrIsServiceAllowed
	#define EXT_CmpUserMgrUserMgrIsServiceAllowed
	#define GET_CmpUserMgrUserMgrIsServiceAllowed  ERR_OK
	#define CAL_CmpUserMgrUserMgrIsServiceAllowed pICmpUserMgr->IUserMgrIsServiceAllowed
	#define CHK_CmpUserMgrUserMgrIsServiceAllowed (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrIsServiceAllowed  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrIsServiceAllowed
	#define EXT_UserMgrIsServiceAllowed
	#define GET_UserMgrIsServiceAllowed(fl)  CAL_CMGETAPI( "UserMgrIsServiceAllowed" ) 
	#define CAL_UserMgrIsServiceAllowed pICmpUserMgr->IUserMgrIsServiceAllowed
	#define CHK_UserMgrIsServiceAllowed (pICmpUserMgr != NULL)
	#define EXP_UserMgrIsServiceAllowed  CAL_CMEXPAPI( "UserMgrIsServiceAllowed" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrIsServiceAllowed  PFUSERMGRISSERVICEALLOWED pfUserMgrIsServiceAllowed;
	#define EXT_UserMgrIsServiceAllowed  extern PFUSERMGRISSERVICEALLOWED pfUserMgrIsServiceAllowed;
	#define GET_UserMgrIsServiceAllowed(fl)  s_pfCMGetAPI2( "UserMgrIsServiceAllowed", (RTS_VOID_FCTPTR *)&pfUserMgrIsServiceAllowed, (fl), 0, 0)
	#define CAL_UserMgrIsServiceAllowed  pfUserMgrIsServiceAllowed
	#define CHK_UserMgrIsServiceAllowed  (pfUserMgrIsServiceAllowed != NULL)
	#define EXP_UserMgrIsServiceAllowed  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsServiceAllowed", (RTS_UINTPTR)UserMgrIsServiceAllowed, 0, 0) 
#endif




/**
 * <description>Checks if the user management is active.</description>
 * <result>TRUE: The user management is activated or enforced. FALSE: The user management is not activated.</result>
 */
RTS_BOOL CDECL UserMgrIsActive(void);
typedef RTS_BOOL (CDECL * PFUSERMGRISACTIVE) (void);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRISACTIVE_NOTIMPLEMENTED)
	#define USE_UserMgrIsActive
	#define EXT_UserMgrIsActive
	#define GET_UserMgrIsActive(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrIsActive()  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrIsActive  FALSE
	#define EXP_UserMgrIsActive  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrIsActive
	#define EXT_UserMgrIsActive
	#define GET_UserMgrIsActive(fl)  CAL_CMGETAPI( "UserMgrIsActive" ) 
	#define CAL_UserMgrIsActive  UserMgrIsActive
	#define CHK_UserMgrIsActive  TRUE
	#define EXP_UserMgrIsActive  CAL_CMEXPAPI( "UserMgrIsActive" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrIsActive
	#define EXT_UserMgrIsActive
	#define GET_UserMgrIsActive(fl)  CAL_CMGETAPI( "UserMgrIsActive" ) 
	#define CAL_UserMgrIsActive  UserMgrIsActive
	#define CHK_UserMgrIsActive  TRUE
	#define EXP_UserMgrIsActive  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsActive", (RTS_UINTPTR)UserMgrIsActive, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrIsActive
	#define EXT_CmpUserMgrUserMgrIsActive
	#define GET_CmpUserMgrUserMgrIsActive  ERR_OK
	#define CAL_CmpUserMgrUserMgrIsActive pICmpUserMgr->IUserMgrIsActive
	#define CHK_CmpUserMgrUserMgrIsActive (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrIsActive  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrIsActive
	#define EXT_UserMgrIsActive
	#define GET_UserMgrIsActive(fl)  CAL_CMGETAPI( "UserMgrIsActive" ) 
	#define CAL_UserMgrIsActive pICmpUserMgr->IUserMgrIsActive
	#define CHK_UserMgrIsActive (pICmpUserMgr != NULL)
	#define EXP_UserMgrIsActive  CAL_CMEXPAPI( "UserMgrIsActive" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrIsActive  PFUSERMGRISACTIVE pfUserMgrIsActive;
	#define EXT_UserMgrIsActive  extern PFUSERMGRISACTIVE pfUserMgrIsActive;
	#define GET_UserMgrIsActive(fl)  s_pfCMGetAPI2( "UserMgrIsActive", (RTS_VOID_FCTPTR *)&pfUserMgrIsActive, (fl), 0, 0)
	#define CAL_UserMgrIsActive  pfUserMgrIsActive
	#define CHK_UserMgrIsActive  (pfUserMgrIsActive != NULL)
	#define EXP_UserMgrIsActive  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsActive", (RTS_UINTPTR)UserMgrIsActive, 0, 0) 
#endif




/**
 * <description>
 * Registers an anonymous login for a specified component. For each registered anonymous login, a group will be created implicitely with the specified rights
 * (and a user will be added) as soon as the security setting SECURITY.UserMgmtAllowAnonymous is set. The specified component is then allowed
 * to login anonymously by using UserMgrLoginAnonymous().
 * NOTE:
 * The anonymous login has to be registered within CH_PRE_INIT3 or CH_INIT3.
 * </description>
 * <param name="pszName" type="IN">Name of the anonymous login. Typically name of the component that allows the anonymous login.</param>
 * <param name="pObjectRights" type="IN">Array of objects with default rights.</param>
 * <param name="numOfObjectRights" type="IN">Number of elements in array pObjectRights.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Anonymous login successfully registered</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pszName or pObjectRights or pObjectRights->pszObject is NULL or numOfObjectRights is 0.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">An anonymous login with the same name is already registered.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory to register the anonymous login.</errorcode>
 * <result>Handle to the anonymous login</result>
 */
RTS_HANDLE CDECL UserMgrRegisterAnonymousLogin(char *pszName, UserMgrAnonymousObjectRights *pObjectRights, RTS_UI32 numOfObjectRights, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGRREGISTERANONYMOUSLOGIN) (char *pszName, UserMgrAnonymousObjectRights *pObjectRights, RTS_UI32 numOfObjectRights, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREGISTERANONYMOUSLOGIN_NOTIMPLEMENTED)
	#define USE_UserMgrRegisterAnonymousLogin
	#define EXT_UserMgrRegisterAnonymousLogin
	#define GET_UserMgrRegisterAnonymousLogin(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRegisterAnonymousLogin(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrRegisterAnonymousLogin  FALSE
	#define EXP_UserMgrRegisterAnonymousLogin  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRegisterAnonymousLogin
	#define EXT_UserMgrRegisterAnonymousLogin
	#define GET_UserMgrRegisterAnonymousLogin(fl)  CAL_CMGETAPI( "UserMgrRegisterAnonymousLogin" ) 
	#define CAL_UserMgrRegisterAnonymousLogin  UserMgrRegisterAnonymousLogin
	#define CHK_UserMgrRegisterAnonymousLogin  TRUE
	#define EXP_UserMgrRegisterAnonymousLogin  CAL_CMEXPAPI( "UserMgrRegisterAnonymousLogin" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRegisterAnonymousLogin
	#define EXT_UserMgrRegisterAnonymousLogin
	#define GET_UserMgrRegisterAnonymousLogin(fl)  CAL_CMGETAPI( "UserMgrRegisterAnonymousLogin" ) 
	#define CAL_UserMgrRegisterAnonymousLogin  UserMgrRegisterAnonymousLogin
	#define CHK_UserMgrRegisterAnonymousLogin  TRUE
	#define EXP_UserMgrRegisterAnonymousLogin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterAnonymousLogin", (RTS_UINTPTR)UserMgrRegisterAnonymousLogin, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRegisterAnonymousLogin
	#define EXT_CmpUserMgrUserMgrRegisterAnonymousLogin
	#define GET_CmpUserMgrUserMgrRegisterAnonymousLogin  ERR_OK
	#define CAL_CmpUserMgrUserMgrRegisterAnonymousLogin pICmpUserMgr->IUserMgrRegisterAnonymousLogin
	#define CHK_CmpUserMgrUserMgrRegisterAnonymousLogin (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRegisterAnonymousLogin  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRegisterAnonymousLogin
	#define EXT_UserMgrRegisterAnonymousLogin
	#define GET_UserMgrRegisterAnonymousLogin(fl)  CAL_CMGETAPI( "UserMgrRegisterAnonymousLogin" ) 
	#define CAL_UserMgrRegisterAnonymousLogin pICmpUserMgr->IUserMgrRegisterAnonymousLogin
	#define CHK_UserMgrRegisterAnonymousLogin (pICmpUserMgr != NULL)
	#define EXP_UserMgrRegisterAnonymousLogin  CAL_CMEXPAPI( "UserMgrRegisterAnonymousLogin" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRegisterAnonymousLogin  PFUSERMGRREGISTERANONYMOUSLOGIN pfUserMgrRegisterAnonymousLogin;
	#define EXT_UserMgrRegisterAnonymousLogin  extern PFUSERMGRREGISTERANONYMOUSLOGIN pfUserMgrRegisterAnonymousLogin;
	#define GET_UserMgrRegisterAnonymousLogin(fl)  s_pfCMGetAPI2( "UserMgrRegisterAnonymousLogin", (RTS_VOID_FCTPTR *)&pfUserMgrRegisterAnonymousLogin, (fl), 0, 0)
	#define CAL_UserMgrRegisterAnonymousLogin  pfUserMgrRegisterAnonymousLogin
	#define CHK_UserMgrRegisterAnonymousLogin  (pfUserMgrRegisterAnonymousLogin != NULL)
	#define EXP_UserMgrRegisterAnonymousLogin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRegisterAnonymousLogin", (RTS_UINTPTR)UserMgrRegisterAnonymousLogin, 0, 0) 
#endif




/**
 * <description>Unregisters an anonymous login.</description>
 * <param name="hAnonymousLogin" type="IN">Handle to the anonymous login.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Anonymous login successfully unregistered.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">Anonymous login not found.</errorcode>
 */
RTS_RESULT CDECL UserMgrUnregisterAnonymousLogin(RTS_HANDLE hAnonymousLogin);
typedef RTS_RESULT (CDECL * PFUSERMGRUNREGISTERANONYMOUSLOGIN) (RTS_HANDLE hAnonymousLogin);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUNREGISTERANONYMOUSLOGIN_NOTIMPLEMENTED)
	#define USE_UserMgrUnregisterAnonymousLogin
	#define EXT_UserMgrUnregisterAnonymousLogin
	#define GET_UserMgrUnregisterAnonymousLogin(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUnregisterAnonymousLogin(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUnregisterAnonymousLogin  FALSE
	#define EXP_UserMgrUnregisterAnonymousLogin  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUnregisterAnonymousLogin
	#define EXT_UserMgrUnregisterAnonymousLogin
	#define GET_UserMgrUnregisterAnonymousLogin(fl)  CAL_CMGETAPI( "UserMgrUnregisterAnonymousLogin" ) 
	#define CAL_UserMgrUnregisterAnonymousLogin  UserMgrUnregisterAnonymousLogin
	#define CHK_UserMgrUnregisterAnonymousLogin  TRUE
	#define EXP_UserMgrUnregisterAnonymousLogin  CAL_CMEXPAPI( "UserMgrUnregisterAnonymousLogin" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUnregisterAnonymousLogin
	#define EXT_UserMgrUnregisterAnonymousLogin
	#define GET_UserMgrUnregisterAnonymousLogin(fl)  CAL_CMGETAPI( "UserMgrUnregisterAnonymousLogin" ) 
	#define CAL_UserMgrUnregisterAnonymousLogin  UserMgrUnregisterAnonymousLogin
	#define CHK_UserMgrUnregisterAnonymousLogin  TRUE
	#define EXP_UserMgrUnregisterAnonymousLogin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterAnonymousLogin", (RTS_UINTPTR)UserMgrUnregisterAnonymousLogin, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUnregisterAnonymousLogin
	#define EXT_CmpUserMgrUserMgrUnregisterAnonymousLogin
	#define GET_CmpUserMgrUserMgrUnregisterAnonymousLogin  ERR_OK
	#define CAL_CmpUserMgrUserMgrUnregisterAnonymousLogin pICmpUserMgr->IUserMgrUnregisterAnonymousLogin
	#define CHK_CmpUserMgrUserMgrUnregisterAnonymousLogin (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUnregisterAnonymousLogin  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUnregisterAnonymousLogin
	#define EXT_UserMgrUnregisterAnonymousLogin
	#define GET_UserMgrUnregisterAnonymousLogin(fl)  CAL_CMGETAPI( "UserMgrUnregisterAnonymousLogin" ) 
	#define CAL_UserMgrUnregisterAnonymousLogin pICmpUserMgr->IUserMgrUnregisterAnonymousLogin
	#define CHK_UserMgrUnregisterAnonymousLogin (pICmpUserMgr != NULL)
	#define EXP_UserMgrUnregisterAnonymousLogin  CAL_CMEXPAPI( "UserMgrUnregisterAnonymousLogin" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUnregisterAnonymousLogin  PFUSERMGRUNREGISTERANONYMOUSLOGIN pfUserMgrUnregisterAnonymousLogin;
	#define EXT_UserMgrUnregisterAnonymousLogin  extern PFUSERMGRUNREGISTERANONYMOUSLOGIN pfUserMgrUnregisterAnonymousLogin;
	#define GET_UserMgrUnregisterAnonymousLogin(fl)  s_pfCMGetAPI2( "UserMgrUnregisterAnonymousLogin", (RTS_VOID_FCTPTR *)&pfUserMgrUnregisterAnonymousLogin, (fl), 0, 0)
	#define CAL_UserMgrUnregisterAnonymousLogin  pfUserMgrUnregisterAnonymousLogin
	#define CHK_UserMgrUnregisterAnonymousLogin  (pfUserMgrUnregisterAnonymousLogin != NULL)
	#define EXP_UserMgrUnregisterAnonymousLogin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUnregisterAnonymousLogin", (RTS_UINTPTR)UserMgrUnregisterAnonymousLogin, 0, 0) 
#endif




/**
 * <description>Logs in to the runtime system using an anonymous login. The anonymous login has to be registered by UserMgrRegisterAnonymousLogin()</description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Anonymous login was successful.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">hAnonymousLogin is not valid.</errorcode>
 * <result>Handle to the user or RTS_INVALID_HANDLE if not available</result>
 */
RTS_HANDLE CDECL UserMgrLoginAnonymous(RTS_HANDLE hAnonymousLogin, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGRLOGINANONYMOUS) (RTS_HANDLE hAnonymousLogin, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRLOGINANONYMOUS_NOTIMPLEMENTED)
	#define USE_UserMgrLoginAnonymous
	#define EXT_UserMgrLoginAnonymous
	#define GET_UserMgrLoginAnonymous(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrLoginAnonymous(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrLoginAnonymous  FALSE
	#define EXP_UserMgrLoginAnonymous  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrLoginAnonymous
	#define EXT_UserMgrLoginAnonymous
	#define GET_UserMgrLoginAnonymous(fl)  CAL_CMGETAPI( "UserMgrLoginAnonymous" ) 
	#define CAL_UserMgrLoginAnonymous  UserMgrLoginAnonymous
	#define CHK_UserMgrLoginAnonymous  TRUE
	#define EXP_UserMgrLoginAnonymous  CAL_CMEXPAPI( "UserMgrLoginAnonymous" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrLoginAnonymous
	#define EXT_UserMgrLoginAnonymous
	#define GET_UserMgrLoginAnonymous(fl)  CAL_CMGETAPI( "UserMgrLoginAnonymous" ) 
	#define CAL_UserMgrLoginAnonymous  UserMgrLoginAnonymous
	#define CHK_UserMgrLoginAnonymous  TRUE
	#define EXP_UserMgrLoginAnonymous  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrLoginAnonymous", (RTS_UINTPTR)UserMgrLoginAnonymous, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrLoginAnonymous
	#define EXT_CmpUserMgrUserMgrLoginAnonymous
	#define GET_CmpUserMgrUserMgrLoginAnonymous  ERR_OK
	#define CAL_CmpUserMgrUserMgrLoginAnonymous pICmpUserMgr->IUserMgrLoginAnonymous
	#define CHK_CmpUserMgrUserMgrLoginAnonymous (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrLoginAnonymous  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrLoginAnonymous
	#define EXT_UserMgrLoginAnonymous
	#define GET_UserMgrLoginAnonymous(fl)  CAL_CMGETAPI( "UserMgrLoginAnonymous" ) 
	#define CAL_UserMgrLoginAnonymous pICmpUserMgr->IUserMgrLoginAnonymous
	#define CHK_UserMgrLoginAnonymous (pICmpUserMgr != NULL)
	#define EXP_UserMgrLoginAnonymous  CAL_CMEXPAPI( "UserMgrLoginAnonymous" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrLoginAnonymous  PFUSERMGRLOGINANONYMOUS pfUserMgrLoginAnonymous;
	#define EXT_UserMgrLoginAnonymous  extern PFUSERMGRLOGINANONYMOUS pfUserMgrLoginAnonymous;
	#define GET_UserMgrLoginAnonymous(fl)  s_pfCMGetAPI2( "UserMgrLoginAnonymous", (RTS_VOID_FCTPTR *)&pfUserMgrLoginAnonymous, (fl), 0, 0)
	#define CAL_UserMgrLoginAnonymous  pfUserMgrLoginAnonymous
	#define CHK_UserMgrLoginAnonymous  (pfUserMgrLoginAnonymous != NULL)
	#define EXP_UserMgrLoginAnonymous  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrLoginAnonymous", (RTS_UINTPTR)UserMgrLoginAnonymous, 0, 0) 
#endif




/**
 * <description>Re-logs user in to the runtime system using an anonymous login. The handle to the anonymous login must match to the initial login.</description>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Anonymous relogin was successful.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">hAnonymousLogin is not valid.</errorcode>
 */
RTS_RESULT CDECL UserMgrReloginAnonymous(RTS_HANDLE hUser, RTS_HANDLE hAnonymousLogin);
typedef RTS_RESULT (CDECL * PFUSERMGRRELOGINANONYMOUS) (RTS_HANDLE hUser, RTS_HANDLE hAnonymousLogin);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRRELOGINANONYMOUS_NOTIMPLEMENTED)
	#define USE_UserMgrReloginAnonymous
	#define EXT_UserMgrReloginAnonymous
	#define GET_UserMgrReloginAnonymous(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrReloginAnonymous(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrReloginAnonymous  FALSE
	#define EXP_UserMgrReloginAnonymous  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrReloginAnonymous
	#define EXT_UserMgrReloginAnonymous
	#define GET_UserMgrReloginAnonymous(fl)  CAL_CMGETAPI( "UserMgrReloginAnonymous" ) 
	#define CAL_UserMgrReloginAnonymous  UserMgrReloginAnonymous
	#define CHK_UserMgrReloginAnonymous  TRUE
	#define EXP_UserMgrReloginAnonymous  CAL_CMEXPAPI( "UserMgrReloginAnonymous" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrReloginAnonymous
	#define EXT_UserMgrReloginAnonymous
	#define GET_UserMgrReloginAnonymous(fl)  CAL_CMGETAPI( "UserMgrReloginAnonymous" ) 
	#define CAL_UserMgrReloginAnonymous  UserMgrReloginAnonymous
	#define CHK_UserMgrReloginAnonymous  TRUE
	#define EXP_UserMgrReloginAnonymous  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrReloginAnonymous", (RTS_UINTPTR)UserMgrReloginAnonymous, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrReloginAnonymous
	#define EXT_CmpUserMgrUserMgrReloginAnonymous
	#define GET_CmpUserMgrUserMgrReloginAnonymous  ERR_OK
	#define CAL_CmpUserMgrUserMgrReloginAnonymous pICmpUserMgr->IUserMgrReloginAnonymous
	#define CHK_CmpUserMgrUserMgrReloginAnonymous (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrReloginAnonymous  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrReloginAnonymous
	#define EXT_UserMgrReloginAnonymous
	#define GET_UserMgrReloginAnonymous(fl)  CAL_CMGETAPI( "UserMgrReloginAnonymous" ) 
	#define CAL_UserMgrReloginAnonymous pICmpUserMgr->IUserMgrReloginAnonymous
	#define CHK_UserMgrReloginAnonymous (pICmpUserMgr != NULL)
	#define EXP_UserMgrReloginAnonymous  CAL_CMEXPAPI( "UserMgrReloginAnonymous" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrReloginAnonymous  PFUSERMGRRELOGINANONYMOUS pfUserMgrReloginAnonymous;
	#define EXT_UserMgrReloginAnonymous  extern PFUSERMGRRELOGINANONYMOUS pfUserMgrReloginAnonymous;
	#define GET_UserMgrReloginAnonymous(fl)  s_pfCMGetAPI2( "UserMgrReloginAnonymous", (RTS_VOID_FCTPTR *)&pfUserMgrReloginAnonymous, (fl), 0, 0)
	#define CAL_UserMgrReloginAnonymous  pfUserMgrReloginAnonymous
	#define CHK_UserMgrReloginAnonymous  (pfUserMgrReloginAnonymous != NULL)
	#define EXP_UserMgrReloginAnonymous  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrReloginAnonymous", (RTS_UINTPTR)UserMgrReloginAnonymous, 0, 0) 
#endif




/**
 * <description>Checks if anonymous login is allowed for registered anonymous login, see UserMgrRegisterAnonymousLogin().</description>
 * <param name="hAnonymousLogin" type="IN">Handle to the anonymous login.</param>
 * <result>TRUE if allowed, FALSE if not.</result>
 */
RTS_BOOL CDECL UserMgrIsAnonymousLoginAllowed(RTS_HANDLE hAnonymousLogin);
typedef RTS_BOOL (CDECL * PFUSERMGRISANONYMOUSLOGINALLOWED) (RTS_HANDLE hAnonymousLogin);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRISANONYMOUSLOGINALLOWED_NOTIMPLEMENTED)
	#define USE_UserMgrIsAnonymousLoginAllowed
	#define EXT_UserMgrIsAnonymousLoginAllowed
	#define GET_UserMgrIsAnonymousLoginAllowed(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrIsAnonymousLoginAllowed(p0)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrIsAnonymousLoginAllowed  FALSE
	#define EXP_UserMgrIsAnonymousLoginAllowed  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrIsAnonymousLoginAllowed
	#define EXT_UserMgrIsAnonymousLoginAllowed
	#define GET_UserMgrIsAnonymousLoginAllowed(fl)  CAL_CMGETAPI( "UserMgrIsAnonymousLoginAllowed" ) 
	#define CAL_UserMgrIsAnonymousLoginAllowed  UserMgrIsAnonymousLoginAllowed
	#define CHK_UserMgrIsAnonymousLoginAllowed  TRUE
	#define EXP_UserMgrIsAnonymousLoginAllowed  CAL_CMEXPAPI( "UserMgrIsAnonymousLoginAllowed" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrIsAnonymousLoginAllowed
	#define EXT_UserMgrIsAnonymousLoginAllowed
	#define GET_UserMgrIsAnonymousLoginAllowed(fl)  CAL_CMGETAPI( "UserMgrIsAnonymousLoginAllowed" ) 
	#define CAL_UserMgrIsAnonymousLoginAllowed  UserMgrIsAnonymousLoginAllowed
	#define CHK_UserMgrIsAnonymousLoginAllowed  TRUE
	#define EXP_UserMgrIsAnonymousLoginAllowed  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsAnonymousLoginAllowed", (RTS_UINTPTR)UserMgrIsAnonymousLoginAllowed, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrIsAnonymousLoginAllowed
	#define EXT_CmpUserMgrUserMgrIsAnonymousLoginAllowed
	#define GET_CmpUserMgrUserMgrIsAnonymousLoginAllowed  ERR_OK
	#define CAL_CmpUserMgrUserMgrIsAnonymousLoginAllowed pICmpUserMgr->IUserMgrIsAnonymousLoginAllowed
	#define CHK_CmpUserMgrUserMgrIsAnonymousLoginAllowed (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrIsAnonymousLoginAllowed  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrIsAnonymousLoginAllowed
	#define EXT_UserMgrIsAnonymousLoginAllowed
	#define GET_UserMgrIsAnonymousLoginAllowed(fl)  CAL_CMGETAPI( "UserMgrIsAnonymousLoginAllowed" ) 
	#define CAL_UserMgrIsAnonymousLoginAllowed pICmpUserMgr->IUserMgrIsAnonymousLoginAllowed
	#define CHK_UserMgrIsAnonymousLoginAllowed (pICmpUserMgr != NULL)
	#define EXP_UserMgrIsAnonymousLoginAllowed  CAL_CMEXPAPI( "UserMgrIsAnonymousLoginAllowed" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrIsAnonymousLoginAllowed  PFUSERMGRISANONYMOUSLOGINALLOWED pfUserMgrIsAnonymousLoginAllowed;
	#define EXT_UserMgrIsAnonymousLoginAllowed  extern PFUSERMGRISANONYMOUSLOGINALLOWED pfUserMgrIsAnonymousLoginAllowed;
	#define GET_UserMgrIsAnonymousLoginAllowed(fl)  s_pfCMGetAPI2( "UserMgrIsAnonymousLoginAllowed", (RTS_VOID_FCTPTR *)&pfUserMgrIsAnonymousLoginAllowed, (fl), 0, 0)
	#define CAL_UserMgrIsAnonymousLoginAllowed  pfUserMgrIsAnonymousLoginAllowed
	#define CHK_UserMgrIsAnonymousLoginAllowed  (pfUserMgrIsAnonymousLoginAllowed != NULL)
	#define EXP_UserMgrIsAnonymousLoginAllowed  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsAnonymousLoginAllowed", (RTS_UINTPTR)UserMgrIsAnonymousLoginAllowed, 0, 0) 
#endif




/**
 * <description>Logs user in to the runtime system using an active user management.</description>
 * <param name="pszUser" type="IN">Name of the user</param>
 * <param name="pszPassword" type="IN">Password (plain text)</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User/password combination is valid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_ACTIVATE_USERMGMT">The login was successful. But the user management is enforced and not active. Activate the user management.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_CHANGE_PASSWORD">The login was successful. But the user must change his password.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_USERMGMT">Login was not successful. The user management is not activated. Use empty user and password to login.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">The login was not successful, because of another reason.</errorcode>
 * <result>Handle to the user or RTS_INVALID_HANDLE if not available</result>
 */
RTS_HANDLE CDECL UserMgrLogin(char *pszUser, char *pszPassword, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGRLOGIN) (char *pszUser, char *pszPassword, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRLOGIN_NOTIMPLEMENTED)
	#define USE_UserMgrLogin
	#define EXT_UserMgrLogin
	#define GET_UserMgrLogin(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrLogin(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
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
 * <description>Logout specified by the user</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User is available and logout succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If user is not available</errorcode>
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
 * <description>Re-logs user in to the runtime system using an active user management. Using this API it is possible to change the user identity of the given handle.</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pszUser" type="IN">Name of the user</param>
 * <param name="pszPassword" type="IN">Password (plain text)</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User/password combination is valid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pszUser is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_RELOGIN">hUser known, but not logged in</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Password is wrong.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_USER_MISMATCH">User name does not match</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser is invalid or unknown</errorcode>
 */
RTS_RESULT CDECL UserMgrRelogin(RTS_HANDLE hUser, char *pszUser, char *pszPassword);
typedef RTS_RESULT (CDECL * PFUSERMGRRELOGIN) (RTS_HANDLE hUser, char *pszUser, char *pszPassword);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRRELOGIN_NOTIMPLEMENTED)
	#define USE_UserMgrRelogin
	#define EXT_UserMgrRelogin
	#define GET_UserMgrRelogin(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRelogin(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRelogin  FALSE
	#define EXP_UserMgrRelogin  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRelogin
	#define EXT_UserMgrRelogin
	#define GET_UserMgrRelogin(fl)  CAL_CMGETAPI( "UserMgrRelogin" ) 
	#define CAL_UserMgrRelogin  UserMgrRelogin
	#define CHK_UserMgrRelogin  TRUE
	#define EXP_UserMgrRelogin  CAL_CMEXPAPI( "UserMgrRelogin" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRelogin
	#define EXT_UserMgrRelogin
	#define GET_UserMgrRelogin(fl)  CAL_CMGETAPI( "UserMgrRelogin" ) 
	#define CAL_UserMgrRelogin  UserMgrRelogin
	#define CHK_UserMgrRelogin  TRUE
	#define EXP_UserMgrRelogin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRelogin", (RTS_UINTPTR)UserMgrRelogin, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRelogin
	#define EXT_CmpUserMgrUserMgrRelogin
	#define GET_CmpUserMgrUserMgrRelogin  ERR_OK
	#define CAL_CmpUserMgrUserMgrRelogin pICmpUserMgr->IUserMgrRelogin
	#define CHK_CmpUserMgrUserMgrRelogin (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRelogin  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRelogin
	#define EXT_UserMgrRelogin
	#define GET_UserMgrRelogin(fl)  CAL_CMGETAPI( "UserMgrRelogin" ) 
	#define CAL_UserMgrRelogin pICmpUserMgr->IUserMgrRelogin
	#define CHK_UserMgrRelogin (pICmpUserMgr != NULL)
	#define EXP_UserMgrRelogin  CAL_CMEXPAPI( "UserMgrRelogin" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRelogin  PFUSERMGRRELOGIN pfUserMgrRelogin;
	#define EXT_UserMgrRelogin  extern PFUSERMGRRELOGIN pfUserMgrRelogin;
	#define GET_UserMgrRelogin(fl)  s_pfCMGetAPI2( "UserMgrRelogin", (RTS_VOID_FCTPTR *)&pfUserMgrRelogin, (fl), 0, 0)
	#define CAL_UserMgrRelogin  pfUserMgrRelogin
	#define CHK_UserMgrRelogin  (pfUserMgrRelogin != NULL)
	#define EXP_UserMgrRelogin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRelogin", (RTS_UINTPTR)UserMgrRelogin, 0, 0) 
#endif




/**
 * <description>Checks if the a user is a member of the given group</description>
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
 * <description>Checks if the a user an administrator.</description>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <result>TRUE: The user is an administrator, FALSE: The user is not an administrator.</result>
 */
RTS_BOOL CDECL UserMgrIsAdministrator(RTS_HANDLE hUser);
typedef RTS_BOOL (CDECL * PFUSERMGRISADMINISTRATOR) (RTS_HANDLE hUser);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRISADMINISTRATOR_NOTIMPLEMENTED)
	#define USE_UserMgrIsAdministrator
	#define EXT_UserMgrIsAdministrator
	#define GET_UserMgrIsAdministrator(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrIsAdministrator(p0)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrIsAdministrator  FALSE
	#define EXP_UserMgrIsAdministrator  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrIsAdministrator
	#define EXT_UserMgrIsAdministrator
	#define GET_UserMgrIsAdministrator(fl)  CAL_CMGETAPI( "UserMgrIsAdministrator" ) 
	#define CAL_UserMgrIsAdministrator  UserMgrIsAdministrator
	#define CHK_UserMgrIsAdministrator  TRUE
	#define EXP_UserMgrIsAdministrator  CAL_CMEXPAPI( "UserMgrIsAdministrator" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrIsAdministrator
	#define EXT_UserMgrIsAdministrator
	#define GET_UserMgrIsAdministrator(fl)  CAL_CMGETAPI( "UserMgrIsAdministrator" ) 
	#define CAL_UserMgrIsAdministrator  UserMgrIsAdministrator
	#define CHK_UserMgrIsAdministrator  TRUE
	#define EXP_UserMgrIsAdministrator  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsAdministrator", (RTS_UINTPTR)UserMgrIsAdministrator, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrIsAdministrator
	#define EXT_CmpUserMgrUserMgrIsAdministrator
	#define GET_CmpUserMgrUserMgrIsAdministrator  ERR_OK
	#define CAL_CmpUserMgrUserMgrIsAdministrator pICmpUserMgr->IUserMgrIsAdministrator
	#define CHK_CmpUserMgrUserMgrIsAdministrator (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrIsAdministrator  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrIsAdministrator
	#define EXT_UserMgrIsAdministrator
	#define GET_UserMgrIsAdministrator(fl)  CAL_CMGETAPI( "UserMgrIsAdministrator" ) 
	#define CAL_UserMgrIsAdministrator pICmpUserMgr->IUserMgrIsAdministrator
	#define CHK_UserMgrIsAdministrator (pICmpUserMgr != NULL)
	#define EXP_UserMgrIsAdministrator  CAL_CMEXPAPI( "UserMgrIsAdministrator" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrIsAdministrator  PFUSERMGRISADMINISTRATOR pfUserMgrIsAdministrator;
	#define EXT_UserMgrIsAdministrator  extern PFUSERMGRISADMINISTRATOR pfUserMgrIsAdministrator;
	#define GET_UserMgrIsAdministrator(fl)  s_pfCMGetAPI2( "UserMgrIsAdministrator", (RTS_VOID_FCTPTR *)&pfUserMgrIsAdministrator, (fl), 0, 0)
	#define CAL_UserMgrIsAdministrator  pfUserMgrIsAdministrator
	#define CHK_UserMgrIsAdministrator  (pfUserMgrIsAdministrator != NULL)
	#define EXP_UserMgrIsAdministrator  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrIsAdministrator", (RTS_UINTPTR)UserMgrIsAdministrator, 0, 0) 
#endif




/**
 * <description>Checks user access rights on the specified object (authorization)!</description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="ulSessionId" type="IN">SessionID of the device online connection (see CmpDeviceItf.h)</param>
 * <param name="ulRequestedRights" type="IN">Requested rights on that object</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">If user has the requested rights on the object or if no user management is configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If SessionID or user is not available or the object does not exist</errorcode>
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
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If SessionID or user is not available or the object does not exist</errorcode>
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
 * <description>Checks user access rights on the specified object (authorization)!</description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="ulRequestedRights" type="IN">Requested rights on that object</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">If user has the requested rights on the object or if no user management is configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If user is not available or the object does not exist</errorcode>
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
 * <description>Gets user access rights on the specified object (authorization)!</description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <param name="pulRights" type="IN">Pointer to get admitted rights</param>
 * <param name="pulDeniedRights" type="IN">Pointer to get denied rights.
 *	 NOTE:
 *	 If the required right is denied _and_ admitted on the specified object, the denied right is more significant and so the access must be denied !!</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">If user has the requested rights on the object or if no user management is configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If user is not available or the object does not exist</errorcode>
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
 * <description>Adds an value to an logged in user. This can be used as some kind of cache that exists as long as the user is logged in.</description>
 * <param name="hUser" type="IN">Handle to the user.</param>
 * <param name="key" type="IN">Access key for the cache value. This has to be used at UserMgrGetInfoOfUser to access the value.</param>
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
 * <description>Removes a specific cache value from an user.</description>
 * <param name="hUser" type="IN">Handle to the user.</param>
 * <param name="key" type="IN">Key of the cache value.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Removing cache value successful</errorcode>
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
 * <description>Removes a specific cache value from all logged in users.</description>
 * <param name="key" type="IN">Key of the cache value.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Removing cache value successful</errorcode>
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
 * <errorcode name="RTS_RESULT" type="ERR_OK">Retrieving cache value successful. Value copied to pValue.</errorcode>
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
 * <description>Retrieves user name from a user handle.</description>
 * <param name="hUser" type="IN">Handle to the user.</param>
 * <param name="pName" type="IN">Pointer to the buffer where to store the name.</param>
 * <param name="maxLen" type="IN">Maximum size of the user name.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Retrieving cache value successful. Value copied to pValue.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser was not a valid user handle.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pName was NULL.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">The buffer was too small.</errorcode>
 */
RTS_RESULT CDECL UserMgrGetUserName(RTS_HANDLE hUser, char *pName, RTS_UI32 maxLen);
typedef RTS_RESULT (CDECL * PFUSERMGRGETUSERNAME) (RTS_HANDLE hUser, char *pName, RTS_UI32 maxLen);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGETUSERNAME_NOTIMPLEMENTED)
	#define USE_UserMgrGetUserName
	#define EXT_UserMgrGetUserName
	#define GET_UserMgrGetUserName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGetUserName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGetUserName  FALSE
	#define EXP_UserMgrGetUserName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGetUserName
	#define EXT_UserMgrGetUserName
	#define GET_UserMgrGetUserName(fl)  CAL_CMGETAPI( "UserMgrGetUserName" ) 
	#define CAL_UserMgrGetUserName  UserMgrGetUserName
	#define CHK_UserMgrGetUserName  TRUE
	#define EXP_UserMgrGetUserName  CAL_CMEXPAPI( "UserMgrGetUserName" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGetUserName
	#define EXT_UserMgrGetUserName
	#define GET_UserMgrGetUserName(fl)  CAL_CMGETAPI( "UserMgrGetUserName" ) 
	#define CAL_UserMgrGetUserName  UserMgrGetUserName
	#define CHK_UserMgrGetUserName  TRUE
	#define EXP_UserMgrGetUserName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetUserName", (RTS_UINTPTR)UserMgrGetUserName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGetUserName
	#define EXT_CmpUserMgrUserMgrGetUserName
	#define GET_CmpUserMgrUserMgrGetUserName  ERR_OK
	#define CAL_CmpUserMgrUserMgrGetUserName pICmpUserMgr->IUserMgrGetUserName
	#define CHK_CmpUserMgrUserMgrGetUserName (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGetUserName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGetUserName
	#define EXT_UserMgrGetUserName
	#define GET_UserMgrGetUserName(fl)  CAL_CMGETAPI( "UserMgrGetUserName" ) 
	#define CAL_UserMgrGetUserName pICmpUserMgr->IUserMgrGetUserName
	#define CHK_UserMgrGetUserName (pICmpUserMgr != NULL)
	#define EXP_UserMgrGetUserName  CAL_CMEXPAPI( "UserMgrGetUserName" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGetUserName  PFUSERMGRGETUSERNAME pfUserMgrGetUserName;
	#define EXT_UserMgrGetUserName  extern PFUSERMGRGETUSERNAME pfUserMgrGetUserName;
	#define GET_UserMgrGetUserName(fl)  s_pfCMGetAPI2( "UserMgrGetUserName", (RTS_VOID_FCTPTR *)&pfUserMgrGetUserName, (fl), 0, 0)
	#define CAL_UserMgrGetUserName  pfUserMgrGetUserName
	#define CHK_UserMgrGetUserName  (pfUserMgrGetUserName != NULL)
	#define EXP_UserMgrGetUserName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetUserName", (RTS_UINTPTR)UserMgrGetUserName, 0, 0) 
#endif




/**
 * <description>Get a group by it's name.</description>
 * <param name="pszGroupName" type="IN">Name of the group.</param>
 * <param name="pResult" type="OUT">Error code.</param>
 * <return>Handle to the group</return>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group found.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pszGroupName was NULL.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">There is not group with this name.</errorcode>
 */
RTS_HANDLE CDECL UserMgrGetGroup(char *pszGroupName, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGRGETGROUP) (char *pszGroupName, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGETGROUP_NOTIMPLEMENTED)
	#define USE_UserMgrGetGroup
	#define EXT_UserMgrGetGroup
	#define GET_UserMgrGetGroup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGetGroup(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrGetGroup  FALSE
	#define EXP_UserMgrGetGroup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGetGroup
	#define EXT_UserMgrGetGroup
	#define GET_UserMgrGetGroup(fl)  CAL_CMGETAPI( "UserMgrGetGroup" ) 
	#define CAL_UserMgrGetGroup  UserMgrGetGroup
	#define CHK_UserMgrGetGroup  TRUE
	#define EXP_UserMgrGetGroup  CAL_CMEXPAPI( "UserMgrGetGroup" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGetGroup
	#define EXT_UserMgrGetGroup
	#define GET_UserMgrGetGroup(fl)  CAL_CMGETAPI( "UserMgrGetGroup" ) 
	#define CAL_UserMgrGetGroup  UserMgrGetGroup
	#define CHK_UserMgrGetGroup  TRUE
	#define EXP_UserMgrGetGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetGroup", (RTS_UINTPTR)UserMgrGetGroup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGetGroup
	#define EXT_CmpUserMgrUserMgrGetGroup
	#define GET_CmpUserMgrUserMgrGetGroup  ERR_OK
	#define CAL_CmpUserMgrUserMgrGetGroup pICmpUserMgr->IUserMgrGetGroup
	#define CHK_CmpUserMgrUserMgrGetGroup (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGetGroup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGetGroup
	#define EXT_UserMgrGetGroup
	#define GET_UserMgrGetGroup(fl)  CAL_CMGETAPI( "UserMgrGetGroup" ) 
	#define CAL_UserMgrGetGroup pICmpUserMgr->IUserMgrGetGroup
	#define CHK_UserMgrGetGroup (pICmpUserMgr != NULL)
	#define EXP_UserMgrGetGroup  CAL_CMEXPAPI( "UserMgrGetGroup" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGetGroup  PFUSERMGRGETGROUP pfUserMgrGetGroup;
	#define EXT_UserMgrGetGroup  extern PFUSERMGRGETGROUP pfUserMgrGetGroup;
	#define GET_UserMgrGetGroup(fl)  s_pfCMGetAPI2( "UserMgrGetGroup", (RTS_VOID_FCTPTR *)&pfUserMgrGetGroup, (fl), 0, 0)
	#define CAL_UserMgrGetGroup  pfUserMgrGetGroup
	#define CHK_UserMgrGetGroup  (pfUserMgrGetGroup != NULL)
	#define EXP_UserMgrGetGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGetGroup", (RTS_UINTPTR)UserMgrGetGroup, 0, 0) 
#endif




/**
 * <description>Checks if a user is member of a specific group.</description>
 * <param name="hGroup" type="IN">Handle to the uroup.</param>
 * <param name="hUser" type="IN">Handle to the user</param>
 * <return>TRUE: The user is member of that group. FALSE: The user is not member.</return>
 */
RTS_BOOL CDECL UserMgrGroupHasUser(RTS_HANDLE hGroup, RTS_HANDLE hUser);
typedef RTS_BOOL (CDECL * PFUSERMGRGROUPHASUSER) (RTS_HANDLE hGroup, RTS_HANDLE hUser);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPHASUSER_NOTIMPLEMENTED)
	#define USE_UserMgrGroupHasUser
	#define EXT_UserMgrGroupHasUser
	#define GET_UserMgrGroupHasUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupHasUser(p0,p1)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGroupHasUser  FALSE
	#define EXP_UserMgrGroupHasUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupHasUser
	#define EXT_UserMgrGroupHasUser
	#define GET_UserMgrGroupHasUser(fl)  CAL_CMGETAPI( "UserMgrGroupHasUser" ) 
	#define CAL_UserMgrGroupHasUser  UserMgrGroupHasUser
	#define CHK_UserMgrGroupHasUser  TRUE
	#define EXP_UserMgrGroupHasUser  CAL_CMEXPAPI( "UserMgrGroupHasUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupHasUser
	#define EXT_UserMgrGroupHasUser
	#define GET_UserMgrGroupHasUser(fl)  CAL_CMGETAPI( "UserMgrGroupHasUser" ) 
	#define CAL_UserMgrGroupHasUser  UserMgrGroupHasUser
	#define CHK_UserMgrGroupHasUser  TRUE
	#define EXP_UserMgrGroupHasUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupHasUser", (RTS_UINTPTR)UserMgrGroupHasUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupHasUser
	#define EXT_CmpUserMgrUserMgrGroupHasUser
	#define GET_CmpUserMgrUserMgrGroupHasUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupHasUser pICmpUserMgr->IUserMgrGroupHasUser
	#define CHK_CmpUserMgrUserMgrGroupHasUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupHasUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupHasUser
	#define EXT_UserMgrGroupHasUser
	#define GET_UserMgrGroupHasUser(fl)  CAL_CMGETAPI( "UserMgrGroupHasUser" ) 
	#define CAL_UserMgrGroupHasUser pICmpUserMgr->IUserMgrGroupHasUser
	#define CHK_UserMgrGroupHasUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupHasUser  CAL_CMEXPAPI( "UserMgrGroupHasUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupHasUser  PFUSERMGRGROUPHASUSER pfUserMgrGroupHasUser;
	#define EXT_UserMgrGroupHasUser  extern PFUSERMGRGROUPHASUSER pfUserMgrGroupHasUser;
	#define GET_UserMgrGroupHasUser(fl)  s_pfCMGetAPI2( "UserMgrGroupHasUser", (RTS_VOID_FCTPTR *)&pfUserMgrGroupHasUser, (fl), 0, 0)
	#define CAL_UserMgrGroupHasUser  pfUserMgrGroupHasUser
	#define CHK_UserMgrGroupHasUser  (pfUserMgrGroupHasUser != NULL)
	#define EXP_UserMgrGroupHasUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupHasUser", (RTS_UINTPTR)UserMgrGroupHasUser, 0, 0) 
#endif




/**
 * <description>Retrieves user name from a user group.</description>
 * <param name="hUser" type="IN">Handle to the group.</param>
 * <param name="pName" type="IN">Pointer to the buffer where to store the name.</param>
 * <param name="maxLen" type="IN">Maximum size of the user name.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Retrieving cache value successful. Value copied to pValue.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_INVALID_HANDLE">hUser was not a valid user handle.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pName was NULL.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">The buffer was too small.</errorcode>
 */
RTS_RESULT CDECL UserMgrGroupGetName(RTS_HANDLE hGroup, char *pName, RTS_UI32 maxLen);
typedef RTS_RESULT (CDECL * PFUSERMGRGROUPGETNAME) (RTS_HANDLE hGroup, char *pName, RTS_UI32 maxLen);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPGETNAME_NOTIMPLEMENTED)
	#define USE_UserMgrGroupGetName
	#define EXT_UserMgrGroupGetName
	#define GET_UserMgrGroupGetName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupGetName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGroupGetName  FALSE
	#define EXP_UserMgrGroupGetName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupGetName
	#define EXT_UserMgrGroupGetName
	#define GET_UserMgrGroupGetName(fl)  CAL_CMGETAPI( "UserMgrGroupGetName" ) 
	#define CAL_UserMgrGroupGetName  UserMgrGroupGetName
	#define CHK_UserMgrGroupGetName  TRUE
	#define EXP_UserMgrGroupGetName  CAL_CMEXPAPI( "UserMgrGroupGetName" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupGetName
	#define EXT_UserMgrGroupGetName
	#define GET_UserMgrGroupGetName(fl)  CAL_CMGETAPI( "UserMgrGroupGetName" ) 
	#define CAL_UserMgrGroupGetName  UserMgrGroupGetName
	#define CHK_UserMgrGroupGetName  TRUE
	#define EXP_UserMgrGroupGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetName", (RTS_UINTPTR)UserMgrGroupGetName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupGetName
	#define EXT_CmpUserMgrUserMgrGroupGetName
	#define GET_CmpUserMgrUserMgrGroupGetName  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupGetName pICmpUserMgr->IUserMgrGroupGetName
	#define CHK_CmpUserMgrUserMgrGroupGetName (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupGetName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupGetName
	#define EXT_UserMgrGroupGetName
	#define GET_UserMgrGroupGetName(fl)  CAL_CMGETAPI( "UserMgrGroupGetName" ) 
	#define CAL_UserMgrGroupGetName pICmpUserMgr->IUserMgrGroupGetName
	#define CHK_UserMgrGroupGetName (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupGetName  CAL_CMEXPAPI( "UserMgrGroupGetName" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupGetName  PFUSERMGRGROUPGETNAME pfUserMgrGroupGetName;
	#define EXT_UserMgrGroupGetName  extern PFUSERMGRGROUPGETNAME pfUserMgrGroupGetName;
	#define GET_UserMgrGroupGetName(fl)  s_pfCMGetAPI2( "UserMgrGroupGetName", (RTS_VOID_FCTPTR *)&pfUserMgrGroupGetName, (fl), 0, 0)
	#define CAL_UserMgrGroupGetName  pfUserMgrGroupGetName
	#define CHK_UserMgrGroupGetName  (pfUserMgrGroupGetName != NULL)
	#define EXP_UserMgrGroupGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetName", (RTS_UINTPTR)UserMgrGroupGetName, 0, 0) 
#endif






/**
 * <description>Adds a user to user database</description>
 * <param name="hUser" type="IN">Handle of user with the appropriate rights</param>
 * <param name="pszUserName" type="IN">Name of the user.</param>
 * <param name="pCredentials" type="IN">Credentials of the user.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pszUserName was NULL.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory to add</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">pszUserName is already added.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User has no access rights</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PASSWORD_POLICY">Password does not fulfill the password policy requirements</errorcode>
 */
RTS_RESULT CDECL UserMgrAddUser(RTS_HANDLE hUser, char *pszUserName, UserMgrCredentials *pCredentials);
typedef RTS_RESULT (CDECL * PFUSERMGRADDUSER) (RTS_HANDLE hUser, char *pszUserName, UserMgrCredentials *pCredentials);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRADDUSER_NOTIMPLEMENTED)
	#define USE_UserMgrAddUser
	#define EXT_UserMgrAddUser
	#define GET_UserMgrAddUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrAddUser(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrAddUser  FALSE
	#define EXP_UserMgrAddUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrAddUser
	#define EXT_UserMgrAddUser
	#define GET_UserMgrAddUser(fl)  CAL_CMGETAPI( "UserMgrAddUser" ) 
	#define CAL_UserMgrAddUser  UserMgrAddUser
	#define CHK_UserMgrAddUser  TRUE
	#define EXP_UserMgrAddUser  CAL_CMEXPAPI( "UserMgrAddUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrAddUser
	#define EXT_UserMgrAddUser
	#define GET_UserMgrAddUser(fl)  CAL_CMGETAPI( "UserMgrAddUser" ) 
	#define CAL_UserMgrAddUser  UserMgrAddUser
	#define CHK_UserMgrAddUser  TRUE
	#define EXP_UserMgrAddUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddUser", (RTS_UINTPTR)UserMgrAddUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrAddUser
	#define EXT_CmpUserMgrUserMgrAddUser
	#define GET_CmpUserMgrUserMgrAddUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrAddUser pICmpUserMgr->IUserMgrAddUser
	#define CHK_CmpUserMgrUserMgrAddUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrAddUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrAddUser
	#define EXT_UserMgrAddUser
	#define GET_UserMgrAddUser(fl)  CAL_CMGETAPI( "UserMgrAddUser" ) 
	#define CAL_UserMgrAddUser pICmpUserMgr->IUserMgrAddUser
	#define CHK_UserMgrAddUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrAddUser  CAL_CMEXPAPI( "UserMgrAddUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrAddUser  PFUSERMGRADDUSER pfUserMgrAddUser;
	#define EXT_UserMgrAddUser  extern PFUSERMGRADDUSER pfUserMgrAddUser;
	#define GET_UserMgrAddUser(fl)  s_pfCMGetAPI2( "UserMgrAddUser", (RTS_VOID_FCTPTR *)&pfUserMgrAddUser, (fl), 0, 0)
	#define CAL_UserMgrAddUser  pfUserMgrAddUser
	#define CHK_UserMgrAddUser  (pfUserMgrAddUser != NULL)
	#define EXP_UserMgrAddUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddUser", (RTS_UINTPTR)UserMgrAddUser, 0, 0) 
#endif




/**
 * <description>Removes a user from user database</description>
 * <param name="hUser" type="IN">Handle of user with the appropriate rights</param>
 * <param name="pszUserName" type="IN">Name of the user.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pszUserName was NULL.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User has no access rights</errorcode>
 */
RTS_RESULT CDECL UserMgrRemoveUser(RTS_HANDLE hUser, char *pszUserName);
typedef RTS_RESULT (CDECL * PFUSERMGRREMOVEUSER) (RTS_HANDLE hUser, char *pszUserName);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREMOVEUSER_NOTIMPLEMENTED)
	#define USE_UserMgrRemoveUser
	#define EXT_UserMgrRemoveUser
	#define GET_UserMgrRemoveUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRemoveUser(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRemoveUser  FALSE
	#define EXP_UserMgrRemoveUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRemoveUser
	#define EXT_UserMgrRemoveUser
	#define GET_UserMgrRemoveUser(fl)  CAL_CMGETAPI( "UserMgrRemoveUser" ) 
	#define CAL_UserMgrRemoveUser  UserMgrRemoveUser
	#define CHK_UserMgrRemoveUser  TRUE
	#define EXP_UserMgrRemoveUser  CAL_CMEXPAPI( "UserMgrRemoveUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRemoveUser
	#define EXT_UserMgrRemoveUser
	#define GET_UserMgrRemoveUser(fl)  CAL_CMGETAPI( "UserMgrRemoveUser" ) 
	#define CAL_UserMgrRemoveUser  UserMgrRemoveUser
	#define CHK_UserMgrRemoveUser  TRUE
	#define EXP_UserMgrRemoveUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveUser", (RTS_UINTPTR)UserMgrRemoveUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRemoveUser
	#define EXT_CmpUserMgrUserMgrRemoveUser
	#define GET_CmpUserMgrUserMgrRemoveUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrRemoveUser pICmpUserMgr->IUserMgrRemoveUser
	#define CHK_CmpUserMgrUserMgrRemoveUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRemoveUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRemoveUser
	#define EXT_UserMgrRemoveUser
	#define GET_UserMgrRemoveUser(fl)  CAL_CMGETAPI( "UserMgrRemoveUser" ) 
	#define CAL_UserMgrRemoveUser pICmpUserMgr->IUserMgrRemoveUser
	#define CHK_UserMgrRemoveUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrRemoveUser  CAL_CMEXPAPI( "UserMgrRemoveUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRemoveUser  PFUSERMGRREMOVEUSER pfUserMgrRemoveUser;
	#define EXT_UserMgrRemoveUser  extern PFUSERMGRREMOVEUSER pfUserMgrRemoveUser;
	#define GET_UserMgrRemoveUser(fl)  s_pfCMGetAPI2( "UserMgrRemoveUser", (RTS_VOID_FCTPTR *)&pfUserMgrRemoveUser, (fl), 0, 0)
	#define CAL_UserMgrRemoveUser  pfUserMgrRemoveUser
	#define CHK_UserMgrRemoveUser  (pfUserMgrRemoveUser != NULL)
	#define EXP_UserMgrRemoveUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveUser", (RTS_UINTPTR)UserMgrRemoveUser, 0, 0) 
#endif




/**
 * <description>Adds a group to user group database</description>
 * <param name="hUser" type="IN">Handle of user with the appropriate rights</param>
 * <param name="pszGroupName" type="IN">Name of the group.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group could be added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pszGroupName was NULL.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory to add group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">pszGroupName is already added.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Creating user or group lists failed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User has no access rights</errorcode>
 */
RTS_RESULT CDECL UserMgrAddGroup(RTS_HANDLE hUser, char *pszGroupName);
typedef RTS_RESULT (CDECL * PFUSERMGRADDGROUP) (RTS_HANDLE hUser, char *pszGroupName);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRADDGROUP_NOTIMPLEMENTED)
	#define USE_UserMgrAddGroup
	#define EXT_UserMgrAddGroup
	#define GET_UserMgrAddGroup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrAddGroup(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrAddGroup  FALSE
	#define EXP_UserMgrAddGroup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrAddGroup
	#define EXT_UserMgrAddGroup
	#define GET_UserMgrAddGroup(fl)  CAL_CMGETAPI( "UserMgrAddGroup" ) 
	#define CAL_UserMgrAddGroup  UserMgrAddGroup
	#define CHK_UserMgrAddGroup  TRUE
	#define EXP_UserMgrAddGroup  CAL_CMEXPAPI( "UserMgrAddGroup" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrAddGroup
	#define EXT_UserMgrAddGroup
	#define GET_UserMgrAddGroup(fl)  CAL_CMGETAPI( "UserMgrAddGroup" ) 
	#define CAL_UserMgrAddGroup  UserMgrAddGroup
	#define CHK_UserMgrAddGroup  TRUE
	#define EXP_UserMgrAddGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddGroup", (RTS_UINTPTR)UserMgrAddGroup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrAddGroup
	#define EXT_CmpUserMgrUserMgrAddGroup
	#define GET_CmpUserMgrUserMgrAddGroup  ERR_OK
	#define CAL_CmpUserMgrUserMgrAddGroup pICmpUserMgr->IUserMgrAddGroup
	#define CHK_CmpUserMgrUserMgrAddGroup (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrAddGroup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrAddGroup
	#define EXT_UserMgrAddGroup
	#define GET_UserMgrAddGroup(fl)  CAL_CMGETAPI( "UserMgrAddGroup" ) 
	#define CAL_UserMgrAddGroup pICmpUserMgr->IUserMgrAddGroup
	#define CHK_UserMgrAddGroup (pICmpUserMgr != NULL)
	#define EXP_UserMgrAddGroup  CAL_CMEXPAPI( "UserMgrAddGroup" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrAddGroup  PFUSERMGRADDGROUP pfUserMgrAddGroup;
	#define EXT_UserMgrAddGroup  extern PFUSERMGRADDGROUP pfUserMgrAddGroup;
	#define GET_UserMgrAddGroup(fl)  s_pfCMGetAPI2( "UserMgrAddGroup", (RTS_VOID_FCTPTR *)&pfUserMgrAddGroup, (fl), 0, 0)
	#define CAL_UserMgrAddGroup  pfUserMgrAddGroup
	#define CHK_UserMgrAddGroup  (pfUserMgrAddGroup != NULL)
	#define EXP_UserMgrAddGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAddGroup", (RTS_UINTPTR)UserMgrAddGroup, 0, 0) 
#endif




/**
 * <description>Removes a group from user group database</description>
 * <param name="hUser" type="IN">Handle of user with the appropriate rights</param>
 * <param name="pszGroupName" type="IN">Name of the group.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group was successfully removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pszGroupName was NULL.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User has no access rights</errorcode>
 */
RTS_RESULT CDECL UserMgrRemoveGroup(RTS_HANDLE hUser, char *pszGroupName);
typedef RTS_RESULT (CDECL * PFUSERMGRREMOVEGROUP) (RTS_HANDLE hUser, char *pszGroupName);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRREMOVEGROUP_NOTIMPLEMENTED)
	#define USE_UserMgrRemoveGroup
	#define EXT_UserMgrRemoveGroup
	#define GET_UserMgrRemoveGroup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRemoveGroup(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRemoveGroup  FALSE
	#define EXP_UserMgrRemoveGroup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRemoveGroup
	#define EXT_UserMgrRemoveGroup
	#define GET_UserMgrRemoveGroup(fl)  CAL_CMGETAPI( "UserMgrRemoveGroup" ) 
	#define CAL_UserMgrRemoveGroup  UserMgrRemoveGroup
	#define CHK_UserMgrRemoveGroup  TRUE
	#define EXP_UserMgrRemoveGroup  CAL_CMEXPAPI( "UserMgrRemoveGroup" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRemoveGroup
	#define EXT_UserMgrRemoveGroup
	#define GET_UserMgrRemoveGroup(fl)  CAL_CMGETAPI( "UserMgrRemoveGroup" ) 
	#define CAL_UserMgrRemoveGroup  UserMgrRemoveGroup
	#define CHK_UserMgrRemoveGroup  TRUE
	#define EXP_UserMgrRemoveGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveGroup", (RTS_UINTPTR)UserMgrRemoveGroup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRemoveGroup
	#define EXT_CmpUserMgrUserMgrRemoveGroup
	#define GET_CmpUserMgrUserMgrRemoveGroup  ERR_OK
	#define CAL_CmpUserMgrUserMgrRemoveGroup pICmpUserMgr->IUserMgrRemoveGroup
	#define CHK_CmpUserMgrUserMgrRemoveGroup (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRemoveGroup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRemoveGroup
	#define EXT_UserMgrRemoveGroup
	#define GET_UserMgrRemoveGroup(fl)  CAL_CMGETAPI( "UserMgrRemoveGroup" ) 
	#define CAL_UserMgrRemoveGroup pICmpUserMgr->IUserMgrRemoveGroup
	#define CHK_UserMgrRemoveGroup (pICmpUserMgr != NULL)
	#define EXP_UserMgrRemoveGroup  CAL_CMEXPAPI( "UserMgrRemoveGroup" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRemoveGroup  PFUSERMGRREMOVEGROUP pfUserMgrRemoveGroup;
	#define EXT_UserMgrRemoveGroup  extern PFUSERMGRREMOVEGROUP pfUserMgrRemoveGroup;
	#define GET_UserMgrRemoveGroup(fl)  s_pfCMGetAPI2( "UserMgrRemoveGroup", (RTS_VOID_FCTPTR *)&pfUserMgrRemoveGroup, (fl), 0, 0)
	#define CAL_UserMgrRemoveGroup  pfUserMgrRemoveGroup
	#define CHK_UserMgrRemoveGroup  (pfUserMgrRemoveGroup != NULL)
	#define EXP_UserMgrRemoveGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRemoveGroup", (RTS_UINTPTR)UserMgrRemoveGroup, 0, 0) 
#endif




/**
 * <description>Adds a user to a group in the user group database</description>
 * <param name="hUser" type="IN">Handle of user with the appropriate rights</param>
 * <param name="pszGroupName" type="IN">Name of the group.</param>
 * <param name="pszUserName" type="IN">Name of the user.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pszGroupName or pszUserName was NULL.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory to add user to group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">pszUserName is already added.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User has no access rights</errorcode>
 */
RTS_RESULT CDECL UserMgrGroupAddUser(RTS_HANDLE hUser, char *pszGroupName, char *pszUserName);
typedef RTS_RESULT (CDECL * PFUSERMGRGROUPADDUSER) (RTS_HANDLE hUser, char *pszGroupName, char *pszUserName);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPADDUSER_NOTIMPLEMENTED)
	#define USE_UserMgrGroupAddUser
	#define EXT_UserMgrGroupAddUser
	#define GET_UserMgrGroupAddUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupAddUser(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGroupAddUser  FALSE
	#define EXP_UserMgrGroupAddUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupAddUser
	#define EXT_UserMgrGroupAddUser
	#define GET_UserMgrGroupAddUser(fl)  CAL_CMGETAPI( "UserMgrGroupAddUser" ) 
	#define CAL_UserMgrGroupAddUser  UserMgrGroupAddUser
	#define CHK_UserMgrGroupAddUser  TRUE
	#define EXP_UserMgrGroupAddUser  CAL_CMEXPAPI( "UserMgrGroupAddUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupAddUser
	#define EXT_UserMgrGroupAddUser
	#define GET_UserMgrGroupAddUser(fl)  CAL_CMGETAPI( "UserMgrGroupAddUser" ) 
	#define CAL_UserMgrGroupAddUser  UserMgrGroupAddUser
	#define CHK_UserMgrGroupAddUser  TRUE
	#define EXP_UserMgrGroupAddUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupAddUser", (RTS_UINTPTR)UserMgrGroupAddUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupAddUser
	#define EXT_CmpUserMgrUserMgrGroupAddUser
	#define GET_CmpUserMgrUserMgrGroupAddUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupAddUser pICmpUserMgr->IUserMgrGroupAddUser
	#define CHK_CmpUserMgrUserMgrGroupAddUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupAddUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupAddUser
	#define EXT_UserMgrGroupAddUser
	#define GET_UserMgrGroupAddUser(fl)  CAL_CMGETAPI( "UserMgrGroupAddUser" ) 
	#define CAL_UserMgrGroupAddUser pICmpUserMgr->IUserMgrGroupAddUser
	#define CHK_UserMgrGroupAddUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupAddUser  CAL_CMEXPAPI( "UserMgrGroupAddUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupAddUser  PFUSERMGRGROUPADDUSER pfUserMgrGroupAddUser;
	#define EXT_UserMgrGroupAddUser  extern PFUSERMGRGROUPADDUSER pfUserMgrGroupAddUser;
	#define GET_UserMgrGroupAddUser(fl)  s_pfCMGetAPI2( "UserMgrGroupAddUser", (RTS_VOID_FCTPTR *)&pfUserMgrGroupAddUser, (fl), 0, 0)
	#define CAL_UserMgrGroupAddUser  pfUserMgrGroupAddUser
	#define CHK_UserMgrGroupAddUser  (pfUserMgrGroupAddUser != NULL)
	#define EXP_UserMgrGroupAddUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupAddUser", (RTS_UINTPTR)UserMgrGroupAddUser, 0, 0) 
#endif




/**
 * <description>Removes user from a group in the user group database</description>
 * <param name="hUser" type="IN">Handle of user with the appropriate rights</param>
 * <param name="pszGroupName" type="IN">Name of the group.</param>
 * <param name="pszUserName" type="IN">Name of the user.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">User could be removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pszGroupName or pszUserName was NULL.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User has no access rights</errorcode>
 */
RTS_RESULT CDECL UserMgrGroupRemoveUser(RTS_HANDLE hUser, char *pszGroupName, char *pszUserName);
typedef RTS_RESULT (CDECL * PFUSERMGRGROUPREMOVEUSER) (RTS_HANDLE hUser, char *pszGroupName, char *pszUserName);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPREMOVEUSER_NOTIMPLEMENTED)
	#define USE_UserMgrGroupRemoveUser
	#define EXT_UserMgrGroupRemoveUser
	#define GET_UserMgrGroupRemoveUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupRemoveUser(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGroupRemoveUser  FALSE
	#define EXP_UserMgrGroupRemoveUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupRemoveUser
	#define EXT_UserMgrGroupRemoveUser
	#define GET_UserMgrGroupRemoveUser(fl)  CAL_CMGETAPI( "UserMgrGroupRemoveUser" ) 
	#define CAL_UserMgrGroupRemoveUser  UserMgrGroupRemoveUser
	#define CHK_UserMgrGroupRemoveUser  TRUE
	#define EXP_UserMgrGroupRemoveUser  CAL_CMEXPAPI( "UserMgrGroupRemoveUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupRemoveUser
	#define EXT_UserMgrGroupRemoveUser
	#define GET_UserMgrGroupRemoveUser(fl)  CAL_CMGETAPI( "UserMgrGroupRemoveUser" ) 
	#define CAL_UserMgrGroupRemoveUser  UserMgrGroupRemoveUser
	#define CHK_UserMgrGroupRemoveUser  TRUE
	#define EXP_UserMgrGroupRemoveUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupRemoveUser", (RTS_UINTPTR)UserMgrGroupRemoveUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupRemoveUser
	#define EXT_CmpUserMgrUserMgrGroupRemoveUser
	#define GET_CmpUserMgrUserMgrGroupRemoveUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupRemoveUser pICmpUserMgr->IUserMgrGroupRemoveUser
	#define CHK_CmpUserMgrUserMgrGroupRemoveUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupRemoveUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupRemoveUser
	#define EXT_UserMgrGroupRemoveUser
	#define GET_UserMgrGroupRemoveUser(fl)  CAL_CMGETAPI( "UserMgrGroupRemoveUser" ) 
	#define CAL_UserMgrGroupRemoveUser pICmpUserMgr->IUserMgrGroupRemoveUser
	#define CHK_UserMgrGroupRemoveUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupRemoveUser  CAL_CMEXPAPI( "UserMgrGroupRemoveUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupRemoveUser  PFUSERMGRGROUPREMOVEUSER pfUserMgrGroupRemoveUser;
	#define EXT_UserMgrGroupRemoveUser  extern PFUSERMGRGROUPREMOVEUSER pfUserMgrGroupRemoveUser;
	#define GET_UserMgrGroupRemoveUser(fl)  s_pfCMGetAPI2( "UserMgrGroupRemoveUser", (RTS_VOID_FCTPTR *)&pfUserMgrGroupRemoveUser, (fl), 0, 0)
	#define CAL_UserMgrGroupRemoveUser  pfUserMgrGroupRemoveUser
	#define CHK_UserMgrGroupRemoveUser  (pfUserMgrGroupRemoveUser != NULL)
	#define EXP_UserMgrGroupRemoveUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupRemoveUser", (RTS_UINTPTR)UserMgrGroupRemoveUser, 0, 0) 
#endif




/**
 * <description>Adds the error tag to the reply stream of an online service, if the service is denied because of a failed access right (authorization error)!</description>
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
 *	Adds a new object to the user management, so the access rights can be configured on that object.
 *	NOTE:
 *	The name of the object must include the full name-space with "Device" as the root node, e.g. "Device.MyObject" or if it's a file system object with "/" as the root node.
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
 * <description>Removes an object from the user management and all its children</description>
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
 *	Adds a new object under a father object specified by its name to the user management, so the access rights can be configured on that object.
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
 *	Sets the used access rights for the object specified by handle.
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
 *	Gets the used access rights for the object specified by handle.
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
 *	Opens an existing object.
 *	NOTE:
 *	The name of the object must include the full name space with "Device" as the root node, e.g. "Device.MyObject" or if it's a file system object with "/" as the root node.
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
 * <description>Closes an object</description>
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
 * <description>
 *	Adds a new object to the object management, so the access rights can be configured on that object.
 *	NOTE:
 *	The name of the object must include the full name space with "Device" as the root node, e.g. "Device.MyObject" or if it's a file system object with "/" as the root node.
 * </description>
 * <param name="pszObject" type="IN">Full object name (see object tree)</param>
 * <param name="properties" type="IN">Properties of the object. See Properties.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Object could be added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Object invalid = NULL or empty</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">If memory could not be allocated to store this object</errorcode>
 * <result>Handle to the object or RTS_INVALID_HANDLE if it could not be added</result>
 */
RTS_HANDLE CDECL UserMgrObjectAdd2(char *pszObject, RTS_UI32 properties, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTADD2) (char *pszObject, RTS_UI32 properties, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTADD2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectAdd2
	#define EXT_UserMgrObjectAdd2
	#define GET_UserMgrObjectAdd2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectAdd2(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectAdd2  FALSE
	#define EXP_UserMgrObjectAdd2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectAdd2
	#define EXT_UserMgrObjectAdd2
	#define GET_UserMgrObjectAdd2(fl)  CAL_CMGETAPI( "UserMgrObjectAdd2" ) 
	#define CAL_UserMgrObjectAdd2  UserMgrObjectAdd2
	#define CHK_UserMgrObjectAdd2  TRUE
	#define EXP_UserMgrObjectAdd2  CAL_CMEXPAPI( "UserMgrObjectAdd2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectAdd2
	#define EXT_UserMgrObjectAdd2
	#define GET_UserMgrObjectAdd2(fl)  CAL_CMGETAPI( "UserMgrObjectAdd2" ) 
	#define CAL_UserMgrObjectAdd2  UserMgrObjectAdd2
	#define CHK_UserMgrObjectAdd2  TRUE
	#define EXP_UserMgrObjectAdd2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAdd2", (RTS_UINTPTR)UserMgrObjectAdd2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectAdd2
	#define EXT_CmpUserMgrUserMgrObjectAdd2
	#define GET_CmpUserMgrUserMgrObjectAdd2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectAdd2 pICmpUserMgr->IUserMgrObjectAdd2
	#define CHK_CmpUserMgrUserMgrObjectAdd2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectAdd2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectAdd2
	#define EXT_UserMgrObjectAdd2
	#define GET_UserMgrObjectAdd2(fl)  CAL_CMGETAPI( "UserMgrObjectAdd2" ) 
	#define CAL_UserMgrObjectAdd2 pICmpUserMgr->IUserMgrObjectAdd2
	#define CHK_UserMgrObjectAdd2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectAdd2  CAL_CMEXPAPI( "UserMgrObjectAdd2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectAdd2  PFUSERMGROBJECTADD2 pfUserMgrObjectAdd2;
	#define EXT_UserMgrObjectAdd2  extern PFUSERMGROBJECTADD2 pfUserMgrObjectAdd2;
	#define GET_UserMgrObjectAdd2(fl)  s_pfCMGetAPI2( "UserMgrObjectAdd2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectAdd2, (fl), 0, 0)
	#define CAL_UserMgrObjectAdd2  pfUserMgrObjectAdd2
	#define CHK_UserMgrObjectAdd2  (pfUserMgrObjectAdd2 != NULL)
	#define EXP_UserMgrObjectAdd2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAdd2", (RTS_UINTPTR)UserMgrObjectAdd2, 0, 0) 
#endif


 

/**
 * <description>Adds a group to the specified object to manage access rights</description>
 * <param name="hObject" type="IN">Handle to object</param>
 * <param name="hGroup" type="IN">Handle to the group that should be attached</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group could be added or is already added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory to add group</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectAddGroup(RTS_HANDLE hObject, RTS_HANDLE hGroup);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTADDGROUP) (RTS_HANDLE hObject, RTS_HANDLE hGroup);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTADDGROUP_NOTIMPLEMENTED)
	#define USE_UserMgrObjectAddGroup
	#define EXT_UserMgrObjectAddGroup
	#define GET_UserMgrObjectAddGroup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectAddGroup(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectAddGroup  FALSE
	#define EXP_UserMgrObjectAddGroup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectAddGroup
	#define EXT_UserMgrObjectAddGroup
	#define GET_UserMgrObjectAddGroup(fl)  CAL_CMGETAPI( "UserMgrObjectAddGroup" ) 
	#define CAL_UserMgrObjectAddGroup  UserMgrObjectAddGroup
	#define CHK_UserMgrObjectAddGroup  TRUE
	#define EXP_UserMgrObjectAddGroup  CAL_CMEXPAPI( "UserMgrObjectAddGroup" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectAddGroup
	#define EXT_UserMgrObjectAddGroup
	#define GET_UserMgrObjectAddGroup(fl)  CAL_CMGETAPI( "UserMgrObjectAddGroup" ) 
	#define CAL_UserMgrObjectAddGroup  UserMgrObjectAddGroup
	#define CHK_UserMgrObjectAddGroup  TRUE
	#define EXP_UserMgrObjectAddGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAddGroup", (RTS_UINTPTR)UserMgrObjectAddGroup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectAddGroup
	#define EXT_CmpUserMgrUserMgrObjectAddGroup
	#define GET_CmpUserMgrUserMgrObjectAddGroup  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectAddGroup pICmpUserMgr->IUserMgrObjectAddGroup
	#define CHK_CmpUserMgrUserMgrObjectAddGroup (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectAddGroup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectAddGroup
	#define EXT_UserMgrObjectAddGroup
	#define GET_UserMgrObjectAddGroup(fl)  CAL_CMGETAPI( "UserMgrObjectAddGroup" ) 
	#define CAL_UserMgrObjectAddGroup pICmpUserMgr->IUserMgrObjectAddGroup
	#define CHK_UserMgrObjectAddGroup (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectAddGroup  CAL_CMEXPAPI( "UserMgrObjectAddGroup" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectAddGroup  PFUSERMGROBJECTADDGROUP pfUserMgrObjectAddGroup;
	#define EXT_UserMgrObjectAddGroup  extern PFUSERMGROBJECTADDGROUP pfUserMgrObjectAddGroup;
	#define GET_UserMgrObjectAddGroup(fl)  s_pfCMGetAPI2( "UserMgrObjectAddGroup", (RTS_VOID_FCTPTR *)&pfUserMgrObjectAddGroup, (fl), 0, 0)
	#define CAL_UserMgrObjectAddGroup  pfUserMgrObjectAddGroup
	#define CHK_UserMgrObjectAddGroup  (pfUserMgrObjectAddGroup != NULL)
	#define EXP_UserMgrObjectAddGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAddGroup", (RTS_UINTPTR)UserMgrObjectAddGroup, 0, 0) 
#endif




/**
 * <description>Removes a group from the specified object</description>
 * <param name="hObject" type="IN">Handle to object</param>
 * <param name="hGroup" type="IN">Handle to the group that should be removed</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group could be removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectRemoveGroup(RTS_HANDLE hObject, RTS_HANDLE hGroup);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTREMOVEGROUP) (RTS_HANDLE hObject, RTS_HANDLE hGroup);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTREMOVEGROUP_NOTIMPLEMENTED)
	#define USE_UserMgrObjectRemoveGroup
	#define EXT_UserMgrObjectRemoveGroup
	#define GET_UserMgrObjectRemoveGroup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectRemoveGroup(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectRemoveGroup  FALSE
	#define EXP_UserMgrObjectRemoveGroup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectRemoveGroup
	#define EXT_UserMgrObjectRemoveGroup
	#define GET_UserMgrObjectRemoveGroup(fl)  CAL_CMGETAPI( "UserMgrObjectRemoveGroup" ) 
	#define CAL_UserMgrObjectRemoveGroup  UserMgrObjectRemoveGroup
	#define CHK_UserMgrObjectRemoveGroup  TRUE
	#define EXP_UserMgrObjectRemoveGroup  CAL_CMEXPAPI( "UserMgrObjectRemoveGroup" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectRemoveGroup
	#define EXT_UserMgrObjectRemoveGroup
	#define GET_UserMgrObjectRemoveGroup(fl)  CAL_CMGETAPI( "UserMgrObjectRemoveGroup" ) 
	#define CAL_UserMgrObjectRemoveGroup  UserMgrObjectRemoveGroup
	#define CHK_UserMgrObjectRemoveGroup  TRUE
	#define EXP_UserMgrObjectRemoveGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectRemoveGroup", (RTS_UINTPTR)UserMgrObjectRemoveGroup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectRemoveGroup
	#define EXT_CmpUserMgrUserMgrObjectRemoveGroup
	#define GET_CmpUserMgrUserMgrObjectRemoveGroup  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectRemoveGroup pICmpUserMgr->IUserMgrObjectRemoveGroup
	#define CHK_CmpUserMgrUserMgrObjectRemoveGroup (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectRemoveGroup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectRemoveGroup
	#define EXT_UserMgrObjectRemoveGroup
	#define GET_UserMgrObjectRemoveGroup(fl)  CAL_CMGETAPI( "UserMgrObjectRemoveGroup" ) 
	#define CAL_UserMgrObjectRemoveGroup pICmpUserMgr->IUserMgrObjectRemoveGroup
	#define CHK_UserMgrObjectRemoveGroup (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectRemoveGroup  CAL_CMEXPAPI( "UserMgrObjectRemoveGroup" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectRemoveGroup  PFUSERMGROBJECTREMOVEGROUP pfUserMgrObjectRemoveGroup;
	#define EXT_UserMgrObjectRemoveGroup  extern PFUSERMGROBJECTREMOVEGROUP pfUserMgrObjectRemoveGroup;
	#define GET_UserMgrObjectRemoveGroup(fl)  s_pfCMGetAPI2( "UserMgrObjectRemoveGroup", (RTS_VOID_FCTPTR *)&pfUserMgrObjectRemoveGroup, (fl), 0, 0)
	#define CAL_UserMgrObjectRemoveGroup  pfUserMgrObjectRemoveGroup
	#define CHK_UserMgrObjectRemoveGroup  (pfUserMgrObjectRemoveGroup != NULL)
	#define EXP_UserMgrObjectRemoveGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectRemoveGroup", (RTS_UINTPTR)UserMgrObjectRemoveGroup, 0, 0) 
#endif




/**
 * <description>Sets the access rights for the group at the specified object</description>
 * <param name="hObject" type="IN">Handle to object</param>
 * <param name="hGroup" type="IN">Handle to the group</param>
 * <element name="ulRights" type="IN">Rights of the group. See category "User rights" for details</element>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Access rights could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectSetGroupRights(RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulRights);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTSETGROUPRIGHTS) (RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTSETGROUPRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrObjectSetGroupRights
	#define EXT_UserMgrObjectSetGroupRights
	#define GET_UserMgrObjectSetGroupRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectSetGroupRights(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectSetGroupRights  FALSE
	#define EXP_UserMgrObjectSetGroupRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectSetGroupRights
	#define EXT_UserMgrObjectSetGroupRights
	#define GET_UserMgrObjectSetGroupRights(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupRights" ) 
	#define CAL_UserMgrObjectSetGroupRights  UserMgrObjectSetGroupRights
	#define CHK_UserMgrObjectSetGroupRights  TRUE
	#define EXP_UserMgrObjectSetGroupRights  CAL_CMEXPAPI( "UserMgrObjectSetGroupRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectSetGroupRights
	#define EXT_UserMgrObjectSetGroupRights
	#define GET_UserMgrObjectSetGroupRights(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupRights" ) 
	#define CAL_UserMgrObjectSetGroupRights  UserMgrObjectSetGroupRights
	#define CHK_UserMgrObjectSetGroupRights  TRUE
	#define EXP_UserMgrObjectSetGroupRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetGroupRights", (RTS_UINTPTR)UserMgrObjectSetGroupRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectSetGroupRights
	#define EXT_CmpUserMgrUserMgrObjectSetGroupRights
	#define GET_CmpUserMgrUserMgrObjectSetGroupRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectSetGroupRights pICmpUserMgr->IUserMgrObjectSetGroupRights
	#define CHK_CmpUserMgrUserMgrObjectSetGroupRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectSetGroupRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectSetGroupRights
	#define EXT_UserMgrObjectSetGroupRights
	#define GET_UserMgrObjectSetGroupRights(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupRights" ) 
	#define CAL_UserMgrObjectSetGroupRights pICmpUserMgr->IUserMgrObjectSetGroupRights
	#define CHK_UserMgrObjectSetGroupRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectSetGroupRights  CAL_CMEXPAPI( "UserMgrObjectSetGroupRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectSetGroupRights  PFUSERMGROBJECTSETGROUPRIGHTS pfUserMgrObjectSetGroupRights;
	#define EXT_UserMgrObjectSetGroupRights  extern PFUSERMGROBJECTSETGROUPRIGHTS pfUserMgrObjectSetGroupRights;
	#define GET_UserMgrObjectSetGroupRights(fl)  s_pfCMGetAPI2( "UserMgrObjectSetGroupRights", (RTS_VOID_FCTPTR *)&pfUserMgrObjectSetGroupRights, (fl), 0, 0)
	#define CAL_UserMgrObjectSetGroupRights  pfUserMgrObjectSetGroupRights
	#define CHK_UserMgrObjectSetGroupRights  (pfUserMgrObjectSetGroupRights != NULL)
	#define EXP_UserMgrObjectSetGroupRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetGroupRights", (RTS_UINTPTR)UserMgrObjectSetGroupRights, 0, 0) 
#endif




/**
 * <description>Sets the denied access rights for the group at the specified object</description>
 * <param name="hObject" type="IN">Handle to object</param>
 * <param name="hGroup" type="IN">Handle to the group</param>
 * <element name="ulDeniedRights" type="IN">Denied rights of the group. See category "User rights" for details</element>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Access rights could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectSetGroupDeniedRights(RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulDeniedRights);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTSETGROUPDENIEDRIGHTS) (RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulDeniedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTSETGROUPDENIEDRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrObjectSetGroupDeniedRights
	#define EXT_UserMgrObjectSetGroupDeniedRights
	#define GET_UserMgrObjectSetGroupDeniedRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectSetGroupDeniedRights(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectSetGroupDeniedRights  FALSE
	#define EXP_UserMgrObjectSetGroupDeniedRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectSetGroupDeniedRights
	#define EXT_UserMgrObjectSetGroupDeniedRights
	#define GET_UserMgrObjectSetGroupDeniedRights(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupDeniedRights" ) 
	#define CAL_UserMgrObjectSetGroupDeniedRights  UserMgrObjectSetGroupDeniedRights
	#define CHK_UserMgrObjectSetGroupDeniedRights  TRUE
	#define EXP_UserMgrObjectSetGroupDeniedRights  CAL_CMEXPAPI( "UserMgrObjectSetGroupDeniedRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectSetGroupDeniedRights
	#define EXT_UserMgrObjectSetGroupDeniedRights
	#define GET_UserMgrObjectSetGroupDeniedRights(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupDeniedRights" ) 
	#define CAL_UserMgrObjectSetGroupDeniedRights  UserMgrObjectSetGroupDeniedRights
	#define CHK_UserMgrObjectSetGroupDeniedRights  TRUE
	#define EXP_UserMgrObjectSetGroupDeniedRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetGroupDeniedRights", (RTS_UINTPTR)UserMgrObjectSetGroupDeniedRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectSetGroupDeniedRights
	#define EXT_CmpUserMgrUserMgrObjectSetGroupDeniedRights
	#define GET_CmpUserMgrUserMgrObjectSetGroupDeniedRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectSetGroupDeniedRights pICmpUserMgr->IUserMgrObjectSetGroupDeniedRights
	#define CHK_CmpUserMgrUserMgrObjectSetGroupDeniedRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectSetGroupDeniedRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectSetGroupDeniedRights
	#define EXT_UserMgrObjectSetGroupDeniedRights
	#define GET_UserMgrObjectSetGroupDeniedRights(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupDeniedRights" ) 
	#define CAL_UserMgrObjectSetGroupDeniedRights pICmpUserMgr->IUserMgrObjectSetGroupDeniedRights
	#define CHK_UserMgrObjectSetGroupDeniedRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectSetGroupDeniedRights  CAL_CMEXPAPI( "UserMgrObjectSetGroupDeniedRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectSetGroupDeniedRights  PFUSERMGROBJECTSETGROUPDENIEDRIGHTS pfUserMgrObjectSetGroupDeniedRights;
	#define EXT_UserMgrObjectSetGroupDeniedRights  extern PFUSERMGROBJECTSETGROUPDENIEDRIGHTS pfUserMgrObjectSetGroupDeniedRights;
	#define GET_UserMgrObjectSetGroupDeniedRights(fl)  s_pfCMGetAPI2( "UserMgrObjectSetGroupDeniedRights", (RTS_VOID_FCTPTR *)&pfUserMgrObjectSetGroupDeniedRights, (fl), 0, 0)
	#define CAL_UserMgrObjectSetGroupDeniedRights  pfUserMgrObjectSetGroupDeniedRights
	#define CHK_UserMgrObjectSetGroupDeniedRights  (pfUserMgrObjectSetGroupDeniedRights != NULL)
	#define EXP_UserMgrObjectSetGroupDeniedRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetGroupDeniedRights", (RTS_UINTPTR)UserMgrObjectSetGroupDeniedRights, 0, 0) 
#endif




/**
 * <description>This function clears all granted and denied rights for all groups on that object.</description>
 * <param name="hObject" type="IN">Handle to object</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Access rights could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectClearRights(RTS_HANDLE hObject);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTCLEARRIGHTS) (RTS_HANDLE hObject);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTCLEARRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrObjectClearRights
	#define EXT_UserMgrObjectClearRights
	#define GET_UserMgrObjectClearRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectClearRights(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectClearRights  FALSE
	#define EXP_UserMgrObjectClearRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectClearRights
	#define EXT_UserMgrObjectClearRights
	#define GET_UserMgrObjectClearRights(fl)  CAL_CMGETAPI( "UserMgrObjectClearRights" ) 
	#define CAL_UserMgrObjectClearRights  UserMgrObjectClearRights
	#define CHK_UserMgrObjectClearRights  TRUE
	#define EXP_UserMgrObjectClearRights  CAL_CMEXPAPI( "UserMgrObjectClearRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectClearRights
	#define EXT_UserMgrObjectClearRights
	#define GET_UserMgrObjectClearRights(fl)  CAL_CMGETAPI( "UserMgrObjectClearRights" ) 
	#define CAL_UserMgrObjectClearRights  UserMgrObjectClearRights
	#define CHK_UserMgrObjectClearRights  TRUE
	#define EXP_UserMgrObjectClearRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectClearRights", (RTS_UINTPTR)UserMgrObjectClearRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectClearRights
	#define EXT_CmpUserMgrUserMgrObjectClearRights
	#define GET_CmpUserMgrUserMgrObjectClearRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectClearRights pICmpUserMgr->IUserMgrObjectClearRights
	#define CHK_CmpUserMgrUserMgrObjectClearRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectClearRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectClearRights
	#define EXT_UserMgrObjectClearRights
	#define GET_UserMgrObjectClearRights(fl)  CAL_CMGETAPI( "UserMgrObjectClearRights" ) 
	#define CAL_UserMgrObjectClearRights pICmpUserMgr->IUserMgrObjectClearRights
	#define CHK_UserMgrObjectClearRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectClearRights  CAL_CMEXPAPI( "UserMgrObjectClearRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectClearRights  PFUSERMGROBJECTCLEARRIGHTS pfUserMgrObjectClearRights;
	#define EXT_UserMgrObjectClearRights  extern PFUSERMGROBJECTCLEARRIGHTS pfUserMgrObjectClearRights;
	#define GET_UserMgrObjectClearRights(fl)  s_pfCMGetAPI2( "UserMgrObjectClearRights", (RTS_VOID_FCTPTR *)&pfUserMgrObjectClearRights, (fl), 0, 0)
	#define CAL_UserMgrObjectClearRights  pfUserMgrObjectClearRights
	#define CHK_UserMgrObjectClearRights  (pfUserMgrObjectClearRights != NULL)
	#define EXP_UserMgrObjectClearRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectClearRights", (RTS_UINTPTR)UserMgrObjectClearRights, 0, 0) 
#endif




/**
 * <description>Gets object name</description>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="pszObject" type="OUT">Pointer to get object name</param>
 * <param name="piMaxLen" type="INOUT">Pointer to length of buffer in pszObject. If pszObject = NULL, only the length of the object name is returned in *piMaxLen (including NUL character)</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Object name is returned</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid object handle or pszObject = NULL and piMaxLen = NULL</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectGetName(RTS_HANDLE hObject, char *pszObject, int *piMaxLen);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTGETNAME) (RTS_HANDLE hObject, char *pszObject, int *piMaxLen);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETNAME_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetName
	#define EXT_UserMgrObjectGetName
	#define GET_UserMgrObjectGetName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectGetName  FALSE
	#define EXP_UserMgrObjectGetName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetName
	#define EXT_UserMgrObjectGetName
	#define GET_UserMgrObjectGetName(fl)  CAL_CMGETAPI( "UserMgrObjectGetName" ) 
	#define CAL_UserMgrObjectGetName  UserMgrObjectGetName
	#define CHK_UserMgrObjectGetName  TRUE
	#define EXP_UserMgrObjectGetName  CAL_CMEXPAPI( "UserMgrObjectGetName" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetName
	#define EXT_UserMgrObjectGetName
	#define GET_UserMgrObjectGetName(fl)  CAL_CMGETAPI( "UserMgrObjectGetName" ) 
	#define CAL_UserMgrObjectGetName  UserMgrObjectGetName
	#define CHK_UserMgrObjectGetName  TRUE
	#define EXP_UserMgrObjectGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetName", (RTS_UINTPTR)UserMgrObjectGetName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetName
	#define EXT_CmpUserMgrUserMgrObjectGetName
	#define GET_CmpUserMgrUserMgrObjectGetName  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetName pICmpUserMgr->IUserMgrObjectGetName
	#define CHK_CmpUserMgrUserMgrObjectGetName (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetName
	#define EXT_UserMgrObjectGetName
	#define GET_UserMgrObjectGetName(fl)  CAL_CMGETAPI( "UserMgrObjectGetName" ) 
	#define CAL_UserMgrObjectGetName pICmpUserMgr->IUserMgrObjectGetName
	#define CHK_UserMgrObjectGetName (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetName  CAL_CMEXPAPI( "UserMgrObjectGetName" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetName  PFUSERMGROBJECTGETNAME pfUserMgrObjectGetName;
	#define EXT_UserMgrObjectGetName  extern PFUSERMGROBJECTGETNAME pfUserMgrObjectGetName;
	#define GET_UserMgrObjectGetName(fl)  s_pfCMGetAPI2( "UserMgrObjectGetName", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetName, (fl), 0, 0)
	#define CAL_UserMgrObjectGetName  pfUserMgrObjectGetName
	#define CHK_UserMgrObjectGetName  (pfUserMgrObjectGetName != NULL)
	#define EXP_UserMgrObjectGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetName", (RTS_UINTPTR)UserMgrObjectGetName, 0, 0) 
#endif




/**
 * <description>Gets first child object in object tree of the specified father object</description>
 * <param name="hFatherObject" type="IN">Handle to the father object</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First child object available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter hFatherObject</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <result>Handle to the child object or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserMgrObjectGetFirstChild(RTS_HANDLE hFatherObject, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTGETFIRSTCHILD) (RTS_HANDLE hFatherObject, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETFIRSTCHILD_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetFirstChild
	#define EXT_UserMgrObjectGetFirstChild
	#define GET_UserMgrObjectGetFirstChild(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetFirstChild(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectGetFirstChild  FALSE
	#define EXP_UserMgrObjectGetFirstChild  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetFirstChild
	#define EXT_UserMgrObjectGetFirstChild
	#define GET_UserMgrObjectGetFirstChild(fl)  CAL_CMGETAPI( "UserMgrObjectGetFirstChild" ) 
	#define CAL_UserMgrObjectGetFirstChild  UserMgrObjectGetFirstChild
	#define CHK_UserMgrObjectGetFirstChild  TRUE
	#define EXP_UserMgrObjectGetFirstChild  CAL_CMEXPAPI( "UserMgrObjectGetFirstChild" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetFirstChild
	#define EXT_UserMgrObjectGetFirstChild
	#define GET_UserMgrObjectGetFirstChild(fl)  CAL_CMGETAPI( "UserMgrObjectGetFirstChild" ) 
	#define CAL_UserMgrObjectGetFirstChild  UserMgrObjectGetFirstChild
	#define CHK_UserMgrObjectGetFirstChild  TRUE
	#define EXP_UserMgrObjectGetFirstChild  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetFirstChild", (RTS_UINTPTR)UserMgrObjectGetFirstChild, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetFirstChild
	#define EXT_CmpUserMgrUserMgrObjectGetFirstChild
	#define GET_CmpUserMgrUserMgrObjectGetFirstChild  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetFirstChild pICmpUserMgr->IUserMgrObjectGetFirstChild
	#define CHK_CmpUserMgrUserMgrObjectGetFirstChild (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetFirstChild  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetFirstChild
	#define EXT_UserMgrObjectGetFirstChild
	#define GET_UserMgrObjectGetFirstChild(fl)  CAL_CMGETAPI( "UserMgrObjectGetFirstChild" ) 
	#define CAL_UserMgrObjectGetFirstChild pICmpUserMgr->IUserMgrObjectGetFirstChild
	#define CHK_UserMgrObjectGetFirstChild (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetFirstChild  CAL_CMEXPAPI( "UserMgrObjectGetFirstChild" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetFirstChild  PFUSERMGROBJECTGETFIRSTCHILD pfUserMgrObjectGetFirstChild;
	#define EXT_UserMgrObjectGetFirstChild  extern PFUSERMGROBJECTGETFIRSTCHILD pfUserMgrObjectGetFirstChild;
	#define GET_UserMgrObjectGetFirstChild(fl)  s_pfCMGetAPI2( "UserMgrObjectGetFirstChild", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetFirstChild, (fl), 0, 0)
	#define CAL_UserMgrObjectGetFirstChild  pfUserMgrObjectGetFirstChild
	#define CHK_UserMgrObjectGetFirstChild  (pfUserMgrObjectGetFirstChild != NULL)
	#define EXP_UserMgrObjectGetFirstChild  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetFirstChild", (RTS_UINTPTR)UserMgrObjectGetFirstChild, 0, 0) 
#endif




/**
 * <description>Gets next child object in object tree of the specified father object. Must be started with UserDBObjectGetFirstChild()</description>
 * <param name="hFatherObject" type="IN">Handle to the father object</param>
 * <param name="hPrevChildObject" type="IN">Handle to the previous child object retrieved by UserDBObjectGetFirstChild() or subsequent calls of UserDBObjectGetNextChild()</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next child object available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter hFatherObject or hPrevChildObject</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">The user management is not initialized or it is not loaded yet</errorcode>
 * <result>Handle to the next child object or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserMgrObjectGetNextChild(RTS_HANDLE hFatherObject, RTS_HANDLE hPrevChildObject, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTGETNEXTCHILD) (RTS_HANDLE hFatherObject, RTS_HANDLE hPrevChildObject, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETNEXTCHILD_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetNextChild
	#define EXT_UserMgrObjectGetNextChild
	#define GET_UserMgrObjectGetNextChild(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetNextChild(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectGetNextChild  FALSE
	#define EXP_UserMgrObjectGetNextChild  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetNextChild
	#define EXT_UserMgrObjectGetNextChild
	#define GET_UserMgrObjectGetNextChild(fl)  CAL_CMGETAPI( "UserMgrObjectGetNextChild" ) 
	#define CAL_UserMgrObjectGetNextChild  UserMgrObjectGetNextChild
	#define CHK_UserMgrObjectGetNextChild  TRUE
	#define EXP_UserMgrObjectGetNextChild  CAL_CMEXPAPI( "UserMgrObjectGetNextChild" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetNextChild
	#define EXT_UserMgrObjectGetNextChild
	#define GET_UserMgrObjectGetNextChild(fl)  CAL_CMGETAPI( "UserMgrObjectGetNextChild" ) 
	#define CAL_UserMgrObjectGetNextChild  UserMgrObjectGetNextChild
	#define CHK_UserMgrObjectGetNextChild  TRUE
	#define EXP_UserMgrObjectGetNextChild  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetNextChild", (RTS_UINTPTR)UserMgrObjectGetNextChild, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetNextChild
	#define EXT_CmpUserMgrUserMgrObjectGetNextChild
	#define GET_CmpUserMgrUserMgrObjectGetNextChild  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetNextChild pICmpUserMgr->IUserMgrObjectGetNextChild
	#define CHK_CmpUserMgrUserMgrObjectGetNextChild (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetNextChild  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetNextChild
	#define EXT_UserMgrObjectGetNextChild
	#define GET_UserMgrObjectGetNextChild(fl)  CAL_CMGETAPI( "UserMgrObjectGetNextChild" ) 
	#define CAL_UserMgrObjectGetNextChild pICmpUserMgr->IUserMgrObjectGetNextChild
	#define CHK_UserMgrObjectGetNextChild (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetNextChild  CAL_CMEXPAPI( "UserMgrObjectGetNextChild" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetNextChild  PFUSERMGROBJECTGETNEXTCHILD pfUserMgrObjectGetNextChild;
	#define EXT_UserMgrObjectGetNextChild  extern PFUSERMGROBJECTGETNEXTCHILD pfUserMgrObjectGetNextChild;
	#define GET_UserMgrObjectGetNextChild(fl)  s_pfCMGetAPI2( "UserMgrObjectGetNextChild", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetNextChild, (fl), 0, 0)
	#define CAL_UserMgrObjectGetNextChild  pfUserMgrObjectGetNextChild
	#define CHK_UserMgrObjectGetNextChild  (pfUserMgrObjectGetNextChild != NULL)
	#define EXP_UserMgrObjectGetNextChild  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetNextChild", (RTS_UINTPTR)UserMgrObjectGetNextChild, 0, 0) 
#endif




/**
 * <description>Iteration interface to get the first group in the user rights management of the specified object</description>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No group available</errorcode>
 * <result>Handle to the first group of the object in the user rights management or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserMgrObjectGetFirstGroup(RTS_HANDLE hObject, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTGETFIRSTGROUP) (RTS_HANDLE hObject, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETFIRSTGROUP_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetFirstGroup
	#define EXT_UserMgrObjectGetFirstGroup
	#define GET_UserMgrObjectGetFirstGroup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetFirstGroup(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectGetFirstGroup  FALSE
	#define EXP_UserMgrObjectGetFirstGroup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetFirstGroup
	#define EXT_UserMgrObjectGetFirstGroup
	#define GET_UserMgrObjectGetFirstGroup(fl)  CAL_CMGETAPI( "UserMgrObjectGetFirstGroup" ) 
	#define CAL_UserMgrObjectGetFirstGroup  UserMgrObjectGetFirstGroup
	#define CHK_UserMgrObjectGetFirstGroup  TRUE
	#define EXP_UserMgrObjectGetFirstGroup  CAL_CMEXPAPI( "UserMgrObjectGetFirstGroup" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetFirstGroup
	#define EXT_UserMgrObjectGetFirstGroup
	#define GET_UserMgrObjectGetFirstGroup(fl)  CAL_CMGETAPI( "UserMgrObjectGetFirstGroup" ) 
	#define CAL_UserMgrObjectGetFirstGroup  UserMgrObjectGetFirstGroup
	#define CHK_UserMgrObjectGetFirstGroup  TRUE
	#define EXP_UserMgrObjectGetFirstGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetFirstGroup", (RTS_UINTPTR)UserMgrObjectGetFirstGroup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetFirstGroup
	#define EXT_CmpUserMgrUserMgrObjectGetFirstGroup
	#define GET_CmpUserMgrUserMgrObjectGetFirstGroup  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetFirstGroup pICmpUserMgr->IUserMgrObjectGetFirstGroup
	#define CHK_CmpUserMgrUserMgrObjectGetFirstGroup (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetFirstGroup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetFirstGroup
	#define EXT_UserMgrObjectGetFirstGroup
	#define GET_UserMgrObjectGetFirstGroup(fl)  CAL_CMGETAPI( "UserMgrObjectGetFirstGroup" ) 
	#define CAL_UserMgrObjectGetFirstGroup pICmpUserMgr->IUserMgrObjectGetFirstGroup
	#define CHK_UserMgrObjectGetFirstGroup (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetFirstGroup  CAL_CMEXPAPI( "UserMgrObjectGetFirstGroup" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetFirstGroup  PFUSERMGROBJECTGETFIRSTGROUP pfUserMgrObjectGetFirstGroup;
	#define EXT_UserMgrObjectGetFirstGroup  extern PFUSERMGROBJECTGETFIRSTGROUP pfUserMgrObjectGetFirstGroup;
	#define GET_UserMgrObjectGetFirstGroup(fl)  s_pfCMGetAPI2( "UserMgrObjectGetFirstGroup", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetFirstGroup, (fl), 0, 0)
	#define CAL_UserMgrObjectGetFirstGroup  pfUserMgrObjectGetFirstGroup
	#define CHK_UserMgrObjectGetFirstGroup  (pfUserMgrObjectGetFirstGroup != NULL)
	#define EXP_UserMgrObjectGetFirstGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetFirstGroup", (RTS_UINTPTR)UserMgrObjectGetFirstGroup, 0, 0) 
#endif




/**
 * <description>Iteration interface to get next group in the user rights management of the specified object. Must be started with UserDBObjectGetFirstGroup()</description>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="hPrevGroup" type="IN">Handle to the previous group retrieved via UserDBObjectGetFirstGroup() or subsequent calls of UserDBObjectGetNextGroup()</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next group available</errorcode>
 * <result>Handle to the next group of the object in the user rights management or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserMgrObjectGetNextGroup(RTS_HANDLE hObject, RTS_HANDLE hPrevGroup, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTGETNEXTGROUP) (RTS_HANDLE hObject, RTS_HANDLE hPrevGroup, RTS_RESULT *pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETNEXTGROUP_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetNextGroup
	#define EXT_UserMgrObjectGetNextGroup
	#define GET_UserMgrObjectGetNextGroup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetNextGroup(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectGetNextGroup  FALSE
	#define EXP_UserMgrObjectGetNextGroup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetNextGroup
	#define EXT_UserMgrObjectGetNextGroup
	#define GET_UserMgrObjectGetNextGroup(fl)  CAL_CMGETAPI( "UserMgrObjectGetNextGroup" ) 
	#define CAL_UserMgrObjectGetNextGroup  UserMgrObjectGetNextGroup
	#define CHK_UserMgrObjectGetNextGroup  TRUE
	#define EXP_UserMgrObjectGetNextGroup  CAL_CMEXPAPI( "UserMgrObjectGetNextGroup" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetNextGroup
	#define EXT_UserMgrObjectGetNextGroup
	#define GET_UserMgrObjectGetNextGroup(fl)  CAL_CMGETAPI( "UserMgrObjectGetNextGroup" ) 
	#define CAL_UserMgrObjectGetNextGroup  UserMgrObjectGetNextGroup
	#define CHK_UserMgrObjectGetNextGroup  TRUE
	#define EXP_UserMgrObjectGetNextGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetNextGroup", (RTS_UINTPTR)UserMgrObjectGetNextGroup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetNextGroup
	#define EXT_CmpUserMgrUserMgrObjectGetNextGroup
	#define GET_CmpUserMgrUserMgrObjectGetNextGroup  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetNextGroup pICmpUserMgr->IUserMgrObjectGetNextGroup
	#define CHK_CmpUserMgrUserMgrObjectGetNextGroup (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetNextGroup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetNextGroup
	#define EXT_UserMgrObjectGetNextGroup
	#define GET_UserMgrObjectGetNextGroup(fl)  CAL_CMGETAPI( "UserMgrObjectGetNextGroup" ) 
	#define CAL_UserMgrObjectGetNextGroup pICmpUserMgr->IUserMgrObjectGetNextGroup
	#define CHK_UserMgrObjectGetNextGroup (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetNextGroup  CAL_CMEXPAPI( "UserMgrObjectGetNextGroup" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetNextGroup  PFUSERMGROBJECTGETNEXTGROUP pfUserMgrObjectGetNextGroup;
	#define EXT_UserMgrObjectGetNextGroup  extern PFUSERMGROBJECTGETNEXTGROUP pfUserMgrObjectGetNextGroup;
	#define GET_UserMgrObjectGetNextGroup(fl)  s_pfCMGetAPI2( "UserMgrObjectGetNextGroup", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetNextGroup, (fl), 0, 0)
	#define CAL_UserMgrObjectGetNextGroup  pfUserMgrObjectGetNextGroup
	#define CHK_UserMgrObjectGetNextGroup  (pfUserMgrObjectGetNextGroup != NULL)
	#define EXP_UserMgrObjectGetNextGroup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetNextGroup", (RTS_UINTPTR)UserMgrObjectGetNextGroup, 0, 0) 
#endif




/**
 * <description>Gets the rights and the denied rights of a group on a specified object</description>
 * <param name="hObject" type="IN">Handle to object</param>
 * <param name="hGroup" type="IN">Handle to the group which access rights are requested</param>
 * <element name="pulRights" type="IN">Pointer to get rights of the group. See category "User rights" for details</element>
 * <element name="pulDeniedRights" type="IN">Pointer to get denied rights of the group. See category "User rights" for details</element>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Access rights could be retrieved</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectGetGroupRights(RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTGETGROUPRIGHTS) (RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETGROUPRIGHTS_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetGroupRights
	#define EXT_UserMgrObjectGetGroupRights
	#define GET_UserMgrObjectGetGroupRights(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetGroupRights(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectGetGroupRights  FALSE
	#define EXP_UserMgrObjectGetGroupRights  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetGroupRights
	#define EXT_UserMgrObjectGetGroupRights
	#define GET_UserMgrObjectGetGroupRights(fl)  CAL_CMGETAPI( "UserMgrObjectGetGroupRights" ) 
	#define CAL_UserMgrObjectGetGroupRights  UserMgrObjectGetGroupRights
	#define CHK_UserMgrObjectGetGroupRights  TRUE
	#define EXP_UserMgrObjectGetGroupRights  CAL_CMEXPAPI( "UserMgrObjectGetGroupRights" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetGroupRights
	#define EXT_UserMgrObjectGetGroupRights
	#define GET_UserMgrObjectGetGroupRights(fl)  CAL_CMGETAPI( "UserMgrObjectGetGroupRights" ) 
	#define CAL_UserMgrObjectGetGroupRights  UserMgrObjectGetGroupRights
	#define CHK_UserMgrObjectGetGroupRights  TRUE
	#define EXP_UserMgrObjectGetGroupRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetGroupRights", (RTS_UINTPTR)UserMgrObjectGetGroupRights, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetGroupRights
	#define EXT_CmpUserMgrUserMgrObjectGetGroupRights
	#define GET_CmpUserMgrUserMgrObjectGetGroupRights  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetGroupRights pICmpUserMgr->IUserMgrObjectGetGroupRights
	#define CHK_CmpUserMgrUserMgrObjectGetGroupRights (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetGroupRights  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetGroupRights
	#define EXT_UserMgrObjectGetGroupRights
	#define GET_UserMgrObjectGetGroupRights(fl)  CAL_CMGETAPI( "UserMgrObjectGetGroupRights" ) 
	#define CAL_UserMgrObjectGetGroupRights pICmpUserMgr->IUserMgrObjectGetGroupRights
	#define CHK_UserMgrObjectGetGroupRights (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetGroupRights  CAL_CMEXPAPI( "UserMgrObjectGetGroupRights" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetGroupRights  PFUSERMGROBJECTGETGROUPRIGHTS pfUserMgrObjectGetGroupRights;
	#define EXT_UserMgrObjectGetGroupRights  extern PFUSERMGROBJECTGETGROUPRIGHTS pfUserMgrObjectGetGroupRights;
	#define GET_UserMgrObjectGetGroupRights(fl)  s_pfCMGetAPI2( "UserMgrObjectGetGroupRights", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetGroupRights, (fl), 0, 0)
	#define CAL_UserMgrObjectGetGroupRights  pfUserMgrObjectGetGroupRights
	#define CHK_UserMgrObjectGetGroupRights  (pfUserMgrObjectGetGroupRights != NULL)
	#define EXP_UserMgrObjectGetGroupRights  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetGroupRights", (RTS_UINTPTR)UserMgrObjectGetGroupRights, 0, 0) 
#endif




/**
 * <description>Storing the current user rights management into permanent database file</description>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Error storing user rights database</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">Is not implemented perhaps the UserRights is fix</errorcode>
 */
RTS_RESULT CDECL UserMgrRightsStore(void);
typedef RTS_RESULT (CDECL * PFUSERMGRRIGHTSSTORE) (void);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRRIGHTSSTORE_NOTIMPLEMENTED)
	#define USE_UserMgrRightsStore
	#define EXT_UserMgrRightsStore
	#define GET_UserMgrRightsStore(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrRightsStore()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrRightsStore  FALSE
	#define EXP_UserMgrRightsStore  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrRightsStore
	#define EXT_UserMgrRightsStore
	#define GET_UserMgrRightsStore(fl)  CAL_CMGETAPI( "UserMgrRightsStore" ) 
	#define CAL_UserMgrRightsStore  UserMgrRightsStore
	#define CHK_UserMgrRightsStore  TRUE
	#define EXP_UserMgrRightsStore  CAL_CMEXPAPI( "UserMgrRightsStore" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrRightsStore
	#define EXT_UserMgrRightsStore
	#define GET_UserMgrRightsStore(fl)  CAL_CMGETAPI( "UserMgrRightsStore" ) 
	#define CAL_UserMgrRightsStore  UserMgrRightsStore
	#define CHK_UserMgrRightsStore  TRUE
	#define EXP_UserMgrRightsStore  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRightsStore", (RTS_UINTPTR)UserMgrRightsStore, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrRightsStore
	#define EXT_CmpUserMgrUserMgrRightsStore
	#define GET_CmpUserMgrUserMgrRightsStore  ERR_OK
	#define CAL_CmpUserMgrUserMgrRightsStore pICmpUserMgr->IUserMgrRightsStore
	#define CHK_CmpUserMgrUserMgrRightsStore (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrRightsStore  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrRightsStore
	#define EXT_UserMgrRightsStore
	#define GET_UserMgrRightsStore(fl)  CAL_CMGETAPI( "UserMgrRightsStore" ) 
	#define CAL_UserMgrRightsStore pICmpUserMgr->IUserMgrRightsStore
	#define CHK_UserMgrRightsStore (pICmpUserMgr != NULL)
	#define EXP_UserMgrRightsStore  CAL_CMEXPAPI( "UserMgrRightsStore" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrRightsStore  PFUSERMGRRIGHTSSTORE pfUserMgrRightsStore;
	#define EXT_UserMgrRightsStore  extern PFUSERMGRRIGHTSSTORE pfUserMgrRightsStore;
	#define GET_UserMgrRightsStore(fl)  s_pfCMGetAPI2( "UserMgrRightsStore", (RTS_VOID_FCTPTR *)&pfUserMgrRightsStore, (fl), 0, 0)
	#define CAL_UserMgrRightsStore  pfUserMgrRightsStore
	#define CHK_UserMgrRightsStore  (pfUserMgrRightsStore != NULL)
	#define EXP_UserMgrRightsStore  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrRightsStore", (RTS_UINTPTR)UserMgrRightsStore, 0, 0) 
#endif




/**
 * <description>
 * Checks if the given string fits in a buffer determined by the given length including the terminating NUL
 * Returns TRUE if the string was shortened to handle this case
 * </description>
 * <param name="psz" type="IN">The string to check</param>
 * <param name="len" type="IN">Maximum buffer length</param>
 * <result>TRUE means the string was shortened</result>
 */
RTS_BOOL CDECL UserMgrCheckLen(char *psz, RTS_UI32 len);
typedef RTS_BOOL (CDECL * PFUSERMGRCHECKLEN) (char *psz, RTS_UI32 len);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRCHECKLEN_NOTIMPLEMENTED)
	#define USE_UserMgrCheckLen
	#define EXT_UserMgrCheckLen
	#define GET_UserMgrCheckLen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrCheckLen(p0,p1)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrCheckLen  FALSE
	#define EXP_UserMgrCheckLen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrCheckLen
	#define EXT_UserMgrCheckLen
	#define GET_UserMgrCheckLen(fl)  CAL_CMGETAPI( "UserMgrCheckLen" ) 
	#define CAL_UserMgrCheckLen  UserMgrCheckLen
	#define CHK_UserMgrCheckLen  TRUE
	#define EXP_UserMgrCheckLen  CAL_CMEXPAPI( "UserMgrCheckLen" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrCheckLen
	#define EXT_UserMgrCheckLen
	#define GET_UserMgrCheckLen(fl)  CAL_CMGETAPI( "UserMgrCheckLen" ) 
	#define CAL_UserMgrCheckLen  UserMgrCheckLen
	#define CHK_UserMgrCheckLen  TRUE
	#define EXP_UserMgrCheckLen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrCheckLen", (RTS_UINTPTR)UserMgrCheckLen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrCheckLen
	#define EXT_CmpUserMgrUserMgrCheckLen
	#define GET_CmpUserMgrUserMgrCheckLen  ERR_OK
	#define CAL_CmpUserMgrUserMgrCheckLen pICmpUserMgr->IUserMgrCheckLen
	#define CHK_CmpUserMgrUserMgrCheckLen (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrCheckLen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrCheckLen
	#define EXT_UserMgrCheckLen
	#define GET_UserMgrCheckLen(fl)  CAL_CMGETAPI( "UserMgrCheckLen" ) 
	#define CAL_UserMgrCheckLen pICmpUserMgr->IUserMgrCheckLen
	#define CHK_UserMgrCheckLen (pICmpUserMgr != NULL)
	#define EXP_UserMgrCheckLen  CAL_CMEXPAPI( "UserMgrCheckLen" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrCheckLen  PFUSERMGRCHECKLEN pfUserMgrCheckLen;
	#define EXT_UserMgrCheckLen  extern PFUSERMGRCHECKLEN pfUserMgrCheckLen;
	#define GET_UserMgrCheckLen(fl)  s_pfCMGetAPI2( "UserMgrCheckLen", (RTS_VOID_FCTPTR *)&pfUserMgrCheckLen, (fl), 0, 0)
	#define CAL_UserMgrCheckLen  pfUserMgrCheckLen
	#define CHK_UserMgrCheckLen  (pfUserMgrCheckLen != NULL)
	#define EXP_UserMgrCheckLen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrCheckLen", (RTS_UINTPTR)UserMgrCheckLen, 0, 0) 
#endif




/**
 * <description>
 * Tells the UserManager that there is an old format database found:
 * The announcement ANNOUNCE may return an error other than ERR_OK (ERR_OPERATION_DENIED is recommended) which indicates that the conversion of the old database is denied.
 * The announcement FINISH tells the UserManager that an old database is converted and adjustment may be processed now.
 * </description>
 * <param name="info" type="IN">ANNOUNCE, FINISH, see Old database announcements</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Succeeded</errorcode>
 */
RTS_RESULT CDECL UserMgrAnnounceOldDatabase(RTS_UI32 info);
typedef RTS_RESULT (CDECL * PFUSERMGRANNOUNCEOLDDATABASE) (RTS_UI32 info);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRANNOUNCEOLDDATABASE_NOTIMPLEMENTED)
	#define USE_UserMgrAnnounceOldDatabase
	#define EXT_UserMgrAnnounceOldDatabase
	#define GET_UserMgrAnnounceOldDatabase(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrAnnounceOldDatabase(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrAnnounceOldDatabase  FALSE
	#define EXP_UserMgrAnnounceOldDatabase  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrAnnounceOldDatabase
	#define EXT_UserMgrAnnounceOldDatabase
	#define GET_UserMgrAnnounceOldDatabase(fl)  CAL_CMGETAPI( "UserMgrAnnounceOldDatabase" ) 
	#define CAL_UserMgrAnnounceOldDatabase  UserMgrAnnounceOldDatabase
	#define CHK_UserMgrAnnounceOldDatabase  TRUE
	#define EXP_UserMgrAnnounceOldDatabase  CAL_CMEXPAPI( "UserMgrAnnounceOldDatabase" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrAnnounceOldDatabase
	#define EXT_UserMgrAnnounceOldDatabase
	#define GET_UserMgrAnnounceOldDatabase(fl)  CAL_CMGETAPI( "UserMgrAnnounceOldDatabase" ) 
	#define CAL_UserMgrAnnounceOldDatabase  UserMgrAnnounceOldDatabase
	#define CHK_UserMgrAnnounceOldDatabase  TRUE
	#define EXP_UserMgrAnnounceOldDatabase  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAnnounceOldDatabase", (RTS_UINTPTR)UserMgrAnnounceOldDatabase, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrAnnounceOldDatabase
	#define EXT_CmpUserMgrUserMgrAnnounceOldDatabase
	#define GET_CmpUserMgrUserMgrAnnounceOldDatabase  ERR_OK
	#define CAL_CmpUserMgrUserMgrAnnounceOldDatabase pICmpUserMgr->IUserMgrAnnounceOldDatabase
	#define CHK_CmpUserMgrUserMgrAnnounceOldDatabase (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrAnnounceOldDatabase  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrAnnounceOldDatabase
	#define EXT_UserMgrAnnounceOldDatabase
	#define GET_UserMgrAnnounceOldDatabase(fl)  CAL_CMGETAPI( "UserMgrAnnounceOldDatabase" ) 
	#define CAL_UserMgrAnnounceOldDatabase pICmpUserMgr->IUserMgrAnnounceOldDatabase
	#define CHK_UserMgrAnnounceOldDatabase (pICmpUserMgr != NULL)
	#define EXP_UserMgrAnnounceOldDatabase  CAL_CMEXPAPI( "UserMgrAnnounceOldDatabase" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrAnnounceOldDatabase  PFUSERMGRANNOUNCEOLDDATABASE pfUserMgrAnnounceOldDatabase;
	#define EXT_UserMgrAnnounceOldDatabase  extern PFUSERMGRANNOUNCEOLDDATABASE pfUserMgrAnnounceOldDatabase;
	#define GET_UserMgrAnnounceOldDatabase(fl)  s_pfCMGetAPI2( "UserMgrAnnounceOldDatabase", (RTS_VOID_FCTPTR *)&pfUserMgrAnnounceOldDatabase, (fl), 0, 0)
	#define CAL_UserMgrAnnounceOldDatabase  pfUserMgrAnnounceOldDatabase
	#define CHK_UserMgrAnnounceOldDatabase  (pfUserMgrAnnounceOldDatabase != NULL)
	#define EXP_UserMgrAnnounceOldDatabase  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrAnnounceOldDatabase", (RTS_UINTPTR)UserMgrAnnounceOldDatabase, 0, 0) 
#endif





/**
 * <description>
 * Iteration interface to get the first user group
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="pResult" type="IN">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First user group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No user group configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 * <result>Handle to the first user group or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserMgrGroupGetFirst(RTS_HANDLE hUser, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGRGROUPGETFIRST) (RTS_HANDLE hUser, RTS_RESULT* pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPGETFIRST_NOTIMPLEMENTED)
	#define USE_UserMgrGroupGetFirst
	#define EXT_UserMgrGroupGetFirst
	#define GET_UserMgrGroupGetFirst(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupGetFirst(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrGroupGetFirst  FALSE
	#define EXP_UserMgrGroupGetFirst  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupGetFirst
	#define EXT_UserMgrGroupGetFirst
	#define GET_UserMgrGroupGetFirst(fl)  CAL_CMGETAPI( "UserMgrGroupGetFirst" ) 
	#define CAL_UserMgrGroupGetFirst  UserMgrGroupGetFirst
	#define CHK_UserMgrGroupGetFirst  TRUE
	#define EXP_UserMgrGroupGetFirst  CAL_CMEXPAPI( "UserMgrGroupGetFirst" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupGetFirst
	#define EXT_UserMgrGroupGetFirst
	#define GET_UserMgrGroupGetFirst(fl)  CAL_CMGETAPI( "UserMgrGroupGetFirst" ) 
	#define CAL_UserMgrGroupGetFirst  UserMgrGroupGetFirst
	#define CHK_UserMgrGroupGetFirst  TRUE
	#define EXP_UserMgrGroupGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetFirst", (RTS_UINTPTR)UserMgrGroupGetFirst, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupGetFirst
	#define EXT_CmpUserMgrUserMgrGroupGetFirst
	#define GET_CmpUserMgrUserMgrGroupGetFirst  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupGetFirst pICmpUserMgr->IUserMgrGroupGetFirst
	#define CHK_CmpUserMgrUserMgrGroupGetFirst (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupGetFirst  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupGetFirst
	#define EXT_UserMgrGroupGetFirst
	#define GET_UserMgrGroupGetFirst(fl)  CAL_CMGETAPI( "UserMgrGroupGetFirst" ) 
	#define CAL_UserMgrGroupGetFirst pICmpUserMgr->IUserMgrGroupGetFirst
	#define CHK_UserMgrGroupGetFirst (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupGetFirst  CAL_CMEXPAPI( "UserMgrGroupGetFirst" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupGetFirst  PFUSERMGRGROUPGETFIRST pfUserMgrGroupGetFirst;
	#define EXT_UserMgrGroupGetFirst  extern PFUSERMGRGROUPGETFIRST pfUserMgrGroupGetFirst;
	#define GET_UserMgrGroupGetFirst(fl)  s_pfCMGetAPI2( "UserMgrGroupGetFirst", (RTS_VOID_FCTPTR *)&pfUserMgrGroupGetFirst, (fl), 0, 0)
	#define CAL_UserMgrGroupGetFirst  pfUserMgrGroupGetFirst
	#define CHK_UserMgrGroupGetFirst  (pfUserMgrGroupGetFirst != NULL)
	#define EXP_UserMgrGroupGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetFirst", (RTS_UINTPTR)UserMgrGroupGetFirst, 0, 0) 
#endif




/**
 * <description>
 * Iteration interface to get the first user of the user group
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hGroup" type="IN">Group handle</param>
 * <param name="phIterator" type="IN">Pointer to iteration handle to use with UserMgrGroupGetNextUser()</param>
 * <param name="pszUsername" type="OUT">Buffer to store the name, 59 chars max, plus NUL character</param>
 * <param name="iMaxLen" type="IN">Length of buffer</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First user in the group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No user configured in the user group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">The buffer is too small</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrGroupGetFirstUser(RTS_HANDLE hUser, RTS_HANDLE hGroup, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen);
typedef RTS_RESULT (CDECL * PFUSERMGRGROUPGETFIRSTUSER) (RTS_HANDLE hUser, RTS_HANDLE hGroup, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPGETFIRSTUSER_NOTIMPLEMENTED)
	#define USE_UserMgrGroupGetFirstUser
	#define EXT_UserMgrGroupGetFirstUser
	#define GET_UserMgrGroupGetFirstUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupGetFirstUser(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGroupGetFirstUser  FALSE
	#define EXP_UserMgrGroupGetFirstUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupGetFirstUser
	#define EXT_UserMgrGroupGetFirstUser
	#define GET_UserMgrGroupGetFirstUser(fl)  CAL_CMGETAPI( "UserMgrGroupGetFirstUser" ) 
	#define CAL_UserMgrGroupGetFirstUser  UserMgrGroupGetFirstUser
	#define CHK_UserMgrGroupGetFirstUser  TRUE
	#define EXP_UserMgrGroupGetFirstUser  CAL_CMEXPAPI( "UserMgrGroupGetFirstUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupGetFirstUser
	#define EXT_UserMgrGroupGetFirstUser
	#define GET_UserMgrGroupGetFirstUser(fl)  CAL_CMGETAPI( "UserMgrGroupGetFirstUser" ) 
	#define CAL_UserMgrGroupGetFirstUser  UserMgrGroupGetFirstUser
	#define CHK_UserMgrGroupGetFirstUser  TRUE
	#define EXP_UserMgrGroupGetFirstUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetFirstUser", (RTS_UINTPTR)UserMgrGroupGetFirstUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupGetFirstUser
	#define EXT_CmpUserMgrUserMgrGroupGetFirstUser
	#define GET_CmpUserMgrUserMgrGroupGetFirstUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupGetFirstUser pICmpUserMgr->IUserMgrGroupGetFirstUser
	#define CHK_CmpUserMgrUserMgrGroupGetFirstUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupGetFirstUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupGetFirstUser
	#define EXT_UserMgrGroupGetFirstUser
	#define GET_UserMgrGroupGetFirstUser(fl)  CAL_CMGETAPI( "UserMgrGroupGetFirstUser" ) 
	#define CAL_UserMgrGroupGetFirstUser pICmpUserMgr->IUserMgrGroupGetFirstUser
	#define CHK_UserMgrGroupGetFirstUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupGetFirstUser  CAL_CMEXPAPI( "UserMgrGroupGetFirstUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupGetFirstUser  PFUSERMGRGROUPGETFIRSTUSER pfUserMgrGroupGetFirstUser;
	#define EXT_UserMgrGroupGetFirstUser  extern PFUSERMGRGROUPGETFIRSTUSER pfUserMgrGroupGetFirstUser;
	#define GET_UserMgrGroupGetFirstUser(fl)  s_pfCMGetAPI2( "UserMgrGroupGetFirstUser", (RTS_VOID_FCTPTR *)&pfUserMgrGroupGetFirstUser, (fl), 0, 0)
	#define CAL_UserMgrGroupGetFirstUser  pfUserMgrGroupGetFirstUser
	#define CHK_UserMgrGroupGetFirstUser  (pfUserMgrGroupGetFirstUser != NULL)
	#define EXP_UserMgrGroupGetFirstUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetFirstUser", (RTS_UINTPTR)UserMgrGroupGetFirstUser, 0, 0) 
#endif




/**
 * <description>
 * Retrieve the handle from the given group name
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="pszGroupName" type="IN">Name of the group</param>
 * <param name="pResult" type="IN">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group handle could be retrieved</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">Group was not found</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 * <result>Correspondig group handle or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserMgrGroupGetHandle(RTS_HANDLE hUser, char* pszGroupName, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGRGROUPGETHANDLE) (RTS_HANDLE hUser, char* pszGroupName, RTS_RESULT* pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPGETHANDLE_NOTIMPLEMENTED)
	#define USE_UserMgrGroupGetHandle
	#define EXT_UserMgrGroupGetHandle
	#define GET_UserMgrGroupGetHandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupGetHandle(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrGroupGetHandle  FALSE
	#define EXP_UserMgrGroupGetHandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupGetHandle
	#define EXT_UserMgrGroupGetHandle
	#define GET_UserMgrGroupGetHandle(fl)  CAL_CMGETAPI( "UserMgrGroupGetHandle" ) 
	#define CAL_UserMgrGroupGetHandle  UserMgrGroupGetHandle
	#define CHK_UserMgrGroupGetHandle  TRUE
	#define EXP_UserMgrGroupGetHandle  CAL_CMEXPAPI( "UserMgrGroupGetHandle" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupGetHandle
	#define EXT_UserMgrGroupGetHandle
	#define GET_UserMgrGroupGetHandle(fl)  CAL_CMGETAPI( "UserMgrGroupGetHandle" ) 
	#define CAL_UserMgrGroupGetHandle  UserMgrGroupGetHandle
	#define CHK_UserMgrGroupGetHandle  TRUE
	#define EXP_UserMgrGroupGetHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetHandle", (RTS_UINTPTR)UserMgrGroupGetHandle, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupGetHandle
	#define EXT_CmpUserMgrUserMgrGroupGetHandle
	#define GET_CmpUserMgrUserMgrGroupGetHandle  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupGetHandle pICmpUserMgr->IUserMgrGroupGetHandle
	#define CHK_CmpUserMgrUserMgrGroupGetHandle (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupGetHandle  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupGetHandle
	#define EXT_UserMgrGroupGetHandle
	#define GET_UserMgrGroupGetHandle(fl)  CAL_CMGETAPI( "UserMgrGroupGetHandle" ) 
	#define CAL_UserMgrGroupGetHandle pICmpUserMgr->IUserMgrGroupGetHandle
	#define CHK_UserMgrGroupGetHandle (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupGetHandle  CAL_CMEXPAPI( "UserMgrGroupGetHandle" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupGetHandle  PFUSERMGRGROUPGETHANDLE pfUserMgrGroupGetHandle;
	#define EXT_UserMgrGroupGetHandle  extern PFUSERMGRGROUPGETHANDLE pfUserMgrGroupGetHandle;
	#define GET_UserMgrGroupGetHandle(fl)  s_pfCMGetAPI2( "UserMgrGroupGetHandle", (RTS_VOID_FCTPTR *)&pfUserMgrGroupGetHandle, (fl), 0, 0)
	#define CAL_UserMgrGroupGetHandle  pfUserMgrGroupGetHandle
	#define CHK_UserMgrGroupGetHandle  (pfUserMgrGroupGetHandle != NULL)
	#define EXP_UserMgrGroupGetHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetHandle", (RTS_UINTPTR)UserMgrGroupGetHandle, 0, 0) 
#endif




/**
 * <description>
 * Get group name
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hGroup" type="IN">Group handle</param>
 * <param name="pszGroup" type="OUT">Buffer to store the name, 59 chars max, plus NUL character</param>
 * <param name="iMaxLen" type="IN">Length of buffer</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group name is returned</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid group handle</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">The buffer is too small</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrGroupGetName2(RTS_HANDLE hUser, RTS_HANDLE hGroup, char* pszGroup, RTS_UI32 iMaxLen);
typedef RTS_RESULT (CDECL * PFUSERMGRGROUPGETNAME2) (RTS_HANDLE hUser, RTS_HANDLE hGroup, char* pszGroup, RTS_UI32 iMaxLen);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPGETNAME2_NOTIMPLEMENTED)
	#define USE_UserMgrGroupGetName2
	#define EXT_UserMgrGroupGetName2
	#define GET_UserMgrGroupGetName2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupGetName2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGroupGetName2  FALSE
	#define EXP_UserMgrGroupGetName2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupGetName2
	#define EXT_UserMgrGroupGetName2
	#define GET_UserMgrGroupGetName2(fl)  CAL_CMGETAPI( "UserMgrGroupGetName2" ) 
	#define CAL_UserMgrGroupGetName2  UserMgrGroupGetName2
	#define CHK_UserMgrGroupGetName2  TRUE
	#define EXP_UserMgrGroupGetName2  CAL_CMEXPAPI( "UserMgrGroupGetName2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupGetName2
	#define EXT_UserMgrGroupGetName2
	#define GET_UserMgrGroupGetName2(fl)  CAL_CMGETAPI( "UserMgrGroupGetName2" ) 
	#define CAL_UserMgrGroupGetName2  UserMgrGroupGetName2
	#define CHK_UserMgrGroupGetName2  TRUE
	#define EXP_UserMgrGroupGetName2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetName2", (RTS_UINTPTR)UserMgrGroupGetName2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupGetName2
	#define EXT_CmpUserMgrUserMgrGroupGetName2
	#define GET_CmpUserMgrUserMgrGroupGetName2  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupGetName2 pICmpUserMgr->IUserMgrGroupGetName2
	#define CHK_CmpUserMgrUserMgrGroupGetName2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupGetName2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupGetName2
	#define EXT_UserMgrGroupGetName2
	#define GET_UserMgrGroupGetName2(fl)  CAL_CMGETAPI( "UserMgrGroupGetName2" ) 
	#define CAL_UserMgrGroupGetName2 pICmpUserMgr->IUserMgrGroupGetName2
	#define CHK_UserMgrGroupGetName2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupGetName2  CAL_CMEXPAPI( "UserMgrGroupGetName2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupGetName2  PFUSERMGRGROUPGETNAME2 pfUserMgrGroupGetName2;
	#define EXT_UserMgrGroupGetName2  extern PFUSERMGRGROUPGETNAME2 pfUserMgrGroupGetName2;
	#define GET_UserMgrGroupGetName2(fl)  s_pfCMGetAPI2( "UserMgrGroupGetName2", (RTS_VOID_FCTPTR *)&pfUserMgrGroupGetName2, (fl), 0, 0)
	#define CAL_UserMgrGroupGetName2  pfUserMgrGroupGetName2
	#define CHK_UserMgrGroupGetName2  (pfUserMgrGroupGetName2 != NULL)
	#define EXP_UserMgrGroupGetName2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetName2", (RTS_UINTPTR)UserMgrGroupGetName2, 0, 0) 
#endif




/**
 * <description>
 * Iteration interface to get next user group. Must be started with UserMgrGroupGetFirst()
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hPrevGroup" type="IN">Handle to the previous user group retrieved via UserMgrGroupGetFirst() or subsequent calls of UserMgrGroupGetNext()</param>
 * <param name="pResult" type="IN">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next user group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next user group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 * <result>Handle to the next user group or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserMgrGroupGetNext(RTS_HANDLE hUser, RTS_HANDLE hPrevGroup, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGRGROUPGETNEXT) (RTS_HANDLE hUser, RTS_HANDLE hPrevGroup, RTS_RESULT* pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPGETNEXT_NOTIMPLEMENTED)
	#define USE_UserMgrGroupGetNext
	#define EXT_UserMgrGroupGetNext
	#define GET_UserMgrGroupGetNext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupGetNext(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrGroupGetNext  FALSE
	#define EXP_UserMgrGroupGetNext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupGetNext
	#define EXT_UserMgrGroupGetNext
	#define GET_UserMgrGroupGetNext(fl)  CAL_CMGETAPI( "UserMgrGroupGetNext" ) 
	#define CAL_UserMgrGroupGetNext  UserMgrGroupGetNext
	#define CHK_UserMgrGroupGetNext  TRUE
	#define EXP_UserMgrGroupGetNext  CAL_CMEXPAPI( "UserMgrGroupGetNext" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupGetNext
	#define EXT_UserMgrGroupGetNext
	#define GET_UserMgrGroupGetNext(fl)  CAL_CMGETAPI( "UserMgrGroupGetNext" ) 
	#define CAL_UserMgrGroupGetNext  UserMgrGroupGetNext
	#define CHK_UserMgrGroupGetNext  TRUE
	#define EXP_UserMgrGroupGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetNext", (RTS_UINTPTR)UserMgrGroupGetNext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupGetNext
	#define EXT_CmpUserMgrUserMgrGroupGetNext
	#define GET_CmpUserMgrUserMgrGroupGetNext  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupGetNext pICmpUserMgr->IUserMgrGroupGetNext
	#define CHK_CmpUserMgrUserMgrGroupGetNext (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupGetNext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupGetNext
	#define EXT_UserMgrGroupGetNext
	#define GET_UserMgrGroupGetNext(fl)  CAL_CMGETAPI( "UserMgrGroupGetNext" ) 
	#define CAL_UserMgrGroupGetNext pICmpUserMgr->IUserMgrGroupGetNext
	#define CHK_UserMgrGroupGetNext (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupGetNext  CAL_CMEXPAPI( "UserMgrGroupGetNext" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupGetNext  PFUSERMGRGROUPGETNEXT pfUserMgrGroupGetNext;
	#define EXT_UserMgrGroupGetNext  extern PFUSERMGRGROUPGETNEXT pfUserMgrGroupGetNext;
	#define GET_UserMgrGroupGetNext(fl)  s_pfCMGetAPI2( "UserMgrGroupGetNext", (RTS_VOID_FCTPTR *)&pfUserMgrGroupGetNext, (fl), 0, 0)
	#define CAL_UserMgrGroupGetNext  pfUserMgrGroupGetNext
	#define CHK_UserMgrGroupGetNext  (pfUserMgrGroupGetNext != NULL)
	#define EXP_UserMgrGroupGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetNext", (RTS_UINTPTR)UserMgrGroupGetNext, 0, 0) 
#endif




/**
 * <description>
 * Iteration interface to get next configured user of the user group. Must be started with UserMgrGroupGetFirstUser()
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hGroup" type="IN">Group handle</param>
 * <param name="phIterator" type="IN">Pointer to iteration handle to use with next call to UserMgrGroupGetNextUser()</param>
 * <param name="pszUsername" type="OUT">Buffer to store the name, 59 chars max, plus NUL character</param>
 * <param name="iMaxLen" type="IN">Length of buffer</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next user available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next user available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid iterator handle</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">The buffer is too small</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrGroupGetNextUser(RTS_HANDLE hUser, RTS_HANDLE hGroup, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen);
typedef RTS_RESULT (CDECL * PFUSERMGRGROUPGETNEXTUSER) (RTS_HANDLE hUser, RTS_HANDLE hGroup, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRGROUPGETNEXTUSER_NOTIMPLEMENTED)
	#define USE_UserMgrGroupGetNextUser
	#define EXT_UserMgrGroupGetNextUser
	#define GET_UserMgrGroupGetNextUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrGroupGetNextUser(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrGroupGetNextUser  FALSE
	#define EXP_UserMgrGroupGetNextUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrGroupGetNextUser
	#define EXT_UserMgrGroupGetNextUser
	#define GET_UserMgrGroupGetNextUser(fl)  CAL_CMGETAPI( "UserMgrGroupGetNextUser" ) 
	#define CAL_UserMgrGroupGetNextUser  UserMgrGroupGetNextUser
	#define CHK_UserMgrGroupGetNextUser  TRUE
	#define EXP_UserMgrGroupGetNextUser  CAL_CMEXPAPI( "UserMgrGroupGetNextUser" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrGroupGetNextUser
	#define EXT_UserMgrGroupGetNextUser
	#define GET_UserMgrGroupGetNextUser(fl)  CAL_CMGETAPI( "UserMgrGroupGetNextUser" ) 
	#define CAL_UserMgrGroupGetNextUser  UserMgrGroupGetNextUser
	#define CHK_UserMgrGroupGetNextUser  TRUE
	#define EXP_UserMgrGroupGetNextUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetNextUser", (RTS_UINTPTR)UserMgrGroupGetNextUser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrGroupGetNextUser
	#define EXT_CmpUserMgrUserMgrGroupGetNextUser
	#define GET_CmpUserMgrUserMgrGroupGetNextUser  ERR_OK
	#define CAL_CmpUserMgrUserMgrGroupGetNextUser pICmpUserMgr->IUserMgrGroupGetNextUser
	#define CHK_CmpUserMgrUserMgrGroupGetNextUser (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrGroupGetNextUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrGroupGetNextUser
	#define EXT_UserMgrGroupGetNextUser
	#define GET_UserMgrGroupGetNextUser(fl)  CAL_CMGETAPI( "UserMgrGroupGetNextUser" ) 
	#define CAL_UserMgrGroupGetNextUser pICmpUserMgr->IUserMgrGroupGetNextUser
	#define CHK_UserMgrGroupGetNextUser (pICmpUserMgr != NULL)
	#define EXP_UserMgrGroupGetNextUser  CAL_CMEXPAPI( "UserMgrGroupGetNextUser" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrGroupGetNextUser  PFUSERMGRGROUPGETNEXTUSER pfUserMgrGroupGetNextUser;
	#define EXT_UserMgrGroupGetNextUser  extern PFUSERMGRGROUPGETNEXTUSER pfUserMgrGroupGetNextUser;
	#define GET_UserMgrGroupGetNextUser(fl)  s_pfCMGetAPI2( "UserMgrGroupGetNextUser", (RTS_VOID_FCTPTR *)&pfUserMgrGroupGetNextUser, (fl), 0, 0)
	#define CAL_UserMgrGroupGetNextUser  pfUserMgrGroupGetNextUser
	#define CHK_UserMgrGroupGetNextUser  (pfUserMgrGroupGetNextUser != NULL)
	#define EXP_UserMgrGroupGetNextUser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrGroupGetNextUser", (RTS_UINTPTR)UserMgrGroupGetNextUser, 0, 0) 
#endif




/**
 * <description>
 * Add a group to the specified object to manage access rights
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="hGroup" type="IN">Handle to the group that should be attached</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group could be added or is already added</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No memory to add group</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectAddGroup2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTADDGROUP2) (RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTADDGROUP2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectAddGroup2
	#define EXT_UserMgrObjectAddGroup2
	#define GET_UserMgrObjectAddGroup2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectAddGroup2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectAddGroup2  FALSE
	#define EXP_UserMgrObjectAddGroup2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectAddGroup2
	#define EXT_UserMgrObjectAddGroup2
	#define GET_UserMgrObjectAddGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectAddGroup2" ) 
	#define CAL_UserMgrObjectAddGroup2  UserMgrObjectAddGroup2
	#define CHK_UserMgrObjectAddGroup2  TRUE
	#define EXP_UserMgrObjectAddGroup2  CAL_CMEXPAPI( "UserMgrObjectAddGroup2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectAddGroup2
	#define EXT_UserMgrObjectAddGroup2
	#define GET_UserMgrObjectAddGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectAddGroup2" ) 
	#define CAL_UserMgrObjectAddGroup2  UserMgrObjectAddGroup2
	#define CHK_UserMgrObjectAddGroup2  TRUE
	#define EXP_UserMgrObjectAddGroup2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAddGroup2", (RTS_UINTPTR)UserMgrObjectAddGroup2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectAddGroup2
	#define EXT_CmpUserMgrUserMgrObjectAddGroup2
	#define GET_CmpUserMgrUserMgrObjectAddGroup2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectAddGroup2 pICmpUserMgr->IUserMgrObjectAddGroup2
	#define CHK_CmpUserMgrUserMgrObjectAddGroup2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectAddGroup2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectAddGroup2
	#define EXT_UserMgrObjectAddGroup2
	#define GET_UserMgrObjectAddGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectAddGroup2" ) 
	#define CAL_UserMgrObjectAddGroup2 pICmpUserMgr->IUserMgrObjectAddGroup2
	#define CHK_UserMgrObjectAddGroup2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectAddGroup2  CAL_CMEXPAPI( "UserMgrObjectAddGroup2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectAddGroup2  PFUSERMGROBJECTADDGROUP2 pfUserMgrObjectAddGroup2;
	#define EXT_UserMgrObjectAddGroup2  extern PFUSERMGROBJECTADDGROUP2 pfUserMgrObjectAddGroup2;
	#define GET_UserMgrObjectAddGroup2(fl)  s_pfCMGetAPI2( "UserMgrObjectAddGroup2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectAddGroup2, (fl), 0, 0)
	#define CAL_UserMgrObjectAddGroup2  pfUserMgrObjectAddGroup2
	#define CHK_UserMgrObjectAddGroup2  (pfUserMgrObjectAddGroup2 != NULL)
	#define EXP_UserMgrObjectAddGroup2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectAddGroup2", (RTS_UINTPTR)UserMgrObjectAddGroup2, 0, 0) 
#endif




/**
 * <description>
 * This function clears all granted and denied rights for all groups on that object.
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Access rights could be cleared</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectClearRights2(RTS_HANDLE hUser, RTS_HANDLE hObject);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTCLEARRIGHTS2) (RTS_HANDLE hUser, RTS_HANDLE hObject);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTCLEARRIGHTS2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectClearRights2
	#define EXT_UserMgrObjectClearRights2
	#define GET_UserMgrObjectClearRights2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectClearRights2(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectClearRights2  FALSE
	#define EXP_UserMgrObjectClearRights2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectClearRights2
	#define EXT_UserMgrObjectClearRights2
	#define GET_UserMgrObjectClearRights2(fl)  CAL_CMGETAPI( "UserMgrObjectClearRights2" ) 
	#define CAL_UserMgrObjectClearRights2  UserMgrObjectClearRights2
	#define CHK_UserMgrObjectClearRights2  TRUE
	#define EXP_UserMgrObjectClearRights2  CAL_CMEXPAPI( "UserMgrObjectClearRights2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectClearRights2
	#define EXT_UserMgrObjectClearRights2
	#define GET_UserMgrObjectClearRights2(fl)  CAL_CMGETAPI( "UserMgrObjectClearRights2" ) 
	#define CAL_UserMgrObjectClearRights2  UserMgrObjectClearRights2
	#define CHK_UserMgrObjectClearRights2  TRUE
	#define EXP_UserMgrObjectClearRights2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectClearRights2", (RTS_UINTPTR)UserMgrObjectClearRights2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectClearRights2
	#define EXT_CmpUserMgrUserMgrObjectClearRights2
	#define GET_CmpUserMgrUserMgrObjectClearRights2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectClearRights2 pICmpUserMgr->IUserMgrObjectClearRights2
	#define CHK_CmpUserMgrUserMgrObjectClearRights2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectClearRights2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectClearRights2
	#define EXT_UserMgrObjectClearRights2
	#define GET_UserMgrObjectClearRights2(fl)  CAL_CMGETAPI( "UserMgrObjectClearRights2" ) 
	#define CAL_UserMgrObjectClearRights2 pICmpUserMgr->IUserMgrObjectClearRights2
	#define CHK_UserMgrObjectClearRights2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectClearRights2  CAL_CMEXPAPI( "UserMgrObjectClearRights2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectClearRights2  PFUSERMGROBJECTCLEARRIGHTS2 pfUserMgrObjectClearRights2;
	#define EXT_UserMgrObjectClearRights2  extern PFUSERMGROBJECTCLEARRIGHTS2 pfUserMgrObjectClearRights2;
	#define GET_UserMgrObjectClearRights2(fl)  s_pfCMGetAPI2( "UserMgrObjectClearRights2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectClearRights2, (fl), 0, 0)
	#define CAL_UserMgrObjectClearRights2  pfUserMgrObjectClearRights2
	#define CHK_UserMgrObjectClearRights2  (pfUserMgrObjectClearRights2 != NULL)
	#define EXP_UserMgrObjectClearRights2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectClearRights2", (RTS_UINTPTR)UserMgrObjectClearRights2, 0, 0) 
#endif




/**
 * <description>
 * Iteration interface to get the first group in the user rights management of the specified object
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="pResult" type="IN">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 * <result>Handle to the first group of the object in the user rights management or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserMgrObjectGetFirstGroup2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTGETFIRSTGROUP2) (RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_RESULT* pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETFIRSTGROUP2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetFirstGroup2
	#define EXT_UserMgrObjectGetFirstGroup2
	#define GET_UserMgrObjectGetFirstGroup2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetFirstGroup2(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectGetFirstGroup2  FALSE
	#define EXP_UserMgrObjectGetFirstGroup2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetFirstGroup2
	#define EXT_UserMgrObjectGetFirstGroup2
	#define GET_UserMgrObjectGetFirstGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectGetFirstGroup2" ) 
	#define CAL_UserMgrObjectGetFirstGroup2  UserMgrObjectGetFirstGroup2
	#define CHK_UserMgrObjectGetFirstGroup2  TRUE
	#define EXP_UserMgrObjectGetFirstGroup2  CAL_CMEXPAPI( "UserMgrObjectGetFirstGroup2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetFirstGroup2
	#define EXT_UserMgrObjectGetFirstGroup2
	#define GET_UserMgrObjectGetFirstGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectGetFirstGroup2" ) 
	#define CAL_UserMgrObjectGetFirstGroup2  UserMgrObjectGetFirstGroup2
	#define CHK_UserMgrObjectGetFirstGroup2  TRUE
	#define EXP_UserMgrObjectGetFirstGroup2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetFirstGroup2", (RTS_UINTPTR)UserMgrObjectGetFirstGroup2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetFirstGroup2
	#define EXT_CmpUserMgrUserMgrObjectGetFirstGroup2
	#define GET_CmpUserMgrUserMgrObjectGetFirstGroup2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetFirstGroup2 pICmpUserMgr->IUserMgrObjectGetFirstGroup2
	#define CHK_CmpUserMgrUserMgrObjectGetFirstGroup2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetFirstGroup2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetFirstGroup2
	#define EXT_UserMgrObjectGetFirstGroup2
	#define GET_UserMgrObjectGetFirstGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectGetFirstGroup2" ) 
	#define CAL_UserMgrObjectGetFirstGroup2 pICmpUserMgr->IUserMgrObjectGetFirstGroup2
	#define CHK_UserMgrObjectGetFirstGroup2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetFirstGroup2  CAL_CMEXPAPI( "UserMgrObjectGetFirstGroup2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetFirstGroup2  PFUSERMGROBJECTGETFIRSTGROUP2 pfUserMgrObjectGetFirstGroup2;
	#define EXT_UserMgrObjectGetFirstGroup2  extern PFUSERMGROBJECTGETFIRSTGROUP2 pfUserMgrObjectGetFirstGroup2;
	#define GET_UserMgrObjectGetFirstGroup2(fl)  s_pfCMGetAPI2( "UserMgrObjectGetFirstGroup2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetFirstGroup2, (fl), 0, 0)
	#define CAL_UserMgrObjectGetFirstGroup2  pfUserMgrObjectGetFirstGroup2
	#define CHK_UserMgrObjectGetFirstGroup2  (pfUserMgrObjectGetFirstGroup2 != NULL)
	#define EXP_UserMgrObjectGetFirstGroup2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetFirstGroup2", (RTS_UINTPTR)UserMgrObjectGetFirstGroup2, 0, 0) 
#endif




/**
 * <description>
 * Get the rights and the denied rights of a group on a specified object
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="hGroup" type="IN">Handle to the group which access rights are requested</param>
 * <param name="pulRights" type="IN">Pointer to rights of the group. See category "User rights" for details</param>
 * <param name="pulDeniedRights" type="IN">Pointer to denied rights of the group. See category "User rights" for details</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Access rights could be retrieved</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectGetGroupRights2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32* pulRights, RTS_UI32* pulDeniedRights);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTGETGROUPRIGHTS2) (RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32* pulRights, RTS_UI32* pulDeniedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETGROUPRIGHTS2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetGroupRights2
	#define EXT_UserMgrObjectGetGroupRights2
	#define GET_UserMgrObjectGetGroupRights2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetGroupRights2(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectGetGroupRights2  FALSE
	#define EXP_UserMgrObjectGetGroupRights2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetGroupRights2
	#define EXT_UserMgrObjectGetGroupRights2
	#define GET_UserMgrObjectGetGroupRights2(fl)  CAL_CMGETAPI( "UserMgrObjectGetGroupRights2" ) 
	#define CAL_UserMgrObjectGetGroupRights2  UserMgrObjectGetGroupRights2
	#define CHK_UserMgrObjectGetGroupRights2  TRUE
	#define EXP_UserMgrObjectGetGroupRights2  CAL_CMEXPAPI( "UserMgrObjectGetGroupRights2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetGroupRights2
	#define EXT_UserMgrObjectGetGroupRights2
	#define GET_UserMgrObjectGetGroupRights2(fl)  CAL_CMGETAPI( "UserMgrObjectGetGroupRights2" ) 
	#define CAL_UserMgrObjectGetGroupRights2  UserMgrObjectGetGroupRights2
	#define CHK_UserMgrObjectGetGroupRights2  TRUE
	#define EXP_UserMgrObjectGetGroupRights2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetGroupRights2", (RTS_UINTPTR)UserMgrObjectGetGroupRights2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetGroupRights2
	#define EXT_CmpUserMgrUserMgrObjectGetGroupRights2
	#define GET_CmpUserMgrUserMgrObjectGetGroupRights2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetGroupRights2 pICmpUserMgr->IUserMgrObjectGetGroupRights2
	#define CHK_CmpUserMgrUserMgrObjectGetGroupRights2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetGroupRights2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetGroupRights2
	#define EXT_UserMgrObjectGetGroupRights2
	#define GET_UserMgrObjectGetGroupRights2(fl)  CAL_CMGETAPI( "UserMgrObjectGetGroupRights2" ) 
	#define CAL_UserMgrObjectGetGroupRights2 pICmpUserMgr->IUserMgrObjectGetGroupRights2
	#define CHK_UserMgrObjectGetGroupRights2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetGroupRights2  CAL_CMEXPAPI( "UserMgrObjectGetGroupRights2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetGroupRights2  PFUSERMGROBJECTGETGROUPRIGHTS2 pfUserMgrObjectGetGroupRights2;
	#define EXT_UserMgrObjectGetGroupRights2  extern PFUSERMGROBJECTGETGROUPRIGHTS2 pfUserMgrObjectGetGroupRights2;
	#define GET_UserMgrObjectGetGroupRights2(fl)  s_pfCMGetAPI2( "UserMgrObjectGetGroupRights2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetGroupRights2, (fl), 0, 0)
	#define CAL_UserMgrObjectGetGroupRights2  pfUserMgrObjectGetGroupRights2
	#define CHK_UserMgrObjectGetGroupRights2  (pfUserMgrObjectGetGroupRights2 != NULL)
	#define EXP_UserMgrObjectGetGroupRights2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetGroupRights2", (RTS_UINTPTR)UserMgrObjectGetGroupRights2, 0, 0) 
#endif




/**
 * <description>
 * Iteration interface to get the next group in the user rights management of the specified object. Must be started with UserMgrObjectGetFirstGroup()
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="hPrevGroup" type="IN">Handle to the previous group retrieved via UserMgrObjectGetFirstGroup() or subsequent calls of UserMgrObjectGetNextGroup()</param>
 * <param name="pResult" type="IN">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Next group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No next group available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 * <result>Handle to the next group of the object in the user rights management or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL UserMgrObjectGetNextGroup2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hPrevGroup, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFUSERMGROBJECTGETNEXTGROUP2) (RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hPrevGroup, RTS_RESULT* pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTGETNEXTGROUP2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectGetNextGroup2
	#define EXT_UserMgrObjectGetNextGroup2
	#define GET_UserMgrObjectGetNextGroup2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectGetNextGroup2(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_UserMgrObjectGetNextGroup2  FALSE
	#define EXP_UserMgrObjectGetNextGroup2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectGetNextGroup2
	#define EXT_UserMgrObjectGetNextGroup2
	#define GET_UserMgrObjectGetNextGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectGetNextGroup2" ) 
	#define CAL_UserMgrObjectGetNextGroup2  UserMgrObjectGetNextGroup2
	#define CHK_UserMgrObjectGetNextGroup2  TRUE
	#define EXP_UserMgrObjectGetNextGroup2  CAL_CMEXPAPI( "UserMgrObjectGetNextGroup2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectGetNextGroup2
	#define EXT_UserMgrObjectGetNextGroup2
	#define GET_UserMgrObjectGetNextGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectGetNextGroup2" ) 
	#define CAL_UserMgrObjectGetNextGroup2  UserMgrObjectGetNextGroup2
	#define CHK_UserMgrObjectGetNextGroup2  TRUE
	#define EXP_UserMgrObjectGetNextGroup2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetNextGroup2", (RTS_UINTPTR)UserMgrObjectGetNextGroup2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectGetNextGroup2
	#define EXT_CmpUserMgrUserMgrObjectGetNextGroup2
	#define GET_CmpUserMgrUserMgrObjectGetNextGroup2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectGetNextGroup2 pICmpUserMgr->IUserMgrObjectGetNextGroup2
	#define CHK_CmpUserMgrUserMgrObjectGetNextGroup2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectGetNextGroup2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectGetNextGroup2
	#define EXT_UserMgrObjectGetNextGroup2
	#define GET_UserMgrObjectGetNextGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectGetNextGroup2" ) 
	#define CAL_UserMgrObjectGetNextGroup2 pICmpUserMgr->IUserMgrObjectGetNextGroup2
	#define CHK_UserMgrObjectGetNextGroup2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectGetNextGroup2  CAL_CMEXPAPI( "UserMgrObjectGetNextGroup2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectGetNextGroup2  PFUSERMGROBJECTGETNEXTGROUP2 pfUserMgrObjectGetNextGroup2;
	#define EXT_UserMgrObjectGetNextGroup2  extern PFUSERMGROBJECTGETNEXTGROUP2 pfUserMgrObjectGetNextGroup2;
	#define GET_UserMgrObjectGetNextGroup2(fl)  s_pfCMGetAPI2( "UserMgrObjectGetNextGroup2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectGetNextGroup2, (fl), 0, 0)
	#define CAL_UserMgrObjectGetNextGroup2  pfUserMgrObjectGetNextGroup2
	#define CHK_UserMgrObjectGetNextGroup2  (pfUserMgrObjectGetNextGroup2 != NULL)
	#define EXP_UserMgrObjectGetNextGroup2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectGetNextGroup2", (RTS_UINTPTR)UserMgrObjectGetNextGroup2, 0, 0) 
#endif




/**
 * <description>
 * Remove a group from the specified object
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="hGroup" type="IN">Handle to the group that should be removed</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Group could be removed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectRemoveGroup2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTREMOVEGROUP2) (RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTREMOVEGROUP2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectRemoveGroup2
	#define EXT_UserMgrObjectRemoveGroup2
	#define GET_UserMgrObjectRemoveGroup2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectRemoveGroup2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectRemoveGroup2  FALSE
	#define EXP_UserMgrObjectRemoveGroup2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectRemoveGroup2
	#define EXT_UserMgrObjectRemoveGroup2
	#define GET_UserMgrObjectRemoveGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectRemoveGroup2" ) 
	#define CAL_UserMgrObjectRemoveGroup2  UserMgrObjectRemoveGroup2
	#define CHK_UserMgrObjectRemoveGroup2  TRUE
	#define EXP_UserMgrObjectRemoveGroup2  CAL_CMEXPAPI( "UserMgrObjectRemoveGroup2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectRemoveGroup2
	#define EXT_UserMgrObjectRemoveGroup2
	#define GET_UserMgrObjectRemoveGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectRemoveGroup2" ) 
	#define CAL_UserMgrObjectRemoveGroup2  UserMgrObjectRemoveGroup2
	#define CHK_UserMgrObjectRemoveGroup2  TRUE
	#define EXP_UserMgrObjectRemoveGroup2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectRemoveGroup2", (RTS_UINTPTR)UserMgrObjectRemoveGroup2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectRemoveGroup2
	#define EXT_CmpUserMgrUserMgrObjectRemoveGroup2
	#define GET_CmpUserMgrUserMgrObjectRemoveGroup2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectRemoveGroup2 pICmpUserMgr->IUserMgrObjectRemoveGroup2
	#define CHK_CmpUserMgrUserMgrObjectRemoveGroup2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectRemoveGroup2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectRemoveGroup2
	#define EXT_UserMgrObjectRemoveGroup2
	#define GET_UserMgrObjectRemoveGroup2(fl)  CAL_CMGETAPI( "UserMgrObjectRemoveGroup2" ) 
	#define CAL_UserMgrObjectRemoveGroup2 pICmpUserMgr->IUserMgrObjectRemoveGroup2
	#define CHK_UserMgrObjectRemoveGroup2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectRemoveGroup2  CAL_CMEXPAPI( "UserMgrObjectRemoveGroup2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectRemoveGroup2  PFUSERMGROBJECTREMOVEGROUP2 pfUserMgrObjectRemoveGroup2;
	#define EXT_UserMgrObjectRemoveGroup2  extern PFUSERMGROBJECTREMOVEGROUP2 pfUserMgrObjectRemoveGroup2;
	#define GET_UserMgrObjectRemoveGroup2(fl)  s_pfCMGetAPI2( "UserMgrObjectRemoveGroup2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectRemoveGroup2, (fl), 0, 0)
	#define CAL_UserMgrObjectRemoveGroup2  pfUserMgrObjectRemoveGroup2
	#define CHK_UserMgrObjectRemoveGroup2  (pfUserMgrObjectRemoveGroup2 != NULL)
	#define EXP_UserMgrObjectRemoveGroup2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectRemoveGroup2", (RTS_UINTPTR)UserMgrObjectRemoveGroup2, 0, 0) 
#endif




/**
 * <description>
 * Set the denied access rights for the group at the specified object
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="hGroup" type="IN">Handle to the group</param>
 * <param name="ulDeniedRights" type="IN">Denied rights of the group. See category "User rights" for details</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Access rights could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectSetGroupDeniedRights2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulDeniedRights);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTSETGROUPDENIEDRIGHTS2) (RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulDeniedRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTSETGROUPDENIEDRIGHTS2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectSetGroupDeniedRights2
	#define EXT_UserMgrObjectSetGroupDeniedRights2
	#define GET_UserMgrObjectSetGroupDeniedRights2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectSetGroupDeniedRights2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectSetGroupDeniedRights2  FALSE
	#define EXP_UserMgrObjectSetGroupDeniedRights2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectSetGroupDeniedRights2
	#define EXT_UserMgrObjectSetGroupDeniedRights2
	#define GET_UserMgrObjectSetGroupDeniedRights2(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupDeniedRights2" ) 
	#define CAL_UserMgrObjectSetGroupDeniedRights2  UserMgrObjectSetGroupDeniedRights2
	#define CHK_UserMgrObjectSetGroupDeniedRights2  TRUE
	#define EXP_UserMgrObjectSetGroupDeniedRights2  CAL_CMEXPAPI( "UserMgrObjectSetGroupDeniedRights2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectSetGroupDeniedRights2
	#define EXT_UserMgrObjectSetGroupDeniedRights2
	#define GET_UserMgrObjectSetGroupDeniedRights2(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupDeniedRights2" ) 
	#define CAL_UserMgrObjectSetGroupDeniedRights2  UserMgrObjectSetGroupDeniedRights2
	#define CHK_UserMgrObjectSetGroupDeniedRights2  TRUE
	#define EXP_UserMgrObjectSetGroupDeniedRights2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetGroupDeniedRights2", (RTS_UINTPTR)UserMgrObjectSetGroupDeniedRights2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectSetGroupDeniedRights2
	#define EXT_CmpUserMgrUserMgrObjectSetGroupDeniedRights2
	#define GET_CmpUserMgrUserMgrObjectSetGroupDeniedRights2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectSetGroupDeniedRights2 pICmpUserMgr->IUserMgrObjectSetGroupDeniedRights2
	#define CHK_CmpUserMgrUserMgrObjectSetGroupDeniedRights2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectSetGroupDeniedRights2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectSetGroupDeniedRights2
	#define EXT_UserMgrObjectSetGroupDeniedRights2
	#define GET_UserMgrObjectSetGroupDeniedRights2(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupDeniedRights2" ) 
	#define CAL_UserMgrObjectSetGroupDeniedRights2 pICmpUserMgr->IUserMgrObjectSetGroupDeniedRights2
	#define CHK_UserMgrObjectSetGroupDeniedRights2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectSetGroupDeniedRights2  CAL_CMEXPAPI( "UserMgrObjectSetGroupDeniedRights2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectSetGroupDeniedRights2  PFUSERMGROBJECTSETGROUPDENIEDRIGHTS2 pfUserMgrObjectSetGroupDeniedRights2;
	#define EXT_UserMgrObjectSetGroupDeniedRights2  extern PFUSERMGROBJECTSETGROUPDENIEDRIGHTS2 pfUserMgrObjectSetGroupDeniedRights2;
	#define GET_UserMgrObjectSetGroupDeniedRights2(fl)  s_pfCMGetAPI2( "UserMgrObjectSetGroupDeniedRights2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectSetGroupDeniedRights2, (fl), 0, 0)
	#define CAL_UserMgrObjectSetGroupDeniedRights2  pfUserMgrObjectSetGroupDeniedRights2
	#define CHK_UserMgrObjectSetGroupDeniedRights2  (pfUserMgrObjectSetGroupDeniedRights2 != NULL)
	#define EXP_UserMgrObjectSetGroupDeniedRights2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetGroupDeniedRights2", (RTS_UINTPTR)UserMgrObjectSetGroupDeniedRights2, 0, 0) 
#endif




/**
 * <description>
 * Set the access rights for the group at the specified object
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="hObject" type="IN">Handle to the object</param>
 * <param name="hGroup" type="IN">Handle to the group</param>
 * <param name="ulRights" type="IN">Rights of the group. See category "User rights" for details</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Access rights could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid handles</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrObjectSetGroupRights2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulRights);
typedef RTS_RESULT (CDECL * PFUSERMGROBJECTSETGROUPRIGHTS2) (RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulRights);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGROBJECTSETGROUPRIGHTS2_NOTIMPLEMENTED)
	#define USE_UserMgrObjectSetGroupRights2
	#define EXT_UserMgrObjectSetGroupRights2
	#define GET_UserMgrObjectSetGroupRights2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrObjectSetGroupRights2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrObjectSetGroupRights2  FALSE
	#define EXP_UserMgrObjectSetGroupRights2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrObjectSetGroupRights2
	#define EXT_UserMgrObjectSetGroupRights2
	#define GET_UserMgrObjectSetGroupRights2(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupRights2" ) 
	#define CAL_UserMgrObjectSetGroupRights2  UserMgrObjectSetGroupRights2
	#define CHK_UserMgrObjectSetGroupRights2  TRUE
	#define EXP_UserMgrObjectSetGroupRights2  CAL_CMEXPAPI( "UserMgrObjectSetGroupRights2" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrObjectSetGroupRights2
	#define EXT_UserMgrObjectSetGroupRights2
	#define GET_UserMgrObjectSetGroupRights2(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupRights2" ) 
	#define CAL_UserMgrObjectSetGroupRights2  UserMgrObjectSetGroupRights2
	#define CHK_UserMgrObjectSetGroupRights2  TRUE
	#define EXP_UserMgrObjectSetGroupRights2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetGroupRights2", (RTS_UINTPTR)UserMgrObjectSetGroupRights2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrObjectSetGroupRights2
	#define EXT_CmpUserMgrUserMgrObjectSetGroupRights2
	#define GET_CmpUserMgrUserMgrObjectSetGroupRights2  ERR_OK
	#define CAL_CmpUserMgrUserMgrObjectSetGroupRights2 pICmpUserMgr->IUserMgrObjectSetGroupRights2
	#define CHK_CmpUserMgrUserMgrObjectSetGroupRights2 (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrObjectSetGroupRights2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrObjectSetGroupRights2
	#define EXT_UserMgrObjectSetGroupRights2
	#define GET_UserMgrObjectSetGroupRights2(fl)  CAL_CMGETAPI( "UserMgrObjectSetGroupRights2" ) 
	#define CAL_UserMgrObjectSetGroupRights2 pICmpUserMgr->IUserMgrObjectSetGroupRights2
	#define CHK_UserMgrObjectSetGroupRights2 (pICmpUserMgr != NULL)
	#define EXP_UserMgrObjectSetGroupRights2  CAL_CMEXPAPI( "UserMgrObjectSetGroupRights2" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrObjectSetGroupRights2  PFUSERMGROBJECTSETGROUPRIGHTS2 pfUserMgrObjectSetGroupRights2;
	#define EXT_UserMgrObjectSetGroupRights2  extern PFUSERMGROBJECTSETGROUPRIGHTS2 pfUserMgrObjectSetGroupRights2;
	#define GET_UserMgrObjectSetGroupRights2(fl)  s_pfCMGetAPI2( "UserMgrObjectSetGroupRights2", (RTS_VOID_FCTPTR *)&pfUserMgrObjectSetGroupRights2, (fl), 0, 0)
	#define CAL_UserMgrObjectSetGroupRights2  pfUserMgrObjectSetGroupRights2
	#define CHK_UserMgrObjectSetGroupRights2  (pfUserMgrObjectSetGroupRights2 != NULL)
	#define EXP_UserMgrObjectSetGroupRights2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrObjectSetGroupRights2", (RTS_UINTPTR)UserMgrObjectSetGroupRights2, 0, 0) 
#endif




/**
 * <description>
 * Iteration interface to get the first user
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="phIterator" type="IN">Pointer to iteration handle to use with UserMgrUserGetNext()</param>
 * <param name="pszUsername" type="OUT">Buffer to store the name, 59 chars max, plus NUL character</param>
 * <param name="iMaxLen" type="IN">Length of buffer in pszUsername</param>
 * <param name="pUserProperty" type="OUT">Pointer to user properties. See category 'User and group properites'</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First user available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No user configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrUserGetFirst(RTS_HANDLE hUser, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen, RTS_UI32* pUserProperty);
typedef RTS_RESULT (CDECL * PFUSERMGRUSERGETFIRST) (RTS_HANDLE hUser, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen, RTS_UI32* pUserProperty);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUSERGETFIRST_NOTIMPLEMENTED)
	#define USE_UserMgrUserGetFirst
	#define EXT_UserMgrUserGetFirst
	#define GET_UserMgrUserGetFirst(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUserGetFirst(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUserGetFirst  FALSE
	#define EXP_UserMgrUserGetFirst  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUserGetFirst
	#define EXT_UserMgrUserGetFirst
	#define GET_UserMgrUserGetFirst(fl)  CAL_CMGETAPI( "UserMgrUserGetFirst" ) 
	#define CAL_UserMgrUserGetFirst  UserMgrUserGetFirst
	#define CHK_UserMgrUserGetFirst  TRUE
	#define EXP_UserMgrUserGetFirst  CAL_CMEXPAPI( "UserMgrUserGetFirst" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUserGetFirst
	#define EXT_UserMgrUserGetFirst
	#define GET_UserMgrUserGetFirst(fl)  CAL_CMGETAPI( "UserMgrUserGetFirst" ) 
	#define CAL_UserMgrUserGetFirst  UserMgrUserGetFirst
	#define CHK_UserMgrUserGetFirst  TRUE
	#define EXP_UserMgrUserGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserGetFirst", (RTS_UINTPTR)UserMgrUserGetFirst, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUserGetFirst
	#define EXT_CmpUserMgrUserMgrUserGetFirst
	#define GET_CmpUserMgrUserMgrUserGetFirst  ERR_OK
	#define CAL_CmpUserMgrUserMgrUserGetFirst pICmpUserMgr->IUserMgrUserGetFirst
	#define CHK_CmpUserMgrUserMgrUserGetFirst (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUserGetFirst  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUserGetFirst
	#define EXT_UserMgrUserGetFirst
	#define GET_UserMgrUserGetFirst(fl)  CAL_CMGETAPI( "UserMgrUserGetFirst" ) 
	#define CAL_UserMgrUserGetFirst pICmpUserMgr->IUserMgrUserGetFirst
	#define CHK_UserMgrUserGetFirst (pICmpUserMgr != NULL)
	#define EXP_UserMgrUserGetFirst  CAL_CMEXPAPI( "UserMgrUserGetFirst" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUserGetFirst  PFUSERMGRUSERGETFIRST pfUserMgrUserGetFirst;
	#define EXT_UserMgrUserGetFirst  extern PFUSERMGRUSERGETFIRST pfUserMgrUserGetFirst;
	#define GET_UserMgrUserGetFirst(fl)  s_pfCMGetAPI2( "UserMgrUserGetFirst", (RTS_VOID_FCTPTR *)&pfUserMgrUserGetFirst, (fl), 0, 0)
	#define CAL_UserMgrUserGetFirst  pfUserMgrUserGetFirst
	#define CHK_UserMgrUserGetFirst  (pfUserMgrUserGetFirst != NULL)
	#define EXP_UserMgrUserGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserGetFirst", (RTS_UINTPTR)UserMgrUserGetFirst, 0, 0) 
#endif




/**
 * <description>
 * Iteration interface to get next user. Must be started with UserMgrUserGetFirst()
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="phIterator" type="IN">Pointer to iteration handle to use with next call to UserMgrUserGetNext()</param>
 * <param name="pszUsername" type="OUT">Buffer to store the name, 59 chars max, plus NUL character</param>
 * <param name="iMaxLen" type="IN">Length of buffer in pszUsername</param>
 * <param name="pUserProperty" type="OUT">Pointer to user properties. See category 'User and group properites'</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">First user available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">No user configured</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">The buffer is too small</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrUserGetNext(RTS_HANDLE hUser, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen, RTS_UI32* pUserProperty);
typedef RTS_RESULT (CDECL * PFUSERMGRUSERGETNEXT) (RTS_HANDLE hUser, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen, RTS_UI32* pUserProperty);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUSERGETNEXT_NOTIMPLEMENTED)
	#define USE_UserMgrUserGetNext
	#define EXT_UserMgrUserGetNext
	#define GET_UserMgrUserGetNext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUserGetNext(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUserGetNext  FALSE
	#define EXP_UserMgrUserGetNext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUserGetNext
	#define EXT_UserMgrUserGetNext
	#define GET_UserMgrUserGetNext(fl)  CAL_CMGETAPI( "UserMgrUserGetNext" ) 
	#define CAL_UserMgrUserGetNext  UserMgrUserGetNext
	#define CHK_UserMgrUserGetNext  TRUE
	#define EXP_UserMgrUserGetNext  CAL_CMEXPAPI( "UserMgrUserGetNext" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUserGetNext
	#define EXT_UserMgrUserGetNext
	#define GET_UserMgrUserGetNext(fl)  CAL_CMGETAPI( "UserMgrUserGetNext" ) 
	#define CAL_UserMgrUserGetNext  UserMgrUserGetNext
	#define CHK_UserMgrUserGetNext  TRUE
	#define EXP_UserMgrUserGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserGetNext", (RTS_UINTPTR)UserMgrUserGetNext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUserGetNext
	#define EXT_CmpUserMgrUserMgrUserGetNext
	#define GET_CmpUserMgrUserMgrUserGetNext  ERR_OK
	#define CAL_CmpUserMgrUserMgrUserGetNext pICmpUserMgr->IUserMgrUserGetNext
	#define CHK_CmpUserMgrUserMgrUserGetNext (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUserGetNext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUserGetNext
	#define EXT_UserMgrUserGetNext
	#define GET_UserMgrUserGetNext(fl)  CAL_CMGETAPI( "UserMgrUserGetNext" ) 
	#define CAL_UserMgrUserGetNext pICmpUserMgr->IUserMgrUserGetNext
	#define CHK_UserMgrUserGetNext (pICmpUserMgr != NULL)
	#define EXP_UserMgrUserGetNext  CAL_CMEXPAPI( "UserMgrUserGetNext" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUserGetNext  PFUSERMGRUSERGETNEXT pfUserMgrUserGetNext;
	#define EXT_UserMgrUserGetNext  extern PFUSERMGRUSERGETNEXT pfUserMgrUserGetNext;
	#define GET_UserMgrUserGetNext(fl)  s_pfCMGetAPI2( "UserMgrUserGetNext", (RTS_VOID_FCTPTR *)&pfUserMgrUserGetNext, (fl), 0, 0)
	#define CAL_UserMgrUserGetNext  pfUserMgrUserGetNext
	#define CHK_UserMgrUserGetNext  (pfUserMgrUserGetNext != NULL)
	#define EXP_UserMgrUserGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserGetNext", (RTS_UINTPTR)UserMgrUserGetNext, 0, 0) 
#endif




/**
 * <description>
 * Get properties of a user
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="pszUser" type="IN">User name, limited to 59 chars</param>
 * <param name="pResult" type="IN">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Properties could be retrieved</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 * <result>The users properties. See category 'User and group properites'</result>
 */
RTS_UI32 CDECL UserMgrUserGetProperty(RTS_HANDLE hUser, char* pszUser, RTS_RESULT* pResult);
typedef RTS_UI32 (CDECL * PFUSERMGRUSERGETPROPERTY) (RTS_HANDLE hUser, char* pszUser, RTS_RESULT* pResult);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUSERGETPROPERTY_NOTIMPLEMENTED)
	#define USE_UserMgrUserGetProperty
	#define EXT_UserMgrUserGetProperty
	#define GET_UserMgrUserGetProperty(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUserGetProperty(p0,p1,p2)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUserGetProperty  FALSE
	#define EXP_UserMgrUserGetProperty  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUserGetProperty
	#define EXT_UserMgrUserGetProperty
	#define GET_UserMgrUserGetProperty(fl)  CAL_CMGETAPI( "UserMgrUserGetProperty" ) 
	#define CAL_UserMgrUserGetProperty  UserMgrUserGetProperty
	#define CHK_UserMgrUserGetProperty  TRUE
	#define EXP_UserMgrUserGetProperty  CAL_CMEXPAPI( "UserMgrUserGetProperty" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUserGetProperty
	#define EXT_UserMgrUserGetProperty
	#define GET_UserMgrUserGetProperty(fl)  CAL_CMGETAPI( "UserMgrUserGetProperty" ) 
	#define CAL_UserMgrUserGetProperty  UserMgrUserGetProperty
	#define CHK_UserMgrUserGetProperty  TRUE
	#define EXP_UserMgrUserGetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserGetProperty", (RTS_UINTPTR)UserMgrUserGetProperty, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUserGetProperty
	#define EXT_CmpUserMgrUserMgrUserGetProperty
	#define GET_CmpUserMgrUserMgrUserGetProperty  ERR_OK
	#define CAL_CmpUserMgrUserMgrUserGetProperty pICmpUserMgr->IUserMgrUserGetProperty
	#define CHK_CmpUserMgrUserMgrUserGetProperty (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUserGetProperty  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUserGetProperty
	#define EXT_UserMgrUserGetProperty
	#define GET_UserMgrUserGetProperty(fl)  CAL_CMGETAPI( "UserMgrUserGetProperty" ) 
	#define CAL_UserMgrUserGetProperty pICmpUserMgr->IUserMgrUserGetProperty
	#define CHK_UserMgrUserGetProperty (pICmpUserMgr != NULL)
	#define EXP_UserMgrUserGetProperty  CAL_CMEXPAPI( "UserMgrUserGetProperty" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUserGetProperty  PFUSERMGRUSERGETPROPERTY pfUserMgrUserGetProperty;
	#define EXT_UserMgrUserGetProperty  extern PFUSERMGRUSERGETPROPERTY pfUserMgrUserGetProperty;
	#define GET_UserMgrUserGetProperty(fl)  s_pfCMGetAPI2( "UserMgrUserGetProperty", (RTS_VOID_FCTPTR *)&pfUserMgrUserGetProperty, (fl), 0, 0)
	#define CAL_UserMgrUserGetProperty  pfUserMgrUserGetProperty
	#define CHK_UserMgrUserGetProperty  (pfUserMgrUserGetProperty != NULL)
	#define EXP_UserMgrUserGetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserGetProperty", (RTS_UINTPTR)UserMgrUserGetProperty, 0, 0) 
#endif




/**
 * <description>
 * Set or overwrite the credentials of the user
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="pszUser" type="IN">User name, limited to 59 chars</param>
 * <param name="pCredentials" type="IN">Credentials, only password is available so far, password (plain text) is limited to 59 chars</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Credentials could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user name or credentials</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">The new and the old credentials are the same</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PASSWORD_POLICY">Password does not fulfill the password policy requirements</errorcode>
 */
RTS_RESULT CDECL UserMgrUserSetCredentials(RTS_HANDLE hUser, char* pszUser, UserMgrCredentials* pCredentials);
typedef RTS_RESULT (CDECL * PFUSERMGRUSERSETCREDENTIALS) (RTS_HANDLE hUser, char* pszUser, UserMgrCredentials* pCredentials);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUSERSETCREDENTIALS_NOTIMPLEMENTED)
	#define USE_UserMgrUserSetCredentials
	#define EXT_UserMgrUserSetCredentials
	#define GET_UserMgrUserSetCredentials(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUserSetCredentials(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUserSetCredentials  FALSE
	#define EXP_UserMgrUserSetCredentials  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUserSetCredentials
	#define EXT_UserMgrUserSetCredentials
	#define GET_UserMgrUserSetCredentials(fl)  CAL_CMGETAPI( "UserMgrUserSetCredentials" ) 
	#define CAL_UserMgrUserSetCredentials  UserMgrUserSetCredentials
	#define CHK_UserMgrUserSetCredentials  TRUE
	#define EXP_UserMgrUserSetCredentials  CAL_CMEXPAPI( "UserMgrUserSetCredentials" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUserSetCredentials
	#define EXT_UserMgrUserSetCredentials
	#define GET_UserMgrUserSetCredentials(fl)  CAL_CMGETAPI( "UserMgrUserSetCredentials" ) 
	#define CAL_UserMgrUserSetCredentials  UserMgrUserSetCredentials
	#define CHK_UserMgrUserSetCredentials  TRUE
	#define EXP_UserMgrUserSetCredentials  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserSetCredentials", (RTS_UINTPTR)UserMgrUserSetCredentials, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUserSetCredentials
	#define EXT_CmpUserMgrUserMgrUserSetCredentials
	#define GET_CmpUserMgrUserMgrUserSetCredentials  ERR_OK
	#define CAL_CmpUserMgrUserMgrUserSetCredentials pICmpUserMgr->IUserMgrUserSetCredentials
	#define CHK_CmpUserMgrUserMgrUserSetCredentials (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUserSetCredentials  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUserSetCredentials
	#define EXT_UserMgrUserSetCredentials
	#define GET_UserMgrUserSetCredentials(fl)  CAL_CMGETAPI( "UserMgrUserSetCredentials" ) 
	#define CAL_UserMgrUserSetCredentials pICmpUserMgr->IUserMgrUserSetCredentials
	#define CHK_UserMgrUserSetCredentials (pICmpUserMgr != NULL)
	#define EXP_UserMgrUserSetCredentials  CAL_CMEXPAPI( "UserMgrUserSetCredentials" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUserSetCredentials  PFUSERMGRUSERSETCREDENTIALS pfUserMgrUserSetCredentials;
	#define EXT_UserMgrUserSetCredentials  extern PFUSERMGRUSERSETCREDENTIALS pfUserMgrUserSetCredentials;
	#define GET_UserMgrUserSetCredentials(fl)  s_pfCMGetAPI2( "UserMgrUserSetCredentials", (RTS_VOID_FCTPTR *)&pfUserMgrUserSetCredentials, (fl), 0, 0)
	#define CAL_UserMgrUserSetCredentials  pfUserMgrUserSetCredentials
	#define CHK_UserMgrUserSetCredentials  (pfUserMgrUserSetCredentials != NULL)
	#define EXP_UserMgrUserSetCredentials  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserSetCredentials", (RTS_UINTPTR)UserMgrUserSetCredentials, 0, 0) 
#endif




/**
 * <description>
 * Set properties of a user. See category 'User and group properites'
 * </description>
 * <param name="hUser" type="IN">Handle to authorized user</param>
 * <param name="pszUser" type="IN">User name, limited to 59 chars</param>
 * <param name="userProperty" type="IN">User properties</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Properties could be set</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid user</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Invalid combination of property flags (e.g. password change not allowed but password not up to date)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">User not authorized</errorcode>
 */
RTS_RESULT CDECL UserMgrUserSetProperty(RTS_HANDLE hUser, char* pszUser, RTS_UI32 userProperty);
typedef RTS_RESULT (CDECL * PFUSERMGRUSERSETPROPERTY) (RTS_HANDLE hUser, char* pszUser, RTS_UI32 userProperty);
#if defined(CMPUSERMGR_NOTIMPLEMENTED) || defined(USERMGRUSERSETPROPERTY_NOTIMPLEMENTED)
	#define USE_UserMgrUserSetProperty
	#define EXT_UserMgrUserSetProperty
	#define GET_UserMgrUserSetProperty(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserMgrUserSetProperty(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserMgrUserSetProperty  FALSE
	#define EXP_UserMgrUserSetProperty  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserMgrUserSetProperty
	#define EXT_UserMgrUserSetProperty
	#define GET_UserMgrUserSetProperty(fl)  CAL_CMGETAPI( "UserMgrUserSetProperty" ) 
	#define CAL_UserMgrUserSetProperty  UserMgrUserSetProperty
	#define CHK_UserMgrUserSetProperty  TRUE
	#define EXP_UserMgrUserSetProperty  CAL_CMEXPAPI( "UserMgrUserSetProperty" ) 
#elif defined(MIXED_LINK) && !defined(CMPUSERMGR_EXTERNAL)
	#define USE_UserMgrUserSetProperty
	#define EXT_UserMgrUserSetProperty
	#define GET_UserMgrUserSetProperty(fl)  CAL_CMGETAPI( "UserMgrUserSetProperty" ) 
	#define CAL_UserMgrUserSetProperty  UserMgrUserSetProperty
	#define CHK_UserMgrUserSetProperty  TRUE
	#define EXP_UserMgrUserSetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserSetProperty", (RTS_UINTPTR)UserMgrUserSetProperty, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserMgrUserMgrUserSetProperty
	#define EXT_CmpUserMgrUserMgrUserSetProperty
	#define GET_CmpUserMgrUserMgrUserSetProperty  ERR_OK
	#define CAL_CmpUserMgrUserMgrUserSetProperty pICmpUserMgr->IUserMgrUserSetProperty
	#define CHK_CmpUserMgrUserMgrUserSetProperty (pICmpUserMgr != NULL)
	#define EXP_CmpUserMgrUserMgrUserSetProperty  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserMgrUserSetProperty
	#define EXT_UserMgrUserSetProperty
	#define GET_UserMgrUserSetProperty(fl)  CAL_CMGETAPI( "UserMgrUserSetProperty" ) 
	#define CAL_UserMgrUserSetProperty pICmpUserMgr->IUserMgrUserSetProperty
	#define CHK_UserMgrUserSetProperty (pICmpUserMgr != NULL)
	#define EXP_UserMgrUserSetProperty  CAL_CMEXPAPI( "UserMgrUserSetProperty" ) 
#else /* DYNAMIC_LINK */
	#define USE_UserMgrUserSetProperty  PFUSERMGRUSERSETPROPERTY pfUserMgrUserSetProperty;
	#define EXT_UserMgrUserSetProperty  extern PFUSERMGRUSERSETPROPERTY pfUserMgrUserSetProperty;
	#define GET_UserMgrUserSetProperty(fl)  s_pfCMGetAPI2( "UserMgrUserSetProperty", (RTS_VOID_FCTPTR *)&pfUserMgrUserSetProperty, (fl), 0, 0)
	#define CAL_UserMgrUserSetProperty  pfUserMgrUserSetProperty
	#define CHK_UserMgrUserSetProperty  (pfUserMgrUserSetProperty != NULL)
	#define EXP_UserMgrUserSetProperty  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"UserMgrUserSetProperty", (RTS_UINTPTR)UserMgrUserSetProperty, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFUSERMGRREGISTERUSERDB IUserMgrRegisterUserDB;
 	PFUSERMGRUNREGISTERUSERDB IUserMgrUnregisterUserDB;
 	PFUSERMGRREGISTERUSERDBCONFIG IUserMgrRegisterUserDBConfig;
 	PFUSERMGRUNREGISTERUSERDBCONFIG IUserMgrUnregisterUserDBConfig;
 	PFUSERMGRREGISTERUSERGROUPSDB IUserMgrRegisterUserGroupsDB;
 	PFUSERMGRUNREGISTERUSERGROUPSDB IUserMgrUnregisterUserGroupsDB;
 	PFUSERMGRREGISTERUSERGROUPSDBCONFIG IUserMgrRegisterUserGroupsDBConfig;
 	PFUSERMGRUNREGISTERUSERGROUPSDBCONFIG IUserMgrUnregisterUserGroupsDBConfig;
 	PFUSERMGRREGISTERUSEROBJECTSDB IUserMgrRegisterUserObjectsDB;
 	PFUSERMGRUNREGISTERUSEROBJECTSDB IUserMgrUnregisterUserObjectsDB;
 	PFUSERMGRREGISTERUSEROBJECTSDBCONFIG IUserMgrRegisterUserObjectsDBConfig;
 	PFUSERMGRUNREGISTERUSEROBJECTSDBCONFIG IUserMgrUnregisterUserObjectsDBConfig;
 	PFUSERMGRSELECTAUTHTYPE IUserMgrSelectAuthType;
 	PFUSERMGRHANDLELOGINSERVICE IUserMgrHandleLoginService;
 	PFUSERMGRHANDLEASYMMETRICLOGINPROTOCOL IUserMgrHandleAsymmetricLoginProtocol;
 	PFUSERMGRISSERVICEALLOWED IUserMgrIsServiceAllowed;
 	PFUSERMGRISACTIVE IUserMgrIsActive;
 	PFUSERMGRREGISTERANONYMOUSLOGIN IUserMgrRegisterAnonymousLogin;
 	PFUSERMGRUNREGISTERANONYMOUSLOGIN IUserMgrUnregisterAnonymousLogin;
 	PFUSERMGRLOGINANONYMOUS IUserMgrLoginAnonymous;
 	PFUSERMGRRELOGINANONYMOUS IUserMgrReloginAnonymous;
 	PFUSERMGRISANONYMOUSLOGINALLOWED IUserMgrIsAnonymousLoginAllowed;
 	PFUSERMGRLOGIN IUserMgrLogin;
 	PFUSERMGRLOGOUT IUserMgrLogout;
 	PFUSERMGRRELOGIN IUserMgrRelogin;
 	PFUSERMGRISGROUPMEMBER IUserMgrIsGroupMember;
 	PFUSERMGRISADMINISTRATOR IUserMgrIsAdministrator;
 	PFUSERMGRHASACCESSRIGHTS IUserMgrHasAccessRights;
 	PFUSERMGRGETACCESSRIGHTS IUserMgrGetAccessRights;
 	PFUSERMGRHASUSERACCESSRIGHTS IUserMgrHasUserAccessRights;
 	PFUSERMGRGETUSERACCESSRIGHTS IUserMgrGetUserAccessRights;
 	PFUSERMGRADDINFOTOUSER IUserMgrAddInfoToUser;
 	PFUSERMGRREMOVEINFOFROMUSER IUserMgrRemoveInfoFromUser;
 	PFUSERMGRREMOVEINFOFROMALLUSERS IUserMgrRemoveInfoFromAllUsers;
 	PFUSERMGRGETINFOOFUSER IUserMgrGetInfoOfUser;
 	PFUSERMGRGETUSERNAME IUserMgrGetUserName;
 	PFUSERMGRGETGROUP IUserMgrGetGroup;
 	PFUSERMGRGROUPHASUSER IUserMgrGroupHasUser;
 	PFUSERMGRGROUPGETNAME IUserMgrGroupGetName;
 	PFUSERMGRADDUSER IUserMgrAddUser;
 	PFUSERMGRREMOVEUSER IUserMgrRemoveUser;
 	PFUSERMGRADDGROUP IUserMgrAddGroup;
 	PFUSERMGRREMOVEGROUP IUserMgrRemoveGroup;
 	PFUSERMGRGROUPADDUSER IUserMgrGroupAddUser;
 	PFUSERMGRGROUPREMOVEUSER IUserMgrGroupRemoveUser;
 	PFUSERMGRADDONLINEACCESSERROR IUserMgrAddOnlineAccessError;
 	PFUSERMGROBJECTADD IUserMgrObjectAdd;
 	PFUSERMGROBJECTREMOVE IUserMgrObjectRemove;
 	PFUSERMGROBJECTADDCHILD IUserMgrObjectAddChild;
 	PFUSERMGROBJECTSETUSEDRIGHTS IUserMgrObjectSetUsedRights;
 	PFUSERMGROBJECTGETUSEDRIGHTS IUserMgrObjectGetUsedRights;
 	PFUSERMGROBJECTOPEN IUserMgrObjectOpen;
 	PFUSERMGROBJECTOPEN2 IUserMgrObjectOpen2;
 	PFUSERMGROBJECTCLOSE IUserMgrObjectClose;
 	PFUSERMGROBJECTADD2 IUserMgrObjectAdd2;
 	PFUSERMGROBJECTADDGROUP IUserMgrObjectAddGroup;
 	PFUSERMGROBJECTREMOVEGROUP IUserMgrObjectRemoveGroup;
 	PFUSERMGROBJECTSETGROUPRIGHTS IUserMgrObjectSetGroupRights;
 	PFUSERMGROBJECTSETGROUPDENIEDRIGHTS IUserMgrObjectSetGroupDeniedRights;
 	PFUSERMGROBJECTCLEARRIGHTS IUserMgrObjectClearRights;
 	PFUSERMGROBJECTGETNAME IUserMgrObjectGetName;
 	PFUSERMGROBJECTGETFIRSTCHILD IUserMgrObjectGetFirstChild;
 	PFUSERMGROBJECTGETNEXTCHILD IUserMgrObjectGetNextChild;
 	PFUSERMGROBJECTGETFIRSTGROUP IUserMgrObjectGetFirstGroup;
 	PFUSERMGROBJECTGETNEXTGROUP IUserMgrObjectGetNextGroup;
 	PFUSERMGROBJECTGETGROUPRIGHTS IUserMgrObjectGetGroupRights;
 	PFUSERMGRRIGHTSSTORE IUserMgrRightsStore;
 	PFUSERMGRCHECKLEN IUserMgrCheckLen;
 	PFUSERMGRANNOUNCEOLDDATABASE IUserMgrAnnounceOldDatabase;
 	PFUSERMGRGROUPGETFIRST IUserMgrGroupGetFirst;
 	PFUSERMGRGROUPGETFIRSTUSER IUserMgrGroupGetFirstUser;
 	PFUSERMGRGROUPGETHANDLE IUserMgrGroupGetHandle;
 	PFUSERMGRGROUPGETNAME2 IUserMgrGroupGetName2;
 	PFUSERMGRGROUPGETNEXT IUserMgrGroupGetNext;
 	PFUSERMGRGROUPGETNEXTUSER IUserMgrGroupGetNextUser;
 	PFUSERMGROBJECTADDGROUP2 IUserMgrObjectAddGroup2;
 	PFUSERMGROBJECTCLEARRIGHTS2 IUserMgrObjectClearRights2;
 	PFUSERMGROBJECTGETFIRSTGROUP2 IUserMgrObjectGetFirstGroup2;
 	PFUSERMGROBJECTGETGROUPRIGHTS2 IUserMgrObjectGetGroupRights2;
 	PFUSERMGROBJECTGETNEXTGROUP2 IUserMgrObjectGetNextGroup2;
 	PFUSERMGROBJECTREMOVEGROUP2 IUserMgrObjectRemoveGroup2;
 	PFUSERMGROBJECTSETGROUPDENIEDRIGHTS2 IUserMgrObjectSetGroupDeniedRights2;
 	PFUSERMGROBJECTSETGROUPRIGHTS2 IUserMgrObjectSetGroupRights2;
 	PFUSERMGRUSERGETFIRST IUserMgrUserGetFirst;
 	PFUSERMGRUSERGETNEXT IUserMgrUserGetNext;
 	PFUSERMGRUSERGETPROPERTY IUserMgrUserGetProperty;
 	PFUSERMGRUSERSETCREDENTIALS IUserMgrUserSetCredentials;
 	PFUSERMGRUSERSETPROPERTY IUserMgrUserSetProperty;
 } ICmpUserMgr_C;

#ifdef CPLUSPLUS
class ICmpUserMgr : public IBase
{
	public:
		virtual RTS_RESULT CDECL IUserMgrRegisterUserDB(RTS_HANDLE hDatabase, RTS_UI32 type) =0;
		virtual RTS_RESULT CDECL IUserMgrUnregisterUserDB(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrRegisterUserDBConfig(RTS_HANDLE hDatabase, RTS_UI32 type) =0;
		virtual RTS_RESULT CDECL IUserMgrUnregisterUserDBConfig(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrRegisterUserGroupsDB(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrUnregisterUserGroupsDB(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrRegisterUserGroupsDBConfig(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrUnregisterUserGroupsDBConfig(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrRegisterUserObjectsDB(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrUnregisterUserObjectsDB(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrRegisterUserObjectsDBConfig(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrUnregisterUserObjectsDBConfig(RTS_HANDLE hDatabase) =0;
		virtual RTS_RESULT CDECL IUserMgrSelectAuthType(RTS_UI32 ui32ClientFlags, RTS_UI32 *pui32AuthType) =0;
		virtual RTS_RESULT CDECL IUserMgrHandleLoginService(RTS_UI32 SessionId, RTS_UI32 SessionAuthType, RTS_UI32 SessionFlags, BINTAGWRITER *pWriter, BINTAGREADER *pReader, RTS_HANDLE *phUser) =0;
		virtual RTS_RESULT CDECL IUserMgrHandleAsymmetricLoginProtocol(RTS_UI32 sessionId, BINTAGREADER *pReader, BINTAGWRITER *pWriter, RTS_BOOL bChangePassword, char *pUserName, RTS_UI32 userNameSize, char *pPassword, RTS_UI32 passwordSize, char *pNewPassword, RTS_UI32 newPasswordSize) =0;
		virtual RTS_RESULT CDECL IUserMgrIsServiceAllowed(RTS_UI32 channelID, HEADER_TAG_EXT *pHeader, RTS_HANDLE hUser, PROTOCOL_DATA_UNIT *pPduReply) =0;
		virtual RTS_BOOL CDECL IUserMgrIsActive(void) =0;
		virtual RTS_HANDLE CDECL IUserMgrRegisterAnonymousLogin(char *pszName, UserMgrAnonymousObjectRights *pObjectRights, RTS_UI32 numOfObjectRights, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrUnregisterAnonymousLogin(RTS_HANDLE hAnonymousLogin) =0;
		virtual RTS_HANDLE CDECL IUserMgrLoginAnonymous(RTS_HANDLE hAnonymousLogin, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrReloginAnonymous(RTS_HANDLE hUser, RTS_HANDLE hAnonymousLogin) =0;
		virtual RTS_BOOL CDECL IUserMgrIsAnonymousLoginAllowed(RTS_HANDLE hAnonymousLogin) =0;
		virtual RTS_HANDLE CDECL IUserMgrLogin(char *pszUser, char *pszPassword, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrLogout(RTS_HANDLE hUser) =0;
		virtual RTS_RESULT CDECL IUserMgrRelogin(RTS_HANDLE hUser, char *pszUser, char *pszPassword) =0;
		virtual RTS_RESULT CDECL IUserMgrIsGroupMember(char *pszGroup, RTS_HANDLE hUser) =0;
		virtual RTS_BOOL CDECL IUserMgrIsAdministrator(RTS_HANDLE hUser) =0;
		virtual RTS_RESULT CDECL IUserMgrHasAccessRights(char *pszObject, RTS_UI32 ulSessionId, RTS_UI32 ulRequestedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrGetAccessRights(char *pszObject, RTS_UI32 ulSessionId, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrHasUserAccessRights(char *pszObject, RTS_HANDLE hUser, RTS_UI32 ulRequestedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrGetUserAccessRights(char *pszObject, RTS_HANDLE hUser, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrAddInfoToUser(RTS_HANDLE hUser, RTS_SIZE key, RTS_TYPEDVALUE *pValue) =0;
		virtual RTS_RESULT CDECL IUserMgrRemoveInfoFromUser(RTS_HANDLE hUser, RTS_SIZE key) =0;
		virtual RTS_RESULT CDECL IUserMgrRemoveInfoFromAllUsers(RTS_SIZE key) =0;
		virtual RTS_RESULT CDECL IUserMgrGetInfoOfUser(RTS_HANDLE hUser, RTS_SIZE key, RTS_TYPEDVALUE *pValue) =0;
		virtual RTS_RESULT CDECL IUserMgrGetUserName(RTS_HANDLE hUser, char *pName, RTS_UI32 maxLen) =0;
		virtual RTS_HANDLE CDECL IUserMgrGetGroup(char *pszGroupName, RTS_RESULT *pResult) =0;
		virtual RTS_BOOL CDECL IUserMgrGroupHasUser(RTS_HANDLE hGroup, RTS_HANDLE hUser) =0;
		virtual RTS_RESULT CDECL IUserMgrGroupGetName(RTS_HANDLE hGroup, char *pName, RTS_UI32 maxLen) =0;
		virtual RTS_RESULT CDECL IUserMgrAddUser(RTS_HANDLE hUser, char *pszUserName, UserMgrCredentials *pCredentials) =0;
		virtual RTS_RESULT CDECL IUserMgrRemoveUser(RTS_HANDLE hUser, char *pszUserName) =0;
		virtual RTS_RESULT CDECL IUserMgrAddGroup(RTS_HANDLE hUser, char *pszGroupName) =0;
		virtual RTS_RESULT CDECL IUserMgrRemoveGroup(RTS_HANDLE hUser, char *pszGroupName) =0;
		virtual RTS_RESULT CDECL IUserMgrGroupAddUser(RTS_HANDLE hUser, char *pszGroupName, char *pszUserName) =0;
		virtual RTS_RESULT CDECL IUserMgrGroupRemoveUser(RTS_HANDLE hUser, char *pszGroupName, char *pszUserName) =0;
		virtual RTS_RESULT CDECL IUserMgrAddOnlineAccessError(BINTAGWRITER *pWriter, RTS_RESULT Result, char *pszObject, RTS_UI32 ulRequestedRights, RTS_UI32 ulSessionID) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectAdd(char *pszObject, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectRemove(RTS_HANDLE hObject) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectAddChild(RTS_HANDLE hFatherObject, char *pszObject, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectSetUsedRights(RTS_HANDLE hObject, RTS_UI32 ulUsedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectGetUsedRights(RTS_HANDLE hObject, RTS_UI32 *pulUsedRights) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectOpen(char *pszObject, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectOpen2(char *pszObject, RTS_HANDLE hUser, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectClose(RTS_HANDLE hObject) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectAdd2(char *pszObject, RTS_UI32 properties, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectAddGroup(RTS_HANDLE hObject, RTS_HANDLE hGroup) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectRemoveGroup(RTS_HANDLE hObject, RTS_HANDLE hGroup) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectSetGroupRights(RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulRights) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectSetGroupDeniedRights(RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulDeniedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectClearRights(RTS_HANDLE hObject) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectGetName(RTS_HANDLE hObject, char *pszObject, int *piMaxLen) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectGetFirstChild(RTS_HANDLE hFatherObject, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectGetNextChild(RTS_HANDLE hFatherObject, RTS_HANDLE hPrevChildObject, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectGetFirstGroup(RTS_HANDLE hObject, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectGetNextGroup(RTS_HANDLE hObject, RTS_HANDLE hPrevGroup, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectGetGroupRights(RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 *pulRights, RTS_UI32 *pulDeniedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrRightsStore(void) =0;
		virtual RTS_BOOL CDECL IUserMgrCheckLen(char *psz, RTS_UI32 len) =0;
		virtual RTS_RESULT CDECL IUserMgrAnnounceOldDatabase(RTS_UI32 info) =0;
		virtual RTS_HANDLE CDECL IUserMgrGroupGetFirst(RTS_HANDLE hUser, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrGroupGetFirstUser(RTS_HANDLE hUser, RTS_HANDLE hGroup, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen) =0;
		virtual RTS_HANDLE CDECL IUserMgrGroupGetHandle(RTS_HANDLE hUser, char* pszGroupName, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrGroupGetName2(RTS_HANDLE hUser, RTS_HANDLE hGroup, char* pszGroup, RTS_UI32 iMaxLen) =0;
		virtual RTS_HANDLE CDECL IUserMgrGroupGetNext(RTS_HANDLE hUser, RTS_HANDLE hPrevGroup, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrGroupGetNextUser(RTS_HANDLE hUser, RTS_HANDLE hGroup, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectAddGroup2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectClearRights2(RTS_HANDLE hUser, RTS_HANDLE hObject) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectGetFirstGroup2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectGetGroupRights2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32* pulRights, RTS_UI32* pulDeniedRights) =0;
		virtual RTS_HANDLE CDECL IUserMgrObjectGetNextGroup2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hPrevGroup, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectRemoveGroup2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectSetGroupDeniedRights2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulDeniedRights) =0;
		virtual RTS_RESULT CDECL IUserMgrObjectSetGroupRights2(RTS_HANDLE hUser, RTS_HANDLE hObject, RTS_HANDLE hGroup, RTS_UI32 ulRights) =0;
		virtual RTS_RESULT CDECL IUserMgrUserGetFirst(RTS_HANDLE hUser, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen, RTS_UI32* pUserProperty) =0;
		virtual RTS_RESULT CDECL IUserMgrUserGetNext(RTS_HANDLE hUser, RTS_HANDLE* phIterator, char* pszUsername, RTS_UI32 iMaxLen, RTS_UI32* pUserProperty) =0;
		virtual RTS_UI32 CDECL IUserMgrUserGetProperty(RTS_HANDLE hUser, char* pszUser, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IUserMgrUserSetCredentials(RTS_HANDLE hUser, char* pszUser, UserMgrCredentials* pCredentials) =0;
		virtual RTS_RESULT CDECL IUserMgrUserSetProperty(RTS_HANDLE hUser, char* pszUser, RTS_UI32 userProperty) =0;
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
