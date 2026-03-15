 /**
 * <interfacename>SysOut</interfacename>
 * <description> 
 *	<p>The SysOut interface is projected to get access to the console output routines. This
 *	can be used for debugging or logging needs.</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _SYSOUTITF_H_
#define _SYSOUTITF_H_

#include "CmpStd.h"

 

 




#include "CmpStd.h"

#ifndef CDECL
	#define CDECL
#endif


#ifdef __cplusplus
extern "C" {
#endif

/* Init routines for OS specific modules */
RTS_RESULT CDECL SysOutOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysOutOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>Function to print out a formatted string on the standard console output</description>
 * <param name="szFormat" type="IN">Format string with optional arguments</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysOutPrintf(char *szFormat, ...);
typedef RTS_RESULT (CDECL * PFSYSOUTPRINTF) (char *szFormat, ...);
#if defined(SYSOUT_NOTIMPLEMENTED) || defined(SYSOUTPRINTF_NOTIMPLEMENTED)
	#define USE_SysOutPrintf
	#define EXT_SysOutPrintf
	#define GET_SysOutPrintf(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysOutPrintf  FUNCTION_NOTIMPLEMENTED2 
	#define CHK_SysOutPrintf  FALSE
	#define EXP_SysOutPrintf  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysOutPrintf
	#define EXT_SysOutPrintf
	#define GET_SysOutPrintf(fl)  CAL_CMGETAPI( "SysOutPrintf" ) 
	#define CAL_SysOutPrintf  SysOutPrintf
	#define CHK_SysOutPrintf  TRUE
	#define EXP_SysOutPrintf  CAL_CMEXPAPI( "SysOutPrintf" ) 
#elif defined(MIXED_LINK) && !defined(SYSOUT_EXTERNAL)
	#define USE_SysOutPrintf
	#define EXT_SysOutPrintf
	#define GET_SysOutPrintf(fl)  CAL_CMGETAPI( "SysOutPrintf" ) 
	#define CAL_SysOutPrintf  SysOutPrintf
	#define CHK_SysOutPrintf  TRUE
	#define EXP_SysOutPrintf  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysOutPrintf", (RTS_UINTPTR)SysOutPrintf, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysOutSysOutPrintf
	#define EXT_SysOutSysOutPrintf
	#define GET_SysOutSysOutPrintf  ERR_OK
	#define CAL_SysOutSysOutPrintf pISysOut->ISysOutPrintf
	#define CHK_SysOutSysOutPrintf (pISysOut != NULL)
	#define EXP_SysOutSysOutPrintf  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysOutPrintf
	#define EXT_SysOutPrintf
	#define GET_SysOutPrintf(fl)  CAL_CMGETAPI( "SysOutPrintf" ) 
	#define CAL_SysOutPrintf pISysOut->ISysOutPrintf
	#define CHK_SysOutPrintf (pISysOut != NULL)
	#define EXP_SysOutPrintf  CAL_CMEXPAPI( "SysOutPrintf" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysOutPrintf  PFSYSOUTPRINTF pfSysOutPrintf;
	#define EXT_SysOutPrintf  extern PFSYSOUTPRINTF pfSysOutPrintf;
	#define GET_SysOutPrintf(fl)  s_pfCMGetAPI2( "SysOutPrintf", (RTS_VOID_FCTPTR *)&pfSysOutPrintf, (fl), 0, 0)
	#define CAL_SysOutPrintf  pfSysOutPrintf
	#define CHK_SysOutPrintf  (pfSysOutPrintf != NULL)
	#define EXP_SysOutPrintf  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysOutPrintf", (RTS_UINTPTR)SysOutPrintf, 0, 0) 
#endif




/**
 * <description>Function to print out an argument list on the standard console output</description>
 * <param name="szFormat" type="IN">Format string</param>
 * <param name="pargList" type="IN">Pointer to argument list for the format string</param>
 * <result>error code</result>
 */
#define SysOutPrintfArg		SysOutVPrintf
RTS_RESULT CDECL SysOutVPrintf(char *szFormat, va_list *pargList);
typedef RTS_RESULT (CDECL * PFSYSOUTVPRINTF) (char *szFormat, va_list *pargList);
#if defined(SYSOUT_NOTIMPLEMENTED) || defined(SYSOUTVPRINTF_NOTIMPLEMENTED)
	#define USE_SysOutVPrintf
	#define EXT_SysOutVPrintf
	#define GET_SysOutVPrintf(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysOutVPrintf(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysOutVPrintf  FALSE
	#define EXP_SysOutVPrintf  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysOutVPrintf
	#define EXT_SysOutVPrintf
	#define GET_SysOutVPrintf(fl)  CAL_CMGETAPI( "SysOutVPrintf" ) 
	#define CAL_SysOutVPrintf  SysOutVPrintf
	#define CHK_SysOutVPrintf  TRUE
	#define EXP_SysOutVPrintf  CAL_CMEXPAPI( "SysOutVPrintf" ) 
#elif defined(MIXED_LINK) && !defined(SYSOUT_EXTERNAL)
	#define USE_SysOutVPrintf
	#define EXT_SysOutVPrintf
	#define GET_SysOutVPrintf(fl)  CAL_CMGETAPI( "SysOutVPrintf" ) 
	#define CAL_SysOutVPrintf  SysOutVPrintf
	#define CHK_SysOutVPrintf  TRUE
	#define EXP_SysOutVPrintf  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysOutVPrintf", (RTS_UINTPTR)SysOutVPrintf, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysOutSysOutVPrintf
	#define EXT_SysOutSysOutVPrintf
	#define GET_SysOutSysOutVPrintf  ERR_OK
	#define CAL_SysOutSysOutVPrintf pISysOut->ISysOutVPrintf
	#define CHK_SysOutSysOutVPrintf (pISysOut != NULL)
	#define EXP_SysOutSysOutVPrintf  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysOutVPrintf
	#define EXT_SysOutVPrintf
	#define GET_SysOutVPrintf(fl)  CAL_CMGETAPI( "SysOutVPrintf" ) 
	#define CAL_SysOutVPrintf pISysOut->ISysOutVPrintf
	#define CHK_SysOutVPrintf (pISysOut != NULL)
	#define EXP_SysOutVPrintf  CAL_CMEXPAPI( "SysOutVPrintf" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysOutVPrintf  PFSYSOUTVPRINTF pfSysOutVPrintf;
	#define EXT_SysOutVPrintf  extern PFSYSOUTVPRINTF pfSysOutVPrintf;
	#define GET_SysOutVPrintf(fl)  s_pfCMGetAPI2( "SysOutVPrintf", (RTS_VOID_FCTPTR *)&pfSysOutVPrintf, (fl), 0, 0)
	#define CAL_SysOutVPrintf  pfSysOutVPrintf
	#define CHK_SysOutVPrintf  (pfSysOutVPrintf != NULL)
	#define EXP_SysOutVPrintf  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysOutVPrintf", (RTS_UINTPTR)SysOutVPrintf, 0, 0) 
#endif




/**
 * <description>
 *	Debug output fo a formatted string on the standard console output.
 *	NOTE: Actual time should be added in front of each debug output string
 * </description>
 * <param name="szFormat" type="IN">Format string with optional arguments</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysOutDebug(char *szFormat, ...);
typedef RTS_RESULT (CDECL * PFSYSOUTDEBUG) (char *szFormat, ...);
#if defined(SYSOUT_NOTIMPLEMENTED) || defined(SYSOUTDEBUG_NOTIMPLEMENTED)
	#define USE_SysOutDebug
	#define EXT_SysOutDebug
	#define GET_SysOutDebug(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysOutDebug  FUNCTION_NOTIMPLEMENTED2 
	#define CHK_SysOutDebug  FALSE
	#define EXP_SysOutDebug  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysOutDebug
	#define EXT_SysOutDebug
	#define GET_SysOutDebug(fl)  CAL_CMGETAPI( "SysOutDebug" ) 
	#define CAL_SysOutDebug  SysOutDebug
	#define CHK_SysOutDebug  TRUE
	#define EXP_SysOutDebug  CAL_CMEXPAPI( "SysOutDebug" ) 
#elif defined(MIXED_LINK) && !defined(SYSOUT_EXTERNAL)
	#define USE_SysOutDebug
	#define EXT_SysOutDebug
	#define GET_SysOutDebug(fl)  CAL_CMGETAPI( "SysOutDebug" ) 
	#define CAL_SysOutDebug  SysOutDebug
	#define CHK_SysOutDebug  TRUE
	#define EXP_SysOutDebug  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysOutDebug", (RTS_UINTPTR)SysOutDebug, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysOutSysOutDebug
	#define EXT_SysOutSysOutDebug
	#define GET_SysOutSysOutDebug  ERR_OK
	#define CAL_SysOutSysOutDebug pISysOut->ISysOutDebug
	#define CHK_SysOutSysOutDebug (pISysOut != NULL)
	#define EXP_SysOutSysOutDebug  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysOutDebug
	#define EXT_SysOutDebug
	#define GET_SysOutDebug(fl)  CAL_CMGETAPI( "SysOutDebug" ) 
	#define CAL_SysOutDebug pISysOut->ISysOutDebug
	#define CHK_SysOutDebug (pISysOut != NULL)
	#define EXP_SysOutDebug  CAL_CMEXPAPI( "SysOutDebug" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysOutDebug  PFSYSOUTDEBUG pfSysOutDebug;
	#define EXT_SysOutDebug  extern PFSYSOUTDEBUG pfSysOutDebug;
	#define GET_SysOutDebug(fl)  s_pfCMGetAPI2( "SysOutDebug", (RTS_VOID_FCTPTR *)&pfSysOutDebug, (fl), 0, 0)
	#define CAL_SysOutDebug  pfSysOutDebug
	#define CHK_SysOutDebug  (pfSysOutDebug != NULL)
	#define EXP_SysOutDebug  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysOutDebug", (RTS_UINTPTR)SysOutDebug, 0, 0) 
#endif




/**
 * <description>
 *	Debug output of an argument list on the standard console output.
 *	NOTE: Actual time should be added in front of each debug output string
 * </description>
 * <param name="szFormat" type="IN">Format string</param>
 * <param name="pargList" type="IN">Pointer to argument list for the format string</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysOutDebugArg(char *szFormat, va_list *pargList);
typedef RTS_RESULT (CDECL * PFSYSOUTDEBUGARG) (char *szFormat, va_list *pargList);
#if defined(SYSOUT_NOTIMPLEMENTED) || defined(SYSOUTDEBUGARG_NOTIMPLEMENTED)
	#define USE_SysOutDebugArg
	#define EXT_SysOutDebugArg
	#define GET_SysOutDebugArg(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysOutDebugArg(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysOutDebugArg  FALSE
	#define EXP_SysOutDebugArg  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysOutDebugArg
	#define EXT_SysOutDebugArg
	#define GET_SysOutDebugArg(fl)  CAL_CMGETAPI( "SysOutDebugArg" ) 
	#define CAL_SysOutDebugArg  SysOutDebugArg
	#define CHK_SysOutDebugArg  TRUE
	#define EXP_SysOutDebugArg  CAL_CMEXPAPI( "SysOutDebugArg" ) 
#elif defined(MIXED_LINK) && !defined(SYSOUT_EXTERNAL)
	#define USE_SysOutDebugArg
	#define EXT_SysOutDebugArg
	#define GET_SysOutDebugArg(fl)  CAL_CMGETAPI( "SysOutDebugArg" ) 
	#define CAL_SysOutDebugArg  SysOutDebugArg
	#define CHK_SysOutDebugArg  TRUE
	#define EXP_SysOutDebugArg  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysOutDebugArg", (RTS_UINTPTR)SysOutDebugArg, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysOutSysOutDebugArg
	#define EXT_SysOutSysOutDebugArg
	#define GET_SysOutSysOutDebugArg  ERR_OK
	#define CAL_SysOutSysOutDebugArg pISysOut->ISysOutDebugArg
	#define CHK_SysOutSysOutDebugArg (pISysOut != NULL)
	#define EXP_SysOutSysOutDebugArg  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysOutDebugArg
	#define EXT_SysOutDebugArg
	#define GET_SysOutDebugArg(fl)  CAL_CMGETAPI( "SysOutDebugArg" ) 
	#define CAL_SysOutDebugArg pISysOut->ISysOutDebugArg
	#define CHK_SysOutDebugArg (pISysOut != NULL)
	#define EXP_SysOutDebugArg  CAL_CMEXPAPI( "SysOutDebugArg" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysOutDebugArg  PFSYSOUTDEBUGARG pfSysOutDebugArg;
	#define EXT_SysOutDebugArg  extern PFSYSOUTDEBUGARG pfSysOutDebugArg;
	#define GET_SysOutDebugArg(fl)  s_pfCMGetAPI2( "SysOutDebugArg", (RTS_VOID_FCTPTR *)&pfSysOutDebugArg, (fl), 0, 0)
	#define CAL_SysOutDebugArg  pfSysOutDebugArg
	#define CHK_SysOutDebugArg  (pfSysOutDebugArg != NULL)
	#define EXP_SysOutDebugArg  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysOutDebugArg", (RTS_UINTPTR)SysOutDebugArg, 0, 0) 
#endif




/**
 * <description>Checkpoints for debugging needs. Prints the actual file and line number</description>
 */
#ifndef CPT
	#define CPT				SysOutDebug("%s checkpoint at line %d\n", __FILE__, __LINE__);
#endif
#ifndef CPTPRINTF
	#define CPTPRINTF		("%s checkpoint at line %d\n", __FILE__, __LINE__);
#endif

#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSOUTPRINTF ISysOutPrintf;
 	PFSYSOUTVPRINTF ISysOutVPrintf;
 	PFSYSOUTDEBUG ISysOutDebug;
 	PFSYSOUTDEBUGARG ISysOutDebugArg;
 } ISysOut_C;

#ifdef CPLUSPLUS
class ISysOut : public IBase
{
	public:
		virtual RTS_RESULT CDECL ISysOutPrintf(char *szFormat, ...) =0;
		virtual RTS_RESULT CDECL ISysOutVPrintf(char *szFormat, va_list *pargList) =0;
		virtual RTS_RESULT CDECL ISysOutDebug(char *szFormat, ...) =0;
		virtual RTS_RESULT CDECL ISysOutDebugArg(char *szFormat, va_list *pargList) =0;
};
	#ifndef ITF_SysOut
		#define ITF_SysOut static ISysOut *pISysOut = NULL;
	#endif
	#define EXTITF_SysOut
#else	/*CPLUSPLUS*/
	typedef ISysOut_C		ISysOut;
	#ifndef ITF_SysOut
		#define ITF_SysOut
	#endif
	#define EXTITF_SysOut
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSOUTITF_H_*/
