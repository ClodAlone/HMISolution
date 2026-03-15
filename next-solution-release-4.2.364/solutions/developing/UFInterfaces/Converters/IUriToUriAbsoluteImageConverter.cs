using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VFS;

namespace UFInterfaces.Converters
{
    public interface IUriToUriAbsoluteImageConverter : IValueConverter
    {
        FileSystemProviderBase FileSystemProviderBase { get; set; }
        Uri AbsolutePath { get; set; }
        Uri AbsolutePath2 { get; set; }
    }
}
