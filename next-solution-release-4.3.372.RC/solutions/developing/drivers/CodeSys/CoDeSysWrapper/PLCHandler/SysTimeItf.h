 /**
 * <interfacename>SysTime</interfacename>
 * <description>
 * <p>The SysTime interface is projected to get access to time tick values with 
 * different resolutions (millisecond, microsecond, nanosecond).</p>
 * <p>All different ticks are wrapping around their natural data type limits
 * (RTS_UI32 or RTS_SYSTIME). Therefore, they can be used to measure
 * difference timings. They can't either be used to measure absolute timings
 * nor can they be calculated from one to each other.</p>
 * <p>Implementation Notes:</p>
 * <ul>
 *   <li>All three different timers need their own handling of wrap arounds,
 *   because they have a wrap around at different points in time at different
 *   boundaries. In practice, this implies the need for an own, static offset
 *   counter for every timer.</li>
 *   <li>If the timer is based on a periodic interrupt (e.g. millisecond tick),
 *   it works only as long as no interrupt lock is held. This might be the
 *   case in some flash drivers for example.</li>
 *   <li>Some timers are called very regularly. So if the wrap around of the
 *   timer source itself is very late (low frequency + large timer register),
 *   it might be enough for the system to detect timer overruns only at every
 *   call.</li>
 * </ul>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _SYSTIMEITF_H_
#define _SYSTIMEITF_H_

#include "CmpStd.h"

 

 




#include "CmpStd.h"

#ifndef CDECL
	#define CDECL
#endif

#if !defined(_RTS_SYSTIME)
	#define _RTS_SYSTIME
	typedef RTS_UI64			RTS_SYSTIME;
	#define RTS_SYSTIME_MAX		(~((RTS_SYSTIME)0))
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Setting to specify a threshold for the jump distance from one call to GetPerformanceCounterValue to the next.</description>
 */
#define SYSTIMEKEY_INT_WINCE_TIME_PERFORMANCE_COUNTER_THRESHOLD			"WinCE.MaxDistancePC"
#define SYSTIMEVALUE_INT_TIME_PERFORMANCE_COUNTER_THRESHOLD				0

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Setting to specify whether SysTimeGetMs under Windows CE should take the time from the scheduler ticks.</description>
 */
#define SYSTIMEKEY_INT_WINCE_TIME_MS_FROM_SCHEDULER						"WinCE.GetMsTimeFromSchedulerTicks"
#define SYSTIMEVALUE_INT_WINCE_TIME_MS_FROM_SCHEDULER					0

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting to specify a customerspecific function for SysTimeGetUs (VxWorks only).</description>
 */
#define SYSTIMEKEY_INT_VXWORKS_CUSTOMSYSTIMEGETUS				"VxWorks.CustomSysTimeGetUs"
#define SYSTIMEVALUE_INT_VXWORKS_CUSTOMSYSTIMEGETUS				""

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting to specify a customerspecific function for SysTimeGetNs (VxWorks only).</description>
 */
#define SYSTIMEKEY_INT_VXWORKS_CUSTOMSYSTIMEGETNS				"VxWorks.CustomSysTimeGetNs"
#define SYSTIMEVALUE_INT_VXWORKS_CUSTOMSYSTIMEGETNS				""

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Setting to allow CODESYS usage of Timestamp (VxWorks only).</description>
 */
#define SYSTIMEKEY_INT_VXWORKS_USE_TIMESTAMP				"VxWorks.UseTimestamp"
#define SYSTIMEKEY_INT_VXWORKS_USE_TIMESTAMP_DEFAULT 		1

/** EXTERN LIB SECTION BEGIN **/

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <SIL2/>
 * Returns a monotonic rising millisecond tick. This tick can be used for timeout and
 * relative time measurements.
 * NOTE: The realtime clock is not influencing this tick!
 * RETURN: Returns the millisecond tick
 */
typedef struct tagsystimegetms_struct
{
	RTS_IEC_UDINT SysTimeGetMs;			/* VAR_OUTPUT */	
} systimegetms_struct;

void CDECL CDECL_EXT systimegetms(systimegetms_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMEGETMS_IEC) (systimegetms_struct *p);
#if defined(SYSTIME_NOTIMPLEMENTED) || defined(SYSTIMEGETMS_NOTIMPLEMENTED)
	#define USE_systimegetms
	#define EXT_systimegetms
	#define GET_systimegetms(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimegetms(p0) 
	#define CHK_systimegetms  FALSE
	#define EXP_systimegetms  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimegetms
	#define EXT_systimegetms
	#define GET_systimegetms(fl)  CAL_CMGETAPI( "systimegetms" ) 
	#define CAL_systimegetms  systimegetms
	#define CHK_systimegetms  TRUE
	#define EXP_systimegetms  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetms", (RTS_UINTPTR)systimegetms, 1, RTSITF_GET_SIGNATURE(0, 0x9306CA2B), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIME_EXTERNAL)
	#define USE_systimegetms
	#define EXT_systimegetms
	#define GET_systimegetms(fl)  CAL_CMGETAPI( "systimegetms" ) 
	#define CAL_systimegetms  systimegetms
	#define CHK_systimegetms  TRUE
	#define EXP_systimegetms  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetms", (RTS_UINTPTR)systimegetms, 1, RTSITF_GET_SIGNATURE(0, 0x9306CA2B), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimesystimegetms
	#define EXT_SysTimesystimegetms
	#define GET_SysTimesystimegetms  ERR_OK
	#define CAL_SysTimesystimegetms  systimegetms
	#define CHK_SysTimesystimegetms  TRUE
	#define EXP_SysTimesystimegetms  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetms", (RTS_UINTPTR)systimegetms, 1, RTSITF_GET_SIGNATURE(0, 0x9306CA2B), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimegetms
	#define EXT_systimegetms
	#define GET_systimegetms(fl)  CAL_CMGETAPI( "systimegetms" ) 
	#define CAL_systimegetms  systimegetms
	#define CHK_systimegetms  TRUE
	#define EXP_systimegetms  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetms", (RTS_UINTPTR)systimegetms, 1, RTSITF_GET_SIGNATURE(0, 0x9306CA2B), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimegetms  PFSYSTIMEGETMS_IEC pfsystimegetms;
	#define EXT_systimegetms  extern PFSYSTIMEGETMS_IEC pfsystimegetms;
	#define GET_systimegetms(fl)  s_pfCMGetAPI2( "systimegetms", (RTS_VOID_FCTPTR *)&pfsystimegetms, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x9306CA2B), 0x03050500)
	#define CAL_systimegetms  pfsystimegetms
	#define CHK_systimegetms  (pfsystimegetms != NULL)
	#define EXP_systimegetms   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetms", (RTS_UINTPTR)systimegetms, 1, RTSITF_GET_SIGNATURE(0, 0x9306CA2B), 0x03050500) 
#endif


/**
 * <SIL2/>
 * Returns a monotonic rising nanosecond tick. This tick can be used for very high
 * resolution time measurements
 * NOTE: The realtime clock does not influences this tick!
 * RETURN: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsystimegetns_struct
{
	RTS_IEC_ULINT *pUsTime;				/* VAR_IN_OUT */	/* Time tick result in nanoseconds */
	RTS_IEC_RESULT SysTimeGetNs;		/* VAR_OUTPUT */	
} systimegetns_struct;

void CDECL CDECL_EXT systimegetns(systimegetns_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMEGETNS_IEC) (systimegetns_struct *p);
#if defined(SYSTIME_NOTIMPLEMENTED) || defined(SYSTIMEGETNS_NOTIMPLEMENTED)
	#define USE_systimegetns
	#define EXT_systimegetns
	#define GET_systimegetns(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimegetns(p0) 
	#define CHK_systimegetns  FALSE
	#define EXP_systimegetns  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimegetns
	#define EXT_systimegetns
	#define GET_systimegetns(fl)  CAL_CMGETAPI( "systimegetns" ) 
	#define CAL_systimegetns  systimegetns
	#define CHK_systimegetns  TRUE
	#define EXP_systimegetns  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetns", (RTS_UINTPTR)systimegetns, 1, RTSITF_GET_SIGNATURE(0, 0x7EED4E09), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIME_EXTERNAL)
	#define USE_systimegetns
	#define EXT_systimegetns
	#define GET_systimegetns(fl)  CAL_CMGETAPI( "systimegetns" ) 
	#define CAL_systimegetns  systimegetns
	#define CHK_systimegetns  TRUE
	#define EXP_systimegetns  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetns", (RTS_UINTPTR)systimegetns, 1, RTSITF_GET_SIGNATURE(0, 0x7EED4E09), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimesystimegetns
	#define EXT_SysTimesystimegetns
	#define GET_SysTimesystimegetns  ERR_OK
	#define CAL_SysTimesystimegetns  systimegetns
	#define CHK_SysTimesystimegetns  TRUE
	#define EXP_SysTimesystimegetns  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetns", (RTS_UINTPTR)systimegetns, 1, RTSITF_GET_SIGNATURE(0, 0x7EED4E09), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimegetns
	#define EXT_systimegetns
	#define GET_systimegetns(fl)  CAL_CMGETAPI( "systimegetns" ) 
	#define CAL_systimegetns  systimegetns
	#define CHK_systimegetns  TRUE
	#define EXP_systimegetns  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetns", (RTS_UINTPTR)systimegetns, 1, RTSITF_GET_SIGNATURE(0, 0x7EED4E09), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimegetns  PFSYSTIMEGETNS_IEC pfsystimegetns;
	#define EXT_systimegetns  extern PFSYSTIMEGETNS_IEC pfsystimegetns;
	#define GET_systimegetns(fl)  s_pfCMGetAPI2( "systimegetns", (RTS_VOID_FCTPTR *)&pfsystimegetns, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x7EED4E09), 0x03050500)
	#define CAL_systimegetns  pfsystimegetns
	#define CHK_systimegetns  (pfsystimegetns != NULL)
	#define EXP_systimegetns   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetns", (RTS_UINTPTR)systimegetns, 1, RTSITF_GET_SIGNATURE(0, 0x7EED4E09), 0x03050500) 
#endif


/**
 * <SIL2/>
 * Returns a monotonic rising microsecond tick. This tick can be used for timeout and
 * high resolution time measurements.
 * NOTE: The realtime clock does not influences this tick!
 * RETURN: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsystimegetus_struct
{
	RTS_IEC_ULINT *pUsTime;				/* VAR_IN_OUT */	/* Time tick result in microseconds */
	RTS_IEC_RESULT SysTimeGetUs;		/* VAR_OUTPUT */	
} systimegetus_struct;

void CDECL CDECL_EXT systimegetus(systimegetus_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMEGETUS_IEC) (systimegetus_struct *p);
#if defined(SYSTIME_NOTIMPLEMENTED) || defined(SYSTIMEGETUS_NOTIMPLEMENTED)
	#define USE_systimegetus
	#define EXT_systimegetus
	#define GET_systimegetus(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimegetus(p0) 
	#define CHK_systimegetus  FALSE
	#define EXP_systimegetus  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimegetus
	#define EXT_systimegetus
	#define GET_systimegetus(fl)  CAL_CMGETAPI( "systimegetus" ) 
	#define CAL_systimegetus  systimegetus
	#define CHK_systimegetus  TRUE
	#define EXP_systimegetus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetus", (RTS_UINTPTR)systimegetus, 1, RTSITF_GET_SIGNATURE(0, 0x6C6308BF), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIME_EXTERNAL)
	#define USE_systimegetus
	#define EXT_systimegetus
	#define GET_systimegetus(fl)  CAL_CMGETAPI( "systimegetus" ) 
	#define CAL_systimegetus  systimegetus
	#define CHK_systimegetus  TRUE
	#define EXP_systimegetus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetus", (RTS_UINTPTR)systimegetus, 1, RTSITF_GET_SIGNATURE(0, 0x6C6308BF), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimesystimegetus
	#define EXT_SysTimesystimegetus
	#define GET_SysTimesystimegetus  ERR_OK
	#define CAL_SysTimesystimegetus  systimegetus
	#define CHK_SysTimesystimegetus  TRUE
	#define EXP_SysTimesystimegetus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetus", (RTS_UINTPTR)systimegetus, 1, RTSITF_GET_SIGNATURE(0, 0x6C6308BF), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimegetus
	#define EXT_systimegetus
	#define GET_systimegetus(fl)  CAL_CMGETAPI( "systimegetus" ) 
	#define CAL_systimegetus  systimegetus
	#define CHK_systimegetus  TRUE
	#define EXP_systimegetus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetus", (RTS_UINTPTR)systimegetus, 1, RTSITF_GET_SIGNATURE(0, 0x6C6308BF), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimegetus  PFSYSTIMEGETUS_IEC pfsystimegetus;
	#define EXT_systimegetus  extern PFSYSTIMEGETUS_IEC pfsystimegetus;
	#define GET_systimegetus(fl)  s_pfCMGetAPI2( "systimegetus", (RTS_VOID_FCTPTR *)&pfsystimegetus, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x6C6308BF), 0x03050500)
	#define CAL_systimegetus  pfsystimegetus
	#define CHK_systimegetus  (pfsystimegetus != NULL)
	#define EXP_systimegetus   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimegetus", (RTS_UINTPTR)systimegetus, 1, RTSITF_GET_SIGNATURE(0, 0x6C6308BF), 0x03050500) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/* Init routines for OS specific modules */
RTS_RESULT CDECL SysTimeOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysTimeOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>
 * <p>Return a monotonic, rising millisecond tick.</p>
 * </description> 
 * <result>Returns the millisecond tick</result>
 */
RTS_UI32 CDECL SysTimeGetMs(void);
typedef RTS_UI32 (CDECL * PFSYSTIMEGETMS) (void);
#if defined(SYSTIME_NOTIMPLEMENTED) || defined(SYSTIMEGETMS_NOTIMPLEMENTED)
	#define USE_SysTimeGetMs
	#define EXT_SysTimeGetMs
	#define GET_SysTimeGetMs(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeGetMs()  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeGetMs  FALSE
	#define EXP_SysTimeGetMs  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeGetMs
	#define EXT_SysTimeGetMs
	#define GET_SysTimeGetMs(fl)  CAL_CMGETAPI( "SysTimeGetMs" ) 
	#define CAL_SysTimeGetMs  SysTimeGetMs
	#define CHK_SysTimeGetMs  TRUE
	#define EXP_SysTimeGetMs  CAL_CMEXPAPI( "SysTimeGetMs" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIME_EXTERNAL)
	#define USE_SysTimeGetMs
	#define EXT_SysTimeGetMs
	#define GET_SysTimeGetMs(fl)  CAL_CMGETAPI( "SysTimeGetMs" ) 
	#define CAL_SysTimeGetMs  SysTimeGetMs
	#define CHK_SysTimeGetMs  TRUE
	#define EXP_SysTimeGetMs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeGetMs", (RTS_UINTPTR)SysTimeGetMs, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeSysTimeGetMs
	#define EXT_SysTimeSysTimeGetMs
	#define GET_SysTimeSysTimeGetMs  ERR_OK
	#define CAL_SysTimeSysTimeGetMs pISysTime->ISysTimeGetMs
	#define CHK_SysTimeSysTimeGetMs (pISysTime != NULL)
	#define EXP_SysTimeSysTimeGetMs  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeGetMs
	#define EXT_SysTimeGetMs
	#define GET_SysTimeGetMs(fl)  CAL_CMGETAPI( "SysTimeGetMs" ) 
	#define CAL_SysTimeGetMs pISysTime->ISysTimeGetMs
	#define CHK_SysTimeGetMs (pISysTime != NULL)
	#define EXP_SysTimeGetMs  CAL_CMEXPAPI( "SysTimeGetMs" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeGetMs  PFSYSTIMEGETMS pfSysTimeGetMs;
	#define EXT_SysTimeGetMs  extern PFSYSTIMEGETMS pfSysTimeGetMs;
	#define GET_SysTimeGetMs(fl)  s_pfCMGetAPI2( "SysTimeGetMs", (RTS_VOID_FCTPTR *)&pfSysTimeGetMs, (fl), 0, 0)
	#define CAL_SysTimeGetMs  pfSysTimeGetMs
	#define CHK_SysTimeGetMs  (pfSysTimeGetMs != NULL)
	#define EXP_SysTimeGetMs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeGetMs", (RTS_UINTPTR)SysTimeGetMs, 0, 0) 
#endif




/**
 * <description>
 * <p>Return a monotonic, rising microsecond tick.</p>
 * </description>
 * <param name="pTime" type="INOUT" range="[NULL,VALID_PTIME]">Pointer to the time tick result</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Time was returned successfully</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pTime was NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Internal Error</errorcode>
 */
RTS_RESULT CDECL SysTimeGetUs(RTS_SYSTIME* pTime);
typedef RTS_RESULT (CDECL * PFSYSTIMEGETUS) (RTS_SYSTIME* pTime);
#if defined(SYSTIME_NOTIMPLEMENTED) || defined(SYSTIMEGETUS_NOTIMPLEMENTED)
	#define USE_SysTimeGetUs
	#define EXT_SysTimeGetUs
	#define GET_SysTimeGetUs(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeGetUs(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeGetUs  FALSE
	#define EXP_SysTimeGetUs  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeGetUs
	#define EXT_SysTimeGetUs
	#define GET_SysTimeGetUs(fl)  CAL_CMGETAPI( "SysTimeGetUs" ) 
	#define CAL_SysTimeGetUs  SysTimeGetUs
	#define CHK_SysTimeGetUs  TRUE
	#define EXP_SysTimeGetUs  CAL_CMEXPAPI( "SysTimeGetUs" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIME_EXTERNAL)
	#define USE_SysTimeGetUs
	#define EXT_SysTimeGetUs
	#define GET_SysTimeGetUs(fl)  CAL_CMGETAPI( "SysTimeGetUs" ) 
	#define CAL_SysTimeGetUs  SysTimeGetUs
	#define CHK_SysTimeGetUs  TRUE
	#define EXP_SysTimeGetUs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeGetUs", (RTS_UINTPTR)SysTimeGetUs, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeSysTimeGetUs
	#define EXT_SysTimeSysTimeGetUs
	#define GET_SysTimeSysTimeGetUs  ERR_OK
	#define CAL_SysTimeSysTimeGetUs pISysTime->ISysTimeGetUs
	#define CHK_SysTimeSysTimeGetUs (pISysTime != NULL)
	#define EXP_SysTimeSysTimeGetUs  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeGetUs
	#define EXT_SysTimeGetUs
	#define GET_SysTimeGetUs(fl)  CAL_CMGETAPI( "SysTimeGetUs" ) 
	#define CAL_SysTimeGetUs pISysTime->ISysTimeGetUs
	#define CHK_SysTimeGetUs (pISysTime != NULL)
	#define EXP_SysTimeGetUs  CAL_CMEXPAPI( "SysTimeGetUs" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeGetUs  PFSYSTIMEGETUS pfSysTimeGetUs;
	#define EXT_SysTimeGetUs  extern PFSYSTIMEGETUS pfSysTimeGetUs;
	#define GET_SysTimeGetUs(fl)  s_pfCMGetAPI2( "SysTimeGetUs", (RTS_VOID_FCTPTR *)&pfSysTimeGetUs, (fl), 0, 0)
	#define CAL_SysTimeGetUs  pfSysTimeGetUs
	#define CHK_SysTimeGetUs  (pfSysTimeGetUs != NULL)
	#define EXP_SysTimeGetUs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeGetUs", (RTS_UINTPTR)SysTimeGetUs, 0, 0) 
#endif




/**
 * <description>
 * <p>Return a monotonic, rising nanosecond tick.</p>
 * </description>
 * <param name="pTime" type="INOUT" range="[NULL,VALID_PTIME]">Pointer to the time tick result</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Time was returned successfully</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pTime was NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Internal Error</errorcode>
 */
RTS_RESULT CDECL SysTimeGetNs(RTS_SYSTIME* pTime);
typedef RTS_RESULT (CDECL * PFSYSTIMEGETNS) (RTS_SYSTIME* pTime);
#if defined(SYSTIME_NOTIMPLEMENTED) || defined(SYSTIMEGETNS_NOTIMPLEMENTED)
	#define USE_SysTimeGetNs
	#define EXT_SysTimeGetNs
	#define GET_SysTimeGetNs(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeGetNs(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeGetNs  FALSE
	#define EXP_SysTimeGetNs  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeGetNs
	#define EXT_SysTimeGetNs
	#define GET_SysTimeGetNs(fl)  CAL_CMGETAPI( "SysTimeGetNs" ) 
	#define CAL_SysTimeGetNs  SysTimeGetNs
	#define CHK_SysTimeGetNs  TRUE
	#define EXP_SysTimeGetNs  CAL_CMEXPAPI( "SysTimeGetNs" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIME_EXTERNAL)
	#define USE_SysTimeGetNs
	#define EXT_SysTimeGetNs
	#define GET_SysTimeGetNs(fl)  CAL_CMGETAPI( "SysTimeGetNs" ) 
	#define CAL_SysTimeGetNs  SysTimeGetNs
	#define CHK_SysTimeGetNs  TRUE
	#define EXP_SysTimeGetNs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeGetNs", (RTS_UINTPTR)SysTimeGetNs, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeSysTimeGetNs
	#define EXT_SysTimeSysTimeGetNs
	#define GET_SysTimeSysTimeGetNs  ERR_OK
	#define CAL_SysTimeSysTimeGetNs pISysTime->ISysTimeGetNs
	#define CHK_SysTimeSysTimeGetNs (pISysTime != NULL)
	#define EXP_SysTimeSysTimeGetNs  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeGetNs
	#define EXT_SysTimeGetNs
	#define GET_SysTimeGetNs(fl)  CAL_CMGETAPI( "SysTimeGetNs" ) 
	#define CAL_SysTimeGetNs pISysTime->ISysTimeGetNs
	#define CHK_SysTimeGetNs (pISysTime != NULL)
	#define EXP_SysTimeGetNs  CAL_CMEXPAPI( "SysTimeGetNs" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeGetNs  PFSYSTIMEGETNS pfSysTimeGetNs;
	#define EXT_SysTimeGetNs  extern PFSYSTIMEGETNS pfSysTimeGetNs;
	#define GET_SysTimeGetNs(fl)  s_pfCMGetAPI2( "SysTimeGetNs", (RTS_VOID_FCTPTR *)&pfSysTimeGetNs, (fl), 0, 0)
	#define CAL_SysTimeGetNs  pfSysTimeGetNs
	#define CHK_SysTimeGetNs  (pfSysTimeGetNs != NULL)
	#define EXP_SysTimeGetNs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeGetNs", (RTS_UINTPTR)SysTimeGetNs, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSTIMEGETMS ISysTimeGetMs;
 	PFSYSTIMEGETUS ISysTimeGetUs;
 	PFSYSTIMEGETNS ISysTimeGetNs;
 } ISysTime_C;

#ifdef CPLUSPLUS
class ISysTime : public IBase
{
	public:
		virtual RTS_UI32 CDECL ISysTimeGetMs(void) =0;
		virtual RTS_RESULT CDECL ISysTimeGetUs(RTS_SYSTIME* pTime) =0;
		virtual RTS_RESULT CDECL ISysTimeGetNs(RTS_SYSTIME* pTime) =0;
};
	#ifndef ITF_SysTime
		#define ITF_SysTime static ISysTime *pISysTime = NULL;
	#endif
	#define EXTITF_SysTime
#else	/*CPLUSPLUS*/
	typedef ISysTime_C		ISysTime;
	#ifndef ITF_SysTime
		#define ITF_SysTime
	#endif
	#define EXTITF_SysTime
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSTIMEITF_H_*/
