 /**
 * <interfacename>CmpLogBackend</interfacename>
 * <description> 
 *	Interface of a logger back-end, to store and dump log entries.
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPLOGBACKENDITF_H_
#define _CMPLOGBACKENDITF_H_

#include "CmpStd.h"

 

 




#include <stdarg.h>

struct tagLogOptions;
struct tagLogEntry;


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description> Create a logger </description>
 * <param name="pOptions" type="IN">Options for logger</param>
 * <param name="pResult" type="OUT">Pointer to get the result</param>
 * <result>Handle to the logger, or RTS_INVALID_HANDLE if failed</result>
 */
STATICITF_DEF RTS_HANDLE CDECL LogBackendCreate(RTS_HANDLE hICmpLogBackend, CLASSID ClassId, struct tagLogOptions *pOptions);
typedef RTS_HANDLE (CDECL * PFLOGBACKENDCREATE) (RTS_HANDLE hICmpLogBackend, CLASSID ClassId, struct tagLogOptions *pOptions);
#if defined(CMPLOGBACKEND_NOTIMPLEMENTED) || defined(LOGBACKENDCREATE_NOTIMPLEMENTED)
	#define USE_LogBackendCreate
	#define EXT_LogBackendCreate
	#define GET_LogBackendCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogBackendCreate(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogBackendCreate  FALSE
	#define EXP_LogBackendCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogBackendCreate
	#define EXT_LogBackendCreate
	#define GET_LogBackendCreate(fl)  ERR_OK
	#define CAL_LogBackendCreate(p0,p1,p2)	\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									(((IBase *)p0)->ClassId = p1) == CLASSID_INVALID ? RTS_INVALID_HANDLE :\
									(\
										(((IBase *)p0)->hInstance = ((ICmpLogBackend*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ICmpLogBackend, NULL))->ILogBackendCreate(p0,p1,p2)) == RTS_INVALID_HANDLE ? (s_pfCMDeleteInstance2(p1, (IBase *)p0) ? RTS_INVALID_HANDLE : RTS_INVALID_HANDLE) : \
										(\
											((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ICmpLogBackend, NULL)\
										)\
									)\
								)\
							)
	#define CHK_LogBackendCreate  TRUE
	#define EXP_LogBackendCreate  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPLOGBACKEND_EXTERNAL)
	#define USE_LogBackendCreate
	#define EXT_LogBackendCreate
	#define GET_LogBackendCreate(fl)  ERR_OK
	#define CAL_LogBackendCreate(p0,p1,p2)		\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									(((IBase *)p0)->ClassId = p1) == CLASSID_INVALID ? RTS_INVALID_HANDLE :\
									(\
										(((IBase *)p0)->hInstance = ((ICmpLogBackend*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ICmpLogBackend, NULL))->ILogBackendCreate(p0,p1,p2)) == RTS_INVALID_HANDLE ? (s_pfCMDeleteInstance2(p1, (IBase *)p0) ? RTS_INVALID_HANDLE : RTS_INVALID_HANDLE) : \
										(\
											((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ICmpLogBackend, NULL)\
										)\
									)\
								)\
							)
	#define CHK_LogBackendCreate  TRUE
	#define EXP_LogBackendCreate  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogBackendLogBackendCreate
	#define EXT_CmpLogBackendLogBackendCreate
	#define GET_CmpLogBackendLogBackendCreate  ERR_OK
	#define CAL_CmpLogBackendLogBackendCreate(p0,p1,p2)	\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									((ICmpLogBackend*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ICmpLogBackend, (RTS_RESULT *)1))->ILogBackendCreate(p0,p1,p2)\
								)\
							)
	#define CHK_CmpLogBackendLogBackendCreate  (s_pfCMCreateInstance != NULL)
	#define EXP_CmpLogBackendLogBackendCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogBackendCreate
	#define EXT_LogBackendCreate
	#define GET_LogBackendCreate(fl)  ERR_OK
	#define CAL_LogBackendCreate(p0,p1,p2)\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									((ICmpLogBackend*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ICmpLogBackend, (RTS_RESULT *)1))->ILogBackendCreate(p0,p1,p2)\
								)\
							)
	#define CHK_LogBackendCreate  (s_pfCMCreateInstance != NULL)
	#define EXP_LogBackendCreate  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_LogBackendCreate
	#define EXT_LogBackendCreate
	#define GET_LogBackendCreate(fl)  ERR_OK
	#define CAL_LogBackendCreate(p0,p1,p2)		\
							(\
								(p0 = (IBase *)s_pfCMCreateInstance(p1, NULL)) == NULL ? RTS_INVALID_HANDLE : \
								(\
									(((IBase *)p0)->ClassId = p1) == CLASSID_INVALID ? RTS_INVALID_HANDLE :\
									(\
										(((IBase *)p0)->hInstance = ((ICmpLogBackend*)((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ICmpLogBackend, NULL))->ILogBackendCreate(p0,p1,p2)) == RTS_INVALID_HANDLE ? (s_pfCMDeleteInstance2(p1, (IBase *)p0) ? RTS_INVALID_HANDLE : RTS_INVALID_HANDLE) : \
										(\
											((IBase *)p0)->QueryInterface((IBase *)p0, ITFID_ICmpLogBackend, NULL)\
										)\
									)\
								)\
							)
	#define CHK_LogBackendCreate  TRUE
	#define EXP_LogBackendCreate  ERR_OK
#endif




/**
 * <description> Delete a logger </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <result>ERR_OK</result>
 */
STATICITF_DEF RTS_RESULT CDECL LogBackendDelete(RTS_HANDLE hBackend);
typedef RTS_RESULT (CDECL * PFLOGBACKENDDELETE) (RTS_HANDLE hBackend);
#if defined(CMPLOGBACKEND_NOTIMPLEMENTED) || defined(LOGBACKENDDELETE_NOTIMPLEMENTED)
	#define USE_LogBackendDelete
	#define EXT_LogBackendDelete
	#define GET_LogBackendDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogBackendDelete(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_LogBackendDelete  FALSE
	#define EXP_LogBackendDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogBackendDelete
	#define EXT_LogBackendDelete
	#define GET_LogBackendDelete(fl)  ERR_OK
	#define CAL_LogBackendDelete(p0) 		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpLogBackend*)p0)->ILogBackendDelete(((ICmpLogBackend*)p0)->pBase->hInstance) != ERR_OK ? \
														ERR_FAILED :\
														s_pfCMDeleteInstance2(((ICmpLogBackend*)p0)->pBase->ClassId, ((ICmpLogBackend*)p0)->pBase))
	#define CHK_LogBackendDelete  TRUE
	#define EXP_LogBackendDelete  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPLOGBACKEND_EXTERNAL)
	#define USE_LogBackendDelete
	#define EXT_LogBackendDelete
	#define GET_LogBackendDelete(fl)  ERR_OK
	#define CAL_LogBackendDelete(p0) 	 	(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpLogBackend*)p0)->ILogBackendDelete(((ICmpLogBackend*)p0)->pBase->hInstance) != ERR_OK ? \
														ERR_FAILED :\
														s_pfCMDeleteInstance2(((ICmpLogBackend*)p0)->pBase->ClassId, ((ICmpLogBackend*)p0)->pBase))
	#define CHK_LogBackendDelete  TRUE
	#define EXP_LogBackendDelete  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogBackendLogBackendDelete
	#define EXT_CmpLogBackendLogBackendDelete
	#define GET_CmpLogBackendLogBackendDelete  ERR_OK
	#define CAL_CmpLogBackendLogBackendDelete(p0)	 (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpLogBackend*)p0)->ILogBackendDelete())
	#define CHK_CmpLogBackendLogBackendDelete  TRUE
	#define EXP_CmpLogBackendLogBackendDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogBackendDelete
	#define EXT_LogBackendDelete
	#define GET_LogBackendDelete(fl)  ERR_OK
	#define CAL_LogBackendDelete(p0)	 (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpLogBackend*)p0)->ILogBackendDelete())
	#define CHK_LogBackendDelete  TRUE
	#define EXP_LogBackendDelete  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_LogBackendDelete
	#define EXT_LogBackendDelete
	#define GET_LogBackendDelete(fl)  ERR_OK
	#define CAL_LogBackendDelete(p0)			(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpLogBackend*)p0)->ILogBackendDelete(((ICmpLogBackend*)p0)->pBase->hInstance) != ERR_OK ? \
														ERR_FAILED :\
														s_pfCMDeleteInstance2(((ICmpLogBackend*)p0)->pBase->ClassId, ((ICmpLogBackend*)p0)->pBase))
	#define CHK_LogBackendDelete  TRUE
	#define EXP_LogBackendDelete  ERR_OK
#endif




/**
 * <description> Add a new log entry.
 * <p>IMPLEMENTATION NOTE: The logger is implemented without any 
 * synchronization. So calls to this function may occur parallel 
 * from different tasks. Especially when LT_DUMP_ALWAYS is configured 
 * this can happen.
 * If the backend needs a synchronization to write the entries to the backend, this have to be implemented by the backend itself.</p>
 * </description>
 * <param name="hLog" type="IN">Handle to logger</param>
 * <param name="pOptions" type="IN">Log options</param>
 * <param name="pLog" type="IN">Log entry</param>
 * <param name="pResult" type="OUT">Result</param>
 * <result>Handle to logger (logger could be split into a new logger)</result>
 */
STATICITF_DEF RTS_HANDLE CDECL LogBackendAdd(RTS_HANDLE hBackend, struct tagLogOptions *pOptions, struct tagLogEntry *pLog, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFLOGBACKENDADD) (RTS_HANDLE hBackend, struct tagLogOptions *pOptions, struct tagLogEntry *pLog, RTS_RESULT *pResult);
#if defined(CMPLOGBACKEND_NOTIMPLEMENTED) || defined(LOGBACKENDADD_NOTIMPLEMENTED)
	#define USE_LogBackendAdd
	#define EXT_LogBackendAdd
	#define GET_LogBackendAdd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_LogBackendAdd(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_LogBackendAdd  FALSE
	#define EXP_LogBackendAdd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_LogBackendAdd
	#define EXT_LogBackendAdd
	#define GET_LogBackendAdd(fl)  ERR_OK
	#define CAL_LogBackendAdd(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_HANDLE)RTS_INVALID_HANDLE : (((ICmpLogBackend*)p0)->pBase->hInstance = ((ICmpLogBackend*)p0)->ILogBackendAdd(((ICmpLogBackend*)p0)->pBase->hInstance,p1,p2,p3)))
	#define CHK_LogBackendAdd  TRUE
	#define EXP_LogBackendAdd  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPLOGBACKEND_EXTERNAL)
	#define USE_LogBackendAdd
	#define EXT_LogBackendAdd
	#define GET_LogBackendAdd(fl)  ERR_OK
	#define CAL_LogBackendAdd(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_HANDLE)RTS_INVALID_HANDLE : (((ICmpLogBackend*)p0)->pBase->hInstance = ((ICmpLogBackend*)p0)->ILogBackendAdd(((ICmpLogBackend*)p0)->pBase->hInstance,p1,p2,p3)))
	#define CHK_LogBackendAdd  TRUE
	#define EXP_LogBackendAdd  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpLogBackendLogBackendAdd
	#define EXT_CmpLogBackendLogBackendAdd
	#define GET_CmpLogBackendLogBackendAdd  ERR_OK
	#define CAL_CmpLogBackendLogBackendAdd(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_HANDLE)RTS_INVALID_HANDLE : ((ICmpLogBackend*)p0)->ILogBackendAdd(p1,p2,p3))
	#define CHK_CmpLogBackendLogBackendAdd  TRUE
	#define EXP_CmpLogBackendLogBackendAdd  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_LogBackendAdd
	#define EXT_LogBackendAdd
	#define GET_LogBackendAdd(fl)  ERR_OK
	#define CAL_LogBackendAdd(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_HANDLE)RTS_INVALID_HANDLE : ((ICmpLogBackend*)p0)->ILogBackendAdd(p1,p2,p3))
	#define CHK_LogBackendAdd  TRUE
	#define EXP_LogBackendAdd  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_LogBackendAdd
	#define EXT_LogBackendAdd
	#define GET_LogBackendAdd(fl)  ERR_OK
	#define CAL_LogBackendAdd(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_HANDLE)RTS_INVALID_HANDLE : (((ICmpLogBackend*)p0)->pBase->hInstance = ((ICmpLogBackend*)p0)->ILogBackendAdd(((ICmpLogBackend*)p0)->pBase->hInstance,p1,p2,p3)))
	#define CHK_LogBackendAdd  TRUE
	#define EXP_LogBackendAdd  ERR_OK
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFLOGBACKENDCREATE ILogBackendCreate;
 	PFLOGBACKENDDELETE ILogBackendDelete;
 	PFLOGBACKENDADD ILogBackendAdd;
 } ICmpLogBackend_C;

#ifdef CPLUSPLUS
class ICmpLogBackend : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ILogBackendCreate(RTS_HANDLE hICmpLogBackend, CLASSID ClassId, struct tagLogOptions *pOptions) =0;
		virtual RTS_RESULT CDECL ILogBackendDelete(void) =0;
		virtual RTS_HANDLE CDECL ILogBackendAdd(struct tagLogOptions *pOptions, struct tagLogEntry *pLog, RTS_RESULT *pResult) =0;
};
	#ifndef ITF_CmpLogBackend
		#define ITF_CmpLogBackend static ICmpLogBackend *pICmpLogBackend = NULL;
	#endif
	#define EXTITF_CmpLogBackend
#else	/*CPLUSPLUS*/
	typedef ICmpLogBackend_C		ICmpLogBackend;
	#ifndef ITF_CmpLogBackend
		#define ITF_CmpLogBackend
	#endif
	#define EXTITF_CmpLogBackend
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPLOGBACKENDITF_H_*/
