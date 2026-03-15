 /**
 * <interfacename>CmpSettings</interfacename>
 * <description> 
 *	Interface for the settings component. The settings component can have different backend
 *	components, to realise different sources for the settings (e.g. ini-File, hardcoded, XML, etc.).
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPSETTINGSITF_H_
#define _CMPSETTINGSITF_H_

#include "CmpStd.h"

 

 




/**
 * <category>Static defines</category>
 * <description>This define can be used to call always the standard settings database.
 * </description>
 */
#ifndef STD_SETTINGS_DATABASE
	#define STD_SETTINGS_DATABASE				"CODESYSControl"
#endif
#ifndef STD_SETTINGS_DATABASE_OLD2
	#define STD_SETTINGS_DATABASE_OLD2			"CoDeSysControl"
#endif

/**
 * <category>Static defines</category>
 * <description>This define can be used to call always the standard settings database.
 * </description>
 */
#ifndef STD_SETTINGS_DATABASE_OLD
	#define STD_SETTINGS_DATABASE_OLD			"CoDeSysSP"
#endif

/**
 * <category>Static defines</category>
 * <description>Delimiter for the end of line, if a settings file is used as backend.
 * </description>
 */
#ifndef FILE_END_OF_LINE_DELIMITER
	#define FILE_END_OF_LINE_DELIMITER			"\r\n"
#endif

/**
 * <category>Static defines</category>
 * <description>Number of static setting files. Only used by CmpSettings component to handle configuration
 *	files in static data.</description>
 */
#ifndef CMPSETTINGS_NUM_OF_STATIC_FILES
	#define CMPSETTINGS_NUM_OF_STATIC_FILES		1
#endif

/**
 * <category>Static defines</category>
 * <description>Name of the master configuration file, if standard config file could not be opened</description>
 */
#ifndef CMPSETTINGS_MASTER_CONFIG
	#define CMPSETTINGS_MASTER_CONFIG			"Master.cfg"
#endif

/**
 * <category>Static defines</category>
 * <description>Name of the standard configuration file</description>
 */
#ifndef CMPSETTINGS_STD_CONFIG
	#define CMPSETTINGS_STD_CONFIG				STD_SETTINGS_DATABASE".cfg"
#endif



/**
 * <category>Settings</category>
 * <description>
 *	Setting for the settings component to specify, if the each component has its own settings database file.
 *	1: Split database files
 *	0: All settings in one database file
 * </description>
 */
#define CMPSETTINGS_KEY_INT_SPLIT_DATABASES					"SplitDatabases"
#ifndef CMPSETTINGS_KEY_INT_SPLIT_DATABASES_DEFAULT
	#define CMPSETTINGS_KEY_INT_SPLIT_DATABASES_DEFAULT		0
#endif

/**
 * <category>Settings</category>
 * <description>
 *	Setting to specify external configuration files:
 *	File reference list must be indexed, started with 0 or 1, e.g.:
 *	FileReference.0=[Configurationfile.cfg]
 *
 *	If the file does not exist, the file is created.
 *  You can specify sections, that should be created in this referenced file, by a comma separated list. e.g.:
 *	FileReference.0=[Configurationfile.cfg], CmpApp
 * </description>
 */
#define CMPSETTINGS_KEY_STRING_FILE_REFERENCE				"FileReference"
#ifndef CMPSETTINGS_KEY_STRING_FILE_REFERENCE_DEFAULT
	#define CMPSETTINGS_KEY_STRING_FILE_REFERENCE_DEFAULT	""
#endif

/**
 * <category>Static defines</category>
 * <description>Settings categories</description>
 */
#define CMPSETTINGS_CATEGORY_SECURITY						"SECURITY"


/**
 * <category>Static defines</category>
 * <description>Predefined objects in the runtime</description>
 */
#define USERDB_OBJECT_SETTINGS		"Device.Settings"

/**
 * <category>Static defines</category>
 * <description>Options of the parameter bCached</description>
 */
/* Value is read from cache (default) */
#define CMPSETTINGS_CACHED 0
/* Value is read from file and cache is updated for all values */
#define CMPSETTINGS_NOT_CACHED 1

/**
 * <category>SelectableSettingEntryFlags</category>
 * <description>
 *	Flags/options for an selectable entry
 * </description>
 */
#define CMPSETTINGS_SFLAGS_NONE			0x00000000

/**
 * <description>
 *	Entry of one slectable setting.
 * </description>
 * <element name="id" type="IN">ID of the selectable setting. Must be unique within this set of selectable settings!</element>
 * <element name="flags" type="IN">Flags of the entry. See "SelectableSettingEntryFlags" for details.</element>
 * <element name="pszValue" type="IN">Value of the setting</element>
 * <element name="pszDescription" type="IN">Description of the setting</element>
 */
typedef struct _SelectableSettingEntry
{
	RTS_I32 id;
	RTS_UI32 flags;
	char *pszValue;
	char *pszDescription;
} SelectableSettingEntry;

/**
 * <description>
 *	This structure stores a single entry of a selectable setting. pList is an array of selctable entries!
  * </description>
 * <element name="pszValueSelected" type="IN">Name of the setting, which is currently selected</element>
 * <element name="ui32Settings" type="IN">Number of selectable settings in pList</element>
 * <element name="pList" type="IN">Pointer to array of selectable entries</element>
 */
typedef struct _SelectableSetting
{
	char *pszValueSelected;
	RTS_UI32 ui32Settings;
	const SelectableSettingEntry *pList;
} SelectableSetting;


/**
 * <description>
 *	Union for an editable integer, string or wstring setting.
 * </description>
 * <element name="pStringSetting" type="IN">String value UTF8</element>
 * <element name="pWStringSetting" type="IN">String value UTF16</element>
 * <element name="intSetting" type="IN">Integer value</element>
 */
typedef union
{
    RTS_CSTRING *pStringSetting;
	RTS_WCHAR *pWStringSetting;
    RTS_I32 intSetting;
} SettingValue;

/**
 * <category>EditableFlags</category>
 * <description>
 *	Flags/options for an editable setting
 * </description>
 */
#define CMPSETTINGS_EFLAGS_EDITABLE_STRING			0x00000001
#define CMPSETTINGS_EFLAGS_EDITABLE_INT				0x00000002
#define CMPSETTINGS_EFLAGS_EDITABLE_WSTRING			0x00000004

/**
 * <description>
 *	This structure is to transmit an editable setting.
 * </description>
 * <element name="valueLen" type="IN">Lenght of the value (if it is a string or wstring without trailing 0!)</element>
 * <element name="value" type="IN">Current value</element>
 * <element name="valueDefault" type="IN">Default value</element>
 * <element name="flags" type="IN">Flags of the entry. See "EditableFlags" for details.</element>
 */
typedef struct _EditableSetting
{
	RTS_SIZE valueLen;
	SettingValue value;
	SettingValue valueDefault;
	RTS_UI32 flags;
} EditableSetting;

/**
 * <category>SettingFlags</category>
 * <description>
 *	Flags/options for a setting
 * </description>
 */
#define CMPSETTINGS_FLAGS_NONE						0
#define CMPSETTINGS_FLAGS_SELECTABLE				0x00000001
#define CMPSETTINGS_FLAGS_EDITABLE					0x00000002

/**
 * <description>
 *	Entry of one complete setting.
 * </description>
 * <element name="pszComponentName" type="IN">Component name which the setting belongs to</element>
 * <element name="pszKey" type="IN">Key name</element>
 * <element name="pszDescription" type="IN">Description of the setting</element>
 * <element name="flags" type="IN">Flags of the entry. See "SettingFlags" for details.</element>
 * <element name="setting" type="IN">Selectable or editable entry</element>
 */
typedef struct _SettingEntry
{
	const char *pszComponentName;
	const char *pszKey;
	const char *pszDescription;

	RTS_UI32 flags;
	union
	{
		SelectableSetting selectableSetting;
		EditableSetting editableSetting;
	} setting;
} SettingEntry;


/**
 * <category>Static defines</category>
 * <description>Length of application list that is allocated static</description>
 */
#ifndef SETTG_NUM_OF_STATIC_SETTINGS
	#define SETTG_NUM_OF_STATIC_SETTINGS				10
#endif

typedef struct _EVTPARAM_SettgReadSetting
{
	char *pszCategory;
	SettingEntry entry;
	RTS_UI32 ulSessionID;
	RTS_RESULT Result;
} EVTPARAM_SettgReadSetting;
#define EVTPARAMID_CmpSettings_ReadSetting				0x0001
#define EVTVERSION_CmpSettings_ReadSetting				0x0001

typedef struct _EVTPARAM_SettgWriteSetting
{
	char *pszCategory;
	SettingEntry entry;
	RTS_UI32 ulSessionID;
	RTS_RESULT Result;
} EVTPARAM_SettgWriteSetting;
#define EVTPARAMID_CmpSettings_WriteSetting				0x0002
#define EVTVERSION_CmpSettings_WriteSetting				0x0001

typedef struct _EVTPARAM_SettgReadSettings
{
	char *pszCategory;
	RTS_UI32 ulSessionID;
	RTS_HANDLE hMemPool;	/* ATTENTION: This is a pool created with a list of SettingEntry structs! So pool entries that matched the selected category. */
} EVTPARAM_SettgReadSettings;
#define EVTPARAMID_CmpSettings_ReadSettings				0x0003
#define EVTVERSION_CmpSettings_ReadSettings				0x0001

/**
 * <category>Events</category>
 * <description>Event to signalize, that a setting is read online!</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SettgReadSetting</param>
 */
#define EVT_Settg_ReadSetting					MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Events</category>
 * <description>Event to signalize, that a setting is written online!</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SettgWriteSetting</param>
 */
#define EVT_Settg_WriteSetting					MAKE_EVENTID(EVTCLASS_INFO, 2)

/**
 * <category>Events</category>
 * <description>Event to signalize, that a list of settings (optionally specified by a category) is read online!</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SettgReadSettings</param>
 */
#define EVT_Settg_ReadSettings					MAKE_EVENTID(EVTCLASS_INFO, 3)

/**
 * <category>Online services</category>
 * <description>
 *	Online services for the settings.
 * </description>
 * <servicegroup name="SG_SETTINGS">0x0006</servicegroup>
 */
#define SRV_SETTG_GET_INT_VALUE						0x01
#define SRV_SETTG_SET_INT_VALUE						0x02
#define SRV_SETTG_GET_STRING_VALUE					0x03
#define SRV_SETTG_SET_STRING_VALUE					0x04
#define SRV_SETTG_REMOVE_KEY						0x05
#define SRV_SETTG_READ_SETTINGS						0x06
#define SRV_SETTG_WRITE_SETTINGS					0x07

/* Tags */
#define TAGNEST_SETTG_GET_INT_VALUE					0x81
#define TAG_SETTG_GET_INT_RESPONSE					0x01

#define TAGNEST_SETTG_SET_INT_VALUE					0x81
#define TAG_SETTG_SET_INT_RESPONSE					0x01

#define TAGNEST_SETTG_GET_STRING_VALUE				0x81
#define TAG_SETTG_GET_STRING_RESPONSE				0x01

#define TAGNEST_SETTG_SET_STRING_VALUE				0x81
#define TAG_SETTG_SET_STRING_RESPONSE				0x01

#define TAGNEST_SETTG_REMOVE_KEY					0x81
#define TAG_SETTG_REMOVE_KEY_RESPONSE				0x01

/* Tags */
#define TAG_SETTG_COMPONENT							0x10
#define TAG_SETTG_KEY								0x11
#define TAG_SETTG_INT_VALUE							0x12
#define TAG_SETTG_STRING_VALUE						0x13
#define TAG_SETTG_ERROR_RESPONSE_					0x14
#define TAG_SETTG_RESULT							0x15

#define TAGNEST_SETTINGS							0x86
#define TAGNEST_SETTING_INFO						0x87
#define TAG_SETTG_CATEGORY							0x20
#define TAG_SETTG_DESCRIPTION						0x21
#define TAG_SETTG_FLAGS								0x22

#define TAGNEST_SETTG_SETTING_SELECTABLE			0x88
#define TAG_SETTG_SELECTED							0x23
#define TAG_SETTG_NUM_OF_SETTINGS					0x24

#define TAGNEST_SETTG_SETTING_SELECTABLE_ENTRY		0x89
#define TAG_SETTG_SETTING_FLAGS						0x31
#define TAG_SETTG_SETTING_NAME						0x32
#define TAG_SETTG_SETTING_DESCRIPTION				0x33

#define TAGNEST_SETTG_SETTING_EDITABLE				0x8A
#define TAG_SETTG_SETTING_VALUE_INT					0x40
#define TAG_SETTG_SETTING_DEFAULTVALUE_INT			0x41
#define TAG_SETTG_SETTING_VALUE_STRING				0x42
#define TAG_SETTG_SETTING_DEFAULTVALUE_STRING		0x43



/**
 * <category>Online services</category>
 * <description>
 *	Online service to read all security setting.
 * </description>
 * <servicegroup name="SG_SETTINGS">0x0006</servicegroup>
 * <service name="SRV_SETTG_READ_SETTINGS">
 *	<Request>
 *		<tag name="TAG_SETTG_CATEGORY" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *		<tag name="TAGNEST_SETTING_INFO" required="optional">Complex tag: Is only sent, if previous service sent result ERR_ENTRIES_REMAINING!</tag>
 *			<tag name="TAG_SETTG_COMPONENT" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *			<tag name="TAG_SETTG_KEY" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *	</Request>
 *	<Response>
 *		<tag[] name="TAGNEST_SETTINGS" required="mandatory">Complex tag</tag>
 *			<tag name="TAGNEST_SETTING_INFO" required="mandatory">Complex tag</tag>
 *				<tag name="TAG_SETTG_COMPONENT" required="mandatory">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SETTG_KEY" required="mandatory">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SETTG_DESCRIPTION" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SETTG_FLAGS" required="mandatory">[RTS_UI32]: TODO</tag>
 *
 *			<alternate tag[] name="TAGNEST_SETTG_SETTING_SELECTABLE" required="optional">Complex tag</tag>
 *				<tag name="TAG_SETTG_SELECTED" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SETTG_NUM_OF_SETTINGS" required="mandatory">[RTS_UI32]: TODO</tag>
 
 *				<tag[] name="TAGNEST_SETTG_SETTING_SELECTABLE_ENTRY" required="mandatory">Complex tag</tag>
 *					<tag name="TAG_SETTG_SETTING_FLAGS" required="mandatory">[RTS_UI32]: TODO</tag>
 *					<tag name="TAG_SETTG_SETTING_NAME" required="mandatory">[RTS_CSTRING::UTF-8]: TODO</tag>
 *					<tag name="TAG_SETTG_SETTING_DESCRIPTION" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *
 *			<alternate tag name="TAGNEST_SETTG_SETTING_EDITABLE" required="optional">Complex tag</tag>
 *				<tag name="TAG_SETTG_SETTING_FLAGS" required="mandatory">[RTS_UI32]: TODO</tag>
 *				<tag name="TAG_SETTG_SETTING_NAME" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SETTG_SETTING_DESCRIPTION" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SETTG_SETTING_VALUE_INT" required="optional">[RTS_I32]: TODO</tag>
 *				<tag name="TAG_SETTG_SETTING_DEFAULTVALUE_INT" required="optional">[RTS_I32]: TODO</tag>
 *				<tag name="TAG_SETTG_SETTING_VALUE_STRING" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SETTG_SETTING_DEFAULTVALUE_STRING" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *
 *		<tag name="TAG_SETTG_RESULT" required="mandatory">[RTS_I16]: Result code of the complete operation</tag>
 *	</Response>
 * </service>
 */

/**
 * <category>Online services</category>
 * <description>
 *	Online service to write changed security settings.
 * </description>
 * <servicegroup name="SG_SETTINGS">0x0006</servicegroup>
 * <service name="SRV_SETTG_WRITE_SETTINGS">
 *	<Request>
 *		<tag name="TAG_SETTG_CATEGORY" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *		<tag[] name="TAGNEST_SETTINGS" required="mandatory">Complex tag</tag>
 *			<tag name="TAGNEST_SETTING_INFO" required="mandatory">Complex tag</tag>
 *				<tag name="TAG_SETTG_COMPONENT" required="mandatory">[RTS_CSTRING::UTF-8]: TODO</tag>
 *				<tag name="TAG_SETTG_KEY" required="mandatory">[RTS_CSTRING::UTF-8]: TODO</tag>
 *
 *			<alternate tag name="TAGNEST_SETTG_SETTING_SELECTABLE" required="optional">Complex tag</tag>
 *				<tag name="TAG_SETTG_SELECTED" required="optional">[RTS_CSTRING::UTF-8]: Is only sent, if it is a selectable setting!</tag>
 *
 *			<alternate tag name="TAGNEST_SETTG_SETTING_EDITABLE" required="optional">Complex tag</tag>
 *				<tag name="TAG_SETTG_SETTING_VALUE_INT" required="optional">[RTS_I32]: TODO</tag>
 *				<tag name="TAG_SETTG_SETTING_VALUE_STRING" required="optional">[RTS_CSTRING::UTF-8]: TODO</tag>
 *	</Request>
 *	<Response>
 *		<tag[] name="TAG_SETTG_RESULT" required="mandatory">[RTS_I16]: Result code of each write operation</tag>
 *	</Response>
 * </service>
 */


/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * | Function to block writing settings to database file until |SettgEndUpdate| is called! But the settings cache is updated.
 * | NOTE:
 * | - Funcion can be called nested, so only at the last call of |SettgEndUpdate|, the database is written!
 * | - Always call SettgBeginUpdate() and SettgEndUpdate() symmetric!!!
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsettgbeginupdate_struct
{
	RTS_IEC_RESULT SettgBeginUpdate;	/* VAR_OUTPUT */	
} settgbeginupdate_struct;

void CDECL CDECL_EXT settgbeginupdate(settgbeginupdate_struct *p);
typedef void (CDECL CDECL_EXT* PFSETTGBEGINUPDATE_IEC) (settgbeginupdate_struct *p);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGBEGINUPDATE_NOTIMPLEMENTED)
	#define USE_settgbeginupdate
	#define EXT_settgbeginupdate
	#define GET_settgbeginupdate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_settgbeginupdate(p0) 
	#define CHK_settgbeginupdate  FALSE
	#define EXP_settgbeginupdate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_settgbeginupdate
	#define EXT_settgbeginupdate
	#define GET_settgbeginupdate(fl)  CAL_CMGETAPI( "settgbeginupdate" ) 
	#define CAL_settgbeginupdate  settgbeginupdate
	#define CHK_settgbeginupdate  TRUE
	#define EXP_settgbeginupdate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgbeginupdate", (RTS_UINTPTR)settgbeginupdate, 1, 0x8767D13E, 0x03050700) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_settgbeginupdate
	#define EXT_settgbeginupdate
	#define GET_settgbeginupdate(fl)  CAL_CMGETAPI( "settgbeginupdate" ) 
	#define CAL_settgbeginupdate  settgbeginupdate
	#define CHK_settgbeginupdate  TRUE
	#define EXP_settgbeginupdate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgbeginupdate", (RTS_UINTPTR)settgbeginupdate, 1, 0x8767D13E, 0x03050700) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingssettgbeginupdate
	#define EXT_CmpSettingssettgbeginupdate
	#define GET_CmpSettingssettgbeginupdate  ERR_OK
	#define CAL_CmpSettingssettgbeginupdate  settgbeginupdate
	#define CHK_CmpSettingssettgbeginupdate  TRUE
	#define EXP_CmpSettingssettgbeginupdate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgbeginupdate", (RTS_UINTPTR)settgbeginupdate, 1, 0x8767D13E, 0x03050700) 
#elif defined(CPLUSPLUS)
	#define USE_settgbeginupdate
	#define EXT_settgbeginupdate
	#define GET_settgbeginupdate(fl)  CAL_CMGETAPI( "settgbeginupdate" ) 
	#define CAL_settgbeginupdate  settgbeginupdate
	#define CHK_settgbeginupdate  TRUE
	#define EXP_settgbeginupdate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgbeginupdate", (RTS_UINTPTR)settgbeginupdate, 1, 0x8767D13E, 0x03050700) 
#else /* DYNAMIC_LINK */
	#define USE_settgbeginupdate  PFSETTGBEGINUPDATE_IEC pfsettgbeginupdate;
	#define EXT_settgbeginupdate  extern PFSETTGBEGINUPDATE_IEC pfsettgbeginupdate;
	#define GET_settgbeginupdate(fl)  s_pfCMGetAPI2( "settgbeginupdate", (RTS_VOID_FCTPTR *)&pfsettgbeginupdate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x8767D13E, 0x03050700)
	#define CAL_settgbeginupdate  pfsettgbeginupdate
	#define CHK_settgbeginupdate  (pfsettgbeginupdate != NULL)
	#define EXP_settgbeginupdate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgbeginupdate", (RTS_UINTPTR)settgbeginupdate, 1, 0x8767D13E, 0x03050700) 
#endif


/**
 * | Function to initiate writing settings to database file, if settings are modified after |SettgBeginUpdate| was called.
 * | NOTE:
 * | - Funcion can be called nested, so only at the last call of |SettgEndUpdate|, the database is written!
 * | - Always call |SettgBeginUpdate| and |SettgEndUpdate| symmetric!!!
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsettgendupdate_struct
{
	RTS_IEC_RESULT SettgEndUpdate;		/* VAR_OUTPUT */	
} settgendupdate_struct;

void CDECL CDECL_EXT settgendupdate(settgendupdate_struct *p);
typedef void (CDECL CDECL_EXT* PFSETTGENDUPDATE_IEC) (settgendupdate_struct *p);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGENDUPDATE_NOTIMPLEMENTED)
	#define USE_settgendupdate
	#define EXT_settgendupdate
	#define GET_settgendupdate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_settgendupdate(p0) 
	#define CHK_settgendupdate  FALSE
	#define EXP_settgendupdate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_settgendupdate
	#define EXT_settgendupdate
	#define GET_settgendupdate(fl)  CAL_CMGETAPI( "settgendupdate" ) 
	#define CAL_settgendupdate  settgendupdate
	#define CHK_settgendupdate  TRUE
	#define EXP_settgendupdate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgendupdate", (RTS_UINTPTR)settgendupdate, 1, 0x460858D3, 0x03050700) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_settgendupdate
	#define EXT_settgendupdate
	#define GET_settgendupdate(fl)  CAL_CMGETAPI( "settgendupdate" ) 
	#define CAL_settgendupdate  settgendupdate
	#define CHK_settgendupdate  TRUE
	#define EXP_settgendupdate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgendupdate", (RTS_UINTPTR)settgendupdate, 1, 0x460858D3, 0x03050700) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingssettgendupdate
	#define EXT_CmpSettingssettgendupdate
	#define GET_CmpSettingssettgendupdate  ERR_OK
	#define CAL_CmpSettingssettgendupdate  settgendupdate
	#define CHK_CmpSettingssettgendupdate  TRUE
	#define EXP_CmpSettingssettgendupdate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgendupdate", (RTS_UINTPTR)settgendupdate, 1, 0x460858D3, 0x03050700) 
#elif defined(CPLUSPLUS)
	#define USE_settgendupdate
	#define EXT_settgendupdate
	#define GET_settgendupdate(fl)  CAL_CMGETAPI( "settgendupdate" ) 
	#define CAL_settgendupdate  settgendupdate
	#define CHK_settgendupdate  TRUE
	#define EXP_settgendupdate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgendupdate", (RTS_UINTPTR)settgendupdate, 1, 0x460858D3, 0x03050700) 
#else /* DYNAMIC_LINK */
	#define USE_settgendupdate  PFSETTGENDUPDATE_IEC pfsettgendupdate;
	#define EXT_settgendupdate  extern PFSETTGENDUPDATE_IEC pfsettgendupdate;
	#define GET_settgendupdate(fl)  s_pfCMGetAPI2( "settgendupdate", (RTS_VOID_FCTPTR *)&pfsettgendupdate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x460858D3, 0x03050700)
	#define CAL_settgendupdate  pfsettgendupdate
	#define CHK_settgendupdate  (pfsettgendupdate != NULL)
	#define EXP_settgendupdate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgendupdate", (RTS_UINTPTR)settgendupdate, 1, 0x460858D3, 0x03050700) 
#endif


/**
 * | Get an integer value from settings
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsettggetintvalue_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	/* Name of component */
	RTS_IEC_STRING *pszKey;				/* VAR_INPUT */	/* Name of key */
	RTS_IEC_DINT *piValue;				/* VAR_INPUT */	/* Pointer to value for result */
	RTS_IEC_DINT iDefault;				/* VAR_INPUT */	/* Default value to set, if key not found */
	RTS_IEC_DINT bCached;				/* VAR_INPUT */	/* Flag, if value should be read cached or direct from file (CMPSETTINGS_CACHED, CMPSETTINGS_NOT_CACHED) */
	RTS_IEC_RESULT SettgGetIntValue;	/* VAR_OUTPUT */	
} settggetintvalue_struct;

void CDECL CDECL_EXT settggetintvalue(settggetintvalue_struct *p);
typedef void (CDECL CDECL_EXT* PFSETTGGETINTVALUE_IEC) (settggetintvalue_struct *p);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGGETINTVALUE_NOTIMPLEMENTED)
	#define USE_settggetintvalue
	#define EXT_settggetintvalue
	#define GET_settggetintvalue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_settggetintvalue(p0) 
	#define CHK_settggetintvalue  FALSE
	#define EXP_settggetintvalue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_settggetintvalue
	#define EXT_settggetintvalue
	#define GET_settggetintvalue(fl)  CAL_CMGETAPI( "settggetintvalue" ) 
	#define CAL_settggetintvalue  settggetintvalue
	#define CHK_settggetintvalue  TRUE
	#define EXP_settggetintvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetintvalue", (RTS_UINTPTR)settggetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0x0065176D), 0x03050700) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_settggetintvalue
	#define EXT_settggetintvalue
	#define GET_settggetintvalue(fl)  CAL_CMGETAPI( "settggetintvalue" ) 
	#define CAL_settggetintvalue  settggetintvalue
	#define CHK_settggetintvalue  TRUE
	#define EXP_settggetintvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetintvalue", (RTS_UINTPTR)settggetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0x0065176D), 0x03050700) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingssettggetintvalue
	#define EXT_CmpSettingssettggetintvalue
	#define GET_CmpSettingssettggetintvalue  ERR_OK
	#define CAL_CmpSettingssettggetintvalue  settggetintvalue
	#define CHK_CmpSettingssettggetintvalue  TRUE
	#define EXP_CmpSettingssettggetintvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetintvalue", (RTS_UINTPTR)settggetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0x0065176D), 0x03050700) 
#elif defined(CPLUSPLUS)
	#define USE_settggetintvalue
	#define EXT_settggetintvalue
	#define GET_settggetintvalue(fl)  CAL_CMGETAPI( "settggetintvalue" ) 
	#define CAL_settggetintvalue  settggetintvalue
	#define CHK_settggetintvalue  TRUE
	#define EXP_settggetintvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetintvalue", (RTS_UINTPTR)settggetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0x0065176D), 0x03050700) 
#else /* DYNAMIC_LINK */
	#define USE_settggetintvalue  PFSETTGGETINTVALUE_IEC pfsettggetintvalue;
	#define EXT_settggetintvalue  extern PFSETTGGETINTVALUE_IEC pfsettggetintvalue;
	#define GET_settggetintvalue(fl)  s_pfCMGetAPI2( "settggetintvalue", (RTS_VOID_FCTPTR *)&pfsettggetintvalue, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x0065176D), 0x03050700)
	#define CAL_settggetintvalue  pfsettggetintvalue
	#define CHK_settggetintvalue  (pfsettggetintvalue != NULL)
	#define EXP_settggetintvalue   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetintvalue", (RTS_UINTPTR)settggetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0x0065176D), 0x03050700) 
#endif


/**
 * | Get a string value from settings
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsettggetstringvalue_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	/* Name of component */
	RTS_IEC_STRING *pszKey;				/* VAR_INPUT */	/* Name of key */
	RTS_IEC_STRING *pszValue;			/* VAR_INPUT */	/* Pointer to value for result */
	RTS_IEC_DINT *piLen;				/* VAR_INPUT */	/* Max length of string value */
	RTS_IEC_STRING *pszDefault;			/* VAR_INPUT */	/* Default value to set, if key not found */
	RTS_IEC_DINT bCached;				/* VAR_INPUT */	/* Flag, if value should be read cached or direct from file (CMPSETTINGS_CACHED, CMPSETTINGS_NOT_CACHED) */
	RTS_IEC_RESULT SettgGetStringValue;	/* VAR_OUTPUT */	
} settggetstringvalue_struct;

void CDECL CDECL_EXT settggetstringvalue(settggetstringvalue_struct *p);
typedef void (CDECL CDECL_EXT* PFSETTGGETSTRINGVALUE_IEC) (settggetstringvalue_struct *p);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGGETSTRINGVALUE_NOTIMPLEMENTED)
	#define USE_settggetstringvalue
	#define EXT_settggetstringvalue
	#define GET_settggetstringvalue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_settggetstringvalue(p0) 
	#define CHK_settggetstringvalue  FALSE
	#define EXP_settggetstringvalue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_settggetstringvalue
	#define EXT_settggetstringvalue
	#define GET_settggetstringvalue(fl)  CAL_CMGETAPI( "settggetstringvalue" ) 
	#define CAL_settggetstringvalue  settggetstringvalue
	#define CHK_settggetstringvalue  TRUE
	#define EXP_settggetstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetstringvalue", (RTS_UINTPTR)settggetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x366FF6F7), 0x03050700) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_settggetstringvalue
	#define EXT_settggetstringvalue
	#define GET_settggetstringvalue(fl)  CAL_CMGETAPI( "settggetstringvalue" ) 
	#define CAL_settggetstringvalue  settggetstringvalue
	#define CHK_settggetstringvalue  TRUE
	#define EXP_settggetstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetstringvalue", (RTS_UINTPTR)settggetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x366FF6F7), 0x03050700) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingssettggetstringvalue
	#define EXT_CmpSettingssettggetstringvalue
	#define GET_CmpSettingssettggetstringvalue  ERR_OK
	#define CAL_CmpSettingssettggetstringvalue  settggetstringvalue
	#define CHK_CmpSettingssettggetstringvalue  TRUE
	#define EXP_CmpSettingssettggetstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetstringvalue", (RTS_UINTPTR)settggetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x366FF6F7), 0x03050700) 
#elif defined(CPLUSPLUS)
	#define USE_settggetstringvalue
	#define EXT_settggetstringvalue
	#define GET_settggetstringvalue(fl)  CAL_CMGETAPI( "settggetstringvalue" ) 
	#define CAL_settggetstringvalue  settggetstringvalue
	#define CHK_settggetstringvalue  TRUE
	#define EXP_settggetstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetstringvalue", (RTS_UINTPTR)settggetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x366FF6F7), 0x03050700) 
#else /* DYNAMIC_LINK */
	#define USE_settggetstringvalue  PFSETTGGETSTRINGVALUE_IEC pfsettggetstringvalue;
	#define EXT_settggetstringvalue  extern PFSETTGGETSTRINGVALUE_IEC pfsettggetstringvalue;
	#define GET_settggetstringvalue(fl)  s_pfCMGetAPI2( "settggetstringvalue", (RTS_VOID_FCTPTR *)&pfsettggetstringvalue, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x366FF6F7), 0x03050700)
	#define CAL_settggetstringvalue  pfsettggetstringvalue
	#define CHK_settggetstringvalue  (pfsettggetstringvalue != NULL)
	#define EXP_settggetstringvalue   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetstringvalue", (RTS_UINTPTR)settggetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x366FF6F7), 0x03050700) 
#endif


/**
 * | Get a WSTRING value from settings
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsettggetwstringvalue_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	/* Name of component */
	RTS_IEC_STRING *pszKey;				/* VAR_INPUT */	/* Name of key */
	RTS_IEC_WSTRING *pwszValue;			/* VAR_INPUT */	/* Pointer to value for result */
	RTS_IEC_DINT *piLen;				/* VAR_INPUT */	/* Max length of string in WSTRING characters (not bytes!) */
	RTS_IEC_WSTRING *pwszDefault;		/* VAR_INPUT */	/* Default value to set, if key not found */
	RTS_IEC_DINT bCached;				/* VAR_INPUT */	/* Flag, if value should be read cached or direct from file (CMPSETTINGS_CACHED, CMPSETTINGS_NOT_CACHED) */
	RTS_IEC_RESULT SettgGetWStringValue;	/* VAR_OUTPUT */	
} settggetwstringvalue_struct;

void CDECL CDECL_EXT settggetwstringvalue(settggetwstringvalue_struct *p);
typedef void (CDECL CDECL_EXT* PFSETTGGETWSTRINGVALUE_IEC) (settggetwstringvalue_struct *p);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGGETWSTRINGVALUE_NOTIMPLEMENTED)
	#define USE_settggetwstringvalue
	#define EXT_settggetwstringvalue
	#define GET_settggetwstringvalue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_settggetwstringvalue(p0) 
	#define CHK_settggetwstringvalue  FALSE
	#define EXP_settggetwstringvalue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_settggetwstringvalue
	#define EXT_settggetwstringvalue
	#define GET_settggetwstringvalue(fl)  CAL_CMGETAPI( "settggetwstringvalue" ) 
	#define CAL_settggetwstringvalue  settggetwstringvalue
	#define CHK_settggetwstringvalue  TRUE
	#define EXP_settggetwstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetwstringvalue", (RTS_UINTPTR)settggetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x17E6DC47), 0x03050700) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_settggetwstringvalue
	#define EXT_settggetwstringvalue
	#define GET_settggetwstringvalue(fl)  CAL_CMGETAPI( "settggetwstringvalue" ) 
	#define CAL_settggetwstringvalue  settggetwstringvalue
	#define CHK_settggetwstringvalue  TRUE
	#define EXP_settggetwstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetwstringvalue", (RTS_UINTPTR)settggetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x17E6DC47), 0x03050700) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingssettggetwstringvalue
	#define EXT_CmpSettingssettggetwstringvalue
	#define GET_CmpSettingssettggetwstringvalue  ERR_OK
	#define CAL_CmpSettingssettggetwstringvalue  settggetwstringvalue
	#define CHK_CmpSettingssettggetwstringvalue  TRUE
	#define EXP_CmpSettingssettggetwstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetwstringvalue", (RTS_UINTPTR)settggetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x17E6DC47), 0x03050700) 
#elif defined(CPLUSPLUS)
	#define USE_settggetwstringvalue
	#define EXT_settggetwstringvalue
	#define GET_settggetwstringvalue(fl)  CAL_CMGETAPI( "settggetwstringvalue" ) 
	#define CAL_settggetwstringvalue  settggetwstringvalue
	#define CHK_settggetwstringvalue  TRUE
	#define EXP_settggetwstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetwstringvalue", (RTS_UINTPTR)settggetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x17E6DC47), 0x03050700) 
#else /* DYNAMIC_LINK */
	#define USE_settggetwstringvalue  PFSETTGGETWSTRINGVALUE_IEC pfsettggetwstringvalue;
	#define EXT_settggetwstringvalue  extern PFSETTGGETWSTRINGVALUE_IEC pfsettggetwstringvalue;
	#define GET_settggetwstringvalue(fl)  s_pfCMGetAPI2( "settggetwstringvalue", (RTS_VOID_FCTPTR *)&pfsettggetwstringvalue, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x17E6DC47), 0x03050700)
	#define CAL_settggetwstringvalue  pfsettggetwstringvalue
	#define CHK_settggetwstringvalue  (pfsettggetwstringvalue != NULL)
	#define EXP_settggetwstringvalue   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settggetwstringvalue", (RTS_UINTPTR)settggetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x17E6DC47), 0x03050700) 
#endif


/**
 * | Remove the specified key
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsettgremovekey_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	/* Name of component */
	RTS_IEC_STRING *pszKey;				/* VAR_INPUT */	/* Name of key */
	RTS_IEC_RESULT SettgRemoveKey;		/* VAR_OUTPUT */	
} settgremovekey_struct;

void CDECL CDECL_EXT settgremovekey(settgremovekey_struct *p);
typedef void (CDECL CDECL_EXT* PFSETTGREMOVEKEY_IEC) (settgremovekey_struct *p);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGREMOVEKEY_NOTIMPLEMENTED)
	#define USE_settgremovekey
	#define EXT_settgremovekey
	#define GET_settgremovekey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_settgremovekey(p0) 
	#define CHK_settgremovekey  FALSE
	#define EXP_settgremovekey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_settgremovekey
	#define EXT_settgremovekey
	#define GET_settgremovekey(fl)  CAL_CMGETAPI( "settgremovekey" ) 
	#define CAL_settgremovekey  settgremovekey
	#define CHK_settgremovekey  TRUE
	#define EXP_settgremovekey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgremovekey", (RTS_UINTPTR)settgremovekey, 1, RTSITF_GET_SIGNATURE(0, 0xBA5C250F), 0x03050700) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_settgremovekey
	#define EXT_settgremovekey
	#define GET_settgremovekey(fl)  CAL_CMGETAPI( "settgremovekey" ) 
	#define CAL_settgremovekey  settgremovekey
	#define CHK_settgremovekey  TRUE
	#define EXP_settgremovekey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgremovekey", (RTS_UINTPTR)settgremovekey, 1, RTSITF_GET_SIGNATURE(0, 0xBA5C250F), 0x03050700) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingssettgremovekey
	#define EXT_CmpSettingssettgremovekey
	#define GET_CmpSettingssettgremovekey  ERR_OK
	#define CAL_CmpSettingssettgremovekey  settgremovekey
	#define CHK_CmpSettingssettgremovekey  TRUE
	#define EXP_CmpSettingssettgremovekey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgremovekey", (RTS_UINTPTR)settgremovekey, 1, RTSITF_GET_SIGNATURE(0, 0xBA5C250F), 0x03050700) 
#elif defined(CPLUSPLUS)
	#define USE_settgremovekey
	#define EXT_settgremovekey
	#define GET_settgremovekey(fl)  CAL_CMGETAPI( "settgremovekey" ) 
	#define CAL_settgremovekey  settgremovekey
	#define CHK_settgremovekey  TRUE
	#define EXP_settgremovekey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgremovekey", (RTS_UINTPTR)settgremovekey, 1, RTSITF_GET_SIGNATURE(0, 0xBA5C250F), 0x03050700) 
#else /* DYNAMIC_LINK */
	#define USE_settgremovekey  PFSETTGREMOVEKEY_IEC pfsettgremovekey;
	#define EXT_settgremovekey  extern PFSETTGREMOVEKEY_IEC pfsettgremovekey;
	#define GET_settgremovekey(fl)  s_pfCMGetAPI2( "settgremovekey", (RTS_VOID_FCTPTR *)&pfsettgremovekey, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xBA5C250F), 0x03050700)
	#define CAL_settgremovekey  pfsettgremovekey
	#define CHK_settgremovekey  (pfsettgremovekey != NULL)
	#define EXP_settgremovekey   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgremovekey", (RTS_UINTPTR)settgremovekey, 1, RTSITF_GET_SIGNATURE(0, 0xBA5C250F), 0x03050700) 
#endif


/**
 * | Write an integer value to settings
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsettgsetintvalue_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	/* Name of component */
	RTS_IEC_STRING *pszKey;				/* VAR_INPUT */	/* Name of key */
	RTS_IEC_DINT iValue;				/* VAR_INPUT */	/* Value to write */
	RTS_IEC_DINT iBase;					/* VAR_INPUT */	/* 2=Base 2, 10=Decimal values, 16=Hex values */
	RTS_IEC_RESULT SettgSetIntValue;	/* VAR_OUTPUT */	
} settgsetintvalue_struct;

void CDECL CDECL_EXT settgsetintvalue(settgsetintvalue_struct *p);
typedef void (CDECL CDECL_EXT* PFSETTGSETINTVALUE_IEC) (settgsetintvalue_struct *p);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGSETINTVALUE_NOTIMPLEMENTED)
	#define USE_settgsetintvalue
	#define EXT_settgsetintvalue
	#define GET_settgsetintvalue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_settgsetintvalue(p0) 
	#define CHK_settgsetintvalue  FALSE
	#define EXP_settgsetintvalue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_settgsetintvalue
	#define EXT_settgsetintvalue
	#define GET_settgsetintvalue(fl)  CAL_CMGETAPI( "settgsetintvalue" ) 
	#define CAL_settgsetintvalue  settgsetintvalue
	#define CHK_settgsetintvalue  TRUE
	#define EXP_settgsetintvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetintvalue", (RTS_UINTPTR)settgsetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0xBC5E022C), 0x03050700) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_settgsetintvalue
	#define EXT_settgsetintvalue
	#define GET_settgsetintvalue(fl)  CAL_CMGETAPI( "settgsetintvalue" ) 
	#define CAL_settgsetintvalue  settgsetintvalue
	#define CHK_settgsetintvalue  TRUE
	#define EXP_settgsetintvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetintvalue", (RTS_UINTPTR)settgsetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0xBC5E022C), 0x03050700) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingssettgsetintvalue
	#define EXT_CmpSettingssettgsetintvalue
	#define GET_CmpSettingssettgsetintvalue  ERR_OK
	#define CAL_CmpSettingssettgsetintvalue  settgsetintvalue
	#define CHK_CmpSettingssettgsetintvalue  TRUE
	#define EXP_CmpSettingssettgsetintvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetintvalue", (RTS_UINTPTR)settgsetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0xBC5E022C), 0x03050700) 
#elif defined(CPLUSPLUS)
	#define USE_settgsetintvalue
	#define EXT_settgsetintvalue
	#define GET_settgsetintvalue(fl)  CAL_CMGETAPI( "settgsetintvalue" ) 
	#define CAL_settgsetintvalue  settgsetintvalue
	#define CHK_settgsetintvalue  TRUE
	#define EXP_settgsetintvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetintvalue", (RTS_UINTPTR)settgsetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0xBC5E022C), 0x03050700) 
#else /* DYNAMIC_LINK */
	#define USE_settgsetintvalue  PFSETTGSETINTVALUE_IEC pfsettgsetintvalue;
	#define EXT_settgsetintvalue  extern PFSETTGSETINTVALUE_IEC pfsettgsetintvalue;
	#define GET_settgsetintvalue(fl)  s_pfCMGetAPI2( "settgsetintvalue", (RTS_VOID_FCTPTR *)&pfsettgsetintvalue, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xBC5E022C), 0x03050700)
	#define CAL_settgsetintvalue  pfsettgsetintvalue
	#define CHK_settgsetintvalue  (pfsettgsetintvalue != NULL)
	#define EXP_settgsetintvalue   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetintvalue", (RTS_UINTPTR)settgsetintvalue, 1, RTSITF_GET_SIGNATURE(0, 0xBC5E022C), 0x03050700) 
#endif


/**
 * | Write a string value to settings
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsettgsetstringvalue_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	/* Name of component */
	RTS_IEC_STRING *pszKey;				/* VAR_INPUT */	/* Name of key */
	RTS_IEC_STRING *pszValue;			/* VAR_INPUT */	/* Pointer to write value */
	RTS_IEC_DINT iLen;					/* VAR_INPUT */	/* Length of string to write */
	RTS_IEC_RESULT SettgSetStringValue;	/* VAR_OUTPUT */	
} settgsetstringvalue_struct;

void CDECL CDECL_EXT settgsetstringvalue(settgsetstringvalue_struct *p);
typedef void (CDECL CDECL_EXT* PFSETTGSETSTRINGVALUE_IEC) (settgsetstringvalue_struct *p);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGSETSTRINGVALUE_NOTIMPLEMENTED)
	#define USE_settgsetstringvalue
	#define EXT_settgsetstringvalue
	#define GET_settgsetstringvalue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_settgsetstringvalue(p0) 
	#define CHK_settgsetstringvalue  FALSE
	#define EXP_settgsetstringvalue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_settgsetstringvalue
	#define EXT_settgsetstringvalue
	#define GET_settgsetstringvalue(fl)  CAL_CMGETAPI( "settgsetstringvalue" ) 
	#define CAL_settgsetstringvalue  settgsetstringvalue
	#define CHK_settgsetstringvalue  TRUE
	#define EXP_settgsetstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetstringvalue", (RTS_UINTPTR)settgsetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x63FA6ABA), 0x03050700) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_settgsetstringvalue
	#define EXT_settgsetstringvalue
	#define GET_settgsetstringvalue(fl)  CAL_CMGETAPI( "settgsetstringvalue" ) 
	#define CAL_settgsetstringvalue  settgsetstringvalue
	#define CHK_settgsetstringvalue  TRUE
	#define EXP_settgsetstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetstringvalue", (RTS_UINTPTR)settgsetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x63FA6ABA), 0x03050700) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingssettgsetstringvalue
	#define EXT_CmpSettingssettgsetstringvalue
	#define GET_CmpSettingssettgsetstringvalue  ERR_OK
	#define CAL_CmpSettingssettgsetstringvalue  settgsetstringvalue
	#define CHK_CmpSettingssettgsetstringvalue  TRUE
	#define EXP_CmpSettingssettgsetstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetstringvalue", (RTS_UINTPTR)settgsetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x63FA6ABA), 0x03050700) 
#elif defined(CPLUSPLUS)
	#define USE_settgsetstringvalue
	#define EXT_settgsetstringvalue
	#define GET_settgsetstringvalue(fl)  CAL_CMGETAPI( "settgsetstringvalue" ) 
	#define CAL_settgsetstringvalue  settgsetstringvalue
	#define CHK_settgsetstringvalue  TRUE
	#define EXP_settgsetstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetstringvalue", (RTS_UINTPTR)settgsetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x63FA6ABA), 0x03050700) 
#else /* DYNAMIC_LINK */
	#define USE_settgsetstringvalue  PFSETTGSETSTRINGVALUE_IEC pfsettgsetstringvalue;
	#define EXT_settgsetstringvalue  extern PFSETTGSETSTRINGVALUE_IEC pfsettgsetstringvalue;
	#define GET_settgsetstringvalue(fl)  s_pfCMGetAPI2( "settgsetstringvalue", (RTS_VOID_FCTPTR *)&pfsettgsetstringvalue, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x63FA6ABA), 0x03050700)
	#define CAL_settgsetstringvalue  pfsettgsetstringvalue
	#define CHK_settgsetstringvalue  (pfsettgsetstringvalue != NULL)
	#define EXP_settgsetstringvalue   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetstringvalue", (RTS_UINTPTR)settgsetstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x63FA6ABA), 0x03050700) 
#endif


/**
 * | Write a WSTRING value to settings
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsettgsetwstringvalue_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	/* Name of component */
	RTS_IEC_STRING *pszKey;				/* VAR_INPUT */	/* Name of key */
	RTS_IEC_WSTRING *pwszValue;			/* VAR_INPUT */	/* Pointer to write value */
	RTS_IEC_DINT iLen;					/* VAR_INPUT */	/* Length of string in WSTRING characters (not bytes!) to write */
	RTS_IEC_UDINT SettgSetWStringValue;	/* VAR_OUTPUT */	
} settgsetwstringvalue_struct;

void CDECL CDECL_EXT settgsetwstringvalue(settgsetwstringvalue_struct *p);
typedef void (CDECL CDECL_EXT* PFSETTGSETWSTRINGVALUE_IEC) (settgsetwstringvalue_struct *p);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGSETWSTRINGVALUE_NOTIMPLEMENTED)
	#define USE_settgsetwstringvalue
	#define EXT_settgsetwstringvalue
	#define GET_settgsetwstringvalue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_settgsetwstringvalue(p0) 
	#define CHK_settgsetwstringvalue  FALSE
	#define EXP_settgsetwstringvalue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_settgsetwstringvalue
	#define EXT_settgsetwstringvalue
	#define GET_settgsetwstringvalue(fl)  CAL_CMGETAPI( "settgsetwstringvalue" ) 
	#define CAL_settgsetwstringvalue  settgsetwstringvalue
	#define CHK_settgsetwstringvalue  TRUE
	#define EXP_settgsetwstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetwstringvalue", (RTS_UINTPTR)settgsetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x61870FE4), 0x03050700) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_settgsetwstringvalue
	#define EXT_settgsetwstringvalue
	#define GET_settgsetwstringvalue(fl)  CAL_CMGETAPI( "settgsetwstringvalue" ) 
	#define CAL_settgsetwstringvalue  settgsetwstringvalue
	#define CHK_settgsetwstringvalue  TRUE
	#define EXP_settgsetwstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetwstringvalue", (RTS_UINTPTR)settgsetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x61870FE4), 0x03050700) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingssettgsetwstringvalue
	#define EXT_CmpSettingssettgsetwstringvalue
	#define GET_CmpSettingssettgsetwstringvalue  ERR_OK
	#define CAL_CmpSettingssettgsetwstringvalue  settgsetwstringvalue
	#define CHK_CmpSettingssettgsetwstringvalue  TRUE
	#define EXP_CmpSettingssettgsetwstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetwstringvalue", (RTS_UINTPTR)settgsetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x61870FE4), 0x03050700) 
#elif defined(CPLUSPLUS)
	#define USE_settgsetwstringvalue
	#define EXT_settgsetwstringvalue
	#define GET_settgsetwstringvalue(fl)  CAL_CMGETAPI( "settgsetwstringvalue" ) 
	#define CAL_settgsetwstringvalue  settgsetwstringvalue
	#define CHK_settgsetwstringvalue  TRUE
	#define EXP_settgsetwstringvalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetwstringvalue", (RTS_UINTPTR)settgsetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x61870FE4), 0x03050700) 
#else /* DYNAMIC_LINK */
	#define USE_settgsetwstringvalue  PFSETTGSETWSTRINGVALUE_IEC pfsettgsetwstringvalue;
	#define EXT_settgsetwstringvalue  extern PFSETTGSETWSTRINGVALUE_IEC pfsettgsetwstringvalue;
	#define GET_settgsetwstringvalue(fl)  s_pfCMGetAPI2( "settgsetwstringvalue", (RTS_VOID_FCTPTR *)&pfsettgsetwstringvalue, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x61870FE4), 0x03050700)
	#define CAL_settgsetwstringvalue  pfsettgsetwstringvalue
	#define CHK_settgsetwstringvalue  (pfsettgsetwstringvalue != NULL)
	#define EXP_settgsetwstringvalue   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"settgsetwstringvalue", (RTS_UINTPTR)settgsetwstringvalue, 1, RTSITF_GET_SIGNATURE(0, 0x61870FE4), 0x03050700) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

 
/**
 * <description>Set database name for all settings</description>
 * <param name="pszName" type="IN">Name of settings file. Can be *.cfg or *.ini</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL SettgSetDatabaseName(char *pszName);
typedef RTS_RESULT (CDECL * PFSETTGSETDATABASENAME) (char *pszName);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGSETDATABASENAME_NOTIMPLEMENTED)
	#define USE_SettgSetDatabaseName
	#define EXT_SettgSetDatabaseName
	#define GET_SettgSetDatabaseName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgSetDatabaseName(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgSetDatabaseName  FALSE
	#define EXP_SettgSetDatabaseName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgSetDatabaseName
	#define EXT_SettgSetDatabaseName
	#define GET_SettgSetDatabaseName(fl)  CAL_CMGETAPI( "SettgSetDatabaseName" ) 
	#define CAL_SettgSetDatabaseName  SettgSetDatabaseName
	#define CHK_SettgSetDatabaseName  TRUE
	#define EXP_SettgSetDatabaseName  CAL_CMEXPAPI( "SettgSetDatabaseName" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgSetDatabaseName
	#define EXT_SettgSetDatabaseName
	#define GET_SettgSetDatabaseName(fl)  CAL_CMGETAPI( "SettgSetDatabaseName" ) 
	#define CAL_SettgSetDatabaseName  SettgSetDatabaseName
	#define CHK_SettgSetDatabaseName  TRUE
	#define EXP_SettgSetDatabaseName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetDatabaseName", (RTS_UINTPTR)SettgSetDatabaseName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgSetDatabaseName
	#define EXT_CmpSettingsSettgSetDatabaseName
	#define GET_CmpSettingsSettgSetDatabaseName  ERR_OK
	#define CAL_CmpSettingsSettgSetDatabaseName pICmpSettings->ISettgSetDatabaseName
	#define CHK_CmpSettingsSettgSetDatabaseName (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgSetDatabaseName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgSetDatabaseName
	#define EXT_SettgSetDatabaseName
	#define GET_SettgSetDatabaseName(fl)  CAL_CMGETAPI( "SettgSetDatabaseName" ) 
	#define CAL_SettgSetDatabaseName pICmpSettings->ISettgSetDatabaseName
	#define CHK_SettgSetDatabaseName (pICmpSettings != NULL)
	#define EXP_SettgSetDatabaseName  CAL_CMEXPAPI( "SettgSetDatabaseName" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgSetDatabaseName  PFSETTGSETDATABASENAME pfSettgSetDatabaseName;
	#define EXT_SettgSetDatabaseName  extern PFSETTGSETDATABASENAME pfSettgSetDatabaseName;
	#define GET_SettgSetDatabaseName(fl)  s_pfCMGetAPI2( "SettgSetDatabaseName", (RTS_VOID_FCTPTR *)&pfSettgSetDatabaseName, (fl), 0, 0)
	#define CAL_SettgSetDatabaseName  pfSettgSetDatabaseName
	#define CHK_SettgSetDatabaseName  (pfSettgSetDatabaseName != NULL)
	#define EXP_SettgSetDatabaseName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetDatabaseName", (RTS_UINTPTR)SettgSetDatabaseName, 0, 0) 
#endif




/**
 * <description>Get database name for all settings</description>
 * <param name="pszName" type="OUT">Name of settings file. Can be *.cfg or *.ini</param>
 * <param name="nNameLen" type="IN">Lenght of the name buffer in bytes</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL SettgGetDatabaseName(char *pszName, RTS_SIZE nNameLen);
typedef RTS_RESULT (CDECL * PFSETTGGETDATABASENAME) (char *pszName, RTS_SIZE nNameLen);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGGETDATABASENAME_NOTIMPLEMENTED)
	#define USE_SettgGetDatabaseName
	#define EXT_SettgGetDatabaseName
	#define GET_SettgGetDatabaseName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgGetDatabaseName(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgGetDatabaseName  FALSE
	#define EXP_SettgGetDatabaseName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgGetDatabaseName
	#define EXT_SettgGetDatabaseName
	#define GET_SettgGetDatabaseName(fl)  CAL_CMGETAPI( "SettgGetDatabaseName" ) 
	#define CAL_SettgGetDatabaseName  SettgGetDatabaseName
	#define CHK_SettgGetDatabaseName  TRUE
	#define EXP_SettgGetDatabaseName  CAL_CMEXPAPI( "SettgGetDatabaseName" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgGetDatabaseName
	#define EXT_SettgGetDatabaseName
	#define GET_SettgGetDatabaseName(fl)  CAL_CMGETAPI( "SettgGetDatabaseName" ) 
	#define CAL_SettgGetDatabaseName  SettgGetDatabaseName
	#define CHK_SettgGetDatabaseName  TRUE
	#define EXP_SettgGetDatabaseName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetDatabaseName", (RTS_UINTPTR)SettgGetDatabaseName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgGetDatabaseName
	#define EXT_CmpSettingsSettgGetDatabaseName
	#define GET_CmpSettingsSettgGetDatabaseName  ERR_OK
	#define CAL_CmpSettingsSettgGetDatabaseName pICmpSettings->ISettgGetDatabaseName
	#define CHK_CmpSettingsSettgGetDatabaseName (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgGetDatabaseName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgGetDatabaseName
	#define EXT_SettgGetDatabaseName
	#define GET_SettgGetDatabaseName(fl)  CAL_CMGETAPI( "SettgGetDatabaseName" ) 
	#define CAL_SettgGetDatabaseName pICmpSettings->ISettgGetDatabaseName
	#define CHK_SettgGetDatabaseName (pICmpSettings != NULL)
	#define EXP_SettgGetDatabaseName  CAL_CMEXPAPI( "SettgGetDatabaseName" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgGetDatabaseName  PFSETTGGETDATABASENAME pfSettgGetDatabaseName;
	#define EXT_SettgGetDatabaseName  extern PFSETTGGETDATABASENAME pfSettgGetDatabaseName;
	#define GET_SettgGetDatabaseName(fl)  s_pfCMGetAPI2( "SettgGetDatabaseName", (RTS_VOID_FCTPTR *)&pfSettgGetDatabaseName, (fl), 0, 0)
	#define CAL_SettgGetDatabaseName  pfSettgGetDatabaseName
	#define CHK_SettgGetDatabaseName  (pfSettgGetDatabaseName != NULL)
	#define EXP_SettgGetDatabaseName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetDatabaseName", (RTS_UINTPTR)SettgGetDatabaseName, 0, 0) 
#endif




/**
 * <description>Function to block writing settings to database file until SettgEndUpdate() is called! But the settings cache is updated. 
 * NOTE:
 * - Funcion can be called nested, so only at the last call of SettgEndUpdate(), the database is written!
 * - Always call SettgBeginUpdate() and SettgEndUpdate() symmetric!!!
 * </description>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Task found</errorcode>
 */
RTS_RESULT CDECL SettgBeginUpdate(void);
typedef RTS_RESULT (CDECL * PFSETTGBEGINUPDATE) (void);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGBEGINUPDATE_NOTIMPLEMENTED)
	#define USE_SettgBeginUpdate
	#define EXT_SettgBeginUpdate
	#define GET_SettgBeginUpdate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgBeginUpdate()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgBeginUpdate  FALSE
	#define EXP_SettgBeginUpdate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgBeginUpdate
	#define EXT_SettgBeginUpdate
	#define GET_SettgBeginUpdate(fl)  CAL_CMGETAPI( "SettgBeginUpdate" ) 
	#define CAL_SettgBeginUpdate  SettgBeginUpdate
	#define CHK_SettgBeginUpdate  TRUE
	#define EXP_SettgBeginUpdate  CAL_CMEXPAPI( "SettgBeginUpdate" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgBeginUpdate
	#define EXT_SettgBeginUpdate
	#define GET_SettgBeginUpdate(fl)  CAL_CMGETAPI( "SettgBeginUpdate" ) 
	#define CAL_SettgBeginUpdate  SettgBeginUpdate
	#define CHK_SettgBeginUpdate  TRUE
	#define EXP_SettgBeginUpdate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgBeginUpdate", (RTS_UINTPTR)SettgBeginUpdate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgBeginUpdate
	#define EXT_CmpSettingsSettgBeginUpdate
	#define GET_CmpSettingsSettgBeginUpdate  ERR_OK
	#define CAL_CmpSettingsSettgBeginUpdate pICmpSettings->ISettgBeginUpdate
	#define CHK_CmpSettingsSettgBeginUpdate (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgBeginUpdate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgBeginUpdate
	#define EXT_SettgBeginUpdate
	#define GET_SettgBeginUpdate(fl)  CAL_CMGETAPI( "SettgBeginUpdate" ) 
	#define CAL_SettgBeginUpdate pICmpSettings->ISettgBeginUpdate
	#define CHK_SettgBeginUpdate (pICmpSettings != NULL)
	#define EXP_SettgBeginUpdate  CAL_CMEXPAPI( "SettgBeginUpdate" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgBeginUpdate  PFSETTGBEGINUPDATE pfSettgBeginUpdate;
	#define EXT_SettgBeginUpdate  extern PFSETTGBEGINUPDATE pfSettgBeginUpdate;
	#define GET_SettgBeginUpdate(fl)  s_pfCMGetAPI2( "SettgBeginUpdate", (RTS_VOID_FCTPTR *)&pfSettgBeginUpdate, (fl), 0, 0)
	#define CAL_SettgBeginUpdate  pfSettgBeginUpdate
	#define CHK_SettgBeginUpdate  (pfSettgBeginUpdate != NULL)
	#define EXP_SettgBeginUpdate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgBeginUpdate", (RTS_UINTPTR)SettgBeginUpdate, 0, 0) 
#endif




/**
 * <description>Function to initiate writing settings to database file, if settings are modified after SettgBeginUpdate() was called.</description>
 * NOTE:
 * - Funcion can be called nested, so only at the last call of SettgEndUpdate(), the database is written!
 * - Always call SettgBeginUpdate() and SettgEndUpdate() symmetric!!!
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Database could be written</errorcode>
 * <errorcode name="RTS_RESULT" type="RTS_FAILED">Database could not be written</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PENDING">We still wait for the last call of SettgEndUpdate()</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">SettgBeginUpdate() not called before</errorcode>
 */
RTS_RESULT CDECL SettgEndUpdate(void);
typedef RTS_RESULT (CDECL * PFSETTGENDUPDATE) (void);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGENDUPDATE_NOTIMPLEMENTED)
	#define USE_SettgEndUpdate
	#define EXT_SettgEndUpdate
	#define GET_SettgEndUpdate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgEndUpdate()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgEndUpdate  FALSE
	#define EXP_SettgEndUpdate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgEndUpdate
	#define EXT_SettgEndUpdate
	#define GET_SettgEndUpdate(fl)  CAL_CMGETAPI( "SettgEndUpdate" ) 
	#define CAL_SettgEndUpdate  SettgEndUpdate
	#define CHK_SettgEndUpdate  TRUE
	#define EXP_SettgEndUpdate  CAL_CMEXPAPI( "SettgEndUpdate" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgEndUpdate
	#define EXT_SettgEndUpdate
	#define GET_SettgEndUpdate(fl)  CAL_CMGETAPI( "SettgEndUpdate" ) 
	#define CAL_SettgEndUpdate  SettgEndUpdate
	#define CHK_SettgEndUpdate  TRUE
	#define EXP_SettgEndUpdate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgEndUpdate", (RTS_UINTPTR)SettgEndUpdate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgEndUpdate
	#define EXT_CmpSettingsSettgEndUpdate
	#define GET_CmpSettingsSettgEndUpdate  ERR_OK
	#define CAL_CmpSettingsSettgEndUpdate pICmpSettings->ISettgEndUpdate
	#define CHK_CmpSettingsSettgEndUpdate (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgEndUpdate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgEndUpdate
	#define EXT_SettgEndUpdate
	#define GET_SettgEndUpdate(fl)  CAL_CMGETAPI( "SettgEndUpdate" ) 
	#define CAL_SettgEndUpdate pICmpSettings->ISettgEndUpdate
	#define CHK_SettgEndUpdate (pICmpSettings != NULL)
	#define EXP_SettgEndUpdate  CAL_CMEXPAPI( "SettgEndUpdate" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgEndUpdate  PFSETTGENDUPDATE pfSettgEndUpdate;
	#define EXT_SettgEndUpdate  extern PFSETTGENDUPDATE pfSettgEndUpdate;
	#define GET_SettgEndUpdate(fl)  s_pfCMGetAPI2( "SettgEndUpdate", (RTS_VOID_FCTPTR *)&pfSettgEndUpdate, (fl), 0, 0)
	#define CAL_SettgEndUpdate  pfSettgEndUpdate
	#define CHK_SettgEndUpdate  (pfSettgEndUpdate != NULL)
	#define EXP_SettgEndUpdate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgEndUpdate", (RTS_UINTPTR)SettgEndUpdate, 0, 0) 
#endif




/**
 * <description>Internal function to get the accesscounter value which is incremented by every call of SettgBeginUpdate() and is decremented 
 *	by every call of SettgEndUpdate().
 * </description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="pResult" type="ERR_OK">Access counter can be retrieved</errorcode>
 * <result>Actual accesscounter value</result>
 */
RTS_I32 CDECL SettgGetAccessCounter_(RTS_RESULT *pResult);
typedef RTS_I32 (CDECL * PFSETTGGETACCESSCOUNTER_) (RTS_RESULT *pResult);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGGETACCESSCOUNTER__NOTIMPLEMENTED)
	#define USE_SettgGetAccessCounter_
	#define EXT_SettgGetAccessCounter_
	#define GET_SettgGetAccessCounter_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgGetAccessCounter_(p0)  (RTS_I32)ERR_NOTIMPLEMENTED
	#define CHK_SettgGetAccessCounter_  FALSE
	#define EXP_SettgGetAccessCounter_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgGetAccessCounter_
	#define EXT_SettgGetAccessCounter_
	#define GET_SettgGetAccessCounter_(fl)  CAL_CMGETAPI( "SettgGetAccessCounter_" ) 
	#define CAL_SettgGetAccessCounter_  SettgGetAccessCounter_
	#define CHK_SettgGetAccessCounter_  TRUE
	#define EXP_SettgGetAccessCounter_  CAL_CMEXPAPI( "SettgGetAccessCounter_" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgGetAccessCounter_
	#define EXT_SettgGetAccessCounter_
	#define GET_SettgGetAccessCounter_(fl)  CAL_CMGETAPI( "SettgGetAccessCounter_" ) 
	#define CAL_SettgGetAccessCounter_  SettgGetAccessCounter_
	#define CHK_SettgGetAccessCounter_  TRUE
	#define EXP_SettgGetAccessCounter_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetAccessCounter_", (RTS_UINTPTR)SettgGetAccessCounter_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgGetAccessCounter_
	#define EXT_CmpSettingsSettgGetAccessCounter_
	#define GET_CmpSettingsSettgGetAccessCounter_  ERR_OK
	#define CAL_CmpSettingsSettgGetAccessCounter_ pICmpSettings->ISettgGetAccessCounter_
	#define CHK_CmpSettingsSettgGetAccessCounter_ (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgGetAccessCounter_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgGetAccessCounter_
	#define EXT_SettgGetAccessCounter_
	#define GET_SettgGetAccessCounter_(fl)  CAL_CMGETAPI( "SettgGetAccessCounter_" ) 
	#define CAL_SettgGetAccessCounter_ pICmpSettings->ISettgGetAccessCounter_
	#define CHK_SettgGetAccessCounter_ (pICmpSettings != NULL)
	#define EXP_SettgGetAccessCounter_  CAL_CMEXPAPI( "SettgGetAccessCounter_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgGetAccessCounter_  PFSETTGGETACCESSCOUNTER_ pfSettgGetAccessCounter_;
	#define EXT_SettgGetAccessCounter_  extern PFSETTGGETACCESSCOUNTER_ pfSettgGetAccessCounter_;
	#define GET_SettgGetAccessCounter_(fl)  s_pfCMGetAPI2( "SettgGetAccessCounter_", (RTS_VOID_FCTPTR *)&pfSettgGetAccessCounter_, (fl), 0, 0)
	#define CAL_SettgGetAccessCounter_  pfSettgGetAccessCounter_
	#define CHK_SettgGetAccessCounter_  (pfSettgGetAccessCounter_ != NULL)
	#define EXP_SettgGetAccessCounter_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetAccessCounter_", (RTS_UINTPTR)SettgGetAccessCounter_, 0, 0) 
#endif




/**
 * <description>Set optional settings</description>
 * <param name="bSeparateDatabases" type="IN">If bSeparateDatabases=1, split settings file into
 *	each file per component</param>
 * <result>ERR_OK</result>
 */
RTS_RESULT CDECL SettgSetOptions(int bSplitDatabases);
typedef RTS_RESULT (CDECL * PFSETTGSETOPTIONS) (int bSplitDatabases);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGSETOPTIONS_NOTIMPLEMENTED)
	#define USE_SettgSetOptions
	#define EXT_SettgSetOptions
	#define GET_SettgSetOptions(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgSetOptions(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgSetOptions  FALSE
	#define EXP_SettgSetOptions  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgSetOptions
	#define EXT_SettgSetOptions
	#define GET_SettgSetOptions(fl)  CAL_CMGETAPI( "SettgSetOptions" ) 
	#define CAL_SettgSetOptions  SettgSetOptions
	#define CHK_SettgSetOptions  TRUE
	#define EXP_SettgSetOptions  CAL_CMEXPAPI( "SettgSetOptions" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgSetOptions
	#define EXT_SettgSetOptions
	#define GET_SettgSetOptions(fl)  CAL_CMGETAPI( "SettgSetOptions" ) 
	#define CAL_SettgSetOptions  SettgSetOptions
	#define CHK_SettgSetOptions  TRUE
	#define EXP_SettgSetOptions  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetOptions", (RTS_UINTPTR)SettgSetOptions, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgSetOptions
	#define EXT_CmpSettingsSettgSetOptions
	#define GET_CmpSettingsSettgSetOptions  ERR_OK
	#define CAL_CmpSettingsSettgSetOptions pICmpSettings->ISettgSetOptions
	#define CHK_CmpSettingsSettgSetOptions (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgSetOptions  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgSetOptions
	#define EXT_SettgSetOptions
	#define GET_SettgSetOptions(fl)  CAL_CMGETAPI( "SettgSetOptions" ) 
	#define CAL_SettgSetOptions pICmpSettings->ISettgSetOptions
	#define CHK_SettgSetOptions (pICmpSettings != NULL)
	#define EXP_SettgSetOptions  CAL_CMEXPAPI( "SettgSetOptions" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgSetOptions  PFSETTGSETOPTIONS pfSettgSetOptions;
	#define EXT_SettgSetOptions  extern PFSETTGSETOPTIONS pfSettgSetOptions;
	#define GET_SettgSetOptions(fl)  s_pfCMGetAPI2( "SettgSetOptions", (RTS_VOID_FCTPTR *)&pfSettgSetOptions, (fl), 0, 0)
	#define CAL_SettgSetOptions  pfSettgSetOptions
	#define CHK_SettgSetOptions  (pfSettgSetOptions != NULL)
	#define EXP_SettgSetOptions  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetOptions", (RTS_UINTPTR)SettgSetOptions, 0, 0) 
#endif




/**
 * <description>Is option set to split all settings databases into each file per component</description>
 * <result>ERR_OK</result>
 */
RTS_RESULT CDECL SettgIsOptionSplitDatabases(void);
typedef RTS_RESULT (CDECL * PFSETTGISOPTIONSPLITDATABASES) (void);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGISOPTIONSPLITDATABASES_NOTIMPLEMENTED)
	#define USE_SettgIsOptionSplitDatabases
	#define EXT_SettgIsOptionSplitDatabases
	#define GET_SettgIsOptionSplitDatabases(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgIsOptionSplitDatabases()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgIsOptionSplitDatabases  FALSE
	#define EXP_SettgIsOptionSplitDatabases  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgIsOptionSplitDatabases
	#define EXT_SettgIsOptionSplitDatabases
	#define GET_SettgIsOptionSplitDatabases(fl)  CAL_CMGETAPI( "SettgIsOptionSplitDatabases" ) 
	#define CAL_SettgIsOptionSplitDatabases  SettgIsOptionSplitDatabases
	#define CHK_SettgIsOptionSplitDatabases  TRUE
	#define EXP_SettgIsOptionSplitDatabases  CAL_CMEXPAPI( "SettgIsOptionSplitDatabases" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgIsOptionSplitDatabases
	#define EXT_SettgIsOptionSplitDatabases
	#define GET_SettgIsOptionSplitDatabases(fl)  CAL_CMGETAPI( "SettgIsOptionSplitDatabases" ) 
	#define CAL_SettgIsOptionSplitDatabases  SettgIsOptionSplitDatabases
	#define CHK_SettgIsOptionSplitDatabases  TRUE
	#define EXP_SettgIsOptionSplitDatabases  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgIsOptionSplitDatabases", (RTS_UINTPTR)SettgIsOptionSplitDatabases, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgIsOptionSplitDatabases
	#define EXT_CmpSettingsSettgIsOptionSplitDatabases
	#define GET_CmpSettingsSettgIsOptionSplitDatabases  ERR_OK
	#define CAL_CmpSettingsSettgIsOptionSplitDatabases pICmpSettings->ISettgIsOptionSplitDatabases
	#define CHK_CmpSettingsSettgIsOptionSplitDatabases (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgIsOptionSplitDatabases  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgIsOptionSplitDatabases
	#define EXT_SettgIsOptionSplitDatabases
	#define GET_SettgIsOptionSplitDatabases(fl)  CAL_CMGETAPI( "SettgIsOptionSplitDatabases" ) 
	#define CAL_SettgIsOptionSplitDatabases pICmpSettings->ISettgIsOptionSplitDatabases
	#define CHK_SettgIsOptionSplitDatabases (pICmpSettings != NULL)
	#define EXP_SettgIsOptionSplitDatabases  CAL_CMEXPAPI( "SettgIsOptionSplitDatabases" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgIsOptionSplitDatabases  PFSETTGISOPTIONSPLITDATABASES pfSettgIsOptionSplitDatabases;
	#define EXT_SettgIsOptionSplitDatabases  extern PFSETTGISOPTIONSPLITDATABASES pfSettgIsOptionSplitDatabases;
	#define GET_SettgIsOptionSplitDatabases(fl)  s_pfCMGetAPI2( "SettgIsOptionSplitDatabases", (RTS_VOID_FCTPTR *)&pfSettgIsOptionSplitDatabases, (fl), 0, 0)
	#define CAL_SettgIsOptionSplitDatabases  pfSettgIsOptionSplitDatabases
	#define CHK_SettgIsOptionSplitDatabases  (pfSettgIsOptionSplitDatabases != NULL)
	#define EXP_SettgIsOptionSplitDatabases  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgIsOptionSplitDatabases", (RTS_UINTPTR)SettgIsOptionSplitDatabases, 0, 0) 
#endif




/**
 * <description>Get an integer value from settings</description>
 * <param name="pszComponent" type="IN">Name of component</param>
 * <param name="pszKey" type="IN">Name of key</param>
 * <param name="piValue" type="INOUT">Pointer to value for result</param>
 * <param name="iDefault" type="IN">Default value to set, if key not found</param>
 * <param name="bCached" type="IN">Flag, if value should be read cached or direct from file (CMPSETTINGS_CACHED, CMPSETTINGS_NOT_CACHED)</param>
 * <result>ERR_OK</result>
 * <result>ERR_FAILED: Key not found</result>
 */
RTS_RESULT CDECL SettgGetIntValue(const char *pszComponent, const char *pszKey, RTS_I32 *piValue, int iDefault, int bCached);
typedef RTS_RESULT (CDECL * PFSETTGGETINTVALUE) (const char *pszComponent, const char *pszKey, RTS_I32 *piValue, int iDefault, int bCached);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGGETINTVALUE_NOTIMPLEMENTED)
	#define USE_SettgGetIntValue
	#define EXT_SettgGetIntValue
	#define GET_SettgGetIntValue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgGetIntValue(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgGetIntValue  FALSE
	#define EXP_SettgGetIntValue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgGetIntValue
	#define EXT_SettgGetIntValue
	#define GET_SettgGetIntValue(fl)  CAL_CMGETAPI( "SettgGetIntValue" ) 
	#define CAL_SettgGetIntValue  SettgGetIntValue
	#define CHK_SettgGetIntValue  TRUE
	#define EXP_SettgGetIntValue  CAL_CMEXPAPI( "SettgGetIntValue" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgGetIntValue
	#define EXT_SettgGetIntValue
	#define GET_SettgGetIntValue(fl)  CAL_CMGETAPI( "SettgGetIntValue" ) 
	#define CAL_SettgGetIntValue  SettgGetIntValue
	#define CHK_SettgGetIntValue  TRUE
	#define EXP_SettgGetIntValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetIntValue", (RTS_UINTPTR)SettgGetIntValue, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgGetIntValue
	#define EXT_CmpSettingsSettgGetIntValue
	#define GET_CmpSettingsSettgGetIntValue  ERR_OK
	#define CAL_CmpSettingsSettgGetIntValue pICmpSettings->ISettgGetIntValue
	#define CHK_CmpSettingsSettgGetIntValue (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgGetIntValue  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgGetIntValue
	#define EXT_SettgGetIntValue
	#define GET_SettgGetIntValue(fl)  CAL_CMGETAPI( "SettgGetIntValue" ) 
	#define CAL_SettgGetIntValue pICmpSettings->ISettgGetIntValue
	#define CHK_SettgGetIntValue (pICmpSettings != NULL)
	#define EXP_SettgGetIntValue  CAL_CMEXPAPI( "SettgGetIntValue" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgGetIntValue  PFSETTGGETINTVALUE pfSettgGetIntValue;
	#define EXT_SettgGetIntValue  extern PFSETTGGETINTVALUE pfSettgGetIntValue;
	#define GET_SettgGetIntValue(fl)  s_pfCMGetAPI2( "SettgGetIntValue", (RTS_VOID_FCTPTR *)&pfSettgGetIntValue, (fl), 0, 0)
	#define CAL_SettgGetIntValue  pfSettgGetIntValue
	#define CHK_SettgGetIntValue  (pfSettgGetIntValue != NULL)
	#define EXP_SettgGetIntValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetIntValue", (RTS_UINTPTR)SettgGetIntValue, 0, 0) 
#endif




/**
 * <description>Write an integer value to settings</description>
 * <param name="pszComponent" type="IN">Name of component</param>
 * <param name="pszKey" type="IN">Name of key</param>
 * <param name="iValue" type="IN">Value to write</param>
 * <param name="iBase" type="IN">2=Base 2, 10=Decimal values, 16=Hex values</param>
 * <result>ERR_OK</result>
 */
RTS_RESULT CDECL SettgSetIntValue(const char *pszComponent, const char *pszKey, RTS_I32 iValue, int iBase);
typedef RTS_RESULT (CDECL * PFSETTGSETINTVALUE) (const char *pszComponent, const char *pszKey, RTS_I32 iValue, int iBase);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGSETINTVALUE_NOTIMPLEMENTED)
	#define USE_SettgSetIntValue
	#define EXT_SettgSetIntValue
	#define GET_SettgSetIntValue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgSetIntValue(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgSetIntValue  FALSE
	#define EXP_SettgSetIntValue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgSetIntValue
	#define EXT_SettgSetIntValue
	#define GET_SettgSetIntValue(fl)  CAL_CMGETAPI( "SettgSetIntValue" ) 
	#define CAL_SettgSetIntValue  SettgSetIntValue
	#define CHK_SettgSetIntValue  TRUE
	#define EXP_SettgSetIntValue  CAL_CMEXPAPI( "SettgSetIntValue" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgSetIntValue
	#define EXT_SettgSetIntValue
	#define GET_SettgSetIntValue(fl)  CAL_CMGETAPI( "SettgSetIntValue" ) 
	#define CAL_SettgSetIntValue  SettgSetIntValue
	#define CHK_SettgSetIntValue  TRUE
	#define EXP_SettgSetIntValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetIntValue", (RTS_UINTPTR)SettgSetIntValue, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgSetIntValue
	#define EXT_CmpSettingsSettgSetIntValue
	#define GET_CmpSettingsSettgSetIntValue  ERR_OK
	#define CAL_CmpSettingsSettgSetIntValue pICmpSettings->ISettgSetIntValue
	#define CHK_CmpSettingsSettgSetIntValue (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgSetIntValue  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgSetIntValue
	#define EXT_SettgSetIntValue
	#define GET_SettgSetIntValue(fl)  CAL_CMGETAPI( "SettgSetIntValue" ) 
	#define CAL_SettgSetIntValue pICmpSettings->ISettgSetIntValue
	#define CHK_SettgSetIntValue (pICmpSettings != NULL)
	#define EXP_SettgSetIntValue  CAL_CMEXPAPI( "SettgSetIntValue" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgSetIntValue  PFSETTGSETINTVALUE pfSettgSetIntValue;
	#define EXT_SettgSetIntValue  extern PFSETTGSETINTVALUE pfSettgSetIntValue;
	#define GET_SettgSetIntValue(fl)  s_pfCMGetAPI2( "SettgSetIntValue", (RTS_VOID_FCTPTR *)&pfSettgSetIntValue, (fl), 0, 0)
	#define CAL_SettgSetIntValue  pfSettgSetIntValue
	#define CHK_SettgSetIntValue  (pfSettgSetIntValue != NULL)
	#define EXP_SettgSetIntValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetIntValue", (RTS_UINTPTR)SettgSetIntValue, 0, 0) 
#endif




/**
 * <description>Get a string value from settings</description>
 *	The following parameter sets are typical use cases:
 *  - pszValue = NULL, piLen = NULL: Check if the setting exists (ERR_OK) or not (ERR_FAILED).
 *	- pszValue = NULL, piLen != NULL: Check if the setting exists (ERR_OK) or not (ERR_FAILED) and retrieve the needed buffer size
 *                                    to read the setting or the default value (including NUL termination).
 *  - pszValue != NULL, piLen != NULL: Read the setting (or default value). If ERR_BUFFERSIZE is returned, the buffer was too small
 *                                     regardless of whether the setting or the default value was read. Thus in this case there is
 *                                     no indication, if the setting exists or not.
 * <param name="pszComponent" type="IN">Name of component</param>
 * <param name="pszKey" type="IN">Name of key</param>
 * <param name="pszValue" type="INOUT">Pointer to value for result. Can be NULL to get the needed buffer length for the value (including the NUL ending).</param>
 * <param name="piLen" type="INOUT">Max length of string value as IN and number of copied characters excluding the NUL ending as OUT. If pszValue is NULL, then the needed buffer length is returned.</param>
 * <param name="pszDefault" type="IN">Default value to set, if key not found</param>
 * <param name="bCached" type="IN">Flag, if value should be read cached or direct from file (CMPSETTINGS_CACHED, CMPSETTINGS_NOT_CACHED)</param>
 * <result>ERR_OK</result>
 * <result>ERR_PARAMETER: Invalid parameter given</result>
 * <result>ERR_BUFFERSIZE: Buffer size is not sufficient to carry out the complete key; only a part is returned.</result>
 * <result>ERR_FAILED: Key not found</result>
 */
RTS_RESULT CDECL SettgGetStringValue(const char *pszComponent, const char *pszKey, char *pszValue, RTS_I32 *piLen, char *pszDefault, int bCached);
typedef RTS_RESULT (CDECL * PFSETTGGETSTRINGVALUE) (const char *pszComponent, const char *pszKey, char *pszValue, RTS_I32 *piLen, char *pszDefault, int bCached);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGGETSTRINGVALUE_NOTIMPLEMENTED)
	#define USE_SettgGetStringValue
	#define EXT_SettgGetStringValue
	#define GET_SettgGetStringValue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgGetStringValue(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgGetStringValue  FALSE
	#define EXP_SettgGetStringValue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgGetStringValue
	#define EXT_SettgGetStringValue
	#define GET_SettgGetStringValue(fl)  CAL_CMGETAPI( "SettgGetStringValue" ) 
	#define CAL_SettgGetStringValue  SettgGetStringValue
	#define CHK_SettgGetStringValue  TRUE
	#define EXP_SettgGetStringValue  CAL_CMEXPAPI( "SettgGetStringValue" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgGetStringValue
	#define EXT_SettgGetStringValue
	#define GET_SettgGetStringValue(fl)  CAL_CMGETAPI( "SettgGetStringValue" ) 
	#define CAL_SettgGetStringValue  SettgGetStringValue
	#define CHK_SettgGetStringValue  TRUE
	#define EXP_SettgGetStringValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetStringValue", (RTS_UINTPTR)SettgGetStringValue, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgGetStringValue
	#define EXT_CmpSettingsSettgGetStringValue
	#define GET_CmpSettingsSettgGetStringValue  ERR_OK
	#define CAL_CmpSettingsSettgGetStringValue pICmpSettings->ISettgGetStringValue
	#define CHK_CmpSettingsSettgGetStringValue (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgGetStringValue  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgGetStringValue
	#define EXT_SettgGetStringValue
	#define GET_SettgGetStringValue(fl)  CAL_CMGETAPI( "SettgGetStringValue" ) 
	#define CAL_SettgGetStringValue pICmpSettings->ISettgGetStringValue
	#define CHK_SettgGetStringValue (pICmpSettings != NULL)
	#define EXP_SettgGetStringValue  CAL_CMEXPAPI( "SettgGetStringValue" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgGetStringValue  PFSETTGGETSTRINGVALUE pfSettgGetStringValue;
	#define EXT_SettgGetStringValue  extern PFSETTGGETSTRINGVALUE pfSettgGetStringValue;
	#define GET_SettgGetStringValue(fl)  s_pfCMGetAPI2( "SettgGetStringValue", (RTS_VOID_FCTPTR *)&pfSettgGetStringValue, (fl), 0, 0)
	#define CAL_SettgGetStringValue  pfSettgGetStringValue
	#define CHK_SettgGetStringValue  (pfSettgGetStringValue != NULL)
	#define EXP_SettgGetStringValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetStringValue", (RTS_UINTPTR)SettgGetStringValue, 0, 0) 
#endif




/**
 * <description>Write a string value to settings</description>
 * <param name="pszComponent" type="IN">Name of component</param>
 * <param name="pszKey" type="IN">Name of key</param>
 * <param name="pszValue" type="IN">Pointer to write value</param>
 * <param name="iLen" type="IN">Length of string to write excluding the NUL ending</param>
 * <result>ERR_OK: Success</result>
 * <result>ERR_PARAMETER: Invalid parameter given</result>
 * <result>ERR_NOTINITIALIZED: Component has not been initialized yet</result>
 * <result>ERR_NOMEMORY: Not enough memory to handle the settings cache</result> 
 * <result>ERR_NOT_SUPPORTED: Unsupported character in key value: '=', '[', ']' or control character 0x00-0x1F, 0x7F</result>
 * <result>ERR_NO_ACCESS_RIGHTS: No write access rights to corresponding settings file</result>
 * <result>ERR_FAILED: Any other reason for failure</result>
 */
RTS_RESULT CDECL SettgSetStringValue(const char *pszComponent, const char *pszKey, char *pszValue, RTS_I32 iLen);
typedef RTS_RESULT (CDECL * PFSETTGSETSTRINGVALUE) (const char *pszComponent, const char *pszKey, char *pszValue, RTS_I32 iLen);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGSETSTRINGVALUE_NOTIMPLEMENTED)
	#define USE_SettgSetStringValue
	#define EXT_SettgSetStringValue
	#define GET_SettgSetStringValue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgSetStringValue(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgSetStringValue  FALSE
	#define EXP_SettgSetStringValue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgSetStringValue
	#define EXT_SettgSetStringValue
	#define GET_SettgSetStringValue(fl)  CAL_CMGETAPI( "SettgSetStringValue" ) 
	#define CAL_SettgSetStringValue  SettgSetStringValue
	#define CHK_SettgSetStringValue  TRUE
	#define EXP_SettgSetStringValue  CAL_CMEXPAPI( "SettgSetStringValue" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgSetStringValue
	#define EXT_SettgSetStringValue
	#define GET_SettgSetStringValue(fl)  CAL_CMGETAPI( "SettgSetStringValue" ) 
	#define CAL_SettgSetStringValue  SettgSetStringValue
	#define CHK_SettgSetStringValue  TRUE
	#define EXP_SettgSetStringValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetStringValue", (RTS_UINTPTR)SettgSetStringValue, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgSetStringValue
	#define EXT_CmpSettingsSettgSetStringValue
	#define GET_CmpSettingsSettgSetStringValue  ERR_OK
	#define CAL_CmpSettingsSettgSetStringValue pICmpSettings->ISettgSetStringValue
	#define CHK_CmpSettingsSettgSetStringValue (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgSetStringValue  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgSetStringValue
	#define EXT_SettgSetStringValue
	#define GET_SettgSetStringValue(fl)  CAL_CMGETAPI( "SettgSetStringValue" ) 
	#define CAL_SettgSetStringValue pICmpSettings->ISettgSetStringValue
	#define CHK_SettgSetStringValue (pICmpSettings != NULL)
	#define EXP_SettgSetStringValue  CAL_CMEXPAPI( "SettgSetStringValue" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgSetStringValue  PFSETTGSETSTRINGVALUE pfSettgSetStringValue;
	#define EXT_SettgSetStringValue  extern PFSETTGSETSTRINGVALUE pfSettgSetStringValue;
	#define GET_SettgSetStringValue(fl)  s_pfCMGetAPI2( "SettgSetStringValue", (RTS_VOID_FCTPTR *)&pfSettgSetStringValue, (fl), 0, 0)
	#define CAL_SettgSetStringValue  pfSettgSetStringValue
	#define CHK_SettgSetStringValue  (pfSettgSetStringValue != NULL)
	#define EXP_SettgSetStringValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetStringValue", (RTS_UINTPTR)SettgSetStringValue, 0, 0) 
#endif




/**
 * <description> 
 *  Get a WSTRING value from settings.
 *  The storage format is conform to UTF-16, that means one character consists of 2 byte values like "\ab\cd".
 *  Plain ASCII characters can also be represented in a mixed way in order to keep the readability of the string, e.g. "a\00".
 *  The value is interpreted depending on the target byteorder, e.g:
 *   - INTEL: NodeNameUnicode="T\00e\00s\00t\00"
 *   - MOTOROLA: NodeNameUnicode="\00T\00e\00s\00t"
 *  Example: The unicode character 'DEGREE CELSIUS' (U+2103) is stored as 
 *   - INTEL: "\03\21"
 *   - MOTOROLA: "\21\03"
 *	The following parameter sets are typical use cases:
 *	- pwszValue = NULL, piLen = NULL: Check if the setting exists (ERR_OK) or not (ERR_FAILED).
 *	- pwszValue = NULL, piLen != NULL: Check if the setting exists (ERR_OK) or not (ERR_FAILED) and retrieve the needed buffer size
 *                                     to read the setting or the default value (in WSTRING characters including NUL termination).
 *  - pwszValue != NULL, piLen != NULL: Read the setting (or default value). If ERR_BUFFERSIZE is returned, the buffer was too small
 *                                      regardless of whether the setting or the default value was read. Thus in this case there is
 *                                      no indication, if the setting exists or not. 
 * </description>
 * <param name="pszComponent" type="IN">Name of component</param>
 * <param name="pszKey" type="IN">Name of key</param>
 * <param name="pwszValue" type="INOUT">Pointer to value for result. Can be NULL to get the needed buffer length for the value (including the NUL ending).</param>
 * <param name="piLen" type="INOUT">Max length of buffer in WSTRING characters (not bytes!) as IN and number of copied WSTRING characters excluding the NUL ending as OUT. If pwszValue is NULL, then the needed buffer length is returned.</param>
 * <param name="pwszDefault" type="IN">Default value to set, if key not found</param>
 * <param name="bCached" type="IN">Flag, if value should be read cached or direct from file (CMPSETTINGS_CACHED, CMPSETTINGS_NOT_CACHED)</param>
 * <result>ERR_OK</result>
 * <result>ERR_PARAMETER: Invalid parameter given</result>
 * <result>ERR_BUFFERSIZE: Buffer size is not sufficient to carry out the complete setting or default value; only a part is returned.</result>
 * <result>ERR_FAILED: Key not found</result>
 */
RTS_RESULT CDECL SettgGetWStringValue(const char *pszComponent, const char *pszKey, RTS_WCHAR *pwszValue, RTS_I32 *piLen, RTS_WCHAR *pwszDefault, int bCached);
typedef RTS_RESULT (CDECL * PFSETTGGETWSTRINGVALUE) (const char *pszComponent, const char *pszKey, RTS_WCHAR *pwszValue, RTS_I32 *piLen, RTS_WCHAR *pwszDefault, int bCached);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGGETWSTRINGVALUE_NOTIMPLEMENTED)
	#define USE_SettgGetWStringValue
	#define EXT_SettgGetWStringValue
	#define GET_SettgGetWStringValue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgGetWStringValue(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgGetWStringValue  FALSE
	#define EXP_SettgGetWStringValue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgGetWStringValue
	#define EXT_SettgGetWStringValue
	#define GET_SettgGetWStringValue(fl)  CAL_CMGETAPI( "SettgGetWStringValue" ) 
	#define CAL_SettgGetWStringValue  SettgGetWStringValue
	#define CHK_SettgGetWStringValue  TRUE
	#define EXP_SettgGetWStringValue  CAL_CMEXPAPI( "SettgGetWStringValue" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgGetWStringValue
	#define EXT_SettgGetWStringValue
	#define GET_SettgGetWStringValue(fl)  CAL_CMGETAPI( "SettgGetWStringValue" ) 
	#define CAL_SettgGetWStringValue  SettgGetWStringValue
	#define CHK_SettgGetWStringValue  TRUE
	#define EXP_SettgGetWStringValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetWStringValue", (RTS_UINTPTR)SettgGetWStringValue, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgGetWStringValue
	#define EXT_CmpSettingsSettgGetWStringValue
	#define GET_CmpSettingsSettgGetWStringValue  ERR_OK
	#define CAL_CmpSettingsSettgGetWStringValue pICmpSettings->ISettgGetWStringValue
	#define CHK_CmpSettingsSettgGetWStringValue (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgGetWStringValue  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgGetWStringValue
	#define EXT_SettgGetWStringValue
	#define GET_SettgGetWStringValue(fl)  CAL_CMGETAPI( "SettgGetWStringValue" ) 
	#define CAL_SettgGetWStringValue pICmpSettings->ISettgGetWStringValue
	#define CHK_SettgGetWStringValue (pICmpSettings != NULL)
	#define EXP_SettgGetWStringValue  CAL_CMEXPAPI( "SettgGetWStringValue" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgGetWStringValue  PFSETTGGETWSTRINGVALUE pfSettgGetWStringValue;
	#define EXT_SettgGetWStringValue  extern PFSETTGGETWSTRINGVALUE pfSettgGetWStringValue;
	#define GET_SettgGetWStringValue(fl)  s_pfCMGetAPI2( "SettgGetWStringValue", (RTS_VOID_FCTPTR *)&pfSettgGetWStringValue, (fl), 0, 0)
	#define CAL_SettgGetWStringValue  pfSettgGetWStringValue
	#define CHK_SettgGetWStringValue  (pfSettgGetWStringValue != NULL)
	#define EXP_SettgGetWStringValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgGetWStringValue", (RTS_UINTPTR)SettgGetWStringValue, 0, 0) 
#endif




/**
 * <description>
 *	Write a WSTRING value to settings.
 *  The storage format is conform to UTF-16, that means one character consists of 2 byte values like "\ab\cd".
 *  Plain ASCII characters can also be represented in a mixed way in order to keep the readability of the string, e.g. "a\00".
 *  The value is interpreted depending on the target byteorder, e.g:
 *   - INTEL: NodeNameUnicode="T\00e\00s\00t\00"
 *   - MOTOROLA: NodeNameUnicode="\00T\00e\00s\00t"
 *  Example: The unicode character 'DEGREE CELSIUS' (U+2103) is stored as 
 *   - INTEL: "\03\21"
 *   - MOTOROLA: "\21\03"
 * </description>
 * <param name="pszComponent" type="IN">Name of component</param>
 * <param name="pszKey" type="IN">Name of key</param>
 * <param name="pszValue" type="IN">Pointer to write value</param>
 * <param name="iLen" type="IN">Length of string in WSTRING characters (not bytes!) to write without terminating NUL</param>
 * <result>ERR_OK</result>
 */
RTS_RESULT CDECL SettgSetWStringValue(const char *pszComponent, const char *pszKey, RTS_WCHAR *pwszValue, RTS_I32 iLen);
typedef RTS_RESULT (CDECL * PFSETTGSETWSTRINGVALUE) (const char *pszComponent, const char *pszKey, RTS_WCHAR *pwszValue, RTS_I32 iLen);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGSETWSTRINGVALUE_NOTIMPLEMENTED)
	#define USE_SettgSetWStringValue
	#define EXT_SettgSetWStringValue
	#define GET_SettgSetWStringValue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgSetWStringValue(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgSetWStringValue  FALSE
	#define EXP_SettgSetWStringValue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgSetWStringValue
	#define EXT_SettgSetWStringValue
	#define GET_SettgSetWStringValue(fl)  CAL_CMGETAPI( "SettgSetWStringValue" ) 
	#define CAL_SettgSetWStringValue  SettgSetWStringValue
	#define CHK_SettgSetWStringValue  TRUE
	#define EXP_SettgSetWStringValue  CAL_CMEXPAPI( "SettgSetWStringValue" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgSetWStringValue
	#define EXT_SettgSetWStringValue
	#define GET_SettgSetWStringValue(fl)  CAL_CMGETAPI( "SettgSetWStringValue" ) 
	#define CAL_SettgSetWStringValue  SettgSetWStringValue
	#define CHK_SettgSetWStringValue  TRUE
	#define EXP_SettgSetWStringValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetWStringValue", (RTS_UINTPTR)SettgSetWStringValue, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgSetWStringValue
	#define EXT_CmpSettingsSettgSetWStringValue
	#define GET_CmpSettingsSettgSetWStringValue  ERR_OK
	#define CAL_CmpSettingsSettgSetWStringValue pICmpSettings->ISettgSetWStringValue
	#define CHK_CmpSettingsSettgSetWStringValue (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgSetWStringValue  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgSetWStringValue
	#define EXT_SettgSetWStringValue
	#define GET_SettgSetWStringValue(fl)  CAL_CMGETAPI( "SettgSetWStringValue" ) 
	#define CAL_SettgSetWStringValue pICmpSettings->ISettgSetWStringValue
	#define CHK_SettgSetWStringValue (pICmpSettings != NULL)
	#define EXP_SettgSetWStringValue  CAL_CMEXPAPI( "SettgSetWStringValue" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgSetWStringValue  PFSETTGSETWSTRINGVALUE pfSettgSetWStringValue;
	#define EXT_SettgSetWStringValue  extern PFSETTGSETWSTRINGVALUE pfSettgSetWStringValue;
	#define GET_SettgSetWStringValue(fl)  s_pfCMGetAPI2( "SettgSetWStringValue", (RTS_VOID_FCTPTR *)&pfSettgSetWStringValue, (fl), 0, 0)
	#define CAL_SettgSetWStringValue  pfSettgSetWStringValue
	#define CHK_SettgSetWStringValue  (pfSettgSetWStringValue != NULL)
	#define EXP_SettgSetWStringValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgSetWStringValue", (RTS_UINTPTR)SettgSetWStringValue, 0, 0) 
#endif




/**
 * <description>Remove the specified key</description>
 * <param name="pszComponent" type="IN">Name of component</param>
 * <param name="pszKey" type="IN">Name of key</param>
 * <result>ERR_OK</result>
 */
RTS_RESULT CDECL SettgRemoveKey(const char *pszComponent, const char *pszKey);
typedef RTS_RESULT (CDECL * PFSETTGREMOVEKEY) (const char *pszComponent, const char *pszKey);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGREMOVEKEY_NOTIMPLEMENTED)
	#define USE_SettgRemoveKey
	#define EXT_SettgRemoveKey
	#define GET_SettgRemoveKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgRemoveKey(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgRemoveKey  FALSE
	#define EXP_SettgRemoveKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgRemoveKey
	#define EXT_SettgRemoveKey
	#define GET_SettgRemoveKey(fl)  CAL_CMGETAPI( "SettgRemoveKey" ) 
	#define CAL_SettgRemoveKey  SettgRemoveKey
	#define CHK_SettgRemoveKey  TRUE
	#define EXP_SettgRemoveKey  CAL_CMEXPAPI( "SettgRemoveKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgRemoveKey
	#define EXT_SettgRemoveKey
	#define GET_SettgRemoveKey(fl)  CAL_CMGETAPI( "SettgRemoveKey" ) 
	#define CAL_SettgRemoveKey  SettgRemoveKey
	#define CHK_SettgRemoveKey  TRUE
	#define EXP_SettgRemoveKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgRemoveKey", (RTS_UINTPTR)SettgRemoveKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgRemoveKey
	#define EXT_CmpSettingsSettgRemoveKey
	#define GET_CmpSettingsSettgRemoveKey  ERR_OK
	#define CAL_CmpSettingsSettgRemoveKey pICmpSettings->ISettgRemoveKey
	#define CHK_CmpSettingsSettgRemoveKey (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgRemoveKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgRemoveKey
	#define EXT_SettgRemoveKey
	#define GET_SettgRemoveKey(fl)  CAL_CMGETAPI( "SettgRemoveKey" ) 
	#define CAL_SettgRemoveKey pICmpSettings->ISettgRemoveKey
	#define CHK_SettgRemoveKey (pICmpSettings != NULL)
	#define EXP_SettgRemoveKey  CAL_CMEXPAPI( "SettgRemoveKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgRemoveKey  PFSETTGREMOVEKEY pfSettgRemoveKey;
	#define EXT_SettgRemoveKey  extern PFSETTGREMOVEKEY pfSettgRemoveKey;
	#define GET_SettgRemoveKey(fl)  s_pfCMGetAPI2( "SettgRemoveKey", (RTS_VOID_FCTPTR *)&pfSettgRemoveKey, (fl), 0, 0)
	#define CAL_SettgRemoveKey  pfSettgRemoveKey
	#define CHK_SettgRemoveKey  (pfSettgRemoveKey != NULL)
	#define EXP_SettgRemoveKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgRemoveKey", (RTS_UINTPTR)SettgRemoveKey, 0, 0) 
#endif




/**
 * <description>Clear the specified component by deleting all keys</description>
 * <param name="pszSection" type="IN">Name of the component</param>
 * <result>ERR_OK</result>
 */
RTS_RESULT CDECL SettgClearComponent(const char *pszComponent);
typedef RTS_RESULT (CDECL * PFSETTGCLEARCOMPONENT) (const char *pszComponent);
#if defined(CMPSETTINGS_NOTIMPLEMENTED) || defined(SETTGCLEARCOMPONENT_NOTIMPLEMENTED)
	#define USE_SettgClearComponent
	#define EXT_SettgClearComponent
	#define GET_SettgClearComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SettgClearComponent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SettgClearComponent  FALSE
	#define EXP_SettgClearComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SettgClearComponent
	#define EXT_SettgClearComponent
	#define GET_SettgClearComponent(fl)  CAL_CMGETAPI( "SettgClearComponent" ) 
	#define CAL_SettgClearComponent  SettgClearComponent
	#define CHK_SettgClearComponent  TRUE
	#define EXP_SettgClearComponent  CAL_CMEXPAPI( "SettgClearComponent" ) 
#elif defined(MIXED_LINK) && !defined(CMPSETTINGS_EXTERNAL)
	#define USE_SettgClearComponent
	#define EXT_SettgClearComponent
	#define GET_SettgClearComponent(fl)  CAL_CMGETAPI( "SettgClearComponent" ) 
	#define CAL_SettgClearComponent  SettgClearComponent
	#define CHK_SettgClearComponent  TRUE
	#define EXP_SettgClearComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgClearComponent", (RTS_UINTPTR)SettgClearComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSettingsSettgClearComponent
	#define EXT_CmpSettingsSettgClearComponent
	#define GET_CmpSettingsSettgClearComponent  ERR_OK
	#define CAL_CmpSettingsSettgClearComponent pICmpSettings->ISettgClearComponent
	#define CHK_CmpSettingsSettgClearComponent (pICmpSettings != NULL)
	#define EXP_CmpSettingsSettgClearComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SettgClearComponent
	#define EXT_SettgClearComponent
	#define GET_SettgClearComponent(fl)  CAL_CMGETAPI( "SettgClearComponent" ) 
	#define CAL_SettgClearComponent pICmpSettings->ISettgClearComponent
	#define CHK_SettgClearComponent (pICmpSettings != NULL)
	#define EXP_SettgClearComponent  CAL_CMEXPAPI( "SettgClearComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_SettgClearComponent  PFSETTGCLEARCOMPONENT pfSettgClearComponent;
	#define EXT_SettgClearComponent  extern PFSETTGCLEARCOMPONENT pfSettgClearComponent;
	#define GET_SettgClearComponent(fl)  s_pfCMGetAPI2( "SettgClearComponent", (RTS_VOID_FCTPTR *)&pfSettgClearComponent, (fl), 0, 0)
	#define CAL_SettgClearComponent  pfSettgClearComponent
	#define CHK_SettgClearComponent  (pfSettgClearComponent != NULL)
	#define EXP_SettgClearComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SettgClearComponent", (RTS_UINTPTR)SettgClearComponent, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSETTGSETDATABASENAME ISettgSetDatabaseName;
 	PFSETTGGETDATABASENAME ISettgGetDatabaseName;
 	PFSETTGBEGINUPDATE ISettgBeginUpdate;
 	PFSETTGENDUPDATE ISettgEndUpdate;
 	PFSETTGGETACCESSCOUNTER_ ISettgGetAccessCounter_;
 	PFSETTGSETOPTIONS ISettgSetOptions;
 	PFSETTGISOPTIONSPLITDATABASES ISettgIsOptionSplitDatabases;
 	PFSETTGGETINTVALUE ISettgGetIntValue;
 	PFSETTGSETINTVALUE ISettgSetIntValue;
 	PFSETTGGETSTRINGVALUE ISettgGetStringValue;
 	PFSETTGSETSTRINGVALUE ISettgSetStringValue;
 	PFSETTGGETWSTRINGVALUE ISettgGetWStringValue;
 	PFSETTGSETWSTRINGVALUE ISettgSetWStringValue;
 	PFSETTGREMOVEKEY ISettgRemoveKey;
 	PFSETTGCLEARCOMPONENT ISettgClearComponent;
 } ICmpSettings_C;

#ifdef CPLUSPLUS
class ICmpSettings : public IBase
{
	public:
		virtual RTS_RESULT CDECL ISettgSetDatabaseName(char *pszName) =0;
		virtual RTS_RESULT CDECL ISettgGetDatabaseName(char *pszName, RTS_SIZE nNameLen) =0;
		virtual RTS_RESULT CDECL ISettgBeginUpdate(void) =0;
		virtual RTS_RESULT CDECL ISettgEndUpdate(void) =0;
		virtual RTS_I32 CDECL ISettgGetAccessCounter_(RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISettgSetOptions(int bSplitDatabases) =0;
		virtual RTS_RESULT CDECL ISettgIsOptionSplitDatabases(void) =0;
		virtual RTS_RESULT CDECL ISettgGetIntValue(const char *pszComponent, const char *pszKey, RTS_I32 *piValue, int iDefault, int bCached) =0;
		virtual RTS_RESULT CDECL ISettgSetIntValue(const char *pszComponent, const char *pszKey, RTS_I32 iValue, int iBase) =0;
		virtual RTS_RESULT CDECL ISettgGetStringValue(const char *pszComponent, const char *pszKey, char *pszValue, RTS_I32 *piLen, char *pszDefault, int bCached) =0;
		virtual RTS_RESULT CDECL ISettgSetStringValue(const char *pszComponent, const char *pszKey, char *pszValue, RTS_I32 iLen) =0;
		virtual RTS_RESULT CDECL ISettgGetWStringValue(const char *pszComponent, const char *pszKey, RTS_WCHAR *pwszValue, RTS_I32 *piLen, RTS_WCHAR *pwszDefault, int bCached) =0;
		virtual RTS_RESULT CDECL ISettgSetWStringValue(const char *pszComponent, const char *pszKey, RTS_WCHAR *pwszValue, RTS_I32 iLen) =0;
		virtual RTS_RESULT CDECL ISettgRemoveKey(const char *pszComponent, const char *pszKey) =0;
		virtual RTS_RESULT CDECL ISettgClearComponent(const char *pszComponent) =0;
};
	#ifndef ITF_CmpSettings
		#define ITF_CmpSettings static ICmpSettings *pICmpSettings = NULL;
	#endif
	#define EXTITF_CmpSettings
#else	/*CPLUSPLUS*/
	typedef ICmpSettings_C		ICmpSettings;
	#ifndef ITF_CmpSettings
		#define ITF_CmpSettings
	#endif
	#define EXTITF_CmpSettings
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPSETTINGSITF_H_*/
