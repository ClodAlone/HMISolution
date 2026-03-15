 /**
 * <interfacename>SysExcept</interfacename>
 * <description> 
 *	<p>The SysExcept interface is projected to handle all exceptions in the runtime system. All
 *	available exceptions on the target should be handled. Exception handling is typically CPU
 *	and operating system dependent.</p>
 *	<p>All active code parts in the runtime system (tasks, timer, interrupt handler) must be protected
 *	against software errors, because this could lead to an unpredictable behavior or crash of the
 *	complete runtime system.</p>
 *	<p>All exceptions are handled in this component as first level.
 *	If the first level handling is done in another component, the component has to call the 
 *	SysExceptGenerateException() routine to	enable second level handling. In this case, the first
 *	level handling must be disabled with a configuration setting (see below).</p>
 *	<p>The SysExcept component forwards every exception to the component, that manages the active
 *	code, mostly the SysTask, SysTimer or SysInt component. These components must register exception
 *	handlers to the SysExcept component.</p>
 *	<p>As the last station of exception handling, the exception handler of the component, that creates
 *	the active object, was called from the SysTask, SysTimer or SysInt component. Below you can see
 *	the structure and the layers of exception handling:</p>
 *
 * <pre>
 *   +----------------------------------------------------------+
 *   | XXX component:                       Code that generates |
 *   | Registered exception handler         exception (1)       |
 *   | of component, is called. (4)                             |
 *   +----------------------------------------------------------+
 *        ^                                                 | 
 *        |                                                 | 
 *        |                                                 | 
 *   +-----------------------------------------------+      | 
 *   | SysTask or SysTimer component:                |      | 
 *   | Registered exception handler is called (3)    |      | 
 *   +-----------------------------------------------+      | 
 *        ^                                                 | 
 *        |                                                 | 
 *        |                                                 | 
 *   +-----------------------------------------------+      | 
 *   | SysExcept component:                          |      | 
 *   | ::SysExceptGenerateException() is called (2)  |      | 
 *   +-----------------------------------------------+      | 
 *        ^                                                 | 
 *        |                                                 | 
 *        |                    Exception occurred (1a)      | 
 *        |&lt;------------------------------------------------| 
 *        |                                                 | 
 *   +-----------------------------------------------+      | 
 *   | OEM component:                                |      | (1b)
 *   | First level exception handling. If the        |      | 
 *   | exception is not in the OEM context, it is    |      | 
 *   | forwarded to the SysExcept component          |      | 
 *   +-----------------------------------------------+      | 
 *        ^                                                 |
 *        '-------------------------------------------------'
 * </pre>
 * <p>If RTS_STRUCTURED_EXCEPTION_HANDLING is set as a define, the structured exception handling is activated.
 * To use this exception handling, the following functions must be imported:</p>
 * <ul>
 *    <li>SysExceptTry</li>
 *    <li>SysExceptCatch</li>
 *    <li>SysExceptCatchException</li>
 * </ul>
 * 
 * <p>Here is an example of the usage:</p>
 * <pre>
 *     Variant 1:
 *     ----------
 *     rts_try
 *     {
 *         ...
 *     }
 *     rts_catch_exception(RTSEXCPT_DIVIDEBYZERO)
 *     {
 *         RTS_UI32 exceptionCode = EXCPT_GET_CODE();
 *         RegContext *pExceptionContext = EXCPT_GET_CONTEXT();
 *         ...
 *     }
 *     rts_catch_exception(RTSEXCPT_DIVIDEBYZERO)
 *     {
 *         ...
 *     }
 *     rts_catch
 *     {
 *         ...
 *     }
 *     rts_try_end
 *    
 *    
 *     Variant 2:
 *     ----------
 *    
 *     rts_try
 *     {
 *         ...
 *     }
 *     rts_catch
 *     {
 *         RTS_UI32 exceptionCode = EXCPT_GET_CODE();
 *         RegContext *pExceptionContext = EXCPT_GET_CONTEXT();
 *         ...
 *     }
 *     rts_try_end
 * </pre>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _SYSEXCEPTITF_H_
#define _SYSEXCEPTITF_H_

#include "CmpStd.h"

 

 




/**
 * <category>Compiler switch</category>
 * <description>
 *	Compiler switches to enable/disable single features in the component.
 * </description>
 * <element name="RTS_STRUCTURED_EXCEPTION_HANDLING">Switch to enabled structured exception handling. See rts_try/rts_catch for details.</element>
 */

/**
 * <category>Static defines</category>
 * <description>Default number of static interfaces that can be registered on exceptions</description>
 */
#ifndef EXCPT_DEFAULT_NUM_OF_INTERFACES
	#define EXCPT_DEFAULT_NUM_OF_INTERFACES		5
#endif

/**
 * <category>Static defines</category>
 * <description>Maximum number of nested structured exception handler on the same task</description>
 */
#ifndef EXCPT_MAX_NUM_OF_SEH_HANDLER
	#define EXCPT_MAX_NUM_OF_SEH_HANDLER		5
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Setting to enable/disable first level exception handling:
 *	1: enable first level handling
 *	0: disable first level handling. Is done by an OEM component.
 * </description>
 */
#define SYSEXCEPT_KEY_INT_ENABLE_FIRST_LEVEL_HANDLING				"EnableFirstLevelHandling"
#define SYSEXCEPT_VALUE_INT_ENABLE_FIRST_LEVEL_HANDLING_DEFAULT		1

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Disable the FPU Invalid Operation exception</description>
 */
#define SYSEXCEPTKEY_INT_LINUX_DISABLE_FPU_INVALID_EXCEPTION			"Linux.DisableFpuInvalidOperationException"
#define SYSEXCEPTKEY_INT_LINUX_DISABLE_FPU_INVALID_EXCEPTION_DEFAULT	0

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Disable the FPU Overflow exception</description>
 */
#define SYSEXCEPTKEY_INT_LINUX_DISABLE_FPU_OVERFLOW_EXCEPTION			"Linux.DisableFpuOverflowException"
#define SYSEXCEPTKEY_INT_LINUX_DISABLE_FPU_OVERFLOW_EXCEPTION_DEFAULT	0

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Disable the FPU Underflow exception</description>
 */
#define SYSEXCEPTKEY_INT_LINUX_DISABLE_FPU_UNDERFLOW_EXCEPTION			"Linux.DisableFpuUnderflowException"
#define SYSEXCEPTKEY_INT_LINUX_DISABLE_FPU_UNDERFLOW_EXCEPTION_DEFAULT	0

#ifdef RTS_STRUCTURED_EXCEPTION_HANDLING
	#ifndef RTS_JMP_BUF
		#include <setjmp.h>
		#define RTS_JMP_BUF		jmp_buf
	#endif
	#ifndef RTS_SETJMP
		#define RTS_SETJMP		setjmp
	#endif
	#ifndef RTS_LONGJMP
		#define RTS_LONGJMP		longjmp
	#endif
#else
	#ifndef RTS_JMP_BUF
		#define RTS_JMP_BUF			void*
	#endif
	#ifndef RTS_SETJMP
		#define RTS_SETJMP(buf)
	#endif
	#ifndef RTS_LONGJMP
		#define RTS_LONGJMP(buf, val)
	#endif
#endif

#ifndef RTS_FP_BUF
	#define RTS_FP_BUF			void*
#endif
#ifndef RTS_GET_FPENV
	#define RTS_GET_FPENV(buf)
#endif
#ifndef RTS_SET_FPENV
	#define RTS_SET_FPENV(buf)
#endif

/**
 * <category>Exception code</category>
 * <description></description>
 * <element name="ulCode" type="IN">Exception code. Can be an operating system specific exception code
 *		or a runtime specific exception code (see exception code category)</element>
 * <element name="bOSException" type="IN">Exception code flag:
 *		<p>1=exception code is an operating system specific exception code</p>
 *		<p>0=exception code is a runtime code. See exception code category</p></element>
 */
typedef struct tagExceptionCode
{
	RTS_UI32 ulCode;
	int bOSException;
} ExceptionCode;

/**
 * <SIL2/>
 * <category>Exception context</category>
 * <description></description>
 *	<element name="IP" type="IN">Instruction pointer or program counter</element>
 *	<element name="BP" type="IN">Base pointer or frame pointer</element>
 *	<element name="SP" type="IN">Stack pointer</element>
 */
typedef struct RegContexttag
{
	RTS_UINTPTR IP;
	RTS_UINTPTR BP;
	RTS_UINTPTR SP;	
} RegContext;

/**
 * <category>SEH Context</category>
 * <description>Linked list of contexts for the structured exception handling.</description>
 * <element name="ui32Pattern" type="IN">Pattern to check if the stack was overwritten</element>
 * <element name="ui32ExceptionCode" type="IN">Current exception code for this exception frame (might be none)</element>
 * <element name="jmpbuf" type="IN">Context saving buffer for RTS_SETJMP / RTS_LONGJMP</element>
 * <element name="bHandled" type="IN">True, when the exception in this exception frame was already handled</element>
 */
typedef struct tagSEHCOntext
{
	struct tagSEHCOntext *pNext;
	RTS_UI32    ui32Pattern;
	RTS_UI32    ui32ExceptionCode;
	RTS_JMP_BUF jmpbuf;
	RegContext  context;
	RTS_BOOL    bHandled;
	RTS_BOOL    bRegistered;
	RTS_FP_BUF	fpenvbuf;
} SEHContext;

#define SEH_CONTEXT_PATTERN 0xA5A5ACDC

typedef struct
{
	RTS_IEC_HANDLE hSysTask;
	RTS_IEC_UDINT ulException;
	RegContext Context;
	RTS_IEC_RESULT excpthandler;
} excpthandler_struct;

/**
 * <category>Exception handler</category>
 * <description>Function prototype for the exception handler</description>
 * <param name="uiTaskOSHandle" type="IN">Operating system specific task handle</param>
 * <param name="ulException" type="IN">Exception code structure</param>
 * <param name="Context" type="IN">Context of actual exception position</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL ExceptionHandler(RTS_HANDLE uiTaskOSHandle, RTS_UI32 ulException, RegContext Context);
typedef RTS_RESULT (CDECL *PFEXCEPTIONHANDLER)(RTS_HANDLE uiTaskOSHandle, RTS_UI32 ulException, RegContext Context);
typedef void (CDECL*PFEXCEPTIONHANDLERIEC)(excpthandler_struct *p);


/**
 * <category>Exception pseudo code</category>
 * <description>Exception pseudo code to prepare exception handling. Can be used e.g. to release locking objects in the context of the task!</description>
 */
#define RTSEXCPT_FIRST_LEVEL				0x80000000

/**
 * <category>Exception code</category>
 * <description>Invalid</description>
 */
#define RTSEXCPT_UNKNOWN					(UINT32_MAX & ~RTSEXCPT_FIRST_LEVEL) 

/**
 * <category>Exception code</category>
 * <description>No exception</description>
 */
#define RTSEXCPT_NOEXCEPTION				0x00000000

/**
 * <category>Exception code</category>
 * <description>Software watchdog of IEC-task expired</description>
 */
#define RTSEXCPT_WATCHDOG					0x00000010

/**
 * <category>Exception code</category>
 * <description>Hardware watchdog expired. Global software error</description>
 */
#define RTSEXCPT_HARDWAREWATCHDOG			0x00000011

/**
 * <category>Exception code</category>
 * <description>IO-configuration error</description>
 */
#define RTSEXCPT_IO_CONFIG_ERROR			0x00000012

/**
 * <category>Exception code</category>
 * <description>Checksum error after program download</description>
 */
#define RTSEXCPT_PROGRAMCHECKSUM			0x00000013

/**
 * <category>Exception code</category>
 * <description>Field-bus error</description>
 */
#define RTSEXCPT_FIELDBUS_ERROR				0x00000014

/**
 * <category>Exception code</category>
 * <description>IO-update error</description>
 */
#define RTSEXCPT_IOUPDATE_ERROR				0x00000015

/**
 * <category>Exception code</category>
 * <description>Cycle time exceed</description>
 */
#define RTSEXCPT_CYCLE_TIME_EXCEED			0x00000016

/**
 * <category>Exception code</category>
 * <description>Online change program too large</description>
 */
#define RTSEXCPT_ONLCHANGE_PROGRAM_EXCEEDED 0x00000017

/**
 * <category>Exception code</category>
 * <description>Unresolved external references</description>
 */
#define RTSEXCPT_UNRESOLVED_EXTREFS			0x00000018

/**
 * <category>Exception code</category>
 * <description>Download was rejected</description>
 */
#define RTSEXCPT_DOWNLOAD_REJECTED			0x00000019

/**
 * <category>Exception code</category>
 * <description>Boot project not loaded because retain variables could not be relocated</description>
 */
#define RTSEXCPT_BOOTPROJECT_REJECTED_DUE_RETAIN_ERROR	0x0000001A

/**
 * <category>Exception code</category>
 * <description>Boot project not loaded and deleted</description>
 */
#define RTSEXCPT_LOADBOOTPROJECT_FAILED		0x0000001B

/**
 * <category>Exception code</category>
 * <description>Out of heap memory</description>
 */
#define RTSEXCPT_OUT_OF_MEMORY				0x0000001C

/**
 * <category>Exception code</category>
 * <description>Retain memory corrupt or cannot be mapped</description>
 */
#define RTSEXCPT_RETAIN_MEMORY_ERROR		0x0000001D

/**
 * <category>Exception code</category>
 * <description>Boot project that could be loaded but caused a crash later</description>
 */
#define RTSEXCPT_BOOTPROJECT_CRASH			0x0000001E

/**
 * <category>Exception code</category>
 * <description>Target of the boot application doesn't match the current target</description>
 */
#define RTSEXCPT_BOOTPROJECTTARGETMISMATCH	0x00000021

/**
 * <category>Exception code</category>
 * <description>Error at scheduling tasks</description>
 */
#define RTSEXCPT_SCHEDULEERROR				0x00000022

/**
 * <category>Exception code</category>
 * <description>Checksum of downloaded file does not match</description>
 */
#define RTSEXCPT_FILE_CHECKSUM_ERR			0x00000023

/**
 * <category>Exception code</category>
 * <description>Retain identity does not match to current boot application program identity</description>
 */
#define RTSEXCPT_RETAIN_IDENTITY_MISMATCH	0x00000024

/**
 * <category>Exception code</category>
 * <description>Iec task configuration failed</description>
 */
#define RTSEXCPT_IEC_TASK_CONFIG_ERROR		0x00000025

/**
 * <category>Exception code</category>
 * <description>Application is running on wrong target. Can be used for library protection!</description>
 */
#define RTSEXCPT_APP_TARGET_MISMATCH		0x00000026


/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Illegal instruction</description>
 */
#define RTSEXCPT_ILLEGAL_INSTRUCTION		0x00000050

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Access violation</description>
 */
#define RTSEXCPT_ACCESS_VIOLATION			0x00000051

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Privileged instruction</description>
 */
#define RTSEXCPT_PRIV_INSTRUCTION			0x00000052

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Page fault</description>
 */
#define RTSEXCPT_IN_PAGE_ERROR				0x00000053

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Stack overflow</description>
 */
#define RTSEXCPT_STACK_OVERFLOW				0x00000054

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Invalid disposition</description>
 */
#define RTSEXCPT_INVALID_DISPOSITION		0x00000055

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Invalid handle</description>
 */
#define RTSEXCPT_INVALID_HANDLE				0x00000056

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Guard page</description>
 */
#define RTSEXCPT_GUARD_PAGE					0x00000057

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Double fault</description>
 */
#define RTSEXCPT_DOUBLE_FAULT				0x00000058

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Invalid OpCode</description>
 */
#define RTSEXCPT_INVALID_OPCODE				0x00000059

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Data type misalignment</description>
 */
#define RTSEXCPT_MISALIGNMENT				0x00000100

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Array bounds exceeded</description>
 */
#define RTSEXCPT_ARRAYBOUNDS				0x00000101

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Division by zero</description>
 */
#define RTSEXCPT_DIVIDEBYZERO				0x00000102

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Overflow</description>
 */
#define RTSEXCPT_OVERFLOW					0x00000103

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Non continuable</description>
 */
#define RTSEXCPT_NONCONTINUABLE				0x00000104

/**
 * <category>Exception code</category>
 * <description>Processor load watchdog of all IEC-tasks detected</description>
 */
#define RTSEXCPT_PROCESSORLOAD_WATCHDOG		0x00000105


/**
 * <category>Exception code, hardware FPU exceptions</category>
 * <description>FPU: Unspecified error</description>
 */
#define RTSEXCPT_FPU_ERROR					0x00000150

/**
 * <category>Exception code, hardware FPU exceptions</category>
 * <description>FPU: De normalized operand</description>
 */
#define RTSEXCPT_FPU_DENORMAL_OPERAND		0x00000151

/**
 * <category>Exception code, hardware FPU exceptions</category>
 * <description>FPU: Division by zero</description>
 */
#define RTSEXCPT_FPU_DIVIDEBYZERO			0x00000152

/**
 * <category>Exception code, hardware FPU exceptions</category>
 * <description>FPU: Inexact result</description>
 */
#define RTSEXCPT_FPU_INEXACT_RESULT			0x00000153

/**
 * <category>Exception code, hardware FPU exceptions</category>
 * <description>FPU: Invalid operation</description>
 */
#define RTSEXCPT_FPU_INVALID_OPERATION		0x00000154

/**
 * <category>Exception code, hardware FPU exceptions</category>
 * <description>FPU: Overflow</description>
 */
#define RTSEXCPT_FPU_OVERFLOW				0x00000155

/**
 * <category>Exception code, hardware FPU exceptions</category>
 * <description>FPU: Stack check</description>
 */
#define RTSEXCPT_FPU_STACK_CHECK			0x00000156

/**
 * <category>Exception code, hardware FPU exceptions</category>
 * <description>FPU: Underflow</description>
 */
#define RTSEXCPT_FPU_UNDERFLOW				0x00000157

/**
 * <category>Exception code, hardware exceptions</category>
 * <description>Hardware breakpoint</description>
 */
#define RTSEXCPT_BREAKPOINT					0x00000200

/**
 * <category>Exception code</category>
 * <description>Masks all exception codes so far</description>
 */
#define RTSEXCPT_MASK						0x00000fff

/**
 * <category>Exception code, 3SLicense exceptions</category>
 * <description>3SLicense: License invalid</description>
 */
#define RTSEXCPT_LICENSE_INVALID			0x100D0001

/**
 * <category>Exception code</category>
 * <description>In addition to RTSEXCPT_WATCHDOG
 * in order to separate OmittedCycle watchdog from "normal" CycleExceed watchdog</description>
 */
#define RTSEXCPT_WATCHDOG_OMITTED_CYCLE		0x00001000

/**
 * <category>Exception code, vendor specific</category>
 * <description>Base number for vendor specific exception codes. VendorID must be specified as prefix
 *	inside the exception code:
 *	RTS_UI32 ulException = ADDVENDORID([VendorId], RTSEXCPT_VENDOR_EXCEPTION_BASE + [Exception]);
 * </description>
 */
#define RTSEXCPT_VENDOR_EXCEPTION_BASE		0x00002000

/**
 * <category>Exception code</category>
 * <description>Macro to test masked exception codes</description>
 */
#define SysExcptIsException(e, exception)	((e & RTSEXCPT_MASK) == exception)


/**
 * <category>Event parameter</category>
 * <description></description>
 *	<element name="uiTaskOSHandle" type="IN">Operating system specific task handle</element>
 *	<element name="ulException" type="IN">Exception number. See category "Exception code"</element>
 *	<element name="Context" type="IN">Register context, where the exception occurred</element>
 */
typedef struct
{
	RTS_HANDLE uiTaskOSHandle;
	RTS_IEC_DWORD ulException;
	RegContext Context;
	RTS_RESULT Result;
} EVTPARAM_SysExcept;
#define EVTPARAMID_SysExcept		0x0001
#define EVTVERSION_SysExcept		0x0002

/**
 * <category>Events</category>
 * <description>The event is sent if an exception occurred (see category exception code for 
 *	detailed information).
 *	NOTE:
 *	This event is fired for every exception! That means also for handled exceptions via SEH (Structured Exception Handling)
 *	with rts_try/rts_catch! 
 * </description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysExcept</param>
 */
#define EVT_EXCPT_GenerateException					MAKE_EVENTID(EVTCLASS_EXCEPTION, 1)

/**
 * <category>Events</category>
 * <description>The event is sent if an exception occurred (see category exception code for 
 *	detailed information).
 *	NOTE:
 *	This event is fired only for exceptions that are not handled via SEH (Structured Exception Handling)
 *	with rts_try/rts_catch! 
 * </description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysExcept</param>
 */
#define EVT_EXCPT_GenerateException2				MAKE_EVENTID(EVTCLASS_EXCEPTION, 2)


#ifdef __cplusplus
extern "C" {
#endif

/* Initialization routines for OS specific modules */
RTS_RESULT CDECL SysExceptOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysExceptOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>Map the operating system specific exception into the standard runtime exception code</description>
 * <param name="ulOSException" type="IN">Operating system specific exception code</param>
 * <result>Standard exception code (see category exception code above)</result>
 */
RTS_UI32 CDECL SysExceptMapException(RTS_UI32 ulOSException);
typedef RTS_UI32 (CDECL * PFSYSEXCEPTMAPEXCEPTION) (RTS_UI32 ulOSException);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTMAPEXCEPTION_NOTIMPLEMENTED)
	#define USE_SysExceptMapException
	#define EXT_SysExceptMapException
	#define GET_SysExceptMapException(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysExceptMapException(p0)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SysExceptMapException  FALSE
	#define EXP_SysExceptMapException  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysExceptMapException
	#define EXT_SysExceptMapException
	#define GET_SysExceptMapException(fl)  CAL_CMGETAPI( "SysExceptMapException" ) 
	#define CAL_SysExceptMapException  SysExceptMapException
	#define CHK_SysExceptMapException  TRUE
	#define EXP_SysExceptMapException  CAL_CMEXPAPI( "SysExceptMapException" ) 
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_SysExceptMapException
	#define EXT_SysExceptMapException
	#define GET_SysExceptMapException(fl)  CAL_CMGETAPI( "SysExceptMapException" ) 
	#define CAL_SysExceptMapException  SysExceptMapException
	#define CHK_SysExceptMapException  TRUE
	#define EXP_SysExceptMapException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptMapException", (RTS_UINTPTR)SysExceptMapException, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptSysExceptMapException
	#define EXT_SysExceptSysExceptMapException
	#define GET_SysExceptSysExceptMapException  ERR_OK
	#define CAL_SysExceptSysExceptMapException pISysExcept->ISysExceptMapException
	#define CHK_SysExceptSysExceptMapException (pISysExcept != NULL)
	#define EXP_SysExceptSysExceptMapException  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysExceptMapException
	#define EXT_SysExceptMapException
	#define GET_SysExceptMapException(fl)  CAL_CMGETAPI( "SysExceptMapException" ) 
	#define CAL_SysExceptMapException pISysExcept->ISysExceptMapException
	#define CHK_SysExceptMapException (pISysExcept != NULL)
	#define EXP_SysExceptMapException  CAL_CMEXPAPI( "SysExceptMapException" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysExceptMapException  PFSYSEXCEPTMAPEXCEPTION pfSysExceptMapException;
	#define EXT_SysExceptMapException  extern PFSYSEXCEPTMAPEXCEPTION pfSysExceptMapException;
	#define GET_SysExceptMapException(fl)  s_pfCMGetAPI2( "SysExceptMapException", (RTS_VOID_FCTPTR *)&pfSysExceptMapException, (fl), 0, 0)
	#define CAL_SysExceptMapException  pfSysExceptMapException
	#define CHK_SysExceptMapException  (pfSysExceptMapException != NULL)
	#define EXP_SysExceptMapException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptMapException", (RTS_UINTPTR)SysExceptMapException, 0, 0) 
#endif




/**
 * <description>
 * <p>Structured Exception Handling</p>
 * <p>This function is called from Exception handlers of the hardware,
 * operating systems or, in case of soft-errors, within the task code.
 * </p>
 * <p>The structured exception handling calls every registered exception
 * handler, stops the IEC Tasks and prepares the task context of the
 * task that generated the exception, so that it can be read by the
 * CoDeSys programming system for analysis purposes.
 * </p>
 * <p>OEM customers can use this function also from their own exception
 * handlers to propagate generic as well as customer specific exceptions
 * to the application. This way, they can profit from the debugging
 * infrastructure of CoDeSys to find the fault address within the IEC
 * application.</p>
 * <p><b>Note:</b> On SIL2 Platforms, this function will act only as
 * described above, when the PLC is currently in Debug-Mode. If it is
 * actually in the Safety-Mode, this call puts the PLC directly into
 * the safe mode, by calling the function SIL2OEMException().</p>
 * </description>
 * <param name="ulTaskOSHandle" type="IN" range="[RTS_INVALID_HANDLE,NULL,VALID_TASKHANDLE]">System specific task or timer handle or RTS_INVALID_HANDLE if unknown</param>
 * <param name="Exception" type="IN" range="[RTSEXCPT_UNKNOWN]">Exception code (OS or runtime specific)</param>
 * <param name="Context" type="IN" range="[NULL,VALID_CONTEXT]">Context to detect the code location where the exception was generated. If ulTaskOSHandle is RTS_INVALID_HANDLE, values can be 0.</param>
 * <parampseudo name="Mode" type="IN" range="[SAFETY,DEBUG]">Defines the current mode of the PLC.</parampseudo>
 * <parampseudo name="StructuredHandling" type="OUT">TRUE if structured Exception Handling was used.</parampseudo>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysExceptGenerateException(RTS_HANDLE ulTaskOSHandle, ExceptionCode Exception, RegContext Context);
typedef RTS_RESULT (CDECL * PFSYSEXCEPTGENERATEEXCEPTION) (RTS_HANDLE ulTaskOSHandle, ExceptionCode Exception, RegContext Context);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTGENERATEEXCEPTION_NOTIMPLEMENTED)
	#define USE_SysExceptGenerateException
	#define EXT_SysExceptGenerateException
	#define GET_SysExceptGenerateException(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysExceptGenerateException(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysExceptGenerateException  FALSE
	#define EXP_SysExceptGenerateException  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysExceptGenerateException
	#define EXT_SysExceptGenerateException
	#define GET_SysExceptGenerateException(fl)  CAL_CMGETAPI( "SysExceptGenerateException" ) 
	#define CAL_SysExceptGenerateException  SysExceptGenerateException
	#define CHK_SysExceptGenerateException  TRUE
	#define EXP_SysExceptGenerateException  CAL_CMEXPAPI( "SysExceptGenerateException" ) 
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_SysExceptGenerateException
	#define EXT_SysExceptGenerateException
	#define GET_SysExceptGenerateException(fl)  CAL_CMGETAPI( "SysExceptGenerateException" ) 
	#define CAL_SysExceptGenerateException  SysExceptGenerateException
	#define CHK_SysExceptGenerateException  TRUE
	#define EXP_SysExceptGenerateException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptGenerateException", (RTS_UINTPTR)SysExceptGenerateException, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptSysExceptGenerateException
	#define EXT_SysExceptSysExceptGenerateException
	#define GET_SysExceptSysExceptGenerateException  ERR_OK
	#define CAL_SysExceptSysExceptGenerateException pISysExcept->ISysExceptGenerateException
	#define CHK_SysExceptSysExceptGenerateException (pISysExcept != NULL)
	#define EXP_SysExceptSysExceptGenerateException  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysExceptGenerateException
	#define EXT_SysExceptGenerateException
	#define GET_SysExceptGenerateException(fl)  CAL_CMGETAPI( "SysExceptGenerateException" ) 
	#define CAL_SysExceptGenerateException pISysExcept->ISysExceptGenerateException
	#define CHK_SysExceptGenerateException (pISysExcept != NULL)
	#define EXP_SysExceptGenerateException  CAL_CMEXPAPI( "SysExceptGenerateException" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysExceptGenerateException  PFSYSEXCEPTGENERATEEXCEPTION pfSysExceptGenerateException;
	#define EXT_SysExceptGenerateException  extern PFSYSEXCEPTGENERATEEXCEPTION pfSysExceptGenerateException;
	#define GET_SysExceptGenerateException(fl)  s_pfCMGetAPI2( "SysExceptGenerateException", (RTS_VOID_FCTPTR *)&pfSysExceptGenerateException, (fl), 0, 0)
	#define CAL_SysExceptGenerateException  pfSysExceptGenerateException
	#define CHK_SysExceptGenerateException  (pfSysExceptGenerateException != NULL)
	#define EXP_SysExceptGenerateException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptGenerateException", (RTS_UINTPTR)SysExceptGenerateException, 0, 0) 
#endif




/**
 * <description>
 * <p>Structured Exception Handling</p>
 * <p>This function acts the same way as SysExceptGenerateException(),
 * except for the case, that it will try to suspend the task in the
 * following condition:
 * </p>
 * <ul>
 *   <li>bSuspendExceptionTask is set to TRUE</li>
 *   <li>None of the registered exception handlers returned ERR_DONT_SUSPEND_TASK</li>
 * </ul>
 * </description>
 * <param name="ulTaskOSHandle" type="IN" range="[RTS_INVALID_HANDLE,NULL,VALID_TASKHANDLE]">System specific task or timer handle or RTS_INVALID_HANDLE if unknown</param>
 * <param name="Exception" type="IN" range="[EXCEPTION_1]">Exception code (OS or runtime specific)</param>
 * <param name="Context" type="IN" range="[NULL,VALID_CONTEXT]">Context to detect the code location where the exception was generated. If ulTaskOSHandle is RTS_INVALID_HANDLE, values can be 0.</param>
 * <param name="bSuspendExceptionTask" type="IN" range="[0,1]">Flag to specify, if the exception task is forced to suspended</param>
 * <parampseudo name="bSafetyMode" type="IN" range="[0,1]">Defines the current mode of the PLC.</parampseudo>
 * <parampseudo name="bStructuredHandling" type="OUT">TRUE if structured Exception Handling was used.</parampseudo>
 * <errorcode name="RTS_RESULT" type="ERR_OK">None of the registered exception handlers returned ERR_DONT_SUSPEND_TASK</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DONT_SUSPEND_TASK">An exception handler suspends the task itself</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysExceptGenerateException2(RTS_HANDLE ulTaskOSHandle, ExceptionCode Exception, RegContext Context, int bSuspendExceptionTask);
typedef RTS_RESULT (CDECL * PFSYSEXCEPTGENERATEEXCEPTION2) (RTS_HANDLE ulTaskOSHandle, ExceptionCode Exception, RegContext Context, int bSuspendExceptionTask);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTGENERATEEXCEPTION2_NOTIMPLEMENTED)
	#define USE_SysExceptGenerateException2
	#define EXT_SysExceptGenerateException2
	#define GET_SysExceptGenerateException2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysExceptGenerateException2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysExceptGenerateException2  FALSE
	#define EXP_SysExceptGenerateException2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysExceptGenerateException2
	#define EXT_SysExceptGenerateException2
	#define GET_SysExceptGenerateException2(fl)  CAL_CMGETAPI( "SysExceptGenerateException2" ) 
	#define CAL_SysExceptGenerateException2  SysExceptGenerateException2
	#define CHK_SysExceptGenerateException2  TRUE
	#define EXP_SysExceptGenerateException2  CAL_CMEXPAPI( "SysExceptGenerateException2" ) 
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_SysExceptGenerateException2
	#define EXT_SysExceptGenerateException2
	#define GET_SysExceptGenerateException2(fl)  CAL_CMGETAPI( "SysExceptGenerateException2" ) 
	#define CAL_SysExceptGenerateException2  SysExceptGenerateException2
	#define CHK_SysExceptGenerateException2  TRUE
	#define EXP_SysExceptGenerateException2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptGenerateException2", (RTS_UINTPTR)SysExceptGenerateException2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptSysExceptGenerateException2
	#define EXT_SysExceptSysExceptGenerateException2
	#define GET_SysExceptSysExceptGenerateException2  ERR_OK
	#define CAL_SysExceptSysExceptGenerateException2 pISysExcept->ISysExceptGenerateException2
	#define CHK_SysExceptSysExceptGenerateException2 (pISysExcept != NULL)
	#define EXP_SysExceptSysExceptGenerateException2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysExceptGenerateException2
	#define EXT_SysExceptGenerateException2
	#define GET_SysExceptGenerateException2(fl)  CAL_CMGETAPI( "SysExceptGenerateException2" ) 
	#define CAL_SysExceptGenerateException2 pISysExcept->ISysExceptGenerateException2
	#define CHK_SysExceptGenerateException2 (pISysExcept != NULL)
	#define EXP_SysExceptGenerateException2  CAL_CMEXPAPI( "SysExceptGenerateException2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysExceptGenerateException2  PFSYSEXCEPTGENERATEEXCEPTION2 pfSysExceptGenerateException2;
	#define EXT_SysExceptGenerateException2  extern PFSYSEXCEPTGENERATEEXCEPTION2 pfSysExceptGenerateException2;
	#define GET_SysExceptGenerateException2(fl)  s_pfCMGetAPI2( "SysExceptGenerateException2", (RTS_VOID_FCTPTR *)&pfSysExceptGenerateException2, (fl), 0, 0)
	#define CAL_SysExceptGenerateException2  pfSysExceptGenerateException2
	#define CHK_SysExceptGenerateException2  (pfSysExceptGenerateException2 != NULL)
	#define EXP_SysExceptGenerateException2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptGenerateException2", (RTS_UINTPTR)SysExceptGenerateException2, 0, 0) 
#endif




/**
 * <description>Function to register an exception handler</description>
 * <param name="pExceptionHandler" type="IN">Pointer to exception handler</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysExceptRegisterInterface(PFEXCEPTIONHANDLER pExceptionHandler);
typedef RTS_RESULT (CDECL * PFSYSEXCEPTREGISTERINTERFACE) (PFEXCEPTIONHANDLER pExceptionHandler);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTREGISTERINTERFACE_NOTIMPLEMENTED)
	#define USE_SysExceptRegisterInterface
	#define EXT_SysExceptRegisterInterface
	#define GET_SysExceptRegisterInterface(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysExceptRegisterInterface(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysExceptRegisterInterface  FALSE
	#define EXP_SysExceptRegisterInterface  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysExceptRegisterInterface
	#define EXT_SysExceptRegisterInterface
	#define GET_SysExceptRegisterInterface(fl)  CAL_CMGETAPI( "SysExceptRegisterInterface" ) 
	#define CAL_SysExceptRegisterInterface  SysExceptRegisterInterface
	#define CHK_SysExceptRegisterInterface  TRUE
	#define EXP_SysExceptRegisterInterface  CAL_CMEXPAPI( "SysExceptRegisterInterface" ) 
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_SysExceptRegisterInterface
	#define EXT_SysExceptRegisterInterface
	#define GET_SysExceptRegisterInterface(fl)  CAL_CMGETAPI( "SysExceptRegisterInterface" ) 
	#define CAL_SysExceptRegisterInterface  SysExceptRegisterInterface
	#define CHK_SysExceptRegisterInterface  TRUE
	#define EXP_SysExceptRegisterInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptRegisterInterface", (RTS_UINTPTR)SysExceptRegisterInterface, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptSysExceptRegisterInterface
	#define EXT_SysExceptSysExceptRegisterInterface
	#define GET_SysExceptSysExceptRegisterInterface  ERR_OK
	#define CAL_SysExceptSysExceptRegisterInterface pISysExcept->ISysExceptRegisterInterface
	#define CHK_SysExceptSysExceptRegisterInterface (pISysExcept != NULL)
	#define EXP_SysExceptSysExceptRegisterInterface  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysExceptRegisterInterface
	#define EXT_SysExceptRegisterInterface
	#define GET_SysExceptRegisterInterface(fl)  CAL_CMGETAPI( "SysExceptRegisterInterface" ) 
	#define CAL_SysExceptRegisterInterface pISysExcept->ISysExceptRegisterInterface
	#define CHK_SysExceptRegisterInterface (pISysExcept != NULL)
	#define EXP_SysExceptRegisterInterface  CAL_CMEXPAPI( "SysExceptRegisterInterface" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysExceptRegisterInterface  PFSYSEXCEPTREGISTERINTERFACE pfSysExceptRegisterInterface;
	#define EXT_SysExceptRegisterInterface  extern PFSYSEXCEPTREGISTERINTERFACE pfSysExceptRegisterInterface;
	#define GET_SysExceptRegisterInterface(fl)  s_pfCMGetAPI2( "SysExceptRegisterInterface", (RTS_VOID_FCTPTR *)&pfSysExceptRegisterInterface, (fl), 0, 0)
	#define CAL_SysExceptRegisterInterface  pfSysExceptRegisterInterface
	#define CHK_SysExceptRegisterInterface  (pfSysExceptRegisterInterface != NULL)
	#define EXP_SysExceptRegisterInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptRegisterInterface", (RTS_UINTPTR)SysExceptRegisterInterface, 0, 0) 
#endif




/**
 * <description>This function registers or cleans up an exception context.
 * If this is the entry, it returns ERR_OK, otherwise ERR_FAILED</description>
 * <param name="context" type="IN">Context, describing one structured exception frame</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysExceptTry(SEHContext *context);
typedef RTS_RESULT (CDECL * PFSYSEXCEPTTRY) (SEHContext *context);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTTRY_NOTIMPLEMENTED)
	#define USE_SysExceptTry
	#define EXT_SysExceptTry
	#define GET_SysExceptTry(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysExceptTry(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysExceptTry  FALSE
	#define EXP_SysExceptTry  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysExceptTry
	#define EXT_SysExceptTry
	#define GET_SysExceptTry(fl)  CAL_CMGETAPI( "SysExceptTry" ) 
	#define CAL_SysExceptTry  SysExceptTry
	#define CHK_SysExceptTry  TRUE
	#define EXP_SysExceptTry  CAL_CMEXPAPI( "SysExceptTry" ) 
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_SysExceptTry
	#define EXT_SysExceptTry
	#define GET_SysExceptTry(fl)  CAL_CMGETAPI( "SysExceptTry" ) 
	#define CAL_SysExceptTry  SysExceptTry
	#define CHK_SysExceptTry  TRUE
	#define EXP_SysExceptTry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptTry", (RTS_UINTPTR)SysExceptTry, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptSysExceptTry
	#define EXT_SysExceptSysExceptTry
	#define GET_SysExceptSysExceptTry  ERR_OK
	#define CAL_SysExceptSysExceptTry pISysExcept->ISysExceptTry
	#define CHK_SysExceptSysExceptTry (pISysExcept != NULL)
	#define EXP_SysExceptSysExceptTry  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysExceptTry
	#define EXT_SysExceptTry
	#define GET_SysExceptTry(fl)  CAL_CMGETAPI( "SysExceptTry" ) 
	#define CAL_SysExceptTry pISysExcept->ISysExceptTry
	#define CHK_SysExceptTry (pISysExcept != NULL)
	#define EXP_SysExceptTry  CAL_CMEXPAPI( "SysExceptTry" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysExceptTry  PFSYSEXCEPTTRY pfSysExceptTry;
	#define EXT_SysExceptTry  extern PFSYSEXCEPTTRY pfSysExceptTry;
	#define GET_SysExceptTry(fl)  s_pfCMGetAPI2( "SysExceptTry", (RTS_VOID_FCTPTR *)&pfSysExceptTry, (fl), 0, 0)
	#define CAL_SysExceptTry  pfSysExceptTry
	#define CHK_SysExceptTry  (pfSysExceptTry != NULL)
	#define EXP_SysExceptTry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptTry", (RTS_UINTPTR)SysExceptTry, 0, 0) 
#endif




/**
 * <description>This function returns ERR_OK when the passed exception matches
 * the exception code that really occurred.</description>
 * <param name="context" type="IN">Context, describing one structured exception frame</param>
 * <param name="ui32ExceptionCode" type="IN">Exception code to match on</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysExceptCatchException(SEHContext *context, RTS_UI32 ui32ExceptionCode);
typedef RTS_RESULT (CDECL * PFSYSEXCEPTCATCHEXCEPTION) (SEHContext *context, RTS_UI32 ui32ExceptionCode);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTCATCHEXCEPTION_NOTIMPLEMENTED)
	#define USE_SysExceptCatchException
	#define EXT_SysExceptCatchException
	#define GET_SysExceptCatchException(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysExceptCatchException(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysExceptCatchException  FALSE
	#define EXP_SysExceptCatchException  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysExceptCatchException
	#define EXT_SysExceptCatchException
	#define GET_SysExceptCatchException(fl)  CAL_CMGETAPI( "SysExceptCatchException" ) 
	#define CAL_SysExceptCatchException  SysExceptCatchException
	#define CHK_SysExceptCatchException  TRUE
	#define EXP_SysExceptCatchException  CAL_CMEXPAPI( "SysExceptCatchException" ) 
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_SysExceptCatchException
	#define EXT_SysExceptCatchException
	#define GET_SysExceptCatchException(fl)  CAL_CMGETAPI( "SysExceptCatchException" ) 
	#define CAL_SysExceptCatchException  SysExceptCatchException
	#define CHK_SysExceptCatchException  TRUE
	#define EXP_SysExceptCatchException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptCatchException", (RTS_UINTPTR)SysExceptCatchException, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptSysExceptCatchException
	#define EXT_SysExceptSysExceptCatchException
	#define GET_SysExceptSysExceptCatchException  ERR_OK
	#define CAL_SysExceptSysExceptCatchException pISysExcept->ISysExceptCatchException
	#define CHK_SysExceptSysExceptCatchException (pISysExcept != NULL)
	#define EXP_SysExceptSysExceptCatchException  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysExceptCatchException
	#define EXT_SysExceptCatchException
	#define GET_SysExceptCatchException(fl)  CAL_CMGETAPI( "SysExceptCatchException" ) 
	#define CAL_SysExceptCatchException pISysExcept->ISysExceptCatchException
	#define CHK_SysExceptCatchException (pISysExcept != NULL)
	#define EXP_SysExceptCatchException  CAL_CMEXPAPI( "SysExceptCatchException" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysExceptCatchException  PFSYSEXCEPTCATCHEXCEPTION pfSysExceptCatchException;
	#define EXT_SysExceptCatchException  extern PFSYSEXCEPTCATCHEXCEPTION pfSysExceptCatchException;
	#define GET_SysExceptCatchException(fl)  s_pfCMGetAPI2( "SysExceptCatchException", (RTS_VOID_FCTPTR *)&pfSysExceptCatchException, (fl), 0, 0)
	#define CAL_SysExceptCatchException  pfSysExceptCatchException
	#define CHK_SysExceptCatchException  (pfSysExceptCatchException != NULL)
	#define EXP_SysExceptCatchException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptCatchException", (RTS_UINTPTR)SysExceptCatchException, 0, 0) 
#endif




/**
 * <description>This function returns ERR_OK if the exception was not yet handled.</description>
 * <param name="context" type="IN">Context, describing one structured exception frame</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysExceptCatch(SEHContext *context);
typedef RTS_RESULT (CDECL * PFSYSEXCEPTCATCH) (SEHContext *context);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTCATCH_NOTIMPLEMENTED)
	#define USE_SysExceptCatch
	#define EXT_SysExceptCatch
	#define GET_SysExceptCatch(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysExceptCatch(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysExceptCatch  FALSE
	#define EXP_SysExceptCatch  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysExceptCatch
	#define EXT_SysExceptCatch
	#define GET_SysExceptCatch(fl)  CAL_CMGETAPI( "SysExceptCatch" ) 
	#define CAL_SysExceptCatch  SysExceptCatch
	#define CHK_SysExceptCatch  TRUE
	#define EXP_SysExceptCatch  CAL_CMEXPAPI( "SysExceptCatch" ) 
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_SysExceptCatch
	#define EXT_SysExceptCatch
	#define GET_SysExceptCatch(fl)  CAL_CMGETAPI( "SysExceptCatch" ) 
	#define CAL_SysExceptCatch  SysExceptCatch
	#define CHK_SysExceptCatch  TRUE
	#define EXP_SysExceptCatch  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptCatch", (RTS_UINTPTR)SysExceptCatch, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptSysExceptCatch
	#define EXT_SysExceptSysExceptCatch
	#define GET_SysExceptSysExceptCatch  ERR_OK
	#define CAL_SysExceptSysExceptCatch pISysExcept->ISysExceptCatch
	#define CHK_SysExceptSysExceptCatch (pISysExcept != NULL)
	#define EXP_SysExceptSysExceptCatch  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysExceptCatch
	#define EXT_SysExceptCatch
	#define GET_SysExceptCatch(fl)  CAL_CMGETAPI( "SysExceptCatch" ) 
	#define CAL_SysExceptCatch pISysExcept->ISysExceptCatch
	#define CHK_SysExceptCatch (pISysExcept != NULL)
	#define EXP_SysExceptCatch  CAL_CMEXPAPI( "SysExceptCatch" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysExceptCatch  PFSYSEXCEPTCATCH pfSysExceptCatch;
	#define EXT_SysExceptCatch  extern PFSYSEXCEPTCATCH pfSysExceptCatch;
	#define GET_SysExceptCatch(fl)  s_pfCMGetAPI2( "SysExceptCatch", (RTS_VOID_FCTPTR *)&pfSysExceptCatch, (fl), 0, 0)
	#define CAL_SysExceptCatch  pfSysExceptCatch
	#define CHK_SysExceptCatch  (pfSysExceptCatch != NULL)
	#define EXP_SysExceptCatch  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptCatch", (RTS_UINTPTR)SysExceptCatch, 0, 0) 
#endif




/**
 * <description>Function to unregister exception handler</description>
 * <param name="pExceptionHandler" type="IN">Pointer to exception handler</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysExceptUnregisterInterface(PFEXCEPTIONHANDLER pExceptionHandler);
typedef RTS_RESULT (CDECL * PFSYSEXCEPTUNREGISTERINTERFACE) (PFEXCEPTIONHANDLER pExceptionHandler);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTUNREGISTERINTERFACE_NOTIMPLEMENTED)
	#define USE_SysExceptUnregisterInterface
	#define EXT_SysExceptUnregisterInterface
	#define GET_SysExceptUnregisterInterface(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysExceptUnregisterInterface(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysExceptUnregisterInterface  FALSE
	#define EXP_SysExceptUnregisterInterface  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysExceptUnregisterInterface
	#define EXT_SysExceptUnregisterInterface
	#define GET_SysExceptUnregisterInterface(fl)  CAL_CMGETAPI( "SysExceptUnregisterInterface" ) 
	#define CAL_SysExceptUnregisterInterface  SysExceptUnregisterInterface
	#define CHK_SysExceptUnregisterInterface  TRUE
	#define EXP_SysExceptUnregisterInterface  CAL_CMEXPAPI( "SysExceptUnregisterInterface" ) 
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_SysExceptUnregisterInterface
	#define EXT_SysExceptUnregisterInterface
	#define GET_SysExceptUnregisterInterface(fl)  CAL_CMGETAPI( "SysExceptUnregisterInterface" ) 
	#define CAL_SysExceptUnregisterInterface  SysExceptUnregisterInterface
	#define CHK_SysExceptUnregisterInterface  TRUE
	#define EXP_SysExceptUnregisterInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptUnregisterInterface", (RTS_UINTPTR)SysExceptUnregisterInterface, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptSysExceptUnregisterInterface
	#define EXT_SysExceptSysExceptUnregisterInterface
	#define GET_SysExceptSysExceptUnregisterInterface  ERR_OK
	#define CAL_SysExceptSysExceptUnregisterInterface pISysExcept->ISysExceptUnregisterInterface
	#define CHK_SysExceptSysExceptUnregisterInterface (pISysExcept != NULL)
	#define EXP_SysExceptSysExceptUnregisterInterface  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysExceptUnregisterInterface
	#define EXT_SysExceptUnregisterInterface
	#define GET_SysExceptUnregisterInterface(fl)  CAL_CMGETAPI( "SysExceptUnregisterInterface" ) 
	#define CAL_SysExceptUnregisterInterface pISysExcept->ISysExceptUnregisterInterface
	#define CHK_SysExceptUnregisterInterface (pISysExcept != NULL)
	#define EXP_SysExceptUnregisterInterface  CAL_CMEXPAPI( "SysExceptUnregisterInterface" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysExceptUnregisterInterface  PFSYSEXCEPTUNREGISTERINTERFACE pfSysExceptUnregisterInterface;
	#define EXT_SysExceptUnregisterInterface  extern PFSYSEXCEPTUNREGISTERINTERFACE pfSysExceptUnregisterInterface;
	#define GET_SysExceptUnregisterInterface(fl)  s_pfCMGetAPI2( "SysExceptUnregisterInterface", (RTS_VOID_FCTPTR *)&pfSysExceptUnregisterInterface, (fl), 0, 0)
	#define CAL_SysExceptUnregisterInterface  pfSysExceptUnregisterInterface
	#define CHK_SysExceptUnregisterInterface  (pfSysExceptUnregisterInterface != NULL)
	#define EXP_SysExceptUnregisterInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptUnregisterInterface", (RTS_UINTPTR)SysExceptUnregisterInterface, 0, 0) 
#endif




/**
 * <description>Convert an exception to string</description>
 * <param name="ulExceptionCode" type="IN">Exception code</param>
 * <param name="pszException" type="OUT">Pointer to exception string</param>
 * <param name="iMaxExceptionLen" type="IN">Maximum length of exception string pointer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysExceptConvertToString(RTS_UI32 ulExceptionCode, char*pszException, int iMaxExceptionLen);
typedef RTS_RESULT (CDECL * PFSYSEXCEPTCONVERTTOSTRING) (RTS_UI32 ulExceptionCode, char*pszException, int iMaxExceptionLen);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTCONVERTTOSTRING_NOTIMPLEMENTED)
	#define USE_SysExceptConvertToString
	#define EXT_SysExceptConvertToString
	#define GET_SysExceptConvertToString(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysExceptConvertToString(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysExceptConvertToString  FALSE
	#define EXP_SysExceptConvertToString  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysExceptConvertToString
	#define EXT_SysExceptConvertToString
	#define GET_SysExceptConvertToString(fl)  CAL_CMGETAPI( "SysExceptConvertToString" ) 
	#define CAL_SysExceptConvertToString  SysExceptConvertToString
	#define CHK_SysExceptConvertToString  TRUE
	#define EXP_SysExceptConvertToString  CAL_CMEXPAPI( "SysExceptConvertToString" ) 
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_SysExceptConvertToString
	#define EXT_SysExceptConvertToString
	#define GET_SysExceptConvertToString(fl)  CAL_CMGETAPI( "SysExceptConvertToString" ) 
	#define CAL_SysExceptConvertToString  SysExceptConvertToString
	#define CHK_SysExceptConvertToString  TRUE
	#define EXP_SysExceptConvertToString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptConvertToString", (RTS_UINTPTR)SysExceptConvertToString, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptSysExceptConvertToString
	#define EXT_SysExceptSysExceptConvertToString
	#define GET_SysExceptSysExceptConvertToString  ERR_OK
	#define CAL_SysExceptSysExceptConvertToString pISysExcept->ISysExceptConvertToString
	#define CHK_SysExceptSysExceptConvertToString (pISysExcept != NULL)
	#define EXP_SysExceptSysExceptConvertToString  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysExceptConvertToString
	#define EXT_SysExceptConvertToString
	#define GET_SysExceptConvertToString(fl)  CAL_CMGETAPI( "SysExceptConvertToString" ) 
	#define CAL_SysExceptConvertToString pISysExcept->ISysExceptConvertToString
	#define CHK_SysExceptConvertToString (pISysExcept != NULL)
	#define EXP_SysExceptConvertToString  CAL_CMEXPAPI( "SysExceptConvertToString" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysExceptConvertToString  PFSYSEXCEPTCONVERTTOSTRING pfSysExceptConvertToString;
	#define EXT_SysExceptConvertToString  extern PFSYSEXCEPTCONVERTTOSTRING pfSysExceptConvertToString;
	#define GET_SysExceptConvertToString(fl)  s_pfCMGetAPI2( "SysExceptConvertToString", (RTS_VOID_FCTPTR *)&pfSysExceptConvertToString, (fl), 0, 0)
	#define CAL_SysExceptConvertToString  pfSysExceptConvertToString
	#define CHK_SysExceptConvertToString  (pfSysExceptConvertToString != NULL)
	#define EXP_SysExceptConvertToString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysExceptConvertToString", (RTS_UINTPTR)SysExceptConvertToString, 0, 0) 
#endif




/** EXTERN LIB SECTION BEGIN **/

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Interrupt the execution of the currently running
 * task, and set the application into an exception state. If the
 * runtime system supports it, CODESYS may even highlight the source
 * position where this function was called.</description>
 * <result><p>RESULT: If succeeded this function doesn't return at all, otherwise it returns ERR_FAILED.</p></result>
 */
typedef struct tagsysexceptgenerateexception_struct
{
	RTS_IEC_UDINT udiException;			/* VAR_INPUT */	
	RTS_IEC_RESULT SysExceptGenerateException;	/* VAR_OUTPUT */	
} sysexceptgenerateexception_struct;

void CDECL CDECL_EXT sysexceptgenerateexception(sysexceptgenerateexception_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSEXCEPTGENERATEEXCEPTION_IEC) (sysexceptgenerateexception_struct *p);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTGENERATEEXCEPTION_NOTIMPLEMENTED)
	#define USE_sysexceptgenerateexception
	#define EXT_sysexceptgenerateexception
	#define GET_sysexceptgenerateexception(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysexceptgenerateexception(p0) 
	#define CHK_sysexceptgenerateexception  FALSE
	#define EXP_sysexceptgenerateexception  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysexceptgenerateexception
	#define EXT_sysexceptgenerateexception
	#define GET_sysexceptgenerateexception(fl)  CAL_CMGETAPI( "sysexceptgenerateexception" ) 
	#define CAL_sysexceptgenerateexception  sysexceptgenerateexception
	#define CHK_sysexceptgenerateexception  TRUE
	#define EXP_sysexceptgenerateexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptgenerateexception", (RTS_UINTPTR)sysexceptgenerateexception, 1, 0xE91614FA, 0x03051100) 
	#define EXTREF_ITF_sysexceptgenerateexception {(RTS_VOID_FCTPTR)sysexceptgenerateexception, "sysexceptgenerateexception", 0xE91614FA, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_sysexceptgenerateexception
	#define EXT_sysexceptgenerateexception
	#define GET_sysexceptgenerateexception(fl)  CAL_CMGETAPI( "sysexceptgenerateexception" ) 
	#define CAL_sysexceptgenerateexception  sysexceptgenerateexception
	#define CHK_sysexceptgenerateexception  TRUE
	#define EXP_sysexceptgenerateexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptgenerateexception", (RTS_UINTPTR)sysexceptgenerateexception, 1, 0xE91614FA, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptsysexceptgenerateexception
	#define EXT_SysExceptsysexceptgenerateexception
	#define GET_SysExceptsysexceptgenerateexception  ERR_OK
	#define CAL_SysExceptsysexceptgenerateexception  sysexceptgenerateexception
	#define CHK_SysExceptsysexceptgenerateexception  TRUE
	#define EXP_SysExceptsysexceptgenerateexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptgenerateexception", (RTS_UINTPTR)sysexceptgenerateexception, 1, 0xE91614FA, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysexceptgenerateexception
	#define EXT_sysexceptgenerateexception
	#define GET_sysexceptgenerateexception(fl)  CAL_CMGETAPI( "sysexceptgenerateexception" ) 
	#define CAL_sysexceptgenerateexception  sysexceptgenerateexception
	#define CHK_sysexceptgenerateexception  TRUE
	#define EXP_sysexceptgenerateexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptgenerateexception", (RTS_UINTPTR)sysexceptgenerateexception, 1, 0xE91614FA, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysexceptgenerateexception  PFSYSEXCEPTGENERATEEXCEPTION_IEC pfsysexceptgenerateexception;
	#define EXT_sysexceptgenerateexception  extern PFSYSEXCEPTGENERATEEXCEPTION_IEC pfsysexceptgenerateexception;
	#define GET_sysexceptgenerateexception(fl)  s_pfCMGetAPI2( "sysexceptgenerateexception", (RTS_VOID_FCTPTR *)&pfsysexceptgenerateexception, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xE91614FA, 0x03051100)
	#define CAL_sysexceptgenerateexception  pfsysexceptgenerateexception
	#define CHK_sysexceptgenerateexception  (pfsysexceptgenerateexception != NULL)
	#define EXP_sysexceptgenerateexception   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptgenerateexception", (RTS_UINTPTR)sysexceptgenerateexception, 1, 0xE91614FA, 0x03051100) 
#endif


/**
 * <description>Obsolete function</description>
 */
typedef struct tagsysexceptdisableseh_struct
{
	RTS_IEC_RESULT SysExceptDisableSEH;	/* VAR_OUTPUT */	
} sysexceptdisableseh_struct;

void CDECL CDECL_EXT sysexceptdisableseh(sysexceptdisableseh_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSEXCEPTDISABLESEH_IEC) (sysexceptdisableseh_struct *p);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTDISABLESEH_NOTIMPLEMENTED)
	#define USE_sysexceptdisableseh
	#define EXT_sysexceptdisableseh
	#define GET_sysexceptdisableseh(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysexceptdisableseh(p0) 
	#define CHK_sysexceptdisableseh  FALSE
	#define EXP_sysexceptdisableseh  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysexceptdisableseh
	#define EXT_sysexceptdisableseh
	#define GET_sysexceptdisableseh(fl)  CAL_CMGETAPI( "sysexceptdisableseh" ) 
	#define CAL_sysexceptdisableseh  sysexceptdisableseh
	#define CHK_sysexceptdisableseh  TRUE
	#define EXP_sysexceptdisableseh  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh", (RTS_UINTPTR)sysexceptdisableseh, 1, 0x8C7B1A37, 0x03051100) 
	#define EXTREF_ITF_sysexceptdisableseh {(RTS_VOID_FCTPTR)sysexceptdisableseh, "sysexceptdisableseh", 0x8C7B1A37, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_sysexceptdisableseh
	#define EXT_sysexceptdisableseh
	#define GET_sysexceptdisableseh(fl)  CAL_CMGETAPI( "sysexceptdisableseh" ) 
	#define CAL_sysexceptdisableseh  sysexceptdisableseh
	#define CHK_sysexceptdisableseh  TRUE
	#define EXP_sysexceptdisableseh  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh", (RTS_UINTPTR)sysexceptdisableseh, 1, 0x8C7B1A37, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptsysexceptdisableseh
	#define EXT_SysExceptsysexceptdisableseh
	#define GET_SysExceptsysexceptdisableseh  ERR_OK
	#define CAL_SysExceptsysexceptdisableseh  sysexceptdisableseh
	#define CHK_SysExceptsysexceptdisableseh  TRUE
	#define EXP_SysExceptsysexceptdisableseh  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh", (RTS_UINTPTR)sysexceptdisableseh, 1, 0x8C7B1A37, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysexceptdisableseh
	#define EXT_sysexceptdisableseh
	#define GET_sysexceptdisableseh(fl)  CAL_CMGETAPI( "sysexceptdisableseh" ) 
	#define CAL_sysexceptdisableseh  sysexceptdisableseh
	#define CHK_sysexceptdisableseh  TRUE
	#define EXP_sysexceptdisableseh  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh", (RTS_UINTPTR)sysexceptdisableseh, 1, 0x8C7B1A37, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysexceptdisableseh  PFSYSEXCEPTDISABLESEH_IEC pfsysexceptdisableseh;
	#define EXT_sysexceptdisableseh  extern PFSYSEXCEPTDISABLESEH_IEC pfsysexceptdisableseh;
	#define GET_sysexceptdisableseh(fl)  s_pfCMGetAPI2( "sysexceptdisableseh", (RTS_VOID_FCTPTR *)&pfsysexceptdisableseh, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x8C7B1A37, 0x03051100)
	#define CAL_sysexceptdisableseh  pfsysexceptdisableseh
	#define CHK_sysexceptdisableseh  (pfsysexceptdisableseh != NULL)
	#define EXP_sysexceptdisableseh   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh", (RTS_UINTPTR)sysexceptdisableseh, 1, 0x8C7B1A37, 0x03051100) 
#endif


/**
 * <description>Obsolete function</description>
 */
typedef struct tagsysexceptdisableseh2_struct
{
	RTS_IEC_BYTE *pfExceptionHandler;	/* VAR_INPUT */	/* <param name="pfExceptionHandler" type="IN">Function pointer to the exception handler</param> */
	RTS_IEC_RESULT SysExceptDisableSEH2;	/* VAR_OUTPUT */	
} sysexceptdisableseh2_struct;

void CDECL CDECL_EXT sysexceptdisableseh2(sysexceptdisableseh2_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSEXCEPTDISABLESEH2_IEC) (sysexceptdisableseh2_struct *p);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTDISABLESEH2_NOTIMPLEMENTED)
	#define USE_sysexceptdisableseh2
	#define EXT_sysexceptdisableseh2
	#define GET_sysexceptdisableseh2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysexceptdisableseh2(p0) 
	#define CHK_sysexceptdisableseh2  FALSE
	#define EXP_sysexceptdisableseh2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysexceptdisableseh2
	#define EXT_sysexceptdisableseh2
	#define GET_sysexceptdisableseh2(fl)  CAL_CMGETAPI( "sysexceptdisableseh2" ) 
	#define CAL_sysexceptdisableseh2  sysexceptdisableseh2
	#define CHK_sysexceptdisableseh2  TRUE
	#define EXP_sysexceptdisableseh2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh2", (RTS_UINTPTR)sysexceptdisableseh2, 1, RTSITF_GET_SIGNATURE(0, 0xB55730A3), 0x03051100) 
	#define EXTREF_ITF_sysexceptdisableseh2 {(RTS_VOID_FCTPTR)sysexceptdisableseh2, "sysexceptdisableseh2", RTSITF_GET_SIGNATURE(0, 0xB55730A3), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_sysexceptdisableseh2
	#define EXT_sysexceptdisableseh2
	#define GET_sysexceptdisableseh2(fl)  CAL_CMGETAPI( "sysexceptdisableseh2" ) 
	#define CAL_sysexceptdisableseh2  sysexceptdisableseh2
	#define CHK_sysexceptdisableseh2  TRUE
	#define EXP_sysexceptdisableseh2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh2", (RTS_UINTPTR)sysexceptdisableseh2, 1, RTSITF_GET_SIGNATURE(0, 0xB55730A3), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptsysexceptdisableseh2
	#define EXT_SysExceptsysexceptdisableseh2
	#define GET_SysExceptsysexceptdisableseh2  ERR_OK
	#define CAL_SysExceptsysexceptdisableseh2  sysexceptdisableseh2
	#define CHK_SysExceptsysexceptdisableseh2  TRUE
	#define EXP_SysExceptsysexceptdisableseh2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh2", (RTS_UINTPTR)sysexceptdisableseh2, 1, RTSITF_GET_SIGNATURE(0, 0xB55730A3), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysexceptdisableseh2
	#define EXT_sysexceptdisableseh2
	#define GET_sysexceptdisableseh2(fl)  CAL_CMGETAPI( "sysexceptdisableseh2" ) 
	#define CAL_sysexceptdisableseh2  sysexceptdisableseh2
	#define CHK_sysexceptdisableseh2  TRUE
	#define EXP_sysexceptdisableseh2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh2", (RTS_UINTPTR)sysexceptdisableseh2, 1, RTSITF_GET_SIGNATURE(0, 0xB55730A3), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysexceptdisableseh2  PFSYSEXCEPTDISABLESEH2_IEC pfsysexceptdisableseh2;
	#define EXT_sysexceptdisableseh2  extern PFSYSEXCEPTDISABLESEH2_IEC pfsysexceptdisableseh2;
	#define GET_sysexceptdisableseh2(fl)  s_pfCMGetAPI2( "sysexceptdisableseh2", (RTS_VOID_FCTPTR *)&pfsysexceptdisableseh2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB55730A3), 0x03051100)
	#define CAL_sysexceptdisableseh2  pfsysexceptdisableseh2
	#define CHK_sysexceptdisableseh2  (pfsysexceptdisableseh2 != NULL)
	#define EXP_sysexceptdisableseh2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptdisableseh2", (RTS_UINTPTR)sysexceptdisableseh2, 1, RTSITF_GET_SIGNATURE(0, 0xB55730A3), 0x03051100) 
#endif


/**
 * <description>Obsolete function</description>
 */
typedef struct tagsysexceptenableseh_struct
{
	RTS_IEC_RESULT SysExceptEnableSEH;	/* VAR_OUTPUT */	
} sysexceptenableseh_struct;

void CDECL CDECL_EXT sysexceptenableseh(sysexceptenableseh_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSEXCEPTENABLESEH_IEC) (sysexceptenableseh_struct *p);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTENABLESEH_NOTIMPLEMENTED)
	#define USE_sysexceptenableseh
	#define EXT_sysexceptenableseh
	#define GET_sysexceptenableseh(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysexceptenableseh(p0) 
	#define CHK_sysexceptenableseh  FALSE
	#define EXP_sysexceptenableseh  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysexceptenableseh
	#define EXT_sysexceptenableseh
	#define GET_sysexceptenableseh(fl)  CAL_CMGETAPI( "sysexceptenableseh" ) 
	#define CAL_sysexceptenableseh  sysexceptenableseh
	#define CHK_sysexceptenableseh  TRUE
	#define EXP_sysexceptenableseh  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh", (RTS_UINTPTR)sysexceptenableseh, 1, 0x41742724, 0x03051100) 
	#define EXTREF_ITF_sysexceptenableseh {(RTS_VOID_FCTPTR)sysexceptenableseh, "sysexceptenableseh", 0x41742724, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_sysexceptenableseh
	#define EXT_sysexceptenableseh
	#define GET_sysexceptenableseh(fl)  CAL_CMGETAPI( "sysexceptenableseh" ) 
	#define CAL_sysexceptenableseh  sysexceptenableseh
	#define CHK_sysexceptenableseh  TRUE
	#define EXP_sysexceptenableseh  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh", (RTS_UINTPTR)sysexceptenableseh, 1, 0x41742724, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptsysexceptenableseh
	#define EXT_SysExceptsysexceptenableseh
	#define GET_SysExceptsysexceptenableseh  ERR_OK
	#define CAL_SysExceptsysexceptenableseh  sysexceptenableseh
	#define CHK_SysExceptsysexceptenableseh  TRUE
	#define EXP_SysExceptsysexceptenableseh  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh", (RTS_UINTPTR)sysexceptenableseh, 1, 0x41742724, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysexceptenableseh
	#define EXT_sysexceptenableseh
	#define GET_sysexceptenableseh(fl)  CAL_CMGETAPI( "sysexceptenableseh" ) 
	#define CAL_sysexceptenableseh  sysexceptenableseh
	#define CHK_sysexceptenableseh  TRUE
	#define EXP_sysexceptenableseh  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh", (RTS_UINTPTR)sysexceptenableseh, 1, 0x41742724, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysexceptenableseh  PFSYSEXCEPTENABLESEH_IEC pfsysexceptenableseh;
	#define EXT_sysexceptenableseh  extern PFSYSEXCEPTENABLESEH_IEC pfsysexceptenableseh;
	#define GET_sysexceptenableseh(fl)  s_pfCMGetAPI2( "sysexceptenableseh", (RTS_VOID_FCTPTR *)&pfsysexceptenableseh, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x41742724, 0x03051100)
	#define CAL_sysexceptenableseh  pfsysexceptenableseh
	#define CHK_sysexceptenableseh  (pfsysexceptenableseh != NULL)
	#define EXP_sysexceptenableseh   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh", (RTS_UINTPTR)sysexceptenableseh, 1, 0x41742724, 0x03051100) 
#endif


/**
 * <description>Obsolete function</description>
 */
typedef struct tagsysexceptenableseh2_struct
{
	RTS_IEC_BYTE *pfExceptionHandler;	/* VAR_INPUT */	/* <param name="pfExceptionHandler" type="IN">Function pointer to the exception handler</param> */
	RTS_IEC_RESULT SysExceptEnableSEH2;	/* VAR_OUTPUT */	
} sysexceptenableseh2_struct;

void CDECL CDECL_EXT sysexceptenableseh2(sysexceptenableseh2_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSEXCEPTENABLESEH2_IEC) (sysexceptenableseh2_struct *p);
#if defined(SYSEXCEPT_NOTIMPLEMENTED) || defined(SYSEXCEPTENABLESEH2_NOTIMPLEMENTED)
	#define USE_sysexceptenableseh2
	#define EXT_sysexceptenableseh2
	#define GET_sysexceptenableseh2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysexceptenableseh2(p0) 
	#define CHK_sysexceptenableseh2  FALSE
	#define EXP_sysexceptenableseh2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysexceptenableseh2
	#define EXT_sysexceptenableseh2
	#define GET_sysexceptenableseh2(fl)  CAL_CMGETAPI( "sysexceptenableseh2" ) 
	#define CAL_sysexceptenableseh2  sysexceptenableseh2
	#define CHK_sysexceptenableseh2  TRUE
	#define EXP_sysexceptenableseh2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh2", (RTS_UINTPTR)sysexceptenableseh2, 1, RTSITF_GET_SIGNATURE(0, 0x5FF166F1), 0x03051100) 
	#define EXTREF_ITF_sysexceptenableseh2 {(RTS_VOID_FCTPTR)sysexceptenableseh2, "sysexceptenableseh2", RTSITF_GET_SIGNATURE(0, 0x5FF166F1), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSEXCEPT_EXTERNAL)
	#define USE_sysexceptenableseh2
	#define EXT_sysexceptenableseh2
	#define GET_sysexceptenableseh2(fl)  CAL_CMGETAPI( "sysexceptenableseh2" ) 
	#define CAL_sysexceptenableseh2  sysexceptenableseh2
	#define CHK_sysexceptenableseh2  TRUE
	#define EXP_sysexceptenableseh2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh2", (RTS_UINTPTR)sysexceptenableseh2, 1, RTSITF_GET_SIGNATURE(0, 0x5FF166F1), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysExceptsysexceptenableseh2
	#define EXT_SysExceptsysexceptenableseh2
	#define GET_SysExceptsysexceptenableseh2  ERR_OK
	#define CAL_SysExceptsysexceptenableseh2  sysexceptenableseh2
	#define CHK_SysExceptsysexceptenableseh2  TRUE
	#define EXP_SysExceptsysexceptenableseh2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh2", (RTS_UINTPTR)sysexceptenableseh2, 1, RTSITF_GET_SIGNATURE(0, 0x5FF166F1), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysexceptenableseh2
	#define EXT_sysexceptenableseh2
	#define GET_sysexceptenableseh2(fl)  CAL_CMGETAPI( "sysexceptenableseh2" ) 
	#define CAL_sysexceptenableseh2  sysexceptenableseh2
	#define CHK_sysexceptenableseh2  TRUE
	#define EXP_sysexceptenableseh2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh2", (RTS_UINTPTR)sysexceptenableseh2, 1, RTSITF_GET_SIGNATURE(0, 0x5FF166F1), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysexceptenableseh2  PFSYSEXCEPTENABLESEH2_IEC pfsysexceptenableseh2;
	#define EXT_sysexceptenableseh2  extern PFSYSEXCEPTENABLESEH2_IEC pfsysexceptenableseh2;
	#define GET_sysexceptenableseh2(fl)  s_pfCMGetAPI2( "sysexceptenableseh2", (RTS_VOID_FCTPTR *)&pfsysexceptenableseh2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x5FF166F1), 0x03051100)
	#define CAL_sysexceptenableseh2  pfsysexceptenableseh2
	#define CHK_sysexceptenableseh2  (pfsysexceptenableseh2 != NULL)
	#define EXP_sysexceptenableseh2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysexceptenableseh2", (RTS_UINTPTR)sysexceptenableseh2, 1, RTSITF_GET_SIGNATURE(0, 0x5FF166F1), 0x03051100) 
#endif



#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef RTS_STRUCTURED_EXCEPTION_HANDLING
	/**
	 * <description>Structured exception handling:
	 * rts_try:             macro to enter an exception frame
	 * rts_catch_exception: macro to catch a specific exception
	 * rts_catch:           macro to catch all exception codes
	 * rts_try_end:         macro to close the block of the exception frame
	 * </description>
	 */

	#ifndef EXCPT_GET_CODE
		#define EXCPT_GET_CODE() 	__SEHContext.ui32ExceptionCode
	#endif

	#ifndef EXCPT_GET_CONTEXT
		#define EXCPT_GET_CONTEXT() &__SEHContext.context
	#endif

	#ifndef rts_try
		#define rts_try \
		{ \
			SEHContext __SEHContext; \
			__SEHContext.bRegistered = 0; \
			RTS_GET_FPENV(&__SEHContext.fpenvbuf) \
			__SEHContext.ui32ExceptionCode = RTS_SETJMP(__SEHContext.jmpbuf); \
			if(__SEHContext.ui32ExceptionCode != 0) \
				{RTS_SET_FPENV(&__SEHContext.fpenvbuf)} \
			while (CAL_SysExceptTry(&__SEHContext) == ERR_OK)

	#endif


	#ifndef rts_catch_exception
		#define rts_catch_exception(ui32ExceptionCode) \
			if (CAL_SysExceptCatchException(&__SEHContext, ui32ExceptionCode) == ERR_OK)
	#endif

	#ifndef rts_catch
		#define rts_catch \
			if (CAL_SysExceptCatch(&__SEHContext) == ERR_OK)
	#endif

	#ifndef rts_try_end
		#define rts_try_end \
		}
	#endif
#else	/*RTS_STRUCTURED_EXCEPTION_HANDLING*/

	#define EXCPT_GET_CODE()                       RTSEXCPT_NOEXCEPTION
	#define EXCPT_GET_CONTEXT()                    NULL
	#define rts_try                                {
	#define rts_catch_exception(ui32ExceptionCode) if (0)
	#define rts_catch                              if (0)
	#define rts_try_end                            }
#endif

#ifdef __cplusplus
}
#endif




typedef struct
{
	IBase_C *pBase;
	PFSYSEXCEPTMAPEXCEPTION ISysExceptMapException;
 	PFSYSEXCEPTGENERATEEXCEPTION ISysExceptGenerateException;
 	PFSYSEXCEPTGENERATEEXCEPTION2 ISysExceptGenerateException2;
 	PFSYSEXCEPTREGISTERINTERFACE ISysExceptRegisterInterface;
 	PFSYSEXCEPTTRY ISysExceptTry;
 	PFSYSEXCEPTCATCHEXCEPTION ISysExceptCatchException;
 	PFSYSEXCEPTCATCH ISysExceptCatch;
 	PFSYSEXCEPTUNREGISTERINTERFACE ISysExceptUnregisterInterface;
 	PFSYSEXCEPTCONVERTTOSTRING ISysExceptConvertToString;
 } ISysExcept_C;

#ifdef CPLUSPLUS
class ISysExcept : public IBase
{
	public:
		virtual RTS_UI32 CDECL ISysExceptMapException(RTS_UI32 ulOSException) =0;
		virtual RTS_RESULT CDECL ISysExceptGenerateException(RTS_HANDLE ulTaskOSHandle, ExceptionCode Exception, RegContext Context) =0;
		virtual RTS_RESULT CDECL ISysExceptGenerateException2(RTS_HANDLE ulTaskOSHandle, ExceptionCode Exception, RegContext Context, int bSuspendExceptionTask) =0;
		virtual RTS_RESULT CDECL ISysExceptRegisterInterface(PFEXCEPTIONHANDLER pExceptionHandler) =0;
		virtual RTS_RESULT CDECL ISysExceptTry(SEHContext *context) =0;
		virtual RTS_RESULT CDECL ISysExceptCatchException(SEHContext *context, RTS_UI32 ui32ExceptionCode) =0;
		virtual RTS_RESULT CDECL ISysExceptCatch(SEHContext *context) =0;
		virtual RTS_RESULT CDECL ISysExceptUnregisterInterface(PFEXCEPTIONHANDLER pExceptionHandler) =0;
		virtual RTS_RESULT CDECL ISysExceptConvertToString(RTS_UI32 ulExceptionCode, char*pszException, int iMaxExceptionLen) =0;
};
	#ifndef ITF_SysExcept
		#define ITF_SysExcept static ISysExcept *pISysExcept = NULL;
	#endif
	#define EXTITF_SysExcept
#else	/*CPLUSPLUS*/
	typedef ISysExcept_C		ISysExcept;
	#ifndef ITF_SysExcept
		#define ITF_SysExcept
	#endif
	#define EXTITF_SysExcept
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSEXCEPTITF_H_*/
