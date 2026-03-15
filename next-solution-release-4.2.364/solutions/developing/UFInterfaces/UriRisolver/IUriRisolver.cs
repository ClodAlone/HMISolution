using System;
using System.Collections.Generic;
using UFInterfaces;
using System.ComponentModel;
using DocumentManager.ComponentService;

namespace UriResolver.ComponentService
{
    public interface IUriRisolver : IUFInterfaceBase
    {
        IContainer Container { get; set; }

        IComponent ResolveUri(Uri uri);
        String GetOpenFileFilter();
        String GetOpenFileScheme();
        String GetUriType(Uri uri);

#if !WINDOWS_UWP && !NET_STANDARD
        void RegisterFileTypes(bool bRegister);
#else
        void RegisterDocumentManager(IDocumentManager document, String scheme);
#endif
        List<IDocumentManager> GetListInstalledDocumentManagers();

        IDocumentManager GetManagerFromDocument(IDocument document);

#if !WINDOWS_UWP && !NET_STANDARD
        Uri CreateNewDocumentFromUriAndScheme(Uri relative, String scheme, IDocument parent, bool encryptFile);
#endif
    }
}
