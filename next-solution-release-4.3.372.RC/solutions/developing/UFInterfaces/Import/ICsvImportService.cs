using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFInterfaces
{
    public interface ICsvImportService<T, TMap>
    {
        List<T> ImportCsv(string filePath, string delimiter);
    }
}
