using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UFUAHistorianModel.Helpers
{
    class CustomThreadSafeDataLayer : ThreadSafeDataLayer
    {
        public int CommandTimeout { get; private set; }
        public CustomThreadSafeDataLayer(int commadTimeout, XPDictionary dictionary, IDataStore provider, params Assembly[] persistentObjectsAssemblies) : base(dictionary, provider, persistentObjectsAssemblies)
        {
            CommandTimeout = commadTimeout;
        }
        public override IDbCommand CreateCommand()
        {
            IDbCommand result = base.CreateCommand();
            if(CommandTimeout != 0)
                result.CommandTimeout = CommandTimeout;
            return result;
        }
    }
}
