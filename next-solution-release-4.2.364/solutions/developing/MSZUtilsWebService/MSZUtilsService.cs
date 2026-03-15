using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using MSZServiceCMS;
using Utilities.Logger;
using MSZUtilsWebService.ServerCMS;
using MSZUtilsServiceHelper;
using MSZFactory;
using System.Linq;
using DataReader;
using DataReader.Extensions;
using System.Data.Common;

namespace MSZUtilsWebService
{
    public partial class MSZUtilsService : ServiceBase
    {
        #region Declarations
        protected MSZUtilsServiceCSM serverCSM;
        object lockObject = new object();
        Thread serverThread = null;
        ManualResetEvent serverStopping;
        String _defaultDataProvider = string.Empty;
        String _defaultConnectionString = string.Empty;
        ComponentHost componentHost = new ComponentHost();
        PluginServices Plugins = new PluginServices();
        static string[] allowedRequests;
        int iDProdotto;
        string IDProdotto
        {
            get { return string.Format("[IdProdotto] = {0}", iDProdotto); }
        }

        string dbConnectionError;
        #endregion

        #region ctor
        public MSZUtilsService(string _title)
        {
            InitializeComponent();
            allowedRequests = Enum.GetNames(typeof(MSZUtilsServiceHelper.RequestType));

            title = _title;
            _defaultDataProvider = Properties.Settings.Default.DataProvider;
            var serviceUser = WPFUtilities.CryptString.CryptString.DecryptString("318dt4qSKTwFQOzGjkLKj1usGDB9p4vXReNrBTgPfv9QUbIQEZxl1jRmzpdxtzZ2");
            _defaultConnectionString = $"data source={Properties.Settings.Default.DataSource};{serviceUser};initial catalog={Properties.Settings.Default.InitialCatalog};Persist Security Info=true;";
            InitValues();
        }
        #endregion

        #region Virtual Methods
        public virtual void StartService(string[] args)
        {
            OnStartingService(new EventArgs());

            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);

            var appName = Assembly.GetEntryAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, appName))
            {
                try
                {
                    Mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        ex.Message, System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.License);
                }

                try
                {
                    serverCSM = new MSZUtilsServiceCSM(this);
                    serverCSM.HostServer();
                }
                catch (Exception ex)
                {
                    if (ex is AddressAlreadyInUseException ||
                        ex is System.Net.Sockets.SocketException ||
                        ex is CommunicationException)
                    {
                        Mutex.ReleaseMutex();

                        if (ex is AddressAlreadyInUseException)
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    Properties.Resources.ProjectAlreadyRunning,
                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.License);

                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show(Properties.Resources.ProjectAlreadyRunning, Properties.Resources.Server, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }
                        else
                        {
                            var communicationErrorMessage = String.Format(Properties.Resources.CommunciationErrorOnStartingService.Replace("'newline'", Environment.NewLine), ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    communicationErrorMessage,
                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.License);

                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show(communicationErrorMessage, Properties.Resources.Server, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }

                        System.Environment.Exit(-10);
                    }
                    else
                    {
                        Program.LogServer(ex.ToString());
                        throw ex;
                    }
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }
        #endregion

        #region Methods
        internal void StopService()
        {
            //using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.License, 
            //                                Properties.Resources.StoppingServer,
            //                                Properties.Resources.StoppedServer))
            {
                OnStoppingService(new EventArgs());
            }
        }
        private void InitValues()
        {
            try
            {
                dbConnectionError = string.Empty;
                //numSerial.Value = RangeStartNr;
                DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                        _defaultConnectionString,
                                                        $"EXECUTE [spLicenzeIdProdotto] '%{Properties.Settings.Default.ProductName}%'",
                                                        null, null,
                                                        null));
                foreach (DataRowView rowView in dataView)
                {
                    iDProdotto = (int)rowView["IdProdotto"];
                    break;
                }
            }
            catch (Exception ex)
            {
                dbConnectionError = ex.Message;
                Program.LogServer(ex.ToString());
            }
        }
        private MSZUWResponse GetSerialNumber(ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            string licSerial = string.Empty;
            try
            {
                DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                            _defaultConnectionString,
                                           $"EXECUTE [spLicenzeGetNextSerial] {iDProdotto}, {GetUserQueryParameters(userInfo)}",
                                                    null, null,
                                                    null));
                foreach (DataRowView rowView in dataView)
                {
                    var lid = rowView["NumeroLicenza"];
                    int licNumber;
                    if (lid != null && int.TryParse(lid.ToString(), out licNumber))
                    {
                        licSerial = (licNumber + 1).ToString();
                        res.Response.Add(GetString(licSerial));
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                res.Response.Add(GetString(RequestType.Error.ToString() + ex.GetType().Name));
                Program.LogServer(ex.ToString());
            }
            return res;
        }
        string sTrue = GetString(true.ToString());
        string sFalse = GetString(false.ToString());
        string sZero = GetString((0).ToString());

        const char optionsSeparator = '☺';
        const char lineSeparator = '↨';
        const char itemSeparator = '§';
        internal static string GetString(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : WPFUtilities.CryptString.CryptString.EncryptString(value);
        }
        internal static string DecryptString(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : WPFUtilities.CryptString.CryptString.DecryptString(value);
        }
        bool IsValidConnection
        {
            get
            {
                return !string.IsNullOrEmpty(_defaultDataProvider) && !string.IsNullOrEmpty(_defaultConnectionString);
            }
        }
        MSZUWRequest clientRequest;
        public MSZUWResponse Request(MSZUWRequest request)
        {
            clientRequest = request;

#if DEBUG
            Program.LogServer(request.ToString());
#endif
            MSZUWResponse res = new MSZUWResponse();
            try
            {
                if (!string.IsNullOrEmpty(dbConnectionError))
                {
                    res = new MSZUWResponse();
                    res.Response.Add(GetString(RequestType.Error.ToString() + dbConnectionError));
                }
                else
                {
                    string requestStringType = WPFUtilities.CryptString.CryptString.DecryptString(request.RequestType);
                    /* clickonce check new version client */
                    String clientReq = WPFUtilities.CryptString.CryptString.DecryptString(request.RequestID1);
                    var ret = clientReq.Count(x => x == '@');
                    if(ret != 3)
                    {
                        res = new MSZUWResponse();
                        res.Response.Add(GetString(RequestType.Illegal.ToString()));
                        MSZUWResponseArgs me = new MSZUWResponseArgs(res, clientRequest);
                        OnRequestService(me);
                        return res;
                    }
                    /* ---- */
                    ClientAutenticationCredentials userInfo = new ClientAutenticationCredentials(WPFUtilities.CryptString.CryptString.DecryptString(request.RequestID3));
                    RequestType requestType = GetRequest(requestStringType);
                    string mainrequest = WPFUtilities.CryptString.CryptString.DecryptString(request.Request);
                    if (IsValidConnection)
                    {
                        switch (requestType)
                        {
                            case MSZUtilsServiceHelper.RequestType.UserLogOn:
                                res = ValidateUser(userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.UserLogOff: 
                                LogOffUser(userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.GetLicTypeList:
                                res = GetLicTypeList(userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.GetAreeGeoIDList:
                                res = GetAreeGeoIDList(userInfo);
                                break;
                            case RequestType.GetSerialLogOptions:
                                res = GetSerialLogInfo(mainrequest, userInfo);
                                break;
                            case RequestType.GetSerialListInfo:
                                res = GetSerialListInfo(userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.GetNumericOptionsList:
                                if (string.IsNullOrEmpty(mainrequest))
                                    res = GetNumericOptionsList(userInfo);
                                else
                                    res = GetNumericOptionsList(mainrequest, userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.GetBoolOptionsList:
                                if(string.IsNullOrEmpty(mainrequest))
                                    res = GetBoolOptionsList(userInfo);
                                else
                                    res = GetBoolOptionsList(mainrequest, userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.GetCustomerList:
                                res = GetCustomerList(userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.GetSerialInfo:
                                res = GetSerialinfo(mainrequest, userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.GetSerialOptions:
                                res = GetSerialOptions(mainrequest, userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.InserOrUpdateSerialOptions:
                                res = InsertOrUpdateSerialOptions(mainrequest, userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.InserOrUpdateLicTypeOptions:
                                res = InsertOrUpdateLicTypeOptions(mainrequest, userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.InitSerial:
                                res = GetSerialNumber(userInfo);
                                break;
                            case MSZUtilsServiceHelper.RequestType.Illegal:
                                {
                                    res = new MSZUWResponse();
                                    res.Response.Add(GetString(RequestType.Illegal.ToString()));
                                }
                                break;
                            case MSZUtilsServiceHelper.RequestType.Error:
                                {
                                    res = new MSZUWResponse();
                                    res.Response.Add(GetString(RequestType.Error.ToString() + dbConnectionError));
                                }
                                break;
                            case MSZUtilsServiceHelper.RequestType.InsertOrUpdateCustomer:
                                res = InsertOrUpdateCustomer(mainrequest, userInfo);
                                break;
                            default:
                                {
                                    res = new MSZUWResponse();
                                    res.Response.Add(GetString(RequestType.Illegal.ToString()));
                                }
                                break;
                        }
                    }
                    else
                    {
                        res = new MSZUWResponse();
                        res.Response.Add(GetString(RequestType.Illegal.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        ex.Message, System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.License);
                Program.LogServer(ex.ToString());
            }

            MSZUWResponseArgs m = new MSZUWResponseArgs(res, clientRequest);
            OnRequestService(m);
            return res;
        }

        private MSZUWResponse InsertOrUpdateCustomer(string mainrequest, ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            try
            {
                Customer customer = Customer.FromString(mainrequest);
                List<IDbDataParameter> parameters = new List<IDbDataParameter>();
                AddParameters<string>(userInfo.UserName, "@Username", parameters);
                AddParameters<string>(userInfo.Password, "@Password", parameters);
                AddParameters<int>(customer.ID, "@ID", parameters);
                AddParameters<string>(customer.Code, "@CodiceCliente", parameters);
                AddParameters<string>(customer.ShortName, "@Descrizione", parameters);
                AddParameters<string>(customer.PIva, "@PIva", parameters);
                AddParameters<string>(customer.Area, "Area", parameters);

                DataView dataDefView = new DataView(GetDataSetSqlData(_defaultDataProvider,
                                                _defaultConnectionString,
                                                 "spLicenzeCustomerInsertOrUpdate", parameters));
                string ret = string.Empty;
                foreach (DataRowView rowdView in dataDefView)
                {
                    res.Response.Add(GetString(customer.ToString()));
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        ex.Message, System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.License);
                Program.LogServer(ex.ToString());
            }

            MSZUWResponseArgs m = new MSZUWResponseArgs(res, clientRequest);
            OnRequestService(m);
            return res;
        }

        private MSZUWResponse GetAreeGeoIDList(ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    $"EXECUTE [spLicenzeAreeGeoIdList] {GetUserQueryParameters(userInfo)}",
                                                          null, null, null));
            foreach (DataRowView rowView in dataView)
            {
                AreaGeoID ret = new AreaGeoID();

                ret.ID = (int)rowView["AreaGeoID"];
                ret.Area = rowView["AreaGeoDescr"].ToString();
                res.Response.Add(GetString(ret.ToString()));
            }

            return res;
        }

        private MSZUWResponse InsertOrUpdateLicTypeOptions(string mainrequest, ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            try
            {
                var values = mainrequest.Split(optionsSeparator);
                LicenceType licenceTypeInfo = LicenceType.FromString(values[0]);
                List<BoolOption> bOptionList = values[1] != null ? (from o in values[1].Split(lineSeparator)
                                                                    select BoolOption.FromString(o)).ToList() as List<BoolOption> :
                                                                    new List<BoolOption>();
                List<IntOption> iOptionList = values[2] != null ? (from o in values[2].Split(lineSeparator)
                                                                   select IntOption.FromString(o)).ToList() as List<IntOption> :
                                                                    new List<IntOption>();


                List<IDbDataParameter> parameters = new List<IDbDataParameter>();
                AddParameters<string>(userInfo.UserName, "@Username", parameters);
                AddParameters<string>(userInfo.Password, "@Password", parameters);
                AddParameters<int>(licenceTypeInfo.ID, "@IDTipoLicenza", parameters);
                AddParameters<int>(iDProdotto, "@IdProdotto", parameters);
                AddParameters<string>(licenceTypeInfo.Name, "@NomeTipoLicenza", parameters);
                AddParameters<string>(licenceTypeInfo.Description, "@Descrizione", parameters);
                AddParameters<bool>(licenceTypeInfo.Hidden, "@Hidden", parameters);

                StringBuilder queryOpzioni = new StringBuilder();

                if (bOptionList.Count > 0)
                {
                    foreach (BoolOption option in bOptionList)
                        queryOpzioni.Append($"{option.MszParameter}={option.Value.ToString()};");
                    if (iOptionList.Count == 0)
                        queryOpzioni.Remove(queryOpzioni.Length - 1, 1);
                }

                if (iOptionList.Count > 0)
                {
                    foreach (IntOption option in iOptionList)
                        queryOpzioni.Append($"{option.MszParameter}={option.Value.ToString()};");
                    queryOpzioni.Remove(queryOpzioni.Length - 1, 1);
                }

                if (queryOpzioni.Length > 0)
                    AddParameters<string>(queryOpzioni.ToString(), "@Opzioni", parameters);
                //queryString.Append($", @Opzioni ='{queryOpzioni.ToString()}'");

                DataView dataDefView = new DataView(GetDataSetSqlData(_defaultDataProvider,
                                                _defaultConnectionString,
                                                 "spLicenzeTipiInsertOrUpdate", parameters));
                string ret = string.Empty;
                foreach (DataRowView rowdView in dataDefView)
                {
                    res.Response.Add(GetString(licenceTypeInfo.ToString()));
                    break;
                }
            }
            catch (Exception ex)
            {
                return res;
            }

            return res;
        }

        private MSZUWResponse GetSerialLogInfo(string mainrequest, ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                            _defaultConnectionString,
                                            $"EXECUTE [spLicenzeGetHistoryLogSerialInfo] {mainrequest},  {GetUserQueryParameters(userInfo)}",
                                                          null, null, null));

            foreach (DataRowView rowdView in dataDefView)
            {

                LogLicenceInfo ret = new LogLicenceInfo();
                //ret.LicenceOption = rowdView["LicenzaColumnName"].ToString();
                //ret.OldValue = rowdView["ValorePrecedente"].ToString();
                ret.NewValue = rowdView["ValoreNuovo"].ToString();
                ret.ChangesUser = rowdView["ProgeaUserName"].ToString();
                DateTime date;
                if (DateTime.TryParse(rowdView["DataModifica"].ToString(), out date))
                    ret.DateOfChange = date;
                res.Response.Add(GetString(ret.ToString()));
            }
            return res;
        }

        private MSZUWResponse GetSerialListInfo(ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();

            DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                       _defaultConnectionString,
                                                       $"EXECUTE [spLicenzeListInfo] @IdProdotto = {iDProdotto}, {GetUserQueryParameters(userInfo)}",
                                                         null, null, null));
            foreach (DataRowView rowdView in dataDefView)
            {
                LicenceInfo ret = new LicenceInfo();
                ret.ID = (int)rowdView["IdLicenza"];
                ret.SerialNumber = (int)rowdView["NumeroLicenza"];
                ret.IDType = (int)rowdView["IdTipoLicenza"];
                ret.TypeDescr = rowdView["DescrizioneTipo"].ToString();
                ret.TypeName = rowdView["NomeTipoLicenza"].ToString();
                ret.LicDescr = rowdView["DescrizioneLicenza"].ToString();
                ret.CustomerID = (int)rowdView["AnagrafeID"];
                ret.CustomerCode = rowdView["CodiceCliente"].ToString();
                ret.CustomerDescription = rowdView["descrizion"].ToString();
                ret.RemovedCode = rowdView["RemovedCode"].ToString();
                ret.Order = rowdView["Ordine"].ToString();
                ret.SiteCode = rowdView["SiteCode"].ToString();
                ret.Bill = rowdView["Fattura"].ToString();
                ret.Price = rowdView["Prezzo"].ToString();
                ret.FinalCustomer = rowdView["ClienteFinale"].ToString();
                ret.Note = rowdView["Note"].ToString();
                ret.LastChangesUser = rowdView["UtenteIdUltimaMod"].ToString();
                ret.AreaGeoDescr = rowdView["AreaGeoDescr"].ToString();
                DateTime date;
                if (DateTime.TryParse(rowdView["DataScadenza"].ToString(), out date))
                    ret.SKExpiredDate = date;
                if (DateTime.TryParse(rowdView["DataUltimaMod"].ToString(), out date))
                    ret.LastChangesDate = date;
                res.Response.Add(GetString(ret.ToString()));
            }

            if (clientRequest.ContinuationPoint != deadBandValue || GetSizeOf(res) > Properties.Settings.Default.MaxSentRecordNumber)
                res = GetNextResponse(res);
            return res;
        }

        uint MaxTransactionsBeforeCommit = Properties.Settings.Default.MaxTransactionsBeforeCommit;
        private MSZUWResponse InsertOrUpdateSerialOptions(string mainrequest, ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            try
            {
                var values = mainrequest.Split(optionsSeparator);
                LicenceInfo licenceInfo = LicenceInfo.FromString(values[0]);
                List<BoolOption> bOptionList = values[1] != null ? (from o in values[1].Split(lineSeparator)
                                                                    select BoolOption.FromString(o)).ToList() as List<BoolOption> :
                                                                    new List<BoolOption>();
                List<IntOption> iOptionList = values[2] != null ? (from o in values[2].Split(lineSeparator)
                                                                   select IntOption.FromString(o)).ToList() as List<IntOption> :
                                                                    new List<IntOption>();


                List<IDbDataParameter> parameters = new List<IDbDataParameter>();
                AddParameters<string>(userInfo.UserName, "@Username", parameters);
                AddParameters<string>(userInfo.Password, "@Password", parameters);
                AddParameters<int>(licenceInfo.SerialNumber, "@NumeroLicenza", parameters);
                AddParameters<int>(licenceInfo.CustomerID, "@AnagrafeID", parameters);
                AddParameters<string>(licenceInfo.CustomerCode, "@CodiceCliente", parameters);
                AddParameters<string>(licenceInfo.RemovedCode, "@RemovedCode", parameters);
                AddParameters<string>(licenceInfo.SiteCode, "@SiteCode", parameters);
                AddParameters<string>(licenceInfo.SKUserName, "@UserNameGenerazioneSoftKey", parameters);
                AddParameters<string>(licenceInfo.Order, "@Ordine", parameters);
                AddParameters<string>(licenceInfo.Bill, "@Fattura", parameters);
                AddParameters<string>(licenceInfo.Price, "@Prezzo", parameters);
                AddParameters<string>(licenceInfo.FinalCustomer, "@ClienteFinale", parameters);
                AddParameters<string>(licenceInfo.Note, "@Note", parameters);
                AddParameters<string>(licenceInfo.Description, "@Descrizione", parameters);
                AddParameters<int>(iDProdotto, "@IdProdotto", parameters);
                AddParameters<int>(licenceInfo.IDType, "@IdTipoLicenza", parameters);
                AddParameters<int>((int)licenceInfo.IDLicType, "@TipoChiaveID", parameters);
                AddParameters<int>((int)licenceInfo.InstanceNumber, "@NumeroIstanzePerLicenza", parameters);

                if (licenceInfo.GenerateSWCode)
                {
                    var craddle = new Craddle();
                    string lic = craddle.Generate(0,
                                            licenceInfo.SiteCode,
                                            licenceInfo.SWCodeOptions);
                    licenceInfo.FileKey = Encoding.ASCII.GetBytes(lic);
                }

                AddParameters<byte[]>(licenceInfo.FileKey, "@FileKey", parameters, true);

                if (licenceInfo.SKGenerationDate != DateTime.MinValue)
                    AddParameters<DateTime>(licenceInfo.SKGenerationDate, "@DataGenerazioneSoftKey", parameters);
                if (licenceInfo.SKExpiredDate.CompareTo(DateTime.MinValue) == 0)
                    AddParameters<DateTime>(null, "@DataScadenza", parameters, true);
                else
                    AddParameters<DateTime>(licenceInfo.SKExpiredDate, "@DataScadenza", parameters);

                StringBuilder queryOpzioni = new StringBuilder();

                if(bOptionList.Count > 0)
                {
                    foreach (BoolOption option in bOptionList)
                        queryOpzioni.Append($"{option.MszParameter}={option.Value.ToString()};");
                    if (iOptionList.Count == 0)
                        queryOpzioni.Remove(queryOpzioni.Length - 1, 1);
                }

                if (iOptionList.Count > 0)
                {
                    foreach (IntOption option in iOptionList)
                        queryOpzioni.Append($"{option.MszParameter}={option.Value.ToString()};");
                    queryOpzioni.Remove(queryOpzioni.Length - 1, 1);
                }

                if (queryOpzioni.Length > 0)
                    AddParameters<string>(queryOpzioni.ToString(), "@Opzioni", parameters);
                //queryString.Append($", @Opzioni ='{queryOpzioni.ToString()}'");
                
                DataView dataDefView = new DataView(GetDataSetSqlData(_defaultDataProvider,
                                                _defaultConnectionString,
                                                 "spLicenzeInsertOrUpdate", parameters));
                string ret = string.Empty;
                foreach (DataRowView rowdView in dataDefView)
                {
                    res.Response.Add(GetString(licenceInfo.ToString()));
                    break;
                }
            }
            catch (Exception ex)
            {
                return res;
            }

            return res;
        }
        private void AddParameters<T>(object parameterValue, string parameterName, List<IDbDataParameter> parameters, bool useNullValue = false)
        {
            var keyparameter = GetParameters<T>(parameterValue, parameterName, useNullValue);
            if (keyparameter != null || useNullValue)
                parameters.Add(keyparameter);
        }

        private DbParameter GetParameters<T>(object parameterValue, string parameterName, bool useNullValue = false)
        {
            if (parameterValue != null)
            {
                var keyparameter = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                keyparameter.ParameterName = parameterName;
                keyparameter.DbType = (typeof(T)).ToDbType();
                keyparameter.Value = TypeExtensions.ChangeType(parameterValue, keyparameter.DbType);
                return keyparameter;
            }
            else if(useNullValue)
            {
                var keyparameter = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                keyparameter.ParameterName = parameterName;
                keyparameter.DbType = (typeof(T)).ToDbType();
                keyparameter.Value = DBNull.Value;
                return keyparameter;
            }

            return null;
        }

        static DataTable GetDataSetSqlData(string dataprovider,
                                                    string connectionstring,
                                                    string select,
                                                    List<IDbDataParameter> parameters)
        {
            using (var connection = DataReader.DataReader.CreateDbConnection(dataprovider, connectionstring))
            {
                connection.Open();
                var dbdapater = DataReader.DataReader.CreateDbDataAdapter(dataprovider);
                dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(dataprovider);
                dbdapater.SelectCommand.Connection = connection;
                dbdapater.SelectCommand.CommandType = CommandType.StoredProcedure;
                parameters.ForEach(p =>dbdapater.SelectCommand.Parameters.Add(p));
                dbdapater.SelectCommand.CommandText = select;
                var ret = new DataSet();
                try
                {
                    dbdapater.Fill(ret);
                }
                catch (Exception ex)
                {
                }

                if (ret.Tables.Count > 0)
                    return ret.Tables[0];
                return null;
            }
        }
        private MSZUWResponse GetSerialOptions(string mainrequest, ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                            _defaultConnectionString,
                                             $"EXECUTE [spLicenzeGetSerialOptions] {mainrequest},  {GetUserQueryParameters(userInfo)}",
                                                          null, null, null));

            string id;
            string valore;
            string ret = string.Empty;
            foreach (DataRowView rowdView in dataDefView)
            {
                id = rowdView["IdOpzione"].ToString();
                valore = rowdView["Valore"].ToString();
                ret = $"{id}{itemSeparator}{valore}";
                res.Response.Add(GetString(ret));
            }
            return res;
        }

        private MSZUWResponse GetSerialinfo(string mainrequest, ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                            _defaultConnectionString,
                                            $"EXECUTE [spLicenzeGetSerialInfo] {mainrequest},  {GetUserQueryParameters(userInfo)}",
                                                          null, null, null));

            foreach (DataRowView rowdView in dataDefView)
            {

                LicenceInfo ret = new LicenceInfo();
                ret.ID = (int)rowdView["IdLicenza"];
                ret.IDType = (int)rowdView["IdTipoLicenza"];

                var tipo = rowdView["TipoChiaveID"];
                int licType;
                if (tipo != null && int.TryParse(tipo.ToString(), out licType))
                {
                    ret.IDLicType = (LicType)licType;
                }
                DateTime date;
                if (DateTime.TryParse(rowdView["DataScadenza"]?.ToString(), out date))
                    ret.SKExpiredDate = date; 

                ret.LicDescr = rowdView["Descrizione"]?.ToString(); 
                ret.CustomerID = (int)rowdView["AnagrafeID"];
                ret.CustomerCode = rowdView["CodiceCliente"]?.ToString();
                ret.RemovedCode = rowdView["RemovedCode"]?.ToString();
                ret.Order = rowdView["Ordine"]?.ToString();
                ret.SiteCode = rowdView["SiteCode"]?.ToString();
                ret.Bill = rowdView["Fattura"]?.ToString();
                ret.Price = rowdView["Prezzo"]?.ToString();
                ret.FinalCustomer = rowdView["ClienteFinale"]?.ToString();
                ret.Note = rowdView["Note"]?.ToString();
                ret.InstanceNumber = (int)rowdView["NumeroIstanzePerLicenza"];
                ret.ID = (int)rowdView["IdLicenza"];
                if(!(rowdView["FileKey"] is System.DBNull))
                    ret.FileKey = (byte[])rowdView["FileKey"];
                res.Response.Add(GetString(ret.ToString()));
            }
            return res;
        }

        private MSZUWResponse GetSwLicense(string mainrequest)
        {
            MSZUWResponse res = new MSZUWResponse();
            try
            {
                uint type = uint.Parse(mainrequest?.Split(itemSeparator)[0]);
                string param = mainrequest?.Split(itemSeparator)[1];
                string siteCode = mainrequest?.Split(itemSeparator)[2];
                string serial = mainrequest?.Split(itemSeparator)[3];
                var craddle = new Craddle();
                string lic = craddle.Generate(type,
                                    siteCode,
                                    param);
                res.Response.Add(lic);
            }
            catch (Exception ex)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        ex.Message, System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.License);
                Program.LogServer(ex.ToString());
            }
            return res;
        }
        const int deadBandValue = -1;
        private MSZUWResponse GetCustomerList(ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            string key = $"{clientRequest.RequestID1}{clientRequest.RequestID2}";

            DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                       _defaultConnectionString,
                                                       $"EXECUTE [spLicenzeGetCustomerList]  {GetUserQueryParameters(userInfo)}",
                                                         null, null, null));
            foreach (DataRowView rowView in dataDefView)
            {
                Customer ret = new Customer();
                ret.ID = (int)rowView["AnagrafeID"];
                ret.PIva = rowView["partiva"].ToString();
                ret.AreaGeoID = (int)rowView["areageoid"];
                ret.Area = rowView["area"].ToString();
                ret.Editable = bool.Parse(rowView["editable"].ToString());
                ret.Name = string.Format("{0} ({1}) - {2}", rowView["descrizion"].ToString(), rowView["prov"].ToString(), rowView["codice"].ToString());
                ret.ShortName = rowView["descrizion"].ToString();
                ret.Code = rowView["codice"].ToString();
                res.Response.Add(GetString(ret.ToString()));
            }

            if (clientRequest.ContinuationPoint != deadBandValue || GetSizeOf(res) > Properties.Settings.Default.MaxSentRecordNumber)
                res = GetNextResponse(res);
            return res;
        }

        private MSZUWResponse GetNextResponse(MSZUWResponse res)
        {
            var bufferList = res.Response;
            var continuationPoint = clientRequest.ContinuationPoint;
            var count = Properties.Settings.Default.MaxSentRecordNumber - 1;

            if (continuationPoint == -1)
                continuationPoint = 0;

            if (continuationPoint >= bufferList.Count - 1 || bufferList.Count == 0)
                return new MSZUWResponse();
            if (continuationPoint + count >= bufferList.Count)
                count = bufferList.Count - continuationPoint;

            var returnList = bufferList.GetRange(continuationPoint, count);
            return new MSZUWResponse() { Response = new List<string>(returnList), ContinuationPoint = continuationPoint + count };
        }

        private long GetSizeOf(MSZUWResponse res)
        {
            return res.Response.Count;
        }

        private MSZUWResponse GetNumericOptionsList(ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    $"EXECUTE [spLicenzeOpzioniNum] {iDProdotto},  {GetUserQueryParameters(userInfo)}",
                                                          null, null, null));

            foreach (DataRowView rowdView in dataDefView)
            {
                IntOption ret = new IntOption();
                ret.ID = (int)rowdView["IdOpzione"];
                ret.Name = rowdView["NomeOpzione"].ToString();
                ret.MszParameter = rowdView["ParametroMsz"].ToString();
                ret.Enabled = bool.Parse((rowdView["Writable"]).ToString());
                ret.Value = 0;
                ret.AddInfo = rowdView["AddInfo"].ToString();
                ret.OrderInfo = (int)rowdView["OrderHW"];
                res.Response.Add(GetString(ret.ToString()));
            }
            return res;
        }

        private MSZUWResponse GetBoolOptionsList(ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    $"EXECUTE [spLicenzeOpzioniBool] {iDProdotto},  {GetUserQueryParameters(userInfo)}",
                                                          null, null, null));

            foreach (DataRowView rowdView in dataDefView)
            {
                BoolOption ret = new BoolOption();
                ret.ID = (int)rowdView["IdOpzione"];
                ret.Name = rowdView["NomeOpzione"].ToString();
                ret.MszParameter = rowdView["ParametroMsz"].ToString();
                ret.Enabled = bool.Parse((rowdView["Writable"]).ToString());
                ret.Value = !ret.Enabled;
                ret.AddInfo = rowdView["AddInfo"].ToString();
                ret.OrderInfo = (int)rowdView["OrderHW"];
                res.Response.Add(GetString(ret.ToString()));
            }
            return res;
        }

        private MSZUWResponse GetNumericOptionsList(string request, ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                _defaultConnectionString,
                                                $"EXECUTE [spLicenzeDefaultValNum] {request},  {GetUserQueryParameters(userInfo)}",
                                                null, null, null));

            foreach (DataRowView rowdView in dataDefView)
            {
                IntOption ret = new IntOption();
                ret.ID = (int)rowdView["IdOpzione"];
                ret.Name = rowdView["NomeOpzione"].ToString();
                ret.MszParameter = rowdView["ParametroMsz"].ToString();
                ret.Enabled = bool.Parse((rowdView["Writable"]).ToString());
                ret.Value = uint.Parse(rowdView["Valore"].ToString());
                ret.OrderInfo = (int)rowdView["OrderHW"];
                res.Response.Add(GetString(ret.ToString()));
            }
            return res;
        }

        private MSZUWResponse GetBoolOptionsList(string request,ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                _defaultConnectionString,
                                                $"EXECUTE [spLicenzeDefaultValBool] {request},  {GetUserQueryParameters(userInfo)}",
                                                null, null, null));

            foreach (DataRowView rowdView in dataDefView)
            {
                BoolOption ret = new BoolOption();
                ret.ID = (int)rowdView["IdOpzione"];
                ret.Name = rowdView["NomeOpzione"].ToString();
                ret.MszParameter = rowdView["ParametroMsz"].ToString();
                ret.Enabled = bool.Parse(rowdView["Writable"].ToString());
                ret.Value = bool.Parse(rowdView["Valore"].ToString());
                ret.OrderInfo = (int)rowdView["OrderHW"];
                res.Response.Add(GetString(ret.ToString()));
            }
            return res;
        }

        private MSZUWResponse GetLicTypeList(ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();
            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    $"EXECUTE [spLicenzeTipiLicenza] {iDProdotto},  {GetUserQueryParameters(userInfo)}",
                                                          null, null, null));
            foreach (DataRowView rowView in dataView)
            {
                LicenceType ret = new LicenceType();

                ret.ID = (int)rowView["IdTipoLicenza"];
                ret.Name = rowView["NomeTipoLicenza"].ToString();
                ret.Description = rowView["Descrizione"].ToString();
                ret.Hidden = bool.Parse(rowView["Hidden"].ToString());
                res.Response.Add(GetString(ret.ToString()));
            }

            return res;
        }

        string GetUserQueryParameters(ClientAutenticationCredentials userInfo)
        {
            return $"@Username = '{userInfo.UserName}', @Password = '{userInfo.Password}'";
        }
        private void LogOffUser(ClientAutenticationCredentials userInfo)
        {
            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                          _defaultConnectionString,
                                                          $"EXECUTE [spLicenzeLogOffUtente] {GetUserQueryParameters(userInfo)}",
                                                          null, null, null));
            if (dataView.Count != 0)
            {
                //CallLogOffProcedure();
            }
        }
        private MSZUWResponse ValidateUser(ClientAutenticationCredentials userInfo)
        {
            MSZUWResponse res = new MSZUWResponse();

            var ret = ValidateDBUser(userInfo);
            if (ret != -1)
            {
                userInfo.UserType = ValidateDBUserType(userInfo);
                res.Response.Add(GetString($"{ret}|{userInfo.UserType}"));
            }
            else
                res.Response.Add(GetString(RequestType.Illegal.ToString()));
            return res;
        }

        private int ValidateDBUser(ClientAutenticationCredentials userInfo)
        {
            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    $"EXECUTE [spLicenzeLoginUtente] {GetUserQueryParameters(userInfo)}",
                                                    null, null, null));
            int ret = -1;
            if (dataView.Count == 0)
            {
                userInfo.UserID = ret;
                return ret;
            }
            else
                foreach (DataRowView rowView in dataView)
                {
                    ret = (int)rowView["UtenteID"];
                    break;
                }

            userInfo.UserID = ret;
            return ret;
        }
        private int ValidateDBUserType(ClientAutenticationCredentials userInfo)
        {
            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    $"EXECUTE [spLicenzeTipoUtente] {GetUserQueryParameters(userInfo)}",
                                                    null, null, null));
            int ret = -1;
            if (dataView.Count == 0)
            {
                userInfo.UserType = ret;
                return ret;
            }
            else
                foreach (DataRowView rowView in dataView)
                {
                    ret = (int)rowView["UtenteTypeID"];
                    break;
                }

            userInfo.UserType = ret;
            return ret;
        }


        private RequestType GetRequest(string v)
        {
            RequestType requestType = (RequestType)Enum.Parse(typeof(RequestType), v);
            if (Enum.IsDefined(typeof(RequestType), requestType))
                return requestType;
            else
                return RequestType.Illegal;
        }
        #endregion

        #region Override Methods

        protected override void OnStart(string[] args)
        {
            lock (lockObject)
            {
                if (serverThread == null)
                {
                    serverStopping = new ManualResetEvent(false);
                    serverThread = new Thread(() =>
                    {
                        if (args.Length == 0)
                            args = Environment.GetCommandLineArgs();

                        StartService(args);
                        IsRunningAsService = true;

                        if (serverStopping.WaitOne())
                        {
                            StopService();
                            IsRunningAsService = false;
                        }
                    });
                    serverThread.Name = ServiceName;
                    serverThread.IsBackground = true;
                }
                serverThread.Start();
            }
        }

        protected override void OnStop()
        {
            Thread thread = null;
            lock (lockObject)
            {
                thread = serverThread;
                if (serverStopping != null)
                    serverStopping.Set();
            }

            if (thread != null)
            {
                while (serverThread.IsAlive && !serverThread.Join(2000))
                {
                    RequestAdditionalTime(5000);
                }
            }

            lock (lockObject)
            {
                serverThread = null;
                if (serverStopping != null)
                {
                    serverStopping.Dispose();
                    serverStopping = null;
                }
            }
        }

        #endregion

        #region Properties

        bool isStarted;
        public bool IsStarted
        {
            get
            {
                return isStarted;
            }
        }

        private bool _IsRunningAsService;
        public bool IsRunningAsService
        {
            get { return _IsRunningAsService; }
            internal set
            {
                _IsRunningAsService = value;
            }
        }

        protected string ProjectConnectionString { get; private set; }

        String title;
        public String Title
        {
            get
            {
                return title;
            }
        }

        int serverPort;
        public int ServerPort
        {
            get
            {
                return serverPort;
            }
        }

        bool communicationStatus;
        public bool CommunicationStatus
        {
            get
            {
                return communicationStatus;
            }
        }

        String statusText;
        public String StatusText
        {
            get
            {
                return statusText;
            }
        }

        String _BalloonMessage = String.Empty;
        public String BalloonMessage
        {
            get { return _BalloonMessage; }
            set { _BalloonMessage = value; }
        }

        System.Windows.Forms.ToolTipIcon _BalloonIcon = System.Windows.Forms.ToolTipIcon.None;
        public System.Windows.Forms.ToolTipIcon BalloonIcon
        {
            get { return _BalloonIcon; }
            set { _BalloonIcon = value; }
        }
        #endregion

        #region Events

        public event EventHandler StartingService;
        #region OnStartingService
        /// <summary>
        /// Triggers the StartingService event.
        /// </summary>
        public virtual void OnStartingService(EventArgs ea)
        {
            var t = StartingService;
            if (t != null)
                t(this, ea);
        }

        #endregion

        public event EventHandler StoppingService;
        #region OnStoppingService
        /// <summary>
        /// Triggers the StoppingService event.
        /// </summary>
        public virtual void OnStoppingService(EventArgs ea)
        {
            var t = StoppingService;
            if (t != null)
                t(this, ea);
        }
       
        public event EventHandler<MSZUWResponseArgs> RequestService;
        private void OnRequestService(MSZUWResponseArgs e)
        {
            EventHandler<MSZUWResponseArgs> temp = RequestService;
            if (temp != null)
                temp(null, e);
        }
       
        #endregion
        #endregion
    }
}
