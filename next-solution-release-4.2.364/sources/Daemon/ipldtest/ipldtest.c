// ipldtest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <stdio.h>
#include "libipld.h"

#define LINEFEED    printf("\n")

// Construct pointer types for the IPLD service functions
typedef ipld_version_t (WINAPI * funcIpldGetVersion)(void);
typedef BOOL (WINAPI * funcIpldGetLicStatus)(IN         char* progea_vsn_string,
                                             OUT        char* json_buffer,
                                             IN OUT     UINT32* bufsize,
                                             OUT        UINT32* result
                                             );

// Pointers to dynamically loaded functions
funcIpldGetVersion      pIpldGetVersion = NULL;
funcIpldGetLicStatus    pIpldGetLicStatus = NULL;


char progeaVsnString1[] = "3.2.100";      // Older version for testing (fictitious)
char progeaVsnString2[] = "4.0.300";      // Current version
char progeaVsnString3[] = "5.0.22";       // A next Major version to come (fictitious)

#define LOCAL_JSON_BUFFSIZE 1024
BYTE json_buffer[LOCAL_JSON_BUFFSIZE];

//
// Function prototypes
//
BOOL doDemoTesting();
BOOL doReleaseTesting();
void dispIpldResult(BOOL retval, UINT32 result);


//
// Main function
//
int main()
{
    HMODULE         hIpldLib = NULL;
    BOOL            bSuccess = TRUE;
    ipld_version_t  ipldVersion;


    // Load the ICC/Progea License Daemon library
    hIpldLib = LoadLibrary("libipld.dll");
    if (NULL == hIpldLib)
    {
        printf("Error loading ipld library: %ld\n", GetLastError());
        bSuccess = FALSE;
    } else {
        printf("Ipld Lib loaded successfully\n");
    }

    // If no error so far, perform run-time linking 
    if (bSuccess)
    {
        (FARPROC)pIpldGetVersion = GetProcAddress(hIpldLib, "ipldGetVersion");
        if (NULL == pIpldGetVersion)
        {
            printf("Error loading external function 'ipldGetVersion'\n");
            bSuccess = FALSE;
        }

        (FARPROC)pIpldGetLicStatus = GetProcAddress(hIpldLib, "ipldGetLicStatus");
        if (NULL == pIpldGetLicStatus)
        {
            printf("Error loading external function 'ipldGetLicStatus'\n");
            bSuccess = FALSE;
        }
    }

    // If no error so far, do the regular processing
    if (bSuccess)
    {
        printf("Run-time dynamic linking successful, executing tests...\n");
        LINEFEED;

        ipldVersion = pIpldGetVersion();
        printf("IPLD library version: %d.%d\n", IPLD_VSN_MAJOR(ipldVersion), IPLD_VSN_MINOR(ipldVersion));

        if (0 == IPLD_VSN_MAJOR(ipldVersion))
        {
            printf("Major version is '0': Library is a preliminary demo build.\n");
            bSuccess = doDemoTesting();
        } else {
            bSuccess = doReleaseTesting();
        }

    } else {
        printf("Errors occurred so far, aborting...\n");
    }
 
    // Cleanup
    if (hIpldLib) FreeLibrary(hIpldLib);
}


//
// DEMO testing procedure: executed if IPLD Major Version == 0
// This procedure demonstrates various aspects of the interface behavior
//
BOOL doDemoTesting(void)
{
    BOOL            bSuccess = TRUE;
    UINT32          jsonBufferSize;
    DWORD           ipldResult;

    LINEFEED;
    printf("Entering Demo test procedure...\n");
    LINEFEED;

    // Test argument validation (1)
    printf("Calling ipldGetLicStatus with invalid arguments (1)\n");
    jsonBufferSize = 0;
    bSuccess = pIpldGetLicStatus(NULL, NULL, &jsonBufferSize, NULL);
    printf("  Function returned: %s\n", bSuccess ? "TRUE" : "FALSE");

    LINEFEED;

    // Test argument validation (2)
    printf("Calling ipldGetLicStatus with invalid arguments (2)\n");
    bSuccess = pIpldGetLicStatus(NULL, NULL, NULL, &ipldResult);
    dispIpldResult(bSuccess, ipldResult);

    LINEFEED;

    // Get the required size for the JSON buffer
    // Note that the argument validation is only done for last two arguments
    printf("Calling ipldGetLicStatus with BUFFSIZE 0\n");
    jsonBufferSize = 0;
    bSuccess = pIpldGetLicStatus(NULL, NULL, &jsonBufferSize, &ipldResult);
    dispIpldResult(bSuccess, ipldResult);
    printf("  Required buffer size: %d\n", jsonBufferSize);

    LINEFEED;

    // Test argument validation (3)
    // We pass sufficient buffer size but no buffer pointer
    printf("Calling ipldGetLicStatus with invalid arguments (3)\n");
    jsonBufferSize = LOCAL_JSON_BUFFSIZE;
    bSuccess = pIpldGetLicStatus(NULL, NULL, &jsonBufferSize, &ipldResult);
    dispIpldResult(bSuccess, ipldResult);

    LINEFEED;

    // Test argument validation (4)
    // We pass sufficient buffer and buffer pointer but no version string
    printf("Calling ipldGetLicStatus with invalid arguments (4)\n");
    jsonBufferSize = LOCAL_JSON_BUFFSIZE;
    bSuccess = pIpldGetLicStatus(NULL, json_buffer, &jsonBufferSize, &ipldResult);
    dispIpldResult(bSuccess, ipldResult);

    LINEFEED;

    // Pass valid arguments with matching Progea Major Version
    printf("Calling ipldGetLicStatus witch Progea version: %s\n", progeaVsnString2);
    jsonBufferSize = LOCAL_JSON_BUFFSIZE;
    bSuccess = pIpldGetLicStatus(progeaVsnString2, json_buffer, &jsonBufferSize, &ipldResult);
    dispIpldResult(bSuccess, ipldResult);
    if (bSuccess)
    {
        printf("Size of JSON information in buffer: %d\n", jsonBufferSize);
        json_buffer[jsonBufferSize] = 0;
        printf("JSON buffer: %s\n", json_buffer);
    }

    LINEFEED;

    // Pass valid arguments with older Progea Major Version (Major version lower than version in license file)
    printf("Calling ipldGetLicStatus witch Progea version: %s\n", progeaVsnString1);
    jsonBufferSize = LOCAL_JSON_BUFFSIZE;
    bSuccess = pIpldGetLicStatus(progeaVsnString1, json_buffer, &jsonBufferSize, &ipldResult);
    dispIpldResult(bSuccess, ipldResult);
    if (bSuccess)
    {
        printf("Size of JSON information in buffer: %d\n", jsonBufferSize);
        json_buffer[jsonBufferSize] = 0;
        printf("JSON buffer: %s\n", json_buffer);
    }

    LINEFEED;

    // Pass valid arguments with newer Progea Major Version (Major version higher than version in license file)
    printf("Calling ipldGetLicStatus witch Progea version: %s\n", progeaVsnString3);
    jsonBufferSize = LOCAL_JSON_BUFFSIZE;
    bSuccess = pIpldGetLicStatus(progeaVsnString3, json_buffer, &jsonBufferSize, &ipldResult);
    dispIpldResult(bSuccess, ipldResult);
    if (bSuccess)
    {
        printf("Size of JSON information in buffer: %d\n", jsonBufferSize);
        json_buffer[jsonBufferSize] = 0;
        printf("JSON buffer: %s\n", json_buffer);
    }


    return bSuccess;
}


//
// RELEASE testing procedure: executed if IPLD Major Version >= 1
//

BOOL doReleaseTesting(void)
{
    BOOL    bSuccess = TRUE;

    LINEFEED;
    printf("Entering Release test procedure...\n");
    LINEFEED;

    return bSuccess;
}


void dispIpldResult(
    BOOL retval,
    UINT32 result
) {
    printf("  Function returned: %s\n", retval ? "TRUE" : "FALSE");
    // printf("  Result code: %d / %d\n", result, IPLD_RESULT_CODE_MAX);
    if (result <= IPLD_RESULT_CODE_MAX)
    {
        printf("  Result code: %s\n", strIpldResult[result]);
    } else {
        printf("  Result code UNDEFINED\n");
    }

}