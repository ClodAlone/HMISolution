/***********************************************************************
 * DO NOT MODIFY!
 * This is a generated file. Do not modify it's contents directly
 ***********************************************************************/

/**
 *  <name>Component PLCHandler</name>
 *  <description> 
 *  An example on how to implement a component.
 *  This component does no usefull work and it exports no functions
 *  which are intended to be used for anything. Use at your own risk.
 *  </description>
 *  <copyright>
 *  Copyright CODESYS Development GmbH
 *  </copyright>
 */
#ifndef _CMPPLCHANDLERDEP_H_
#define _CMPPLCHANDLERDEP_H_

#define COMPONENT_NAME "CmpPLCHandler" COMPONENT_NAME_POSTFIX
#define COMPONENT_ID    ADDVENDORID(CMP_VENDORID, CMPID_CmpPLCHandler)
#define COMPONENT_NAME_UNQUOTED CmpPLCHandler








    

/* Version is defined in an extra header file. */
#include "PLCHandlerVersion.h"

#define CMP_VENDORID       RTS_VENDORID_3S

#ifndef WIN32_RESOURCES

#include "CmpLogItf.h"
#include "CMUtilsItf.h"







/**
 * \file CmpPLCHandlerItf.h
 */
#include "CmpPLCHandlerItf.h"







#ifdef __cplusplus
	extern "C" {
#endif

#ifndef CMPLPLCHANDLERMAIN_NOTIMPLEMENTED
	RTS_RESULT CDECL SetSettingsFileName(char *pszSettingsFile);
	RTS_RESULT CDECL InitComponents(void);
	RTS_RESULT CDECL ExitComponents(int bFinalShutDown);
	RTS_RESULT CDECL ExitComponentsFinal(void);
#endif

RTS_RESULT CDECL PLCHandlerGetInitResult(void);
void CDECL PLCHandlerSetInitResult(RTS_RESULT NewResult);
RTS_RESULT CDECL PLCHRegisterHookFunction(PF_HOOK_FUNCTION pfHookFunction);

#ifdef __cplusplus
	}
#endif

/* Disable external library interface by default. If the interface is needed set the CMPPLCHANDLER_ENABLE_EXTREF macro in your build */
#ifndef CMPPLCHANDLER_ENABLE_EXTREF
    #ifndef CMPPLCHANDLER_DISABLE_EXTREF
        #define CMPPLCHANDLER_DISABLE_EXTREF
    #endif
#endif 

/* Disable component interface by default. If the interface is needed set the CMPPLCHANDLER_ENABLE_CMPITF macro in your build */
#ifndef CMPPLCHANDLER_ENABLE_CMPITF
    #ifndef CMPPLCHANDLER_DISABLE_CMPITF
        #define CMPPLCHANDLER_DISABLE_CMPITF
    #endif
#endif 

#include "SysFileItf.h"


#include "SysModuleItf.h"


/*Obsolete include: CMUtilsItf.m4*/

	
#include "CMItf.h"


#include "SysEventItf.h"


#include "SysTaskItf.h"


#include "SysSemItf.h"


#include "SysDirItf.h"


#include "SysTimeItf.h"


#include "SysTimeRtcItf.h"


#include "CmpXMLParserItf.h"


#include "CmpBinTagUtilItf.h"


#include "CmpGwClientItf.h"


#include "SysMemItf.h"


#include "CmpCommunicationLibItf.h"


#include "SysComItf.h"


#include "SysSocketItf.h"


#include "CmpChannelClientItf.h"


#include "CmpNameServiceClientItf.h"


#include "SysCpuHandlingItf.h"


#include "CmpMemPoolItf.h"


#include "CmpTlsItf.h"


#include "CmpX509CertItf.h"


#include "CmpSecureChannelItf.h"

;
#include "CmpCryptoItf.h"

;

#include "CmpUserMgrItf.h"
#include "CmpSrvItf.h"
#include "CmpSessionInformationItf.h"

        


















































































































































































     


















































































































































































     


        


















































      


















































      



#ifdef CPLUSPLUS
    #define INIT_STMT \
    {\
        IBase *pIBase;\
        RTS_RESULT initResult;\
        if (pICmpLog == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpLog, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpLog = (ICmpLog *)pIBase->QueryInterface(pIBase, ITFID_ICmpLog, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
        if (pICMUtils == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCMUtils, &initResult); \
            if (pIBase != NULL) \
            { \
                pICMUtils = (ICMUtils *)pIBase->QueryInterface(pIBase, ITFID_ICMUtils, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
        if (pICmpCrypto == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpCrypto, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpCrypto = (ICmpCrypto *)pIBase->QueryInterface(pIBase, ITFID_ICmpCrypto, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpSecureChannel == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpSecureChannel, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpSecureChannel = (ICmpSecureChannel *)pIBase->QueryInterface(pIBase, ITFID_ICmpSecureChannel, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpX509Cert == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpX509Cert, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpX509Cert = (ICmpX509Cert *)pIBase->QueryInterface(pIBase, ITFID_ICmpX509Cert, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpTls == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpTls, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpTls = (ICmpTls *)pIBase->QueryInterface(pIBase, ITFID_ICmpTls, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpMemPool == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpMemPool, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpMemPool = (ICmpMemPool *)pIBase->QueryInterface(pIBase, ITFID_ICmpMemPool, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysCpuHandling == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysCpuHandling, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysCpuHandling = (ISysCpuHandling *)pIBase->QueryInterface(pIBase, ITFID_ISysCpuHandling, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpNameServiceClient == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpNameServiceClient, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpNameServiceClient = (ICmpNameServiceClient *)pIBase->QueryInterface(pIBase, ITFID_ICmpNameServiceClient, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpChannelClient == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpChannelClient, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpChannelClient = (ICmpChannelClient *)pIBase->QueryInterface(pIBase, ITFID_ICmpChannelClient, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysSocket == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysSocket, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysSocket = (ISysSocket *)pIBase->QueryInterface(pIBase, ITFID_ISysSocket, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysCom == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysCom, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysCom = (ISysCom *)pIBase->QueryInterface(pIBase, ITFID_ISysCom, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpCommunicationLib == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpCommunicationLib, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpCommunicationLib = (ICmpCommunicationLib *)pIBase->QueryInterface(pIBase, ITFID_ICmpCommunicationLib, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysMem == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysMem, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysMem = (ISysMem *)pIBase->QueryInterface(pIBase, ITFID_ISysMem, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpGwClient == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpGwClient, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpGwClient = (ICmpGwClient *)pIBase->QueryInterface(pIBase, ITFID_ICmpGwClient, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpBinTagUtil == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpBinTagUtil, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpBinTagUtil = (ICmpBinTagUtil *)pIBase->QueryInterface(pIBase, ITFID_ICmpBinTagUtil, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICmpXMLParser == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCmpXMLParser, &initResult); \
            if (pIBase != NULL) \
            { \
                pICmpXMLParser = (ICmpXMLParser *)pIBase->QueryInterface(pIBase, ITFID_ICmpXMLParser, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysTimeRtc == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysTimeRtc, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysTimeRtc = (ISysTimeRtc *)pIBase->QueryInterface(pIBase, ITFID_ISysTimeRtc, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysTime == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysTime, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysTime = (ISysTime *)pIBase->QueryInterface(pIBase, ITFID_ISysTime, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysDir == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysDir, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysDir = (ISysDir *)pIBase->QueryInterface(pIBase, ITFID_ISysDir, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysSem == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysSem, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysSem = (ISysSem *)pIBase->QueryInterface(pIBase, ITFID_ISysSem, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysTask == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysTask, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysTask = (ISysTask *)pIBase->QueryInterface(pIBase, ITFID_ISysTask, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysEvent == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysEvent, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysEvent = (ISysEvent *)pIBase->QueryInterface(pIBase, ITFID_ISysEvent, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pICM == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CCM, &initResult); \
            if (pIBase != NULL) \
            { \
                pICM = (ICM *)pIBase->QueryInterface(pIBase, ITFID_ICM, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          /*Obsolete include CMUtils*/ \
		  if (pISysModule == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysModule, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysModule = (ISysModule *)pIBase->QueryInterface(pIBase, ITFID_ISysModule, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
          if (pISysFile == NULL && s_pfCMCreateInstance != NULL) \
        { \
            pIBase = (IBase *)s_pfCMCreateInstance(CLASSID_CSysFile, &initResult); \
            if (pIBase != NULL) \
            { \
                pISysFile = (ISysFile *)pIBase->QueryInterface(pIBase, ITFID_ISysFile, &initResult); \
                pIBase->Release(pIBase); \
            } \
        } \
           \
    }
    #define INIT_LOCALS_STMT \
    {\
        pICmpLog = NULL; \
        pICMUtils = NULL; \
        pICmpCrypto = NULL; \
          pICmpSecureChannel = NULL; \
          pICmpX509Cert = NULL; \
          pICmpTls = NULL; \
          pICmpMemPool = NULL; \
          pISysCpuHandling = NULL; \
          pICmpNameServiceClient = NULL; \
          pICmpChannelClient = NULL; \
          pISysSocket = NULL; \
          pISysCom = NULL; \
          pICmpCommunicationLib = NULL; \
          pISysMem = NULL; \
          pICmpGwClient = NULL; \
          pICmpBinTagUtil = NULL; \
          pICmpXMLParser = NULL; \
          pISysTimeRtc = NULL; \
          pISysTime = NULL; \
          pISysDir = NULL; \
          pISysSem = NULL; \
          pISysTask = NULL; \
          pISysEvent = NULL; \
          pICM = NULL; \
          /*Obsolete include CMUtils*/ \
		  pISysModule = NULL; \
          pISysFile = NULL; \
           \
    }
    #define EXIT_STMT \
    {\
        IBase *pIBase;\
        RTS_RESULT exitResult;\
        if (pICmpLog != NULL) \
        { \
            pIBase = (IBase *)pICmpLog->QueryInterface(pICmpLog, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpLog = NULL; \
            } \
        } \
        if (pICMUtils != NULL) \
        { \
            pIBase = (IBase *)pICMUtils->QueryInterface(pICMUtils, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICMUtils = NULL; \
            } \
        } \
        if (pICmpCrypto != NULL) \
        { \
            pIBase = (IBase *)pICmpCrypto->QueryInterface(pICmpCrypto, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpCrypto = NULL; \
            } \
        } \
          if (pICmpSecureChannel != NULL) \
        { \
            pIBase = (IBase *)pICmpSecureChannel->QueryInterface(pICmpSecureChannel, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpSecureChannel = NULL; \
            } \
        } \
          if (pICmpX509Cert != NULL) \
        { \
            pIBase = (IBase *)pICmpX509Cert->QueryInterface(pICmpX509Cert, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpX509Cert = NULL; \
            } \
        } \
          if (pICmpTls != NULL) \
        { \
            pIBase = (IBase *)pICmpTls->QueryInterface(pICmpTls, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpTls = NULL; \
            } \
        } \
          if (pICmpMemPool != NULL) \
        { \
            pIBase = (IBase *)pICmpMemPool->QueryInterface(pICmpMemPool, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpMemPool = NULL; \
            } \
        } \
          if (pISysCpuHandling != NULL) \
        { \
            pIBase = (IBase *)pISysCpuHandling->QueryInterface(pISysCpuHandling, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysCpuHandling = NULL; \
            } \
        } \
          if (pICmpNameServiceClient != NULL) \
        { \
            pIBase = (IBase *)pICmpNameServiceClient->QueryInterface(pICmpNameServiceClient, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpNameServiceClient = NULL; \
            } \
        } \
          if (pICmpChannelClient != NULL) \
        { \
            pIBase = (IBase *)pICmpChannelClient->QueryInterface(pICmpChannelClient, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpChannelClient = NULL; \
            } \
        } \
          if (pISysSocket != NULL) \
        { \
            pIBase = (IBase *)pISysSocket->QueryInterface(pISysSocket, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysSocket = NULL; \
            } \
        } \
          if (pISysCom != NULL) \
        { \
            pIBase = (IBase *)pISysCom->QueryInterface(pISysCom, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysCom = NULL; \
            } \
        } \
          if (pICmpCommunicationLib != NULL) \
        { \
            pIBase = (IBase *)pICmpCommunicationLib->QueryInterface(pICmpCommunicationLib, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpCommunicationLib = NULL; \
            } \
        } \
          if (pISysMem != NULL) \
        { \
            pIBase = (IBase *)pISysMem->QueryInterface(pISysMem, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysMem = NULL; \
            } \
        } \
          if (pICmpGwClient != NULL) \
        { \
            pIBase = (IBase *)pICmpGwClient->QueryInterface(pICmpGwClient, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpGwClient = NULL; \
            } \
        } \
          if (pICmpBinTagUtil != NULL) \
        { \
            pIBase = (IBase *)pICmpBinTagUtil->QueryInterface(pICmpBinTagUtil, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpBinTagUtil = NULL; \
            } \
        } \
          if (pICmpXMLParser != NULL) \
        { \
            pIBase = (IBase *)pICmpXMLParser->QueryInterface(pICmpXMLParser, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICmpXMLParser = NULL; \
            } \
        } \
          if (pISysTimeRtc != NULL) \
        { \
            pIBase = (IBase *)pISysTimeRtc->QueryInterface(pISysTimeRtc, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysTimeRtc = NULL; \
            } \
        } \
          if (pISysTime != NULL) \
        { \
            pIBase = (IBase *)pISysTime->QueryInterface(pISysTime, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysTime = NULL; \
            } \
        } \
          if (pISysDir != NULL) \
        { \
            pIBase = (IBase *)pISysDir->QueryInterface(pISysDir, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysDir = NULL; \
            } \
        } \
          if (pISysSem != NULL) \
        { \
            pIBase = (IBase *)pISysSem->QueryInterface(pISysSem, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysSem = NULL; \
            } \
        } \
          if (pISysTask != NULL) \
        { \
            pIBase = (IBase *)pISysTask->QueryInterface(pISysTask, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysTask = NULL; \
            } \
        } \
          if (pISysEvent != NULL) \
        { \
            pIBase = (IBase *)pISysEvent->QueryInterface(pISysEvent, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysEvent = NULL; \
            } \
        } \
          if (pICM != NULL) \
        { \
            pIBase = (IBase *)pICM->QueryInterface(pICM, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pICM = NULL; \
            } \
        } \
          /*Obsolete include CMUtils*/ \
		  if (pISysModule != NULL) \
        { \
            pIBase = (IBase *)pISysModule->QueryInterface(pISysModule, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysModule = NULL; \
            } \
        } \
          if (pISysFile != NULL) \
        { \
            pIBase = (IBase *)pISysFile->QueryInterface(pISysFile, ITFID_IBase, &exitResult); \
            if (pIBase != NULL) \
            { \
                 pIBase->Release(pIBase); \
                 if (pIBase->Release(pIBase) == 0) /* The object will be deleted here! */ \
                    pISysFile = NULL; \
            } \
        } \
           \
    }
#else
    #define INIT_STMT
    #define INIT_LOCALS_STMT
    #define EXIT_STMT
#endif



#if defined(STATIC_LINK)
    #define IMPORT_STMT
#else
    #define IMPORT_STMT \
    {\
        RTS_RESULT importResult = ERR_OK;\
        RTS_RESULT TempResult = ERR_OK;\
        INIT_STMT   \
        TempResult = GET_LogAdd(CM_IMPORT_OPTIONAL_FUNCTION); \
        TempResult = GET_CMUtlMemCpy(CM_IMPORT_OPTIONAL_FUNCTION); \
        if (ERR_OK == importResult ) TempResult = GET_RtsOIDGetName(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoSymmetricEncrypt(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoKeyInit(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoKeyAgreement(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoImportRawAsymmetricKey(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoExportRawAsymmetricKey(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoGenerateAsymmetricKeyPair(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoGenerateRandomNumber(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoKeyExit(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoAsymmetricEncrypt(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoImportAsymmetricKey(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoGetAlgorithmById(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoRtsByteStringExit(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoRtsByteStringInit3(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoRtsByteStringInit2(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_CryptoRtsByteStringInit(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertGetBinary(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertClose(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertStoreRemoveCert(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertStoreAddCert(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509ParseCertificate(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertGetThumbprint(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertGetContent(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertInfoExit(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertInfoInit(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertStoreGetNextCert(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertStoreGetFirstCert(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertStoreClose(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_X509CertStoreOpen(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_TlsFreeContext(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_TlsCreateContext3(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_SecChGetSecureChannelSize(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_SecChClientCleanup(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_SecChClientInit(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_SecChClientDataReceived(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_SecChClientSendData(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientEndResolveBlockDriverAddress(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientBeginResolveBlockDriverAddress3(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientEndIncrementalResolveAllNodes(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientBeginIncrementalResolveAllNodes3(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientEndIncrementalResolveName(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientBeginIncrementalResolveName3(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientBeginCloseChannel(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientBeginOpenChannel2(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientEndSendRequest(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientBeginSendRequest(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientDisconnectFromGateway(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientEndConnectToGateway(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientBeginConnectToGateway2(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientEnumCommDrivers2(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) TempResult = GET_GWClientGetInterfaceVersion(CM_IMPORT_OPTIONAL_FUNCTION);\
          if (ERR_OK == importResult ) importResult = GET_MemPoolUnlock(0);\
          if (ERR_OK == importResult ) importResult = GET_MemPoolLock(0);\
          if (ERR_OK == importResult ) importResult = GET_MemPoolPutBlock(0);\
          if (ERR_OK == importResult ) importResult = GET_MemPoolRemoveUsedBlock(0);\
          if (ERR_OK == importResult ) importResult = GET_MemPoolAppendUsedBlock(0);\
          if (ERR_OK == importResult ) importResult = GET_MemPoolGetBlock2(0);\
          if (ERR_OK == importResult ) importResult = GET_MemPoolDelete(0);\
          if (ERR_OK == importResult ) importResult = GET_MemPoolCreateStatic(0);\
          if (ERR_OK == importResult ) importResult = GET_SysCpuAtomicAdd(0);\
          if (ERR_OK == importResult ) importResult = GET_SysCpuTestAndSetBit(0);\
          if (ERR_OK == importResult ) importResult = GET_NSResolveAll3(0);\
          if (ERR_OK == importResult ) importResult = GET_NSResolveBlkDrvAddress3(0);\
          if (ERR_OK == importResult ) importResult = GET_NSResolveName3(0);\
          if (ERR_OK == importResult ) importResult = GET_NetClientSend(0);\
          if (ERR_OK == importResult ) importResult = GET_NetClientUnregisterAppInterface(0);\
          if (ERR_OK == importResult ) importResult = GET_NetClientRegisterAppInterface(0);\
          if (ERR_OK == importResult ) importResult = GET_NetClientCloseChannel(0);\
          if (ERR_OK == importResult ) importResult = GET_NetClientOpenChannelResult(0);\
          if (ERR_OK == importResult ) importResult = GET_NetClientOpenChannel(0);\
          if (ERR_OK == importResult ) importResult = GET_CMImportFunctions(0);\
          if (ERR_OK == importResult ) importResult = GET_CMGetComponentByName(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUnloadComponent(0);\
          if (ERR_OK == importResult ) importResult = GET_CMExitComponent(0);\
          if (ERR_OK == importResult ) importResult = GET_CMInitComponent(0);\
          if (ERR_OK == importResult ) importResult = GET_CMLoadComponent(0);\
          if (ERR_OK == importResult ) importResult = GET_CMDeleteInstance2(0);\
          if (ERR_OK == importResult ) importResult = GET_CMCreateInstance(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockPing(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockFdIsset(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockFdInit(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockFdZero(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockInetNtoa(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockAccept(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockListen(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockBind(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockSendTo(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockRecvFrom(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockSend(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockRecv(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockSelect(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockConnect(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockNtohl(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockHtonl(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockHtons(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockGetNextAdapterInfo(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockGetFirstAdapterInfo(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockGetHostByName(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockGetHostName(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockInetAddr(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockIoctl(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockSetOption(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockClose(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSockCreate(0);\
          if (ERR_OK == importResult ) importResult = GET_SysComWrite(0);\
          if (ERR_OK == importResult ) importResult = GET_SysComRead(0);\
          if (ERR_OK == importResult ) importResult = GET_SysComPurge(0);\
          if (ERR_OK == importResult ) importResult = GET_SysComSetTimeout(0);\
          if (ERR_OK == importResult ) importResult = GET_SysComClose(0);\
          if (ERR_OK == importResult ) importResult = GET_SysComOpen2(0);\
          if (ERR_OK == importResult ) importResult = GET_SysComOpen(0);\
          if (ERR_OK == importResult ) importResult = GET_SysMemForceSwap(0);\
          if (ERR_OK == importResult ) importResult = GET_SysMemSwap(0);\
          if (ERR_OK == importResult ) importResult = GET_SysMemFreeData(0);\
          if (ERR_OK == importResult ) importResult = GET_SysMemAllocData(0);\
          if (ERR_OK == importResult ) importResult = GET_CopyAddrToBuffer(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderGetFirstTag(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderGetNextTag(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderPeekNext(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderGetString(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderSkipContent(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderGetContent(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderIsDataTag(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderGetTagLen(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderGetTagId(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderMoveNext(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderFileInit(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagReaderInit(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterGetAvailableBuffer(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterRestoreSavepoint(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterCreateSavepoint(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterFinishService(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterStartService(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterFinish(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterEndTag(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterAppendString(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterAppendRaw(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterAppendFillBytes(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterAppendBlob(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterAppendWString(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterStartTag(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterFileStreamInit(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterFileInit(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterInit2(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagWriterInit(0);\
          if (ERR_OK == importResult ) importResult = GET_BTagSwapHeader(0);\
          if (ERR_OK == importResult ) importResult = GET_FreeXMLParser(0);\
          if (ERR_OK == importResult ) importResult = GET_ParseXML2(0);\
          if (ERR_OK == importResult ) importResult = GET_SetXMLCharacterDataHandler(0);\
          if (ERR_OK == importResult ) importResult = GET_SetXMLElementHandler(0);\
          if (ERR_OK == importResult ) importResult = GET_CreateXMLParser3(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTimeRtcGet(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTimeGetMs(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskGetInfo(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskGetCurrentOSHandle(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskGetOSHandle(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskAutoReleaseOnExit(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskSetPriority(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskResume(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskWaitSleep(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskExit(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskSetExit(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskEnd(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskLeave(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskEnter(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskDestroy(0);\
          if (ERR_OK == importResult ) importResult = GET_SysTaskCreate(0);\
          if (ERR_OK == importResult ) importResult = GET_SysEventWait(0);\
          if (ERR_OK == importResult ) importResult = GET_SysEventSet(0);\
          if (ERR_OK == importResult ) importResult = GET_SysEventDelete(0);\
          if (ERR_OK == importResult ) importResult = GET_SysEventCreate(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSemTry(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSemLeave(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSemEnter(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSemDelete(0);\
          if (ERR_OK == importResult ) importResult = GET_SysSemCreate(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlStringToVersion(0);\
          if (ERR_OK == importResult ) importResult = GET_CMStringDelete(0);\
          if (ERR_OK == importResult ) importResult = GET_CMStringCreate(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlStringToGUID(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlGUIDToString(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlWToUtf8(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlsnprintf(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlSafeStrCat(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlSafeStrNCpy(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlSafeStrCpy(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlCWToStr(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlcwstrlen(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlwstrlen(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlwstrcpy(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlcwstrcpy(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlcwstrcmp(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlWToStr(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlStrToW(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlwstrcmp(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlStrNICmp(0);\
          if (ERR_OK == importResult ) importResult = GET_CMUtlStrICmp(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileGetName(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileGenerateCRC3_(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileGenerateCRC2(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileSetPos(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileGetPos(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileEOF(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileDelete(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileRename(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileCopy(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileGetSizeByHandle(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileGetSize_(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileGetSize(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileWrite(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileRead(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileClose_(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileClose(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileOpen_(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileOpen(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileGetPath2(0);\
          if (ERR_OK == importResult ) importResult = GET_SysFileGetPath(0);\
          if (ERR_OK == importResult ) importResult = GET_SysDirCreate_(0);\
          if (ERR_OK == importResult ) importResult = GET_SysDirClose_(0);\
          if (ERR_OK == importResult ) importResult = GET_SysDirOpen_(0);\
          if (ERR_OK == importResult ) importResult = GET_SysModuleGetFunctionPointer(0);\
          if (ERR_OK == importResult ) importResult = GET_SysModuleUnload(0);\
          if (ERR_OK == importResult ) importResult = GET_SysModuleLoad(0);\
          if (ERR_OK == importResult ) importResult = GET_LogAddArg(0);\
          if (ERR_OK == importResult ) importResult = GET_LogRegisterInterface(0);\
          if (ERR_OK == importResult ) importResult = GET_LogDelete(0);\
          if (ERR_OK == importResult ) importResult = GET_LogCreate(0);\
          if (ERR_OK == importResult ) importResult = GET_LogOpen(0);\
          if (ERR_OK == importResult ) importResult = GET_LogSetFilter(0);\
          if (ERR_OK == importResult ) importResult = GET_LogGetFilter(0);\
           \
        /* To make LINT happy */\
        TempResult = TempResult;\
        if (ERR_OK != importResult) return importResult;\
    }
#endif



#ifndef CMPPLCHANDLER_DISABLE_EXTREF
#define EXPORT_EXTREF_STMT \
            { (RTS_VOID_FCTPTR)plchandlervarlistread, "plchandlervarlistread", RTSITF_GET_SIGNATURE(0x5FFAB125, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlervarlistleave, "plchandlervarlistleave", RTSITF_GET_SIGNATURE(0x6150164D, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlervarlistenter, "plchandlervarlistenter", RTSITF_GET_SIGNATURE(0x7F252097, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlervarlistenable, "plchandlervarlistenable", RTSITF_GET_SIGNATURE(0x5EDC0FD8, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlervarlistdisable, "plchandlervarlistdisable", RTSITF_GET_SIGNATURE(0xA9F0A7EB, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlervarlistdelete, "plchandlervarlistdelete", RTSITF_GET_SIGNATURE(0xAD8E5D4F, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlervarlistdefine, "plchandlervarlistdefine", RTSITF_GET_SIGNATURE(0x81C780F4, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlersyncwritevars, "plchandlersyncwritevars", RTSITF_GET_SIGNATURE(0xDACACBAF, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlersyncwritevarlisttoplc, "plchandlersyncwritevarlisttoplc", RTSITF_GET_SIGNATURE(0xA486E098, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlersyncsendservice, "plchandlersyncsendservice", RTSITF_GET_SIGNATURE(0x303EF9A4, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlersyncreadvarsrelease, "plchandlersyncreadvarsrelease", RTSITF_GET_SIGNATURE(0xD4834060, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlersyncreadvars, "plchandlersyncreadvars", RTSITF_GET_SIGNATURE(0x7F4F3550, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlersyncreadvarlistfromplc, "plchandlersyncreadvarlistfromplc", RTSITF_GET_SIGNATURE(0xD0C1A01F, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlersyncreadvarlist, "plchandlersyncreadvarlist", RTSITF_GET_SIGNATURE(0x22BA041E, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlersyncdeletevarlist, "plchandlersyncdeletevarlist", RTSITF_GET_SIGNATURE(0xA82CAF19, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlersyncdefinevarlist, "plchandlersyncdefinevarlist", RTSITF_GET_SIGNATURE(0x9C816C52, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlerlogginginit, "plchandlerlogginginit", RTSITF_GET_SIGNATURE(0xFB1F6D11, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlergetstate, "plchandlergetstate", RTSITF_GET_SIGNATURE(0x3882F552, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlergetdeviceinfo, "plchandlergetdeviceinfo", RTSITF_GET_SIGNATURE(0x9D8B2632, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlerdisconnect, "plchandlerdisconnect", RTSITF_GET_SIGNATURE(0xBEC54443, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlerdeleteinstance, "plchandlerdeleteinstance", RTSITF_GET_SIGNATURE(0x62C4DDDA, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlercreateinstance, "plchandlercreateinstance", RTSITF_GET_SIGNATURE(0xF3B6D6E9, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlerconnect2, "plchandlerconnect2", RTSITF_GET_SIGNATURE(0x229609D6, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlerconnect, "plchandlerconnect", RTSITF_GET_SIGNATURE(0xA16ECE49, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlerconfigurebystring, "plchandlerconfigurebystring", RTSITF_GET_SIGNATURE(0x9A5EB7EA, 0), 0x03051200 },\
          { (RTS_VOID_FCTPTR)plchandlerchecksymbolvalidity, "plchandlerchecksymbolvalidity", RTSITF_GET_SIGNATURE(0x35B50EAA, 0), 0x03051200 },\
          
#else
#define EXPORT_EXTREF_STMT
#endif
#ifndef CMPPLCHANDLER_DISABLE_EXTREF2
#define EXPORT_EXTREF2_STMT \
                                                                
#else
#define EXPORT_EXTREF2_STMT
#endif
#if !defined(CMPPLCHANDLER_DISABLE_CMPITF) && !defined(STATIC_LINK) && !defined(CPLUSPLUS) && !defined(CPLUSPLUS_ONLY)
#define EXPORT_CMPITF_STMT \
    {\
        { (RTS_VOID_FCTPTR)PLCHandlerDeleteInstance, "PLCHandlerDeleteInstance", 0, 0 },\
          { (RTS_VOID_FCTPTR)PLCHandlerCreateInstance, "PLCHandlerCreateInstance", 0, 0 },\
                                                              \
        { ((RTS_VOID_FCTPTR)(void *)0), "", 0, 0 }\
    }
#else
#define EXPORT_CMPITF_STMT \
    {\
        { ((RTS_VOID_FCTPTR)(void *)0), "", 0, 0 }\
    }
#endif
#define EXPORT_CPP_STMT


#if defined(STATIC_LINK)
    #define EXPORT_STMT\
    {\
        RTS_RESULT ExpResult;\
        if (NULL == s_pfCMRegisterAPI)\
            return ERR_NOTINITIALIZED;\
        ExpResult = s_pfCMRegisterAPI(s_ExternalsTable, 0, 1, COMPONENT_ID);\
        if (ERR_OK != ExpResult)\
            return ExpResult;\
    }
#else
    #define EXPORT_STMT\
    {\
        RTS_RESULT ExpResult;\
        if (NULL == s_pfCMRegisterAPI)\
            return ERR_NOTINITIALIZED;\
        ExpResult = s_pfCMRegisterAPI(s_ExternalsTable, 0, 1, COMPONENT_ID);\
        if (ERR_OK != ExpResult)\
            return ExpResult;\
        ExpResult = s_pfCMRegisterAPI(s_ItfTable, 0, 0, COMPONENT_ID);\
        if (ERR_OK != ExpResult)\
            return ExpResult;\
    }
#endif

#define USE_STMT \
    /*lint -save --e{528} --e{551} */ \
    static volatile PF_REGISTER_API s_pfCMRegisterAPI; \
    static volatile PF_REGISTER_API2 s_pfCMRegisterAPI2; \
    static volatile PF_GET_API s_pfCMGetAPI; \
    static volatile PF_GET_API2 s_pfCMGetAPI2; \
    static volatile PF_REGISTER_CLASS s_pfCMRegisterClass; \
    static volatile PF_CREATEINSTANCE s_pfCMCreateInstance; \
    static volatile PF_CALL_HOOK s_pfCMCallHook; \
    static const CMP_EXT_FUNCTION_REF s_ExternalsTable[] =\
    {\
        EXPORT_EXTREF_STMT\
        EXPORT_EXTREF2_STMT\
        { ((RTS_VOID_FCTPTR)(void *)0), "", 0, 0 }\
    };\
    static const CMP_EXT_FUNCTION_REF s_ItfTable[] = EXPORT_CMPITF_STMT; \
    /*lint -restore */  \
    static int CDECL ExportFunctions(void); \
    static int CDECL ImportFunctions(void); \
    static IBase* CDECL CreateInstance(CLASSID cid, RTS_RESULT *pResult); \
    static RTS_RESULT CDECL DeleteInstance(IBase *pIBase); \
    static RTS_UI32 CDECL CmpGetVersion(void); \
    static RTS_RESULT CDECL HookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2); \
    ITF_CmpLog   \
    ITF_CMUtils  \
    USE_CMUtlMemCpy  \
    USE_LogAdd \
	ITF_SysFile     \
	ITF_SysModule     \
	/*obsolete entry ITF_CMUtils*/      \
	ITF_CM     \
	ITF_SysEvent     \
	ITF_SysTask     \
	ITF_SysSem     \
	ITF_SysDir     \
	ITF_SysTime     \
	ITF_SysTimeRtc     \
	ITF_CmpXMLParser     \
	ITF_CmpBinTagUtil     \
	ITF_CmpGwClient     \
	ITF_SysMem     \
	ITF_CmpCommunicationLib     \
	ITF_SysCom     \
	ITF_SysSocket     \
	ITF_CmpChannelClient     \
	ITF_CmpNameServiceClient     \
	ITF_SysCpuHandling     \
	ITF_CmpMemPool     \
	ITF_CmpTls     \
	ITF_CmpX509Cert     \
	ITF_CmpSecureChannel     \
	ITF_CmpCrypto      \
    USE_LogGetFilter      \
    USE_LogSetFilter      \
    USE_LogOpen      \
    USE_LogCreate      \
    USE_LogDelete      \
    USE_LogRegisterInterface      \
    USE_LogAddArg      \
    USE_SysModuleLoad      \
    USE_SysModuleUnload      \
    USE_SysModuleGetFunctionPointer      \
    USE_SysDirOpen_      \
    USE_SysDirClose_      \
    USE_SysDirCreate_      \
    USE_SysFileGetPath      \
    USE_SysFileGetPath2      \
    USE_SysFileOpen      \
    USE_SysFileOpen_      \
    USE_SysFileClose      \
    USE_SysFileClose_      \
    USE_SysFileRead      \
    USE_SysFileWrite      \
    USE_SysFileGetSize      \
    USE_SysFileGetSize_      \
    USE_SysFileGetSizeByHandle      \
    USE_SysFileCopy      \
    USE_SysFileRename      \
    USE_SysFileDelete      \
    USE_SysFileEOF      \
    USE_SysFileGetPos      \
    USE_SysFileSetPos      \
    USE_SysFileGenerateCRC2      \
    USE_SysFileGenerateCRC3_      \
    USE_SysFileGetName      \
    USE_CMUtlStrICmp      \
    USE_CMUtlStrNICmp      \
    USE_CMUtlwstrcmp      \
    USE_CMUtlStrToW      \
    USE_CMUtlWToStr      \
    USE_CMUtlcwstrcmp      \
    USE_CMUtlcwstrcpy      \
    USE_CMUtlwstrcpy      \
    USE_CMUtlwstrlen      \
    USE_CMUtlcwstrlen      \
    USE_CMUtlCWToStr      \
    USE_CMUtlSafeStrCpy      \
    USE_CMUtlSafeStrNCpy      \
    USE_CMUtlSafeStrCat      \
    USE_CMUtlsnprintf      \
    USE_CMUtlWToUtf8      \
    USE_CMUtlGUIDToString      \
    USE_CMUtlStringToGUID      \
    USE_CMStringCreate      \
    USE_CMStringDelete      \
    USE_CMUtlStringToVersion      \
    USE_SysSemCreate      \
    USE_SysSemDelete      \
    USE_SysSemEnter      \
    USE_SysSemLeave      \
    USE_SysSemTry      \
    USE_SysEventCreate      \
    USE_SysEventDelete      \
    USE_SysEventSet      \
    USE_SysEventWait      \
    USE_SysTaskCreate      \
    USE_SysTaskDestroy      \
    USE_SysTaskEnter      \
    USE_SysTaskLeave      \
    USE_SysTaskEnd      \
    USE_SysTaskSetExit      \
    USE_SysTaskExit      \
    USE_SysTaskWaitSleep      \
    USE_SysTaskResume      \
    USE_SysTaskSetPriority      \
    USE_SysTaskAutoReleaseOnExit      \
    USE_SysTaskGetOSHandle      \
    USE_SysTaskGetCurrentOSHandle      \
    USE_SysTaskGetInfo      \
    USE_SysTimeGetMs      \
    USE_SysTimeRtcGet      \
    USE_CreateXMLParser3      \
    USE_SetXMLElementHandler      \
    USE_SetXMLCharacterDataHandler      \
    USE_ParseXML2      \
    USE_FreeXMLParser      \
    USE_BTagSwapHeader      \
    USE_BTagWriterInit      \
    USE_BTagWriterInit2      \
    USE_BTagWriterFileInit      \
    USE_BTagWriterFileStreamInit      \
    USE_BTagWriterStartTag      \
    USE_BTagWriterAppendWString      \
    USE_BTagWriterAppendBlob      \
    USE_BTagWriterAppendFillBytes      \
    USE_BTagWriterAppendRaw      \
    USE_BTagWriterAppendString      \
    USE_BTagWriterEndTag      \
    USE_BTagWriterFinish      \
    USE_BTagWriterStartService      \
    USE_BTagWriterFinishService      \
    USE_BTagWriterCreateSavepoint      \
    USE_BTagWriterRestoreSavepoint      \
    USE_BTagWriterGetAvailableBuffer      \
    USE_BTagReaderInit      \
    USE_BTagReaderFileInit      \
    USE_BTagReaderMoveNext      \
    USE_BTagReaderGetTagId      \
    USE_BTagReaderGetTagLen      \
    USE_BTagReaderIsDataTag      \
    USE_BTagReaderGetContent      \
    USE_BTagReaderSkipContent      \
    USE_BTagReaderGetString      \
    USE_BTagReaderPeekNext      \
    USE_BTagReaderGetNextTag      \
    USE_BTagReaderGetFirstTag      \
    USE_CopyAddrToBuffer      \
    USE_SysMemAllocData      \
    USE_SysMemFreeData      \
    USE_SysMemSwap      \
    USE_SysMemForceSwap      \
    USE_SysComOpen      \
    USE_SysComOpen2      \
    USE_SysComClose      \
    USE_SysComSetTimeout      \
    USE_SysComPurge      \
    USE_SysComRead      \
    USE_SysComWrite      \
    USE_SysSockCreate      \
    USE_SysSockClose      \
    USE_SysSockSetOption      \
    USE_SysSockIoctl      \
    USE_SysSockInetAddr      \
    USE_SysSockGetHostName      \
    USE_SysSockGetHostByName      \
    USE_SysSockGetFirstAdapterInfo      \
    USE_SysSockGetNextAdapterInfo      \
    USE_SysSockHtons      \
    USE_SysSockHtonl      \
    USE_SysSockNtohl      \
    USE_SysSockConnect      \
    USE_SysSockSelect      \
    USE_SysSockRecv      \
    USE_SysSockSend      \
    USE_SysSockRecvFrom      \
    USE_SysSockSendTo      \
    USE_SysSockBind      \
    USE_SysSockListen      \
    USE_SysSockAccept      \
    USE_SysSockInetNtoa      \
    USE_SysSockFdZero      \
    USE_SysSockFdInit      \
    USE_SysSockFdIsset      \
    USE_SysSockPing      \
    USE_CMCreateInstance      \
    USE_CMDeleteInstance2      \
    USE_CMLoadComponent      \
    USE_CMInitComponent      \
    USE_CMExitComponent      \
    USE_CMUnloadComponent      \
    USE_CMGetComponentByName      \
    USE_CMImportFunctions      \
    USE_NetClientOpenChannel      \
    USE_NetClientOpenChannelResult      \
    USE_NetClientCloseChannel      \
    USE_NetClientRegisterAppInterface      \
    USE_NetClientUnregisterAppInterface      \
    USE_NetClientSend      \
    USE_NSResolveName3      \
    USE_NSResolveBlkDrvAddress3      \
    USE_NSResolveAll3      \
    USE_SysCpuTestAndSetBit      \
    USE_SysCpuAtomicAdd      \
    USE_MemPoolCreateStatic      \
    USE_MemPoolDelete      \
    USE_MemPoolGetBlock2      \
    USE_MemPoolAppendUsedBlock      \
    USE_MemPoolRemoveUsedBlock      \
    USE_MemPoolPutBlock      \
    USE_MemPoolLock      \
    USE_MemPoolUnlock      \
    USE_GWClientGetInterfaceVersion      \
    USE_GWClientEnumCommDrivers2      \
    USE_GWClientBeginConnectToGateway2      \
    USE_GWClientEndConnectToGateway      \
    USE_GWClientDisconnectFromGateway      \
    USE_GWClientBeginSendRequest      \
    USE_GWClientEndSendRequest      \
    USE_GWClientBeginOpenChannel2      \
    USE_GWClientBeginCloseChannel      \
    USE_GWClientBeginIncrementalResolveName3      \
    USE_GWClientEndIncrementalResolveName      \
    USE_GWClientBeginIncrementalResolveAllNodes3      \
    USE_GWClientEndIncrementalResolveAllNodes      \
    USE_GWClientBeginResolveBlockDriverAddress3      \
    USE_GWClientEndResolveBlockDriverAddress      \
    USE_SecChClientSendData      \
    USE_SecChClientDataReceived      \
    USE_SecChClientInit      \
    USE_SecChClientCleanup      \
    USE_SecChGetSecureChannelSize      \
    USE_TlsCreateContext3      \
    USE_TlsFreeContext      \
    USE_X509CertStoreOpen      \
    USE_X509CertStoreClose      \
    USE_X509CertStoreGetFirstCert      \
    USE_X509CertStoreGetNextCert      \
    USE_X509CertInfoInit      \
    USE_X509CertInfoExit      \
    USE_X509CertGetContent      \
    USE_X509CertGetThumbprint      \
    USE_X509ParseCertificate      \
    USE_X509CertStoreAddCert      \
    USE_X509CertStoreRemoveCert      \
    USE_X509CertClose      \
    USE_X509CertGetBinary      \
    USE_CryptoRtsByteStringInit      \
    USE_CryptoRtsByteStringInit2      \
    USE_CryptoRtsByteStringInit3      \
    USE_CryptoRtsByteStringExit      \
    USE_CryptoGetAlgorithmById      \
    USE_CryptoImportAsymmetricKey      \
    USE_CryptoAsymmetricEncrypt      \
    USE_CryptoKeyExit      \
    USE_CryptoGenerateRandomNumber      \
    USE_CryptoGenerateAsymmetricKeyPair      \
    USE_CryptoExportRawAsymmetricKey      \
    USE_CryptoImportRawAsymmetricKey      \
    USE_CryptoKeyAgreement      \
    USE_CryptoKeyInit      \
    USE_CryptoSymmetricEncrypt      \
    USE_RtsOIDGetName     
#define USEIMPORT_STMT \
    /*lint -save --e{551} */ \
    static volatile PF_REGISTER_API s_pfCMRegisterAPI; \
    static volatile PF_REGISTER_API2 s_pfCMRegisterAPI2; \
    static volatile PF_GET_API s_pfCMGetAPI; \
    static volatile PF_GET_API2 s_pfCMGetAPI2; \
    static volatile PF_REGISTER_CLASS s_pfCMRegisterClass; \
    static volatile PF_CREATEINSTANCE s_pfCMCreateInstance; \
    static volatile PF_CALL_HOOK s_pfCMCallHook; \
    /*lint -restore */  \
    ITF_CmpLog   \
    ITF_CMUtils  \
    USE_CMUtlMemCpy  \
    USE_LogAdd \
	ITF_SysFile    \
	ITF_SysModule    \
	/*obsolete entry ITF_CMUtils*/     \
	ITF_CM    \
	ITF_SysEvent    \
	ITF_SysTask    \
	ITF_SysSem    \
	ITF_SysDir    \
	ITF_SysTime    \
	ITF_SysTimeRtc    \
	ITF_CmpXMLParser    \
	ITF_CmpBinTagUtil    \
	ITF_CmpGwClient    \
	ITF_SysMem    \
	ITF_CmpCommunicationLib    \
	ITF_SysCom    \
	ITF_SysSocket    \
	ITF_CmpChannelClient    \
	ITF_CmpNameServiceClient    \
	ITF_SysCpuHandling    \
	ITF_CmpMemPool    \
	ITF_CmpTls    \
	ITF_CmpX509Cert    \
	ITF_CmpSecureChannel    \
	ITF_CmpCrypto     \
    USE_LogGetFilter      \
    USE_LogSetFilter      \
    USE_LogOpen      \
    USE_LogCreate      \
    USE_LogDelete      \
    USE_LogRegisterInterface      \
    USE_LogAddArg      \
    USE_SysModuleLoad      \
    USE_SysModuleUnload      \
    USE_SysModuleGetFunctionPointer      \
    USE_SysDirOpen_      \
    USE_SysDirClose_      \
    USE_SysDirCreate_      \
    USE_SysFileGetPath      \
    USE_SysFileGetPath2      \
    USE_SysFileOpen      \
    USE_SysFileOpen_      \
    USE_SysFileClose      \
    USE_SysFileClose_      \
    USE_SysFileRead      \
    USE_SysFileWrite      \
    USE_SysFileGetSize      \
    USE_SysFileGetSize_      \
    USE_SysFileGetSizeByHandle      \
    USE_SysFileCopy      \
    USE_SysFileRename      \
    USE_SysFileDelete      \
    USE_SysFileEOF      \
    USE_SysFileGetPos      \
    USE_SysFileSetPos      \
    USE_SysFileGenerateCRC2      \
    USE_SysFileGenerateCRC3_      \
    USE_SysFileGetName      \
    USE_CMUtlStrICmp      \
    USE_CMUtlStrNICmp      \
    USE_CMUtlwstrcmp      \
    USE_CMUtlStrToW      \
    USE_CMUtlWToStr      \
    USE_CMUtlcwstrcmp      \
    USE_CMUtlcwstrcpy      \
    USE_CMUtlwstrcpy      \
    USE_CMUtlwstrlen      \
    USE_CMUtlcwstrlen      \
    USE_CMUtlCWToStr      \
    USE_CMUtlSafeStrCpy      \
    USE_CMUtlSafeStrNCpy      \
    USE_CMUtlSafeStrCat      \
    USE_CMUtlsnprintf      \
    USE_CMUtlWToUtf8      \
    USE_CMUtlGUIDToString      \
    USE_CMUtlStringToGUID      \
    USE_CMStringCreate      \
    USE_CMStringDelete      \
    USE_CMUtlStringToVersion      \
    USE_SysSemCreate      \
    USE_SysSemDelete      \
    USE_SysSemEnter      \
    USE_SysSemLeave      \
    USE_SysSemTry      \
    USE_SysEventCreate      \
    USE_SysEventDelete      \
    USE_SysEventSet      \
    USE_SysEventWait      \
    USE_SysTaskCreate      \
    USE_SysTaskDestroy      \
    USE_SysTaskEnter      \
    USE_SysTaskLeave      \
    USE_SysTaskEnd      \
    USE_SysTaskSetExit      \
    USE_SysTaskExit      \
    USE_SysTaskWaitSleep      \
    USE_SysTaskResume      \
    USE_SysTaskSetPriority      \
    USE_SysTaskAutoReleaseOnExit      \
    USE_SysTaskGetOSHandle      \
    USE_SysTaskGetCurrentOSHandle      \
    USE_SysTaskGetInfo      \
    USE_SysTimeGetMs      \
    USE_SysTimeRtcGet      \
    USE_CreateXMLParser3      \
    USE_SetXMLElementHandler      \
    USE_SetXMLCharacterDataHandler      \
    USE_ParseXML2      \
    USE_FreeXMLParser      \
    USE_BTagSwapHeader      \
    USE_BTagWriterInit      \
    USE_BTagWriterInit2      \
    USE_BTagWriterFileInit      \
    USE_BTagWriterFileStreamInit      \
    USE_BTagWriterStartTag      \
    USE_BTagWriterAppendWString      \
    USE_BTagWriterAppendBlob      \
    USE_BTagWriterAppendFillBytes      \
    USE_BTagWriterAppendRaw      \
    USE_BTagWriterAppendString      \
    USE_BTagWriterEndTag      \
    USE_BTagWriterFinish      \
    USE_BTagWriterStartService      \
    USE_BTagWriterFinishService      \
    USE_BTagWriterCreateSavepoint      \
    USE_BTagWriterRestoreSavepoint      \
    USE_BTagWriterGetAvailableBuffer      \
    USE_BTagReaderInit      \
    USE_BTagReaderFileInit      \
    USE_BTagReaderMoveNext      \
    USE_BTagReaderGetTagId      \
    USE_BTagReaderGetTagLen      \
    USE_BTagReaderIsDataTag      \
    USE_BTagReaderGetContent      \
    USE_BTagReaderSkipContent      \
    USE_BTagReaderGetString      \
    USE_BTagReaderPeekNext      \
    USE_BTagReaderGetNextTag      \
    USE_BTagReaderGetFirstTag      \
    USE_CopyAddrToBuffer      \
    USE_SysMemAllocData      \
    USE_SysMemFreeData      \
    USE_SysMemSwap      \
    USE_SysMemForceSwap      \
    USE_SysComOpen      \
    USE_SysComOpen2      \
    USE_SysComClose      \
    USE_SysComSetTimeout      \
    USE_SysComPurge      \
    USE_SysComRead      \
    USE_SysComWrite      \
    USE_SysSockCreate      \
    USE_SysSockClose      \
    USE_SysSockSetOption      \
    USE_SysSockIoctl      \
    USE_SysSockInetAddr      \
    USE_SysSockGetHostName      \
    USE_SysSockGetHostByName      \
    USE_SysSockGetFirstAdapterInfo      \
    USE_SysSockGetNextAdapterInfo      \
    USE_SysSockHtons      \
    USE_SysSockHtonl      \
    USE_SysSockNtohl      \
    USE_SysSockConnect      \
    USE_SysSockSelect      \
    USE_SysSockRecv      \
    USE_SysSockSend      \
    USE_SysSockRecvFrom      \
    USE_SysSockSendTo      \
    USE_SysSockBind      \
    USE_SysSockListen      \
    USE_SysSockAccept      \
    USE_SysSockInetNtoa      \
    USE_SysSockFdZero      \
    USE_SysSockFdInit      \
    USE_SysSockFdIsset      \
    USE_SysSockPing      \
    USE_CMCreateInstance      \
    USE_CMDeleteInstance2      \
    USE_CMLoadComponent      \
    USE_CMInitComponent      \
    USE_CMExitComponent      \
    USE_CMUnloadComponent      \
    USE_CMGetComponentByName      \
    USE_CMImportFunctions      \
    USE_NetClientOpenChannel      \
    USE_NetClientOpenChannelResult      \
    USE_NetClientCloseChannel      \
    USE_NetClientRegisterAppInterface      \
    USE_NetClientUnregisterAppInterface      \
    USE_NetClientSend      \
    USE_NSResolveName3      \
    USE_NSResolveBlkDrvAddress3      \
    USE_NSResolveAll3      \
    USE_SysCpuTestAndSetBit      \
    USE_SysCpuAtomicAdd      \
    USE_MemPoolCreateStatic      \
    USE_MemPoolDelete      \
    USE_MemPoolGetBlock2      \
    USE_MemPoolAppendUsedBlock      \
    USE_MemPoolRemoveUsedBlock      \
    USE_MemPoolPutBlock      \
    USE_MemPoolLock      \
    USE_MemPoolUnlock      \
    USE_GWClientGetInterfaceVersion      \
    USE_GWClientEnumCommDrivers2      \
    USE_GWClientBeginConnectToGateway2      \
    USE_GWClientEndConnectToGateway      \
    USE_GWClientDisconnectFromGateway      \
    USE_GWClientBeginSendRequest      \
    USE_GWClientEndSendRequest      \
    USE_GWClientBeginOpenChannel2      \
    USE_GWClientBeginCloseChannel      \
    USE_GWClientBeginIncrementalResolveName3      \
    USE_GWClientEndIncrementalResolveName      \
    USE_GWClientBeginIncrementalResolveAllNodes3      \
    USE_GWClientEndIncrementalResolveAllNodes      \
    USE_GWClientBeginResolveBlockDriverAddress3      \
    USE_GWClientEndResolveBlockDriverAddress      \
    USE_SecChClientSendData      \
    USE_SecChClientDataReceived      \
    USE_SecChClientInit      \
    USE_SecChClientCleanup      \
    USE_SecChGetSecureChannelSize      \
    USE_TlsCreateContext3      \
    USE_TlsFreeContext      \
    USE_X509CertStoreOpen      \
    USE_X509CertStoreClose      \
    USE_X509CertStoreGetFirstCert      \
    USE_X509CertStoreGetNextCert      \
    USE_X509CertInfoInit      \
    USE_X509CertInfoExit      \
    USE_X509CertGetContent      \
    USE_X509CertGetThumbprint      \
    USE_X509ParseCertificate      \
    USE_X509CertStoreAddCert      \
    USE_X509CertStoreRemoveCert      \
    USE_X509CertClose      \
    USE_X509CertGetBinary      \
    USE_CryptoRtsByteStringInit      \
    USE_CryptoRtsByteStringInit2      \
    USE_CryptoRtsByteStringInit3      \
    USE_CryptoRtsByteStringExit      \
    USE_CryptoGetAlgorithmById      \
    USE_CryptoImportAsymmetricKey      \
    USE_CryptoAsymmetricEncrypt      \
    USE_CryptoKeyExit      \
    USE_CryptoGenerateRandomNumber      \
    USE_CryptoGenerateAsymmetricKeyPair      \
    USE_CryptoExportRawAsymmetricKey      \
    USE_CryptoImportRawAsymmetricKey      \
    USE_CryptoKeyAgreement      \
    USE_CryptoKeyInit      \
    USE_CryptoSymmetricEncrypt      \
    USE_RtsOIDGetName     
#define USEEXTERN_STMT \
    EXT_CMUtlMemCpy  \
    EXT_LogAdd \
	EXTITF_SysFile    \
	EXTITF_SysModule    \
	/*obsolete entry EXTITF_CMUtils*/     \
	EXTITF_CM    \
	EXTITF_SysEvent    \
	EXTITF_SysTask    \
	EXTITF_SysSem    \
	EXTITF_SysDir    \
	EXTITF_SysTime    \
	EXTITF_SysTimeRtc    \
	EXTITF_CmpXMLParser    \
	EXTITF_CmpBinTagUtil    \
	EXTITF_CmpGwClient    \
	EXTITF_SysMem    \
	EXTITF_CmpCommunicationLib    \
	EXTITF_SysCom    \
	EXTITF_SysSocket    \
	EXTITF_CmpChannelClient    \
	EXTITF_CmpNameServiceClient    \
	EXTITF_SysCpuHandling    \
	EXTITF_CmpMemPool    \
	EXTITF_CmpTls    \
	EXTITF_CmpX509Cert    \
	EXTITF_CmpSecureChannel    \
	EXTITF_CmpCrypto     \
    EXT_LogGetFilter  \
    EXT_LogSetFilter  \
    EXT_LogOpen  \
    EXT_LogCreate  \
    EXT_LogDelete  \
    EXT_LogRegisterInterface  \
    EXT_LogAddArg  \
    EXT_SysModuleLoad  \
    EXT_SysModuleUnload  \
    EXT_SysModuleGetFunctionPointer  \
    EXT_SysDirOpen_  \
    EXT_SysDirClose_  \
    EXT_SysDirCreate_  \
    EXT_SysFileGetPath  \
    EXT_SysFileGetPath2  \
    EXT_SysFileOpen  \
    EXT_SysFileOpen_  \
    EXT_SysFileClose  \
    EXT_SysFileClose_  \
    EXT_SysFileRead  \
    EXT_SysFileWrite  \
    EXT_SysFileGetSize  \
    EXT_SysFileGetSize_  \
    EXT_SysFileGetSizeByHandle  \
    EXT_SysFileCopy  \
    EXT_SysFileRename  \
    EXT_SysFileDelete  \
    EXT_SysFileEOF  \
    EXT_SysFileGetPos  \
    EXT_SysFileSetPos  \
    EXT_SysFileGenerateCRC2  \
    EXT_SysFileGenerateCRC3_  \
    EXT_SysFileGetName  \
    EXT_CMUtlStrICmp  \
    EXT_CMUtlStrNICmp  \
    EXT_CMUtlwstrcmp  \
    EXT_CMUtlStrToW  \
    EXT_CMUtlWToStr  \
    EXT_CMUtlcwstrcmp  \
    EXT_CMUtlcwstrcpy  \
    EXT_CMUtlwstrcpy  \
    EXT_CMUtlwstrlen  \
    EXT_CMUtlcwstrlen  \
    EXT_CMUtlCWToStr  \
    EXT_CMUtlSafeStrCpy  \
    EXT_CMUtlSafeStrNCpy  \
    EXT_CMUtlSafeStrCat  \
    EXT_CMUtlsnprintf  \
    EXT_CMUtlWToUtf8  \
    EXT_CMUtlGUIDToString  \
    EXT_CMUtlStringToGUID  \
    EXT_CMStringCreate  \
    EXT_CMStringDelete  \
    EXT_CMUtlStringToVersion  \
    EXT_SysSemCreate  \
    EXT_SysSemDelete  \
    EXT_SysSemEnter  \
    EXT_SysSemLeave  \
    EXT_SysSemTry  \
    EXT_SysEventCreate  \
    EXT_SysEventDelete  \
    EXT_SysEventSet  \
    EXT_SysEventWait  \
    EXT_SysTaskCreate  \
    EXT_SysTaskDestroy  \
    EXT_SysTaskEnter  \
    EXT_SysTaskLeave  \
    EXT_SysTaskEnd  \
    EXT_SysTaskSetExit  \
    EXT_SysTaskExit  \
    EXT_SysTaskWaitSleep  \
    EXT_SysTaskResume  \
    EXT_SysTaskSetPriority  \
    EXT_SysTaskAutoReleaseOnExit  \
    EXT_SysTaskGetOSHandle  \
    EXT_SysTaskGetCurrentOSHandle  \
    EXT_SysTaskGetInfo  \
    EXT_SysTimeGetMs  \
    EXT_SysTimeRtcGet  \
    EXT_CreateXMLParser3  \
    EXT_SetXMLElementHandler  \
    EXT_SetXMLCharacterDataHandler  \
    EXT_ParseXML2  \
    EXT_FreeXMLParser  \
    EXT_BTagSwapHeader  \
    EXT_BTagWriterInit  \
    EXT_BTagWriterInit2  \
    EXT_BTagWriterFileInit  \
    EXT_BTagWriterFileStreamInit  \
    EXT_BTagWriterStartTag  \
    EXT_BTagWriterAppendWString  \
    EXT_BTagWriterAppendBlob  \
    EXT_BTagWriterAppendFillBytes  \
    EXT_BTagWriterAppendRaw  \
    EXT_BTagWriterAppendString  \
    EXT_BTagWriterEndTag  \
    EXT_BTagWriterFinish  \
    EXT_BTagWriterStartService  \
    EXT_BTagWriterFinishService  \
    EXT_BTagWriterCreateSavepoint  \
    EXT_BTagWriterRestoreSavepoint  \
    EXT_BTagWriterGetAvailableBuffer  \
    EXT_BTagReaderInit  \
    EXT_BTagReaderFileInit  \
    EXT_BTagReaderMoveNext  \
    EXT_BTagReaderGetTagId  \
    EXT_BTagReaderGetTagLen  \
    EXT_BTagReaderIsDataTag  \
    EXT_BTagReaderGetContent  \
    EXT_BTagReaderSkipContent  \
    EXT_BTagReaderGetString  \
    EXT_BTagReaderPeekNext  \
    EXT_BTagReaderGetNextTag  \
    EXT_BTagReaderGetFirstTag  \
    EXT_CopyAddrToBuffer  \
    EXT_SysMemAllocData  \
    EXT_SysMemFreeData  \
    EXT_SysMemSwap  \
    EXT_SysMemForceSwap  \
    EXT_SysComOpen  \
    EXT_SysComOpen2  \
    EXT_SysComClose  \
    EXT_SysComSetTimeout  \
    EXT_SysComPurge  \
    EXT_SysComRead  \
    EXT_SysComWrite  \
    EXT_SysSockCreate  \
    EXT_SysSockClose  \
    EXT_SysSockSetOption  \
    EXT_SysSockIoctl  \
    EXT_SysSockInetAddr  \
    EXT_SysSockGetHostName  \
    EXT_SysSockGetHostByName  \
    EXT_SysSockGetFirstAdapterInfo  \
    EXT_SysSockGetNextAdapterInfo  \
    EXT_SysSockHtons  \
    EXT_SysSockHtonl  \
    EXT_SysSockNtohl  \
    EXT_SysSockConnect  \
    EXT_SysSockSelect  \
    EXT_SysSockRecv  \
    EXT_SysSockSend  \
    EXT_SysSockRecvFrom  \
    EXT_SysSockSendTo  \
    EXT_SysSockBind  \
    EXT_SysSockListen  \
    EXT_SysSockAccept  \
    EXT_SysSockInetNtoa  \
    EXT_SysSockFdZero  \
    EXT_SysSockFdInit  \
    EXT_SysSockFdIsset  \
    EXT_SysSockPing  \
    EXT_CMCreateInstance  \
    EXT_CMDeleteInstance2  \
    EXT_CMLoadComponent  \
    EXT_CMInitComponent  \
    EXT_CMExitComponent  \
    EXT_CMUnloadComponent  \
    EXT_CMGetComponentByName  \
    EXT_CMImportFunctions  \
    EXT_NetClientOpenChannel  \
    EXT_NetClientOpenChannelResult  \
    EXT_NetClientCloseChannel  \
    EXT_NetClientRegisterAppInterface  \
    EXT_NetClientUnregisterAppInterface  \
    EXT_NetClientSend  \
    EXT_NSResolveName3  \
    EXT_NSResolveBlkDrvAddress3  \
    EXT_NSResolveAll3  \
    EXT_SysCpuTestAndSetBit  \
    EXT_SysCpuAtomicAdd  \
    EXT_MemPoolCreateStatic  \
    EXT_MemPoolDelete  \
    EXT_MemPoolGetBlock2  \
    EXT_MemPoolAppendUsedBlock  \
    EXT_MemPoolRemoveUsedBlock  \
    EXT_MemPoolPutBlock  \
    EXT_MemPoolLock  \
    EXT_MemPoolUnlock  \
    EXT_GWClientGetInterfaceVersion  \
    EXT_GWClientEnumCommDrivers2  \
    EXT_GWClientBeginConnectToGateway2  \
    EXT_GWClientEndConnectToGateway  \
    EXT_GWClientDisconnectFromGateway  \
    EXT_GWClientBeginSendRequest  \
    EXT_GWClientEndSendRequest  \
    EXT_GWClientBeginOpenChannel2  \
    EXT_GWClientBeginCloseChannel  \
    EXT_GWClientBeginIncrementalResolveName3  \
    EXT_GWClientEndIncrementalResolveName  \
    EXT_GWClientBeginIncrementalResolveAllNodes3  \
    EXT_GWClientEndIncrementalResolveAllNodes  \
    EXT_GWClientBeginResolveBlockDriverAddress3  \
    EXT_GWClientEndResolveBlockDriverAddress  \
    EXT_SecChClientSendData  \
    EXT_SecChClientDataReceived  \
    EXT_SecChClientInit  \
    EXT_SecChClientCleanup  \
    EXT_SecChGetSecureChannelSize  \
    EXT_TlsCreateContext3  \
    EXT_TlsFreeContext  \
    EXT_X509CertStoreOpen  \
    EXT_X509CertStoreClose  \
    EXT_X509CertStoreGetFirstCert  \
    EXT_X509CertStoreGetNextCert  \
    EXT_X509CertInfoInit  \
    EXT_X509CertInfoExit  \
    EXT_X509CertGetContent  \
    EXT_X509CertGetThumbprint  \
    EXT_X509ParseCertificate  \
    EXT_X509CertStoreAddCert  \
    EXT_X509CertStoreRemoveCert  \
    EXT_X509CertClose  \
    EXT_X509CertGetBinary  \
    EXT_CryptoRtsByteStringInit  \
    EXT_CryptoRtsByteStringInit2  \
    EXT_CryptoRtsByteStringInit3  \
    EXT_CryptoRtsByteStringExit  \
    EXT_CryptoGetAlgorithmById  \
    EXT_CryptoImportAsymmetricKey  \
    EXT_CryptoAsymmetricEncrypt  \
    EXT_CryptoKeyExit  \
    EXT_CryptoGenerateRandomNumber  \
    EXT_CryptoGenerateAsymmetricKeyPair  \
    EXT_CryptoExportRawAsymmetricKey  \
    EXT_CryptoImportRawAsymmetricKey  \
    EXT_CryptoKeyAgreement  \
    EXT_CryptoKeyInit  \
    EXT_CryptoSymmetricEncrypt  \
    EXT_RtsOIDGetName 
#ifndef COMPONENT_NAME
    #error COMPONENT_NAME is not defined. This prevents the component from being linked statically. Use SET_COMPONENT_NAME(<name_of_your_component>) to set the name of the component in your .m4 component description.
#endif




#if defined(STATIC_LINK) || defined(MIXED_LINK) || defined(DYNAMIC_LINK) || defined(CPLUSPLUS_STATIC_LINK)
    #define ComponentEntry CmpPLCHandler__Entry
#endif


#ifdef CPLUSPLUS

class CCmpPLCHandler : public ICmpPLCHandler 
{
    public:
        CCmpPLCHandler() : hCmpPLCHandler(RTS_INVALID_HANDLE), iRefCount(0)
        {
        }
        virtual ~CCmpPLCHandler()
        {
        }
        virtual unsigned long AddRef(IBase *pIBase = NULL)
        {
            iRefCount++;
            return iRefCount;
        }
        virtual unsigned long Release(IBase *pIBase = NULL)
        {
            iRefCount--;
            if (iRefCount == 0)
            {
                delete this;
                return 0;
            }
            return iRefCount;
        }

        
        virtual void* QueryInterface(IBase *pIBase, ITFID iid, RTS_RESULT *pResult)
        {
            void *pItf;
            if (iid == ITFID_IBase)
                pItf = dynamic_cast<IBase *>((ICmpPLCHandler *)this);            
            else if (iid == ITFID_ICmpPLCHandler)
                pItf = dynamic_cast<ICmpPLCHandler *>(this); 
            else
            {
                if (pResult != NULL)
                    *pResult = ERR_NOTIMPLEMENTED;
                return NULL;
            }
            if (pResult != (RTS_RESULT *)1)
                (reinterpret_cast<IBase *>(pItf))->AddRef();
            if (pResult != NULL && pResult != (RTS_RESULT *)1)
                *pResult = ERR_OK;
            return pItf;
        }
        virtual RTS_HANDLE CDECL IPLCHandlerCreateInstance(RTS_RESULT *pResult);
        virtual RTS_RESULT CDECL IPLCHandlerDeleteInstance(RTS_HANDLE hPlcHandler);

    public:
        RTS_HANDLE hCmpPLCHandler;
        int iRefCount;
};

#endif /*CPLUSPLUS*/

#endif /*WIN32_RESOURCES*/
#endif /*_DEP_H_*/
