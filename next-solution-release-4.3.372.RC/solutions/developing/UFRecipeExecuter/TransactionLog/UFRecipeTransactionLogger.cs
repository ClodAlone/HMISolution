using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using XpoHelpers;
using DevExpress.XtraRichEdit.Model;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using UFRecipeSettings.Documents;
using UFRecipeSettings.Helpers;
using System.Xml.Linq;

namespace UFRecipeExecuter
{
    public enum TransactionLogEventType
    {
        Added,
        Deleted,
        Modified,
        Unknown
    }

    public class UFRecipeTransactionLogger : IDisposable
    {
        #region Declarations
        readonly String xpoConnectionString;
        readonly TimeSpan TransactionLogMaxAge;
        bool bDisposed;
        readonly List<String> corruptFileNames = new List<String>();
        readonly String recipeFolder;
        #endregion

        #region Constructors
        public UFRecipeTransactionLogger(String connectionString, TimeSpan transactionLogMaxAge)
        {
            xpoConnectionString = connectionString;
            TransactionLogMaxAge = transactionLogMaxAge;
        }
        #endregion

        #region Persistance
        IDataLayer CreateDataLayer(DataSet dataChanges, out IDisposable[] objectsToDisposeOnDisconnect, bool retry = true, string filename = null)
        {
            
            string file = null;
            try
            {
                file = XpoHelper.GetDataSourceFilePath(xpoConnectionString);
                if (file != null && corruptFileNames.Contains(file))
                {
                    corruptFileNames.Remove(file);
                    if (File.Exists(file))
                    {
                        File.Copy(file, file + ".bak", true);
                        File.Delete(file);
                    }
                }

                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                dict.GetDataStoreSchema(typeof(UFRecipeTransactionLogItem).Assembly);
                return XpoDefault.GetDataLayer(xpoConnectionString, dict, AutoCreateOption.DatabaseAndSchema, out objectsToDisposeOnDisconnect);
            }
            catch (PathTooLongException e)
            {
                if (filename == null && xpoConnectionString != null)
                {
                    var index = xpoConnectionString.LastIndexOf('.');
                    if (index != -1)
                        return CreateDataLayer(dataChanges, out objectsToDisposeOnDisconnect, filename: xpoConnectionString.Substring(index + 1));
                }
                else
                {
                    var message = String.Format(Properties.Resources.ErrorCreatingDataLayer, e.Message);
                    var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                    Console.WriteLine(logMessage);
                }
            }
            catch (Exception e)
            {
                if (file != null && File.Exists(file))
                {
                    try
                    {
                        if (e.InnerException != null && e.InnerException is System.UnauthorizedAccessException)
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            if (retry)
                                return CreateDataLayer(dataChanges, out objectsToDisposeOnDisconnect, false);
                        }
                        File.Copy(file, file + ".bak", true);
                        File.Delete(file);
                    }
                    catch
                    {
                        corruptFileNames.Add(file);
                    }
                }
                var message = String.Format(Properties.Resources.ErrorCreatingDataLayer, xpoConnectionString, e.InnerException != null ? e.InnerException.Message : e.Message);
                var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                Console.WriteLine(logMessage);
            }

            objectsToDisposeOnDisconnect = new IDisposable[0];

            return null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method to write, through a session, the data in the transaction log
        /// </summary>
        /// <param name="dataChanges">Modified DataSet</param>
        /// <param name="guid"></param>
        /// <param name="recipeDocument"></param>
        /// <param name="eventType">Tyoe of action recorded in the transaction log</param>
        public void UpdateTransactionLog(DataSet dataChanges, UFRecipeDocument recipeDocument, TransactionLogEventType eventType)
        {
            //In case of TransactionLogMaxAge equal to zero deletion never executed
            if (TransactionLogMaxAge != TimeSpan.Zero)
                CleanOldData(dataChanges);

            IDisposable[] objectsToDisposeOnDisconnect;
            var idl = CreateDataLayer(dataChanges, out objectsToDisposeOnDisconnect);
            if (idl != null)
            {
                var xmlDataSet = new System.Text.StringBuilder();
                using (var dataSet = dataChanges)
                {
                    System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings()
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        OmitXmlDeclaration = true,
                        Indent = false,
                        CloseOutput = true,
                        CheckCharacters = false
                    };
                    

                    if (dataChanges != null)
                    {
                        using (var xmlWriter = System.Xml.XmlWriter.Create(xmlDataSet, settings))
                        {
                            dataChanges.WriteXml(xmlWriter, XmlWriteMode.IgnoreSchema);
                        }
                    }
                }

                try
                {
                    using (var ufw = new UnitOfWork(idl))
                    {

                        UFRecipeTransactionLogItem item = new UFRecipeTransactionLogItem(ufw);
                                
                        item.EventType = eventType;
                        item.RecipeName = recipeDocument.Title;
                        item.XmlDataSet = xmlDataSet.ToString();
                        item.EventDateTime = DateTime.Now;
                        item.EventDateTimeUtc = DateTime.UtcNow;

                        ufw.CommitChanges();
                    }
                }
                catch (Exception ex)
                {
                    var message = String.Format(Properties.Resources.ErrorOnSaving, dataChanges.DataSetName, ex.Message);
                    var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                    Console.WriteLine(logMessage);
                }
                finally
                {
                    idl.Dispose();
                    foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                        obj.Dispose();
                }
            }
        }

        /// <summary>
        /// Delete data older than TransactionLogMaxAge
        /// </summary>
        /// <param name="dataChanges">Modified Dataset</param>
        private void CleanOldData(DataSet dataChanges)
        {
            IDisposable[] objectsToDisposeOnDisconnect;
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(UFRecipeTransactionLogItem).Assembly);
            using (var idl = XpoDefault.GetDataLayer(xpoConnectionString, dict, AutoCreateOption.DatabaseAndSchema, out objectsToDisposeOnDisconnect))
            {
                if(idl != null)
                {
                    try
                    {
                        using (var ufw = new UnitOfWork(idl))
                        {
                            var itemsToDelete = from logItem in new XPQuery<UFRecipeTransactionLogItem>(ufw, true)/*.AsParallel()*/
                                                where logItem.EventDateTimeUtc < DateTime.UtcNow - TransactionLogMaxAge
                                                select logItem;

                            foreach (UFRecipeTransactionLogItem item in itemsToDelete)
                            {
                                ufw.Delete(item);
                                ufw.CommitChanges();
                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        var message = String.Format(Properties.Resources.ErrorOnCleaningTransactionLog, dataChanges.DataSetName, ex.Message);
                        var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                        Console.WriteLine(logMessage);
                    }
                    finally
                    {
                        idl.Dispose();
                        foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                            obj.Dispose();
                    }
                }
                
            }

        }

        /// <summary>
        /// Read the transaction log starting from the UTC DateTime indicated through a DevExpress session
        /// </summary>
        /// <param name="dtUTC"></param>
        /// <returns>List of UFRecipeTransactionLogItem</returns>
        public List<UFRecipeTransactionLogItem> ReadTransactionLog(DateTime dtUTC, string RecipeName)
        {
            IDisposable[] objectsToDisposeOnDisconnect;
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(UFRecipeTransactionLogItem).Assembly);
            List<UFRecipeTransactionLogItem> retList = new List<UFRecipeTransactionLogItem>();
            using (var idl = XpoDefault.GetDataLayer(xpoConnectionString, dict, AutoCreateOption.DatabaseAndSchema, out objectsToDisposeOnDisconnect))
            {
                if (idl != null)
                {
                    try
                    {
                        using (var ufw = new UnitOfWork(idl))
                        {
                            retList = (from logItem in new XPQuery<UFRecipeTransactionLogItem>(ufw, true)
                                                where logItem.EventDateTimeUtc > dtUTC && logItem.RecipeName == RecipeName
                                                select logItem).ToList();

                        }
                    }
                    catch (Exception ex)
                    {
                        var message = String.Format(Properties.Resources.ErrorReadingTransactionLog, ex.Message);
                        var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                        Console.WriteLine(logMessage);
                    }
                    finally
                    {
                        idl.Dispose();
                        foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                            obj.Dispose();
                    }
                }
                return retList;
            }
        }
        #endregion

        #region IDisposable
        public void Dispose() 
        {
            if (bDisposed)
                return;
            bDisposed = true;
        }
        #endregion
    }
}
