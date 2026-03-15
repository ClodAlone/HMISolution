#ifndef __PLCCOMBASE3_H__
#define __PLCCOMBASE3_H__

#include "CmpPLCHandlerDep.h"
#include "PLCConfig.h"
#include "PLCComBase.h"
#include "XmlSymbolParser.h"
#include "CmpIecVarAccessItf.h"
#include "CmpBinTagUtilItf.h"
#include "CmpGwClientItf.h"
#include "CmpGwClient.h"

// Timeout to resolve node name
#ifndef RESOLVE_NODENAME_TIMEOUT
	#define RESOLVE_NODENAME_TIMEOUT			5000
#endif
#define MAX_RESOLVE_NODENAME_TIMEOUT			12000
#define RESOLVE_NODENAME_SUPERVISOR_TIMEOUT		(MAX_RESOLVE_NODENAME_TIMEOUT + 8000)

// States to detect PLC address
#define DA_STATE_INACTIVE			0
#define DA_STATE_WAIT				1
#define DA_STATE_DONE				2

typedef struct
{
	RTS_HANDLE				hVarList;
	unsigned long			ulStartIndex;
	unsigned long			ulNumOfVars;
	unsigned long			ulNumOfAllVars;
	PlcVarValue**			ppPlcVarValue;
	VarAccessInfo*			pVars;
	void*					pMasterList;
	void*					pNextSlaveList;
	unsigned long			ulFlags;
	unsigned long*			pulChangedVarIndexList;
	unsigned long			ulNumOfChangedVars;
	int						bCompleteUpdateRequired;
}COM3VARLIST;

typedef struct
{
	unsigned long ulTypeID;
	RTS_UI32 ulSize;
	unsigned char bySwapSize;
	RTS_HANDLE hVar;
} VarAccessInfoMoni;

typedef struct
{
	unsigned long			ulNumOfAllVars;
	PlcVarValue**			ppPlcVarValue;
	VarAccessInfoMoni*		paVarsMoni;
	unsigned long			ulFlags;
	unsigned long*			pulExpressionSize;
	unsigned char**			ppExpression;
	int						iAccess;
}COM3MONIVARLIST;

typedef struct
{
	unsigned long			ulTypeNode;
	TypeDescAsUnion*		ptd3;
}COM3TYPENODE;

typedef struct
{
	unsigned long			ulInterface;
	RTS_GUID				guid;
}COM3INTERFACE;

// Only used for ClientAddressResolution
typedef struct
{
	unsigned long ulNode;
	unsigned long ulOffset;
}CAR;

// Only used for RecallAddressInformation
typedef struct
{
	RTS_UI8* pCompleteVarDesc;
	RTS_UI32 ulVarDescLen;
}RAI;

typedef struct
{
	TypeClass3 typeClass;
	AccessRights accessRights;
	unsigned long ulSize;
	unsigned long ulInterface;
	unsigned long ulTypeNode;
	char *pszName;
	union 
	{
		CAR Car;
		RAI Rai;
	};
	unsigned long ulFlags;
} COM3BrowseLeafNode;

class CSymbolList
{
	public:
		CSymbolList(unsigned long ulAllocItems, CPLCComBase *m_pPLCComBase3);
		virtual ~CSymbolList(void);
		virtual long AddItem(COM3BrowseLeafNode *pLeaf);
		virtual PlcSymbolDesc *GetItemList(void);
		virtual unsigned long GetItems(void);

	protected:
		CPLCComBase *m_pPLCComBase3;
		unsigned long m_ulAllocItems;
		unsigned long m_ulMaxItems;
		unsigned long m_ulItems;
		PlcSymbolDesc *m_pItemList;
};

class CPLCComBase3 : public CPLCComBase
{
	public:
		// Construction with specific PLC and Device configuration 
		CPLCComBase3(PlcConfig * const pConfig, PlcDeviceDesc * const pDevice, const PFVERIFYCERTCALLBACK pfVerifyCallback, void * const pVerifyUserData, const PFREQUESTCREDENTIALSCALLBACK pfRequestCredentialsCallback, void * const pRequestCredentialsUserData, const RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

		virtual ~CPLCComBase3(void);

		virtual long Open(void);
		virtual long Close(void);
		virtual long CreateSession(RTS_UTF8STRING * const pszClientName, RTS_UTF8STRING * const pszClientVendorName, RTS_UTF8STRING * const pszClientVersion, unsigned short usClientSpecific);
		virtual long Login(void);
		virtual long Logout(void);
		virtual long CheckConnectResult(void);

		virtual long CheckConnection(void);
		virtual long GetPlcProjectName(char *pszProject, unsigned long ulLen);

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

		virtual long GetSymbol(char *pszSymbol, PlcSymbolDesc *pSymbol);
		virtual long ExpandItem(char *pszSymbol, PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);
		virtual long ReleaseExpandedItems(PlcSymbolDesc *pSymbolList);

		virtual HVARLIST DefineVarList(char **pszSymbols, unsigned long ulNumOfSymbols, int bDataChange, unsigned long ulFlags, long *plResult);
		virtual long DeleteVarList(HVARLIST hVarList, int bDeleteLocalListOnly);
		virtual long AddVarsToVarList(HVARLIST hVarlist, char **pszSymbols, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);
		virtual long RemoveVarsFromVarList(HVARLIST hVarlist, unsigned long *pulIndexList, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);

		virtual long GetValueLengths(HVARLIST hVarList, unsigned long *pValueLength, unsigned long ulNumOfValues, unsigned long ulStartIndex = 0);
		virtual long SendVarList(HVARLIST hVarList, RTS_HANDLE hEvent, RTS_HANDLE hSem, int *pbSync = NULL, int bGetChangedValues = 0, unsigned long *pulVarFlags = NULL);
		virtual long GetVarList(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);
		virtual long GetValues(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);
		virtual long GetChangedValues(HVARLIST hVarList, unsigned long *pulVarFlags, PlcVarValue ***pppValues, unsigned long *pulNumOfValues, unsigned long **ppulChangedValuesIndexList, unsigned long *pulNumOfChangedValues);

		virtual long WriteVarList(HVARLIST hVarList, unsigned long ulNumOfSymbols, unsigned char **ppValues, unsigned long *pulValueSizes);
		virtual long WriteVars(char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned char** ppValues, unsigned long *pulValueSizes);

		virtual long SendService(unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize);

		virtual unsigned long GetSessionId(void);
		virtual long RenamePlc(RTS_WCHAR *pwszNodeName);

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
		virtual long DeviceManagementInstallFirmwareUpdate(char *pszPackageOnPLC, long *plResult);
		virtual long DeviceManagementGetLastInstallResult(char *pszPackageOnPLC, char **ppSummary, char **ppErrors, char **ppWarnings, char **ppUserActions, long *plResult);
#endif

	protected:
		virtual long SendService3(PROTOCOL_DATA_UNIT pduSend, PROTOCOL_DATA_UNIT *ppduRecv, int bUseExternalRxBuffer = 0) = 0;
		virtual long LoadSymbols(void);
		virtual long DeleteSymbols(void);
		virtual long GetSymbols(PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);
		virtual long GetSymbolAddress(unsigned long ulSymbolIndex, char *pszMappedAddr, long lLen);
		virtual long CheckProjectIds(void);

		virtual long MONILoadSymbols(void);
		virtual long MONIDeleteSymbols(void);
		virtual long MONIGetSymbols(PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);
		virtual long MONIGetSymbolAddress(unsigned long ulSymbolIndex, char *pszMappedAddr, long lLen);
		virtual long MONICheckProjectIds(void);
		virtual long MONIExpandItem(char *pszSymbol, PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);
		virtual long MONIReleaseExpandedItems(PlcSymbolDesc *pSymbolList);

		virtual HVARLIST MONIDefineVarList(char **pszSymbols, unsigned long ulNumOfSymbols, int bDataChange, unsigned long ulFlags, long *plResult);
		virtual long MONIDeleteVarList(HVARLIST hVarList, int bDeleteLocalListOnly);
		virtual long MONIAddVarsToVarList(HVARLIST hVarlist, char **pszSymbols, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);
		virtual long MONIRemoveVarsFromVarList(HVARLIST hVarlist, unsigned long *pulIndexList, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);

		virtual long MONIGetValueLengths(HVARLIST hVarList, unsigned long *pValueLength, unsigned long ulNumOfValues, unsigned long ulStartIndex);
		virtual long MONISendVarList(HVARLIST hVarList, RTS_HANDLE hEvent, RTS_HANDLE hSem, int *pbSync = NULL, int bGetChangedValues = 0, unsigned long *pulVarFlags = NULL);
		virtual long MONIGetValues(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);
		virtual long MONIGetChangedValues(HVARLIST hVarList, unsigned long *pulVarFlags, PlcVarValue ***pppValues, unsigned long *pulNumOfValues, unsigned long **ppulChangedValuesIndexList, unsigned long *pulNumOfChangedValues);

		virtual long MONIWriteVarList(HVARLIST hVarList, unsigned long ulNumOfSymbols, unsigned char **ppValues, unsigned long *pulValueSizes);
		virtual long MONIWriteVars(char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char** ppValues, unsigned long *pulValueSizes);

		RTS_HANDLE				m_hGateway;
		RTS_HANDLE				m_hChannel;
		char*					m_pszTcpIpAddress;
		unsigned short			m_usTcpPort;
		PEERADDRESS				m_PlcAddress;
		PROTOCOL_DATA_UNIT		m_SendPdu;
		PROTOCOL_DATA_UNIT		m_ReceivePdu;
		RTS_UI32				m_ulSessionID;
		RTS_UI32				m_ulAuthType;
		int						m_nLeafNodes;
		int						m_nMaxLeafNodes;
		COM3BrowseLeafNode**	m_ppLeafNodes;
		int						m_nTypeNodes;
		int						m_nMaxTypeNodes;
		COM3TYPENODE*			m_pTypeNodes;
		RTS_WCHAR*				m_pwszNodeName;
		int						m_DetectAddressState;
		int						m_iNumNodesAnswered;
		unsigned long			m_ulResolveNodenameTimeout;
		unsigned long			m_ulResolveNodenameStarttime;
		int						m_bIpAddressIsResolved;
		int						m_bClientAddressResolution;
		COM3INTERFACE*			m_pInterfaces;
		int						m_nInterfaces;
		unsigned long			m_ulConfigChangedTimestamp;
		char**					m_ppApplications;
		unsigned long			m_ulNumOfApplications;
		int						m_bDontExpandSimpleTypeArrays;
		int						m_bDontExpandComplexTypeArrays;
		int						m_bDontLoadSymbolsFromPlc;
		CSymbolList**			m_ppExpandedSymbolLists;
		int						m_nExpandedSymbolLists;
		int						m_nMaxExpandedSymbolLists;
		CStringAllocator*		m_pTypeStringAllocater;
		char*					m_pszSimulationDeviceName;
		int						m_bRecallAddressInformation;
		char*					m_pszSymbolAppName;
		int						m_bLoadSymbolfileFromPlc;
		char*					m_pszCompleteSymbolFileName;
		CXmlSymbolParser*		m_pXmlParser;
		char**					m_ppszMappedAddr;
		unsigned long			m_ulSymbolAppSessionId;
		RTS_GUID				m_SymbolAppDataGUID;
		int						m_bEncryptCommunication;
		int						m_bEncryptedCommunicationPossible;
		char*					m_pszCipherList;
		RTS_HANDLE				m_hTlsContext;
		PFVERIFYCERTCALLBACK	m_pfVerifyCallback;
		void*					m_pVerifyUserData;
		PFREQUESTCREDENTIALSCALLBACK m_pfRequestCredentialsCallback;
		void*					m_pRequestCredentialsUserData;
		int						m_iAddressType;
		unsigned long			m_ulNumOfResetOriginDescriptions;
		RTS_UTF8STRING**        m_ppszResetOriginDescriptions;
		RTS_BOOL				m_bConnectToSelf;

		friend class CSymbolList;

	private:
		unsigned long getVersion(char *psz);
		int dirWalker(char *pszDir, int iProc, void *param1, void *param2, long *plResult);
		int createFilePath(char *pszPath);
		int processPath(char *pszPath, int iProc, void *param1, void *param2, long *plResult);
		RTS_SSIZE getLine(char *pszSrcBuf, RTS_SSIZE lPos, RTS_SSIZE lSrcSize, char *pszLine, RTS_SSIZE lLineSize);
		long RetainSaveInternal(char *pszRetainFile, long lBufferLen, char *pszApplication, unsigned short nBackupState, long *plResult);
		long RetainRestoreInternal(char *pszRetainFile, long lBufferLen, char *pszApplication, unsigned long ulAppSessionId, long *plResult);
		long GetTypeDesc(unsigned long ulInterface, unsigned long ulTypeNode, TypeDescAsUnion **ppTypedesc);
		int MapType(unsigned long ulInterface, unsigned long ulTypeNode, TypeClass3 typeClass, unsigned long ulSize, char **ppszType, unsigned long *pulTypeId);
		PlcSymbolDesc* FindSymbol(char *pszSymbol, unsigned long *pulPosition);
		int FindInsertPosition(char *pszLeafName);
		COM3BrowseLeafNode* FindNode(char *pszSymbol, char **ppszEnd);
		long AddNode(COM3BrowseLeafNode *pLeafNode);
		long ApplicationLogin(char *pszApplication, unsigned long *pulAppSessionId, RTS_GUID *pCodeGuid = NULL, RTS_GUID *pDataGuid = NULL);
		long ApplicationLogout(unsigned long ulAppSessionId);
		long CheckForces(char *pszApplication);
		long GetPrjInfo3(char *pszApplication, ProjectInfo **ppPrjInfo);
		long DefineVarListBase(HVARLIST *phVarlist, char **pszSymbols, unsigned long ulNumOfSymbols, int bDataChange, PlcVarValue ***pppValues, unsigned long ulFlags);
		long ConvertSymbol(char *pszSymbol, COM3BrowseLeafNode *pLeaf, TypeDescAsUnion *pTypedesc, PlcSymbolDesc *pSymbol);
		long CheckArray(char *pszSymbol, TypeDescArrayAsStruct *pArray, char **ppszSymbol);
		long CheckStruct(char *pszSymbol, TypeDescStructAsStruct *pStruct, char **ppszSymbol, unsigned long *pulTypeNode);
		long MONIFindLocalSymbolfile(void);
		long MONIDefineVarListInternal(COM3MONIVARLIST *pList, char **pszSymbols, unsigned long ulStartIndex, unsigned long ulNumOfSymbols);
		long MONIWriteVarsInternal(HVARLIST hVarList, char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char** ppValues, unsigned long *pulValueSizes);
		static RTS_RESULT CDECL TlsVerifyCallback(RTS_HANDLE hCert, RTS_RESULT verifyResult, void* pUserParameter);
		static void CDECL EncryptedCommunicationTask(struct tagSYS_TASK_PARAM *ptp);
		long SendServiceInternal(PROTOCOL_DATA_UNIT pduSend, PROTOCOL_DATA_UNIT *ppduRecv, int bUseExternalRxBuffer = 0);
		long MONISwapBytes(void * const pvData, const unsigned long ulIECTypeID, const unsigned long ulCompleteSize_bytes, const unsigned char byElementSwapSize_bytes);

		/* Asynchronous handling of SendService3 */
		RTS_HANDLE				m_hSendServiceTask;
		RTS_HANDLE				m_hEventSendData;
		RTS_HANDLE				m_hEventDataReceived;
		PROTOCOL_DATA_UNIT		m_pduRequest;
		PROTOCOL_DATA_UNIT*		m_ppduResponse;
		int						m_bUseExternalRxBuffer;
		long					m_SendService3Result;
};


class CPLCComBase3Selector : public CPLCComBase3
{
public:
	// Construction with specific PLC and Device configuration 
	CPLCComBase3Selector(PlcConfig * const pConfig, PlcDeviceDesc * const pDevice, const PFVERIFYCERTCALLBACK pfVerifyCallback, void * const pVerifyUserData, const PFREQUESTCREDENTIALSCALLBACK pfRequestCredentialsCallback, void * const pRequestCredentialsUserData, const RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

	virtual ~CPLCComBase3Selector(void);

	virtual long LoadSymbols(void);
	virtual long DeleteSymbols(void);
	virtual long GetSymbols(PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);
	virtual long CheckProjectIds(void);
	virtual long GetSymbolAddress(unsigned long ulSymbolIndex, char *pszMappedAddr, long lLen);
	virtual HVARLIST DefineVarList(char **pszSymbols, unsigned long ulNumOfSymbols, int bDataChange, unsigned long ulFlags, long *plResult);
	virtual long AddVarsToVarList(HVARLIST hVarlist, char **pszSymbols, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);
	virtual long RemoveVarsFromVarList(HVARLIST hVarlist, unsigned long *pulIndexList, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);
	virtual long DeleteVarList(HVARLIST hVarList, int bDeleteLocalListOnly);
	virtual long GetValueLengths(HVARLIST hVarList, unsigned long *pValueLength, unsigned long ulNumOfValues, unsigned long ulStartIndex);
	virtual long SendVarList(HVARLIST hVarList, RTS_HANDLE hEvent, RTS_HANDLE hSem, int *pbSync = NULL, int bGetChangedValues = 0, unsigned long *pulVarFlags = NULL);
	virtual long GetValues(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);
	virtual long GetChangedValues(HVARLIST hVarList, unsigned long *pulVarFlags, PlcVarValue ***pppValues, unsigned long *pulNumOfValues, unsigned long **ppulChangedValuesIndexList, unsigned long *pulNumOfChangedValues);
	virtual long ExpandItem(char *pszSymbol, PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);
	virtual long ReleaseExpandedItems(PlcSymbolDesc *pSymbolList);
	virtual long WriteVarList(HVARLIST hVarList, unsigned long ulNumOfSymbols, unsigned char **ppValues, unsigned long *pulValueSizes);
	virtual long WriteVars(char **pszSymbols, unsigned long ulNumOfSymbols, unsigned char** ppValues, unsigned long *pulValueSizes);

private:
	int			m_bSendMonitoringServices;
};


#endif	//__PLCCOMBASE3_H__

