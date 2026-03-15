#ifndef __PLCCOMARTI_H__
#define __PLCCOMARTI_H__

#include "ARTIClient.h"
#include "ARTISymbol.h"
#include "PLCConfig.h"
#include "PLCComBase.h"

#include "SysTimeItf.h"
#include "SysTaskItf.h"
#include "SysEventItf.h"
#include "SysModuleItf.h"

#define	PRE_INIT	0
#define POST_INIT	1

typedef struct CPLCComARTIThreadParamsTag
{
	void		*pARTIDrv;
	HVARLIST	hVarList;
	RTS_HANDLE	hEvent;
	RTS_HANDLE	hSem;
} CPLCComARTIThreadParams;

void CDECL VarListThread(SYS_TASK_PARAM *ptp);

class CPLCComARTI : public CPLCComBase
{
	public:
		// Construction with specific PLC and Device configuration 
		CPLCComARTI(PlcConfig *pConfig, PlcDeviceDesc *pDevice, RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

		virtual ~CPLCComARTI(void);

		virtual long Open(void);
		virtual long Close(void);

		virtual long Login(void);
		virtual long Logout(void);

		virtual long CheckProjectIds(void);
		virtual long CheckConnection(void);
		virtual long GetPlcProjectName(char *pszProject, unsigned long ulLen);
		virtual long GetPlcStatus(PLC_STATUS *pPlcStatus = NULL);
		
		virtual long LoadSymbols(void);
		virtual long DeleteSymbols(void);
		virtual long LoadSymbolsOffline(void);
		virtual long GetSymbols(PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);

		virtual HVARLIST DefineVarList(char **pszSymbols, unsigned long ulNumOfSymbols, int bDataChange, unsigned long ulFlags, long *plResult);
		virtual long DeleteVarList(HVARLIST hVarList, int bDeleteLocalListOnly);

		virtual long SendVarList(HVARLIST hVarList, RTS_HANDLE hEvent, RTS_HANDLE hSem, int *pbSync, int bGetChangedValues, unsigned long *pulVarFlags);
		virtual long GetVarList(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);
		virtual long GetValues(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);

		virtual long WriteVarList(HVARLIST hVarList, unsigned long ulNumOfSymbols, unsigned char **ppValues, unsigned long *pulValueSizes);
		virtual long WriteVars(char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned char** ppValues, unsigned long *pulValueSizes);

		virtual long SendService(unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize);

	private:
		virtual long LoadSymARTIFcts(void);
		void Init(char bType);
		virtual unsigned long GetARTIFlags(void);

		ARTIDeviceInfo	m_DeviceInfo;
		unsigned long	m_ulCommFlags;
		char *			m_pszSymbolFileName;
		unsigned short	m_usHWType;
		unsigned long	m_ulHWVersion;
		unsigned long	m_ulSpecialMode;

		RTS_HANDLE		m_hARTILib;
		RTS_HANDLE 		m_hSymARTILib;

		int				m_bSymbolsLoaded;
		RTS_HANDLE		m_hARTISymbolListOffline;

		PFARTIGETVERSION m_pfARTIGetVersion;
		PFARTISENDDATA m_pfARTISendData;
		PFARTIGETREPLYSIZE m_pfARTIGetReplySize;
		PFARTIGETMESSAGE m_pfARTIGetMessage;
		PFARTISETCOMMTIMEOUT m_pfARTISetCommTimeout;

		PFSYMARTIGETVERSION				m_pfSymARTIGetVersion;
		PFSYMARTIOPENCHANNELLOGGED2		m_pfSymARTIOpenChannelLogged2;
		PFSYMARTICLOSECHANNEL			m_pfSymARTICloseChannel;
		PFSYMARTILOGIN2					m_pfSymARTILogin2;
		PFSYMARTILOGINRESULT			m_pfSymARTILoginResult;
		PFSYMARTILOGINRESULTEX			m_pfSymARTILoginResultEx;
		PFSYMARTILOGOUT					m_pfSymARTILogout;
		PFSYMARTILOGOUTRESULT			m_pfSymARTILogoutResult;
		PFSYMARTIGETLASTERROR			m_pfSymARTIGetLastError;
		PFSYMARTIGETSTATE				m_pfSymARTIGetState;
		PFSYMARTIGETPLCSTATE			m_pfSymARTIGetPLCState;
		PFSYMARTIGETPLCSTATERESULT		m_pfSymARTIGetPLCStateResult;
		PFSYMARTILOADSYMBOLSEX2			m_pfSymARTILoadSymbolsEx2;
		PFSYMARTILOADSYMBOLSOFFLINEEX2	m_pfSymARTILoadSymbolsOfflineEx2;
		PFSYMARTIGETSDBPROJECTID		m_pfSymARTIGetSDBProjectID;
		PFSYMARTIDELETESYMBOLS			m_pfSymARTIDeleteSymbols;
		PFSYMARTIDELETESYMBOLSOFFLINE	m_pfSymARTIDeleteSymbolsOffline;
		PFSYMARTIGETSYMBOLS				m_pfSymARTIGetSymbols;
		PFSYMARTIGETSYMBOLSOFFLINE		m_pfSymARTIGetSymbolsOffline;
		PFSYMARTIDEFINEVARLIST			m_pfSymARTIDefineVarList;
		PFSYMARTIDELETEVARLIST			m_pfSymARTIDeleteVarList;
		PFSYMARTIUPDATEVARLIST			m_pfSymARTIUpdateVarList;
		PFSYMARTISENDVARLIST			m_pfSymARTISendVarList;
		PFSYMARTIUPDATEVARLISTBLOCKS	m_pfSymARTIUpdateVarListBlocks;
		PFSYMARTIGETVALUES				m_pfSymARTIGetValues;
		PFSYMARTIWRITEVARLISTMP			m_pfSymARTIWriteVarListMP;
		PFSYMARTIWRITEVARSMP			m_pfSymARTIWriteVarsMP;
		PFSYMARTIWRITERESULT			m_pfSymARTIWriteResult;
		PFSYMARTISETIDENTITYCHECK		m_pfSymARTISetIdentityCheck;
		PFSYMARTIGETSYMBOLDESCBYINDEX	m_pfSymARTIGetSymbolDescByIndex;
		PFSYMARTIGETTYPEINFO			m_pfSymARTIGetTypeInfo;
		PFSYMARTISETCOMMBUFFERSIZE		m_pfSymARTISetCommBufferSize;
};

#endif	//__PLCCOMARTI_H__

