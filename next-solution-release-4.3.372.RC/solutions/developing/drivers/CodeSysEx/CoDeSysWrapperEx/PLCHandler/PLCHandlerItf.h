#ifndef __PLCHANDLERITF_H__
#define __PLCHANDLERITF_H__

#include "CmpStd.h"
#include "PLCHandlerDefines.h"	/* Basic definitions of used types, enums, options, flags ... */
#include "PLCConfig.h"
#ifndef CMPLPLCHANDLERMAIN_NOTIMPLEMENTED
	#include "CmpPLCHandlerDep.h"
#endif

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

typedef struct GatewayConnectionStructTag
{
	char*		pszDeviceName;					/* Name of the gateway device/protocol ("Tcp/Ip", "Local", "SharedMemory", "NamedSharedMemory", ...) */
	char*		pszAddress;						/* Tcp/Ip address or shared memory name of the gateway */
	unsigned long ulPort;						/* Tcp/Ip port of the gateway */
	char*		pszPassword;					/* Optional gateway password */
} GatewayConnectionStruct;

typedef struct PlcConfigStructTag
{
	unsigned long		ulId;					/* unique ID for every PLC */
	char				*pszName;				/* name of the PLC */
	ItfType				it;						/* interface type of the PLC (IT_ARTI, IT_GATEWAY, ...) */
	char				bActive;				/* flag if the PLC is active or not */
	char				bMotorola;				/* flag if data has to be swapped for motorola byte order */
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


#if __cplusplus
extern "C" {
#endif

typedef void(*PLCHANDLERSCANNETWORKCALLBACK)(RTS_UINTPTR ulPLCHandler, NodeInfotyp2 *pNodeInfo2);
typedef long(*PLCHANDLERSCANNETWORKCALLBACK2)(RTS_UINTPTR ulPLCHandler, NodeInfotyp2 *pNodeInfo2, void *pScanCstData);

typedef void(*PLCHANDLERBACKUPRESTORERESULTCALLBACK)(RTS_UINTPTR ulPLCHandler, long lResult);

typedef long(*PLCHANDLERVERIFYPLCCERTCALLBACK)(RTS_UINTPTR ulPLCHandler, char *pBase64PlcCert, unsigned long ulBase64PlcCertSize, long lVerifyResult);
typedef long(*PLCHANDLERVERIFYPLCCERTCALLBACK2)(RTS_UINTPTR ulPLCHandler, char *pBase64PlcCert, unsigned long ulBase64PlcCertSize, long lVerifyResult, void *pVerifyCstData);

typedef long(*PLCHANDLERREQUESTCREDENTIALSCALLBACK)(RTS_UINTPTR ulPLCHandler, RequestCredentialsType requestCredentialsType, RTS_UTF8STRING * const pUserName, unsigned long userNameBufSize, RTS_UTF8STRING * const pPassword, unsigned long passwordBufSize, void * const pRequestCredentialsCstData);

typedef long(*PLCHANDLERSTATECHANGECALLBACK)(RTS_UINTPTR ulPLCHandler, long lNewState);
typedef long(*PLCHANDLERSTATECHANGECALLBACK2)(RTS_UINTPTR ulPLCHandler, long lNewState, void *pStateCstData);

typedef long(*PLCHANDLERREADAPPSTATECALLBACK)(RTS_UINTPTR ulPLCHandler, AppStateListCmp *pAppStateListCmp, void *pReadAppStateCstData);

typedef long(*PLCHANDLERVARLISTCALLBACK)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, void* pCstData); /* For UpdateReady, DataChanged and ReactivateInactiveVarList callbacks. */

PLCH_DLL_DECL RTS_UINTPTR PLCHandlerInit(PlcConfigStruct *pPlcConfig, PlcDeviceDescStruct *pDeviceDesc, char *pszLogFile);
typedef RTS_UINTPTR (*PFPLCHANDLERINIT)(PlcConfigStruct *pPlcConfig, PlcDeviceDescStruct *pDeviceDesc, char *pszLogFile);

PLCH_DLL_DECL RTS_UINTPTR PLCHandlerInitByFile(unsigned long ulId, char *pszIniFile, char *pszLogFile);
typedef RTS_UINTPTR(*PFPLCHANDLERINITBYFILE)(unsigned long ulId, char *pszIniFile, char *pszLogFile);

PLCH_DLL_DECL RTS_UINTPTR PLCHandlerInitByFilePlcName(char *pszPlcName, char *pszIniFile, char *pszLogFile);
typedef RTS_UINTPTR(*PFPLCHANDLERINITBYFILEPLCNAME)(char *pszPlcName, char *pszIniFile, char *pszLogFile);

PLCH_DLL_DECL RTS_UINTPTR PLCHandlerInit2(ItfType it, char *pszLogFile);
typedef RTS_UINTPTR(*PFPLCHANDLERINIT2)(ItfType it, char *pszLogFile);

PLCH_DLL_DECL long PLCHandlerExit(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLEREXIT)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerSetClientInformation(RTS_UINTPTR ulPLCHandler, RTS_UTF8STRING *pszClientName, RTS_UTF8STRING *pszClientVendorName, RTS_UTF8STRING *pszClientVersion, unsigned short usClientSpecific);
typedef long(*PFPLCHANDLERSETCLIENTINFORMATION)(RTS_UINTPTR ulPLCHandler, RTS_UTF8STRING *pszClientName, RTS_UTF8STRING *pszClientVendorName, RTS_UTF8STRING *pszClientVersion, unsigned short usClientSpecific);

PLCH_DLL_DECL long PLCHandlerSetFinalShutDown(RTS_UINTPTR ulPLCHandler);
typedef long(*PFPLCHANDLERSETFINALSHUTDOWN)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL int PLCHandlerSetLogging(RTS_UINTPTR ulPLCHandler, int bEnable, unsigned long ulLogFilter);
typedef int(*PFPLCHANDLERSETLOGGING)(RTS_UINTPTR ulPLCHandler, int bEnable, unsigned long ulLogFilter);

PLCH_DLL_DECL int PLCHandlerGetLogging(RTS_UINTPTR ulPLCHandler);
typedef int(*PFPLCHANDLERGETLOGGING)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerSetLogFileCapacity(RTS_UINTPTR ulPLCHandler, int iMaxFileSize, int iMaxFiles);
typedef long(*PFPLCHANDLERSETLOGFILECAPACITY)(RTS_UINTPTR ulPLCHandler, int iMaxFileSize, int iMaxFiles);

PLCH_DLL_DECL long PLCHandlerSetLogFile(RTS_UINTPTR ulPLCHandler, char *pszLogFile);
typedef long(*PFPLCHANDLERSETLOGFILE)(RTS_UINTPTR ulPLCHandler, char *pszLogFile);

PLCH_DLL_DECL long PLCHandlerAddLogEntry(RTS_UINTPTR ulPLCHandler, unsigned long CmpId, int iClassID, int iErrorID, char *pszInfo, ...);
typedef long(*PFPLCHANDLERADDLOGENTRY)(RTS_UINTPTR ulPLCHandler, unsigned long CmpId, int iClassID, int iErrorID, char *pszInfo, ...);

PLCH_DLL_DECL long PLCHandlerAddLogEntryArg(RTS_UINTPTR ulPLCHandler, unsigned long CmpId, int iClassID, int iErrorID, char *pszInfo, va_list *pargList);
typedef long(*PFPLCHANDLERADDLOGENTRYARG)(RTS_UINTPTR ulPLCHandler, unsigned long CmpId, int iClassID, int iErrorID, char *pszInfo, va_list *pargList);

PLCH_DLL_DECL long PLCHandlerScanNetwork(RTS_UINTPTR ulPLCHandler, GatewayConnectionStruct *pGatewayConnection, PLCHANDLERSCANNETWORKCALLBACK pfPlcFoundCallback);
typedef long(*PFPLCHANDLERSCANNETWORK)(RTS_UINTPTR ulPLCHandler, GatewayConnectionStruct *pGatewayConnection, PLCHANDLERSCANNETWORKCALLBACK pfPlcFoundCallback);

PLCH_DLL_DECL long PLCHandlerScanNetwork2(RTS_UINTPTR ulPLCHandler, GatewayConnectionStruct *pGatewayConnection, PLCHANDLERSCANNETWORKCALLBACK2 pfPlcFoundCallback, void *pScanCstData);
typedef long(*PFPLCHANDLERSCANNETWORK2)(RTS_UINTPTR ulPLCHandler, GatewayConnectionStruct *pGatewayConnection, PLCHANDLERSCANNETWORKCALLBACK2 pfPlcFoundCallback, void *pCstData);

PLCH_DLL_DECL long PLCHandlerSetConfigInteractive(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLERSETCONFIGINTERACTIVE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerSetConfig(RTS_UINTPTR ulPLCHandler, PlcConfigStruct *pPlcConfig, PlcDeviceDescStruct *pDeviceDesc);
typedef long (*PFPLCHANDLERSETCONFIG)(RTS_UINTPTR ulPLCHandler, PlcConfigStruct *pPlcConfig, PlcDeviceDescStruct *pDeviceDesc);

PLCH_DLL_DECL long PLCHandlerSetConfigByString(RTS_UINTPTR ulPLCHandler, char *pszConfig, unsigned long ulConfigLen, char *pszLineEnd);
typedef long (*PFPLCHANDLERSETCONFIGBYSTRING)(RTS_UINTPTR ulPLCHandler, char *pszConfig, unsigned long ulConfigLen, char *pszLineEnd);

PLCH_DLL_DECL long PLCHandlerSetConfigByFile(RTS_UINTPTR ulPLCHandler, char *pszIniFile);
typedef long(*PFPLCHANDLERSETCONFIGBYFILE)(RTS_UINTPTR ulPLCHandler, char *pszIniFile);

PLCH_DLL_DECL long PLCHandlerSetConnectionCallbacks(RTS_UINTPTR ulPLCHandler, PLCHANDLERSTATECHANGECALLBACK pfStateChangeCallback, PLCHANDLERVERIFYPLCCERTCALLBACK pfVerifyPlcCertCallback);
typedef long (*PFPLCHANDLERSETCONNECTIONCALLBACKS)(RTS_UINTPTR ulPLCHandler, PLCHANDLERSTATECHANGECALLBACK pfStateChangeCallback, PLCHANDLERVERIFYPLCCERTCALLBACK pfVerifyPlcCertCallback);

PLCH_DLL_DECL long PLCHandlerSetConnectionCallbacks2(RTS_UINTPTR ulPLCHandler, PLCHANDLERSTATECHANGECALLBACK2 pfStateChangeCallback, void *pStateCstData, PLCHANDLERVERIFYPLCCERTCALLBACK2 pfVerifyPlcCertCallback, void *pVerifyCstData);
typedef long(*PFPLCHANDLERSETCONNECTIONCALLBACKS2)(RTS_UINTPTR ulPLCHandler, PLCHANDLERSTATECHANGECALLBACK2 pfStateChangeCallback, void *pStateCstData, PLCHANDLERVERIFYPLCCERTCALLBACK2 pfVerifyPlcCertCallback, void *pVerifyCstData);

PLCH_DLL_DECL long PLCHandlerSetConnectionCallbacks3(RTS_UINTPTR ulPLCHandler, const PLCHANDLERSTATECHANGECALLBACK2 pfStateChangeCallback, void * const pStateCstData, const PLCHANDLERVERIFYPLCCERTCALLBACK2 pfVerifyPlcCertCallback, void * const pVerifyCstData, const PLCHANDLERREQUESTCREDENTIALSCALLBACK pfRequestCredentialsCallback, void* const pRequestCredentialsCstData);
typedef long(*PFPLCHANDLERSETCONNECTIONCALLBACKS3)(RTS_UINTPTR ulPLCHandler, const PLCHANDLERSTATECHANGECALLBACK2 pfStateChangeCallback, void * const pStateCstData, const PLCHANDLERVERIFYPLCCERTCALLBACK2 pfVerifyPlcCertCallback, void * const pVerifyCstData, const PLCHANDLERREQUESTCREDENTIALSCALLBACK pfRequestCredentialsCallback, void* const pRequestCredentialsCstData);

PLCH_DLL_DECL long PLCHandlerGetConfig(RTS_UINTPTR ulPLCHandler, PlcConfigStruct **ppPlcConfig, PlcDeviceDescStruct **ppDeviceDesc);
typedef long (*PFPLCHANDLERGETCONFIG)(RTS_UINTPTR ulPLCHandler, PlcConfigStruct **ppPlcConfig, PlcDeviceDescStruct **ppDeviceDesc);

PLCH_DLL_DECL long PLCHandlerSaveConfigInFile(RTS_UINTPTR ulPLCHandler, char *pszIniFile);
typedef long (*PFPLCHANDLERSAVECONFIGINFILE)(RTS_UINTPTR ulPLCHandler, char *pszIniFile);

PLCH_DLL_DECL long PLCHandlerConnect(RTS_UINTPTR ulPLCHandler, unsigned long ulTimeout, RTS_UINTPTR hStateChangedEvent, int bLoadSymbols);
typedef long (*PFPLCHANDLERCONNECT)(RTS_UINTPTR ulPLCHandler, unsigned long ulTimeout, RTS_UINTPTR hStateChangedEvent, int bLoadSymbols);

PLCH_DLL_DECL long PLCHandlerConnectTcpipViaGateway(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, char *pszPlcIP, char *pszProtocol, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, unsigned long ulPort);
typedef long (*PFPLCHANDLERCONNECTTCPIPVIAGATEWAY)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, char *pszPlcIP, char *pszProtocol, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, unsigned long ulPort);

PLCH_DLL_DECL long PLCHandlerConnectRs232ViaGateway(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout);
typedef long (*PFPLCHANDLERCONNECTRS232VIAGATEWAY)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectRs232ViaGatewayEx(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, EXT_RS232_PARAMStyp *pExtParams);
typedef long (*PFPLCHANDLERCONNECTRS232VIAGATEWAYEX)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, EXT_RS232_PARAMStyp *pExtParams);

PLCH_DLL_DECL long PLCHandlerConnectTcpipViaArti(RTS_UINTPTR ulPLCHandler, char *pszPlcIP, char *pszProtocol, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, unsigned long ulPort);
typedef long (*PFPLCHANDLERCONNECTTCPIPVIAARTI)(RTS_UINTPTR ulPLCHandler, char *pszPlcIP, char *pszProtocol, int bMotorola, int bLoadSymbols, unsigned long ulTimeout, unsigned long ulPort);

PLCH_DLL_DECL long PLCHandlerConnectRs232ViaArti(RTS_UINTPTR ulPLCHandler, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout);
typedef long (*PFPLCHANDLERCONNECTRS232VIAARTI)(RTS_UINTPTR ulPLCHandler, short sPort, unsigned long ulBaudrate, int bMotorola, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectToSimulation(RTS_UINTPTR ulPLCHandler, char *pszSdbFile, int bLoadSymbols, unsigned long ulTimeout);
typedef long (*PFPLCHANDLERCONNECTTOSIMULATION)(RTS_UINTPTR ulPLCHandler, char *pszSdbFile, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaGateway3(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);
typedef long (*PFPLCHANDLERCONNECTVIAGATEWAY3)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaGateway3Ex(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);
typedef long (*PFPLCHANDLERCONNECTVIAGATEWAY3EX)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaGateway3ByName(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, RTS_WCHAR *pwszDeviceName, int bLoadSymbols, unsigned long ulTimeout);
typedef long(*PFPLCHANDLERCONNECTVIAGATEWAY3BYNAME)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, RTS_WCHAR *pwszDeviceName, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaArti3(RTS_UINTPTR ulPLCHandler, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);
typedef long (*PFPLCHANDLERCONNECTVIAARTI3)(RTS_UINTPTR ulPLCHandler, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectViaArti3ByName(RTS_UINTPTR ulPLCHandler, RTS_WCHAR *pwszDeviceName, int bLoadSymbols, unsigned long ulTimeout);
typedef long(*PFPLCHANDLERCONNECTVIAARTI3BYNAME)(RTS_UINTPTR ulPLCHandler, RTS_WCHAR *pwszDeviceName, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectToSimulation3(RTS_UINTPTR ulPLCHandler, char *pszSdb3XmlFile, int bLoadSymbols, unsigned long ulTimeout);
typedef long (*PFPLCHANDLERCONNECTTOSIMULATION3)(RTS_UINTPTR ulPLCHandler, char *pszSdb3XmlFile, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectTcpipViaGateway3(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, char *pszPlcIP, unsigned long ulPlcPort, int bLoadSymbols, unsigned long ulTimeout);
typedef long (*PFPLCHANDLERCONNECTTCPIPVIAGATEWAY3)(RTS_UINTPTR ulPLCHandler, char *pszGatewayIP, unsigned long ulPort, char *pszPlcIP, unsigned long ulPlcPort, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerConnectTcpipViaArti3(RTS_UINTPTR ulPLCHandler, char *pszPlcIP, unsigned long ulPlcPort, int bLoadSymbols, unsigned long ulTimeout);
typedef long (*PFPLCHANDLERCONNECTTCPIPVIAARTI3)(RTS_UINTPTR ulPLCHandler, char *pszPlcIP, unsigned long ulPlcPort, int bLoadSymbols, unsigned long ulTimeout);

PLCH_DLL_DECL long PLCHandlerDisconnect(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLERDISCONNECT)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL unsigned long PLCHandlerGetVersion(RTS_UINTPTR ulPLCHandler);
typedef unsigned long (*PFPLCHANDLERGETVERSION)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL int PLCHandlerGetState(RTS_UINTPTR ulPLCHandler);
typedef int (*PFPLCHANDLERGETSTATE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL char *PLCHandlerGetName(RTS_UINTPTR ulPLCHandler);
typedef char *(*PFPLCHANDLERGETNAME)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerStartKeeplive(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLERSTARTKEEPLIVE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerStopKeeplive(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLERSTOPKEEPLIVE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerLoadSymbolsFromPlc(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLERLOADSYMBOLSFROMPLC)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerLoadSymbolsFromPlc2(RTS_UINTPTR ulPLCHandler, int bLoadSymbolsOnReconnect);
typedef long (*PFPLCHANDLERLOADSYMBOLSFROMPLC2)(RTS_UINTPTR ulPLCHandler, int bLoadSymbolsOnReconnect);

PLCH_DLL_DECL long PLCHandlerGetAllItems(RTS_UINTPTR ulPLCHandler, PlcSymbolDescStruct **ppSymbolList, unsigned long *pulNumOfSymbols);
typedef long (*PFPLCHANDLERGETALLITEMS)(RTS_UINTPTR ulPLCHandler, PlcSymbolDescStruct **ppSymbolList, unsigned long *pulNumOfSymbols);

PLCH_DLL_DECL long PLCHandlerEnterItemAccess(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLERENTERITEMACCESS)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerLeaveItemAccess(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLERLEAVEITEMACCESS)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerEnterOnlineAccess(RTS_UINTPTR ulPLCHandler);
typedef long(*PFPLCHANDLERENTERONLINEACCESS)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerLeaveOnlineAccess(RTS_UINTPTR ulPLCHandler);
typedef long(*PFPLCHANDLERLEAVEONLINEACCESS)(RTS_UINTPTR ulPLCHandler);

/* Get the definition of an item specified by name. The call of this method and the access of pSymbol must be
   protected by PLCHandlerEnterItemAccess() and PLCHandlerLeaveItemAccess(). */
PLCH_DLL_DECL long PLCHandlerGetItem(RTS_UINTPTR ulPLCHandler, char *pszSymbol, PlcSymbolDescStruct *pSymbol);
typedef long (*PFPLCHANDLERGETITEM)(RTS_UINTPTR ulPLCHandler, char *pszSymbol, PlcSymbolDescStruct *pSymbol);

PLCH_DLL_DECL long PLCHandlerGetAddressOfMappedItem(RTS_UINTPTR ulPLCHandler, char *pszSymbol, char *pszMappedAddr, long lLen);
typedef long (*PFPLCHANDLERGETADDRESSOFMAPPEDITEM)(RTS_UINTPTR ulPLCHandler, char *pszSymbol, char *pszMappedAddr, long lLen);

/* Expand an item (if this is an Array or UserDef type) on the fly. The call of this method must be
   protected by PLCHandlerEnterItemAccess() and PLCHandlerLeaveItemAccess().
   pSymbolList must be released with PLCHandlerReleaseExpandedItems() after usage!
   Returns an error, if it is a simple type */
PLCH_DLL_DECL long PLCHandlerExpandItem(RTS_UINTPTR ulPLCHandler, char *pszSymbol, PlcSymbolDescStruct **ppSymbolList, unsigned long *pulNumOfSymbols);
typedef long (*PFPLCHANDLEREXPANDITEM)(RTS_UINTPTR ulPLCHandler, char *pszSymbol, PlcSymbolDescStruct **ppSymbolList, unsigned long *pulNumOfSymbols);

/* Function to release dynamic expanded item list */
PLCH_DLL_DECL long PLCHandlerReleaseExpandedItems(RTS_UINTPTR ulPLCHandler, PlcSymbolDescStruct *pSymbolList);
typedef long (*PFPLCHANDLERRELEASEEXPANDEDITEMS)(RTS_UINTPTR ulPLCHandler, PlcSymbolDescStruct *pSymbolList);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycDefineVarList(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent);
typedef HCYCLIST (*PFPLCHANDLERCYCDEFINEVARLIST)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycDefineVarList2(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent, unsigned long ulFlags, long *plResult);
typedef HCYCLIST(*PFPLCHANDLERCYCDEFINEVARLIST2)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent, unsigned long ulFlags, long *plResult);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycDefineVarList3(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, const PLCHANDLERVARLISTCALLBACK pfUpdateReadyCallback, void * const pUpdateReadyCstData, const PLCHANDLERVARLISTCALLBACK pfDataChangedCallback, void * const pDataChangedCstData, unsigned long ulFlags, long *plResult);
typedef HCYCLIST(*PFPLCHANDLERCYCDEFINEVARLIST3)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, const PLCHANDLERVARLISTCALLBACK pfUpdateReadyCallback, void * const pUpdateReadyCstData, const PLCHANDLERVARLISTCALLBACK pfDataChangedCallback, void * const pDataChangedCstData, unsigned long ulFlags, long *plResult);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycDefineVarList4(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, const PLCHANDLERVARLISTCALLBACK pfUpdateReadyCallback, void * const pUpdateReadyCstData, const PLCHANDLERVARLISTCALLBACK pfDataChangedCallback, void * const pDataChangedCstData, unsigned long ulFlags, long *plResult, const PLCHANDLERVARLISTCALLBACK pfReactivateInactiveVarListCallback, void * const pReactivateInactiveVarListCstData);
typedef HCYCLIST(*PFPLCHANDLERCYCDEFINEVARLIST4)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, const PLCHANDLERVARLISTCALLBACK pfUpdateReadyCallback, void * const pUpdateReadyCstData, const PLCHANDLERVARLISTCALLBACK pfDataChangedCallback, void * const pDataChangedCstData, unsigned long ulFlags, long *plResult, const PLCHANDLERVARLISTCALLBACK pfReactivateInactiveVarListCallback, void * const pReactivateInactiveVarListCstData);

PLCH_DLL_DECL long PLCHandlerCycAddSymbolsToVarList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **ppszSymbols, unsigned long ulNumOfSymbolsToAdd);
typedef long(*PFPLCHANDLERCYCADDSYMBOLSTOVARLIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **ppszSymbols, unsigned long ulNumOfSymbolsToAdd);

PLCH_DLL_DECL long PLCHandlerCycRemoveSymbolsFromVarList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **ppszSymbols, unsigned long ulNumOfSymbolsToRemove);
typedef long(*PFPLCHANDLERCYCREMOVESYMBOLSFROMVARLIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **ppszSymbols, unsigned long ulNumOfSymbolsToRemove);

PLCH_DLL_DECL long PLCHandlerCycDeleteVarList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef long (*PFPLCHANDLERCYCDELETEVARLIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

PLCH_DLL_DECL long PLCHandlerCycDeleteVarList2(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, int bKeepalive);
typedef long(*PFPLCHANDLERCYCDELETEVARLIST2)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, int bKeepalive);

/* Enable the list: enable cyclic update */
PLCH_DLL_DECL long PLCHandlerCycEnableList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef long (*PFPLCHANDLERCYCENABLELIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

/* Disable the list: disable cyclic update */
PLCH_DLL_DECL long PLCHandlerCycDisableList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef long (*PFPLCHANDLERCYCDISABLELIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycUpdateVarList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent);
typedef HCYCLIST (*PFPLCHANDLERCYCUPDATEVARLIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycUpdateVarList2(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent, unsigned long ulFlags, long *plResult);
typedef HCYCLIST(*PFPLCHANDLERCYCUPDATEVARLIST2)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, RTS_UINTPTR hUpdateReadyEvent, RTS_UINTPTR hDataChangeEvent, unsigned long ulFlags, long *plResult);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycUpdateVarList3(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, const PLCHANDLERVARLISTCALLBACK pfUpdateReadyCallback, void * const pUpdateReadyCstData, const PLCHANDLERVARLISTCALLBACK pfDataChangedCallback, void * const pDataChangedCstData, unsigned long ulFlags, long *plResult);
typedef HCYCLIST(*PFPLCHANDLERCYCUPDATEVARLIST3)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, const PLCHANDLERVARLISTCALLBACK pfUpdateReadyCallback, void * const pUpdateReadyCstData, const PLCHANDLERVARLISTCALLBACK pfDataChangedCallback, void * const pDataChangedCstData, unsigned long ulFlags, long *plResult);

PLCH_DLL_DECL HCYCLIST PLCHandlerCycUpdateVarList4(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, const PLCHANDLERVARLISTCALLBACK pfUpdateReadyCallback, void * const pUpdateReadyCstData, const PLCHANDLERVARLISTCALLBACK pfDataChangedCallback, void * const pDataChangedCstData, unsigned long ulFlags, long *plResult, const PLCHANDLERVARLISTCALLBACK pfReactivateInactiveVarListCallback, void * const pReactivateInactiveVarListCstData);
typedef HCYCLIST(*PFPLCHANDLERCYCUPDATEVARLIST4)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned long ulUpdateRate, const PLCHANDLERVARLISTCALLBACK pfUpdateReadyCallback, void * const pUpdateReadyCstData, const PLCHANDLERVARLISTCALLBACK pfDataChangedCallback, void * const pDataChangedCstData, unsigned long ulFlags, long *plResult, const PLCHANDLERVARLISTCALLBACK pfReactivateInactiveVarListCallback, void * const pReactivateInactiveVarListCstData);

PLCH_DLL_DECL void PLCHandlerCycEnterVarAccess(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef void (*PFPLCHANDLERCYCENTERVARACCESS)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

PLCH_DLL_DECL long PLCHandlerCycReadVars(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);
typedef long (*PFPLCHANDLERCYCREADVARS)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);

PLCH_DLL_DECL long PLCHandlerCycReadChangedVars(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, PlcVarValueStruct ***pppChangedValues, unsigned long **ppChangedValueIndex, unsigned long *pulNumOfChangedValues);
typedef long(*PFPLCHANDLERCYCREADCHANGEDVARS)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, PlcVarValueStruct ***pppChangedValues, unsigned long **ppChangedValueIndex, unsigned long *pulNumOfChangedValues);

PLCH_DLL_DECL void PLCHandlerCycLeaveVarAccess(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef void (*PFPLCHANDLERCYCLEAVEVARACCESS)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

/* Get the update rate of the cyclic list. */
PLCH_DLL_DECL unsigned long PLCHandlerCycGetUpdateRate(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef unsigned long (*PFPLCHANDLERCYCGETUPDATERATE)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

/* Set the update rate of the cyclic list. */
PLCH_DLL_DECL long PLCHandlerCycSetUpdateRate(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, unsigned long ulUpdateRate);
typedef long (*PFPLCHANDLERCYCSETUPDATERATE)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, unsigned long ulUpdateRate);

PLCH_DLL_DECL long PLCHandlerCycGetSymbolList(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char ***pppCycSymbolList);
typedef long (*PFPLCHANDLERCYCGETSYMBOLLIST)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList, char ***pppCycSymbolList);

PLCH_DLL_DECL unsigned long PLCHandlerCycGetOperatingRate(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);
typedef unsigned long (*PFPLCHANDLERCYCGETOPERATINGRATE)(RTS_UINTPTR ulPLCHandler, HCYCLIST hCycVarList);

PLCH_DLL_DECL HVARLIST PLCHandlerSyncDefineVarList(RTS_UINTPTR ulPLCHandler, char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned long ulFlags, long *plResult);
typedef HVARLIST(*PFPLCHANDLERSYNCDEFINEVARLIST)(RTS_UINTPTR ulPLCHandler, char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned long ulFlags, long *plResult);

PLCH_DLL_DECL long PLCHandlerSyncDeleteVarList(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList);
typedef long(*PFPLCHANDLERSYNCDELETEVARLIST)(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList);

PLCH_DLL_DECL long PLCHandlerSyncReadVarListFromPlc(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);
typedef long(*PFPLCHANDLERSYNCREADVARLISTFROMPLC)(RTS_UINTPTR ulPLCHandler, HVARLIST hVarList, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);

PLCH_DLL_DECL long PLCHandlerSyncWriteVarListToPlc(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList, unsigned long ulNumOfSymbols, unsigned char **ppbyValues, unsigned long *pulValueSizes);
typedef long(*PFPLCHANDLERSYNCWRITEVARLISTTOPLC)(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncVarList, unsigned long ulNumOfSymbols, unsigned char **ppbyValues, unsigned long *pulValueSizes);

PLCH_DLL_DECL HVARLIST PLCHandlerSyncReadVarsFromPlc(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);
typedef HVARLIST (*PFPLCHANDLERSYNCREADVARSFROMPLC)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, PlcVarValueStruct ***pppValues, unsigned long *pulNumOfValues);

PLCH_DLL_DECL long PLCHandlerSyncReadVarsFromPlcReleaseValues(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncRead);
typedef long (*PFPLCHANDLERSYNCREADVARSFROMPLCRELEASEVALUES)(RTS_UINTPTR ulPLCHandler, HVARLIST hSyncRead);

PLCH_DLL_DECL long PLCHandlerSyncWriteVarsToPlc(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char **ppbyValues);
typedef long (*PFPLCHANDLERSYNCWRITEVARSTOPLC)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char **ppbyValues);

PLCH_DLL_DECL long PLCHandlerSyncWriteVarsToPlc2(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char **ppbyValues, unsigned long *pulValueSizes);
typedef long (*PFPLCHANDLERSYNCWRITEVARSTOPLC2)(RTS_UINTPTR ulPLCHandler, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char **ppbyValues, unsigned long *pulValueSizes);

PLCH_DLL_DECL long PLCHandlerUploadFile(RTS_UINTPTR ulPLCHandler, char *pszPlc, char *pszHost);
typedef long (*PFPLCHANDLERUPLOADFILE)(RTS_UINTPTR ulPLCHandler, char *pszPlc, char *pszHost);

PLCH_DLL_DECL long PLCHandlerDownloadFile(RTS_UINTPTR ulPLCHandler, char *pszHost, char *pszPlc);
typedef long (*PFPLCHANDLERDOWNLOADFILE)(RTS_UINTPTR ulPLCHandler, char *pszHost, char *pszPlc);

PLCH_DLL_DECL long PLCHandlerReloadBootproject(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLERRELOADBOOTPROJECT)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerRegisterBootApplication(RTS_UINTPTR ulPLCHandler, char *pszApplication);
typedef long (*PFPLCHANDLERREGISTERBOOTAPPLICATION)(RTS_UINTPTR ulPLCHandler, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerReloadBootApplication(RTS_UINTPTR ulPLCHandler, char *pszApplication);
typedef long (*PFPLCHANDLERRELOADBOOTAPPLICATION)(RTS_UINTPTR ulPLCHandler, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerSaveRetains(RTS_UINTPTR ulPLCHandler, char *pszRetainFile, long lBufferLen, char *pszApplication);
typedef long (*PFPLCHANDLERSAVERETAINS)(RTS_UINTPTR ulPLCHandler, char *pszRetainFile, long lBufferLen, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerRestoreRetains(RTS_UINTPTR ulPLCHandler, char *pszRetainFile, long lBufferLen, char *pszApplication);
typedef long (*PFPLCHANDLERRESTORERETAINS)(RTS_UINTPTR ulPLCHandler, char *pszRetainFile, long lBufferLen, char *pszApplication);

/* Send echo service to PLC e. g. to measure the communication performance (only for V3) */
PLCH_DLL_DECL long PLCHandlerSendPlcEcho(RTS_UINTPTR ulPLCHandler, unsigned long *pulSendDataLen, unsigned long *pulReceiveDataLen);
typedef long (*PFPLCHANDLERSENDPLCECHO)(RTS_UINTPTR ulPLCHandler, unsigned long *pulSendDataLen, unsigned long *pulReceiveDataLen);

PLCH_DLL_DECL long PLCHandlerCheckTarget(RTS_UINTPTR ulPLCHandler, unsigned long ulTargetId, unsigned long ulHookId, unsigned long ulMagic);
typedef long (*PFPLCHANDLERCHECKTARGET)(RTS_UINTPTR ulPLCHandler, unsigned long ulTargetId, unsigned long ulHookId, unsigned long ulMagic);

PLCH_DLL_DECL long PLCHandlerGetProjectInfo(RTS_UINTPTR ulPLCHandler, ProjectInfoStruct **ppProjectInfo);
typedef long (*PFPLCHANDLERGETPROJECTINFO)(RTS_UINTPTR ulPLCHandler, ProjectInfoStruct **ppProjectInfo);

PLCH_DLL_DECL long PLCHandlerGetApplicationList(RTS_UINTPTR ulPLCHandler, char ***pppszApplications, unsigned long *pulNumOfApplications);
typedef long (*PFPLCHANDLERGETAPPLICATIONLIST)(RTS_UINTPTR ulPLCHandler, char ***pppszApplications, unsigned long *pulNumOfApplications);

PLCH_DLL_DECL long PLCHandlerCheckApplicationFileConsistency(RTS_UINTPTR ulPLCHandler, char *pszApplication, long *plBootProjectResult, long *plArchiveResult);
typedef long(*PFPLCHANDLERCHECKAPPLICATIONFILECONSISTENCY)(RTS_UINTPTR ulPLCHandler, char *pszApplication, long *plBootProjectResult, long *plArchiveResult);

PLCH_DLL_DECL long PLCHandlerGetApplicationInfo(RTS_UINTPTR ulPLCHandler, char *pszApplication, ProjectInfoStruct **ppPrjInfo, ApplicationInfoStruct **ppAppInfo);
typedef long (*PFPLCHANDLERGETAPPLICATIONINFO)(RTS_UINTPTR ulPLCHandler, char *pszApplication, ProjectInfoStruct **ppPrjInfo, ApplicationInfoStruct **ppAppInfo);

PLCH_DLL_DECL long PLCHandlerGetApplicationInfo2(RTS_UINTPTR ulPLCHandler, char *pszApplication, ProjectInfoStruct **ppPrjInfo, ApplicationInfoStruct2 **ppAppInfo);
typedef long(*PFPLCHANDLERGETAPPLICATIONINFO2)(RTS_UINTPTR ulPLCHandler, char *pszApplication, ProjectInfoStruct **ppPrjInfo, ApplicationInfoStruct2 **ppAppInfo);

PLCH_DLL_DECL long PLCHandlerGetDeviceInfo(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct **ppDeviceInfo);
typedef long (*PFPLCHANDLERGETDEVICEINFO)(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct **ppDeviceInfo);

PLCH_DLL_DECL long PLCHandlerGetDeviceInfo2(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct2 **ppDeviceInfo);
typedef long(*PFPLCHANDLERGETDEVICEINFO2)(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct2 **ppDeviceInfo);

PLCH_DLL_DECL long PLCHandlerGetDeviceInfo3(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct3 **ppDeviceInfo);
typedef long(*PFPLCHANDLERGETDEVICEINFO3)(RTS_UINTPTR ulPLCHandler, DeviceInfoStruct3 **ppDeviceInfo);

PLCH_DLL_DECL long PLCHandlerSyncSendService(RTS_UINTPTR ulPLCHandler, unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize);
typedef long (*PFPLCHANDLERSYNCSENDSERVICE)(RTS_UINTPTR ulPLCHandler, unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize);

PLCH_DLL_DECL long PLCHandlerGetLastError(RTS_UINTPTR ulPLCHandler);
typedef long (*PFPLCHANDLERGETLASTERROR)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerGetPlcStatus(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM *pPlcStatus);
typedef long (*PFPLCHANDLERGETPLCSTATUS)(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM *pPlcStatus);

PLCH_DLL_DECL long PLCHandlerSetPlcStatus(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM PlcStatus);
typedef long (*PFPLCHANDLERSETPLCSTATUS)(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM PlcStatus);

PLCH_DLL_DECL long PLCHandlerResetPlc(RTS_UINTPTR ulPLCHandler, RESET_OPTION_ENUM ResetCommand);
typedef long (*PFPLCHANDLERRESETPLC)(RTS_UINTPTR ulPLCHandler, RESET_OPTION_ENUM ResetCommand);

PLCH_DLL_DECL long PLCHandlerGetApplicationStatus(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM *pAppStatus, char *pszApplication);
typedef long (*PFPLCHANDLERGETAPPLICATIONSTATUS)(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM *pAppStatus, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerSetApplicationStatus(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM AppStatus, char *pszApplication);
typedef long (*PFPLCHANDLERSETAPPLICATIONSTATUS)(RTS_UINTPTR ulPLCHandler, PLC_STATUS_ENUM AppStatus, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerResetApplication(RTS_UINTPTR ulPLCHandler, RESET_OPTION_ENUM ResetCommand, char *pszApplication);
typedef long (*PFPLCHANDLERRESETAPPLICATION)(RTS_UINTPTR ulPLCHandler, RESET_OPTION_ENUM ResetCommand, char *pszApplication);

PLCH_DLL_DECL long PLCHandlerGetResetOriginDeviceConfig(RTS_UINTPTR ulPLCHandler, unsigned long *pulConfigOptions, unsigned long *pulRights, RTS_UTF8STRING ***pppszDescriptions);
typedef long(*PFPLCHANDLERGETRESETORIGINDEVICECONFIG)(RTS_UINTPTR ulPLCHandler, unsigned long *pulConfigOptions, unsigned long *pulRights, RTS_UTF8STRING ***pppszDescriptions);

PLCH_DLL_DECL long PLCHandlerResetOriginDevice2(RTS_UINTPTR ulPLCHandler, unsigned long ulConfiguration, unsigned long *pulResults);
typedef long(*PFPLCHANDLERRESETORIGINDEVICE2)(RTS_UINTPTR ulPLCHandler, unsigned long ulConfiguration, unsigned long *pulResults);

PLCH_DLL_DECL long PLCHandlerResetOriginDevice(RTS_UINTPTR ulPLCHandler);
typedef long(*PFPLCHANDLERRESETORIGINDEVICE)(RTS_UINTPTR ulPLCHandler);

PLCH_DLL_DECL long PLCHandlerGetDeviceOperationMode(RTS_UINTPTR ulPLCHandler, DEVICE_OPERATION_MODE *pOpMode);
typedef long(*PFPLCHANDLERGETDEVICEOPERATIONMODE)(RTS_UINTPTR ulPLCHandler, DEVICE_OPERATION_MODE *pOpMode);

PLCH_DLL_DECL long PLCHandlerSetDeviceOperationMode(RTS_UINTPTR ulPLCHandler, DEVICE_OPERATION_MODE OpMode);
typedef long(*PFPLCHANDLERSETDEVICEOPERATIONMODE)(RTS_UINTPTR ulPLCHandler, DEVICE_OPERATION_MODE OpMode);

PLCH_DLL_DECL long PLCHandlerRenameDevice(RTS_UINTPTR ulPLCHandler, RTS_WCHAR *pwszNodeName);
typedef long(*PFPLCHANDLERRENAMEDEVICE)(RTS_UINTPTR ulPLCHandler, RTS_WCHAR *pwszNodeName);

PLCH_DLL_DECL long PLCHandlerBackupIECApplications(RTS_UINTPTR ulPLCHandler, char *pszBackupFilePath, PLCHANDLERBACKUPRESTORERESULTCALLBACK pfBackupResultCallback, int bForceBackup, int bCreateTbf);
typedef long(*PFPLCHANDLERBACKUPIECAPPLICATIONS)(RTS_UINTPTR ulPLCHandler, char *pszBackupFilePath, PLCHANDLERBACKUPRESTORERESULTCALLBACK pfBackupResultCallback, int bForceBackup, int bCreateTbf);

PLCH_DLL_DECL long PLCHandlerRestoreIECApplications(RTS_UINTPTR ulPLCHandler, char *pszRestoreFilePath, PLCHANDLERBACKUPRESTORERESULTCALLBACK pfRestoreResultCallback, int bStartBootprojects);
typedef long(*PFPLCHANDLERRESTOREIECAPPLICATIONS)(RTS_UINTPTR ulPLCHandler, char *pszRestoreFilePath, PLCHANDLERBACKUPRESTORERESULTCALLBACK pfRestoreResultCallback, int bStartBootprojects);

PLCH_DLL_DECL long PLCHandlerStartReadAppState(RTS_UINTPTR ulPLCHandler, unsigned long ulCheckInterval, PLCHANDLERREADAPPSTATECALLBACK pfReadAppStateCallback, void *pReadAppStateCstData);
typedef long (*PLCHANDLERSTARTREADAPPSTATE)(RTS_UINTPTR ulPLCHandler, unsigned long ulCheckInterval, PLCHANDLERREADAPPSTATECALLBACK pfReadAppStateCallback, void *pReadAppStateCstData);

PLCH_DLL_DECL long PLCHandlerStopReadAppState(RTS_UINTPTR ulPLCHandler);
typedef long(*PLCHANDLERSTOPREADAPPSTATE)(RTS_UINTPTR ulPLCHandler);

/* Firmware update is not released yet. */
//PLCH_DLL_DECL long PLCHandlerInstallFirmwareUpdate(RTS_UINTPTR ulPLCHandler, char *pszFirmwarePackage);
//typedef long(*PLCHANDLERINSTALLFIRMWAREUPDATE)(RTS_UINTPTR ulPLCHandler, char *pszFirmwarePackage);

//PLCH_DLL_DECL long PLCHandlerGetFirmwareUpdateResult(RTS_UINTPTR ulPLCHandler, char *pszFirmwarePackage, char **ppSummary, char **ppErrors, char **ppWarnings, char **ppUserActions);
//typedef long(*PLCHANDLERGETFIRMWAREUPDATERESULT)(RTS_UINTPTR ulPLCHandler, char *pszFirmwarePackage, char **ppSummary, char **ppErrors, char **ppWarnings, char **ppUserActions);

//PLCH_DLL_DECL long PLCHandlerGetFirmwareUpdateResultRelease(RTS_UINTPTR ulPLCHandler, char* ppSummary, char* pErrors, char* pWarnings, char* pUserActions);
//typedef long(*PLCHANDLERGETFIRMWAREUPDATERESULTRELEASE)(RTS_UINTPTR ulPLCHandler, char* ppSummary, char* pErrors, char* pWarnings, char* pUserActions);

#if __cplusplus
}
#endif


#endif /*__PLCHANDLERITF_H__*/
