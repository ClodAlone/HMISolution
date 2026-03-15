 /**
 * <interfacename>CmpEventCallback</interfacename>
 * <description> 
 *	This interface specifies the callback interface for event callbacks.
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPEVENTCALLBACKITF_H_
#define _CMPEVENTCALLBACKITF_H_

#include "CmpStd.h"

 

 




#include "CmpItf.h"

typedef	RTS_UI32	EVENTID;
typedef	RTS_UI16	EVENTCLASSID;

/**
 * <category>Base event parameter</category>
 * <element name="EventId" type="IN">EventID of the event. Contains the class and the event</element>
 * <element name="usParamId" type="IN">Id of the parameter structure (see pParameter)</element>
 * <element name="usVersion" type="IN">Version of the parameter structure (see pParameter)</element>
 * <element name="pParameter" type="IN">Pointer to the event specific parameter, that is specified by Id (see usParamId)</element>
 * <element name="pUserParameter" type="IN">Pointer to the user specific parameter, that can be specified by registering the callback</element>
 */
typedef struct
{
	EVENTID EventId;
	CMPID CmpIdProvider;
	RTS_UI16 usParamId;
	RTS_UI16 usVersion;
	void* pParameter;
	void* pUserParameter;
} EventParam;

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description> Callback function </description>
 * <param name="pEventParam" type="IN">Pointer to the event parameter.</param>
 * <result></result>
 */
typedef void (CDECL *PFEVENTCALLBACKFUNCTION)(EventParam *pEventParam);

/**
 * <description> Callback interface </description>
 * <param name="hInstance" type="IN">Handle to callback instance.</param>
 * <param name="pEventParam" type="IN">Pointer to the event parameter.</param>
 * <result></result>
 */
STATICITF_DEF void CDECL EventCallback(RTS_HANDLE hInstance, EventParam *pEventParam);
typedef void (CDECL * PFEVENTCALLBACK) (RTS_HANDLE hInstance, EventParam *pEventParam);
#if defined(CMPEVENTCALLBACK_NOTIMPLEMENTED) || defined(EVENTCALLBACK_NOTIMPLEMENTED)
	#define USE_EventCallback
	#define EXT_EventCallback
	#define GET_EventCallback(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventCallback(p0,p1)  
	#define CHK_EventCallback  FALSE
	#define EXP_EventCallback  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventCallback
	#define EXT_EventCallback
	#define GET_EventCallback(fl)  ERR_OK
	#define CAL_EventCallback(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpEventCallback*)p0)->IEventCallback(((ICmpEventCallback*)p0)->pBase->hInstance,p1))
	#define CHK_EventCallback  TRUE
	#define EXP_EventCallback  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPEVENTCALLBACK_EXTERNAL)
	#define USE_EventCallback
	#define EXT_EventCallback
	#define GET_EventCallback(fl)  ERR_OK
	#define CAL_EventCallback(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpEventCallback*)p0)->IEventCallback(((ICmpEventCallback*)p0)->pBase->hInstance,p1))
	#define CHK_EventCallback  TRUE
	#define EXP_EventCallback  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventCallbackEventCallback
	#define EXT_CmpEventCallbackEventCallback
	#define GET_CmpEventCallbackEventCallback  ERR_OK
	#define CAL_CmpEventCallbackEventCallback(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpEventCallback*)p0)->IEventCallback(p1))
	#define CHK_CmpEventCallbackEventCallback  TRUE
	#define EXP_CmpEventCallbackEventCallback  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventCallback
	#define EXT_EventCallback
	#define GET_EventCallback(fl)  ERR_OK
	#define CAL_EventCallback(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpEventCallback*)p0)->IEventCallback(p1))
	#define CHK_EventCallback  TRUE
	#define EXP_EventCallback  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_EventCallback
	#define EXT_EventCallback
	#define GET_EventCallback(fl)  ERR_OK
	#define CAL_EventCallback(p0,p1) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpEventCallback*)p0)->IEventCallback(((ICmpEventCallback*)p0)->pBase->hInstance,p1))
	#define CHK_EventCallback  TRUE
	#define EXP_EventCallback  ERR_OK
#endif




#ifdef __cplusplus
}
#endif




typedef struct
{
	IBase_C *pBase;
	PFEVENTCALLBACK IEventCallback;
 } ICmpEventCallback_C;

#ifdef CPLUSPLUS
class ICmpEventCallback : public IBase
{
	public:
		virtual void CDECL IEventCallback(EventParam *pEventParam) =0;
};
	#ifndef ITF_CmpEventCallback
		#define ITF_CmpEventCallback static ICmpEventCallback *pICmpEventCallback = NULL;
	#endif
	#define EXTITF_CmpEventCallback
#else	/*CPLUSPLUS*/
	typedef ICmpEventCallback_C		ICmpEventCallback;
	#ifndef ITF_CmpEventCallback
		#define ITF_CmpEventCallback
	#endif
	#define EXTITF_CmpEventCallback
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPEVENTCALLBACKITF_H_*/
