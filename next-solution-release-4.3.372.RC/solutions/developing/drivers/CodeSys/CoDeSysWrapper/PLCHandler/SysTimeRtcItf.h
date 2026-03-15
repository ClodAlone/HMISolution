 /**
 * <interfacename>SysTimeRtc</interfacename>
 * <description> 
 *	The SysTimeRtc interface is projected to get access to an optional realtime clock
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _SYSTIMERTCITF_H_
#define _SYSTIMERTCITF_H_

#include "CmpStd.h"

 

 




#include "CmpStd.h"

#ifndef CDECL
	#define CDECL
#endif

#include "SysTimeItf.h"


/**
 * <category>Control status HourMode</category>
 * <description></description>
 * <element name="RTC_12HOURS" type="IN">12 hours mode</element>
 * <element name="RTC_24HOURS" type="IN">24 hoursmode</element>
 */
#define RTC_12HOURS		0
#define RTC_24HOURS		1

/**
 * <category>Control status CheckBattery</category>
 * <description></description>
 * <element name="RTC_BATTERY_FAILED" type="IN">Battery empty or failed</element>
 * <element name="RTC_BATTERY_OK" type="IN">Battery Ok</element>
 */
#define RTC_BATTERY_FAILED		0
#define RTC_BATTERY_OK			1

/**
 * <category>Control tags</category>
 * <description></description>
 * <element name="RTC_CTRL_CHECKBATTERY" type="IN">Check the battery of the RTC</element>
 * <element name="RTC_CTRL_HOURMODE" type="IN">Check the hour mode of the RTC</element>
 */
#define RTC_CTRL_CHECKBATTERY	0
#define RTC_CTRL_HOURMODE		1


/** EXTERN LIB SECTION BEGIN **/

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Time and date in structured format</description>
 */
typedef struct tagRTS_SYSTIMEDATE
{
	RTS_IEC_UINT wYear;			/* <element="wYear" type="IN">Year (e.g. 2006)</element> */
	RTS_IEC_UINT wMonth;		/* <element="wMonth" type="IN">Month (1..12: January = 1, December = 12)</element> */
	RTS_IEC_UINT wDay;			/* <element="wDay" type="IN">Day of month (1..31)</element> */
	RTS_IEC_UINT wHour;			/* <element="wHour" type="IN">Hours after midnight (0..23)</element> */
	RTS_IEC_UINT wMinute;		/* <element="wMinute" type="IN">Minutes after hour (0..59)</element> */
	RTS_IEC_UINT wSecond;		/* <element="wSecond" type="IN">Seconds after minute (0..59)</element> */
	RTS_IEC_UINT wMilliseconds;	/* <element="wMilliseconds" type="IN">Milliseconds after second (0..999). Optional!</element> */
	RTS_IEC_UINT wDayOfWeek;	/* <element="wDayOfWeek" type="IN">Day of week (1..7: Monday = 1, Sunday = 7</element> */
	RTS_IEC_UINT wYday;			/* <element="wYday" type="IN">Day of year (1..366): January 1 = 1, December 31 = 365 / leap year=366</element> */
} RTS_SYSTIMEDATE;

/**
 * <category>Timezone information</category>
 * <description>
 *     This infomation describes a local timezone with standard- and daylight-saving-time
 *     (also known as summer- and wintertime).
 * </description>
 */
typedef struct tagTimezoneInformation
{
	RTS_IEC_UDINT ulStandardDate;		/* <element name="ulStandardDate" type="IN">Date to switch to standard time (from summer- to wintertime) [local time in seconds since 1.1.1970 00:00:00]</element> */
	RTS_IEC_UDINT ulDaylightDate;		/* <element name="ulDaylightDate" type="IN">Date to switch to daylight savings time (from winter- to summertime) [local time in seconds since 1.1.1970 00:00:00]</element> */
	RTS_IEC_STRING szStandardName[33];		/* <element name="szStandardName" type="IN">Standard name of the timezone (wintertime) [string with max. 32bytes]</element> */
	RTS_IEC_STRING szDaylightName[33];		/* <element name="szDaylightName" type="IN">Daylight savings time name (summertime) [string with max. 32bytes]</element> */
	RTS_IEC_INT iBias;		/* <element name="iBias" type="IN">UTC = local time + bias [minutes]</element> */
	RTS_IEC_INT iStandardBias;		/* <element name="iStandardBias" type="IN">Additional offset at standard time (wintertime) [minutes]</element> */
	RTS_IEC_INT iDaylightBias;		/* <element name="iDaylightBias" type="IN">Additional offset at daylight savings time (summertime) [minutes]</element> */
} TimezoneInformation;

/**
 * <description>Control the Rtc and read out hardware status information</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertccontrol_struct
{
	RTS_IEC_DINT iControlTag;			/* VAR_INPUT */	/* <param name="iControl" type="IN">Control key. See corresponding category for detailed information</param> */
	RTS_IEC_DINT *pdiControlResult;		/* VAR_IN_OUT */	/* <param name="pdiControlResult" type="OUT">Control state. See category Control status for detailed information</param> */
	RTS_IEC_RESULT SysTimeRtcControl;	/* VAR_OUTPUT */	
} systimertccontrol_struct;

void CDECL CDECL_EXT systimertccontrol(systimertccontrol_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCCONTROL_IEC) (systimertccontrol_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONTROL_NOTIMPLEMENTED)
	#define USE_systimertccontrol
	#define EXT_systimertccontrol
	#define GET_systimertccontrol(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertccontrol(p0) 
	#define CHK_systimertccontrol  FALSE
	#define EXP_systimertccontrol  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertccontrol
	#define EXT_systimertccontrol
	#define GET_systimertccontrol(fl)  CAL_CMGETAPI( "systimertccontrol" ) 
	#define CAL_systimertccontrol  systimertccontrol
	#define CHK_systimertccontrol  TRUE
	#define EXP_systimertccontrol  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertccontrol", (RTS_UINTPTR)systimertccontrol, 1, RTSITF_GET_SIGNATURE(0, 0x6C6D5178), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertccontrol
	#define EXT_systimertccontrol
	#define GET_systimertccontrol(fl)  CAL_CMGETAPI( "systimertccontrol" ) 
	#define CAL_systimertccontrol  systimertccontrol
	#define CHK_systimertccontrol  TRUE
	#define EXP_systimertccontrol  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertccontrol", (RTS_UINTPTR)systimertccontrol, 1, RTSITF_GET_SIGNATURE(0, 0x6C6D5178), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertccontrol
	#define EXT_SysTimeRtcsystimertccontrol
	#define GET_SysTimeRtcsystimertccontrol  ERR_OK
	#define CAL_SysTimeRtcsystimertccontrol  systimertccontrol
	#define CHK_SysTimeRtcsystimertccontrol  TRUE
	#define EXP_SysTimeRtcsystimertccontrol  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertccontrol", (RTS_UINTPTR)systimertccontrol, 1, RTSITF_GET_SIGNATURE(0, 0x6C6D5178), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertccontrol
	#define EXT_systimertccontrol
	#define GET_systimertccontrol(fl)  CAL_CMGETAPI( "systimertccontrol" ) 
	#define CAL_systimertccontrol  systimertccontrol
	#define CHK_systimertccontrol  TRUE
	#define EXP_systimertccontrol  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertccontrol", (RTS_UINTPTR)systimertccontrol, 1, RTSITF_GET_SIGNATURE(0, 0x6C6D5178), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertccontrol  PFSYSTIMERTCCONTROL_IEC pfsystimertccontrol;
	#define EXT_systimertccontrol  extern PFSYSTIMERTCCONTROL_IEC pfsystimertccontrol;
	#define GET_systimertccontrol(fl)  s_pfCMGetAPI2( "systimertccontrol", (RTS_VOID_FCTPTR *)&pfsystimertccontrol, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x6C6D5178), 0x03050500)
	#define CAL_systimertccontrol  pfsystimertccontrol
	#define CHK_systimertccontrol  (pfsystimertccontrol != NULL)
	#define EXP_systimertccontrol   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertccontrol", (RTS_UINTPTR)systimertccontrol, 1, RTSITF_GET_SIGNATURE(0, 0x6C6D5178), 0x03050500) 
#endif


/**
 * <description>
 * This function converts the time given by time structure into a High Resolution Time of format SysTime.
 *	Time values are always UTC!</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertcconvertdatetohighres_struct
{
	RTS_SYSTIMEDATE *pDate;				/* VAR_IN_OUT */	/* <param name="pDate" type="IN">Structure format</param> */
	RTS_IEC_ULINT *pTimestamp;			/* VAR_IN_OUT */	/* <param name="pTimestamp" type="OUT">Milliseconds since 1.1.1970 00:00:000 UTC</param> */
	RTS_IEC_RESULT SysTimeRtcConvertDateToHighRes;	/* VAR_OUTPUT */	
} systimertcconvertdatetohighres_struct;

void CDECL CDECL_EXT systimertcconvertdatetohighres(systimertcconvertdatetohighres_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCCONVERTDATETOHIGHRES_IEC) (systimertcconvertdatetohighres_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTDATETOHIGHRES_NOTIMPLEMENTED)
	#define USE_systimertcconvertdatetohighres
	#define EXT_systimertcconvertdatetohighres
	#define GET_systimertcconvertdatetohighres(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcconvertdatetohighres(p0) 
	#define CHK_systimertcconvertdatetohighres  FALSE
	#define EXP_systimertcconvertdatetohighres  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcconvertdatetohighres
	#define EXT_systimertcconvertdatetohighres
	#define GET_systimertcconvertdatetohighres(fl)  CAL_CMGETAPI( "systimertcconvertdatetohighres" ) 
	#define CAL_systimertcconvertdatetohighres  systimertcconvertdatetohighres
	#define CHK_systimertcconvertdatetohighres  TRUE
	#define EXP_systimertcconvertdatetohighres  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetohighres", (RTS_UINTPTR)systimertcconvertdatetohighres, 1, RTSITF_GET_SIGNATURE(0, 0x615A4496), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcconvertdatetohighres
	#define EXT_systimertcconvertdatetohighres
	#define GET_systimertcconvertdatetohighres(fl)  CAL_CMGETAPI( "systimertcconvertdatetohighres" ) 
	#define CAL_systimertcconvertdatetohighres  systimertcconvertdatetohighres
	#define CHK_systimertcconvertdatetohighres  TRUE
	#define EXP_systimertcconvertdatetohighres  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetohighres", (RTS_UINTPTR)systimertcconvertdatetohighres, 1, RTSITF_GET_SIGNATURE(0, 0x615A4496), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcconvertdatetohighres
	#define EXT_SysTimeRtcsystimertcconvertdatetohighres
	#define GET_SysTimeRtcsystimertcconvertdatetohighres  ERR_OK
	#define CAL_SysTimeRtcsystimertcconvertdatetohighres  systimertcconvertdatetohighres
	#define CHK_SysTimeRtcsystimertcconvertdatetohighres  TRUE
	#define EXP_SysTimeRtcsystimertcconvertdatetohighres  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetohighres", (RTS_UINTPTR)systimertcconvertdatetohighres, 1, RTSITF_GET_SIGNATURE(0, 0x615A4496), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcconvertdatetohighres
	#define EXT_systimertcconvertdatetohighres
	#define GET_systimertcconvertdatetohighres(fl)  CAL_CMGETAPI( "systimertcconvertdatetohighres" ) 
	#define CAL_systimertcconvertdatetohighres  systimertcconvertdatetohighres
	#define CHK_systimertcconvertdatetohighres  TRUE
	#define EXP_systimertcconvertdatetohighres  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetohighres", (RTS_UINTPTR)systimertcconvertdatetohighres, 1, RTSITF_GET_SIGNATURE(0, 0x615A4496), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcconvertdatetohighres  PFSYSTIMERTCCONVERTDATETOHIGHRES_IEC pfsystimertcconvertdatetohighres;
	#define EXT_systimertcconvertdatetohighres  extern PFSYSTIMERTCCONVERTDATETOHIGHRES_IEC pfsystimertcconvertdatetohighres;
	#define GET_systimertcconvertdatetohighres(fl)  s_pfCMGetAPI2( "systimertcconvertdatetohighres", (RTS_VOID_FCTPTR *)&pfsystimertcconvertdatetohighres, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x615A4496), 0x03050500)
	#define CAL_systimertcconvertdatetohighres  pfsystimertcconvertdatetohighres
	#define CHK_systimertcconvertdatetohighres  (pfsystimertcconvertdatetohighres != NULL)
	#define EXP_systimertcconvertdatetohighres   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetohighres", (RTS_UINTPTR)systimertcconvertdatetohighres, 1, RTSITF_GET_SIGNATURE(0, 0x615A4496), 0x03050500) 
#endif


/**
 * <description>Convert UTC in a structured format to timestamp in seconds</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertcconvertdatetoutc_struct
{
	RTS_SYSTIMEDATE *pDate;				/* VAR_IN_OUT */	/* <param name="pDate" type="IN">Pointer to structure format. See category for detailed information</param> */
	RTS_IEC_DWORD *pdwTimestampUtc;		/* VAR_IN_OUT */	/* <param name="pdwTimestampUtc" type="OUT">Pointer to UTC time in seconds since 1.1.1970 00:00:00</param> */
	RTS_IEC_RESULT SysTimeRtcConvertDateToUtc;	/* VAR_OUTPUT */	
} systimertcconvertdatetoutc_struct;

void CDECL CDECL_EXT systimertcconvertdatetoutc(systimertcconvertdatetoutc_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCCONVERTDATETOUTC_IEC) (systimertcconvertdatetoutc_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTDATETOUTC_NOTIMPLEMENTED)
	#define USE_systimertcconvertdatetoutc
	#define EXT_systimertcconvertdatetoutc
	#define GET_systimertcconvertdatetoutc(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcconvertdatetoutc(p0) 
	#define CHK_systimertcconvertdatetoutc  FALSE
	#define EXP_systimertcconvertdatetoutc  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcconvertdatetoutc
	#define EXT_systimertcconvertdatetoutc
	#define GET_systimertcconvertdatetoutc(fl)  CAL_CMGETAPI( "systimertcconvertdatetoutc" ) 
	#define CAL_systimertcconvertdatetoutc  systimertcconvertdatetoutc
	#define CHK_systimertcconvertdatetoutc  TRUE
	#define EXP_systimertcconvertdatetoutc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetoutc", (RTS_UINTPTR)systimertcconvertdatetoutc, 1, RTSITF_GET_SIGNATURE(0, 0x808E752B), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcconvertdatetoutc
	#define EXT_systimertcconvertdatetoutc
	#define GET_systimertcconvertdatetoutc(fl)  CAL_CMGETAPI( "systimertcconvertdatetoutc" ) 
	#define CAL_systimertcconvertdatetoutc  systimertcconvertdatetoutc
	#define CHK_systimertcconvertdatetoutc  TRUE
	#define EXP_systimertcconvertdatetoutc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetoutc", (RTS_UINTPTR)systimertcconvertdatetoutc, 1, RTSITF_GET_SIGNATURE(0, 0x808E752B), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcconvertdatetoutc
	#define EXT_SysTimeRtcsystimertcconvertdatetoutc
	#define GET_SysTimeRtcsystimertcconvertdatetoutc  ERR_OK
	#define CAL_SysTimeRtcsystimertcconvertdatetoutc  systimertcconvertdatetoutc
	#define CHK_SysTimeRtcsystimertcconvertdatetoutc  TRUE
	#define EXP_SysTimeRtcsystimertcconvertdatetoutc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetoutc", (RTS_UINTPTR)systimertcconvertdatetoutc, 1, RTSITF_GET_SIGNATURE(0, 0x808E752B), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcconvertdatetoutc
	#define EXT_systimertcconvertdatetoutc
	#define GET_systimertcconvertdatetoutc(fl)  CAL_CMGETAPI( "systimertcconvertdatetoutc" ) 
	#define CAL_systimertcconvertdatetoutc  systimertcconvertdatetoutc
	#define CHK_systimertcconvertdatetoutc  TRUE
	#define EXP_systimertcconvertdatetoutc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetoutc", (RTS_UINTPTR)systimertcconvertdatetoutc, 1, RTSITF_GET_SIGNATURE(0, 0x808E752B), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcconvertdatetoutc  PFSYSTIMERTCCONVERTDATETOUTC_IEC pfsystimertcconvertdatetoutc;
	#define EXT_systimertcconvertdatetoutc  extern PFSYSTIMERTCCONVERTDATETOUTC_IEC pfsystimertcconvertdatetoutc;
	#define GET_systimertcconvertdatetoutc(fl)  s_pfCMGetAPI2( "systimertcconvertdatetoutc", (RTS_VOID_FCTPTR *)&pfsystimertcconvertdatetoutc, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x808E752B), 0x03050500)
	#define CAL_systimertcconvertdatetoutc  pfsystimertcconvertdatetoutc
	#define CHK_systimertcconvertdatetoutc  (pfsystimertcconvertdatetoutc != NULL)
	#define EXP_systimertcconvertdatetoutc   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertdatetoutc", (RTS_UINTPTR)systimertcconvertdatetoutc, 1, RTSITF_GET_SIGNATURE(0, 0x808E752B), 0x03050500) 
#endif


/**
 * <description>Convert a high resolution timestamp to a structure format.
 *	Time values are always UTC!</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertcconverthighrestodate_struct
{
	RTS_IEC_ULINT *pTimestamp;			/* VAR_IN_OUT */	/* <param name="pTimestamp" type="IN">Milliseconds since 1.1.1970 00:00:000 UTC</param> */
	RTS_SYSTIMEDATE *pDate;				/* VAR_IN_OUT */	/* <param name="pDate" type="OUT">Structure format</param> */
	RTS_IEC_RESULT SysTimeRtcConvertHighResToDate;	/* VAR_OUTPUT */	
} systimertcconverthighrestodate_struct;

void CDECL CDECL_EXT systimertcconverthighrestodate(systimertcconverthighrestodate_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCCONVERTHIGHRESTODATE_IEC) (systimertcconverthighrestodate_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTHIGHRESTODATE_NOTIMPLEMENTED)
	#define USE_systimertcconverthighrestodate
	#define EXT_systimertcconverthighrestodate
	#define GET_systimertcconverthighrestodate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcconverthighrestodate(p0) 
	#define CHK_systimertcconverthighrestodate  FALSE
	#define EXP_systimertcconverthighrestodate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcconverthighrestodate
	#define EXT_systimertcconverthighrestodate
	#define GET_systimertcconverthighrestodate(fl)  CAL_CMGETAPI( "systimertcconverthighrestodate" ) 
	#define CAL_systimertcconverthighrestodate  systimertcconverthighrestodate
	#define CHK_systimertcconverthighrestodate  TRUE
	#define EXP_systimertcconverthighrestodate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestodate", (RTS_UINTPTR)systimertcconverthighrestodate, 1, RTSITF_GET_SIGNATURE(0, 0x7343C11D), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcconverthighrestodate
	#define EXT_systimertcconverthighrestodate
	#define GET_systimertcconverthighrestodate(fl)  CAL_CMGETAPI( "systimertcconverthighrestodate" ) 
	#define CAL_systimertcconverthighrestodate  systimertcconverthighrestodate
	#define CHK_systimertcconverthighrestodate  TRUE
	#define EXP_systimertcconverthighrestodate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestodate", (RTS_UINTPTR)systimertcconverthighrestodate, 1, RTSITF_GET_SIGNATURE(0, 0x7343C11D), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcconverthighrestodate
	#define EXT_SysTimeRtcsystimertcconverthighrestodate
	#define GET_SysTimeRtcsystimertcconverthighrestodate  ERR_OK
	#define CAL_SysTimeRtcsystimertcconverthighrestodate  systimertcconverthighrestodate
	#define CHK_SysTimeRtcsystimertcconverthighrestodate  TRUE
	#define EXP_SysTimeRtcsystimertcconverthighrestodate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestodate", (RTS_UINTPTR)systimertcconverthighrestodate, 1, RTSITF_GET_SIGNATURE(0, 0x7343C11D), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcconverthighrestodate
	#define EXT_systimertcconverthighrestodate
	#define GET_systimertcconverthighrestodate(fl)  CAL_CMGETAPI( "systimertcconverthighrestodate" ) 
	#define CAL_systimertcconverthighrestodate  systimertcconverthighrestodate
	#define CHK_systimertcconverthighrestodate  TRUE
	#define EXP_systimertcconverthighrestodate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestodate", (RTS_UINTPTR)systimertcconverthighrestodate, 1, RTSITF_GET_SIGNATURE(0, 0x7343C11D), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcconverthighrestodate  PFSYSTIMERTCCONVERTHIGHRESTODATE_IEC pfsystimertcconverthighrestodate;
	#define EXT_systimertcconverthighrestodate  extern PFSYSTIMERTCCONVERTHIGHRESTODATE_IEC pfsystimertcconverthighrestodate;
	#define GET_systimertcconverthighrestodate(fl)  s_pfCMGetAPI2( "systimertcconverthighrestodate", (RTS_VOID_FCTPTR *)&pfsystimertcconverthighrestodate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x7343C11D), 0x03050500)
	#define CAL_systimertcconverthighrestodate  pfsystimertcconverthighrestodate
	#define CHK_systimertcconverthighrestodate  (pfsystimertcconverthighrestodate != NULL)
	#define EXP_systimertcconverthighrestodate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestodate", (RTS_UINTPTR)systimertcconverthighrestodate, 1, RTSITF_GET_SIGNATURE(0, 0x7343C11D), 0x03050500) 
#endif


/**
 * <description>
 * Convert a high resolution timestamp [UTC] to a structure format [Localtime]
 * </description>
 * <errorcode name="RTS_RESULT Result" TYPE="ERR_OK">Timestamp can be converted</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Timestamp failed to convert</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Conversion is not available on the target</errorcode>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertcconverthighrestolocal_struct
{
	RTS_IEC_ULINT *pTimestamp;			/* VAR_IN_OUT */	/* <param name="pTimestamp" type="IN">Milliseconds since 1.1.1970 00:00:000 [UTC]</param> */
	RTS_SYSTIMEDATE *pDate;				/* VAR_IN_OUT */	/* <param name="pDate" type="OUT">Structure format [Localtime]</param> */
	RTS_IEC_RESULT SysTimeRtcConvertHighResToLocal;	/* VAR_OUTPUT */	
} systimertcconverthighrestolocal_struct;

void CDECL CDECL_EXT systimertcconverthighrestolocal(systimertcconverthighrestolocal_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCCONVERTHIGHRESTOLOCAL_IEC) (systimertcconverthighrestolocal_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTHIGHRESTOLOCAL_NOTIMPLEMENTED)
	#define USE_systimertcconverthighrestolocal
	#define EXT_systimertcconverthighrestolocal
	#define GET_systimertcconverthighrestolocal(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcconverthighrestolocal(p0) 
	#define CHK_systimertcconverthighrestolocal  FALSE
	#define EXP_systimertcconverthighrestolocal  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcconverthighrestolocal
	#define EXT_systimertcconverthighrestolocal
	#define GET_systimertcconverthighrestolocal(fl)  CAL_CMGETAPI( "systimertcconverthighrestolocal" ) 
	#define CAL_systimertcconverthighrestolocal  systimertcconverthighrestolocal
	#define CHK_systimertcconverthighrestolocal  TRUE
	#define EXP_systimertcconverthighrestolocal  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestolocal", (RTS_UINTPTR)systimertcconverthighrestolocal, 1, 0x326AB8DF, 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcconverthighrestolocal
	#define EXT_systimertcconverthighrestolocal
	#define GET_systimertcconverthighrestolocal(fl)  CAL_CMGETAPI( "systimertcconverthighrestolocal" ) 
	#define CAL_systimertcconverthighrestolocal  systimertcconverthighrestolocal
	#define CHK_systimertcconverthighrestolocal  TRUE
	#define EXP_systimertcconverthighrestolocal  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestolocal", (RTS_UINTPTR)systimertcconverthighrestolocal, 1, 0x326AB8DF, 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcconverthighrestolocal
	#define EXT_SysTimeRtcsystimertcconverthighrestolocal
	#define GET_SysTimeRtcsystimertcconverthighrestolocal  ERR_OK
	#define CAL_SysTimeRtcsystimertcconverthighrestolocal  systimertcconverthighrestolocal
	#define CHK_SysTimeRtcsystimertcconverthighrestolocal  TRUE
	#define EXP_SysTimeRtcsystimertcconverthighrestolocal  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestolocal", (RTS_UINTPTR)systimertcconverthighrestolocal, 1, 0x326AB8DF, 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcconverthighrestolocal
	#define EXT_systimertcconverthighrestolocal
	#define GET_systimertcconverthighrestolocal(fl)  CAL_CMGETAPI( "systimertcconverthighrestolocal" ) 
	#define CAL_systimertcconverthighrestolocal  systimertcconverthighrestolocal
	#define CHK_systimertcconverthighrestolocal  TRUE
	#define EXP_systimertcconverthighrestolocal  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestolocal", (RTS_UINTPTR)systimertcconverthighrestolocal, 1, 0x326AB8DF, 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcconverthighrestolocal  PFSYSTIMERTCCONVERTHIGHRESTOLOCAL_IEC pfsystimertcconverthighrestolocal;
	#define EXT_systimertcconverthighrestolocal  extern PFSYSTIMERTCCONVERTHIGHRESTOLOCAL_IEC pfsystimertcconverthighrestolocal;
	#define GET_systimertcconverthighrestolocal(fl)  s_pfCMGetAPI2( "systimertcconverthighrestolocal", (RTS_VOID_FCTPTR *)&pfsystimertcconverthighrestolocal, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x326AB8DF, 0x03050500)
	#define CAL_systimertcconverthighrestolocal  pfsystimertcconverthighrestolocal
	#define CHK_systimertcconverthighrestolocal  (pfsystimertcconverthighrestolocal != NULL)
	#define EXP_systimertcconverthighrestolocal   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconverthighrestolocal", (RTS_UINTPTR)systimertcconverthighrestolocal, 1, 0x326AB8DF, 0x03050500) 
#endif


/**
 * <description>
 * This function converts the time given by time structure [Localtime] into a High Resolution Time of format SysTime [UTC].
 *	</description>
 * <errorcode name="RTS_RESULT Result" TYPE="ERR_OK">DATE can be converted</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Date failed to convert</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Conversion is not available on the target</errorcode>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertcconvertlocaltohighres_struct
{
	RTS_SYSTIMEDATE *pDate;				/* VAR_IN_OUT */	/* <param name="pDate" type="IN">Structure format [Localtime]</param> */
	RTS_IEC_ULINT *pTimestamp;			/* VAR_IN_OUT */	/* <param name="pTimestamp" type="OUT">Milliseconds since 1.1.1970 00:00:000 [UTC]</param> */
	RTS_IEC_RESULT SysTimeRtcConvertLocalToHighRes;	/* VAR_OUTPUT */	
} systimertcconvertlocaltohighres_struct;

void CDECL CDECL_EXT systimertcconvertlocaltohighres(systimertcconvertlocaltohighres_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCCONVERTLOCALTOHIGHRES_IEC) (systimertcconvertlocaltohighres_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTLOCALTOHIGHRES_NOTIMPLEMENTED)
	#define USE_systimertcconvertlocaltohighres
	#define EXT_systimertcconvertlocaltohighres
	#define GET_systimertcconvertlocaltohighres(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcconvertlocaltohighres(p0) 
	#define CHK_systimertcconvertlocaltohighres  FALSE
	#define EXP_systimertcconvertlocaltohighres  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcconvertlocaltohighres
	#define EXT_systimertcconvertlocaltohighres
	#define GET_systimertcconvertlocaltohighres(fl)  CAL_CMGETAPI( "systimertcconvertlocaltohighres" ) 
	#define CAL_systimertcconvertlocaltohighres  systimertcconvertlocaltohighres
	#define CHK_systimertcconvertlocaltohighres  TRUE
	#define EXP_systimertcconvertlocaltohighres  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltohighres", (RTS_UINTPTR)systimertcconvertlocaltohighres, 1, 0x82603563, 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcconvertlocaltohighres
	#define EXT_systimertcconvertlocaltohighres
	#define GET_systimertcconvertlocaltohighres(fl)  CAL_CMGETAPI( "systimertcconvertlocaltohighres" ) 
	#define CAL_systimertcconvertlocaltohighres  systimertcconvertlocaltohighres
	#define CHK_systimertcconvertlocaltohighres  TRUE
	#define EXP_systimertcconvertlocaltohighres  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltohighres", (RTS_UINTPTR)systimertcconvertlocaltohighres, 1, 0x82603563, 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcconvertlocaltohighres
	#define EXT_SysTimeRtcsystimertcconvertlocaltohighres
	#define GET_SysTimeRtcsystimertcconvertlocaltohighres  ERR_OK
	#define CAL_SysTimeRtcsystimertcconvertlocaltohighres  systimertcconvertlocaltohighres
	#define CHK_SysTimeRtcsystimertcconvertlocaltohighres  TRUE
	#define EXP_SysTimeRtcsystimertcconvertlocaltohighres  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltohighres", (RTS_UINTPTR)systimertcconvertlocaltohighres, 1, 0x82603563, 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcconvertlocaltohighres
	#define EXT_systimertcconvertlocaltohighres
	#define GET_systimertcconvertlocaltohighres(fl)  CAL_CMGETAPI( "systimertcconvertlocaltohighres" ) 
	#define CAL_systimertcconvertlocaltohighres  systimertcconvertlocaltohighres
	#define CHK_systimertcconvertlocaltohighres  TRUE
	#define EXP_systimertcconvertlocaltohighres  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltohighres", (RTS_UINTPTR)systimertcconvertlocaltohighres, 1, 0x82603563, 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcconvertlocaltohighres  PFSYSTIMERTCCONVERTLOCALTOHIGHRES_IEC pfsystimertcconvertlocaltohighres;
	#define EXT_systimertcconvertlocaltohighres  extern PFSYSTIMERTCCONVERTLOCALTOHIGHRES_IEC pfsystimertcconvertlocaltohighres;
	#define GET_systimertcconvertlocaltohighres(fl)  s_pfCMGetAPI2( "systimertcconvertlocaltohighres", (RTS_VOID_FCTPTR *)&pfsystimertcconvertlocaltohighres, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x82603563, 0x03050500)
	#define CAL_systimertcconvertlocaltohighres  pfsystimertcconvertlocaltohighres
	#define CHK_systimertcconvertlocaltohighres  (pfsystimertcconvertlocaltohighres != NULL)
	#define EXP_systimertcconvertlocaltohighres   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltohighres", (RTS_UINTPTR)systimertcconvertlocaltohighres, 1, 0x82603563, 0x03050500) 
#endif


/**
 * <description>*OBSOLETE*: Only for backward compatibility.</description>
 */
typedef struct tagsystimertcconvertlocaltoutc_struct
{
	RTS_IEC_DWORD dwTimestampLocal;		/* VAR_INPUT */	
	RTS_IEC_DWORD *pdwTimestampUtc;		/* VAR_IN_OUT */	
	RTS_IEC_RESULT SysTimeRtcConvertLocalToUtc;	/* VAR_OUTPUT */	
} systimertcconvertlocaltoutc_struct;

void CDECL CDECL_EXT systimertcconvertlocaltoutc(systimertcconvertlocaltoutc_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCCONVERTLOCALTOUTC_IEC) (systimertcconvertlocaltoutc_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTLOCALTOUTC_NOTIMPLEMENTED)
	#define USE_systimertcconvertlocaltoutc
	#define EXT_systimertcconvertlocaltoutc
	#define GET_systimertcconvertlocaltoutc(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcconvertlocaltoutc(p0) 
	#define CHK_systimertcconvertlocaltoutc  FALSE
	#define EXP_systimertcconvertlocaltoutc  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcconvertlocaltoutc
	#define EXT_systimertcconvertlocaltoutc
	#define GET_systimertcconvertlocaltoutc(fl)  CAL_CMGETAPI( "systimertcconvertlocaltoutc" ) 
	#define CAL_systimertcconvertlocaltoutc  systimertcconvertlocaltoutc
	#define CHK_systimertcconvertlocaltoutc  TRUE
	#define EXP_systimertcconvertlocaltoutc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltoutc", (RTS_UINTPTR)systimertcconvertlocaltoutc, 1, RTSITF_GET_SIGNATURE(0, 0x7D11A77B), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcconvertlocaltoutc
	#define EXT_systimertcconvertlocaltoutc
	#define GET_systimertcconvertlocaltoutc(fl)  CAL_CMGETAPI( "systimertcconvertlocaltoutc" ) 
	#define CAL_systimertcconvertlocaltoutc  systimertcconvertlocaltoutc
	#define CHK_systimertcconvertlocaltoutc  TRUE
	#define EXP_systimertcconvertlocaltoutc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltoutc", (RTS_UINTPTR)systimertcconvertlocaltoutc, 1, RTSITF_GET_SIGNATURE(0, 0x7D11A77B), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcconvertlocaltoutc
	#define EXT_SysTimeRtcsystimertcconvertlocaltoutc
	#define GET_SysTimeRtcsystimertcconvertlocaltoutc  ERR_OK
	#define CAL_SysTimeRtcsystimertcconvertlocaltoutc  systimertcconvertlocaltoutc
	#define CHK_SysTimeRtcsystimertcconvertlocaltoutc  TRUE
	#define EXP_SysTimeRtcsystimertcconvertlocaltoutc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltoutc", (RTS_UINTPTR)systimertcconvertlocaltoutc, 1, RTSITF_GET_SIGNATURE(0, 0x7D11A77B), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcconvertlocaltoutc
	#define EXT_systimertcconvertlocaltoutc
	#define GET_systimertcconvertlocaltoutc(fl)  CAL_CMGETAPI( "systimertcconvertlocaltoutc" ) 
	#define CAL_systimertcconvertlocaltoutc  systimertcconvertlocaltoutc
	#define CHK_systimertcconvertlocaltoutc  TRUE
	#define EXP_systimertcconvertlocaltoutc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltoutc", (RTS_UINTPTR)systimertcconvertlocaltoutc, 1, RTSITF_GET_SIGNATURE(0, 0x7D11A77B), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcconvertlocaltoutc  PFSYSTIMERTCCONVERTLOCALTOUTC_IEC pfsystimertcconvertlocaltoutc;
	#define EXT_systimertcconvertlocaltoutc  extern PFSYSTIMERTCCONVERTLOCALTOUTC_IEC pfsystimertcconvertlocaltoutc;
	#define GET_systimertcconvertlocaltoutc(fl)  s_pfCMGetAPI2( "systimertcconvertlocaltoutc", (RTS_VOID_FCTPTR *)&pfsystimertcconvertlocaltoutc, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x7D11A77B), 0x03050500)
	#define CAL_systimertcconvertlocaltoutc  pfsystimertcconvertlocaltoutc
	#define CHK_systimertcconvertlocaltoutc  (pfsystimertcconvertlocaltoutc != NULL)
	#define EXP_systimertcconvertlocaltoutc   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertlocaltoutc", (RTS_UINTPTR)systimertcconvertlocaltoutc, 1, RTSITF_GET_SIGNATURE(0, 0x7D11A77B), 0x03050500) 
#endif


/**
 * <description>Convert UTC timestamp in seconds to a structured format</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertcconvertutctodate_struct
{
	RTS_IEC_DWORD dwTimestampUtc;		/* VAR_INPUT */	/* <param name="dwTimestampUtc" type="IN">UTC time in seconds since 1.1.1970 00:00:00</param> */
	RTS_SYSTIMEDATE *pDate;				/* VAR_IN_OUT */	/* <param name="pDate" type="OUT">Pointer to structure format. See category for detailed information</param> */
	RTS_IEC_RESULT SysTimeRtcConvertUtcToDate;	/* VAR_OUTPUT */	
} systimertcconvertutctodate_struct;

void CDECL CDECL_EXT systimertcconvertutctodate(systimertcconvertutctodate_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCCONVERTUTCTODATE_IEC) (systimertcconvertutctodate_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTUTCTODATE_NOTIMPLEMENTED)
	#define USE_systimertcconvertutctodate
	#define EXT_systimertcconvertutctodate
	#define GET_systimertcconvertutctodate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcconvertutctodate(p0) 
	#define CHK_systimertcconvertutctodate  FALSE
	#define EXP_systimertcconvertutctodate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcconvertutctodate
	#define EXT_systimertcconvertutctodate
	#define GET_systimertcconvertutctodate(fl)  CAL_CMGETAPI( "systimertcconvertutctodate" ) 
	#define CAL_systimertcconvertutctodate  systimertcconvertutctodate
	#define CHK_systimertcconvertutctodate  TRUE
	#define EXP_systimertcconvertutctodate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctodate", (RTS_UINTPTR)systimertcconvertutctodate, 1, RTSITF_GET_SIGNATURE(0, 0x6F250D17), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcconvertutctodate
	#define EXT_systimertcconvertutctodate
	#define GET_systimertcconvertutctodate(fl)  CAL_CMGETAPI( "systimertcconvertutctodate" ) 
	#define CAL_systimertcconvertutctodate  systimertcconvertutctodate
	#define CHK_systimertcconvertutctodate  TRUE
	#define EXP_systimertcconvertutctodate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctodate", (RTS_UINTPTR)systimertcconvertutctodate, 1, RTSITF_GET_SIGNATURE(0, 0x6F250D17), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcconvertutctodate
	#define EXT_SysTimeRtcsystimertcconvertutctodate
	#define GET_SysTimeRtcsystimertcconvertutctodate  ERR_OK
	#define CAL_SysTimeRtcsystimertcconvertutctodate  systimertcconvertutctodate
	#define CHK_SysTimeRtcsystimertcconvertutctodate  TRUE
	#define EXP_SysTimeRtcsystimertcconvertutctodate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctodate", (RTS_UINTPTR)systimertcconvertutctodate, 1, RTSITF_GET_SIGNATURE(0, 0x6F250D17), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcconvertutctodate
	#define EXT_systimertcconvertutctodate
	#define GET_systimertcconvertutctodate(fl)  CAL_CMGETAPI( "systimertcconvertutctodate" ) 
	#define CAL_systimertcconvertutctodate  systimertcconvertutctodate
	#define CHK_systimertcconvertutctodate  TRUE
	#define EXP_systimertcconvertutctodate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctodate", (RTS_UINTPTR)systimertcconvertutctodate, 1, RTSITF_GET_SIGNATURE(0, 0x6F250D17), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcconvertutctodate  PFSYSTIMERTCCONVERTUTCTODATE_IEC pfsystimertcconvertutctodate;
	#define EXT_systimertcconvertutctodate  extern PFSYSTIMERTCCONVERTUTCTODATE_IEC pfsystimertcconvertutctodate;
	#define GET_systimertcconvertutctodate(fl)  s_pfCMGetAPI2( "systimertcconvertutctodate", (RTS_VOID_FCTPTR *)&pfsystimertcconvertutctodate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x6F250D17), 0x03050500)
	#define CAL_systimertcconvertutctodate  pfsystimertcconvertutctodate
	#define CHK_systimertcconvertutctodate  (pfsystimertcconvertutctodate != NULL)
	#define EXP_systimertcconvertutctodate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctodate", (RTS_UINTPTR)systimertcconvertutctodate, 1, RTSITF_GET_SIGNATURE(0, 0x6F250D17), 0x03050500) 
#endif


/**
 * <description>*OBSOLETE*: Only for backward compatibility.</description>
 */
typedef struct tagsystimertcconvertutctolocal_struct
{
	RTS_IEC_DWORD dwTimestampUtc;		/* VAR_INPUT */	
	RTS_IEC_DWORD *pdwTimestampLocal;	/* VAR_IN_OUT */	
	RTS_IEC_RESULT SysTimeRtcConvertUtcToLocal;	/* VAR_OUTPUT */	
} systimertcconvertutctolocal_struct;

void CDECL CDECL_EXT systimertcconvertutctolocal(systimertcconvertutctolocal_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCCONVERTUTCTOLOCAL_IEC) (systimertcconvertutctolocal_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTUTCTOLOCAL_NOTIMPLEMENTED)
	#define USE_systimertcconvertutctolocal
	#define EXT_systimertcconvertutctolocal
	#define GET_systimertcconvertutctolocal(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcconvertutctolocal(p0) 
	#define CHK_systimertcconvertutctolocal  FALSE
	#define EXP_systimertcconvertutctolocal  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcconvertutctolocal
	#define EXT_systimertcconvertutctolocal
	#define GET_systimertcconvertutctolocal(fl)  CAL_CMGETAPI( "systimertcconvertutctolocal" ) 
	#define CAL_systimertcconvertutctolocal  systimertcconvertutctolocal
	#define CHK_systimertcconvertutctolocal  TRUE
	#define EXP_systimertcconvertutctolocal  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctolocal", (RTS_UINTPTR)systimertcconvertutctolocal, 1, RTSITF_GET_SIGNATURE(0, 0xFE9BA7D8), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcconvertutctolocal
	#define EXT_systimertcconvertutctolocal
	#define GET_systimertcconvertutctolocal(fl)  CAL_CMGETAPI( "systimertcconvertutctolocal" ) 
	#define CAL_systimertcconvertutctolocal  systimertcconvertutctolocal
	#define CHK_systimertcconvertutctolocal  TRUE
	#define EXP_systimertcconvertutctolocal  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctolocal", (RTS_UINTPTR)systimertcconvertutctolocal, 1, RTSITF_GET_SIGNATURE(0, 0xFE9BA7D8), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcconvertutctolocal
	#define EXT_SysTimeRtcsystimertcconvertutctolocal
	#define GET_SysTimeRtcsystimertcconvertutctolocal  ERR_OK
	#define CAL_SysTimeRtcsystimertcconvertutctolocal  systimertcconvertutctolocal
	#define CHK_SysTimeRtcsystimertcconvertutctolocal  TRUE
	#define EXP_SysTimeRtcsystimertcconvertutctolocal  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctolocal", (RTS_UINTPTR)systimertcconvertutctolocal, 1, RTSITF_GET_SIGNATURE(0, 0xFE9BA7D8), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcconvertutctolocal
	#define EXT_systimertcconvertutctolocal
	#define GET_systimertcconvertutctolocal(fl)  CAL_CMGETAPI( "systimertcconvertutctolocal" ) 
	#define CAL_systimertcconvertutctolocal  systimertcconvertutctolocal
	#define CHK_systimertcconvertutctolocal  TRUE
	#define EXP_systimertcconvertutctolocal  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctolocal", (RTS_UINTPTR)systimertcconvertutctolocal, 1, RTSITF_GET_SIGNATURE(0, 0xFE9BA7D8), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcconvertutctolocal  PFSYSTIMERTCCONVERTUTCTOLOCAL_IEC pfsystimertcconvertutctolocal;
	#define EXT_systimertcconvertutctolocal  extern PFSYSTIMERTCCONVERTUTCTOLOCAL_IEC pfsystimertcconvertutctolocal;
	#define GET_systimertcconvertutctolocal(fl)  s_pfCMGetAPI2( "systimertcconvertutctolocal", (RTS_VOID_FCTPTR *)&pfsystimertcconvertutctolocal, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xFE9BA7D8), 0x03050500)
	#define CAL_systimertcconvertutctolocal  pfsystimertcconvertutctolocal
	#define CHK_systimertcconvertutctolocal  (pfsystimertcconvertutctolocal != NULL)
	#define EXP_systimertcconvertutctolocal   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcconvertutctolocal", (RTS_UINTPTR)systimertcconvertutctolocal, 1, RTSITF_GET_SIGNATURE(0, 0xFE9BA7D8), 0x03050500) 
#endif


/**
 * <description>Returns the curent Rtc (realtime clock) value in UTC.
 *	<p>UTC time: current coordinated universal time; has replaced the Greenwich Mean Time.
 * The time zones are given as positive or negative deviation from UTC.:
 *	e.g. „UTC+1“ corresponds to the Central European Time (CET) and 
 * ”UTC+2” corresponds to the Central European Summer Time (CEST)</p>
 * </description>
 * <result><p>RESULT: Seconds since 1.1.1970 00:00:00 UTC</p></result>
 */
typedef struct tagsystimertcget_struct
{
	RTS_IEC_RESULT *pResult;			/* VAR_IN_OUT */	/* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
	RTS_IEC_DWORD SysTimeRtcGet;		/* VAR_OUTPUT */	
} systimertcget_struct;

void CDECL CDECL_EXT systimertcget(systimertcget_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCGET_IEC) (systimertcget_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCGET_NOTIMPLEMENTED)
	#define USE_systimertcget
	#define EXT_systimertcget
	#define GET_systimertcget(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcget(p0) 
	#define CHK_systimertcget  FALSE
	#define EXP_systimertcget  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcget
	#define EXT_systimertcget
	#define GET_systimertcget(fl)  CAL_CMGETAPI( "systimertcget" ) 
	#define CAL_systimertcget  systimertcget
	#define CHK_systimertcget  TRUE
	#define EXP_systimertcget  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcget", (RTS_UINTPTR)systimertcget, 1, RTSITF_GET_SIGNATURE(0, 0xC885177F), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcget
	#define EXT_systimertcget
	#define GET_systimertcget(fl)  CAL_CMGETAPI( "systimertcget" ) 
	#define CAL_systimertcget  systimertcget
	#define CHK_systimertcget  TRUE
	#define EXP_systimertcget  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcget", (RTS_UINTPTR)systimertcget, 1, RTSITF_GET_SIGNATURE(0, 0xC885177F), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcget
	#define EXT_SysTimeRtcsystimertcget
	#define GET_SysTimeRtcsystimertcget  ERR_OK
	#define CAL_SysTimeRtcsystimertcget  systimertcget
	#define CHK_SysTimeRtcsystimertcget  TRUE
	#define EXP_SysTimeRtcsystimertcget  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcget", (RTS_UINTPTR)systimertcget, 1, RTSITF_GET_SIGNATURE(0, 0xC885177F), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcget
	#define EXT_systimertcget
	#define GET_systimertcget(fl)  CAL_CMGETAPI( "systimertcget" ) 
	#define CAL_systimertcget  systimertcget
	#define CHK_systimertcget  TRUE
	#define EXP_systimertcget  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcget", (RTS_UINTPTR)systimertcget, 1, RTSITF_GET_SIGNATURE(0, 0xC885177F), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcget  PFSYSTIMERTCGET_IEC pfsystimertcget;
	#define EXT_systimertcget  extern PFSYSTIMERTCGET_IEC pfsystimertcget;
	#define GET_systimertcget(fl)  s_pfCMGetAPI2( "systimertcget", (RTS_VOID_FCTPTR *)&pfsystimertcget, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC885177F), 0x03050500)
	#define CAL_systimertcget  pfsystimertcget
	#define CHK_systimertcget  (pfsystimertcget != NULL)
	#define EXP_systimertcget   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcget", (RTS_UINTPTR)systimertcget, 1, RTSITF_GET_SIGNATURE(0, 0xC885177F), 0x03050500) 
#endif


/**
 * <description>Returns the timezone information</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertcgettimezone_struct
{
	TimezoneInformation *pTimezone;		/* VAR_IN_OUT */	/* <param name="pTimezone" type="OUT">Pointer to timezone information. See corresponding category.</param> */
	RTS_IEC_RESULT SysTimeRtcGetTimezone;	/* VAR_OUTPUT */	
} systimertcgettimezone_struct;

void CDECL CDECL_EXT systimertcgettimezone(systimertcgettimezone_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCGETTIMEZONE_IEC) (systimertcgettimezone_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCGETTIMEZONE_NOTIMPLEMENTED)
	#define USE_systimertcgettimezone
	#define EXT_systimertcgettimezone
	#define GET_systimertcgettimezone(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcgettimezone(p0) 
	#define CHK_systimertcgettimezone  FALSE
	#define EXP_systimertcgettimezone  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcgettimezone
	#define EXT_systimertcgettimezone
	#define GET_systimertcgettimezone(fl)  CAL_CMGETAPI( "systimertcgettimezone" ) 
	#define CAL_systimertcgettimezone  systimertcgettimezone
	#define CHK_systimertcgettimezone  TRUE
	#define EXP_systimertcgettimezone  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcgettimezone", (RTS_UINTPTR)systimertcgettimezone, 1, RTSITF_GET_SIGNATURE(0, 0xCC6D982F), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcgettimezone
	#define EXT_systimertcgettimezone
	#define GET_systimertcgettimezone(fl)  CAL_CMGETAPI( "systimertcgettimezone" ) 
	#define CAL_systimertcgettimezone  systimertcgettimezone
	#define CHK_systimertcgettimezone  TRUE
	#define EXP_systimertcgettimezone  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcgettimezone", (RTS_UINTPTR)systimertcgettimezone, 1, RTSITF_GET_SIGNATURE(0, 0xCC6D982F), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcgettimezone
	#define EXT_SysTimeRtcsystimertcgettimezone
	#define GET_SysTimeRtcsystimertcgettimezone  ERR_OK
	#define CAL_SysTimeRtcsystimertcgettimezone  systimertcgettimezone
	#define CHK_SysTimeRtcsystimertcgettimezone  TRUE
	#define EXP_SysTimeRtcsystimertcgettimezone  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcgettimezone", (RTS_UINTPTR)systimertcgettimezone, 1, RTSITF_GET_SIGNATURE(0, 0xCC6D982F), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcgettimezone
	#define EXT_systimertcgettimezone
	#define GET_systimertcgettimezone(fl)  CAL_CMGETAPI( "systimertcgettimezone" ) 
	#define CAL_systimertcgettimezone  systimertcgettimezone
	#define CHK_systimertcgettimezone  TRUE
	#define EXP_systimertcgettimezone  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcgettimezone", (RTS_UINTPTR)systimertcgettimezone, 1, RTSITF_GET_SIGNATURE(0, 0xCC6D982F), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcgettimezone  PFSYSTIMERTCGETTIMEZONE_IEC pfsystimertcgettimezone;
	#define EXT_systimertcgettimezone  extern PFSYSTIMERTCGETTIMEZONE_IEC pfsystimertcgettimezone;
	#define GET_systimertcgettimezone(fl)  s_pfCMGetAPI2( "systimertcgettimezone", (RTS_VOID_FCTPTR *)&pfsystimertcgettimezone, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xCC6D982F), 0x03050500)
	#define CAL_systimertcgettimezone  pfsystimertcgettimezone
	#define CHK_systimertcgettimezone  (pfsystimertcgettimezone != NULL)
	#define EXP_systimertcgettimezone   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcgettimezone", (RTS_UINTPTR)systimertcgettimezone, 1, RTSITF_GET_SIGNATURE(0, 0xCC6D982F), 0x03050500) 
#endif


/**
 * <description>Returns the Rtc (realtime clock) value with a high resolution in UTC.
 *	<p>UTC time: current coordinated universal time; has replaced the Greenwich Mean Time.
 * The time zones are given as positive or negative deviation from UTC.:
 *	e.g. „UTC+1“ corresponds to the Central European Time (CET) and 
 * ”UTC+2” corresponds to the Central European Summer Time (CEST)</p>
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertchighresget_struct
{
	RTS_IEC_ULINT *pTimestamp;			/* VAR_IN_OUT */	/* <param name="pTimestamp" type="OUT">Milliseconds since 1.1.1970 00:00:000 UTC</param> */
	RTS_IEC_RESULT SysTimeRtcHighResGet;	/* VAR_OUTPUT */	
} systimertchighresget_struct;

void CDECL CDECL_EXT systimertchighresget(systimertchighresget_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCHIGHRESGET_IEC) (systimertchighresget_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCHIGHRESGET_NOTIMPLEMENTED)
	#define USE_systimertchighresget
	#define EXT_systimertchighresget
	#define GET_systimertchighresget(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertchighresget(p0) 
	#define CHK_systimertchighresget  FALSE
	#define EXP_systimertchighresget  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertchighresget
	#define EXT_systimertchighresget
	#define GET_systimertchighresget(fl)  CAL_CMGETAPI( "systimertchighresget" ) 
	#define CAL_systimertchighresget  systimertchighresget
	#define CHK_systimertchighresget  TRUE
	#define EXP_systimertchighresget  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresget", (RTS_UINTPTR)systimertchighresget, 1, RTSITF_GET_SIGNATURE(0, 0xDA82D1F0), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertchighresget
	#define EXT_systimertchighresget
	#define GET_systimertchighresget(fl)  CAL_CMGETAPI( "systimertchighresget" ) 
	#define CAL_systimertchighresget  systimertchighresget
	#define CHK_systimertchighresget  TRUE
	#define EXP_systimertchighresget  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresget", (RTS_UINTPTR)systimertchighresget, 1, RTSITF_GET_SIGNATURE(0, 0xDA82D1F0), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertchighresget
	#define EXT_SysTimeRtcsystimertchighresget
	#define GET_SysTimeRtcsystimertchighresget  ERR_OK
	#define CAL_SysTimeRtcsystimertchighresget  systimertchighresget
	#define CHK_SysTimeRtcsystimertchighresget  TRUE
	#define EXP_SysTimeRtcsystimertchighresget  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresget", (RTS_UINTPTR)systimertchighresget, 1, RTSITF_GET_SIGNATURE(0, 0xDA82D1F0), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertchighresget
	#define EXT_systimertchighresget
	#define GET_systimertchighresget(fl)  CAL_CMGETAPI( "systimertchighresget" ) 
	#define CAL_systimertchighresget  systimertchighresget
	#define CHK_systimertchighresget  TRUE
	#define EXP_systimertchighresget  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresget", (RTS_UINTPTR)systimertchighresget, 1, RTSITF_GET_SIGNATURE(0, 0xDA82D1F0), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertchighresget  PFSYSTIMERTCHIGHRESGET_IEC pfsystimertchighresget;
	#define EXT_systimertchighresget  extern PFSYSTIMERTCHIGHRESGET_IEC pfsystimertchighresget;
	#define GET_systimertchighresget(fl)  s_pfCMGetAPI2( "systimertchighresget", (RTS_VOID_FCTPTR *)&pfsystimertchighresget, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xDA82D1F0), 0x03050500)
	#define CAL_systimertchighresget  pfsystimertchighresget
	#define CHK_systimertchighresget  (pfsystimertchighresget != NULL)
	#define EXP_systimertchighresget   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresget", (RTS_UINTPTR)systimertchighresget, 1, RTSITF_GET_SIGNATURE(0, 0xDA82D1F0), 0x03050500) 
#endif


/**
 * <description>Set the Rtc (realtime clock) value with a high resolution.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystimertchighresset_struct
{
	RTS_IEC_ULINT *pTimestamp;			/* VAR_IN_OUT */	/* <param name="pTimestamp" type="OUT">Milliseconds since 1.1.1970 00:00:000 UTC</param> */
	RTS_IEC_RESULT SysTimeRtcHighResSet;	/* VAR_OUTPUT */	
} systimertchighresset_struct;

void CDECL CDECL_EXT systimertchighresset(systimertchighresset_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCHIGHRESSET_IEC) (systimertchighresset_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCHIGHRESSET_NOTIMPLEMENTED)
	#define USE_systimertchighresset
	#define EXT_systimertchighresset
	#define GET_systimertchighresset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertchighresset(p0) 
	#define CHK_systimertchighresset  FALSE
	#define EXP_systimertchighresset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertchighresset
	#define EXT_systimertchighresset
	#define GET_systimertchighresset(fl)  CAL_CMGETAPI( "systimertchighresset" ) 
	#define CAL_systimertchighresset  systimertchighresset
	#define CHK_systimertchighresset  TRUE
	#define EXP_systimertchighresset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresset", (RTS_UINTPTR)systimertchighresset, 1, RTSITF_GET_SIGNATURE(0, 0x6902D901), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertchighresset
	#define EXT_systimertchighresset
	#define GET_systimertchighresset(fl)  CAL_CMGETAPI( "systimertchighresset" ) 
	#define CAL_systimertchighresset  systimertchighresset
	#define CHK_systimertchighresset  TRUE
	#define EXP_systimertchighresset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresset", (RTS_UINTPTR)systimertchighresset, 1, RTSITF_GET_SIGNATURE(0, 0x6902D901), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertchighresset
	#define EXT_SysTimeRtcsystimertchighresset
	#define GET_SysTimeRtcsystimertchighresset  ERR_OK
	#define CAL_SysTimeRtcsystimertchighresset  systimertchighresset
	#define CHK_SysTimeRtcsystimertchighresset  TRUE
	#define EXP_SysTimeRtcsystimertchighresset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresset", (RTS_UINTPTR)systimertchighresset, 1, RTSITF_GET_SIGNATURE(0, 0x6902D901), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertchighresset
	#define EXT_systimertchighresset
	#define GET_systimertchighresset(fl)  CAL_CMGETAPI( "systimertchighresset" ) 
	#define CAL_systimertchighresset  systimertchighresset
	#define CHK_systimertchighresset  TRUE
	#define EXP_systimertchighresset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresset", (RTS_UINTPTR)systimertchighresset, 1, RTSITF_GET_SIGNATURE(0, 0x6902D901), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertchighresset  PFSYSTIMERTCHIGHRESSET_IEC pfsystimertchighresset;
	#define EXT_systimertchighresset  extern PFSYSTIMERTCHIGHRESSET_IEC pfsystimertchighresset;
	#define GET_systimertchighresset(fl)  s_pfCMGetAPI2( "systimertchighresset", (RTS_VOID_FCTPTR *)&pfsystimertchighresset, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x6902D901), 0x03050500)
	#define CAL_systimertchighresset  pfsystimertchighresset
	#define CHK_systimertchighresset  (pfsystimertchighresset != NULL)
	#define EXP_systimertchighresset   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertchighresset", (RTS_UINTPTR)systimertchighresset, 1, RTSITF_GET_SIGNATURE(0, 0x6902D901), 0x03050500) 
#endif


/**
 * <description>Set the Rtc (realtime clock) value in UTC.
 *	<p>UTC time: current coordinated universal time; has replaced the Greenwich Mean Time.
 * The time zones are given as positive or negative deviation from UTC.:
 *	e.g. „UTC+1“ corresponds to the Central European Time (CET) and 
 * ”UTC+2” corresponds to the Central European Summer Time (CEST)</p>
 * </description>
 */
typedef struct tagsystimertcset_struct
{
	RTS_IEC_DWORD ulTimestamp;			/* VAR_INPUT */	/* <param name="ulTimestamp" type="IN">Seconds since 1.1.1970 00:00:00 UTC</param> */
	RTS_IEC_RESULT SysTimeRtcSet;		/* VAR_OUTPUT */	
} systimertcset_struct;

void CDECL CDECL_EXT systimertcset(systimertcset_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCSET_IEC) (systimertcset_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCSET_NOTIMPLEMENTED)
	#define USE_systimertcset
	#define EXT_systimertcset
	#define GET_systimertcset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcset(p0) 
	#define CHK_systimertcset  FALSE
	#define EXP_systimertcset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcset
	#define EXT_systimertcset
	#define GET_systimertcset(fl)  CAL_CMGETAPI( "systimertcset" ) 
	#define CAL_systimertcset  systimertcset
	#define CHK_systimertcset  TRUE
	#define EXP_systimertcset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcset", (RTS_UINTPTR)systimertcset, 1, RTSITF_GET_SIGNATURE(0, 0x288F7E63), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcset
	#define EXT_systimertcset
	#define GET_systimertcset(fl)  CAL_CMGETAPI( "systimertcset" ) 
	#define CAL_systimertcset  systimertcset
	#define CHK_systimertcset  TRUE
	#define EXP_systimertcset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcset", (RTS_UINTPTR)systimertcset, 1, RTSITF_GET_SIGNATURE(0, 0x288F7E63), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcset
	#define EXT_SysTimeRtcsystimertcset
	#define GET_SysTimeRtcsystimertcset  ERR_OK
	#define CAL_SysTimeRtcsystimertcset  systimertcset
	#define CHK_SysTimeRtcsystimertcset  TRUE
	#define EXP_SysTimeRtcsystimertcset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcset", (RTS_UINTPTR)systimertcset, 1, RTSITF_GET_SIGNATURE(0, 0x288F7E63), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcset
	#define EXT_systimertcset
	#define GET_systimertcset(fl)  CAL_CMGETAPI( "systimertcset" ) 
	#define CAL_systimertcset  systimertcset
	#define CHK_systimertcset  TRUE
	#define EXP_systimertcset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcset", (RTS_UINTPTR)systimertcset, 1, RTSITF_GET_SIGNATURE(0, 0x288F7E63), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcset  PFSYSTIMERTCSET_IEC pfsystimertcset;
	#define EXT_systimertcset  extern PFSYSTIMERTCSET_IEC pfsystimertcset;
	#define GET_systimertcset(fl)  s_pfCMGetAPI2( "systimertcset", (RTS_VOID_FCTPTR *)&pfsystimertcset, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x288F7E63), 0x03050500)
	#define CAL_systimertcset  pfsystimertcset
	#define CHK_systimertcset  (pfsystimertcset != NULL)
	#define EXP_systimertcset   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcset", (RTS_UINTPTR)systimertcset, 1, RTSITF_GET_SIGNATURE(0, 0x288F7E63), 0x03050500) 
#endif


/**
 * <description>Set the specified timezone information</description>
 */
typedef struct tagsystimertcsettimezone_struct
{
	TimezoneInformation *pTimezone;		/* VAR_IN_OUT */	/* <param name="pTimezone" type="IN">Pointer to timezone information. See corresponding category.</param> */
	RTS_IEC_RESULT SysTimeRtcSetTimezone;	/* VAR_OUTPUT */	
} systimertcsettimezone_struct;

void CDECL CDECL_EXT systimertcsettimezone(systimertcsettimezone_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTIMERTCSETTIMEZONE_IEC) (systimertcsettimezone_struct *p);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCSETTIMEZONE_NOTIMPLEMENTED)
	#define USE_systimertcsettimezone
	#define EXT_systimertcsettimezone
	#define GET_systimertcsettimezone(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systimertcsettimezone(p0) 
	#define CHK_systimertcsettimezone  FALSE
	#define EXP_systimertcsettimezone  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systimertcsettimezone
	#define EXT_systimertcsettimezone
	#define GET_systimertcsettimezone(fl)  CAL_CMGETAPI( "systimertcsettimezone" ) 
	#define CAL_systimertcsettimezone  systimertcsettimezone
	#define CHK_systimertcsettimezone  TRUE
	#define EXP_systimertcsettimezone  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcsettimezone", (RTS_UINTPTR)systimertcsettimezone, 1, RTSITF_GET_SIGNATURE(0, 0x018F4E27), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_systimertcsettimezone
	#define EXT_systimertcsettimezone
	#define GET_systimertcsettimezone(fl)  CAL_CMGETAPI( "systimertcsettimezone" ) 
	#define CAL_systimertcsettimezone  systimertcsettimezone
	#define CHK_systimertcsettimezone  TRUE
	#define EXP_systimertcsettimezone  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcsettimezone", (RTS_UINTPTR)systimertcsettimezone, 1, RTSITF_GET_SIGNATURE(0, 0x018F4E27), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcsystimertcsettimezone
	#define EXT_SysTimeRtcsystimertcsettimezone
	#define GET_SysTimeRtcsystimertcsettimezone  ERR_OK
	#define CAL_SysTimeRtcsystimertcsettimezone  systimertcsettimezone
	#define CHK_SysTimeRtcsystimertcsettimezone  TRUE
	#define EXP_SysTimeRtcsystimertcsettimezone  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcsettimezone", (RTS_UINTPTR)systimertcsettimezone, 1, RTSITF_GET_SIGNATURE(0, 0x018F4E27), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_systimertcsettimezone
	#define EXT_systimertcsettimezone
	#define GET_systimertcsettimezone(fl)  CAL_CMGETAPI( "systimertcsettimezone" ) 
	#define CAL_systimertcsettimezone  systimertcsettimezone
	#define CHK_systimertcsettimezone  TRUE
	#define EXP_systimertcsettimezone  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcsettimezone", (RTS_UINTPTR)systimertcsettimezone, 1, RTSITF_GET_SIGNATURE(0, 0x018F4E27), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_systimertcsettimezone  PFSYSTIMERTCSETTIMEZONE_IEC pfsystimertcsettimezone;
	#define EXT_systimertcsettimezone  extern PFSYSTIMERTCSETTIMEZONE_IEC pfsystimertcsettimezone;
	#define GET_systimertcsettimezone(fl)  s_pfCMGetAPI2( "systimertcsettimezone", (RTS_VOID_FCTPTR *)&pfsystimertcsettimezone, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x018F4E27), 0x03050500)
	#define CAL_systimertcsettimezone  pfsystimertcsettimezone
	#define CHK_systimertcsettimezone  (pfsystimertcsettimezone != NULL)
	#define EXP_systimertcsettimezone   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systimertcsettimezone", (RTS_UINTPTR)systimertcsettimezone, 1, RTSITF_GET_SIGNATURE(0, 0x018F4E27), 0x03050500) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/* Init routines for OS specific modules */
RTS_RESULT CDECL SysTimeRtcOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysTimeRtcOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>Returns the Rtc value (realtime clock). The value is the elapsed number of seconds since
 *	1.1.1970 00:00:00 UTC time.</description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Seconds since 1.1.1970 00:00:00 UTC</result>
 */
RTS_UI32 CDECL SysTimeRtcGet(RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFSYSTIMERTCGET) (RTS_RESULT *pResult);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCGET_NOTIMPLEMENTED)
	#define USE_SysTimeRtcGet
	#define EXT_SysTimeRtcGet
	#define GET_SysTimeRtcGet(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcGet(p0)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcGet  FALSE
	#define EXP_SysTimeRtcGet  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcGet
	#define EXT_SysTimeRtcGet
	#define GET_SysTimeRtcGet(fl)  CAL_CMGETAPI( "SysTimeRtcGet" ) 
	#define CAL_SysTimeRtcGet  SysTimeRtcGet
	#define CHK_SysTimeRtcGet  TRUE
	#define EXP_SysTimeRtcGet  CAL_CMEXPAPI( "SysTimeRtcGet" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcGet
	#define EXT_SysTimeRtcGet
	#define GET_SysTimeRtcGet(fl)  CAL_CMGETAPI( "SysTimeRtcGet" ) 
	#define CAL_SysTimeRtcGet  SysTimeRtcGet
	#define CHK_SysTimeRtcGet  TRUE
	#define EXP_SysTimeRtcGet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcGet", (RTS_UINTPTR)SysTimeRtcGet, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcGet
	#define EXT_SysTimeRtcSysTimeRtcGet
	#define GET_SysTimeRtcSysTimeRtcGet  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcGet pISysTimeRtc->ISysTimeRtcGet
	#define CHK_SysTimeRtcSysTimeRtcGet (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcGet  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcGet
	#define EXT_SysTimeRtcGet
	#define GET_SysTimeRtcGet(fl)  CAL_CMGETAPI( "SysTimeRtcGet" ) 
	#define CAL_SysTimeRtcGet pISysTimeRtc->ISysTimeRtcGet
	#define CHK_SysTimeRtcGet (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcGet  CAL_CMEXPAPI( "SysTimeRtcGet" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcGet  PFSYSTIMERTCGET pfSysTimeRtcGet;
	#define EXT_SysTimeRtcGet  extern PFSYSTIMERTCGET pfSysTimeRtcGet;
	#define GET_SysTimeRtcGet(fl)  s_pfCMGetAPI2( "SysTimeRtcGet", (RTS_VOID_FCTPTR *)&pfSysTimeRtcGet, (fl), 0, 0)
	#define CAL_SysTimeRtcGet  pfSysTimeRtcGet
	#define CHK_SysTimeRtcGet  (pfSysTimeRtcGet != NULL)
	#define EXP_SysTimeRtcGet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcGet", (RTS_UINTPTR)SysTimeRtcGet, 0, 0) 
#endif




/**
 * <description>
 * OPTIONAL INTERFACE FUNCTION
 * Set the Rtc value (realtime clock). The value is the elapsed number of seconds since
 *	1.1.1970 00:00:00 UTC time.
 * </description>
 * <param name="ulTimestamp" type="IN">Seconds since 1.1.1970 00:00:00 [UTC]</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Timestamp can be set</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Timestamp failed to set</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Set is not available on the target</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_ACCESS_RIGHTS">No access rights to set the system time</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcSet(RTS_UI32 ulTimestampUtc);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCSET) (RTS_UI32 ulTimestampUtc);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCSET_NOTIMPLEMENTED)
	#define USE_SysTimeRtcSet
	#define EXT_SysTimeRtcSet
	#define GET_SysTimeRtcSet(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcSet(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcSet  FALSE
	#define EXP_SysTimeRtcSet  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcSet
	#define EXT_SysTimeRtcSet
	#define GET_SysTimeRtcSet(fl)  CAL_CMGETAPI( "SysTimeRtcSet" ) 
	#define CAL_SysTimeRtcSet  SysTimeRtcSet
	#define CHK_SysTimeRtcSet  TRUE
	#define EXP_SysTimeRtcSet  CAL_CMEXPAPI( "SysTimeRtcSet" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcSet
	#define EXT_SysTimeRtcSet
	#define GET_SysTimeRtcSet(fl)  CAL_CMGETAPI( "SysTimeRtcSet" ) 
	#define CAL_SysTimeRtcSet  SysTimeRtcSet
	#define CHK_SysTimeRtcSet  TRUE
	#define EXP_SysTimeRtcSet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcSet", (RTS_UINTPTR)SysTimeRtcSet, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcSet
	#define EXT_SysTimeRtcSysTimeRtcSet
	#define GET_SysTimeRtcSysTimeRtcSet  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcSet pISysTimeRtc->ISysTimeRtcSet
	#define CHK_SysTimeRtcSysTimeRtcSet (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcSet  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcSet
	#define EXT_SysTimeRtcSet
	#define GET_SysTimeRtcSet(fl)  CAL_CMGETAPI( "SysTimeRtcSet" ) 
	#define CAL_SysTimeRtcSet pISysTimeRtc->ISysTimeRtcSet
	#define CHK_SysTimeRtcSet (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSet  CAL_CMEXPAPI( "SysTimeRtcSet" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcSet  PFSYSTIMERTCSET pfSysTimeRtcSet;
	#define EXT_SysTimeRtcSet  extern PFSYSTIMERTCSET pfSysTimeRtcSet;
	#define GET_SysTimeRtcSet(fl)  s_pfCMGetAPI2( "SysTimeRtcSet", (RTS_VOID_FCTPTR *)&pfSysTimeRtcSet, (fl), 0, 0)
	#define CAL_SysTimeRtcSet  pfSysTimeRtcSet
	#define CHK_SysTimeRtcSet  (pfSysTimeRtcSet != NULL)
	#define EXP_SysTimeRtcSet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcSet", (RTS_UINTPTR)SysTimeRtcSet, 0, 0) 
#endif




/**
 * <description>
 * OPTIONAL INTERFACE FUNCTION
 * Returns the timezone information
 * </description>
 * <param name="pTimezone" type="OUT">Pointer to timezone information. See corresponding category.</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Timezone available</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Get Timezone failed</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Timezone not available on the target</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcGetTimezone(TimezoneInformation *pTimezone);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCGETTIMEZONE) (TimezoneInformation *pTimezone);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCGETTIMEZONE_NOTIMPLEMENTED)
	#define USE_SysTimeRtcGetTimezone
	#define EXT_SysTimeRtcGetTimezone
	#define GET_SysTimeRtcGetTimezone(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcGetTimezone(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcGetTimezone  FALSE
	#define EXP_SysTimeRtcGetTimezone  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcGetTimezone
	#define EXT_SysTimeRtcGetTimezone
	#define GET_SysTimeRtcGetTimezone(fl)  CAL_CMGETAPI( "SysTimeRtcGetTimezone" ) 
	#define CAL_SysTimeRtcGetTimezone  SysTimeRtcGetTimezone
	#define CHK_SysTimeRtcGetTimezone  TRUE
	#define EXP_SysTimeRtcGetTimezone  CAL_CMEXPAPI( "SysTimeRtcGetTimezone" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcGetTimezone
	#define EXT_SysTimeRtcGetTimezone
	#define GET_SysTimeRtcGetTimezone(fl)  CAL_CMGETAPI( "SysTimeRtcGetTimezone" ) 
	#define CAL_SysTimeRtcGetTimezone  SysTimeRtcGetTimezone
	#define CHK_SysTimeRtcGetTimezone  TRUE
	#define EXP_SysTimeRtcGetTimezone  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcGetTimezone", (RTS_UINTPTR)SysTimeRtcGetTimezone, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcGetTimezone
	#define EXT_SysTimeRtcSysTimeRtcGetTimezone
	#define GET_SysTimeRtcSysTimeRtcGetTimezone  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcGetTimezone pISysTimeRtc->ISysTimeRtcGetTimezone
	#define CHK_SysTimeRtcSysTimeRtcGetTimezone (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcGetTimezone  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcGetTimezone
	#define EXT_SysTimeRtcGetTimezone
	#define GET_SysTimeRtcGetTimezone(fl)  CAL_CMGETAPI( "SysTimeRtcGetTimezone" ) 
	#define CAL_SysTimeRtcGetTimezone pISysTimeRtc->ISysTimeRtcGetTimezone
	#define CHK_SysTimeRtcGetTimezone (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcGetTimezone  CAL_CMEXPAPI( "SysTimeRtcGetTimezone" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcGetTimezone  PFSYSTIMERTCGETTIMEZONE pfSysTimeRtcGetTimezone;
	#define EXT_SysTimeRtcGetTimezone  extern PFSYSTIMERTCGETTIMEZONE pfSysTimeRtcGetTimezone;
	#define GET_SysTimeRtcGetTimezone(fl)  s_pfCMGetAPI2( "SysTimeRtcGetTimezone", (RTS_VOID_FCTPTR *)&pfSysTimeRtcGetTimezone, (fl), 0, 0)
	#define CAL_SysTimeRtcGetTimezone  pfSysTimeRtcGetTimezone
	#define CHK_SysTimeRtcGetTimezone  (pfSysTimeRtcGetTimezone != NULL)
	#define EXP_SysTimeRtcGetTimezone  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcGetTimezone", (RTS_UINTPTR)SysTimeRtcGetTimezone, 0, 0) 
#endif




/**
 * <description>
 * OPTIONAL INTERFACE FUNCTION
 * Set the specified timezone information
 * </description>
 * <param name="pTimezone" type="IN">Pointer to timezone information. See corresponding category.</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Timezone available</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Set Timezone failed</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Timezone not available on the target</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcSetTimezone(TimezoneInformation *pTimezone);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCSETTIMEZONE) (TimezoneInformation *pTimezone);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCSETTIMEZONE_NOTIMPLEMENTED)
	#define USE_SysTimeRtcSetTimezone
	#define EXT_SysTimeRtcSetTimezone
	#define GET_SysTimeRtcSetTimezone(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcSetTimezone(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcSetTimezone  FALSE
	#define EXP_SysTimeRtcSetTimezone  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcSetTimezone
	#define EXT_SysTimeRtcSetTimezone
	#define GET_SysTimeRtcSetTimezone(fl)  CAL_CMGETAPI( "SysTimeRtcSetTimezone" ) 
	#define CAL_SysTimeRtcSetTimezone  SysTimeRtcSetTimezone
	#define CHK_SysTimeRtcSetTimezone  TRUE
	#define EXP_SysTimeRtcSetTimezone  CAL_CMEXPAPI( "SysTimeRtcSetTimezone" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcSetTimezone
	#define EXT_SysTimeRtcSetTimezone
	#define GET_SysTimeRtcSetTimezone(fl)  CAL_CMGETAPI( "SysTimeRtcSetTimezone" ) 
	#define CAL_SysTimeRtcSetTimezone  SysTimeRtcSetTimezone
	#define CHK_SysTimeRtcSetTimezone  TRUE
	#define EXP_SysTimeRtcSetTimezone  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcSetTimezone", (RTS_UINTPTR)SysTimeRtcSetTimezone, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcSetTimezone
	#define EXT_SysTimeRtcSysTimeRtcSetTimezone
	#define GET_SysTimeRtcSysTimeRtcSetTimezone  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcSetTimezone pISysTimeRtc->ISysTimeRtcSetTimezone
	#define CHK_SysTimeRtcSysTimeRtcSetTimezone (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcSetTimezone  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcSetTimezone
	#define EXT_SysTimeRtcSetTimezone
	#define GET_SysTimeRtcSetTimezone(fl)  CAL_CMGETAPI( "SysTimeRtcSetTimezone" ) 
	#define CAL_SysTimeRtcSetTimezone pISysTimeRtc->ISysTimeRtcSetTimezone
	#define CHK_SysTimeRtcSetTimezone (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSetTimezone  CAL_CMEXPAPI( "SysTimeRtcSetTimezone" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcSetTimezone  PFSYSTIMERTCSETTIMEZONE pfSysTimeRtcSetTimezone;
	#define EXT_SysTimeRtcSetTimezone  extern PFSYSTIMERTCSETTIMEZONE pfSysTimeRtcSetTimezone;
	#define GET_SysTimeRtcSetTimezone(fl)  s_pfCMGetAPI2( "SysTimeRtcSetTimezone", (RTS_VOID_FCTPTR *)&pfSysTimeRtcSetTimezone, (fl), 0, 0)
	#define CAL_SysTimeRtcSetTimezone  pfSysTimeRtcSetTimezone
	#define CHK_SysTimeRtcSetTimezone  (pfSysTimeRtcSetTimezone != NULL)
	#define EXP_SysTimeRtcSetTimezone  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcSetTimezone", (RTS_UINTPTR)SysTimeRtcSetTimezone, 0, 0) 
#endif




/**
 * <description>Convert UTC time to local time</description>
 * <param name="ulTimestampUtc" type="IN">UTC time in seconds since 1.1.1970 00:00:00</param>
 * <param name="pulTimestampLocal" type="OUT">Local time in seconds since 1.1.1970 00:00:00</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcConvertUtcToLocal(RTS_UI32 ulTimestampUtc, RTS_UI32 *pulTimestampLocal);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCCONVERTUTCTOLOCAL) (RTS_UI32 ulTimestampUtc, RTS_UI32 *pulTimestampLocal);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTUTCTOLOCAL_NOTIMPLEMENTED)
	#define USE_SysTimeRtcConvertUtcToLocal
	#define EXT_SysTimeRtcConvertUtcToLocal
	#define GET_SysTimeRtcConvertUtcToLocal(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcConvertUtcToLocal(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcConvertUtcToLocal  FALSE
	#define EXP_SysTimeRtcConvertUtcToLocal  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcConvertUtcToLocal
	#define EXT_SysTimeRtcConvertUtcToLocal
	#define GET_SysTimeRtcConvertUtcToLocal(fl)  CAL_CMGETAPI( "SysTimeRtcConvertUtcToLocal" ) 
	#define CAL_SysTimeRtcConvertUtcToLocal  SysTimeRtcConvertUtcToLocal
	#define CHK_SysTimeRtcConvertUtcToLocal  TRUE
	#define EXP_SysTimeRtcConvertUtcToLocal  CAL_CMEXPAPI( "SysTimeRtcConvertUtcToLocal" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcConvertUtcToLocal
	#define EXT_SysTimeRtcConvertUtcToLocal
	#define GET_SysTimeRtcConvertUtcToLocal(fl)  CAL_CMGETAPI( "SysTimeRtcConvertUtcToLocal" ) 
	#define CAL_SysTimeRtcConvertUtcToLocal  SysTimeRtcConvertUtcToLocal
	#define CHK_SysTimeRtcConvertUtcToLocal  TRUE
	#define EXP_SysTimeRtcConvertUtcToLocal  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertUtcToLocal", (RTS_UINTPTR)SysTimeRtcConvertUtcToLocal, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcConvertUtcToLocal
	#define EXT_SysTimeRtcSysTimeRtcConvertUtcToLocal
	#define GET_SysTimeRtcSysTimeRtcConvertUtcToLocal  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcConvertUtcToLocal pISysTimeRtc->ISysTimeRtcConvertUtcToLocal
	#define CHK_SysTimeRtcSysTimeRtcConvertUtcToLocal (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcConvertUtcToLocal  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcConvertUtcToLocal
	#define EXT_SysTimeRtcConvertUtcToLocal
	#define GET_SysTimeRtcConvertUtcToLocal(fl)  CAL_CMGETAPI( "SysTimeRtcConvertUtcToLocal" ) 
	#define CAL_SysTimeRtcConvertUtcToLocal pISysTimeRtc->ISysTimeRtcConvertUtcToLocal
	#define CHK_SysTimeRtcConvertUtcToLocal (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcConvertUtcToLocal  CAL_CMEXPAPI( "SysTimeRtcConvertUtcToLocal" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcConvertUtcToLocal  PFSYSTIMERTCCONVERTUTCTOLOCAL pfSysTimeRtcConvertUtcToLocal;
	#define EXT_SysTimeRtcConvertUtcToLocal  extern PFSYSTIMERTCCONVERTUTCTOLOCAL pfSysTimeRtcConvertUtcToLocal;
	#define GET_SysTimeRtcConvertUtcToLocal(fl)  s_pfCMGetAPI2( "SysTimeRtcConvertUtcToLocal", (RTS_VOID_FCTPTR *)&pfSysTimeRtcConvertUtcToLocal, (fl), 0, 0)
	#define CAL_SysTimeRtcConvertUtcToLocal  pfSysTimeRtcConvertUtcToLocal
	#define CHK_SysTimeRtcConvertUtcToLocal  (pfSysTimeRtcConvertUtcToLocal != NULL)
	#define EXP_SysTimeRtcConvertUtcToLocal  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertUtcToLocal", (RTS_UINTPTR)SysTimeRtcConvertUtcToLocal, 0, 0) 
#endif




/**
 * <description>Convert local time to UTC</description>
 * <param name="ulTimestampLocal" type="IN">Local time in seconds since 1.1.1970 00:00:00</param>
 * <param name="pulTimestampUtc" type="OUT">UTC time in seconds since 1.1.1970 00:00:00</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcConvertLocalToUtc(RTS_UI32 ulTimestampLocal, RTS_UI32 *pulTimestampUtc);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCCONVERTLOCALTOUTC) (RTS_UI32 ulTimestampLocal, RTS_UI32 *pulTimestampUtc);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTLOCALTOUTC_NOTIMPLEMENTED)
	#define USE_SysTimeRtcConvertLocalToUtc
	#define EXT_SysTimeRtcConvertLocalToUtc
	#define GET_SysTimeRtcConvertLocalToUtc(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcConvertLocalToUtc(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcConvertLocalToUtc  FALSE
	#define EXP_SysTimeRtcConvertLocalToUtc  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcConvertLocalToUtc
	#define EXT_SysTimeRtcConvertLocalToUtc
	#define GET_SysTimeRtcConvertLocalToUtc(fl)  CAL_CMGETAPI( "SysTimeRtcConvertLocalToUtc" ) 
	#define CAL_SysTimeRtcConvertLocalToUtc  SysTimeRtcConvertLocalToUtc
	#define CHK_SysTimeRtcConvertLocalToUtc  TRUE
	#define EXP_SysTimeRtcConvertLocalToUtc  CAL_CMEXPAPI( "SysTimeRtcConvertLocalToUtc" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcConvertLocalToUtc
	#define EXT_SysTimeRtcConvertLocalToUtc
	#define GET_SysTimeRtcConvertLocalToUtc(fl)  CAL_CMGETAPI( "SysTimeRtcConvertLocalToUtc" ) 
	#define CAL_SysTimeRtcConvertLocalToUtc  SysTimeRtcConvertLocalToUtc
	#define CHK_SysTimeRtcConvertLocalToUtc  TRUE
	#define EXP_SysTimeRtcConvertLocalToUtc  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertLocalToUtc", (RTS_UINTPTR)SysTimeRtcConvertLocalToUtc, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcConvertLocalToUtc
	#define EXT_SysTimeRtcSysTimeRtcConvertLocalToUtc
	#define GET_SysTimeRtcSysTimeRtcConvertLocalToUtc  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcConvertLocalToUtc pISysTimeRtc->ISysTimeRtcConvertLocalToUtc
	#define CHK_SysTimeRtcSysTimeRtcConvertLocalToUtc (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcConvertLocalToUtc  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcConvertLocalToUtc
	#define EXT_SysTimeRtcConvertLocalToUtc
	#define GET_SysTimeRtcConvertLocalToUtc(fl)  CAL_CMGETAPI( "SysTimeRtcConvertLocalToUtc" ) 
	#define CAL_SysTimeRtcConvertLocalToUtc pISysTimeRtc->ISysTimeRtcConvertLocalToUtc
	#define CHK_SysTimeRtcConvertLocalToUtc (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcConvertLocalToUtc  CAL_CMEXPAPI( "SysTimeRtcConvertLocalToUtc" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcConvertLocalToUtc  PFSYSTIMERTCCONVERTLOCALTOUTC pfSysTimeRtcConvertLocalToUtc;
	#define EXT_SysTimeRtcConvertLocalToUtc  extern PFSYSTIMERTCCONVERTLOCALTOUTC pfSysTimeRtcConvertLocalToUtc;
	#define GET_SysTimeRtcConvertLocalToUtc(fl)  s_pfCMGetAPI2( "SysTimeRtcConvertLocalToUtc", (RTS_VOID_FCTPTR *)&pfSysTimeRtcConvertLocalToUtc, (fl), 0, 0)
	#define CAL_SysTimeRtcConvertLocalToUtc  pfSysTimeRtcConvertLocalToUtc
	#define CHK_SysTimeRtcConvertLocalToUtc  (pfSysTimeRtcConvertLocalToUtc != NULL)
	#define EXP_SysTimeRtcConvertLocalToUtc  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertLocalToUtc", (RTS_UINTPTR)SysTimeRtcConvertLocalToUtc, 0, 0) 
#endif




/**
 * <description>Convert UTC time in seconds to a structure format</description>
 * <param name="ulTimestampUtc" type="IN">UTC time in seconds since 1.1.1970 00:00:00</param>
 * <param name="pDate" type="OUT">Pointer to structure format. See category for detailed information</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcConvertUtcToDate(RTS_UI32 ulTimestampUtc, RTS_SYSTIMEDATE *pDate);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCCONVERTUTCTODATE) (RTS_UI32 ulTimestampUtc, RTS_SYSTIMEDATE *pDate);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTUTCTODATE_NOTIMPLEMENTED)
	#define USE_SysTimeRtcConvertUtcToDate
	#define EXT_SysTimeRtcConvertUtcToDate
	#define GET_SysTimeRtcConvertUtcToDate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcConvertUtcToDate(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcConvertUtcToDate  FALSE
	#define EXP_SysTimeRtcConvertUtcToDate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcConvertUtcToDate
	#define EXT_SysTimeRtcConvertUtcToDate
	#define GET_SysTimeRtcConvertUtcToDate(fl)  CAL_CMGETAPI( "SysTimeRtcConvertUtcToDate" ) 
	#define CAL_SysTimeRtcConvertUtcToDate  SysTimeRtcConvertUtcToDate
	#define CHK_SysTimeRtcConvertUtcToDate  TRUE
	#define EXP_SysTimeRtcConvertUtcToDate  CAL_CMEXPAPI( "SysTimeRtcConvertUtcToDate" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcConvertUtcToDate
	#define EXT_SysTimeRtcConvertUtcToDate
	#define GET_SysTimeRtcConvertUtcToDate(fl)  CAL_CMGETAPI( "SysTimeRtcConvertUtcToDate" ) 
	#define CAL_SysTimeRtcConvertUtcToDate  SysTimeRtcConvertUtcToDate
	#define CHK_SysTimeRtcConvertUtcToDate  TRUE
	#define EXP_SysTimeRtcConvertUtcToDate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertUtcToDate", (RTS_UINTPTR)SysTimeRtcConvertUtcToDate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcConvertUtcToDate
	#define EXT_SysTimeRtcSysTimeRtcConvertUtcToDate
	#define GET_SysTimeRtcSysTimeRtcConvertUtcToDate  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcConvertUtcToDate pISysTimeRtc->ISysTimeRtcConvertUtcToDate
	#define CHK_SysTimeRtcSysTimeRtcConvertUtcToDate (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcConvertUtcToDate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcConvertUtcToDate
	#define EXT_SysTimeRtcConvertUtcToDate
	#define GET_SysTimeRtcConvertUtcToDate(fl)  CAL_CMGETAPI( "SysTimeRtcConvertUtcToDate" ) 
	#define CAL_SysTimeRtcConvertUtcToDate pISysTimeRtc->ISysTimeRtcConvertUtcToDate
	#define CHK_SysTimeRtcConvertUtcToDate (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcConvertUtcToDate  CAL_CMEXPAPI( "SysTimeRtcConvertUtcToDate" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcConvertUtcToDate  PFSYSTIMERTCCONVERTUTCTODATE pfSysTimeRtcConvertUtcToDate;
	#define EXT_SysTimeRtcConvertUtcToDate  extern PFSYSTIMERTCCONVERTUTCTODATE pfSysTimeRtcConvertUtcToDate;
	#define GET_SysTimeRtcConvertUtcToDate(fl)  s_pfCMGetAPI2( "SysTimeRtcConvertUtcToDate", (RTS_VOID_FCTPTR *)&pfSysTimeRtcConvertUtcToDate, (fl), 0, 0)
	#define CAL_SysTimeRtcConvertUtcToDate  pfSysTimeRtcConvertUtcToDate
	#define CHK_SysTimeRtcConvertUtcToDate  (pfSysTimeRtcConvertUtcToDate != NULL)
	#define EXP_SysTimeRtcConvertUtcToDate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertUtcToDate", (RTS_UINTPTR)SysTimeRtcConvertUtcToDate, 0, 0) 
#endif




/**
 * <description>Convert UTC in a structure format to time in seconds</description>
 * <param name="pDate" type="IN">Pointer to structure format. See category for detailed information</param>
 * <param name="pulTimestampUtc" type="OUT">Pointer to UTC time in seconds since 1.1.1970 00:00:00</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcConvertDateToUtc(RTS_SYSTIMEDATE *pDate, RTS_UI32 *pulTimestampUtc);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCCONVERTDATETOUTC) (RTS_SYSTIMEDATE *pDate, RTS_UI32 *pulTimestampUtc);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTDATETOUTC_NOTIMPLEMENTED)
	#define USE_SysTimeRtcConvertDateToUtc
	#define EXT_SysTimeRtcConvertDateToUtc
	#define GET_SysTimeRtcConvertDateToUtc(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcConvertDateToUtc(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcConvertDateToUtc  FALSE
	#define EXP_SysTimeRtcConvertDateToUtc  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcConvertDateToUtc
	#define EXT_SysTimeRtcConvertDateToUtc
	#define GET_SysTimeRtcConvertDateToUtc(fl)  CAL_CMGETAPI( "SysTimeRtcConvertDateToUtc" ) 
	#define CAL_SysTimeRtcConvertDateToUtc  SysTimeRtcConvertDateToUtc
	#define CHK_SysTimeRtcConvertDateToUtc  TRUE
	#define EXP_SysTimeRtcConvertDateToUtc  CAL_CMEXPAPI( "SysTimeRtcConvertDateToUtc" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcConvertDateToUtc
	#define EXT_SysTimeRtcConvertDateToUtc
	#define GET_SysTimeRtcConvertDateToUtc(fl)  CAL_CMGETAPI( "SysTimeRtcConvertDateToUtc" ) 
	#define CAL_SysTimeRtcConvertDateToUtc  SysTimeRtcConvertDateToUtc
	#define CHK_SysTimeRtcConvertDateToUtc  TRUE
	#define EXP_SysTimeRtcConvertDateToUtc  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertDateToUtc", (RTS_UINTPTR)SysTimeRtcConvertDateToUtc, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcConvertDateToUtc
	#define EXT_SysTimeRtcSysTimeRtcConvertDateToUtc
	#define GET_SysTimeRtcSysTimeRtcConvertDateToUtc  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcConvertDateToUtc pISysTimeRtc->ISysTimeRtcConvertDateToUtc
	#define CHK_SysTimeRtcSysTimeRtcConvertDateToUtc (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcConvertDateToUtc  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcConvertDateToUtc
	#define EXT_SysTimeRtcConvertDateToUtc
	#define GET_SysTimeRtcConvertDateToUtc(fl)  CAL_CMGETAPI( "SysTimeRtcConvertDateToUtc" ) 
	#define CAL_SysTimeRtcConvertDateToUtc pISysTimeRtc->ISysTimeRtcConvertDateToUtc
	#define CHK_SysTimeRtcConvertDateToUtc (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcConvertDateToUtc  CAL_CMEXPAPI( "SysTimeRtcConvertDateToUtc" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcConvertDateToUtc  PFSYSTIMERTCCONVERTDATETOUTC pfSysTimeRtcConvertDateToUtc;
	#define EXT_SysTimeRtcConvertDateToUtc  extern PFSYSTIMERTCCONVERTDATETOUTC pfSysTimeRtcConvertDateToUtc;
	#define GET_SysTimeRtcConvertDateToUtc(fl)  s_pfCMGetAPI2( "SysTimeRtcConvertDateToUtc", (RTS_VOID_FCTPTR *)&pfSysTimeRtcConvertDateToUtc, (fl), 0, 0)
	#define CAL_SysTimeRtcConvertDateToUtc  pfSysTimeRtcConvertDateToUtc
	#define CHK_SysTimeRtcConvertDateToUtc  (pfSysTimeRtcConvertDateToUtc != NULL)
	#define EXP_SysTimeRtcConvertDateToUtc  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertDateToUtc", (RTS_UINTPTR)SysTimeRtcConvertDateToUtc, 0, 0) 
#endif




/**
 * <description>
 * OPTIONAL INTERFACE FUNCTION
 * Control the Rtc and read out hardware status information
 * </description>
 * <param name="rtcControl" type="IN">Control key. See corresponding category for detailed information</param>
 * <param name="piControlResult" type="OUT">Pointer to result. See category Control status for detailed information</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">RTC control available</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">RTC control failed</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">RTC control not available on the target</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcControl(int iControlTag, int *piControlResult);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCCONTROL) (int iControlTag, int *piControlResult);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONTROL_NOTIMPLEMENTED)
	#define USE_SysTimeRtcControl
	#define EXT_SysTimeRtcControl
	#define GET_SysTimeRtcControl(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcControl(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcControl  FALSE
	#define EXP_SysTimeRtcControl  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcControl
	#define EXT_SysTimeRtcControl
	#define GET_SysTimeRtcControl(fl)  CAL_CMGETAPI( "SysTimeRtcControl" ) 
	#define CAL_SysTimeRtcControl  SysTimeRtcControl
	#define CHK_SysTimeRtcControl  TRUE
	#define EXP_SysTimeRtcControl  CAL_CMEXPAPI( "SysTimeRtcControl" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcControl
	#define EXT_SysTimeRtcControl
	#define GET_SysTimeRtcControl(fl)  CAL_CMGETAPI( "SysTimeRtcControl" ) 
	#define CAL_SysTimeRtcControl  SysTimeRtcControl
	#define CHK_SysTimeRtcControl  TRUE
	#define EXP_SysTimeRtcControl  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcControl", (RTS_UINTPTR)SysTimeRtcControl, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcControl
	#define EXT_SysTimeRtcSysTimeRtcControl
	#define GET_SysTimeRtcSysTimeRtcControl  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcControl pISysTimeRtc->ISysTimeRtcControl
	#define CHK_SysTimeRtcSysTimeRtcControl (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcControl  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcControl
	#define EXT_SysTimeRtcControl
	#define GET_SysTimeRtcControl(fl)  CAL_CMGETAPI( "SysTimeRtcControl" ) 
	#define CAL_SysTimeRtcControl pISysTimeRtc->ISysTimeRtcControl
	#define CHK_SysTimeRtcControl (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcControl  CAL_CMEXPAPI( "SysTimeRtcControl" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcControl  PFSYSTIMERTCCONTROL pfSysTimeRtcControl;
	#define EXT_SysTimeRtcControl  extern PFSYSTIMERTCCONTROL pfSysTimeRtcControl;
	#define GET_SysTimeRtcControl(fl)  s_pfCMGetAPI2( "SysTimeRtcControl", (RTS_VOID_FCTPTR *)&pfSysTimeRtcControl, (fl), 0, 0)
	#define CAL_SysTimeRtcControl  pfSysTimeRtcControl
	#define CHK_SysTimeRtcControl  (pfSysTimeRtcControl != NULL)
	#define EXP_SysTimeRtcControl  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcControl", (RTS_UINTPTR)SysTimeRtcControl, 0, 0) 
#endif






/**
 * <description>Returns the Rtc value (realtime clock). The value is the elapsed number of milliseconds since
 *	1.1.1970 00:00:00:000 UTC time.</description>
 * <param name="pTimestamp" type="OUT">Milliseconds since 1.1.1970 00:00:000 UTC</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcHighResGet(RTS_SYSTIME* pTimestampUtcHighRes);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCHIGHRESGET) (RTS_SYSTIME* pTimestampUtcHighRes);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCHIGHRESGET_NOTIMPLEMENTED)
	#define USE_SysTimeRtcHighResGet
	#define EXT_SysTimeRtcHighResGet
	#define GET_SysTimeRtcHighResGet(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcHighResGet(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcHighResGet  FALSE
	#define EXP_SysTimeRtcHighResGet  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcHighResGet
	#define EXT_SysTimeRtcHighResGet
	#define GET_SysTimeRtcHighResGet(fl)  CAL_CMGETAPI( "SysTimeRtcHighResGet" ) 
	#define CAL_SysTimeRtcHighResGet  SysTimeRtcHighResGet
	#define CHK_SysTimeRtcHighResGet  TRUE
	#define EXP_SysTimeRtcHighResGet  CAL_CMEXPAPI( "SysTimeRtcHighResGet" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcHighResGet
	#define EXT_SysTimeRtcHighResGet
	#define GET_SysTimeRtcHighResGet(fl)  CAL_CMGETAPI( "SysTimeRtcHighResGet" ) 
	#define CAL_SysTimeRtcHighResGet  SysTimeRtcHighResGet
	#define CHK_SysTimeRtcHighResGet  TRUE
	#define EXP_SysTimeRtcHighResGet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcHighResGet", (RTS_UINTPTR)SysTimeRtcHighResGet, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcHighResGet
	#define EXT_SysTimeRtcSysTimeRtcHighResGet
	#define GET_SysTimeRtcSysTimeRtcHighResGet  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcHighResGet pISysTimeRtc->ISysTimeRtcHighResGet
	#define CHK_SysTimeRtcSysTimeRtcHighResGet (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcHighResGet  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcHighResGet
	#define EXT_SysTimeRtcHighResGet
	#define GET_SysTimeRtcHighResGet(fl)  CAL_CMGETAPI( "SysTimeRtcHighResGet" ) 
	#define CAL_SysTimeRtcHighResGet pISysTimeRtc->ISysTimeRtcHighResGet
	#define CHK_SysTimeRtcHighResGet (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcHighResGet  CAL_CMEXPAPI( "SysTimeRtcHighResGet" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcHighResGet  PFSYSTIMERTCHIGHRESGET pfSysTimeRtcHighResGet;
	#define EXT_SysTimeRtcHighResGet  extern PFSYSTIMERTCHIGHRESGET pfSysTimeRtcHighResGet;
	#define GET_SysTimeRtcHighResGet(fl)  s_pfCMGetAPI2( "SysTimeRtcHighResGet", (RTS_VOID_FCTPTR *)&pfSysTimeRtcHighResGet, (fl), 0, 0)
	#define CAL_SysTimeRtcHighResGet  pfSysTimeRtcHighResGet
	#define CHK_SysTimeRtcHighResGet  (pfSysTimeRtcHighResGet != NULL)
	#define EXP_SysTimeRtcHighResGet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcHighResGet", (RTS_UINTPTR)SysTimeRtcHighResGet, 0, 0) 
#endif




/**
 * <description>
 * OPTIONAL INTERFACE FUNCTION
 * Set the Rtc value (realtime clock). The value is the elapsed number of milliseconds since
 *	1.1.1970 00:00:00:000 UTC time.
 * </description>
 * <param name="pTimestamp" type="OUT">Milliseconds since 1.1.1970 00:00:000 UTC</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Timestamp can be set</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Timestamp failed to set</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Set is not available on the target</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcHighResSet(RTS_SYSTIME* pTimestampUtcHighRes);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCHIGHRESSET) (RTS_SYSTIME* pTimestampUtcHighRes);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCHIGHRESSET_NOTIMPLEMENTED)
	#define USE_SysTimeRtcHighResSet
	#define EXT_SysTimeRtcHighResSet
	#define GET_SysTimeRtcHighResSet(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcHighResSet(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcHighResSet  FALSE
	#define EXP_SysTimeRtcHighResSet  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcHighResSet
	#define EXT_SysTimeRtcHighResSet
	#define GET_SysTimeRtcHighResSet(fl)  CAL_CMGETAPI( "SysTimeRtcHighResSet" ) 
	#define CAL_SysTimeRtcHighResSet  SysTimeRtcHighResSet
	#define CHK_SysTimeRtcHighResSet  TRUE
	#define EXP_SysTimeRtcHighResSet  CAL_CMEXPAPI( "SysTimeRtcHighResSet" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcHighResSet
	#define EXT_SysTimeRtcHighResSet
	#define GET_SysTimeRtcHighResSet(fl)  CAL_CMGETAPI( "SysTimeRtcHighResSet" ) 
	#define CAL_SysTimeRtcHighResSet  SysTimeRtcHighResSet
	#define CHK_SysTimeRtcHighResSet  TRUE
	#define EXP_SysTimeRtcHighResSet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcHighResSet", (RTS_UINTPTR)SysTimeRtcHighResSet, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcHighResSet
	#define EXT_SysTimeRtcSysTimeRtcHighResSet
	#define GET_SysTimeRtcSysTimeRtcHighResSet  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcHighResSet pISysTimeRtc->ISysTimeRtcHighResSet
	#define CHK_SysTimeRtcSysTimeRtcHighResSet (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcHighResSet  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcHighResSet
	#define EXT_SysTimeRtcHighResSet
	#define GET_SysTimeRtcHighResSet(fl)  CAL_CMGETAPI( "SysTimeRtcHighResSet" ) 
	#define CAL_SysTimeRtcHighResSet pISysTimeRtc->ISysTimeRtcHighResSet
	#define CHK_SysTimeRtcHighResSet (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcHighResSet  CAL_CMEXPAPI( "SysTimeRtcHighResSet" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcHighResSet  PFSYSTIMERTCHIGHRESSET pfSysTimeRtcHighResSet;
	#define EXT_SysTimeRtcHighResSet  extern PFSYSTIMERTCHIGHRESSET pfSysTimeRtcHighResSet;
	#define GET_SysTimeRtcHighResSet(fl)  s_pfCMGetAPI2( "SysTimeRtcHighResSet", (RTS_VOID_FCTPTR *)&pfSysTimeRtcHighResSet, (fl), 0, 0)
	#define CAL_SysTimeRtcHighResSet  pfSysTimeRtcHighResSet
	#define CHK_SysTimeRtcHighResSet  (pfSysTimeRtcHighResSet != NULL)
	#define EXP_SysTimeRtcHighResSet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcHighResSet", (RTS_UINTPTR)SysTimeRtcHighResSet, 0, 0) 
#endif




/**
 * <description>Convert a high resolution timestamp [UTC] to a structure format [UTC]</description>
 * <param name="pTimestamp" type="IN">Milliseconds since 1.1.1970 00:00:000 [UTC]</param>
 * <param name="pDate" type="OUT">Structure format [UTC]</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcConvertHighResToDate(RTS_SYSTIME *pTimestampUtcHighRes, RTS_SYSTIMEDATE *pDate);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCCONVERTHIGHRESTODATE) (RTS_SYSTIME *pTimestampUtcHighRes, RTS_SYSTIMEDATE *pDate);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTHIGHRESTODATE_NOTIMPLEMENTED)
	#define USE_SysTimeRtcConvertHighResToDate
	#define EXT_SysTimeRtcConvertHighResToDate
	#define GET_SysTimeRtcConvertHighResToDate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcConvertHighResToDate(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcConvertHighResToDate  FALSE
	#define EXP_SysTimeRtcConvertHighResToDate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcConvertHighResToDate
	#define EXT_SysTimeRtcConvertHighResToDate
	#define GET_SysTimeRtcConvertHighResToDate(fl)  CAL_CMGETAPI( "SysTimeRtcConvertHighResToDate" ) 
	#define CAL_SysTimeRtcConvertHighResToDate  SysTimeRtcConvertHighResToDate
	#define CHK_SysTimeRtcConvertHighResToDate  TRUE
	#define EXP_SysTimeRtcConvertHighResToDate  CAL_CMEXPAPI( "SysTimeRtcConvertHighResToDate" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcConvertHighResToDate
	#define EXT_SysTimeRtcConvertHighResToDate
	#define GET_SysTimeRtcConvertHighResToDate(fl)  CAL_CMGETAPI( "SysTimeRtcConvertHighResToDate" ) 
	#define CAL_SysTimeRtcConvertHighResToDate  SysTimeRtcConvertHighResToDate
	#define CHK_SysTimeRtcConvertHighResToDate  TRUE
	#define EXP_SysTimeRtcConvertHighResToDate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertHighResToDate", (RTS_UINTPTR)SysTimeRtcConvertHighResToDate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcConvertHighResToDate
	#define EXT_SysTimeRtcSysTimeRtcConvertHighResToDate
	#define GET_SysTimeRtcSysTimeRtcConvertHighResToDate  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcConvertHighResToDate pISysTimeRtc->ISysTimeRtcConvertHighResToDate
	#define CHK_SysTimeRtcSysTimeRtcConvertHighResToDate (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcConvertHighResToDate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcConvertHighResToDate
	#define EXT_SysTimeRtcConvertHighResToDate
	#define GET_SysTimeRtcConvertHighResToDate(fl)  CAL_CMGETAPI( "SysTimeRtcConvertHighResToDate" ) 
	#define CAL_SysTimeRtcConvertHighResToDate pISysTimeRtc->ISysTimeRtcConvertHighResToDate
	#define CHK_SysTimeRtcConvertHighResToDate (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcConvertHighResToDate  CAL_CMEXPAPI( "SysTimeRtcConvertHighResToDate" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcConvertHighResToDate  PFSYSTIMERTCCONVERTHIGHRESTODATE pfSysTimeRtcConvertHighResToDate;
	#define EXT_SysTimeRtcConvertHighResToDate  extern PFSYSTIMERTCCONVERTHIGHRESTODATE pfSysTimeRtcConvertHighResToDate;
	#define GET_SysTimeRtcConvertHighResToDate(fl)  s_pfCMGetAPI2( "SysTimeRtcConvertHighResToDate", (RTS_VOID_FCTPTR *)&pfSysTimeRtcConvertHighResToDate, (fl), 0, 0)
	#define CAL_SysTimeRtcConvertHighResToDate  pfSysTimeRtcConvertHighResToDate
	#define CHK_SysTimeRtcConvertHighResToDate  (pfSysTimeRtcConvertHighResToDate != NULL)
	#define EXP_SysTimeRtcConvertHighResToDate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertHighResToDate", (RTS_UINTPTR)SysTimeRtcConvertHighResToDate, 0, 0) 
#endif




/**
 * <description>Convert structure format [UTC] to a high resolution timestamp [UTC]</description>
 * <param name="pDate" type="IN">Structure format [UTC]</param>
 * <param name="pTimestampUtcHighRes" type="OUT">Milliseconds since 1.1.1970 00:00:000 [UTC]</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcConvertDateToHighRes(RTS_SYSTIMEDATE *pDate, RTS_SYSTIME *pTimestampUtcHighRes);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCCONVERTDATETOHIGHRES) (RTS_SYSTIMEDATE *pDate, RTS_SYSTIME *pTimestampUtcHighRes);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTDATETOHIGHRES_NOTIMPLEMENTED)
	#define USE_SysTimeRtcConvertDateToHighRes
	#define EXT_SysTimeRtcConvertDateToHighRes
	#define GET_SysTimeRtcConvertDateToHighRes(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcConvertDateToHighRes(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcConvertDateToHighRes  FALSE
	#define EXP_SysTimeRtcConvertDateToHighRes  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcConvertDateToHighRes
	#define EXT_SysTimeRtcConvertDateToHighRes
	#define GET_SysTimeRtcConvertDateToHighRes(fl)  CAL_CMGETAPI( "SysTimeRtcConvertDateToHighRes" ) 
	#define CAL_SysTimeRtcConvertDateToHighRes  SysTimeRtcConvertDateToHighRes
	#define CHK_SysTimeRtcConvertDateToHighRes  TRUE
	#define EXP_SysTimeRtcConvertDateToHighRes  CAL_CMEXPAPI( "SysTimeRtcConvertDateToHighRes" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcConvertDateToHighRes
	#define EXT_SysTimeRtcConvertDateToHighRes
	#define GET_SysTimeRtcConvertDateToHighRes(fl)  CAL_CMGETAPI( "SysTimeRtcConvertDateToHighRes" ) 
	#define CAL_SysTimeRtcConvertDateToHighRes  SysTimeRtcConvertDateToHighRes
	#define CHK_SysTimeRtcConvertDateToHighRes  TRUE
	#define EXP_SysTimeRtcConvertDateToHighRes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertDateToHighRes", (RTS_UINTPTR)SysTimeRtcConvertDateToHighRes, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcConvertDateToHighRes
	#define EXT_SysTimeRtcSysTimeRtcConvertDateToHighRes
	#define GET_SysTimeRtcSysTimeRtcConvertDateToHighRes  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcConvertDateToHighRes pISysTimeRtc->ISysTimeRtcConvertDateToHighRes
	#define CHK_SysTimeRtcSysTimeRtcConvertDateToHighRes (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcConvertDateToHighRes  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcConvertDateToHighRes
	#define EXT_SysTimeRtcConvertDateToHighRes
	#define GET_SysTimeRtcConvertDateToHighRes(fl)  CAL_CMGETAPI( "SysTimeRtcConvertDateToHighRes" ) 
	#define CAL_SysTimeRtcConvertDateToHighRes pISysTimeRtc->ISysTimeRtcConvertDateToHighRes
	#define CHK_SysTimeRtcConvertDateToHighRes (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcConvertDateToHighRes  CAL_CMEXPAPI( "SysTimeRtcConvertDateToHighRes" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcConvertDateToHighRes  PFSYSTIMERTCCONVERTDATETOHIGHRES pfSysTimeRtcConvertDateToHighRes;
	#define EXT_SysTimeRtcConvertDateToHighRes  extern PFSYSTIMERTCCONVERTDATETOHIGHRES pfSysTimeRtcConvertDateToHighRes;
	#define GET_SysTimeRtcConvertDateToHighRes(fl)  s_pfCMGetAPI2( "SysTimeRtcConvertDateToHighRes", (RTS_VOID_FCTPTR *)&pfSysTimeRtcConvertDateToHighRes, (fl), 0, 0)
	#define CAL_SysTimeRtcConvertDateToHighRes  pfSysTimeRtcConvertDateToHighRes
	#define CHK_SysTimeRtcConvertDateToHighRes  (pfSysTimeRtcConvertDateToHighRes != NULL)
	#define EXP_SysTimeRtcConvertDateToHighRes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertDateToHighRes", (RTS_UINTPTR)SysTimeRtcConvertDateToHighRes, 0, 0) 
#endif




/**
 * <description>
 * OPTIONAL INTERFACE FUNCTION
 * Convert a high resolution timestamp [UTC ] to a structure format [Localtime].
 * </description>
 * <param name="pTimestampUtcHighRes" type="IN">Milliseconds since 1.1.1970 00:00:000 [UTC]</param>
 * <param name="pDate" type="OUT">Structure format [Localtime]</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Timestamp can be converted</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Timestamp failed to convert</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Conversion is not available on the target</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcConvertHighResToLocal(RTS_SYSTIME *pTimestampUtcHighRes, RTS_SYSTIMEDATE *pDate);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCCONVERTHIGHRESTOLOCAL) (RTS_SYSTIME *pTimestampUtcHighRes, RTS_SYSTIMEDATE *pDate);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTHIGHRESTOLOCAL_NOTIMPLEMENTED)
	#define USE_SysTimeRtcConvertHighResToLocal
	#define EXT_SysTimeRtcConvertHighResToLocal
	#define GET_SysTimeRtcConvertHighResToLocal(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcConvertHighResToLocal(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcConvertHighResToLocal  FALSE
	#define EXP_SysTimeRtcConvertHighResToLocal  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcConvertHighResToLocal
	#define EXT_SysTimeRtcConvertHighResToLocal
	#define GET_SysTimeRtcConvertHighResToLocal(fl)  CAL_CMGETAPI( "SysTimeRtcConvertHighResToLocal" ) 
	#define CAL_SysTimeRtcConvertHighResToLocal  SysTimeRtcConvertHighResToLocal
	#define CHK_SysTimeRtcConvertHighResToLocal  TRUE
	#define EXP_SysTimeRtcConvertHighResToLocal  CAL_CMEXPAPI( "SysTimeRtcConvertHighResToLocal" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcConvertHighResToLocal
	#define EXT_SysTimeRtcConvertHighResToLocal
	#define GET_SysTimeRtcConvertHighResToLocal(fl)  CAL_CMGETAPI( "SysTimeRtcConvertHighResToLocal" ) 
	#define CAL_SysTimeRtcConvertHighResToLocal  SysTimeRtcConvertHighResToLocal
	#define CHK_SysTimeRtcConvertHighResToLocal  TRUE
	#define EXP_SysTimeRtcConvertHighResToLocal  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertHighResToLocal", (RTS_UINTPTR)SysTimeRtcConvertHighResToLocal, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcConvertHighResToLocal
	#define EXT_SysTimeRtcSysTimeRtcConvertHighResToLocal
	#define GET_SysTimeRtcSysTimeRtcConvertHighResToLocal  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcConvertHighResToLocal pISysTimeRtc->ISysTimeRtcConvertHighResToLocal
	#define CHK_SysTimeRtcSysTimeRtcConvertHighResToLocal (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcConvertHighResToLocal  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcConvertHighResToLocal
	#define EXT_SysTimeRtcConvertHighResToLocal
	#define GET_SysTimeRtcConvertHighResToLocal(fl)  CAL_CMGETAPI( "SysTimeRtcConvertHighResToLocal" ) 
	#define CAL_SysTimeRtcConvertHighResToLocal pISysTimeRtc->ISysTimeRtcConvertHighResToLocal
	#define CHK_SysTimeRtcConvertHighResToLocal (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcConvertHighResToLocal  CAL_CMEXPAPI( "SysTimeRtcConvertHighResToLocal" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcConvertHighResToLocal  PFSYSTIMERTCCONVERTHIGHRESTOLOCAL pfSysTimeRtcConvertHighResToLocal;
	#define EXT_SysTimeRtcConvertHighResToLocal  extern PFSYSTIMERTCCONVERTHIGHRESTOLOCAL pfSysTimeRtcConvertHighResToLocal;
	#define GET_SysTimeRtcConvertHighResToLocal(fl)  s_pfCMGetAPI2( "SysTimeRtcConvertHighResToLocal", (RTS_VOID_FCTPTR *)&pfSysTimeRtcConvertHighResToLocal, (fl), 0, 0)
	#define CAL_SysTimeRtcConvertHighResToLocal  pfSysTimeRtcConvertHighResToLocal
	#define CHK_SysTimeRtcConvertHighResToLocal  (pfSysTimeRtcConvertHighResToLocal != NULL)
	#define EXP_SysTimeRtcConvertHighResToLocal  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertHighResToLocal", (RTS_UINTPTR)SysTimeRtcConvertHighResToLocal, 0, 0) 
#endif




/**
 * <description>
 * OPTIONAL INTERFACE FUNCTION
 * Convert a structure format [Localtime] to a high resolution timestamp [UTC ].
 * </description>
 * <param name="pDate" type="IN">Structure format [Localtime]</param>
 * <param name="pTimestampUtcHighRes" type="OUT">Milliseconds since 1.1.1970 00:00:000 [UTC]</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Date can be converted</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Date failed to convert</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Conversion is not available on the target</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTimeRtcConvertLocalToHighRes(RTS_SYSTIMEDATE *pDate, RTS_SYSTIME *pTimestampUtcHighRes);
typedef RTS_RESULT (CDECL * PFSYSTIMERTCCONVERTLOCALTOHIGHRES) (RTS_SYSTIMEDATE *pDate, RTS_SYSTIME *pTimestampUtcHighRes);
#if defined(SYSTIMERTC_NOTIMPLEMENTED) || defined(SYSTIMERTCCONVERTLOCALTOHIGHRES_NOTIMPLEMENTED)
	#define USE_SysTimeRtcConvertLocalToHighRes
	#define EXT_SysTimeRtcConvertLocalToHighRes
	#define GET_SysTimeRtcConvertLocalToHighRes(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTimeRtcConvertLocalToHighRes(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTimeRtcConvertLocalToHighRes  FALSE
	#define EXP_SysTimeRtcConvertLocalToHighRes  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTimeRtcConvertLocalToHighRes
	#define EXT_SysTimeRtcConvertLocalToHighRes
	#define GET_SysTimeRtcConvertLocalToHighRes(fl)  CAL_CMGETAPI( "SysTimeRtcConvertLocalToHighRes" ) 
	#define CAL_SysTimeRtcConvertLocalToHighRes  SysTimeRtcConvertLocalToHighRes
	#define CHK_SysTimeRtcConvertLocalToHighRes  TRUE
	#define EXP_SysTimeRtcConvertLocalToHighRes  CAL_CMEXPAPI( "SysTimeRtcConvertLocalToHighRes" ) 
#elif defined(MIXED_LINK) && !defined(SYSTIMERTC_EXTERNAL)
	#define USE_SysTimeRtcConvertLocalToHighRes
	#define EXT_SysTimeRtcConvertLocalToHighRes
	#define GET_SysTimeRtcConvertLocalToHighRes(fl)  CAL_CMGETAPI( "SysTimeRtcConvertLocalToHighRes" ) 
	#define CAL_SysTimeRtcConvertLocalToHighRes  SysTimeRtcConvertLocalToHighRes
	#define CHK_SysTimeRtcConvertLocalToHighRes  TRUE
	#define EXP_SysTimeRtcConvertLocalToHighRes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertLocalToHighRes", (RTS_UINTPTR)SysTimeRtcConvertLocalToHighRes, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTimeRtcSysTimeRtcConvertLocalToHighRes
	#define EXT_SysTimeRtcSysTimeRtcConvertLocalToHighRes
	#define GET_SysTimeRtcSysTimeRtcConvertLocalToHighRes  ERR_OK
	#define CAL_SysTimeRtcSysTimeRtcConvertLocalToHighRes pISysTimeRtc->ISysTimeRtcConvertLocalToHighRes
	#define CHK_SysTimeRtcSysTimeRtcConvertLocalToHighRes (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcSysTimeRtcConvertLocalToHighRes  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTimeRtcConvertLocalToHighRes
	#define EXT_SysTimeRtcConvertLocalToHighRes
	#define GET_SysTimeRtcConvertLocalToHighRes(fl)  CAL_CMGETAPI( "SysTimeRtcConvertLocalToHighRes" ) 
	#define CAL_SysTimeRtcConvertLocalToHighRes pISysTimeRtc->ISysTimeRtcConvertLocalToHighRes
	#define CHK_SysTimeRtcConvertLocalToHighRes (pISysTimeRtc != NULL)
	#define EXP_SysTimeRtcConvertLocalToHighRes  CAL_CMEXPAPI( "SysTimeRtcConvertLocalToHighRes" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTimeRtcConvertLocalToHighRes  PFSYSTIMERTCCONVERTLOCALTOHIGHRES pfSysTimeRtcConvertLocalToHighRes;
	#define EXT_SysTimeRtcConvertLocalToHighRes  extern PFSYSTIMERTCCONVERTLOCALTOHIGHRES pfSysTimeRtcConvertLocalToHighRes;
	#define GET_SysTimeRtcConvertLocalToHighRes(fl)  s_pfCMGetAPI2( "SysTimeRtcConvertLocalToHighRes", (RTS_VOID_FCTPTR *)&pfSysTimeRtcConvertLocalToHighRes, (fl), 0, 0)
	#define CAL_SysTimeRtcConvertLocalToHighRes  pfSysTimeRtcConvertLocalToHighRes
	#define CHK_SysTimeRtcConvertLocalToHighRes  (pfSysTimeRtcConvertLocalToHighRes != NULL)
	#define EXP_SysTimeRtcConvertLocalToHighRes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTimeRtcConvertLocalToHighRes", (RTS_UINTPTR)SysTimeRtcConvertLocalToHighRes, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSTIMERTCGET ISysTimeRtcGet;
 	PFSYSTIMERTCSET ISysTimeRtcSet;
 	PFSYSTIMERTCGETTIMEZONE ISysTimeRtcGetTimezone;
 	PFSYSTIMERTCSETTIMEZONE ISysTimeRtcSetTimezone;
 	PFSYSTIMERTCCONVERTUTCTOLOCAL ISysTimeRtcConvertUtcToLocal;
 	PFSYSTIMERTCCONVERTLOCALTOUTC ISysTimeRtcConvertLocalToUtc;
 	PFSYSTIMERTCCONVERTUTCTODATE ISysTimeRtcConvertUtcToDate;
 	PFSYSTIMERTCCONVERTDATETOUTC ISysTimeRtcConvertDateToUtc;
 	PFSYSTIMERTCCONTROL ISysTimeRtcControl;
 	PFSYSTIMERTCHIGHRESGET ISysTimeRtcHighResGet;
 	PFSYSTIMERTCHIGHRESSET ISysTimeRtcHighResSet;
 	PFSYSTIMERTCCONVERTHIGHRESTODATE ISysTimeRtcConvertHighResToDate;
 	PFSYSTIMERTCCONVERTDATETOHIGHRES ISysTimeRtcConvertDateToHighRes;
 	PFSYSTIMERTCCONVERTHIGHRESTOLOCAL ISysTimeRtcConvertHighResToLocal;
 	PFSYSTIMERTCCONVERTLOCALTOHIGHRES ISysTimeRtcConvertLocalToHighRes;
 } ISysTimeRtc_C;

#ifdef CPLUSPLUS
class ISysTimeRtc : public IBase
{
	public:
		virtual RTS_UI32 CDECL ISysTimeRtcGet(RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcSet(RTS_UI32 ulTimestampUtc) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcGetTimezone(TimezoneInformation *pTimezone) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcSetTimezone(TimezoneInformation *pTimezone) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcConvertUtcToLocal(RTS_UI32 ulTimestampUtc, RTS_UI32 *pulTimestampLocal) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcConvertLocalToUtc(RTS_UI32 ulTimestampLocal, RTS_UI32 *pulTimestampUtc) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcConvertUtcToDate(RTS_UI32 ulTimestampUtc, RTS_SYSTIMEDATE *pDate) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcConvertDateToUtc(RTS_SYSTIMEDATE *pDate, RTS_UI32 *pulTimestampUtc) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcControl(int iControlTag, int *piControlResult) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcHighResGet(RTS_SYSTIME* pTimestampUtcHighRes) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcHighResSet(RTS_SYSTIME* pTimestampUtcHighRes) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcConvertHighResToDate(RTS_SYSTIME *pTimestampUtcHighRes, RTS_SYSTIMEDATE *pDate) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcConvertDateToHighRes(RTS_SYSTIMEDATE *pDate, RTS_SYSTIME *pTimestampUtcHighRes) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcConvertHighResToLocal(RTS_SYSTIME *pTimestampUtcHighRes, RTS_SYSTIMEDATE *pDate) =0;
		virtual RTS_RESULT CDECL ISysTimeRtcConvertLocalToHighRes(RTS_SYSTIMEDATE *pDate, RTS_SYSTIME *pTimestampUtcHighRes) =0;
};
	#ifndef ITF_SysTimeRtc
		#define ITF_SysTimeRtc static ISysTimeRtc *pISysTimeRtc = NULL;
	#endif
	#define EXTITF_SysTimeRtc
#else	/*CPLUSPLUS*/
	typedef ISysTimeRtc_C		ISysTimeRtc;
	#ifndef ITF_SysTimeRtc
		#define ITF_SysTimeRtc
	#endif
	#define EXTITF_SysTimeRtc
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSTIMERTCITF_H_*/
