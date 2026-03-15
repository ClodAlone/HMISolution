#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Tables;
namespace Syncfusion.XlsIO.Implementation
{
   public class QueryTableImpl 
    {

       public QueryTableImpl(IApplication application, object parent, ExternalConnection ExternalConnection)
       {
           this.m_externalConnection = ExternalConnection;
           if (ExternalConnection.OLEDBConnection != null)
               m_dataBaseProperty = ExternalConnection.OLEDBConnection as DataBaseProperty;
           else if (ExternalConnection.ODBCConnection != null)
               m_dataBaseProperty = ExternalConnection.ODBCConnection;
       }    

       /// <summary>
       /// Represent the external connection
       /// </summary>
       private ExternalConnection m_externalConnection;
       /// <summary>
       /// It's define the database property
       /// </summary>
       private DataBaseProperty m_dataBaseProperty;
       /// <summary>
       /// Represent the column width
       /// </summary>
       private bool m_adjustColumnWidth = true;
       /// <summary>
       /// It's define the connection availability
       /// </summary>
       private bool m_isDeleted = false;        
       private string m_name;
       /// <summary>
       /// Represent the Connection for the Specified QueryTable
       /// </summary>
       public ExternalConnection ExternalConnection
       {
           get { return m_externalConnection; }
           //set { m_externalConnection = value; }
       }
       /// <summary>
       /// The Query Table is automatically updated when open the workbook
       /// </summary>
       public bool RefreshOnFileOpen
       {
           get
           {
               if (m_dataBaseProperty == null)
                   throw new ArgumentNullException("Connection is not valid");
             return m_dataBaseProperty.RefreshOnFileOpen;     
           }
           set
           {
               if (m_dataBaseProperty == null)
                   throw new ArgumentNullException("Connection is not valid");
               m_dataBaseProperty.RefreshOnFileOpen = value;
           }
       }
       /// <summary>
       /// Command Type for specified connection
       /// </summary>
       public ExcelCommandType CommandType
       {
           get
           {
               if (m_dataBaseProperty == null)
                   throw new ArgumentNullException("Connection is not valid");

               return m_dataBaseProperty.CommandType;
           }
           set
           {
               if (m_dataBaseProperty == null)
                   throw new ArgumentNullException("Connection is not valid");

               m_dataBaseProperty.CommandType = value;
           }
       }
       /// <summary>
       /// Represent the command string for the specified data source
       /// </summary>
       public object CommandText
       {
           get
           {
               if (m_dataBaseProperty == null )
                   throw new ArgumentNullException("Connection is Deleted");

               return m_dataBaseProperty.CommandText;

           }
           set
           {
               if (m_dataBaseProperty == null || ConnectionDeleted)
                   throw new ArgumentNullException("Connection is not valid");

               m_dataBaseProperty.CommandText = value;
           }
       }
       /// <summary>
       /// Define connection information
       /// </summary>
       public object ConnectionString
       {
           get
           {
               if (m_dataBaseProperty == null )
                   throw new ArgumentNullException("Connection is not valid");

               return m_dataBaseProperty.ConnectionString;

           }
           set
           {
               if (m_dataBaseProperty == null || ConnectionDeleted)
                   throw new ArgumentNullException("Connection is not valid");

               m_dataBaseProperty.ConnectionString = value;
           }
       }
       /// <summary>
       /// Query table performed Asynchronously
       /// </summary>
       public bool BackgroundQuery
       {
           get
           {
               return m_dataBaseProperty.BackgroundQuery;
           }
           set
           {
               m_dataBaseProperty.BackgroundQuery = value;
           }
       }
       /// <summary>
       /// Represent the Name of the Query Table
       /// </summary>
       public string Name
       {
           get
           {
               return m_name;
           }
           set
           {
               value=value.Replace(" ", "_");
               m_name = value;
           }
       }
       /// <summary>
       /// Represent the connection ID-Read-Only
       /// </summary>
       public uint ConncetionId
       {
           get
           {
               return m_externalConnection.ConncetionId;
           }           
       }
       /// <summary>
       /// Represent the Column width for QueryTable
       /// </summary>
       public bool AdjustColumnWidth
       {
           get 
           {
               return m_adjustColumnWidth;
           }
           set
           {
               m_adjustColumnWidth = value;
           }
       }
       /// <summary>
       /// Represent the connection state
       /// </summary>
       internal bool ConnectionDeleted
       {
           get
           {
               return m_externalConnection.Deleted;
           }           
       }
       internal bool IsDeleted
       {
           get
           {
               return m_isDeleted;
           }
       }       
       internal QueryTableImpl Clone(ListObject Obj,WorkbookImpl book,string ConnectionName)
       {
           QueryTableImpl table = (QueryTableImpl)MemberwiseClone();
           ExternalConnectionCollection Connections;           
           table.m_externalConnection = table.ExternalConnection.Clone(book, ConnectionName);           
           return table;
       }
       /// <summary>
       /// Delete the query table from this object
       /// </summary>
       public void Delete()
       {
           
           WorkbookImpl book = ExternalConnection.Range.Worksheet.Workbook as WorkbookImpl;
           IWorksheet sheet = this.ExternalConnection.Range.Worksheet;

           if (sheet.Names.Contains(this.Name))
               sheet.Names.Remove(this.Name);
           else
               book.Names.Remove(this.Name);
           if (!ConnectionDeleted)
            {
             book.DeleteConnection(ExternalConnection);
            }
           m_isDeleted = true;
           this.ExternalConnection.Dispose();
           this.m_externalConnection = null;
           this.m_dataBaseProperty = null;
           
           
       }
    }
}
