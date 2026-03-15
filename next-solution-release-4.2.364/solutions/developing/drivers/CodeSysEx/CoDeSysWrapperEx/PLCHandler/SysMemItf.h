 /**
 * <interfacename>SysMem</interfacename>
 * <description> 
 *	<p>The SysMem interface is projected to get access to heap memory or special
 *	memory areas for the PLC program.</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _SYSMEMITF_H_
#define _SYSMEMITF_H_

#include "CmpStd.h"

 

 




#ifndef CDECL
	#define CDECL
#endif

/**
 * <category>Area Types</category>
 * <description>Application Area Type: None</description>
 */
#define DA_NONE			0x0000

/**
 * <category>Area Types</category>
 * <description>Application Area Type: Data</description>
 */
#define DA_DATA			0x0001

/**
 * <category>Area Types</category>
 * <description>Application Area Type: Constant</description>
 */
#define DA_CONSTANT		0x0002

/**
 * <category>Area Types</category>
 * <description>Application Area Type: Input</description>
 */
#define DA_INPUT		0x0004
/**
 * <category>Area Types</category>
 * <description>Application Area Type: Output</description>
 */
#define DA_OUTPUT		0x0008

/**
 * <category>Area Types</category>
 * <description>Application Area Type: Memory</description>
 */
#define DA_MEMORY		0x0010

/**
 * <category>Area Types</category>
 * <description>Application Area Type: Retain</description>
 */
#define DA_RETAIN		0x0020

/**
 * <category>Area Types</category>
 * <description>Application Area Type: Code</description>
 */
#define DA_CODE			0x0040

/* #define DA_AREA		0x0080	!RESERVED FOR INTERNAL USE IN CODESYS ONLY! */

/**
 * <category>Area Types</category>
 * <description>Application Area Type: Persistent</description>
 */
#define DA_PERSISTENT	0x0100

/**
 * <category>Area Types</category>
 * <description>Application Area Type: Non Safety data within Safety SIL2 systems</description>
 */
#define DA_NONSAFETY	0x0800

/**
 * <category>Area Types</category>
 * <description>Application Area Type: All</description>
 */
#define DA_ALL			0xFFFF

/**
 * <category>Area Types</category>
 * <description>Define for checking Area Types</description>
 */
#define IsArea(type, DA)	(type & DA)

/**
 * <category>Static defines</category>
 * <description>
 *	Required alignment for all areas: Code, Data, RETAIN, RETINA PERSISTENT, Marker, etc.
 *	Note: If CmpMemGC is used, only 8 or 32 byte alignment is supported.
 * </description>
 */
#ifndef SYSMEM_IECAREA_BYTE_ALIGNMENT
#if defined (TRG_64BIT)
	#define SYSMEM_IECAREA_BYTE_ALIGNMENT		32
#else
	#define SYSMEM_IECAREA_BYTE_ALIGNMENT		8
#endif
#endif

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Settings to set the a special address for %M variables (markers)</description>
 */
#define CMPMEMKEY_MEMORYADDRESS	"WinCE.MemoryAddress"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting for size of a special area (only one fixed big area for code and data)
 * Attention: only for use with Windows CE and versions &lt; 6
 * Attention: only for use together with a special device description with fixed size, e.g. 32 MB
 * Example:
 * <pre>
 *	&lt;ts:setting name="flags" type="integer" access="visible"&gt;
 *		&lt;ts:value&gt;0xfedf&lt;/ts:value&gt;
 *  &lt;/ts:setting&gt;
 *  &lt;ts:setting name="minimal-area-size" type="integer" access="visible"&gt;
 *		&lt;ts:value&gt;0x2000000&lt;/ts:value&gt;
 *  &lt;/ts:setting&gt;
 *  &lt;ts:setting name="maximal-area-size" type="integer" access="visible"&gt;
 *		&lt;ts:value&gt;0x2000000&lt;/ts:value&gt;
 *  &lt;/ts:setting&gt;
 *  &lt;ts:setting name="allocation-plus-in-percent" type="integer" access="visible"&gt;
 *		 &lt;ts:value&gt;0&lt;/ts:value&gt;
 *  &lt;/ts:setting&gt;
 * </pre>
 * </description>
 */
#define SYSMEMKEY_WINCE_CE5_BIGAREASIZE				"WinCE.CE5BigAreaSize"
#define SYSMEMKEY_WINCE_CE5_BIGAREASIZE_DEFAULT		0

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Avoid paging of RAM that is used by the CODESYS runtime process.
 * This is only useful if the application has real-time requirements</description>
 */
#define SYSMEMKEY_INT_LINUX_MEMLOCK					"Linux.Memlock"
#if defined (CMPAPP_NOTIMPLEMENTED) || SYSTARGET_DEVICE_TYPE==SYSTARGET_TYPE_HMI
 #define SYSMEMKEY_INT_LINUX_MEMLOCK_DEFAULT		0
#else
 #define SYSMEMKEY_INT_LINUX_MEMLOCK_DEFAULT		1
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Specify the flags of a special (= not dynamically allocated) memory area</description>
 */
#define SYSMEMKEY_INT_VXWORKS_FLAGS         "VxWorks.Area.%u.Flags"
#define SYSMEMKEY_INT_VXWORKS_FLAGS_DEFAULT 0

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Specify the address of a special (= not dynamically allocated) memory area</description>
 */
#define SYSMEMKEY_INT_VXWORKS_ADDRESS         "VxWorks.Area.%u.Address"
#define SYSMEMKEY_INT_VXWORKS_ADDRESS_DEFAULT 0

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Specify the size of a special (= not dynamically allocated) memory area</description>
 */
#define SYSMEMKEY_INT_VXWORKS_SIZE         "VxWorks.Area.%u.Size"
#define SYSMEMKEY_INT_VXWORKS_SIZE_DEFAULT 0

/**
 * <category>Memory protection constants</category>
 * <type>RTS_UI32</type>
 * <description>
 *	Protections are chosen from these bits, OR'd together.  The
 *	implementation does not necessarily support SYSMEM_PROTECT_EXEC or SYSMEM_PROTECT_WRITE
 *	without SYSMEM_PROTECT_READ.  The only guarantees are that no writing will be
 *	allowed without SYSMEM_PROTECT_WRITE and no access will be allowed for SYSMEM_PROTECT_NONE.
 * </description>
 * <element name="SYSMEM_PROTECT_READ">Page can be read.</element>
 * <element name="SYSMEM_PROTECT_WRITE">Page can be written.</element>
 * <element name="SYSMEM_PROTECT_EXEC">Page can be executed.</element>
 * <element name="SYSMEM_PROTECT_NONE">Page can not be accessed.</element>
 */
#define SYSMEM_PROTECT_READ		UINT32_C(0x1)
#define SYSMEM_PROTECT_WRITE	UINT32_C(0x2)
#define SYSMEM_PROTECT_EXEC		UINT32_C(0x4)
#define SYSMEM_PROTECT_NONE		UINT32_C(0x0)

/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * Reserved function for the runtime system! Should not be used by an IEC application!
 */
typedef struct tagsysmemallocarea_struct
{
	RTS_IEC_STRING *szComponent;		/* VAR_INPUT */	
	RTS_IEC_WORD usType;				/* VAR_INPUT */	
	RTS_IEC_XWORD udiSize;				/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_BYTE *SysMemAllocArea;		/* VAR_OUTPUT */	
} sysmemallocarea_struct;

void CDECL CDECL_EXT sysmemallocarea(sysmemallocarea_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMALLOCAREA_IEC) (sysmemallocarea_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMALLOCAREA_NOTIMPLEMENTED)
	#define USE_sysmemallocarea
	#define EXT_sysmemallocarea
	#define GET_sysmemallocarea(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemallocarea(p0) 
	#define CHK_sysmemallocarea  FALSE
	#define EXP_sysmemallocarea  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemallocarea
	#define EXT_sysmemallocarea
	#define GET_sysmemallocarea(fl)  CAL_CMGETAPI( "sysmemallocarea" ) 
	#define CAL_sysmemallocarea  sysmemallocarea
	#define CHK_sysmemallocarea  TRUE
	#define EXP_sysmemallocarea  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocarea", (RTS_UINTPTR)sysmemallocarea, 1, RTSITF_GET_SIGNATURE(0, 0x140ECCD0), 0x03051100) 
	#define EXTREF_ITF_sysmemallocarea {(RTS_VOID_FCTPTR)sysmemallocarea, "sysmemallocarea", RTSITF_GET_SIGNATURE(0, 0x140ECCD0), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemallocarea
	#define EXT_sysmemallocarea
	#define GET_sysmemallocarea(fl)  CAL_CMGETAPI( "sysmemallocarea" ) 
	#define CAL_sysmemallocarea  sysmemallocarea
	#define CHK_sysmemallocarea  TRUE
	#define EXP_sysmemallocarea  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocarea", (RTS_UINTPTR)sysmemallocarea, 1, RTSITF_GET_SIGNATURE(0, 0x140ECCD0), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemallocarea
	#define EXT_SysMemsysmemallocarea
	#define GET_SysMemsysmemallocarea  ERR_OK
	#define CAL_SysMemsysmemallocarea  sysmemallocarea
	#define CHK_SysMemsysmemallocarea  TRUE
	#define EXP_SysMemsysmemallocarea  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocarea", (RTS_UINTPTR)sysmemallocarea, 1, RTSITF_GET_SIGNATURE(0, 0x140ECCD0), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemallocarea
	#define EXT_sysmemallocarea
	#define GET_sysmemallocarea(fl)  CAL_CMGETAPI( "sysmemallocarea" ) 
	#define CAL_sysmemallocarea  sysmemallocarea
	#define CHK_sysmemallocarea  TRUE
	#define EXP_sysmemallocarea  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocarea", (RTS_UINTPTR)sysmemallocarea, 1, RTSITF_GET_SIGNATURE(0, 0x140ECCD0), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemallocarea  PFSYSMEMALLOCAREA_IEC pfsysmemallocarea;
	#define EXT_sysmemallocarea  extern PFSYSMEMALLOCAREA_IEC pfsysmemallocarea;
	#define GET_sysmemallocarea(fl)  s_pfCMGetAPI2( "sysmemallocarea", (RTS_VOID_FCTPTR *)&pfsysmemallocarea, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x140ECCD0), 0x03051100)
	#define CAL_sysmemallocarea  pfsysmemallocarea
	#define CHK_sysmemallocarea  (pfsysmemallocarea != NULL)
	#define EXP_sysmemallocarea   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocarea", (RTS_UINTPTR)sysmemallocarea, 1, RTSITF_GET_SIGNATURE(0, 0x140ECCD0), 0x03051100) 
#endif


/**
 * Reserved function for the runtime system! Should not be used by an IEC application!
 */
typedef struct tagsysmemalloccode_struct
{
	RTS_IEC_STRING *szComponent;		/* VAR_INPUT */	
	RTS_IEC_XWORD udiSize;				/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_BYTE *SysMemAllocCode;		/* VAR_OUTPUT */	
} sysmemalloccode_struct;

void CDECL CDECL_EXT sysmemalloccode(sysmemalloccode_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMALLOCCODE_IEC) (sysmemalloccode_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMALLOCCODE_NOTIMPLEMENTED)
	#define USE_sysmemalloccode
	#define EXT_sysmemalloccode
	#define GET_sysmemalloccode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemalloccode(p0) 
	#define CHK_sysmemalloccode  FALSE
	#define EXP_sysmemalloccode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemalloccode
	#define EXT_sysmemalloccode
	#define GET_sysmemalloccode(fl)  CAL_CMGETAPI( "sysmemalloccode" ) 
	#define CAL_sysmemalloccode  sysmemalloccode
	#define CHK_sysmemalloccode  TRUE
	#define EXP_sysmemalloccode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemalloccode", (RTS_UINTPTR)sysmemalloccode, 1, RTSITF_GET_SIGNATURE(0, 0xB0EDB0B3), 0x03051100) 
	#define EXTREF_ITF_sysmemalloccode {(RTS_VOID_FCTPTR)sysmemalloccode, "sysmemalloccode", RTSITF_GET_SIGNATURE(0, 0xB0EDB0B3), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemalloccode
	#define EXT_sysmemalloccode
	#define GET_sysmemalloccode(fl)  CAL_CMGETAPI( "sysmemalloccode" ) 
	#define CAL_sysmemalloccode  sysmemalloccode
	#define CHK_sysmemalloccode  TRUE
	#define EXP_sysmemalloccode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemalloccode", (RTS_UINTPTR)sysmemalloccode, 1, RTSITF_GET_SIGNATURE(0, 0xB0EDB0B3), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemalloccode
	#define EXT_SysMemsysmemalloccode
	#define GET_SysMemsysmemalloccode  ERR_OK
	#define CAL_SysMemsysmemalloccode  sysmemalloccode
	#define CHK_SysMemsysmemalloccode  TRUE
	#define EXP_SysMemsysmemalloccode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemalloccode", (RTS_UINTPTR)sysmemalloccode, 1, RTSITF_GET_SIGNATURE(0, 0xB0EDB0B3), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemalloccode
	#define EXT_sysmemalloccode
	#define GET_sysmemalloccode(fl)  CAL_CMGETAPI( "sysmemalloccode" ) 
	#define CAL_sysmemalloccode  sysmemalloccode
	#define CHK_sysmemalloccode  TRUE
	#define EXP_sysmemalloccode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemalloccode", (RTS_UINTPTR)sysmemalloccode, 1, RTSITF_GET_SIGNATURE(0, 0xB0EDB0B3), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemalloccode  PFSYSMEMALLOCCODE_IEC pfsysmemalloccode;
	#define EXT_sysmemalloccode  extern PFSYSMEMALLOCCODE_IEC pfsysmemalloccode;
	#define GET_sysmemalloccode(fl)  s_pfCMGetAPI2( "sysmemalloccode", (RTS_VOID_FCTPTR *)&pfsysmemalloccode, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB0EDB0B3), 0x03051100)
	#define CAL_sysmemalloccode  pfsysmemalloccode
	#define CHK_sysmemalloccode  (pfsysmemalloccode != NULL)
	#define EXP_sysmemalloccode   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemalloccode", (RTS_UINTPTR)sysmemalloccode, 1, RTSITF_GET_SIGNATURE(0, 0xB0EDB0B3), 0x03051100) 
#endif


/**
 * Allocates data memory of the specified size
 * RETURN: Pointer to the memory block. 0 if no memory is not available
 */
typedef struct tagsysmemallocdata_struct
{
	RTS_IEC_STRING *szComponent;		/* VAR_INPUT */	/* Can be the library- or POU-name from IEC */
	RTS_IEC_XWORD udiSize;				/* VAR_INPUT */	/* Requested size of the memory block */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors_Itfs.library) */
	RTS_IEC_BYTE *SysMemAllocData;		/* VAR_OUTPUT */	
} sysmemallocdata_struct;

void CDECL CDECL_EXT sysmemallocdata(sysmemallocdata_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMALLOCDATA_IEC) (sysmemallocdata_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMALLOCDATA_NOTIMPLEMENTED)
	#define USE_sysmemallocdata
	#define EXT_sysmemallocdata
	#define GET_sysmemallocdata(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemallocdata(p0) 
	#define CHK_sysmemallocdata  FALSE
	#define EXP_sysmemallocdata  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemallocdata
	#define EXT_sysmemallocdata
	#define GET_sysmemallocdata(fl)  CAL_CMGETAPI( "sysmemallocdata" ) 
	#define CAL_sysmemallocdata  sysmemallocdata
	#define CHK_sysmemallocdata  TRUE
	#define EXP_sysmemallocdata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocdata", (RTS_UINTPTR)sysmemallocdata, 1, RTSITF_GET_SIGNATURE(0, 0xDF3F1AFF), 0x03051100) 
	#define EXTREF_ITF_sysmemallocdata {(RTS_VOID_FCTPTR)sysmemallocdata, "sysmemallocdata", RTSITF_GET_SIGNATURE(0, 0xDF3F1AFF), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemallocdata
	#define EXT_sysmemallocdata
	#define GET_sysmemallocdata(fl)  CAL_CMGETAPI( "sysmemallocdata" ) 
	#define CAL_sysmemallocdata  sysmemallocdata
	#define CHK_sysmemallocdata  TRUE
	#define EXP_sysmemallocdata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocdata", (RTS_UINTPTR)sysmemallocdata, 1, RTSITF_GET_SIGNATURE(0, 0xDF3F1AFF), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemallocdata
	#define EXT_SysMemsysmemallocdata
	#define GET_SysMemsysmemallocdata  ERR_OK
	#define CAL_SysMemsysmemallocdata  sysmemallocdata
	#define CHK_SysMemsysmemallocdata  TRUE
	#define EXP_SysMemsysmemallocdata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocdata", (RTS_UINTPTR)sysmemallocdata, 1, RTSITF_GET_SIGNATURE(0, 0xDF3F1AFF), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemallocdata
	#define EXT_sysmemallocdata
	#define GET_sysmemallocdata(fl)  CAL_CMGETAPI( "sysmemallocdata" ) 
	#define CAL_sysmemallocdata  sysmemallocdata
	#define CHK_sysmemallocdata  TRUE
	#define EXP_sysmemallocdata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocdata", (RTS_UINTPTR)sysmemallocdata, 1, RTSITF_GET_SIGNATURE(0, 0xDF3F1AFF), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemallocdata  PFSYSMEMALLOCDATA_IEC pfsysmemallocdata;
	#define EXT_sysmemallocdata  extern PFSYSMEMALLOCDATA_IEC pfsysmemallocdata;
	#define GET_sysmemallocdata(fl)  s_pfCMGetAPI2( "sysmemallocdata", (RTS_VOID_FCTPTR *)&pfsysmemallocdata, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xDF3F1AFF), 0x03051100)
	#define CAL_sysmemallocdata  pfsysmemallocdata
	#define CHK_sysmemallocdata  (pfsysmemallocdata != NULL)
	#define EXP_sysmemallocdata   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemallocdata", (RTS_UINTPTR)sysmemallocdata, 1, RTSITF_GET_SIGNATURE(0, 0xDF3F1AFF), 0x03051100) 
#endif


/**
 * Compares the content of two buffers
 * RETURN: Returns 0 if the buffer contents are equal, else !=0
 */
typedef struct tagsysmemcmp_struct
{
	RTS_IEC_BYTE *pBuffer1;				/* VAR_INPUT */	/* Address of first memory area (buffer 1) */
	RTS_IEC_BYTE *pBuffer2;				/* VAR_INPUT */	/* Address of second memory area (buffer 2) */
	RTS_IEC_XWORD udiCount;				/* VAR_INPUT */	/* Number of bytes to be compared */
	RTS_IEC_DINT SysMemCmp;				/* VAR_OUTPUT */	
} sysmemcmp_struct;

void CDECL CDECL_EXT sysmemcmp(sysmemcmp_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMCMP_IEC) (sysmemcmp_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMCMP_NOTIMPLEMENTED)
	#define USE_sysmemcmp
	#define EXT_sysmemcmp
	#define GET_sysmemcmp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemcmp(p0) 
	#define CHK_sysmemcmp  FALSE
	#define EXP_sysmemcmp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemcmp
	#define EXT_sysmemcmp
	#define GET_sysmemcmp(fl)  CAL_CMGETAPI( "sysmemcmp" ) 
	#define CAL_sysmemcmp  sysmemcmp
	#define CHK_sysmemcmp  TRUE
	#define EXP_sysmemcmp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcmp", (RTS_UINTPTR)sysmemcmp, 1, RTSITF_GET_SIGNATURE(0, 0x40048CD7), 0x03051100) 
	#define EXTREF_ITF_sysmemcmp {(RTS_VOID_FCTPTR)sysmemcmp, "sysmemcmp", RTSITF_GET_SIGNATURE(0, 0x40048CD7), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemcmp
	#define EXT_sysmemcmp
	#define GET_sysmemcmp(fl)  CAL_CMGETAPI( "sysmemcmp" ) 
	#define CAL_sysmemcmp  sysmemcmp
	#define CHK_sysmemcmp  TRUE
	#define EXP_sysmemcmp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcmp", (RTS_UINTPTR)sysmemcmp, 1, RTSITF_GET_SIGNATURE(0, 0x40048CD7), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemcmp
	#define EXT_SysMemsysmemcmp
	#define GET_SysMemsysmemcmp  ERR_OK
	#define CAL_SysMemsysmemcmp  sysmemcmp
	#define CHK_SysMemsysmemcmp  TRUE
	#define EXP_SysMemsysmemcmp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcmp", (RTS_UINTPTR)sysmemcmp, 1, RTSITF_GET_SIGNATURE(0, 0x40048CD7), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemcmp
	#define EXT_sysmemcmp
	#define GET_sysmemcmp(fl)  CAL_CMGETAPI( "sysmemcmp" ) 
	#define CAL_sysmemcmp  sysmemcmp
	#define CHK_sysmemcmp  TRUE
	#define EXP_sysmemcmp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcmp", (RTS_UINTPTR)sysmemcmp, 1, RTSITF_GET_SIGNATURE(0, 0x40048CD7), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemcmp  PFSYSMEMCMP_IEC pfsysmemcmp;
	#define EXT_sysmemcmp  extern PFSYSMEMCMP_IEC pfsysmemcmp;
	#define GET_sysmemcmp(fl)  s_pfCMGetAPI2( "sysmemcmp", (RTS_VOID_FCTPTR *)&pfsysmemcmp, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x40048CD7), 0x03051100)
	#define CAL_sysmemcmp  pfsysmemcmp
	#define CHK_sysmemcmp  (pfsysmemcmp != NULL)
	#define EXP_sysmemcmp   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcmp", (RTS_UINTPTR)sysmemcmp, 1, RTSITF_GET_SIGNATURE(0, 0x40048CD7), 0x03051100) 
#endif


/**
 * Copy the content from source (pSrc) to destination buffer (pDest)
 * RETURN: Pointer to the destination buffer, where the copy was done
 */
typedef struct tagsysmemcpy_struct
{
	RTS_IEC_BYTE *pDest;				/* VAR_INPUT */	/* Pointer to memory address to be copied to (target) */
	RTS_IEC_BYTE *pSrc;					/* VAR_INPUT */	/* Pointer to memory address to be copied from (source) */
	RTS_IEC_XWORD udiCount;				/* VAR_INPUT */	/* Number of bytes to be copied */
	RTS_IEC_BYTE *SysMemCpy;			/* VAR_OUTPUT */	
} sysmemcpy_struct;

void CDECL CDECL_EXT sysmemcpy(sysmemcpy_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMCPY_IEC) (sysmemcpy_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMCPY_NOTIMPLEMENTED)
	#define USE_sysmemcpy
	#define EXT_sysmemcpy
	#define GET_sysmemcpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemcpy(p0) 
	#define CHK_sysmemcpy  FALSE
	#define EXP_sysmemcpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemcpy
	#define EXT_sysmemcpy
	#define GET_sysmemcpy(fl)  CAL_CMGETAPI( "sysmemcpy" ) 
	#define CAL_sysmemcpy  sysmemcpy
	#define CHK_sysmemcpy  TRUE
	#define EXP_sysmemcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcpy", (RTS_UINTPTR)sysmemcpy, 1, RTSITF_GET_SIGNATURE(0, 0x4B2D0668), 0x03051100) 
	#define EXTREF_ITF_sysmemcpy {(RTS_VOID_FCTPTR)sysmemcpy, "sysmemcpy", RTSITF_GET_SIGNATURE(0, 0x4B2D0668), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemcpy
	#define EXT_sysmemcpy
	#define GET_sysmemcpy(fl)  CAL_CMGETAPI( "sysmemcpy" ) 
	#define CAL_sysmemcpy  sysmemcpy
	#define CHK_sysmemcpy  TRUE
	#define EXP_sysmemcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcpy", (RTS_UINTPTR)sysmemcpy, 1, RTSITF_GET_SIGNATURE(0, 0x4B2D0668), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemcpy
	#define EXT_SysMemsysmemcpy
	#define GET_SysMemsysmemcpy  ERR_OK
	#define CAL_SysMemsysmemcpy  sysmemcpy
	#define CHK_SysMemsysmemcpy  TRUE
	#define EXP_SysMemsysmemcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcpy", (RTS_UINTPTR)sysmemcpy, 1, RTSITF_GET_SIGNATURE(0, 0x4B2D0668), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemcpy
	#define EXT_sysmemcpy
	#define GET_sysmemcpy(fl)  CAL_CMGETAPI( "sysmemcpy" ) 
	#define CAL_sysmemcpy  sysmemcpy
	#define CHK_sysmemcpy  TRUE
	#define EXP_sysmemcpy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcpy", (RTS_UINTPTR)sysmemcpy, 1, RTSITF_GET_SIGNATURE(0, 0x4B2D0668), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemcpy  PFSYSMEMCPY_IEC pfsysmemcpy;
	#define EXT_sysmemcpy  extern PFSYSMEMCPY_IEC pfsysmemcpy;
	#define GET_sysmemcpy(fl)  s_pfCMGetAPI2( "sysmemcpy", (RTS_VOID_FCTPTR *)&pfsysmemcpy, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x4B2D0668), 0x03051100)
	#define CAL_sysmemcpy  pfsysmemcpy
	#define CHK_sysmemcpy  (pfsysmemcpy != NULL)
	#define EXP_sysmemcpy   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemcpy", (RTS_UINTPTR)sysmemcpy, 1, RTSITF_GET_SIGNATURE(0, 0x4B2D0668), 0x03051100) 
#endif


/**
 * Routine to force swapping memory independent of the byte-order of the system!
 * RETURN: Number of bytes swapped:
 *	+ -1 = failed (size too large)
 *	+ >0 = Number of bytes swapped
 */
typedef struct tagsysmemforceswap_struct
{
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* POINTER TO data TO swap. You can check, which order is selected by calling the routine with pbyBuffer=0 */
	RTS_IEC_UDINT udiSize;				/* VAR_INPUT */	/* Size of one element to swap */
	RTS_IEC_UDINT udiCount;				/* VAR_INPUT */	/* Number of elements to swap */
	RTS_IEC_DINT SysMemForceSwap;		/* VAR_OUTPUT */	
} sysmemforceswap_struct;

void CDECL CDECL_EXT sysmemforceswap(sysmemforceswap_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMFORCESWAP_IEC) (sysmemforceswap_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMFORCESWAP_NOTIMPLEMENTED)
	#define USE_sysmemforceswap
	#define EXT_sysmemforceswap
	#define GET_sysmemforceswap(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemforceswap(p0) 
	#define CHK_sysmemforceswap  FALSE
	#define EXP_sysmemforceswap  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemforceswap
	#define EXT_sysmemforceswap
	#define GET_sysmemforceswap(fl)  CAL_CMGETAPI( "sysmemforceswap" ) 
	#define CAL_sysmemforceswap  sysmemforceswap
	#define CHK_sysmemforceswap  TRUE
	#define EXP_sysmemforceswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemforceswap", (RTS_UINTPTR)sysmemforceswap, 1, RTSITF_GET_SIGNATURE(0, 0x1718FBB3), 0x03051100) 
	#define EXTREF_ITF_sysmemforceswap {(RTS_VOID_FCTPTR)sysmemforceswap, "sysmemforceswap", RTSITF_GET_SIGNATURE(0, 0x1718FBB3), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemforceswap
	#define EXT_sysmemforceswap
	#define GET_sysmemforceswap(fl)  CAL_CMGETAPI( "sysmemforceswap" ) 
	#define CAL_sysmemforceswap  sysmemforceswap
	#define CHK_sysmemforceswap  TRUE
	#define EXP_sysmemforceswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemforceswap", (RTS_UINTPTR)sysmemforceswap, 1, RTSITF_GET_SIGNATURE(0, 0x1718FBB3), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemforceswap
	#define EXT_SysMemsysmemforceswap
	#define GET_SysMemsysmemforceswap  ERR_OK
	#define CAL_SysMemsysmemforceswap  sysmemforceswap
	#define CHK_SysMemsysmemforceswap  TRUE
	#define EXP_SysMemsysmemforceswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemforceswap", (RTS_UINTPTR)sysmemforceswap, 1, RTSITF_GET_SIGNATURE(0, 0x1718FBB3), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemforceswap
	#define EXT_sysmemforceswap
	#define GET_sysmemforceswap(fl)  CAL_CMGETAPI( "sysmemforceswap" ) 
	#define CAL_sysmemforceswap  sysmemforceswap
	#define CHK_sysmemforceswap  TRUE
	#define EXP_sysmemforceswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemforceswap", (RTS_UINTPTR)sysmemforceswap, 1, RTSITF_GET_SIGNATURE(0, 0x1718FBB3), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemforceswap  PFSYSMEMFORCESWAP_IEC pfsysmemforceswap;
	#define EXT_sysmemforceswap  extern PFSYSMEMFORCESWAP_IEC pfsysmemforceswap;
	#define GET_sysmemforceswap(fl)  s_pfCMGetAPI2( "sysmemforceswap", (RTS_VOID_FCTPTR *)&pfsysmemforceswap, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x1718FBB3), 0x03051100)
	#define CAL_sysmemforceswap  pfsysmemforceswap
	#define CHK_sysmemforceswap  (pfsysmemforceswap != NULL)
	#define EXP_sysmemforceswap   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemforceswap", (RTS_UINTPTR)sysmemforceswap, 1, RTSITF_GET_SIGNATURE(0, 0x1718FBB3), 0x03051100) 
#endif


/**
 * Reserved function for the runtime system! Should not be used by an IEC application!
 * RETURN: Error code
 */
typedef struct tagsysmemfreearea_struct
{
	RTS_IEC_STRING *szComponent;		/* VAR_INPUT */	
	RTS_IEC_BYTE *pMemory;				/* VAR_INPUT */	
	RTS_IEC_RESULT SysMemFreeArea;		/* VAR_OUTPUT */	
} sysmemfreearea_struct;

void CDECL CDECL_EXT sysmemfreearea(sysmemfreearea_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMFREEAREA_IEC) (sysmemfreearea_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMFREEAREA_NOTIMPLEMENTED)
	#define USE_sysmemfreearea
	#define EXT_sysmemfreearea
	#define GET_sysmemfreearea(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemfreearea(p0) 
	#define CHK_sysmemfreearea  FALSE
	#define EXP_sysmemfreearea  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemfreearea
	#define EXT_sysmemfreearea
	#define GET_sysmemfreearea(fl)  CAL_CMGETAPI( "sysmemfreearea" ) 
	#define CAL_sysmemfreearea  sysmemfreearea
	#define CHK_sysmemfreearea  TRUE
	#define EXP_sysmemfreearea  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreearea", (RTS_UINTPTR)sysmemfreearea, 1, RTSITF_GET_SIGNATURE(0, 0x46CA745C), 0x03051100) 
	#define EXTREF_ITF_sysmemfreearea {(RTS_VOID_FCTPTR)sysmemfreearea, "sysmemfreearea", RTSITF_GET_SIGNATURE(0, 0x46CA745C), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemfreearea
	#define EXT_sysmemfreearea
	#define GET_sysmemfreearea(fl)  CAL_CMGETAPI( "sysmemfreearea" ) 
	#define CAL_sysmemfreearea  sysmemfreearea
	#define CHK_sysmemfreearea  TRUE
	#define EXP_sysmemfreearea  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreearea", (RTS_UINTPTR)sysmemfreearea, 1, RTSITF_GET_SIGNATURE(0, 0x46CA745C), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemfreearea
	#define EXT_SysMemsysmemfreearea
	#define GET_SysMemsysmemfreearea  ERR_OK
	#define CAL_SysMemsysmemfreearea  sysmemfreearea
	#define CHK_SysMemsysmemfreearea  TRUE
	#define EXP_SysMemsysmemfreearea  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreearea", (RTS_UINTPTR)sysmemfreearea, 1, RTSITF_GET_SIGNATURE(0, 0x46CA745C), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemfreearea
	#define EXT_sysmemfreearea
	#define GET_sysmemfreearea(fl)  CAL_CMGETAPI( "sysmemfreearea" ) 
	#define CAL_sysmemfreearea  sysmemfreearea
	#define CHK_sysmemfreearea  TRUE
	#define EXP_sysmemfreearea  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreearea", (RTS_UINTPTR)sysmemfreearea, 1, RTSITF_GET_SIGNATURE(0, 0x46CA745C), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemfreearea  PFSYSMEMFREEAREA_IEC pfsysmemfreearea;
	#define EXT_sysmemfreearea  extern PFSYSMEMFREEAREA_IEC pfsysmemfreearea;
	#define GET_sysmemfreearea(fl)  s_pfCMGetAPI2( "sysmemfreearea", (RTS_VOID_FCTPTR *)&pfsysmemfreearea, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x46CA745C), 0x03051100)
	#define CAL_sysmemfreearea  pfsysmemfreearea
	#define CHK_sysmemfreearea  (pfsysmemfreearea != NULL)
	#define EXP_sysmemfreearea   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreearea", (RTS_UINTPTR)sysmemfreearea, 1, RTSITF_GET_SIGNATURE(0, 0x46CA745C), 0x03051100) 
#endif


/**
 * Reserved function for the runtime system! Should not be used by an IEC application!
 * RETURN: Error code
 */
typedef struct tagsysmemfreecode_struct
{
	RTS_IEC_STRING *szComponent;		/* VAR_INPUT */	/* Component name */
	RTS_IEC_BYTE *pMemory;				/* VAR_INPUT */	/* POINTER TO code memory */
	RTS_IEC_RESULT SysMemFreeCode;		/* VAR_OUTPUT */	
} sysmemfreecode_struct;

void CDECL CDECL_EXT sysmemfreecode(sysmemfreecode_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMFREECODE_IEC) (sysmemfreecode_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMFREECODE_NOTIMPLEMENTED)
	#define USE_sysmemfreecode
	#define EXT_sysmemfreecode
	#define GET_sysmemfreecode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemfreecode(p0) 
	#define CHK_sysmemfreecode  FALSE
	#define EXP_sysmemfreecode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemfreecode
	#define EXT_sysmemfreecode
	#define GET_sysmemfreecode(fl)  CAL_CMGETAPI( "sysmemfreecode" ) 
	#define CAL_sysmemfreecode  sysmemfreecode
	#define CHK_sysmemfreecode  TRUE
	#define EXP_sysmemfreecode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreecode", (RTS_UINTPTR)sysmemfreecode, 1, RTSITF_GET_SIGNATURE(0, 0x92B98F40), 0x03051100) 
	#define EXTREF_ITF_sysmemfreecode {(RTS_VOID_FCTPTR)sysmemfreecode, "sysmemfreecode", RTSITF_GET_SIGNATURE(0, 0x92B98F40), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemfreecode
	#define EXT_sysmemfreecode
	#define GET_sysmemfreecode(fl)  CAL_CMGETAPI( "sysmemfreecode" ) 
	#define CAL_sysmemfreecode  sysmemfreecode
	#define CHK_sysmemfreecode  TRUE
	#define EXP_sysmemfreecode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreecode", (RTS_UINTPTR)sysmemfreecode, 1, RTSITF_GET_SIGNATURE(0, 0x92B98F40), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemfreecode
	#define EXT_SysMemsysmemfreecode
	#define GET_SysMemsysmemfreecode  ERR_OK
	#define CAL_SysMemsysmemfreecode  sysmemfreecode
	#define CHK_SysMemsysmemfreecode  TRUE
	#define EXP_SysMemsysmemfreecode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreecode", (RTS_UINTPTR)sysmemfreecode, 1, RTSITF_GET_SIGNATURE(0, 0x92B98F40), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemfreecode
	#define EXT_sysmemfreecode
	#define GET_sysmemfreecode(fl)  CAL_CMGETAPI( "sysmemfreecode" ) 
	#define CAL_sysmemfreecode  sysmemfreecode
	#define CHK_sysmemfreecode  TRUE
	#define EXP_sysmemfreecode  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreecode", (RTS_UINTPTR)sysmemfreecode, 1, RTSITF_GET_SIGNATURE(0, 0x92B98F40), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemfreecode  PFSYSMEMFREECODE_IEC pfsysmemfreecode;
	#define EXT_sysmemfreecode  extern PFSYSMEMFREECODE_IEC pfsysmemfreecode;
	#define GET_sysmemfreecode(fl)  s_pfCMGetAPI2( "sysmemfreecode", (RTS_VOID_FCTPTR *)&pfsysmemfreecode, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x92B98F40), 0x03051100)
	#define CAL_sysmemfreecode  pfsysmemfreecode
	#define CHK_sysmemfreecode  (pfsysmemfreecode != NULL)
	#define EXP_sysmemfreecode   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreecode", (RTS_UINTPTR)sysmemfreecode, 1, RTSITF_GET_SIGNATURE(0, 0x92B98F40), 0x03051100) 
#endif


/**
 * Release data memory
 * RETURN: Returns the runtime system error code (see CmpErrors_Itfs.library)
 */
typedef struct tagsysmemfreedata_struct
{
	RTS_IEC_STRING *szComponent;		/* VAR_INPUT */	/* Name of the component. Can be the library- or POU-name from IEC. */
	RTS_IEC_BYTE *pMemory;				/* VAR_INPUT */	/* Pointer to memory to be released */
	RTS_IEC_RESULT SysMemFreeData;		/* VAR_OUTPUT */	
} sysmemfreedata_struct;

void CDECL CDECL_EXT sysmemfreedata(sysmemfreedata_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMFREEDATA_IEC) (sysmemfreedata_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMFREEDATA_NOTIMPLEMENTED)
	#define USE_sysmemfreedata
	#define EXT_sysmemfreedata
	#define GET_sysmemfreedata(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemfreedata(p0) 
	#define CHK_sysmemfreedata  FALSE
	#define EXP_sysmemfreedata  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemfreedata
	#define EXT_sysmemfreedata
	#define GET_sysmemfreedata(fl)  CAL_CMGETAPI( "sysmemfreedata" ) 
	#define CAL_sysmemfreedata  sysmemfreedata
	#define CHK_sysmemfreedata  TRUE
	#define EXP_sysmemfreedata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreedata", (RTS_UINTPTR)sysmemfreedata, 1, RTSITF_GET_SIGNATURE(0, 0xFC9A307C), 0x03051100) 
	#define EXTREF_ITF_sysmemfreedata {(RTS_VOID_FCTPTR)sysmemfreedata, "sysmemfreedata", RTSITF_GET_SIGNATURE(0, 0xFC9A307C), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemfreedata
	#define EXT_sysmemfreedata
	#define GET_sysmemfreedata(fl)  CAL_CMGETAPI( "sysmemfreedata" ) 
	#define CAL_sysmemfreedata  sysmemfreedata
	#define CHK_sysmemfreedata  TRUE
	#define EXP_sysmemfreedata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreedata", (RTS_UINTPTR)sysmemfreedata, 1, RTSITF_GET_SIGNATURE(0, 0xFC9A307C), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemfreedata
	#define EXT_SysMemsysmemfreedata
	#define GET_SysMemsysmemfreedata  ERR_OK
	#define CAL_SysMemsysmemfreedata  sysmemfreedata
	#define CHK_SysMemsysmemfreedata  TRUE
	#define EXP_SysMemsysmemfreedata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreedata", (RTS_UINTPTR)sysmemfreedata, 1, RTSITF_GET_SIGNATURE(0, 0xFC9A307C), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemfreedata
	#define EXT_sysmemfreedata
	#define GET_sysmemfreedata(fl)  CAL_CMGETAPI( "sysmemfreedata" ) 
	#define CAL_sysmemfreedata  sysmemfreedata
	#define CHK_sysmemfreedata  TRUE
	#define EXP_sysmemfreedata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreedata", (RTS_UINTPTR)sysmemfreedata, 1, RTSITF_GET_SIGNATURE(0, 0xFC9A307C), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemfreedata  PFSYSMEMFREEDATA_IEC pfsysmemfreedata;
	#define EXT_sysmemfreedata  extern PFSYSMEMFREEDATA_IEC pfsysmemfreedata;
	#define GET_sysmemfreedata(fl)  s_pfCMGetAPI2( "sysmemfreedata", (RTS_VOID_FCTPTR *)&pfsysmemfreedata, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xFC9A307C), 0x03051100)
	#define CAL_sysmemfreedata  pfsysmemfreedata
	#define CHK_sysmemfreedata  (pfsysmemfreedata != NULL)
	#define EXP_sysmemfreedata   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemfreedata", (RTS_UINTPTR)sysmemfreedata, 1, RTSITF_GET_SIGNATURE(0, 0xFC9A307C), 0x03051100) 
#endif


/**
 * Routine to get total size of the currently allocated heap memory
 * RETURN: Size of the memory in bytes that is currently allocated from the heap
 */
typedef struct tagsysmemgetcurrentheapsize_struct
{
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors_Itfs.library) */
	RTS_IEC_XWORD SysMemGetCurrentHeapSize;	/* VAR_OUTPUT */
} sysmemgetcurrentheapsize_struct;

void CDECL CDECL_EXT sysmemgetcurrentheapsize(sysmemgetcurrentheapsize_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMGETCURRENTHEAPSIZE_IEC) (sysmemgetcurrentheapsize_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMGETCURRENTHEAPSIZE_NOTIMPLEMENTED)
	#define USE_sysmemgetcurrentheapsize
	#define EXT_sysmemgetcurrentheapsize
	#define GET_sysmemgetcurrentheapsize(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemgetcurrentheapsize(p0) 
	#define CHK_sysmemgetcurrentheapsize  FALSE
	#define EXP_sysmemgetcurrentheapsize  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemgetcurrentheapsize
	#define EXT_sysmemgetcurrentheapsize
	#define GET_sysmemgetcurrentheapsize(fl)  CAL_CMGETAPI( "sysmemgetcurrentheapsize" ) 
	#define CAL_sysmemgetcurrentheapsize  sysmemgetcurrentheapsize
	#define CHK_sysmemgetcurrentheapsize  TRUE
	#define EXP_sysmemgetcurrentheapsize  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemgetcurrentheapsize", (RTS_UINTPTR)sysmemgetcurrentheapsize, 1, RTSITF_GET_SIGNATURE(0xFA1C1A89, 0x55C8863E), 0x03051100) 
	#define EXTREF_ITF_sysmemgetcurrentheapsize {(RTS_VOID_FCTPTR)sysmemgetcurrentheapsize, "sysmemgetcurrentheapsize", RTSITF_GET_SIGNATURE(0xFA1C1A89, 0x55C8863E), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemgetcurrentheapsize
	#define EXT_sysmemgetcurrentheapsize
	#define GET_sysmemgetcurrentheapsize(fl)  CAL_CMGETAPI( "sysmemgetcurrentheapsize" ) 
	#define CAL_sysmemgetcurrentheapsize  sysmemgetcurrentheapsize
	#define CHK_sysmemgetcurrentheapsize  TRUE
	#define EXP_sysmemgetcurrentheapsize  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemgetcurrentheapsize", (RTS_UINTPTR)sysmemgetcurrentheapsize, 1, RTSITF_GET_SIGNATURE(0xFA1C1A89, 0x55C8863E), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemgetcurrentheapsize
	#define EXT_SysMemsysmemgetcurrentheapsize
	#define GET_SysMemsysmemgetcurrentheapsize  ERR_OK
	#define CAL_SysMemsysmemgetcurrentheapsize  sysmemgetcurrentheapsize
	#define CHK_SysMemsysmemgetcurrentheapsize  TRUE
	#define EXP_SysMemsysmemgetcurrentheapsize  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemgetcurrentheapsize", (RTS_UINTPTR)sysmemgetcurrentheapsize, 1, RTSITF_GET_SIGNATURE(0xFA1C1A89, 0x55C8863E), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemgetcurrentheapsize
	#define EXT_sysmemgetcurrentheapsize
	#define GET_sysmemgetcurrentheapsize(fl)  CAL_CMGETAPI( "sysmemgetcurrentheapsize" ) 
	#define CAL_sysmemgetcurrentheapsize  sysmemgetcurrentheapsize
	#define CHK_sysmemgetcurrentheapsize  TRUE
	#define EXP_sysmemgetcurrentheapsize  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemgetcurrentheapsize", (RTS_UINTPTR)sysmemgetcurrentheapsize, 1, RTSITF_GET_SIGNATURE(0xFA1C1A89, 0x55C8863E), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemgetcurrentheapsize  PFSYSMEMGETCURRENTHEAPSIZE_IEC pfsysmemgetcurrentheapsize;
	#define EXT_sysmemgetcurrentheapsize  extern PFSYSMEMGETCURRENTHEAPSIZE_IEC pfsysmemgetcurrentheapsize;
	#define GET_sysmemgetcurrentheapsize(fl)  s_pfCMGetAPI2( "sysmemgetcurrentheapsize", (RTS_VOID_FCTPTR *)&pfsysmemgetcurrentheapsize, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xFA1C1A89, 0x55C8863E), 0x03051100)
	#define CAL_sysmemgetcurrentheapsize  pfsysmemgetcurrentheapsize
	#define CHK_sysmemgetcurrentheapsize  (pfsysmemgetcurrentheapsize != NULL)
	#define EXP_sysmemgetcurrentheapsize   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemgetcurrentheapsize", (RTS_UINTPTR)sysmemgetcurrentheapsize, 1, RTSITF_GET_SIGNATURE(0xFA1C1A89, 0x55C8863E), 0x03051100) 
#endif


/**
 * Check if a pointer points to a valid address
 * RETURN: Returns the runtime system error code (see CmpErrors_Itfs.library):
 *	+ ERR_OK: Memory is valid
 *	+ ERR_FAILED: Memory is invalid. Cannot be accessed with the requested access mode bWrite
 */
typedef struct tagsysmemisvalidpointer_struct
{
	RTS_IEC_BYTE *ptr;					/* VAR_INPUT */	/* Pointer to the memory to be checked */
	RTS_IEC_XWORD udiSize;				/* VAR_INPUT */	/* Size of the memory to be checked */
	RTS_IEC_BOOL bWrite;				/* VAR_INPUT */	/* TRUE=Check, if memory can be written, FALSE=Check only for read access */
	RTS_IEC_RESULT SysMemIsValidPointer;	/* VAR_OUTPUT */	
} sysmemisvalidpointer_struct;

void CDECL CDECL_EXT sysmemisvalidpointer(sysmemisvalidpointer_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMISVALIDPOINTER_IEC) (sysmemisvalidpointer_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMISVALIDPOINTER_NOTIMPLEMENTED)
	#define USE_sysmemisvalidpointer
	#define EXT_sysmemisvalidpointer
	#define GET_sysmemisvalidpointer(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemisvalidpointer(p0) 
	#define CHK_sysmemisvalidpointer  FALSE
	#define EXP_sysmemisvalidpointer  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemisvalidpointer
	#define EXT_sysmemisvalidpointer
	#define GET_sysmemisvalidpointer(fl)  CAL_CMGETAPI( "sysmemisvalidpointer" ) 
	#define CAL_sysmemisvalidpointer  sysmemisvalidpointer
	#define CHK_sysmemisvalidpointer  TRUE
	#define EXP_sysmemisvalidpointer  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemisvalidpointer", (RTS_UINTPTR)sysmemisvalidpointer, 1, RTSITF_GET_SIGNATURE(0, 0x2B9D06D6), 0x03051100) 
	#define EXTREF_ITF_sysmemisvalidpointer {(RTS_VOID_FCTPTR)sysmemisvalidpointer, "sysmemisvalidpointer", RTSITF_GET_SIGNATURE(0, 0x2B9D06D6), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemisvalidpointer
	#define EXT_sysmemisvalidpointer
	#define GET_sysmemisvalidpointer(fl)  CAL_CMGETAPI( "sysmemisvalidpointer" ) 
	#define CAL_sysmemisvalidpointer  sysmemisvalidpointer
	#define CHK_sysmemisvalidpointer  TRUE
	#define EXP_sysmemisvalidpointer  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemisvalidpointer", (RTS_UINTPTR)sysmemisvalidpointer, 1, RTSITF_GET_SIGNATURE(0, 0x2B9D06D6), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemisvalidpointer
	#define EXT_SysMemsysmemisvalidpointer
	#define GET_SysMemsysmemisvalidpointer  ERR_OK
	#define CAL_SysMemsysmemisvalidpointer  sysmemisvalidpointer
	#define CHK_SysMemsysmemisvalidpointer  TRUE
	#define EXP_SysMemsysmemisvalidpointer  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemisvalidpointer", (RTS_UINTPTR)sysmemisvalidpointer, 1, RTSITF_GET_SIGNATURE(0, 0x2B9D06D6), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemisvalidpointer
	#define EXT_sysmemisvalidpointer
	#define GET_sysmemisvalidpointer(fl)  CAL_CMGETAPI( "sysmemisvalidpointer" ) 
	#define CAL_sysmemisvalidpointer  sysmemisvalidpointer
	#define CHK_sysmemisvalidpointer  TRUE
	#define EXP_sysmemisvalidpointer  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemisvalidpointer", (RTS_UINTPTR)sysmemisvalidpointer, 1, RTSITF_GET_SIGNATURE(0, 0x2B9D06D6), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemisvalidpointer  PFSYSMEMISVALIDPOINTER_IEC pfsysmemisvalidpointer;
	#define EXT_sysmemisvalidpointer  extern PFSYSMEMISVALIDPOINTER_IEC pfsysmemisvalidpointer;
	#define GET_sysmemisvalidpointer(fl)  s_pfCMGetAPI2( "sysmemisvalidpointer", (RTS_VOID_FCTPTR *)&pfsysmemisvalidpointer, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x2B9D06D6), 0x03051100)
	#define CAL_sysmemisvalidpointer  pfsysmemisvalidpointer
	#define CHK_sysmemisvalidpointer  (pfsysmemisvalidpointer != NULL)
	#define EXP_sysmemisvalidpointer   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemisvalidpointer", (RTS_UINTPTR)sysmemisvalidpointer, 1, RTSITF_GET_SIGNATURE(0, 0x2B9D06D6), 0x03051100) 
#endif


/**
 * Copy the content from source (pSrc) to destination buffer (pDest). This routine works for overlapping buffers too in opposite to SysMemCpy!
 * RETURN: Pointer to the destination buffer, where the move was done
 */
typedef struct tagsysmemmove_struct
{
	RTS_IEC_BYTE *pDest;				/* VAR_INPUT */	/* Pointer to memory address to be moved to (target) */
	RTS_IEC_BYTE *pSrc;					/* VAR_INPUT */	/* Pointer to memory address to be moved from (source) */
	RTS_IEC_XWORD udiCount;				/* VAR_INPUT */	/* Number of bytes to be moved */
	RTS_IEC_BYTE *SysMemMove;			/* VAR_OUTPUT */	
} sysmemmove_struct;

void CDECL CDECL_EXT sysmemmove(sysmemmove_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMMOVE_IEC) (sysmemmove_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMMOVE_NOTIMPLEMENTED)
	#define USE_sysmemmove
	#define EXT_sysmemmove
	#define GET_sysmemmove(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemmove(p0) 
	#define CHK_sysmemmove  FALSE
	#define EXP_sysmemmove  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemmove
	#define EXT_sysmemmove
	#define GET_sysmemmove(fl)  CAL_CMGETAPI( "sysmemmove" ) 
	#define CAL_sysmemmove  sysmemmove
	#define CHK_sysmemmove  TRUE
	#define EXP_sysmemmove  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemmove", (RTS_UINTPTR)sysmemmove, 1, RTSITF_GET_SIGNATURE(0, 0x9837ABBD), 0x03051100) 
	#define EXTREF_ITF_sysmemmove {(RTS_VOID_FCTPTR)sysmemmove, "sysmemmove", RTSITF_GET_SIGNATURE(0, 0x9837ABBD), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemmove
	#define EXT_sysmemmove
	#define GET_sysmemmove(fl)  CAL_CMGETAPI( "sysmemmove" ) 
	#define CAL_sysmemmove  sysmemmove
	#define CHK_sysmemmove  TRUE
	#define EXP_sysmemmove  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemmove", (RTS_UINTPTR)sysmemmove, 1, RTSITF_GET_SIGNATURE(0, 0x9837ABBD), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemmove
	#define EXT_SysMemsysmemmove
	#define GET_SysMemsysmemmove  ERR_OK
	#define CAL_SysMemsysmemmove  sysmemmove
	#define CHK_SysMemsysmemmove  TRUE
	#define EXP_SysMemsysmemmove  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemmove", (RTS_UINTPTR)sysmemmove, 1, RTSITF_GET_SIGNATURE(0, 0x9837ABBD), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemmove
	#define EXT_sysmemmove
	#define GET_sysmemmove(fl)  CAL_CMGETAPI( "sysmemmove" ) 
	#define CAL_sysmemmove  sysmemmove
	#define CHK_sysmemmove  TRUE
	#define EXP_sysmemmove  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemmove", (RTS_UINTPTR)sysmemmove, 1, RTSITF_GET_SIGNATURE(0, 0x9837ABBD), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemmove  PFSYSMEMMOVE_IEC pfsysmemmove;
	#define EXT_sysmemmove  extern PFSYSMEMMOVE_IEC pfsysmemmove;
	#define GET_sysmemmove(fl)  s_pfCMGetAPI2( "sysmemmove", (RTS_VOID_FCTPTR *)&pfsysmemmove, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x9837ABBD), 0x03051100)
	#define CAL_sysmemmove  pfsysmemmove
	#define CHK_sysmemmove  (pfsysmemmove != NULL)
	#define EXP_sysmemmove   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemmove", (RTS_UINTPTR)sysmemmove, 1, RTSITF_GET_SIGNATURE(0, 0x9837ABBD), 0x03051100) 
#endif


/**
 * Reallocate data memory with the specified size.
 * By use of this function the size of a memory block can be modified (reallocation). Therefor a pointer on the memory block to be
 * reallocated is passed to the function. The content of the memory block will not be modified. 
 * If the requested memory size cannot made available at the same place (address) , the function will allocate new memory space,
 * RETURN: Pointer to the memory block. 0 if no memory is not available
 */
typedef struct tagsysmemreallocdata_struct
{
	RTS_IEC_STRING *szComponent;		/* VAR_INPUT */	/* Name of the component. Can be the library- or POU-name from IEC. */
	RTS_IEC_BYTE *pMemory;				/* VAR_INPUT */	/* Pointer to memory to resize */
	RTS_IEC_XWORD udiSize;				/* VAR_INPUT */	/* Requested size of the memory */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors_Itfs.library) */
	RTS_IEC_BYTE *SysMemReallocData;	/* VAR_OUTPUT */	
} sysmemreallocdata_struct;

void CDECL CDECL_EXT sysmemreallocdata(sysmemreallocdata_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMREALLOCDATA_IEC) (sysmemreallocdata_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMREALLOCDATA_NOTIMPLEMENTED)
	#define USE_sysmemreallocdata
	#define EXT_sysmemreallocdata
	#define GET_sysmemreallocdata(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemreallocdata(p0) 
	#define CHK_sysmemreallocdata  FALSE
	#define EXP_sysmemreallocdata  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemreallocdata
	#define EXT_sysmemreallocdata
	#define GET_sysmemreallocdata(fl)  CAL_CMGETAPI( "sysmemreallocdata" ) 
	#define CAL_sysmemreallocdata  sysmemreallocdata
	#define CHK_sysmemreallocdata  TRUE
	#define EXP_sysmemreallocdata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemreallocdata", (RTS_UINTPTR)sysmemreallocdata, 1, RTSITF_GET_SIGNATURE(0, 0x574ABCC0), 0x03051100) 
	#define EXTREF_ITF_sysmemreallocdata {(RTS_VOID_FCTPTR)sysmemreallocdata, "sysmemreallocdata", RTSITF_GET_SIGNATURE(0, 0x574ABCC0), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemreallocdata
	#define EXT_sysmemreallocdata
	#define GET_sysmemreallocdata(fl)  CAL_CMGETAPI( "sysmemreallocdata" ) 
	#define CAL_sysmemreallocdata  sysmemreallocdata
	#define CHK_sysmemreallocdata  TRUE
	#define EXP_sysmemreallocdata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemreallocdata", (RTS_UINTPTR)sysmemreallocdata, 1, RTSITF_GET_SIGNATURE(0, 0x574ABCC0), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemreallocdata
	#define EXT_SysMemsysmemreallocdata
	#define GET_SysMemsysmemreallocdata  ERR_OK
	#define CAL_SysMemsysmemreallocdata  sysmemreallocdata
	#define CHK_SysMemsysmemreallocdata  TRUE
	#define EXP_SysMemsysmemreallocdata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemreallocdata", (RTS_UINTPTR)sysmemreallocdata, 1, RTSITF_GET_SIGNATURE(0, 0x574ABCC0), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemreallocdata
	#define EXT_sysmemreallocdata
	#define GET_sysmemreallocdata(fl)  CAL_CMGETAPI( "sysmemreallocdata" ) 
	#define CAL_sysmemreallocdata  sysmemreallocdata
	#define CHK_sysmemreallocdata  TRUE
	#define EXP_sysmemreallocdata  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemreallocdata", (RTS_UINTPTR)sysmemreallocdata, 1, RTSITF_GET_SIGNATURE(0, 0x574ABCC0), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemreallocdata  PFSYSMEMREALLOCDATA_IEC pfsysmemreallocdata;
	#define EXT_sysmemreallocdata  extern PFSYSMEMREALLOCDATA_IEC pfsysmemreallocdata;
	#define GET_sysmemreallocdata(fl)  s_pfCMGetAPI2( "sysmemreallocdata", (RTS_VOID_FCTPTR *)&pfsysmemreallocdata, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x574ABCC0), 0x03051100)
	#define CAL_sysmemreallocdata  pfsysmemreallocdata
	#define CHK_sysmemreallocdata  (pfsysmemreallocdata != NULL)
	#define EXP_sysmemreallocdata   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemreallocdata", (RTS_UINTPTR)sysmemreallocdata, 1, RTSITF_GET_SIGNATURE(0, 0x574ABCC0), 0x03051100) 
#endif


/**
 * By use of this function a memory space is initialized with a specified value
 * RETURN: Pointer to the memory block which was initialized. 0 if the operation failed
 */
typedef struct tagsysmemset_struct
{
	RTS_IEC_BYTE *pDest;				/* VAR_INPUT */	/* Pointer to memory block to initialize */
	RTS_IEC_UDINT udiValue;				/* VAR_INPUT */	/* Value with which the memory is to be initialized */
	RTS_IEC_XWORD udiCount;				/* VAR_INPUT */	/* Number of bytes to be initialized in the memory block */
	RTS_IEC_BYTE *SysMemSet;			/* VAR_OUTPUT */	
} sysmemset_struct;

void CDECL CDECL_EXT sysmemset(sysmemset_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMSET_IEC) (sysmemset_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMSET_NOTIMPLEMENTED)
	#define USE_sysmemset
	#define EXT_sysmemset
	#define GET_sysmemset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemset(p0) 
	#define CHK_sysmemset  FALSE
	#define EXP_sysmemset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemset
	#define EXT_sysmemset
	#define GET_sysmemset(fl)  CAL_CMGETAPI( "sysmemset" ) 
	#define CAL_sysmemset  sysmemset
	#define CHK_sysmemset  TRUE
	#define EXP_sysmemset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemset", (RTS_UINTPTR)sysmemset, 1, RTSITF_GET_SIGNATURE(0, 0x663CE5DB), 0x03051100) 
	#define EXTREF_ITF_sysmemset {(RTS_VOID_FCTPTR)sysmemset, "sysmemset", RTSITF_GET_SIGNATURE(0, 0x663CE5DB), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemset
	#define EXT_sysmemset
	#define GET_sysmemset(fl)  CAL_CMGETAPI( "sysmemset" ) 
	#define CAL_sysmemset  sysmemset
	#define CHK_sysmemset  TRUE
	#define EXP_sysmemset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemset", (RTS_UINTPTR)sysmemset, 1, RTSITF_GET_SIGNATURE(0, 0x663CE5DB), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemset
	#define EXT_SysMemsysmemset
	#define GET_SysMemsysmemset  ERR_OK
	#define CAL_SysMemsysmemset  sysmemset
	#define CHK_SysMemsysmemset  TRUE
	#define EXP_SysMemsysmemset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemset", (RTS_UINTPTR)sysmemset, 1, RTSITF_GET_SIGNATURE(0, 0x663CE5DB), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemset
	#define EXT_sysmemset
	#define GET_sysmemset(fl)  CAL_CMGETAPI( "sysmemset" ) 
	#define CAL_sysmemset  sysmemset
	#define CHK_sysmemset  TRUE
	#define EXP_sysmemset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemset", (RTS_UINTPTR)sysmemset, 1, RTSITF_GET_SIGNATURE(0, 0x663CE5DB), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemset  PFSYSMEMSET_IEC pfsysmemset;
	#define EXT_sysmemset  extern PFSYSMEMSET_IEC pfsysmemset;
	#define GET_sysmemset(fl)  s_pfCMGetAPI2( "sysmemset", (RTS_VOID_FCTPTR *)&pfsysmemset, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x663CE5DB), 0x03051100)
	#define CAL_sysmemset  pfsysmemset
	#define CHK_sysmemset  (pfsysmemset != NULL)
	#define EXP_sysmemset   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemset", (RTS_UINTPTR)sysmemset, 1, RTSITF_GET_SIGNATURE(0, 0x663CE5DB), 0x03051100) 
#endif


/**
 * Routine to swap memory into little endian. If little endian (Intel) byte-order is received and platform has
 * big endian (Motorola) byte-order. On little endian byte-order platforms, routine does nothing
 * RETURN: Number of bytes swapped:
 *	+ -1 = failed (iSize too large)
 *	+ 0 = no swapping necessary (little endian byte-order)
 *	+ >0 = Number of bytes swapped (big endian byte-order)
 */
typedef struct tagsysmemswap_struct
{
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Pointer to data to swap. You can check, which order is selected by calling the routine with pbyBuffer=0 */
	RTS_IEC_UDINT udiSize;				/* VAR_INPUT */	/* Size of one element to swap */
	RTS_IEC_UDINT udiCount;				/* VAR_INPUT */	/* Number of elements to swap */
	RTS_IEC_DINT SysMemSwap;			/* VAR_OUTPUT */	
} sysmemswap_struct;

void CDECL CDECL_EXT sysmemswap(sysmemswap_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSMEMSWAP_IEC) (sysmemswap_struct *p);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMSWAP_NOTIMPLEMENTED)
	#define USE_sysmemswap
	#define EXT_sysmemswap
	#define GET_sysmemswap(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysmemswap(p0) 
	#define CHK_sysmemswap  FALSE
	#define EXP_sysmemswap  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysmemswap
	#define EXT_sysmemswap
	#define GET_sysmemswap(fl)  CAL_CMGETAPI( "sysmemswap" ) 
	#define CAL_sysmemswap  sysmemswap
	#define CHK_sysmemswap  TRUE
	#define EXP_sysmemswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemswap", (RTS_UINTPTR)sysmemswap, 1, RTSITF_GET_SIGNATURE(0, 0x77D02AA2), 0x03051100) 
	#define EXTREF_ITF_sysmemswap {(RTS_VOID_FCTPTR)sysmemswap, "sysmemswap", RTSITF_GET_SIGNATURE(0, 0x77D02AA2), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_sysmemswap
	#define EXT_sysmemswap
	#define GET_sysmemswap(fl)  CAL_CMGETAPI( "sysmemswap" ) 
	#define CAL_sysmemswap  sysmemswap
	#define CHK_sysmemswap  TRUE
	#define EXP_sysmemswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemswap", (RTS_UINTPTR)sysmemswap, 1, RTSITF_GET_SIGNATURE(0, 0x77D02AA2), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemsysmemswap
	#define EXT_SysMemsysmemswap
	#define GET_SysMemsysmemswap  ERR_OK
	#define CAL_SysMemsysmemswap  sysmemswap
	#define CHK_SysMemsysmemswap  TRUE
	#define EXP_SysMemsysmemswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemswap", (RTS_UINTPTR)sysmemswap, 1, RTSITF_GET_SIGNATURE(0, 0x77D02AA2), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysmemswap
	#define EXT_sysmemswap
	#define GET_sysmemswap(fl)  CAL_CMGETAPI( "sysmemswap" ) 
	#define CAL_sysmemswap  sysmemswap
	#define CHK_sysmemswap  TRUE
	#define EXP_sysmemswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemswap", (RTS_UINTPTR)sysmemswap, 1, RTSITF_GET_SIGNATURE(0, 0x77D02AA2), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysmemswap  PFSYSMEMSWAP_IEC pfsysmemswap;
	#define EXT_sysmemswap  extern PFSYSMEMSWAP_IEC pfsysmemswap;
	#define GET_sysmemswap(fl)  s_pfCMGetAPI2( "sysmemswap", (RTS_VOID_FCTPTR *)&pfsysmemswap, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x77D02AA2), 0x03051100)
	#define CAL_sysmemswap  pfsysmemswap
	#define CHK_sysmemswap  (pfsysmemswap != NULL)
	#define EXP_sysmemswap   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysmemswap", (RTS_UINTPTR)sysmemswap, 1, RTSITF_GET_SIGNATURE(0, 0x77D02AA2), 0x03051100) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/********** Functions **********/

/**
 * <description>Initialization routines for OS specific modules</description>
 * <param name="pInit" type="IN">Initialization structure for OS specific modules</param>
 * <result>Result of initialization.</result>
 */
RTS_RESULT CDECL SysMemOSInit(INIT_STRUCT *pInit);
/**
 * <description>Hook Function for OS specific modules</description>
 * <param name="ulHook" type="IN">Hook Type for OS specific modules</param>
 * <param name="ulParam1" type="IN">First Hook Parameter for OS specific modules</param>
 * <param name="ulParam2" type="IN">Second Hook Parameter for OS specific modules</param>
 * <result>Result of OS Hook.</result>
 */
RTS_RESULT CDECL SysMemOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>Allocates data memory of the specified size.
 * IMPLEMENTATION NOTE:
 * New allocated memory must be initialized with 0!
 * </description>
 * <param name="pszComponentName" type="IN">Name of the component</param>
 * <param name="ulSize" type="IN">Requested size of the memory</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Memory could be allocated</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">If memory could not be allocated</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_OUT_OF_LIMITS">Memory could not be allocated because memory limit of the component has been reached</errorcode>
 * <result>Pointer to the memory block. NULL if no memory is available.</result>
 */
void* CDECL SysMemAllocData(char *pszComponentName, RTS_SIZE ulSize, RTS_RESULT *pResult);
typedef void* (CDECL * PFSYSMEMALLOCDATA) (char *pszComponentName, RTS_SIZE ulSize, RTS_RESULT *pResult);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMALLOCDATA_NOTIMPLEMENTED)
	#define USE_SysMemAllocData
	#define EXT_SysMemAllocData
	#define GET_SysMemAllocData(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemAllocData(p0,p1,p2)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_SysMemAllocData  FALSE
	#define EXP_SysMemAllocData  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemAllocData
	#define EXT_SysMemAllocData
	#define GET_SysMemAllocData(fl)  CAL_CMGETAPI( "SysMemAllocData" ) 
	#define CAL_SysMemAllocData  SysMemAllocData
	#define CHK_SysMemAllocData  TRUE
	#define EXP_SysMemAllocData  CAL_CMEXPAPI( "SysMemAllocData" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemAllocData
	#define EXT_SysMemAllocData
	#define GET_SysMemAllocData(fl)  CAL_CMGETAPI( "SysMemAllocData" ) 
	#define CAL_SysMemAllocData  SysMemAllocData
	#define CHK_SysMemAllocData  TRUE
	#define EXP_SysMemAllocData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemAllocData", (RTS_UINTPTR)SysMemAllocData, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemAllocData
	#define EXT_SysMemSysMemAllocData
	#define GET_SysMemSysMemAllocData  ERR_OK
	#define CAL_SysMemSysMemAllocData pISysMem->ISysMemAllocData
	#define CHK_SysMemSysMemAllocData (pISysMem != NULL)
	#define EXP_SysMemSysMemAllocData  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemAllocData
	#define EXT_SysMemAllocData
	#define GET_SysMemAllocData(fl)  CAL_CMGETAPI( "SysMemAllocData" ) 
	#define CAL_SysMemAllocData pISysMem->ISysMemAllocData
	#define CHK_SysMemAllocData (pISysMem != NULL)
	#define EXP_SysMemAllocData  CAL_CMEXPAPI( "SysMemAllocData" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemAllocData  PFSYSMEMALLOCDATA pfSysMemAllocData;
	#define EXT_SysMemAllocData  extern PFSYSMEMALLOCDATA pfSysMemAllocData;
	#define GET_SysMemAllocData(fl)  s_pfCMGetAPI2( "SysMemAllocData", (RTS_VOID_FCTPTR *)&pfSysMemAllocData, (fl), 0, 0)
	#define CAL_SysMemAllocData  pfSysMemAllocData
	#define CHK_SysMemAllocData  (pfSysMemAllocData != NULL)
	#define EXP_SysMemAllocData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemAllocData", (RTS_UINTPTR)SysMemAllocData, 0, 0) 
#endif




/**
 * <description>Reallocate data memory with the specified size</description>
 * <param name="pszComponentName" type="IN">Name of the component</param>
 * <param name="pData" type="IN">Pointer to memory to resize</param>
 * <param name="ulSize" type="IN">Requested size of the memory</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to the memory block. NULL if no memory is available.</result>
 */
void* CDECL SysMemReallocData(char *pszComponentName, void* pData, RTS_SIZE ulSize, RTS_RESULT *pResult);
typedef void* (CDECL * PFSYSMEMREALLOCDATA) (char *pszComponentName, void* pData, RTS_SIZE ulSize, RTS_RESULT *pResult);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMREALLOCDATA_NOTIMPLEMENTED)
	#define USE_SysMemReallocData
	#define EXT_SysMemReallocData
	#define GET_SysMemReallocData(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemReallocData(p0,p1,p2,p3)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_SysMemReallocData  FALSE
	#define EXP_SysMemReallocData  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemReallocData
	#define EXT_SysMemReallocData
	#define GET_SysMemReallocData(fl)  CAL_CMGETAPI( "SysMemReallocData" ) 
	#define CAL_SysMemReallocData  SysMemReallocData
	#define CHK_SysMemReallocData  TRUE
	#define EXP_SysMemReallocData  CAL_CMEXPAPI( "SysMemReallocData" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemReallocData
	#define EXT_SysMemReallocData
	#define GET_SysMemReallocData(fl)  CAL_CMGETAPI( "SysMemReallocData" ) 
	#define CAL_SysMemReallocData  SysMemReallocData
	#define CHK_SysMemReallocData  TRUE
	#define EXP_SysMemReallocData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemReallocData", (RTS_UINTPTR)SysMemReallocData, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemReallocData
	#define EXT_SysMemSysMemReallocData
	#define GET_SysMemSysMemReallocData  ERR_OK
	#define CAL_SysMemSysMemReallocData pISysMem->ISysMemReallocData
	#define CHK_SysMemSysMemReallocData (pISysMem != NULL)
	#define EXP_SysMemSysMemReallocData  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemReallocData
	#define EXT_SysMemReallocData
	#define GET_SysMemReallocData(fl)  CAL_CMGETAPI( "SysMemReallocData" ) 
	#define CAL_SysMemReallocData pISysMem->ISysMemReallocData
	#define CHK_SysMemReallocData (pISysMem != NULL)
	#define EXP_SysMemReallocData  CAL_CMEXPAPI( "SysMemReallocData" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemReallocData  PFSYSMEMREALLOCDATA pfSysMemReallocData;
	#define EXT_SysMemReallocData  extern PFSYSMEMREALLOCDATA pfSysMemReallocData;
	#define GET_SysMemReallocData(fl)  s_pfCMGetAPI2( "SysMemReallocData", (RTS_VOID_FCTPTR *)&pfSysMemReallocData, (fl), 0, 0)
	#define CAL_SysMemReallocData  pfSysMemReallocData
	#define CHK_SysMemReallocData  (pfSysMemReallocData != NULL)
	#define EXP_SysMemReallocData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemReallocData", (RTS_UINTPTR)SysMemReallocData, 0, 0) 
#endif




/**
 * <description>Release data memory</description>
 * <param name="pszComponentName" type="IN">Name of the component</param>
 * <param name="pData" type="IN">Pointer to memory to be released. If pointer is NULL, ERR_PARAMETER is returned.</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Memory could be released</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Memory pointer is NULL</errorcode>
 */
RTS_RESULT CDECL SysMemFreeData(char *pszComponentName, void* pData);
typedef RTS_RESULT (CDECL * PFSYSMEMFREEDATA) (char *pszComponentName, void* pData);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMFREEDATA_NOTIMPLEMENTED)
	#define USE_SysMemFreeData
	#define EXT_SysMemFreeData
	#define GET_SysMemFreeData(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemFreeData(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysMemFreeData  FALSE
	#define EXP_SysMemFreeData  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemFreeData
	#define EXT_SysMemFreeData
	#define GET_SysMemFreeData(fl)  CAL_CMGETAPI( "SysMemFreeData" ) 
	#define CAL_SysMemFreeData  SysMemFreeData
	#define CHK_SysMemFreeData  TRUE
	#define EXP_SysMemFreeData  CAL_CMEXPAPI( "SysMemFreeData" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemFreeData
	#define EXT_SysMemFreeData
	#define GET_SysMemFreeData(fl)  CAL_CMGETAPI( "SysMemFreeData" ) 
	#define CAL_SysMemFreeData  SysMemFreeData
	#define CHK_SysMemFreeData  TRUE
	#define EXP_SysMemFreeData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemFreeData", (RTS_UINTPTR)SysMemFreeData, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemFreeData
	#define EXT_SysMemSysMemFreeData
	#define GET_SysMemSysMemFreeData  ERR_OK
	#define CAL_SysMemSysMemFreeData pISysMem->ISysMemFreeData
	#define CHK_SysMemSysMemFreeData (pISysMem != NULL)
	#define EXP_SysMemSysMemFreeData  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemFreeData
	#define EXT_SysMemFreeData
	#define GET_SysMemFreeData(fl)  CAL_CMGETAPI( "SysMemFreeData" ) 
	#define CAL_SysMemFreeData pISysMem->ISysMemFreeData
	#define CHK_SysMemFreeData (pISysMem != NULL)
	#define EXP_SysMemFreeData  CAL_CMEXPAPI( "SysMemFreeData" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemFreeData  PFSYSMEMFREEDATA pfSysMemFreeData;
	#define EXT_SysMemFreeData  extern PFSYSMEMFREEDATA pfSysMemFreeData;
	#define GET_SysMemFreeData(fl)  s_pfCMGetAPI2( "SysMemFreeData", (RTS_VOID_FCTPTR *)&pfSysMemFreeData, (fl), 0, 0)
	#define CAL_SysMemFreeData  pfSysMemFreeData
	#define CHK_SysMemFreeData  (pfSysMemFreeData != NULL)
	#define EXP_SysMemFreeData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemFreeData", (RTS_UINTPTR)SysMemFreeData, 0, 0) 
#endif




/**
 * <description>
 *	Allocates any kind of memory area for an IEC application. See category "Area Types" for detailed description!
 *	Can be used to set an application area to a still existing memory location.
 *
 *  IMPLEMENTATION NOTE:
 *  - New allocated memory must be initialized with 0, but _not_ the retain memory!
 *  - Memory area must be aligned specified by SYSMEM_IECAREA_BYTE_ALIGNMENT! So the pointer to the area must be correctly aligned before returning the pointer or
 *	  you have to use an appropriate operating system API function to get an aligned heap memory block from the memory manager!
 *
 * </description>
 * <param name="pszComponentName" type="IN" range="[NULL,VALID_COMPONENT_NAME]">Name of the component</param>
 * <param name="usType" type="IN" range="[DA_NONE,DA_CODE,DA_CONSTANT,DA_DATA,DA_RETAIN,INVALID_DA_AREA]">Type of the area (see category "Area Types")</param>
 * <param name="ulSize" type="IN" range="[0,VALID_SIZE,RTS_SIZE_MAX]">Requested size of the memory</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="pResult" type="ERR_OK">Memory Area could be allocated</errorcode>
 * <errorcode name="pResult" type="ERR_FAILED">The requested Area could not be allocated - invalid combination of DA-Flags, or all areas of this type are already in use</errorcode>
 * <errorcode name="pResult" type="ERR_NOT_SUPPORTED">Allocation is not supported, it might be allocated dynamically</errorcode>
 * <errorcode name="pResult" type="ERR_OUT_OF_LIMITS">The requested area size was too large or too small</errorcode>
 * <errorcode name="pResult" type="ERR_APP_DONT_INIT_AREA">The returned area must not be initialized outside, because the content is handled inside this function!</errorcode>
 * <result>Pointer to the memory block. NULL if no memory is available (allocate area on heap).</result>
 */
void* CDECL SysMemAllocArea(char *pszComponentName, unsigned short usType, RTS_SIZE ulSize, RTS_RESULT *pResult);
typedef void* (CDECL * PFSYSMEMALLOCAREA) (char *pszComponentName, unsigned short usType, RTS_SIZE ulSize, RTS_RESULT *pResult);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMALLOCAREA_NOTIMPLEMENTED)
	#define USE_SysMemAllocArea
	#define EXT_SysMemAllocArea
	#define GET_SysMemAllocArea(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemAllocArea(p0,p1,p2,p3)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_SysMemAllocArea  FALSE
	#define EXP_SysMemAllocArea  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemAllocArea
	#define EXT_SysMemAllocArea
	#define GET_SysMemAllocArea(fl)  CAL_CMGETAPI( "SysMemAllocArea" ) 
	#define CAL_SysMemAllocArea  SysMemAllocArea
	#define CHK_SysMemAllocArea  TRUE
	#define EXP_SysMemAllocArea  CAL_CMEXPAPI( "SysMemAllocArea" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemAllocArea
	#define EXT_SysMemAllocArea
	#define GET_SysMemAllocArea(fl)  CAL_CMGETAPI( "SysMemAllocArea" ) 
	#define CAL_SysMemAllocArea  SysMemAllocArea
	#define CHK_SysMemAllocArea  TRUE
	#define EXP_SysMemAllocArea  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemAllocArea", (RTS_UINTPTR)SysMemAllocArea, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemAllocArea
	#define EXT_SysMemSysMemAllocArea
	#define GET_SysMemSysMemAllocArea  ERR_OK
	#define CAL_SysMemSysMemAllocArea pISysMem->ISysMemAllocArea
	#define CHK_SysMemSysMemAllocArea (pISysMem != NULL)
	#define EXP_SysMemSysMemAllocArea  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemAllocArea
	#define EXT_SysMemAllocArea
	#define GET_SysMemAllocArea(fl)  CAL_CMGETAPI( "SysMemAllocArea" ) 
	#define CAL_SysMemAllocArea pISysMem->ISysMemAllocArea
	#define CHK_SysMemAllocArea (pISysMem != NULL)
	#define EXP_SysMemAllocArea  CAL_CMEXPAPI( "SysMemAllocArea" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemAllocArea  PFSYSMEMALLOCAREA pfSysMemAllocArea;
	#define EXT_SysMemAllocArea  extern PFSYSMEMALLOCAREA pfSysMemAllocArea;
	#define GET_SysMemAllocArea(fl)  s_pfCMGetAPI2( "SysMemAllocArea", (RTS_VOID_FCTPTR *)&pfSysMemAllocArea, (fl), 0, 0)
	#define CAL_SysMemAllocArea  pfSysMemAllocArea
	#define CHK_SysMemAllocArea  (pfSysMemAllocArea != NULL)
	#define EXP_SysMemAllocArea  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemAllocArea", (RTS_UINTPTR)SysMemAllocArea, 0, 0) 
#endif




/**
 * <description>
 * <p>Release data area of an application.</p>
 * <p>Note: On SIL2 runtime this function may generate an exception, when called in SAFE-Mode.</p>
 * </description>
 * <param name="pszComponentName" type="IN" range="[NULL,VALID_COMPONENT_NAME]">Name of the component</param>
 * <param name="pData" type="IN" range="[NULL,VALID_AREA_POINTER,INVALID_AREA_POINTER]">Pointer to area to free</param>
 * <parampseudo name="OperationMode" type="IN" range="[RTS_SIL2_OPMODE_SAFE,RTS_SIL2_OPMODE_DEBUG]">Specifies the current operation mode of the PLC</parampseudo>
 * <parampseudo name="bExceptionGenerated" type="OUT" range="[TRUE,FALSE]">Specifies, if an exception occurred, or not</parampseudo>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Memory Area could be freed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">The memory area could not be freed (wrong area pointer, or area is not allocated)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Parameter pData was NULL</errorcode>
 */
RTS_RESULT CDECL SysMemFreeArea(char *pszComponentName, void* pData);
typedef RTS_RESULT (CDECL * PFSYSMEMFREEAREA) (char *pszComponentName, void* pData);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMFREEAREA_NOTIMPLEMENTED)
	#define USE_SysMemFreeArea
	#define EXT_SysMemFreeArea
	#define GET_SysMemFreeArea(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemFreeArea(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysMemFreeArea  FALSE
	#define EXP_SysMemFreeArea  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemFreeArea
	#define EXT_SysMemFreeArea
	#define GET_SysMemFreeArea(fl)  CAL_CMGETAPI( "SysMemFreeArea" ) 
	#define CAL_SysMemFreeArea  SysMemFreeArea
	#define CHK_SysMemFreeArea  TRUE
	#define EXP_SysMemFreeArea  CAL_CMEXPAPI( "SysMemFreeArea" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemFreeArea
	#define EXT_SysMemFreeArea
	#define GET_SysMemFreeArea(fl)  CAL_CMGETAPI( "SysMemFreeArea" ) 
	#define CAL_SysMemFreeArea  SysMemFreeArea
	#define CHK_SysMemFreeArea  TRUE
	#define EXP_SysMemFreeArea  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemFreeArea", (RTS_UINTPTR)SysMemFreeArea, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemFreeArea
	#define EXT_SysMemSysMemFreeArea
	#define GET_SysMemSysMemFreeArea  ERR_OK
	#define CAL_SysMemSysMemFreeArea pISysMem->ISysMemFreeArea
	#define CHK_SysMemSysMemFreeArea (pISysMem != NULL)
	#define EXP_SysMemSysMemFreeArea  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemFreeArea
	#define EXT_SysMemFreeArea
	#define GET_SysMemFreeArea(fl)  CAL_CMGETAPI( "SysMemFreeArea" ) 
	#define CAL_SysMemFreeArea pISysMem->ISysMemFreeArea
	#define CHK_SysMemFreeArea (pISysMem != NULL)
	#define EXP_SysMemFreeArea  CAL_CMEXPAPI( "SysMemFreeArea" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemFreeArea  PFSYSMEMFREEAREA pfSysMemFreeArea;
	#define EXT_SysMemFreeArea  extern PFSYSMEMFREEAREA pfSysMemFreeArea;
	#define GET_SysMemFreeArea(fl)  s_pfCMGetAPI2( "SysMemFreeArea", (RTS_VOID_FCTPTR *)&pfSysMemFreeArea, (fl), 0, 0)
	#define CAL_SysMemFreeArea  pfSysMemFreeArea
	#define CHK_SysMemFreeArea  (pfSysMemFreeArea != NULL)
	#define EXP_SysMemFreeArea  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemFreeArea", (RTS_UINTPTR)SysMemFreeArea, 0, 0) 
#endif




/**
 * <description>
 *	Allocate code memory with the specified size (in the memory, code can be executed).
 *	NOTE: This routine can be used on architectures, where standard data memory is protected
 *	against code execution!
 * IMPLEMENTATION NOTE:
 * New allocated memory must be initialized with 0!
 * </description>
 * <param name="pszComponentName" type="IN">Name of the component</param>
 * <param name="ulSize" type="IN">Requested size of the memory</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to the memory block. NULL if no memory is available.</result>
 */
void* CDECL SysMemAllocCode(char *pszComponentName, RTS_SIZE ulSize, RTS_RESULT *pResult);
typedef void* (CDECL * PFSYSMEMALLOCCODE) (char *pszComponentName, RTS_SIZE ulSize, RTS_RESULT *pResult);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMALLOCCODE_NOTIMPLEMENTED)
	#define USE_SysMemAllocCode
	#define EXT_SysMemAllocCode
	#define GET_SysMemAllocCode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemAllocCode(p0,p1,p2)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_SysMemAllocCode  FALSE
	#define EXP_SysMemAllocCode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemAllocCode
	#define EXT_SysMemAllocCode
	#define GET_SysMemAllocCode(fl)  CAL_CMGETAPI( "SysMemAllocCode" ) 
	#define CAL_SysMemAllocCode  SysMemAllocCode
	#define CHK_SysMemAllocCode  TRUE
	#define EXP_SysMemAllocCode  CAL_CMEXPAPI( "SysMemAllocCode" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemAllocCode
	#define EXT_SysMemAllocCode
	#define GET_SysMemAllocCode(fl)  CAL_CMGETAPI( "SysMemAllocCode" ) 
	#define CAL_SysMemAllocCode  SysMemAllocCode
	#define CHK_SysMemAllocCode  TRUE
	#define EXP_SysMemAllocCode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemAllocCode", (RTS_UINTPTR)SysMemAllocCode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemAllocCode
	#define EXT_SysMemSysMemAllocCode
	#define GET_SysMemSysMemAllocCode  ERR_OK
	#define CAL_SysMemSysMemAllocCode pISysMem->ISysMemAllocCode
	#define CHK_SysMemSysMemAllocCode (pISysMem != NULL)
	#define EXP_SysMemSysMemAllocCode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemAllocCode
	#define EXT_SysMemAllocCode
	#define GET_SysMemAllocCode(fl)  CAL_CMGETAPI( "SysMemAllocCode" ) 
	#define CAL_SysMemAllocCode pISysMem->ISysMemAllocCode
	#define CHK_SysMemAllocCode (pISysMem != NULL)
	#define EXP_SysMemAllocCode  CAL_CMEXPAPI( "SysMemAllocCode" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemAllocCode  PFSYSMEMALLOCCODE pfSysMemAllocCode;
	#define EXT_SysMemAllocCode  extern PFSYSMEMALLOCCODE pfSysMemAllocCode;
	#define GET_SysMemAllocCode(fl)  s_pfCMGetAPI2( "SysMemAllocCode", (RTS_VOID_FCTPTR *)&pfSysMemAllocCode, (fl), 0, 0)
	#define CAL_SysMemAllocCode  pfSysMemAllocCode
	#define CHK_SysMemAllocCode  (pfSysMemAllocCode != NULL)
	#define EXP_SysMemAllocCode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemAllocCode", (RTS_UINTPTR)SysMemAllocCode, 0, 0) 
#endif




/**
 * <description>
 *	Release code memory
 * </description>
 * <param name="pszComponentName" type="IN">Name of the component</param>
 * <param name="pData" type="IN">Pointer to memory to resize</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysMemFreeCode(char *pszComponentName, void* pCode);
typedef RTS_RESULT (CDECL * PFSYSMEMFREECODE) (char *pszComponentName, void* pCode);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMFREECODE_NOTIMPLEMENTED)
	#define USE_SysMemFreeCode
	#define EXT_SysMemFreeCode
	#define GET_SysMemFreeCode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemFreeCode(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysMemFreeCode  FALSE
	#define EXP_SysMemFreeCode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemFreeCode
	#define EXT_SysMemFreeCode
	#define GET_SysMemFreeCode(fl)  CAL_CMGETAPI( "SysMemFreeCode" ) 
	#define CAL_SysMemFreeCode  SysMemFreeCode
	#define CHK_SysMemFreeCode  TRUE
	#define EXP_SysMemFreeCode  CAL_CMEXPAPI( "SysMemFreeCode" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemFreeCode
	#define EXT_SysMemFreeCode
	#define GET_SysMemFreeCode(fl)  CAL_CMGETAPI( "SysMemFreeCode" ) 
	#define CAL_SysMemFreeCode  SysMemFreeCode
	#define CHK_SysMemFreeCode  TRUE
	#define EXP_SysMemFreeCode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemFreeCode", (RTS_UINTPTR)SysMemFreeCode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemFreeCode
	#define EXT_SysMemSysMemFreeCode
	#define GET_SysMemSysMemFreeCode  ERR_OK
	#define CAL_SysMemSysMemFreeCode pISysMem->ISysMemFreeCode
	#define CHK_SysMemSysMemFreeCode (pISysMem != NULL)
	#define EXP_SysMemSysMemFreeCode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemFreeCode
	#define EXT_SysMemFreeCode
	#define GET_SysMemFreeCode(fl)  CAL_CMGETAPI( "SysMemFreeCode" ) 
	#define CAL_SysMemFreeCode pISysMem->ISysMemFreeCode
	#define CHK_SysMemFreeCode (pISysMem != NULL)
	#define EXP_SysMemFreeCode  CAL_CMEXPAPI( "SysMemFreeCode" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemFreeCode  PFSYSMEMFREECODE pfSysMemFreeCode;
	#define EXT_SysMemFreeCode  extern PFSYSMEMFREECODE pfSysMemFreeCode;
	#define GET_SysMemFreeCode(fl)  s_pfCMGetAPI2( "SysMemFreeCode", (RTS_VOID_FCTPTR *)&pfSysMemFreeCode, (fl), 0, 0)
	#define CAL_SysMemFreeCode  pfSysMemFreeCode
	#define CHK_SysMemFreeCode  (pfSysMemFreeCode != NULL)
	#define EXP_SysMemFreeCode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemFreeCode", (RTS_UINTPTR)SysMemFreeCode, 0, 0) 
#endif




/**
 * <description>
 *	Check if a pointer points to a valid address.
 * </description>
 * <param name="ptr" type="IN" range="[NULL,VALID_MEMORY,INVALID_MEMORY]">Pointer to the memory to be checked</param>
 * <param name="ulSize" type="IN" range="[0,VALID_SIZE]">Size of the memory to be checked</param>
 * <param name="bWrite" type="IN" range="[0,1]">1=Check, if memory can be written, 0=Check only for read access</param>
 * <result>Error code:
 * <ul>
 *		<li>ERR_OK: Memory is valid</li>
 *		<li>ERR_FAILED: Memory is invalid. Cannot be accessed with the requested access mode bWrite</li>
 * </ul>
 * </result>
 */
RTS_RESULT CDECL SysMemIsValidPointer(void* ptr, RTS_SIZE ulSize, int bWrite);
typedef RTS_RESULT (CDECL * PFSYSMEMISVALIDPOINTER) (void* ptr, RTS_SIZE ulSize, int bWrite);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMISVALIDPOINTER_NOTIMPLEMENTED)
	#define USE_SysMemIsValidPointer
	#define EXT_SysMemIsValidPointer
	#define GET_SysMemIsValidPointer(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemIsValidPointer(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysMemIsValidPointer  FALSE
	#define EXP_SysMemIsValidPointer  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemIsValidPointer
	#define EXT_SysMemIsValidPointer
	#define GET_SysMemIsValidPointer(fl)  CAL_CMGETAPI( "SysMemIsValidPointer" ) 
	#define CAL_SysMemIsValidPointer  SysMemIsValidPointer
	#define CHK_SysMemIsValidPointer  TRUE
	#define EXP_SysMemIsValidPointer  CAL_CMEXPAPI( "SysMemIsValidPointer" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemIsValidPointer
	#define EXT_SysMemIsValidPointer
	#define GET_SysMemIsValidPointer(fl)  CAL_CMGETAPI( "SysMemIsValidPointer" ) 
	#define CAL_SysMemIsValidPointer  SysMemIsValidPointer
	#define CHK_SysMemIsValidPointer  TRUE
	#define EXP_SysMemIsValidPointer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemIsValidPointer", (RTS_UINTPTR)SysMemIsValidPointer, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemIsValidPointer
	#define EXT_SysMemSysMemIsValidPointer
	#define GET_SysMemSysMemIsValidPointer  ERR_OK
	#define CAL_SysMemSysMemIsValidPointer pISysMem->ISysMemIsValidPointer
	#define CHK_SysMemSysMemIsValidPointer (pISysMem != NULL)
	#define EXP_SysMemSysMemIsValidPointer  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemIsValidPointer
	#define EXT_SysMemIsValidPointer
	#define GET_SysMemIsValidPointer(fl)  CAL_CMGETAPI( "SysMemIsValidPointer" ) 
	#define CAL_SysMemIsValidPointer pISysMem->ISysMemIsValidPointer
	#define CHK_SysMemIsValidPointer (pISysMem != NULL)
	#define EXP_SysMemIsValidPointer  CAL_CMEXPAPI( "SysMemIsValidPointer" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemIsValidPointer  PFSYSMEMISVALIDPOINTER pfSysMemIsValidPointer;
	#define EXT_SysMemIsValidPointer  extern PFSYSMEMISVALIDPOINTER pfSysMemIsValidPointer;
	#define GET_SysMemIsValidPointer(fl)  s_pfCMGetAPI2( "SysMemIsValidPointer", (RTS_VOID_FCTPTR *)&pfSysMemIsValidPointer, (fl), 0, 0)
	#define CAL_SysMemIsValidPointer  pfSysMemIsValidPointer
	#define CHK_SysMemIsValidPointer  (pfSysMemIsValidPointer != NULL)
	#define EXP_SysMemIsValidPointer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemIsValidPointer", (RTS_UINTPTR)SysMemIsValidPointer, 0, 0) 
#endif




/**
 * <description>
 *	Routine to swap memory. If little endian (Intel) byte-order is received and platform has
 *	big endian (Motorola) byte-order. On little endian byte-order platforms, routine does nothing.
 * </description>
 * <param name="pbyBuffer" type="IN">Pointer to data to swap. You can check, which order is selected by 
 *									calling the routine with pbyBuffer=NULL</param>
 * <param name="iSize" type="IN">Size of one element to swap</param>
 * <param name="iCount" type="IN">Number of elements to swap</param>
 * <result>	-1 = failed (iSize too large)
 *    		 0 = no swapping necessary (little endian byte-order)
 *			>0 = Number of bytes swapped (big endian byte-order)
 *			 1 = big endian byte-order, if pbyBuffer=NULL
 * </result>
 */
int CDECL SysMemSwap(unsigned char *pbyBuffer, int iSize, int iCount);
typedef int (CDECL * PFSYSMEMSWAP) (unsigned char *pbyBuffer, int iSize, int iCount);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMSWAP_NOTIMPLEMENTED)
	#define USE_SysMemSwap
	#define EXT_SysMemSwap
	#define GET_SysMemSwap(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemSwap(p0,p1,p2)  (int)ERR_NOTIMPLEMENTED
	#define CHK_SysMemSwap  FALSE
	#define EXP_SysMemSwap  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemSwap
	#define EXT_SysMemSwap
	#define GET_SysMemSwap(fl)  CAL_CMGETAPI( "SysMemSwap" ) 
	#define CAL_SysMemSwap  SysMemSwap
	#define CHK_SysMemSwap  TRUE
	#define EXP_SysMemSwap  CAL_CMEXPAPI( "SysMemSwap" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemSwap
	#define EXT_SysMemSwap
	#define GET_SysMemSwap(fl)  CAL_CMGETAPI( "SysMemSwap" ) 
	#define CAL_SysMemSwap  SysMemSwap
	#define CHK_SysMemSwap  TRUE
	#define EXP_SysMemSwap  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemSwap", (RTS_UINTPTR)SysMemSwap, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemSwap
	#define EXT_SysMemSysMemSwap
	#define GET_SysMemSysMemSwap  ERR_OK
	#define CAL_SysMemSysMemSwap pISysMem->ISysMemSwap
	#define CHK_SysMemSysMemSwap (pISysMem != NULL)
	#define EXP_SysMemSysMemSwap  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemSwap
	#define EXT_SysMemSwap
	#define GET_SysMemSwap(fl)  CAL_CMGETAPI( "SysMemSwap" ) 
	#define CAL_SysMemSwap pISysMem->ISysMemSwap
	#define CHK_SysMemSwap (pISysMem != NULL)
	#define EXP_SysMemSwap  CAL_CMEXPAPI( "SysMemSwap" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemSwap  PFSYSMEMSWAP pfSysMemSwap;
	#define EXT_SysMemSwap  extern PFSYSMEMSWAP pfSysMemSwap;
	#define GET_SysMemSwap(fl)  s_pfCMGetAPI2( "SysMemSwap", (RTS_VOID_FCTPTR *)&pfSysMemSwap, (fl), 0, 0)
	#define CAL_SysMemSwap  pfSysMemSwap
	#define CHK_SysMemSwap  (pfSysMemSwap != NULL)
	#define EXP_SysMemSwap  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemSwap", (RTS_UINTPTR)SysMemSwap, 0, 0) 
#endif




/**
 * <description>
 *	Routine to force swapping memory independent of the byte-order of the system!
 * </description>
 * <param name="pbyBuffer" type="IN">Pointer to data to swap. You can check, which order is selected by 
 *									calling the routine with pbyBuffer=NULL</param>
 * <param name="iSize" type="IN">Size of one element to swap</param>
 * <param name="iCount" type="IN">Number of elements to swap</param>
 * <result>	-1 = failed (iSize too large)
 *			>0 = Number of bytes swapped
 * </result>
 */
int CDECL SysMemForceSwap(unsigned char *pbyBuffer, int iSize, int iCount);
typedef int (CDECL * PFSYSMEMFORCESWAP) (unsigned char *pbyBuffer, int iSize, int iCount);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMFORCESWAP_NOTIMPLEMENTED)
	#define USE_SysMemForceSwap
	#define EXT_SysMemForceSwap
	#define GET_SysMemForceSwap(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemForceSwap(p0,p1,p2)  (int)ERR_NOTIMPLEMENTED
	#define CHK_SysMemForceSwap  FALSE
	#define EXP_SysMemForceSwap  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemForceSwap
	#define EXT_SysMemForceSwap
	#define GET_SysMemForceSwap(fl)  CAL_CMGETAPI( "SysMemForceSwap" ) 
	#define CAL_SysMemForceSwap  SysMemForceSwap
	#define CHK_SysMemForceSwap  TRUE
	#define EXP_SysMemForceSwap  CAL_CMEXPAPI( "SysMemForceSwap" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemForceSwap
	#define EXT_SysMemForceSwap
	#define GET_SysMemForceSwap(fl)  CAL_CMGETAPI( "SysMemForceSwap" ) 
	#define CAL_SysMemForceSwap  SysMemForceSwap
	#define CHK_SysMemForceSwap  TRUE
	#define EXP_SysMemForceSwap  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemForceSwap", (RTS_UINTPTR)SysMemForceSwap, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemForceSwap
	#define EXT_SysMemSysMemForceSwap
	#define GET_SysMemSysMemForceSwap  ERR_OK
	#define CAL_SysMemSysMemForceSwap pISysMem->ISysMemForceSwap
	#define CHK_SysMemSysMemForceSwap (pISysMem != NULL)
	#define EXP_SysMemSysMemForceSwap  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemForceSwap
	#define EXT_SysMemForceSwap
	#define GET_SysMemForceSwap(fl)  CAL_CMGETAPI( "SysMemForceSwap" ) 
	#define CAL_SysMemForceSwap pISysMem->ISysMemForceSwap
	#define CHK_SysMemForceSwap (pISysMem != NULL)
	#define EXP_SysMemForceSwap  CAL_CMEXPAPI( "SysMemForceSwap" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemForceSwap  PFSYSMEMFORCESWAP pfSysMemForceSwap;
	#define EXT_SysMemForceSwap  extern PFSYSMEMFORCESWAP pfSysMemForceSwap;
	#define GET_SysMemForceSwap(fl)  s_pfCMGetAPI2( "SysMemForceSwap", (RTS_VOID_FCTPTR *)&pfSysMemForceSwap, (fl), 0, 0)
	#define CAL_SysMemForceSwap  pfSysMemForceSwap
	#define CHK_SysMemForceSwap  (pfSysMemForceSwap != NULL)
	#define EXP_SysMemForceSwap  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemForceSwap", (RTS_UINTPTR)SysMemForceSwap, 0, 0) 
#endif




/**
 * <description>
 *	Routine to get total size of the currently allocated heap memory.
 * </description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="pResult" type="ERR_OK">Current heap size is available</errorcode>
 * <errorcode name="pResult" type="ERR_NOT_SUPPORTED">Determination of current heap size not supported. CmpMemGC is necessary!</errorcode>
 * <result>Size of the memory in bytes that is currently allocated from the heap</result>
 */
RTS_SIZE CDECL SysMemGetCurrentHeapSize(RTS_RESULT *pResult);
typedef RTS_SIZE (CDECL * PFSYSMEMGETCURRENTHEAPSIZE) (RTS_RESULT *pResult);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMGETCURRENTHEAPSIZE_NOTIMPLEMENTED)
	#define USE_SysMemGetCurrentHeapSize
	#define EXT_SysMemGetCurrentHeapSize
	#define GET_SysMemGetCurrentHeapSize(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemGetCurrentHeapSize(p0)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysMemGetCurrentHeapSize  FALSE
	#define EXP_SysMemGetCurrentHeapSize  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemGetCurrentHeapSize
	#define EXT_SysMemGetCurrentHeapSize
	#define GET_SysMemGetCurrentHeapSize(fl)  CAL_CMGETAPI( "SysMemGetCurrentHeapSize" ) 
	#define CAL_SysMemGetCurrentHeapSize  SysMemGetCurrentHeapSize
	#define CHK_SysMemGetCurrentHeapSize  TRUE
	#define EXP_SysMemGetCurrentHeapSize  CAL_CMEXPAPI( "SysMemGetCurrentHeapSize" ) 
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemGetCurrentHeapSize
	#define EXT_SysMemGetCurrentHeapSize
	#define GET_SysMemGetCurrentHeapSize(fl)  CAL_CMGETAPI( "SysMemGetCurrentHeapSize" ) 
	#define CAL_SysMemGetCurrentHeapSize  SysMemGetCurrentHeapSize
	#define CHK_SysMemGetCurrentHeapSize  TRUE
	#define EXP_SysMemGetCurrentHeapSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemGetCurrentHeapSize", (RTS_UINTPTR)SysMemGetCurrentHeapSize, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemGetCurrentHeapSize
	#define EXT_SysMemSysMemGetCurrentHeapSize
	#define GET_SysMemSysMemGetCurrentHeapSize  ERR_OK
	#define CAL_SysMemSysMemGetCurrentHeapSize pISysMem->ISysMemGetCurrentHeapSize
	#define CHK_SysMemSysMemGetCurrentHeapSize (pISysMem != NULL)
	#define EXP_SysMemSysMemGetCurrentHeapSize  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemGetCurrentHeapSize
	#define EXT_SysMemGetCurrentHeapSize
	#define GET_SysMemGetCurrentHeapSize(fl)  CAL_CMGETAPI( "SysMemGetCurrentHeapSize" ) 
	#define CAL_SysMemGetCurrentHeapSize pISysMem->ISysMemGetCurrentHeapSize
	#define CHK_SysMemGetCurrentHeapSize (pISysMem != NULL)
	#define EXP_SysMemGetCurrentHeapSize  CAL_CMEXPAPI( "SysMemGetCurrentHeapSize" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemGetCurrentHeapSize  PFSYSMEMGETCURRENTHEAPSIZE pfSysMemGetCurrentHeapSize;
	#define EXT_SysMemGetCurrentHeapSize  extern PFSYSMEMGETCURRENTHEAPSIZE pfSysMemGetCurrentHeapSize;
	#define GET_SysMemGetCurrentHeapSize(fl)  s_pfCMGetAPI2( "SysMemGetCurrentHeapSize", (RTS_VOID_FCTPTR *)&pfSysMemGetCurrentHeapSize, (fl), 0, 0)
	#define CAL_SysMemGetCurrentHeapSize  pfSysMemGetCurrentHeapSize
	#define CHK_SysMemGetCurrentHeapSize  (pfSysMemGetCurrentHeapSize != NULL)
	#define EXP_SysMemGetCurrentHeapSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemGetCurrentHeapSize", (RTS_UINTPTR)SysMemGetCurrentHeapSize, 0, 0) 
#endif




/**
 * <description>
 *	C-Library compatible malloc() function! Is internally mapped to SysMemAllocData()!
 * </description>
 * <param name="size" type="IN">Size in bytes of the heap buffer to allocate</param>
 * <result>Pointer to the allocated heap buffer</result>
 */
void * CDECL SysMemMalloc(RTS_SIZE size);
typedef void * (CDECL * PFSYSMEMMALLOC) (RTS_SIZE size);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMMALLOC_NOTIMPLEMENTED)
	#define USE_SysMemMalloc
	#define EXT_SysMemMalloc
	#define GET_SysMemMalloc(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemMalloc(p0) (void *)ERR_NOTIMPLEMENTED
	#define CHK_SysMemMalloc  FALSE
	#define EXP_SysMemMalloc  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemMalloc
	#define EXT_SysMemMalloc
	#define GET_SysMemMalloc(fl)  CAL_CMGETAPI( "SysMemMalloc" ) 
	#define CAL_SysMemMalloc  SysMemMalloc
	#define CHK_SysMemMalloc  TRUE
	#define EXP_SysMemMalloc  CAL_CMEXPAPI( "SysMemMalloc" ) 
	#define EXTREF_ITF_SysMemMalloc {(RTS_VOID_FCTPTR)SysMemMalloc, "SysMemMalloc", 0, 0}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemMalloc
	#define EXT_SysMemMalloc
	#define GET_SysMemMalloc(fl)  CAL_CMGETAPI( "SysMemMalloc" ) 
	#define CAL_SysMemMalloc  SysMemMalloc
	#define CHK_SysMemMalloc  TRUE
	#define EXP_SysMemMalloc  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemMalloc", (RTS_UINTPTR)SysMemMalloc, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemMalloc
	#define EXT_SysMemSysMemMalloc
	#define GET_SysMemSysMemMalloc  ERR_OK
	#define CAL_SysMemSysMemMalloc  SysMemMalloc
	#define CHK_SysMemSysMemMalloc  TRUE
	#define EXP_SysMemSysMemMalloc  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemMalloc
	#define EXT_SysMemMalloc
	#define GET_SysMemMalloc(fl)  CAL_CMGETAPI( "SysMemMalloc" ) 
	#define CAL_SysMemMalloc  SysMemMalloc
	#define CHK_SysMemMalloc  TRUE
	#define EXP_SysMemMalloc  CAL_CMEXPAPI( "SysMemMalloc" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemMalloc  PFSYSMEMMALLOC pfSysMemMalloc;
	#define EXT_SysMemMalloc  extern PFSYSMEMMALLOC pfSysMemMalloc;
	#define GET_SysMemMalloc(fl)  s_pfCMGetAPI2( "SysMemMalloc", (RTS_VOID_FCTPTR *)&pfSysMemMalloc, (fl), 0, 0)
	#define CAL_SysMemMalloc  pfSysMemMalloc
	#define CHK_SysMemMalloc  (pfSysMemMalloc != NULL)
	#define EXP_SysMemMalloc   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemMalloc", (RTS_UINTPTR)SysMemMalloc, 0, 0) 
#endif


/**
 * <description>
 *	C-Library compatible realloc() function! Is internally mapped to SysMemReallocData()!
 * </description>
 * <param name="pMem" type="IN">Pointer to buffer to be reallocated</param>
 * <param name="size" type="IN">Size in bytes of the heap buffer to allocate</param>
 * <result>Pointer to the reallocated heap buffer</result>
 */
void * CDECL SysMemRealloc(void *pMem, RTS_SIZE size);
typedef void * (CDECL * PFSYSMEMREALLOC) (void *pMem, RTS_SIZE size);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMREALLOC_NOTIMPLEMENTED)
	#define USE_SysMemRealloc
	#define EXT_SysMemRealloc
	#define GET_SysMemRealloc(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemRealloc(p0,p1) (void *)ERR_NOTIMPLEMENTED
	#define CHK_SysMemRealloc  FALSE
	#define EXP_SysMemRealloc  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemRealloc
	#define EXT_SysMemRealloc
	#define GET_SysMemRealloc(fl)  CAL_CMGETAPI( "SysMemRealloc" ) 
	#define CAL_SysMemRealloc  SysMemRealloc
	#define CHK_SysMemRealloc  TRUE
	#define EXP_SysMemRealloc  CAL_CMEXPAPI( "SysMemRealloc" ) 
	#define EXTREF_ITF_SysMemRealloc {(RTS_VOID_FCTPTR)SysMemRealloc, "SysMemRealloc", 0, 0}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemRealloc
	#define EXT_SysMemRealloc
	#define GET_SysMemRealloc(fl)  CAL_CMGETAPI( "SysMemRealloc" ) 
	#define CAL_SysMemRealloc  SysMemRealloc
	#define CHK_SysMemRealloc  TRUE
	#define EXP_SysMemRealloc  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemRealloc", (RTS_UINTPTR)SysMemRealloc, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemRealloc
	#define EXT_SysMemSysMemRealloc
	#define GET_SysMemSysMemRealloc  ERR_OK
	#define CAL_SysMemSysMemRealloc  SysMemRealloc
	#define CHK_SysMemSysMemRealloc  TRUE
	#define EXP_SysMemSysMemRealloc  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemRealloc
	#define EXT_SysMemRealloc
	#define GET_SysMemRealloc(fl)  CAL_CMGETAPI( "SysMemRealloc" ) 
	#define CAL_SysMemRealloc  SysMemRealloc
	#define CHK_SysMemRealloc  TRUE
	#define EXP_SysMemRealloc  CAL_CMEXPAPI( "SysMemRealloc" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemRealloc  PFSYSMEMREALLOC pfSysMemRealloc;
	#define EXT_SysMemRealloc  extern PFSYSMEMREALLOC pfSysMemRealloc;
	#define GET_SysMemRealloc(fl)  s_pfCMGetAPI2( "SysMemRealloc", (RTS_VOID_FCTPTR *)&pfSysMemRealloc, (fl), 0, 0)
	#define CAL_SysMemRealloc  pfSysMemRealloc
	#define CHK_SysMemRealloc  (pfSysMemRealloc != NULL)
	#define EXP_SysMemRealloc   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemRealloc", (RTS_UINTPTR)SysMemRealloc, 0, 0) 
#endif


/**
 * <description>
 *	C-Library compatible free() function! Is internally mapped to SysMemFreeData()!
 * </description>
 * <param name="pMem" type="IN">Pointer to buffer to release/free</param>
 */
void CDECL SysMemFree(void *pMem);
typedef void (CDECL * PFSYSMEMFREE) (void *pMem);
#if defined(SYSMEM_NOTIMPLEMENTED) || defined(SYSMEMFREE_NOTIMPLEMENTED)
	#define USE_SysMemFree
	#define EXT_SysMemFree
	#define GET_SysMemFree(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysMemFree(p0) 
	#define CHK_SysMemFree  FALSE
	#define EXP_SysMemFree  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysMemFree
	#define EXT_SysMemFree
	#define GET_SysMemFree(fl)  CAL_CMGETAPI( "SysMemFree" ) 
	#define CAL_SysMemFree  SysMemFree
	#define CHK_SysMemFree  TRUE
	#define EXP_SysMemFree  CAL_CMEXPAPI( "SysMemFree" ) 
	#define EXTREF_ITF_SysMemFree {(RTS_VOID_FCTPTR)SysMemFree, "SysMemFree", 0, 0}
#elif defined(MIXED_LINK) && !defined(SYSMEM_EXTERNAL)
	#define USE_SysMemFree
	#define EXT_SysMemFree
	#define GET_SysMemFree(fl)  CAL_CMGETAPI( "SysMemFree" ) 
	#define CAL_SysMemFree  SysMemFree
	#define CHK_SysMemFree  TRUE
	#define EXP_SysMemFree  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemFree", (RTS_UINTPTR)SysMemFree, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysMemSysMemFree
	#define EXT_SysMemSysMemFree
	#define GET_SysMemSysMemFree  ERR_OK
	#define CAL_SysMemSysMemFree  SysMemFree
	#define CHK_SysMemSysMemFree  TRUE
	#define EXP_SysMemSysMemFree  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysMemFree
	#define EXT_SysMemFree
	#define GET_SysMemFree(fl)  CAL_CMGETAPI( "SysMemFree" ) 
	#define CAL_SysMemFree  SysMemFree
	#define CHK_SysMemFree  TRUE
	#define EXP_SysMemFree  CAL_CMEXPAPI( "SysMemFree" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysMemFree  PFSYSMEMFREE pfSysMemFree;
	#define EXT_SysMemFree  extern PFSYSMEMFREE pfSysMemFree;
	#define GET_SysMemFree(fl)  s_pfCMGetAPI2( "SysMemFree", (RTS_VOID_FCTPTR *)&pfSysMemFree, (fl), 0, 0)
	#define CAL_SysMemFree  pfSysMemFree
	#define CHK_SysMemFree  (pfSysMemFree != NULL)
	#define EXP_SysMemFree   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysMemFree", (RTS_UINTPTR)SysMemFree, 0, 0) 
#endif



/**
 * <description>
 *	Set protection of memory mapping.
 *  
 *  Changes the access protections to be that specified by
 *  protectionFlags for those whole pages containing any part
 *  of the address space of the process starting at address pAddr and
 *  continuing for len bytes.
 * </description>
 * <param name="pAddr" type="IN">The address of the starting page of
 *  the region of pages whose access protection attributes are to be
 *  changed.</param>
 * <param name="len" type="IN">The size of the region whose access
 *  protection attributes are to be changed, in bytes. The region of
 *  affected pages includes all pages containing one or more bytes in
 *  the range from the pAddr parameter to (pAddr+len-1).</param>
 * <param name="protectionFlags" type="IN">Determines whether
 *  read, write, execute, or some combination of accesses are
 *  permitted to the data being mapped. The protectionFlags argument should be
 *  either SYSMEM_PROTECT_NONE or the bit-wise-inclusive OR of one or more of
 *  SYSMEM_PROTECT_READ, SYSMEM_PROTECT_WRITE, and SYSMEM_PROTECT_EXEC.
 *  See corresponding category "Memory protection constants".</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successful completion.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_ACCESS_RIGHTS">The protectionFlags
 *  argument specifies a protection that violates the access permission the
 *  process has to the underlying memory object.</errorcode>  
 * <errorcode name="RTS_RESULT" type="ERR_NOBUFFER">On POSIX systems: The
 *  protectionFlags argument specifies SYSMEM_PROTECT_WRITE over a MAP_PRIVATE
 *  mapping and there are insufficient memory resources to reserve for locking
 *  the private page.</errorcode>  
 * <errorcode name="RTS_RESULT" type="ERR_NOBUFFER">On POSIX systems: The
 *  protectionFlags argument specifies SYSMEM_PROTECT_WRITE on a MAP_PRIVATE
 *  mapping, and it would require more space than the system is able to supply
 *  for locking the private pages, if required.</errorcode>  
 * <errorcode name="RTS_RESULT" type="ERR_ALIGNMENT">The pAddr argument is not
 *  a multiple of the page size of the system.</errorcode>  
 * <errorcode name="RTS_RESULT" type="ERR_NO_CONSISTENCY">Addresses in the range
 *  [pAddr,pAddr+len) are invalid for the address space of a process, or specify
 *  one or more pages which are not mapped.</errorcode>  
 * <errorcode name="RTS_RESULT" type="ERR_NOT_SUPPORTED">The implementation does
 *  not support the combination of accesses requested in the protectionFlags
 *  argument.</errorcode>  
 */
/*
DEF_ITF_API(`RTS_RESULT',`CDECL',`SysMemProtect',`(void* pAddr, RTS_SIZE len, RTS_UI32 protectionFlags)')
*/

#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSMEMALLOCDATA ISysMemAllocData;
 	PFSYSMEMREALLOCDATA ISysMemReallocData;
 	PFSYSMEMFREEDATA ISysMemFreeData;
 	PFSYSMEMALLOCAREA ISysMemAllocArea;
 	PFSYSMEMFREEAREA ISysMemFreeArea;
 	PFSYSMEMALLOCCODE ISysMemAllocCode;
 	PFSYSMEMFREECODE ISysMemFreeCode;
 	PFSYSMEMISVALIDPOINTER ISysMemIsValidPointer;
 	PFSYSMEMSWAP ISysMemSwap;
 	PFSYSMEMFORCESWAP ISysMemForceSwap;
 	PFSYSMEMGETCURRENTHEAPSIZE ISysMemGetCurrentHeapSize;
 } ISysMem_C;

#ifdef CPLUSPLUS
class ISysMem : public IBase
{
	public:
		virtual void* CDECL ISysMemAllocData(char *pszComponentName, RTS_SIZE ulSize, RTS_RESULT *pResult) =0;
		virtual void* CDECL ISysMemReallocData(char *pszComponentName, void* pData, RTS_SIZE ulSize, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysMemFreeData(char *pszComponentName, void* pData) =0;
		virtual void* CDECL ISysMemAllocArea(char *pszComponentName, unsigned short usType, RTS_SIZE ulSize, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysMemFreeArea(char *pszComponentName, void* pData) =0;
		virtual void* CDECL ISysMemAllocCode(char *pszComponentName, RTS_SIZE ulSize, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysMemFreeCode(char *pszComponentName, void* pCode) =0;
		virtual RTS_RESULT CDECL ISysMemIsValidPointer(void* ptr, RTS_SIZE ulSize, int bWrite) =0;
		virtual int CDECL ISysMemSwap(unsigned char *pbyBuffer, int iSize, int iCount) =0;
		virtual int CDECL ISysMemForceSwap(unsigned char *pbyBuffer, int iSize, int iCount) =0;
		virtual RTS_SIZE CDECL ISysMemGetCurrentHeapSize(RTS_RESULT *pResult) =0;
};
	#ifndef ITF_SysMem
		#define ITF_SysMem static ISysMem *pISysMem = NULL;
	#endif
	#define EXTITF_SysMem
#else	/*CPLUSPLUS*/
	typedef ISysMem_C		ISysMem;
	#ifndef ITF_SysMem
		#define ITF_SysMem
	#endif
	#define EXTITF_SysMem
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSMEMITF_H_*/
