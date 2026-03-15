#ifndef __PLCCOMSIM_H__
#define __PLCCOMSIM_H__

#include "CmpPLCHandlerDep.h"
#include "ARTIClient.h"
#include "ARTISymbol.h"
#include "PLCConfig.h"
#include "PLCComBase.h"

class CPLCComSim;

struct SymbolList
{
	unsigned char		bMotorola;
	unsigned long		ulProjectId;
	unsigned long		ulAllEntries;
	PlcSymbolDesc		*pAllSymbols;
};


class CPLCComSim : public CPLCComBase
{
	public:
		// Construction with specific PLC configuration 
		CPLCComSim(PlcConfig *pConfig, PlcDeviceDesc *pDevice, RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

		virtual ~CPLCComSim(void);

		virtual long Open(void);
		virtual long Close(void);

		virtual long Login(void);
		virtual long Logout(void);

		virtual long CheckProjectIds(void);
		virtual long GetPlcProjectName(char *pszProject, unsigned long ulLen);
		virtual long GetPlcStatus(PLC_STATUS *pPlcStatus = NULL);
		virtual long SetPlcStatus(PLC_STATUS PlcStatus);
		virtual long ResetPlc(RESET_OPTION ResetCommand);
		
		virtual long LoadSymbols(void);
		virtual long DeleteSymbols(void);
		virtual long GetSymbols(PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);

		virtual HVARLIST DefineVarList(char **pszSymbols, unsigned long ulNumOfSymbols, int bDataChange, unsigned long ulFlags, long *plResult);
		virtual long DeleteVarList(HVARLIST hVarList, int bDeleteLocalListOnly);

		virtual long SendVarList(HVARLIST hVarList, RTS_HANDLE hEvent, RTS_HANDLE hSem, int *pbSync = NULL, int bGetChangedValues = 0, unsigned long *pulVarFlags = NULL);
		virtual long GetVarList(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);
		virtual long GetValues(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);

		virtual long WriteVarList(HVARLIST hVarList, unsigned long ulNumOfSymbols, unsigned char **ppValues, unsigned long *pulValueSizes);
		virtual long WriteVars(char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned char** ppValues, unsigned long *pulValueSizes);

		virtual long SendService(unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize);

		virtual long UploadFile(char *pszPlc, char *pszHost, long *plResult);
		virtual long DownloadFile(char *pszHost, char *pszPlc, long *plResult);
		virtual long ReadDirectory(CDirInfo **ppdi, char *pszBaseDir, long *plResult);
		virtual long FileRename(char *pszOldFile, char *pszNewFile, long *plResult);
		virtual long FileDelete(char *pszFile, long *plResult);

		virtual long GetDevInfo(DeviceInfo2 **ppDeviceInfo);
		virtual long GetPrjInfo(ProjectInfo **ppPrjInfo);

	private:
		void Init(void);
		virtual long ParseBinarySymFile(unsigned char *pSymbolFile, unsigned long ulFileSize, SymbolList* pList, unsigned short *pMaxPOURef);
		virtual long SortSymbolList(PlcSymbolDesc *pSymbols, unsigned long ulNumOfSymbols);

		SymbolList m_SymbolList;
		unsigned char ***m_pppbyItemCache;
		unsigned short m_usMaxPOURef;
		PLC_STATUS m_PlcStatus;
};

#endif	//__PLCCOMSIM_H__

