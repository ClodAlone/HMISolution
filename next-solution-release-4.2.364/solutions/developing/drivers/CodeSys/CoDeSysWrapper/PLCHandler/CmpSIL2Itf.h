 /**
 * <interfacename>CmpSIL2</interfacename>
 * <description> 
 * <p>This is the interface of the SIL2 component. The CmpSIL2 is used to implement several
 *    savety related feates, which are (in this form) only necessary in safety PLCs, following
 *    the Safety SIL2 concept of 3S.</p>
 * <p>These features include the following:</p>
 * <ul>
 *   <li>Safe loading of a bootproject</li>
 *   <li>Selftest Hooks</li>
 *   <li>Switch between safe- and unsafe contexts</li>
 *   <li>Safe logger interface</li>
 *   <li>Check functions, to determine the current context and operation mode</li>
 *   <li>Control Flow</li>
 * </ul>
 * <p>1) Bootproject</p>
 * <p>In CoDeSys Control SIL2, the bootproject is always loaded from flash,
 *    and from there, only in the "compact download format", as it is used
 *    by CmpAppEmbedded. In a standard CoDeSys Control Runtime, the bootproject
 *    is implicitely loaded, by CmpApp/CmpAppEmbedded within an init hook, that
 *    is executed during the call of CMInit() at startup.</p>
 * <p>In CoDeSys Control SIL2, this implicite loading is deactivated. Instead, the
 *    OEM customer has to call SIL2AppBoot() after CMInit().</p>
 * <p>The reason for this constraint is, that the function SIL2AppBoot() can make
 *    sure that the application is loaded with the constraints that are defined
 *    for a CoDeSys Control SIL2 system. Additionally, it is more safe to load 
 *    the application after system startup is completed, because no unsafe init code
 *    is executed anymore after the application is loaded.</p>
 * <p>2) Selftest Hooks</p>
 * <p>Virtually every safety system needs some kind of self tests, which are either
 *    executed in every Safety Process Cycle, or within a specific time frame in
 *    the background.</p>
 * <p>While the selftests in the background need to be seperated into small chunks,
 *    which are executed in the COMM-Cycle (= Super Loop), the selftests at startup
 *    need to be executed at once, to prohibit the execution of the application when
 *    there is an error in the hardware or firmware.</p>
 * <p>3) Switches between safe, unsafe and system contexts</p>
 * <p>The current execution context is managed by the OEM customer. But, to have
 *    a clear interface to switch between the contexts in a higher level, and to
 *    be able to execute unsafe code when coming from a safe context.</p>
 * <p>4) Safe logger interface</p>
 * <p>The standard monitoring of CoDeSys as well as the standard logger are not safe.
 *    Therefore, they may not be usable for the Endcustomer in some circumstances.</p>
 * <p>To have a safe interface to transport messages and data from the CoDeSys Control
 *    Runtiem to the programming and debugging system, the component CmpSIL2 provides
 *    it's own logger interface, which secures the data from unintended modifications
 *    with a checksum.</p>
 * <p>4) Check functions</p>
 * <p>The CmpSIL2 interface specifies a few functions, which need to be implemented by
 *    the OEM customer to determine the current context or operation mode. This is mainly
 *    necessary to restrict the execution of several functions, but there might also
 *    be other use-cases in which those functions migth be helpful for the OEM himself.</p>
 * <p>5) Control Flow</p>
 * <p>The component CmpSIL2, provides a control flow logging mechanism, that is used
 *    for diagnostic purposes, to check the control flow of the IEC tasks, before they
 *    are passing their outputs to the I/O drivers.</p>
 * <p>This interface is generically designed, and may therefore also be used by the OEM
 *    customer, to log and check his own control flow of his subsystems.</p>
 * <pre>
 * Example usage:
 * SIL2_CONTROLFLOW s_IoMgrControlFlow[3];
 * SIL2ControlFlowLog(s_IoMgrControlFlow, 0); 
 * SIL2ControlFlowLog(s_IoMgrControlFlow, 1); 
 * SIL2ControlFlowLog(s_IoMgrControlFlow, 2); 
 * SIL2ControlFlowCheck(s_IoMgrControlFlow, 3);
 * </pre>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPSIL2ITF_H_
#define _CMPSIL2ITF_H_

#include "CmpStd.h"

 

 




typedef RTS_UI32 RTS_SIL2_EXCEPTION;
typedef RTS_UI32 RTS_SIL2_OPMODE;
typedef RTS_UI32 RTS_SIL2_ADDRESSSTATE;

/**
 * <category>Buffer sizes</category>
 * <description>Specifies the maximum logable string size.</description>
 */
#ifndef CMPSIL2_LOGADD_MAX_STRLEN
	#define CMPSIL2_LOGADD_MAX_STRLEN 36
#endif

/**
 * <category>Buffer sizes</category>
 * <description>Specifies the maximum number of strings.</description>
 */
#ifndef CMPSIL2_LOGADD_MAX_ENTRIES
	#define CMPSIL2_LOGADD_MAX_ENTRIES 50
#endif

#define RTS_SIL2_OPMODE_SAFE 					0xAABB
#define RTS_SIL2_OPMODE_DEBUG 					0x7788

#define RTS_SIL2_ADDRESS_SAFE 					0xCCDD
#define RTS_SIL2_ADDRESS_UNSAFE					0x5566
#define RTS_SIL2_OPMODE_ERROR					0xFFFF

#define RTS_SIL2_CALLERCTX_SAFE					0xBBAA
#define RTS_SIL2_CALLERCTX_UNSAFE				0xDDCC
#define RTS_SIL2_CALLERCTX_SYSTEM				0xFFEE
#define RTS_SIL2_CALLERCTX_ERROR				0xFFFF

/**
 * <category>Exceptions</category>
 * <description>Exceptions: Runtime has encountered an error in safety mode and passes the info to an OEM Interface function</description>
 */
/* unspecified error */
#define RTS_SIL2_FAIL_GENERAL						0x01 	/* unspecified Error */

/* while loading bootproject*/
#define RTS_SIL2_EXCEPTION_LOADBOOTPROJECT			0x10	/* unspecified Error while Loading Bootproject */
#define RTS_SIL2_EXCEPTION_BOOTPROJECT_INVALID		0x11	/* Bootproject Format does not match */
#define RTS_SIL2_EXCEPTION_COMPACTPREPARECODE		0x12	/* Error while Loading Bootproject, Creating of IEC Application */
#define RTS_SIL2_EXCEPTION_COMPACTFINISH			0x13	/* Error while Setting up IEC Application, Calling IEC init Code (global init...) */
#define RTS_SIL2_EXCEPTION_EXTREFS					0x14	/* Error while Setting up external References */
#define RTS_SIL2_EXCEPTION_RUNAFTERDOWNLOAD			0x15	/* Error while SIL2AppRunAfterDownloadCode */
#define RTS_SIL2_EXCEPTION_MEMORY					0x16	/* Error regarding the Memory at Bootproject start */
#define RTS_SIL2_EXCEPTION_RUNTIME_INIT				0x17	/* Error while CMInit2 */


/* while operational */
#define RTS_SIL2_EXCEPTION_SCHED_WD					0x20	/* Scheduler Watchdog Exception */
#define RTS_SIL2_EXCEPTION_CRC_CYCLIC				0x21	/* Cyclic Check of CRC was wrong */
#define RTS_SIL2_EXCEPTION_CRC_COMPLETE				0x22	/* Complete Check of CRC was wrong */
#define RTS_SIL2_EXCEPTION_CRC_INIT					0x23	/* Init of CRC Check wrong */
#define RTS_SIL2_EXCEPTION_LIFE_COUNTER				0x24	/* Not all Components were called in safe Hook */
#define RTS_SIL2_EXCEPTION_CALLERCONTEXT			0x25	/* Caller Context Check was wrong */

/* By Default ignore the SIL2 attribute */
#ifndef RTS_SIL2_ATTRIBUTE
	#define RTS_SIL2_ATTRIBUTE
#endif

/* Define the default list of components, that need a safety COMM-Cycle */
#ifndef RTS_SAFETY_COMPONENTS
	int CmpScheduleTimer__Entry(INIT_STRUCT *pInitStruct);
	int CmpSIL2__Entry(INIT_STRUCT *pInitStruct);
	int IoDrvUnsafeBridge__Entry(INIT_STRUCT *pInitStruct);
	#define RTS_SAFETY_COMPONENTS \
		{"CmpScheduleTimer", CmpScheduleTimer__Entry, 1}, \
		{"CmpSIL2", CmpSIL2__Entry, 1}, \
		{"IoDrvUnsafeBridge", IoDrvUnsafeBridge__Entry, 1}, \
		{NULL, NULL, 0}

#endif

/**
 * <SIL2/>
 * <category>Typedef</category>
 * <description><p>Buffertype for Control Flow mechanism</p>
 * </description>
 */
typedef struct SIL2_CONTROLFLOW
{
	RTS_UI32 uiPattern;
	RTS_UI32 uiCRC;
} SIL2_CONTROLFLOW;

#define SIL2_NUM_CONTROL_FLOW_POINTS 3

#ifdef RTS_SIL2
extern SIL2_CONTROLFLOW g_ControlFlowIecTasks[NUM_OF_STATIC_IEC_TASKS][SIL2_NUM_CONTROL_FLOW_POINTS];
#endif

/**
 * <SIL2/>
 * <category>Typedef</category>
 * <description><p>SIL2 life counter for safe comm cycle hook</p>
 * </description>
 */
#ifdef RTS_SIL2
extern RTS_UI32 g_RTS_SIL2_Cycle_LifeCounter_CmpSIL2;
#endif




#ifdef __cplusplus
extern "C" {
#endif

/* Init routines for OEM specific module */
RTS_RESULT CDECL CmpSIL2OEMInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL CmpSIL2OEMHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);



/**
 * <description>
 * <p>Function to load and start a Bootproject from Flash</p>
 * <p>Application Note: The caller must ensure that this function is called only within a safe context!</p>
 * <p>The SIL2 Component only loads bootprojects, which are in the "compact download format" format, and those only from flash.
 * This format includes an checksum to check the consistency of that bootproject.
 * Before loading the bootproject the CmpSIL2 checks this CRC to make sure that the bootproject was not corrupted after CoDeSys Programming System created it.
 * As the data-initialization is done from bootproject-code, the CRC also covers the data in the bootproject.
 * The main steps of loading the bootproject are:
 * </p>
 * <ul>
 *   <li>Create an application with the given name</li>
 *   <li>Get the Startaddress of the Bootproject in Flash</li>
 *   <li>Allocate Application Areas (Code - Flash, Data - SysMemAllocArea())</li>
 *   <li>Check the Bootproject (Bootproject Type, Target ID, Type, Version) and create GUIDs</li>
 *   <li>Call the Data-Initialization-Code from the bootproject (Relocation, Code Init, Download POU, Global Init, Global Exit) </li>
 *   <li>Set the Application State to loaded</li>
 *   <li>If everything is correct, start the Application</li>
 *	</ul>
 * <p>Function has no function parameter, but behavior is also dependant on state of bootproject</p>
 * </description> 
 * <parampseudo name="bExtFuncsAreValid" type="IN" range="[TRUE,FALSE]">Specifies if the application references an unsafe or non-existent external function.</parampseudo>
 * <parampseudo name="bAreaIsValid" type="IN" range="[TRUE,FALSE]">Specifies if the Areas of the bootapplication are valid or not.</parampseudo>
 * <parampseudo name="Bootproject.CRC" type="IN" range="[BOOTP_NO_PROJECT,BOOTP_VALID_CRC,BOOTP_INVALID_CRC]">Bootproject in flash, States: valid CRC, invalid CRC </parampseudo>
 * <parampseudo name="Bootproject.Code" type="IN" range="[INVALID_AREA,INVALID_DEV_VERSION,RELOC_VALID,RELOC_IDLE,RELOC_EXCEPTION,CODEINIT_VALID,CODEINIT_IDLE,CODEINIT_EXCEPTION,GLOBALINIT_VALID,GLOBALINIT_IDLE,GLOBALINIT_EXCEPTION,DOWNLOAD_POU_VALID,DOWNLOAD_POU_IDLE,DOWNLOAD_POU_EXCEPTION,GLOBALEXIT_VALID,GLOBALEXIT_IDLE,GLOBALEXIT_EXCEPTION]">valid Code, idle Code (harmless), invalid Code producing Exception</parampseudo>  
 * <parampseudo name="Bootproject.TargetType" type="IN" range="[INVALID_TARGET_TYPE,VALID_TARGET_TYPE]">Target Type</parampseudo>  
 * <parampseudo name="Bootproject.TargetID" type="IN" range="[INVALID_TARGET_ID,VALID_TARGET_ID]">Target ID</parampseudo>  
 * <parampseudo name="bExceptionGenerated" type="OUT" range="[FALSE,TRUE]">TRUE, when an exception occured, during execution</parampseudo>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Load Bootproject successful</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_OBJECT">No Bootproject in Flash</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Error while loading Bootapp (memory area could not be allocated, CRC was invalid, or an external function, which was referenced by the application, is not listed in the list of safe functions)</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_TYPE_MISMATCH">wrong DeviceType in Bootproject</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_ID_MISMATCH">wrong DeviceId in Bootproject</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_VERSION_MISMATCH">wrong DeviceVersion in Bootproject</errorcode>
 * <result>Result of loading bootproject</result>
 */
RTS_RESULT CDECL SIL2AppBoot(void);
typedef RTS_RESULT (CDECL * PFSIL2APPBOOT) (void);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2APPBOOT_NOTIMPLEMENTED)
	#define USE_SIL2AppBoot
	#define EXT_SIL2AppBoot
	#define GET_SIL2AppBoot(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2AppBoot()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SIL2AppBoot  FALSE
	#define EXP_SIL2AppBoot  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2AppBoot
	#define EXT_SIL2AppBoot
	#define GET_SIL2AppBoot(fl)  CAL_CMGETAPI( "SIL2AppBoot" ) 
	#define CAL_SIL2AppBoot  SIL2AppBoot
	#define CHK_SIL2AppBoot  TRUE
	#define EXP_SIL2AppBoot  CAL_CMEXPAPI( "SIL2AppBoot" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2AppBoot
	#define EXT_SIL2AppBoot
	#define GET_SIL2AppBoot(fl)  CAL_CMGETAPI( "SIL2AppBoot" ) 
	#define CAL_SIL2AppBoot  SIL2AppBoot
	#define CHK_SIL2AppBoot  TRUE
	#define EXP_SIL2AppBoot  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2AppBoot", (RTS_UINTPTR)SIL2AppBoot, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2AppBoot
	#define EXT_CmpSIL2SIL2AppBoot
	#define GET_CmpSIL2SIL2AppBoot  ERR_OK
	#define CAL_CmpSIL2SIL2AppBoot pICmpSIL2->ISIL2AppBoot
	#define CHK_CmpSIL2SIL2AppBoot (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2AppBoot  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2AppBoot
	#define EXT_SIL2AppBoot
	#define GET_SIL2AppBoot(fl)  CAL_CMGETAPI( "SIL2AppBoot" ) 
	#define CAL_SIL2AppBoot pICmpSIL2->ISIL2AppBoot
	#define CHK_SIL2AppBoot (pICmpSIL2 != NULL)
	#define EXP_SIL2AppBoot  CAL_CMEXPAPI( "SIL2AppBoot" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2AppBoot  PFSIL2APPBOOT pfSIL2AppBoot;
	#define EXT_SIL2AppBoot  extern PFSIL2APPBOOT pfSIL2AppBoot;
	#define GET_SIL2AppBoot(fl)  s_pfCMGetAPI2( "SIL2AppBoot", (RTS_VOID_FCTPTR *)&pfSIL2AppBoot, (fl), 0, 0)
	#define CAL_SIL2AppBoot  pfSIL2AppBoot
	#define CHK_SIL2AppBoot  (pfSIL2AppBoot != NULL)
	#define EXP_SIL2AppBoot  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2AppBoot", (RTS_UINTPTR)SIL2AppBoot, 0, 0) 
#endif





/**
 * <description>sil2checkcallercontext</description>
 * <SIL2/>
 */
typedef struct tagsil2checkcallercontext_struct
{
	RTS_IEC_UDINT udiCallerContextExpected;	/* VAR_INPUT */
	RTS_IEC_RESULT SIL2CheckCallerContext;	/* VAR_OUTPUT */
} sil2checkcallercontext_struct;

/**
 * <description>
 * <p>Function to check the current Caller Context of the Runtime</p>
 * <p>Expected Context is checked vs the current context, and an exception is thrown if they are different.</p>
 * </description> 
 * <param name="ulCallerContextExpected" type="IN" range="[RTS_SIL2_CALLERCTX_SAFE,RTS_SIL2_CALLERCTX_UNSAFE,RTS_SIL2_CALLERCTX_ERROR]">Expected Context, that is checked against current context.</param>
 * <parampseudo name="bSafeContext" type="IN" range="[TRUE,FALSE]">Specifies if the context is safe or unsafe.</parampseudo>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Context is ok</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Context could not be checked successfully, an internal error occured.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_EXCEPTION">Contexts are different</errorcode> 
 * <result>Returns if comparision was ok or not, or if problem occured.</result>
 */
RTS_RESULT CDECL SIL2CheckCallerContext(RTS_UI32 ulCallerContextExpected);
typedef RTS_RESULT (CDECL * PFSIL2CHECKCALLERCONTEXT) (RTS_UI32 ulCallerContextExpected);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2CHECKCALLERCONTEXT_NOTIMPLEMENTED)
	#define USE_SIL2CheckCallerContext
	#define EXT_SIL2CheckCallerContext
	#define GET_SIL2CheckCallerContext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2CheckCallerContext(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SIL2CheckCallerContext  FALSE
	#define EXP_SIL2CheckCallerContext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2CheckCallerContext
	#define EXT_SIL2CheckCallerContext
	#define GET_SIL2CheckCallerContext(fl)  CAL_CMGETAPI( "SIL2CheckCallerContext" ) 
	#define CAL_SIL2CheckCallerContext  SIL2CheckCallerContext
	#define CHK_SIL2CheckCallerContext  TRUE
	#define EXP_SIL2CheckCallerContext  CAL_CMEXPAPI( "SIL2CheckCallerContext" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2CheckCallerContext
	#define EXT_SIL2CheckCallerContext
	#define GET_SIL2CheckCallerContext(fl)  CAL_CMGETAPI( "SIL2CheckCallerContext" ) 
	#define CAL_SIL2CheckCallerContext  SIL2CheckCallerContext
	#define CHK_SIL2CheckCallerContext  TRUE
	#define EXP_SIL2CheckCallerContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2CheckCallerContext", (RTS_UINTPTR)SIL2CheckCallerContext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2CheckCallerContext
	#define EXT_CmpSIL2SIL2CheckCallerContext
	#define GET_CmpSIL2SIL2CheckCallerContext  ERR_OK
	#define CAL_CmpSIL2SIL2CheckCallerContext pICmpSIL2->ISIL2CheckCallerContext
	#define CHK_CmpSIL2SIL2CheckCallerContext (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2CheckCallerContext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2CheckCallerContext
	#define EXT_SIL2CheckCallerContext
	#define GET_SIL2CheckCallerContext(fl)  CAL_CMGETAPI( "SIL2CheckCallerContext" ) 
	#define CAL_SIL2CheckCallerContext pICmpSIL2->ISIL2CheckCallerContext
	#define CHK_SIL2CheckCallerContext (pICmpSIL2 != NULL)
	#define EXP_SIL2CheckCallerContext  CAL_CMEXPAPI( "SIL2CheckCallerContext" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2CheckCallerContext  PFSIL2CHECKCALLERCONTEXT pfSIL2CheckCallerContext;
	#define EXT_SIL2CheckCallerContext  extern PFSIL2CHECKCALLERCONTEXT pfSIL2CheckCallerContext;
	#define GET_SIL2CheckCallerContext(fl)  s_pfCMGetAPI2( "SIL2CheckCallerContext", (RTS_VOID_FCTPTR *)&pfSIL2CheckCallerContext, (fl), 0, 0)
	#define CAL_SIL2CheckCallerContext  pfSIL2CheckCallerContext
	#define CHK_SIL2CheckCallerContext  (pfSIL2CheckCallerContext != NULL)
	#define EXP_SIL2CheckCallerContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2CheckCallerContext", (RTS_UINTPTR)SIL2CheckCallerContext, 0, 0) 
#endif



void CDECL CDECL_EXT sil2checkcallercontext(sil2checkcallercontext_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2CHECKCALLERCONTEXT_IEC) (sil2checkcallercontext_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2CHECKCALLERCONTEXT_NOTIMPLEMENTED)
	#define USE_sil2checkcallercontext
	#define EXT_sil2checkcallercontext
	#define GET_sil2checkcallercontext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2checkcallercontext(p0) 
	#define CHK_sil2checkcallercontext  FALSE
	#define EXP_sil2checkcallercontext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2checkcallercontext
	#define EXT_sil2checkcallercontext
	#define GET_sil2checkcallercontext(fl)  CAL_CMGETAPI( "sil2checkcallercontext" ) 
	#define CAL_sil2checkcallercontext  sil2checkcallercontext
	#define CHK_sil2checkcallercontext  TRUE
	#define EXP_sil2checkcallercontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2checkcallercontext", (RTS_UINTPTR)sil2checkcallercontext, 1, 0x829BB451, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2checkcallercontext
	#define EXT_sil2checkcallercontext
	#define GET_sil2checkcallercontext(fl)  CAL_CMGETAPI( "sil2checkcallercontext" ) 
	#define CAL_sil2checkcallercontext  sil2checkcallercontext
	#define CHK_sil2checkcallercontext  TRUE
	#define EXP_sil2checkcallercontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2checkcallercontext", (RTS_UINTPTR)sil2checkcallercontext, 1, 0x829BB451, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2checkcallercontext
	#define EXT_CmpSIL2sil2checkcallercontext
	#define GET_CmpSIL2sil2checkcallercontext  ERR_OK
	#define CAL_CmpSIL2sil2checkcallercontext  sil2checkcallercontext
	#define CHK_CmpSIL2sil2checkcallercontext  TRUE
	#define EXP_CmpSIL2sil2checkcallercontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2checkcallercontext", (RTS_UINTPTR)sil2checkcallercontext, 1, 0x829BB451, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2checkcallercontext
	#define EXT_sil2checkcallercontext
	#define GET_sil2checkcallercontext(fl)  CAL_CMGETAPI( "sil2checkcallercontext" ) 
	#define CAL_sil2checkcallercontext  sil2checkcallercontext
	#define CHK_sil2checkcallercontext  TRUE
	#define EXP_sil2checkcallercontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2checkcallercontext", (RTS_UINTPTR)sil2checkcallercontext, 1, 0x829BB451, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2checkcallercontext  PFSIL2CHECKCALLERCONTEXT_IEC pfsil2checkcallercontext;
	#define EXT_sil2checkcallercontext  extern PFSIL2CHECKCALLERCONTEXT_IEC pfsil2checkcallercontext;
	#define GET_sil2checkcallercontext(fl)  s_pfCMGetAPI2( "sil2checkcallercontext", (RTS_VOID_FCTPTR *)&pfsil2checkcallercontext, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x829BB451, 0x03050400)
	#define CAL_sil2checkcallercontext  pfsil2checkcallercontext
	#define CHK_sil2checkcallercontext  (pfsil2checkcallercontext != NULL)
	#define EXP_sil2checkcallercontext   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2checkcallercontext", (RTS_UINTPTR)sil2checkcallercontext, 1, 0x829BB451, 0x03050400) 
#endif



/**
 * <description>
 * <p>Function to copy the current CodeGuid to a given location (by Pointer)</p>
 * <p>Size is always 16 BYTES! The caller must provided this much space.</p>
 * </description> 
 * <param name="pDstCodeGuid" type="IN" range="[NULL,VALID_PDSTCODEGUID]">Pointer where the guid should be copied to.</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Copy was ok</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Could not get requested Guid.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Pointer invalid</errorcode>
 * <SIL2/>
 */
typedef struct tagsil2copycodeguid_struct
{
	RTS_IEC_BYTE *pDstCodeGuid;			/* VAR_INPUT */
	RTS_IEC_RESULT SIL2CopyCodeGuid;	/* VAR_OUTPUT */
} sil2copycodeguid_struct;

void CDECL CDECL_EXT sil2copycodeguid(sil2copycodeguid_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2COPYCODEGUID_IEC) (sil2copycodeguid_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2COPYCODEGUID_NOTIMPLEMENTED)
	#define USE_sil2copycodeguid
	#define EXT_sil2copycodeguid
	#define GET_sil2copycodeguid(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2copycodeguid(p0) 
	#define CHK_sil2copycodeguid  FALSE
	#define EXP_sil2copycodeguid  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2copycodeguid
	#define EXT_sil2copycodeguid
	#define GET_sil2copycodeguid(fl)  CAL_CMGETAPI( "sil2copycodeguid" ) 
	#define CAL_sil2copycodeguid  sil2copycodeguid
	#define CHK_sil2copycodeguid  TRUE
	#define EXP_sil2copycodeguid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copycodeguid", (RTS_UINTPTR)sil2copycodeguid, 1, 0x638D4565, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2copycodeguid
	#define EXT_sil2copycodeguid
	#define GET_sil2copycodeguid(fl)  CAL_CMGETAPI( "sil2copycodeguid" ) 
	#define CAL_sil2copycodeguid  sil2copycodeguid
	#define CHK_sil2copycodeguid  TRUE
	#define EXP_sil2copycodeguid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copycodeguid", (RTS_UINTPTR)sil2copycodeguid, 1, 0x638D4565, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2copycodeguid
	#define EXT_CmpSIL2sil2copycodeguid
	#define GET_CmpSIL2sil2copycodeguid  ERR_OK
	#define CAL_CmpSIL2sil2copycodeguid  sil2copycodeguid
	#define CHK_CmpSIL2sil2copycodeguid  TRUE
	#define EXP_CmpSIL2sil2copycodeguid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copycodeguid", (RTS_UINTPTR)sil2copycodeguid, 1, 0x638D4565, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2copycodeguid
	#define EXT_sil2copycodeguid
	#define GET_sil2copycodeguid(fl)  CAL_CMGETAPI( "sil2copycodeguid" ) 
	#define CAL_sil2copycodeguid  sil2copycodeguid
	#define CHK_sil2copycodeguid  TRUE
	#define EXP_sil2copycodeguid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copycodeguid", (RTS_UINTPTR)sil2copycodeguid, 1, 0x638D4565, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2copycodeguid  PFSIL2COPYCODEGUID_IEC pfsil2copycodeguid;
	#define EXT_sil2copycodeguid  extern PFSIL2COPYCODEGUID_IEC pfsil2copycodeguid;
	#define GET_sil2copycodeguid(fl)  s_pfCMGetAPI2( "sil2copycodeguid", (RTS_VOID_FCTPTR *)&pfsil2copycodeguid, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x638D4565, 0x03050400)
	#define CAL_sil2copycodeguid  pfsil2copycodeguid
	#define CHK_sil2copycodeguid  (pfsil2copycodeguid != NULL)
	#define EXP_sil2copycodeguid   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copycodeguid", (RTS_UINTPTR)sil2copycodeguid, 1, 0x638D4565, 0x03050400) 
#endif


/**
 * <description>
 * <p>Function to copy the current DataGuid to a given location (by Pointer)</p>
 * <p>Size is always 16 BYTES! The caller must provided this much space.</p>
 * </description> 
 * <param name="pDstCodeGuid" type="IN" range="[NULL,VALID_PDSTDATAGUID]">Pointer where the guid should be copied to.</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Copy was ok</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Could not get requested Guid.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Pointer invalid</errorcode>
 * <SIL2/>
 */
typedef struct tagsil2copydataguid_struct
{
	RTS_IEC_BYTE *pDstDataGuid;			/* VAR_INPUT */
	RTS_IEC_RESULT SIL2CopyDataGuid;	/* VAR_OUTPUT */
} sil2copydataguid_struct;

void CDECL CDECL_EXT sil2copydataguid(sil2copydataguid_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2COPYDATAGUID_IEC) (sil2copydataguid_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2COPYDATAGUID_NOTIMPLEMENTED)
	#define USE_sil2copydataguid
	#define EXT_sil2copydataguid
	#define GET_sil2copydataguid(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2copydataguid(p0) 
	#define CHK_sil2copydataguid  FALSE
	#define EXP_sil2copydataguid  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2copydataguid
	#define EXT_sil2copydataguid
	#define GET_sil2copydataguid(fl)  CAL_CMGETAPI( "sil2copydataguid" ) 
	#define CAL_sil2copydataguid  sil2copydataguid
	#define CHK_sil2copydataguid  TRUE
	#define EXP_sil2copydataguid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copydataguid", (RTS_UINTPTR)sil2copydataguid, 1, 0x44F9CD8A, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2copydataguid
	#define EXT_sil2copydataguid
	#define GET_sil2copydataguid(fl)  CAL_CMGETAPI( "sil2copydataguid" ) 
	#define CAL_sil2copydataguid  sil2copydataguid
	#define CHK_sil2copydataguid  TRUE
	#define EXP_sil2copydataguid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copydataguid", (RTS_UINTPTR)sil2copydataguid, 1, 0x44F9CD8A, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2copydataguid
	#define EXT_CmpSIL2sil2copydataguid
	#define GET_CmpSIL2sil2copydataguid  ERR_OK
	#define CAL_CmpSIL2sil2copydataguid  sil2copydataguid
	#define CHK_CmpSIL2sil2copydataguid  TRUE
	#define EXP_CmpSIL2sil2copydataguid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copydataguid", (RTS_UINTPTR)sil2copydataguid, 1, 0x44F9CD8A, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2copydataguid
	#define EXT_sil2copydataguid
	#define GET_sil2copydataguid(fl)  CAL_CMGETAPI( "sil2copydataguid" ) 
	#define CAL_sil2copydataguid  sil2copydataguid
	#define CHK_sil2copydataguid  TRUE
	#define EXP_sil2copydataguid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copydataguid", (RTS_UINTPTR)sil2copydataguid, 1, 0x44F9CD8A, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2copydataguid  PFSIL2COPYDATAGUID_IEC pfsil2copydataguid;
	#define EXT_sil2copydataguid  extern PFSIL2COPYDATAGUID_IEC pfsil2copydataguid;
	#define GET_sil2copydataguid(fl)  s_pfCMGetAPI2( "sil2copydataguid", (RTS_VOID_FCTPTR *)&pfsil2copydataguid, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x44F9CD8A, 0x03050400)
	#define CAL_sil2copydataguid  pfsil2copydataguid
	#define CHK_sil2copydataguid  (pfsil2copydataguid != NULL)
	#define EXP_sil2copydataguid   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2copydataguid", (RTS_UINTPTR)sil2copydataguid, 1, 0x44F9CD8A, 0x03050400) 
#endif




/**
 * <description>
 * <p>Function to set Runtime to DebugMode</p>
 * <p>This function is called from save context after receiving an "EnterDebugMode" online service to set runtime to debug mode</p>
 * <p>As this function is only called from within save context no further check is required</p>
 * </description> 
 * <parampseudo name="SIL2OpMode" type="IN" range="[RTS_SIL2_OPMODE_SAFE,RTS_SIL2_OPMODE_DEBUG]">Specifies the current operation mode of the PLC</parampseudo>
 * <parampseudo name="SIL2PollOnlineService" type="IN" range="[TRUE,FALSE]">Specifies if the function PollOnlineServices is called or not</parampseudo>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Runtime could be set to Debugmode</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_CHANGE">Runtime was already set to Debugmode</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Runtime could not be set to Debugmode</errorcode>
 * <result>Result of Setting Mode</result>
 * <SIL2/>
 */
RTS_RESULT CDECL SIL2OEMEnterDebugMode(void);
typedef RTS_RESULT (CDECL * PFSIL2OEMENTERDEBUGMODE) (void);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMENTERDEBUGMODE_NOTIMPLEMENTED)
	#define USE_SIL2OEMEnterDebugMode
	#define EXT_SIL2OEMEnterDebugMode
	#define GET_SIL2OEMEnterDebugMode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2OEMEnterDebugMode()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SIL2OEMEnterDebugMode  FALSE
	#define EXP_SIL2OEMEnterDebugMode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2OEMEnterDebugMode
	#define EXT_SIL2OEMEnterDebugMode
	#define GET_SIL2OEMEnterDebugMode(fl)  CAL_CMGETAPI( "SIL2OEMEnterDebugMode" ) 
	#define CAL_SIL2OEMEnterDebugMode  SIL2OEMEnterDebugMode
	#define CHK_SIL2OEMEnterDebugMode  TRUE
	#define EXP_SIL2OEMEnterDebugMode  CAL_CMEXPAPI( "SIL2OEMEnterDebugMode" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2OEMEnterDebugMode
	#define EXT_SIL2OEMEnterDebugMode
	#define GET_SIL2OEMEnterDebugMode(fl)  CAL_CMGETAPI( "SIL2OEMEnterDebugMode" ) 
	#define CAL_SIL2OEMEnterDebugMode  SIL2OEMEnterDebugMode
	#define CHK_SIL2OEMEnterDebugMode  TRUE
	#define EXP_SIL2OEMEnterDebugMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMEnterDebugMode", (RTS_UINTPTR)SIL2OEMEnterDebugMode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2OEMEnterDebugMode
	#define EXT_CmpSIL2SIL2OEMEnterDebugMode
	#define GET_CmpSIL2SIL2OEMEnterDebugMode  ERR_OK
	#define CAL_CmpSIL2SIL2OEMEnterDebugMode pICmpSIL2->ISIL2OEMEnterDebugMode
	#define CHK_CmpSIL2SIL2OEMEnterDebugMode (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2OEMEnterDebugMode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2OEMEnterDebugMode
	#define EXT_SIL2OEMEnterDebugMode
	#define GET_SIL2OEMEnterDebugMode(fl)  CAL_CMGETAPI( "SIL2OEMEnterDebugMode" ) 
	#define CAL_SIL2OEMEnterDebugMode pICmpSIL2->ISIL2OEMEnterDebugMode
	#define CHK_SIL2OEMEnterDebugMode (pICmpSIL2 != NULL)
	#define EXP_SIL2OEMEnterDebugMode  CAL_CMEXPAPI( "SIL2OEMEnterDebugMode" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2OEMEnterDebugMode  PFSIL2OEMENTERDEBUGMODE pfSIL2OEMEnterDebugMode;
	#define EXT_SIL2OEMEnterDebugMode  extern PFSIL2OEMENTERDEBUGMODE pfSIL2OEMEnterDebugMode;
	#define GET_SIL2OEMEnterDebugMode(fl)  s_pfCMGetAPI2( "SIL2OEMEnterDebugMode", (RTS_VOID_FCTPTR *)&pfSIL2OEMEnterDebugMode, (fl), 0, 0)
	#define CAL_SIL2OEMEnterDebugMode  pfSIL2OEMEnterDebugMode
	#define CHK_SIL2OEMEnterDebugMode  (pfSIL2OEMEnterDebugMode != NULL)
	#define EXP_SIL2OEMEnterDebugMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMEnterDebugMode", (RTS_UINTPTR)SIL2OEMEnterDebugMode, 0, 0) 
#endif




/**
 * <description>sil2oemexception</description>
 * <SIL2/>
 */
typedef struct tagsil2oemexception_struct
{
	RTS_IEC_UDINT udiException;			/* VAR_INPUT */
	RTS_IEC_RESULT SIL2OEMException;	/* VAR_OUTPUT */
} sil2oemexception_struct;

/**
 * <description>
 * <p>Function to set Runtime into Exception-Mode</p> 
 * <p>Whenever the runtime detects invalid behaviour, values or states, it calls this function with a specific Exception Code.</p>
 * <p>Depending on the implementation this function may not return!</p>
 * </description> 
 * <param name="Exception" type="IN" range="[RTS_SIL2_EXCEPTION_LOADBOOTPROJECT,RTS_SIL2_EXCEPTION_BOOTPROJECT_INVALID,RTS_SIL2_EXCEPTION_COMPACTPREPARECODE,RTS_SIL2_EXCEPTION_COMPACTFINISH,RTS_SIL2_EXCEPTION_EXTREFS,RTS_SIL2_EXCEPTION_RUNAFTERDOWNLOAD,RTS_SIL2_EXCEPTION_MEMORY,RTS_SIL2_EXCEPTION_RUNTIME_INIT,RTS_SIL2_EXCEPTION_SCHED_WD,RTS_SIL2_EXCEPTION_CRC_CYCLIC,RTS_SIL2_EXCEPTION_CRC_COMPLETE,RTS_SIL2_EXCEPTION_CRC_INIT,RTS_SIL2_EXCEPTION_LIFE_COUNTER,RTS_SIL2_EXCEPTION_CALLERCONTEXT]">Exception Code</param>
 * <parampseudo name="bExceptionGenerated" type="OUT" range="[FALSE,TRUE]">TRUE, when an exception occured, during execution</parampseudo>  
 */
void CDECL SIL2OEMException(RTS_SIL2_EXCEPTION Exception);
typedef void (CDECL * PFSIL2OEMEXCEPTION) (RTS_SIL2_EXCEPTION Exception);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMEXCEPTION_NOTIMPLEMENTED)
	#define USE_SIL2OEMException
	#define EXT_SIL2OEMException
	#define GET_SIL2OEMException(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2OEMException(p0)  
	#define CHK_SIL2OEMException  FALSE
	#define EXP_SIL2OEMException  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2OEMException
	#define EXT_SIL2OEMException
	#define GET_SIL2OEMException(fl)  CAL_CMGETAPI( "SIL2OEMException" ) 
	#define CAL_SIL2OEMException  SIL2OEMException
	#define CHK_SIL2OEMException  TRUE
	#define EXP_SIL2OEMException  CAL_CMEXPAPI( "SIL2OEMException" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2OEMException
	#define EXT_SIL2OEMException
	#define GET_SIL2OEMException(fl)  CAL_CMGETAPI( "SIL2OEMException" ) 
	#define CAL_SIL2OEMException  SIL2OEMException
	#define CHK_SIL2OEMException  TRUE
	#define EXP_SIL2OEMException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMException", (RTS_UINTPTR)SIL2OEMException, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2OEMException
	#define EXT_CmpSIL2SIL2OEMException
	#define GET_CmpSIL2SIL2OEMException  ERR_OK
	#define CAL_CmpSIL2SIL2OEMException pICmpSIL2->ISIL2OEMException
	#define CHK_CmpSIL2SIL2OEMException (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2OEMException  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2OEMException
	#define EXT_SIL2OEMException
	#define GET_SIL2OEMException(fl)  CAL_CMGETAPI( "SIL2OEMException" ) 
	#define CAL_SIL2OEMException pICmpSIL2->ISIL2OEMException
	#define CHK_SIL2OEMException (pICmpSIL2 != NULL)
	#define EXP_SIL2OEMException  CAL_CMEXPAPI( "SIL2OEMException" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2OEMException  PFSIL2OEMEXCEPTION pfSIL2OEMException;
	#define EXT_SIL2OEMException  extern PFSIL2OEMEXCEPTION pfSIL2OEMException;
	#define GET_SIL2OEMException(fl)  s_pfCMGetAPI2( "SIL2OEMException", (RTS_VOID_FCTPTR *)&pfSIL2OEMException, (fl), 0, 0)
	#define CAL_SIL2OEMException  pfSIL2OEMException
	#define CHK_SIL2OEMException  (pfSIL2OEMException != NULL)
	#define EXP_SIL2OEMException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMException", (RTS_UINTPTR)SIL2OEMException, 0, 0) 
#endif



void CDECL CDECL_EXT sil2oemexception(sil2oemexception_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2OEMEXCEPTION_IEC) (sil2oemexception_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMEXCEPTION_NOTIMPLEMENTED)
	#define USE_sil2oemexception
	#define EXT_sil2oemexception
	#define GET_sil2oemexception(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2oemexception(p0) 
	#define CHK_sil2oemexception  FALSE
	#define EXP_sil2oemexception  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2oemexception
	#define EXT_sil2oemexception
	#define GET_sil2oemexception(fl)  CAL_CMGETAPI( "sil2oemexception" ) 
	#define CAL_sil2oemexception  sil2oemexception
	#define CHK_sil2oemexception  TRUE
	#define EXP_sil2oemexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemexception", (RTS_UINTPTR)sil2oemexception, 1, 0xA54830DE, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2oemexception
	#define EXT_sil2oemexception
	#define GET_sil2oemexception(fl)  CAL_CMGETAPI( "sil2oemexception" ) 
	#define CAL_sil2oemexception  sil2oemexception
	#define CHK_sil2oemexception  TRUE
	#define EXP_sil2oemexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemexception", (RTS_UINTPTR)sil2oemexception, 1, 0xA54830DE, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2oemexception
	#define EXT_CmpSIL2sil2oemexception
	#define GET_CmpSIL2sil2oemexception  ERR_OK
	#define CAL_CmpSIL2sil2oemexception  sil2oemexception
	#define CHK_CmpSIL2sil2oemexception  TRUE
	#define EXP_CmpSIL2sil2oemexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemexception", (RTS_UINTPTR)sil2oemexception, 1, 0xA54830DE, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2oemexception
	#define EXT_sil2oemexception
	#define GET_sil2oemexception(fl)  CAL_CMGETAPI( "sil2oemexception" ) 
	#define CAL_sil2oemexception  sil2oemexception
	#define CHK_sil2oemexception  TRUE
	#define EXP_sil2oemexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemexception", (RTS_UINTPTR)sil2oemexception, 1, 0xA54830DE, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2oemexception  PFSIL2OEMEXCEPTION_IEC pfsil2oemexception;
	#define EXT_sil2oemexception  extern PFSIL2OEMEXCEPTION_IEC pfsil2oemexception;
	#define GET_sil2oemexception(fl)  s_pfCMGetAPI2( "sil2oemexception", (RTS_VOID_FCTPTR *)&pfsil2oemexception, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xA54830DE, 0x03050400)
	#define CAL_sil2oemexception  pfsil2oemexception
	#define CHK_sil2oemexception  (pfsil2oemexception != NULL)
	#define EXP_sil2oemexception   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemexception", (RTS_UINTPTR)sil2oemexception, 1, 0xA54830DE, 0x03050400) 
#endif


/**
 * <description>sil2addlog</description>
 * <SIL2/>
 */
typedef struct tagsil2addlog_struct
{
	RTS_IEC_STRING sMessage[81];		/* VAR_INPUT */
	RTS_IEC_UDINT udiLogId;				/* VAR_INPUT */
	RTS_IEC_RESULT SIL2AddLog;			/* VAR_OUTPUT */
} sil2addlog_struct;

/**
 * <description>
 * <p>Function to add a secure Logentry</p> 
 * <p>These secure logentries don't use the standard Logging mechanism! With this function it is possible to add secure messages to a Messagequeue within CmpSIL2.
 *    This Messagequeue can be fetched by an Onlineservice from the SIL2 Programmingsystemplugin. The messages are stored and transmitted with a CRC to provide a secure logging.
 *    The maximal length of the string is restricted (CMPSIL2_LOGADD_MAX_STRLEN). 
 * </p>
 *
 * </description> 
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Entry could not be added to log</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Message pointer was NULL, or message was longer than CMPSIL2_LOGADD_MAX_STRLEN</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Entry could be added to log correctly</errorcode> 
 * <param name="pszMessage" type="IN" range="[NULL,VALID_STRING,LONG_STRING]">String containing the message, must not be longer than CMPSIL2_LOGADD_MAX_STRLEN</param>
 * <param name="ulLogId" type="IN" range="[RTS_RANGE_OF_ULONG]">Id for own Identification purposes</param>
 * <result>Result of adding secure Logentry</result> 
 */
RTS_RESULT CDECL SIL2AddLog(char* pszMessage,RTS_UI32 ulLogId);
typedef RTS_RESULT (CDECL * PFSIL2ADDLOG) (char* pszMessage,RTS_UI32 ulLogId);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2ADDLOG_NOTIMPLEMENTED)
	#define USE_SIL2AddLog
	#define EXT_SIL2AddLog
	#define GET_SIL2AddLog(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2AddLog(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SIL2AddLog  FALSE
	#define EXP_SIL2AddLog  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2AddLog
	#define EXT_SIL2AddLog
	#define GET_SIL2AddLog(fl)  CAL_CMGETAPI( "SIL2AddLog" ) 
	#define CAL_SIL2AddLog  SIL2AddLog
	#define CHK_SIL2AddLog  TRUE
	#define EXP_SIL2AddLog  CAL_CMEXPAPI( "SIL2AddLog" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2AddLog
	#define EXT_SIL2AddLog
	#define GET_SIL2AddLog(fl)  CAL_CMGETAPI( "SIL2AddLog" ) 
	#define CAL_SIL2AddLog  SIL2AddLog
	#define CHK_SIL2AddLog  TRUE
	#define EXP_SIL2AddLog  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2AddLog", (RTS_UINTPTR)SIL2AddLog, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2AddLog
	#define EXT_CmpSIL2SIL2AddLog
	#define GET_CmpSIL2SIL2AddLog  ERR_OK
	#define CAL_CmpSIL2SIL2AddLog pICmpSIL2->ISIL2AddLog
	#define CHK_CmpSIL2SIL2AddLog (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2AddLog  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2AddLog
	#define EXT_SIL2AddLog
	#define GET_SIL2AddLog(fl)  CAL_CMGETAPI( "SIL2AddLog" ) 
	#define CAL_SIL2AddLog pICmpSIL2->ISIL2AddLog
	#define CHK_SIL2AddLog (pICmpSIL2 != NULL)
	#define EXP_SIL2AddLog  CAL_CMEXPAPI( "SIL2AddLog" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2AddLog  PFSIL2ADDLOG pfSIL2AddLog;
	#define EXT_SIL2AddLog  extern PFSIL2ADDLOG pfSIL2AddLog;
	#define GET_SIL2AddLog(fl)  s_pfCMGetAPI2( "SIL2AddLog", (RTS_VOID_FCTPTR *)&pfSIL2AddLog, (fl), 0, 0)
	#define CAL_SIL2AddLog  pfSIL2AddLog
	#define CHK_SIL2AddLog  (pfSIL2AddLog != NULL)
	#define EXP_SIL2AddLog  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2AddLog", (RTS_UINTPTR)SIL2AddLog, 0, 0) 
#endif



void CDECL CDECL_EXT sil2addlog(sil2addlog_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2ADDLOG_IEC) (sil2addlog_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2ADDLOG_NOTIMPLEMENTED)
	#define USE_sil2addlog
	#define EXT_sil2addlog
	#define GET_sil2addlog(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2addlog(p0) 
	#define CHK_sil2addlog  FALSE
	#define EXP_sil2addlog  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2addlog
	#define EXT_sil2addlog
	#define GET_sil2addlog(fl)  CAL_CMGETAPI( "sil2addlog" ) 
	#define CAL_sil2addlog  sil2addlog
	#define CHK_sil2addlog  TRUE
	#define EXP_sil2addlog  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2addlog", (RTS_UINTPTR)sil2addlog, 1, 0x742BC625, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2addlog
	#define EXT_sil2addlog
	#define GET_sil2addlog(fl)  CAL_CMGETAPI( "sil2addlog" ) 
	#define CAL_sil2addlog  sil2addlog
	#define CHK_sil2addlog  TRUE
	#define EXP_sil2addlog  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2addlog", (RTS_UINTPTR)sil2addlog, 1, 0x742BC625, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2addlog
	#define EXT_CmpSIL2sil2addlog
	#define GET_CmpSIL2sil2addlog  ERR_OK
	#define CAL_CmpSIL2sil2addlog  sil2addlog
	#define CHK_CmpSIL2sil2addlog  TRUE
	#define EXP_CmpSIL2sil2addlog  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2addlog", (RTS_UINTPTR)sil2addlog, 1, 0x742BC625, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2addlog
	#define EXT_sil2addlog
	#define GET_sil2addlog(fl)  CAL_CMGETAPI( "sil2addlog" ) 
	#define CAL_sil2addlog  sil2addlog
	#define CHK_sil2addlog  TRUE
	#define EXP_sil2addlog  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2addlog", (RTS_UINTPTR)sil2addlog, 1, 0x742BC625, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2addlog  PFSIL2ADDLOG_IEC pfsil2addlog;
	#define EXT_sil2addlog  extern PFSIL2ADDLOG_IEC pfsil2addlog;
	#define GET_sil2addlog(fl)  s_pfCMGetAPI2( "sil2addlog", (RTS_VOID_FCTPTR *)&pfsil2addlog, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x742BC625, 0x03050400)
	#define CAL_sil2addlog  pfsil2addlog
	#define CHK_sil2addlog  (pfsil2addlog != NULL)
	#define EXP_sil2addlog   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2addlog", (RTS_UINTPTR)sil2addlog, 1, 0x742BC625, 0x03050400) 
#endif


/**
 * <description>sil2oemgetoperationmode</description>
 * <SIL2/>
 */
typedef struct tagsil2oemgetoperationmode_struct
{
	RTS_IEC_UDINT SIL2OEMGetOperationMode;	/* VAR_OUTPUT */
} sil2oemgetoperationmode_struct;

/**
 * <description>
 * <p>Function to get the Operationmode of the Runtime</p>
 * <p>Returns RTS_SIL2_OPMODE_DEBUG or RTS_SIL2_OPMODE_SAFE depending on Operationmode, returns RTS_SIL2_OPMODE_ERROR if error occured or if in unknown state</p>
 * </description> 
 * <parampseudo name="SIL2OpMode" type="IN" range="[RTS_SIL2_OPMODE_SAFE,RTS_SIL2_OPMODE_DEBUG]">Specifies the current operation mode of the PLC</parampseudo>
 * <result>Returns SIL2 Operation Mode: RTS_SIL2_OPMODE_DEBUG or RTS_SIL2_OPMODE_SAFE if operation was successful, RTS_SIL2_OPMODE_ERROR if error occured or if in unknown state!</result>
 */
RTS_SIL2_OPMODE CDECL SIL2OEMGetOperationMode(void);
typedef RTS_SIL2_OPMODE (CDECL * PFSIL2OEMGETOPERATIONMODE) (void);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMGETOPERATIONMODE_NOTIMPLEMENTED)
	#define USE_SIL2OEMGetOperationMode
	#define EXT_SIL2OEMGetOperationMode
	#define GET_SIL2OEMGetOperationMode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2OEMGetOperationMode()  (RTS_SIL2_OPMODE)ERR_NOTIMPLEMENTED
	#define CHK_SIL2OEMGetOperationMode  FALSE
	#define EXP_SIL2OEMGetOperationMode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2OEMGetOperationMode
	#define EXT_SIL2OEMGetOperationMode
	#define GET_SIL2OEMGetOperationMode(fl)  CAL_CMGETAPI( "SIL2OEMGetOperationMode" ) 
	#define CAL_SIL2OEMGetOperationMode  SIL2OEMGetOperationMode
	#define CHK_SIL2OEMGetOperationMode  TRUE
	#define EXP_SIL2OEMGetOperationMode  CAL_CMEXPAPI( "SIL2OEMGetOperationMode" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2OEMGetOperationMode
	#define EXT_SIL2OEMGetOperationMode
	#define GET_SIL2OEMGetOperationMode(fl)  CAL_CMGETAPI( "SIL2OEMGetOperationMode" ) 
	#define CAL_SIL2OEMGetOperationMode  SIL2OEMGetOperationMode
	#define CHK_SIL2OEMGetOperationMode  TRUE
	#define EXP_SIL2OEMGetOperationMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMGetOperationMode", (RTS_UINTPTR)SIL2OEMGetOperationMode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2OEMGetOperationMode
	#define EXT_CmpSIL2SIL2OEMGetOperationMode
	#define GET_CmpSIL2SIL2OEMGetOperationMode  ERR_OK
	#define CAL_CmpSIL2SIL2OEMGetOperationMode pICmpSIL2->ISIL2OEMGetOperationMode
	#define CHK_CmpSIL2SIL2OEMGetOperationMode (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2OEMGetOperationMode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2OEMGetOperationMode
	#define EXT_SIL2OEMGetOperationMode
	#define GET_SIL2OEMGetOperationMode(fl)  CAL_CMGETAPI( "SIL2OEMGetOperationMode" ) 
	#define CAL_SIL2OEMGetOperationMode pICmpSIL2->ISIL2OEMGetOperationMode
	#define CHK_SIL2OEMGetOperationMode (pICmpSIL2 != NULL)
	#define EXP_SIL2OEMGetOperationMode  CAL_CMEXPAPI( "SIL2OEMGetOperationMode" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2OEMGetOperationMode  PFSIL2OEMGETOPERATIONMODE pfSIL2OEMGetOperationMode;
	#define EXT_SIL2OEMGetOperationMode  extern PFSIL2OEMGETOPERATIONMODE pfSIL2OEMGetOperationMode;
	#define GET_SIL2OEMGetOperationMode(fl)  s_pfCMGetAPI2( "SIL2OEMGetOperationMode", (RTS_VOID_FCTPTR *)&pfSIL2OEMGetOperationMode, (fl), 0, 0)
	#define CAL_SIL2OEMGetOperationMode  pfSIL2OEMGetOperationMode
	#define CHK_SIL2OEMGetOperationMode  (pfSIL2OEMGetOperationMode != NULL)
	#define EXP_SIL2OEMGetOperationMode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMGetOperationMode", (RTS_UINTPTR)SIL2OEMGetOperationMode, 0, 0) 
#endif



void CDECL CDECL_EXT sil2oemgetoperationmode(sil2oemgetoperationmode_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2OEMGETOPERATIONMODE_IEC) (sil2oemgetoperationmode_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMGETOPERATIONMODE_NOTIMPLEMENTED)
	#define USE_sil2oemgetoperationmode
	#define EXT_sil2oemgetoperationmode
	#define GET_sil2oemgetoperationmode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2oemgetoperationmode(p0) 
	#define CHK_sil2oemgetoperationmode  FALSE
	#define EXP_sil2oemgetoperationmode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2oemgetoperationmode
	#define EXT_sil2oemgetoperationmode
	#define GET_sil2oemgetoperationmode(fl)  CAL_CMGETAPI( "sil2oemgetoperationmode" ) 
	#define CAL_sil2oemgetoperationmode  sil2oemgetoperationmode
	#define CHK_sil2oemgetoperationmode  TRUE
	#define EXP_sil2oemgetoperationmode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetoperationmode", (RTS_UINTPTR)sil2oemgetoperationmode, 1, 0xB292F534, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2oemgetoperationmode
	#define EXT_sil2oemgetoperationmode
	#define GET_sil2oemgetoperationmode(fl)  CAL_CMGETAPI( "sil2oemgetoperationmode" ) 
	#define CAL_sil2oemgetoperationmode  sil2oemgetoperationmode
	#define CHK_sil2oemgetoperationmode  TRUE
	#define EXP_sil2oemgetoperationmode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetoperationmode", (RTS_UINTPTR)sil2oemgetoperationmode, 1, 0xB292F534, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2oemgetoperationmode
	#define EXT_CmpSIL2sil2oemgetoperationmode
	#define GET_CmpSIL2sil2oemgetoperationmode  ERR_OK
	#define CAL_CmpSIL2sil2oemgetoperationmode  sil2oemgetoperationmode
	#define CHK_CmpSIL2sil2oemgetoperationmode  TRUE
	#define EXP_CmpSIL2sil2oemgetoperationmode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetoperationmode", (RTS_UINTPTR)sil2oemgetoperationmode, 1, 0xB292F534, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2oemgetoperationmode
	#define EXT_sil2oemgetoperationmode
	#define GET_sil2oemgetoperationmode(fl)  CAL_CMGETAPI( "sil2oemgetoperationmode" ) 
	#define CAL_sil2oemgetoperationmode  sil2oemgetoperationmode
	#define CHK_sil2oemgetoperationmode  TRUE
	#define EXP_sil2oemgetoperationmode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetoperationmode", (RTS_UINTPTR)sil2oemgetoperationmode, 1, 0xB292F534, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2oemgetoperationmode  PFSIL2OEMGETOPERATIONMODE_IEC pfsil2oemgetoperationmode;
	#define EXT_sil2oemgetoperationmode  extern PFSIL2OEMGETOPERATIONMODE_IEC pfsil2oemgetoperationmode;
	#define GET_sil2oemgetoperationmode(fl)  s_pfCMGetAPI2( "sil2oemgetoperationmode", (RTS_VOID_FCTPTR *)&pfsil2oemgetoperationmode, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xB292F534, 0x03050400)
	#define CAL_sil2oemgetoperationmode  pfsil2oemgetoperationmode
	#define CHK_sil2oemgetoperationmode  (pfsil2oemgetoperationmode != NULL)
	#define EXP_sil2oemgetoperationmode   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetoperationmode", (RTS_UINTPTR)sil2oemgetoperationmode, 1, 0xB292F534, 0x03050400) 
#endif



/**
 * <description>sil2oemgetcallercontext</description>
 * <SIL2/>
 */
typedef struct tagsil2oemgetcallercontext_struct
{
	RTS_IEC_UDINT SIL2OEMGetCallerContext;	/* VAR_OUTPUT */
} sil2oemgetcallercontext_struct;

/**
 * <description>
 * <p>Function to get the current Caller Context of the Runtime</p>
 * <p>Expected return the current context, and an exception is thrown if they are different.</p>
 * </description> 
 * <parampseudo name="bDebugMode" type="IN" range="[TRUE,FALSE]">Specifies if run in debug mode or not.</parampseudo>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Context is ok</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Context could not be checked successfully, an internal error occured.</errorcode>
 * <result>Returns if comparision was ok or not.</result>
 */                                                                                                                                                                                             
RTS_UI32 CDECL SIL2OEMGetCallerContext(void);
typedef RTS_UI32 (CDECL * PFSIL2OEMGETCALLERCONTEXT) (void);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMGETCALLERCONTEXT_NOTIMPLEMENTED)
	#define USE_SIL2OEMGetCallerContext
	#define EXT_SIL2OEMGetCallerContext
	#define GET_SIL2OEMGetCallerContext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2OEMGetCallerContext()  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SIL2OEMGetCallerContext  FALSE
	#define EXP_SIL2OEMGetCallerContext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2OEMGetCallerContext
	#define EXT_SIL2OEMGetCallerContext
	#define GET_SIL2OEMGetCallerContext(fl)  CAL_CMGETAPI( "SIL2OEMGetCallerContext" ) 
	#define CAL_SIL2OEMGetCallerContext  SIL2OEMGetCallerContext
	#define CHK_SIL2OEMGetCallerContext  TRUE
	#define EXP_SIL2OEMGetCallerContext  CAL_CMEXPAPI( "SIL2OEMGetCallerContext" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2OEMGetCallerContext
	#define EXT_SIL2OEMGetCallerContext
	#define GET_SIL2OEMGetCallerContext(fl)  CAL_CMGETAPI( "SIL2OEMGetCallerContext" ) 
	#define CAL_SIL2OEMGetCallerContext  SIL2OEMGetCallerContext
	#define CHK_SIL2OEMGetCallerContext  TRUE
	#define EXP_SIL2OEMGetCallerContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMGetCallerContext", (RTS_UINTPTR)SIL2OEMGetCallerContext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2OEMGetCallerContext
	#define EXT_CmpSIL2SIL2OEMGetCallerContext
	#define GET_CmpSIL2SIL2OEMGetCallerContext  ERR_OK
	#define CAL_CmpSIL2SIL2OEMGetCallerContext pICmpSIL2->ISIL2OEMGetCallerContext
	#define CHK_CmpSIL2SIL2OEMGetCallerContext (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2OEMGetCallerContext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2OEMGetCallerContext
	#define EXT_SIL2OEMGetCallerContext
	#define GET_SIL2OEMGetCallerContext(fl)  CAL_CMGETAPI( "SIL2OEMGetCallerContext" ) 
	#define CAL_SIL2OEMGetCallerContext pICmpSIL2->ISIL2OEMGetCallerContext
	#define CHK_SIL2OEMGetCallerContext (pICmpSIL2 != NULL)
	#define EXP_SIL2OEMGetCallerContext  CAL_CMEXPAPI( "SIL2OEMGetCallerContext" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2OEMGetCallerContext  PFSIL2OEMGETCALLERCONTEXT pfSIL2OEMGetCallerContext;
	#define EXT_SIL2OEMGetCallerContext  extern PFSIL2OEMGETCALLERCONTEXT pfSIL2OEMGetCallerContext;
	#define GET_SIL2OEMGetCallerContext(fl)  s_pfCMGetAPI2( "SIL2OEMGetCallerContext", (RTS_VOID_FCTPTR *)&pfSIL2OEMGetCallerContext, (fl), 0, 0)
	#define CAL_SIL2OEMGetCallerContext  pfSIL2OEMGetCallerContext
	#define CHK_SIL2OEMGetCallerContext  (pfSIL2OEMGetCallerContext != NULL)
	#define EXP_SIL2OEMGetCallerContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMGetCallerContext", (RTS_UINTPTR)SIL2OEMGetCallerContext, 0, 0) 
#endif



void CDECL CDECL_EXT sil2oemgetcallercontext(sil2oemgetcallercontext_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2OEMGETCALLERCONTEXT_IEC) (sil2oemgetcallercontext_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMGETCALLERCONTEXT_NOTIMPLEMENTED)
	#define USE_sil2oemgetcallercontext
	#define EXT_sil2oemgetcallercontext
	#define GET_sil2oemgetcallercontext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2oemgetcallercontext(p0) 
	#define CHK_sil2oemgetcallercontext  FALSE
	#define EXP_sil2oemgetcallercontext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2oemgetcallercontext
	#define EXT_sil2oemgetcallercontext
	#define GET_sil2oemgetcallercontext(fl)  CAL_CMGETAPI( "sil2oemgetcallercontext" ) 
	#define CAL_sil2oemgetcallercontext  sil2oemgetcallercontext
	#define CHK_sil2oemgetcallercontext  TRUE
	#define EXP_sil2oemgetcallercontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetcallercontext", (RTS_UINTPTR)sil2oemgetcallercontext, 1, 0x2A37A2FD, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2oemgetcallercontext
	#define EXT_sil2oemgetcallercontext
	#define GET_sil2oemgetcallercontext(fl)  CAL_CMGETAPI( "sil2oemgetcallercontext" ) 
	#define CAL_sil2oemgetcallercontext  sil2oemgetcallercontext
	#define CHK_sil2oemgetcallercontext  TRUE
	#define EXP_sil2oemgetcallercontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetcallercontext", (RTS_UINTPTR)sil2oemgetcallercontext, 1, 0x2A37A2FD, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2oemgetcallercontext
	#define EXT_CmpSIL2sil2oemgetcallercontext
	#define GET_CmpSIL2sil2oemgetcallercontext  ERR_OK
	#define CAL_CmpSIL2sil2oemgetcallercontext  sil2oemgetcallercontext
	#define CHK_CmpSIL2sil2oemgetcallercontext  TRUE
	#define EXP_CmpSIL2sil2oemgetcallercontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetcallercontext", (RTS_UINTPTR)sil2oemgetcallercontext, 1, 0x2A37A2FD, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2oemgetcallercontext
	#define EXT_sil2oemgetcallercontext
	#define GET_sil2oemgetcallercontext(fl)  CAL_CMGETAPI( "sil2oemgetcallercontext" ) 
	#define CAL_sil2oemgetcallercontext  sil2oemgetcallercontext
	#define CHK_sil2oemgetcallercontext  TRUE
	#define EXP_sil2oemgetcallercontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetcallercontext", (RTS_UINTPTR)sil2oemgetcallercontext, 1, 0x2A37A2FD, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2oemgetcallercontext  PFSIL2OEMGETCALLERCONTEXT_IEC pfsil2oemgetcallercontext;
	#define EXT_sil2oemgetcallercontext  extern PFSIL2OEMGETCALLERCONTEXT_IEC pfsil2oemgetcallercontext;
	#define GET_sil2oemgetcallercontext(fl)  s_pfCMGetAPI2( "sil2oemgetcallercontext", (RTS_VOID_FCTPTR *)&pfsil2oemgetcallercontext, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x2A37A2FD, 0x03050400)
	#define CAL_sil2oemgetcallercontext  pfsil2oemgetcallercontext
	#define CHK_sil2oemgetcallercontext  (pfsil2oemgetcallercontext != NULL)
	#define EXP_sil2oemgetcallercontext   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetcallercontext", (RTS_UINTPTR)sil2oemgetcallercontext, 1, 0x2A37A2FD, 0x03050400) 
#endif


/**
 * <description>sil2oemgetmemorystate</description>
 * <SIL2/>
 */
typedef struct tagsil2oemgetmemorystate_struct
{
	RTS_IEC_BYTE *pAddress;				/* VAR_INPUT */
	RTS_IEC_UDINT udiLength;			/* VAR_INPUT */
	RTS_IEC_UDINT SIL2OEMGetMemoryState;	/* VAR_OUTPUT */
} sil2oemgetmemorystate_struct;

/**
 * <description>
 * <p>Function to get the MemoryState (safe/unsafe) for a specific Memoryrange </p>
 * <p>The Addressrange where pAddress points to with the length of ulLength is checked and the corresponding RTS_SIL2_ADDRESSSTATE is returned: RTS_SIL2_ADDRESS_SAFE or RTS_SIL2_ADDRESS_UNSAFE</p>
 * </description> 
 * <param name="pAddress" type="IN" range="[NULL,SAFE_POINTER,UNSAFE_POINTER]">Pointer to Addressrange to check for Addressstate</param>
 * <param name="ulLength" type="IN" range="[0,VALID_POINTER_SIZE,ULONG_MAX]">Length of Addressrange to check for Addressstate</param>
 * <errorcode name="RTS_SIL2_ADDRESSSTATE Result" type="RTS_SIL2_ADDRESS_SAFE">This Addressrange is in Safe Range</errorcode>
 * <errorcode name="RTS_SIL2_ADDRESSSTATE Result" type="RTS_SIL2_ADDRESS_UNSAFE">This Addressrange is in Unsafe Range</errorcode>
 * <result>Result for Memoryaddress check</result>
 */
RTS_SIL2_ADDRESSSTATE CDECL SIL2OEMGetMemoryState(RTS_UI8 *pAddress, RTS_UI32 ulLength);
typedef RTS_SIL2_ADDRESSSTATE (CDECL * PFSIL2OEMGETMEMORYSTATE) (RTS_UI8 *pAddress, RTS_UI32 ulLength);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMGETMEMORYSTATE_NOTIMPLEMENTED)
	#define USE_SIL2OEMGetMemoryState
	#define EXT_SIL2OEMGetMemoryState
	#define GET_SIL2OEMGetMemoryState(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2OEMGetMemoryState(p0,p1)  (RTS_SIL2_ADDRESSSTATE)ERR_NOTIMPLEMENTED
	#define CHK_SIL2OEMGetMemoryState  FALSE
	#define EXP_SIL2OEMGetMemoryState  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2OEMGetMemoryState
	#define EXT_SIL2OEMGetMemoryState
	#define GET_SIL2OEMGetMemoryState(fl)  CAL_CMGETAPI( "SIL2OEMGetMemoryState" ) 
	#define CAL_SIL2OEMGetMemoryState  SIL2OEMGetMemoryState
	#define CHK_SIL2OEMGetMemoryState  TRUE
	#define EXP_SIL2OEMGetMemoryState  CAL_CMEXPAPI( "SIL2OEMGetMemoryState" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2OEMGetMemoryState
	#define EXT_SIL2OEMGetMemoryState
	#define GET_SIL2OEMGetMemoryState(fl)  CAL_CMGETAPI( "SIL2OEMGetMemoryState" ) 
	#define CAL_SIL2OEMGetMemoryState  SIL2OEMGetMemoryState
	#define CHK_SIL2OEMGetMemoryState  TRUE
	#define EXP_SIL2OEMGetMemoryState  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMGetMemoryState", (RTS_UINTPTR)SIL2OEMGetMemoryState, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2OEMGetMemoryState
	#define EXT_CmpSIL2SIL2OEMGetMemoryState
	#define GET_CmpSIL2SIL2OEMGetMemoryState  ERR_OK
	#define CAL_CmpSIL2SIL2OEMGetMemoryState pICmpSIL2->ISIL2OEMGetMemoryState
	#define CHK_CmpSIL2SIL2OEMGetMemoryState (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2OEMGetMemoryState  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2OEMGetMemoryState
	#define EXT_SIL2OEMGetMemoryState
	#define GET_SIL2OEMGetMemoryState(fl)  CAL_CMGETAPI( "SIL2OEMGetMemoryState" ) 
	#define CAL_SIL2OEMGetMemoryState pICmpSIL2->ISIL2OEMGetMemoryState
	#define CHK_SIL2OEMGetMemoryState (pICmpSIL2 != NULL)
	#define EXP_SIL2OEMGetMemoryState  CAL_CMEXPAPI( "SIL2OEMGetMemoryState" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2OEMGetMemoryState  PFSIL2OEMGETMEMORYSTATE pfSIL2OEMGetMemoryState;
	#define EXT_SIL2OEMGetMemoryState  extern PFSIL2OEMGETMEMORYSTATE pfSIL2OEMGetMemoryState;
	#define GET_SIL2OEMGetMemoryState(fl)  s_pfCMGetAPI2( "SIL2OEMGetMemoryState", (RTS_VOID_FCTPTR *)&pfSIL2OEMGetMemoryState, (fl), 0, 0)
	#define CAL_SIL2OEMGetMemoryState  pfSIL2OEMGetMemoryState
	#define CHK_SIL2OEMGetMemoryState  (pfSIL2OEMGetMemoryState != NULL)
	#define EXP_SIL2OEMGetMemoryState  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMGetMemoryState", (RTS_UINTPTR)SIL2OEMGetMemoryState, 0, 0) 
#endif



void CDECL CDECL_EXT sil2oemgetmemorystate(sil2oemgetmemorystate_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2OEMGETMEMORYSTATE_IEC) (sil2oemgetmemorystate_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMGETMEMORYSTATE_NOTIMPLEMENTED)
	#define USE_sil2oemgetmemorystate
	#define EXT_sil2oemgetmemorystate
	#define GET_sil2oemgetmemorystate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2oemgetmemorystate(p0) 
	#define CHK_sil2oemgetmemorystate  FALSE
	#define EXP_sil2oemgetmemorystate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2oemgetmemorystate
	#define EXT_sil2oemgetmemorystate
	#define GET_sil2oemgetmemorystate(fl)  CAL_CMGETAPI( "sil2oemgetmemorystate" ) 
	#define CAL_sil2oemgetmemorystate  sil2oemgetmemorystate
	#define CHK_sil2oemgetmemorystate  TRUE
	#define EXP_sil2oemgetmemorystate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetmemorystate", (RTS_UINTPTR)sil2oemgetmemorystate, 1, 0x2FE5CB14, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2oemgetmemorystate
	#define EXT_sil2oemgetmemorystate
	#define GET_sil2oemgetmemorystate(fl)  CAL_CMGETAPI( "sil2oemgetmemorystate" ) 
	#define CAL_sil2oemgetmemorystate  sil2oemgetmemorystate
	#define CHK_sil2oemgetmemorystate  TRUE
	#define EXP_sil2oemgetmemorystate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetmemorystate", (RTS_UINTPTR)sil2oemgetmemorystate, 1, 0x2FE5CB14, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2oemgetmemorystate
	#define EXT_CmpSIL2sil2oemgetmemorystate
	#define GET_CmpSIL2sil2oemgetmemorystate  ERR_OK
	#define CAL_CmpSIL2sil2oemgetmemorystate  sil2oemgetmemorystate
	#define CHK_CmpSIL2sil2oemgetmemorystate  TRUE
	#define EXP_CmpSIL2sil2oemgetmemorystate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetmemorystate", (RTS_UINTPTR)sil2oemgetmemorystate, 1, 0x2FE5CB14, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2oemgetmemorystate
	#define EXT_sil2oemgetmemorystate
	#define GET_sil2oemgetmemorystate(fl)  CAL_CMGETAPI( "sil2oemgetmemorystate" ) 
	#define CAL_sil2oemgetmemorystate  sil2oemgetmemorystate
	#define CHK_sil2oemgetmemorystate  TRUE
	#define EXP_sil2oemgetmemorystate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetmemorystate", (RTS_UINTPTR)sil2oemgetmemorystate, 1, 0x2FE5CB14, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2oemgetmemorystate  PFSIL2OEMGETMEMORYSTATE_IEC pfsil2oemgetmemorystate;
	#define EXT_sil2oemgetmemorystate  extern PFSIL2OEMGETMEMORYSTATE_IEC pfsil2oemgetmemorystate;
	#define GET_sil2oemgetmemorystate(fl)  s_pfCMGetAPI2( "sil2oemgetmemorystate", (RTS_VOID_FCTPTR *)&pfsil2oemgetmemorystate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x2FE5CB14, 0x03050400)
	#define CAL_sil2oemgetmemorystate  pfsil2oemgetmemorystate
	#define CHK_sil2oemgetmemorystate  (pfsil2oemgetmemorystate != NULL)
	#define EXP_sil2oemgetmemorystate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemgetmemorystate", (RTS_UINTPTR)sil2oemgetmemorystate, 1, 0x2FE5CB14, 0x03050400) 
#endif


/**
 * <description>sil2oemstackisvalid</description>
 * <SIL2/>
 */
typedef struct tagsil2oemstackisvalid_struct
{
	RTS_IEC_RESULT SIL2OEMStackIsValid;	/* VAR_OUTPUT */
} sil2oemstackisvalid_struct;

/**
 * <description>
 * <p>Function to check if Stack is Valid</p>
 * <p>This function is called before entering the Safemode, it returns ERR_OK if the stack is valid, and ERR_FAILED if an error occured or was detected!</p>
 * </description> 
 * <parampseudo name="bStackIsValid" type="IN" range="[FALSE,TRUE]">Define if the current stack is valid.</parampseudo>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Check stack was successful</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Check stack was not successful</errorcode>
 * <result>Result of Stackcheck</result>
 */
RTS_RESULT CDECL SIL2OEMStackIsValid(void);
typedef RTS_RESULT (CDECL * PFSIL2OEMSTACKISVALID) (void);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMSTACKISVALID_NOTIMPLEMENTED)
	#define USE_SIL2OEMStackIsValid
	#define EXT_SIL2OEMStackIsValid
	#define GET_SIL2OEMStackIsValid(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2OEMStackIsValid()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SIL2OEMStackIsValid  FALSE
	#define EXP_SIL2OEMStackIsValid  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2OEMStackIsValid
	#define EXT_SIL2OEMStackIsValid
	#define GET_SIL2OEMStackIsValid(fl)  CAL_CMGETAPI( "SIL2OEMStackIsValid" ) 
	#define CAL_SIL2OEMStackIsValid  SIL2OEMStackIsValid
	#define CHK_SIL2OEMStackIsValid  TRUE
	#define EXP_SIL2OEMStackIsValid  CAL_CMEXPAPI( "SIL2OEMStackIsValid" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2OEMStackIsValid
	#define EXT_SIL2OEMStackIsValid
	#define GET_SIL2OEMStackIsValid(fl)  CAL_CMGETAPI( "SIL2OEMStackIsValid" ) 
	#define CAL_SIL2OEMStackIsValid  SIL2OEMStackIsValid
	#define CHK_SIL2OEMStackIsValid  TRUE
	#define EXP_SIL2OEMStackIsValid  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMStackIsValid", (RTS_UINTPTR)SIL2OEMStackIsValid, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2OEMStackIsValid
	#define EXT_CmpSIL2SIL2OEMStackIsValid
	#define GET_CmpSIL2SIL2OEMStackIsValid  ERR_OK
	#define CAL_CmpSIL2SIL2OEMStackIsValid pICmpSIL2->ISIL2OEMStackIsValid
	#define CHK_CmpSIL2SIL2OEMStackIsValid (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2OEMStackIsValid  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2OEMStackIsValid
	#define EXT_SIL2OEMStackIsValid
	#define GET_SIL2OEMStackIsValid(fl)  CAL_CMGETAPI( "SIL2OEMStackIsValid" ) 
	#define CAL_SIL2OEMStackIsValid pICmpSIL2->ISIL2OEMStackIsValid
	#define CHK_SIL2OEMStackIsValid (pICmpSIL2 != NULL)
	#define EXP_SIL2OEMStackIsValid  CAL_CMEXPAPI( "SIL2OEMStackIsValid" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2OEMStackIsValid  PFSIL2OEMSTACKISVALID pfSIL2OEMStackIsValid;
	#define EXT_SIL2OEMStackIsValid  extern PFSIL2OEMSTACKISVALID pfSIL2OEMStackIsValid;
	#define GET_SIL2OEMStackIsValid(fl)  s_pfCMGetAPI2( "SIL2OEMStackIsValid", (RTS_VOID_FCTPTR *)&pfSIL2OEMStackIsValid, (fl), 0, 0)
	#define CAL_SIL2OEMStackIsValid  pfSIL2OEMStackIsValid
	#define CHK_SIL2OEMStackIsValid  (pfSIL2OEMStackIsValid != NULL)
	#define EXP_SIL2OEMStackIsValid  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMStackIsValid", (RTS_UINTPTR)SIL2OEMStackIsValid, 0, 0) 
#endif



void CDECL CDECL_EXT sil2oemstackisvalid(sil2oemstackisvalid_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2OEMSTACKISVALID_IEC) (sil2oemstackisvalid_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMSTACKISVALID_NOTIMPLEMENTED)
	#define USE_sil2oemstackisvalid
	#define EXT_sil2oemstackisvalid
	#define GET_sil2oemstackisvalid(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2oemstackisvalid(p0) 
	#define CHK_sil2oemstackisvalid  FALSE
	#define EXP_sil2oemstackisvalid  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2oemstackisvalid
	#define EXT_sil2oemstackisvalid
	#define GET_sil2oemstackisvalid(fl)  CAL_CMGETAPI( "sil2oemstackisvalid" ) 
	#define CAL_sil2oemstackisvalid  sil2oemstackisvalid
	#define CHK_sil2oemstackisvalid  TRUE
	#define EXP_sil2oemstackisvalid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemstackisvalid", (RTS_UINTPTR)sil2oemstackisvalid, 1, 0x56A27207, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2oemstackisvalid
	#define EXT_sil2oemstackisvalid
	#define GET_sil2oemstackisvalid(fl)  CAL_CMGETAPI( "sil2oemstackisvalid" ) 
	#define CAL_sil2oemstackisvalid  sil2oemstackisvalid
	#define CHK_sil2oemstackisvalid  TRUE
	#define EXP_sil2oemstackisvalid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemstackisvalid", (RTS_UINTPTR)sil2oemstackisvalid, 1, 0x56A27207, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2oemstackisvalid
	#define EXT_CmpSIL2sil2oemstackisvalid
	#define GET_CmpSIL2sil2oemstackisvalid  ERR_OK
	#define CAL_CmpSIL2sil2oemstackisvalid  sil2oemstackisvalid
	#define CHK_CmpSIL2sil2oemstackisvalid  TRUE
	#define EXP_CmpSIL2sil2oemstackisvalid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemstackisvalid", (RTS_UINTPTR)sil2oemstackisvalid, 1, 0x56A27207, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2oemstackisvalid
	#define EXT_sil2oemstackisvalid
	#define GET_sil2oemstackisvalid(fl)  CAL_CMGETAPI( "sil2oemstackisvalid" ) 
	#define CAL_sil2oemstackisvalid  sil2oemstackisvalid
	#define CHK_sil2oemstackisvalid  TRUE
	#define EXP_sil2oemstackisvalid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemstackisvalid", (RTS_UINTPTR)sil2oemstackisvalid, 1, 0x56A27207, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2oemstackisvalid  PFSIL2OEMSTACKISVALID_IEC pfsil2oemstackisvalid;
	#define EXT_sil2oemstackisvalid  extern PFSIL2OEMSTACKISVALID_IEC pfsil2oemstackisvalid;
	#define GET_sil2oemstackisvalid(fl)  s_pfCMGetAPI2( "sil2oemstackisvalid", (RTS_VOID_FCTPTR *)&pfsil2oemstackisvalid, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x56A27207, 0x03050400)
	#define CAL_sil2oemstackisvalid  pfsil2oemstackisvalid
	#define CHK_sil2oemstackisvalid  (pfsil2oemstackisvalid != NULL)
	#define EXP_sil2oemstackisvalid   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2oemstackisvalid", (RTS_UINTPTR)sil2oemstackisvalid, 1, 0x56A27207, 0x03050400) 
#endif


/**
 * <description>
 * <p>Function to delegate a Non-Safety Job</p>
 * <p>This function can be used to delegate a non-safety job from within the safe-context to be executed from the Unsafe context</p>
 * <p>The function pfNonSafetyJob has to be called from Unsafe Context with pParam as argument</p>
 * </description> 
 * <param name="pfNonSafetyJob" type="IN" range="[NULL,VALID_NONSAFETY_FUNCTION]">Function Pointer to NonSafety Job</param>
 * <param name="pParam" type="IN" range="[NULL,VALID_POINTER]">Pointer to Parameter for NonSafety Job</param>
 * <parampseudo name="uiCallerCtx" type="IN" range="[RTS_SIL2_CALLERCTX_SAFE,RTS_SIL2_CALLERCTX_UNSAFE]">Specifies the context of the caller</parampseudo>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Job was done correctly</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Job could not be done</errorcode>
 * <result>Result of delegating a Non-Safety Job</result>
 */
RTS_RESULT CDECL SIL2OEMExecuteNonSafetyJob(void (*pfNonSafetyJob)(void * pParam),void * pParam);
typedef RTS_RESULT (CDECL * PFSIL2OEMEXECUTENONSAFETYJOB) (void (*pfNonSafetyJob)(void * pParam),void * pParam);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMEXECUTENONSAFETYJOB_NOTIMPLEMENTED)
	#define USE_SIL2OEMExecuteNonSafetyJob
	#define EXT_SIL2OEMExecuteNonSafetyJob
	#define GET_SIL2OEMExecuteNonSafetyJob(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2OEMExecuteNonSafetyJob(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SIL2OEMExecuteNonSafetyJob  FALSE
	#define EXP_SIL2OEMExecuteNonSafetyJob  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2OEMExecuteNonSafetyJob
	#define EXT_SIL2OEMExecuteNonSafetyJob
	#define GET_SIL2OEMExecuteNonSafetyJob(fl)  CAL_CMGETAPI( "SIL2OEMExecuteNonSafetyJob" ) 
	#define CAL_SIL2OEMExecuteNonSafetyJob  SIL2OEMExecuteNonSafetyJob
	#define CHK_SIL2OEMExecuteNonSafetyJob  TRUE
	#define EXP_SIL2OEMExecuteNonSafetyJob  CAL_CMEXPAPI( "SIL2OEMExecuteNonSafetyJob" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2OEMExecuteNonSafetyJob
	#define EXT_SIL2OEMExecuteNonSafetyJob
	#define GET_SIL2OEMExecuteNonSafetyJob(fl)  CAL_CMGETAPI( "SIL2OEMExecuteNonSafetyJob" ) 
	#define CAL_SIL2OEMExecuteNonSafetyJob  SIL2OEMExecuteNonSafetyJob
	#define CHK_SIL2OEMExecuteNonSafetyJob  TRUE
	#define EXP_SIL2OEMExecuteNonSafetyJob  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMExecuteNonSafetyJob", (RTS_UINTPTR)SIL2OEMExecuteNonSafetyJob, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2OEMExecuteNonSafetyJob
	#define EXT_CmpSIL2SIL2OEMExecuteNonSafetyJob
	#define GET_CmpSIL2SIL2OEMExecuteNonSafetyJob  ERR_OK
	#define CAL_CmpSIL2SIL2OEMExecuteNonSafetyJob pICmpSIL2->ISIL2OEMExecuteNonSafetyJob
	#define CHK_CmpSIL2SIL2OEMExecuteNonSafetyJob (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2OEMExecuteNonSafetyJob  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2OEMExecuteNonSafetyJob
	#define EXT_SIL2OEMExecuteNonSafetyJob
	#define GET_SIL2OEMExecuteNonSafetyJob(fl)  CAL_CMGETAPI( "SIL2OEMExecuteNonSafetyJob" ) 
	#define CAL_SIL2OEMExecuteNonSafetyJob pICmpSIL2->ISIL2OEMExecuteNonSafetyJob
	#define CHK_SIL2OEMExecuteNonSafetyJob (pICmpSIL2 != NULL)
	#define EXP_SIL2OEMExecuteNonSafetyJob  CAL_CMEXPAPI( "SIL2OEMExecuteNonSafetyJob" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2OEMExecuteNonSafetyJob  PFSIL2OEMEXECUTENONSAFETYJOB pfSIL2OEMExecuteNonSafetyJob;
	#define EXT_SIL2OEMExecuteNonSafetyJob  extern PFSIL2OEMEXECUTENONSAFETYJOB pfSIL2OEMExecuteNonSafetyJob;
	#define GET_SIL2OEMExecuteNonSafetyJob(fl)  s_pfCMGetAPI2( "SIL2OEMExecuteNonSafetyJob", (RTS_VOID_FCTPTR *)&pfSIL2OEMExecuteNonSafetyJob, (fl), 0, 0)
	#define CAL_SIL2OEMExecuteNonSafetyJob  pfSIL2OEMExecuteNonSafetyJob
	#define CHK_SIL2OEMExecuteNonSafetyJob  (pfSIL2OEMExecuteNonSafetyJob != NULL)
	#define EXP_SIL2OEMExecuteNonSafetyJob  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMExecuteNonSafetyJob", (RTS_UINTPTR)SIL2OEMExecuteNonSafetyJob, 0, 0) 
#endif




/**
 * <description>sil2executenonsafetyjob</description>
 * <SIL2/>
 */
typedef struct tagsil2executenonsafetyjob_struct
{
	RTS_IEC_BYTE *pfNonSafetyJob;		/* VAR_INPUT */
	RTS_IEC_BYTE *pParam;			/* VAR_INPUT */
	RTS_IEC_ULINT ulSize;			/* VAR_INPUT */
	RTS_IEC_RESULT SIL2ExecuteNonSafetyJob;	/* VAR_OUTPUT */
} sil2executenonsafetyjob_struct;

/**
 * <description>
 * <p>Function to delegate a Non-Safety Job</p>
 * <p>This function behaves the exact same way as SIL2OEMExecuteNonSafetyJob(), but
 * it provides also the size of the parameter to the function. This way, the OEM customer
 * is able to copy the parameter temporarily to a shared memory, message box or a different stack.</p>
 * </description> 
 * <param name="pfNonSafetyJob" type="IN" range="[NULL,VALID_NONSAFETY_FUNCTION]">Function Pointer to NonSafety Job</param>
 * <param name="pParam" type="IN" range="[NULL,VALID_POINTER]">Pointer to Parameter for NonSafety Job</param>
 * <param name="iSize" type="IN" range="[0..VALID_PARAM_SIZE]">Size of Parameter for NonSafety Job</param>
 * <parampseudo name="uiCallerCtx" type="IN" range="[RTS_SIL2_CALLERCTX_SAFE,RTS_SIL2_CALLERCTX_UNSAFE]">Specifies the context of the caller</parampseudo>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Job was done correctly</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">A parameter was passed, but the size is 0 or parameter is NULL and size > 0</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Job could not be done</errorcode>
 * <result>Result of delegating a Non-Safety Job</result>
 */
RTS_RESULT CDECL SIL2OEMExecuteNonSafetyJob2(void (*pfNonSafetyJob)(void * pParam),void * pParam,RTS_SIZE iSize);
typedef RTS_RESULT (CDECL * PFSIL2OEMEXECUTENONSAFETYJOB2) (void (*pfNonSafetyJob)(void * pParam),void * pParam,RTS_SIZE iSize);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2OEMEXECUTENONSAFETYJOB2_NOTIMPLEMENTED)
	#define USE_SIL2OEMExecuteNonSafetyJob2
	#define EXT_SIL2OEMExecuteNonSafetyJob2
	#define GET_SIL2OEMExecuteNonSafetyJob2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2OEMExecuteNonSafetyJob2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SIL2OEMExecuteNonSafetyJob2  FALSE
	#define EXP_SIL2OEMExecuteNonSafetyJob2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2OEMExecuteNonSafetyJob2
	#define EXT_SIL2OEMExecuteNonSafetyJob2
	#define GET_SIL2OEMExecuteNonSafetyJob2(fl)  CAL_CMGETAPI( "SIL2OEMExecuteNonSafetyJob2" ) 
	#define CAL_SIL2OEMExecuteNonSafetyJob2  SIL2OEMExecuteNonSafetyJob2
	#define CHK_SIL2OEMExecuteNonSafetyJob2  TRUE
	#define EXP_SIL2OEMExecuteNonSafetyJob2  CAL_CMEXPAPI( "SIL2OEMExecuteNonSafetyJob2" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2OEMExecuteNonSafetyJob2
	#define EXT_SIL2OEMExecuteNonSafetyJob2
	#define GET_SIL2OEMExecuteNonSafetyJob2(fl)  CAL_CMGETAPI( "SIL2OEMExecuteNonSafetyJob2" ) 
	#define CAL_SIL2OEMExecuteNonSafetyJob2  SIL2OEMExecuteNonSafetyJob2
	#define CHK_SIL2OEMExecuteNonSafetyJob2  TRUE
	#define EXP_SIL2OEMExecuteNonSafetyJob2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMExecuteNonSafetyJob2", (RTS_UINTPTR)SIL2OEMExecuteNonSafetyJob2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2OEMExecuteNonSafetyJob2
	#define EXT_CmpSIL2SIL2OEMExecuteNonSafetyJob2
	#define GET_CmpSIL2SIL2OEMExecuteNonSafetyJob2  ERR_OK
	#define CAL_CmpSIL2SIL2OEMExecuteNonSafetyJob2 pICmpSIL2->ISIL2OEMExecuteNonSafetyJob2
	#define CHK_CmpSIL2SIL2OEMExecuteNonSafetyJob2 (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2OEMExecuteNonSafetyJob2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2OEMExecuteNonSafetyJob2
	#define EXT_SIL2OEMExecuteNonSafetyJob2
	#define GET_SIL2OEMExecuteNonSafetyJob2(fl)  CAL_CMGETAPI( "SIL2OEMExecuteNonSafetyJob2" ) 
	#define CAL_SIL2OEMExecuteNonSafetyJob2 pICmpSIL2->ISIL2OEMExecuteNonSafetyJob2
	#define CHK_SIL2OEMExecuteNonSafetyJob2 (pICmpSIL2 != NULL)
	#define EXP_SIL2OEMExecuteNonSafetyJob2  CAL_CMEXPAPI( "SIL2OEMExecuteNonSafetyJob2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2OEMExecuteNonSafetyJob2  PFSIL2OEMEXECUTENONSAFETYJOB2 pfSIL2OEMExecuteNonSafetyJob2;
	#define EXT_SIL2OEMExecuteNonSafetyJob2  extern PFSIL2OEMEXECUTENONSAFETYJOB2 pfSIL2OEMExecuteNonSafetyJob2;
	#define GET_SIL2OEMExecuteNonSafetyJob2(fl)  s_pfCMGetAPI2( "SIL2OEMExecuteNonSafetyJob2", (RTS_VOID_FCTPTR *)&pfSIL2OEMExecuteNonSafetyJob2, (fl), 0, 0)
	#define CAL_SIL2OEMExecuteNonSafetyJob2  pfSIL2OEMExecuteNonSafetyJob2
	#define CHK_SIL2OEMExecuteNonSafetyJob2  (pfSIL2OEMExecuteNonSafetyJob2 != NULL)
	#define EXP_SIL2OEMExecuteNonSafetyJob2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2OEMExecuteNonSafetyJob2", (RTS_UINTPTR)SIL2OEMExecuteNonSafetyJob2, 0, 0) 
#endif



void CDECL CDECL_EXT sil2executenonsafetyjob(sil2executenonsafetyjob_struct *p);
typedef void (CDECL CDECL_EXT* PFSIL2EXECUTENONSAFETYJOB_IEC) (sil2executenonsafetyjob_struct *p);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2EXECUTENONSAFETYJOB_NOTIMPLEMENTED)
	#define USE_sil2executenonsafetyjob
	#define EXT_sil2executenonsafetyjob
	#define GET_sil2executenonsafetyjob(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sil2executenonsafetyjob(p0) 
	#define CHK_sil2executenonsafetyjob  FALSE
	#define EXP_sil2executenonsafetyjob  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sil2executenonsafetyjob
	#define EXT_sil2executenonsafetyjob
	#define GET_sil2executenonsafetyjob(fl)  CAL_CMGETAPI( "sil2executenonsafetyjob" ) 
	#define CAL_sil2executenonsafetyjob  sil2executenonsafetyjob
	#define CHK_sil2executenonsafetyjob  TRUE
	#define EXP_sil2executenonsafetyjob  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2executenonsafetyjob", (RTS_UINTPTR)sil2executenonsafetyjob, 1, 0x142EECAE, 0x03050400) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_sil2executenonsafetyjob
	#define EXT_sil2executenonsafetyjob
	#define GET_sil2executenonsafetyjob(fl)  CAL_CMGETAPI( "sil2executenonsafetyjob" ) 
	#define CAL_sil2executenonsafetyjob  sil2executenonsafetyjob
	#define CHK_sil2executenonsafetyjob  TRUE
	#define EXP_sil2executenonsafetyjob  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2executenonsafetyjob", (RTS_UINTPTR)sil2executenonsafetyjob, 1, 0x142EECAE, 0x03050400) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2sil2executenonsafetyjob
	#define EXT_CmpSIL2sil2executenonsafetyjob
	#define GET_CmpSIL2sil2executenonsafetyjob  ERR_OK
	#define CAL_CmpSIL2sil2executenonsafetyjob  sil2executenonsafetyjob
	#define CHK_CmpSIL2sil2executenonsafetyjob  TRUE
	#define EXP_CmpSIL2sil2executenonsafetyjob  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2executenonsafetyjob", (RTS_UINTPTR)sil2executenonsafetyjob, 1, 0x142EECAE, 0x03050400) 
#elif defined(CPLUSPLUS)
	#define USE_sil2executenonsafetyjob
	#define EXT_sil2executenonsafetyjob
	#define GET_sil2executenonsafetyjob(fl)  CAL_CMGETAPI( "sil2executenonsafetyjob" ) 
	#define CAL_sil2executenonsafetyjob  sil2executenonsafetyjob
	#define CHK_sil2executenonsafetyjob  TRUE
	#define EXP_sil2executenonsafetyjob  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2executenonsafetyjob", (RTS_UINTPTR)sil2executenonsafetyjob, 1, 0x142EECAE, 0x03050400) 
#else /* DYNAMIC_LINK */
	#define USE_sil2executenonsafetyjob  PFSIL2EXECUTENONSAFETYJOB_IEC pfsil2executenonsafetyjob;
	#define EXT_sil2executenonsafetyjob  extern PFSIL2EXECUTENONSAFETYJOB_IEC pfsil2executenonsafetyjob;
	#define GET_sil2executenonsafetyjob(fl)  s_pfCMGetAPI2( "sil2executenonsafetyjob", (RTS_VOID_FCTPTR *)&pfsil2executenonsafetyjob, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x142EECAE, 0x03050400)
	#define CAL_sil2executenonsafetyjob  pfsil2executenonsafetyjob
	#define CHK_sil2executenonsafetyjob  (pfsil2executenonsafetyjob != NULL)
	#define EXP_sil2executenonsafetyjob   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sil2executenonsafetyjob", (RTS_UINTPTR)sil2executenonsafetyjob, 1, 0x142EECAE, 0x03050400) 
#endif



/**
 * <description>
 * <p>Function to log the program flow</p>
 * <p>The caller is responsible for providing buffer for storing the control flow data. After having logged, the function
 * SIL2ControlFlowCheck can be used to check if all control positions have been logged in the correct order. </p>
 * <p>The Log IDs may not be out of range of the specified log buffer.</p>
 * </description> 
 * <param name="pControlFlow" type="IN" range="[NULL,VALID_CONTROLFLOW_BUFFER]">Buffer to store control flow data in</param>
 * <param name="uiCurrID" type="IN" range="[0..NUM_OF_CONTROLFLOW_LOGENTRIES]">Current Log Nr</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">successfully added logentry to control flow</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOBUFFER">pControlFlow was NULL</errorcode> 
 * <result>Result of Log to control-flow</result>
 */
RTS_RESULT CDECL SIL2ControlFlowLog(SIL2_CONTROLFLOW* pControlFlow, RTS_UI8 uiCurrID);
typedef RTS_RESULT (CDECL * PFSIL2CONTROLFLOWLOG) (SIL2_CONTROLFLOW* pControlFlow, RTS_UI8 uiCurrID);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2CONTROLFLOWLOG_NOTIMPLEMENTED)
	#define USE_SIL2ControlFlowLog
	#define EXT_SIL2ControlFlowLog
	#define GET_SIL2ControlFlowLog(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2ControlFlowLog(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SIL2ControlFlowLog  FALSE
	#define EXP_SIL2ControlFlowLog  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2ControlFlowLog
	#define EXT_SIL2ControlFlowLog
	#define GET_SIL2ControlFlowLog(fl)  CAL_CMGETAPI( "SIL2ControlFlowLog" ) 
	#define CAL_SIL2ControlFlowLog  SIL2ControlFlowLog
	#define CHK_SIL2ControlFlowLog  TRUE
	#define EXP_SIL2ControlFlowLog  CAL_CMEXPAPI( "SIL2ControlFlowLog" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2ControlFlowLog
	#define EXT_SIL2ControlFlowLog
	#define GET_SIL2ControlFlowLog(fl)  CAL_CMGETAPI( "SIL2ControlFlowLog" ) 
	#define CAL_SIL2ControlFlowLog  SIL2ControlFlowLog
	#define CHK_SIL2ControlFlowLog  TRUE
	#define EXP_SIL2ControlFlowLog  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2ControlFlowLog", (RTS_UINTPTR)SIL2ControlFlowLog, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2ControlFlowLog
	#define EXT_CmpSIL2SIL2ControlFlowLog
	#define GET_CmpSIL2SIL2ControlFlowLog  ERR_OK
	#define CAL_CmpSIL2SIL2ControlFlowLog pICmpSIL2->ISIL2ControlFlowLog
	#define CHK_CmpSIL2SIL2ControlFlowLog (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2ControlFlowLog  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2ControlFlowLog
	#define EXT_SIL2ControlFlowLog
	#define GET_SIL2ControlFlowLog(fl)  CAL_CMGETAPI( "SIL2ControlFlowLog" ) 
	#define CAL_SIL2ControlFlowLog pICmpSIL2->ISIL2ControlFlowLog
	#define CHK_SIL2ControlFlowLog (pICmpSIL2 != NULL)
	#define EXP_SIL2ControlFlowLog  CAL_CMEXPAPI( "SIL2ControlFlowLog" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2ControlFlowLog  PFSIL2CONTROLFLOWLOG pfSIL2ControlFlowLog;
	#define EXT_SIL2ControlFlowLog  extern PFSIL2CONTROLFLOWLOG pfSIL2ControlFlowLog;
	#define GET_SIL2ControlFlowLog(fl)  s_pfCMGetAPI2( "SIL2ControlFlowLog", (RTS_VOID_FCTPTR *)&pfSIL2ControlFlowLog, (fl), 0, 0)
	#define CAL_SIL2ControlFlowLog  pfSIL2ControlFlowLog
	#define CHK_SIL2ControlFlowLog  (pfSIL2ControlFlowLog != NULL)
	#define EXP_SIL2ControlFlowLog  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2ControlFlowLog", (RTS_UINTPTR)SIL2ControlFlowLog, 0, 0) 
#endif




/**
 * <description>
 * <p>Function to check the previously logged program flow</p>
 * <p>The caller is responsible for providing buffer for storing the control flow data. After having logged, the function
 * SIL2ControlFlowCheck can be used to check if all control positions have been logged in the correct order. </p>
 * <p>The number of IDs may not be out of range of the specified log buffer.</p>
 * </description> 
 * <param name="pControlFlow" type="IN" range="[NULL,VALID_CONTROLFLOW_BUFFER,INVALID_CONTROLFLOW_BUFFER]">Buffer to store control flow data in</param>
 * <param name="uiTotalNr" type="IN" range="[0,NUM_OF_CONTROLFLOW_LOGENTRIES]">Nr of Logs expected or to check</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">all logentries were added in the correct order, check successfull</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_OUT_OF_LIMITS">uiTotalNr exceeds buffersize</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOBUFFER">pControlFlow is NULL</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">check failed, order wrong or not all logentries correct</errorcode>
 * <result>Result of control-flow check</result>
 */
RTS_RESULT CDECL SIL2ControlFlowCheck(SIL2_CONTROLFLOW* pControlFlow, RTS_UI8 uiTotalNr);
typedef RTS_RESULT (CDECL * PFSIL2CONTROLFLOWCHECK) (SIL2_CONTROLFLOW* pControlFlow, RTS_UI8 uiTotalNr);
#if defined(CMPSIL2_NOTIMPLEMENTED) || defined(SIL2CONTROLFLOWCHECK_NOTIMPLEMENTED)
	#define USE_SIL2ControlFlowCheck
	#define EXT_SIL2ControlFlowCheck
	#define GET_SIL2ControlFlowCheck(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SIL2ControlFlowCheck(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SIL2ControlFlowCheck  FALSE
	#define EXP_SIL2ControlFlowCheck  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SIL2ControlFlowCheck
	#define EXT_SIL2ControlFlowCheck
	#define GET_SIL2ControlFlowCheck(fl)  CAL_CMGETAPI( "SIL2ControlFlowCheck" ) 
	#define CAL_SIL2ControlFlowCheck  SIL2ControlFlowCheck
	#define CHK_SIL2ControlFlowCheck  TRUE
	#define EXP_SIL2ControlFlowCheck  CAL_CMEXPAPI( "SIL2ControlFlowCheck" ) 
#elif defined(MIXED_LINK) && !defined(CMPSIL2_EXTERNAL)
	#define USE_SIL2ControlFlowCheck
	#define EXT_SIL2ControlFlowCheck
	#define GET_SIL2ControlFlowCheck(fl)  CAL_CMGETAPI( "SIL2ControlFlowCheck" ) 
	#define CAL_SIL2ControlFlowCheck  SIL2ControlFlowCheck
	#define CHK_SIL2ControlFlowCheck  TRUE
	#define EXP_SIL2ControlFlowCheck  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2ControlFlowCheck", (RTS_UINTPTR)SIL2ControlFlowCheck, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpSIL2SIL2ControlFlowCheck
	#define EXT_CmpSIL2SIL2ControlFlowCheck
	#define GET_CmpSIL2SIL2ControlFlowCheck  ERR_OK
	#define CAL_CmpSIL2SIL2ControlFlowCheck pICmpSIL2->ISIL2ControlFlowCheck
	#define CHK_CmpSIL2SIL2ControlFlowCheck (pICmpSIL2 != NULL)
	#define EXP_CmpSIL2SIL2ControlFlowCheck  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SIL2ControlFlowCheck
	#define EXT_SIL2ControlFlowCheck
	#define GET_SIL2ControlFlowCheck(fl)  CAL_CMGETAPI( "SIL2ControlFlowCheck" ) 
	#define CAL_SIL2ControlFlowCheck pICmpSIL2->ISIL2ControlFlowCheck
	#define CHK_SIL2ControlFlowCheck (pICmpSIL2 != NULL)
	#define EXP_SIL2ControlFlowCheck  CAL_CMEXPAPI( "SIL2ControlFlowCheck" ) 
#else /* DYNAMIC_LINK */
	#define USE_SIL2ControlFlowCheck  PFSIL2CONTROLFLOWCHECK pfSIL2ControlFlowCheck;
	#define EXT_SIL2ControlFlowCheck  extern PFSIL2CONTROLFLOWCHECK pfSIL2ControlFlowCheck;
	#define GET_SIL2ControlFlowCheck(fl)  s_pfCMGetAPI2( "SIL2ControlFlowCheck", (RTS_VOID_FCTPTR *)&pfSIL2ControlFlowCheck, (fl), 0, 0)
	#define CAL_SIL2ControlFlowCheck  pfSIL2ControlFlowCheck
	#define CHK_SIL2ControlFlowCheck  (pfSIL2ControlFlowCheck != NULL)
	#define EXP_SIL2ControlFlowCheck  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SIL2ControlFlowCheck", (RTS_UINTPTR)SIL2ControlFlowCheck, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSIL2APPBOOT ISIL2AppBoot;
 	PFSIL2CHECKCALLERCONTEXT ISIL2CheckCallerContext;
 	PFSIL2OEMENTERDEBUGMODE ISIL2OEMEnterDebugMode;
 	PFSIL2OEMEXCEPTION ISIL2OEMException;
 	PFSIL2ADDLOG ISIL2AddLog;
 	PFSIL2OEMGETOPERATIONMODE ISIL2OEMGetOperationMode;
 	PFSIL2OEMGETCALLERCONTEXT ISIL2OEMGetCallerContext;
 	PFSIL2OEMGETMEMORYSTATE ISIL2OEMGetMemoryState;
 	PFSIL2OEMSTACKISVALID ISIL2OEMStackIsValid;
 	PFSIL2OEMEXECUTENONSAFETYJOB ISIL2OEMExecuteNonSafetyJob;
 	PFSIL2OEMEXECUTENONSAFETYJOB2 ISIL2OEMExecuteNonSafetyJob2;
 	PFSIL2CONTROLFLOWLOG ISIL2ControlFlowLog;
 	PFSIL2CONTROLFLOWCHECK ISIL2ControlFlowCheck;
 } ICmpSIL2_C;

#ifdef CPLUSPLUS
class ICmpSIL2 : public IBase
{
	public:
		virtual RTS_RESULT CDECL ISIL2AppBoot(void) =0;
		virtual RTS_RESULT CDECL ISIL2CheckCallerContext(RTS_UI32 ulCallerContextExpected) =0;
		virtual RTS_RESULT CDECL ISIL2OEMEnterDebugMode(void) =0;
		virtual void CDECL ISIL2OEMException(RTS_SIL2_EXCEPTION Exception) =0;
		virtual RTS_RESULT CDECL ISIL2AddLog(char* pszMessage, RTS_UI32 ulLogId) =0;
		virtual RTS_SIL2_OPMODE CDECL ISIL2OEMGetOperationMode(void) =0;
		virtual RTS_UI32 CDECL ISIL2OEMGetCallerContext(void) =0;
		virtual RTS_SIL2_ADDRESSSTATE CDECL ISIL2OEMGetMemoryState(RTS_UI8 *pAddress, RTS_UI32 ulLength) =0;
		virtual RTS_RESULT CDECL ISIL2OEMStackIsValid(void) =0;
		virtual RTS_RESULT CDECL ISIL2OEMExecuteNonSafetyJob(void (*pfNonSafetyJob)(void * pParam), void * pParam) =0;
		virtual RTS_RESULT CDECL ISIL2OEMExecuteNonSafetyJob2(void (*pfNonSafetyJob)(void * pParam), void * pParam, RTS_SIZE iSize) =0;
		virtual RTS_RESULT CDECL ISIL2ControlFlowLog(SIL2_CONTROLFLOW* pControlFlow, RTS_UI8 uiCurrID) =0;
		virtual RTS_RESULT CDECL ISIL2ControlFlowCheck(SIL2_CONTROLFLOW* pControlFlow, RTS_UI8 uiTotalNr) =0;
};
	#ifndef ITF_CmpSIL2
		#define ITF_CmpSIL2 static ICmpSIL2 *pICmpSIL2 = NULL;
	#endif
	#define EXTITF_CmpSIL2
#else	/*CPLUSPLUS*/
	typedef ICmpSIL2_C		ICmpSIL2;
	#ifndef ITF_CmpSIL2
		#define ITF_CmpSIL2
	#endif
	#define EXTITF_CmpSIL2
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPSIL2ITF_H_*/
