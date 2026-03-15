#ifndef __PLCUTIL_H__
#define __PLCUTIL_H__

#if defined (WIN32) || defined (_WIN32_WCE) || defined(_WIN32_WCE_EMULATION)
	#include <windows.h>
#endif

#if !defined(LPTSTR) && !defined (_WIN32_WCE) && !defined(_WIN32_WCE_EMULATION) && !defined(WIN32)
	#ifdef _UNICODE
		#error Definition of LPTSTR missing!
	#else
		#define LPTSTR		char*
	#endif
#endif
#if !defined(TCHAR)	&& !defined (_WIN32_WCE) && !defined(_WIN32_WCE_EMULATION) && !defined(WIN32)
	#ifdef _UNICODE
		#error Definition of TCHAR missing!
	#else
		#define TCHAR		char
	#endif
#endif

#ifndef MAX_PATH
	#define MAX_PATH		512
#endif

#include "PLCConfig.h"

#if __cplusplus
extern "C" {
#endif

int PLCHUtlIniReadInt(char *pszSection, char *pszKey, int nDefault, char *pszIniFile);
int PLCHUtlIniReadString(char *pszSection, char *pszKey, char *pszDefault, char *pszDest, unsigned long ulDest, char *pszIniFile);
int PLCHUtlIniReadWString(char *pszSection, char *pszKey, RTS_WCHAR *pwszDefault, RTS_WCHAR *pwszDest, unsigned long ulDest, char *pszIniFile);
double PLCHUtlIniReadDouble(char *pszSection, char *pszKey, double dDefault, char *pszIniFile);

#if defined (WIN32) || defined (_WIN32_WCE) || defined(_WIN32_WCE_EMULATION)
	LPTSTR Str2TStr(LPTSTR ptszOut, char* pszIn);
	char*  TStr2Str(char* pszOut, LPTSTR ptszIn);
	int UtlRegWriteString(char *pszKey, char *pszValue, char *pszData, unsigned long ulLen);
	int UtlRegReadString(char *pszKey, char *pszValue, char *pszData, unsigned long *pulLen);
	int UtlRegWriteInt(char *pszKey, char *pszValue, int iValue);
	int UtlRegReadInt(char *pszKey, char *pszValue, int iDefault);
#endif

void UtlSplitPath(char *pszPath, char *pszDrive, char *pszDir, char *pszFileName, char *pszExtension);

unsigned long UtlStructReadULong(char *pszKey, unsigned long ulDefault, PlcDeviceDesc *pDesc);
int UtlStructReadString(char *pszKey, char *pszDefault, char *pszDest, unsigned long ulDest, PlcDeviceDesc *pDesc);
int UtlStructReadWString(char *pszKey, RTS_WCHAR *pwszDefault, RTS_WCHAR *pwszDest, unsigned long ulDest, PlcDeviceDesc *pDesc);
void UtlStructRemoveParameter(char* pszParameterName, PlcDeviceDesc* pDeviceDesc);

long UtlCopyGatewayConnection(GatewayConnection* pDest, GatewayConnection* pSrc);
void UtlDeleteGatewayConnection(GatewayConnection* pGWC);
long UtlCopyPlcConfig(PlcConfig* pDest, PlcConfig* pSrc);
void UtlDeletePlcConfig(PlcConfig* pPlcConfig);
long UtlCopyPlcDeviceDesc( PlcDeviceDesc* pDest, PlcDeviceDesc* pSrc);
void UtlDeletePlcDeviceDesc(PlcDeviceDesc* pDeviceDesc);

int PLCHUtlStrICmp(char *pszString1, char *pszString2);
int PLCHUtlStrNICmp(char *pszString1, char *pszString2, int n);

unsigned long PLCHUtlGenerateKey(void);
unsigned long PLCHUtlGetTargetEncryption(unsigned long dwKey, unsigned long dwTargetId, unsigned long dwHookId);

int PLCHUtlStringToLogicalPlcAdr(char *pszString, int iMaxNumofElements, unsigned char *pucAddressComp, int *pResult);
int PLCHUtlLogicalPlcAdrToString(ADDRESSCOMPONENT *pAddress, unsigned int NumofElements, char *pszString, unsigned int StringLen);

#if __cplusplus
}
#endif

#endif	/* __PLCUTIL_H__ */

