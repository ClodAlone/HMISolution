using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Collections;
using System.Web.UI;

namespace VFS
{
    public class UrlUtils
    {
        private static string[] AbsolutePathPrefixes = new string[] { "about:", "file:///", "ftp://", "gopher://", 
			"http://", "https://", "javascript:", "mailto:", "news:", "res://", "telnet://", "view-source:" };
        public static string AppDomainAppVirtualPathString
        {
            get { return HttpRuntime.AppDomainAppVirtualPath + "/"; }
        }
        public static string Combine(string appPath, string basepath, string relative)
        {
            string ret = "";
            if (string.IsNullOrEmpty(relative))
                throw new ArgumentNullException("relative");
            if (string.IsNullOrEmpty(basepath))
                throw new ArgumentNullException("basepath");
            if ((basepath[0] == '~') && (basepath.Length == 1))
                basepath = "~/";
            else
            {
                int index = basepath.LastIndexOf('/');
                if (index < (basepath.Length - 1))
                    basepath = basepath.Substring(0, index + 1);
            }
            if (IsRooted(relative))
                ret = relative;
            else
            {
                if ((relative.Length == 1) && (relative[0] == '~'))
                    return appPath;
                if (IsAppRelativePath(relative))
                {
                    ret = appPath.Length > 1 ? appPath + "/" + relative.Substring(2) :
                        ret = "/" + relative.Substring(2);
                }
                else
                    ret = SimpleCombine(basepath, relative);
            }
            return Reduce(ret);
        }
        public static bool IsAbsoluteVirtualPath(string virtualPath)
        {
            return virtualPath != "" ? virtualPath[0] == '/' : false;
        }
        public static bool IsAbsolutePhysicalPath(string path)
        {
            if ((path == null) || (path.Length < 3))
                return false;
            if ((path[1] == ':') && IsDirectorySeparatorChar(path[2]))
                return true;
            return IsUncSharePath(path);
        }
        public static bool IsAppRelativePath(string path)
        {
            if ((path.Length > 0) && (path[0] == '~'))
                return (path.Length == 1) || IsRooted(path.Substring(1));
            else
                return false;
        }
        public static bool IsAbsoluteUrl(string url)
        {
            if (url != "")
            {
                foreach (string prefix in AbsolutePathPrefixes)
                    if (url.StartsWith(prefix))
                        return true;
            }
            return false;
        }
        public static bool IsCurrentUrl(string resolvedUrl)
        {
            return IsCurrentUrl(resolvedUrl, false);
        }
        public static bool IsCurrentUrl(string resolvedUrl, bool ignoreQueryString)
        {
            if (HttpContext.Current == null)
                return false;
            if (IsAbsoluteUrl(resolvedUrl))
            {
                string rawUrl = GetAbsoluteUrlFromRawUrl(HttpContext.Current.Request.RawUrl,
                    HttpContext.Current.Request.Url.Host);
                return CompareUrls(rawUrl, resolvedUrl, ignoreQueryString) ||
                    CompareUrls(HttpContext.Current.Request.Url.AbsoluteUri, resolvedUrl, ignoreQueryString);
            }
            else
            {
                int pos = resolvedUrl.IndexOf("/?");
                if (pos != -1)
                    resolvedUrl = resolvedUrl.Substring(pos + 1);
                string curentRawUrl = HttpContext.Current.Request.RawUrl;
                string currentUrl = HttpContext.Current.Request.Url.PathAndQuery;
                if (resolvedUrl.Trim().StartsWith("?"))
                {
                    curentRawUrl = GetQueryFromRawUrl(curentRawUrl);
                    currentUrl = HttpContext.Current.Request.Url.Query;
                }
                return CompareUrls(curentRawUrl, resolvedUrl, ignoreQueryString) ||
                    CompareUrls(currentUrl, resolvedUrl, ignoreQueryString);
            }
        }
        protected static bool CompareUrls(string currentUrl, string resolvedUrl, bool ignoreQueryString)
        {
            if (ignoreQueryString)
            {
                int pos = currentUrl.IndexOf("?", StringComparison.Ordinal);
                if (pos != -1)
                    currentUrl = currentUrl.Substring(0, pos);
            }
            return HttpUtility.UrlDecode(resolvedUrl.ToLower()) == HttpUtility.UrlDecode(currentUrl.ToLower());
        }
        protected static string GetAbsoluteUrlFromRawUrl(string rawUrl, string host)
        {
            string path = rawUrl;
            string query = "";
            int pos = rawUrl.IndexOf("?", StringComparison.Ordinal);
            if (pos != -1)
            {
                path = rawUrl.Substring(0, pos);
                query = rawUrl.Substring(pos + 1);
            }
            UriBuilder b = new UriBuilder();
            b.Host = host;
            b.Path = path;
            b.Query = query;
            return b.Uri.AbsoluteUri;
        }
        protected static string GetQueryFromRawUrl(string rawUrl)
        {
            string ret = "";
            int pos = rawUrl.IndexOf("?", StringComparison.Ordinal);
            if (pos != -1)
                ret = rawUrl.Substring(pos);
            return ret;
        }
        public static bool IsRelativeUrl(string virtualPath)
        {
            if (virtualPath.IndexOf(":", StringComparison.Ordinal) != -1)
                return false;
            return !IsRooted(virtualPath);
        }
        public static string GetPhysicalPath(string relativePath)
        {
            string ret = relativePath;
            if (ret != "")
            {
                if (ret[0] != '~')
                    ret = "~" + ret;
                if (ret[1] != '/')
                    ret = ret.Substring(0, 1) + "/" + ret.Substring(2, 1);
                if (ret[ret.Length - 1] != '/')
                    ret += "/";
                ret = HttpContext.Current.Server.MapPath(ret);
            }
            return ret;
        }
        public static string ResolvePhysicalPath(string relativePath)
        {
            string path = relativePath ?? string.Empty;
            if (System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath == null)
                return path;
            if (path.Length > 0 && path.Substring(1).StartsWith(":\\") || path.StartsWith("\\\\"))
                return path;
            path = path.StartsWith(".\\") ? "~" + path.Substring(1) : path;
            if (path.StartsWith("~"))
                return System.Web.Hosting.HostingEnvironment.MapPath(path);
            return Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, relativePath.Trim('\\', '/'));
        }
        public static string MakeVirtualPathAppAbsolute(string virtualPath)
        {
            string ret = virtualPath;
            if (HttpRuntime.AppDomainAppId != null)
                ret = MakeVirtualPathAppAbsolute(virtualPath, HttpRuntime.AppDomainAppVirtualPath);
            return ret;
        }
        public static string MakeVirtualPathAppAbsolute(string virtualPath, string applicationPath)
        {
            string ret = virtualPath;
            if ((virtualPath.Length == 1) && (virtualPath[0] == '~'))
                ret = applicationPath;
            else
            {
                if (((virtualPath.Length >= 2) && (virtualPath[0] == '~')) && ((virtualPath[1] == '/') || (virtualPath[1] == '\\')))
                    ret = applicationPath.Length > 1 ? applicationPath + "/" + virtualPath.Substring(2) :
                        "/" + virtualPath.Substring(2);
                else
                {
                    if (!IsRooted(virtualPath))
                        new ArgumentException(string.Format(Properties.Resources.InvalidVirtualPath, virtualPath));
                }
            }
            return ret;
        }
        internal static string FixVirtualPathSlashes(string virtualPath)
        {
            string ret = "";
            virtualPath = Replace(virtualPath, '\\', '/');
            string newVirtualPath = virtualPath.Replace("///", "/");
            newVirtualPath = newVirtualPath.Replace("//", "/");
            if (newVirtualPath == virtualPath)
                ret = virtualPath;
            ret = newVirtualPath;
            return ret;
        }
        protected static bool HasTrailingSlash(string virtualPath)
        {
            return (virtualPath[virtualPath.Length - 1] == '/');
        }
        protected static bool IsDirectorySeparatorChar(char ch)
        {
            if (ch != '\\')
                return (ch == '/');
            return true;
        }
        protected static bool IsRooted(string basepath)
        {
            return !string.IsNullOrEmpty(basepath) && ((basepath[0] == '\\') || (basepath[0] == '/'));
        }
        protected static bool IsUncSharePath(string path)
        {
            return ((path.Length > 2) && IsDirectorySeparatorChar(path[0])) && IsDirectorySeparatorChar(path[1]);
        }
        protected static string Reduce(string path)
        {
            string pathPrefix = "";
            if (path != null)
            {
                int index = path.IndexOf('?');
                if (index >= 0)
                {
                    pathPrefix = path.Substring(index);
                    path = path.Substring(0, index);
                }
            }
            path = FixVirtualPathSlashes(path);
            path = ReduceVirtualPath(path);
            if (pathPrefix == "")
                return path;
            return (path + pathPrefix);
        }
        protected static string ReduceVirtualPath(string path)
        {
            int length = path.Length;
            int dotIndex = 0;
            while (true)
            {
                dotIndex = path.IndexOf('.', dotIndex);
                if (dotIndex < 0)
                    return path;
                if (((dotIndex == 0) || (path[dotIndex - 1] == '/')) && ((((dotIndex + 1) == length) ||
                    (path[dotIndex + 1] == '/')) || ((path[dotIndex + 1] == '.') &&
                    (((dotIndex + 2) == length) || (path[dotIndex + 2] == '/')))))
                    break;
                dotIndex++;
            }
            ArrayList dirs = new ArrayList();
            StringBuilder strBuilder = new StringBuilder();
            dotIndex = 0;
            while (true)
            {
                int slashIndex = dotIndex;
                dotIndex = path.IndexOf('/', slashIndex + 1);
                if (dotIndex < 0)
                    dotIndex = length;
                if ((((dotIndex - slashIndex) <= 3) && ((dotIndex < 1) || (path[dotIndex - 1] == '.'))) && (((slashIndex + 1) >= length) || (path[slashIndex + 1] == '.')))
                {
                    if ((dotIndex - slashIndex) == 3)
                    {
                        if (dirs.Count == 0)
                            throw new HttpException("Cannot_exit_up_top_directory");
                        if ((dirs.Count == 1) && IsAppRelativePath(path))
                            return ReduceVirtualPath(MakeVirtualPathAppAbsolute(path));
                        strBuilder.Length = (int)dirs[dirs.Count - 1];
                        dirs.RemoveRange(dirs.Count - 1, 1);
                    }
                }
                else
                {
                    dirs.Add(strBuilder.Length);
                    strBuilder.Append(path, slashIndex, dotIndex - slashIndex);
                }
                if (dotIndex == length)
                {
                    string newVirtualPath = strBuilder.ToString();
                    if (newVirtualPath.Length != 0)
                        return newVirtualPath;
                    if ((length > 0) && (path[0] == '/'))
                        return "/";
                    return ".";
                }
            }
        }
        protected static string Replace(string s, char c1, char c2)
        {
            int index = s.IndexOf(c1);
            if (index < 0)
                return s;
            return s.Replace(c1, c2);
        }
        protected static string SimpleCombine(string basepath, string relative)
        {
            if (HasTrailingSlash(basepath))
                return (basepath + relative);
            return (basepath + "/" + relative);
        }
        public static void ValidateFolderUrl(ref string url)
        {
            if (string.IsNullOrEmpty(url) || IsAbsoluteUrl(url)) return;
            if (!url.EndsWith("/"))
                url += '/';
        }
        public static string ResolvePhysicalPath(IUrlResolutionService rs, string physicalPath)
        {
            string appRelativePath = string.Empty;
            if (TryGetAppRelativePath(physicalPath, ref appRelativePath))
                return rs.ResolveClientUrl(appRelativePath);
            throw new ArgumentException("Cannot resolve a specified physical path to a relative path.");
        }
        public static bool TryGetAppRelativePath(string path, ref string result)
        {
            string applicationAbsolutePath = new DirectoryInfo(System.Web.Hosting.HostingEnvironment.MapPath("~/")).FullName;
            if (path.StartsWith(applicationAbsolutePath))
            {
                result = "~/" + path.Substring(applicationAbsolutePath.Length);
                return true;
            }
            result = path;
            return false;
        }
        public static string GetAppRelativePath(string path)
        {
            TryGetAppRelativePath(path, ref path);
            return path;
        }
        public static string ToAppRelative(string virtualPath)
        {
            int i = virtualPath.IndexOf('?');
            string validVirtualPath = i > -1 ? virtualPath.Substring(0, i) : virtualPath;
            string appRelativePath = VirtualPathUtility.ToAppRelative(validVirtualPath);
            if (i > -1)
                appRelativePath += virtualPath.Substring(i);
            return appRelativePath;
        }
    }
}
