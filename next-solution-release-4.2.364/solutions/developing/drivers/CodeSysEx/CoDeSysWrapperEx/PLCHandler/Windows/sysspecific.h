#ifndef __SYSSPECIFIC__H__
#define __SYSSPECIFIC__H__

/* 
 * ---WIN32 specific---
 * 
 * This file is included by CmpStd.h.
 * It may contain system specific include files etc. 
 * 
 */

#include <float.h>
#ifdef CODESYSCONTROL_LEAKCHECK
	/*========================================================================
	 * Install "Visual Studio Enhanced Leak Detector" first:
	 *		https ://kinddragon.github.io/vld/
	 *		https ://github.com/KindDragon/vld/wiki/Using-Visual-Leak-Detector
	 *
	 * NOTE:
	 * We expect it in the path "C:\Program Files (x86)\Visual Leak Detector"
	 *========================================================================*/
	#include "vld.h"
#endif


#define X86_FPU_INIT_CW 0x027b

#if defined (_WIN32_WCE)
	/*Some Makros do not exist under Win CE*/
	#ifndef offsetof 
		#define offsetof(s,m)   (size_t)&(((s *)0)->m)
	#endif
	
	#define max(a,b)    (((a) > (b)) ? (a) : (b))
	#define min(a,b)    (((a) < (b)) ? (a) : (b))

#if defined(NO_NATIVE_MEMCPY)
		#include "CMUtilsItf.h"
		#define memcpy(a, b, c) CAL_CMUtlMemCpy(a, b, c)
	#endif
#else
	#include <conio.h>
#endif

#if !defined (_WIN32_WCE)
	#define itoa	_itoa
	#define stricmp	_stricmp

	 /* Must be higher than 260 (see Windows define MAX_PATH)! */
	#define MAX_PATH_LEN					512
	#define RTS_MAX_PATH_LEN				MAX_PATH_LEN
#endif



#define SYS_ATOMIC_COMPARE_AND_SWAP(piValue, iExchangeValue, iCompareValue, Result) \
	do { \
		if (InterlockedCompareExchange((volatile LONG *)piValue, iExchangeValue, iCompareValue) == iCompareValue) \
			Result = ERR_OK; \
		else \
			Result = ERR_FAILED; \
	} while (0)

#if !defined (_WIN32_WCE)
	#if defined(_WIN64)
		#define SYS_ATOMIC_COMPARE_AND_SWAP64(pi64Value, i64ExchangeValue, i64CompareValue, Result) \
				do { \
					if (InterlockedCompareExchange64(pi64Value, i64ExchangeValue, i64CompareValue) == i64CompareValue) \
						Result = ERR_OK; \
					else \
						Result = ERR_FAILED; \
				} while (0)
	#else
		#define SYS_ATOMIC_COMPARE_AND_SWAP64(pi64Value, i64ExchangeValue, i64CompareValue, Result) \
			do\
			{\
				typedef __int64(WINAPI *PF_INTERLOCKEDCOMPAREEXCHANGE64)(__int64 volatile * _Destination, __int64 _Exchange, __int64 _Comparand);\
				static PF_INTERLOCKEDCOMPAREEXCHANGE64 s_pfInterlockedCompareExchange64 = NULL;\
				if (s_pfInterlockedCompareExchange64 == NULL)\
				{\
					HMODULE hModule = GetModuleHandle(TEXT("kernel32.dll"));\
					s_pfInterlockedCompareExchange64 = (PF_INTERLOCKEDCOMPAREEXCHANGE64)GetProcAddress(hModule, "InterlockedCompareExchange64");\
				}\
				if (s_pfInterlockedCompareExchange64 == NULL)\
					Result = ERR_NOT_SUPPORTED;\
				else\
				{\
					if (s_pfInterlockedCompareExchange64(pi64Value, i64ExchangeValue, i64CompareValue) == i64CompareValue) \
						Result = ERR_OK; \
					else \
						Result = ERR_FAILED; \
				}\
			} while(0)
	#endif /* defined(_WIN64) */
#else /*_WIN32_WCE */
	#if defined(TRG_X86) && (_WIN32_WCE >= 0x0700)
		#define SYS_ATOMIC_COMPARE_AND_SWAP64(pi64Value, i64ExchangeValue, i64CompareValue, Result) \
				do { \
					if (_InterlockedCompareExchange64(pi64Value, i64ExchangeValue, i64CompareValue) == i64CompareValue) \
						Result = ERR_OK; \
					else \
						Result = ERR_FAILED; \
				} while (0)
	#elif defined(TRG_CORTEX) /* CE 8 */
		/* this does not work, even with everything 8 byte aligned. 
		The *pi64Value is modified incorrectly at least in some cases
		#define SYS_ATOMIC_COMPARE_AND_SWAP64(pi64Value, i64ExchangeValue, i64CompareValue, Result) \
				do { \
					if (InterlockedCompareExchange64(pi64Value, i64ExchangeValue, i64CompareValue) == i64CompareValue) \
						Result = ERR_OK; \
					else \
						Result = ERR_FAILED; \
				} while (0)
		*/
		#define SYS_ATOMIC_COMPARE_AND_SWAP64_NOT_SUPPORTED
		#define READWRITE64_NOT_SUPPORTED
	#else 
		/* ARM CE 7, other CPUs like SH and Mips are no longer supported by CE */
		/* no 64 bit atomic operation => no define !! so the caller performs a non-atomic read/write*/
		#define SYS_ATOMIC_COMPARE_AND_SWAP64_NOT_SUPPORTED
		#define READWRITE64_NOT_SUPPORTED
	#endif
#endif /* !defined (_WIN32_WCE) */

#define RTS_CPU_SET_STACKPOINTER(pStack)				_asm mov esp, pStack

#define CCO_DEFAULT				CCO_TASK_GAP_SEMAPHORE

#if defined(TRG_ARM) || defined(TRG_CORTEX)
	#define RTS_STRUCTURED_EXCEPTION_HANDLING
	extern RTS_UINTPTR CDECL SysCpuGetRegisterSaveArea(RTS_UINTPTR sa);
	#define SYSCPU_EXTLIBCALL_GET_REGISTER_SAVE_AREA(sa)         SysCpuGetRegisterSaveArea(sa)
#endif

#ifndef WIN32_RESOURCES
#ifdef RTS_STRUCTURED_EXCEPTION_HANDLING
	#define __MapException(ulOSException, pulException)\
	{\
		switch ((RTS_UI32)ulOSException)\
		{\
			case /*lint -e650 */(DWORD)EXCEPTION_ACCESS_VIOLATION:\
				*(RTS_UI32 *)pulException = 0x00000051;\
				break;\
			case EXCEPTION_DATATYPE_MISALIGNMENT:\
				*(RTS_UI32 *)pulException = 0x00000100;\
				break;\
			case EXCEPTION_ARRAY_BOUNDS_EXCEEDED:\
				*(RTS_UI32 *)pulException = 0x00000101;\
				break;\
			case EXCEPTION_FLT_DENORMAL_OPERAND:\
				*(RTS_UI32 *)pulException = 0x00000151;\
				break;\
			case EXCEPTION_FLT_DIVIDE_BY_ZERO:\
				*(RTS_UI32 *)pulException = 0x00000152;\
				break;\
			case EXCEPTION_FLT_INEXACT_RESULT:\
				*(RTS_UI32 *)pulException = 0x00000153;\
				break;\
			case EXCEPTION_FLT_INVALID_OPERATION:\
				*(RTS_UI32 *)pulException = 0x00000154;\
				break;\
			case EXCEPTION_FLT_OVERFLOW:\
				*(RTS_UI32 *)pulException = 0x00000155;\
				break;\
			case EXCEPTION_FLT_STACK_CHECK:\
				*(RTS_UI32 *)pulException = 0x00000156;\
				break;\
			case EXCEPTION_FLT_UNDERFLOW:\
				*(RTS_UI32 *)pulException = 0x00000157;\
				break;\
			case EXCEPTION_INT_DIVIDE_BY_ZERO:\
				*(RTS_UI32 *)pulException = 0x00000102;\
				break;\
			case EXCEPTION_INT_OVERFLOW:\
				*(RTS_UI32 *)pulException = 0x00000103;\
				break;\
			case EXCEPTION_PRIV_INSTRUCTION:\
				*(RTS_UI32 *)pulException = 0x00000052;\
				break;\
			case EXCEPTION_NONCONTINUABLE_EXCEPTION:\
				*(RTS_UI32 *)pulException = 0x00000104;\
				break;\
			default:\
				*(RTS_UI32 *)pulException = ulOSException;\
				break;\
		}\
	}

	typedef struct __tagEXPTRSTRUCT
	{
		EXCEPTION_RECORD *pRecord;
		CONTEXT          *pContext;
	}__EXPTRSTRUCT;

	typedef struct __RegContext
	{
		RTS_UINTPTR IP;
		RTS_UINTPTR BP;
		RTS_UINTPTR SP;
	} __RegContext;

	static unsigned long __Win32ExceptionHandler(__EXPTRSTRUCT *exptr, __RegContext *pExceptionContext, RTS_UI32 ulOSException, RTS_UI32 *pExceptionCode)
	{
		__MapException(ulOSException, pExceptionCode);
		#if defined (TRG_ARM) || defined(TRG_CORTEX)
				pExceptionContext->IP = exptr->pContext->Pc;
				pExceptionContext->BP = exptr->pContext->R10;	/*Frame Pointer*/
				pExceptionContext->SP = exptr->pContext->Sp;
		#elif defined (TRG_MIPS)
				pExceptionContext->IP = exptr->pContext->Fir;
				pExceptionContext->BP = exptr->pContext->IntS8;
				pExceptionContext->SP = exptr->pContext->IntSp;
		#elif defined (TRG_SH)
				pExceptionContext->IP = exptr->pContext->Fir;
				pExceptionContext->BP = exptr->pContext->R14;
				pExceptionContext->SP = exptr->pContext->R15;
		#else/*TRG_X86*/
		#	ifdef _WIN64
				pExceptionContext->IP = exptr->pContext->Rip;
				pExceptionContext->BP = exptr->pContext->Rbp;
				pExceptionContext->SP = exptr->pContext->Rsp;
		#	else
				{
					unsigned short us = X86_FPU_INIT_CW;
					_clearfp();
					_asm finit
					_asm fwait
					_asm fldcw us
					_asm fwait
				}
				pExceptionContext->IP = exptr->pContext->Eip;
				pExceptionContext->BP = exptr->pContext->Ebp;
				pExceptionContext->SP = exptr->pContext->Esp;
		#	endif
		#endif
		return EXCEPTION_EXECUTE_HANDLER;
	}


												

#if defined(SYSEXCEPTWIN32_SPECIFIC_TRY_CATCH)
	#define EXCPT_GET_CODE() 	__SEHContext.ui32ExceptionCode
	#define EXCPT_GET_CONTEXT() ((RegContext*)&__SEHContext.context)

	#define rts_try \
	{ \
		SEHContext __SEHContext = {0}; \
		for (;;) \
		{ \
			__try \
			{ \
				if (__SEHContext.ui32ExceptionCode == RTSEXCPT_NOEXCEPTION) \


	#define rts_try_end \
			} \
			__except(__Win32ExceptionHandler((__EXPTRSTRUCT *)_exception_info(), (__RegContext *)(&__SEHContext.context), GetExceptionCode(), &(__SEHContext.ui32ExceptionCode))) \
			{ \
				continue; \
			} \
			break; \
		} \
	}

#endif
#endif	/*RTS_STRUCTURED_EXCEPTION_HANDLING*/
#endif	/*WIN32_RESOURCES*/


#endif	/*__SYSSPECIFIC__H__*/
