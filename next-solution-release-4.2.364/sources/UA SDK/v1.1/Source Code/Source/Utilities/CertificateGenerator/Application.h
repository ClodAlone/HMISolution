/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

#pragma once

// Stores information associated with a UA application instance.
class Application
{
public:

    // Constructor
	Application(void);

    // Destructor
	~Application(void);

	// Initializes the stack and application.
	virtual void Initialize(void);

	// Frees all resources used by the stack and application.
	virtual void Uninitialize(void);

    // Creates a new certificate.
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
		std::string* privateKeyFilePath);
	
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
		std::string* newPrivateKeyFilePath);
	
    // Revokes a certificate.
    std::string Application::Revoke(
		std::string storePath,
		std::string publicKeyFilePath,
		std::string issuerKeyFilePath,
		std::string issuerKeyPassword,
		bool unrevoke);
    
	// Installs or converts a certificate.
    void Application::Install(
		std::string storePath,
		std::string publicKeyFilePath,
		std::string privateKeyFilePath,
		std::string privateKeyPassword,
		bool usePEMFormat,
		std::string password,
		std::string* newPublicKeyFilePath,
		std::string* newPrivateKeyFilePath);

private:

    // Logs a message.
    void Log(OpcUa_UInt32 uTraceLevel, OpcUa_CharA* sFormat, ...);

	OpcUa_Handle m_hPlatformLayer;
	OpcUa_ProxyStubConfiguration m_tConfiguration;
};
