 /**
 * <interfacename>CmpChannelClientApp</interfacename>
 * <description>
 *	Interface for the client application to be exported to the NetClient component.
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPCHANNELCLIENTAPPITF_H_
#define _CMPCHANNELCLIENTAPPITF_H_

#include "CmpStd.h"

 

 




#include "CmpCommunicationLibItf.h"

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 *   Called when a layer4 message has been received.
 * </description>
 * <param name="hInstance" type="IN">Handle to the instance</param>
 * <param name="wChannelHandle" type="IN">The channel that received the message</param>
 * <param name="pduData" type="IN">The message that has been received</param>
 * <result>error code</result>
 */
STATICITF_DEF int CDECL ClientAppHandleMessage(RTS_HANDLE hInstance, unsigned short wChannelHandle, PROTOCOL_DATA_UNIT pduData);
typedef int (CDECL * PFCLIENTAPPHANDLEMESSAGE) (RTS_HANDLE hInstance, unsigned short wChannelHandle, PROTOCOL_DATA_UNIT pduData);
#if defined(CMPCHANNELCLIENTAPP_NOTIMPLEMENTED) || defined(CLIENTAPPHANDLEMESSAGE_NOTIMPLEMENTED)
	#define USE_ClientAppHandleMessage
	#define EXT_ClientAppHandleMessage
	#define GET_ClientAppHandleMessage(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ClientAppHandleMessage(p0,p1,p2)  (int)ERR_NOTIMPLEMENTED
	#define CHK_ClientAppHandleMessage  FALSE
	#define EXP_ClientAppHandleMessage  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ClientAppHandleMessage
	#define EXT_ClientAppHandleMessage
	#define GET_ClientAppHandleMessage(fl)  ERR_OK
	#define CAL_ClientAppHandleMessage(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppHandleMessage(((ICmpChannelClientApp*)p0)->pBase->hInstance,p1,p2))
	#define CHK_ClientAppHandleMessage  TRUE
	#define EXP_ClientAppHandleMessage  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENTAPP_EXTERNAL)
	#define USE_ClientAppHandleMessage
	#define EXT_ClientAppHandleMessage
	#define GET_ClientAppHandleMessage(fl)  ERR_OK
	#define CAL_ClientAppHandleMessage(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppHandleMessage(((ICmpChannelClientApp*)p0)->pBase->hInstance,p1,p2))
	#define CHK_ClientAppHandleMessage  TRUE
	#define EXP_ClientAppHandleMessage  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientAppClientAppHandleMessage
	#define EXT_CmpChannelClientAppClientAppHandleMessage
	#define GET_CmpChannelClientAppClientAppHandleMessage  ERR_OK
	#define CAL_CmpChannelClientAppClientAppHandleMessage(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppHandleMessage(p1,p2))
	#define CHK_CmpChannelClientAppClientAppHandleMessage  TRUE
	#define EXP_CmpChannelClientAppClientAppHandleMessage  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ClientAppHandleMessage
	#define EXT_ClientAppHandleMessage
	#define GET_ClientAppHandleMessage(fl)  ERR_OK
	#define CAL_ClientAppHandleMessage(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppHandleMessage(p1,p2))
	#define CHK_ClientAppHandleMessage  TRUE
	#define EXP_ClientAppHandleMessage  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_ClientAppHandleMessage
	#define EXT_ClientAppHandleMessage
	#define GET_ClientAppHandleMessage(fl)  ERR_OK
	#define CAL_ClientAppHandleMessage(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppHandleMessage(((ICmpChannelClientApp*)p0)->pBase->hInstance,p1,p2))
	#define CHK_ClientAppHandleMessage  TRUE
	#define EXP_ClientAppHandleMessage  ERR_OK
#endif




/**
 * <description>
 *   Called by the NetClient component when a channel is closed by the server or a 
 *   communication error occurs. 
 *   This function is not called when the channel is closed by a call to NetClientCloseChannel.
 * </description>
 * <param name="hInstance" type="IN">Handle to the instance</param>
 * <param name="wChannelHandle" type="IN">The closed channel</param>
 * <param name="nError" type="IN">The cause why the channel is closed</param>
 * <result>error code</result>
 */
STATICITF_DEF int CDECL ClientAppOnChannelError(RTS_HANDLE hInstance, unsigned short wChannelHandle, int nError);
typedef int (CDECL * PFCLIENTAPPONCHANNELERROR) (RTS_HANDLE hInstance, unsigned short wChannelHandle, int nError);
#if defined(CMPCHANNELCLIENTAPP_NOTIMPLEMENTED) || defined(CLIENTAPPONCHANNELERROR_NOTIMPLEMENTED)
	#define USE_ClientAppOnChannelError
	#define EXT_ClientAppOnChannelError
	#define GET_ClientAppOnChannelError(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ClientAppOnChannelError(p0,p1,p2)  (int)ERR_NOTIMPLEMENTED
	#define CHK_ClientAppOnChannelError  FALSE
	#define EXP_ClientAppOnChannelError  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ClientAppOnChannelError
	#define EXT_ClientAppOnChannelError
	#define GET_ClientAppOnChannelError(fl)  ERR_OK
	#define CAL_ClientAppOnChannelError(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppOnChannelError(((ICmpChannelClientApp*)p0)->pBase->hInstance,p1,p2))
	#define CHK_ClientAppOnChannelError  TRUE
	#define EXP_ClientAppOnChannelError  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPCHANNELCLIENTAPP_EXTERNAL)
	#define USE_ClientAppOnChannelError
	#define EXT_ClientAppOnChannelError
	#define GET_ClientAppOnChannelError(fl)  ERR_OK
	#define CAL_ClientAppOnChannelError(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppOnChannelError(((ICmpChannelClientApp*)p0)->pBase->hInstance,p1,p2))
	#define CHK_ClientAppOnChannelError  TRUE
	#define EXP_ClientAppOnChannelError  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpChannelClientAppClientAppOnChannelError
	#define EXT_CmpChannelClientAppClientAppOnChannelError
	#define GET_CmpChannelClientAppClientAppOnChannelError  ERR_OK
	#define CAL_CmpChannelClientAppClientAppOnChannelError(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppOnChannelError(p1,p2))
	#define CHK_CmpChannelClientAppClientAppOnChannelError  TRUE
	#define EXP_CmpChannelClientAppClientAppOnChannelError  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ClientAppOnChannelError
	#define EXT_ClientAppOnChannelError
	#define GET_ClientAppOnChannelError(fl)  ERR_OK
	#define CAL_ClientAppOnChannelError(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppOnChannelError(p1,p2))
	#define CHK_ClientAppOnChannelError  TRUE
	#define EXP_ClientAppOnChannelError  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_ClientAppOnChannelError
	#define EXT_ClientAppOnChannelError
	#define GET_ClientAppOnChannelError(fl)  ERR_OK
	#define CAL_ClientAppOnChannelError(p0,p1,p2) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpChannelClientApp*)p0)->IClientAppOnChannelError(((ICmpChannelClientApp*)p0)->pBase->hInstance,p1,p2))
	#define CHK_ClientAppOnChannelError  TRUE
	#define EXP_ClientAppOnChannelError  ERR_OK
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFCLIENTAPPHANDLEMESSAGE IClientAppHandleMessage;
 	PFCLIENTAPPONCHANNELERROR IClientAppOnChannelError;
 } ICmpChannelClientApp_C;

#ifdef CPLUSPLUS
class ICmpChannelClientApp : public IBase
{
	public:
		virtual int CDECL IClientAppHandleMessage(unsigned short wChannelHandle, PROTOCOL_DATA_UNIT pduData) =0;
		virtual int CDECL IClientAppOnChannelError(unsigned short wChannelHandle, int nError) =0;
};
	#ifndef ITF_CmpChannelClientApp
		#define ITF_CmpChannelClientApp static ICmpChannelClientApp *pICmpChannelClientApp = NULL;
	#endif
	#define EXTITF_CmpChannelClientApp
#else	/*CPLUSPLUS*/
	typedef ICmpChannelClientApp_C		ICmpChannelClientApp;
	#ifndef ITF_CmpChannelClientApp
		#define ITF_CmpChannelClientApp
	#endif
	#define EXTITF_CmpChannelClientApp
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPCHANNELCLIENTAPPITF_H_*/
