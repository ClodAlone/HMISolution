 
	
	
#ifndef _CMPCRYPTMD5ITF_H_
#define _CMPCRYPTMD5ITF_H_

#include "CmpStd.h"

 

 




#include "CmpItf.h"

/**
 * <category>Static defines</category>
 * <description>MD5 key length</description>
 */
#define	CRYPTMD5_KEY_LENGTH					33

#ifdef __cplusplus
extern "C" {
#endif

RTS_RESULT CDECL CryptMD5Calculate(const unsigned char* pchInput, int nInputLength, char* pcResult, int iResultLen);
typedef RTS_RESULT (CDECL * PFCRYPTMD5CALCULATE) (const unsigned char* pchInput, int nInputLength, char* pcResult, int iResultLen);
#if defined(CMPCRYPTMD5_NOTIMPLEMENTED) || defined(CRYPTMD5CALCULATE_NOTIMPLEMENTED)
	#define USE_CryptMD5Calculate
	#define EXT_CryptMD5Calculate
	#define GET_CryptMD5Calculate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptMD5Calculate(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptMD5Calculate  FALSE
	#define EXP_CryptMD5Calculate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptMD5Calculate
	#define EXT_CryptMD5Calculate
	#define GET_CryptMD5Calculate(fl)  CAL_CMGETAPI( "CryptMD5Calculate" ) 
	#define CAL_CryptMD5Calculate  CryptMD5Calculate
	#define CHK_CryptMD5Calculate  TRUE
	#define EXP_CryptMD5Calculate  CAL_CMEXPAPI( "CryptMD5Calculate" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTMD5_EXTERNAL)
	#define USE_CryptMD5Calculate
	#define EXT_CryptMD5Calculate
	#define GET_CryptMD5Calculate(fl)  CAL_CMGETAPI( "CryptMD5Calculate" ) 
	#define CAL_CryptMD5Calculate  CryptMD5Calculate
	#define CHK_CryptMD5Calculate  TRUE
	#define EXP_CryptMD5Calculate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptMD5Calculate", (RTS_UINTPTR)CryptMD5Calculate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptMD5CryptMD5Calculate
	#define EXT_CmpCryptMD5CryptMD5Calculate
	#define GET_CmpCryptMD5CryptMD5Calculate  ERR_OK
	#define CAL_CmpCryptMD5CryptMD5Calculate pICmpCryptMD5->ICryptMD5Calculate
	#define CHK_CmpCryptMD5CryptMD5Calculate (pICmpCryptMD5 != NULL)
	#define EXP_CmpCryptMD5CryptMD5Calculate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptMD5Calculate
	#define EXT_CryptMD5Calculate
	#define GET_CryptMD5Calculate(fl)  CAL_CMGETAPI( "CryptMD5Calculate" ) 
	#define CAL_CryptMD5Calculate pICmpCryptMD5->ICryptMD5Calculate
	#define CHK_CryptMD5Calculate (pICmpCryptMD5 != NULL)
	#define EXP_CryptMD5Calculate  CAL_CMEXPAPI( "CryptMD5Calculate" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptMD5Calculate  PFCRYPTMD5CALCULATE pfCryptMD5Calculate;
	#define EXT_CryptMD5Calculate  extern PFCRYPTMD5CALCULATE pfCryptMD5Calculate;
	#define GET_CryptMD5Calculate(fl)  s_pfCMGetAPI2( "CryptMD5Calculate", (RTS_VOID_FCTPTR *)&pfCryptMD5Calculate, (fl), 0, 0)
	#define CAL_CryptMD5Calculate  pfCryptMD5Calculate
	#define CHK_CryptMD5Calculate  (pfCryptMD5Calculate != NULL)
	#define EXP_CryptMD5Calculate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptMD5Calculate", (RTS_UINTPTR)CryptMD5Calculate, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFCRYPTMD5CALCULATE ICryptMD5Calculate;
 } ICmpCryptMD5_C;

#ifdef CPLUSPLUS
class ICmpCryptMD5 : public IBase
{
	public:
		virtual RTS_RESULT CDECL ICryptMD5Calculate(const unsigned char* pchInput, int nInputLength, char* pcResult, int iResultLen) =0;
};
	#ifndef ITF_CmpCryptMD5
		#define ITF_CmpCryptMD5 static ICmpCryptMD5 *pICmpCryptMD5 = NULL;
	#endif
	#define EXTITF_CmpCryptMD5
#else	/*CPLUSPLUS*/
	typedef ICmpCryptMD5_C		ICmpCryptMD5;
	#ifndef ITF_CmpCryptMD5
		#define ITF_CmpCryptMD5
	#endif
	#define EXTITF_CmpCryptMD5
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPCRYPTMD5ITF_H_*/
