/*****************************************************************************
*
*	Copyright:		(c) 3S - Smart Software Solutions GmbH, Kempten
*	Module: 		PLCHandler
*	Purpose:		Class to handle complete communication to one plc
*
*	Description:	see PLCHandler Programming Guide					
*
******************************************************************************/

#ifndef __PLCHANDLER_H__
#define __PLCHANDLER_H__

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

#include "CmpStd.h"
#include "CmpPLCHandlerDep.h"
#include "PLCHandlerDefines.h"	/* Basic definitions of used types, enums, options, flags ... */
#include "PLCConfig.h"
#include "PLCComBase.h"
#include "PLCUtil.h"
#include "PLCHandlerPrivate.h"

// undef #defines from winbase.h
#if defined (WIN32) && defined (UNICODE) && !defined(_WIN32_WCE) && !defined(OPC_SERVER_3S)
	#undef CreateDirectory
	#define CreateDirectory  CreateDirectoryA
#endif

#if defined (WIN32) && defined (UNICODE) && !defined(_WIN32_WCE) && !defined(OPC_SERVER_3S)
	#undef DeleteFile
	#define DeleteFile  DeleteFileA
#endif

#define INVALID_ID							((unsigned long)-1)

// Callback types
#define PLCH_STATUS_CHANGE_CALLBACK			0
#define PLCH_UPDATE_READY_CALLBACK			1
#define PLCH_DATA_CHANGE_CALLBACK			2
#define PLCH_ASYNC_SERVICE_CALLBACK			3
#define PLCH_SCAN_NETWORK_CALLBACK			4

// Type definitions
typedef void* HCYCLIST;

typedef struct CallbackAddInfoTag
{
	unsigned long ulType;				// see Callback types
	union 
	{
		HCYCLIST hCycList;				// used by PLCH_DATA_CHANGE_CALLBACK, PLCH_UPDATE_READY_CALLBACK
		long lPLCHandlerState;			// used by PLCH_STATUS_CHANGE_CALLBACK
		int iInvokeId;					// used by PLCH_ASYNC_SERVICE_CALLBACK
		NodeInfotyp *pNodeInfo;			// used by PLCH_SCAN_NETWORK_CALLBACK - only for compatibility
		NodeInfotyp2 *pNodeInfo2;		// used by PLCH_SCAN_NETWORK_CALLBACK
	} AddInf;	
} CallbackAddInfotyp;


// Forward declaration
class PLCH_DLL_DECL CPLCHandlerCallback;

// Private threads				
void CDECL UpdateThread(SYS_TASK_PARAM *ptp);
void CDECL ReconnectThread(SYS_TASK_PARAM *ptp);
void CDECL KeepAliveThread(SYS_TASK_PARAM *ptp);
void CDECL AsyncMgrThread(SYS_TASK_PARAM *ptp);

// Class declaration
class PLCH_DLL_DECL CPLCHandler
{
	public:
		// Constructor:
		//	ulId = Index of the plc to handle
		//	pszIniFile = Ini-File, in which the configuration is set (for all PLCHandlers)
		//  hLogfile = Handle to a logger, which was retrieved from another PLCHandler instance by GetLogFile() or created by CAL_LogCreate(). 
		CPLCHandler(/*[In]*/ unsigned long ulId, /*[In]*/ char *pszIniFile, /*[In]*/ RTS_HANDLE hLogFile = RTS_INVALID_HANDLE);

		// Constructor:
		//	pszPlcName = Name of the plc to handle
		//	pszIniFile = Ini-File, in which the configuration is set (for all PLCHandlers)
		//  hLogfile = Handle to a logger, which was retrieved from another PLCHandler instance by GetLogFile() or created by CAL_LogCreate(). 
		CPLCHandler(/*[In]*/ char *pszPlcName, /*[In]*/ char *pszIniFile, /*[In]*/ RTS_HANDLE hLogFile = RTS_INVALID_HANDLE);

		// Constructor:
		//  pPlcConfig = Configuration for a specific PLCHandler
		//  pDeviceDesc = Configuration for the corresponding device
		//  hLogfile = Handle to a logger, which was retrieved from another PLCHandler instance by GetLogFile() or created by CAL_LogCreate(). 
		// NOTE:if pPlcConfig->gwc == NULL and pDeviceDesc == NULL, then
		//		the communication parameters must be called interactive from
		//		user with GetConfigInteractive() method, before calling Connect()!!
		CPLCHandler(/*[In]*/ PlcConfig *pPlcConfig, /*[In]*/ PlcDeviceDesc *pDeviceDesc, /*[In]*/ RTS_HANDLE hLogFile = RTS_INVALID_HANDLE);

		// Constructor, to set all values to default.
		//  hLogfile = Handle to a logger, which was retrieved from another PLCHandler instance by GetLogFile() or created by CAL_LogCreate(). 
		// NOTE:In this case, the communication parameters must be called interactive
		//		from user with GetConfigInteractive() or SetConfig(), before calling Connect()!!
		CPLCHandler(/*[In]*/ RTS_HANDLE hLogFile = RTS_INVALID_HANDLE);

		// Destructor
		virtual ~CPLCHandler(void);
		
		////////////////////////////////////////////////////////////////////////////////////////
		// Configuration and use of the logger

		// Enable/Disable logging
		virtual int SetLogging(/*[In]*/ int bEnable, /*[In]*/ unsigned long ulLogFilter = LOG_STD);
		// Get the state (Enable/Disable) of the logging 
		virtual int GetLogging(void);

		// Set file size and number of files, which are use for storing the logs
		virtual long SetLogFileCapacity(/*[In]*/ int iMaxFileSize, /*[In]*/ int iMaxFiles);

		// Configure the logger to use the same log file as another PLCHandler instance (retrieved using GetLogFile())
		virtual long SetLogFile(/*[In]*/ RTS_HANDLE hLogFile);

		// Set the log file specified by name (CmpLogger object is allocated by plchandler)
		// Before calling this method, the logger must be preinitalized by calling
		// SetLogging() and SetLogFileCapacity()
		virtual long SetLogFile(/*[In]*/ char *pszLogFile);

		// Get the logger object
		virtual RTS_HANDLE GetLogFile(void);

		// Add a entry to the logger:
		// CmpId: ComponentId (currently not evaluated)
		// iClassID: error class of the entry, see CmpLogItf.h (LOG_INFO, LOG_WARNING, LOG_ERROR, ...)
		// iErrorID: typically return value of the failed function
		// pszInfo and optional arguments: log string, printf format placeholders can be used  
		virtual long AddLogEntry(/*[In]*/ unsigned long CmpId, /*[In]*/ int iClassID, /*[In]*/ int iErrorID, /*[In]*/ char *pszInfo, /*[In]*/ ...);
		virtual long AddLogEntry(/*[In]*/ unsigned long CmpId, /*[In]*/ int iClassID, /*[In]*/ int iErrorID, /*[In]*/ char *pszInfo, /*[In]*/ va_list *pargList);

		////////////////////////////////////////////////////////////////////////////////////////
		// Get version 

		// Get version of the PLCHandler
		virtual unsigned long GetVersion(/*[InOut]*/ char *pszVersion = NULL, /*[In]*/ long lLen = 0);

		////////////////////////////////////////////////////////////////////////////////////////
		// Scan the PLC network

		// Method to scan the PLC network (only for V3). If GatewayConnection==NULL, then the ARTI3 interface is used.
		// The callback is called once for each answering PLC and additionally to signal the end of the scan.
		// This last callback returns the result code of the scan.
		virtual long ScanNetwork(/*[In]*/ GatewayConnection *pGatewayConnection, 
								 /*[In]*/ CPLCHandlerCallback *pPlcFoundCallback);

		////////////////////////////////////////////////////////////////////////////////////////
		// Set and get the configuration

		// Set PLCHandler configuration out of an ini-File
		virtual long SetConfig(/*[In]*/ char *pszIniFile);
		// Set PLCHandler configuration by structures
		virtual long SetConfig(/*[In]*/ PlcConfig *pPlcConfig, /*[In]*/ PlcDeviceDesc *pDeviceDesc);
		// Set PLCHandler configuration with an ini-File string
		virtual long SetConfig(/*[In]*/ char *pszConfig, /*[In]*/ unsigned long ulConfigLen, /*[In]*/ char *pszLineEnd);

		// Set communication parameters interactive from user
		virtual long SetConfigInteractive(void);

		// Get communication parameters
		// ATTENTION: If *ppPlcConfig!=NULL or *ppDeviceDesc!=NULL, the config will be copied to the parameters,
		//            in the other case you get a pointer to the internal object!
		virtual long GetConfig(/*[Out]*/ PlcConfig **ppPlcConfig, /*[Out]*/ PlcDeviceDesc **ppDeviceDesc);

		// Save configuration in ini-File.
		virtual long SaveConfigInFile(/*[In]*/ char *pszIniFile);

		// Set/get timeout for communication
		virtual long SetTimeout(/*[In]*/ unsigned long ulTimeout);
		virtual unsigned long GetTimeout(void);

		// Get name of PLCHandler
		virtual char *GetName(void);
		// Get index of PLCHandler
		virtual unsigned long GetId(void);

		////////////////////////////////////////////////////////////////////////////////////////
		// Connection handling

		// With the connect function, the communication to the plc will be opened
		virtual long Connect(/*[In]*/ unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT,
							 /*[In]*/ CPLCHandlerCallback *pStateChangedCallback = NULL,
							 /*[In]*/ int bLoadSymbols = 1);

		// With the disconnect function, the communication to the plc will be closed
		// NOTE: Is called implicit in class desctructor
		virtual long Disconnect(void);

		// Get the actual state of the plc communication		
		virtual PLCHANDLER_STATE GetState(void);

		// Get the last occurred error
		virtual long GetLastError(void);

		// Set/get the state changed callback
		virtual long SetStateChangedCallback(CPLCHandlerCallback *pCallback);
		virtual CPLCHandlerCallback *GetStateChangedCallback(void);

		// Get the number of reconnect already done
		virtual long GetReconnectCycles(void);

		// Enable/Disable keepalive thread
		virtual long StartKeeplive(void);
		virtual long StopKeeplive(void);

		////////////////////////////////////////////////////////////////////////////////////////
		// Methods to ask to PLC for all available variables

		// This routine can be used to load the symbols from the PLC on an active connection,  
		// if the connect was done without loading the symbols (bLoadSymbols=0).
		virtual long LoadSymbolsFromPlc(void);

		// Must be called before GetAllItems(). This blocks all other online functions in the PLCHandler to
		// change the internal symbol list.
		virtual long EnterItemAccess(void); 

		// Must be called after the access of ppSymbolList (returned by GetAllItems), to release the internal 
		// symbol list. After calling this method you must not access the ppSymbolList anymore.
		virtual void LeaveItemAccess(void);

		// Get all items defined in the controller with name, datatype, size, address in the plc and the access rights
		// This routine can be used for browsing.
		virtual long GetAllItems(/*[Out]*/ PlcSymbolDesc **ppSymbolList, /*[Out]*/ unsigned long *pulNumOfSymbols);

		// Get the definition of an item specified by name. The call of this method and the access of pSymbol must be
		// protected by EnterItemAccess() and LeaveItemAccess().
		virtual long GetItem(/*[In]*/ char *pszSymbol, /*[Out]*/ PlcSymbolDesc *pSymbol);

		// Get the AddressString (e. g. %MB0) of a mapped item specified by name. The call of this method must be
		// protected by EnterItemAccess() and LeaveItemAccess().
		virtual long GetAddressOfMappedItem(/*[In]*/ char *pszSymbol, /*[InOut]*/ char *pszMappedAddr, /*[In]*/ long lLen);

		// Expand an item (if this is an Array or UserDef type) on the fly. The call of this method must be
		// protected by EnterItemAccess() and LeaveItemAccess().
		virtual long ExpandItem(/*[In]*/ char *pszSymbol, /*[Out]*/ PlcSymbolDesc **ppSymbolList, /*[Out]*/ unsigned long *pulNumOfSymbols);
		virtual long ReleaseExpandedItems(/*[In]*/ PlcSymbolDesc *pSymbolList);
		
		////////////////////////////////////////////////////////////////////////////////////////
		// Methods to work with cyclic variable lists

		// Define a new varlist, which is cyclically read by the PLCHandler
		virtual HCYCLIST CycDefineVarList(/*[In]*/ char **ppszSymbols, /*[In]*/ unsigned long ulNumOfSymbols,
										  /*[In]*/ unsigned long ulUpdateRate, /*[In]*/ CPLCHandlerCallback *pUpdateReadyCallback = NULL,
										  /*[In]*/ CPLCHandlerCallback *pDataChangeCallback = NULL, /*[In]*/ RTS_HANDLE hUpdateEvent = 0,
										  /*[In]*/ unsigned long ulFlags = 0, /*[Out]*/ long *plResult = NULL);
		// ppszSymbols:			List of symbols as strings to add to the cyclic list
		// ulNumOfSymbols:		Numer of symbols in the list
		// ulUpdateRate:		Interval, in which the values of the list are updated
		// pUpdateReadyCallback:Event to get notified, if update has finished
		// pDataChangeCallback:	Event to get notified, if data in the cyclic list has changed
		// hUpdateEvent:		Event to trigger an update cycle
		// ulFlags:				Flags for the variable list (e. g. consistancy flags)
		// plResult:			Pointer to return the method's result

		// Reuse an existing list for new variables (combines CycDeleteVarList() and CycDefineVarList()).
		// Should only be used if the client application can not accept a new varlist handle. 
		virtual HCYCLIST CycUpdateVarList(/*[In]*/ HCYCLIST hCycVarList, /*[In]*/ char **ppszSymbols, /*[In]*/ unsigned long ulNumOfSymbols,
										  /*[In]*/ unsigned long ulUpdateRate, /*[In]*/ CPLCHandlerCallback *pUpdateReadyCallback = NULL,
										  /*[In]*/ CPLCHandlerCallback *pDataChangeCallback = NULL, /*[In]*/ RTS_HANDLE hUpdateEvent = 0,
										  /*[In]*/ unsigned long ulFlags = 0, /*[Out]*/ long *plResult = NULL);
		
		// Deletes a varlist.
		virtual long CycDeleteVarList(/*[In]*/ HCYCLIST hCycVarList, /*[In]*/ int bKeepalive = 1);
		// Enable the list: reenable cyclic update, after this was disabled by CycDisableList()
		virtual long CycEnableList(/*[In]*/ HCYCLIST hCycVarList);
		// Disable the list: disable cyclic update
		virtual long CycDisableList(/*[In]*/ HCYCLIST hCycVarList);
		// Checks if the handle is a valid varlist handle.
		virtual long CycIsValidList(/*[In]*/ HCYCLIST hCycVarList);
		// Must be called before CycReadVars(). This blocks the UpdateThread 
		// to change the values or delete the internal list because of an communication error.
		virtual long CycEnterVarAccess(/*[In]*/ HCYCLIST hCycVarList);
		// Must be called after the access of pppValues (returned by CycReadVars()), to release the  
		// varlist. After calling this method you must not access pppValues anymore.
		virtual void CycLeaveVarAccess(/*[In]*/ HCYCLIST hCycVarList);
		// Read a varlist from the cache. Must only be called within CycEnterVarAccess() and CycLeaveVarAccess().
		virtual long CycReadVars(/*[In]*/ HCYCLIST hCycVarList, /*[Out]*/ PlcVarValue ***pppValues, /*[Out]*/ unsigned long *pulNumOfValues);
		// Read the changed vars from the cache. Must only be called from DataChangeCallback.
		virtual long CycReadChangedVars(/*[In]*/HCYCLIST hCycVarList, /*[Out]*/PlcVarValue ***pppChangedValues, /*[Out]*/unsigned long **ppChangedValuesIndex, /*[Out]*/unsigned long *pulNumOfChangedValues);
		// Get the last update time for updating the varlist. The value is returned in % of the configured updaterate.
		virtual unsigned long CycGetOperatingRate(/*[In]*/ HCYCLIST hCycVarList);
		// Set/Get the update rate of the cyclic list.
		virtual long CycSetUpdateRate(/*[In]*/ HCYCLIST hCycVarList, /*[In]*/ unsigned long ulUpdateRate);
		virtual unsigned long CycGetUpdateRate(/*[In]*/ HCYCLIST hCycVarList);
		// Get the variable names of the cyclic list
		virtual long CycGetSymbolList(/*[In]*/ HCYCLIST hCycVarList, /*[Out]*/ char ***pppCycSymbolList);
		// Get the number of currently defined cyclic variable lists.	
		virtual unsigned long CycGetNumOfLists(void);
		// Get the varlist handle by index.
		virtual HCYCLIST CycGetVarListByIndex(/*[In]*/ unsigned long ulIndex);
		// Get the index of a varlist.
		virtual long CycGetVarListIndex(/*[In]*/HCYCLIST hCycVarList);
		// Add additional symbols to an existing varlist. (Only for V3, runtime system version >= V3.5)
		virtual long CycAddSymbolsToVarList(/*[In]*/ HCYCLIST hCycVarList, /*[In]*/ char **ppszSymbols, /*[In]*/ unsigned long ulNumOfSymbolsToAdd);
		// Remove symbols from an existing varlist. (Only for V3, runtime system version >= V3.5)
		virtual long CycRemoveSymbolsFromVarList(/*[In]*/ HCYCLIST hCycVarList, /*[In]*/ char **ppszSymbols, /*[In]*/ unsigned long ulNumOfSymbolsToRemove);
		// Remove symbols from an existing varlist by index numbers of the existing list. (Only for V3, runtime system version >= V3.5)
		virtual long CycRemoveSymbolsFromVarList(/*[In]*/ HCYCLIST hCycVarList, /*[In]*/ unsigned long *pulRemoveIndexList, /*[In]*/ unsigned long ulNumOfSymbolsToRemove);

		////////////////////////////////////////////////////////////////////////////////////////
		// Methods for synchronous variable access

		// Defining/deleting varlists for synchronous variable access
		virtual HVARLIST SyncDefineVarList(/*[In]*/ char **ppszSymbols, /*[In]*/ unsigned long ulNumOfSymbols,
										   /*[In]*/ unsigned long ulFlags = 0, /*[Out]*/ long *plResult = NULL);
		virtual long SyncDeleteVarList(/*[In]*/ HVARLIST hSyncVarList);

		virtual long SyncReadVarListFromPlc(/*[In]*/ HVARLIST hSyncVarList, /*[Out]*/ PlcVarValue ***pppValues, /*[Out]*/ unsigned long *pulNumOfValues);
		virtual long SyncWriteVarListToPlc(/*[In]*/ HVARLIST hSyncVarList, /*[In]*/ unsigned long ulNumOfSymbols, /*[In]*/ unsigned char **ppbyValues, /*[In]*/ unsigned long *pulValueSizes = NULL);

		// Reading variables by an implicit list
		virtual HVARLIST SyncReadVarsFromPlc(/*[In]*/ char **ppszSymbols, /*[In]*/ unsigned long ulNumOfSymbols,
											 /*[Out]*/ PlcVarValue ***pppValues, /*[Out]*/ unsigned long *pulNumOfValues);
		virtual long SyncReadVarsFromPlc(/*[In]*/ HVARLIST hVarList, /*[Out]*/ PlcVarValue ***pppValues, /*[Out]*/ unsigned long *pulNumOfValues);
		virtual long SyncReadVarsFromPlcReleaseValues(/*[In]*/ HVARLIST hSyncRead);
		// Writing variables by an implicit list
		virtual long SyncWriteVarsToPlc(/*[In]*/ char **ppszSymbols, /*[In]*/ unsigned long ulNumOfSymbols, /*[In]*/ unsigned char **ppbyValues, /*[In]*/ unsigned long *pulValueSizes = NULL);

		////////////////////////////////////////////////////////////////////////////////////////
		// File and directory functions

		// File transfer functions with the PLC
		virtual long UploadFile(/*[In]*/ char *pszPlc, /*[In]*/ char *pszHost = NULL);
		virtual long DownloadFile(/*[In]*/ char *pszHost, /*[In]*/ char *pszPlc = NULL);

		// File functions on the PLC 
		virtual long RenameFile(/*[In]*/ char *pszOldFile, /*[In]*/ char *pszNewFile);
		virtual long DeleteFile(/*[In]*/ char *pszFileName);

		// Directory routines on the PLC 
		virtual long ReadDirectory(/*[Out]*/ CDirInfo **ppdi, /*[In]*/ char *pszBaseDir = NULL);
		virtual long CreateDirectory(/*[In]*/ char *pszDirectoryName); /* (only for V3) */
		virtual long RenameDirectory(/*[In]*/ char *pszOldDirectory, /*[In]*/ char *pszNewDirectory); /* (only for V3) */
		virtual long DeleteDirectory(/*[In]*/ char *pszDirectoryName); /* (only for V3) */

		////////////////////////////////////////////////////////////////////////////////////////
		// Retrieve project and application information from the PLC

		// Get a list with the names of all applications in the PLC (only for V3)
		virtual long GetApplicationList (/*[Out]*/ char ***pppszApplications, /*[Out]*/ unsigned long *pulNumOfApplications);
		// Check if there is a boot application file and/or an project archive file (only for V3) on the PLC
		virtual long CheckApplicationFileConsistency(/*[In]*/ char *pszApplication, /*[Out]*/ long *plBootProjectResult, /*[Out]*/ long *plArchiveResult = NULL);
		// Get ProjectInfo and the ApplicationInfo of the specified application (only for V3)
		virtual long GetApplicationInfo(/*[In]*/ char *pszApplication, /*[Out]*/ ProjectInfo **ppPrjInfo, /*[Out]*/ ApplicationInfo **ppAppInfo);
		virtual long GetApplicationInfo(/*[In]*/ char *pszApplication, /*[Out]*/ ProjectInfo **ppPrjInfo, /*[Out]*/ ApplicationInfo2 **ppAppInfo);
		// Get ProjectInfo of the PLC project (V2) or the ProjectInfo for the first application, which contains this info (V3) 
		virtual long GetProjectInfo(/*[Out]*/ ProjectInfo **ppPrjInfo);

		////////////////////////////////////////////////////////////////////////////////////////
		// Get and set the status of the application/PLC

		// Get/set/reset the status (RUN/STOP) of the PLC
		virtual long GetPlcStatus(/*[Out]*/ PLC_STATUS *pPlcStatus);
		virtual long SetPlcStatus(/*[In]*/ PLC_STATUS PlcStatus);
		virtual long ResetPlc(/*[In]*/ RESET_OPTION ResetCommand);
		// Get/set/reset the status (RUN/STOP) of the PLC applications (only for V3)
		virtual long GetApplicationStatus(/*[Out]*/ PLC_STATUS *pAppStatus, /*[In]*/ char *pszApplication = NULL);
		virtual long SetApplicationStatus(/*[In]*/ PLC_STATUS AppStatus, /*[In]*/ char *pszApplication = NULL);
		virtual long ResetApplication(/*[In]*/ RESET_OPTION ResetCommand, /*[In]*/ char *pszApplication = NULL);

		////////////////////////////////////////////////////////////////////////////////////////
		// Application related operations

		// Register and reload the bootproject on the PLC (only for V3) 
		virtual long RegisterBootApplication(/*[In]*/ char *pszApplication);
		virtual long ReloadBootApplication(/*[In]*/ char *pszApplication = NULL);

		// Reload the bootproject on the PLC 
		virtual long ReloadBootproject(void);

		// Save/Restore retains (V2 and V3)
		virtual long SaveRetains(/*[InOut]*/ char *pszRetainFile, /*[In]*/ long lBufferLen, /*[In]*/ char *pszApplication = NULL);
		virtual long RestoreRetains(/*[InOut]*/ char *ppszRetainFile, /*[In]*/ long lBufferLen, /*[In]*/ char *pszApplication = NULL);

		// Backup and Restore all IEC Applications
		virtual long BackupIECApplications( /*[In]*/ char *pszBackupFilePath, /*[In]*/ CPLCHandlerCallback *pBackupResultCallback, /*[In]*/ int bForceBackup = 0, /*[In]*/ int bCreateTbf = 0);
		virtual long RestoreIECApplications( /*[In]*/ char *pszRestoreFilePath, /*[In]*/ CPLCHandlerCallback *pRestoreResultCallback, /*[In]*/ int bStartBootprojects = 1);

		////////////////////////////////////////////////////////////////////////////////////////
		// Device functions

		// Execute a reset origin on the device (only for V3)
		virtual long ResetOriginDevice(void);
		// Get/set the device operation mode (only for V3)
		virtual long GetDeviceOperationMode(DEVICE_OPERATION_MODE *pOpMode);
		virtual long SetDeviceOperationMode(DEVICE_OPERATION_MODE OpMode);

		// Check TargetId of the PLC (only for V2)
		virtual long CheckTarget(/*[In]*/ unsigned long ulTargetId, /*[In]*/ unsigned long ulHookId = 0, /*[In]*/ unsigned long ulMagic = 0);

		// Get the DeviceInfo from the PLC (only for V3)
		virtual long GetDeviceInfo(/*[Out]*/ DeviceInfo **ppDeviceInfo);
		virtual long GetDeviceInfo(/*[Out]*/ DeviceInfo2 **ppDeviceInfo);

		// Change the node name of the PLC (only for V3)
		virtual long RenameDevice(/*[In]*/ RTS_WCHAR *pwszNodeName);

		// Send echo service to PLC e. g. to measure the communication performance (only for V3)
		virtual long SendPlcEcho(/*[InOut]*/ unsigned long *pulSendDataLen, /*[InOut]*/ unsigned long *pulReceiveDataLen);

		////////////////////////////////////////////////////////////////////////////////////////
		// Miscellaneous

		// Enter the internal OnlineAccess semaphore to synchronize caller actions with the PLCHandler's update threads 
		virtual long EnterOnlineAccess(/*[In]*/ unsigned long ulTimeout = PLCHANDLER_TIMEOUT_INFINITE);
		// Leave the internal OnlineAccess semaphore
		virtual long LeaveOnlineAccess(void);

		// Get the communication object (only for internal use and test purposes)
		virtual CPLCComBase *GetPlcComObject(void);

		// CustomerData to store client data related to the instance of the PLCHandler.
	 	// This data is guaranteed to be untouched by the PLCHandler.
		unsigned long ulCstData;
		void* pCstData; 

	protected:
		// This routines can only be used, if you derive your class from CPLCHandler.
		// Attention: The structure of the service must be known in detail! 
		//            An erroneous service might cause a crash of the PLC! 
	
		// Send own service to plc and receive reply
		virtual long SyncSendService(/*[In]*/ unsigned char *pbySend, /*[In]*/ unsigned long ulSendSize, /*[Out]*/ unsigned char **ppbyRecv, /*[Out]*/ unsigned long *pulRecvSize);

		// Send own service to plc
		virtual long AsyncSendService(/*[Out]*/ int *piInvokeId, /*[In]*/ unsigned char *pbySend, /*[In]*/ unsigned long ulSendSize, /*[In]*/ CPLCHandlerCallback *pAsyncServiceCallback = NULL);
		// Get service reply
		virtual long AsyncGetServiceReply(/*[In]*/ int *piInvokeId, /*[Out]*/ unsigned char **ppbyRecv, /*[Out]*/ unsigned long *pulRecvSize, /*[Out]*/ long *plServiceResult);

		// Get SessionId for Device services (only used for GW3 and ARTI3) 
		virtual unsigned long GetDeviceSessionId(void);

		// Protected member variables
		RTS_I32 m_i32InstanceNumber;

	private:
		// Private member methods
		virtual void Init(RTS_HANDLE hLogger = RTS_INVALID_HANDLE);
		virtual unsigned long GetPlcIdbyName(char *pszPlcName, char *pszIniFile);
		virtual void ValidateAndReadConfig(void);
		virtual void PrintConfig(void);
		virtual long ConnectPlc(void);
		virtual long Reconnect(long *pbExit);
		virtual long DisconnectPlc(void);
		virtual long LoadSymbols(void);
		virtual long LoadSymbolsOffline(void);
		virtual long DeleteSymbols(void);
		virtual long StartReconnectThread(void);
		virtual long TerminateReconnectThread(void);
		virtual long StartUpdateThread(CycVarList* pCycVarList);
		virtual long TerminateUpdateThread(CycVarList* pCycVarList);
		virtual long ExitAllUpdateThreads(int bWaitForExitAllThreads);
		virtual void InitState(void);
		virtual void SetState(PLCHANDLER_STATE iState);
		virtual void SetLastError(long lError);
		virtual long AddLogEntry(int iClassID, int iErrorID, char *pszInfo, ...);
		virtual long HandleOnlineResult(long lResult);
		virtual long HandleOnlineErrorResult(long lResult);
		virtual unsigned long GetRetries(void);
		virtual void CycEnterListAccess(void);
		virtual void CycLeaveListAccess(void);
		virtual unsigned long GetIndex(char* pszSymbol, PlcSymbolDesc* pSymbolList, unsigned long ulNumOfSymbols);

		virtual void CycDeleteVarListTable(void);
		virtual CycVarList* CycCreateVarList(void);
		virtual void CycDeleteVarListFromTable(CycVarList *pCycVarList, long bDelete = 1);
		virtual long CycClearVarList(CycVarList *pCycVarList);
		virtual long CycSendUpdateReadyCallback(HCYCLIST hCycVarList, PlcVarValue **ppChangedValues = NULL, unsigned long ulNumOfValues = 0);
		virtual long CycPrepareDatachangeCallback(CycVarList *pCycVarList);
		virtual long CycGenerateDatachangeCallback(CycVarList *pCycVarList);
		virtual long CycSendDatachangeCallbackForPlc(CycVarList *pCycVarList, unsigned long* pulChangedValuesIndex, unsigned long ulNumOfChangedValues);
		virtual long CycRemoveSymbolsFromVarListInternal(HCYCLIST hCycVarList, char **ppszSymbols, unsigned long *pulRemoveIndexList, unsigned long ulNumOfSymbols);
		virtual long CreateKeepAliveThread(void);
		virtual long DeleteKeepAliveThread(void);
		virtual long CreateAsyncMgrThread(void);
		virtual long DeleteAsyncMgrThread(void);

		virtual long EnterOnlineAccessWithStateCheck(unsigned long ulTimeout = PLCHANDLER_TIMEOUT_INFINITE);
		virtual long WaitSleep(SYS_TASK_PARAM *ptp, unsigned long *pulMsToSleep, unsigned long ulStartTime, unsigned long ulStepMs);

		static void CDECL PlcFoundCallback (RTS_UINTPTR ulUserData, NodeInfotyp2 *pNodeInfo2);
		static long VerifyCallback(void* pUserData, char* pszBase64PlcCert, unsigned long ulBase64PlcCertSize, long lVerifyResult);

		// Private member variables
		PlcConfig* m_pPlcConfig;
		PlcDeviceDesc* m_pDeviceDesc;

		CPLCHandlerCallback *m_pPlcFoundCallback;
		CPLCComBase *m_pplccomScanNetwork;		
		CPLCComBase *m_pplccom;		
		PLCHANDLER_STATE m_iState;
		RTS_HANDLE m_hReconnectThread;

		RTS_HANDLE m_hsemCycListAccess;
		RTS_HANDLE m_hsemOnlineAccess;
		RTS_HANDLE m_hsemConnect;
		RTS_HANDLE m_hsemReconnect;
		RTS_HANDLE m_hsemCycListRef;
		CPLCHandlerCallback *m_pStateChangedCallback;
		RTS_HANDLE m_hSleep;

		unsigned long m_ulCycVarLists;
		CycVarList **m_ppCycVarListTable;

		UpdatethreadWaitExitTag *m_pUpdateThreadExitTable;
		unsigned long m_ulNumUpdateThreadsToExit;
		unsigned long m_ulMaxUpdateThreadsToExit;

		RTS_HANDLE m_hKeepAliveThread;
		RTS_HANDLE m_hLogger;
		IBase *m_pILogBackendBase;
		int m_iMaxFileSize;
		int m_iMaxFiles;
		int m_bDeleteLogger;
		int m_bLoadSymbols;
		int m_bLoadSymbolsOffline;
		int m_bWriteThroughReadCache;
		int	m_bDataChangeOnPlc;
		int m_bUseLocalTimeInLogfile;
		int m_iReconnect;
		int m_bConnected;
		int m_bLogout;
		int m_bKeepAliveActive;
		int m_iItemAccessRefCount;

		long m_lLastError;
		int m_bConfigIsPrinted;

		RTS_HANDLE m_hAsyncMgrThread;
		RTS_HANDLE m_hsemAsyncMgr;
		AsyncServiceTag m_AsyncSrvList[MAX_ASYNC_COMMANDS];
		int m_iAsyncListReadPtr;
		int m_iAsyncListWritePtr;
		int m_iAsyncListLevel;
		int m_iAsyncListWorkPtr;
		int m_iAsyncListWorkLevel;

		// Private threads				
		friend void CDECL UpdateThread(SYS_TASK_PARAM *ptp);
		friend void CDECL ReconnectThread(SYS_TASK_PARAM *ptp);
		friend void CDECL KeepAliveThread(SYS_TASK_PARAM *ptp);
		friend void CDECL AsyncMgrThread(SYS_TASK_PARAM *ptp);

		friend class CycVarList;
		friend class SyncVarList;
};


class PLCH_DLL_DECL CEasyPLCHandler : public CPLCHandler
{
	public:
		CEasyPLCHandler(RTS_HANDLE hLogFile = RTS_INVALID_HANDLE);
		CEasyPLCHandler(PlcConfig *pPlcConfig, PlcDeviceDesc *pDeviceDesc, RTS_HANDLE hLogFile = RTS_INVALID_HANDLE);
		CEasyPLCHandler(unsigned long ulId, char *pszIniFile, RTS_HANDLE hLogFile = RTS_INVALID_HANDLE);
		CEasyPLCHandler(char *pszPlcName, char *pszIniFile, RTS_HANDLE hLogFile = RTS_INVALID_HANDLE);
		virtual ~CEasyPLCHandler(void);

		// See PLCConfig.h for defines of pszProtocol
		// e.g. #define PLCC_DN_TCPIP_L2ROUTE	"Tcp/Ip (Level 2 Route)"
		virtual long ConnectTcpipViaGateway(char *pszGatewayIP, char *pszPlcIP, char *pszProtocol = (char*)PLCC_DN_TCPIP_L2ROUTE, int bMotorola = 0, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, unsigned long ulPort = 1200, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectRs232ViaGateway(char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola = 0, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectRs232ViaGatewayEx(char *pszGatewayIP, short sPort, unsigned long ulBaudrate, int bMotorola = 0, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, EXT_RS232_PARAMStyp *pExtParams = NULL, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectTcpipViaArti(char *pszPlcIP, char *pszProtocol = (char*)PLCC_DN_TCPIP_L2ROUTE, int bMotorola = 0, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, unsigned long ulPort = 1200, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectRs232ViaArti(short sPort, unsigned long ulBaudrate, int bMotorola = 0, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectToSimulation(char *pszSdbFile, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectViaGateway3(char *pszGatewayIP, char *pszAddress, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectViaGateway3Ex(char *pszGatewayIP, unsigned long ulPort, char *pszAddress, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectViaGateway3ByName(char *pszGatewayIP, unsigned long ulPort, RTS_WCHAR *pwszDeviceName, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectViaArti3(char *pszAddress, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectViaArti3ByName(RTS_WCHAR *pwszDeviceName, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectToSimulation3(char *pszSdb3XmlFile, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectTcpipViaGateway3(char *pszGatewayIP, unsigned long ulPort, char *pszPlcIP, unsigned long ulPlcPort = 11740, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		virtual long ConnectTcpipViaArti3(char *pszPlcIP, unsigned long ulPlcPort = 11740, int bLoadSymbols = 1, unsigned long ulTimeout = PLCHANDLER_USE_DEFAULT, CPLCHandlerCallback* pStateChangeCallback = NULL);
		
		// Attention: The structure of the service must be known in detail! 
		//            An erroneous service might cause a crash of the PLC! 
		// Send own service to plc and receive reply
		virtual long SyncSendService(/*[In]*/ unsigned char *pbySend, /*[In]*/ unsigned long ulSendSize, /*[Out]*/ unsigned char **ppbyRecv, /*[Out]*/ unsigned long *pulRecvSize);
		// Send own service to plc
		virtual long AsyncSendService(/*[Out]*/ int *piInvokeId, /*[In]*/ unsigned char *pbySend, /*[In]*/ unsigned long ulSendSize, /*[In]*/ CPLCHandlerCallback *pAsyncServiceCallback = NULL);
		// Get service reply
		virtual long AsyncGetServiceReply(/*[In]*/ int *piInvokeId, /*[Out]*/ unsigned char **ppbyRecv, /*[Out]*/ unsigned long *pulRecvSize, long *plServiceResult);
		// Get SessionId for Device services (only used for GW3 and ARTI3) 
		virtual unsigned long GetDeviceSessionId(void);
};


class PLCH_DLL_DECL CPLCHandlerCallback
{
	public:
		CPLCHandlerCallback(unsigned long hOSEvent);
		CPLCHandlerCallback(RTS_HANDLE hEvent = NULL, int bDeleteEvent = 0);
		virtual ~CPLCHandlerCallback(void);

		virtual long Notify(CPLCHandler *pPlcHandler, CallbackAddInfoTag CallbackAdditionalInfo);
		virtual long VerifyPlcCertificate(CPLCHandler *pPlcHandler, char* pszBase64PlcCert, unsigned long ulBase64PlcCertSize, long lVerifyResult);
	
	protected:
		RTS_HANDLE m_hEvent;
		int m_bDeleteEvent;
};

// redefine #defines from winbase.h
#if defined (WIN32) && defined (UNICODE) && !defined(_WIN32_WCE) && !defined(OPC_SERVER_3S)
	#undef CreateDirectory
	#define CreateDirectory  CreateDirectoryW
#endif

#if defined (WIN32) && defined (UNICODE) && !defined(_WIN32_WCE) && !defined(OPC_SERVER_3S)
	#undef DeleteFile
	#define DeleteFile  DeleteFileW
#endif

#endif	//__PLCHANDLER_H__
