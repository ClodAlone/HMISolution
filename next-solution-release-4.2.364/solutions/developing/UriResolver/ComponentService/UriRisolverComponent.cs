using System;
using UFInterfaces.CoreHostComponents;
using System.IO;
using UFInterfaces;
using System.Collections.Generic;
#if !WINDOWS_UWP && !NET_STANDARD
using Tracing.ComponentService;
#endif
using System.ComponentModel;
using Utilities;
using DocumentManager.ComponentService;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace UriResolver.ComponentService
{
    public class UriResolverComponent : ComponentBase<IUriRisolver>, IUriRisolver, IDisposable
    {
#region Declaration
        Object lockObject = new Object();
        Dictionary<String, IDocumentManager> mapSchemeComponent = new Dictionary<String, IDocumentManager>();
        Dictionary<String, String> mapExtensionScheme = new Dictionary<String, String>();
        List<IDocumentManager> listInstalledComponent;
#if !WINDOWS_UWP && !NET_STANDARD
        IWorkspace workspace;
        ISimpleLogging simpleLogging;
#endif
        String baseFolderDesigner;
        IContainer container;
#endregion

#region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            GetComponentInterfaces();
        }
#endregion

        private void GetComponentInterfaces()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            //if (workspace == null)
            //    throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            if (simpleLogging == null)
                simpleLogging = GetService(typeof(ISimpleLogging)) as ISimpleLogging;
            baseFolderDesigner = String.Format("{0}{1}\\", AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.DocumentManagersPath);
#endif
            // RegisterFileTypes();
        }

        IDocumentManager GetDocumentManager(String scheme)
        {
            //using (new WaitCursor())
            {
                scheme = scheme.ToLower();
                lock (lockObject)
                {
                    IDocumentManager component = null;
                    if (mapSchemeComponent.TryGetValue(scheme, out component))
                        return component;

#if !WINDOWS_UWP && !NET_STANDARD
                    using (new WaitCursor())
                    {
                        List<IComponent> list = FindAndLoadDLL.LoadDLLs<IComponent>(baseFolderDesigner, String.Format("{0}*.dll", scheme));
                        if (list.Count <= 0)
                            throw new NotImplementedException(
                                String.Format("Cannot resolve the scheme {0}.", scheme));

                        foreach (IComponent c in list)
                        {
                            if (!(c is IDocumentManager))
                                continue;

                            var doc = list[0] as IDocumentManager;
                            if (!mapSchemeComponent.ContainsKey(scheme))
                            {
                                mapSchemeComponent.Add(scheme, doc);
                                if (Container != null)
                                    Container.Add(list[0]);
                                if (workspace != null)
                                    workspace.RegisterComponent(list[0]);
                            }

                            var ext = doc.FileType.ToLower();
                            int index = ext.IndexOf('.');
                            if (index >= 0)
                                ext = ext.Substring(index + 1);
                            if (!mapExtensionScheme.ContainsKey(ext))
                            {
                                mapExtensionScheme.Add(ext, scheme);
                            }

                            return doc;
                        }
                    }
#endif
                }
            }

            return null;
        }
        
        public String GetOpenFileFilter()
        {
#if !WINDOWS_UWP
            return Properties.Settings.Default.SupportedTypeFilter;
#else
            return "Project Files (*.UFProject)|*.UFProject|All Files (*.*)|*.*";
#endif
        }

        public String GetOpenFileScheme()
        {
#if !WINDOWS_UWP
            return Properties.Settings.Default.UFProject;
#else
            return "UFProjectManager";
#endif
        }

        public String GetUriType(Uri uri)
        {
            IDocumentManager component = ResolveUri(uri) as IDocumentManager;
            if (component == null)
                return null;
            return component.TypeScheme;
        }

        public IComponent ResolveUri(Uri uri)
        {
            String scheme = null;
            try
            {
                scheme = uri.Scheme;
            }
            catch 
            {
            }

#if !WINDOWS_UWP
            if (String.IsNullOrEmpty(scheme) || String.Compare(scheme, Uri.UriSchemeFile, false) == 0)
#endif
            {
                GetListInstalledDocumentManagers();

                String ext = Path.GetExtension(uri.GetPathString());
                if (String.IsNullOrEmpty(ext))
                    return null;

                int index = ext.IndexOf('.');
                if (index >= 0)
                    ext = ext.Substring(index + 1).ToLower();
                if (mapExtensionScheme.ContainsKey(ext))
                    scheme = mapExtensionScheme[ext];
                else
                    scheme = ApplicationPropertiesHelper.GetProperty<String>(ext);
            }

            if (String.IsNullOrEmpty(scheme))
                return null;

            IComponent ret = GetDocumentManager(scheme) as IComponent;
            if (ret == null)
                throw new NotImplementedException(String.Format("Cannot resolve the Uri {0}. missing DLL for the scheme {1}", uri, scheme));

            return ret;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public void RegisterFileTypes(bool bRegister)
        {
            if (!CurrentUser.IsAnAdministrator())
                return;

            var curAssembly = Assembly.GetEntryAssembly();

            var list = GetListInstalledDocumentManagers();
            Parallel.ForEach(list, c =>
                {
                    if (c.RegisterFileType)
                    {
                        try
                        {
                            var FA = new Org.Mentalis.Utilities.FileAssociation()
                            {
                                Extension = c.FileType,
                                ContentType = c.TypeLabel,
                                FullName = c.TypeScheme,
                                ProperName = c.TypeScheme
                            };
                            FA.AddCommand("open", String.Format("{0} \"%1\"", curAssembly.Location));
                            var assembly = Assembly.GetAssembly(c.GetType());
                            FA.IconPath = Path.ChangeExtension(assembly.Location, "ico");
                            if (bRegister)
                                FA.Create();
                            else
                                FA.Remove();
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                });
        }
#endif

        public List<IDocumentManager> GetListInstalledDocumentManagers()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            return GetListInstalledDocumentManagers(baseFolderDesigner);
#else
            if (listInstalledComponent == null)
                listInstalledComponent = new List<IDocumentManager>();
            return listInstalledComponent;
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public List<IDocumentManager> GetListInstalledDocumentManagers(string searchPath)
        {
            lock(lockObject)
            {
                if (listInstalledComponent == null)
                {
                    listInstalledComponent = new List<IDocumentManager>();

                    var list = FindAndLoadDLL.LoadDLLs<IComponent>(searchPath, "*.dll");

                    list.ForEach(c =>
                        {
                            if (c is IDocumentManager)
                            {
                                var doc = c as IDocumentManager;
                                String scheme = doc.TypeScheme.ToLower();
                                lock (mapSchemeComponent)
                                {
                                    if (!mapSchemeComponent.ContainsKey(scheme))
                                    {
                                        mapSchemeComponent.Add(scheme, doc);
                                        if (Container != null)
                                            Container.Add(c);
                                        if (workspace != null)
                                            workspace.RegisterComponent(c);
                                        else if (c is IUFInterfaceBase)
                                        {
                                            try
                                            {
                                                (c as IUFInterfaceBase).Initialize();
                                            }
                                            catch { }
                                        }
                                    }
                                }

                                var ext = doc.FileType.ToLower();
                                int index = ext.IndexOf('.');
                                if (index >= 0)
                                    ext = ext.Substring(index + 1);
                                lock (mapExtensionScheme)
                                {
                                    if (!mapExtensionScheme.ContainsKey(ext))
                                    {
                                        mapExtensionScheme.Add(ext, scheme);
                                    }
                                }

                                lock (listInstalledComponent)
                                {
                                    listInstalledComponent.Add(doc);
                                }
                            }
                        });
                }

                return listInstalledComponent;
            }
        }
#endif
        public void RegisterDocumentManager(IDocumentManager document, String scheme)
        {
            if (listInstalledComponent == null)
                listInstalledComponent = new List<IDocumentManager>();
            if (!listInstalledComponent.Contains(document))
                listInstalledComponent.Add(document);
            scheme = scheme.ToLower();
            if (mapSchemeComponent.ContainsKey(scheme))
                mapSchemeComponent.Remove(scheme);
            mapSchemeComponent.Add(scheme, document);

            var ext = document.FileType.ToLower();
            int index = ext.IndexOf('.');
            if (index >= 0)
                ext = ext.Substring(index + 1);
            if (!mapExtensionScheme.ContainsKey(ext))
                mapExtensionScheme.Add(ext, scheme);
        }

        public IDocumentManager GetManagerFromDocument(IDocument document)
        {
            var list = (from c in GetListInstalledDocumentManagers()// .AsParallel()
                         where c.DocumentType == document.GetType()
                         select c).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public Uri CreateNewDocumentFromUriAndScheme(Uri relative, String scheme, IDocument parent, bool encryptFile)
        {
            var doc = GetDocumentManager(scheme);
            if (doc == null)
                return null;

            return doc.CreateNewDocument(relative, parent, encryptFile);
        }
#endif

        public IContainer Container
        {
            get
            {
                return container;
            }
            set
            {
                container = value;
            }
        }

#region IDisposable Members

        void IDisposable.Dispose()
        {
            // lockObject = null;
        }

#endregion
    }
}
