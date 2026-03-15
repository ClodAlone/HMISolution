#ifndef __PLCCOMARTI3_H__
#define __PLCCOMARTI3_H__

#include "PLCComBase3.h"
#include "CmpNameServiceClientItf.h"


typedef struct 
{
	int					bChannelError;
	RTS_RESULT			Result;
	RTS_HANDLE			hEvent;
	PROTOCOL_DATA_UNIT* pRecvPdu;
	unsigned short		usChannel;
} SendService3Async;

typedef struct
{
	IBase Base;
	ICmpChannelClientApp ClientApp;
} IAllCmpChannelClientApp;


class CPLCComARTI3 : public CPLCComBase3Selector
{
	public:
		// Construction with specific PLC and Device configuration 
		CPLCComARTI3(PlcConfig * const pConfig, PlcDeviceDesc * const pDevice, const PFVERIFYCERTCALLBACK pfVerifyCallback, void * const pVerifyUserData, const PFREQUESTCREDENTIALSCALLBACK pfRequestCredentialsCallback, void * const pRequestCredentialsUserData, const RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

		virtual ~CPLCComARTI3(void);

		virtual long Open(void);
		virtual long Close(void);
		virtual long ScanNetwork(RTS_UINTPTR ulUserData, PFSCANNETWORKCALLBACK pfCallback);

	private:
		virtual long SendService3(PROTOCOL_DATA_UNIT pduSend, PROTOCOL_DATA_UNIT *ppduRecv, int bUseExternalRxBuffer = 0);
		virtual long ResolveIpAddress(void);
		virtual long ResolveNodename(RTS_WCHAR *pwszNodeName);
		static void CDECL ResolveIpAddressCallback (RTS_RESULT nResult, NODEADDRESS addrNode, NODEINFOPACKED3 *pNodeInfo3, unsigned short wVersion, RTS_UI8 byClientBlkDrvType, RTS_HANDLE dwRequestId, int bComplete);
		static void CDECL ResolveNodeNameCallback (RTS_RESULT nResult, NODEADDRESS addrNode, NODEINFOPACKED3 *pNodeInfo3, unsigned short wVersion, RTS_UI8 byClientBlkDrvType, RTS_HANDLE dwRequestId, int bComplete);
		static void CDECL ResolveAllCallback (RTS_RESULT nResult, NODEADDRESS addrNode, NODEINFOPACKED3 *pNodeInfo3, unsigned short wVersion, RTS_UI8 byClientBlkDrvType, RTS_HANDLE dwRequestId, int bComplete);
		static RTS_RESULT CDECL SecureChannelSendDataCallback(PROTOCOL_DATA_UNIT pduData, void* pSendUserData);
		static RTS_RESULT CDECL SecureChannelDataReceivedCallback(PROTOCOL_DATA_UNIT pduData, void* pReceiveUserData);

		PFSCANNETWORKCALLBACK	m_pfScanNetworkCallback;
		RTS_UINTPTR				m_ulScanNetworkUserData;
		unsigned short			m_usChannel;
		IAllCmpChannelClientApp	m_IAllChannelClientApp;
		SendService3Async		m_tSendService3Async;
		unsigned long			m_ulDriverMask;
		int						m_bPlainTextAvailable;

		RTS_HANDLE				m_hSecureChannel;
		PROTOCOL_DATA_UNIT		m_pduPlainTextData;
  };

#endif	//__PLCCOMARTI3_H__

