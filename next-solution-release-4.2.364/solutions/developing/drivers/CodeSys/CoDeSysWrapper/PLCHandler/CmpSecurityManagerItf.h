 /**
 * <interfacename>CmpSecurityManager</interfacename>
 * <description>
 *	The security manager interface is used to specify the level of all available security features in one central component.
 *	Additionally the X.509 certificates for servers in the runtime system can be specified here.
 *
 *	The actual available security features (for which the level can be specified) are:
 *		1. Programming system communication: encrypted / unencrypted
 *		2. IEC application code and bootproject: signed / unsigned, encrypted / unencrypted 
 *		3. WebServer: http / https
 *
 *	NOTE:
 *		This list will be extended in the future, if new security features are available and the corresponding security level must be specified!
 *
 *  SECURITY-NOTE:
 *		The security manager must be linked statically to the runtime system core to prevent, that it cannot be removed or replaced by a malware!
 *
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPSECURITYMANAGERITF_H_
#define _CMPSECURITYMANAGERITF_H_

#include "CmpStd.h"

 

 




#include "CmpStd.h"
#include "CmpX509CertItf.h"
#include "CmpSettingsItf.h"

#ifdef CMPSECURITYMANAGER_EXTERNAL
	#error CmpSecurityManager must not be overloadable so defining CMPSECURITYMANAGER_EXTERNAL is forbidden because of security requirements!
#endif

/**
 * <category>Compiler switch</category>
 * <description>
 *	Compiler switches to enable/disable single features in the component.
 * </description>
 * <element name="CMPSECURITYMANAGER_SET_SECURITY_LEVEL_MAXIMUM">Set security level to maximum for all available security features!</element>
 */

 /**
 * <category>Static defines</category>
 * <description>Number of static security settings to store</description>
 */
#ifndef RTS_SECMAN_NUMOF_STATIC_SETTINGS
	#define RTS_SECMAN_NUMOF_STATIC_SETTINGS			10
#endif


/**
 * <category>Event parameter</category>
 * <element name="reserved" type="IN">Not used yet.</element>
 */
typedef struct
{
	RTS_UI8 reserved;
} EVTPARAM_CmpSecMan_ConfigChanged;
#define EVTPARAMID_CmpSecMan_ConfigChanged				0x0001
#define EVTVERSION_CmpSecMan_ConfigChanged				0x0001

/**
 * <category>Events</category>
 * <description>Event is sent before start of the specified application</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpSecMan_ConfigChanged</param>
 */
#define EVT_CmpSecMan_ConfigurationChanged				MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>SecuritySettingFlags</category>
 * <description>
 *	Flags/options for a security setting
 * </description>
 */
#define CMPSECMAN_FLAGS_NONE							0
#define CMPSECMAN_FLAGS_SELECTABLE						CMPSETTINGS_FLAGS_SELECTABLE
#define CMPSECMAN_FLAGS_EDITABLE						CMPSETTINGS_FLAGS_EDITABLE
#define CMPSECMAN_FLAGS_SETTING_FROM_FILE				0x00000010


/**
 * <description>
 *	Entry of one security feature to be managed by the CmpSecurityManager.
 * </description>
 * <element name="pszKey" type="IN">Name of the security key/feature</element>
 * <element name="pszDescription" type="IN">Description of the security entry/feature</element>
 * <element name="pszDescription" type="IN">Description of the security entry/feature</element>
 * <element name="cmpId" type="IN">ComponentID of the component that provides security features</element>
 * <element name="flags" type="IN">Flags of the entry. See "SecuritySettingFlags" for details.</element>
 */
typedef struct _SecuritySetting
{
	const char *pszKey;
	const char *pszDescription;
	CMPID cmpId;
	RTS_UI32 flags;
} SecuritySetting;

/**
 * <category>SecuritySelectableFlags</category>
 * <description>
 *	Flags/options for a security selectable setting
 * </description>
 */
#define CMPSECMAN_SFLAGS_NONE					CMPSECMAN_FLAGS_NONE

/**
 * <description>
 *	This structure stores a single entry of a selectable setting. Each 
 *  possible selection is a single struture element when registering the setting.
 *  The registration can be done using SecManRegisterSelectableSetting function. The 
 *  selected setting can be retrieved by using SecManGetSelectableSetting.
 * </description>
 * <element name="secID" type="IN">ID of the selectable setting. Must be unique within this set of selectable settings!</element>
 * <element name="flags" type="IN">Flags of the entry. See "SecuritySettingFlags" for details.</element>
 * <element name="pszValue" type="IN">Value of the security setting</element>
 * <element name="pszDescription" type="IN">Description of the security setting</element>
 */
typedef struct _SecuritySelectableSetting
{
	RTS_I32 secID;
	RTS_UI32 flags;
	char *pszValue;
	char *pszDescription;
} SecuritySelectableSetting;

/**
 * <description>
 *	Union to store an editable integer, string or wstring value. Value can be specified in the security manager.
 * </description>
 * <element name="pStringSetting" type="IN">String value UTF8</element>
 * <element name="pWStringSetting" type="IN">String value UTF16</element>
 * <element name="intSetting" type="IN">Integer value</element>
 */
typedef union _SecuritySettingValue
{
    RTS_CSTRING *pStringSetting;
	RTS_WCHAR *pWStringSetting;
    RTS_I32 intSetting;
} SecuritySettingValue;

/**
 * <category>SecurityEditableFlags</category>
 * <description>
 *	Flags/options for a security editable setting
 * </description>
 */
#define CMPSECMAN_EFLAGS_EDITABLE_STRING			CMPSETTINGS_EFLAGS_EDITABLE_STRING
#define CMPSECMAN_EFLAGS_EDITABLE_INT				CMPSETTINGS_EFLAGS_EDITABLE_INT

/**
 * <description>
 *	This structure stores a editable setting. The setting can be registerd using
 *  SecManRegisterEditableSetting function. The default value given when registring
 *  will be returned until the setting has been changed by the someone. Then the
 *  changed value will be returned. To indicate if a string or an intager is stored
 *  use either CMPSECMAN_FLAGS_STRINGSETTING or CMPSECMAN_FLAGS_INTSETTING. The
 *  corresponding field in the secValue union is then used.
 *  Use the function SecManGetEditableSetting to retrieve the valued stored.
 * </description>
 * <element name="flags" type="IN">Flags of the entry. See "SecurityEditableFlags" for details.</element>
 * <element name="secValue" type="IN">Editable value</element>
 */
typedef struct _SecurityEditableSetting
{
	RTS_UI32 flags;
	SecuritySettingValue secValue;
} SecurityEditableSetting;


/**
 * <category>SecuritySettings</category>
 * <description>Security setting to specify, if a selfsigned certificate should be created during startup if not available</description>
 */
#define SECURITY_KEY_STRING_CREATE_SELFSIGNED_CERT			CMPSETTINGS_CATEGORY_SECURITY ".CreateSelfSignedCert"
#define SECURITY_KEY_STRING_CREATE_SELFSIGNED_CERT_DESC		"Specifies if self signed certificate should automatically be created during startup!"

#define SECURITY_VALUE_CREATE_SELFSIGNED_CERT_YES			"YES"
#define SECURITY_VALUE_CREATE_SELFSIGNED_CERT_NO			"NO"

#ifndef SECURITY_VALUE_CREATE_SELFSIGNED_CERT_DEFAULT
	#define SECURITY_VALUE_CREATE_SELFSIGNED_CERT_DEFAULT	SECURITY_VALUE_CREATE_SELFSIGNED_CERT_NO
#endif

/**
 * <description>SecurityIDs</description>
 * <element name="CREATE_SELFSIGNED_CERT_ENABLE" type="IN">Create a selfsigned certificate at startup</element>
 * <element name="CREATE_SELFSIGNED_CERT_DISABLE" type="IN">Don't create a selfsigned certificate at startup</element>
 */
typedef enum
{
	CREATE_SELFSIGNED_CERT_YES,
	CREATE_SELFSIGNED_CERT_NO
} SECURITY_CREATESELFSIGNEDCERT_IDS;

#define SECURITY_VALUE_CREATE_SELFSIGNED_CERT_LIST			{\
																{(RTS_I32)CREATE_SELFSIGNED_CERT_YES, CMPSECMAN_SFLAGS_NONE, SECURITY_VALUE_CREATE_SELFSIGNED_CERT_YES, "Create a selfsigned cert"},\
																{(RTS_I32)CREATE_SELFSIGNED_CERT_NO, CMPSECMAN_SFLAGS_NONE, SECURITY_VALUE_CREATE_SELFSIGNED_CERT_NO, "Don't create a selfsigned cert"}\
															}


/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	With this setting you can configure, if all certificates and parameter should be created automatically at startup, if a registered certificate 
 *	is not available.
 * </description>
 */
#define SECURITYMANAGER_KEY_INT_CREATE_ALL_ON_STARTUP					"CreateAllOnStartup"
#ifndef SECURITYMANAGER_VALUE_INT_CREATE_ALL_ON_STARTUP_DEFAULT
	#define SECURITYMANAGER_VALUE_INT_CREATE_ALL_ON_STARTUP_DEFAULT		0
#endif


/**
 * <category>Online services</category>
 * <description>
 *	Online service to read all security setting.
 * </description>
 * <servicegroup name="SG_SECURITY_MANAGER">0x0023</servicegroup>
 * <service name="SRV_SECMAN_READ_SETTINGS">
 *	<Request>
 *		<No further tag>
 *	</Request>
 *	<Response>
 *		<tag[] name="TAGNEST_SECURITY_ENTRY" required="mandatory">Complex tag</tag>
 *			<tag name="TAGNEST_SECURITY_ENTRY_INFO" required="mandatory">Complex tag</tag>
 *				<tag name="TAG_SECURITY_COMPONENT_ID" required="mandatory">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SECURITY_ID" required="mandatory">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SECURITY_NAME" required="mandatory">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SECURITY_DESCRIPTION" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SECURITY_FLAGS" required="mandatory">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_ID_SELECTED" required="optional">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SECURITY_NUM_OF_SETTINGS" required="mandatory">[RTS_UI32]: TODO</tag>
 *
 *			<alternate tag[] name="TAGNEST_SECURITY_SETTING_SELECTABLE" required="optional">Complex tag</tag>
 *				<tag name="TAG_SECURITY_SETTING_ID" required="mandatory">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_FLAGS" required="mandatory">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_NAME" required="mandatory">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_DESCRIPTION" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *
 *			<alternate tag name="TAGNEST_SECURITY_SETTING_EDITABLE" required="optional">Complex tag</tag>
 *				<tag name="TAG_SECURITY_SETTING_FLAGS" required="mandatory">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_NAME" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_DESCRIPTION" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_VALUE_INT" required="optional">[RTS_I32]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_DEFAULTVALUE_INT" required="optional">[RTS_I32]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_VALUE_STRING" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_DEFAULTVALUE_STRING" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *
 *		<tag name="TAG_ERROR" required="mandatory">[RTS_I16]: Result code of the complete operation</tag>
 *	</Response>
 * </service>
 */
#define SRV_SECMAN_READ_SETTINGS						0x01

#define TAGNEST_SECURITY_ENTRY							0x81
#define TAGNEST_SECURITY_ENTRY_INFO						0x82
#define TAG_RESULT										0x01
#define TAG_SECURITY_COMPONENT_ID						0x10
#define TAG_SECURITY_ID									0x11
#define TAG_SECURITY_NAME								0x12
#define TAG_SECURITY_DESCRIPTION						0x13
#define TAG_SECURITY_FLAGS								0x14
#define TAG_SECURITY_SETTING_ID_SELECTED				0x15
#define TAG_SECURITY_NUM_OF_SETTINGS					0x16

#define TAGNEST_SECURITY_SETTING_SELECTABLE				0x83
#define TAG_SECURITY_SETTING_ID							0x20
#define TAG_SECURITY_SETTING_FLAGS						0x21
#define TAG_SECURITY_SETTING_NAME						0x22
#define TAG_SECURITY_SETTING_DESCRIPTION				0x23

#define TAGNEST_SECURITY_SETTING_EDITABLE				0x84
#define TAG_SECURITY_SETTING_VALUE_INT					0x30
#define TAG_SECURITY_SETTING_DEFAULTVALUE_INT			0x31
#define TAG_SECURITY_SETTING_VALUE_STRING				0x32
#define TAG_SECURITY_SETTING_DEFAULTVALUE_STRING		0x33

/**
 * <category>Online services</category>
 * <description>
 *	Online service to write changed security settings.
 * </description>
 * <servicegroup name="SG_SECURITY_MANAGER">0x0023</servicegroup>
 * <service name="SRV_SECMAN_WRITE_SETTINGS">
 *	<Request>
 *		<No further tag>
 *	</Request>
 *	<Response>
 *		<tag[] name="TAGNEST_SECURITY_ENTRY" required="mandatory">Complex tag</tag>
 *			<tag name="TAGNEST_SECURITY_ENTRY_INFO" required="mandatory">Complex tag</tag>
 *				<tag name="TAG_SECURITY_COMPONENT_ID" required="mandatory">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SECURITY_ID" required="mandatory">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_ID_SELECTED" required="optional">[RTS_UI32]: Is only sent, if it is a selectable setting!</tag>
 *
 *			<tag name="TAGNEST_SECMAN_SETTING_EDITABLE" required="optional">Complex tag</tag>
 *				<tag name="TAG_SECURITY_SETTING_VALUE_INT" required="optional">[RTS_I32]: TODO</tag>
 *				<tag name="TAG_SECURITY_SETTING_VALUE_STRING" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *
 *			<tag name="TAG_ERROR" required="mandatory">[RTS_I16]: Result code of each write operation</tag>
 *	</Response>
 * </service>
 */
#define SRV_SECMAN_WRITE_SETTINGS							0x02


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Function to register one security setting of a component.</description>
 * <param name="pEntry" type="IN">Pointer to description structure of the security setting</param>
 * <param name="pSettings" type="IN">Pointer to the security settings list</param>
 * <param name="ui32Settings" type="IN">Number of security settings in the list</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Settings could be registered</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOTINITIALIZED">CmpSecurityManager is not yet initialized</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_MEMORY">No memory to register security settings</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_DUPLICATE">The setting was registered already. The handle of the existing is returned.</errorcode>
 * <result>Handle to the registered security settings</result>
 */
RTS_HANDLE CDECL SecManRegisterSelectableSetting(CMPID cmpId, const char *pszKey, const char *pszDescription, const char *pszDefaultSecName, const SecuritySelectableSetting *pList, RTS_UI32 ui32Settings, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSECMANREGISTERSELECTABLESETTING) (CMPID cmpId, const char *pszKey, const char *pszDescription, const char *pszDefaultSecName, const SecuritySelectableSetting *pList, RTS_UI32 ui32Settings, RTS_RESULT *pResult);
#if defined(CMPSECURITYMANAGER_NOTIMPLEMENTED) || defined(SECMANREGISTERSELECTABLESETTING_NOTIMPLEMENTED)
	#define USE_SecManRegisterSelectableSetting
	#define EXT_SecManRegisterSelectableSetting
	#define GET_SecManRegisterSelectableSetting(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecManRegisterSelectableSetting(p0,p1,p2,p3,p4,p5,p6)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SecManRegisterSelectableSetting  FALSE
	#define EXP_SecManRegisterSelectableSetting  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecManRegisterSelectableSetting
	#define EXT_SecManRegisterSelectableSetting
	#define GET_SecManRegisterSelectableSetting(fl)  CAL_CMGETAPI( "SecManRegisterSelectableSetting" ) 
	#define CAL_SecManRegisterSelectableSetting  SecManRegisterSelectableSetting
	#define CHK_SecManRegisterSelectableSetting  TRUE
	#define EXP_SecManRegisterSelectableSetting  CAL_CMEXPAPI( "SecManRegisterSelectableSetting" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURITYMANAGER_EXTERNAL)
	#define USE_SecManRegisterSelectableSetting
	#define EXT_SecManRegisterSelectableSetting
	#define GET_SecManRegisterSelectableSetting(fl)  CAL_CMGETAPI( "SecManRegisterSelectableSetting" ) 
	#define CAL_SecManRegisterSelectableSetting  SecManRegisterSelectableSetting
	#define CHK_SecManRegisterSelectableSetting  TRUE
	#define EXP_SecManRegisterSelectableSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManRegisterSelectableSetting", (RTS_UINTPTR)SecManRegisterSelectableSetting, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecurityManagerSecManRegisterSelectableSetting
	#define EXT_CmpSecurityManagerSecManRegisterSelectableSetting
	#define GET_CmpSecurityManagerSecManRegisterSelectableSetting  ERR_OK
	#define CAL_CmpSecurityManagerSecManRegisterSelectableSetting pICmpSecurityManager->ISecManRegisterSelectableSetting
	#define CHK_CmpSecurityManagerSecManRegisterSelectableSetting (pICmpSecurityManager != NULL)
	#define EXP_CmpSecurityManagerSecManRegisterSelectableSetting  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecManRegisterSelectableSetting
	#define EXT_SecManRegisterSelectableSetting
	#define GET_SecManRegisterSelectableSetting(fl)  CAL_CMGETAPI( "SecManRegisterSelectableSetting" ) 
	#define CAL_SecManRegisterSelectableSetting pICmpSecurityManager->ISecManRegisterSelectableSetting
	#define CHK_SecManRegisterSelectableSetting (pICmpSecurityManager != NULL)
	#define EXP_SecManRegisterSelectableSetting  CAL_CMEXPAPI( "SecManRegisterSelectableSetting" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecManRegisterSelectableSetting  PFSECMANREGISTERSELECTABLESETTING pfSecManRegisterSelectableSetting;
	#define EXT_SecManRegisterSelectableSetting  extern PFSECMANREGISTERSELECTABLESETTING pfSecManRegisterSelectableSetting;
	#define GET_SecManRegisterSelectableSetting(fl)  s_pfCMGetAPI2( "SecManRegisterSelectableSetting", (RTS_VOID_FCTPTR *)&pfSecManRegisterSelectableSetting, (fl), 0, 0)
	#define CAL_SecManRegisterSelectableSetting  pfSecManRegisterSelectableSetting
	#define CHK_SecManRegisterSelectableSetting  (pfSecManRegisterSelectableSetting != NULL)
	#define EXP_SecManRegisterSelectableSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManRegisterSelectableSetting", (RTS_UINTPTR)SecManRegisterSelectableSetting, 0, 0) 
#endif




/**
 * <description>Function to register one security setting of a component.</description>
 * <param name="pEntry" type="IN">Pointer to description structure of the security setting</param>
 * <param name="pSetting" type="IN">Pointer to the editable security settings</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Settings could be registered</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOTINITIALIZED">CmpSecurityManager is not yet initialized</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_MEMORY">No memory to register security settings</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_DUPLICATE">The setting was registered already. The handle of the existing is returned.</errorcode>
 * <result>Handle to the registered security settings</result>
 */
RTS_HANDLE CDECL SecManRegisterEditableSetting(CMPID cmpId, const char *pszKey, const char *pszDescription, SecurityEditableSetting setting, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSECMANREGISTEREDITABLESETTING) (CMPID cmpId, const char *pszKey, const char *pszDescription, SecurityEditableSetting setting, RTS_RESULT *pResult);
#if defined(CMPSECURITYMANAGER_NOTIMPLEMENTED) || defined(SECMANREGISTEREDITABLESETTING_NOTIMPLEMENTED)
	#define USE_SecManRegisterEditableSetting
	#define EXT_SecManRegisterEditableSetting
	#define GET_SecManRegisterEditableSetting(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecManRegisterEditableSetting(p0,p1,p2,p3,p4)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SecManRegisterEditableSetting  FALSE
	#define EXP_SecManRegisterEditableSetting  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecManRegisterEditableSetting
	#define EXT_SecManRegisterEditableSetting
	#define GET_SecManRegisterEditableSetting(fl)  CAL_CMGETAPI( "SecManRegisterEditableSetting" ) 
	#define CAL_SecManRegisterEditableSetting  SecManRegisterEditableSetting
	#define CHK_SecManRegisterEditableSetting  TRUE
	#define EXP_SecManRegisterEditableSetting  CAL_CMEXPAPI( "SecManRegisterEditableSetting" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURITYMANAGER_EXTERNAL)
	#define USE_SecManRegisterEditableSetting
	#define EXT_SecManRegisterEditableSetting
	#define GET_SecManRegisterEditableSetting(fl)  CAL_CMGETAPI( "SecManRegisterEditableSetting" ) 
	#define CAL_SecManRegisterEditableSetting  SecManRegisterEditableSetting
	#define CHK_SecManRegisterEditableSetting  TRUE
	#define EXP_SecManRegisterEditableSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManRegisterEditableSetting", (RTS_UINTPTR)SecManRegisterEditableSetting, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecurityManagerSecManRegisterEditableSetting
	#define EXT_CmpSecurityManagerSecManRegisterEditableSetting
	#define GET_CmpSecurityManagerSecManRegisterEditableSetting  ERR_OK
	#define CAL_CmpSecurityManagerSecManRegisterEditableSetting pICmpSecurityManager->ISecManRegisterEditableSetting
	#define CHK_CmpSecurityManagerSecManRegisterEditableSetting (pICmpSecurityManager != NULL)
	#define EXP_CmpSecurityManagerSecManRegisterEditableSetting  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecManRegisterEditableSetting
	#define EXT_SecManRegisterEditableSetting
	#define GET_SecManRegisterEditableSetting(fl)  CAL_CMGETAPI( "SecManRegisterEditableSetting" ) 
	#define CAL_SecManRegisterEditableSetting pICmpSecurityManager->ISecManRegisterEditableSetting
	#define CHK_SecManRegisterEditableSetting (pICmpSecurityManager != NULL)
	#define EXP_SecManRegisterEditableSetting  CAL_CMEXPAPI( "SecManRegisterEditableSetting" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecManRegisterEditableSetting  PFSECMANREGISTEREDITABLESETTING pfSecManRegisterEditableSetting;
	#define EXT_SecManRegisterEditableSetting  extern PFSECMANREGISTEREDITABLESETTING pfSecManRegisterEditableSetting;
	#define GET_SecManRegisterEditableSetting(fl)  s_pfCMGetAPI2( "SecManRegisterEditableSetting", (RTS_VOID_FCTPTR *)&pfSecManRegisterEditableSetting, (fl), 0, 0)
	#define CAL_SecManRegisterEditableSetting  pfSecManRegisterEditableSetting
	#define CHK_SecManRegisterEditableSetting  (pfSecManRegisterEditableSetting != NULL)
	#define EXP_SecManRegisterEditableSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManRegisterEditableSetting", (RTS_UINTPTR)SecManRegisterEditableSetting, 0, 0) 
#endif




/**
 * <description>Unregister security setting</description>
 * <param name="hSecuritySetting" type="IN">Handle to the security settings returned by SecManRegisterSelectableSetting() or SecManFindSettings() or SecManRegisterEditableSetting()</param>
 * <result>Result of the operation</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Unregister succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">hSecuritySetting invalid</errorcode>
 */
RTS_RESULT CDECL SecManUnregisterSetting(RTS_HANDLE hSecuritySetting);
typedef RTS_RESULT (CDECL * PFSECMANUNREGISTERSETTING) (RTS_HANDLE hSecuritySetting);
#if defined(CMPSECURITYMANAGER_NOTIMPLEMENTED) || defined(SECMANUNREGISTERSETTING_NOTIMPLEMENTED)
	#define USE_SecManUnregisterSetting
	#define EXT_SecManUnregisterSetting
	#define GET_SecManUnregisterSetting(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecManUnregisterSetting(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecManUnregisterSetting  FALSE
	#define EXP_SecManUnregisterSetting  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecManUnregisterSetting
	#define EXT_SecManUnregisterSetting
	#define GET_SecManUnregisterSetting(fl)  CAL_CMGETAPI( "SecManUnregisterSetting" ) 
	#define CAL_SecManUnregisterSetting  SecManUnregisterSetting
	#define CHK_SecManUnregisterSetting  TRUE
	#define EXP_SecManUnregisterSetting  CAL_CMEXPAPI( "SecManUnregisterSetting" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURITYMANAGER_EXTERNAL)
	#define USE_SecManUnregisterSetting
	#define EXT_SecManUnregisterSetting
	#define GET_SecManUnregisterSetting(fl)  CAL_CMGETAPI( "SecManUnregisterSetting" ) 
	#define CAL_SecManUnregisterSetting  SecManUnregisterSetting
	#define CHK_SecManUnregisterSetting  TRUE
	#define EXP_SecManUnregisterSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManUnregisterSetting", (RTS_UINTPTR)SecManUnregisterSetting, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecurityManagerSecManUnregisterSetting
	#define EXT_CmpSecurityManagerSecManUnregisterSetting
	#define GET_CmpSecurityManagerSecManUnregisterSetting  ERR_OK
	#define CAL_CmpSecurityManagerSecManUnregisterSetting pICmpSecurityManager->ISecManUnregisterSetting
	#define CHK_CmpSecurityManagerSecManUnregisterSetting (pICmpSecurityManager != NULL)
	#define EXP_CmpSecurityManagerSecManUnregisterSetting  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecManUnregisterSetting
	#define EXT_SecManUnregisterSetting
	#define GET_SecManUnregisterSetting(fl)  CAL_CMGETAPI( "SecManUnregisterSetting" ) 
	#define CAL_SecManUnregisterSetting pICmpSecurityManager->ISecManUnregisterSetting
	#define CHK_SecManUnregisterSetting (pICmpSecurityManager != NULL)
	#define EXP_SecManUnregisterSetting  CAL_CMEXPAPI( "SecManUnregisterSetting" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecManUnregisterSetting  PFSECMANUNREGISTERSETTING pfSecManUnregisterSetting;
	#define EXT_SecManUnregisterSetting  extern PFSECMANUNREGISTERSETTING pfSecManUnregisterSetting;
	#define GET_SecManUnregisterSetting(fl)  s_pfCMGetAPI2( "SecManUnregisterSetting", (RTS_VOID_FCTPTR *)&pfSecManUnregisterSetting, (fl), 0, 0)
	#define CAL_SecManUnregisterSetting  pfSecManUnregisterSetting
	#define CHK_SecManUnregisterSetting  (pfSecManUnregisterSetting != NULL)
	#define EXP_SecManUnregisterSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManUnregisterSetting", (RTS_UINTPTR)SecManUnregisterSetting, 0, 0) 
#endif




/**
 * <description>Find settings specified by componentID and usageID</description>
 * <param name="cmpId" type="IN">ComponentID of the server that provides security features</param>
 * <param name="pszKey" type="IN">Name of the setting</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Settings could be found</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOTINITIALIZED">CmpSecurityManager is not yet initialized</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_OBJECT">No settings registered</errorcode>
 * <result>Handle to the registered security settings</result>
 */
RTS_HANDLE CDECL SecManFindSetting(CMPID cmpId, const char *pszKey, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSECMANFINDSETTING) (CMPID cmpId, const char *pszKey, RTS_RESULT *pResult);
#if defined(CMPSECURITYMANAGER_NOTIMPLEMENTED) || defined(SECMANFINDSETTING_NOTIMPLEMENTED)
	#define USE_SecManFindSetting
	#define EXT_SecManFindSetting
	#define GET_SecManFindSetting(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecManFindSetting(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SecManFindSetting  FALSE
	#define EXP_SecManFindSetting  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecManFindSetting
	#define EXT_SecManFindSetting
	#define GET_SecManFindSetting(fl)  CAL_CMGETAPI( "SecManFindSetting" ) 
	#define CAL_SecManFindSetting  SecManFindSetting
	#define CHK_SecManFindSetting  TRUE
	#define EXP_SecManFindSetting  CAL_CMEXPAPI( "SecManFindSetting" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURITYMANAGER_EXTERNAL)
	#define USE_SecManFindSetting
	#define EXT_SecManFindSetting
	#define GET_SecManFindSetting(fl)  CAL_CMGETAPI( "SecManFindSetting" ) 
	#define CAL_SecManFindSetting  SecManFindSetting
	#define CHK_SecManFindSetting  TRUE
	#define EXP_SecManFindSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManFindSetting", (RTS_UINTPTR)SecManFindSetting, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecurityManagerSecManFindSetting
	#define EXT_CmpSecurityManagerSecManFindSetting
	#define GET_CmpSecurityManagerSecManFindSetting  ERR_OK
	#define CAL_CmpSecurityManagerSecManFindSetting pICmpSecurityManager->ISecManFindSetting
	#define CHK_CmpSecurityManagerSecManFindSetting (pICmpSecurityManager != NULL)
	#define EXP_CmpSecurityManagerSecManFindSetting  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecManFindSetting
	#define EXT_SecManFindSetting
	#define GET_SecManFindSetting(fl)  CAL_CMGETAPI( "SecManFindSetting" ) 
	#define CAL_SecManFindSetting pICmpSecurityManager->ISecManFindSetting
	#define CHK_SecManFindSetting (pICmpSecurityManager != NULL)
	#define EXP_SecManFindSetting  CAL_CMEXPAPI( "SecManFindSetting" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecManFindSetting  PFSECMANFINDSETTING pfSecManFindSetting;
	#define EXT_SecManFindSetting  extern PFSECMANFINDSETTING pfSecManFindSetting;
	#define GET_SecManFindSetting(fl)  s_pfCMGetAPI2( "SecManFindSetting", (RTS_VOID_FCTPTR *)&pfSecManFindSetting, (fl), 0, 0)
	#define CAL_SecManFindSetting  pfSecManFindSetting
	#define CHK_SecManFindSetting  (pfSecManFindSetting != NULL)
	#define EXP_SecManFindSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManFindSetting", (RTS_UINTPTR)SecManFindSetting, 0, 0) 
#endif




/**
 * <description>Get the selected setting because of the security policy of the target!</description>
 * <param name="hSecuritySetting" type="IN">Handle to the security settings returned by SecManRegisterSelectableSetting() or SecManFindSettings()</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">One setting is returned</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">hSecuritySetting is invalid</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_OBJECT">No setting selected</errorcode>
 * <result>Pointer to the security setting which is selected on the target</result>
 */
const SecuritySelectableSetting * CDECL SecManGetSelectableSetting(RTS_HANDLE hSecuritySetting, RTS_RESULT *pResult);
typedef const SecuritySelectableSetting * (CDECL * PFSECMANGETSELECTABLESETTING) (RTS_HANDLE hSecuritySetting, RTS_RESULT *pResult);
#if defined(CMPSECURITYMANAGER_NOTIMPLEMENTED) || defined(SECMANGETSELECTABLESETTING_NOTIMPLEMENTED)
	#define USE_SecManGetSelectableSetting
	#define EXT_SecManGetSelectableSetting
	#define GET_SecManGetSelectableSetting(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecManGetSelectableSetting(p0,p1)  (const SecuritySelectableSetting *)ERR_NOTIMPLEMENTED
	#define CHK_SecManGetSelectableSetting  FALSE
	#define EXP_SecManGetSelectableSetting  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecManGetSelectableSetting
	#define EXT_SecManGetSelectableSetting
	#define GET_SecManGetSelectableSetting(fl)  CAL_CMGETAPI( "SecManGetSelectableSetting" ) 
	#define CAL_SecManGetSelectableSetting  SecManGetSelectableSetting
	#define CHK_SecManGetSelectableSetting  TRUE
	#define EXP_SecManGetSelectableSetting  CAL_CMEXPAPI( "SecManGetSelectableSetting" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURITYMANAGER_EXTERNAL)
	#define USE_SecManGetSelectableSetting
	#define EXT_SecManGetSelectableSetting
	#define GET_SecManGetSelectableSetting(fl)  CAL_CMGETAPI( "SecManGetSelectableSetting" ) 
	#define CAL_SecManGetSelectableSetting  SecManGetSelectableSetting
	#define CHK_SecManGetSelectableSetting  TRUE
	#define EXP_SecManGetSelectableSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManGetSelectableSetting", (RTS_UINTPTR)SecManGetSelectableSetting, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecurityManagerSecManGetSelectableSetting
	#define EXT_CmpSecurityManagerSecManGetSelectableSetting
	#define GET_CmpSecurityManagerSecManGetSelectableSetting  ERR_OK
	#define CAL_CmpSecurityManagerSecManGetSelectableSetting pICmpSecurityManager->ISecManGetSelectableSetting
	#define CHK_CmpSecurityManagerSecManGetSelectableSetting (pICmpSecurityManager != NULL)
	#define EXP_CmpSecurityManagerSecManGetSelectableSetting  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecManGetSelectableSetting
	#define EXT_SecManGetSelectableSetting
	#define GET_SecManGetSelectableSetting(fl)  CAL_CMGETAPI( "SecManGetSelectableSetting" ) 
	#define CAL_SecManGetSelectableSetting pICmpSecurityManager->ISecManGetSelectableSetting
	#define CHK_SecManGetSelectableSetting (pICmpSecurityManager != NULL)
	#define EXP_SecManGetSelectableSetting  CAL_CMEXPAPI( "SecManGetSelectableSetting" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecManGetSelectableSetting  PFSECMANGETSELECTABLESETTING pfSecManGetSelectableSetting;
	#define EXT_SecManGetSelectableSetting  extern PFSECMANGETSELECTABLESETTING pfSecManGetSelectableSetting;
	#define GET_SecManGetSelectableSetting(fl)  s_pfCMGetAPI2( "SecManGetSelectableSetting", (RTS_VOID_FCTPTR *)&pfSecManGetSelectableSetting, (fl), 0, 0)
	#define CAL_SecManGetSelectableSetting  pfSecManGetSelectableSetting
	#define CHK_SecManGetSelectableSetting  (pfSecManGetSelectableSetting != NULL)
	#define EXP_SecManGetSelectableSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManGetSelectableSetting", (RTS_UINTPTR)SecManGetSelectableSetting, 0, 0) 
#endif




/**
 * <description>Get the edited value setting because of the security policy of the target!</description>
 * <param name="hSecuritySetting" type="IN">Handle to the security settings returned by SecManRegisterSelectableSetting() or SecManFindSettings()</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">One setting is returned</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">hSecuritySetting is invalid</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_OBJECT">No setting selected</errorcode>
 * <result>Pointer to the security setting which is selected on the target</result>
 */
SecurityEditableSetting * CDECL SecManGetEditableSetting(RTS_HANDLE hSecuritySetting, RTS_RESULT *pResult);
typedef SecurityEditableSetting * (CDECL * PFSECMANGETEDITABLESETTING) (RTS_HANDLE hSecuritySetting, RTS_RESULT *pResult);
#if defined(CMPSECURITYMANAGER_NOTIMPLEMENTED) || defined(SECMANGETEDITABLESETTING_NOTIMPLEMENTED)
	#define USE_SecManGetEditableSetting
	#define EXT_SecManGetEditableSetting
	#define GET_SecManGetEditableSetting(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecManGetEditableSetting(p0,p1)  (SecurityEditableSetting *)ERR_NOTIMPLEMENTED
	#define CHK_SecManGetEditableSetting  FALSE
	#define EXP_SecManGetEditableSetting  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecManGetEditableSetting
	#define EXT_SecManGetEditableSetting
	#define GET_SecManGetEditableSetting(fl)  CAL_CMGETAPI( "SecManGetEditableSetting" ) 
	#define CAL_SecManGetEditableSetting  SecManGetEditableSetting
	#define CHK_SecManGetEditableSetting  TRUE
	#define EXP_SecManGetEditableSetting  CAL_CMEXPAPI( "SecManGetEditableSetting" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURITYMANAGER_EXTERNAL)
	#define USE_SecManGetEditableSetting
	#define EXT_SecManGetEditableSetting
	#define GET_SecManGetEditableSetting(fl)  CAL_CMGETAPI( "SecManGetEditableSetting" ) 
	#define CAL_SecManGetEditableSetting  SecManGetEditableSetting
	#define CHK_SecManGetEditableSetting  TRUE
	#define EXP_SecManGetEditableSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManGetEditableSetting", (RTS_UINTPTR)SecManGetEditableSetting, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecurityManagerSecManGetEditableSetting
	#define EXT_CmpSecurityManagerSecManGetEditableSetting
	#define GET_CmpSecurityManagerSecManGetEditableSetting  ERR_OK
	#define CAL_CmpSecurityManagerSecManGetEditableSetting pICmpSecurityManager->ISecManGetEditableSetting
	#define CHK_CmpSecurityManagerSecManGetEditableSetting (pICmpSecurityManager != NULL)
	#define EXP_CmpSecurityManagerSecManGetEditableSetting  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecManGetEditableSetting
	#define EXT_SecManGetEditableSetting
	#define GET_SecManGetEditableSetting(fl)  CAL_CMGETAPI( "SecManGetEditableSetting" ) 
	#define CAL_SecManGetEditableSetting pICmpSecurityManager->ISecManGetEditableSetting
	#define CHK_SecManGetEditableSetting (pICmpSecurityManager != NULL)
	#define EXP_SecManGetEditableSetting  CAL_CMEXPAPI( "SecManGetEditableSetting" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecManGetEditableSetting  PFSECMANGETEDITABLESETTING pfSecManGetEditableSetting;
	#define EXT_SecManGetEditableSetting  extern PFSECMANGETEDITABLESETTING pfSecManGetEditableSetting;
	#define GET_SecManGetEditableSetting(fl)  s_pfCMGetAPI2( "SecManGetEditableSetting", (RTS_VOID_FCTPTR *)&pfSecManGetEditableSetting, (fl), 0, 0)
	#define CAL_SecManGetEditableSetting  pfSecManGetEditableSetting
	#define CHK_SecManGetEditableSetting  (pfSecManGetEditableSetting != NULL)
	#define EXP_SecManGetEditableSetting  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManGetEditableSetting", (RTS_UINTPTR)SecManGetEditableSetting, 0, 0) 
#endif




/**
 * <description>Function to overload the certificate info from a specified server resp. for a usage</description>
 * <param name="cmpId" type="IN">ComponentID of the server that provides security features</param>
 * <param name="pCertInfo" type="IN">Pointer to the preinitialized certificate info</param>
 * <result>Result of the operation</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Certificate info was overloaded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">No overload done</errorcode>
 */
RTS_RESULT CDECL SecManFillCertificateInfo(CMPID cmpId, RtsX509CertInfo *pCertInfo);
typedef RTS_RESULT (CDECL * PFSECMANFILLCERTIFICATEINFO) (CMPID cmpId, RtsX509CertInfo *pCertInfo);
#if defined(CMPSECURITYMANAGER_NOTIMPLEMENTED) || defined(SECMANFILLCERTIFICATEINFO_NOTIMPLEMENTED)
	#define USE_SecManFillCertificateInfo
	#define EXT_SecManFillCertificateInfo
	#define GET_SecManFillCertificateInfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecManFillCertificateInfo(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecManFillCertificateInfo  FALSE
	#define EXP_SecManFillCertificateInfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecManFillCertificateInfo
	#define EXT_SecManFillCertificateInfo
	#define GET_SecManFillCertificateInfo(fl)  CAL_CMGETAPI( "SecManFillCertificateInfo" ) 
	#define CAL_SecManFillCertificateInfo  SecManFillCertificateInfo
	#define CHK_SecManFillCertificateInfo  TRUE
	#define EXP_SecManFillCertificateInfo  CAL_CMEXPAPI( "SecManFillCertificateInfo" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURITYMANAGER_EXTERNAL)
	#define USE_SecManFillCertificateInfo
	#define EXT_SecManFillCertificateInfo
	#define GET_SecManFillCertificateInfo(fl)  CAL_CMGETAPI( "SecManFillCertificateInfo" ) 
	#define CAL_SecManFillCertificateInfo  SecManFillCertificateInfo
	#define CHK_SecManFillCertificateInfo  TRUE
	#define EXP_SecManFillCertificateInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManFillCertificateInfo", (RTS_UINTPTR)SecManFillCertificateInfo, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecurityManagerSecManFillCertificateInfo
	#define EXT_CmpSecurityManagerSecManFillCertificateInfo
	#define GET_CmpSecurityManagerSecManFillCertificateInfo  ERR_OK
	#define CAL_CmpSecurityManagerSecManFillCertificateInfo pICmpSecurityManager->ISecManFillCertificateInfo
	#define CHK_CmpSecurityManagerSecManFillCertificateInfo (pICmpSecurityManager != NULL)
	#define EXP_CmpSecurityManagerSecManFillCertificateInfo  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecManFillCertificateInfo
	#define EXT_SecManFillCertificateInfo
	#define GET_SecManFillCertificateInfo(fl)  CAL_CMGETAPI( "SecManFillCertificateInfo" ) 
	#define CAL_SecManFillCertificateInfo pICmpSecurityManager->ISecManFillCertificateInfo
	#define CHK_SecManFillCertificateInfo (pICmpSecurityManager != NULL)
	#define EXP_SecManFillCertificateInfo  CAL_CMEXPAPI( "SecManFillCertificateInfo" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecManFillCertificateInfo  PFSECMANFILLCERTIFICATEINFO pfSecManFillCertificateInfo;
	#define EXT_SecManFillCertificateInfo  extern PFSECMANFILLCERTIFICATEINFO pfSecManFillCertificateInfo;
	#define GET_SecManFillCertificateInfo(fl)  s_pfCMGetAPI2( "SecManFillCertificateInfo", (RTS_VOID_FCTPTR *)&pfSecManFillCertificateInfo, (fl), 0, 0)
	#define CAL_SecManFillCertificateInfo  pfSecManFillCertificateInfo
	#define CHK_SecManFillCertificateInfo  (pfSecManFillCertificateInfo != NULL)
	#define EXP_SecManFillCertificateInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManFillCertificateInfo", (RTS_UINTPTR)SecManFillCertificateInfo, 0, 0) 
#endif




/**
 * <description>This function checks a TLS context that is created via TlsCreateContext() resp. TlsCreateContext2(). Here the TlsMethod, CipherList and VerifyMethod
 * can be checked against own security policy!
 *
 * NOTE:
 *	This is only a callback function from the implementation of the CmpTlsItf! So don't us it directly!
 * </description>
 * <param name="ui32TlsMethod" type="IN">Mode of the TLS connections. See: TLS_METHOD_... macros</param>
 * <param name="szCipherList" type="IN">Cipher selection of the TLS connection. See CipherList setting for details. If NULL the default of the setting will be used.</param>
 * <param name="i32VerifyMode" type="IN">Verify mode of the TLS connection. See: TLS_VERIFY... macros. Default server: NONE; Default client: VERIFY_PEER.</param>
 * <result>Result of the operation</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">All context settings are complying the security policy</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">At least one context setting is not complying the security policy!</errorcode>
 */
RTS_RESULT CDECL SecManCheckTlsContext(RTS_UI32 ui32TlsMethod, char* pszCipherList, RTS_I32 i32VerifyMode);
typedef RTS_RESULT (CDECL * PFSECMANCHECKTLSCONTEXT) (RTS_UI32 ui32TlsMethod, char* pszCipherList, RTS_I32 i32VerifyMode);
#if defined(CMPSECURITYMANAGER_NOTIMPLEMENTED) || defined(SECMANCHECKTLSCONTEXT_NOTIMPLEMENTED)
	#define USE_SecManCheckTlsContext
	#define EXT_SecManCheckTlsContext
	#define GET_SecManCheckTlsContext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SecManCheckTlsContext(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SecManCheckTlsContext  FALSE
	#define EXP_SecManCheckTlsContext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SecManCheckTlsContext
	#define EXT_SecManCheckTlsContext
	#define GET_SecManCheckTlsContext(fl)  CAL_CMGETAPI( "SecManCheckTlsContext" ) 
	#define CAL_SecManCheckTlsContext  SecManCheckTlsContext
	#define CHK_SecManCheckTlsContext  TRUE
	#define EXP_SecManCheckTlsContext  CAL_CMEXPAPI( "SecManCheckTlsContext" ) 
#elif defined(MIXED_LINK) && !defined(CMPSECURITYMANAGER_EXTERNAL)
	#define USE_SecManCheckTlsContext
	#define EXT_SecManCheckTlsContext
	#define GET_SecManCheckTlsContext(fl)  CAL_CMGETAPI( "SecManCheckTlsContext" ) 
	#define CAL_SecManCheckTlsContext  SecManCheckTlsContext
	#define CHK_SecManCheckTlsContext  TRUE
	#define EXP_SecManCheckTlsContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManCheckTlsContext", (RTS_UINTPTR)SecManCheckTlsContext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSecurityManagerSecManCheckTlsContext
	#define EXT_CmpSecurityManagerSecManCheckTlsContext
	#define GET_CmpSecurityManagerSecManCheckTlsContext  ERR_OK
	#define CAL_CmpSecurityManagerSecManCheckTlsContext pICmpSecurityManager->ISecManCheckTlsContext
	#define CHK_CmpSecurityManagerSecManCheckTlsContext (pICmpSecurityManager != NULL)
	#define EXP_CmpSecurityManagerSecManCheckTlsContext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SecManCheckTlsContext
	#define EXT_SecManCheckTlsContext
	#define GET_SecManCheckTlsContext(fl)  CAL_CMGETAPI( "SecManCheckTlsContext" ) 
	#define CAL_SecManCheckTlsContext pICmpSecurityManager->ISecManCheckTlsContext
	#define CHK_SecManCheckTlsContext (pICmpSecurityManager != NULL)
	#define EXP_SecManCheckTlsContext  CAL_CMEXPAPI( "SecManCheckTlsContext" ) 
#else /* DYNAMIC_LINK */
	#define USE_SecManCheckTlsContext  PFSECMANCHECKTLSCONTEXT pfSecManCheckTlsContext;
	#define EXT_SecManCheckTlsContext  extern PFSECMANCHECKTLSCONTEXT pfSecManCheckTlsContext;
	#define GET_SecManCheckTlsContext(fl)  s_pfCMGetAPI2( "SecManCheckTlsContext", (RTS_VOID_FCTPTR *)&pfSecManCheckTlsContext, (fl), 0, 0)
	#define CAL_SecManCheckTlsContext  pfSecManCheckTlsContext
	#define CHK_SecManCheckTlsContext  (pfSecManCheckTlsContext != NULL)
	#define EXP_SecManCheckTlsContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SecManCheckTlsContext", (RTS_UINTPTR)SecManCheckTlsContext, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSECMANREGISTERSELECTABLESETTING ISecManRegisterSelectableSetting;
 	PFSECMANREGISTEREDITABLESETTING ISecManRegisterEditableSetting;
 	PFSECMANUNREGISTERSETTING ISecManUnregisterSetting;
 	PFSECMANFINDSETTING ISecManFindSetting;
 	PFSECMANGETSELECTABLESETTING ISecManGetSelectableSetting;
 	PFSECMANGETEDITABLESETTING ISecManGetEditableSetting;
 	PFSECMANFILLCERTIFICATEINFO ISecManFillCertificateInfo;
 	PFSECMANCHECKTLSCONTEXT ISecManCheckTlsContext;
 } ICmpSecurityManager_C;

#ifdef CPLUSPLUS
class ICmpSecurityManager : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISecManRegisterSelectableSetting(CMPID cmpId, const char *pszKey, const char *pszDescription, const char *pszDefaultSecName, const SecuritySelectableSetting *pList, RTS_UI32 ui32Settings, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISecManRegisterEditableSetting(CMPID cmpId, const char *pszKey, const char *pszDescription, SecurityEditableSetting setting, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISecManUnregisterSetting(RTS_HANDLE hSecuritySetting) =0;
		virtual RTS_HANDLE CDECL ISecManFindSetting(CMPID cmpId, const char *pszKey, RTS_RESULT *pResult) =0;
		virtual const SecuritySelectableSetting * CDECL ISecManGetSelectableSetting(RTS_HANDLE hSecuritySetting, RTS_RESULT *pResult) =0;
		virtual SecurityEditableSetting * CDECL ISecManGetEditableSetting(RTS_HANDLE hSecuritySetting, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISecManFillCertificateInfo(CMPID cmpId, RtsX509CertInfo *pCertInfo) =0;
		virtual RTS_RESULT CDECL ISecManCheckTlsContext(RTS_UI32 ui32TlsMethod, char* pszCipherList, RTS_I32 i32VerifyMode) =0;
};
	#ifndef ITF_CmpSecurityManager
		#define ITF_CmpSecurityManager static ICmpSecurityManager *pICmpSecurityManager = NULL;
	#endif
	#define EXTITF_CmpSecurityManager
#else	/*CPLUSPLUS*/
	typedef ICmpSecurityManager_C		ICmpSecurityManager;
	#ifndef ITF_CmpSecurityManager
		#define ITF_CmpSecurityManager
	#endif
	#define EXTITF_CmpSecurityManager
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPSECURITYMANAGERITF_H_*/
