#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace Syncfusion.HtmlConverter.Natives
{

    #region IPersistMoniker
    [ComImport, ComVisible(true),
    Guid("79eac9c9-baf9-11ce-8c82-00aa004ba90b"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistMoniker
    {
        void GetClassID(
            [In, Out] ref Guid pClassID);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int IsDirty();

        void Load([In] int fFullyAvailable,
            [In, MarshalAs(UnmanagedType.Interface)] IMoniker pmk,
            [In, MarshalAs(UnmanagedType.Interface)] Object pbc,
            [In, MarshalAs(UnmanagedType.U4)] uint grfMode);

        void SaveCompleted(
            [In, MarshalAs(UnmanagedType.Interface)] IMoniker pmk,
            [In, MarshalAs(UnmanagedType.Interface)] Object pbc);

        [return: MarshalAs(UnmanagedType.Interface)]
        IMoniker GetCurMoniker();
    }
    #endregion

    #region IAsyncMoniker
    [ComVisible(true), ComImport(),
    Guid("79EAC9D3-BAF9-11CE-8C82-00AA004BA90B"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAsyncMoniker
    {
    }
    #endregion

    #region LoadHTMLMoniker
    internal class LoadHTMLMoniker : IMoniker, IAsyncMoniker
    {
        private IStream m_stream = null;
        private string m_sBaseName = string.Empty;

        public void InitLoader(string sContent, string sBaseUrl)
        {
            m_sBaseName = sBaseUrl;
            int hr = WinApis.CreateStreamOnHGlobal(Marshal.StringToHGlobalAuto(sContent), true, out m_stream);
            if ((hr != 0) || (m_stream == null))
                return;
        }



        void IMoniker.BindToObject(IBindCtx pbc, IMoniker pmkToLeft, ref Guid riidResult, out object ppvResult)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.BindToStorage(IBindCtx pbc, IMoniker pmkToLeft, ref Guid riid, out object ppvObj)
        {
            ppvObj = null;
            if (riid.Equals(Iid_Clsids.IID_IStream))
                ppvObj = (IStream)m_stream;
        }

        void IMoniker.CommonPrefixWith(IMoniker pmkOther, out IMoniker ppmkPrefix)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.ComposeWith(IMoniker pmkRight, bool fOnlyIfNotGeneric, out IMoniker ppmkComposite)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.Enum(bool fForward, out IEnumMoniker ppenumMoniker)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.GetClassID(out Guid pClassID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.GetDisplayName(IBindCtx pbc, IMoniker pmkToLeft, out string ppszDisplayName)
        {
            ppszDisplayName = m_sBaseName;
        }

        void IMoniker.GetSizeMax(out long pcbSize)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.GetTimeOfLastChange(IBindCtx pbc, IMoniker pmkToLeft, out System.Runtime.InteropServices.ComTypes.FILETIME pFileTime)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.Hash(out int pdwHash)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.Inverse(out IMoniker ppmk)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IMoniker.IsDirty()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IMoniker.IsEqual(IMoniker pmkOtherMoniker)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IMoniker.IsRunning(IBindCtx pbc, IMoniker pmkToLeft, IMoniker pmkNewlyRunning)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IMoniker.IsSystemMoniker(out int pdwMksys)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.Load(IStream pStm)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.ParseDisplayName(IBindCtx pbc, IMoniker pmkToLeft, string pszDisplayName, out int pchEaten, out IMoniker ppmkOut)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.Reduce(IBindCtx pbc, int dwReduceHowFar, ref IMoniker ppmkToLeft, out IMoniker ppmkReduced)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.RelativePathTo(IMoniker pmkOther, out IMoniker ppmkRelPath)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IMoniker.Save(IStream pStm, bool fClearDirty)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
    #endregion
}
