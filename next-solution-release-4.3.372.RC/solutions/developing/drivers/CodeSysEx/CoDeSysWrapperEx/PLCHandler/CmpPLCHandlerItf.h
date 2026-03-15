 /**
 * <interfacename>CmpPLCHandler</interfacename>
 * <description></description>
 *
 * <copyright></copyright>
 */


	
	
#ifndef _CMPPLCHANDLERITF_H_
#define _CMPPLCHANDLERITF_H_

#include "CmpStd.h"

 

 




/**
 * Instances of this structure represent the value and status information for a single variable
 *
 * .. note:: In fact this structure is the same than PlcVarValue from the PLCHandler SDK
 */
typedef struct tagVariableValue
{
	RTS_IEC_UDINT ulTimeStamp;		/* UTC-Timestamp of the value, is read from the PLC

 .. note:: The UTC-timestamp is not supported by all PLCs */
	RTS_IEC_BYTE quality;		/* Quality of the value
 * 1: Value was read from the PLC
 * 0: Value was not read from the PLC */
	RTS_IEC_BYTE data[1];		/* Array with the value. The concrete size of this array depends on the
 data type (byData[1] is only a placeholder with the minimum
 size). Should only be accessed, if bQuality is TRUE */
} VariableValue;

/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * This enumeration defines all possible error codes that can be returned
 * by the functions from this library. If you need information for the purpose
 * of these error codes exceeding the name of the error itself then please have
 * a look at the PLCHandler SDK.
 */
#define PLCHANDLERIECERROR_FAILED    RTS_IEC_INT_C(0xFFFFFFFFFFFFFFFF)	
#define PLCHANDLERIECERROR_OK    RTS_IEC_INT_C(0x0)	
#define PLCHANDLERIECERROR_PLCNOTCONNECTED    RTS_IEC_INT_C(0x1)	
#define PLCHANDLERIECERROR_PLCLOGINFAILED    RTS_IEC_INT_C(0x2)	
#define PLCHANDLERIECERROR_PLCNOCYCLICLISTDEFINED    RTS_IEC_INT_C(0x3)	
#define PLCHANDLERIECERROR_PLCHANDLERINACTIVE    RTS_IEC_INT_C(0x4)	
#define PLCHANDLERIECERROR_LOADINGSYMBOLSFAILED    RTS_IEC_INT_C(0x5)	
#define PLCHANDLERIECERROR_ITFNOTSUPPORTED    RTS_IEC_INT_C(0x6)	
#define PLCHANDLERIECERROR_COMMFATAL    RTS_IEC_INT_C(0x7)	
#define PLCHANDLERIECERROR_NOCONFIGURATION    RTS_IEC_INT_C(0x8)	
#define PLCHANDLERIECERROR_INVALIDPARAMETER    RTS_IEC_INT_C(0x9)	
#define PLCHANDLERIECERROR_ITFFAILED    RTS_IEC_INT_C(0xA)	
#define PLCHANDLERIECERROR_NOTSUPPORTED    RTS_IEC_INT_C(0xB)	
#define PLCHANDLERIECERROR_EXCEPTION    RTS_IEC_INT_C(0xC)	
#define PLCHANDLERIECERROR_TIMEOUT    RTS_IEC_INT_C(0xD)	
#define PLCHANDLERIECERROR_STILLCONNECTED    RTS_IEC_INT_C(0xE)	
#define PLCHANDLERIECERROR_RECONNECTTHREADSTILLALIVE    RTS_IEC_INT_C(0xF)	
#define PLCHANDLERIECERROR_PLCNOTCONNECTEDSYMBOLSLOADED    RTS_IEC_INT_C(0x10)	
#define PLCHANDLERIECERROR_NOUPDATE    RTS_IEC_INT_C(0x11)	
#define PLCHANDLERIECERROR_OCXCONVERSIONFAILED    RTS_IEC_INT_C(0x12)	
#define PLCHANDLERIECERROR_TARGETIDMISMATCH    RTS_IEC_INT_C(0x13)	
#define PLCHANDLERIECERROR_NOOBJECT    RTS_IEC_INT_C(0x14)	
#define PLCHANDLERIECERROR_COMPONENTSNOTLOADED    RTS_IEC_INT_C(0x15)	
#define PLCHANDLERIECERROR_BUSY    RTS_IEC_INT_C(0x16)	
#define PLCHANDLERIECERROR_DISABLED    RTS_IEC_INT_C(0x17)	
#define PLCHANDLERIECERROR_PLCFAILED    RTS_IEC_INT_C(0x18)	
#define PLCHANDLERIECERROR_INVALIDSYMBOL    RTS_IEC_INT_C(0x19)	
#define PLCHANDLERIECERROR_BUFFERTOOSMALL    RTS_IEC_INT_C(0x1A)	
#define PLCHANDLERIECERROR_NOPROJECT    RTS_IEC_INT_C(0x1B)	
#define PLCHANDLERIECERROR_FILEERROR    RTS_IEC_INT_C(0x1C)	
#define PLCHANDLERIECERROR_RETAINMISMATCH    RTS_IEC_INT_C(0x1D)	
#define PLCHANDLERIECERROR_NOACCESSRIGHTS    RTS_IEC_INT_C(0x1E)	
#define PLCHANDLERIECERROR_DUPLICATEPLCNAME    RTS_IEC_INT_C(0x1F)	
#define PLCHANDLERIECERROR_SIZEMISMATCH    RTS_IEC_INT_C(0x20)	
#define PLCHANDLERIECERROR_LISTNOWRITEACCESS    RTS_IEC_INT_C(0x21)	
#define PLCHANDLERIECERROR_CONSISTENTACCESSTIMEOUT    RTS_IEC_INT_C(0x22)	
#define PLCHANDLERIECERROR_SYNCCONSISTENTACCESSDENIED    RTS_IEC_INT_C(0x23)	
#define PLCHANDLERIECERROR_INVALIDASCIISTRING    RTS_IEC_INT_C(0x24)	
#define PLCHANDLERIECERROR_INVALIDSTRINGLENGTH    RTS_IEC_INT_C(0x25)	
#define PLCHANDLERIECERROR_OUTOFMEMORY    RTS_IEC_INT_C(0x26)	
#define PLCHANDLERIECERROR_NOFILE    RTS_IEC_INT_C(0x27)	
#define PLCHANDLERIECERROR_APPLICATIONNOTINSTOP    RTS_IEC_INT_C(0x28)	
#define PLCHANDLERIECERROR_APPLICATIONNOTINRUN    RTS_IEC_INT_C(0x29)	
#define PLCHANDLERIECERROR_OPERATIONMODENOTINDEBUG    RTS_IEC_INT_C(0x2A)	
#define PLCHANDLERIECERROR_BACKUPRESTORENOTSUPPORTED    RTS_IEC_INT_C(0x2B)	
#define PLCHANDLERIECERROR_PLCINCONSISTENTSTATE    RTS_IEC_INT_C(0x2C)	
#define PLCHANDLERIECERROR_PLCINCOMPATIBLE    RTS_IEC_INT_C(0x2D)	
#define PLCHANDLERIECERROR_PLCVERSIONINCOMPATIBLE    RTS_IEC_INT_C(0x2E)	
#define PLCHANDLERIECERROR_RETAINERROR    RTS_IEC_INT_C(0x2F)	
#define PLCHANDLERIECERROR_APPLICATIONLOADERROR    RTS_IEC_INT_C(0x30)	
#define PLCHANDLERIECERROR_APPLICATIONSTARTERROR    RTS_IEC_INT_C(0x31)	
#define PLCHANDLERIECERROR_FILETRANSFERERROR    RTS_IEC_INT_C(0x32)	
#define PLCHANDLERIECERROR_OPERATIONDENIED    RTS_IEC_INT_C(0x33)	
#define PLCHANDLERIECERROR_FORCESACTIVE    RTS_IEC_INT_C(0x34)	
#define PLCHANDLERIECERROR_METAVERSIONMISMATCH    RTS_IEC_INT_C(0x35)	
#define PLCHANDLERIECERROR_PROTOCOLMISMATCH    RTS_IEC_INT_C(0x36)	
#define PLCHANDLERIECERROR_USERACCESSDENIED    RTS_IEC_INT_C(0x37)	
#define PLCHANDLERIECERROR_CALLAGAIN    RTS_IEC_INT_C(0x38)	
/* Typed enum definition */
#define PLCHANDLERIECERROR    RTS_IEC_INT

/**
 * This enumeration represents the state of the PLCHandler. 
 */
#define PLCHANDLERIECSTATE_TERMINATE    RTS_IEC_INT_C(0xFFFFFFFFFFFFFFFF)	/* PLCHandler currently is terminating */
#define PLCHANDLERIECSTATE_PLCNOTCONNECTED    RTS_IEC_INT_C(0x0)	/* PLC is not connected (init state) */
#define PLCHANDLERIECSTATE_PLCCONNECTED    RTS_IEC_INT_C(0x1)	/* PLC is connected */
#define PLCHANDLERIECSTATE_NOSYMBOLS    RTS_IEC_INT_C(0x2)	/* No symbols are available or symbol mismatch because of changed project */
#define PLCHANDLERIECSTATE_SYMBOLSLOADED    RTS_IEC_INT_C(0x3)	/* PLC is connected and symbols are loaded */
#define PLCHANDLERIECSTATE_RUNNING    RTS_IEC_INT_C(0x4)	/* PLC is connected, symbols are loaded and all of them are verified. 
 Now you can work correctly with the PLCHandler object */
#define PLCHANDLERIECSTATE_DISCONNECT    RTS_IEC_INT_C(0x5)	/* Connection is just getting terminated */
#define PLCHANDLERIECSTATE_NOCONFIGURATION    RTS_IEC_INT_C(0x6)	/* Configuration of the PLCHandler is invalid */
#define PLCHANDLERIECSTATE_NOTCONNECTEDSYMBOLSLOADED    RTS_IEC_INT_C(0x7)	/* PLC is currently not available, but the symbol information could be loaded offline. */
/* Typed enum definition */
#define PLCHANDLERIECSTATE    RTS_IEC_INT

/**
 * This enumeration defines the flags that can be used in calls to |SyncDefineVarList| for example.
 * For recent details in documentation have a look at the PLCHandler documentation. 
 */
#define PLCHANDLERVARLISTFLAGS_NONE    RTS_IEC_DWORD_C(0x0)	
#define PLCHANDLERVARLISTFLAGS_WRITEACCESS    RTS_IEC_DWORD_C(0x1)	
#define PLCHANDLERVARLISTFLAGS_CONSISTENTBACKGROUNDREAD    RTS_IEC_DWORD_C(0x2)	
#define PLCHANDLERVARLISTFLAGS_CONSISTENTSYNCHRONIZEDREAD    RTS_IEC_DWORD_C(0x4)	
#define PLCHANDLERVARLISTFLAGS_CONSISTENTSYNCHRONIZEDWRITE    RTS_IEC_DWORD_C(0x8)	
/* Typed enum definition */
#define PLCHANDLERVARLISTFLAGS    RTS_IEC_DWORD

/**
 * This enumeration defines all possible error codes that can be returned
 * by the functions from this library. If you need information for the purpose
 * of these error codes exceeding the name of the error itself then please have
 * a look at the PLCHandler SDK.
 */
#define PLCHANDLERIECERROR_FAILED    RTS_IEC_INT_C(0xFFFFFFFFFFFFFFFF)	
#define PLCHANDLERIECERROR_OK    RTS_IEC_INT_C(0x0)	
#define PLCHANDLERIECERROR_PLCNOTCONNECTED    RTS_IEC_INT_C(0x1)	
#define PLCHANDLERIECERROR_PLCLOGINFAILED    RTS_IEC_INT_C(0x2)	
#define PLCHANDLERIECERROR_PLCNOCYCLICLISTDEFINED    RTS_IEC_INT_C(0x3)	
#define PLCHANDLERIECERROR_PLCHANDLERINACTIVE    RTS_IEC_INT_C(0x4)	
#define PLCHANDLERIECERROR_LOADINGSYMBOLSFAILED    RTS_IEC_INT_C(0x5)	
#define PLCHANDLERIECERROR_ITFNOTSUPPORTED    RTS_IEC_INT_C(0x6)	
#define PLCHANDLERIECERROR_COMMFATAL    RTS_IEC_INT_C(0x7)	
#define PLCHANDLERIECERROR_NOCONFIGURATION    RTS_IEC_INT_C(0x8)	
#define PLCHANDLERIECERROR_INVALIDPARAMETER    RTS_IEC_INT_C(0x9)	
#define PLCHANDLERIECERROR_ITFFAILED    RTS_IEC_INT_C(0xA)	
#define PLCHANDLERIECERROR_NOTSUPPORTED    RTS_IEC_INT_C(0xB)	
#define PLCHANDLERIECERROR_EXCEPTION    RTS_IEC_INT_C(0xC)	
#define PLCHANDLERIECERROR_TIMEOUT    RTS_IEC_INT_C(0xD)	
#define PLCHANDLERIECERROR_STILLCONNECTED    RTS_IEC_INT_C(0xE)	
#define PLCHANDLERIECERROR_RECONNECTTHREADSTILLALIVE    RTS_IEC_INT_C(0xF)	
#define PLCHANDLERIECERROR_PLCNOTCONNECTEDSYMBOLSLOADED    RTS_IEC_INT_C(0x10)	
#define PLCHANDLERIECERROR_NOUPDATE    RTS_IEC_INT_C(0x11)	
#define PLCHANDLERIECERROR_OCXCONVERSIONFAILED    RTS_IEC_INT_C(0x12)	
#define PLCHANDLERIECERROR_TARGETIDMISMATCH    RTS_IEC_INT_C(0x13)	
#define PLCHANDLERIECERROR_NOOBJECT    RTS_IEC_INT_C(0x14)	
#define PLCHANDLERIECERROR_COMPONENTSNOTLOADED    RTS_IEC_INT_C(0x15)	
#define PLCHANDLERIECERROR_BUSY    RTS_IEC_INT_C(0x16)	
#define PLCHANDLERIECERROR_DISABLED    RTS_IEC_INT_C(0x17)	
#define PLCHANDLERIECERROR_PLCFAILED    RTS_IEC_INT_C(0x18)	
#define PLCHANDLERIECERROR_INVALIDSYMBOL    RTS_IEC_INT_C(0x19)	
#define PLCHANDLERIECERROR_BUFFERTOOSMALL    RTS_IEC_INT_C(0x1A)	
#define PLCHANDLERIECERROR_NOPROJECT    RTS_IEC_INT_C(0x1B)	
#define PLCHANDLERIECERROR_FILEERROR    RTS_IEC_INT_C(0x1C)	
#define PLCHANDLERIECERROR_RETAINMISMATCH    RTS_IEC_INT_C(0x1D)	
#define PLCHANDLERIECERROR_NOACCESSRIGHTS    RTS_IEC_INT_C(0x1E)	
#define PLCHANDLERIECERROR_DUPLICATEPLCNAME    RTS_IEC_INT_C(0x1F)	
#define PLCHANDLERIECERROR_SIZEMISMATCH    RTS_IEC_INT_C(0x20)	
#define PLCHANDLERIECERROR_LISTNOWRITEACCESS    RTS_IEC_INT_C(0x21)	
#define PLCHANDLERIECERROR_CONSISTENTACCESSTIMEOUT    RTS_IEC_INT_C(0x22)	
#define PLCHANDLERIECERROR_SYNCCONSISTENTACCESSDENIED    RTS_IEC_INT_C(0x23)	
#define PLCHANDLERIECERROR_INVALIDASCIISTRING    RTS_IEC_INT_C(0x24)	
#define PLCHANDLERIECERROR_INVALIDSTRINGLENGTH    RTS_IEC_INT_C(0x25)	
#define PLCHANDLERIECERROR_OUTOFMEMORY    RTS_IEC_INT_C(0x26)	
#define PLCHANDLERIECERROR_NOFILE    RTS_IEC_INT_C(0x27)	
#define PLCHANDLERIECERROR_APPLICATIONNOTINSTOP    RTS_IEC_INT_C(0x28)	
#define PLCHANDLERIECERROR_APPLICATIONNOTINRUN    RTS_IEC_INT_C(0x29)	
#define PLCHANDLERIECERROR_OPERATIONMODENOTINDEBUG    RTS_IEC_INT_C(0x2A)	
#define PLCHANDLERIECERROR_BACKUPRESTORENOTSUPPORTED    RTS_IEC_INT_C(0x2B)	
#define PLCHANDLERIECERROR_PLCINCONSISTENTSTATE    RTS_IEC_INT_C(0x2C)	
#define PLCHANDLERIECERROR_PLCINCOMPATIBLE    RTS_IEC_INT_C(0x2D)	
#define PLCHANDLERIECERROR_PLCVERSIONINCOMPATIBLE    RTS_IEC_INT_C(0x2E)	
#define PLCHANDLERIECERROR_RETAINERROR    RTS_IEC_INT_C(0x2F)	
#define PLCHANDLERIECERROR_APPLICATIONLOADERROR    RTS_IEC_INT_C(0x30)	
#define PLCHANDLERIECERROR_APPLICATIONSTARTERROR    RTS_IEC_INT_C(0x31)	
#define PLCHANDLERIECERROR_FILETRANSFERERROR    RTS_IEC_INT_C(0x32)	
#define PLCHANDLERIECERROR_OPERATIONDENIED    RTS_IEC_INT_C(0x33)	
#define PLCHANDLERIECERROR_FORCESACTIVE    RTS_IEC_INT_C(0x34)	
#define PLCHANDLERIECERROR_METAVERSIONMISMATCH    RTS_IEC_INT_C(0x35)	
#define PLCHANDLERIECERROR_PROTOCOLMISMATCH    RTS_IEC_INT_C(0x36)	
#define PLCHANDLERIECERROR_USERACCESSDENIED    RTS_IEC_INT_C(0x37)	
#define PLCHANDLERIECERROR_CALLAGAIN    RTS_IEC_INT_C(0x38)	
/* Typed enum definition */
#define PLCHANDLERIECERROR    RTS_IEC_INT

/**
 * This enumeration represents the state of the PLCHandler. 
 */
#define PLCHANDLERIECSTATE_TERMINATE    RTS_IEC_INT_C(0xFFFFFFFFFFFFFFFF)	/* PLCHandler currently is terminating */
#define PLCHANDLERIECSTATE_PLCNOTCONNECTED    RTS_IEC_INT_C(0x0)	/* PLC is not connected (init state) */
#define PLCHANDLERIECSTATE_PLCCONNECTED    RTS_IEC_INT_C(0x1)	/* PLC is connected */
#define PLCHANDLERIECSTATE_NOSYMBOLS    RTS_IEC_INT_C(0x2)	/* No symbols are available or symbol mismatch because of changed project */
#define PLCHANDLERIECSTATE_SYMBOLSLOADED    RTS_IEC_INT_C(0x3)	/* PLC is connected and symbols are loaded */
#define PLCHANDLERIECSTATE_RUNNING    RTS_IEC_INT_C(0x4)	/* PLC is connected, symbols are loaded and all of them are verified. 
 Now you can work correctly with the PLCHandler object */
#define PLCHANDLERIECSTATE_DISCONNECT    RTS_IEC_INT_C(0x5)	/* Connection is just getting terminated */
#define PLCHANDLERIECSTATE_NOCONFIGURATION    RTS_IEC_INT_C(0x6)	/* Configuration of the PLCHandler is invalid */
#define PLCHANDLERIECSTATE_NOTCONNECTEDSYMBOLSLOADED    RTS_IEC_INT_C(0x7)	/* PLC is currently not available, but the symbol information could be loaded offline. */
/* Typed enum definition */
#define PLCHANDLERIECSTATE    RTS_IEC_INT

/**
 * This enumeration defines the flags that can be used in calls to |SyncDefineVarList| for example.
 * For recent details in documentation have a look at the PLCHandler documentation. 
 */
#define PLCHANDLERVARLISTFLAGS_NONE    RTS_IEC_DWORD_C(0x0)	
#define PLCHANDLERVARLISTFLAGS_WRITEACCESS    RTS_IEC_DWORD_C(0x1)	
#define PLCHANDLERVARLISTFLAGS_CONSISTENTBACKGROUNDREAD    RTS_IEC_DWORD_C(0x2)	
#define PLCHANDLERVARLISTFLAGS_CONSISTENTSYNCHRONIZEDREAD    RTS_IEC_DWORD_C(0x4)	
#define PLCHANDLERVARLISTFLAGS_CONSISTENTSYNCHRONIZEDWRITE    RTS_IEC_DWORD_C(0x8)	
/* Typed enum definition */
#define PLCHANDLERVARLISTFLAGS    RTS_IEC_DWORD

/**
 * This type bundles information about the device the PLCHandler is connected to.
 *
 * .. note:: This structure is a subset (with different types) of the DeviceInfo structure from the PLCHandler SDK
 */
typedef struct tagRudimentaryDeviceInfo
{
	RTS_IEC_UDINT udiTargetId;		/* Target id of the device. */
	RTS_IEC_UDINT udiTargetType;		/* Target type of the device. */
	RTS_IEC_UDINT udiBufferSize;		/* Communication buffer size of the device. No service must exceed this size! */
	RTS_IEC_UDINT udiDeviceSessionId;		/* Session id of the current connection */
	RTS_IEC_BOOL xMotorola;		/* Byte order of the device: TRUE in case of big endian byte order, FALSE in case of little endian byte order */
} RudimentaryDeviceInfo;

/**
 * This function checks whether the requested symbol is valid and can be monitored 
 * by calling |VarListDefine| for example.
 * :return: An error code, ERR_OK in case the symbol is valid
 */
typedef struct tagplchandlerchecksymbolvalidity_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance to check the symbol on */
	RTS_IEC_BYTE *pstSymbol;			/* VAR_INPUT */	/* The symbol to check for validity */
	RTS_IEC_INT PLCHandlerCheckSymbolValidity;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlerchecksymbolvalidity_struct;

void CDECL CDECL_EXT plchandlerchecksymbolvalidity(plchandlerchecksymbolvalidity_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERCHECKSYMBOLVALIDITY_IEC) (plchandlerchecksymbolvalidity_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERCHECKSYMBOLVALIDITY_NOTIMPLEMENTED)
	#define USE_plchandlerchecksymbolvalidity
	#define EXT_plchandlerchecksymbolvalidity
	#define GET_plchandlerchecksymbolvalidity(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlerchecksymbolvalidity(p0) 
	#define CHK_plchandlerchecksymbolvalidity  FALSE
	#define EXP_plchandlerchecksymbolvalidity  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlerchecksymbolvalidity
	#define EXT_plchandlerchecksymbolvalidity
	#define GET_plchandlerchecksymbolvalidity(fl)  CAL_CMGETAPI( "plchandlerchecksymbolvalidity" ) 
	#define CAL_plchandlerchecksymbolvalidity  plchandlerchecksymbolvalidity
	#define CHK_plchandlerchecksymbolvalidity  TRUE
	#define EXP_plchandlerchecksymbolvalidity  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerchecksymbolvalidity", (RTS_UINTPTR)plchandlerchecksymbolvalidity, 1, RTSITF_GET_SIGNATURE(0x35B50EAA, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlerchecksymbolvalidity {(RTS_VOID_FCTPTR)plchandlerchecksymbolvalidity, "plchandlerchecksymbolvalidity", RTSITF_GET_SIGNATURE(0x35B50EAA, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlerchecksymbolvalidity
	#define EXT_plchandlerchecksymbolvalidity
	#define GET_plchandlerchecksymbolvalidity(fl)  CAL_CMGETAPI( "plchandlerchecksymbolvalidity" ) 
	#define CAL_plchandlerchecksymbolvalidity  plchandlerchecksymbolvalidity
	#define CHK_plchandlerchecksymbolvalidity  TRUE
	#define EXP_plchandlerchecksymbolvalidity  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerchecksymbolvalidity", (RTS_UINTPTR)plchandlerchecksymbolvalidity, 1, RTSITF_GET_SIGNATURE(0x35B50EAA, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlerchecksymbolvalidity
	#define EXT_CmpPLCHandlerplchandlerchecksymbolvalidity
	#define GET_CmpPLCHandlerplchandlerchecksymbolvalidity  ERR_OK
	#define CAL_CmpPLCHandlerplchandlerchecksymbolvalidity  plchandlerchecksymbolvalidity
	#define CHK_CmpPLCHandlerplchandlerchecksymbolvalidity  TRUE
	#define EXP_CmpPLCHandlerplchandlerchecksymbolvalidity  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerchecksymbolvalidity", (RTS_UINTPTR)plchandlerchecksymbolvalidity, 1, RTSITF_GET_SIGNATURE(0x35B50EAA, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlerchecksymbolvalidity
	#define EXT_plchandlerchecksymbolvalidity
	#define GET_plchandlerchecksymbolvalidity(fl)  CAL_CMGETAPI( "plchandlerchecksymbolvalidity" ) 
	#define CAL_plchandlerchecksymbolvalidity  plchandlerchecksymbolvalidity
	#define CHK_plchandlerchecksymbolvalidity  TRUE
	#define EXP_plchandlerchecksymbolvalidity  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerchecksymbolvalidity", (RTS_UINTPTR)plchandlerchecksymbolvalidity, 1, RTSITF_GET_SIGNATURE(0x35B50EAA, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlerchecksymbolvalidity  PFPLCHANDLERCHECKSYMBOLVALIDITY_IEC pfplchandlerchecksymbolvalidity;
	#define EXT_plchandlerchecksymbolvalidity  extern PFPLCHANDLERCHECKSYMBOLVALIDITY_IEC pfplchandlerchecksymbolvalidity;
	#define GET_plchandlerchecksymbolvalidity(fl)  s_pfCMGetAPI2( "plchandlerchecksymbolvalidity", (RTS_VOID_FCTPTR *)&pfplchandlerchecksymbolvalidity, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x35B50EAA, 0), 0x03051200)
	#define CAL_plchandlerchecksymbolvalidity  pfplchandlerchecksymbolvalidity
	#define CHK_plchandlerchecksymbolvalidity  (pfplchandlerchecksymbolvalidity != NULL)
	#define EXP_plchandlerchecksymbolvalidity   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerchecksymbolvalidity", (RTS_UINTPTR)plchandlerchecksymbolvalidity, 1, RTSITF_GET_SIGNATURE(0x35B50EAA, 0), 0x03051200) 
#endif


/**
 * Configures the given PLCHandler instance with the given string.
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlerconfigurebystring_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to configured. */
	RTS_IEC_STRING *pstConfiguration;	/* VAR_INPUT */	/* A POINTER to a zero terminated ASCII STRING containing the configuration of the PLCHandler. The STRING
 is expected to contain the ini-format as defined by the PLCHandler SDK. 
 The length of the given string is not limited by 80 characters. */
	RTS_IEC_INT PLCHandlerConfigureByString;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlerconfigurebystring_struct;

void CDECL CDECL_EXT plchandlerconfigurebystring(plchandlerconfigurebystring_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERCONFIGUREBYSTRING_IEC) (plchandlerconfigurebystring_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERCONFIGUREBYSTRING_NOTIMPLEMENTED)
	#define USE_plchandlerconfigurebystring
	#define EXT_plchandlerconfigurebystring
	#define GET_plchandlerconfigurebystring(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlerconfigurebystring(p0) 
	#define CHK_plchandlerconfigurebystring  FALSE
	#define EXP_plchandlerconfigurebystring  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlerconfigurebystring
	#define EXT_plchandlerconfigurebystring
	#define GET_plchandlerconfigurebystring(fl)  CAL_CMGETAPI( "plchandlerconfigurebystring" ) 
	#define CAL_plchandlerconfigurebystring  plchandlerconfigurebystring
	#define CHK_plchandlerconfigurebystring  TRUE
	#define EXP_plchandlerconfigurebystring  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconfigurebystring", (RTS_UINTPTR)plchandlerconfigurebystring, 1, RTSITF_GET_SIGNATURE(0x9A5EB7EA, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlerconfigurebystring {(RTS_VOID_FCTPTR)plchandlerconfigurebystring, "plchandlerconfigurebystring", RTSITF_GET_SIGNATURE(0x9A5EB7EA, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlerconfigurebystring
	#define EXT_plchandlerconfigurebystring
	#define GET_plchandlerconfigurebystring(fl)  CAL_CMGETAPI( "plchandlerconfigurebystring" ) 
	#define CAL_plchandlerconfigurebystring  plchandlerconfigurebystring
	#define CHK_plchandlerconfigurebystring  TRUE
	#define EXP_plchandlerconfigurebystring  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconfigurebystring", (RTS_UINTPTR)plchandlerconfigurebystring, 1, RTSITF_GET_SIGNATURE(0x9A5EB7EA, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlerconfigurebystring
	#define EXT_CmpPLCHandlerplchandlerconfigurebystring
	#define GET_CmpPLCHandlerplchandlerconfigurebystring  ERR_OK
	#define CAL_CmpPLCHandlerplchandlerconfigurebystring  plchandlerconfigurebystring
	#define CHK_CmpPLCHandlerplchandlerconfigurebystring  TRUE
	#define EXP_CmpPLCHandlerplchandlerconfigurebystring  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconfigurebystring", (RTS_UINTPTR)plchandlerconfigurebystring, 1, RTSITF_GET_SIGNATURE(0x9A5EB7EA, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlerconfigurebystring
	#define EXT_plchandlerconfigurebystring
	#define GET_plchandlerconfigurebystring(fl)  CAL_CMGETAPI( "plchandlerconfigurebystring" ) 
	#define CAL_plchandlerconfigurebystring  plchandlerconfigurebystring
	#define CHK_plchandlerconfigurebystring  TRUE
	#define EXP_plchandlerconfigurebystring  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconfigurebystring", (RTS_UINTPTR)plchandlerconfigurebystring, 1, RTSITF_GET_SIGNATURE(0x9A5EB7EA, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlerconfigurebystring  PFPLCHANDLERCONFIGUREBYSTRING_IEC pfplchandlerconfigurebystring;
	#define EXT_plchandlerconfigurebystring  extern PFPLCHANDLERCONFIGUREBYSTRING_IEC pfplchandlerconfigurebystring;
	#define GET_plchandlerconfigurebystring(fl)  s_pfCMGetAPI2( "plchandlerconfigurebystring", (RTS_VOID_FCTPTR *)&pfplchandlerconfigurebystring, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x9A5EB7EA, 0), 0x03051200)
	#define CAL_plchandlerconfigurebystring  pfplchandlerconfigurebystring
	#define CHK_plchandlerconfigurebystring  (pfplchandlerconfigurebystring != NULL)
	#define EXP_plchandlerconfigurebystring   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconfigurebystring", (RTS_UINTPTR)plchandlerconfigurebystring, 1, RTSITF_GET_SIGNATURE(0x9A5EB7EA, 0), 0x03051200) 
#endif


/**
 * Attempts to connect according to the configuration of this instance.
 * This function executes a single synchronous connection attempt. There will be no automatic reconnect attempts, neither
 * for the initial connection nor after connection losses.
 *
 * .. important:: Calling this method might take some time (eg. for scanning the network etc.)
 *    so be careful when calling from an IEC-Task. Typically this method should be called in an
 *    asynchronous way. 
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlerconnect_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance to connect. */
	RTS_IEC_INT PLCHandlerConnect;		/* VAR_OUTPUT, Enum: ERROR */
} plchandlerconnect_struct;

void CDECL CDECL_EXT plchandlerconnect(plchandlerconnect_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERCONNECT_IEC) (plchandlerconnect_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERCONNECT_NOTIMPLEMENTED)
	#define USE_plchandlerconnect
	#define EXT_plchandlerconnect
	#define GET_plchandlerconnect(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlerconnect(p0) 
	#define CHK_plchandlerconnect  FALSE
	#define EXP_plchandlerconnect  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlerconnect
	#define EXT_plchandlerconnect
	#define GET_plchandlerconnect(fl)  CAL_CMGETAPI( "plchandlerconnect" ) 
	#define CAL_plchandlerconnect  plchandlerconnect
	#define CHK_plchandlerconnect  TRUE
	#define EXP_plchandlerconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect", (RTS_UINTPTR)plchandlerconnect, 1, RTSITF_GET_SIGNATURE(0xA16ECE49, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlerconnect {(RTS_VOID_FCTPTR)plchandlerconnect, "plchandlerconnect", RTSITF_GET_SIGNATURE(0xA16ECE49, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlerconnect
	#define EXT_plchandlerconnect
	#define GET_plchandlerconnect(fl)  CAL_CMGETAPI( "plchandlerconnect" ) 
	#define CAL_plchandlerconnect  plchandlerconnect
	#define CHK_plchandlerconnect  TRUE
	#define EXP_plchandlerconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect", (RTS_UINTPTR)plchandlerconnect, 1, RTSITF_GET_SIGNATURE(0xA16ECE49, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlerconnect
	#define EXT_CmpPLCHandlerplchandlerconnect
	#define GET_CmpPLCHandlerplchandlerconnect  ERR_OK
	#define CAL_CmpPLCHandlerplchandlerconnect  plchandlerconnect
	#define CHK_CmpPLCHandlerplchandlerconnect  TRUE
	#define EXP_CmpPLCHandlerplchandlerconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect", (RTS_UINTPTR)plchandlerconnect, 1, RTSITF_GET_SIGNATURE(0xA16ECE49, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlerconnect
	#define EXT_plchandlerconnect
	#define GET_plchandlerconnect(fl)  CAL_CMGETAPI( "plchandlerconnect" ) 
	#define CAL_plchandlerconnect  plchandlerconnect
	#define CHK_plchandlerconnect  TRUE
	#define EXP_plchandlerconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect", (RTS_UINTPTR)plchandlerconnect, 1, RTSITF_GET_SIGNATURE(0xA16ECE49, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlerconnect  PFPLCHANDLERCONNECT_IEC pfplchandlerconnect;
	#define EXT_plchandlerconnect  extern PFPLCHANDLERCONNECT_IEC pfplchandlerconnect;
	#define GET_plchandlerconnect(fl)  s_pfCMGetAPI2( "plchandlerconnect", (RTS_VOID_FCTPTR *)&pfplchandlerconnect, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xA16ECE49, 0), 0x03051200)
	#define CAL_plchandlerconnect  pfplchandlerconnect
	#define CHK_plchandlerconnect  (pfplchandlerconnect != NULL)
	#define EXP_plchandlerconnect   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect", (RTS_UINTPTR)plchandlerconnect, 1, RTSITF_GET_SIGNATURE(0xA16ECE49, 0), 0x03051200) 
#endif


/**
 * Supports the new error messages for denied user access and protocol mismatch.
 * 
 * Attempts to connect according to the configuration of this instance.
 * This function executes a single synchronous connection attempt. There will be no automatic reconnect attempts, neither
 * for the initial connection nor after connection losses.
 *
 * .. important:: Calling this method might take some time (eg. for scanning the network etc.)
 *    so be careful when calling from an IEC-Task. Typically this method should be called in an
 *    asynchronous way. 
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlerconnect2_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance to connect. */
	RTS_IEC_INT PLCHandlerConnect2;		/* VAR_OUTPUT, Enum: ERROR */
} plchandlerconnect2_struct;

void CDECL CDECL_EXT plchandlerconnect2(plchandlerconnect2_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERCONNECT2_IEC) (plchandlerconnect2_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERCONNECT2_NOTIMPLEMENTED)
	#define USE_plchandlerconnect2
	#define EXT_plchandlerconnect2
	#define GET_plchandlerconnect2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlerconnect2(p0) 
	#define CHK_plchandlerconnect2  FALSE
	#define EXP_plchandlerconnect2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlerconnect2
	#define EXT_plchandlerconnect2
	#define GET_plchandlerconnect2(fl)  CAL_CMGETAPI( "plchandlerconnect2" ) 
	#define CAL_plchandlerconnect2  plchandlerconnect2
	#define CHK_plchandlerconnect2  TRUE
	#define EXP_plchandlerconnect2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect2", (RTS_UINTPTR)plchandlerconnect2, 1, RTSITF_GET_SIGNATURE(0x229609D6, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlerconnect2 {(RTS_VOID_FCTPTR)plchandlerconnect2, "plchandlerconnect2", RTSITF_GET_SIGNATURE(0x229609D6, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlerconnect2
	#define EXT_plchandlerconnect2
	#define GET_plchandlerconnect2(fl)  CAL_CMGETAPI( "plchandlerconnect2" ) 
	#define CAL_plchandlerconnect2  plchandlerconnect2
	#define CHK_plchandlerconnect2  TRUE
	#define EXP_plchandlerconnect2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect2", (RTS_UINTPTR)plchandlerconnect2, 1, RTSITF_GET_SIGNATURE(0x229609D6, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlerconnect2
	#define EXT_CmpPLCHandlerplchandlerconnect2
	#define GET_CmpPLCHandlerplchandlerconnect2  ERR_OK
	#define CAL_CmpPLCHandlerplchandlerconnect2  plchandlerconnect2
	#define CHK_CmpPLCHandlerplchandlerconnect2  TRUE
	#define EXP_CmpPLCHandlerplchandlerconnect2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect2", (RTS_UINTPTR)plchandlerconnect2, 1, RTSITF_GET_SIGNATURE(0x229609D6, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlerconnect2
	#define EXT_plchandlerconnect2
	#define GET_plchandlerconnect2(fl)  CAL_CMGETAPI( "plchandlerconnect2" ) 
	#define CAL_plchandlerconnect2  plchandlerconnect2
	#define CHK_plchandlerconnect2  TRUE
	#define EXP_plchandlerconnect2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect2", (RTS_UINTPTR)plchandlerconnect2, 1, RTSITF_GET_SIGNATURE(0x229609D6, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlerconnect2  PFPLCHANDLERCONNECT2_IEC pfplchandlerconnect2;
	#define EXT_plchandlerconnect2  extern PFPLCHANDLERCONNECT2_IEC pfplchandlerconnect2;
	#define GET_plchandlerconnect2(fl)  s_pfCMGetAPI2( "plchandlerconnect2", (RTS_VOID_FCTPTR *)&pfplchandlerconnect2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x229609D6, 0), 0x03051200)
	#define CAL_plchandlerconnect2  pfplchandlerconnect2
	#define CHK_plchandlerconnect2  (pfplchandlerconnect2 != NULL)
	#define EXP_plchandlerconnect2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerconnect2", (RTS_UINTPTR)plchandlerconnect2, 1, RTSITF_GET_SIGNATURE(0x229609D6, 0), 0x03051200) 
#endif


/**
 * Creates a new PLC Handler instance.
 * 
 * .. note::The returned instance must be released by calling |DeleteInstance|
 *
 * :return: A handle to the newly created instance
 */
typedef struct tagplchandlercreateinstance_struct
{
	RTS_IEC_HANDLE PLCHandlerCreateInstance;	/* VAR_OUTPUT */	
	RTS_IEC_INT result;					/* VAR_OUTPUT, Enum: ERROR */
} plchandlercreateinstance_struct;

void CDECL CDECL_EXT plchandlercreateinstance(plchandlercreateinstance_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERCREATEINSTANCE_IEC) (plchandlercreateinstance_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERCREATEINSTANCE_NOTIMPLEMENTED)
	#define USE_plchandlercreateinstance
	#define EXT_plchandlercreateinstance
	#define GET_plchandlercreateinstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlercreateinstance(p0) 
	#define CHK_plchandlercreateinstance  FALSE
	#define EXP_plchandlercreateinstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlercreateinstance
	#define EXT_plchandlercreateinstance
	#define GET_plchandlercreateinstance(fl)  CAL_CMGETAPI( "plchandlercreateinstance" ) 
	#define CAL_plchandlercreateinstance  plchandlercreateinstance
	#define CHK_plchandlercreateinstance  TRUE
	#define EXP_plchandlercreateinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlercreateinstance", (RTS_UINTPTR)plchandlercreateinstance, 1, RTSITF_GET_SIGNATURE(0xF3B6D6E9, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlercreateinstance {(RTS_VOID_FCTPTR)plchandlercreateinstance, "plchandlercreateinstance", RTSITF_GET_SIGNATURE(0xF3B6D6E9, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlercreateinstance
	#define EXT_plchandlercreateinstance
	#define GET_plchandlercreateinstance(fl)  CAL_CMGETAPI( "plchandlercreateinstance" ) 
	#define CAL_plchandlercreateinstance  plchandlercreateinstance
	#define CHK_plchandlercreateinstance  TRUE
	#define EXP_plchandlercreateinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlercreateinstance", (RTS_UINTPTR)plchandlercreateinstance, 1, RTSITF_GET_SIGNATURE(0xF3B6D6E9, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlercreateinstance
	#define EXT_CmpPLCHandlerplchandlercreateinstance
	#define GET_CmpPLCHandlerplchandlercreateinstance  ERR_OK
	#define CAL_CmpPLCHandlerplchandlercreateinstance  plchandlercreateinstance
	#define CHK_CmpPLCHandlerplchandlercreateinstance  TRUE
	#define EXP_CmpPLCHandlerplchandlercreateinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlercreateinstance", (RTS_UINTPTR)plchandlercreateinstance, 1, RTSITF_GET_SIGNATURE(0xF3B6D6E9, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlercreateinstance
	#define EXT_plchandlercreateinstance
	#define GET_plchandlercreateinstance(fl)  CAL_CMGETAPI( "plchandlercreateinstance" ) 
	#define CAL_plchandlercreateinstance  plchandlercreateinstance
	#define CHK_plchandlercreateinstance  TRUE
	#define EXP_plchandlercreateinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlercreateinstance", (RTS_UINTPTR)plchandlercreateinstance, 1, RTSITF_GET_SIGNATURE(0xF3B6D6E9, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlercreateinstance  PFPLCHANDLERCREATEINSTANCE_IEC pfplchandlercreateinstance;
	#define EXT_plchandlercreateinstance  extern PFPLCHANDLERCREATEINSTANCE_IEC pfplchandlercreateinstance;
	#define GET_plchandlercreateinstance(fl)  s_pfCMGetAPI2( "plchandlercreateinstance", (RTS_VOID_FCTPTR *)&pfplchandlercreateinstance, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xF3B6D6E9, 0), 0x03051200)
	#define CAL_plchandlercreateinstance  pfplchandlercreateinstance
	#define CHK_plchandlercreateinstance  (pfplchandlercreateinstance != NULL)
	#define EXP_plchandlercreateinstance   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlercreateinstance", (RTS_UINTPTR)plchandlercreateinstance, 1, RTSITF_GET_SIGNATURE(0xF3B6D6E9, 0), 0x03051200) 
#endif


/**
 * Deletes an instance of the PLCHandler that was previously created by |CreateInstance|
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlerdeleteinstance_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance to delete. */
	RTS_IEC_INT PLCHandlerDeleteInstance;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlerdeleteinstance_struct;

void CDECL CDECL_EXT plchandlerdeleteinstance(plchandlerdeleteinstance_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERDELETEINSTANCE_IEC) (plchandlerdeleteinstance_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERDELETEINSTANCE_NOTIMPLEMENTED)
	#define USE_plchandlerdeleteinstance
	#define EXT_plchandlerdeleteinstance
	#define GET_plchandlerdeleteinstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlerdeleteinstance(p0) 
	#define CHK_plchandlerdeleteinstance  FALSE
	#define EXP_plchandlerdeleteinstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlerdeleteinstance
	#define EXT_plchandlerdeleteinstance
	#define GET_plchandlerdeleteinstance(fl)  CAL_CMGETAPI( "plchandlerdeleteinstance" ) 
	#define CAL_plchandlerdeleteinstance  plchandlerdeleteinstance
	#define CHK_plchandlerdeleteinstance  TRUE
	#define EXP_plchandlerdeleteinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdeleteinstance", (RTS_UINTPTR)plchandlerdeleteinstance, 1, RTSITF_GET_SIGNATURE(0x62C4DDDA, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlerdeleteinstance {(RTS_VOID_FCTPTR)plchandlerdeleteinstance, "plchandlerdeleteinstance", RTSITF_GET_SIGNATURE(0x62C4DDDA, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlerdeleteinstance
	#define EXT_plchandlerdeleteinstance
	#define GET_plchandlerdeleteinstance(fl)  CAL_CMGETAPI( "plchandlerdeleteinstance" ) 
	#define CAL_plchandlerdeleteinstance  plchandlerdeleteinstance
	#define CHK_plchandlerdeleteinstance  TRUE
	#define EXP_plchandlerdeleteinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdeleteinstance", (RTS_UINTPTR)plchandlerdeleteinstance, 1, RTSITF_GET_SIGNATURE(0x62C4DDDA, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlerdeleteinstance
	#define EXT_CmpPLCHandlerplchandlerdeleteinstance
	#define GET_CmpPLCHandlerplchandlerdeleteinstance  ERR_OK
	#define CAL_CmpPLCHandlerplchandlerdeleteinstance  plchandlerdeleteinstance
	#define CHK_CmpPLCHandlerplchandlerdeleteinstance  TRUE
	#define EXP_CmpPLCHandlerplchandlerdeleteinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdeleteinstance", (RTS_UINTPTR)plchandlerdeleteinstance, 1, RTSITF_GET_SIGNATURE(0x62C4DDDA, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlerdeleteinstance
	#define EXT_plchandlerdeleteinstance
	#define GET_plchandlerdeleteinstance(fl)  CAL_CMGETAPI( "plchandlerdeleteinstance" ) 
	#define CAL_plchandlerdeleteinstance  plchandlerdeleteinstance
	#define CHK_plchandlerdeleteinstance  TRUE
	#define EXP_plchandlerdeleteinstance  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdeleteinstance", (RTS_UINTPTR)plchandlerdeleteinstance, 1, RTSITF_GET_SIGNATURE(0x62C4DDDA, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlerdeleteinstance  PFPLCHANDLERDELETEINSTANCE_IEC pfplchandlerdeleteinstance;
	#define EXT_plchandlerdeleteinstance  extern PFPLCHANDLERDELETEINSTANCE_IEC pfplchandlerdeleteinstance;
	#define GET_plchandlerdeleteinstance(fl)  s_pfCMGetAPI2( "plchandlerdeleteinstance", (RTS_VOID_FCTPTR *)&pfplchandlerdeleteinstance, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x62C4DDDA, 0), 0x03051200)
	#define CAL_plchandlerdeleteinstance  pfplchandlerdeleteinstance
	#define CHK_plchandlerdeleteinstance  (pfplchandlerdeleteinstance != NULL)
	#define EXP_plchandlerdeleteinstance   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdeleteinstance", (RTS_UINTPTR)plchandlerdeleteinstance, 1, RTSITF_GET_SIGNATURE(0x62C4DDDA, 0), 0x03051200) 
#endif


/**
 * Disconnects a previously created connection of this instance.
 *
 * .. important:: Calling this method might take some time (eg. for logging out etc.)
 *    so be careful when calling from an IEC-Task. Typically this method should be called in an
 *    asynchronous way. 
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlerdisconnect_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance to disconnect */
	RTS_IEC_INT PLCHandlerDisconnect;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlerdisconnect_struct;

void CDECL CDECL_EXT plchandlerdisconnect(plchandlerdisconnect_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERDISCONNECT_IEC) (plchandlerdisconnect_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERDISCONNECT_NOTIMPLEMENTED)
	#define USE_plchandlerdisconnect
	#define EXT_plchandlerdisconnect
	#define GET_plchandlerdisconnect(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlerdisconnect(p0) 
	#define CHK_plchandlerdisconnect  FALSE
	#define EXP_plchandlerdisconnect  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlerdisconnect
	#define EXT_plchandlerdisconnect
	#define GET_plchandlerdisconnect(fl)  CAL_CMGETAPI( "plchandlerdisconnect" ) 
	#define CAL_plchandlerdisconnect  plchandlerdisconnect
	#define CHK_plchandlerdisconnect  TRUE
	#define EXP_plchandlerdisconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdisconnect", (RTS_UINTPTR)plchandlerdisconnect, 1, RTSITF_GET_SIGNATURE(0xBEC54443, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlerdisconnect {(RTS_VOID_FCTPTR)plchandlerdisconnect, "plchandlerdisconnect", RTSITF_GET_SIGNATURE(0xBEC54443, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlerdisconnect
	#define EXT_plchandlerdisconnect
	#define GET_plchandlerdisconnect(fl)  CAL_CMGETAPI( "plchandlerdisconnect" ) 
	#define CAL_plchandlerdisconnect  plchandlerdisconnect
	#define CHK_plchandlerdisconnect  TRUE
	#define EXP_plchandlerdisconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdisconnect", (RTS_UINTPTR)plchandlerdisconnect, 1, RTSITF_GET_SIGNATURE(0xBEC54443, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlerdisconnect
	#define EXT_CmpPLCHandlerplchandlerdisconnect
	#define GET_CmpPLCHandlerplchandlerdisconnect  ERR_OK
	#define CAL_CmpPLCHandlerplchandlerdisconnect  plchandlerdisconnect
	#define CHK_CmpPLCHandlerplchandlerdisconnect  TRUE
	#define EXP_CmpPLCHandlerplchandlerdisconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdisconnect", (RTS_UINTPTR)plchandlerdisconnect, 1, RTSITF_GET_SIGNATURE(0xBEC54443, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlerdisconnect
	#define EXT_plchandlerdisconnect
	#define GET_plchandlerdisconnect(fl)  CAL_CMGETAPI( "plchandlerdisconnect" ) 
	#define CAL_plchandlerdisconnect  plchandlerdisconnect
	#define CHK_plchandlerdisconnect  TRUE
	#define EXP_plchandlerdisconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdisconnect", (RTS_UINTPTR)plchandlerdisconnect, 1, RTSITF_GET_SIGNATURE(0xBEC54443, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlerdisconnect  PFPLCHANDLERDISCONNECT_IEC pfplchandlerdisconnect;
	#define EXT_plchandlerdisconnect  extern PFPLCHANDLERDISCONNECT_IEC pfplchandlerdisconnect;
	#define GET_plchandlerdisconnect(fl)  s_pfCMGetAPI2( "plchandlerdisconnect", (RTS_VOID_FCTPTR *)&pfplchandlerdisconnect, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xBEC54443, 0), 0x03051200)
	#define CAL_plchandlerdisconnect  pfplchandlerdisconnect
	#define CHK_plchandlerdisconnect  (pfplchandlerdisconnect != NULL)
	#define EXP_plchandlerdisconnect   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerdisconnect", (RTS_UINTPTR)plchandlerdisconnect, 1, RTSITF_GET_SIGNATURE(0xBEC54443, 0), 0x03051200) 
#endif


/**
 * This function can be used to retrieve information of the PLC to which the PLCHandler is connected to. 
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlergetdeviceinfo_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RudimentaryDeviceInfo *pDeviceInfo;	/* VAR_INPUT */	/* Points to a RudimentaryDeviceInfo structure, where the information will be stored */
	RTS_IEC_INT PLCHandlerGetDeviceInfo;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlergetdeviceinfo_struct;

void CDECL CDECL_EXT plchandlergetdeviceinfo(plchandlergetdeviceinfo_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERGETDEVICEINFO_IEC) (plchandlergetdeviceinfo_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERGETDEVICEINFO_NOTIMPLEMENTED)
	#define USE_plchandlergetdeviceinfo
	#define EXT_plchandlergetdeviceinfo
	#define GET_plchandlergetdeviceinfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlergetdeviceinfo(p0) 
	#define CHK_plchandlergetdeviceinfo  FALSE
	#define EXP_plchandlergetdeviceinfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlergetdeviceinfo
	#define EXT_plchandlergetdeviceinfo
	#define GET_plchandlergetdeviceinfo(fl)  CAL_CMGETAPI( "plchandlergetdeviceinfo" ) 
	#define CAL_plchandlergetdeviceinfo  plchandlergetdeviceinfo
	#define CHK_plchandlergetdeviceinfo  TRUE
	#define EXP_plchandlergetdeviceinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetdeviceinfo", (RTS_UINTPTR)plchandlergetdeviceinfo, 1, RTSITF_GET_SIGNATURE(0x9D8B2632, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlergetdeviceinfo {(RTS_VOID_FCTPTR)plchandlergetdeviceinfo, "plchandlergetdeviceinfo", RTSITF_GET_SIGNATURE(0x9D8B2632, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlergetdeviceinfo
	#define EXT_plchandlergetdeviceinfo
	#define GET_plchandlergetdeviceinfo(fl)  CAL_CMGETAPI( "plchandlergetdeviceinfo" ) 
	#define CAL_plchandlergetdeviceinfo  plchandlergetdeviceinfo
	#define CHK_plchandlergetdeviceinfo  TRUE
	#define EXP_plchandlergetdeviceinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetdeviceinfo", (RTS_UINTPTR)plchandlergetdeviceinfo, 1, RTSITF_GET_SIGNATURE(0x9D8B2632, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlergetdeviceinfo
	#define EXT_CmpPLCHandlerplchandlergetdeviceinfo
	#define GET_CmpPLCHandlerplchandlergetdeviceinfo  ERR_OK
	#define CAL_CmpPLCHandlerplchandlergetdeviceinfo  plchandlergetdeviceinfo
	#define CHK_CmpPLCHandlerplchandlergetdeviceinfo  TRUE
	#define EXP_CmpPLCHandlerplchandlergetdeviceinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetdeviceinfo", (RTS_UINTPTR)plchandlergetdeviceinfo, 1, RTSITF_GET_SIGNATURE(0x9D8B2632, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlergetdeviceinfo
	#define EXT_plchandlergetdeviceinfo
	#define GET_plchandlergetdeviceinfo(fl)  CAL_CMGETAPI( "plchandlergetdeviceinfo" ) 
	#define CAL_plchandlergetdeviceinfo  plchandlergetdeviceinfo
	#define CHK_plchandlergetdeviceinfo  TRUE
	#define EXP_plchandlergetdeviceinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetdeviceinfo", (RTS_UINTPTR)plchandlergetdeviceinfo, 1, RTSITF_GET_SIGNATURE(0x9D8B2632, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlergetdeviceinfo  PFPLCHANDLERGETDEVICEINFO_IEC pfplchandlergetdeviceinfo;
	#define EXT_plchandlergetdeviceinfo  extern PFPLCHANDLERGETDEVICEINFO_IEC pfplchandlergetdeviceinfo;
	#define GET_plchandlergetdeviceinfo(fl)  s_pfCMGetAPI2( "plchandlergetdeviceinfo", (RTS_VOID_FCTPTR *)&pfplchandlergetdeviceinfo, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x9D8B2632, 0), 0x03051200)
	#define CAL_plchandlergetdeviceinfo  pfplchandlergetdeviceinfo
	#define CHK_plchandlergetdeviceinfo  (pfplchandlergetdeviceinfo != NULL)
	#define EXP_plchandlergetdeviceinfo   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetdeviceinfo", (RTS_UINTPTR)plchandlergetdeviceinfo, 1, RTSITF_GET_SIGNATURE(0x9D8B2632, 0), 0x03051200) 
#endif


/**
 * This function returns the state of a given PLCHandler instance.
 */
typedef struct tagplchandlergetstate_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance to retrieve the state of. */
	RTS_IEC_INT PLCHandlerGetState;		/* VAR_OUTPUT, Enum: STATE */
	RTS_IEC_INT result;					/* VAR_OUTPUT, Enum: ERROR */
} plchandlergetstate_struct;

void CDECL CDECL_EXT plchandlergetstate(plchandlergetstate_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERGETSTATE_IEC) (plchandlergetstate_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERGETSTATE_NOTIMPLEMENTED)
	#define USE_plchandlergetstate
	#define EXT_plchandlergetstate
	#define GET_plchandlergetstate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlergetstate(p0) 
	#define CHK_plchandlergetstate  FALSE
	#define EXP_plchandlergetstate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlergetstate
	#define EXT_plchandlergetstate
	#define GET_plchandlergetstate(fl)  CAL_CMGETAPI( "plchandlergetstate" ) 
	#define CAL_plchandlergetstate  plchandlergetstate
	#define CHK_plchandlergetstate  TRUE
	#define EXP_plchandlergetstate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetstate", (RTS_UINTPTR)plchandlergetstate, 1, RTSITF_GET_SIGNATURE(0x3882F552, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlergetstate {(RTS_VOID_FCTPTR)plchandlergetstate, "plchandlergetstate", RTSITF_GET_SIGNATURE(0x3882F552, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlergetstate
	#define EXT_plchandlergetstate
	#define GET_plchandlergetstate(fl)  CAL_CMGETAPI( "plchandlergetstate" ) 
	#define CAL_plchandlergetstate  plchandlergetstate
	#define CHK_plchandlergetstate  TRUE
	#define EXP_plchandlergetstate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetstate", (RTS_UINTPTR)plchandlergetstate, 1, RTSITF_GET_SIGNATURE(0x3882F552, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlergetstate
	#define EXT_CmpPLCHandlerplchandlergetstate
	#define GET_CmpPLCHandlerplchandlergetstate  ERR_OK
	#define CAL_CmpPLCHandlerplchandlergetstate  plchandlergetstate
	#define CHK_CmpPLCHandlerplchandlergetstate  TRUE
	#define EXP_CmpPLCHandlerplchandlergetstate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetstate", (RTS_UINTPTR)plchandlergetstate, 1, RTSITF_GET_SIGNATURE(0x3882F552, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlergetstate
	#define EXT_plchandlergetstate
	#define GET_plchandlergetstate(fl)  CAL_CMGETAPI( "plchandlergetstate" ) 
	#define CAL_plchandlergetstate  plchandlergetstate
	#define CHK_plchandlergetstate  TRUE
	#define EXP_plchandlergetstate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetstate", (RTS_UINTPTR)plchandlergetstate, 1, RTSITF_GET_SIGNATURE(0x3882F552, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlergetstate  PFPLCHANDLERGETSTATE_IEC pfplchandlergetstate;
	#define EXT_plchandlergetstate  extern PFPLCHANDLERGETSTATE_IEC pfplchandlergetstate;
	#define GET_plchandlergetstate(fl)  s_pfCMGetAPI2( "plchandlergetstate", (RTS_VOID_FCTPTR *)&pfplchandlergetstate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x3882F552, 0), 0x03051200)
	#define CAL_plchandlergetstate  pfplchandlergetstate
	#define CHK_plchandlergetstate  (pfplchandlergetstate != NULL)
	#define EXP_plchandlergetstate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlergetstate", (RTS_UINTPTR)plchandlergetstate, 1, RTSITF_GET_SIGNATURE(0x3882F552, 0), 0x03051200) 
#endif


/**
 * Configures and activates logging of PLCHandler messages into a logfile.
 * 
 * .. note:: If logging is necessary, then this method should be called immediately after the call to |CreateInstance|.  
 *
 * :return: An errorcode representing the result of this operation. If the call is not done at the correct
 * point of time, then |Error.Busy| is returned.
 */
typedef struct tagplchandlerlogginginit_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance to initialize logging on */
	RTS_IEC_UDINT logFilter;			/* VAR_INPUT */	/* The active filter for log messages. For details of the messages classes have a look at the PLCHandler documentation. */
	RTS_IEC_STRING *logFile;			/* VAR_IN_OUT */	/* The path of the logfile to store the messages in */
	RTS_IEC_INT PLCHandlerLoggingInit;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlerlogginginit_struct;

void CDECL CDECL_EXT plchandlerlogginginit(plchandlerlogginginit_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERLOGGINGINIT_IEC) (plchandlerlogginginit_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERLOGGINGINIT_NOTIMPLEMENTED)
	#define USE_plchandlerlogginginit
	#define EXT_plchandlerlogginginit
	#define GET_plchandlerlogginginit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlerlogginginit(p0) 
	#define CHK_plchandlerlogginginit  FALSE
	#define EXP_plchandlerlogginginit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlerlogginginit
	#define EXT_plchandlerlogginginit
	#define GET_plchandlerlogginginit(fl)  CAL_CMGETAPI( "plchandlerlogginginit" ) 
	#define CAL_plchandlerlogginginit  plchandlerlogginginit
	#define CHK_plchandlerlogginginit  TRUE
	#define EXP_plchandlerlogginginit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerlogginginit", (RTS_UINTPTR)plchandlerlogginginit, 1, RTSITF_GET_SIGNATURE(0xFB1F6D11, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlerlogginginit {(RTS_VOID_FCTPTR)plchandlerlogginginit, "plchandlerlogginginit", RTSITF_GET_SIGNATURE(0xFB1F6D11, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlerlogginginit
	#define EXT_plchandlerlogginginit
	#define GET_plchandlerlogginginit(fl)  CAL_CMGETAPI( "plchandlerlogginginit" ) 
	#define CAL_plchandlerlogginginit  plchandlerlogginginit
	#define CHK_plchandlerlogginginit  TRUE
	#define EXP_plchandlerlogginginit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerlogginginit", (RTS_UINTPTR)plchandlerlogginginit, 1, RTSITF_GET_SIGNATURE(0xFB1F6D11, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlerlogginginit
	#define EXT_CmpPLCHandlerplchandlerlogginginit
	#define GET_CmpPLCHandlerplchandlerlogginginit  ERR_OK
	#define CAL_CmpPLCHandlerplchandlerlogginginit  plchandlerlogginginit
	#define CHK_CmpPLCHandlerplchandlerlogginginit  TRUE
	#define EXP_CmpPLCHandlerplchandlerlogginginit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerlogginginit", (RTS_UINTPTR)plchandlerlogginginit, 1, RTSITF_GET_SIGNATURE(0xFB1F6D11, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlerlogginginit
	#define EXT_plchandlerlogginginit
	#define GET_plchandlerlogginginit(fl)  CAL_CMGETAPI( "plchandlerlogginginit" ) 
	#define CAL_plchandlerlogginginit  plchandlerlogginginit
	#define CHK_plchandlerlogginginit  TRUE
	#define EXP_plchandlerlogginginit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerlogginginit", (RTS_UINTPTR)plchandlerlogginginit, 1, RTSITF_GET_SIGNATURE(0xFB1F6D11, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlerlogginginit  PFPLCHANDLERLOGGINGINIT_IEC pfplchandlerlogginginit;
	#define EXT_plchandlerlogginginit  extern PFPLCHANDLERLOGGINGINIT_IEC pfplchandlerlogginginit;
	#define GET_plchandlerlogginginit(fl)  s_pfCMGetAPI2( "plchandlerlogginginit", (RTS_VOID_FCTPTR *)&pfplchandlerlogginginit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xFB1F6D11, 0), 0x03051200)
	#define CAL_plchandlerlogginginit  pfplchandlerlogginginit
	#define CHK_plchandlerlogginginit  (pfplchandlerlogginginit != NULL)
	#define EXP_plchandlerlogginginit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlerlogginginit", (RTS_UINTPTR)plchandlerlogginginit, 1, RTSITF_GET_SIGNATURE(0xFB1F6D11, 0), 0x03051200) 
#endif


/**
 * Creates a variable list to be used lateron with |SyncReadVarListFromPlc| or |SyncWriteVarListToPlc|. The resulting variable list must be released by a followup call to 
 * |SyncDeleteVarList|.
 *
 * .. important:: As communication and symbolic resolution is involved, take care (async calls etc.) when using this method from an IEC-Task
 *
 * :return: A handle to the newly created variable list
 */
typedef struct tagplchandlersyncdefinevarlist_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_BYTE **ppInstPaths;			/* VAR_INPUT */	/* A POINTER to an ARRAY of POINTERs to zero terminated ASCII STRINGs representing the instance paths that should be written */
	RTS_IEC_UDINT numSymbols;			/* VAR_INPUT */	/* The number of variables to read. In fact this number represents the number of entries in ``ppInstPaths`` */
	RTS_IEC_DWORD flags;				/* VAR_INPUT, Enum: VARLISTFLAGS */
	RTS_IEC_HANDLE PLCHandlerSyncDefineVarList;	/* VAR_OUTPUT */	
	RTS_IEC_INT result;					/* VAR_OUTPUT, Enum: ERROR */
} plchandlersyncdefinevarlist_struct;

void CDECL CDECL_EXT plchandlersyncdefinevarlist(plchandlersyncdefinevarlist_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERSYNCDEFINEVARLIST_IEC) (plchandlersyncdefinevarlist_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERSYNCDEFINEVARLIST_NOTIMPLEMENTED)
	#define USE_plchandlersyncdefinevarlist
	#define EXT_plchandlersyncdefinevarlist
	#define GET_plchandlersyncdefinevarlist(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlersyncdefinevarlist(p0) 
	#define CHK_plchandlersyncdefinevarlist  FALSE
	#define EXP_plchandlersyncdefinevarlist  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlersyncdefinevarlist
	#define EXT_plchandlersyncdefinevarlist
	#define GET_plchandlersyncdefinevarlist(fl)  CAL_CMGETAPI( "plchandlersyncdefinevarlist" ) 
	#define CAL_plchandlersyncdefinevarlist  plchandlersyncdefinevarlist
	#define CHK_plchandlersyncdefinevarlist  TRUE
	#define EXP_plchandlersyncdefinevarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdefinevarlist", (RTS_UINTPTR)plchandlersyncdefinevarlist, 1, RTSITF_GET_SIGNATURE(0x9C816C52, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlersyncdefinevarlist {(RTS_VOID_FCTPTR)plchandlersyncdefinevarlist, "plchandlersyncdefinevarlist", RTSITF_GET_SIGNATURE(0x9C816C52, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlersyncdefinevarlist
	#define EXT_plchandlersyncdefinevarlist
	#define GET_plchandlersyncdefinevarlist(fl)  CAL_CMGETAPI( "plchandlersyncdefinevarlist" ) 
	#define CAL_plchandlersyncdefinevarlist  plchandlersyncdefinevarlist
	#define CHK_plchandlersyncdefinevarlist  TRUE
	#define EXP_plchandlersyncdefinevarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdefinevarlist", (RTS_UINTPTR)plchandlersyncdefinevarlist, 1, RTSITF_GET_SIGNATURE(0x9C816C52, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlersyncdefinevarlist
	#define EXT_CmpPLCHandlerplchandlersyncdefinevarlist
	#define GET_CmpPLCHandlerplchandlersyncdefinevarlist  ERR_OK
	#define CAL_CmpPLCHandlerplchandlersyncdefinevarlist  plchandlersyncdefinevarlist
	#define CHK_CmpPLCHandlerplchandlersyncdefinevarlist  TRUE
	#define EXP_CmpPLCHandlerplchandlersyncdefinevarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdefinevarlist", (RTS_UINTPTR)plchandlersyncdefinevarlist, 1, RTSITF_GET_SIGNATURE(0x9C816C52, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlersyncdefinevarlist
	#define EXT_plchandlersyncdefinevarlist
	#define GET_plchandlersyncdefinevarlist(fl)  CAL_CMGETAPI( "plchandlersyncdefinevarlist" ) 
	#define CAL_plchandlersyncdefinevarlist  plchandlersyncdefinevarlist
	#define CHK_plchandlersyncdefinevarlist  TRUE
	#define EXP_plchandlersyncdefinevarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdefinevarlist", (RTS_UINTPTR)plchandlersyncdefinevarlist, 1, RTSITF_GET_SIGNATURE(0x9C816C52, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlersyncdefinevarlist  PFPLCHANDLERSYNCDEFINEVARLIST_IEC pfplchandlersyncdefinevarlist;
	#define EXT_plchandlersyncdefinevarlist  extern PFPLCHANDLERSYNCDEFINEVARLIST_IEC pfplchandlersyncdefinevarlist;
	#define GET_plchandlersyncdefinevarlist(fl)  s_pfCMGetAPI2( "plchandlersyncdefinevarlist", (RTS_VOID_FCTPTR *)&pfplchandlersyncdefinevarlist, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x9C816C52, 0), 0x03051200)
	#define CAL_plchandlersyncdefinevarlist  pfplchandlersyncdefinevarlist
	#define CHK_plchandlersyncdefinevarlist  (pfplchandlersyncdefinevarlist != NULL)
	#define EXP_plchandlersyncdefinevarlist   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdefinevarlist", (RTS_UINTPTR)plchandlersyncdefinevarlist, 1, RTSITF_GET_SIGNATURE(0x9C816C52, 0), 0x03051200) 
#endif


/**
 * Deletes the given variable list that was previously created by |SyncDefineVarList|
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlersyncdeletevarlist_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The variable list to delete. */
	RTS_IEC_INT PLCHandlerSyncDeleteVarList;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlersyncdeletevarlist_struct;

void CDECL CDECL_EXT plchandlersyncdeletevarlist(plchandlersyncdeletevarlist_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERSYNCDELETEVARLIST_IEC) (plchandlersyncdeletevarlist_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERSYNCDELETEVARLIST_NOTIMPLEMENTED)
	#define USE_plchandlersyncdeletevarlist
	#define EXT_plchandlersyncdeletevarlist
	#define GET_plchandlersyncdeletevarlist(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlersyncdeletevarlist(p0) 
	#define CHK_plchandlersyncdeletevarlist  FALSE
	#define EXP_plchandlersyncdeletevarlist  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlersyncdeletevarlist
	#define EXT_plchandlersyncdeletevarlist
	#define GET_plchandlersyncdeletevarlist(fl)  CAL_CMGETAPI( "plchandlersyncdeletevarlist" ) 
	#define CAL_plchandlersyncdeletevarlist  plchandlersyncdeletevarlist
	#define CHK_plchandlersyncdeletevarlist  TRUE
	#define EXP_plchandlersyncdeletevarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdeletevarlist", (RTS_UINTPTR)plchandlersyncdeletevarlist, 1, RTSITF_GET_SIGNATURE(0xA82CAF19, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlersyncdeletevarlist {(RTS_VOID_FCTPTR)plchandlersyncdeletevarlist, "plchandlersyncdeletevarlist", RTSITF_GET_SIGNATURE(0xA82CAF19, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlersyncdeletevarlist
	#define EXT_plchandlersyncdeletevarlist
	#define GET_plchandlersyncdeletevarlist(fl)  CAL_CMGETAPI( "plchandlersyncdeletevarlist" ) 
	#define CAL_plchandlersyncdeletevarlist  plchandlersyncdeletevarlist
	#define CHK_plchandlersyncdeletevarlist  TRUE
	#define EXP_plchandlersyncdeletevarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdeletevarlist", (RTS_UINTPTR)plchandlersyncdeletevarlist, 1, RTSITF_GET_SIGNATURE(0xA82CAF19, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlersyncdeletevarlist
	#define EXT_CmpPLCHandlerplchandlersyncdeletevarlist
	#define GET_CmpPLCHandlerplchandlersyncdeletevarlist  ERR_OK
	#define CAL_CmpPLCHandlerplchandlersyncdeletevarlist  plchandlersyncdeletevarlist
	#define CHK_CmpPLCHandlerplchandlersyncdeletevarlist  TRUE
	#define EXP_CmpPLCHandlerplchandlersyncdeletevarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdeletevarlist", (RTS_UINTPTR)plchandlersyncdeletevarlist, 1, RTSITF_GET_SIGNATURE(0xA82CAF19, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlersyncdeletevarlist
	#define EXT_plchandlersyncdeletevarlist
	#define GET_plchandlersyncdeletevarlist(fl)  CAL_CMGETAPI( "plchandlersyncdeletevarlist" ) 
	#define CAL_plchandlersyncdeletevarlist  plchandlersyncdeletevarlist
	#define CHK_plchandlersyncdeletevarlist  TRUE
	#define EXP_plchandlersyncdeletevarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdeletevarlist", (RTS_UINTPTR)plchandlersyncdeletevarlist, 1, RTSITF_GET_SIGNATURE(0xA82CAF19, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlersyncdeletevarlist  PFPLCHANDLERSYNCDELETEVARLIST_IEC pfplchandlersyncdeletevarlist;
	#define EXT_plchandlersyncdeletevarlist  extern PFPLCHANDLERSYNCDELETEVARLIST_IEC pfplchandlersyncdeletevarlist;
	#define GET_plchandlersyncdeletevarlist(fl)  s_pfCMGetAPI2( "plchandlersyncdeletevarlist", (RTS_VOID_FCTPTR *)&pfplchandlersyncdeletevarlist, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xA82CAF19, 0), 0x03051200)
	#define CAL_plchandlersyncdeletevarlist  pfplchandlersyncdeletevarlist
	#define CHK_plchandlersyncdeletevarlist  (pfplchandlersyncdeletevarlist != NULL)
	#define EXP_plchandlersyncdeletevarlist   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncdeletevarlist", (RTS_UINTPTR)plchandlersyncdeletevarlist, 1, RTSITF_GET_SIGNATURE(0xA82CAF19, 0), 0x03051200) 
#endif


/**
 * Reads the given variable list synchronously. The list must have been created previously by a call to |SyncReadVars|
 *
 * .. important:: As communication is involved, take care (async calls etc.) when using this method from an IEC-Task
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlersyncreadvarlist_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The variable list to read. */
	RTS_IEC_INT PLCHandlerSyncReadVarList;	/* VAR_OUTPUT, Enum: ERROR */
	VariableValue **ppVarInfos;			/* VAR_OUTPUT */	/* An ARRAY OF POINTERs to VarInfo instances representing each single read value */
	RTS_IEC_UDINT numVarsResult;		/* VAR_OUTPUT */	/* The size of the array of returned variables */
} plchandlersyncreadvarlist_struct;

void CDECL CDECL_EXT plchandlersyncreadvarlist(plchandlersyncreadvarlist_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERSYNCREADVARLIST_IEC) (plchandlersyncreadvarlist_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERSYNCREADVARLIST_NOTIMPLEMENTED)
	#define USE_plchandlersyncreadvarlist
	#define EXT_plchandlersyncreadvarlist
	#define GET_plchandlersyncreadvarlist(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlersyncreadvarlist(p0) 
	#define CHK_plchandlersyncreadvarlist  FALSE
	#define EXP_plchandlersyncreadvarlist  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlersyncreadvarlist
	#define EXT_plchandlersyncreadvarlist
	#define GET_plchandlersyncreadvarlist(fl)  CAL_CMGETAPI( "plchandlersyncreadvarlist" ) 
	#define CAL_plchandlersyncreadvarlist  plchandlersyncreadvarlist
	#define CHK_plchandlersyncreadvarlist  TRUE
	#define EXP_plchandlersyncreadvarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlist", (RTS_UINTPTR)plchandlersyncreadvarlist, 1, RTSITF_GET_SIGNATURE(0x22BA041E, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlersyncreadvarlist {(RTS_VOID_FCTPTR)plchandlersyncreadvarlist, "plchandlersyncreadvarlist", RTSITF_GET_SIGNATURE(0x22BA041E, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlersyncreadvarlist
	#define EXT_plchandlersyncreadvarlist
	#define GET_plchandlersyncreadvarlist(fl)  CAL_CMGETAPI( "plchandlersyncreadvarlist" ) 
	#define CAL_plchandlersyncreadvarlist  plchandlersyncreadvarlist
	#define CHK_plchandlersyncreadvarlist  TRUE
	#define EXP_plchandlersyncreadvarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlist", (RTS_UINTPTR)plchandlersyncreadvarlist, 1, RTSITF_GET_SIGNATURE(0x22BA041E, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlersyncreadvarlist
	#define EXT_CmpPLCHandlerplchandlersyncreadvarlist
	#define GET_CmpPLCHandlerplchandlersyncreadvarlist  ERR_OK
	#define CAL_CmpPLCHandlerplchandlersyncreadvarlist  plchandlersyncreadvarlist
	#define CHK_CmpPLCHandlerplchandlersyncreadvarlist  TRUE
	#define EXP_CmpPLCHandlerplchandlersyncreadvarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlist", (RTS_UINTPTR)plchandlersyncreadvarlist, 1, RTSITF_GET_SIGNATURE(0x22BA041E, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlersyncreadvarlist
	#define EXT_plchandlersyncreadvarlist
	#define GET_plchandlersyncreadvarlist(fl)  CAL_CMGETAPI( "plchandlersyncreadvarlist" ) 
	#define CAL_plchandlersyncreadvarlist  plchandlersyncreadvarlist
	#define CHK_plchandlersyncreadvarlist  TRUE
	#define EXP_plchandlersyncreadvarlist  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlist", (RTS_UINTPTR)plchandlersyncreadvarlist, 1, RTSITF_GET_SIGNATURE(0x22BA041E, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlersyncreadvarlist  PFPLCHANDLERSYNCREADVARLIST_IEC pfplchandlersyncreadvarlist;
	#define EXT_plchandlersyncreadvarlist  extern PFPLCHANDLERSYNCREADVARLIST_IEC pfplchandlersyncreadvarlist;
	#define GET_plchandlersyncreadvarlist(fl)  s_pfCMGetAPI2( "plchandlersyncreadvarlist", (RTS_VOID_FCTPTR *)&pfplchandlersyncreadvarlist, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x22BA041E, 0), 0x03051200)
	#define CAL_plchandlersyncreadvarlist  pfplchandlersyncreadvarlist
	#define CHK_plchandlersyncreadvarlist  (pfplchandlersyncreadvarlist != NULL)
	#define EXP_plchandlersyncreadvarlist   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlist", (RTS_UINTPTR)plchandlersyncreadvarlist, 1, RTSITF_GET_SIGNATURE(0x22BA041E, 0), 0x03051200) 
#endif


/**
 * Reads the given variable list synchronously. The list must have been created previously by a call to |SyncDefineVarList|
 *
 * .. important:: As communication is involved, take care (async calls etc.) when using this method from an IEC-Task
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlersyncreadvarlistfromplc_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The variable list to read. */
	RTS_IEC_INT PLCHandlerSyncReadVarListFromPlc;	/* VAR_OUTPUT, Enum: ERROR */
	VariableValue **ppVarInfos;			/* VAR_OUTPUT */	/* An ARRAY OF POINTERs to VarInfo instances representing each single read value */
	RTS_IEC_UDINT numVarsResult;		/* VAR_OUTPUT */	/* The size of the array of returned variables */
} plchandlersyncreadvarlistfromplc_struct;

void CDECL CDECL_EXT plchandlersyncreadvarlistfromplc(plchandlersyncreadvarlistfromplc_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERSYNCREADVARLISTFROMPLC_IEC) (plchandlersyncreadvarlistfromplc_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERSYNCREADVARLISTFROMPLC_NOTIMPLEMENTED)
	#define USE_plchandlersyncreadvarlistfromplc
	#define EXT_plchandlersyncreadvarlistfromplc
	#define GET_plchandlersyncreadvarlistfromplc(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlersyncreadvarlistfromplc(p0) 
	#define CHK_plchandlersyncreadvarlistfromplc  FALSE
	#define EXP_plchandlersyncreadvarlistfromplc  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlersyncreadvarlistfromplc
	#define EXT_plchandlersyncreadvarlistfromplc
	#define GET_plchandlersyncreadvarlistfromplc(fl)  CAL_CMGETAPI( "plchandlersyncreadvarlistfromplc" ) 
	#define CAL_plchandlersyncreadvarlistfromplc  plchandlersyncreadvarlistfromplc
	#define CHK_plchandlersyncreadvarlistfromplc  TRUE
	#define EXP_plchandlersyncreadvarlistfromplc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlistfromplc", (RTS_UINTPTR)plchandlersyncreadvarlistfromplc, 1, RTSITF_GET_SIGNATURE(0xD0C1A01F, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlersyncreadvarlistfromplc {(RTS_VOID_FCTPTR)plchandlersyncreadvarlistfromplc, "plchandlersyncreadvarlistfromplc", RTSITF_GET_SIGNATURE(0xD0C1A01F, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlersyncreadvarlistfromplc
	#define EXT_plchandlersyncreadvarlistfromplc
	#define GET_plchandlersyncreadvarlistfromplc(fl)  CAL_CMGETAPI( "plchandlersyncreadvarlistfromplc" ) 
	#define CAL_plchandlersyncreadvarlistfromplc  plchandlersyncreadvarlistfromplc
	#define CHK_plchandlersyncreadvarlistfromplc  TRUE
	#define EXP_plchandlersyncreadvarlistfromplc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlistfromplc", (RTS_UINTPTR)plchandlersyncreadvarlistfromplc, 1, RTSITF_GET_SIGNATURE(0xD0C1A01F, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlersyncreadvarlistfromplc
	#define EXT_CmpPLCHandlerplchandlersyncreadvarlistfromplc
	#define GET_CmpPLCHandlerplchandlersyncreadvarlistfromplc  ERR_OK
	#define CAL_CmpPLCHandlerplchandlersyncreadvarlistfromplc  plchandlersyncreadvarlistfromplc
	#define CHK_CmpPLCHandlerplchandlersyncreadvarlistfromplc  TRUE
	#define EXP_CmpPLCHandlerplchandlersyncreadvarlistfromplc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlistfromplc", (RTS_UINTPTR)plchandlersyncreadvarlistfromplc, 1, RTSITF_GET_SIGNATURE(0xD0C1A01F, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlersyncreadvarlistfromplc
	#define EXT_plchandlersyncreadvarlistfromplc
	#define GET_plchandlersyncreadvarlistfromplc(fl)  CAL_CMGETAPI( "plchandlersyncreadvarlistfromplc" ) 
	#define CAL_plchandlersyncreadvarlistfromplc  plchandlersyncreadvarlistfromplc
	#define CHK_plchandlersyncreadvarlistfromplc  TRUE
	#define EXP_plchandlersyncreadvarlistfromplc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlistfromplc", (RTS_UINTPTR)plchandlersyncreadvarlistfromplc, 1, RTSITF_GET_SIGNATURE(0xD0C1A01F, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlersyncreadvarlistfromplc  PFPLCHANDLERSYNCREADVARLISTFROMPLC_IEC pfplchandlersyncreadvarlistfromplc;
	#define EXT_plchandlersyncreadvarlistfromplc  extern PFPLCHANDLERSYNCREADVARLISTFROMPLC_IEC pfplchandlersyncreadvarlistfromplc;
	#define GET_plchandlersyncreadvarlistfromplc(fl)  s_pfCMGetAPI2( "plchandlersyncreadvarlistfromplc", (RTS_VOID_FCTPTR *)&pfplchandlersyncreadvarlistfromplc, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xD0C1A01F, 0), 0x03051200)
	#define CAL_plchandlersyncreadvarlistfromplc  pfplchandlersyncreadvarlistfromplc
	#define CHK_plchandlersyncreadvarlistfromplc  (pfplchandlersyncreadvarlistfromplc != NULL)
	#define EXP_plchandlersyncreadvarlistfromplc   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarlistfromplc", (RTS_UINTPTR)plchandlersyncreadvarlistfromplc, 1, RTSITF_GET_SIGNATURE(0xD0C1A01F, 0), 0x03051200) 
#endif


/**
 * Reads the given variables synchronously. The resulting variable list must be released by a followup call to 
 * |SyncReadVarsRelease|.
 *
 * .. note:: The returned variable list may be reused for further read calls (without additional symbolic resolution) by
 *    calling |SyncReadVarList|
 *
 * .. important:: As communication and symbolic resolution is involved, take care (async calls etc.) when using this method from an IEC-Task
 *
 * :return: A handle to the newly created variable list
 */
typedef struct tagplchandlersyncreadvars_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_BYTE **ppInstPaths;			/* VAR_INPUT */	/* A POINTER to an ARRAY of POINTERs to zero terminated ASCII STRINGs representing the instance paths that should be written */
	RTS_IEC_UDINT numSymbols;			/* VAR_INPUT */	/* The number of variables to read. In fact this number represents the number of entries in ``ppInstPaths`` */
	RTS_IEC_HANDLE PLCHandlerSyncReadVars;	/* VAR_OUTPUT */	
	VariableValue **ppVarInfos;			/* VAR_OUTPUT */	/* An ARRAY OF POINTERs to VarInfo instances representing each single read value */
	RTS_IEC_UDINT numVarsResult;		/* VAR_OUTPUT */	/* The size of the array of returned variables */
	RTS_IEC_INT result;					/* VAR_OUTPUT, Enum: ERROR */
} plchandlersyncreadvars_struct;

void CDECL CDECL_EXT plchandlersyncreadvars(plchandlersyncreadvars_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERSYNCREADVARS_IEC) (plchandlersyncreadvars_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERSYNCREADVARS_NOTIMPLEMENTED)
	#define USE_plchandlersyncreadvars
	#define EXT_plchandlersyncreadvars
	#define GET_plchandlersyncreadvars(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlersyncreadvars(p0) 
	#define CHK_plchandlersyncreadvars  FALSE
	#define EXP_plchandlersyncreadvars  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlersyncreadvars
	#define EXT_plchandlersyncreadvars
	#define GET_plchandlersyncreadvars(fl)  CAL_CMGETAPI( "plchandlersyncreadvars" ) 
	#define CAL_plchandlersyncreadvars  plchandlersyncreadvars
	#define CHK_plchandlersyncreadvars  TRUE
	#define EXP_plchandlersyncreadvars  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvars", (RTS_UINTPTR)plchandlersyncreadvars, 1, RTSITF_GET_SIGNATURE(0x7F4F3550, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlersyncreadvars {(RTS_VOID_FCTPTR)plchandlersyncreadvars, "plchandlersyncreadvars", RTSITF_GET_SIGNATURE(0x7F4F3550, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlersyncreadvars
	#define EXT_plchandlersyncreadvars
	#define GET_plchandlersyncreadvars(fl)  CAL_CMGETAPI( "plchandlersyncreadvars" ) 
	#define CAL_plchandlersyncreadvars  plchandlersyncreadvars
	#define CHK_plchandlersyncreadvars  TRUE
	#define EXP_plchandlersyncreadvars  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvars", (RTS_UINTPTR)plchandlersyncreadvars, 1, RTSITF_GET_SIGNATURE(0x7F4F3550, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlersyncreadvars
	#define EXT_CmpPLCHandlerplchandlersyncreadvars
	#define GET_CmpPLCHandlerplchandlersyncreadvars  ERR_OK
	#define CAL_CmpPLCHandlerplchandlersyncreadvars  plchandlersyncreadvars
	#define CHK_CmpPLCHandlerplchandlersyncreadvars  TRUE
	#define EXP_CmpPLCHandlerplchandlersyncreadvars  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvars", (RTS_UINTPTR)plchandlersyncreadvars, 1, RTSITF_GET_SIGNATURE(0x7F4F3550, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlersyncreadvars
	#define EXT_plchandlersyncreadvars
	#define GET_plchandlersyncreadvars(fl)  CAL_CMGETAPI( "plchandlersyncreadvars" ) 
	#define CAL_plchandlersyncreadvars  plchandlersyncreadvars
	#define CHK_plchandlersyncreadvars  TRUE
	#define EXP_plchandlersyncreadvars  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvars", (RTS_UINTPTR)plchandlersyncreadvars, 1, RTSITF_GET_SIGNATURE(0x7F4F3550, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlersyncreadvars  PFPLCHANDLERSYNCREADVARS_IEC pfplchandlersyncreadvars;
	#define EXT_plchandlersyncreadvars  extern PFPLCHANDLERSYNCREADVARS_IEC pfplchandlersyncreadvars;
	#define GET_plchandlersyncreadvars(fl)  s_pfCMGetAPI2( "plchandlersyncreadvars", (RTS_VOID_FCTPTR *)&pfplchandlersyncreadvars, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x7F4F3550, 0), 0x03051200)
	#define CAL_plchandlersyncreadvars  pfplchandlersyncreadvars
	#define CHK_plchandlersyncreadvars  (pfplchandlersyncreadvars != NULL)
	#define EXP_plchandlersyncreadvars   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvars", (RTS_UINTPTR)plchandlersyncreadvars, 1, RTSITF_GET_SIGNATURE(0x7F4F3550, 0), 0x03051200) 
#endif


/**
 * Releases the given variable list that was previously created by |SyncReadVars|
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlersyncreadvarsrelease_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The variable list to release. */
	RTS_IEC_INT PLCHandlerSyncReadVarsRelease;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlersyncreadvarsrelease_struct;

void CDECL CDECL_EXT plchandlersyncreadvarsrelease(plchandlersyncreadvarsrelease_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERSYNCREADVARSRELEASE_IEC) (plchandlersyncreadvarsrelease_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERSYNCREADVARSRELEASE_NOTIMPLEMENTED)
	#define USE_plchandlersyncreadvarsrelease
	#define EXT_plchandlersyncreadvarsrelease
	#define GET_plchandlersyncreadvarsrelease(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlersyncreadvarsrelease(p0) 
	#define CHK_plchandlersyncreadvarsrelease  FALSE
	#define EXP_plchandlersyncreadvarsrelease  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlersyncreadvarsrelease
	#define EXT_plchandlersyncreadvarsrelease
	#define GET_plchandlersyncreadvarsrelease(fl)  CAL_CMGETAPI( "plchandlersyncreadvarsrelease" ) 
	#define CAL_plchandlersyncreadvarsrelease  plchandlersyncreadvarsrelease
	#define CHK_plchandlersyncreadvarsrelease  TRUE
	#define EXP_plchandlersyncreadvarsrelease  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarsrelease", (RTS_UINTPTR)plchandlersyncreadvarsrelease, 1, RTSITF_GET_SIGNATURE(0xD4834060, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlersyncreadvarsrelease {(RTS_VOID_FCTPTR)plchandlersyncreadvarsrelease, "plchandlersyncreadvarsrelease", RTSITF_GET_SIGNATURE(0xD4834060, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlersyncreadvarsrelease
	#define EXT_plchandlersyncreadvarsrelease
	#define GET_plchandlersyncreadvarsrelease(fl)  CAL_CMGETAPI( "plchandlersyncreadvarsrelease" ) 
	#define CAL_plchandlersyncreadvarsrelease  plchandlersyncreadvarsrelease
	#define CHK_plchandlersyncreadvarsrelease  TRUE
	#define EXP_plchandlersyncreadvarsrelease  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarsrelease", (RTS_UINTPTR)plchandlersyncreadvarsrelease, 1, RTSITF_GET_SIGNATURE(0xD4834060, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlersyncreadvarsrelease
	#define EXT_CmpPLCHandlerplchandlersyncreadvarsrelease
	#define GET_CmpPLCHandlerplchandlersyncreadvarsrelease  ERR_OK
	#define CAL_CmpPLCHandlerplchandlersyncreadvarsrelease  plchandlersyncreadvarsrelease
	#define CHK_CmpPLCHandlerplchandlersyncreadvarsrelease  TRUE
	#define EXP_CmpPLCHandlerplchandlersyncreadvarsrelease  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarsrelease", (RTS_UINTPTR)plchandlersyncreadvarsrelease, 1, RTSITF_GET_SIGNATURE(0xD4834060, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlersyncreadvarsrelease
	#define EXT_plchandlersyncreadvarsrelease
	#define GET_plchandlersyncreadvarsrelease(fl)  CAL_CMGETAPI( "plchandlersyncreadvarsrelease" ) 
	#define CAL_plchandlersyncreadvarsrelease  plchandlersyncreadvarsrelease
	#define CHK_plchandlersyncreadvarsrelease  TRUE
	#define EXP_plchandlersyncreadvarsrelease  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarsrelease", (RTS_UINTPTR)plchandlersyncreadvarsrelease, 1, RTSITF_GET_SIGNATURE(0xD4834060, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlersyncreadvarsrelease  PFPLCHANDLERSYNCREADVARSRELEASE_IEC pfplchandlersyncreadvarsrelease;
	#define EXT_plchandlersyncreadvarsrelease  extern PFPLCHANDLERSYNCREADVARSRELEASE_IEC pfplchandlersyncreadvarsrelease;
	#define GET_plchandlersyncreadvarsrelease(fl)  s_pfCMGetAPI2( "plchandlersyncreadvarsrelease", (RTS_VOID_FCTPTR *)&pfplchandlersyncreadvarsrelease, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xD4834060, 0), 0x03051200)
	#define CAL_plchandlersyncreadvarsrelease  pfplchandlersyncreadvarsrelease
	#define CHK_plchandlersyncreadvarsrelease  (pfplchandlersyncreadvarsrelease != NULL)
	#define EXP_plchandlersyncreadvarsrelease   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncreadvarsrelease", (RTS_UINTPTR)plchandlersyncreadvarsrelease, 1, RTSITF_GET_SIGNATURE(0xD4834060, 0), 0x03051200) 
#endif


/**
 * This function can be used to transfer any runtime system service to the PLC.
 *
 * .. important:: Before this function can be used it is necessary to call |GetDeviceInfo| to retrieve information about the PLC. 
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlersyncsendservice_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_BYTE *pbySend;				/* VAR_INPUT */	/* Pointer to the byte stream, which should be sent to the PLC */
	RTS_IEC_UDINT udiSendSize;			/* VAR_INPUT */	/* Length of the byte stream to send */
	RTS_IEC_BYTE **ppbyRecv;			/* VAR_INPUT */	/* Points to the buffer, where the result of the call is available */
	RTS_IEC_UDINT *pudiRecvSize;		/* VAR_INPUT */	/* Points to an UDINT variable, that contains the length of the PLC’s service reply */
	RTS_IEC_INT PLCHandlerSyncSendService;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlersyncsendservice_struct;

void CDECL CDECL_EXT plchandlersyncsendservice(plchandlersyncsendservice_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERSYNCSENDSERVICE_IEC) (plchandlersyncsendservice_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERSYNCSENDSERVICE_NOTIMPLEMENTED)
	#define USE_plchandlersyncsendservice
	#define EXT_plchandlersyncsendservice
	#define GET_plchandlersyncsendservice(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlersyncsendservice(p0) 
	#define CHK_plchandlersyncsendservice  FALSE
	#define EXP_plchandlersyncsendservice  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlersyncsendservice
	#define EXT_plchandlersyncsendservice
	#define GET_plchandlersyncsendservice(fl)  CAL_CMGETAPI( "plchandlersyncsendservice" ) 
	#define CAL_plchandlersyncsendservice  plchandlersyncsendservice
	#define CHK_plchandlersyncsendservice  TRUE
	#define EXP_plchandlersyncsendservice  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncsendservice", (RTS_UINTPTR)plchandlersyncsendservice, 1, RTSITF_GET_SIGNATURE(0x303EF9A4, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlersyncsendservice {(RTS_VOID_FCTPTR)plchandlersyncsendservice, "plchandlersyncsendservice", RTSITF_GET_SIGNATURE(0x303EF9A4, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlersyncsendservice
	#define EXT_plchandlersyncsendservice
	#define GET_plchandlersyncsendservice(fl)  CAL_CMGETAPI( "plchandlersyncsendservice" ) 
	#define CAL_plchandlersyncsendservice  plchandlersyncsendservice
	#define CHK_plchandlersyncsendservice  TRUE
	#define EXP_plchandlersyncsendservice  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncsendservice", (RTS_UINTPTR)plchandlersyncsendservice, 1, RTSITF_GET_SIGNATURE(0x303EF9A4, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlersyncsendservice
	#define EXT_CmpPLCHandlerplchandlersyncsendservice
	#define GET_CmpPLCHandlerplchandlersyncsendservice  ERR_OK
	#define CAL_CmpPLCHandlerplchandlersyncsendservice  plchandlersyncsendservice
	#define CHK_CmpPLCHandlerplchandlersyncsendservice  TRUE
	#define EXP_CmpPLCHandlerplchandlersyncsendservice  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncsendservice", (RTS_UINTPTR)plchandlersyncsendservice, 1, RTSITF_GET_SIGNATURE(0x303EF9A4, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlersyncsendservice
	#define EXT_plchandlersyncsendservice
	#define GET_plchandlersyncsendservice(fl)  CAL_CMGETAPI( "plchandlersyncsendservice" ) 
	#define CAL_plchandlersyncsendservice  plchandlersyncsendservice
	#define CHK_plchandlersyncsendservice  TRUE
	#define EXP_plchandlersyncsendservice  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncsendservice", (RTS_UINTPTR)plchandlersyncsendservice, 1, RTSITF_GET_SIGNATURE(0x303EF9A4, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlersyncsendservice  PFPLCHANDLERSYNCSENDSERVICE_IEC pfplchandlersyncsendservice;
	#define EXT_plchandlersyncsendservice  extern PFPLCHANDLERSYNCSENDSERVICE_IEC pfplchandlersyncsendservice;
	#define GET_plchandlersyncsendservice(fl)  s_pfCMGetAPI2( "plchandlersyncsendservice", (RTS_VOID_FCTPTR *)&pfplchandlersyncsendservice, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x303EF9A4, 0), 0x03051200)
	#define CAL_plchandlersyncsendservice  pfplchandlersyncsendservice
	#define CHK_plchandlersyncsendservice  (pfplchandlersyncsendservice != NULL)
	#define EXP_plchandlersyncsendservice   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncsendservice", (RTS_UINTPTR)plchandlersyncsendservice, 1, RTSITF_GET_SIGNATURE(0x303EF9A4, 0), 0x03051200) 
#endif


/**
 * Writes the given variable list synchronously. The list must have been created previously by a call to |SyncDefineVarList|
 *
 * .. important:: As communication is involved, take care (async calls etc.) when using this method from an IEC-Task
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlersyncwritevarlisttoplc_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The variable list to write. */
	RTS_IEC_BYTE **ppValues;			/* VAR_INPUT */	/* A POINTER to an ARRAY of POINTERs to the values that should be written. */
	RTS_IEC_UDINT *pulSizes;			/* VAR_INPUT */	/* An optional POINTER to an ARRAY of sizes of each item. If omitted, the performance of the write operation might not be optimal
 because the according information has to be derived from the internal symbol tables. */
	RTS_IEC_UDINT numVars;				/* VAR_INPUT */	/* The number of variables to write. In fact this number represents the sizes of the arrays given by ``ppValues`` and ``pulSizes`` so
 all three arrays are expected to have the same size. */
	RTS_IEC_INT PLCHandlerSyncWriteVarListToPlc;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlersyncwritevarlisttoplc_struct;

void CDECL CDECL_EXT plchandlersyncwritevarlisttoplc(plchandlersyncwritevarlisttoplc_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERSYNCWRITEVARLISTTOPLC_IEC) (plchandlersyncwritevarlisttoplc_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERSYNCWRITEVARLISTTOPLC_NOTIMPLEMENTED)
	#define USE_plchandlersyncwritevarlisttoplc
	#define EXT_plchandlersyncwritevarlisttoplc
	#define GET_plchandlersyncwritevarlisttoplc(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlersyncwritevarlisttoplc(p0) 
	#define CHK_plchandlersyncwritevarlisttoplc  FALSE
	#define EXP_plchandlersyncwritevarlisttoplc  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlersyncwritevarlisttoplc
	#define EXT_plchandlersyncwritevarlisttoplc
	#define GET_plchandlersyncwritevarlisttoplc(fl)  CAL_CMGETAPI( "plchandlersyncwritevarlisttoplc" ) 
	#define CAL_plchandlersyncwritevarlisttoplc  plchandlersyncwritevarlisttoplc
	#define CHK_plchandlersyncwritevarlisttoplc  TRUE
	#define EXP_plchandlersyncwritevarlisttoplc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevarlisttoplc", (RTS_UINTPTR)plchandlersyncwritevarlisttoplc, 1, RTSITF_GET_SIGNATURE(0xA486E098, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlersyncwritevarlisttoplc {(RTS_VOID_FCTPTR)plchandlersyncwritevarlisttoplc, "plchandlersyncwritevarlisttoplc", RTSITF_GET_SIGNATURE(0xA486E098, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlersyncwritevarlisttoplc
	#define EXT_plchandlersyncwritevarlisttoplc
	#define GET_plchandlersyncwritevarlisttoplc(fl)  CAL_CMGETAPI( "plchandlersyncwritevarlisttoplc" ) 
	#define CAL_plchandlersyncwritevarlisttoplc  plchandlersyncwritevarlisttoplc
	#define CHK_plchandlersyncwritevarlisttoplc  TRUE
	#define EXP_plchandlersyncwritevarlisttoplc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevarlisttoplc", (RTS_UINTPTR)plchandlersyncwritevarlisttoplc, 1, RTSITF_GET_SIGNATURE(0xA486E098, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlersyncwritevarlisttoplc
	#define EXT_CmpPLCHandlerplchandlersyncwritevarlisttoplc
	#define GET_CmpPLCHandlerplchandlersyncwritevarlisttoplc  ERR_OK
	#define CAL_CmpPLCHandlerplchandlersyncwritevarlisttoplc  plchandlersyncwritevarlisttoplc
	#define CHK_CmpPLCHandlerplchandlersyncwritevarlisttoplc  TRUE
	#define EXP_CmpPLCHandlerplchandlersyncwritevarlisttoplc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevarlisttoplc", (RTS_UINTPTR)plchandlersyncwritevarlisttoplc, 1, RTSITF_GET_SIGNATURE(0xA486E098, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlersyncwritevarlisttoplc
	#define EXT_plchandlersyncwritevarlisttoplc
	#define GET_plchandlersyncwritevarlisttoplc(fl)  CAL_CMGETAPI( "plchandlersyncwritevarlisttoplc" ) 
	#define CAL_plchandlersyncwritevarlisttoplc  plchandlersyncwritevarlisttoplc
	#define CHK_plchandlersyncwritevarlisttoplc  TRUE
	#define EXP_plchandlersyncwritevarlisttoplc  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevarlisttoplc", (RTS_UINTPTR)plchandlersyncwritevarlisttoplc, 1, RTSITF_GET_SIGNATURE(0xA486E098, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlersyncwritevarlisttoplc  PFPLCHANDLERSYNCWRITEVARLISTTOPLC_IEC pfplchandlersyncwritevarlisttoplc;
	#define EXT_plchandlersyncwritevarlisttoplc  extern PFPLCHANDLERSYNCWRITEVARLISTTOPLC_IEC pfplchandlersyncwritevarlisttoplc;
	#define GET_plchandlersyncwritevarlisttoplc(fl)  s_pfCMGetAPI2( "plchandlersyncwritevarlisttoplc", (RTS_VOID_FCTPTR *)&pfplchandlersyncwritevarlisttoplc, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xA486E098, 0), 0x03051200)
	#define CAL_plchandlersyncwritevarlisttoplc  pfplchandlersyncwritevarlisttoplc
	#define CHK_plchandlersyncwritevarlisttoplc  (pfplchandlersyncwritevarlisttoplc != NULL)
	#define EXP_plchandlersyncwritevarlisttoplc   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevarlisttoplc", (RTS_UINTPTR)plchandlersyncwritevarlisttoplc, 1, RTSITF_GET_SIGNATURE(0xA486E098, 0), 0x03051200) 
#endif


/**
 * Writes the given list of values synchronously to the given list of variables.
 *
 * .. important:: As communication and symbolic resolution is involved, take care (async calls etc.) when using this method from an IEC-Task
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlersyncwritevars_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_BYTE **ppInstPaths;			/* VAR_INPUT */	/* A POINTER to an ARRAY of POINTERs to zero terminated ASCII STRINGs representing the instance paths that should be written */
	RTS_IEC_BYTE **ppValues;			/* VAR_INPUT */	/* A POINTER to an ARRAY of POINTERs to the values that should be written. */
	RTS_IEC_UDINT *pulSizes;			/* VAR_INPUT */	/* An optional POINTER to an ARRAY of sizes of each item. If omitted, the performance of the write operation might not be optimal
 because the according information has to be derived from the internal symbol tables. */
	RTS_IEC_UDINT numVars;				/* VAR_INPUT */	/* The number of variables to write. In fact this number represents the sizes of the arrays given by ``ppInstPaths``, ``ppValues`` and ``pulSizes`` so
 all three arrays are expected to have the same size. */
	RTS_IEC_INT PLCHandlerSyncWriteVars;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlersyncwritevars_struct;

void CDECL CDECL_EXT plchandlersyncwritevars(plchandlersyncwritevars_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERSYNCWRITEVARS_IEC) (plchandlersyncwritevars_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERSYNCWRITEVARS_NOTIMPLEMENTED)
	#define USE_plchandlersyncwritevars
	#define EXT_plchandlersyncwritevars
	#define GET_plchandlersyncwritevars(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlersyncwritevars(p0) 
	#define CHK_plchandlersyncwritevars  FALSE
	#define EXP_plchandlersyncwritevars  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlersyncwritevars
	#define EXT_plchandlersyncwritevars
	#define GET_plchandlersyncwritevars(fl)  CAL_CMGETAPI( "plchandlersyncwritevars" ) 
	#define CAL_plchandlersyncwritevars  plchandlersyncwritevars
	#define CHK_plchandlersyncwritevars  TRUE
	#define EXP_plchandlersyncwritevars  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevars", (RTS_UINTPTR)plchandlersyncwritevars, 1, RTSITF_GET_SIGNATURE(0xDACACBAF, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlersyncwritevars {(RTS_VOID_FCTPTR)plchandlersyncwritevars, "plchandlersyncwritevars", RTSITF_GET_SIGNATURE(0xDACACBAF, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlersyncwritevars
	#define EXT_plchandlersyncwritevars
	#define GET_plchandlersyncwritevars(fl)  CAL_CMGETAPI( "plchandlersyncwritevars" ) 
	#define CAL_plchandlersyncwritevars  plchandlersyncwritevars
	#define CHK_plchandlersyncwritevars  TRUE
	#define EXP_plchandlersyncwritevars  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevars", (RTS_UINTPTR)plchandlersyncwritevars, 1, RTSITF_GET_SIGNATURE(0xDACACBAF, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlersyncwritevars
	#define EXT_CmpPLCHandlerplchandlersyncwritevars
	#define GET_CmpPLCHandlerplchandlersyncwritevars  ERR_OK
	#define CAL_CmpPLCHandlerplchandlersyncwritevars  plchandlersyncwritevars
	#define CHK_CmpPLCHandlerplchandlersyncwritevars  TRUE
	#define EXP_CmpPLCHandlerplchandlersyncwritevars  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevars", (RTS_UINTPTR)plchandlersyncwritevars, 1, RTSITF_GET_SIGNATURE(0xDACACBAF, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlersyncwritevars
	#define EXT_plchandlersyncwritevars
	#define GET_plchandlersyncwritevars(fl)  CAL_CMGETAPI( "plchandlersyncwritevars" ) 
	#define CAL_plchandlersyncwritevars  plchandlersyncwritevars
	#define CHK_plchandlersyncwritevars  TRUE
	#define EXP_plchandlersyncwritevars  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevars", (RTS_UINTPTR)plchandlersyncwritevars, 1, RTSITF_GET_SIGNATURE(0xDACACBAF, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlersyncwritevars  PFPLCHANDLERSYNCWRITEVARS_IEC pfplchandlersyncwritevars;
	#define EXT_plchandlersyncwritevars  extern PFPLCHANDLERSYNCWRITEVARS_IEC pfplchandlersyncwritevars;
	#define GET_plchandlersyncwritevars(fl)  s_pfCMGetAPI2( "plchandlersyncwritevars", (RTS_VOID_FCTPTR *)&pfplchandlersyncwritevars, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xDACACBAF, 0), 0x03051200)
	#define CAL_plchandlersyncwritevars  pfplchandlersyncwritevars
	#define CHK_plchandlersyncwritevars  (pfplchandlersyncwritevars != NULL)
	#define EXP_plchandlersyncwritevars   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlersyncwritevars", (RTS_UINTPTR)plchandlersyncwritevars, 1, RTSITF_GET_SIGNATURE(0xDACACBAF, 0), 0x03051200) 
#endif


/**
 * Creates a new variable list that will be cyclically updated by the underlying
 * PLCHandler instance. The returned handle must be deleted by calling |VarListDelete| if no longer in use.
 *
 * .. important:: Calling this method might take some time (for registering the list in the connected PLC)
 *    so be careful when calling from an IEC-Task. Typically this method should be called in an
 *    asynchronous way.
 *
 * :return: A handle to the newly created variable list  
 */
typedef struct tagplchandlervarlistdefine_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance to define the variable list on. */
	RTS_IEC_BYTE **ppInstPaths;			/* VAR_INPUT */	/* A POINTER to an ARRAY of POINTERs to zero terminated ASCII STRINGs representing the instance paths that should be monitored */
	RTS_IEC_UDINT numVars;				/* VAR_INPUT */	/* The number of variables for the new variable list as pointed to by ``ppInstPaths`` */
	RTS_IEC_UDINT updateRate;			/* VAR_INPUT */	/* The update rate for this variable list in milliseconds */
	RTS_IEC_HANDLE PLCHandlerVarListDefine;	/* VAR_OUTPUT */	
	RTS_IEC_INT result;					/* VAR_OUTPUT, Enum: ERROR */
} plchandlervarlistdefine_struct;

void CDECL CDECL_EXT plchandlervarlistdefine(plchandlervarlistdefine_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERVARLISTDEFINE_IEC) (plchandlervarlistdefine_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERVARLISTDEFINE_NOTIMPLEMENTED)
	#define USE_plchandlervarlistdefine
	#define EXT_plchandlervarlistdefine
	#define GET_plchandlervarlistdefine(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlervarlistdefine(p0) 
	#define CHK_plchandlervarlistdefine  FALSE
	#define EXP_plchandlervarlistdefine  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlervarlistdefine
	#define EXT_plchandlervarlistdefine
	#define GET_plchandlervarlistdefine(fl)  CAL_CMGETAPI( "plchandlervarlistdefine" ) 
	#define CAL_plchandlervarlistdefine  plchandlervarlistdefine
	#define CHK_plchandlervarlistdefine  TRUE
	#define EXP_plchandlervarlistdefine  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdefine", (RTS_UINTPTR)plchandlervarlistdefine, 1, RTSITF_GET_SIGNATURE(0x81C780F4, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlervarlistdefine {(RTS_VOID_FCTPTR)plchandlervarlistdefine, "plchandlervarlistdefine", RTSITF_GET_SIGNATURE(0x81C780F4, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlervarlistdefine
	#define EXT_plchandlervarlistdefine
	#define GET_plchandlervarlistdefine(fl)  CAL_CMGETAPI( "plchandlervarlistdefine" ) 
	#define CAL_plchandlervarlistdefine  plchandlervarlistdefine
	#define CHK_plchandlervarlistdefine  TRUE
	#define EXP_plchandlervarlistdefine  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdefine", (RTS_UINTPTR)plchandlervarlistdefine, 1, RTSITF_GET_SIGNATURE(0x81C780F4, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlervarlistdefine
	#define EXT_CmpPLCHandlerplchandlervarlistdefine
	#define GET_CmpPLCHandlerplchandlervarlistdefine  ERR_OK
	#define CAL_CmpPLCHandlerplchandlervarlistdefine  plchandlervarlistdefine
	#define CHK_CmpPLCHandlerplchandlervarlistdefine  TRUE
	#define EXP_CmpPLCHandlerplchandlervarlistdefine  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdefine", (RTS_UINTPTR)plchandlervarlistdefine, 1, RTSITF_GET_SIGNATURE(0x81C780F4, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlervarlistdefine
	#define EXT_plchandlervarlistdefine
	#define GET_plchandlervarlistdefine(fl)  CAL_CMGETAPI( "plchandlervarlistdefine" ) 
	#define CAL_plchandlervarlistdefine  plchandlervarlistdefine
	#define CHK_plchandlervarlistdefine  TRUE
	#define EXP_plchandlervarlistdefine  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdefine", (RTS_UINTPTR)plchandlervarlistdefine, 1, RTSITF_GET_SIGNATURE(0x81C780F4, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlervarlistdefine  PFPLCHANDLERVARLISTDEFINE_IEC pfplchandlervarlistdefine;
	#define EXT_plchandlervarlistdefine  extern PFPLCHANDLERVARLISTDEFINE_IEC pfplchandlervarlistdefine;
	#define GET_plchandlervarlistdefine(fl)  s_pfCMGetAPI2( "plchandlervarlistdefine", (RTS_VOID_FCTPTR *)&pfplchandlervarlistdefine, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x81C780F4, 0), 0x03051200)
	#define CAL_plchandlervarlistdefine  pfplchandlervarlistdefine
	#define CHK_plchandlervarlistdefine  (pfplchandlervarlistdefine != NULL)
	#define EXP_plchandlervarlistdefine   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdefine", (RTS_UINTPTR)plchandlervarlistdefine, 1, RTSITF_GET_SIGNATURE(0x81C780F4, 0), 0x03051200) 
#endif


/**
 * Deletes a variable list previously created by |VarListDefine|
 *
 * .. important:: Calling this method might take some time (due to necessary communication)
 *    so be careful when calling from an IEC-Task. Typically this method should be called in an
 *    asynchronous way. 
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlervarlistdelete_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance to delete the variable list on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The varlist to delete */
	RTS_IEC_INT PLCHandlerVarListDelete;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlervarlistdelete_struct;

void CDECL CDECL_EXT plchandlervarlistdelete(plchandlervarlistdelete_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERVARLISTDELETE_IEC) (plchandlervarlistdelete_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERVARLISTDELETE_NOTIMPLEMENTED)
	#define USE_plchandlervarlistdelete
	#define EXT_plchandlervarlistdelete
	#define GET_plchandlervarlistdelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlervarlistdelete(p0) 
	#define CHK_plchandlervarlistdelete  FALSE
	#define EXP_plchandlervarlistdelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlervarlistdelete
	#define EXT_plchandlervarlistdelete
	#define GET_plchandlervarlistdelete(fl)  CAL_CMGETAPI( "plchandlervarlistdelete" ) 
	#define CAL_plchandlervarlistdelete  plchandlervarlistdelete
	#define CHK_plchandlervarlistdelete  TRUE
	#define EXP_plchandlervarlistdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdelete", (RTS_UINTPTR)plchandlervarlistdelete, 1, RTSITF_GET_SIGNATURE(0xAD8E5D4F, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlervarlistdelete {(RTS_VOID_FCTPTR)plchandlervarlistdelete, "plchandlervarlistdelete", RTSITF_GET_SIGNATURE(0xAD8E5D4F, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlervarlistdelete
	#define EXT_plchandlervarlistdelete
	#define GET_plchandlervarlistdelete(fl)  CAL_CMGETAPI( "plchandlervarlistdelete" ) 
	#define CAL_plchandlervarlistdelete  plchandlervarlistdelete
	#define CHK_plchandlervarlistdelete  TRUE
	#define EXP_plchandlervarlistdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdelete", (RTS_UINTPTR)plchandlervarlistdelete, 1, RTSITF_GET_SIGNATURE(0xAD8E5D4F, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlervarlistdelete
	#define EXT_CmpPLCHandlerplchandlervarlistdelete
	#define GET_CmpPLCHandlerplchandlervarlistdelete  ERR_OK
	#define CAL_CmpPLCHandlerplchandlervarlistdelete  plchandlervarlistdelete
	#define CHK_CmpPLCHandlerplchandlervarlistdelete  TRUE
	#define EXP_CmpPLCHandlerplchandlervarlistdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdelete", (RTS_UINTPTR)plchandlervarlistdelete, 1, RTSITF_GET_SIGNATURE(0xAD8E5D4F, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlervarlistdelete
	#define EXT_plchandlervarlistdelete
	#define GET_plchandlervarlistdelete(fl)  CAL_CMGETAPI( "plchandlervarlistdelete" ) 
	#define CAL_plchandlervarlistdelete  plchandlervarlistdelete
	#define CHK_plchandlervarlistdelete  TRUE
	#define EXP_plchandlervarlistdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdelete", (RTS_UINTPTR)plchandlervarlistdelete, 1, RTSITF_GET_SIGNATURE(0xAD8E5D4F, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlervarlistdelete  PFPLCHANDLERVARLISTDELETE_IEC pfplchandlervarlistdelete;
	#define EXT_plchandlervarlistdelete  extern PFPLCHANDLERVARLISTDELETE_IEC pfplchandlervarlistdelete;
	#define GET_plchandlervarlistdelete(fl)  s_pfCMGetAPI2( "plchandlervarlistdelete", (RTS_VOID_FCTPTR *)&pfplchandlervarlistdelete, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xAD8E5D4F, 0), 0x03051200)
	#define CAL_plchandlervarlistdelete  pfplchandlervarlistdelete
	#define CHK_plchandlervarlistdelete  (pfplchandlervarlistdelete != NULL)
	#define EXP_plchandlervarlistdelete   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdelete", (RTS_UINTPTR)plchandlervarlistdelete, 1, RTSITF_GET_SIGNATURE(0xAD8E5D4F, 0), 0x03051200) 
#endif


/**
 * Deactivates monitoring the given variable list.
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlervarlistdisable_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The varlist to disable */
	RTS_IEC_INT PLCHandlerVarListDisable;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlervarlistdisable_struct;

void CDECL CDECL_EXT plchandlervarlistdisable(plchandlervarlistdisable_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERVARLISTDISABLE_IEC) (plchandlervarlistdisable_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERVARLISTDISABLE_NOTIMPLEMENTED)
	#define USE_plchandlervarlistdisable
	#define EXT_plchandlervarlistdisable
	#define GET_plchandlervarlistdisable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlervarlistdisable(p0) 
	#define CHK_plchandlervarlistdisable  FALSE
	#define EXP_plchandlervarlistdisable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlervarlistdisable
	#define EXT_plchandlervarlistdisable
	#define GET_plchandlervarlistdisable(fl)  CAL_CMGETAPI( "plchandlervarlistdisable" ) 
	#define CAL_plchandlervarlistdisable  plchandlervarlistdisable
	#define CHK_plchandlervarlistdisable  TRUE
	#define EXP_plchandlervarlistdisable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdisable", (RTS_UINTPTR)plchandlervarlistdisable, 1, RTSITF_GET_SIGNATURE(0xA9F0A7EB, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlervarlistdisable {(RTS_VOID_FCTPTR)plchandlervarlistdisable, "plchandlervarlistdisable", RTSITF_GET_SIGNATURE(0xA9F0A7EB, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlervarlistdisable
	#define EXT_plchandlervarlistdisable
	#define GET_plchandlervarlistdisable(fl)  CAL_CMGETAPI( "plchandlervarlistdisable" ) 
	#define CAL_plchandlervarlistdisable  plchandlervarlistdisable
	#define CHK_plchandlervarlistdisable  TRUE
	#define EXP_plchandlervarlistdisable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdisable", (RTS_UINTPTR)plchandlervarlistdisable, 1, RTSITF_GET_SIGNATURE(0xA9F0A7EB, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlervarlistdisable
	#define EXT_CmpPLCHandlerplchandlervarlistdisable
	#define GET_CmpPLCHandlerplchandlervarlistdisable  ERR_OK
	#define CAL_CmpPLCHandlerplchandlervarlistdisable  plchandlervarlistdisable
	#define CHK_CmpPLCHandlerplchandlervarlistdisable  TRUE
	#define EXP_CmpPLCHandlerplchandlervarlistdisable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdisable", (RTS_UINTPTR)plchandlervarlistdisable, 1, RTSITF_GET_SIGNATURE(0xA9F0A7EB, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlervarlistdisable
	#define EXT_plchandlervarlistdisable
	#define GET_plchandlervarlistdisable(fl)  CAL_CMGETAPI( "plchandlervarlistdisable" ) 
	#define CAL_plchandlervarlistdisable  plchandlervarlistdisable
	#define CHK_plchandlervarlistdisable  TRUE
	#define EXP_plchandlervarlistdisable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdisable", (RTS_UINTPTR)plchandlervarlistdisable, 1, RTSITF_GET_SIGNATURE(0xA9F0A7EB, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlervarlistdisable  PFPLCHANDLERVARLISTDISABLE_IEC pfplchandlervarlistdisable;
	#define EXT_plchandlervarlistdisable  extern PFPLCHANDLERVARLISTDISABLE_IEC pfplchandlervarlistdisable;
	#define GET_plchandlervarlistdisable(fl)  s_pfCMGetAPI2( "plchandlervarlistdisable", (RTS_VOID_FCTPTR *)&pfplchandlervarlistdisable, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0xA9F0A7EB, 0), 0x03051200)
	#define CAL_plchandlervarlistdisable  pfplchandlervarlistdisable
	#define CHK_plchandlervarlistdisable  (pfplchandlervarlistdisable != NULL)
	#define EXP_plchandlervarlistdisable   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistdisable", (RTS_UINTPTR)plchandlervarlistdisable, 1, RTSITF_GET_SIGNATURE(0xA9F0A7EB, 0), 0x03051200) 
#endif


/**
 * Activates monitoring the given variable list.
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlervarlistenable_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The varlist to enable. */
	RTS_IEC_INT PLCHandlerVarListEnable;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlervarlistenable_struct;

void CDECL CDECL_EXT plchandlervarlistenable(plchandlervarlistenable_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERVARLISTENABLE_IEC) (plchandlervarlistenable_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERVARLISTENABLE_NOTIMPLEMENTED)
	#define USE_plchandlervarlistenable
	#define EXT_plchandlervarlistenable
	#define GET_plchandlervarlistenable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlervarlistenable(p0) 
	#define CHK_plchandlervarlistenable  FALSE
	#define EXP_plchandlervarlistenable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlervarlistenable
	#define EXT_plchandlervarlistenable
	#define GET_plchandlervarlistenable(fl)  CAL_CMGETAPI( "plchandlervarlistenable" ) 
	#define CAL_plchandlervarlistenable  plchandlervarlistenable
	#define CHK_plchandlervarlistenable  TRUE
	#define EXP_plchandlervarlistenable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenable", (RTS_UINTPTR)plchandlervarlistenable, 1, RTSITF_GET_SIGNATURE(0x5EDC0FD8, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlervarlistenable {(RTS_VOID_FCTPTR)plchandlervarlistenable, "plchandlervarlistenable", RTSITF_GET_SIGNATURE(0x5EDC0FD8, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlervarlistenable
	#define EXT_plchandlervarlistenable
	#define GET_plchandlervarlistenable(fl)  CAL_CMGETAPI( "plchandlervarlistenable" ) 
	#define CAL_plchandlervarlistenable  plchandlervarlistenable
	#define CHK_plchandlervarlistenable  TRUE
	#define EXP_plchandlervarlistenable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenable", (RTS_UINTPTR)plchandlervarlistenable, 1, RTSITF_GET_SIGNATURE(0x5EDC0FD8, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlervarlistenable
	#define EXT_CmpPLCHandlerplchandlervarlistenable
	#define GET_CmpPLCHandlerplchandlervarlistenable  ERR_OK
	#define CAL_CmpPLCHandlerplchandlervarlistenable  plchandlervarlistenable
	#define CHK_CmpPLCHandlerplchandlervarlistenable  TRUE
	#define EXP_CmpPLCHandlerplchandlervarlistenable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenable", (RTS_UINTPTR)plchandlervarlistenable, 1, RTSITF_GET_SIGNATURE(0x5EDC0FD8, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlervarlistenable
	#define EXT_plchandlervarlistenable
	#define GET_plchandlervarlistenable(fl)  CAL_CMGETAPI( "plchandlervarlistenable" ) 
	#define CAL_plchandlervarlistenable  plchandlervarlistenable
	#define CHK_plchandlervarlistenable  TRUE
	#define EXP_plchandlervarlistenable  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenable", (RTS_UINTPTR)plchandlervarlistenable, 1, RTSITF_GET_SIGNATURE(0x5EDC0FD8, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlervarlistenable  PFPLCHANDLERVARLISTENABLE_IEC pfplchandlervarlistenable;
	#define EXT_plchandlervarlistenable  extern PFPLCHANDLERVARLISTENABLE_IEC pfplchandlervarlistenable;
	#define GET_plchandlervarlistenable(fl)  s_pfCMGetAPI2( "plchandlervarlistenable", (RTS_VOID_FCTPTR *)&pfplchandlervarlistenable, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x5EDC0FD8, 0), 0x03051200)
	#define CAL_plchandlervarlistenable  pfplchandlervarlistenable
	#define CHK_plchandlervarlistenable  (pfplchandlervarlistenable != NULL)
	#define EXP_plchandlervarlistenable   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenable", (RTS_UINTPTR)plchandlervarlistenable, 1, RTSITF_GET_SIGNATURE(0x5EDC0FD8, 0), 0x03051200) 
#endif


/**
 * Protects the access to the variable list so it cannot be changed by the update thread.
 * Should be used only in combination with |VarListRead|
 *
 * .. important:: After using the variable list it must be released by calling |VarListLeave| 
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlervarlistenter_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The varlist to lock access on. */
	RTS_IEC_INT PLCHandlerVarListEnter;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlervarlistenter_struct;

void CDECL CDECL_EXT plchandlervarlistenter(plchandlervarlistenter_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERVARLISTENTER_IEC) (plchandlervarlistenter_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERVARLISTENTER_NOTIMPLEMENTED)
	#define USE_plchandlervarlistenter
	#define EXT_plchandlervarlistenter
	#define GET_plchandlervarlistenter(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlervarlistenter(p0) 
	#define CHK_plchandlervarlistenter  FALSE
	#define EXP_plchandlervarlistenter  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlervarlistenter
	#define EXT_plchandlervarlistenter
	#define GET_plchandlervarlistenter(fl)  CAL_CMGETAPI( "plchandlervarlistenter" ) 
	#define CAL_plchandlervarlistenter  plchandlervarlistenter
	#define CHK_plchandlervarlistenter  TRUE
	#define EXP_plchandlervarlistenter  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenter", (RTS_UINTPTR)plchandlervarlistenter, 1, RTSITF_GET_SIGNATURE(0x7F252097, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlervarlistenter {(RTS_VOID_FCTPTR)plchandlervarlistenter, "plchandlervarlistenter", RTSITF_GET_SIGNATURE(0x7F252097, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlervarlistenter
	#define EXT_plchandlervarlistenter
	#define GET_plchandlervarlistenter(fl)  CAL_CMGETAPI( "plchandlervarlistenter" ) 
	#define CAL_plchandlervarlistenter  plchandlervarlistenter
	#define CHK_plchandlervarlistenter  TRUE
	#define EXP_plchandlervarlistenter  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenter", (RTS_UINTPTR)plchandlervarlistenter, 1, RTSITF_GET_SIGNATURE(0x7F252097, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlervarlistenter
	#define EXT_CmpPLCHandlerplchandlervarlistenter
	#define GET_CmpPLCHandlerplchandlervarlistenter  ERR_OK
	#define CAL_CmpPLCHandlerplchandlervarlistenter  plchandlervarlistenter
	#define CHK_CmpPLCHandlerplchandlervarlistenter  TRUE
	#define EXP_CmpPLCHandlerplchandlervarlistenter  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenter", (RTS_UINTPTR)plchandlervarlistenter, 1, RTSITF_GET_SIGNATURE(0x7F252097, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlervarlistenter
	#define EXT_plchandlervarlistenter
	#define GET_plchandlervarlistenter(fl)  CAL_CMGETAPI( "plchandlervarlistenter" ) 
	#define CAL_plchandlervarlistenter  plchandlervarlistenter
	#define CHK_plchandlervarlistenter  TRUE
	#define EXP_plchandlervarlistenter  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenter", (RTS_UINTPTR)plchandlervarlistenter, 1, RTSITF_GET_SIGNATURE(0x7F252097, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlervarlistenter  PFPLCHANDLERVARLISTENTER_IEC pfplchandlervarlistenter;
	#define EXT_plchandlervarlistenter  extern PFPLCHANDLERVARLISTENTER_IEC pfplchandlervarlistenter;
	#define GET_plchandlervarlistenter(fl)  s_pfCMGetAPI2( "plchandlervarlistenter", (RTS_VOID_FCTPTR *)&pfplchandlervarlistenter, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x7F252097, 0), 0x03051200)
	#define CAL_plchandlervarlistenter  pfplchandlervarlistenter
	#define CHK_plchandlervarlistenter  (pfplchandlervarlistenter != NULL)
	#define EXP_plchandlervarlistenter   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistenter", (RTS_UINTPTR)plchandlervarlistenter, 1, RTSITF_GET_SIGNATURE(0x7F252097, 0), 0x03051200) 
#endif


/**
 * Releases the access to the variable list so it can be changed again by the update thread.
 * Should be used only in combination with |VarListRead|
 *
 * .. important:: Must be called only after a previous call to |VarListEnter| 
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlervarlistleave_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The varlist to release access on. */
	RTS_IEC_INT PLCHandlerVarListLeave;	/* VAR_OUTPUT, Enum: ERROR */
} plchandlervarlistleave_struct;

void CDECL CDECL_EXT plchandlervarlistleave(plchandlervarlistleave_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERVARLISTLEAVE_IEC) (plchandlervarlistleave_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERVARLISTLEAVE_NOTIMPLEMENTED)
	#define USE_plchandlervarlistleave
	#define EXT_plchandlervarlistleave
	#define GET_plchandlervarlistleave(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlervarlistleave(p0) 
	#define CHK_plchandlervarlistleave  FALSE
	#define EXP_plchandlervarlistleave  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlervarlistleave
	#define EXT_plchandlervarlistleave
	#define GET_plchandlervarlistleave(fl)  CAL_CMGETAPI( "plchandlervarlistleave" ) 
	#define CAL_plchandlervarlistleave  plchandlervarlistleave
	#define CHK_plchandlervarlistleave  TRUE
	#define EXP_plchandlervarlistleave  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistleave", (RTS_UINTPTR)plchandlervarlistleave, 1, RTSITF_GET_SIGNATURE(0x6150164D, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlervarlistleave {(RTS_VOID_FCTPTR)plchandlervarlistleave, "plchandlervarlistleave", RTSITF_GET_SIGNATURE(0x6150164D, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlervarlistleave
	#define EXT_plchandlervarlistleave
	#define GET_plchandlervarlistleave(fl)  CAL_CMGETAPI( "plchandlervarlistleave" ) 
	#define CAL_plchandlervarlistleave  plchandlervarlistleave
	#define CHK_plchandlervarlistleave  TRUE
	#define EXP_plchandlervarlistleave  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistleave", (RTS_UINTPTR)plchandlervarlistleave, 1, RTSITF_GET_SIGNATURE(0x6150164D, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlervarlistleave
	#define EXT_CmpPLCHandlerplchandlervarlistleave
	#define GET_CmpPLCHandlerplchandlervarlistleave  ERR_OK
	#define CAL_CmpPLCHandlerplchandlervarlistleave  plchandlervarlistleave
	#define CHK_CmpPLCHandlerplchandlervarlistleave  TRUE
	#define EXP_CmpPLCHandlerplchandlervarlistleave  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistleave", (RTS_UINTPTR)plchandlervarlistleave, 1, RTSITF_GET_SIGNATURE(0x6150164D, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlervarlistleave
	#define EXT_plchandlervarlistleave
	#define GET_plchandlervarlistleave(fl)  CAL_CMGETAPI( "plchandlervarlistleave" ) 
	#define CAL_plchandlervarlistleave  plchandlervarlistleave
	#define CHK_plchandlervarlistleave  TRUE
	#define EXP_plchandlervarlistleave  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistleave", (RTS_UINTPTR)plchandlervarlistleave, 1, RTSITF_GET_SIGNATURE(0x6150164D, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlervarlistleave  PFPLCHANDLERVARLISTLEAVE_IEC pfplchandlervarlistleave;
	#define EXT_plchandlervarlistleave  extern PFPLCHANDLERVARLISTLEAVE_IEC pfplchandlervarlistleave;
	#define GET_plchandlervarlistleave(fl)  s_pfCMGetAPI2( "plchandlervarlistleave", (RTS_VOID_FCTPTR *)&pfplchandlervarlistleave, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x6150164D, 0), 0x03051200)
	#define CAL_plchandlervarlistleave  pfplchandlervarlistleave
	#define CHK_plchandlervarlistleave  (pfplchandlervarlistleave != NULL)
	#define EXP_plchandlervarlistleave   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistleave", (RTS_UINTPTR)plchandlervarlistleave, 1, RTSITF_GET_SIGNATURE(0x6150164D, 0), 0x03051200) 
#endif


/**
 * Reads the cached (last retrieved) values from the given variable list.
 *
 * .. important:: Must be called only in code protected by |VarListEnter| and |VarListLeave| 
 *
 * :return: An errorcode representing the result of this operation
 */
typedef struct tagplchandlervarlistread_struct
{
	RTS_IEC_HANDLE hPlcHandler;			/* VAR_INPUT */	/* The instance of the PLCHandler to work on. */
	RTS_IEC_HANDLE hVarList;			/* VAR_INPUT */	/* The varlist to read the values from */
	RTS_IEC_INT PLCHandlerVarListRead;	/* VAR_OUTPUT, Enum: ERROR */
	VariableValue **ppVarInfos;			/* VAR_OUTPUT */	/* An ARRAY OF POINTERs to VarInfo instances representing each single read value */
	RTS_IEC_UDINT numVars;				/* VAR_OUTPUT */	/* The number of variables in ``ppVarInfos`` */
} plchandlervarlistread_struct;

void CDECL CDECL_EXT plchandlervarlistread(plchandlervarlistread_struct *p);
typedef void (CDECL CDECL_EXT* PFPLCHANDLERVARLISTREAD_IEC) (plchandlervarlistread_struct *p);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERVARLISTREAD_NOTIMPLEMENTED)
	#define USE_plchandlervarlistread
	#define EXT_plchandlervarlistread
	#define GET_plchandlervarlistread(fl)  ERR_NOTIMPLEMENTED
	#define CAL_plchandlervarlistread(p0) 
	#define CHK_plchandlervarlistread  FALSE
	#define EXP_plchandlervarlistread  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_plchandlervarlistread
	#define EXT_plchandlervarlistread
	#define GET_plchandlervarlistread(fl)  CAL_CMGETAPI( "plchandlervarlistread" ) 
	#define CAL_plchandlervarlistread  plchandlervarlistread
	#define CHK_plchandlervarlistread  TRUE
	#define EXP_plchandlervarlistread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistread", (RTS_UINTPTR)plchandlervarlistread, 1, RTSITF_GET_SIGNATURE(0x5FFAB125, 0), 0x03051200) 
	#define EXTREF_ITF_plchandlervarlistread {(RTS_VOID_FCTPTR)plchandlervarlistread, "plchandlervarlistread", RTSITF_GET_SIGNATURE(0x5FFAB125, 0), 0x03051200}
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_plchandlervarlistread
	#define EXT_plchandlervarlistread
	#define GET_plchandlervarlistread(fl)  CAL_CMGETAPI( "plchandlervarlistread" ) 
	#define CAL_plchandlervarlistread  plchandlervarlistread
	#define CHK_plchandlervarlistread  TRUE
	#define EXP_plchandlervarlistread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistread", (RTS_UINTPTR)plchandlervarlistread, 1, RTSITF_GET_SIGNATURE(0x5FFAB125, 0), 0x03051200) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerplchandlervarlistread
	#define EXT_CmpPLCHandlerplchandlervarlistread
	#define GET_CmpPLCHandlerplchandlervarlistread  ERR_OK
	#define CAL_CmpPLCHandlerplchandlervarlistread  plchandlervarlistread
	#define CHK_CmpPLCHandlerplchandlervarlistread  TRUE
	#define EXP_CmpPLCHandlerplchandlervarlistread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistread", (RTS_UINTPTR)plchandlervarlistread, 1, RTSITF_GET_SIGNATURE(0x5FFAB125, 0), 0x03051200) 
#elif defined(CPLUSPLUS)
	#define USE_plchandlervarlistread
	#define EXT_plchandlervarlistread
	#define GET_plchandlervarlistread(fl)  CAL_CMGETAPI( "plchandlervarlistread" ) 
	#define CAL_plchandlervarlistread  plchandlervarlistread
	#define CHK_plchandlervarlistread  TRUE
	#define EXP_plchandlervarlistread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistread", (RTS_UINTPTR)plchandlervarlistread, 1, RTSITF_GET_SIGNATURE(0x5FFAB125, 0), 0x03051200) 
#else /* DYNAMIC_LINK */
	#define USE_plchandlervarlistread  PFPLCHANDLERVARLISTREAD_IEC pfplchandlervarlistread;
	#define EXT_plchandlervarlistread  extern PFPLCHANDLERVARLISTREAD_IEC pfplchandlervarlistread;
	#define GET_plchandlervarlistread(fl)  s_pfCMGetAPI2( "plchandlervarlistread", (RTS_VOID_FCTPTR *)&pfplchandlervarlistread, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x5FFAB125, 0), 0x03051200)
	#define CAL_plchandlervarlistread  pfplchandlervarlistread
	#define CHK_plchandlervarlistread  (pfplchandlervarlistread != NULL)
	#define EXP_plchandlervarlistread   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"plchandlervarlistread", (RTS_UINTPTR)plchandlervarlistread, 1, RTSITF_GET_SIGNATURE(0x5FFAB125, 0), 0x03051200) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Function to create a new PLCHandler instance.
 * <p>NOTE:The returned instance must be released by calling PLCHandlerDeleteInstance</p>
 * </description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Instance successfully created</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_OBJECT">Instance could not be created</errorcode>
 * <result>A handle to the newly created PLCHandler instance</result>
 */
RTS_HANDLE CDECL PLCHandlerCreateInstance(RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFPLCHANDLERCREATEINSTANCE) (RTS_RESULT *pResult);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERCREATEINSTANCE_NOTIMPLEMENTED)
	#define USE_PLCHandlerCreateInstance
	#define EXT_PLCHandlerCreateInstance
	#define GET_PLCHandlerCreateInstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_PLCHandlerCreateInstance(p0)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_PLCHandlerCreateInstance  FALSE
	#define EXP_PLCHandlerCreateInstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_PLCHandlerCreateInstance
	#define EXT_PLCHandlerCreateInstance
	#define GET_PLCHandlerCreateInstance(fl)  CAL_CMGETAPI( "PLCHandlerCreateInstance" ) 
	#define CAL_PLCHandlerCreateInstance  PLCHandlerCreateInstance
	#define CHK_PLCHandlerCreateInstance  TRUE
	#define EXP_PLCHandlerCreateInstance  CAL_CMEXPAPI( "PLCHandlerCreateInstance" ) 
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_PLCHandlerCreateInstance
	#define EXT_PLCHandlerCreateInstance
	#define GET_PLCHandlerCreateInstance(fl)  CAL_CMGETAPI( "PLCHandlerCreateInstance" ) 
	#define CAL_PLCHandlerCreateInstance  PLCHandlerCreateInstance
	#define CHK_PLCHandlerCreateInstance  TRUE
	#define EXP_PLCHandlerCreateInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"PLCHandlerCreateInstance", (RTS_UINTPTR)PLCHandlerCreateInstance, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerPLCHandlerCreateInstance
	#define EXT_CmpPLCHandlerPLCHandlerCreateInstance
	#define GET_CmpPLCHandlerPLCHandlerCreateInstance  ERR_OK
	#define CAL_CmpPLCHandlerPLCHandlerCreateInstance pICmpPLCHandler->IPLCHandlerCreateInstance
	#define CHK_CmpPLCHandlerPLCHandlerCreateInstance (pICmpPLCHandler != NULL)
	#define EXP_CmpPLCHandlerPLCHandlerCreateInstance  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_PLCHandlerCreateInstance
	#define EXT_PLCHandlerCreateInstance
	#define GET_PLCHandlerCreateInstance(fl)  CAL_CMGETAPI( "PLCHandlerCreateInstance" ) 
	#define CAL_PLCHandlerCreateInstance pICmpPLCHandler->IPLCHandlerCreateInstance
	#define CHK_PLCHandlerCreateInstance (pICmpPLCHandler != NULL)
	#define EXP_PLCHandlerCreateInstance  CAL_CMEXPAPI( "PLCHandlerCreateInstance" ) 
#else /* DYNAMIC_LINK */
	#define USE_PLCHandlerCreateInstance  PFPLCHANDLERCREATEINSTANCE pfPLCHandlerCreateInstance;
	#define EXT_PLCHandlerCreateInstance  extern PFPLCHANDLERCREATEINSTANCE pfPLCHandlerCreateInstance;
	#define GET_PLCHandlerCreateInstance(fl)  s_pfCMGetAPI2( "PLCHandlerCreateInstance", (RTS_VOID_FCTPTR *)&pfPLCHandlerCreateInstance, (fl), 0, 0)
	#define CAL_PLCHandlerCreateInstance  pfPLCHandlerCreateInstance
	#define CHK_PLCHandlerCreateInstance  (pfPLCHandlerCreateInstance != NULL)
	#define EXP_PLCHandlerCreateInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"PLCHandlerCreateInstance", (RTS_UINTPTR)PLCHandlerCreateInstance, 0, 0) 
#endif




/**
 * <description>Function to delete an instance of the PLCHandler that was previously created by PLCHandlerCreateInstance.</description>
 * <param name="hPlcHandler" type="IN">Handle to the PLCHandler instance to delete</param>
 * <result>Error code:
 *		ERR_OK: PLCHandler instance successfully deleted
 *		ERR_PARAMETER: Invalid parameter
 * </result>
 */
RTS_RESULT CDECL PLCHandlerDeleteInstance(RTS_HANDLE hPlcHandler);
typedef RTS_RESULT (CDECL * PFPLCHANDLERDELETEINSTANCE) (RTS_HANDLE hPlcHandler);
#if defined(CMPPLCHANDLER_NOTIMPLEMENTED) || defined(PLCHANDLERDELETEINSTANCE_NOTIMPLEMENTED)
	#define USE_PLCHandlerDeleteInstance
	#define EXT_PLCHandlerDeleteInstance
	#define GET_PLCHandlerDeleteInstance(fl)  ERR_NOTIMPLEMENTED
	#define CAL_PLCHandlerDeleteInstance(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_PLCHandlerDeleteInstance  FALSE
	#define EXP_PLCHandlerDeleteInstance  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_PLCHandlerDeleteInstance
	#define EXT_PLCHandlerDeleteInstance
	#define GET_PLCHandlerDeleteInstance(fl)  CAL_CMGETAPI( "PLCHandlerDeleteInstance" ) 
	#define CAL_PLCHandlerDeleteInstance  PLCHandlerDeleteInstance
	#define CHK_PLCHandlerDeleteInstance  TRUE
	#define EXP_PLCHandlerDeleteInstance  CAL_CMEXPAPI( "PLCHandlerDeleteInstance" ) 
#elif defined(MIXED_LINK) && !defined(CMPPLCHANDLER_EXTERNAL)
	#define USE_PLCHandlerDeleteInstance
	#define EXT_PLCHandlerDeleteInstance
	#define GET_PLCHandlerDeleteInstance(fl)  CAL_CMGETAPI( "PLCHandlerDeleteInstance" ) 
	#define CAL_PLCHandlerDeleteInstance  PLCHandlerDeleteInstance
	#define CHK_PLCHandlerDeleteInstance  TRUE
	#define EXP_PLCHandlerDeleteInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"PLCHandlerDeleteInstance", (RTS_UINTPTR)PLCHandlerDeleteInstance, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpPLCHandlerPLCHandlerDeleteInstance
	#define EXT_CmpPLCHandlerPLCHandlerDeleteInstance
	#define GET_CmpPLCHandlerPLCHandlerDeleteInstance  ERR_OK
	#define CAL_CmpPLCHandlerPLCHandlerDeleteInstance pICmpPLCHandler->IPLCHandlerDeleteInstance
	#define CHK_CmpPLCHandlerPLCHandlerDeleteInstance (pICmpPLCHandler != NULL)
	#define EXP_CmpPLCHandlerPLCHandlerDeleteInstance  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_PLCHandlerDeleteInstance
	#define EXT_PLCHandlerDeleteInstance
	#define GET_PLCHandlerDeleteInstance(fl)  CAL_CMGETAPI( "PLCHandlerDeleteInstance" ) 
	#define CAL_PLCHandlerDeleteInstance pICmpPLCHandler->IPLCHandlerDeleteInstance
	#define CHK_PLCHandlerDeleteInstance (pICmpPLCHandler != NULL)
	#define EXP_PLCHandlerDeleteInstance  CAL_CMEXPAPI( "PLCHandlerDeleteInstance" ) 
#else /* DYNAMIC_LINK */
	#define USE_PLCHandlerDeleteInstance  PFPLCHANDLERDELETEINSTANCE pfPLCHandlerDeleteInstance;
	#define EXT_PLCHandlerDeleteInstance  extern PFPLCHANDLERDELETEINSTANCE pfPLCHandlerDeleteInstance;
	#define GET_PLCHandlerDeleteInstance(fl)  s_pfCMGetAPI2( "PLCHandlerDeleteInstance", (RTS_VOID_FCTPTR *)&pfPLCHandlerDeleteInstance, (fl), 0, 0)
	#define CAL_PLCHandlerDeleteInstance  pfPLCHandlerDeleteInstance
	#define CHK_PLCHandlerDeleteInstance  (pfPLCHandlerDeleteInstance != NULL)
	#define EXP_PLCHandlerDeleteInstance  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"PLCHandlerDeleteInstance", (RTS_UINTPTR)PLCHandlerDeleteInstance, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFPLCHANDLERCREATEINSTANCE IPLCHandlerCreateInstance;
 	PFPLCHANDLERDELETEINSTANCE IPLCHandlerDeleteInstance;
 } ICmpPLCHandler_C;

#ifdef CPLUSPLUS
class ICmpPLCHandler : public IBase
{
	public:
		virtual RTS_HANDLE CDECL IPLCHandlerCreateInstance(RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IPLCHandlerDeleteInstance(RTS_HANDLE hPlcHandler) =0;
};
	#ifndef ITF_CmpPLCHandler
		#define ITF_CmpPLCHandler static ICmpPLCHandler *pICmpPLCHandler = NULL;
	#endif
	#define EXTITF_CmpPLCHandler
#else	/*CPLUSPLUS*/
	typedef ICmpPLCHandler_C		ICmpPLCHandler;
	#ifndef ITF_CmpPLCHandler
		#define ITF_CmpPLCHandler
	#endif
	#define EXTITF_CmpPLCHandler
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPPLCHANDLERITF_H_*/
