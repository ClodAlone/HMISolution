using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VFS;

namespace SymbolGallery
{
    public class LibraryDropChangedEventArgs : EventArgs
    {
        public LibraryBrowser Name { get; private set; }
        public bool bDrop { get; private set; }

        public LibraryDropChangedEventArgs(LibraryBrowser n, bool d)
        {
            Name = n;
            bDrop = d;
        }
    }
}
