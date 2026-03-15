#include "stdafx.h"
#include "CoDeSysWrapper.h"

// defined custom error managed by wrapper
#define RESULT_UNMAPPED_STATION							999
#define RESULT_STATION_ID_INVALID						998
#define RESULT_READVALUE_UNMAPPED_VAR					997

#define VAR_LIST_SEPARATOR								"#"

//#pragma region Class and Static vars
//
//
#pragma region CEasyPLCHandlerWrapper


ExchangeValuesHandle::ExchangeValuesHandle(void) {
	hList = NULL;
	ppVarValues = NULL;
	ulNumVarValues = 0;
}
ExchangeValuesHandle::~ExchangeValuesHandle() {
	//delete hList;
	hList = NULL;
	// reset only, do not delete it (call destructor) --> see CoDeSys documentation
	ppVarValues = NULL;
	ulNumVarValues = 0;
}


CEasyPLCHandlerWrapper::CEasyPLCHandlerWrapper(void) {
	nID = 0;
}
CEasyPLCHandlerWrapper::CEasyPLCHandlerWrapper(int nStationID) {
	
	nID = nStationID;	
	pHandler = new CEasyPLCHandler(RTS_INVALID_HANDLE);
	pSynRead = new ExchangeValuesHandle();
	pCyclingRead = new ExchangeValuesHandle();
	ppWriteValues = NULL;
	ppWriteSymbols = NULL;
	ulNumVriteValues = 0;
	pCallbackUpdateList = new CUpdateListCallback(this);
	bCallBackUpdated = false;
	nCallBackNotifyCounter = 0;
}

CEasyPLCHandlerWrapper::~CEasyPLCHandlerWrapper() {

	ReleaseObjectMemory();
	
}

void CEasyPLCHandlerWrapper::ReleaseObjectMemory(void) {

	if (pHandler != NULL) {

		pHandler->Disconnect();

		delete pCallbackUpdateList;
		pCallbackUpdateList = NULL;

		bCallBackUpdated = false;
		nCallBackNotifyCounter = 0;

		pHandler = NULL;
	}

	delete pSynRead;
	pSynRead = NULL;

	delete pCyclingRead;
	pCyclingRead = NULL;

	delete ppWriteValues;
	ppWriteValues = NULL;

	delete ppWriteSymbols;
	ppWriteSymbols = NULL;

}
#pragma endregion


#pragma region CPLCHandlerCallback
CUpdateListCallback::CUpdateListCallback(void) : CPLCHandlerCallback()
{
}

CUpdateListCallback::CUpdateListCallback(CEasyPLCHandlerWrapper* p)
{
	pStation = p;
}

CUpdateListCallback::~CUpdateListCallback()
{
}

long CUpdateListCallback::Notify(CPLCHandler *pPlcHandler, CallbackAddInfoTag CallbackAdditionalInfo)
{
	if (pPlcHandler == NULL)
		return RESULT_FAILED;

	pStation->bCallBackUpdated = true;

	pStation->nCallBackNotifyCounter++;

	return RESULT_OK;
}
#pragma endregion

// first time library init checker
bool m_CSWInitDone = false;
// map of all PLCHandler instance created from external client
CAtlMap <int, CEasyPLCHandlerWrapper *> m_MapStations;
// instance nr of last created station
int m_iLastStationID = 0;
// lock object used to access to m_MapStations onject
CMutex m_StationMutex;


//#pragma endregion

// Used from caller to check if library is present into the system
bool CSWIsWrapperInstalled(){
	return true;
}

bool CSWIsCoDeSysInstalled() {
	bool Present = false;
	
	CSingleLock StationLock(&m_StationMutex);
	StationLock.Lock();
	try {
		CPLCHandler *pPLCH = new CPLCHandler(RTS_INVALID_HANDLE);		
		pPLCH = NULL;
		Present = true;
	} catch (int i) {

	}
	StationLock.Unlock();

	return Present;
}

void CSWInit() {
	
	CSingleLock StationLock(&m_StationMutex);
	StationLock.Lock();
	if (!m_CSWInitDone) {
		m_CSWInitDone = true;
		m_iLastStationID = 0;
	}
	StationLock.Unlock();
}

long CSWCreateStation(int &nStationID) {
	
	CSingleLock StationLock(&m_StationMutex);
	StationLock.Lock();
		
	m_iLastStationID++;

	CEasyPLCHandlerWrapper *pStation = new CEasyPLCHandlerWrapper(m_iLastStationID);	
	
	m_MapStations.SetAt(pStation->nID, pStation);		

	nStationID = pStation->nID;
	StationLock.Unlock();

	return RESULT_OK;
}

int CSWNumActiveStations() {
	int Nr = 0;
	
	CSingleLock StationLock(&m_StationMutex);
	StationLock.Unlock();
	Nr = (int)m_MapStations.GetCount();
	StationLock.Unlock();

	return Nr;
}

CEasyPLCHandlerWrapper *GetStationByID(int nStationID) {
		
	CEasyPLCHandlerWrapper *pStation;
	bool bFound = false;

	CSingleLock StationLock(&m_StationMutex);
	StationLock.Lock();	
	bFound = m_MapStations.Lookup(nStationID, pStation);		
	StationLock.Unlock();

	if (bFound)		
		return pStation;
	else
		return NULL;
}


long CSWReleaseStation(int nStationID) {

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation != NULL) {

		CSingleLock StationLock(&m_StationMutex);
		StationLock.Lock();

		pStation->ReleaseObjectMemory();
		/*delete pStation;
		pStation = NULL;*/
				
		//delete[] * Station->ppGetItem;
		m_MapStations.RemoveKey(nStationID);

		StationLock.Unlock();
	}

	return RESULT_OK;
}


char** SplitSymbolsIntoArray(char *pszSymbols, /*[In]*/ unsigned long ulNumOfSymbols) {
		
	char** ppszsymbols = new char*[ulNumOfSymbols];
	
	unsigned long i = 0;
	char* chars_array = strtok(pszSymbols, VAR_LIST_SEPARATOR);
	while (chars_array != NULL) {
		ppszsymbols[i] = new char[strlen(chars_array) + 1];
		_tcscpy(ppszsymbols[i], chars_array);
		chars_array = strtok(NULL, VAR_LIST_SEPARATOR);
		i++;
	}
		
	return ppszsymbols;
}

void EmptySymbolsArray(char** ppszsymbols, unsigned long ulNumOfSymbols) {
		
	if (ppszsymbols != NULL) {

		for (unsigned long i = 0; i < ulNumOfSymbols; i++) {
			if (ppszsymbols[i] != NULL) {
				delete[] ppszsymbols[i];
				ppszsymbols[i] = NULL;
			}
		}
				
		delete ppszsymbols;
		ppszsymbols = NULL;
	}

}


long CSWGetLastError(int nStationID)
{
	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	return pStation->pHandler->GetLastError();
}

long CSWGetState(int nStationID)
{
	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return STATE_TERMINATE;

	if (pStation->pHandler == NULL)
		return STATE_TERMINATE;

	Result = pStation->pHandler->GetState();

	return Result;
}

long CSWConnectViaGateway3(int nStationID, char *pszGatewayIP, char *pszAddress, int bLoadSymbols, unsigned long ulTimeout) {
	
	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	Result = pStation->pHandler->ConnectViaGateway3(pszGatewayIP, pszAddress, bLoadSymbols, ulTimeout);

	return Result;
}

long CSWDisconnect(int nStationID) {
	
	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	Result = pStation->pHandler->Disconnect();

	return Result;
}

long CSWGetVarListFromPLC(int nStationID, PlcSymbolDesc** pSymbols, unsigned long &ulNumOfSymbols)
{
	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	Result = pStation->pHandler->GetAllItems(pSymbols, &ulNumOfSymbols);

	return Result;
}

long CSWGetItem(int nStationID,/*[In]*/ char *pszSymbol, /*[Out]*/ PlcSymbolDesc** pSymbol) {

	long Result = STATE_TERMINATE;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	PlcSymbolDesc sDesc;
	Result = pStation->pHandler->GetItem(pszSymbol, &sDesc);
	if (Result == RESULT_OK) {
		//delete[] * Station->ppGetItem;
		/*pStation->(*ppGetItem) = new PlcSymbolDesc[1];
		Station->(*ppGetItem)[0] = sDesc;*/
		pStation->ppGetItem[0] = sDesc;
		*pSymbol = &pStation->ppGetItem[0];
	}

	return Result;
}



void ReleaseExchangedVars(ExchangeValuesHandle *pEx) {
	
	// do not execute delete --> please refere to Codesys documentation
	//delete pEx->ppVarValues;
	pEx->ppVarValues = NULL;
	pEx->ulNumVarValues = 0;
}

long CSWSyncReadVarsFromPlc(int nStationID, char *pszSymbols, /*[In]*/ unsigned long ulNumOfSymbols) {

	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	char** ppszSymbols = SplitSymbolsIntoArray(pszSymbols, ulNumOfSymbols);
		
	ReleaseExchangedVars(pStation->pSynRead);
	
	pStation->pSynRead->hList = pStation->pHandler->SyncReadVarsFromPlc(ppszSymbols, ulNumOfSymbols, &pStation->pSynRead->ppVarValues, &pStation->pSynRead->ulNumVarValues);

	EmptySymbolsArray(ppszSymbols, ulNumOfSymbols);

	if (pStation->pSynRead->hList != NULL && pStation->pSynRead->ulNumVarValues > 0) {
		Result = RESULT_OK;
	} else {

		ReleaseExchangedVars(pStation->pSynRead);

		Result = RESULT_FAILED;
	}

	return Result;
}

long CSWSyncReadVarsFromPlcReleaseValues(int nStationID) {

	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;
	
	Result = pStation->pHandler->SyncReadVarsFromPlcReleaseValues(pStation->pSynRead->hList);

	return Result;
}

long GetReadedValue(ExchangeValuesHandle *pEx, unsigned long ulSymbolNr, unsigned int nSymbolSize, /*[Out]*/ byte *pResultValue, unsigned long &ulTimeStamp, unsigned char &bQuality) {
		
	long lResult = RESULT_READVALUE_UNMAPPED_VAR;

	if (*pEx->ppVarValues != NULL) {
		if (ulSymbolNr<0 || ulSymbolNr < pEx->ulNumVarValues) {
			memcpy(pResultValue, pEx->ppVarValues[ulSymbolNr]->byData, nSymbolSize);
			ulTimeStamp = pEx->ppVarValues[ulSymbolNr]->ulTimeStamp;
			bQuality = pEx->ppVarValues[ulSymbolNr]->bQuality;
			lResult = RESULT_OK;
		}
	}
	
	return lResult;
}

long CSWGetSyncReadedVarFromPlc(int nStationID, unsigned long ulSymbolNr, unsigned int nSymbolSize, /*[Out]*/ byte *pResultValue, unsigned long &ulTimeStamp, unsigned char &bQuality) {

	long lResult = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	lResult = GetReadedValue(pStation->pSynRead, ulSymbolNr, nSymbolSize, pResultValue, ulTimeStamp, bQuality);

	return lResult;
}

void CSWReleaseSyncReadedVars(int nStationID) {

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return;

	if (pStation->pHandler == NULL)
		return;

	ReleaseExchangedVars(pStation->pSynRead);
}

long CSWCycDefineVarList(int nStationID,/*[In]*/ char *pszSymbols, /*[In]*/ unsigned long ulNumOfSymbols, /*[In]*/ unsigned long ulUpdateRate) {
	
	long lResult = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	char** ppszSymbols = SplitSymbolsIntoArray(pszSymbols, ulNumOfSymbols);

	pStation->pCyclingRead->hList = pStation->pHandler->CycDefineVarList(ppszSymbols, ulNumOfSymbols, ulUpdateRate, pStation->pCallbackUpdateList);

	EmptySymbolsArray(ppszSymbols, ulNumOfSymbols);

	if (pStation->pCyclingRead->hList != NULL)
		lResult = RESULT_OK;
	else
		lResult = RESULT_FAILED;

	return lResult;
}

long CSWCycDeleteVarList(int nStationID,/*[In]*/ int bKeepalive) {

	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	if (pStation->pCyclingRead->hList == NULL)
		return RESULT_OK;

	Result = pStation->pHandler->CycDeleteVarList(pStation->pCyclingRead->hList, bKeepalive);
	pStation->pCyclingRead->hList = NULL;

	return Result;
}

long CSWCycEnterVarAccess(int nStationID) {

	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	if (pStation->pCyclingRead->hList == NULL)
		return RESULT_NO_OBJECT;

	if (pStation->pCyclingRead->hList != NULL)
		return RESULT_OK;

	Result = pStation->pHandler->CycEnterVarAccess(pStation->pCyclingRead->hList);

	return (Result == 1 ? RESULT_OK : RESULT_FAILED);
}

void CSWCycLeaveVarAccess(int nStationID) {

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return;

	if (pStation->pHandler == NULL)
		return;

	if (pStation->pCyclingRead != NULL) {
		pStation->pHandler->CycLeaveVarAccess(pStation->pCyclingRead->hList);

		ReleaseExchangedVars(pStation->pCyclingRead);
	}
}

long CSWGetCysReadCallBackInfo(int nStationID, bool &bCallBackUpdated, unsigned int &nCallBackNotifyCounter) {
	
	bCallBackUpdated = false;
	nCallBackNotifyCounter = 0;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pCallbackUpdateList == NULL)
		return RESULT_NO_OBJECT;

	bCallBackUpdated = pStation->bCallBackUpdated;
	nCallBackNotifyCounter = pStation->nCallBackNotifyCounter;

	return RESULT_OK;
}

long CSWResetCallBackUpdatedCycRead(int nStationID) {

	CPLCHandlerCallback *pUpdateReadyCallback = NULL;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pCallbackUpdateList == NULL)
		return RESULT_NO_OBJECT;
	
	pStation->bCallBackUpdated = false;
	
	return RESULT_OK;
}


long CSWCycReadVars(int nStationID) {

	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	if (pStation->pCyclingRead->hList == NULL)
		return RESULT_NO_OBJECT;

	Result = pStation->pHandler->CycReadVars(pStation->pCyclingRead->hList, &pStation->pCyclingRead->ppVarValues, &pStation->pCyclingRead->ulNumVarValues);

	return Result;
}

long CSWGetCycReadedVar(int nStationID, unsigned long ulSymbolNr, unsigned int nSymbolSize, /*[Out]*/ byte *pResultValue, unsigned long &ulTimeStamp, unsigned char &bQuality) {

	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	Result = GetReadedValue(pStation->pCyclingRead, ulSymbolNr, nSymbolSize, pResultValue, ulTimeStamp, bQuality);

	return Result;
}

void EmptyWriteValuesArray(CEasyPLCHandlerWrapper *pStation) {

	if (pStation->ppWriteSymbols != NULL && pStation->ppWriteValues != NULL) {
		for (unsigned long i = 0; i < pStation->ulNumVriteValues; i++) {
			
			if (pStation->ppWriteSymbols[i] != NULL) {
				delete[] pStation->ppWriteSymbols[i];
				pStation->ppWriteSymbols[i] = NULL;
			}

			if (pStation->ppWriteValues[i] != NULL) {
				delete[] pStation->ppWriteValues[i];
				pStation->ppWriteValues[i] = NULL;
			}

		}
		delete[] pStation->ppWriteSymbols;
		pStation->ppWriteSymbols = NULL;
		delete[] pStation->ppWriteValues;
		pStation->ppWriteValues = NULL;
	}

	pStation->ulNumVriteValues = 0;
}


long CSWSyncWriteInitValues(int nStationID,unsigned long ulNumVarValues) {
	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	EmptyWriteValuesArray(pStation);

	pStation->ppWriteSymbols = new char*[ulNumVarValues];
	pStation->ppWriteValues = new byte*[ulNumVarValues];
	pStation->ulNumVriteValues = ulNumVarValues;
	
	return RESULT_OK;
}

long CSWSyncWriteAddValue(int nStationID, unsigned long ulSymbolNr, char *pszSymbol, byte *pValue, unsigned int nSymbolSize) {

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	pStation->ppWriteSymbols[ulSymbolNr] = new char[strlen(pszSymbol) + 1];
	_tcscpy(pStation->ppWriteSymbols[ulSymbolNr], pszSymbol);

	pStation->ppWriteValues[ulSymbolNr] = new byte[nSymbolSize];
	memcpy(pStation->ppWriteValues[ulSymbolNr],pValue, nSymbolSize);

	return RESULT_OK;
}

long CSWSyncWriteVarsToPlc(int nStationID) {
	
	long Result = RESULT_FAILED;

	CPLCHandlerCallback *pUpdateReadyCallback = NULL;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	if (pStation->ulNumVriteValues ==0 )
		return RESULT_NO_OBJECT;

	Result = pStation->pHandler->SyncWriteVarsToPlc(pStation->ppWriteSymbols, pStation->ulNumVriteValues, pStation->ppWriteValues);

	return Result;
}

long CSWConnect(int nStationID, unsigned long nTypeConnection, char* GatewayAddress, char* PLCAddress, unsigned long port, char* username, char* passwordPLC, char* GatewayPassword)
{

	PlcConfig*	plcConfig;
	PlcDeviceDesc*	pDevDesc;
	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
	{
		return (long)RESULT_UNMAPPED_STATION;
	}
	
	if (pStation->pHandler == NULL)
	{
		return (long)RESULT_NO_OBJECT;
	}

	plcConfig = new PlcConfig();
	pDevDesc = new PlcDeviceDesc();

	//Set objects default values
	if (DefineStartupsInStruct(plcConfig, pDevDesc, nTypeConnection, GatewayAddress, PLCAddress, port, username, passwordPLC, GatewayPassword) == 0)
	{
		ClearPlcDeviceParams(plcConfig, pDevDesc);
		return RESULT_NO_OBJECT;
	}

	//Set configuration
	if (pStation->pHandler->SetConfig(plcConfig, pDevDesc) == RESULT_FAILED)
	{
		ClearPlcDeviceParams(plcConfig, pDevDesc);
		return (long)RESULT_NO_OBJECT;
	}

	ClearPlcDeviceParams(NULL, pDevDesc);

	Result = pStation->pHandler->Connect(PLCHANDLER_USE_DEFAULT, NULL, 1);
	return(Result);
}

void ClearPlcDeviceParams(PlcConfig* pConfig, PlcDeviceDesc* pDevice)
{
	for (unsigned long i = 0; i < pDevice->ulNumParams; i++)
	{
		delete pDevice->ppd[i].pParameter;
	}
	delete[] pDevice->ppd;
	pDevice->ppd = NULL;
	delete pDevice;
	pDevice = NULL;

	if (pConfig != NULL)
	{	
		if (pConfig->gwc != NULL)
		{
			//delete pConfig->gwc->pszDeviceName;
			if (pConfig->gwc->pszPassword != NULL)
			{
				delete pConfig->gwc->pszPassword;
			}
			//delete pConfig->gwc;
		}
		delete pConfig;
		pConfig = NULL;
	}
}

char DefineStartupsInStruct(PlcConfig* pConfig, PlcDeviceDesc* pDevice, unsigned long nTypeConnection, char* GatewayAddress, char* PLCAddress, unsigned long port, char* username, char* passwordPLC, char* GatewayPassword)
{
	if (pConfig == NULL || pDevice == NULL)
		return 0;

	pConfig->ulId = 0;
	if (nTypeConnection == GATEWAY)
	{
		pConfig->it = IT_GATEWAY3;
	}
	else//Connection direct
	{
		pConfig->it = IT_ARTI3;
	}

	pConfig->pszName = new char[strlen("PLC 0") + 1];
	strcpy(pConfig->pszName, "PLC 0");
	pConfig->bActive = 1;
	pConfig->ulLogFilter = 0x000000FF;
	pConfig->bLogToFile = 0;
	pConfig->bMotorola = 0;
	pConfig->bLogin = 1;
	pConfig->bPreCheckIdentity = 0;
	pConfig->ulTimeout = 5000;
	pConfig->ulNumTries = 3;
	pConfig->ulWaitTime = 10;
	pConfig->ulReconnectTime = PLCHANDLER_TIMEOUT_INFINITE;
	pConfig->pszHwType = NULL;
	pConfig->ulHwVersion = 0;
	pConfig->ulBufferSize = 0;	// take device default size
	pConfig->pszProjectName = NULL;//new char[strlen("PlcHandlerTest.pro")+1]; 	// symbol file name "OPCTest.sdb" in simulation mode
	pConfig->pszDllDirectory = NULL;

	//Settings for the Gateway connection
	if (nTypeConnection == GATEWAY)
	{
		pConfig->gwc = new GatewayConnection();
		pConfig->gwc->pszDeviceName = new char[strlen("Tcp/Ip") + 1];
		strcpy(pConfig->gwc->pszDeviceName, "Tcp/Ip");

		pConfig->gwc->pszAddress = new char[strlen(GatewayAddress) + 1];
		strcpy(pConfig->gwc->pszAddress, GatewayAddress);
		pConfig->gwc->ulPort = 1217;
		if ((GatewayPassword == NULL) || (GatewayPassword[0] == 0x00))
		{
			pConfig->gwc->pszPassword = NULL;
		}
		else
		{
			pConfig->gwc->pszPassword = new char[strlen(GatewayPassword) + 1];
			strcpy(pConfig->gwc->pszPassword, GatewayPassword);
		}
	}
	else
	{
		pConfig->gwc = NULL;   /* No Gateway used */
	}//End Settings for the Gateway connection

	pDevice->pszName = NULL;
	pDevice->pszInstance = NULL;
	pDevice->pszProject = NULL;

	//Settings for the PLC connection
	if (((passwordPLC == NULL) || (username == NULL)) ||
		(passwordPLC[0] == 0x00) || (username[0] == 0x00))
	{
		if (nTypeConnection == GATEWAY)
		{
			pDevice->ulNumParams = 1;
			pDevice->ppd = new PlcParameterDesc[1];

			/* V2: TCP/IP Address or V3 logical address or PLC name as string*/
			pDevice->ppd[0].ulId = 0;
			pDevice->ppd[0].pszName = const_cast<char*>(PLCC_PN_ADDRESS);
			pDevice->ppd[0].pParameter = new PlcParameter;
			pDevice->ppd[0].pParameter->Type = PLC_PT_STRING;
			pDevice->ppd[0].pParameter->Value.psz = PLCAddress;

		}
		else //Connection Direct no Password
		{
			pDevice->ulNumParams = 2;
			pDevice->ppd = new PlcParameterDesc[2];

			pDevice->ppd[0].ulId = 0;
			pDevice->ppd[0].pszName = const_cast<char*>(PLCC_PN_IP_ADDRESS);
			pDevice->ppd[0].pParameter = new PlcParameter;
			pDevice->ppd[0].pParameter->Type = PLC_PT_STRING;
			pDevice->ppd[0].pParameter->Value.psz = PLCAddress;

			pDevice->ppd[1].ulId = 1;
			pDevice->ppd[1].pszName = const_cast<char*>(PLCC_PN_PORT);
			pDevice->ppd[1].pParameter = new PlcParameter;
			pDevice->ppd[1].pParameter->Type = PLC_PT_ULONG;
			pDevice->ppd[1].pParameter->Value.dw = (unsigned long)port;

		}
	}
	else //PLC with the username and password
	{
		pDevice->ulNumParams = 4;
		pDevice->ppd = new PlcParameterDesc[4];

		pDevice->ppd[0].ulId = 0;
		pDevice->ppd[0].pszName = const_cast<char*>(PLCC_PN_IP_ADDRESS);
		pDevice->ppd[0].pParameter = new PlcParameter;
		pDevice->ppd[0].pParameter->Type = PLC_PT_STRING;
		pDevice->ppd[0].pParameter->Value.psz = PLCAddress;

		pDevice->ppd[1].ulId = 1;
		pDevice->ppd[1].pszName = const_cast<char*>(PLCC_PN_PORT);
		pDevice->ppd[1].pParameter = new PlcParameter;
		pDevice->ppd[1].pParameter->Type = PLC_PT_ULONG;
		pDevice->ppd[1].pParameter->Value.dw = (unsigned long)port;

		pDevice->ppd[2].ulId = 2;
		pDevice->ppd[2].pszName = const_cast<char*>(PLCC_PN_USER);
		pDevice->ppd[2].pParameter = new PlcParameter;
		pDevice->ppd[2].pParameter->Type = PLC_PT_STRING;
		pDevice->ppd[2].pParameter->Value.psz = username;

		pDevice->ppd[3].ulId = 3;
		pDevice->ppd[3].pszName = const_cast<char*>(PLCC_PN_PASSWORD);
		pDevice->ppd[3].pParameter = new PlcParameter;
		pDevice->ppd[3].pParameter->Type = PLC_PT_STRING;
		pDevice->ppd[3].pParameter->Value.psz = passwordPLC;

	}//End Settings for the PLC connection
	return 1;
}



//int PHTestBufferRead(byte **buffer, int ulNumOfBytes) {
//
//	byte SourceBuffer[] = { 0,1,2,3,99 };
//	memcpy(*buffer, SourceBuffer, ulNumOfBytes);
//
//	return 0;
//}