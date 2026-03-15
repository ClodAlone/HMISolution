#ifndef __SYSDEFINES__H__
#define __SYSDEFINES__H__

#ifdef _WINDOWS
	#ifndef WIN32
		#define WIN32
	#endif
#endif

#if defined(WIN32) || defined (_WIN32_WCE)
#ifdef CPLUSPLUS
	#pragma warning( disable: 4611 )
#endif
	#pragma warning( disable: 4100 )	/* Disable warning messages in Visual C++ */
	#pragma warning( disable: 4152 )	/* Disable warning messages in Visual C++ */
	#pragma warning( disable: 4505 )	/* Disable warning messages in Visual C++ */
	#pragma warning( disable: 4127 )	/* Disable warning messages in Visual C++ */
	#pragma warning( disable: 4996 )	/* Disable warning messages in Visual C++ */

	#pragma warning( disable: 4214 )	/* Warning only disabled because of CAA components. If this is fixed, this warning can be removed. */
	#pragma warning( disable: 4054 )	/* Warning only disabled because of wrong interfaces in SysCpuHandling and SysApp, after fix => remove */
	#pragma warning( disable: 4055 )	/* Warning only disabled because of type cast warning in CmpApp */

#if (_MSC_VER >= 1400)
	#pragma warning( disable: 4131 )	/* uses old-style declarator */
	#pragma warning( disable: 6237 )	/* (<zero> && <expression>) is always zero. <expression> is never evaluated and may have side effects */
	#pragma warning( disable: 6240 )	/* (<expression> && <non-zero constant>) always evaluates to the result of <expression>. Did you intend to use the bitwise-and operator? */
	#pragma warning( disable: 6258 )	/* using TerminateThread does not allow proper thread clean up. */
	#pragma warning( disable: 6285 )	/* (<non-zero constant> || <non-zero constant>) is always a non-zero constant. Did you intend to use the bitwise-and operator? */
	#pragma warning( disable: 6287 )	/* redundant code: the left and right sub-expressions are identical */
	#pragma warning( disable: 6326 )	/* potential comparison of a constant with another constant */
	#pragma warning( disable: 28159 )	/* Consider using another function instead. */
#endif

#endif

#if (_MSC_VER < 1600)
	#define RTS_NO_STDINT_H
	#if defined(_WIN32_WCE) && (_WIN32_WCE < 0x0600)
		/* not so nice, for remote visu for CE 5, x86, built with VS 2005 */
		#define _INTPTR_T_DEFINED
	#endif
#endif

#define __STDC_FORMAT_MACROS

#if (_MSC_VER < 1400)
	#define RTS_VSNPRINTF								_vsnprintf
	#ifndef SIZE_MAX
		#define SIZE_MAX								ULONG_MAX
	#endif
#else
	#if defined(_WIN32_WCE)
		#define RTS_VSNPRINTF							_vsnprintf
	#else
		/* Needed for Visual Studio >= 2005
		 * #if !defined(_TRUNCATE)
		 *	 #define _TRUNCATE ((size_t)-1)
		 * #endif
		 * #define RTS_VSNPRINTF(s,len,format,arglist)		vsnprintf_s	(s,len,_TRUNCATE,format,arglist) 
		 * vsnprintf_s cannot be used in the runtime because it calls _invalid_parameter which calls exit (_invoke_watson)
		 * when an invalid format parameter is passed to the function. The function RTS_VSNPRINTF can be called from the IEC application f.e. LogAdd
		 * or IECStringUtils and the user did not expect a shutdown of the runtime when an invalid format parameter is passed (f.e. %5.1.f).
		*/
		#define RTS_VSNPRINTF(s,len,format,arglist)			vsnprintf(s,len,format,arglist)
	#endif

	#if defined(_WIN64)
		#define STDINT_H_UINTPTR_T_DEFINED
		#define TRG_64BIT
		/* Table-based exceptions handling on Win64 instead of the frame-based on Win32 */
		#define	TRG_TABLE_BASED_EXCEPTIONS_HANDLING
	#endif

	#ifndef _CRT_SECURE_NO_DEPRECATE 
		#define _CRT_SECURE_NO_DEPRECATE
	#endif

	#ifndef _CRT_SECURE_NO_WARNINGS
		#define _CRT_SECURE_NO_WARNINGS
	#endif
#endif

#ifndef SYS_DRIVER_NAME
	#define SYS_DRIVER_NAME			L"LZS"
#endif
#ifndef SYS_DRIVER_INSTANCE
	#define SYS_DRIVER_INSTANCE		 "LZS0:"
#endif

#ifndef SYS_DRIVER_DLL
	#define SYS_DRIVER_DLL L"SysDrv3SCE.dll"
#endif

/*lint -e553 */
#if (_WIN32_WCE >= 0x600)
	#ifndef ETH_DEV_CONTEXT
		#define ETH_DEV_CONTEXT 0x4545
	#endif
#endif

/*
 * This include file is required by CmpStd.h. 
 * It's intention is to contain system specific "#define"'s.
 *
 */

#include <targetdefines.h>

#if defined(SYSDEFINES_USE_PROFILE) && !defined(RC_INVOKED)
	#include <profile.h>
#endif /* defined(SYSDEFINES_USE_PROFILE) && !defined(RC_INVOKED) */

#define INTEL_BYTE_ORDER
#define CMPMGR_VALUE_INT_OVERLOADABLE_FUNCTIONS_DEFAULT		1

#if defined (_WIN32_WCE)
	#define SYSTARGET_OS_WINDOWS_CE
	#if (_MSC_VER <= 1500)
		/* old MS compiler with almost no C99 support, i.e. CE 6 and 7 */
		#define SYSTARGET_OS_WINDOWS_CE_WITHOUT_C99			1
	#endif
	#if (_MSC_VER < 1900)
		#define __func__ __FUNCTION__
	#endif
	#if defined(REMOTEVISU)
		#define PATHS_RELATIVE
		#define VISH_REMOTE_START_IN_COMM_CYCLE
		/* disable this feature at the moment*/
		#define NO_AUTOMATIC_COM_PORTS
	#endif
#else
	#if defined(_WINDOWS) || defined(WIN32)
		#define SYSTARGET_OS_WINDOWS
	#endif
    #if (SYSTARGET_DEVICE_TYPE != 4101 /* SYSTARGET_TYPE_HMI */)
		#define MY_WIBU_PRODUCTCODE			305021 
		#define MY_WIBU_PRODUCTCODE_NET		305022
		#define	MY_WIBU_PRODUCTCODE_MAPPING {{8785,8786}}
	#endif
#endif

#if defined(SYSTARGET_OS_WINDOWS)
	#include <winsock2.h>
	#include <windows.h>
	#ifdef WIN32_LEAN_AND_MEAN
		#include <WinPerf.h>
		#include <MMSystem.h>
		#include <ShellAPI.h>
		#include <CommDlg.h>
		#include <Rpc.h>
		#include <RpcNsi.h>
		#include <RpcNdr.h>
		#include <Wincrypt.h>
		#include <unknwn.h>
	#endif
#endif

#if defined(SYSTARGET_OS_WINDOWS_CE)
	#ifdef WIN32_LEAN_AND_MEAN
		#include <winsock2.h>
	#endif
	#include <windows.h>
#endif

#ifndef WIN32
	#define WIN32
#endif

#ifndef CDECL
	#define CDECL
#endif

#ifndef CDECL_EXT
	#define CDECL_EXT
#endif

#define RTS_OS_KEYPRESSED				_kbhit()

#define RTS_DEFINE_MODE_T
#define RTS_DEFINE_GID_T
#define RTS_DEFINE_UID_T
#define RTS_DEFINE_ID_T
#define RTS_DEFINE_PID_T
#define RTS_DEFINE_NLINK_T


#if defined(SYSTARGET_OS_WINDOWS)
	#define TRG_X86
	#define BTAG_MAX_NESTED_TAGS				100
	
	#define SYSINTERNALLIB_DISABLE_INT64_DIVBYZERO_CHECK
	#define RTS_STRUCTURED_EXCEPTION_HANDLING

	#define MAX_COM_PORTS	16	/* Allow 16 com ports to be used (Overrides default 3 out of CmpBlkDrvCom.h) */

	#define CMPIECTASK_STACK_SIZE				0x800000	/* Set the reserved stack size for an IEC task to 8MB! This is the maximum stack size a thread can use! */
	#define CMPIECTASK_STACK_SIZE_ADDITIONAL	0			/* 0MB: no additional stack size */

	#define SCHEDULEVALUE_INT_SUPERVISOR_TIMEOUT_MULTIPLIER_DEFAULT		INT32_C(10)

	#define MAX_NUM_ADAPTERS 100	/* Allow up to 100 ethernet adapters (overrides the default value from SysEthernetItf.h) */
#elif defined (SYSTARGET_OS_WINDOWS_CE)

	#ifndef RTS_NO_STDINT_H
		#define RTS_NO_STDINT_H
	#endif

	#if defined(_X86_) || defined(_i386_)
		#define TRG_X86
		#ifndef SYSTARGET_SIGNATUREID
			#if (_WIN32_WCE >= 0x600)
				#if defined(_HMI_)
					#define SYSTARGET_SIGNATUREID		0xDAA11CFE
				#elif defined(SYSTARGET_COMMUNICATION)
					/* CE x86 Gateway */
					#define SYSTARGET_SIGNATUREID	0x6CF50B48
					#define
				#else
					#define SYSTARGET_SIGNATUREID		0xCAD10A37
				#endif
			#else
				#define SYSTARGET_SIGNATUREID		0x7371FE50
			#endif
		#endif

		/* CE X86 (4.2 /EVC, 5.0 /EVC, 6.0 /VS2005, 7.0 / VS2008) => we don't have __seh_longjmp_unwind */
		#define SYSEXCEPTWIN32_SPECIFIC_TRY_CATCH
		#define RTS_STRUCTURED_EXCEPTION_HANDLING
	#endif

	#ifdef _ARM_
		#if (_WIN32_WCE < 0x800)
			#define TRG_ARM		2
			#ifndef SYSTARGET_SIGNATUREID
				#if (_WIN32_WCE >= 0x600)
					#if defined(_HMI_)
						#define SYSTARGET_SIGNATUREID		0x23AD98F8
					#elif defined(SYSTARGET_COMMUNICATION)
						/* CE ARM Gateway */
						#define SYSTARGET_SIGNATUREID	0x144BDFEE
					#else
						#define SYSTARGET_SIGNATUREID		0xB26FDE91
					#endif
				#else
					#define SYSTARGET_SIGNATUREID		0x7C4FDF8F
				#endif
			#endif
		#else
			#define TRG_CORTEX		2
			#ifndef SYSTARGET_SIGNATUREID
				#if defined(_HMI_)
					#define SYSTARGET_SIGNATUREID	0xA3394022
				#elif defined(SYSTARGET_COMMUNICATION)
					/* CE Cortex Gateway */
					#define SYSTARGET_SIGNATUREID	0x156D5794
				#else
					#define SYSTARGET_SIGNATUREID	0xC4C9A392
				#endif
			#endif
		#endif
	#endif

	#ifdef _MIPS_
		#define TRG_MIPS	3
		#ifndef SYSTARGET_SIGNATUREID
			#define SYSTARGET_SIGNATUREID		0xB187ACF2
		#endif
	#endif

	#ifdef _SH3_
		#define TRG_SH		5
		#define TRG_SH3
		#ifndef SYSTARGET_SIGNATUREID
			#define SYSTARGET_SIGNATUREID		0x10C42D18
		#endif
	#endif
	#ifdef _SH4_
		#define TRG_SH		5
		#define TRG_SH4
		#ifndef SYSTARGET_SIGNATUREID
			#define SYSTARGET_SIGNATUREID		0x1F8F54F6
		#endif
	#endif
	#define BTAG_MAX_NESTED_TAGS	50
#endif

typedef signed   __int64 RTS_I64;
typedef unsigned __int64 RTS_UI64;
#define RTS_UI64_MAX				(RTS_UI64)(~((RTS_UI64)0))
#define RTS_I64_MAX					(RTS_I64)(~((RTS_I64)0))
#define BASE64BITTYPES_DEFINED

#define PCI_MAX_BUSSES		255
#define PCI_MAX_DEVICES		32

/*
 * Defines the available communication buffers in the
 * communication server.
 * 
 * BUFFERSIZE is the overall amount of memory to be used by
 *  all channels
 * MAXCHANNELS defines the number of channels that may be handled 
 *  concurrently. 
 *
 * Since each channel needs separate send and receive channels
 * the available communication buffer is BUFFERSIZE / (2*MAXCHANNELS)
 */
#ifndef NETSERVER_BUFFERSIZE
	#define NETSERVER_BUFFERSIZE			800000
#endif
#ifndef NETSERVER_MAXCHANNELS
	#define NETSERVER_MAXCHANNELS			4
#endif

#if defined (SYSTARGET_OS_WINDOWS_CE)
	#define stricmp		_stricmp	
#else
	#ifndef MAX_USB_DEVICES
		#define MAX_USB_DEVICES	5 /*Maximum number of devices per driver.*/
	#endif
#endif
#define NUM_OF_STATIC_IEC_EVENTS		10

#define HUGEPTR
#define RTS_UNICODE

#define RTS_I64_CONST(a)	(a##i64)
#define RTS_UI64_CONST(a)	(a##ui64)

#ifdef _DEBUG
	#define RTS_DEBUG
	#if !defined (_WIN32_WCE)
		#include <assert.h>
		#define RTS_ASSERT	assert
	#endif
#endif

#if defined (SYSTARGET_OS_WINDOWS_CE)
	#define PATHS_RELATIVE
	#define _INC_WINSOCK2

	#if (_WIN32_WCE >= 0x600)
		#define CMPAUDITLOG_NOTIMPLEMENTED
	#else
		#define CMPAUDITLOG_NOTIMPLEMENTED
		#define CMPSESSIONINFORMATION_NOTIMPLEMENTED
	#endif

#endif

#if defined(_WIN32_WCE) && defined(STANDALONE_GATEWAY)
	#define CMPEDGEGATEWAY_NOTIMPLEMENTED
	#define CMPNAMESERVICESERVER_NOTIMPLEMENTED
	#define CMPUSERMGR_NOTIMPLEMENTED
	#define CMPAPP_NOTIMPLEMENTED
	#define CMPSCHEDULE_NOTIMPLEMENTED
	#define CMPAPPBP_NOTIMPLEMENTED
	#define SYSTIMER_NOTIMPLEMENTED
	#define CMPSCHEDULE_NOTIMPLEMENTED
	#define CMPMONITOR_NOTIMPLEMENTED
	#define CMPMONITOR2_NOTIMPLEMENTED
	#define SYSDIR_NOTIMPLEMENTED
	#define CMPTLS_NOTIMPLEMENTED
	#define CMPSECURITYMANAGER_NOTIMPLEMENTED
	#define CMPSECURECHANNEL_NOTIMPLEMENTED
	#define CMPSESSIONINFORMATION_NOTIMPLEMENTED
	#define CMPSUPERVISOR_NOTIMPLEMENTED
	#define CMPPLCSHELL_NOTIMPLEMENTED
	#define CMPIECTASK_NOTIMPLEMENTED
	#define CMPCRYPTO_NOTIMPLEMENTED
	#define SYSCPUMULTICORE_NOTIMPLEMENTED
	#define SYSREADWRITELOCK_NOTIMPLEMENTED
	#define CMPDEVICEMANAGEMENT_NOTIMPLEMENTED
#endif
#ifdef MIXED_LINK
	#define SYSTARGET_EXTERNAL
	#define CMPUSERDB_EXTERNAL
#endif

#ifndef SYSTARGET_VENDOR_ID
	#define SYSTARGET_VENDOR_ID				RTS_VENDORID_3S
#endif

#ifndef SYSTARGET_DEVICE_ID
	#ifdef TRG_X86
		#if (_WIN32_WCE >= 0x600)
			#if defined(_HMI_)
				#define SYSTARGET_DEVICE_ID		0x8018
			#elif defined(SYSTARGET_COMMUNICATION)
				#define SYSTARGET_DEVICE_ID		0x0011
			#else
				#define SYSTARGET_DEVICE_ID		0x8016
			#endif
		#elif !defined(SYSTARGET_OS_WINDOWS)
			#if defined(SYSTARGET_COMMUNICATION)
				#define SYSTARGET_DEVICE_ID		0x0011
			#else
				#define SYSTARGET_DEVICE_ID		0x800C
			#endif
		#endif
	#elif TRG_CORTEX
		#if defined(_HMI_)
			#define SYSTARGET_DEVICE_ID		0x801A
		#elif defined(SYSTARGET_COMMUNICATION)
			#define SYSTARGET_DEVICE_ID		0x0013
		#else
			#define SYSTARGET_DEVICE_ID		0x8019
		#endif
	#elif TRG_ARM
		#if (_WIN32_WCE >= 0x600)
			#if defined(_HMI_)
				#define SYSTARGET_DEVICE_ID			0x8017
			#elif defined(SYSTARGET_COMMUNICATION)
				#define SYSTARGET_DEVICE_ID		0x0012
			#else
				#define SYSTARGET_DEVICE_ID			0x8015
			#endif
		#else
			#define SYSTARGET_DEVICE_ID			0x8002
		#endif
	#elif TRG_MIPS
		#define SYSTARGET_DEVICE_ID			0x8003
	#elif TRG_SH
		#ifdef TRG_SH3
			#define SYSTARGET_DEVICE_ID		0x8005
		#endif
		#ifdef TRG_SH4
			#define SYSTARGET_DEVICE_ID		0x8006
		#endif
	#else
		#define SYSTARGET_DEVICE_ID			0
	#endif
#endif


#ifndef SYSTARGET_DEVICE_VERSION
	#define SYSTARGET_DEVICE_VERSION		RTS_VERSION
#endif

#ifndef SYSTARGET_NODE_NAME_WINCE
	#if defined(_WIN32_WCE)
		#if defined (_HMI_)
			#define SYSTARGET_NODE_NAME_WINCE		RTS_PRODUCT_FAMILY_NAME" HMI WinCE V3"
		#else
			#define	SYSTARGET_NODE_NAME_WINCE		RTS_PRODUCT_NAME" WinCE V3"
		#endif
	#endif
#endif

#ifndef SYSTARGET_DEVICE_NAME
	#if defined(_WIN32_WCE)
		#if defined(_HMI_)
			#define SYSTARGET_DEVICE_NAME		RTS_PRODUCT_FAMILY_NAME" HMI WinCE V3"
		#else
			#define SYSTARGET_DEVICE_NAME		RTS_PRODUCT_NAME" WinCE V3"
		#endif
	#endif
#endif

#ifndef SYSTARGET_VENDOR_NAME
	#define	SYSTARGET_VENDOR_NAME			"3S - Smart Software Solutions GmbH"
#endif

#define CMPCODEMETER_EXTERNAL

#if !defined(_WIN32_WCE) && !defined(SYSDEFINES_USE_PROFILE)
	#define SYSCPUMULTICORE_EXTERNAL
	#define SYSREADWRITELOCK_EXTERNAL
#endif


#ifndef RTS_SIL2
	#ifndef CMPSIL2_NOTIMPLEMENTED
		#define CMPSIL2_NOTIMPLEMENTED
	#endif
#endif

/* Table-based exceptions handling requires POU tables */
#ifndef APP_POU_TABLES_ENABLED
#	ifdef TRG_TABLE_BASED_EXCEPTIONS_HANDLING
#		define APP_POU_TABLES_ENABLED
#	endif
#endif

#ifndef CM_OBJECT_MANAGER_ENABLED
#	ifdef TRG_64BIT
#		define CM_OBJECT_MANAGER_ENABLED
#	endif
#endif

#ifndef WIN32_RESOURCES
#if defined(CPLUSPLUS) && (SYSTARGET_DEVICE_ID == 0x0004 || SYSTARGET_DEVICE_ID == 0x0001) && !defined(CODESYSCONTROLSTATICCPP_EMBEDDED)
/* Only for detecting memory leaks in VisualStudio */
	#define _CRTDBG_MAP_ALLOC
	#include <stdlib.h>
	#include <crtdbg.h>
/*
	#if !defined(DBG_NEW) && defined(CPLUSPLUS)
		#define DBG_NEW new ( _NORMAL_BLOCK , __FILE__ , __LINE__ )
		#define new DBG_NEW
    #endif
*/
#endif
#endif

#define SYSWIN32_ENABLE_WMI_SUPPORT

#if !defined(RTS_SSIZE_T_DEFINED)
	#ifdef RTS_NO_STDINT_H
		#include "pstdint.h"
	#else
		#include <stdint.h>
	#endif
	#define RTS_SSIZE_T_DEFINED
	typedef intptr_t ssize_t;
#endif


#endif /* __SYSDEFINES__H__ */
