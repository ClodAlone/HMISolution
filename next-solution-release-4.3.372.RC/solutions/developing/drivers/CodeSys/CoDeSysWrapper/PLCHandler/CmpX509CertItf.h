 /**
 * <interfacename>CmpX509Cert</interfacename>
 * <description> 
 *	<p>This interface provides functionality to handle and verify X.509 Certificates</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */

	
	
#ifndef _CMPX509CERTITF_H_
#define _CMPX509CERTITF_H_

#include "CmpStd.h"

 

 



#include "CmpCryptoItf.h"
#include "SysSocketItf.h"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Enables or disables CRL checking while verifying a certificat. (DEFAULT: Disabled)</description>
 */
#define X509CERTKEY_INT_VERIFY_USE_CRL					"VerifyUseCRL"
#ifndef X509CERTVALUE_INT_VERIFY_USE_CRL
	#define X509CERTVALUE_INT_VERIFY_USE_CRL			0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Configures the minim allowed key size (in bits) for newly created asymmetric key. Refer to NIST or BSI suggestions.</description>
 */
#define X509CERTKEY_INT_MIN_ASYMMETRIC_KEY_SIZE			"MinAsymmetricKeySize"
#ifndef X509CERTVALUE_INT_MIN_ASYMMETRIC_KEY_SIZE
	#define X509CERTVALUE_INT_MIN_ASYMMETRIC_KEY_SIZE	2048
#endif

/**
 * <category>Static defines</category>
 * <description>This define is used for the PlcShell command "cert-genselfsigned" to specify the number of days until the certificate will expire (if no argument is specified)</description>
 */
#ifndef X509CERT_STANDARD_DAYS_FOR_CERT
	#define X509CERT_STANDARD_DAYS_FOR_CERT				365
#endif

/**
 * <category>Static defines</category>
 * <description>This define is used for the number of days until the certificate will expire for selfsigned certificates</description>
 */
#ifndef X509CERT_STANDARD_DAYS_FOR_SELFIGNEDCERT
	#define X509CERT_STANDARD_DAYS_FOR_SELFIGNEDCERT	30
#endif

#define X509CERT_MILLISECONDS_PER_DAY					(24 * 60 * 60 * 1000)

/**
 * <category>SecurityIDs</category>
 * <description>Security IDs of this component</description>
 */
typedef enum
{
    CMPX509CERT_SECURITY_ID_DIFFIEHELLMANPARAMETER = 0
} CmpX509Cert_SecurityIDs;

/**
 * <category>SecuritySettings</category>
 * <description>Security settings of the X.509 certificate management to create diffie hellman parameters. Can be selected in CmpSecurityManager.
 *	NOTE:
 *		Security modes must be ordered in an descending sorting, that the setting with the highest security level is found at the beginning of the list!
 * </description>
 */
#define CMPX509CERT_KEY_STRING_DIFFIEHELLMANPARAMETER			CMPSETTINGS_CATEGORY_SECURITY ".DiffieHellmanParameter_Generate"
#define CMPX509CERT_KEY_STRING_DIFFIEHELLMANPARAMETER_DESC		"Communication mode for online communication"
	
#define CMPX509CERT_VALUE_DIFFIEHELLMANPARAMETER_ON_STARTUP		"ON_STARTUP"
#define CMPX509CERT_VALUE_DIFFIEHELLMANPARAMETER_ON_REQUEST		"ON_REQUEST"

#ifndef CMPX509CERT_VALUE_DIFFIEHELLMANPARAMETER_DEFAULT
	#define CMPX509CERT_VALUE_DIFFIEHELLMANPARAMETER_DEFAULT	CMPX509CERT_VALUE_DIFFIEHELLMANPARAMETER_ON_REQUEST
#endif

#define CMPX509CERT_VALUE_DIFFIEHELLMANPARAMETER_LIST			{ \
																	{1, CMPSECMAN_SFLAGS_NONE, "ON_STARTUP", "Create Diffie Hellman parameters on startup"},\
																	{0, CMPSECMAN_SFLAGS_NONE, "ON_REQUEST", "Create Diffie Hellman parameters only on request"}\
																}

/**
 * <category>Static defines</category>
 * <description>This define is used to specify the default length of the diffie hellman parameters</description>
 */
#ifndef RTS_X509_DIFFIEHELLMAN_DEFAULT_LEN
	#define RTS_X509_DIFFIEHELLMAN_DEFAULT_LEN		2048
#endif

/**
 * <category>Online services</category>
 */ 
#define SRV_X509_IMPORT_CERT        0x01
#define SRV_X509_EXPORT_CERT        0x02
#define SRV_X509_DELETE_CERT        0x03
#define SRV_X509_MOVE_CERT          0x04
#define SRV_X509_LIST_CERTS         0x05
#define SRV_X509_LIST_USECASES      0x06
#define SRV_X509_CREATE_SELFSIGNED  0x07
#define SRV_X509_CREATE_REQUEST     0x08
#define SRV_X509_GET_STATUS         0x09

/**
 * <category>Online service tags</category>
 */
#define TAG_X509_TRUSTLEVEL		    0x01
#define TAG_X509_BACKEND            0x02
#define TAG_X509_USECASE            0x03
#define TAG_X509_THUMBPRINT         0x04
#define TAG_X509_CERT               0x05
#define TAG_X509_CSR                0x06
#define TAG_X509_DEST_TRUSTLEVEL    0x07
#define TAG_X509_DEST_BACKEND       0x08
#define TAG_X509_KEY_LENGTH         0x09
#define TAG_X509_USECASE_NAME       0x0A
#define TAG_X509_USECASE_DESC       0x0B
#define TAG_X509_SUBJECT_ID         0x0C
#define TAG_X509_SUBJECT_VALUE      0x0D
#define TAG_X509_ALT_NAME_EMAIL     0x0E
#define TAG_X509_ALT_NAME_DNSNAME   0x0F
#define TAG_X509_ALT_NAME_URI       0x10
#define TAG_X509_ALT_NAME_IPADDRESS 0x12
#define TAG_X509_ALT_NAME_REGISTEREDID 0x13
#define TAG_X509_KEYUSAGE           0x14
#define TAG_X509_EXT_KEYUSAGE       0x15
#define TAG_X509_USECASE_COMPONENT  0x16
#define TAG_X509_DELETE_PKEY        0x17
#define TAG_X509_DAYS_VALID         0x18
#define TAG_X509_GEN_SELF_SIGNED    0x19
#define TAG_X509_GEN_CSR            0x1A
#define TAG_X509_STATUS_HANDLE      0x1B
#define TAG_X509_OPERATION          0x1C
/* Leave tags 0x22 - 0x24 empty. Use for user management. */
#define TAG_X509_RESULT             TAG_ERROR

/* Complex tags */
#define TAG_X509_USECASE_DETAILS    0x81
/* Leave TAG 0x082 empty. Is used for user management. */
 
 
/**
 * <category>Online services</category>
 * <Description>
 *	Import certificate service. This service imports the given certificate into the corresponding store of the PLC.
 *  Since this service changes the security configuration of the PLC administrator rights are needed to execute this service.
 * </Description>
 * <service name="SRV_X509_IMPORT_CERT">
 *	<Request>
 *		<tag name="TAG_X509_TRUSTLEVEL" required="required">[RTS_UI32]: Trust level where to import the certificate. See the RTSCERTTRUSTLEVEL_* defines for details.</tag>
 *		<tag name="TAG_X509_BACKEND" required="optional">[Type not yet defined]: Identifier of the backend where to store the certificate. Not used by now.</tag>
 *		<tag name="TAG_X509_CERT" required="required">Certificate in ANS.1 encoding to import.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_X509_RESULT" required="required">[RTS_UI16]: Result code of online operation</tag>
 *	</Response>
 * </service>
 */
 
/**
 * <category>Online services</category>
 * <Description>
 *	Export certificate service. This service exports a selected certificate from the PLC for use in other applications.
 * </Description>
 * <service name="SRV_X509_EXPORT_CERT">
 *	<Request>
 *		<tag name="TAG_X509_TRUSTLEVEL" required="required">[RTS_UI32]: Trust level of the certificate to export. See the RTSCERTTRUSTLEVEL_* defines for details.</tag>
 *		<tag name="TAG_X509_BACKEND" required="optional">[Type not yet defined]: Identifier of the backend of the certificate. Not used by now.</tag>
 *		<tag name="TAG_X509_THUMBPRINT" required="required">Thumbprint of certificate calculated by the runtime system.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_X509_RESULT" required="required">[RTS_UI16]: Result code of online operation</tag>
 *		<tag name="TAG_X509_CERT" required="optional">Exported certificate in ASN.1 encoding if the certificate was available.</tag>
 *	</Response>
 * </service>
 */
 
/**
 * <category>Online services</category>
 * <Description>
 *	Delete certificate service. This service deletes a certificate from the PLC.
 *  Since this service changes the security configuration of the PLC administrator rights are needed to execute this service.
 * </Description>
 * <service name="SRV_X509_DELETE_CERT">
 *	<Request>
 *		<tag name="TAG_X509_TRUSTLEVEL" required="required">[RTS_UI32]: Trust level of the certificate to delete. See the RTSCERTTRUSTLEVEL_* defines for details.</tag>
 *		<tag name="TAG_X509_BACKEND" required="optional">[Type not yet defined]: Identifier of the backend of the certificate. Not used by now.</tag>
 *		<tag name="TAG_X509_THUMBPRINT" required="required">Thumbprint of certificate calculated by the runtime system.</tag>
 *		<tag name="TAG_X509_DELETE_PKEY" required="optional">[RTS_UI32]: Flag if the private key should be deleted too. This is only used for trust level 'OWN'.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_X509_RESULT" required="required">[RTS_UI16]: Result code of online operation</tag>
 *	</Response>
 * </service>
 */
 
/**
 * <category>Online services</category>
 * <Description>
 *	Move certificate service. This service moves certificates from one place to another.
 *  Since this service changes the security configuration of the PLC administrator rights are needed to execute this service.
 * </Description>
 * <service name="SRV_X509_MOVE_CERT">
 *	<Request>
 *		<tag name="TAG_X509_TRUSTLEVEL" required="required">[RTS_UI32]: Trust level of the certificate to move. See the RTSCERTTRUSTLEVEL_* defines for details.</tag>
 *		<tag name="TAG_X509_BACKEND" required="optional">[Type not yet defined]: Identifier of the backend of the certificate. Not used by now.</tag>
 *		<tag name="TAG_X509_THUMBPRINT" required="required">Thumbprint of certificate calculated by the runtime system.</tag>
 *		<tag name="TAG_X509_DEST_TRUSTLEVEL" required="required">[RTS_UI32]: Trust level where to move the certificate to. See the RTSCERTTRUSTLEVEL_* defines for details.</tag>
 *		<tag name="TAG_X509_DEST_BACKEND" required="optional">[Type not yet defined]:Backend where to move the certificate to. Not used by now.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_X509_RESULT" required="required">[RTS_UI16]: Result code of online operation</tag>
 *	</Response>
 * </service>
 */
 
/**
 * <category>Online services</category>
 * <Description>
 *	List certificate service. This service lists all certificates from one place. Maybe not all certificates can be transmitted in one single online service.
 *  In this case the service return ERR_ENTRIES_REMAINING to indicate that more certificates are available. To get the next junk of certificates put the thumbprint of the
 *  last certificate of the last service as a starting point for the next certificates. The thumbprint tag in the response contains the thumbprint of the certificate 
 *  contained in the previous tag. These tags must always occur as a pair.
 * </Description>
 * <service name="SRV_X509_LIST_CERTS">
 *	<Request>
 *		<tag name="TAG_X509_TRUSTLEVEL" required="required">[RTS_UI32]: Trust level of the certificate to list. See the RTSCERTTRUSTLEVEL_* defines for details.</tag>
 *		<tag name="TAG_X509_BACKEND" required="optional">[Type not yet defined]: Identifier of the backend of the certificate. Not used by now.</tag>
 *		<tag name="TAG_X509_THUMBPRINT" required="optional">Thumbprint of certificate calculated by the runtime system of the last received certificate.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_X509_RESULT" required="required">[RTS_UI16]: Result code of online operation</tag>
 *		<tag name="TAG_X509_CERT" required="required">Certificate in ASN.1 format. This tag may occur multiple times.</tag>
 *		<tag name="TAG_X509_THUMBPRINT" required="required">Thumbprint of certificate calculated by the runtime system.</tag>
 *	</Response>
 * </service>
 */
 
/**
 * <category>Online services</category>
 * <Description>
 *	List certificate use service. This service return a list of all registered servers or clients that need their own certificates. This can be used to generate the needed
 *  self signed certificates or to generate the corresponding certificate requests. If a certificate is available for a dedicated use case the thumbprint of this certificate
 *  will be put the the service response. This allows to display the corresponding certificate.
 *  If the complete list of use cases can't be transmitted in one single service it will be split up. In this case the service return ERR_ENTRIES_REMAINING to indicate that more use 
 *  cases are available. The get the next junk of use cases is identified by the last usecases index.
 * </Description>
 * <service name="SRV_X509_LIST_USECASES">
 *	<Request>
 *      <tag name="TAG_X509_USECASE" required="optional">[RTS_UI32]: If the last service returned ERR_ENTRIES_REMAINING this is the ID of the last received use case.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_X509_RESULT" required="required">[RTS_UI16]: Result code of online operation</tag>
 *      <tag name="TAG_X509_USECASE_DETAILS" required="required">
 *		    <tag name="TAG_X509_USECASE" required="required">[RTS_UI32]: ID of the use case.</tag>
 *          <tag name="TAG_X509_USECASE_COMPONENT" required="required">[String]: Component which registered the use case.</tag>
 *          <tag name="TAG_X509_USECASE_NAME" required="optional">[String]: Name of the usecase. Provided by the component.</tag>
 *          <tag name="TAG_X509_USECASE_DESC" required="optional">[String]: Description of the use case. Provided by the component.</tag>
 *          <tag name="TAG_X509_SUBJECT_ID" required="optional">[String]: OID of the current subject field. Always exists with the following TAG_X509_SUBJECT_VALUE field. Sequenz is likley to be repeated</tag>
 *          <tag name="TAG_X509_SUBJECT_VALUE" required="optional">[String]: Value of the current subject field.</tag>
 *          <tag name="TAG_X509_ALT_NAME_EMAIL" required="optional">[String]: Alternative name containing an email address.</tag>
 *          <tag name="TAG_X509_ALT_NAME_DNSNAME" required="optional">[String]: Alternative name containing the DNS name.</tag>
 *          <tag name="TAG_X509_ALT_NAME_URI" required="optional">[String]: Alternative name containing the URI</tag>
 *          <tag name="TAG_X509_ALT_NAME_IPADDRESS" required="optional">[String]: Alternative name containing the IP-Address.</tag>
 *          <tag name="TAG_X509_ALT_NAME_REGISTEREDID" required="optional">[String]: Alternative name containing an OID in textual representation.</tag>
 *          <tag name="TAG_X509_KEYUSAGE" required="optional">[RTS_UI32]: Key usage of the certificate. See: RTS_KEY_USAGE_DIGITAL_* macros.</tag>
 *          <tag name="TAG_X509_EXT_KEYUSAGE" required="optional">[String]: Extended key usage identified by its textual representation. Can occur multiple times.</tag>
 *          <tag name="TAG_X509_THUMBPRINT" required="optional">Thumbprint of the certificate available for this use case. Left out if no certificate is available.</tag>
 *          <tag name="TAG_X509_GEN_SELF_SIGNED" required="optional">[Empty tag]: A self signed certificate is beeing generated at the moment.</tag>
 *          <tag name="TAG_X509_GEN_CSR" required="optional">[Empty tag]: A certificate request is beeing generated at the moment.</tag>
 *      </tag>
 *	</Response>
 * </service>
 */
 
/**
 * <category>Online services</category>
 * <Description>
 *	Generate self signed certificate service. This service is used to generate a self signed certificate for a registered use case. 
 *  This service will also generate the private key used by the new certificate.
 *  Since this service changes the security configuration of the PLC administrator rights are needed to execute this service.
 * Repeat this service 
 * </Description>
 * <service name="SRV_X509_CREATE_SELFSIGNED">
 *	<Request>
 *		<tag name="TAG_X509_USECASE" required="required">[RTS_UI32]: Usecase ID which the certificate is generated for.</tag>
 *		<tag name="TAG_X509_BACKEND" required="optional">[Type not yet defined]: Backend where to store the certificate. Not used by now.</tag>
 *      <tag name="TAG_X509_KEY_LENGTH" required="optional">[RTS_UI32]: The length of the asymmetric key pair in bits. If left out the default (see MinAsymmetricKeySize) will be used.</tag>
 *      <tag name="TAG_X509_DAYS_VALID" required="optional">[RTS_UI32]: Number of days the self signed certificate is valid.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_X509_RESULT" required="required">[RTS_UI16]: Result code of online operation</tag>
 *		<tag name="TAG_X509_STATUS_HANDLE" required="required">[RTS_UI16]: Result code of online operation</tag>
 *	</Response>
 * </service>
 */
 
/**
 * <category>Online services</category>
 * <Description>
 *	Generate certificate signing request service. This service is used to generate a certificate signing request (CSR) for a registered use case.
 *  The private key is stored on the PLC. If the certificate has been generated by a CA this certificate has to be imported to the own store.
 * </Description>
 * <service name="SRV_X509_CREATE_REQUEST">
 *	<Request>
 *		<tag name="TAG_X509_USECASE" required="required">[RTS_UI32]: Usecase ID which the CSR is generated for.</tag>
 *		<tag name="TAG_X509_BACKEND" required="optional">[Type not yet defined]: Backend where to store the private key. Not used by now.</tag>
 *      <tag name="TAG_X509_KEY_LENGTH" required="optional">[RTS_UI32]: The length of the asymmetric key pair in bits. If left out the default (see MinAsymmetricKeySize) will be used.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_X509_RESULT" required="required">[RTS_UI16]: Result code of online operation</tag>
 *		<tag name="TAG_X509_CSR" required="optional">The generated CSR in ASN.1 encoding.</tag>
 *	</Response>
 * </service>
 */
 
/**
 * <category>Online services</category>
 * <Description>
 *	Get the status of long lasting operation. Actually this is used for generating self signed certificates.
 * </Description>
 * <service name="SRV_X509_GET_STATUS">
 *	<Request>
 *      <tag name="TAG_X509_OPERATION" required="required">[RTS_UI32]: Identifier of the operation. Use the service wating for as ID.</tag>
 *		<tag name="TAG_X509_STATUS_HANDLE" required="required">[RTS_UI32]: Usecase ID which the CSR is generated for.</tag>
 *	</Request>
 *	<Response>
 *		<tag name="TAG_X509_RESULT" required="required">[RTS_UI16]: Result code of online operation. 
 *              ERR_OK: Operation finished. 
 *              ERR_INVALID_HANDLE: TAG_X509_STATUS_HANDLE was invalid. 
 *              ERR_PENDING: Operations still running. 
 *              ERR_FAILED: Operations failed.
 *      </tag>
 *	</Response>
 * </service>
 */
 
/**
 * <category>Static defines</category>
 * <description>Predefined objects in the runtime</description>
 */
#define USERDB_OBJECT_X509				        "Device.X509"
#define USERDB_OBJECT_X509_IMPORT_CERT          "Device.X509.ImportCertificate"
#define USERDB_OBJECT_X509_EXPORT_CERT          "Device.X509.ExportCertificate"
#define USERDB_OBJECT_X509_DELETE_CERT          "Device.X509.DeleteCertificate"
#define USERDB_OBJECT_X509_MOVE_CERT            "Device.X509.MoveCertificate"
#define USERDB_OBJECT_X509_LIST_CERTS           "Device.X509.ListCertificates"
#define USERDB_OBJECT_X509_LIST_USE_CASES       "Device.X509.ListUseCases"
#define USERDB_OBJECT_X509_CREATE_SELFSIGNED    "Device.X509.CreateSelfSignedCertificate"
#define USERDB_OBJECT_X509_CREATE_CSR           "Device.X509.CreateCertificateSigningRequest"
#define USERDB_OBJECT_X509_GET_STAUTS           "Device.X509.GetStatus"
#define USERDB_OBJECT_X509_UNKNOWN              "Device.X509.Unknown"







/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Enum: RtsCertEncoding</description>
 */
#define RTSCERTENCODING_ASN1    RTS_IEC_INT_C(0x0)	
#define RTSCERTENCODING_BASE64    RTS_IEC_INT_C(0x1)	
/* Typed enum definition */
#define RTSCERTENCODING    RTS_IEC_INT

/**
 * The truste level has two main meanings:
 * 1. In opend certificates it indicates if the certificate is trusted or not
 * 2. If adding or searching certificates in the store it indicates where to add / search the certificate.
 */
#define RTSCERTTRUSTLEVEL_UNTRUSTED    RTS_IEC_INT_C(0x1)	
#define RTSCERTTRUSTLEVEL_TRUSTED    RTS_IEC_INT_C(0x2)	
#define RTSCERTTRUSTLEVEL_OWN    RTS_IEC_INT_C(0x3)	
#define RTSCERTTRUSTLEVEL_QUARANTINE    RTS_IEC_INT_C(0x4)	
#define RTSCERTTRUSTLEVEL_UNKNOWN    RTS_IEC_INT_C(0x5)	
/* Typed enum definition */
#define RTSCERTTRUSTLEVEL    RTS_IEC_INT

/**
 * <description>Enum: RtsX509AltNameType</description>
 */
#define RTSX509ALTNAMETYPE_OTHERNAME    RTS_IEC_INT_C(0x0)	/* Not supported */
#define RTSX509ALTNAMETYPE_EMAIL    RTS_IEC_INT_C(0x1)	/* Supported */
#define RTSX509ALTNAMETYPE_DNSNAME    RTS_IEC_INT_C(0x2)	/* Supported */
#define RTSX509ALTNAMETYPE_X400ADDRESS    RTS_IEC_INT_C(0x3)	/* Not supported */
#define RTSX509ALTNAMETYPE_DIRECTORYNAME    RTS_IEC_INT_C(0x4)	/* Not supported */
#define RTSX509ALTNAMETYPE_EDIPARTYNAME    RTS_IEC_INT_C(0x5)	/* Not supported */
#define RTSX509ALTNAMETYPE_URI    RTS_IEC_INT_C(0x6)	/* Supported */
#define RTSX509ALTNAMETYPE_IPADDRESS    RTS_IEC_INT_C(0x7)	/* Supported */
#define RTSX509ALTNAMETYPE_REGISTERDID    RTS_IEC_INT_C(0x8)	/* Supported */
/* Typed enum definition */
#define RTSX509ALTNAMETYPE    RTS_IEC_INT

/**
 * <description>Enum: RtsX509CertFilterType</description>
 */
#define RTSX509CERTFILTERTYPE_TRUST_LEVEL    RTS_IEC_INT_C(0x0)	
#define RTSX509CERTFILTERTYPE_SUBJECT    RTS_IEC_INT_C(0x1)	
#define RTSX509CERTFILTERTYPE_SUBJECT_MATCHALL    RTS_IEC_INT_C(0x2)	
#define RTSX509CERTFILTERTYPE_CERT_DATEVALID    RTS_IEC_INT_C(0x3)	
#define RTSX509CERTFILTERTYPE_CERT_KEYUSAGE    RTS_IEC_INT_C(0x4)	
#define RTSX509CERTFILTERTYPE_CERT_EXKEYUSAGE    RTS_IEC_INT_C(0x5)	
#define RTSX509CERTFILTERTYPE_CERT_THUMBPRINT    RTS_IEC_INT_C(0x6)	
/* Typed enum definition */
#define RTSX509CERTFILTERTYPE    RTS_IEC_INT

/**
 * <description>RtsOIDStore</description>
 */
typedef union
{
	RTS_IEC_BYTE aOID[16];		
	RTS_IEC_BYTE *pOID;		
} RtsOIDStore;

/**
 * <description>RtsOID</description>
 */
typedef struct tagRtsOID
{
	RTS_IEC_UDINT length;		/* Lenght of the OID in memory */
	RtsOIDStore data;		/* Storage of the OID */
	RTS_IEC_UDINT flags;		/* Internal */
} RtsOID;

/**
 * <description>RtsX509AltNameStore</description>
 */
typedef union
{
	RTS_IEC_STRING *pEmail;		
	RTS_IEC_STRING *pDnsName;		
	RTS_IEC_STRING *pUri;		
	INADDR ipAddress;		
	RtsOID registerdID;		
} RtsX509AltNameStore;

/**
 * Represents an alternative name of a certificate. The alternative 
 * name can be selected out of different meanings. E.g. E-Mail 
 * address, IP-Address, URI or a DNS name. Use the corresponding
 * fiels of the union to geht the values. The meaning is declared
 * in the valueType field.
 */
typedef struct tagRtsX509AltName
{
	RTS_IEC_INT valueType;		/* Type of the alternative name */
	RtsX509AltNameStore value;		
} RtsX509AltName;

/**
 * Represents an entry of the X509 Names used in the
 * subject or issuer fields in the certificate.
 * This sturucter contains the OID to identify the field
 * and the corresponding string as value. Use the 
 * RtsOIDGetName function to get the string
 * representations of the field.
 */
typedef struct tagRtsX509NameEntry
{
	RtsOID id;		/* ID of the entry */
	RTS_IEC_STRING *value;		/* String value of the entry */
} RtsX509NameEntry;

/**
 * <description>RtsX509CertName</description>
 */
typedef struct tagRtsX509CertName
{
	RTS_IEC_UDINT numOfEntries;		/* Number of entries in the X509 Name */
	RtsX509NameEntry *entries;		/* Pointer to a list of name entries. */
} RtsX509CertName;

/**
 * <description>RtsX509ExKeyUsage</description>
 */
typedef struct tagRtsX509ExKeyUsage
{
	RTS_IEC_UDINT numOfExKeyUsages;		/* Number of extended key usages */
	RtsOID *pExKeyUsages;		/* Pointer to a list of extended key usages. */
} RtsX509ExKeyUsage;

/**
 * <description>RtsX509CertFilterContent</description>
 */
typedef union
{
	RTS_IEC_INT trustLevel;		
	RtsX509CertName *subject;		
	RTS_IEC_UDINT keyUsage;		
	RtsX509ExKeyUsage *exKeyUsage;		
	RtsByteString *thumpbrint;		
} RtsX509CertFilterContent;

/**
 * <description>RtsX509CertFilter</description>
 */
typedef struct tagRtsX509CertFilter
{
	RTS_IEC_INT filterType;		
	RtsX509CertFilterContent filterContent;		
} RtsX509CertFilter;

/**
 * This structuer contains common fiels of an certificate. Most values are stored in
 * textual form if available. The alternative key usages and the signature algorithm
 * are stored as RtsOID. Use the RtsOIDGetName function to get human readable names
 * of this fields. The notBefor and notAfter fiels are in UTC format and high resolution
 * timestam
 */
typedef struct tagRtsX509CertInfo
{
	RTS_IEC_UDINT ui32StructSize;		/* Structure size. Used if the structuer is extended in the futuer. */
	RtsX509CertName issuer;		/* Common fields of the issuer */
	RtsX509CertName subject;		/* Common fields of the subject */
	RTS_IEC_UDINT numOfAltNames;		/* Number of stored alternative names */
	RtsX509AltName *pAltNames;		/* Pointer to a list of alternative names. */
	RTS_IEC_ULINT notBefore;		/* High resolution UTC timestamp */
	RTS_IEC_ULINT notAfter;		/* High resolution UTC timestamp */
	RTS_IEC_UDINT keyUsage;		/* Key usage field. Use the constans of X509CertKeyUsage to determin the key usages. Every value is represented by a specific bit of the value. */
	RTS_IEC_UDINT numOfExKeyUsages;		/* Number of extended key usages */
	RtsOID *pExKeyUsages;		/* Pointer to a list of extended key usages. */
	RtsOID signatureAlgorithm;		/* OID of the signature algorithm. Use the RtsOIDGetName to get a human readable string. */
} RtsX509CertInfo;

/**
 * Clear an OID
 * :return: Result of the operation
 */
typedef struct tagrtsoidclear_struct
{
	RtsOID *oid;						/* VAR_INPUT */	
	RTS_IEC_RESULT RtsOIDClear;			/* VAR_OUTPUT */	
} rtsoidclear_struct;

void CDECL CDECL_EXT rtsoidclear(rtsoidclear_struct *p);
typedef void (CDECL CDECL_EXT* PFRTSOIDCLEAR_IEC) (rtsoidclear_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(RTSOIDCLEAR_NOTIMPLEMENTED)
	#define USE_rtsoidclear
	#define EXT_rtsoidclear
	#define GET_rtsoidclear(fl)  ERR_NOTIMPLEMENTED
	#define CAL_rtsoidclear(p0) 
	#define CHK_rtsoidclear  FALSE
	#define EXP_rtsoidclear  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_rtsoidclear
	#define EXT_rtsoidclear
	#define GET_rtsoidclear(fl)  CAL_CMGETAPI( "rtsoidclear" ) 
	#define CAL_rtsoidclear  rtsoidclear
	#define CHK_rtsoidclear  TRUE
	#define EXP_rtsoidclear  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidclear", (RTS_UINTPTR)rtsoidclear, 1, 0xB53A1CAF, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_rtsoidclear
	#define EXT_rtsoidclear
	#define GET_rtsoidclear(fl)  CAL_CMGETAPI( "rtsoidclear" ) 
	#define CAL_rtsoidclear  rtsoidclear
	#define CHK_rtsoidclear  TRUE
	#define EXP_rtsoidclear  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidclear", (RTS_UINTPTR)rtsoidclear, 1, 0xB53A1CAF, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certrtsoidclear
	#define EXT_CmpX509Certrtsoidclear
	#define GET_CmpX509Certrtsoidclear  ERR_OK
	#define CAL_CmpX509Certrtsoidclear  rtsoidclear
	#define CHK_CmpX509Certrtsoidclear  TRUE
	#define EXP_CmpX509Certrtsoidclear  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidclear", (RTS_UINTPTR)rtsoidclear, 1, 0xB53A1CAF, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_rtsoidclear
	#define EXT_rtsoidclear
	#define GET_rtsoidclear(fl)  CAL_CMGETAPI( "rtsoidclear" ) 
	#define CAL_rtsoidclear  rtsoidclear
	#define CHK_rtsoidclear  TRUE
	#define EXP_rtsoidclear  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidclear", (RTS_UINTPTR)rtsoidclear, 1, 0xB53A1CAF, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_rtsoidclear  PFRTSOIDCLEAR_IEC pfrtsoidclear;
	#define EXT_rtsoidclear  extern PFRTSOIDCLEAR_IEC pfrtsoidclear;
	#define GET_rtsoidclear(fl)  s_pfCMGetAPI2( "rtsoidclear", (RTS_VOID_FCTPTR *)&pfrtsoidclear, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xB53A1CAF, 0x03050B00)
	#define CAL_rtsoidclear  pfrtsoidclear
	#define CHK_rtsoidclear  (pfrtsoidclear != NULL)
	#define EXP_rtsoidclear   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidclear", (RTS_UINTPTR)rtsoidclear, 1, 0xB53A1CAF, 0x03050B00) 
#endif


/**
 * Create an RtsOID based on a given ID.
 * :return: Result of the operation.
 */
typedef struct tagrtsoidcreate_struct
{
	RTS_IEC_STRING *id;					/* VAR_INPUT */	/* OID as string in dottet format. E.g. (1.2.3.4.5.6.7.8.9.0) */
	RtsOID *oid;						/* VAR_INPUT */	/* Pointer to an OID to fill up. */
	RTS_IEC_RESULT RtsOIDCreate;		/* VAR_OUTPUT */	
} rtsoidcreate_struct;

void CDECL CDECL_EXT rtsoidcreate(rtsoidcreate_struct *p);
typedef void (CDECL CDECL_EXT* PFRTSOIDCREATE_IEC) (rtsoidcreate_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(RTSOIDCREATE_NOTIMPLEMENTED)
	#define USE_rtsoidcreate
	#define EXT_rtsoidcreate
	#define GET_rtsoidcreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_rtsoidcreate(p0) 
	#define CHK_rtsoidcreate  FALSE
	#define EXP_rtsoidcreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_rtsoidcreate
	#define EXT_rtsoidcreate
	#define GET_rtsoidcreate(fl)  CAL_CMGETAPI( "rtsoidcreate" ) 
	#define CAL_rtsoidcreate  rtsoidcreate
	#define CHK_rtsoidcreate  TRUE
	#define EXP_rtsoidcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidcreate", (RTS_UINTPTR)rtsoidcreate, 1, 0xE11A70BD, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_rtsoidcreate
	#define EXT_rtsoidcreate
	#define GET_rtsoidcreate(fl)  CAL_CMGETAPI( "rtsoidcreate" ) 
	#define CAL_rtsoidcreate  rtsoidcreate
	#define CHK_rtsoidcreate  TRUE
	#define EXP_rtsoidcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidcreate", (RTS_UINTPTR)rtsoidcreate, 1, 0xE11A70BD, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certrtsoidcreate
	#define EXT_CmpX509Certrtsoidcreate
	#define GET_CmpX509Certrtsoidcreate  ERR_OK
	#define CAL_CmpX509Certrtsoidcreate  rtsoidcreate
	#define CHK_CmpX509Certrtsoidcreate  TRUE
	#define EXP_CmpX509Certrtsoidcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidcreate", (RTS_UINTPTR)rtsoidcreate, 1, 0xE11A70BD, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_rtsoidcreate
	#define EXT_rtsoidcreate
	#define GET_rtsoidcreate(fl)  CAL_CMGETAPI( "rtsoidcreate" ) 
	#define CAL_rtsoidcreate  rtsoidcreate
	#define CHK_rtsoidcreate  TRUE
	#define EXP_rtsoidcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidcreate", (RTS_UINTPTR)rtsoidcreate, 1, 0xE11A70BD, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_rtsoidcreate  PFRTSOIDCREATE_IEC pfrtsoidcreate;
	#define EXT_rtsoidcreate  extern PFRTSOIDCREATE_IEC pfrtsoidcreate;
	#define GET_rtsoidcreate(fl)  s_pfCMGetAPI2( "rtsoidcreate", (RTS_VOID_FCTPTR *)&pfrtsoidcreate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xE11A70BD, 0x03050B00)
	#define CAL_rtsoidcreate  pfrtsoidcreate
	#define CHK_rtsoidcreate  (pfrtsoidcreate != NULL)
	#define EXP_rtsoidcreate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidcreate", (RTS_UINTPTR)rtsoidcreate, 1, 0xE11A70BD, 0x03050B00) 
#endif


/**
 * Get the ID of an OID in dottet notation.
 * :return: Result of the operation
 */
typedef struct tagrtsoidgetid_struct
{
	RtsOID *oid;						/* VAR_INPUT */	/* OID to print. */
	RtsByteString *pBuffer;				/* VAR_INPUT */	/* Buffer where to store the ID. */
	RTS_IEC_RESULT RtsOIDGetID;			/* VAR_OUTPUT */	
} rtsoidgetid_struct;

void CDECL CDECL_EXT rtsoidgetid(rtsoidgetid_struct *p);
typedef void (CDECL CDECL_EXT* PFRTSOIDGETID_IEC) (rtsoidgetid_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(RTSOIDGETID_NOTIMPLEMENTED)
	#define USE_rtsoidgetid
	#define EXT_rtsoidgetid
	#define GET_rtsoidgetid(fl)  ERR_NOTIMPLEMENTED
	#define CAL_rtsoidgetid(p0) 
	#define CHK_rtsoidgetid  FALSE
	#define EXP_rtsoidgetid  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_rtsoidgetid
	#define EXT_rtsoidgetid
	#define GET_rtsoidgetid(fl)  CAL_CMGETAPI( "rtsoidgetid" ) 
	#define CAL_rtsoidgetid  rtsoidgetid
	#define CHK_rtsoidgetid  TRUE
	#define EXP_rtsoidgetid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetid", (RTS_UINTPTR)rtsoidgetid, 1, 0xB69D55FF, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_rtsoidgetid
	#define EXT_rtsoidgetid
	#define GET_rtsoidgetid(fl)  CAL_CMGETAPI( "rtsoidgetid" ) 
	#define CAL_rtsoidgetid  rtsoidgetid
	#define CHK_rtsoidgetid  TRUE
	#define EXP_rtsoidgetid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetid", (RTS_UINTPTR)rtsoidgetid, 1, 0xB69D55FF, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certrtsoidgetid
	#define EXT_CmpX509Certrtsoidgetid
	#define GET_CmpX509Certrtsoidgetid  ERR_OK
	#define CAL_CmpX509Certrtsoidgetid  rtsoidgetid
	#define CHK_CmpX509Certrtsoidgetid  TRUE
	#define EXP_CmpX509Certrtsoidgetid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetid", (RTS_UINTPTR)rtsoidgetid, 1, 0xB69D55FF, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_rtsoidgetid
	#define EXT_rtsoidgetid
	#define GET_rtsoidgetid(fl)  CAL_CMGETAPI( "rtsoidgetid" ) 
	#define CAL_rtsoidgetid  rtsoidgetid
	#define CHK_rtsoidgetid  TRUE
	#define EXP_rtsoidgetid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetid", (RTS_UINTPTR)rtsoidgetid, 1, 0xB69D55FF, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_rtsoidgetid  PFRTSOIDGETID_IEC pfrtsoidgetid;
	#define EXT_rtsoidgetid  extern PFRTSOIDGETID_IEC pfrtsoidgetid;
	#define GET_rtsoidgetid(fl)  s_pfCMGetAPI2( "rtsoidgetid", (RTS_VOID_FCTPTR *)&pfrtsoidgetid, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xB69D55FF, 0x03050B00)
	#define CAL_rtsoidgetid  pfrtsoidgetid
	#define CHK_rtsoidgetid  (pfrtsoidgetid != NULL)
	#define EXP_rtsoidgetid   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetid", (RTS_UINTPTR)rtsoidgetid, 1, 0xB69D55FF, 0x03050B00) 
#endif


/**
 * Get the given name of an OID. If the OID is unknown 
 * to the system the dottet notation will be returned.
 * :return: Result of the operation.
 */
typedef struct tagrtsoidgetname_struct
{
	RtsOID *oid;						/* VAR_INPUT */	
	RtsByteString *pBuffer;				/* VAR_INPUT */	
	RTS_IEC_RESULT RtsOIDGetName;		/* VAR_OUTPUT */	
} rtsoidgetname_struct;

void CDECL CDECL_EXT rtsoidgetname(rtsoidgetname_struct *p);
typedef void (CDECL CDECL_EXT* PFRTSOIDGETNAME_IEC) (rtsoidgetname_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(RTSOIDGETNAME_NOTIMPLEMENTED)
	#define USE_rtsoidgetname
	#define EXT_rtsoidgetname
	#define GET_rtsoidgetname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_rtsoidgetname(p0) 
	#define CHK_rtsoidgetname  FALSE
	#define EXP_rtsoidgetname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_rtsoidgetname
	#define EXT_rtsoidgetname
	#define GET_rtsoidgetname(fl)  CAL_CMGETAPI( "rtsoidgetname" ) 
	#define CAL_rtsoidgetname  rtsoidgetname
	#define CHK_rtsoidgetname  TRUE
	#define EXP_rtsoidgetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetname", (RTS_UINTPTR)rtsoidgetname, 1, 0xBEA6E204, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_rtsoidgetname
	#define EXT_rtsoidgetname
	#define GET_rtsoidgetname(fl)  CAL_CMGETAPI( "rtsoidgetname" ) 
	#define CAL_rtsoidgetname  rtsoidgetname
	#define CHK_rtsoidgetname  TRUE
	#define EXP_rtsoidgetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetname", (RTS_UINTPTR)rtsoidgetname, 1, 0xBEA6E204, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certrtsoidgetname
	#define EXT_CmpX509Certrtsoidgetname
	#define GET_CmpX509Certrtsoidgetname  ERR_OK
	#define CAL_CmpX509Certrtsoidgetname  rtsoidgetname
	#define CHK_CmpX509Certrtsoidgetname  TRUE
	#define EXP_CmpX509Certrtsoidgetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetname", (RTS_UINTPTR)rtsoidgetname, 1, 0xBEA6E204, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_rtsoidgetname
	#define EXT_rtsoidgetname
	#define GET_rtsoidgetname(fl)  CAL_CMGETAPI( "rtsoidgetname" ) 
	#define CAL_rtsoidgetname  rtsoidgetname
	#define CHK_rtsoidgetname  TRUE
	#define EXP_rtsoidgetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetname", (RTS_UINTPTR)rtsoidgetname, 1, 0xBEA6E204, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_rtsoidgetname  PFRTSOIDGETNAME_IEC pfrtsoidgetname;
	#define EXT_rtsoidgetname  extern PFRTSOIDGETNAME_IEC pfrtsoidgetname;
	#define GET_rtsoidgetname(fl)  s_pfCMGetAPI2( "rtsoidgetname", (RTS_VOID_FCTPTR *)&pfrtsoidgetname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xBEA6E204, 0x03050B00)
	#define CAL_rtsoidgetname  pfrtsoidgetname
	#define CHK_rtsoidgetname  (pfrtsoidgetname != NULL)
	#define EXP_rtsoidgetname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"rtsoidgetname", (RTS_UINTPTR)rtsoidgetname, 1, 0xBEA6E204, 0x03050B00) 
#endif


/**
 * Close the certificate.
 * :return: Result of the operation.
 */
typedef struct tagx509certclose_struct
{
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	/* Handle of to certificate. */
	RTS_IEC_RESULT X509CertClose;		/* VAR_OUTPUT */	
} x509certclose_struct;

void CDECL CDECL_EXT x509certclose(x509certclose_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTCLOSE_IEC) (x509certclose_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCLOSE_NOTIMPLEMENTED)
	#define USE_x509certclose
	#define EXT_x509certclose
	#define GET_x509certclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certclose(p0) 
	#define CHK_x509certclose  FALSE
	#define EXP_x509certclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certclose
	#define EXT_x509certclose
	#define GET_x509certclose(fl)  CAL_CMGETAPI( "x509certclose" ) 
	#define CAL_x509certclose  x509certclose
	#define CHK_x509certclose  TRUE
	#define EXP_x509certclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certclose", (RTS_UINTPTR)x509certclose, 1, 0x150A5E6B, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certclose
	#define EXT_x509certclose
	#define GET_x509certclose(fl)  CAL_CMGETAPI( "x509certclose" ) 
	#define CAL_x509certclose  x509certclose
	#define CHK_x509certclose  TRUE
	#define EXP_x509certclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certclose", (RTS_UINTPTR)x509certclose, 1, 0x150A5E6B, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certclose
	#define EXT_CmpX509Certx509certclose
	#define GET_CmpX509Certx509certclose  ERR_OK
	#define CAL_CmpX509Certx509certclose  x509certclose
	#define CHK_CmpX509Certx509certclose  TRUE
	#define EXP_CmpX509Certx509certclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certclose", (RTS_UINTPTR)x509certclose, 1, 0x150A5E6B, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certclose
	#define EXT_x509certclose
	#define GET_x509certclose(fl)  CAL_CMGETAPI( "x509certclose" ) 
	#define CAL_x509certclose  x509certclose
	#define CHK_x509certclose  TRUE
	#define EXP_x509certclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certclose", (RTS_UINTPTR)x509certclose, 1, 0x150A5E6B, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certclose  PFX509CERTCLOSE_IEC pfx509certclose;
	#define EXT_x509certclose  extern PFX509CERTCLOSE_IEC pfx509certclose;
	#define GET_x509certclose(fl)  s_pfCMGetAPI2( "x509certclose", (RTS_VOID_FCTPTR *)&pfx509certclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x150A5E6B, 0x03050B00)
	#define CAL_x509certclose  pfx509certclose
	#define CHK_x509certclose  (pfx509certclose != NULL)
	#define EXP_x509certclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certclose", (RTS_UINTPTR)x509certclose, 1, 0x150A5E6B, 0x03050B00) 
#endif


/**
 * <description>x509certcmsdecrypt</description>
 */
typedef struct tagx509certcmsdecrypt_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hRecipientCert;		/* VAR_INPUT */	
	RtsByteString *pCms;				/* VAR_INPUT */	
	RTS_IEC_INT encoding;				/* VAR_INPUT, Enum: RTSCERTENCODING */
	RtsByteString *pOutData;			/* VAR_INPUT */	
	RTS_IEC_RESULT X509CertCmsDecrypt;	/* VAR_OUTPUT */	
} x509certcmsdecrypt_struct;

void CDECL CDECL_EXT x509certcmsdecrypt(x509certcmsdecrypt_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTCMSDECRYPT_IEC) (x509certcmsdecrypt_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCMSDECRYPT_NOTIMPLEMENTED)
	#define USE_x509certcmsdecrypt
	#define EXT_x509certcmsdecrypt
	#define GET_x509certcmsdecrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certcmsdecrypt(p0) 
	#define CHK_x509certcmsdecrypt  FALSE
	#define EXP_x509certcmsdecrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certcmsdecrypt
	#define EXT_x509certcmsdecrypt
	#define GET_x509certcmsdecrypt(fl)  CAL_CMGETAPI( "x509certcmsdecrypt" ) 
	#define CAL_x509certcmsdecrypt  x509certcmsdecrypt
	#define CHK_x509certcmsdecrypt  TRUE
	#define EXP_x509certcmsdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsdecrypt", (RTS_UINTPTR)x509certcmsdecrypt, 1, 0xCCAD85D1, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certcmsdecrypt
	#define EXT_x509certcmsdecrypt
	#define GET_x509certcmsdecrypt(fl)  CAL_CMGETAPI( "x509certcmsdecrypt" ) 
	#define CAL_x509certcmsdecrypt  x509certcmsdecrypt
	#define CHK_x509certcmsdecrypt  TRUE
	#define EXP_x509certcmsdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsdecrypt", (RTS_UINTPTR)x509certcmsdecrypt, 1, 0xCCAD85D1, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certcmsdecrypt
	#define EXT_CmpX509Certx509certcmsdecrypt
	#define GET_CmpX509Certx509certcmsdecrypt  ERR_OK
	#define CAL_CmpX509Certx509certcmsdecrypt  x509certcmsdecrypt
	#define CHK_CmpX509Certx509certcmsdecrypt  TRUE
	#define EXP_CmpX509Certx509certcmsdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsdecrypt", (RTS_UINTPTR)x509certcmsdecrypt, 1, 0xCCAD85D1, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certcmsdecrypt
	#define EXT_x509certcmsdecrypt
	#define GET_x509certcmsdecrypt(fl)  CAL_CMGETAPI( "x509certcmsdecrypt" ) 
	#define CAL_x509certcmsdecrypt  x509certcmsdecrypt
	#define CHK_x509certcmsdecrypt  TRUE
	#define EXP_x509certcmsdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsdecrypt", (RTS_UINTPTR)x509certcmsdecrypt, 1, 0xCCAD85D1, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certcmsdecrypt  PFX509CERTCMSDECRYPT_IEC pfx509certcmsdecrypt;
	#define EXT_x509certcmsdecrypt  extern PFX509CERTCMSDECRYPT_IEC pfx509certcmsdecrypt;
	#define GET_x509certcmsdecrypt(fl)  s_pfCMGetAPI2( "x509certcmsdecrypt", (RTS_VOID_FCTPTR *)&pfx509certcmsdecrypt, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xCCAD85D1, 0x03050B00)
	#define CAL_x509certcmsdecrypt  pfx509certcmsdecrypt
	#define CHK_x509certcmsdecrypt  (pfx509certcmsdecrypt != NULL)
	#define EXP_x509certcmsdecrypt   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsdecrypt", (RTS_UINTPTR)x509certcmsdecrypt, 1, 0xCCAD85D1, 0x03050B00) 
#endif


/**
 * <description>x509certcmsverify</description>
 */
typedef struct tagx509certcmsverify_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RtsByteString *pCms;				/* VAR_INPUT */	
	RTS_IEC_INT encoding;				/* VAR_INPUT, Enum: RTSCERTENCODING */
	RtsByteString *pInData;				/* VAR_INPUT */	
	RtsByteString *pOutData;			/* VAR_INPUT */	
	RTS_IEC_RESULT X509CertCmsVerify;	/* VAR_OUTPUT */	
} x509certcmsverify_struct;

void CDECL CDECL_EXT x509certcmsverify(x509certcmsverify_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTCMSVERIFY_IEC) (x509certcmsverify_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCMSVERIFY_NOTIMPLEMENTED)
	#define USE_x509certcmsverify
	#define EXT_x509certcmsverify
	#define GET_x509certcmsverify(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certcmsverify(p0) 
	#define CHK_x509certcmsverify  FALSE
	#define EXP_x509certcmsverify  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certcmsverify
	#define EXT_x509certcmsverify
	#define GET_x509certcmsverify(fl)  CAL_CMGETAPI( "x509certcmsverify" ) 
	#define CAL_x509certcmsverify  x509certcmsverify
	#define CHK_x509certcmsverify  TRUE
	#define EXP_x509certcmsverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsverify", (RTS_UINTPTR)x509certcmsverify, 1, 0x73653424, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certcmsverify
	#define EXT_x509certcmsverify
	#define GET_x509certcmsverify(fl)  CAL_CMGETAPI( "x509certcmsverify" ) 
	#define CAL_x509certcmsverify  x509certcmsverify
	#define CHK_x509certcmsverify  TRUE
	#define EXP_x509certcmsverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsverify", (RTS_UINTPTR)x509certcmsverify, 1, 0x73653424, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certcmsverify
	#define EXT_CmpX509Certx509certcmsverify
	#define GET_CmpX509Certx509certcmsverify  ERR_OK
	#define CAL_CmpX509Certx509certcmsverify  x509certcmsverify
	#define CHK_CmpX509Certx509certcmsverify  TRUE
	#define EXP_CmpX509Certx509certcmsverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsverify", (RTS_UINTPTR)x509certcmsverify, 1, 0x73653424, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certcmsverify
	#define EXT_x509certcmsverify
	#define GET_x509certcmsverify(fl)  CAL_CMGETAPI( "x509certcmsverify" ) 
	#define CAL_x509certcmsverify  x509certcmsverify
	#define CHK_x509certcmsverify  TRUE
	#define EXP_x509certcmsverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsverify", (RTS_UINTPTR)x509certcmsverify, 1, 0x73653424, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certcmsverify  PFX509CERTCMSVERIFY_IEC pfx509certcmsverify;
	#define EXT_x509certcmsverify  extern PFX509CERTCMSVERIFY_IEC pfx509certcmsverify;
	#define GET_x509certcmsverify(fl)  s_pfCMGetAPI2( "x509certcmsverify", (RTS_VOID_FCTPTR *)&pfx509certcmsverify, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x73653424, 0x03050B00)
	#define CAL_x509certcmsverify  pfx509certcmsverify
	#define CHK_x509certcmsverify  (pfx509certcmsverify != NULL)
	#define EXP_x509certcmsverify   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcmsverify", (RTS_UINTPTR)x509certcmsverify, 1, 0x73653424, 0x03050B00) 
#endif


/**
 * <description>x509certcreatecsr</description>
 */
typedef struct tagx509certcreatecsr_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hBackend;			/* VAR_INPUT */	
	RtsX509CertInfo *pCertInfo;			/* VAR_INPUT */	
	RTS_IEC_UDINT ui32KeySize;			/* VAR_INPUT */	
	RTS_IEC_INT encoding;				/* VAR_INPUT, Enum: RTSCERTENCODING */
	RtsByteString *pCSR;				/* VAR_INPUT */	
	RTS_IEC_RESULT X509CertCreateCSR;	/* VAR_OUTPUT */	
} x509certcreatecsr_struct;

void CDECL CDECL_EXT x509certcreatecsr(x509certcreatecsr_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTCREATECSR_IEC) (x509certcreatecsr_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCREATECSR_NOTIMPLEMENTED)
	#define USE_x509certcreatecsr
	#define EXT_x509certcreatecsr
	#define GET_x509certcreatecsr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certcreatecsr(p0) 
	#define CHK_x509certcreatecsr  FALSE
	#define EXP_x509certcreatecsr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certcreatecsr
	#define EXT_x509certcreatecsr
	#define GET_x509certcreatecsr(fl)  CAL_CMGETAPI( "x509certcreatecsr" ) 
	#define CAL_x509certcreatecsr  x509certcreatecsr
	#define CHK_x509certcreatecsr  TRUE
	#define EXP_x509certcreatecsr  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreatecsr", (RTS_UINTPTR)x509certcreatecsr, 1, 0xF64C1D04, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certcreatecsr
	#define EXT_x509certcreatecsr
	#define GET_x509certcreatecsr(fl)  CAL_CMGETAPI( "x509certcreatecsr" ) 
	#define CAL_x509certcreatecsr  x509certcreatecsr
	#define CHK_x509certcreatecsr  TRUE
	#define EXP_x509certcreatecsr  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreatecsr", (RTS_UINTPTR)x509certcreatecsr, 1, 0xF64C1D04, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certcreatecsr
	#define EXT_CmpX509Certx509certcreatecsr
	#define GET_CmpX509Certx509certcreatecsr  ERR_OK
	#define CAL_CmpX509Certx509certcreatecsr  x509certcreatecsr
	#define CHK_CmpX509Certx509certcreatecsr  TRUE
	#define EXP_CmpX509Certx509certcreatecsr  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreatecsr", (RTS_UINTPTR)x509certcreatecsr, 1, 0xF64C1D04, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certcreatecsr
	#define EXT_x509certcreatecsr
	#define GET_x509certcreatecsr(fl)  CAL_CMGETAPI( "x509certcreatecsr" ) 
	#define CAL_x509certcreatecsr  x509certcreatecsr
	#define CHK_x509certcreatecsr  TRUE
	#define EXP_x509certcreatecsr  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreatecsr", (RTS_UINTPTR)x509certcreatecsr, 1, 0xF64C1D04, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certcreatecsr  PFX509CERTCREATECSR_IEC pfx509certcreatecsr;
	#define EXT_x509certcreatecsr  extern PFX509CERTCREATECSR_IEC pfx509certcreatecsr;
	#define GET_x509certcreatecsr(fl)  s_pfCMGetAPI2( "x509certcreatecsr", (RTS_VOID_FCTPTR *)&pfx509certcreatecsr, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xF64C1D04, 0x03050B00)
	#define CAL_x509certcreatecsr  pfx509certcreatecsr
	#define CHK_x509certcreatecsr  (pfx509certcreatecsr != NULL)
	#define EXP_x509certcreatecsr   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreatecsr", (RTS_UINTPTR)x509certcreatecsr, 1, 0xF64C1D04, 0x03050B00) 
#endif


/**
 * <description>x509certcreateselfsigned</description>
 */
typedef struct tagx509certcreateselfsigned_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hBackend;			/* VAR_INPUT */	
	RtsX509CertInfo *pCertInfo;			/* VAR_INPUT */	
	RTS_IEC_UDINT ui32KeySize;			/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE X509CertCreateSelfSigned;	/* VAR_OUTPUT */	
} x509certcreateselfsigned_struct;

void CDECL CDECL_EXT x509certcreateselfsigned(x509certcreateselfsigned_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTCREATESELFSIGNED_IEC) (x509certcreateselfsigned_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCREATESELFSIGNED_NOTIMPLEMENTED)
	#define USE_x509certcreateselfsigned
	#define EXT_x509certcreateselfsigned
	#define GET_x509certcreateselfsigned(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certcreateselfsigned(p0) 
	#define CHK_x509certcreateselfsigned  FALSE
	#define EXP_x509certcreateselfsigned  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certcreateselfsigned
	#define EXT_x509certcreateselfsigned
	#define GET_x509certcreateselfsigned(fl)  CAL_CMGETAPI( "x509certcreateselfsigned" ) 
	#define CAL_x509certcreateselfsigned  x509certcreateselfsigned
	#define CHK_x509certcreateselfsigned  TRUE
	#define EXP_x509certcreateselfsigned  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreateselfsigned", (RTS_UINTPTR)x509certcreateselfsigned, 1, 0x6765E8BD, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certcreateselfsigned
	#define EXT_x509certcreateselfsigned
	#define GET_x509certcreateselfsigned(fl)  CAL_CMGETAPI( "x509certcreateselfsigned" ) 
	#define CAL_x509certcreateselfsigned  x509certcreateselfsigned
	#define CHK_x509certcreateselfsigned  TRUE
	#define EXP_x509certcreateselfsigned  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreateselfsigned", (RTS_UINTPTR)x509certcreateselfsigned, 1, 0x6765E8BD, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certcreateselfsigned
	#define EXT_CmpX509Certx509certcreateselfsigned
	#define GET_CmpX509Certx509certcreateselfsigned  ERR_OK
	#define CAL_CmpX509Certx509certcreateselfsigned  x509certcreateselfsigned
	#define CHK_CmpX509Certx509certcreateselfsigned  TRUE
	#define EXP_CmpX509Certx509certcreateselfsigned  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreateselfsigned", (RTS_UINTPTR)x509certcreateselfsigned, 1, 0x6765E8BD, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certcreateselfsigned
	#define EXT_x509certcreateselfsigned
	#define GET_x509certcreateselfsigned(fl)  CAL_CMGETAPI( "x509certcreateselfsigned" ) 
	#define CAL_x509certcreateselfsigned  x509certcreateselfsigned
	#define CHK_x509certcreateselfsigned  TRUE
	#define EXP_x509certcreateselfsigned  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreateselfsigned", (RTS_UINTPTR)x509certcreateselfsigned, 1, 0x6765E8BD, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certcreateselfsigned  PFX509CERTCREATESELFSIGNED_IEC pfx509certcreateselfsigned;
	#define EXT_x509certcreateselfsigned  extern PFX509CERTCREATESELFSIGNED_IEC pfx509certcreateselfsigned;
	#define GET_x509certcreateselfsigned(fl)  s_pfCMGetAPI2( "x509certcreateselfsigned", (RTS_VOID_FCTPTR *)&pfx509certcreateselfsigned, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x6765E8BD, 0x03050B00)
	#define CAL_x509certcreateselfsigned  pfx509certcreateselfsigned
	#define CHK_x509certcreateselfsigned  (pfx509certcreateselfsigned != NULL)
	#define EXP_x509certcreateselfsigned   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certcreateselfsigned", (RTS_UINTPTR)x509certcreateselfsigned, 1, 0x6765E8BD, 0x03050B00) 
#endif


/**
 * Get the certificate in a binary encoded format. Use the encoding parameter
 * to set the needed format.
 * :return: Result of the operation.
 */
typedef struct tagx509certgetbinary_struct
{
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	/* Handle of the certificate of intrest. */
	RTS_IEC_INT encoding;				/* VAR_INPUT, Enum: RTSCERTENCODING */
	RtsByteString *pCert;				/* VAR_INPUT */	/* Buffer where to store the certificate. If the bytestring contains a NULL memory pointer the function will write the needed bufferlength to the pCert->ui32Length member. */
	RTS_IEC_RESULT X509CertGetBinary;	/* VAR_OUTPUT */	
} x509certgetbinary_struct;

void CDECL CDECL_EXT x509certgetbinary(x509certgetbinary_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTGETBINARY_IEC) (x509certgetbinary_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETBINARY_NOTIMPLEMENTED)
	#define USE_x509certgetbinary
	#define EXT_x509certgetbinary
	#define GET_x509certgetbinary(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certgetbinary(p0) 
	#define CHK_x509certgetbinary  FALSE
	#define EXP_x509certgetbinary  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certgetbinary
	#define EXT_x509certgetbinary
	#define GET_x509certgetbinary(fl)  CAL_CMGETAPI( "x509certgetbinary" ) 
	#define CAL_x509certgetbinary  x509certgetbinary
	#define CHK_x509certgetbinary  TRUE
	#define EXP_x509certgetbinary  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetbinary", (RTS_UINTPTR)x509certgetbinary, 1, 0x6F7D6C42, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certgetbinary
	#define EXT_x509certgetbinary
	#define GET_x509certgetbinary(fl)  CAL_CMGETAPI( "x509certgetbinary" ) 
	#define CAL_x509certgetbinary  x509certgetbinary
	#define CHK_x509certgetbinary  TRUE
	#define EXP_x509certgetbinary  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetbinary", (RTS_UINTPTR)x509certgetbinary, 1, 0x6F7D6C42, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certgetbinary
	#define EXT_CmpX509Certx509certgetbinary
	#define GET_CmpX509Certx509certgetbinary  ERR_OK
	#define CAL_CmpX509Certx509certgetbinary  x509certgetbinary
	#define CHK_CmpX509Certx509certgetbinary  TRUE
	#define EXP_CmpX509Certx509certgetbinary  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetbinary", (RTS_UINTPTR)x509certgetbinary, 1, 0x6F7D6C42, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certgetbinary
	#define EXT_x509certgetbinary
	#define GET_x509certgetbinary(fl)  CAL_CMGETAPI( "x509certgetbinary" ) 
	#define CAL_x509certgetbinary  x509certgetbinary
	#define CHK_x509certgetbinary  TRUE
	#define EXP_x509certgetbinary  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetbinary", (RTS_UINTPTR)x509certgetbinary, 1, 0x6F7D6C42, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certgetbinary  PFX509CERTGETBINARY_IEC pfx509certgetbinary;
	#define EXT_x509certgetbinary  extern PFX509CERTGETBINARY_IEC pfx509certgetbinary;
	#define GET_x509certgetbinary(fl)  s_pfCMGetAPI2( "x509certgetbinary", (RTS_VOID_FCTPTR *)&pfx509certgetbinary, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x6F7D6C42, 0x03050B00)
	#define CAL_x509certgetbinary  pfx509certgetbinary
	#define CHK_x509certgetbinary  (pfx509certgetbinary != NULL)
	#define EXP_x509certgetbinary   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetbinary", (RTS_UINTPTR)x509certgetbinary, 1, 0x6F7D6C42, 0x03050B00) 
#endif


/**
 * Get some common content from the certificate.
 * :return: Result of the operation.
 */
typedef struct tagx509certgetcontent_struct
{
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	/* Handle of the certificate of intrest. */
	RtsX509CertInfo *pCertContent;		/* VAR_INPUT */	/* Pointer where to store the information. */
	RTS_IEC_RESULT X509CertGetContent;	/* VAR_OUTPUT */	
} x509certgetcontent_struct;

void CDECL CDECL_EXT x509certgetcontent(x509certgetcontent_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTGETCONTENT_IEC) (x509certgetcontent_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETCONTENT_NOTIMPLEMENTED)
	#define USE_x509certgetcontent
	#define EXT_x509certgetcontent
	#define GET_x509certgetcontent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certgetcontent(p0) 
	#define CHK_x509certgetcontent  FALSE
	#define EXP_x509certgetcontent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certgetcontent
	#define EXT_x509certgetcontent
	#define GET_x509certgetcontent(fl)  CAL_CMGETAPI( "x509certgetcontent" ) 
	#define CAL_x509certgetcontent  x509certgetcontent
	#define CHK_x509certgetcontent  TRUE
	#define EXP_x509certgetcontent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetcontent", (RTS_UINTPTR)x509certgetcontent, 1, 0xDA69570E, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certgetcontent
	#define EXT_x509certgetcontent
	#define GET_x509certgetcontent(fl)  CAL_CMGETAPI( "x509certgetcontent" ) 
	#define CAL_x509certgetcontent  x509certgetcontent
	#define CHK_x509certgetcontent  TRUE
	#define EXP_x509certgetcontent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetcontent", (RTS_UINTPTR)x509certgetcontent, 1, 0xDA69570E, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certgetcontent
	#define EXT_CmpX509Certx509certgetcontent
	#define GET_CmpX509Certx509certgetcontent  ERR_OK
	#define CAL_CmpX509Certx509certgetcontent  x509certgetcontent
	#define CHK_CmpX509Certx509certgetcontent  TRUE
	#define EXP_CmpX509Certx509certgetcontent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetcontent", (RTS_UINTPTR)x509certgetcontent, 1, 0xDA69570E, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certgetcontent
	#define EXT_x509certgetcontent
	#define GET_x509certgetcontent(fl)  CAL_CMGETAPI( "x509certgetcontent" ) 
	#define CAL_x509certgetcontent  x509certgetcontent
	#define CHK_x509certgetcontent  TRUE
	#define EXP_x509certgetcontent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetcontent", (RTS_UINTPTR)x509certgetcontent, 1, 0xDA69570E, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certgetcontent  PFX509CERTGETCONTENT_IEC pfx509certgetcontent;
	#define EXT_x509certgetcontent  extern PFX509CERTGETCONTENT_IEC pfx509certgetcontent;
	#define GET_x509certgetcontent(fl)  s_pfCMGetAPI2( "x509certgetcontent", (RTS_VOID_FCTPTR *)&pfx509certgetcontent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xDA69570E, 0x03050B00)
	#define CAL_x509certgetcontent  pfx509certgetcontent
	#define CHK_x509certgetcontent  (pfx509certgetcontent != NULL)
	#define EXP_x509certgetcontent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetcontent", (RTS_UINTPTR)x509certgetcontent, 1, 0xDA69570E, 0x03050B00) 
#endif


/**
 * <description>x509certgetprivatekey</description>
 */
typedef struct tagx509certgetprivatekey_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	
	RtsCryptoKey *pPrivateKey;			/* VAR_INPUT */	
	RTS_IEC_RESULT X509CertGetPrivateKey;	/* VAR_OUTPUT */	
} x509certgetprivatekey_struct;

void CDECL CDECL_EXT x509certgetprivatekey(x509certgetprivatekey_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTGETPRIVATEKEY_IEC) (x509certgetprivatekey_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETPRIVATEKEY_NOTIMPLEMENTED)
	#define USE_x509certgetprivatekey
	#define EXT_x509certgetprivatekey
	#define GET_x509certgetprivatekey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certgetprivatekey(p0) 
	#define CHK_x509certgetprivatekey  FALSE
	#define EXP_x509certgetprivatekey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certgetprivatekey
	#define EXT_x509certgetprivatekey
	#define GET_x509certgetprivatekey(fl)  CAL_CMGETAPI( "x509certgetprivatekey" ) 
	#define CAL_x509certgetprivatekey  x509certgetprivatekey
	#define CHK_x509certgetprivatekey  TRUE
	#define EXP_x509certgetprivatekey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetprivatekey", (RTS_UINTPTR)x509certgetprivatekey, 1, 0x279C9A63, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certgetprivatekey
	#define EXT_x509certgetprivatekey
	#define GET_x509certgetprivatekey(fl)  CAL_CMGETAPI( "x509certgetprivatekey" ) 
	#define CAL_x509certgetprivatekey  x509certgetprivatekey
	#define CHK_x509certgetprivatekey  TRUE
	#define EXP_x509certgetprivatekey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetprivatekey", (RTS_UINTPTR)x509certgetprivatekey, 1, 0x279C9A63, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certgetprivatekey
	#define EXT_CmpX509Certx509certgetprivatekey
	#define GET_CmpX509Certx509certgetprivatekey  ERR_OK
	#define CAL_CmpX509Certx509certgetprivatekey  x509certgetprivatekey
	#define CHK_CmpX509Certx509certgetprivatekey  TRUE
	#define EXP_CmpX509Certx509certgetprivatekey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetprivatekey", (RTS_UINTPTR)x509certgetprivatekey, 1, 0x279C9A63, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certgetprivatekey
	#define EXT_x509certgetprivatekey
	#define GET_x509certgetprivatekey(fl)  CAL_CMGETAPI( "x509certgetprivatekey" ) 
	#define CAL_x509certgetprivatekey  x509certgetprivatekey
	#define CHK_x509certgetprivatekey  TRUE
	#define EXP_x509certgetprivatekey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetprivatekey", (RTS_UINTPTR)x509certgetprivatekey, 1, 0x279C9A63, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certgetprivatekey  PFX509CERTGETPRIVATEKEY_IEC pfx509certgetprivatekey;
	#define EXT_x509certgetprivatekey  extern PFX509CERTGETPRIVATEKEY_IEC pfx509certgetprivatekey;
	#define GET_x509certgetprivatekey(fl)  s_pfCMGetAPI2( "x509certgetprivatekey", (RTS_VOID_FCTPTR *)&pfx509certgetprivatekey, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x279C9A63, 0x03050B00)
	#define CAL_x509certgetprivatekey  pfx509certgetprivatekey
	#define CHK_x509certgetprivatekey  (pfx509certgetprivatekey != NULL)
	#define EXP_x509certgetprivatekey   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetprivatekey", (RTS_UINTPTR)x509certgetprivatekey, 1, 0x279C9A63, 0x03050B00) 
#endif


/**
 * Get the PUBLIC key FROM the certificate.
 *
 * The format of this key is an implementation detail and has to match
 * the expected format of the asymmetric functions of the CmpCrpyotItf.
 * The key data is opaque for the user of the keys. If the keys are needed in a
 * defined exchange format use the CryptoKeyExport function of the CmpCryptoItf.
 * :return: Result of the operation. 
 */
typedef struct tagx509certgetpublickey_struct
{
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	/* Handle of the certificate of intrest. */
	RtsCryptoKey *pPublicKey;			/* VAR_INPUT */	/* Pointer to the crypto key storing the public key. */
	RTS_IEC_RESULT X509CertGetPublicKey;	/* VAR_OUTPUT */	
} x509certgetpublickey_struct;

void CDECL CDECL_EXT x509certgetpublickey(x509certgetpublickey_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTGETPUBLICKEY_IEC) (x509certgetpublickey_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETPUBLICKEY_NOTIMPLEMENTED)
	#define USE_x509certgetpublickey
	#define EXT_x509certgetpublickey
	#define GET_x509certgetpublickey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certgetpublickey(p0) 
	#define CHK_x509certgetpublickey  FALSE
	#define EXP_x509certgetpublickey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certgetpublickey
	#define EXT_x509certgetpublickey
	#define GET_x509certgetpublickey(fl)  CAL_CMGETAPI( "x509certgetpublickey" ) 
	#define CAL_x509certgetpublickey  x509certgetpublickey
	#define CHK_x509certgetpublickey  TRUE
	#define EXP_x509certgetpublickey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetpublickey", (RTS_UINTPTR)x509certgetpublickey, 1, 0x35142E48, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certgetpublickey
	#define EXT_x509certgetpublickey
	#define GET_x509certgetpublickey(fl)  CAL_CMGETAPI( "x509certgetpublickey" ) 
	#define CAL_x509certgetpublickey  x509certgetpublickey
	#define CHK_x509certgetpublickey  TRUE
	#define EXP_x509certgetpublickey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetpublickey", (RTS_UINTPTR)x509certgetpublickey, 1, 0x35142E48, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certgetpublickey
	#define EXT_CmpX509Certx509certgetpublickey
	#define GET_CmpX509Certx509certgetpublickey  ERR_OK
	#define CAL_CmpX509Certx509certgetpublickey  x509certgetpublickey
	#define CHK_CmpX509Certx509certgetpublickey  TRUE
	#define EXP_CmpX509Certx509certgetpublickey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetpublickey", (RTS_UINTPTR)x509certgetpublickey, 1, 0x35142E48, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certgetpublickey
	#define EXT_x509certgetpublickey
	#define GET_x509certgetpublickey(fl)  CAL_CMGETAPI( "x509certgetpublickey" ) 
	#define CAL_x509certgetpublickey  x509certgetpublickey
	#define CHK_x509certgetpublickey  TRUE
	#define EXP_x509certgetpublickey  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetpublickey", (RTS_UINTPTR)x509certgetpublickey, 1, 0x35142E48, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certgetpublickey  PFX509CERTGETPUBLICKEY_IEC pfx509certgetpublickey;
	#define EXT_x509certgetpublickey  extern PFX509CERTGETPUBLICKEY_IEC pfx509certgetpublickey;
	#define GET_x509certgetpublickey(fl)  s_pfCMGetAPI2( "x509certgetpublickey", (RTS_VOID_FCTPTR *)&pfx509certgetpublickey, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x35142E48, 0x03050B00)
	#define CAL_x509certgetpublickey  pfx509certgetpublickey
	#define CHK_x509certgetpublickey  (pfx509certgetpublickey != NULL)
	#define EXP_x509certgetpublickey   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetpublickey", (RTS_UINTPTR)x509certgetpublickey, 1, 0x35142E48, 0x03050B00) 
#endif


/**
 * Generate the certificate thumbprint. The thumbprint will be just the plain bytes of the hash. 
 * The thumprint can be used to identify certificates of the local store. The thumprint may not be globally unique.
 * :return: Result of the operation.
 */
typedef struct tagx509certgetthumbprint_struct
{
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	/* Handle of the certificate of intrest. */
	RTS_IEC_UDINT *hashAlgorithm;		/* VAR_INPUT */	/* The hashing algorithmen used to generate the thumbprint. */
	RtsByteString *pThumprint;			/* VAR_INPUT */	/* Buffer to store the thumbprint. If bytestring is NULL memory ia allocated. */
	RTS_IEC_RESULT X509CertGetThumbprint;	/* VAR_OUTPUT */	
} x509certgetthumbprint_struct;

void CDECL CDECL_EXT x509certgetthumbprint(x509certgetthumbprint_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTGETTHUMBPRINT_IEC) (x509certgetthumbprint_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETTHUMBPRINT_NOTIMPLEMENTED)
	#define USE_x509certgetthumbprint
	#define EXT_x509certgetthumbprint
	#define GET_x509certgetthumbprint(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certgetthumbprint(p0) 
	#define CHK_x509certgetthumbprint  FALSE
	#define EXP_x509certgetthumbprint  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certgetthumbprint
	#define EXT_x509certgetthumbprint
	#define GET_x509certgetthumbprint(fl)  CAL_CMGETAPI( "x509certgetthumbprint" ) 
	#define CAL_x509certgetthumbprint  x509certgetthumbprint
	#define CHK_x509certgetthumbprint  TRUE
	#define EXP_x509certgetthumbprint  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetthumbprint", (RTS_UINTPTR)x509certgetthumbprint, 1, 0x5B1EA1C5, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certgetthumbprint
	#define EXT_x509certgetthumbprint
	#define GET_x509certgetthumbprint(fl)  CAL_CMGETAPI( "x509certgetthumbprint" ) 
	#define CAL_x509certgetthumbprint  x509certgetthumbprint
	#define CHK_x509certgetthumbprint  TRUE
	#define EXP_x509certgetthumbprint  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetthumbprint", (RTS_UINTPTR)x509certgetthumbprint, 1, 0x5B1EA1C5, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certgetthumbprint
	#define EXT_CmpX509Certx509certgetthumbprint
	#define GET_CmpX509Certx509certgetthumbprint  ERR_OK
	#define CAL_CmpX509Certx509certgetthumbprint  x509certgetthumbprint
	#define CHK_CmpX509Certx509certgetthumbprint  TRUE
	#define EXP_CmpX509Certx509certgetthumbprint  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetthumbprint", (RTS_UINTPTR)x509certgetthumbprint, 1, 0x5B1EA1C5, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certgetthumbprint
	#define EXT_x509certgetthumbprint
	#define GET_x509certgetthumbprint(fl)  CAL_CMGETAPI( "x509certgetthumbprint" ) 
	#define CAL_x509certgetthumbprint  x509certgetthumbprint
	#define CHK_x509certgetthumbprint  TRUE
	#define EXP_x509certgetthumbprint  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetthumbprint", (RTS_UINTPTR)x509certgetthumbprint, 1, 0x5B1EA1C5, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certgetthumbprint  PFX509CERTGETTHUMBPRINT_IEC pfx509certgetthumbprint;
	#define EXT_x509certgetthumbprint  extern PFX509CERTGETTHUMBPRINT_IEC pfx509certgetthumbprint;
	#define GET_x509certgetthumbprint(fl)  s_pfCMGetAPI2( "x509certgetthumbprint", (RTS_VOID_FCTPTR *)&pfx509certgetthumbprint, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x5B1EA1C5, 0x03050B00)
	#define CAL_x509certgetthumbprint  pfx509certgetthumbprint
	#define CHK_x509certgetthumbprint  (pfx509certgetthumbprint != NULL)
	#define EXP_x509certgetthumbprint   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certgetthumbprint", (RTS_UINTPTR)x509certgetthumbprint, 1, 0x5B1EA1C5, 0x03050B00) 
#endif


/**
 * Cleanup an RtsX509CertInfo structure. All allocated memory used by this strucutre will be release.
 * :return: Result of the operation
 */
typedef struct tagx509certinfoexit_struct
{
	RtsX509CertInfo *pCertInfo;			/* VAR_INPUT */	/* Pointer to the object to clean up. */
	RTS_IEC_RESULT X509CertInfoExit;	/* VAR_OUTPUT */	
} x509certinfoexit_struct;

void CDECL CDECL_EXT x509certinfoexit(x509certinfoexit_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTINFOEXIT_IEC) (x509certinfoexit_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTINFOEXIT_NOTIMPLEMENTED)
	#define USE_x509certinfoexit
	#define EXT_x509certinfoexit
	#define GET_x509certinfoexit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certinfoexit(p0) 
	#define CHK_x509certinfoexit  FALSE
	#define EXP_x509certinfoexit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certinfoexit
	#define EXT_x509certinfoexit
	#define GET_x509certinfoexit(fl)  CAL_CMGETAPI( "x509certinfoexit" ) 
	#define CAL_x509certinfoexit  x509certinfoexit
	#define CHK_x509certinfoexit  TRUE
	#define EXP_x509certinfoexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoexit", (RTS_UINTPTR)x509certinfoexit, 1, 0x4F4B0310, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certinfoexit
	#define EXT_x509certinfoexit
	#define GET_x509certinfoexit(fl)  CAL_CMGETAPI( "x509certinfoexit" ) 
	#define CAL_x509certinfoexit  x509certinfoexit
	#define CHK_x509certinfoexit  TRUE
	#define EXP_x509certinfoexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoexit", (RTS_UINTPTR)x509certinfoexit, 1, 0x4F4B0310, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certinfoexit
	#define EXT_CmpX509Certx509certinfoexit
	#define GET_CmpX509Certx509certinfoexit  ERR_OK
	#define CAL_CmpX509Certx509certinfoexit  x509certinfoexit
	#define CHK_CmpX509Certx509certinfoexit  TRUE
	#define EXP_CmpX509Certx509certinfoexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoexit", (RTS_UINTPTR)x509certinfoexit, 1, 0x4F4B0310, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certinfoexit
	#define EXT_x509certinfoexit
	#define GET_x509certinfoexit(fl)  CAL_CMGETAPI( "x509certinfoexit" ) 
	#define CAL_x509certinfoexit  x509certinfoexit
	#define CHK_x509certinfoexit  TRUE
	#define EXP_x509certinfoexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoexit", (RTS_UINTPTR)x509certinfoexit, 1, 0x4F4B0310, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certinfoexit  PFX509CERTINFOEXIT_IEC pfx509certinfoexit;
	#define EXT_x509certinfoexit  extern PFX509CERTINFOEXIT_IEC pfx509certinfoexit;
	#define GET_x509certinfoexit(fl)  s_pfCMGetAPI2( "x509certinfoexit", (RTS_VOID_FCTPTR *)&pfx509certinfoexit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x4F4B0310, 0x03050B00)
	#define CAL_x509certinfoexit  pfx509certinfoexit
	#define CHK_x509certinfoexit  (pfx509certinfoexit != NULL)
	#define EXP_x509certinfoexit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoexit", (RTS_UINTPTR)x509certinfoexit, 1, 0x4F4B0310, 0x03050B00) 
#endif


/**
 * Initialize the RtsX509CertInfo structure.
 * :return: Result of the operation
 */
typedef struct tagx509certinfoinit_struct
{
	RtsX509CertInfo *pCertInfo;			/* VAR_INPUT */	/* Pointer to the structure. */
	RTS_IEC_UDINT structSize;			/* VAR_INPUT */	/* Size of the structure. */
	RTS_IEC_RESULT X509CertInfoInit;	/* VAR_OUTPUT */	
} x509certinfoinit_struct;

void CDECL CDECL_EXT x509certinfoinit(x509certinfoinit_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTINFOINIT_IEC) (x509certinfoinit_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTINFOINIT_NOTIMPLEMENTED)
	#define USE_x509certinfoinit
	#define EXT_x509certinfoinit
	#define GET_x509certinfoinit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certinfoinit(p0) 
	#define CHK_x509certinfoinit  FALSE
	#define EXP_x509certinfoinit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certinfoinit
	#define EXT_x509certinfoinit
	#define GET_x509certinfoinit(fl)  CAL_CMGETAPI( "x509certinfoinit" ) 
	#define CAL_x509certinfoinit  x509certinfoinit
	#define CHK_x509certinfoinit  TRUE
	#define EXP_x509certinfoinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoinit", (RTS_UINTPTR)x509certinfoinit, 1, 0xD52F28DD, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certinfoinit
	#define EXT_x509certinfoinit
	#define GET_x509certinfoinit(fl)  CAL_CMGETAPI( "x509certinfoinit" ) 
	#define CAL_x509certinfoinit  x509certinfoinit
	#define CHK_x509certinfoinit  TRUE
	#define EXP_x509certinfoinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoinit", (RTS_UINTPTR)x509certinfoinit, 1, 0xD52F28DD, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certinfoinit
	#define EXT_CmpX509Certx509certinfoinit
	#define GET_CmpX509Certx509certinfoinit  ERR_OK
	#define CAL_CmpX509Certx509certinfoinit  x509certinfoinit
	#define CHK_CmpX509Certx509certinfoinit  TRUE
	#define EXP_CmpX509Certx509certinfoinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoinit", (RTS_UINTPTR)x509certinfoinit, 1, 0xD52F28DD, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certinfoinit
	#define EXT_x509certinfoinit
	#define GET_x509certinfoinit(fl)  CAL_CMGETAPI( "x509certinfoinit" ) 
	#define CAL_x509certinfoinit  x509certinfoinit
	#define CHK_x509certinfoinit  TRUE
	#define EXP_x509certinfoinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoinit", (RTS_UINTPTR)x509certinfoinit, 1, 0xD52F28DD, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certinfoinit  PFX509CERTINFOINIT_IEC pfx509certinfoinit;
	#define EXT_x509certinfoinit  extern PFX509CERTINFOINIT_IEC pfx509certinfoinit;
	#define GET_x509certinfoinit(fl)  s_pfCMGetAPI2( "x509certinfoinit", (RTS_VOID_FCTPTR *)&pfx509certinfoinit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xD52F28DD, 0x03050B00)
	#define CAL_x509certinfoinit  pfx509certinfoinit
	#define CHK_x509certinfoinit  (pfx509certinfoinit != NULL)
	#define EXP_x509certinfoinit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certinfoinit", (RTS_UINTPTR)x509certinfoinit, 1, 0xD52F28DD, 0x03050B00) 
#endif


/**
 * Release a PRIVATE OR PUBLIC key with THIS FUNCTION. THIS FUNCTION
 * indicates that this key is not needed anymore and allows to clean up ressources
 * used by this key
 * :return: Result of the operation
 */
typedef struct tagx509certkeyclose_struct
{
	RtsCryptoKey *pKey;					/* VAR_INPUT */	/* Pointer to the key */
	RTS_IEC_RESULT X509CertKeyClose;	/* VAR_OUTPUT */	
} x509certkeyclose_struct;

void CDECL CDECL_EXT x509certkeyclose(x509certkeyclose_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTKEYCLOSE_IEC) (x509certkeyclose_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTKEYCLOSE_NOTIMPLEMENTED)
	#define USE_x509certkeyclose
	#define EXT_x509certkeyclose
	#define GET_x509certkeyclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certkeyclose(p0) 
	#define CHK_x509certkeyclose  FALSE
	#define EXP_x509certkeyclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certkeyclose
	#define EXT_x509certkeyclose
	#define GET_x509certkeyclose(fl)  CAL_CMGETAPI( "x509certkeyclose" ) 
	#define CAL_x509certkeyclose  x509certkeyclose
	#define CHK_x509certkeyclose  TRUE
	#define EXP_x509certkeyclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certkeyclose", (RTS_UINTPTR)x509certkeyclose, 1, 0x6A598D05, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certkeyclose
	#define EXT_x509certkeyclose
	#define GET_x509certkeyclose(fl)  CAL_CMGETAPI( "x509certkeyclose" ) 
	#define CAL_x509certkeyclose  x509certkeyclose
	#define CHK_x509certkeyclose  TRUE
	#define EXP_x509certkeyclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certkeyclose", (RTS_UINTPTR)x509certkeyclose, 1, 0x6A598D05, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certkeyclose
	#define EXT_CmpX509Certx509certkeyclose
	#define GET_CmpX509Certx509certkeyclose  ERR_OK
	#define CAL_CmpX509Certx509certkeyclose  x509certkeyclose
	#define CHK_CmpX509Certx509certkeyclose  TRUE
	#define EXP_CmpX509Certx509certkeyclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certkeyclose", (RTS_UINTPTR)x509certkeyclose, 1, 0x6A598D05, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certkeyclose
	#define EXT_x509certkeyclose
	#define GET_x509certkeyclose(fl)  CAL_CMGETAPI( "x509certkeyclose" ) 
	#define CAL_x509certkeyclose  x509certkeyclose
	#define CHK_x509certkeyclose  TRUE
	#define EXP_x509certkeyclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certkeyclose", (RTS_UINTPTR)x509certkeyclose, 1, 0x6A598D05, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certkeyclose  PFX509CERTKEYCLOSE_IEC pfx509certkeyclose;
	#define EXT_x509certkeyclose  extern PFX509CERTKEYCLOSE_IEC pfx509certkeyclose;
	#define GET_x509certkeyclose(fl)  s_pfCMGetAPI2( "x509certkeyclose", (RTS_VOID_FCTPTR *)&pfx509certkeyclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x6A598D05, 0x03050B00)
	#define CAL_x509certkeyclose  pfx509certkeyclose
	#define CHK_x509certkeyclose  (pfx509certkeyclose != NULL)
	#define EXP_x509certkeyclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certkeyclose", (RTS_UINTPTR)x509certkeyclose, 1, 0x6A598D05, 0x03050B00) 
#endif


/**
 * <description>x509certstoreaddcert</description>
 */
typedef struct tagx509certstoreaddcert_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hBackend;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	
	RTS_IEC_INT trustLevel;				/* VAR_INPUT, Enum: RTSCERTTRUSTLEVEL */
	RTS_IEC_RESULT X509CertStoreAddCert;	/* VAR_OUTPUT */	
} x509certstoreaddcert_struct;

void CDECL CDECL_EXT x509certstoreaddcert(x509certstoreaddcert_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTOREADDCERT_IEC) (x509certstoreaddcert_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREADDCERT_NOTIMPLEMENTED)
	#define USE_x509certstoreaddcert
	#define EXT_x509certstoreaddcert
	#define GET_x509certstoreaddcert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoreaddcert(p0) 
	#define CHK_x509certstoreaddcert  FALSE
	#define EXP_x509certstoreaddcert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoreaddcert
	#define EXT_x509certstoreaddcert
	#define GET_x509certstoreaddcert(fl)  CAL_CMGETAPI( "x509certstoreaddcert" ) 
	#define CAL_x509certstoreaddcert  x509certstoreaddcert
	#define CHK_x509certstoreaddcert  TRUE
	#define EXP_x509certstoreaddcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreaddcert", (RTS_UINTPTR)x509certstoreaddcert, 1, 0x690E5D79, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoreaddcert
	#define EXT_x509certstoreaddcert
	#define GET_x509certstoreaddcert(fl)  CAL_CMGETAPI( "x509certstoreaddcert" ) 
	#define CAL_x509certstoreaddcert  x509certstoreaddcert
	#define CHK_x509certstoreaddcert  TRUE
	#define EXP_x509certstoreaddcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreaddcert", (RTS_UINTPTR)x509certstoreaddcert, 1, 0x690E5D79, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoreaddcert
	#define EXT_CmpX509Certx509certstoreaddcert
	#define GET_CmpX509Certx509certstoreaddcert  ERR_OK
	#define CAL_CmpX509Certx509certstoreaddcert  x509certstoreaddcert
	#define CHK_CmpX509Certx509certstoreaddcert  TRUE
	#define EXP_CmpX509Certx509certstoreaddcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreaddcert", (RTS_UINTPTR)x509certstoreaddcert, 1, 0x690E5D79, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoreaddcert
	#define EXT_x509certstoreaddcert
	#define GET_x509certstoreaddcert(fl)  CAL_CMGETAPI( "x509certstoreaddcert" ) 
	#define CAL_x509certstoreaddcert  x509certstoreaddcert
	#define CHK_x509certstoreaddcert  TRUE
	#define EXP_x509certstoreaddcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreaddcert", (RTS_UINTPTR)x509certstoreaddcert, 1, 0x690E5D79, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoreaddcert  PFX509CERTSTOREADDCERT_IEC pfx509certstoreaddcert;
	#define EXT_x509certstoreaddcert  extern PFX509CERTSTOREADDCERT_IEC pfx509certstoreaddcert;
	#define GET_x509certstoreaddcert(fl)  s_pfCMGetAPI2( "x509certstoreaddcert", (RTS_VOID_FCTPTR *)&pfx509certstoreaddcert, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x690E5D79, 0x03050B00)
	#define CAL_x509certstoreaddcert  pfx509certstoreaddcert
	#define CHK_x509certstoreaddcert  (pfx509certstoreaddcert != NULL)
	#define EXP_x509certstoreaddcert   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreaddcert", (RTS_UINTPTR)x509certstoreaddcert, 1, 0x690E5D79, 0x03050B00) 
#endif


/**
 * <description>x509certstoreclose</description>
 */
typedef struct tagx509certstoreclose_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_RESULT X509CertStoreClose;	/* VAR_OUTPUT */	
} x509certstoreclose_struct;

void CDECL CDECL_EXT x509certstoreclose(x509certstoreclose_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTORECLOSE_IEC) (x509certstoreclose_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTORECLOSE_NOTIMPLEMENTED)
	#define USE_x509certstoreclose
	#define EXT_x509certstoreclose
	#define GET_x509certstoreclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoreclose(p0) 
	#define CHK_x509certstoreclose  FALSE
	#define EXP_x509certstoreclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoreclose
	#define EXT_x509certstoreclose
	#define GET_x509certstoreclose(fl)  CAL_CMGETAPI( "x509certstoreclose" ) 
	#define CAL_x509certstoreclose  x509certstoreclose
	#define CHK_x509certstoreclose  TRUE
	#define EXP_x509certstoreclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreclose", (RTS_UINTPTR)x509certstoreclose, 1, 0x048C3968, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoreclose
	#define EXT_x509certstoreclose
	#define GET_x509certstoreclose(fl)  CAL_CMGETAPI( "x509certstoreclose" ) 
	#define CAL_x509certstoreclose  x509certstoreclose
	#define CHK_x509certstoreclose  TRUE
	#define EXP_x509certstoreclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreclose", (RTS_UINTPTR)x509certstoreclose, 1, 0x048C3968, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoreclose
	#define EXT_CmpX509Certx509certstoreclose
	#define GET_CmpX509Certx509certstoreclose  ERR_OK
	#define CAL_CmpX509Certx509certstoreclose  x509certstoreclose
	#define CHK_CmpX509Certx509certstoreclose  TRUE
	#define EXP_CmpX509Certx509certstoreclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreclose", (RTS_UINTPTR)x509certstoreclose, 1, 0x048C3968, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoreclose
	#define EXT_x509certstoreclose
	#define GET_x509certstoreclose(fl)  CAL_CMGETAPI( "x509certstoreclose" ) 
	#define CAL_x509certstoreclose  x509certstoreclose
	#define CHK_x509certstoreclose  TRUE
	#define EXP_x509certstoreclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreclose", (RTS_UINTPTR)x509certstoreclose, 1, 0x048C3968, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoreclose  PFX509CERTSTORECLOSE_IEC pfx509certstoreclose;
	#define EXT_x509certstoreclose  extern PFX509CERTSTORECLOSE_IEC pfx509certstoreclose;
	#define GET_x509certstoreclose(fl)  s_pfCMGetAPI2( "x509certstoreclose", (RTS_VOID_FCTPTR *)&pfx509certstoreclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x048C3968, 0x03050B00)
	#define CAL_x509certstoreclose  pfx509certstoreclose
	#define CHK_x509certstoreclose  (pfx509certstoreclose != NULL)
	#define EXP_x509certstoreclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreclose", (RTS_UINTPTR)x509certstoreclose, 1, 0x048C3968, 0x03050B00) 
#endif


/**
 * <description>x509certstoregetfirstcert</description>
 */
typedef struct tagx509certstoregetfirstcert_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hBackend;			/* VAR_INPUT */	
	RTS_IEC_INT trustLevel;				/* VAR_INPUT, Enum: RTSCERTTRUSTLEVEL */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE X509CertStoreGetFirstCert;	/* VAR_OUTPUT */	
} x509certstoregetfirstcert_struct;

void CDECL CDECL_EXT x509certstoregetfirstcert(x509certstoregetfirstcert_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTOREGETFIRSTCERT_IEC) (x509certstoregetfirstcert_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREGETFIRSTCERT_NOTIMPLEMENTED)
	#define USE_x509certstoregetfirstcert
	#define EXT_x509certstoregetfirstcert
	#define GET_x509certstoregetfirstcert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoregetfirstcert(p0) 
	#define CHK_x509certstoregetfirstcert  FALSE
	#define EXP_x509certstoregetfirstcert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoregetfirstcert
	#define EXT_x509certstoregetfirstcert
	#define GET_x509certstoregetfirstcert(fl)  CAL_CMGETAPI( "x509certstoregetfirstcert" ) 
	#define CAL_x509certstoregetfirstcert  x509certstoregetfirstcert
	#define CHK_x509certstoregetfirstcert  TRUE
	#define EXP_x509certstoregetfirstcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetfirstcert", (RTS_UINTPTR)x509certstoregetfirstcert, 1, 0x9165907D, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoregetfirstcert
	#define EXT_x509certstoregetfirstcert
	#define GET_x509certstoregetfirstcert(fl)  CAL_CMGETAPI( "x509certstoregetfirstcert" ) 
	#define CAL_x509certstoregetfirstcert  x509certstoregetfirstcert
	#define CHK_x509certstoregetfirstcert  TRUE
	#define EXP_x509certstoregetfirstcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetfirstcert", (RTS_UINTPTR)x509certstoregetfirstcert, 1, 0x9165907D, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoregetfirstcert
	#define EXT_CmpX509Certx509certstoregetfirstcert
	#define GET_CmpX509Certx509certstoregetfirstcert  ERR_OK
	#define CAL_CmpX509Certx509certstoregetfirstcert  x509certstoregetfirstcert
	#define CHK_CmpX509Certx509certstoregetfirstcert  TRUE
	#define EXP_CmpX509Certx509certstoregetfirstcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetfirstcert", (RTS_UINTPTR)x509certstoregetfirstcert, 1, 0x9165907D, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoregetfirstcert
	#define EXT_x509certstoregetfirstcert
	#define GET_x509certstoregetfirstcert(fl)  CAL_CMGETAPI( "x509certstoregetfirstcert" ) 
	#define CAL_x509certstoregetfirstcert  x509certstoregetfirstcert
	#define CHK_x509certstoregetfirstcert  TRUE
	#define EXP_x509certstoregetfirstcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetfirstcert", (RTS_UINTPTR)x509certstoregetfirstcert, 1, 0x9165907D, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoregetfirstcert  PFX509CERTSTOREGETFIRSTCERT_IEC pfx509certstoregetfirstcert;
	#define EXT_x509certstoregetfirstcert  extern PFX509CERTSTOREGETFIRSTCERT_IEC pfx509certstoregetfirstcert;
	#define GET_x509certstoregetfirstcert(fl)  s_pfCMGetAPI2( "x509certstoregetfirstcert", (RTS_VOID_FCTPTR *)&pfx509certstoregetfirstcert, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x9165907D, 0x03050B00)
	#define CAL_x509certstoregetfirstcert  pfx509certstoregetfirstcert
	#define CHK_x509certstoregetfirstcert  (pfx509certstoregetfirstcert != NULL)
	#define EXP_x509certstoregetfirstcert   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetfirstcert", (RTS_UINTPTR)x509certstoregetfirstcert, 1, 0x9165907D, 0x03050B00) 
#endif


/**
 * <description>x509certstoregetnextcert</description>
 */
typedef struct tagx509certstoregetnextcert_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hBackend;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE X509CertStoreGetNextCert;	/* VAR_OUTPUT */	
} x509certstoregetnextcert_struct;

void CDECL CDECL_EXT x509certstoregetnextcert(x509certstoregetnextcert_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTOREGETNEXTCERT_IEC) (x509certstoregetnextcert_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREGETNEXTCERT_NOTIMPLEMENTED)
	#define USE_x509certstoregetnextcert
	#define EXT_x509certstoregetnextcert
	#define GET_x509certstoregetnextcert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoregetnextcert(p0) 
	#define CHK_x509certstoregetnextcert  FALSE
	#define EXP_x509certstoregetnextcert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoregetnextcert
	#define EXT_x509certstoregetnextcert
	#define GET_x509certstoregetnextcert(fl)  CAL_CMGETAPI( "x509certstoregetnextcert" ) 
	#define CAL_x509certstoregetnextcert  x509certstoregetnextcert
	#define CHK_x509certstoregetnextcert  TRUE
	#define EXP_x509certstoregetnextcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetnextcert", (RTS_UINTPTR)x509certstoregetnextcert, 1, 0x894377FA, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoregetnextcert
	#define EXT_x509certstoregetnextcert
	#define GET_x509certstoregetnextcert(fl)  CAL_CMGETAPI( "x509certstoregetnextcert" ) 
	#define CAL_x509certstoregetnextcert  x509certstoregetnextcert
	#define CHK_x509certstoregetnextcert  TRUE
	#define EXP_x509certstoregetnextcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetnextcert", (RTS_UINTPTR)x509certstoregetnextcert, 1, 0x894377FA, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoregetnextcert
	#define EXT_CmpX509Certx509certstoregetnextcert
	#define GET_CmpX509Certx509certstoregetnextcert  ERR_OK
	#define CAL_CmpX509Certx509certstoregetnextcert  x509certstoregetnextcert
	#define CHK_CmpX509Certx509certstoregetnextcert  TRUE
	#define EXP_CmpX509Certx509certstoregetnextcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetnextcert", (RTS_UINTPTR)x509certstoregetnextcert, 1, 0x894377FA, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoregetnextcert
	#define EXT_x509certstoregetnextcert
	#define GET_x509certstoregetnextcert(fl)  CAL_CMGETAPI( "x509certstoregetnextcert" ) 
	#define CAL_x509certstoregetnextcert  x509certstoregetnextcert
	#define CHK_x509certstoregetnextcert  TRUE
	#define EXP_x509certstoregetnextcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetnextcert", (RTS_UINTPTR)x509certstoregetnextcert, 1, 0x894377FA, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoregetnextcert  PFX509CERTSTOREGETNEXTCERT_IEC pfx509certstoregetnextcert;
	#define EXT_x509certstoregetnextcert  extern PFX509CERTSTOREGETNEXTCERT_IEC pfx509certstoregetnextcert;
	#define GET_x509certstoregetnextcert(fl)  s_pfCMGetAPI2( "x509certstoregetnextcert", (RTS_VOID_FCTPTR *)&pfx509certstoregetnextcert, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x894377FA, 0x03050B00)
	#define CAL_x509certstoregetnextcert  pfx509certstoregetnextcert
	#define CHK_x509certstoregetnextcert  (pfx509certstoregetnextcert != NULL)
	#define EXP_x509certstoregetnextcert   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetnextcert", (RTS_UINTPTR)x509certstoregetnextcert, 1, 0x894377FA, 0x03050B00) 
#endif


/**
 * <description>x509certstoregetregisteredcert</description>
 */
typedef struct tagx509certstoregetregisteredcert_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hUseCase;			/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE X509CertStoreGetRegisteredCert;	/* VAR_OUTPUT */	
} x509certstoregetregisteredcert_struct;

void CDECL CDECL_EXT x509certstoregetregisteredcert(x509certstoregetregisteredcert_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTOREGETREGISTEREDCERT_IEC) (x509certstoregetregisteredcert_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREGETREGISTEREDCERT_NOTIMPLEMENTED)
	#define USE_x509certstoregetregisteredcert
	#define EXT_x509certstoregetregisteredcert
	#define GET_x509certstoregetregisteredcert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoregetregisteredcert(p0) 
	#define CHK_x509certstoregetregisteredcert  FALSE
	#define EXP_x509certstoregetregisteredcert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoregetregisteredcert
	#define EXT_x509certstoregetregisteredcert
	#define GET_x509certstoregetregisteredcert(fl)  CAL_CMGETAPI( "x509certstoregetregisteredcert" ) 
	#define CAL_x509certstoregetregisteredcert  x509certstoregetregisteredcert
	#define CHK_x509certstoregetregisteredcert  TRUE
	#define EXP_x509certstoregetregisteredcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetregisteredcert", (RTS_UINTPTR)x509certstoregetregisteredcert, 1, 0x8C0F7D34, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoregetregisteredcert
	#define EXT_x509certstoregetregisteredcert
	#define GET_x509certstoregetregisteredcert(fl)  CAL_CMGETAPI( "x509certstoregetregisteredcert" ) 
	#define CAL_x509certstoregetregisteredcert  x509certstoregetregisteredcert
	#define CHK_x509certstoregetregisteredcert  TRUE
	#define EXP_x509certstoregetregisteredcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetregisteredcert", (RTS_UINTPTR)x509certstoregetregisteredcert, 1, 0x8C0F7D34, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoregetregisteredcert
	#define EXT_CmpX509Certx509certstoregetregisteredcert
	#define GET_CmpX509Certx509certstoregetregisteredcert  ERR_OK
	#define CAL_CmpX509Certx509certstoregetregisteredcert  x509certstoregetregisteredcert
	#define CHK_CmpX509Certx509certstoregetregisteredcert  TRUE
	#define EXP_CmpX509Certx509certstoregetregisteredcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetregisteredcert", (RTS_UINTPTR)x509certstoregetregisteredcert, 1, 0x8C0F7D34, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoregetregisteredcert
	#define EXT_x509certstoregetregisteredcert
	#define GET_x509certstoregetregisteredcert(fl)  CAL_CMGETAPI( "x509certstoregetregisteredcert" ) 
	#define CAL_x509certstoregetregisteredcert  x509certstoregetregisteredcert
	#define CHK_x509certstoregetregisteredcert  TRUE
	#define EXP_x509certstoregetregisteredcert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetregisteredcert", (RTS_UINTPTR)x509certstoregetregisteredcert, 1, 0x8C0F7D34, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoregetregisteredcert  PFX509CERTSTOREGETREGISTEREDCERT_IEC pfx509certstoregetregisteredcert;
	#define EXT_x509certstoregetregisteredcert  extern PFX509CERTSTOREGETREGISTEREDCERT_IEC pfx509certstoregetregisteredcert;
	#define GET_x509certstoregetregisteredcert(fl)  s_pfCMGetAPI2( "x509certstoregetregisteredcert", (RTS_VOID_FCTPTR *)&pfx509certstoregetregisteredcert, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x8C0F7D34, 0x03050B00)
	#define CAL_x509certstoregetregisteredcert  pfx509certstoregetregisteredcert
	#define CHK_x509certstoregetregisteredcert  (pfx509certstoregetregisteredcert != NULL)
	#define EXP_x509certstoregetregisteredcert   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoregetregisteredcert", (RTS_UINTPTR)x509certstoregetregisteredcert, 1, 0x8C0F7D34, 0x03050B00) 
#endif


/**
 * <description>x509certstoreopen</description>
 */
typedef struct tagx509certstoreopen_struct
{
	RTS_IEC_UDINT componentID;			/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE X509CertStoreOpen;	/* VAR_OUTPUT */	
} x509certstoreopen_struct;

void CDECL CDECL_EXT x509certstoreopen(x509certstoreopen_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTOREOPEN_IEC) (x509certstoreopen_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREOPEN_NOTIMPLEMENTED)
	#define USE_x509certstoreopen
	#define EXT_x509certstoreopen
	#define GET_x509certstoreopen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoreopen(p0) 
	#define CHK_x509certstoreopen  FALSE
	#define EXP_x509certstoreopen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoreopen
	#define EXT_x509certstoreopen
	#define GET_x509certstoreopen(fl)  CAL_CMGETAPI( "x509certstoreopen" ) 
	#define CAL_x509certstoreopen  x509certstoreopen
	#define CHK_x509certstoreopen  TRUE
	#define EXP_x509certstoreopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreopen", (RTS_UINTPTR)x509certstoreopen, 1, 0xFE7CCE6C, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoreopen
	#define EXT_x509certstoreopen
	#define GET_x509certstoreopen(fl)  CAL_CMGETAPI( "x509certstoreopen" ) 
	#define CAL_x509certstoreopen  x509certstoreopen
	#define CHK_x509certstoreopen  TRUE
	#define EXP_x509certstoreopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreopen", (RTS_UINTPTR)x509certstoreopen, 1, 0xFE7CCE6C, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoreopen
	#define EXT_CmpX509Certx509certstoreopen
	#define GET_CmpX509Certx509certstoreopen  ERR_OK
	#define CAL_CmpX509Certx509certstoreopen  x509certstoreopen
	#define CHK_CmpX509Certx509certstoreopen  TRUE
	#define EXP_CmpX509Certx509certstoreopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreopen", (RTS_UINTPTR)x509certstoreopen, 1, 0xFE7CCE6C, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoreopen
	#define EXT_x509certstoreopen
	#define GET_x509certstoreopen(fl)  CAL_CMGETAPI( "x509certstoreopen" ) 
	#define CAL_x509certstoreopen  x509certstoreopen
	#define CHK_x509certstoreopen  TRUE
	#define EXP_x509certstoreopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreopen", (RTS_UINTPTR)x509certstoreopen, 1, 0xFE7CCE6C, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoreopen  PFX509CERTSTOREOPEN_IEC pfx509certstoreopen;
	#define EXT_x509certstoreopen  extern PFX509CERTSTOREOPEN_IEC pfx509certstoreopen;
	#define GET_x509certstoreopen(fl)  s_pfCMGetAPI2( "x509certstoreopen", (RTS_VOID_FCTPTR *)&pfx509certstoreopen, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xFE7CCE6C, 0x03050B00)
	#define CAL_x509certstoreopen  pfx509certstoreopen
	#define CHK_x509certstoreopen  (pfx509certstoreopen != NULL)
	#define EXP_x509certstoreopen   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreopen", (RTS_UINTPTR)x509certstoreopen, 1, 0xFE7CCE6C, 0x03050B00) 
#endif


/**
 * <description>x509certstoreregister</description>
 */
typedef struct tagx509certstoreregister_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_UDINT cmpID;				/* VAR_INPUT */	
	RtsX509CertInfo *pCertInfo;			/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE X509CertStoreRegister;	/* VAR_OUTPUT */	
} x509certstoreregister_struct;

void CDECL CDECL_EXT x509certstoreregister(x509certstoreregister_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTOREREGISTER_IEC) (x509certstoreregister_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREREGISTER_NOTIMPLEMENTED)
	#define USE_x509certstoreregister
	#define EXT_x509certstoreregister
	#define GET_x509certstoreregister(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoreregister(p0) 
	#define CHK_x509certstoreregister  FALSE
	#define EXP_x509certstoreregister  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoreregister
	#define EXT_x509certstoreregister
	#define GET_x509certstoreregister(fl)  CAL_CMGETAPI( "x509certstoreregister" ) 
	#define CAL_x509certstoreregister  x509certstoreregister
	#define CHK_x509certstoreregister  TRUE
	#define EXP_x509certstoreregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreregister", (RTS_UINTPTR)x509certstoreregister, 1, 0x35F7F607, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoreregister
	#define EXT_x509certstoreregister
	#define GET_x509certstoreregister(fl)  CAL_CMGETAPI( "x509certstoreregister" ) 
	#define CAL_x509certstoreregister  x509certstoreregister
	#define CHK_x509certstoreregister  TRUE
	#define EXP_x509certstoreregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreregister", (RTS_UINTPTR)x509certstoreregister, 1, 0x35F7F607, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoreregister
	#define EXT_CmpX509Certx509certstoreregister
	#define GET_CmpX509Certx509certstoreregister  ERR_OK
	#define CAL_CmpX509Certx509certstoreregister  x509certstoreregister
	#define CHK_CmpX509Certx509certstoreregister  TRUE
	#define EXP_CmpX509Certx509certstoreregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreregister", (RTS_UINTPTR)x509certstoreregister, 1, 0x35F7F607, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoreregister
	#define EXT_x509certstoreregister
	#define GET_x509certstoreregister(fl)  CAL_CMGETAPI( "x509certstoreregister" ) 
	#define CAL_x509certstoreregister  x509certstoreregister
	#define CHK_x509certstoreregister  TRUE
	#define EXP_x509certstoreregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreregister", (RTS_UINTPTR)x509certstoreregister, 1, 0x35F7F607, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoreregister  PFX509CERTSTOREREGISTER_IEC pfx509certstoreregister;
	#define EXT_x509certstoreregister  extern PFX509CERTSTOREREGISTER_IEC pfx509certstoreregister;
	#define GET_x509certstoreregister(fl)  s_pfCMGetAPI2( "x509certstoreregister", (RTS_VOID_FCTPTR *)&pfx509certstoreregister, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x35F7F607, 0x03050B00)
	#define CAL_x509certstoreregister  pfx509certstoreregister
	#define CHK_x509certstoreregister  (pfx509certstoreregister != NULL)
	#define EXP_x509certstoreregister   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreregister", (RTS_UINTPTR)x509certstoreregister, 1, 0x35F7F607, 0x03050B00) 
#endif


/**
 * <description>x509certstoreremovecert</description>
 */
typedef struct tagx509certstoreremovecert_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hBackend;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	
	RTS_IEC_BOOL xRemovePrivateKey;		/* VAR_INPUT */	
	RTS_IEC_RESULT X509CertStoreRemoveCert;	/* VAR_OUTPUT */	
} x509certstoreremovecert_struct;

void CDECL CDECL_EXT x509certstoreremovecert(x509certstoreremovecert_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTOREREMOVECERT_IEC) (x509certstoreremovecert_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREREMOVECERT_NOTIMPLEMENTED)
	#define USE_x509certstoreremovecert
	#define EXT_x509certstoreremovecert
	#define GET_x509certstoreremovecert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoreremovecert(p0) 
	#define CHK_x509certstoreremovecert  FALSE
	#define EXP_x509certstoreremovecert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoreremovecert
	#define EXT_x509certstoreremovecert
	#define GET_x509certstoreremovecert(fl)  CAL_CMGETAPI( "x509certstoreremovecert" ) 
	#define CAL_x509certstoreremovecert  x509certstoreremovecert
	#define CHK_x509certstoreremovecert  TRUE
	#define EXP_x509certstoreremovecert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreremovecert", (RTS_UINTPTR)x509certstoreremovecert, 1, 0x0E795BC4, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoreremovecert
	#define EXT_x509certstoreremovecert
	#define GET_x509certstoreremovecert(fl)  CAL_CMGETAPI( "x509certstoreremovecert" ) 
	#define CAL_x509certstoreremovecert  x509certstoreremovecert
	#define CHK_x509certstoreremovecert  TRUE
	#define EXP_x509certstoreremovecert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreremovecert", (RTS_UINTPTR)x509certstoreremovecert, 1, 0x0E795BC4, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoreremovecert
	#define EXT_CmpX509Certx509certstoreremovecert
	#define GET_CmpX509Certx509certstoreremovecert  ERR_OK
	#define CAL_CmpX509Certx509certstoreremovecert  x509certstoreremovecert
	#define CHK_CmpX509Certx509certstoreremovecert  TRUE
	#define EXP_CmpX509Certx509certstoreremovecert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreremovecert", (RTS_UINTPTR)x509certstoreremovecert, 1, 0x0E795BC4, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoreremovecert
	#define EXT_x509certstoreremovecert
	#define GET_x509certstoreremovecert(fl)  CAL_CMGETAPI( "x509certstoreremovecert" ) 
	#define CAL_x509certstoreremovecert  x509certstoreremovecert
	#define CHK_x509certstoreremovecert  TRUE
	#define EXP_x509certstoreremovecert  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreremovecert", (RTS_UINTPTR)x509certstoreremovecert, 1, 0x0E795BC4, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoreremovecert  PFX509CERTSTOREREMOVECERT_IEC pfx509certstoreremovecert;
	#define EXT_x509certstoreremovecert  extern PFX509CERTSTOREREMOVECERT_IEC pfx509certstoreremovecert;
	#define GET_x509certstoreremovecert(fl)  s_pfCMGetAPI2( "x509certstoreremovecert", (RTS_VOID_FCTPTR *)&pfx509certstoreremovecert, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x0E795BC4, 0x03050B00)
	#define CAL_x509certstoreremovecert  pfx509certstoreremovecert
	#define CHK_x509certstoreremovecert  (pfx509certstoreremovecert != NULL)
	#define EXP_x509certstoreremovecert   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreremovecert", (RTS_UINTPTR)x509certstoreremovecert, 1, 0x0E795BC4, 0x03050B00) 
#endif


/**
 * <description>x509certstoresearchgetfirst</description>
 */
typedef struct tagx509certstoresearchgetfirst_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RtsX509CertFilter *pFilters;		/* VAR_INPUT */	
	RTS_IEC_UDINT ui32NumOfFilters;		/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE X509CertStoreSearchGetFirst;	/* VAR_OUTPUT */	
} x509certstoresearchgetfirst_struct;

void CDECL CDECL_EXT x509certstoresearchgetfirst(x509certstoresearchgetfirst_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTORESEARCHGETFIRST_IEC) (x509certstoresearchgetfirst_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTORESEARCHGETFIRST_NOTIMPLEMENTED)
	#define USE_x509certstoresearchgetfirst
	#define EXT_x509certstoresearchgetfirst
	#define GET_x509certstoresearchgetfirst(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoresearchgetfirst(p0) 
	#define CHK_x509certstoresearchgetfirst  FALSE
	#define EXP_x509certstoresearchgetfirst  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoresearchgetfirst
	#define EXT_x509certstoresearchgetfirst
	#define GET_x509certstoresearchgetfirst(fl)  CAL_CMGETAPI( "x509certstoresearchgetfirst" ) 
	#define CAL_x509certstoresearchgetfirst  x509certstoresearchgetfirst
	#define CHK_x509certstoresearchgetfirst  TRUE
	#define EXP_x509certstoresearchgetfirst  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetfirst", (RTS_UINTPTR)x509certstoresearchgetfirst, 1, 0, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoresearchgetfirst
	#define EXT_x509certstoresearchgetfirst
	#define GET_x509certstoresearchgetfirst(fl)  CAL_CMGETAPI( "x509certstoresearchgetfirst" ) 
	#define CAL_x509certstoresearchgetfirst  x509certstoresearchgetfirst
	#define CHK_x509certstoresearchgetfirst  TRUE
	#define EXP_x509certstoresearchgetfirst  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetfirst", (RTS_UINTPTR)x509certstoresearchgetfirst, 1, 0, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoresearchgetfirst
	#define EXT_CmpX509Certx509certstoresearchgetfirst
	#define GET_CmpX509Certx509certstoresearchgetfirst  ERR_OK
	#define CAL_CmpX509Certx509certstoresearchgetfirst  x509certstoresearchgetfirst
	#define CHK_CmpX509Certx509certstoresearchgetfirst  TRUE
	#define EXP_CmpX509Certx509certstoresearchgetfirst  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetfirst", (RTS_UINTPTR)x509certstoresearchgetfirst, 1, 0, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoresearchgetfirst
	#define EXT_x509certstoresearchgetfirst
	#define GET_x509certstoresearchgetfirst(fl)  CAL_CMGETAPI( "x509certstoresearchgetfirst" ) 
	#define CAL_x509certstoresearchgetfirst  x509certstoresearchgetfirst
	#define CHK_x509certstoresearchgetfirst  TRUE
	#define EXP_x509certstoresearchgetfirst  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetfirst", (RTS_UINTPTR)x509certstoresearchgetfirst, 1, 0, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoresearchgetfirst  PFX509CERTSTORESEARCHGETFIRST_IEC pfx509certstoresearchgetfirst;
	#define EXT_x509certstoresearchgetfirst  extern PFX509CERTSTORESEARCHGETFIRST_IEC pfx509certstoresearchgetfirst;
	#define GET_x509certstoresearchgetfirst(fl)  s_pfCMGetAPI2( "x509certstoresearchgetfirst", (RTS_VOID_FCTPTR *)&pfx509certstoresearchgetfirst, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0, 0x03050B00)
	#define CAL_x509certstoresearchgetfirst  pfx509certstoresearchgetfirst
	#define CHK_x509certstoresearchgetfirst  (pfx509certstoresearchgetfirst != NULL)
	#define EXP_x509certstoresearchgetfirst   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetfirst", (RTS_UINTPTR)x509certstoresearchgetfirst, 1, 0, 0x03050B00) 
#endif


/**
 * <description>x509certstoresearchgetnext</description>
 */
typedef struct tagx509certstoresearchgetnext_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	
	RtsX509CertFilter *pFilters;		/* VAR_INPUT */	
	RTS_IEC_UDINT ui32NumOfFilters;		/* VAR_INPUT */	
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	
	RTS_IEC_HANDLE X509CertStoreSearchGetNext;	/* VAR_OUTPUT */	
} x509certstoresearchgetnext_struct;

void CDECL CDECL_EXT x509certstoresearchgetnext(x509certstoresearchgetnext_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTORESEARCHGETNEXT_IEC) (x509certstoresearchgetnext_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTORESEARCHGETNEXT_NOTIMPLEMENTED)
	#define USE_x509certstoresearchgetnext
	#define EXT_x509certstoresearchgetnext
	#define GET_x509certstoresearchgetnext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoresearchgetnext(p0) 
	#define CHK_x509certstoresearchgetnext  FALSE
	#define EXP_x509certstoresearchgetnext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoresearchgetnext
	#define EXT_x509certstoresearchgetnext
	#define GET_x509certstoresearchgetnext(fl)  CAL_CMGETAPI( "x509certstoresearchgetnext" ) 
	#define CAL_x509certstoresearchgetnext  x509certstoresearchgetnext
	#define CHK_x509certstoresearchgetnext  TRUE
	#define EXP_x509certstoresearchgetnext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetnext", (RTS_UINTPTR)x509certstoresearchgetnext, 1, 0, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoresearchgetnext
	#define EXT_x509certstoresearchgetnext
	#define GET_x509certstoresearchgetnext(fl)  CAL_CMGETAPI( "x509certstoresearchgetnext" ) 
	#define CAL_x509certstoresearchgetnext  x509certstoresearchgetnext
	#define CHK_x509certstoresearchgetnext  TRUE
	#define EXP_x509certstoresearchgetnext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetnext", (RTS_UINTPTR)x509certstoresearchgetnext, 1, 0, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoresearchgetnext
	#define EXT_CmpX509Certx509certstoresearchgetnext
	#define GET_CmpX509Certx509certstoresearchgetnext  ERR_OK
	#define CAL_CmpX509Certx509certstoresearchgetnext  x509certstoresearchgetnext
	#define CHK_CmpX509Certx509certstoresearchgetnext  TRUE
	#define EXP_CmpX509Certx509certstoresearchgetnext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetnext", (RTS_UINTPTR)x509certstoresearchgetnext, 1, 0, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoresearchgetnext
	#define EXT_x509certstoresearchgetnext
	#define GET_x509certstoresearchgetnext(fl)  CAL_CMGETAPI( "x509certstoresearchgetnext" ) 
	#define CAL_x509certstoresearchgetnext  x509certstoresearchgetnext
	#define CHK_x509certstoresearchgetnext  TRUE
	#define EXP_x509certstoresearchgetnext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetnext", (RTS_UINTPTR)x509certstoresearchgetnext, 1, 0, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoresearchgetnext  PFX509CERTSTORESEARCHGETNEXT_IEC pfx509certstoresearchgetnext;
	#define EXT_x509certstoresearchgetnext  extern PFX509CERTSTORESEARCHGETNEXT_IEC pfx509certstoresearchgetnext;
	#define GET_x509certstoresearchgetnext(fl)  s_pfCMGetAPI2( "x509certstoresearchgetnext", (RTS_VOID_FCTPTR *)&pfx509certstoresearchgetnext, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0, 0x03050B00)
	#define CAL_x509certstoresearchgetnext  pfx509certstoresearchgetnext
	#define CHK_x509certstoresearchgetnext  (pfx509certstoresearchgetnext != NULL)
	#define EXP_x509certstoresearchgetnext   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoresearchgetnext", (RTS_UINTPTR)x509certstoresearchgetnext, 1, 0, 0x03050B00) 
#endif


/**
 * <description>x509certstoreunregister</description>
 */
typedef struct tagx509certstoreunregister_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hRegisteredComponent;	/* VAR_INPUT */	
	RTS_IEC_RESULT X509CertStoreUnregister;	/* VAR_OUTPUT */	
} x509certstoreunregister_struct;

void CDECL CDECL_EXT x509certstoreunregister(x509certstoreunregister_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTSTOREUNREGISTER_IEC) (x509certstoreunregister_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREUNREGISTER_NOTIMPLEMENTED)
	#define USE_x509certstoreunregister
	#define EXT_x509certstoreunregister
	#define GET_x509certstoreunregister(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certstoreunregister(p0) 
	#define CHK_x509certstoreunregister  FALSE
	#define EXP_x509certstoreunregister  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certstoreunregister
	#define EXT_x509certstoreunregister
	#define GET_x509certstoreunregister(fl)  CAL_CMGETAPI( "x509certstoreunregister" ) 
	#define CAL_x509certstoreunregister  x509certstoreunregister
	#define CHK_x509certstoreunregister  TRUE
	#define EXP_x509certstoreunregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreunregister", (RTS_UINTPTR)x509certstoreunregister, 1, 0xE41C13F3, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certstoreunregister
	#define EXT_x509certstoreunregister
	#define GET_x509certstoreunregister(fl)  CAL_CMGETAPI( "x509certstoreunregister" ) 
	#define CAL_x509certstoreunregister  x509certstoreunregister
	#define CHK_x509certstoreunregister  TRUE
	#define EXP_x509certstoreunregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreunregister", (RTS_UINTPTR)x509certstoreunregister, 1, 0xE41C13F3, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certstoreunregister
	#define EXT_CmpX509Certx509certstoreunregister
	#define GET_CmpX509Certx509certstoreunregister  ERR_OK
	#define CAL_CmpX509Certx509certstoreunregister  x509certstoreunregister
	#define CHK_CmpX509Certx509certstoreunregister  TRUE
	#define EXP_CmpX509Certx509certstoreunregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreunregister", (RTS_UINTPTR)x509certstoreunregister, 1, 0xE41C13F3, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certstoreunregister
	#define EXT_x509certstoreunregister
	#define GET_x509certstoreunregister(fl)  CAL_CMGETAPI( "x509certstoreunregister" ) 
	#define CAL_x509certstoreunregister  x509certstoreunregister
	#define CHK_x509certstoreunregister  TRUE
	#define EXP_x509certstoreunregister  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreunregister", (RTS_UINTPTR)x509certstoreunregister, 1, 0xE41C13F3, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certstoreunregister  PFX509CERTSTOREUNREGISTER_IEC pfx509certstoreunregister;
	#define EXT_x509certstoreunregister  extern PFX509CERTSTOREUNREGISTER_IEC pfx509certstoreunregister;
	#define GET_x509certstoreunregister(fl)  s_pfCMGetAPI2( "x509certstoreunregister", (RTS_VOID_FCTPTR *)&pfx509certstoreunregister, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xE41C13F3, 0x03050B00)
	#define CAL_x509certstoreunregister  pfx509certstoreunregister
	#define CHK_x509certstoreunregister  (pfx509certstoreunregister != NULL)
	#define EXP_x509certstoreunregister   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certstoreunregister", (RTS_UINTPTR)x509certstoreunregister, 1, 0xE41C13F3, 0x03050B00) 
#endif


/**
 * <description>x509certverify</description>
 */
typedef struct tagx509certverify_struct
{
	RTS_IEC_HANDLE hCertStore;			/* VAR_INPUT */	
	RTS_IEC_HANDLE hCert;				/* VAR_INPUT */	
	RTS_IEC_RESULT X509CertVerify;		/* VAR_OUTPUT */	
} x509certverify_struct;

void CDECL CDECL_EXT x509certverify(x509certverify_struct *p);
typedef void (CDECL CDECL_EXT* PFX509CERTVERIFY_IEC) (x509certverify_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTVERIFY_NOTIMPLEMENTED)
	#define USE_x509certverify
	#define EXT_x509certverify
	#define GET_x509certverify(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509certverify(p0) 
	#define CHK_x509certverify  FALSE
	#define EXP_x509certverify  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509certverify
	#define EXT_x509certverify
	#define GET_x509certverify(fl)  CAL_CMGETAPI( "x509certverify" ) 
	#define CAL_x509certverify  x509certverify
	#define CHK_x509certverify  TRUE
	#define EXP_x509certverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certverify", (RTS_UINTPTR)x509certverify, 1, 0x9822EB29, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509certverify
	#define EXT_x509certverify
	#define GET_x509certverify(fl)  CAL_CMGETAPI( "x509certverify" ) 
	#define CAL_x509certverify  x509certverify
	#define CHK_x509certverify  TRUE
	#define EXP_x509certverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certverify", (RTS_UINTPTR)x509certverify, 1, 0x9822EB29, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509certverify
	#define EXT_CmpX509Certx509certverify
	#define GET_CmpX509Certx509certverify  ERR_OK
	#define CAL_CmpX509Certx509certverify  x509certverify
	#define CHK_CmpX509Certx509certverify  TRUE
	#define EXP_CmpX509Certx509certverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certverify", (RTS_UINTPTR)x509certverify, 1, 0x9822EB29, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509certverify
	#define EXT_x509certverify
	#define GET_x509certverify(fl)  CAL_CMGETAPI( "x509certverify" ) 
	#define CAL_x509certverify  x509certverify
	#define CHK_x509certverify  TRUE
	#define EXP_x509certverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certverify", (RTS_UINTPTR)x509certverify, 1, 0x9822EB29, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509certverify  PFX509CERTVERIFY_IEC pfx509certverify;
	#define EXT_x509certverify  extern PFX509CERTVERIFY_IEC pfx509certverify;
	#define GET_x509certverify(fl)  s_pfCMGetAPI2( "x509certverify", (RTS_VOID_FCTPTR *)&pfx509certverify, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x9822EB29, 0x03050B00)
	#define CAL_x509certverify  pfx509certverify
	#define CHK_x509certverify  (pfx509certverify != NULL)
	#define EXP_x509certverify   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509certverify", (RTS_UINTPTR)x509certverify, 1, 0x9822EB29, 0x03050B00) 
#endif


/**
 * Parse a certificate which is located in the memory (e.g received FROM a
 * communication partner). Once parsed the certificate is stored
 * in a temporary store. Use the function X509CertClose to close the certificate.
 * :return: Handle to the parsed certificate.
 */
typedef struct tagx509parsecertificate_struct
{
	RtsByteString *pCert;				/* VAR_INPUT */	/* Binary encoded certificate. */
	RTS_IEC_INT encoding;				/* VAR_INPUT, Enum: RTSCERTENCODING */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Result of the operation */
	RTS_IEC_HANDLE X509ParseCertificate;	/* VAR_OUTPUT */	
} x509parsecertificate_struct;

void CDECL CDECL_EXT x509parsecertificate(x509parsecertificate_struct *p);
typedef void (CDECL CDECL_EXT* PFX509PARSECERTIFICATE_IEC) (x509parsecertificate_struct *p);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509PARSECERTIFICATE_NOTIMPLEMENTED)
	#define USE_x509parsecertificate
	#define EXT_x509parsecertificate
	#define GET_x509parsecertificate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_x509parsecertificate(p0) 
	#define CHK_x509parsecertificate  FALSE
	#define EXP_x509parsecertificate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_x509parsecertificate
	#define EXT_x509parsecertificate
	#define GET_x509parsecertificate(fl)  CAL_CMGETAPI( "x509parsecertificate" ) 
	#define CAL_x509parsecertificate  x509parsecertificate
	#define CHK_x509parsecertificate  TRUE
	#define EXP_x509parsecertificate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509parsecertificate", (RTS_UINTPTR)x509parsecertificate, 1, 0x01B27207, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_x509parsecertificate
	#define EXT_x509parsecertificate
	#define GET_x509parsecertificate(fl)  CAL_CMGETAPI( "x509parsecertificate" ) 
	#define CAL_x509parsecertificate  x509parsecertificate
	#define CHK_x509parsecertificate  TRUE
	#define EXP_x509parsecertificate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509parsecertificate", (RTS_UINTPTR)x509parsecertificate, 1, 0x01B27207, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509Certx509parsecertificate
	#define EXT_CmpX509Certx509parsecertificate
	#define GET_CmpX509Certx509parsecertificate  ERR_OK
	#define CAL_CmpX509Certx509parsecertificate  x509parsecertificate
	#define CHK_CmpX509Certx509parsecertificate  TRUE
	#define EXP_CmpX509Certx509parsecertificate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509parsecertificate", (RTS_UINTPTR)x509parsecertificate, 1, 0x01B27207, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_x509parsecertificate
	#define EXT_x509parsecertificate
	#define GET_x509parsecertificate(fl)  CAL_CMGETAPI( "x509parsecertificate" ) 
	#define CAL_x509parsecertificate  x509parsecertificate
	#define CHK_x509parsecertificate  TRUE
	#define EXP_x509parsecertificate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509parsecertificate", (RTS_UINTPTR)x509parsecertificate, 1, 0x01B27207, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_x509parsecertificate  PFX509PARSECERTIFICATE_IEC pfx509parsecertificate;
	#define EXT_x509parsecertificate  extern PFX509PARSECERTIFICATE_IEC pfx509parsecertificate;
	#define GET_x509parsecertificate(fl)  s_pfCMGetAPI2( "x509parsecertificate", (RTS_VOID_FCTPTR *)&pfx509parsecertificate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x01B27207, 0x03050B00)
	#define CAL_x509parsecertificate  pfx509parsecertificate
	#define CHK_x509parsecertificate  (pfx509parsecertificate != NULL)
	#define EXP_x509parsecertificate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"x509parsecertificate", (RTS_UINTPTR)x509parsecertificate, 1, 0x01B27207, 0x03050B00) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/

/**
 * <element name="Result" type="OUT">Result returned by X509CertCreateSelfSigned</element>
 * <element name="hCert" type="OUT">Handle of the geneatred certificate.</element>
 */
typedef struct
{
	RTS_RESULT Result;
	RTS_HANDLE hCert;
} X509CertCreateSelfSignedAsyncResult;

/**
 * <element name="Result" type="OUT">Result returned by X509CertCreateCSR</element>
 * <element name="hCert" type="OUT">Pointer to the RtsByteString given as parameter in X509CertCreateCSR containing the CSR.</element>
 */
typedef struct
{
	RTS_RESULT Result;
	RtsByteString* pCSR;
} X509CertCreateCSRAsyncResult;


typedef union
{
	X509CertCreateSelfSignedAsyncResult selfSigned;
	X509CertCreateCSRAsyncResult csr;
} X509AsyncOperationResult;

/**
 * <element name="ui32Operation" type="OUT">Identifier which operation was executed.</element>
 * <element name="Result" type="OUT">Result of the operation. Use the field according the ui32Operation.</element>
 * <element name="pCallbackParam" type="OUT">Pointer to callback parameter given in the X509CertCreate...Async function.</element>
 */
typedef struct
{
	RTS_UI32 ui32Operation;
	X509AsyncOperationResult Result;
	void* pCallbackParam;
} X509AsyncResult;

#define CMPX509CERT_ASYNC_FUNCTION_CREATESELFSIGNED		1
#define CMPX509CERT_ASYNC_FUNCTION_CREATECSR			2


typedef void HUGEPTR(CDECL *PFX509ASYNCCALLBACK)(X509AsyncResult *pAsyncStruct);



/* --------------------------------------------------------------------------- */
/*                             Static defines                                  */
/* --------------------------------------------------------------------------- */
/**
 * <category>Key Usage</category>
 * <description>These defines are the bits representing the key usages in the 
 * keyUsage field of the RtsX509CertInfo structure</description>
 */
#define RTS_KEY_USAGE_DIGITAL_SIGNATURE 0x1
#define RTS_KEY_USAGE_DIGITAL_SIGNATURE_STR "Digital Signature"
#define RTS_KEY_USAGE_NON_REPUDIATION 0x2
#define RTS_KEY_USAGE_NON_REPUDIATION_STR "Non Repudiation"
#define RTS_KEY_USAGE_ENCIPHERMENT 0x4
#define RTS_KEY_USAGE_ENCIPHERMENT_STR "Key Encipherment"
#define RTS_KEY_USAGE_DATA_ENCIPHERMENT 0x8
#define RTS_KEY_USAGE_DATA_ENCIPHERMENT_STR "Data Encipherment"
#define RTS_KEY_USAGE_KEY_AGREEMENT 0x10
#define RTS_KEY_USAGE_KEY_AGREEMENT_STR "Key Agreement"
#define RTS_KEY_USAGE_CERTIFICATE_SIGN 0x20
#define RTS_KEY_USAGE_CERTIFICATE_SIGN_STR "Certificate Sign"
#define RTS_KEY_USAGE_CRL_SIGN 0x40
#define RTS_KEY_USAGE_CRL_SIGN_STR "CRL Sign"
#define RTS_KEY_USAGE_ENCIPHER_ONLY 0x80
#define RTS_KEY_USAGE_ENCIPHER_ONLY_STR "Encipher Only"
#define RTS_KEY_USAGE_DECIPHER_ONLY 0x100
#define RTS_KEY_USAGE_DECIPHER_ONLY_STR "Decipher Only"

/**
 * <category>Object Identifiers</category>
 * <description>A list of well known OIDs in their string representations. Use this defines
 * with the function RtsOidCreate to generate a new OID object.</description>
 */
#define RTS_OID_COUNTRY_NAME                "2.5.4.6"
#define RTS_OID_ORGANIZATION_NAME           "2.5.4.10"
#define RTS_OID_ORGANIZATIONAL_UNIT_NAME	"2.5.4.11"
#define RTS_OID_COMMON_NAME				    "2.5.4.3"
#define RTS_OID_LOCALITY_NAME				"2.5.4.7"
#define RTS_OID_STATE_OR_PROVINCE_NAME		"2.5.4.8"
#define RTS_OID_TITLE						"2.5.4.12"
#define RTS_OID_GIVEN_NAME					"2.5.4.42"
#define RTS_OID_INITIALS					"2.5.4.43"
#define RTS_OID_SUR_NAME					"2.5.4.4"
#define RTS_OID_DOMAIN_COMPONENT			"0.9.2342.19200300.100.1.25"
#define RTS_OID_RSA_EMAILADDR				"1.2.840.113549.1.9.1"
#define RTS_OID_STREET_ADDRESS				"2.5.4.9"
#define RTS_OID_RSA_UNSTRUCTNAME			"1.2.840.113549.1.9.2"
#define RTS_OID_RSA_UNSTRUCTADDR			"1.2.840.113549.1.9.8"
#define RTS_OID_DEVICE_SERIAL_NUMBER		"2.5.4.5"
#define RTS_OID_TLS_WEBSERVER_AUTHENTICATION "1.3.6.1.5.5.7.3.1"
#define RTS_OID_TLS_WEBCLIENT_AUTHENTICATION "1.3.6.1.5.5.7.3.2"

/* --------------------------------------------------------------------------- */
/*                                  Types                                      */
/* --------------------------------------------------------------------------- */

typedef struct RtsCertBackendInfo_
{
	int dummy;
} RtsCertBackendInfo;


/* --------------------------------------------------------------------------- */
/*                                  Events                                     */
/* --------------------------------------------------------------------------- */

/**
 * <category>Events</category>
 * <description>This event is fired if the certificate store has been changed.
 * components can then update their certificates if needed.</description>
 */
#define EVT_X509CertStoreChanged		MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Event parameter</category>
 * <element name="trustLevel" type="OUT">Trust level that has changed.</element>
 */
typedef struct
{
	RTSCERTTRUSTLEVEL trustLevel;
} EVTPARAM_X509CertStoreChanged;
#define EVTPARAMID_X509CertStoreChanged						0x0001
#define EVTVERSION_X509CertStoreChanged						0x0001


#ifdef __cplusplus
extern "C" {
#endif

/* --------------------------------------------------------------------------- */
/*                   Basic Certificate store functions                         */
/* --------------------------------------------------------------------------- */

/**
 * <description>Open the certificate store.</description>
 * <param name="componentID" type="IN">ID of the component opening the certificate store.</param>
 * <param name="pResult" type="OUT">Result of the operation.</param>
 * <result>Handle to the certificate store</result>
 */
RTS_HANDLE CDECL X509CertStoreOpen(CMPID componentID, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTSTOREOPEN) (CMPID componentID, RTS_RESULT* pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREOPEN_NOTIMPLEMENTED)
	#define USE_X509CertStoreOpen
	#define EXT_X509CertStoreOpen
	#define GET_X509CertStoreOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreOpen(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertStoreOpen  FALSE
	#define EXP_X509CertStoreOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreOpen
	#define EXT_X509CertStoreOpen
	#define GET_X509CertStoreOpen(fl)  CAL_CMGETAPI( "X509CertStoreOpen" ) 
	#define CAL_X509CertStoreOpen  X509CertStoreOpen
	#define CHK_X509CertStoreOpen  TRUE
	#define EXP_X509CertStoreOpen  CAL_CMEXPAPI( "X509CertStoreOpen" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreOpen
	#define EXT_X509CertStoreOpen
	#define GET_X509CertStoreOpen(fl)  CAL_CMGETAPI( "X509CertStoreOpen" ) 
	#define CAL_X509CertStoreOpen  X509CertStoreOpen
	#define CHK_X509CertStoreOpen  TRUE
	#define EXP_X509CertStoreOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreOpen", (RTS_UINTPTR)X509CertStoreOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreOpen
	#define EXT_CmpX509CertX509CertStoreOpen
	#define GET_CmpX509CertX509CertStoreOpen  ERR_OK
	#define CAL_CmpX509CertX509CertStoreOpen pICmpX509Cert->IX509CertStoreOpen
	#define CHK_CmpX509CertX509CertStoreOpen (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreOpen
	#define EXT_X509CertStoreOpen
	#define GET_X509CertStoreOpen(fl)  CAL_CMGETAPI( "X509CertStoreOpen" ) 
	#define CAL_X509CertStoreOpen pICmpX509Cert->IX509CertStoreOpen
	#define CHK_X509CertStoreOpen (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreOpen  CAL_CMEXPAPI( "X509CertStoreOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreOpen  PFX509CERTSTOREOPEN pfX509CertStoreOpen;
	#define EXT_X509CertStoreOpen  extern PFX509CERTSTOREOPEN pfX509CertStoreOpen;
	#define GET_X509CertStoreOpen(fl)  s_pfCMGetAPI2( "X509CertStoreOpen", (RTS_VOID_FCTPTR *)&pfX509CertStoreOpen, (fl), 0, 0)
	#define CAL_X509CertStoreOpen  pfX509CertStoreOpen
	#define CHK_X509CertStoreOpen  (pfX509CertStoreOpen != NULL)
	#define EXP_X509CertStoreOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreOpen", (RTS_UINTPTR)X509CertStoreOpen, 0, 0) 
#endif




/**
 * <description>Close the certificate store.</description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertStoreClose(RTS_HANDLE hCertStore);
typedef RTS_RESULT (CDECL * PFX509CERTSTORECLOSE) (RTS_HANDLE hCertStore);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTORECLOSE_NOTIMPLEMENTED)
	#define USE_X509CertStoreClose
	#define EXT_X509CertStoreClose
	#define GET_X509CertStoreClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertStoreClose  FALSE
	#define EXP_X509CertStoreClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreClose
	#define EXT_X509CertStoreClose
	#define GET_X509CertStoreClose(fl)  CAL_CMGETAPI( "X509CertStoreClose" ) 
	#define CAL_X509CertStoreClose  X509CertStoreClose
	#define CHK_X509CertStoreClose  TRUE
	#define EXP_X509CertStoreClose  CAL_CMEXPAPI( "X509CertStoreClose" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreClose
	#define EXT_X509CertStoreClose
	#define GET_X509CertStoreClose(fl)  CAL_CMGETAPI( "X509CertStoreClose" ) 
	#define CAL_X509CertStoreClose  X509CertStoreClose
	#define CHK_X509CertStoreClose  TRUE
	#define EXP_X509CertStoreClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreClose", (RTS_UINTPTR)X509CertStoreClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreClose
	#define EXT_CmpX509CertX509CertStoreClose
	#define GET_CmpX509CertX509CertStoreClose  ERR_OK
	#define CAL_CmpX509CertX509CertStoreClose pICmpX509Cert->IX509CertStoreClose
	#define CHK_CmpX509CertX509CertStoreClose (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreClose
	#define EXT_X509CertStoreClose
	#define GET_X509CertStoreClose(fl)  CAL_CMGETAPI( "X509CertStoreClose" ) 
	#define CAL_X509CertStoreClose pICmpX509Cert->IX509CertStoreClose
	#define CHK_X509CertStoreClose (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreClose  CAL_CMEXPAPI( "X509CertStoreClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreClose  PFX509CERTSTORECLOSE pfX509CertStoreClose;
	#define EXT_X509CertStoreClose  extern PFX509CERTSTORECLOSE pfX509CertStoreClose;
	#define GET_X509CertStoreClose(fl)  s_pfCMGetAPI2( "X509CertStoreClose", (RTS_VOID_FCTPTR *)&pfX509CertStoreClose, (fl), 0, 0)
	#define CAL_X509CertStoreClose  pfX509CertStoreClose
	#define CHK_X509CertStoreClose  (pfX509CertStoreClose != NULL)
	#define EXP_X509CertStoreClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreClose", (RTS_UINTPTR)X509CertStoreClose, 0, 0) 
#endif




/**
 * <description>Verify the given certificate. This verify checks only if the certificate
 * itself is trusted using the certificates in the store. The verify checks
 * if the certificate was added to the untrusted certs list. No subjects, key usages
 * or other fields are verified. This has to be done by the application using this function.
 * If the verify should use CRL to verify the certificates set the setting VerifyUseCRL to 1.
 * This is disabled by default.</description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <param name="hCert" type="IN">Certificate to verify.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertVerify(RTS_HANDLE hCertStore, RTS_HANDLE hCert);
typedef RTS_RESULT (CDECL * PFX509CERTVERIFY) (RTS_HANDLE hCertStore, RTS_HANDLE hCert);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTVERIFY_NOTIMPLEMENTED)
	#define USE_X509CertVerify
	#define EXT_X509CertVerify
	#define GET_X509CertVerify(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertVerify(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertVerify  FALSE
	#define EXP_X509CertVerify  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertVerify
	#define EXT_X509CertVerify
	#define GET_X509CertVerify(fl)  CAL_CMGETAPI( "X509CertVerify" ) 
	#define CAL_X509CertVerify  X509CertVerify
	#define CHK_X509CertVerify  TRUE
	#define EXP_X509CertVerify  CAL_CMEXPAPI( "X509CertVerify" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertVerify
	#define EXT_X509CertVerify
	#define GET_X509CertVerify(fl)  CAL_CMGETAPI( "X509CertVerify" ) 
	#define CAL_X509CertVerify  X509CertVerify
	#define CHK_X509CertVerify  TRUE
	#define EXP_X509CertVerify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertVerify", (RTS_UINTPTR)X509CertVerify, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertVerify
	#define EXT_CmpX509CertX509CertVerify
	#define GET_CmpX509CertX509CertVerify  ERR_OK
	#define CAL_CmpX509CertX509CertVerify pICmpX509Cert->IX509CertVerify
	#define CHK_CmpX509CertX509CertVerify (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertVerify  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertVerify
	#define EXT_X509CertVerify
	#define GET_X509CertVerify(fl)  CAL_CMGETAPI( "X509CertVerify" ) 
	#define CAL_X509CertVerify pICmpX509Cert->IX509CertVerify
	#define CHK_X509CertVerify (pICmpX509Cert != NULL)
	#define EXP_X509CertVerify  CAL_CMEXPAPI( "X509CertVerify" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertVerify  PFX509CERTVERIFY pfX509CertVerify;
	#define EXT_X509CertVerify  extern PFX509CERTVERIFY pfX509CertVerify;
	#define GET_X509CertVerify(fl)  s_pfCMGetAPI2( "X509CertVerify", (RTS_VOID_FCTPTR *)&pfX509CertVerify, (fl), 0, 0)
	#define CAL_X509CertVerify  pfX509CertVerify
	#define CHK_X509CertVerify  (pfX509CertVerify != NULL)
	#define EXP_X509CertVerify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertVerify", (RTS_UINTPTR)X509CertVerify, 0, 0) 
#endif




/**
 * <description>Search for the given certificate in the cert store.</description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <param name="pFilters" type="IN">Pointer to array of filters to be applied.</param>
 * <param name="ui32NumOfFilters" type="IN">Number of applied filters.</param>
 * <result>Handle to the first certificate matching the filter criteria.</result>
 */
RTS_HANDLE CDECL X509CertStoreSearchGetFirst(RTS_HANDLE hCertStore, RtsX509CertFilter *pFilters, RTS_UI32 ui32NumOfFilters, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTSTORESEARCHGETFIRST) (RTS_HANDLE hCertStore, RtsX509CertFilter *pFilters, RTS_UI32 ui32NumOfFilters, RTS_RESULT *pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTORESEARCHGETFIRST_NOTIMPLEMENTED)
	#define USE_X509CertStoreSearchGetFirst
	#define EXT_X509CertStoreSearchGetFirst
	#define GET_X509CertStoreSearchGetFirst(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreSearchGetFirst(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertStoreSearchGetFirst  FALSE
	#define EXP_X509CertStoreSearchGetFirst  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreSearchGetFirst
	#define EXT_X509CertStoreSearchGetFirst
	#define GET_X509CertStoreSearchGetFirst(fl)  CAL_CMGETAPI( "X509CertStoreSearchGetFirst" ) 
	#define CAL_X509CertStoreSearchGetFirst  X509CertStoreSearchGetFirst
	#define CHK_X509CertStoreSearchGetFirst  TRUE
	#define EXP_X509CertStoreSearchGetFirst  CAL_CMEXPAPI( "X509CertStoreSearchGetFirst" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreSearchGetFirst
	#define EXT_X509CertStoreSearchGetFirst
	#define GET_X509CertStoreSearchGetFirst(fl)  CAL_CMGETAPI( "X509CertStoreSearchGetFirst" ) 
	#define CAL_X509CertStoreSearchGetFirst  X509CertStoreSearchGetFirst
	#define CHK_X509CertStoreSearchGetFirst  TRUE
	#define EXP_X509CertStoreSearchGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreSearchGetFirst", (RTS_UINTPTR)X509CertStoreSearchGetFirst, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreSearchGetFirst
	#define EXT_CmpX509CertX509CertStoreSearchGetFirst
	#define GET_CmpX509CertX509CertStoreSearchGetFirst  ERR_OK
	#define CAL_CmpX509CertX509CertStoreSearchGetFirst pICmpX509Cert->IX509CertStoreSearchGetFirst
	#define CHK_CmpX509CertX509CertStoreSearchGetFirst (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreSearchGetFirst  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreSearchGetFirst
	#define EXT_X509CertStoreSearchGetFirst
	#define GET_X509CertStoreSearchGetFirst(fl)  CAL_CMGETAPI( "X509CertStoreSearchGetFirst" ) 
	#define CAL_X509CertStoreSearchGetFirst pICmpX509Cert->IX509CertStoreSearchGetFirst
	#define CHK_X509CertStoreSearchGetFirst (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreSearchGetFirst  CAL_CMEXPAPI( "X509CertStoreSearchGetFirst" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreSearchGetFirst  PFX509CERTSTORESEARCHGETFIRST pfX509CertStoreSearchGetFirst;
	#define EXT_X509CertStoreSearchGetFirst  extern PFX509CERTSTORESEARCHGETFIRST pfX509CertStoreSearchGetFirst;
	#define GET_X509CertStoreSearchGetFirst(fl)  s_pfCMGetAPI2( "X509CertStoreSearchGetFirst", (RTS_VOID_FCTPTR *)&pfX509CertStoreSearchGetFirst, (fl), 0, 0)
	#define CAL_X509CertStoreSearchGetFirst  pfX509CertStoreSearchGetFirst
	#define CHK_X509CertStoreSearchGetFirst  (pfX509CertStoreSearchGetFirst != NULL)
	#define EXP_X509CertStoreSearchGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreSearchGetFirst", (RTS_UINTPTR)X509CertStoreSearchGetFirst, 0, 0) 
#endif




/**
 * <description>Search for the given certificate in the cert store.</description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <param name="hCert" type="IN">Handle of the last found certificate.</param>
 * <param name="pFilters" type="IN">Pointer to array of filters to be applied.</param>
 * <param name="ui32NumOfFilters" type="IN">Number of applied filters.</param>
 * <result>Handle to the first certificate matching the filter criteria.</result>
 */
RTS_HANDLE CDECL X509CertStoreSearchGetNext(RTS_HANDLE hCertStore, RTS_HANDLE hCert, RtsX509CertFilter *pFilters, RTS_UI32 ui32NumOfFilters, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTSTORESEARCHGETNEXT) (RTS_HANDLE hCertStore, RTS_HANDLE hCert, RtsX509CertFilter *pFilters, RTS_UI32 ui32NumOfFilters, RTS_RESULT *pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTORESEARCHGETNEXT_NOTIMPLEMENTED)
	#define USE_X509CertStoreSearchGetNext
	#define EXT_X509CertStoreSearchGetNext
	#define GET_X509CertStoreSearchGetNext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreSearchGetNext(p0,p1,p2,p3,p4)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertStoreSearchGetNext  FALSE
	#define EXP_X509CertStoreSearchGetNext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreSearchGetNext
	#define EXT_X509CertStoreSearchGetNext
	#define GET_X509CertStoreSearchGetNext(fl)  CAL_CMGETAPI( "X509CertStoreSearchGetNext" ) 
	#define CAL_X509CertStoreSearchGetNext  X509CertStoreSearchGetNext
	#define CHK_X509CertStoreSearchGetNext  TRUE
	#define EXP_X509CertStoreSearchGetNext  CAL_CMEXPAPI( "X509CertStoreSearchGetNext" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreSearchGetNext
	#define EXT_X509CertStoreSearchGetNext
	#define GET_X509CertStoreSearchGetNext(fl)  CAL_CMGETAPI( "X509CertStoreSearchGetNext" ) 
	#define CAL_X509CertStoreSearchGetNext  X509CertStoreSearchGetNext
	#define CHK_X509CertStoreSearchGetNext  TRUE
	#define EXP_X509CertStoreSearchGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreSearchGetNext", (RTS_UINTPTR)X509CertStoreSearchGetNext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreSearchGetNext
	#define EXT_CmpX509CertX509CertStoreSearchGetNext
	#define GET_CmpX509CertX509CertStoreSearchGetNext  ERR_OK
	#define CAL_CmpX509CertX509CertStoreSearchGetNext pICmpX509Cert->IX509CertStoreSearchGetNext
	#define CHK_CmpX509CertX509CertStoreSearchGetNext (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreSearchGetNext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreSearchGetNext
	#define EXT_X509CertStoreSearchGetNext
	#define GET_X509CertStoreSearchGetNext(fl)  CAL_CMGETAPI( "X509CertStoreSearchGetNext" ) 
	#define CAL_X509CertStoreSearchGetNext pICmpX509Cert->IX509CertStoreSearchGetNext
	#define CHK_X509CertStoreSearchGetNext (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreSearchGetNext  CAL_CMEXPAPI( "X509CertStoreSearchGetNext" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreSearchGetNext  PFX509CERTSTORESEARCHGETNEXT pfX509CertStoreSearchGetNext;
	#define EXT_X509CertStoreSearchGetNext  extern PFX509CERTSTORESEARCHGETNEXT pfX509CertStoreSearchGetNext;
	#define GET_X509CertStoreSearchGetNext(fl)  s_pfCMGetAPI2( "X509CertStoreSearchGetNext", (RTS_VOID_FCTPTR *)&pfX509CertStoreSearchGetNext, (fl), 0, 0)
	#define CAL_X509CertStoreSearchGetNext  pfX509CertStoreSearchGetNext
	#define CHK_X509CertStoreSearchGetNext  (pfX509CertStoreSearchGetNext != NULL)
	#define EXP_X509CertStoreSearchGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreSearchGetNext", (RTS_UINTPTR)X509CertStoreSearchGetNext, 0, 0) 
#endif





/* --------------------------------------------------------------------------- */
/*                      Handling single certificates                           */
/* --------------------------------------------------------------------------- */

/**
 * <description>Parse a certificate which is located in the memory (e.g received from a
 * communication partner). Once parsed the certificate is stored
 * in a temporary store. Use the function X509CertClose to close the certificate.
 * </description>
 * <param name="pCert" type="IN">Binary encoded certificate.</param>
 * <param name="encoding" type="IN">Encoding of the certificate.</param>
 * <param name="pResult" type="OUT">Result of the operation</param>
 * <result>Handle to the parsed certificate.</result>
 */
RTS_HANDLE CDECL X509ParseCertificate(RtsByteString* pCert, RTSCERTENCODING encoding, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFX509PARSECERTIFICATE) (RtsByteString* pCert, RTSCERTENCODING encoding, RTS_RESULT* pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509PARSECERTIFICATE_NOTIMPLEMENTED)
	#define USE_X509ParseCertificate
	#define EXT_X509ParseCertificate
	#define GET_X509ParseCertificate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509ParseCertificate(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509ParseCertificate  FALSE
	#define EXP_X509ParseCertificate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509ParseCertificate
	#define EXT_X509ParseCertificate
	#define GET_X509ParseCertificate(fl)  CAL_CMGETAPI( "X509ParseCertificate" ) 
	#define CAL_X509ParseCertificate  X509ParseCertificate
	#define CHK_X509ParseCertificate  TRUE
	#define EXP_X509ParseCertificate  CAL_CMEXPAPI( "X509ParseCertificate" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509ParseCertificate
	#define EXT_X509ParseCertificate
	#define GET_X509ParseCertificate(fl)  CAL_CMGETAPI( "X509ParseCertificate" ) 
	#define CAL_X509ParseCertificate  X509ParseCertificate
	#define CHK_X509ParseCertificate  TRUE
	#define EXP_X509ParseCertificate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509ParseCertificate", (RTS_UINTPTR)X509ParseCertificate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509ParseCertificate
	#define EXT_CmpX509CertX509ParseCertificate
	#define GET_CmpX509CertX509ParseCertificate  ERR_OK
	#define CAL_CmpX509CertX509ParseCertificate pICmpX509Cert->IX509ParseCertificate
	#define CHK_CmpX509CertX509ParseCertificate (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509ParseCertificate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509ParseCertificate
	#define EXT_X509ParseCertificate
	#define GET_X509ParseCertificate(fl)  CAL_CMGETAPI( "X509ParseCertificate" ) 
	#define CAL_X509ParseCertificate pICmpX509Cert->IX509ParseCertificate
	#define CHK_X509ParseCertificate (pICmpX509Cert != NULL)
	#define EXP_X509ParseCertificate  CAL_CMEXPAPI( "X509ParseCertificate" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509ParseCertificate  PFX509PARSECERTIFICATE pfX509ParseCertificate;
	#define EXT_X509ParseCertificate  extern PFX509PARSECERTIFICATE pfX509ParseCertificate;
	#define GET_X509ParseCertificate(fl)  s_pfCMGetAPI2( "X509ParseCertificate", (RTS_VOID_FCTPTR *)&pfX509ParseCertificate, (fl), 0, 0)
	#define CAL_X509ParseCertificate  pfX509ParseCertificate
	#define CHK_X509ParseCertificate  (pfX509ParseCertificate != NULL)
	#define EXP_X509ParseCertificate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509ParseCertificate", (RTS_UINTPTR)X509ParseCertificate, 0, 0) 
#endif




/**
 * <description>Get some common content from the certificate.</description>
 * <param name="hCert" type="IN">Handle of the certificate of intrest.</param>
 * <param name="pCertContent" type="OUT">Pointer where to store the information.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertGetContent(RTS_HANDLE hCert, RtsX509CertInfo* pCertContent);
typedef RTS_RESULT (CDECL * PFX509CERTGETCONTENT) (RTS_HANDLE hCert, RtsX509CertInfo* pCertContent);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETCONTENT_NOTIMPLEMENTED)
	#define USE_X509CertGetContent
	#define EXT_X509CertGetContent
	#define GET_X509CertGetContent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertGetContent(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertGetContent  FALSE
	#define EXP_X509CertGetContent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertGetContent
	#define EXT_X509CertGetContent
	#define GET_X509CertGetContent(fl)  CAL_CMGETAPI( "X509CertGetContent" ) 
	#define CAL_X509CertGetContent  X509CertGetContent
	#define CHK_X509CertGetContent  TRUE
	#define EXP_X509CertGetContent  CAL_CMEXPAPI( "X509CertGetContent" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertGetContent
	#define EXT_X509CertGetContent
	#define GET_X509CertGetContent(fl)  CAL_CMGETAPI( "X509CertGetContent" ) 
	#define CAL_X509CertGetContent  X509CertGetContent
	#define CHK_X509CertGetContent  TRUE
	#define EXP_X509CertGetContent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetContent", (RTS_UINTPTR)X509CertGetContent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertGetContent
	#define EXT_CmpX509CertX509CertGetContent
	#define GET_CmpX509CertX509CertGetContent  ERR_OK
	#define CAL_CmpX509CertX509CertGetContent pICmpX509Cert->IX509CertGetContent
	#define CHK_CmpX509CertX509CertGetContent (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertGetContent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertGetContent
	#define EXT_X509CertGetContent
	#define GET_X509CertGetContent(fl)  CAL_CMGETAPI( "X509CertGetContent" ) 
	#define CAL_X509CertGetContent pICmpX509Cert->IX509CertGetContent
	#define CHK_X509CertGetContent (pICmpX509Cert != NULL)
	#define EXP_X509CertGetContent  CAL_CMEXPAPI( "X509CertGetContent" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertGetContent  PFX509CERTGETCONTENT pfX509CertGetContent;
	#define EXT_X509CertGetContent  extern PFX509CERTGETCONTENT pfX509CertGetContent;
	#define GET_X509CertGetContent(fl)  s_pfCMGetAPI2( "X509CertGetContent", (RTS_VOID_FCTPTR *)&pfX509CertGetContent, (fl), 0, 0)
	#define CAL_X509CertGetContent  pfX509CertGetContent
	#define CHK_X509CertGetContent  (pfX509CertGetContent != NULL)
	#define EXP_X509CertGetContent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetContent", (RTS_UINTPTR)X509CertGetContent, 0, 0) 
#endif




/**
 * <description>Get the certificate in a binary encoded format. Use the encoding parameter
 * to set the needed format.
 * </description>
 * <param name="hCert" type="IN">Handle of the certificate of intrest.</param>
 * <param name="encoding" type="IN">How to encode the certificate.</param>
 * <param name="pCert" type="OUT">Buffer where to store the certificate. If the bytestring contains a NULL memory pointer the function will write the needed bufferlength to the pCert->ui32Length member.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertGetBinary(RTS_HANDLE hCert, RTSCERTENCODING encoding, RtsByteString* pCert);
typedef RTS_RESULT (CDECL * PFX509CERTGETBINARY) (RTS_HANDLE hCert, RTSCERTENCODING encoding, RtsByteString* pCert);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETBINARY_NOTIMPLEMENTED)
	#define USE_X509CertGetBinary
	#define EXT_X509CertGetBinary
	#define GET_X509CertGetBinary(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertGetBinary(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertGetBinary  FALSE
	#define EXP_X509CertGetBinary  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertGetBinary
	#define EXT_X509CertGetBinary
	#define GET_X509CertGetBinary(fl)  CAL_CMGETAPI( "X509CertGetBinary" ) 
	#define CAL_X509CertGetBinary  X509CertGetBinary
	#define CHK_X509CertGetBinary  TRUE
	#define EXP_X509CertGetBinary  CAL_CMEXPAPI( "X509CertGetBinary" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertGetBinary
	#define EXT_X509CertGetBinary
	#define GET_X509CertGetBinary(fl)  CAL_CMGETAPI( "X509CertGetBinary" ) 
	#define CAL_X509CertGetBinary  X509CertGetBinary
	#define CHK_X509CertGetBinary  TRUE
	#define EXP_X509CertGetBinary  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetBinary", (RTS_UINTPTR)X509CertGetBinary, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertGetBinary
	#define EXT_CmpX509CertX509CertGetBinary
	#define GET_CmpX509CertX509CertGetBinary  ERR_OK
	#define CAL_CmpX509CertX509CertGetBinary pICmpX509Cert->IX509CertGetBinary
	#define CHK_CmpX509CertX509CertGetBinary (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertGetBinary  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertGetBinary
	#define EXT_X509CertGetBinary
	#define GET_X509CertGetBinary(fl)  CAL_CMGETAPI( "X509CertGetBinary" ) 
	#define CAL_X509CertGetBinary pICmpX509Cert->IX509CertGetBinary
	#define CHK_X509CertGetBinary (pICmpX509Cert != NULL)
	#define EXP_X509CertGetBinary  CAL_CMEXPAPI( "X509CertGetBinary" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertGetBinary  PFX509CERTGETBINARY pfX509CertGetBinary;
	#define EXT_X509CertGetBinary  extern PFX509CERTGETBINARY pfX509CertGetBinary;
	#define GET_X509CertGetBinary(fl)  s_pfCMGetAPI2( "X509CertGetBinary", (RTS_VOID_FCTPTR *)&pfX509CertGetBinary, (fl), 0, 0)
	#define CAL_X509CertGetBinary  pfX509CertGetBinary
	#define CHK_X509CertGetBinary  (pfX509CertGetBinary != NULL)
	#define EXP_X509CertGetBinary  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetBinary", (RTS_UINTPTR)X509CertGetBinary, 0, 0) 
#endif




/**
 * <description>Get the public key from the certificate.
 *
 * The format of this key is an implementation detail and has to match
 * the expected format of the asymmetric functions of the CmpCrpyotItf.
 * The key data is opaque for the user of the keys. If the keys are needed in a
 * defined exchange format use the CryptoKeyExport function of the CmpCryptoItf.
 * </description>
 * <param name="hCert" type="IN">Handle of the certificate of intrest.</param>
 * <param name="pPublicKey" type="OUT">Pointer to the crypto key storing the public key.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertGetPublicKey(RTS_HANDLE hCert, RtsCryptoKey* pPublicKey);
typedef RTS_RESULT (CDECL * PFX509CERTGETPUBLICKEY) (RTS_HANDLE hCert, RtsCryptoKey* pPublicKey);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETPUBLICKEY_NOTIMPLEMENTED)
	#define USE_X509CertGetPublicKey
	#define EXT_X509CertGetPublicKey
	#define GET_X509CertGetPublicKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertGetPublicKey(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertGetPublicKey  FALSE
	#define EXP_X509CertGetPublicKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertGetPublicKey
	#define EXT_X509CertGetPublicKey
	#define GET_X509CertGetPublicKey(fl)  CAL_CMGETAPI( "X509CertGetPublicKey" ) 
	#define CAL_X509CertGetPublicKey  X509CertGetPublicKey
	#define CHK_X509CertGetPublicKey  TRUE
	#define EXP_X509CertGetPublicKey  CAL_CMEXPAPI( "X509CertGetPublicKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertGetPublicKey
	#define EXT_X509CertGetPublicKey
	#define GET_X509CertGetPublicKey(fl)  CAL_CMGETAPI( "X509CertGetPublicKey" ) 
	#define CAL_X509CertGetPublicKey  X509CertGetPublicKey
	#define CHK_X509CertGetPublicKey  TRUE
	#define EXP_X509CertGetPublicKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetPublicKey", (RTS_UINTPTR)X509CertGetPublicKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertGetPublicKey
	#define EXT_CmpX509CertX509CertGetPublicKey
	#define GET_CmpX509CertX509CertGetPublicKey  ERR_OK
	#define CAL_CmpX509CertX509CertGetPublicKey pICmpX509Cert->IX509CertGetPublicKey
	#define CHK_CmpX509CertX509CertGetPublicKey (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertGetPublicKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertGetPublicKey
	#define EXT_X509CertGetPublicKey
	#define GET_X509CertGetPublicKey(fl)  CAL_CMGETAPI( "X509CertGetPublicKey" ) 
	#define CAL_X509CertGetPublicKey pICmpX509Cert->IX509CertGetPublicKey
	#define CHK_X509CertGetPublicKey (pICmpX509Cert != NULL)
	#define EXP_X509CertGetPublicKey  CAL_CMEXPAPI( "X509CertGetPublicKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertGetPublicKey  PFX509CERTGETPUBLICKEY pfX509CertGetPublicKey;
	#define EXT_X509CertGetPublicKey  extern PFX509CERTGETPUBLICKEY pfX509CertGetPublicKey;
	#define GET_X509CertGetPublicKey(fl)  s_pfCMGetAPI2( "X509CertGetPublicKey", (RTS_VOID_FCTPTR *)&pfX509CertGetPublicKey, (fl), 0, 0)
	#define CAL_X509CertGetPublicKey  pfX509CertGetPublicKey
	#define CHK_X509CertGetPublicKey  (pfX509CertGetPublicKey != NULL)
	#define EXP_X509CertGetPublicKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetPublicKey", (RTS_UINTPTR)X509CertGetPublicKey, 0, 0) 
#endif




/**
 * <description>Get the private key of the certificate. This is only
 * possible if the private key stored in the certificate store. 
 *
 * The format of this key is an implementation detail and has to match
 * the expected format of the asymmetric functions of the CmpCrpyotItf.
 * The key data is opaque for the user of the keys.
 * </description>
 * <param name="hCert" type="IN">Handle of the certificate of intrest.</param>
 * <param name="pPublicKey" type="OUT">Pointer to the crypto key storing the priave key.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertGetPrivateKey(RTS_HANDLE hCertStore, RTS_HANDLE hCert, RtsCryptoKey* pPrivateKey);
typedef RTS_RESULT (CDECL * PFX509CERTGETPRIVATEKEY) (RTS_HANDLE hCertStore, RTS_HANDLE hCert, RtsCryptoKey* pPrivateKey);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETPRIVATEKEY_NOTIMPLEMENTED)
	#define USE_X509CertGetPrivateKey
	#define EXT_X509CertGetPrivateKey
	#define GET_X509CertGetPrivateKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertGetPrivateKey(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertGetPrivateKey  FALSE
	#define EXP_X509CertGetPrivateKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertGetPrivateKey
	#define EXT_X509CertGetPrivateKey
	#define GET_X509CertGetPrivateKey(fl)  CAL_CMGETAPI( "X509CertGetPrivateKey" ) 
	#define CAL_X509CertGetPrivateKey  X509CertGetPrivateKey
	#define CHK_X509CertGetPrivateKey  TRUE
	#define EXP_X509CertGetPrivateKey  CAL_CMEXPAPI( "X509CertGetPrivateKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertGetPrivateKey
	#define EXT_X509CertGetPrivateKey
	#define GET_X509CertGetPrivateKey(fl)  CAL_CMGETAPI( "X509CertGetPrivateKey" ) 
	#define CAL_X509CertGetPrivateKey  X509CertGetPrivateKey
	#define CHK_X509CertGetPrivateKey  TRUE
	#define EXP_X509CertGetPrivateKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetPrivateKey", (RTS_UINTPTR)X509CertGetPrivateKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertGetPrivateKey
	#define EXT_CmpX509CertX509CertGetPrivateKey
	#define GET_CmpX509CertX509CertGetPrivateKey  ERR_OK
	#define CAL_CmpX509CertX509CertGetPrivateKey pICmpX509Cert->IX509CertGetPrivateKey
	#define CHK_CmpX509CertX509CertGetPrivateKey (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertGetPrivateKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertGetPrivateKey
	#define EXT_X509CertGetPrivateKey
	#define GET_X509CertGetPrivateKey(fl)  CAL_CMGETAPI( "X509CertGetPrivateKey" ) 
	#define CAL_X509CertGetPrivateKey pICmpX509Cert->IX509CertGetPrivateKey
	#define CHK_X509CertGetPrivateKey (pICmpX509Cert != NULL)
	#define EXP_X509CertGetPrivateKey  CAL_CMEXPAPI( "X509CertGetPrivateKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertGetPrivateKey  PFX509CERTGETPRIVATEKEY pfX509CertGetPrivateKey;
	#define EXT_X509CertGetPrivateKey  extern PFX509CERTGETPRIVATEKEY pfX509CertGetPrivateKey;
	#define GET_X509CertGetPrivateKey(fl)  s_pfCMGetAPI2( "X509CertGetPrivateKey", (RTS_VOID_FCTPTR *)&pfX509CertGetPrivateKey, (fl), 0, 0)
	#define CAL_X509CertGetPrivateKey  pfX509CertGetPrivateKey
	#define CHK_X509CertGetPrivateKey  (pfX509CertGetPrivateKey != NULL)
	#define EXP_X509CertGetPrivateKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetPrivateKey", (RTS_UINTPTR)X509CertGetPrivateKey, 0, 0) 
#endif




/**
 * <description>Generate the certificate thumbprint. The thumbprint will be just the plain bytes of the hash.
 * Set the hash algorithm to your needs.</description>
 * <param name="hCert" type="IN">Handle of the certificate of intrest.</param>
 * <param name="hashAlgorithm" type="IN">The hashing algorithmen used to generate the thumbprint.</param>
 * <param name="pThumbprint" type="OUT">Buffer to store the thumbprint. If bytestring is NULL memory ia allocated.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertGetThumbprint(RTS_HANDLE hCert,  RTSCRYPTOID* hashAlgorithm, RtsByteString* pThumbprint);
typedef RTS_RESULT (CDECL * PFX509CERTGETTHUMBPRINT) (RTS_HANDLE hCert,  RTSCRYPTOID* hashAlgorithm, RtsByteString* pThumbprint);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTGETTHUMBPRINT_NOTIMPLEMENTED)
	#define USE_X509CertGetThumbprint
	#define EXT_X509CertGetThumbprint
	#define GET_X509CertGetThumbprint(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertGetThumbprint(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertGetThumbprint  FALSE
	#define EXP_X509CertGetThumbprint  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertGetThumbprint
	#define EXT_X509CertGetThumbprint
	#define GET_X509CertGetThumbprint(fl)  CAL_CMGETAPI( "X509CertGetThumbprint" ) 
	#define CAL_X509CertGetThumbprint  X509CertGetThumbprint
	#define CHK_X509CertGetThumbprint  TRUE
	#define EXP_X509CertGetThumbprint  CAL_CMEXPAPI( "X509CertGetThumbprint" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertGetThumbprint
	#define EXT_X509CertGetThumbprint
	#define GET_X509CertGetThumbprint(fl)  CAL_CMGETAPI( "X509CertGetThumbprint" ) 
	#define CAL_X509CertGetThumbprint  X509CertGetThumbprint
	#define CHK_X509CertGetThumbprint  TRUE
	#define EXP_X509CertGetThumbprint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetThumbprint", (RTS_UINTPTR)X509CertGetThumbprint, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertGetThumbprint
	#define EXT_CmpX509CertX509CertGetThumbprint
	#define GET_CmpX509CertX509CertGetThumbprint  ERR_OK
	#define CAL_CmpX509CertX509CertGetThumbprint pICmpX509Cert->IX509CertGetThumbprint
	#define CHK_CmpX509CertX509CertGetThumbprint (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertGetThumbprint  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertGetThumbprint
	#define EXT_X509CertGetThumbprint
	#define GET_X509CertGetThumbprint(fl)  CAL_CMGETAPI( "X509CertGetThumbprint" ) 
	#define CAL_X509CertGetThumbprint pICmpX509Cert->IX509CertGetThumbprint
	#define CHK_X509CertGetThumbprint (pICmpX509Cert != NULL)
	#define EXP_X509CertGetThumbprint  CAL_CMEXPAPI( "X509CertGetThumbprint" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertGetThumbprint  PFX509CERTGETTHUMBPRINT pfX509CertGetThumbprint;
	#define EXT_X509CertGetThumbprint  extern PFX509CERTGETTHUMBPRINT pfX509CertGetThumbprint;
	#define GET_X509CertGetThumbprint(fl)  s_pfCMGetAPI2( "X509CertGetThumbprint", (RTS_VOID_FCTPTR *)&pfX509CertGetThumbprint, (fl), 0, 0)
	#define CAL_X509CertGetThumbprint  pfX509CertGetThumbprint
	#define CHK_X509CertGetThumbprint  (pfX509CertGetThumbprint != NULL)
	#define EXP_X509CertGetThumbprint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertGetThumbprint", (RTS_UINTPTR)X509CertGetThumbprint, 0, 0) 
#endif




/**
 * <description>Check if a certificate has a valid date (notbefore and notafter is in the current time period)</description>
 * <param name="hCert" type="IN">Handle of the certificate</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">hCert is valid</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">hCert is invalid</errorcode>
 * <result>Result of the operation</result>
 * <errorcode name="RTS_BOOL" type="TRUE">Date of certificate is valid</errorcode>
 * <errorcode name="RTS_BOOL" type="FALSE">Date of certificate is out of current period</errorcode>
 */
RTS_BOOL CDECL X509CertIsDateValid(RTS_HANDLE hCert, RTS_RESULT *pResult);
typedef RTS_BOOL (CDECL * PFX509CERTISDATEVALID) (RTS_HANDLE hCert, RTS_RESULT *pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTISDATEVALID_NOTIMPLEMENTED)
	#define USE_X509CertIsDateValid
	#define EXT_X509CertIsDateValid
	#define GET_X509CertIsDateValid(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertIsDateValid(p0,p1)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_X509CertIsDateValid  FALSE
	#define EXP_X509CertIsDateValid  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertIsDateValid
	#define EXT_X509CertIsDateValid
	#define GET_X509CertIsDateValid(fl)  CAL_CMGETAPI( "X509CertIsDateValid" ) 
	#define CAL_X509CertIsDateValid  X509CertIsDateValid
	#define CHK_X509CertIsDateValid  TRUE
	#define EXP_X509CertIsDateValid  CAL_CMEXPAPI( "X509CertIsDateValid" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertIsDateValid
	#define EXT_X509CertIsDateValid
	#define GET_X509CertIsDateValid(fl)  CAL_CMGETAPI( "X509CertIsDateValid" ) 
	#define CAL_X509CertIsDateValid  X509CertIsDateValid
	#define CHK_X509CertIsDateValid  TRUE
	#define EXP_X509CertIsDateValid  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertIsDateValid", (RTS_UINTPTR)X509CertIsDateValid, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertIsDateValid
	#define EXT_CmpX509CertX509CertIsDateValid
	#define GET_CmpX509CertX509CertIsDateValid  ERR_OK
	#define CAL_CmpX509CertX509CertIsDateValid pICmpX509Cert->IX509CertIsDateValid
	#define CHK_CmpX509CertX509CertIsDateValid (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertIsDateValid  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertIsDateValid
	#define EXT_X509CertIsDateValid
	#define GET_X509CertIsDateValid(fl)  CAL_CMGETAPI( "X509CertIsDateValid" ) 
	#define CAL_X509CertIsDateValid pICmpX509Cert->IX509CertIsDateValid
	#define CHK_X509CertIsDateValid (pICmpX509Cert != NULL)
	#define EXP_X509CertIsDateValid  CAL_CMEXPAPI( "X509CertIsDateValid" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertIsDateValid  PFX509CERTISDATEVALID pfX509CertIsDateValid;
	#define EXT_X509CertIsDateValid  extern PFX509CERTISDATEVALID pfX509CertIsDateValid;
	#define GET_X509CertIsDateValid(fl)  s_pfCMGetAPI2( "X509CertIsDateValid", (RTS_VOID_FCTPTR *)&pfX509CertIsDateValid, (fl), 0, 0)
	#define CAL_X509CertIsDateValid  pfX509CertIsDateValid
	#define CHK_X509CertIsDateValid  (pfX509CertIsDateValid != NULL)
	#define EXP_X509CertIsDateValid  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertIsDateValid", (RTS_UINTPTR)X509CertIsDateValid, 0, 0) 
#endif




/**
 * <description>Check if a certificate is selfsigned of is signed by a CA authority</description>
 * <param name="hCert" type="IN">Handle of the certificate</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">hCert is valid</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">hCert is invalid</errorcode>
 * <result>Result of the operation</result>
 * <errorcode name="RTS_BOOL" type="TRUE">Certificate is selfsigned</errorcode>
 * <errorcode name="RTS_BOOL" type="FALSE">Certificate is signed ba a CA authority</errorcode>
 */
RTS_BOOL CDECL X509CertIsSelfSigned(RTS_HANDLE hCert, RTS_RESULT *pResult);
typedef RTS_BOOL (CDECL * PFX509CERTISSELFSIGNED) (RTS_HANDLE hCert, RTS_RESULT *pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTISSELFSIGNED_NOTIMPLEMENTED)
	#define USE_X509CertIsSelfSigned
	#define EXT_X509CertIsSelfSigned
	#define GET_X509CertIsSelfSigned(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertIsSelfSigned(p0,p1)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_X509CertIsSelfSigned  FALSE
	#define EXP_X509CertIsSelfSigned  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertIsSelfSigned
	#define EXT_X509CertIsSelfSigned
	#define GET_X509CertIsSelfSigned(fl)  CAL_CMGETAPI( "X509CertIsSelfSigned" ) 
	#define CAL_X509CertIsSelfSigned  X509CertIsSelfSigned
	#define CHK_X509CertIsSelfSigned  TRUE
	#define EXP_X509CertIsSelfSigned  CAL_CMEXPAPI( "X509CertIsSelfSigned" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertIsSelfSigned
	#define EXT_X509CertIsSelfSigned
	#define GET_X509CertIsSelfSigned(fl)  CAL_CMGETAPI( "X509CertIsSelfSigned" ) 
	#define CAL_X509CertIsSelfSigned  X509CertIsSelfSigned
	#define CHK_X509CertIsSelfSigned  TRUE
	#define EXP_X509CertIsSelfSigned  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertIsSelfSigned", (RTS_UINTPTR)X509CertIsSelfSigned, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertIsSelfSigned
	#define EXT_CmpX509CertX509CertIsSelfSigned
	#define GET_CmpX509CertX509CertIsSelfSigned  ERR_OK
	#define CAL_CmpX509CertX509CertIsSelfSigned pICmpX509Cert->IX509CertIsSelfSigned
	#define CHK_CmpX509CertX509CertIsSelfSigned (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertIsSelfSigned  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertIsSelfSigned
	#define EXT_X509CertIsSelfSigned
	#define GET_X509CertIsSelfSigned(fl)  CAL_CMGETAPI( "X509CertIsSelfSigned" ) 
	#define CAL_X509CertIsSelfSigned pICmpX509Cert->IX509CertIsSelfSigned
	#define CHK_X509CertIsSelfSigned (pICmpX509Cert != NULL)
	#define EXP_X509CertIsSelfSigned  CAL_CMEXPAPI( "X509CertIsSelfSigned" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertIsSelfSigned  PFX509CERTISSELFSIGNED pfX509CertIsSelfSigned;
	#define EXT_X509CertIsSelfSigned  extern PFX509CERTISSELFSIGNED pfX509CertIsSelfSigned;
	#define GET_X509CertIsSelfSigned(fl)  s_pfCMGetAPI2( "X509CertIsSelfSigned", (RTS_VOID_FCTPTR *)&pfX509CertIsSelfSigned, (fl), 0, 0)
	#define CAL_X509CertIsSelfSigned  pfX509CertIsSelfSigned
	#define CHK_X509CertIsSelfSigned  (pfX509CertIsSelfSigned != NULL)
	#define EXP_X509CertIsSelfSigned  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertIsSelfSigned", (RTS_UINTPTR)X509CertIsSelfSigned, 0, 0) 
#endif




/**
 * <description>Check if the extended keyusages of the specified certificate has the specified extended keyusages</description>
 * <param name="hCert" type="IN">Handle of the certificate</param>
 * <param name="numOfExKeyUsages" type="IN">Number of extended keyusages</param>
 * <param name="pExKeyUsages" type="IN">Pointer to list of extended keyusages</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">hCert is valid</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">hCert is invalid</errorcode>
 * <result>Result of the operation</result>
 * <errorcode name="RTS_BOOL" type="TRUE">Extended keyusages identical</errorcode>
 * <errorcode name="RTS_BOOL" type="FALSE">Extended keyusages not identical identical</errorcode>
 */
RTS_BOOL CDECL X509CertHasExtendedKeyUsage(RTS_HANDLE hCert, RTS_UI32 numOfExKeyUsages, RtsOID *pExKeyUsages, RTS_RESULT *pResult);
typedef RTS_BOOL (CDECL * PFX509CERTHASEXTENDEDKEYUSAGE) (RTS_HANDLE hCert, RTS_UI32 numOfExKeyUsages, RtsOID *pExKeyUsages, RTS_RESULT *pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTHASEXTENDEDKEYUSAGE_NOTIMPLEMENTED)
	#define USE_X509CertHasExtendedKeyUsage
	#define EXT_X509CertHasExtendedKeyUsage
	#define GET_X509CertHasExtendedKeyUsage(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertHasExtendedKeyUsage(p0,p1,p2,p3)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_X509CertHasExtendedKeyUsage  FALSE
	#define EXP_X509CertHasExtendedKeyUsage  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertHasExtendedKeyUsage
	#define EXT_X509CertHasExtendedKeyUsage
	#define GET_X509CertHasExtendedKeyUsage(fl)  CAL_CMGETAPI( "X509CertHasExtendedKeyUsage" ) 
	#define CAL_X509CertHasExtendedKeyUsage  X509CertHasExtendedKeyUsage
	#define CHK_X509CertHasExtendedKeyUsage  TRUE
	#define EXP_X509CertHasExtendedKeyUsage  CAL_CMEXPAPI( "X509CertHasExtendedKeyUsage" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertHasExtendedKeyUsage
	#define EXT_X509CertHasExtendedKeyUsage
	#define GET_X509CertHasExtendedKeyUsage(fl)  CAL_CMGETAPI( "X509CertHasExtendedKeyUsage" ) 
	#define CAL_X509CertHasExtendedKeyUsage  X509CertHasExtendedKeyUsage
	#define CHK_X509CertHasExtendedKeyUsage  TRUE
	#define EXP_X509CertHasExtendedKeyUsage  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertHasExtendedKeyUsage", (RTS_UINTPTR)X509CertHasExtendedKeyUsage, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertHasExtendedKeyUsage
	#define EXT_CmpX509CertX509CertHasExtendedKeyUsage
	#define GET_CmpX509CertX509CertHasExtendedKeyUsage  ERR_OK
	#define CAL_CmpX509CertX509CertHasExtendedKeyUsage pICmpX509Cert->IX509CertHasExtendedKeyUsage
	#define CHK_CmpX509CertX509CertHasExtendedKeyUsage (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertHasExtendedKeyUsage  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertHasExtendedKeyUsage
	#define EXT_X509CertHasExtendedKeyUsage
	#define GET_X509CertHasExtendedKeyUsage(fl)  CAL_CMGETAPI( "X509CertHasExtendedKeyUsage" ) 
	#define CAL_X509CertHasExtendedKeyUsage pICmpX509Cert->IX509CertHasExtendedKeyUsage
	#define CHK_X509CertHasExtendedKeyUsage (pICmpX509Cert != NULL)
	#define EXP_X509CertHasExtendedKeyUsage  CAL_CMEXPAPI( "X509CertHasExtendedKeyUsage" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertHasExtendedKeyUsage  PFX509CERTHASEXTENDEDKEYUSAGE pfX509CertHasExtendedKeyUsage;
	#define EXT_X509CertHasExtendedKeyUsage  extern PFX509CERTHASEXTENDEDKEYUSAGE pfX509CertHasExtendedKeyUsage;
	#define GET_X509CertHasExtendedKeyUsage(fl)  s_pfCMGetAPI2( "X509CertHasExtendedKeyUsage", (RTS_VOID_FCTPTR *)&pfX509CertHasExtendedKeyUsage, (fl), 0, 0)
	#define CAL_X509CertHasExtendedKeyUsage  pfX509CertHasExtendedKeyUsage
	#define CHK_X509CertHasExtendedKeyUsage  (pfX509CertHasExtendedKeyUsage != NULL)
	#define EXP_X509CertHasExtendedKeyUsage  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertHasExtendedKeyUsage", (RTS_UINTPTR)X509CertHasExtendedKeyUsage, 0, 0) 
#endif




/**
 * <description>Compare two certificates if they are identical.</description>
 * <param name="hCert1" type="IN">Handle to first certificate</param>
 * <param name="hCert1" type="IN">Handle to second certificate</param>
 * <result>TRUE if they are identical, FALSE if not.</result>
 */
RTS_BOOL CDECL X509CertCompare(RTS_HANDLE hCert1, RTS_HANDLE hCert2);
typedef RTS_BOOL (CDECL * PFX509CERTCOMPARE) (RTS_HANDLE hCert1, RTS_HANDLE hCert2);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCOMPARE_NOTIMPLEMENTED)
	#define USE_X509CertCompare
	#define EXT_X509CertCompare
	#define GET_X509CertCompare(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertCompare(p0,p1)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_X509CertCompare  FALSE
	#define EXP_X509CertCompare  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertCompare
	#define EXT_X509CertCompare
	#define GET_X509CertCompare(fl)  CAL_CMGETAPI( "X509CertCompare" ) 
	#define CAL_X509CertCompare  X509CertCompare
	#define CHK_X509CertCompare  TRUE
	#define EXP_X509CertCompare  CAL_CMEXPAPI( "X509CertCompare" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertCompare
	#define EXT_X509CertCompare
	#define GET_X509CertCompare(fl)  CAL_CMGETAPI( "X509CertCompare" ) 
	#define CAL_X509CertCompare  X509CertCompare
	#define CHK_X509CertCompare  TRUE
	#define EXP_X509CertCompare  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCompare", (RTS_UINTPTR)X509CertCompare, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertCompare
	#define EXT_CmpX509CertX509CertCompare
	#define GET_CmpX509CertX509CertCompare  ERR_OK
	#define CAL_CmpX509CertX509CertCompare pICmpX509Cert->IX509CertCompare
	#define CHK_CmpX509CertX509CertCompare (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertCompare  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertCompare
	#define EXT_X509CertCompare
	#define GET_X509CertCompare(fl)  CAL_CMGETAPI( "X509CertCompare" ) 
	#define CAL_X509CertCompare pICmpX509Cert->IX509CertCompare
	#define CHK_X509CertCompare (pICmpX509Cert != NULL)
	#define EXP_X509CertCompare  CAL_CMEXPAPI( "X509CertCompare" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertCompare  PFX509CERTCOMPARE pfX509CertCompare;
	#define EXT_X509CertCompare  extern PFX509CERTCOMPARE pfX509CertCompare;
	#define GET_X509CertCompare(fl)  s_pfCMGetAPI2( "X509CertCompare", (RTS_VOID_FCTPTR *)&pfX509CertCompare, (fl), 0, 0)
	#define CAL_X509CertCompare  pfX509CertCompare
	#define CHK_X509CertCompare  (pfX509CertCompare != NULL)
	#define EXP_X509CertCompare  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCompare", (RTS_UINTPTR)X509CertCompare, 0, 0) 
#endif




/**
 * <description>Release a private or public key with this function. This function
 * indicates that this key is not needed anymore and allows to clean up ressources
 * used by this key</description>
 * <para name="pKey" type="IN">Pointer to the key</param>
 * <result>Result of the operation</result>
 */
RTS_RESULT CDECL X509CertKeyClose(RtsCryptoKey* pKey);
typedef RTS_RESULT (CDECL * PFX509CERTKEYCLOSE) (RtsCryptoKey* pKey);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTKEYCLOSE_NOTIMPLEMENTED)
	#define USE_X509CertKeyClose
	#define EXT_X509CertKeyClose
	#define GET_X509CertKeyClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertKeyClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertKeyClose  FALSE
	#define EXP_X509CertKeyClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertKeyClose
	#define EXT_X509CertKeyClose
	#define GET_X509CertKeyClose(fl)  CAL_CMGETAPI( "X509CertKeyClose" ) 
	#define CAL_X509CertKeyClose  X509CertKeyClose
	#define CHK_X509CertKeyClose  TRUE
	#define EXP_X509CertKeyClose  CAL_CMEXPAPI( "X509CertKeyClose" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertKeyClose
	#define EXT_X509CertKeyClose
	#define GET_X509CertKeyClose(fl)  CAL_CMGETAPI( "X509CertKeyClose" ) 
	#define CAL_X509CertKeyClose  X509CertKeyClose
	#define CHK_X509CertKeyClose  TRUE
	#define EXP_X509CertKeyClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertKeyClose", (RTS_UINTPTR)X509CertKeyClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertKeyClose
	#define EXT_CmpX509CertX509CertKeyClose
	#define GET_CmpX509CertX509CertKeyClose  ERR_OK
	#define CAL_CmpX509CertX509CertKeyClose pICmpX509Cert->IX509CertKeyClose
	#define CHK_CmpX509CertX509CertKeyClose (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertKeyClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertKeyClose
	#define EXT_X509CertKeyClose
	#define GET_X509CertKeyClose(fl)  CAL_CMGETAPI( "X509CertKeyClose" ) 
	#define CAL_X509CertKeyClose pICmpX509Cert->IX509CertKeyClose
	#define CHK_X509CertKeyClose (pICmpX509Cert != NULL)
	#define EXP_X509CertKeyClose  CAL_CMEXPAPI( "X509CertKeyClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertKeyClose  PFX509CERTKEYCLOSE pfX509CertKeyClose;
	#define EXT_X509CertKeyClose  extern PFX509CERTKEYCLOSE pfX509CertKeyClose;
	#define GET_X509CertKeyClose(fl)  s_pfCMGetAPI2( "X509CertKeyClose", (RTS_VOID_FCTPTR *)&pfX509CertKeyClose, (fl), 0, 0)
	#define CAL_X509CertKeyClose  pfX509CertKeyClose
	#define CHK_X509CertKeyClose  (pfX509CertKeyClose != NULL)
	#define EXP_X509CertKeyClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertKeyClose", (RTS_UINTPTR)X509CertKeyClose, 0, 0) 
#endif




/**
 * <description>Close the certificate.</description>
 * <param name="hCert" type="IN">Handle of to certificate.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertClose(RTS_HANDLE hCert);
typedef RTS_RESULT (CDECL * PFX509CERTCLOSE) (RTS_HANDLE hCert);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCLOSE_NOTIMPLEMENTED)
	#define USE_X509CertClose
	#define EXT_X509CertClose
	#define GET_X509CertClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertClose  FALSE
	#define EXP_X509CertClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertClose
	#define EXT_X509CertClose
	#define GET_X509CertClose(fl)  CAL_CMGETAPI( "X509CertClose" ) 
	#define CAL_X509CertClose  X509CertClose
	#define CHK_X509CertClose  TRUE
	#define EXP_X509CertClose  CAL_CMEXPAPI( "X509CertClose" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertClose
	#define EXT_X509CertClose
	#define GET_X509CertClose(fl)  CAL_CMGETAPI( "X509CertClose" ) 
	#define CAL_X509CertClose  X509CertClose
	#define CHK_X509CertClose  TRUE
	#define EXP_X509CertClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertClose", (RTS_UINTPTR)X509CertClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertClose
	#define EXT_CmpX509CertX509CertClose
	#define GET_CmpX509CertX509CertClose  ERR_OK
	#define CAL_CmpX509CertX509CertClose pICmpX509Cert->IX509CertClose
	#define CHK_CmpX509CertX509CertClose (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertClose
	#define EXT_X509CertClose
	#define GET_X509CertClose(fl)  CAL_CMGETAPI( "X509CertClose" ) 
	#define CAL_X509CertClose pICmpX509Cert->IX509CertClose
	#define CHK_X509CertClose (pICmpX509Cert != NULL)
	#define EXP_X509CertClose  CAL_CMEXPAPI( "X509CertClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertClose  PFX509CERTCLOSE pfX509CertClose;
	#define EXT_X509CertClose  extern PFX509CERTCLOSE pfX509CertClose;
	#define GET_X509CertClose(fl)  s_pfCMGetAPI2( "X509CertClose", (RTS_VOID_FCTPTR *)&pfX509CertClose, (fl), 0, 0)
	#define CAL_X509CertClose  pfX509CertClose
	#define CHK_X509CertClose  (pfX509CertClose != NULL)
	#define EXP_X509CertClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertClose", (RTS_UINTPTR)X509CertClose, 0, 0) 
#endif




/* --------------------------------------------------------------------------- */
/*                       Handling CMS containers                               */
/* --------------------------------------------------------------------------- */

/**
 * <description>Verifys a CMS signature. This function can handle both (detached and attached) signatures.
 * The signers certificate has to be located in the CMS container. Otherwhise the verify will fail.
 * The signers certificate is always verified against the trusted certificate store on the PLC.</description>
 * <param name="hCertStore" type="IN">Handle to the certificate store.</param>
 * <param name="pCms" type="IN">Pointer to a bytestring storing the CMS container.</param>
 * <param name="encoding" type="IN">Encoding of the CMS container.</param>
 * <param name="pInData" type="IN">Used only if the signature is detached. In this case this bytestring has to store the data to be verified</param>
 * <param name="pOutData" type="IN">Pointer to a bytestring where to store the extraced data.</param>
 * <result>ERR_OK if the signature is valid and the signer was trustworthy.
 * ERR_SIGNATURE_MISMATCH if the signature is invalid and / or the signer was not trustworthy.
 * ERR_PARAMETER if the given parameters are not consistent.</result>
 */
RTS_RESULT CDECL X509CertCmsVerify(RTS_HANDLE hCertStore, RtsByteString* pCms, RTSCERTENCODING encoding, RtsByteString* pInData, RtsByteString* pOutData);
typedef RTS_RESULT (CDECL * PFX509CERTCMSVERIFY) (RTS_HANDLE hCertStore, RtsByteString* pCms, RTSCERTENCODING encoding, RtsByteString* pInData, RtsByteString* pOutData);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCMSVERIFY_NOTIMPLEMENTED)
	#define USE_X509CertCmsVerify
	#define EXT_X509CertCmsVerify
	#define GET_X509CertCmsVerify(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertCmsVerify(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertCmsVerify  FALSE
	#define EXP_X509CertCmsVerify  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertCmsVerify
	#define EXT_X509CertCmsVerify
	#define GET_X509CertCmsVerify(fl)  CAL_CMGETAPI( "X509CertCmsVerify" ) 
	#define CAL_X509CertCmsVerify  X509CertCmsVerify
	#define CHK_X509CertCmsVerify  TRUE
	#define EXP_X509CertCmsVerify  CAL_CMEXPAPI( "X509CertCmsVerify" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertCmsVerify
	#define EXT_X509CertCmsVerify
	#define GET_X509CertCmsVerify(fl)  CAL_CMGETAPI( "X509CertCmsVerify" ) 
	#define CAL_X509CertCmsVerify  X509CertCmsVerify
	#define CHK_X509CertCmsVerify  TRUE
	#define EXP_X509CertCmsVerify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCmsVerify", (RTS_UINTPTR)X509CertCmsVerify, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertCmsVerify
	#define EXT_CmpX509CertX509CertCmsVerify
	#define GET_CmpX509CertX509CertCmsVerify  ERR_OK
	#define CAL_CmpX509CertX509CertCmsVerify pICmpX509Cert->IX509CertCmsVerify
	#define CHK_CmpX509CertX509CertCmsVerify (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertCmsVerify  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertCmsVerify
	#define EXT_X509CertCmsVerify
	#define GET_X509CertCmsVerify(fl)  CAL_CMGETAPI( "X509CertCmsVerify" ) 
	#define CAL_X509CertCmsVerify pICmpX509Cert->IX509CertCmsVerify
	#define CHK_X509CertCmsVerify (pICmpX509Cert != NULL)
	#define EXP_X509CertCmsVerify  CAL_CMEXPAPI( "X509CertCmsVerify" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertCmsVerify  PFX509CERTCMSVERIFY pfX509CertCmsVerify;
	#define EXT_X509CertCmsVerify  extern PFX509CERTCMSVERIFY pfX509CertCmsVerify;
	#define GET_X509CertCmsVerify(fl)  s_pfCMGetAPI2( "X509CertCmsVerify", (RTS_VOID_FCTPTR *)&pfX509CertCmsVerify, (fl), 0, 0)
	#define CAL_X509CertCmsVerify  pfX509CertCmsVerify
	#define CHK_X509CertCmsVerify  (pfX509CertCmsVerify != NULL)
	#define EXP_X509CertCmsVerify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCmsVerify", (RTS_UINTPTR)X509CertCmsVerify, 0, 0) 
#endif




/**
 * <description>The description of X509CertCmsVerify applies here too. Additional this interface returns a handle to the signers certificate.</description>
 * <param name="hCertStore" type="IN">Handle to the certificate store.</param>
 * <param name="pCms" type="IN">Pointer to a bytestring storing the CMS container.</param>
 * <param name="encoding" type="IN">Encoding of the CMS container.</param>
 * <param name="pInData" type="IN">Used only if the signature is detached. In this case this bytestring has to store the data to be verified</param>
 * <param name="pOutData" type="IN">Pointer to a bytestring where to store the extraced data.</param>
 * <param name="phSignerCert" type="IN">Pointer to a handle where to store the signers certificate. 
 *  This handle is only returned if the verification was successful. If the container was signed multiple times this handle contains the first valid certificate. 
 *  Can be NULL.
 * </param>
 * <result>ERR_OK if the signature is valid and the signer was trustworthy.
 * ERR_SIGNATURE_MISMATCH if the signature is invalid and / or the signer was not trustworthy.
 * ERR_PARAMETER if the given parameters are not consistent.</result>
 */
RTS_RESULT CDECL X509CertCmsVerify2(RTS_HANDLE hCertStore, RtsByteString* pCms, RTSCERTENCODING encoding, RtsByteString* pInData, RtsByteString* pOutData, RTS_HANDLE* phSignerCert);
typedef RTS_RESULT (CDECL * PFX509CERTCMSVERIFY2) (RTS_HANDLE hCertStore, RtsByteString* pCms, RTSCERTENCODING encoding, RtsByteString* pInData, RtsByteString* pOutData, RTS_HANDLE* phSignerCert);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCMSVERIFY2_NOTIMPLEMENTED)
	#define USE_X509CertCmsVerify2
	#define EXT_X509CertCmsVerify2
	#define GET_X509CertCmsVerify2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertCmsVerify2(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertCmsVerify2  FALSE
	#define EXP_X509CertCmsVerify2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertCmsVerify2
	#define EXT_X509CertCmsVerify2
	#define GET_X509CertCmsVerify2(fl)  CAL_CMGETAPI( "X509CertCmsVerify2" ) 
	#define CAL_X509CertCmsVerify2  X509CertCmsVerify2
	#define CHK_X509CertCmsVerify2  TRUE
	#define EXP_X509CertCmsVerify2  CAL_CMEXPAPI( "X509CertCmsVerify2" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertCmsVerify2
	#define EXT_X509CertCmsVerify2
	#define GET_X509CertCmsVerify2(fl)  CAL_CMGETAPI( "X509CertCmsVerify2" ) 
	#define CAL_X509CertCmsVerify2  X509CertCmsVerify2
	#define CHK_X509CertCmsVerify2  TRUE
	#define EXP_X509CertCmsVerify2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCmsVerify2", (RTS_UINTPTR)X509CertCmsVerify2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertCmsVerify2
	#define EXT_CmpX509CertX509CertCmsVerify2
	#define GET_CmpX509CertX509CertCmsVerify2  ERR_OK
	#define CAL_CmpX509CertX509CertCmsVerify2 pICmpX509Cert->IX509CertCmsVerify2
	#define CHK_CmpX509CertX509CertCmsVerify2 (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertCmsVerify2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertCmsVerify2
	#define EXT_X509CertCmsVerify2
	#define GET_X509CertCmsVerify2(fl)  CAL_CMGETAPI( "X509CertCmsVerify2" ) 
	#define CAL_X509CertCmsVerify2 pICmpX509Cert->IX509CertCmsVerify2
	#define CHK_X509CertCmsVerify2 (pICmpX509Cert != NULL)
	#define EXP_X509CertCmsVerify2  CAL_CMEXPAPI( "X509CertCmsVerify2" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertCmsVerify2  PFX509CERTCMSVERIFY2 pfX509CertCmsVerify2;
	#define EXT_X509CertCmsVerify2  extern PFX509CERTCMSVERIFY2 pfX509CertCmsVerify2;
	#define GET_X509CertCmsVerify2(fl)  s_pfCMGetAPI2( "X509CertCmsVerify2", (RTS_VOID_FCTPTR *)&pfX509CertCmsVerify2, (fl), 0, 0)
	#define CAL_X509CertCmsVerify2  pfX509CertCmsVerify2
	#define CHK_X509CertCmsVerify2  (pfX509CertCmsVerify2 != NULL)
	#define EXP_X509CertCmsVerify2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCmsVerify2", (RTS_UINTPTR)X509CertCmsVerify2, 0, 0) 
#endif




/**
 * <description>Decrypt a CMS container.</description>
 * <param name="hCertStore" type="IN">Handle to the certificate store.</param>
 * <param name="hRecipientCert" type="IN">Handle to the recipients certificat. A private key of the certificate has to be available. Though the certificate has to be located in the "own" store.</param>
 * <param name="pCms" type="IN">Pointer to a bytestring storing the CMS container.</param>
 * <param name="encoding" type="IN">Encoding of the CMS container.</param>
 * <param name="pOutData" type="IN">Pointer to a bytestring where to store the extraced data.</param>
 * <result>ERR_OK if the container was successfully decrypted.
 * ERR_PARAMETER if the given parameters are not consistent.</result>
 */
RTS_RESULT CDECL X509CertCmsDecrypt(RTS_HANDLE hCertStore, RTS_HANDLE hRecipientCert, RtsByteString* pCms, RTSCERTENCODING encoding, RtsByteString* pOutData);
typedef RTS_RESULT (CDECL * PFX509CERTCMSDECRYPT) (RTS_HANDLE hCertStore, RTS_HANDLE hRecipientCert, RtsByteString* pCms, RTSCERTENCODING encoding, RtsByteString* pOutData);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCMSDECRYPT_NOTIMPLEMENTED)
	#define USE_X509CertCmsDecrypt
	#define EXT_X509CertCmsDecrypt
	#define GET_X509CertCmsDecrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertCmsDecrypt(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertCmsDecrypt  FALSE
	#define EXP_X509CertCmsDecrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertCmsDecrypt
	#define EXT_X509CertCmsDecrypt
	#define GET_X509CertCmsDecrypt(fl)  CAL_CMGETAPI( "X509CertCmsDecrypt" ) 
	#define CAL_X509CertCmsDecrypt  X509CertCmsDecrypt
	#define CHK_X509CertCmsDecrypt  TRUE
	#define EXP_X509CertCmsDecrypt  CAL_CMEXPAPI( "X509CertCmsDecrypt" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertCmsDecrypt
	#define EXT_X509CertCmsDecrypt
	#define GET_X509CertCmsDecrypt(fl)  CAL_CMGETAPI( "X509CertCmsDecrypt" ) 
	#define CAL_X509CertCmsDecrypt  X509CertCmsDecrypt
	#define CHK_X509CertCmsDecrypt  TRUE
	#define EXP_X509CertCmsDecrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCmsDecrypt", (RTS_UINTPTR)X509CertCmsDecrypt, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertCmsDecrypt
	#define EXT_CmpX509CertX509CertCmsDecrypt
	#define GET_CmpX509CertX509CertCmsDecrypt  ERR_OK
	#define CAL_CmpX509CertX509CertCmsDecrypt pICmpX509Cert->IX509CertCmsDecrypt
	#define CHK_CmpX509CertX509CertCmsDecrypt (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertCmsDecrypt  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertCmsDecrypt
	#define EXT_X509CertCmsDecrypt
	#define GET_X509CertCmsDecrypt(fl)  CAL_CMGETAPI( "X509CertCmsDecrypt" ) 
	#define CAL_X509CertCmsDecrypt pICmpX509Cert->IX509CertCmsDecrypt
	#define CHK_X509CertCmsDecrypt (pICmpX509Cert != NULL)
	#define EXP_X509CertCmsDecrypt  CAL_CMEXPAPI( "X509CertCmsDecrypt" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertCmsDecrypt  PFX509CERTCMSDECRYPT pfX509CertCmsDecrypt;
	#define EXT_X509CertCmsDecrypt  extern PFX509CERTCMSDECRYPT pfX509CertCmsDecrypt;
	#define GET_X509CertCmsDecrypt(fl)  s_pfCMGetAPI2( "X509CertCmsDecrypt", (RTS_VOID_FCTPTR *)&pfX509CertCmsDecrypt, (fl), 0, 0)
	#define CAL_X509CertCmsDecrypt  pfX509CertCmsDecrypt
	#define CHK_X509CertCmsDecrypt  (pfX509CertCmsDecrypt != NULL)
	#define EXP_X509CertCmsDecrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCmsDecrypt", (RTS_UINTPTR)X509CertCmsDecrypt, 0, 0) 
#endif




/* --------------------------------------------------------------------------- */
/*                             Utility functions                               */
/* --------------------------------------------------------------------------- */

/**
 * <description>Create an RtsOID based on a given ID.</description>
 * <param name="id" type="IN">OID as string in dottet format. E.g. (1.2.3.4.5.6.7.8.9.0)</param>
 * <param name="oid" type="OUT">Pointer to an OID to fill up.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL RtsOIDCreate(char* id, RtsOID* oid);
typedef RTS_RESULT (CDECL * PFRTSOIDCREATE) (char* id, RtsOID* oid);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(RTSOIDCREATE_NOTIMPLEMENTED)
	#define USE_RtsOIDCreate
	#define EXT_RtsOIDCreate
	#define GET_RtsOIDCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RtsOIDCreate(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RtsOIDCreate  FALSE
	#define EXP_RtsOIDCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RtsOIDCreate
	#define EXT_RtsOIDCreate
	#define GET_RtsOIDCreate(fl)  CAL_CMGETAPI( "RtsOIDCreate" ) 
	#define CAL_RtsOIDCreate  RtsOIDCreate
	#define CHK_RtsOIDCreate  TRUE
	#define EXP_RtsOIDCreate  CAL_CMEXPAPI( "RtsOIDCreate" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_RtsOIDCreate
	#define EXT_RtsOIDCreate
	#define GET_RtsOIDCreate(fl)  CAL_CMGETAPI( "RtsOIDCreate" ) 
	#define CAL_RtsOIDCreate  RtsOIDCreate
	#define CHK_RtsOIDCreate  TRUE
	#define EXP_RtsOIDCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RtsOIDCreate", (RTS_UINTPTR)RtsOIDCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertRtsOIDCreate
	#define EXT_CmpX509CertRtsOIDCreate
	#define GET_CmpX509CertRtsOIDCreate  ERR_OK
	#define CAL_CmpX509CertRtsOIDCreate pICmpX509Cert->IRtsOIDCreate
	#define CHK_CmpX509CertRtsOIDCreate (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertRtsOIDCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RtsOIDCreate
	#define EXT_RtsOIDCreate
	#define GET_RtsOIDCreate(fl)  CAL_CMGETAPI( "RtsOIDCreate" ) 
	#define CAL_RtsOIDCreate pICmpX509Cert->IRtsOIDCreate
	#define CHK_RtsOIDCreate (pICmpX509Cert != NULL)
	#define EXP_RtsOIDCreate  CAL_CMEXPAPI( "RtsOIDCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_RtsOIDCreate  PFRTSOIDCREATE pfRtsOIDCreate;
	#define EXT_RtsOIDCreate  extern PFRTSOIDCREATE pfRtsOIDCreate;
	#define GET_RtsOIDCreate(fl)  s_pfCMGetAPI2( "RtsOIDCreate", (RTS_VOID_FCTPTR *)&pfRtsOIDCreate, (fl), 0, 0)
	#define CAL_RtsOIDCreate  pfRtsOIDCreate
	#define CHK_RtsOIDCreate  (pfRtsOIDCreate != NULL)
	#define EXP_RtsOIDCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RtsOIDCreate", (RTS_UINTPTR)RtsOIDCreate, 0, 0) 
#endif




/**
 * <description>Get the ID of an OID in dottet notation.</description>
 * <param name="oid" type="IN">OID to print.</param>
 * <param name="pBuffer" type="OUT">Buffer where to store the name.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL RtsOIDGetID(RtsOID* oid, RtsByteString* pBuffer);
typedef RTS_RESULT (CDECL * PFRTSOIDGETID) (RtsOID* oid, RtsByteString* pBuffer);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(RTSOIDGETID_NOTIMPLEMENTED)
	#define USE_RtsOIDGetID
	#define EXT_RtsOIDGetID
	#define GET_RtsOIDGetID(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RtsOIDGetID(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RtsOIDGetID  FALSE
	#define EXP_RtsOIDGetID  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RtsOIDGetID
	#define EXT_RtsOIDGetID
	#define GET_RtsOIDGetID(fl)  CAL_CMGETAPI( "RtsOIDGetID" ) 
	#define CAL_RtsOIDGetID  RtsOIDGetID
	#define CHK_RtsOIDGetID  TRUE
	#define EXP_RtsOIDGetID  CAL_CMEXPAPI( "RtsOIDGetID" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_RtsOIDGetID
	#define EXT_RtsOIDGetID
	#define GET_RtsOIDGetID(fl)  CAL_CMGETAPI( "RtsOIDGetID" ) 
	#define CAL_RtsOIDGetID  RtsOIDGetID
	#define CHK_RtsOIDGetID  TRUE
	#define EXP_RtsOIDGetID  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RtsOIDGetID", (RTS_UINTPTR)RtsOIDGetID, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertRtsOIDGetID
	#define EXT_CmpX509CertRtsOIDGetID
	#define GET_CmpX509CertRtsOIDGetID  ERR_OK
	#define CAL_CmpX509CertRtsOIDGetID pICmpX509Cert->IRtsOIDGetID
	#define CHK_CmpX509CertRtsOIDGetID (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertRtsOIDGetID  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RtsOIDGetID
	#define EXT_RtsOIDGetID
	#define GET_RtsOIDGetID(fl)  CAL_CMGETAPI( "RtsOIDGetID" ) 
	#define CAL_RtsOIDGetID pICmpX509Cert->IRtsOIDGetID
	#define CHK_RtsOIDGetID (pICmpX509Cert != NULL)
	#define EXP_RtsOIDGetID  CAL_CMEXPAPI( "RtsOIDGetID" ) 
#else /* DYNAMIC_LINK */
	#define USE_RtsOIDGetID  PFRTSOIDGETID pfRtsOIDGetID;
	#define EXT_RtsOIDGetID  extern PFRTSOIDGETID pfRtsOIDGetID;
	#define GET_RtsOIDGetID(fl)  s_pfCMGetAPI2( "RtsOIDGetID", (RTS_VOID_FCTPTR *)&pfRtsOIDGetID, (fl), 0, 0)
	#define CAL_RtsOIDGetID  pfRtsOIDGetID
	#define CHK_RtsOIDGetID  (pfRtsOIDGetID != NULL)
	#define EXP_RtsOIDGetID  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RtsOIDGetID", (RTS_UINTPTR)RtsOIDGetID, 0, 0) 
#endif




/**
 * <description>Get the given name of an OID. If the OID is unknown to the system the number.number.number... notation will be returned.</description>
 * <param name="oid" type="IN">OID to print.</param>
 * <param name="pBuffer" type="OUT">Buffer where to store the name.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL RtsOIDGetName(RtsOID* oid, RtsByteString* pBuffer);
typedef RTS_RESULT (CDECL * PFRTSOIDGETNAME) (RtsOID* oid, RtsByteString* pBuffer);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(RTSOIDGETNAME_NOTIMPLEMENTED)
	#define USE_RtsOIDGetName
	#define EXT_RtsOIDGetName
	#define GET_RtsOIDGetName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RtsOIDGetName(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RtsOIDGetName  FALSE
	#define EXP_RtsOIDGetName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RtsOIDGetName
	#define EXT_RtsOIDGetName
	#define GET_RtsOIDGetName(fl)  CAL_CMGETAPI( "RtsOIDGetName" ) 
	#define CAL_RtsOIDGetName  RtsOIDGetName
	#define CHK_RtsOIDGetName  TRUE
	#define EXP_RtsOIDGetName  CAL_CMEXPAPI( "RtsOIDGetName" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_RtsOIDGetName
	#define EXT_RtsOIDGetName
	#define GET_RtsOIDGetName(fl)  CAL_CMGETAPI( "RtsOIDGetName" ) 
	#define CAL_RtsOIDGetName  RtsOIDGetName
	#define CHK_RtsOIDGetName  TRUE
	#define EXP_RtsOIDGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RtsOIDGetName", (RTS_UINTPTR)RtsOIDGetName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertRtsOIDGetName
	#define EXT_CmpX509CertRtsOIDGetName
	#define GET_CmpX509CertRtsOIDGetName  ERR_OK
	#define CAL_CmpX509CertRtsOIDGetName pICmpX509Cert->IRtsOIDGetName
	#define CHK_CmpX509CertRtsOIDGetName (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertRtsOIDGetName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RtsOIDGetName
	#define EXT_RtsOIDGetName
	#define GET_RtsOIDGetName(fl)  CAL_CMGETAPI( "RtsOIDGetName" ) 
	#define CAL_RtsOIDGetName pICmpX509Cert->IRtsOIDGetName
	#define CHK_RtsOIDGetName (pICmpX509Cert != NULL)
	#define EXP_RtsOIDGetName  CAL_CMEXPAPI( "RtsOIDGetName" ) 
#else /* DYNAMIC_LINK */
	#define USE_RtsOIDGetName  PFRTSOIDGETNAME pfRtsOIDGetName;
	#define EXT_RtsOIDGetName  extern PFRTSOIDGETNAME pfRtsOIDGetName;
	#define GET_RtsOIDGetName(fl)  s_pfCMGetAPI2( "RtsOIDGetName", (RTS_VOID_FCTPTR *)&pfRtsOIDGetName, (fl), 0, 0)
	#define CAL_RtsOIDGetName  pfRtsOIDGetName
	#define CHK_RtsOIDGetName  (pfRtsOIDGetName != NULL)
	#define EXP_RtsOIDGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RtsOIDGetName", (RTS_UINTPTR)RtsOIDGetName, 0, 0) 
#endif




/**
 * <description>Clear an OID</description>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL RtsOIDClear(RtsOID* oid);
typedef RTS_RESULT (CDECL * PFRTSOIDCLEAR) (RtsOID* oid);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(RTSOIDCLEAR_NOTIMPLEMENTED)
	#define USE_RtsOIDClear
	#define EXT_RtsOIDClear
	#define GET_RtsOIDClear(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RtsOIDClear(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RtsOIDClear  FALSE
	#define EXP_RtsOIDClear  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RtsOIDClear
	#define EXT_RtsOIDClear
	#define GET_RtsOIDClear(fl)  CAL_CMGETAPI( "RtsOIDClear" ) 
	#define CAL_RtsOIDClear  RtsOIDClear
	#define CHK_RtsOIDClear  TRUE
	#define EXP_RtsOIDClear  CAL_CMEXPAPI( "RtsOIDClear" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_RtsOIDClear
	#define EXT_RtsOIDClear
	#define GET_RtsOIDClear(fl)  CAL_CMGETAPI( "RtsOIDClear" ) 
	#define CAL_RtsOIDClear  RtsOIDClear
	#define CHK_RtsOIDClear  TRUE
	#define EXP_RtsOIDClear  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RtsOIDClear", (RTS_UINTPTR)RtsOIDClear, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertRtsOIDClear
	#define EXT_CmpX509CertRtsOIDClear
	#define GET_CmpX509CertRtsOIDClear  ERR_OK
	#define CAL_CmpX509CertRtsOIDClear pICmpX509Cert->IRtsOIDClear
	#define CHK_CmpX509CertRtsOIDClear (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertRtsOIDClear  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RtsOIDClear
	#define EXT_RtsOIDClear
	#define GET_RtsOIDClear(fl)  CAL_CMGETAPI( "RtsOIDClear" ) 
	#define CAL_RtsOIDClear pICmpX509Cert->IRtsOIDClear
	#define CHK_RtsOIDClear (pICmpX509Cert != NULL)
	#define EXP_RtsOIDClear  CAL_CMEXPAPI( "RtsOIDClear" ) 
#else /* DYNAMIC_LINK */
	#define USE_RtsOIDClear  PFRTSOIDCLEAR pfRtsOIDClear;
	#define EXT_RtsOIDClear  extern PFRTSOIDCLEAR pfRtsOIDClear;
	#define GET_RtsOIDClear(fl)  s_pfCMGetAPI2( "RtsOIDClear", (RTS_VOID_FCTPTR *)&pfRtsOIDClear, (fl), 0, 0)
	#define CAL_RtsOIDClear  pfRtsOIDClear
	#define CHK_RtsOIDClear  (pfRtsOIDClear != NULL)
	#define EXP_RtsOIDClear  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RtsOIDClear", (RTS_UINTPTR)RtsOIDClear, 0, 0) 
#endif




/**
 * <description>Initialize the RtsX509CertInfo structure.</description>
 * <param name="pCertInfo" type="IN">Pointer to the structure.</param>
 * <param name="structSize" type="IN">Size of the structure.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertInfoInit(RtsX509CertInfo* pCertInfo, RTS_UI32 structSize);
typedef RTS_RESULT (CDECL * PFX509CERTINFOINIT) (RtsX509CertInfo* pCertInfo, RTS_UI32 structSize);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTINFOINIT_NOTIMPLEMENTED)
	#define USE_X509CertInfoInit
	#define EXT_X509CertInfoInit
	#define GET_X509CertInfoInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertInfoInit(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertInfoInit  FALSE
	#define EXP_X509CertInfoInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertInfoInit
	#define EXT_X509CertInfoInit
	#define GET_X509CertInfoInit(fl)  CAL_CMGETAPI( "X509CertInfoInit" ) 
	#define CAL_X509CertInfoInit  X509CertInfoInit
	#define CHK_X509CertInfoInit  TRUE
	#define EXP_X509CertInfoInit  CAL_CMEXPAPI( "X509CertInfoInit" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertInfoInit
	#define EXT_X509CertInfoInit
	#define GET_X509CertInfoInit(fl)  CAL_CMGETAPI( "X509CertInfoInit" ) 
	#define CAL_X509CertInfoInit  X509CertInfoInit
	#define CHK_X509CertInfoInit  TRUE
	#define EXP_X509CertInfoInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertInfoInit", (RTS_UINTPTR)X509CertInfoInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertInfoInit
	#define EXT_CmpX509CertX509CertInfoInit
	#define GET_CmpX509CertX509CertInfoInit  ERR_OK
	#define CAL_CmpX509CertX509CertInfoInit pICmpX509Cert->IX509CertInfoInit
	#define CHK_CmpX509CertX509CertInfoInit (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertInfoInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertInfoInit
	#define EXT_X509CertInfoInit
	#define GET_X509CertInfoInit(fl)  CAL_CMGETAPI( "X509CertInfoInit" ) 
	#define CAL_X509CertInfoInit pICmpX509Cert->IX509CertInfoInit
	#define CHK_X509CertInfoInit (pICmpX509Cert != NULL)
	#define EXP_X509CertInfoInit  CAL_CMEXPAPI( "X509CertInfoInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertInfoInit  PFX509CERTINFOINIT pfX509CertInfoInit;
	#define EXT_X509CertInfoInit  extern PFX509CERTINFOINIT pfX509CertInfoInit;
	#define GET_X509CertInfoInit(fl)  s_pfCMGetAPI2( "X509CertInfoInit", (RTS_VOID_FCTPTR *)&pfX509CertInfoInit, (fl), 0, 0)
	#define CAL_X509CertInfoInit  pfX509CertInfoInit
	#define CHK_X509CertInfoInit  (pfX509CertInfoInit != NULL)
	#define EXP_X509CertInfoInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertInfoInit", (RTS_UINTPTR)X509CertInfoInit, 0, 0) 
#endif




/**
 * <description>Cleanup an RtsX509CertInfo structure. All allocated memory used by this strucutre will be release.</description>
 * <param name="hCert" type="IN">Pointer to the object to clean up.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertInfoExit(RtsX509CertInfo* pCertInfo);
typedef RTS_RESULT (CDECL * PFX509CERTINFOEXIT) (RtsX509CertInfo* pCertInfo);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTINFOEXIT_NOTIMPLEMENTED)
	#define USE_X509CertInfoExit
	#define EXT_X509CertInfoExit
	#define GET_X509CertInfoExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertInfoExit(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertInfoExit  FALSE
	#define EXP_X509CertInfoExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertInfoExit
	#define EXT_X509CertInfoExit
	#define GET_X509CertInfoExit(fl)  CAL_CMGETAPI( "X509CertInfoExit" ) 
	#define CAL_X509CertInfoExit  X509CertInfoExit
	#define CHK_X509CertInfoExit  TRUE
	#define EXP_X509CertInfoExit  CAL_CMEXPAPI( "X509CertInfoExit" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertInfoExit
	#define EXT_X509CertInfoExit
	#define GET_X509CertInfoExit(fl)  CAL_CMGETAPI( "X509CertInfoExit" ) 
	#define CAL_X509CertInfoExit  X509CertInfoExit
	#define CHK_X509CertInfoExit  TRUE
	#define EXP_X509CertInfoExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertInfoExit", (RTS_UINTPTR)X509CertInfoExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertInfoExit
	#define EXT_CmpX509CertX509CertInfoExit
	#define GET_CmpX509CertX509CertInfoExit  ERR_OK
	#define CAL_CmpX509CertX509CertInfoExit pICmpX509Cert->IX509CertInfoExit
	#define CHK_CmpX509CertX509CertInfoExit (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertInfoExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertInfoExit
	#define EXT_X509CertInfoExit
	#define GET_X509CertInfoExit(fl)  CAL_CMGETAPI( "X509CertInfoExit" ) 
	#define CAL_X509CertInfoExit pICmpX509Cert->IX509CertInfoExit
	#define CHK_X509CertInfoExit (pICmpX509Cert != NULL)
	#define EXP_X509CertInfoExit  CAL_CMEXPAPI( "X509CertInfoExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertInfoExit  PFX509CERTINFOEXIT pfX509CertInfoExit;
	#define EXT_X509CertInfoExit  extern PFX509CERTINFOEXIT pfX509CertInfoExit;
	#define GET_X509CertInfoExit(fl)  s_pfCMGetAPI2( "X509CertInfoExit", (RTS_VOID_FCTPTR *)&pfX509CertInfoExit, (fl), 0, 0)
	#define CAL_X509CertInfoExit  pfX509CertInfoExit
	#define CHK_X509CertInfoExit  (pfX509CertInfoExit != NULL)
	#define EXP_X509CertInfoExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertInfoExit", (RTS_UINTPTR)X509CertInfoExit, 0, 0) 
#endif





/* --------------------------------------------------------------------------- */
/*                   Organizing the certificate store                          */
/*                                                                             */
/* The following functions are needed to manage the certificate store. If this */
/* management is not implemented by the component the functions should return  */
/* invalid handles and ERR_NOTIMPLEMENTED                                      */
/* --------------------------------------------------------------------------- */


/**
 * <description>Get the first registerd backend of the certificate store.</description> 
 * <param name="hCertStore" type="IN">Handle to the certificate store</param>
 * <param name="pInfo" type="OUT">Pointer to backend info. Filled up by the function.</param>
 * <param name="pResult" type="OUT">Result of the operation.</param>
 * <result>Handle of the first backend.</result>
 */
RTS_HANDLE CDECL X509CertStoreGetFirstBackend(RTS_HANDLE hCertStore, RtsCertBackendInfo* pInfo, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTSTOREGETFIRSTBACKEND) (RTS_HANDLE hCertStore, RtsCertBackendInfo* pInfo, RTS_RESULT* pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREGETFIRSTBACKEND_NOTIMPLEMENTED)
	#define USE_X509CertStoreGetFirstBackend
	#define EXT_X509CertStoreGetFirstBackend
	#define GET_X509CertStoreGetFirstBackend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreGetFirstBackend(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertStoreGetFirstBackend  FALSE
	#define EXP_X509CertStoreGetFirstBackend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreGetFirstBackend
	#define EXT_X509CertStoreGetFirstBackend
	#define GET_X509CertStoreGetFirstBackend(fl)  CAL_CMGETAPI( "X509CertStoreGetFirstBackend" ) 
	#define CAL_X509CertStoreGetFirstBackend  X509CertStoreGetFirstBackend
	#define CHK_X509CertStoreGetFirstBackend  TRUE
	#define EXP_X509CertStoreGetFirstBackend  CAL_CMEXPAPI( "X509CertStoreGetFirstBackend" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreGetFirstBackend
	#define EXT_X509CertStoreGetFirstBackend
	#define GET_X509CertStoreGetFirstBackend(fl)  CAL_CMGETAPI( "X509CertStoreGetFirstBackend" ) 
	#define CAL_X509CertStoreGetFirstBackend  X509CertStoreGetFirstBackend
	#define CHK_X509CertStoreGetFirstBackend  TRUE
	#define EXP_X509CertStoreGetFirstBackend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetFirstBackend", (RTS_UINTPTR)X509CertStoreGetFirstBackend, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreGetFirstBackend
	#define EXT_CmpX509CertX509CertStoreGetFirstBackend
	#define GET_CmpX509CertX509CertStoreGetFirstBackend  ERR_OK
	#define CAL_CmpX509CertX509CertStoreGetFirstBackend pICmpX509Cert->IX509CertStoreGetFirstBackend
	#define CHK_CmpX509CertX509CertStoreGetFirstBackend (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreGetFirstBackend  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreGetFirstBackend
	#define EXT_X509CertStoreGetFirstBackend
	#define GET_X509CertStoreGetFirstBackend(fl)  CAL_CMGETAPI( "X509CertStoreGetFirstBackend" ) 
	#define CAL_X509CertStoreGetFirstBackend pICmpX509Cert->IX509CertStoreGetFirstBackend
	#define CHK_X509CertStoreGetFirstBackend (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreGetFirstBackend  CAL_CMEXPAPI( "X509CertStoreGetFirstBackend" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreGetFirstBackend  PFX509CERTSTOREGETFIRSTBACKEND pfX509CertStoreGetFirstBackend;
	#define EXT_X509CertStoreGetFirstBackend  extern PFX509CERTSTOREGETFIRSTBACKEND pfX509CertStoreGetFirstBackend;
	#define GET_X509CertStoreGetFirstBackend(fl)  s_pfCMGetAPI2( "X509CertStoreGetFirstBackend", (RTS_VOID_FCTPTR *)&pfX509CertStoreGetFirstBackend, (fl), 0, 0)
	#define CAL_X509CertStoreGetFirstBackend  pfX509CertStoreGetFirstBackend
	#define CHK_X509CertStoreGetFirstBackend  (pfX509CertStoreGetFirstBackend != NULL)
	#define EXP_X509CertStoreGetFirstBackend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetFirstBackend", (RTS_UINTPTR)X509CertStoreGetFirstBackend, 0, 0) 
#endif




/**
 * <description>Get the next registerd backend of the certificate store.</description>
 * <param name="hCertStore" type="IN">Handle to the certificate store</param>
 * <param name="hBackend" type="IN">Handle to the preverios backend.</param>
 * <param name="pInfo" type="OUT">Pointer to backend info. Filled up by the function.</param>
 * <param name="pResult" type="OUT">Result of the operation.</param>
 * <result>Handle of the next Backend. RTS_INVALID_HANDLE if no more backend is available.</result>
 */
RTS_HANDLE CDECL X509CertStoreGetNextBackend(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsCertBackendInfo* pInfo, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTSTOREGETNEXTBACKEND) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsCertBackendInfo* pInfo, RTS_RESULT* pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREGETNEXTBACKEND_NOTIMPLEMENTED)
	#define USE_X509CertStoreGetNextBackend
	#define EXT_X509CertStoreGetNextBackend
	#define GET_X509CertStoreGetNextBackend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreGetNextBackend(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertStoreGetNextBackend  FALSE
	#define EXP_X509CertStoreGetNextBackend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreGetNextBackend
	#define EXT_X509CertStoreGetNextBackend
	#define GET_X509CertStoreGetNextBackend(fl)  CAL_CMGETAPI( "X509CertStoreGetNextBackend" ) 
	#define CAL_X509CertStoreGetNextBackend  X509CertStoreGetNextBackend
	#define CHK_X509CertStoreGetNextBackend  TRUE
	#define EXP_X509CertStoreGetNextBackend  CAL_CMEXPAPI( "X509CertStoreGetNextBackend" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreGetNextBackend
	#define EXT_X509CertStoreGetNextBackend
	#define GET_X509CertStoreGetNextBackend(fl)  CAL_CMGETAPI( "X509CertStoreGetNextBackend" ) 
	#define CAL_X509CertStoreGetNextBackend  X509CertStoreGetNextBackend
	#define CHK_X509CertStoreGetNextBackend  TRUE
	#define EXP_X509CertStoreGetNextBackend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetNextBackend", (RTS_UINTPTR)X509CertStoreGetNextBackend, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreGetNextBackend
	#define EXT_CmpX509CertX509CertStoreGetNextBackend
	#define GET_CmpX509CertX509CertStoreGetNextBackend  ERR_OK
	#define CAL_CmpX509CertX509CertStoreGetNextBackend pICmpX509Cert->IX509CertStoreGetNextBackend
	#define CHK_CmpX509CertX509CertStoreGetNextBackend (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreGetNextBackend  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreGetNextBackend
	#define EXT_X509CertStoreGetNextBackend
	#define GET_X509CertStoreGetNextBackend(fl)  CAL_CMGETAPI( "X509CertStoreGetNextBackend" ) 
	#define CAL_X509CertStoreGetNextBackend pICmpX509Cert->IX509CertStoreGetNextBackend
	#define CHK_X509CertStoreGetNextBackend (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreGetNextBackend  CAL_CMEXPAPI( "X509CertStoreGetNextBackend" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreGetNextBackend  PFX509CERTSTOREGETNEXTBACKEND pfX509CertStoreGetNextBackend;
	#define EXT_X509CertStoreGetNextBackend  extern PFX509CERTSTOREGETNEXTBACKEND pfX509CertStoreGetNextBackend;
	#define GET_X509CertStoreGetNextBackend(fl)  s_pfCMGetAPI2( "X509CertStoreGetNextBackend", (RTS_VOID_FCTPTR *)&pfX509CertStoreGetNextBackend, (fl), 0, 0)
	#define CAL_X509CertStoreGetNextBackend  pfX509CertStoreGetNextBackend
	#define CHK_X509CertStoreGetNextBackend  (pfX509CertStoreGetNextBackend != NULL)
	#define EXP_X509CertStoreGetNextBackend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetNextBackend", (RTS_UINTPTR)X509CertStoreGetNextBackend, 0, 0) 
#endif





/**
 * <description>The the information of a specific certificate backend.</description>
 * <param name="hCertStore" type="IN">Handle to the certificate store</param>
 * <param name="hBackend" type="IN">Handle to the preverios backend.</param>
 * <param name="pInfo" type="OUT">Pointer to backend info. Filled up by the function.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertStorGetBackendInfo(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsCertBackendInfo* pInfo);
typedef RTS_RESULT (CDECL * PFX509CERTSTORGETBACKENDINFO) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsCertBackendInfo* pInfo);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTORGETBACKENDINFO_NOTIMPLEMENTED)
	#define USE_X509CertStorGetBackendInfo
	#define EXT_X509CertStorGetBackendInfo
	#define GET_X509CertStorGetBackendInfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStorGetBackendInfo(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertStorGetBackendInfo  FALSE
	#define EXP_X509CertStorGetBackendInfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStorGetBackendInfo
	#define EXT_X509CertStorGetBackendInfo
	#define GET_X509CertStorGetBackendInfo(fl)  CAL_CMGETAPI( "X509CertStorGetBackendInfo" ) 
	#define CAL_X509CertStorGetBackendInfo  X509CertStorGetBackendInfo
	#define CHK_X509CertStorGetBackendInfo  TRUE
	#define EXP_X509CertStorGetBackendInfo  CAL_CMEXPAPI( "X509CertStorGetBackendInfo" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStorGetBackendInfo
	#define EXT_X509CertStorGetBackendInfo
	#define GET_X509CertStorGetBackendInfo(fl)  CAL_CMGETAPI( "X509CertStorGetBackendInfo" ) 
	#define CAL_X509CertStorGetBackendInfo  X509CertStorGetBackendInfo
	#define CHK_X509CertStorGetBackendInfo  TRUE
	#define EXP_X509CertStorGetBackendInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStorGetBackendInfo", (RTS_UINTPTR)X509CertStorGetBackendInfo, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStorGetBackendInfo
	#define EXT_CmpX509CertX509CertStorGetBackendInfo
	#define GET_CmpX509CertX509CertStorGetBackendInfo  ERR_OK
	#define CAL_CmpX509CertX509CertStorGetBackendInfo pICmpX509Cert->IX509CertStorGetBackendInfo
	#define CHK_CmpX509CertX509CertStorGetBackendInfo (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStorGetBackendInfo  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStorGetBackendInfo
	#define EXT_X509CertStorGetBackendInfo
	#define GET_X509CertStorGetBackendInfo(fl)  CAL_CMGETAPI( "X509CertStorGetBackendInfo" ) 
	#define CAL_X509CertStorGetBackendInfo pICmpX509Cert->IX509CertStorGetBackendInfo
	#define CHK_X509CertStorGetBackendInfo (pICmpX509Cert != NULL)
	#define EXP_X509CertStorGetBackendInfo  CAL_CMEXPAPI( "X509CertStorGetBackendInfo" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStorGetBackendInfo  PFX509CERTSTORGETBACKENDINFO pfX509CertStorGetBackendInfo;
	#define EXT_X509CertStorGetBackendInfo  extern PFX509CERTSTORGETBACKENDINFO pfX509CertStorGetBackendInfo;
	#define GET_X509CertStorGetBackendInfo(fl)  s_pfCMGetAPI2( "X509CertStorGetBackendInfo", (RTS_VOID_FCTPTR *)&pfX509CertStorGetBackendInfo, (fl), 0, 0)
	#define CAL_X509CertStorGetBackendInfo  pfX509CertStorGetBackendInfo
	#define CHK_X509CertStorGetBackendInfo  (pfX509CertStorGetBackendInfo != NULL)
	#define EXP_X509CertStorGetBackendInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStorGetBackendInfo", (RTS_UINTPTR)X509CertStorGetBackendInfo, 0, 0) 
#endif




/**
 * <description>Create a new self signed certificate. A private key is generated and automatically stored.</description>
 * <param name="hCertStore" type="IN">Handle to the certificate store</param>
 * <param name="hBackend" type="IN">Handle to the backend where to store the generated certificate. RTS_INVALID_HANDLE should be used if no backend is specified.</param>
 * <param name="pCertInfo" type="IN">Information of the certificate to be generated.</param>
 * <param name="ui32KeySize" type="IN">Size in bits of the private key.</param>
 * <param name="pResult" type="OUT">Result of the operation.
 *      - ERR_OK: Everything went fine.
 *      - ERR_INVALID_HANDLE: Some input handle is invalid.
 *      - ERR_PARAMETER: Some input parameter is invalid.
 *      - ERR_OVERFLOW: The timestamps of the certificate would overflow.
 *      - ERR_NOMEMORY: Not enough memory to perform this operation.
 *      - ERR_FAILED: Creation of self signed certificate failed. failed</param>
 * <result>Handle to the created selfsigned certificate</result>
 */
RTS_HANDLE CDECL X509CertCreateSelfSigned(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, RTS_UI32 ui32KeySize, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTCREATESELFSIGNED) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, RTS_UI32 ui32KeySize, RTS_RESULT *pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCREATESELFSIGNED_NOTIMPLEMENTED)
	#define USE_X509CertCreateSelfSigned
	#define EXT_X509CertCreateSelfSigned
	#define GET_X509CertCreateSelfSigned(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertCreateSelfSigned(p0,p1,p2,p3,p4)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertCreateSelfSigned  FALSE
	#define EXP_X509CertCreateSelfSigned  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertCreateSelfSigned
	#define EXT_X509CertCreateSelfSigned
	#define GET_X509CertCreateSelfSigned(fl)  CAL_CMGETAPI( "X509CertCreateSelfSigned" ) 
	#define CAL_X509CertCreateSelfSigned  X509CertCreateSelfSigned
	#define CHK_X509CertCreateSelfSigned  TRUE
	#define EXP_X509CertCreateSelfSigned  CAL_CMEXPAPI( "X509CertCreateSelfSigned" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertCreateSelfSigned
	#define EXT_X509CertCreateSelfSigned
	#define GET_X509CertCreateSelfSigned(fl)  CAL_CMGETAPI( "X509CertCreateSelfSigned" ) 
	#define CAL_X509CertCreateSelfSigned  X509CertCreateSelfSigned
	#define CHK_X509CertCreateSelfSigned  TRUE
	#define EXP_X509CertCreateSelfSigned  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCreateSelfSigned", (RTS_UINTPTR)X509CertCreateSelfSigned, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertCreateSelfSigned
	#define EXT_CmpX509CertX509CertCreateSelfSigned
	#define GET_CmpX509CertX509CertCreateSelfSigned  ERR_OK
	#define CAL_CmpX509CertX509CertCreateSelfSigned pICmpX509Cert->IX509CertCreateSelfSigned
	#define CHK_CmpX509CertX509CertCreateSelfSigned (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertCreateSelfSigned  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertCreateSelfSigned
	#define EXT_X509CertCreateSelfSigned
	#define GET_X509CertCreateSelfSigned(fl)  CAL_CMGETAPI( "X509CertCreateSelfSigned" ) 
	#define CAL_X509CertCreateSelfSigned pICmpX509Cert->IX509CertCreateSelfSigned
	#define CHK_X509CertCreateSelfSigned (pICmpX509Cert != NULL)
	#define EXP_X509CertCreateSelfSigned  CAL_CMEXPAPI( "X509CertCreateSelfSigned" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertCreateSelfSigned  PFX509CERTCREATESELFSIGNED pfX509CertCreateSelfSigned;
	#define EXT_X509CertCreateSelfSigned  extern PFX509CERTCREATESELFSIGNED pfX509CertCreateSelfSigned;
	#define GET_X509CertCreateSelfSigned(fl)  s_pfCMGetAPI2( "X509CertCreateSelfSigned", (RTS_VOID_FCTPTR *)&pfX509CertCreateSelfSigned, (fl), 0, 0)
	#define CAL_X509CertCreateSelfSigned  pfX509CertCreateSelfSigned
	#define CHK_X509CertCreateSelfSigned  (pfX509CertCreateSelfSigned != NULL)
	#define EXP_X509CertCreateSelfSigned  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCreateSelfSigned", (RTS_UINTPTR)X509CertCreateSelfSigned, 0, 0) 
#endif




/**
 * <description>Create a new self signed certificate, asynchronously. A private key is generated and automatically stored.</description>
 * <param name="hCertStore" type="IN">Handle to the certificate store</param>
 * <param name="hBackend" type="IN">Handle to the backend where to store the generated certificate. RTS_INVALID_HANDLE should be used if no backend is specified.</param>
 * <param name="pCertInfo" type="IN">Information of the certificate to be generated.</param>
 * <param name="ui32KeySize" type="IN">Size in bits of the private key.</param>
 * <param name="ui32KeySize" type="IN">Size in bits of the private key.</param>
 * <param name="pCallbackParam" type="IN">Parameter used when the callback is called when the operation has finished.</param>
 * <param name="pfCallback" type="IN">Callback function that is called at the end of the operation</param>
 * <result>Result of the operation</result>
 */
RTS_RESULT CDECL X509CertCreateSelfSignedAsync(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, RTS_UI32 ui32KeySize, void* pCallbackParam, PFX509ASYNCCALLBACK pfCallback);
typedef RTS_RESULT (CDECL * PFX509CERTCREATESELFSIGNEDASYNC) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, RTS_UI32 ui32KeySize, void* pCallbackParam, PFX509ASYNCCALLBACK pfCallback);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCREATESELFSIGNEDASYNC_NOTIMPLEMENTED)
	#define USE_X509CertCreateSelfSignedAsync
	#define EXT_X509CertCreateSelfSignedAsync
	#define GET_X509CertCreateSelfSignedAsync(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertCreateSelfSignedAsync(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertCreateSelfSignedAsync  FALSE
	#define EXP_X509CertCreateSelfSignedAsync  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertCreateSelfSignedAsync
	#define EXT_X509CertCreateSelfSignedAsync
	#define GET_X509CertCreateSelfSignedAsync(fl)  CAL_CMGETAPI( "X509CertCreateSelfSignedAsync" ) 
	#define CAL_X509CertCreateSelfSignedAsync  X509CertCreateSelfSignedAsync
	#define CHK_X509CertCreateSelfSignedAsync  TRUE
	#define EXP_X509CertCreateSelfSignedAsync  CAL_CMEXPAPI( "X509CertCreateSelfSignedAsync" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertCreateSelfSignedAsync
	#define EXT_X509CertCreateSelfSignedAsync
	#define GET_X509CertCreateSelfSignedAsync(fl)  CAL_CMGETAPI( "X509CertCreateSelfSignedAsync" ) 
	#define CAL_X509CertCreateSelfSignedAsync  X509CertCreateSelfSignedAsync
	#define CHK_X509CertCreateSelfSignedAsync  TRUE
	#define EXP_X509CertCreateSelfSignedAsync  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCreateSelfSignedAsync", (RTS_UINTPTR)X509CertCreateSelfSignedAsync, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertCreateSelfSignedAsync
	#define EXT_CmpX509CertX509CertCreateSelfSignedAsync
	#define GET_CmpX509CertX509CertCreateSelfSignedAsync  ERR_OK
	#define CAL_CmpX509CertX509CertCreateSelfSignedAsync pICmpX509Cert->IX509CertCreateSelfSignedAsync
	#define CHK_CmpX509CertX509CertCreateSelfSignedAsync (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertCreateSelfSignedAsync  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertCreateSelfSignedAsync
	#define EXT_X509CertCreateSelfSignedAsync
	#define GET_X509CertCreateSelfSignedAsync(fl)  CAL_CMGETAPI( "X509CertCreateSelfSignedAsync" ) 
	#define CAL_X509CertCreateSelfSignedAsync pICmpX509Cert->IX509CertCreateSelfSignedAsync
	#define CHK_X509CertCreateSelfSignedAsync (pICmpX509Cert != NULL)
	#define EXP_X509CertCreateSelfSignedAsync  CAL_CMEXPAPI( "X509CertCreateSelfSignedAsync" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertCreateSelfSignedAsync  PFX509CERTCREATESELFSIGNEDASYNC pfX509CertCreateSelfSignedAsync;
	#define EXT_X509CertCreateSelfSignedAsync  extern PFX509CERTCREATESELFSIGNEDASYNC pfX509CertCreateSelfSignedAsync;
	#define GET_X509CertCreateSelfSignedAsync(fl)  s_pfCMGetAPI2( "X509CertCreateSelfSignedAsync", (RTS_VOID_FCTPTR *)&pfX509CertCreateSelfSignedAsync, (fl), 0, 0)
	#define CAL_X509CertCreateSelfSignedAsync  pfX509CertCreateSelfSignedAsync
	#define CHK_X509CertCreateSelfSignedAsync  (pfX509CertCreateSelfSignedAsync != NULL)
	#define EXP_X509CertCreateSelfSignedAsync  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCreateSelfSignedAsync", (RTS_UINTPTR)X509CertCreateSelfSignedAsync, 0, 0) 
#endif




/**
 * <description>Create a new certificate request. A new private key will be 
 * generated and automatically stored in the key store. The certificate request 
 * can be sent to an offical CA to get a signed certificate.</description>
 * <param name="hCertStore" type="IN">Handle to the certificate store</param>
 * <param name="hBackend" type="IN">Handle to the backend where to store the generated certificate. RTS_INVALID_HANDLE should be used if no backend is specified.</param>
 * <param name="pCertInfo" type="IN">Information of the certificate to be generated.</param>
 * <param name="ui32KeySize" type="IN">Size in bits of the private key.</param>
 * <param name="encoding" type="IN">Encoding of the generated CSR.</param>
 * <param name="pCSR" type="OUT">The signing request in binary form.</param>
 * <result>Result of the operation</result>
 */
RTS_RESULT CDECL X509CertCreateCSR(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, RTS_UI32 ui32KeySize, RTSCERTENCODING encoding, RtsByteString* pCSR);
typedef RTS_RESULT (CDECL * PFX509CERTCREATECSR) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, RTS_UI32 ui32KeySize, RTSCERTENCODING encoding, RtsByteString* pCSR);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCREATECSR_NOTIMPLEMENTED)
	#define USE_X509CertCreateCSR
	#define EXT_X509CertCreateCSR
	#define GET_X509CertCreateCSR(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertCreateCSR(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertCreateCSR  FALSE
	#define EXP_X509CertCreateCSR  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertCreateCSR
	#define EXT_X509CertCreateCSR
	#define GET_X509CertCreateCSR(fl)  CAL_CMGETAPI( "X509CertCreateCSR" ) 
	#define CAL_X509CertCreateCSR  X509CertCreateCSR
	#define CHK_X509CertCreateCSR  TRUE
	#define EXP_X509CertCreateCSR  CAL_CMEXPAPI( "X509CertCreateCSR" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertCreateCSR
	#define EXT_X509CertCreateCSR
	#define GET_X509CertCreateCSR(fl)  CAL_CMGETAPI( "X509CertCreateCSR" ) 
	#define CAL_X509CertCreateCSR  X509CertCreateCSR
	#define CHK_X509CertCreateCSR  TRUE
	#define EXP_X509CertCreateCSR  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCreateCSR", (RTS_UINTPTR)X509CertCreateCSR, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertCreateCSR
	#define EXT_CmpX509CertX509CertCreateCSR
	#define GET_CmpX509CertX509CertCreateCSR  ERR_OK
	#define CAL_CmpX509CertX509CertCreateCSR pICmpX509Cert->IX509CertCreateCSR
	#define CHK_CmpX509CertX509CertCreateCSR (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertCreateCSR  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertCreateCSR
	#define EXT_X509CertCreateCSR
	#define GET_X509CertCreateCSR(fl)  CAL_CMGETAPI( "X509CertCreateCSR" ) 
	#define CAL_X509CertCreateCSR pICmpX509Cert->IX509CertCreateCSR
	#define CHK_X509CertCreateCSR (pICmpX509Cert != NULL)
	#define EXP_X509CertCreateCSR  CAL_CMEXPAPI( "X509CertCreateCSR" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertCreateCSR  PFX509CERTCREATECSR pfX509CertCreateCSR;
	#define EXT_X509CertCreateCSR  extern PFX509CERTCREATECSR pfX509CertCreateCSR;
	#define GET_X509CertCreateCSR(fl)  s_pfCMGetAPI2( "X509CertCreateCSR", (RTS_VOID_FCTPTR *)&pfX509CertCreateCSR, (fl), 0, 0)
	#define CAL_X509CertCreateCSR  pfX509CertCreateCSR
	#define CHK_X509CertCreateCSR  (pfX509CertCreateCSR != NULL)
	#define EXP_X509CertCreateCSR  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCreateCSR", (RTS_UINTPTR)X509CertCreateCSR, 0, 0) 
#endif




/**
 * <description>Create a new certificate request. A new private key will be 
 * generated and automatically stored in the key store. The certificate request 
 * can be sent to an offical CA to get a signed certificate.</description>
 * <param name="hCertStore" type="IN">Handle to the certificate store</param>
 * <param name="hBackend" type="IN">Handle to the backend where to store the generated certificate. RTS_INVALID_HANDLE should be used if no backend is specified.</param>
 * <param name="pCertInfo" type="IN">Information of the certificate to be generated.</param>
 * <param name="cmpID" type="IN">Component ID of the component for which the CSR is created.</param>
 * <param name="ui32KeySize" type="IN">Size in bits of the private key.</param>
 * <param name="encoding" type="IN">Encoding of the generated CSR.</param>
 * <param name="pCSR" type="OUT">The signing request in binary form.</param>
 * <param name="pCallbackParam" type="IN">Parameter used when the callback is called when the operation has finished.</param>
 * <param name="pfCallback" type="IN">Callback function that is called at the end of the operation</param>
 * <result>Result of the operation</result>
 */
RTS_RESULT CDECL X509CertCreateCSRAsync(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, CMPID cmpID, RTS_UI32 ui32KeySize, RTSCERTENCODING encoding, RtsByteString* pCSR, void* pCallbackParam, PFX509ASYNCCALLBACK pfCallback);
typedef RTS_RESULT (CDECL * PFX509CERTCREATECSRASYNC) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, CMPID cmpID, RTS_UI32 ui32KeySize, RTSCERTENCODING encoding, RtsByteString* pCSR, void* pCallbackParam, PFX509ASYNCCALLBACK pfCallback);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTCREATECSRASYNC_NOTIMPLEMENTED)
	#define USE_X509CertCreateCSRAsync
	#define EXT_X509CertCreateCSRAsync
	#define GET_X509CertCreateCSRAsync(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertCreateCSRAsync(p0,p1,p2,p3,p4,p5,p6,p7,p8)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertCreateCSRAsync  FALSE
	#define EXP_X509CertCreateCSRAsync  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertCreateCSRAsync
	#define EXT_X509CertCreateCSRAsync
	#define GET_X509CertCreateCSRAsync(fl)  CAL_CMGETAPI( "X509CertCreateCSRAsync" ) 
	#define CAL_X509CertCreateCSRAsync  X509CertCreateCSRAsync
	#define CHK_X509CertCreateCSRAsync  TRUE
	#define EXP_X509CertCreateCSRAsync  CAL_CMEXPAPI( "X509CertCreateCSRAsync" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertCreateCSRAsync
	#define EXT_X509CertCreateCSRAsync
	#define GET_X509CertCreateCSRAsync(fl)  CAL_CMGETAPI( "X509CertCreateCSRAsync" ) 
	#define CAL_X509CertCreateCSRAsync  X509CertCreateCSRAsync
	#define CHK_X509CertCreateCSRAsync  TRUE
	#define EXP_X509CertCreateCSRAsync  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCreateCSRAsync", (RTS_UINTPTR)X509CertCreateCSRAsync, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertCreateCSRAsync
	#define EXT_CmpX509CertX509CertCreateCSRAsync
	#define GET_CmpX509CertX509CertCreateCSRAsync  ERR_OK
	#define CAL_CmpX509CertX509CertCreateCSRAsync pICmpX509Cert->IX509CertCreateCSRAsync
	#define CHK_CmpX509CertX509CertCreateCSRAsync (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertCreateCSRAsync  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertCreateCSRAsync
	#define EXT_X509CertCreateCSRAsync
	#define GET_X509CertCreateCSRAsync(fl)  CAL_CMGETAPI( "X509CertCreateCSRAsync" ) 
	#define CAL_X509CertCreateCSRAsync pICmpX509Cert->IX509CertCreateCSRAsync
	#define CHK_X509CertCreateCSRAsync (pICmpX509Cert != NULL)
	#define EXP_X509CertCreateCSRAsync  CAL_CMEXPAPI( "X509CertCreateCSRAsync" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertCreateCSRAsync  PFX509CERTCREATECSRASYNC pfX509CertCreateCSRAsync;
	#define EXT_X509CertCreateCSRAsync  extern PFX509CERTCREATECSRASYNC pfX509CertCreateCSRAsync;
	#define GET_X509CertCreateCSRAsync(fl)  s_pfCMGetAPI2( "X509CertCreateCSRAsync", (RTS_VOID_FCTPTR *)&pfX509CertCreateCSRAsync, (fl), 0, 0)
	#define CAL_X509CertCreateCSRAsync  pfX509CertCreateCSRAsync
	#define CHK_X509CertCreateCSRAsync  (pfX509CertCreateCSRAsync != NULL)
	#define EXP_X509CertCreateCSRAsync  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertCreateCSRAsync", (RTS_UINTPTR)X509CertCreateCSRAsync, 0, 0) 
#endif




/**
 * <description>Add the given certificate to the Certificate store</description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <param name="hBackend" type="IN">Handle to the backend where to store the generated certificate. RTS_INVALID_HANDLE should be used if no backend is specified.</param>
 * <param name="hCert" type="IN">Handle of the certificate to be imported.</param>
 * <param name="trustLevel" type="IN">The category where to store the certificate. Note: RtsTrustLevel_Own ist not selectable.</param>
 * <param name="pResult">Result of the operation.</param>
 * <result>Handle to the imported certificate.</result>
 */
RTS_RESULT CDECL X509CertStoreAddCert(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTS_HANDLE hCert, RTSCERTTRUSTLEVEL trustLevel);
typedef RTS_RESULT (CDECL * PFX509CERTSTOREADDCERT) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTS_HANDLE hCert, RTSCERTTRUSTLEVEL trustLevel);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREADDCERT_NOTIMPLEMENTED)
	#define USE_X509CertStoreAddCert
	#define EXT_X509CertStoreAddCert
	#define GET_X509CertStoreAddCert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreAddCert(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertStoreAddCert  FALSE
	#define EXP_X509CertStoreAddCert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreAddCert
	#define EXT_X509CertStoreAddCert
	#define GET_X509CertStoreAddCert(fl)  CAL_CMGETAPI( "X509CertStoreAddCert" ) 
	#define CAL_X509CertStoreAddCert  X509CertStoreAddCert
	#define CHK_X509CertStoreAddCert  TRUE
	#define EXP_X509CertStoreAddCert  CAL_CMEXPAPI( "X509CertStoreAddCert" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreAddCert
	#define EXT_X509CertStoreAddCert
	#define GET_X509CertStoreAddCert(fl)  CAL_CMGETAPI( "X509CertStoreAddCert" ) 
	#define CAL_X509CertStoreAddCert  X509CertStoreAddCert
	#define CHK_X509CertStoreAddCert  TRUE
	#define EXP_X509CertStoreAddCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreAddCert", (RTS_UINTPTR)X509CertStoreAddCert, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreAddCert
	#define EXT_CmpX509CertX509CertStoreAddCert
	#define GET_CmpX509CertX509CertStoreAddCert  ERR_OK
	#define CAL_CmpX509CertX509CertStoreAddCert pICmpX509Cert->IX509CertStoreAddCert
	#define CHK_CmpX509CertX509CertStoreAddCert (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreAddCert  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreAddCert
	#define EXT_X509CertStoreAddCert
	#define GET_X509CertStoreAddCert(fl)  CAL_CMGETAPI( "X509CertStoreAddCert" ) 
	#define CAL_X509CertStoreAddCert pICmpX509Cert->IX509CertStoreAddCert
	#define CHK_X509CertStoreAddCert (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreAddCert  CAL_CMEXPAPI( "X509CertStoreAddCert" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreAddCert  PFX509CERTSTOREADDCERT pfX509CertStoreAddCert;
	#define EXT_X509CertStoreAddCert  extern PFX509CERTSTOREADDCERT pfX509CertStoreAddCert;
	#define GET_X509CertStoreAddCert(fl)  s_pfCMGetAPI2( "X509CertStoreAddCert", (RTS_VOID_FCTPTR *)&pfX509CertStoreAddCert, (fl), 0, 0)
	#define CAL_X509CertStoreAddCert  pfX509CertStoreAddCert
	#define CHK_X509CertStoreAddCert  (pfX509CertStoreAddCert != NULL)
	#define EXP_X509CertStoreAddCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreAddCert", (RTS_UINTPTR)X509CertStoreAddCert, 0, 0) 
#endif




/**
 * <description>Remove the certificate from the store.</description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <param name="hBackend" type="IN">Handle to the backend where to store the generated certificate. RTS_INVALID_HANDLE should be used if no backend is specified.</param>
 * <param name="hCert" type="IN">Handle of the certificate to remove.</param>
 * <param name="bRemovePrivateKey" type="IN">Select if the private key of the certificate should be deleted too. If the certificate is renewed the same private key the key shouldn't be deleted.</param>
 * <result>Result of the operation.</result>
 */
RTS_RESULT CDECL X509CertStoreRemoveCert(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTS_HANDLE hCert, RTS_BOOL bRemovePrivateKey);
typedef RTS_RESULT (CDECL * PFX509CERTSTOREREMOVECERT) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTS_HANDLE hCert, RTS_BOOL bRemovePrivateKey);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREREMOVECERT_NOTIMPLEMENTED)
	#define USE_X509CertStoreRemoveCert
	#define EXT_X509CertStoreRemoveCert
	#define GET_X509CertStoreRemoveCert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreRemoveCert(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertStoreRemoveCert  FALSE
	#define EXP_X509CertStoreRemoveCert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreRemoveCert
	#define EXT_X509CertStoreRemoveCert
	#define GET_X509CertStoreRemoveCert(fl)  CAL_CMGETAPI( "X509CertStoreRemoveCert" ) 
	#define CAL_X509CertStoreRemoveCert  X509CertStoreRemoveCert
	#define CHK_X509CertStoreRemoveCert  TRUE
	#define EXP_X509CertStoreRemoveCert  CAL_CMEXPAPI( "X509CertStoreRemoveCert" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreRemoveCert
	#define EXT_X509CertStoreRemoveCert
	#define GET_X509CertStoreRemoveCert(fl)  CAL_CMGETAPI( "X509CertStoreRemoveCert" ) 
	#define CAL_X509CertStoreRemoveCert  X509CertStoreRemoveCert
	#define CHK_X509CertStoreRemoveCert  TRUE
	#define EXP_X509CertStoreRemoveCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreRemoveCert", (RTS_UINTPTR)X509CertStoreRemoveCert, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreRemoveCert
	#define EXT_CmpX509CertX509CertStoreRemoveCert
	#define GET_CmpX509CertX509CertStoreRemoveCert  ERR_OK
	#define CAL_CmpX509CertX509CertStoreRemoveCert pICmpX509Cert->IX509CertStoreRemoveCert
	#define CHK_CmpX509CertX509CertStoreRemoveCert (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreRemoveCert  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreRemoveCert
	#define EXT_X509CertStoreRemoveCert
	#define GET_X509CertStoreRemoveCert(fl)  CAL_CMGETAPI( "X509CertStoreRemoveCert" ) 
	#define CAL_X509CertStoreRemoveCert pICmpX509Cert->IX509CertStoreRemoveCert
	#define CHK_X509CertStoreRemoveCert (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreRemoveCert  CAL_CMEXPAPI( "X509CertStoreRemoveCert" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreRemoveCert  PFX509CERTSTOREREMOVECERT pfX509CertStoreRemoveCert;
	#define EXT_X509CertStoreRemoveCert  extern PFX509CERTSTOREREMOVECERT pfX509CertStoreRemoveCert;
	#define GET_X509CertStoreRemoveCert(fl)  s_pfCMGetAPI2( "X509CertStoreRemoveCert", (RTS_VOID_FCTPTR *)&pfX509CertStoreRemoveCert, (fl), 0, 0)
	#define CAL_X509CertStoreRemoveCert  pfX509CertStoreRemoveCert
	#define CHK_X509CertStoreRemoveCert  (pfX509CertStoreRemoveCert != NULL)
	#define EXP_X509CertStoreRemoveCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreRemoveCert", (RTS_UINTPTR)X509CertStoreRemoveCert, 0, 0) 
#endif




/**
 * <description>Get the first certificate of the certificate store.</description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <param name="hBackend" type="IN">Pointer to a handle of the backend where to iterate. RTS_INVALID_HANDLE 
 * if all backends should be iterated. A valid handle if only a specific backend should be iterated.</param>
 * <param name="trustLevel" type="IN">Category of certificates to iterate.</param>
 * <param name="pResult" type="OUT">Result of the operation.</param>
 * <result>Handle to the first certificate of the given category.</param>
 */
RTS_HANDLE CDECL X509CertStoreGetFirstCert(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTSCERTTRUSTLEVEL trustLevel, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTSTOREGETFIRSTCERT) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTSCERTTRUSTLEVEL trustLevel, RTS_RESULT* pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREGETFIRSTCERT_NOTIMPLEMENTED)
	#define USE_X509CertStoreGetFirstCert
	#define EXT_X509CertStoreGetFirstCert
	#define GET_X509CertStoreGetFirstCert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreGetFirstCert(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertStoreGetFirstCert  FALSE
	#define EXP_X509CertStoreGetFirstCert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreGetFirstCert
	#define EXT_X509CertStoreGetFirstCert
	#define GET_X509CertStoreGetFirstCert(fl)  CAL_CMGETAPI( "X509CertStoreGetFirstCert" ) 
	#define CAL_X509CertStoreGetFirstCert  X509CertStoreGetFirstCert
	#define CHK_X509CertStoreGetFirstCert  TRUE
	#define EXP_X509CertStoreGetFirstCert  CAL_CMEXPAPI( "X509CertStoreGetFirstCert" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreGetFirstCert
	#define EXT_X509CertStoreGetFirstCert
	#define GET_X509CertStoreGetFirstCert(fl)  CAL_CMGETAPI( "X509CertStoreGetFirstCert" ) 
	#define CAL_X509CertStoreGetFirstCert  X509CertStoreGetFirstCert
	#define CHK_X509CertStoreGetFirstCert  TRUE
	#define EXP_X509CertStoreGetFirstCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetFirstCert", (RTS_UINTPTR)X509CertStoreGetFirstCert, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreGetFirstCert
	#define EXT_CmpX509CertX509CertStoreGetFirstCert
	#define GET_CmpX509CertX509CertStoreGetFirstCert  ERR_OK
	#define CAL_CmpX509CertX509CertStoreGetFirstCert pICmpX509Cert->IX509CertStoreGetFirstCert
	#define CHK_CmpX509CertX509CertStoreGetFirstCert (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreGetFirstCert  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreGetFirstCert
	#define EXT_X509CertStoreGetFirstCert
	#define GET_X509CertStoreGetFirstCert(fl)  CAL_CMGETAPI( "X509CertStoreGetFirstCert" ) 
	#define CAL_X509CertStoreGetFirstCert pICmpX509Cert->IX509CertStoreGetFirstCert
	#define CHK_X509CertStoreGetFirstCert (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreGetFirstCert  CAL_CMEXPAPI( "X509CertStoreGetFirstCert" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreGetFirstCert  PFX509CERTSTOREGETFIRSTCERT pfX509CertStoreGetFirstCert;
	#define EXT_X509CertStoreGetFirstCert  extern PFX509CERTSTOREGETFIRSTCERT pfX509CertStoreGetFirstCert;
	#define GET_X509CertStoreGetFirstCert(fl)  s_pfCMGetAPI2( "X509CertStoreGetFirstCert", (RTS_VOID_FCTPTR *)&pfX509CertStoreGetFirstCert, (fl), 0, 0)
	#define CAL_X509CertStoreGetFirstCert  pfX509CertStoreGetFirstCert
	#define CHK_X509CertStoreGetFirstCert  (pfX509CertStoreGetFirstCert != NULL)
	#define EXP_X509CertStoreGetFirstCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetFirstCert", (RTS_UINTPTR)X509CertStoreGetFirstCert, 0, 0) 
#endif




/**
 * <description>Get the next certificate of the certificate store. If all certificates
 * are read the function will retrun RTS_INVALID_HANDLE.</description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <param name="hBackend" type="IN">Pointer to a handle of the backend where to iterate. RTS_INVALID_HANDLE 
 * if all backends should be iterated. A valid handle if only a specific backend should be iterated.</param>
 * <param name="hCert" type="IN">Handle to the last certificate got by X509CertStoreGetFirst or X509CertStoreGetNext</param>
 * <param name="pResult" type="OUT">Result of the operation.</param>
 * <result>Handle to the next certificate of the given category.</param>
 */
RTS_HANDLE CDECL X509CertStoreGetNextCert(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTS_HANDLE hCert, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTSTOREGETNEXTCERT) (RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTS_HANDLE hCert, RTS_RESULT* pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREGETNEXTCERT_NOTIMPLEMENTED)
	#define USE_X509CertStoreGetNextCert
	#define EXT_X509CertStoreGetNextCert
	#define GET_X509CertStoreGetNextCert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreGetNextCert(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertStoreGetNextCert  FALSE
	#define EXP_X509CertStoreGetNextCert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreGetNextCert
	#define EXT_X509CertStoreGetNextCert
	#define GET_X509CertStoreGetNextCert(fl)  CAL_CMGETAPI( "X509CertStoreGetNextCert" ) 
	#define CAL_X509CertStoreGetNextCert  X509CertStoreGetNextCert
	#define CHK_X509CertStoreGetNextCert  TRUE
	#define EXP_X509CertStoreGetNextCert  CAL_CMEXPAPI( "X509CertStoreGetNextCert" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreGetNextCert
	#define EXT_X509CertStoreGetNextCert
	#define GET_X509CertStoreGetNextCert(fl)  CAL_CMGETAPI( "X509CertStoreGetNextCert" ) 
	#define CAL_X509CertStoreGetNextCert  X509CertStoreGetNextCert
	#define CHK_X509CertStoreGetNextCert  TRUE
	#define EXP_X509CertStoreGetNextCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetNextCert", (RTS_UINTPTR)X509CertStoreGetNextCert, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreGetNextCert
	#define EXT_CmpX509CertX509CertStoreGetNextCert
	#define GET_CmpX509CertX509CertStoreGetNextCert  ERR_OK
	#define CAL_CmpX509CertX509CertStoreGetNextCert pICmpX509Cert->IX509CertStoreGetNextCert
	#define CHK_CmpX509CertX509CertStoreGetNextCert (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreGetNextCert  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreGetNextCert
	#define EXT_X509CertStoreGetNextCert
	#define GET_X509CertStoreGetNextCert(fl)  CAL_CMGETAPI( "X509CertStoreGetNextCert" ) 
	#define CAL_X509CertStoreGetNextCert pICmpX509Cert->IX509CertStoreGetNextCert
	#define CHK_X509CertStoreGetNextCert (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreGetNextCert  CAL_CMEXPAPI( "X509CertStoreGetNextCert" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreGetNextCert  PFX509CERTSTOREGETNEXTCERT pfX509CertStoreGetNextCert;
	#define EXT_X509CertStoreGetNextCert  extern PFX509CERTSTOREGETNEXTCERT pfX509CertStoreGetNextCert;
	#define GET_X509CertStoreGetNextCert(fl)  s_pfCMGetAPI2( "X509CertStoreGetNextCert", (RTS_VOID_FCTPTR *)&pfX509CertStoreGetNextCert, (fl), 0, 0)
	#define CAL_X509CertStoreGetNextCert  pfX509CertStoreGetNextCert
	#define CHK_X509CertStoreGetNextCert  (pfX509CertStoreGetNextCert != NULL)
	#define EXP_X509CertStoreGetNextCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetNextCert", (RTS_UINTPTR)X509CertStoreGetNextCert, 0, 0) 
#endif




/**
 * <description>
 * Register your certificate at the certificate store. This has to be
 * done in order to allow the user the view the different configured certificates
 * and to generate missing certificates or to renew outdated certificates. Register the same
 * information you are using in the X509CertStoreSearch function.
 * </description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <param name="CmpID" type="IN">Component ID which registerd the certificate</param>
 * <param name="pCertInfo" type="IN">Certificate information to be registerd. Should match the information needed to use the Search function.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the entry</result>
 */
RTS_HANDLE CDECL X509CertStoreRegister(RTS_HANDLE hCertStore, CMPID cmpID, RtsX509CertInfo* pCertInfo, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTSTOREREGISTER) (RTS_HANDLE hCertStore, CMPID cmpID, RtsX509CertInfo* pCertInfo, RTS_RESULT* pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREREGISTER_NOTIMPLEMENTED)
	#define USE_X509CertStoreRegister
	#define EXT_X509CertStoreRegister
	#define GET_X509CertStoreRegister(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreRegister(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertStoreRegister  FALSE
	#define EXP_X509CertStoreRegister  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreRegister
	#define EXT_X509CertStoreRegister
	#define GET_X509CertStoreRegister(fl)  CAL_CMGETAPI( "X509CertStoreRegister" ) 
	#define CAL_X509CertStoreRegister  X509CertStoreRegister
	#define CHK_X509CertStoreRegister  TRUE
	#define EXP_X509CertStoreRegister  CAL_CMEXPAPI( "X509CertStoreRegister" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreRegister
	#define EXT_X509CertStoreRegister
	#define GET_X509CertStoreRegister(fl)  CAL_CMGETAPI( "X509CertStoreRegister" ) 
	#define CAL_X509CertStoreRegister  X509CertStoreRegister
	#define CHK_X509CertStoreRegister  TRUE
	#define EXP_X509CertStoreRegister  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreRegister", (RTS_UINTPTR)X509CertStoreRegister, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreRegister
	#define EXT_CmpX509CertX509CertStoreRegister
	#define GET_CmpX509CertX509CertStoreRegister  ERR_OK
	#define CAL_CmpX509CertX509CertStoreRegister pICmpX509Cert->IX509CertStoreRegister
	#define CHK_CmpX509CertX509CertStoreRegister (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreRegister  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreRegister
	#define EXT_X509CertStoreRegister
	#define GET_X509CertStoreRegister(fl)  CAL_CMGETAPI( "X509CertStoreRegister" ) 
	#define CAL_X509CertStoreRegister pICmpX509Cert->IX509CertStoreRegister
	#define CHK_X509CertStoreRegister (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreRegister  CAL_CMEXPAPI( "X509CertStoreRegister" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreRegister  PFX509CERTSTOREREGISTER pfX509CertStoreRegister;
	#define EXT_X509CertStoreRegister  extern PFX509CERTSTOREREGISTER pfX509CertStoreRegister;
	#define GET_X509CertStoreRegister(fl)  s_pfCMGetAPI2( "X509CertStoreRegister", (RTS_VOID_FCTPTR *)&pfX509CertStoreRegister, (fl), 0, 0)
	#define CAL_X509CertStoreRegister  pfX509CertStoreRegister
	#define CHK_X509CertStoreRegister  (pfX509CertStoreRegister != NULL)
	#define EXP_X509CertStoreRegister  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreRegister", (RTS_UINTPTR)X509CertStoreRegister, 0, 0) 
#endif




/**
 * <description>
 * </description>
 * <param name="hCertStore" type="IN">Handle of to certificate store</param>
 * <param name="hRegisteredComponent" type="IN">Handle to the entry retrieved by X509CertStoreRegister</param>
 * <result>Result of the operation</result>
 */
RTS_RESULT CDECL X509CertStoreUnregister(RTS_HANDLE hCertStore, RTS_HANDLE hRegisteredComponent);
typedef RTS_RESULT (CDECL * PFX509CERTSTOREUNREGISTER) (RTS_HANDLE hCertStore, RTS_HANDLE hRegisteredComponent);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREUNREGISTER_NOTIMPLEMENTED)
	#define USE_X509CertStoreUnregister
	#define EXT_X509CertStoreUnregister
	#define GET_X509CertStoreUnregister(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreUnregister(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_X509CertStoreUnregister  FALSE
	#define EXP_X509CertStoreUnregister  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreUnregister
	#define EXT_X509CertStoreUnregister
	#define GET_X509CertStoreUnregister(fl)  CAL_CMGETAPI( "X509CertStoreUnregister" ) 
	#define CAL_X509CertStoreUnregister  X509CertStoreUnregister
	#define CHK_X509CertStoreUnregister  TRUE
	#define EXP_X509CertStoreUnregister  CAL_CMEXPAPI( "X509CertStoreUnregister" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreUnregister
	#define EXT_X509CertStoreUnregister
	#define GET_X509CertStoreUnregister(fl)  CAL_CMGETAPI( "X509CertStoreUnregister" ) 
	#define CAL_X509CertStoreUnregister  X509CertStoreUnregister
	#define CHK_X509CertStoreUnregister  TRUE
	#define EXP_X509CertStoreUnregister  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreUnregister", (RTS_UINTPTR)X509CertStoreUnregister, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreUnregister
	#define EXT_CmpX509CertX509CertStoreUnregister
	#define GET_CmpX509CertX509CertStoreUnregister  ERR_OK
	#define CAL_CmpX509CertX509CertStoreUnregister pICmpX509Cert->IX509CertStoreUnregister
	#define CHK_CmpX509CertX509CertStoreUnregister (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreUnregister  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreUnregister
	#define EXT_X509CertStoreUnregister
	#define GET_X509CertStoreUnregister(fl)  CAL_CMGETAPI( "X509CertStoreUnregister" ) 
	#define CAL_X509CertStoreUnregister pICmpX509Cert->IX509CertStoreUnregister
	#define CHK_X509CertStoreUnregister (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreUnregister  CAL_CMEXPAPI( "X509CertStoreUnregister" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreUnregister  PFX509CERTSTOREUNREGISTER pfX509CertStoreUnregister;
	#define EXT_X509CertStoreUnregister  extern PFX509CERTSTOREUNREGISTER pfX509CertStoreUnregister;
	#define GET_X509CertStoreUnregister(fl)  s_pfCMGetAPI2( "X509CertStoreUnregister", (RTS_VOID_FCTPTR *)&pfX509CertStoreUnregister, (fl), 0, 0)
	#define CAL_X509CertStoreUnregister  pfX509CertStoreUnregister
	#define CHK_X509CertStoreUnregister  (pfX509CertStoreUnregister != NULL)
	#define EXP_X509CertStoreUnregister  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreUnregister", (RTS_UINTPTR)X509CertStoreUnregister, 0, 0) 
#endif




/**
 * <description>
 * Get the certificate available for a registered use case. If not certificate is available RTS_INVALID_HANDLE will be returned.
 * The following rules apply for selecting the certificate:
 * - Check if a certificate is configured by the user. (The thumbprint is pinned for component and index). Use this one if available, return error if not available.
 * - If no certificate is configured filter the available certificates by subject, key usage, extended key usage and a valid timestamp. Return error if list is empty.
 * - Split the available certificates by signed and self signed certificates. Prefer the signed certificates over the self signed certificates:
 * - Select the certificate with the longest period of validity.
 * - If some certificates have the same period of validity select the one with the strongest key.
 * </description>
 * <param name="hCertStore" type="IN">Handle of to certificate store.</param>
 * <param name="hUseCase" type="IN">Handle to the registered use case. Retrieved using X509CertStoreRegister.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the certificate. Use X509CertClose to close the certificat.</result>
 */
RTS_HANDLE CDECL X509CertStoreGetRegisteredCert(RTS_HANDLE hCertStore, RTS_HANDLE hUseCase, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFX509CERTSTOREGETREGISTEREDCERT) (RTS_HANDLE hCertStore, RTS_HANDLE hUseCase, RTS_RESULT* pResult);
#if defined(CMPX509CERT_NOTIMPLEMENTED) || defined(X509CERTSTOREGETREGISTEREDCERT_NOTIMPLEMENTED)
	#define USE_X509CertStoreGetRegisteredCert
	#define EXT_X509CertStoreGetRegisteredCert
	#define GET_X509CertStoreGetRegisteredCert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_X509CertStoreGetRegisteredCert(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_X509CertStoreGetRegisteredCert  FALSE
	#define EXP_X509CertStoreGetRegisteredCert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_X509CertStoreGetRegisteredCert
	#define EXT_X509CertStoreGetRegisteredCert
	#define GET_X509CertStoreGetRegisteredCert(fl)  CAL_CMGETAPI( "X509CertStoreGetRegisteredCert" ) 
	#define CAL_X509CertStoreGetRegisteredCert  X509CertStoreGetRegisteredCert
	#define CHK_X509CertStoreGetRegisteredCert  TRUE
	#define EXP_X509CertStoreGetRegisteredCert  CAL_CMEXPAPI( "X509CertStoreGetRegisteredCert" ) 
#elif defined(MIXED_LINK) && !defined(CMPX509CERT_EXTERNAL)
	#define USE_X509CertStoreGetRegisteredCert
	#define EXT_X509CertStoreGetRegisteredCert
	#define GET_X509CertStoreGetRegisteredCert(fl)  CAL_CMGETAPI( "X509CertStoreGetRegisteredCert" ) 
	#define CAL_X509CertStoreGetRegisteredCert  X509CertStoreGetRegisteredCert
	#define CHK_X509CertStoreGetRegisteredCert  TRUE
	#define EXP_X509CertStoreGetRegisteredCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetRegisteredCert", (RTS_UINTPTR)X509CertStoreGetRegisteredCert, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpX509CertX509CertStoreGetRegisteredCert
	#define EXT_CmpX509CertX509CertStoreGetRegisteredCert
	#define GET_CmpX509CertX509CertStoreGetRegisteredCert  ERR_OK
	#define CAL_CmpX509CertX509CertStoreGetRegisteredCert pICmpX509Cert->IX509CertStoreGetRegisteredCert
	#define CHK_CmpX509CertX509CertStoreGetRegisteredCert (pICmpX509Cert != NULL)
	#define EXP_CmpX509CertX509CertStoreGetRegisteredCert  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_X509CertStoreGetRegisteredCert
	#define EXT_X509CertStoreGetRegisteredCert
	#define GET_X509CertStoreGetRegisteredCert(fl)  CAL_CMGETAPI( "X509CertStoreGetRegisteredCert" ) 
	#define CAL_X509CertStoreGetRegisteredCert pICmpX509Cert->IX509CertStoreGetRegisteredCert
	#define CHK_X509CertStoreGetRegisteredCert (pICmpX509Cert != NULL)
	#define EXP_X509CertStoreGetRegisteredCert  CAL_CMEXPAPI( "X509CertStoreGetRegisteredCert" ) 
#else /* DYNAMIC_LINK */
	#define USE_X509CertStoreGetRegisteredCert  PFX509CERTSTOREGETREGISTEREDCERT pfX509CertStoreGetRegisteredCert;
	#define EXT_X509CertStoreGetRegisteredCert  extern PFX509CERTSTOREGETREGISTEREDCERT pfX509CertStoreGetRegisteredCert;
	#define GET_X509CertStoreGetRegisteredCert(fl)  s_pfCMGetAPI2( "X509CertStoreGetRegisteredCert", (RTS_VOID_FCTPTR *)&pfX509CertStoreGetRegisteredCert, (fl), 0, 0)
	#define CAL_X509CertStoreGetRegisteredCert  pfX509CertStoreGetRegisteredCert
	#define CHK_X509CertStoreGetRegisteredCert  (pfX509CertStoreGetRegisteredCert != NULL)
	#define EXP_X509CertStoreGetRegisteredCert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"X509CertStoreGetRegisteredCert", (RTS_UINTPTR)X509CertStoreGetRegisteredCert, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif






typedef struct
{
	IBase_C *pBase;
	PFX509CERTSTOREOPEN IX509CertStoreOpen;
 	PFX509CERTSTORECLOSE IX509CertStoreClose;
 	PFX509CERTVERIFY IX509CertVerify;
 	PFX509CERTSTORESEARCHGETFIRST IX509CertStoreSearchGetFirst;
 	PFX509CERTSTORESEARCHGETNEXT IX509CertStoreSearchGetNext;
 	PFX509PARSECERTIFICATE IX509ParseCertificate;
 	PFX509CERTGETCONTENT IX509CertGetContent;
 	PFX509CERTGETBINARY IX509CertGetBinary;
 	PFX509CERTGETPUBLICKEY IX509CertGetPublicKey;
 	PFX509CERTGETPRIVATEKEY IX509CertGetPrivateKey;
 	PFX509CERTGETTHUMBPRINT IX509CertGetThumbprint;
 	PFX509CERTISDATEVALID IX509CertIsDateValid;
 	PFX509CERTISSELFSIGNED IX509CertIsSelfSigned;
 	PFX509CERTHASEXTENDEDKEYUSAGE IX509CertHasExtendedKeyUsage;
 	PFX509CERTCOMPARE IX509CertCompare;
 	PFX509CERTKEYCLOSE IX509CertKeyClose;
 	PFX509CERTCLOSE IX509CertClose;
 	PFX509CERTCMSVERIFY IX509CertCmsVerify;
 	PFX509CERTCMSVERIFY2 IX509CertCmsVerify2;
 	PFX509CERTCMSDECRYPT IX509CertCmsDecrypt;
 	PFRTSOIDCREATE IRtsOIDCreate;
 	PFRTSOIDGETID IRtsOIDGetID;
 	PFRTSOIDGETNAME IRtsOIDGetName;
 	PFRTSOIDCLEAR IRtsOIDClear;
 	PFX509CERTINFOINIT IX509CertInfoInit;
 	PFX509CERTINFOEXIT IX509CertInfoExit;
 	PFX509CERTSTOREGETFIRSTBACKEND IX509CertStoreGetFirstBackend;
 	PFX509CERTSTOREGETNEXTBACKEND IX509CertStoreGetNextBackend;
 	PFX509CERTSTORGETBACKENDINFO IX509CertStorGetBackendInfo;
 	PFX509CERTCREATESELFSIGNED IX509CertCreateSelfSigned;
 	PFX509CERTCREATESELFSIGNEDASYNC IX509CertCreateSelfSignedAsync;
 	PFX509CERTCREATECSR IX509CertCreateCSR;
 	PFX509CERTCREATECSRASYNC IX509CertCreateCSRAsync;
 	PFX509CERTSTOREADDCERT IX509CertStoreAddCert;
 	PFX509CERTSTOREREMOVECERT IX509CertStoreRemoveCert;
 	PFX509CERTSTOREGETFIRSTCERT IX509CertStoreGetFirstCert;
 	PFX509CERTSTOREGETNEXTCERT IX509CertStoreGetNextCert;
 	PFX509CERTSTOREREGISTER IX509CertStoreRegister;
 	PFX509CERTSTOREUNREGISTER IX509CertStoreUnregister;
 	PFX509CERTSTOREGETREGISTEREDCERT IX509CertStoreGetRegisteredCert;
 } ICmpX509Cert_C;

#ifdef CPLUSPLUS
class ICmpX509Cert : public IBase
{
	public:
		virtual RTS_HANDLE CDECL IX509CertStoreOpen(CMPID componentID, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IX509CertStoreClose(RTS_HANDLE hCertStore) =0;
		virtual RTS_RESULT CDECL IX509CertVerify(RTS_HANDLE hCertStore, RTS_HANDLE hCert) =0;
		virtual RTS_HANDLE CDECL IX509CertStoreSearchGetFirst(RTS_HANDLE hCertStore, RtsX509CertFilter *pFilters, RTS_UI32 ui32NumOfFilters, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IX509CertStoreSearchGetNext(RTS_HANDLE hCertStore, RTS_HANDLE hCert, RtsX509CertFilter *pFilters, RTS_UI32 ui32NumOfFilters, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IX509ParseCertificate(RtsByteString* pCert, RTSCERTENCODING encoding, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IX509CertGetContent(RTS_HANDLE hCert, RtsX509CertInfo* pCertContent) =0;
		virtual RTS_RESULT CDECL IX509CertGetBinary(RTS_HANDLE hCert, RTSCERTENCODING encoding, RtsByteString* pCert) =0;
		virtual RTS_RESULT CDECL IX509CertGetPublicKey(RTS_HANDLE hCert, RtsCryptoKey* pPublicKey) =0;
		virtual RTS_RESULT CDECL IX509CertGetPrivateKey(RTS_HANDLE hCertStore, RTS_HANDLE hCert, RtsCryptoKey* pPrivateKey) =0;
		virtual RTS_RESULT CDECL IX509CertGetThumbprint(RTS_HANDLE hCert, RTSCRYPTOID* hashAlgorithm, RtsByteString* pThumbprint) =0;
		virtual RTS_BOOL CDECL IX509CertIsDateValid(RTS_HANDLE hCert, RTS_RESULT *pResult) =0;
		virtual RTS_BOOL CDECL IX509CertIsSelfSigned(RTS_HANDLE hCert, RTS_RESULT *pResult) =0;
		virtual RTS_BOOL CDECL IX509CertHasExtendedKeyUsage(RTS_HANDLE hCert, RTS_UI32 numOfExKeyUsages, RtsOID *pExKeyUsages, RTS_RESULT *pResult) =0;
		virtual RTS_BOOL CDECL IX509CertCompare(RTS_HANDLE hCert1, RTS_HANDLE hCert2) =0;
		virtual RTS_RESULT CDECL IX509CertKeyClose(RtsCryptoKey* pKey) =0;
		virtual RTS_RESULT CDECL IX509CertClose(RTS_HANDLE hCert) =0;
		virtual RTS_RESULT CDECL IX509CertCmsVerify(RTS_HANDLE hCertStore, RtsByteString* pCms, RTSCERTENCODING encoding, RtsByteString* pInData, RtsByteString* pOutData) =0;
		virtual RTS_RESULT CDECL IX509CertCmsVerify2(RTS_HANDLE hCertStore, RtsByteString* pCms, RTSCERTENCODING encoding, RtsByteString* pInData, RtsByteString* pOutData, RTS_HANDLE* phSignerCert) =0;
		virtual RTS_RESULT CDECL IX509CertCmsDecrypt(RTS_HANDLE hCertStore, RTS_HANDLE hRecipientCert, RtsByteString* pCms, RTSCERTENCODING encoding, RtsByteString* pOutData) =0;
		virtual RTS_RESULT CDECL IRtsOIDCreate(char* id, RtsOID* oid) =0;
		virtual RTS_RESULT CDECL IRtsOIDGetID(RtsOID* oid, RtsByteString* pBuffer) =0;
		virtual RTS_RESULT CDECL IRtsOIDGetName(RtsOID* oid, RtsByteString* pBuffer) =0;
		virtual RTS_RESULT CDECL IRtsOIDClear(RtsOID* oid) =0;
		virtual RTS_RESULT CDECL IX509CertInfoInit(RtsX509CertInfo* pCertInfo, RTS_UI32 structSize) =0;
		virtual RTS_RESULT CDECL IX509CertInfoExit(RtsX509CertInfo* pCertInfo) =0;
		virtual RTS_HANDLE CDECL IX509CertStoreGetFirstBackend(RTS_HANDLE hCertStore, RtsCertBackendInfo* pInfo, RTS_RESULT* pResult) =0;
		virtual RTS_HANDLE CDECL IX509CertStoreGetNextBackend(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsCertBackendInfo* pInfo, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IX509CertStorGetBackendInfo(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsCertBackendInfo* pInfo) =0;
		virtual RTS_HANDLE CDECL IX509CertCreateSelfSigned(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, RTS_UI32 ui32KeySize, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IX509CertCreateSelfSignedAsync(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, RTS_UI32 ui32KeySize, void* pCallbackParam, PFX509ASYNCCALLBACK pfCallback) =0;
		virtual RTS_RESULT CDECL IX509CertCreateCSR(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, RTS_UI32 ui32KeySize, RTSCERTENCODING encoding, RtsByteString* pCSR) =0;
		virtual RTS_RESULT CDECL IX509CertCreateCSRAsync(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RtsX509CertInfo* pCertInfo, CMPID cmpID, RTS_UI32 ui32KeySize, RTSCERTENCODING encoding, RtsByteString* pCSR, void* pCallbackParam, PFX509ASYNCCALLBACK pfCallback) =0;
		virtual RTS_RESULT CDECL IX509CertStoreAddCert(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTS_HANDLE hCert, RTSCERTTRUSTLEVEL trustLevel) =0;
		virtual RTS_RESULT CDECL IX509CertStoreRemoveCert(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTS_HANDLE hCert, RTS_BOOL bRemovePrivateKey) =0;
		virtual RTS_HANDLE CDECL IX509CertStoreGetFirstCert(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTSCERTTRUSTLEVEL trustLevel, RTS_RESULT* pResult) =0;
		virtual RTS_HANDLE CDECL IX509CertStoreGetNextCert(RTS_HANDLE hCertStore, RTS_HANDLE hBackend, RTS_HANDLE hCert, RTS_RESULT* pResult) =0;
		virtual RTS_HANDLE CDECL IX509CertStoreRegister(RTS_HANDLE hCertStore, CMPID cmpID, RtsX509CertInfo* pCertInfo, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL IX509CertStoreUnregister(RTS_HANDLE hCertStore, RTS_HANDLE hRegisteredComponent) =0;
		virtual RTS_HANDLE CDECL IX509CertStoreGetRegisteredCert(RTS_HANDLE hCertStore, RTS_HANDLE hUseCase, RTS_RESULT* pResult) =0;
};
	#ifndef ITF_CmpX509Cert
		#define ITF_CmpX509Cert static ICmpX509Cert *pICmpX509Cert = NULL;
	#endif
	#define EXTITF_CmpX509Cert
#else	/*CPLUSPLUS*/
	typedef ICmpX509Cert_C		ICmpX509Cert;
	#ifndef ITF_CmpX509Cert
		#define ITF_CmpX509Cert
	#endif
	#define EXTITF_CmpX509Cert
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPX509CERTITF_H_*/
