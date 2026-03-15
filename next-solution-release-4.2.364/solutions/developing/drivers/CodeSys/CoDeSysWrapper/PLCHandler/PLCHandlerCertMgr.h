/*****************************************************************************
*
*	Copyright:		(c) 3S - Smart Software Solutions GmbH, Kempten
*	Module: 		PLCHandler
*	Purpose:		Class to handle complete communication to one plc
*
*	Description:	see PLCHandler Programming Guide
*
******************************************************************************/

#ifndef __PLCHANDLERCERTMGR_H__
#define __PLCHANDLERCERTMGR_H__


#include "PLCHandler.h"
#include <string>

typedef enum
{
	CERT_UNTRUSTED = 1,
	CERT_TRUSTED,
	CERT_OWN,
	CERT_QUARANTINE
}PLCHANDLER_CERTTRUSTSTORE;

#if __cplusplus

class PLCH_DLL_DECL PLCHandlerCertMgr
{
	public:
		// Get the first certificate from a dedicated trustStore.
		//  trustLevel = The where to get the certificate.
		//  ppCert = Pointer to the certificate. This void* is used as a handle to the certificate.
		// Note: Use ReleaseCert to free up the resources if the certificate is not needed anymore.
		// Note: *pCert will be set to NULL if no certificate is available.
		long GetFirstCert(PLCHANDLER_CERTTRUSTSTORE trustStore, void **ppCert);

		// Get the next certificate. The trustStore is determined for the previos certificate.
		//  pLastCert: The previos certificate. Either from GetFirstCert or GetNextCert.
		//  ppNextCert: Pointer to the certificate. This void* is used as a handle to the certificate.
		// Note: Use ReleaseCert to free up the resources if the certificate is not needed anymore.
		// Note: *ppCert will be set to NULL if the last certificate has been reached.
		long GetNextCert(void *pLastCert, void **ppNextCert);

		// Get the certificate content in a human readable form. The following fields 
		// will be available: Issuer, Validity, Subject, Subject alternative names, 
		// KeyUsage, ExtendedKeyUsage, SHA-1 Thumprint
		//  pCert = Handle of the certificate
		//  ppszCertContent = Pointer to get the output. Note: memory will be allocated 
		//                    by this call. *ppszCertContent must be NULL. 
		//  pszIndent = A prefix that will be added infront of each line.
		// Note: Use ReleaseBuffer before calling this function or ExportCert again.
		long GetCertContent(void *pCert, const char** ppszCertContent, char* pszIndent = NULL);

		// Use this function to import a certificate file to the PLCHandler certificate trustStore.
		// The file must be BASE64 encoded to be imported.
		//  pszBase64Cert = Certificate to be imported.
		//  trustStore = Where to import the certificate.
		long ImportCert(char *pszBase64Cert, PLCHANDLER_CERTTRUSTSTORE trustStore);

		// Use this function to export a certificate of the PLCHandler certificate trustStore.
		// You will be a BASE64 encoded certificate.
		//  pCert = Handle of the certificate.
		//  ppszBase64Cert = Pointer ot the output. Note: memory will be allocated
		//                   by this call. *ppszBase64Cert must be NULL.
		// Note: Use ReleaseBuffer before calling this function or ExportCert again.
		long ExportCert(void *pCert, const char **ppszBase64Cert);

		// Move a certificate inside the PLCHandler certificate trustStore from one trustStore
		// to another.
		//  pCert = Handle of the certificate.
		//  newTrustStore = New location of the certificate.
		long MoveCert(void *pCert, PLCHANDLER_CERTTRUSTSTORE newTrustStore);

		// Remove a certificate from the PLCHandler certificate trustStore.
		//  pCert = Handle of the certificate.
		// Note: pCert must be closed.
		long RemoveCert(void *pCert);

		// Release a certificate. Free up the ressources.
		//  pCert = Handle of the certificate.
		long ReleaseCert(void *pCert);

		// Relase the memory allocated by
		// CertGetContent or CertExport.
		long ReleaseBuffer();

		PLCHandlerCertMgr();
		~PLCHandlerCertMgr();
	private:
		CPLCHandler* m_plcHandler;
		RTS_HANDLE m_hCertStore;
		std::string *m_buffer;
};

#endif /* __cplusplus */
#endif
