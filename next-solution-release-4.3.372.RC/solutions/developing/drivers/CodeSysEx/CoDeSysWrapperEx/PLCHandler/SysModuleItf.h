 /**
 * <interfacename>SysModule</interfacename>
 * <description> 
 *	<p>The SysModule interface is projected to handle component modules. This interface is only
 *	available on systems, that supports dynamically loading and unloading modules.</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _SYSMODULEITF_H_
#define _SYSMODULEITF_H_

#include "CmpStd.h"

 

 




#include "CmpStd.h"

#ifndef CDECL
	#define CDECL
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* Init routines for OS specific modules */
RTS_RESULT CDECL SysModuleOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysModuleOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 * Setting to set VxWorks loadModule() options:
 * Proximity: loadModule( ) with default / proximity heap flag  
 * Common: loadModule( ) with common heap flag (only available for VxWorks 7)
 * </description>
 */
#define SYSMODULEKEY_STRING_VXWORKS_LOAD_MODULE_HEAP            "VxWorks.LoadModuleHeap"
#define SYSMODULEVALUE_STRING_VXWORKS_LOAD_MODULE_PROXIMITY     "Proximity"
#define SYSMODULEVALUE_STRING_VXWORKS_LOAD_MODULE_COMMON        "Common"
#ifndef SYSMODULEVALUE_STRING_VXWORKS_LOAD_MODULE_DEFAULT
  #define SYSMODULEVALUE_STRING_VXWORKS_LOAD_MODULE_DEFAULT     SYSMODULEVALUE_STRING_VXWORKS_LOAD_MODULE_PROXIMITY
#endif

/**
 * <description>
 *	Load a specified module
 * </description>
 * <param name="pszModuleName" type="IN">Pointer to the module name.
 *	NOTE: Module names MUST NOT include	operating system specific endings (e.g. .dll, .o, etc.)</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the loaded module or RTS_INVALID_HANDLE if failed.</result>
 */
RTS_HANDLE CDECL SysModuleLoad(char *pszModuleName, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSMODULELOAD) (char *pszModuleName, RTS_RESULT *pResult);
#if defined(SYSMODULE_NOTIMPLEMENTED) || defined(SYSMODULELOAD_NOTIMPLEMENTED)
	#define USE_SysModuleLoad
	#define EXT_SysModuleLoad
	#define GET_SysModuleLoad(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysModuleLoad(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysModuleLoad  FALSE
	#define EXP_SysModuleLoad  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysModuleLoad
	#define EXT_SysModuleLoad
	#define GET_SysModuleLoad(fl)  CAL_CMGETAPI( "SysModuleLoad" ) 
	#define CAL_SysModuleLoad  SysModuleLoad
	#define CHK_SysModuleLoad  TRUE
	#define EXP_SysModuleLoad  CAL_CMEXPAPI( "SysModuleLoad" ) 
#elif defined(MIXED_LINK) && !defined(SYSMODULE_EXTERNAL)
	#define USE_SysModuleLoad
	#define EXT_SysModuleLoad
	#define GET_SysModuleLoad(fl)  CAL_CMGETAPI( "SysModuleLoad" ) 
	#define CAL_SysModuleLoad  SysModuleLoad
	#define CHK_SysModuleLoad  TRUE
	#define EXP_SysModuleLoad  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysModuleLoad", (RTS_UINTPTR)SysModuleLoad, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysModuleSysModuleLoad
	#define EXT_SysModuleSysModuleLoad
	#define GET_SysModuleSysModuleLoad  ERR_OK
	#define CAL_SysModuleSysModuleLoad  ((ISysModule*)s_pfCMCreateInstance(CLASSID_CSysModule, NULL))->ISysModuleLoad
	#define CHK_SysModuleSysModuleLoad	(s_pfCMCreateInstance != NULL && pISysModule != NULL)
	#define EXP_SysModuleSysModuleLoad  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysModuleLoad
	#define EXT_SysModuleLoad
	#define GET_SysModuleLoad(fl)  CAL_CMGETAPI( "SysModuleLoad" ) 
	#define CAL_SysModuleLoad  ((ISysModule*)s_pfCMCreateInstance(CLASSID_CSysModule, NULL))->ISysModuleLoad
	#define CHK_SysModuleLoad	(s_pfCMCreateInstance != NULL && pISysModule != NULL)
	#define EXP_SysModuleLoad  CAL_CMEXPAPI( "SysModuleLoad" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysModuleLoad  PFSYSMODULELOAD pfSysModuleLoad;
	#define EXT_SysModuleLoad  extern PFSYSMODULELOAD pfSysModuleLoad;
	#define GET_SysModuleLoad(fl)  s_pfCMGetAPI2( "SysModuleLoad", (RTS_VOID_FCTPTR *)&pfSysModuleLoad, (fl), 0, 0)
	#define CAL_SysModuleLoad  pfSysModuleLoad
	#define CHK_SysModuleLoad  (pfSysModuleLoad != NULL)
	#define EXP_SysModuleLoad  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysModuleLoad", (RTS_UINTPTR)SysModuleLoad, 0, 0) 
#endif




/**
 * <description>
 * Load a specified module
 * </description>
 * <param name="pszModuleName" type="IN">Pointer to the module name.
 * NOTE:
 * Module names must _NOT_ include operating system specific endings (e.g. .dll, .o, etc.)</param>
 * <param name="pszModuleFileName" type="IN">Pointer to the module file name.
 * NOTE:
 * Module names _must_ include operating system specific endings (e.g. .dll, .o, etc.) and optional the path of the file!</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Module could be loaded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Failed to load module</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (pszModuleName = NULL/empty or pszModuleFileName = NULL/empty)</errorcode>
 * <result>Handle to the loaded module or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL SysModuleLoad2(char *pszModuleName, char *pszModuleFileName, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSMODULELOAD2) (char *pszModuleName, char *pszModuleFileName, RTS_RESULT *pResult);
#if defined(SYSMODULE_NOTIMPLEMENTED) || defined(SYSMODULELOAD2_NOTIMPLEMENTED)
	#define USE_SysModuleLoad2
	#define EXT_SysModuleLoad2
	#define GET_SysModuleLoad2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysModuleLoad2(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysModuleLoad2  FALSE
	#define EXP_SysModuleLoad2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysModuleLoad2
	#define EXT_SysModuleLoad2
	#define GET_SysModuleLoad2(fl)  CAL_CMGETAPI( "SysModuleLoad2" ) 
	#define CAL_SysModuleLoad2  SysModuleLoad2
	#define CHK_SysModuleLoad2  TRUE
	#define EXP_SysModuleLoad2  CAL_CMEXPAPI( "SysModuleLoad2" ) 
#elif defined(MIXED_LINK) && !defined(SYSMODULE_EXTERNAL)
	#define USE_SysModuleLoad2
	#define EXT_SysModuleLoad2
	#define GET_SysModuleLoad2(fl)  CAL_CMGETAPI( "SysModuleLoad2" ) 
	#define CAL_SysModuleLoad2  SysModuleLoad2
	#define CHK_SysModuleLoad2  TRUE
	#define EXP_SysModuleLoad2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysModuleLoad2", (RTS_UINTPTR)SysModuleLoad2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysModuleSysModuleLoad2
	#define EXT_SysModuleSysModuleLoad2
	#define GET_SysModuleSysModuleLoad2  ERR_OK
	#define CAL_SysModuleSysModuleLoad2  ((ISysModule*)s_pfCMCreateInstance(CLASSID_CSysModule, NULL))->ISysModuleLoad2
	#define CHK_SysModuleSysModuleLoad2	(s_pfCMCreateInstance != NULL && pISysModule != NULL)
	#define EXP_SysModuleSysModuleLoad2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysModuleLoad2
	#define EXT_SysModuleLoad2
	#define GET_SysModuleLoad2(fl)  CAL_CMGETAPI( "SysModuleLoad2" ) 
	#define CAL_SysModuleLoad2  ((ISysModule*)s_pfCMCreateInstance(CLASSID_CSysModule, NULL))->ISysModuleLoad2
	#define CHK_SysModuleLoad2	(s_pfCMCreateInstance != NULL && pISysModule != NULL)
	#define EXP_SysModuleLoad2  CAL_CMEXPAPI( "SysModuleLoad2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysModuleLoad2  PFSYSMODULELOAD2 pfSysModuleLoad2;
	#define EXT_SysModuleLoad2  extern PFSYSMODULELOAD2 pfSysModuleLoad2;
	#define GET_SysModuleLoad2(fl)  s_pfCMGetAPI2( "SysModuleLoad2", (RTS_VOID_FCTPTR *)&pfSysModuleLoad2, (fl), 0, 0)
	#define CAL_SysModuleLoad2  pfSysModuleLoad2
	#define CHK_SysModuleLoad2  (pfSysModuleLoad2 != NULL)
	#define EXP_SysModuleLoad2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysModuleLoad2", (RTS_UINTPTR)SysModuleLoad2, 0, 0) 
#endif


 

/**
 * <description>
 *	Unload a module
 * </description>
 * <param name="hModule" type="IN">Handle to the module</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysModuleUnload(RTS_HANDLE hModule);
typedef RTS_RESULT (CDECL * PFSYSMODULEUNLOAD) (RTS_HANDLE hModule);
#if defined(SYSMODULE_NOTIMPLEMENTED) || defined(SYSMODULEUNLOAD_NOTIMPLEMENTED)
	#define USE_SysModuleUnload
	#define EXT_SysModuleUnload
	#define GET_SysModuleUnload(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysModuleUnload(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysModuleUnload  FALSE
	#define EXP_SysModuleUnload  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysModuleUnload
	#define EXT_SysModuleUnload
	#define GET_SysModuleUnload(fl)  CAL_CMGETAPI( "SysModuleUnload" ) 
	#define CAL_SysModuleUnload  SysModuleUnload
	#define CHK_SysModuleUnload  TRUE
	#define EXP_SysModuleUnload  CAL_CMEXPAPI( "SysModuleUnload" ) 
#elif defined(MIXED_LINK) && !defined(SYSMODULE_EXTERNAL)
	#define USE_SysModuleUnload
	#define EXT_SysModuleUnload
	#define GET_SysModuleUnload(fl)  CAL_CMGETAPI( "SysModuleUnload" ) 
	#define CAL_SysModuleUnload  SysModuleUnload
	#define CHK_SysModuleUnload  TRUE
	#define EXP_SysModuleUnload  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysModuleUnload", (RTS_UINTPTR)SysModuleUnload, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysModuleSysModuleUnload
	#define EXT_SysModuleSysModuleUnload
	#define GET_SysModuleSysModuleUnload  ERR_OK
	#define CAL_SysModuleSysModuleUnload(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysModule*)p0)->ISysModuleUnload())
	#define CHK_SysModuleSysModuleUnload  TRUE
	#define EXP_SysModuleSysModuleUnload  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysModuleUnload
	#define EXT_SysModuleUnload
	#define GET_SysModuleUnload(fl)  CAL_CMGETAPI( "SysModuleUnload" ) 
	#define CAL_SysModuleUnload(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysModule*)p0)->ISysModuleUnload())
	#define CHK_SysModuleUnload  TRUE
	#define EXP_SysModuleUnload  CAL_CMEXPAPI( "SysModuleUnload" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysModuleUnload  PFSYSMODULEUNLOAD pfSysModuleUnload;
	#define EXT_SysModuleUnload  extern PFSYSMODULEUNLOAD pfSysModuleUnload;
	#define GET_SysModuleUnload(fl)  s_pfCMGetAPI2( "SysModuleUnload", (RTS_VOID_FCTPTR *)&pfSysModuleUnload, (fl), 0, 0)
	#define CAL_SysModuleUnload  pfSysModuleUnload
	#define CHK_SysModuleUnload  (pfSysModuleUnload != NULL)
	#define EXP_SysModuleUnload  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysModuleUnload", (RTS_UINTPTR)SysModuleUnload, 0, 0) 
#endif




/**
 * <description>
 *	Returns a function pointer of the routine in the module specified by name. 
 * </description>
 * <param name="hModule" type="IN">Handle to the module</param>
 * <param name="pszFctName" type="IN">Pointer to the function name</param>
 * <param name="ppFunction" type="OUT">Pointer to function pointer or NULL, if function is not available in the 
 *	specified module</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysModuleGetFunctionPointer(RTS_HANDLE hModule, char *pszFctName, void **ppFunction);
typedef RTS_RESULT (CDECL * PFSYSMODULEGETFUNCTIONPOINTER) (RTS_HANDLE hModule, char *pszFctName, void **ppFunction);
#if defined(SYSMODULE_NOTIMPLEMENTED) || defined(SYSMODULEGETFUNCTIONPOINTER_NOTIMPLEMENTED)
	#define USE_SysModuleGetFunctionPointer
	#define EXT_SysModuleGetFunctionPointer
	#define GET_SysModuleGetFunctionPointer(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysModuleGetFunctionPointer(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysModuleGetFunctionPointer  FALSE
	#define EXP_SysModuleGetFunctionPointer  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysModuleGetFunctionPointer
	#define EXT_SysModuleGetFunctionPointer
	#define GET_SysModuleGetFunctionPointer(fl)  CAL_CMGETAPI( "SysModuleGetFunctionPointer" ) 
	#define CAL_SysModuleGetFunctionPointer  SysModuleGetFunctionPointer
	#define CHK_SysModuleGetFunctionPointer  TRUE
	#define EXP_SysModuleGetFunctionPointer  CAL_CMEXPAPI( "SysModuleGetFunctionPointer" ) 
#elif defined(MIXED_LINK) && !defined(SYSMODULE_EXTERNAL)
	#define USE_SysModuleGetFunctionPointer
	#define EXT_SysModuleGetFunctionPointer
	#define GET_SysModuleGetFunctionPointer(fl)  CAL_CMGETAPI( "SysModuleGetFunctionPointer" ) 
	#define CAL_SysModuleGetFunctionPointer  SysModuleGetFunctionPointer
	#define CHK_SysModuleGetFunctionPointer  TRUE
	#define EXP_SysModuleGetFunctionPointer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysModuleGetFunctionPointer", (RTS_UINTPTR)SysModuleGetFunctionPointer, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysModuleSysModuleGetFunctionPointer
	#define EXT_SysModuleSysModuleGetFunctionPointer
	#define GET_SysModuleSysModuleGetFunctionPointer  ERR_OK
	#define CAL_SysModuleSysModuleGetFunctionPointer(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysModule->ISysModuleGetFunctionPointer(p1,p2) : ((ISysModule*)p0)->ISysModuleGetFunctionPointer(p1,p2))
	#define CHK_SysModuleSysModuleGetFunctionPointer  (pISysModule != NULL)
	#define EXP_SysModuleSysModuleGetFunctionPointer  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysModuleGetFunctionPointer
	#define EXT_SysModuleGetFunctionPointer
	#define GET_SysModuleGetFunctionPointer(fl)  CAL_CMGETAPI( "SysModuleGetFunctionPointer" ) 
	#define CAL_SysModuleGetFunctionPointer(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysModule->ISysModuleGetFunctionPointer(p1,p2) : ((ISysModule*)p0)->ISysModuleGetFunctionPointer(p1,p2))
	#define CHK_SysModuleGetFunctionPointer  (pISysModule != NULL)
	#define EXP_SysModuleGetFunctionPointer  CAL_CMEXPAPI( "SysModuleGetFunctionPointer" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysModuleGetFunctionPointer  PFSYSMODULEGETFUNCTIONPOINTER pfSysModuleGetFunctionPointer;
	#define EXT_SysModuleGetFunctionPointer  extern PFSYSMODULEGETFUNCTIONPOINTER pfSysModuleGetFunctionPointer;
	#define GET_SysModuleGetFunctionPointer(fl)  s_pfCMGetAPI2( "SysModuleGetFunctionPointer", (RTS_VOID_FCTPTR *)&pfSysModuleGetFunctionPointer, (fl), 0, 0)
	#define CAL_SysModuleGetFunctionPointer  pfSysModuleGetFunctionPointer
	#define CHK_SysModuleGetFunctionPointer  (pfSysModuleGetFunctionPointer != NULL)
	#define EXP_SysModuleGetFunctionPointer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysModuleGetFunctionPointer", (RTS_UINTPTR)SysModuleGetFunctionPointer, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSMODULELOAD ISysModuleLoad;
 	PFSYSMODULELOAD2 ISysModuleLoad2;
 	PFSYSMODULEUNLOAD ISysModuleUnload;
 	PFSYSMODULEGETFUNCTIONPOINTER ISysModuleGetFunctionPointer;
 } ISysModule_C;

#ifdef CPLUSPLUS
class ISysModule : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISysModuleLoad(char *pszModuleName, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysModuleLoad2(char *pszModuleName, char *pszModuleFileName, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysModuleUnload(void) =0;
		virtual RTS_RESULT CDECL ISysModuleGetFunctionPointer(char *pszFctName, void **ppFunction) =0;
};
	#ifndef ITF_SysModule
		#define ITF_SysModule static ISysModule *pISysModule = NULL;
	#endif
	#define EXTITF_SysModule
#else	/*CPLUSPLUS*/
	typedef ISysModule_C		ISysModule;
	#ifndef ITF_SysModule
		#define ITF_SysModule
	#endif
	#define EXTITF_SysModule
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSMODULEITF_H_*/
