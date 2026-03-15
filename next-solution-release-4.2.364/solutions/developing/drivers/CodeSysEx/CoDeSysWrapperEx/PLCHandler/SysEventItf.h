 /**
 * <interfacename>SysEvent</interfacename>
 * <description> 
 *	<p>The SysEvent interface is projected to use an operating system event to activate a task. So this component
 *	can only be used on systems with tasks!</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _SYSEVENTITF_H_
#define _SYSEVENTITF_H_

#include "CmpStd.h"

 

 




#include "CmpStd.h"

/** EXTERN LIB SECTION BEGIN **/

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Create a new event object specified with name. Two components can open the same event, if they specify
 *	the same name.
 *	<p>IMPLEMENTATION NOTE:</p>
 *	<ul>
 *		<li>If a name is specified in szEvent, typically a system wide event is created.</li>
 *		<li>If an event still exists with this name, the routine returns the handle to the existing event</li>
 *		<li>szEvent can be NULL, so a new unique event with an empty name must be created</li>
 *		<li>If SysEventWait() is done after SysEventSet(), the event should signal the task!</li>
 *		<li>An event must not be used to signal several tasks!</li>
 *	</ul>
 *	<p>TARGET SPECIFIC IMPLEMENTATION:</p>
 *	<ul>
 *		<li>CoDeSys Control RTE:
 *			- If szEvent is specified, the event will work as a system wide Windows event. In this case, every call to SysEventSet/Wait will 
 *				call the corresponding Windows API-functions and this may last an unpredictable time!
 *			- In case SysEventCreate is called with a NULL-pointer in szEvent, the event can be used to synchronize RTE-tasks only. The calls
 *				to SysEventSet/Wait keep their real time capabilities.
 *		</li> 
 *  </ul>
 * </description>
 * <result><p>RESULT: Handle to the event or RTS_INVALID_HANDLE if failed.</p></result>
 */
typedef struct tagsyseventcreate_struct
{
	RTS_IEC_STRING *szEvent;			/* VAR_INPUT */	/* <param name="szEvent" type="IN">Name for the new event. Can be NULL!</param> */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
	RTS_IEC_HANDLE SysEventCreate;		/* VAR_OUTPUT */	
} syseventcreate_struct;

void CDECL CDECL_EXT syseventcreate(syseventcreate_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSEVENTCREATE_IEC) (syseventcreate_struct *p);
#if defined(SYSEVENT_NOTIMPLEMENTED) || defined(SYSEVENTCREATE_NOTIMPLEMENTED)
	#define USE_syseventcreate
	#define EXT_syseventcreate
	#define GET_syseventcreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syseventcreate(p0) 
	#define CHK_syseventcreate  FALSE
	#define EXP_syseventcreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syseventcreate
	#define EXT_syseventcreate
	#define GET_syseventcreate(fl)  CAL_CMGETAPI( "syseventcreate" ) 
	#define CAL_syseventcreate  syseventcreate
	#define CHK_syseventcreate  TRUE
	#define EXP_syseventcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventcreate", (RTS_UINTPTR)syseventcreate, 1, RTSITF_GET_SIGNATURE(0, 0x67712592), 0x03051100) 
	#define EXTREF_ITF_syseventcreate {(RTS_VOID_FCTPTR)syseventcreate, "syseventcreate", RTSITF_GET_SIGNATURE(0, 0x67712592), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSEVENT_EXTERNAL)
	#define USE_syseventcreate
	#define EXT_syseventcreate
	#define GET_syseventcreate(fl)  CAL_CMGETAPI( "syseventcreate" ) 
	#define CAL_syseventcreate  syseventcreate
	#define CHK_syseventcreate  TRUE
	#define EXP_syseventcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventcreate", (RTS_UINTPTR)syseventcreate, 1, RTSITF_GET_SIGNATURE(0, 0x67712592), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysEventsyseventcreate
	#define EXT_SysEventsyseventcreate
	#define GET_SysEventsyseventcreate  ERR_OK
	#define CAL_SysEventsyseventcreate  syseventcreate
	#define CHK_SysEventsyseventcreate  TRUE
	#define EXP_SysEventsyseventcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventcreate", (RTS_UINTPTR)syseventcreate, 1, RTSITF_GET_SIGNATURE(0, 0x67712592), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syseventcreate
	#define EXT_syseventcreate
	#define GET_syseventcreate(fl)  CAL_CMGETAPI( "syseventcreate" ) 
	#define CAL_syseventcreate  syseventcreate
	#define CHK_syseventcreate  TRUE
	#define EXP_syseventcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventcreate", (RTS_UINTPTR)syseventcreate, 1, RTSITF_GET_SIGNATURE(0, 0x67712592), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syseventcreate  PFSYSEVENTCREATE_IEC pfsyseventcreate;
	#define EXT_syseventcreate  extern PFSYSEVENTCREATE_IEC pfsyseventcreate;
	#define GET_syseventcreate(fl)  s_pfCMGetAPI2( "syseventcreate", (RTS_VOID_FCTPTR *)&pfsyseventcreate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x67712592), 0x03051100)
	#define CAL_syseventcreate  pfsyseventcreate
	#define CHK_syseventcreate  (pfsyseventcreate != NULL)
	#define EXP_syseventcreate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventcreate", (RTS_UINTPTR)syseventcreate, 1, RTSITF_GET_SIGNATURE(0, 0x67712592), 0x03051100) 
#endif


/**
 * <description>Delete an exisiting event object</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsyseventdelete_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* <param name="hEvent" type="IN">Handle of the event</param> */
	RTS_IEC_RESULT SysEventDelete;		/* VAR_OUTPUT */	
} syseventdelete_struct;

void CDECL CDECL_EXT syseventdelete(syseventdelete_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSEVENTDELETE_IEC) (syseventdelete_struct *p);
#if defined(SYSEVENT_NOTIMPLEMENTED) || defined(SYSEVENTDELETE_NOTIMPLEMENTED)
	#define USE_syseventdelete
	#define EXT_syseventdelete
	#define GET_syseventdelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syseventdelete(p0) 
	#define CHK_syseventdelete  FALSE
	#define EXP_syseventdelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syseventdelete
	#define EXT_syseventdelete
	#define GET_syseventdelete(fl)  CAL_CMGETAPI( "syseventdelete" ) 
	#define CAL_syseventdelete  syseventdelete
	#define CHK_syseventdelete  TRUE
	#define EXP_syseventdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventdelete", (RTS_UINTPTR)syseventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x97BA564C), 0x03051100) 
	#define EXTREF_ITF_syseventdelete {(RTS_VOID_FCTPTR)syseventdelete, "syseventdelete", RTSITF_GET_SIGNATURE(0, 0x97BA564C), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSEVENT_EXTERNAL)
	#define USE_syseventdelete
	#define EXT_syseventdelete
	#define GET_syseventdelete(fl)  CAL_CMGETAPI( "syseventdelete" ) 
	#define CAL_syseventdelete  syseventdelete
	#define CHK_syseventdelete  TRUE
	#define EXP_syseventdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventdelete", (RTS_UINTPTR)syseventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x97BA564C), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysEventsyseventdelete
	#define EXT_SysEventsyseventdelete
	#define GET_SysEventsyseventdelete  ERR_OK
	#define CAL_SysEventsyseventdelete  syseventdelete
	#define CHK_SysEventsyseventdelete  TRUE
	#define EXP_SysEventsyseventdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventdelete", (RTS_UINTPTR)syseventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x97BA564C), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syseventdelete
	#define EXT_syseventdelete
	#define GET_syseventdelete(fl)  CAL_CMGETAPI( "syseventdelete" ) 
	#define CAL_syseventdelete  syseventdelete
	#define CHK_syseventdelete  TRUE
	#define EXP_syseventdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventdelete", (RTS_UINTPTR)syseventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x97BA564C), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syseventdelete  PFSYSEVENTDELETE_IEC pfsyseventdelete;
	#define EXT_syseventdelete  extern PFSYSEVENTDELETE_IEC pfsyseventdelete;
	#define GET_syseventdelete(fl)  s_pfCMGetAPI2( "syseventdelete", (RTS_VOID_FCTPTR *)&pfsyseventdelete, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x97BA564C), 0x03051100)
	#define CAL_syseventdelete  pfsyseventdelete
	#define CHK_syseventdelete  (pfsyseventdelete != NULL)
	#define EXP_syseventdelete   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventdelete", (RTS_UINTPTR)syseventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x97BA564C), 0x03051100) 
#endif


/**
 * <description>Set the given Event. With this operation, a task is activated if it waits for the
 *	event with SysEventWait.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsyseventset_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* <param name="hEvent" type="IN">Handle of the event</param> */
	RTS_IEC_RESULT SysEventSet;			/* VAR_OUTPUT */	
} syseventset_struct;

void CDECL CDECL_EXT syseventset(syseventset_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSEVENTSET_IEC) (syseventset_struct *p);
#if defined(SYSEVENT_NOTIMPLEMENTED) || defined(SYSEVENTSET_NOTIMPLEMENTED)
	#define USE_syseventset
	#define EXT_syseventset
	#define GET_syseventset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syseventset(p0) 
	#define CHK_syseventset  FALSE
	#define EXP_syseventset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syseventset
	#define EXT_syseventset
	#define GET_syseventset(fl)  CAL_CMGETAPI( "syseventset" ) 
	#define CAL_syseventset  syseventset
	#define CHK_syseventset  TRUE
	#define EXP_syseventset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventset", (RTS_UINTPTR)syseventset, 1, RTSITF_GET_SIGNATURE(0, 0xBDDBC5FE), 0x03051100) 
	#define EXTREF_ITF_syseventset {(RTS_VOID_FCTPTR)syseventset, "syseventset", RTSITF_GET_SIGNATURE(0, 0xBDDBC5FE), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSEVENT_EXTERNAL)
	#define USE_syseventset
	#define EXT_syseventset
	#define GET_syseventset(fl)  CAL_CMGETAPI( "syseventset" ) 
	#define CAL_syseventset  syseventset
	#define CHK_syseventset  TRUE
	#define EXP_syseventset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventset", (RTS_UINTPTR)syseventset, 1, RTSITF_GET_SIGNATURE(0, 0xBDDBC5FE), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysEventsyseventset
	#define EXT_SysEventsyseventset
	#define GET_SysEventsyseventset  ERR_OK
	#define CAL_SysEventsyseventset  syseventset
	#define CHK_SysEventsyseventset  TRUE
	#define EXP_SysEventsyseventset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventset", (RTS_UINTPTR)syseventset, 1, RTSITF_GET_SIGNATURE(0, 0xBDDBC5FE), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syseventset
	#define EXT_syseventset
	#define GET_syseventset(fl)  CAL_CMGETAPI( "syseventset" ) 
	#define CAL_syseventset  syseventset
	#define CHK_syseventset  TRUE
	#define EXP_syseventset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventset", (RTS_UINTPTR)syseventset, 1, RTSITF_GET_SIGNATURE(0, 0xBDDBC5FE), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syseventset  PFSYSEVENTSET_IEC pfsyseventset;
	#define EXT_syseventset  extern PFSYSEVENTSET_IEC pfsyseventset;
	#define GET_syseventset(fl)  s_pfCMGetAPI2( "syseventset", (RTS_VOID_FCTPTR *)&pfsyseventset, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xBDDBC5FE), 0x03051100)
	#define CAL_syseventset  pfsyseventset
	#define CHK_syseventset  (pfsyseventset != NULL)
	#define EXP_syseventset   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventset", (RTS_UINTPTR)syseventset, 1, RTSITF_GET_SIGNATURE(0, 0xBDDBC5FE), 0x03051100) 
#endif


/**
 * <description>Wait for the given Event</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library):
 *	<ul>
 *		<li>ERR_OK: if event was received</li>
 *		<li>ERR_TIMEOUT: for timeout</li>
 *	</ul>
 * </p></result>
 */
typedef struct tagsyseventwait_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* <param name="hEvent" type="IN">Handle of the event</param> */
	RTS_IEC_UDINT ulTimeout;			/* VAR_INPUT */	/* <param name="lTimeout" type="IN">Timeout in ms to wait for the event:
	<ul>
		<li>ulTimeout = 16#FFFFFFFF: means infinite wait</li>
		<li>ulTimeout = 0: means no wait, only check the event!</li>
	</ul>
 </param> */
	RTS_IEC_RESULT SysEventWait;		/* VAR_OUTPUT */	
} syseventwait_struct;

void CDECL CDECL_EXT syseventwait(syseventwait_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSEVENTWAIT_IEC) (syseventwait_struct *p);
#if defined(SYSEVENT_NOTIMPLEMENTED) || defined(SYSEVENTWAIT_NOTIMPLEMENTED)
	#define USE_syseventwait
	#define EXT_syseventwait
	#define GET_syseventwait(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syseventwait(p0) 
	#define CHK_syseventwait  FALSE
	#define EXP_syseventwait  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syseventwait
	#define EXT_syseventwait
	#define GET_syseventwait(fl)  CAL_CMGETAPI( "syseventwait" ) 
	#define CAL_syseventwait  syseventwait
	#define CHK_syseventwait  TRUE
	#define EXP_syseventwait  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventwait", (RTS_UINTPTR)syseventwait, 1, RTSITF_GET_SIGNATURE(0, 0x34C44553), 0x03051100) 
	#define EXTREF_ITF_syseventwait {(RTS_VOID_FCTPTR)syseventwait, "syseventwait", RTSITF_GET_SIGNATURE(0, 0x34C44553), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSEVENT_EXTERNAL)
	#define USE_syseventwait
	#define EXT_syseventwait
	#define GET_syseventwait(fl)  CAL_CMGETAPI( "syseventwait" ) 
	#define CAL_syseventwait  syseventwait
	#define CHK_syseventwait  TRUE
	#define EXP_syseventwait  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventwait", (RTS_UINTPTR)syseventwait, 1, RTSITF_GET_SIGNATURE(0, 0x34C44553), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysEventsyseventwait
	#define EXT_SysEventsyseventwait
	#define GET_SysEventsyseventwait  ERR_OK
	#define CAL_SysEventsyseventwait  syseventwait
	#define CHK_SysEventsyseventwait  TRUE
	#define EXP_SysEventsyseventwait  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventwait", (RTS_UINTPTR)syseventwait, 1, RTSITF_GET_SIGNATURE(0, 0x34C44553), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syseventwait
	#define EXT_syseventwait
	#define GET_syseventwait(fl)  CAL_CMGETAPI( "syseventwait" ) 
	#define CAL_syseventwait  syseventwait
	#define CHK_syseventwait  TRUE
	#define EXP_syseventwait  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventwait", (RTS_UINTPTR)syseventwait, 1, RTSITF_GET_SIGNATURE(0, 0x34C44553), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syseventwait  PFSYSEVENTWAIT_IEC pfsyseventwait;
	#define EXT_syseventwait  extern PFSYSEVENTWAIT_IEC pfsyseventwait;
	#define GET_syseventwait(fl)  s_pfCMGetAPI2( "syseventwait", (RTS_VOID_FCTPTR *)&pfsyseventwait, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x34C44553), 0x03051100)
	#define CAL_syseventwait  pfsyseventwait
	#define CHK_syseventwait  (pfsyseventwait != NULL)
	#define EXP_syseventwait   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syseventwait", (RTS_UINTPTR)syseventwait, 1, RTSITF_GET_SIGNATURE(0, 0x34C44553), 0x03051100) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#define SYSEVENT_WAIT_INFINITE	((long)-1)

#ifdef __cplusplus
extern "C" {
#endif

/* Init routines for OS specific modules */
RTS_RESULT CDECL SysEventOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysEventOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>
 *	<p>Create a new event object specified with name. Two components can open the same event, if they specify
 *	the same name.</p>
 *	<p>IMPLEMENTATION NOTE:</p>
 *	<ul>
 *		<li>If a name is specified in pszName, typically a system wide event is created.</li>
 *		<li>If an event still exists with this name, the routine returns the handle to the existing event</li>
 *		<li>pszName can be NULL, so a new unique event with an empty name must be created</li>
 *		<li>If SysEventWait() is done after SysEventSet(), the event should signal the task!</li>
 *		<li>An event must not be used to signal several tasks!</li>
 *	</ul>
 *	<p>TARGET SPECIFIC IMPLEMENTATION:</p>
 *	<ul>
 *		<li>CoDeSys Control RTE:
 *			- If pszName is specified, the event will work as a system wide Windows event. In this case, every call to SysEventSet/Wait will 
 *				call the corresponding Windows API-functions and this may last an unpredictable time!
 *			- In case SysEventCreate is called with a NULL-pointer in pszName, the event can be used to synchronize RTE-tasks only. The calls
 *				to SysEventSet/Wait keep their real time capabilities.
 *		</li> 
 *  </ul>
 * </description>
 * <param name="pszName" type="IN">Name for the new event. Can be NULL!</param>
 * <param name="pResult" type="OUT">Pointer to error code </param>
 * <result>Handle to the event or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL SysEventCreate(char *pszName, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSEVENTCREATE) (char *pszName, RTS_RESULT *pResult);
#if defined(SYSEVENT_NOTIMPLEMENTED) || defined(SYSEVENTCREATE_NOTIMPLEMENTED)
	#define USE_SysEventCreate
	#define EXT_SysEventCreate
	#define GET_SysEventCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysEventCreate(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysEventCreate  FALSE
	#define EXP_SysEventCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysEventCreate
	#define EXT_SysEventCreate
	#define GET_SysEventCreate(fl)  CAL_CMGETAPI( "SysEventCreate" ) 
	#define CAL_SysEventCreate  SysEventCreate
	#define CHK_SysEventCreate  TRUE
	#define EXP_SysEventCreate  CAL_CMEXPAPI( "SysEventCreate" ) 
#elif defined(MIXED_LINK) && !defined(SYSEVENT_EXTERNAL)
	#define USE_SysEventCreate
	#define EXT_SysEventCreate
	#define GET_SysEventCreate(fl)  CAL_CMGETAPI( "SysEventCreate" ) 
	#define CAL_SysEventCreate  SysEventCreate
	#define CHK_SysEventCreate  TRUE
	#define EXP_SysEventCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysEventCreate", (RTS_UINTPTR)SysEventCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysEventSysEventCreate
	#define EXT_SysEventSysEventCreate
	#define GET_SysEventSysEventCreate  ERR_OK
	#define CAL_SysEventSysEventCreate  ((ISysEvent*)s_pfCMCreateInstance(CLASSID_CSysEvent, NULL))->ISysEventCreate
	#define CHK_SysEventSysEventCreate	(s_pfCMCreateInstance != NULL && pISysEvent != NULL)
	#define EXP_SysEventSysEventCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysEventCreate
	#define EXT_SysEventCreate
	#define GET_SysEventCreate(fl)  CAL_CMGETAPI( "SysEventCreate" ) 
	#define CAL_SysEventCreate  ((ISysEvent*)s_pfCMCreateInstance(CLASSID_CSysEvent, NULL))->ISysEventCreate
	#define CHK_SysEventCreate	(s_pfCMCreateInstance != NULL && pISysEvent != NULL)
	#define EXP_SysEventCreate  CAL_CMEXPAPI( "SysEventCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysEventCreate  PFSYSEVENTCREATE pfSysEventCreate;
	#define EXT_SysEventCreate  extern PFSYSEVENTCREATE pfSysEventCreate;
	#define GET_SysEventCreate(fl)  s_pfCMGetAPI2( "SysEventCreate", (RTS_VOID_FCTPTR *)&pfSysEventCreate, (fl), 0, 0)
	#define CAL_SysEventCreate  pfSysEventCreate
	#define CHK_SysEventCreate  (pfSysEventCreate != NULL)
	#define EXP_SysEventCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysEventCreate", (RTS_UINTPTR)SysEventCreate, 0, 0) 
#endif




/**
 * <description>Delete an existing event object</description>
 * <param name="hEvent" type="IN">Handle to the event. If hEvent is RTS_INVALID_HANDLE, 
 *	function returned with an error</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysEventDelete(RTS_HANDLE hEvent);
typedef RTS_RESULT (CDECL * PFSYSEVENTDELETE) (RTS_HANDLE hEvent);
#if defined(SYSEVENT_NOTIMPLEMENTED) || defined(SYSEVENTDELETE_NOTIMPLEMENTED)
	#define USE_SysEventDelete
	#define EXT_SysEventDelete
	#define GET_SysEventDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysEventDelete(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysEventDelete  FALSE
	#define EXP_SysEventDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysEventDelete
	#define EXT_SysEventDelete
	#define GET_SysEventDelete(fl)  CAL_CMGETAPI( "SysEventDelete" ) 
	#define CAL_SysEventDelete  SysEventDelete
	#define CHK_SysEventDelete  TRUE
	#define EXP_SysEventDelete  CAL_CMEXPAPI( "SysEventDelete" ) 
#elif defined(MIXED_LINK) && !defined(SYSEVENT_EXTERNAL)
	#define USE_SysEventDelete
	#define EXT_SysEventDelete
	#define GET_SysEventDelete(fl)  CAL_CMGETAPI( "SysEventDelete" ) 
	#define CAL_SysEventDelete  SysEventDelete
	#define CHK_SysEventDelete  TRUE
	#define EXP_SysEventDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysEventDelete", (RTS_UINTPTR)SysEventDelete, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysEventSysEventDelete
	#define EXT_SysEventSysEventDelete
	#define GET_SysEventSysEventDelete  ERR_OK
	#define CAL_SysEventSysEventDelete(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysEvent*)p0)->ISysEventDelete())
	#define CHK_SysEventSysEventDelete  TRUE
	#define EXP_SysEventSysEventDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysEventDelete
	#define EXT_SysEventDelete
	#define GET_SysEventDelete(fl)  CAL_CMGETAPI( "SysEventDelete" ) 
	#define CAL_SysEventDelete(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysEvent*)p0)->ISysEventDelete())
	#define CHK_SysEventDelete  TRUE
	#define EXP_SysEventDelete  CAL_CMEXPAPI( "SysEventDelete" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysEventDelete  PFSYSEVENTDELETE pfSysEventDelete;
	#define EXT_SysEventDelete  extern PFSYSEVENTDELETE pfSysEventDelete;
	#define GET_SysEventDelete(fl)  s_pfCMGetAPI2( "SysEventDelete", (RTS_VOID_FCTPTR *)&pfSysEventDelete, (fl), 0, 0)
	#define CAL_SysEventDelete  pfSysEventDelete
	#define CHK_SysEventDelete  (pfSysEventDelete != NULL)
	#define EXP_SysEventDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysEventDelete", (RTS_UINTPTR)SysEventDelete, 0, 0) 
#endif




/**
 * <description>Set the given Event. With this operation, a task is activated, if this task waits for the
 *	event (see SysEventWait).</description>
 * <param name="hEvent" type="IN">Handle to the event. If hEvent is RTS_INVALID_HANDLE, 
 *	function returned with an error</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysEventSet(RTS_HANDLE hEvent);
typedef RTS_RESULT (CDECL * PFSYSEVENTSET) (RTS_HANDLE hEvent);
#if defined(SYSEVENT_NOTIMPLEMENTED) || defined(SYSEVENTSET_NOTIMPLEMENTED)
	#define USE_SysEventSet
	#define EXT_SysEventSet
	#define GET_SysEventSet(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysEventSet(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysEventSet  FALSE
	#define EXP_SysEventSet  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysEventSet
	#define EXT_SysEventSet
	#define GET_SysEventSet(fl)  CAL_CMGETAPI( "SysEventSet" ) 
	#define CAL_SysEventSet  SysEventSet
	#define CHK_SysEventSet  TRUE
	#define EXP_SysEventSet  CAL_CMEXPAPI( "SysEventSet" ) 
#elif defined(MIXED_LINK) && !defined(SYSEVENT_EXTERNAL)
	#define USE_SysEventSet
	#define EXT_SysEventSet
	#define GET_SysEventSet(fl)  CAL_CMGETAPI( "SysEventSet" ) 
	#define CAL_SysEventSet  SysEventSet
	#define CHK_SysEventSet  TRUE
	#define EXP_SysEventSet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysEventSet", (RTS_UINTPTR)SysEventSet, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysEventSysEventSet
	#define EXT_SysEventSysEventSet
	#define GET_SysEventSysEventSet  ERR_OK
	#define CAL_SysEventSysEventSet(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysEvent->ISysEventSet() : ((ISysEvent*)p0)->ISysEventSet())
	#define CHK_SysEventSysEventSet  (pISysEvent != NULL)
	#define EXP_SysEventSysEventSet  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysEventSet
	#define EXT_SysEventSet
	#define GET_SysEventSet(fl)  CAL_CMGETAPI( "SysEventSet" ) 
	#define CAL_SysEventSet(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysEvent->ISysEventSet() : ((ISysEvent*)p0)->ISysEventSet())
	#define CHK_SysEventSet  (pISysEvent != NULL)
	#define EXP_SysEventSet  CAL_CMEXPAPI( "SysEventSet" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysEventSet  PFSYSEVENTSET pfSysEventSet;
	#define EXT_SysEventSet  extern PFSYSEVENTSET pfSysEventSet;
	#define GET_SysEventSet(fl)  s_pfCMGetAPI2( "SysEventSet", (RTS_VOID_FCTPTR *)&pfSysEventSet, (fl), 0, 0)
	#define CAL_SysEventSet  pfSysEventSet
	#define CHK_SysEventSet  (pfSysEventSet != NULL)
	#define EXP_SysEventSet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysEventSet", (RTS_UINTPTR)SysEventSet, 0, 0) 
#endif




/**
 * <description>Wait for the given Event</description>
 * <param name="hEvent" type="IN">Handle to the event. If hEvent is RTS_INVALID_HANDLE, 
 *	function returned with an error</param>
 * <param name="lTimeout" type="IN">Timeout in ms to wait for the event.
 *	<ul>
 *		<li>lTimeout = -1 (SYSEVENT_WAIT_INFINITE): means infinite wait</li>
 *		<li>lTimeout = 0: means no wait, only checks the event!</li>
 *	</ul>
 * </param>
 * <result>error code:
 * <ul>
 *		<li>ERR_OK: if event was received</li>
 *		<li>ERR_TIMEOUT: for timeout</li>
 * </ul>
 * </result>
 */
RTS_RESULT CDECL SysEventWait(RTS_HANDLE hEvent, long lTimeout);
typedef RTS_RESULT (CDECL * PFSYSEVENTWAIT) (RTS_HANDLE hEvent, long lTimeout);
#if defined(SYSEVENT_NOTIMPLEMENTED) || defined(SYSEVENTWAIT_NOTIMPLEMENTED)
	#define USE_SysEventWait
	#define EXT_SysEventWait
	#define GET_SysEventWait(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysEventWait(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysEventWait  FALSE
	#define EXP_SysEventWait  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysEventWait
	#define EXT_SysEventWait
	#define GET_SysEventWait(fl)  CAL_CMGETAPI( "SysEventWait" ) 
	#define CAL_SysEventWait  SysEventWait
	#define CHK_SysEventWait  TRUE
	#define EXP_SysEventWait  CAL_CMEXPAPI( "SysEventWait" ) 
#elif defined(MIXED_LINK) && !defined(SYSEVENT_EXTERNAL)
	#define USE_SysEventWait
	#define EXT_SysEventWait
	#define GET_SysEventWait(fl)  CAL_CMGETAPI( "SysEventWait" ) 
	#define CAL_SysEventWait  SysEventWait
	#define CHK_SysEventWait  TRUE
	#define EXP_SysEventWait  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysEventWait", (RTS_UINTPTR)SysEventWait, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysEventSysEventWait
	#define EXT_SysEventSysEventWait
	#define GET_SysEventSysEventWait  ERR_OK
	#define CAL_SysEventSysEventWait(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysEvent->ISysEventWait(p1) : ((ISysEvent*)p0)->ISysEventWait(p1))
	#define CHK_SysEventSysEventWait  (pISysEvent != NULL)
	#define EXP_SysEventSysEventWait  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysEventWait
	#define EXT_SysEventWait
	#define GET_SysEventWait(fl)  CAL_CMGETAPI( "SysEventWait" ) 
	#define CAL_SysEventWait(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysEvent->ISysEventWait(p1) : ((ISysEvent*)p0)->ISysEventWait(p1))
	#define CHK_SysEventWait  (pISysEvent != NULL)
	#define EXP_SysEventWait  CAL_CMEXPAPI( "SysEventWait" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysEventWait  PFSYSEVENTWAIT pfSysEventWait;
	#define EXT_SysEventWait  extern PFSYSEVENTWAIT pfSysEventWait;
	#define GET_SysEventWait(fl)  s_pfCMGetAPI2( "SysEventWait", (RTS_VOID_FCTPTR *)&pfSysEventWait, (fl), 0, 0)
	#define CAL_SysEventWait  pfSysEventWait
	#define CHK_SysEventWait  (pfSysEventWait != NULL)
	#define EXP_SysEventWait  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysEventWait", (RTS_UINTPTR)SysEventWait, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSEVENTCREATE ISysEventCreate;
 	PFSYSEVENTDELETE ISysEventDelete;
 	PFSYSEVENTSET ISysEventSet;
 	PFSYSEVENTWAIT ISysEventWait;
 } ISysEvent_C;

#ifdef CPLUSPLUS
class ISysEvent : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISysEventCreate(char *pszName, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysEventDelete(void) =0;
		virtual RTS_RESULT CDECL ISysEventSet(void) =0;
		virtual RTS_RESULT CDECL ISysEventWait(long lTimeout) =0;
};
	#ifndef ITF_SysEvent
		#define ITF_SysEvent static ISysEvent *pISysEvent = NULL;
	#endif
	#define EXTITF_SysEvent
#else	/*CPLUSPLUS*/
	typedef ISysEvent_C		ISysEvent;
	#ifndef ITF_SysEvent
		#define ITF_SysEvent
	#endif
	#define EXTITF_SysEvent
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSEVENTITF_H_*/
