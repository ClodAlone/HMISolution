using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using System.Windows.Controls;

namespace SymbolGallery.ComponentService
{
    public interface ISymbolGallery : IUFInterfaceBase
    {
        bool AddSymbolToLibrary(String folder, String name, String xaml, String settings);
        bool UpdateSymbolToLibrary(String xaml, String settings, String provider, String path, String relativePath);

        String GetCurrentDropSettings(UserControl control = null);
        String GetCurrentSourceSymbolProvider(UserControl control = null);
        String GetCurrentSourceSymbolPath(UserControl control = null, string currentSourceSymbolPath = null);
        String GetCurrentSourceSymbolCode(UserControl control = null);

        String GetSymbolElement(String sourceSymbolProvider, String sourceSymbolPath, String relativePath = null);
        String GetSymbolSettings(String sourceSymbolProvider, String sourceSymbolPath, String relativePath = null);
        String GetSymbolCode(String sourceSymbolProvider, String sourceSymbolPath, String relativePath = null);

        UserControl GetStyleLibraryControl(String type, String current);
    }
}
