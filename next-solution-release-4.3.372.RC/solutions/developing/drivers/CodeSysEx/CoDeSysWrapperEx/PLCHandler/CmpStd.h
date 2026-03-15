/**
 * <name>CmpStd.h</name>
 * <description> 
 *	<p>This is the standard header file of the runtime system, that must be included as the first file in every component!</p>
 *	<p>sysdefines.h ist included here as the first file. The platform specific defines must be set here.</p>
 *	<ul>
 *		<li>STATIC_LINK, MIXED_LINK, DYNAMIC_LINK(default) and CPLUSPLUS.</li>
 *		<li>RTS_LITTLEENDIAN_BYTEORDER or RTS_BIGENDIAN_BYTEORDER / RTS_BIGENDIAN_BITORDER
 *			Backward compatibility: INTEL_BYTE_ORDER or MOTOROLA_BYTE_ORDER  			
 *		</li>
 *		<li>RTS_SIXTEENBITBYTES for a DSP platform (e.g. Texas Instruments TI28x)</li>
 *		<li>BASETYPES_DEFINED to overload all standard types</li>
 *		<li>IECBASETYPES_DEFINED to overload all IEC standard types</li>
 *	</ul>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */

#ifndef __RTSSTD_H__
#define __RTSSTD_H__

#include "sysdefines.h"

#ifndef RC_INVOKED

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <stdarg.h>
#include <stddef.h>
#include <ctype.h>
#include <math.h>

/**
 * <category>Compiler switch</category>
 * <description>
 *	Compiler switches to specify the byteorder of the runtime.
 * </description>
 * <element name="RTS_LITTLEENDIAN_BYTEORDER">Switch to specify the little endian byteorder</element>
 * <element name="RTS_BIGENDIAN_BYTEORDER">Switch to specify the big endian byteorder</element>
 */
#if defined(MOTOROLA_BYTE_ORDER) && !defined(RTS_BIGENDIAN_BYTEORDER)
	#define RTS_BIGENDIAN_BYTEORDER
#endif
#if defined(RTS_BIGENDIAN_BYTEORDER) && !defined(MOTOROLA_BYTE_ORDER)
	#define MOTOROLA_BYTE_ORDER	
#endif
#if !defined(RTS_BIGENDIAN_BYTEORDER)
	#if defined(INTEL_BYTE_ORDER) && !defined(RTS_LITTLEENDIAN_BYTEORDER)
		#define RTS_LITTLEENDIAN_BYTEORDER
	#endif
	#if defined(RTS_LITTLEENDIAN_BYTEORDER) && !defined(INTEL_BYTE_ORDER)
		#define INTEL_BYTE_ORDER
	#endif	
#endif

/*
 * Portable integer and memsize-types are included here.
 * Some operations with integers and pointers (LOWWORD, etc.) have also been
 * moved to cstdtypes.h																														
 */
#include "cstdtypes.h"
#include "CmpErrors.h"
#include "LogInfoIds.h"

#ifdef __cplusplus
	#define EXTERNC		extern "C"
	#define VIRTUAL		virtual
#else
	#define EXTERNC
#endif

typedef void*	OBJREF;

#endif /* RC_INVOKED */

/*
 * The RTS_VERSION below is to be changed only by 3S - Smart Software Solutions. 
 */
#define RTS_VERSION							UINT32_C(0x03051300)
#define RTS_VERSION_STRING					"3.5.19.0"
#define RTS_VERSION_RC						3,5,19,0

#define RTS_GETVERSION_MAJOR(version)		(((version)>>24) & UINT32_C(0x000000FF))
#define RTS_GETVERSION_SUB(version)			(((version)>>16) & UINT32_C(0x000000FF))
#define RTS_GETVERSION_SERVICEPACK(version)	(((version)>>8) & UINT32_C(0x000000FF))
#define RTS_GETVERSION_PATCH(version)		((version) & UINT32_C(0x000000FF))

#define RTS_SETVERSION(major, minor, servicepack, patch)		(((major & UINT32_C(0x000000FF)) << 24) | \
																 ((minor & UINT32_C(0x000000FF)) << 16) | \
																 ((servicepack & UINT32_C(0x000000FF)) << 8) | \
																 (patch & UINT32_C(0x000000FF)))

#ifndef RTS_PRODUCT_FAMILY_NAME
	#define RTS_PRODUCT_FAMILY_NAME			"CODESYS"
#endif

#ifndef RTS_PRODUCT_NAME
	#define RTS_PRODUCT_NAME				RTS_PRODUCT_FAMILY_NAME" Control"
#endif

#ifndef RTS_CONFIGURATION_FILE
	#define RTS_CONFIGURATION_FILE			RTS_PRODUCT_FAMILY_NAME"Control.cfg"
#endif

#ifndef RC_INVOKED

#ifndef RTS_ASSERT
	#define RTS_ASSERT(a)
#endif

/*
 * Macros to export and check availability of functions in statically linked
 * runtimes. For performance reasons, those checks are disabled by default.
 * But for larger systems, they should be enabled, to avoid misconfigurations
 * of the system.
 * The macros can look, s.th. like this and should be defined in sysdefines.h:
 * #define CAL_CMGETAPI(name) s_pfCMGetAPI( name, (RTS_VOID_FCTPTR *)NULL, 0)
 * #define CAL_CMEXPAPI(name) s_pfCMRegisterAPI( name, (RTS_VOID_FCTPTR)-1, 0, 0)
 */
#ifndef CAL_CMGETAPI
	#define CAL_CMGETAPI(name) ERR_OK
#endif
#ifndef CAL_CMEXPAPI
	#define CAL_CMEXPAPI(name) ERR_OK
#endif


#if !defined(__cplusplus) /* no need for C++ */
   #if (__STDC_VERSION__ < 199901L) /* no need vor C99 or newer */
      #if defined(_MSC_VER) && !defined(inline)
        #define inline __inline
      #endif
  #endif
#endif


/**
 * This define specifies, that the bits inside of a bitfield are
 * organized in oposite order. Usually this is the case for
 * platforms with motorola byte order, but some compilers are
 * doing it differently.
 */
#ifndef RTS_BIGENDIAN_BITORDER
	#if defined(MOTOROLA_BYTE_ORDER) || defined(RTS_BIGENDIAN_BYTEORDER)
		#define RTS_BIGENDIAN_BITORDER
	#endif
#endif


#ifndef RTS_OS_KEYPRESSED
	#define RTS_OS_KEYPRESSED			FALSE
#endif

#define RTS_MAXSTR				80

#ifndef _RTS_VOID_FCTPTR
	#define _RTS_VOID_FCTPTR
	typedef void HUGEPTR(CDECL *RTS_VOID_FCTPTR)(void);
#endif

/*
 * Runtime system basic typedefinitions.
 * Defines some types with special meaning. Systems not using standard type sizes 
 * must redefine this types in sysdefines.h and set BASETYPES_DEFINED
 */

#ifndef RTS_I64_CONST
	#define RTS_I64_CONST(a)	(a##LL)
#endif
#ifndef RTS_UI64_CONST
	#define RTS_UI64_CONST(a)	(a##ULL)
#endif

#ifndef BASETYPES_DEFINED
#	define BASETYPES_DEFINED

	typedef int8_t				RTS_I8;		/* Signed 8 bit value */
	typedef int16_t				RTS_I16;	/* Signed 16 bit value */
#ifndef RTS_I32_DEFINED
	#define RTS_I32_DEFINED
	typedef int32_t				RTS_I32; 	/* Signed 32 bit value */
#endif

	typedef uint8_t				RTS_UI8;	/* Unsigned 8 bit value */
#	define	RTS_UI8_MAX			(RTS_UI8)(~((RTS_UI8)0))

	typedef uint16_t			RTS_UI16;	/* Unsigned 16 bit value */
#	define	RTS_UI16_MAX		(RTS_UI16)(~((RTS_UI16)0))

#ifndef RTS_UI32_DEFINED
	#define RTS_UI32_DEFINED
	typedef uint32_t			RTS_UI32;	/* Unsigned 32 bit value */
#endif	
#	define	RTS_UI32_MAX		(RTS_UI32)(~((RTS_UI32)0))

#ifndef IEEE754TYPES_DEFINED
#	define IEEE754TYPES_DEFINED
	typedef float				RTS_REAL32;		/* 32 bit floating point value */
	typedef double				RTS_REAL64;		/* 64 bit floating point value */
	#define RTS_REAL32_MAX		3.4028234e+38
	#define RTS_REAL64_MAX		1.7976931348623157e+308
#endif

	typedef void*				RTS_HANDLE;		/* Handle for files, semaphores, ... Usually the native integer type of the platform*/
	#define RTS_INVALID_HANDLE	((RTS_HANDLE)(~((uintptr_t)0)))

	typedef RTS_UI32			RTS_RESULT;		/* Result type for most functions. At least 16 bit. Should be the natural integer of a platform. */

	typedef uintptr_t			RTS_UINTPTR;	/* Unsigned integral type that can hold a pointer */
	#define RTS_UINTPTR_MAX		(RTS_UINTPTR)(~((RTS_UINTPTR)0))

	typedef intptr_t			RTS_INTPTR;		/* Signed integral type that can hold a pointer */
	#define RTS_INTPTR_MAX		(RTS_INTPTR)(~((RTS_INTPTR)0))

	typedef ptrdiff_t			RTS_PTRDIFF;	/* Signed integral type that can hold an array index */
	#define RTS_PTRDIFF_MAX		(RTS_PTRDIFF)(~((RTS_PTRDIFF)0))

#ifndef RTS_SIZE_DEFINED
	#define RTS_SIZE_DEFINED
	typedef size_t				RTS_SIZE;	/* Unsigned integral type that can hold a buffer offset */
#endif
	#define RTS_SIZE_MAX		(RTS_SIZE)(~((RTS_SIZE)0))

/* Type definition of RTS_SSIZE should be changed to standard datatype ssize_t, if available on any target! */
#ifndef RTS_SSIZE_DEFINED
	#define RTS_SSIZE_DEFINED
	typedef intptr_t	RTS_SSIZE;			/* integral type that can hold a buffer offset */
#endif
	#define RTS_SSIZE_MAX		(RTS_SSIZE)(~((RTS_SSIZE)0))

	typedef RTS_UINTPTR	RTS_TIME;			/* Coordinated Universal Time (UTC) */

	typedef int					RTS_INT;	/* Data type has no constant size, so be careful in sharing shuch datatypes with IEC! */
	typedef int					RTS_BOOL;	/* Boolean value (TRUE or FALSE). For best performance use platform specific int.
											   Data type has no constant size, so be carful in sharing shuch datatypes with IEC! */
#if defined(RTS_DISABLE_64BIT_SUPPORT) && !defined(SYSINTERNAL_DISABLE_64BIT)
	#define SYSINTERNAL_DISABLE_64BIT
#endif

#if !defined(BASE64BITTYPES_DEFINED) && !defined(SYSINTERNAL_DISABLE_64BIT)
#	define	BASE64BITTYPES_DEFINED
	typedef uint64_t			RTS_UI64;
	typedef int64_t				RTS_I64;
	#define RTS_UI64_MAX		(RTS_UI64)(~((RTS_UI64)0))
	#define RTS_I64_MAX			(RTS_I64)(~((RTS_I64)0))
#endif

	/* C-string macros */
	#ifndef RTS_CSTRING_TEXT
		#define RTS_CSTRING_TEXT
	#endif
	#define RTS_CSTRING_EMPTY			RTS_CSTRING_TEXT("")
	/* UTF-8-string macros */
	#ifndef RTS_UTF8STRING_TEXT
		#define RTS_UTF8STRING_TEXT		u8
	#endif
	#define RTS_UTF8STRING_EMPTY		RTS_UTF8STRING_TEXT("")

	typedef char				RTS_CSTRING;	/* Standard C-string with UTF-8 support */
	typedef char				RTS_UTF8STRING;	/* Explicitly marked UTF-8 support */

#	ifndef RTS_WCHAR_DEFINED
#		define RTS_WCHAR_DEFINED
		typedef wchar_t			RTS_WCHAR;	/* wide character value. We expect 2 bytes unicode here! */
#	endif	/*RTS_WCHAR_DEFINED*/

#	ifndef RTS_CWCHAR_DEFINED
#		define RTS_CWCHAR_DEFINED
#		ifdef RTS_UNICODE
			typedef RTS_WCHAR	RTS_CWCHAR;
#		else
			typedef RTS_CSTRING	 RTS_CWCHAR;
#		endif
#	endif	/*RTS_CWCHAR_DEFINED*/

#ifdef RTS_DEFINE_MODE_T
	typedef unsigned short mode_t;
#endif
#ifdef RTS_DEFINE_GID_T
	typedef short gid_t;
#endif
#ifdef RTS_DEFINE_UID_T
	typedef short uid_t;
#endif
#ifdef RTS_DEFINE_ID_T
	typedef intptr_t id_t;
#endif
#ifdef RTS_DEFINE_PID_T
	typedef uintptr_t pid_t;
#endif
#ifdef RTS_DEFINE_OFF_T
	typedef uintptr_t off_t;
#endif
#ifdef RTS_DEFINE_NLINK_T
	typedef	short nlink_t;
#endif

	

#endif /*BASETYPES_DEFINED*/

typedef enum
{
    RTSTYPE_I8,
    RTSTYPE_I16,
    RTSTYPE_I32,
    RTSTYPE_UI8,
    RTSTYPE_UI16,
    RTSTYPE_UI32,
    RTSTYPE_REAL32,
    RTSTYPE_REAL64,
    RTSTYPE_HANDLE,
    RTSTYPE_RESULT,
    RTSTYPE_UINTPTR,
    RTSTYPE_INTPTR,
    RTSTYPE_SIZE,
    RTSTYPE_PTRDIFF,
    RTSTYPE_INT,
    RTSTYPE_BOOL,
    RTSTYPE_UI64,
    RTSTYPE_I64,
    RTSTYPE_CSTRING,
    RTSTYPE_WCHAR,
    RTSTYPE_CWCHAR
}RTS_TYPECLASS;

typedef union
{
    RTS_I8          i8Value;
    RTS_I16         i16Value;
    RTS_I32         i32Value;
    RTS_UI8         ui8Value;
    RTS_UI16        ui16Value;
    RTS_UI32        ui32Value;
    RTS_REAL32      real32Value;
    RTS_REAL64      real64Value;
    RTS_HANDLE      handleValue;
    RTS_RESULT      resultValue;
    RTS_UINTPTR     uintptrValue;
    RTS_INTPTR      intptrValue;
    RTS_SIZE        sizeValue;
    RTS_PTRDIFF     ptrdiffValue;
    RTS_INT         intValue;
    RTS_BOOL        boolValue;
    RTS_UI64        ui64Value;
    RTS_I64         i64Value;
    RTS_CSTRING     *cstringValue;
    RTS_WCHAR       *wstringValue;
    RTS_CWCHAR      *cwstringValue;
}RTS_VALUE;

typedef struct
{
    RTS_TYPECLASS typeClass;
    RTS_VALUE value;
}RTS_TYPEDVALUE;


/*
 * IEC basic typedefinitions.
 * Defines all datatypes used in IEC programs. Datatypes must match to CoDeSys compiler.
 * Systems that overloads standard types must redefine this types in sysdefines.h and set IECBASETYPES_DEFINED
 */
#ifndef IECBASETYPES_DEFINED
#define IECBASETYPES_DEFINED

#	define RTS_IEC_VOID_FCTPTR		RTS_VOID_FCTPTR
	typedef void*			RTS_IEC_VOIDPTR;

	typedef RTS_I8			RTS_IEC_BOOL;
	typedef RTS_I8			RTS_IEC_SINT;
	typedef RTS_UI8			RTS_IEC_USINT;
	typedef RTS_UI8			RTS_IEC_BYTE;
	typedef RTS_I16			RTS_IEC_INT;
	typedef RTS_UI16		RTS_IEC_UINT;
	typedef RTS_UI16		RTS_IEC_WORD;
	typedef RTS_I32			RTS_IEC_DINT;
	typedef RTS_UI32		RTS_IEC_UDINT;
	typedef RTS_UI32		RTS_IEC_DWORD;

	typedef RTS_UI32		RTS_IEC_TIME;
	typedef RTS_UI32		RTS_IEC_TIME_OF_DAY;
	typedef RTS_UI32		RTS_IEC_TOD;
	typedef RTS_UI32		RTS_IEC_DATE_AND_TIME;
	typedef RTS_UI32		RTS_IEC_DT;
	typedef RTS_UI32		RTS_IEC_DATE;
	
	typedef RTS_I64			RTS_IEC_LINT;
	typedef RTS_UI64		RTS_IEC_ULINT;
	typedef RTS_UI64		RTS_IEC_LWORD;
	typedef RTS_UI64		RTS_IEC_LTIME;

	typedef RTS_SIZE		RTS_IEC_XWORD;
	typedef RTS_UINTPTR		RTS_IEC_UXINT;
	typedef RTS_PTRDIFF		RTS_IEC_XINT;

	typedef RTS_REAL32		RTS_IEC_REAL;
	typedef RTS_REAL64		RTS_IEC_LREAL;

	typedef RTS_CSTRING		RTS_IEC_STRING;
#	ifdef RTS_UNICODE
		typedef RTS_WCHAR	RTS_IEC_WSTRING;
#	else
		typedef RTS_UI16	RTS_IEC_WSTRING;
#	endif
	typedef RTS_CWCHAR		RTS_IEC_CWCHAR;

	typedef RTS_SIZE		RTS_IEC_SIZE;
	typedef RTS_HANDLE		RTS_IEC_HANDLE;
	typedef RTS_IEC_UDINT	RTS_IEC_RESULT;
#endif	/*IECBASETYPES_DEFINED*/


/*
 * IEC safe typedefinitions.
 * Defines all safety datatypes used in IEC programs. Datatypes must match to CODESYS compiler.
 * Systems that overloads safety types must redefine this types in sysdefines.h and set IECBASETYPES_DEFINED
 */
#ifndef IECSAFETYPES_DEFINED
#define IECSAFETYPES_DEFINED

	typedef RTS_IEC_BOOL	RTS_IEC_SAFEBOOL;
	typedef RTS_IEC_BYTE	RTS_IEC_SAFEBYTE;
	typedef RTS_IEC_SINT	RTS_IEC_SAFESINT;
	typedef RTS_IEC_USINT	RTS_IEC_SAFEUSINT;
	typedef RTS_IEC_WORD	RTS_IEC_SAFEWORD;
	typedef RTS_IEC_INT		RTS_IEC_SAFEINT;
	typedef RTS_IEC_UINT	RTS_IEC_SAFEUINT;
	typedef RTS_IEC_DWORD	RTS_IEC_SAFEDWORD;
	typedef RTS_IEC_DINT	RTS_IEC_SAFEDINT;
	typedef RTS_IEC_UDINT	RTS_IEC_SAFEUDINT;
	typedef RTS_IEC_LWORD	RTS_IEC_SAFELWORD;
	typedef RTS_IEC_LINT	RTS_IEC_SAFELINT;
	typedef RTS_IEC_ULINT	RTS_IEC_SAFEULINT;
	typedef RTS_IEC_TIME	RTS_IEC_SAFETIME;
	typedef RTS_IEC_REAL	RTS_IEC_SAFEREAL;
	typedef RTS_IEC_LREAL	RTS_IEC_SAFELREAL;

#endif	/*IECSAFETYPES_DEFINED*/


/**
*	<description>
*	Data type RTS_GUID:
*	Structure with a defined length of 16 octets on all plattforms to transport the packet representation of a GUID in a binary format.
*	The internal representation of each RTS_GUID field is defined in Intel byte order. 
*
*   ATTENTION:
*   To write platform independent code, the RTS_GUID structure must only be used like this:
*   - Never access struct fields of RTS_GUID directly!
*	- Never interpret the content of variables of type RTS_GUID!
*	- Do not implement own code for RTS_GUID from/to string. Use the conversion functions CMUtlGUIDToString() and CMUtlStringToGUID() instead. 
*     These are implemented platform specificly and are aware of the internal format. 
*   - Beside the string conversion, variables of type RTS_GUID should be stored and compared only. 
*     For this memcpy()/memcmp() with sizeof(RTS_GUID) as length parameter should be used. 
*	</description>
*/
/* <SIL2/> */
typedef struct tagRTS_GUID
{
	RTS_UI32 data1;
	RTS_UI16 data2;
	RTS_UI16 data3;
#ifdef RTS_SIXTEENBITBYTES
	/* note: this field must contain packed bytes, i.e 8 octets! */
	RTS_UI16 data4packed[4];
#else
	RTS_UI8 data4[8];
#endif
} RTS_GUID;

#ifndef RTS_MEM_REGION_DEFINED
#define RTS_MEM_REGION_DEFINED

/**
 *	<description>
 *	 Abstract Memory Region.
 *	</description>
 */  
typedef struct tagRTS_MEM_REGION
{
	/* base address of the region */
	RTS_UINTPTR base;

	/* region size */
	RTS_SIZE		size;

} RTS_MEM_REGION;
#endif

#ifndef EXT_FUNCT_REF_DEFINED
#define EXT_FUNCT_REF_DEFINED
/**
 *	Exported function descriptor
 */
typedef struct tagCMP_EXT_FUNCTION_REF
{
	/* pointer to an exported function */
	RTS_VOID_FCTPTR	pfExtCall;

	/* name of an exported function */
	const char*			pszExtCallName;

	/* signature */
	RTS_UI32				signature;

	/* version */
	RTS_UI32				version;

} CMP_EXT_FUNCTION_REF;
#endif

/* The CMGetAPI2/CMGetAPI3 importOptions parameter flag which specifies an import */
/* as a required function (error if it is unresolved) */
#define CM_IMPORT_REQUIRED_FUNCTION			0

/* The CMGetAPI2/CMGetAPI3 importOptions parameter flag which specifies an import */
/* as an external libray function */
#define CM_IMPORT_EXTERNAL_LIB_FUNCTION		1

/* The CMGetAPI2/CMGetAPI3 importOptions parameter flag which specifies an import */
/* as an optional function */
#define CM_IMPORT_OPTIONAL_FUNCTION			2

/* Imported function belongs to an external libray */
#define ImportIsExternalLibFunction(par) (0 != ((par) & CM_IMPORT_EXTERNAL_LIB_FUNCTION))

/* Imported function is optional */
#define ImportIsOptionalFunction(par) (0 != ((par) & CM_IMPORT_OPTIONAL_FUNCTION))


/**
 * <category>Handling of Infinity</category>
 * <description>
 *     Check Macros to check a REAL value for positive or negative infinity.
 * </description>
 */
#ifndef RTS_IS_INFINITY_DEFINED
	#define RTS_IS_REAL32_INFINITY_POS(dVal) (*((RTS_UI32*)&dVal) == (UINT32_C(0x7f800000)))
	#define RTS_IS_REAL32_INFINITY_NEG(dVal) (*((RTS_UI32*)&dVal) == (UINT32_C(0xff800000)))
	#define RTS_IS_REAL64_INFINITY_POS(dVal) (*((RTS_UI64*)&dVal) == (RTS_UI64_CONST(0x7ff0000000000000)))
	#define RTS_IS_REAL64_INFINITY_NEG(dVal) (*((RTS_UI64*)&dVal) == (RTS_UI64_CONST(0xfff0000000000000)))
#endif


/**
 * <category>DSP specific sizeof and offset definitions</category>
 * <description>
 *	sizeof() on a DSP platform returned 1 for byte and word datatypes, so sizeof() must be handled specific.
 *	Offset calculations must be done WORD wize!
 * </description>
 */
#ifdef RTS_SIXTEENBITBYTES
	#define sizeof_RTS_I8					1
	#define sizeof_RTS_UI8					1
	#define sizeof_RTS_I16					2
	#define sizeof_RTS_UI16					2
	#define sizeof_RTS_I32					4
	#define sizeof_RTS_UI32					4
	#define sizeof_RTS_UINTPTR				4
	#define sizeof_RTS_SIZE					sizeof_RTS_UINTPTR
	#define sizeof_RTS_IEC_DWORD			sizeof_RTS_UI32
	#define sizeof_RTS_RESULT				sizeof_RTS_UI32
	#define sizeof_RTS_HANDLE				sizeof_RTS_UI32
	#define sizeof_RTS_WCHAR				sizeof_RTS_UI16
	#define sizeof_RTS_IEC_DATE_AND_TIME	sizeof_RTS_UI32
	#define sizeof_AREA_INFO				(sizeof(AREA_INFO) * 2)
	#define sizeof_EXT_REF_INFO				(sizeof(EXT_REF_INFO) * 2)
	#define sizeof_ADDRESSCOMPONENT         sizeof(ADDRESSCOMPONENT)
	#define sizeof_CMPID					sizeof_RTS_UI32

	#define RTS_SIZEOF(type)				sizeof_##type
	#define RTS_GET_WORD_OFFSET(offset)		((offset) / 2)
	#define RTS_GET_WORD_SIZE				RTS_GET_WORD_OFFSET
	#define RTS_GET_BYTE_OFFSET(offset)		((offset) * 2)
	#define RTS_GET_BYTE_SIZE				RTS_GET_BYTE_OFFSET
	#define RTS_MEMSET(dest, c, count)		memset(dest, c, count / 2)
	#define RTS_ALIGN_4BYTES				2
	#define RTS_BITS_IN_BYTE				16
#else
	#define RTS_SIZEOF						sizeof
	#define RTS_GET_WORD_OFFSET(typesize)	(typesize)
	#define RTS_GET_WORD_SIZE				RTS_GET_WORD_OFFSET
	#define RTS_GET_BYTE_OFFSET(offset)		(offset)
	#define RTS_GET_BYTE_SIZE(size)			(size)
	#define RTS_MEMSET						memset
	#define RTS_ALIGN_4BYTES				4
	#define RTS_BITS_IN_BYTE				8
#endif

#define RTS_EMPTY_STRING			""
#define RTS_IS_EMPTY_STRING(s)		((char *)s == NULL || strcmp((char *)s, RTS_EMPTY_STRING) == 0)

#define RTS_TIMEOUT_INFINITE	(~(RTS_UI32)0)
#define RTS_TIMEOUT_DEFAULT		(RTS_UI32)(-2)
#define RTS_TIMEOUT_NO_WAIT		(0)

#define RTS_INVALID_FILETRANSFER	(RTS_UI32)(-1)

#include "CmpItf.h"

#ifdef PLC_HANDLER
	#include "CmpPLCHandler.h"
#endif

/* 
 * This file must exist in a systemspecific include directory (eg. <rts_src>\Win32) 
 * and may include any additional includes/definitions needed for a special system/environment.
 */
#include "sysspecific.h"


#ifdef PREFER_PORTABLE_SNPRINTF

/* Portable snprintf routines are implemented in CMConvToStr.c */
#ifdef CPLUSPLUS
extern "C"
{
#endif
	int CDECL CMUtlPortableSnprintf(char* str, RTS_SIZE buf_len, const char* fmt, ...);
	int CDECL CMUtlPortableVsnprintf(char* str, RTS_SIZE buf_len, const char* fmt, va_list ap);
#ifdef CPLUSPLUS
}
#endif

#	ifdef RTS_VSNPRINTF
#	undef RTS_VSNPRINTF
#	define RTS_VSNPRINTF(s,len,format,arglist) CMUtlPortableVsnprintf((s), (len), (format), (arglist))
#	endif

#endif


#ifndef  COMPONENT_NAME_POSTFIX
	#define COMPONENT_NAME_POSTFIX
#endif

#ifdef TRG_64BIT
#	ifndef CM_OBJECT_MANAGER_ENABLED
#		define CM_OBJECT_MANAGER_ENABLED
#	endif
#else
	#define CMPOBJECTMGR_NOTIMPLEMENTED
#endif

#endif /* RC_INVOKED */

#if !defined(RTS_CONSOLE_BANNER) && !defined(RTS_DISABLE_CONSOLE_BANNER)
	#define RTS_CONSOLE_BANNER		" _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/\n"\
									"_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/\n"\
									"\n"\
									"        _/_/_/_/      _/_/_/_/    _/_/_/_/_/    _/_/_/_/_/_/    _/_/_/_/_/  _/_/    _/_/    _/_/_/_/_/          _/\n"\
									"     _/_/_/_/_/_/  _/_/_/_/_/_/  _/_/_/_/_/_/  _/_/_/_/_/_/  _/_/_/_/_/_/  _/_/    _/_/  _/_/_/_/_/_/        _/_/_/\n"\
									"    _/_/    _/_/  _/_/    _/_/  _/_/    _/_/  _/_/          _/_/          _/_/    _/_/  _/_/              _/_/_/_/_/\n"\
									"   _/_/          _/_/    _/_/  _/_/    _/_/  _/_/_/_/        _/_/_/_/      _/_/_/_/      _/_/_/_/      _/_/_/_/_/_/_/\n"\
									"  _/_/    _/_/  _/_/    _/_/  _/_/    _/_/  _/_/                  _/_/      _/_/              _/_/      _/_/_/_/_/\n"\
									" _/_/_/_/_/_/  _/_/_/_/_/_/  _/_/_/_/_/_/  _/_/_/_/_/_/  _/_/_/_/_/_/      _/_/      _/_/_/_/_/_/        _/_/_/\n"\
									"  _/_/_/_/      _/_/_/_/    _/_/_/_/_/    _/_/_/_/_/_/  _/_/_/_/_/        _/_/      _/_/_/_/_/            _/\n"\
									"\n"\
									" _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/\n"\
									"_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/"
#endif

#endif /*__RTSSTD_H__*/
