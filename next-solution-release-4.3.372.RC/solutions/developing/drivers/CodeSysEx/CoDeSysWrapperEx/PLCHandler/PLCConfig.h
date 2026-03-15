/*****************************************************************************
*
*	Copyright:		CODESYS Development GmbH
*	Module: 		PLCConfig
*	Purpose:		Contains all structure definitions for PLC configuration.
*
******************************************************************************/

#ifndef __PLCCONFIG_H__
#define __PLCCONFIG_H__

#undef PLCH_DLL_DECL
#if defined(PLCH_USE_DLL) && (defined(WIN32) || defined(_WIN32_WCE) || defined(_WIN32_WCE_EMULATION))
	#ifdef _USRDLL
		#define PLCH_DLL_DECL _declspec(dllexport)
	#else
		#define PLCH_DLL_DECL _declspec(dllimport)
	#endif
#elif (defined(LINUX) || defined(QNX)) && defined(_USRDLL)
	#define PLCH_DLL_DECL __attribute__ ((visibility ("default")))
#else
	#define PLCH_DLL_DECL
#endif

#ifndef TRG_64BIT 
	/* needed for compatibility because PlcParameterValue (8 byte) is misaligned in PlcParameter structure (4 byte offset) */
	#pragma pack (4)						
#endif

/* ComItfs */
typedef enum ItfTypetag
{
	IT_INVALID,
	IT_SIMULATION,
	IT_ARTI,
	IT_GATEWAY,
	IT_DIRECT,
	IT_GATEWAY3,
	IT_SIMULATION3,
	IT_ARTI3
} ItfType;

/* Defines for HW names */
#define PLCC_HW_STANDARD	"Standard"
#define PLCC_HW_MAX4		"ElauMaX4"

/* Various parameter types */
typedef enum PlcParameterTypeTag			/* various types for the device parameters */
{
	PLC_PT_VOID = 0,
	PLC_PT_ULONG,
	PLC_PT_REAL,
	PLC_PT_LREAL,
	PLC_PT_STRING,
	PLC_PT_WSTRING
} PlcParameterType;

/* Access defines to parameter value */
#define PLCC_PV_ULONG(v)		(*(unsigned long*)&(v)->Value.dw)
#define PLCC_PV_REAL(v)			(*(float*)&(v)->Value.r)
#define PLCC_PV_LREAL(v)		(*(double*)&(v)->Value.lr)
#define PLCC_PV_STRING(v)		((v)->Value.psz)
#define PLCC_PV_WSTRING(v)		((v)->Value.pwsz)

#if __cplusplus

/* Connection to Gateway */
struct PLCH_DLL_DECL GatewayConnection
{
	GatewayConnection(void);
	~GatewayConnection(void);

	char*		pszDeviceName;					/* Name of the gateway device/protocol ("Tcp/Ip", "Local", "SharedMemory", "NamedSharedMemory", ...) */
	char*		pszAddress;						/* Tcp/Ip address or shared memory name of the gateway */
	unsigned long ulPort;						/* Tcp/Ip port of the gateway */
	char*		pszPassword;					/* Optional gateway password */
private:
	GatewayConnection(const GatewayConnection& rhs);
	GatewayConnection& operator=(const GatewayConnection& rhs);
};

/* Configuration structure for a single PLC */
struct PLCH_DLL_DECL PlcConfig
{
	PlcConfig(void);
	~PlcConfig(void);

	unsigned long		ulId;					/* unique ID for every PLC */
	char				*pszName;				/* name of the PLC */
	ItfType				it;						/* interface type of the PLC (IT_ARTI, IT_GATEWAY, ...) */
	char				bActive;				/* flag if the PLC is active or not */
	char				bMotorola;				/* flag if data has to be swapt for motorola byte order	(only used for V2) */			
	char				bLogin;					/* flag if the application can do a login */
	char				bLogToFile;				/* flag if the application should log to a file */
	char				bPreCheckIdentity;		/* flag if the identity will be checked before every write/read (only used for V2)*/
	unsigned long		ulTimeout;				/* communication timeout in ms */
	unsigned long		ulNumTries;				/* number of tries of receive's before throwing an COMM_FATAL error */
	unsigned long		ulWaitTime;				/* time in ms to wait for a connection to the PLC before Connect() throws an error */
	unsigned long		ulReconnectTime;		/* time interval in ms to try for a reconnection */
	char				*pszHwType;				/* HW type of the PLC (PLCC_HW_STANDARD, PLCC_HW_MAX4) */
	unsigned long		ulHwVersion;			/* HW version  */
	unsigned long		ulBufferSize;			/* communication buffersize of the runtime system running on the PLC; 0 = default size */
	char				*pszProjectName;		/* name of the CoDeSys project running on the runtime system */
	char				*pszDllDirectory;		/* directory where to find DLLs (only for very special cases)*/
	GatewayConnection	*gwc;					/* pointer to the gateway connection if using the gateway interface */
	unsigned long		ulLogFilter;			/* filter for log actions */
private:
	PlcConfig(const PlcConfig& rhs);
	PlcConfig& operator=(const PlcConfig& rhs);
};

/* Various parameter values */
union PLCH_DLL_DECL PlcParameterValue			/* various values for the device parameters */
{
	PlcParameterValue(void);
	~PlcParameterValue(void);

	unsigned long		dw;
	float				r;
	double				lr;
	char				*psz;
	RTS_WCHAR			*pwsz;
};

/* Description of a parameter value */
struct PLCH_DLL_DECL PlcParameter
{
	PlcParameter(void);
	~PlcParameter(void);

	PlcParameterType	Type;					/* type of the parameter */
	PlcParameterValue	Value;					/* value of the parameter */
private:
	PlcParameter(const PlcParameter& rhs);
	PlcParameter(const PlcParameterType& ppt);
};

/* Complete description of a parameter */
struct PLCH_DLL_DECL PlcParameterDesc
{
	PlcParameterDesc(void);
	~PlcParameterDesc(void);

	unsigned long	ulId;						/* id for the parameter, currently not evaluated */
	char*			pszName;					/* name of the parameter, identifies the parameter */
	PlcParameter*	pParameter;					/* type and value of the parameter */
private:
	PlcParameterDesc(const PlcParameterDesc& rhs);
	PlcParameterDesc& operator=(const PlcParameterDesc& rhs);
};

/* Device description */
struct PLCH_DLL_DECL PlcDeviceDesc
{
	PlcDeviceDesc(void);
	~PlcDeviceDesc(void);

	char*				pszName;				/* name of the device / protocol °(V2 only) */
	char*				pszInstance;			/* optional instance name of the gateway device (V2 only) */
	char*				pszProject;				/* not used - see PlcConfig.pszProjectName instead */
	unsigned long		ulNumParams;			/* number of the following parameters */
	PlcParameterDesc*	ppd;					/* pointer to an array of parameter descriptions */
private:
	PlcDeviceDesc(const PlcDeviceDesc& rhs);
	PlcDeviceDesc& operator=(const PlcDeviceDesc& rhs);
};

#endif


/* Defines for device names */
#define PLCC_DN_TCPIP   		"Tcp/Ip"				/* is identical with "Tcp/Ip (Level 4)" */
#define PLCC_DN_TCPIP_L4		"Tcp/Ip (Level 4)"		/* is identical with "Tcp/Ip" */
#define PLCC_DN_TCPIP_L2ROUTE	"Tcp/Ip (Level 2 Route)"
#define PLCC_DN_TCPIP_L2		"Tcp/Ip (Level 2)"
#define PLCC_DN_SERIAL			"Serial (RS232)"
#define PLCC_DN_TCPIP_L2ABB		"Tcp/Ip (Level 2 ABB)"

/* Defines for parameter names */
#define PLCC_PN_IP_ADDRESS				"IpAddress"					/* V3: TCP/IP Address or network name as string*/
#define PLCC_PN_PORT					"Port"						/* All: TCP/IP or serial port */
#define PLCC_PN_ADDRESS					"Address"					/* V2: TCP/IP Address or V3 logical address or PLC name as string*/
#define PLCC_PN_TARGET					"TargetId"					/* V2: Target ID when using the route protocol */
#define PLCC_PN_MOTOROLA				"Motorola byteorder"		/* V2: flag if to swap protocol data for motorola byte order */
#define PLCC_PN_BLOCKSIZE				"Blocksize"					/* V2: blocksize for communication, when using L2 protocol */
#define PLCC_PN_BAUDRATE				"Baudrate"					/* V2: baudrate for underlaying communication system */
#define PLCC_PN_PARITY					"Parity"					/* V2: parity for serial interface */
#define PLCC_PN_STOPBITS				"Stop bits"					/* V2: number of stop bits for serial interface */
#define PLCC_PN_PING					"Ping"						/* ARTI: enable/disable ping to target before connect (1=enabled [default], 0=disabled) */
#define PLCC_PN_SYMBOLFILE				"SymbolFile"				/* ARTI: name of the symbol file if it's not "Download.sdb" or on ELAU targets "Default.sdb" */
#define PLCC_PN_SYMBOLFILE_PATH			"SymbolFilePath"			/* ARTI: path for the local symbolfile (caching functionality) */
#define PLCC_PN_DIRECT_ADDR_ONLY		"DirectAddressesOnly"		/* ARTI: Use only direct (e. g. %MB0) addresses (no symbolfile needed) (0=disabled [default], 1=enabled)*/
#define PLCC_PN_USER					"User"						/* V3: UserManagment on PLC: User */
#define PLCC_PN_PASSWORD				"Password"					/* V3: UserManagment on PLC: Password */
#define PLCC_PN_CLIENT_ADDR_RES			"ClientAddressResolution"	/* V3: Symbol address resolution in the client (0=disabled [default], 1=enabled) */
#define PLCC_PN_DONT_EXPAND_ARRAYS		"DontExpandSimpleTypeArrays"/* V3: Arraylements of arrays of simple types are supressed (0=disabled [default], 1=enabled) */
#define PLCC_PN_DONT_EXPAND_COMPLEX_ARRAYS	"DontExpandComplexTypeArrays"/* V3: Arraylements of arrays of complex types are supressed (0=disabled [default], 1=enabled) */
#define PLCC_PN_DATA_CHANGE_ON_PLC		"CheckDataChangeOnPlc"		/* V3: Data change is evaluated in the PLC, and not in the PLCHandler (0=disabled [default], 1=enabled)*/
#define PLCC_PN_DONT_LOAD_SYMBOLS		"DontLoadSymbolsFromPlc"	/* V3: Supress symbol load, but still allow symbolic access to the PLC (0=disabled [default], 1=enabled)*/
#define PLCC_PN_WRITE_THROUGH_CACHE		"WriteThroughReadCache"		/* All: SyncWrite change also the cache of the CycLists (0=disabled [default], 1=enabled) */
#define PLCC_PN_ALTERNATE_SYMBOLFILE	"AllowAlternateSymbolfile"	/* GATEWAY: Use sdb-file from Gateway without file compare with PLC (ProjectID must match) (0=disabled [default], 1=enabled) */
#define PLCC_PN_CONFIG_IS_OVERLOADABLE	"ConfigIsOverloadable"		/* All: Config set with next call of SetConfig is merged with the current one. (0=disabled [default], 1=enabled) */
#define PLCC_PN_DEVICE_NAME_IN_PROJECT	"DeviceNameInProject"		/* V3: Connect to CODESYS-Simulation: DeviceName of the PLC inside the CODESYS project */
#define PLCC_PN_RESOLVE_PLCNAME_TO		"ResolvePlcNameTimeout"		/* V3: Connect by Nodename only: Time in ms while PLCHandler waits for ResolveName answers from the PLC */
#define PLCC_PN_DRIVER_MASK				"DriverMask"				/* ARTI3: Mask for drivers, which should be enabled (default=0xFFFFFFFF, all drivers enabled). For driver list see CmpCommunicationLibItf.h */
#define PLCC_PN_USE_LOCAL_TIME_LOGGER	"UseLocalTimeInLogfile"		/* All: Logfile: Write timestamps as local time instead of UTC (0=UTC [default], 1=LocalTime) */
#define PLCC_PN_RECALL_ADDR_INFO		"RecallAddressInformation"	/* V3: The PLCHandler retrieves the address information on the first access and recalls it on further accesses (0=disabled [default], 1=enabled) */
#define PLCC_PN_DEVICE_NAME				"DeviceName"				/* V3: CODESYS node name as UTF-16 WString */
#define PLCC_PN_USE_MONITORING_SERVICES	"UseMonitoringServices"		/* V3: Use CmpMonitoring2 services instead of CmpIecVarAccess services to access symbols (0=disabled [default], 1=enabled)*/
#define PLCC_PN_APPLICATION_NAME		"ApplicationName"			/* V3: UseMonitoringServices=1: If set, the PLCHandler tries to load the XML symbolfile for the specified application from the PLC */
#define PLCC_PN_ENCRYPT_COMMUNICATION	"EncryptCommunication"		/* V3: EncryptCommunication=1: If set, the PLCHandler establishes a TLS secured connection to the PLC */
#define PLCC_PN_CIPHER_SUITE			"CipherSuite"				/* V3: Defines the list of allowed ciphers for secure connections */
#define PLCC_PN_ADDRESS_TYPE			"AddressType"				/* V3: Parameter Address is explicitly a logical 'address' or a PLC 'name' */
#define PLCC_PN_CONNECT_ONLY			"ConnectOnly"				/* ALL: Only a connect is done without sending any other services (e.g. login). Can be used for direct communication through SyncSendService to the device. */
#define PLCC_PN_SEND_KEEP_ALIVE			"SendKeepAlive"				/* ALL: Configure the automatic keep alive online services (1=keep alive is sent [default], 0=no keep alive is sent) */

/* Defines for AddressType */
#define PLCC_PN_ADDR_TYPE_NAME			"name"
#define PLCC_PN_ADDR_TYPE_ADDRESS		"address"
#define PLCC_ADDR_TYPE_UNKNOWN		0
#define PLCC_ADDR_TYPE_NAME			1
#define PLCC_ADDR_TYPE_ADDRESS		2

/* Defines for ABB parameter names */
#define PLCC_PN_ROUTINGLEVELS		"RoutingLevels"
#define PLCC_PN_LEVEL1KOPPLER		"Level1Koppler"
#define PLCC_PN_LEVEL1CHANNEL		"Level1Channel"
#define PLCC_PN_LEVEL1ADDRESS		"Level1Address"
#define PLCC_PN_LEVEL2KOPPLER		"Level2Koppler"
#define PLCC_PN_LEVEL2CHANNEL		"Level2Channel"
#define PLCC_PN_LEVEL2ADDRESS		"Level2Address"

/* Defines for parameter id's - !!! obsolete !!! */
#define PLCC_PID_ADDRESS			1001
#define PLCC_PID_PORT				1000
#define PLCC_PID_TARGET				1002
#define PLCC_PID_BLOCKSIZE			1002
#define PLCC_PID_MOTOROLA			1003
#define PLCC_PID_SYMBOLFILE			1005
#define PLCC_PID_BAUDRATE			1001
#define PLCC_PID_PARITY				1002
#define PLCC_PID_STOPBITS			1003
#define PLCC_PID_PING				1004
#define PLCC_PID_SYMBOLFILE_PATH	1006
#define PLCC_PID_DIRECT_ADDR_ONLY	1007
#define PLCC_PID_CLIENT_ADDR_RES	1008

#ifndef TRG_64BIT 
	#ifdef USE_PRAGMA_PACK_0
		#pragma pack(0)
	#else
		#pragma pack()
	#endif	
#endif

#endif	/*__PLCCONFIG_H__*/
