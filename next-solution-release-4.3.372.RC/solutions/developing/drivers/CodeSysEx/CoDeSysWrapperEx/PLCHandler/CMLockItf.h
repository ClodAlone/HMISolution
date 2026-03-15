 /**
 * <interfacename>CMLock</interfacename>
 * <description>
 *	<p>Interface for a generic locking mechanism</p>
 *	<p>If both SysSem and SysInt components are implemented in the runtime system,
 *	a global int-lock implementation can be chosen with CMLOCK_PREFER_SYSINT macro.
 *	Otherwise, a mutex-based implementation is automatically selected.</p>
 *	<p>If either SysSem or SysInt component is implemented only, then
 *	the CMLock implementation is based on an existing component.</p>
 *  <pre>
 *	USAGE in a module:
 *
 *		RTS_HANDLE s_hLock = RTS_INVALID_HANDLE;
 *		CH_INIT:
 *			RTS_RESULT result;
 *			s_hLock = CAL_CMLockCreate(&amp;result);
 *		CH_EXIT:
 *			CAL_CMLockDelete(s_hLock);
 *	
 *		Function()
 *		{
 *			RTS_HANDLE hLock;
 *			CM_GET_LOCK(s_hLock, hLock);
 *		
 *			CAL_CMLockEnter(hLock);
 *			...
 *			CAL_CMLockLeave(hLock);
 *		}
 * </pre>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMLOCKITF_H_
#define _CMLOCKITF_H_

#include "CmpStd.h"

 

 




#ifdef __cplusplus
extern "C" {
#endif

#if !defined(SYSSEM_NOTIMPLEMENTED) && !defined(SYSINT_NOTIMPLEMENTED)
/* If SysSem and SysInt are both implemented, check if there is */
/* a preference to use SysInt */
#	ifndef CMLOCK_PREFER_SYSINT
/* SysInt is not explicitly defined, than we choose CMLOCK_PREFER_SYSSEM */
#		ifndef CMLOCK_PREFER_SYSSEM
#			define CMLOCK_PREFER_SYSSEM
#		endif
#	else
/* SysInt is explicitly defined, than we undef CMLOCK_PREFER_SYSSEM */
#		ifdef CMLOCK_PREFER_SYSSEM
#			undef CMLOCK_PREFER_SYSSEM
#		endif
#	endif
#elif defined(SYSINT_NOTIMPLEMENTED) 
/* SysInt is not implemented, than SysSem has to be used */
#	ifdef CMLOCK_PREFER_SYSINT
#		undef		CMLOCK_PREFER_SYSINT /* SysInt should NOT be used */
#	endif
#ifndef CMLOCK_PREFER_SYSSEM
#		define	CMLOCK_PREFER_SYSSEM	/* SysSem SHOULD be used */
#	endif
#else
/* SysSem is not implemented, than SysInt has to be used */
#	ifdef CMLOCK_PREFER_SYSSEM
#		undef		CMLOCK_PREFER_SYSSEM /* SysSem should NOT be used */
#	endif
#	ifndef CMLOCK_PREFER_SYSINT
#		define	CMLOCK_PREFER_SYSINT /* SysInt should be used */
#	endif
#endif

/**
 * <description> 
 *  Macro to get the correct parameter for the lock functions:
 *		CM_GET_LOCK(hLock, hLocalLock)
 * </description>
 * <param name="hLockObject" type="IN">is retrieved by CMLockCreate()</param>
 * <param name="hLock" type="IN">local variable of type RTS_HANDLE (declared in the local function!)</param>
 * <result>Error code</result>
 */
#if defined(CMLOCK_PREFER_SYSINT)
	#define CM_GET_LOCK(lockobject, lock)	 	RTS_UI32 cmlock;\
												lock = &cmlock;
#elif defined(CMLOCK_PREFER_SYSSEM)
	#define CM_GET_LOCK(lockobject, lock)	 	lock = lockobject;
#else
	#define CM_GET_LOCK(lockobject, lock)
#endif


/**
 * <description> 
 *  Function to create a lock object to synchronize data against concurrent task or interrupt access.
 * </description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the lock object</result>
 */
RTS_HANDLE CDECL CMLockCreate(RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFCMLOCKCREATE) (RTS_RESULT *pResult);
#if defined(CMLOCK_NOTIMPLEMENTED) || defined(CMLOCKCREATE_NOTIMPLEMENTED)
	#define USE_CMLockCreate
	#define EXT_CMLockCreate
	#define GET_CMLockCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMLockCreate(p0)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CMLockCreate  FALSE
	#define EXP_CMLockCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMLockCreate
	#define EXT_CMLockCreate
	#define GET_CMLockCreate(fl)  CAL_CMGETAPI( "CMLockCreate" ) 
	#define CAL_CMLockCreate  CMLockCreate
	#define CHK_CMLockCreate  TRUE
	#define EXP_CMLockCreate  CAL_CMEXPAPI( "CMLockCreate" ) 
#elif defined(MIXED_LINK) && !defined(CMLOCK_EXTERNAL)
	#define USE_CMLockCreate
	#define EXT_CMLockCreate
	#define GET_CMLockCreate(fl)  CAL_CMGETAPI( "CMLockCreate" ) 
	#define CAL_CMLockCreate  CMLockCreate
	#define CHK_CMLockCreate  TRUE
	#define EXP_CMLockCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLockCreate", (RTS_UINTPTR)CMLockCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMLockCMLockCreate
	#define EXT_CMLockCMLockCreate
	#define GET_CMLockCMLockCreate  ERR_OK
	#define CAL_CMLockCMLockCreate pICMLock->ICMLockCreate
	#define CHK_CMLockCMLockCreate (pICMLock != NULL)
	#define EXP_CMLockCMLockCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMLockCreate
	#define EXT_CMLockCreate
	#define GET_CMLockCreate(fl)  CAL_CMGETAPI( "CMLockCreate" ) 
	#define CAL_CMLockCreate pICMLock->ICMLockCreate
	#define CHK_CMLockCreate (pICMLock != NULL)
	#define EXP_CMLockCreate  CAL_CMEXPAPI( "CMLockCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMLockCreate  PFCMLOCKCREATE pfCMLockCreate;
	#define EXT_CMLockCreate  extern PFCMLOCKCREATE pfCMLockCreate;
	#define GET_CMLockCreate(fl)  s_pfCMGetAPI2( "CMLockCreate", (RTS_VOID_FCTPTR *)&pfCMLockCreate, (fl), 0, 0)
	#define CAL_CMLockCreate  pfCMLockCreate
	#define CHK_CMLockCreate  (pfCMLockCreate != NULL)
	#define EXP_CMLockCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLockCreate", (RTS_UINTPTR)CMLockCreate, 0, 0) 
#endif




/**
 * <description> 
 *  Function to delete a lock object.
 * </description>
 * <param name="hLock" type="IN">Handle to the lock object retrieved by CMLockCreate()</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL CMLockDelete(RTS_HANDLE hLock);
typedef RTS_RESULT (CDECL * PFCMLOCKDELETE) (RTS_HANDLE hLock);
#if defined(CMLOCK_NOTIMPLEMENTED) || defined(CMLOCKDELETE_NOTIMPLEMENTED)
	#define USE_CMLockDelete
	#define EXT_CMLockDelete
	#define GET_CMLockDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMLockDelete(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMLockDelete  FALSE
	#define EXP_CMLockDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMLockDelete
	#define EXT_CMLockDelete
	#define GET_CMLockDelete(fl)  CAL_CMGETAPI( "CMLockDelete" ) 
	#define CAL_CMLockDelete  CMLockDelete
	#define CHK_CMLockDelete  TRUE
	#define EXP_CMLockDelete  CAL_CMEXPAPI( "CMLockDelete" ) 
#elif defined(MIXED_LINK) && !defined(CMLOCK_EXTERNAL)
	#define USE_CMLockDelete
	#define EXT_CMLockDelete
	#define GET_CMLockDelete(fl)  CAL_CMGETAPI( "CMLockDelete" ) 
	#define CAL_CMLockDelete  CMLockDelete
	#define CHK_CMLockDelete  TRUE
	#define EXP_CMLockDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLockDelete", (RTS_UINTPTR)CMLockDelete, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMLockCMLockDelete
	#define EXT_CMLockCMLockDelete
	#define GET_CMLockCMLockDelete  ERR_OK
	#define CAL_CMLockCMLockDelete pICMLock->ICMLockDelete
	#define CHK_CMLockCMLockDelete (pICMLock != NULL)
	#define EXP_CMLockCMLockDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMLockDelete
	#define EXT_CMLockDelete
	#define GET_CMLockDelete(fl)  CAL_CMGETAPI( "CMLockDelete" ) 
	#define CAL_CMLockDelete pICMLock->ICMLockDelete
	#define CHK_CMLockDelete (pICMLock != NULL)
	#define EXP_CMLockDelete  CAL_CMEXPAPI( "CMLockDelete" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMLockDelete  PFCMLOCKDELETE pfCMLockDelete;
	#define EXT_CMLockDelete  extern PFCMLOCKDELETE pfCMLockDelete;
	#define GET_CMLockDelete(fl)  s_pfCMGetAPI2( "CMLockDelete", (RTS_VOID_FCTPTR *)&pfCMLockDelete, (fl), 0, 0)
	#define CAL_CMLockDelete  pfCMLockDelete
	#define CHK_CMLockDelete  (pfCMLockDelete != NULL)
	#define EXP_CMLockDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLockDelete", (RTS_UINTPTR)CMLockDelete, 0, 0) 
#endif




/**
 * <description> 
 *  Function to enter a lock object. After entering, the accessed data is _safe_ against concurrent
 *	access.
 * </description>
 * <param name="hLock" type="IN">Handle to the lock object retrieved by macro: CM_GET_LOCK(hLockObject, hLock)
 *					- hLockObject is retrieved by CMLockCreate()
 *					- hLock is a local variable of type RTS_HANDLE (declared in the local function!)</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL CMLockEnter(RTS_HANDLE hLock);
typedef RTS_RESULT (CDECL * PFCMLOCKENTER) (RTS_HANDLE hLock);
#if defined(CMLOCK_NOTIMPLEMENTED) || defined(CMLOCKENTER_NOTIMPLEMENTED)
	#define USE_CMLockEnter
	#define EXT_CMLockEnter
	#define GET_CMLockEnter(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMLockEnter(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMLockEnter  FALSE
	#define EXP_CMLockEnter  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMLockEnter
	#define EXT_CMLockEnter
	#define GET_CMLockEnter(fl)  CAL_CMGETAPI( "CMLockEnter" ) 
	#define CAL_CMLockEnter  CMLockEnter
	#define CHK_CMLockEnter  TRUE
	#define EXP_CMLockEnter  CAL_CMEXPAPI( "CMLockEnter" ) 
#elif defined(MIXED_LINK) && !defined(CMLOCK_EXTERNAL)
	#define USE_CMLockEnter
	#define EXT_CMLockEnter
	#define GET_CMLockEnter(fl)  CAL_CMGETAPI( "CMLockEnter" ) 
	#define CAL_CMLockEnter  CMLockEnter
	#define CHK_CMLockEnter  TRUE
	#define EXP_CMLockEnter  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLockEnter", (RTS_UINTPTR)CMLockEnter, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMLockCMLockEnter
	#define EXT_CMLockCMLockEnter
	#define GET_CMLockCMLockEnter  ERR_OK
	#define CAL_CMLockCMLockEnter pICMLock->ICMLockEnter
	#define CHK_CMLockCMLockEnter (pICMLock != NULL)
	#define EXP_CMLockCMLockEnter  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMLockEnter
	#define EXT_CMLockEnter
	#define GET_CMLockEnter(fl)  CAL_CMGETAPI( "CMLockEnter" ) 
	#define CAL_CMLockEnter pICMLock->ICMLockEnter
	#define CHK_CMLockEnter (pICMLock != NULL)
	#define EXP_CMLockEnter  CAL_CMEXPAPI( "CMLockEnter" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMLockEnter  PFCMLOCKENTER pfCMLockEnter;
	#define EXT_CMLockEnter  extern PFCMLOCKENTER pfCMLockEnter;
	#define GET_CMLockEnter(fl)  s_pfCMGetAPI2( "CMLockEnter", (RTS_VOID_FCTPTR *)&pfCMLockEnter, (fl), 0, 0)
	#define CAL_CMLockEnter  pfCMLockEnter
	#define CHK_CMLockEnter  (pfCMLockEnter != NULL)
	#define EXP_CMLockEnter  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLockEnter", (RTS_UINTPTR)CMLockEnter, 0, 0) 
#endif




/**
 * <description> 
 *  Function to leave a lock object. After leaving, the accessed data is _unsafe_ against concurrent
 *	access.
 * </description>
 * <param name="hLock" type="IN">Handle to the lock object retrieved by CMLockCreate()</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL CMLockLeave(RTS_HANDLE hLock);
typedef RTS_RESULT (CDECL * PFCMLOCKLEAVE) (RTS_HANDLE hLock);
#if defined(CMLOCK_NOTIMPLEMENTED) || defined(CMLOCKLEAVE_NOTIMPLEMENTED)
	#define USE_CMLockLeave
	#define EXT_CMLockLeave
	#define GET_CMLockLeave(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMLockLeave(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMLockLeave  FALSE
	#define EXP_CMLockLeave  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMLockLeave
	#define EXT_CMLockLeave
	#define GET_CMLockLeave(fl)  CAL_CMGETAPI( "CMLockLeave" ) 
	#define CAL_CMLockLeave  CMLockLeave
	#define CHK_CMLockLeave  TRUE
	#define EXP_CMLockLeave  CAL_CMEXPAPI( "CMLockLeave" ) 
#elif defined(MIXED_LINK) && !defined(CMLOCK_EXTERNAL)
	#define USE_CMLockLeave
	#define EXT_CMLockLeave
	#define GET_CMLockLeave(fl)  CAL_CMGETAPI( "CMLockLeave" ) 
	#define CAL_CMLockLeave  CMLockLeave
	#define CHK_CMLockLeave  TRUE
	#define EXP_CMLockLeave  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLockLeave", (RTS_UINTPTR)CMLockLeave, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMLockCMLockLeave
	#define EXT_CMLockCMLockLeave
	#define GET_CMLockCMLockLeave  ERR_OK
	#define CAL_CMLockCMLockLeave pICMLock->ICMLockLeave
	#define CHK_CMLockCMLockLeave (pICMLock != NULL)
	#define EXP_CMLockCMLockLeave  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMLockLeave
	#define EXT_CMLockLeave
	#define GET_CMLockLeave(fl)  CAL_CMGETAPI( "CMLockLeave" ) 
	#define CAL_CMLockLeave pICMLock->ICMLockLeave
	#define CHK_CMLockLeave (pICMLock != NULL)
	#define EXP_CMLockLeave  CAL_CMEXPAPI( "CMLockLeave" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMLockLeave  PFCMLOCKLEAVE pfCMLockLeave;
	#define EXT_CMLockLeave  extern PFCMLOCKLEAVE pfCMLockLeave;
	#define GET_CMLockLeave(fl)  s_pfCMGetAPI2( "CMLockLeave", (RTS_VOID_FCTPTR *)&pfCMLockLeave, (fl), 0, 0)
	#define CAL_CMLockLeave  pfCMLockLeave
	#define CHK_CMLockLeave  (pfCMLockLeave != NULL)
	#define EXP_CMLockLeave  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLockLeave", (RTS_UINTPTR)CMLockLeave, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFCMLOCKCREATE ICMLockCreate;
 	PFCMLOCKDELETE ICMLockDelete;
 	PFCMLOCKENTER ICMLockEnter;
 	PFCMLOCKLEAVE ICMLockLeave;
 } ICMLock_C;

#ifdef CPLUSPLUS
class ICMLock : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ICMLockCreate(RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ICMLockDelete(RTS_HANDLE hLock) =0;
		virtual RTS_RESULT CDECL ICMLockEnter(RTS_HANDLE hLock) =0;
		virtual RTS_RESULT CDECL ICMLockLeave(RTS_HANDLE hLock) =0;
};
	#ifndef ITF_CMLock
		#define ITF_CMLock static ICMLock *pICMLock = NULL;
	#endif
	#define EXTITF_CMLock
#else	/*CPLUSPLUS*/
	typedef ICMLock_C		ICMLock;
	#ifndef ITF_CMLock
		#define ITF_CMLock
	#endif
	#define EXTITF_CMLock
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMLOCKITF_H_*/
