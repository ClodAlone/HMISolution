#ifndef __PLCCOMGATEWAY3_H__
#define __PLCCOMGATEWAY3_H__

#include "PLCComBase3.h"
#include "CmpGwClientItf.h"
#include "CmpGwClient.h"

typedef struct
{
	COMMDRVINFO		ComDrvInfo;
	RTS_I8			bAllCommDriversChecked;
	RTS_I8			bDriverIsAvailable;
} GW3COMMINFO;

class CPLCComGateway3 : public CPLCComBase3Selector
{
	public:
		// Construction with specific PLC and Device configuration 
		CPLCComGateway3(PlcConfig * const pConfig, PlcDeviceDesc * const pDevice, const PFVERIFYCERTCALLBACK pfVerifyCallback, void * const pVerifyUserData, const PFREQUESTCREDENTIALSCALLBACK pfRequestCredentialsCallback, void * const pRequestCredentialsUserData, const RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

		virtual ~CPLCComGateway3(void);

		virtual long Open(void);
		virtual long Close(void);
		virtual long ScanNetwork(RTS_UINTPTR ulUserData, PFSCANNETWORKCALLBACK pfCallback);

	private:
		virtual long SendService3(PROTOCOL_DATA_UNIT pduSend, PROTOCOL_DATA_UNIT *ppduRecv, int bUseExternalRxBuffer = 0);
		virtual void ClearGw3CommInfo(void); 
		virtual long ResolveIpAddress(void);
		virtual long ResolveNodename(RTS_WCHAR *pwszNodeName);
		static void STDCALL ResolveIpAddressCallback(RTS_UINTPTR dwUser, NODEADDRESS_OLD addrNode, NODEADDRESS_OLD addrParent, GWCLIENTNODEINFO *pGwClientNodeInfo, RTS_WCHAR *wszNodeName, RTS_WCHAR *wszDeviceName, RTS_WCHAR *wszVendorName, RTS_CSTRING *pszSerialNumber, RTS_UI8 *pbyOemData, RTS_UI32 dwOemDataLength);
		static void STDCALL ResolveNodeNameCallback(RTS_UINTPTR dwUser, NODEADDRESS_OLD addrNode, NODEADDRESS_OLD addrParent, GWCLIENTNODEINFO *pGwClientNodeInfo, RTS_WCHAR *wszNodeName, RTS_WCHAR *wszDeviceName, RTS_WCHAR *wszVendorName, RTS_CSTRING *pszSerialNumber, RTS_UI8 *pbyOemData, RTS_UI32 dwOemDataLength);
		static void STDCALL ResolveAllCallback(RTS_UINTPTR dwUser, NODEADDRESS_OLD addrNode, NODEADDRESS_OLD addrParent, GWCLIENTNODEINFO *pGwClientNodeInfo, RTS_WCHAR *wszNodeName, RTS_WCHAR *wszDeviceName, RTS_WCHAR *wszVendorName, RTS_CSTRING *pszSerialNumber, RTS_UI8 *pbyOemData, RTS_UI32 dwOemDataLength);
		static void STDCALL ResolveAllAsyncResultCallback (struct tagASYNCRESULT *pAsyncRes);
		static void STDCALL ResolveNameAsyncResultCallback (struct tagASYNCRESULT *pAsyncRes);
		static void STDCALL ConnectGwAsyncResultCallback (struct tagASYNCRESULT *pAsyncRes);
		static RTS_RESULT CDECL SecureChannelSendDataCallback(PROTOCOL_DATA_UNIT pduData, void* pSendUserData);
		static RTS_RESULT CDECL SecureChannelDataReceivedCallback(PROTOCOL_DATA_UNIT pduData, void* pReceiveUserData);

		PFSCANNETWORKCALLBACK	m_pfScanNetworkCallback;
		RTS_UINTPTR				m_ulScanNetworkUserData;

		GW3COMMINFO				m_Gw3CommInfo;

		RTS_HANDLE				m_hSecureChannel;
		ASYNCRESULT				m_AsyncResult;

		unsigned long			m_ulPlainChannelSize;
		PROTOCOL_DATA_UNIT		m_pduProtocolData;
		PROTOCOL_DATA_UNIT		m_pduPlainTextData;
		int						m_bPlainTextAvailable;
 };

#endif	//__PLCCOMGATEWAY3_H__

