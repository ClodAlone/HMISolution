 /**
 * <interfacename>CmpLog</interfacename>
 * <description><p>Interface of the runtime system logger component.</p>
 *
 * <p>The logger can log runtime system messages in form of strings, together
 * with some describing informations, like message categories and IDs.
 * The messages are saved in a local buffer in the RAM and uploaded to
 * the CoDeSys programming system or an external service tool on demand.</p>
 *
 * <p>The embedded variant of this component does not support backends and
 * is therefore not able to save the log messages into a file.</p>
 *
 * <p>Neither the log buffer in RAM, nor the communication medium are safe.
 * Therefore one can not essentially rely on the content of the log
 * messages. They can only be used for analytical purposes.</p>
 *
 * <p>For runtimes with the define RTS_SIL2 defined, the component may be a
 * bit more limited. For example, no events are supported.</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPLOGITF_H_
#define _CMPLOGITF_H_

#include "CmpStd.h"

 

 




#include <stdarg.h>
#include "CmpStd.h"
#include "CmpLogBackendItf.h"
#include "SysTimeRtcItf.h"


#ifndef LOG_TEXT
	#define LOG_TEXT(s)					s
#endif

#ifndef LOG_DEFAULT_NUM_OF_LOGGER
	#define LOG_DEFAULT_NUM_OF_LOGGER	1		/* Default number of loggers */
#endif

#ifndef LOG_DEFAULT_NUM_OF_ITF
	#define LOG_DEFAULT_NUM_OF_ITF		3		/* Default number of logger interfaces per logger */
#endif


/**
 * <category>Settings</category>
 * <description>
 *	For all logger settings you have to specify the prefix and the logger ID: "Logger.<ID>.xxx".
 *	ID=0 is the standard logger of the runtime system!
 *	Example:
 *		Logger.0.Name=PlcLog
 *		Logger.1.Name=MyLogger
 * </description>
 */
#define LOGKEY_LOGGER_PREFIX			"Logger"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Setting to specify the logger name
 *	Example:
 *		Logger.0.Name=PlcLog
 * </description>
 */
#ifndef LOGKEY_STRING_NAME
	#define LOGKEY_STRING_NAME			"Name"
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to enable a logger. Enable=0 means disable, Enable=1 means enable
 *	Example:
 *		Logger.0.Name=PlcLog
 *		Logger.0.Enable=0
 * </description>
 */
#define LOGKEY_INT_ENABLE				"Enable"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to filter which type of log entries the logger will store. See "Log class/filter" for details.
 *	Example:
 *		Logger.0.Name=PlcLog
 *		Logger.0.Filter=0x0000000F	; Only logger entries of type (LOG_INFO | LOG_WARNING | LOG_ERROR | LOG_EXCEPTION) are logged
 * </description>
 */
#define LOGKEY_INT_FILTER				"Filter"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to specify the type of a logger. See "Log types" for details.
 *	Example:
 *		Logger.0.Name=PlcLog
 *		Logger.0.Type=0x00000514	; This is a standard logger (LT_NORMAL | LT_TIMESTAMP_RTC | LT_NO_DISABLE | LT_DUMP_ALWAYS)
 * </description>
 */
#define LOGKEY_INT_TYPE					"Type"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to specify the maximum number of entries that can be stored in the ringbuffer of the logger.
 *	Example:
 *		Logger.0.Name=PlcLog
 *		Logger.0.MaxEntries=500
 * </description>
 */
#define LOGKEY_INT_MAXENTRIES			"MaxEntries"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to specify the maximum file size, if logger is stored in SysFile backend.
 *	Example:
 *		Logger.0.Name=PlcLog
 *		Logger.0.MaxFileSize=100000
 * </description>
 */
#define LOGKEY_INT_MAXFILESIZE			"MaxFileSize"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to specify the maximum number of files, if logger is stored in SysFile backend.
 *	Example:
 *		Logger.0.Name=PlcLog
 *		Logger.0.MaxFiles=3
 * </description>
 */
#define LOGKEY_INT_MAXFILES				"MaxFiles"


/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to specify the logger backend
 *	It must be used the way:  "Logger.<ID>.Backend.<ID Backend>.xxx"
 *	Example:
 *		Logger.0.Backend.0.ClassId=0x00000104
 * </description>
 */
#define LOGKEY_BACKEND					"Backend"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to specify the type of backend. This is specified via the corresponding class id of the backend (classes see CmpItf.h).
 *	E.g.
 *		ClassId=0x00000104		; This the class id of the SysFile component. See CLASSID_CSysFile in CmpItf.h.
 *		ClassId=0x0000010B		; This the class id of the CLASSID_CSysOut component for the outputs on a console. See CLASSID_CSysOut in CmpItf.h.
 *
 *	Example:
 *		Logger.0.Backend.0.ClassId=0x00000104
 * </description>
 */
#define LOGKEY_INT_CLASSID				"ClassId"


/**
 * <category>Log types</category>
 * <description>Types of a logger instance</description>
 * <element name="LT_HIGHSPEED" type="">Not supported</element>
 * <element name="LT_SAFE" type="">Not supported. Dump log entries in ring buffer residing in retain memory.</element>
 * <element name="LT_NORMAL" type="">Dump log entries in ring buffer residing in volatile memory (RAM)</element>
 * <element name="LT_TIMESTAMP_RTC" type="">Use realtime clock for the timestamp</element>
 * <element name="LT_TIMESTAMP_RTC_HIGHRES" type="">Use realtime clock with high resolution for the timestamp</element>
 * <element name="LT_TIMESTAMP_MS" type="">Use millisecond ticks for the timestamp</element>
 * <element name="LT_TIMESTAMP_US" type="">Use microsecond ticks for the timestamp</element>
 * <element name="LT_TIMESTAMP_NS" type="">Use nanosecond ticks for the timestamp</element>
 * <element name="LT_NO_DISABLE" type="">If this option is set, the logger cannot be disabled</element>
 * <element name="LT_DUMP_ASYNC" type="">Dump asynchronously in the backends</element>
 * <element name="LT_DUMP_ALWAYS" type="">Dump into the backends at every log entry</element>
 * <element name="LT_DUMP_ON_CLOSE" type="">Dump only the backends at closing the logger</element>
 * <element name="LT_DUMP_ON_REQUEST" type="">Dump can be done by calling LogDumpEntries()</element>
 * <element name="LT_DUMP_FORMAT2" type="">Dump with optional format 2. Is only recognized in the backends.</element>
 * <element name="LT_STD" type=""></element>
 */
#define LT_HIGHSPEED				UINT32_C(0x00000001)
#define LT_SAFE 					UINT32_C(0x00000002)
#define LT_NORMAL					UINT32_C(0x00000004)
#define LT_TS_BACKEND_LOCALTIME		UINT32_C(0x00000008)
#define LT_TIMESTAMP_RTC			UINT32_C(0x00000010)
#define LT_TIMESTAMP_RTC_HIGHRES	UINT32_C(0x00002000)
#define LT_TIMESTAMP_MS 			UINT32_C(0x00000020)
#define LT_TIMESTAMP_US 			UINT32_C(0x00000040)
#define LT_TIMESTAMP_NS 			UINT32_C(0x00000080)
#define LT_NO_DISABLE				UINT32_C(0x00000100)
#define LT_DUMP_ASYNC				UINT32_C(0x00000200)
#define LT_DUMP_ALWAYS				UINT32_C(0x00000400)
#define LT_DUMP_ON_CLOSE			UINT32_C(0x00000800)
#define LT_DUMP_ON_REQUEST			UINT32_C(0x00001000)
#define LT_DUMP_FORMAT2 			UINT32_C(0x00004000)
#ifndef LT_STD
	#define LT_STD						(LT_NORMAL | LT_TIMESTAMP_RTC | LT_NO_DISABLE | LT_DUMP_ALWAYS)
#endif
#define LT_HAS_TYPE(pLogger, Type)	 (pLogger->lo.iType & Type)

/**
 * <category>Log class</category>
 * <description>Log entry classes and filters</description>
 * <element name="LOG_NONE" type="No defined class"></element>
 * <element name="LOG_INFO" type="Information"></element>
 * <element name="LOG_WARNING" type="Warning"></element>
 * <element name="LOG_ERROR" type="Error"></element>
 * <element name="LOG_EXCEPTION" type="Exception"></element>
 * <element name="LOG_DEBUG" type="Debug log entries (reserved for development, diagnosis or test)"></element>
 * <element name="LOG_PRINTF" type="Debug log entries like printf. For this entry, no timestamp ist logged."></element>
 * <element name="LOG_COM" type="Communication"></element>
 * <element name="LOG_INFO_TIMESTAMP_RELATIVE" type="Information with timestamp relative to the previous entry"></element>
 * <element name="LOG_CRIT_SEC" type="Semaphore log entries"></element>
 * <element name="LOG_USER_NOTIFY" type="Log entries are displayed as a message box in CoDeSys"></element>
 */
#define LOG_NONE					UINT32_C(0x00000000)
#define LOG_INFO					UINT32_C(0x00000001)
#define LOG_WARNING					UINT32_C(0x00000002)
#define LOG_ERROR					UINT32_C(0x00000004)
#define LOG_EXCEPTION				UINT32_C(0x00000008)
#define LOG_DEBUG					UINT32_C(0x00000010)
#define LOG_PRINTF					UINT32_C(0x00000020)
#define LOG_COM						UINT32_C(0x00000040)
#define LOG_INFO_TIMESTAMP_RELATIVE	UINT32_C(0x00000080)
#define LOG_CRIT_SEC				UINT32_C(0x00000100)
#define LOG_USER_NOTIFY				UINT32_C(0x00010000)

/**
 * <category>Log filter</category>
 * <description>Log entry classes and filters</description>
 * <element name="LOG_NONE" type="LogFilter"></element>
 */
#define LOG_ALL						UINT32_MAX
#define LOG_STD						(LOG_INFO | LOG_WARNING | LOG_ERROR | LOG_EXCEPTION)
#define ToLog(Class, Filter)		(Class & Filter)


/**
 * <category>Static defines</category>
 * <description>Predefined objects in the runtime</description>
 */
#define USERDB_OBJECT_LOGGER		"Device.Logger"


/* Log info id */
#define LOG_INFOID_NONE				0x00000000

/* Predefined logger handles */
#define STD_LOGGER					0

/* Predefined logger used for the Component manager profiling. */
#if defined RTS_ENABLE_PROFILING
#define PROFILER_LOGGER				(RTS_HANDLE)1
#define PROFILER_LOGGER_NAME		"Profiler"
#define PROFILER_LOGGER_MAX_ENTRIES 1000
#endif

#ifndef STD_LOGGER_NAME
	#define STD_LOGGER_NAME				"PlcLog"
#endif
#ifndef COMM_LOGGER_NAME
	#define COMM_LOGGER_NAME			"CommLog"
#endif
#ifndef STD_LOGFILE_ENDING
	#define STD_LOGFILE_ENDING			".csv"
#endif

/**
 * <category>Static defines</category>
 * <description>Default maximimum number of log entries in a logger instance</description>
 */
#ifndef LOG_STD_MAX_NUM_OF_ENTRIES
	#define LOG_STD_MAX_NUM_OF_ENTRIES	500
#endif

/**
 * <category>Static defines</category>
 * <description>Default maximimum number of files for a logger with a file backend</description>
 */
#ifndef LOG_STD_MAX_NUM_OF_FILES
	#define LOG_STD_MAX_NUM_OF_FILES	3
#endif

/**
 * <category>Static defines</category>
 * <description>Default maximimum file size for a logger with a file backend</description>
 */
#ifndef LOG_STD_MAX_FILE_SIZE
	#define LOG_STD_MAX_FILE_SIZE		100000
#endif

/**
 * <category>Static defines</category>
 * <description>Maximimum lenght of the info string in a logger entry</description>
 */
#ifndef LOG_MAX_INFO_LEN
	#define LOG_MAX_INFO_LEN		128
#endif

/**
 * <category>Event parameter</category>
 * <element name="pApp" type="IN">Pointer to application description</element>
 */
typedef struct
{
	RTS_HANDLE hLog;
	CMPID CmpId;
	RTS_I32 iClassID;
	RTS_RESULT iErrorID;
	RTS_I32 iInfoID;
	char *pszInfo;
	va_list *pargList;
} EVTPARAM_CmpLogAdd;
#define EVTPARAMID_CmpLogAdd		0x0001
#define EVTVERSION_CmpLogAdd		0x0002
/**
 * <category>Events</category>
 * <description>Event is sent, after a new log entry added to the logger</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpLogAdd</param>
 */
#define EVT_LogAdd							MAKE_EVENTID(EVTCLASS_INFO, 1)



/**
 * <category>Online services</category>
 * <description>
 *	Get all log entries
 * </description>
 */
#define LOG_GET_ENTRIES				0x01

/**
 * <category>Online services</category>
 * <description>
 *	Get component name specified by component id
 * </description>
 */
#define LOG_GET_COMPONENT_NAMES		0x02

/**
 * <category>Online services</category>
 * <description>
 *	Get all registered logger names
 * </description>
 */
#define LOG_GET_LOGGER_LIST			0x03

/**
 * <category>Online service tags</category>
 * <description>
 * </description>
 */
#define TAG_LOGGER_NAME				0x01
#define TAG_START_HANDLE			0x02
#define TAG_START_TIMESTAMP			0x03
#define TAG_NUM_OF_ENTRIES			0x04
#define TAG_COMPONENT_ID			0x05

#define TAG_LOG_REPLY_ERROR			0x01

#define TAG_REPLY_LOGENTRY			0x82
#define TAG_ENTRY_HANDLE			0x01
#define TAG_TIMESTAMP_UTC			0x02
#define TAG_LOG_ENTRY				0x03
#define TAG_LOG_INFO				0x04
#define TAG_COMPONENT_NAME			0x05
#define TAG_TIMESTAMP_MS			0x06
#define TAG_TIMESTAMP_US			0x07
#define TAG_TIMESTAMP_NS			0x08
#define TAG_TIMESTAMP_UTC_HIGH_RES	0x09


/**
 * <SIL2/>
 * <category>Logger</category>
 * <description>Options of a logger (part of the configuration).</description>
 * <param name="szName" type="IN">Name of logger</param>
 * <param name="bEnable" type="IN">Logger active/inactive</param>
 * <param name="iType" type="IN">Logger type. See above log types.</param>
 * <param name="iFilter" type="IN">Filter for the logger entries to store. It is a combination of log classes</param>
 * <param name="iMaxEntries" type="IN">Max number of possible log entries</param>
 * <param name="iMaxFileSize" type="IN">Max file size, if logger content is stored in a file</param>
 * <param name="iMaxFiles" type="IN">Max number of files, if logger content is stored in a file</param>
 * <param name="pszPath" type="IN">Optional path for the logger file</param>
 */
typedef struct tagLogOptions
{
	char szName[32];
	RTS_I32 bEnable;
	RTS_I32 iType;
	RTS_I32 iFilter;
	RTS_I32 iMaxEntries;
	RTS_I32 iMaxFileSize;
	RTS_I32 iMaxFiles;
	char *pszPath;
} LogOptions;

typedef union
{
	RTS_UI32 ulTs;
	RTS_UI64 ullTs;
} LogTimestamp;


/**
 * <SIL2/>
 * <category>Logger</category>
 * <description>One logger Entry, including the message.</description>
 */
typedef struct tagLogEntry
{
	LogTimestamp tTimestamp;
	CMPID CmpId;
	RTS_UI32 iClassID;
	RTS_RESULT iErrorID;
	RTS_UI32 iInfoID;
	char szInfo[LOG_MAX_INFO_LEN];
} LogEntry;

/**
 * <SIL2/>
 * <category>Logger</category>
 * <description>Backend interface of one logger.</description>
 */
typedef struct
{
	CLASSID ClassId;
	RTS_HANDLE hLogBackend;
	RTS_I32 iLastDumpedIndex;
} LogItf;

/**
 * <SIL2/>
 * <category>Logger</category>
 * <description>Configuration of one logger.</description>
 */
typedef struct
{
	LogOptions lo;
	RTS_UI32 ulStartTimestamp;
	RTS_I32 iIndex;
	RTS_I32 iFirstIndex;
	RTS_I32 iRegInterfaces;
	RTS_I32 iDumpSync;
	LogEntry *pLog;
	LogItf tLogItf[LOG_DEFAULT_NUM_OF_ITF];
} Logger;

typedef RTS_RESULT (CDECL * PFLOGADD2) (RTS_HANDLE hLog, CMPID CmpId, RTS_I32 iClassID, RTS_RESULT iErrorID, RTS_I32 iInfoID, char *pszInfo, ...);


/** EXTERN LIB SECTION BEGIN **/

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <SIL2/>
 */
typedef struct taglogadd_struct
{
	RTS_IEC_HANDLE hLogger;				/* VAR_INPUT */	
	RTS_IEC_STRING stCmpName[81];		/* VAR_INPUT */	
	RTS_IEC_DINT diClassID;				/* VAR_INPUT */	
	RTS_IEC_DINT diErrorID;				/* VAR_INPUT */	
	RTS_IEC_DINT diInfoID;				/* VAR_INPUT */	
	RTS_IEC_STRING stInfo[256];			/* VAR_INPUT */	
	RTS_IEC_RESULT LogAdd;				/* VAR_OUTPUT */	
} logadd_struct;

void CDECL CDECL_EXT logadd(logadd_struct *p);
typedef void (CDECL CDECL_EXT* PFLOGADD_IEC) (logadd_struct *p);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGADD_NOTIMPLEMENTED)
	#define USE_logadd
	#define EXT_logadd
	#define GET_logadd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_logadd(p0) 
	#define CHK_logadd  FALSE
	#define EXP_logadd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_logadd
	#define EXT_logadd
	#define GET_logadd(fl)  CAL_CMGETAPI( "logadd" ) 
	#define CAL_logadd  logadd
	#define CHK_logadd  TRUE
	#define EXP_logadd  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd", (RTS_UINTPTR)logadd, 1, RTSITF_GET_SIGNATURE(0, 0x23C1D811), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_logadd
	#define EXT_logadd
	#define GET_logadd(fl)  CAL_CMGETAPI( "logadd" ) 
	#define CAL_logadd  logadd
	#define CHK_logadd  TRUE
	#define EXP_logadd  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd", (RTS_UINTPTR)logadd, 1, RTSITF_GET_SIGNATURE(0, 0x23C1D811), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLoglogadd
	#define EXT_CmpLoglogadd
	#define GET_CmpLoglogadd  ERR_OK
	#define CAL_CmpLoglogadd  logadd
	#define CHK_CmpLoglogadd  TRUE
	#define EXP_CmpLoglogadd  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd", (RTS_UINTPTR)logadd, 1, RTSITF_GET_SIGNATURE(0, 0x23C1D811), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_logadd
	#define EXT_logadd
	#define GET_logadd(fl)  CAL_CMGETAPI( "logadd" ) 
	#define CAL_logadd  logadd
	#define CHK_logadd  TRUE
	#define EXP_logadd  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd", (RTS_UINTPTR)logadd, 1, RTSITF_GET_SIGNATURE(0, 0x23C1D811), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_logadd  PFLOGADD_IEC pflogadd;
	#define EXT_logadd  extern PFLOGADD_IEC pflogadd;
	#define GET_logadd(fl)  s_pfCMGetAPI2( "logadd", (RTS_VOID_FCTPTR *)&pflogadd, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x23C1D811), 0x03050500)
	#define CAL_logadd  pflogadd
	#define CHK_logadd  (pflogadd != NULL)
	#define EXP_logadd   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd", (RTS_UINTPTR)logadd, 1, RTSITF_GET_SIGNATURE(0, 0x23C1D811), 0x03050500) 
#endif


/**
 * <SIL2/>
 */
typedef struct taglogadd2_struct
{
	RTS_IEC_HANDLE hLogger;				/* VAR_INPUT */	
	RTS_IEC_UDINT udiCmpID;				/* VAR_INPUT */	
	RTS_IEC_UDINT udiClassID;			/* VAR_INPUT */	
	RTS_IEC_UDINT udiErrorID;			/* VAR_INPUT */	
	RTS_IEC_UDINT udiInfoID;			/* VAR_INPUT */	
	RTS_IEC_STRING *pszInfo;			/* VAR_INPUT */	
	RTS_IEC_RESULT LogAdd2;				/* VAR_OUTPUT */	
} logadd2_struct;

void CDECL CDECL_EXT logadd2(logadd2_struct *p);
typedef void (CDECL CDECL_EXT* PFLOGADD2_IEC) (logadd2_struct *p);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGADD2_NOTIMPLEMENTED)
	#define USE_logadd2
	#define EXT_logadd2
	#define GET_logadd2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_logadd2(p0) 
	#define CHK_logadd2  FALSE
	#define EXP_logadd2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_logadd2
	#define EXT_logadd2
	#define GET_logadd2(fl)  CAL_CMGETAPI( "logadd2" ) 
	#define CAL_logadd2  logadd2
	#define CHK_logadd2  TRUE
	#define EXP_logadd2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd2", (RTS_UINTPTR)logadd2, 1, RTSITF_GET_SIGNATURE(0, 0x317F5220), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_logadd2
	#define EXT_logadd2
	#define GET_logadd2(fl)  CAL_CMGETAPI( "logadd2" ) 
	#define CAL_logadd2  logadd2
	#define CHK_logadd2  TRUE
	#define EXP_logadd2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd2", (RTS_UINTPTR)logadd2, 1, RTSITF_GET_SIGNATURE(0, 0x317F5220), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLoglogadd2
	#define EXT_CmpLoglogadd2
	#define GET_CmpLoglogadd2  ERR_OK
	#define CAL_CmpLoglogadd2  logadd2
	#define CHK_CmpLoglogadd2  TRUE
	#define EXP_CmpLoglogadd2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd2", (RTS_UINTPTR)logadd2, 1, RTSITF_GET_SIGNATURE(0, 0x317F5220), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_logadd2
	#define EXT_logadd2
	#define GET_logadd2(fl)  CAL_CMGETAPI( "logadd2" ) 
	#define CAL_logadd2  logadd2
	#define CHK_logadd2  TRUE
	#define EXP_logadd2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd2", (RTS_UINTPTR)logadd2, 1, RTSITF_GET_SIGNATURE(0, 0x317F5220), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_logadd2  PFLOGADD2_IEC pflogadd2;
	#define EXT_logadd2  extern PFLOGADD2_IEC pflogadd2;
	#define GET_logadd2(fl)  s_pfCMGetAPI2( "logadd2", (RTS_VOID_FCTPTR *)&pflogadd2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x317F5220), 0x03050500)
	#define CAL_logadd2  pflogadd2
	#define CHK_logadd2  (pflogadd2 != NULL)
	#define EXP_logadd2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logadd2", (RTS_UINTPTR)logadd2, 1, RTSITF_GET_SIGNATURE(0, 0x317F5220), 0x03050500) 
#endif


/**
 * <SIL2/>
 */
typedef struct taglogclose_struct
{
	RTS_IEC_HANDLE hLogger;				/* VAR_INPUT */	
	RTS_IEC_RESULT LogClose;			/* VAR_OUTPUT */	
} logclose_struct;

void CDECL CDECL_EXT logclose(logclose_struct *p);
typedef void (CDECL CDECL_EXT* PFLOGCLOSE_IEC) (logclose_struct *p);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGCLOSE_NOTIMPLEMENTED)
	#define USE_logclose
	#define EXT_logclose
	#define GET_logclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_logclose(p0) 
	#define CHK_logclose  FALSE
	#define EXP_logclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_logclose
	#define EXT_logclose
	#define GET_logclose(fl)  CAL_CMGETAPI( "logclose" ) 
	#define CAL_logclose  logclose
	#define CHK_logclose  TRUE
	#define EXP_logclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logclose", (RTS_UINTPTR)logclose, 1, RTSITF_GET_SIGNATURE(0, 0xD950850C), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_logclose
	#define EXT_logclose
	#define GET_logclose(fl)  CAL_CMGETAPI( "logclose" ) 
	#define CAL_logclose  logclose
	#define CHK_logclose  TRUE
	#define EXP_logclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logclose", (RTS_UINTPTR)logclose, 1, RTSITF_GET_SIGNATURE(0, 0xD950850C), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLoglogclose
	#define EXT_CmpLoglogclose
	#define GET_CmpLoglogclose  ERR_OK
	#define CAL_CmpLoglogclose  logclose
	#define CHK_CmpLoglogclose  TRUE
	#define EXP_CmpLoglogclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logclose", (RTS_UINTPTR)logclose, 1, RTSITF_GET_SIGNATURE(0, 0xD950850C), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_logclose
	#define EXT_logclose
	#define GET_logclose(fl)  CAL_CMGETAPI( "logclose" ) 
	#define CAL_logclose  logclose
	#define CHK_logclose  TRUE
	#define EXP_logclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logclose", (RTS_UINTPTR)logclose, 1, RTSITF_GET_SIGNATURE(0, 0xD950850C), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_logclose  PFLOGCLOSE_IEC pflogclose;
	#define EXT_logclose  extern PFLOGCLOSE_IEC pflogclose;
	#define GET_logclose(fl)  s_pfCMGetAPI2( "logclose", (RTS_VOID_FCTPTR *)&pflogclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xD950850C), 0x03050500)
	#define CAL_logclose  pflogclose
	#define CHK_logclose  (pflogclose != NULL)
	#define EXP_logclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logclose", (RTS_UINTPTR)logclose, 1, RTSITF_GET_SIGNATURE(0, 0xD950850C), 0x03050500) 
#endif


/**
 * <SIL2/>
 */
typedef struct taglogcreate_struct
{
	LogOptions *pOptions;				/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE LogCreate;			/* VAR_OUTPUT */	
} logcreate_struct;

void CDECL CDECL_EXT logcreate(logcreate_struct *p);
typedef void (CDECL CDECL_EXT* PFLOGCREATE_IEC) (logcreate_struct *p);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGCREATE_NOTIMPLEMENTED)
	#define USE_logcreate
	#define EXT_logcreate
	#define GET_logcreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_logcreate(p0) 
	#define CHK_logcreate  FALSE
	#define EXP_logcreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_logcreate
	#define EXT_logcreate
	#define GET_logcreate(fl)  CAL_CMGETAPI( "logcreate" ) 
	#define CAL_logcreate  logcreate
	#define CHK_logcreate  TRUE
	#define EXP_logcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logcreate", (RTS_UINTPTR)logcreate, 1, RTSITF_GET_SIGNATURE(0, 0x428C258C), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_logcreate
	#define EXT_logcreate
	#define GET_logcreate(fl)  CAL_CMGETAPI( "logcreate" ) 
	#define CAL_logcreate  logcreate
	#define CHK_logcreate  TRUE
	#define EXP_logcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logcreate", (RTS_UINTPTR)logcreate, 1, RTSITF_GET_SIGNATURE(0, 0x428C258C), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLoglogcreate
	#define EXT_CmpLoglogcreate
	#define GET_CmpLoglogcreate  ERR_OK
	#define CAL_CmpLoglogcreate  logcreate
	#define CHK_CmpLoglogcreate  TRUE
	#define EXP_CmpLoglogcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logcreate", (RTS_UINTPTR)logcreate, 1, RTSITF_GET_SIGNATURE(0, 0x428C258C), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_logcreate
	#define EXT_logcreate
	#define GET_logcreate(fl)  CAL_CMGETAPI( "logcreate" ) 
	#define CAL_logcreate  logcreate
	#define CHK_logcreate  TRUE
	#define EXP_logcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logcreate", (RTS_UINTPTR)logcreate, 1, RTSITF_GET_SIGNATURE(0, 0x428C258C), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_logcreate  PFLOGCREATE_IEC pflogcreate;
	#define EXT_logcreate  extern PFLOGCREATE_IEC pflogcreate;
	#define GET_logcreate(fl)  s_pfCMGetAPI2( "logcreate", (RTS_VOID_FCTPTR *)&pflogcreate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x428C258C), 0x03050500)
	#define CAL_logcreate  pflogcreate
	#define CHK_logcreate  (pflogcreate != NULL)
	#define EXP_logcreate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logcreate", (RTS_UINTPTR)logcreate, 1, RTSITF_GET_SIGNATURE(0, 0x428C258C), 0x03050500) 
#endif


/**
 * <SIL2/>
 */
typedef struct taglogdelete_struct
{
	RTS_IEC_HANDLE hLogger;				/* VAR_INPUT */	
	RTS_IEC_RESULT LogDelete;			/* VAR_OUTPUT */	
} logdelete_struct;

void CDECL CDECL_EXT logdelete(logdelete_struct *p);
typedef void (CDECL CDECL_EXT* PFLOGDELETE_IEC) (logdelete_struct *p);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGDELETE_NOTIMPLEMENTED)
	#define USE_logdelete
	#define EXT_logdelete
	#define GET_logdelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_logdelete(p0) 
	#define CHK_logdelete  FALSE
	#define EXP_logdelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_logdelete
	#define EXT_logdelete
	#define GET_logdelete(fl)  CAL_CMGETAPI( "logdelete" ) 
	#define CAL_logdelete  logdelete
	#define CHK_logdelete  TRUE
	#define EXP_logdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logdelete", (RTS_UINTPTR)logdelete, 1, RTSITF_GET_SIGNATURE(0, 0x676DF4F9), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_logdelete
	#define EXT_logdelete
	#define GET_logdelete(fl)  CAL_CMGETAPI( "logdelete" ) 
	#define CAL_logdelete  logdelete
	#define CHK_logdelete  TRUE
	#define EXP_logdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logdelete", (RTS_UINTPTR)logdelete, 1, RTSITF_GET_SIGNATURE(0, 0x676DF4F9), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLoglogdelete
	#define EXT_CmpLoglogdelete
	#define GET_CmpLoglogdelete  ERR_OK
	#define CAL_CmpLoglogdelete  logdelete
	#define CHK_CmpLoglogdelete  TRUE
	#define EXP_CmpLoglogdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logdelete", (RTS_UINTPTR)logdelete, 1, RTSITF_GET_SIGNATURE(0, 0x676DF4F9), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_logdelete
	#define EXT_logdelete
	#define GET_logdelete(fl)  CAL_CMGETAPI( "logdelete" ) 
	#define CAL_logdelete  logdelete
	#define CHK_logdelete  TRUE
	#define EXP_logdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logdelete", (RTS_UINTPTR)logdelete, 1, RTSITF_GET_SIGNATURE(0, 0x676DF4F9), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_logdelete  PFLOGDELETE_IEC pflogdelete;
	#define EXT_logdelete  extern PFLOGDELETE_IEC pflogdelete;
	#define GET_logdelete(fl)  s_pfCMGetAPI2( "logdelete", (RTS_VOID_FCTPTR *)&pflogdelete, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x676DF4F9), 0x03050500)
	#define CAL_logdelete  pflogdelete
	#define CHK_logdelete  (pflogdelete != NULL)
	#define EXP_logdelete   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logdelete", (RTS_UINTPTR)logdelete, 1, RTSITF_GET_SIGNATURE(0, 0x676DF4F9), 0x03050500) 
#endif


/**
 * <SIL2/>
 */
typedef struct taglogopen_struct
{
	RTS_IEC_STRING *pszName;			/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE LogOpen;				/* VAR_OUTPUT */	
} logopen_struct;

void CDECL CDECL_EXT logopen(logopen_struct *p);
typedef void (CDECL CDECL_EXT* PFLOGOPEN_IEC) (logopen_struct *p);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGOPEN_NOTIMPLEMENTED)
	#define USE_logopen
	#define EXT_logopen
	#define GET_logopen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_logopen(p0) 
	#define CHK_logopen  FALSE
	#define EXP_logopen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_logopen
	#define EXT_logopen
	#define GET_logopen(fl)  CAL_CMGETAPI( "logopen" ) 
	#define CAL_logopen  logopen
	#define CHK_logopen  TRUE
	#define EXP_logopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logopen", (RTS_UINTPTR)logopen, 1, RTSITF_GET_SIGNATURE(0, 0xABBDCC4F), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_logopen
	#define EXT_logopen
	#define GET_logopen(fl)  CAL_CMGETAPI( "logopen" ) 
	#define CAL_logopen  logopen
	#define CHK_logopen  TRUE
	#define EXP_logopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logopen", (RTS_UINTPTR)logopen, 1, RTSITF_GET_SIGNATURE(0, 0xABBDCC4F), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLoglogopen
	#define EXT_CmpLoglogopen
	#define GET_CmpLoglogopen  ERR_OK
	#define CAL_CmpLoglogopen  logopen
	#define CHK_CmpLoglogopen  TRUE
	#define EXP_CmpLoglogopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logopen", (RTS_UINTPTR)logopen, 1, RTSITF_GET_SIGNATURE(0, 0xABBDCC4F), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_logopen
	#define EXT_logopen
	#define GET_logopen(fl)  CAL_CMGETAPI( "logopen" ) 
	#define CAL_logopen  logopen
	#define CHK_logopen  TRUE
	#define EXP_logopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logopen", (RTS_UINTPTR)logopen, 1, RTSITF_GET_SIGNATURE(0, 0xABBDCC4F), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_logopen  PFLOGOPEN_IEC pflogopen;
	#define EXT_logopen  extern PFLOGOPEN_IEC pflogopen;
	#define GET_logopen(fl)  s_pfCMGetAPI2( "logopen", (RTS_VOID_FCTPTR *)&pflogopen, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xABBDCC4F), 0x03050500)
	#define CAL_logopen  pflogopen
	#define CHK_logopen  (pflogopen != NULL)
	#define EXP_logopen   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"logopen", (RTS_UINTPTR)logopen, 1, RTSITF_GET_SIGNATURE(0, 0xABBDCC4F), 0x03050500) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description> Create a logger </description>
 * <param name="pOptions" type="IN">Options for logger</param>
 * <param name="pResult" type="OUT">Pointer to get the result</param>
 * <result>Handle to the logger, or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL LogCreate(LogOptions *pOptions,  RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFLOGCREATE) (LogOptions *pOptions,  RTS_RESULT *pResult);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGCREATE_NOTIMPLEMENTED)
	#define USE_LogCreate
	#define EXT_LogCreate
	#define GET_LogCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogCreate(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogCreate  FALSE
	#define EXP_LogCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogCreate
	#define EXT_LogCreate
	#define GET_LogCreate(fl)  CAL_CMGETAPI( "LogCreate" ) 
	#define CAL_LogCreate  LogCreate
	#define CHK_LogCreate  TRUE
	#define EXP_LogCreate  CAL_CMEXPAPI( "LogCreate" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogCreate
	#define EXT_LogCreate
	#define GET_LogCreate(fl)  CAL_CMGETAPI( "LogCreate" ) 
	#define CAL_LogCreate  LogCreate
	#define CHK_LogCreate  TRUE
	#define EXP_LogCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogCreate", (RTS_UINTPTR)LogCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogCreate
	#define EXT_CmpLogLogCreate
	#define GET_CmpLogLogCreate  ERR_OK
	#define CAL_CmpLogLogCreate pICmpLog->ILogCreate
	#define CHK_CmpLogLogCreate (pICmpLog != NULL)
	#define EXP_CmpLogLogCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogCreate
	#define EXT_LogCreate
	#define GET_LogCreate(fl)  CAL_CMGETAPI( "LogCreate" ) 
	#define CAL_LogCreate pICmpLog->ILogCreate
	#define CHK_LogCreate (pICmpLog != NULL)
	#define EXP_LogCreate  CAL_CMEXPAPI( "LogCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogCreate  PFLOGCREATE pfLogCreate;
	#define EXT_LogCreate  extern PFLOGCREATE pfLogCreate;
	#define GET_LogCreate(fl)  s_pfCMGetAPI2( "LogCreate", (RTS_VOID_FCTPTR *)&pfLogCreate, (fl), 0, 0)
	#define CAL_LogCreate  pfLogCreate
	#define CHK_LogCreate  (pfLogCreate != NULL)
	#define EXP_LogCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogCreate", (RTS_UINTPTR)LogCreate, 0, 0) 
#endif




/**
 * <description> Open a logger with the specified name. Logger must exist! </description>
 * <param name="pszName" type="IN">Logger name</param>
 * <param name="pResult" type="OUT">Pointer to get the result</param>
 * <result>Handle to the logger, or RTS_INVALID_HANDLE if logger does not exist</result>
 */
RTS_HANDLE CDECL LogOpen(char *pszName, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFLOGOPEN) (char *pszName, RTS_RESULT *pResult);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGOPEN_NOTIMPLEMENTED)
	#define USE_LogOpen
	#define EXT_LogOpen
	#define GET_LogOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogOpen(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogOpen  FALSE
	#define EXP_LogOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogOpen
	#define EXT_LogOpen
	#define GET_LogOpen(fl)  CAL_CMGETAPI( "LogOpen" ) 
	#define CAL_LogOpen  LogOpen
	#define CHK_LogOpen  TRUE
	#define EXP_LogOpen  CAL_CMEXPAPI( "LogOpen" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogOpen
	#define EXT_LogOpen
	#define GET_LogOpen(fl)  CAL_CMGETAPI( "LogOpen" ) 
	#define CAL_LogOpen  LogOpen
	#define CHK_LogOpen  TRUE
	#define EXP_LogOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogOpen", (RTS_UINTPTR)LogOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogOpen
	#define EXT_CmpLogLogOpen
	#define GET_CmpLogLogOpen  ERR_OK
	#define CAL_CmpLogLogOpen pICmpLog->ILogOpen
	#define CHK_CmpLogLogOpen (pICmpLog != NULL)
	#define EXP_CmpLogLogOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogOpen
	#define EXT_LogOpen
	#define GET_LogOpen(fl)  CAL_CMGETAPI( "LogOpen" ) 
	#define CAL_LogOpen pICmpLog->ILogOpen
	#define CHK_LogOpen (pICmpLog != NULL)
	#define EXP_LogOpen  CAL_CMEXPAPI( "LogOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogOpen  PFLOGOPEN pfLogOpen;
	#define EXT_LogOpen  extern PFLOGOPEN pfLogOpen;
	#define GET_LogOpen(fl)  s_pfCMGetAPI2( "LogOpen", (RTS_VOID_FCTPTR *)&pfLogOpen, (fl), 0, 0)
	#define CAL_LogOpen  pfLogOpen
	#define CHK_LogOpen  (pfLogOpen != NULL)
	#define EXP_LogOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogOpen", (RTS_UINTPTR)LogOpen, 0, 0) 
#endif




/**
 * <description> Close the handle to a logger </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogClose(RTS_HANDLE hLog);
typedef RTS_RESULT (CDECL * PFLOGCLOSE) (RTS_HANDLE hLog);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGCLOSE_NOTIMPLEMENTED)
	#define USE_LogClose
	#define EXT_LogClose
	#define GET_LogClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogClose  FALSE
	#define EXP_LogClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogClose
	#define EXT_LogClose
	#define GET_LogClose(fl)  CAL_CMGETAPI( "LogClose" ) 
	#define CAL_LogClose  LogClose
	#define CHK_LogClose  TRUE
	#define EXP_LogClose  CAL_CMEXPAPI( "LogClose" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogClose
	#define EXT_LogClose
	#define GET_LogClose(fl)  CAL_CMGETAPI( "LogClose" ) 
	#define CAL_LogClose  LogClose
	#define CHK_LogClose  TRUE
	#define EXP_LogClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogClose", (RTS_UINTPTR)LogClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogClose
	#define EXT_CmpLogLogClose
	#define GET_CmpLogLogClose  ERR_OK
	#define CAL_CmpLogLogClose pICmpLog->ILogClose
	#define CHK_CmpLogLogClose (pICmpLog != NULL)
	#define EXP_CmpLogLogClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogClose
	#define EXT_LogClose
	#define GET_LogClose(fl)  CAL_CMGETAPI( "LogClose" ) 
	#define CAL_LogClose pICmpLog->ILogClose
	#define CHK_LogClose (pICmpLog != NULL)
	#define EXP_LogClose  CAL_CMEXPAPI( "LogClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogClose  PFLOGCLOSE pfLogClose;
	#define EXT_LogClose  extern PFLOGCLOSE pfLogClose;
	#define GET_LogClose(fl)  s_pfCMGetAPI2( "LogClose", (RTS_VOID_FCTPTR *)&pfLogClose, (fl), 0, 0)
	#define CAL_LogClose  pfLogClose
	#define CHK_LogClose  (pfLogClose != NULL)
	#define EXP_LogClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogClose", (RTS_UINTPTR)LogClose, 0, 0) 
#endif




/**
 * <description> Delete a logger </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogDelete(RTS_HANDLE hLog);
typedef RTS_RESULT (CDECL * PFLOGDELETE) (RTS_HANDLE hLog);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGDELETE_NOTIMPLEMENTED)
	#define USE_LogDelete
	#define EXT_LogDelete
	#define GET_LogDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogDelete(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogDelete  FALSE
	#define EXP_LogDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogDelete
	#define EXT_LogDelete
	#define GET_LogDelete(fl)  CAL_CMGETAPI( "LogDelete" ) 
	#define CAL_LogDelete  LogDelete
	#define CHK_LogDelete  TRUE
	#define EXP_LogDelete  CAL_CMEXPAPI( "LogDelete" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogDelete
	#define EXT_LogDelete
	#define GET_LogDelete(fl)  CAL_CMGETAPI( "LogDelete" ) 
	#define CAL_LogDelete  LogDelete
	#define CHK_LogDelete  TRUE
	#define EXP_LogDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogDelete", (RTS_UINTPTR)LogDelete, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogDelete
	#define EXT_CmpLogLogDelete
	#define GET_CmpLogLogDelete  ERR_OK
	#define CAL_CmpLogLogDelete pICmpLog->ILogDelete
	#define CHK_CmpLogLogDelete (pICmpLog != NULL)
	#define EXP_CmpLogLogDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogDelete
	#define EXT_LogDelete
	#define GET_LogDelete(fl)  CAL_CMGETAPI( "LogDelete" ) 
	#define CAL_LogDelete pICmpLog->ILogDelete
	#define CHK_LogDelete (pICmpLog != NULL)
	#define EXP_LogDelete  CAL_CMEXPAPI( "LogDelete" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogDelete  PFLOGDELETE pfLogDelete;
	#define EXT_LogDelete  extern PFLOGDELETE pfLogDelete;
	#define GET_LogDelete(fl)  s_pfCMGetAPI2( "LogDelete", (RTS_VOID_FCTPTR *)&pfLogDelete, (fl), 0, 0)
	#define CAL_LogDelete  pfLogDelete
	#define CHK_LogDelete  (pfLogDelete != NULL)
	#define EXP_LogDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogDelete", (RTS_UINTPTR)LogDelete, 0, 0) 
#endif




/**
 * <description> Get options of logger </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <param name="ppOptions" type="OUT">Pointer to pointer to log options</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogGetOptions(RTS_HANDLE hLog, LogOptions **ppOptions);
typedef RTS_RESULT (CDECL * PFLOGGETOPTIONS) (RTS_HANDLE hLog, LogOptions **ppOptions);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETOPTIONS_NOTIMPLEMENTED)
	#define USE_LogGetOptions
	#define EXT_LogGetOptions
	#define GET_LogGetOptions(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetOptions(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogGetOptions  FALSE
	#define EXP_LogGetOptions  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetOptions
	#define EXT_LogGetOptions
	#define GET_LogGetOptions(fl)  CAL_CMGETAPI( "LogGetOptions" ) 
	#define CAL_LogGetOptions  LogGetOptions
	#define CHK_LogGetOptions  TRUE
	#define EXP_LogGetOptions  CAL_CMEXPAPI( "LogGetOptions" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetOptions
	#define EXT_LogGetOptions
	#define GET_LogGetOptions(fl)  CAL_CMGETAPI( "LogGetOptions" ) 
	#define CAL_LogGetOptions  LogGetOptions
	#define CHK_LogGetOptions  TRUE
	#define EXP_LogGetOptions  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetOptions", (RTS_UINTPTR)LogGetOptions, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetOptions
	#define EXT_CmpLogLogGetOptions
	#define GET_CmpLogLogGetOptions  ERR_OK
	#define CAL_CmpLogLogGetOptions pICmpLog->ILogGetOptions
	#define CHK_CmpLogLogGetOptions (pICmpLog != NULL)
	#define EXP_CmpLogLogGetOptions  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetOptions
	#define EXT_LogGetOptions
	#define GET_LogGetOptions(fl)  CAL_CMGETAPI( "LogGetOptions" ) 
	#define CAL_LogGetOptions pICmpLog->ILogGetOptions
	#define CHK_LogGetOptions (pICmpLog != NULL)
	#define EXP_LogGetOptions  CAL_CMEXPAPI( "LogGetOptions" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetOptions  PFLOGGETOPTIONS pfLogGetOptions;
	#define EXT_LogGetOptions  extern PFLOGGETOPTIONS pfLogGetOptions;
	#define GET_LogGetOptions(fl)  s_pfCMGetAPI2( "LogGetOptions", (RTS_VOID_FCTPTR *)&pfLogGetOptions, (fl), 0, 0)
	#define CAL_LogGetOptions  pfLogGetOptions
	#define CHK_LogGetOptions  (pfLogGetOptions != NULL)
	#define EXP_LogGetOptions  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetOptions", (RTS_UINTPTR)LogGetOptions, 0, 0) 
#endif




/**
 * <description> Enable logging </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogEnable(RTS_HANDLE hLog);
typedef RTS_RESULT (CDECL * PFLOGENABLE) (RTS_HANDLE hLog);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGENABLE_NOTIMPLEMENTED)
	#define USE_LogEnable
	#define EXT_LogEnable
	#define GET_LogEnable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogEnable(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogEnable  FALSE
	#define EXP_LogEnable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogEnable
	#define EXT_LogEnable
	#define GET_LogEnable(fl)  CAL_CMGETAPI( "LogEnable" ) 
	#define CAL_LogEnable  LogEnable
	#define CHK_LogEnable  TRUE
	#define EXP_LogEnable  CAL_CMEXPAPI( "LogEnable" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogEnable
	#define EXT_LogEnable
	#define GET_LogEnable(fl)  CAL_CMGETAPI( "LogEnable" ) 
	#define CAL_LogEnable  LogEnable
	#define CHK_LogEnable  TRUE
	#define EXP_LogEnable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogEnable", (RTS_UINTPTR)LogEnable, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogEnable
	#define EXT_CmpLogLogEnable
	#define GET_CmpLogLogEnable  ERR_OK
	#define CAL_CmpLogLogEnable pICmpLog->ILogEnable
	#define CHK_CmpLogLogEnable (pICmpLog != NULL)
	#define EXP_CmpLogLogEnable  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogEnable
	#define EXT_LogEnable
	#define GET_LogEnable(fl)  CAL_CMGETAPI( "LogEnable" ) 
	#define CAL_LogEnable pICmpLog->ILogEnable
	#define CHK_LogEnable (pICmpLog != NULL)
	#define EXP_LogEnable  CAL_CMEXPAPI( "LogEnable" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogEnable  PFLOGENABLE pfLogEnable;
	#define EXT_LogEnable  extern PFLOGENABLE pfLogEnable;
	#define GET_LogEnable(fl)  s_pfCMGetAPI2( "LogEnable", (RTS_VOID_FCTPTR *)&pfLogEnable, (fl), 0, 0)
	#define CAL_LogEnable  pfLogEnable
	#define CHK_LogEnable  (pfLogEnable != NULL)
	#define EXP_LogEnable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogEnable", (RTS_UINTPTR)LogEnable, 0, 0) 
#endif




/**
 * <description> Disable logging </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogDisable(RTS_HANDLE hLog);
typedef RTS_RESULT (CDECL * PFLOGDISABLE) (RTS_HANDLE hLog);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGDISABLE_NOTIMPLEMENTED)
	#define USE_LogDisable
	#define EXT_LogDisable
	#define GET_LogDisable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogDisable(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogDisable  FALSE
	#define EXP_LogDisable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogDisable
	#define EXT_LogDisable
	#define GET_LogDisable(fl)  CAL_CMGETAPI( "LogDisable" ) 
	#define CAL_LogDisable  LogDisable
	#define CHK_LogDisable  TRUE
	#define EXP_LogDisable  CAL_CMEXPAPI( "LogDisable" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogDisable
	#define EXT_LogDisable
	#define GET_LogDisable(fl)  CAL_CMGETAPI( "LogDisable" ) 
	#define CAL_LogDisable  LogDisable
	#define CHK_LogDisable  TRUE
	#define EXP_LogDisable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogDisable", (RTS_UINTPTR)LogDisable, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogDisable
	#define EXT_CmpLogLogDisable
	#define GET_CmpLogLogDisable  ERR_OK
	#define CAL_CmpLogLogDisable pICmpLog->ILogDisable
	#define CHK_CmpLogLogDisable (pICmpLog != NULL)
	#define EXP_CmpLogLogDisable  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogDisable
	#define EXT_LogDisable
	#define GET_LogDisable(fl)  CAL_CMGETAPI( "LogDisable" ) 
	#define CAL_LogDisable pICmpLog->ILogDisable
	#define CHK_LogDisable (pICmpLog != NULL)
	#define EXP_LogDisable  CAL_CMEXPAPI( "LogDisable" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogDisable  PFLOGDISABLE pfLogDisable;
	#define EXT_LogDisable  extern PFLOGDISABLE pfLogDisable;
	#define GET_LogDisable(fl)  s_pfCMGetAPI2( "LogDisable", (RTS_VOID_FCTPTR *)&pfLogDisable, (fl), 0, 0)
	#define CAL_LogDisable  pfLogDisable
	#define CHK_LogDisable  (pfLogDisable != NULL)
	#define EXP_LogDisable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogDisable", (RTS_UINTPTR)LogDisable, 0, 0) 
#endif




/**
 * <description> Set filter of logger </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <param name="iFilter" type="IN">Logger filter</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogSetFilter(RTS_HANDLE hLog, RTS_I32 iFilter);
typedef RTS_RESULT (CDECL * PFLOGSETFILTER) (RTS_HANDLE hLog, RTS_I32 iFilter);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGSETFILTER_NOTIMPLEMENTED)
	#define USE_LogSetFilter
	#define EXT_LogSetFilter
	#define GET_LogSetFilter(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogSetFilter(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogSetFilter  FALSE
	#define EXP_LogSetFilter  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogSetFilter
	#define EXT_LogSetFilter
	#define GET_LogSetFilter(fl)  CAL_CMGETAPI( "LogSetFilter" ) 
	#define CAL_LogSetFilter  LogSetFilter
	#define CHK_LogSetFilter  TRUE
	#define EXP_LogSetFilter  CAL_CMEXPAPI( "LogSetFilter" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogSetFilter
	#define EXT_LogSetFilter
	#define GET_LogSetFilter(fl)  CAL_CMGETAPI( "LogSetFilter" ) 
	#define CAL_LogSetFilter  LogSetFilter
	#define CHK_LogSetFilter  TRUE
	#define EXP_LogSetFilter  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogSetFilter", (RTS_UINTPTR)LogSetFilter, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogSetFilter
	#define EXT_CmpLogLogSetFilter
	#define GET_CmpLogLogSetFilter  ERR_OK
	#define CAL_CmpLogLogSetFilter pICmpLog->ILogSetFilter
	#define CHK_CmpLogLogSetFilter (pICmpLog != NULL)
	#define EXP_CmpLogLogSetFilter  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogSetFilter
	#define EXT_LogSetFilter
	#define GET_LogSetFilter(fl)  CAL_CMGETAPI( "LogSetFilter" ) 
	#define CAL_LogSetFilter pICmpLog->ILogSetFilter
	#define CHK_LogSetFilter (pICmpLog != NULL)
	#define EXP_LogSetFilter  CAL_CMEXPAPI( "LogSetFilter" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogSetFilter  PFLOGSETFILTER pfLogSetFilter;
	#define EXT_LogSetFilter  extern PFLOGSETFILTER pfLogSetFilter;
	#define GET_LogSetFilter(fl)  s_pfCMGetAPI2( "LogSetFilter", (RTS_VOID_FCTPTR *)&pfLogSetFilter, (fl), 0, 0)
	#define CAL_LogSetFilter  pfLogSetFilter
	#define CHK_LogSetFilter  (pfLogSetFilter != NULL)
	#define EXP_LogSetFilter  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogSetFilter", (RTS_UINTPTR)LogSetFilter, 0, 0) 
#endif




/**
 * <description> Get filter of logger</description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <result> Filter </result>
 */
RTS_I32 CDECL LogGetFilter(RTS_HANDLE hLog);
typedef RTS_I32 (CDECL * PFLOGGETFILTER) (RTS_HANDLE hLog);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETFILTER_NOTIMPLEMENTED)
	#define USE_LogGetFilter
	#define EXT_LogGetFilter
	#define GET_LogGetFilter(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetFilter(p0)  (RTS_I32)ERR_NOTIMPLEMENTED
	#define CHK_LogGetFilter  FALSE
	#define EXP_LogGetFilter  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetFilter
	#define EXT_LogGetFilter
	#define GET_LogGetFilter(fl)  CAL_CMGETAPI( "LogGetFilter" ) 
	#define CAL_LogGetFilter  LogGetFilter
	#define CHK_LogGetFilter  TRUE
	#define EXP_LogGetFilter  CAL_CMEXPAPI( "LogGetFilter" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetFilter
	#define EXT_LogGetFilter
	#define GET_LogGetFilter(fl)  CAL_CMGETAPI( "LogGetFilter" ) 
	#define CAL_LogGetFilter  LogGetFilter
	#define CHK_LogGetFilter  TRUE
	#define EXP_LogGetFilter  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetFilter", (RTS_UINTPTR)LogGetFilter, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetFilter
	#define EXT_CmpLogLogGetFilter
	#define GET_CmpLogLogGetFilter  ERR_OK
	#define CAL_CmpLogLogGetFilter pICmpLog->ILogGetFilter
	#define CHK_CmpLogLogGetFilter (pICmpLog != NULL)
	#define EXP_CmpLogLogGetFilter  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetFilter
	#define EXT_LogGetFilter
	#define GET_LogGetFilter(fl)  CAL_CMGETAPI( "LogGetFilter" ) 
	#define CAL_LogGetFilter pICmpLog->ILogGetFilter
	#define CHK_LogGetFilter (pICmpLog != NULL)
	#define EXP_LogGetFilter  CAL_CMEXPAPI( "LogGetFilter" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetFilter  PFLOGGETFILTER pfLogGetFilter;
	#define EXT_LogGetFilter  extern PFLOGGETFILTER pfLogGetFilter;
	#define GET_LogGetFilter(fl)  s_pfCMGetAPI2( "LogGetFilter", (RTS_VOID_FCTPTR *)&pfLogGetFilter, (fl), 0, 0)
	#define CAL_LogGetFilter  pfLogGetFilter
	#define CHK_LogGetFilter  (pfLogGetFilter != NULL)
	#define EXP_LogGetFilter  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetFilter", (RTS_UINTPTR)LogGetFilter, 0, 0) 
#endif




/**
 * <description>
 * <p>Add a new log entry to the log buffer.</p>
 *
 * <p>If the buffer is full when this function is called, the oldest
 * log entry in the buffer will be overwritten.</p>
 *
 * <p>If the Class ID contains LOG_INFO_TIMESTAMP_RELATIVE, there is an additional
 * tag, called "TimeRel" added to the text of the log entry. This will
 * limit the message size of the entry by the size of this tag.</p>
 *
 * <p>If the Class ID contains the flag LOG_USER_NOTIFY, the log message
 * will be shown in form of a message box at the next log in of 
 * CoDeSys or instantly if CoDeSys is still loged in.</p>
 *
 * <p>The interface supports a minimum of 8 variable arguments. Depending
 * on the C-Library, this might be more.</p>
 * <p>LT_TIMESTAMP_RTC, LT_TIMESTAMP_RTC_HIGHRES is not supported in SIL2 Runtime.</p>
 * </description>
 * <param name="hLog" type="IN" range="[RTS_INVALID_HANDLE,STD_LOGGER]">Handle to logger</param>
 * <param name="CmpId" type="IN" range="[VALID_CMPID]">Component id</param>
 * <param name="iClassID" type="IN" range="[LOG_ALL,LOG_NONE,LOG_ALL_NOT_LOG_USER_NOTIFY,LOG_ALL_NOT_LOG_INFO_TIMESTAMP_RELATIVE]">ClassID of entry (Info, Warning, Error, etc.)</param>
 * <param name="iErrorID" type="IN" range="[VALID_IERRORID]">Error code if available</param>
 * <param name="iInfoID" type="IN" range="[VALID_IINFOID]">ID of info text to enable multiple language error texts</param>
 * <param name="pszInfo" type="IN" range="[NULL,VALID_PSZINFO]">String to info text (in english or informations coded in XML)</param>  
 * <parampseudo name="_iFilterID" type="IN" range="[LOG_ALL,LOG_ALL_NOT_LOG_USER_NOTIFY,LOG_ALL_NOT_LOG_INFO_TIMESTAMP_RELATIVE]">Filter option</parampseudo>
 * <parampseudo name="_iType_Timestamp" type="IN" range="[LT_TIMESTAMP_MS,LT_TIMESTAMP_US,LT_TIMESTAMP_NS]">Logger Type Timestamp</parampseudo>
 * <parampseudo name="_bAddedEntry" type="OUT">Entry Added to Log</parampseudo> 
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid logger handle</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">iClassID was filtered for this logger or Logger is not enabled,</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">Configured memory for log buffer could not be allocated</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogAdd(RTS_HANDLE hLog, CMPID CmpId, RTS_I32 iClassID, RTS_RESULT iErrorID, RTS_I32 iInfoID, char *pszInfo, ...);
typedef RTS_RESULT (CDECL * PFLOGADD) (RTS_HANDLE hLog, CMPID CmpId, RTS_I32 iClassID, RTS_RESULT iErrorID, RTS_I32 iInfoID, char *pszInfo, ...);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGADD_NOTIMPLEMENTED)
	#define USE_LogAdd
	#define EXT_LogAdd
	#define GET_LogAdd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogAdd  FUNCTION_NOTIMPLEMENTED2 
	#define CHK_LogAdd  FALSE
	#define EXP_LogAdd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogAdd
	#define EXT_LogAdd
	#define GET_LogAdd(fl)  CAL_CMGETAPI( "LogAdd" ) 
	#define CAL_LogAdd  LogAdd
	#define CHK_LogAdd  TRUE
	#define EXP_LogAdd  CAL_CMEXPAPI( "LogAdd" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogAdd
	#define EXT_LogAdd
	#define GET_LogAdd(fl)  CAL_CMGETAPI( "LogAdd" ) 
	#define CAL_LogAdd  LogAdd
	#define CHK_LogAdd  TRUE
	#define EXP_LogAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogAdd", (RTS_UINTPTR)LogAdd, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogAdd
	#define EXT_CmpLogLogAdd
	#define GET_CmpLogLogAdd  ERR_OK
	#define CAL_CmpLogLogAdd pICmpLog->ILogAdd
	#define CHK_CmpLogLogAdd (pICmpLog != NULL)
	#define EXP_CmpLogLogAdd  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogAdd
	#define EXT_LogAdd
	#define GET_LogAdd(fl)  CAL_CMGETAPI( "LogAdd" ) 
	#define CAL_LogAdd pICmpLog->ILogAdd
	#define CHK_LogAdd (pICmpLog != NULL)
	#define EXP_LogAdd  CAL_CMEXPAPI( "LogAdd" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogAdd  PFLOGADD pfLogAdd;
	#define EXT_LogAdd  extern PFLOGADD pfLogAdd;
	#define GET_LogAdd(fl)  s_pfCMGetAPI2( "LogAdd", (RTS_VOID_FCTPTR *)&pfLogAdd, (fl), 0, 0)
	#define CAL_LogAdd  pfLogAdd
	#define CHK_LogAdd  (pfLogAdd != NULL)
	#define EXP_LogAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogAdd", (RTS_UINTPTR)LogAdd, 0, 0) 
#endif




/**
 * <description>
 * <p>Add a new log entry to the log buffer.</p>
 *
 * <p>The behavior is the same as the behavior of "LogAdd()", except that the 
 * parameter is a pointer to a variable argument list instead of a direct
 * variable argument list, passed to the function.</p>
 * </description>
 * <param name="hLog" type="IN" range="[RTS_INVALID_HANDLE,STD_LOGGER]">Handle to logger</param>
 * <param name="CmpId" type="IN" range="[VALID_CMPID]">Component id</param>
 * <param name="iClassID" type="IN" range="[LOG_ALL,LOG_NONE]">ClassID of entry (Info, Warning, Error, etc.)</param>
 * <param name="iErrorID" type="IN" range="[VALID_IERRORID]">Error code if available</param>
 * <param name="iInfoID" type="IN" range="[VALID_IINFOID]">ID of info text to enable multiple language error texts</param>
 * <param name="pszInfo" type="IN" range="[NULL,VALID_PSZINFO_1,VALID_PSZINFO_MAX]">String to info text (in english or informations coded in XML)</param>
 * <param name="pargList" type="IN" range="[NULL,VALID_PARGLIST]">Pointer to argument list, format is specified in pszInfo</param>
 * <parampseudo name="_bAddedEntry" type="OUT">Entry Added to Log</parampseudo>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid logger handle</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">iClassID was filtered for this logger or Logger is not enabled,</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">Configured memory for log buffer could not be allocated</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogAddArg(RTS_HANDLE hLog, CMPID CmpId, RTS_I32 iClassID, RTS_RESULT iErrorID, RTS_I32 iInfoID, char *pszInfo, va_list *pargList);
typedef RTS_RESULT (CDECL * PFLOGADDARG) (RTS_HANDLE hLog, CMPID CmpId, RTS_I32 iClassID, RTS_RESULT iErrorID, RTS_I32 iInfoID, char *pszInfo, va_list *pargList);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGADDARG_NOTIMPLEMENTED)
	#define USE_LogAddArg
	#define EXT_LogAddArg
	#define GET_LogAddArg(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogAddArg(p0,p1,p2,p3,p4,p5,p6)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogAddArg  FALSE
	#define EXP_LogAddArg  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogAddArg
	#define EXT_LogAddArg
	#define GET_LogAddArg(fl)  CAL_CMGETAPI( "LogAddArg" ) 
	#define CAL_LogAddArg  LogAddArg
	#define CHK_LogAddArg  TRUE
	#define EXP_LogAddArg  CAL_CMEXPAPI( "LogAddArg" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogAddArg
	#define EXT_LogAddArg
	#define GET_LogAddArg(fl)  CAL_CMGETAPI( "LogAddArg" ) 
	#define CAL_LogAddArg  LogAddArg
	#define CHK_LogAddArg  TRUE
	#define EXP_LogAddArg  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogAddArg", (RTS_UINTPTR)LogAddArg, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogAddArg
	#define EXT_CmpLogLogAddArg
	#define GET_CmpLogLogAddArg  ERR_OK
	#define CAL_CmpLogLogAddArg pICmpLog->ILogAddArg
	#define CHK_CmpLogLogAddArg (pICmpLog != NULL)
	#define EXP_CmpLogLogAddArg  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogAddArg
	#define EXT_LogAddArg
	#define GET_LogAddArg(fl)  CAL_CMGETAPI( "LogAddArg" ) 
	#define CAL_LogAddArg pICmpLog->ILogAddArg
	#define CHK_LogAddArg (pICmpLog != NULL)
	#define EXP_LogAddArg  CAL_CMEXPAPI( "LogAddArg" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogAddArg  PFLOGADDARG pfLogAddArg;
	#define EXT_LogAddArg  extern PFLOGADDARG pfLogAddArg;
	#define GET_LogAddArg(fl)  s_pfCMGetAPI2( "LogAddArg", (RTS_VOID_FCTPTR *)&pfLogAddArg, (fl), 0, 0)
	#define CAL_LogAddArg  pfLogAddArg
	#define CHK_LogAddArg  (pfLogAddArg != NULL)
	#define EXP_LogAddArg  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogAddArg", (RTS_UINTPTR)LogAddArg, 0, 0) 
#endif




/**
 * <description> Dump all entries from all log files </description>
 * <param name="iOptions" type="IN"><p>One or multiple of the following options:</p> 
 *	<ul>
 *		<li>LT_DUMP_ASYNC: If dump is done from an asychronous event</li>
 *		<li>LT_DUMP_ALWAYS: If dump should be forced (always)</li>
 *		<li>LT_DUMP_ON_CLOSE: If dump is called from closing the logger instance</li>
 *		<li>LT_DUMP_ON_REQUEST: If dump is forced from a request</li>
 *	</ul>
 * </param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogDumpAll(int iOptions);
typedef RTS_RESULT (CDECL * PFLOGDUMPALL) (int iOptions);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGDUMPALL_NOTIMPLEMENTED)
	#define USE_LogDumpAll
	#define EXT_LogDumpAll
	#define GET_LogDumpAll(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogDumpAll(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogDumpAll  FALSE
	#define EXP_LogDumpAll  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogDumpAll
	#define EXT_LogDumpAll
	#define GET_LogDumpAll(fl)  CAL_CMGETAPI( "LogDumpAll" ) 
	#define CAL_LogDumpAll  LogDumpAll
	#define CHK_LogDumpAll  TRUE
	#define EXP_LogDumpAll  CAL_CMEXPAPI( "LogDumpAll" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogDumpAll
	#define EXT_LogDumpAll
	#define GET_LogDumpAll(fl)  CAL_CMGETAPI( "LogDumpAll" ) 
	#define CAL_LogDumpAll  LogDumpAll
	#define CHK_LogDumpAll  TRUE
	#define EXP_LogDumpAll  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogDumpAll", (RTS_UINTPTR)LogDumpAll, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogDumpAll
	#define EXT_CmpLogLogDumpAll
	#define GET_CmpLogLogDumpAll  ERR_OK
	#define CAL_CmpLogLogDumpAll pICmpLog->ILogDumpAll
	#define CHK_CmpLogLogDumpAll (pICmpLog != NULL)
	#define EXP_CmpLogLogDumpAll  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogDumpAll
	#define EXT_LogDumpAll
	#define GET_LogDumpAll(fl)  CAL_CMGETAPI( "LogDumpAll" ) 
	#define CAL_LogDumpAll pICmpLog->ILogDumpAll
	#define CHK_LogDumpAll (pICmpLog != NULL)
	#define EXP_LogDumpAll  CAL_CMEXPAPI( "LogDumpAll" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogDumpAll  PFLOGDUMPALL pfLogDumpAll;
	#define EXT_LogDumpAll  extern PFLOGDUMPALL pfLogDumpAll;
	#define GET_LogDumpAll(fl)  s_pfCMGetAPI2( "LogDumpAll", (RTS_VOID_FCTPTR *)&pfLogDumpAll, (fl), 0, 0)
	#define CAL_LogDumpAll  pfLogDumpAll
	#define CHK_LogDumpAll  (pfLogDumpAll != NULL)
	#define EXP_LogDumpAll  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogDumpAll", (RTS_UINTPTR)LogDumpAll, 0, 0) 
#endif




/**
 * <description> Dump all entries from the last still dumped entry </description>
 * <param name="hLog" type="IN">Handle to the logger</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogDumpEntries(RTS_HANDLE hLog);
typedef RTS_RESULT (CDECL * PFLOGDUMPENTRIES) (RTS_HANDLE hLog);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGDUMPENTRIES_NOTIMPLEMENTED)
	#define USE_LogDumpEntries
	#define EXT_LogDumpEntries
	#define GET_LogDumpEntries(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogDumpEntries(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogDumpEntries  FALSE
	#define EXP_LogDumpEntries  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogDumpEntries
	#define EXT_LogDumpEntries
	#define GET_LogDumpEntries(fl)  CAL_CMGETAPI( "LogDumpEntries" ) 
	#define CAL_LogDumpEntries  LogDumpEntries
	#define CHK_LogDumpEntries  TRUE
	#define EXP_LogDumpEntries  CAL_CMEXPAPI( "LogDumpEntries" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogDumpEntries
	#define EXT_LogDumpEntries
	#define GET_LogDumpEntries(fl)  CAL_CMGETAPI( "LogDumpEntries" ) 
	#define CAL_LogDumpEntries  LogDumpEntries
	#define CHK_LogDumpEntries  TRUE
	#define EXP_LogDumpEntries  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogDumpEntries", (RTS_UINTPTR)LogDumpEntries, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogDumpEntries
	#define EXT_CmpLogLogDumpEntries
	#define GET_CmpLogLogDumpEntries  ERR_OK
	#define CAL_CmpLogLogDumpEntries pICmpLog->ILogDumpEntries
	#define CHK_CmpLogLogDumpEntries (pICmpLog != NULL)
	#define EXP_CmpLogLogDumpEntries  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogDumpEntries
	#define EXT_LogDumpEntries
	#define GET_LogDumpEntries(fl)  CAL_CMGETAPI( "LogDumpEntries" ) 
	#define CAL_LogDumpEntries pICmpLog->ILogDumpEntries
	#define CHK_LogDumpEntries (pICmpLog != NULL)
	#define EXP_LogDumpEntries  CAL_CMEXPAPI( "LogDumpEntries" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogDumpEntries  PFLOGDUMPENTRIES pfLogDumpEntries;
	#define EXT_LogDumpEntries  extern PFLOGDUMPENTRIES pfLogDumpEntries;
	#define GET_LogDumpEntries(fl)  s_pfCMGetAPI2( "LogDumpEntries", (RTS_VOID_FCTPTR *)&pfLogDumpEntries, (fl), 0, 0)
	#define CAL_LogDumpEntries  pfLogDumpEntries
	#define CHK_LogDumpEntries  (pfLogDumpEntries != NULL)
	#define EXP_LogDumpEntries  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogDumpEntries", (RTS_UINTPTR)LogDumpEntries, 0, 0) 
#endif




/**
 * OBSOLETE FUNCTION: Use LogRegisterBackend instead
 */
RTS_RESULT CDECL LogRegisterInterface(RTS_HANDLE hLog, CLASSID ClassId, ICmpLogBackend *pIBackend);
typedef RTS_RESULT (CDECL * PFLOGREGISTERINTERFACE) (RTS_HANDLE hLog, CLASSID ClassId, ICmpLogBackend *pIBackend);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGREGISTERINTERFACE_NOTIMPLEMENTED)
	#define USE_LogRegisterInterface
	#define EXT_LogRegisterInterface
	#define GET_LogRegisterInterface(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogRegisterInterface(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogRegisterInterface  FALSE
	#define EXP_LogRegisterInterface  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogRegisterInterface
	#define EXT_LogRegisterInterface
	#define GET_LogRegisterInterface(fl)  CAL_CMGETAPI( "LogRegisterInterface" ) 
	#define CAL_LogRegisterInterface  LogRegisterInterface
	#define CHK_LogRegisterInterface  TRUE
	#define EXP_LogRegisterInterface  CAL_CMEXPAPI( "LogRegisterInterface" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogRegisterInterface
	#define EXT_LogRegisterInterface
	#define GET_LogRegisterInterface(fl)  CAL_CMGETAPI( "LogRegisterInterface" ) 
	#define CAL_LogRegisterInterface  LogRegisterInterface
	#define CHK_LogRegisterInterface  TRUE
	#define EXP_LogRegisterInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogRegisterInterface", (RTS_UINTPTR)LogRegisterInterface, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogRegisterInterface
	#define EXT_CmpLogLogRegisterInterface
	#define GET_CmpLogLogRegisterInterface  ERR_OK
	#define CAL_CmpLogLogRegisterInterface pICmpLog->ILogRegisterInterface
	#define CHK_CmpLogLogRegisterInterface (pICmpLog != NULL)
	#define EXP_CmpLogLogRegisterInterface  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogRegisterInterface
	#define EXT_LogRegisterInterface
	#define GET_LogRegisterInterface(fl)  CAL_CMGETAPI( "LogRegisterInterface" ) 
	#define CAL_LogRegisterInterface pICmpLog->ILogRegisterInterface
	#define CHK_LogRegisterInterface (pICmpLog != NULL)
	#define EXP_LogRegisterInterface  CAL_CMEXPAPI( "LogRegisterInterface" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogRegisterInterface  PFLOGREGISTERINTERFACE pfLogRegisterInterface;
	#define EXT_LogRegisterInterface  extern PFLOGREGISTERINTERFACE pfLogRegisterInterface;
	#define GET_LogRegisterInterface(fl)  s_pfCMGetAPI2( "LogRegisterInterface", (RTS_VOID_FCTPTR *)&pfLogRegisterInterface, (fl), 0, 0)
	#define CAL_LogRegisterInterface  pfLogRegisterInterface
	#define CHK_LogRegisterInterface  (pfLogRegisterInterface != NULL)
	#define EXP_LogRegisterInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogRegisterInterface", (RTS_UINTPTR)LogRegisterInterface, 0, 0) 
#endif




/**
 * OBSOLETE FUNCTION: Use LogUnregisterBackend instead
 */
RTS_RESULT CDECL LogUnregisterInterface(RTS_HANDLE hLog, ICmpLogBackend *pIBackend);
typedef RTS_RESULT (CDECL * PFLOGUNREGISTERINTERFACE) (RTS_HANDLE hLog, ICmpLogBackend *pIBackend);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGUNREGISTERINTERFACE_NOTIMPLEMENTED)
	#define USE_LogUnregisterInterface
	#define EXT_LogUnregisterInterface
	#define GET_LogUnregisterInterface(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogUnregisterInterface(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogUnregisterInterface  FALSE
	#define EXP_LogUnregisterInterface  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogUnregisterInterface
	#define EXT_LogUnregisterInterface
	#define GET_LogUnregisterInterface(fl)  CAL_CMGETAPI( "LogUnregisterInterface" ) 
	#define CAL_LogUnregisterInterface  LogUnregisterInterface
	#define CHK_LogUnregisterInterface  TRUE
	#define EXP_LogUnregisterInterface  CAL_CMEXPAPI( "LogUnregisterInterface" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogUnregisterInterface
	#define EXT_LogUnregisterInterface
	#define GET_LogUnregisterInterface(fl)  CAL_CMGETAPI( "LogUnregisterInterface" ) 
	#define CAL_LogUnregisterInterface  LogUnregisterInterface
	#define CHK_LogUnregisterInterface  TRUE
	#define EXP_LogUnregisterInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogUnregisterInterface", (RTS_UINTPTR)LogUnregisterInterface, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogUnregisterInterface
	#define EXT_CmpLogLogUnregisterInterface
	#define GET_CmpLogLogUnregisterInterface  ERR_OK
	#define CAL_CmpLogLogUnregisterInterface pICmpLog->ILogUnregisterInterface
	#define CHK_CmpLogLogUnregisterInterface (pICmpLog != NULL)
	#define EXP_CmpLogLogUnregisterInterface  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogUnregisterInterface
	#define EXT_LogUnregisterInterface
	#define GET_LogUnregisterInterface(fl)  CAL_CMGETAPI( "LogUnregisterInterface" ) 
	#define CAL_LogUnregisterInterface pICmpLog->ILogUnregisterInterface
	#define CHK_LogUnregisterInterface (pICmpLog != NULL)
	#define EXP_LogUnregisterInterface  CAL_CMEXPAPI( "LogUnregisterInterface" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogUnregisterInterface  PFLOGUNREGISTERINTERFACE pfLogUnregisterInterface;
	#define EXT_LogUnregisterInterface  extern PFLOGUNREGISTERINTERFACE pfLogUnregisterInterface;
	#define GET_LogUnregisterInterface(fl)  s_pfCMGetAPI2( "LogUnregisterInterface", (RTS_VOID_FCTPTR *)&pfLogUnregisterInterface, (fl), 0, 0)
	#define CAL_LogUnregisterInterface  pfLogUnregisterInterface
	#define CHK_LogUnregisterInterface  (pfLogUnregisterInterface != NULL)
	#define EXP_LogUnregisterInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogUnregisterInterface", (RTS_UINTPTR)LogUnregisterInterface, 0, 0) 
#endif




/**
 * <description>Register a backend at the specified logger</description>
 * <param name="hLog" type="IN">Handle to the logger. Can be RTS_INVALID_HANDLE an so the backend is unregistered from all logger instances!</param>
 * <param name="ClassId" type="IN">ClassId of the backend. ClassId must be registered previously in the backend component with CMRegisterClass!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogRegisterBackend(RTS_HANDLE hLog, CLASSID ClassId);
typedef RTS_RESULT (CDECL * PFLOGREGISTERBACKEND) (RTS_HANDLE hLog, CLASSID ClassId);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGREGISTERBACKEND_NOTIMPLEMENTED)
	#define USE_LogRegisterBackend
	#define EXT_LogRegisterBackend
	#define GET_LogRegisterBackend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogRegisterBackend(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogRegisterBackend  FALSE
	#define EXP_LogRegisterBackend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogRegisterBackend
	#define EXT_LogRegisterBackend
	#define GET_LogRegisterBackend(fl)  CAL_CMGETAPI( "LogRegisterBackend" ) 
	#define CAL_LogRegisterBackend  LogRegisterBackend
	#define CHK_LogRegisterBackend  TRUE
	#define EXP_LogRegisterBackend  CAL_CMEXPAPI( "LogRegisterBackend" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogRegisterBackend
	#define EXT_LogRegisterBackend
	#define GET_LogRegisterBackend(fl)  CAL_CMGETAPI( "LogRegisterBackend" ) 
	#define CAL_LogRegisterBackend  LogRegisterBackend
	#define CHK_LogRegisterBackend  TRUE
	#define EXP_LogRegisterBackend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogRegisterBackend", (RTS_UINTPTR)LogRegisterBackend, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogRegisterBackend
	#define EXT_CmpLogLogRegisterBackend
	#define GET_CmpLogLogRegisterBackend  ERR_OK
	#define CAL_CmpLogLogRegisterBackend pICmpLog->ILogRegisterBackend
	#define CHK_CmpLogLogRegisterBackend (pICmpLog != NULL)
	#define EXP_CmpLogLogRegisterBackend  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogRegisterBackend
	#define EXT_LogRegisterBackend
	#define GET_LogRegisterBackend(fl)  CAL_CMGETAPI( "LogRegisterBackend" ) 
	#define CAL_LogRegisterBackend pICmpLog->ILogRegisterBackend
	#define CHK_LogRegisterBackend (pICmpLog != NULL)
	#define EXP_LogRegisterBackend  CAL_CMEXPAPI( "LogRegisterBackend" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogRegisterBackend  PFLOGREGISTERBACKEND pfLogRegisterBackend;
	#define EXT_LogRegisterBackend  extern PFLOGREGISTERBACKEND pfLogRegisterBackend;
	#define GET_LogRegisterBackend(fl)  s_pfCMGetAPI2( "LogRegisterBackend", (RTS_VOID_FCTPTR *)&pfLogRegisterBackend, (fl), 0, 0)
	#define CAL_LogRegisterBackend  pfLogRegisterBackend
	#define CHK_LogRegisterBackend  (pfLogRegisterBackend != NULL)
	#define EXP_LogRegisterBackend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogRegisterBackend", (RTS_UINTPTR)LogRegisterBackend, 0, 0) 
#endif




/**
 * <description>Unregister a backend at the specified logger</description>
 * <param name="hLog" type="IN">Handle to the logger. Can be RTS_INVALID_HANDLE an so the backend is unregistered from all logger instances!</param>
 * <param name="ClassId" type="IN">ClassId of the backend. ClassId must be registered previously in the backend component with CMRegisterClass!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogUnregisterBackend(RTS_HANDLE hLog, CLASSID ClassId);
typedef RTS_RESULT (CDECL * PFLOGUNREGISTERBACKEND) (RTS_HANDLE hLog, CLASSID ClassId);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGUNREGISTERBACKEND_NOTIMPLEMENTED)
	#define USE_LogUnregisterBackend
	#define EXT_LogUnregisterBackend
	#define GET_LogUnregisterBackend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogUnregisterBackend(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogUnregisterBackend  FALSE
	#define EXP_LogUnregisterBackend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogUnregisterBackend
	#define EXT_LogUnregisterBackend
	#define GET_LogUnregisterBackend(fl)  CAL_CMGETAPI( "LogUnregisterBackend" ) 
	#define CAL_LogUnregisterBackend  LogUnregisterBackend
	#define CHK_LogUnregisterBackend  TRUE
	#define EXP_LogUnregisterBackend  CAL_CMEXPAPI( "LogUnregisterBackend" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogUnregisterBackend
	#define EXT_LogUnregisterBackend
	#define GET_LogUnregisterBackend(fl)  CAL_CMGETAPI( "LogUnregisterBackend" ) 
	#define CAL_LogUnregisterBackend  LogUnregisterBackend
	#define CHK_LogUnregisterBackend  TRUE
	#define EXP_LogUnregisterBackend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogUnregisterBackend", (RTS_UINTPTR)LogUnregisterBackend, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogUnregisterBackend
	#define EXT_CmpLogLogUnregisterBackend
	#define GET_CmpLogLogUnregisterBackend  ERR_OK
	#define CAL_CmpLogLogUnregisterBackend pICmpLog->ILogUnregisterBackend
	#define CHK_CmpLogLogUnregisterBackend (pICmpLog != NULL)
	#define EXP_CmpLogLogUnregisterBackend  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogUnregisterBackend
	#define EXT_LogUnregisterBackend
	#define GET_LogUnregisterBackend(fl)  CAL_CMGETAPI( "LogUnregisterBackend" ) 
	#define CAL_LogUnregisterBackend pICmpLog->ILogUnregisterBackend
	#define CHK_LogUnregisterBackend (pICmpLog != NULL)
	#define EXP_LogUnregisterBackend  CAL_CMEXPAPI( "LogUnregisterBackend" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogUnregisterBackend  PFLOGUNREGISTERBACKEND pfLogUnregisterBackend;
	#define EXT_LogUnregisterBackend  extern PFLOGUNREGISTERBACKEND pfLogUnregisterBackend;
	#define GET_LogUnregisterBackend(fl)  s_pfCMGetAPI2( "LogUnregisterBackend", (RTS_VOID_FCTPTR *)&pfLogUnregisterBackend, (fl), 0, 0)
	#define CAL_LogUnregisterBackend  pfLogUnregisterBackend
	#define CHK_LogUnregisterBackend  (pfLogUnregisterBackend != NULL)
	#define EXP_LogUnregisterBackend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogUnregisterBackend", (RTS_UINTPTR)LogUnregisterBackend, 0, 0) 
#endif




/**
 * <description> Get the first logentry of a logger </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <param name="iIndex" type="IN">Index of entry to get. 0 is the first entry.</param>
 * <param name="pLogEntry" type="IN">Pointer to log entry</param>
 * <param name="pResult" type="IN">Pointer to result</param>
 * <result>Handle to next log entry or RTS_INVALID_HANDLE, if end of logger is reached</result>
 */
RTS_HANDLE CDECL LogGetEntryByIndex(RTS_HANDLE hLog, int iIndex, LogEntry *pLogEntry, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFLOGGETENTRYBYINDEX) (RTS_HANDLE hLog, int iIndex, LogEntry *pLogEntry, RTS_RESULT *pResult);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETENTRYBYINDEX_NOTIMPLEMENTED)
	#define USE_LogGetEntryByIndex
	#define EXT_LogGetEntryByIndex
	#define GET_LogGetEntryByIndex(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetEntryByIndex(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogGetEntryByIndex  FALSE
	#define EXP_LogGetEntryByIndex  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetEntryByIndex
	#define EXT_LogGetEntryByIndex
	#define GET_LogGetEntryByIndex(fl)  CAL_CMGETAPI( "LogGetEntryByIndex" ) 
	#define CAL_LogGetEntryByIndex  LogGetEntryByIndex
	#define CHK_LogGetEntryByIndex  TRUE
	#define EXP_LogGetEntryByIndex  CAL_CMEXPAPI( "LogGetEntryByIndex" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetEntryByIndex
	#define EXT_LogGetEntryByIndex
	#define GET_LogGetEntryByIndex(fl)  CAL_CMGETAPI( "LogGetEntryByIndex" ) 
	#define CAL_LogGetEntryByIndex  LogGetEntryByIndex
	#define CHK_LogGetEntryByIndex  TRUE
	#define EXP_LogGetEntryByIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetEntryByIndex", (RTS_UINTPTR)LogGetEntryByIndex, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetEntryByIndex
	#define EXT_CmpLogLogGetEntryByIndex
	#define GET_CmpLogLogGetEntryByIndex  ERR_OK
	#define CAL_CmpLogLogGetEntryByIndex pICmpLog->ILogGetEntryByIndex
	#define CHK_CmpLogLogGetEntryByIndex (pICmpLog != NULL)
	#define EXP_CmpLogLogGetEntryByIndex  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetEntryByIndex
	#define EXT_LogGetEntryByIndex
	#define GET_LogGetEntryByIndex(fl)  CAL_CMGETAPI( "LogGetEntryByIndex" ) 
	#define CAL_LogGetEntryByIndex pICmpLog->ILogGetEntryByIndex
	#define CHK_LogGetEntryByIndex (pICmpLog != NULL)
	#define EXP_LogGetEntryByIndex  CAL_CMEXPAPI( "LogGetEntryByIndex" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetEntryByIndex  PFLOGGETENTRYBYINDEX pfLogGetEntryByIndex;
	#define EXT_LogGetEntryByIndex  extern PFLOGGETENTRYBYINDEX pfLogGetEntryByIndex;
	#define GET_LogGetEntryByIndex(fl)  s_pfCMGetAPI2( "LogGetEntryByIndex", (RTS_VOID_FCTPTR *)&pfLogGetEntryByIndex, (fl), 0, 0)
	#define CAL_LogGetEntryByIndex  pfLogGetEntryByIndex
	#define CHK_LogGetEntryByIndex  (pfLogGetEntryByIndex != NULL)
	#define EXP_LogGetEntryByIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetEntryByIndex", (RTS_UINTPTR)LogGetEntryByIndex, 0, 0) 
#endif




/**
 * <description> Get the first logentry of a logger </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <param name="iQueueIndex" type="IN">Index of entry to get. -1 get the first entry.</param>
 * <param name="pLogEntry" type="IN">Pointer to log entry</param>
 * <param name="pResult" type="IN">Pointer to result</param>
 * <result>Handle to next log entry or RTS_INVALID_HANDLE, if end of logger is reached</result>
 */
RTS_HANDLE CDECL LogGetEntryByQueueIndex(RTS_HANDLE hLog, int iQueueIndex, LogEntry *pLogEntry, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFLOGGETENTRYBYQUEUEINDEX) (RTS_HANDLE hLog, int iQueueIndex, LogEntry *pLogEntry, RTS_RESULT *pResult);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETENTRYBYQUEUEINDEX_NOTIMPLEMENTED)
	#define USE_LogGetEntryByQueueIndex
	#define EXT_LogGetEntryByQueueIndex
	#define GET_LogGetEntryByQueueIndex(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetEntryByQueueIndex(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogGetEntryByQueueIndex  FALSE
	#define EXP_LogGetEntryByQueueIndex  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetEntryByQueueIndex
	#define EXT_LogGetEntryByQueueIndex
	#define GET_LogGetEntryByQueueIndex(fl)  CAL_CMGETAPI( "LogGetEntryByQueueIndex" ) 
	#define CAL_LogGetEntryByQueueIndex  LogGetEntryByQueueIndex
	#define CHK_LogGetEntryByQueueIndex  TRUE
	#define EXP_LogGetEntryByQueueIndex  CAL_CMEXPAPI( "LogGetEntryByQueueIndex" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetEntryByQueueIndex
	#define EXT_LogGetEntryByQueueIndex
	#define GET_LogGetEntryByQueueIndex(fl)  CAL_CMGETAPI( "LogGetEntryByQueueIndex" ) 
	#define CAL_LogGetEntryByQueueIndex  LogGetEntryByQueueIndex
	#define CHK_LogGetEntryByQueueIndex  TRUE
	#define EXP_LogGetEntryByQueueIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetEntryByQueueIndex", (RTS_UINTPTR)LogGetEntryByQueueIndex, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetEntryByQueueIndex
	#define EXT_CmpLogLogGetEntryByQueueIndex
	#define GET_CmpLogLogGetEntryByQueueIndex  ERR_OK
	#define CAL_CmpLogLogGetEntryByQueueIndex pICmpLog->ILogGetEntryByQueueIndex
	#define CHK_CmpLogLogGetEntryByQueueIndex (pICmpLog != NULL)
	#define EXP_CmpLogLogGetEntryByQueueIndex  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetEntryByQueueIndex
	#define EXT_LogGetEntryByQueueIndex
	#define GET_LogGetEntryByQueueIndex(fl)  CAL_CMGETAPI( "LogGetEntryByQueueIndex" ) 
	#define CAL_LogGetEntryByQueueIndex pICmpLog->ILogGetEntryByQueueIndex
	#define CHK_LogGetEntryByQueueIndex (pICmpLog != NULL)
	#define EXP_LogGetEntryByQueueIndex  CAL_CMEXPAPI( "LogGetEntryByQueueIndex" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetEntryByQueueIndex  PFLOGGETENTRYBYQUEUEINDEX pfLogGetEntryByQueueIndex;
	#define EXT_LogGetEntryByQueueIndex  extern PFLOGGETENTRYBYQUEUEINDEX pfLogGetEntryByQueueIndex;
	#define GET_LogGetEntryByQueueIndex(fl)  s_pfCMGetAPI2( "LogGetEntryByQueueIndex", (RTS_VOID_FCTPTR *)&pfLogGetEntryByQueueIndex, (fl), 0, 0)
	#define CAL_LogGetEntryByQueueIndex  pfLogGetEntryByQueueIndex
	#define CHK_LogGetEntryByQueueIndex  (pfLogGetEntryByQueueIndex != NULL)
	#define EXP_LogGetEntryByQueueIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetEntryByQueueIndex", (RTS_UINTPTR)LogGetEntryByQueueIndex, 0, 0) 
#endif




/**
 * <description> Get the first logentry of a logger </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <param name="pLogEntry" type="IN">Pointer to log entry</param>
 * <param name="pResult" type="IN">Pointer to result</param>
 * <result>Handle to next log entry or RTS_INVALID_HANDLE, if end of logger is reached</result>
 */
RTS_HANDLE CDECL LogGetFirstEntry(RTS_HANDLE hLog, LogEntry *pLogEntry, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFLOGGETFIRSTENTRY) (RTS_HANDLE hLog, LogEntry *pLogEntry, RTS_RESULT *pResult);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETFIRSTENTRY_NOTIMPLEMENTED)
	#define USE_LogGetFirstEntry
	#define EXT_LogGetFirstEntry
	#define GET_LogGetFirstEntry(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetFirstEntry(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogGetFirstEntry  FALSE
	#define EXP_LogGetFirstEntry  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetFirstEntry
	#define EXT_LogGetFirstEntry
	#define GET_LogGetFirstEntry(fl)  CAL_CMGETAPI( "LogGetFirstEntry" ) 
	#define CAL_LogGetFirstEntry  LogGetFirstEntry
	#define CHK_LogGetFirstEntry  TRUE
	#define EXP_LogGetFirstEntry  CAL_CMEXPAPI( "LogGetFirstEntry" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetFirstEntry
	#define EXT_LogGetFirstEntry
	#define GET_LogGetFirstEntry(fl)  CAL_CMGETAPI( "LogGetFirstEntry" ) 
	#define CAL_LogGetFirstEntry  LogGetFirstEntry
	#define CHK_LogGetFirstEntry  TRUE
	#define EXP_LogGetFirstEntry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetFirstEntry", (RTS_UINTPTR)LogGetFirstEntry, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetFirstEntry
	#define EXT_CmpLogLogGetFirstEntry
	#define GET_CmpLogLogGetFirstEntry  ERR_OK
	#define CAL_CmpLogLogGetFirstEntry pICmpLog->ILogGetFirstEntry
	#define CHK_CmpLogLogGetFirstEntry (pICmpLog != NULL)
	#define EXP_CmpLogLogGetFirstEntry  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetFirstEntry
	#define EXT_LogGetFirstEntry
	#define GET_LogGetFirstEntry(fl)  CAL_CMGETAPI( "LogGetFirstEntry" ) 
	#define CAL_LogGetFirstEntry pICmpLog->ILogGetFirstEntry
	#define CHK_LogGetFirstEntry (pICmpLog != NULL)
	#define EXP_LogGetFirstEntry  CAL_CMEXPAPI( "LogGetFirstEntry" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetFirstEntry  PFLOGGETFIRSTENTRY pfLogGetFirstEntry;
	#define EXT_LogGetFirstEntry  extern PFLOGGETFIRSTENTRY pfLogGetFirstEntry;
	#define GET_LogGetFirstEntry(fl)  s_pfCMGetAPI2( "LogGetFirstEntry", (RTS_VOID_FCTPTR *)&pfLogGetFirstEntry, (fl), 0, 0)
	#define CAL_LogGetFirstEntry  pfLogGetFirstEntry
	#define CHK_LogGetFirstEntry  (pfLogGetFirstEntry != NULL)
	#define EXP_LogGetFirstEntry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetFirstEntry", (RTS_UINTPTR)LogGetFirstEntry, 0, 0) 
#endif




/**
 * <description> Get the next logentry of a logger </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <param name="hEntry" type="IN">Handle to log entry (is returned by LogGetFirstEntry or LogGetNextEntry)</param>
 * <param name="pLogEntry" type="IN">Pointer to log entry</param>
 * <param name="pResult" type="IN">Pointer to result</param>
 * <result>Handle to next log entry or RTS_INVALID_HANDLE, if end of logger is reached</result>
 */
RTS_HANDLE CDECL LogGetNextEntry(RTS_HANDLE hLog, RTS_HANDLE hEntry, LogEntry *pLogEntry, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFLOGGETNEXTENTRY) (RTS_HANDLE hLog, RTS_HANDLE hEntry, LogEntry *pLogEntry, RTS_RESULT *pResult);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETNEXTENTRY_NOTIMPLEMENTED)
	#define USE_LogGetNextEntry
	#define EXT_LogGetNextEntry
	#define GET_LogGetNextEntry(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetNextEntry(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogGetNextEntry  FALSE
	#define EXP_LogGetNextEntry  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetNextEntry
	#define EXT_LogGetNextEntry
	#define GET_LogGetNextEntry(fl)  CAL_CMGETAPI( "LogGetNextEntry" ) 
	#define CAL_LogGetNextEntry  LogGetNextEntry
	#define CHK_LogGetNextEntry  TRUE
	#define EXP_LogGetNextEntry  CAL_CMEXPAPI( "LogGetNextEntry" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetNextEntry
	#define EXT_LogGetNextEntry
	#define GET_LogGetNextEntry(fl)  CAL_CMGETAPI( "LogGetNextEntry" ) 
	#define CAL_LogGetNextEntry  LogGetNextEntry
	#define CHK_LogGetNextEntry  TRUE
	#define EXP_LogGetNextEntry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetNextEntry", (RTS_UINTPTR)LogGetNextEntry, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetNextEntry
	#define EXT_CmpLogLogGetNextEntry
	#define GET_CmpLogLogGetNextEntry  ERR_OK
	#define CAL_CmpLogLogGetNextEntry pICmpLog->ILogGetNextEntry
	#define CHK_CmpLogLogGetNextEntry (pICmpLog != NULL)
	#define EXP_CmpLogLogGetNextEntry  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetNextEntry
	#define EXT_LogGetNextEntry
	#define GET_LogGetNextEntry(fl)  CAL_CMGETAPI( "LogGetNextEntry" ) 
	#define CAL_LogGetNextEntry pICmpLog->ILogGetNextEntry
	#define CHK_LogGetNextEntry (pICmpLog != NULL)
	#define EXP_LogGetNextEntry  CAL_CMEXPAPI( "LogGetNextEntry" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetNextEntry  PFLOGGETNEXTENTRY pfLogGetNextEntry;
	#define EXT_LogGetNextEntry  extern PFLOGGETNEXTENTRY pfLogGetNextEntry;
	#define GET_LogGetNextEntry(fl)  s_pfCMGetAPI2( "LogGetNextEntry", (RTS_VOID_FCTPTR *)&pfLogGetNextEntry, (fl), 0, 0)
	#define CAL_LogGetNextEntry  pfLogGetNextEntry
	#define CHK_LogGetNextEntry  (pfLogGetNextEntry != NULL)
	#define EXP_LogGetNextEntry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetNextEntry", (RTS_UINTPTR)LogGetNextEntry, 0, 0) 
#endif




/**
 * <description> Get an entry specified by handle</description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <param name="hEntry" type="IN">Handle to log entry (is returned by LogGetFirstEntry or LogGetNextEntry)</param>
 * <param name="pLogEntry" type="IN">Pointer to log entry</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogGetEntry(RTS_HANDLE hLog, RTS_HANDLE hEntry, LogEntry *pLogEntry);
typedef RTS_RESULT (CDECL * PFLOGGETENTRY) (RTS_HANDLE hLog, RTS_HANDLE hEntry, LogEntry *pLogEntry);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETENTRY_NOTIMPLEMENTED)
	#define USE_LogGetEntry
	#define EXT_LogGetEntry
	#define GET_LogGetEntry(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetEntry(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogGetEntry  FALSE
	#define EXP_LogGetEntry  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetEntry
	#define EXT_LogGetEntry
	#define GET_LogGetEntry(fl)  CAL_CMGETAPI( "LogGetEntry" ) 
	#define CAL_LogGetEntry  LogGetEntry
	#define CHK_LogGetEntry  TRUE
	#define EXP_LogGetEntry  CAL_CMEXPAPI( "LogGetEntry" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetEntry
	#define EXT_LogGetEntry
	#define GET_LogGetEntry(fl)  CAL_CMGETAPI( "LogGetEntry" ) 
	#define CAL_LogGetEntry  LogGetEntry
	#define CHK_LogGetEntry  TRUE
	#define EXP_LogGetEntry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetEntry", (RTS_UINTPTR)LogGetEntry, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetEntry
	#define EXT_CmpLogLogGetEntry
	#define GET_CmpLogLogGetEntry  ERR_OK
	#define CAL_CmpLogLogGetEntry pICmpLog->ILogGetEntry
	#define CHK_CmpLogLogGetEntry (pICmpLog != NULL)
	#define EXP_CmpLogLogGetEntry  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetEntry
	#define EXT_LogGetEntry
	#define GET_LogGetEntry(fl)  CAL_CMGETAPI( "LogGetEntry" ) 
	#define CAL_LogGetEntry pICmpLog->ILogGetEntry
	#define CHK_LogGetEntry (pICmpLog != NULL)
	#define EXP_LogGetEntry  CAL_CMEXPAPI( "LogGetEntry" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetEntry  PFLOGGETENTRY pfLogGetEntry;
	#define EXT_LogGetEntry  extern PFLOGGETENTRY pfLogGetEntry;
	#define GET_LogGetEntry(fl)  s_pfCMGetAPI2( "LogGetEntry", (RTS_VOID_FCTPTR *)&pfLogGetEntry, (fl), 0, 0)
	#define CAL_LogGetEntry  pfLogGetEntry
	#define CHK_LogGetEntry  (pfLogGetEntry != NULL)
	#define EXP_LogGetEntry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetEntry", (RTS_UINTPTR)LogGetEntry, 0, 0) 
#endif




/**
 * <description>Get the first registered logger</description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the first logger</result>
 */
RTS_HANDLE CDECL LogGetFirstLogger(RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFLOGGETFIRSTLOGGER) (RTS_RESULT *pResult);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETFIRSTLOGGER_NOTIMPLEMENTED)
	#define USE_LogGetFirstLogger
	#define EXT_LogGetFirstLogger
	#define GET_LogGetFirstLogger(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetFirstLogger(p0)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogGetFirstLogger  FALSE
	#define EXP_LogGetFirstLogger  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetFirstLogger
	#define EXT_LogGetFirstLogger
	#define GET_LogGetFirstLogger(fl)  CAL_CMGETAPI( "LogGetFirstLogger" ) 
	#define CAL_LogGetFirstLogger  LogGetFirstLogger
	#define CHK_LogGetFirstLogger  TRUE
	#define EXP_LogGetFirstLogger  CAL_CMEXPAPI( "LogGetFirstLogger" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetFirstLogger
	#define EXT_LogGetFirstLogger
	#define GET_LogGetFirstLogger(fl)  CAL_CMGETAPI( "LogGetFirstLogger" ) 
	#define CAL_LogGetFirstLogger  LogGetFirstLogger
	#define CHK_LogGetFirstLogger  TRUE
	#define EXP_LogGetFirstLogger  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetFirstLogger", (RTS_UINTPTR)LogGetFirstLogger, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetFirstLogger
	#define EXT_CmpLogLogGetFirstLogger
	#define GET_CmpLogLogGetFirstLogger  ERR_OK
	#define CAL_CmpLogLogGetFirstLogger pICmpLog->ILogGetFirstLogger
	#define CHK_CmpLogLogGetFirstLogger (pICmpLog != NULL)
	#define EXP_CmpLogLogGetFirstLogger  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetFirstLogger
	#define EXT_LogGetFirstLogger
	#define GET_LogGetFirstLogger(fl)  CAL_CMGETAPI( "LogGetFirstLogger" ) 
	#define CAL_LogGetFirstLogger pICmpLog->ILogGetFirstLogger
	#define CHK_LogGetFirstLogger (pICmpLog != NULL)
	#define EXP_LogGetFirstLogger  CAL_CMEXPAPI( "LogGetFirstLogger" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetFirstLogger  PFLOGGETFIRSTLOGGER pfLogGetFirstLogger;
	#define EXT_LogGetFirstLogger  extern PFLOGGETFIRSTLOGGER pfLogGetFirstLogger;
	#define GET_LogGetFirstLogger(fl)  s_pfCMGetAPI2( "LogGetFirstLogger", (RTS_VOID_FCTPTR *)&pfLogGetFirstLogger, (fl), 0, 0)
	#define CAL_LogGetFirstLogger  pfLogGetFirstLogger
	#define CHK_LogGetFirstLogger  (pfLogGetFirstLogger != NULL)
	#define EXP_LogGetFirstLogger  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetFirstLogger", (RTS_UINTPTR)LogGetFirstLogger, 0, 0) 
#endif




/**
 * <description>Get the next registered logger</description>
 * <param name="hLogger" type="IN">Handle to previous logger</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the first logger</result>
 */
RTS_HANDLE CDECL LogGetNextLogger(RTS_HANDLE hLogger, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFLOGGETNEXTLOGGER) (RTS_HANDLE hLogger, RTS_RESULT *pResult);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETNEXTLOGGER_NOTIMPLEMENTED)
	#define USE_LogGetNextLogger
	#define EXT_LogGetNextLogger
	#define GET_LogGetNextLogger(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetNextLogger(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogGetNextLogger  FALSE
	#define EXP_LogGetNextLogger  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetNextLogger
	#define EXT_LogGetNextLogger
	#define GET_LogGetNextLogger(fl)  CAL_CMGETAPI( "LogGetNextLogger" ) 
	#define CAL_LogGetNextLogger  LogGetNextLogger
	#define CHK_LogGetNextLogger  TRUE
	#define EXP_LogGetNextLogger  CAL_CMEXPAPI( "LogGetNextLogger" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetNextLogger
	#define EXT_LogGetNextLogger
	#define GET_LogGetNextLogger(fl)  CAL_CMGETAPI( "LogGetNextLogger" ) 
	#define CAL_LogGetNextLogger  LogGetNextLogger
	#define CHK_LogGetNextLogger  TRUE
	#define EXP_LogGetNextLogger  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetNextLogger", (RTS_UINTPTR)LogGetNextLogger, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetNextLogger
	#define EXT_CmpLogLogGetNextLogger
	#define GET_CmpLogLogGetNextLogger  ERR_OK
	#define CAL_CmpLogLogGetNextLogger pICmpLog->ILogGetNextLogger
	#define CHK_CmpLogLogGetNextLogger (pICmpLog != NULL)
	#define EXP_CmpLogLogGetNextLogger  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetNextLogger
	#define EXT_LogGetNextLogger
	#define GET_LogGetNextLogger(fl)  CAL_CMGETAPI( "LogGetNextLogger" ) 
	#define CAL_LogGetNextLogger pICmpLog->ILogGetNextLogger
	#define CHK_LogGetNextLogger (pICmpLog != NULL)
	#define EXP_LogGetNextLogger  CAL_CMEXPAPI( "LogGetNextLogger" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetNextLogger  PFLOGGETNEXTLOGGER pfLogGetNextLogger;
	#define EXT_LogGetNextLogger  extern PFLOGGETNEXTLOGGER pfLogGetNextLogger;
	#define GET_LogGetNextLogger(fl)  s_pfCMGetAPI2( "LogGetNextLogger", (RTS_VOID_FCTPTR *)&pfLogGetNextLogger, (fl), 0, 0)
	#define CAL_LogGetNextLogger  pfLogGetNextLogger
	#define CHK_LogGetNextLogger  (pfLogGetNextLogger != NULL)
	#define EXP_LogGetNextLogger  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetNextLogger", (RTS_UINTPTR)LogGetNextLogger, 0, 0) 
#endif




/**
 * <description>Get the logger name of the logger specified by handle</description>
 * <param name="hLogger" type="IN">Handle to the logger</param>
 * <param name="pszLogger" type="INOUT">Pointer to logger name</param>
 * <param name="nMaxLen" type="IN">Max length of pszLogger</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL LogGetName(RTS_HANDLE hLog, char *pszLoggerName, int nMaxLen);
typedef RTS_RESULT (CDECL * PFLOGGETNAME) (RTS_HANDLE hLog, char *pszLoggerName, int nMaxLen);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETNAME_NOTIMPLEMENTED)
	#define USE_LogGetName
	#define EXT_LogGetName
	#define GET_LogGetName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogGetName  FALSE
	#define EXP_LogGetName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetName
	#define EXT_LogGetName
	#define GET_LogGetName(fl)  CAL_CMGETAPI( "LogGetName" ) 
	#define CAL_LogGetName  LogGetName
	#define CHK_LogGetName  TRUE
	#define EXP_LogGetName  CAL_CMEXPAPI( "LogGetName" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetName
	#define EXT_LogGetName
	#define GET_LogGetName(fl)  CAL_CMGETAPI( "LogGetName" ) 
	#define CAL_LogGetName  LogGetName
	#define CHK_LogGetName  TRUE
	#define EXP_LogGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetName", (RTS_UINTPTR)LogGetName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetName
	#define EXT_CmpLogLogGetName
	#define GET_CmpLogLogGetName  ERR_OK
	#define CAL_CmpLogLogGetName pICmpLog->ILogGetName
	#define CHK_CmpLogLogGetName (pICmpLog != NULL)
	#define EXP_CmpLogLogGetName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetName
	#define EXT_LogGetName
	#define GET_LogGetName(fl)  CAL_CMGETAPI( "LogGetName" ) 
	#define CAL_LogGetName pICmpLog->ILogGetName
	#define CHK_LogGetName (pICmpLog != NULL)
	#define EXP_LogGetName  CAL_CMEXPAPI( "LogGetName" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetName  PFLOGGETNAME pfLogGetName;
	#define EXT_LogGetName  extern PFLOGGETNAME pfLogGetName;
	#define GET_LogGetName(fl)  s_pfCMGetAPI2( "LogGetName", (RTS_VOID_FCTPTR *)&pfLogGetName, (fl), 0, 0)
	#define CAL_LogGetName  pfLogGetName
	#define CHK_LogGetName  (pfLogGetName != NULL)
	#define EXP_LogGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetName", (RTS_UINTPTR)LogGetName, 0, 0) 
#endif




/**
 * <description>Get the last log entry of class LOG_USER_NOTIFY</description>
 * <param name="ppLogEntryUserNotify" type="OUT">Returns the pointer to the user notify entry</param>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_OK: There is still an unread log entry of the type LOG_USER_NOTIFY</li>
 *		<li>ERR_NO_OBJECT: No pending log entry of the type LOG_USER_NOTIFY</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL LogGetUserNotify(LogEntry **ppLogEntryUserNotify);
typedef RTS_RESULT (CDECL * PFLOGGETUSERNOTIFY) (LogEntry **ppLogEntryUserNotify);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGGETUSERNOTIFY_NOTIMPLEMENTED)
	#define USE_LogGetUserNotify
	#define EXT_LogGetUserNotify
	#define GET_LogGetUserNotify(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogGetUserNotify(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogGetUserNotify  FALSE
	#define EXP_LogGetUserNotify  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogGetUserNotify
	#define EXT_LogGetUserNotify
	#define GET_LogGetUserNotify(fl)  CAL_CMGETAPI( "LogGetUserNotify" ) 
	#define CAL_LogGetUserNotify  LogGetUserNotify
	#define CHK_LogGetUserNotify  TRUE
	#define EXP_LogGetUserNotify  CAL_CMEXPAPI( "LogGetUserNotify" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogGetUserNotify
	#define EXT_LogGetUserNotify
	#define GET_LogGetUserNotify(fl)  CAL_CMGETAPI( "LogGetUserNotify" ) 
	#define CAL_LogGetUserNotify  LogGetUserNotify
	#define CHK_LogGetUserNotify  TRUE
	#define EXP_LogGetUserNotify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetUserNotify", (RTS_UINTPTR)LogGetUserNotify, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogGetUserNotify
	#define EXT_CmpLogLogGetUserNotify
	#define GET_CmpLogLogGetUserNotify  ERR_OK
	#define CAL_CmpLogLogGetUserNotify pICmpLog->ILogGetUserNotify
	#define CHK_CmpLogLogGetUserNotify (pICmpLog != NULL)
	#define EXP_CmpLogLogGetUserNotify  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogGetUserNotify
	#define EXT_LogGetUserNotify
	#define GET_LogGetUserNotify(fl)  CAL_CMGETAPI( "LogGetUserNotify" ) 
	#define CAL_LogGetUserNotify pICmpLog->ILogGetUserNotify
	#define CHK_LogGetUserNotify (pICmpLog != NULL)
	#define EXP_LogGetUserNotify  CAL_CMEXPAPI( "LogGetUserNotify" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogGetUserNotify  PFLOGGETUSERNOTIFY pfLogGetUserNotify;
	#define EXT_LogGetUserNotify  extern PFLOGGETUSERNOTIFY pfLogGetUserNotify;
	#define GET_LogGetUserNotify(fl)  s_pfCMGetAPI2( "LogGetUserNotify", (RTS_VOID_FCTPTR *)&pfLogGetUserNotify, (fl), 0, 0)
	#define CAL_LogGetUserNotify  pfLogGetUserNotify
	#define CHK_LogGetUserNotify  (pfLogGetUserNotify != NULL)
	#define EXP_LogGetUserNotify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogGetUserNotify", (RTS_UINTPTR)LogGetUserNotify, 0, 0) 
#endif




/**
 * <description>Convert the timestamp of a log entry into the corresponding structured RTS_SYSTIMEDATE format</description>
 * <param name="hLog" type="IN">Handle to the logger</param>
 * <param name="tTimestamp" type="IN">Timestamp of the log entry</param>
 * <param name="pDate" type="OUT">Return the converted timestamp</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Timestamp type of the logger. See "Log types" for details</result>
 */
RTS_UI32 CDECL LogConvertTimestamp(RTS_HANDLE hLog, LogTimestamp tTimestamp, RTS_SYSTIMEDATE *pDate, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFLOGCONVERTTIMESTAMP) (RTS_HANDLE hLog, LogTimestamp tTimestamp, RTS_SYSTIMEDATE *pDate, RTS_RESULT *pResult);
#if defined(CMPLOG_NOTIMPLEMENTED) || defined(LOGCONVERTTIMESTAMP_NOTIMPLEMENTED)
	#define USE_LogConvertTimestamp
	#define EXT_LogConvertTimestamp
	#define GET_LogConvertTimestamp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogConvertTimestamp(p0,p1,p2,p3)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_LogConvertTimestamp  FALSE
	#define EXP_LogConvertTimestamp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogConvertTimestamp
	#define EXT_LogConvertTimestamp
	#define GET_LogConvertTimestamp(fl)  CAL_CMGETAPI( "LogConvertTimestamp" ) 
	#define CAL_LogConvertTimestamp  LogConvertTimestamp
	#define CHK_LogConvertTimestamp  TRUE
	#define EXP_LogConvertTimestamp  CAL_CMEXPAPI( "LogConvertTimestamp" ) 
#elif defined(MIXED_LINK) && !defined(CMPLOG_EXTERNAL)
	#define USE_LogConvertTimestamp
	#define EXT_LogConvertTimestamp
	#define GET_LogConvertTimestamp(fl)  CAL_CMGETAPI( "LogConvertTimestamp" ) 
	#define CAL_LogConvertTimestamp  LogConvertTimestamp
	#define CHK_LogConvertTimestamp  TRUE
	#define EXP_LogConvertTimestamp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogConvertTimestamp", (RTS_UINTPTR)LogConvertTimestamp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogLogConvertTimestamp
	#define EXT_CmpLogLogConvertTimestamp
	#define GET_CmpLogLogConvertTimestamp  ERR_OK
	#define CAL_CmpLogLogConvertTimestamp pICmpLog->ILogConvertTimestamp
	#define CHK_CmpLogLogConvertTimestamp (pICmpLog != NULL)
	#define EXP_CmpLogLogConvertTimestamp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogConvertTimestamp
	#define EXT_LogConvertTimestamp
	#define GET_LogConvertTimestamp(fl)  CAL_CMGETAPI( "LogConvertTimestamp" ) 
	#define CAL_LogConvertTimestamp pICmpLog->ILogConvertTimestamp
	#define CHK_LogConvertTimestamp (pICmpLog != NULL)
	#define EXP_LogConvertTimestamp  CAL_CMEXPAPI( "LogConvertTimestamp" ) 
#else /* DYNAMIC_LINK */
	#define USE_LogConvertTimestamp  PFLOGCONVERTTIMESTAMP pfLogConvertTimestamp;
	#define EXT_LogConvertTimestamp  extern PFLOGCONVERTTIMESTAMP pfLogConvertTimestamp;
	#define GET_LogConvertTimestamp(fl)  s_pfCMGetAPI2( "LogConvertTimestamp", (RTS_VOID_FCTPTR *)&pfLogConvertTimestamp, (fl), 0, 0)
	#define CAL_LogConvertTimestamp  pfLogConvertTimestamp
	#define CHK_LogConvertTimestamp  (pfLogConvertTimestamp != NULL)
	#define EXP_LogConvertTimestamp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"LogConvertTimestamp", (RTS_UINTPTR)LogConvertTimestamp, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif




typedef struct
{
	IBase_C *pBase;
	PFLOGCREATE ILogCreate;
 	PFLOGOPEN ILogOpen;
 	PFLOGCLOSE ILogClose;
 	PFLOGDELETE ILogDelete;
 	PFLOGGETOPTIONS ILogGetOptions;
 	PFLOGENABLE ILogEnable;
 	PFLOGDISABLE ILogDisable;
 	PFLOGSETFILTER ILogSetFilter;
 	PFLOGGETFILTER ILogGetFilter;
 	PFLOGADD ILogAdd;
 	PFLOGADDARG ILogAddArg;
 	PFLOGDUMPALL ILogDumpAll;
 	PFLOGDUMPENTRIES ILogDumpEntries;
 	PFLOGREGISTERINTERFACE ILogRegisterInterface;
 	PFLOGUNREGISTERINTERFACE ILogUnregisterInterface;
 	PFLOGREGISTERBACKEND ILogRegisterBackend;
 	PFLOGUNREGISTERBACKEND ILogUnregisterBackend;
 	PFLOGGETENTRYBYINDEX ILogGetEntryByIndex;
 	PFLOGGETENTRYBYQUEUEINDEX ILogGetEntryByQueueIndex;
 	PFLOGGETFIRSTENTRY ILogGetFirstEntry;
 	PFLOGGETNEXTENTRY ILogGetNextEntry;
 	PFLOGGETENTRY ILogGetEntry;
 	PFLOGGETFIRSTLOGGER ILogGetFirstLogger;
 	PFLOGGETNEXTLOGGER ILogGetNextLogger;
 	PFLOGGETNAME ILogGetName;
 	PFLOGGETUSERNOTIFY ILogGetUserNotify;
 	PFLOGCONVERTTIMESTAMP ILogConvertTimestamp;
 } ICmpLog_C;

#ifdef CPLUSPLUS
class ICmpLog : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ILogCreate(LogOptions *pOptions, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ILogOpen(char *pszName, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ILogClose(RTS_HANDLE hLog) =0;
		virtual RTS_RESULT CDECL ILogDelete(RTS_HANDLE hLog) =0;
		virtual RTS_RESULT CDECL ILogGetOptions(RTS_HANDLE hLog, LogOptions **ppOptions) =0;
		virtual RTS_RESULT CDECL ILogEnable(RTS_HANDLE hLog) =0;
		virtual RTS_RESULT CDECL ILogDisable(RTS_HANDLE hLog) =0;
		virtual RTS_RESULT CDECL ILogSetFilter(RTS_HANDLE hLog, RTS_I32 iFilter) =0;
		virtual RTS_I32 CDECL ILogGetFilter(RTS_HANDLE hLog) =0;
		virtual RTS_RESULT CDECL ILogAdd(RTS_HANDLE hLog, CMPID CmpId, RTS_I32 iClassID, RTS_RESULT iErrorID, RTS_I32 iInfoID, char *pszInfo, ...) =0;
		virtual RTS_RESULT CDECL ILogAddArg(RTS_HANDLE hLog, CMPID CmpId, RTS_I32 iClassID, RTS_RESULT iErrorID, RTS_I32 iInfoID, char *pszInfo, va_list *pargList) =0;
		virtual RTS_RESULT CDECL ILogDumpAll(int iOptions) =0;
		virtual RTS_RESULT CDECL ILogDumpEntries(RTS_HANDLE hLog) =0;
		virtual RTS_RESULT CDECL ILogRegisterInterface(RTS_HANDLE hLog, CLASSID ClassId, ICmpLogBackend *pIBackend) =0;
		virtual RTS_RESULT CDECL ILogUnregisterInterface(RTS_HANDLE hLog, ICmpLogBackend *pIBackend) =0;
		virtual RTS_RESULT CDECL ILogRegisterBackend(RTS_HANDLE hLog, CLASSID ClassId) =0;
		virtual RTS_RESULT CDECL ILogUnregisterBackend(RTS_HANDLE hLog, CLASSID ClassId) =0;
		virtual RTS_HANDLE CDECL ILogGetEntryByIndex(RTS_HANDLE hLog, int iIndex, LogEntry *pLogEntry, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ILogGetEntryByQueueIndex(RTS_HANDLE hLog, int iQueueIndex, LogEntry *pLogEntry, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ILogGetFirstEntry(RTS_HANDLE hLog, LogEntry *pLogEntry, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ILogGetNextEntry(RTS_HANDLE hLog, RTS_HANDLE hEntry, LogEntry *pLogEntry, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ILogGetEntry(RTS_HANDLE hLog, RTS_HANDLE hEntry, LogEntry *pLogEntry) =0;
		virtual RTS_HANDLE CDECL ILogGetFirstLogger(RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ILogGetNextLogger(RTS_HANDLE hLogger, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ILogGetName(RTS_HANDLE hLog, char *pszLoggerName, int nMaxLen) =0;
		virtual RTS_RESULT CDECL ILogGetUserNotify(LogEntry **ppLogEntryUserNotify) =0;
		virtual RTS_UI32 CDECL ILogConvertTimestamp(RTS_HANDLE hLog, LogTimestamp tTimestamp, RTS_SYSTIMEDATE *pDate, RTS_RESULT *pResult) =0;
};
	#ifndef ITF_CmpLog
		#define ITF_CmpLog static ICmpLog *pICmpLog = NULL;
	#endif
	#define EXTITF_CmpLog
#else	/*CPLUSPLUS*/
	typedef ICmpLog_C		ICmpLog;
	#ifndef ITF_CmpLog
		#define ITF_CmpLog
	#endif
	#define EXTITF_CmpLog
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPLOGITF_H_*/
