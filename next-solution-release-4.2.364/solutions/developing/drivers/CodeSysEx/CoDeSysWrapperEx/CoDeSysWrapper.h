#pragma once
//#include "stdafx.h"

enum
{
	GATEWAY,
	DIRECT
};


//tell to compiler that specific class (CEasyPLCHandlerWrapper) exist but will be declared later
class CEasyPLCHandlerWrapper;

class CUpdateListCallback : public CPLCHandlerCallback
{
public:
	CUpdateListCallback(void);
	CUpdateListCallback(CEasyPLCHandlerWrapper *p);
	virtual ~CUpdateListCallback();

	CEasyPLCHandlerWrapper* pStation;

	virtual long Notify(CPLCHandler *pPlcHandler, CallbackAddInfoTag CallbackAdditionalInfo);
};

class ExchangeValuesHandle {
public:
	ExchangeValuesHandle(void);
	~ExchangeValuesHandle();

public:
	// handle of requested values
	HCYCLIST hList;
	// pointer to requested values 
	PlcVarValue **ppVarValues;
	// num of requeded values 
	unsigned long ulNumVarValues;
	PlcSymbolDesc ppGetItem[1]; // pointer used in GetItem
};


class CEasyPLCHandlerWrapper
{
public:
	CEasyPLCHandlerWrapper(void);
	CEasyPLCHandlerWrapper(int nStationID);
	void ReleaseObjectMemory(void);
	void DeletePLCHandler(void);
	~CEasyPLCHandlerWrapper();

public:
	// unique id of session (released to client on create)
	int nID;
	// object used to manage communication with CODESYS device
	CEasyPLCHandler* pHandler;

	PlcConfig *pPlcConfig;
	PlcDeviceDesc *pDevDesc;

	ExchangeValuesHandle *pSynRead;

	ExchangeValuesHandle *pCyclingRead;

	PlcSymbolDesc ppGetItem[1]; // pointer used in GetItem

	char **ppWriteSymbols;
	byte **ppWriteValues;
	unsigned long ulNumVriteValues;

	CUpdateListCallback *pCallbackUpdateList;
	bool bCallBackUpdated;
	unsigned int nCallBackNotifyCounter;
	bool ConnectionErrorOccured;
	int nCycVarAccessKeepAlive;

	CPLCHandlerCallback *pCallbackStateChanged;
};


#ifdef __cplusplus
extern "C" {
#endif
	char DefineStartupsInStruct(PlcConfig* pConfig, PlcDeviceDesc* pDevice, unsigned long nTypeConnection, char* GatewayAddress, char* PLCAddress, unsigned long port, char* username, char* passwordPLC, char* GatewayPassword, unsigned long nTimeOut, unsigned long ulNumTries);

	void ClearPlcDeviceParams(PlcDeviceDesc* pDevice);

	extern __declspec(dllexport) bool CSWIsWrapperInstalled();
	
	extern __declspec(dllexport) bool CSWIsCoDeSysInstalled();

	extern __declspec(dllexport) bool CSWIsPlcHandlerInitialized(int nStationID);
	
	extern __declspec(dllexport) void CSWInit();

	extern __declspec(dllexport) long CSWCreateStation(int *nStationID);

	extern __declspec(dllexport) int CSWNumActiveStations();

	extern __declspec(dllexport) long CSWReleaseStation(int nStationID);
			
	extern __declspec(dllexport) long CSWGetState(int nStationID);

	extern __declspec(dllexport) long CSWGetLastError(int nStationID);

	extern __declspec(dllexport) long CSWConnectViaGateway3(int nStationID, char *pszGatewayIP, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout);

	extern __declspec(dllexport) long CSWDisconnect(int nStationID);

	extern __declspec(dllexport) long CSWGetVarListFromPLC(int nStationID, PlcSymbolDesc** pSymbols, unsigned long &ulNumOfSymbols);

	extern __declspec(dllexport) long CSWGetItem(int nStationID, char *pszSymbol, PlcSymbolDesc** pSymbols);

	extern __declspec(dllexport) long CSWSetCycVarAccessKeepAlive(int nStationID, int bKeepAlive);

	extern __declspec(dllexport) long CSWCycDeleteVarList(int nStationID);

	extern __declspec(dllexport) long CSWSyncReadVarsFromPlc(int nStationID, char *pszSymbols, unsigned long ulNumOfSymbols);
		
	extern __declspec(dllexport) long CSWGetSyncReadedVarFromPlc(int nStationID, unsigned long ulSymbolNr, unsigned int nSymbolSize, /*[Out]*/ byte *pResultValue, unsigned long &ulTimeStamp, unsigned char &bQuality);

	extern __declspec(dllexport) long CSWResetCallBackUpdatedCycRead(int nStationID);

	extern __declspec(dllexport) long CSWSyncReadVarsRelease(int nStationID);

	/*extern __declspec(dllexport) void CSWCycLeaveVarAccess(int nStationID);

	extern __declspec(dllexport) long CSWCycEnterVarAccess(int nStationID);*/

	extern __declspec(dllexport) long CSWGetCysReadCallBackInfo(int nStationID, bool &bCallBackUpdated, unsigned int &nCallBackNotifyCounter, int &nState);

	extern __declspec(dllexport) bool CSWConnectionErrorOccured(int nStationID);

	extern __declspec(dllexport) long CSWCycDefineVarList(int nStationID,/*[In]*/ char *pszSymbols, /*[In]*/ unsigned long ulNumOfSymbols,/*[In]*/ unsigned long ulUpdateRate);

	extern __declspec(dllexport) long CSWCycReadVars(int nStationID);

	extern __declspec(dllexport) void CSWCycReadVarsRelease(int nStationID);

	extern __declspec(dllexport) long CSWGetCycReadedVar(int nStationID, unsigned long ulSymbolNr, unsigned int nSymbolSize, /*[Out]*/ byte *pResultValue, unsigned long &ulTimeStamp, unsigned char &bQuality);

	extern __declspec(dllexport) long CSWSyncWriteInitValues(int nStationID, unsigned long ulNumVarValues);

	extern __declspec(dllexport) long CSWSyncWriteDeleteValues(int nStationID);

	extern __declspec(dllexport) long CSWSyncWriteAddValue(int nStationID, unsigned long ulSymbolNr, char *pszSymbol, byte *pValue, unsigned int nSymbolSize);

	extern __declspec(dllexport) long CSWSyncWriteVarsToPlc(int nStationID);	

	extern __declspec(dllexport) long CSWConnect(int nStationID, unsigned long nTypeConnection, char* GatewayAddress, char* PLCAddress , unsigned long port, char* username , char* passwordPLC, char* GatewayPassword, unsigned long nTimeOut, unsigned long ulNumTries, char* logFile);

	extern __declspec(dllexport) long CSWDeletePLCHandler(int nStationID);
	
	extern __declspec(dllexport) long CSWEnableLogToFile(int nStationID, char* logFile);

	//extern __declspec(dllexport) long TestAll();

	//extern __declspec(dllexport) int PHTestBufferRead(byte **buffer, int ulNumOfBytes);

#ifdef __cplusplus
}
#endif

