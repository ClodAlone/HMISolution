 /**
 * <interfacename>SysCpuHandling</interfacename>
 * <description> 
 *	<p>The SysCpuHandling interface contains all cpu specific routines.</p>
 *	<p>To detect, for which platform the component is compiled, there are special defines that must be set
 *	in sysdefines.h dependant of the compiler specific options (see category "Processor ID" in SysTargetItf.h)</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _SYSCPUHANDLINGITF_H_
#define _SYSCPUHANDLINGITF_H_

#include "CmpStd.h"

 

 




#include "SysExceptItf.h"

/**
 * <category>Settings</category>
 * <description>Obsolete: detection is done automatically by checking the generated code.</description>
 */
#define SYSCPUHANDLING_WINCE7_ARM_SEH				"EnableWinCE7ArmSEH"
#define SYSCPUHANDLING_WINCE7_ARM_SEH_DEFAULT		0

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>for realtime requirements. opens /dev/cpu_dma_latency and writes a zero to it. This will prevent transitions to deep sleep states. Additionally it emulates the idle=poll behavior</description>
 */
#define SYSCPUHANDLINGKEY_INT_LINUX_DISABLE_CPU_DMA_LATENCY				"Linux.DisableCpuDmaLatency"
#define SYSCPUHANDLINGKEY_INT_LINUX_DISABLE_CPU_DMA_LATENCY_DEFAULT		0

/**
 * <category>Static defines</category>
 * <description>Definition of the stack marker to locate the caller of an external library function on the stack</description>
 */
#ifdef TRG_64BIT
	#define SYSCPU_STACK_MARKER		0xCDE1F2CDCDE1F2CD
#else
	#define SYSCPU_STACK_MARKER		0xCDE1F2CD
#endif


/**
 * <category>Platform defines - IEC codegenerator specific!</category>
 * <description>
 *	Stack alignment.
 *	NOTE:
 *	Must match to the following Codegenerator TargetSetting in the DeviceDescription of the target:
 *		<ts:section name="codegenerator">
 *			<ts:section name="memory-layout">
 *				<ts:setting name="stack-alignment" type="integer" access="visible">
 *					<ts:value>4</ts:value>
 *				</ts:setting>
 *			</ts:section>
 *		</ts:section>
 * </description>
 */
#ifndef SYSCPU_STACK_ALIGNMENT
	#if defined(TRG_64BIT)
		#define SYSCPU_STACK_ALIGNMENT					16
	#elif defined(TRG_ARM)
		/* Can be 4 or 8 */
		#define SYSCPU_STACK_ALIGNMENT					4
	#elif defined(TRG_CORTEX)
		/* Can be 4 or 8 */
		#define SYSCPU_STACK_ALIGNMENT					4
	#elif defined(TRG_PPC)
		/* Can be 4 or 8 */
		#define SYSCPU_STACK_ALIGNMENT					4
	#elif defined(TRG_MIPS)
		/* Can be 4 or 8 */
		#define SYSCPU_STACK_ALIGNMENT					4
	#elif defined(TRG_SH)
		#define SYSCPU_STACK_ALIGNMENT					4
	#else
		#define SYSCPU_STACK_ALIGNMENT					4
	#endif
#endif


/**
 * <category>Platform defines - IEC codegenerator specific!</category>
 * <description>
 *	Size of the register save area in number of saved registers! Is used for locating the return address
 *	of an external library call in SysCpuGetCallstackEntry2().
 * </description>
 */
#ifndef SYSCPU_EXTLIBCALL_REGISTER_SAVE_AREA
	#if defined(TRG_X86)
		#define SYSCPU_EXTLIBCALL_REGISTER_SAVE_AREA	4
	#elif defined(TRG_ARM)
		#define SYSCPU_EXTLIBCALL_REGISTER_SAVE_AREA	1
	#elif defined(TRG_CORTEX)
		#define SYSCPU_EXTLIBCALL_REGISTER_SAVE_AREA	0
	#elif defined(TRG_PPC)
		#define SYSCPU_EXTLIBCALL_REGISTER_SAVE_AREA	8
	#elif defined(TRG_MIPS)
		#define SYSCPU_EXTLIBCALL_REGISTER_SAVE_AREA	4
	#elif defined(TRG_SH)
		#define SYSCPU_EXTLIBCALL_REGISTER_SAVE_AREA	2
	#else
		#define SYSCPU_EXTLIBCALL_REGISTER_SAVE_AREA	0
	#endif
#endif


/**
 * <category>Platform defines - IEC codegenerator specific!</category>
 * <description>
 *	Processor specific adjustment of offset for stack examination. 
 *  Is used for locating the return address of an external library 
 *  call in SysCpuGetCallstackEntry2().
 * </description>
 */
#ifndef SYSCPU_EXTLIBCALL_ADJUST_OFFSET
		#define SYSCPU_EXTLIBCALL_ADJUST_OFFSET(offset) 	(offset)
#endif


/**
 * <category>Platform defines</category>
 * <description>Nop opcode definitions. Is used by CmpSchedule to detect the breakpoint position after external library call to suspend task!</description>							  
 */
#ifndef SYSCPU_NOP_OPCODE
	#if defined(TRG_X86)
		/* x86: 0x90 */
		#define SYSCPU_NOP_OPCODE			{0x90}
		#define SYSCPU_NOP_OPCODE_SIZE		1
	#elif defined(TRG_ARM)
		/* ARM: 0x60000000 */
		#define SYSCPU_NOP_OPCODE			{0x00,0x00,0x00,0x60}
		#define SYSCPU_NOP_OPCODE_SIZE		4
	#elif defined(TRG_CORTEX)
		/* CortexM3:  0xBF00 */
		#define SYSCPU_NOP_OPCODE			{0x00,0xBF}
		#define SYSCPU_NOP_OPCODE_SIZE		2
	#elif defined(TRG_PPC)
		/* PPC: 0x60000000 */
		#define SYSCPU_NOP_OPCODE			{0x60,0x00,0x00,0x00}
		#define SYSCPU_NOP_OPCODE_SIZE		4
	#elif defined(TRG_MIPS)
		/* PPC: 0x00000000 */
		#define SYSCPU_NOP_OPCODE			{0x00,0x00,0x00,0x60}
		#define SYSCPU_NOP_OPCODE_SIZE		4
	#elif defined(TRG_SH)
		/* SH: 0x0009 */
		#define SYSCPU_NOP_OPCODE			{0x09,0x00}
		#define SYSCPU_NOP_OPCODE_SIZE		2
	#else
		#define SYSCPU_NOP_OPCODE			{0x00}
		#define SYSCPU_NOP_OPCODE_SIZE		0
	#endif
#endif


/**
 * <category>Platform defines</category>	
 * <description>
 *	Alignment of 2-byte data types will be checked and handled according to this define.
 * </description> 					  
 */
#ifndef SYSCPU_ALIGNMENT_CHECK_16
	#ifdef TRG_X86
		#define SYSCPU_ALIGNMENT_CHECK_16 1
	#else
		#define SYSCPU_ALIGNMENT_CHECK_16 2
	#endif
#endif


/**
 * <category>Platform defines</category>	
 * <description>
 *	Alignment of 4-byte data types will be checked and handled according to this define.
 * </description> 					  
 */
#ifndef SYSCPU_ALIGNMENT_CHECK_32
	#ifdef TRG_X86
		#define SYSCPU_ALIGNMENT_CHECK_32 1
	#else
		#define SYSCPU_ALIGNMENT_CHECK_32 4
	#endif
#endif


/**
 * <category>Platform defines</category>	
 * <description>
 *	Alignment of 8-byte data types will be checked and handled according to this define.
 * </description> 					  
 */
#ifndef SYSCPU_ALIGNMENT_CHECK_64
	#ifdef TRG_X86
		#define SYSCPU_ALIGNMENT_CHECK_64 1
	#else
		#define SYSCPU_ALIGNMENT_CHECK_64 8
	#endif
#endif


/**
 * <category>Register Offsets</category>
 * <description>
 *	The syscpudebughandler function stores the register context to stack on enty and calls
 *	AppDebugHandler2() function with the register context, which is system specific.
 *	According to the currently used CPU one of those stored registers, which is addressed
 *	by register number, is tried to be set in the SysCpuSetRegisterValue() function.
 *	Depending to the system adaptation and the syscpudebughandler implementation, the 
 *	register context may have different layouts and the offset to the given register number
 *	may vary and this macro may be used to specify the pointer size wide offset to the 
 *	given register number in the stored register context. 
 * </description>
 */
#ifndef SYSCPU_REGISTER_OFFSET
	#define SYSCPU_REGISTER_OFFSET(regNumber)	(regNumber)
#endif

/** EXTERN LIB SECTION BEGIN **/

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 *  Function to increment the content of the given pointer by nSum in one atomic operation (task safe).
 *  IMPLEMENTATION NOTE:
 *  - Add/Sub the value to the content of the pointer
 *  - Return the value after the Add/Sub operation
 *  Both things must be done atomic!
 * </description>
 * <result><p>RESULT: Returns the value after the increment operation in an atomic way!
 * </p></result>
 * <SIL2/>
 */
typedef struct tagsyscpuatomicadd_struct
{
	RTS_IEC_DINT *piValue;				/* VAR_INPUT */	/* <param name="piValue" type="INOUT">Pointer to the value to increment</param> */
	RTS_IEC_DINT nSum;					/* VAR_INPUT */	/* <param name="nSum" type="IN">Summand for the operation. &gt;0 to increment, &lt;0 to decrement</param> */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
	RTS_IEC_DINT SysCpuAtomicAdd;		/* VAR_OUTPUT */	
} syscpuatomicadd_struct;

void CDECL CDECL_EXT syscpuatomicadd(syscpuatomicadd_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCPUATOMICADD_IEC) (syscpuatomicadd_struct *p);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUATOMICADD_NOTIMPLEMENTED)
	#define USE_syscpuatomicadd
	#define EXT_syscpuatomicadd
	#define GET_syscpuatomicadd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscpuatomicadd(p0) 
	#define CHK_syscpuatomicadd  FALSE
	#define EXP_syscpuatomicadd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscpuatomicadd
	#define EXT_syscpuatomicadd
	#define GET_syscpuatomicadd(fl)  CAL_CMGETAPI( "syscpuatomicadd" ) 
	#define CAL_syscpuatomicadd  syscpuatomicadd
	#define CHK_syscpuatomicadd  TRUE
	#define EXP_syscpuatomicadd  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd", (RTS_UINTPTR)syscpuatomicadd, 1, 0x414E4706, 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_syscpuatomicadd
	#define EXT_syscpuatomicadd
	#define GET_syscpuatomicadd(fl)  CAL_CMGETAPI( "syscpuatomicadd" ) 
	#define CAL_syscpuatomicadd  syscpuatomicadd
	#define CHK_syscpuatomicadd  TRUE
	#define EXP_syscpuatomicadd  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd", (RTS_UINTPTR)syscpuatomicadd, 1, 0x414E4706, 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingsyscpuatomicadd
	#define EXT_SysCpuHandlingsyscpuatomicadd
	#define GET_SysCpuHandlingsyscpuatomicadd  ERR_OK
	#define CAL_SysCpuHandlingsyscpuatomicadd  syscpuatomicadd
	#define CHK_SysCpuHandlingsyscpuatomicadd  TRUE
	#define EXP_SysCpuHandlingsyscpuatomicadd  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd", (RTS_UINTPTR)syscpuatomicadd, 1, 0x414E4706, 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_syscpuatomicadd
	#define EXT_syscpuatomicadd
	#define GET_syscpuatomicadd(fl)  CAL_CMGETAPI( "syscpuatomicadd" ) 
	#define CAL_syscpuatomicadd  syscpuatomicadd
	#define CHK_syscpuatomicadd  TRUE
	#define EXP_syscpuatomicadd  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd", (RTS_UINTPTR)syscpuatomicadd, 1, 0x414E4706, 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_syscpuatomicadd  PFSYSCPUATOMICADD_IEC pfsyscpuatomicadd;
	#define EXT_syscpuatomicadd  extern PFSYSCPUATOMICADD_IEC pfsyscpuatomicadd;
	#define GET_syscpuatomicadd(fl)  s_pfCMGetAPI2( "syscpuatomicadd", (RTS_VOID_FCTPTR *)&pfsyscpuatomicadd, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x414E4706, 0x03050D00)
	#define CAL_syscpuatomicadd  pfsyscpuatomicadd
	#define CHK_syscpuatomicadd  (pfsyscpuatomicadd != NULL)
	#define EXP_syscpuatomicadd   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd", (RTS_UINTPTR)syscpuatomicadd, 1, 0x414E4706, 0x03050D00) 
#endif


/**
 * <description>
 *  Function to increment the content of the given pointer by nSum in one atomic operation (task safe).
 *  IMPLEMENTATION NOTE:
 *  - Add/Sub the value to the content of the pointer
 *  - Return the value after the Add/Sub operation
 *  Both things must be done atomic!
 * </description>
 * <result><p>RESULT: Returns the value after the increment operation in an atomic way!
 * </p></result>
 * <SIL2/>
 */
typedef struct tagsyscpuatomicadd64_struct
{
	RTS_IEC_LINT *piValue;				/* VAR_INPUT */	/* <param name="piValue" type="INOUT">Pointer to the value to increment</param> */
	RTS_IEC_LINT nSum;					/* VAR_INPUT */	/* <param name="nSum" type="IN">Summand for the operation. &gt;0 to increment, &lt;0 to decrement</param> */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
	RTS_IEC_LINT SysCpuAtomicAdd64;		/* VAR_OUTPUT */	
} syscpuatomicadd64_struct;

void CDECL CDECL_EXT syscpuatomicadd64(syscpuatomicadd64_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCPUATOMICADD64_IEC) (syscpuatomicadd64_struct *p);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUATOMICADD64_NOTIMPLEMENTED)
	#define USE_syscpuatomicadd64
	#define EXT_syscpuatomicadd64
	#define GET_syscpuatomicadd64(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscpuatomicadd64(p0) 
	#define CHK_syscpuatomicadd64  FALSE
	#define EXP_syscpuatomicadd64  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscpuatomicadd64
	#define EXT_syscpuatomicadd64
	#define GET_syscpuatomicadd64(fl)  CAL_CMGETAPI( "syscpuatomicadd64" ) 
	#define CAL_syscpuatomicadd64  syscpuatomicadd64
	#define CHK_syscpuatomicadd64  TRUE
	#define EXP_syscpuatomicadd64  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd64", (RTS_UINTPTR)syscpuatomicadd64, 1, 0xA535E8A0, 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_syscpuatomicadd64
	#define EXT_syscpuatomicadd64
	#define GET_syscpuatomicadd64(fl)  CAL_CMGETAPI( "syscpuatomicadd64" ) 
	#define CAL_syscpuatomicadd64  syscpuatomicadd64
	#define CHK_syscpuatomicadd64  TRUE
	#define EXP_syscpuatomicadd64  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd64", (RTS_UINTPTR)syscpuatomicadd64, 1, 0xA535E8A0, 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingsyscpuatomicadd64
	#define EXT_SysCpuHandlingsyscpuatomicadd64
	#define GET_SysCpuHandlingsyscpuatomicadd64  ERR_OK
	#define CAL_SysCpuHandlingsyscpuatomicadd64  syscpuatomicadd64
	#define CHK_SysCpuHandlingsyscpuatomicadd64  TRUE
	#define EXP_SysCpuHandlingsyscpuatomicadd64  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd64", (RTS_UINTPTR)syscpuatomicadd64, 1, 0xA535E8A0, 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_syscpuatomicadd64
	#define EXT_syscpuatomicadd64
	#define GET_syscpuatomicadd64(fl)  CAL_CMGETAPI( "syscpuatomicadd64" ) 
	#define CAL_syscpuatomicadd64  syscpuatomicadd64
	#define CHK_syscpuatomicadd64  TRUE
	#define EXP_syscpuatomicadd64  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd64", (RTS_UINTPTR)syscpuatomicadd64, 1, 0xA535E8A0, 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_syscpuatomicadd64  PFSYSCPUATOMICADD64_IEC pfsyscpuatomicadd64;
	#define EXT_syscpuatomicadd64  extern PFSYSCPUATOMICADD64_IEC pfsyscpuatomicadd64;
	#define GET_syscpuatomicadd64(fl)  s_pfCMGetAPI2( "syscpuatomicadd64", (RTS_VOID_FCTPTR *)&pfsyscpuatomicadd64, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xA535E8A0, 0x03050D00)
	#define CAL_syscpuatomicadd64  pfsyscpuatomicadd64
	#define CHK_syscpuatomicadd64  (pfsyscpuatomicadd64 != NULL)
	#define EXP_syscpuatomicadd64   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomicadd64", (RTS_UINTPTR)syscpuatomicadd64, 1, 0xA535E8A0, 0x03050D00) 
#endif


/**
 * <description>
 * Function for compare-and-swap in one atomic operation (task safe).
 * Supports only 32- and 64-bit types 
 * </description>
 * <result><p>RESULT: Returns ERR_OK if the swap was successful
 * </p></result>
 * <SIL2/>
 */
typedef struct tagsyscpuatomiccompareandswap_struct
{
	RTS_IEC_BYTE *pAddress;				/* VAR_INPUT */	/* pointer to the value to test */
	RTS_IEC_BYTE *pSwapValue;			/* VAR_INPUT */	/* pointer to new valie */
	RTS_IEC_BYTE *pCompareValue;		/* VAR_INPUT */	/* pointer to compare value */
	RTS_IEC_USINT valueSize;			/* VAR_INPUT */	/* size of the value */
	RTS_IEC_RESULT SysCpuAtomicCompareAndSwap;	/* VAR_OUTPUT */	
} syscpuatomiccompareandswap_struct;

void CDECL CDECL_EXT syscpuatomiccompareandswap(syscpuatomiccompareandswap_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCPUATOMICCOMPAREANDSWAP_IEC) (syscpuatomiccompareandswap_struct *p);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUATOMICCOMPAREANDSWAP_NOTIMPLEMENTED)
	#define USE_syscpuatomiccompareandswap
	#define EXT_syscpuatomiccompareandswap
	#define GET_syscpuatomiccompareandswap(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscpuatomiccompareandswap(p0) 
	#define CHK_syscpuatomiccompareandswap  FALSE
	#define EXP_syscpuatomiccompareandswap  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscpuatomiccompareandswap
	#define EXT_syscpuatomiccompareandswap
	#define GET_syscpuatomiccompareandswap(fl)  CAL_CMGETAPI( "syscpuatomiccompareandswap" ) 
	#define CAL_syscpuatomiccompareandswap  syscpuatomiccompareandswap
	#define CHK_syscpuatomiccompareandswap  TRUE
	#define EXP_syscpuatomiccompareandswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomiccompareandswap", (RTS_UINTPTR)syscpuatomiccompareandswap, 1, 0xCBB4ED00, 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_syscpuatomiccompareandswap
	#define EXT_syscpuatomiccompareandswap
	#define GET_syscpuatomiccompareandswap(fl)  CAL_CMGETAPI( "syscpuatomiccompareandswap" ) 
	#define CAL_syscpuatomiccompareandswap  syscpuatomiccompareandswap
	#define CHK_syscpuatomiccompareandswap  TRUE
	#define EXP_syscpuatomiccompareandswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomiccompareandswap", (RTS_UINTPTR)syscpuatomiccompareandswap, 1, 0xCBB4ED00, 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingsyscpuatomiccompareandswap
	#define EXT_SysCpuHandlingsyscpuatomiccompareandswap
	#define GET_SysCpuHandlingsyscpuatomiccompareandswap  ERR_OK
	#define CAL_SysCpuHandlingsyscpuatomiccompareandswap  syscpuatomiccompareandswap
	#define CHK_SysCpuHandlingsyscpuatomiccompareandswap  TRUE
	#define EXP_SysCpuHandlingsyscpuatomiccompareandswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomiccompareandswap", (RTS_UINTPTR)syscpuatomiccompareandswap, 1, 0xCBB4ED00, 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_syscpuatomiccompareandswap
	#define EXT_syscpuatomiccompareandswap
	#define GET_syscpuatomiccompareandswap(fl)  CAL_CMGETAPI( "syscpuatomiccompareandswap" ) 
	#define CAL_syscpuatomiccompareandswap  syscpuatomiccompareandswap
	#define CHK_syscpuatomiccompareandswap  TRUE
	#define EXP_syscpuatomiccompareandswap  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomiccompareandswap", (RTS_UINTPTR)syscpuatomiccompareandswap, 1, 0xCBB4ED00, 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_syscpuatomiccompareandswap  PFSYSCPUATOMICCOMPAREANDSWAP_IEC pfsyscpuatomiccompareandswap;
	#define EXT_syscpuatomiccompareandswap  extern PFSYSCPUATOMICCOMPAREANDSWAP_IEC pfsyscpuatomiccompareandswap;
	#define GET_syscpuatomiccompareandswap(fl)  s_pfCMGetAPI2( "syscpuatomiccompareandswap", (RTS_VOID_FCTPTR *)&pfsyscpuatomiccompareandswap, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xCBB4ED00, 0x03050D00)
	#define CAL_syscpuatomiccompareandswap  pfsyscpuatomiccompareandswap
	#define CHK_syscpuatomiccompareandswap  (pfsyscpuatomiccompareandswap != NULL)
	#define EXP_syscpuatomiccompareandswap   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpuatomiccompareandswap", (RTS_UINTPTR)syscpuatomiccompareandswap, 1, 0xCBB4ED00, 0x03050D00) 
#endif


/**
 * Call an IEC function from plain C code.
 * Since different CPU's/systems use different calling conventions, this function 
 * should be used as a wrapper.
 * IEC functions or methods of function block use all the same calling convention:
 * They have no return value and exactly one parameter, which is a pointer to a struct that contains all required
 * IN and OUT parameters.
 * RETURN: Returns the runtime system error code (see CmpErrors.library)
 * <SIL2/>
 */
typedef struct tagsyscpucalliecfuncwithparams_struct
{
	RTS_IEC_BYTE *pfIECFunc;			/* VAR_INPUT */	/* Pointer to the IEC function that should be called */
	RTS_IEC_BYTE *pParam;				/* VAR_INPUT */	/* Pointer to the parameter struct that contains the function parameters. Can be 0 if not used. */
	RTS_IEC_UDINT ulSize;				/* VAR_INPUT */	/* Size of the parameter structure to copy the content on stack. Can be 0. */
	RTS_IEC_RESULT SysCpuCallIecFuncWithParams;	/* VAR_OUTPUT */	
} syscpucalliecfuncwithparams_struct;

void CDECL CDECL_EXT syscpucalliecfuncwithparams(syscpucalliecfuncwithparams_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCPUCALLIECFUNCWITHPARAMS_IEC) (syscpucalliecfuncwithparams_struct *p);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUCALLIECFUNCWITHPARAMS_NOTIMPLEMENTED)
	#define USE_syscpucalliecfuncwithparams
	#define EXT_syscpucalliecfuncwithparams
	#define GET_syscpucalliecfuncwithparams(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscpucalliecfuncwithparams(p0) 
	#define CHK_syscpucalliecfuncwithparams  FALSE
	#define EXP_syscpucalliecfuncwithparams  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscpucalliecfuncwithparams
	#define EXT_syscpucalliecfuncwithparams
	#define GET_syscpucalliecfuncwithparams(fl)  CAL_CMGETAPI( "syscpucalliecfuncwithparams" ) 
	#define CAL_syscpucalliecfuncwithparams  syscpucalliecfuncwithparams
	#define CHK_syscpucalliecfuncwithparams  TRUE
	#define EXP_syscpucalliecfuncwithparams  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpucalliecfuncwithparams", (RTS_UINTPTR)syscpucalliecfuncwithparams, 1, RTSITF_GET_SIGNATURE(0, 0x837FB349), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_syscpucalliecfuncwithparams
	#define EXT_syscpucalliecfuncwithparams
	#define GET_syscpucalliecfuncwithparams(fl)  CAL_CMGETAPI( "syscpucalliecfuncwithparams" ) 
	#define CAL_syscpucalliecfuncwithparams  syscpucalliecfuncwithparams
	#define CHK_syscpucalliecfuncwithparams  TRUE
	#define EXP_syscpucalliecfuncwithparams  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpucalliecfuncwithparams", (RTS_UINTPTR)syscpucalliecfuncwithparams, 1, RTSITF_GET_SIGNATURE(0, 0x837FB349), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingsyscpucalliecfuncwithparams
	#define EXT_SysCpuHandlingsyscpucalliecfuncwithparams
	#define GET_SysCpuHandlingsyscpucalliecfuncwithparams  ERR_OK
	#define CAL_SysCpuHandlingsyscpucalliecfuncwithparams  syscpucalliecfuncwithparams
	#define CHK_SysCpuHandlingsyscpucalliecfuncwithparams  TRUE
	#define EXP_SysCpuHandlingsyscpucalliecfuncwithparams  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpucalliecfuncwithparams", (RTS_UINTPTR)syscpucalliecfuncwithparams, 1, RTSITF_GET_SIGNATURE(0, 0x837FB349), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_syscpucalliecfuncwithparams
	#define EXT_syscpucalliecfuncwithparams
	#define GET_syscpucalliecfuncwithparams(fl)  CAL_CMGETAPI( "syscpucalliecfuncwithparams" ) 
	#define CAL_syscpucalliecfuncwithparams  syscpucalliecfuncwithparams
	#define CHK_syscpucalliecfuncwithparams  TRUE
	#define EXP_syscpucalliecfuncwithparams  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpucalliecfuncwithparams", (RTS_UINTPTR)syscpucalliecfuncwithparams, 1, RTSITF_GET_SIGNATURE(0, 0x837FB349), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_syscpucalliecfuncwithparams  PFSYSCPUCALLIECFUNCWITHPARAMS_IEC pfsyscpucalliecfuncwithparams;
	#define EXT_syscpucalliecfuncwithparams  extern PFSYSCPUCALLIECFUNCWITHPARAMS_IEC pfsyscpucalliecfuncwithparams;
	#define GET_syscpucalliecfuncwithparams(fl)  s_pfCMGetAPI2( "syscpucalliecfuncwithparams", (RTS_VOID_FCTPTR *)&pfsyscpucalliecfuncwithparams, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x837FB349), 0x03050500)
	#define CAL_syscpucalliecfuncwithparams  pfsyscpucalliecfuncwithparams
	#define CHK_syscpucalliecfuncwithparams  (pfsyscpucalliecfuncwithparams != NULL)
	#define EXP_syscpucalliecfuncwithparams   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscpucalliecfuncwithparams", (RTS_UINTPTR)syscpucalliecfuncwithparams, 1, RTSITF_GET_SIGNATURE(0, 0x837FB349), 0x03050500) 
#endif


/**
 * <description>
 *  Test and reset a bit in an ULONG variable in one processor step. This operation is to provide
 *	 a multitasking save operation.
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).
 *	ERR_OK: If bit could be reset and was set before,
 *	ERR_FAILED: If bit is still reset
 * </p></result>
 */
typedef struct tagsyscputestandreset_struct
{
	RTS_IEC_UDINT *pulValue;			/* VAR_INPUT */	/* <param name="pulValue" type="IN">Pointer to the unsigned value to test and reset a bit inside in one atomic
	processor step</param> */
	RTS_IEC_UDINT ulBit;				/* VAR_INPUT */	/* <param name="ulBit" type="IN">Bit number inside the variable to test and reset. 0=first bit, 31=last bit</param> */
	RTS_IEC_RESULT SysCpuTestAndReset;	/* VAR_OUTPUT */	
} syscputestandreset_struct;

void CDECL CDECL_EXT syscputestandreset(syscputestandreset_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCPUTESTANDRESET_IEC) (syscputestandreset_struct *p);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUTESTANDRESET_NOTIMPLEMENTED)
	#define USE_syscputestandreset
	#define EXT_syscputestandreset
	#define GET_syscputestandreset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscputestandreset(p0) 
	#define CHK_syscputestandreset  FALSE
	#define EXP_syscputestandreset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscputestandreset
	#define EXT_syscputestandreset
	#define GET_syscputestandreset(fl)  CAL_CMGETAPI( "syscputestandreset" ) 
	#define CAL_syscputestandreset  syscputestandreset
	#define CHK_syscputestandreset  TRUE
	#define EXP_syscputestandreset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandreset", (RTS_UINTPTR)syscputestandreset, 1, RTSITF_GET_SIGNATURE(0, 0xCD8E165F), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_syscputestandreset
	#define EXT_syscputestandreset
	#define GET_syscputestandreset(fl)  CAL_CMGETAPI( "syscputestandreset" ) 
	#define CAL_syscputestandreset  syscputestandreset
	#define CHK_syscputestandreset  TRUE
	#define EXP_syscputestandreset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandreset", (RTS_UINTPTR)syscputestandreset, 1, RTSITF_GET_SIGNATURE(0, 0xCD8E165F), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingsyscputestandreset
	#define EXT_SysCpuHandlingsyscputestandreset
	#define GET_SysCpuHandlingsyscputestandreset  ERR_OK
	#define CAL_SysCpuHandlingsyscputestandreset  syscputestandreset
	#define CHK_SysCpuHandlingsyscputestandreset  TRUE
	#define EXP_SysCpuHandlingsyscputestandreset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandreset", (RTS_UINTPTR)syscputestandreset, 1, RTSITF_GET_SIGNATURE(0, 0xCD8E165F), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_syscputestandreset
	#define EXT_syscputestandreset
	#define GET_syscputestandreset(fl)  CAL_CMGETAPI( "syscputestandreset" ) 
	#define CAL_syscputestandreset  syscputestandreset
	#define CHK_syscputestandreset  TRUE
	#define EXP_syscputestandreset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandreset", (RTS_UINTPTR)syscputestandreset, 1, RTSITF_GET_SIGNATURE(0, 0xCD8E165F), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_syscputestandreset  PFSYSCPUTESTANDRESET_IEC pfsyscputestandreset;
	#define EXT_syscputestandreset  extern PFSYSCPUTESTANDRESET_IEC pfsyscputestandreset;
	#define GET_syscputestandreset(fl)  s_pfCMGetAPI2( "syscputestandreset", (RTS_VOID_FCTPTR *)&pfsyscputestandreset, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xCD8E165F), 0x03050500)
	#define CAL_syscputestandreset  pfsyscputestandreset
	#define CHK_syscputestandreset  (pfsyscputestandreset != NULL)
	#define EXP_syscputestandreset   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandreset", (RTS_UINTPTR)syscputestandreset, 1, RTSITF_GET_SIGNATURE(0, 0xCD8E165F), 0x03050500) 
#endif


/**
 * <description>
 *  Test and set a bit in an ULONG variable in one processor step. This operation is to provide
 *	 a multitasking save operation.
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).
 *	ERR_OK: If bit could be set and was set before,
 *	ERR_FAILED: If bit is still set
 * </p></result>
 */
typedef struct tagsyscputestandset_struct
{
	RTS_IEC_UDINT *pulValue;			/* VAR_INPUT */	/* <param name="pulValue" type="IN">Pointer to the unsigned value to test and set a bit inside in one atomic
	processor step</param> */
	RTS_IEC_UDINT ulBit;				/* VAR_INPUT */	/* <param name="ulBit" type="IN">Bit number inside the variable to test and set. 0=first bit, 31=last bit</param> */
	RTS_IEC_RESULT SysCpuTestAndSet;	/* VAR_OUTPUT */	
} syscputestandset_struct;

void CDECL CDECL_EXT syscputestandset(syscputestandset_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCPUTESTANDSET_IEC) (syscputestandset_struct *p);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUTESTANDSET_NOTIMPLEMENTED)
	#define USE_syscputestandset
	#define EXT_syscputestandset
	#define GET_syscputestandset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscputestandset(p0) 
	#define CHK_syscputestandset  FALSE
	#define EXP_syscputestandset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscputestandset
	#define EXT_syscputestandset
	#define GET_syscputestandset(fl)  CAL_CMGETAPI( "syscputestandset" ) 
	#define CAL_syscputestandset  syscputestandset
	#define CHK_syscputestandset  TRUE
	#define EXP_syscputestandset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandset", (RTS_UINTPTR)syscputestandset, 1, RTSITF_GET_SIGNATURE(0, 0xC50F8C00), 0x03050500) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_syscputestandset
	#define EXT_syscputestandset
	#define GET_syscputestandset(fl)  CAL_CMGETAPI( "syscputestandset" ) 
	#define CAL_syscputestandset  syscputestandset
	#define CHK_syscputestandset  TRUE
	#define EXP_syscputestandset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandset", (RTS_UINTPTR)syscputestandset, 1, RTSITF_GET_SIGNATURE(0, 0xC50F8C00), 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingsyscputestandset
	#define EXT_SysCpuHandlingsyscputestandset
	#define GET_SysCpuHandlingsyscputestandset  ERR_OK
	#define CAL_SysCpuHandlingsyscputestandset  syscputestandset
	#define CHK_SysCpuHandlingsyscputestandset  TRUE
	#define EXP_SysCpuHandlingsyscputestandset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandset", (RTS_UINTPTR)syscputestandset, 1, RTSITF_GET_SIGNATURE(0, 0xC50F8C00), 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_syscputestandset
	#define EXT_syscputestandset
	#define GET_syscputestandset(fl)  CAL_CMGETAPI( "syscputestandset" ) 
	#define CAL_syscputestandset  syscputestandset
	#define CHK_syscputestandset  TRUE
	#define EXP_syscputestandset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandset", (RTS_UINTPTR)syscputestandset, 1, RTSITF_GET_SIGNATURE(0, 0xC50F8C00), 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_syscputestandset  PFSYSCPUTESTANDSET_IEC pfsyscputestandset;
	#define EXT_syscputestandset  extern PFSYSCPUTESTANDSET_IEC pfsyscputestandset;
	#define GET_syscputestandset(fl)  s_pfCMGetAPI2( "syscputestandset", (RTS_VOID_FCTPTR *)&pfsyscputestandset, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC50F8C00), 0x03050500)
	#define CAL_syscputestandset  pfsyscputestandset
	#define CHK_syscputestandset  (pfsyscputestandset != NULL)
	#define EXP_syscputestandset   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandset", (RTS_UINTPTR)syscputestandset, 1, RTSITF_GET_SIGNATURE(0, 0xC50F8C00), 0x03050500) 
#endif


/**
 * <description>
 *  <p>The function test and set or clear a bit in a variable in one processor step. This operation must be atomic to provide a multitasking save operation.</p>
 *	<p>IMPLEMENTATION NOTE: Try to use a processor opcode, that provides this operation. If such an opcode
 *	is not available, use SysCpuTestAndSetBitBase in your platform adaptation.</p>
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p>
 *	<ul>
 *		<li>ERR_OK: bSet=1: If bit could be set and was not set before
 *					bSet=0: If bit could be cleared and was set before</li>
 *		<li>ERR_FAILED: bSet=1: If bit is still set
 *						bSet=0: If bit is still cleared</li>
 *		<li>ERR_PARAMETER: If pAddress=NULL or pAddress is unaligned or iBit is out nSize range</li>
 *		<li>ERR_NOT_SUPPORTED: If function is not available because of missing components (e.g. SysInt for locking bit access)</li>
 *		<li>ERR_NOTIMPLEMENTED: If function is not implemented on this platform</li>
 *	</ul>
 * </result>
 * <SIL2/>
 */
typedef struct tagsyscputestandsetbit_struct
{
	RTS_IEC_BYTE *pAddress;				/* VAR_INPUT */	/* <param name="pAddress" type="IN">Pointer to test and set or clear a bit inside in one atomic processor step.
									NOTE: The pointer must be natural aligned! nLen=2: pAddress must be 2Byte aligned; nLen=4: pAddress must be 4Byte aligned</param> */
	RTS_IEC_UDINT nLen;					/* VAR_INPUT */	/* <param name="nLen" type="IN">Size of the value behind the address. Can only be 1 (unsigned char), 2 (unsigned short) or 4 (unsigned long)</param> */
	RTS_IEC_DINT iBit;					/* VAR_INPUT */	/* <param name="iBit" type="IN">Bit number inside the variable to test and set or clear:
	<ul>
		<li>nLen = 1: iBit 0..7</li>
		<li>nLen = 2: iBit 0..15</li>
		<li>nLen = 4: iBit 0..31</li>
	</ul>
 </param> */
	RTS_IEC_DINT bSet;					/* VAR_INPUT */	/* <param name="bSet" type="IN">1=Set bit, 0=Clear bit</param> */
	RTS_IEC_RESULT SysCpuTestAndSetBit;	/* VAR_OUTPUT */	
} syscputestandsetbit_struct;

void CDECL CDECL_EXT syscputestandsetbit(syscputestandsetbit_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCPUTESTANDSETBIT_IEC) (syscputestandsetbit_struct *p);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUTESTANDSETBIT_NOTIMPLEMENTED)
	#define USE_syscputestandsetbit
	#define EXT_syscputestandsetbit
	#define GET_syscputestandsetbit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscputestandsetbit(p0) 
	#define CHK_syscputestandsetbit  FALSE
	#define EXP_syscputestandsetbit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscputestandsetbit
	#define EXT_syscputestandsetbit
	#define GET_syscputestandsetbit(fl)  CAL_CMGETAPI( "syscputestandsetbit" ) 
	#define CAL_syscputestandsetbit  syscputestandsetbit
	#define CHK_syscputestandsetbit  TRUE
	#define EXP_syscputestandsetbit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandsetbit", (RTS_UINTPTR)syscputestandsetbit, 1, 0x6A76133F, 0x03050D00) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_syscputestandsetbit
	#define EXT_syscputestandsetbit
	#define GET_syscputestandsetbit(fl)  CAL_CMGETAPI( "syscputestandsetbit" ) 
	#define CAL_syscputestandsetbit  syscputestandsetbit
	#define CHK_syscputestandsetbit  TRUE
	#define EXP_syscputestandsetbit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandsetbit", (RTS_UINTPTR)syscputestandsetbit, 1, 0x6A76133F, 0x03050D00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingsyscputestandsetbit
	#define EXT_SysCpuHandlingsyscputestandsetbit
	#define GET_SysCpuHandlingsyscputestandsetbit  ERR_OK
	#define CAL_SysCpuHandlingsyscputestandsetbit  syscputestandsetbit
	#define CHK_SysCpuHandlingsyscputestandsetbit  TRUE
	#define EXP_SysCpuHandlingsyscputestandsetbit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandsetbit", (RTS_UINTPTR)syscputestandsetbit, 1, 0x6A76133F, 0x03050D00) 
#elif defined(CPLUSPLUS)
	#define USE_syscputestandsetbit
	#define EXT_syscputestandsetbit
	#define GET_syscputestandsetbit(fl)  CAL_CMGETAPI( "syscputestandsetbit" ) 
	#define CAL_syscputestandsetbit  syscputestandsetbit
	#define CHK_syscputestandsetbit  TRUE
	#define EXP_syscputestandsetbit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandsetbit", (RTS_UINTPTR)syscputestandsetbit, 1, 0x6A76133F, 0x03050D00) 
#else /* DYNAMIC_LINK */
	#define USE_syscputestandsetbit  PFSYSCPUTESTANDSETBIT_IEC pfsyscputestandsetbit;
	#define EXT_syscputestandsetbit  extern PFSYSCPUTESTANDSETBIT_IEC pfsyscputestandsetbit;
	#define GET_syscputestandsetbit(fl)  s_pfCMGetAPI2( "syscputestandsetbit", (RTS_VOID_FCTPTR *)&pfsyscputestandsetbit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x6A76133F, 0x03050D00)
	#define CAL_syscputestandsetbit  pfsyscputestandsetbit
	#define CHK_syscputestandsetbit  (pfsyscputestandsetbit != NULL)
	#define EXP_syscputestandsetbit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscputestandsetbit", (RTS_UINTPTR)syscputestandsetbit, 1, 0x6A76133F, 0x03050D00) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/* Init routines for OS specific modules */
RTS_RESULT CDECL SysCpuHandlingOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysCpuHandlingOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>
 *   On some processors/operating systems this function must be called after code 
 *   has been changed, so that the CPU is notified about the change.
 * </description>
 * <param name="pBaseAddress" type="IN">Start address of the region to be flushed. Set to NULL to flush the
 *	complete instruction cache.</param>
 * <param name="ulSize" type="IN">Length of the region to be flushed. This parameter is ignored if 
 *	pBaseAddress is NULL.</param>
 * <result>error code</result>
 */
int CDECL SysCpuFlushInstructionCache(void * pBaseAddress, unsigned long ulSize);
typedef int (CDECL * PFSYSCPUFLUSHINSTRUCTIONCACHE) (void * pBaseAddress, unsigned long ulSize);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUFLUSHINSTRUCTIONCACHE_NOTIMPLEMENTED)
	#define USE_SysCpuFlushInstructionCache
	#define EXT_SysCpuFlushInstructionCache
	#define GET_SysCpuFlushInstructionCache(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuFlushInstructionCache(p0,p1)  (int)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuFlushInstructionCache  FALSE
	#define EXP_SysCpuFlushInstructionCache  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuFlushInstructionCache
	#define EXT_SysCpuFlushInstructionCache
	#define GET_SysCpuFlushInstructionCache(fl)  CAL_CMGETAPI( "SysCpuFlushInstructionCache" ) 
	#define CAL_SysCpuFlushInstructionCache  SysCpuFlushInstructionCache
	#define CHK_SysCpuFlushInstructionCache  TRUE
	#define EXP_SysCpuFlushInstructionCache  CAL_CMEXPAPI( "SysCpuFlushInstructionCache" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuFlushInstructionCache
	#define EXT_SysCpuFlushInstructionCache
	#define GET_SysCpuFlushInstructionCache(fl)  CAL_CMGETAPI( "SysCpuFlushInstructionCache" ) 
	#define CAL_SysCpuFlushInstructionCache  SysCpuFlushInstructionCache
	#define CHK_SysCpuFlushInstructionCache  TRUE
	#define EXP_SysCpuFlushInstructionCache  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuFlushInstructionCache", (RTS_UINTPTR)SysCpuFlushInstructionCache, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuFlushInstructionCache
	#define EXT_SysCpuHandlingSysCpuFlushInstructionCache
	#define GET_SysCpuHandlingSysCpuFlushInstructionCache  ERR_OK
	#define CAL_SysCpuHandlingSysCpuFlushInstructionCache pISysCpuHandling->ISysCpuFlushInstructionCache
	#define CHK_SysCpuHandlingSysCpuFlushInstructionCache (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuFlushInstructionCache  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuFlushInstructionCache
	#define EXT_SysCpuFlushInstructionCache
	#define GET_SysCpuFlushInstructionCache(fl)  CAL_CMGETAPI( "SysCpuFlushInstructionCache" ) 
	#define CAL_SysCpuFlushInstructionCache pISysCpuHandling->ISysCpuFlushInstructionCache
	#define CHK_SysCpuFlushInstructionCache (pISysCpuHandling != NULL)
	#define EXP_SysCpuFlushInstructionCache  CAL_CMEXPAPI( "SysCpuFlushInstructionCache" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuFlushInstructionCache  PFSYSCPUFLUSHINSTRUCTIONCACHE pfSysCpuFlushInstructionCache;
	#define EXT_SysCpuFlushInstructionCache  extern PFSYSCPUFLUSHINSTRUCTIONCACHE pfSysCpuFlushInstructionCache;
	#define GET_SysCpuFlushInstructionCache(fl)  s_pfCMGetAPI2( "SysCpuFlushInstructionCache", (RTS_VOID_FCTPTR *)&pfSysCpuFlushInstructionCache, (fl), 0, 0)
	#define CAL_SysCpuFlushInstructionCache  pfSysCpuFlushInstructionCache
	#define CHK_SysCpuFlushInstructionCache  (pfSysCpuFlushInstructionCache != NULL)
	#define EXP_SysCpuFlushInstructionCache  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuFlushInstructionCache", (RTS_UINTPTR)SysCpuFlushInstructionCache, 0, 0) 
#endif




/**
 * <description>
 *	<p>Call an IEC function from plain C code.
 *	Since different CPU's/systems use different calling conventions, this function 
 *	should be used as a wrapper. But this function respectively the IEC callback code is not synchronized against an OnlineChange of the corresponding application!</p>
 *
 *  <p>ATTENTION: To be safe on multitasking and muticore systems you have to use IecTaskCallIecFuncWithParams() instead (see CmpIecTaskItf.h)!</p>
 *
 *  <p>IEC functions or methods of function block use all the same calling convention:
 *	They have no return value and exactly one parameter, which is a pointer to a struct that contains all required
 *  IN and OUT parameters.</p>
 *
 *	<p>IMPLEMENTATION NOTE: The content of the parameter structure must be copied completely on the stack as 
 *	an input parameter! Don't copy only the pointer! Because of this, the size of the structure is provided as a 
 *	separate parameter to this function. Additionally, the structure of the IEC function must be copied back into the
 *	give parameter to return result values of the IEC function! For all this operations you have to ensure the stack
 *  alignment, but avoid copying more bytes than iSize</p>
 *
 *	<p>IMPLEMENTATION NOTE:
 *	Unused parameter pParam can be NULL, if function has no argument and no result (e.g. CodeInit)!
 *	</p>
 * </description>
 * <param name="pfIECFunc" type="IN" range="[NULL,VALID_IEC_FUNC,INVALID_IEC_FUNC]">Pointer to the IEC function that should be called</param>
 * <param name="pParam" type="INOUT" range="[NULL,VALID_PARAMETER]">Pointer to the parameter struct that contains the function parameters. ATTENTION: Can be NULL!</param>
 * <param name="iSize" type="IN" range="[0,VALID_SIZE]">Size of the parameter structure to copy the content on stack. ATTENTION: Can be 0!</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Parameter check was successfull and pfIECFunc was called</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">pfIECFunc is NULL or for a paramter size > 0 pParam is NULL</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_NOTIMPLEMENTED">Function is not implemented</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysCpuCallIecFuncWithParams(RTS_VOID_FCTPTR pfIECFunc, void* pParam, int iSize);
typedef RTS_RESULT (CDECL * PFSYSCPUCALLIECFUNCWITHPARAMS) (RTS_VOID_FCTPTR pfIECFunc, void* pParam, int iSize);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUCALLIECFUNCWITHPARAMS_NOTIMPLEMENTED)
	#define USE_SysCpuCallIecFuncWithParams
	#define EXT_SysCpuCallIecFuncWithParams
	#define GET_SysCpuCallIecFuncWithParams(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuCallIecFuncWithParams(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuCallIecFuncWithParams  FALSE
	#define EXP_SysCpuCallIecFuncWithParams  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuCallIecFuncWithParams
	#define EXT_SysCpuCallIecFuncWithParams
	#define GET_SysCpuCallIecFuncWithParams(fl)  CAL_CMGETAPI( "SysCpuCallIecFuncWithParams" ) 
	#define CAL_SysCpuCallIecFuncWithParams  SysCpuCallIecFuncWithParams
	#define CHK_SysCpuCallIecFuncWithParams  TRUE
	#define EXP_SysCpuCallIecFuncWithParams  CAL_CMEXPAPI( "SysCpuCallIecFuncWithParams" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuCallIecFuncWithParams
	#define EXT_SysCpuCallIecFuncWithParams
	#define GET_SysCpuCallIecFuncWithParams(fl)  CAL_CMGETAPI( "SysCpuCallIecFuncWithParams" ) 
	#define CAL_SysCpuCallIecFuncWithParams  SysCpuCallIecFuncWithParams
	#define CHK_SysCpuCallIecFuncWithParams  TRUE
	#define EXP_SysCpuCallIecFuncWithParams  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuCallIecFuncWithParams", (RTS_UINTPTR)SysCpuCallIecFuncWithParams, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuCallIecFuncWithParams
	#define EXT_SysCpuHandlingSysCpuCallIecFuncWithParams
	#define GET_SysCpuHandlingSysCpuCallIecFuncWithParams  ERR_OK
	#define CAL_SysCpuHandlingSysCpuCallIecFuncWithParams pISysCpuHandling->ISysCpuCallIecFuncWithParams
	#define CHK_SysCpuHandlingSysCpuCallIecFuncWithParams (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuCallIecFuncWithParams  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuCallIecFuncWithParams
	#define EXT_SysCpuCallIecFuncWithParams
	#define GET_SysCpuCallIecFuncWithParams(fl)  CAL_CMGETAPI( "SysCpuCallIecFuncWithParams" ) 
	#define CAL_SysCpuCallIecFuncWithParams pISysCpuHandling->ISysCpuCallIecFuncWithParams
	#define CHK_SysCpuCallIecFuncWithParams (pISysCpuHandling != NULL)
	#define EXP_SysCpuCallIecFuncWithParams  CAL_CMEXPAPI( "SysCpuCallIecFuncWithParams" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuCallIecFuncWithParams  PFSYSCPUCALLIECFUNCWITHPARAMS pfSysCpuCallIecFuncWithParams;
	#define EXT_SysCpuCallIecFuncWithParams  extern PFSYSCPUCALLIECFUNCWITHPARAMS pfSysCpuCallIecFuncWithParams;
	#define GET_SysCpuCallIecFuncWithParams(fl)  s_pfCMGetAPI2( "SysCpuCallIecFuncWithParams", (RTS_VOID_FCTPTR *)&pfSysCpuCallIecFuncWithParams, (fl), 0, 0)
	#define CAL_SysCpuCallIecFuncWithParams  pfSysCpuCallIecFuncWithParams
	#define CHK_SysCpuCallIecFuncWithParams  (pfSysCpuCallIecFuncWithParams != NULL)
	#define EXP_SysCpuCallIecFuncWithParams  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuCallIecFuncWithParams", (RTS_UINTPTR)SysCpuCallIecFuncWithParams, 0, 0) 
#endif




/**
 * <description>
 *	Sets the specified register to the given value inside the 'syscpudebughandler'
 * </description>
 * <param name="pRegBuff" type="IN">Pointer to start of saved registers</param>
 * <param name="nRegisterNumber" type="IN">Register number to be set. See category "Register offsets" for details</param>
 * <param name="newValue" type="IN">The new value</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysCpuSetRegisterValue(RTS_UINTPTR *pRegBuff, RTS_I32 nRegisterNumber, RTS_UINTPTR newValue);
typedef RTS_RESULT (CDECL * PFSYSCPUSETREGISTERVALUE) (RTS_UINTPTR *pRegBuff, RTS_I32 nRegisterNumber, RTS_UINTPTR newValue);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUSETREGISTERVALUE_NOTIMPLEMENTED)
	#define USE_SysCpuSetRegisterValue
	#define EXT_SysCpuSetRegisterValue
	#define GET_SysCpuSetRegisterValue(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuSetRegisterValue(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuSetRegisterValue  FALSE
	#define EXP_SysCpuSetRegisterValue  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuSetRegisterValue
	#define EXT_SysCpuSetRegisterValue
	#define GET_SysCpuSetRegisterValue(fl)  CAL_CMGETAPI( "SysCpuSetRegisterValue" ) 
	#define CAL_SysCpuSetRegisterValue  SysCpuSetRegisterValue
	#define CHK_SysCpuSetRegisterValue  TRUE
	#define EXP_SysCpuSetRegisterValue  CAL_CMEXPAPI( "SysCpuSetRegisterValue" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuSetRegisterValue
	#define EXT_SysCpuSetRegisterValue
	#define GET_SysCpuSetRegisterValue(fl)  CAL_CMGETAPI( "SysCpuSetRegisterValue" ) 
	#define CAL_SysCpuSetRegisterValue  SysCpuSetRegisterValue
	#define CHK_SysCpuSetRegisterValue  TRUE
	#define EXP_SysCpuSetRegisterValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuSetRegisterValue", (RTS_UINTPTR)SysCpuSetRegisterValue, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuSetRegisterValue
	#define EXT_SysCpuHandlingSysCpuSetRegisterValue
	#define GET_SysCpuHandlingSysCpuSetRegisterValue  ERR_OK
	#define CAL_SysCpuHandlingSysCpuSetRegisterValue pISysCpuHandling->ISysCpuSetRegisterValue
	#define CHK_SysCpuHandlingSysCpuSetRegisterValue (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuSetRegisterValue  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuSetRegisterValue
	#define EXT_SysCpuSetRegisterValue
	#define GET_SysCpuSetRegisterValue(fl)  CAL_CMGETAPI( "SysCpuSetRegisterValue" ) 
	#define CAL_SysCpuSetRegisterValue pISysCpuHandling->ISysCpuSetRegisterValue
	#define CHK_SysCpuSetRegisterValue (pISysCpuHandling != NULL)
	#define EXP_SysCpuSetRegisterValue  CAL_CMEXPAPI( "SysCpuSetRegisterValue" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuSetRegisterValue  PFSYSCPUSETREGISTERVALUE pfSysCpuSetRegisterValue;
	#define EXT_SysCpuSetRegisterValue  extern PFSYSCPUSETREGISTERVALUE pfSysCpuSetRegisterValue;
	#define GET_SysCpuSetRegisterValue(fl)  s_pfCMGetAPI2( "SysCpuSetRegisterValue", (RTS_VOID_FCTPTR *)&pfSysCpuSetRegisterValue, (fl), 0, 0)
	#define CAL_SysCpuSetRegisterValue  pfSysCpuSetRegisterValue
	#define CHK_SysCpuSetRegisterValue  (pfSysCpuSetRegisterValue != NULL)
	#define EXP_SysCpuSetRegisterValue  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuSetRegisterValue", (RTS_UINTPTR)SysCpuSetRegisterValue, 0, 0) 
#endif




/**
 * <description>
 *	<p>Routine is the entry of the IEC code debugger. Is called out of the IEC task if breakpoint is reached.</p>
 *	<p>IMPLEMENTATION NOTE: In this routine, the AppDebugHandler() function of CmpApp must be called with
 *	the appropriate parameters. See CmpAppItf.h for detailed information. The return value of
 *	AppDebugHandler() must be used to set the return address in the IEC code. To this address we will return
 *	when leaving syscpudebughandler().</p>
 * <p>SIL2 IMPLEMENTATION NOTE: This routine may never be reached within safety mode. If it is called in safety mode 
 * an Exception must be generated immediately! The Execution may not proceed to further debug mechanism!</p>
 * </description>
 * <parampseudo name="OperationMode" type="IN" range="[RTS_SIL2_OPMODE_SAFE,RTS_SIL2_OPMODE_DEBUG]">Specifies only that the function should be called</parampseudo>
 * <parampseudo name="bExceptionOccured" type="OUT" range="[TRUE,FALSE]">Specifies, if an exception should occure, or not</parampseudo>
 * <result>no return value</result>
 */
void CDECL CDECL_EXT syscpudebughandler(void);
typedef void (CDECL CDECL_EXT* PFSYSCPUDEBUGHANDLER_IEC) (void);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUDEBUGHANDLER_NOTIMPLEMENTED)
	#define USE_syscpudebughandler
	#define EXT_syscpudebughandler
	#define GET_syscpudebughandler(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscpudebughandler() 
	#define CHK_syscpudebughandler  FALSE
	#define EXP_syscpudebughandler  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscpudebughandler
	#define EXT_syscpudebughandler
	#define GET_syscpudebughandler(fl)  CAL_CMGETAPI( "syscpudebughandler" ) 
	#define CAL_syscpudebughandler  syscpudebughandler
	#define CHK_syscpudebughandler  TRUE
	#define EXP_syscpudebughandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"syscpudebughandler", (RTS_UINTPTR)syscpudebughandler, 1, 0) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_syscpudebughandler
	#define EXT_syscpudebughandler
	#define GET_syscpudebughandler(fl)  CAL_CMGETAPI( "syscpudebughandler" ) 
	#define CAL_syscpudebughandler  syscpudebughandler
	#define CHK_syscpudebughandler  TRUE
	#define EXP_syscpudebughandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"syscpudebughandler", (RTS_UINTPTR)syscpudebughandler, 1, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingsyscpudebughandler
	#define EXT_SysCpuHandlingsyscpudebughandler
	#define GET_SysCpuHandlingsyscpudebughandler  ERR_OK
	#define CAL_SysCpuHandlingsyscpudebughandler  syscpudebughandler
	#define CHK_SysCpuHandlingsyscpudebughandler  TRUE
	#define EXP_SysCpuHandlingsyscpudebughandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"syscpudebughandler", (RTS_UINTPTR)syscpudebughandler, 1, 0) 
#elif defined(CPLUSPLUS)
	#define USE_syscpudebughandler
	#define EXT_syscpudebughandler
	#define GET_syscpudebughandler(fl)  CAL_CMGETAPI( "syscpudebughandler" ) 
	#define CAL_syscpudebughandler  syscpudebughandler
	#define CHK_syscpudebughandler  TRUE
	#define EXP_syscpudebughandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"syscpudebughandler", (RTS_UINTPTR)syscpudebughandler, 1, 0) 
#else /* DYNAMIC_LINK */
	#define USE_syscpudebughandler  PFSYSCPUDEBUGHANDLER_IEC pfsyscpudebughandler;
	#define EXT_syscpudebughandler  extern PFSYSCPUDEBUGHANDLER_IEC pfsyscpudebughandler;
	#define GET_syscpudebughandler(fl)  s_pfCMGetAPI2( "syscpudebughandler", (RTS_VOID_FCTPTR *)&pfsyscpudebughandler, (fl), 0, 0)
	#define CAL_syscpudebughandler  pfsyscpudebughandler
	#define CHK_syscpudebughandler  (pfsyscpudebughandler != NULL)
	#define EXP_syscpudebughandler   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"syscpudebughandler", (RTS_UINTPTR)syscpudebughandler, 1, 0) 
#endif


/**
 * <description>
 *	Routine retrieves the breakpoint opcode for IEC code debugging.
 *	This is cpu dependant and depends additionally on the 
 *	CoDeSys codegenerator.
 * </description>
 * <param name="pbyAreaStart" type="IN">Pointer to start of the area.</param>
 * <param name="pbyBPAddress" type="IN">Pointer to breakpoint address (where the breakpoint will be set).
 *	This parameter is used for breakpoints that uses absolut jumps.</param>
 * <param name="pbyOpCode" type="OUT">Pointer to get opcode</param>
 * <param name="piOpcodeSize" type="INOUT">Pointer to maximum size of opcode buffer and return the real size
 *	of the opcode</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysCpuGetBreakpointCode(unsigned char* pbyAreaStart, unsigned char* pbyBPAddress, unsigned char* pbyOpCode, int *piOpcodeSize);
typedef RTS_RESULT (CDECL * PFSYSCPUGETBREAKPOINTCODE) (unsigned char* pbyAreaStart, unsigned char* pbyBPAddress, unsigned char* pbyOpCode, int *piOpcodeSize);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUGETBREAKPOINTCODE_NOTIMPLEMENTED)
	#define USE_SysCpuGetBreakpointCode
	#define EXT_SysCpuGetBreakpointCode
	#define GET_SysCpuGetBreakpointCode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuGetBreakpointCode(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuGetBreakpointCode  FALSE
	#define EXP_SysCpuGetBreakpointCode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuGetBreakpointCode
	#define EXT_SysCpuGetBreakpointCode
	#define GET_SysCpuGetBreakpointCode(fl)  CAL_CMGETAPI( "SysCpuGetBreakpointCode" ) 
	#define CAL_SysCpuGetBreakpointCode  SysCpuGetBreakpointCode
	#define CHK_SysCpuGetBreakpointCode  TRUE
	#define EXP_SysCpuGetBreakpointCode  CAL_CMEXPAPI( "SysCpuGetBreakpointCode" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuGetBreakpointCode
	#define EXT_SysCpuGetBreakpointCode
	#define GET_SysCpuGetBreakpointCode(fl)  CAL_CMGETAPI( "SysCpuGetBreakpointCode" ) 
	#define CAL_SysCpuGetBreakpointCode  SysCpuGetBreakpointCode
	#define CHK_SysCpuGetBreakpointCode  TRUE
	#define EXP_SysCpuGetBreakpointCode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetBreakpointCode", (RTS_UINTPTR)SysCpuGetBreakpointCode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuGetBreakpointCode
	#define EXT_SysCpuHandlingSysCpuGetBreakpointCode
	#define GET_SysCpuHandlingSysCpuGetBreakpointCode  ERR_OK
	#define CAL_SysCpuHandlingSysCpuGetBreakpointCode pISysCpuHandling->ISysCpuGetBreakpointCode
	#define CHK_SysCpuHandlingSysCpuGetBreakpointCode (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuGetBreakpointCode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuGetBreakpointCode
	#define EXT_SysCpuGetBreakpointCode
	#define GET_SysCpuGetBreakpointCode(fl)  CAL_CMGETAPI( "SysCpuGetBreakpointCode" ) 
	#define CAL_SysCpuGetBreakpointCode pISysCpuHandling->ISysCpuGetBreakpointCode
	#define CHK_SysCpuGetBreakpointCode (pISysCpuHandling != NULL)
	#define EXP_SysCpuGetBreakpointCode  CAL_CMEXPAPI( "SysCpuGetBreakpointCode" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuGetBreakpointCode  PFSYSCPUGETBREAKPOINTCODE pfSysCpuGetBreakpointCode;
	#define EXT_SysCpuGetBreakpointCode  extern PFSYSCPUGETBREAKPOINTCODE pfSysCpuGetBreakpointCode;
	#define GET_SysCpuGetBreakpointCode(fl)  s_pfCMGetAPI2( "SysCpuGetBreakpointCode", (RTS_VOID_FCTPTR *)&pfSysCpuGetBreakpointCode, (fl), 0, 0)
	#define CAL_SysCpuGetBreakpointCode  pfSysCpuGetBreakpointCode
	#define CHK_SysCpuGetBreakpointCode  (pfSysCpuGetBreakpointCode != NULL)
	#define EXP_SysCpuGetBreakpointCode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetBreakpointCode", (RTS_UINTPTR)SysCpuGetBreakpointCode, 0, 0) 
#endif




/**
 * <description>
 * <p>Routine retrieves an IEC callstack entry, if the IEC code execution is stopped in the
 * IEC code (breakpoint, exception, etc.). The callstack can be investigated out of the stack frames of each
 * entered nested function. The first callstack entry is calculated outside this routine from the 
 * instruction pointer address (IP) that is provided for the AppDebugHandler() routine from syscpudebughandler().
 * All further entries are called from this routine.</p>
 * <p>IMPLEMENTATION NOTE: The stack frame has typically the following structure:</p>
 *  <pre>
 * .         STACK                        CODE
 * .         -----                        ----
 * .
 * .         BP0 /--------|               Fct0:
 * .                      |                    ...
 * .         Return Fct0  |                    Call Fct1
 * .    |--\ BP1 ---------|                    Return Fct0
 * .    |
 * .    |    Return Fct1                  Fct1:
 * .    |--- BP2 /--------|                    ...
 * .         ...          |                    Call Fct2
 * .         Stop         |                    Return Fct1
 * .                      |-- pulBP
 * .                                      Fct2:
 * .                                        ...
 * .                           IP --------/    Stop
 *  </pre>
 * <p>In this example, Fct0 calls Fct1 and Fct1 calls Fct2. This is a nested call with 3 Functions.
 * Inside Fct2 we do a halt (e.g. stop on breakpoint). If we take a look on the stack, we see the return 
 * addresses from Fct0 and Fct1 on the stack. At the entry of each function, the base- (or frame-) pointer
 * is pushed on the stack with the previous content. So you can see, the BP entries on the stack
 * are organized as a chained list from stackframe to stack frame.</p>
 * <p>The first stack entry is the current position. This is calculated out of the IP address and cannot be 
 * investigated from stack.</p>
 * <p>The pulBP Parameter is a pointer, thats content is the address of the stack, where the BP entry resides.</p>
 * </description>
 * <param name="pBP" type="INOUT">Pointer to last base pointer entry (or frame pointer) and returns
 * the pointer to the next base pointer in the stack frame</param>
 * <param name="ppAddress" type="OUT">Pointer pointer to return address in the code of the caller</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysCpuGetCallstackEntry(RTS_UINTPTR *pBP, void **ppAddress);
typedef RTS_RESULT (CDECL * PFSYSCPUGETCALLSTACKENTRY) (RTS_UINTPTR *pBP, void **ppAddress);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUGETCALLSTACKENTRY_NOTIMPLEMENTED)
	#define USE_SysCpuGetCallstackEntry
	#define EXT_SysCpuGetCallstackEntry
	#define GET_SysCpuGetCallstackEntry(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuGetCallstackEntry(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuGetCallstackEntry  FALSE
	#define EXP_SysCpuGetCallstackEntry  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuGetCallstackEntry
	#define EXT_SysCpuGetCallstackEntry
	#define GET_SysCpuGetCallstackEntry(fl)  CAL_CMGETAPI( "SysCpuGetCallstackEntry" ) 
	#define CAL_SysCpuGetCallstackEntry  SysCpuGetCallstackEntry
	#define CHK_SysCpuGetCallstackEntry  TRUE
	#define EXP_SysCpuGetCallstackEntry  CAL_CMEXPAPI( "SysCpuGetCallstackEntry" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuGetCallstackEntry
	#define EXT_SysCpuGetCallstackEntry
	#define GET_SysCpuGetCallstackEntry(fl)  CAL_CMGETAPI( "SysCpuGetCallstackEntry" ) 
	#define CAL_SysCpuGetCallstackEntry  SysCpuGetCallstackEntry
	#define CHK_SysCpuGetCallstackEntry  TRUE
	#define EXP_SysCpuGetCallstackEntry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetCallstackEntry", (RTS_UINTPTR)SysCpuGetCallstackEntry, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuGetCallstackEntry
	#define EXT_SysCpuHandlingSysCpuGetCallstackEntry
	#define GET_SysCpuHandlingSysCpuGetCallstackEntry  ERR_OK
	#define CAL_SysCpuHandlingSysCpuGetCallstackEntry pISysCpuHandling->ISysCpuGetCallstackEntry
	#define CHK_SysCpuHandlingSysCpuGetCallstackEntry (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuGetCallstackEntry  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuGetCallstackEntry
	#define EXT_SysCpuGetCallstackEntry
	#define GET_SysCpuGetCallstackEntry(fl)  CAL_CMGETAPI( "SysCpuGetCallstackEntry" ) 
	#define CAL_SysCpuGetCallstackEntry pISysCpuHandling->ISysCpuGetCallstackEntry
	#define CHK_SysCpuGetCallstackEntry (pISysCpuHandling != NULL)
	#define EXP_SysCpuGetCallstackEntry  CAL_CMEXPAPI( "SysCpuGetCallstackEntry" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuGetCallstackEntry  PFSYSCPUGETCALLSTACKENTRY pfSysCpuGetCallstackEntry;
	#define EXT_SysCpuGetCallstackEntry  extern PFSYSCPUGETCALLSTACKENTRY pfSysCpuGetCallstackEntry;
	#define GET_SysCpuGetCallstackEntry(fl)  s_pfCMGetAPI2( "SysCpuGetCallstackEntry", (RTS_VOID_FCTPTR *)&pfSysCpuGetCallstackEntry, (fl), 0, 0)
	#define CAL_SysCpuGetCallstackEntry  pfSysCpuGetCallstackEntry
	#define CHK_SysCpuGetCallstackEntry  (pfSysCpuGetCallstackEntry != NULL)
	#define EXP_SysCpuGetCallstackEntry  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetCallstackEntry", (RTS_UINTPTR)SysCpuGetCallstackEntry, 0, 0) 
#endif




/**
 * <description>Routine to retrieve a callstack entry. The additional parameter bIecCode specifies, if the callstack
 *	should be retrieved in IEC-code (bIecCode = 1) or in C-code of the runtime system (bIecCode = 0).
 *	IMPLEMENTATION NOTE:
 *	The callstack in C-code is sometimes compiler dependant and must be implemented here specific to the compiler.
 *  If ERR_NOTIMPLEMENTED is returned, the generic linear backtrace on stack is done in SysCpuHandling.c.
 * </description>
 * <param name="bIecCode" type="IN">Is callstack expected in IEC- or in runtime-code:
 *		bIecCode=1: Expected in IEC-code
 *		bIecCode=0: Expected in runtime-code (external library call)
 * </param>
 * <param name="pBP" type="INOUT">Pointer to last base pointer entry (or frame pointer) and returns
 *	the pointer to the next base pointer in the stack frame</param>
 * <param name="ppAddress" type="OUT">Pointer pointer to return address in the code of the caller</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">If ERR_NOTIMPLEMENTED is returned, the generic linear backtrace on stack is done in SysCpuHandling.c</errorcode>
 */
RTS_RESULT CDECL SysCpuGetCallstackEntry2(RTS_I32 bIecCode, RTS_UINTPTR *pBP, void **ppAddress);
typedef RTS_RESULT (CDECL * PFSYSCPUGETCALLSTACKENTRY2) (RTS_I32 bIecCode, RTS_UINTPTR *pBP, void **ppAddress);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUGETCALLSTACKENTRY2_NOTIMPLEMENTED)
	#define USE_SysCpuGetCallstackEntry2
	#define EXT_SysCpuGetCallstackEntry2
	#define GET_SysCpuGetCallstackEntry2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuGetCallstackEntry2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuGetCallstackEntry2  FALSE
	#define EXP_SysCpuGetCallstackEntry2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuGetCallstackEntry2
	#define EXT_SysCpuGetCallstackEntry2
	#define GET_SysCpuGetCallstackEntry2(fl)  CAL_CMGETAPI( "SysCpuGetCallstackEntry2" ) 
	#define CAL_SysCpuGetCallstackEntry2  SysCpuGetCallstackEntry2
	#define CHK_SysCpuGetCallstackEntry2  TRUE
	#define EXP_SysCpuGetCallstackEntry2  CAL_CMEXPAPI( "SysCpuGetCallstackEntry2" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuGetCallstackEntry2
	#define EXT_SysCpuGetCallstackEntry2
	#define GET_SysCpuGetCallstackEntry2(fl)  CAL_CMGETAPI( "SysCpuGetCallstackEntry2" ) 
	#define CAL_SysCpuGetCallstackEntry2  SysCpuGetCallstackEntry2
	#define CHK_SysCpuGetCallstackEntry2  TRUE
	#define EXP_SysCpuGetCallstackEntry2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetCallstackEntry2", (RTS_UINTPTR)SysCpuGetCallstackEntry2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuGetCallstackEntry2
	#define EXT_SysCpuHandlingSysCpuGetCallstackEntry2
	#define GET_SysCpuHandlingSysCpuGetCallstackEntry2  ERR_OK
	#define CAL_SysCpuHandlingSysCpuGetCallstackEntry2 pISysCpuHandling->ISysCpuGetCallstackEntry2
	#define CHK_SysCpuHandlingSysCpuGetCallstackEntry2 (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuGetCallstackEntry2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuGetCallstackEntry2
	#define EXT_SysCpuGetCallstackEntry2
	#define GET_SysCpuGetCallstackEntry2(fl)  CAL_CMGETAPI( "SysCpuGetCallstackEntry2" ) 
	#define CAL_SysCpuGetCallstackEntry2 pISysCpuHandling->ISysCpuGetCallstackEntry2
	#define CHK_SysCpuGetCallstackEntry2 (pISysCpuHandling != NULL)
	#define EXP_SysCpuGetCallstackEntry2  CAL_CMEXPAPI( "SysCpuGetCallstackEntry2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuGetCallstackEntry2  PFSYSCPUGETCALLSTACKENTRY2 pfSysCpuGetCallstackEntry2;
	#define EXT_SysCpuGetCallstackEntry2  extern PFSYSCPUGETCALLSTACKENTRY2 pfSysCpuGetCallstackEntry2;
	#define GET_SysCpuGetCallstackEntry2(fl)  s_pfCMGetAPI2( "SysCpuGetCallstackEntry2", (RTS_VOID_FCTPTR *)&pfSysCpuGetCallstackEntry2, (fl), 0, 0)
	#define CAL_SysCpuGetCallstackEntry2  pfSysCpuGetCallstackEntry2
	#define CHK_SysCpuGetCallstackEntry2  (pfSysCpuGetCallstackEntry2 != NULL)
	#define EXP_SysCpuGetCallstackEntry2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetCallstackEntry2", (RTS_UINTPTR)SysCpuGetCallstackEntry2, 0, 0) 
#endif




/**
 * <description>Routine to retrieve a callstack entry. The additional parameter bIecCode specifies, if the callstack
 *	should be retrieved in IEC-code (bIecCode = 1) or in C-code of the runtime system (bIecCode = 0).
 *  We use here first SysCpuGetCallstackEntry2() of the platform adaptation. If it is not implemented, we do a generic
 *	backtrace on the stack.
 * </description>
 * <param name="bIecCode" type="IN">Is callstack expected in IEC- or in runtime-code:
 *		bIecCode=1: Expected in IEC-code
 *		bIecCode=0: Expected in runtime-code (external library call)
 * </param>
 * <param name="pContext" type="INOUT">Pointer to last base context entry and returns
 *	the pointer to the next context of the stack frame</param>
 * <param name="ppAddress" type="OUT">Pointer pointer to return address in the code corresponding to the callstack entry</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Callstack entry resp. the IEC caller could be detected</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Something failed during detecting the callstack</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">Callstack entry resp. the IEC caller could _not_ be detected. Perhaps there is no IEC caller or
 *	the necessary setting in the DeviceDescription is not set for the platform:
 *		 ARM, CortexM3, MIPS, PPC, SH, x86-64 and x86/WindowsCE
 *
 *		 &lt;ts:section name=&quot;codegenerator&quot;&gt;
 *				&lt;ts:setting name=&quot;compiler-defines&quot; type=&quot;string&quot; access=&quot;visible&quot;&gt;
 *					   &lt;ts:value&gt;generate_exceptioninfo&lt;/ts:value&gt;
 *				&lt;/ts:setting&gt;
 *		 &lt;/ts:section&gt; corresponding
 * </errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOT_SUPPORTED">A necessary interface function is not available (CmpApp::AppGetApplicationByAreaAddress)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">If one of the parameters is invalid</errorcode>
 */
RTS_RESULT CDECL SysCpuGetCallstackEntry3(RTS_I32 bIecCode, RegContext *pContext, void **ppAddress);
typedef RTS_RESULT (CDECL * PFSYSCPUGETCALLSTACKENTRY3) (RTS_I32 bIecCode, RegContext *pContext, void **ppAddress);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUGETCALLSTACKENTRY3_NOTIMPLEMENTED)
	#define USE_SysCpuGetCallstackEntry3
	#define EXT_SysCpuGetCallstackEntry3
	#define GET_SysCpuGetCallstackEntry3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuGetCallstackEntry3(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuGetCallstackEntry3  FALSE
	#define EXP_SysCpuGetCallstackEntry3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuGetCallstackEntry3
	#define EXT_SysCpuGetCallstackEntry3
	#define GET_SysCpuGetCallstackEntry3(fl)  CAL_CMGETAPI( "SysCpuGetCallstackEntry3" ) 
	#define CAL_SysCpuGetCallstackEntry3  SysCpuGetCallstackEntry3
	#define CHK_SysCpuGetCallstackEntry3  TRUE
	#define EXP_SysCpuGetCallstackEntry3  CAL_CMEXPAPI( "SysCpuGetCallstackEntry3" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuGetCallstackEntry3
	#define EXT_SysCpuGetCallstackEntry3
	#define GET_SysCpuGetCallstackEntry3(fl)  CAL_CMGETAPI( "SysCpuGetCallstackEntry3" ) 
	#define CAL_SysCpuGetCallstackEntry3  SysCpuGetCallstackEntry3
	#define CHK_SysCpuGetCallstackEntry3  TRUE
	#define EXP_SysCpuGetCallstackEntry3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetCallstackEntry3", (RTS_UINTPTR)SysCpuGetCallstackEntry3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuGetCallstackEntry3
	#define EXT_SysCpuHandlingSysCpuGetCallstackEntry3
	#define GET_SysCpuHandlingSysCpuGetCallstackEntry3  ERR_OK
	#define CAL_SysCpuHandlingSysCpuGetCallstackEntry3 pISysCpuHandling->ISysCpuGetCallstackEntry3
	#define CHK_SysCpuHandlingSysCpuGetCallstackEntry3 (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuGetCallstackEntry3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuGetCallstackEntry3
	#define EXT_SysCpuGetCallstackEntry3
	#define GET_SysCpuGetCallstackEntry3(fl)  CAL_CMGETAPI( "SysCpuGetCallstackEntry3" ) 
	#define CAL_SysCpuGetCallstackEntry3 pISysCpuHandling->ISysCpuGetCallstackEntry3
	#define CHK_SysCpuGetCallstackEntry3 (pISysCpuHandling != NULL)
	#define EXP_SysCpuGetCallstackEntry3  CAL_CMEXPAPI( "SysCpuGetCallstackEntry3" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuGetCallstackEntry3  PFSYSCPUGETCALLSTACKENTRY3 pfSysCpuGetCallstackEntry3;
	#define EXT_SysCpuGetCallstackEntry3  extern PFSYSCPUGETCALLSTACKENTRY3 pfSysCpuGetCallstackEntry3;
	#define GET_SysCpuGetCallstackEntry3(fl)  s_pfCMGetAPI2( "SysCpuGetCallstackEntry3", (RTS_VOID_FCTPTR *)&pfSysCpuGetCallstackEntry3, (fl), 0, 0)
	#define CAL_SysCpuGetCallstackEntry3  pfSysCpuGetCallstackEntry3
	#define CHK_SysCpuGetCallstackEntry3  (pfSysCpuGetCallstackEntry3 != NULL)
	#define EXP_SysCpuGetCallstackEntry3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetCallstackEntry3", (RTS_UINTPTR)SysCpuGetCallstackEntry3, 0, 0) 
#endif




/**
 * <description>Routine to find pattern on stack</description>
 * <param name="pSP" type="IN">Pointer to stack to start search</param>
 * <param name="pattern" type="IN">Pattern to search for on the stack</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to the stack, where the pattern was found or NULL if failed</result>
 */
RTS_UINTPTR * CDECL SysCpuSearchOnStack_(RTS_UINTPTR *pSP, RTS_UINTPTR pattern, RTS_RESULT *pResult);
typedef RTS_UINTPTR * (CDECL * PFSYSCPUSEARCHONSTACK_) (RTS_UINTPTR *pSP, RTS_UINTPTR pattern, RTS_RESULT *pResult);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUSEARCHONSTACK__NOTIMPLEMENTED)
	#define USE_SysCpuSearchOnStack_
	#define EXT_SysCpuSearchOnStack_
	#define GET_SysCpuSearchOnStack_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuSearchOnStack_(p0,p1,p2)  (RTS_UINTPTR *)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuSearchOnStack_  FALSE
	#define EXP_SysCpuSearchOnStack_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuSearchOnStack_
	#define EXT_SysCpuSearchOnStack_
	#define GET_SysCpuSearchOnStack_(fl)  CAL_CMGETAPI( "SysCpuSearchOnStack_" ) 
	#define CAL_SysCpuSearchOnStack_  SysCpuSearchOnStack_
	#define CHK_SysCpuSearchOnStack_  TRUE
	#define EXP_SysCpuSearchOnStack_  CAL_CMEXPAPI( "SysCpuSearchOnStack_" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuSearchOnStack_
	#define EXT_SysCpuSearchOnStack_
	#define GET_SysCpuSearchOnStack_(fl)  CAL_CMGETAPI( "SysCpuSearchOnStack_" ) 
	#define CAL_SysCpuSearchOnStack_  SysCpuSearchOnStack_
	#define CHK_SysCpuSearchOnStack_  TRUE
	#define EXP_SysCpuSearchOnStack_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuSearchOnStack_", (RTS_UINTPTR)SysCpuSearchOnStack_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuSearchOnStack_
	#define EXT_SysCpuHandlingSysCpuSearchOnStack_
	#define GET_SysCpuHandlingSysCpuSearchOnStack_  ERR_OK
	#define CAL_SysCpuHandlingSysCpuSearchOnStack_ pISysCpuHandling->ISysCpuSearchOnStack_
	#define CHK_SysCpuHandlingSysCpuSearchOnStack_ (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuSearchOnStack_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuSearchOnStack_
	#define EXT_SysCpuSearchOnStack_
	#define GET_SysCpuSearchOnStack_(fl)  CAL_CMGETAPI( "SysCpuSearchOnStack_" ) 
	#define CAL_SysCpuSearchOnStack_ pISysCpuHandling->ISysCpuSearchOnStack_
	#define CHK_SysCpuSearchOnStack_ (pISysCpuHandling != NULL)
	#define EXP_SysCpuSearchOnStack_  CAL_CMEXPAPI( "SysCpuSearchOnStack_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuSearchOnStack_  PFSYSCPUSEARCHONSTACK_ pfSysCpuSearchOnStack_;
	#define EXT_SysCpuSearchOnStack_  extern PFSYSCPUSEARCHONSTACK_ pfSysCpuSearchOnStack_;
	#define GET_SysCpuSearchOnStack_(fl)  s_pfCMGetAPI2( "SysCpuSearchOnStack_", (RTS_VOID_FCTPTR *)&pfSysCpuSearchOnStack_, (fl), 0, 0)
	#define CAL_SysCpuSearchOnStack_  pfSysCpuSearchOnStack_
	#define CHK_SysCpuSearchOnStack_  (pfSysCpuSearchOnStack_ != NULL)
	#define EXP_SysCpuSearchOnStack_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuSearchOnStack_", (RTS_UINTPTR)SysCpuSearchOnStack_, 0, 0) 
#endif




/**
 * <description>Routine to retrieve the instance pointer of an FB. Is used to get an instance specific callstack.
 *	The position of the instance pointer depends on the CoDeSys codegenerator and is cpu dependant.</description>
 * <param name="pBP" type="INOUT">Pointer to last base pointer entry (or frame pointer) and returns
 *	the pointer to the next base pointer in the stack frame</param>
 * <param name="ppInstancePointer" type="OUT">Pointer pointer to the instance pointer of an FB</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysCpuGetInstancePointer(RTS_UINTPTR *pBP, void** ppInstancePointer);
typedef RTS_RESULT (CDECL * PFSYSCPUGETINSTANCEPOINTER) (RTS_UINTPTR *pBP, void** ppInstancePointer);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUGETINSTANCEPOINTER_NOTIMPLEMENTED)
	#define USE_SysCpuGetInstancePointer
	#define EXT_SysCpuGetInstancePointer
	#define GET_SysCpuGetInstancePointer(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuGetInstancePointer(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuGetInstancePointer  FALSE
	#define EXP_SysCpuGetInstancePointer  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuGetInstancePointer
	#define EXT_SysCpuGetInstancePointer
	#define GET_SysCpuGetInstancePointer(fl)  CAL_CMGETAPI( "SysCpuGetInstancePointer" ) 
	#define CAL_SysCpuGetInstancePointer  SysCpuGetInstancePointer
	#define CHK_SysCpuGetInstancePointer  TRUE
	#define EXP_SysCpuGetInstancePointer  CAL_CMEXPAPI( "SysCpuGetInstancePointer" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuGetInstancePointer
	#define EXT_SysCpuGetInstancePointer
	#define GET_SysCpuGetInstancePointer(fl)  CAL_CMGETAPI( "SysCpuGetInstancePointer" ) 
	#define CAL_SysCpuGetInstancePointer  SysCpuGetInstancePointer
	#define CHK_SysCpuGetInstancePointer  TRUE
	#define EXP_SysCpuGetInstancePointer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetInstancePointer", (RTS_UINTPTR)SysCpuGetInstancePointer, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuGetInstancePointer
	#define EXT_SysCpuHandlingSysCpuGetInstancePointer
	#define GET_SysCpuHandlingSysCpuGetInstancePointer  ERR_OK
	#define CAL_SysCpuHandlingSysCpuGetInstancePointer pISysCpuHandling->ISysCpuGetInstancePointer
	#define CHK_SysCpuHandlingSysCpuGetInstancePointer (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuGetInstancePointer  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuGetInstancePointer
	#define EXT_SysCpuGetInstancePointer
	#define GET_SysCpuGetInstancePointer(fl)  CAL_CMGETAPI( "SysCpuGetInstancePointer" ) 
	#define CAL_SysCpuGetInstancePointer pISysCpuHandling->ISysCpuGetInstancePointer
	#define CHK_SysCpuGetInstancePointer (pISysCpuHandling != NULL)
	#define EXP_SysCpuGetInstancePointer  CAL_CMEXPAPI( "SysCpuGetInstancePointer" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuGetInstancePointer  PFSYSCPUGETINSTANCEPOINTER pfSysCpuGetInstancePointer;
	#define EXT_SysCpuGetInstancePointer  extern PFSYSCPUGETINSTANCEPOINTER pfSysCpuGetInstancePointer;
	#define GET_SysCpuGetInstancePointer(fl)  s_pfCMGetAPI2( "SysCpuGetInstancePointer", (RTS_VOID_FCTPTR *)&pfSysCpuGetInstancePointer, (fl), 0, 0)
	#define CAL_SysCpuGetInstancePointer  pfSysCpuGetInstancePointer
	#define CHK_SysCpuGetInstancePointer  (pfSysCpuGetInstancePointer != NULL)
	#define EXP_SysCpuGetInstancePointer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetInstancePointer", (RTS_UINTPTR)SysCpuGetInstancePointer, 0, 0) 
#endif




/**
 * <description>Routine to retrieve the frame pointer for monitoring data access.</description>
 * <param name="pBP" type="INOUT">Pointer to last base pointer entry (or frame pointer) and returns
 *	the pointer on the stack for monitoring data access</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysCpuGetMonitoringBase(RTS_UINTPTR *pBP);
typedef RTS_RESULT (CDECL * PFSYSCPUGETMONITORINGBASE) (RTS_UINTPTR *pBP);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUGETMONITORINGBASE_NOTIMPLEMENTED)
	#define USE_SysCpuGetMonitoringBase
	#define EXT_SysCpuGetMonitoringBase
	#define GET_SysCpuGetMonitoringBase(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuGetMonitoringBase(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuGetMonitoringBase  FALSE
	#define EXP_SysCpuGetMonitoringBase  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuGetMonitoringBase
	#define EXT_SysCpuGetMonitoringBase
	#define GET_SysCpuGetMonitoringBase(fl)  CAL_CMGETAPI( "SysCpuGetMonitoringBase" ) 
	#define CAL_SysCpuGetMonitoringBase  SysCpuGetMonitoringBase
	#define CHK_SysCpuGetMonitoringBase  TRUE
	#define EXP_SysCpuGetMonitoringBase  CAL_CMEXPAPI( "SysCpuGetMonitoringBase" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuGetMonitoringBase
	#define EXT_SysCpuGetMonitoringBase
	#define GET_SysCpuGetMonitoringBase(fl)  CAL_CMGETAPI( "SysCpuGetMonitoringBase" ) 
	#define CAL_SysCpuGetMonitoringBase  SysCpuGetMonitoringBase
	#define CHK_SysCpuGetMonitoringBase  TRUE
	#define EXP_SysCpuGetMonitoringBase  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetMonitoringBase", (RTS_UINTPTR)SysCpuGetMonitoringBase, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuGetMonitoringBase
	#define EXT_SysCpuHandlingSysCpuGetMonitoringBase
	#define GET_SysCpuHandlingSysCpuGetMonitoringBase  ERR_OK
	#define CAL_SysCpuHandlingSysCpuGetMonitoringBase pISysCpuHandling->ISysCpuGetMonitoringBase
	#define CHK_SysCpuHandlingSysCpuGetMonitoringBase (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuGetMonitoringBase  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuGetMonitoringBase
	#define EXT_SysCpuGetMonitoringBase
	#define GET_SysCpuGetMonitoringBase(fl)  CAL_CMGETAPI( "SysCpuGetMonitoringBase" ) 
	#define CAL_SysCpuGetMonitoringBase pISysCpuHandling->ISysCpuGetMonitoringBase
	#define CHK_SysCpuGetMonitoringBase (pISysCpuHandling != NULL)
	#define EXP_SysCpuGetMonitoringBase  CAL_CMEXPAPI( "SysCpuGetMonitoringBase" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuGetMonitoringBase  PFSYSCPUGETMONITORINGBASE pfSysCpuGetMonitoringBase;
	#define EXT_SysCpuGetMonitoringBase  extern PFSYSCPUGETMONITORINGBASE pfSysCpuGetMonitoringBase;
	#define GET_SysCpuGetMonitoringBase(fl)  s_pfCMGetAPI2( "SysCpuGetMonitoringBase", (RTS_VOID_FCTPTR *)&pfSysCpuGetMonitoringBase, (fl), 0, 0)
	#define CAL_SysCpuGetMonitoringBase  pfSysCpuGetMonitoringBase
	#define CHK_SysCpuGetMonitoringBase  (pfSysCpuGetMonitoringBase != NULL)
	#define EXP_SysCpuGetMonitoringBase  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetMonitoringBase", (RTS_UINTPTR)SysCpuGetMonitoringBase, 0, 0) 
#endif




/**
 * <description>Obsolete: Use SysCpuTestAndSetBit instead!
 * <p>OBSOLETE function to test and set a bit in an ULONG variable in one processor step. This operation must be atomic to provide
 *	a multitasking save operation.</p>
 *	<p>IMPLEMENTATION NOTE: Try to use a processor opcode, that provides this operation. If such an opcode
 *	is not available, use SysCpuTestAndSetBase in your platform adaptation.</p>
 * </description>
 * <param name="pul" type="IN">Pointer to the unsigned value to test an set a bit inside in one atomic
 *	processor step</param>
 * <parampseudo name="iBitOut" type="OUT">Bit that was really set</parampseudo>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_OK: If bit could be set and was not set before</li>
 *		<li>ERR_FAILED: If bit is still set</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL SysCpuTestAndSet(RTS_UI32* pul, int iBit);
typedef RTS_RESULT (CDECL * PFSYSCPUTESTANDSET) (RTS_UI32* pul, int iBit);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUTESTANDSET_NOTIMPLEMENTED)
	#define USE_SysCpuTestAndSet
	#define EXT_SysCpuTestAndSet
	#define GET_SysCpuTestAndSet(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuTestAndSet(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuTestAndSet  FALSE
	#define EXP_SysCpuTestAndSet  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuTestAndSet
	#define EXT_SysCpuTestAndSet
	#define GET_SysCpuTestAndSet(fl)  CAL_CMGETAPI( "SysCpuTestAndSet" ) 
	#define CAL_SysCpuTestAndSet  SysCpuTestAndSet
	#define CHK_SysCpuTestAndSet  TRUE
	#define EXP_SysCpuTestAndSet  CAL_CMEXPAPI( "SysCpuTestAndSet" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuTestAndSet
	#define EXT_SysCpuTestAndSet
	#define GET_SysCpuTestAndSet(fl)  CAL_CMGETAPI( "SysCpuTestAndSet" ) 
	#define CAL_SysCpuTestAndSet  SysCpuTestAndSet
	#define CHK_SysCpuTestAndSet  TRUE
	#define EXP_SysCpuTestAndSet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuTestAndSet", (RTS_UINTPTR)SysCpuTestAndSet, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuTestAndSet
	#define EXT_SysCpuHandlingSysCpuTestAndSet
	#define GET_SysCpuHandlingSysCpuTestAndSet  ERR_OK
	#define CAL_SysCpuHandlingSysCpuTestAndSet pISysCpuHandling->ISysCpuTestAndSet
	#define CHK_SysCpuHandlingSysCpuTestAndSet (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuTestAndSet  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuTestAndSet
	#define EXT_SysCpuTestAndSet
	#define GET_SysCpuTestAndSet(fl)  CAL_CMGETAPI( "SysCpuTestAndSet" ) 
	#define CAL_SysCpuTestAndSet pISysCpuHandling->ISysCpuTestAndSet
	#define CHK_SysCpuTestAndSet (pISysCpuHandling != NULL)
	#define EXP_SysCpuTestAndSet  CAL_CMEXPAPI( "SysCpuTestAndSet" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuTestAndSet  PFSYSCPUTESTANDSET pfSysCpuTestAndSet;
	#define EXT_SysCpuTestAndSet  extern PFSYSCPUTESTANDSET pfSysCpuTestAndSet;
	#define GET_SysCpuTestAndSet(fl)  s_pfCMGetAPI2( "SysCpuTestAndSet", (RTS_VOID_FCTPTR *)&pfSysCpuTestAndSet, (fl), 0, 0)
	#define CAL_SysCpuTestAndSet  pfSysCpuTestAndSet
	#define CHK_SysCpuTestAndSet  (pfSysCpuTestAndSet != NULL)
	#define EXP_SysCpuTestAndSet  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuTestAndSet", (RTS_UINTPTR)SysCpuTestAndSet, 0, 0) 
#endif




/**
 * <description>Obsolete: Use SysCpuTestAndSetBit instead!
 * <p>OBSOLETE function to reset a bit in an ULONG variable in one processor step. This operation must be atomic to provide
 *	a multitasking save operation.</p>
 *	<p>IMPLEMENTATION NOTE: Try to use a processor opcode, that provides this operation. If such an opcode
 *	is not available, use SysCpuTestAndResetBase in your platform adaptation.</p>
 * </description>
 * <param name="pul" type="IN">Pointer to the unsigned value to test an reset a bit inside in one atomic
 *	processor step</param>
 * <param name="iBit" type="IN">Bit number inside the variable to test and reset. 0=first bit, 31=last bit</param>
 * <result>
 *	<ul>
 *		<li>ERR_OK: If bit could be reset</li>
 *		<li>ERR_FAILED: If bit reset failed</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL SysCpuTestAndReset(RTS_UI32* pul, int iBit);
typedef RTS_RESULT (CDECL * PFSYSCPUTESTANDRESET) (RTS_UI32* pul, int iBit);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUTESTANDRESET_NOTIMPLEMENTED)
	#define USE_SysCpuTestAndReset
	#define EXT_SysCpuTestAndReset
	#define GET_SysCpuTestAndReset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuTestAndReset(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuTestAndReset  FALSE
	#define EXP_SysCpuTestAndReset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuTestAndReset
	#define EXT_SysCpuTestAndReset
	#define GET_SysCpuTestAndReset(fl)  CAL_CMGETAPI( "SysCpuTestAndReset" ) 
	#define CAL_SysCpuTestAndReset  SysCpuTestAndReset
	#define CHK_SysCpuTestAndReset  TRUE
	#define EXP_SysCpuTestAndReset  CAL_CMEXPAPI( "SysCpuTestAndReset" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuTestAndReset
	#define EXT_SysCpuTestAndReset
	#define GET_SysCpuTestAndReset(fl)  CAL_CMGETAPI( "SysCpuTestAndReset" ) 
	#define CAL_SysCpuTestAndReset  SysCpuTestAndReset
	#define CHK_SysCpuTestAndReset  TRUE
	#define EXP_SysCpuTestAndReset  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuTestAndReset", (RTS_UINTPTR)SysCpuTestAndReset, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuTestAndReset
	#define EXT_SysCpuHandlingSysCpuTestAndReset
	#define GET_SysCpuHandlingSysCpuTestAndReset  ERR_OK
	#define CAL_SysCpuHandlingSysCpuTestAndReset pISysCpuHandling->ISysCpuTestAndReset
	#define CHK_SysCpuHandlingSysCpuTestAndReset (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuTestAndReset  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuTestAndReset
	#define EXT_SysCpuTestAndReset
	#define GET_SysCpuTestAndReset(fl)  CAL_CMGETAPI( "SysCpuTestAndReset" ) 
	#define CAL_SysCpuTestAndReset pISysCpuHandling->ISysCpuTestAndReset
	#define CHK_SysCpuTestAndReset (pISysCpuHandling != NULL)
	#define EXP_SysCpuTestAndReset  CAL_CMEXPAPI( "SysCpuTestAndReset" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuTestAndReset  PFSYSCPUTESTANDRESET pfSysCpuTestAndReset;
	#define EXT_SysCpuTestAndReset  extern PFSYSCPUTESTANDRESET pfSysCpuTestAndReset;
	#define GET_SysCpuTestAndReset(fl)  s_pfCMGetAPI2( "SysCpuTestAndReset", (RTS_VOID_FCTPTR *)&pfSysCpuTestAndReset, (fl), 0, 0)
	#define CAL_SysCpuTestAndReset  pfSysCpuTestAndReset
	#define CHK_SysCpuTestAndReset  (pfSysCpuTestAndReset != NULL)
	#define EXP_SysCpuTestAndReset  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuTestAndReset", (RTS_UINTPTR)SysCpuTestAndReset, 0, 0) 
#endif




/**
 * <description>
 *  <p>The function test and set or clear a bit in a variable in one processor step. This operation must be atomic to provide a multitasking save operation.</p>
 *	<p>IMPLEMENTATION NOTE: Try to use a processor opcode, that provides this operation. If such an opcode
 *	is not available, use SysCpuTestAndSetBitBase in your platform adaptation. The function returns ERR_FAILED if the bit was already set or reset.</p>
 * </description>
 * <param name="pAddress" type="IN" range="[NULL,ADDR_ALIGN0,ADDR_ALIGN1,ADDR_ALIGN2,ADDR_ALIGN3]">Pointer to test and set or clear a bit inside in one atomic processor step.
 *									NOTE: The pointer must be natural aligned! nLen=2: pAddress must be 2Byte aligned; nLen=4: pAddress must be 4Byte aligned</param>
 * <param name="nLen" type="IN" range="[1,2,4,INVALID_LEN]">Size of the value behind the address. Can only be 1 (unsigned char), 2 (unsigned short) or 4 (unsigned long)</param>
 * <param name="iBit" type="IN" range="[INT_MIN..0,0..7,8..15,16..31,32..INT_MAX]">Bit number inside the variable to test and set or clear:
 *	<ul>
 *		<li>nLen = 1: iBit 0..7</li>
 *		<li>nLen = 2: iBit 0..15</li>
 *		<li>nLen = 4: iBit 0..31</li>
 *	</ul>
 * </param>
 * <param name="bSet" type="IN" range="[0,1]">1=Set bit, 0=Clear bit</param>
 * <param name="iBit" type="IN">Bit number inside the variable to test and set. 0=first bit, 31=last bit</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Bit could be set/reset successfully</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">Bit could not be set/reset successfully, perhaps it already was set/reset</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">pAddress may not be null or misaligned, nLen may only be 1,2,4, iBit must be in correct range</errorcode>
 * <result>Error code: returns if Bit could be set/reset successfully, or if any  problem occured</result>
 */
RTS_RESULT CDECL SysCpuTestAndSetBit(void* pAddress, int nLen, int iBit, int bSet);
typedef RTS_RESULT (CDECL * PFSYSCPUTESTANDSETBIT) (void* pAddress, int nLen, int iBit, int bSet);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUTESTANDSETBIT_NOTIMPLEMENTED)
	#define USE_SysCpuTestAndSetBit
	#define EXT_SysCpuTestAndSetBit
	#define GET_SysCpuTestAndSetBit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuTestAndSetBit(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuTestAndSetBit  FALSE
	#define EXP_SysCpuTestAndSetBit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuTestAndSetBit
	#define EXT_SysCpuTestAndSetBit
	#define GET_SysCpuTestAndSetBit(fl)  CAL_CMGETAPI( "SysCpuTestAndSetBit" ) 
	#define CAL_SysCpuTestAndSetBit  SysCpuTestAndSetBit
	#define CHK_SysCpuTestAndSetBit  TRUE
	#define EXP_SysCpuTestAndSetBit  CAL_CMEXPAPI( "SysCpuTestAndSetBit" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuTestAndSetBit
	#define EXT_SysCpuTestAndSetBit
	#define GET_SysCpuTestAndSetBit(fl)  CAL_CMGETAPI( "SysCpuTestAndSetBit" ) 
	#define CAL_SysCpuTestAndSetBit  SysCpuTestAndSetBit
	#define CHK_SysCpuTestAndSetBit  TRUE
	#define EXP_SysCpuTestAndSetBit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuTestAndSetBit", (RTS_UINTPTR)SysCpuTestAndSetBit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuTestAndSetBit
	#define EXT_SysCpuHandlingSysCpuTestAndSetBit
	#define GET_SysCpuHandlingSysCpuTestAndSetBit  ERR_OK
	#define CAL_SysCpuHandlingSysCpuTestAndSetBit pISysCpuHandling->ISysCpuTestAndSetBit
	#define CHK_SysCpuHandlingSysCpuTestAndSetBit (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuTestAndSetBit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuTestAndSetBit
	#define EXT_SysCpuTestAndSetBit
	#define GET_SysCpuTestAndSetBit(fl)  CAL_CMGETAPI( "SysCpuTestAndSetBit" ) 
	#define CAL_SysCpuTestAndSetBit pISysCpuHandling->ISysCpuTestAndSetBit
	#define CHK_SysCpuTestAndSetBit (pISysCpuHandling != NULL)
	#define EXP_SysCpuTestAndSetBit  CAL_CMEXPAPI( "SysCpuTestAndSetBit" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuTestAndSetBit  PFSYSCPUTESTANDSETBIT pfSysCpuTestAndSetBit;
	#define EXT_SysCpuTestAndSetBit  extern PFSYSCPUTESTANDSETBIT pfSysCpuTestAndSetBit;
	#define GET_SysCpuTestAndSetBit(fl)  s_pfCMGetAPI2( "SysCpuTestAndSetBit", (RTS_VOID_FCTPTR *)&pfSysCpuTestAndSetBit, (fl), 0, 0)
	#define CAL_SysCpuTestAndSetBit  pfSysCpuTestAndSetBit
	#define CHK_SysCpuTestAndSetBit  (pfSysCpuTestAndSetBit != NULL)
	#define EXP_SysCpuTestAndSetBit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuTestAndSetBit", (RTS_UINTPTR)SysCpuTestAndSetBit, 0, 0) 
#endif




/**
 * <description>
 *	Get the actual register context.
 * </description>
 * <param name="pContext" type="OUT">Actual register context</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysCpuGetContext(RegContext *pContext);
typedef RTS_RESULT (CDECL * PFSYSCPUGETCONTEXT) (RegContext *pContext);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUGETCONTEXT_NOTIMPLEMENTED)
	#define USE_SysCpuGetContext
	#define EXT_SysCpuGetContext
	#define GET_SysCpuGetContext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuGetContext(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuGetContext  FALSE
	#define EXP_SysCpuGetContext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuGetContext
	#define EXT_SysCpuGetContext
	#define GET_SysCpuGetContext(fl)  CAL_CMGETAPI( "SysCpuGetContext" ) 
	#define CAL_SysCpuGetContext  SysCpuGetContext
	#define CHK_SysCpuGetContext  TRUE
	#define EXP_SysCpuGetContext  CAL_CMEXPAPI( "SysCpuGetContext" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuGetContext
	#define EXT_SysCpuGetContext
	#define GET_SysCpuGetContext(fl)  CAL_CMGETAPI( "SysCpuGetContext" ) 
	#define CAL_SysCpuGetContext  SysCpuGetContext
	#define CHK_SysCpuGetContext  TRUE
	#define EXP_SysCpuGetContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetContext", (RTS_UINTPTR)SysCpuGetContext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuGetContext
	#define EXT_SysCpuHandlingSysCpuGetContext
	#define GET_SysCpuHandlingSysCpuGetContext  ERR_OK
	#define CAL_SysCpuHandlingSysCpuGetContext pISysCpuHandling->ISysCpuGetContext
	#define CHK_SysCpuHandlingSysCpuGetContext (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuGetContext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuGetContext
	#define EXT_SysCpuGetContext
	#define GET_SysCpuGetContext(fl)  CAL_CMGETAPI( "SysCpuGetContext" ) 
	#define CAL_SysCpuGetContext pISysCpuHandling->ISysCpuGetContext
	#define CHK_SysCpuGetContext (pISysCpuHandling != NULL)
	#define EXP_SysCpuGetContext  CAL_CMEXPAPI( "SysCpuGetContext" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuGetContext  PFSYSCPUGETCONTEXT pfSysCpuGetContext;
	#define EXT_SysCpuGetContext  extern PFSYSCPUGETCONTEXT pfSysCpuGetContext;
	#define GET_SysCpuGetContext(fl)  s_pfCMGetAPI2( "SysCpuGetContext", (RTS_VOID_FCTPTR *)&pfSysCpuGetContext, (fl), 0, 0)
	#define CAL_SysCpuGetContext  pfSysCpuGetContext
	#define CHK_SysCpuGetContext  (pfSysCpuGetContext != NULL)
	#define EXP_SysCpuGetContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuGetContext", (RTS_UINTPTR)SysCpuGetContext, 0, 0) 
#endif




/**
 * <description>
 * <p>Function to increment the content of the given pointer by nSum in one atomic operation (task safe).</p>
 * <p>IMPLEMENTATION NOTE: Add or substract the value to/from the content of the pointer,
 * and return the value after this operation atomically.</p>
 * </description>
 * <param name="piValue" type="INOUT" range="[NULL,VALID_ATOMICADD_ADDR]">Pointer to the value to increment</param>
 * <param name="nSum" type="IN" range="[RTS_RANGE_OF_RTS_I32]">Summand for the operation. greater 0 to increment, lower 0 to decrement</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns the value after the increment operation in an atomic way!</result>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Call was sucessful</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">piValue was NULL</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOTIMPLEMENTED">Function is not supported</errorcode>
 */
RTS_I32 CDECL SysCpuAtomicAdd(RTS_I32 *piValue, RTS_I32 nSum, RTS_RESULT *pResult);
typedef RTS_I32 (CDECL * PFSYSCPUATOMICADD) (RTS_I32 *piValue, RTS_I32 nSum, RTS_RESULT *pResult);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUATOMICADD_NOTIMPLEMENTED)
	#define USE_SysCpuAtomicAdd
	#define EXT_SysCpuAtomicAdd
	#define GET_SysCpuAtomicAdd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuAtomicAdd(p0,p1,p2)  (RTS_I32)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuAtomicAdd  FALSE
	#define EXP_SysCpuAtomicAdd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuAtomicAdd
	#define EXT_SysCpuAtomicAdd
	#define GET_SysCpuAtomicAdd(fl)  CAL_CMGETAPI( "SysCpuAtomicAdd" ) 
	#define CAL_SysCpuAtomicAdd  SysCpuAtomicAdd
	#define CHK_SysCpuAtomicAdd  TRUE
	#define EXP_SysCpuAtomicAdd  CAL_CMEXPAPI( "SysCpuAtomicAdd" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuAtomicAdd
	#define EXT_SysCpuAtomicAdd
	#define GET_SysCpuAtomicAdd(fl)  CAL_CMGETAPI( "SysCpuAtomicAdd" ) 
	#define CAL_SysCpuAtomicAdd  SysCpuAtomicAdd
	#define CHK_SysCpuAtomicAdd  TRUE
	#define EXP_SysCpuAtomicAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuAtomicAdd", (RTS_UINTPTR)SysCpuAtomicAdd, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuAtomicAdd
	#define EXT_SysCpuHandlingSysCpuAtomicAdd
	#define GET_SysCpuHandlingSysCpuAtomicAdd  ERR_OK
	#define CAL_SysCpuHandlingSysCpuAtomicAdd pISysCpuHandling->ISysCpuAtomicAdd
	#define CHK_SysCpuHandlingSysCpuAtomicAdd (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuAtomicAdd  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuAtomicAdd
	#define EXT_SysCpuAtomicAdd
	#define GET_SysCpuAtomicAdd(fl)  CAL_CMGETAPI( "SysCpuAtomicAdd" ) 
	#define CAL_SysCpuAtomicAdd pISysCpuHandling->ISysCpuAtomicAdd
	#define CHK_SysCpuAtomicAdd (pISysCpuHandling != NULL)
	#define EXP_SysCpuAtomicAdd  CAL_CMEXPAPI( "SysCpuAtomicAdd" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuAtomicAdd  PFSYSCPUATOMICADD pfSysCpuAtomicAdd;
	#define EXT_SysCpuAtomicAdd  extern PFSYSCPUATOMICADD pfSysCpuAtomicAdd;
	#define GET_SysCpuAtomicAdd(fl)  s_pfCMGetAPI2( "SysCpuAtomicAdd", (RTS_VOID_FCTPTR *)&pfSysCpuAtomicAdd, (fl), 0, 0)
	#define CAL_SysCpuAtomicAdd  pfSysCpuAtomicAdd
	#define CHK_SysCpuAtomicAdd  (pfSysCpuAtomicAdd != NULL)
	#define EXP_SysCpuAtomicAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuAtomicAdd", (RTS_UINTPTR)SysCpuAtomicAdd, 0, 0) 
#endif




/**
 * <description>
 * <p>Function to increment the content of the given pointer by nSum in one atomic operation (task safe).</p>
 * <p>IMPLEMENTATION NOTE: Add or substract the value to/from the content of the pointer,
 * and return the value after this operation atomically.</p>
 * </description>
 * <param name="pi64Value" type="INOUT">Pointer to the value to increment</param>
 * <param name="nSum" type="IN">Summand for the operation. greater 0 to increment, lower 0 to decrement</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns the value after the increment operation in an atomic way!</result>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Call was sucessful</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">pi64Value was NULL</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOT_SUPPORTED">Function is not supported</errorcode>
 */
RTS_I64 CDECL SysCpuAtomicAdd64(RTS_I64 *pi64Value, RTS_I64 nSum, RTS_RESULT *pResult);
typedef RTS_I64 (CDECL * PFSYSCPUATOMICADD64) (RTS_I64 *pi64Value, RTS_I64 nSum, RTS_RESULT *pResult);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUATOMICADD64_NOTIMPLEMENTED)
	#define USE_SysCpuAtomicAdd64
	#define EXT_SysCpuAtomicAdd64
	#define GET_SysCpuAtomicAdd64(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuAtomicAdd64(p0,p1,p2)  (RTS_I64)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuAtomicAdd64  FALSE
	#define EXP_SysCpuAtomicAdd64  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuAtomicAdd64
	#define EXT_SysCpuAtomicAdd64
	#define GET_SysCpuAtomicAdd64(fl)  CAL_CMGETAPI( "SysCpuAtomicAdd64" ) 
	#define CAL_SysCpuAtomicAdd64  SysCpuAtomicAdd64
	#define CHK_SysCpuAtomicAdd64  TRUE
	#define EXP_SysCpuAtomicAdd64  CAL_CMEXPAPI( "SysCpuAtomicAdd64" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuAtomicAdd64
	#define EXT_SysCpuAtomicAdd64
	#define GET_SysCpuAtomicAdd64(fl)  CAL_CMGETAPI( "SysCpuAtomicAdd64" ) 
	#define CAL_SysCpuAtomicAdd64  SysCpuAtomicAdd64
	#define CHK_SysCpuAtomicAdd64  TRUE
	#define EXP_SysCpuAtomicAdd64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuAtomicAdd64", (RTS_UINTPTR)SysCpuAtomicAdd64, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuAtomicAdd64
	#define EXT_SysCpuHandlingSysCpuAtomicAdd64
	#define GET_SysCpuHandlingSysCpuAtomicAdd64  ERR_OK
	#define CAL_SysCpuHandlingSysCpuAtomicAdd64 pISysCpuHandling->ISysCpuAtomicAdd64
	#define CHK_SysCpuHandlingSysCpuAtomicAdd64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuAtomicAdd64  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuAtomicAdd64
	#define EXT_SysCpuAtomicAdd64
	#define GET_SysCpuAtomicAdd64(fl)  CAL_CMGETAPI( "SysCpuAtomicAdd64" ) 
	#define CAL_SysCpuAtomicAdd64 pISysCpuHandling->ISysCpuAtomicAdd64
	#define CHK_SysCpuAtomicAdd64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuAtomicAdd64  CAL_CMEXPAPI( "SysCpuAtomicAdd64" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuAtomicAdd64  PFSYSCPUATOMICADD64 pfSysCpuAtomicAdd64;
	#define EXT_SysCpuAtomicAdd64  extern PFSYSCPUATOMICADD64 pfSysCpuAtomicAdd64;
	#define GET_SysCpuAtomicAdd64(fl)  s_pfCMGetAPI2( "SysCpuAtomicAdd64", (RTS_VOID_FCTPTR *)&pfSysCpuAtomicAdd64, (fl), 0, 0)
	#define CAL_SysCpuAtomicAdd64  pfSysCpuAtomicAdd64
	#define CHK_SysCpuAtomicAdd64  (pfSysCpuAtomicAdd64 != NULL)
	#define EXP_SysCpuAtomicAdd64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuAtomicAdd64", (RTS_UINTPTR)SysCpuAtomicAdd64, 0, 0) 
#endif




/**
 * <description>
 * Function for compare-and-swap in one atomic operation (task safe).
 * Supports only 32- and 64-bit types (i.e. a valid valueSize is 4 or 8)
 * </description>
 * <param name="pAddress" type="INOUT">Pointer to the value to handle</param>
 * <param name="pSwapValue" type="IN">Pointer to the swap value</param>
 * <param name="pCompareValue" type="IN">Pointer to the compare value</param>
 * <param name="valueSize" type="IN">Size of the value in bytes (a valid valueSize is 4 or 8)</param>
 * <result>Returns errorcode</result>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Swap was successful</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">Swap was not successful</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">At least one of the pointers was NULL</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOT_SUPPORTED">Swap not supported for this byte size or on this platform</errorcode>
 */
RTS_RESULT CDECL SysCpuAtomicCompareAndSwap(void *pAddress, void *pSwapValue, void *pCompareValue, RTS_UI8 valueSize);
typedef RTS_RESULT (CDECL * PFSYSCPUATOMICCOMPAREANDSWAP) (void *pAddress, void *pSwapValue, void *pCompareValue, RTS_UI8 valueSize);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUATOMICCOMPAREANDSWAP_NOTIMPLEMENTED)
	#define USE_SysCpuAtomicCompareAndSwap
	#define EXT_SysCpuAtomicCompareAndSwap
	#define GET_SysCpuAtomicCompareAndSwap(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuAtomicCompareAndSwap(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuAtomicCompareAndSwap  FALSE
	#define EXP_SysCpuAtomicCompareAndSwap  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuAtomicCompareAndSwap
	#define EXT_SysCpuAtomicCompareAndSwap
	#define GET_SysCpuAtomicCompareAndSwap(fl)  CAL_CMGETAPI( "SysCpuAtomicCompareAndSwap" ) 
	#define CAL_SysCpuAtomicCompareAndSwap  SysCpuAtomicCompareAndSwap
	#define CHK_SysCpuAtomicCompareAndSwap  TRUE
	#define EXP_SysCpuAtomicCompareAndSwap  CAL_CMEXPAPI( "SysCpuAtomicCompareAndSwap" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuAtomicCompareAndSwap
	#define EXT_SysCpuAtomicCompareAndSwap
	#define GET_SysCpuAtomicCompareAndSwap(fl)  CAL_CMGETAPI( "SysCpuAtomicCompareAndSwap" ) 
	#define CAL_SysCpuAtomicCompareAndSwap  SysCpuAtomicCompareAndSwap
	#define CHK_SysCpuAtomicCompareAndSwap  TRUE
	#define EXP_SysCpuAtomicCompareAndSwap  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuAtomicCompareAndSwap", (RTS_UINTPTR)SysCpuAtomicCompareAndSwap, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuAtomicCompareAndSwap
	#define EXT_SysCpuHandlingSysCpuAtomicCompareAndSwap
	#define GET_SysCpuHandlingSysCpuAtomicCompareAndSwap  ERR_OK
	#define CAL_SysCpuHandlingSysCpuAtomicCompareAndSwap pISysCpuHandling->ISysCpuAtomicCompareAndSwap
	#define CHK_SysCpuHandlingSysCpuAtomicCompareAndSwap (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuAtomicCompareAndSwap  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuAtomicCompareAndSwap
	#define EXT_SysCpuAtomicCompareAndSwap
	#define GET_SysCpuAtomicCompareAndSwap(fl)  CAL_CMGETAPI( "SysCpuAtomicCompareAndSwap" ) 
	#define CAL_SysCpuAtomicCompareAndSwap pISysCpuHandling->ISysCpuAtomicCompareAndSwap
	#define CHK_SysCpuAtomicCompareAndSwap (pISysCpuHandling != NULL)
	#define EXP_SysCpuAtomicCompareAndSwap  CAL_CMEXPAPI( "SysCpuAtomicCompareAndSwap" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuAtomicCompareAndSwap  PFSYSCPUATOMICCOMPAREANDSWAP pfSysCpuAtomicCompareAndSwap;
	#define EXT_SysCpuAtomicCompareAndSwap  extern PFSYSCPUATOMICCOMPAREANDSWAP pfSysCpuAtomicCompareAndSwap;
	#define GET_SysCpuAtomicCompareAndSwap(fl)  s_pfCMGetAPI2( "SysCpuAtomicCompareAndSwap", (RTS_VOID_FCTPTR *)&pfSysCpuAtomicCompareAndSwap, (fl), 0, 0)
	#define CAL_SysCpuAtomicCompareAndSwap  pfSysCpuAtomicCompareAndSwap
	#define CHK_SysCpuAtomicCompareAndSwap  (pfSysCpuAtomicCompareAndSwap != NULL)
	#define EXP_SysCpuAtomicCompareAndSwap  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuAtomicCompareAndSwap", (RTS_UINTPTR)SysCpuAtomicCompareAndSwap, 0, 0) 
#endif




/**
 * <description>
 * Function to read a 64bit integer value atomic / consistent.
 * </description>
 * <param name="pSrc" type="IN">Pointer to the value to read</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns the read value</result>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Call was sucessful</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">pSrc is NULL</errorcode> 
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOT_SUPPORTED">Atomic function is not supported!</errorcode>
 */
RTS_I64 CDECL SysCpuReadInt64(RTS_I64 *pSrc, RTS_RESULT *pResult);
typedef RTS_I64 (CDECL * PFSYSCPUREADINT64) (RTS_I64 *pSrc, RTS_RESULT *pResult);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUREADINT64_NOTIMPLEMENTED)
	#define USE_SysCpuReadInt64
	#define EXT_SysCpuReadInt64
	#define GET_SysCpuReadInt64(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuReadInt64(p0,p1)  (RTS_I64)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuReadInt64  FALSE
	#define EXP_SysCpuReadInt64  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuReadInt64
	#define EXT_SysCpuReadInt64
	#define GET_SysCpuReadInt64(fl)  CAL_CMGETAPI( "SysCpuReadInt64" ) 
	#define CAL_SysCpuReadInt64  SysCpuReadInt64
	#define CHK_SysCpuReadInt64  TRUE
	#define EXP_SysCpuReadInt64  CAL_CMEXPAPI( "SysCpuReadInt64" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuReadInt64
	#define EXT_SysCpuReadInt64
	#define GET_SysCpuReadInt64(fl)  CAL_CMGETAPI( "SysCpuReadInt64" ) 
	#define CAL_SysCpuReadInt64  SysCpuReadInt64
	#define CHK_SysCpuReadInt64  TRUE
	#define EXP_SysCpuReadInt64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuReadInt64", (RTS_UINTPTR)SysCpuReadInt64, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuReadInt64
	#define EXT_SysCpuHandlingSysCpuReadInt64
	#define GET_SysCpuHandlingSysCpuReadInt64  ERR_OK
	#define CAL_SysCpuHandlingSysCpuReadInt64 pISysCpuHandling->ISysCpuReadInt64
	#define CHK_SysCpuHandlingSysCpuReadInt64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuReadInt64  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuReadInt64
	#define EXT_SysCpuReadInt64
	#define GET_SysCpuReadInt64(fl)  CAL_CMGETAPI( "SysCpuReadInt64" ) 
	#define CAL_SysCpuReadInt64 pISysCpuHandling->ISysCpuReadInt64
	#define CHK_SysCpuReadInt64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuReadInt64  CAL_CMEXPAPI( "SysCpuReadInt64" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuReadInt64  PFSYSCPUREADINT64 pfSysCpuReadInt64;
	#define EXT_SysCpuReadInt64  extern PFSYSCPUREADINT64 pfSysCpuReadInt64;
	#define GET_SysCpuReadInt64(fl)  s_pfCMGetAPI2( "SysCpuReadInt64", (RTS_VOID_FCTPTR *)&pfSysCpuReadInt64, (fl), 0, 0)
	#define CAL_SysCpuReadInt64  pfSysCpuReadInt64
	#define CHK_SysCpuReadInt64  (pfSysCpuReadInt64 != NULL)
	#define EXP_SysCpuReadInt64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuReadInt64", (RTS_UINTPTR)SysCpuReadInt64, 0, 0) 
#endif




/**
 * <description>
 * Function to write a 64bit integer value atomic / consistent.
 * </description>
 * <param name="pDest" type="IN">Pointer to the value to write</param>
 * <param name="i64Value" type="IN">Value to write</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Call was sucessful</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pDest is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOT_SUPPORTED">Atomic function is not supported!</errorcode>
 */
RTS_RESULT CDECL SysCpuWriteInt64(RTS_I64 *pDest, RTS_I64 i64Value);
typedef RTS_RESULT (CDECL * PFSYSCPUWRITEINT64) (RTS_I64 *pDest, RTS_I64 i64Value);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUWRITEINT64_NOTIMPLEMENTED)
	#define USE_SysCpuWriteInt64
	#define EXT_SysCpuWriteInt64
	#define GET_SysCpuWriteInt64(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuWriteInt64(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuWriteInt64  FALSE
	#define EXP_SysCpuWriteInt64  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuWriteInt64
	#define EXT_SysCpuWriteInt64
	#define GET_SysCpuWriteInt64(fl)  CAL_CMGETAPI( "SysCpuWriteInt64" ) 
	#define CAL_SysCpuWriteInt64  SysCpuWriteInt64
	#define CHK_SysCpuWriteInt64  TRUE
	#define EXP_SysCpuWriteInt64  CAL_CMEXPAPI( "SysCpuWriteInt64" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuWriteInt64
	#define EXT_SysCpuWriteInt64
	#define GET_SysCpuWriteInt64(fl)  CAL_CMGETAPI( "SysCpuWriteInt64" ) 
	#define CAL_SysCpuWriteInt64  SysCpuWriteInt64
	#define CHK_SysCpuWriteInt64  TRUE
	#define EXP_SysCpuWriteInt64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuWriteInt64", (RTS_UINTPTR)SysCpuWriteInt64, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuWriteInt64
	#define EXT_SysCpuHandlingSysCpuWriteInt64
	#define GET_SysCpuHandlingSysCpuWriteInt64  ERR_OK
	#define CAL_SysCpuHandlingSysCpuWriteInt64 pISysCpuHandling->ISysCpuWriteInt64
	#define CHK_SysCpuHandlingSysCpuWriteInt64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuWriteInt64  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuWriteInt64
	#define EXT_SysCpuWriteInt64
	#define GET_SysCpuWriteInt64(fl)  CAL_CMGETAPI( "SysCpuWriteInt64" ) 
	#define CAL_SysCpuWriteInt64 pISysCpuHandling->ISysCpuWriteInt64
	#define CHK_SysCpuWriteInt64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuWriteInt64  CAL_CMEXPAPI( "SysCpuWriteInt64" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuWriteInt64  PFSYSCPUWRITEINT64 pfSysCpuWriteInt64;
	#define EXT_SysCpuWriteInt64  extern PFSYSCPUWRITEINT64 pfSysCpuWriteInt64;
	#define GET_SysCpuWriteInt64(fl)  s_pfCMGetAPI2( "SysCpuWriteInt64", (RTS_VOID_FCTPTR *)&pfSysCpuWriteInt64, (fl), 0, 0)
	#define CAL_SysCpuWriteInt64  pfSysCpuWriteInt64
	#define CHK_SysCpuWriteInt64  (pfSysCpuWriteInt64 != NULL)
	#define EXP_SysCpuWriteInt64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuWriteInt64", (RTS_UINTPTR)SysCpuWriteInt64, 0, 0) 
#endif




/**
 * <description>
 * Function to read a 64bit real value atomic / consistent.
 * </description>
 * <param name="pSrc" type="IN">Pointer to the value to read</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns the read value</result>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Call was sucessful</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">pSrc is NULL</errorcode> 
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOT_SUPPORTED">Atomic function is not supported!</errorcode>
 */
RTS_REAL64 CDECL SysCpuReadReal64(RTS_REAL64 *pSrc, RTS_RESULT *pResult);
typedef RTS_REAL64 (CDECL * PFSYSCPUREADREAL64) (RTS_REAL64 *pSrc, RTS_RESULT *pResult);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUREADREAL64_NOTIMPLEMENTED)
	#define USE_SysCpuReadReal64
	#define EXT_SysCpuReadReal64
	#define GET_SysCpuReadReal64(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuReadReal64(p0,p1)  (RTS_REAL64)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuReadReal64  FALSE
	#define EXP_SysCpuReadReal64  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuReadReal64
	#define EXT_SysCpuReadReal64
	#define GET_SysCpuReadReal64(fl)  CAL_CMGETAPI( "SysCpuReadReal64" ) 
	#define CAL_SysCpuReadReal64  SysCpuReadReal64
	#define CHK_SysCpuReadReal64  TRUE
	#define EXP_SysCpuReadReal64  CAL_CMEXPAPI( "SysCpuReadReal64" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuReadReal64
	#define EXT_SysCpuReadReal64
	#define GET_SysCpuReadReal64(fl)  CAL_CMGETAPI( "SysCpuReadReal64" ) 
	#define CAL_SysCpuReadReal64  SysCpuReadReal64
	#define CHK_SysCpuReadReal64  TRUE
	#define EXP_SysCpuReadReal64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuReadReal64", (RTS_UINTPTR)SysCpuReadReal64, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuReadReal64
	#define EXT_SysCpuHandlingSysCpuReadReal64
	#define GET_SysCpuHandlingSysCpuReadReal64  ERR_OK
	#define CAL_SysCpuHandlingSysCpuReadReal64 pISysCpuHandling->ISysCpuReadReal64
	#define CHK_SysCpuHandlingSysCpuReadReal64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuReadReal64  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuReadReal64
	#define EXT_SysCpuReadReal64
	#define GET_SysCpuReadReal64(fl)  CAL_CMGETAPI( "SysCpuReadReal64" ) 
	#define CAL_SysCpuReadReal64 pISysCpuHandling->ISysCpuReadReal64
	#define CHK_SysCpuReadReal64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuReadReal64  CAL_CMEXPAPI( "SysCpuReadReal64" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuReadReal64  PFSYSCPUREADREAL64 pfSysCpuReadReal64;
	#define EXT_SysCpuReadReal64  extern PFSYSCPUREADREAL64 pfSysCpuReadReal64;
	#define GET_SysCpuReadReal64(fl)  s_pfCMGetAPI2( "SysCpuReadReal64", (RTS_VOID_FCTPTR *)&pfSysCpuReadReal64, (fl), 0, 0)
	#define CAL_SysCpuReadReal64  pfSysCpuReadReal64
	#define CHK_SysCpuReadReal64  (pfSysCpuReadReal64 != NULL)
	#define EXP_SysCpuReadReal64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuReadReal64", (RTS_UINTPTR)SysCpuReadReal64, 0, 0) 
#endif




/**
 * <description>
 * Function to write a 64bit real value atomic / consistent.
 * </description>
 * <param name="pDest" type="IN">Pointer to the value to write</param>
 * <param name="r64Value" type="IN">Value to write</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Call was sucessful</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pDest is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOT_SUPPORTED">Atomic function is not supported!</errorcode>
 */
RTS_RESULT CDECL SysCpuWriteReal64(RTS_REAL64 *pDest, RTS_REAL64 r64Value);
typedef RTS_RESULT (CDECL * PFSYSCPUWRITEREAL64) (RTS_REAL64 *pDest, RTS_REAL64 r64Value);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUWRITEREAL64_NOTIMPLEMENTED)
	#define USE_SysCpuWriteReal64
	#define EXT_SysCpuWriteReal64
	#define GET_SysCpuWriteReal64(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuWriteReal64(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuWriteReal64  FALSE
	#define EXP_SysCpuWriteReal64  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuWriteReal64
	#define EXT_SysCpuWriteReal64
	#define GET_SysCpuWriteReal64(fl)  CAL_CMGETAPI( "SysCpuWriteReal64" ) 
	#define CAL_SysCpuWriteReal64  SysCpuWriteReal64
	#define CHK_SysCpuWriteReal64  TRUE
	#define EXP_SysCpuWriteReal64  CAL_CMEXPAPI( "SysCpuWriteReal64" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuWriteReal64
	#define EXT_SysCpuWriteReal64
	#define GET_SysCpuWriteReal64(fl)  CAL_CMGETAPI( "SysCpuWriteReal64" ) 
	#define CAL_SysCpuWriteReal64  SysCpuWriteReal64
	#define CHK_SysCpuWriteReal64  TRUE
	#define EXP_SysCpuWriteReal64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuWriteReal64", (RTS_UINTPTR)SysCpuWriteReal64, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuWriteReal64
	#define EXT_SysCpuHandlingSysCpuWriteReal64
	#define GET_SysCpuHandlingSysCpuWriteReal64  ERR_OK
	#define CAL_SysCpuHandlingSysCpuWriteReal64 pISysCpuHandling->ISysCpuWriteReal64
	#define CHK_SysCpuHandlingSysCpuWriteReal64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuWriteReal64  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuWriteReal64
	#define EXT_SysCpuWriteReal64
	#define GET_SysCpuWriteReal64(fl)  CAL_CMGETAPI( "SysCpuWriteReal64" ) 
	#define CAL_SysCpuWriteReal64 pISysCpuHandling->ISysCpuWriteReal64
	#define CHK_SysCpuWriteReal64 (pISysCpuHandling != NULL)
	#define EXP_SysCpuWriteReal64  CAL_CMEXPAPI( "SysCpuWriteReal64" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuWriteReal64  PFSYSCPUWRITEREAL64 pfSysCpuWriteReal64;
	#define EXT_SysCpuWriteReal64  extern PFSYSCPUWRITEREAL64 pfSysCpuWriteReal64;
	#define GET_SysCpuWriteReal64(fl)  s_pfCMGetAPI2( "SysCpuWriteReal64", (RTS_VOID_FCTPTR *)&pfSysCpuWriteReal64, (fl), 0, 0)
	#define CAL_SysCpuWriteReal64  pfSysCpuWriteReal64
	#define CHK_SysCpuWriteReal64  (pfSysCpuWriteReal64 != NULL)
	#define EXP_SysCpuWriteReal64  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuWriteReal64", (RTS_UINTPTR)SysCpuWriteReal64, 0, 0) 
#endif




/**
 * <description>
 * This function will write values with with 1, 2, 4 or 8 byte size as an atomic operation.
 * Note: pDest has to be aligned according to the nLen parameter. If the alignment isn't correct the function will fail.
 * </description>
 * <param name="pDest" type="IN">Pointer to the destination address. The value will be written to this address. The write operation to this address will be atomic.</param>
 * <param name="pSrc" type="IN">Pointer to the source address. The value will be read from this address.</param>
 * <param name="nLen" type="IN">Size of the value to be written. 1, 2, 4 and 8 will be accepted.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Call was sucessful</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pDest, pSrc is NULL or nLen isn't supported.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOT_SUPPORTED">Atomic function is not supported!</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_ALIGNMENT">pDest was not aligned according to nLen.</errorcode>
 */
RTS_RESULT CDECL SysCpuWriteValueAtomic(void* pDest, void* pSrc, RTS_UI32 nLen);
typedef RTS_RESULT (CDECL * PFSYSCPUWRITEVALUEATOMIC) (void* pDest, void* pSrc, RTS_UI32 nLen);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUWRITEVALUEATOMIC_NOTIMPLEMENTED)
	#define USE_SysCpuWriteValueAtomic
	#define EXT_SysCpuWriteValueAtomic
	#define GET_SysCpuWriteValueAtomic(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuWriteValueAtomic(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuWriteValueAtomic  FALSE
	#define EXP_SysCpuWriteValueAtomic  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuWriteValueAtomic
	#define EXT_SysCpuWriteValueAtomic
	#define GET_SysCpuWriteValueAtomic(fl)  CAL_CMGETAPI( "SysCpuWriteValueAtomic" ) 
	#define CAL_SysCpuWriteValueAtomic  SysCpuWriteValueAtomic
	#define CHK_SysCpuWriteValueAtomic  TRUE
	#define EXP_SysCpuWriteValueAtomic  CAL_CMEXPAPI( "SysCpuWriteValueAtomic" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuWriteValueAtomic
	#define EXT_SysCpuWriteValueAtomic
	#define GET_SysCpuWriteValueAtomic(fl)  CAL_CMGETAPI( "SysCpuWriteValueAtomic" ) 
	#define CAL_SysCpuWriteValueAtomic  SysCpuWriteValueAtomic
	#define CHK_SysCpuWriteValueAtomic  TRUE
	#define EXP_SysCpuWriteValueAtomic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuWriteValueAtomic", (RTS_UINTPTR)SysCpuWriteValueAtomic, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuWriteValueAtomic
	#define EXT_SysCpuHandlingSysCpuWriteValueAtomic
	#define GET_SysCpuHandlingSysCpuWriteValueAtomic  ERR_OK
	#define CAL_SysCpuHandlingSysCpuWriteValueAtomic pISysCpuHandling->ISysCpuWriteValueAtomic
	#define CHK_SysCpuHandlingSysCpuWriteValueAtomic (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuWriteValueAtomic  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuWriteValueAtomic
	#define EXT_SysCpuWriteValueAtomic
	#define GET_SysCpuWriteValueAtomic(fl)  CAL_CMGETAPI( "SysCpuWriteValueAtomic" ) 
	#define CAL_SysCpuWriteValueAtomic pISysCpuHandling->ISysCpuWriteValueAtomic
	#define CHK_SysCpuWriteValueAtomic (pISysCpuHandling != NULL)
	#define EXP_SysCpuWriteValueAtomic  CAL_CMEXPAPI( "SysCpuWriteValueAtomic" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuWriteValueAtomic  PFSYSCPUWRITEVALUEATOMIC pfSysCpuWriteValueAtomic;
	#define EXT_SysCpuWriteValueAtomic  extern PFSYSCPUWRITEVALUEATOMIC pfSysCpuWriteValueAtomic;
	#define GET_SysCpuWriteValueAtomic(fl)  s_pfCMGetAPI2( "SysCpuWriteValueAtomic", (RTS_VOID_FCTPTR *)&pfSysCpuWriteValueAtomic, (fl), 0, 0)
	#define CAL_SysCpuWriteValueAtomic  pfSysCpuWriteValueAtomic
	#define CHK_SysCpuWriteValueAtomic  (pfSysCpuWriteValueAtomic != NULL)
	#define EXP_SysCpuWriteValueAtomic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuWriteValueAtomic", (RTS_UINTPTR)SysCpuWriteValueAtomic, 0, 0) 
#endif




/**
 * <description>
 * This function will read values with with 1, 2, 4 or 8 byte size as an atomic operation.
 * Note: pSrc has to be aligned according to the nLen parameter. If the alignment isn't correct the function will fail.
 * </description>
 * <param name="pSrc" type="IN">Pointer to the source address. The value will be read from this address. The read operation from this address will be atomic.</param>
 * <param name="pDst" type="IN">Pointer to the destination address. The value will be written to this address.</param>
 * <param name="nLen" type="IN">Size of the value to be read. 1, 2, 4 and 8 will be accepted.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Call was sucessful</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pSrc or pDest is NULL or nLen isn't supported.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOT_SUPPORTED">Atomic function is not supported!</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_ALIGNMENT">pSrc was not aligned according to nLen.</errorcode>
 */
RTS_RESULT CDECL SysCpuReadValueAtomic(void* pSrc, void* pDest, RTS_UI32 nLen);
typedef RTS_RESULT (CDECL * PFSYSCPUREADVALUEATOMIC) (void* pSrc, void* pDest, RTS_UI32 nLen);
#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUREADVALUEATOMIC_NOTIMPLEMENTED)
	#define USE_SysCpuReadValueAtomic
	#define EXT_SysCpuReadValueAtomic
	#define GET_SysCpuReadValueAtomic(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysCpuReadValueAtomic(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysCpuReadValueAtomic  FALSE
	#define EXP_SysCpuReadValueAtomic  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysCpuReadValueAtomic
	#define EXT_SysCpuReadValueAtomic
	#define GET_SysCpuReadValueAtomic(fl)  CAL_CMGETAPI( "SysCpuReadValueAtomic" ) 
	#define CAL_SysCpuReadValueAtomic  SysCpuReadValueAtomic
	#define CHK_SysCpuReadValueAtomic  TRUE
	#define EXP_SysCpuReadValueAtomic  CAL_CMEXPAPI( "SysCpuReadValueAtomic" ) 
#elif defined(MIXED_LINK) && !defined(SYSCPUHANDLING_EXTERNAL)
	#define USE_SysCpuReadValueAtomic
	#define EXT_SysCpuReadValueAtomic
	#define GET_SysCpuReadValueAtomic(fl)  CAL_CMGETAPI( "SysCpuReadValueAtomic" ) 
	#define CAL_SysCpuReadValueAtomic  SysCpuReadValueAtomic
	#define CHK_SysCpuReadValueAtomic  TRUE
	#define EXP_SysCpuReadValueAtomic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuReadValueAtomic", (RTS_UINTPTR)SysCpuReadValueAtomic, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysCpuHandlingSysCpuReadValueAtomic
	#define EXT_SysCpuHandlingSysCpuReadValueAtomic
	#define GET_SysCpuHandlingSysCpuReadValueAtomic  ERR_OK
	#define CAL_SysCpuHandlingSysCpuReadValueAtomic pISysCpuHandling->ISysCpuReadValueAtomic
	#define CHK_SysCpuHandlingSysCpuReadValueAtomic (pISysCpuHandling != NULL)
	#define EXP_SysCpuHandlingSysCpuReadValueAtomic  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysCpuReadValueAtomic
	#define EXT_SysCpuReadValueAtomic
	#define GET_SysCpuReadValueAtomic(fl)  CAL_CMGETAPI( "SysCpuReadValueAtomic" ) 
	#define CAL_SysCpuReadValueAtomic pISysCpuHandling->ISysCpuReadValueAtomic
	#define CHK_SysCpuReadValueAtomic (pISysCpuHandling != NULL)
	#define EXP_SysCpuReadValueAtomic  CAL_CMEXPAPI( "SysCpuReadValueAtomic" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysCpuReadValueAtomic  PFSYSCPUREADVALUEATOMIC pfSysCpuReadValueAtomic;
	#define EXT_SysCpuReadValueAtomic  extern PFSYSCPUREADVALUEATOMIC pfSysCpuReadValueAtomic;
	#define GET_SysCpuReadValueAtomic(fl)  s_pfCMGetAPI2( "SysCpuReadValueAtomic", (RTS_VOID_FCTPTR *)&pfSysCpuReadValueAtomic, (fl), 0, 0)
	#define CAL_SysCpuReadValueAtomic  pfSysCpuReadValueAtomic
	#define CHK_SysCpuReadValueAtomic  (pfSysCpuReadValueAtomic != NULL)
	#define EXP_SysCpuReadValueAtomic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysCpuReadValueAtomic", (RTS_UINTPTR)SysCpuReadValueAtomic, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif




typedef struct
{
	IBase_C *pBase;
	PFSYSCPUFLUSHINSTRUCTIONCACHE ISysCpuFlushInstructionCache;
 	PFSYSCPUCALLIECFUNCWITHPARAMS ISysCpuCallIecFuncWithParams;
 	PFSYSCPUSETREGISTERVALUE ISysCpuSetRegisterValue;
 	PFSYSCPUGETBREAKPOINTCODE ISysCpuGetBreakpointCode;
 	PFSYSCPUGETCALLSTACKENTRY ISysCpuGetCallstackEntry;
 	PFSYSCPUGETCALLSTACKENTRY2 ISysCpuGetCallstackEntry2;
 	PFSYSCPUGETCALLSTACKENTRY3 ISysCpuGetCallstackEntry3;
 	PFSYSCPUSEARCHONSTACK_ ISysCpuSearchOnStack_;
 	PFSYSCPUGETINSTANCEPOINTER ISysCpuGetInstancePointer;
 	PFSYSCPUGETMONITORINGBASE ISysCpuGetMonitoringBase;
 	PFSYSCPUTESTANDSET ISysCpuTestAndSet;
 	PFSYSCPUTESTANDRESET ISysCpuTestAndReset;
 	PFSYSCPUTESTANDSETBIT ISysCpuTestAndSetBit;
 	PFSYSCPUGETCONTEXT ISysCpuGetContext;
 	PFSYSCPUATOMICADD ISysCpuAtomicAdd;
 	PFSYSCPUATOMICADD64 ISysCpuAtomicAdd64;
 	PFSYSCPUATOMICCOMPAREANDSWAP ISysCpuAtomicCompareAndSwap;
 	PFSYSCPUREADINT64 ISysCpuReadInt64;
 	PFSYSCPUWRITEINT64 ISysCpuWriteInt64;
 	PFSYSCPUREADREAL64 ISysCpuReadReal64;
 	PFSYSCPUWRITEREAL64 ISysCpuWriteReal64;
 	PFSYSCPUWRITEVALUEATOMIC ISysCpuWriteValueAtomic;
 	PFSYSCPUREADVALUEATOMIC ISysCpuReadValueAtomic;
 } ISysCpuHandling_C;

#ifdef CPLUSPLUS
class ISysCpuHandling : public IBase
{
	public:
		virtual int CDECL ISysCpuFlushInstructionCache(void * pBaseAddress, unsigned long ulSize) =0;
		virtual RTS_RESULT CDECL ISysCpuCallIecFuncWithParams(RTS_VOID_FCTPTR pfIECFunc, void* pParam, int iSize) =0;
		virtual RTS_RESULT CDECL ISysCpuSetRegisterValue(RTS_UINTPTR *pRegBuff, RTS_I32 nRegisterNumber, RTS_UINTPTR newValue) =0;
		virtual RTS_RESULT CDECL ISysCpuGetBreakpointCode(unsigned char* pbyAreaStart, unsigned char* pbyBPAddress, unsigned char* pbyOpCode, int *piOpcodeSize) =0;
		virtual RTS_RESULT CDECL ISysCpuGetCallstackEntry(RTS_UINTPTR *pBP, void **ppAddress) =0;
		virtual RTS_RESULT CDECL ISysCpuGetCallstackEntry2(RTS_I32 bIecCode, RTS_UINTPTR *pBP, void **ppAddress) =0;
		virtual RTS_RESULT CDECL ISysCpuGetCallstackEntry3(RTS_I32 bIecCode, RegContext *pContext, void **ppAddress) =0;
		virtual RTS_UINTPTR * CDECL ISysCpuSearchOnStack_(RTS_UINTPTR *pSP, RTS_UINTPTR pattern, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysCpuGetInstancePointer(RTS_UINTPTR *pBP, void** ppInstancePointer) =0;
		virtual RTS_RESULT CDECL ISysCpuGetMonitoringBase(RTS_UINTPTR *pBP) =0;
		virtual RTS_RESULT CDECL ISysCpuTestAndSet(RTS_UI32* pul, int iBit) =0;
		virtual RTS_RESULT CDECL ISysCpuTestAndReset(RTS_UI32* pul, int iBit) =0;
		virtual RTS_RESULT CDECL ISysCpuTestAndSetBit(void* pAddress, int nLen, int iBit, int bSet) =0;
		virtual RTS_RESULT CDECL ISysCpuGetContext(RegContext *pContext) =0;
		virtual RTS_I32 CDECL ISysCpuAtomicAdd(RTS_I32 *piValue, RTS_I32 nSum, RTS_RESULT *pResult) =0;
		virtual RTS_I64 CDECL ISysCpuAtomicAdd64(RTS_I64 *pi64Value, RTS_I64 nSum, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysCpuAtomicCompareAndSwap(void *pAddress, void *pSwapValue, void *pCompareValue, RTS_UI8 valueSize) =0;
		virtual RTS_I64 CDECL ISysCpuReadInt64(RTS_I64 *pSrc, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysCpuWriteInt64(RTS_I64 *pDest, RTS_I64 i64Value) =0;
		virtual RTS_REAL64 CDECL ISysCpuReadReal64(RTS_REAL64 *pSrc, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysCpuWriteReal64(RTS_REAL64 *pDest, RTS_REAL64 r64Value) =0;
		virtual RTS_RESULT CDECL ISysCpuWriteValueAtomic(void* pDest, void* pSrc, RTS_UI32 nLen) =0;
		virtual RTS_RESULT CDECL ISysCpuReadValueAtomic(void* pSrc, void* pDest, RTS_UI32 nLen) =0;
};
	#ifndef ITF_SysCpuHandling
		#define ITF_SysCpuHandling static ISysCpuHandling *pISysCpuHandling = NULL;
	#endif
	#define EXTITF_SysCpuHandling
#else	/*CPLUSPLUS*/
	typedef ISysCpuHandling_C		ISysCpuHandling;
	#ifndef ITF_SysCpuHandling
		#define ITF_SysCpuHandling
	#endif
	#define EXTITF_SysCpuHandling
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSCPUHANDLINGITF_H_*/
