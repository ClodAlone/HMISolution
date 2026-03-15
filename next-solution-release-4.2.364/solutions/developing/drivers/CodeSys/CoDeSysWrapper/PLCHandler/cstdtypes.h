/**
 * <name>cstdtypes.h</name>
 * <description> 
 *	<p>Compiler specific helper to include the portable integer types header</p>
 * </description>
 *
 * <copyright>(c) 2003-2011 3S-Smart Software Solutions</copyright>
 */
#ifndef ___CSTDTYPES_H___
#define ___CSTDTYPES_H___

/**
 * set default register width
 */
#ifndef RTS_REGISTER_WIDTH
    #if defined(TRG_C16X)
        #define RTS_REGISTER_WIDTH 16
    #elif defined(TRG_64BIT) || defined(TRG_IA64)
        #define RTS_REGISTER_WIDTH 64
    #else
        #define RTS_REGISTER_WIDTH 32
    #endif
#endif

/**
 * Include header file, which defines the standard c datatypes
 */
#ifdef RTS_NO_STDINT_H
	/* Include widely compatible type definitions for many compilers, from BSD */
	#include "pstdint.h"
#else
	/* Include standard header from the C compiler.
	 * Note: If your compiler toolchain doesn't provide ANSI-C99 file called
	 *       stdint.h (as some old compilers or highly embedded toolchains
	 *       are doing), you may try set the define RTS_NO_STDINT_H.
	 *       This will include "pstdint.h" instead, which works already in
	 *       for most compilers.
	 *       If it doesn't work, please contact the 3S support.
	 */
	#include <stdint.h>

#endif		/*ifdef RTS_NO_STDINT_H*/

#include "pinttypes.h"

/**
 * Redefine UINT32_C(), INT16_C, ...,
 * as many compilers are defining this in a bad way. They
 * are using type-casts to get the correct data-width. This
 * makes it impossible to use for some pedantic C-Compilers.
 */
#ifdef INT64_C
	#undef INT64_C
#endif
#ifdef UINT64_C
	#undef UINT64_C
#endif
#ifdef INT32_C
	#undef INT32_C
#endif
#ifdef UINT32_C
	#undef UINT32_C
#endif
#ifdef INT16_C
	#undef INT16_C
#endif
#ifdef UINT16_C
	#undef UINT16_C
#endif
#ifdef INT8_C
	#undef INT8_C
#endif
#ifdef UINT8_C
	#undef UINT8_C
#endif

#define INT64_C(x)		(RTS_I64_CONST(x))
#define UINT64_C(x)		(RTS_UI64_CONST(x))
#define INT32_C(x)		(x ## L)
#define UINT32_C(x)		(x ## UL) 
#define INT16_C(x)		(x)
#define UINT16_C(x)		(x ## U)
#define INT8_C(x)		(x)
#define UINT8_C(x)		(x ## U)

#define RTS_IEC_BOOL_C(x)	(x)
#define RTS_IEC_BYTE_C(x)	(UINT8_C(x))
#define RTS_IEC_SINT_C(x)	(INT8_C(x))
#define RTS_IEC_USINT_C(x)	(UINT8_C(x))
#define RTS_IEC_INT_C(x)	(INT16_C(x))
#define RTS_IEC_UINT_C(x)	(UINT16_C(x))
#define RTS_IEC_DINT_C(x)	(INT32_C(x))
#define RTS_IEC_UDINT_C(x)	(UINT32_C(x))
#define RTS_IEC_LINT_C(x)	(INT64_C(x))
#define RTS_IEC_ULINT_C(x)	(UINT64_C(x))
#define RTS_IEC_WORD_C(x)	(UINT16_C(x))
#define RTS_IEC_DWORD_C(x)	(UINT32_C(x))
#define RTS_IEC_LWORD_C(x)	(UINT64_C(x))
#define RTS_IEC_REAL_C(x)	(x)
#define RTS_IEC_LREAL_C(x)	(x)
#ifdef TRG_64BIT
#define RTS_IEC_XINT_C(x)		(INT64_C(x))
#define RTS_IEC_UXINT_C(x)		(UINT64_C(x))
#define RTS_IEC_XWORD_C(x)		(UINT64_C(x))
#else
#define RTS_IEC_XINT_C(x)		(INT32_C(x))
#define RTS_IEC_UXINT_C(x)		(UINT32_C(x))
#define RTS_IEC_XWORD_C(x)		(UINT32_C(x))
#endif

/**
 * Define some more exotic types, which are missing
 * in many versions of stdint.h.
 */
#if !defined(STDINT_H_UINTPTR_T_DEFINED)
/* Note: If your compiler complains about a "redefinition"
 *       or s.th. similar here, you may set the defines
 *       _UINTPTR_T, _PTRDIFF_T, ... in your sysdefines.h.
 */
	#ifndef  _UINTPTR_T
		typedef uint32_t uintptr_t;
		#define _UINTPTR_T
	#endif
	#ifndef  _PTRDIFF_T
		typedef int32_t ptrdiff_t;
		#define _PTRDIFF_T
	#endif
	#ifndef  _INTPTR_T
		typedef ptrdiff_t intptr_t;
		#define _INTPTR_T
	#endif
	#define STDINT_H_UINTPTR_T_DEFINED
#endif	/*!defined(STDINT_H_UINTPTR_T_DEFINED)*/

/* Format modifiers for integral types */
#ifndef ULONG_MAX
#	include <limits.h>
#	ifndef ULONG_MAX
#		error ULONG_MAX does not exist in limits.h!
#	endif
#endif

/*
 * Some basic definitions 
 */

#ifndef NULL
#	define NULL 0L
#endif

#ifndef TRUE
#	define TRUE 1
#endif

#ifndef FALSE
#	define FALSE 0
#endif

/*lint -e666 -e506 */
#ifndef MIN
#	define MIN(x,y)	((x)<(y)?(x):(y))
#endif

#ifndef MAX
#	define MAX(x,y)	((x)>(y)?(x):(y))
#endif

#ifndef RTS_SETRESULT
#	define RTS_SETRESULT(p,r)		do { if(p) *(p)=r; } while(0)
#endif

#ifndef RTS_GETRESULT
#	define RTS_GETRESULT(Return,Result)		(Result == ERR_OK ? (Return) : ((Return) ? Result : Result))
#endif

#ifndef LOWWORD
#	define LOWWORD(p)		((uint16_t)((uint32_t)(p) & 0x0000FFFF))
#endif

#ifndef HIGHWORD
#	define HIGHWORD(p)		((uint16_t)(((uint32_t)(p)>>16) & 0x0000FFFF))
#endif


/* Operations on flags */
#ifndef RTS_SETFLAGS
	#define RTS_SETFLAGS(value, flags)			(value |= (flags))
#endif
#ifndef RTS_SETFLAG
	#define RTS_SETFLAG(value, flag)			RTS_SETFLAGS(value, flag)
#endif

#ifndef RTS_RESETFLAGS
	#define RTS_RESETFLAGS(value, flags)		(value &= ~(flags))
#endif
#ifndef RTS_RESETFLAG
	#define RTS_RESETFLAG(value, flag)			RTS_RESETFLAGS(value, flag)
#endif

#ifndef RTS_HASFLAGS
	#define RTS_HASFLAGS(value, flags)			((value & (flags)) == (flags))
#endif
#ifndef RTS_HASFLAG
	#define RTS_HASFLAG(value, flag)			(value & (flag))
#endif



#ifndef ALIGN_MEMORY
#	define ALIGN_MEMORY(pMemory)	((void *)(((((uintptr_t)(pMemory) + sizeof(void*) - 1) / sizeof(void*)) * sizeof(void*))))
#endif

#ifndef ALIGN_MEMORY_SIZE
#	define ALIGN_MEMORY_SIZE(pMemory, iSize)	((iSize) - (((((uintptr_t)(pMemory) + sizeof(void*) - 1) / sizeof(void*)) * sizeof(void*)) - (uintptr_t)(pMemory)))
#endif

#ifndef ALIGN_MEMORY2
#	define ALIGN_MEMORY2(pMemory, Modulo)	((void *)((((uintptr_t)(pMemory) + (Modulo) - 1) / (uintptr_t)(Modulo)) * (uintptr_t)(Modulo)))
#endif

#ifndef ALIGN_MEMORY2_BACK
#	define ALIGN_MEMORY2_BACK(pMemory, Modulo)	((void *)(((uintptr_t)(pMemory) / (uintptr_t)(Modulo)) * (uintptr_t)(Modulo)))
#endif

#ifndef ALIGN_MEMORY_SIZE2
#	define ALIGN_MEMORY_SIZE2(pMemory, iSize, Modulo)	((iSize) - (((((uintptr_t)(pMemory) + (Modulo) - 1) / sizeof(void*)) * (Modulo)) - (uintptr_t)(pMemory)))
#endif

#ifndef ALIGN_SIZE
/* NOTE: it seems like this operation can be made without a comparison:									*/
/* #define ALIGN_SIZE(Size, Modulo) (( (Size) + ((Modulo) - 1) ) / (Modulo)) * (Modulo)	*/
#	define ALIGN_SIZE(Size, Modulo)		/*lint -e{506} */(((Size) % (Modulo)) ? ((Size) + ((Modulo) - ((Size) % (Modulo)))) : (Size))
#endif

#ifndef RTS_VSNPRINTF
	#if defined(RTS_VSNPRINTF_NOT_AVAILABLE)
		#define RTS_VSNPRINTF(s,len,format,arglist)	vsprintf(s,format,arglist)
	#else
		#define RTS_VSNPRINTF(s,len,format,arglist) vsnprintf(s,len,format,arglist)
	#endif
#endif

#define CONCAT(x,y) x##y

/* Some preprocessors do not allow to place such a macro within other macros	*/
/* and/or pragmas.																															*/
#define ____QUOTE____(x) #x
#define QUOTE(x) ____QUOTE____(x)

#ifdef INTEL_BYTE_ORDER
#	define SWAP16u(x) (x)
#	define SWAP16s(x) (x)
#	define SWAP32u(x) (x)
#	define SWAP32s(x) (x)
#elif defined(MOTOROLA_BYTE_ORDER)
#	define SWAP16u(x) ( (uint16_t)((((x) & 0xFF)<<8) | (((x) & 0xFF00) >> 8)) )
#	define SWAP16s(x) ( (int16_t)((((x) & 0xFF)<<8) | (((x) & 0xFF00) >> 8)) )
#	define SWAP32u(x) ( (uint32_t)((((x) & 0xff000000) >> 24) | (((x) & 0x00ff0000) >> 8) | (((x) & 0x0000ff00) << 8) | (((x) & 0x000000ff) << 24)) )
#	define SWAP32s(x) ((int32_t)((((uint32_t)(x)) & 0xff000000) >> 24) | (((x) & 0x00ff0000) >> 8) | (((x) & 0x0000ff00) << 8) | (((x) & 0x000000ff) << 24))
#else
	#error "No byte order defined for the current target"
#endif



#endif

