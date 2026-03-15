using System;
using System.Linq;
using System.Text;
using MSZ;
using DataReader.Helpers;
using DevExpress.Xpo;
using log4net;
using System.Threading;

namespace NextEnergyLicense
{
    class Program
    {
        //DB_Test
        //string ConnectionString = "XpoProvider=MSSqlServer;data source=(local);Trusted_Connection=True;initial catalog=TestingCLR;Persist Security Info=true";
        static bool isRedirected => Console.CursorVisible && false;
        static bool isLocal = false;
        static long measEnabled = 0;
        static bool bInit = false;
        static string _connectionString;
        static int iState = 1;
        private static readonly ILog log = LogManager.GetLogger("NextEnergyLicense");
        static string ConnectionString => _connectionString;
        static DateTime date;
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                CommandLineOptions cl = new CommandLineOptions(args);
                if (cl.IsValid)
                {
                    switch (cl.Operation)
                    {
                        case Operations.OpSection:
                            {
                                _connectionString = cl.ConnectionString;
#if DEBUG
                                LogMessage("ConnectionString: {0}", ConnectionString);
#endif
                            }
                            break;
                    }
                }
                else
                    bInit = true;
            }
            else
                bInit = true;
            
            if(!bInit)
            {
                bool mSZState = MSZ.MSZView.CheckState(true, true);
                CheckLicence(mSZState);
                WriteDB();
            }
        }

        private static void WriteDB()
        {
            try
            {
                String _defaultDataProvider = string.Empty;
                String _defaultConnectionString = string.Empty;
                _defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(ConnectionString);
                _defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(ConnectionString);


                using (var idlDel = CreateLicDataLayer(ConnectionString))
                {
                    using (var ufw = new UnitOfWork(idlDel))
                    {
                        if (ufw == null)
                        {
                            LogMessage(Properties.Resources.ErrorInfo, Properties.Resources.DBConnectionError);
                            return;
                        }

                        ufw.LockingOption = LockingOption.None;

                        ufw.BeginTransaction();

                        Random rnd = new Random();
                        Byte[] b = new Byte[122];
                        rnd.NextBytes(b);

                        var ret = ConcatByteArrays(
                            b, 
                            Encoding.ASCII.GetBytes(date.Year.ToString("0000")),
                            Encoding.ASCII.GetBytes(date.Month.ToString("00")),
                            Encoding.ASCII.GetBytes(date.Day.ToString("00")),
                            Encoding.ASCII.GetBytes(date.Hour.ToString("00")),
                            Encoding.ASCII.GetBytes(date.Minute.ToString("00")),
                            Encoding.ASCII.GetBytes(date.Second.ToString("00")),
                            Encoding.ASCII.GetBytes(iState.ToString("0")),
                            Encoding.ASCII.GetBytes(measEnabled.ToString("00000"))
                            );

                        Byte[] a = new Byte[255 - ret.Length];
                        rnd.NextBytes(a);
                        var _ret = ConcatByteArrays(ret, a);

                        try
                        {
                            TbOptimistic _det = (from entry in new XPQuery<TbOptimistic>(ufw)/*.AsParallel()*/
                                                     select entry).FirstOrDefault();
                            if (_det == null)
                                _det = new TbOptimistic(ufw);

#if DEBUG
                            StringBuilder _log = new StringBuilder();
                            _log.Append($"Year {Encoding.ASCII.GetString(_det.ConstPerf,122,4)}");
                            _log.Append($"Month {Encoding.ASCII.GetString(_det.ConstPerf, 126, 2)}");
                            _log.Append($"Day {Encoding.ASCII.GetString(_det.ConstPerf, 128, 2)}");
                            _log.Append($"Hour {Encoding.ASCII.GetString(_det.ConstPerf, 130, 2)}");
                            _log.Append($"Minute {Encoding.ASCII.GetString(_det.ConstPerf, 132, 2)}");
                            _log.Append($"Second {Encoding.ASCII.GetString(_det.ConstPerf, 134, 2)}");
                            _log.Append($"iState {Encoding.ASCII.GetString(_det.ConstPerf, 136, 1)}");
                            _log.Append($"MeasEnabled {Encoding.ASCII.GetString(_det.ConstPerf, 137, 5)}");
                            LogMessage("Byte read: {0}", _log.ToString());

#endif
                            _det.ConstPerf = _ret;
                            _det.ProgNum = rnd.Next(15756);

                            ufw.CommitChanges();
                            ufw.DropIdentityMap();

                            var recorToBeInserted = (from entry in new XPQuery<TbOptimistic>(ufw)/*.AsParallel()*/
                                                     where entry.Id == _det.Id
                                                     select entry).FirstOrDefault();
#if DEBUG
                            if (recorToBeInserted != null)
                                LogMessage(Properties.Resources.RecordInserted, recorToBeInserted.Id);
                            else
                                LogMessage(Properties.Resources.RecordNotInserted);
#endif

                        }
                        catch (Exception ex)
                        {
                            ufw.RollbackTransaction();
                            LogMessage(Properties.Resources.ErrorInfo, ex);
                            return;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage(Properties.Resources.ErrorInfo, ex);
            }
        }
        public static byte[] ConcatByteArrays(params byte[][] arrays)
        {
            return arrays.SelectMany(x => x).ToArray();
        }

        private static void LogMessage(string format, params object[] args)
        {
            try
            {
                //var rootAppender = ((Hierarchy)LogManager.GetRepository()).Root.Appenders.OfType<FileAppender>().FirstOrDefault();
                //var filename = rootAppender != null ? rootAppender.File : string.Empty;
                //if (string.IsNullOrEmpty(filename))
                //    return;
                log.Info(string.Format(format, args));
#if DEBUG
                if (Environment.UserInteractive && !isRedirected && Console.In != System.IO.StreamReader.Null)
                {
                    Console.WriteLine(format, args);
                }
#endif
            }
            catch (Exception)
            {
            }
        }

        static IDataLayer CreateLicDataLayer(String settings)
        {
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var store = XpoDefault.GetConnectionProvider(settings, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            dict.GetDataStoreSchema(typeof(TbOptimistic));
            return new ThreadSafeDataLayer(dict, store);
        }

        private static void CheckLicence(bool mSZState)
        {
            try
            {
                iState = mSZState ? 1 : 0;
                if (!mSZState)
                    isLocal = MSZ.MSZView.CheckLocal();
                measEnabled = MSZ.MSZView.GetModule("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxDXOvujXgK6tvUKtbGQzR9A=="/* PEN */);
                if (!mSZState && measEnabled < 0)
                    measEnabled = 0;
                date = DateTime.Now;
#if DEBUG
                bool bModuleEnabled = measEnabled > 0;
                LogMessage(Properties.Resources.LicenceInfo, mSZState, null, isLocal, bModuleEnabled, date, measEnabled);
#endif
            }
            catch (Exception ex)
            {
                LogMessage(Properties.Resources.ErrorInfo, ex);
            }
        }
    }
}
