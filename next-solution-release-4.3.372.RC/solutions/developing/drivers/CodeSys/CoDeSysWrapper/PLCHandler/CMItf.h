 /**
 * <interfacename>CmpMgr</interfacename>
 * <description> 
 *	<p>Interface of the component manager</p>
 *	<p>The component manager component is the central component of the runtime system.
 *	The component manager is responsible for:</p>
 *	<ul>
 *		<li>startup and shutdown of the runtime system</li>
 *		<li>loading and unloading all components</li>
 *		<li>intializing all components</li>
 *		<li>linking all functions, including the external IEC library functions</li>
 *		<li>checking the consistency of calling a function (matching function prototypes)</li>
 *	</ul>
 *	<p>The list of components that should be loaded can be specified in different ways.</p>
 *	<p>If the components are linked static, there are two different ways to specify the component list:</p>
 *	<ul>
 *		<li>Static list: The list of static loaded components can be specified as a calling option for the CMInit()
 *		interface method. The name of the component on the entry funtion of each component must be specified.
 *		See the description of the CMInit() method for detailed information.</li>
 *		<li>Settings: The settings component has a settings interface, where the component list can be specified.
 *		Here only the component name must be specified. In this case, the function MainLoadComponent() must be
 *		impleemnted in the Main module and here all static linked components must be added to the list. The advantage
 *		of this method is that in the settings you can specify, which static component can be loaded or not. This
 *		is quite flexible.</li>
 *	</ul>
 *	<p>For dynamically linked components, the component list can only be specified in the settings component.</p>
 *	<p>External IEC library functions are managed by the component manager too. Each interface have to specify,
 *	if it could be used for IEC code or not in the m4 Interface definition.</p>
 *
 *	<p>An additional feature is the possibility to use all components with a C++ interface. Here you can implement
 *	your own component in C++ and can use all other runtime components object oriented with a C++ interface.</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMITF_H_
#define _CMITF_H_

#include "CmpStd.h"

 

 




#include "CmpMemPoolItf.h"
#include "CmpEventMgrItf.h"
#include "CMUtilsItf.h"
#include "SysSemItf.h"
#include "SysIntItf.h"

/**
 * <category>Compiler switch</category>
 * <description>The way components are linked together can be specified by the following compiler switches.
 *	<p>If nothing is specified, DYNAMIC_LINK is used by default.</p>
 *	<p>Link examples:</p>
 *	<ul>
 *		<li>Static linked and closed runtime system (e.g. embedded):
 *			STATIC_LINK must be specified</li>
 *		<li>Static linked runtime system, that can be extended via separate, dynamically linkable components:
 *			MIXED_LINK must be specified for the runtime system.
 *			Nothing must be specified in the external component.</li>
 *		<li>Complete dynamically linked runtime system (each component is a separate loadable module):
 *			-- Nothing must be specified in each component.</li>
 *		<li>Static linked runtime system, that can be extended via separate, dynamically linkable components
 *			and all interface functions should be overloadable:
 *			DYNAMIC_LINK must be specified for the runtime system.
 *			-- Nothing must be specified in the external component.</li>
 *		<li>C++ runtime system: 
 *			CPLUSPLUS must be specified always in each component, that is used in a C++ runtime system.
 *			This is independant, if the kernel or a separate component is linked (static or dynamically linked!</li>
 *	</ul>
 * </description>
 * <element name="STATIC_LINK">All components are linked statically to the component manager during compile time.
 *			All components call each other via C-function calls.</element>
 * <element name="MIXED_LINK">All components are linked static, but can be extended by dynamically linked components.
 *			All static components call each other via C-function. All dynamically linked components are
 *			called and call the kernel components via function pointers.</element>
 * <element name="DYNAMIC_LINK">[Default] All components exists as separate compiled modules. So this modules can be loaded dynamically.
 *			All components call each other via function pointers.
 *			NOTE:
 *			If DYNAMIC_LINK is specified explicitly in a runtime system that is linked static,
 *			all component functions	(also the kernel components) are called via function pointers!</element>
 * <element name="CPLUSPLUS">All components are linked together via C++ classes.
 *			All components call each other via C++ method calls.
 *			Internally, the components calls itself	via C-function calls.</element>
 */


/**
 * <category>Compiler switch</category>
 * <description>
 *	Compiler switches to check if the alignment between the DeviceDescription setting "pack-mode" of the CODESYS compiler 
 *	and the C-compiler standard pack mode match together.
 * </description>
 * <element name="EXTERNAL_LIB_PACKMODE">Define the alignment of the CODESYS Compiler:
 *			2=2Byte alignment ("pack-mode"=2)
 *			4=4Byte alignment ("pack-mode"=4)
 *			8=8Byte alignment [default] ("pack-mode"=8)</element>
 */
#ifndef EXTERNAL_LIB_PACKMODE
	#define EXTERNAL_LIB_PACKMODE	8
#endif


/**
 * <category>Compiler switch</category>
 * <description>
 *	Compiler switches to enable/disable single features in the component.
 * </description>
 * <element name="CM_SYSTARGET_DISABLE_OVERLOADABLE_FUNCTIONS">Switch to disable the possibility to overload the SysTarget functions</element>
 * <element name="CM_DISABLE_API_SIGNATURE_CHECK">Switch to disable the signature check of all api functions</element>
 * <element name="CM_DISABLE_API_VERSION_CHECK">Switch to disable the version check of all api functions</element>
 * <element name="CM_NO_EXIT">Switch to disable definite exit or shutdown process of the runtime system</element>
 * <element name="CM_NO_DYNAMIC_COMPONENTS">Switch to disable dynamic loadable components</element>
 */
#if defined(RTS_COMPACT) || defined(RTS_SIL2)
	#define CM_NO_EXIT
	#define CM_NO_DYNAMIC_COMPONENTS
#endif



/**
 * <category>Static defines</category>
 * <description>Maximum length of component name</description>
 */
#define MAX_COMPONENT_NAME						32

/**
 * <category>Static defines</category>
 * <description>Maximum length of api function name</description>
 */
#define MAX_API_NAME							64


/**
 * <category>Static defines</category>
 * <description>Length of component list that is allocated static</description>
 */
#ifndef CM_NUM_OF_STATIC_COMPONENTS
	#if defined(CPLUSPLUS)
		#define CM_NUM_OF_STATIC_COMPONENTS		110
	#else
		#define CM_NUM_OF_STATIC_COMPONENTS		40
	#endif
#endif

/**
 * <category>Static defines</category>
 * <description>Length of class list that is allocated static</description>
 */
#ifndef CM_NUM_OF_STATIC_CLASSES
	#ifdef CPLUSPLUS
		#define CM_NUM_OF_STATIC_CLASSES		CM_NUM_OF_STATIC_COMPONENTS
	#else		
		#define CM_NUM_OF_STATIC_CLASSES		5
	#endif
#endif

/**
 * <category>Static defines</category>
 * <description>Length of instance list that is allocated static</description>
 */
#ifndef CM_NUM_OF_STATIC_INSTANCES
	#ifdef CPLUSPLUS
		#define CM_NUM_OF_STATIC_INSTANCES		CM_NUM_OF_STATIC_COMPONENTS
	#else		
		#define CM_NUM_OF_STATIC_INSTANCES		5
	#endif
#endif

/**
 * <category>Static defines</category>
 * <description>
 *	Significant mask to check different versions of an api function. If the significant
 *	parts of the version don't match, the api versions don't match!
 * </description>
 */
#ifndef CM_API_VERSION_CHECK_MASK
	#define CM_API_VERSION_CHECK_MASK				UINT32_C(0xFF000000)
#endif

/**
 * <category>Static defines</category>
 * <description>
 *	Specifies the minimum supported version of all api functions.
 * </description>
 */
#ifndef CM_API_VERSION_CHECK_MINIMUM
	#define CM_API_VERSION_CHECK_MINIMUM			UINT32_C(0x00000000)
#endif


/**
 * <category>Static defines</category>
 * <description>
 *	Significant mask to check different versions of an external library api function. If the significant
 *	parts of the version don't match, the api versions don't match!
 * </description>
 */
#ifndef CM_EXTLIB_API_VERSION_CHECK_MASK
	#define CM_EXTLIB_API_VERSION_CHECK_MASK		UINT32_C(0xFF000000)
#endif

/**
 * <category>Static defines</category>
 * <description>
 *	Specifies the minimum supported version of all external library api functions.
 * </description>
 */
#ifndef CM_EXTLIB_API_VERSION_CHECK_MINIMUM
	#define CM_EXTLIB_API_VERSION_CHECK_MINIMUM		UINT32_C(0x00000000)
#endif



/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting to read in list of components.
 *	Component list must be indexed, started with 0 or 1, e.g.:
 *	Component.0=[Component Name]	or
 *	Component.1=[Component Name]
 * </description>
 */
#define CMPMGR_KEY_STRING_COMPONENT_NAME		"Component"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to enable/disable the possibility to overload single interface functions of a component.
 *	ATTENTION: The component, which interface function should be overloaded, must be loaded _before_ the component
 *			   that overloads the function!
 * </description>
 */
#define CMPMGR_KEY_INT_OVERLOADABLE_FUNCTIONS							"OverloadableFunctions"
#ifndef CMPMGR_VALUE_INT_OVERLOADABLE_FUNCTIONS_DEFAULT
	#define CMPMGR_VALUE_INT_OVERLOADABLE_FUNCTIONS_DEFAULT				0
#endif

/**
 * <category>Static defines</category>
 * <description>OBSOLETE: Only for backward compatibility</description>
 */
#ifdef CMPMGR_KEY_INT_OVERLOADABLE_FUNCTIONS_DEFAULT
	#undef CMPMGR_VALUE_INT_OVERLOADABLE_FUNCTIONS_DEFAULT
	#define CMPMGR_VALUE_INT_OVERLOADABLE_FUNCTIONS_DEFAULT				CMPMGR_KEY_INT_OVERLOADABLE_FUNCTIONS_DEFAULT
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to define the minimal interval (in ms) in which the function CMCallExtraCommCycleHook() calls on
 *  non-multitaskling systems the CommCycleHook additionally.
 * </description>
 */
#define CMPMGR_KEY_INT_MINIMAL_COMM_CYCLE_HOOK_INTERVAL					"MinimalCommCycleHookInterval"
#ifndef CMPMGR_VALUE_INT_MINIMAL_COMM_CYCLE_HOOK_INTERVAL_DEFAULT
	#define CMPMGR_VALUE_INT_MINIMAL_COMM_CYCLE_HOOK_INTERVAL_DEFAULT	250
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to define the sleep time (in ms) after each comm cycle on WinCE targets. 
 * </description>
 */
#define CMPMGR_KEY_INT_WINCE_COMM_CYCLE_INTERVAL						"WinCE.CommCycleInterval"
#ifndef CMPMGR_VALUE_INT_WINCE_COMM_CYCLE_INTERVAL_DEFAULT
	#define CMPMGR_VALUE_INT_WINCE_COMM_CYCLE_INTERVAL_DEFAULT			9
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to define Windows CE thread priority of the comm cycle thread on WinCE targets. 
 * Important for communication performance
 * </description>
 */
#define CMPMGR_KEY_INT_WINCE_COMM_CYCLE_THREAD_PRIORITY					"WinCE.CommCycleThreadPriority"
#ifndef CMPMGR_KEY_INT_WINCE_COMM_CYCLE_THREAD_PRIORITY_DEFAULT
	#define CMPMGR_KEY_INT_WINCE_COMM_CYCLE_THREAD_PRIORITY_DEFAULT		250
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to define if the PLC window is hided after startup.
 * </description>
 */
#define CMPMGR_KEY_INT_WINDOWHIDED					"WindowHided"
#ifndef CMPMGR_KEY_INT_WINDOWHIDED_DEFAULT
	#define CMPMGR_KEY_INT_WINDOWHIDED_DEFAULT		0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to switch on/off the possibility to disable single operations by the corresponding event EVT_CmpMgr_DisableOperation.
 *	DisablingOperations=1: Operations can be disabled
 *	DisablingOperations=0: Operations cannot be disabled
 * </description>
 */
#define CMPMGR_KEY_INT_DISABLING_OPERATIONS								"DisablingOperations"
#ifndef CMPMGR_VALUE_INT_DISABLING_OPERATIONS_DEFAULT
	#define CMPMGR_VALUE_INT_DISABLING_OPERATIONS_DEFAULT				1
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to specify, for which component the profiler is active. See category "Profiler" for details.
 * </description>
 */
#define CMPMGR_KEY_INT_PROFILER_COMPONENTID								"Profiler.ComponentID"
#ifndef CMPMGR_VALUE_INT_PROFILER_COMPONENTID_DEFAULT					
	#define CMPMGR_VALUE_INT_PROFILER_COMPONENTID_DEFAULT				0
#endif


/**
 * <category>Event parameter</category>
 * <element name="pdummy" type="IN">dummy</element>
 */
typedef struct
{
	void* pdummy;
} EVTPARAM_CmpMgr_Shutdown;
#define EVTPARAMID_CmpMgr_Shutdown	0x0001
#define EVTVERSION_CmpMgr_Shutdown	0x0001

/**
 * <category>Event parameter</category>
 * <element name="cmpId" type="IN">ComponentID of the component, which operation should be disabled</element>
 * <element name="ulOperation" type="IN">Operation that can be disabled. See category "Operations" in Itf.h file of the corresponding component for details.</element>
 * <element name="cmpIdDisabled" type="IN">ComponentID of the source, which disabled the operation</element>
 * <element name="bDisable" type="IN">Must be set to 1 to disable the operation, else: do nothing and remain the value as it is!</element>
 */
typedef struct
{
	CMPID cmpId;
	RTS_UI32 ulOperation;
	CMPID cmpIdDisabled;
	RTS_I32 bDisable;
} EVTPARAM_CmpMgr_DisableOperation;
#define EVTPARAMID_CmpMgr_DisableOperation	0x0002
#define EVTVERSION_CmpMgr_DisableOperation	0x0001

/**
 * <category>Event parameter</category>
 * <element name="bLastCall" type="IN">This value will be != 0 for the last call of this event, otherwise 0.</element>
 * <element name="bFurtherCallNecessary" type="OUT">Callees of the event can set this value to <c>!= 0</c> if they expect a further call of
 *	the same event with inbetween comm cycle calls. It is not expected that a callee sets this value to <c>0</c>!</element>
 */
typedef struct
{
	RTS_I32 bLastCall;
	RTS_I32 bFurtherCallNecessary;
} EVTPARAM_CmpMgr_PrepareExitCommProcessing;
#define EVTPARAMID_CmpMgr_PrepareExitCommProcessing	0x0003
#define EVTVERSION_CmpMgr_PrepareExitCommProcessing	0x0001

/**
 * <category>Event parameter</category>
 * <element name="ulLicenseID" type="IN">License ID requested.</element>
 * <element name="ulLicenseValue" type="IN">Value of the license.</element>
 */
typedef struct
{
	RTS_IEC_UDINT ulLicenseID;
	RTS_IEC_UDINT ulLicenseValue;
} EVTPARAM_CmpMgr_LicenseRequest;
#define EVTPARAMID_CmpMgr_LicenseRequest			0x0004
#define EVTVERSION_CmpMgr_LicenseRequest					0x0001



/**
 * <category>Events</category>
 * <description>Event is sent before shutdown of the runtime system</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpMgr_Shutdown</param>
 */
#define EVT_CmpMgr_PrepareShutdown				MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Events</category>
 * <description>Event is sent before exit the communication servers during shutdown</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpMgr_Shutdown</param>
 */
#define EVT_CmpMgr_PrepareExitComm				MAKE_EVENTID(EVTCLASS_INFO, 2)

/**
 * <category>Events</category>
 * <description>Event is sent before exit all tasks during shutdown</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpMgr_Shutdown</param>
 */
#define EVT_CmpMgr_PrepareExitTasks				MAKE_EVENTID(EVTCLASS_INFO, 3)

/**
 * <category>Events</category>
 * <description>Event is sent before exit of the runtime system during shutdown</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpMgr_Shutdown</param>
 */
#define EVT_CmpMgr_Exit3						MAKE_EVENTID(EVTCLASS_INFO, 4)

/**
 * <category>Events</category>
 * <description>Event is sent before exit of the runtime system during shutdown</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpMgr_Shutdown</param>
 */
#define EVT_CmpMgr_PrepareExit					MAKE_EVENTID(EVTCLASS_INFO, 5)

/**
 * <category>Events</category>
 * <description>Event is sent before exit of the runtime system during shutdown</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpMgr_Shutdown</param>
 */
#define EVT_CmpMgr_Exit2						MAKE_EVENTID(EVTCLASS_INFO, 5)

/**
 * <category>Events</category>
 * <description>Event is sent to disable operations. Each operation is defined in the corresponding Itf.h file of the component, which is specified by its ComponentID</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpMgr_DisableOperation</param>
 */
#define EVT_CmpMgr_DisableOperation				MAKE_EVENTID(EVTCLASS_INFO, 6)

 /**
 * <category>Events</category>
 * <description>Event is sent after <see>EVT_CmpMgr_PrepareExitComm</see> and can be used if comm cycles are necessary to perform some kind
 *	of shutdown operation. The event may be called several times if one of the callees requests further calls. This can happen if the shutdown operation
 *	takes some time.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpMgr_Shutdown</param>
 */
#define EVT_CmpMgr_PrepareExitCommProcessing	MAKE_EVENTID(EVTCLASS_INFO, 7)

 /**
 * <category>Events</category>
 * <description>Event to get informed when a license was requrested. Only for informational use. 
 * No result of the event will be evaulated.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpMgr_LicenseRequest</param>
 */
#define EVT_CmpMgr_LicenseRequest				MAKE_EVENTID(EVTCLASS_INFO, 8)

/**
 * <category>CMInit options</category>
 * <element name="RTS_CMINIT_OPTION_SETTINGSFILEISOPTIONAL" type="IN">Specifies, if the Settingsfile is only used optional</element> 
 * <element name="RTS_CMINIT_OPTION_DONTCALLINITHOOKS" type="IN">If this option is set, the CMInit sequence is stopped right after CH_INIT hook.
 *	To finish the startup sequence, you have to call CMInitEnd() afterwards.
 *	This can be used, if you have to do things first before before all other components, but you need here the runtime system components.
 * </element> 
 */
#define RTS_CMINIT_OPTION_SETTINGSFILEISOPTIONAL			UINT32_C(0x00000001)
#define RTS_CMINIT_OPTION_DONTCALLINITHOOKS					UINT32_C(0x00000002)

/**
 * <category>ComponentType</category>
 * <description>
 *	A component can only be a member of type CMPTYPE_STANDARD or type CMPTYPE_SAFETY.
 *	To separate the components in detail, we use the types CMPTYPE_STATIC, CMPTYPE_DYNAMIC, CMPTYPE_SYSTEM or CMPTYPE_IEC.
 * </description>
 * <element name="CMPTYPE_STANDARD" type="IN">Standard component category</element> 
 * <element name="CMPTYPE_SAFETY" type="IN">Safety component category</element> 
 * <element name="CMPTYPE_ALL" type="IN">Can be used to select all components. Only a mask to select STANDARD and SAFETY.</element> 
 * <element name="CMPTYPE_STATIC" type="IN">Static linked component</element> 
 * <element name="CMPTYPE_DYNAMIC" type="IN">Dynamically linked component</element> 
 * <element name="CMPTYPE_SYSTEM" type="IN">System component</element> 
 * <element name="CMPTYPE_IEC" type="IN">IEC component (IEC code is registered as a component)</element> 
 * <element name="CMPTYPE_APP_CMODULE" type="IN">Application C-Module component</element> 
 */
#define CMPTYPE_STANDARD		UINT16_C(0x0100)
#define CMPTYPE_SAFETY			UINT16_C(0x0200)
#define CMPTYPE_ALL				(CMPTYPE_STANDARD | CMPTYPE_SAFETY)
#define CMPTYPE_STATIC			UINT16_C(0x0001)
#define CMPTYPE_DYNAMIC			UINT16_C(0x0002)
#define CMPTYPE_SYSTEM			UINT16_C(0x0004)
#define CMPTYPE_IEC				UINT16_C(0x0008)
#define CMPTYPE_APP_CMODULE		UINT16_C(0x0010)


/**
 * <category>ComponentState</category>
 * <element name="RTS_CS_UNKNOWN" type="IN">Unknown state</element> 
 * <element name="RTS_CS_NOT_FOUND" type="IN">Not found to load</element> 
 * <element name="RTS_CS_LOADED" type="IN">Component successfully loaded</element> 
 * <element name="RTS_CS_DISABLED" type="IN">Component disabled in the configuration</element> 
 * <element name="RTS_CS_INITIALIZED" type="IN">Component initialized</element> 
 * <element name="RTS_CS_READY" type="IN">Component loaded and initialized and is ready to use</element> 
 * <element name="RTS_CS_UNLOADED" type="IN">Component is unloaded</element> 
 */
#define RTS_CS_UNKNOWN			UINT16_C(0x0000)
#define RTS_CS_NOT_FOUND		UINT16_C(0x0001)
#define RTS_CS_LOADED			UINT16_C(0x0002)
#define RTS_CS_DISABLED			UINT16_C(0x0003)
#define RTS_CS_INITIALIZED		UINT16_C(0x0004)
#define RTS_CS_READY			UINT16_C(0x0005)
#define RTS_CS_UNLOADED			UINT16_C(0x0006)

/**
 * <category>COMM_CYCLE_HOOK types</category>
 * <description>
 *	<ul>
 *		<li>CM_HOOK_TYPE_NORMAL: Cyclic call of COMM_CYCLE_HOOK </li>
 *		<li>CM_HOOK_TYPE_FLASH_ACCESS: Additional call during flash accesses </li>
 *	</ul>
 * </description>
 */
#define CM_HOOK_TYPE_NORMAL			0
#define CM_HOOK_TYPE_FLASH_ACCESS	1


/**
 * <category>ComponentNameFlags</category>
 * <description>
 *	<ul>
 *		<li>CM_CNF_STATIC: Name is stored in a static buffer</li>
 *		<li>CM_CNF_DYNAMIC: Name is stored in a dynamic buffer</li>
 *	</ul>
 * </description>
 */
#define CM_CNF_STATIC		0
#define CM_CNF_DYNAMIC		1

/**
 * <category>ComponentName</category>
 * <description>
 *	Can be used to manage component names static or dynamic.
 * </description>
 */
typedef union ComponentNameString_
{
	char *psz;	
	char sz[MAX_COMPONENT_NAME];
} ComponentNameString;

typedef struct ComponentName_
{
	RTS_UI32 ulFlags;
	ComponentNameString string;
} ComponentName;


/* Component list */
typedef struct tagComponent_ENTRY
{
	CMPID Id;
	char *pszName;
#ifndef CM_NO_DYNAMIC_COMPONENTS	
	RTS_HANDLE hModule;
#endif
	RTS_UI32 ulVersion;
	PF_COMPONENT_ENTRY pfEntry;
	PF_EXPORT_FUNCTIONS pfExportFunctions;
	PF_IMPORT_FUNCTIONS pfImportFunctions;
	PF_GET_VERSION pfGetVersion;
	PF_HOOK_FUNCTION pfHookFunction;
	PF_CREATEINSTANCE pfCreateInstance;
	PF_DELETEINSTANCE pfDeleteInstance;
	RTS_UI16 iState;
	RTS_UI16 iType;
	RTS_UI8 bStaticName;
} COMPONENT_ENTRY;

typedef struct tagStaticComponent
{
	char *pszCmpName;
	PF_COMPONENT_ENTRY pfCmpEntry;
	int bSystem;
} StaticComponent;

typedef struct tagComponent_LIST
{
	COMPONENT_ENTRY *pComponent;
	int nCount;
} COMPONENT_LIST;

typedef struct
{
	COMPONENT_ENTRY *pCmp;
	CLASSID ClassId;
	CMPID CmpId;
} ClassEntry;

typedef struct
{
	ClassEntry *pClass;
	OBJID ObjId;
	IBase *pIBase;
} InstanceEntry;

/* Api list */
typedef struct tagApi_ENTRY
{
	char szAPIName[MAX_API_NAME];
	RTS_VOID_FCTPTR pfAPIFunction;
	int nRefCount;
	int bExternalLibrary;
	RTS_UI32 ulSignatureID;
	RTS_UI32 ulVersion;
} API_ENTRY;

typedef struct tagApi_LIST
{
	API_ENTRY *pApiList;
	int nCount;
	int nAlloc;
} API_LIST;

/**
 * <category>ApiResolveInfo</category>
 * <description>
 *	API info of the current API function to resolve. See CMGetAPIToResolve.
 * </description>
 */
typedef struct tagAPI_RESOLVE_INFO
{
    char *pszName;
    int importOptions;
    RTS_UI32 ulSignatureID;
    RTS_UI32 ulVersion;
} API_RESOLVE_INFO;


/**
 * <category>Profiler</category>
 * <description>
 *	- To use profiling, RTS_ENABLE_PROFILING must be set during compile. If it is not set, the code for profiling is empty.
 *  - To enable profile of single component hooks set RTS_ENABLE_PROFILING_COMPONENTS during compile time.
 *  - To use the profiler the SysTime component has to be linked statically to the runtime core.
 *	- All profiling information is stored in a separate logger with name "Profiler"
 *	- Profiling component can be specified in setting "Profiler.ComponentID" or during compile in CMPMGR_VALUE_INT_PROFILER_COMPONENTID_DEFAULT.
 *	- RTS_USE_PROFILING must be specified in the beginning of the file, in which profiling is used. Typically this can be specified right after USE_STMT.
 *	- RTS_PROFILING("") can be used to specify a profiling point. In the logger the absolute time and the time difference between two entries is stored.
 *	- With RTS_PROFILING_BEGIN(), RTS_PROFILING2() and RTS_PROFILING_END() a special sequence can be profiled that is marked with a unique ID. All time
 *		differences	between two profiling points with the same ID are store in the profiler.
 * </description>
 */
#if defined(RTS_ENABLE_PROFILING)
#if defined(STATIC_LINK) || defined(MIXED_LINK)	|| defined(CPLUSPLUS)
    #define RTS_USE_PROFILING		ITF_CM
#else
    #define RTS_USE_PROFILING		static PFCMPROFILING pfCMProfiling = NULL;
#endif

#define RTS_PROFILING(s)			{\
                                        IMPORT_SINGLE_ITF(CM);\
                                        if (!CHK_CMProfiling)\
                                            GET_CMProfiling(0);\
                                        if (CHK_CMProfiling)\
                                            CAL_CMProfiling(s,COMPONENT_ID,COMPONENT_NAME,__FILE__,__LINE__,-1);\
                                    }		

#ifdef RTS_ENABLE_PROFILING_COMPONENTS
#define RTS_PROFILING_COMPONENTS(s,cmp)	{\
											IMPORT_SINGLE_ITF(CM);\
											if (!CHK_CMProfiling)\
												GET_CMProfiling(0);\
											if (CHK_CMProfiling)\
											{\
												char tmp[128]; \
												CAL_CMUtlsnprintf(tmp, sizeof(tmp), s, cmp); \
												CAL_CMProfiling(tmp,COMPONENT_ID,COMPONENT_NAME,__FILE__,__LINE__,-1);\
											}\
										}
#else
#define RTS_PROFILING_COMPONENTS(s,cmp)
#endif

	#define RTS_PROFILING_BEGIN(s,id)	{\
											IMPORT_SINGLE_ITF(CM);\
											if (!CHK_CMProfiling)\
												GET_CMProfiling(0);\
											if (CHK_CMProfiling)\
												CAL_CMProfiling(s,COMPONENT_ID,COMPONENT_NAME,__FILE__,__LINE__,CM_PROFILING_ID_ADD_BEGIN(id));\
										}		

	#define RTS_PROFILING2(s,id)		{\
											IMPORT_SINGLE_ITF(CM);\
											if (!CHK_CMProfiling)\
												GET_CMProfiling(0);\
											if (CHK_CMProfiling)\
												CAL_CMProfiling(s,COMPONENT_ID,COMPONENT_NAME,__FILE__,__LINE__,id);\
										}		

	#define RTS_PROFILING_END(s,id)		{\
											IMPORT_SINGLE_ITF(CM);\
											if (!CHK_CMProfiling)\
												GET_CMProfiling(0);\
											if (CHK_CMProfiling)\
												CAL_CMProfiling(s,COMPONENT_ID,COMPONENT_NAME,__FILE__,__LINE__,CM_PROFILING_ID_ADD_END(id));\
										}		
#else
	#define RTS_USE_PROFILING
	#define RTS_PROFILING(s)
	#define RTS_PROFILING_COMPONENTS(s,cmp)
	#define RTS_PROFILING_BEGIN(s,id)
	#define RTS_PROFILING2(s,id)
	#define RTS_PROFILING_END(s,id)
#endif


/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>LicenseFunctions</description>
 */
typedef struct tagLicenseFunctions
{
	RTS_IEC_DWORD dwStructSize;		
	RTS_IEC_BYTE *pfGetUserLicenseValue;		
	RTS_IEC_BYTE *pfConfDynLicChallenge;		
	RTS_IEC_BYTE *pfReqDynLicChallenge;		
	RTS_IEC_DWORD dwVersion;		
} LicenseFunctions;

/**
 * <description>cmaddcomponent</description>
 */
typedef struct tagcmaddcomponent_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	
	RTS_IEC_UDINT udiCmpId;				/* VAR_INPUT */	
	RTS_IEC_UDINT udiVersion;			/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE CMAddComponent;		/* VAR_OUTPUT */	
} cmaddcomponent_struct;

void CDECL CDECL_EXT cmaddcomponent(cmaddcomponent_struct *p);
typedef void (CDECL CDECL_EXT* PFCMADDCOMPONENT_IEC) (cmaddcomponent_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMADDCOMPONENT_NOTIMPLEMENTED)
	#define USE_cmaddcomponent
	#define EXT_cmaddcomponent
	#define GET_cmaddcomponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmaddcomponent(p0) 
	#define CHK_cmaddcomponent  FALSE
	#define EXP_cmaddcomponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmaddcomponent
	#define EXT_cmaddcomponent
	#define GET_cmaddcomponent(fl)  CAL_CMGETAPI( "cmaddcomponent" ) 
	#define CAL_cmaddcomponent  cmaddcomponent
	#define CHK_cmaddcomponent  TRUE
	#define EXP_cmaddcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent", (RTS_UINTPTR)cmaddcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x3FB931A6), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmaddcomponent
	#define EXT_cmaddcomponent
	#define GET_cmaddcomponent(fl)  CAL_CMGETAPI( "cmaddcomponent" ) 
	#define CAL_cmaddcomponent  cmaddcomponent
	#define CHK_cmaddcomponent  TRUE
	#define EXP_cmaddcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent", (RTS_UINTPTR)cmaddcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x3FB931A6), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmaddcomponent
	#define EXT_CMcmaddcomponent
	#define GET_CMcmaddcomponent  ERR_OK
	#define CAL_CMcmaddcomponent  cmaddcomponent
	#define CHK_CMcmaddcomponent  TRUE
	#define EXP_CMcmaddcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent", (RTS_UINTPTR)cmaddcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x3FB931A6), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmaddcomponent
	#define EXT_cmaddcomponent
	#define GET_cmaddcomponent(fl)  CAL_CMGETAPI( "cmaddcomponent" ) 
	#define CAL_cmaddcomponent  cmaddcomponent
	#define CHK_cmaddcomponent  TRUE
	#define EXP_cmaddcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent", (RTS_UINTPTR)cmaddcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x3FB931A6), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmaddcomponent  PFCMADDCOMPONENT_IEC pfcmaddcomponent;
	#define EXT_cmaddcomponent  extern PFCMADDCOMPONENT_IEC pfcmaddcomponent;
	#define GET_cmaddcomponent(fl)  s_pfCMGetAPI2( "cmaddcomponent", (RTS_VOID_FCTPTR *)&pfcmaddcomponent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x3FB931A6), 0x03050D00)
	#define CAL_cmaddcomponent  pfcmaddcomponent
	#define CHK_cmaddcomponent  (pfcmaddcomponent != NULL)
	#define EXP_cmaddcomponent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent", (RTS_UINTPTR)cmaddcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x3FB931A6), 0x03050D00) 
#endif


/**
 * <description>cmaddcomponent2</description>
 */
typedef struct tagcmaddcomponent2_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	
	RTS_IEC_UDINT udiVersion;			/* VAR_INPUT */	
	RTS_IEC_UDINT *udiCmpId;			/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE CMAddComponent2;		/* VAR_OUTPUT */	
} cmaddcomponent2_struct;

void CDECL CDECL_EXT cmaddcomponent2(cmaddcomponent2_struct *p);
typedef void (CDECL CDECL_EXT* PFCMADDCOMPONENT2_IEC) (cmaddcomponent2_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMADDCOMPONENT2_NOTIMPLEMENTED)
	#define USE_cmaddcomponent2
	#define EXT_cmaddcomponent2
	#define GET_cmaddcomponent2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmaddcomponent2(p0) 
	#define CHK_cmaddcomponent2  FALSE
	#define EXP_cmaddcomponent2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmaddcomponent2
	#define EXT_cmaddcomponent2
	#define GET_cmaddcomponent2(fl)  CAL_CMGETAPI( "cmaddcomponent2" ) 
	#define CAL_cmaddcomponent2  cmaddcomponent2
	#define CHK_cmaddcomponent2  TRUE
	#define EXP_cmaddcomponent2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent2", (RTS_UINTPTR)cmaddcomponent2, 1, 0x1057D488, 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmaddcomponent2
	#define EXT_cmaddcomponent2
	#define GET_cmaddcomponent2(fl)  CAL_CMGETAPI( "cmaddcomponent2" ) 
	#define CAL_cmaddcomponent2  cmaddcomponent2
	#define CHK_cmaddcomponent2  TRUE
	#define EXP_cmaddcomponent2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent2", (RTS_UINTPTR)cmaddcomponent2, 1, 0x1057D488, 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmaddcomponent2
	#define EXT_CMcmaddcomponent2
	#define GET_CMcmaddcomponent2  ERR_OK
	#define CAL_CMcmaddcomponent2  cmaddcomponent2
	#define CHK_CMcmaddcomponent2  TRUE
	#define EXP_CMcmaddcomponent2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent2", (RTS_UINTPTR)cmaddcomponent2, 1, 0x1057D488, 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmaddcomponent2
	#define EXT_cmaddcomponent2
	#define GET_cmaddcomponent2(fl)  CAL_CMGETAPI( "cmaddcomponent2" ) 
	#define CAL_cmaddcomponent2  cmaddcomponent2
	#define CHK_cmaddcomponent2  TRUE
	#define EXP_cmaddcomponent2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent2", (RTS_UINTPTR)cmaddcomponent2, 1, 0x1057D488, 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmaddcomponent2  PFCMADDCOMPONENT2_IEC pfcmaddcomponent2;
	#define EXT_cmaddcomponent2  extern PFCMADDCOMPONENT2_IEC pfcmaddcomponent2;
	#define GET_cmaddcomponent2(fl)  s_pfCMGetAPI2( "cmaddcomponent2", (RTS_VOID_FCTPTR *)&pfcmaddcomponent2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x1057D488, 0x03050D00)
	#define CAL_cmaddcomponent2  pfcmaddcomponent2
	#define CHK_cmaddcomponent2  (pfcmaddcomponent2 != NULL)
	#define EXP_cmaddcomponent2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmaddcomponent2", (RTS_UINTPTR)cmaddcomponent2, 1, 0x1057D488, 0x03050D00) 
#endif


/**
 * <description>cmexitcomponent</description>
 */
typedef struct tagcmexitcomponent_struct
{
	RTS_IEC_HANDLE hComponent;			/* VAR_INPUT */	
	RTS_IEC_RESULT CMExitComponent;		/* VAR_OUTPUT */	
} cmexitcomponent_struct;

void CDECL CDECL_EXT cmexitcomponent(cmexitcomponent_struct *p);
typedef void (CDECL CDECL_EXT* PFCMEXITCOMPONENT_IEC) (cmexitcomponent_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMEXITCOMPONENT_NOTIMPLEMENTED)
	#define USE_cmexitcomponent
	#define EXT_cmexitcomponent
	#define GET_cmexitcomponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmexitcomponent(p0) 
	#define CHK_cmexitcomponent  FALSE
	#define EXP_cmexitcomponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmexitcomponent
	#define EXT_cmexitcomponent
	#define GET_cmexitcomponent(fl)  CAL_CMGETAPI( "cmexitcomponent" ) 
	#define CAL_cmexitcomponent  cmexitcomponent
	#define CHK_cmexitcomponent  TRUE
	#define EXP_cmexitcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmexitcomponent", (RTS_UINTPTR)cmexitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x0DDC8E7F), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmexitcomponent
	#define EXT_cmexitcomponent
	#define GET_cmexitcomponent(fl)  CAL_CMGETAPI( "cmexitcomponent" ) 
	#define CAL_cmexitcomponent  cmexitcomponent
	#define CHK_cmexitcomponent  TRUE
	#define EXP_cmexitcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmexitcomponent", (RTS_UINTPTR)cmexitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x0DDC8E7F), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmexitcomponent
	#define EXT_CMcmexitcomponent
	#define GET_CMcmexitcomponent  ERR_OK
	#define CAL_CMcmexitcomponent  cmexitcomponent
	#define CHK_CMcmexitcomponent  TRUE
	#define EXP_CMcmexitcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmexitcomponent", (RTS_UINTPTR)cmexitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x0DDC8E7F), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmexitcomponent
	#define EXT_cmexitcomponent
	#define GET_cmexitcomponent(fl)  CAL_CMGETAPI( "cmexitcomponent" ) 
	#define CAL_cmexitcomponent  cmexitcomponent
	#define CHK_cmexitcomponent  TRUE
	#define EXP_cmexitcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmexitcomponent", (RTS_UINTPTR)cmexitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x0DDC8E7F), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmexitcomponent  PFCMEXITCOMPONENT_IEC pfcmexitcomponent;
	#define EXT_cmexitcomponent  extern PFCMEXITCOMPONENT_IEC pfcmexitcomponent;
	#define GET_cmexitcomponent(fl)  s_pfCMGetAPI2( "cmexitcomponent", (RTS_VOID_FCTPTR *)&pfcmexitcomponent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x0DDC8E7F), 0x03050D00)
	#define CAL_cmexitcomponent  pfcmexitcomponent
	#define CHK_cmexitcomponent  (pfcmexitcomponent != NULL)
	#define EXP_cmexitcomponent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmexitcomponent", (RTS_UINTPTR)cmexitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0x0DDC8E7F), 0x03050D00) 
#endif


/**
 * <description>cmgetcomponentbyname</description>
 */
typedef struct tagcmgetcomponentbyname_struct
{
	RTS_IEC_STRING *pszComponent;		/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE CMGetComponentByName;	/* VAR_OUTPUT */	
} cmgetcomponentbyname_struct;

void CDECL CDECL_EXT cmgetcomponentbyname(cmgetcomponentbyname_struct *p);
typedef void (CDECL CDECL_EXT* PFCMGETCOMPONENTBYNAME_IEC) (cmgetcomponentbyname_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETCOMPONENTBYNAME_NOTIMPLEMENTED)
	#define USE_cmgetcomponentbyname
	#define EXT_cmgetcomponentbyname
	#define GET_cmgetcomponentbyname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmgetcomponentbyname(p0) 
	#define CHK_cmgetcomponentbyname  FALSE
	#define EXP_cmgetcomponentbyname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmgetcomponentbyname
	#define EXT_cmgetcomponentbyname
	#define GET_cmgetcomponentbyname(fl)  CAL_CMGETAPI( "cmgetcomponentbyname" ) 
	#define CAL_cmgetcomponentbyname  cmgetcomponentbyname
	#define CHK_cmgetcomponentbyname  TRUE
	#define EXP_cmgetcomponentbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmgetcomponentbyname", (RTS_UINTPTR)cmgetcomponentbyname, 1, RTSITF_GET_SIGNATURE(0, 0x1C90B550), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmgetcomponentbyname
	#define EXT_cmgetcomponentbyname
	#define GET_cmgetcomponentbyname(fl)  CAL_CMGETAPI( "cmgetcomponentbyname" ) 
	#define CAL_cmgetcomponentbyname  cmgetcomponentbyname
	#define CHK_cmgetcomponentbyname  TRUE
	#define EXP_cmgetcomponentbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmgetcomponentbyname", (RTS_UINTPTR)cmgetcomponentbyname, 1, RTSITF_GET_SIGNATURE(0, 0x1C90B550), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmgetcomponentbyname
	#define EXT_CMcmgetcomponentbyname
	#define GET_CMcmgetcomponentbyname  ERR_OK
	#define CAL_CMcmgetcomponentbyname  cmgetcomponentbyname
	#define CHK_CMcmgetcomponentbyname  TRUE
	#define EXP_CMcmgetcomponentbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmgetcomponentbyname", (RTS_UINTPTR)cmgetcomponentbyname, 1, RTSITF_GET_SIGNATURE(0, 0x1C90B550), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmgetcomponentbyname
	#define EXT_cmgetcomponentbyname
	#define GET_cmgetcomponentbyname(fl)  CAL_CMGETAPI( "cmgetcomponentbyname" ) 
	#define CAL_cmgetcomponentbyname  cmgetcomponentbyname
	#define CHK_cmgetcomponentbyname  TRUE
	#define EXP_cmgetcomponentbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmgetcomponentbyname", (RTS_UINTPTR)cmgetcomponentbyname, 1, RTSITF_GET_SIGNATURE(0, 0x1C90B550), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmgetcomponentbyname  PFCMGETCOMPONENTBYNAME_IEC pfcmgetcomponentbyname;
	#define EXT_cmgetcomponentbyname  extern PFCMGETCOMPONENTBYNAME_IEC pfcmgetcomponentbyname;
	#define GET_cmgetcomponentbyname(fl)  s_pfCMGetAPI2( "cmgetcomponentbyname", (RTS_VOID_FCTPTR *)&pfcmgetcomponentbyname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x1C90B550), 0x03050D00)
	#define CAL_cmgetcomponentbyname  pfcmgetcomponentbyname
	#define CHK_cmgetcomponentbyname  (pfcmgetcomponentbyname != NULL)
	#define EXP_cmgetcomponentbyname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmgetcomponentbyname", (RTS_UINTPTR)cmgetcomponentbyname, 1, RTSITF_GET_SIGNATURE(0, 0x1C90B550), 0x03050D00) 
#endif


/**
 * <description>cminitcomponent</description>
 */
typedef struct tagcminitcomponent_struct
{
	RTS_IEC_HANDLE hComponent;			/* VAR_INPUT */	
	RTS_IEC_RESULT CMInitComponent;		/* VAR_OUTPUT */	
} cminitcomponent_struct;

void CDECL CDECL_EXT cminitcomponent(cminitcomponent_struct *p);
typedef void (CDECL CDECL_EXT* PFCMINITCOMPONENT_IEC) (cminitcomponent_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMINITCOMPONENT_NOTIMPLEMENTED)
	#define USE_cminitcomponent
	#define EXT_cminitcomponent
	#define GET_cminitcomponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cminitcomponent(p0) 
	#define CHK_cminitcomponent  FALSE
	#define EXP_cminitcomponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cminitcomponent
	#define EXT_cminitcomponent
	#define GET_cminitcomponent(fl)  CAL_CMGETAPI( "cminitcomponent" ) 
	#define CAL_cminitcomponent  cminitcomponent
	#define CHK_cminitcomponent  TRUE
	#define EXP_cminitcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cminitcomponent", (RTS_UINTPTR)cminitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0xAD863340), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cminitcomponent
	#define EXT_cminitcomponent
	#define GET_cminitcomponent(fl)  CAL_CMGETAPI( "cminitcomponent" ) 
	#define CAL_cminitcomponent  cminitcomponent
	#define CHK_cminitcomponent  TRUE
	#define EXP_cminitcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cminitcomponent", (RTS_UINTPTR)cminitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0xAD863340), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcminitcomponent
	#define EXT_CMcminitcomponent
	#define GET_CMcminitcomponent  ERR_OK
	#define CAL_CMcminitcomponent  cminitcomponent
	#define CHK_CMcminitcomponent  TRUE
	#define EXP_CMcminitcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cminitcomponent", (RTS_UINTPTR)cminitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0xAD863340), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cminitcomponent
	#define EXT_cminitcomponent
	#define GET_cminitcomponent(fl)  CAL_CMGETAPI( "cminitcomponent" ) 
	#define CAL_cminitcomponent  cminitcomponent
	#define CHK_cminitcomponent  TRUE
	#define EXP_cminitcomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cminitcomponent", (RTS_UINTPTR)cminitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0xAD863340), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cminitcomponent  PFCMINITCOMPONENT_IEC pfcminitcomponent;
	#define EXT_cminitcomponent  extern PFCMINITCOMPONENT_IEC pfcminitcomponent;
	#define GET_cminitcomponent(fl)  s_pfCMGetAPI2( "cminitcomponent", (RTS_VOID_FCTPTR *)&pfcminitcomponent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xAD863340), 0x03050D00)
	#define CAL_cminitcomponent  pfcminitcomponent
	#define CHK_cminitcomponent  (pfcminitcomponent != NULL)
	#define EXP_cminitcomponent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cminitcomponent", (RTS_UINTPTR)cminitcomponent, 1, RTSITF_GET_SIGNATURE(0, 0xAD863340), 0x03050D00) 
#endif


/**
 * OBSOLETE FUNCTION: Use CMRegisterLicenseFunctions instead
 */
typedef struct tagcmregistergetuserlicensevalue_struct
{
	RTS_IEC_BYTE *pfGetUserLicenseValue;	/* VAR_INPUT */	
	RTS_IEC_RESULT CMRegisterGetUserLicenseValue;	/* VAR_OUTPUT */	
} cmregistergetuserlicensevalue_struct;

void CDECL CDECL_EXT cmregistergetuserlicensevalue(cmregistergetuserlicensevalue_struct *p);
typedef void (CDECL CDECL_EXT* PFCMREGISTERGETUSERLICENSEVALUE_IEC) (cmregistergetuserlicensevalue_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMREGISTERGETUSERLICENSEVALUE_NOTIMPLEMENTED)
	#define USE_cmregistergetuserlicensevalue
	#define EXT_cmregistergetuserlicensevalue
	#define GET_cmregistergetuserlicensevalue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmregistergetuserlicensevalue(p0) 
	#define CHK_cmregistergetuserlicensevalue  FALSE
	#define EXP_cmregistergetuserlicensevalue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmregistergetuserlicensevalue
	#define EXT_cmregistergetuserlicensevalue
	#define GET_cmregistergetuserlicensevalue(fl)  CAL_CMGETAPI( "cmregistergetuserlicensevalue" ) 
	#define CAL_cmregistergetuserlicensevalue  cmregistergetuserlicensevalue
	#define CHK_cmregistergetuserlicensevalue  TRUE
	#define EXP_cmregistergetuserlicensevalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregistergetuserlicensevalue", (RTS_UINTPTR)cmregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0x84364FC6), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmregistergetuserlicensevalue
	#define EXT_cmregistergetuserlicensevalue
	#define GET_cmregistergetuserlicensevalue(fl)  CAL_CMGETAPI( "cmregistergetuserlicensevalue" ) 
	#define CAL_cmregistergetuserlicensevalue  cmregistergetuserlicensevalue
	#define CHK_cmregistergetuserlicensevalue  TRUE
	#define EXP_cmregistergetuserlicensevalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregistergetuserlicensevalue", (RTS_UINTPTR)cmregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0x84364FC6), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmregistergetuserlicensevalue
	#define EXT_CMcmregistergetuserlicensevalue
	#define GET_CMcmregistergetuserlicensevalue  ERR_OK
	#define CAL_CMcmregistergetuserlicensevalue  cmregistergetuserlicensevalue
	#define CHK_CMcmregistergetuserlicensevalue  TRUE
	#define EXP_CMcmregistergetuserlicensevalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregistergetuserlicensevalue", (RTS_UINTPTR)cmregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0x84364FC6), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmregistergetuserlicensevalue
	#define EXT_cmregistergetuserlicensevalue
	#define GET_cmregistergetuserlicensevalue(fl)  CAL_CMGETAPI( "cmregistergetuserlicensevalue" ) 
	#define CAL_cmregistergetuserlicensevalue  cmregistergetuserlicensevalue
	#define CHK_cmregistergetuserlicensevalue  TRUE
	#define EXP_cmregistergetuserlicensevalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregistergetuserlicensevalue", (RTS_UINTPTR)cmregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0x84364FC6), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmregistergetuserlicensevalue  PFCMREGISTERGETUSERLICENSEVALUE_IEC pfcmregistergetuserlicensevalue;
	#define EXT_cmregistergetuserlicensevalue  extern PFCMREGISTERGETUSERLICENSEVALUE_IEC pfcmregistergetuserlicensevalue;
	#define GET_cmregistergetuserlicensevalue(fl)  s_pfCMGetAPI2( "cmregistergetuserlicensevalue", (RTS_VOID_FCTPTR *)&pfcmregistergetuserlicensevalue, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x84364FC6), 0x03050D00)
	#define CAL_cmregistergetuserlicensevalue  pfcmregistergetuserlicensevalue
	#define CHK_cmregistergetuserlicensevalue  (pfcmregistergetuserlicensevalue != NULL)
	#define EXP_cmregistergetuserlicensevalue   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregistergetuserlicensevalue", (RTS_UINTPTR)cmregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0x84364FC6), 0x03050D00) 
#endif


/**
 * <description>cmregisterlicensefunctions</description>
 */
typedef struct tagcmregisterlicensefunctions_struct
{
	LicenseFunctions licenseFunctions;	/* VAR_INPUT */	
	RTS_IEC_RESULT CMRegisterLicenseFunctions;	/* VAR_OUTPUT */	
} cmregisterlicensefunctions_struct;

void CDECL CDECL_EXT cmregisterlicensefunctions(cmregisterlicensefunctions_struct *p);
typedef void (CDECL CDECL_EXT* PFCMREGISTERLICENSEFUNCTIONS_IEC) (cmregisterlicensefunctions_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMREGISTERLICENSEFUNCTIONS_NOTIMPLEMENTED)
	#define USE_cmregisterlicensefunctions
	#define EXT_cmregisterlicensefunctions
	#define GET_cmregisterlicensefunctions(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmregisterlicensefunctions(p0) 
	#define CHK_cmregisterlicensefunctions  FALSE
	#define EXP_cmregisterlicensefunctions  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmregisterlicensefunctions
	#define EXT_cmregisterlicensefunctions
	#define GET_cmregisterlicensefunctions(fl)  CAL_CMGETAPI( "cmregisterlicensefunctions" ) 
	#define CAL_cmregisterlicensefunctions  cmregisterlicensefunctions
	#define CHK_cmregisterlicensefunctions  TRUE
	#define EXP_cmregisterlicensefunctions  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregisterlicensefunctions", (RTS_UINTPTR)cmregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0xC18CCC88), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmregisterlicensefunctions
	#define EXT_cmregisterlicensefunctions
	#define GET_cmregisterlicensefunctions(fl)  CAL_CMGETAPI( "cmregisterlicensefunctions" ) 
	#define CAL_cmregisterlicensefunctions  cmregisterlicensefunctions
	#define CHK_cmregisterlicensefunctions  TRUE
	#define EXP_cmregisterlicensefunctions  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregisterlicensefunctions", (RTS_UINTPTR)cmregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0xC18CCC88), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmregisterlicensefunctions
	#define EXT_CMcmregisterlicensefunctions
	#define GET_CMcmregisterlicensefunctions  ERR_OK
	#define CAL_CMcmregisterlicensefunctions  cmregisterlicensefunctions
	#define CHK_CMcmregisterlicensefunctions  TRUE
	#define EXP_CMcmregisterlicensefunctions  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregisterlicensefunctions", (RTS_UINTPTR)cmregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0xC18CCC88), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmregisterlicensefunctions
	#define EXT_cmregisterlicensefunctions
	#define GET_cmregisterlicensefunctions(fl)  CAL_CMGETAPI( "cmregisterlicensefunctions" ) 
	#define CAL_cmregisterlicensefunctions  cmregisterlicensefunctions
	#define CHK_cmregisterlicensefunctions  TRUE
	#define EXP_cmregisterlicensefunctions  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregisterlicensefunctions", (RTS_UINTPTR)cmregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0xC18CCC88), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmregisterlicensefunctions  PFCMREGISTERLICENSEFUNCTIONS_IEC pfcmregisterlicensefunctions;
	#define EXT_cmregisterlicensefunctions  extern PFCMREGISTERLICENSEFUNCTIONS_IEC pfcmregisterlicensefunctions;
	#define GET_cmregisterlicensefunctions(fl)  s_pfCMGetAPI2( "cmregisterlicensefunctions", (RTS_VOID_FCTPTR *)&pfcmregisterlicensefunctions, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC18CCC88), 0x03050D00)
	#define CAL_cmregisterlicensefunctions  pfcmregisterlicensefunctions
	#define CHK_cmregisterlicensefunctions  (pfcmregisterlicensefunctions != NULL)
	#define EXP_cmregisterlicensefunctions   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmregisterlicensefunctions", (RTS_UINTPTR)cmregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0xC18CCC88), 0x03050D00) 
#endif


/**
 * <description>cmremovecomponent</description>
 */
typedef struct tagcmremovecomponent_struct
{
	RTS_IEC_HANDLE hComponent;			/* VAR_INPUT */	
	RTS_IEC_RESULT CMRemoveComponent;	/* VAR_OUTPUT */	
} cmremovecomponent_struct;

void CDECL CDECL_EXT cmremovecomponent(cmremovecomponent_struct *p);
typedef void (CDECL CDECL_EXT* PFCMREMOVECOMPONENT_IEC) (cmremovecomponent_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMREMOVECOMPONENT_NOTIMPLEMENTED)
	#define USE_cmremovecomponent
	#define EXT_cmremovecomponent
	#define GET_cmremovecomponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmremovecomponent(p0) 
	#define CHK_cmremovecomponent  FALSE
	#define EXP_cmremovecomponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmremovecomponent
	#define EXT_cmremovecomponent
	#define GET_cmremovecomponent(fl)  CAL_CMGETAPI( "cmremovecomponent" ) 
	#define CAL_cmremovecomponent  cmremovecomponent
	#define CHK_cmremovecomponent  TRUE
	#define EXP_cmremovecomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmremovecomponent", (RTS_UINTPTR)cmremovecomponent, 1, RTSITF_GET_SIGNATURE(0, 0xB53B03F4), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmremovecomponent
	#define EXT_cmremovecomponent
	#define GET_cmremovecomponent(fl)  CAL_CMGETAPI( "cmremovecomponent" ) 
	#define CAL_cmremovecomponent  cmremovecomponent
	#define CHK_cmremovecomponent  TRUE
	#define EXP_cmremovecomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmremovecomponent", (RTS_UINTPTR)cmremovecomponent, 1, RTSITF_GET_SIGNATURE(0, 0xB53B03F4), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmremovecomponent
	#define EXT_CMcmremovecomponent
	#define GET_CMcmremovecomponent  ERR_OK
	#define CAL_CMcmremovecomponent  cmremovecomponent
	#define CHK_CMcmremovecomponent  TRUE
	#define EXP_CMcmremovecomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmremovecomponent", (RTS_UINTPTR)cmremovecomponent, 1, RTSITF_GET_SIGNATURE(0, 0xB53B03F4), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmremovecomponent
	#define EXT_cmremovecomponent
	#define GET_cmremovecomponent(fl)  CAL_CMGETAPI( "cmremovecomponent" ) 
	#define CAL_cmremovecomponent  cmremovecomponent
	#define CHK_cmremovecomponent  TRUE
	#define EXP_cmremovecomponent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmremovecomponent", (RTS_UINTPTR)cmremovecomponent, 1, RTSITF_GET_SIGNATURE(0, 0xB53B03F4), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmremovecomponent  PFCMREMOVECOMPONENT_IEC pfcmremovecomponent;
	#define EXT_cmremovecomponent  extern PFCMREMOVECOMPONENT_IEC pfcmremovecomponent;
	#define GET_cmremovecomponent(fl)  s_pfCMGetAPI2( "cmremovecomponent", (RTS_VOID_FCTPTR *)&pfcmremovecomponent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB53B03F4), 0x03050D00)
	#define CAL_cmremovecomponent  pfcmremovecomponent
	#define CHK_cmremovecomponent  (pfcmremovecomponent != NULL)
	#define EXP_cmremovecomponent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmremovecomponent", (RTS_UINTPTR)cmremovecomponent, 1, RTSITF_GET_SIGNATURE(0, 0xB53B03F4), 0x03050D00) 
#endif


/**
 * <description>cmshutdown</description>
 */
typedef struct tagcmshutdown_struct
{
	RTS_IEC_UDINT dwReason;				/* VAR_INPUT */	
	RTS_IEC_RESULT CMShutDown;			/* VAR_OUTPUT */	
} cmshutdown_struct;

void CDECL CDECL_EXT cmshutdown(cmshutdown_struct *p);
typedef void (CDECL CDECL_EXT* PFCMSHUTDOWN_IEC) (cmshutdown_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMSHUTDOWN_NOTIMPLEMENTED)
	#define USE_cmshutdown
	#define EXT_cmshutdown
	#define GET_cmshutdown(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmshutdown(p0) 
	#define CHK_cmshutdown  FALSE
	#define EXP_cmshutdown  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmshutdown
	#define EXT_cmshutdown
	#define GET_cmshutdown(fl)  CAL_CMGETAPI( "cmshutdown" ) 
	#define CAL_cmshutdown  cmshutdown
	#define CHK_cmshutdown  TRUE
	#define EXP_cmshutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmshutdown", (RTS_UINTPTR)cmshutdown, 1, RTSITF_GET_SIGNATURE(0, 0x28C83A52), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmshutdown
	#define EXT_cmshutdown
	#define GET_cmshutdown(fl)  CAL_CMGETAPI( "cmshutdown" ) 
	#define CAL_cmshutdown  cmshutdown
	#define CHK_cmshutdown  TRUE
	#define EXP_cmshutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmshutdown", (RTS_UINTPTR)cmshutdown, 1, RTSITF_GET_SIGNATURE(0, 0x28C83A52), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmshutdown
	#define EXT_CMcmshutdown
	#define GET_CMcmshutdown  ERR_OK
	#define CAL_CMcmshutdown  cmshutdown
	#define CHK_CMcmshutdown  TRUE
	#define EXP_CMcmshutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmshutdown", (RTS_UINTPTR)cmshutdown, 1, RTSITF_GET_SIGNATURE(0, 0x28C83A52), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmshutdown
	#define EXT_cmshutdown
	#define GET_cmshutdown(fl)  CAL_CMGETAPI( "cmshutdown" ) 
	#define CAL_cmshutdown  cmshutdown
	#define CHK_cmshutdown  TRUE
	#define EXP_cmshutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmshutdown", (RTS_UINTPTR)cmshutdown, 1, RTSITF_GET_SIGNATURE(0, 0x28C83A52), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmshutdown  PFCMSHUTDOWN_IEC pfcmshutdown;
	#define EXT_cmshutdown  extern PFCMSHUTDOWN_IEC pfcmshutdown;
	#define GET_cmshutdown(fl)  s_pfCMGetAPI2( "cmshutdown", (RTS_VOID_FCTPTR *)&pfcmshutdown, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x28C83A52), 0x03050D00)
	#define CAL_cmshutdown  pfcmshutdown
	#define CHK_cmshutdown  (pfcmshutdown != NULL)
	#define EXP_cmshutdown   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmshutdown", (RTS_UINTPTR)cmshutdown, 1, RTSITF_GET_SIGNATURE(0, 0x28C83A52), 0x03050D00) 
#endif


/**
 * OBSOLETE FUNCTION: Use CMUnregisterLicenseFunctions instead
 */
typedef struct tagcmunregistergetuserlicensevalue_struct
{
	RTS_IEC_BYTE *pfGetUserLicenseValue;	/* VAR_INPUT */	
	RTS_IEC_RESULT CMUnregisterGetUserLicenseValue;	/* VAR_OUTPUT */	
} cmunregistergetuserlicensevalue_struct;

void CDECL CDECL_EXT cmunregistergetuserlicensevalue(cmunregistergetuserlicensevalue_struct *p);
typedef void (CDECL CDECL_EXT* PFCMUNREGISTERGETUSERLICENSEVALUE_IEC) (cmunregistergetuserlicensevalue_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUNREGISTERGETUSERLICENSEVALUE_NOTIMPLEMENTED)
	#define USE_cmunregistergetuserlicensevalue
	#define EXT_cmunregistergetuserlicensevalue
	#define GET_cmunregistergetuserlicensevalue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmunregistergetuserlicensevalue(p0) 
	#define CHK_cmunregistergetuserlicensevalue  FALSE
	#define EXP_cmunregistergetuserlicensevalue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmunregistergetuserlicensevalue
	#define EXT_cmunregistergetuserlicensevalue
	#define GET_cmunregistergetuserlicensevalue(fl)  CAL_CMGETAPI( "cmunregistergetuserlicensevalue" ) 
	#define CAL_cmunregistergetuserlicensevalue  cmunregistergetuserlicensevalue
	#define CHK_cmunregistergetuserlicensevalue  TRUE
	#define EXP_cmunregistergetuserlicensevalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregistergetuserlicensevalue", (RTS_UINTPTR)cmunregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0xE400F55F), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmunregistergetuserlicensevalue
	#define EXT_cmunregistergetuserlicensevalue
	#define GET_cmunregistergetuserlicensevalue(fl)  CAL_CMGETAPI( "cmunregistergetuserlicensevalue" ) 
	#define CAL_cmunregistergetuserlicensevalue  cmunregistergetuserlicensevalue
	#define CHK_cmunregistergetuserlicensevalue  TRUE
	#define EXP_cmunregistergetuserlicensevalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregistergetuserlicensevalue", (RTS_UINTPTR)cmunregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0xE400F55F), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmunregistergetuserlicensevalue
	#define EXT_CMcmunregistergetuserlicensevalue
	#define GET_CMcmunregistergetuserlicensevalue  ERR_OK
	#define CAL_CMcmunregistergetuserlicensevalue  cmunregistergetuserlicensevalue
	#define CHK_CMcmunregistergetuserlicensevalue  TRUE
	#define EXP_CMcmunregistergetuserlicensevalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregistergetuserlicensevalue", (RTS_UINTPTR)cmunregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0xE400F55F), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmunregistergetuserlicensevalue
	#define EXT_cmunregistergetuserlicensevalue
	#define GET_cmunregistergetuserlicensevalue(fl)  CAL_CMGETAPI( "cmunregistergetuserlicensevalue" ) 
	#define CAL_cmunregistergetuserlicensevalue  cmunregistergetuserlicensevalue
	#define CHK_cmunregistergetuserlicensevalue  TRUE
	#define EXP_cmunregistergetuserlicensevalue  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregistergetuserlicensevalue", (RTS_UINTPTR)cmunregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0xE400F55F), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmunregistergetuserlicensevalue  PFCMUNREGISTERGETUSERLICENSEVALUE_IEC pfcmunregistergetuserlicensevalue;
	#define EXT_cmunregistergetuserlicensevalue  extern PFCMUNREGISTERGETUSERLICENSEVALUE_IEC pfcmunregistergetuserlicensevalue;
	#define GET_cmunregistergetuserlicensevalue(fl)  s_pfCMGetAPI2( "cmunregistergetuserlicensevalue", (RTS_VOID_FCTPTR *)&pfcmunregistergetuserlicensevalue, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xE400F55F), 0x03050D00)
	#define CAL_cmunregistergetuserlicensevalue  pfcmunregistergetuserlicensevalue
	#define CHK_cmunregistergetuserlicensevalue  (pfcmunregistergetuserlicensevalue != NULL)
	#define EXP_cmunregistergetuserlicensevalue   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregistergetuserlicensevalue", (RTS_UINTPTR)cmunregistergetuserlicensevalue, 1, RTSITF_GET_SIGNATURE(0, 0xE400F55F), 0x03050D00) 
#endif


/**
 * <description>cmunregisterlicensefunctions</description>
 */
typedef struct tagcmunregisterlicensefunctions_struct
{
	LicenseFunctions licenseFunctions;	/* VAR_INPUT */	
	RTS_IEC_RESULT CMUnregisterLicenseFunctions;	/* VAR_OUTPUT */	
} cmunregisterlicensefunctions_struct;

void CDECL CDECL_EXT cmunregisterlicensefunctions(cmunregisterlicensefunctions_struct *p);
typedef void (CDECL CDECL_EXT* PFCMUNREGISTERLICENSEFUNCTIONS_IEC) (cmunregisterlicensefunctions_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUNREGISTERLICENSEFUNCTIONS_NOTIMPLEMENTED)
	#define USE_cmunregisterlicensefunctions
	#define EXT_cmunregisterlicensefunctions
	#define GET_cmunregisterlicensefunctions(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmunregisterlicensefunctions(p0) 
	#define CHK_cmunregisterlicensefunctions  FALSE
	#define EXP_cmunregisterlicensefunctions  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmunregisterlicensefunctions
	#define EXT_cmunregisterlicensefunctions
	#define GET_cmunregisterlicensefunctions(fl)  CAL_CMGETAPI( "cmunregisterlicensefunctions" ) 
	#define CAL_cmunregisterlicensefunctions  cmunregisterlicensefunctions
	#define CHK_cmunregisterlicensefunctions  TRUE
	#define EXP_cmunregisterlicensefunctions  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregisterlicensefunctions", (RTS_UINTPTR)cmunregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0x7144A275), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmunregisterlicensefunctions
	#define EXT_cmunregisterlicensefunctions
	#define GET_cmunregisterlicensefunctions(fl)  CAL_CMGETAPI( "cmunregisterlicensefunctions" ) 
	#define CAL_cmunregisterlicensefunctions  cmunregisterlicensefunctions
	#define CHK_cmunregisterlicensefunctions  TRUE
	#define EXP_cmunregisterlicensefunctions  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregisterlicensefunctions", (RTS_UINTPTR)cmunregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0x7144A275), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmunregisterlicensefunctions
	#define EXT_CMcmunregisterlicensefunctions
	#define GET_CMcmunregisterlicensefunctions  ERR_OK
	#define CAL_CMcmunregisterlicensefunctions  cmunregisterlicensefunctions
	#define CHK_CMcmunregisterlicensefunctions  TRUE
	#define EXP_CMcmunregisterlicensefunctions  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregisterlicensefunctions", (RTS_UINTPTR)cmunregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0x7144A275), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmunregisterlicensefunctions
	#define EXT_cmunregisterlicensefunctions
	#define GET_cmunregisterlicensefunctions(fl)  CAL_CMGETAPI( "cmunregisterlicensefunctions" ) 
	#define CAL_cmunregisterlicensefunctions  cmunregisterlicensefunctions
	#define CHK_cmunregisterlicensefunctions  TRUE
	#define EXP_cmunregisterlicensefunctions  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregisterlicensefunctions", (RTS_UINTPTR)cmunregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0x7144A275), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmunregisterlicensefunctions  PFCMUNREGISTERLICENSEFUNCTIONS_IEC pfcmunregisterlicensefunctions;
	#define EXT_cmunregisterlicensefunctions  extern PFCMUNREGISTERLICENSEFUNCTIONS_IEC pfcmunregisterlicensefunctions;
	#define GET_cmunregisterlicensefunctions(fl)  s_pfCMGetAPI2( "cmunregisterlicensefunctions", (RTS_VOID_FCTPTR *)&pfcmunregisterlicensefunctions, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x7144A275), 0x03050D00)
	#define CAL_cmunregisterlicensefunctions  pfcmunregisterlicensefunctions
	#define CHK_cmunregisterlicensefunctions  (pfcmunregisterlicensefunctions != NULL)
	#define EXP_cmunregisterlicensefunctions   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmunregisterlicensefunctions", (RTS_UINTPTR)cmunregisterlicensefunctions, 1, RTSITF_GET_SIGNATURE(0, 0x7144A275), 0x03050D00) 
#endif


/**
 * <description>cmutlcwstrcpy</description>
 */
typedef struct tagcmutlcwstrcpy_struct
{
	RTS_IEC_CWCHAR *pcwszDest;			/* VAR_INPUT */	
	RTS_IEC_XINT nDestSize;				/* VAR_INPUT */	
	RTS_IEC_CWCHAR *pcwszSrc;			/* VAR_INPUT */	
	RTS_IEC_RESULT CMUtlcwstrcpy;		/* VAR_OUTPUT */	
} cmutlcwstrcpy_struct;

void CDECL CDECL_EXT cmutlcwstrcpy(cmutlcwstrcpy_struct *p);
typedef void (CDECL CDECL_EXT* PFCMUTLCWSTRCPY_IEC) (cmutlcwstrcpy_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUTLCWSTRCPY_NOTIMPLEMENTED)
	#define USE_cmutlcwstrcpy
	#define EXT_cmutlcwstrcpy
	#define GET_cmutlcwstrcpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmutlcwstrcpy(p0) 
	#define CHK_cmutlcwstrcpy  FALSE
	#define EXP_cmutlcwstrcpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmutlcwstrcpy
	#define EXT_cmutlcwstrcpy
	#define GET_cmutlcwstrcpy(fl)  CAL_CMGETAPI( "cmutlcwstrcpy" ) 
	#define CAL_cmutlcwstrcpy  cmutlcwstrcpy
	#define CHK_cmutlcwstrcpy  TRUE
	#define EXP_cmutlcwstrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlcwstrcpy", (RTS_UINTPTR)cmutlcwstrcpy, 1, RTSITF_GET_SIGNATURE(0x09DB4923, 0xCCB6DDB5), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmutlcwstrcpy
	#define EXT_cmutlcwstrcpy
	#define GET_cmutlcwstrcpy(fl)  CAL_CMGETAPI( "cmutlcwstrcpy" ) 
	#define CAL_cmutlcwstrcpy  cmutlcwstrcpy
	#define CHK_cmutlcwstrcpy  TRUE
	#define EXP_cmutlcwstrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlcwstrcpy", (RTS_UINTPTR)cmutlcwstrcpy, 1, RTSITF_GET_SIGNATURE(0x09DB4923, 0xCCB6DDB5), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmutlcwstrcpy
	#define EXT_CMcmutlcwstrcpy
	#define GET_CMcmutlcwstrcpy  ERR_OK
	#define CAL_CMcmutlcwstrcpy  cmutlcwstrcpy
	#define CHK_CMcmutlcwstrcpy  TRUE
	#define EXP_CMcmutlcwstrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlcwstrcpy", (RTS_UINTPTR)cmutlcwstrcpy, 1, RTSITF_GET_SIGNATURE(0x09DB4923, 0xCCB6DDB5), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmutlcwstrcpy
	#define EXT_cmutlcwstrcpy
	#define GET_cmutlcwstrcpy(fl)  CAL_CMGETAPI( "cmutlcwstrcpy" ) 
	#define CAL_cmutlcwstrcpy  cmutlcwstrcpy
	#define CHK_cmutlcwstrcpy  TRUE
	#define EXP_cmutlcwstrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlcwstrcpy", (RTS_UINTPTR)cmutlcwstrcpy, 1, RTSITF_GET_SIGNATURE(0x09DB4923, 0xCCB6DDB5), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmutlcwstrcpy  PFCMUTLCWSTRCPY_IEC pfcmutlcwstrcpy;
	#define EXT_cmutlcwstrcpy  extern PFCMUTLCWSTRCPY_IEC pfcmutlcwstrcpy;
	#define GET_cmutlcwstrcpy(fl)  s_pfCMGetAPI2( "cmutlcwstrcpy", (RTS_VOID_FCTPTR *)&pfcmutlcwstrcpy, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x09DB4923, 0xCCB6DDB5), 0x03050D00)
	#define CAL_cmutlcwstrcpy  pfcmutlcwstrcpy
	#define CHK_cmutlcwstrcpy  (pfcmutlcwstrcpy != NULL)
	#define EXP_cmutlcwstrcpy   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlcwstrcpy", (RTS_UINTPTR)cmutlcwstrcpy, 1, RTSITF_GET_SIGNATURE(0x09DB4923, 0xCCB6DDB5), 0x03050D00) 
#endif


/**
 * <description>cmutlsafestrcpy</description>
 */
typedef struct tagcmutlsafestrcpy_struct
{
	RTS_IEC_STRING *pszDest;			/* VAR_INPUT */	
	RTS_IEC_XINT nDestSize;				/* VAR_INPUT */	
	RTS_IEC_STRING *pszSrc;				/* VAR_INPUT */	
	RTS_IEC_RESULT CMUtlSafeStrCpy;		/* VAR_OUTPUT */	
} cmutlsafestrcpy_struct;

void CDECL CDECL_EXT cmutlsafestrcpy(cmutlsafestrcpy_struct *p);
typedef void (CDECL CDECL_EXT* PFCMUTLSAFESTRCPY_IEC) (cmutlsafestrcpy_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUTLSAFESTRCPY_NOTIMPLEMENTED)
	#define USE_cmutlsafestrcpy
	#define EXT_cmutlsafestrcpy
	#define GET_cmutlsafestrcpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmutlsafestrcpy(p0) 
	#define CHK_cmutlsafestrcpy  FALSE
	#define EXP_cmutlsafestrcpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmutlsafestrcpy
	#define EXT_cmutlsafestrcpy
	#define GET_cmutlsafestrcpy(fl)  CAL_CMGETAPI( "cmutlsafestrcpy" ) 
	#define CAL_cmutlsafestrcpy  cmutlsafestrcpy
	#define CHK_cmutlsafestrcpy  TRUE
	#define EXP_cmutlsafestrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlsafestrcpy", (RTS_UINTPTR)cmutlsafestrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x794C4461), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmutlsafestrcpy
	#define EXT_cmutlsafestrcpy
	#define GET_cmutlsafestrcpy(fl)  CAL_CMGETAPI( "cmutlsafestrcpy" ) 
	#define CAL_cmutlsafestrcpy  cmutlsafestrcpy
	#define CHK_cmutlsafestrcpy  TRUE
	#define EXP_cmutlsafestrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlsafestrcpy", (RTS_UINTPTR)cmutlsafestrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x794C4461), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmutlsafestrcpy
	#define EXT_CMcmutlsafestrcpy
	#define GET_CMcmutlsafestrcpy  ERR_OK
	#define CAL_CMcmutlsafestrcpy  cmutlsafestrcpy
	#define CHK_CMcmutlsafestrcpy  TRUE
	#define EXP_CMcmutlsafestrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlsafestrcpy", (RTS_UINTPTR)cmutlsafestrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x794C4461), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmutlsafestrcpy
	#define EXT_cmutlsafestrcpy
	#define GET_cmutlsafestrcpy(fl)  CAL_CMGETAPI( "cmutlsafestrcpy" ) 
	#define CAL_cmutlsafestrcpy  cmutlsafestrcpy
	#define CHK_cmutlsafestrcpy  TRUE
	#define EXP_cmutlsafestrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlsafestrcpy", (RTS_UINTPTR)cmutlsafestrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x794C4461), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmutlsafestrcpy  PFCMUTLSAFESTRCPY_IEC pfcmutlsafestrcpy;
	#define EXT_cmutlsafestrcpy  extern PFCMUTLSAFESTRCPY_IEC pfcmutlsafestrcpy;
	#define GET_cmutlsafestrcpy(fl)  s_pfCMGetAPI2( "cmutlsafestrcpy", (RTS_VOID_FCTPTR *)&pfcmutlsafestrcpy, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x794C4461), 0x03050D00)
	#define CAL_cmutlsafestrcpy  pfcmutlsafestrcpy
	#define CHK_cmutlsafestrcpy  (pfcmutlsafestrcpy != NULL)
	#define EXP_cmutlsafestrcpy   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlsafestrcpy", (RTS_UINTPTR)cmutlsafestrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x794C4461), 0x03050D00) 
#endif


/**
 * <description>cmutlstricmp</description>
 */
typedef struct tagcmutlstricmp_struct
{
	RTS_IEC_STRING *pszString1;			/* VAR_INPUT */	
	RTS_IEC_STRING *pszString2;			/* VAR_INPUT */	
	RTS_IEC_DINT CMUtlStrICmp;			/* VAR_OUTPUT */	
} cmutlstricmp_struct;

void CDECL CDECL_EXT cmutlstricmp(cmutlstricmp_struct *p);
typedef void (CDECL CDECL_EXT* PFCMUTLSTRICMP_IEC) (cmutlstricmp_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUTLSTRICMP_NOTIMPLEMENTED)
	#define USE_cmutlstricmp
	#define EXT_cmutlstricmp
	#define GET_cmutlstricmp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmutlstricmp(p0) 
	#define CHK_cmutlstricmp  FALSE
	#define EXP_cmutlstricmp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmutlstricmp
	#define EXT_cmutlstricmp
	#define GET_cmutlstricmp(fl)  CAL_CMGETAPI( "cmutlstricmp" ) 
	#define CAL_cmutlstricmp  cmutlstricmp
	#define CHK_cmutlstricmp  TRUE
	#define EXP_cmutlstricmp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlstricmp", (RTS_UINTPTR)cmutlstricmp, 1, RTSITF_GET_SIGNATURE(0, 0xF3161529), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmutlstricmp
	#define EXT_cmutlstricmp
	#define GET_cmutlstricmp(fl)  CAL_CMGETAPI( "cmutlstricmp" ) 
	#define CAL_cmutlstricmp  cmutlstricmp
	#define CHK_cmutlstricmp  TRUE
	#define EXP_cmutlstricmp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlstricmp", (RTS_UINTPTR)cmutlstricmp, 1, RTSITF_GET_SIGNATURE(0, 0xF3161529), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmutlstricmp
	#define EXT_CMcmutlstricmp
	#define GET_CMcmutlstricmp  ERR_OK
	#define CAL_CMcmutlstricmp  cmutlstricmp
	#define CHK_CMcmutlstricmp  TRUE
	#define EXP_CMcmutlstricmp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlstricmp", (RTS_UINTPTR)cmutlstricmp, 1, RTSITF_GET_SIGNATURE(0, 0xF3161529), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmutlstricmp
	#define EXT_cmutlstricmp
	#define GET_cmutlstricmp(fl)  CAL_CMGETAPI( "cmutlstricmp" ) 
	#define CAL_cmutlstricmp  cmutlstricmp
	#define CHK_cmutlstricmp  TRUE
	#define EXP_cmutlstricmp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlstricmp", (RTS_UINTPTR)cmutlstricmp, 1, RTSITF_GET_SIGNATURE(0, 0xF3161529), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmutlstricmp  PFCMUTLSTRICMP_IEC pfcmutlstricmp;
	#define EXT_cmutlstricmp  extern PFCMUTLSTRICMP_IEC pfcmutlstricmp;
	#define GET_cmutlstricmp(fl)  s_pfCMGetAPI2( "cmutlstricmp", (RTS_VOID_FCTPTR *)&pfcmutlstricmp, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xF3161529), 0x03050D00)
	#define CAL_cmutlstricmp  pfcmutlstricmp
	#define CHK_cmutlstricmp  (pfcmutlstricmp != NULL)
	#define EXP_cmutlstricmp   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlstricmp", (RTS_UINTPTR)cmutlstricmp, 1, RTSITF_GET_SIGNATURE(0, 0xF3161529), 0x03050D00) 
#endif


/**
 * <description>cmutlutf8tow</description>
 */
typedef struct tagcmutlutf8tow_struct
{
	RTS_IEC_BYTE *pUtf8Str;				/* VAR_INPUT */	
	RTS_IEC_UXINT utf8BufferSize;		/* VAR_INPUT */	
	RTS_IEC_WSTRING *pwsz;				/* VAR_INPUT */	
	RTS_IEC_UXINT wstrLen;				/* VAR_INPUT */	
	RTS_IEC_RESULT CMUtlUtf8ToW;		/* VAR_OUTPUT */	
} cmutlutf8tow_struct;

void CDECL CDECL_EXT cmutlutf8tow(cmutlutf8tow_struct *p);
typedef void (CDECL CDECL_EXT* PFCMUTLUTF8TOW_IEC) (cmutlutf8tow_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUTLUTF8TOW_NOTIMPLEMENTED)
	#define USE_cmutlutf8tow
	#define EXT_cmutlutf8tow
	#define GET_cmutlutf8tow(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmutlutf8tow(p0) 
	#define CHK_cmutlutf8tow  FALSE
	#define EXP_cmutlutf8tow  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmutlutf8tow
	#define EXT_cmutlutf8tow
	#define GET_cmutlutf8tow(fl)  CAL_CMGETAPI( "cmutlutf8tow" ) 
	#define CAL_cmutlutf8tow  cmutlutf8tow
	#define CHK_cmutlutf8tow  TRUE
	#define EXP_cmutlutf8tow  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlutf8tow", (RTS_UINTPTR)cmutlutf8tow, 1, RTSITF_GET_SIGNATURE(0xB78A45E0, 0x76EB6470), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmutlutf8tow
	#define EXT_cmutlutf8tow
	#define GET_cmutlutf8tow(fl)  CAL_CMGETAPI( "cmutlutf8tow" ) 
	#define CAL_cmutlutf8tow  cmutlutf8tow
	#define CHK_cmutlutf8tow  TRUE
	#define EXP_cmutlutf8tow  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlutf8tow", (RTS_UINTPTR)cmutlutf8tow, 1, RTSITF_GET_SIGNATURE(0xB78A45E0, 0x76EB6470), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmutlutf8tow
	#define EXT_CMcmutlutf8tow
	#define GET_CMcmutlutf8tow  ERR_OK
	#define CAL_CMcmutlutf8tow  cmutlutf8tow
	#define CHK_CMcmutlutf8tow  TRUE
	#define EXP_CMcmutlutf8tow  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlutf8tow", (RTS_UINTPTR)cmutlutf8tow, 1, RTSITF_GET_SIGNATURE(0xB78A45E0, 0x76EB6470), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmutlutf8tow
	#define EXT_cmutlutf8tow
	#define GET_cmutlutf8tow(fl)  CAL_CMGETAPI( "cmutlutf8tow" ) 
	#define CAL_cmutlutf8tow  cmutlutf8tow
	#define CHK_cmutlutf8tow  TRUE
	#define EXP_cmutlutf8tow  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlutf8tow", (RTS_UINTPTR)cmutlutf8tow, 1, RTSITF_GET_SIGNATURE(0xB78A45E0, 0x76EB6470), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmutlutf8tow  PFCMUTLUTF8TOW_IEC pfcmutlutf8tow;
	#define EXT_cmutlutf8tow  extern PFCMUTLUTF8TOW_IEC pfcmutlutf8tow;
	#define GET_cmutlutf8tow(fl)  s_pfCMGetAPI2( "cmutlutf8tow", (RTS_VOID_FCTPTR *)&pfcmutlutf8tow, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xB78A45E0, 0x76EB6470), 0x03050D00)
	#define CAL_cmutlutf8tow  pfcmutlutf8tow
	#define CHK_cmutlutf8tow  (pfcmutlutf8tow != NULL)
	#define EXP_cmutlutf8tow   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlutf8tow", (RTS_UINTPTR)cmutlutf8tow, 1, RTSITF_GET_SIGNATURE(0xB78A45E0, 0x76EB6470), 0x03050D00) 
#endif


/**
 * <description>cmutlwstrcpy</description>
 */
typedef struct tagcmutlwstrcpy_struct
{
	RTS_IEC_WSTRING *pwszDest;			/* VAR_INPUT */	
	RTS_IEC_XINT nDestSize;				/* VAR_INPUT */	
	RTS_IEC_WSTRING *pwszSrc;			/* VAR_INPUT */	
	RTS_IEC_RESULT CMUtlwstrcpy;		/* VAR_OUTPUT */	
} cmutlwstrcpy_struct;

void CDECL CDECL_EXT cmutlwstrcpy(cmutlwstrcpy_struct *p);
typedef void (CDECL CDECL_EXT* PFCMUTLWSTRCPY_IEC) (cmutlwstrcpy_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUTLWSTRCPY_NOTIMPLEMENTED)
	#define USE_cmutlwstrcpy
	#define EXT_cmutlwstrcpy
	#define GET_cmutlwstrcpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmutlwstrcpy(p0) 
	#define CHK_cmutlwstrcpy  FALSE
	#define EXP_cmutlwstrcpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmutlwstrcpy
	#define EXT_cmutlwstrcpy
	#define GET_cmutlwstrcpy(fl)  CAL_CMGETAPI( "cmutlwstrcpy" ) 
	#define CAL_cmutlwstrcpy  cmutlwstrcpy
	#define CHK_cmutlwstrcpy  TRUE
	#define EXP_cmutlwstrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwstrcpy", (RTS_UINTPTR)cmutlwstrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x92BF32B1), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmutlwstrcpy
	#define EXT_cmutlwstrcpy
	#define GET_cmutlwstrcpy(fl)  CAL_CMGETAPI( "cmutlwstrcpy" ) 
	#define CAL_cmutlwstrcpy  cmutlwstrcpy
	#define CHK_cmutlwstrcpy  TRUE
	#define EXP_cmutlwstrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwstrcpy", (RTS_UINTPTR)cmutlwstrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x92BF32B1), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmutlwstrcpy
	#define EXT_CMcmutlwstrcpy
	#define GET_CMcmutlwstrcpy  ERR_OK
	#define CAL_CMcmutlwstrcpy  cmutlwstrcpy
	#define CHK_CMcmutlwstrcpy  TRUE
	#define EXP_CMcmutlwstrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwstrcpy", (RTS_UINTPTR)cmutlwstrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x92BF32B1), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmutlwstrcpy
	#define EXT_cmutlwstrcpy
	#define GET_cmutlwstrcpy(fl)  CAL_CMGETAPI( "cmutlwstrcpy" ) 
	#define CAL_cmutlwstrcpy  cmutlwstrcpy
	#define CHK_cmutlwstrcpy  TRUE
	#define EXP_cmutlwstrcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwstrcpy", (RTS_UINTPTR)cmutlwstrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x92BF32B1), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmutlwstrcpy  PFCMUTLWSTRCPY_IEC pfcmutlwstrcpy;
	#define EXT_cmutlwstrcpy  extern PFCMUTLWSTRCPY_IEC pfcmutlwstrcpy;
	#define GET_cmutlwstrcpy(fl)  s_pfCMGetAPI2( "cmutlwstrcpy", (RTS_VOID_FCTPTR *)&pfcmutlwstrcpy, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x92BF32B1), 0x03050D00)
	#define CAL_cmutlwstrcpy  pfcmutlwstrcpy
	#define CHK_cmutlwstrcpy  (pfcmutlwstrcpy != NULL)
	#define EXP_cmutlwstrcpy   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwstrcpy", (RTS_UINTPTR)cmutlwstrcpy, 1, RTSITF_GET_SIGNATURE(0, 0x92BF32B1), 0x03050D00) 
#endif


/**
 * <description>cmutlwtoutf8</description>
 */
typedef struct tagcmutlwtoutf8_struct
{
	RTS_IEC_WSTRING *pwsz;				/* VAR_INPUT */	
	RTS_IEC_UXINT wstrLen;				/* VAR_INPUT */	
	RTS_IEC_BYTE *pUtf8Str;				/* VAR_INPUT */	
	RTS_IEC_UXINT utf8BufferSize;		/* VAR_INPUT */	
	RTS_IEC_RESULT CMUtlWToUtf8;		/* VAR_OUTPUT */	
} cmutlwtoutf8_struct;

void CDECL CDECL_EXT cmutlwtoutf8(cmutlwtoutf8_struct *p);
typedef void (CDECL CDECL_EXT* PFCMUTLWTOUTF8_IEC) (cmutlwtoutf8_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUTLWTOUTF8_NOTIMPLEMENTED)
	#define USE_cmutlwtoutf8
	#define EXT_cmutlwtoutf8
	#define GET_cmutlwtoutf8(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmutlwtoutf8(p0) 
	#define CHK_cmutlwtoutf8  FALSE
	#define EXP_cmutlwtoutf8  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmutlwtoutf8
	#define EXT_cmutlwtoutf8
	#define GET_cmutlwtoutf8(fl)  CAL_CMGETAPI( "cmutlwtoutf8" ) 
	#define CAL_cmutlwtoutf8  cmutlwtoutf8
	#define CHK_cmutlwtoutf8  TRUE
	#define EXP_cmutlwtoutf8  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwtoutf8", (RTS_UINTPTR)cmutlwtoutf8, 1, RTSITF_GET_SIGNATURE(0xAE6E95C8, 0x61F1F05D), 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmutlwtoutf8
	#define EXT_cmutlwtoutf8
	#define GET_cmutlwtoutf8(fl)  CAL_CMGETAPI( "cmutlwtoutf8" ) 
	#define CAL_cmutlwtoutf8  cmutlwtoutf8
	#define CHK_cmutlwtoutf8  TRUE
	#define EXP_cmutlwtoutf8  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwtoutf8", (RTS_UINTPTR)cmutlwtoutf8, 1, RTSITF_GET_SIGNATURE(0xAE6E95C8, 0x61F1F05D), 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmutlwtoutf8
	#define EXT_CMcmutlwtoutf8
	#define GET_CMcmutlwtoutf8  ERR_OK
	#define CAL_CMcmutlwtoutf8  cmutlwtoutf8
	#define CHK_CMcmutlwtoutf8  TRUE
	#define EXP_CMcmutlwtoutf8  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwtoutf8", (RTS_UINTPTR)cmutlwtoutf8, 1, RTSITF_GET_SIGNATURE(0xAE6E95C8, 0x61F1F05D), 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_cmutlwtoutf8
	#define EXT_cmutlwtoutf8
	#define GET_cmutlwtoutf8(fl)  CAL_CMGETAPI( "cmutlwtoutf8" ) 
	#define CAL_cmutlwtoutf8  cmutlwtoutf8
	#define CHK_cmutlwtoutf8  TRUE
	#define EXP_cmutlwtoutf8  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwtoutf8", (RTS_UINTPTR)cmutlwtoutf8, 1, RTSITF_GET_SIGNATURE(0xAE6E95C8, 0x61F1F05D), 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_cmutlwtoutf8  PFCMUTLWTOUTF8_IEC pfcmutlwtoutf8;
	#define EXT_cmutlwtoutf8  extern PFCMUTLWTOUTF8_IEC pfcmutlwtoutf8;
	#define GET_cmutlwtoutf8(fl)  s_pfCMGetAPI2( "cmutlwtoutf8", (RTS_VOID_FCTPTR *)&pfcmutlwtoutf8, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xAE6E95C8, 0x61F1F05D), 0x03050D00)
	#define CAL_cmutlwtoutf8  pfcmutlwtoutf8
	#define CHK_cmutlwtoutf8  (pfcmutlwtoutf8 != NULL)
	#define EXP_cmutlwtoutf8   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cmutlwtoutf8", (RTS_UINTPTR)cmutlwtoutf8, 1, RTSITF_GET_SIGNATURE(0xAE6E95C8, 0x61F1F05D), 0x03050D00) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


typedef struct
{
	RTS_IEC_STRING *pszComponent;
	RTS_IEC_RESULT *pResult;
	RTS_IEC_HANDLE hComponent;
} cmloadcomponent_struct;

typedef struct
{
	RTS_IEC_HANDLE hComponent;
	RTS_IEC_RESULT Result;
} cmunloadcomponent_struct;

typedef struct
{
	RTS_IEC_UDINT udiLicenseID;
	RTS_IEC_RESULT *pResult;
	RTS_IEC_RESULT GetUserLicenseValue;
} getuserlicensevalue_struct;

typedef struct
{
	RTS_IEC_UDINT udiLicenseID;
	RTS_IEC_UDINT udiNewLicenseValue;
	RTS_IEC_RESULT *pResult;
	RTS_IEC_UDINT ReqDynLicChallenge;
} reqdynlicchallenge_struct;

typedef struct
{
	RTS_IEC_UDINT udiLicenseID;
	RTS_IEC_UDINT udiNewLicenseValue;
	RTS_IEC_UDINT udiChallenge;
	RTS_IEC_RESULT *pResult;
	RTS_IEC_BOOL ConfDynLicChallenge;
} confdynlicchallenge_struct;

typedef void (CDECL *PFGETUSERLICENSEVALUE)(getuserlicensevalue_struct);
typedef void (CDECL *PFREQDYNLICCHALLENGE)(reqdynlicchallenge_struct);
typedef void (CDECL *PFCONFDYNLICCHALLENGE)(confdynlicchallenge_struct);


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description> 
 *   Called to initialize the component manager
 * </description>
 * <param name="pszSettingsFile" type="IN">Pointer to name of the configuration file</param>
 * <param name="pStaticComponents" type="IN">Pointer to list of components with name and entry routine to initialize without configuration</param>
 * <result>Error code:
 * <ul>
 *		<li>ERR_OK</li>
 *		<li>ERR_FAILED: One or more components failed to load</li>
 *		<li>ERR_ID_MISMATCH: Signature mismatch of the SysTargetIDs</li>
 * </ul>
 * </result>
 */
RTS_RESULT CDECL CMInit(char *pszSettingsFile, StaticComponent *pStaticComponents);
typedef RTS_RESULT (CDECL * PFCMINIT) (char *pszSettingsFile, StaticComponent *pStaticComponents);
#if defined(CM_NOTIMPLEMENTED) || defined(CMINIT_NOTIMPLEMENTED)
	#define USE_CMInit
	#define EXT_CMInit
	#define GET_CMInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMInit(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMInit  FALSE
	#define EXP_CMInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMInit
	#define EXT_CMInit
	#define GET_CMInit(fl)  CAL_CMGETAPI( "CMInit" ) 
	#define CAL_CMInit  CMInit
	#define CHK_CMInit  TRUE
	#define EXP_CMInit  CAL_CMEXPAPI( "CMInit" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMInit
	#define EXT_CMInit
	#define GET_CMInit(fl)  CAL_CMGETAPI( "CMInit" ) 
	#define CAL_CMInit  CMInit
	#define CHK_CMInit  TRUE
	#define EXP_CMInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInit", (RTS_UINTPTR)CMInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMInit
	#define EXT_CMCMInit
	#define GET_CMCMInit  ERR_OK
	#define CAL_CMCMInit pICM->ICMInit
	#define CHK_CMCMInit (pICM != NULL)
	#define EXP_CMCMInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMInit
	#define EXT_CMInit
	#define GET_CMInit(fl)  CAL_CMGETAPI( "CMInit" ) 
	#define CAL_CMInit pICM->ICMInit
	#define CHK_CMInit (pICM != NULL)
	#define EXP_CMInit  CAL_CMEXPAPI( "CMInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMInit  PFCMINIT pfCMInit;
	#define EXT_CMInit  extern PFCMINIT pfCMInit;
	#define GET_CMInit(fl)  s_pfCMGetAPI2( "CMInit", (RTS_VOID_FCTPTR *)&pfCMInit, (fl), 0, 0)
	#define CAL_CMInit  pfCMInit
	#define CHK_CMInit  (pfCMInit != NULL)
	#define EXP_CMInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInit", (RTS_UINTPTR)CMInit, 0, 0) 
#endif




/**
 * <description> 
 *   Called to initialize the component manager
 * </description>
 * <param name="pszSettingsFile" type="IN">Pointer to name of the configuration file</param>
 * <param name="pStaticComponents" type="IN">Pointer to list of components with name and entry routine to initialize without configuration</param>
 * <param name="bSettingsFileIsOptional" type="IN">Specifies, if the Settingsfile is only used optionally</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMInit2(char *pszSettingsFile, StaticComponent *pStaticComponents, int bSettingsFileIsOptional);
typedef RTS_RESULT (CDECL * PFCMINIT2) (char *pszSettingsFile, StaticComponent *pStaticComponents, int bSettingsFileIsOptional);
#if defined(CM_NOTIMPLEMENTED) || defined(CMINIT2_NOTIMPLEMENTED)
	#define USE_CMInit2
	#define EXT_CMInit2
	#define GET_CMInit2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMInit2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMInit2  FALSE
	#define EXP_CMInit2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMInit2
	#define EXT_CMInit2
	#define GET_CMInit2(fl)  CAL_CMGETAPI( "CMInit2" ) 
	#define CAL_CMInit2  CMInit2
	#define CHK_CMInit2  TRUE
	#define EXP_CMInit2  CAL_CMEXPAPI( "CMInit2" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMInit2
	#define EXT_CMInit2
	#define GET_CMInit2(fl)  CAL_CMGETAPI( "CMInit2" ) 
	#define CAL_CMInit2  CMInit2
	#define CHK_CMInit2  TRUE
	#define EXP_CMInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInit2", (RTS_UINTPTR)CMInit2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMInit2
	#define EXT_CMCMInit2
	#define GET_CMCMInit2  ERR_OK
	#define CAL_CMCMInit2 pICM->ICMInit2
	#define CHK_CMCMInit2 (pICM != NULL)
	#define EXP_CMCMInit2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMInit2
	#define EXT_CMInit2
	#define GET_CMInit2(fl)  CAL_CMGETAPI( "CMInit2" ) 
	#define CAL_CMInit2 pICM->ICMInit2
	#define CHK_CMInit2 (pICM != NULL)
	#define EXP_CMInit2  CAL_CMEXPAPI( "CMInit2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMInit2  PFCMINIT2 pfCMInit2;
	#define EXT_CMInit2  extern PFCMINIT2 pfCMInit2;
	#define GET_CMInit2(fl)  s_pfCMGetAPI2( "CMInit2", (RTS_VOID_FCTPTR *)&pfCMInit2, (fl), 0, 0)
	#define CAL_CMInit2  pfCMInit2
	#define CHK_CMInit2  (pfCMInit2 != NULL)
	#define EXP_CMInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInit2", (RTS_UINTPTR)CMInit2, 0, 0) 
#endif




/**
 * <description> 
 *   Called to initialize the component manager
 * </description>
 * <param name="pszSettingsFile" type="IN">Pointer to name of the configuration file</param>
 * <param name="pStaticComponents" type="IN">Pointer to list of components with name and entry routine to initialize without configuration</param>
 * <param name="options" type="IN">Options for the init sequence of the component manager. See category "CMInit options" for details.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMInit3(char *pszSettingsFile, StaticComponent *pStaticComponents, RTS_UI32 options);
typedef RTS_RESULT (CDECL * PFCMINIT3) (char *pszSettingsFile, StaticComponent *pStaticComponents, RTS_UI32 options);
#if defined(CM_NOTIMPLEMENTED) || defined(CMINIT3_NOTIMPLEMENTED)
	#define USE_CMInit3
	#define EXT_CMInit3
	#define GET_CMInit3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMInit3(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMInit3  FALSE
	#define EXP_CMInit3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMInit3
	#define EXT_CMInit3
	#define GET_CMInit3(fl)  CAL_CMGETAPI( "CMInit3" ) 
	#define CAL_CMInit3  CMInit3
	#define CHK_CMInit3  TRUE
	#define EXP_CMInit3  CAL_CMEXPAPI( "CMInit3" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMInit3
	#define EXT_CMInit3
	#define GET_CMInit3(fl)  CAL_CMGETAPI( "CMInit3" ) 
	#define CAL_CMInit3  CMInit3
	#define CHK_CMInit3  TRUE
	#define EXP_CMInit3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInit3", (RTS_UINTPTR)CMInit3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMInit3
	#define EXT_CMCMInit3
	#define GET_CMCMInit3  ERR_OK
	#define CAL_CMCMInit3 pICM->ICMInit3
	#define CHK_CMCMInit3 (pICM != NULL)
	#define EXP_CMCMInit3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMInit3
	#define EXT_CMInit3
	#define GET_CMInit3(fl)  CAL_CMGETAPI( "CMInit3" ) 
	#define CAL_CMInit3 pICM->ICMInit3
	#define CHK_CMInit3 (pICM != NULL)
	#define EXP_CMInit3  CAL_CMEXPAPI( "CMInit3" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMInit3  PFCMINIT3 pfCMInit3;
	#define EXT_CMInit3  extern PFCMINIT3 pfCMInit3;
	#define GET_CMInit3(fl)  s_pfCMGetAPI2( "CMInit3", (RTS_VOID_FCTPTR *)&pfCMInit3, (fl), 0, 0)
	#define CAL_CMInit3  pfCMInit3
	#define CHK_CMInit3  (pfCMInit3 != NULL)
	#define EXP_CMInit3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInit3", (RTS_UINTPTR)CMInit3, 0, 0) 
#endif




/**
 * <description> 
 *   Called to deinitialize the component manager
 * </description>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMExit(void);
typedef RTS_RESULT (CDECL * PFCMEXIT) (void);
#if defined(CM_NOTIMPLEMENTED) || defined(CMEXIT_NOTIMPLEMENTED)
	#define USE_CMExit
	#define EXT_CMExit
	#define GET_CMExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMExit()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMExit  FALSE
	#define EXP_CMExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMExit
	#define EXT_CMExit
	#define GET_CMExit(fl)  CAL_CMGETAPI( "CMExit" ) 
	#define CAL_CMExit  CMExit
	#define CHK_CMExit  TRUE
	#define EXP_CMExit  CAL_CMEXPAPI( "CMExit" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMExit
	#define EXT_CMExit
	#define GET_CMExit(fl)  CAL_CMGETAPI( "CMExit" ) 
	#define CAL_CMExit  CMExit
	#define CHK_CMExit  TRUE
	#define EXP_CMExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMExit", (RTS_UINTPTR)CMExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMExit
	#define EXT_CMCMExit
	#define GET_CMCMExit  ERR_OK
	#define CAL_CMCMExit pICM->ICMExit
	#define CHK_CMCMExit (pICM != NULL)
	#define EXP_CMCMExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMExit
	#define EXT_CMExit
	#define GET_CMExit(fl)  CAL_CMGETAPI( "CMExit" ) 
	#define CAL_CMExit pICM->ICMExit
	#define CHK_CMExit (pICM != NULL)
	#define EXP_CMExit  CAL_CMEXPAPI( "CMExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMExit  PFCMEXIT pfCMExit;
	#define EXT_CMExit  extern PFCMEXIT pfCMExit;
	#define GET_CMExit(fl)  s_pfCMGetAPI2( "CMExit", (RTS_VOID_FCTPTR *)&pfCMExit, (fl), 0, 0)
	#define CAL_CMExit  pfCMExit
	#define CHK_CMExit  (pfCMExit != NULL)
	#define EXP_CMExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMExit", (RTS_UINTPTR)CMExit, 0, 0) 
#endif




/**
 * <description>Set a flag for main loop to exit</description>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMSetExit(void);
typedef RTS_RESULT (CDECL * PFCMSETEXIT) (void);
#if defined(CM_NOTIMPLEMENTED) || defined(CMSETEXIT_NOTIMPLEMENTED)
	#define USE_CMSetExit
	#define EXT_CMSetExit
	#define GET_CMSetExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMSetExit()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMSetExit  FALSE
	#define EXP_CMSetExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMSetExit
	#define EXT_CMSetExit
	#define GET_CMSetExit(fl)  CAL_CMGETAPI( "CMSetExit" ) 
	#define CAL_CMSetExit  CMSetExit
	#define CHK_CMSetExit  TRUE
	#define EXP_CMSetExit  CAL_CMEXPAPI( "CMSetExit" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMSetExit
	#define EXT_CMSetExit
	#define GET_CMSetExit(fl)  CAL_CMGETAPI( "CMSetExit" ) 
	#define CAL_CMSetExit  CMSetExit
	#define CHK_CMSetExit  TRUE
	#define EXP_CMSetExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMSetExit", (RTS_UINTPTR)CMSetExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMSetExit
	#define EXT_CMCMSetExit
	#define GET_CMCMSetExit  ERR_OK
	#define CAL_CMCMSetExit pICM->ICMSetExit
	#define CHK_CMCMSetExit (pICM != NULL)
	#define EXP_CMCMSetExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMSetExit
	#define EXT_CMSetExit
	#define GET_CMSetExit(fl)  CAL_CMGETAPI( "CMSetExit" ) 
	#define CAL_CMSetExit pICM->ICMSetExit
	#define CHK_CMSetExit (pICM != NULL)
	#define EXP_CMSetExit  CAL_CMEXPAPI( "CMSetExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMSetExit  PFCMSETEXIT pfCMSetExit;
	#define EXT_CMSetExit  extern PFCMSETEXIT pfCMSetExit;
	#define GET_CMSetExit(fl)  s_pfCMGetAPI2( "CMSetExit", (RTS_VOID_FCTPTR *)&pfCMSetExit, (fl), 0, 0)
	#define CAL_CMSetExit  pfCMSetExit
	#define CHK_CMSetExit  (pfCMSetExit != NULL)
	#define EXP_CMSetExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMSetExit", (RTS_UINTPTR)CMSetExit, 0, 0) 
#endif




/**
 * <description>Get a flag to exit main loop</description>
 * <result>ERR_OK, if exit flag is set
 * ERR_FAILED else</result>
 */
RTS_RESULT CDECL CMGetExit(void);
typedef RTS_RESULT (CDECL * PFCMGETEXIT) (void);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETEXIT_NOTIMPLEMENTED)
	#define USE_CMGetExit
	#define EXT_CMGetExit
	#define GET_CMGetExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetExit()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMGetExit  FALSE
	#define EXP_CMGetExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetExit
	#define EXT_CMGetExit
	#define GET_CMGetExit(fl)  CAL_CMGETAPI( "CMGetExit" ) 
	#define CAL_CMGetExit  CMGetExit
	#define CHK_CMGetExit  TRUE
	#define EXP_CMGetExit  CAL_CMEXPAPI( "CMGetExit" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetExit
	#define EXT_CMGetExit
	#define GET_CMGetExit(fl)  CAL_CMGETAPI( "CMGetExit" ) 
	#define CAL_CMGetExit  CMGetExit
	#define CHK_CMGetExit  TRUE
	#define EXP_CMGetExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetExit", (RTS_UINTPTR)CMGetExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetExit
	#define EXT_CMCMGetExit
	#define GET_CMCMGetExit  ERR_OK
	#define CAL_CMCMGetExit pICM->ICMGetExit
	#define CHK_CMCMGetExit (pICM != NULL)
	#define EXP_CMCMGetExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetExit
	#define EXT_CMGetExit
	#define GET_CMGetExit(fl)  CAL_CMGETAPI( "CMGetExit" ) 
	#define CAL_CMGetExit pICM->ICMGetExit
	#define CHK_CMGetExit (pICM != NULL)
	#define EXP_CMGetExit  CAL_CMEXPAPI( "CMGetExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetExit  PFCMGETEXIT pfCMGetExit;
	#define EXT_CMGetExit  extern PFCMGETEXIT pfCMGetExit;
	#define GET_CMGetExit(fl)  s_pfCMGetAPI2( "CMGetExit", (RTS_VOID_FCTPTR *)&pfCMGetExit, (fl), 0, 0)
	#define CAL_CMGetExit  pfCMGetExit
	#define CHK_CMGetExit  (pfCMGetExit != NULL)
	#define EXP_CMGetExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetExit", (RTS_UINTPTR)CMGetExit, 0, 0) 
#endif




#ifdef RTS_DEBUG
	/* Macro can be used to check, if a single code line is reached. Result is printed in the console. */
	#define RTS_CHECKPOINT		if (CHK_CMDebugOut)CAL_CMDebugOut("===> %s checkpoint at line %d\n", __FILE__, __LINE__)
#else
	#define RTS_CHECKPOINT
#endif

/**
 * <description>Can be used for debug outputs on a console.</description>
 * <result>ERR_OK or ERR_NOT_SUPPORTED, if SysOut component is not available</result>
 */
RTS_RESULT CDECL CMDebugOut(const char *szFormat, ...);
typedef RTS_RESULT (CDECL * PFCMDEBUGOUT) (const char *szFormat, ...);
#if defined(CM_NOTIMPLEMENTED) || defined(CMDEBUGOUT_NOTIMPLEMENTED)
	#define USE_CMDebugOut
	#define EXT_CMDebugOut
	#define GET_CMDebugOut(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMDebugOut  FUNCTION_NOTIMPLEMENTED2 
	#define CHK_CMDebugOut  FALSE
	#define EXP_CMDebugOut  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMDebugOut
	#define EXT_CMDebugOut
	#define GET_CMDebugOut(fl)  CAL_CMGETAPI( "CMDebugOut" ) 
	#define CAL_CMDebugOut  CMDebugOut
	#define CHK_CMDebugOut  TRUE
	#define EXP_CMDebugOut  CAL_CMEXPAPI( "CMDebugOut" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMDebugOut
	#define EXT_CMDebugOut
	#define GET_CMDebugOut(fl)  CAL_CMGETAPI( "CMDebugOut" ) 
	#define CAL_CMDebugOut  CMDebugOut
	#define CHK_CMDebugOut  TRUE
	#define EXP_CMDebugOut  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMDebugOut", (RTS_UINTPTR)CMDebugOut, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMDebugOut
	#define EXT_CMCMDebugOut
	#define GET_CMCMDebugOut  ERR_OK
	#define CAL_CMCMDebugOut pICM->ICMDebugOut
	#define CHK_CMCMDebugOut (pICM != NULL)
	#define EXP_CMCMDebugOut  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMDebugOut
	#define EXT_CMDebugOut
	#define GET_CMDebugOut(fl)  CAL_CMGETAPI( "CMDebugOut" ) 
	#define CAL_CMDebugOut pICM->ICMDebugOut
	#define CHK_CMDebugOut (pICM != NULL)
	#define EXP_CMDebugOut  CAL_CMEXPAPI( "CMDebugOut" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMDebugOut  PFCMDEBUGOUT pfCMDebugOut;
	#define EXT_CMDebugOut  extern PFCMDEBUGOUT pfCMDebugOut;
	#define GET_CMDebugOut(fl)  s_pfCMGetAPI2( "CMDebugOut", (RTS_VOID_FCTPTR *)&pfCMDebugOut, (fl), 0, 0)
	#define CAL_CMDebugOut  pfCMDebugOut
	#define CHK_CMDebugOut  (pfCMDebugOut != NULL)
	#define EXP_CMDebugOut  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMDebugOut", (RTS_UINTPTR)CMDebugOut, 0, 0) 
#endif




/**
 * <description>Can be used for debug outputs on a console.</description>
 * <result>ERR_OK or ERR_NOT_SUPPORTED, if SysOut component is not available</result>
 */
RTS_RESULT CDECL CMDebugOutArg(const char *szFormat, va_list *pargList);
typedef RTS_RESULT (CDECL * PFCMDEBUGOUTARG) (const char *szFormat, va_list *pargList);
#if defined(CM_NOTIMPLEMENTED) || defined(CMDEBUGOUTARG_NOTIMPLEMENTED)
	#define USE_CMDebugOutArg
	#define EXT_CMDebugOutArg
	#define GET_CMDebugOutArg(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMDebugOutArg(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMDebugOutArg  FALSE
	#define EXP_CMDebugOutArg  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMDebugOutArg
	#define EXT_CMDebugOutArg
	#define GET_CMDebugOutArg(fl)  CAL_CMGETAPI( "CMDebugOutArg" ) 
	#define CAL_CMDebugOutArg  CMDebugOutArg
	#define CHK_CMDebugOutArg  TRUE
	#define EXP_CMDebugOutArg  CAL_CMEXPAPI( "CMDebugOutArg" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMDebugOutArg
	#define EXT_CMDebugOutArg
	#define GET_CMDebugOutArg(fl)  CAL_CMGETAPI( "CMDebugOutArg" ) 
	#define CAL_CMDebugOutArg  CMDebugOutArg
	#define CHK_CMDebugOutArg  TRUE
	#define EXP_CMDebugOutArg  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMDebugOutArg", (RTS_UINTPTR)CMDebugOutArg, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMDebugOutArg
	#define EXT_CMCMDebugOutArg
	#define GET_CMCMDebugOutArg  ERR_OK
	#define CAL_CMCMDebugOutArg pICM->ICMDebugOutArg
	#define CHK_CMCMDebugOutArg (pICM != NULL)
	#define EXP_CMCMDebugOutArg  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMDebugOutArg
	#define EXT_CMDebugOutArg
	#define GET_CMDebugOutArg(fl)  CAL_CMGETAPI( "CMDebugOutArg" ) 
	#define CAL_CMDebugOutArg pICM->ICMDebugOutArg
	#define CHK_CMDebugOutArg (pICM != NULL)
	#define EXP_CMDebugOutArg  CAL_CMEXPAPI( "CMDebugOutArg" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMDebugOutArg  PFCMDEBUGOUTARG pfCMDebugOutArg;
	#define EXT_CMDebugOutArg  extern PFCMDEBUGOUTARG pfCMDebugOutArg;
	#define GET_CMDebugOutArg(fl)  s_pfCMGetAPI2( "CMDebugOutArg", (RTS_VOID_FCTPTR *)&pfCMDebugOutArg, (fl), 0, 0)
	#define CAL_CMDebugOutArg  pfCMDebugOutArg
	#define CHK_CMDebugOutArg  (pfCMDebugOutArg != NULL)
	#define EXP_CMDebugOutArg  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMDebugOutArg", (RTS_UINTPTR)CMDebugOutArg, 0, 0) 
#endif




/**
 * <description> 
 *   Called to register a list of component API functions at the component manager
 * </description>
 * <param name="pExpTable" type="IN">Table of functions to be registered</param>
 * <param name="CmpId" type="IN">Component identifier</param>
 * <param name="bExternalLibrary" type="IN">Can be used as external library in the plc program</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMRegisterAPI(const CMP_EXT_FUNCTION_REF *pExpTable, RTS_UINTPTR dummy, int bExternalLibrary, RTS_UI32 cmpId);
typedef RTS_RESULT (CDECL * PFCMREGISTERAPI) (const CMP_EXT_FUNCTION_REF *pExpTable, RTS_UINTPTR dummy, int bExternalLibrary, RTS_UI32 cmpId);
#if defined(CM_NOTIMPLEMENTED) || defined(CMREGISTERAPI_NOTIMPLEMENTED)
	#define USE_CMRegisterAPI
	#define EXT_CMRegisterAPI
	#define GET_CMRegisterAPI(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMRegisterAPI(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMRegisterAPI  FALSE
	#define EXP_CMRegisterAPI  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMRegisterAPI
	#define EXT_CMRegisterAPI
	#define GET_CMRegisterAPI(fl)  CAL_CMGETAPI( "CMRegisterAPI" ) 
	#define CAL_CMRegisterAPI  CMRegisterAPI
	#define CHK_CMRegisterAPI  TRUE
	#define EXP_CMRegisterAPI  CAL_CMEXPAPI( "CMRegisterAPI" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMRegisterAPI
	#define EXT_CMRegisterAPI
	#define GET_CMRegisterAPI(fl)  CAL_CMGETAPI( "CMRegisterAPI" ) 
	#define CAL_CMRegisterAPI  CMRegisterAPI
	#define CHK_CMRegisterAPI  TRUE
	#define EXP_CMRegisterAPI  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMRegisterAPI", (RTS_UINTPTR)CMRegisterAPI, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMRegisterAPI
	#define EXT_CMCMRegisterAPI
	#define GET_CMCMRegisterAPI  ERR_OK
	#define CAL_CMCMRegisterAPI pICM->ICMRegisterAPI
	#define CHK_CMCMRegisterAPI (pICM != NULL)
	#define EXP_CMCMRegisterAPI  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMRegisterAPI
	#define EXT_CMRegisterAPI
	#define GET_CMRegisterAPI(fl)  CAL_CMGETAPI( "CMRegisterAPI" ) 
	#define CAL_CMRegisterAPI pICM->ICMRegisterAPI
	#define CHK_CMRegisterAPI (pICM != NULL)
	#define EXP_CMRegisterAPI  CAL_CMEXPAPI( "CMRegisterAPI" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMRegisterAPI  PFCMREGISTERAPI pfCMRegisterAPI;
	#define EXT_CMRegisterAPI  extern PFCMREGISTERAPI pfCMRegisterAPI;
	#define GET_CMRegisterAPI(fl)  s_pfCMGetAPI2( "CMRegisterAPI", (RTS_VOID_FCTPTR *)&pfCMRegisterAPI, (fl), 0, 0)
	#define CAL_CMRegisterAPI  pfCMRegisterAPI
	#define CHK_CMRegisterAPI  (pfCMRegisterAPI != NULL)
	#define EXP_CMRegisterAPI  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMRegisterAPI", (RTS_UINTPTR)CMRegisterAPI, 0, 0) 
#endif




/**
 * <description> 
 *   Called to register a component API function at the component manager
 * </description>
 * <param name="pszAPIName" type="IN">Name of the API routine</param>
 * <param name="pfAPIFunction" type="IN">Function pointer of the API routine</param>
 * <param name="bExternalLibrary" type="IN">Can be used as external library in the plc program</param>
 * <param name="ulSignatureID" type="IN">SignatureID of the function prototype</param>
 * <param name="ulVersion" type="IN">Actual supported implementation version</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMRegisterAPI2(const char *pszAPIName, RTS_VOID_FCTPTR pfAPIFunction, int bExternalLibrary, RTS_UI32 ulSignatureID, RTS_UI32 ulVersion);
typedef RTS_RESULT (CDECL * PFCMREGISTERAPI2) (const char *pszAPIName, RTS_VOID_FCTPTR pfAPIFunction, int bExternalLibrary, RTS_UI32 ulSignatureID, RTS_UI32 ulVersion);
#if defined(CM_NOTIMPLEMENTED) || defined(CMREGISTERAPI2_NOTIMPLEMENTED)
	#define USE_CMRegisterAPI2
	#define EXT_CMRegisterAPI2
	#define GET_CMRegisterAPI2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMRegisterAPI2(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMRegisterAPI2  FALSE
	#define EXP_CMRegisterAPI2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMRegisterAPI2
	#define EXT_CMRegisterAPI2
	#define GET_CMRegisterAPI2(fl)  CAL_CMGETAPI( "CMRegisterAPI2" ) 
	#define CAL_CMRegisterAPI2  CMRegisterAPI2
	#define CHK_CMRegisterAPI2  TRUE
	#define EXP_CMRegisterAPI2  CAL_CMEXPAPI( "CMRegisterAPI2" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMRegisterAPI2
	#define EXT_CMRegisterAPI2
	#define GET_CMRegisterAPI2(fl)  CAL_CMGETAPI( "CMRegisterAPI2" ) 
	#define CAL_CMRegisterAPI2  CMRegisterAPI2
	#define CHK_CMRegisterAPI2  TRUE
	#define EXP_CMRegisterAPI2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMRegisterAPI2", (RTS_UINTPTR)CMRegisterAPI2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMRegisterAPI2
	#define EXT_CMCMRegisterAPI2
	#define GET_CMCMRegisterAPI2  ERR_OK
	#define CAL_CMCMRegisterAPI2 pICM->ICMRegisterAPI2
	#define CHK_CMCMRegisterAPI2 (pICM != NULL)
	#define EXP_CMCMRegisterAPI2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMRegisterAPI2
	#define EXT_CMRegisterAPI2
	#define GET_CMRegisterAPI2(fl)  CAL_CMGETAPI( "CMRegisterAPI2" ) 
	#define CAL_CMRegisterAPI2 pICM->ICMRegisterAPI2
	#define CHK_CMRegisterAPI2 (pICM != NULL)
	#define EXP_CMRegisterAPI2  CAL_CMEXPAPI( "CMRegisterAPI2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMRegisterAPI2  PFCMREGISTERAPI2 pfCMRegisterAPI2;
	#define EXT_CMRegisterAPI2  extern PFCMREGISTERAPI2 pfCMRegisterAPI2;
	#define GET_CMRegisterAPI2(fl)  s_pfCMGetAPI2( "CMRegisterAPI2", (RTS_VOID_FCTPTR *)&pfCMRegisterAPI2, (fl), 0, 0)
	#define CAL_CMRegisterAPI2  pfCMRegisterAPI2
	#define CHK_CMRegisterAPI2  (pfCMRegisterAPI2 != NULL)
	#define EXP_CMRegisterAPI2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMRegisterAPI2", (RTS_UINTPTR)CMRegisterAPI2, 0, 0) 
#endif




/**
 * <description> 
 *   Called to get an API function of another component
 * </description>
 * <param name="pszAPIName" type="IN">Name of the API routine</param>
 * <param name="ppfAPIFunction" type="OUT">Returned the function pointer of the API routine</param>
 * <param name="ulSignatureID" type="IN">SignatureID of the function prototype we expected</param>
 * <result>error code</result>
 */
DLL_DECL RTS_RESULT CDECL CMGetAPI(char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, RTS_UI32 ulSignatureID);
typedef RTS_RESULT (CDECL * PFCMGETAPI) (char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, RTS_UI32 ulSignatureID);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETAPI_NOTIMPLEMENTED)
	#define USE_CMGetAPI
	#define EXT_CMGetAPI
	#define GET_CMGetAPI(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetAPI(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMGetAPI  FALSE
	#define EXP_CMGetAPI  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetAPI
	#define EXT_CMGetAPI
	#define GET_CMGetAPI(fl)  CAL_CMGETAPI( "CMGetAPI" ) 
	#define CAL_CMGetAPI  CMGetAPI
	#define CHK_CMGetAPI  TRUE
	#define EXP_CMGetAPI  CAL_CMEXPAPI( "CMGetAPI" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetAPI
	#define EXT_CMGetAPI
	#define GET_CMGetAPI(fl)  CAL_CMGETAPI( "CMGetAPI" ) 
	#define CAL_CMGetAPI  CMGetAPI
	#define CHK_CMGetAPI  TRUE
	#define EXP_CMGetAPI  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetAPI", (RTS_UINTPTR)CMGetAPI, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetAPI
	#define EXT_CMCMGetAPI
	#define GET_CMCMGetAPI  ERR_OK
	#define CAL_CMCMGetAPI pICM->ICMGetAPI
	#define CHK_CMCMGetAPI (pICM != NULL)
	#define EXP_CMCMGetAPI  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetAPI
	#define EXT_CMGetAPI
	#define GET_CMGetAPI(fl)  CAL_CMGETAPI( "CMGetAPI" ) 
	#define CAL_CMGetAPI pICM->ICMGetAPI
	#define CHK_CMGetAPI (pICM != NULL)
	#define EXP_CMGetAPI  CAL_CMEXPAPI( "CMGetAPI" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetAPI  PFCMGETAPI pfCMGetAPI;
	#define EXT_CMGetAPI  extern PFCMGETAPI pfCMGetAPI;
	#define GET_CMGetAPI(fl)  s_pfCMGetAPI2( "CMGetAPI", (RTS_VOID_FCTPTR *)&pfCMGetAPI, (fl), 0, 0)
	#define CAL_CMGetAPI  pfCMGetAPI
	#define CHK_CMGetAPI  (pfCMGetAPI != NULL)
	#define EXP_CMGetAPI  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetAPI", (RTS_UINTPTR)CMGetAPI, 0, 0) 
#endif




/**
 * <description> 
 *   Called to get an API function of another component
 * </description>
 * <param name="pszAPIName" type="IN">Name of the API routine</param>
 * <param name="ppfAPIFunction" type="OUT">Returned the function pointer of the API routine</param>
 * <param name="ulSignatureID" type="IN">SignatureID of the function prototype we expected</param>
 * <param name="ulVersion" type="IN">Actual implementation version requested</param>
 * <param name="importOptions" type="IN">Import options. See CM_IMPORT_xx defines in CmpStd.h for details</param>
 * <result>error code</result>
 */
DLL_DECL RTS_RESULT CDECL CMGetAPI2(char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, int importOptions, RTS_UI32 ulSignatureID, RTS_UI32 ulVersion);
typedef RTS_RESULT (CDECL * PFCMGETAPI2) (char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, int importOptions, RTS_UI32 ulSignatureID, RTS_UI32 ulVersion);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETAPI2_NOTIMPLEMENTED)
	#define USE_CMGetAPI2
	#define EXT_CMGetAPI2
	#define GET_CMGetAPI2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetAPI2(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMGetAPI2  FALSE
	#define EXP_CMGetAPI2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetAPI2
	#define EXT_CMGetAPI2
	#define GET_CMGetAPI2(fl)  CAL_CMGETAPI( "CMGetAPI2" ) 
	#define CAL_CMGetAPI2  CMGetAPI2
	#define CHK_CMGetAPI2  TRUE
	#define EXP_CMGetAPI2  CAL_CMEXPAPI( "CMGetAPI2" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetAPI2
	#define EXT_CMGetAPI2
	#define GET_CMGetAPI2(fl)  CAL_CMGETAPI( "CMGetAPI2" ) 
	#define CAL_CMGetAPI2  CMGetAPI2
	#define CHK_CMGetAPI2  TRUE
	#define EXP_CMGetAPI2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetAPI2", (RTS_UINTPTR)CMGetAPI2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetAPI2
	#define EXT_CMCMGetAPI2
	#define GET_CMCMGetAPI2  ERR_OK
	#define CAL_CMCMGetAPI2 pICM->ICMGetAPI2
	#define CHK_CMCMGetAPI2 (pICM != NULL)
	#define EXP_CMCMGetAPI2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetAPI2
	#define EXT_CMGetAPI2
	#define GET_CMGetAPI2(fl)  CAL_CMGETAPI( "CMGetAPI2" ) 
	#define CAL_CMGetAPI2 pICM->ICMGetAPI2
	#define CHK_CMGetAPI2 (pICM != NULL)
	#define EXP_CMGetAPI2  CAL_CMEXPAPI( "CMGetAPI2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetAPI2  PFCMGETAPI2 pfCMGetAPI2;
	#define EXT_CMGetAPI2  extern PFCMGETAPI2 pfCMGetAPI2;
	#define GET_CMGetAPI2(fl)  s_pfCMGetAPI2( "CMGetAPI2", (RTS_VOID_FCTPTR *)&pfCMGetAPI2, (fl), 0, 0)
	#define CAL_CMGetAPI2  pfCMGetAPI2
	#define CHK_CMGetAPI2  (pfCMGetAPI2 != NULL)
	#define EXP_CMGetAPI2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetAPI2", (RTS_UINTPTR)CMGetAPI2, 0, 0) 
#endif




/**
 * <description> 
 *   Called to get an API function of another component
 * </description>
 * <param name="pszAPIName" type="IN">Name of the API routine</param>
 * <param name="ppfAPIFunction" type="OUT">Returned the function pointer of the API routine</param>
 * <param name="pulSignatureID" type="INOUT">SignatureID of the function prototype requested and returns the actual signature implemented</param>
 * <param name="pulVersion" type="INOUT">Implementation version requested and returns the actual version implemented</param>
 * <param name="importOptions" type="IN">Import options. See CM_IMPORT_xx defines in CmpStd.h for details</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMGetAPI3(char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, int importOptions, RTS_UI32 *pulSignatureID, RTS_UI32 *pulVersion);
typedef RTS_RESULT (CDECL * PFCMGETAPI3) (char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, int importOptions, RTS_UI32 *pulSignatureID, RTS_UI32 *pulVersion);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETAPI3_NOTIMPLEMENTED)
	#define USE_CMGetAPI3
	#define EXT_CMGetAPI3
	#define GET_CMGetAPI3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetAPI3(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMGetAPI3  FALSE
	#define EXP_CMGetAPI3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetAPI3
	#define EXT_CMGetAPI3
	#define GET_CMGetAPI3(fl)  CAL_CMGETAPI( "CMGetAPI3" ) 
	#define CAL_CMGetAPI3  CMGetAPI3
	#define CHK_CMGetAPI3  TRUE
	#define EXP_CMGetAPI3  CAL_CMEXPAPI( "CMGetAPI3" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetAPI3
	#define EXT_CMGetAPI3
	#define GET_CMGetAPI3(fl)  CAL_CMGETAPI( "CMGetAPI3" ) 
	#define CAL_CMGetAPI3  CMGetAPI3
	#define CHK_CMGetAPI3  TRUE
	#define EXP_CMGetAPI3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetAPI3", (RTS_UINTPTR)CMGetAPI3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetAPI3
	#define EXT_CMCMGetAPI3
	#define GET_CMCMGetAPI3  ERR_OK
	#define CAL_CMCMGetAPI3 pICM->ICMGetAPI3
	#define CHK_CMCMGetAPI3 (pICM != NULL)
	#define EXP_CMCMGetAPI3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetAPI3
	#define EXT_CMGetAPI3
	#define GET_CMGetAPI3(fl)  CAL_CMGETAPI( "CMGetAPI3" ) 
	#define CAL_CMGetAPI3 pICM->ICMGetAPI3
	#define CHK_CMGetAPI3 (pICM != NULL)
	#define EXP_CMGetAPI3  CAL_CMEXPAPI( "CMGetAPI3" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetAPI3  PFCMGETAPI3 pfCMGetAPI3;
	#define EXT_CMGetAPI3  extern PFCMGETAPI3 pfCMGetAPI3;
	#define GET_CMGetAPI3(fl)  s_pfCMGetAPI2( "CMGetAPI3", (RTS_VOID_FCTPTR *)&pfCMGetAPI3, (fl), 0, 0)
	#define CAL_CMGetAPI3  pfCMGetAPI3
	#define CHK_CMGetAPI3  (pfCMGetAPI3 != NULL)
	#define EXP_CMGetAPI3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetAPI3", (RTS_UINTPTR)CMGetAPI3, 0, 0) 
#endif




/**
 * <description> 
 *   Returns the info of the current API function to resolve. Only valid during the run of CMGetAPIx. Otherwise the function returns ERR_NOT_INITIALIZED.
 * </description>
 * <param name="pApiResolveInfo" type="OUT">API function info to be filled. See ApiResolveInfo.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMGetAPIToResolve(API_RESOLVE_INFO *pApiResolveInfo);
typedef RTS_RESULT (CDECL * PFCMGETAPITORESOLVE) (API_RESOLVE_INFO *pApiResolveInfo);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETAPITORESOLVE_NOTIMPLEMENTED)
	#define USE_CMGetAPIToResolve
	#define EXT_CMGetAPIToResolve
	#define GET_CMGetAPIToResolve(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetAPIToResolve(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMGetAPIToResolve  FALSE
	#define EXP_CMGetAPIToResolve  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetAPIToResolve
	#define EXT_CMGetAPIToResolve
	#define GET_CMGetAPIToResolve(fl)  CAL_CMGETAPI( "CMGetAPIToResolve" ) 
	#define CAL_CMGetAPIToResolve  CMGetAPIToResolve
	#define CHK_CMGetAPIToResolve  TRUE
	#define EXP_CMGetAPIToResolve  CAL_CMEXPAPI( "CMGetAPIToResolve" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetAPIToResolve
	#define EXT_CMGetAPIToResolve
	#define GET_CMGetAPIToResolve(fl)  CAL_CMGETAPI( "CMGetAPIToResolve" ) 
	#define CAL_CMGetAPIToResolve  CMGetAPIToResolve
	#define CHK_CMGetAPIToResolve  TRUE
	#define EXP_CMGetAPIToResolve  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetAPIToResolve", (RTS_UINTPTR)CMGetAPIToResolve, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetAPIToResolve
	#define EXT_CMCMGetAPIToResolve
	#define GET_CMCMGetAPIToResolve  ERR_OK
	#define CAL_CMCMGetAPIToResolve pICM->ICMGetAPIToResolve
	#define CHK_CMCMGetAPIToResolve (pICM != NULL)
	#define EXP_CMCMGetAPIToResolve  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetAPIToResolve
	#define EXT_CMGetAPIToResolve
	#define GET_CMGetAPIToResolve(fl)  CAL_CMGETAPI( "CMGetAPIToResolve" ) 
	#define CAL_CMGetAPIToResolve pICM->ICMGetAPIToResolve
	#define CHK_CMGetAPIToResolve (pICM != NULL)
	#define EXP_CMGetAPIToResolve  CAL_CMEXPAPI( "CMGetAPIToResolve" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetAPIToResolve  PFCMGETAPITORESOLVE pfCMGetAPIToResolve;
	#define EXT_CMGetAPIToResolve  extern PFCMGETAPITORESOLVE pfCMGetAPIToResolve;
	#define GET_CMGetAPIToResolve(fl)  s_pfCMGetAPI2( "CMGetAPIToResolve", (RTS_VOID_FCTPTR *)&pfCMGetAPIToResolve, (fl), 0, 0)
	#define CAL_CMGetAPIToResolve  pfCMGetAPIToResolve
	#define CHK_CMGetAPIToResolve  (pfCMGetAPIToResolve != NULL)
	#define EXP_CMGetAPIToResolve  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetAPIToResolve", (RTS_UINTPTR)CMGetAPIToResolve, 0, 0) 
#endif




/**
 * <description> 
 *   Called to release an API function of another component
 * </description>
 * <param name="pfAPIFunction" type="IN"></param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMReleaseAPI(RTS_VOID_FCTPTR pfAPIFunction);
typedef RTS_RESULT (CDECL * PFCMRELEASEAPI) (RTS_VOID_FCTPTR pfAPIFunction);
#if defined(CM_NOTIMPLEMENTED) || defined(CMRELEASEAPI_NOTIMPLEMENTED)
	#define USE_CMReleaseAPI
	#define EXT_CMReleaseAPI
	#define GET_CMReleaseAPI(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMReleaseAPI(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMReleaseAPI  FALSE
	#define EXP_CMReleaseAPI  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMReleaseAPI
	#define EXT_CMReleaseAPI
	#define GET_CMReleaseAPI(fl)  CAL_CMGETAPI( "CMReleaseAPI" ) 
	#define CAL_CMReleaseAPI  CMReleaseAPI
	#define CHK_CMReleaseAPI  TRUE
	#define EXP_CMReleaseAPI  CAL_CMEXPAPI( "CMReleaseAPI" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMReleaseAPI
	#define EXT_CMReleaseAPI
	#define GET_CMReleaseAPI(fl)  CAL_CMGETAPI( "CMReleaseAPI" ) 
	#define CAL_CMReleaseAPI  CMReleaseAPI
	#define CHK_CMReleaseAPI  TRUE
	#define EXP_CMReleaseAPI  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMReleaseAPI", (RTS_UINTPTR)CMReleaseAPI, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMReleaseAPI
	#define EXT_CMCMReleaseAPI
	#define GET_CMCMReleaseAPI  ERR_OK
	#define CAL_CMCMReleaseAPI pICM->ICMReleaseAPI
	#define CHK_CMCMReleaseAPI (pICM != NULL)
	#define EXP_CMCMReleaseAPI  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMReleaseAPI
	#define EXT_CMReleaseAPI
	#define GET_CMReleaseAPI(fl)  CAL_CMGETAPI( "CMReleaseAPI" ) 
	#define CAL_CMReleaseAPI pICM->ICMReleaseAPI
	#define CHK_CMReleaseAPI (pICM != NULL)
	#define EXP_CMReleaseAPI  CAL_CMEXPAPI( "CMReleaseAPI" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMReleaseAPI  PFCMRELEASEAPI pfCMReleaseAPI;
	#define EXT_CMReleaseAPI  extern PFCMRELEASEAPI pfCMReleaseAPI;
	#define GET_CMReleaseAPI(fl)  s_pfCMGetAPI2( "CMReleaseAPI", (RTS_VOID_FCTPTR *)&pfCMReleaseAPI, (fl), 0, 0)
	#define CAL_CMReleaseAPI  pfCMReleaseAPI
	#define CHK_CMReleaseAPI  (pfCMReleaseAPI != NULL)
	#define EXP_CMReleaseAPI  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMReleaseAPI", (RTS_UINTPTR)CMReleaseAPI, 0, 0) 
#endif




/**
 * <description> 
 *   Called to load a component. Can also be called during runtime.
 *	 ATTENTION: If component has references to other components, referenced components must be loaded first!
 * </description>
 * <param name="pszComponent" type="IN">Name of the component</param>
 * <param name="pResult" type="INOUT">Pointer to error code</param>
 * <result>Component handle or RTS_INVALID_HANDLE, if an error is occured</result>
 */
RTS_HANDLE CDECL CMLoadComponent(char *pszComponent, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFCMLOADCOMPONENT) (char *pszComponent, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMLOADCOMPONENT_NOTIMPLEMENTED)
	#define USE_CMLoadComponent
	#define EXT_CMLoadComponent
	#define GET_CMLoadComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMLoadComponent(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CMLoadComponent  FALSE
	#define EXP_CMLoadComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMLoadComponent
	#define EXT_CMLoadComponent
	#define GET_CMLoadComponent(fl)  CAL_CMGETAPI( "CMLoadComponent" ) 
	#define CAL_CMLoadComponent  CMLoadComponent
	#define CHK_CMLoadComponent  TRUE
	#define EXP_CMLoadComponent  CAL_CMEXPAPI( "CMLoadComponent" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMLoadComponent
	#define EXT_CMLoadComponent
	#define GET_CMLoadComponent(fl)  CAL_CMGETAPI( "CMLoadComponent" ) 
	#define CAL_CMLoadComponent  CMLoadComponent
	#define CHK_CMLoadComponent  TRUE
	#define EXP_CMLoadComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLoadComponent", (RTS_UINTPTR)CMLoadComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMLoadComponent
	#define EXT_CMCMLoadComponent
	#define GET_CMCMLoadComponent  ERR_OK
	#define CAL_CMCMLoadComponent pICM->ICMLoadComponent
	#define CHK_CMCMLoadComponent (pICM != NULL)
	#define EXP_CMCMLoadComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMLoadComponent
	#define EXT_CMLoadComponent
	#define GET_CMLoadComponent(fl)  CAL_CMGETAPI( "CMLoadComponent" ) 
	#define CAL_CMLoadComponent pICM->ICMLoadComponent
	#define CHK_CMLoadComponent (pICM != NULL)
	#define EXP_CMLoadComponent  CAL_CMEXPAPI( "CMLoadComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMLoadComponent  PFCMLOADCOMPONENT pfCMLoadComponent;
	#define EXT_CMLoadComponent  extern PFCMLOADCOMPONENT pfCMLoadComponent;
	#define GET_CMLoadComponent(fl)  s_pfCMGetAPI2( "CMLoadComponent", (RTS_VOID_FCTPTR *)&pfCMLoadComponent, (fl), 0, 0)
	#define CAL_CMLoadComponent  pfCMLoadComponent
	#define CHK_CMLoadComponent  (pfCMLoadComponent != NULL)
	#define EXP_CMLoadComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLoadComponent", (RTS_UINTPTR)CMLoadComponent, 0, 0) 
#endif



void CDECL CDECL_EXT cmloadcomponent(cmloadcomponent_struct *p);
typedef void (CDECL CDECL_EXT* PFCMLOADCOMPONENT_IEC) (cmloadcomponent_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMLOADCOMPONENT_NOTIMPLEMENTED)
	#define USE_cmloadcomponent
	#define EXT_cmloadcomponent
	#define GET_cmloadcomponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmloadcomponent(p0) 
	#define CHK_cmloadcomponent  FALSE
	#define EXP_cmloadcomponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmloadcomponent
	#define EXT_cmloadcomponent
	#define GET_cmloadcomponent(fl)  CAL_CMGETAPI( "cmloadcomponent" ) 
	#define CAL_cmloadcomponent  cmloadcomponent
	#define CHK_cmloadcomponent  TRUE
	#define EXP_cmloadcomponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmloadcomponent", (RTS_UINTPTR)cmloadcomponent, 1, 0) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmloadcomponent
	#define EXT_cmloadcomponent
	#define GET_cmloadcomponent(fl)  CAL_CMGETAPI( "cmloadcomponent" ) 
	#define CAL_cmloadcomponent  cmloadcomponent
	#define CHK_cmloadcomponent  TRUE
	#define EXP_cmloadcomponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmloadcomponent", (RTS_UINTPTR)cmloadcomponent, 1, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmloadcomponent
	#define EXT_CMcmloadcomponent
	#define GET_CMcmloadcomponent  ERR_OK
	#define CAL_CMcmloadcomponent  cmloadcomponent
	#define CHK_CMcmloadcomponent  TRUE
	#define EXP_CMcmloadcomponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmloadcomponent", (RTS_UINTPTR)cmloadcomponent, 1, 0) 
#elif defined(CPLUSPLUS)
	#define USE_cmloadcomponent
	#define EXT_cmloadcomponent
	#define GET_cmloadcomponent(fl)  CAL_CMGETAPI( "cmloadcomponent" ) 
	#define CAL_cmloadcomponent  cmloadcomponent
	#define CHK_cmloadcomponent  TRUE
	#define EXP_cmloadcomponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmloadcomponent", (RTS_UINTPTR)cmloadcomponent, 1, 0) 
#else /* DYNAMIC_LINK */
	#define USE_cmloadcomponent  PFCMLOADCOMPONENT_IEC pfcmloadcomponent;
	#define EXT_cmloadcomponent  extern PFCMLOADCOMPONENT_IEC pfcmloadcomponent;
	#define GET_cmloadcomponent(fl)  s_pfCMGetAPI2( "cmloadcomponent", (RTS_VOID_FCTPTR *)&pfcmloadcomponent, (fl), 0, 0)
	#define CAL_cmloadcomponent  pfcmloadcomponent
	#define CHK_cmloadcomponent  (pfcmloadcomponent != NULL)
	#define EXP_cmloadcomponent   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmloadcomponent", (RTS_UINTPTR)cmloadcomponent, 1, 0) 
#endif


/**
 * <description> 
 *   Called to load a component. Can also be called during runtime.
 *	 ATTENTION: If component has references to other components, referenced components must be loaded first!
 * </description>
 * <param name="pszComponent" type="IN">Name of the component</param>
 * <param name="iType" type="IN">Type of the component. See category "ComponentType" for details.</param>
 * <param name="pResult" type="INOUT">Pointer to error code</param>
 * <result>Component handle or RTS_INVALID_HANDLE, if an error is occured</result>
 */
RTS_HANDLE CDECL CMLoadComponent2(char *pszComponent, RTS_UI16 iType, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFCMLOADCOMPONENT2) (char *pszComponent, RTS_UI16 iType, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMLOADCOMPONENT2_NOTIMPLEMENTED)
	#define USE_CMLoadComponent2
	#define EXT_CMLoadComponent2
	#define GET_CMLoadComponent2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMLoadComponent2(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CMLoadComponent2  FALSE
	#define EXP_CMLoadComponent2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMLoadComponent2
	#define EXT_CMLoadComponent2
	#define GET_CMLoadComponent2(fl)  CAL_CMGETAPI( "CMLoadComponent2" ) 
	#define CAL_CMLoadComponent2  CMLoadComponent2
	#define CHK_CMLoadComponent2  TRUE
	#define EXP_CMLoadComponent2  CAL_CMEXPAPI( "CMLoadComponent2" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMLoadComponent2
	#define EXT_CMLoadComponent2
	#define GET_CMLoadComponent2(fl)  CAL_CMGETAPI( "CMLoadComponent2" ) 
	#define CAL_CMLoadComponent2  CMLoadComponent2
	#define CHK_CMLoadComponent2  TRUE
	#define EXP_CMLoadComponent2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLoadComponent2", (RTS_UINTPTR)CMLoadComponent2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMLoadComponent2
	#define EXT_CMCMLoadComponent2
	#define GET_CMCMLoadComponent2  ERR_OK
	#define CAL_CMCMLoadComponent2 pICM->ICMLoadComponent2
	#define CHK_CMCMLoadComponent2 (pICM != NULL)
	#define EXP_CMCMLoadComponent2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMLoadComponent2
	#define EXT_CMLoadComponent2
	#define GET_CMLoadComponent2(fl)  CAL_CMGETAPI( "CMLoadComponent2" ) 
	#define CAL_CMLoadComponent2 pICM->ICMLoadComponent2
	#define CHK_CMLoadComponent2 (pICM != NULL)
	#define EXP_CMLoadComponent2  CAL_CMEXPAPI( "CMLoadComponent2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMLoadComponent2  PFCMLOADCOMPONENT2 pfCMLoadComponent2;
	#define EXT_CMLoadComponent2  extern PFCMLOADCOMPONENT2 pfCMLoadComponent2;
	#define GET_CMLoadComponent2(fl)  s_pfCMGetAPI2( "CMLoadComponent2", (RTS_VOID_FCTPTR *)&pfCMLoadComponent2, (fl), 0, 0)
	#define CAL_CMLoadComponent2  pfCMLoadComponent2
	#define CHK_CMLoadComponent2  (pfCMLoadComponent2 != NULL)
	#define EXP_CMLoadComponent2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLoadComponent2", (RTS_UINTPTR)CMLoadComponent2, 0, 0) 
#endif




/**
 * <description> 
 *   Called to load a component. Can also be called during runtime.
 *	 ATTENTION: If component has references to other components, referenced components must be loaded first!
 * </description>
 * <param name="pszComponent" type="IN">Name of the component</param>
 * <param name="pszFilePath" type="IN">Complete file path of the component</param>
 * <param name="iType" type="IN">Type of the component. See category "ComponentType" for details.</param>
 * <param name="pResult" type="INOUT">Pointer to error code</param>
 * <result>Component handle or RTS_INVALID_HANDLE, if an error is occured</result>
 */
RTS_HANDLE CDECL CMLoadComponent3(char *pszComponent, char *pszFilePath, RTS_UI16 iType, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFCMLOADCOMPONENT3) (char *pszComponent, char *pszFilePath, RTS_UI16 iType, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMLOADCOMPONENT3_NOTIMPLEMENTED)
	#define USE_CMLoadComponent3
	#define EXT_CMLoadComponent3
	#define GET_CMLoadComponent3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMLoadComponent3(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CMLoadComponent3  FALSE
	#define EXP_CMLoadComponent3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMLoadComponent3
	#define EXT_CMLoadComponent3
	#define GET_CMLoadComponent3(fl)  CAL_CMGETAPI( "CMLoadComponent3" ) 
	#define CAL_CMLoadComponent3  CMLoadComponent3
	#define CHK_CMLoadComponent3  TRUE
	#define EXP_CMLoadComponent3  CAL_CMEXPAPI( "CMLoadComponent3" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMLoadComponent3
	#define EXT_CMLoadComponent3
	#define GET_CMLoadComponent3(fl)  CAL_CMGETAPI( "CMLoadComponent3" ) 
	#define CAL_CMLoadComponent3  CMLoadComponent3
	#define CHK_CMLoadComponent3  TRUE
	#define EXP_CMLoadComponent3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLoadComponent3", (RTS_UINTPTR)CMLoadComponent3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMLoadComponent3
	#define EXT_CMCMLoadComponent3
	#define GET_CMCMLoadComponent3  ERR_OK
	#define CAL_CMCMLoadComponent3 pICM->ICMLoadComponent3
	#define CHK_CMCMLoadComponent3 (pICM != NULL)
	#define EXP_CMCMLoadComponent3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMLoadComponent3
	#define EXT_CMLoadComponent3
	#define GET_CMLoadComponent3(fl)  CAL_CMGETAPI( "CMLoadComponent3" ) 
	#define CAL_CMLoadComponent3 pICM->ICMLoadComponent3
	#define CHK_CMLoadComponent3 (pICM != NULL)
	#define EXP_CMLoadComponent3  CAL_CMEXPAPI( "CMLoadComponent3" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMLoadComponent3  PFCMLOADCOMPONENT3 pfCMLoadComponent3;
	#define EXT_CMLoadComponent3  extern PFCMLOADCOMPONENT3 pfCMLoadComponent3;
	#define GET_CMLoadComponent3(fl)  s_pfCMGetAPI2( "CMLoadComponent3", (RTS_VOID_FCTPTR *)&pfCMLoadComponent3, (fl), 0, 0)
	#define CAL_CMLoadComponent3  pfCMLoadComponent3
	#define CHK_CMLoadComponent3  (pfCMLoadComponent3 != NULL)
	#define EXP_CMLoadComponent3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMLoadComponent3", (RTS_UINTPTR)CMLoadComponent3, 0, 0) 
#endif




/**
 * <description> 
 *   Called to initialize a component after it was loaded during runtime.
 * </description>
 * <param name="hComponent" type="IN">Handle of the component</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMInitComponent(RTS_HANDLE hComponent);
typedef RTS_RESULT (CDECL * PFCMINITCOMPONENT) (RTS_HANDLE hComponent);
#if defined(CM_NOTIMPLEMENTED) || defined(CMINITCOMPONENT_NOTIMPLEMENTED)
	#define USE_CMInitComponent
	#define EXT_CMInitComponent
	#define GET_CMInitComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMInitComponent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMInitComponent  FALSE
	#define EXP_CMInitComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMInitComponent
	#define EXT_CMInitComponent
	#define GET_CMInitComponent(fl)  CAL_CMGETAPI( "CMInitComponent" ) 
	#define CAL_CMInitComponent  CMInitComponent
	#define CHK_CMInitComponent  TRUE
	#define EXP_CMInitComponent  CAL_CMEXPAPI( "CMInitComponent" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMInitComponent
	#define EXT_CMInitComponent
	#define GET_CMInitComponent(fl)  CAL_CMGETAPI( "CMInitComponent" ) 
	#define CAL_CMInitComponent  CMInitComponent
	#define CHK_CMInitComponent  TRUE
	#define EXP_CMInitComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInitComponent", (RTS_UINTPTR)CMInitComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMInitComponent
	#define EXT_CMCMInitComponent
	#define GET_CMCMInitComponent  ERR_OK
	#define CAL_CMCMInitComponent pICM->ICMInitComponent
	#define CHK_CMCMInitComponent (pICM != NULL)
	#define EXP_CMCMInitComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMInitComponent
	#define EXT_CMInitComponent
	#define GET_CMInitComponent(fl)  CAL_CMGETAPI( "CMInitComponent" ) 
	#define CAL_CMInitComponent pICM->ICMInitComponent
	#define CHK_CMInitComponent (pICM != NULL)
	#define EXP_CMInitComponent  CAL_CMEXPAPI( "CMInitComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMInitComponent  PFCMINITCOMPONENT pfCMInitComponent;
	#define EXT_CMInitComponent  extern PFCMINITCOMPONENT pfCMInitComponent;
	#define GET_CMInitComponent(fl)  s_pfCMGetAPI2( "CMInitComponent", (RTS_VOID_FCTPTR *)&pfCMInitComponent, (fl), 0, 0)
	#define CAL_CMInitComponent  pfCMInitComponent
	#define CHK_CMInitComponent  (pfCMInitComponent != NULL)
	#define EXP_CMInitComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInitComponent", (RTS_UINTPTR)CMInitComponent, 0, 0) 
#endif




/**
 * <description> 
 *   Called to update the function pointers of the previous loaded components to let them point to the new added functions.  
 * </description>
 *  <result>error code</result>
 */
RTS_RESULT CDECL CMImportFunctions(void);
typedef RTS_RESULT (CDECL * PFCMIMPORTFUNCTIONS) (void);
#if defined(CM_NOTIMPLEMENTED) || defined(CMIMPORTFUNCTIONS_NOTIMPLEMENTED)
	#define USE_CMImportFunctions
	#define EXT_CMImportFunctions
	#define GET_CMImportFunctions(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMImportFunctions()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMImportFunctions  FALSE
	#define EXP_CMImportFunctions  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMImportFunctions
	#define EXT_CMImportFunctions
	#define GET_CMImportFunctions(fl)  CAL_CMGETAPI( "CMImportFunctions" ) 
	#define CAL_CMImportFunctions  CMImportFunctions
	#define CHK_CMImportFunctions  TRUE
	#define EXP_CMImportFunctions  CAL_CMEXPAPI( "CMImportFunctions" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMImportFunctions
	#define EXT_CMImportFunctions
	#define GET_CMImportFunctions(fl)  CAL_CMGETAPI( "CMImportFunctions" ) 
	#define CAL_CMImportFunctions  CMImportFunctions
	#define CHK_CMImportFunctions  TRUE
	#define EXP_CMImportFunctions  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMImportFunctions", (RTS_UINTPTR)CMImportFunctions, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMImportFunctions
	#define EXT_CMCMImportFunctions
	#define GET_CMCMImportFunctions  ERR_OK
	#define CAL_CMCMImportFunctions pICM->ICMImportFunctions
	#define CHK_CMCMImportFunctions (pICM != NULL)
	#define EXP_CMCMImportFunctions  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMImportFunctions
	#define EXT_CMImportFunctions
	#define GET_CMImportFunctions(fl)  CAL_CMGETAPI( "CMImportFunctions" ) 
	#define CAL_CMImportFunctions pICM->ICMImportFunctions
	#define CHK_CMImportFunctions (pICM != NULL)
	#define EXP_CMImportFunctions  CAL_CMEXPAPI( "CMImportFunctions" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMImportFunctions  PFCMIMPORTFUNCTIONS pfCMImportFunctions;
	#define EXT_CMImportFunctions  extern PFCMIMPORTFUNCTIONS pfCMImportFunctions;
	#define GET_CMImportFunctions(fl)  s_pfCMGetAPI2( "CMImportFunctions", (RTS_VOID_FCTPTR *)&pfCMImportFunctions, (fl), 0, 0)
	#define CAL_CMImportFunctions  pfCMImportFunctions
	#define CHK_CMImportFunctions  (pfCMImportFunctions != NULL)
	#define EXP_CMImportFunctions  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMImportFunctions", (RTS_UINTPTR)CMImportFunctions, 0, 0) 
#endif




/**
 * <description> 
 *   Called to exit a component after it was loaded during runtime. After this it can be unloaded,
 *   if no other components references this component
 * </description>
 * <param name="hComponent" type="IN">Handle of the component</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMExitComponent(RTS_HANDLE hComponent);
typedef RTS_RESULT (CDECL * PFCMEXITCOMPONENT) (RTS_HANDLE hComponent);
#if defined(CM_NOTIMPLEMENTED) || defined(CMEXITCOMPONENT_NOTIMPLEMENTED)
	#define USE_CMExitComponent
	#define EXT_CMExitComponent
	#define GET_CMExitComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMExitComponent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMExitComponent  FALSE
	#define EXP_CMExitComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMExitComponent
	#define EXT_CMExitComponent
	#define GET_CMExitComponent(fl)  CAL_CMGETAPI( "CMExitComponent" ) 
	#define CAL_CMExitComponent  CMExitComponent
	#define CHK_CMExitComponent  TRUE
	#define EXP_CMExitComponent  CAL_CMEXPAPI( "CMExitComponent" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMExitComponent
	#define EXT_CMExitComponent
	#define GET_CMExitComponent(fl)  CAL_CMGETAPI( "CMExitComponent" ) 
	#define CAL_CMExitComponent  CMExitComponent
	#define CHK_CMExitComponent  TRUE
	#define EXP_CMExitComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMExitComponent", (RTS_UINTPTR)CMExitComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMExitComponent
	#define EXT_CMCMExitComponent
	#define GET_CMCMExitComponent  ERR_OK
	#define CAL_CMCMExitComponent pICM->ICMExitComponent
	#define CHK_CMCMExitComponent (pICM != NULL)
	#define EXP_CMCMExitComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMExitComponent
	#define EXT_CMExitComponent
	#define GET_CMExitComponent(fl)  CAL_CMGETAPI( "CMExitComponent" ) 
	#define CAL_CMExitComponent pICM->ICMExitComponent
	#define CHK_CMExitComponent (pICM != NULL)
	#define EXP_CMExitComponent  CAL_CMEXPAPI( "CMExitComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMExitComponent  PFCMEXITCOMPONENT pfCMExitComponent;
	#define EXT_CMExitComponent  extern PFCMEXITCOMPONENT pfCMExitComponent;
	#define GET_CMExitComponent(fl)  s_pfCMGetAPI2( "CMExitComponent", (RTS_VOID_FCTPTR *)&pfCMExitComponent, (fl), 0, 0)
	#define CAL_CMExitComponent  pfCMExitComponent
	#define CHK_CMExitComponent  (pfCMExitComponent != NULL)
	#define EXP_CMExitComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMExitComponent", (RTS_UINTPTR)CMExitComponent, 0, 0) 
#endif




/**
 * <description> 
 *   Called to unload a component. Can also be called during runtime.
 *	 ATTENTION: CMExitComponent must be called before unloading the component!
 * </description>
 * <param name="hComponent" type="IN">Handle of the component</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUnloadComponent(RTS_HANDLE hComponent);
typedef RTS_RESULT (CDECL * PFCMUNLOADCOMPONENT) (RTS_HANDLE hComponent);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUNLOADCOMPONENT_NOTIMPLEMENTED)
	#define USE_CMUnloadComponent
	#define EXT_CMUnloadComponent
	#define GET_CMUnloadComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUnloadComponent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUnloadComponent  FALSE
	#define EXP_CMUnloadComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUnloadComponent
	#define EXT_CMUnloadComponent
	#define GET_CMUnloadComponent(fl)  CAL_CMGETAPI( "CMUnloadComponent" ) 
	#define CAL_CMUnloadComponent  CMUnloadComponent
	#define CHK_CMUnloadComponent  TRUE
	#define EXP_CMUnloadComponent  CAL_CMEXPAPI( "CMUnloadComponent" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMUnloadComponent
	#define EXT_CMUnloadComponent
	#define GET_CMUnloadComponent(fl)  CAL_CMGETAPI( "CMUnloadComponent" ) 
	#define CAL_CMUnloadComponent  CMUnloadComponent
	#define CHK_CMUnloadComponent  TRUE
	#define EXP_CMUnloadComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUnloadComponent", (RTS_UINTPTR)CMUnloadComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMUnloadComponent
	#define EXT_CMCMUnloadComponent
	#define GET_CMCMUnloadComponent  ERR_OK
	#define CAL_CMCMUnloadComponent pICM->ICMUnloadComponent
	#define CHK_CMCMUnloadComponent (pICM != NULL)
	#define EXP_CMCMUnloadComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUnloadComponent
	#define EXT_CMUnloadComponent
	#define GET_CMUnloadComponent(fl)  CAL_CMGETAPI( "CMUnloadComponent" ) 
	#define CAL_CMUnloadComponent pICM->ICMUnloadComponent
	#define CHK_CMUnloadComponent (pICM != NULL)
	#define EXP_CMUnloadComponent  CAL_CMEXPAPI( "CMUnloadComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUnloadComponent  PFCMUNLOADCOMPONENT pfCMUnloadComponent;
	#define EXT_CMUnloadComponent  extern PFCMUNLOADCOMPONENT pfCMUnloadComponent;
	#define GET_CMUnloadComponent(fl)  s_pfCMGetAPI2( "CMUnloadComponent", (RTS_VOID_FCTPTR *)&pfCMUnloadComponent, (fl), 0, 0)
	#define CAL_CMUnloadComponent  pfCMUnloadComponent
	#define CHK_CMUnloadComponent  (pfCMUnloadComponent != NULL)
	#define EXP_CMUnloadComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUnloadComponent", (RTS_UINTPTR)CMUnloadComponent, 0, 0) 
#endif



void CDECL CDECL_EXT cmunloadcomponent(cmunloadcomponent_struct *p);
typedef void (CDECL CDECL_EXT* PFCMUNLOADCOMPONENT_IEC) (cmunloadcomponent_struct *p);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUNLOADCOMPONENT_NOTIMPLEMENTED)
	#define USE_cmunloadcomponent
	#define EXT_cmunloadcomponent
	#define GET_cmunloadcomponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cmunloadcomponent(p0) 
	#define CHK_cmunloadcomponent  FALSE
	#define EXP_cmunloadcomponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cmunloadcomponent
	#define EXT_cmunloadcomponent
	#define GET_cmunloadcomponent(fl)  CAL_CMGETAPI( "cmunloadcomponent" ) 
	#define CAL_cmunloadcomponent  cmunloadcomponent
	#define CHK_cmunloadcomponent  TRUE
	#define EXP_cmunloadcomponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmunloadcomponent", (RTS_UINTPTR)cmunloadcomponent, 1, 0) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_cmunloadcomponent
	#define EXT_cmunloadcomponent
	#define GET_cmunloadcomponent(fl)  CAL_CMGETAPI( "cmunloadcomponent" ) 
	#define CAL_cmunloadcomponent  cmunloadcomponent
	#define CHK_cmunloadcomponent  TRUE
	#define EXP_cmunloadcomponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmunloadcomponent", (RTS_UINTPTR)cmunloadcomponent, 1, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMcmunloadcomponent
	#define EXT_CMcmunloadcomponent
	#define GET_CMcmunloadcomponent  ERR_OK
	#define CAL_CMcmunloadcomponent  cmunloadcomponent
	#define CHK_CMcmunloadcomponent  TRUE
	#define EXP_CMcmunloadcomponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmunloadcomponent", (RTS_UINTPTR)cmunloadcomponent, 1, 0) 
#elif defined(CPLUSPLUS)
	#define USE_cmunloadcomponent
	#define EXT_cmunloadcomponent
	#define GET_cmunloadcomponent(fl)  CAL_CMGETAPI( "cmunloadcomponent" ) 
	#define CAL_cmunloadcomponent  cmunloadcomponent
	#define CHK_cmunloadcomponent  TRUE
	#define EXP_cmunloadcomponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmunloadcomponent", (RTS_UINTPTR)cmunloadcomponent, 1, 0) 
#else /* DYNAMIC_LINK */
	#define USE_cmunloadcomponent  PFCMUNLOADCOMPONENT_IEC pfcmunloadcomponent;
	#define EXT_cmunloadcomponent  extern PFCMUNLOADCOMPONENT_IEC pfcmunloadcomponent;
	#define GET_cmunloadcomponent(fl)  s_pfCMGetAPI2( "cmunloadcomponent", (RTS_VOID_FCTPTR *)&pfcmunloadcomponent, (fl), 0, 0)
	#define CAL_cmunloadcomponent  pfcmunloadcomponent
	#define CHK_cmunloadcomponent  (pfcmunloadcomponent != NULL)
	#define EXP_cmunloadcomponent   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"cmunloadcomponent", (RTS_UINTPTR)cmunloadcomponent, 1, 0) 
#endif


/**
 * <description> 
 *   Add a component to the list
 * </description>
 * <param name="pComponent" type="IN">Pointer to component description</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>returns a handle to the component</result>
 */
RTS_HANDLE CDECL CMAddComponent(COMPONENT_ENTRY *pComponent, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFCMADDCOMPONENT) (COMPONENT_ENTRY *pComponent, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMADDCOMPONENT_NOTIMPLEMENTED)
	#define USE_CMAddComponent
	#define EXT_CMAddComponent
	#define GET_CMAddComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMAddComponent(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CMAddComponent  FALSE
	#define EXP_CMAddComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMAddComponent
	#define EXT_CMAddComponent
	#define GET_CMAddComponent(fl)  CAL_CMGETAPI( "CMAddComponent" ) 
	#define CAL_CMAddComponent  CMAddComponent
	#define CHK_CMAddComponent  TRUE
	#define EXP_CMAddComponent  CAL_CMEXPAPI( "CMAddComponent" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMAddComponent
	#define EXT_CMAddComponent
	#define GET_CMAddComponent(fl)  CAL_CMGETAPI( "CMAddComponent" ) 
	#define CAL_CMAddComponent  CMAddComponent
	#define CHK_CMAddComponent  TRUE
	#define EXP_CMAddComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMAddComponent", (RTS_UINTPTR)CMAddComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMAddComponent
	#define EXT_CMCMAddComponent
	#define GET_CMCMAddComponent  ERR_OK
	#define CAL_CMCMAddComponent pICM->ICMAddComponent
	#define CHK_CMCMAddComponent (pICM != NULL)
	#define EXP_CMCMAddComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMAddComponent
	#define EXT_CMAddComponent
	#define GET_CMAddComponent(fl)  CAL_CMGETAPI( "CMAddComponent" ) 
	#define CAL_CMAddComponent pICM->ICMAddComponent
	#define CHK_CMAddComponent (pICM != NULL)
	#define EXP_CMAddComponent  CAL_CMEXPAPI( "CMAddComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMAddComponent  PFCMADDCOMPONENT pfCMAddComponent;
	#define EXT_CMAddComponent  extern PFCMADDCOMPONENT pfCMAddComponent;
	#define GET_CMAddComponent(fl)  s_pfCMGetAPI2( "CMAddComponent", (RTS_VOID_FCTPTR *)&pfCMAddComponent, (fl), 0, 0)
	#define CAL_CMAddComponent  pfCMAddComponent
	#define CHK_CMAddComponent  (pfCMAddComponent != NULL)
	#define EXP_CMAddComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMAddComponent", (RTS_UINTPTR)CMAddComponent, 0, 0) 
#endif




/**
 * <description> 
 *   Remove a component from the list
 * </description>
 * <param name="hComponent" type="IN">Handle to the component</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMRemoveComponent(RTS_HANDLE hComponent);
typedef RTS_RESULT (CDECL * PFCMREMOVECOMPONENT) (RTS_HANDLE hComponent);
#if defined(CM_NOTIMPLEMENTED) || defined(CMREMOVECOMPONENT_NOTIMPLEMENTED)
	#define USE_CMRemoveComponent
	#define EXT_CMRemoveComponent
	#define GET_CMRemoveComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMRemoveComponent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMRemoveComponent  FALSE
	#define EXP_CMRemoveComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMRemoveComponent
	#define EXT_CMRemoveComponent
	#define GET_CMRemoveComponent(fl)  CAL_CMGETAPI( "CMRemoveComponent" ) 
	#define CAL_CMRemoveComponent  CMRemoveComponent
	#define CHK_CMRemoveComponent  TRUE
	#define EXP_CMRemoveComponent  CAL_CMEXPAPI( "CMRemoveComponent" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMRemoveComponent
	#define EXT_CMRemoveComponent
	#define GET_CMRemoveComponent(fl)  CAL_CMGETAPI( "CMRemoveComponent" ) 
	#define CAL_CMRemoveComponent  CMRemoveComponent
	#define CHK_CMRemoveComponent  TRUE
	#define EXP_CMRemoveComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMRemoveComponent", (RTS_UINTPTR)CMRemoveComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMRemoveComponent
	#define EXT_CMCMRemoveComponent
	#define GET_CMCMRemoveComponent  ERR_OK
	#define CAL_CMCMRemoveComponent pICM->ICMRemoveComponent
	#define CHK_CMCMRemoveComponent (pICM != NULL)
	#define EXP_CMCMRemoveComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMRemoveComponent
	#define EXT_CMRemoveComponent
	#define GET_CMRemoveComponent(fl)  CAL_CMGETAPI( "CMRemoveComponent" ) 
	#define CAL_CMRemoveComponent pICM->ICMRemoveComponent
	#define CHK_CMRemoveComponent (pICM != NULL)
	#define EXP_CMRemoveComponent  CAL_CMEXPAPI( "CMRemoveComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMRemoveComponent  PFCMREMOVECOMPONENT pfCMRemoveComponent;
	#define EXT_CMRemoveComponent  extern PFCMREMOVECOMPONENT pfCMRemoveComponent;
	#define GET_CMRemoveComponent(fl)  s_pfCMGetAPI2( "CMRemoveComponent", (RTS_VOID_FCTPTR *)&pfCMRemoveComponent, (fl), 0, 0)
	#define CAL_CMRemoveComponent  pfCMRemoveComponent
	#define CHK_CMRemoveComponent  (pfCMRemoveComponent != NULL)
	#define EXP_CMRemoveComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMRemoveComponent", (RTS_UINTPTR)CMRemoveComponent, 0, 0) 
#endif




/**
 * <description> 
 *	Get the component handle of a component specified by name.
 * </description>
 * <param name="pszCmpName" type="IN">Name of the component</param>
 * <param name="pResult" type="INOUT">Pointer to error code</param>
 * <result>Component handle or RTS_INVALID_HANDLE, if an error is occured</result>
 */
RTS_HANDLE CDECL CMGetComponentByName(char *pszCmpName, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFCMGETCOMPONENTBYNAME) (char *pszCmpName, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETCOMPONENTBYNAME_NOTIMPLEMENTED)
	#define USE_CMGetComponentByName
	#define EXT_CMGetComponentByName
	#define GET_CMGetComponentByName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetComponentByName(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CMGetComponentByName  FALSE
	#define EXP_CMGetComponentByName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetComponentByName
	#define EXT_CMGetComponentByName
	#define GET_CMGetComponentByName(fl)  CAL_CMGETAPI( "CMGetComponentByName" ) 
	#define CAL_CMGetComponentByName  CMGetComponentByName
	#define CHK_CMGetComponentByName  TRUE
	#define EXP_CMGetComponentByName  CAL_CMEXPAPI( "CMGetComponentByName" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetComponentByName
	#define EXT_CMGetComponentByName
	#define GET_CMGetComponentByName(fl)  CAL_CMGETAPI( "CMGetComponentByName" ) 
	#define CAL_CMGetComponentByName  CMGetComponentByName
	#define CHK_CMGetComponentByName  TRUE
	#define EXP_CMGetComponentByName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetComponentByName", (RTS_UINTPTR)CMGetComponentByName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetComponentByName
	#define EXT_CMCMGetComponentByName
	#define GET_CMCMGetComponentByName  ERR_OK
	#define CAL_CMCMGetComponentByName pICM->ICMGetComponentByName
	#define CHK_CMCMGetComponentByName (pICM != NULL)
	#define EXP_CMCMGetComponentByName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetComponentByName
	#define EXT_CMGetComponentByName
	#define GET_CMGetComponentByName(fl)  CAL_CMGETAPI( "CMGetComponentByName" ) 
	#define CAL_CMGetComponentByName pICM->ICMGetComponentByName
	#define CHK_CMGetComponentByName (pICM != NULL)
	#define EXP_CMGetComponentByName  CAL_CMEXPAPI( "CMGetComponentByName" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetComponentByName  PFCMGETCOMPONENTBYNAME pfCMGetComponentByName;
	#define EXT_CMGetComponentByName  extern PFCMGETCOMPONENTBYNAME pfCMGetComponentByName;
	#define GET_CMGetComponentByName(fl)  s_pfCMGetAPI2( "CMGetComponentByName", (RTS_VOID_FCTPTR *)&pfCMGetComponentByName, (fl), 0, 0)
	#define CAL_CMGetComponentByName  pfCMGetComponentByName
	#define CHK_CMGetComponentByName  (pfCMGetComponentByName != NULL)
	#define EXP_CMGetComponentByName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetComponentByName", (RTS_UINTPTR)CMGetComponentByName, 0, 0) 
#endif




/**
 * <description> 
 *	Get the component id of a component specified by name.
 *	A component id always consists of the unique vendor id as high word and the specific component id
 *	as low word. So each component can be assigned to the specific vendor.
 * </description>
 * <param name="pszCmpName" type="IN">Name of the component</param>
 * <param name="pCmpId" type="OUT">Component id</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMGetCmpId(char *pszCmpName, CMPID *pCmpId);
typedef RTS_RESULT (CDECL * PFCMGETCMPID) (char *pszCmpName, CMPID *pCmpId);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETCMPID_NOTIMPLEMENTED)
	#define USE_CMGetCmpId
	#define EXT_CMGetCmpId
	#define GET_CMGetCmpId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetCmpId(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMGetCmpId  FALSE
	#define EXP_CMGetCmpId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetCmpId
	#define EXT_CMGetCmpId
	#define GET_CMGetCmpId(fl)  CAL_CMGETAPI( "CMGetCmpId" ) 
	#define CAL_CMGetCmpId  CMGetCmpId
	#define CHK_CMGetCmpId  TRUE
	#define EXP_CMGetCmpId  CAL_CMEXPAPI( "CMGetCmpId" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetCmpId
	#define EXT_CMGetCmpId
	#define GET_CMGetCmpId(fl)  CAL_CMGETAPI( "CMGetCmpId" ) 
	#define CAL_CMGetCmpId  CMGetCmpId
	#define CHK_CMGetCmpId  TRUE
	#define EXP_CMGetCmpId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetCmpId", (RTS_UINTPTR)CMGetCmpId, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetCmpId
	#define EXT_CMCMGetCmpId
	#define GET_CMCMGetCmpId  ERR_OK
	#define CAL_CMCMGetCmpId pICM->ICMGetCmpId
	#define CHK_CMCMGetCmpId (pICM != NULL)
	#define EXP_CMCMGetCmpId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetCmpId
	#define EXT_CMGetCmpId
	#define GET_CMGetCmpId(fl)  CAL_CMGETAPI( "CMGetCmpId" ) 
	#define CAL_CMGetCmpId pICM->ICMGetCmpId
	#define CHK_CMGetCmpId (pICM != NULL)
	#define EXP_CMGetCmpId  CAL_CMEXPAPI( "CMGetCmpId" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetCmpId  PFCMGETCMPID pfCMGetCmpId;
	#define EXT_CMGetCmpId  extern PFCMGETCMPID pfCMGetCmpId;
	#define GET_CMGetCmpId(fl)  s_pfCMGetAPI2( "CMGetCmpId", (RTS_VOID_FCTPTR *)&pfCMGetCmpId, (fl), 0, 0)
	#define CAL_CMGetCmpId  pfCMGetCmpId
	#define CHK_CMGetCmpId  (pfCMGetCmpId != NULL)
	#define EXP_CMGetCmpId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetCmpId", (RTS_UINTPTR)CMGetCmpId, 0, 0) 
#endif




/**
 * <description> 
 *	Get the component name of a component specified by the component id.
 * </description>
 * <param name="pCmpId" type="IN">Component id</param>
 * <param name="pszCmpName" type="OUT">Name of the component</param>
 * <param name="iMaxCmpName" type="IN">Max length of the component name buffer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMGetCmpName(CMPID CmpId, char *pszCmpName, int iMaxCmpName);
typedef RTS_RESULT (CDECL * PFCMGETCMPNAME) (CMPID CmpId, char *pszCmpName, int iMaxCmpName);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETCMPNAME_NOTIMPLEMENTED)
	#define USE_CMGetCmpName
	#define EXT_CMGetCmpName
	#define GET_CMGetCmpName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetCmpName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMGetCmpName  FALSE
	#define EXP_CMGetCmpName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetCmpName
	#define EXT_CMGetCmpName
	#define GET_CMGetCmpName(fl)  CAL_CMGETAPI( "CMGetCmpName" ) 
	#define CAL_CMGetCmpName  CMGetCmpName
	#define CHK_CMGetCmpName  TRUE
	#define EXP_CMGetCmpName  CAL_CMEXPAPI( "CMGetCmpName" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetCmpName
	#define EXT_CMGetCmpName
	#define GET_CMGetCmpName(fl)  CAL_CMGETAPI( "CMGetCmpName" ) 
	#define CAL_CMGetCmpName  CMGetCmpName
	#define CHK_CMGetCmpName  TRUE
	#define EXP_CMGetCmpName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetCmpName", (RTS_UINTPTR)CMGetCmpName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetCmpName
	#define EXT_CMCMGetCmpName
	#define GET_CMCMGetCmpName  ERR_OK
	#define CAL_CMCMGetCmpName pICM->ICMGetCmpName
	#define CHK_CMCMGetCmpName (pICM != NULL)
	#define EXP_CMCMGetCmpName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetCmpName
	#define EXT_CMGetCmpName
	#define GET_CMGetCmpName(fl)  CAL_CMGETAPI( "CMGetCmpName" ) 
	#define CAL_CMGetCmpName pICM->ICMGetCmpName
	#define CHK_CMGetCmpName (pICM != NULL)
	#define EXP_CMGetCmpName  CAL_CMEXPAPI( "CMGetCmpName" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetCmpName  PFCMGETCMPNAME pfCMGetCmpName;
	#define EXT_CMGetCmpName  extern PFCMGETCMPNAME pfCMGetCmpName;
	#define GET_CMGetCmpName(fl)  s_pfCMGetAPI2( "CMGetCmpName", (RTS_VOID_FCTPTR *)&pfCMGetCmpName, (fl), 0, 0)
	#define CAL_CMGetCmpName  pfCMGetCmpName
	#define CHK_CMGetCmpName  (pfCMGetCmpName != NULL)
	#define EXP_CMGetCmpName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetCmpName", (RTS_UINTPTR)CMGetCmpName, 0, 0) 
#endif




/**
 * <description> 
 *	Get the component description of a component specified by the component id.
 * </description>
 * <param name="pCmpId" type="IN">Component id</param>
 * <result>Pointer to COMPONENT_ENTRY</result>
 */
COMPONENT_ENTRY* CDECL CMGetComponent(CMPID CmpId);
typedef COMPONENT_ENTRY* (CDECL * PFCMGETCOMPONENT) (CMPID CmpId);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETCOMPONENT_NOTIMPLEMENTED)
	#define USE_CMGetComponent
	#define EXT_CMGetComponent
	#define GET_CMGetComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetComponent(p0)  (COMPONENT_ENTRY*)ERR_NOTIMPLEMENTED
	#define CHK_CMGetComponent  FALSE
	#define EXP_CMGetComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetComponent
	#define EXT_CMGetComponent
	#define GET_CMGetComponent(fl)  CAL_CMGETAPI( "CMGetComponent" ) 
	#define CAL_CMGetComponent  CMGetComponent
	#define CHK_CMGetComponent  TRUE
	#define EXP_CMGetComponent  CAL_CMEXPAPI( "CMGetComponent" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetComponent
	#define EXT_CMGetComponent
	#define GET_CMGetComponent(fl)  CAL_CMGETAPI( "CMGetComponent" ) 
	#define CAL_CMGetComponent  CMGetComponent
	#define CHK_CMGetComponent  TRUE
	#define EXP_CMGetComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetComponent", (RTS_UINTPTR)CMGetComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetComponent
	#define EXT_CMCMGetComponent
	#define GET_CMCMGetComponent  ERR_OK
	#define CAL_CMCMGetComponent pICM->ICMGetComponent
	#define CHK_CMCMGetComponent (pICM != NULL)
	#define EXP_CMCMGetComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetComponent
	#define EXT_CMGetComponent
	#define GET_CMGetComponent(fl)  CAL_CMGETAPI( "CMGetComponent" ) 
	#define CAL_CMGetComponent pICM->ICMGetComponent
	#define CHK_CMGetComponent (pICM != NULL)
	#define EXP_CMGetComponent  CAL_CMEXPAPI( "CMGetComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetComponent  PFCMGETCOMPONENT pfCMGetComponent;
	#define EXT_CMGetComponent  extern PFCMGETCOMPONENT pfCMGetComponent;
	#define GET_CMGetComponent(fl)  s_pfCMGetAPI2( "CMGetComponent", (RTS_VOID_FCTPTR *)&pfCMGetComponent, (fl), 0, 0)
	#define CAL_CMGetComponent  pfCMGetComponent
	#define CHK_CMGetComponent  (pfCMGetComponent != NULL)
	#define EXP_CMGetComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetComponent", (RTS_UINTPTR)CMGetComponent, 0, 0) 
#endif




/**
 * <description>
 *	Returns the number of registered components
 * </description>
 * <result>Number of components</result>
 */
int CDECL CMGetNumOfComponents(void);
typedef int (CDECL * PFCMGETNUMOFCOMPONENTS) (void);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETNUMOFCOMPONENTS_NOTIMPLEMENTED)
	#define USE_CMGetNumOfComponents
	#define EXT_CMGetNumOfComponents
	#define GET_CMGetNumOfComponents(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetNumOfComponents()  (int)ERR_NOTIMPLEMENTED
	#define CHK_CMGetNumOfComponents  FALSE
	#define EXP_CMGetNumOfComponents  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetNumOfComponents
	#define EXT_CMGetNumOfComponents
	#define GET_CMGetNumOfComponents(fl)  CAL_CMGETAPI( "CMGetNumOfComponents" ) 
	#define CAL_CMGetNumOfComponents  CMGetNumOfComponents
	#define CHK_CMGetNumOfComponents  TRUE
	#define EXP_CMGetNumOfComponents  CAL_CMEXPAPI( "CMGetNumOfComponents" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetNumOfComponents
	#define EXT_CMGetNumOfComponents
	#define GET_CMGetNumOfComponents(fl)  CAL_CMGETAPI( "CMGetNumOfComponents" ) 
	#define CAL_CMGetNumOfComponents  CMGetNumOfComponents
	#define CHK_CMGetNumOfComponents  TRUE
	#define EXP_CMGetNumOfComponents  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetNumOfComponents", (RTS_UINTPTR)CMGetNumOfComponents, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetNumOfComponents
	#define EXT_CMCMGetNumOfComponents
	#define GET_CMCMGetNumOfComponents  ERR_OK
	#define CAL_CMCMGetNumOfComponents pICM->ICMGetNumOfComponents
	#define CHK_CMCMGetNumOfComponents (pICM != NULL)
	#define EXP_CMCMGetNumOfComponents  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetNumOfComponents
	#define EXT_CMGetNumOfComponents
	#define GET_CMGetNumOfComponents(fl)  CAL_CMGETAPI( "CMGetNumOfComponents" ) 
	#define CAL_CMGetNumOfComponents pICM->ICMGetNumOfComponents
	#define CHK_CMGetNumOfComponents (pICM != NULL)
	#define EXP_CMGetNumOfComponents  CAL_CMEXPAPI( "CMGetNumOfComponents" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetNumOfComponents  PFCMGETNUMOFCOMPONENTS pfCMGetNumOfComponents;
	#define EXT_CMGetNumOfComponents  extern PFCMGETNUMOFCOMPONENTS pfCMGetNumOfComponents;
	#define GET_CMGetNumOfComponents(fl)  s_pfCMGetAPI2( "CMGetNumOfComponents", (RTS_VOID_FCTPTR *)&pfCMGetNumOfComponents, (fl), 0, 0)
	#define CAL_CMGetNumOfComponents  pfCMGetNumOfComponents
	#define CHK_CMGetNumOfComponents  (pfCMGetNumOfComponents != NULL)
	#define EXP_CMGetNumOfComponents  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetNumOfComponents", (RTS_UINTPTR)CMGetNumOfComponents, 0, 0) 
#endif




/**
 * <description>
 *	Returns the registered component specified by index
 * </description>
 * <param name="iIndex" type="IN">Index of the component in the list</param>
 * <result>Pointer to component or NULL if index ist out of range</result>
 */
COMPONENT_ENTRY* CDECL CMGetComponentByIndex(int iIndex);
typedef COMPONENT_ENTRY* (CDECL * PFCMGETCOMPONENTBYINDEX) (int iIndex);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETCOMPONENTBYINDEX_NOTIMPLEMENTED)
	#define USE_CMGetComponentByIndex
	#define EXT_CMGetComponentByIndex
	#define GET_CMGetComponentByIndex(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetComponentByIndex(p0)  (COMPONENT_ENTRY*)ERR_NOTIMPLEMENTED
	#define CHK_CMGetComponentByIndex  FALSE
	#define EXP_CMGetComponentByIndex  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetComponentByIndex
	#define EXT_CMGetComponentByIndex
	#define GET_CMGetComponentByIndex(fl)  CAL_CMGETAPI( "CMGetComponentByIndex" ) 
	#define CAL_CMGetComponentByIndex  CMGetComponentByIndex
	#define CHK_CMGetComponentByIndex  TRUE
	#define EXP_CMGetComponentByIndex  CAL_CMEXPAPI( "CMGetComponentByIndex" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetComponentByIndex
	#define EXT_CMGetComponentByIndex
	#define GET_CMGetComponentByIndex(fl)  CAL_CMGETAPI( "CMGetComponentByIndex" ) 
	#define CAL_CMGetComponentByIndex  CMGetComponentByIndex
	#define CHK_CMGetComponentByIndex  TRUE
	#define EXP_CMGetComponentByIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetComponentByIndex", (RTS_UINTPTR)CMGetComponentByIndex, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetComponentByIndex
	#define EXT_CMCMGetComponentByIndex
	#define GET_CMCMGetComponentByIndex  ERR_OK
	#define CAL_CMCMGetComponentByIndex pICM->ICMGetComponentByIndex
	#define CHK_CMCMGetComponentByIndex (pICM != NULL)
	#define EXP_CMCMGetComponentByIndex  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetComponentByIndex
	#define EXT_CMGetComponentByIndex
	#define GET_CMGetComponentByIndex(fl)  CAL_CMGETAPI( "CMGetComponentByIndex" ) 
	#define CAL_CMGetComponentByIndex pICM->ICMGetComponentByIndex
	#define CHK_CMGetComponentByIndex (pICM != NULL)
	#define EXP_CMGetComponentByIndex  CAL_CMEXPAPI( "CMGetComponentByIndex" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetComponentByIndex  PFCMGETCOMPONENTBYINDEX pfCMGetComponentByIndex;
	#define EXT_CMGetComponentByIndex  extern PFCMGETCOMPONENTBYINDEX pfCMGetComponentByIndex;
	#define GET_CMGetComponentByIndex(fl)  s_pfCMGetAPI2( "CMGetComponentByIndex", (RTS_VOID_FCTPTR *)&pfCMGetComponentByIndex, (fl), 0, 0)
	#define CAL_CMGetComponentByIndex  pfCMGetComponentByIndex
	#define CHK_CMGetComponentByIndex  (pfCMGetComponentByIndex != NULL)
	#define EXP_CMGetComponentByIndex  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetComponentByIndex", (RTS_UINTPTR)CMGetComponentByIndex, 0, 0) 
#endif




/**
 * <description>
 *	Returns the first component entry
 * </description>
 * <param name="pResult" type="INOUT">Pointer to error code</param>
 * <result>Pointer to component or NULL if no component available</result>
 */
COMPONENT_ENTRY* CDECL CMGetFirstComponent(RTS_RESULT *pResult);
typedef COMPONENT_ENTRY* (CDECL * PFCMGETFIRSTCOMPONENT) (RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETFIRSTCOMPONENT_NOTIMPLEMENTED)
	#define USE_CMGetFirstComponent
	#define EXT_CMGetFirstComponent
	#define GET_CMGetFirstComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetFirstComponent(p0)  (COMPONENT_ENTRY*)ERR_NOTIMPLEMENTED
	#define CHK_CMGetFirstComponent  FALSE
	#define EXP_CMGetFirstComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetFirstComponent
	#define EXT_CMGetFirstComponent
	#define GET_CMGetFirstComponent(fl)  CAL_CMGETAPI( "CMGetFirstComponent" ) 
	#define CAL_CMGetFirstComponent  CMGetFirstComponent
	#define CHK_CMGetFirstComponent  TRUE
	#define EXP_CMGetFirstComponent  CAL_CMEXPAPI( "CMGetFirstComponent" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetFirstComponent
	#define EXT_CMGetFirstComponent
	#define GET_CMGetFirstComponent(fl)  CAL_CMGETAPI( "CMGetFirstComponent" ) 
	#define CAL_CMGetFirstComponent  CMGetFirstComponent
	#define CHK_CMGetFirstComponent  TRUE
	#define EXP_CMGetFirstComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetFirstComponent", (RTS_UINTPTR)CMGetFirstComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetFirstComponent
	#define EXT_CMCMGetFirstComponent
	#define GET_CMCMGetFirstComponent  ERR_OK
	#define CAL_CMCMGetFirstComponent pICM->ICMGetFirstComponent
	#define CHK_CMCMGetFirstComponent (pICM != NULL)
	#define EXP_CMCMGetFirstComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetFirstComponent
	#define EXT_CMGetFirstComponent
	#define GET_CMGetFirstComponent(fl)  CAL_CMGETAPI( "CMGetFirstComponent" ) 
	#define CAL_CMGetFirstComponent pICM->ICMGetFirstComponent
	#define CHK_CMGetFirstComponent (pICM != NULL)
	#define EXP_CMGetFirstComponent  CAL_CMEXPAPI( "CMGetFirstComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetFirstComponent  PFCMGETFIRSTCOMPONENT pfCMGetFirstComponent;
	#define EXT_CMGetFirstComponent  extern PFCMGETFIRSTCOMPONENT pfCMGetFirstComponent;
	#define GET_CMGetFirstComponent(fl)  s_pfCMGetAPI2( "CMGetFirstComponent", (RTS_VOID_FCTPTR *)&pfCMGetFirstComponent, (fl), 0, 0)
	#define CAL_CMGetFirstComponent  pfCMGetFirstComponent
	#define CHK_CMGetFirstComponent  (pfCMGetFirstComponent != NULL)
	#define EXP_CMGetFirstComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetFirstComponent", (RTS_UINTPTR)CMGetFirstComponent, 0, 0) 
#endif




/**
 * <description>
 *	Returns the registered component specified by index
 * </description>
 * <param name="pCmp" type="IN">Pointer to the previous component</param>
 * <param name="pResult" type="INOUT">Pointer to error code</param>
 * <result>Pointer to component or NULL if end of list is reached</result>
 */
COMPONENT_ENTRY* CDECL CMGetNextComponent(COMPONENT_ENTRY *pCmp, RTS_RESULT *pResult);
typedef COMPONENT_ENTRY* (CDECL * PFCMGETNEXTCOMPONENT) (COMPONENT_ENTRY *pCmp, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETNEXTCOMPONENT_NOTIMPLEMENTED)
	#define USE_CMGetNextComponent
	#define EXT_CMGetNextComponent
	#define GET_CMGetNextComponent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetNextComponent(p0,p1)  (COMPONENT_ENTRY*)ERR_NOTIMPLEMENTED
	#define CHK_CMGetNextComponent  FALSE
	#define EXP_CMGetNextComponent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetNextComponent
	#define EXT_CMGetNextComponent
	#define GET_CMGetNextComponent(fl)  CAL_CMGETAPI( "CMGetNextComponent" ) 
	#define CAL_CMGetNextComponent  CMGetNextComponent
	#define CHK_CMGetNextComponent  TRUE
	#define EXP_CMGetNextComponent  CAL_CMEXPAPI( "CMGetNextComponent" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetNextComponent
	#define EXT_CMGetNextComponent
	#define GET_CMGetNextComponent(fl)  CAL_CMGETAPI( "CMGetNextComponent" ) 
	#define CAL_CMGetNextComponent  CMGetNextComponent
	#define CHK_CMGetNextComponent  TRUE
	#define EXP_CMGetNextComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetNextComponent", (RTS_UINTPTR)CMGetNextComponent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetNextComponent
	#define EXT_CMCMGetNextComponent
	#define GET_CMCMGetNextComponent  ERR_OK
	#define CAL_CMCMGetNextComponent pICM->ICMGetNextComponent
	#define CHK_CMCMGetNextComponent (pICM != NULL)
	#define EXP_CMCMGetNextComponent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetNextComponent
	#define EXT_CMGetNextComponent
	#define GET_CMGetNextComponent(fl)  CAL_CMGETAPI( "CMGetNextComponent" ) 
	#define CAL_CMGetNextComponent pICM->ICMGetNextComponent
	#define CHK_CMGetNextComponent (pICM != NULL)
	#define EXP_CMGetNextComponent  CAL_CMEXPAPI( "CMGetNextComponent" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetNextComponent  PFCMGETNEXTCOMPONENT pfCMGetNextComponent;
	#define EXT_CMGetNextComponent  extern PFCMGETNEXTCOMPONENT pfCMGetNextComponent;
	#define GET_CMGetNextComponent(fl)  s_pfCMGetAPI2( "CMGetNextComponent", (RTS_VOID_FCTPTR *)&pfCMGetNextComponent, (fl), 0, 0)
	#define CAL_CMGetNextComponent  pfCMGetNextComponent
	#define CHK_CMGetNextComponent  (pfCMGetNextComponent != NULL)
	#define EXP_CMGetNextComponent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetNextComponent", (RTS_UINTPTR)CMGetNextComponent, 0, 0) 
#endif




/**
 * <description>
 *	Create an instance of a specified class.
 * </description>
 * <param name="ClassId" type="IN">ClassId of the class to create an object</param>
 * <param name="pResult" type="OUT">Pointer to error code for result</param>
 * <result>Pointer to IBase interface of the object</result>
 */
IBase* CDECL CMCreateInstance(CLASSID ClassId, RTS_RESULT *pResult);
typedef IBase* (CDECL * PFCMCREATEINSTANCE) (CLASSID ClassId, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMCREATEINSTANCE_NOTIMPLEMENTED)
	#define USE_CMCreateInstance
	#define EXT_CMCreateInstance
	#define GET_CMCreateInstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMCreateInstance(p0,p1)  (IBase*)ERR_NOTIMPLEMENTED
	#define CHK_CMCreateInstance  FALSE
	#define EXP_CMCreateInstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMCreateInstance
	#define EXT_CMCreateInstance
	#define GET_CMCreateInstance(fl)  CAL_CMGETAPI( "CMCreateInstance" ) 
	#define CAL_CMCreateInstance  CMCreateInstance
	#define CHK_CMCreateInstance  TRUE
	#define EXP_CMCreateInstance  CAL_CMEXPAPI( "CMCreateInstance" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMCreateInstance
	#define EXT_CMCreateInstance
	#define GET_CMCreateInstance(fl)  CAL_CMGETAPI( "CMCreateInstance" ) 
	#define CAL_CMCreateInstance  CMCreateInstance
	#define CHK_CMCreateInstance  TRUE
	#define EXP_CMCreateInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCreateInstance", (RTS_UINTPTR)CMCreateInstance, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMCreateInstance
	#define EXT_CMCMCreateInstance
	#define GET_CMCMCreateInstance  ERR_OK
	#define CAL_CMCMCreateInstance pICM->ICMCreateInstance
	#define CHK_CMCMCreateInstance (pICM != NULL)
	#define EXP_CMCMCreateInstance  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMCreateInstance
	#define EXT_CMCreateInstance
	#define GET_CMCreateInstance(fl)  CAL_CMGETAPI( "CMCreateInstance" ) 
	#define CAL_CMCreateInstance pICM->ICMCreateInstance
	#define CHK_CMCreateInstance (pICM != NULL)
	#define EXP_CMCreateInstance  CAL_CMEXPAPI( "CMCreateInstance" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMCreateInstance  PFCMCREATEINSTANCE pfCMCreateInstance;
	#define EXT_CMCreateInstance  extern PFCMCREATEINSTANCE pfCMCreateInstance;
	#define GET_CMCreateInstance(fl)  s_pfCMGetAPI2( "CMCreateInstance", (RTS_VOID_FCTPTR *)&pfCMCreateInstance, (fl), 0, 0)
	#define CAL_CMCreateInstance  pfCMCreateInstance
	#define CHK_CMCreateInstance  (pfCMCreateInstance != NULL)
	#define EXP_CMCreateInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCreateInstance", (RTS_UINTPTR)CMCreateInstance, 0, 0) 
#endif




/**
 * <description>
 *	Delete an instance.
 * </description>
 * <param name="pIBase" type="IN">Pointer to IBase interface</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMDeleteInstance(IBase *pIBase);
typedef RTS_RESULT (CDECL * PFCMDELETEINSTANCE) (IBase *pIBase);
#if defined(CM_NOTIMPLEMENTED) || defined(CMDELETEINSTANCE_NOTIMPLEMENTED)
	#define USE_CMDeleteInstance
	#define EXT_CMDeleteInstance
	#define GET_CMDeleteInstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMDeleteInstance(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMDeleteInstance  FALSE
	#define EXP_CMDeleteInstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMDeleteInstance
	#define EXT_CMDeleteInstance
	#define GET_CMDeleteInstance(fl)  CAL_CMGETAPI( "CMDeleteInstance" ) 
	#define CAL_CMDeleteInstance  CMDeleteInstance
	#define CHK_CMDeleteInstance  TRUE
	#define EXP_CMDeleteInstance  CAL_CMEXPAPI( "CMDeleteInstance" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMDeleteInstance
	#define EXT_CMDeleteInstance
	#define GET_CMDeleteInstance(fl)  CAL_CMGETAPI( "CMDeleteInstance" ) 
	#define CAL_CMDeleteInstance  CMDeleteInstance
	#define CHK_CMDeleteInstance  TRUE
	#define EXP_CMDeleteInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMDeleteInstance", (RTS_UINTPTR)CMDeleteInstance, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMDeleteInstance
	#define EXT_CMCMDeleteInstance
	#define GET_CMCMDeleteInstance  ERR_OK
	#define CAL_CMCMDeleteInstance pICM->ICMDeleteInstance
	#define CHK_CMCMDeleteInstance (pICM != NULL)
	#define EXP_CMCMDeleteInstance  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMDeleteInstance
	#define EXT_CMDeleteInstance
	#define GET_CMDeleteInstance(fl)  CAL_CMGETAPI( "CMDeleteInstance" ) 
	#define CAL_CMDeleteInstance pICM->ICMDeleteInstance
	#define CHK_CMDeleteInstance (pICM != NULL)
	#define EXP_CMDeleteInstance  CAL_CMEXPAPI( "CMDeleteInstance" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMDeleteInstance  PFCMDELETEINSTANCE pfCMDeleteInstance;
	#define EXT_CMDeleteInstance  extern PFCMDELETEINSTANCE pfCMDeleteInstance;
	#define GET_CMDeleteInstance(fl)  s_pfCMGetAPI2( "CMDeleteInstance", (RTS_VOID_FCTPTR *)&pfCMDeleteInstance, (fl), 0, 0)
	#define CAL_CMDeleteInstance  pfCMDeleteInstance
	#define CHK_CMDeleteInstance  (pfCMDeleteInstance != NULL)
	#define EXP_CMDeleteInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMDeleteInstance", (RTS_UINTPTR)CMDeleteInstance, 0, 0) 
#endif




/**
 * <description>
 *	Delete an instance.
 * </description>
 * <param name="ClassId" type="IN">ClassId of the instance</param>
 * <param name="pIBase" type="IN">Pointer to IBase interface</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMDeleteInstance2(CLASSID ClassId, IBase *pIBase);
typedef RTS_RESULT (CDECL * PFCMDELETEINSTANCE2) (CLASSID ClassId, IBase *pIBase);
#if defined(CM_NOTIMPLEMENTED) || defined(CMDELETEINSTANCE2_NOTIMPLEMENTED)
	#define USE_CMDeleteInstance2
	#define EXT_CMDeleteInstance2
	#define GET_CMDeleteInstance2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMDeleteInstance2(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMDeleteInstance2  FALSE
	#define EXP_CMDeleteInstance2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMDeleteInstance2
	#define EXT_CMDeleteInstance2
	#define GET_CMDeleteInstance2(fl)  CAL_CMGETAPI( "CMDeleteInstance2" ) 
	#define CAL_CMDeleteInstance2  CMDeleteInstance2
	#define CHK_CMDeleteInstance2  TRUE
	#define EXP_CMDeleteInstance2  CAL_CMEXPAPI( "CMDeleteInstance2" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMDeleteInstance2
	#define EXT_CMDeleteInstance2
	#define GET_CMDeleteInstance2(fl)  CAL_CMGETAPI( "CMDeleteInstance2" ) 
	#define CAL_CMDeleteInstance2  CMDeleteInstance2
	#define CHK_CMDeleteInstance2  TRUE
	#define EXP_CMDeleteInstance2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMDeleteInstance2", (RTS_UINTPTR)CMDeleteInstance2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMDeleteInstance2
	#define EXT_CMCMDeleteInstance2
	#define GET_CMCMDeleteInstance2  ERR_OK
	#define CAL_CMCMDeleteInstance2 pICM->ICMDeleteInstance2
	#define CHK_CMCMDeleteInstance2 (pICM != NULL)
	#define EXP_CMCMDeleteInstance2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMDeleteInstance2
	#define EXT_CMDeleteInstance2
	#define GET_CMDeleteInstance2(fl)  CAL_CMGETAPI( "CMDeleteInstance2" ) 
	#define CAL_CMDeleteInstance2 pICM->ICMDeleteInstance2
	#define CHK_CMDeleteInstance2 (pICM != NULL)
	#define EXP_CMDeleteInstance2  CAL_CMEXPAPI( "CMDeleteInstance2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMDeleteInstance2  PFCMDELETEINSTANCE2 pfCMDeleteInstance2;
	#define EXT_CMDeleteInstance2  extern PFCMDELETEINSTANCE2 pfCMDeleteInstance2;
	#define GET_CMDeleteInstance2(fl)  s_pfCMGetAPI2( "CMDeleteInstance2", (RTS_VOID_FCTPTR *)&pfCMDeleteInstance2, (fl), 0, 0)
	#define CAL_CMDeleteInstance2  pfCMDeleteInstance2
	#define CHK_CMDeleteInstance2  (pfCMDeleteInstance2 != NULL)
	#define EXP_CMDeleteInstance2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMDeleteInstance2", (RTS_UINTPTR)CMDeleteInstance2, 0, 0) 
#endif




/**
 * <description>
 *	Get instance of a class.
 * </description>
 * <param name="ClassId" type="IN">ClassId of the class to create an object</param>
 * <param name="ObjId" type="IN">Index number of the instance</param>
 * <param name="pResult" type="OUT">Pointer to error code for result</param>
 * <result>Pointer to IBase interface of the object</result>
 */
IBase* CDECL CMGetInstance(CLASSID ClassId, OBJID ObjId, RTS_RESULT *pResult);
typedef IBase* (CDECL * PFCMGETINSTANCE) (CLASSID ClassId, OBJID ObjId, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETINSTANCE_NOTIMPLEMENTED)
	#define USE_CMGetInstance
	#define EXT_CMGetInstance
	#define GET_CMGetInstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetInstance(p0,p1,p2)  (IBase*)ERR_NOTIMPLEMENTED
	#define CHK_CMGetInstance  FALSE
	#define EXP_CMGetInstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetInstance
	#define EXT_CMGetInstance
	#define GET_CMGetInstance(fl)  CAL_CMGETAPI( "CMGetInstance" ) 
	#define CAL_CMGetInstance  CMGetInstance
	#define CHK_CMGetInstance  TRUE
	#define EXP_CMGetInstance  CAL_CMEXPAPI( "CMGetInstance" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetInstance
	#define EXT_CMGetInstance
	#define GET_CMGetInstance(fl)  CAL_CMGETAPI( "CMGetInstance" ) 
	#define CAL_CMGetInstance  CMGetInstance
	#define CHK_CMGetInstance  TRUE
	#define EXP_CMGetInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetInstance", (RTS_UINTPTR)CMGetInstance, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetInstance
	#define EXT_CMCMGetInstance
	#define GET_CMCMGetInstance  ERR_OK
	#define CAL_CMCMGetInstance pICM->ICMGetInstance
	#define CHK_CMCMGetInstance (pICM != NULL)
	#define EXP_CMCMGetInstance  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetInstance
	#define EXT_CMGetInstance
	#define GET_CMGetInstance(fl)  CAL_CMGETAPI( "CMGetInstance" ) 
	#define CAL_CMGetInstance pICM->ICMGetInstance
	#define CHK_CMGetInstance (pICM != NULL)
	#define EXP_CMGetInstance  CAL_CMEXPAPI( "CMGetInstance" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetInstance  PFCMGETINSTANCE pfCMGetInstance;
	#define EXT_CMGetInstance  extern PFCMGETINSTANCE pfCMGetInstance;
	#define GET_CMGetInstance(fl)  s_pfCMGetAPI2( "CMGetInstance", (RTS_VOID_FCTPTR *)&pfCMGetInstance, (fl), 0, 0)
	#define CAL_CMGetInstance  pfCMGetInstance
	#define CHK_CMGetInstance  (pfCMGetInstance != NULL)
	#define EXP_CMGetInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetInstance", (RTS_UINTPTR)CMGetInstance, 0, 0) 
#endif




/**
 * <description>
 *	Register an exisiting instance to the component manager. 
 * </description>
 * <param name="ClassId" type="IN">ClassId of the class to create an object</param>
 * <param name="ObjId" type="IN">Index number of the instance</param>
 * <param name="pResult" type="OUT">Pointer to error code for result</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMRegisterInstance(CLASSID ClassId, OBJID ObjId, IBase *pIBase);
typedef RTS_RESULT (CDECL * PFCMREGISTERINSTANCE) (CLASSID ClassId, OBJID ObjId, IBase *pIBase);
#if defined(CM_NOTIMPLEMENTED) || defined(CMREGISTERINSTANCE_NOTIMPLEMENTED)
	#define USE_CMRegisterInstance
	#define EXT_CMRegisterInstance
	#define GET_CMRegisterInstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMRegisterInstance(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMRegisterInstance  FALSE
	#define EXP_CMRegisterInstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMRegisterInstance
	#define EXT_CMRegisterInstance
	#define GET_CMRegisterInstance(fl)  CAL_CMGETAPI( "CMRegisterInstance" ) 
	#define CAL_CMRegisterInstance  CMRegisterInstance
	#define CHK_CMRegisterInstance  TRUE
	#define EXP_CMRegisterInstance  CAL_CMEXPAPI( "CMRegisterInstance" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMRegisterInstance
	#define EXT_CMRegisterInstance
	#define GET_CMRegisterInstance(fl)  CAL_CMGETAPI( "CMRegisterInstance" ) 
	#define CAL_CMRegisterInstance  CMRegisterInstance
	#define CHK_CMRegisterInstance  TRUE
	#define EXP_CMRegisterInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMRegisterInstance", (RTS_UINTPTR)CMRegisterInstance, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMRegisterInstance
	#define EXT_CMCMRegisterInstance
	#define GET_CMCMRegisterInstance  ERR_OK
	#define CAL_CMCMRegisterInstance pICM->ICMRegisterInstance
	#define CHK_CMCMRegisterInstance (pICM != NULL)
	#define EXP_CMCMRegisterInstance  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMRegisterInstance
	#define EXT_CMRegisterInstance
	#define GET_CMRegisterInstance(fl)  CAL_CMGETAPI( "CMRegisterInstance" ) 
	#define CAL_CMRegisterInstance pICM->ICMRegisterInstance
	#define CHK_CMRegisterInstance (pICM != NULL)
	#define EXP_CMRegisterInstance  CAL_CMEXPAPI( "CMRegisterInstance" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMRegisterInstance  PFCMREGISTERINSTANCE pfCMRegisterInstance;
	#define EXT_CMRegisterInstance  extern PFCMREGISTERINSTANCE pfCMRegisterInstance;
	#define GET_CMRegisterInstance(fl)  s_pfCMGetAPI2( "CMRegisterInstance", (RTS_VOID_FCTPTR *)&pfCMRegisterInstance, (fl), 0, 0)
	#define CAL_CMRegisterInstance  pfCMRegisterInstance
	#define CHK_CMRegisterInstance  (pfCMRegisterInstance != NULL)
	#define EXP_CMRegisterInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMRegisterInstance", (RTS_UINTPTR)CMRegisterInstance, 0, 0) 
#endif




/**
 * <description>
 *	Unregister an existing instance from component manager.
 * </description>
 * <param name="pIBase" type="IN">Pointer to IBase interface</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUnregisterInstance(IBase *pIBase);
typedef RTS_RESULT (CDECL * PFCMUNREGISTERINSTANCE) (IBase *pIBase);
#if defined(CM_NOTIMPLEMENTED) || defined(CMUNREGISTERINSTANCE_NOTIMPLEMENTED)
	#define USE_CMUnregisterInstance
	#define EXT_CMUnregisterInstance
	#define GET_CMUnregisterInstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUnregisterInstance(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUnregisterInstance  FALSE
	#define EXP_CMUnregisterInstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUnregisterInstance
	#define EXT_CMUnregisterInstance
	#define GET_CMUnregisterInstance(fl)  CAL_CMGETAPI( "CMUnregisterInstance" ) 
	#define CAL_CMUnregisterInstance  CMUnregisterInstance
	#define CHK_CMUnregisterInstance  TRUE
	#define EXP_CMUnregisterInstance  CAL_CMEXPAPI( "CMUnregisterInstance" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMUnregisterInstance
	#define EXT_CMUnregisterInstance
	#define GET_CMUnregisterInstance(fl)  CAL_CMGETAPI( "CMUnregisterInstance" ) 
	#define CAL_CMUnregisterInstance  CMUnregisterInstance
	#define CHK_CMUnregisterInstance  TRUE
	#define EXP_CMUnregisterInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUnregisterInstance", (RTS_UINTPTR)CMUnregisterInstance, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMUnregisterInstance
	#define EXT_CMCMUnregisterInstance
	#define GET_CMCMUnregisterInstance  ERR_OK
	#define CAL_CMCMUnregisterInstance pICM->ICMUnregisterInstance
	#define CHK_CMCMUnregisterInstance (pICM != NULL)
	#define EXP_CMCMUnregisterInstance  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUnregisterInstance
	#define EXT_CMUnregisterInstance
	#define GET_CMUnregisterInstance(fl)  CAL_CMGETAPI( "CMUnregisterInstance" ) 
	#define CAL_CMUnregisterInstance pICM->ICMUnregisterInstance
	#define CHK_CMUnregisterInstance (pICM != NULL)
	#define EXP_CMUnregisterInstance  CAL_CMEXPAPI( "CMUnregisterInstance" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUnregisterInstance  PFCMUNREGISTERINSTANCE pfCMUnregisterInstance;
	#define EXT_CMUnregisterInstance  extern PFCMUNREGISTERINSTANCE pfCMUnregisterInstance;
	#define GET_CMUnregisterInstance(fl)  s_pfCMGetAPI2( "CMUnregisterInstance", (RTS_VOID_FCTPTR *)&pfCMUnregisterInstance, (fl), 0, 0)
	#define CAL_CMUnregisterInstance  pfCMUnregisterInstance
	#define CHK_CMUnregisterInstance  (pfCMUnregisterInstance != NULL)
	#define EXP_CMUnregisterInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUnregisterInstance", (RTS_UINTPTR)CMUnregisterInstance, 0, 0) 
#endif




/**
 * <description>
 *	Get a list of instances, that implements the specified interface.
 * </description>
 * <param name="ItfId" type="IN">Id of the interface</param>
 * <param name="hIBasePool" type="INOUT">Must be a MemPool handle. All instances that implements the interface are
 * stored in this pool as result.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMGetInstanceList(ITFID ItfId, RTS_HANDLE hIBasePool);
typedef RTS_RESULT (CDECL * PFCMGETINSTANCELIST) (ITFID ItfId, RTS_HANDLE hIBasePool);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETINSTANCELIST_NOTIMPLEMENTED)
	#define USE_CMGetInstanceList
	#define EXT_CMGetInstanceList
	#define GET_CMGetInstanceList(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetInstanceList(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMGetInstanceList  FALSE
	#define EXP_CMGetInstanceList  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetInstanceList
	#define EXT_CMGetInstanceList
	#define GET_CMGetInstanceList(fl)  CAL_CMGETAPI( "CMGetInstanceList" ) 
	#define CAL_CMGetInstanceList  CMGetInstanceList
	#define CHK_CMGetInstanceList  TRUE
	#define EXP_CMGetInstanceList  CAL_CMEXPAPI( "CMGetInstanceList" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetInstanceList
	#define EXT_CMGetInstanceList
	#define GET_CMGetInstanceList(fl)  CAL_CMGETAPI( "CMGetInstanceList" ) 
	#define CAL_CMGetInstanceList  CMGetInstanceList
	#define CHK_CMGetInstanceList  TRUE
	#define EXP_CMGetInstanceList  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetInstanceList", (RTS_UINTPTR)CMGetInstanceList, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetInstanceList
	#define EXT_CMCMGetInstanceList
	#define GET_CMCMGetInstanceList  ERR_OK
	#define CAL_CMCMGetInstanceList pICM->ICMGetInstanceList
	#define CHK_CMCMGetInstanceList (pICM != NULL)
	#define EXP_CMCMGetInstanceList  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetInstanceList
	#define EXT_CMGetInstanceList
	#define GET_CMGetInstanceList(fl)  CAL_CMGETAPI( "CMGetInstanceList" ) 
	#define CAL_CMGetInstanceList pICM->ICMGetInstanceList
	#define CHK_CMGetInstanceList (pICM != NULL)
	#define EXP_CMGetInstanceList  CAL_CMEXPAPI( "CMGetInstanceList" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetInstanceList  PFCMGETINSTANCELIST pfCMGetInstanceList;
	#define EXT_CMGetInstanceList  extern PFCMGETINSTANCELIST pfCMGetInstanceList;
	#define GET_CMGetInstanceList(fl)  s_pfCMGetAPI2( "CMGetInstanceList", (RTS_VOID_FCTPTR *)&pfCMGetInstanceList, (fl), 0, 0)
	#define CAL_CMGetInstanceList  pfCMGetInstanceList
	#define CHK_CMGetInstanceList  (pfCMGetInstanceList != NULL)
	#define EXP_CMGetInstanceList  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetInstanceList", (RTS_UINTPTR)CMGetInstanceList, 0, 0) 
#endif




/**
 * <description> 
 *   Called to call all components with the specified hook
 * </description>
 * <param name="ulHook" type="IN">Hook (for definition, look into CmpItf.h)</param>
 * <param name="ulParam1" type="IN">First parameter. Hook dependant</param>
 * <param name="ulParam2" type="IN">Second parameter. Hook dependant</param>
 * <param name="bReverse" type="IN">Called the components in the opposite order as they were loaded</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMCallHook(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2, int bReverse);
typedef RTS_RESULT (CDECL * PFCMCALLHOOK) (RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2, int bReverse);
#if defined(CM_NOTIMPLEMENTED) || defined(CMCALLHOOK_NOTIMPLEMENTED)
	#define USE_CMCallHook
	#define EXT_CMCallHook
	#define GET_CMCallHook(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMCallHook(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMCallHook  FALSE
	#define EXP_CMCallHook  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMCallHook
	#define EXT_CMCallHook
	#define GET_CMCallHook(fl)  CAL_CMGETAPI( "CMCallHook" ) 
	#define CAL_CMCallHook  CMCallHook
	#define CHK_CMCallHook  TRUE
	#define EXP_CMCallHook  CAL_CMEXPAPI( "CMCallHook" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMCallHook
	#define EXT_CMCallHook
	#define GET_CMCallHook(fl)  CAL_CMGETAPI( "CMCallHook" ) 
	#define CAL_CMCallHook  CMCallHook
	#define CHK_CMCallHook  TRUE
	#define EXP_CMCallHook  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCallHook", (RTS_UINTPTR)CMCallHook, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMCallHook
	#define EXT_CMCMCallHook
	#define GET_CMCMCallHook  ERR_OK
	#define CAL_CMCMCallHook pICM->ICMCallHook
	#define CHK_CMCMCallHook (pICM != NULL)
	#define EXP_CMCMCallHook  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMCallHook
	#define EXT_CMCallHook
	#define GET_CMCallHook(fl)  CAL_CMGETAPI( "CMCallHook" ) 
	#define CAL_CMCallHook pICM->ICMCallHook
	#define CHK_CMCallHook (pICM != NULL)
	#define EXP_CMCallHook  CAL_CMEXPAPI( "CMCallHook" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMCallHook  PFCMCALLHOOK pfCMCallHook;
	#define EXT_CMCallHook  extern PFCMCALLHOOK pfCMCallHook;
	#define GET_CMCallHook(fl)  s_pfCMGetAPI2( "CMCallHook", (RTS_VOID_FCTPTR *)&pfCMCallHook, (fl), 0, 0)
	#define CAL_CMCallHook  pfCMCallHook
	#define CHK_CMCallHook  (pfCMCallHook != NULL)
	#define EXP_CMCallHook  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCallHook", (RTS_UINTPTR)CMCallHook, 0, 0) 
#endif




/**
 * <description> 
 *   Called to call all components with the specified type provided with the specified hook
 * </description>
 * <param name="usComponentTypeMask" type="IN" range="[CMPTYPE_ALL,CMPTYPE_SAFETY,CMPTYPE_STANDARD]">ComponentType mask. See corresponding category</param>
 * <param name="ulHook" type="IN" range="[CH_COMM_CYCLE,CMCALLHOOK_PSEUDOHOOK,RTS_UI32_MAX]">Hook (for definition, look into CmpItf.h)</param>
 * <param name="ulParam1" type="IN" range="[0,CMCALLHOOK_TEST1]">First parameter. Hook dependant</param>
 * <param name="ulParam2" type="IN" range="[0,CMCALLHOOK_TEST2]">Second parameter. Hook dependant</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Hookfunction successfully called</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">Component Pool not initialized or any Component responded with some error</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PENDING">Any Component responded with ERR_PENDING</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMCallHook2(RTS_UI16 usComponentTypeMask, RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);
typedef RTS_RESULT (CDECL * PFCMCALLHOOK2) (RTS_UI16 usComponentTypeMask, RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);
#if defined(CM_NOTIMPLEMENTED) || defined(CMCALLHOOK2_NOTIMPLEMENTED)
	#define USE_CMCallHook2
	#define EXT_CMCallHook2
	#define GET_CMCallHook2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMCallHook2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMCallHook2  FALSE
	#define EXP_CMCallHook2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMCallHook2
	#define EXT_CMCallHook2
	#define GET_CMCallHook2(fl)  CAL_CMGETAPI( "CMCallHook2" ) 
	#define CAL_CMCallHook2  CMCallHook2
	#define CHK_CMCallHook2  TRUE
	#define EXP_CMCallHook2  CAL_CMEXPAPI( "CMCallHook2" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMCallHook2
	#define EXT_CMCallHook2
	#define GET_CMCallHook2(fl)  CAL_CMGETAPI( "CMCallHook2" ) 
	#define CAL_CMCallHook2  CMCallHook2
	#define CHK_CMCallHook2  TRUE
	#define EXP_CMCallHook2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCallHook2", (RTS_UINTPTR)CMCallHook2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMCallHook2
	#define EXT_CMCMCallHook2
	#define GET_CMCMCallHook2  ERR_OK
	#define CAL_CMCMCallHook2 pICM->ICMCallHook2
	#define CHK_CMCMCallHook2 (pICM != NULL)
	#define EXP_CMCMCallHook2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMCallHook2
	#define EXT_CMCallHook2
	#define GET_CMCallHook2(fl)  CAL_CMGETAPI( "CMCallHook2" ) 
	#define CAL_CMCallHook2 pICM->ICMCallHook2
	#define CHK_CMCallHook2 (pICM != NULL)
	#define EXP_CMCallHook2  CAL_CMEXPAPI( "CMCallHook2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMCallHook2  PFCMCALLHOOK2 pfCMCallHook2;
	#define EXT_CMCallHook2  extern PFCMCALLHOOK2 pfCMCallHook2;
	#define GET_CMCallHook2(fl)  s_pfCMGetAPI2( "CMCallHook2", (RTS_VOID_FCTPTR *)&pfCMCallHook2, (fl), 0, 0)
	#define CAL_CMCallHook2  pfCMCallHook2
	#define CHK_CMCallHook2  (pfCMCallHook2 != NULL)
	#define EXP_CMCallHook2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCallHook2", (RTS_UINTPTR)CMCallHook2, 0, 0) 
#endif




/**
 * <description>Routine to check, if runtime runs in demo mode</description>
 * <result>1=Demo mode, 0=No demo</result>
 */
int CDECL CMIsDemo(void);
typedef int (CDECL * PFCMISDEMO) (void);
#if defined(CM_NOTIMPLEMENTED) || defined(CMISDEMO_NOTIMPLEMENTED)
	#define USE_CMIsDemo
	#define EXT_CMIsDemo
	#define GET_CMIsDemo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMIsDemo()  (int)ERR_NOTIMPLEMENTED
	#define CHK_CMIsDemo  FALSE
	#define EXP_CMIsDemo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMIsDemo
	#define EXT_CMIsDemo
	#define GET_CMIsDemo(fl)  CAL_CMGETAPI( "CMIsDemo" ) 
	#define CAL_CMIsDemo  CMIsDemo
	#define CHK_CMIsDemo  TRUE
	#define EXP_CMIsDemo  CAL_CMEXPAPI( "CMIsDemo" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMIsDemo
	#define EXT_CMIsDemo
	#define GET_CMIsDemo(fl)  CAL_CMGETAPI( "CMIsDemo" ) 
	#define CAL_CMIsDemo  CMIsDemo
	#define CHK_CMIsDemo  TRUE
	#define EXP_CMIsDemo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMIsDemo", (RTS_UINTPTR)CMIsDemo, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMIsDemo
	#define EXT_CMCMIsDemo
	#define GET_CMCMIsDemo  ERR_OK
	#define CAL_CMCMIsDemo pICM->ICMIsDemo
	#define CHK_CMCMIsDemo (pICM != NULL)
	#define EXP_CMCMIsDemo  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMIsDemo
	#define EXT_CMIsDemo
	#define GET_CMIsDemo(fl)  CAL_CMGETAPI( "CMIsDemo" ) 
	#define CAL_CMIsDemo pICM->ICMIsDemo
	#define CHK_CMIsDemo (pICM != NULL)
	#define EXP_CMIsDemo  CAL_CMEXPAPI( "CMIsDemo" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMIsDemo  PFCMISDEMO pfCMIsDemo;
	#define EXT_CMIsDemo  extern PFCMISDEMO pfCMIsDemo;
	#define GET_CMIsDemo(fl)  s_pfCMGetAPI2( "CMIsDemo", (RTS_VOID_FCTPTR *)&pfCMIsDemo, (fl), 0, 0)
	#define CAL_CMIsDemo  pfCMIsDemo
	#define CHK_CMIsDemo  (pfCMIsDemo != NULL)
	#define EXP_CMIsDemo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMIsDemo", (RTS_UINTPTR)CMIsDemo, 0, 0) 
#endif




/**
 * <description>Routine to complete the component manager initialization.
 *	Contains calling the hooks from CH_INIT2 to CH_INIT_COMM.</description>
 * <result>
 *  <ul>
 *   <li>ERR_OK</li>
 *   <li>ERR_DUPLICATE: If init end is still done, this error message will be returned</li>
 *  </ul>
 * </result>
 */
RTS_RESULT CDECL CMInitEnd(void);
typedef RTS_RESULT (CDECL * PFCMINITEND) (void);
#if defined(CM_NOTIMPLEMENTED) || defined(CMINITEND_NOTIMPLEMENTED)
	#define USE_CMInitEnd
	#define EXT_CMInitEnd
	#define GET_CMInitEnd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMInitEnd()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMInitEnd  FALSE
	#define EXP_CMInitEnd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMInitEnd
	#define EXT_CMInitEnd
	#define GET_CMInitEnd(fl)  CAL_CMGETAPI( "CMInitEnd" ) 
	#define CAL_CMInitEnd  CMInitEnd
	#define CHK_CMInitEnd  TRUE
	#define EXP_CMInitEnd  CAL_CMEXPAPI( "CMInitEnd" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMInitEnd
	#define EXT_CMInitEnd
	#define GET_CMInitEnd(fl)  CAL_CMGETAPI( "CMInitEnd" ) 
	#define CAL_CMInitEnd  CMInitEnd
	#define CHK_CMInitEnd  TRUE
	#define EXP_CMInitEnd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInitEnd", (RTS_UINTPTR)CMInitEnd, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMInitEnd
	#define EXT_CMCMInitEnd
	#define GET_CMCMInitEnd  ERR_OK
	#define CAL_CMCMInitEnd pICM->ICMInitEnd
	#define CHK_CMCMInitEnd (pICM != NULL)
	#define EXP_CMCMInitEnd  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMInitEnd
	#define EXT_CMInitEnd
	#define GET_CMInitEnd(fl)  CAL_CMGETAPI( "CMInitEnd" ) 
	#define CAL_CMInitEnd pICM->ICMInitEnd
	#define CHK_CMInitEnd (pICM != NULL)
	#define EXP_CMInitEnd  CAL_CMEXPAPI( "CMInitEnd" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMInitEnd  PFCMINITEND pfCMInitEnd;
	#define EXT_CMInitEnd  extern PFCMINITEND pfCMInitEnd;
	#define GET_CMInitEnd(fl)  s_pfCMGetAPI2( "CMInitEnd", (RTS_VOID_FCTPTR *)&pfCMInitEnd, (fl), 0, 0)
	#define CAL_CMInitEnd  pfCMInitEnd
	#define CHK_CMInitEnd  (pfCMInitEnd != NULL)
	#define EXP_CMInitEnd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMInitEnd", (RTS_UINTPTR)CMInitEnd, 0, 0) 
#endif




/**
 * <description>
 *	Functtion checks, if the SysTarget component is originally signed and was not modified.
 * </description>
 * <result>error code: ERR_OK: SysTarget is signed, ERR_FAILED: SysTarget was modified or is unsigned</result>
 */
RTS_RESULT CDECL CMCheckSysTargetSignature(void);
typedef RTS_RESULT (CDECL * PFCMCHECKSYSTARGETSIGNATURE) (void);
#if defined(CM_NOTIMPLEMENTED) || defined(CMCHECKSYSTARGETSIGNATURE_NOTIMPLEMENTED)
	#define USE_CMCheckSysTargetSignature
	#define EXT_CMCheckSysTargetSignature
	#define GET_CMCheckSysTargetSignature(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMCheckSysTargetSignature()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMCheckSysTargetSignature  FALSE
	#define EXP_CMCheckSysTargetSignature  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMCheckSysTargetSignature
	#define EXT_CMCheckSysTargetSignature
	#define GET_CMCheckSysTargetSignature(fl)  CAL_CMGETAPI( "CMCheckSysTargetSignature" ) 
	#define CAL_CMCheckSysTargetSignature  CMCheckSysTargetSignature
	#define CHK_CMCheckSysTargetSignature  TRUE
	#define EXP_CMCheckSysTargetSignature  CAL_CMEXPAPI( "CMCheckSysTargetSignature" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMCheckSysTargetSignature
	#define EXT_CMCheckSysTargetSignature
	#define GET_CMCheckSysTargetSignature(fl)  CAL_CMGETAPI( "CMCheckSysTargetSignature" ) 
	#define CAL_CMCheckSysTargetSignature  CMCheckSysTargetSignature
	#define CHK_CMCheckSysTargetSignature  TRUE
	#define EXP_CMCheckSysTargetSignature  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCheckSysTargetSignature", (RTS_UINTPTR)CMCheckSysTargetSignature, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMCheckSysTargetSignature
	#define EXT_CMCMCheckSysTargetSignature
	#define GET_CMCMCheckSysTargetSignature  ERR_OK
	#define CAL_CMCMCheckSysTargetSignature pICM->ICMCheckSysTargetSignature
	#define CHK_CMCMCheckSysTargetSignature (pICM != NULL)
	#define EXP_CMCMCheckSysTargetSignature  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMCheckSysTargetSignature
	#define EXT_CMCheckSysTargetSignature
	#define GET_CMCheckSysTargetSignature(fl)  CAL_CMGETAPI( "CMCheckSysTargetSignature" ) 
	#define CAL_CMCheckSysTargetSignature pICM->ICMCheckSysTargetSignature
	#define CHK_CMCheckSysTargetSignature (pICM != NULL)
	#define EXP_CMCheckSysTargetSignature  CAL_CMEXPAPI( "CMCheckSysTargetSignature" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMCheckSysTargetSignature  PFCMCHECKSYSTARGETSIGNATURE pfCMCheckSysTargetSignature;
	#define EXT_CMCheckSysTargetSignature  extern PFCMCHECKSYSTARGETSIGNATURE pfCMCheckSysTargetSignature;
	#define GET_CMCheckSysTargetSignature(fl)  s_pfCMGetAPI2( "CMCheckSysTargetSignature", (RTS_VOID_FCTPTR *)&pfCMCheckSysTargetSignature, (fl), 0, 0)
	#define CAL_CMCheckSysTargetSignature  pfCMCheckSysTargetSignature
	#define CHK_CMCheckSysTargetSignature  (pfCMCheckSysTargetSignature != NULL)
	#define EXP_CMCheckSysTargetSignature  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCheckSysTargetSignature", (RTS_UINTPTR)CMCheckSysTargetSignature, 0, 0) 
#endif




#define LICENSE_NOT_AVAILABLE	UINT32_C(0xffffffff)
#define LICENSE_INVALID			UINT32_C(0x00000000)

/**
 * <description>Function to check the license of a feature</description>
 * <param name="ulLicenseID" type="IN">HIGHWORD: VendorID, LOWWORD: FeatureID</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>LICENSE_INVALID, LICENSE_NOT_AVAILABLE (Demo mode) or specific license code</result>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Specific license code or LICENSE_NOT_AVAILABLE (Demo mode)</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">Not licensed</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOT_SUPPORTED">License library is not registered</errorcode>
 */
RTS_UI32 CDECL CMGetUserLicenseValue(RTS_UI32 ulLicenseID, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFCMGETUSERLICENSEVALUE) (RTS_UI32 ulLicenseID, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETUSERLICENSEVALUE_NOTIMPLEMENTED)
	#define USE_CMGetUserLicenseValue
	#define EXT_CMGetUserLicenseValue
	#define GET_CMGetUserLicenseValue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetUserLicenseValue(p0,p1)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_CMGetUserLicenseValue  FALSE
	#define EXP_CMGetUserLicenseValue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetUserLicenseValue
	#define EXT_CMGetUserLicenseValue
	#define GET_CMGetUserLicenseValue(fl)  CAL_CMGETAPI( "CMGetUserLicenseValue" ) 
	#define CAL_CMGetUserLicenseValue  CMGetUserLicenseValue
	#define CHK_CMGetUserLicenseValue  TRUE
	#define EXP_CMGetUserLicenseValue  CAL_CMEXPAPI( "CMGetUserLicenseValue" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetUserLicenseValue
	#define EXT_CMGetUserLicenseValue
	#define GET_CMGetUserLicenseValue(fl)  CAL_CMGETAPI( "CMGetUserLicenseValue" ) 
	#define CAL_CMGetUserLicenseValue  CMGetUserLicenseValue
	#define CHK_CMGetUserLicenseValue  TRUE
	#define EXP_CMGetUserLicenseValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetUserLicenseValue", (RTS_UINTPTR)CMGetUserLicenseValue, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetUserLicenseValue
	#define EXT_CMCMGetUserLicenseValue
	#define GET_CMCMGetUserLicenseValue  ERR_OK
	#define CAL_CMCMGetUserLicenseValue pICM->ICMGetUserLicenseValue
	#define CHK_CMCMGetUserLicenseValue (pICM != NULL)
	#define EXP_CMCMGetUserLicenseValue  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetUserLicenseValue
	#define EXT_CMGetUserLicenseValue
	#define GET_CMGetUserLicenseValue(fl)  CAL_CMGETAPI( "CMGetUserLicenseValue" ) 
	#define CAL_CMGetUserLicenseValue pICM->ICMGetUserLicenseValue
	#define CHK_CMGetUserLicenseValue (pICM != NULL)
	#define EXP_CMGetUserLicenseValue  CAL_CMEXPAPI( "CMGetUserLicenseValue" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetUserLicenseValue  PFCMGETUSERLICENSEVALUE pfCMGetUserLicenseValue;
	#define EXT_CMGetUserLicenseValue  extern PFCMGETUSERLICENSEVALUE pfCMGetUserLicenseValue;
	#define GET_CMGetUserLicenseValue(fl)  s_pfCMGetAPI2( "CMGetUserLicenseValue", (RTS_VOID_FCTPTR *)&pfCMGetUserLicenseValue, (fl), 0, 0)
	#define CAL_CMGetUserLicenseValue  pfCMGetUserLicenseValue
	#define CHK_CMGetUserLicenseValue  (pfCMGetUserLicenseValue != NULL)
	#define EXP_CMGetUserLicenseValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetUserLicenseValue", (RTS_UINTPTR)CMGetUserLicenseValue, 0, 0) 
#endif




/**
 * <description></description>
 * <param name="ulLicenseID" type="IN">HIGHWORD: VendorID, LOWWORD: FeatureID</param>
 * <param name="ulNewLicenseValue" type="IN"></param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result></result>
 */
RTS_UI32 CDECL CMReqDynLicChallenge(RTS_UI32 ulLicenseID, RTS_UI32 ulNewLicenseValue, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFCMREQDYNLICCHALLENGE) (RTS_UI32 ulLicenseID, RTS_UI32 ulNewLicenseValue, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMREQDYNLICCHALLENGE_NOTIMPLEMENTED)
	#define USE_CMReqDynLicChallenge
	#define EXT_CMReqDynLicChallenge
	#define GET_CMReqDynLicChallenge(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMReqDynLicChallenge(p0,p1,p2)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_CMReqDynLicChallenge  FALSE
	#define EXP_CMReqDynLicChallenge  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMReqDynLicChallenge
	#define EXT_CMReqDynLicChallenge
	#define GET_CMReqDynLicChallenge(fl)  CAL_CMGETAPI( "CMReqDynLicChallenge" ) 
	#define CAL_CMReqDynLicChallenge  CMReqDynLicChallenge
	#define CHK_CMReqDynLicChallenge  TRUE
	#define EXP_CMReqDynLicChallenge  CAL_CMEXPAPI( "CMReqDynLicChallenge" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMReqDynLicChallenge
	#define EXT_CMReqDynLicChallenge
	#define GET_CMReqDynLicChallenge(fl)  CAL_CMGETAPI( "CMReqDynLicChallenge" ) 
	#define CAL_CMReqDynLicChallenge  CMReqDynLicChallenge
	#define CHK_CMReqDynLicChallenge  TRUE
	#define EXP_CMReqDynLicChallenge  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMReqDynLicChallenge", (RTS_UINTPTR)CMReqDynLicChallenge, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMReqDynLicChallenge
	#define EXT_CMCMReqDynLicChallenge
	#define GET_CMCMReqDynLicChallenge  ERR_OK
	#define CAL_CMCMReqDynLicChallenge pICM->ICMReqDynLicChallenge
	#define CHK_CMCMReqDynLicChallenge (pICM != NULL)
	#define EXP_CMCMReqDynLicChallenge  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMReqDynLicChallenge
	#define EXT_CMReqDynLicChallenge
	#define GET_CMReqDynLicChallenge(fl)  CAL_CMGETAPI( "CMReqDynLicChallenge" ) 
	#define CAL_CMReqDynLicChallenge pICM->ICMReqDynLicChallenge
	#define CHK_CMReqDynLicChallenge (pICM != NULL)
	#define EXP_CMReqDynLicChallenge  CAL_CMEXPAPI( "CMReqDynLicChallenge" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMReqDynLicChallenge  PFCMREQDYNLICCHALLENGE pfCMReqDynLicChallenge;
	#define EXT_CMReqDynLicChallenge  extern PFCMREQDYNLICCHALLENGE pfCMReqDynLicChallenge;
	#define GET_CMReqDynLicChallenge(fl)  s_pfCMGetAPI2( "CMReqDynLicChallenge", (RTS_VOID_FCTPTR *)&pfCMReqDynLicChallenge, (fl), 0, 0)
	#define CAL_CMReqDynLicChallenge  pfCMReqDynLicChallenge
	#define CHK_CMReqDynLicChallenge  (pfCMReqDynLicChallenge != NULL)
	#define EXP_CMReqDynLicChallenge  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMReqDynLicChallenge", (RTS_UINTPTR)CMReqDynLicChallenge, 0, 0) 
#endif




/**
 * <description></description>
 * <param name="ulLicenseID" type="IN">HIGHWORD: VendorID, LOWWORD: FeatureID</param>
 * <param name="ulNewLicenseValue" type="IN"></param>
 * <param name="ulChallenge" type="IN"></param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result></result>
 */
int CDECL CMConfDynLicChallenge(RTS_UI32 ulLicenseID, RTS_UI32 ulNewLicenseValue, RTS_UI32 ulChallenge, RTS_RESULT *pResult);
typedef int (CDECL * PFCMCONFDYNLICCHALLENGE) (RTS_UI32 ulLicenseID, RTS_UI32 ulNewLicenseValue, RTS_UI32 ulChallenge, RTS_RESULT *pResult);
#if defined(CM_NOTIMPLEMENTED) || defined(CMCONFDYNLICCHALLENGE_NOTIMPLEMENTED)
	#define USE_CMConfDynLicChallenge
	#define EXT_CMConfDynLicChallenge
	#define GET_CMConfDynLicChallenge(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMConfDynLicChallenge(p0,p1,p2,p3)  (int)ERR_NOTIMPLEMENTED
	#define CHK_CMConfDynLicChallenge  FALSE
	#define EXP_CMConfDynLicChallenge  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMConfDynLicChallenge
	#define EXT_CMConfDynLicChallenge
	#define GET_CMConfDynLicChallenge(fl)  CAL_CMGETAPI( "CMConfDynLicChallenge" ) 
	#define CAL_CMConfDynLicChallenge  CMConfDynLicChallenge
	#define CHK_CMConfDynLicChallenge  TRUE
	#define EXP_CMConfDynLicChallenge  CAL_CMEXPAPI( "CMConfDynLicChallenge" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMConfDynLicChallenge
	#define EXT_CMConfDynLicChallenge
	#define GET_CMConfDynLicChallenge(fl)  CAL_CMGETAPI( "CMConfDynLicChallenge" ) 
	#define CAL_CMConfDynLicChallenge  CMConfDynLicChallenge
	#define CHK_CMConfDynLicChallenge  TRUE
	#define EXP_CMConfDynLicChallenge  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMConfDynLicChallenge", (RTS_UINTPTR)CMConfDynLicChallenge, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMConfDynLicChallenge
	#define EXT_CMCMConfDynLicChallenge
	#define GET_CMCMConfDynLicChallenge  ERR_OK
	#define CAL_CMCMConfDynLicChallenge pICM->ICMConfDynLicChallenge
	#define CHK_CMCMConfDynLicChallenge (pICM != NULL)
	#define EXP_CMCMConfDynLicChallenge  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMConfDynLicChallenge
	#define EXT_CMConfDynLicChallenge
	#define GET_CMConfDynLicChallenge(fl)  CAL_CMGETAPI( "CMConfDynLicChallenge" ) 
	#define CAL_CMConfDynLicChallenge pICM->ICMConfDynLicChallenge
	#define CHK_CMConfDynLicChallenge (pICM != NULL)
	#define EXP_CMConfDynLicChallenge  CAL_CMEXPAPI( "CMConfDynLicChallenge" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMConfDynLicChallenge  PFCMCONFDYNLICCHALLENGE pfCMConfDynLicChallenge;
	#define EXT_CMConfDynLicChallenge  extern PFCMCONFDYNLICCHALLENGE pfCMConfDynLicChallenge;
	#define GET_CMConfDynLicChallenge(fl)  s_pfCMGetAPI2( "CMConfDynLicChallenge", (RTS_VOID_FCTPTR *)&pfCMConfDynLicChallenge, (fl), 0, 0)
	#define CAL_CMConfDynLicChallenge  pfCMConfDynLicChallenge
	#define CHK_CMConfDynLicChallenge  (pfCMConfDynLicChallenge != NULL)
	#define EXP_CMConfDynLicChallenge  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMConfDynLicChallenge", (RTS_UINTPTR)CMConfDynLicChallenge, 0, 0) 
#endif




/**
 * <description> 
 *   Function to call the CommCycleHook of all components from outside the CM, if neccessary. This function is 
 *   intended to be called on singletasking systems (with CmpScheduleTimer or CmpScheduleEmbedded) during long
 *   lasting operations. For example SysFlash calls this function to keep the communication alive during writing
 *   long memory areas. 
 *   Before calling the CommCycleHook, the function internally checks the following conditions:
 *   - Is SysTask implemented?
 *   - Is the less then the configured time since the last call of the CommCycleHook elapsed?
 *   If one of this conditions is true, the hook is not called. This allows to use this function without doing
 *   these checks locally in the caller. 
 *   On multitasking systems this function has no effect. Use AsyncServices instead.  
 * </description>
 * <param name="ulParam1" type="IN">Type of the COMM_CYCLE_HOOK. See CM_HOOK_TYPE... types.</param>
 * <param name="ulParam2" type="IN">Second parameter. Hook dependant, typically 0</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMCallExtraCommCycleHook(RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);
typedef RTS_RESULT (CDECL * PFCMCALLEXTRACOMMCYCLEHOOK) (RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);
#if defined(CM_NOTIMPLEMENTED) || defined(CMCALLEXTRACOMMCYCLEHOOK_NOTIMPLEMENTED)
	#define USE_CMCallExtraCommCycleHook
	#define EXT_CMCallExtraCommCycleHook
	#define GET_CMCallExtraCommCycleHook(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMCallExtraCommCycleHook(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMCallExtraCommCycleHook  FALSE
	#define EXP_CMCallExtraCommCycleHook  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMCallExtraCommCycleHook
	#define EXT_CMCallExtraCommCycleHook
	#define GET_CMCallExtraCommCycleHook(fl)  CAL_CMGETAPI( "CMCallExtraCommCycleHook" ) 
	#define CAL_CMCallExtraCommCycleHook  CMCallExtraCommCycleHook
	#define CHK_CMCallExtraCommCycleHook  TRUE
	#define EXP_CMCallExtraCommCycleHook  CAL_CMEXPAPI( "CMCallExtraCommCycleHook" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMCallExtraCommCycleHook
	#define EXT_CMCallExtraCommCycleHook
	#define GET_CMCallExtraCommCycleHook(fl)  CAL_CMGETAPI( "CMCallExtraCommCycleHook" ) 
	#define CAL_CMCallExtraCommCycleHook  CMCallExtraCommCycleHook
	#define CHK_CMCallExtraCommCycleHook  TRUE
	#define EXP_CMCallExtraCommCycleHook  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCallExtraCommCycleHook", (RTS_UINTPTR)CMCallExtraCommCycleHook, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMCallExtraCommCycleHook
	#define EXT_CMCMCallExtraCommCycleHook
	#define GET_CMCMCallExtraCommCycleHook  ERR_OK
	#define CAL_CMCMCallExtraCommCycleHook pICM->ICMCallExtraCommCycleHook
	#define CHK_CMCMCallExtraCommCycleHook (pICM != NULL)
	#define EXP_CMCMCallExtraCommCycleHook  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMCallExtraCommCycleHook
	#define EXT_CMCallExtraCommCycleHook
	#define GET_CMCallExtraCommCycleHook(fl)  CAL_CMGETAPI( "CMCallExtraCommCycleHook" ) 
	#define CAL_CMCallExtraCommCycleHook pICM->ICMCallExtraCommCycleHook
	#define CHK_CMCallExtraCommCycleHook (pICM != NULL)
	#define EXP_CMCallExtraCommCycleHook  CAL_CMEXPAPI( "CMCallExtraCommCycleHook" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMCallExtraCommCycleHook  PFCMCALLEXTRACOMMCYCLEHOOK pfCMCallExtraCommCycleHook;
	#define EXT_CMCallExtraCommCycleHook  extern PFCMCALLEXTRACOMMCYCLEHOOK pfCMCallExtraCommCycleHook;
	#define GET_CMCallExtraCommCycleHook(fl)  s_pfCMGetAPI2( "CMCallExtraCommCycleHook", (RTS_VOID_FCTPTR *)&pfCMCallExtraCommCycleHook, (fl), 0, 0)
	#define CAL_CMCallExtraCommCycleHook  pfCMCallExtraCommCycleHook
	#define CHK_CMCallExtraCommCycleHook  (pfCMCallExtraCommCycleHook != NULL)
	#define EXP_CMCallExtraCommCycleHook  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMCallExtraCommCycleHook", (RTS_UINTPTR)CMCallExtraCommCycleHook, 0, 0) 
#endif




/**
 * <description> 
 *   Function returns the current hook function, that is called by the component manager
 * </description>
 * <result>Current hook</result>
 */
RTS_UI32 CDECL CMGetCurrentHook(void);
typedef RTS_UI32 (CDECL * PFCMGETCURRENTHOOK) (void);
#if defined(CM_NOTIMPLEMENTED) || defined(CMGETCURRENTHOOK_NOTIMPLEMENTED)
	#define USE_CMGetCurrentHook
	#define EXT_CMGetCurrentHook
	#define GET_CMGetCurrentHook(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMGetCurrentHook()  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_CMGetCurrentHook  FALSE
	#define EXP_CMGetCurrentHook  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMGetCurrentHook
	#define EXT_CMGetCurrentHook
	#define GET_CMGetCurrentHook(fl)  CAL_CMGETAPI( "CMGetCurrentHook" ) 
	#define CAL_CMGetCurrentHook  CMGetCurrentHook
	#define CHK_CMGetCurrentHook  TRUE
	#define EXP_CMGetCurrentHook  CAL_CMEXPAPI( "CMGetCurrentHook" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMGetCurrentHook
	#define EXT_CMGetCurrentHook
	#define GET_CMGetCurrentHook(fl)  CAL_CMGETAPI( "CMGetCurrentHook" ) 
	#define CAL_CMGetCurrentHook  CMGetCurrentHook
	#define CHK_CMGetCurrentHook  TRUE
	#define EXP_CMGetCurrentHook  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetCurrentHook", (RTS_UINTPTR)CMGetCurrentHook, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMGetCurrentHook
	#define EXT_CMCMGetCurrentHook
	#define GET_CMCMGetCurrentHook  ERR_OK
	#define CAL_CMCMGetCurrentHook pICM->ICMGetCurrentHook
	#define CHK_CMCMGetCurrentHook (pICM != NULL)
	#define EXP_CMCMGetCurrentHook  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMGetCurrentHook
	#define EXT_CMGetCurrentHook
	#define GET_CMGetCurrentHook(fl)  CAL_CMGETAPI( "CMGetCurrentHook" ) 
	#define CAL_CMGetCurrentHook pICM->ICMGetCurrentHook
	#define CHK_CMGetCurrentHook (pICM != NULL)
	#define EXP_CMGetCurrentHook  CAL_CMEXPAPI( "CMGetCurrentHook" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMGetCurrentHook  PFCMGETCURRENTHOOK pfCMGetCurrentHook;
	#define EXT_CMGetCurrentHook  extern PFCMGETCURRENTHOOK pfCMGetCurrentHook;
	#define GET_CMGetCurrentHook(fl)  s_pfCMGetAPI2( "CMGetCurrentHook", (RTS_VOID_FCTPTR *)&pfCMGetCurrentHook, (fl), 0, 0)
	#define CAL_CMGetCurrentHook  pfCMGetCurrentHook
	#define CHK_CMGetCurrentHook  (pfCMGetCurrentHook != NULL)
	#define EXP_CMGetCurrentHook  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMGetCurrentHook", (RTS_UINTPTR)CMGetCurrentHook, 0, 0) 
#endif




/**
 * <description> 
 *  Function can be called from each component to disable single operations. If this function is called, the event EVT_CmpMgr_DisableOperation is
 *	sent to disable this operation from any component or IEC program.
 * </description>
 * <element name="cmpId" type="IN">ComponentID</element>
 * <element name="ulOperation" type="IN">Operation that can be disabled. See category "Operations" in Itf.h file of the corresponding component for details.</element>
 * <element name="pCmpIdDisabled" type="OUT">Pointer to ComponentID to get the component which disabled the operation. Can be NULL.</element>
 * <result>
 *	<ul>
 *		<li>ERR_OK: Operation is disabled</li>
 *		<li>ERR_FAILED: Operation is enabled [default]</li>
 *		<li>ERR_NOT_SUPPORTED: Cannot be retrieved, because the event is not available</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL CMIsOperationDisabled(CMPID cmpId, RTS_UI32 ulOperation, CMPID *pCmpIdDisabled);
typedef RTS_RESULT (CDECL * PFCMISOPERATIONDISABLED) (CMPID cmpId, RTS_UI32 ulOperation, CMPID *pCmpIdDisabled);
#if defined(CM_NOTIMPLEMENTED) || defined(CMISOPERATIONDISABLED_NOTIMPLEMENTED)
	#define USE_CMIsOperationDisabled
	#define EXT_CMIsOperationDisabled
	#define GET_CMIsOperationDisabled(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMIsOperationDisabled(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMIsOperationDisabled  FALSE
	#define EXP_CMIsOperationDisabled  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMIsOperationDisabled
	#define EXT_CMIsOperationDisabled
	#define GET_CMIsOperationDisabled(fl)  CAL_CMGETAPI( "CMIsOperationDisabled" ) 
	#define CAL_CMIsOperationDisabled  CMIsOperationDisabled
	#define CHK_CMIsOperationDisabled  TRUE
	#define EXP_CMIsOperationDisabled  CAL_CMEXPAPI( "CMIsOperationDisabled" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMIsOperationDisabled
	#define EXT_CMIsOperationDisabled
	#define GET_CMIsOperationDisabled(fl)  CAL_CMGETAPI( "CMIsOperationDisabled" ) 
	#define CAL_CMIsOperationDisabled  CMIsOperationDisabled
	#define CHK_CMIsOperationDisabled  TRUE
	#define EXP_CMIsOperationDisabled  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMIsOperationDisabled", (RTS_UINTPTR)CMIsOperationDisabled, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMIsOperationDisabled
	#define EXT_CMCMIsOperationDisabled
	#define GET_CMCMIsOperationDisabled  ERR_OK
	#define CAL_CMCMIsOperationDisabled pICM->ICMIsOperationDisabled
	#define CHK_CMCMIsOperationDisabled (pICM != NULL)
	#define EXP_CMCMIsOperationDisabled  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMIsOperationDisabled
	#define EXT_CMIsOperationDisabled
	#define GET_CMIsOperationDisabled(fl)  CAL_CMGETAPI( "CMIsOperationDisabled" ) 
	#define CAL_CMIsOperationDisabled pICM->ICMIsOperationDisabled
	#define CHK_CMIsOperationDisabled (pICM != NULL)
	#define EXP_CMIsOperationDisabled  CAL_CMEXPAPI( "CMIsOperationDisabled" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMIsOperationDisabled  PFCMISOPERATIONDISABLED pfCMIsOperationDisabled;
	#define EXT_CMIsOperationDisabled  extern PFCMISOPERATIONDISABLED pfCMIsOperationDisabled;
	#define GET_CMIsOperationDisabled(fl)  s_pfCMGetAPI2( "CMIsOperationDisabled", (RTS_VOID_FCTPTR *)&pfCMIsOperationDisabled, (fl), 0, 0)
	#define CAL_CMIsOperationDisabled  pfCMIsOperationDisabled
	#define CHK_CMIsOperationDisabled  (pfCMIsOperationDisabled != NULL)
	#define EXP_CMIsOperationDisabled  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMIsOperationDisabled", (RTS_UINTPTR)CMIsOperationDisabled, 0, 0) 
#endif




#define CM_PROFILING_ID_ADD_BEGIN(ID)		(ID | UINT32_C(0x40000000))
#define CM_PROFILING_ID_ADD_END(ID)			(ID | UINT32_C(0x80000000))
#define CM_PROFILING_ID_ISBEGIN(ID)			((ID & UINT32_C(0x40000000)) != 0)
#define CM_PROFILING_ID_ISEND(ID)			((ID & UINT32_C(0x80000000)) != 0)
#define CM_PROFILING_ID_GET(ID)				(ID & UINT32_C(0x3FFFFFFF))

/**
 * <description> 
 *  Central function for profiling
 * </description>
 * <element name="pszInfo" type="IN">Optional info string. Can be NULL.</element>
 * <element name="cmpId" type="IN">Component ID</element>
 * <element name="pszComponentName" type="IN">Component name</element>
 * <element name="pszModuleName" type="IN">Module name</element>
 * <element name="nLine" type="IN">Line number in the module</element>
 * <element name="iID" type="IN">Optional ID to calculate the time difference between two entries with the same ID.
 *	iID = -1: No difference time
 *	iID = 0: No difference time 
 * </element>
 * <result>Error code</result>
 */
RTS_RESULT CDECL CMProfiling(char *pszInfo, CMPID cmpId, char *pszComponentName, char *pszModuleName, RTS_I32 nLine, RTS_I32 iID);
typedef RTS_RESULT (CDECL * PFCMPROFILING) (char *pszInfo, CMPID cmpId, char *pszComponentName, char *pszModuleName, RTS_I32 nLine, RTS_I32 iID);
#if defined(CM_NOTIMPLEMENTED) || defined(CMPROFILING_NOTIMPLEMENTED)
	#define USE_CMProfiling
	#define EXT_CMProfiling
	#define GET_CMProfiling(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMProfiling(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMProfiling  FALSE
	#define EXP_CMProfiling  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMProfiling
	#define EXT_CMProfiling
	#define GET_CMProfiling(fl)  CAL_CMGETAPI( "CMProfiling" ) 
	#define CAL_CMProfiling  CMProfiling
	#define CHK_CMProfiling  TRUE
	#define EXP_CMProfiling  CAL_CMEXPAPI( "CMProfiling" ) 
#elif defined(MIXED_LINK) && !defined(CM_EXTERNAL)
	#define USE_CMProfiling
	#define EXT_CMProfiling
	#define GET_CMProfiling(fl)  CAL_CMGETAPI( "CMProfiling" ) 
	#define CAL_CMProfiling  CMProfiling
	#define CHK_CMProfiling  TRUE
	#define EXP_CMProfiling  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMProfiling", (RTS_UINTPTR)CMProfiling, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMCMProfiling
	#define EXT_CMCMProfiling
	#define GET_CMCMProfiling  ERR_OK
	#define CAL_CMCMProfiling pICM->ICMProfiling
	#define CHK_CMCMProfiling (pICM != NULL)
	#define EXP_CMCMProfiling  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMProfiling
	#define EXT_CMProfiling
	#define GET_CMProfiling(fl)  CAL_CMGETAPI( "CMProfiling" ) 
	#define CAL_CMProfiling pICM->ICMProfiling
	#define CHK_CMProfiling (pICM != NULL)
	#define EXP_CMProfiling  CAL_CMEXPAPI( "CMProfiling" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMProfiling  PFCMPROFILING pfCMProfiling;
	#define EXT_CMProfiling  extern PFCMPROFILING pfCMProfiling;
	#define GET_CMProfiling(fl)  s_pfCMGetAPI2( "CMProfiling", (RTS_VOID_FCTPTR *)&pfCMProfiling, (fl), 0, 0)
	#define CAL_CMProfiling  pfCMProfiling
	#define CHK_CMProfiling  (pfCMProfiling != NULL)
	#define EXP_CMProfiling  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMProfiling", (RTS_UINTPTR)CMProfiling, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFCMINIT ICMInit;
 	PFCMINIT2 ICMInit2;
 	PFCMINIT3 ICMInit3;
 	PFCMEXIT ICMExit;
 	PFCMSETEXIT ICMSetExit;
 	PFCMGETEXIT ICMGetExit;
 	PFCMDEBUGOUT ICMDebugOut;
 	PFCMDEBUGOUTARG ICMDebugOutArg;
 	PFCMREGISTERAPI ICMRegisterAPI;
 	PFCMREGISTERAPI2 ICMRegisterAPI2;
 	PFCMGETAPI ICMGetAPI;
 	PFCMGETAPI2 ICMGetAPI2;
 	PFCMGETAPI3 ICMGetAPI3;
 	PFCMGETAPITORESOLVE ICMGetAPIToResolve;
 	PFCMRELEASEAPI ICMReleaseAPI;
 	PFCMLOADCOMPONENT ICMLoadComponent;
 	PFCMLOADCOMPONENT2 ICMLoadComponent2;
 	PFCMLOADCOMPONENT3 ICMLoadComponent3;
 	PFCMINITCOMPONENT ICMInitComponent;
 	PFCMIMPORTFUNCTIONS ICMImportFunctions;
 	PFCMEXITCOMPONENT ICMExitComponent;
 	PFCMUNLOADCOMPONENT ICMUnloadComponent;
 	PFCMADDCOMPONENT ICMAddComponent;
 	PFCMREMOVECOMPONENT ICMRemoveComponent;
 	PFCMGETCOMPONENTBYNAME ICMGetComponentByName;
 	PFCMGETCMPID ICMGetCmpId;
 	PFCMGETCMPNAME ICMGetCmpName;
 	PFCMGETCOMPONENT ICMGetComponent;
 	PFCMGETNUMOFCOMPONENTS ICMGetNumOfComponents;
 	PFCMGETCOMPONENTBYINDEX ICMGetComponentByIndex;
 	PFCMGETFIRSTCOMPONENT ICMGetFirstComponent;
 	PFCMGETNEXTCOMPONENT ICMGetNextComponent;
 	PFCMCREATEINSTANCE ICMCreateInstance;
 	PFCMDELETEINSTANCE ICMDeleteInstance;
 	PFCMDELETEINSTANCE2 ICMDeleteInstance2;
 	PFCMGETINSTANCE ICMGetInstance;
 	PFCMREGISTERINSTANCE ICMRegisterInstance;
 	PFCMUNREGISTERINSTANCE ICMUnregisterInstance;
 	PFCMGETINSTANCELIST ICMGetInstanceList;
 	PFCMCALLHOOK ICMCallHook;
 	PFCMCALLHOOK2 ICMCallHook2;
 	PFCMISDEMO ICMIsDemo;
 	PFCMINITEND ICMInitEnd;
 	PFCMCHECKSYSTARGETSIGNATURE ICMCheckSysTargetSignature;
 	PFCMGETUSERLICENSEVALUE ICMGetUserLicenseValue;
 	PFCMREQDYNLICCHALLENGE ICMReqDynLicChallenge;
 	PFCMCONFDYNLICCHALLENGE ICMConfDynLicChallenge;
 	PFCMCALLEXTRACOMMCYCLEHOOK ICMCallExtraCommCycleHook;
 	PFCMGETCURRENTHOOK ICMGetCurrentHook;
 	PFCMISOPERATIONDISABLED ICMIsOperationDisabled;
 	PFCMPROFILING ICMProfiling;
 } ICM_C;

#ifdef CPLUSPLUS
class ICM : public IBase
{
	public:
		virtual RTS_RESULT CDECL ICMInit(char *pszSettingsFile, StaticComponent *pStaticComponents) =0;
		virtual RTS_RESULT CDECL ICMInit2(char *pszSettingsFile, StaticComponent *pStaticComponents, int bSettingsFileIsOptional) =0;
		virtual RTS_RESULT CDECL ICMInit3(char *pszSettingsFile, StaticComponent *pStaticComponents, RTS_UI32 options) =0;
		virtual RTS_RESULT CDECL ICMExit(void) =0;
		virtual RTS_RESULT CDECL ICMSetExit(void) =0;
		virtual RTS_RESULT CDECL ICMGetExit(void) =0;
		virtual RTS_RESULT CDECL ICMDebugOut(const char *szFormat, ...) =0;
		virtual RTS_RESULT CDECL ICMDebugOutArg(const char *szFormat, va_list *pargList) =0;
		virtual RTS_RESULT CDECL ICMRegisterAPI(const CMP_EXT_FUNCTION_REF *pExpTable, RTS_UINTPTR dummy, int bExternalLibrary, RTS_UI32 cmpId) =0;
		virtual RTS_RESULT CDECL ICMRegisterAPI2(const char *pszAPIName, RTS_VOID_FCTPTR pfAPIFunction, int bExternalLibrary, RTS_UI32 ulSignatureID, RTS_UI32 ulVersion) =0;
		virtual RTS_RESULT CDECL ICMGetAPI(char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, RTS_UI32 ulSignatureID) =0;
		virtual RTS_RESULT CDECL ICMGetAPI2(char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, int importOptions, RTS_UI32 ulSignatureID, RTS_UI32 ulVersion) =0;
		virtual RTS_RESULT CDECL ICMGetAPI3(char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, int importOptions, RTS_UI32 *pulSignatureID, RTS_UI32 *pulVersion) =0;
		virtual RTS_RESULT CDECL ICMGetAPIToResolve(API_RESOLVE_INFO *pApiResolveInfo) =0;
		virtual RTS_RESULT CDECL ICMReleaseAPI(RTS_VOID_FCTPTR pfAPIFunction) =0;
		virtual RTS_HANDLE CDECL ICMLoadComponent(char *pszComponent, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ICMLoadComponent2(char *pszComponent, RTS_UI16 iType, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ICMLoadComponent3(char *pszComponent, char *pszFilePath, RTS_UI16 iType, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ICMInitComponent(RTS_HANDLE hComponent) =0;
		virtual RTS_RESULT CDECL ICMImportFunctions(void) =0;
		virtual RTS_RESULT CDECL ICMExitComponent(RTS_HANDLE hComponent) =0;
		virtual RTS_RESULT CDECL ICMUnloadComponent(RTS_HANDLE hComponent) =0;
		virtual RTS_HANDLE CDECL ICMAddComponent(COMPONENT_ENTRY *pComponent, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ICMRemoveComponent(RTS_HANDLE hComponent) =0;
		virtual RTS_HANDLE CDECL ICMGetComponentByName(char *pszCmpName, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ICMGetCmpId(char *pszCmpName, CMPID *pCmpId) =0;
		virtual RTS_RESULT CDECL ICMGetCmpName(CMPID CmpId, char *pszCmpName, int iMaxCmpName) =0;
		virtual COMPONENT_ENTRY* CDECL ICMGetComponent(CMPID CmpId) =0;
		virtual int CDECL ICMGetNumOfComponents(void) =0;
		virtual COMPONENT_ENTRY* CDECL ICMGetComponentByIndex(int iIndex) =0;
		virtual COMPONENT_ENTRY* CDECL ICMGetFirstComponent(RTS_RESULT *pResult) =0;
		virtual COMPONENT_ENTRY* CDECL ICMGetNextComponent(COMPONENT_ENTRY *pCmp, RTS_RESULT *pResult) =0;
		virtual IBase* CDECL ICMCreateInstance(CLASSID ClassId, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ICMDeleteInstance(IBase *pIBase) =0;
		virtual RTS_RESULT CDECL ICMDeleteInstance2(CLASSID ClassId, IBase *pIBase) =0;
		virtual IBase* CDECL ICMGetInstance(CLASSID ClassId, OBJID ObjId, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ICMRegisterInstance(CLASSID ClassId, OBJID ObjId, IBase *pIBase) =0;
		virtual RTS_RESULT CDECL ICMUnregisterInstance(IBase *pIBase) =0;
		virtual RTS_RESULT CDECL ICMGetInstanceList(ITFID ItfId, RTS_HANDLE hIBasePool) =0;
		virtual RTS_RESULT CDECL ICMCallHook(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2, int bReverse) =0;
		virtual RTS_RESULT CDECL ICMCallHook2(RTS_UI16 usComponentTypeMask, RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2) =0;
		virtual int CDECL ICMIsDemo(void) =0;
		virtual RTS_RESULT CDECL ICMInitEnd(void) =0;
		virtual RTS_RESULT CDECL ICMCheckSysTargetSignature(void) =0;
		virtual RTS_UI32 CDECL ICMGetUserLicenseValue(RTS_UI32 ulLicenseID, RTS_RESULT *pResult) =0;
		virtual RTS_UI32 CDECL ICMReqDynLicChallenge(RTS_UI32 ulLicenseID, RTS_UI32 ulNewLicenseValue, RTS_RESULT *pResult) =0;
		virtual int CDECL ICMConfDynLicChallenge(RTS_UI32 ulLicenseID, RTS_UI32 ulNewLicenseValue, RTS_UI32 ulChallenge, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ICMCallExtraCommCycleHook(RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2) =0;
		virtual RTS_UI32 CDECL ICMGetCurrentHook(void) =0;
		virtual RTS_RESULT CDECL ICMIsOperationDisabled(CMPID cmpId, RTS_UI32 ulOperation, CMPID *pCmpIdDisabled) =0;
		virtual RTS_RESULT CDECL ICMProfiling(char *pszInfo, CMPID cmpId, char *pszComponentName, char *pszModuleName, RTS_I32 nLine, RTS_I32 iID) =0;
};
	#ifndef ITF_CM
		#define ITF_CM static ICM *pICM = NULL;
	#endif
	#define EXTITF_CM
#else	/*CPLUSPLUS*/
	typedef ICM_C		ICM;
	#ifndef ITF_CM
		#define ITF_CM
	#endif
	#define EXTITF_CM
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMITF_H_*/
