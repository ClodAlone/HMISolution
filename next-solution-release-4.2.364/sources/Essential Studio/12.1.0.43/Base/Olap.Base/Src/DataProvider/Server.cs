//-------------------------------------------------------------------------------------------------
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
//-------------------------------------------------------------------------------------------------

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Syncfusion.Olap.DataProvider
//{
//    public class Server
//    {
//        string _serverName = "";
//        //string 
//        public Server(string datasource, bool isOfflineCube)
//        {
//            if (isOfflineCube)
//            {
//                IsOfflineCube = isOfflineCube;
//            }
//            else
//            {
//                IsOfflineCube = isOfflineCube;
//                _serverName = datasource;
//            }
//        }

//        public bool IsOfflineCube { get; set; }

//        public string ConnectionString
//        {
//            get
//            {
//                if (!IsOfflineCube)
//                    return "Data Source=" + _serverName;
//                else
//                    return GetLocalCubeConnectionString(_serverName);
//            }
//        }

//        public string GetServerConnectionString(string databaseName)
//        {
//            if (_serverName != "" && databaseName != "")
//            {
//                return string.Format("Data Source={0}; Initial Catalog={1};", _serverName, databaseName);
//            }
//            return null;
//        }

//        public string GetLocalCubeConnectionString(string cubeLocation)
//        {
//            if (cubeLocation != "")
//            {
//                return string.Format(@"Datasource='{0}'; Provider=msolap;", cubeLocation);
//            }
//            return null;
//        }


//        //public string GetConnectionString(string databaseName, string userName, string password)
//        //{
//        //    thr
//        //}

//        string CheckConnection()
//        {
//            Microsoft.AnalysisServices.Server masServer = new Microsoft.AnalysisServices.Server();
//            if (_serverName != "")
//            {
//                try
//                {
//                    masServer.Connect(this.ConnectionString);
//                    masServer.Disconnect();
//                    masServer = null;
//                }
//                catch (Exception ex)
//                {
//                    throw ex;
//                }
//            }
//            return "";
//        }

//        public List<string> GetDataBases()
//        {
//            Microsoft.AnalysisServices.Server masServer = new Microsoft.AnalysisServices.Server();
//            if (_serverName != "")
//            {
//                List<string> listOfDatatBases = new List<string>();
//                masServer.Connect(this.ConnectionString);
//                foreach (Microsoft.AnalysisServices.Database dataBase in masServer.Databases)
//                {
//                    listOfDatatBases.Add(dataBase.Name);
//                }
//                masServer.Disconnect();
//                masServer = null;
//                return listOfDatatBases;
//            }
//            return null;
//        }
//    }
//}
