/* System */
#define WIN32_LEAN_AND_MEAN
#include <targetver.h>
#include <windows.h>
#include <stdio.h>
#include <errno.h>
#include <conio.h>
#include <crtdbg.h>
#include <direct.h>
#include <string>
#include <map>
#include <vector>
#include <opcua.h>

/* vld */
#if UATESTCLIENT_USE_VISUAL_LEAK_DETECTOR
#include <vld.h>
#endif /* STLS_USE_VISUAL_LEAK_DETECTOR */

#include "Application.h"
#include "StatusCodeException.h"

// Reads the arguments from stdin.
int ReadArgumentsFromFile(FILE* pFile, std::map<std::string,std::string>* arguments)
{
    CHAR sBuffer[4096];
    std::string flag;
    std::string value;
    char* pResult = NULL;
    bool readingValue = false;

    do
    {
        // get the next block.
        memset(sBuffer, 0, sizeof(sBuffer));
        pResult = fgets(sBuffer, sizeof(sBuffer), pFile);

        if (pResult == NULL)
        {
            break;
        }

        for (char* pPos = pResult; *pPos != 0; pPos++)
        {
			if (*pPos == '\r')
			{
                continue;
			}

            // check for end of line.
            if (*pPos == '\n')
            {
                // blank line means end of command.
                if (flag.empty())
                {
                    return 0;
                }

                // save the argument value.
                (*arguments)[flag] = value;
                flag.clear();
                value.clear();
                readingValue = false;
                continue;
            }

            // skip whitespace until encountering the argument.
            if (!readingValue)
            {
                if (isspace(*pPos))
                {
                    if (!flag.empty())
                    {
                        readingValue = true;
                    }

                    continue;
                }

                flag.push_back(*pPos);
                continue;
            }

            // skip whitespace until encountering the value.
            else
            {
                if (isspace(*pPos))
                {
                    if (value.empty())
                    {
                        continue;
                    }
                }

                value.push_back(*pPos);
                continue;
            }
        }
    }
    while (pResult != NULL);

    return 0;
}

// Reads the arguments from command line.
int ReadArgumentsFromCommandLine(int argc, char* argv[], std::map<std::string,std::string>* arguments)
{
    std::string flag;
    bool readingValue = false;

    for (int ii = 1; ii < argc; ii++)
    {
        if (!readingValue)
        {
            flag = argv[ii];
            readingValue = true;

			if (flag[0] != '-')
			{		
				std::string message = "Unrecognized Parameter: ";
				message += flag;
				throw StatusCodeException(OpcUa_BadInvalidArgument, message);
			}

			if (flag == "-?")
			{
				(*arguments)[flag] = "";
				return 0;
			}
        }
        else
        {
            (*arguments)[flag] = argv[ii];
            flag.clear();
            readingValue = false;
        }
    }

    return 0;
}

static std::string IsArgSpecified(std::map<std::string,std::string>& arguments, std::string longForm, std::string shortForm)
{
	std::string arg;
    std::map<std::string,std::string>::iterator it;

	if ((it = arguments.find(longForm)) != arguments.end())
	{
		arg = it->second;
	}

	else if ((it = arguments.find(shortForm)) != arguments.end())
	{
		arg = it->second;
	}

	arguments.erase(longForm);
	arguments.erase(shortForm);

	return arg;
}

int main(int argc, char* argv[])
{
    std::map<std::string,std::string> arguments;
    std::map<std::string,std::string>::iterator it;

    std::string command;   
    std::string storePath;                
    std::string applicationName;          
    std::string applicationUri;           
    std::string subjectName;              
    std::string organization;             
    std::vector<std::string> domainNames;       
    std::string password;           
    std::string issuerKeyFilePath;        
    std::string issuerKeyPassword;       
    unsigned short keySize = 1024;        
    unsigned short lifetimeInMonths = 60;         
    std::string publicKeyFilePath;
    std::string privateKeyFilePath;   
    std::string privateKeyPassword;      
	bool isCA = false;                    
	bool usePEMFormat = false;            
	bool reuseKey = false;     
	std::string arg;  
	FILE* pFile = NULL; 
	bool freeOutFile = false;

    try
    {
		pFile = stdout;

        // read the arguments from stdin.
        if (argc <= 1)
        {
            ReadArgumentsFromFile(stdin, &arguments);
        }

        // read the arguments from command line.
        else
        {
            ReadArgumentsFromCommandLine(argc, argv, &arguments);

			if (!(arg = IsArgSpecified(arguments, "-file", "-f")).empty())
			{
				arguments.clear();
				std::string tempFilePath = arg;

				if (fopen_s(&pFile, tempFilePath.c_str(), "rb") != 0)
				{
					pFile = stdout;
					std::string message = "Could not open input file: ";
					message += tempFilePath;
					throw StatusCodeException(OpcUa_BadInvalidArgument, message);
				}

				ReadArgumentsFromFile(pFile, &arguments);
				fclose(pFile);
				pFile = NULL;

				if (fopen_s(&pFile, tempFilePath.c_str(), "wb") != 0)
				{
					pFile = stdout;
					std::string message = "Could not open output file: ";
					message += tempFilePath;
					throw StatusCodeException(OpcUa_BadInvalidArgument, message);
				}

				freeOutFile = true;
			}
        }

		// check if help requested.
		if ((it = arguments.find("-?")) != arguments.end())
		{		
			fputs("-command or -cmd <issue | revoke | unrevoke | install> The action to perform (default = issue).\r\n", pFile);	
			fputs("-storePath or -sp <filepath>                The directory of the certificate store (mandatory, must be writeable).\r\n", pFile);
			fputs("-applicationName or -an <name>              The name of the application (mandatory).\r\n", pFile);
			fputs("-applicationUri or -au <uri>                The URI for the appplication (optional).\r\n", pFile);
			fputs("-subjectName or -sn <DN>                    The distinguished subject name, fields seperated by a / (i.e. CN=Hello/O=World).\r\n", pFile);
			fputs("-organization or -o <name>                  The organization (optional).\r\n", pFile);
			fputs("-domainNames or -dn <name>,<name>           A list of domain names seperated by commas (optional)\r\n", pFile);
			fputs("-password or -pw <password>                 The password for the new private key file (optional).\r\n", pFile);
			fputs("-issuerKeyFilePath or -ikf <filepath>       The path to the issuer private key file (optional).\r\n", pFile);
			fputs("-issuerKeyPassword or -ikp <password>       The password for the issuer private key file (optional).\r\n", pFile);
			fputs("-keySize or -ks  <bits>                     The size of key as a multiple of 1024 (default = 1024).\r\n", pFile);
			fputs("-lifetimeInMonths or -lm <months>           The lifetime in months (default = 60).\r\n", pFile);
			fputs("-publicKeyFilePath or -pbf <filepath>       The path to the certificate to renew or revoke (a DER file).\r\n", pFile);
			fputs("-privateKeyFilePath or -pvf <filepath>      The path to an existing private key to reuse or convert.\r\n", pFile);
			fputs("-privateKeyPassword or -pvp <password>      The password for the private key.\r\n", pFile);
			fputs("-reuseKey or -rk <true | false>             Whether to reuse an existing public key (default = false).\r\n", pFile);
			fputs("-ca <true | false>                          Whether to create a CA certificate (default = false).\r\n", pFile);
			fputs("-pem <true | false>                         Whether to output in the PEM format (default = PFX).\r\n", pFile);
			fputs("\r\n", pFile);
			fputs("\r\n", pFile);
			fputs("Create a self-signed Application Certificate: -cmd issue -sp . -sn MyApp\r\n", pFile);
			fputs("Create a CA Certificate: -cmd issue -sp . -an MyCA -ca true\r\n", pFile);
			fputs("Issue an Application Certificate: -cmd issue -sp . -an MyApp -ikf CaKeyFile -ikp CaPassword\r\n", pFile);
			fputs("Renew a Certificate: -cmd issue -sp . -pbf MyCertFile -ikf CaKeyFile -ikp CaPassword\r\n", pFile);
			fputs("Revoke a Certificate: -cmd revoke -sp . -pbf MyCertFile -ikf CaKeyFile -ikp CaPassword\r\n", pFile);
			fputs("Unrevoke a Certificate: -cmd unrevoke -sp . -pbf MyCertFile -ikf CaKeyFile -ikp CaPassword\r\n", pFile);
			fputs("Convert key format: -cmd convert true -pw newpassword -pvf MyKeyFile -pvp oldpassword -pem true\r\n", pFile);

	        return 0;
		}
          
        // set the argument values.
		if (!(arg = IsArgSpecified(arguments, "-command", "-cmd")).empty())
        {
            command = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-storePath", "-sp")).empty())
        {
            storePath = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-applicationName", "-an")).empty())
        {
            applicationName = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-applicationUri", "-au")).empty())
        {
            applicationUri = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-subjectName", "-sn")).empty())
        {
            subjectName = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-organization", "-o")).empty())
        {
            organization = arg;
        }    

		if (!(arg = IsArgSpecified(arguments, "-domainNames", "-dn")).empty())
        {
            int start = 0;
            int index = arg.find(",");

            while (index != std::string::npos)
            {
                domainNames.push_back(arg.substr(start, index-start));
                start = index+1;
                index = arg.find(",", start);
            }
                
            domainNames.push_back(arg.substr(start));
        }
		
		if (!(arg = IsArgSpecified(arguments, "-password", "-pw")).empty())
        {
            password = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-issuerKeyFilePath", "-ikf")).empty())
        {
            issuerKeyFilePath = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-issuerKeyPassword", "-ikp")).empty())
        {
            issuerKeyPassword = arg;
        }
    
		if (!(arg = IsArgSpecified(arguments, "-keySize", "-ks")).empty())
        {
            keySize = atoi(arg.c_str());
        }

		if (!(arg = IsArgSpecified(arguments, "-lifetimeInMonths", "-lm")).empty())
        {
            lifetimeInMonths = atoi(arg.c_str());
        }

		if (!(arg = IsArgSpecified(arguments, "-publicKeyFilePath", "-pbf")).empty())
        {
            publicKeyFilePath = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-privateKeyFilePath", "-pvf")).empty())
        {
            privateKeyFilePath = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-privateKeyPassword", "-pvp")).empty())
        {
            privateKeyPassword = arg;
        }

		if (!(arg = IsArgSpecified(arguments, "-ca", "-ca")).empty())
        {
			isCA = (arg == "true")?true:false;
        }

		if (!(arg = IsArgSpecified(arguments, "-pem", "-pem")).empty())
        {
			usePEMFormat = (arg == "true")?true:false;
        }

		if (!(arg = IsArgSpecified(arguments, "-reuseKey", "-rk")).empty())
        {
			reuseKey = (arg == "true")?true:false;
        }

		if (arguments.size() > 0)
		{
            fputs("-error Unprocessed arguments exist possible syntax error: ", pFile);
 
			for (std::map<std::string,std::string>::iterator ii = arguments.begin(); ii != arguments.end(); ++ii)  
			{  
				fputs(ii->first.c_str() , pFile);
				fputs(" ", pFile);
			}  

            fputs("\r\n", pFile);
	        return 0;
		}

        // create the store.
        if (_mkdir(storePath.c_str()) != 0 && errno != EEXIST)
        {
            fputs("-error Could not access certificate store: ", pFile);
            fputs(storePath.c_str(), pFile);
            fputs("\r\n", pFile);
	        return 0;
        }

        Application application;
        application.Initialize();

        // create a new certificate.
		if (command.empty() || command == "issue")
		{   
			std::string newPublicKeyFilePath;
			std::string newPrivateKeyFilePath;

			std::string thumbprint = application.Issue(
				storePath,
				applicationName,
				applicationUri,
				subjectName,
				organization,
				domainNames,
				keySize,
				lifetimeInMonths,
				issuerKeyFilePath,
				issuerKeyPassword,
				publicKeyFilePath,
				privateKeyFilePath,
				privateKeyPassword,
				isCA,
				reuseKey,
				usePEMFormat,
				password,
				&newPublicKeyFilePath,
				&newPrivateKeyFilePath);

			application.Uninitialize();

			// return the thumbprint.
			fputs("-thumbprint ", pFile);
			fputs(thumbprint.c_str(), pFile);
			fputs("\r\n", pFile);

			// return the publicKeyFilePath.
			fputs("-publicKeyFilePath ", pFile);
			fputs(newPublicKeyFilePath.c_str(), pFile);
			fputs("\r\n", pFile);

			// return the privateKeyFilePath.
			fputs("-privateKeyFilePath ", pFile);
			fputs(newPrivateKeyFilePath.c_str(), pFile);
			fputs("\r\n", pFile);

			return 0;
		}

		// revoke a certificate
		if (command == "revoke" || command == "unrevoke")
		{
			std::string newCrlFilePath = application.Revoke(
				storePath,
				publicKeyFilePath,
				issuerKeyFilePath,
				issuerKeyPassword,
				command == "unrevoke");

			// return the crlFilePath.
			fputs("-crlFilePath ", pFile);
			fputs(newCrlFilePath.c_str(), pFile);
			fputs("\r\n", pFile);

			return 0;
		}

		// convert a certificate
		if (command == "install")
		{
			std::string newPublicKeyFilePath;
			std::string newPrivateKeyFilePath;

			application.Install(
				storePath,
				publicKeyFilePath,
				privateKeyFilePath,
				privateKeyPassword,
				usePEMFormat,
				password,
				&newPublicKeyFilePath,
				&newPrivateKeyFilePath);

			// return the publicKeyFilePath.
			fputs("-publicKeyFilePath ", pFile);
			fputs(newPublicKeyFilePath.c_str(), pFile);
			fputs("\r\n", pFile);

			// return the privateKeyFilePath.
			fputs("-privateKeyFilePath ", pFile);
			fputs(newPrivateKeyFilePath.c_str(), pFile);
			fputs("\r\n", pFile);

			return 0;
		}
				
		std::string message = "Unsupported Command: ";
		message += command;
		throw StatusCodeException(OpcUa_BadInvalidArgument, message);
    }
    catch (StatusCodeException e)
    {
        fputs("-error ", pFile);
        fputs(e.GetMessage().c_str(), pFile);
        fputs("\r\n", pFile);
    }
    catch (...)
    {
        fputs("-error ", pFile);
        fputs("Unhandled exception.", pFile);
        fputs("\r\n", pFile);
    }

	if (freeOutFile)
	{
		fclose(pFile);
	}

	return 0;
}
