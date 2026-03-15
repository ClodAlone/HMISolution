#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.IO;
using Syncfusion.XlsIO;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Spreadsheet.Commands;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using System;
using Syncfusion.Windows.Controls.Grid;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    public class ExcelProperties : DependencyObject,INotifyPropertyChanged 
    {
        #region PrivateMembers
        IWorkbook _workBook;
        ExcelEngine _excelEngine;
        IApplication _application;
        private string _fileName;
        private int _visibleSheetCount;
        private int _defaultRowCount = 100;
        private int _defaultColumnCount = 100;
        private double _defaultRowHeight = 18;
        private double _defaultColumnWidth = 12;
        private IRange _currentExcelRangeStyle;
        private bool _extendRowAndColumn = true;
        BinaryList changedCellList;
        XElement changedXMLCellList;

        #endregion

        public ExcelProperties()
        {
            ExcelEngine = new ExcelEngine();
            ExcelEngine.Excel.DefaultVersion = ExcelVersion.Excel2010;
            Application = ExcelEngine.Excel;
            WireEvent();

        }
        private void WireEvent()
        {
            this.Application.OnPasswordRequired -= new PasswordRequiredEventHandler(Application_OnPasswordRequired);
            this.Application.OnWrongPassword -= new PasswordRequiredEventHandler(Application_OnWrongPassword);
            this.Application.OnWrongPassword += new PasswordRequiredEventHandler(Application_OnWrongPassword);
            this.Application.OnPasswordRequired += new PasswordRequiredEventHandler(Application_OnPasswordRequired);
        }

        void Application_OnWrongPassword(object sender, PasswordRequiredEventArgs e)
        {
            MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_InCorrectPassword, SpreadsheetResourceWrapper.ValidationMessage_Error, MessageBoxButton.OK);
        }


        /// <summary>
        /// Gets or sets the spread control.
        /// </summary>
        /// <value>The spread control.</value>
        internal SpreadsheetControl spreadControl { get; set; }

        void Application_OnPasswordRequired(object sender, PasswordRequiredEventArgs e)
        {
            GetPasswordWindow pass = new GetPasswordWindow();
            pass.Text = FileName;


#if !SILVERLIGHT
            
            pass.ShowDialog();
            if (pass.CancelPassword)
            {
                e.StopParsing = false;
            }
            else
            {
                e.NewPassword = pass.passwordBox.Password;
            }
#else
            
            e.StopParsing = false;
            //set the width for Silverlight Password window
            pass.Width = 300.00;
            pass.AssociatedSpreadsheet = spreadControl;
            pass.ShowDialog();
           
#endif
        }

        

        #region Dependency property registeration

        /// <summary>
        /// Author Dependency property
        /// </summary>
        public static readonly DependencyProperty AuthorProperty = DependencyProperty.Register(
          "Author", typeof(string), typeof(ExcelProperties), new PropertyMetadata(null, OnAuthorPropertyChanged));



        /// <summary>
        /// ApplicationName Dependency Property
        /// </summary>
        public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register(
          "ApplicationName", typeof(string), typeof(ExcelProperties), new PropertyMetadata(null, ApplicationNameChanged));



        /// <summary>
        /// Category Dependency property
        /// </summary>
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(
        "Category", typeof(string), typeof(ExcelProperties), new PropertyMetadata(null, CategoryChanged));

        /// <summary>
        /// Comments Dependency property
        /// </summary>
        public static readonly DependencyProperty CommentsProperty = DependencyProperty.Register(
        "Comments", typeof(string), typeof(ExcelProperties), new PropertyMetadata(null, CommentsNameChanged));

        /// <summary>
        /// Company Dependency property
        /// </summary>
        public static readonly DependencyProperty CompanyProperty = DependencyProperty.Register(
         "Company", typeof(string), typeof(ExcelProperties), new PropertyMetadata(null, CompanyChanged));

        /// <summary>
        /// Subject Dependency property
        /// </summary>
        public static readonly DependencyProperty SubjectProperty = DependencyProperty.Register(
         "Subject", typeof(string), typeof(ExcelProperties), new PropertyMetadata(null, SubjectChanged));

        /// <summary>
        /// Keywords Dependency property
        /// </summary>
        public static readonly DependencyProperty KeywordsProperty = DependencyProperty.Register(
        "Keywords", typeof(string), typeof(ExcelProperties), new PropertyMetadata(null, KeywordsChanged));



        /// <summary>
        /// Manager Dependency property
        /// </summary>
        public static readonly DependencyProperty ManagerProperty = DependencyProperty.Register(
       "Manager", typeof(string), typeof(ExcelProperties), new PropertyMetadata(null, ManagerChanged));



        /// <summary>
        /// Title Dependency property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
          "Title", typeof(string), typeof(ExcelProperties), new PropertyMetadata(null, TitleChanged));




        #endregion

        #region Property Definition

        /// <summary>
        /// Gets or sets the author name.
        /// </summary>
        /// <value>The author.</value>
        public string Author
        {
            get { return (string)this.GetValue(AuthorProperty); }
            set { this.SetValue(AuthorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName
        {
            get { return (string)this.GetValue(ApplicationNameProperty); }
            set { this.SetValue(ApplicationNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        public string Category
        {
            get { return (string)this.GetValue(CategoryProperty); }
            set { this.SetValue(CategoryProperty, value); }
        }


        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        public string Comments
        {
            get { return (string)this.GetValue(CommentsProperty); }
            set { this.SetValue(CommentsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>The company.</value>
        public string Company
        {
            get { return (string)this.GetValue(CompanyProperty); }
            set { this.SetValue(CompanyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get { return (string)this.GetValue(SubjectProperty); }
            set { this.SetValue(SubjectProperty, value); }
        }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public string Keywords
        {
            get { return (string)this.GetValue(KeywordsProperty); }
            set { this.SetValue(KeywordsProperty, value); }
        }


        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        /// <value>The manager.</value>
        public string Manager
        {
            get { return (string)this.GetValue(ManagerProperty); }
            set { this.SetValue(ManagerProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

#endregion
        
        #region Property changed events
        
        /// <summary>
        /// Called when [author property changed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAuthorPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            ExcelProperties control = source as ExcelProperties;
            string authName = (string)e.NewValue;
            if (control != null && e.NewValue != null)
                control.WorkBook.Author = authName;
        }
        
        /// <summary>
        /// Applications the name changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ApplicationNameChanged(DependencyObject source,
      DependencyPropertyChangedEventArgs e)
        {
            ExcelProperties control = source as ExcelProperties;
            string appName = (string)e.NewValue;
            //if (control != null && e.NewValue != null)
            //  this.bu

        }
        
        /// <summary>
        /// Categories the changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void CategoryChanged(DependencyObject source,
   DependencyPropertyChangedEventArgs e)
        {
            ExcelProperties control = source as ExcelProperties;
            string categoryName = (string)e.NewValue;

            if (control != null && e.NewValue != null)
                control.WorkBook.BuiltInDocumentProperties.Category = categoryName;

        }




        /// <summary>
        /// Commentses the name changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void CommentsNameChanged(DependencyObject source,
   DependencyPropertyChangedEventArgs e)
        {
            ExcelProperties control = source as ExcelProperties;
            string commentText = (string)e.NewValue;

            if (control != null && e.NewValue != null)
                control.WorkBook.BuiltInDocumentProperties.Comments = commentText;


        }
        
        /// <summary>
        /// Companies the changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void CompanyChanged(DependencyObject source,
   DependencyPropertyChangedEventArgs e)
        {
            ExcelProperties control = source as ExcelProperties;
            string companyName = (string)e.NewValue;

            if (control != null && e.NewValue != null)
                control.WorkBook.BuiltInDocumentProperties.Company = companyName;

        }
        
        /// <summary>
        /// Subjects the changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void SubjectChanged(DependencyObject source,
   DependencyPropertyChangedEventArgs e)
        {
            ExcelProperties control = source as ExcelProperties;
            string subject = (string)e.NewValue;

            if (control != null && e.NewValue != null)
                control.WorkBook.BuiltInDocumentProperties.Subject = subject;


        }
        
        /// <summary>
        /// Keywordses the changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void KeywordsChanged(DependencyObject source,
   DependencyPropertyChangedEventArgs e)
        {
            ExcelProperties control = source as ExcelProperties;
            string keyword = (string)e.NewValue;

            if (control != null && e.NewValue != null)
                control.WorkBook.BuiltInDocumentProperties.Keywords = keyword;


        }
        
        /// <summary>
        /// Managers the changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ManagerChanged(DependencyObject source,
   DependencyPropertyChangedEventArgs e)
        {
            ExcelProperties control = source as ExcelProperties;
            string manager = (string)e.NewValue;

            if (control != null && e.NewValue != null)
                control.WorkBook.BuiltInDocumentProperties.Manager = manager;


        }



        /// <summary>
        /// Titles the changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void TitleChanged(DependencyObject source,
   DependencyPropertyChangedEventArgs e)
        {
            ExcelProperties control = source as ExcelProperties;
            string Title = (string)e.NewValue;

            if (control != null && e.NewValue != null)
                control.WorkBook.BuiltInDocumentProperties.Title = Title;

        }

        #endregion

      

        bool _isPasswordProtected=false;
        public bool IsPasswordProtected
        {
            get
            {
                return _isPasswordProtected;
            }
            set
            {
                _isPasswordProtected = value;
                if (value)
                    this.spreadControl.GridProperties.IsDisableMode = true;
                else if (!this.spreadControl.GridProperties.IsFocusedOnGraphicCells && !this.spreadControl.FormulaRangeSelection.IsInFormulaEditing)
                    this.spreadControl.GridProperties.IsDisableMode = false;
                OnPropertyChanged("IsPasswordProtected");
            }
        }

        bool _isWorkBookProtected;
        public bool IsWorkBookProtected
        {
            get
            {
                return _isWorkBookProtected;
            }
            set
            {
                _isWorkBookProtected = value;
                OnPropertyChanged("IsWorkBookProtected");

            }

        }

        
        internal Stream wbs; //workbookStream declared for store workbook to decrypt workbook in silverlight
        /// <summary>
        /// Sets the excel engine.
        /// </summary>
        /// <param name="workBookStream">The work book stream.</param>
        /// <param name="FileName">Name of the file.</param>
        internal void SetExcelEngine(Stream workBookStream, string FileName)
        {
            if (Application.Workbooks.Count > 0)
                Application.Workbooks.Close();

            WorkBook = Application.Workbooks.Open(workBookStream, ExcelOpenType.Automatic);
        }

        /// <summary>
        /// Sets the excel engine.
        /// </summary>
        /// <param name="workBookStream">The work book stream.</param>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="Control">The control.</param>
        internal void SetExcelEngine(Stream workBookStream, string FileName, SpreadsheetControl Control)
        {
            if (Application.Workbooks.Count > 0)
                Application.Workbooks.Close();
            this.spreadControl = Control;
            wbs = workBookStream;
            WorkBook = Application.Workbooks.Open(workBookStream, ExcelOpenType.Automatic);
        }

        /// <summary>
        /// Sets the excel engine.
        /// </summary>
        /// <param name="workBookStream">The work book stream.</param>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="Password">The password.</param>
        internal void SetExcelEngine(Stream workBookStream, string FileName, string Password)
        {
            if (Application.Workbooks.Count > 0)
            {
                Application.Workbooks.Close();
                WorkBook.Close();
              
            }
            workBookStream.Position = 0;
            try
            {
                //try to decrypt workbookstream with given password
                //WorkBook = Application.Workbooks.Open(workBookStream, ExcelParseOptions.Default, false, Password, ExcelOpenType.Automatic);
                this.spreadControl.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Table());
                this.spreadControl.GridProperties.CurrentExcelGridModel.InvalidateVisual();
               
            }
            catch (Exception ex)
            {
                if (ex.Message == "Workbook is protected and password wasn't specified.")
                {
                    this.spreadControl.New(3);
                }
            }
           
        }

        internal void SetExcelEngine(string filename)
        {
            if (Application.Workbooks.Count > 0)
                Application.Workbooks.Close();
            WorkBook = Application.Workbooks.Open(filename, ExcelOpenType.Automatic);
        }

        public IWorkbook WorkBook
        {
            get { return _workBook; }
            internal set
            {
                _workBook = value;
                OnPropertyChanged("WorkBook");
            }
        }

        // internal BinaryList ChangedCellList = new BinaryList();    
        public BinaryList ChangedCellList
        {
            get { return changedCellList; }
            set { changedCellList = value; }
        }

        public XElement ChangedXMLCellList
        {
            get { return changedXMLCellList; }
            set { changedXMLCellList = value; }
        }


        public ExcelEngine ExcelEngine
        {
            get { return _excelEngine; }
            internal set { _excelEngine = value; }
        }

        public IApplication Application
        {
            get { return _application; }
            internal set { _application = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set 
            { 
                _fileName = value;
                OnPropertyChanged("FileName");
                OnPropertyChanged("RibbonTitle");
            }
        }

        public string RibbonTitle
        {
            get 
            {
                if (FileName != null && FileName != string.Empty)
                    return FileName + " - Spreadsheet Control";
                else
                    return "Book1 - Spreadsheet Control";
            }
        }

        private void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public int VisibleSheetCount
        {
            get { return _visibleSheetCount; }
            internal set { _visibleSheetCount = value; }
        }

        public int DefaultRowCount
        {
            get { return _defaultRowCount; }
            set { _defaultRowCount = value; }
        }
        
        public int DefaultColumnCount
        {
            get { return _defaultColumnCount; }
            set { _defaultColumnCount = value; }
        }

        public double DefaultRowHeight
        {
            get { return _defaultRowHeight; }
            set { _defaultRowHeight = value; }
        }

        public double DefaultColumnWidth
        {
            get { return _defaultColumnWidth; }
            set { _defaultColumnWidth = value; }
        }

        public bool ExtendRowAndColumn
        {
            get { return _extendRowAndColumn; }
            set { _extendRowAndColumn = value; }
        }

        public IRange CurrentExcelRangeStyle
        {
            get { return _currentExcelRangeStyle; }
            set 
            { 
                _currentExcelRangeStyle = value;
                OnPropertyChanged("CurrentExcelRangeStyle");
            }
        }

    }

    public class BinaryList : List<string>
    {
        public int AddIfUnique(string o)
        {
            int loc = -1;
            if (o != null)
            {
                loc = this.BinarySearch(o);
                if (loc < 0)
                {
                    this.Insert(-loc - 1, o);
                }
            }
            return loc;
        }
    }

}
