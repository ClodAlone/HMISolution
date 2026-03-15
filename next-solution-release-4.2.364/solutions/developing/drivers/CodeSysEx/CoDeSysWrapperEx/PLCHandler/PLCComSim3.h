#ifndef __PLCCOMSIM3_H__
#define __PLCCOMSIM3_H__

#include "PLCConfig.h"
#include "PLCComBase.h"
#include "XmlSymbolParser.h"


typedef struct
{
	unsigned long ulSymbols;
	PlcSymbolDesc **ppSymbols;
	PlcVarValue **ppValues;
} SIMVARLIST3;


class CPLCComSim3 : public CPLCComBase
{
	public:
		// Construction with specific PLC and Device configuration 
		CPLCComSim3(PlcConfig *pConfig, PlcDeviceDesc *pDevice, RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

		virtual ~CPLCComSim3(void);

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
		virtual long GetSymbolAddress(unsigned long ulSymbolIndex, char *pszMappedAddr, long lLen);

		virtual HVARLIST DefineVarList(char **pszSymbols, unsigned long ulNumOfSymbols, int bDataChange, unsigned long ulFlags, long *plResult);
		virtual long DeleteVarList(HVARLIST hVarList, int bDeleteLocalListOnly);
		virtual long AddVarsToVarList(HVARLIST hVarlist, char **pszSymbols, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);
		virtual long RemoveVarsFromVarList(HVARLIST hVarlist, unsigned long *pulIndexList, unsigned long ulNumOfSymbols, PlcVarValue ***pppValues);

		virtual long SendVarList(HVARLIST hVarList, RTS_HANDLE hEvent, RTS_HANDLE hSem, int *pbSync = NULL, int bGetChangedValues = 0, unsigned long *pulVarFlags = NULL);
		virtual long GetVarList(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);
		virtual long GetValues(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);

		virtual long WriteVarList(HVARLIST hVarList, unsigned long ulNumOfSymbols, unsigned char **ppValues, unsigned long *pulValueSizes);
		virtual long WriteVars(char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned char** ppValues, unsigned long *pulValueSizes);

		virtual long SendService(unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize);

		virtual long UploadFile(char *pszPlc, char *pszHost, long *plResult, bool bUseStandardPath = true);
		virtual long DownloadFile(char *pszHost, char *pszPlc, long *plResult, bool bUseStandardPath = true);
		virtual long ReadDirectory(CDirInfo **ppdi, char *pszBaseDir, long *plResult);
		virtual long FileRename(char *pszOldFile, char *pszNewFile, long *plResult);
		virtual long FileDelete(char *pszFile, long *plResult);

		virtual long GetDevInfo(DeviceInfo3 **ppDeviceInfo);
		virtual long GetPrjInfo(ProjectInfo **ppPrjInfo);

	private:
		PlcSymbolDesc *FindSymbol(char *pszSymbol);

		PLC_STATUS				m_PlcStatus;
		PlcSymbolDesc			*m_pSymbolList;
		char					**m_ppszMappedAddr;
		unsigned char			*m_pbyCache;
		unsigned long			m_ulCacheSize;
		int						m_bDontExpandSimpleTypeArrays;
		int						m_bDontExpandComplexTypeArrays;
		CXmlSymbolParser		*m_pXmlParser;
};

#endif	//__PLCCOMSIM3_H__

