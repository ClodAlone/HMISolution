using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAEditor.Document
{
    internal class CacheTags
    {
        readonly public Dictionary<String, UFUAModel.UFUATag> BackslashTags = new Dictionary<String, UFUAModel.UFUATag>(StringComparer.OrdinalIgnoreCase);
        readonly public Dictionary<String, UFUAModel.UFUATag> UnderscoreTags = new Dictionary<String, UFUAModel.UFUATag>(StringComparer.OrdinalIgnoreCase);
    }
}
