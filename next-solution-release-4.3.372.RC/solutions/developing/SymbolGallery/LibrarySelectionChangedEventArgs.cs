using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VFS;

namespace SymbolGallery
{
    public class LibrarySelectionChangedEventArgs : EventArgs
    {
        public String Name { get; private set;  }
        public bool IsDynamic { get; private set; }
        public FileSystemProviderBase fileSystemProvider { get; private set; }

        public LibrarySelectionChangedEventArgs(String n, bool d, FileSystemProviderBase f, bool u = false)
        {
            Name = n;
            IsDynamic = d;
            fileSystemProvider = f;
        }
    }
}