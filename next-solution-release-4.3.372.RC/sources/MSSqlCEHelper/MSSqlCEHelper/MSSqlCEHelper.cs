using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSqlCEHelper
{
    public static class Utilities
    {
        public static bool IsV40Installed()
        {
            try
            {
                System.Reflection.Assembly.Load("System.Data.SqlServerCe, Version=4.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
            try
            {
                var factory = System.Data.Common.DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0");
            }
            catch (System.Configuration.ConfigurationException)
            {
                return false;
            }
            catch (System.ArgumentException)
            {
                return false;
            }
            return true;
        }
    }
}
