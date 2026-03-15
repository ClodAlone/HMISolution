using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Utilities
{
    public static class ExtractFileFromResource
    {
        public static void Extract(Assembly Assemb, String fileName, String nameResource)
        {
            if (Assemb == null) 
                Assemb = Assembly.GetExecutingAssembly();
            var stream = Assemb.GetManifestResourceStream(nameResource);
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                CopyStream(stream, fs);
            }
        }
        public static Stream Extract(Assembly Assemb, String nameResource)
        {
            if (Assemb == null)
                Assemb = Assembly.GetExecutingAssembly();
            var stream = Assemb.GetManifestResourceStream(nameResource);
            return stream;
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}
