 /**
 * <interfacename>SysInt</interfacename>
 * <description> 
 *	<p>The SysInt interface is projected to get access to the hardware interrupts of 
 *	the system.</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _SYSINTITF_H_
#define _SYSINTITF_H_

#include "CmpStd.h"

 

 




#include "CmpItf.h"

/**
 * <category>Static defines</category>
 * <description>Maximum number of interrupts</description>
 */
#ifndef MAX_INT
	#define		MAX_INT		2
#endif

/**
 * <category>Static defines</category>
 * <description>Maximum number of callback function for each interrupt</description>
 */
#ifndef MAX_INT_CALLBACKS
	#define		MAX_INT_CALLBACKS	1
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * Use SysInt mapping for interrupts. Only used for WinCE.
 * 0: Do not use
 * 1: Use it
 * </description>
 */
#define SYSINTKEY_INT_WINCE_USE_SYSINTMAPPING				"WinCE.UseIRQSysIntrMapping"
#define SYSINTVALUE_INT_WINCE_DEFAULT_USE_SYSINTMAPPING		0

/**
 * <category>Interrupt handler type</category>
 * <description>Specification, which type of routine the handler is</description>
 */
#define		SYS_INT_NONE	0
#define		SYS_INT_IEC		1
#define		SYS_INT_C		2
	
/**
 * <category>Interrupt handler</category>
 * <description>Interrupt handler prototype</description>
 * <param name="ulAdditionalInfo" type="IN">Additional info that can be specified at registering the handler</param>
 */
typedef void HUGEPTR(CDECL *SYS_INT_INTHANDLER)(RTS_UINTPTR ulAdditionalInfo);

RTS_RESULT CDECL SysIntOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysIntOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <category>Event parameter</category>
 * <element name="ulInterrupt" type="IN">Interrupt number</element>
 */
typedef struct
{
	unsigned long ulInterrupt;
} EVTPARAM_SysInt;
#define EVTPARAMID_SysInt			0x0001
#define EVTVERSION_SysInt			0x0001

/**
 * <category>Events</category>
 * <description>Hardware interrupt events</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysInt</param>
 */
#define EVT_INTERRUPT_0						MAKE_EVENTID(EVTCLASS_INTERRUPT, 0)
#define EVT_INTERRUPT_1						MAKE_EVENTID(EVTCLASS_INTERRUPT, 1)
#define EVT_INTERRUPT_2						MAKE_EVENTID(EVTCLASS_INTERRUPT, 2)
#define EVT_INTERRUPT_3						MAKE_EVENTID(EVTCLASS_INTERRUPT, 3)
#define EVT_INTERRUPT_4						MAKE_EVENTID(EVTCLASS_INTERRUPT, 4)
#define EVT_INTERRUPT_5						MAKE_EVENTID(EVTCLASS_INTERRUPT, 5)
#define EVT_INTERRUPT_6						MAKE_EVENTID(EVTCLASS_INTERRUPT, 6)
#define EVT_INTERRUPT_7						MAKE_EVENTID(EVTCLASS_INTERRUPT, 7)
#define EVT_INTERRUPT_8						MAKE_EVENTID(EVTCLASS_INTERRUPT, 8)
#define EVT_INTERRUPT_9						MAKE_EVENTID(EVTCLASS_INTERRUPT, 9)
#define EVT_INTERRUPT_10					MAKE_EVENTID(EVTCLASS_INTERRUPT, 10)
#define EVT_INTERRUPT_11					MAKE_EVENTID(EVTCLASS_INTERRUPT, 11)
#define EVT_INTERRUPT_12					MAKE_EVENTID(EVTCLASS_INTERRUPT, 12)
#define EVT_INTERRUPT_13					MAKE_EVENTID(EVTCLASS_INTERRUPT, 13)
#define EVT_INTERRUPT_14					MAKE_EVENTID(EVTCLASS_INTERRUPT, 14)
#define EVT_INTERRUPT_15					MAKE_EVENTID(EVTCLASS_INTERRUPT, 15)
#define EVT_INTERRUPT_255					MAKE_EVENTID(EVTCLASS_INTERRUPT, 255)

/**
 * <description>Sys Int Callback Info Struct</description>
 * <param name="pCallback">Pointer to Callback for Sys Int Callback Info</param>
 * <param name="ulType">Type of Sys Int Callback Info</param>
 * <param name="ulAdditionalInfo">Additional Info for Sys Int Callback Info</param>
 */
typedef struct
{
	SYS_INT_INTHANDLER 	pCallback;
	unsigned long		ulType;
	RTS_UINTPTR			ulAdditionalInfo;
}SYS_INT_CALLBACK_INFO;

/**
 * <description>Sys Int Info Struct</description>
 * <param name="pszName">Name for Sys Int Info</param>
 * <param name="pSysData">SysData Pointer for Sys Int Info</param>
 * <param name="hCallbackPool">Handle for CallbackPool for Sys Int Info</param>
 * <param name="ulInterrupt">InterruptNr for Sys Int Info</param>
 */
typedef struct
{
	char*					pszName;
	void*					pSysData;
	RTS_HANDLE				hCallbackPool;
	unsigned long			ulInterrupt;
}SYS_INT_INFO;

/**
 * <category>Bus types</category>
 * <description>Architecture specific bus types</description>
 */
#define BT_Internal		0
#define BT_ISA			1
#define BT_EISA			2
#define BT_PCI			3
#define BT_VME			4
#define BT_PCMCIA		5
#define BT_ProcessorInternal	6
#define BT_MaxType		7

/**
 * <category>Interrupt modes</category>
 * <description></description>
 */
#define IM_LevelSensitive	0
#define IM_Latched			1


#define ISysIntIEC			isysintiec_struct

/** EXTERN LIB SECTION BEGIN **/

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <category>Pci interrupt</category>
 * <description><p>
 *	PCI specific interrupt description
 * </p></description>
 */
typedef struct tagPciInterrupt
{
	RTS_IEC_UDINT ulBusNumber;		/* <element name="ulBusNumber" type="IN">Bus number on the PCI bus</element> */
	RTS_IEC_UDINT ulDevciceNumber;		/* <element name="ulDevciceNumber" type="IN">Device number on the PCI bus</element> */
	RTS_IEC_UDINT ulFunctionNumber;		/* <element name="ulFunctionNumber" type="IN">Function number on the PCI bus. Is used for different functions on the same device.</element> */
	RTS_IEC_UDINT ulIntLine;		/* <element name="ulIntLine" type="IN">Interrupt line on the PCI bus.</element> */
} PciInterrupt;

/**
 * <category>Isa interrupt</category>
 * <description><p>
 *	ISA specific interrupt description
 * </p></description>
 */
typedef struct tagIsaInterrupt
{
	RTS_IEC_UDINT ulBusNumber;		/* <element name="ulBusNumber" type="IN">Unused on the ISA bus</element> */
	RTS_IEC_UDINT ulDevciceNumber;		/* <element name="ulDevciceNumber" type="IN">Unused on the ISA bus</element> */
	RTS_IEC_UDINT ulFunctionNumber;		/* <element name="ulFunctionNumber" type="IN">Unused on the ISA bus</element> */
	RTS_IEC_UDINT ulIntLine;		/* <element name="ulIntLine" type="IN">Unused on the ISA bus</element> */
} IsaInterrupt;

/**
 * <category>Bus specific</category>
 * <description><p>
 *	Bus specific interrupt description
 * </p></description>
 */
typedef union
{
	PciInterrupt pciInterrupt;		/* <param name="pciInterrupt">PCI interrupt</param> */
	IsaInterrupt isaInterrupt;		/* <param name="isaInterrupt">ISA interrupt</param> */
} BusSpecific;

/**
 * <category>Interrupt description</category>
 * <description><p>Optional description for an interrupt</p></description>
 */
typedef struct tagSYS_INT_DESCRIPTION
{
	RTS_IEC_UDINT BusType;		/* <param name="BusType">Bus type (architecture specific). See "BusTypes" for details.</param> */
	RTS_IEC_UDINT InterruptMode;		/* <param name="InterruptMode">Interrupt mode</param> */
	BusSpecific busSpecific;		/* <param name="busSpecific">Pci bus specific definitions</param> */
} SYS_INT_DESCRIPTION;

typedef struct
{
	void* __VFTABLEPOINTER;	/* Pointer to virtual function table */
} isysintiec_struct;

/**
 * <description>Close an interrupt</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsysintclose_struct
{
	RTS_IEC_HANDLE hInt;				/* VAR_INPUT */	/* <param name="hInt" type="IN">Handle to interrupt returned by SysIntOpen.</param> */
	RTS_IEC_RESULT SysIntClose;			/* VAR_OUTPUT */	
} sysintclose_struct;

void CDECL CDECL_EXT sysintclose(sysintclose_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTCLOSE_IEC) (sysintclose_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTCLOSE_NOTIMPLEMENTED)
	#define USE_sysintclose
	#define EXT_sysintclose
	#define GET_sysintclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintclose(p0) 
	#define CHK_sysintclose  FALSE
	#define EXP_sysintclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintclose
	#define EXT_sysintclose
	#define GET_sysintclose(fl)  CAL_CMGETAPI( "sysintclose" ) 
	#define CAL_sysintclose  sysintclose
	#define CHK_sysintclose  TRUE
	#define EXP_sysintclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintclose", (RTS_UINTPTR)sysintclose, 1, RTSITF_GET_SIGNATURE(0, 0x0E05F037), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintclose
	#define EXT_sysintclose
	#define GET_sysintclose(fl)  CAL_CMGETAPI( "sysintclose" ) 
	#define CAL_sysintclose  sysintclose
	#define CHK_sysintclose  TRUE
	#define EXP_sysintclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintclose", (RTS_UINTPTR)sysintclose, 1, RTSITF_GET_SIGNATURE(0, 0x0E05F037), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintclose
	#define EXT_SysIntsysintclose
	#define GET_SysIntsysintclose  ERR_OK
	#define CAL_SysIntsysintclose  sysintclose
	#define CHK_SysIntsysintclose  TRUE
	#define EXP_SysIntsysintclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintclose", (RTS_UINTPTR)sysintclose, 1, RTSITF_GET_SIGNATURE(0, 0x0E05F037), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintclose
	#define EXT_sysintclose
	#define GET_sysintclose(fl)  CAL_CMGETAPI( "sysintclose" ) 
	#define CAL_sysintclose  sysintclose
	#define CHK_sysintclose  TRUE
	#define EXP_sysintclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintclose", (RTS_UINTPTR)sysintclose, 1, RTSITF_GET_SIGNATURE(0, 0x0E05F037), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintclose  PFSYSINTCLOSE_IEC pfsysintclose;
	#define EXT_sysintclose  extern PFSYSINTCLOSE_IEC pfsysintclose;
	#define GET_sysintclose(fl)  s_pfCMGetAPI2( "sysintclose", (RTS_VOID_FCTPTR *)&pfsysintclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x0E05F037), 0x03050500)
	#define CAL_sysintclose  pfsysintclose
	#define CHK_sysintclose  (pfsysintclose != NULL)
	#define EXP_sysintclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintclose", (RTS_UINTPTR)sysintclose, 1, RTSITF_GET_SIGNATURE(0, 0x0E05F037), 0x03050500) 
#endif


/**
 * <description>Function to disable an interrupt. By use of this function the interrupt specified by its handle
 * gets deactivated (masked).</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsysintdisable_struct
{
	RTS_IEC_HANDLE hInt;				/* VAR_INPUT */	/* <param name="hInt" type="IN">Handle to interrupt returned by SysIntOpen.</param> */
	RTS_IEC_RESULT SysIntDisable;		/* VAR_OUTPUT */	
} sysintdisable_struct;

void CDECL CDECL_EXT sysintdisable(sysintdisable_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTDISABLE_IEC) (sysintdisable_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTDISABLE_NOTIMPLEMENTED)
	#define USE_sysintdisable
	#define EXT_sysintdisable
	#define GET_sysintdisable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintdisable(p0) 
	#define CHK_sysintdisable  FALSE
	#define EXP_sysintdisable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintdisable
	#define EXT_sysintdisable
	#define GET_sysintdisable(fl)  CAL_CMGETAPI( "sysintdisable" ) 
	#define CAL_sysintdisable  sysintdisable
	#define CHK_sysintdisable  TRUE
	#define EXP_sysintdisable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisable", (RTS_UINTPTR)sysintdisable, 1, RTSITF_GET_SIGNATURE(0, 0xC9EC050C), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintdisable
	#define EXT_sysintdisable
	#define GET_sysintdisable(fl)  CAL_CMGETAPI( "sysintdisable" ) 
	#define CAL_sysintdisable  sysintdisable
	#define CHK_sysintdisable  TRUE
	#define EXP_sysintdisable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisable", (RTS_UINTPTR)sysintdisable, 1, RTSITF_GET_SIGNATURE(0, 0xC9EC050C), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintdisable
	#define EXT_SysIntsysintdisable
	#define GET_SysIntsysintdisable  ERR_OK
	#define CAL_SysIntsysintdisable  sysintdisable
	#define CHK_SysIntsysintdisable  TRUE
	#define EXP_SysIntsysintdisable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisable", (RTS_UINTPTR)sysintdisable, 1, RTSITF_GET_SIGNATURE(0, 0xC9EC050C), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintdisable
	#define EXT_sysintdisable
	#define GET_sysintdisable(fl)  CAL_CMGETAPI( "sysintdisable" ) 
	#define CAL_sysintdisable  sysintdisable
	#define CHK_sysintdisable  TRUE
	#define EXP_sysintdisable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisable", (RTS_UINTPTR)sysintdisable, 1, RTSITF_GET_SIGNATURE(0, 0xC9EC050C), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintdisable  PFSYSINTDISABLE_IEC pfsysintdisable;
	#define EXT_sysintdisable  extern PFSYSINTDISABLE_IEC pfsysintdisable;
	#define GET_sysintdisable(fl)  s_pfCMGetAPI2( "sysintdisable", (RTS_VOID_FCTPTR *)&pfsysintdisable, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC9EC050C), 0x03050500)
	#define CAL_sysintdisable  pfsysintdisable
	#define CHK_sysintdisable  (pfsysintdisable != NULL)
	#define EXP_sysintdisable   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisable", (RTS_UINTPTR)sysintdisable, 1, RTSITF_GET_SIGNATURE(0, 0xC9EC050C), 0x03050500) 
#endif


/**
 * <description>Function to disable all interrupts. By use of this functions all interrupts are deactivated (masked).
 * A call of SysIntDisableAll has to be succeeded by a call to SysIntEnableAll!</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsysintdisableall_struct
{
	RTS_IEC_UXINT *pulParam;			/* VAR_INPUT */	/* <param name="pulParam" type="IN">Parameter for the target to return the actual masled interrupts to use after in SysIntEnableAll().</param> */
	RTS_IEC_RESULT SysIntDisableAll;	/* VAR_OUTPUT */	
} sysintdisableall_struct;

void CDECL CDECL_EXT sysintdisableall(sysintdisableall_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTDISABLEALL_IEC) (sysintdisableall_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTDISABLEALL_NOTIMPLEMENTED)
	#define USE_sysintdisableall
	#define EXT_sysintdisableall
	#define GET_sysintdisableall(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintdisableall(p0) 
	#define CHK_sysintdisableall  FALSE
	#define EXP_sysintdisableall  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintdisableall
	#define EXT_sysintdisableall
	#define GET_sysintdisableall(fl)  CAL_CMGETAPI( "sysintdisableall" ) 
	#define CAL_sysintdisableall  sysintdisableall
	#define CHK_sysintdisableall  TRUE
	#define EXP_sysintdisableall  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisableall", (RTS_UINTPTR)sysintdisableall, 1, RTSITF_GET_SIGNATURE(0, 0xBE4C0839), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintdisableall
	#define EXT_sysintdisableall
	#define GET_sysintdisableall(fl)  CAL_CMGETAPI( "sysintdisableall" ) 
	#define CAL_sysintdisableall  sysintdisableall
	#define CHK_sysintdisableall  TRUE
	#define EXP_sysintdisableall  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisableall", (RTS_UINTPTR)sysintdisableall, 1, RTSITF_GET_SIGNATURE(0, 0xBE4C0839), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintdisableall
	#define EXT_SysIntsysintdisableall
	#define GET_SysIntsysintdisableall  ERR_OK
	#define CAL_SysIntsysintdisableall  sysintdisableall
	#define CHK_SysIntsysintdisableall  TRUE
	#define EXP_SysIntsysintdisableall  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisableall", (RTS_UINTPTR)sysintdisableall, 1, RTSITF_GET_SIGNATURE(0, 0xBE4C0839), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintdisableall
	#define EXT_sysintdisableall
	#define GET_sysintdisableall(fl)  CAL_CMGETAPI( "sysintdisableall" ) 
	#define CAL_sysintdisableall  sysintdisableall
	#define CHK_sysintdisableall  TRUE
	#define EXP_sysintdisableall  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisableall", (RTS_UINTPTR)sysintdisableall, 1, RTSITF_GET_SIGNATURE(0, 0xBE4C0839), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintdisableall  PFSYSINTDISABLEALL_IEC pfsysintdisableall;
	#define EXT_sysintdisableall  extern PFSYSINTDISABLEALL_IEC pfsysintdisableall;
	#define GET_sysintdisableall(fl)  s_pfCMGetAPI2( "sysintdisableall", (RTS_VOID_FCTPTR *)&pfsysintdisableall, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xBE4C0839), 0x03050500)
	#define CAL_sysintdisableall  pfsysintdisableall
	#define CHK_sysintdisableall  (pfsysintdisableall != NULL)
	#define EXP_sysintdisableall   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintdisableall", (RTS_UINTPTR)sysintdisableall, 1, RTSITF_GET_SIGNATURE(0, 0xBE4C0839), 0x03050500) 
#endif


/**
 * <description>Function to enable an interrupt. By use of this function the interrupt specified by its handle
 * is activated.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsysintenable_struct
{
	RTS_IEC_HANDLE hInt;				/* VAR_INPUT */	/* <param name="hInt" type="IN">Handle to interrupt returned by SysIntOpen.</param> */
	RTS_IEC_RESULT SysIntEnable;		/* VAR_OUTPUT */	
} sysintenable_struct;

void CDECL CDECL_EXT sysintenable(sysintenable_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTENABLE_IEC) (sysintenable_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTENABLE_NOTIMPLEMENTED)
	#define USE_sysintenable
	#define EXT_sysintenable
	#define GET_sysintenable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintenable(p0) 
	#define CHK_sysintenable  FALSE
	#define EXP_sysintenable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintenable
	#define EXT_sysintenable
	#define GET_sysintenable(fl)  CAL_CMGETAPI( "sysintenable" ) 
	#define CAL_sysintenable  sysintenable
	#define CHK_sysintenable  TRUE
	#define EXP_sysintenable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenable", (RTS_UINTPTR)sysintenable, 1, RTSITF_GET_SIGNATURE(0, 0x30131B72), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintenable
	#define EXT_sysintenable
	#define GET_sysintenable(fl)  CAL_CMGETAPI( "sysintenable" ) 
	#define CAL_sysintenable  sysintenable
	#define CHK_sysintenable  TRUE
	#define EXP_sysintenable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenable", (RTS_UINTPTR)sysintenable, 1, RTSITF_GET_SIGNATURE(0, 0x30131B72), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintenable
	#define EXT_SysIntsysintenable
	#define GET_SysIntsysintenable  ERR_OK
	#define CAL_SysIntsysintenable  sysintenable
	#define CHK_SysIntsysintenable  TRUE
	#define EXP_SysIntsysintenable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenable", (RTS_UINTPTR)sysintenable, 1, RTSITF_GET_SIGNATURE(0, 0x30131B72), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintenable
	#define EXT_sysintenable
	#define GET_sysintenable(fl)  CAL_CMGETAPI( "sysintenable" ) 
	#define CAL_sysintenable  sysintenable
	#define CHK_sysintenable  TRUE
	#define EXP_sysintenable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenable", (RTS_UINTPTR)sysintenable, 1, RTSITF_GET_SIGNATURE(0, 0x30131B72), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintenable  PFSYSINTENABLE_IEC pfsysintenable;
	#define EXT_sysintenable  extern PFSYSINTENABLE_IEC pfsysintenable;
	#define GET_sysintenable(fl)  s_pfCMGetAPI2( "sysintenable", (RTS_VOID_FCTPTR *)&pfsysintenable, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x30131B72), 0x03050500)
	#define CAL_sysintenable  pfsysintenable
	#define CHK_sysintenable  (pfsysintenable != NULL)
	#define EXP_sysintenable   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenable", (RTS_UINTPTR)sysintenable, 1, RTSITF_GET_SIGNATURE(0, 0x30131B72), 0x03050500) 
#endif


/**
 * A call to this function will be done after a call of function SysIntDisableAll and  will restore the situation
 * preceding the execution of SysIntDisableAll. That means, all interrupts, having been activated before the call
 * to SysIntDisableAll are activated again.
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsysintenableall_struct
{
	RTS_IEC_UXINT *pulParam;			/* VAR_INPUT */	/* <param name="pulParam" type="IN">Parameter for the target with the masked interrupts that must are deactivated by the
 previous call to SysIntDisableAll().</param> */
	RTS_IEC_RESULT SysIntEnableAll;		/* VAR_OUTPUT */	
} sysintenableall_struct;

void CDECL CDECL_EXT sysintenableall(sysintenableall_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTENABLEALL_IEC) (sysintenableall_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTENABLEALL_NOTIMPLEMENTED)
	#define USE_sysintenableall
	#define EXT_sysintenableall
	#define GET_sysintenableall(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintenableall(p0) 
	#define CHK_sysintenableall  FALSE
	#define EXP_sysintenableall  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintenableall
	#define EXT_sysintenableall
	#define GET_sysintenableall(fl)  CAL_CMGETAPI( "sysintenableall" ) 
	#define CAL_sysintenableall  sysintenableall
	#define CHK_sysintenableall  TRUE
	#define EXP_sysintenableall  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenableall", (RTS_UINTPTR)sysintenableall, 1, RTSITF_GET_SIGNATURE(0, 0xDE565CD4), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintenableall
	#define EXT_sysintenableall
	#define GET_sysintenableall(fl)  CAL_CMGETAPI( "sysintenableall" ) 
	#define CAL_sysintenableall  sysintenableall
	#define CHK_sysintenableall  TRUE
	#define EXP_sysintenableall  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenableall", (RTS_UINTPTR)sysintenableall, 1, RTSITF_GET_SIGNATURE(0, 0xDE565CD4), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintenableall
	#define EXT_SysIntsysintenableall
	#define GET_SysIntsysintenableall  ERR_OK
	#define CAL_SysIntsysintenableall  sysintenableall
	#define CHK_SysIntsysintenableall  TRUE
	#define EXP_SysIntsysintenableall  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenableall", (RTS_UINTPTR)sysintenableall, 1, RTSITF_GET_SIGNATURE(0, 0xDE565CD4), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintenableall
	#define EXT_sysintenableall
	#define GET_sysintenableall(fl)  CAL_CMGETAPI( "sysintenableall" ) 
	#define CAL_sysintenableall  sysintenableall
	#define CHK_sysintenableall  TRUE
	#define EXP_sysintenableall  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenableall", (RTS_UINTPTR)sysintenableall, 1, RTSITF_GET_SIGNATURE(0, 0xDE565CD4), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintenableall  PFSYSINTENABLEALL_IEC pfsysintenableall;
	#define EXT_sysintenableall  extern PFSYSINTENABLEALL_IEC pfsysintenableall;
	#define GET_sysintenableall(fl)  s_pfCMGetAPI2( "sysintenableall", (RTS_VOID_FCTPTR *)&pfsysintenableall, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xDE565CD4), 0x03050500)
	#define CAL_sysintenableall  pfsysintenableall
	#define CHK_sysintenableall  (pfsysintenableall != NULL)
	#define EXP_sysintenableall   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintenableall", (RTS_UINTPTR)sysintenableall, 1, RTSITF_GET_SIGNATURE(0, 0xDE565CD4), 0x03050500) 
#endif


/**
 * <description> 
 * This function checks if the processor is actually executing an ISR. This information is necessary, as some functions
 * of an operating system must not be called within the interrupt level.
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library):
 *	<ul>
 *		<li>ERR_OK: We are still in an interrupt handler</li>
 *		<li>ERR_FAILED: not in interrupt context</li>
 *	</ul>
 * </p></result>
 */
typedef struct tagsysintlevel_struct
{
	RTS_IEC_RESULT SysIntLevel;			/* VAR_OUTPUT */	
} sysintlevel_struct;

void CDECL CDECL_EXT sysintlevel(sysintlevel_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTLEVEL_IEC) (sysintlevel_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTLEVEL_NOTIMPLEMENTED)
	#define USE_sysintlevel
	#define EXT_sysintlevel
	#define GET_sysintlevel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintlevel(p0) 
	#define CHK_sysintlevel  FALSE
	#define EXP_sysintlevel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintlevel
	#define EXT_sysintlevel
	#define GET_sysintlevel(fl)  CAL_CMGETAPI( "sysintlevel" ) 
	#define CAL_sysintlevel  sysintlevel
	#define CHK_sysintlevel  TRUE
	#define EXP_sysintlevel  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintlevel", (RTS_UINTPTR)sysintlevel, 1, RTSITF_GET_SIGNATURE(0, 0xCA219D99), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintlevel
	#define EXT_sysintlevel
	#define GET_sysintlevel(fl)  CAL_CMGETAPI( "sysintlevel" ) 
	#define CAL_sysintlevel  sysintlevel
	#define CHK_sysintlevel  TRUE
	#define EXP_sysintlevel  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintlevel", (RTS_UINTPTR)sysintlevel, 1, RTSITF_GET_SIGNATURE(0, 0xCA219D99), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintlevel
	#define EXT_SysIntsysintlevel
	#define GET_SysIntsysintlevel  ERR_OK
	#define CAL_SysIntsysintlevel  sysintlevel
	#define CHK_SysIntsysintlevel  TRUE
	#define EXP_SysIntsysintlevel  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintlevel", (RTS_UINTPTR)sysintlevel, 1, RTSITF_GET_SIGNATURE(0, 0xCA219D99), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintlevel
	#define EXT_sysintlevel
	#define GET_sysintlevel(fl)  CAL_CMGETAPI( "sysintlevel" ) 
	#define CAL_sysintlevel  sysintlevel
	#define CHK_sysintlevel  TRUE
	#define EXP_sysintlevel  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintlevel", (RTS_UINTPTR)sysintlevel, 1, RTSITF_GET_SIGNATURE(0, 0xCA219D99), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintlevel  PFSYSINTLEVEL_IEC pfsysintlevel;
	#define EXT_sysintlevel  extern PFSYSINTLEVEL_IEC pfsysintlevel;
	#define GET_sysintlevel(fl)  s_pfCMGetAPI2( "sysintlevel", (RTS_VOID_FCTPTR *)&pfsysintlevel, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xCA219D99), 0x03050500)
	#define CAL_sysintlevel  pfsysintlevel
	#define CHK_sysintlevel  (pfsysintlevel != NULL)
	#define EXP_sysintlevel   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintlevel", (RTS_UINTPTR)sysintlevel, 1, RTSITF_GET_SIGNATURE(0, 0xCA219D99), 0x03050500) 
#endif


/**
 * <description>Open a valid interrupt</description>
 * <result><p>RESULT: Handle to the interrupt or RTS_INVALID_HANDLE if failed.</p></result>
 */
typedef struct tagsysintopen_struct
{
	RTS_IEC_UDINT ulInterrupt;			/* VAR_INPUT */	/* <param name="ulInterrupt" type="IN">Interrupt number</param> */
	SYS_INT_DESCRIPTION *pIntDescription;	/* VAR_INPUT */	/* <param name="pIntDescription" type="IN">Pointer to optional interrupt description</param> */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
	RTS_IEC_HANDLE SysIntOpen;			/* VAR_OUTPUT */	
} sysintopen_struct;

void CDECL CDECL_EXT sysintopen(sysintopen_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTOPEN_IEC) (sysintopen_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTOPEN_NOTIMPLEMENTED)
	#define USE_sysintopen
	#define EXT_sysintopen
	#define GET_sysintopen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintopen(p0) 
	#define CHK_sysintopen  FALSE
	#define EXP_sysintopen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintopen
	#define EXT_sysintopen
	#define GET_sysintopen(fl)  CAL_CMGETAPI( "sysintopen" ) 
	#define CAL_sysintopen  sysintopen
	#define CHK_sysintopen  TRUE
	#define EXP_sysintopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopen", (RTS_UINTPTR)sysintopen, 1, RTSITF_GET_SIGNATURE(0, 0xB3D594FF), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintopen
	#define EXT_sysintopen
	#define GET_sysintopen(fl)  CAL_CMGETAPI( "sysintopen" ) 
	#define CAL_sysintopen  sysintopen
	#define CHK_sysintopen  TRUE
	#define EXP_sysintopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopen", (RTS_UINTPTR)sysintopen, 1, RTSITF_GET_SIGNATURE(0, 0xB3D594FF), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintopen
	#define EXT_SysIntsysintopen
	#define GET_SysIntsysintopen  ERR_OK
	#define CAL_SysIntsysintopen  sysintopen
	#define CHK_SysIntsysintopen  TRUE
	#define EXP_SysIntsysintopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopen", (RTS_UINTPTR)sysintopen, 1, RTSITF_GET_SIGNATURE(0, 0xB3D594FF), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintopen
	#define EXT_sysintopen
	#define GET_sysintopen(fl)  CAL_CMGETAPI( "sysintopen" ) 
	#define CAL_sysintopen  sysintopen
	#define CHK_sysintopen  TRUE
	#define EXP_sysintopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopen", (RTS_UINTPTR)sysintopen, 1, RTSITF_GET_SIGNATURE(0, 0xB3D594FF), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintopen  PFSYSINTOPEN_IEC pfsysintopen;
	#define EXT_sysintopen  extern PFSYSINTOPEN_IEC pfsysintopen;
	#define GET_sysintopen(fl)  s_pfCMGetAPI2( "sysintopen", (RTS_VOID_FCTPTR *)&pfsysintopen, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB3D594FF), 0x03050500)
	#define CAL_sysintopen  pfsysintopen
	#define CHK_sysintopen  (pfsysintopen != NULL)
	#define EXP_sysintopen   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopen", (RTS_UINTPTR)sysintopen, 1, RTSITF_GET_SIGNATURE(0, 0xB3D594FF), 0x03050500) 
#endif


/**
 * <description>Open a valid interrupt specified by name</description>
 * <result><p>RESULT: Handle to the interrupt or RTS_INVALID_HANDLE if failed.</p></result>
 */
typedef struct tagsysintopenbyname_struct
{
	RTS_IEC_STRING *pszIntName;			/* VAR_INPUT */	/* <param name="pszIntName" type="IN">Interrupt name defined in the settings component</param> */
	RTS_IEC_RESULT *Result;				/* VAR_INPUT */	/* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
	RTS_IEC_HANDLE SysIntOpenByName;	/* VAR_OUTPUT */	
} sysintopenbyname_struct;

void CDECL CDECL_EXT sysintopenbyname(sysintopenbyname_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTOPENBYNAME_IEC) (sysintopenbyname_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTOPENBYNAME_NOTIMPLEMENTED)
	#define USE_sysintopenbyname
	#define EXT_sysintopenbyname
	#define GET_sysintopenbyname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintopenbyname(p0) 
	#define CHK_sysintopenbyname  FALSE
	#define EXP_sysintopenbyname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintopenbyname
	#define EXT_sysintopenbyname
	#define GET_sysintopenbyname(fl)  CAL_CMGETAPI( "sysintopenbyname" ) 
	#define CAL_sysintopenbyname  sysintopenbyname
	#define CHK_sysintopenbyname  TRUE
	#define EXP_sysintopenbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopenbyname", (RTS_UINTPTR)sysintopenbyname, 1, RTSITF_GET_SIGNATURE(0, 0x928117BD), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintopenbyname
	#define EXT_sysintopenbyname
	#define GET_sysintopenbyname(fl)  CAL_CMGETAPI( "sysintopenbyname" ) 
	#define CAL_sysintopenbyname  sysintopenbyname
	#define CHK_sysintopenbyname  TRUE
	#define EXP_sysintopenbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopenbyname", (RTS_UINTPTR)sysintopenbyname, 1, RTSITF_GET_SIGNATURE(0, 0x928117BD), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintopenbyname
	#define EXT_SysIntsysintopenbyname
	#define GET_SysIntsysintopenbyname  ERR_OK
	#define CAL_SysIntsysintopenbyname  sysintopenbyname
	#define CHK_SysIntsysintopenbyname  TRUE
	#define EXP_SysIntsysintopenbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopenbyname", (RTS_UINTPTR)sysintopenbyname, 1, RTSITF_GET_SIGNATURE(0, 0x928117BD), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintopenbyname
	#define EXT_sysintopenbyname
	#define GET_sysintopenbyname(fl)  CAL_CMGETAPI( "sysintopenbyname" ) 
	#define CAL_sysintopenbyname  sysintopenbyname
	#define CHK_sysintopenbyname  TRUE
	#define EXP_sysintopenbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopenbyname", (RTS_UINTPTR)sysintopenbyname, 1, RTSITF_GET_SIGNATURE(0, 0x928117BD), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintopenbyname  PFSYSINTOPENBYNAME_IEC pfsysintopenbyname;
	#define EXT_sysintopenbyname  extern PFSYSINTOPENBYNAME_IEC pfsysintopenbyname;
	#define GET_sysintopenbyname(fl)  s_pfCMGetAPI2( "sysintopenbyname", (RTS_VOID_FCTPTR *)&pfsysintopenbyname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x928117BD), 0x03050500)
	#define CAL_sysintopenbyname  pfsysintopenbyname
	#define CHK_sysintopenbyname  (pfsysintopenbyname != NULL)
	#define EXP_sysintopenbyname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintopenbyname", (RTS_UINTPTR)sysintopenbyname, 1, RTSITF_GET_SIGNATURE(0, 0x928117BD), 0x03050500) 
#endif


/**
 * <description>Function to register an interrupt handler.
 * By use of this function an interrupt handler (Interrupt Service Routine, ISR) for the interrupt specified by its handle 
 * gets registered. During registration, the start address of the function to be executed in case of the associated interrupt
 * request is filled into the vector table on the target.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsysintregister_struct
{
	RTS_IEC_HANDLE hInt;				/* VAR_INPUT */	/* <param name="hInt" type="IN">Handle to interrupt returned by SysIntOpen.</param> */
	RTS_IEC_BYTE *pCallback;			/* VAR_INPUT */	/* <param name="pCallback" type="IN">Interrupt handler. Callback layout:<br />
	FUNCTION IntHandler : RTS_IEC_RESULT<br />
 VAR_INPUT<br />
		ulAdditionalInfo : UDINT;<br />
	END_VAR
 </param> */
	RTS_IEC_UXINT ulAdditionalInfo;		/* VAR_INPUT */	/* <param name="ulAdditionalInfo" type="IN">Info value that is transmitted to the interrupt handler</param> */
	RTS_IEC_RESULT SysIntRegister;		/* VAR_OUTPUT */	
} sysintregister_struct;

void CDECL CDECL_EXT sysintregister(sysintregister_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTREGISTER_IEC) (sysintregister_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTREGISTER_NOTIMPLEMENTED)
	#define USE_sysintregister
	#define EXT_sysintregister
	#define GET_sysintregister(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintregister(p0) 
	#define CHK_sysintregister  FALSE
	#define EXP_sysintregister  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintregister
	#define EXT_sysintregister
	#define GET_sysintregister(fl)  CAL_CMGETAPI( "sysintregister" ) 
	#define CAL_sysintregister  sysintregister
	#define CHK_sysintregister  TRUE
	#define EXP_sysintregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregister", (RTS_UINTPTR)sysintregister, 1, RTSITF_GET_SIGNATURE(0, 0x63A36B82), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintregister
	#define EXT_sysintregister
	#define GET_sysintregister(fl)  CAL_CMGETAPI( "sysintregister" ) 
	#define CAL_sysintregister  sysintregister
	#define CHK_sysintregister  TRUE
	#define EXP_sysintregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregister", (RTS_UINTPTR)sysintregister, 1, RTSITF_GET_SIGNATURE(0, 0x63A36B82), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintregister
	#define EXT_SysIntsysintregister
	#define GET_SysIntsysintregister  ERR_OK
	#define CAL_SysIntsysintregister  sysintregister
	#define CHK_SysIntsysintregister  TRUE
	#define EXP_SysIntsysintregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregister", (RTS_UINTPTR)sysintregister, 1, RTSITF_GET_SIGNATURE(0, 0x63A36B82), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintregister
	#define EXT_sysintregister
	#define GET_sysintregister(fl)  CAL_CMGETAPI( "sysintregister" ) 
	#define CAL_sysintregister  sysintregister
	#define CHK_sysintregister  TRUE
	#define EXP_sysintregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregister", (RTS_UINTPTR)sysintregister, 1, RTSITF_GET_SIGNATURE(0, 0x63A36B82), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintregister  PFSYSINTREGISTER_IEC pfsysintregister;
	#define EXT_sysintregister  extern PFSYSINTREGISTER_IEC pfsysintregister;
	#define GET_sysintregister(fl)  s_pfCMGetAPI2( "sysintregister", (RTS_VOID_FCTPTR *)&pfsysintregister, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x63A36B82), 0x03050500)
	#define CAL_sysintregister  pfsysintregister
	#define CHK_sysintregister  (pfsysintregister != NULL)
	#define EXP_sysintregister   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregister", (RTS_UINTPTR)sysintregister, 1, RTSITF_GET_SIGNATURE(0, 0x63A36B82), 0x03050500) 
#endif


/**
 * <description>Still unsupported function!</description>
 */
typedef struct tagsysintregisterinstance_struct
{
	RTS_IEC_HANDLE hInt;				/* VAR_INPUT */	
	RTS_IEC_UDINT ulClassId;			/* VAR_INPUT */	
	RTS_IEC_UDINT ulItfId;				/* VAR_INPUT */	
	ISysIntIEC *pInterface;				/* VAR_INPUT */	
	RTS_IEC_UXINT ulAdditionalInfo;		/* VAR_INPUT */	
	RTS_IEC_RESULT SysIntRegisterInstance;	/* VAR_OUTPUT */	
} sysintregisterinstance_struct;

void CDECL CDECL_EXT sysintregisterinstance(sysintregisterinstance_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTREGISTERINSTANCE_IEC) (sysintregisterinstance_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTREGISTERINSTANCE_NOTIMPLEMENTED)
	#define USE_sysintregisterinstance
	#define EXT_sysintregisterinstance
	#define GET_sysintregisterinstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintregisterinstance(p0) 
	#define CHK_sysintregisterinstance  FALSE
	#define EXP_sysintregisterinstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintregisterinstance
	#define EXT_sysintregisterinstance
	#define GET_sysintregisterinstance(fl)  CAL_CMGETAPI( "sysintregisterinstance" ) 
	#define CAL_sysintregisterinstance  sysintregisterinstance
	#define CHK_sysintregisterinstance  TRUE
	#define EXP_sysintregisterinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregisterinstance", (RTS_UINTPTR)sysintregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xC0428DE2), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintregisterinstance
	#define EXT_sysintregisterinstance
	#define GET_sysintregisterinstance(fl)  CAL_CMGETAPI( "sysintregisterinstance" ) 
	#define CAL_sysintregisterinstance  sysintregisterinstance
	#define CHK_sysintregisterinstance  TRUE
	#define EXP_sysintregisterinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregisterinstance", (RTS_UINTPTR)sysintregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xC0428DE2), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintregisterinstance
	#define EXT_SysIntsysintregisterinstance
	#define GET_SysIntsysintregisterinstance  ERR_OK
	#define CAL_SysIntsysintregisterinstance  sysintregisterinstance
	#define CHK_SysIntsysintregisterinstance  TRUE
	#define EXP_SysIntsysintregisterinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregisterinstance", (RTS_UINTPTR)sysintregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xC0428DE2), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintregisterinstance
	#define EXT_sysintregisterinstance
	#define GET_sysintregisterinstance(fl)  CAL_CMGETAPI( "sysintregisterinstance" ) 
	#define CAL_sysintregisterinstance  sysintregisterinstance
	#define CHK_sysintregisterinstance  TRUE
	#define EXP_sysintregisterinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregisterinstance", (RTS_UINTPTR)sysintregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xC0428DE2), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintregisterinstance  PFSYSINTREGISTERINSTANCE_IEC pfsysintregisterinstance;
	#define EXT_sysintregisterinstance  extern PFSYSINTREGISTERINSTANCE_IEC pfsysintregisterinstance;
	#define GET_sysintregisterinstance(fl)  s_pfCMGetAPI2( "sysintregisterinstance", (RTS_VOID_FCTPTR *)&pfsysintregisterinstance, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC0428DE2), 0x03050500)
	#define CAL_sysintregisterinstance  pfsysintregisterinstance
	#define CHK_sysintregisterinstance  (pfsysintregisterinstance != NULL)
	#define EXP_sysintregisterinstance   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintregisterinstance", (RTS_UINTPTR)sysintregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xC0428DE2), 0x03050500) 
#endif


/**
 * <description>Function to unregister an interrupt handler</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsysintunregister_struct
{
	RTS_IEC_HANDLE hInt;				/* VAR_INPUT */	/* <param name="hInt" type="IN">Handle to interrupt returned by SysIntOpen.</param> */
	RTS_IEC_BYTE *pCallback;			/* VAR_INPUT */	/* <param name="pCallback" type="IN">Interrupt handler</param> */
	RTS_IEC_RESULT SysIntUnregister;	/* VAR_OUTPUT */	
} sysintunregister_struct;

void CDECL CDECL_EXT sysintunregister(sysintunregister_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTUNREGISTER_IEC) (sysintunregister_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTUNREGISTER_NOTIMPLEMENTED)
	#define USE_sysintunregister
	#define EXT_sysintunregister
	#define GET_sysintunregister(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintunregister(p0) 
	#define CHK_sysintunregister  FALSE
	#define EXP_sysintunregister  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintunregister
	#define EXT_sysintunregister
	#define GET_sysintunregister(fl)  CAL_CMGETAPI( "sysintunregister" ) 
	#define CAL_sysintunregister  sysintunregister
	#define CHK_sysintunregister  TRUE
	#define EXP_sysintunregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregister", (RTS_UINTPTR)sysintunregister, 1, RTSITF_GET_SIGNATURE(0, 0x9CA9B33A), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintunregister
	#define EXT_sysintunregister
	#define GET_sysintunregister(fl)  CAL_CMGETAPI( "sysintunregister" ) 
	#define CAL_sysintunregister  sysintunregister
	#define CHK_sysintunregister  TRUE
	#define EXP_sysintunregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregister", (RTS_UINTPTR)sysintunregister, 1, RTSITF_GET_SIGNATURE(0, 0x9CA9B33A), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintunregister
	#define EXT_SysIntsysintunregister
	#define GET_SysIntsysintunregister  ERR_OK
	#define CAL_SysIntsysintunregister  sysintunregister
	#define CHK_SysIntsysintunregister  TRUE
	#define EXP_SysIntsysintunregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregister", (RTS_UINTPTR)sysintunregister, 1, RTSITF_GET_SIGNATURE(0, 0x9CA9B33A), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintunregister
	#define EXT_sysintunregister
	#define GET_sysintunregister(fl)  CAL_CMGETAPI( "sysintunregister" ) 
	#define CAL_sysintunregister  sysintunregister
	#define CHK_sysintunregister  TRUE
	#define EXP_sysintunregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregister", (RTS_UINTPTR)sysintunregister, 1, RTSITF_GET_SIGNATURE(0, 0x9CA9B33A), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintunregister  PFSYSINTUNREGISTER_IEC pfsysintunregister;
	#define EXT_sysintunregister  extern PFSYSINTUNREGISTER_IEC pfsysintunregister;
	#define GET_sysintunregister(fl)  s_pfCMGetAPI2( "sysintunregister", (RTS_VOID_FCTPTR *)&pfsysintunregister, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x9CA9B33A), 0x03050500)
	#define CAL_sysintunregister  pfsysintunregister
	#define CHK_sysintunregister  (pfsysintunregister != NULL)
	#define EXP_sysintunregister   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregister", (RTS_UINTPTR)sysintunregister, 1, RTSITF_GET_SIGNATURE(0, 0x9CA9B33A), 0x03050500) 
#endif


/**
 * <description>Still unsupported function!</description>
 */
typedef struct tagsysintunregisterinstance_struct
{
	RTS_IEC_HANDLE hInt;				/* VAR_INPUT */	
	ISysIntIEC *pInterface;				/* VAR_INPUT */	
	RTS_IEC_RESULT SysIntUnregisterInstance;	/* VAR_OUTPUT */	
} sysintunregisterinstance_struct;

void CDECL CDECL_EXT sysintunregisterinstance(sysintunregisterinstance_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSINTUNREGISTERINSTANCE_IEC) (sysintunregisterinstance_struct *p);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTUNREGISTERINSTANCE_NOTIMPLEMENTED)
	#define USE_sysintunregisterinstance
	#define EXT_sysintunregisterinstance
	#define GET_sysintunregisterinstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysintunregisterinstance(p0) 
	#define CHK_sysintunregisterinstance  FALSE
	#define EXP_sysintunregisterinstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysintunregisterinstance
	#define EXT_sysintunregisterinstance
	#define GET_sysintunregisterinstance(fl)  CAL_CMGETAPI( "sysintunregisterinstance" ) 
	#define CAL_sysintunregisterinstance  sysintunregisterinstance
	#define CHK_sysintunregisterinstance  TRUE
	#define EXP_sysintunregisterinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregisterinstance", (RTS_UINTPTR)sysintunregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xA2A67F42), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_sysintunregisterinstance
	#define EXT_sysintunregisterinstance
	#define GET_sysintunregisterinstance(fl)  CAL_CMGETAPI( "sysintunregisterinstance" ) 
	#define CAL_sysintunregisterinstance  sysintunregisterinstance
	#define CHK_sysintunregisterinstance  TRUE
	#define EXP_sysintunregisterinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregisterinstance", (RTS_UINTPTR)sysintunregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xA2A67F42), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntsysintunregisterinstance
	#define EXT_SysIntsysintunregisterinstance
	#define GET_SysIntsysintunregisterinstance  ERR_OK
	#define CAL_SysIntsysintunregisterinstance  sysintunregisterinstance
	#define CHK_SysIntsysintunregisterinstance  TRUE
	#define EXP_SysIntsysintunregisterinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregisterinstance", (RTS_UINTPTR)sysintunregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xA2A67F42), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_sysintunregisterinstance
	#define EXT_sysintunregisterinstance
	#define GET_sysintunregisterinstance(fl)  CAL_CMGETAPI( "sysintunregisterinstance" ) 
	#define CAL_sysintunregisterinstance  sysintunregisterinstance
	#define CHK_sysintunregisterinstance  TRUE
	#define EXP_sysintunregisterinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregisterinstance", (RTS_UINTPTR)sysintunregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xA2A67F42), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_sysintunregisterinstance  PFSYSINTUNREGISTERINSTANCE_IEC pfsysintunregisterinstance;
	#define EXT_sysintunregisterinstance  extern PFSYSINTUNREGISTERINSTANCE_IEC pfsysintunregisterinstance;
	#define GET_sysintunregisterinstance(fl)  s_pfCMGetAPI2( "sysintunregisterinstance", (RTS_VOID_FCTPTR *)&pfsysintunregisterinstance, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xA2A67F42), 0x03050500)
	#define CAL_sysintunregisterinstance  pfsysintunregisterinstance
	#define CHK_sysintunregisterinstance  (pfsysintunregisterinstance != NULL)
	#define EXP_sysintunregisterinstance   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysintunregisterinstance", (RTS_UINTPTR)sysintunregisterinstance, 1, RTSITF_GET_SIGNATURE(0, 0xA2A67F42), 0x03050500) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Open a valid interrupt</description>
 * <param name="ulInterrupt" type="IN" range="[0,VALID_INT_NR,INVALID_INT_NR]">Interrupt number</param>
 * <param name="pIntDescription" type="IN" range="[0,VALID_INT_DESCR,INVALID_INT_DESCR]">Pointer to optional interrupt description</param>
 * <param name="pResult" type="OUT">Pointer to result</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Interrupt could be opened, a valid Handle is returned</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">Interrupt could not be opened, an invalid Handle is returned</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">Invalid ulInterrupt Parameter</errorcode>
 * <result>Handle to interrupt, is RTS_INVALID_HANDLE if error occured, see Errorcodes.</result>
 */
RTS_HANDLE CDECL SysIntOpen(unsigned long ulInterrupt, SYS_INT_DESCRIPTION *pIntDescription, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSINTOPEN) (unsigned long ulInterrupt, SYS_INT_DESCRIPTION *pIntDescription, RTS_RESULT *pResult);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTOPEN_NOTIMPLEMENTED)
	#define USE_SysIntOpen
	#define EXT_SysIntOpen
	#define GET_SysIntOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntOpen(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysIntOpen  FALSE
	#define EXP_SysIntOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntOpen
	#define EXT_SysIntOpen
	#define GET_SysIntOpen(fl)  CAL_CMGETAPI( "SysIntOpen" ) 
	#define CAL_SysIntOpen  SysIntOpen
	#define CHK_SysIntOpen  TRUE
	#define EXP_SysIntOpen  CAL_CMEXPAPI( "SysIntOpen" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntOpen
	#define EXT_SysIntOpen
	#define GET_SysIntOpen(fl)  CAL_CMGETAPI( "SysIntOpen" ) 
	#define CAL_SysIntOpen  SysIntOpen
	#define CHK_SysIntOpen  TRUE
	#define EXP_SysIntOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntOpen", (RTS_UINTPTR)SysIntOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntOpen
	#define EXT_SysIntSysIntOpen
	#define GET_SysIntSysIntOpen  ERR_OK
	#define CAL_SysIntSysIntOpen  ((ISysInt*)s_pfCMCreateInstance(CLASSID_CSysInt, NULL))->ISysIntOpen
	#define CHK_SysIntSysIntOpen	(s_pfCMCreateInstance != NULL && pISysInt != NULL)
	#define EXP_SysIntSysIntOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntOpen
	#define EXT_SysIntOpen
	#define GET_SysIntOpen(fl)  CAL_CMGETAPI( "SysIntOpen" ) 
	#define CAL_SysIntOpen  ((ISysInt*)s_pfCMCreateInstance(CLASSID_CSysInt, NULL))->ISysIntOpen
	#define CHK_SysIntOpen	(s_pfCMCreateInstance != NULL && pISysInt != NULL)
	#define EXP_SysIntOpen  CAL_CMEXPAPI( "SysIntOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntOpen  PFSYSINTOPEN pfSysIntOpen;
	#define EXT_SysIntOpen  extern PFSYSINTOPEN pfSysIntOpen;
	#define GET_SysIntOpen(fl)  s_pfCMGetAPI2( "SysIntOpen", (RTS_VOID_FCTPTR *)&pfSysIntOpen, (fl), 0, 0)
	#define CAL_SysIntOpen  pfSysIntOpen
	#define CHK_SysIntOpen  (pfSysIntOpen != NULL)
	#define EXP_SysIntOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntOpen", (RTS_UINTPTR)SysIntOpen, 0, 0) 
#endif




/**
 * <description>Open a valid interrupt specified by name</description>
 * <param name="pszIntName" type="IN" range="[0,VALID_INT_NAMR,INVALID_INT_NAME]">Interrupt name defined in the settings component</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Interrupt could be opened, a valid Handle is returned</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">Interrupt could not be opened, an invalid Handle is returned</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">Parameter wrong, an invalid Handle is returned</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_OBJECT">No Interrupt found for given Name</errorcode>
 * <param name="pResult" type="OUT">Pointer to result</param>
 * <result>Handle to interrupt</result>
 */
RTS_HANDLE CDECL SysIntOpenByName(char* pszIntName, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFSYSINTOPENBYNAME) (char* pszIntName, RTS_RESULT* pResult);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTOPENBYNAME_NOTIMPLEMENTED)
	#define USE_SysIntOpenByName
	#define EXT_SysIntOpenByName
	#define GET_SysIntOpenByName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntOpenByName(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysIntOpenByName  FALSE
	#define EXP_SysIntOpenByName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntOpenByName
	#define EXT_SysIntOpenByName
	#define GET_SysIntOpenByName(fl)  CAL_CMGETAPI( "SysIntOpenByName" ) 
	#define CAL_SysIntOpenByName  SysIntOpenByName
	#define CHK_SysIntOpenByName  TRUE
	#define EXP_SysIntOpenByName  CAL_CMEXPAPI( "SysIntOpenByName" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntOpenByName
	#define EXT_SysIntOpenByName
	#define GET_SysIntOpenByName(fl)  CAL_CMGETAPI( "SysIntOpenByName" ) 
	#define CAL_SysIntOpenByName  SysIntOpenByName
	#define CHK_SysIntOpenByName  TRUE
	#define EXP_SysIntOpenByName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntOpenByName", (RTS_UINTPTR)SysIntOpenByName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntOpenByName
	#define EXT_SysIntSysIntOpenByName
	#define GET_SysIntSysIntOpenByName  ERR_OK
	#define CAL_SysIntSysIntOpenByName  ((ISysInt*)s_pfCMCreateInstance(CLASSID_CSysInt, NULL))->ISysIntOpenByName
	#define CHK_SysIntSysIntOpenByName	(s_pfCMCreateInstance != NULL && pISysInt != NULL)
	#define EXP_SysIntSysIntOpenByName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntOpenByName
	#define EXT_SysIntOpenByName
	#define GET_SysIntOpenByName(fl)  CAL_CMGETAPI( "SysIntOpenByName" ) 
	#define CAL_SysIntOpenByName  ((ISysInt*)s_pfCMCreateInstance(CLASSID_CSysInt, NULL))->ISysIntOpenByName
	#define CHK_SysIntOpenByName	(s_pfCMCreateInstance != NULL && pISysInt != NULL)
	#define EXP_SysIntOpenByName  CAL_CMEXPAPI( "SysIntOpenByName" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntOpenByName  PFSYSINTOPENBYNAME pfSysIntOpenByName;
	#define EXT_SysIntOpenByName  extern PFSYSINTOPENBYNAME pfSysIntOpenByName;
	#define GET_SysIntOpenByName(fl)  s_pfCMGetAPI2( "SysIntOpenByName", (RTS_VOID_FCTPTR *)&pfSysIntOpenByName, (fl), 0, 0)
	#define CAL_SysIntOpenByName  pfSysIntOpenByName
	#define CHK_SysIntOpenByName  (pfSysIntOpenByName != NULL)
	#define EXP_SysIntOpenByName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntOpenByName", (RTS_UINTPTR)SysIntOpenByName, 0, 0) 
#endif




/**
 * <description>Close an interrupt</description>
 * <param name="hInt" type="IN" range="[RTS_INVALID_HANDLE,0,VALID_INT_HANDLE]">Interrupt handle</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Interrupt could be closed</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Interrupt could not be closed</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_OBJECT">No Interrupt found for Parameter</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysIntClose(RTS_HANDLE hInt);
typedef RTS_RESULT (CDECL * PFSYSINTCLOSE) (RTS_HANDLE hInt);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTCLOSE_NOTIMPLEMENTED)
	#define USE_SysIntClose
	#define EXT_SysIntClose
	#define GET_SysIntClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysIntClose  FALSE
	#define EXP_SysIntClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntClose
	#define EXT_SysIntClose
	#define GET_SysIntClose(fl)  CAL_CMGETAPI( "SysIntClose" ) 
	#define CAL_SysIntClose  SysIntClose
	#define CHK_SysIntClose  TRUE
	#define EXP_SysIntClose  CAL_CMEXPAPI( "SysIntClose" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntClose
	#define EXT_SysIntClose
	#define GET_SysIntClose(fl)  CAL_CMGETAPI( "SysIntClose" ) 
	#define CAL_SysIntClose  SysIntClose
	#define CHK_SysIntClose  TRUE
	#define EXP_SysIntClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntClose", (RTS_UINTPTR)SysIntClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntClose
	#define EXT_SysIntSysIntClose
	#define GET_SysIntSysIntClose  ERR_OK
	#define CAL_SysIntSysIntClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysInt*)p0)->ISysIntClose())
	#define CHK_SysIntSysIntClose  TRUE
	#define EXP_SysIntSysIntClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntClose
	#define EXT_SysIntClose
	#define GET_SysIntClose(fl)  CAL_CMGETAPI( "SysIntClose" ) 
	#define CAL_SysIntClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysInt*)p0)->ISysIntClose())
	#define CHK_SysIntClose  TRUE
	#define EXP_SysIntClose  CAL_CMEXPAPI( "SysIntClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntClose  PFSYSINTCLOSE pfSysIntClose;
	#define EXT_SysIntClose  extern PFSYSINTCLOSE pfSysIntClose;
	#define GET_SysIntClose(fl)  s_pfCMGetAPI2( "SysIntClose", (RTS_VOID_FCTPTR *)&pfSysIntClose, (fl), 0, 0)
	#define CAL_SysIntClose  pfSysIntClose
	#define CHK_SysIntClose  (pfSysIntClose != NULL)
	#define EXP_SysIntClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntClose", (RTS_UINTPTR)SysIntClose, 0, 0) 
#endif




/**
 * <description>Function to enable an interrupt</description>
 * <param name="hInt" type="IN" range="[RTS_INVALID_HANDLE,0,VALID_INT_HANDLE]">Handle to the interrupt</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Interrupt could be enabled</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Interrupt could not be enabled</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_OBJECT">No Interrupt found for Parameter</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysIntEnable(RTS_HANDLE hInt);
typedef RTS_RESULT (CDECL * PFSYSINTENABLE) (RTS_HANDLE hInt);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTENABLE_NOTIMPLEMENTED)
	#define USE_SysIntEnable
	#define EXT_SysIntEnable
	#define GET_SysIntEnable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntEnable(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysIntEnable  FALSE
	#define EXP_SysIntEnable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntEnable
	#define EXT_SysIntEnable
	#define GET_SysIntEnable(fl)  CAL_CMGETAPI( "SysIntEnable" ) 
	#define CAL_SysIntEnable  SysIntEnable
	#define CHK_SysIntEnable  TRUE
	#define EXP_SysIntEnable  CAL_CMEXPAPI( "SysIntEnable" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntEnable
	#define EXT_SysIntEnable
	#define GET_SysIntEnable(fl)  CAL_CMGETAPI( "SysIntEnable" ) 
	#define CAL_SysIntEnable  SysIntEnable
	#define CHK_SysIntEnable  TRUE
	#define EXP_SysIntEnable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntEnable", (RTS_UINTPTR)SysIntEnable, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntEnable
	#define EXT_SysIntSysIntEnable
	#define GET_SysIntSysIntEnable  ERR_OK
	#define CAL_SysIntSysIntEnable(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysInt->ISysIntEnable() : ((ISysInt*)p0)->ISysIntEnable())
	#define CHK_SysIntSysIntEnable  (pISysInt != NULL)
	#define EXP_SysIntSysIntEnable  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntEnable
	#define EXT_SysIntEnable
	#define GET_SysIntEnable(fl)  CAL_CMGETAPI( "SysIntEnable" ) 
	#define CAL_SysIntEnable(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysInt->ISysIntEnable() : ((ISysInt*)p0)->ISysIntEnable())
	#define CHK_SysIntEnable  (pISysInt != NULL)
	#define EXP_SysIntEnable  CAL_CMEXPAPI( "SysIntEnable" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntEnable  PFSYSINTENABLE pfSysIntEnable;
	#define EXT_SysIntEnable  extern PFSYSINTENABLE pfSysIntEnable;
	#define GET_SysIntEnable(fl)  s_pfCMGetAPI2( "SysIntEnable", (RTS_VOID_FCTPTR *)&pfSysIntEnable, (fl), 0, 0)
	#define CAL_SysIntEnable  pfSysIntEnable
	#define CHK_SysIntEnable  (pfSysIntEnable != NULL)
	#define EXP_SysIntEnable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntEnable", (RTS_UINTPTR)SysIntEnable, 0, 0) 
#endif




/**
 * <description>Function to disable an interrupt</description>
 * <param name="hInt" type="IN" range="[RTS_INVALID_HANDLE,0,VALID_INT_HANDLE]">Handle to the interrupt</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Interrupt could be disabled</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Interrupt could not be disabled</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_OBJECT">No Interrupt found for Parameter</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysIntDisable(RTS_HANDLE hInt);
typedef RTS_RESULT (CDECL * PFSYSINTDISABLE) (RTS_HANDLE hInt);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTDISABLE_NOTIMPLEMENTED)
	#define USE_SysIntDisable
	#define EXT_SysIntDisable
	#define GET_SysIntDisable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntDisable(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysIntDisable  FALSE
	#define EXP_SysIntDisable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntDisable
	#define EXT_SysIntDisable
	#define GET_SysIntDisable(fl)  CAL_CMGETAPI( "SysIntDisable" ) 
	#define CAL_SysIntDisable  SysIntDisable
	#define CHK_SysIntDisable  TRUE
	#define EXP_SysIntDisable  CAL_CMEXPAPI( "SysIntDisable" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntDisable
	#define EXT_SysIntDisable
	#define GET_SysIntDisable(fl)  CAL_CMGETAPI( "SysIntDisable" ) 
	#define CAL_SysIntDisable  SysIntDisable
	#define CHK_SysIntDisable  TRUE
	#define EXP_SysIntDisable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntDisable", (RTS_UINTPTR)SysIntDisable, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntDisable
	#define EXT_SysIntSysIntDisable
	#define GET_SysIntSysIntDisable  ERR_OK
	#define CAL_SysIntSysIntDisable(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysInt->ISysIntDisable() : ((ISysInt*)p0)->ISysIntDisable())
	#define CHK_SysIntSysIntDisable  (pISysInt != NULL)
	#define EXP_SysIntSysIntDisable  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntDisable
	#define EXT_SysIntDisable
	#define GET_SysIntDisable(fl)  CAL_CMGETAPI( "SysIntDisable" ) 
	#define CAL_SysIntDisable(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysInt->ISysIntDisable() : ((ISysInt*)p0)->ISysIntDisable())
	#define CHK_SysIntDisable  (pISysInt != NULL)
	#define EXP_SysIntDisable  CAL_CMEXPAPI( "SysIntDisable" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntDisable  PFSYSINTDISABLE pfSysIntDisable;
	#define EXT_SysIntDisable  extern PFSYSINTDISABLE pfSysIntDisable;
	#define GET_SysIntDisable(fl)  s_pfCMGetAPI2( "SysIntDisable", (RTS_VOID_FCTPTR *)&pfSysIntDisable, (fl), 0, 0)
	#define CAL_SysIntDisable  pfSysIntDisable
	#define CHK_SysIntDisable  (pfSysIntDisable != NULL)
	#define EXP_SysIntDisable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntDisable", (RTS_UINTPTR)SysIntDisable, 0, 0) 
#endif




/**
 * <description>Function to register an interrupt handler</description>
 * <param name="hInt" type="IN" range="[RTS_INVALID_HANDLE,0,VALID_INT_HANDLE]">Handle to the interrupt</param>
 * <param name="ulType" type="IN" range="[SYS_INT_NONE,SYS_INT_IEC,SYS_INT_C,INVALID_INT_TYPE]">Type of interrupt handler. See category Interrupt handler type</param>
 * <param name="pHandler" type="IN" range="[0,VALID_INT_HANDLER,INVALID_INT_HANDLER]">Interrupt handler</param>
 * <param name="ulAdditionalInfo" type="IN" range="[0,VALID_INT_ADDINFO,INVALID_INT_ADDINFO]">Info value that is transmitted to the interrupt handler</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Interrupt Handler could be registered</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Interrupt Handler could not be registered</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_OBJECT">No Interrupt found for Parameter, Wrong Type</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">pHandler may not be null</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysIntRegister(RTS_HANDLE hInt, unsigned long ulType, SYS_INT_INTHANDLER pHandler, RTS_UINTPTR ulAdditionalInfo);
typedef RTS_RESULT (CDECL * PFSYSINTREGISTER) (RTS_HANDLE hInt, unsigned long ulType, SYS_INT_INTHANDLER pHandler, RTS_UINTPTR ulAdditionalInfo);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTREGISTER_NOTIMPLEMENTED)
	#define USE_SysIntRegister
	#define EXT_SysIntRegister
	#define GET_SysIntRegister(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntRegister(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysIntRegister  FALSE
	#define EXP_SysIntRegister  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntRegister
	#define EXT_SysIntRegister
	#define GET_SysIntRegister(fl)  CAL_CMGETAPI( "SysIntRegister" ) 
	#define CAL_SysIntRegister  SysIntRegister
	#define CHK_SysIntRegister  TRUE
	#define EXP_SysIntRegister  CAL_CMEXPAPI( "SysIntRegister" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntRegister
	#define EXT_SysIntRegister
	#define GET_SysIntRegister(fl)  CAL_CMGETAPI( "SysIntRegister" ) 
	#define CAL_SysIntRegister  SysIntRegister
	#define CHK_SysIntRegister  TRUE
	#define EXP_SysIntRegister  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntRegister", (RTS_UINTPTR)SysIntRegister, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntRegister
	#define EXT_SysIntSysIntRegister
	#define GET_SysIntSysIntRegister  ERR_OK
	#define CAL_SysIntSysIntRegister(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysInt->ISysIntRegister(p1,p2,p3) : ((ISysInt*)p0)->ISysIntRegister(p1,p2,p3))
	#define CHK_SysIntSysIntRegister  (pISysInt != NULL)
	#define EXP_SysIntSysIntRegister  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntRegister
	#define EXT_SysIntRegister
	#define GET_SysIntRegister(fl)  CAL_CMGETAPI( "SysIntRegister" ) 
	#define CAL_SysIntRegister(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysInt->ISysIntRegister(p1,p2,p3) : ((ISysInt*)p0)->ISysIntRegister(p1,p2,p3))
	#define CHK_SysIntRegister  (pISysInt != NULL)
	#define EXP_SysIntRegister  CAL_CMEXPAPI( "SysIntRegister" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntRegister  PFSYSINTREGISTER pfSysIntRegister;
	#define EXT_SysIntRegister  extern PFSYSINTREGISTER pfSysIntRegister;
	#define GET_SysIntRegister(fl)  s_pfCMGetAPI2( "SysIntRegister", (RTS_VOID_FCTPTR *)&pfSysIntRegister, (fl), 0, 0)
	#define CAL_SysIntRegister  pfSysIntRegister
	#define CHK_SysIntRegister  (pfSysIntRegister != NULL)
	#define EXP_SysIntRegister  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntRegister", (RTS_UINTPTR)SysIntRegister, 0, 0) 
#endif




/**
 * <description>Function to unregister an interrupt handler</description>
 * <param name="hInt" type="IN" range="[RTS_INVALID_HANDLE,0,VALID_INT_HANDLE]">Handle to the interrupt</param>
 * <param name="pHandler" type="IN" range="[0,VALID_INT_HANDLER,INVALID_INT_HANDLER]">Interrupt handler</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Interrupt Handler could be unregistered</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Interrupt Handler could not be unregistered</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_OBJECT">No Interrupt found for hInt</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">pHandler may not be null</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysIntUnregister(RTS_HANDLE hInt, SYS_INT_INTHANDLER pHandler);
typedef RTS_RESULT (CDECL * PFSYSINTUNREGISTER) (RTS_HANDLE hInt, SYS_INT_INTHANDLER pHandler);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTUNREGISTER_NOTIMPLEMENTED)
	#define USE_SysIntUnregister
	#define EXT_SysIntUnregister
	#define GET_SysIntUnregister(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntUnregister(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysIntUnregister  FALSE
	#define EXP_SysIntUnregister  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntUnregister
	#define EXT_SysIntUnregister
	#define GET_SysIntUnregister(fl)  CAL_CMGETAPI( "SysIntUnregister" ) 
	#define CAL_SysIntUnregister  SysIntUnregister
	#define CHK_SysIntUnregister  TRUE
	#define EXP_SysIntUnregister  CAL_CMEXPAPI( "SysIntUnregister" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntUnregister
	#define EXT_SysIntUnregister
	#define GET_SysIntUnregister(fl)  CAL_CMGETAPI( "SysIntUnregister" ) 
	#define CAL_SysIntUnregister  SysIntUnregister
	#define CHK_SysIntUnregister  TRUE
	#define EXP_SysIntUnregister  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntUnregister", (RTS_UINTPTR)SysIntUnregister, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntUnregister
	#define EXT_SysIntSysIntUnregister
	#define GET_SysIntSysIntUnregister  ERR_OK
	#define CAL_SysIntSysIntUnregister(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysInt->ISysIntUnregister(p1) : ((ISysInt*)p0)->ISysIntUnregister(p1))
	#define CHK_SysIntSysIntUnregister  (pISysInt != NULL)
	#define EXP_SysIntSysIntUnregister  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntUnregister
	#define EXT_SysIntUnregister
	#define GET_SysIntUnregister(fl)  CAL_CMGETAPI( "SysIntUnregister" ) 
	#define CAL_SysIntUnregister(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysInt->ISysIntUnregister(p1) : ((ISysInt*)p0)->ISysIntUnregister(p1))
	#define CHK_SysIntUnregister  (pISysInt != NULL)
	#define EXP_SysIntUnregister  CAL_CMEXPAPI( "SysIntUnregister" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntUnregister  PFSYSINTUNREGISTER pfSysIntUnregister;
	#define EXT_SysIntUnregister  extern PFSYSINTUNREGISTER pfSysIntUnregister;
	#define GET_SysIntUnregister(fl)  s_pfCMGetAPI2( "SysIntUnregister", (RTS_VOID_FCTPTR *)&pfSysIntUnregister, (fl), 0, 0)
	#define CAL_SysIntUnregister  pfSysIntUnregister
	#define CHK_SysIntUnregister  (pfSysIntUnregister != NULL)
	#define EXP_SysIntUnregister  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntUnregister", (RTS_UINTPTR)SysIntUnregister, 0, 0) 
#endif




/**
 * <description>Function to enable all interrupts</description>
 * <param name="pulParam" type="IN" range="[NULL,VALID_PARAM,INVALID_PARAM_NONE,INVALID_PARAM_FULL]">Parameter for enabling all interrupts</param>
 * <parampseudo name="bTimerOccured" type="OUT" range="[TRUE,FALSE]">Specifies, if a timer was able to run, during the lock, or not</parampseudo>
 * <parampseudo name="bWatchdogOccured" type="OUT" range="[TRUE,FALSE]">Specifies, if the watchdog handler was able to run, during the lock, or not</parampseudo>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Interrupts could be enabled</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Function is not supported by the system</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysIntEnableAll(RTS_UINTPTR *pulParam);
typedef RTS_RESULT (CDECL * PFSYSINTENABLEALL) (RTS_UINTPTR *pulParam);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTENABLEALL_NOTIMPLEMENTED)
	#define USE_SysIntEnableAll
	#define EXT_SysIntEnableAll
	#define GET_SysIntEnableAll(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntEnableAll(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysIntEnableAll  FALSE
	#define EXP_SysIntEnableAll  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntEnableAll
	#define EXT_SysIntEnableAll
	#define GET_SysIntEnableAll(fl)  CAL_CMGETAPI( "SysIntEnableAll" ) 
	#define CAL_SysIntEnableAll  SysIntEnableAll
	#define CHK_SysIntEnableAll  TRUE
	#define EXP_SysIntEnableAll  CAL_CMEXPAPI( "SysIntEnableAll" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntEnableAll
	#define EXT_SysIntEnableAll
	#define GET_SysIntEnableAll(fl)  CAL_CMGETAPI( "SysIntEnableAll" ) 
	#define CAL_SysIntEnableAll  SysIntEnableAll
	#define CHK_SysIntEnableAll  TRUE
	#define EXP_SysIntEnableAll  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntEnableAll", (RTS_UINTPTR)SysIntEnableAll, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntEnableAll
	#define EXT_SysIntSysIntEnableAll
	#define GET_SysIntSysIntEnableAll  ERR_OK
	#define CAL_SysIntSysIntEnableAll pISysInt->ISysIntEnableAll
	#define CHK_SysIntSysIntEnableAll (pISysInt != NULL)
	#define EXP_SysIntSysIntEnableAll  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntEnableAll
	#define EXT_SysIntEnableAll
	#define GET_SysIntEnableAll(fl)  CAL_CMGETAPI( "SysIntEnableAll" ) 
	#define CAL_SysIntEnableAll pISysInt->ISysIntEnableAll
	#define CHK_SysIntEnableAll (pISysInt != NULL)
	#define EXP_SysIntEnableAll  CAL_CMEXPAPI( "SysIntEnableAll" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntEnableAll  PFSYSINTENABLEALL pfSysIntEnableAll;
	#define EXT_SysIntEnableAll  extern PFSYSINTENABLEALL pfSysIntEnableAll;
	#define GET_SysIntEnableAll(fl)  s_pfCMGetAPI2( "SysIntEnableAll", (RTS_VOID_FCTPTR *)&pfSysIntEnableAll, (fl), 0, 0)
	#define CAL_SysIntEnableAll  pfSysIntEnableAll
	#define CHK_SysIntEnableAll  (pfSysIntEnableAll != NULL)
	#define EXP_SysIntEnableAll  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntEnableAll", (RTS_UINTPTR)SysIntEnableAll, 0, 0) 
#endif




/**
 * <description>Function to disable all interrupts</description>
 * <param name="pulParam" type="INOUT" range="[NULL,VALID_PARAM]">Parameter for disabling all interrupts</param>
 * <parampseudo name="bTimerOccured" type="OUT" range="[TRUE,FALSE]">Specifies, if a timer was able to run, during the lock, or not</parampseudo>
 * <parampseudo name="bWatchdogOccured" type="OUT" range="[TRUE,FALSE]">Specifies, if the watchdog handler was able to run, during the lock, or not</parampseudo>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Interrupts could be disabled</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Function is not supported by the system</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysIntDisableAll(RTS_UINTPTR *pulParam);
typedef RTS_RESULT (CDECL * PFSYSINTDISABLEALL) (RTS_UINTPTR *pulParam);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTDISABLEALL_NOTIMPLEMENTED)
	#define USE_SysIntDisableAll
	#define EXT_SysIntDisableAll
	#define GET_SysIntDisableAll(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntDisableAll(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysIntDisableAll  FALSE
	#define EXP_SysIntDisableAll  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntDisableAll
	#define EXT_SysIntDisableAll
	#define GET_SysIntDisableAll(fl)  CAL_CMGETAPI( "SysIntDisableAll" ) 
	#define CAL_SysIntDisableAll  SysIntDisableAll
	#define CHK_SysIntDisableAll  TRUE
	#define EXP_SysIntDisableAll  CAL_CMEXPAPI( "SysIntDisableAll" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntDisableAll
	#define EXT_SysIntDisableAll
	#define GET_SysIntDisableAll(fl)  CAL_CMGETAPI( "SysIntDisableAll" ) 
	#define CAL_SysIntDisableAll  SysIntDisableAll
	#define CHK_SysIntDisableAll  TRUE
	#define EXP_SysIntDisableAll  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntDisableAll", (RTS_UINTPTR)SysIntDisableAll, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntDisableAll
	#define EXT_SysIntSysIntDisableAll
	#define GET_SysIntSysIntDisableAll  ERR_OK
	#define CAL_SysIntSysIntDisableAll pISysInt->ISysIntDisableAll
	#define CHK_SysIntSysIntDisableAll (pISysInt != NULL)
	#define EXP_SysIntSysIntDisableAll  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntDisableAll
	#define EXT_SysIntDisableAll
	#define GET_SysIntDisableAll(fl)  CAL_CMGETAPI( "SysIntDisableAll" ) 
	#define CAL_SysIntDisableAll pISysInt->ISysIntDisableAll
	#define CHK_SysIntDisableAll (pISysInt != NULL)
	#define EXP_SysIntDisableAll  CAL_CMEXPAPI( "SysIntDisableAll" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntDisableAll  PFSYSINTDISABLEALL pfSysIntDisableAll;
	#define EXT_SysIntDisableAll  extern PFSYSINTDISABLEALL pfSysIntDisableAll;
	#define GET_SysIntDisableAll(fl)  s_pfCMGetAPI2( "SysIntDisableAll", (RTS_VOID_FCTPTR *)&pfSysIntDisableAll, (fl), 0, 0)
	#define CAL_SysIntDisableAll  pfSysIntDisableAll
	#define CHK_SysIntDisableAll  (pfSysIntDisableAll != NULL)
	#define EXP_SysIntDisableAll  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntDisableAll", (RTS_UINTPTR)SysIntDisableAll, 0, 0) 
#endif




/**
 * <description>Function to check, if we are running actual in an interrupt context</description>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">We are still in an interrupt handler</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">not in interrupt context</errorcode>
 * <result>Returns ERR_OK if running in an interrupt handler, ERR_FAILED if not</result>
 */
RTS_RESULT CDECL SysIntLevel(void);
typedef RTS_RESULT (CDECL * PFSYSINTLEVEL) (void);
#if defined(SYSINT_NOTIMPLEMENTED) || defined(SYSINTLEVEL_NOTIMPLEMENTED)
	#define USE_SysIntLevel
	#define EXT_SysIntLevel
	#define GET_SysIntLevel(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysIntLevel()  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysIntLevel  FALSE
	#define EXP_SysIntLevel  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysIntLevel
	#define EXT_SysIntLevel
	#define GET_SysIntLevel(fl)  CAL_CMGETAPI( "SysIntLevel" ) 
	#define CAL_SysIntLevel  SysIntLevel
	#define CHK_SysIntLevel  TRUE
	#define EXP_SysIntLevel  CAL_CMEXPAPI( "SysIntLevel" ) 
#elif defined(MIXED_LINK) && !defined(SYSINT_EXTERNAL)
	#define USE_SysIntLevel
	#define EXT_SysIntLevel
	#define GET_SysIntLevel(fl)  CAL_CMGETAPI( "SysIntLevel" ) 
	#define CAL_SysIntLevel  SysIntLevel
	#define CHK_SysIntLevel  TRUE
	#define EXP_SysIntLevel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntLevel", (RTS_UINTPTR)SysIntLevel, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysIntSysIntLevel
	#define EXT_SysIntSysIntLevel
	#define GET_SysIntSysIntLevel  ERR_OK
	#define CAL_SysIntSysIntLevel pISysInt->ISysIntLevel
	#define CHK_SysIntSysIntLevel (pISysInt != NULL)
	#define EXP_SysIntSysIntLevel  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysIntLevel
	#define EXT_SysIntLevel
	#define GET_SysIntLevel(fl)  CAL_CMGETAPI( "SysIntLevel" ) 
	#define CAL_SysIntLevel pISysInt->ISysIntLevel
	#define CHK_SysIntLevel (pISysInt != NULL)
	#define EXP_SysIntLevel  CAL_CMEXPAPI( "SysIntLevel" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysIntLevel  PFSYSINTLEVEL pfSysIntLevel;
	#define EXT_SysIntLevel  extern PFSYSINTLEVEL pfSysIntLevel;
	#define GET_SysIntLevel(fl)  s_pfCMGetAPI2( "SysIntLevel", (RTS_VOID_FCTPTR *)&pfSysIntLevel, (fl), 0, 0)
	#define CAL_SysIntLevel  pfSysIntLevel
	#define CHK_SysIntLevel  (pfSysIntLevel != NULL)
	#define EXP_SysIntLevel  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysIntLevel", (RTS_UINTPTR)SysIntLevel, 0, 0) 
#endif





#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSINTOPEN ISysIntOpen;
 	PFSYSINTOPENBYNAME ISysIntOpenByName;
 	PFSYSINTCLOSE ISysIntClose;
 	PFSYSINTENABLE ISysIntEnable;
 	PFSYSINTDISABLE ISysIntDisable;
 	PFSYSINTREGISTER ISysIntRegister;
 	PFSYSINTUNREGISTER ISysIntUnregister;
 	PFSYSINTENABLEALL ISysIntEnableAll;
 	PFSYSINTDISABLEALL ISysIntDisableAll;
 	PFSYSINTLEVEL ISysIntLevel;
 } ISysInt_C;

#ifdef CPLUSPLUS
class ISysInt : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISysIntOpen(unsigned long ulInterrupt, SYS_INT_DESCRIPTION *pIntDescription, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysIntOpenByName(char* pszIntName, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL ISysIntClose(void) =0;
		virtual RTS_RESULT CDECL ISysIntEnable(void) =0;
		virtual RTS_RESULT CDECL ISysIntDisable(void) =0;
		virtual RTS_RESULT CDECL ISysIntRegister(unsigned long ulType, SYS_INT_INTHANDLER pHandler, RTS_UINTPTR ulAdditionalInfo) =0;
		virtual RTS_RESULT CDECL ISysIntUnregister(SYS_INT_INTHANDLER pHandler) =0;
		virtual RTS_RESULT CDECL ISysIntEnableAll(RTS_UINTPTR *pulParam) =0;
		virtual RTS_RESULT CDECL ISysIntDisableAll(RTS_UINTPTR *pulParam) =0;
		virtual RTS_RESULT CDECL ISysIntLevel(void) =0;
};
	#ifndef ITF_SysInt
		#define ITF_SysInt static ISysInt *pISysInt = NULL;
	#endif
	#define EXTITF_SysInt
#else	/*CPLUSPLUS*/
	typedef ISysInt_C		ISysInt;
	#ifndef ITF_SysInt
		#define ITF_SysInt
	#endif
	#define EXTITF_SysInt
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSINTITF_H_*/
