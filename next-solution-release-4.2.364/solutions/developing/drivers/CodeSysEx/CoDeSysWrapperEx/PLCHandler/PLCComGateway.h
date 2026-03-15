#ifndef __PLCCOMGATEWAY_H__
#define __PLCCOMGATEWAY_H__

#if defined(WIN32) && !defined(_WIN32_WCE) && !defined(_WIN32_WCE_EMULATION)
	#include "GHandle.h"
#endif

#ifndef CDECL
	#define CDECL
#endif

#include "PLCConfig.h"
#include "PLCComBase.h"

#include "SysEventItf.h"
#include "SysModuleItf.h"
#include "SysTaskItf.h"

#define GW_OK(l)				((l)>0)
#define GW_ERROR(l)				((l)<=0)

typedef struct CPLCComGatewayThreadParamsTag
{
	void		*pARTIDrv;
	HVARLIST	hVarList;
	void		*pEvent;
} CPLCComGatewayThreadParams;

void CDECL VarListThread(void *pThreadParams);

class CPLCComGateway : public CPLCComBase
{
	public:
		// Construction with specific PLC and Device configuration 
		CPLCComGateway(PlcConfig *pConfig, PlcDeviceDesc *pDevice, RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

		virtual ~CPLCComGateway(void);

		virtual long GetConfigInteractive(PlcConfig *pPlcConfig, PlcDeviceDesc *pDeviceDesc);

		virtual long Open(void);
		virtual long Close(void);

		virtual long Login(void);
		virtual long Logout(void);

		virtual long LinkToProject(char *pszProject);
		virtual long CheckProjectIds(void);
		virtual long CheckConnection(void);
		virtual long GetPlcProjectName(char *pszProject, unsigned long ulLen);
		virtual long GetPlcStatus(PLC_STATUS *pPlcStatus = NULL);
		
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

		long SetGWPwd(RTS_UTF8STRING * const pszPwd);

	private:
		virtual long LoadGatewayFcts(void);
		void Init(void);
		virtual long HandleNotification(long *plStatus, unsigned long ulTimeout = 0, unsigned long *pulReplySize = NULL);
		long RegisterWindow(void);
		long LoadGwcFromGPDump(GatewayConnection* pGwc, void* pData);
		long EncodeGatewayPassword(char *pszPassword);
		virtual long LoadSymbolsOffline(void);

		RTS_HANDLE				m_Hwnd;
		unsigned long			m_ulGateway;

		RTS_HANDLE				m_hReadyEvent;
		long					m_bClose;

		RTS_INTPTR				m_lParam;
		RTS_HANDLE				m_hWindowTask;
		long					m_bComFatal;
		int						m_bAllowAlternateSymbolfile;

		GatewayConnection		m_gwc;
		PlcDeviceDesc			m_device;

#if defined(WIN32) && !defined(_WIN32_WCE) && !defined(_WIN32_WCE_EMULATION)
		HDEVICEDESCRIPTION		m_hdd;
		GHGatewayParameters		m_gp;

		RTS_HANDLE				m_hGHandleLib;
		
		PFGHNDCLIENTGCONNECTGATEWAYEX	m_pfGHndClientGConnectGatewayEx;
		PFGHNDCLIENTGDISCONNECTGATEWAY	m_pfGHndClientGDisconnectGateway;
		PFGHNDCLIENTCONNECTGATEWAYEX	m_pfGHndClientConnectGatewayEx;
		PFGHNDCLIENTDISCONNECTGATEWAY	m_pfGHndClientDisconnectGateway;
		PFGHNDSYMGOPENCHANNELEX3		m_pfGHndSymGOpenChannelEx3;
		PFGHNDSYMOPENCHANNELEX3			m_pfGHndSymOpenChannelEx3;
		PFGHNDSYMOPENCHANNELEX2			m_pfGHndSymOpenChannelEx2;
		PFGHNDSYMOPENCHANNEL			m_pfGHndSymOpenChannel;
		PFGHNDSYMSETMOTOROLA			m_pfGHndSymSetMotorola;
		PFGHNDSYMCLOSECHANNEL			m_pfGHndSymCloseChannel;
		PFGHNDSYMSETSDBNAME				m_pfGHndSymSetSDBName;
		PFGHNDSYMSETTARGETBUFFERSIZE	m_pfGHndSymSetTargetBufferSize;
		PFGHNDSYMLOGIN					m_pfGHndSymLogin;
		PFGHNDSYMLOGIN2					m_pfGHndSymLogin2;
		PFGHNDSYMLOGINRESULT			m_pfGHndSymLoginResult;
		PFGHNDSYMLOGINRESULTEX			m_pfGHndSymLoginResultEx;
		PFGHNDSYMLOGOUT					m_pfGHndSymLogout;
		PFGHNDSYMLOGOUTRESULT			m_pfGHndSymLogoutResult;
		PFGHNDSYMGETNOTIFYPARAM			m_pfGHndSymGetNotifyParam;
		PFGHNDSYMGETPROJECTID			m_pfGHndSymGetProjectId;
		PFGHNDSYMGETPROJECTINFO			m_pfGHndSymGetProjectInfo;
		PFGHNDSYMPLCPROJECTID			m_pfGHndSymPLCProjectId;
		PFGHNDSYMPLCPROJECTIDRESULT		m_pfGHndSymPLCProjectIdResult;
		PFGHNDSYMLOADSYMBOLSEX			m_pfGHndSymLoadSymbolsEx;
		PFGHNDSYMDELETESYMBOLS			m_pfGHndSymDeleteSymbols;
		PFGHNDSYMGETALLSYMBOLS			m_pfGHndSymGetAllSymbols;
		PFGHNDSYMPLCSTATE				m_pfGHndSymPLCState;
		PFGHNDSYMPLCSTATERESULT			m_pfGHndSymPLCStateResult;
		PFGHNDSYMDEFINEVARLISTEX		m_pfGHndSymDefineVarListEx;
		PFGHNDSYMDELETEVARLIST			m_pfGHndSymDeleteVarList;
		PFGHNDSYMSENDVARLISTEX			m_pfGHndSymSendVarListEx;
		PFGHNDSYMGETVALUES				m_pfGHndSymGetValues;
		PFGHNDSYMWRITEVARLISTEX			m_pfGHndSymWriteVarListEx;
		PFGHNDSYMWRITEVARSEX			m_pfGHndSymWriteVarsEx;
		PFGHNDSYMWRITERESULT			m_pfGHndSymWriteResult;
		PFGHNDSYMWRITERESULTEX			m_pfGHndSymWriteResultEx;

		PFGHNDCLIENTWRITE				m_pfGHndClientWrite;
		PFGHNDCLIENTREAD				m_pfGHndClientRead;
		PFGHNDCLIENTGETLASTERROR		m_pfGHndClientGetLastError;
		PFGHNDCLIENTSETDWORD			m_pfGHndClientSetDWord;

		PFGHNDCLIENTSETGP				m_pfGHndClientSetGP;
		PFGHNDCLIENTINITGP				m_pfGHndClientInitGP;
		PFGHNDCLIENTCOPYGP				m_pfGHndClientCopyGP;
		PFGHNDCLIENTCLEARGP				m_pfGHndClientClearGP;
		PFGHNDCLIENTSAVEGP				m_pfGHndClientSaveGP;
		PFGHNDCLIENTGETSIZEOFGP			m_pfGHndClientGetSizeOfGP;
		PFGHNDCLIENTGETDEVICETABLE		m_pfGHndClientGetDeviceTable;
		PFGHNDCLIENTGGETDEVICETABLE		m_pfGHndClientGGetDeviceTable;
		PFGHNDCLIENTGETPROJECTNAME		m_pfGHndClientGetProjectName;

		PFGHNDDEVTABCREATE				m_pfGHndDevTabCreate;
		PFGHNDDEVTABDESTROY				m_pfGHndDevTabDestroy;
		PFGHNDDEVTABGETDEVICEBYNAME		m_pfGHndDevTabGetDeviceByName;

		PFGHNDDEVDESCSETPROJECTNAME		m_pfGHndDevDescSetProjectName;
		PFGHNDDEVDESCSETINSTANCENAME	m_pfGHndDevDescSetInstanceName;
		PFGHNDDEVDESCDUPLICATE			m_pfGHndDevDescDuplicate;
		PFGHNDDEVDESCASSIGN				m_pfGHndDevDescAssign;
		PFGHNDDEVTABASSIGN				m_pfGHndDevTabAssign;
		PFGHNDDEVTABDUPLICATE			m_pfGHndDevTabDuplicate;
		PFGHNDDEVDESCCREATE				m_pfGHndDevDescCreate;
		PFGHNDDEVDESCCLEAR				m_pfGHndDevDescClear;
		PFGHNDDEVDESCDESTROY			m_pfGHndDevDescDestroy;
		PFGHNDDEVDESCGETPARAMETERBYNAME	m_pfGHndDevDescGetParameterByName;
		PFGHNDDEVDESCGETPARAMETERBYID	m_pfGHndDevDescGetParameterById;

		PFGHNDPARAMDESCGETPARAMETER		m_pfGHndParamDescGetParameter;
		PFGHNDPARAMDESTROY				m_pfGHndParamDestroy;
		PFGHNDPARAMDESCDESTROY			m_pfGHndParamDescDestroy;

		PFGHNDPARAMSETVALUE				m_pfGHndParamSetValue;

		PFGHNDCLIENTPARAMETERDIALOG		m_pfGHndClientParameterDialog;

		PFGHNDCLIENTLINKTOPROJECT		m_pfGHndClientLinkToProject;
		PFGHNDCLIENTLINKTOPROJECTEX		m_pfGHndClientLinkToProjectEx;

		PFGHNDSYMLOADSYMBOLSOFFLINEEX	m_pfGHndSymLoadSymbolsOfflineEx;
		PFGHNDSYMDELETEOFFLINESYMBOLLIST	m_pfGHndSymDeleteOfflineSymbolList;

		GHSymbolDesc**					m_ppSymbolList;
		GHSymbolDesc*					m_hOfflineSymbolList;

		friend LRESULT CALLBACK WndProc(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);
		friend void CDECL WindowThread(SYS_TASK_PARAM *pParam);
#endif
};

#endif	//__PLCCOMGATEWAY_H__

