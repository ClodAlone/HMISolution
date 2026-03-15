// Interface and other declarations for the Windows ICC/Progea Licence Daemon

#ifndef _IPLD_H
#define _IPLD_H

#ifdef _WIN32

#include <windows.h>

    #ifdef _LIBIPLD_CODE
        #define IPLD_API    __declspec(dllexport) __stdcall
    #else
        #define IPLD_API    __declspec(dllimport) __stdcall
    #endif

#elif __linux__ 

    #define UINT32 uint32_t
    #define WORD uint32_t
    #define BYTE uint8_t
    #define BOOL bool
    #define IPLD_API
    #define IN
    #define OUT

#endif 

#ifdef __cplusplus
extern "C" {
#endif


// Types 
typedef WORD ipld_version_t;
#define IPLD_VSN_MAJOR(vsn)     ((BYTE)(vsn>>8))
#define IPLD_VSN_MINOR(vsn)     ((BYTE)(vsn&0x00FF))

//
// ipldGetVersion
//
// Retrieve the version of the IPLD library.
//  
// Return value
//   Word value in the form "0xMMmm"
//   MM - Major version number
//   mm - minor version number
//
ipld_version_t IPLD_API ipldGetVersion(void);

// 
// ipldGetLicStatus
//
// Check the ICC license for integrity, against the hardware platform and against the version of the 
// installed software package.
//
// Arguments
//   progea_vsn_string      - Pointer to a string containing the Progea software package version (e.g. "4.0.300")
//   json_buffer            - Pointer to a buffer receiving the JSON data structure
//   bufsize                - Pointer to a numeric value 
//                            a) containing the size of the JSON buffer on entry
//                            b) holding the size of the copied JSON structure on exit
//                            c) holding the required buffer size if buffer passed in too small
//   result                 - Pointer variable receiving the a result code on function exit
//
// Return value
//   TRUE  - Function succeeded, validation successful, result == NO_ERROR 
//   FALSE - Function failed, result indicates reason for failure 
// Result codes
//   IPLD_NO_ERROR                  - Success (function returned TRUE)
//   IPLD_ERROR_BAD_ARGUMENTS       - Argument validation failed
//   IPLD_ERROR_NOT_ENOUGH_MEMORY   - Size of json_buffer passed in too small, on return bufsize holds the required size
//   IPLD_ERROR_PRODUCT_VERSION     - Mismatch of Progea version passed in and version in license file
//   IPLD_ERROR_FILE_NOT_FOUND      - License file or signature file not found
//   IPLD_ERROR_PUBLIC_KEY          - Error related to public key, e.g. public key for verification not found
//   IPLD_ERROR_VERIFICATION        - Signature verification error
//   ...
BOOL IPLD_API ipldGetLicStatus(
    IN         char* progea_vsn_string,
    OUT        char* json_buffer,
    IN OUT     UINT32* bufsize,
    OUT        UINT32* result
    );
    
#define IPLD_NO_ERROR                   0
#define IPLD_ERROR_BAD_ARGUMENTS        1
#define IPLD_ERROR_NOT_ENOUGH_MEMORY    2
#define IPLD_ERROR_PRODUCT_VERSION      3
#define IPLD_ERROR_FILE_NOT_FOUND       4
#define IPLD_ERROR_PUBLIC_KEY           5
#define IPLD_ERROR_VERIFICATION         6
#define IPLD_ERROR_LICVALIDURL          7
#define IPLD_ERROR_LICURL               8
#define IPLD_ERROR_PARSE                9
//   ...

#ifndef _LIBIPLD_CODE
const char* strIpldResult[] = {
        "IPLD_NO_ERROR",
        "IPLD_ERROR_BAD_ARGUMENTS",
        "IPLD_ERROR_NOT_ENOUGH_MEMORY",
        "IPLD_ERROR_PRODUCT_VERSION",
        "IPLD_ERROR_FILE_NOT_FOUND",
        "IPLD_ERROR_PUBLIC_KEY",
        "IPLD_ERROR_VERIFICATION",
        "IPLD_ERROR_LICVALIDURL",
        "IPLD_ERROR_LICURL",
        "IPLD_ERROR_PARSE"
};
#define IPLD_RESULT_CODE_MAX  ((UINT32)(sizeof(strIpldResult) / sizeof(char *))-1)

#endif


#ifdef __cplusplus
}
#endif

#endif