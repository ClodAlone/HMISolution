/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community License ("RCL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community License ("RCL") Version 1.00, or subsequent versions as 
 * allowed by the RCL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCL.
 * 
 * All software distributed under the RCL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCL for specific 
 * language governing rights and limitations under the RCL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCL/1.00/
 * ======================================================================*/

#define WIN32_LEAN_AND_MEAN
#include <targetver.h>
#include <windows.h>
#include <string>
#include <vector>
#include <opcua.h>

#include "Application.h"
#include "StatusCodeException.h"
#include "opcua_certificates.h"

#define CLIENT_SERIALIZER_MAXALLOC                   16777216
#define CLIENT_ENCODER_MAXSTRINGLENGTH               ((OpcUa_UInt32)16777216)
#define CLIENT_ENCODER_MAXARRAYLENGTH                ((OpcUa_UInt32)65536)
#define CLIENT_ENCODER_MAXBYTESTRINGLENGTH           ((OpcUa_UInt32)16777216)
#define CLIENT_ENCODER_MAXMESSAGELENGTH              ((OpcUa_UInt32)16777216)
#define CLIENT_SECURELISTENER_THREADPOOL_MINTHREADS  5
#define CLIENT_SECURELISTENER_THREADPOOL_MAXTHREADS  5
#define CLIENT_SECURELISTENER_THREADPOOL_MAXJOBS     20
#define CLIENT_SECURITYTOKEN_LIFETIME_MAX            3600000
#define CLIENT_SECURITYTOKEN_LIFETIME_MIN            60000
#define CLIENT_TCPLISTENER_DEFAULTCHUNKSIZE          ((OpcUa_UInt32)65536)
#define CLIENT_TCPCONNECTION_DEFAULTCHUNKSIZE        ((OpcUa_UInt32)65536)

Application::Application(void)
{
	memset(&m_hPlatformLayer, 0, sizeof(OpcUa_ProxyStubConfiguration));
}

Application::~Application(void)
{
	Uninitialize();
}

void Application::Uninitialize(void)
{
	if (m_hPlatformLayer != 0)
	{
		OpcUa_ProxyStub_Clear();
		OpcUa_P_Clean(&m_hPlatformLayer);
		m_hPlatformLayer = 0;
	}
}

void Application::Log(OpcUa_UInt32 uTraceLevel, OpcUa_CharA* sFormat, ...)
{
    OpcUa_P_VA_List argumentList;
    OPCUA_P_VA_START(argumentList, sFormat);
    OpcUa_Trace(uTraceLevel, sFormat, argumentList);
}

void Application::Initialize(void)
{
    OpcUa_StatusCode uStatus = OpcUa_Good;

    try
    {
	    // initialize the WIN32 platform layer. 
	    m_hPlatformLayer = 0;
	    uStatus = OpcUa_P_Initialize(&m_hPlatformLayer);
        ThrowIfBad(uStatus, "Could not initialize the platform layer.");

	    // these parameters control tracing.
	    m_tConfiguration.bProxyStub_Trace_Enabled              = OpcUa_False;
	    m_tConfiguration.uProxyStub_Trace_Level                = OPCUA_TRACE_OUTPUT_LEVEL_ALL;

	    // these parameters are used to protect against buffer overflows caused by bad data.
	    // they may need to be adjusted depending on the needs of the application.
	    // the server also sets these limits which means errors could occur even if these limits are raised. 
	    m_tConfiguration.iSerializer_MaxAlloc                  = CLIENT_SERIALIZER_MAXALLOC;
	    m_tConfiguration.iSerializer_MaxStringLength           = CLIENT_ENCODER_MAXSTRINGLENGTH;
	    m_tConfiguration.iSerializer_MaxByteStringLength       = CLIENT_ENCODER_MAXARRAYLENGTH;
	    m_tConfiguration.iSerializer_MaxArrayLength            = CLIENT_ENCODER_MAXBYTESTRINGLENGTH;
	    m_tConfiguration.iSerializer_MaxMessageSize            = CLIENT_ENCODER_MAXMESSAGELENGTH;

	    // the thread pool is only used in a server to dispatch incoming requests.
	    m_tConfiguration.bSecureListener_ThreadPool_Enabled    = OpcUa_False;
	    m_tConfiguration.iSecureListener_ThreadPool_MinThreads = CLIENT_SECURELISTENER_THREADPOOL_MINTHREADS;
	    m_tConfiguration.iSecureListener_ThreadPool_MaxThreads = CLIENT_SECURELISTENER_THREADPOOL_MAXTHREADS;
	    m_tConfiguration.iSecureListener_ThreadPool_MaxJobs    = CLIENT_SECURELISTENER_THREADPOOL_MAXJOBS;
	    m_tConfiguration.bSecureListener_ThreadPool_BlockOnAdd = OpcUa_True;
	    m_tConfiguration.uSecureListener_ThreadPool_Timeout    = OPCUA_INFINITE;

	    // these parameters are used to tune performance. larger chunks == more memory, slower performance.
	    m_tConfiguration.iTcpListener_DefaultChunkSize         = CLIENT_TCPLISTENER_DEFAULTCHUNKSIZE;
	    m_tConfiguration.iTcpConnection_DefaultChunkSize       = CLIENT_TCPCONNECTION_DEFAULTCHUNKSIZE;
	    m_tConfiguration.iTcpTransport_MaxMessageLength        = CLIENT_ENCODER_MAXMESSAGELENGTH;
	    m_tConfiguration.iTcpTransport_MaxChunkCount           = -1;
	    m_tConfiguration.bTcpListener_ClientThreadsEnabled     = OpcUa_False;
	    m_tConfiguration.bTcpStream_ExpectWriteToBlock         = OpcUa_True;

	    // initialize the stack.
	    uStatus = OpcUa_ProxyStub_Initialize(m_hPlatformLayer, &m_tConfiguration);
        ThrowIfBad(uStatus, "Could not initialize the proxy stub.");
    }
    catch (StatusCodeException e)
    {
        throw;
    }
}

static void Copy(std::vector<std::string> src, OpcUa_StringA** pStrings, OpcUa_UInt32* pNoOfStrings)
{
    OpcUa_StatusCode uStatus = OpcUa_Good;

    try
    {
        *pStrings = NULL;
        *pNoOfStrings = src.size();

        int iLength = src.size()*sizeof(OpcUa_StringA*);
        *pStrings = (OpcUa_StringA*)OpcUa_Alloc(iLength);
        ThrowIfAllocFailed(*pStrings);
        OpcUa_MemSet(*pStrings, 0, iLength);

        for (unsigned int ii = 0; ii < src.size(); ii++)
        {
            iLength = src[ii].size()+1;
            (*pStrings)[ii] = (OpcUa_StringA)OpcUa_Alloc(iLength);
            ThrowIfAllocFailed((*pStrings)[ii]);
            strcpy_s((*pStrings)[ii], iLength, src[ii].c_str());
        }
    }
    catch (StatusCodeException e)
    {
        if (*pStrings != NULL)
        {
            for (unsigned int ii = 0; ii < *pNoOfStrings; ii++)
            {
                OpcUa_Free((*pStrings)[ii]);
            }

            OpcUa_Free(*pStrings);
            *pStrings = NULL;
            *pNoOfStrings = 0;
        }

        throw;
    }
}

std::string Application::CreateCertificate(
    std::string storePath,
    std::string applicationName,
    std::string applicationUri,
    std::string subjectName,
    std::string organization,
    std::vector<std::string> domainNames,
    unsigned short keySize,
    unsigned short lifetimeInMonths,
	bool isCA,
	bool reuseKey,
    std::string existingKeyFilePath,
	bool usePEMFormat,
    std::string issuerKeyFilePath,
    std::string issuerKeyPassword,
    std::string password,
    std::string* publicKeyFilePath,
    std::string* privateKeyFilePath)
{	
    OpcUa_StatusCode uStatus = OpcUa_Good;
    OpcUa_ByteString tCertificate;
    OpcUa_Key tPrivateKey;
    OpcUa_StringA sThumbprint = NULL;
    std::string thumbprint;
    OpcUa_StringA* pDomainNames = NULL;
    OpcUa_UInt32 uNoOfDomainNames = 0;
    OpcUa_StringA sPublicKeyFilePath = NULL;
    OpcUa_StringA sPrivateKeyFilePath = NULL;
    OpcUa_ByteString tIssuerCertificate;
    OpcUa_Key tIssuerPrivateKey;

    try
    {
        OpcUa_ByteString_Initialize(&tIssuerCertificate);
        OpcUa_Key_Initialize(&tIssuerPrivateKey);
        OpcUa_ByteString_Initialize(&tCertificate);
        OpcUa_Key_Initialize(&tPrivateKey);
        Copy(domainNames, &pDomainNames, &uNoOfDomainNames);

		if (!issuerKeyFilePath.empty())
		{
            uStatus = OpcUa_Certificate_LoadPrivateKeyFromFile(
                (OpcUa_StringA)issuerKeyFilePath.c_str(),
                OpcUa_Crypto_Encoding_PKCS12,
				(OpcUa_StringA)issuerKeyPassword.c_str(),
                &tIssuerCertificate,
                &tIssuerPrivateKey);

			ThrowIfBad(uStatus, "Could not load private key for issuer.");
		}

		if (reuseKey)
		{
			if (existingKeyFilePath.empty())
			{
				ThrowIfBad(uStatus, "Need a path to the existing certificate.");
			}

			if (usePEMFormat)
			{
				uStatus = OpcUa_ReadFile(
					(OpcUa_StringA)existingKeyFilePath.c_str(),
					&tCertificate);

				ThrowIfBad(uStatus, "Could not load existing certificate.");
			}
			else
			{
				uStatus = OpcUa_Certificate_LoadPrivateKeyFromFile(
					(OpcUa_StringA)existingKeyFilePath.c_str(),
					OpcUa_Crypto_Encoding_PKCS12,
					(OpcUa_StringA)issuerKeyPassword.c_str(),
					&tCertificate,
					&tPrivateKey);

				ThrowIfBad(uStatus, "Could not load existing certificate.");
			}
		}

		uStatus = OpcUa_Certificate_Create(
			(OpcUa_StringA)storePath.c_str(),
			(OpcUa_StringA)applicationName.c_str(),
			(OpcUa_StringA)applicationUri.c_str(),
			(OpcUa_StringA)organization.c_str(),
			(OpcUa_StringA)subjectName.c_str(),
			uNoOfDomainNames,
			pDomainNames,
			0,
			keySize,
			lifetimeInMonths,
			(isCA)?1:0,
			(reuseKey)?1:0,
			(usePEMFormat)?OpcUa_Crypto_Encoding_PEM:OpcUa_Crypto_Encoding_PKCS12,
            &tIssuerCertificate,
            &tIssuerPrivateKey,
			(OpcUa_StringA)password.c_str(),
			&tCertificate,
			&sPublicKeyFilePath,
			&tPrivateKey,
			&sPrivateKeyFilePath);

		ThrowIfBad(uStatus, "Could not issue a new certificate.");

		uStatus = OpcUa_Certificate_GetThumbprint(&tCertificate, &sThumbprint);
        ThrowIfBad(uStatus, "Could not get thumbprint for the new self-signed certificate.");
        thumbprint = sThumbprint;
        *publicKeyFilePath = sPublicKeyFilePath;
        *privateKeyFilePath = sPrivateKeyFilePath;

        for (unsigned int ii = 0; ii < uNoOfDomainNames; ii++)
        {
            OpcUa_Free(pDomainNames[ii]);
        }

        OpcUa_Free(pDomainNames);
        OpcUa_ByteString_Clear(&tCertificate);
        OpcUa_Key_Clear(&tPrivateKey);
        OpcUa_Free(sThumbprint);
        OpcUa_Free(sPublicKeyFilePath);
        OpcUa_Free(sPrivateKeyFilePath);

        return thumbprint;
    }
    catch (StatusCodeException e)
    {
        for (unsigned int ii = 0; ii < uNoOfDomainNames; ii++)
        {
            OpcUa_Free(pDomainNames[ii]);
        }

        OpcUa_Free(pDomainNames);
        OpcUa_ByteString_Clear(&tCertificate);
        OpcUa_Key_Clear(&tPrivateKey);
        OpcUa_Free(sThumbprint);
        throw;
    }
}


// Issues a new certificate.
std::string Application::Issue(
	std::string storePath,
	std::string applicationName,
	std::string applicationUri,
	std::string subjectName,
	std::string organization,
	std::vector<std::string> domainNames,
	unsigned short keySize,
	unsigned short lifetimeInMonths,
	std::string issuerKeyFilePath,
	std::string issuerKeyPassword,
	std::string publicKeyFilePath,
	std::string privateKeyFilePath,
	std::string privateKeyPassword,
	bool isCA,
	bool reuseKey,
	bool usePEMFormat,
	std::string password,
	std::string* newPublicKeyFilePath,
	std::string* newPrivateKeyFilePath)
{
    OpcUa_StatusCode uStatus = OpcUa_Good;
    OpcUa_ByteString tCertificate;
    OpcUa_Key tPrivateKey;
    OpcUa_StringA sThumbprint = NULL;
    std::string thumbprint;
    OpcUa_StringA* pDomainNames = NULL;
    OpcUa_UInt32 uNoOfDomainNames = 0;
    OpcUa_StringA sPublicKeyFilePath = NULL;
    OpcUa_StringA sPrivateKeyFilePath = NULL;
    OpcUa_ByteString tIssuerCertificate;
    OpcUa_Key tIssuerPrivateKey;

    try
    {
        OpcUa_ByteString_Initialize(&tIssuerCertificate);
        OpcUa_Key_Initialize(&tIssuerPrivateKey);
        OpcUa_ByteString_Initialize(&tCertificate);
        OpcUa_Key_Initialize(&tPrivateKey);
        Copy(domainNames, &pDomainNames, &uNoOfDomainNames);

		if (!issuerKeyFilePath.empty())
		{
            uStatus = OpcUa_Certificate_LoadPrivateKeyFromFile(
                (OpcUa_StringA)issuerKeyFilePath.c_str(),
                OpcUa_Crypto_Encoding_PKCS12,
				(OpcUa_StringA)issuerKeyPassword.c_str(),
                &tIssuerCertificate,
                &tIssuerPrivateKey);

			ThrowIfBad(uStatus, "Could not load private key for issuer.");
		}

		if (reuseKey)
		{
			if (publicKeyFilePath.empty())
			{
				ThrowIfBad(uStatus, "Need a path to the existing certificate.");
			}

			if (usePEMFormat)
			{
				uStatus = OpcUa_ReadFile(
					(OpcUa_StringA)publicKeyFilePath.c_str(),
					&tCertificate);

				ThrowIfBad(uStatus, "Could not load existing certificate.");
			}
			else
			{
				uStatus = OpcUa_Certificate_LoadPrivateKeyFromFile(
					(OpcUa_StringA)privateKeyFilePath.c_str(),
					OpcUa_Crypto_Encoding_PKCS12,
					(OpcUa_StringA)privateKeyPassword.c_str(),
					&tCertificate,
					&tPrivateKey);

				ThrowIfBad(uStatus, "Could not load existing certificate.");
			}
		}

		uStatus = OpcUa_Certificate_Create(
			(OpcUa_StringA)storePath.c_str(),
			(OpcUa_StringA)applicationName.c_str(),
			(OpcUa_StringA)applicationUri.c_str(),
			(OpcUa_StringA)organization.c_str(),
			(OpcUa_StringA)subjectName.c_str(),
			uNoOfDomainNames,
			pDomainNames,
			0,
			keySize,
			lifetimeInMonths,
			(isCA)?1:0,
			(reuseKey)?1:0,
			(usePEMFormat)?OpcUa_Crypto_Encoding_PEM:OpcUa_Crypto_Encoding_PKCS12,
            &tIssuerCertificate,
            &tIssuerPrivateKey,
			(OpcUa_StringA)password.c_str(),
			&tCertificate,
			&sPublicKeyFilePath,
			&tPrivateKey,
			&sPrivateKeyFilePath);

		ThrowIfBad(uStatus, "Could not issue a new certificate.");

		uStatus = OpcUa_Certificate_GetThumbprint(&tCertificate, &sThumbprint);
        ThrowIfBad(uStatus, "Could not get thumbprint for the new self-signed certificate.");
        thumbprint = sThumbprint;
        *newPublicKeyFilePath = sPublicKeyFilePath;
        *newPrivateKeyFilePath = sPrivateKeyFilePath;

        for (unsigned int ii = 0; ii < uNoOfDomainNames; ii++)
        {
            OpcUa_Free(pDomainNames[ii]);
        }

        OpcUa_Free(pDomainNames);
        OpcUa_ByteString_Clear(&tCertificate);
        OpcUa_Key_Clear(&tPrivateKey);
        OpcUa_Free(sThumbprint);
        OpcUa_Free(sPublicKeyFilePath);
        OpcUa_Free(sPrivateKeyFilePath);

        return thumbprint;
    }
    catch (StatusCodeException e)
    {
        for (unsigned int ii = 0; ii < uNoOfDomainNames; ii++)
        {
            OpcUa_Free(pDomainNames[ii]);
        }

        OpcUa_Free(pDomainNames);
        OpcUa_ByteString_Clear(&tCertificate);
        OpcUa_Key_Clear(&tPrivateKey);
        OpcUa_Free(sThumbprint);
        throw;
    }
}

// Revokes a certificate.
std::string Application::Revoke(
	std::string storePath,
	std::string publicKeyFilePath,
	std::string issuerKeyFilePath,
	std::string issuerKeyPassword,
	bool unrevoke)
{
    OpcUa_StatusCode uStatus = OpcUa_Good;
    OpcUa_ByteString tCertificate;
    OpcUa_ByteString tIssuerPrivateKey;
    OpcUa_StringA sCrlFilePath = NULL;
	std::string newCrlFilePath;

    try
    {
        OpcUa_ByteString_Initialize(&tCertificate);
        OpcUa_ByteString_Initialize(&tIssuerPrivateKey);

		// validate arguments.
		if (storePath.empty() || GetFileAttributes(storePath.c_str()) == INVALID_FILE_ATTRIBUTES)
		{
			ThrowIfBad(OpcUa_BadInvalidArgument, "The store path is missing or not accessible.");
		}

		if (issuerKeyFilePath.empty() || GetFileAttributes(issuerKeyFilePath.c_str()) == INVALID_FILE_ATTRIBUTES)
		{
			ThrowIfBad(OpcUa_BadInvalidArgument, "The issuer key file is missing or not accessible.");
		}

		if (publicKeyFilePath.empty() || GetFileAttributes(publicKeyFilePath.c_str()) == INVALID_FILE_ATTRIBUTES)
		{
			ThrowIfBad(OpcUa_BadInvalidArgument, "The public key file is missing or not accessible.");
		}

		// read the certificate.
		uStatus = OpcUa_ReadFile(
			(OpcUa_StringA)publicKeyFilePath.c_str(),
			&tCertificate);

		ThrowIfBad(uStatus, "Could not load the certificate to revoke.");

		// read the issuer private key.
		uStatus = OpcUa_ReadFile(
			(OpcUa_StringA)issuerKeyFilePath.c_str(),
			&tIssuerPrivateKey);

		ThrowIfBad(uStatus, "Could not load the issuer key file.");

		// revoke the certificate.
		uStatus = OpcUa_Certificate_Revoke(
			(OpcUa_StringA)storePath.c_str(),
			&tCertificate,
            &tIssuerPrivateKey,
			(OpcUa_StringA)issuerKeyPassword.c_str(),
			unrevoke,
			&sCrlFilePath);

		ThrowIfBad(uStatus, "Could not revoke the certificate.");

		// return the CRL file path.
        newCrlFilePath = sCrlFilePath;

		// clean up.
        OpcUa_ByteString_Clear(&tCertificate);
        OpcUa_ByteString_Clear(&tIssuerPrivateKey);
        OpcUa_Free(sCrlFilePath);
	}
    catch (StatusCodeException e)
    {
        OpcUa_ByteString_Clear(&tCertificate);
        OpcUa_ByteString_Clear(&tIssuerPrivateKey);
        OpcUa_Free(sCrlFilePath);
        throw;
    }

	return newCrlFilePath;
}

// Converts a certificate.
void Application::Install(
	std::string storePath,
	std::string publicKeyFilePath,
	std::string privateKeyFilePath,
	std::string privateKeyPassword,
	bool usePEMFormat,
	std::string password,
	std::string* newPublicKeyFilePath,
	std::string* newPrivateKeyFilePath)
{
    OpcUa_StatusCode uStatus = OpcUa_Good;
    OpcUa_ByteString tCertificate;
    OpcUa_ByteString tPrivateKey;
    OpcUa_StringA sNewPublicKeyFilePath = NULL;
    OpcUa_StringA sNewPrivateKeyFilePath = NULL;

    try
    {
        OpcUa_ByteString_Initialize(&tCertificate);
        OpcUa_ByteString_Initialize(&tPrivateKey);

		// validate arguments.
		if (storePath.empty() || GetFileAttributes(storePath.c_str()) == INVALID_FILE_ATTRIBUTES)
		{
			ThrowIfBad(OpcUa_BadInvalidArgument, "The store path is missing or not accessible.");
		}

		if (publicKeyFilePath.empty() || GetFileAttributes(publicKeyFilePath.c_str()) == INVALID_FILE_ATTRIBUTES)
		{
			ThrowIfBad(OpcUa_BadInvalidArgument, "The public key file is missing or not accessible.");
		}

		if (privateKeyFilePath.empty() || GetFileAttributes(privateKeyFilePath.c_str()) == INVALID_FILE_ATTRIBUTES)
		{
			ThrowIfBad(OpcUa_BadInvalidArgument, "The private key file is missing or not accessible.");
		}

		// read the certificate.
		uStatus = OpcUa_ReadFile(
			(OpcUa_StringA)publicKeyFilePath.c_str(),
			&tCertificate);

		ThrowIfBad(uStatus, "Could not load the certificate to revoke.");

		// read the issuer private key.
		uStatus = OpcUa_ReadFile(
			(OpcUa_StringA)privateKeyFilePath.c_str(),
			&tPrivateKey);

		ThrowIfBad(uStatus, "Could not load the issuer key file.");

		bool inputIsPEM = false;

		if (_strnicmp(privateKeyFilePath.c_str() + privateKeyFilePath.size()-4, ".pem", 4) == 0)
		{
			inputIsPEM = true;
		}

		// revoke the certificate.
		uStatus = OpcUa_Certificate_Install(
			(OpcUa_StringA)storePath.c_str(),
			&tCertificate,
            &tPrivateKey,
			(OpcUa_StringA)privateKeyPassword.c_str(),
			(inputIsPEM)?OpcUa_Crypto_Encoding_PEM:OpcUa_Crypto_Encoding_PKCS12,
			(OpcUa_StringA)password.c_str(),
			(usePEMFormat)?OpcUa_Crypto_Encoding_PEM:OpcUa_Crypto_Encoding_PKCS12,
			&sNewPublicKeyFilePath,
			&sNewPrivateKeyFilePath);

		// return the new file path.
        *newPublicKeyFilePath = sNewPublicKeyFilePath;
        *newPrivateKeyFilePath = sNewPrivateKeyFilePath;

        OpcUa_ByteString_Clear(&tCertificate);
        OpcUa_ByteString_Clear(&tPrivateKey);
        OpcUa_Free(sNewPublicKeyFilePath);
        OpcUa_Free(sNewPrivateKeyFilePath);
	}
    catch (StatusCodeException e)
    {
        OpcUa_ByteString_Clear(&tCertificate);
        OpcUa_ByteString_Clear(&tPrivateKey);
        OpcUa_Free(sNewPublicKeyFilePath);
        OpcUa_Free(sNewPrivateKeyFilePath);
        throw;
    }
}
