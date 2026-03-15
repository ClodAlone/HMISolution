 /**
 * <interfacename>CmpEventMgr</interfacename>
 * <description> 
 *	<p>This is the interface of the event manager. The manager is responsible to handle all events in the
 *	runtime system and to call special registered functions (callbacks) if an event occurred. An event can be 
 *	sent in any situation, when a state will be changed in the runtime system. An event can be e.g. stop of 
 *	an Iec application, download of an Iec application, exception occurred in a component, etc.</p>
 *
 *	<p>Typically an event will be sent before a state changed (xxxPrepare) and if the state has changed (xxxDone)</p>
 *
 *	<p>The component that provides and sends an event is called the provider. The component that receives the event
 *	is called the consumer of an event.</p>
 *
 *	<p>Each component can define its own event ID. Such an event ID consists of two numbers:
 *	<ul>
 *		<li>Event class:	16 bit number that specifies the class of the event</li>
 *		<li>Event:			16 bit number that specifies the event</li>
 *	</ul>
 *	The event class and the event is matched to one 32 bit number that is called the event ID. The event, 
 *	event class and the component ID of the provider makes an event unique. So every provider has
 *	to specify at least these three things to sent an event.</p>
 *
 *	<p>Each provider can specify additional parameters for each event that is transferred to the consumer. This is
 *	event specific and can be specified by the consumer. Such an optional event parameter must be specified by a
 *	component specific parameter ID and with a parameter version number. With this information, a consumer can 
 *	check, which parameter in which version the provider is sending.</p>
 *	
 *	<p>To use an event, first of all the provider has to register the event. After that, the consumer can open this 
 *	event and can attach its callback routine to this event. Such a callback routine can be:</p>
 *	<ul>
 *		<li>C-Function</li>
 *		<li>Iec-Function</li>
 *		<li>Iec-Method of a function block</li>
 *		<li>C++-Method of a C++ class</li>
 *	</ul>
 *
 *	<p>IMPLEMENTATION NOTE: A provider typically registers its event in the CH_INIT2 hook. The consumer typically
 *	registers its callback to special events in the CH_INIT3 hook.</p>
 *	<p>If a provider only wants to register an event if it is really needed by a consumer, the CmpEventMgr sents
 *	a special event, if a consumer tries to open an event (see EVT_EventOpen). In this event, the provider
 *	can register the event and the consumer can open a valid event.</p>
 *	<p>In opposite, if a consumer wants to register a callback on an event, an event is sent if a provider 
 *	registers its event (see EVT_EventCreate).</p>
 *	<p>If an event is unregistred by a provider, the event EVT_EventDelete is sent. If an event is closed by
 *	a consumer, the event EVT_EventClose is sent.</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPEVENTMGRITF_H_
#define _CMPEVENTMGRITF_H_

#include "CmpStd.h"

 

 




#include "CmpItf.h"
#include "CmpEventCallbackItf.h"


/**
 * <category>Static defines</category>
 * <description>Maximum number of static allocated events</description>
 */
#ifndef EVENTMGR_NUM_OF_STATIC_EVENTS
	#define EVENTMGR_NUM_OF_STATIC_EVENTS		50
#endif

/**
 * <category>Static defines</category>
 * <description>Maximum number of static allocated callback routines to be registered</description>
 */
#ifndef EVENTMGR_NUM_OF_STATIC_CALLBACKS
	#define EVENTMGR_NUM_OF_STATIC_CALLBACKS	10
#endif

/**
 * <category>Static defines</category>
 * <description>Maximum number of static allocated iec callback routines to be registered</description>
 */
#ifndef EVENTMGR_NUM_OF_STATIC_IECCALLBACKS
	#define EVENTMGR_NUM_OF_STATIC_IECCALLBACKS	5
#endif

/**
 * <category>Convert macro</category>
 * <description>Macro to create an event ID with the event class and the event</description>
 */
#define MAKE_EVENTID(Class, Event)				(Class | Event)

/**
 * <category>Provider component ids</category>
 * <description>Special priovider ids</description>
 */
#define EVTPROVIDER_NONE					0
#define EVTPROVIDER_ALL						UINT32_MAX

/**
 * <category>Event classes</category>
 * <description><p>All possible event classes:</p>
 *	<ul>
 *		<li>EVTCLASS_NONE: No class or invalid</li>
 *		<li>EVTCLASS_ALL: All classes</li>
 *		<li>EVTCLASS_INFO: Information</li>
 *		<li>EVTCLASS_WARNING: Warning</li>
 *		<li>EVTCLASS_ERROR: Error</li>
 *		<li>EVTCLASS_EXCEPTION: Exception</li>
 *		<li>EVTCLASS_VENDOR_SPEC: Vendor specific. Can be used for own event classes</li>
 *	</ul>
 * </description>
 */
#define EVTCLASS_NONE						0	
#define EVTCLASS_ALL						UINT32_C(0xFFFF0000)
#define EVTCLASS_INFO						UINT32_C(0x00010000)
#define EVTCLASS_WARNING					UINT32_C(0x00020000)
#define EVTCLASS_ERROR						UINT32_C(0x00040000)
#define EVTCLASS_EXCEPTION					UINT32_C(0x00080000)
#define EVTCLASS_VENDOR_SPEC				UINT32_C(0x10000000)


/**
 * <category>Special Events</category>
 * <description>
 *	<ul>
 *		<li>EVT_NONE: No event or invalid</li>
 *		<li>EVT_ALL: All events</li>
 *	</ul>
 * </description>
 */
#define EVT_NONE							0
#define EVT_ALL								0xFFFF


/**
 * <category>Callback limit</category>
 * <description>No limit of callbacks possible per event</description>
 */
#define EVENT_CALLBACKS_NO_LIMIT			UINT32_MAX

/**
 * <category>Event manager flags</category>
 * <description>
 *	Flags used by the event manager.
 * </description>
 *	<element name="EMF_NO_IEC_CALLBACKS" type="IN">No callbacks in IEC possible</element>
 */
#define EMF_NO_IEC_CALLBACKS	UINT32_C(0x00000001)

/**
 * <category>Event options</category>
 * <element name="nCallbacksPossible" type="IN">Maximum number of callbacks possible on this event or EVENT_CALLBACKS_NO_LIMIT for no limit</element>
 * <element name="flags" type="IN">Event manager flags</element>
 */
typedef struct tagEventOptions
{
       RTS_UI32 nCallbacksPossible;
       RTS_UI32 flags;
} EventOptions;

/**
 * <category>Event parameter</category>
 * <element name="EventId" type="IN">EventID of the event. Contains the class and the event</element>
 * <element name="CmpIdProvider" type="IN">Component ID of the provider</element>
 */
typedef struct
{
	EVENTID EventId;
	CMPID CmpIdProvider;
} EVTPARAM_CmpEventMgr;
#define EVTPARAMID_CmpEventMgr		0x0001
#define EVTVERSION_CmpEventMgr		0x0001

/**
 * <category>Events</category>
 * <description>Event is sent after a provider creates a new event</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpEventMgr</param>
 */
#define EVT_EventCreate						MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Events</category>
 * <description>Event is sent before a provider deletes an event</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpEventMgr</param>
 */
#define EVT_EventDelete						MAKE_EVENTID(EVTCLASS_INFO, 2)

/**
 * <category>Events</category>
 * <description>Event is sent before a consumer tries to open an event</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpEventMgr</param>
 */
#define EVT_EventOpen						MAKE_EVENTID(EVTCLASS_INFO, 3)

/**
 * <category>Events</category>
 * <description>Event is sent before a consumer closes an event</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpEventMgr</param>
 */
#define EVT_EventClose						MAKE_EVENTID(EVTCLASS_INFO, 4)

/**
 * <category>Events</category>
 * <description>Event is sent in every call of the CH_COMM_CYCLE. Can be used for IEC background jobs.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpEventMgr</param>
 */
#define EVT_EventCommCycle					MAKE_EVENTID(EVTCLASS_INFO, 5)

#define EventParam2		EventParam

/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

typedef struct
{
	void* __VFTABLEPOINTER;	/* Pointer to virtual function table */
} icmpeventcallback_struct;

/**
 * The interface ICmpEventCallback must be registered for an event. If this event is posted, this callback method is called.
 */
typedef struct tagicmpeventcallback_eventcallback_struct
{
	icmpeventcallback_struct *pInstance;	/* VAR_INPUT */	
	EventParam *pEventParam;			/* VAR_INPUT */	/* Pointer to the event parameters, see Struct EventParam */
	RTS_IEC_RESULT EventCallback;		/* VAR_OUTPUT */	
} icmpeventcallback_eventcallback_struct;

/**
 * Close an event specified by handle 
 */
typedef struct tageventclose_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Event handle */
	RTS_IEC_RESULT EventClose;			/* VAR_OUTPUT */	/* Error code */
} eventclose_struct;

void CDECL CDECL_EXT eventclose(eventclose_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTCLOSE_IEC) (eventclose_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTCLOSE_NOTIMPLEMENTED)
	#define USE_eventclose
	#define EXT_eventclose
	#define GET_eventclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventclose(p0) 
	#define CHK_eventclose  FALSE
	#define EXP_eventclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventclose
	#define EXT_eventclose
	#define GET_eventclose(fl)  CAL_CMGETAPI( "eventclose" ) 
	#define CAL_eventclose  eventclose
	#define CHK_eventclose  TRUE
	#define EXP_eventclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventclose", (RTS_UINTPTR)eventclose, 1, RTSITF_GET_SIGNATURE(0, 0x4668EEC1), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventclose
	#define EXT_eventclose
	#define GET_eventclose(fl)  CAL_CMGETAPI( "eventclose" ) 
	#define CAL_eventclose  eventclose
	#define CHK_eventclose  TRUE
	#define EXP_eventclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventclose", (RTS_UINTPTR)eventclose, 1, RTSITF_GET_SIGNATURE(0, 0x4668EEC1), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventclose
	#define EXT_CmpEventMgreventclose
	#define GET_CmpEventMgreventclose  ERR_OK
	#define CAL_CmpEventMgreventclose  eventclose
	#define CHK_CmpEventMgreventclose  TRUE
	#define EXP_CmpEventMgreventclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventclose", (RTS_UINTPTR)eventclose, 1, RTSITF_GET_SIGNATURE(0, 0x4668EEC1), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventclose
	#define EXT_eventclose
	#define GET_eventclose(fl)  CAL_CMGETAPI( "eventclose" ) 
	#define CAL_eventclose  eventclose
	#define CHK_eventclose  TRUE
	#define EXP_eventclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventclose", (RTS_UINTPTR)eventclose, 1, RTSITF_GET_SIGNATURE(0, 0x4668EEC1), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventclose  PFEVENTCLOSE_IEC pfeventclose;
	#define EXT_eventclose  extern PFEVENTCLOSE_IEC pfeventclose;
	#define GET_eventclose(fl)  s_pfCMGetAPI2( "eventclose", (RTS_VOID_FCTPTR *)&pfeventclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x4668EEC1), 0x03050C00)
	#define CAL_eventclose  pfeventclose
	#define CHK_eventclose  (pfeventclose != NULL)
	#define EXP_eventclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventclose", (RTS_UINTPTR)eventclose, 1, RTSITF_GET_SIGNATURE(0, 0x4668EEC1), 0x03050C00) 
#endif


/**
 * Creates a new event object. If event still exists, a handle to this object will be returned.
 *   An IEC event is typically created by the provider in FB_Init of a function block 
 */
typedef struct tageventcreate_struct
{
	RTS_IEC_UDINT EventId;				/* VAR_INPUT */	/* Event ID of the event. Contains the class and the event */
	RTS_IEC_UDINT CmpIdProvider;		/* VAR_INPUT */	/* Component ID of the provider */
	RTS_IEC_RESULT *Result;				/* VAR_IN_OUT */	/* Error code */
	RTS_IEC_HANDLE EventCreate;			/* VAR_OUTPUT */	/* Handle to created event */
} eventcreate_struct;

void CDECL CDECL_EXT eventcreate(eventcreate_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTCREATE_IEC) (eventcreate_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTCREATE_NOTIMPLEMENTED)
	#define USE_eventcreate
	#define EXT_eventcreate
	#define GET_eventcreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventcreate(p0) 
	#define CHK_eventcreate  FALSE
	#define EXP_eventcreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventcreate
	#define EXT_eventcreate
	#define GET_eventcreate(fl)  CAL_CMGETAPI( "eventcreate" ) 
	#define CAL_eventcreate  eventcreate
	#define CHK_eventcreate  TRUE
	#define EXP_eventcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate", (RTS_UINTPTR)eventcreate, 1, RTSITF_GET_SIGNATURE(0, 0xAC9E014A), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventcreate
	#define EXT_eventcreate
	#define GET_eventcreate(fl)  CAL_CMGETAPI( "eventcreate" ) 
	#define CAL_eventcreate  eventcreate
	#define CHK_eventcreate  TRUE
	#define EXP_eventcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate", (RTS_UINTPTR)eventcreate, 1, RTSITF_GET_SIGNATURE(0, 0xAC9E014A), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventcreate
	#define EXT_CmpEventMgreventcreate
	#define GET_CmpEventMgreventcreate  ERR_OK
	#define CAL_CmpEventMgreventcreate  eventcreate
	#define CHK_CmpEventMgreventcreate  TRUE
	#define EXP_CmpEventMgreventcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate", (RTS_UINTPTR)eventcreate, 1, RTSITF_GET_SIGNATURE(0, 0xAC9E014A), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventcreate
	#define EXT_eventcreate
	#define GET_eventcreate(fl)  CAL_CMGETAPI( "eventcreate" ) 
	#define CAL_eventcreate  eventcreate
	#define CHK_eventcreate  TRUE
	#define EXP_eventcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate", (RTS_UINTPTR)eventcreate, 1, RTSITF_GET_SIGNATURE(0, 0xAC9E014A), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventcreate  PFEVENTCREATE_IEC pfeventcreate;
	#define EXT_eventcreate  extern PFEVENTCREATE_IEC pfeventcreate;
	#define GET_eventcreate(fl)  s_pfCMGetAPI2( "eventcreate", (RTS_VOID_FCTPTR *)&pfeventcreate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xAC9E014A), 0x03050C00)
	#define CAL_eventcreate  pfeventcreate
	#define CHK_eventcreate  (pfeventcreate != NULL)
	#define EXP_eventcreate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate", (RTS_UINTPTR)eventcreate, 1, RTSITF_GET_SIGNATURE(0, 0xAC9E014A), 0x03050C00) 
#endif


/**
 * Creates a new event object. If event still exists, a handle to this object will be returned.
 *   An IEC event is typically created by the provider in FB_Init of a function block 
 */
typedef struct tageventcreate2_struct
{
	RTS_IEC_UDINT EventId;				/* VAR_INPUT */	/* Event ID of the event. Contains the class and the event */
	RTS_IEC_UDINT CmpIdProvider;		/* VAR_INPUT */	/* Component ID of the provider */
	RTS_IEC_UDINT nCallbacksPossible;	/* VAR_INPUT */	/* Maximum number of callbacks possible on this event or EVENT_CALLBACKS_NO_LIMIT for no limit */
	RTS_IEC_RESULT *Result;				/* VAR_IN_OUT */	/* Error code */
	RTS_IEC_HANDLE EventCreate2;		/* VAR_OUTPUT */	/* Handle to created event */
} eventcreate2_struct;

void CDECL CDECL_EXT eventcreate2(eventcreate2_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTCREATE2_IEC) (eventcreate2_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTCREATE2_NOTIMPLEMENTED)
	#define USE_eventcreate2
	#define EXT_eventcreate2
	#define GET_eventcreate2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventcreate2(p0) 
	#define CHK_eventcreate2  FALSE
	#define EXP_eventcreate2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventcreate2
	#define EXT_eventcreate2
	#define GET_eventcreate2(fl)  CAL_CMGETAPI( "eventcreate2" ) 
	#define CAL_eventcreate2  eventcreate2
	#define CHK_eventcreate2  TRUE
	#define EXP_eventcreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate2", (RTS_UINTPTR)eventcreate2, 1, RTSITF_GET_SIGNATURE(0, 0xD6BF4CE8), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventcreate2
	#define EXT_eventcreate2
	#define GET_eventcreate2(fl)  CAL_CMGETAPI( "eventcreate2" ) 
	#define CAL_eventcreate2  eventcreate2
	#define CHK_eventcreate2  TRUE
	#define EXP_eventcreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate2", (RTS_UINTPTR)eventcreate2, 1, RTSITF_GET_SIGNATURE(0, 0xD6BF4CE8), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventcreate2
	#define EXT_CmpEventMgreventcreate2
	#define GET_CmpEventMgreventcreate2  ERR_OK
	#define CAL_CmpEventMgreventcreate2  eventcreate2
	#define CHK_CmpEventMgreventcreate2  TRUE
	#define EXP_CmpEventMgreventcreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate2", (RTS_UINTPTR)eventcreate2, 1, RTSITF_GET_SIGNATURE(0, 0xD6BF4CE8), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventcreate2
	#define EXT_eventcreate2
	#define GET_eventcreate2(fl)  CAL_CMGETAPI( "eventcreate2" ) 
	#define CAL_eventcreate2  eventcreate2
	#define CHK_eventcreate2  TRUE
	#define EXP_eventcreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate2", (RTS_UINTPTR)eventcreate2, 1, RTSITF_GET_SIGNATURE(0, 0xD6BF4CE8), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventcreate2  PFEVENTCREATE2_IEC pfeventcreate2;
	#define EXT_eventcreate2  extern PFEVENTCREATE2_IEC pfeventcreate2;
	#define GET_eventcreate2(fl)  s_pfCMGetAPI2( "eventcreate2", (RTS_VOID_FCTPTR *)&pfeventcreate2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xD6BF4CE8), 0x03050C00)
	#define CAL_eventcreate2  pfeventcreate2
	#define CHK_eventcreate2  (pfeventcreate2 != NULL)
	#define EXP_eventcreate2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventcreate2", (RTS_UINTPTR)eventcreate2, 1, RTSITF_GET_SIGNATURE(0, 0xD6BF4CE8), 0x03050C00) 
#endif


/**
 * Deletes an event specified by handle. 
 */
typedef struct tageventdelete_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Event handle */
	RTS_IEC_RESULT EventDelete;			/* VAR_OUTPUT */	/* Error code */
} eventdelete_struct;

void CDECL CDECL_EXT eventdelete(eventdelete_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTDELETE_IEC) (eventdelete_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTDELETE_NOTIMPLEMENTED)
	#define USE_eventdelete
	#define EXT_eventdelete
	#define GET_eventdelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventdelete(p0) 
	#define CHK_eventdelete  FALSE
	#define EXP_eventdelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventdelete
	#define EXT_eventdelete
	#define GET_eventdelete(fl)  CAL_CMGETAPI( "eventdelete" ) 
	#define CAL_eventdelete  eventdelete
	#define CHK_eventdelete  TRUE
	#define EXP_eventdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventdelete", (RTS_UINTPTR)eventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x0794C5F5), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventdelete
	#define EXT_eventdelete
	#define GET_eventdelete(fl)  CAL_CMGETAPI( "eventdelete" ) 
	#define CAL_eventdelete  eventdelete
	#define CHK_eventdelete  TRUE
	#define EXP_eventdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventdelete", (RTS_UINTPTR)eventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x0794C5F5), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventdelete
	#define EXT_CmpEventMgreventdelete
	#define GET_CmpEventMgreventdelete  ERR_OK
	#define CAL_CmpEventMgreventdelete  eventdelete
	#define CHK_CmpEventMgreventdelete  TRUE
	#define EXP_CmpEventMgreventdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventdelete", (RTS_UINTPTR)eventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x0794C5F5), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventdelete
	#define EXT_eventdelete
	#define GET_eventdelete(fl)  CAL_CMGETAPI( "eventdelete" ) 
	#define CAL_eventdelete  eventdelete
	#define CHK_eventdelete  TRUE
	#define EXP_eventdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventdelete", (RTS_UINTPTR)eventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x0794C5F5), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventdelete  PFEVENTDELETE_IEC pfeventdelete;
	#define EXT_eventdelete  extern PFEVENTDELETE_IEC pfeventdelete;
	#define GET_eventdelete(fl)  s_pfCMGetAPI2( "eventdelete", (RTS_VOID_FCTPTR *)&pfeventdelete, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x0794C5F5), 0x03050C00)
	#define CAL_eventdelete  pfeventdelete
	#define CHK_eventdelete  (pfeventdelete != NULL)
	#define EXP_eventdelete   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventdelete", (RTS_UINTPTR)eventdelete, 1, RTSITF_GET_SIGNATURE(0, 0x0794C5F5), 0x03050C00) 
#endif


/**
 * Extract the event class from eventid. Return value is the event class 
 */
typedef struct tageventgetclass_struct
{
	RTS_IEC_UDINT EventId;				/* VAR_INPUT */	/* Event id */
	RTS_IEC_UINT EventGetClass;			/* VAR_OUTPUT */	
} eventgetclass_struct;

void CDECL CDECL_EXT eventgetclass(eventgetclass_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTGETCLASS_IEC) (eventgetclass_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTGETCLASS_NOTIMPLEMENTED)
	#define USE_eventgetclass
	#define EXT_eventgetclass
	#define GET_eventgetclass(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventgetclass(p0) 
	#define CHK_eventgetclass  FALSE
	#define EXP_eventgetclass  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventgetclass
	#define EXT_eventgetclass
	#define GET_eventgetclass(fl)  CAL_CMGETAPI( "eventgetclass" ) 
	#define CAL_eventgetclass  eventgetclass
	#define CHK_eventgetclass  TRUE
	#define EXP_eventgetclass  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetclass", (RTS_UINTPTR)eventgetclass, 1, RTSITF_GET_SIGNATURE(0, 0xB0745754), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventgetclass
	#define EXT_eventgetclass
	#define GET_eventgetclass(fl)  CAL_CMGETAPI( "eventgetclass" ) 
	#define CAL_eventgetclass  eventgetclass
	#define CHK_eventgetclass  TRUE
	#define EXP_eventgetclass  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetclass", (RTS_UINTPTR)eventgetclass, 1, RTSITF_GET_SIGNATURE(0, 0xB0745754), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventgetclass
	#define EXT_CmpEventMgreventgetclass
	#define GET_CmpEventMgreventgetclass  ERR_OK
	#define CAL_CmpEventMgreventgetclass  eventgetclass
	#define CHK_CmpEventMgreventgetclass  TRUE
	#define EXP_CmpEventMgreventgetclass  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetclass", (RTS_UINTPTR)eventgetclass, 1, RTSITF_GET_SIGNATURE(0, 0xB0745754), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventgetclass
	#define EXT_eventgetclass
	#define GET_eventgetclass(fl)  CAL_CMGETAPI( "eventgetclass" ) 
	#define CAL_eventgetclass  eventgetclass
	#define CHK_eventgetclass  TRUE
	#define EXP_eventgetclass  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetclass", (RTS_UINTPTR)eventgetclass, 1, RTSITF_GET_SIGNATURE(0, 0xB0745754), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventgetclass  PFEVENTGETCLASS_IEC pfeventgetclass;
	#define EXT_eventgetclass  extern PFEVENTGETCLASS_IEC pfeventgetclass;
	#define GET_eventgetclass(fl)  s_pfCMGetAPI2( "eventgetclass", (RTS_VOID_FCTPTR *)&pfeventgetclass, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB0745754), 0x03050C00)
	#define CAL_eventgetclass  pfeventgetclass
	#define CHK_eventgetclass  (pfeventgetclass != NULL)
	#define EXP_eventgetclass   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetclass", (RTS_UINTPTR)eventgetclass, 1, RTSITF_GET_SIGNATURE(0, 0xB0745754), 0x03050C00) 
#endif


/**
 * Extract the event from eventid. Return value ist the event 
 */
typedef struct tageventgetevent_struct
{
	RTS_IEC_UDINT EventId;				/* VAR_INPUT */	/* Event ID */
	RTS_IEC_UINT EventGetEvent;			/* VAR_OUTPUT */	
} eventgetevent_struct;

void CDECL CDECL_EXT eventgetevent(eventgetevent_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTGETEVENT_IEC) (eventgetevent_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTGETEVENT_NOTIMPLEMENTED)
	#define USE_eventgetevent
	#define EXT_eventgetevent
	#define GET_eventgetevent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventgetevent(p0) 
	#define CHK_eventgetevent  FALSE
	#define EXP_eventgetevent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventgetevent
	#define EXT_eventgetevent
	#define GET_eventgetevent(fl)  CAL_CMGETAPI( "eventgetevent" ) 
	#define CAL_eventgetevent  eventgetevent
	#define CHK_eventgetevent  TRUE
	#define EXP_eventgetevent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetevent", (RTS_UINTPTR)eventgetevent, 1, RTSITF_GET_SIGNATURE(0, 0x0E7E3D61), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventgetevent
	#define EXT_eventgetevent
	#define GET_eventgetevent(fl)  CAL_CMGETAPI( "eventgetevent" ) 
	#define CAL_eventgetevent  eventgetevent
	#define CHK_eventgetevent  TRUE
	#define EXP_eventgetevent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetevent", (RTS_UINTPTR)eventgetevent, 1, RTSITF_GET_SIGNATURE(0, 0x0E7E3D61), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventgetevent
	#define EXT_CmpEventMgreventgetevent
	#define GET_CmpEventMgreventgetevent  ERR_OK
	#define CAL_CmpEventMgreventgetevent  eventgetevent
	#define CHK_CmpEventMgreventgetevent  TRUE
	#define EXP_CmpEventMgreventgetevent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetevent", (RTS_UINTPTR)eventgetevent, 1, RTSITF_GET_SIGNATURE(0, 0x0E7E3D61), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventgetevent
	#define EXT_eventgetevent
	#define GET_eventgetevent(fl)  CAL_CMGETAPI( "eventgetevent" ) 
	#define CAL_eventgetevent  eventgetevent
	#define CHK_eventgetevent  TRUE
	#define EXP_eventgetevent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetevent", (RTS_UINTPTR)eventgetevent, 1, RTSITF_GET_SIGNATURE(0, 0x0E7E3D61), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventgetevent  PFEVENTGETEVENT_IEC pfeventgetevent;
	#define EXT_eventgetevent  extern PFEVENTGETEVENT_IEC pfeventgetevent;
	#define GET_eventgetevent(fl)  s_pfCMGetAPI2( "eventgetevent", (RTS_VOID_FCTPTR *)&pfeventgetevent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x0E7E3D61), 0x03050C00)
	#define CAL_eventgetevent  pfeventgetevent
	#define CHK_eventgetevent  (pfeventgetevent != NULL)
	#define EXP_eventgetevent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventgetevent", (RTS_UINTPTR)eventgetevent, 1, RTSITF_GET_SIGNATURE(0, 0x0E7E3D61), 0x03050C00) 
#endif


/**
 * Opens an existing event object. Can be used to check, if the event was created by the provider.
 *   If the event does not exist, an error code is returned. 
 */
typedef struct tageventopen_struct
{
	RTS_IEC_UDINT EventId;				/* VAR_INPUT */	/* EventID of the event. Contains the class and the event */
	RTS_IEC_UDINT CmpIdProvider;		/* VAR_INPUT */	/* Component ID of the provider */
	RTS_IEC_RESULT *Result;				/* VAR_IN_OUT */	/* Error code */
	RTS_IEC_HANDLE EventOpen;			/* VAR_OUTPUT */	/* Handle to opened event */
} eventopen_struct;

void CDECL CDECL_EXT eventopen(eventopen_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTOPEN_IEC) (eventopen_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTOPEN_NOTIMPLEMENTED)
	#define USE_eventopen
	#define EXT_eventopen
	#define GET_eventopen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventopen(p0) 
	#define CHK_eventopen  FALSE
	#define EXP_eventopen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventopen
	#define EXT_eventopen
	#define GET_eventopen(fl)  CAL_CMGETAPI( "eventopen" ) 
	#define CAL_eventopen  eventopen
	#define CHK_eventopen  TRUE
	#define EXP_eventopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventopen", (RTS_UINTPTR)eventopen, 1, RTSITF_GET_SIGNATURE(0, 0xBC5AE4E1), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventopen
	#define EXT_eventopen
	#define GET_eventopen(fl)  CAL_CMGETAPI( "eventopen" ) 
	#define CAL_eventopen  eventopen
	#define CHK_eventopen  TRUE
	#define EXP_eventopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventopen", (RTS_UINTPTR)eventopen, 1, RTSITF_GET_SIGNATURE(0, 0xBC5AE4E1), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventopen
	#define EXT_CmpEventMgreventopen
	#define GET_CmpEventMgreventopen  ERR_OK
	#define CAL_CmpEventMgreventopen  eventopen
	#define CHK_CmpEventMgreventopen  TRUE
	#define EXP_CmpEventMgreventopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventopen", (RTS_UINTPTR)eventopen, 1, RTSITF_GET_SIGNATURE(0, 0xBC5AE4E1), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventopen
	#define EXT_eventopen
	#define GET_eventopen(fl)  CAL_CMGETAPI( "eventopen" ) 
	#define CAL_eventopen  eventopen
	#define CHK_eventopen  TRUE
	#define EXP_eventopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventopen", (RTS_UINTPTR)eventopen, 1, RTSITF_GET_SIGNATURE(0, 0xBC5AE4E1), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventopen  PFEVENTOPEN_IEC pfeventopen;
	#define EXT_eventopen  extern PFEVENTOPEN_IEC pfeventopen;
	#define GET_eventopen(fl)  s_pfCMGetAPI2( "eventopen", (RTS_VOID_FCTPTR *)&pfeventopen, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xBC5AE4E1), 0x03050C00)
	#define CAL_eventopen  pfeventopen
	#define CHK_eventopen  (pfeventopen != NULL)
	#define EXP_eventopen   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventopen", (RTS_UINTPTR)eventopen, 1, RTSITF_GET_SIGNATURE(0, 0xBC5AE4E1), 0x03050C00) 
#endif


/**
 * Post or sent an event 
 */
typedef struct tageventpost_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Event handle */
	EventParam *pEventParam;			/* VAR_INPUT */	/* Pointer to the event parameters */
	RTS_IEC_RESULT EventPost;			/* VAR_OUTPUT */	/* Error code */
} eventpost_struct;

void CDECL CDECL_EXT eventpost(eventpost_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTPOST_IEC) (eventpost_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTPOST_NOTIMPLEMENTED)
	#define USE_eventpost
	#define EXT_eventpost
	#define GET_eventpost(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventpost(p0) 
	#define CHK_eventpost  FALSE
	#define EXP_eventpost  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventpost
	#define EXT_eventpost
	#define GET_eventpost(fl)  CAL_CMGETAPI( "eventpost" ) 
	#define CAL_eventpost  eventpost
	#define CHK_eventpost  TRUE
	#define EXP_eventpost  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost", (RTS_UINTPTR)eventpost, 1, RTSITF_GET_SIGNATURE(0, 0x9FDEA762), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventpost
	#define EXT_eventpost
	#define GET_eventpost(fl)  CAL_CMGETAPI( "eventpost" ) 
	#define CAL_eventpost  eventpost
	#define CHK_eventpost  TRUE
	#define EXP_eventpost  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost", (RTS_UINTPTR)eventpost, 1, RTSITF_GET_SIGNATURE(0, 0x9FDEA762), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventpost
	#define EXT_CmpEventMgreventpost
	#define GET_CmpEventMgreventpost  ERR_OK
	#define CAL_CmpEventMgreventpost  eventpost
	#define CHK_CmpEventMgreventpost  TRUE
	#define EXP_CmpEventMgreventpost  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost", (RTS_UINTPTR)eventpost, 1, RTSITF_GET_SIGNATURE(0, 0x9FDEA762), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventpost
	#define EXT_eventpost
	#define GET_eventpost(fl)  CAL_CMGETAPI( "eventpost" ) 
	#define CAL_eventpost  eventpost
	#define CHK_eventpost  TRUE
	#define EXP_eventpost  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost", (RTS_UINTPTR)eventpost, 1, RTSITF_GET_SIGNATURE(0, 0x9FDEA762), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventpost  PFEVENTPOST_IEC pfeventpost;
	#define EXT_eventpost  extern PFEVENTPOST_IEC pfeventpost;
	#define GET_eventpost(fl)  s_pfCMGetAPI2( "eventpost", (RTS_VOID_FCTPTR *)&pfeventpost, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x9FDEA762), 0x03050C00)
	#define CAL_eventpost  pfeventpost
	#define CHK_eventpost  (pfeventpost != NULL)
	#define EXP_eventpost   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost", (RTS_UINTPTR)eventpost, 1, RTSITF_GET_SIGNATURE(0, 0x9FDEA762), 0x03050C00) 
#endif


/**
 * Post or sent an event 
 */
typedef struct tageventpost2_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Event handle */
	EventParam2 *pEventParam;			/* VAR_INPUT */	/* Pointer to the event parameters */
	RTS_IEC_RESULT EventPost2;			/* VAR_OUTPUT */	/* Error code */
} eventpost2_struct;

void CDECL CDECL_EXT eventpost2(eventpost2_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTPOST2_IEC) (eventpost2_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTPOST2_NOTIMPLEMENTED)
	#define USE_eventpost2
	#define EXT_eventpost2
	#define GET_eventpost2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventpost2(p0) 
	#define CHK_eventpost2  FALSE
	#define EXP_eventpost2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventpost2
	#define EXT_eventpost2
	#define GET_eventpost2(fl)  CAL_CMGETAPI( "eventpost2" ) 
	#define CAL_eventpost2  eventpost2
	#define CHK_eventpost2  TRUE
	#define EXP_eventpost2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost2", (RTS_UINTPTR)eventpost2, 1, 0xCE90F5F0, 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventpost2
	#define EXT_eventpost2
	#define GET_eventpost2(fl)  CAL_CMGETAPI( "eventpost2" ) 
	#define CAL_eventpost2  eventpost2
	#define CHK_eventpost2  TRUE
	#define EXP_eventpost2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost2", (RTS_UINTPTR)eventpost2, 1, 0xCE90F5F0, 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventpost2
	#define EXT_CmpEventMgreventpost2
	#define GET_CmpEventMgreventpost2  ERR_OK
	#define CAL_CmpEventMgreventpost2  eventpost2
	#define CHK_CmpEventMgreventpost2  TRUE
	#define EXP_CmpEventMgreventpost2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost2", (RTS_UINTPTR)eventpost2, 1, 0xCE90F5F0, 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventpost2
	#define EXT_eventpost2
	#define GET_eventpost2(fl)  CAL_CMGETAPI( "eventpost2" ) 
	#define CAL_eventpost2  eventpost2
	#define CHK_eventpost2  TRUE
	#define EXP_eventpost2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost2", (RTS_UINTPTR)eventpost2, 1, 0xCE90F5F0, 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventpost2  PFEVENTPOST2_IEC pfeventpost2;
	#define EXT_eventpost2  extern PFEVENTPOST2_IEC pfeventpost2;
	#define GET_eventpost2(fl)  s_pfCMGetAPI2( "eventpost2", (RTS_VOID_FCTPTR *)&pfeventpost2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xCE90F5F0, 0x03050C00)
	#define CAL_eventpost2  pfeventpost2
	#define CHK_eventpost2  (pfeventpost2 != NULL)
	#define EXP_eventpost2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpost2", (RTS_UINTPTR)eventpost2, 1, 0xCE90F5F0, 0x03050C00) 
#endif


/**
 * Post an event direct without the event handle 
 */
typedef struct tageventpostbyevent_struct
{
	RTS_IEC_UDINT EventId;				/* VAR_INPUT */	/* Event ID of the event. Contains the class and the event */
	RTS_IEC_UDINT CmpIdProvider;		/* VAR_INPUT */	/* Component ID of the provider */
	EventParam *pEventParam;			/* VAR_INPUT */	/* Pointer to the event parameters */
	RTS_IEC_RESULT EventPostByEvent;	/* VAR_OUTPUT */	/* Error code */
} eventpostbyevent_struct;

void CDECL CDECL_EXT eventpostbyevent(eventpostbyevent_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTPOSTBYEVENT_IEC) (eventpostbyevent_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTPOSTBYEVENT_NOTIMPLEMENTED)
	#define USE_eventpostbyevent
	#define EXT_eventpostbyevent
	#define GET_eventpostbyevent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventpostbyevent(p0) 
	#define CHK_eventpostbyevent  FALSE
	#define EXP_eventpostbyevent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventpostbyevent
	#define EXT_eventpostbyevent
	#define GET_eventpostbyevent(fl)  CAL_CMGETAPI( "eventpostbyevent" ) 
	#define CAL_eventpostbyevent  eventpostbyevent
	#define CHK_eventpostbyevent  TRUE
	#define EXP_eventpostbyevent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent", (RTS_UINTPTR)eventpostbyevent, 1, RTSITF_GET_SIGNATURE(0, 0xA33BD23D), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventpostbyevent
	#define EXT_eventpostbyevent
	#define GET_eventpostbyevent(fl)  CAL_CMGETAPI( "eventpostbyevent" ) 
	#define CAL_eventpostbyevent  eventpostbyevent
	#define CHK_eventpostbyevent  TRUE
	#define EXP_eventpostbyevent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent", (RTS_UINTPTR)eventpostbyevent, 1, RTSITF_GET_SIGNATURE(0, 0xA33BD23D), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventpostbyevent
	#define EXT_CmpEventMgreventpostbyevent
	#define GET_CmpEventMgreventpostbyevent  ERR_OK
	#define CAL_CmpEventMgreventpostbyevent  eventpostbyevent
	#define CHK_CmpEventMgreventpostbyevent  TRUE
	#define EXP_CmpEventMgreventpostbyevent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent", (RTS_UINTPTR)eventpostbyevent, 1, RTSITF_GET_SIGNATURE(0, 0xA33BD23D), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventpostbyevent
	#define EXT_eventpostbyevent
	#define GET_eventpostbyevent(fl)  CAL_CMGETAPI( "eventpostbyevent" ) 
	#define CAL_eventpostbyevent  eventpostbyevent
	#define CHK_eventpostbyevent  TRUE
	#define EXP_eventpostbyevent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent", (RTS_UINTPTR)eventpostbyevent, 1, RTSITF_GET_SIGNATURE(0, 0xA33BD23D), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventpostbyevent  PFEVENTPOSTBYEVENT_IEC pfeventpostbyevent;
	#define EXT_eventpostbyevent  extern PFEVENTPOSTBYEVENT_IEC pfeventpostbyevent;
	#define GET_eventpostbyevent(fl)  s_pfCMGetAPI2( "eventpostbyevent", (RTS_VOID_FCTPTR *)&pfeventpostbyevent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xA33BD23D), 0x03050C00)
	#define CAL_eventpostbyevent  pfeventpostbyevent
	#define CHK_eventpostbyevent  (pfeventpostbyevent != NULL)
	#define EXP_eventpostbyevent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent", (RTS_UINTPTR)eventpostbyevent, 1, RTSITF_GET_SIGNATURE(0, 0xA33BD23D), 0x03050C00) 
#endif


/**
 * Post an event direct without the event handle 
 */
typedef struct tageventpostbyevent2_struct
{
	RTS_IEC_UDINT EventId;				/* VAR_INPUT */	/* Event ID of the event. Contains the class and the event */
	RTS_IEC_UDINT CmpIdProvider;		/* VAR_INPUT */	/* Component ID of the provider */
	EventParam2 *pEventParam;			/* VAR_INPUT */	/* Pointer to the event parameters */
	RTS_IEC_RESULT EventPostByEvent2;	/* VAR_OUTPUT */	/* Error code */
} eventpostbyevent2_struct;

void CDECL CDECL_EXT eventpostbyevent2(eventpostbyevent2_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTPOSTBYEVENT2_IEC) (eventpostbyevent2_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTPOSTBYEVENT2_NOTIMPLEMENTED)
	#define USE_eventpostbyevent2
	#define EXT_eventpostbyevent2
	#define GET_eventpostbyevent2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventpostbyevent2(p0) 
	#define CHK_eventpostbyevent2  FALSE
	#define EXP_eventpostbyevent2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventpostbyevent2
	#define EXT_eventpostbyevent2
	#define GET_eventpostbyevent2(fl)  CAL_CMGETAPI( "eventpostbyevent2" ) 
	#define CAL_eventpostbyevent2  eventpostbyevent2
	#define CHK_eventpostbyevent2  TRUE
	#define EXP_eventpostbyevent2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent2", (RTS_UINTPTR)eventpostbyevent2, 1, 0x61F1EA28, 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventpostbyevent2
	#define EXT_eventpostbyevent2
	#define GET_eventpostbyevent2(fl)  CAL_CMGETAPI( "eventpostbyevent2" ) 
	#define CAL_eventpostbyevent2  eventpostbyevent2
	#define CHK_eventpostbyevent2  TRUE
	#define EXP_eventpostbyevent2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent2", (RTS_UINTPTR)eventpostbyevent2, 1, 0x61F1EA28, 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventpostbyevent2
	#define EXT_CmpEventMgreventpostbyevent2
	#define GET_CmpEventMgreventpostbyevent2  ERR_OK
	#define CAL_CmpEventMgreventpostbyevent2  eventpostbyevent2
	#define CHK_CmpEventMgreventpostbyevent2  TRUE
	#define EXP_CmpEventMgreventpostbyevent2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent2", (RTS_UINTPTR)eventpostbyevent2, 1, 0x61F1EA28, 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventpostbyevent2
	#define EXT_eventpostbyevent2
	#define GET_eventpostbyevent2(fl)  CAL_CMGETAPI( "eventpostbyevent2" ) 
	#define CAL_eventpostbyevent2  eventpostbyevent2
	#define CHK_eventpostbyevent2  TRUE
	#define EXP_eventpostbyevent2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent2", (RTS_UINTPTR)eventpostbyevent2, 1, 0x61F1EA28, 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventpostbyevent2  PFEVENTPOSTBYEVENT2_IEC pfeventpostbyevent2;
	#define EXT_eventpostbyevent2  extern PFEVENTPOSTBYEVENT2_IEC pfeventpostbyevent2;
	#define GET_eventpostbyevent2(fl)  s_pfCMGetAPI2( "eventpostbyevent2", (RTS_VOID_FCTPTR *)&pfeventpostbyevent2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x61F1EA28, 0x03050C00)
	#define CAL_eventpostbyevent2  pfeventpostbyevent2
	#define CHK_eventpostbyevent2  (pfeventpostbyevent2 != NULL)
	#define EXP_eventpostbyevent2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventpostbyevent2", (RTS_UINTPTR)eventpostbyevent2, 1, 0x61F1EA28, 0x03050C00) 
#endif


/**
 * Register an callback method to an event.
 * The callback must be an interface from an Iec function block!
 * Result will be set to ERR_DUPLICATE if pICallback is already registered. 
 */
typedef struct tageventregistercallback_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Handle to event */
	ICmpEventCallback *pICallback;		/* VAR_INPUT */	/* Interface ICmpEventCallback */
	RTS_IEC_RESULT *Result;				/* VAR_IN_OUT */	/* Error code */
	RTS_IEC_HANDLE EventRegisterCallback;	/* VAR_OUTPUT */	/* Handle to the callback that must be used to unregister with EventUnregisterCallback! */
} eventregistercallback_struct;

void CDECL CDECL_EXT eventregistercallback(eventregistercallback_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTREGISTERCALLBACK_IEC) (eventregistercallback_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTERCALLBACK_NOTIMPLEMENTED)
	#define USE_eventregistercallback
	#define EXT_eventregistercallback
	#define GET_eventregistercallback(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventregistercallback(p0) 
	#define CHK_eventregistercallback  FALSE
	#define EXP_eventregistercallback  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventregistercallback
	#define EXT_eventregistercallback
	#define GET_eventregistercallback(fl)  CAL_CMGETAPI( "eventregistercallback" ) 
	#define CAL_eventregistercallback  eventregistercallback
	#define CHK_eventregistercallback  TRUE
	#define EXP_eventregistercallback  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback", (RTS_UINTPTR)eventregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x5EF9AC91), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventregistercallback
	#define EXT_eventregistercallback
	#define GET_eventregistercallback(fl)  CAL_CMGETAPI( "eventregistercallback" ) 
	#define CAL_eventregistercallback  eventregistercallback
	#define CHK_eventregistercallback  TRUE
	#define EXP_eventregistercallback  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback", (RTS_UINTPTR)eventregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x5EF9AC91), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventregistercallback
	#define EXT_CmpEventMgreventregistercallback
	#define GET_CmpEventMgreventregistercallback  ERR_OK
	#define CAL_CmpEventMgreventregistercallback  eventregistercallback
	#define CHK_CmpEventMgreventregistercallback  TRUE
	#define EXP_CmpEventMgreventregistercallback  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback", (RTS_UINTPTR)eventregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x5EF9AC91), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventregistercallback
	#define EXT_eventregistercallback
	#define GET_eventregistercallback(fl)  CAL_CMGETAPI( "eventregistercallback" ) 
	#define CAL_eventregistercallback  eventregistercallback
	#define CHK_eventregistercallback  TRUE
	#define EXP_eventregistercallback  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback", (RTS_UINTPTR)eventregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x5EF9AC91), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventregistercallback  PFEVENTREGISTERCALLBACK_IEC pfeventregistercallback;
	#define EXT_eventregistercallback  extern PFEVENTREGISTERCALLBACK_IEC pfeventregistercallback;
	#define GET_eventregistercallback(fl)  s_pfCMGetAPI2( "eventregistercallback", (RTS_VOID_FCTPTR *)&pfeventregistercallback, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x5EF9AC91), 0x03050C00)
	#define CAL_eventregistercallback  pfeventregistercallback
	#define CHK_eventregistercallback  (pfeventregistercallback != NULL)
	#define EXP_eventregistercallback   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback", (RTS_UINTPTR)eventregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x5EF9AC91), 0x03050C00) 
#endif


/**
 * Register an callback method to an event.
 * The callback must be an interface from an Iec function block!
 * Result will be set to ERR_DUPLICATE if the combination of pICallback and pUserParameter is already registered. 
 */
typedef struct tageventregistercallback2_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Handle to event */
	ICmpEventCallback *pICallback;		/* VAR_INPUT */	/* Interface ICmpEventCallback */
	RTS_IEC_BYTE *pUserParameter;		/* VAR_INPUT */	/* Pointer to user parameter, that is transmitted to the callback (see EventParam) */
	RTS_IEC_RESULT *Result;				/* VAR_IN_OUT */	/* Error code */
	RTS_IEC_HANDLE EventRegisterCallback2;	/* VAR_OUTPUT */	/* Handle to the callback that must be used to unregister with EventUnregisterCallback! */
} eventregistercallback2_struct;

void CDECL CDECL_EXT eventregistercallback2(eventregistercallback2_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTREGISTERCALLBACK2_IEC) (eventregistercallback2_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTERCALLBACK2_NOTIMPLEMENTED)
	#define USE_eventregistercallback2
	#define EXT_eventregistercallback2
	#define GET_eventregistercallback2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventregistercallback2(p0) 
	#define CHK_eventregistercallback2  FALSE
	#define EXP_eventregistercallback2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventregistercallback2
	#define EXT_eventregistercallback2
	#define GET_eventregistercallback2(fl)  CAL_CMGETAPI( "eventregistercallback2" ) 
	#define CAL_eventregistercallback2  eventregistercallback2
	#define CHK_eventregistercallback2  TRUE
	#define EXP_eventregistercallback2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback2", (RTS_UINTPTR)eventregistercallback2, 1, RTSITF_GET_SIGNATURE(0, 0x95D8F44C), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventregistercallback2
	#define EXT_eventregistercallback2
	#define GET_eventregistercallback2(fl)  CAL_CMGETAPI( "eventregistercallback2" ) 
	#define CAL_eventregistercallback2  eventregistercallback2
	#define CHK_eventregistercallback2  TRUE
	#define EXP_eventregistercallback2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback2", (RTS_UINTPTR)eventregistercallback2, 1, RTSITF_GET_SIGNATURE(0, 0x95D8F44C), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventregistercallback2
	#define EXT_CmpEventMgreventregistercallback2
	#define GET_CmpEventMgreventregistercallback2  ERR_OK
	#define CAL_CmpEventMgreventregistercallback2  eventregistercallback2
	#define CHK_CmpEventMgreventregistercallback2  TRUE
	#define EXP_CmpEventMgreventregistercallback2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback2", (RTS_UINTPTR)eventregistercallback2, 1, RTSITF_GET_SIGNATURE(0, 0x95D8F44C), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventregistercallback2
	#define EXT_eventregistercallback2
	#define GET_eventregistercallback2(fl)  CAL_CMGETAPI( "eventregistercallback2" ) 
	#define CAL_eventregistercallback2  eventregistercallback2
	#define CHK_eventregistercallback2  TRUE
	#define EXP_eventregistercallback2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback2", (RTS_UINTPTR)eventregistercallback2, 1, RTSITF_GET_SIGNATURE(0, 0x95D8F44C), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventregistercallback2  PFEVENTREGISTERCALLBACK2_IEC pfeventregistercallback2;
	#define EXT_eventregistercallback2  extern PFEVENTREGISTERCALLBACK2_IEC pfeventregistercallback2;
	#define GET_eventregistercallback2(fl)  s_pfCMGetAPI2( "eventregistercallback2", (RTS_VOID_FCTPTR *)&pfeventregistercallback2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x95D8F44C), 0x03050C00)
	#define CAL_eventregistercallback2  pfeventregistercallback2
	#define CHK_eventregistercallback2  (pfeventregistercallback2 != NULL)
	#define EXP_eventregistercallback2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallback2", (RTS_UINTPTR)eventregistercallback2, 1, RTSITF_GET_SIGNATURE(0, 0x95D8F44C), 0x03050C00) 
#endif


/**
 * Register a callback function to an event. Callback is the address of an Iec function: ADR(function) 
 */
typedef struct tageventregistercallbackfunction_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Handle to event */
	RTS_IEC_BYTE *pfCallbackFunction;	/* VAR_INPUT */	/* Address of callback function. Prototype: same as ICmpEventCallback::EventCallback method
															  Function pointer is retrieved by the ADR operator: ADR(function) */
	RTS_IEC_RESULT EventRegisterCallbackFunction;	/* VAR_OUTPUT */	/* Error code */
} eventregistercallbackfunction_struct;

void CDECL CDECL_EXT eventregistercallbackfunction(eventregistercallbackfunction_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTREGISTERCALLBACKFUNCTION_IEC) (eventregistercallbackfunction_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTERCALLBACKFUNCTION_NOTIMPLEMENTED)
	#define USE_eventregistercallbackfunction
	#define EXT_eventregistercallbackfunction
	#define GET_eventregistercallbackfunction(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventregistercallbackfunction(p0) 
	#define CHK_eventregistercallbackfunction  FALSE
	#define EXP_eventregistercallbackfunction  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventregistercallbackfunction
	#define EXT_eventregistercallbackfunction
	#define GET_eventregistercallbackfunction(fl)  CAL_CMGETAPI( "eventregistercallbackfunction" ) 
	#define CAL_eventregistercallbackfunction  eventregistercallbackfunction
	#define CHK_eventregistercallbackfunction  TRUE
	#define EXP_eventregistercallbackfunction  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction", (RTS_UINTPTR)eventregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x989C8BB3), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventregistercallbackfunction
	#define EXT_eventregistercallbackfunction
	#define GET_eventregistercallbackfunction(fl)  CAL_CMGETAPI( "eventregistercallbackfunction" ) 
	#define CAL_eventregistercallbackfunction  eventregistercallbackfunction
	#define CHK_eventregistercallbackfunction  TRUE
	#define EXP_eventregistercallbackfunction  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction", (RTS_UINTPTR)eventregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x989C8BB3), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventregistercallbackfunction
	#define EXT_CmpEventMgreventregistercallbackfunction
	#define GET_CmpEventMgreventregistercallbackfunction  ERR_OK
	#define CAL_CmpEventMgreventregistercallbackfunction  eventregistercallbackfunction
	#define CHK_CmpEventMgreventregistercallbackfunction  TRUE
	#define EXP_CmpEventMgreventregistercallbackfunction  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction", (RTS_UINTPTR)eventregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x989C8BB3), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventregistercallbackfunction
	#define EXT_eventregistercallbackfunction
	#define GET_eventregistercallbackfunction(fl)  CAL_CMGETAPI( "eventregistercallbackfunction" ) 
	#define CAL_eventregistercallbackfunction  eventregistercallbackfunction
	#define CHK_eventregistercallbackfunction  TRUE
	#define EXP_eventregistercallbackfunction  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction", (RTS_UINTPTR)eventregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x989C8BB3), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventregistercallbackfunction  PFEVENTREGISTERCALLBACKFUNCTION_IEC pfeventregistercallbackfunction;
	#define EXT_eventregistercallbackfunction  extern PFEVENTREGISTERCALLBACKFUNCTION_IEC pfeventregistercallbackfunction;
	#define GET_eventregistercallbackfunction(fl)  s_pfCMGetAPI2( "eventregistercallbackfunction", (RTS_VOID_FCTPTR *)&pfeventregistercallbackfunction, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x989C8BB3), 0x03050C00)
	#define CAL_eventregistercallbackfunction  pfeventregistercallbackfunction
	#define CHK_eventregistercallbackfunction  (pfeventregistercallbackfunction != NULL)
	#define EXP_eventregistercallbackfunction   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction", (RTS_UINTPTR)eventregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x989C8BB3), 0x03050C00) 
#endif


/**
 * Register a callback function to an event. Callback is the address of an Iec function: ADR(function) 
 */
typedef struct tageventregistercallbackfunction2_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Handle to event */
	RTS_IEC_BYTE *pfCallbackFunction;	/* VAR_INPUT */	/* Address of callback function. Prototype: same as ICmpEventCallback::EventCallback method
															  Function pointer is retrieved by the ADR operator: ADR(function) */
	RTS_IEC_BYTE *pUserParameter;		/* VAR_INPUT */	/* Pointer to user parameter, that is transmitted optional to the callback (see EventParam) */
	RTS_IEC_RESULT EventRegisterCallbackFunction2;	/* VAR_OUTPUT */	/* Error code */
} eventregistercallbackfunction2_struct;

void CDECL CDECL_EXT eventregistercallbackfunction2(eventregistercallbackfunction2_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTREGISTERCALLBACKFUNCTION2_IEC) (eventregistercallbackfunction2_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTERCALLBACKFUNCTION2_NOTIMPLEMENTED)
	#define USE_eventregistercallbackfunction2
	#define EXT_eventregistercallbackfunction2
	#define GET_eventregistercallbackfunction2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventregistercallbackfunction2(p0) 
	#define CHK_eventregistercallbackfunction2  FALSE
	#define EXP_eventregistercallbackfunction2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventregistercallbackfunction2
	#define EXT_eventregistercallbackfunction2
	#define GET_eventregistercallbackfunction2(fl)  CAL_CMGETAPI( "eventregistercallbackfunction2" ) 
	#define CAL_eventregistercallbackfunction2  eventregistercallbackfunction2
	#define CHK_eventregistercallbackfunction2  TRUE
	#define EXP_eventregistercallbackfunction2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction2", (RTS_UINTPTR)eventregistercallbackfunction2, 1, RTSITF_GET_SIGNATURE(0, 0xBD946DD7), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventregistercallbackfunction2
	#define EXT_eventregistercallbackfunction2
	#define GET_eventregistercallbackfunction2(fl)  CAL_CMGETAPI( "eventregistercallbackfunction2" ) 
	#define CAL_eventregistercallbackfunction2  eventregistercallbackfunction2
	#define CHK_eventregistercallbackfunction2  TRUE
	#define EXP_eventregistercallbackfunction2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction2", (RTS_UINTPTR)eventregistercallbackfunction2, 1, RTSITF_GET_SIGNATURE(0, 0xBD946DD7), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventregistercallbackfunction2
	#define EXT_CmpEventMgreventregistercallbackfunction2
	#define GET_CmpEventMgreventregistercallbackfunction2  ERR_OK
	#define CAL_CmpEventMgreventregistercallbackfunction2  eventregistercallbackfunction2
	#define CHK_CmpEventMgreventregistercallbackfunction2  TRUE
	#define EXP_CmpEventMgreventregistercallbackfunction2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction2", (RTS_UINTPTR)eventregistercallbackfunction2, 1, RTSITF_GET_SIGNATURE(0, 0xBD946DD7), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventregistercallbackfunction2
	#define EXT_eventregistercallbackfunction2
	#define GET_eventregistercallbackfunction2(fl)  CAL_CMGETAPI( "eventregistercallbackfunction2" ) 
	#define CAL_eventregistercallbackfunction2  eventregistercallbackfunction2
	#define CHK_eventregistercallbackfunction2  TRUE
	#define EXP_eventregistercallbackfunction2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction2", (RTS_UINTPTR)eventregistercallbackfunction2, 1, RTSITF_GET_SIGNATURE(0, 0xBD946DD7), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventregistercallbackfunction2  PFEVENTREGISTERCALLBACKFUNCTION2_IEC pfeventregistercallbackfunction2;
	#define EXT_eventregistercallbackfunction2  extern PFEVENTREGISTERCALLBACKFUNCTION2_IEC pfeventregistercallbackfunction2;
	#define GET_eventregistercallbackfunction2(fl)  s_pfCMGetAPI2( "eventregistercallbackfunction2", (RTS_VOID_FCTPTR *)&pfeventregistercallbackfunction2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xBD946DD7), 0x03050C00)
	#define CAL_eventregistercallbackfunction2  pfeventregistercallbackfunction2
	#define CHK_eventregistercallbackfunction2  (pfeventregistercallbackfunction2 != NULL)
	#define EXP_eventregistercallbackfunction2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregistercallbackfunction2", (RTS_UINTPTR)eventregistercallbackfunction2, 1, RTSITF_GET_SIGNATURE(0, 0xBD946DD7), 0x03050C00) 
#endif


/**
 * Returns the number of registered callbacks on the event 
 */
typedef struct tageventregisteredcallbacks_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Event handle */
	RTS_IEC_RESULT *Result;				/* VAR_IN_OUT */	/* Error code */
	RTS_IEC_UDINT EventRegisteredCallbacks;	/* VAR_OUTPUT */	
} eventregisteredcallbacks_struct;

void CDECL CDECL_EXT eventregisteredcallbacks(eventregisteredcallbacks_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTREGISTEREDCALLBACKS_IEC) (eventregisteredcallbacks_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTEREDCALLBACKS_NOTIMPLEMENTED)
	#define USE_eventregisteredcallbacks
	#define EXT_eventregisteredcallbacks
	#define GET_eventregisteredcallbacks(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventregisteredcallbacks(p0) 
	#define CHK_eventregisteredcallbacks  FALSE
	#define EXP_eventregisteredcallbacks  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventregisteredcallbacks
	#define EXT_eventregisteredcallbacks
	#define GET_eventregisteredcallbacks(fl)  CAL_CMGETAPI( "eventregisteredcallbacks" ) 
	#define CAL_eventregisteredcallbacks  eventregisteredcallbacks
	#define CHK_eventregisteredcallbacks  TRUE
	#define EXP_eventregisteredcallbacks  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregisteredcallbacks", (RTS_UINTPTR)eventregisteredcallbacks, 1, RTSITF_GET_SIGNATURE(0, 0xC4EA0B3B), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventregisteredcallbacks
	#define EXT_eventregisteredcallbacks
	#define GET_eventregisteredcallbacks(fl)  CAL_CMGETAPI( "eventregisteredcallbacks" ) 
	#define CAL_eventregisteredcallbacks  eventregisteredcallbacks
	#define CHK_eventregisteredcallbacks  TRUE
	#define EXP_eventregisteredcallbacks  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregisteredcallbacks", (RTS_UINTPTR)eventregisteredcallbacks, 1, RTSITF_GET_SIGNATURE(0, 0xC4EA0B3B), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventregisteredcallbacks
	#define EXT_CmpEventMgreventregisteredcallbacks
	#define GET_CmpEventMgreventregisteredcallbacks  ERR_OK
	#define CAL_CmpEventMgreventregisteredcallbacks  eventregisteredcallbacks
	#define CHK_CmpEventMgreventregisteredcallbacks  TRUE
	#define EXP_CmpEventMgreventregisteredcallbacks  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregisteredcallbacks", (RTS_UINTPTR)eventregisteredcallbacks, 1, RTSITF_GET_SIGNATURE(0, 0xC4EA0B3B), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventregisteredcallbacks
	#define EXT_eventregisteredcallbacks
	#define GET_eventregisteredcallbacks(fl)  CAL_CMGETAPI( "eventregisteredcallbacks" ) 
	#define CAL_eventregisteredcallbacks  eventregisteredcallbacks
	#define CHK_eventregisteredcallbacks  TRUE
	#define EXP_eventregisteredcallbacks  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregisteredcallbacks", (RTS_UINTPTR)eventregisteredcallbacks, 1, RTSITF_GET_SIGNATURE(0, 0xC4EA0B3B), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventregisteredcallbacks  PFEVENTREGISTEREDCALLBACKS_IEC pfeventregisteredcallbacks;
	#define EXT_eventregisteredcallbacks  extern PFEVENTREGISTEREDCALLBACKS_IEC pfeventregisteredcallbacks;
	#define GET_eventregisteredcallbacks(fl)  s_pfCMGetAPI2( "eventregisteredcallbacks", (RTS_VOID_FCTPTR *)&pfeventregisteredcallbacks, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC4EA0B3B), 0x03050C00)
	#define CAL_eventregisteredcallbacks  pfeventregisteredcallbacks
	#define CHK_eventregisteredcallbacks  (pfeventregisteredcallbacks != NULL)
	#define EXP_eventregisteredcallbacks   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventregisteredcallbacks", (RTS_UINTPTR)eventregisteredcallbacks, 1, RTSITF_GET_SIGNATURE(0, 0xC4EA0B3B), 0x03050C00) 
#endif


/**
 * Unregister a callback interface from an event specified by handle and callback interface 
 */
typedef struct tageventunregistercallback_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Event handle */
	RTS_IEC_HANDLE hInterface;			/* VAR_INPUT */	/* Callback handle is returned by EventRegisterCallback() or EventRegisterCallback2()!!! */
	RTS_IEC_RESULT EventUnregisterCallback;	/* VAR_OUTPUT */	/* Error code */
} eventunregistercallback_struct;

void CDECL CDECL_EXT eventunregistercallback(eventunregistercallback_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTUNREGISTERCALLBACK_IEC) (eventunregistercallback_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTUNREGISTERCALLBACK_NOTIMPLEMENTED)
	#define USE_eventunregistercallback
	#define EXT_eventunregistercallback
	#define GET_eventunregistercallback(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventunregistercallback(p0) 
	#define CHK_eventunregistercallback  FALSE
	#define EXP_eventunregistercallback  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventunregistercallback
	#define EXT_eventunregistercallback
	#define GET_eventunregistercallback(fl)  CAL_CMGETAPI( "eventunregistercallback" ) 
	#define CAL_eventunregistercallback  eventunregistercallback
	#define CHK_eventunregistercallback  TRUE
	#define EXP_eventunregistercallback  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallback", (RTS_UINTPTR)eventunregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x80238B4F), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventunregistercallback
	#define EXT_eventunregistercallback
	#define GET_eventunregistercallback(fl)  CAL_CMGETAPI( "eventunregistercallback" ) 
	#define CAL_eventunregistercallback  eventunregistercallback
	#define CHK_eventunregistercallback  TRUE
	#define EXP_eventunregistercallback  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallback", (RTS_UINTPTR)eventunregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x80238B4F), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventunregistercallback
	#define EXT_CmpEventMgreventunregistercallback
	#define GET_CmpEventMgreventunregistercallback  ERR_OK
	#define CAL_CmpEventMgreventunregistercallback  eventunregistercallback
	#define CHK_CmpEventMgreventunregistercallback  TRUE
	#define EXP_CmpEventMgreventunregistercallback  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallback", (RTS_UINTPTR)eventunregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x80238B4F), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventunregistercallback
	#define EXT_eventunregistercallback
	#define GET_eventunregistercallback(fl)  CAL_CMGETAPI( "eventunregistercallback" ) 
	#define CAL_eventunregistercallback  eventunregistercallback
	#define CHK_eventunregistercallback  TRUE
	#define EXP_eventunregistercallback  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallback", (RTS_UINTPTR)eventunregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x80238B4F), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventunregistercallback  PFEVENTUNREGISTERCALLBACK_IEC pfeventunregistercallback;
	#define EXT_eventunregistercallback  extern PFEVENTUNREGISTERCALLBACK_IEC pfeventunregistercallback;
	#define GET_eventunregistercallback(fl)  s_pfCMGetAPI2( "eventunregistercallback", (RTS_VOID_FCTPTR *)&pfeventunregistercallback, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x80238B4F), 0x03050C00)
	#define CAL_eventunregistercallback  pfeventunregistercallback
	#define CHK_eventunregistercallback  (pfeventunregistercallback != NULL)
	#define EXP_eventunregistercallback   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallback", (RTS_UINTPTR)eventunregistercallback, 1, RTSITF_GET_SIGNATURE(0, 0x80238B4F), 0x03050C00) 
#endif


/**
 * Unregister a callback function from an event specified by handle and callback 
 */
typedef struct tageventunregistercallbackfunction_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Handle to event */
	RTS_IEC_BYTE *pfCallbackFunction;	/* VAR_INPUT */	/* Address of callback function. Function pointer is retrieved by the ADR operator: ADR(function) */
	RTS_IEC_RESULT EventUnregisterCallbackFunction;	/* VAR_OUTPUT */	/* Error code */
} eventunregistercallbackfunction_struct;

void CDECL CDECL_EXT eventunregistercallbackfunction(eventunregistercallbackfunction_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTUNREGISTERCALLBACKFUNCTION_IEC) (eventunregistercallbackfunction_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTUNREGISTERCALLBACKFUNCTION_NOTIMPLEMENTED)
	#define USE_eventunregistercallbackfunction
	#define EXT_eventunregistercallbackfunction
	#define GET_eventunregistercallbackfunction(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventunregistercallbackfunction(p0) 
	#define CHK_eventunregistercallbackfunction  FALSE
	#define EXP_eventunregistercallbackfunction  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventunregistercallbackfunction
	#define EXT_eventunregistercallbackfunction
	#define GET_eventunregistercallbackfunction(fl)  CAL_CMGETAPI( "eventunregistercallbackfunction" ) 
	#define CAL_eventunregistercallbackfunction  eventunregistercallbackfunction
	#define CHK_eventunregistercallbackfunction  TRUE
	#define EXP_eventunregistercallbackfunction  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction", (RTS_UINTPTR)eventunregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x5E0DD5BD), 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventunregistercallbackfunction
	#define EXT_eventunregistercallbackfunction
	#define GET_eventunregistercallbackfunction(fl)  CAL_CMGETAPI( "eventunregistercallbackfunction" ) 
	#define CAL_eventunregistercallbackfunction  eventunregistercallbackfunction
	#define CHK_eventunregistercallbackfunction  TRUE
	#define EXP_eventunregistercallbackfunction  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction", (RTS_UINTPTR)eventunregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x5E0DD5BD), 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventunregistercallbackfunction
	#define EXT_CmpEventMgreventunregistercallbackfunction
	#define GET_CmpEventMgreventunregistercallbackfunction  ERR_OK
	#define CAL_CmpEventMgreventunregistercallbackfunction  eventunregistercallbackfunction
	#define CHK_CmpEventMgreventunregistercallbackfunction  TRUE
	#define EXP_CmpEventMgreventunregistercallbackfunction  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction", (RTS_UINTPTR)eventunregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x5E0DD5BD), 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventunregistercallbackfunction
	#define EXT_eventunregistercallbackfunction
	#define GET_eventunregistercallbackfunction(fl)  CAL_CMGETAPI( "eventunregistercallbackfunction" ) 
	#define CAL_eventunregistercallbackfunction  eventunregistercallbackfunction
	#define CHK_eventunregistercallbackfunction  TRUE
	#define EXP_eventunregistercallbackfunction  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction", (RTS_UINTPTR)eventunregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x5E0DD5BD), 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventunregistercallbackfunction  PFEVENTUNREGISTERCALLBACKFUNCTION_IEC pfeventunregistercallbackfunction;
	#define EXT_eventunregistercallbackfunction  extern PFEVENTUNREGISTERCALLBACKFUNCTION_IEC pfeventunregistercallbackfunction;
	#define GET_eventunregistercallbackfunction(fl)  s_pfCMGetAPI2( "eventunregistercallbackfunction", (RTS_VOID_FCTPTR *)&pfeventunregistercallbackfunction, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x5E0DD5BD), 0x03050C00)
	#define CAL_eventunregistercallbackfunction  pfeventunregistercallbackfunction
	#define CHK_eventunregistercallbackfunction  (pfeventunregistercallbackfunction != NULL)
	#define EXP_eventunregistercallbackfunction   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction", (RTS_UINTPTR)eventunregistercallbackfunction, 1, RTSITF_GET_SIGNATURE(0, 0x5E0DD5BD), 0x03050C00) 
#endif


/**
 * Unregister a callback function with a specific user parameter from an event specified by handle and callback and parameter 
 */
typedef struct tageventunregistercallbackfunction2_struct
{
	RTS_IEC_HANDLE hEvent;				/* VAR_INPUT */	/* Handle to event */
	RTS_IEC_BYTE *pfCallbackFunction;	/* VAR_INPUT */	/* Address of callback function. Function pointer is retrieved by the ADR operator: ADR(function) */
	RTS_IEC_BYTE *pUserParameter;		/* VAR_INPUT */	/* Pointer to user parameter, that was specified as a parameter at EventRegisterCallback2() */
	RTS_IEC_RESULT EventUnregisterCallbackFunction2;	/* VAR_OUTPUT */	/* Error code */
} eventunregistercallbackfunction2_struct;

void CDECL CDECL_EXT eventunregistercallbackfunction2(eventunregistercallbackfunction2_struct *p);
typedef void (CDECL CDECL_EXT* PFEVENTUNREGISTERCALLBACKFUNCTION2_IEC) (eventunregistercallbackfunction2_struct *p);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTUNREGISTERCALLBACKFUNCTION2_NOTIMPLEMENTED)
	#define USE_eventunregistercallbackfunction2
	#define EXT_eventunregistercallbackfunction2
	#define GET_eventunregistercallbackfunction2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_eventunregistercallbackfunction2(p0) 
	#define CHK_eventunregistercallbackfunction2  FALSE
	#define EXP_eventunregistercallbackfunction2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_eventunregistercallbackfunction2
	#define EXT_eventunregistercallbackfunction2
	#define GET_eventunregistercallbackfunction2(fl)  CAL_CMGETAPI( "eventunregistercallbackfunction2" ) 
	#define CAL_eventunregistercallbackfunction2  eventunregistercallbackfunction2
	#define CHK_eventunregistercallbackfunction2  TRUE
	#define EXP_eventunregistercallbackfunction2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction2", (RTS_UINTPTR)eventunregistercallbackfunction2, 1, 0x779853A9, 0x03050C00) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_eventunregistercallbackfunction2
	#define EXT_eventunregistercallbackfunction2
	#define GET_eventunregistercallbackfunction2(fl)  CAL_CMGETAPI( "eventunregistercallbackfunction2" ) 
	#define CAL_eventunregistercallbackfunction2  eventunregistercallbackfunction2
	#define CHK_eventunregistercallbackfunction2  TRUE
	#define EXP_eventunregistercallbackfunction2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction2", (RTS_UINTPTR)eventunregistercallbackfunction2, 1, 0x779853A9, 0x03050C00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgreventunregistercallbackfunction2
	#define EXT_CmpEventMgreventunregistercallbackfunction2
	#define GET_CmpEventMgreventunregistercallbackfunction2  ERR_OK
	#define CAL_CmpEventMgreventunregistercallbackfunction2  eventunregistercallbackfunction2
	#define CHK_CmpEventMgreventunregistercallbackfunction2  TRUE
	#define EXP_CmpEventMgreventunregistercallbackfunction2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction2", (RTS_UINTPTR)eventunregistercallbackfunction2, 1, 0x779853A9, 0x03050C00) 
#elif defined(CPLUSPLUS)
	#define USE_eventunregistercallbackfunction2
	#define EXT_eventunregistercallbackfunction2
	#define GET_eventunregistercallbackfunction2(fl)  CAL_CMGETAPI( "eventunregistercallbackfunction2" ) 
	#define CAL_eventunregistercallbackfunction2  eventunregistercallbackfunction2
	#define CHK_eventunregistercallbackfunction2  TRUE
	#define EXP_eventunregistercallbackfunction2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction2", (RTS_UINTPTR)eventunregistercallbackfunction2, 1, 0x779853A9, 0x03050C00) 
#else /* DYNAMIC_LINK */
	#define USE_eventunregistercallbackfunction2  PFEVENTUNREGISTERCALLBACKFUNCTION2_IEC pfeventunregistercallbackfunction2;
	#define EXT_eventunregistercallbackfunction2  extern PFEVENTUNREGISTERCALLBACKFUNCTION2_IEC pfeventunregistercallbackfunction2;
	#define GET_eventunregistercallbackfunction2(fl)  s_pfCMGetAPI2( "eventunregistercallbackfunction2", (RTS_VOID_FCTPTR *)&pfeventunregistercallbackfunction2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x779853A9, 0x03050C00)
	#define CAL_eventunregistercallbackfunction2  pfeventunregistercallbackfunction2
	#define CHK_eventunregistercallbackfunction2  (pfeventunregistercallbackfunction2 != NULL)
	#define EXP_eventunregistercallbackfunction2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"eventunregistercallbackfunction2", (RTS_UINTPTR)eventunregistercallbackfunction2, 1, 0x779853A9, 0x03050C00) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Creates a new event object. If event still exists, a handle to this object will be returned.
 * An event is typically created by the provider of an event in the CH_INIT_DONE hook.</description>
 * <param name="EventId" type="IN">Event ID of the event. Contains the class and the event</param>
 * <param name="CmpIdProvider" type="IN">Component ID of the provider</param>
 * <param name="pResult" type="OUT">Pointer to the error code</param>
 * <result>Handle to the event object</result>
 */
RTS_HANDLE CDECL EventCreate(EVENTID EventId, CMPID CmpIdProvider, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFEVENTCREATE) (EVENTID EventId, CMPID CmpIdProvider, RTS_RESULT *pResult);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTCREATE_NOTIMPLEMENTED)
	#define USE_EventCreate
	#define EXT_EventCreate
	#define GET_EventCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventCreate(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_EventCreate  FALSE
	#define EXP_EventCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventCreate
	#define EXT_EventCreate
	#define GET_EventCreate(fl)  CAL_CMGETAPI( "EventCreate" ) 
	#define CAL_EventCreate  EventCreate
	#define CHK_EventCreate  TRUE
	#define EXP_EventCreate  CAL_CMEXPAPI( "EventCreate" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventCreate
	#define EXT_EventCreate
	#define GET_EventCreate(fl)  CAL_CMGETAPI( "EventCreate" ) 
	#define CAL_EventCreate  EventCreate
	#define CHK_EventCreate  TRUE
	#define EXP_EventCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventCreate", (RTS_UINTPTR)EventCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventCreate
	#define EXT_CmpEventMgrEventCreate
	#define GET_CmpEventMgrEventCreate  ERR_OK
	#define CAL_CmpEventMgrEventCreate pICmpEventMgr->IEventCreate
	#define CHK_CmpEventMgrEventCreate (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventCreate
	#define EXT_EventCreate
	#define GET_EventCreate(fl)  CAL_CMGETAPI( "EventCreate" ) 
	#define CAL_EventCreate pICmpEventMgr->IEventCreate
	#define CHK_EventCreate (pICmpEventMgr != NULL)
	#define EXP_EventCreate  CAL_CMEXPAPI( "EventCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventCreate  PFEVENTCREATE pfEventCreate;
	#define EXT_EventCreate  extern PFEVENTCREATE pfEventCreate;
	#define GET_EventCreate(fl)  s_pfCMGetAPI2( "EventCreate", (RTS_VOID_FCTPTR *)&pfEventCreate, (fl), 0, 0)
	#define CAL_EventCreate  pfEventCreate
	#define CHK_EventCreate  (pfEventCreate != NULL)
	#define EXP_EventCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventCreate", (RTS_UINTPTR)EventCreate, 0, 0) 
#endif




/**
 * <description>Creates a new event object. If event still exists, a handle to this object will be returned.
 * An event is typically created by the provider of an event in the CH_INIT_DONE hook.</description>
 * <param name="EventId" type="IN">Event ID of the event. Contains the class and the event</param>
 * <param name="CmpIdProvider" type="IN">Component ID of the provider</param>
 * <param name="nCallbacksPossible" type="IN">Maximum number of callbacks possible on this event or EVENT_CALLBACKS_NO_LIMIT for no limit</param>
 * <param name="pResult" type="OUT">Pointer to the error code</param>
 * <result>Handle to the event object</result>
 */
RTS_HANDLE CDECL EventCreate2(EVENTID EventId, CMPID CmpIdProvider, RTS_UI32 nCallbacksPossible, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFEVENTCREATE2) (EVENTID EventId, CMPID CmpIdProvider, RTS_UI32 nCallbacksPossible, RTS_RESULT *pResult);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTCREATE2_NOTIMPLEMENTED)
	#define USE_EventCreate2
	#define EXT_EventCreate2
	#define GET_EventCreate2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventCreate2(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_EventCreate2  FALSE
	#define EXP_EventCreate2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventCreate2
	#define EXT_EventCreate2
	#define GET_EventCreate2(fl)  CAL_CMGETAPI( "EventCreate2" ) 
	#define CAL_EventCreate2  EventCreate2
	#define CHK_EventCreate2  TRUE
	#define EXP_EventCreate2  CAL_CMEXPAPI( "EventCreate2" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventCreate2
	#define EXT_EventCreate2
	#define GET_EventCreate2(fl)  CAL_CMGETAPI( "EventCreate2" ) 
	#define CAL_EventCreate2  EventCreate2
	#define CHK_EventCreate2  TRUE
	#define EXP_EventCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventCreate2", (RTS_UINTPTR)EventCreate2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventCreate2
	#define EXT_CmpEventMgrEventCreate2
	#define GET_CmpEventMgrEventCreate2  ERR_OK
	#define CAL_CmpEventMgrEventCreate2 pICmpEventMgr->IEventCreate2
	#define CHK_CmpEventMgrEventCreate2 (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventCreate2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventCreate2
	#define EXT_EventCreate2
	#define GET_EventCreate2(fl)  CAL_CMGETAPI( "EventCreate2" ) 
	#define CAL_EventCreate2 pICmpEventMgr->IEventCreate2
	#define CHK_EventCreate2 (pICmpEventMgr != NULL)
	#define EXP_EventCreate2  CAL_CMEXPAPI( "EventCreate2" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventCreate2  PFEVENTCREATE2 pfEventCreate2;
	#define EXT_EventCreate2  extern PFEVENTCREATE2 pfEventCreate2;
	#define GET_EventCreate2(fl)  s_pfCMGetAPI2( "EventCreate2", (RTS_VOID_FCTPTR *)&pfEventCreate2, (fl), 0, 0)
	#define CAL_EventCreate2  pfEventCreate2
	#define CHK_EventCreate2  (pfEventCreate2 != NULL)
	#define EXP_EventCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventCreate2", (RTS_UINTPTR)EventCreate2, 0, 0) 
#endif




/**
 * <description>Creates a new event object. If event still exists, a handle to this object will be returned.
 * An event is typically created by the provider of an event in the CH_INIT_DONE hook.</description>
 * <param name="EventId" type="IN">Event ID of the event. Contains the class and the event</param>
 * <param name="CmpIdProvider" type="IN">Component ID of the provider</param>
 * <param name="pEventOptions" type="IN">Pointer to the EventOptions. May be NULL, then EventCreate() will be called without EventOptions.</param>
 * <param name="pResult" type="OUT">Pointer to the error code</param>
 * <result>Handle to the event object</result>
 */
RTS_HANDLE CDECL EventCreate3(EVENTID EventId, CMPID CmpIdProvider, EventOptions *pEventOptions, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFEVENTCREATE3) (EVENTID EventId, CMPID CmpIdProvider, EventOptions *pEventOptions, RTS_RESULT *pResult);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTCREATE3_NOTIMPLEMENTED)
	#define USE_EventCreate3
	#define EXT_EventCreate3
	#define GET_EventCreate3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventCreate3(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_EventCreate3  FALSE
	#define EXP_EventCreate3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventCreate3
	#define EXT_EventCreate3
	#define GET_EventCreate3(fl)  CAL_CMGETAPI( "EventCreate3" ) 
	#define CAL_EventCreate3  EventCreate3
	#define CHK_EventCreate3  TRUE
	#define EXP_EventCreate3  CAL_CMEXPAPI( "EventCreate3" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventCreate3
	#define EXT_EventCreate3
	#define GET_EventCreate3(fl)  CAL_CMGETAPI( "EventCreate3" ) 
	#define CAL_EventCreate3  EventCreate3
	#define CHK_EventCreate3  TRUE
	#define EXP_EventCreate3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventCreate3", (RTS_UINTPTR)EventCreate3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventCreate3
	#define EXT_CmpEventMgrEventCreate3
	#define GET_CmpEventMgrEventCreate3  ERR_OK
	#define CAL_CmpEventMgrEventCreate3 pICmpEventMgr->IEventCreate3
	#define CHK_CmpEventMgrEventCreate3 (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventCreate3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventCreate3
	#define EXT_EventCreate3
	#define GET_EventCreate3(fl)  CAL_CMGETAPI( "EventCreate3" ) 
	#define CAL_EventCreate3 pICmpEventMgr->IEventCreate3
	#define CHK_EventCreate3 (pICmpEventMgr != NULL)
	#define EXP_EventCreate3  CAL_CMEXPAPI( "EventCreate3" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventCreate3  PFEVENTCREATE3 pfEventCreate3;
	#define EXT_EventCreate3  extern PFEVENTCREATE3 pfEventCreate3;
	#define GET_EventCreate3(fl)  s_pfCMGetAPI2( "EventCreate3", (RTS_VOID_FCTPTR *)&pfEventCreate3, (fl), 0, 0)
	#define CAL_EventCreate3  pfEventCreate3
	#define CHK_EventCreate3  (pfEventCreate3 != NULL)
	#define EXP_EventCreate3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventCreate3", (RTS_UINTPTR)EventCreate3, 0, 0) 
#endif




/**
 * <description>Deletes an event specified by handle </description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventDelete(RTS_HANDLE hEvent);
typedef RTS_RESULT (CDECL * PFEVENTDELETE) (RTS_HANDLE hEvent);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTDELETE_NOTIMPLEMENTED)
	#define USE_EventDelete
	#define EXT_EventDelete
	#define GET_EventDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventDelete(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventDelete  FALSE
	#define EXP_EventDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventDelete
	#define EXT_EventDelete
	#define GET_EventDelete(fl)  CAL_CMGETAPI( "EventDelete" ) 
	#define CAL_EventDelete  EventDelete
	#define CHK_EventDelete  TRUE
	#define EXP_EventDelete  CAL_CMEXPAPI( "EventDelete" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventDelete
	#define EXT_EventDelete
	#define GET_EventDelete(fl)  CAL_CMGETAPI( "EventDelete" ) 
	#define CAL_EventDelete  EventDelete
	#define CHK_EventDelete  TRUE
	#define EXP_EventDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventDelete", (RTS_UINTPTR)EventDelete, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventDelete
	#define EXT_CmpEventMgrEventDelete
	#define GET_CmpEventMgrEventDelete  ERR_OK
	#define CAL_CmpEventMgrEventDelete pICmpEventMgr->IEventDelete
	#define CHK_CmpEventMgrEventDelete (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventDelete
	#define EXT_EventDelete
	#define GET_EventDelete(fl)  CAL_CMGETAPI( "EventDelete" ) 
	#define CAL_EventDelete pICmpEventMgr->IEventDelete
	#define CHK_EventDelete (pICmpEventMgr != NULL)
	#define EXP_EventDelete  CAL_CMEXPAPI( "EventDelete" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventDelete  PFEVENTDELETE pfEventDelete;
	#define EXT_EventDelete  extern PFEVENTDELETE pfEventDelete;
	#define GET_EventDelete(fl)  s_pfCMGetAPI2( "EventDelete", (RTS_VOID_FCTPTR *)&pfEventDelete, (fl), 0, 0)
	#define CAL_EventDelete  pfEventDelete
	#define CHK_EventDelete  (pfEventDelete != NULL)
	#define EXP_EventDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventDelete", (RTS_UINTPTR)EventDelete, 0, 0) 
#endif




/**
 * <description>Delete all registered events and the registered callbacks</description>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventDeleteAll(void);
typedef RTS_RESULT (CDECL * PFEVENTDELETEALL) (void);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTDELETEALL_NOTIMPLEMENTED)
	#define USE_EventDeleteAll
	#define EXT_EventDeleteAll
	#define GET_EventDeleteAll(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventDeleteAll()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventDeleteAll  FALSE
	#define EXP_EventDeleteAll  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventDeleteAll
	#define EXT_EventDeleteAll
	#define GET_EventDeleteAll(fl)  CAL_CMGETAPI( "EventDeleteAll" ) 
	#define CAL_EventDeleteAll  EventDeleteAll
	#define CHK_EventDeleteAll  TRUE
	#define EXP_EventDeleteAll  CAL_CMEXPAPI( "EventDeleteAll" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventDeleteAll
	#define EXT_EventDeleteAll
	#define GET_EventDeleteAll(fl)  CAL_CMGETAPI( "EventDeleteAll" ) 
	#define CAL_EventDeleteAll  EventDeleteAll
	#define CHK_EventDeleteAll  TRUE
	#define EXP_EventDeleteAll  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventDeleteAll", (RTS_UINTPTR)EventDeleteAll, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventDeleteAll
	#define EXT_CmpEventMgrEventDeleteAll
	#define GET_CmpEventMgrEventDeleteAll  ERR_OK
	#define CAL_CmpEventMgrEventDeleteAll pICmpEventMgr->IEventDeleteAll
	#define CHK_CmpEventMgrEventDeleteAll (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventDeleteAll  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventDeleteAll
	#define EXT_EventDeleteAll
	#define GET_EventDeleteAll(fl)  CAL_CMGETAPI( "EventDeleteAll" ) 
	#define CAL_EventDeleteAll pICmpEventMgr->IEventDeleteAll
	#define CHK_EventDeleteAll (pICmpEventMgr != NULL)
	#define EXP_EventDeleteAll  CAL_CMEXPAPI( "EventDeleteAll" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventDeleteAll  PFEVENTDELETEALL pfEventDeleteAll;
	#define EXT_EventDeleteAll  extern PFEVENTDELETEALL pfEventDeleteAll;
	#define GET_EventDeleteAll(fl)  s_pfCMGetAPI2( "EventDeleteAll", (RTS_VOID_FCTPTR *)&pfEventDeleteAll, (fl), 0, 0)
	#define CAL_EventDeleteAll  pfEventDeleteAll
	#define CHK_EventDeleteAll  (pfEventDeleteAll != NULL)
	#define EXP_EventDeleteAll  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventDeleteAll", (RTS_UINTPTR)EventDeleteAll, 0, 0) 
#endif




/**
 * <description><p>Opens an existing event object. Can be used to check, if the event was created by the provider.
 * If the event does not exist, an error code is returned.</p>
 * Typically an event is opened by the consumer of an event in the CH_INIT_DONE2 hook.</description>
 * <param name="EventId" type="IN">Event ID of the event. Contains the class and the event</param>
 * <param name="CmpIdProvider" type="IN">Component ID of the provider</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the event object</result>
 */
RTS_HANDLE CDECL EventOpen(EVENTID EventId, CMPID CmpIdProvider, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFEVENTOPEN) (EVENTID EventId, CMPID CmpIdProvider, RTS_RESULT *pResult);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTOPEN_NOTIMPLEMENTED)
	#define USE_EventOpen
	#define EXT_EventOpen
	#define GET_EventOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventOpen(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_EventOpen  FALSE
	#define EXP_EventOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventOpen
	#define EXT_EventOpen
	#define GET_EventOpen(fl)  CAL_CMGETAPI( "EventOpen" ) 
	#define CAL_EventOpen  EventOpen
	#define CHK_EventOpen  TRUE
	#define EXP_EventOpen  CAL_CMEXPAPI( "EventOpen" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventOpen
	#define EXT_EventOpen
	#define GET_EventOpen(fl)  CAL_CMGETAPI( "EventOpen" ) 
	#define CAL_EventOpen  EventOpen
	#define CHK_EventOpen  TRUE
	#define EXP_EventOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventOpen", (RTS_UINTPTR)EventOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventOpen
	#define EXT_CmpEventMgrEventOpen
	#define GET_CmpEventMgrEventOpen  ERR_OK
	#define CAL_CmpEventMgrEventOpen pICmpEventMgr->IEventOpen
	#define CHK_CmpEventMgrEventOpen (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventOpen
	#define EXT_EventOpen
	#define GET_EventOpen(fl)  CAL_CMGETAPI( "EventOpen" ) 
	#define CAL_EventOpen pICmpEventMgr->IEventOpen
	#define CHK_EventOpen (pICmpEventMgr != NULL)
	#define EXP_EventOpen  CAL_CMEXPAPI( "EventOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventOpen  PFEVENTOPEN pfEventOpen;
	#define EXT_EventOpen  extern PFEVENTOPEN pfEventOpen;
	#define GET_EventOpen(fl)  s_pfCMGetAPI2( "EventOpen", (RTS_VOID_FCTPTR *)&pfEventOpen, (fl), 0, 0)
	#define CAL_EventOpen  pfEventOpen
	#define CHK_EventOpen  (pfEventOpen != NULL)
	#define EXP_EventOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventOpen", (RTS_UINTPTR)EventOpen, 0, 0) 
#endif




/**
 * <description>Close an event specified by handle</description>
 * <param name="hEvent" type="IN">Handle to the event</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventClose(RTS_HANDLE hEvent);
typedef RTS_RESULT (CDECL * PFEVENTCLOSE) (RTS_HANDLE hEvent);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTCLOSE_NOTIMPLEMENTED)
	#define USE_EventClose
	#define EXT_EventClose
	#define GET_EventClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventClose  FALSE
	#define EXP_EventClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventClose
	#define EXT_EventClose
	#define GET_EventClose(fl)  CAL_CMGETAPI( "EventClose" ) 
	#define CAL_EventClose  EventClose
	#define CHK_EventClose  TRUE
	#define EXP_EventClose  CAL_CMEXPAPI( "EventClose" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventClose
	#define EXT_EventClose
	#define GET_EventClose(fl)  CAL_CMGETAPI( "EventClose" ) 
	#define CAL_EventClose  EventClose
	#define CHK_EventClose  TRUE
	#define EXP_EventClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventClose", (RTS_UINTPTR)EventClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventClose
	#define EXT_CmpEventMgrEventClose
	#define GET_CmpEventMgrEventClose  ERR_OK
	#define CAL_CmpEventMgrEventClose pICmpEventMgr->IEventClose
	#define CHK_CmpEventMgrEventClose (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventClose
	#define EXT_EventClose
	#define GET_EventClose(fl)  CAL_CMGETAPI( "EventClose" ) 
	#define CAL_EventClose pICmpEventMgr->IEventClose
	#define CHK_EventClose (pICmpEventMgr != NULL)
	#define EXP_EventClose  CAL_CMEXPAPI( "EventClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventClose  PFEVENTCLOSE pfEventClose;
	#define EXT_EventClose  extern PFEVENTCLOSE pfEventClose;
	#define GET_EventClose(fl)  s_pfCMGetAPI2( "EventClose", (RTS_VOID_FCTPTR *)&pfEventClose, (fl), 0, 0)
	#define CAL_EventClose  pfEventClose
	#define CHK_EventClose  (pfEventClose != NULL)
	#define EXP_EventClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventClose", (RTS_UINTPTR)EventClose, 0, 0) 
#endif




/**
 * <description>Extract the event from eventid</description>
 * <param name="EventId" type="IN">Event ID</param>
 * <result>Event. Is specified in the interface of each component</result>
 */
unsigned short CDECL EventGetEvent(EVENTID EventId);
typedef unsigned short (CDECL * PFEVENTGETEVENT) (EVENTID EventId);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTGETEVENT_NOTIMPLEMENTED)
	#define USE_EventGetEvent
	#define EXT_EventGetEvent
	#define GET_EventGetEvent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventGetEvent(p0)  (unsigned short)ERR_NOTIMPLEMENTED
	#define CHK_EventGetEvent  FALSE
	#define EXP_EventGetEvent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventGetEvent
	#define EXT_EventGetEvent
	#define GET_EventGetEvent(fl)  CAL_CMGETAPI( "EventGetEvent" ) 
	#define CAL_EventGetEvent  EventGetEvent
	#define CHK_EventGetEvent  TRUE
	#define EXP_EventGetEvent  CAL_CMEXPAPI( "EventGetEvent" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventGetEvent
	#define EXT_EventGetEvent
	#define GET_EventGetEvent(fl)  CAL_CMGETAPI( "EventGetEvent" ) 
	#define CAL_EventGetEvent  EventGetEvent
	#define CHK_EventGetEvent  TRUE
	#define EXP_EventGetEvent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventGetEvent", (RTS_UINTPTR)EventGetEvent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventGetEvent
	#define EXT_CmpEventMgrEventGetEvent
	#define GET_CmpEventMgrEventGetEvent  ERR_OK
	#define CAL_CmpEventMgrEventGetEvent pICmpEventMgr->IEventGetEvent
	#define CHK_CmpEventMgrEventGetEvent (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventGetEvent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventGetEvent
	#define EXT_EventGetEvent
	#define GET_EventGetEvent(fl)  CAL_CMGETAPI( "EventGetEvent" ) 
	#define CAL_EventGetEvent pICmpEventMgr->IEventGetEvent
	#define CHK_EventGetEvent (pICmpEventMgr != NULL)
	#define EXP_EventGetEvent  CAL_CMEXPAPI( "EventGetEvent" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventGetEvent  PFEVENTGETEVENT pfEventGetEvent;
	#define EXT_EventGetEvent  extern PFEVENTGETEVENT pfEventGetEvent;
	#define GET_EventGetEvent(fl)  s_pfCMGetAPI2( "EventGetEvent", (RTS_VOID_FCTPTR *)&pfEventGetEvent, (fl), 0, 0)
	#define CAL_EventGetEvent  pfEventGetEvent
	#define CHK_EventGetEvent  (pfEventGetEvent != NULL)
	#define EXP_EventGetEvent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventGetEvent", (RTS_UINTPTR)EventGetEvent, 0, 0) 
#endif




/**
 * <description>Extract the event class from eventid</description>
 * <param name="EventId" type="IN">Event ID</param>
 * <result>Event class</result>
 */
unsigned short CDECL EventGetClass(EVENTID EventId);
typedef unsigned short (CDECL * PFEVENTGETCLASS) (EVENTID EventId);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTGETCLASS_NOTIMPLEMENTED)
	#define USE_EventGetClass
	#define EXT_EventGetClass
	#define GET_EventGetClass(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventGetClass(p0)  (unsigned short)ERR_NOTIMPLEMENTED
	#define CHK_EventGetClass  FALSE
	#define EXP_EventGetClass  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventGetClass
	#define EXT_EventGetClass
	#define GET_EventGetClass(fl)  CAL_CMGETAPI( "EventGetClass" ) 
	#define CAL_EventGetClass  EventGetClass
	#define CHK_EventGetClass  TRUE
	#define EXP_EventGetClass  CAL_CMEXPAPI( "EventGetClass" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventGetClass
	#define EXT_EventGetClass
	#define GET_EventGetClass(fl)  CAL_CMGETAPI( "EventGetClass" ) 
	#define CAL_EventGetClass  EventGetClass
	#define CHK_EventGetClass  TRUE
	#define EXP_EventGetClass  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventGetClass", (RTS_UINTPTR)EventGetClass, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventGetClass
	#define EXT_CmpEventMgrEventGetClass
	#define GET_CmpEventMgrEventGetClass  ERR_OK
	#define CAL_CmpEventMgrEventGetClass pICmpEventMgr->IEventGetClass
	#define CHK_CmpEventMgrEventGetClass (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventGetClass  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventGetClass
	#define EXT_EventGetClass
	#define GET_EventGetClass(fl)  CAL_CMGETAPI( "EventGetClass" ) 
	#define CAL_EventGetClass pICmpEventMgr->IEventGetClass
	#define CHK_EventGetClass (pICmpEventMgr != NULL)
	#define EXP_EventGetClass  CAL_CMEXPAPI( "EventGetClass" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventGetClass  PFEVENTGETCLASS pfEventGetClass;
	#define EXT_EventGetClass  extern PFEVENTGETCLASS pfEventGetClass;
	#define GET_EventGetClass(fl)  s_pfCMGetAPI2( "EventGetClass", (RTS_VOID_FCTPTR *)&pfEventGetClass, (fl), 0, 0)
	#define CAL_EventGetClass  pfEventGetClass
	#define CHK_EventGetClass  (pfEventGetClass != NULL)
	#define EXP_EventGetClass  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventGetClass", (RTS_UINTPTR)EventGetClass, 0, 0) 
#endif




/**
 * <description>Register an interface callback function to an event. The interface can be from a C object,
 *	a C++ class or a wrapper class for an Iec function block</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pICallback" type="IN">Pointer to callback interface</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventRegisterCallback(RTS_HANDLE hEvent, ICmpEventCallback *pICallback);
typedef RTS_RESULT (CDECL * PFEVENTREGISTERCALLBACK) (RTS_HANDLE hEvent, ICmpEventCallback *pICallback);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTERCALLBACK_NOTIMPLEMENTED)
	#define USE_EventRegisterCallback
	#define EXT_EventRegisterCallback
	#define GET_EventRegisterCallback(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventRegisterCallback(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventRegisterCallback  FALSE
	#define EXP_EventRegisterCallback  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventRegisterCallback
	#define EXT_EventRegisterCallback
	#define GET_EventRegisterCallback(fl)  CAL_CMGETAPI( "EventRegisterCallback" ) 
	#define CAL_EventRegisterCallback  EventRegisterCallback
	#define CHK_EventRegisterCallback  TRUE
	#define EXP_EventRegisterCallback  CAL_CMEXPAPI( "EventRegisterCallback" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventRegisterCallback
	#define EXT_EventRegisterCallback
	#define GET_EventRegisterCallback(fl)  CAL_CMGETAPI( "EventRegisterCallback" ) 
	#define CAL_EventRegisterCallback  EventRegisterCallback
	#define CHK_EventRegisterCallback  TRUE
	#define EXP_EventRegisterCallback  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallback", (RTS_UINTPTR)EventRegisterCallback, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventRegisterCallback
	#define EXT_CmpEventMgrEventRegisterCallback
	#define GET_CmpEventMgrEventRegisterCallback  ERR_OK
	#define CAL_CmpEventMgrEventRegisterCallback pICmpEventMgr->IEventRegisterCallback
	#define CHK_CmpEventMgrEventRegisterCallback (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventRegisterCallback  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventRegisterCallback
	#define EXT_EventRegisterCallback
	#define GET_EventRegisterCallback(fl)  CAL_CMGETAPI( "EventRegisterCallback" ) 
	#define CAL_EventRegisterCallback pICmpEventMgr->IEventRegisterCallback
	#define CHK_EventRegisterCallback (pICmpEventMgr != NULL)
	#define EXP_EventRegisterCallback  CAL_CMEXPAPI( "EventRegisterCallback" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventRegisterCallback  PFEVENTREGISTERCALLBACK pfEventRegisterCallback;
	#define EXT_EventRegisterCallback  extern PFEVENTREGISTERCALLBACK pfEventRegisterCallback;
	#define GET_EventRegisterCallback(fl)  s_pfCMGetAPI2( "EventRegisterCallback", (RTS_VOID_FCTPTR *)&pfEventRegisterCallback, (fl), 0, 0)
	#define CAL_EventRegisterCallback  pfEventRegisterCallback
	#define CHK_EventRegisterCallback  (pfEventRegisterCallback != NULL)
	#define EXP_EventRegisterCallback  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallback", (RTS_UINTPTR)EventRegisterCallback, 0, 0) 
#endif




/**
 * <description>Register an interface callback function to an event. The interface can be from a C object,
 *	a C++ class or a wrapper class for an Iec function block</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pICallback" type="IN">Pointer to callback interface</param>
 * <param name="pUserParameter" type="IN">Pointer to user parameter, that is transmitted to the callback (see EventParam)</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventRegisterCallback2(RTS_HANDLE hEvent, ICmpEventCallback *pICallback, void *pUserParameter);
typedef RTS_RESULT (CDECL * PFEVENTREGISTERCALLBACK2) (RTS_HANDLE hEvent, ICmpEventCallback *pICallback, void *pUserParameter);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTERCALLBACK2_NOTIMPLEMENTED)
	#define USE_EventRegisterCallback2
	#define EXT_EventRegisterCallback2
	#define GET_EventRegisterCallback2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventRegisterCallback2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventRegisterCallback2  FALSE
	#define EXP_EventRegisterCallback2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventRegisterCallback2
	#define EXT_EventRegisterCallback2
	#define GET_EventRegisterCallback2(fl)  CAL_CMGETAPI( "EventRegisterCallback2" ) 
	#define CAL_EventRegisterCallback2  EventRegisterCallback2
	#define CHK_EventRegisterCallback2  TRUE
	#define EXP_EventRegisterCallback2  CAL_CMEXPAPI( "EventRegisterCallback2" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventRegisterCallback2
	#define EXT_EventRegisterCallback2
	#define GET_EventRegisterCallback2(fl)  CAL_CMGETAPI( "EventRegisterCallback2" ) 
	#define CAL_EventRegisterCallback2  EventRegisterCallback2
	#define CHK_EventRegisterCallback2  TRUE
	#define EXP_EventRegisterCallback2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallback2", (RTS_UINTPTR)EventRegisterCallback2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventRegisterCallback2
	#define EXT_CmpEventMgrEventRegisterCallback2
	#define GET_CmpEventMgrEventRegisterCallback2  ERR_OK
	#define CAL_CmpEventMgrEventRegisterCallback2 pICmpEventMgr->IEventRegisterCallback2
	#define CHK_CmpEventMgrEventRegisterCallback2 (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventRegisterCallback2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventRegisterCallback2
	#define EXT_EventRegisterCallback2
	#define GET_EventRegisterCallback2(fl)  CAL_CMGETAPI( "EventRegisterCallback2" ) 
	#define CAL_EventRegisterCallback2 pICmpEventMgr->IEventRegisterCallback2
	#define CHK_EventRegisterCallback2 (pICmpEventMgr != NULL)
	#define EXP_EventRegisterCallback2  CAL_CMEXPAPI( "EventRegisterCallback2" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventRegisterCallback2  PFEVENTREGISTERCALLBACK2 pfEventRegisterCallback2;
	#define EXT_EventRegisterCallback2  extern PFEVENTREGISTERCALLBACK2 pfEventRegisterCallback2;
	#define GET_EventRegisterCallback2(fl)  s_pfCMGetAPI2( "EventRegisterCallback2", (RTS_VOID_FCTPTR *)&pfEventRegisterCallback2, (fl), 0, 0)
	#define CAL_EventRegisterCallback2  pfEventRegisterCallback2
	#define CHK_EventRegisterCallback2  (pfEventRegisterCallback2 != NULL)
	#define EXP_EventRegisterCallback2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallback2", (RTS_UINTPTR)EventRegisterCallback2, 0, 0) 
#endif




/**
 * <description>Register an interface callback function to an event. The interface can be from a C object,
 *	a C++ class or a wrapper class for an Iec function block</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pICallback" type="IN">Pointer to callback interface</param>
 * <param name="bIec" type="IN">1=Iec interface behind the C interface, 0=C interface</param>
 * <param name="pUserParameter" type="IN">Pointer to user parameter, that is transmitted to the callback (see EventParam)</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventRegisterCallback3(RTS_HANDLE hEvent, ICmpEventCallback *pICallback, int bIec, void *pUserParameter);
typedef RTS_RESULT (CDECL * PFEVENTREGISTERCALLBACK3) (RTS_HANDLE hEvent, ICmpEventCallback *pICallback, int bIec, void *pUserParameter);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTERCALLBACK3_NOTIMPLEMENTED)
	#define USE_EventRegisterCallback3
	#define EXT_EventRegisterCallback3
	#define GET_EventRegisterCallback3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventRegisterCallback3(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventRegisterCallback3  FALSE
	#define EXP_EventRegisterCallback3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventRegisterCallback3
	#define EXT_EventRegisterCallback3
	#define GET_EventRegisterCallback3(fl)  CAL_CMGETAPI( "EventRegisterCallback3" ) 
	#define CAL_EventRegisterCallback3  EventRegisterCallback3
	#define CHK_EventRegisterCallback3  TRUE
	#define EXP_EventRegisterCallback3  CAL_CMEXPAPI( "EventRegisterCallback3" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventRegisterCallback3
	#define EXT_EventRegisterCallback3
	#define GET_EventRegisterCallback3(fl)  CAL_CMGETAPI( "EventRegisterCallback3" ) 
	#define CAL_EventRegisterCallback3  EventRegisterCallback3
	#define CHK_EventRegisterCallback3  TRUE
	#define EXP_EventRegisterCallback3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallback3", (RTS_UINTPTR)EventRegisterCallback3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventRegisterCallback3
	#define EXT_CmpEventMgrEventRegisterCallback3
	#define GET_CmpEventMgrEventRegisterCallback3  ERR_OK
	#define CAL_CmpEventMgrEventRegisterCallback3 pICmpEventMgr->IEventRegisterCallback3
	#define CHK_CmpEventMgrEventRegisterCallback3 (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventRegisterCallback3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventRegisterCallback3
	#define EXT_EventRegisterCallback3
	#define GET_EventRegisterCallback3(fl)  CAL_CMGETAPI( "EventRegisterCallback3" ) 
	#define CAL_EventRegisterCallback3 pICmpEventMgr->IEventRegisterCallback3
	#define CHK_EventRegisterCallback3 (pICmpEventMgr != NULL)
	#define EXP_EventRegisterCallback3  CAL_CMEXPAPI( "EventRegisterCallback3" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventRegisterCallback3  PFEVENTREGISTERCALLBACK3 pfEventRegisterCallback3;
	#define EXT_EventRegisterCallback3  extern PFEVENTREGISTERCALLBACK3 pfEventRegisterCallback3;
	#define GET_EventRegisterCallback3(fl)  s_pfCMGetAPI2( "EventRegisterCallback3", (RTS_VOID_FCTPTR *)&pfEventRegisterCallback3, (fl), 0, 0)
	#define CAL_EventRegisterCallback3  pfEventRegisterCallback3
	#define CHK_EventRegisterCallback3  (pfEventRegisterCallback3 != NULL)
	#define EXP_EventRegisterCallback3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallback3", (RTS_UINTPTR)EventRegisterCallback3, 0, 0) 
#endif




/**
 * <description>Unregister a callback interface from an event specified by handle and callback interface</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pICallback" type="IN">Pointer to callback interface</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventUnregisterCallback(RTS_HANDLE hEvent, ICmpEventCallback *pICallback);
typedef RTS_RESULT (CDECL * PFEVENTUNREGISTERCALLBACK) (RTS_HANDLE hEvent, ICmpEventCallback *pICallback);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTUNREGISTERCALLBACK_NOTIMPLEMENTED)
	#define USE_EventUnregisterCallback
	#define EXT_EventUnregisterCallback
	#define GET_EventUnregisterCallback(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventUnregisterCallback(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventUnregisterCallback  FALSE
	#define EXP_EventUnregisterCallback  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventUnregisterCallback
	#define EXT_EventUnregisterCallback
	#define GET_EventUnregisterCallback(fl)  CAL_CMGETAPI( "EventUnregisterCallback" ) 
	#define CAL_EventUnregisterCallback  EventUnregisterCallback
	#define CHK_EventUnregisterCallback  TRUE
	#define EXP_EventUnregisterCallback  CAL_CMEXPAPI( "EventUnregisterCallback" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventUnregisterCallback
	#define EXT_EventUnregisterCallback
	#define GET_EventUnregisterCallback(fl)  CAL_CMGETAPI( "EventUnregisterCallback" ) 
	#define CAL_EventUnregisterCallback  EventUnregisterCallback
	#define CHK_EventUnregisterCallback  TRUE
	#define EXP_EventUnregisterCallback  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventUnregisterCallback", (RTS_UINTPTR)EventUnregisterCallback, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventUnregisterCallback
	#define EXT_CmpEventMgrEventUnregisterCallback
	#define GET_CmpEventMgrEventUnregisterCallback  ERR_OK
	#define CAL_CmpEventMgrEventUnregisterCallback pICmpEventMgr->IEventUnregisterCallback
	#define CHK_CmpEventMgrEventUnregisterCallback (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventUnregisterCallback  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventUnregisterCallback
	#define EXT_EventUnregisterCallback
	#define GET_EventUnregisterCallback(fl)  CAL_CMGETAPI( "EventUnregisterCallback" ) 
	#define CAL_EventUnregisterCallback pICmpEventMgr->IEventUnregisterCallback
	#define CHK_EventUnregisterCallback (pICmpEventMgr != NULL)
	#define EXP_EventUnregisterCallback  CAL_CMEXPAPI( "EventUnregisterCallback" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventUnregisterCallback  PFEVENTUNREGISTERCALLBACK pfEventUnregisterCallback;
	#define EXT_EventUnregisterCallback  extern PFEVENTUNREGISTERCALLBACK pfEventUnregisterCallback;
	#define GET_EventUnregisterCallback(fl)  s_pfCMGetAPI2( "EventUnregisterCallback", (RTS_VOID_FCTPTR *)&pfEventUnregisterCallback, (fl), 0, 0)
	#define CAL_EventUnregisterCallback  pfEventUnregisterCallback
	#define CHK_EventUnregisterCallback  (pfEventUnregisterCallback != NULL)
	#define EXP_EventUnregisterCallback  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventUnregisterCallback", (RTS_UINTPTR)EventUnregisterCallback, 0, 0) 
#endif




/**
 * <description>Register a callback function to an event. Callback function can be a C or Iec function</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pfCallbackFunction" type="IN">Pointer to callback function</param>
 * <param name="bIec" type="IN">1=Iec function, 0=C function</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventRegisterCallbackFunction(RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction, int bIec);
typedef RTS_RESULT (CDECL * PFEVENTREGISTERCALLBACKFUNCTION) (RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction, int bIec);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTERCALLBACKFUNCTION_NOTIMPLEMENTED)
	#define USE_EventRegisterCallbackFunction
	#define EXT_EventRegisterCallbackFunction
	#define GET_EventRegisterCallbackFunction(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventRegisterCallbackFunction(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventRegisterCallbackFunction  FALSE
	#define EXP_EventRegisterCallbackFunction  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventRegisterCallbackFunction
	#define EXT_EventRegisterCallbackFunction
	#define GET_EventRegisterCallbackFunction(fl)  CAL_CMGETAPI( "EventRegisterCallbackFunction" ) 
	#define CAL_EventRegisterCallbackFunction  EventRegisterCallbackFunction
	#define CHK_EventRegisterCallbackFunction  TRUE
	#define EXP_EventRegisterCallbackFunction  CAL_CMEXPAPI( "EventRegisterCallbackFunction" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventRegisterCallbackFunction
	#define EXT_EventRegisterCallbackFunction
	#define GET_EventRegisterCallbackFunction(fl)  CAL_CMGETAPI( "EventRegisterCallbackFunction" ) 
	#define CAL_EventRegisterCallbackFunction  EventRegisterCallbackFunction
	#define CHK_EventRegisterCallbackFunction  TRUE
	#define EXP_EventRegisterCallbackFunction  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallbackFunction", (RTS_UINTPTR)EventRegisterCallbackFunction, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventRegisterCallbackFunction
	#define EXT_CmpEventMgrEventRegisterCallbackFunction
	#define GET_CmpEventMgrEventRegisterCallbackFunction  ERR_OK
	#define CAL_CmpEventMgrEventRegisterCallbackFunction pICmpEventMgr->IEventRegisterCallbackFunction
	#define CHK_CmpEventMgrEventRegisterCallbackFunction (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventRegisterCallbackFunction  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventRegisterCallbackFunction
	#define EXT_EventRegisterCallbackFunction
	#define GET_EventRegisterCallbackFunction(fl)  CAL_CMGETAPI( "EventRegisterCallbackFunction" ) 
	#define CAL_EventRegisterCallbackFunction pICmpEventMgr->IEventRegisterCallbackFunction
	#define CHK_EventRegisterCallbackFunction (pICmpEventMgr != NULL)
	#define EXP_EventRegisterCallbackFunction  CAL_CMEXPAPI( "EventRegisterCallbackFunction" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventRegisterCallbackFunction  PFEVENTREGISTERCALLBACKFUNCTION pfEventRegisterCallbackFunction;
	#define EXT_EventRegisterCallbackFunction  extern PFEVENTREGISTERCALLBACKFUNCTION pfEventRegisterCallbackFunction;
	#define GET_EventRegisterCallbackFunction(fl)  s_pfCMGetAPI2( "EventRegisterCallbackFunction", (RTS_VOID_FCTPTR *)&pfEventRegisterCallbackFunction, (fl), 0, 0)
	#define CAL_EventRegisterCallbackFunction  pfEventRegisterCallbackFunction
	#define CHK_EventRegisterCallbackFunction  (pfEventRegisterCallbackFunction != NULL)
	#define EXP_EventRegisterCallbackFunction  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallbackFunction", (RTS_UINTPTR)EventRegisterCallbackFunction, 0, 0) 
#endif




/**
 * <description>Register a callback function to an event. Callback function can be a C or Iec function</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pfCallbackFunction" type="IN">Pointer to callback function</param>
 * <param name="bIec" type="IN">1=Iec function, 0=C function</param>
 * <param name="pUserParameter" type="IN">Pointer to user parameter, that is transmitted to the callback (see EventParam)</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventRegisterCallbackFunction2(RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction, int bIec, void *pUserParameter);
typedef RTS_RESULT (CDECL * PFEVENTREGISTERCALLBACKFUNCTION2) (RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction, int bIec, void *pUserParameter);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTERCALLBACKFUNCTION2_NOTIMPLEMENTED)
	#define USE_EventRegisterCallbackFunction2
	#define EXT_EventRegisterCallbackFunction2
	#define GET_EventRegisterCallbackFunction2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventRegisterCallbackFunction2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventRegisterCallbackFunction2  FALSE
	#define EXP_EventRegisterCallbackFunction2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventRegisterCallbackFunction2
	#define EXT_EventRegisterCallbackFunction2
	#define GET_EventRegisterCallbackFunction2(fl)  CAL_CMGETAPI( "EventRegisterCallbackFunction2" ) 
	#define CAL_EventRegisterCallbackFunction2  EventRegisterCallbackFunction2
	#define CHK_EventRegisterCallbackFunction2  TRUE
	#define EXP_EventRegisterCallbackFunction2  CAL_CMEXPAPI( "EventRegisterCallbackFunction2" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventRegisterCallbackFunction2
	#define EXT_EventRegisterCallbackFunction2
	#define GET_EventRegisterCallbackFunction2(fl)  CAL_CMGETAPI( "EventRegisterCallbackFunction2" ) 
	#define CAL_EventRegisterCallbackFunction2  EventRegisterCallbackFunction2
	#define CHK_EventRegisterCallbackFunction2  TRUE
	#define EXP_EventRegisterCallbackFunction2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallbackFunction2", (RTS_UINTPTR)EventRegisterCallbackFunction2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventRegisterCallbackFunction2
	#define EXT_CmpEventMgrEventRegisterCallbackFunction2
	#define GET_CmpEventMgrEventRegisterCallbackFunction2  ERR_OK
	#define CAL_CmpEventMgrEventRegisterCallbackFunction2 pICmpEventMgr->IEventRegisterCallbackFunction2
	#define CHK_CmpEventMgrEventRegisterCallbackFunction2 (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventRegisterCallbackFunction2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventRegisterCallbackFunction2
	#define EXT_EventRegisterCallbackFunction2
	#define GET_EventRegisterCallbackFunction2(fl)  CAL_CMGETAPI( "EventRegisterCallbackFunction2" ) 
	#define CAL_EventRegisterCallbackFunction2 pICmpEventMgr->IEventRegisterCallbackFunction2
	#define CHK_EventRegisterCallbackFunction2 (pICmpEventMgr != NULL)
	#define EXP_EventRegisterCallbackFunction2  CAL_CMEXPAPI( "EventRegisterCallbackFunction2" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventRegisterCallbackFunction2  PFEVENTREGISTERCALLBACKFUNCTION2 pfEventRegisterCallbackFunction2;
	#define EXT_EventRegisterCallbackFunction2  extern PFEVENTREGISTERCALLBACKFUNCTION2 pfEventRegisterCallbackFunction2;
	#define GET_EventRegisterCallbackFunction2(fl)  s_pfCMGetAPI2( "EventRegisterCallbackFunction2", (RTS_VOID_FCTPTR *)&pfEventRegisterCallbackFunction2, (fl), 0, 0)
	#define CAL_EventRegisterCallbackFunction2  pfEventRegisterCallbackFunction2
	#define CHK_EventRegisterCallbackFunction2  (pfEventRegisterCallbackFunction2 != NULL)
	#define EXP_EventRegisterCallbackFunction2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisterCallbackFunction2", (RTS_UINTPTR)EventRegisterCallbackFunction2, 0, 0) 
#endif




/**
 * <description>Unregister a callback function from an event specified by handle and callback </description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pfCallbackFunction" type="IN">Pointer to callback function</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL EventUnregisterCallbackFunction(RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction);
typedef RTS_RESULT (CDECL * PFEVENTUNREGISTERCALLBACKFUNCTION) (RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTUNREGISTERCALLBACKFUNCTION_NOTIMPLEMENTED)
	#define USE_EventUnregisterCallbackFunction
	#define EXT_EventUnregisterCallbackFunction
	#define GET_EventUnregisterCallbackFunction(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventUnregisterCallbackFunction(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventUnregisterCallbackFunction  FALSE
	#define EXP_EventUnregisterCallbackFunction  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventUnregisterCallbackFunction
	#define EXT_EventUnregisterCallbackFunction
	#define GET_EventUnregisterCallbackFunction(fl)  CAL_CMGETAPI( "EventUnregisterCallbackFunction" ) 
	#define CAL_EventUnregisterCallbackFunction  EventUnregisterCallbackFunction
	#define CHK_EventUnregisterCallbackFunction  TRUE
	#define EXP_EventUnregisterCallbackFunction  CAL_CMEXPAPI( "EventUnregisterCallbackFunction" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventUnregisterCallbackFunction
	#define EXT_EventUnregisterCallbackFunction
	#define GET_EventUnregisterCallbackFunction(fl)  CAL_CMGETAPI( "EventUnregisterCallbackFunction" ) 
	#define CAL_EventUnregisterCallbackFunction  EventUnregisterCallbackFunction
	#define CHK_EventUnregisterCallbackFunction  TRUE
	#define EXP_EventUnregisterCallbackFunction  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventUnregisterCallbackFunction", (RTS_UINTPTR)EventUnregisterCallbackFunction, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventUnregisterCallbackFunction
	#define EXT_CmpEventMgrEventUnregisterCallbackFunction
	#define GET_CmpEventMgrEventUnregisterCallbackFunction  ERR_OK
	#define CAL_CmpEventMgrEventUnregisterCallbackFunction pICmpEventMgr->IEventUnregisterCallbackFunction
	#define CHK_CmpEventMgrEventUnregisterCallbackFunction (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventUnregisterCallbackFunction  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventUnregisterCallbackFunction
	#define EXT_EventUnregisterCallbackFunction
	#define GET_EventUnregisterCallbackFunction(fl)  CAL_CMGETAPI( "EventUnregisterCallbackFunction" ) 
	#define CAL_EventUnregisterCallbackFunction pICmpEventMgr->IEventUnregisterCallbackFunction
	#define CHK_EventUnregisterCallbackFunction (pICmpEventMgr != NULL)
	#define EXP_EventUnregisterCallbackFunction  CAL_CMEXPAPI( "EventUnregisterCallbackFunction" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventUnregisterCallbackFunction  PFEVENTUNREGISTERCALLBACKFUNCTION pfEventUnregisterCallbackFunction;
	#define EXT_EventUnregisterCallbackFunction  extern PFEVENTUNREGISTERCALLBACKFUNCTION pfEventUnregisterCallbackFunction;
	#define GET_EventUnregisterCallbackFunction(fl)  s_pfCMGetAPI2( "EventUnregisterCallbackFunction", (RTS_VOID_FCTPTR *)&pfEventUnregisterCallbackFunction, (fl), 0, 0)
	#define CAL_EventUnregisterCallbackFunction  pfEventUnregisterCallbackFunction
	#define CHK_EventUnregisterCallbackFunction  (pfEventUnregisterCallbackFunction != NULL)
	#define EXP_EventUnregisterCallbackFunction  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventUnregisterCallbackFunction", (RTS_UINTPTR)EventUnregisterCallbackFunction, 0, 0) 
#endif




/**
 * <description>Unregister a callback function with a specific user parameter from an event specified by handle and callback and parameter</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pfCallbackFunction" type="IN">Pointer to callback function</param>
 * <param name="bIec" type="IN">1=Iec function, 0=C function</param>
 * <param name="pUserParameter" type="IN">Pointer to user parameter, that was specified as a parameter at EventRegisterCallback2()</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL EventUnregisterCallbackFunction2(RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction, void *pUserParameter);
typedef RTS_RESULT (CDECL * PFEVENTUNREGISTERCALLBACKFUNCTION2) (RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction, void *pUserParameter);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTUNREGISTERCALLBACKFUNCTION2_NOTIMPLEMENTED)
	#define USE_EventUnregisterCallbackFunction2
	#define EXT_EventUnregisterCallbackFunction2
	#define GET_EventUnregisterCallbackFunction2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventUnregisterCallbackFunction2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventUnregisterCallbackFunction2  FALSE
	#define EXP_EventUnregisterCallbackFunction2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventUnregisterCallbackFunction2
	#define EXT_EventUnregisterCallbackFunction2
	#define GET_EventUnregisterCallbackFunction2(fl)  CAL_CMGETAPI( "EventUnregisterCallbackFunction2" ) 
	#define CAL_EventUnregisterCallbackFunction2  EventUnregisterCallbackFunction2
	#define CHK_EventUnregisterCallbackFunction2  TRUE
	#define EXP_EventUnregisterCallbackFunction2  CAL_CMEXPAPI( "EventUnregisterCallbackFunction2" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventUnregisterCallbackFunction2
	#define EXT_EventUnregisterCallbackFunction2
	#define GET_EventUnregisterCallbackFunction2(fl)  CAL_CMGETAPI( "EventUnregisterCallbackFunction2" ) 
	#define CAL_EventUnregisterCallbackFunction2  EventUnregisterCallbackFunction2
	#define CHK_EventUnregisterCallbackFunction2  TRUE
	#define EXP_EventUnregisterCallbackFunction2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventUnregisterCallbackFunction2", (RTS_UINTPTR)EventUnregisterCallbackFunction2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventUnregisterCallbackFunction2
	#define EXT_CmpEventMgrEventUnregisterCallbackFunction2
	#define GET_CmpEventMgrEventUnregisterCallbackFunction2  ERR_OK
	#define CAL_CmpEventMgrEventUnregisterCallbackFunction2 pICmpEventMgr->IEventUnregisterCallbackFunction2
	#define CHK_CmpEventMgrEventUnregisterCallbackFunction2 (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventUnregisterCallbackFunction2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventUnregisterCallbackFunction2
	#define EXT_EventUnregisterCallbackFunction2
	#define GET_EventUnregisterCallbackFunction2(fl)  CAL_CMGETAPI( "EventUnregisterCallbackFunction2" ) 
	#define CAL_EventUnregisterCallbackFunction2 pICmpEventMgr->IEventUnregisterCallbackFunction2
	#define CHK_EventUnregisterCallbackFunction2 (pICmpEventMgr != NULL)
	#define EXP_EventUnregisterCallbackFunction2  CAL_CMEXPAPI( "EventUnregisterCallbackFunction2" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventUnregisterCallbackFunction2  PFEVENTUNREGISTERCALLBACKFUNCTION2 pfEventUnregisterCallbackFunction2;
	#define EXT_EventUnregisterCallbackFunction2  extern PFEVENTUNREGISTERCALLBACKFUNCTION2 pfEventUnregisterCallbackFunction2;
	#define GET_EventUnregisterCallbackFunction2(fl)  s_pfCMGetAPI2( "EventUnregisterCallbackFunction2", (RTS_VOID_FCTPTR *)&pfEventUnregisterCallbackFunction2, (fl), 0, 0)
	#define CAL_EventUnregisterCallbackFunction2  pfEventUnregisterCallbackFunction2
	#define CHK_EventUnregisterCallbackFunction2  (pfEventUnregisterCallbackFunction2 != NULL)
	#define EXP_EventUnregisterCallbackFunction2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventUnregisterCallbackFunction2", (RTS_UINTPTR)EventUnregisterCallbackFunction2, 0, 0) 
#endif



 
/**
 * <description>Function to evaluate the number of registered callback handlers on a single event</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Number of registered callback handlers on this event</result>
 */
RTS_UI32 CDECL EventRegisteredCallbacks(RTS_HANDLE hEvent, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFEVENTREGISTEREDCALLBACKS) (RTS_HANDLE hEvent, RTS_RESULT *pResult);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTREGISTEREDCALLBACKS_NOTIMPLEMENTED)
	#define USE_EventRegisteredCallbacks
	#define EXT_EventRegisteredCallbacks
	#define GET_EventRegisteredCallbacks(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventRegisteredCallbacks(p0,p1)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_EventRegisteredCallbacks  FALSE
	#define EXP_EventRegisteredCallbacks  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventRegisteredCallbacks
	#define EXT_EventRegisteredCallbacks
	#define GET_EventRegisteredCallbacks(fl)  CAL_CMGETAPI( "EventRegisteredCallbacks" ) 
	#define CAL_EventRegisteredCallbacks  EventRegisteredCallbacks
	#define CHK_EventRegisteredCallbacks  TRUE
	#define EXP_EventRegisteredCallbacks  CAL_CMEXPAPI( "EventRegisteredCallbacks" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventRegisteredCallbacks
	#define EXT_EventRegisteredCallbacks
	#define GET_EventRegisteredCallbacks(fl)  CAL_CMGETAPI( "EventRegisteredCallbacks" ) 
	#define CAL_EventRegisteredCallbacks  EventRegisteredCallbacks
	#define CHK_EventRegisteredCallbacks  TRUE
	#define EXP_EventRegisteredCallbacks  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisteredCallbacks", (RTS_UINTPTR)EventRegisteredCallbacks, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventRegisteredCallbacks
	#define EXT_CmpEventMgrEventRegisteredCallbacks
	#define GET_CmpEventMgrEventRegisteredCallbacks  ERR_OK
	#define CAL_CmpEventMgrEventRegisteredCallbacks pICmpEventMgr->IEventRegisteredCallbacks
	#define CHK_CmpEventMgrEventRegisteredCallbacks (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventRegisteredCallbacks  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventRegisteredCallbacks
	#define EXT_EventRegisteredCallbacks
	#define GET_EventRegisteredCallbacks(fl)  CAL_CMGETAPI( "EventRegisteredCallbacks" ) 
	#define CAL_EventRegisteredCallbacks pICmpEventMgr->IEventRegisteredCallbacks
	#define CHK_EventRegisteredCallbacks (pICmpEventMgr != NULL)
	#define EXP_EventRegisteredCallbacks  CAL_CMEXPAPI( "EventRegisteredCallbacks" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventRegisteredCallbacks  PFEVENTREGISTEREDCALLBACKS pfEventRegisteredCallbacks;
	#define EXT_EventRegisteredCallbacks  extern PFEVENTREGISTEREDCALLBACKS pfEventRegisteredCallbacks;
	#define GET_EventRegisteredCallbacks(fl)  s_pfCMGetAPI2( "EventRegisteredCallbacks", (RTS_VOID_FCTPTR *)&pfEventRegisteredCallbacks, (fl), 0, 0)
	#define CAL_EventRegisteredCallbacks  pfEventRegisteredCallbacks
	#define CHK_EventRegisteredCallbacks  (pfEventRegisteredCallbacks != NULL)
	#define EXP_EventRegisteredCallbacks  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventRegisteredCallbacks", (RTS_UINTPTR)EventRegisteredCallbacks, 0, 0) 
#endif




/**
 * <description>Post or sent an event</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="pEventParam" type="IN">Pointer to the event parameters</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventPost(RTS_HANDLE hEvent, EventParam *pEventParam);
typedef RTS_RESULT (CDECL * PFEVENTPOST) (RTS_HANDLE hEvent, EventParam *pEventParam);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTPOST_NOTIMPLEMENTED)
	#define USE_EventPost
	#define EXT_EventPost
	#define GET_EventPost(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventPost(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventPost  FALSE
	#define EXP_EventPost  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventPost
	#define EXT_EventPost
	#define GET_EventPost(fl)  CAL_CMGETAPI( "EventPost" ) 
	#define CAL_EventPost  EventPost
	#define CHK_EventPost  TRUE
	#define EXP_EventPost  CAL_CMEXPAPI( "EventPost" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventPost
	#define EXT_EventPost
	#define GET_EventPost(fl)  CAL_CMGETAPI( "EventPost" ) 
	#define CAL_EventPost  EventPost
	#define CHK_EventPost  TRUE
	#define EXP_EventPost  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventPost", (RTS_UINTPTR)EventPost, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventPost
	#define EXT_CmpEventMgrEventPost
	#define GET_CmpEventMgrEventPost  ERR_OK
	#define CAL_CmpEventMgrEventPost pICmpEventMgr->IEventPost
	#define CHK_CmpEventMgrEventPost (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventPost  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventPost
	#define EXT_EventPost
	#define GET_EventPost(fl)  CAL_CMGETAPI( "EventPost" ) 
	#define CAL_EventPost pICmpEventMgr->IEventPost
	#define CHK_EventPost (pICmpEventMgr != NULL)
	#define EXP_EventPost  CAL_CMEXPAPI( "EventPost" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventPost  PFEVENTPOST pfEventPost;
	#define EXT_EventPost  extern PFEVENTPOST pfEventPost;
	#define GET_EventPost(fl)  s_pfCMGetAPI2( "EventPost", (RTS_VOID_FCTPTR *)&pfEventPost, (fl), 0, 0)
	#define CAL_EventPost  pfEventPost
	#define CHK_EventPost  (pfEventPost != NULL)
	#define EXP_EventPost  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventPost", (RTS_UINTPTR)EventPost, 0, 0) 
#endif




/**
 * <description>Sent an event and call synchronously all registered callback handler</description>
 * <param name="hEvent" type="IN">Handle to event</param>
 * <param name="usParamId" type="IN">Id of the parameter</param>
 * <param name="usVersion" type="IN">Version of the parameter</param>
 * <param name="pParameter" type="IN">Pointer to the event specific parameter, that is specified by Id</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventPost2(RTS_HANDLE hEvent, unsigned short usParamId, unsigned short usVersion, void* pParameter);
typedef RTS_RESULT (CDECL * PFEVENTPOST2) (RTS_HANDLE hEvent, unsigned short usParamId, unsigned short usVersion, void* pParameter);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTPOST2_NOTIMPLEMENTED)
	#define USE_EventPost2
	#define EXT_EventPost2
	#define GET_EventPost2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventPost2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventPost2  FALSE
	#define EXP_EventPost2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventPost2
	#define EXT_EventPost2
	#define GET_EventPost2(fl)  CAL_CMGETAPI( "EventPost2" ) 
	#define CAL_EventPost2  EventPost2
	#define CHK_EventPost2  TRUE
	#define EXP_EventPost2  CAL_CMEXPAPI( "EventPost2" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventPost2
	#define EXT_EventPost2
	#define GET_EventPost2(fl)  CAL_CMGETAPI( "EventPost2" ) 
	#define CAL_EventPost2  EventPost2
	#define CHK_EventPost2  TRUE
	#define EXP_EventPost2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventPost2", (RTS_UINTPTR)EventPost2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventPost2
	#define EXT_CmpEventMgrEventPost2
	#define GET_CmpEventMgrEventPost2  ERR_OK
	#define CAL_CmpEventMgrEventPost2 pICmpEventMgr->IEventPost2
	#define CHK_CmpEventMgrEventPost2 (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventPost2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventPost2
	#define EXT_EventPost2
	#define GET_EventPost2(fl)  CAL_CMGETAPI( "EventPost2" ) 
	#define CAL_EventPost2 pICmpEventMgr->IEventPost2
	#define CHK_EventPost2 (pICmpEventMgr != NULL)
	#define EXP_EventPost2  CAL_CMEXPAPI( "EventPost2" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventPost2  PFEVENTPOST2 pfEventPost2;
	#define EXT_EventPost2  extern PFEVENTPOST2 pfEventPost2;
	#define GET_EventPost2(fl)  s_pfCMGetAPI2( "EventPost2", (RTS_VOID_FCTPTR *)&pfEventPost2, (fl), 0, 0)
	#define CAL_EventPost2  pfEventPost2
	#define CHK_EventPost2  (pfEventPost2 != NULL)
	#define EXP_EventPost2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventPost2", (RTS_UINTPTR)EventPost2, 0, 0) 
#endif




/**
 * <description>Post an event direct without opening the event</description>
 * <param name="EventId" type="IN">Event ID of the event. Contains the class and the event</param>
 * <param name="CmpIdProvider" type="IN">Component ID of the provider</param>
 * <param name="pEventParam" type="IN">Pointer to the event parameters</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL EventPostByEvent(EVENTID EventId, CMPID CmpIdProvider, EventParam *pEventParam);
typedef RTS_RESULT (CDECL * PFEVENTPOSTBYEVENT) (EVENTID EventId, CMPID CmpIdProvider, EventParam *pEventParam);
#if defined(CMPEVENTMGR_NOTIMPLEMENTED) || defined(EVENTPOSTBYEVENT_NOTIMPLEMENTED)
	#define USE_EventPostByEvent
	#define EXT_EventPostByEvent
	#define GET_EventPostByEvent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_EventPostByEvent(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_EventPostByEvent  FALSE
	#define EXP_EventPostByEvent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_EventPostByEvent
	#define EXT_EventPostByEvent
	#define GET_EventPostByEvent(fl)  CAL_CMGETAPI( "EventPostByEvent" ) 
	#define CAL_EventPostByEvent  EventPostByEvent
	#define CHK_EventPostByEvent  TRUE
	#define EXP_EventPostByEvent  CAL_CMEXPAPI( "EventPostByEvent" ) 
#elif defined(MIXED_LINK) && !defined(CMPEVENTMGR_EXTERNAL)
	#define USE_EventPostByEvent
	#define EXT_EventPostByEvent
	#define GET_EventPostByEvent(fl)  CAL_CMGETAPI( "EventPostByEvent" ) 
	#define CAL_EventPostByEvent  EventPostByEvent
	#define CHK_EventPostByEvent  TRUE
	#define EXP_EventPostByEvent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventPostByEvent", (RTS_UINTPTR)EventPostByEvent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpEventMgrEventPostByEvent
	#define EXT_CmpEventMgrEventPostByEvent
	#define GET_CmpEventMgrEventPostByEvent  ERR_OK
	#define CAL_CmpEventMgrEventPostByEvent pICmpEventMgr->IEventPostByEvent
	#define CHK_CmpEventMgrEventPostByEvent (pICmpEventMgr != NULL)
	#define EXP_CmpEventMgrEventPostByEvent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_EventPostByEvent
	#define EXT_EventPostByEvent
	#define GET_EventPostByEvent(fl)  CAL_CMGETAPI( "EventPostByEvent" ) 
	#define CAL_EventPostByEvent pICmpEventMgr->IEventPostByEvent
	#define CHK_EventPostByEvent (pICmpEventMgr != NULL)
	#define EXP_EventPostByEvent  CAL_CMEXPAPI( "EventPostByEvent" ) 
#else /* DYNAMIC_LINK */
	#define USE_EventPostByEvent  PFEVENTPOSTBYEVENT pfEventPostByEvent;
	#define EXT_EventPostByEvent  extern PFEVENTPOSTBYEVENT pfEventPostByEvent;
	#define GET_EventPostByEvent(fl)  s_pfCMGetAPI2( "EventPostByEvent", (RTS_VOID_FCTPTR *)&pfEventPostByEvent, (fl), 0, 0)
	#define CAL_EventPostByEvent  pfEventPostByEvent
	#define CHK_EventPostByEvent  (pfEventPostByEvent != NULL)
	#define EXP_EventPostByEvent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"EventPostByEvent", (RTS_UINTPTR)EventPostByEvent, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif




typedef struct
{
	IBase_C *pBase;
	PFEVENTCREATE IEventCreate;
 	PFEVENTCREATE2 IEventCreate2;
 	PFEVENTCREATE3 IEventCreate3;
 	PFEVENTDELETE IEventDelete;
 	PFEVENTDELETEALL IEventDeleteAll;
 	PFEVENTOPEN IEventOpen;
 	PFEVENTCLOSE IEventClose;
 	PFEVENTGETEVENT IEventGetEvent;
 	PFEVENTGETCLASS IEventGetClass;
 	PFEVENTREGISTERCALLBACK IEventRegisterCallback;
 	PFEVENTREGISTERCALLBACK2 IEventRegisterCallback2;
 	PFEVENTREGISTERCALLBACK3 IEventRegisterCallback3;
 	PFEVENTUNREGISTERCALLBACK IEventUnregisterCallback;
 	PFEVENTREGISTERCALLBACKFUNCTION IEventRegisterCallbackFunction;
 	PFEVENTREGISTERCALLBACKFUNCTION2 IEventRegisterCallbackFunction2;
 	PFEVENTUNREGISTERCALLBACKFUNCTION IEventUnregisterCallbackFunction;
 	PFEVENTUNREGISTERCALLBACKFUNCTION2 IEventUnregisterCallbackFunction2;
 	PFEVENTREGISTEREDCALLBACKS IEventRegisteredCallbacks;
 	PFEVENTPOST IEventPost;
 	PFEVENTPOST2 IEventPost2;
 	PFEVENTPOSTBYEVENT IEventPostByEvent;
 } ICmpEventMgr_C;

#ifdef CPLUSPLUS
class ICmpEventMgr : public IBase
{
	public:
		virtual RTS_HANDLE CDECL IEventCreate(EVENTID EventId, CMPID CmpIdProvider, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IEventCreate2(EVENTID EventId, CMPID CmpIdProvider, RTS_UI32 nCallbacksPossible, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IEventCreate3(EVENTID EventId, CMPID CmpIdProvider, EventOptions *pEventOptions, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IEventDelete(RTS_HANDLE hEvent) =0;
		virtual RTS_RESULT CDECL IEventDeleteAll(void) =0;
		virtual RTS_HANDLE CDECL IEventOpen(EVENTID EventId, CMPID CmpIdProvider, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IEventClose(RTS_HANDLE hEvent) =0;
		virtual unsigned short CDECL IEventGetEvent(EVENTID EventId) =0;
		virtual unsigned short CDECL IEventGetClass(EVENTID EventId) =0;
		virtual RTS_RESULT CDECL IEventRegisterCallback(RTS_HANDLE hEvent, ICmpEventCallback *pICallback) =0;
		virtual RTS_RESULT CDECL IEventRegisterCallback2(RTS_HANDLE hEvent, ICmpEventCallback *pICallback, void *pUserParameter) =0;
		virtual RTS_RESULT CDECL IEventRegisterCallback3(RTS_HANDLE hEvent, ICmpEventCallback *pICallback, int bIec, void *pUserParameter) =0;
		virtual RTS_RESULT CDECL IEventUnregisterCallback(RTS_HANDLE hEvent, ICmpEventCallback *pICallback) =0;
		virtual RTS_RESULT CDECL IEventRegisterCallbackFunction(RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction, int bIec) =0;
		virtual RTS_RESULT CDECL IEventRegisterCallbackFunction2(RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction, int bIec, void *pUserParameter) =0;
		virtual RTS_RESULT CDECL IEventUnregisterCallbackFunction(RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction) =0;
		virtual RTS_RESULT CDECL IEventUnregisterCallbackFunction2(RTS_HANDLE hEvent, PFEVENTCALLBACKFUNCTION pfCallbackFunction, void *pUserParameter) =0;
		virtual RTS_UI32 CDECL IEventRegisteredCallbacks(RTS_HANDLE hEvent, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IEventPost(RTS_HANDLE hEvent, EventParam *pEventParam) =0;
		virtual RTS_RESULT CDECL IEventPost2(RTS_HANDLE hEvent, unsigned short usParamId, unsigned short usVersion, void* pParameter) =0;
		virtual RTS_RESULT CDECL IEventPostByEvent(EVENTID EventId, CMPID CmpIdProvider, EventParam *pEventParam) =0;
};
	#ifndef ITF_CmpEventMgr
		#define ITF_CmpEventMgr static ICmpEventMgr *pICmpEventMgr = NULL;
	#endif
	#define EXTITF_CmpEventMgr
#else	/*CPLUSPLUS*/
	typedef ICmpEventMgr_C		ICmpEventMgr;
	#ifndef ITF_CmpEventMgr
		#define ITF_CmpEventMgr
	#endif
	#define EXTITF_CmpEventMgr
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPEVENTMGRITF_H_*/
