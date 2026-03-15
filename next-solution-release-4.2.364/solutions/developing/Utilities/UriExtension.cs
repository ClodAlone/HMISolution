using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !WINDOWS_UWP
using System.Web;
#endif
namespace Utilities
{
    public static class UriExtension
    {
        public static String GetPathString(this Uri uri)
        {
            if (uri == null)
                return String.Empty;
#if !WINDOWS_UWP
            try
            {
                return HttpUtility.UrlDecode(uri.LocalPath);
            }
            catch (Exception ex)
            {             
            }
            return HttpUtility.UrlDecode(uri.OriginalString);
#else
            return uri.OriginalString;
#endif
        }

        public static bool IsValidFile(this Uri uri)
        {
            if (uri == null)
                return false;

            var path = uri.GetPathString();
            return DirectoryHelper.IsValidFile(path, false);
        }

        public static Uri GetUrlDecodedUri(this Uri uri)
        {
            var path = uri.GetPathString();
            return new Uri(path, UriKind.RelativeOrAbsolute);
        }
    }
}
