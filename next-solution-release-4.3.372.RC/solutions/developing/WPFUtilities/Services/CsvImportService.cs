using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UFInterfaces;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509.Qualified;
using CsvHelper.Configuration;

namespace WPFUtilities.Services
{
    public class CsvImportService<T, TMap>: ICsvImportService<T, TMap> where T : class
        where TMap : ClassMap<T>
    {
        public List<T> ImportCsv(string filePath, string delimiter)
        {
            List<T> dataList = new List<T>();
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    using (var csv = new CsvHelper.CsvReader(reader))
                    {                        
                        csv.Configuration.Delimiter = delimiter;
                        var classMapDataValue = csv.Configuration.RegisterClassMap<TMap>();

                        while (csv.Read())
                        {
                            if (csv.Context.Row == 0)
                                continue;
                            var record = csv.GetRecord<T>();

                            dataList.Add(record);
                        }
                    }
                }
            }
            catch(FileNotFoundException fileNotFoundEx)
            {
#if !NET_STANDARD
                throw new Exception(String.Format(Properties.Resources.FileNotFoundErrorMessage, fileNotFoundEx.FileName));
#endif
            }            
            catch (CsvHelper.HeaderValidationException headerValEx)
            {
#if !NET_STANDARD
                throw new Exception(String.Format(Properties.Resources.ValidateColumnsErrorMessage, String.Concat(headerValEx.HeaderNames)));
#endif
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return dataList;
        }
    }
}
