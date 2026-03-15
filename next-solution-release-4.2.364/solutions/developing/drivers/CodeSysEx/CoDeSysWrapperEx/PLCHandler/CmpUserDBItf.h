 /**
 * <interfacename>CmpUserDB</interfacename>
 * <description> 
 *	<p>This interface provides the minimal set of functions that must be implemented to replace the default CODESYS user database. 
 *  The main part is the possibility to authenticate users with different protocols. There may be several components
 *	that implement this interface. A component which implements this interface has to register
 *	at the CmpUserMgr to be called during the login process.</p>
 *
 *	<p>Calls to AuthenticateUser grant or deny access of a user within the login process.</p>
 *	<p>IMPLEMENTATION NOTE: The implementation of AuthenticateUser affects the security of the login process itself. 
 *		Pay attention to the following aspects:
 *		1. The result of the Login process should not allow any hints if the user identifier or the credentials are wrong.
 *			Just return a binary result.
 *		2. The complete protocol must be done even if a user does not exist. Fill the protocol with random data if a user
 *			does not exist to prevent timing based side channel attacks. E.g. do not immediately return ERR_ACCESS_DENIED if a user does not exist.
 *	</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPUSERDBITF_H_
#define _CMPUSERDBITF_H_

#include "CmpStd.h"

 

 



#include "CmpUserMgrItf.h"

/* Settings for CmpUserDBFile */
/* -------------------------- */

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * Setting for CmpUserDBFile
 * Scrypt interations corresponds to CPU/memory cost parameter N
 * </description>
 */
#ifndef USERDB_SCRYPT_ITERATIONS
#define USERDB_SCRYPT_ITERATIONS 0x8000
#endif
#define USERDB_KEY_INT_SCRYPT_ITERATIONS "ScryptIterations"
#ifndef USERDB_SCRYPT_ITERATIONS_MIN
#define USERDB_SCRYPT_ITERATIONS_MIN 0x2000
#endif

/**
 * <description>Authenticates a user. Depending the protocol implementation this function must be called several times to finish the login process. Pay attention to pass the same
 * data structure to all these calles.</description>
 * <param name="pLoginContext" type="INOUT">Context of the current login process.</param>
 * <param name="pUserProperty" type="OUT">The properites of the user.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">The login process has finished without an error. Authentication successful.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_ACCESS_DENIED">The login process has finished without an error. Authentication not successful.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PENDING">The login process has not finished yet.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pLoginContext was NULL, or the login type can not be handled by this implementation.</errorcode>
 */
STATICITF_DEF RTS_RESULT CDECL UserDBAuthenticate(RTS_HANDLE hDatabase, char *pszUser, UserMgrCredentials *pCredentials, RTS_UI32 *pUserProperty);
typedef RTS_RESULT (CDECL * PFUSERDBAUTHENTICATE) (RTS_HANDLE hDatabase, char *pszUser, UserMgrCredentials *pCredentials, RTS_UI32 *pUserProperty);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBAUTHENTICATE_NOTIMPLEMENTED)
	#define USE_UserDBAuthenticate
	#define EXT_UserDBAuthenticate
	#define GET_UserDBAuthenticate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBAuthenticate(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_UserDBAuthenticate  FALSE
	#define EXP_UserDBAuthenticate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBAuthenticate
	#define EXT_UserDBAuthenticate
	#define GET_UserDBAuthenticate(fl)  ERR_OK
	#define CAL_UserDBAuthenticate(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpUserDB*)p0)->IUserDBAuthenticate(((ICmpUserDB*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_UserDBAuthenticate  TRUE
	#define EXP_UserDBAuthenticate  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBAuthenticate
	#define EXT_UserDBAuthenticate
	#define GET_UserDBAuthenticate(fl)  ERR_OK
	#define CAL_UserDBAuthenticate(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpUserDB*)p0)->IUserDBAuthenticate(((ICmpUserDB*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_UserDBAuthenticate  TRUE
	#define EXP_UserDBAuthenticate  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBAuthenticate
	#define EXT_CmpUserDBUserDBAuthenticate
	#define GET_CmpUserDBUserDBAuthenticate  ERR_OK
	#define CAL_CmpUserDBUserDBAuthenticate(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpUserDB*)p0)->IUserDBAuthenticate(p1,p2,p3))
	#define CHK_CmpUserDBUserDBAuthenticate  TRUE
	#define EXP_CmpUserDBUserDBAuthenticate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBAuthenticate
	#define EXT_UserDBAuthenticate
	#define GET_UserDBAuthenticate(fl)  ERR_OK
	#define CAL_UserDBAuthenticate(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpUserDB*)p0)->IUserDBAuthenticate(p1,p2,p3))
	#define CHK_UserDBAuthenticate  TRUE
	#define EXP_UserDBAuthenticate  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_UserDBAuthenticate
	#define EXT_UserDBAuthenticate
	#define GET_UserDBAuthenticate(fl)  ERR_OK
	#define CAL_UserDBAuthenticate(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpUserDB*)p0)->IUserDBAuthenticate(((ICmpUserDB*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_UserDBAuthenticate  TRUE
	#define EXP_UserDBAuthenticate  ERR_OK
#endif





/**
 * <description>Authenticates a user. Depending the protocol implementation this function must be called several times to finish the login process. Pay attention to pass the same
 * data structure to all these calles.</description>
 * <param name="pLoginContext" type="INOUT">Context of the current login process.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">The login process has finished without an error. Check pLoginContext->bAccessGranted of access is granted.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PENDING">The login process has not finished yet. Is equal to pLoginContext->bProtocolFinished</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pLoginContext was NULL, or the login type can not be handled by this implementation.</errorcode>
 */
STATICITF_DEF char* CDECL UserDBGetFirstUser(RTS_HANDLE hDatabase, RTS_HANDLE *hIterator, RTS_UI32 *pUserProperty, RTS_RESULT *pResult);
typedef char* (CDECL * PFUSERDBGETFIRSTUSER) (RTS_HANDLE hDatabase, RTS_HANDLE *hIterator, RTS_UI32 *pUserProperty, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGETFIRSTUSER_NOTIMPLEMENTED)
	#define USE_UserDBGetFirstUser
	#define EXT_UserDBGetFirstUser
	#define GET_UserDBGetFirstUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGetFirstUser(p0,p1,p2,p3)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGetFirstUser  FALSE
	#define EXP_UserDBGetFirstUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGetFirstUser
	#define EXT_UserDBGetFirstUser
	#define GET_UserDBGetFirstUser(fl)  ERR_OK
	#define CAL_UserDBGetFirstUser(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetFirstUser(((ICmpUserDB*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_UserDBGetFirstUser  TRUE
	#define EXP_UserDBGetFirstUser  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGetFirstUser
	#define EXT_UserDBGetFirstUser
	#define GET_UserDBGetFirstUser(fl)  ERR_OK
	#define CAL_UserDBGetFirstUser(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetFirstUser(((ICmpUserDB*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_UserDBGetFirstUser  TRUE
	#define EXP_UserDBGetFirstUser  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGetFirstUser
	#define EXT_CmpUserDBUserDBGetFirstUser
	#define GET_CmpUserDBUserDBGetFirstUser  ERR_OK
	#define CAL_CmpUserDBUserDBGetFirstUser(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetFirstUser(p1,p2,p3))
	#define CHK_CmpUserDBUserDBGetFirstUser  TRUE
	#define EXP_CmpUserDBUserDBGetFirstUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGetFirstUser
	#define EXT_UserDBGetFirstUser
	#define GET_UserDBGetFirstUser(fl)  ERR_OK
	#define CAL_UserDBGetFirstUser(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetFirstUser(p1,p2,p3))
	#define CHK_UserDBGetFirstUser  TRUE
	#define EXP_UserDBGetFirstUser  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_UserDBGetFirstUser
	#define EXT_UserDBGetFirstUser
	#define GET_UserDBGetFirstUser(fl)  ERR_OK
	#define CAL_UserDBGetFirstUser(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetFirstUser(((ICmpUserDB*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_UserDBGetFirstUser  TRUE
	#define EXP_UserDBGetFirstUser  ERR_OK
#endif





/**
 * <description>Authenticates a user. Depending the protocol implementation this function must be called several times to finish the login process. Pay attention to pass the same
 * data structure to all these calles.</description>
 * <param name="pLoginContext" type="INOUT">Context of the current login process.</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">The login process has finished without an error. Check pLoginContext->bAccessGranted of access is granted.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PENDING">The login process has not finished yet. Is equal to pLoginContext->bProtocolFinished</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">pLoginContext was NULL, or the login type can not be handled by this implementation.</errorcode>
 */
STATICITF_DEF char* CDECL UserDBGetNextUser(RTS_HANDLE hDatabase, RTS_HANDLE *hIterator, RTS_UI32 *pUserProperty, RTS_RESULT *pResult);
typedef char* (CDECL * PFUSERDBGETNEXTUSER) (RTS_HANDLE hDatabase, RTS_HANDLE *hIterator, RTS_UI32 *pUserProperty, RTS_RESULT *pResult);
#if defined(CMPUSERDB_NOTIMPLEMENTED) || defined(USERDBGETNEXTUSER_NOTIMPLEMENTED)
	#define USE_UserDBGetNextUser
	#define EXT_UserDBGetNextUser
	#define GET_UserDBGetNextUser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_UserDBGetNextUser(p0,p1,p2,p3)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_UserDBGetNextUser  FALSE
	#define EXP_UserDBGetNextUser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_UserDBGetNextUser
	#define EXT_UserDBGetNextUser
	#define GET_UserDBGetNextUser(fl)  ERR_OK
	#define CAL_UserDBGetNextUser(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetNextUser(((ICmpUserDB*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_UserDBGetNextUser  TRUE
	#define EXP_UserDBGetNextUser  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPUSERDB_EXTERNAL)
	#define USE_UserDBGetNextUser
	#define EXT_UserDBGetNextUser
	#define GET_UserDBGetNextUser(fl)  ERR_OK
	#define CAL_UserDBGetNextUser(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetNextUser(((ICmpUserDB*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_UserDBGetNextUser  TRUE
	#define EXP_UserDBGetNextUser  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpUserDBUserDBGetNextUser
	#define EXT_CmpUserDBUserDBGetNextUser
	#define GET_CmpUserDBUserDBGetNextUser  ERR_OK
	#define CAL_CmpUserDBUserDBGetNextUser(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetNextUser(p1,p2,p3))
	#define CHK_CmpUserDBUserDBGetNextUser  TRUE
	#define EXP_CmpUserDBUserDBGetNextUser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_UserDBGetNextUser
	#define EXT_UserDBGetNextUser
	#define GET_UserDBGetNextUser(fl)  ERR_OK
	#define CAL_UserDBGetNextUser(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetNextUser(p1,p2,p3))
	#define CHK_UserDBGetNextUser  TRUE
	#define EXP_UserDBGetNextUser  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_UserDBGetNextUser
	#define EXT_UserDBGetNextUser
	#define GET_UserDBGetNextUser(fl)  ERR_OK
	#define CAL_UserDBGetNextUser(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? 0 : ((ICmpUserDB*)p0)->IUserDBGetNextUser(((ICmpUserDB*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_UserDBGetNextUser  TRUE
	#define EXP_UserDBGetNextUser  ERR_OK
#endif







typedef struct
{
	IBase_C *pBase;
	PFUSERDBAUTHENTICATE IUserDBAuthenticate;
 	PFUSERDBGETFIRSTUSER IUserDBGetFirstUser;
 	PFUSERDBGETNEXTUSER IUserDBGetNextUser;
 } ICmpUserDB_C;

#ifdef CPLUSPLUS
class ICmpUserDB : public IBase
{
	public:
		virtual RTS_RESULT CDECL IUserDBAuthenticate(char *pszUser, UserMgrCredentials *pCredentials, RTS_UI32 *pUserProperty) =0;
		virtual char* CDECL IUserDBGetFirstUser(RTS_HANDLE *hIterator, RTS_UI32 *pUserProperty, RTS_RESULT *pResult) =0;
		virtual char* CDECL IUserDBGetNextUser(RTS_HANDLE *hIterator, RTS_UI32 *pUserProperty, RTS_RESULT *pResult) =0;
};
	#ifndef ITF_CmpUserDB
		#define ITF_CmpUserDB static ICmpUserDB *pICmpUserDB = NULL;
	#endif
	#define EXTITF_CmpUserDB
#else	/*CPLUSPLUS*/
	typedef ICmpUserDB_C		ICmpUserDB;
	#ifndef ITF_CmpUserDB
		#define ITF_CmpUserDB
	#endif
	#define EXTITF_CmpUserDB
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPUSERDBITF_H_*/
