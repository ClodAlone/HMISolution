using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScreenSettings
{
    public class CRMapsHeler
    {
        public Dictionary<String, IEnumerable<object>> ConnectionString { get; set; }
        public Dictionary<String, IEnumerable<object>> ScreenLinks { get; set; }
        public Dictionary<String, IEnumerable<object>> StringIDs { get; set; }
        public Dictionary<String, IEnumerable<object>> TagInScreen { get; set; }
        public Dictionary<String, IEnumerable<object>> TagInScreenEntities { get; set; }
        public Dictionary<String, Entities.ScreenEntity> EntityWithStringToParse { get; set; }
        public Dictionary<String, Entities.ScreenEntity> EntityWithCodeToParse { get; set; }
        public Dictionary<String, Entities.ScreenEntity> EntityWithConnectionToParse { get; set; }
        public bool UpdateStrings { get; set; }
        public List<Object> DocDispatcherReferenceList { get; set; }
        public List<String> ErrorMessages { get; set; }
        public CRMapsHeler()
        {
            ErrorMessages = new List<string>();
            ConnectionString = new Dictionary<string, IEnumerable<object>>();
            ScreenLinks = new Dictionary<string, IEnumerable<object>>();
            StringIDs = new Dictionary<string, IEnumerable<object>>();
            TagInScreen = new Dictionary<String, IEnumerable<object>>();
            TagInScreenEntities = new Dictionary<string, IEnumerable<object>>();
            EntityWithCodeToParse = new Dictionary<String, Entities.ScreenEntity>();
            EntityWithConnectionToParse = new Dictionary<String, Entities.ScreenEntity>();
            EntityWithStringToParse = new Dictionary<String, Entities.ScreenEntity>();
        }
    }
}
