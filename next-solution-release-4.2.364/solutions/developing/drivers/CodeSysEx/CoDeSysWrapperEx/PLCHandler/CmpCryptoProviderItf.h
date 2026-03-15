 /**
 * <interfacename>CmpCryptoProvider</interfacename>
 * <description>
 *	<p>This is a low level cryptographic interface and can not be used by any component directly. Use the CmpCryptoItf instead to get a
 *	consistant interface to different algorithms. This system interface is used to provided access to the operating systems or hardware specific
 *	cryptographic capabilities of the device itself.</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */

	
	
#ifndef _CMPCRYPTOPROVIDERITF_H_
#define _CMPCRYPTOPROVIDERITF_H_

#include "CmpStd.h"

 

 




/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 * (Only for Linux).
 * Example:
 * </description>
 */
#define SYSCRYPTO_KEY_STRING_ENTROPYFILE					"Linux.EntropyFile"
#ifndef SYSCRYPTO_KEY_STRING_ENTROPYFILE_DEFAULT
    #define SYSCRYPTO_KEY_STRING_ENTROPYFILE_DEFAULT		""
#endif /* SYSCRYPTO_KEY_STRING_ENTROPYFILE_DEFAULT */

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 * (Only for Linux).
 * Example:
 * </description>
 */
#define SYSCRYPTO_KEY_STRING_ENTROPYSOURCE					"Linux.EntropySource"
#ifndef SYSCRYPTO_KEY_STRING_ENTROPYSOURCE_DEFAULT
    #define SYSCRYPTO_KEY_STRING_ENTROPYSOURCE_DEFAULT		"/dev/hwrng"
#endif /* SYSCRYPTO_KEY_STRING_ENTROPYSOURCE_DEFAULT */

#ifdef __cplusplus
extern "C" {
#endif


/**
 * <category>Crypto provider capabilities</category>
 * <description>
 *  These flages are used to describe which capabilites are implemented in the crypto provider.
 *  These flags can be be combined together if multiple functions are implemented.
 * </description>
 *	<element name="CRYPTO_PROVIDER_CAP_RNG">The provider has a random number generator which can be used.</element>
 */
#define CRYPTO_PROVIDER_CAP_RNG         0x1


/**
 * <category>Cryptographic provider information</category>
 * <description>This structure is used to describe an crypto provider.</description>
 * <element name="structSize">Size of the structure itself</element>
 * <element name="cmpId">The component id of the component implementing this provider.</element>
 * <element name="hProvider">The handle to the provider. Used for calles of the interface.</element>
 * <element name="providerInterfaceSize">The size of the interface implemented (sizeof(ICmpCryptoProvider_C))</element>
 * <element name="providerVersion">The version of the provider. Typically the component version.</element>
 * <element name="providerCapabilities">The capabilities of the provider. See: Crypto provider capabilities</element>
 * <element name="providerFlags">Flags of the provider. Not used by now.</element>
 */
typedef struct
{
    RTS_UI32    structSize;
    CMPID       cmpId;
    RTS_HANDLE  hProvider;
    RTS_UI32    providerInterfaceSize;
    RTS_UI32    providerVersion;
    RTS_UI32    providerCapabilities;
    RTS_UI32    providerFlags;
} CryptoProvider;

#ifdef __cplusplus
}
#endif

/**
 * <description>This function is used a integrate the systems random source into
 *  to the cryptographic library of the runtime system itself.  This function is
 *  called to initially seed the PRNG of the cryptographic library of the runtime
 *  system itself. It is expected that this function return cryptographic secure
 *  randomness. The returned random bytes of this function heavily influence the
 *  quality of the generated cryptographic key material such as session keys or
 *  private keys. If not enough randomness is available at the moment of calling
 *  this function is expected to block the caller until the quality of the underlying
 *  source is good.</description>
 * <param name="hBackend" type="IN">Handle to the backend that is called.</param>
 * <param name="pBuffer" type="INOUT">Pointer to a buffer where to store the random bytes.</param>
 * <param name="pBufferSize" type="IN">Size of the buffer to be filled up.</param>
 * <param name="pEntropy" type="IN">The minimum number of random bits availalbe in pBuffer.</param>
 * <result>ERR_OK: pBuffer has been filled with random data. ERR_FAILED: Something went wrong. No random data available.</result>
 */
STATICITF_DEF RTS_RESULT CDECL CryptoProviderGetRandomBytes(RTS_HANDLE hBackend, RTS_UI8* pBuffer, RTS_SIZE pBufferSize, RTS_SIZE *pEntropy);
typedef RTS_RESULT (CDECL * PFCRYPTOPROVIDERGETRANDOMBYTES) (RTS_HANDLE hBackend, RTS_UI8* pBuffer, RTS_SIZE pBufferSize, RTS_SIZE *pEntropy);
#if defined(CMPCRYPTOPROVIDER_NOTIMPLEMENTED) || defined(CRYPTOPROVIDERGETRANDOMBYTES_NOTIMPLEMENTED)
	#define USE_CryptoProviderGetRandomBytes
	#define EXT_CryptoProviderGetRandomBytes
	#define GET_CryptoProviderGetRandomBytes(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoProviderGetRandomBytes(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoProviderGetRandomBytes  FALSE
	#define EXP_CryptoProviderGetRandomBytes  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoProviderGetRandomBytes
	#define EXT_CryptoProviderGetRandomBytes
	#define GET_CryptoProviderGetRandomBytes(fl)  ERR_OK
	#define CAL_CryptoProviderGetRandomBytes(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpCryptoProvider*)p0)->ICryptoProviderGetRandomBytes(((ICmpCryptoProvider*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_CryptoProviderGetRandomBytes  TRUE
	#define EXP_CryptoProviderGetRandomBytes  ERR_OK
#elif defined(MIXED_LINK) && !defined(CMPCRYPTOPROVIDER_EXTERNAL)
	#define USE_CryptoProviderGetRandomBytes
	#define EXT_CryptoProviderGetRandomBytes
	#define GET_CryptoProviderGetRandomBytes(fl)  ERR_OK
	#define CAL_CryptoProviderGetRandomBytes(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpCryptoProvider*)p0)->ICryptoProviderGetRandomBytes(((ICmpCryptoProvider*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_CryptoProviderGetRandomBytes  TRUE
	#define EXP_CryptoProviderGetRandomBytes  ERR_OK
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoProviderCryptoProviderGetRandomBytes
	#define EXT_CmpCryptoProviderCryptoProviderGetRandomBytes
	#define GET_CmpCryptoProviderCryptoProviderGetRandomBytes  ERR_OK
	#define CAL_CmpCryptoProviderCryptoProviderGetRandomBytes(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpCryptoProvider*)p0)->ICryptoProviderGetRandomBytes(p1,p2,p3))
	#define CHK_CmpCryptoProviderCryptoProviderGetRandomBytes  TRUE
	#define EXP_CmpCryptoProviderCryptoProviderGetRandomBytes  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoProviderGetRandomBytes
	#define EXT_CryptoProviderGetRandomBytes
	#define GET_CryptoProviderGetRandomBytes(fl)  ERR_OK
	#define CAL_CryptoProviderGetRandomBytes(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpCryptoProvider*)p0)->ICryptoProviderGetRandomBytes(p1,p2,p3))
	#define CHK_CryptoProviderGetRandomBytes  TRUE
	#define EXP_CryptoProviderGetRandomBytes  ERR_OK
#else /* DYNAMIC_LINK */
	#define USE_CryptoProviderGetRandomBytes
	#define EXT_CryptoProviderGetRandomBytes
	#define GET_CryptoProviderGetRandomBytes(fl)  ERR_OK
	#define CAL_CryptoProviderGetRandomBytes(p0,p1,p2,p3) (p0 == RTS_INVALID_HANDLE || p0 == NULL ? (RTS_RESULT)ERR_PARAMETER : ((ICmpCryptoProvider*)p0)->ICryptoProviderGetRandomBytes(((ICmpCryptoProvider*)p0)->pBase->hInstance,p1,p2,p3))
	#define CHK_CryptoProviderGetRandomBytes  TRUE
	#define EXP_CryptoProviderGetRandomBytes  ERR_OK
#endif






typedef struct
{
	IBase_C *pBase;
	PFCRYPTOPROVIDERGETRANDOMBYTES ICryptoProviderGetRandomBytes;
 } ICmpCryptoProvider_C;

#ifdef CPLUSPLUS
class ICmpCryptoProvider : public IBase
{
	public:
		virtual RTS_RESULT CDECL ICryptoProviderGetRandomBytes(RTS_UI8* pBuffer, RTS_SIZE pBufferSize, RTS_SIZE *pEntropy) =0;
};
	#ifndef ITF_CmpCryptoProvider
		#define ITF_CmpCryptoProvider static ICmpCryptoProvider *pICmpCryptoProvider = NULL;
	#endif
	#define EXTITF_CmpCryptoProvider
#else	/*CPLUSPLUS*/
	typedef ICmpCryptoProvider_C		ICmpCryptoProvider;
	#ifndef ITF_CmpCryptoProvider
		#define ITF_CmpCryptoProvider
	#endif
	#define EXTITF_CmpCryptoProvider
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPCRYPTOPROVIDERITF_H_*/
