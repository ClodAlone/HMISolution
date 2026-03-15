#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !NETFX_CORE && !WP
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Syncfusion.Pdf.Native
{
    /// <summary>
    /// Summary description for CryptoApi.
    /// </summary>
    internal sealed class CryptoApi
    {

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        private CryptoApi()
        {
        }
        #endregion

        #region Public methods
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CertOpenStore(
            [MarshalAs(UnmanagedType.LPStr)] String storeProvider,
         uint dwMsgAndCertEncodingType,
         IntPtr hCryptProv,
         uint dwFlags,
         String cchNameString);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern IntPtr CertFindCertificateInStore(
            IntPtr hCertStore,
            uint dwCertEncodingType,
            uint dwFindFlags,
            uint dwFindType,
            [In, MarshalAs(UnmanagedType.LPWStr)]String pszFindString,
            IntPtr pPrevCertCntxt);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern IntPtr CertEnumCertificatesInStore(
            IntPtr storeProvider,
            IntPtr prevCertContext);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern IntPtr CertDuplicateCertificateContext(
            IntPtr pCertContext);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern bool CertFreeCertificateContext(
            IntPtr pCertContext);

        [DllImport("Crypt32.dll", EntryPoint = "CryptSignMessage", CharSet = CharSet.Ansi)]
        public static extern bool CryptSignMessage
            (
            ref CRYPT_SIGN_MESSAGE_PARA pSignPara,
            bool fDetachedSignature,
            UInt32 cToBeSigned,
            IntPtr[] rgpbToBeSigned,
            int[] rgcbToBeSigned,
            IntPtr pbSignedBlob,
            ref UInt32 pcbSignedBlob
            );

        [DllImport("CRYPT32.DLL", EntryPoint = "CertCloseStore", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CertCloseStore
            (
            IntPtr storeProvider,
            int flags
            );

        [DllImport("crypt32.dll")]
        public static extern bool CryptDecodeObject(

            uint CertEncodingType,
            uint lpszStructType,
            IntPtr pbEncoded,
            int cbEncoded,
            uint flags,
            IntPtr pvStructInfo,
            ref int cbStructInfo
            );

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern bool PFXIsPFXBlob(ref CRYPT_DATA_BLOB pPfx);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern IntPtr PFXImportCertStore(
            ref CRYPT_DATA_BLOB pPfx,
            [MarshalAs(UnmanagedType.LPWStr)] string szPassword,
         uint dwFlags
         );

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CertOpenSystemStore(
            IntPtr hCryptProv,
            string storename);

        [DllImport("Cryptdll.dll", CharSet = CharSet.Ansi)]
        public static extern void MD5Init(ref Md5_Ctx context);

        [DllImport("Cryptdll.dll", CharSet = CharSet.Ansi)]
        public static extern void MD5Update(ref Md5_Ctx context, byte[] input, int inlen);

        [DllImport("Cryptdll.dll", CharSet = CharSet.Ansi)]
        public static extern void MD5Final(ref Md5_Ctx context);
        #endregion
    }
}
#endif