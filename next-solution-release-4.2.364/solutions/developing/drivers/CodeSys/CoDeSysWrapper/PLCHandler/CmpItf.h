#ifndef __CMPITF_H__
#define __CMPITF_H__

#include <stdio.h>

#if defined(WIN32) || defined(_WIN32_WCE)
	#ifdef _USRDLL
		#define DLL_DECL _declspec(dllexport)
	#endif
#endif
#if !defined(DLL_DECL)
	#define DLL_DECL
#endif

#ifndef CDECL
	#define CDECL
#endif

#ifndef CDECL_EXT
	#define CDECL_EXT
#endif

#ifndef STDCALL
	#define STDCALL __stdcall
#endif

#if defined(__cplusplus)
	extern "C" {
#endif 


typedef	RTS_UI32	CMPID;		/* Component ID to specify a component. Has the vendor ID as a prefix! */
typedef	RTS_UI32	CLASSID;	/* Class ID to specify a specific class (C or C++) */
typedef	RTS_UI32	OBJID;		/* Object ID, typically the instance index starting with 0 */
typedef	RTS_UI32	ITFID;		/* Interface ID to specify a specific interface (C or C++) */

#define RTS_VENDORID_3S						0x0000
#define ADDVENDORID(VendorID, ID)			(((RTS_UI32)(VendorID) << 16 & UINT32_C(0xFFFF0000)) + (ID))
#ifdef TRG_64BIT
	#define RTSITF_GET_SIGNATURE(sign32bit, sign64bit)		sign64bit
#else
	#define RTSITF_GET_SIGNATURE(sign32bit, sign64bit)		sign32bit
#endif


/* Function that can be used for all CAL_ macros to return ERR_NOTIMPLEMENTED */
RTS_RESULT CDECL FUNCTION_NOTIMPLEMENTED(unsigned long dummy,...);
RTS_RESULT CDECL FUNCTION_NOTIMPLEMENTED2(void *dummy,...);

struct tagIBase;

typedef struct
{
	/*pointer to virtual function table*/
	void* __VFTABLEPOINTER;
} instance_struct;

typedef struct
{
	instance_struct *pInstance;
	RTS_IEC_DWORD dwCount;
} addref_struct;

typedef struct
{
	instance_struct *pInstance;
	RTS_IEC_DWORD dwCount;
} release_struct;

typedef struct
{
	instance_struct *pInstance;
	RTS_IEC_DWORD iid;
	RTS_IEC_RESULT *pResult;
	instance_struct **ppInterface;
} queryinterface_struct;

typedef unsigned long (CDECL *PF_ADDREF)(struct tagIBase *pIBase);
typedef unsigned long (CDECL *PF_RELEASE)(struct tagIBase *pIBase);
typedef void *(CDECL *PF_QUERYINTERFACE)(struct tagIBase *pIBase, ITFID iid, RTS_RESULT *pResult);

typedef void (CDECL *PF_IEC_ADDREF)(void);
typedef void (CDECL *PF_IEC_RELEASE)(void);
typedef void (CDECL *PF_IEC_QUERYINTERFACE)(void);

/* <SIL2/> */
typedef struct tagIBase
{
	int bIEC;
	int iRefCount;
	RTS_HANDLE hInstance;
	PF_ADDREF AddRef;
	PF_RELEASE Release;
	PF_QUERYINTERFACE QueryInterface;
	CLASSID ClassId;
} IBase_C;

typedef struct tagIBase_IEC
{
	int bIEC;
	int iRefCount;
	RTS_HANDLE hInstance;
	PF_IEC_ADDREF *AddRef;
	PF_IEC_RELEASE *Release;
	PF_IEC_QUERYINTERFACE *QueryInterface;
} IBase_IEC;


#ifndef STATICITF
	#define STATICITF		static
#endif

#ifndef STATICITF_DEF
	#define STATICITF_DEF
#endif

#if defined(CPLUSPLUS) && defined(__cplusplus)
	class IBase
	{
		public:
			virtual	unsigned long AddRef(IBase *pIBase = NULL) =0;
			virtual	unsigned long Release(IBase *pIBase = NULL) =0;
			virtual	void *QueryInterface(IBase *pIBase = NULL, ITFID iid = 0, RTS_RESULT *pResult = NULL) =0;
			virtual ~IBase() { };
	};

	#define DECLARE_ADDREF()\
	virtual unsigned long AddRef(IBase *pIBase = NULL);

	#define IMPLEMENT_ADDREF()\
	virtual unsigned long AddRef(IBase *pIBase = NULL)\
	{\
		iRefCount++;\
		return iRefCount;\
	}

	#define DECLARE_RELEASE()\
	virtual unsigned long Release(IBase *pIBase = NULL);

	#define IMPLEMENT_RELEASE()\
	virtual unsigned long Release(IBase *pIBase = NULL)\
	{\
		iRefCount--;\
		if (iRefCount == 0)\
		{\
			delete this;\
			return 0;\
		}\
		return iRefCount;\
	}

	#define IMPLEMENT_RELEASE_NODELETE()\
	virtual unsigned long Release(IBase *pIBase = NULL)\
	{\
		iRefCount--;\
		if (iRefCount == 0)\
		{\
			return 0;\
		}\
		return iRefCount;\
	}
#else
	#define DECLARE_ADDREF\
	static unsigned long CDECL AddRef(struct tagIBase *pIBase);

	#define IMPLEMENT_ADDREF\
	static unsigned long CDECL AddRef(struct tagIBase *pIBase)\
	{\
		return (pIBase ? ++pIBase->iRefCount : 0);\
	}

	#define DECLARE_RELEASE\
	static unsigned long CDECL Release(struct tagIBase *pIBase);

	#define IMPLEMENT_RELEASE\
	static unsigned long CDECL Release(struct tagIBase *pIBase)\
	{\
		return (pIBase ? --pIBase->iRefCount : 0);\
	}

	#define DECLARE_QUERYINTERFACE\
	static void *CDECL QueryInterface(IBase *pBase, ITFID iid, RTS_RESULT *pResult);
	
	typedef IBase_C		IBase;
#endif

/* Component IDs */
#define	CMPID_None					0x00000000
#define	CMPID_CmpMgr				0x00000001
#define	CMPID_CM					0x00000001
#define	CMPID_CmpApp				0x00000002
#define	CMPID_CmpAddrSrvc			0x00000003 /* this component is obsolete so this id should no longer be used */
#define	CMPID_CmpBinTagUtil			0x00000004
#define	CMPID_CmpBlkDrvCom			0x00000006
#define	CMPID_CmpBlkDrvUdp			0x00000007
#define	CMPID_CmpChannelClient		0x00000008
#define	CMPID_CmpChannelMgr			0x00000009
#define	CMPID_CmpChannelServer		0x0000000A
#define	CMPID_CmpChecksum			0x0000000B
#define	CMPID_CmpCommunicationLib	0x0000000C
#define	CMPID_CmpComProtocol		0x0000000D /* this component is obsolete so this id should no longer be used */
#define	CMPID_CmpDevice				0x0000000E
#define	CMPID_CmpGateway			0x0000000F
#define	CMPID_CmpGwCommDrvTcp		0x00000010
#define	CMPID_CmpIecTask			0x00000011
#define	CMPID_CmpIoMgr				0x00000012
#define	CMPID_CmpLog				0x00000013
#define	CMPID_CmpMonitor			0x00000014
#define	CMPID_CmpNameServiceClient	0x00000015
#define	CMPID_CmpNameServiceServer	0x00000016
#define	CMPID_CmpRetain				0x00000017
#define	CMPID_CmpRouter				0x00000018
#define	CMPID_CmpSchedule			0x00000019
#define	CMPID_CmpSettings			0x0000001A
#define	CMPID_CmpService			0x0000001B
#define	CMPID_CmpSrv				0x0000001C
#define CMPID_CmpIpc				0x0000001D
#define CMPID_CmpMemPool			0x0000001E
#define CMPID_CmpMemGC				0x0000001F
#define CMPID_CmpSafetyInterface	0x00000020
#define CMPID_CmpGwClient			0x00000021
#define CMPID_CmpGwClientCommDrvTcp 0x00000022
#define CMPID_CmpScheduleEmbedded	0x00000023
#define CMPID_CmpScheduleTimer		0x00000024
#define CMPID_CmpResMgr				0x00000025
#define CMPID_CmpBlkDrvLoopback     0x00000026 /* this component is obsolete so this id should no longer be used */
#define CMPID_CmpBlkDrvCanClient	0x00000027
#define CMPID_CmpBlkDrvCanServer	0x00000028
#define CMPID_CmpPLCHandler			0x0000002A
#define CMPID_CmpBlkDrvUsb			0x0000002B
#define CMPID_CmpUsbMpdUsbLib		0x0000002C
#define CMPID_CmpChannelMgrEmbedded	0x0000002D
#define CMPID_CmpChannelServerEmbedded	0x0000002E
#define CMPID_CmpRouterEmbedded		0x0000002F
#define CMPID_CmpBlkDrvTcp			0x00000030
#define CMPID_CmpBlkDrvDirectCall	0x00000031
#define	CMPID_CmpMonitor2			0x00000032
#define CMPID_CmpOpenSSL			0x00000033

#define CMPID_CmpSercos3Master		0x0000004C
#define CMPID_CmpKnxStack			0x0000004D

#define	CMPID_CmpBitmapPool			0x00000050
#define	CMPID_CmpDynamicText		0x00000051
#define CMPID_CmpTargetVisu			0x00000052
#define CMPID_CmpTargetVisuStub		0x00000053
#define CMPID_CmpVisuHandler		0x00000054
#define CMPID_CmpVisuHandlerStub	0x00000055
#define CMPID_CmpVisuOfflineClient	0x00000056	/* this component is obsolete so this id should no longer be used */
#define CMPID_CmpVisuServer			0x00000057
#define	CMPID_CmpXMLParser			0x00000058
#define	CMPID_CmpMain				0x00000059
#define	CMPID_CmpIoDrvIec			0x0000005A
#define	CMPID_CmpEventMgr			0x0000005B
#define CMPID_CmpBinTagUtilIec		0x0000005C
#define CMPID_CmpChannelClientIec	0x0000005D
#define CMPID_CmpFileTransfer		0x0000005E
#define CMPID_CmpAsyncMgr			0x0000005F
#define CMPID_CmpIecVarAccess		0x00000060
#define CMPID_CmpUserMgr			0x00000061
#define CMPID_CmpGwCommDrvDirectCall			0x00000062
#define CMPID_CmpGwClientCommDrvDirectCall		0x00000063
#define CMPID_CmpUserDB				0x00000064
#define CMPID_CmpUserDBEmbedded		0x00000065
#define CMPID_CmpIoDrvC				0x00000066
#define CMPID_CmpSimulationRts		0x00000067
#define CMPID_CmpBlkDrvShm			0x00000068
#define CMPID_CmpHalconIntegration	0x00000069
#define CMPID_CmpCryptMD5			0x0000006A
#define CMPID_CmpCILExecutionTest	0x0000006B
#define CMPID_CmpCILExecution		0x0000006C
#define CMPID_CmpCILOfflineCompiler			0x0000006D
#define CMPID_CmpGwCommDrvShm		0x0000006E
#define CMPID_CmpGwClientCommDrvShm	0x0000006F
#define CMPID_CmpTraceMgr			0x00000070
#define CMPID_CmpWebServer			0x00000071
#define CMPID_CmpWebServerHandlerV3	0x00000072
#define CMPID_CmpAppBP				0x00000073
#define CMPID_CmpAppForce			0x00000074
#define CMPID_CmpAppEmbedded		0x00000075
#define CMPID_CmpIoMgrEmbedded		0x00000076
#define CMPID_CmpLogEmbedded		0x00000077
#define CMPID_CmpOpcServer          0x00000078
#define CMPID_CmpTargetVisuAutoTest	0x00000079
#define CMPID_CmpCodeMeter			0x0000007A
#define CMPID_CmpSysTest			0x0000007B
#define CMPID_CmpAlarmManager		0x0000007C
#define CMPID_CmpSIL2				0x0000007D
#define CMPID_CmpCAAStorage			0x0000007E
#define CMPID_CmpIecStringUtils		0x0000007F
#define CMPID_CmpObjectMgr			0x00000080
#define CMPID_CmpHeapPool			0x00000081
#define CMPID_CmpBlkDrvShmRtx		0x00000082
#define CMPID_CmpCoreDump			0x00000083
#define CMPID_CmpAppBPEmbedded		0x00000084
#define CMPID_AppCModuleIntegration	0x00000085	/* Is reserved for all application related C-Modules */
#define CMPID_CmpBackup				0x00000086
#define CMPID_CmpBeagleboneBlack	0x00000087
#define CMPID_CmpPfcx00				0x00000088
#define CMPID_CmpRasPi				0x00000089
#define CMPID_CmpKbus				0x0000008A
#define	CMPID_CmpEventMgrEmbedded	0x0000008B
#define CMPID_CmpJanzemPC			0x0000008C
#define CMPID_CmpOPCUAStack    		0x0000008D
#define CMPID_CmpSecurityManager    0x0000008E
#define CMPID_CmpSupervisor			0x0000008F
#define	CMPID_CmpSecureChannel		0x00000090
#define	CMPID_CmpUserDBObjects		0x00000091
#define CMPID_CmpLinuxSL			0x00000092
#define CMPID_CmpPLCnext			0x00000093

#define	CMPID_SysCom				0x00000100
#define	CMPID_SysCpuHandling		0x00000101
#define	CMPID_SysEvent				0x00000102
#define	CMPID_SysExcept				0x00000103
#define	CMPID_SysFile				0x00000104
#define	CMPID_SysGraphic			0x00000105
#define	CMPID_SysInt				0x00000106
#define	CMPID_SysInternalLib		0x00000107
#define	CMPID_SysMem				0x00000108
#define	CMPID_SysModule				0x00000109
#define	CMPID_SysMsgQ				0x0000010A
#define	CMPID_SysOut				0x0000010B
#define	CMPID_SysPci				0x0000010C
#define	CMPID_SysPort				0x0000010D
#define	CMPID_SysProcess			0x0000010E
#define	CMPID_SysSem				0x0000010F
#define	CMPID_SysShm				0x00000110
#define	CMPID_SysSocket				0x00000111
#define	CMPID_SysTarget				0x00000112
#define	CMPID_SysTargetVisu			0x00000113		/* Obsolete, no longer supported */
#define	CMPID_SysTask				0x00000114
#define	CMPID_SysTime				0x00000115
#define	CMPID_SysTimer				0x00000116
#define	CMPID_SysWindow				0x00000117
#define	CMPID_SysSafetyInterface3S	0x00000118
#define	CMPID_SysSemProcess			0x00000119
#define	CMPID_SysWindowFileDialog	0x0000011A
#define	CMPID_SysDir				0x0000011B
#define	CMPID_SysEthernet			0x0000011C
#define CMPID_CmpNameServiceClientIec		0x0000011D
#define CMPID_SysFlash				0x0000011E
#define CMPID_SysFileFlash			0x0000011F
#define CMPID_SysFileStream			0x00000120
#define	CMPID_CmpSettingsEmbedded	0x00000121
#define CMPID_CmpParamDevice		0x00000122
#define CMPID_CmpVisuHandlerRemote  0x00000123
#define CMPID_CmpOPCUAServer		0x00000124
#define CMPID_CmpOPCUA      		0x00000125
#define CMPID_CmpOPCUAProviderIecVarAccess		0x00000126
#define CMPID_SysTimeRtc			0x00000127
#define CMPID_CmpPlcShell			0x00000128
#define CMPID_CmpRedundancy			0x00000129
#define CMPID_SysGraphicGDIPlus		0x0000012A
#define CMPID_SysWindowTest			0x0000012B
#define CMPID_CmpVisuClient			0x0000012C
#define CMPID_SysWindowNative		0x0000012D
#define CMPID_SysGraphicFramebuffer	0x0000012E
#define CMPID_SysNativeControl		0x0000012F
#define CMPID_SysWindowNativeTest	0x00000130
#define CMPID_CmpVisuClientController		0x00000131
#define CMPID_CmpVisuClientControllerHost	0x00000132
#define CMPID_SysSocketEmbedded 	0x00000133
#define CMPID_SysSocketlwIP 	    0x00000134
#define CMPID_SysGraphicLight		0x00000135
#define CMPID_SysModRTMain			0x00000136
#define CMPID_SysModRTSub			0x00000137
#define CMPID_SysNativeCommonControls		0x00000138
#define CMPID_SysSemCount           0x00000139
#define CMPID_SysMutex              0x0000013A
#define CMPID_SysFramebuffer 		0x0000013B
#define CMPID_SysGraphicLightFramebuffer 0x0000013C
#define CMPID_SysCpuMultiCore		0x0000013D
#define CMPID_SysReadWriteLock		0x0000013E
#define CMPID_IoDrvTarget			0x00000200
#define CMPID_CmpHilscherCIFX		0x00000201
#define CMPID_CmpHilscherCIFXlib		0x00000202
#define CMPID_CmpCharDevice			0x00000300
#define CMPID_SysDirEmbedded        0x00000301
#define CMPID_SysFileEmbedded       0x00000302
#define CMPID_SysUserDB		        0x00000303
#define CMPID_CmpOpenCV 		0x00000304
#define CMPID_SysCpuBreakpoints		0x00000305
/* umy: CMPID_CmpBACnet was defined in CmpBACnetDep.m4, so I moved it here, but kept the value */
#define CMPID_CmpBACnet				0x00000400

/* IEC code components */
#define CMPID_IecCode				0x00001000
#define	CMPID_CmpIoDrvHilscher		0x00001001
#define	CMPID_CmpIoDrvCanConfig		0x00001002
#define	CMPID_CmpIoDrvCanNode		0x00001003
#define	CMPID_CmpIoDrvCifX			0x00001004
#define CMPID_CmpIoDrvCifXProfiNetDevice 0x00001005
#define CMPID_CmpSoftingProfibus	0x00001006
#define CMPID_IecVarAccess			0x00001007
#define CMPID_IECEL6631				0x000010A0
#define	CMPID_CmpIoDrvModbus		0x000010A1
#define	CMPID_Profinet				0x000010A2

/*Safety SIL2 components*/
#define CMPID_IoDrvUnsafeBridge		0x00001E00

/*Safety SIL3 components*/
#define CMPID_CmpSafetyProxy		0x00001F02
#define CMPID_SysSafetyProxy		0x00001F80
#define CMPID_SysSafetyProxyStarter	0x00001F81

/* OEM specific components */
#define	CMPID_CmpStartOEM			0x00002000
#define	CMPID_CmpEndOEM				0x00003FFF

/* CAA specific components */
#define	CMPID_CmpStartCAA			0x00004000
#define CMPID_CmpCAACallback		0x00004001
#define CMPID______________			0x00004002
#define CMPID_CmpCAAMemBlockMan		0x00004003
#define CMPID_CmpCAACanL2			0x00004004
#define CMPID_CmpCAACanL2I			0x00004005
#define CMPID_CmpCAATypes			0x00004006
#define CMPID_CmpCAAAsyncMan		0x00004007
#define CMPID_CmpCAAFile			0x00004008
#define CMPID_CmpCAATick			0x00004009
#define CMPID_CmpCAATickUtil		0x00004010
#define CMPID_CmpCAASdoClient		0x00004011
#define CMPID_CmpCAASerialCom		0x00004012
#define CMPID_CmpCAADTUtil			0x00004013
#define CMPID_CmpCAARealTimeClock	0x00004014
#define CMPID_CmpCAABehaviourModel	0x00004015
#define CMPID_CmpCAATimer			0x00004016
#define CMPID_CmpCAASdoServer		0x00004017
#define CMPID_CmpCAANetBaseServices 0x00004018
#define CMPID_CmpCAASegBufferMan	0x00004019

/* CAA Can Mini Driver */
#define	CMPID_CmpStartCMD			0x00005F00
#define CMPID_CmpMSCCanDrv			0x00005F01
#define CMPID_CmpSJACanDrv			0x00005F02
#define CMPID_CmpPUSBCanDrv			0x00005F03
#define CMPID_CmpTC555CanDrv		0x00005F04
#define CMPID_CmpTCCanDrv			0x00005F05
#define CMPID_CmpBFCanDrv			0x00005F06
#define CMPID_CmpIFICanDrv			0x00005F07
#define CMPID_CmpArmCanDrv			0x00005F08
#define CMPID_Cmp82527CanDrv		0x00005F09
#define CMPID_CmpNetXCanDlDrv		0x00005F0A
#define CMPID_CmpEL6751CanDrv		0x00005F0B
#define CMPID_CmpEsdCanDrv			0x00005F0C
#define CMPID_CmpSocketCanDrv		0x00005F0D
#define CMPID_CmpEMSCanDrv			0x00005F0E
#define CMPID_CmpPCANBasicDrv		0x00005F0F
#define CMPID_CmpCANFoxDrv	     	0x00005F10
#define CMPID_CmpIxxatCANDrv		0x00005F11

#define	CMPID_CmpEndCMD				0x00005FFF
#define	CMPID_CmpEndCAA				0x00005FFF

/* Test and verification */
#define CMPID_CmpStartTest			0x00006000
#define CMPID_CmpRuntimeTest		0x00006001

#define CMPID_CmpEndTest            0x000060FF

/* Range reserved for IEC Applications that register as a component.
 * This is needed for logging purposes or for the certificate management */
#define CMPID_CmpIecApplicationStart    0x00008000
#define CMPID_CmpIecApplicationEnd      0x00008FFF

/* C++ Class IDs */
#define	CLASSID_INVALID					0x00000000
#define	CLASSID_CCmpMgr					0x00000001
#define	CLASSID_CCM						0x00000001
#define	CLASSID_CCMUtils				0x00000001
#define	CLASSID_CCmpApp					0x00000002
#define	CLASSID_CCmpAddrSrvc			0x00000003 /* this component is obsolete so this id should no longer be used */
#define	CLASSID_CCmpBinTagUtil			0x00000004
#define	CLASSID_CCmpBitmapPool			0x00000005
#define	CLASSID_CCmpBlkDrvCom			0x00000006
#define	CLASSID_CCmpBlkDrvUdp			0x00000007
#define	CLASSID_CCmpChannelClient		0x00000008
#define	CLASSID_CCmpChannelMgr			0x00000009
#define	CLASSID_CCmpChannelServer		0x0000000A
#define	CLASSID_CCmpChecksum			0x0000000B
#define	CLASSID_CCmpCommunicationLib	0x0000000C
#define	CLASSID_CCmpComProtocol			0x0000000D /* this component is obsolete so this id should no longer be used */
#define	CLASSID_CCmpDevice				0x0000000E
#define	CLASSID_CCmpDynamicText			0x0000000F
#define	CLASSID_CCmpGateway				0x00000010
#define	CLASSID_CCmpGwCommDrvTcp		0x00000011
#define	CLASSID_CCmpIecTask				0x00000012
#define	CLASSID_CCmpIoMgr				0x00000013
#define	CLASSID_CCmpIoDrv				0x00000014
#define	CLASSID_CCmpLog					0x00000015
#define	CLASSID_CCmpMonitor				0x00000016
#define	CLASSID_CCmpNameServiceClient	0x00000017
#define	CLASSID_CCmpNameServiceServer	0x00000018
#define	CLASSID_CCmpRetain				0x00000019
#define	CLASSID_CCmpRouter				0x0000001A
#define	CLASSID_CCmpSchedule			0x0000001B
#define	CLASSID_CCmpSettings			0x0000001C
#define	CLASSID_CCmpService				0x0000001D
#define	CLASSID_CCmpSrv					0x0000001E
#define	CLASSID_CCmpXMLParser			0x0000001F
#define CLASSID_CCmpTargetVisu			0x00000020
#define CLASSID_CCmpVisuHandler			0x00000021
#define CLASSID_CCmpVisuHandlerStub		0x00000022
#define CLASSID_CCmpTargetVisuStub		0x00000023
#define CLASSID_CCmpVisuServer			0x00000024
#define CLASSID_CCmpIpc					0x00000025
#define CLASSID_CCmpVisuOfflineClient	0x00000026	/* this component is obsolete so this id should no longer be used */
#define CLASSID_CCmpMemPool				0x00000027
#define CLASSID_CCmpMemGC				0x00000028
#define CLASSID_CCmpSafetyInterface		0x00000029
#define CLASSID_CCmpGwClient			0x0000002A
#define CLASSID_CCmpChannelServerApp	0x0000002B
#define CLASSID_CCmpIoDrvIec			0x0000002C
#define CLASSID_CCmpEventMgr			0x0000002D
#define CLASSID_CCmpEventCallbackIec	0x0000002E
#define CLASSID_CCmpBinTagUtilIec		0x0000002F
#define CLASSID_CCmpChannelClientIec	0x00000030
#define CLASSID_CCmpFileTransfer		0x00000031
#define CLASSID_CCmpAsyncMgr			0x00000032
#define CLASSID_CCmpIecVarAccess		0x00000033
#define CLASSID_CCmpBlkDrvLoopback      0x00000034 /* this component is obsolete so this id should no longer be used */
#define CLASSID_CCmpUserMgr				0x00000035
#define CLASSID_CCmpUserDB				0x00000036
#define CLASSID______________________1	0x00000037	/*CLASSID_CCmpUserDBEmbedded*/
#define CLASSID_CCmpSimulationRts		0x00000038
#define	CLASSID_CCmpNameServiceClientIec	0x00000039
#define	CLASSID_CCmpIoDrvC				0x0000003A
#define CLASSID_CCmpIoDrvIecWrapper		0x0000003B
#define CLASSID_CIoDrvTarget			0x0000003C
#define CLASSID_CCmpIecVarAccess3		0x0000003D
#define CLASSID_CIecVarAccess3			0x0000003E		/* This is only a dummy for C++ */
#define CLASSID_CIecVarAccess4			0x0000003E		/* This is only a dummy for C++ */
#define CLASSID_CCmpGwCommDrvDirectCall			0x0000003F
#define CLASSID_CCmpGwClientCommDrvDirectCall	0x00000040
#define CLASSID_CCmpIoDrvProfinet		0x00000041
#define	CLASSID_CCmpChannelClientApp	0x00000043
#define	CLASSID_CCmpBlkDrvShm			0x00000044
#define	CLASSID_CCmpCryptMD5			0x00000045
#define	CLASSID_CCmpCILExecutionTest	0x00000046
#define	CLASSID_CCmpCILExecution		0x00000047
#define CLASSID_CCmpCILOfflineCompiler	0x00000048
#define CLASSID_CCmpBlkDrvCanClient		0x00000049
#define CLASSID_CCmpBlkDrvCanServer		0x0000004A
#define CLASSID_CCmpPLCHandler			0x0000004B
#define CLASSID_CCmpSercos3Master		0x0000004C
#define CLASSID_CCmpBlkDrvUsb			0x0000004D
#define CLASSID_CCmpUsbMpdUsbLib		0x0000004E
#define CLASSID_CCmpGwCommDrvShm		0x0000004F
#define CLASSID_CCmpGwClientCommDrvShm	0x00000050
#define CLASSID_CCmpTraceMgr			0x00000051
#define CLASSID_CCmpWebServer			0x00000052
#define CLASSID_CCmpWebServerHandlerV3	0x00000053
#define CLASSID_CIecVarAccess5			0x00000054
#define CLASSID_CCmpAppBP				0x00000055
#define CLASSID_CCmpAppForce			0x00000056
#define CLASSID______________________2	0x00000057	/*CLASSID_CCmpAppEmbedded*/
#define CLASSID_CCmpTargetVisuAutoTest	0x00000058
#define	CLASSID_CCmpBlkDrvTcp			0x00000059
#define CLASSID_CCmpCodeMeter			0x0000005A
#define CLASSID_CCmpSysTest				0x0000005B
#define CLASSID_CCmpAlarmManager 		0x0000005C
#define CLASSID_CCmpSIL2				0x0000005D
#define CLASSID_CCmpCAAStorage			0x0000005E
#define CLASSID_CCmpIecStringUtils		0x0000005F
#define CLASSID_CCmpObjectMgr		   	0x00000060
#define CLASSID_CCMLock					0x00000061
#define	CLASSID_CCmpBlkDrvDirectCall	0x00000062
#define	CLASSID_CCmpMonitor2			0x00000063
#define CLASSID_CCmpHeapPool			0x00000064
#define	CLASSID_CCmpBlkDrvShmRtx		0x00000065
#define CLASSID_CIecVarAccess6			0x00000066
#define CLASSID_CCmpTls					0x00000067
#define CLASSID_CIecVarAccess_Itfs		0x00000068
#define CLASSID_CCmpCoreDump			0x00000069
#define CLASSID_CCmpAppBPEmbedded		0x0000006A
#define CLASSID_CCmpBackup				0x0000006B
#define CLASSID_CCmpCrypto              0x0000006C
#define	CLASSID_CmpEventMgr				0x0000006D
#define	CLASSID_CCmpSecurityManager		0x0000006E
#define	CLASSID_CCmpSupervisor			0x0000006F
#define	CLASSID_CCmpSecureChannel		0x00000070
#define CLASSID_CCmpMemPoolHash			0x00000071
#define CLASSID_CCMUtilsHash			0x00000072
#define CLASSID_CCmpUserDBObjects		0x00000073

#define	CLASSID_CSysCom					0x00000100
#define	CLASSID_CSysCpuHandling			0x00000101
#define	CLASSID_CSysEvent				0x00000102
#define	CLASSID_CSysExcept				0x00000103
#define	CLASSID_CSysFile				0x00000104
#define	CLASSID_CSysGraphic				0x00000105
#define	CLASSID_CSysInt					0x00000106
#define	CLASSID_CSysInternalLib			0x00000107
#define	CLASSID_CSysMem					0x00000108
#define	CLASSID_CSysMemGC				0x00000108
#define	CLASSID_CSysMemPool				0x00000108
#define	CLASSID_CSysModule				0x00000109
#define	CLASSID_CSysMsgQ				0x0000010A
#define	CLASSID_CSysOut					0x0000010B
#define	CLASSID_CSysPci					0x0000010C
#define	CLASSID_CSysPort				0x0000010D
#define	CLASSID_CSysProcess				0x0000010E
#define	CLASSID_CSysSem					0x0000010F
#define	CLASSID_CSysShm					0x00000110
#define	CLASSID_CSysSocket				0x00000111
#define	CLASSID_CSysTarget				0x00000112
#define	CLASSID_CSysTargetVisu			0x00000113		/* Obsolete, no longer supported */
#define	CLASSID_CSysTask				0x00000114
#define	CLASSID_CSysTime				0x00000115
#define	CLASSID_CSysTimer				0x00000116
#define	CLASSID_CSysWindow				0x00000117
#define CLASSID_CSysSafetyInterface3S	0x00000118
#define CLASSID_CSysSemProcess			0x00000119
#define	CLASSID_CSysWindowFileDialog	0x0000011A
#define	CLASSID_CSysDir					0x0000011B
#define	CLASSID_CSysEthernet			0x0000011C
#define CLASSID_CmpNameServiceClientIec	0x0000011D
#define CLASSID_CSysFlash				0x0000011E
#define CLASSID_CSysFileFlash			0x0000011F
#define CLASSID_CSysFileStream			0x00000120
#define	CLASSID______________________3	0x00000121	/*CLASSID_CCmpSettingsEmbedded*/
#define CLASSID_CCmpParamDevice			0x00000122
#define CLASSID_CCmpVisuHandlerRemote	0x00000123
#define CLASSID______________________4	0x00000124
#define CLASSID______________________6  0x00000125
#define CLASSID______________________5	0x00000126
#define CLASSID_CSysTimeRtc				0x00000127
#define CLASSID_CSysGraphicGDIPlus		0x00000128
#define CLASSID_CSysWindowTest			0x00000129
#define CLASSID_CCmpVisuClient			0x0000012A
#define CLASSID_CSysWindowNative		0x0000012B
#define CLASSID_CSysGraphicFramebuffer	0x0000012C
#define CLASSID_CSysWindowNativeTest	0x0000012D
#define CLASSID_CSysNativeControl		0x0000012E
#define CLASSID_CCmpVisuClientController		0x0000012F
#define CLASSID_CCmpVisuClientControllerHost	0x00000130
#define CLASSID_CSysGraphicNative		0x00000131
#define CLASSID_CSysGraphicLight		0x00000132
#define CLASSID_CSysModRTMain			0x00000133
#define CLASSID_CSysModRTSub			0x00000134
#define CLASSID_CSysSocket2				0x00000135
#define CLASSID_CSysNativeCommonControls		0x00000136
#define CLASSID_CSysFramebuffer			0x00000137
#define CLASSID_CSysSemCount			0x00000138
#define CLASSID_CSysMutex				0x00000139
#define CLASSID_CSysCpuMultiCore		0x0000013D
#define CLASSID_CCmpOPCUAProviderIecVarAccess 0x0000013E
#define CLASSID_CCmpX509Cert			0x0000013F
#define CLASSID_CCmpOPCUAStack			0x00000140
#define CLASSID_CCmpOPCUAServer			0x00000141
#define CLASSID_CSysUserDB				0x00000142
#define CLASSID_CCmpOpenCV 				0x00000143
#define CLASSID_CSysReadWriteLock		0x00000144
#define CLASSID_CSysCpuBreakpoints		0x00000145
/* umy: CLASSID_CCmpBACnet was defined in CmpBACnetDep.m4, so I moved it here, but kept the value */
#define CLASSID_CCmpBACnet				0x00000400

/* No kernel classes */
#define	CLASSID_CCmpIoDrvHilscher		0x00001001
#define	CLASSID_CCmpIoDrvCanConfig		0x00001002
#define	CLASSID_CCmpIoDrvCanNode		0x00001003
#define	CLASSID_CCmpIoDrvCifX			0x00001004
#define	CLASSID_CCmpHilscherCIFX		0x00001005
#define	CLASSID_CCmpHalconIntegration	0x00001006
#define	CLASSID_CCmpPlcShell			0x00001007
#define CLASSID_CCmpRedundancy			0x00001008
#define CLASSID_CmpSoftingProfibus		0x00001009
#define CLASSID_CCmpPfcx00				0x0000100A
#define CLASSID_CCmpRasPi				0x0000100B
#define CLASSID_CCmpBeagleboneBlack		0x0000100C
#define CLASSID_CCmpKbus				0x0000100D
#define	CLASSID_CCmpHilscherCIFXlib		0x0000100E
#define CLASSID_CCmpLinuxSL				0x0000100F
#define CLASSID_CCmpPLCnext				0x00001010

/*Safety SIL2 classes*/
#define CLASSID_CIoDrvUnsafeBridge		0x00001E00

/*Safety SIL3 classes*/
#define CLASSID_CCmpSafetyIORouter		0x00001F01
#define CLASSID_CCmpSafetyProxy			0x00001F02

#define CLASSID_CSysSafetyProxy			0x00001F80
#define CLASSID_CSysSafetyProxyStarter	0x00001F81
#define CLASSID_CSysSafetyIORouter		0x00001F83

/* OEM specific start id */
#define	CLASSID_CCmpStartOEM			0x00002000
#define	CLASSID_CCmpEndOEM				0x00003FFF

/* CAA specific components */
#define	CLASSID_CCmpStartCAA			0x00004000
#define CLASSID_CCmpCAACallback			0x00004001
#define CLASSID_________________		0x00004002
#define CLASSID_CCmpCAAMemBlockMan		0x00004003
#define CLASSID_CCmpCAACanL2			0x00004004
#define CLASSID_CCmpCAACanL2I			0x00004005
#define CLASSID_CCmpCAATypes			0x00004006
#define CLASSID_CCmpCAAAsyncMan			0x00004007
#define CLASSID_CCmpCAAFile				0x00004008
#define CLASSID_CCmpCAATick				0x00004009
#define CLASSID_CCmpCAATickUtil			0x00004010
#define CLASSID_CCmpCAASdoClient		0x00004011
#define CLASSID_CCmpCAASerialCom		0x00004012
#define CLASSID_CCmpCAADTUtil			0x00004013
#define CLASSID_CCmpCAARealTimeClock	0x00004014
#define CLASSID_CCmpCAABehaviourModel	0x00004015
#define CLASSID_CCmpCAATimer			0x00004016
#define CLASSID_CCmpCAASdoServer		0x00004017
#define CLASSID_CCmpCAANetBaseServices	0x00004018
#define CLASSID_CCmpCAASegBufferMan		0x00004019


/* CAA Can Mini Driver */
#define	CLASSID_CCmpStartCMD			0x00005F00
#define CLASSID_CCmpMSCCanDrv			0x00005F01
#define CLASSID_CCmpSJACanDrv			0x00005F02
#define CLASSID_CCmpPUSBCanDrv			0x00005F03
#define CLASSID_CCmpTC555CanDrv			0x00005F04
#define CLASSID_CCmpTCCanDrv			0x00005F05
#define CLASSID_CCmpBFCanDrv			0x00005F06
#define CLASSID_CCmpIFICanDrv			0x00005F07
#define CLASSID_CCmpArmCanDrv			0x00005F08
#define CLASSID_CCmp82527CanDrv			0x00005F09
#define CLASSID_CCmpNetXCanDlDrv		0x00005F0A
#define CLASSID_CCmpEL6751CanDrv		0x00005F0B
#define CLASSID_CCmpEsdCanDrv			0x00005F0C
#define CLASSID_CCmpSocketCanDrv		0x00005F0D
#define CLASSID_CCmpEMSCanDrv			0x00005F0E
#define CLASSID_CCmpPCANBasicDrv		0x00005F0F
#define CLASSID_CCmpCANFoxDrv			0x00005F10
#define CLASSID_CCmpIxxatCANDrv			0x00005F11

#define	CLASSID_CCmpEndCMD				0x00005FFF
#define	CLASSID_CCmpEndCAA				0x00005FFF

/* Test and verification CLASS IDs*/
#define CLASSID_CCmpStartTest			0x00006000
#define CLASSID_CCmpRuntimeTest		    0x00006001

#define CLASSID_CCmpEndTest             0x000060FF

/* Interface IDs */
#define ITFID_IBase						0x00000000
#define ITFID_ICmpMgr					0x00000001
#define ITFID_ICM						0x00000001
#define ITFID_ICmpDevice				0x00000002
#define ITFID_ICmpAppMgr				0x00000003
#define ITFID_ICmpApp					0x00000004
#define ITFID_ICmpIecTask				0x00000006
#define ITFID_ICmpScheduler				0x00000007
#define ITFID_ICmpSettings				0x00000008
#define ITFID_ICmpLoggerMgr				0x00000009
#define ITFID_ICmpLog					0x0000000A
#define ITFID_ICmpRouterMgr				0x0000000B
#define ITFID_ICmpRouter				0x0000000C
#define ITFID_ICmpSrv					0x0000000D
#define ITFID_ICmpMonitor				0x0000000E
#define ITFID_ICmpIoMgr					0x0000000F
#define ITFID_ICmpIoDrv					0x00000010
#define ITFID_ICmpBlkDrv				0x00000011
#define	ITFID_ICmpMemGC					0x00000012
#define	ITFID_ICmpMemPool				0x00000013
#define ITFID_ICmpSafetyInterface		0x00000014
#define ITFID_ICmpGwClient				0x00000015
#define ITFID_ICmpLogBackend			0x00000016
#define ITFID_ICmpCommunicationLib		0x00000017
#define ITFID_ICmpAddrSrvc				0x00000018 /* this interface is obsolete so this id should no longer be used */
#define ITFID_ICmpNameServiceServer		0x00000019
#define ITFID_ICmpChannelServerApp		0x0000001A
#define ITFID_ICmpRetain				0x0000001B
#define ITFID_ICmpSchedule				0x0000001C
#define ITFID_ICmpBinTagUtil			0x0000001D
#define ITFID_ICmpBlkDrvUdp				0x0000001E
#define ITFID_ICmpChannelServer			0x0000001F
#define ITFID_ICmpChannelMgr			0x00000020
#define	ITFID_ICmpChecksum				0x00000021
#define ITFID_ICmpIoDrvIec				0x00000022
#define ITFID_ICmpIoDrvParameter		0x00000023
#define ITFID_ICmpEventMgr				0x00000024
#define ITFID_ICmpEventCallback			0x00000025
#define ITFID_ICmpBinTagUtilIec			0x00000026
#define ITFID_ICmpChannelClientIec		0x00000027
#define ITFID_ICmpFileTransfer			0x00000028
#define ITFID_ICmpIecVarAccess			0x00000029
#define ITFID_ICmpMgrUtils				0x0000002A
#define ITFID_ICMUtils					0x0000002A
#define ITFID_ICmpMgrBasicChecks		0x0000002B
#define ITFID_ICmpIoDrvDPV1C1Master		0x0000002C
#define ITFID_ICmpIoDrvDPV1C1Slave		0x0000002D
#define ITFID_ICmpIoDrvBusControl		0x0000002E
#define ITFID_ICmpUserMgr				0x0000002F
#define ITFID_ICmpChannelClientApp		0x00000030
#define ITFID_ICmpAsyncMgr				0x00000031
#define ITFID_ICmpIoDrvProfibus			0x00000032
#define ITFID_IIecVarAccess3			0x00000033
#define ITFID_ICmpDynamicText			0x00000034
#define ITFID_ICmpXMLParser				0x00000035
#define ITFID_ICmpVisuHandler			0x00000036
#define ITFID_ICmpVisuServer			0x00000037
#define ITFID_ICmpUserDB				0x00000038
#define ITFID_ICmpIoDrvParameter2		0x00000039
#define ITFID_ICmpSimulationRts			0x0000003A
#define ITFID_ICmpChannelClient			0x0000003C
#define ITFID_ICmpTargetVisu			0x0000003D
#define ITFID_ICmpIoDrvProfinet			0x0000003E
#define ITFID_ICmpIecVarAccess3			0x0000003F
#define ITFID_ICmpNameServiceClient		0x00000040
#define ITFID_ICmpIoDrvC				0x00000041
#define ITFID_ICmpNameServiceClientIec	0x00000042
#define ITFID_ICmpHilscherCIFX			0x00000043
#define ITFID_ICmpCryptMD5				0x00000044
#define ITFID_ICmpTraceMgr				0x00000045
#define ITFID_ICmpCILExecutionTest		0x00000046
#define ITFID_ICmpCILExecution			0x00000047
#define ITFID_IIecVarAccess4			0x00000048
#define ITFID_ICmpCILOfflineCompiler	0x00000049
#define ITFID_ICmpGateway				0x0000004A
#define ITFID_ICmpGwCommDrvTcp			0x0000004B
#define ITFID_ICmpSercos3Master			0x0000004C
#define ITFID_ICmpPLCHandler			0x0000004D
#define ITFID_ICmpBitmapPool			0x0000004E
#define ITFID_IIecVarAccess5			0x0000004F
#define ITFID_ICmpIoDrvPbSlaveActivation 0x00000050
#define ITFID_ICmpIoDrvPbLiveList		0x00000051
#define ITFID_ICmpWebServer				0x00000052
#define ITFID_ICmpWebServerHandlerV3	0x00000053
#define ITFID_ICmpOPCUAStack			0x00000054
#define ITFID_ICmpOPCUAServer			0x00000055
#define ITFID_ICmpOPCUAClient			0x00000056
#define ITFID_ICmpIoDrvBusControl2		0x00000057
#define ITFID_ICmpAppBP					0x00000058
#define ITFID_ICmpAppForce				0x00000059
#define ITFID_ICmpIoDrvProfibusConfig	0x0000005A
#define ITFID_ICmpTargetVisuAutoTest	0x0000005B
#define ITFID_ICmpVisuClient			0x0000005C
#define ITFID_ICmpIoDrvProfiNetDevice	0x0000005D
#define ITFID_ICmpIoDrvEIPAcyclicServices 0x0000005E
#define ITFID_ICmpCodeMeter				0x0000005F
#define ITFID_ICmpAlarmManager			0x00000060
#define ITFID_ICmpSIL2					0x00000061
#define ITFID_ICmpCAAStorage			0x00000062
#define ITFID_ICmpSoftingProfibus		0x00000063
#define ITFID_ICmpIecStringUtils		0x00000064
#define ITFID_ICmpObjectMgr				0x00000065
#define ITFID_ICMLock					0x00000066
#define ITFID_ICmpIpc					0x00000067
#define ITFID_ICmpVisuClientControllerHost 0x00000068
#define ITFID_ICmpTargetVisuStub		0x00000069
#define ITFID_ICmpMonitor2				0x0000006A
#define ITFID_IIecVarAccess6			0x0000006B
#define ITFID_IIecVarAccess7			0x0000006C
#define ITFID_IIecVarAccess_Itfs		ITFID_IIecVarAccess7
#define ITFID_ICmpCoreDump				0x0000006D
#define ITFID_ICmpHeapPool				0x0000006E
#define ITFID_ICmpParamDevice			0x0000006F
#define ITFID_IIecVarAccess8			0x00000070
#define ITFID_ICmpBackup				0x00000071
#define ITFID_IIecVarAccess9			0x00000072
#define ITFID_ICmpPfcx00				0x00000073
#define ITFID_ICmpRasPi					0x00000074
#define ITFID_ICmpBeagleboneBlack		0x00000075
#define ITFID_ICmpKbus					0x00000076
#define ITFID_IIecVarAccess10			0x00000077
#define ITFID_ICmpOPCUAProvider			0x00000078
#define ITFID_IIecVarAccess11			0x00000079
#define ITFID_ICmpIoDrvDPV1C2Master		0x00000099
#define ITFID_ICmpSecurityManager		0x0000009A
#define ITFID_ICmpSupervisor			0x0000009B
#define ITFID_ISysSocket2external       0x0000009C
#define ITFID_ICmpSecureChannel			0x0000009D
#define ITFID_ICmpMemPoolHash			0x0000009E
#define ITFID_ICMUtilsHash				0x0000009F
#define ITFID_IIecVarAccess12			0x000000A0
#define ITFID_ICmpOPCUAProviderAlarmConfiguration 0x000000A1
#define ITFID_ICmpUserDBObjects			0x000000A2
#define ITFID_ICmpLinuxSL				0x000000A3
#define ITFID_IIecVarAccess13			0x000000A4
#define ITFID_IIecVarAccess14			0x000000A5
#define ITFID_ICmpPLCnext				0x000000A6

/* The following block contains system interfaces */
#define	ITFID_ISysCom					0x00000100
#define	ITFID_ISysCpuHandling			0x00000101
#define	ITFID_ISysEvent					0x00000102
#define	ITFID_ISysExcept				0x00000103
#define	ITFID_ISysFile					0x00000104
#define	ITFID_ISysGraphic				0x00000105
#define	ITFID_ISysInt					0x00000106
#define	ITFID_ISysInternalLib			0x00000107
#define	ITFID_ISysMem					0x00000108
#define	ITFID_ISysModule				0x00000109
#define	ITFID_ISysMsgQ					0x0000010A
#define	ITFID_ISysOut					0x0000010B
#define	ITFID_ISysPci					0x0000010C
#define	ITFID_ISysPort					0x0000010D
#define	ITFID_ISysProcess				0x0000010E
#define	ITFID_ISysSem					0x0000010F
#define	ITFID_ISysShm					0x00000110
#define	ITFID_ISysSocket				0x00000111
#define	ITFID_ISysTarget				0x00000112
#define	ITFID_ISysTargetVisu			0x00000113		/* Obsolete, no longer supported */
#define	ITFID_ISysTask					0x00000114
#define	ITFID_ISysTime					0x00000115
#define	ITFID_ISysTimer					0x00000116
#define	ITFID_ISysWindow				0x00000117
#define	ITFID_ISysSafetyInterface		0x00000118
#define	ITFID_ISysSemProcess			0x00000119
#define	ITFID_ISysWindowFileDialog		0x0000011A
#define	ITFID_ISysDir					0x0000011B
#define	ITFID_ISysEthernet				0x0000011C
#define ITFID_ISysFlash					0x0000011D
#define ITFID_ISysTimeRtc				0x0000011E
#define ITFID_ICmpPlcShell				0x0000011F
#define ITFID_ICmpRedundancy			0x00000120
#define	ITFID_ISysWindowTest			0x00000121
#define	ITFID_ISysWindowNative			0x00000122
#define	ITFID_ISysGraphicFramebuffer	0x00000123
#define ITFID_ISysGraphicNative			0x00000124
#define ITFID_ISysNativeControl			0x00000125
#define	ITFID_ISysGraphicLight			0x00000126
#define ITFID_ISysModRTMain				0x00000127
#define ITFID_ISysModRTSub				0x00000128
#define ITFID_ICmpTls					0x00000129
#define	ITFID_ISysSocket2				0x00000130
#define ITFID_ISysNativeCommonControls			0x00000131
#define ITFID_ISysFileStream			0x00000132
#define ITFID_ISysFramebuffer			0x00000133
#define ITFID_ISysGraphicTestable		0x00000134
#define ITFID_ICmpCrypto                0x00000135
#define ITFID_ISysSemCount				0x00000136
#define ITFID_ISysMutex					0x00000137
#define ITFID_ICmpX509Cert				0x00000138
#define ITFID_ISysCpuMultiCore			0x0000013D
#define ITFID_ISysUserDB 				0x0000013E
#define ITFID_ICmpOpenCV 				0x0000013F
#define ITFID_ISysReadWriteLock			0x00000140
#define ITFID_ISysCpuBreakpoints		0x00000141
/* umy: ITFID_ICmpBACnet was defined in CmpBACnetDep.m4, so I moved it here, but kept the value */
#define ITFID_ICmpBACnet				0x00000400


/*Safety classes*/
#define ITFID_ICmpSafetyIORouter		0x00001F01
#define ITFID_ICmpSafetyProxy			0x00001F02

#define ITFID_ISysSafetyProxy			0x00001F80
#define ITFID_ISysSafetyProxyStarter	0x00001F81
#define ITFID_ISysSafetyIORouter		0x00001F83


/* OEM specific start id */
#define	ITFID_ICmpStartOEM				0x00002000
#define	ITFID_ICmpEndOEM				0x00003FFF

/* CAA specific components */
#define	ITFID_ICmpStartCAA				0x00004000
#define ITFID_ICmpCAACallback			0x00004001
#define ITFID________________			0x00004002
#define ITFID_ICmpCAAMemBlockMan		0x00004003
#define ITFID_ICmpCAACanL2				0x00004004
#define ITFID_ICmpCAACanL2I				0x00004005
#define ITFID_ICmpCAATypes				0x00004006
#define ITFID_ICmpCAAAsyncMan			0x00004007
#define ITFID_ICmpCAAFile				0x00004008
#define ITFID_ICmpCAATick				0x00004009
#define ITFID_ICmpCAATickUtil			0x00004010
#define ITFID_ICmpCAASdoClient			0x00004011
#define ITFID_ICmpCAASerialCom			0x00004012
#define ITFID_ICmpCAADTUtil				0x00004013
#define ITFID_ICmpCAARealTimeClock		0x00004014
#define ITFID_ICmpCAABehaviourModel		0x00004015
#define ITFID_ICmpCAATimer				0x00004016
#define ITFID_ICmpCAASdoServer			0x00004017
#define ITFID_ICmpCAANetBaseServices	0x00004018
#define ITFID_ICmpCAASegBufferMan		0x00004019

/* CAA Can Mini Driver */
#define	ITFID_ICmpStartCMD				0x00005F00
#define ITFID_ICmpMSCCanDrv				0x00005F01
#define ITFID_ICmpSJACanDrv				0x00005F02
#define ITFID_ICmpPUSBCanDrv			0x00005F03
#define ITFID_ICmpTC555CanDrv			0x00005F04
#define ITFID_ICmpTCCanDrv				0x00005F05
#define ITFID_ICmpBFCanDrv				0x00005F06
#define ITFID_ICmpIFICanDrv				0x00005F07
#define ITFID_ICmpArmCanDrv				0x00005F08
#define ITFID_ICmp82527CanDrv			0x00005F09
#define ITFID_ICmpNetXCanDlDrv			0x00005F0A
#define ITFID_ICmpEL6751CanDrv			0x00005F0B
#define ITFID_ICmpEsdCanDrv				0x00005F0C
#define ITFID_ICmpSocketCanDrv			0x00005F0D
#define ITFID_ICmpEMSCanDrv				0x00005F0E
#define ITFID_ICmpPCANBasicDrv			0x00005F0F
#define ITFID_ICmpCANFoxDrv				0x00005F10
#define ITFID_ICmpIxxatCANDrv			0x00005F11

#define	ITFID_ICmpEndCMD				0x00005FFF
#define	ITFID_ICmpEndCAA				0x00005FFF

/* Test and verification Interface IDs*/
#define ITFID_ICmpStartTest			    0x00006000
#define ITFID_ICmpRuntimeTest		    0x00006001

#define ITFID_ICmpEndTest               0x000060FF

typedef int (CDECL *PF_EXPORT_FUNCTIONS)(void);
typedef int (CDECL *PF_IMPORT_FUNCTIONS)(void);
typedef RTS_UI32 (CDECL *PF_GET_VERSION)(void);
typedef RTS_RESULT (CDECL *PF_HOOK_FUNCTION)(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);
/* typedef RTS_RESULT (CDECL *PF_REGISTER_API)(const char *pszAPIName, RTS_VOID_FCTPTR pfAPIFunction, int bExternalLibrary, RTS_UI32 ulSignatureID); */
typedef RTS_RESULT (CDECL *PF_REGISTER_API)(const CMP_EXT_FUNCTION_REF *pExpTable, RTS_UINTPTR dummy, int bExternalLibrary, RTS_UI32 cmpId);
typedef RTS_RESULT (CDECL *PF_GET_API)(char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, RTS_UI32 ulSignatureID);
typedef RTS_RESULT (CDECL *PF_REGISTER_API2)(const char *pszAPIName, RTS_VOID_FCTPTR pfAPIFunction, int bExternalLibrary, RTS_UI32 ulSignatureID, RTS_UI32 ulVersion);
typedef RTS_RESULT (CDECL *PF_GET_API2)(char *pszAPIName, RTS_VOID_FCTPTR *ppfAPIFunction, int bExternalLibrary, RTS_UI32 ulSignatureID, RTS_UI32 ulVersion);
typedef RTS_RESULT (CDECL *PF_CALL_HOOK)(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2, int bReverse);
typedef IBase* (CDECL *PF_CREATEINSTANCE)(CLASSID cid, RTS_RESULT *pResult);
typedef RTS_RESULT (CDECL *PF_DELETEINSTANCE)(IBase *pIBase);
typedef RTS_HANDLE (CDECL *PF_REGISTER_CLASS)(CMPID CmpId, CLASSID ClassId);
typedef RTS_RESULT (CDECL *PF_DELETEINSTANCE2)(CLASSID ClassId, IBase *pIBase);

typedef struct tagINIT_STRUCT
{
	/* Exported by Component */
	CMPID CmpId;
#ifdef TRG_64BIT
	RTS_UI32 aligndummy;
#endif
	PF_EXPORT_FUNCTIONS pfExportFunctions;
	PF_IMPORT_FUNCTIONS pfImportFunctions;
	PF_GET_VERSION pfGetVersion;
	PF_CREATEINSTANCE pfCreateInstance;
	PF_DELETEINSTANCE pfDeleteInstance;
	PF_HOOK_FUNCTION pfHookFunction;
	
	/* Import from CM */
	PF_REGISTER_API pfCMRegisterAPI;
	PF_GET_API pfCMGetAPI;
	PF_CALL_HOOK pfCMCallHook;
	PF_REGISTER_CLASS pfCMRegisterClass;
	PF_CREATEINSTANCE pfCMCreateInstance;
	PF_REGISTER_API2 pfCMRegisterAPI2;
	PF_GET_API2 pfCMGetAPI2;
	PF_DELETEINSTANCE2 pfCMDeleteInstance2;
} INIT_STRUCT;

#define COMPONENT_ENTRY_NAME	"ComponentEntry"

#if defined(CPLUSPLUS) && defined(__cplusplus)
extern "C" {
#endif

#if !defined(STATIC_LINK) && !defined(MIXED_LINK) && !defined(DYNAMIC_LINK) && !defined(CPLUSPLUS_STATIC_LINK)
	DLL_DECL int CDECL ComponentEntry(INIT_STRUCT *pInitStruct);
#endif
typedef int (CDECL *PF_COMPONENT_ENTRY)(INIT_STRUCT *pInitStruct);

#if defined(CPLUSPLUS) && defined(__cplusplus)
}
#endif


/* Functions exported from main */
RTS_RESULT CDECL MainLoadComponent(char *pszName, char *pszPath, RTS_HANDLE *phModule, PF_COMPONENT_ENTRY* ppfComponentEntry);
RTS_RESULT CDECL MainUnloadComponent(char *pszName, RTS_HANDLE hModule);


/* The following functions are always offered by the component manager */
int CDECL MainExitLoop(RTS_UINTPTR ulPar);
typedef int (CDECL *PFMAINEXITLOOP)(RTS_UINTPTR ulPar);
#ifdef STATIC_LINK
	#define USE_CMExitLoop		extern int CDECL MainExitLoop(RTS_UINTPTR ulPar);
	#define EXT_CMExitLoop		extern int CDECL MainExitLoop(RTS_UINTPTR ulPar);
	#define GET_CMExitLoop		ERR_OK
	#define CAL_CMExitLoop		MainExitLoop
	#define CHK_CMExitLoop		TRUE
	#define EXP_CMExitLoop		TRUE
#else /* DYNAMIC_LINK */
	#define USE_CMExitLoop		PFMAINEXITLOOP pfMainExitLoop;
	#define EXT_CMExitLoop		extern PFMAINEXITLOOP pfMainExitLoop;
	#define GET_CMExitLoop		s_pfGetAPI("CMExitLoop", (void **)&pfMainExitLoop, 0)
	#define CAL_CMExitLoop		pfMainExitLoop
	#define CHK_CMExitLoop		(pfMainExitLoop != NULL)
	#define EXP_CMExitLoop		s_pfRegisterAPI("CMExitLoop", MainExitLoop, 1, 0)
#endif

/**
 * <description>Hook definitions to inform all components with a broadcast via HookFunction() ([0..10000] reserved for core components):
 * <ul>
 *		<li>CH_INIT_XXX Hooks: Used for initialization of all components.
 *							Future range, not yet defined! = [1..1000]</li>
 *		<li>CH_EXIT_XXX Hooks: Used for deinitialization of all components.
 *							Future range, not yet defined! = [2000..3000]</li>
 *		<li>CH_XXX Hooks: Used for broadcast commands (e.g. cyclic operations).
 *							Future range, not yet defined! = [4000..5000]</li>
 * </ul>
 *	IMPLEMENTATION NOTE:
 *	Future hooks should be defined in the appropriate range above!
 * </description>
 */

/*
	 CH_INIT_XXX Hooks: Used for initialization of all components.
 */
#define CH_INIT_SYSTEM						1
/*	First call at system startup to initialize all system components.
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_INIT_SYSTEM2						8
/*	Second call at system startup to initialize all system components.
    (used to resolve dependencies between core components)
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_INIT								2
/*	First call at system startup. Compontents should initialize all local variables.
	ulParam1: Pointer to int. Parameter name=pbDontCallInitHooksFromCMInit. Content can be set as followed:
			0 [Default]: CMInitEnd() is called at the end of CMInit().
			1: CMInitEnd (calling hooks from CH_INIT2 to CH_INIT_COMM) can be called asynchronously by your own
			   component and is not called in the context of CMInit()!
	ulParam2: Not used
*/

#define CH_INIT2							3
/*	Called after CH_INIT. All components are initialized. Other components can be used.

	NOTE:
	Events must be created of the provider in this init step!

	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_INIT_DONE						CH_INIT2
/* Backward compatibility */

#define CH_PRE_INIT3						7
/*	Called after CH_INIT2 and before CH_INIT3. All components are initialized. Other components can be used.
	ulParam1: Not used
	ulParam2: Not used
*/
#define CH_INIT201							CH_PRE_INIT3
/* Backward compatibility */

#define CH_INIT3							4
/*	Called after CH_INIT201. All components are initialized. Other components can be used.

	NOTE:
	Event callbacks must be registered by the consumer in this init step! Events are created in CH_INIT2.

	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_INIT_SYSTEM_TASKS				500
/*	Called after CH_INIT3. Tasks can be started that are relevant for the IEC applications.
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_INIT_TASKS						5
/*	Called after CH_INIT_SYSTEM_TASKS. All components should start their tasks.

	NOTE:
	The IEC bootprojects are loaded here!

	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_INIT_COMM						6
/*	Called after CH_INIT_TASKS. Communication can be started at this point, because all
	components are ready and running.
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_INIT_FINISHED					1000
/*	Called at the end of the complete initialization sequence. This is called after CH_INIT_COMM and right before the runtime system
	fall into the cyclic CH_COMM_CYCLE state.

	NOTE:
	The IEC bootprojects are started here!

	ulParam1: Not used
	ulParam2: Not used
*/

/*
	CH_EXIT_XXX Hooks: Used for deinitialization of all components.
*/
#define CH_EXIT_COMM						10
/*	Called at the beginning of the shutdown sequence to disable all communication channels.
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_EXIT_TASKS						11
/*	Called before CH_EXIT_SYSTEM_TASKS to stop and remove all tasks that are not needed by the IEC application.

	NOTE:
	IEC applications are unloaded here.

	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_EXIT_SYSTEM_TASKS				2200
/*	Called before CH_EXIT3 to stop and remove all running tasks, because at CH_EXIT3 all objects are released and
	so no task must run after this point.
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_EXIT3							12
/*	Called before CH_EXIT2 to store data (e.g. retain data).
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_EXIT2							13
/*	Called before CH_EXIT to store data (e.g. retain data).
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_PRE_EXIT							CH_EXIT2
/* Backward compatibility */

#define CH_EXIT								14
/*	Here all memory of the components should be released.
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_EXIT_SYSTEM2						17
/*	Counter part to CH_INIT_SYSTEM2.
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_EXIT_SYSTEM						15
/*	Called at the end of the shutdown process for the system components to release memory.
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_EXIT_GARBAGEMEMORY				16
/*	This is the final call for the garbage collector to check, if heap memory was not released.
	ulParam1: Not used
	ulParam2: Not used
*/

/*
	CH_XXX Hooks: Used for broadcast commands (e.g. cyclic operations). 
 */
#define CH_COMM_CYCLE						20
/*	Called every main cycle period. Can be used for background executions.
	ulParam1: Not used
	ulParam2: Not used
*/

#define CH_ON_IMPORT						23
/*	Called for components to import new functions from a new loaded component
	ulParam1: char *: Pointer to component name
	ulParam2: Not used
*/

#define CH_ON_IMPORT_RELEASE				24
/*	Called for components to release imported functions from a component that will be unloaded
	ulParam1: char *: Pointer to component name
	ulParam2: Not used
*/

#define CH_ON_LOAD_COMPONENT				25
/*	Called after loading a new component
	ulParam1: char *: Pointer to component name
	ulParam2: Not used
*/

#define CH_ON_UNLOAD_COMPONENT				26
/*	Called before unloading a component
	ulParam1: char *: Pointer to component name
	ulParam2: Not used
*/

#define CH_ON_REGISTER_INSTANCE				27
/*	Called after register a new instance
	ulParam1: CLASSID: ClassId of instance
	ulParam2: IBase *: Pointer to IBase
*/

#define CH_ON_UNREGISTER_INSTANCE			28
/*	Called before unregister a new instance
	ulParam1: CLASSID: ClassId of instance
	ulParam2: IBase *: Pointer to IBase
*/

#define CH_ON_LICENSE_FCTS_REGISTERED		29
/*
	Called after the functionpointers from the license-library 
	have been registered to the runtime.
*/

#define CH_ON_LICENSE_FCTS_LEGACY_REGISTERED 30
/*
	Called after the functionpointers from the license-library 
	have been registered to the runtime.
*/

#define CH_SAFEMODE							4000
/*
	Called when a component fails to resolve its required import functions during startup of the runtime system. In this case, the system is not stable any more.
	Every component should check safety.
	After entering the safemode, at least the communication should work and so for example a service engineer can investigate the reason for this error.
	The logger should work too.

	IMPORTANT NOTE:
	It is important, that the IEC-application(s) are not loaded or at least are not started after entering the safemode! This is still implemented in the CmpApp component.
	
	ulParam1: Not used
	ulParam2: Not used
*/


/**
 * <category>Single import of interface functions</category>
 * <description>
 *	These macros are used by the System-Components to import single interface functions without modifying
 *	the global SysXXXDep.m4 files.
 *
 *	ATTENTION:
 *	With usage of these macros, the RtsConfiguration cannot resolve this dependency anymore!!!
 *	So use these macros only in Sys-Components and not in standard or customer components!
 *
 *	Usage:
 *		#include "<Interface>Itf.h"
 *		USE_SINGLE_ITF(<Interface>) or USE_SINGLE_ITF(<Class>, <Interface>)
 *		USE_SINGLE_FUNCTION(<Function>)
 *	
 *		static int CDECL ImportFunctions(void)
 *		{
 *			IMPORT_SINGLE_ITF(<Interface>);
 *			IMPORT_SINGLE_FUNCTION(<Function>);
 *			return ERR_OK;
 *		}
 * </description>
 */
#ifdef CPLUSPLUS
	#define USE_SINGLE_ITF(Interface_)				ITF_##Interface_

	#define IMPORT_SINGLE_ITF(Interface_) \
	{\
		IBase *pIBase_;\
		RTS_RESULT Result_;\
		if (pI##Interface_ == NULL && s_pfCMCreateInstance != NULL) \
		{ \
			pIBase_ = (IBase *)s_pfCMCreateInstance(CLASSID_C##Interface_, &Result_); \
			if (pIBase_ != NULL) \
			{\
				pI##Interface_ = (I##Interface_ *)pIBase_->QueryInterface(pIBase_, ITFID_I##Interface_, &Result_); \
				pIBase_->Release(pIBase_); \
			}\
		} \
	}

	#define EXIT_SINGLE_ITF(Interface_) \
	{\
        IBase *pIBase_;\
        RTS_RESULT exitResult_;\
        if (pI##Interface_ != NULL) \
        { \
            pIBase_ = (IBase *)pI##Interface_->QueryInterface(pI##Interface_, ITFID_IBase, &exitResult_); \
            if (pIBase_ != NULL && exitResult_ == ERR_OK) \
            { \
                 pIBase_->Release(pIBase_); \
                 if (pIBase_->Release(pIBase_) == 0) /* The object will be deleted here! */ \
                    pI##Interface_ = NULL; \
            } \
        } \
	}

	#define IMPORT_SINGLE_ITF2(Class_, Interface_) \
	{\
		IBase *pIBase_;\
		RTS_RESULT Result_;\
		if (pI##Interface_ == NULL && s_pfCMCreateInstance != NULL) \
		{ \
			pIBase_ = (IBase *)s_pfCMCreateInstance(CLASSID_C##Class_, &Result_); \
			if (pIBase_ != NULL) \
			{\
				pI##Interface_ = (I##Interface_ *)pIBase_->QueryInterface(pIBase_, ITFID_I##Interface_, &Result_); \
				pIBase_->Release(pIBase_); \
			}\
		} \
	}

	#define EXIT_SINGLE_ITF2(Class_, Interface_) \
	{\
        IBase *pIBase_;\
        RTS_RESULT exitResult_;\
        if (pI##Interface_ != NULL) \
        { \
            pIBase_ = (IBase *)pI##Interface_->QueryInterface(pI##Interface_, ITFID_IBase, &exitResult_); \
            if (pIBase_ != NULL && exitResult_ == ERR_OK) \
            { \
                 pIBase_->Release(pIBase_); \
                 if (pIBase_->Release(pIBase_) == 0) /* The object will be deleted here! */ \
                    pI##Interface_ = NULL; \
            } \
        } \
	}
#else
	#define USE_SINGLE_ITF(Interface_)
	#define IMPORT_SINGLE_ITF(Interface_)
	#define EXIT_SINGLE_ITF(Interface_)
	#define IMPORT_SINGLE_ITF2(Class_, Interface_)
	#define EXIT_SINGLE_ITF2(Class_, Interface_)
#endif

#define USE_SINGLE_FUNCTION(Function)			USE_##Function
#define IMPORT_SINGLE_FUNCTION(Function)		GET_##Function(0)
#define EXIT_SINGLE_FUNCTION(Function)


#if defined(__cplusplus)
}
#endif 

#endif	/*__CMPITF_H__*/
