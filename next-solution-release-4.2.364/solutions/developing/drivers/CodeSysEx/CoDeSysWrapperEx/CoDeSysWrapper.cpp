#include "stdafx.h"
#include "CoDeSysWrapper.h"

#if defined(_CONSOLE) && defined(WIN32) && !defined(_WIN32_WCE) && !defined(_WIN32_WCE_EMULATION) && defined(_DEBUG) && (_MSC_VER >= 1400)
	#include <crtdbg.h>
#endif

/*
--------------------------------------------------------------------------------
IMPORTANT NOTES for using the PLCHandler as dynamic loaded library (.dll or .so)
--------------------------------------------------------------------------------

I. (only needed for Windows OS)
   Make sure to define the preprocessor identifier PLCH_USE_DLL in your project.


II. To have access to the internal system adaptation layer (i. e. CAL_SysTimeGetMs()) or the
	BinTagUtil you have to consider the following points:

	1. Use this code fragment to declare and import the used function pointers.
		extern "C"
		{
			USE_STMT
		}

		static int CDECL ImportSystemFunctions(void)
		//	Get function pointers of the system components
		{
			// Macro to import functions
			IMPORT_STMT;
			return ERR_OK;
		}

	2. Call this code directly after instanciating the (Easy)PLCHandler. Starting from this
	   moment the functions are ready to be called.
		pPLCHandler = new CEasyPLCHandler(RTS_INVALID_HANDLE);
		s_pfCMGetAPI = CMGetAPI;
		s_pfCMGetAPI2 = CMGetAPI2;
		if(ImportSystemFunctions() != ERR_OK)
		{
			// Exit the Application here. If import didn't work this would lead to crash in CAL_... macros.
			return 1;
		}

	3. Use the CAL_ Makro in front of each call of this functions.
		Example:
		ulStart = CAL_SysTimeGetMs();

	The same code can optionally be used in combination with the static link librarys.
*/

extern "C"
{
	USE_STMT
}

static int CDECL ImportSystemFunctions(void)
/*	Get function pointers of the system components */
{
	/* Macro to import functions */
	IMPORT_STMT;
	return ERR_OK;
}

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
	pHandler = NULL;
	pSynRead = new ExchangeValuesHandle();
	pCyclingRead = new ExchangeValuesHandle();
	pPlcConfig = new PlcConfig();
	pDevDesc = new PlcDeviceDesc();
	ppWriteValues = NULL;
	ppWriteSymbols = NULL;
	ulNumVriteValues = 0;
	pCallbackUpdateList = new CUpdateListCallback(this);
	bCallBackUpdated = false;
	pCallbackStateChanged = new CUpdateListCallback(this);
	nCallBackNotifyCounter = 0;
	ConnectionErrorOccured = false;
	nCycVarAccessKeepAlive = 1;
}

void ClearPlcConfigParams(PlcConfig* pParams);
void ClearDevDescParams(PlcDeviceDesc* pParams);

void CEasyPLCHandlerWrapper::DeletePLCHandler(void)
{
	if (pHandler != NULL) {

		pHandler->Disconnect();
		
		if (pCyclingRead != NULL)
		{
			if (pCyclingRead->hList != NULL)
			{
				//pHandler->CycDeleteVarList(pCyclingRead->hList, nCycVarAccessKeepAlive);
				pCyclingRead->hList = NULL;
			}
			delete pCyclingRead;
			pCyclingRead = NULL;
		}

		if (pCallbackUpdateList != NULL)
		{
			delete pCallbackUpdateList;
			pCallbackUpdateList = NULL;
		}

		if (pCallbackStateChanged != NULL)
		{
			delete pCallbackStateChanged;
			pCallbackStateChanged = NULL;
		}		
		
		ClearPlcConfigParams(pPlcConfig);

		delete pHandler;

		bCallBackUpdated = false;
		nCallBackNotifyCounter = 0;
		ConnectionErrorOccured = false;
		pHandler = NULL;
	}
}

CEasyPLCHandlerWrapper::~CEasyPLCHandlerWrapper()
{
	ReleaseObjectMemory();	
}

void CEasyPLCHandlerWrapper::ReleaseObjectMemory(void)
{
	DeletePLCHandler();
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

	if (!pStation->ConnectionErrorOccured)
	{
		long state = pPlcHandler->GetState();

		pStation->ConnectionErrorOccured = (state == STATE_TERMINATE || state == STATE_PLC_NOT_CONNECTED || state == STATE_NO_SYMBOLS || state == STATE_DISCONNECT || state == STATE_NO_CONFIGURATION || state == STATE_PLC_NOT_CONNECTED_SYMBOLS_LOADED);
	}

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
	} catch (CException* e)
	{
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

long CSWCreateStation(int *nStationID) {
	
	CSingleLock StationLock(&m_StationMutex);
	StationLock.Lock();

	if (*nStationID == 0)
	{
		m_iLastStationID++;
		*nStationID = m_iLastStationID;
	}

	CEasyPLCHandlerWrapper *pStation = new CEasyPLCHandlerWrapper(*nStationID);
	
	m_MapStations.SetAt(pStation->nID, pStation);		
	
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

bool CSWIsPlcHandlerInitialized(int nStationID)
{
	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return FALSE;

	return (pStation->pHandler != NULL);
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

char** SplitSymbolsArray(char *pszSymbols, /*[In]*/ unsigned long ulNumOfSymbols) {
		
	char** ppszsymbols = new char*[ulNumOfSymbols];
	
	unsigned long i = 0;
	char* chars_array = strtok(pszSymbols, VAR_LIST_SEPARATOR);
	while (chars_array != NULL) {
		ppszsymbols[i] = new char[strlen(chars_array) + 1];
		//_tcscpy(ppszsymbols[i], chars_array);
		strcpy(ppszsymbols[i], chars_array);
		chars_array = strtok(NULL, VAR_LIST_SEPARATOR);
		i++;
	}
		
	return ppszsymbols;
}

void EmptySymbolsArray(char** ppszsymbols, unsigned long ulNumOfSymbols)
{	
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

long CSWDisconnect(int nStationID) {
	
	long Result = RESULT_OK;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler != NULL)
	{
		try
		{
			if (pStation->pCyclingRead != NULL)
			{
				if (pStation->pCyclingRead->hList != NULL)
				{					
					//pStation->pHandler->CycDeleteVarList(pStation->pCyclingRead->hList, pStation->nCycVarAccessKeepAlive);
					pStation->pCyclingRead->hList = NULL;
				}
				delete pStation->pCyclingRead;
				pStation->pCyclingRead = NULL;

				pStation->pCyclingRead = new ExchangeValuesHandle();

				Result = pStation->pHandler->Disconnect();
			}
		}
		catch (CException* e)
		{
			Result = RESULT_COMM_FATAL;
		}

		if (Result == RESULT_COMM_FATAL)
		{			
			ClearPlcConfigParams(pStation->pPlcConfig);
			ClearDevDescParams(pStation->pDevDesc);
			::CEasyPLCHandlerWrapper(nStationID);			
		}
	}

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

	Result = pStation->pHandler->EnterItemAccess();
	if (Result == RESULT_OK)
	{
		Result = pStation->pHandler->GetAllItems(pSymbols, &ulNumOfSymbols);
		pStation->pHandler->LeaveItemAccess();
	}

	return Result;
}

long CSWGetItem(int nStationID,/*[In]*/ char* pszSymbol, /*[Out]*/ PlcSymbolDesc** pSymbol) {

	long Result = STATE_TERMINATE;

	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_UNMAPPED_STATION; // RESULT_NO_OBJECT;
	
	Result = pStation->pHandler->EnterItemAccess();
	if (Result == RESULT_OK)
	{		
		Result = pStation->pHandler->GetItem(pszSymbol, &pStation->ppGetItem[0]);
		if (Result == RESULT_OK)
			*pSymbol = &pStation->ppGetItem[0];
		pStation->pHandler->LeaveItemAccess();
	}

	return Result;
}



void ReleaseExchangedVars(ExchangeValuesHandle *pEx) {
	
	// do not execute delete --> please refere to Codesys documentation
	//delete pEx->ppVarValues;
	pEx->ppVarValues = NULL;
	pEx->ulNumVarValues = 0;
}

long CSWSyncReadVarsFromPlc(int nStationID, char* pszSymbols, /*[In]*/ unsigned long ulNumOfSymbols) {

	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	char** ppszSymbols = SplitSymbolsArray(pszSymbols, ulNumOfSymbols);

	//ReleaseExchangedVars(pStation->pSynRead);

	pStation->pSynRead->hList = pStation->pHandler->SyncReadVarsFromPlc(ppszSymbols, ulNumOfSymbols, &pStation->pSynRead->ppVarValues, &pStation->pSynRead->ulNumVarValues);

	EmptySymbolsArray(ppszSymbols, ulNumOfSymbols);

	if (pStation->pSynRead->hList != NULL && pStation->pSynRead->ulNumVarValues > 0)
		Result = RESULT_OK;
	else
		Result = RESULT_FAILED;

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

long CSWSyncReadVarsRelease(int nStationID) {

	long lResult = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	if (pStation->pSynRead->hList == NULL)
		return RESULT_NO_OBJECT;
	
	lResult = pStation->pHandler->SyncReadVarsFromPlcReleaseValues(pStation->pSynRead->hList);

	pStation->pSynRead->hList = NULL;	
	
	ReleaseExchangedVars(pStation->pSynRead);

	return lResult;
}

long CSWCycDefineVarList(int nStationID,/*[In]*/ char *pszSymbols, /*[In]*/ unsigned long ulNumOfSymbols, /*[In]*/ unsigned long ulUpdateRate) {
	
	long lResult = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	char** ppszSymbols = SplitSymbolsArray(pszSymbols, ulNumOfSymbols);

	if (pStation->pCyclingRead->hList != NULL)
		pStation->pCyclingRead->hList = NULL;

	pStation->pCyclingRead->hList = pStation->pHandler->CycDefineVarList(ppszSymbols, ulNumOfSymbols, ulUpdateRate, pStation->pCallbackUpdateList, NULL,NULL, VARLIST_FLAG_KEEP_VARLIST_ON_DISCONNECT);

	EmptySymbolsArray(ppszSymbols, ulNumOfSymbols);

	if (pStation->pCyclingRead->hList != NULL)
		lResult = RESULT_OK;
	else
		lResult = RESULT_FAILED;

	return lResult;
}

long CSWCycDeleteVarList(int nStationID) {

	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	if (pStation->pCyclingRead->hList == NULL)
		return RESULT_OK;

	/*if (pStation->pHandler->CycIsValidList(pStation->pCyclingRead->hList))
	{*/		
		//Result = pStation->pHandler->CycDeleteVarList(pStation->pCyclingRead->hList, pStation->nCycVarAccessKeepAlive);
	/*}*/

	pStation->pCyclingRead->hList = NULL;

	//Result = pStation->pHandler->CycEnterVarAccess(pStation->pCyclingRead->hList);
	//if (Result != RESULT_FAILED)
	//{
	//	pStation->pHandler->CycLeaveVarAccess(pStation->pCyclingRead->hList);
	//	Result = pStation->pHandler->CycDeleteVarList(pStation->pCyclingRead->hList, pStation->nCycVarAccessKeepAlive);
	//	pStation->pCyclingRead->hList = NULL;
	//}
	//else
	//{
	//	pStation->pCyclingRead->hList = NULL;
	//}

	return Result;
}

//long CSWCycEnterVarAccess(int nStationID) {
//
//	long Result = RESULT_FAILED;
//
//	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
//	if (pStation == NULL)
//		return RESULT_UNMAPPED_STATION;
//
//	if (pStation->pHandler == NULL)
//		return RESULT_NO_OBJECT;
//
//	if (pStation->pCyclingRead->hList == NULL)
//		return RESULT_NO_OBJECT;
//
//	if (pStation->pCyclingRead->hList != NULL)
//		return RESULT_OK;
//
//	Result = pStation->pHandler->CycEnterVarAccess(pStation->pCyclingRead->hList);
//
//	return (Result == 1 ? RESULT_OK : RESULT_FAILED);
//}
//
//void CSWCycLeaveVarAccess(int nStationID) {
//
//	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
//	if (pStation == NULL)
//		return;
//
//	if (pStation->pHandler == NULL)
//		return;
//
//	if (pStation->pCyclingRead != NULL) {
//		pStation->pHandler->CycLeaveVarAccess(pStation->pCyclingRead->hList);
//
//		ReleaseExchangedVars(pStation->pCyclingRead);
//	}
//}

long CSWGetCysReadCallBackInfo(int nStationID, bool &bCallBackUpdated, unsigned int &nCallBackNotifyCounter, int& nState) {
	
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

bool CSWConnectionErrorOccured(int nStationID) {

	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return FALSE;

	if (pStation->pCallbackStateChanged == NULL)
		return FALSE;

	if (pStation->ConnectionErrorOccured)
	{	
		pStation->ConnectionErrorOccured = FALSE;
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

long CSWCycReadVars(int nStationID) {

	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	if (pStation->pHandler == NULL)
		return RESULT_NO_OBJECT;

	if (pStation->pCyclingRead == NULL || pStation->pCyclingRead->hList == NULL)
		return RESULT_NO_OBJECT;

	Result = pStation->pHandler->CycEnterVarAccess(pStation->pCyclingRead->hList);
	if (Result != RESULT_FAILED)
	{
		Result = pStation->pHandler->CycReadVars(pStation->pCyclingRead->hList, &pStation->pCyclingRead->ppVarValues, &pStation->pCyclingRead->ulNumVarValues);
		pStation->pHandler->CycLeaveVarAccess(pStation->pCyclingRead->hList);
	}

	return Result;
}

void CSWCycReadVarsRelease(int nStationID) {

	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return;

	if (pStation->pHandler == NULL)
		return;

	ReleaseExchangedVars(pStation->pCyclingRead);
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

long CSWSyncWriteInitValues(int nStationID,unsigned long ulNumVarValues)
{
	CEasyPLCHandlerWrapper *pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	CSWSyncWriteDeleteValues(nStationID);

	pStation->ppWriteSymbols = new char*[ulNumVarValues];
	pStation->ppWriteValues = new byte*[ulNumVarValues];
	pStation->ulNumVriteValues = ulNumVarValues;
	
	return RESULT_OK;
}

long CSWSyncWriteDeleteValues(int nStationID)
{
	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;
	
	if (pStation->ppWriteSymbols != NULL)
	{
		for (unsigned long i = 0; i < pStation->ulNumVriteValues; i++)
		{
			if (pStation->ppWriteSymbols[i] != NULL) {
				delete[] pStation->ppWriteSymbols[i];
				pStation->ppWriteSymbols[i] = NULL;
			}
		}
		delete[] pStation->ppWriteSymbols;
		pStation->ppWriteSymbols = NULL;		
	}

	if (pStation->ppWriteValues != NULL)
	{
		for (unsigned long i = 0; i < pStation->ulNumVriteValues; i++)
		{
			if (pStation->ppWriteValues[i] != NULL) {
				delete[] pStation->ppWriteValues[i];
				pStation->ppWriteValues[i] = NULL;
			}
		}
		delete[] pStation->ppWriteValues;
		pStation->ppWriteValues = NULL;
	}

	pStation->ulNumVriteValues = 0;

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

long CSWConnect(int nStationID, unsigned long nTypeConnection, char* GatewayAddress, char* PLCAddress, unsigned long port, char* username, char* passwordPLC, char* GatewayPassword,unsigned long nTimeOut, unsigned long ulNumTries, char* logFile)
{
	long Result = RESULT_FAILED;

	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
	{
		return (long)RESULT_UNMAPPED_STATION;
	}
	
	if (pStation->pHandler == NULL)
	{
		pStation->pHandler = new CEasyPLCHandler(RTS_INVALID_HANDLE);

		s_pfCMGetAPI = CMGetAPI;
		s_pfCMGetAPI2 = CMGetAPI2;
		if (ImportSystemFunctions() != ERR_OK)
		{
			delete pStation->pHandler;
			pStation->pHandler = NULL;
			//printf("Failed to import system functions. Terminating...");
			return (long)RESULT_NOT_SUPPORTED;
		}

		//Set objects default values
		DefineStartupsInStruct(pStation->pPlcConfig, pStation->pDevDesc, nTypeConnection, GatewayAddress, PLCAddress, port, username, passwordPLC, GatewayPassword, nTimeOut, ulNumTries);

		//Set configuration
		pStation->pHandler->SetConfig(pStation->pPlcConfig, pStation->pDevDesc);

		ClearDevDescParams(pStation->pDevDesc);
	}

	if (logFile != NULL && strlen(logFile) != 0)
		CSWEnableLogToFile(nStationID, logFile);

	//Result = pStation->pHandler->Connect(PLCHANDLER_USE_DEFAULT, pStation->pCallbackStateChanged, 1);
	Result = pStation->pHandler->Connect(PLCHANDLER_USE_DEFAULT, NULL, 1);
		
	return(Result);
}

void ClearPlcConfigParams(PlcConfig *pParams)
{
	if (pParams->gwc != NULL)
	{
		if (pParams->gwc->pszDeviceName != NULL)
		{
			delete[] pParams->gwc->pszDeviceName;
			pParams->gwc->pszDeviceName = NULL;
		}
		if (pParams->gwc->pszAddress != NULL)
		{
			delete[]pParams->gwc->pszAddress;
			pParams->gwc->pszAddress = NULL;
		}
		if (pParams->gwc->pszPassword != NULL)
		{
			delete[]pParams->gwc->pszPassword;
			pParams->gwc->pszPassword = NULL;
		}
		delete pParams->gwc;
		pParams->gwc = NULL;		
		pParams = NULL;
	}	
}

void ClearDevDescParams(PlcDeviceDesc* pParams)
{
	if (pParams != NULL)
	{
		for (unsigned long i = 0; i < pParams->ulNumParams; i++)
			delete pParams->ppd[i].pParameter;

		delete[] pParams->ppd;
		pParams->ppd = NULL;
		pParams = NULL;
	}
}

char DefineStartupsInStruct(PlcConfig* pConfig, PlcDeviceDesc* pDevice, unsigned long nTypeConnection, char* GatewayAddress, char* PLCAddress, unsigned long port, char* username, char* passwordPLC, char* GatewayPassword, unsigned long nTimeOut, unsigned long ulNumTries)
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
	pConfig->ulTimeout = nTimeOut;
	pConfig->ulNumTries = (ulNumTries == 0 ? 1 : ulNumTries);
	pConfig->ulWaitTime = 0;
	//pConfig->ulReconnectTime = PLCHANDLER_TIMEOUT_INFINITE;
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

long CSWDeletePLCHandler(int nStationID) {

	long Result = RESULT_OK;

	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	pStation->DeletePLCHandler();

	return Result;
}

long CSWSetCycVarAccessKeepAlive(int nStationID,int bKeepAlive) {

	long Result = RESULT_OK;

	CEasyPLCHandlerWrapper* pStation = GetStationByID(nStationID);
	if (pStation == NULL)
		return RESULT_UNMAPPED_STATION;

	pStation->nCycVarAccessKeepAlive = bKeepAlive;

	return Result;
}

long CSWEnableLogToFile(int nStationID, char* logFile)
{
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

	//pStation->pHandler->SetLogging(TRUE, LOG_STD);
	pStation->pHandler->SetLogging(TRUE, 0xFFFFFFFF);
	pStation->pHandler->SetLogFileCapacity(1000000, 10);
	Result = pStation->pHandler->SetLogFile(logFile);

	return(Result);
}


//
//char DefineStartupsInStructMovicon(int nStationID, char* StationName, PlcConfig* pConfig, PlcDeviceDesc* pDevice, unsigned long nTypeConnection, char* GatewayAddress, char* PLCAddress, unsigned long port, unsigned long timeout, char* username, char* passwordPLC, char* GatewayPassword)
//{
//	if (pConfig == NULL || pDevice == NULL)
//		return 0;
//
//	pConfig->ulId = 0;
//	if (nTypeConnection == GATEWAY)
//	{
//		pConfig->it = IT_GATEWAY3;
//	}
//	else//Connection direct
//	{
//		pConfig->it = IT_ARTI3;
//	}
//
//	pConfig->pszName = new char[strlen("PLC 0") + 1];
//	strcpy(pConfig->pszName, "PLC 0");
//	pConfig->ulId = nStationID;
//	pConfig->bActive = 1;
//	pConfig->ulLogFilter = 0x000000FF;
//	pConfig->bLogToFile = 0;
//	pConfig->bMotorola = 0;
//	pConfig->bLogin = 1;
//	pConfig->bPreCheckIdentity = 0;
//	//pConfig->ulTimeout = PLCHANDLER_TIMEOUT_INFINITE; // timeout;
//	//pConfig->ulTimeout = PLCHANDLER_USE_DEFAULT;
//	pConfig->ulNumTries = 1;
//	pConfig->ulWaitTime = 0;
//	pConfig->ulTimeout = timeout;
//	pConfig->ulReconnectTime = PLCHANDLER_TIMEOUT_INFINITE; // the internal reconnect thread is disabled at all and the Connect( ) method does one connect try and returns with the result.
//	//pConfig->ulReconnectTime = PLCHANDLER_TIMEOUT_INFINITE; // the internal reconnect thread is disabled at all and the Connect( ) method does one connect try and returns with the result.
//	pConfig->pszHwType = NULL;
//	pConfig->ulHwVersion = 0;
//	pConfig->ulBufferSize = 0;	// take device default size
//	pConfig->pszProjectName = NULL;//new char[strlen("PlcHandlerTest.pro")+1]; 	// symbol file name "OPCTest.sdb" in simulation mode
//	pConfig->pszDllDirectory = NULL;
//
//	//Settings for the Gateway connection
//	if (nTypeConnection == GATEWAY)
//	{
//		pConfig->gwc = new GatewayConnection();
//		pConfig->gwc->pszDeviceName = new char[strlen("Tcp/Ip") + 1];
//		strcpy(pConfig->gwc->pszDeviceName, "Tcp/Ip");
//
//		pConfig->gwc->pszAddress = new char[strlen(GatewayAddress) + 1];
//		strcpy(pConfig->gwc->pszAddress, GatewayAddress);
//		pConfig->gwc->ulPort = 1217;
//		if ((GatewayPassword == NULL) || (GatewayPassword[0] == 0x00))
//		{
//			pConfig->gwc->pszPassword = NULL;
//		}
//		else
//		{
//			pConfig->gwc->pszPassword = new char[strlen(GatewayPassword) + 1];
//			strcpy(pConfig->gwc->pszPassword, GatewayPassword);
//		}
//	}
//	else
//	{
//		pConfig->gwc = NULL;   /* No Gateway used */
//	}//End Settings for the Gateway connection
//
//	pDevice->pszName = NULL;
//	pDevice->pszInstance = NULL;
//	pDevice->pszProject = NULL;
//
//	//Settings for the PLC connection
//	if (((passwordPLC == NULL) || (username == NULL)) ||
//		(passwordPLC[0] == 0x00) || (username[0] == 0x00))
//	{
//		if (nTypeConnection == GATEWAY)
//		{
//			pDevice->ulNumParams = 1;
//			pDevice->ppd = new PlcParameterDesc[1];
//
//			/* V2: TCP/IP Address or V3 logical address or PLC name as string*/
//			pDevice->ppd[0].ulId = 0;
//			pDevice->ppd[0].pszName = const_cast<char*>(PLCC_PN_ADDRESS);
//			pDevice->ppd[0].pParameter = new PlcParameter;
//			pDevice->ppd[0].pParameter->Type = PLC_PT_STRING;
//			//pDevice->ppd[0].pParameter->Value.psz = PLCAddress;
//			pDevice->ppd[0].pParameter->Value.psz = new char[strlen(PLCAddress) + 1];
//			strcpy(pDevice->ppd[0].pParameter->Value.psz, PLCAddress);
//
//		}
//		else //Connection Direct no Password
//		{
//			//pDevice->ulNumParams = 2;
//			//pDevice->ppd = new PlcParameterDesc[2];
//			pDevice->ulNumParams = 1;
//			pDevice->ppd = new PlcParameterDesc[1];
//
//			pDevice->ppd[0].ulId = 0;
//			pDevice->ppd[0].pszName = const_cast<char*>(PLCC_PN_IP_ADDRESS);
//			pDevice->ppd[0].pParameter = new PlcParameter;
//			pDevice->ppd[0].pParameter->Type = PLC_PT_STRING;
//			//pDevice->ppd[0].pParameter->Value.psz = PLCAddress;
//			pDevice->ppd[0].pParameter->Value.psz = new char[strlen(PLCAddress) + 1];
//			strcpy(pDevice->ppd[0].pParameter->Value.psz, PLCAddress);
//
//			/*pDevice->ppd[1].ulId = 1;
//			pDevice->ppd[1].pszName = const_cast<char*>(PLCC_PN_PORT);
//			pDevice->ppd[1].pParameter = new PlcParameter;
//			pDevice->ppd[1].pParameter->Type = PLC_PT_ULONG;
//			pDevice->ppd[1].pParameter->Value.dw = (unsigned long)port;*/
//
//		}
//	}
//	else //PLC with the username and password
//	{
//		pDevice->ulNumParams = 4;
//		pDevice->ppd = new PlcParameterDesc[4];
//
//		pDevice->ppd[0].ulId = 0;
//		pDevice->ppd[0].pszName = const_cast<char*>(PLCC_PN_IP_ADDRESS);
//		pDevice->ppd[0].pParameter = new PlcParameter;
//		pDevice->ppd[0].pParameter->Type = PLC_PT_STRING;
//		pDevice->ppd[0].pParameter->Value.psz = PLCAddress;
//
//		pDevice->ppd[1].ulId = 1;
//		pDevice->ppd[1].pszName = const_cast<char*>(PLCC_PN_PORT);
//		pDevice->ppd[1].pParameter = new PlcParameter;
//		pDevice->ppd[1].pParameter->Type = PLC_PT_ULONG;
//		pDevice->ppd[1].pParameter->Value.dw = (unsigned long)port;
//
//		pDevice->ppd[2].ulId = 2;
//		pDevice->ppd[2].pszName = const_cast<char*>(PLCC_PN_USER);
//		pDevice->ppd[2].pParameter = new PlcParameter;
//		pDevice->ppd[2].pParameter->Type = PLC_PT_STRING;
//		pDevice->ppd[2].pParameter->Value.psz = username;
//
//		pDevice->ppd[3].ulId = 3;
//		pDevice->ppd[3].pszName = const_cast<char*>(PLCC_PN_PASSWORD);
//		pDevice->ppd[3].pParameter = new PlcParameter;
//		pDevice->ppd[3].pParameter->Type = PLC_PT_STRING;
//		pDevice->ppd[3].pParameter->Value.psz = passwordPLC;
//
//	}//End Settings for the PLC connection
//	return 1;
//}
//
//long TestAll() {
//
//	long Result = RESULT_OK;
//
//	
//	int iNumOfLoops = 1; // NUM_OF_LOOPS;
//	
//	int i;
//	for (i = 0; i < iNumOfLoops; i++)
//	{
//		printf("Test nr ... %u\n", i);
//		CEasyPLCHandler* pPLCHandler = new CEasyPLCHandler(RTS_INVALID_HANDLE);
//
//		PlcConfig* plcConfig = new PlcConfig();
//		PlcDeviceDesc* devDesc = new PlcDeviceDesc();
//
//		/*if (DefineStartupsInStruct(&plcConfig, &devDesc) == 0)
//			break;*/
//		int nStationID = 0;
//		int nTypeConnection = (int)DIRECT;
//		char* GatewayAddress = NULL;
//		char* StationName = NULL;
//		char* PLCAddress;
//		unsigned long port = 11740;
//		unsigned long timeout = 5000;
//		char* username = NULL;
//		char* passwordPLC = NULL;
//		char* GatewayPassword = NULL;
//
//
//		PLCAddress = new char[strlen("192.168.1.99") + 1];
//		strcpy(PLCAddress, "192.168.1.99");
//
//		StationName = new char[strlen("Pippo") + 1];
//		strcpy(StationName, "Pippo");
//
//		username = new char[strlen("admin") + 1];
//		strcpy(username, "admin");
//		passwordPLC = new char[strlen("wago") + 1];
//		strcpy(passwordPLC, "wago");
//
//		//							int nStationID, PlcConfig* pConfig, PlcDeviceDesc* pDevice, unsigned long nTypeConnection, char* GatewayAddress, char* PLCAddress, unsigned long port, unsigned long timeout, char* username, char* passwordPLC, char* GatewayPassword)
//		if (DefineStartupsInStructMovicon(nStationID, StationName, plcConfig, devDesc, nTypeConnection, GatewayAddress, PLCAddress, port, timeout, username, passwordPLC, GatewayPassword) == 0)
//			break;
//
//		//char* ip = new char[strlen("192.168.1.210") + 1];
//		//strcpy(ip, "192.168.1.210");
//		//pPLCHandler->ConnectTcpipViaArti3(ip, 11740, 0, PLCHANDLER_USE_DEFAULT, NULL);
//
//		//pPLCHandler->GetConfig(&plcConfig, &devDesc);
//
//		//pPLCHandler->Disconnect();
//
//		//printf("#### PLCHandler test finished. ####\n");
//		//delete pPLCHandler;
//
//
//		if (pPLCHandler->SetConfig(plcConfig, devDesc) == RESULT_FAILED)
//		{
//			printf("\n*** Configuration Error! Check configuration structs! ***\n\n");
//			break;
//		}
//		ClearPlcDeviceParams(devDesc);
//		if (pPLCHandler->Connect(PLCHANDLER_USE_DEFAULT, NULL, 1) == RESULT_OK)
//		{
//			/*GetProjectInfoTest();
//			GetDeviceInfoTest();*/
//
//			/*PlcConfig* plcConfig = NULL;
//			PlcDeviceDesc* devDesc = NULL;
//			pPLCHandler->GetConfig(&plcConfig, &devDesc);*/
//
//			printf("PLC connected successful\n");
//			/* #### Test - Area ############################### */
//
//			// RenamePlcTest();
//			// FileAccessTest(); 
//			// DirectoryTest(); 
//			// BackupRestoreTest();
//			// PlcStateTest(); 
//			// RetainTest();
//			// for (int z=0; z<3; z++)
//			//	SendEchoTest();
//			// GetProjectInfoTest(); 
//			// GetDeviceInfoTest();
//
//			/*SyncReadWriteTest();
//			SyncListReadWriteTest();
//			CyclicReadTest();*/
//
//			/* ONLY FOR TESTING IOCONFIG3
//			unsigned long ulFlags = 0;
//			IoConfig3ReadDiagFlags(40100, 0, &ulFlags);
//			printf("ReadDiagFlags: ModuleType=40100, Instance=0: Value=0x%08x\n", ulFlags);
//
//			unsigned char byValue[10];
//			IoConfig3ReadDiagParameter(40100, 0, 393220, 0, 4, byValue);
//			// *(unsigned long*)byValue = 2222;
//			IoConfig3WriteDiagParameter(40100, 0, 393220, 0, 4, byValue);
//			*/
//
//			/* ################################################ */
//			printf("PLC online test end\n");
//		}
//		else
//		{
//			/*GetProjectInfoTest();
//			GetDeviceInfoTest();*/
//
//			/*PlcConfig* plcConfig = NULL;
//			PlcDeviceDesc* devDesc = NULL;
//			pPLCHandler->GetConfig(&plcConfig, &devDesc);*/
//
//			if (pPLCHandler->GetState() == STATE_NO_SYMBOLS)
//				printf("**** No symbols ****\n");
//			else if (pPLCHandler->GetState() == STATE_PLC_NOT_CONNECTED)
//				printf("**** No connection to plc ****\n");
//		}
//		pPLCHandler->Disconnect();
//
//		printf("#### PLCHandler test finished. ####\n");
//		delete pPLCHandler;
//	}
//
//	return Result;
//}


//int PHTestBufferRead(byte **buffer, int ulNumOfBytes) {
//
//	byte SourceBuffer[] = { 0,1,2,3,99 };
//	memcpy(*buffer, SourceBuffer, ulNumOfBytes);
//
//	return 0;
//}