#ifndef __PLCHANDLERITF_H__
#define __PLCHANDLERITF_H__

#include "CmpStd.h"
#include "PLCHandlerDefines.h"	/* Basic definitions of used types, enums, options, flags ... */
#include "PLCConfig.h"
#include "CmpPLCHandlerDep.h"

#undef PLCH_DLL_DECL
#if defined(PLCH_USE_DLL) && (defined(WIN32) || defined(_WIN32_WCE) || defined(_WIN32_WCE_EMULATION))
	#ifdef _USRDLL
		#define PLCH_DLL_DECL _declspec(dllexport)
	#else
		#define PLCH_DLL_DECL _declspec(dllimport)
	#endif
#else
	#define PLCH_DLL_DECL
#endif

#ifndef TRG_64BIT 
	/* needed for compatibility because PlcParameterValue (8 byte) is misaligned in PlcParameter structure (4 byte offset) */
	#pragma pack (4)						
#endif

typedef struct GatewayConnectionStructTag
{
	char*		pszDeviceName;					/* name of the gateway device (protocol) */
	char*		pszAddress;						/* TCP/IP address of the gateway connection */
	unsigned long ulPort;						/* TCP/IP port of the gateway connection */
	char*		pszPassword;					/* Optional gateway password */
} GatewayConnectionStruct;

typedef struct PlcConfigStructTag
{
	unsigned long		ulId;					/* unique ID for every PLC */
	char				*pszName;				/* name of the PLC */
	ItfType				it;						/* interface type of the PLC (IT_ARTI, IT_GATEWAY, ...) */
	char				bActive;				/* flag if the PLC is active or not */
	char				bMotorola;				/* flag if data has to be swapt for motorola byte order */
	char				bLogin;					/* flag if the application can do a login */
	char				bLogToFile;				/* flag if the application should log to a file */
	char				bPreCheckIdentity;		/* flag if the identity will be checked before every write/read */
	unsigned long		ulTimeout;				/* communication timeout in ms */
	unsigned long		ulNumTries;				/* number of tries of receive's before throwing an COMM_FATAL error */
	unsigned long		ulWaitTime;				/* time in ms to wait for a reconnection to the PLC before Create() throws an error */
	unsigned long		ulReconnectTime;		/* time interval in ms to try for a reconnection */
	char				*pszHwType;				/* HW type of the PLC (PLCC_HW_STANDARD, PLCC_HW_MAX4) */
	unsigned long		ulHwVersion;			/* HW version  */
	unsigned long		ulBufferSize;			/* communication buffersize of the runtime system running on the PLC; 0 = default size */
	char				*pszProjectName;		/* name of the CoDeSys project running on the runtime system */
	char				*pszDllDirectory;		/* directory where to find DLLs (only for very special cases) */
	GatewayConnectionStruct	*gwc;				/* pointer to the gateway connection if using the gateway interface */
	unsigned long		ulLogFilter;			/* Filter for log actions */
} PlcConfigStruct;

typedef union PlcParameterValueUnionTag			/* various values for the device parameters */
{
	unsigned long		dw;
	float				r;
	double				lr;
	char				*psz;
} PlcParameterValueUnion;

typedef struct PlcParameterStructTag
{
	PlcParameterType	Type;					/* type of the parameter */
	PlcParameterValueUnion	Value;				/* value of the parameter */
} PlcParameterStruct;

typedef struct PlcParameterDescStructTag
{
	unsigned long	ulId;						/* unique id for the parameter */
	char*			pszName;					/* name of the parameter */
	PlcParameterStruct*	pParameter;				/* type and value of the parameter */
} PlcParameterDescStruct;

/* Device description */
typedef struct PlcDeviceDescStructTag
{
	char*				pszName;				/* name of the device / protocol */
	char*				pszInstance;			/* instance name of the device */
	char*				pszProject;				/* not used - see PlcConfig.pszProjectName instead  */
	unsigned long		ulNumParams;			/* number of the device parameters */
	PlcParameterDescStruct*	ppd;				/* pointer to an array of parameter descriptions */
} PlcDeviceDescStruct;

#ifndef TRG_64BIT 
	#ifdef USE_PRAGMA_PACK_0
		#pragma pack(0)
	#else
		#pragma pack()
	#endif	
#endif

typedef struct PlcSymbolDescStructTag
{
	char*			pszName;
	unsigned long	ulTypeId;		/* Type class. HIWORD/LOWORD see definitions above */
	char*			pszType;
	unsigned short	usRefId;
	unsigned long	ulOffset;
	unsigned long	ulSize;
	char			szAccess[2];
	unsigned char	bySwapSize;
} PlcSymbolDescStruct;

typedef struct PlcVarValueStructTag
{
	unsigned long	ulTimeStamp;
	unsigned char	bQuality;
	unsigned char	byData[1];
} PlcVarValueStruct;

#ifndef HCYCLIST
	typedef void* HCYCLIST;
#endif

#ifndef HVARLIST
	typedef void* HVARLIST;
#endif

typedef enum
{
	RTS_STATE_RUNNNING		= 0,
	RTS_STATE_STOP			= 1,
	RTS_STATE_STOP_ON_BP	= 2,
	RTS_STATE_UNKNOWN		= 255
} PLC_STATUS_ENUM;

typedef enum
{
	RTS_RESET_WARM			= 0,
	RTS_RESET_COLD			= 1,
	RTS_RESET_ORIGIN		= 2
} RESET_OPTION_ENUM;


#if __cplusplus
extern "C" {
#endif

typedef void(*PLCHANDLERSCANNETWORKCALLBACK)(RTS_UINTPTR ulPLCHandler, NodeInfotyp2 *pNodeInfo2);
typedef void(*PLCHANDLERBACKUPRESTORERESULTCALLBACK)(RTS_UINTPTR ulPLCHandler, long lResult);
typedef long(*PLCHANDLERVERIFYPLCCERTCALLBACK)(RTS_UINTPTR ulPLCHandler, char* pBase64PlcCert, unsigned long ulBase64PlcCertSize, long lVerifyResult);
typedef long(*PLCHANDLERSTATECHANGECALLBACK)(RTS_UINTPTR ulPLCHandler, long lNewState);

PLCH_DLL_DECL RTS_UINTPTR PLCHandlerInit(PlcConfigStruct *pPlcConfig, PlcDeviceDescStruct *pDeviceDesc, char *pszLogFile);
typedef PLCH_DLL_DECL RTS_UINTPTR (*PFPLCHANDLERINIT)(PlcConfigStruct *pPlcConfig, PlcDeviceDescStruct *pDeviceDesc, char *pszLogFile);

PLCH_DLL_DECL RTS_UINTPTR PLCHandlerInitByFile(unsigned long ulId, char *pszIniFile, char *pszLogFile);
typedef PLCH_DLL_DECL RTS_UINTPTR(*PFPLCHANDLERINITBYFILE)(unsigned long ulId, char *pszIniFile, char *pszLogFile);

PLCH_DLL_DECL RTS_UINTPTR PLCHandlerInitByFilePlcName(char *pszPlcName, char *pszIniFile, char *pszLogFile);
typedef PLCH_DLL_DECL RTS_UINTPTR(*PFPLCHANDLERINITBYFILEPLCNAME)(char *pszPlcName, char *pszIniFile, char *pszLogFile);

PLCH_DLL_DECL RTS_UINTPTR PLCHandlerInit2(ItfType it, char *pszLogFile);
typedef PLCH_DLL_DECL RTS_UINTPTR(*PFPLCHANDLERINIT2)(ItfType it, char *pszLogFile);

PLCH_DLL_DECL long PLCHandlerExit(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLEREXIT)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL int PLCHandlerSetLogging(RTS_UINTPTR ulPLCHandler, int bEnable, unsigned long ulLogFilter);
typedef PLCH_DLL_DECL int(*PFPLCHANDLERSETLOGGING)(RTS_UINTPTR ulPLCHandler, int bEnable, unsigned long ulLogFilter);

PLCH_DLL_DECL int PLCHandlerGetLogging(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL int(*PFPLCHANDLERGETLOGGING)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerSetLogFileCapacity(RTS_UINTPTR ulPLCHandler, int iMaxFileSize, int iMaxFiles);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERSETLOGFILECAPACITY)(RTS_UINTPTR ulPLCHandler, int iMaxFileSize, int iMaxFiles);

PLCH_DLL_DECL long PLCHandlerSetLogFile(RTS_UINTPTR ulPLCHandler, char *pszLogFile);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERSETLOGFILE)(RTS_UINTPTR ulPLCHandler, char *pszLogFile);

PLCH_DLL_DECL long PLCHandlerAddLogEntry(RTS_UINTPTR ulPLCHandler, unsigned long CmpId, int iClassID, int iErrorID, char *pszInfo, ...);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERADDLOGENTRY)(RTS_UINTPTR ulPLCHandler, unsigned long CmpId, int iClassID, int iErrorID, char *pszInfo, ...);

PLCH_DLL_DECL long PLCHandlerAddLogEntryArg(RTS_UINTPTR ulPLCHandler, unsigned long CmpId, int iClassID, int iErrorID, char *pszInfo, va_list *pargList);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERADDLOGENTRYARG)(RTS_UINTPTR ulPLCHandler, unsigned long CmpId, int iClassID, int iErrorID, char *pszInfo, va_list *pargList);

PLCH_DLL_DECL long PLCHandlerScanNetwork(RTS_UINTPTR ulPLCHandler, GatewayConnectionStruct *pGatewayConnection, PLCHANDLERSCANNETWORKCALLBACK pfPlcFoundCallback);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERSCANNETWORK)(RTS_UINTPTR ulPLCHandler, GatewayConnectionStruct *pGatewayConnection, PLCHANDLERSCANNETWORKCALLBACK pfPlcFoundCallback);

PLCH_DLL_DECL long PLCHandlerSetConfigInteractive(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSETCONFIGINTERACTIVE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerSetConfig(RTS_UINTPTR ulPLCHandler, PlcConfigStruct *pPlcConfig, PlcDeviceDescStruct *pDeviceDesc);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSETCONFIG)(RTS_UINTPTR ulPLCHandler, PlcConfigStruct *pPlcConfig, PlcDeviceDescStruct *pDeviceDesc);

PLCH_DLL_DECL long PLCHandlerSetConfigByString(RTS_UINTPTR ulPLCHandler, char *pszConfig, unsigned long ulConfigLen, char *pszLineEnd);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSETCONFIGBYSTRING)(RTS_UINTPTR ulPLCHandler, char *pszConfig, unsigned long ulConfigLen, char *pszLineEnd);

PLCH_DLL_DECL long PLCHandlerSetConfigByFile(RTS_UINTPTR ulPLCHandler, char *pszIniFile);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERSETCONFIGBYFILE)(RTS_UINTPTR ulPLCHandler, char *pszIniFile);

PLCH_DLL_DECL long PLCHandlerSetConnectionCallbacks(RTS_UINTPTR ulPLCHandler, PLCHANDLERSTATECHANGECALLBACK pfStateChangeCallback, PLCHANDLERVERIFYPLCCERTCALLBACK pfVerifyPlcCertCallback);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSETCONNECTIONCALLBACKS)(RTS_UINTPTR ulPLCHandler, PLCHANDLERSTATECHANGECALLBACK pfStateChangeCallback, PLCHANDLERVERIFYPLCCERTCALLBACK pfVerifyPlcCertCallback);

PLCH_DLL_DECL long PLCHandlerGetConfig(RTS_UINTPTR ulPLCHandler, PlcConfigStruct **ppPlcConfig, PlcDeviceDescStruct **ppDeviceDesc);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETCONFIG)(RTS_UINTPTR ulPLCHandler, PlcConfigStruct **ppPlcConfig, PlcDeviceDescStruct **ppDeviceDesc);

PLCH_DLL_DECL long PLCHandlerSaveConfigInFile(RTS_UINTPTR ulPLCHandler, char *pszIniFile);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSAVECONFIGINFILE)(RTS_UINTPTR ulPLCHandler, char *pszIniFile);

PLCH_DLL_DECL long PLCHandlerConnect(RTS_UINTPTR ulPLCHandler, unsigned long ulTimeout, RTS_UINTPTR hStateChangedEvent, int bLoadSymbols);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECT)(RTS_UINTPTR ulPLCHandler, unsigned long ulTimeout, RTS_UINTPTR hStateChangedEvent, int bLoadSymbols);

PLCH_DLL_DECL long PLCHandlerConnectTcpipViaGateway(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, char *pszPlcIP, char *pszProtocol, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, unsigned long ulPort);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTTCPIPVIAGATEWAY)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, char *pszPlcIP, char *pszProtocol, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, unsigned long ulPort);

PLCH_DLL_DECL long PLCHandlerConnectRs232ViaGateway(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTRS232VIAGATEWAY)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectRs232ViaGatewayEx(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, EXT_RS232_PARAMStyp *pExtParams);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTRS232VIAGATEWAYEX)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, EXT_RS232_PARAMStyp *pExtParams);

PLCH_DLL_DECL long PLCHandlerConnectTcpipViaArti(RTS_UINTPTR ulPLCHandler, char *pszPlcIP, char *pszProtocol, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, unsigned long ulPort);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTTCPIPVIAARTI)(RTS_UINTPTR ulPLCHandler, char *pszPlcIP, char *pszProtocol, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, unsigned long ulPort);

PLCH_DLL_DECL long PLCHandlerConnectRs232ViaArti(RTS_UINTPTR ulPLCHandler, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTRS232VIAARTI)(RTS_UINTPTR ulPLCHandler, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectToSimulation(RTS_UINTPTR ulPLCHandler, char *pszSdbFile, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTTOSIMULATION)(RTS_UINTPTR ulPLCHandler, char *pszSdbFile, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaGateway3(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTVIAGATEWAY3)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaGateway3Ex(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTVIAGATEWAY3EX)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaGateway3ByName(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, RTS_WCHAR *pwszDeviceName, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERCONNECTVIAGATEWAY3BYNAME)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, RTS_WCHAR *pwszDeviceName, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaArti3(RTS_UINTPTR ulPLCHandler, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTVIAARTI3)(RTS_UINTPTR ulPLCHandler, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaArti3ByName(RTS_UINTPTR ulPLCHandler, RTS_WCHAR *pwszDeviceName, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERCONNECTVIAARTI3BYNAME)(RTS_UINTPTR ulPLCHandler, RTS_WCHAR *pwszDeviceName, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectToSimulation3(RTS_UINTPTR ulPLCHandler, char *pszSdb3XmlFile, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTTOSIMULATION3)(RTS_UINTPTR ulPLCHandler, char *pszSdb3XmlFile, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectTcpipViaGateway3(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, char *pszPlcIP, unsigned long ulPlcPort, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTTCPIPVIAGATEWAY3)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, char *pszPlcIP, unsigned long ulPlcPort, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectTcpipViaArti3(RTS_UINTPTR ulPLCHandler, char *pszPlcIP, unsigned long ulPlcPort, int bLoadSymbols, unsigned long ulTimeout);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCONNECTTCPIPVIAARTI3)(RTS_UINTPTR ulPLCHandler, char *pszPlcIP, unsigned long ulPlcPort, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerDisconnect(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERDISCONNECT)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL unsigned long PLCHandlerGetVersion(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL unsigned long (*PFPLCHANDLERGETVERSION)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL int PLCHandlerGetState(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL int (*PFPLCHANDLERGETSTATE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL char *PLCHandlerGetName(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL char *(*PFPLCHANDLERGETNAME)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerStartKeeplive(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSTARTKEEPLIVE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerStopKeeplive(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSTOPKEEPLIVE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerLoadSymbolsFromPlc(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERLOADSYMBOLSFROMPLC)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerGetAllItems(RTS_UINTPTR ulPLCHandler, PlcSymbolDescStruct **ppSymbolList, unsigned long *pulNumOfSymbols);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETALLITEMS)(RTS_UINTPTR ulPLCHandler, PlcSymbolDescStruct **ppSymbolList, unsigned long *pulNumOfSymbols);

PLCH_DLL_DECL long PLCHandlerEnterItemAccess(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERENTERITEMACCESS)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerLeaveItemAccess(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERLEAVEITEMACCESS)(RTS_UINTPTR ulPLCHandler);

/* Get the definition of an item specified by name. The call of this method and the access of pSymbol must be
   protected by PLCHandlerEnterItemAccess() and PLCHandlerLeaveItemAccess(). */
PLCH_DLL_DECL long PLCHandlerGetItem(RTS_UINTPTR ulPLCHandler, char *pszSymbol, PlcSymbolDescStruct *pSymbol);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETITEM)(RTS_UINTPTR ulPLCHandler, char *pszSymbol, PlcSymbolDescStruct *pSymbol);

PLCH_DLL_DECL long PLCHandlerGetAddressOfMappedItem(RTS_UINTPTR ulPLCHandler, char *pszSymbol, char *pszMappedAddr, long lLen);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETADDRESSOFMAPPEDITEM)(RTS_UINTPTR ulPLCHandler, char *pszSymbol, char *pszMappedAddr, long lLen);

/* Expand an item (if this is an Array or UserDef type) on the fly. The call of this method must be
   protected by PLCHandlerEnterItemAccess() and PLCHandlerLeaveItemAccess().
   pSymbolList must be released with PLCHandlerReleaseExpandedItems() after usage!
   Returns an error, if it is a simple type */
PLCH_DLL_DECL long PLCHandlerExpandItem(RTS_UINTPTR ulPLCHandler, char *pszSymbol, PlcSymbolDescStruct **ppSymbolList, unsigned long *pulNumOfSymbols);
typedef PLCH_DLL_DECL long (*PFPLCHANDLEREXPANDITEM)(RTS_UINTPTR ulPLCHandler, char *pszSymbol, PlcSymbolDescStruct **ppSymbolList, unsigned long *pulNumOfSymbols);

/* Function to release dynamic expanded item list */
PLCH_DLL_DECL long PLCHandlerReleaseExpandedItems(RTS_UINTPTR ulPLCHandler, PlcSymbolDescStruct *pSymbolList);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERRELEASEEXPANDEDITEMS)(RTS_UINTPTR ulPLCHandler, PlcSymbolDescStruct *pSymbolList);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycDefineVarList(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent);
typedef PLCH_DLL_DECL HCYCLIST (*PFPLCHANDLERCYCDEFINEVARLIST)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycDefineVarList2(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent, unsigned long ulFlags, long *plResult);
typedef PLCH_DLL_DECL HCYCLIST(*PFPLCHANDLERCYCDEFINEVARLIST2)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent, unsigned long ulFlags, long *plResult);

PLCH_DLL_DECL long PLCHandlerCycDeleteVarList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCYCDELETEVARLIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

/* Enable the list: enable cyclic update */
PLCH_DLL_DECL long PLCHandlerCycEnableList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCYCENABLELIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

/* Disable the list: disable cyclic update */
PLCH_DLL_DECL long PLCHandlerCycDisableList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCYCDISABLELIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycUpdateVarList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent);
typedef PLCH_DLL_DECL HCYCLIST (*PFPLCHANDLERCYCUPDATEVARLIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycUpdateVarList2(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent, unsigned long ulFlags, long *plResult);
typedef PLCH_DLL_DECL HCYCLIST(*PFPLCHANDLERCYCUPDATEVARLIST2)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent, unsigned long ulFlags, long *plResult);

PLCH_DLL_DECL void PLCHandlerCycEnterVarAccess(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef PLCH_DLL_DECL void (*PFPLCHANDLERCYCENTERVARACCESS)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

PLCH_DLL_DECL long PLCHandlerCycReadVars(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCYCREADVARS)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);

PLCH_DLL_DECL void PLCHandlerCycLeaveVarAccess(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef PLCH_DLL_DECL void (*PFPLCHANDLERCYCLEAVEVARACCESS)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

/* Get the update rate of the cyclic list. */
PLCH_DLL_DECL unsigned long PLCHandlerCycGetUpdateRate(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef PLCH_DLL_DECL unsigned long (*PFPLCHANDLERCYCGETUPDATERATE)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

/* Set the update rate of the cyclic list. */
PLCH_DLL_DECL long PLCHandlerCycSetUpdateRate(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, unsigned long ulUpdateRate);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCYCSETUPDATERATE)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, unsigned long ulUpdateRate);

PLCH_DLL_DECL long PLCHandlerCycGetSymbolList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char ***pppCycSymbolList);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCYCGETSYMBOLLIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char ***pppCycSymbolList);

PLCH_DLL_DECL unsigned long PLCHandlerCycGetOperatingRate(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef PLCH_DLL_DECL unsigned long (*PFPLCHANDLERCYCGETOPERATINGRATE)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

PLCH_DLL_DECL HVARLIST PLCHandlerSyncDefineVarList(RTS_UINTPTR ulPLCHandler, char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned long ulFlags, long *plResult);
typedef PLCH_DLL_DECL HVARLIST(*PFPLCHANDLERSYNCDEFINEVARLIST)(RTS_UINTPTR ulPLCHandler, char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned long ulFlags, long *plResult);

PLCH_DLL_DECL long PLCHandlerSyncDeleteVarList(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERSYNCDELETEVARLIST)(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList);

PLCH_DLL_DECL long PLCHandlerSyncReadVarListFromPlc(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERSYNCREADVARLISTFROMPLC)(RTS_UINTPTR ulPLCHandler, HVARLIST hVarList, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);

PLCH_DLL_DECL long PLCHandlerSyncWriteVarListToPlc(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList, unsigned long ulNumOfSymbols, unsigned char **ppbyValues, unsigned long *pulValueSizes);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERSYNCWRITEVARLISTTOPLC)(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList, unsigned long ulNumOfSymbols, unsigned char **ppbyValues, unsigned long *pulValueSizes);

PLCH_DLL_DECL HVARLIST PLCHandlerSyncReadVarsFromPlc(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);
typedef PLCH_DLL_DECL HVARLIST (*PFPLCHANDLERSYNCREADVARSFROMPLC)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);

PLCH_DLL_DECL long PLCHandlerSyncReadVarsFromPlcReleaseValues(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncRead);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSYNCREADVARSFROMPLCRELEASEVALUES)(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncRead);

PLCH_DLL_DECL long PLCHandlerSyncWriteVarsToPlc(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char **ppbyValues);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSYNCWRITEVARSTOPLC)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char **ppbyValues);

PLCH_DLL_DECL long PLCHandlerSyncWriteVarsToPlc2(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char **ppbyValues, unsigned long *pulValueSizes);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSYNCWRITEVARSTOPLC2)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char **ppbyValues, unsigned long *pulValueSizes);

PLCH_DLL_DECL long PLCHandlerUploadFile(RTS_UINTPTR ulPLCHandler, char *pszPlc, char *pszHost);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERUPLOADFILE)(RTS_UINTPTR ulPLCHandler, char *pszPlc, char *pszHost);

PLCH_DLL_DECL long PLCHandlerDownloadFile(RTS_UINTPTR ulPLCHandler, char *pszHost, char *pszPlc);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERDOWNLOADFILE)(RTS_UINTPTR ulPLCHandler, char *pszHost, char *pszPlc);

PLCH_DLL_DECL long PLCHandlerReloadBootproject(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERRELOADBOOTPROJECT)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerRegisterBootApplication(RTS_UINTPTR ulPLCHandler, char *pszApplication);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERREGISTERBOOTAPPLICATION)(RTS_UINTPTR ulPLCHandler, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerReloadBootApplication(RTS_UINTPTR ulPLCHandler, char *pszApplication);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERRELOADBOOTAPPLICATION)(RTS_UINTPTR ulPLCHandler, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerSaveRetains(RTS_UINTPTR ulPLCHandler, char *pszRetainFile, long lBufferLen, char *pszApplication);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSAVERETAINS)(RTS_UINTPTR ulPLCHandler, char *pszRetainFile, long lBufferLen, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerRestoreRetains(RTS_UINTPTR ulPLCHandler, char *pszRetainFile, long lBufferLen, char *pszApplication);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERRESTORERETAINS)(RTS_UINTPTR ulPLCHandler, char *pszRetainFile, long lBufferLen, char *pszApplication);

/* Send echo service to PLC e. g. to measure the communication performance (only for V3) */
PLCH_DLL_DECL long PLCHandlerSendPlcEcho(RTS_UINTPTR ulPLCHandler, unsigned long *pulSendDataLen, unsigned long *pulReceiveDataLen);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSENDPLCECHO)(RTS_UINTPTR ulPLCHandler, unsigned long *pulSendDataLen, unsigned long *pulReceiveDataLen);

PLCH_DLL_DECL long PLCHandlerCheckTarget(RTS_UINTPTR ulPLCHandler, unsigned long ulTargetId, unsigned long ulHookId, unsigned long ulMagic);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERCHECKTARGET)(RTS_UINTPTR ulPLCHandler, unsigned long ulTargetId, unsigned long ulHookId, unsigned long ulMagic);

PLCH_DLL_DECL long PLCHandlerGetProjectInfo(RTS_UINTPTR ulPLCHandler, ProjectInfoStruct **ppProjectInfo);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETPROJECTINFO)(RTS_UINTPTR ulPLCHandler, ProjectInfoStruct **ppProjectInfo);

PLCH_DLL_DECL long PLCHandlerGetApplicationList(RTS_UINTPTR ulPLCHandler, char ***pppszApplications, unsigned long *pulNumOfApplications);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETAPPLICATIONLIST)(RTS_UINTPTR ulPLCHandler, char ***pppszApplications, unsigned long *pulNumOfApplications);

PLCH_DLL_DECL long PLCHandlerCheckApplicationFileConsistency(RTS_UINTPTR ulPLCHandler, char *pszApplication, long *plBootProjectResult, long *plArchiveResult);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERCHECKAPPLICATIONFILECONSISTENCY)(RTS_UINTPTR ulPLCHandler, char *pszApplication, long *plBootProjectResult, long *plArchiveResult);

PLCH_DLL_DECL long PLCHandlerGetApplicationInfo(RTS_UINTPTR ulPLCHandler, char *pszApplication, ProjectInfoStruct **ppPrjInfo, ApplicationInfoStruct **ppAppInfo);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETAPPLICATIONINFO)(RTS_UINTPTR ulPLCHandler, char *pszApplication, ProjectInfoStruct **ppPrjInfo, ApplicationInfoStruct **ppAppInfo);

PLCH_DLL_DECL long PLCHandlerGetApplicationInfo2(RTS_UINTPTR ulPLCHandler, char *pszApplication, ProjectInfoStruct **ppPrjInfo, ApplicationInfoStruct2 **ppAppInfo);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERGETAPPLICATIONINFO2)(RTS_UINTPTR ulPLCHandler, char *pszApplication, ProjectInfoStruct **ppPrjInfo, ApplicationInfoStruct2 **ppAppInfo);

PLCH_DLL_DECL long PLCHandlerGetDeviceInfo(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct **ppDeviceInfo);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETDEVICEINFO)(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct **ppDeviceInfo);

PLCH_DLL_DECL long PLCHandlerGetDeviceInfo2(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct2 **ppDeviceInfo);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERGETDEVICEINFO2)(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct2 **ppDeviceInfo);

PLCH_DLL_DECL long PLCHandlerSyncSendService(RTS_UINTPTR ulPLCHandler, unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSYNCSENDSERVICE)(RTS_UINTPTR ulPLCHandler, unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize);

PLCH_DLL_DECL long PLCHandlerGetLastError(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETLASTERROR)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerGetPlcStatus(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM *pPlcStatus);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETPLCSTATUS)(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM *pPlcStatus);

PLCH_DLL_DECL long PLCHandlerSetPlcStatus(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM PlcStatus);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSETPLCSTATUS)(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM PlcStatus);

PLCH_DLL_DECL long PLCHandlerResetPlc(RTS_UINTPTR ulPLCHandler, RESET_OPTION_ENUM ResetCommand);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERRESETPLC)(RTS_UINTPTR ulPLCHandler, RESET_OPTION_ENUM ResetCommand);

PLCH_DLL_DECL long PLCHandlerGetApplicationStatus(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM *pAppStatus, char *pszApplication);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERGETAPPLICATIONSTATUS)(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM *pAppStatus, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerSetApplicationStatus(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM AppStatus, char *pszApplication);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERSETAPPLICATIONSTATUS)(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM AppStatus, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerResetApplication(RTS_UINTPTR ulPLCHandler, RESET_OPTION_ENUM ResetCommand, char *pszApplication);
typedef PLCH_DLL_DECL long (*PFPLCHANDLERRESETAPPLICATION)(RTS_UINTPTR ulPLCHandler, RESET_OPTION_ENUM ResetCommand, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerResetOriginDevice(RTS_UINTPTR ulPLCHandler);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERRESETORIGINDEVICE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerGetDeviceOperationMode(RTS_UINTPTR ulPLCHandler, DEVICE_OPERATION_MODE *pOpMode);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERGETDEVICEOPERATIONMODE)(RTS_UINTPTR ulPLCHandler, DEVICE_OPERATION_MODE *pOpMode);

PLCH_DLL_DECL long PLCHandlerSetDeviceOperationMode(RTS_UINTPTR ulPLCHandler, DEVICE_OPERATION_MODE OpMode);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERSETDEVICEOPERATIONMODE)(RTS_UINTPTR ulPLCHandler, DEVICE_OPERATION_MODE OpMode);

PLCH_DLL_DECL long PLCHandlerRenameDevice(RTS_UINTPTR ulPLCHandler, RTS_WCHAR *pwszNodeName);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERRENAMEDEVICE)(RTS_UINTPTR ulPLCHandler, RTS_WCHAR *pwszNodeName);

PLCH_DLL_DECL long PLCHandlerBackupIECApplications(RTS_UINTPTR ulPLCHandler, char *pszBackupFilePath, PLCHANDLERBACKUPRESTORERESULTCALLBACK pfBackupResultCallback, int bForceBackup, int bCreateTbf);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERBACKUPIECAPPLICATIONS)(RTS_UINTPTR ulPLCHandler, char *pszBackupFilePath, PLCHANDLERBACKUPRESTORERESULTCALLBACK pfBackupResultCallback, int bForceBackup, int bCreateTbf);

PLCH_DLL_DECL long PLCHandlerRestoreIECApplications(RTS_UINTPTR ulPLCHandler, char *pszRestoreFilePath, PLCHANDLERBACKUPRESTORERESULTCALLBACK pfRestoreResultCallback, int bStartBootprojects);
typedef PLCH_DLL_DECL long(*PFPLCHANDLERRESTOREIECAPPLICATIONS)(RTS_UINTPTR ulPLCHandler, char *pszRestoreFilePath, PLCHANDLERBACKUPRESTORERESULTCALLBACK pfRestoreResultCallback, int bStartBootprojects);

#if __cplusplus
}
#endif


#endif /*__PLCHANDLERITF_H__*/
