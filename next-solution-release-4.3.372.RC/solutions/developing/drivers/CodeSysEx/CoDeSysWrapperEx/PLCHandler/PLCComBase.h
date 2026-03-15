#ifndef __PLCCOMBASE_H__
#define __PLCCOMBASE_H__

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

#include "PLCHandlerDefines.h"
#include "PLCConfig.h"
#include "HelperClasses.h"

#include "SysEventItf.h"
#include "SysSemItf.h"
#include "CmpLogItf.h"


// undef #defines from winbase.h
#if defined (WIN32) && defined (UNICODE) && !defined(_WIN32_WCE) && !defined(OPC_SERVER_3S)
	#undef CreateDirectory
	#define CreateDirectory  CreateDirectoryA
#endif


#define SECTION_SERVER					"Server"
#define SECTION_PLC_PREFIX				"PLC:"

#define KEY_INTERFACETYPE				"interfacetype"
#define KEY_LOGEVENTS					"logevents"
#define KEY_LOGFILTER					"logfilter"
#define KEY_NUM_OF_PLCS					"PLCs"
#define KEY_PLC_PREFIX					"PLC"
#define KEY_PLC_ACTIVE					"active"
#define KEY_PLC_MOTOROLA				"motorola"
#define KEY_PLC_NOLOGIN					"nologin"
#define KEY_PLC_PRECHECKIDENTITY		"precheckidentity"
#define KEY_PLC_TIMEOUT					"timeout"
#define KEY_PLC_TRIES					"tries"
#define KEY_PLC_WAITTIME				"waittime"
#define KEY_PLC_RECONNECTTIME			"reconnecttime"
#define KEY_PLC_BUFFERSIZE				"buffersize"
#define	KEY_PLC_MAX4VERSION				"max4version"
#define KEY_PLC_PROJECT					"project"
#define KEY_PLC_DLL_DIRECTORY			"dlldirectory"
#define KEY_PLC_DEVICE					"device"
#define KEY_PLC_INSTANCE				"instance"
#define KEY_DEVICE_NUM_OF_PARAMETERS	"parameters"

#define KEY_PLC_GATEWAY					"gateway"
#define KEY_PLC_GATEWAYADDRESS			"gatewayaddress"
#define KEY_PLC_GATEWAYPORT				"gatewayport"
#define KEY_PLC_GATEWAYPASSWORD			"gatewaypassword"

#define VALUE_SIMULATION				"SIMULATION"
#define VALUE_ARTI						"ARTI"
#define VALUE_GATEWAY					"GATEWAY"
#define VALUE_DIRECT					"DIRECT"
#define VALUE_GATEWAY3					"GATEWAY3"
#define VALUE_SIMULATION3				"SIMULATION3"
#define VALUE_ARTI3						"ARTI3"

#define PARAMETER_PREFIX				"parameter"
#define VALUE_PREFIX					"value"
#define INVALID_PARAMETER				"__This__is__NOT__a__valid__PaRaMeTeR__"
#define INVALID_VALUE					"__This__is__NOT__a__valid__VaLuE__"
#define INVALID_WSTRING					L"__This__is__NOT__a__valid__WsTrInG__"

/* Defines for parameters in the config file - !!! obsolete !!! */
#define PARAMETER_DEVICE_IP_ADDRESS				PLCC_PN_IP_ADDRESS
#define PARAMETER_DEVICE_PORT					PLCC_PN_PORT
#define PARAMETER_DEVICE_ADDRESS				PLCC_PN_ADDRESS
#define PARAMETER_DEVICE_MOTOROLA				PLCC_PN_MOTOROLA
#define PARAMETER_DEVICE_TARGET					PLCC_PN_TARGET
#define PARAMETER_DEVICE_BLOCKSIZE				PLCC_PN_BLOCKSIZE
#define PARAMETER_DEVICE_BAUDRATE				PLCC_PN_BAUDRATE
#define PARAMETER_DEVICE_PARITY					PLCC_PN_PARITY
#define PARAMETER_DEVICE_STOPBITS				PLCC_PN_STOPBITS
#define PARAMETER_DEVICE_USER					PLCC_PN_USER
#define PARAMETER_DEVICE_PASSWORD				PLCC_PN_PASSWORD
#define PARAMETER_SYMBOLFILE					PLCC_PN_SYMBOLFILE
#define PARAMETER_PING							PLCC_PN_PING
#define PARAMETER_DIRECT_ADDR_ONLY				PLCC_PN_DIRECT_ADDR_ONLY
#define PARAMETER_SYMBOLFILE_PATH				PLCC_PN_SYMBOLFILE_PATH
#define PARAMETER_CLIENT_ADDRESS_RES			PLCC_PN_CLIENT_ADDR_RES
#define PARAMETER_DONT_EXPAND_ARRAYS			PLCC_PN_DONT_EXPAND_ARRAYS
#define PARAMETER_DONT_EXPAND_COMPLEX_ARRAYS	PLCC_PN_DONT_EXPAND_COMPLEX_ARRAYS
#define PARAMETER_WRITE_THROUGH_CACHE			PLCC_PN_WRITE_THROUGH_CACHE
#define PARAMETER_DATA_CHANGE_ON_PLC			PLCC_PN_DATA_CHANGE_ON_PLC
#define PARAMETER_DONT_LOAD_SYMBOLS				PLCC_PN_DONT_LOAD_SYMBOLS
#define PARAMETER_ALTERNATE_SYMBOLFILE			PLCC_PN_ALTERNATE_SYMBOLFILE

/* Defines for values in the config file - !!! obsolete !!! */
#define VALUE_DEVICE_TCPIP						PLCC_DN_TCPIP			// is identical with PLCC_DN_TCPIP_L4	
#define VALUE_DEVICE_TCPIP_L4					PLCC_DN_TCPIP_L4		// is identical with PLCC_DN_TCPIP
#define VALUE_DEVICE_TCPIP_L2ROUTE				PLCC_DN_TCPIP_L2ROUTE
#define VALUE_DEVICE_TCPIP_L2					PLCC_DN_TCPIP_L2	
#define VALUE_DEVICE_SERIAL						PLCC_DN_SERIAL		


// Error codes
#define PLCCOM_OK									0
#define PLCCOM_ERROR								-1
#define PLCCOM_NOT_SUPPORTED						-2
#define PLCCOM_EXCEPTION							-3

#define PLCCOM_DEVICE_NOTSUPPORTED					-101
#define PLCCOM_NOCHANNEL							-102
#define PLCCOM_ALREADYOPEN							-103
#define PLCCOM_WRONGMESSAGETYPE						-104
#define PLCCOM_NOMESSAGE							-105
#define PLCCOM_DATAFAILED							-106

#define PLCCOM_SQUEUEOVERFLOW						-201
#define PLCCOM_RQUEUEOVERFLOW						-202
#define PLCCOM_SERVICEPENDING						-203

#define PLCCOM_COMMFATAL							-301	// communication corrupt

#define PLCCOM_NOFILE								-401
#define PLCCOM_BUFFEROVERFLOW						-402

#define PLCCOM_INVALID_HANDLE						-501  
#define PLCCOM_2_MANY_CHANNELS						-502
#define	PLCCOM_NO_SYMBOLS							-503
#define PLCCOM_NO_TYPES								-504
#define PLCCOM_EOT									-505
#define PLCCOM_NO_NAME								-506
#define PLCCOM_READ_OVERFLOW						-507
#define PLCCOM_WRITE_OVERFLOW						-508
#define PLCCOM_ACTIVE_HANDLE						-509
#define PLCCOM_MULTIPLE_WRITES						-510
#define PLCCOM_SWAP_ERROR							-511
#define PLCCOM_PARSE_ERROR							-512
#define PLCCOM_SYMBOLS_CHANGED						-513	// project id mismatch
#define PLCCOM_SORT_ERROR							-514
#define PLCCOM_NO_PROJECT							-515	// no program on the controller
#define PLCCOM_SHUTDOWN								-516
#define PLCCOM_INVALID_PARAMETER					-517
#define PLCCOM_OUTOFMEMORY							-518
#define PLCCOM_INVALID_SYMBOLS						-519
#define PLCCOM_TARGET_MISMATCH						-520
#define PLCCOM_TO_MUCH_DATA							-521
#define PLCCOM_TIMEOUT								-522
#define PLCCOM_INVALID_SYMBOL						-523
#define PLCCOM_RETAIN_MISMATCH						-524
#define PLCCOM_NO_ACCESS_RIGHTS						-525
#define PLCCOM_DUPLICATE_PLC_NAME					-526
#define PLCCOM_SIZE_MISMATCH						-527
#define PLCCOM_CONSISTENT_ACCESS_TIMEOUT			-528
#define PLCCOM_SYNC_CONSISTENT_ACCESS_DENIED		-529
#define PLCCOM_PLC_FAILED							-530
#define PLCCOM_INVALID_ASCII_STRING					-531
#define PLCCOM_INVALID_STRING_LENGTH				-532
#define PLCCOM_APPLICATION_NOT_IN_STOP				-533
#define PLCCOM_APPLICATION_NOT_IN_RUN				-534
#define PLCCOM_OPERATIONMODE_NOT_IN_DEBUG			-535
#define PLCCOM_BACKUP_RESTORE_NOT_SUPPORTED			-536
#define PLCCOM_PLC_INCONSISTENT_STATE				-537
#define PLCCOM_PLC_INCOMPATIBLE						-538
#define PLCCOM_PLC_VERSION_INCOMPATIBLE				-539
#define PLCCOM_RETAIN_ERROR							-540
#define PLCCOM_APPLICATIONS_LOAD_ERROR				-541
#define PLCCOM_APPLICATIONS_START_ERROR				-542
#define PLCCOM_FILETRANSFER_ERROR					-543
#define PLCCOM_OPERATION_DENIED						-544
#define PLCCOM_FORCES_ACTIVE						-545
#define PLCCOM_META_VERSION_MISMATCH				-546
#define PLCCOM_PROTOCOL_MISMATCH					-547
#define PLCCOM_USER_ACCESS_DENIED					-548
#define PLCCOM_PLC_SERVICE_FAILED					-549
#define PLCCOM_NO_USR_MGT							-550
#define PLCCOM_ACTIVATE_USERMGMT					-551
#if 0 /* Firmware update is not released yet. */
#define PLCCOM_DEVICEMANAGEMENT_SERVICE_NOT_RUNNING	-552
#endif

// Error code macros
#define PLCCOM_SUCCEEDED(l)			(l>=0)
#define PLCCOM_FAILED(l)			(l<0)
#define PLCCOM_COMMFAILED(l)		(l==PLCCOM_COMMFATAL)
#define PLCCOM_IDMISMATCH(l)		((l==PLCCOM_NO_PROJECT) || (l==PLCCOM_SYMBOLS_CHANGED))


// Runtime system services
#define RTS_START					3
#define RTS_STOP					4
#define RTS_READ_STATUS				16
#define RTS_READ_IDENTITY		    17
#define RTS_CMD_RESET               19
#define RTS_FILE_WRITE_START		47
#define RTS_FILE_WRITE_CONT			48
#define RTS_FILE_READ_START			49
#define RTS_FILE_READ_CONT			50
#define RTS_FILE_READ_LIST			51	/* used by Elau */
#define RTS_FILE_READ_INFO			52
#define RTS_FILE_RENAME				53
#define RTS_FILE_DELETE				54
#define RTS_CHECKTARGETID			66
#define RTS_FILE_TRANSFER_DONE		67
#define RTS_FILE_DIR				72
#define RTS_BROWSERCOMMAND			146

// Runtime system error codes
#define RTS_OK						0		/* Service treated correctly */
#define SRV_ERROR					40
#define SRV_NO_SERVICE				50		/* No valid service (or not implemented) */
#define SRV_FILE_ERROR				80
#define SRV_TEST_MESSAGE			1000	/* Reply after login service */

// Flags for ReadDirectory
#define FILE_FILE		0x00
#define FILE_DIRECTORY	0x01
#define FILE_DRIVE		0x02


#define RTS_FILE_DOWNLOAD_BLOCKSIZE	1000

#define PLCCOM_TIMEOUT_INFINITE		0xFFFFFFFF

typedef void* HVARLIST;

/* internal flags for variables in a varlist */
#define VARFLAG_NONE            0x00000000
#define VARFLAG_FORCE_READ      0x00000001
#define VARFLAG_READ_DONE		0x00000002

/* Size of the buffer in the memory for string values in the PLCHandler.ini file (max. strlen = buf. size - 1). */
#define PLCCOM_INI_FILE_STR_VALUE_BUF_SIZE (256u)

struct PLCH_DLL_DECL PlcSymbolDesc
{
	PlcSymbolDesc(void);
	~PlcSymbolDesc(void);

	char*			pszName;
	unsigned long	ulTypeId;	// Type class. HIWORD/LOWORD see definitions in HelperClasses.h
	char*			pszType;
	unsigned short	usRefId;
	unsigned long	ulOffset;
	unsigned long	ulSize;
	char			szAccess[2];
	unsigned char	bySwapSize;
};

typedef enum
{
	PLCCOMSTATUS_OK,
	PLCCOMSTATUS_NOT_INITIALIZED,
	PLCCOMSTATUS_NO_DLL_FOUND,
	PLCCOMSTATUS_NOT_SUPPORTED,
	PLCCOMSTATUS_NO_ENCRYPTION_POSSIBLE
} PLCCOM_STATUS;

struct PlcFileNameHeader
{
	unsigned short wMessage;
	unsigned short usFileNameLen;
	char           szFileName[2];
};

struct PlcFileName
{
	unsigned short usFileNameLen;
	char           szFileName[2];
};

struct PlcFileReadReply
{
	unsigned short wError;
	unsigned short bContinue;
	unsigned short usCount;
	unsigned char  byData[1];
};

struct PlcFileWriteStartHeader
{
	unsigned char  byMessage;
	unsigned char  bContinue;
	unsigned short usCount;
	unsigned short usFileNameLen;
	char           szFileName[2];
};

struct PlcFileWriteContHeader
{
	unsigned char  byMessage;
	unsigned char  bContinue;
	unsigned short usCount;
	unsigned char  byData[1];
};

struct PlcFileTransferDoneHeader
{
	unsigned char  byMessage;
	unsigned long  ulAdditionalDataLen;
	unsigned long  ulCheckSum;
};

struct PlcDirEntry
{
	char bFlags;
	unsigned char byName[1];
};

struct PlcDirReply
{
	unsigned short wError;
	unsigned short wSubService;
	unsigned short usCount;
	PlcDirEntry DirEntry[1];
};

struct PlcVarValue
{
	unsigned long	ulTimeStamp;
	unsigned char	bQuality;
	unsigned char	byData[1];
};

struct PLCH_DLL_DECL _DirInfo
{
	_DirInfo(void);
	~_DirInfo(void);

	int bDirectory;
	char *pszEntry;
};

class PLCH_DLL_DECL CDirInfo
{
	public:
		CDirInfo();
		~CDirInfo();

		long GetNumOfEntries(void);
		long GetEntry(char **ppszEntry, int *pbDirectory, long lEntry);
		long AddEntry(char *szEntry, int bDirectory);
		long AddExtra(unsigned int uiExtra);
		long GetExtra(unsigned int *puiExtra, long lEntry);

	private:
		_DirInfo	**m_ppdi;
		long m_lEntries;
};

class PLCH_DLL_DECL CMemAllocator
/* Allocates big memory blocks and provide small slices of it to reduce number of system memory allocations.
   - not threadsafe!
   - memory can only completely freed with destructor */
{
	public:
		CMemAllocator(int iMinAllocSize = 10*1024, int iMaxAllocSize = 1024*1024);
		~CMemAllocator(void);
		char* GetMem(int iLen);

	protected:
		char *m_pFirstBlock;

	private:
		int m_iMinAllocSize;
		int m_iMaxAllocSize;
		int m_iAllocSize;
		char *m_pCurrentBlock;
		char *m_pFreeMem;
		int m_iFreeMemSize;
		int m_iAllocationCount;
};

class PLCH_DLL_DECL CStringAllocator : public CMemAllocator
/* Store strings in the MemAllocater. Option AvoidDuplicates checks if an string is already stored
   and returns in this case the pointer to the existing string to reduce memory usage. 
   - not threadsafe!
   - memory can only completely freed with destructor */
{
	public:
		CStringAllocator(int bAvoidDuplicates = 0, int iMinAllocSize = 10*1024, int iMaxAllocSize = 1024*1024);
		~CStringAllocator(void);
		char* StoreString(char *pszString);

	private:
		char* GetMem(int iLen);
		int m_bAvoidDuplicates;
};

typedef void (*PFSCANNETWORKCALLBACK) (RTS_UINTPTR ulUserData, NodeInfotyp2 *pNodeInfo2);
typedef long(*PFVERIFYCERTCALLBACK)(void* pUserData, char* pszBase64PlcCert, unsigned long ulBase64PlcCertSize, long lVerifyResult);
typedef long(*PFREQUESTCREDENTIALSCALLBACK)(void * const pUserData, RequestCredentialsType requestCredentialsType, RTS_UTF8STRING * const pUserName, unsigned long userNameBufSize, RTS_UTF8STRING * const pPassword, unsigned long passwordBufSize);

class Hash;

class PLCH_DLL_DECL CPLCComBase
{
	public:
		// Construction with specific PLC configuration 
		CPLCComBase(PlcConfig *pConfig, PlcDeviceDesc *pDevice, RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

		virtual ~CPLCComBase(void);

		virtual PLCCOM_STATUS GetStatus(void);
		
		virtual long ScanNetwork(RTS_UINTPTR ulUserData, PFSCANNETWORKCALLBACK pfCallback);
		virtual long GetConfigInteractive(PlcConfig *pPlcConfig, PlcDeviceDesc *pDeviceDesc);

		virtual void SetLogger(RTS_HANDLE hLogger);
		virtual int SetLogging(int bEnable, unsigned long ulLogFilter = LOG_STD);
		virtual long AddLogEntry(int iClassID, int iErrorID, char *pszInfo, ... );

		virtual long Open(void) = 0;
		virtual long Close(void) = 0;
		virtual long CreateSession(RTS_UTF8STRING * const pszClientName, RTS_UTF8STRING * const pszClientVendorName, RTS_UTF8STRING * const pszClientVersion, unsigned short usClientSpecific)
		{
			return PLCCOM_OK;
		}
		virtual long Login(void) = 0;
		virtual long Logout(void) = 0;
		virtual long CheckConnectResult(void);

		virtual long LinkToProject(char *pszProject);
		virtual long CheckProjectIds(void) = 0;
		virtual long CheckConnection(void);
		virtual unsigned long GetSDBProjectID(void);
		virtual long GetPlcProjectName(char *pszProject, unsigned long ulLen) = 0;

		virtual long GetPlcStatus(PLC_STATUS *pPlcStatus);
		virtual long SetPlcStatus(PLC_STATUS PlcStatus);
		virtual long ResetPlc(RESET_OPTION ResetCommand);
		virtual long GetApplicationList(char ***pppszApplications, unsigned long *pulNumOfApplications);
		virtual long CheckAppFileConsistency(long *plResult, char *pszApplication, long *plBootProjectResult, long *plArchiveResult);
		virtual long GetApplicationStatus(PLC_STATUS *pAppStatus, char *pszApplication = NULL);
		virtual long SetApplicationStatus(PLC_STATUS AppStatus, char *pszApplication = NULL);
		virtual long ResetApplication(RESET_OPTION ResetCommand, char *pszApplication = NULL);
		virtual long GetResetOriginDeviceConfig(unsigned long *pulConfigOptions, unsigned long *pulRights, RTS_UTF8STRING ***pppszDescriptions, long *plResult);
		virtual long ResetOriginDevice(unsigned long ulConfiguration, unsigned long *pulResults, long *plResult);
		virtual long GetDeviceOpMode(DEVICE_OPERATION_MODE *pOpMode);
		virtual long SetDeviceOpMode(DEVICE_OPERATION_MODE OpMode, long *plResult);

		virtual long LoadSymbols(void) = 0;
		virtual long LoadSymbolsOffline(void);
		virtual long DeleteSymbols(void) = 0;

		virtual long GetSymbols(PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols) = 0;
		virtual long GetSymbolAddress(unsigned long ulSymbolIndex, char *pszMappedAddr, long lLen);
		virtual long GetSymbol(char *pszSymbol, PlcSymbolDesc *pSymbol);
		virtual long ExpandItem(char *pszSymbol, PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);
		virtual long ReleaseExpandedItems(PlcSymbolDesc *pSymbolList);

		virtual HVARLIST DefineVarList(char **pszSymbols, unsigned long ulNumOfSymbols, int bDataChange, unsigned long ulFlags, long *plResult) = 0;
		virtual long DeleteVarList(HVARLIST hVarList, int bDeleteLocalListOnly) = 0;
		virtual long AddVarsToVarList(HVARLIST hVarlist, char **pszSymbols, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);
		virtual long RemoveVarsFromVarList(HVARLIST hVarlist, unsigned long *pulIndexList, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);

		virtual long GetValueLengths(HVARLIST hVarList, unsigned long *pValueLength, unsigned long ulNumOfValues, unsigned long ulStartIndex = 0);
		virtual long SendVarList(HVARLIST hVarList, RTS_HANDLE hEvent, RTS_HANDLE hSem, int *pbSync = NULL, int bGetChangedValues = 0, unsigned long *pulVarFlags = NULL) = 0;
		virtual long GetVarList(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues) = 0;
		virtual long GetValues(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues) = 0;
		virtual long GetChangedValues(HVARLIST hVarList, unsigned long *pulVarFlags, PlcVarValue ***pppValues, unsigned long *pulNumOfValues, unsigned long **ppulChangedValuesIndexList, unsigned long *pulNumOfChangedValues);

		virtual long WriteVarList(HVARLIST hVarList, unsigned long ulNumOfSymbols, unsigned char **ppValues, unsigned long *pulValueSizes) = 0;
		virtual long WriteVars(char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned char **ppValues, unsigned long *pulValueSizes) = 0;

		virtual long SendService(unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize) = 0;

		virtual long SetMotorola(long bMotorola);
		
		virtual long SetTimeout(unsigned long ulTimeout);
		virtual unsigned long GetTimeout(void);
		virtual unsigned long GetRetries(void);
		virtual unsigned long GetChannel(void);

		virtual long SetDeviceCredentials(RTS_UTF8STRING * const pUserName, unsigned long userNameBufSize, RTS_UTF8STRING * const pPassword, unsigned long passwordBufSize);
		virtual long BackupDeviceCredentials(void);
		virtual long RestoreDeviceCredentialsIfBackedUp(void);

		virtual long UploadFile(char *pszPlc, char *pszHost, long *plResult, bool bUseStandardPath = true);
 		virtual long DownloadFile(char *pszHost, char *pszPlc, long *plResult, bool bUseStandardPath = true);
		virtual long FileRename(char *pszOldFile, char *pszNewFile, long *plResult);
		virtual long FileDelete(char *pszFile, long *plResult);
		virtual long CreateDirectory(char *pszDir, long *plResult);
		virtual long RenameDirectory(char *pszOldDir, char *pszNewDir, long *plResult);
		virtual long DeleteDirectory(char *pszDir, long *plResult);
		virtual long ReadDirectory(CDirInfo **ppdi, char *pszBaseDir, long *plResult);

		virtual long BootprojectReload(long *plResult);
		virtual long BootAppRegister(char *pszApplication, long *plResult);
		virtual long BootAppReload(char *pszApplication, long *plResult);
		virtual long RetainSave(char *pszRetainFile, long lBufferLen, char *pszApplication, long *plResult);
		virtual long RetainRestore(char *pszRetainFile, long lBufferLen, char *pszApplication, long *plResult);
		virtual long SendEcho(unsigned long *pulSendDataLen, unsigned long *pulReceiveDataLen, long *plResult);
		virtual long CheckTargetId(unsigned long ulTargetId, unsigned long ulHookId, unsigned long ulMagic, long *plResult);
		virtual long GetPrjInfo(ProjectInfo **ppPrjInfo);
		virtual long GetAppInfo(char *pszApplication, ProjectInfo **ppPrjInfo, ApplicationInfo2 **ppAppInfo);
		virtual long ApplicationsBackup(char *pszBackupFilePath, int bForceBackup, int bCreateTbf, long *plResult);
		virtual long ApplicationsRestore(char *pszRestoreFilePath, int bStartBootprojects, long *plResult);
#if 0 /* Firmware update is not released yet. Documentation to this APIs is attached to CDS-80228 */
		virtual long InstallFirmwareUpdate(char *pszFirmwarePackage, long *plResult);
		virtual long GetFirmwareUpdateResult(char *pszFirmwarePackage, char **ppSummary, char **ppErrors, char **ppWarnings, char **ppUserActions, long *plResult);
#endif
		virtual unsigned long GetSessionId(void); 
		virtual long GetDevInfo(DeviceInfo3 **ppDeviceInfo);
		virtual long RenamePlc(RTS_WCHAR *pwszNodeName);

	protected:
		void Init(RTS_HANDLE hLogger = NULL);
		long Swap(void *pData, long lSize);
		long Swap(void *pData, long lSize, long lCount);
		unsigned short Swap(unsigned short usData);
		RTS_UI32 Swap(RTS_UI32 ulData);
		short Swap(short sData);
		RTS_I32 Swap(RTS_I32 lData);
		unsigned long WriteData(unsigned char **ppbyDest, unsigned char *pbyData, unsigned long ulSize);
		unsigned long ReadData(unsigned char *pbyData, unsigned char **ppbySrc, unsigned long ulSize);
		unsigned long GetType(char *pszType, unsigned long ulSize);
		int IsValidProjectName(char *psz, unsigned long ulSize);
		void DeletePrjInfo(void);
		void DeleteAppInfo(void);
		virtual long ReadV2DevInfo(void);
		virtual void DeleteDevInfo(void);

		unsigned long		m_ulId;
		char				*m_pszProject;
		char				*m_pszSymbolFilePath;
		char				*m_pszDllDirectory;
		unsigned long		m_ulChannel;
		unsigned long		m_ulTimeout;
		unsigned long		m_ulTries;
		RTS_UI32			m_ulBufferSize;
		long				m_bMotorola;		/* ByteOrder of the PLC */
		int					m_bMotorolaHost;	/* ByteOrder of the PLCHandler's machine */
		long				m_bPreCheckIdentity;
		PLCCOM_STATUS		m_Status;
		unsigned long		m_ulLogFilter;
		RTS_HANDLE			m_hLogger;
		int					m_bLogging;
		DataTypeHashTable	*m_pHashTableVarTypes;
		unsigned long		m_ulSdbProjectId;
		unsigned long		m_ulSymbols;
		PlcSymbolDesc		*m_pSymbols;
		ProjectInfo			*m_pPrjInfo;
		ApplicationInfo2	*m_pAppInfo;
		char				*m_pszPlcName;
		DeviceInfo3			m_DeviceInfo3;
		char*				m_pszUser;
		char*				m_pszPassword;
		RTS_UTF8STRING*		m_pszUserBackup;
		RTS_UTF8STRING*		m_pszPasswordBackup;
};

// redefine #defines from winbase.h
#if defined (WIN32) && defined (UNICODE) && !defined(_WIN32_WCE) && !defined(OPC_SERVER_3S)
	#undef CreateDirectory
	#define CreateDirectory  CreateDirectoryW
#endif

#endif	//__PLCCOMBASE_H__

