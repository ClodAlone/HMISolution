using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderUnitTests.Helpers
{
    internal static class FactoryHelper
    {
        public static bool IsProviderAviable(String providerName)
        {
            try
            {
                var factory = System.Data.Common.DbProviderFactories.GetFactory(providerName);
                return factory != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
