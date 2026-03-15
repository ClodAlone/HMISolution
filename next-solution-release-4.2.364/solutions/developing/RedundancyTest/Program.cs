using Opc.Ua.Utilities;
using RedundancyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyTest
{
    class Program
    {
        static bool bExitMode = false;
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the name of the servers, leave blank and press enter to start");
            var server = new List<String>();
            while(true)
            {
                var line = Console.ReadLine();
                if (String.IsNullOrEmpty(line))
                    break;
                server.Add(line);
            }

            Console.WriteLine("Starting the redundancy service");
            using (var redundancy = new ActiveServerManager())
            {
                redundancy.ArrayServers = server.ToArray();
                /*
                redundancy.HistorianConnection = ActiveServerManager.NormalizeConnectionString(@"XpoProvider=MSSqlServer;data source=(local)\SQLEXPRESS;integrated security=SSPI;initial catalog=RedundancyProject_IOServer");
                redundancy.EventConnection = ActiveServerManager.NormalizeConnectionString(@"XpoProvider=MSSqlServer;data source=(local)\SQLEXPRESS;integrated security=SSPI;initial catalog=RedundancyProject_IOServer");
                redundancy.MapDataLoggerConnection = new Dictionary<String, DataConnectionParameters>();
                redundancy.MapDataLoggerConnection.Add("DL_ModBusTCPIP_01", new DataConnectionParameters()
                {
                    DataSourceName = "DL_ModBusTCPIP_01",
                    DataProvider = "System.Data.SqlClient",
                    Connection = @"data source=MaurizioZ-VM1\SQLEXPRESS;integrated security=SSPI;initial catalog=RedundancyProject_IOServer"
                });
                */

                redundancy.ActiveServer += (o, e) =>
                    {
                        Console.WriteLine("THIS SERVER IS THE ACTIVE SERVER");
                    };
                redundancy.InactiveServer += (o, e) =>
                    {
                        Console.WriteLine("this server is NOT the active server now");
                    };
                redundancy.ActiveServerChanged += (o, e) =>
                {
                    Console.WriteLine("a new server was active now, new server '{0}'", e.activeServer);
                };
                redundancy.AliveServerListChanged += (o, e) =>
                {
                    Console.WriteLine("the alive servers list has been changed, new value '{0}'", e.aliveServers);
                };
                redundancy.GettingAllLiveData += (o, e) =>
                    {
                        Console.WriteLine("Getting all live data");
                    };
                //redundancy.PreparingSynchronizeHistoryData += (o, e) =>
                //    {
                //        e.CanExecute = true;
                //        Console.WriteLine("Preparing synchronize history datas");
                //    };
                redundancy.SynchronizeHistoryData += (o, e) =>
                    {
                        Console.WriteLine("request of history data synchronization from connection '{0}' - '{1}', endTime = '{2}'", 
                            e.historySettings.EventDefaultConnection, e.historySettings.HistorianDefaultConnection, e.endTime);
                    };

                redundancy.SynchronizeAllLiveData += (o, e) =>
                    {
                        Console.WriteLine("request of Synchronize All LiveData from connection '{0}'",
                            e.hostName);
                    };
                redundancy.ErrorOccured += (o, e) =>
                    {
                        Console.WriteLine("An error occured : {0}", e.Ex.Message);
                    };

                redundancy.Online = true;

                System.Threading.Thread thread = new System.Threading.Thread((o) =>
                {
                    while (true)
                    {
                        if (bExitMode)
                            break;

                        if (!redundancy.IsActiveServer)
                        {
                            var collection = new WrappedDataValueCollection();
                            collection.Add(new WrappedDataValue(new Opc.Ua.Variant(1), Opc.Ua.StatusCodes.Good, DateTime.Now));
                            redundancy.SendWritingData(new ChangedTags() { NodeId = Guid.NewGuid().ToString(), DataValues = collection });
                        }
                        System.Threading.Thread.Sleep(10);
                    }
                });
                thread.Start();

                Console.WriteLine("redundancy service started, press enter to terminate");
                Console.ReadLine();
                Console.WriteLine("redundancy service Terminating...");
                bExitMode = true;
                thread.Join();
            }
            Console.WriteLine("redundancy service Terminated");
        }
    }
}
