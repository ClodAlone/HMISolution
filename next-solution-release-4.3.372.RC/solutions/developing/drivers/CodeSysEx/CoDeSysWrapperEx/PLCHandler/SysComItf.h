 /**
 * <interfacename>SysCom</interfacename>
 * <description> 
 *	<p>The SysCom interface is projected to connect to a serial COM port (RS232) and 
 *	to send and receive data via this port.</p>
 *	<p>IMPLEMENTATION NODE: All routines must be realized asynchronous! You have to use the FIFO of
 *	the serial device! Don't block the read and write routines, until the operations are finished.</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _SYSCOMITF_H_
#define _SYSCOMITF_H_

#include "CmpStd.h"

 

 




#include "CmpStd.h"

#ifndef CDECL
	#define CDECL
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * Allows to specify a serial port by its Microsoft Windows friendly name instead of the COM port number.
 * The friendly name of the COM port is for example shown by the Microsoft Windows Device Manager. 
 * This feature is useful, if the port number is somehow dynamic, but if the adapter has a well defined 
 * friendly name. Typically it's used for USB devices, which provide a virtual COM port driver. If the 
 * (by SysComOpen) requested COM port number is found in the list of the configured "Windows.PortToCheck" 
 * settings, then the port is opened, which matches to the name specified by "Windows.CheckWithPortName",
 * instead of the requested port number. 
 * (Only for Windows).
 * Example:
 * Windows.PortToCheck.0=99
 * Windows.CheckWithPortName.0=My USB serial device
 * Windows.PortToCheck.1=98
 * Windows.CheckWithPortName.1=PLC USB interface
 * </description>
 */
#define SYSCOMKEY_INT_WINDOWS_PORT_TO_CHECK						"Windows.PortToCheck"
#define SYSTIMERVALUE_INT_WINDOWS_PORT_TO_CHECK_DEFAULT	        0

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 * Specifies a string, which is compared to the Microsoft Windows friendly name of the available COM ports.
 * The name matches, if it starts with the provided string.  
 * See also setting PortToCheck.
 * (Only for Windows).
 * </description>
 */
#define SYSCOMKEY_STRING_WINDOWS_PORTNAME_CHECK_WITH						"Windows.CheckWithPortname"
#define SYSTIMERVALUE_STRING_WINDOWS_PORTNAME_CHECK_WITH_DEFAULT	        ""

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 * Device file for serial interface (Only for Linux).
 * may also be indexed. for example: Linux.Devicefile.7=/dev/usb0 maps /dev/usb0 to COM7
 * </description>
 */
#define SYSCOMKEY_STRING_LINUX_DEVICEFILE			"Linux.Devicefile"
#define SYSCOMKEY_STRING_LINUX_DEVICEFILE_DEFAULT	"/dev/ttyS"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * Activate exclusive access to the device file (Only for Linux).
 * </description>
 */
#define SYSCOMKEY_INT_LINUX_EXCLUSIVE_ACCESS			"Linux.ExclusiveAccess"
#define SYSCOMKEY_INT_LINUX_EXCLUSIVE_ACCESS_DEFAULT	0


/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Physical layer of the serial interface. Valid values are "rs232" and "rs485".
 * (Only for Linux).
 * </description>
 */
#define SYSCOMKEY_STRING_LINUX_PHYSICAL_LAYER						"Linux.PhysicalLayer"
#define SYSCOMKEY_STRING_LINUX_PHYSICAL_LAYER_DEFAULT	        	"rs232"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 * on_send: RTS pin is high on send, and low all other times. 
 * after_send: RTS pin is low on send. 
 * (Only for Linux).
 * </description>
 */
#define SYSCOMKEY_STRING_LINUX_RS485_RTS_PIN						"Linux.rs485.RTSPin"
#define SYSCOMKEY_STRING_LINUX_RS485_RTS_PIN_DEFAULT	        	"on_send"

/**
 * <category>Event parameter</category>
 * <element name="device" type="IN">Name of serial device</element>
 */
typedef struct
{
	unsigned char* name;
} EVTPARAM_SysComOpen;
#define EVTPARAMID_SysComOpen 0x0001
#define EVTVERSION_SysComOpen 0x0001

/**
 * <category>Events</category>
 * <description>Platform dependent event. Sent before the physical driver open.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysComOpenBefore</param>
 */
#define EVT_SysComOpenBefore MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Events</category>
 * <description>Platform dependent event. Sent directly after the physical driver open.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysComOpenAfter</param>
 */
#define EVT_SysComOpenAfter MAKE_EVENTID(EVTCLASS_INFO, 2)


/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <category>Baudrates</category>
 * <description>: Supported baudrates</description>
 */
#define SYS_BR_4800    RTS_IEC_DWORD_C(0x12C0)	
#define SYS_BR_9600    RTS_IEC_DWORD_C(0x2580)	
#define SYS_BR_19200    RTS_IEC_DWORD_C(0x4B00)	
#define SYS_BR_38400    RTS_IEC_DWORD_C(0x9600)	
#define SYS_BR_57600    RTS_IEC_DWORD_C(0xE100)	
#define SYS_BR_115200    RTS_IEC_DWORD_C(0x1C200)	
/* Typed enum definition */
#define SYS_COM_BAUDRATE    RTS_IEC_DWORD

/**
 * <category>DTR control</category>
 * <description>: DTR control configuration</description>
 */
#define SYS_DTR_CONTROL_DISABLE    RTS_IEC_BYTE_C(0x0)	/* <element name="SYS_DTR_CONTROL_DISABLE" TYPE="IN">Disables the DTR line when the device is opened and leaves it disabled.</element> */
#define SYS_DTR_CONTROL_ENABLE    RTS_IEC_BYTE_C(0x1)	/* <element name="SYS_DTR_CONTROL_ENABLE" TYPE="IN">Enables the DTR line when the device is opened and leaves it on.</element> */
#define SYS_DTR_CONTROL_HANDSHAKE    RTS_IEC_BYTE_C(0x2)	/* <element name="SYS_DTR_CONTROL_HANDSHAKE" TYPE="IN">Enables DTR handshaking.</element> */
/* Typed enumeration definition */
#define SYS_COM_DTR_CONTROL    RTS_IEC_BYTE

/**
 * <category>Parity</category>
 * <description>: Supported parity</description>
 */
#define SYS_NOPARITY    RTS_IEC_BYTE_C(0x0)	/* <element name="SYS_NOPARITY" TYPE="IN">No parity</element> */
#define SYS_ODDPARITY    RTS_IEC_BYTE_C(0x1)	/* <element name="SYS_ODDPARITY" TYPE="IN">Odd parity</element> */
#define SYS_EVENPARITY    RTS_IEC_BYTE_C(0x2)	/* <element name="SYS_EVENPARITY" TYPE="IN">Even parity</element> */
/* Typed enumeration definition */
#define SYS_COM_PARITY    RTS_IEC_BYTE

/**
 * <category>Com ports</category>
 * <description>: Supported com port numbers</description>
 */
#define SYS_COMPORT_NONE    RTS_IEC_INT_C(0x0)	
#define SYS_COMPORT1    RTS_IEC_INT_C(0x1)	
#define SYS_COMPORT2    RTS_IEC_INT_C(0x2)	
#define SYS_COMPORT3    RTS_IEC_INT_C(0x3)	
#define SYS_COMPORT4    RTS_IEC_INT_C(0x4)	
/* Typed enumeration definition */
#define SYS_COM_PORTS    RTS_IEC_INT

/**
 * <category>RTS control</category>
 * <description>: RTS control configuration</description>
 */
#define SYS_RTS_CONTROL_DISABLE    RTS_IEC_BYTE_C(0x0)	/* <element name="SYS_RTS_CONTROL_DISABLE" TYPE="IN">Disables the RTS line when the device is opened and leaves it disabled.</element> */
#define SYS_RTS_CONTROL_ENABLE    RTS_IEC_BYTE_C(0x1)	/* <element name="SYS_RTS_CONTROL_ENABLE" TYPE="IN">Enables the RTS line when the device is opened and leaves it on.</element> */
#define SYS_RTS_CONTROL_HANDSHAKE    RTS_IEC_BYTE_C(0x2)	/* <element name="SYS_RTS_CONTROL_HANDSHAKE" TYPE="IN">Enables RTS handshaking.</element> */
#define SYS_RTS_CONTROL_TOGGLE    RTS_IEC_BYTE_C(0x3)	/* <element name="SYS_RTS_CONTROL_TOGGLE" TYPE="IN">Specifies that the RTS line will be high if bytes are available for transmission. After all buffered bytes have been sent, the RTS line will be low.</element> */
/* Typed enumeration definition */
#define SYS_COM_RTS_CONTROL    RTS_IEC_BYTE

/**
 * <category>Stop bits</category>
 * <description>: Supported stop bits</description>
 */
#define SYS_ONESTOPBIT    RTS_IEC_BYTE_C(0x1)	
#define SYS_ONE5STOPBITS    RTS_IEC_BYTE_C(0x2)	
#define SYS_TWOSTOPBITS    RTS_IEC_BYTE_C(0x3)	
/* Typed enumeration definition */
#define SYS_COM_STOPBITS    RTS_IEC_BYTE

/**
 * <category>Timeouts</category>
 * <description>: Specified standard timeouts</description>
 */
#define SYS_NOWAIT    RTS_IEC_UDINT_C(0x0)	
#define SYS_INFINITE    RTS_IEC_UDINT_C(0xFFFFFFFF)	
/* Typed enumeration definition */
#define SYS_COM_TIMEOUT    RTS_IEC_UDINT

/**
 * <category>Com port settings</category>
 * <description>: Settings to open and to configure a COM interface</description>
 */
typedef struct tagSysComSettings
{
	RTS_IEC_INT sPort;		/* <element name="sPort" type="IN">Port number: 0=disabled, 1=COM1, 2=COM2, .... See SYS_COM_PORTS for predefined values.</element> */
	RTS_IEC_BYTE byStopBits;		/* <element name="byStopBits" type="IN">Stop bits. See SYS_COM_STOPBITS for details.</element> */
	RTS_IEC_BYTE byParity;		/* <element name="byParity" type="IN">Parity. See SYS_COM_PARITY for details.</element> */
	RTS_IEC_DWORD ulBaudrate;		/* <element name="ulBaudRate" type="IN">Baudrate. See SYS_COM_BAUDRATE for details.</element> */
	RTS_IEC_UDINT ulTimeout;		/* <element name="ulTimeout" TYPE="IN">Hardware timeout. The Timeout is the TIME between two received OR sent
	 	characters until the read or write operation will return.
		Typically this value should be 0 (returns immediately)
		See SYS_COM_TIMEOUT for predefined values.
 </element> */
	RTS_IEC_UDINT ulBufferSize;		/* <element name="ulBufferSize" type="IN">Buffer size of FIFO buffer</element> */
} SysComSettings;

/**
 * <category>Com port extended settings</category>
 * <description>: This structure can be used to get and set the settings of a COM port.
 *
 * NOTE:
 * The difference of this structure to SysComSettingsEx are the 2 members byDtrControl and byRtsControl! Here you can specify BYTE instead of BOOL values in SysComSettingsEx!  
 *	</description>
 */
typedef struct tagSysComSettingsEx
{
	RTS_IEC_BYTE byByteSize;		/* <element name="byByteSize" TYPE="IN">Number of bits/BYTE, 4-8</element> */
	RTS_IEC_BOOL bBinary;		/* <element name="bBinary" TYPE="IN">Binary mode, no EOF check</element> */
	RTS_IEC_BOOL bOutxCtsFlow;		/* <element name="bOutxCtsFlow" TYPE="IN">CTS handshaking on output</element> */
	RTS_IEC_BOOL bOutxDsrFlow;		/* <element name="bOutxDsrFlow" TYPE="IN">DSR handshaking on output</element> */
	RTS_IEC_BYTE byDtrControl;		/* <element name="byDtrControl" TYPE="IN">DTR (data-terminal-ready) flow control. See SYS_COM_DTR_CONTROL for possible values.</element> */
	RTS_IEC_BOOL bDsrSensitivity;		/* <element name="bDsrSensitivity" TYPE="IN">DSR Sensitivity</element> */
	RTS_IEC_BYTE byRtsControl;		/* <element name="byRtsControl" TYPE="IN">Rts (request-to-send) flow control. See SYS_COM_RTS_CONTROL for possible values.</element> */
	RTS_IEC_BOOL bTXContinueOnXoff;		/* <element name="bTXContinueOnXoff" TYPE="IN">XOFF continues Tx</element> */
	RTS_IEC_BOOL bOutX;		/* <element name="bOutX" TYPE="IN">XON/XOFF out flow control</element> */
	RTS_IEC_BOOL bInX;		/* <element name="bInX" TYPE="IN">XON/XOFF in flow control</element> */
	RTS_IEC_BYTE byXonChar;		/* <element name="XonChar" TYPE="IN">Tx AND Rx XON character</element> */
	RTS_IEC_BYTE byXoffChar;		/* <element name="XoffChar" TYPE="IN">Tx AND Rx XOFF character</element> */
	RTS_IEC_WORD wXonLim;		/* <element name="XonLim" TYPE="IN">transmit XON threshold</element> */
	RTS_IEC_WORD wXoffLim;		/* <element name="XoffLim" TYPE="IN">transmit XOFF threshold</element> */
} SysComSettingsEx;

/**
 * <description>Close a serial communication device.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsyscomclose_struct
{
	RTS_IEC_HANDLE hCom;				/* VAR_INPUT */	/* <param name="hCom" type="IN">Handle to com port to close</param> */
	RTS_IEC_RESULT SysComClose;			/* VAR_OUTPUT */	
} syscomclose_struct;

void CDECL CDECL_EXT syscomclose(syscomclose_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCOMCLOSE_IEC) (syscomclose_struct *p);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMCLOSE_NOTIMPLEMENTED)
	#define USE_syscomclose
	#define EXT_syscomclose
	#define GET_syscomclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscomclose(p0) 
	#define CHK_syscomclose  FALSE
	#define EXP_syscomclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscomclose
	#define EXT_syscomclose
	#define GET_syscomclose(fl)  CAL_CMGETAPI( "syscomclose" ) 
	#define CAL_syscomclose  syscomclose
	#define CHK_syscomclose  TRUE
	#define EXP_syscomclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomclose", (RTS_UINTPTR)syscomclose, 1, RTSITF_GET_SIGNATURE(0, 0xACD27C01), 0x03051100) 
	#define EXTREF_ITF_syscomclose {(RTS_VOID_FCTPTR)syscomclose, "syscomclose", RTSITF_GET_SIGNATURE(0, 0xACD27C01), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_syscomclose
	#define EXT_syscomclose
	#define GET_syscomclose(fl)  CAL_CMGETAPI( "syscomclose" ) 
	#define CAL_syscomclose  syscomclose
	#define CHK_syscomclose  TRUE
	#define EXP_syscomclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomclose", (RTS_UINTPTR)syscomclose, 1, RTSITF_GET_SIGNATURE(0, 0xACD27C01), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComsyscomclose
	#define EXT_SysComsyscomclose
	#define GET_SysComsyscomclose  ERR_OK
	#define CAL_SysComsyscomclose  syscomclose
	#define CHK_SysComsyscomclose  TRUE
	#define EXP_SysComsyscomclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomclose", (RTS_UINTPTR)syscomclose, 1, RTSITF_GET_SIGNATURE(0, 0xACD27C01), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syscomclose
	#define EXT_syscomclose
	#define GET_syscomclose(fl)  CAL_CMGETAPI( "syscomclose" ) 
	#define CAL_syscomclose  syscomclose
	#define CHK_syscomclose  TRUE
	#define EXP_syscomclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomclose", (RTS_UINTPTR)syscomclose, 1, RTSITF_GET_SIGNATURE(0, 0xACD27C01), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syscomclose  PFSYSCOMCLOSE_IEC pfsyscomclose;
	#define EXT_syscomclose  extern PFSYSCOMCLOSE_IEC pfsyscomclose;
	#define GET_syscomclose(fl)  s_pfCMGetAPI2( "syscomclose", (RTS_VOID_FCTPTR *)&pfsyscomclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xACD27C01), 0x03051100)
	#define CAL_syscomclose  pfsyscomclose
	#define CHK_syscomclose  (pfsyscomclose != NULL)
	#define EXP_syscomclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomclose", (RTS_UINTPTR)syscomclose, 1, RTSITF_GET_SIGNATURE(0, 0xACD27C01), 0x03051100) 
#endif


/**
 * <description>Get the parameter of the interface specified by handle.
 * Use SysComGetSettings2()! This function should only be used for backward compatibility!
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsyscomgetsettings_struct
{
	RTS_IEC_HANDLE hCom;				/* VAR_INPUT */	/* <param name="hCom" type="IN">Handle to com port</param> */
	SysComSettings *pSettings;			/* VAR_INPUT */	/* <param name="pSettings" type="IN">Pointer to get settings</param> */
	SysComSettingsEx *pSettingsEx;		/* VAR_INPUT */	/* <param name="pSettingsEx" type="IN">Pointer to get extended settings</param> */
	RTS_IEC_RESULT SysComGetSettings;	/* VAR_OUTPUT */	
} syscomgetsettings_struct;

void CDECL CDECL_EXT syscomgetsettings(syscomgetsettings_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCOMGETSETTINGS_IEC) (syscomgetsettings_struct *p);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMGETSETTINGS_NOTIMPLEMENTED)
	#define USE_syscomgetsettings
	#define EXT_syscomgetsettings
	#define GET_syscomgetsettings(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscomgetsettings(p0) 
	#define CHK_syscomgetsettings  FALSE
	#define EXP_syscomgetsettings  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscomgetsettings
	#define EXT_syscomgetsettings
	#define GET_syscomgetsettings(fl)  CAL_CMGETAPI( "syscomgetsettings" ) 
	#define CAL_syscomgetsettings  syscomgetsettings
	#define CHK_syscomgetsettings  TRUE
	#define EXP_syscomgetsettings  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomgetsettings", (RTS_UINTPTR)syscomgetsettings, 1, 0, 0x03051100) 
	#define EXTREF_ITF_syscomgetsettings {(RTS_VOID_FCTPTR)syscomgetsettings, "syscomgetsettings", 0, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_syscomgetsettings
	#define EXT_syscomgetsettings
	#define GET_syscomgetsettings(fl)  CAL_CMGETAPI( "syscomgetsettings" ) 
	#define CAL_syscomgetsettings  syscomgetsettings
	#define CHK_syscomgetsettings  TRUE
	#define EXP_syscomgetsettings  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomgetsettings", (RTS_UINTPTR)syscomgetsettings, 1, 0, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComsyscomgetsettings
	#define EXT_SysComsyscomgetsettings
	#define GET_SysComsyscomgetsettings  ERR_OK
	#define CAL_SysComsyscomgetsettings  syscomgetsettings
	#define CHK_SysComsyscomgetsettings  TRUE
	#define EXP_SysComsyscomgetsettings  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomgetsettings", (RTS_UINTPTR)syscomgetsettings, 1, 0, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syscomgetsettings
	#define EXT_syscomgetsettings
	#define GET_syscomgetsettings(fl)  CAL_CMGETAPI( "syscomgetsettings" ) 
	#define CAL_syscomgetsettings  syscomgetsettings
	#define CHK_syscomgetsettings  TRUE
	#define EXP_syscomgetsettings  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomgetsettings", (RTS_UINTPTR)syscomgetsettings, 1, 0, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syscomgetsettings  PFSYSCOMGETSETTINGS_IEC pfsyscomgetsettings;
	#define EXT_syscomgetsettings  extern PFSYSCOMGETSETTINGS_IEC pfsyscomgetsettings;
	#define GET_syscomgetsettings(fl)  s_pfCMGetAPI2( "syscomgetsettings", (RTS_VOID_FCTPTR *)&pfsyscomgetsettings, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0, 0x03051100)
	#define CAL_syscomgetsettings  pfsyscomgetsettings
	#define CHK_syscomgetsettings  (pfsyscomgetsettings != NULL)
	#define EXP_syscomgetsettings   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomgetsettings", (RTS_UINTPTR)syscomgetsettings, 1, 0, 0x03051100) 
#endif


/**
 * <description>Open a serial communication device.</description>
 * <result><p>RESULT: Handle to the opened com port or RTS_INVALID_HANDLE if failed</p></result>
 */
typedef struct tagsyscomopen_struct
{
	RTS_IEC_INT sPort;					/* VAR_INPUT, Enum: SYS_COM_PORTS */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
	RTS_IEC_HANDLE SysComOpen;			/* VAR_OUTPUT */	
} syscomopen_struct;

void CDECL CDECL_EXT syscomopen(syscomopen_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCOMOPEN_IEC) (syscomopen_struct *p);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMOPEN_NOTIMPLEMENTED)
	#define USE_syscomopen
	#define EXT_syscomopen
	#define GET_syscomopen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscomopen(p0) 
	#define CHK_syscomopen  FALSE
	#define EXP_syscomopen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscomopen
	#define EXT_syscomopen
	#define GET_syscomopen(fl)  CAL_CMGETAPI( "syscomopen" ) 
	#define CAL_syscomopen  syscomopen
	#define CHK_syscomopen  TRUE
	#define EXP_syscomopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen", (RTS_UINTPTR)syscomopen, 1, RTSITF_GET_SIGNATURE(0, 0x1CE3B492), 0x03051100) 
	#define EXTREF_ITF_syscomopen {(RTS_VOID_FCTPTR)syscomopen, "syscomopen", RTSITF_GET_SIGNATURE(0, 0x1CE3B492), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_syscomopen
	#define EXT_syscomopen
	#define GET_syscomopen(fl)  CAL_CMGETAPI( "syscomopen" ) 
	#define CAL_syscomopen  syscomopen
	#define CHK_syscomopen  TRUE
	#define EXP_syscomopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen", (RTS_UINTPTR)syscomopen, 1, RTSITF_GET_SIGNATURE(0, 0x1CE3B492), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComsyscomopen
	#define EXT_SysComsyscomopen
	#define GET_SysComsyscomopen  ERR_OK
	#define CAL_SysComsyscomopen  syscomopen
	#define CHK_SysComsyscomopen  TRUE
	#define EXP_SysComsyscomopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen", (RTS_UINTPTR)syscomopen, 1, RTSITF_GET_SIGNATURE(0, 0x1CE3B492), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syscomopen
	#define EXT_syscomopen
	#define GET_syscomopen(fl)  CAL_CMGETAPI( "syscomopen" ) 
	#define CAL_syscomopen  syscomopen
	#define CHK_syscomopen  TRUE
	#define EXP_syscomopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen", (RTS_UINTPTR)syscomopen, 1, RTSITF_GET_SIGNATURE(0, 0x1CE3B492), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syscomopen  PFSYSCOMOPEN_IEC pfsyscomopen;
	#define EXT_syscomopen  extern PFSYSCOMOPEN_IEC pfsyscomopen;
	#define GET_syscomopen(fl)  s_pfCMGetAPI2( "syscomopen", (RTS_VOID_FCTPTR *)&pfsyscomopen, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x1CE3B492), 0x03051100)
	#define CAL_syscomopen  pfsyscomopen
	#define CHK_syscomopen  (pfsyscomopen != NULL)
	#define EXP_syscomopen   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen", (RTS_UINTPTR)syscomopen, 1, RTSITF_GET_SIGNATURE(0, 0x1CE3B492), 0x03051100) 
#endif


/**
 * <description>Opens a serial communication device specified by settings</description>
 * <result><p>RESULT: Handle to the opened com port or RTS_INVALID_HANDLE if failed</p></result>
 */
typedef struct tagsyscomopen2_struct
{
	SysComSettings *pSettings;			/* VAR_INPUT */	/* <param name="pSettings" type="IN">Settings for the communication device.
	See category "Com port settings" for detailed information</param> */
	SysComSettingsEx *pSettingsEx;		/* VAR_INPUT */	/* <param name="pSettingsEx" type="IN">Optional extended settings for the serial device. Can be 0.</param> */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
	RTS_IEC_HANDLE SysComOpen2;			/* VAR_OUTPUT */	
} syscomopen2_struct;

void CDECL CDECL_EXT syscomopen2(syscomopen2_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCOMOPEN2_IEC) (syscomopen2_struct *p);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMOPEN2_NOTIMPLEMENTED)
	#define USE_syscomopen2
	#define EXT_syscomopen2
	#define GET_syscomopen2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscomopen2(p0) 
	#define CHK_syscomopen2  FALSE
	#define EXP_syscomopen2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscomopen2
	#define EXT_syscomopen2
	#define GET_syscomopen2(fl)  CAL_CMGETAPI( "syscomopen2" ) 
	#define CAL_syscomopen2  syscomopen2
	#define CHK_syscomopen2  TRUE
	#define EXP_syscomopen2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen2", (RTS_UINTPTR)syscomopen2, 1, 0, 0x03051100) 
	#define EXTREF_ITF_syscomopen2 {(RTS_VOID_FCTPTR)syscomopen2, "syscomopen2", 0, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_syscomopen2
	#define EXT_syscomopen2
	#define GET_syscomopen2(fl)  CAL_CMGETAPI( "syscomopen2" ) 
	#define CAL_syscomopen2  syscomopen2
	#define CHK_syscomopen2  TRUE
	#define EXP_syscomopen2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen2", (RTS_UINTPTR)syscomopen2, 1, 0, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComsyscomopen2
	#define EXT_SysComsyscomopen2
	#define GET_SysComsyscomopen2  ERR_OK
	#define CAL_SysComsyscomopen2  syscomopen2
	#define CHK_SysComsyscomopen2  TRUE
	#define EXP_SysComsyscomopen2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen2", (RTS_UINTPTR)syscomopen2, 1, 0, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syscomopen2
	#define EXT_syscomopen2
	#define GET_syscomopen2(fl)  CAL_CMGETAPI( "syscomopen2" ) 
	#define CAL_syscomopen2  syscomopen2
	#define CHK_syscomopen2  TRUE
	#define EXP_syscomopen2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen2", (RTS_UINTPTR)syscomopen2, 1, 0, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syscomopen2  PFSYSCOMOPEN2_IEC pfsyscomopen2;
	#define EXT_syscomopen2  extern PFSYSCOMOPEN2_IEC pfsyscomopen2;
	#define GET_syscomopen2(fl)  s_pfCMGetAPI2( "syscomopen2", (RTS_VOID_FCTPTR *)&pfsyscomopen2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0, 0x03051100)
	#define CAL_syscomopen2  pfsyscomopen2
	#define CHK_syscomopen2  (pfsyscomopen2 != NULL)
	#define EXP_syscomopen2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomopen2", (RTS_UINTPTR)syscomopen2, 1, 0, 0x03051100) 
#endif


/**
 * <description>Clear the fifo buffer of the serial interface</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsyscompurge_struct
{
	RTS_IEC_HANDLE hCom;				/* VAR_INPUT */	/* <param name="hCom" type="IN">Handle to com port</param> */
	RTS_IEC_RESULT SysComPurge;			/* VAR_OUTPUT */	
} syscompurge_struct;

void CDECL CDECL_EXT syscompurge(syscompurge_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCOMPURGE_IEC) (syscompurge_struct *p);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMPURGE_NOTIMPLEMENTED)
	#define USE_syscompurge
	#define EXT_syscompurge
	#define GET_syscompurge(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscompurge(p0) 
	#define CHK_syscompurge  FALSE
	#define EXP_syscompurge  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscompurge
	#define EXT_syscompurge
	#define GET_syscompurge(fl)  CAL_CMGETAPI( "syscompurge" ) 
	#define CAL_syscompurge  syscompurge
	#define CHK_syscompurge  TRUE
	#define EXP_syscompurge  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscompurge", (RTS_UINTPTR)syscompurge, 1, RTSITF_GET_SIGNATURE(0, 0xC2A013BE), 0x03051100) 
	#define EXTREF_ITF_syscompurge {(RTS_VOID_FCTPTR)syscompurge, "syscompurge", RTSITF_GET_SIGNATURE(0, 0xC2A013BE), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_syscompurge
	#define EXT_syscompurge
	#define GET_syscompurge(fl)  CAL_CMGETAPI( "syscompurge" ) 
	#define CAL_syscompurge  syscompurge
	#define CHK_syscompurge  TRUE
	#define EXP_syscompurge  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscompurge", (RTS_UINTPTR)syscompurge, 1, RTSITF_GET_SIGNATURE(0, 0xC2A013BE), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComsyscompurge
	#define EXT_SysComsyscompurge
	#define GET_SysComsyscompurge  ERR_OK
	#define CAL_SysComsyscompurge  syscompurge
	#define CHK_SysComsyscompurge  TRUE
	#define EXP_SysComsyscompurge  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscompurge", (RTS_UINTPTR)syscompurge, 1, RTSITF_GET_SIGNATURE(0, 0xC2A013BE), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syscompurge
	#define EXT_syscompurge
	#define GET_syscompurge(fl)  CAL_CMGETAPI( "syscompurge" ) 
	#define CAL_syscompurge  syscompurge
	#define CHK_syscompurge  TRUE
	#define EXP_syscompurge  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscompurge", (RTS_UINTPTR)syscompurge, 1, RTSITF_GET_SIGNATURE(0, 0xC2A013BE), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syscompurge  PFSYSCOMPURGE_IEC pfsyscompurge;
	#define EXT_syscompurge  extern PFSYSCOMPURGE_IEC pfsyscompurge;
	#define GET_syscompurge(fl)  s_pfCMGetAPI2( "syscompurge", (RTS_VOID_FCTPTR *)&pfsyscompurge, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC2A013BE), 0x03051100)
	#define CAL_syscompurge  pfsyscompurge
	#define CHK_syscompurge  (pfsyscompurge != NULL)
	#define EXP_syscompurge   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscompurge", (RTS_UINTPTR)syscompurge, 1, RTSITF_GET_SIGNATURE(0, 0xC2A013BE), 0x03051100) 
#endif


/**
 * <description>Reads the number of bytes from the receive buffer of the specified device.
 * <p>IMPLEMENTATION NOTE:
 * If the timeout elapsed until the requested number of bytes are received,
 *	the function returns the number of bytes already received! This must be considered in the caller and
 *	the implementation!</p></description>
 * <result><p>RESULT: Number of actual read bytes</p></result>
 */
typedef struct tagsyscomread_struct
{
	RTS_IEC_HANDLE hCom;				/* VAR_INPUT */	/* <param name="hCom" type="IN">Handle to com port</param> */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* <param name="pbyBuffer" type="IN">Pointer to data buffer for received data</param> */
	RTS_IEC_UDINT ulSize;				/* VAR_INPUT */	/* <param name="uiSize" type="IN">Requested number of bytes to read. Must be less or equal the size
	of the receive buffer!</param> */
	RTS_IEC_UDINT ulTimeout;			/* VAR_INPUT, Enum: SYS_COM_TIMEOUT */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* <param name="pResult" type="OUT">Pointer to return the runtime system error code (see CmpErrors.library).</param> */
	RTS_IEC_UDINT SysComRead;			/* VAR_OUTPUT */	
} syscomread_struct;

void CDECL CDECL_EXT syscomread(syscomread_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCOMREAD_IEC) (syscomread_struct *p);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMREAD_NOTIMPLEMENTED)
	#define USE_syscomread
	#define EXT_syscomread
	#define GET_syscomread(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscomread(p0) 
	#define CHK_syscomread  FALSE
	#define EXP_syscomread  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscomread
	#define EXT_syscomread
	#define GET_syscomread(fl)  CAL_CMGETAPI( "syscomread" ) 
	#define CAL_syscomread  syscomread
	#define CHK_syscomread  TRUE
	#define EXP_syscomread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomread", (RTS_UINTPTR)syscomread, 1, RTSITF_GET_SIGNATURE(0, 0xDEDB5081), 0x03051100) 
	#define EXTREF_ITF_syscomread {(RTS_VOID_FCTPTR)syscomread, "syscomread", RTSITF_GET_SIGNATURE(0, 0xDEDB5081), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_syscomread
	#define EXT_syscomread
	#define GET_syscomread(fl)  CAL_CMGETAPI( "syscomread" ) 
	#define CAL_syscomread  syscomread
	#define CHK_syscomread  TRUE
	#define EXP_syscomread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomread", (RTS_UINTPTR)syscomread, 1, RTSITF_GET_SIGNATURE(0, 0xDEDB5081), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComsyscomread
	#define EXT_SysComsyscomread
	#define GET_SysComsyscomread  ERR_OK
	#define CAL_SysComsyscomread  syscomread
	#define CHK_SysComsyscomread  TRUE
	#define EXP_SysComsyscomread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomread", (RTS_UINTPTR)syscomread, 1, RTSITF_GET_SIGNATURE(0, 0xDEDB5081), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syscomread
	#define EXT_syscomread
	#define GET_syscomread(fl)  CAL_CMGETAPI( "syscomread" ) 
	#define CAL_syscomread  syscomread
	#define CHK_syscomread  TRUE
	#define EXP_syscomread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomread", (RTS_UINTPTR)syscomread, 1, RTSITF_GET_SIGNATURE(0, 0xDEDB5081), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syscomread  PFSYSCOMREAD_IEC pfsyscomread;
	#define EXT_syscomread  extern PFSYSCOMREAD_IEC pfsyscomread;
	#define GET_syscomread(fl)  s_pfCMGetAPI2( "syscomread", (RTS_VOID_FCTPTR *)&pfsyscomread, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xDEDB5081), 0x03051100)
	#define CAL_syscomread  pfsyscomread
	#define CHK_syscomread  (pfsyscomread != NULL)
	#define EXP_syscomread   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomread", (RTS_UINTPTR)syscomread, 1, RTSITF_GET_SIGNATURE(0, 0xDEDB5081), 0x03051100) 
#endif


/**
 * <description>Set the parameter of the interface specified by handle.
 * Use SysComSetSettings2()! This function should only be used for backward compatibility!
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsyscomsetsettings_struct
{
	RTS_IEC_HANDLE hCom;				/* VAR_INPUT */	/* <param name="hCom" type="IN">Handle to com port</param> */
	SysComSettings *pSettings;			/* VAR_INPUT */	/* <param name="pSettings" type="IN">Pointer to new settings</param> */
	SysComSettingsEx *pSettingsEx;		/* VAR_INPUT */	/* <param name="pSettingsEx" type="IN">OPTIONAL: Pointer to extended settings or 0</param> */
	RTS_IEC_RESULT SysComSetSettings;	/* VAR_OUTPUT */	
} syscomsetsettings_struct;

void CDECL CDECL_EXT syscomsetsettings(syscomsetsettings_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCOMSETSETTINGS_IEC) (syscomsetsettings_struct *p);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMSETSETTINGS_NOTIMPLEMENTED)
	#define USE_syscomsetsettings
	#define EXT_syscomsetsettings
	#define GET_syscomsetsettings(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscomsetsettings(p0) 
	#define CHK_syscomsetsettings  FALSE
	#define EXP_syscomsetsettings  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscomsetsettings
	#define EXT_syscomsetsettings
	#define GET_syscomsetsettings(fl)  CAL_CMGETAPI( "syscomsetsettings" ) 
	#define CAL_syscomsetsettings  syscomsetsettings
	#define CHK_syscomsetsettings  TRUE
	#define EXP_syscomsetsettings  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsetsettings", (RTS_UINTPTR)syscomsetsettings, 1, 0, 0x03051100) 
	#define EXTREF_ITF_syscomsetsettings {(RTS_VOID_FCTPTR)syscomsetsettings, "syscomsetsettings", 0, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_syscomsetsettings
	#define EXT_syscomsetsettings
	#define GET_syscomsetsettings(fl)  CAL_CMGETAPI( "syscomsetsettings" ) 
	#define CAL_syscomsetsettings  syscomsetsettings
	#define CHK_syscomsetsettings  TRUE
	#define EXP_syscomsetsettings  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsetsettings", (RTS_UINTPTR)syscomsetsettings, 1, 0, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComsyscomsetsettings
	#define EXT_SysComsyscomsetsettings
	#define GET_SysComsyscomsetsettings  ERR_OK
	#define CAL_SysComsyscomsetsettings  syscomsetsettings
	#define CHK_SysComsyscomsetsettings  TRUE
	#define EXP_SysComsyscomsetsettings  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsetsettings", (RTS_UINTPTR)syscomsetsettings, 1, 0, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syscomsetsettings
	#define EXT_syscomsetsettings
	#define GET_syscomsetsettings(fl)  CAL_CMGETAPI( "syscomsetsettings" ) 
	#define CAL_syscomsetsettings  syscomsetsettings
	#define CHK_syscomsetsettings  TRUE
	#define EXP_syscomsetsettings  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsetsettings", (RTS_UINTPTR)syscomsetsettings, 1, 0, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syscomsetsettings  PFSYSCOMSETSETTINGS_IEC pfsyscomsetsettings;
	#define EXT_syscomsetsettings  extern PFSYSCOMSETSETTINGS_IEC pfsyscomsetsettings;
	#define GET_syscomsetsettings(fl)  s_pfCMGetAPI2( "syscomsetsettings", (RTS_VOID_FCTPTR *)&pfsyscomsetsettings, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0, 0x03051100)
	#define CAL_syscomsetsettings  pfsyscomsetsettings
	#define CHK_syscomsetsettings  (pfsyscomsetsettings != NULL)
	#define EXP_syscomsetsettings   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsetsettings", (RTS_UINTPTR)syscomsetsettings, 1, 0, 0x03051100) 
#endif


/**
 * <description>Set the timeout of the specified serial interface (hardware timeout).
 *	The Timeout is the time between two received or sent characters until the read or write operation
 *	will return. Typically this value should be 0 (returns immediately)</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsyscomsettimeout_struct
{
	RTS_IEC_HANDLE hCom;				/* VAR_INPUT */	/* <param name="hCom" type="IN">Handle to com port</param> */
	RTS_IEC_UDINT ulTimeout;			/* VAR_INPUT, Enum: SYS_COM_TIMEOUT */
	RTS_IEC_RESULT SysComSetTimeout;	/* VAR_OUTPUT */	
} syscomsettimeout_struct;

void CDECL CDECL_EXT syscomsettimeout(syscomsettimeout_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCOMSETTIMEOUT_IEC) (syscomsettimeout_struct *p);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMSETTIMEOUT_NOTIMPLEMENTED)
	#define USE_syscomsettimeout
	#define EXT_syscomsettimeout
	#define GET_syscomsettimeout(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscomsettimeout(p0) 
	#define CHK_syscomsettimeout  FALSE
	#define EXP_syscomsettimeout  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscomsettimeout
	#define EXT_syscomsettimeout
	#define GET_syscomsettimeout(fl)  CAL_CMGETAPI( "syscomsettimeout" ) 
	#define CAL_syscomsettimeout  syscomsettimeout
	#define CHK_syscomsettimeout  TRUE
	#define EXP_syscomsettimeout  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsettimeout", (RTS_UINTPTR)syscomsettimeout, 1, RTSITF_GET_SIGNATURE(0, 0x2E035E87), 0x03051100) 
	#define EXTREF_ITF_syscomsettimeout {(RTS_VOID_FCTPTR)syscomsettimeout, "syscomsettimeout", RTSITF_GET_SIGNATURE(0, 0x2E035E87), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_syscomsettimeout
	#define EXT_syscomsettimeout
	#define GET_syscomsettimeout(fl)  CAL_CMGETAPI( "syscomsettimeout" ) 
	#define CAL_syscomsettimeout  syscomsettimeout
	#define CHK_syscomsettimeout  TRUE
	#define EXP_syscomsettimeout  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsettimeout", (RTS_UINTPTR)syscomsettimeout, 1, RTSITF_GET_SIGNATURE(0, 0x2E035E87), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComsyscomsettimeout
	#define EXT_SysComsyscomsettimeout
	#define GET_SysComsyscomsettimeout  ERR_OK
	#define CAL_SysComsyscomsettimeout  syscomsettimeout
	#define CHK_SysComsyscomsettimeout  TRUE
	#define EXP_SysComsyscomsettimeout  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsettimeout", (RTS_UINTPTR)syscomsettimeout, 1, RTSITF_GET_SIGNATURE(0, 0x2E035E87), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syscomsettimeout
	#define EXT_syscomsettimeout
	#define GET_syscomsettimeout(fl)  CAL_CMGETAPI( "syscomsettimeout" ) 
	#define CAL_syscomsettimeout  syscomsettimeout
	#define CHK_syscomsettimeout  TRUE
	#define EXP_syscomsettimeout  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsettimeout", (RTS_UINTPTR)syscomsettimeout, 1, RTSITF_GET_SIGNATURE(0, 0x2E035E87), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syscomsettimeout  PFSYSCOMSETTIMEOUT_IEC pfsyscomsettimeout;
	#define EXT_syscomsettimeout  extern PFSYSCOMSETTIMEOUT_IEC pfsyscomsettimeout;
	#define GET_syscomsettimeout(fl)  s_pfCMGetAPI2( "syscomsettimeout", (RTS_VOID_FCTPTR *)&pfsyscomsettimeout, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x2E035E87), 0x03051100)
	#define CAL_syscomsettimeout  pfsyscomsettimeout
	#define CHK_syscomsettimeout  (pfsyscomsettimeout != NULL)
	#define EXP_syscomsettimeout   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomsettimeout", (RTS_UINTPTR)syscomsettimeout, 1, RTSITF_GET_SIGNATURE(0, 0x2E035E87), 0x03051100) 
#endif


/**
 * <description><p>Writes the number of bytes to the send buffer of the specified device.</p>
 * <p>IMPLEMENTATION NOTE:
 * If the timeout elapsed until the requested number of bytes are sent,
 *	the function returns the number of bytes already sent! This must be considered in the caller and
 *	the implementation!</p></description>
 * <result><p>RESULT: Number of actual written bytes</p></result>
 */
typedef struct tagsyscomwrite_struct
{
	RTS_IEC_HANDLE hCom;				/* VAR_INPUT */	/* <param name="hCom" type="IN">Handle to com port</param> */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* <param name="pbyBuffer" type="IN">Pointer to data buffer for sent data</param> */
	RTS_IEC_UDINT ulSize;				/* VAR_INPUT */	/* <param name="uiSize" type="IN">Requested number of bytes to sent. Must be less or equal the size
	of the sent buffer!</param> */
	RTS_IEC_UDINT ulTimeout;			/* VAR_INPUT, Enum: SYS_COM_TIMEOUT */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* <param name="pResult" type="OUT">Pointer to return the runtime system error code (see CmpErrors.library).</param> */
	RTS_IEC_UDINT SysComWrite;			/* VAR_OUTPUT */	
} syscomwrite_struct;

void CDECL CDECL_EXT syscomwrite(syscomwrite_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSCOMWRITE_IEC) (syscomwrite_struct *p);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMWRITE_NOTIMPLEMENTED)
	#define USE_syscomwrite
	#define EXT_syscomwrite
	#define GET_syscomwrite(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syscomwrite(p0) 
	#define CHK_syscomwrite  FALSE
	#define EXP_syscomwrite  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syscomwrite
	#define EXT_syscomwrite
	#define GET_syscomwrite(fl)  CAL_CMGETAPI( "syscomwrite" ) 
	#define CAL_syscomwrite  syscomwrite
	#define CHK_syscomwrite  TRUE
	#define EXP_syscomwrite  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomwrite", (RTS_UINTPTR)syscomwrite, 1, RTSITF_GET_SIGNATURE(0, 0xA00AEE60), 0x03051100) 
	#define EXTREF_ITF_syscomwrite {(RTS_VOID_FCTPTR)syscomwrite, "syscomwrite", RTSITF_GET_SIGNATURE(0, 0xA00AEE60), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_syscomwrite
	#define EXT_syscomwrite
	#define GET_syscomwrite(fl)  CAL_CMGETAPI( "syscomwrite" ) 
	#define CAL_syscomwrite  syscomwrite
	#define CHK_syscomwrite  TRUE
	#define EXP_syscomwrite  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomwrite", (RTS_UINTPTR)syscomwrite, 1, RTSITF_GET_SIGNATURE(0, 0xA00AEE60), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComsyscomwrite
	#define EXT_SysComsyscomwrite
	#define GET_SysComsyscomwrite  ERR_OK
	#define CAL_SysComsyscomwrite  syscomwrite
	#define CHK_SysComsyscomwrite  TRUE
	#define EXP_SysComsyscomwrite  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomwrite", (RTS_UINTPTR)syscomwrite, 1, RTSITF_GET_SIGNATURE(0, 0xA00AEE60), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_syscomwrite
	#define EXT_syscomwrite
	#define GET_syscomwrite(fl)  CAL_CMGETAPI( "syscomwrite" ) 
	#define CAL_syscomwrite  syscomwrite
	#define CHK_syscomwrite  TRUE
	#define EXP_syscomwrite  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomwrite", (RTS_UINTPTR)syscomwrite, 1, RTSITF_GET_SIGNATURE(0, 0xA00AEE60), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_syscomwrite  PFSYSCOMWRITE_IEC pfsyscomwrite;
	#define EXT_syscomwrite  extern PFSYSCOMWRITE_IEC pfsyscomwrite;
	#define GET_syscomwrite(fl)  s_pfCMGetAPI2( "syscomwrite", (RTS_VOID_FCTPTR *)&pfsyscomwrite, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xA00AEE60), 0x03051100)
	#define CAL_syscomwrite  pfsyscomwrite
	#define CHK_syscomwrite  (pfsyscomwrite != NULL)
	#define EXP_syscomwrite   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syscomwrite", (RTS_UINTPTR)syscomwrite, 1, RTSITF_GET_SIGNATURE(0, 0xA00AEE60), 0x03051100) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


typedef SysComSettings		COMSettings;
typedef SysComSettingsEx	COMSettingsEx;
typedef SysComSettings		COM_Settings;
typedef SysComSettingsEx	COM_SettingsEx;

#ifdef __cplusplus
extern "C" {
#endif

/* Initialization routines for OS specific modules */
RTS_RESULT CDECL SysComOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysComOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>Open a serial communication device</description>
 * <param name="sPort" type="IN">Number of the Port to open: 1=COM1, 2=COM2, ...</param>
 * <param name="pResult" type="OUT">Pointer to error code </param>
 * <result>Handle to the interface or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL SysComOpen(short sPort, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSCOMOPEN) (short sPort, RTS_RESULT *pResult);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMOPEN_NOTIMPLEMENTED)
	#define USE_SysComOpen
	#define EXT_SysComOpen
	#define GET_SysComOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysComOpen(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysComOpen  FALSE
	#define EXP_SysComOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysComOpen
	#define EXT_SysComOpen
	#define GET_SysComOpen(fl)  CAL_CMGETAPI( "SysComOpen" ) 
	#define CAL_SysComOpen  SysComOpen
	#define CHK_SysComOpen  TRUE
	#define EXP_SysComOpen  CAL_CMEXPAPI( "SysComOpen" ) 
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_SysComOpen
	#define EXT_SysComOpen
	#define GET_SysComOpen(fl)  CAL_CMGETAPI( "SysComOpen" ) 
	#define CAL_SysComOpen  SysComOpen
	#define CHK_SysComOpen  TRUE
	#define EXP_SysComOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComOpen", (RTS_UINTPTR)SysComOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComSysComOpen
	#define EXT_SysComSysComOpen
	#define GET_SysComSysComOpen  ERR_OK
	#define CAL_SysComSysComOpen  ((ISysCom*)s_pfCMCreateInstance(CLASSID_CSysCom, NULL))->ISysComOpen
	#define CHK_SysComSysComOpen	(s_pfCMCreateInstance != NULL && pISysCom != NULL)
	#define EXP_SysComSysComOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysComOpen
	#define EXT_SysComOpen
	#define GET_SysComOpen(fl)  CAL_CMGETAPI( "SysComOpen" ) 
	#define CAL_SysComOpen  ((ISysCom*)s_pfCMCreateInstance(CLASSID_CSysCom, NULL))->ISysComOpen
	#define CHK_SysComOpen	(s_pfCMCreateInstance != NULL && pISysCom != NULL)
	#define EXP_SysComOpen  CAL_CMEXPAPI( "SysComOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysComOpen  PFSYSCOMOPEN pfSysComOpen;
	#define EXT_SysComOpen  extern PFSYSCOMOPEN pfSysComOpen;
	#define GET_SysComOpen(fl)  s_pfCMGetAPI2( "SysComOpen", (RTS_VOID_FCTPTR *)&pfSysComOpen, (fl), 0, 0)
	#define CAL_SysComOpen  pfSysComOpen
	#define CHK_SysComOpen  (pfSysComOpen != NULL)
	#define EXP_SysComOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComOpen", (RTS_UINTPTR)SysComOpen, 0, 0) 
#endif




/**
 * <description>Opens a serial communication device specified with settings</description>
 * <param name="pSettings" type="IN">Settings for the communication device. 
 *	See category "Com port settings" for detailed information</param>
 * <param name="pSettingsEx" type="IN">Optional extended settings for the serial device. Can be NULL.</param>
 * <param name="pResult" type="OUT">Pointer to error code </param>
 * <result>Handle to the interface or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL SysComOpen2(SysComSettings *pSettings, SysComSettingsEx *pSettingsEx, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSCOMOPEN2) (SysComSettings *pSettings, SysComSettingsEx *pSettingsEx, RTS_RESULT *pResult);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMOPEN2_NOTIMPLEMENTED)
	#define USE_SysComOpen2
	#define EXT_SysComOpen2
	#define GET_SysComOpen2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysComOpen2(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysComOpen2  FALSE
	#define EXP_SysComOpen2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysComOpen2
	#define EXT_SysComOpen2
	#define GET_SysComOpen2(fl)  CAL_CMGETAPI( "SysComOpen2" ) 
	#define CAL_SysComOpen2  SysComOpen2
	#define CHK_SysComOpen2  TRUE
	#define EXP_SysComOpen2  CAL_CMEXPAPI( "SysComOpen2" ) 
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_SysComOpen2
	#define EXT_SysComOpen2
	#define GET_SysComOpen2(fl)  CAL_CMGETAPI( "SysComOpen2" ) 
	#define CAL_SysComOpen2  SysComOpen2
	#define CHK_SysComOpen2  TRUE
	#define EXP_SysComOpen2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComOpen2", (RTS_UINTPTR)SysComOpen2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComSysComOpen2
	#define EXT_SysComSysComOpen2
	#define GET_SysComSysComOpen2  ERR_OK
	#define CAL_SysComSysComOpen2  ((ISysCom*)s_pfCMCreateInstance(CLASSID_CSysCom, NULL))->ISysComOpen2
	#define CHK_SysComSysComOpen2	(s_pfCMCreateInstance != NULL && pISysCom != NULL)
	#define EXP_SysComSysComOpen2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysComOpen2
	#define EXT_SysComOpen2
	#define GET_SysComOpen2(fl)  CAL_CMGETAPI( "SysComOpen2" ) 
	#define CAL_SysComOpen2  ((ISysCom*)s_pfCMCreateInstance(CLASSID_CSysCom, NULL))->ISysComOpen2
	#define CHK_SysComOpen2	(s_pfCMCreateInstance != NULL && pISysCom != NULL)
	#define EXP_SysComOpen2  CAL_CMEXPAPI( "SysComOpen2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysComOpen2  PFSYSCOMOPEN2 pfSysComOpen2;
	#define EXT_SysComOpen2  extern PFSYSCOMOPEN2 pfSysComOpen2;
	#define GET_SysComOpen2(fl)  s_pfCMGetAPI2( "SysComOpen2", (RTS_VOID_FCTPTR *)&pfSysComOpen2, (fl), 0, 0)
	#define CAL_SysComOpen2  pfSysComOpen2
	#define CHK_SysComOpen2  (pfSysComOpen2 != NULL)
	#define EXP_SysComOpen2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComOpen2", (RTS_UINTPTR)SysComOpen2, 0, 0) 
#endif




/**
 * <description>Set the parameter of the interface specified by handle</description>
 * <param name="hCom" type="IN">Handle to Com Port</param>
 * <param name="pSettings" type="IN">Pointer to new settings</param>
 * <param name="pSettingsEx" type="IN">Pointer to extended settings</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysComSetSettings(RTS_HANDLE hCom, SysComSettings *pSettings, SysComSettingsEx *pSettingsEx);
typedef RTS_RESULT (CDECL * PFSYSCOMSETSETTINGS) (RTS_HANDLE hCom, SysComSettings *pSettings, SysComSettingsEx *pSettingsEx);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMSETSETTINGS_NOTIMPLEMENTED)
	#define USE_SysComSetSettings
	#define EXT_SysComSetSettings
	#define GET_SysComSetSettings(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysComSetSettings(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysComSetSettings  FALSE
	#define EXP_SysComSetSettings  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysComSetSettings
	#define EXT_SysComSetSettings
	#define GET_SysComSetSettings(fl)  CAL_CMGETAPI( "SysComSetSettings" ) 
	#define CAL_SysComSetSettings  SysComSetSettings
	#define CHK_SysComSetSettings  TRUE
	#define EXP_SysComSetSettings  CAL_CMEXPAPI( "SysComSetSettings" ) 
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_SysComSetSettings
	#define EXT_SysComSetSettings
	#define GET_SysComSetSettings(fl)  CAL_CMGETAPI( "SysComSetSettings" ) 
	#define CAL_SysComSetSettings  SysComSetSettings
	#define CHK_SysComSetSettings  TRUE
	#define EXP_SysComSetSettings  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComSetSettings", (RTS_UINTPTR)SysComSetSettings, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComSysComSetSettings
	#define EXT_SysComSysComSetSettings
	#define GET_SysComSysComSetSettings  ERR_OK
	#define CAL_SysComSysComSetSettings(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComSetSettings(p1,p2) : ((ISysCom*)p0)->ISysComSetSettings(p1,p2))
	#define CHK_SysComSysComSetSettings  (pISysCom != NULL)
	#define EXP_SysComSysComSetSettings  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysComSetSettings
	#define EXT_SysComSetSettings
	#define GET_SysComSetSettings(fl)  CAL_CMGETAPI( "SysComSetSettings" ) 
	#define CAL_SysComSetSettings(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComSetSettings(p1,p2) : ((ISysCom*)p0)->ISysComSetSettings(p1,p2))
	#define CHK_SysComSetSettings  (pISysCom != NULL)
	#define EXP_SysComSetSettings  CAL_CMEXPAPI( "SysComSetSettings" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysComSetSettings  PFSYSCOMSETSETTINGS pfSysComSetSettings;
	#define EXT_SysComSetSettings  extern PFSYSCOMSETSETTINGS pfSysComSetSettings;
	#define GET_SysComSetSettings(fl)  s_pfCMGetAPI2( "SysComSetSettings", (RTS_VOID_FCTPTR *)&pfSysComSetSettings, (fl), 0, 0)
	#define CAL_SysComSetSettings  pfSysComSetSettings
	#define CHK_SysComSetSettings  (pfSysComSetSettings != NULL)
	#define EXP_SysComSetSettings  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComSetSettings", (RTS_UINTPTR)SysComSetSettings, 0, 0) 
#endif




/**
 * <description>Get the parameter of the interface specified by handle</description>
 * <param name="hCom" type="IN">Handle to Com Port</param>
 * <param name="pSettings" type="IN">Pointer to new settings</param>
 * <param name="pSettingsEx" type="IN">Pointer to extended settings</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysComGetSettings(RTS_HANDLE hCom, SysComSettings *pSettings, SysComSettingsEx *pSettingsEx);
typedef RTS_RESULT (CDECL * PFSYSCOMGETSETTINGS) (RTS_HANDLE hCom, SysComSettings *pSettings, SysComSettingsEx *pSettingsEx);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMGETSETTINGS_NOTIMPLEMENTED)
	#define USE_SysComGetSettings
	#define EXT_SysComGetSettings
	#define GET_SysComGetSettings(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysComGetSettings(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysComGetSettings  FALSE
	#define EXP_SysComGetSettings  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysComGetSettings
	#define EXT_SysComGetSettings
	#define GET_SysComGetSettings(fl)  CAL_CMGETAPI( "SysComGetSettings" ) 
	#define CAL_SysComGetSettings  SysComGetSettings
	#define CHK_SysComGetSettings  TRUE
	#define EXP_SysComGetSettings  CAL_CMEXPAPI( "SysComGetSettings" ) 
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_SysComGetSettings
	#define EXT_SysComGetSettings
	#define GET_SysComGetSettings(fl)  CAL_CMGETAPI( "SysComGetSettings" ) 
	#define CAL_SysComGetSettings  SysComGetSettings
	#define CHK_SysComGetSettings  TRUE
	#define EXP_SysComGetSettings  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComGetSettings", (RTS_UINTPTR)SysComGetSettings, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComSysComGetSettings
	#define EXT_SysComSysComGetSettings
	#define GET_SysComSysComGetSettings  ERR_OK
	#define CAL_SysComSysComGetSettings(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComGetSettings(p1,p2) : ((ISysCom*)p0)->ISysComGetSettings(p1,p2))
	#define CHK_SysComSysComGetSettings  (pISysCom != NULL)
	#define EXP_SysComSysComGetSettings  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysComGetSettings
	#define EXT_SysComGetSettings
	#define GET_SysComGetSettings(fl)  CAL_CMGETAPI( "SysComGetSettings" ) 
	#define CAL_SysComGetSettings(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComGetSettings(p1,p2) : ((ISysCom*)p0)->ISysComGetSettings(p1,p2))
	#define CHK_SysComGetSettings  (pISysCom != NULL)
	#define EXP_SysComGetSettings  CAL_CMEXPAPI( "SysComGetSettings" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysComGetSettings  PFSYSCOMGETSETTINGS pfSysComGetSettings;
	#define EXT_SysComGetSettings  extern PFSYSCOMGETSETTINGS pfSysComGetSettings;
	#define GET_SysComGetSettings(fl)  s_pfCMGetAPI2( "SysComGetSettings", (RTS_VOID_FCTPTR *)&pfSysComGetSettings, (fl), 0, 0)
	#define CAL_SysComGetSettings  pfSysComGetSettings
	#define CHK_SysComGetSettings  (pfSysComGetSettings != NULL)
	#define EXP_SysComGetSettings  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComGetSettings", (RTS_UINTPTR)SysComGetSettings, 0, 0) 
#endif




/**
 * <description>Close a serial communication device</description>
 * <param name="hCom" type="IN">Handle to com port</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysComClose(RTS_HANDLE hCom);
typedef RTS_RESULT (CDECL * PFSYSCOMCLOSE) (RTS_HANDLE hCom);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMCLOSE_NOTIMPLEMENTED)
	#define USE_SysComClose
	#define EXT_SysComClose
	#define GET_SysComClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysComClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysComClose  FALSE
	#define EXP_SysComClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysComClose
	#define EXT_SysComClose
	#define GET_SysComClose(fl)  CAL_CMGETAPI( "SysComClose" ) 
	#define CAL_SysComClose  SysComClose
	#define CHK_SysComClose  TRUE
	#define EXP_SysComClose  CAL_CMEXPAPI( "SysComClose" ) 
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_SysComClose
	#define EXT_SysComClose
	#define GET_SysComClose(fl)  CAL_CMGETAPI( "SysComClose" ) 
	#define CAL_SysComClose  SysComClose
	#define CHK_SysComClose  TRUE
	#define EXP_SysComClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComClose", (RTS_UINTPTR)SysComClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComSysComClose
	#define EXT_SysComSysComClose
	#define GET_SysComSysComClose  ERR_OK
	#define CAL_SysComSysComClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysCom*)p0)->ISysComClose())
	#define CHK_SysComSysComClose  TRUE
	#define EXP_SysComSysComClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysComClose
	#define EXT_SysComClose
	#define GET_SysComClose(fl)  CAL_CMGETAPI( "SysComClose" ) 
	#define CAL_SysComClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysCom*)p0)->ISysComClose())
	#define CHK_SysComClose  TRUE
	#define EXP_SysComClose  CAL_CMEXPAPI( "SysComClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysComClose  PFSYSCOMCLOSE pfSysComClose;
	#define EXT_SysComClose  extern PFSYSCOMCLOSE pfSysComClose;
	#define GET_SysComClose(fl)  s_pfCMGetAPI2( "SysComClose", (RTS_VOID_FCTPTR *)&pfSysComClose, (fl), 0, 0)
	#define CAL_SysComClose  pfSysComClose
	#define CHK_SysComClose  (pfSysComClose != NULL)
	#define EXP_SysComClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComClose", (RTS_UINTPTR)SysComClose, 0, 0) 
#endif




/**
 * <description>Reads a number bytes from the specified device to the receive buffer.
 *
 * <p>IMPLEMENTATION NOTE: If the timeout elapsed until the requested number of bytes are received,
 *	the function returns with the bytes still received! This must be considered in the caller and
 *	the implementation!</p></description>
 *
 * <param name="hCom" type="IN">Handle to com port</param>
 * <param name="pbyBuffer" type="IN">Pointer to data buffer for received data</param>
 * <param name="uiSize" type="IN">Requested number of bytes to read. Must be less or equal the size
 *	of the receive buffer!</param>
 * <param name="ulTimeout" type="IN">Timeout to read data from the device. 0=Immediate return.
 *	If the timeout elapsed, the function returns with the still received data (could be less then the 
 *	requested number of bytes!)</param>
 * <param name="pResult" type="IN">Pointer to error code, ERR_TIMEOUT if timeout elapsed</param>
 * <result>Number of actually read bytes</result>
 */
RTS_UI32 CDECL SysComRead(RTS_HANDLE hCom, unsigned char *pbyBuffer, RTS_UI32 uiSize, RTS_UI32 ulTimeout, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFSYSCOMREAD) (RTS_HANDLE hCom, unsigned char *pbyBuffer, RTS_UI32 uiSize, RTS_UI32 ulTimeout, RTS_RESULT *pResult);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMREAD_NOTIMPLEMENTED)
	#define USE_SysComRead
	#define EXT_SysComRead
	#define GET_SysComRead(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysComRead(p0,p1,p2,p3,p4)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SysComRead  FALSE
	#define EXP_SysComRead  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysComRead
	#define EXT_SysComRead
	#define GET_SysComRead(fl)  CAL_CMGETAPI( "SysComRead" ) 
	#define CAL_SysComRead  SysComRead
	#define CHK_SysComRead  TRUE
	#define EXP_SysComRead  CAL_CMEXPAPI( "SysComRead" ) 
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_SysComRead
	#define EXT_SysComRead
	#define GET_SysComRead(fl)  CAL_CMGETAPI( "SysComRead" ) 
	#define CAL_SysComRead  SysComRead
	#define CHK_SysComRead  TRUE
	#define EXP_SysComRead  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComRead", (RTS_UINTPTR)SysComRead, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComSysComRead
	#define EXT_SysComSysComRead
	#define GET_SysComSysComRead  ERR_OK
	#define CAL_SysComSysComRead(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComRead(p1,p2,p3,p4) : ((ISysCom*)p0)->ISysComRead(p1,p2,p3,p4))
	#define CHK_SysComSysComRead  (pISysCom != NULL)
	#define EXP_SysComSysComRead  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysComRead
	#define EXT_SysComRead
	#define GET_SysComRead(fl)  CAL_CMGETAPI( "SysComRead" ) 
	#define CAL_SysComRead(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComRead(p1,p2,p3,p4) : ((ISysCom*)p0)->ISysComRead(p1,p2,p3,p4))
	#define CHK_SysComRead  (pISysCom != NULL)
	#define EXP_SysComRead  CAL_CMEXPAPI( "SysComRead" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysComRead  PFSYSCOMREAD pfSysComRead;
	#define EXT_SysComRead  extern PFSYSCOMREAD pfSysComRead;
	#define GET_SysComRead(fl)  s_pfCMGetAPI2( "SysComRead", (RTS_VOID_FCTPTR *)&pfSysComRead, (fl), 0, 0)
	#define CAL_SysComRead  pfSysComRead
	#define CHK_SysComRead  (pfSysComRead != NULL)
	#define EXP_SysComRead  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComRead", (RTS_UINTPTR)SysComRead, 0, 0) 
#endif




/**
 * <description>Writes a number bytes to the specified device from the sent buffer.
 *
 * <p>IMPLEMENTATION NOTE: If the timeout elapsed until the requested number of bytes are sent,
 *	the function returns with the bytes still sent! This must be considered in the caller and
 *	the implementation!</p></description>
 *
 * <param name="hCom" type="IN">Handle to com port</param>
 * <param name="pbyBuffer" type="IN">Pointer to data buffer for sent data</param>
 * <param name="uiSize" type="IN">Requested number of bytes to sent. Must be less or equal the size
 *	of the sent buffer!</param>
 * <param name="ulTimeout" type="IN">Timeout to sent data to the device. 0=Immediate return.
 *	If the timeout elapsed, the function returns with the still sent data (could be less then the 
 *	requested number of bytes!)</param>
 * <param name="pResult" type="IN">Pointer to error code, ERR_TIMEOUT if timeout elapsed</param>
 * <result>Number of actually written bytes</result>
 */
RTS_UI32 CDECL SysComWrite(RTS_HANDLE hCom, unsigned char *pbyBuffer, RTS_UI32 uiSize, RTS_UI32 ulTimeout, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFSYSCOMWRITE) (RTS_HANDLE hCom, unsigned char *pbyBuffer, RTS_UI32 uiSize, RTS_UI32 ulTimeout, RTS_RESULT *pResult);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMWRITE_NOTIMPLEMENTED)
	#define USE_SysComWrite
	#define EXT_SysComWrite
	#define GET_SysComWrite(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysComWrite(p0,p1,p2,p3,p4)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SysComWrite  FALSE
	#define EXP_SysComWrite  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysComWrite
	#define EXT_SysComWrite
	#define GET_SysComWrite(fl)  CAL_CMGETAPI( "SysComWrite" ) 
	#define CAL_SysComWrite  SysComWrite
	#define CHK_SysComWrite  TRUE
	#define EXP_SysComWrite  CAL_CMEXPAPI( "SysComWrite" ) 
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_SysComWrite
	#define EXT_SysComWrite
	#define GET_SysComWrite(fl)  CAL_CMGETAPI( "SysComWrite" ) 
	#define CAL_SysComWrite  SysComWrite
	#define CHK_SysComWrite  TRUE
	#define EXP_SysComWrite  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComWrite", (RTS_UINTPTR)SysComWrite, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComSysComWrite
	#define EXT_SysComSysComWrite
	#define GET_SysComSysComWrite  ERR_OK
	#define CAL_SysComSysComWrite(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComWrite(p1,p2,p3,p4) : ((ISysCom*)p0)->ISysComWrite(p1,p2,p3,p4))
	#define CHK_SysComSysComWrite  (pISysCom != NULL)
	#define EXP_SysComSysComWrite  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysComWrite
	#define EXT_SysComWrite
	#define GET_SysComWrite(fl)  CAL_CMGETAPI( "SysComWrite" ) 
	#define CAL_SysComWrite(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComWrite(p1,p2,p3,p4) : ((ISysCom*)p0)->ISysComWrite(p1,p2,p3,p4))
	#define CHK_SysComWrite  (pISysCom != NULL)
	#define EXP_SysComWrite  CAL_CMEXPAPI( "SysComWrite" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysComWrite  PFSYSCOMWRITE pfSysComWrite;
	#define EXT_SysComWrite  extern PFSYSCOMWRITE pfSysComWrite;
	#define GET_SysComWrite(fl)  s_pfCMGetAPI2( "SysComWrite", (RTS_VOID_FCTPTR *)&pfSysComWrite, (fl), 0, 0)
	#define CAL_SysComWrite  pfSysComWrite
	#define CHK_SysComWrite  (pfSysComWrite != NULL)
	#define EXP_SysComWrite  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComWrite", (RTS_UINTPTR)SysComWrite, 0, 0) 
#endif




 /**
 * <description>Clear the FIFO buffer of the serial interface</description>
 * <param name="hCom" type="IN">Handle to com port</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysComPurge(RTS_HANDLE hCom);
typedef RTS_RESULT (CDECL * PFSYSCOMPURGE) (RTS_HANDLE hCom);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMPURGE_NOTIMPLEMENTED)
	#define USE_SysComPurge
	#define EXT_SysComPurge
	#define GET_SysComPurge(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysComPurge(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysComPurge  FALSE
	#define EXP_SysComPurge  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysComPurge
	#define EXT_SysComPurge
	#define GET_SysComPurge(fl)  CAL_CMGETAPI( "SysComPurge" ) 
	#define CAL_SysComPurge  SysComPurge
	#define CHK_SysComPurge  TRUE
	#define EXP_SysComPurge  CAL_CMEXPAPI( "SysComPurge" ) 
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_SysComPurge
	#define EXT_SysComPurge
	#define GET_SysComPurge(fl)  CAL_CMGETAPI( "SysComPurge" ) 
	#define CAL_SysComPurge  SysComPurge
	#define CHK_SysComPurge  TRUE
	#define EXP_SysComPurge  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComPurge", (RTS_UINTPTR)SysComPurge, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComSysComPurge
	#define EXT_SysComSysComPurge
	#define GET_SysComSysComPurge  ERR_OK
	#define CAL_SysComSysComPurge(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComPurge() : ((ISysCom*)p0)->ISysComPurge())
	#define CHK_SysComSysComPurge  (pISysCom != NULL)
	#define EXP_SysComSysComPurge  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysComPurge
	#define EXT_SysComPurge
	#define GET_SysComPurge(fl)  CAL_CMGETAPI( "SysComPurge" ) 
	#define CAL_SysComPurge(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComPurge() : ((ISysCom*)p0)->ISysComPurge())
	#define CHK_SysComPurge  (pISysCom != NULL)
	#define EXP_SysComPurge  CAL_CMEXPAPI( "SysComPurge" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysComPurge  PFSYSCOMPURGE pfSysComPurge;
	#define EXT_SysComPurge  extern PFSYSCOMPURGE pfSysComPurge;
	#define GET_SysComPurge(fl)  s_pfCMGetAPI2( "SysComPurge", (RTS_VOID_FCTPTR *)&pfSysComPurge, (fl), 0, 0)
	#define CAL_SysComPurge  pfSysComPurge
	#define CHK_SysComPurge  (pfSysComPurge != NULL)
	#define EXP_SysComPurge  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComPurge", (RTS_UINTPTR)SysComPurge, 0, 0) 
#endif




/**
 * <description>Set the timeout of the specified serial interface (hardware timeout).
 *	The Timeout is the time between two received or sent characters until the read or write operation
 *	will return. Typically this value should be 0 (returns immediately)</description>
 * <param name="hCom" type="IN">Handle to com port</param>
 * <param name="ulTimeout" type="IN">Timeout to set</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysComSetTimeout(RTS_HANDLE hCom, RTS_UI32 ulTimeout);
typedef RTS_RESULT (CDECL * PFSYSCOMSETTIMEOUT) (RTS_HANDLE hCom, RTS_UI32 ulTimeout);
#if defined(SYSCOM_NOTIMPLEMENTED) || defined(SYSCOMSETTIMEOUT_NOTIMPLEMENTED)
	#define USE_SysComSetTimeout
	#define EXT_SysComSetTimeout
	#define GET_SysComSetTimeout(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysComSetTimeout(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysComSetTimeout  FALSE
	#define EXP_SysComSetTimeout  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysComSetTimeout
	#define EXT_SysComSetTimeout
	#define GET_SysComSetTimeout(fl)  CAL_CMGETAPI( "SysComSetTimeout" ) 
	#define CAL_SysComSetTimeout  SysComSetTimeout
	#define CHK_SysComSetTimeout  TRUE
	#define EXP_SysComSetTimeout  CAL_CMEXPAPI( "SysComSetTimeout" ) 
#elif defined(MIXED_LINK) && !defined(SYSCOM_EXTERNAL)
	#define USE_SysComSetTimeout
	#define EXT_SysComSetTimeout
	#define GET_SysComSetTimeout(fl)  CAL_CMGETAPI( "SysComSetTimeout" ) 
	#define CAL_SysComSetTimeout  SysComSetTimeout
	#define CHK_SysComSetTimeout  TRUE
	#define EXP_SysComSetTimeout  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComSetTimeout", (RTS_UINTPTR)SysComSetTimeout, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysComSysComSetTimeout
	#define EXT_SysComSysComSetTimeout
	#define GET_SysComSysComSetTimeout  ERR_OK
	#define CAL_SysComSysComSetTimeout(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComSetTimeout(p1) : ((ISysCom*)p0)->ISysComSetTimeout(p1))
	#define CHK_SysComSysComSetTimeout  (pISysCom != NULL)
	#define EXP_SysComSysComSetTimeout  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysComSetTimeout
	#define EXT_SysComSetTimeout
	#define GET_SysComSetTimeout(fl)  CAL_CMGETAPI( "SysComSetTimeout" ) 
	#define CAL_SysComSetTimeout(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysCom->ISysComSetTimeout(p1) : ((ISysCom*)p0)->ISysComSetTimeout(p1))
	#define CHK_SysComSetTimeout  (pISysCom != NULL)
	#define EXP_SysComSetTimeout  CAL_CMEXPAPI( "SysComSetTimeout" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysComSetTimeout  PFSYSCOMSETTIMEOUT pfSysComSetTimeout;
	#define EXT_SysComSetTimeout  extern PFSYSCOMSETTIMEOUT pfSysComSetTimeout;
	#define GET_SysComSetTimeout(fl)  s_pfCMGetAPI2( "SysComSetTimeout", (RTS_VOID_FCTPTR *)&pfSysComSetTimeout, (fl), 0, 0)
	#define CAL_SysComSetTimeout  pfSysComSetTimeout
	#define CHK_SysComSetTimeout  (pfSysComSetTimeout != NULL)
	#define EXP_SysComSetTimeout  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysComSetTimeout", (RTS_UINTPTR)SysComSetTimeout, 0, 0) 
#endif





#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSCOMOPEN ISysComOpen;
 	PFSYSCOMOPEN2 ISysComOpen2;
 	PFSYSCOMSETSETTINGS ISysComSetSettings;
 	PFSYSCOMGETSETTINGS ISysComGetSettings;
 	PFSYSCOMCLOSE ISysComClose;
 	PFSYSCOMREAD ISysComRead;
 	PFSYSCOMWRITE ISysComWrite;
 	PFSYSCOMPURGE ISysComPurge;
 	PFSYSCOMSETTIMEOUT ISysComSetTimeout;
 } ISysCom_C;

#ifdef CPLUSPLUS
class ISysCom : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISysComOpen(short sPort, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysComOpen2(SysComSettings *pSettings, SysComSettingsEx *pSettingsEx, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysComSetSettings(SysComSettings *pSettings, SysComSettingsEx *pSettingsEx) =0;
		virtual RTS_RESULT CDECL ISysComGetSettings(SysComSettings *pSettings, SysComSettingsEx *pSettingsEx) =0;
		virtual RTS_RESULT CDECL ISysComClose(void) =0;
		virtual RTS_UI32 CDECL ISysComRead(unsigned char *pbyBuffer, RTS_UI32 uiSize, RTS_UI32 ulTimeout, RTS_RESULT *pResult) =0;
		virtual RTS_UI32 CDECL ISysComWrite(unsigned char *pbyBuffer, RTS_UI32 uiSize, RTS_UI32 ulTimeout, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysComPurge(void) =0;
		virtual RTS_RESULT CDECL ISysComSetTimeout(RTS_UI32 ulTimeout) =0;
};
	#ifndef ITF_SysCom
		#define ITF_SysCom static ISysCom *pISysCom = NULL;
	#endif
	#define EXTITF_SysCom
#else	/*CPLUSPLUS*/
	typedef ISysCom_C		ISysCom;
	#ifndef ITF_SysCom
		#define ITF_SysCom
	#endif
	#define EXTITF_SysCom
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSCOMITF_H_*/
