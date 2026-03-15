#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    /// <summary>
    /// The Class that contains information about the files being uploaded
    /// </summary>
    public class UploadFileInfo : INotifyPropertyChanged
    {
        private FileUploadCommand cancelUploadCommand;

        /// <summary>
        /// 
        /// </summary>
        public FileUploadCommand CancelUploadCommand
        {
            get
            {
                if (cancelUploadCommand == null)
                {
                    cancelUploadCommand = new FileUploadCommand(outputParam => OnCancelUploadCommand());
                }
                return cancelUploadCommand;
            }
        }

        private void OnCancelUploadCommand()
        {
            if (this.FileStatus == "Uploading" || this.FileStatus == "Overwriting")//|| this.filestatus == "Canceled"
            {
                //if (Cancel == false)
                //{
                    this.UploadCancel();
                //}
                //else
                //{
                //    this.cancel = false;
                //    this.UploadToServer();
                //}
                //FileEventArgs fileuploadpausedeventargs = new FileEventArgs();
                //fileuploadpausedeventargs.File = this;
                //if (this.Container != null)
                //{
                //    this.Container.FireUploadPausedEvent(fileuploadpausedeventargs);
                //}
                //MessageBox.Show("Upload cancelled.", "File Upload Control", MessageBoxButton.OK);
            }
        }

        private FileUploadCommand removeUploadCommand;

        /// <summary>
        /// 
        /// </summary>
        public FileUploadCommand RemoveUploadCommand
        {
            get
            {
                if (removeUploadCommand == null)
                {
                    removeUploadCommand = new FileUploadCommand(outputParam => OnRemoveUploadCommand());
                }
                return removeUploadCommand;
            }
        }

        private void OnRemoveUploadCommand()
        {
            this.Container.Remove(this);
        }

        #region public fields
        /// <summary>
        /// Identifies the folder where the uploaded files will be saved.
        /// </summary>
        public string UpLoadFolder = string.Empty;
        #endregion Public fields

        #region private Fields
        /// <summary>
        /// Represents Boolean variable
        /// </summary>
        private bool cancel;

        /// <summary>
        /// The Container
        /// </summary>
        private FileUploadControl container;

        /// <summary>
        /// The Dispatcher
        /// </summary>
        private Dispatcher dispatcher;

        /// <summary>
        /// The FileInfo
        /// </summary>
        private FileInfo file;

        /// <summary>
        ///  The File id
        /// </summary>
        private long fileid;

        /// <summary>
        /// The File Length
        /// </summary>
        private long filelength;

        /// <summary>
        /// The File Name
        /// </summary>
        private string filename;

        /// <summary>
        /// The File Size
        /// </summary>
        private string filesize;

        /// <summary>
        /// The File Status
        /// </summary>
        private string filestatus;

        /// <summary>
        /// The File Stream
        /// </summary>
        private Stream fileStream;

        /// <summary>
        /// The percentage Uploaded
        /// </summary>
        private int percentageuploaded;

        /// <summary>
        /// Represents boolean variable for removing the file
        /// </summary>
        private bool remove;

        /// <summary>
        /// The Size uploaded
        /// </summary>
        private long sizeuploaded;

        /// <summary>
        /// The Status Image
        /// </summary>
        private string statusimage;

        #endregion Private fields

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileInfo"/> class
        /// </summary>
        /// <param name="dispatcher">Represents Dispatcher</param>
        /// <param name="c">The FileUpload Control</param>
        public UploadFileInfo(Dispatcher dispatcher, FileUploadControl c)
        {
            this.dispatcher = dispatcher;
            this.container = c;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="UploadFileInfo"/> class
        /// </summary>
        /// <param name="dispatcher">The Dispatcher</param>
        public UploadFileInfo(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        #endregion Constructor

        #region Events
        /// <summary>
        /// Event Handler when the property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating the file to be uploaded
        /// </summary>
        public FileInfo File
        {
            get
            {
                return this.file;
            }

            set
            {
                this.file = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("FileId"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the length of the file
        /// </summary>
        public long FileLength
        {
            get
            {
                return this.filelength;
            }

            set
            {
                this.filelength = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("FileLength"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the file
        /// </summary>
        public string FileName
        {
            get
            {
                return this.filename;
            }

            set
            {
                this.filename = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("FileName"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the FileSize (Size Expressed in KB|MB|GB)
        /// </summary>
        public string FileSize
        {
            get
            {
                return this.filesize;
            }

            set
            {
                this.filesize = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("FileSize"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the status for the file being uploaded.
        /// </summary>
        public string FileStatus
        {
            get
            {
                return this.filestatus;
            }

            set
            {
                this.filestatus = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("FileStatus"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the upload size percentage
        /// </summary>
        public int PercentageUploaded
        {
            get
            {
                return this.percentageuploaded;
            }

            set
            {
                this.percentageuploaded = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("PercentageUploaded"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the upload size
        /// </summary>
        public long SizeUploaded
        {
            get
            {
                return this.sizeuploaded;
            }

            set
            {
                this.sizeuploaded = value;

                this.percentageuploaded = (int)((value * 100) / this.FileLength);
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("SizeUploaded"));
                    }
                });
            }
        }
          
        #endregion Public Properties

        #region Internal Properties
        /// <summary>
        /// Gets or sets a value indicating whether the file being uploaded can be removed or not.
        /// </summary>
        internal bool Remove
        {
            get
            {
                return this.remove;
            }

            set
            {
                this.remove = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Remove"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the uploading can be canceled or not
        /// </summary>
        internal bool Cancel
        {
            get
            {
                return this.cancel;
            }

            set
            {
                this.cancel = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Cancel"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the container for file to be uploaded
        /// </summary>
        internal FileUploadControl Container
        {
            get
            {
                return this.container;
            }

            set
            {
                this.container = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Container"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the file id
        /// </summary>
        internal long FileId
        {
            get
            {
                return this.fileid;
            }

            set
            {
                this.fileid = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("FileId"));
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the status image.
        /// </summary>
        internal string StatusImage
        {
            get
            {
                return this.statusimage;
            }

            set
            {
                this.statusimage = value;
                this.dispatcher.BeginInvoke(delegate
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusImage"));
                    }
                });
            }
        }

        #endregion Internal Properties
    
        #region Internal Methods

        /// <summary>
        /// The ReadCall Back Method
        /// </summary>
        /// <param name="result">The IAsyncResult Object</param>
        internal void ReadCallback(IAsyncResult result)
        {
            HttpWebRequest httpwebrequest = (HttpWebRequest)result.AsyncState;
            HttpWebResponse httpwebresponse = (HttpWebResponse)httpwebrequest.EndGetResponse(result);
            StreamReader reader = new StreamReader(httpwebresponse.GetResponseStream());
            string responsestring = reader.ReadToEnd();
            reader.Close();
            if (this.Cancel)
            {
                if (this.Remove)
                {
                    this.FileStatus = "Removed";
                }
                else
                {
                    this.FileStatus = "Canceled";
                }
            }
            else if (this.SizeUploaded < this.FileLength)
            {
                this.UploadToServer();
            }
            else
            {
                this.dispatcher.BeginInvoke(delegate
                {
                    this.Container.RaiseUploadFinishedEvent(true, this);
                });

                this.FileStatus = "Completed";
            }

            this.fileStream.Close();    
            this.fileStream.Dispose();
        }

       /// <summary>
       /// Method that uploads the file
       /// </summary>
       /// <param name="uploadfolder">Represents the Folder where the uploaded file will be stored</param>
       /// <param name="QueryString"></param>
        internal void Upload(string uploadfolder, string QueryString)
        {
            if (this.FileStatus != "Canceled")
            {
                this.FileStatus = "Uploading";
            }

            this.Cancel = false;
            this.UpLoadFolder = uploadfolder;
            UriBuilder ub = new UriBuilder(uploadfolder);

            if (string.IsNullOrEmpty(QueryString))
                ub.Query = "filename=" + this.filename + "&" + "havebytes=true";
            else
                ub.Query = "filename=" + this.filename + "&" + "havebytes=true" + "&" + QueryString;

            WebClient webclient = new WebClient();
            webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.Webclient_DownloadStringCompleted);
            webclient.DownloadStringAsync(ub.Uri);
        }

        /// <summary>
        /// Method used to set the cancel property when the user wants to cancel the upload process.
        /// </summary>
        internal void UploadCancel()
        {
            this.Cancel = true;
        }
           
        /// <summary>
        /// Method used to set the Remove property when the user wants to Remove the uploaded the file of file to be uploaded.
        /// </summary>
        internal void UploadRemove()
        {
            this.Cancel = true;
            this.Remove = true;
            this.FileStatus = "Removed";
        }
     
        /// <summary>
        /// Method for sending request to server for uploading.
        /// </summary>
        internal void UploadToServer()
        {
            long bytestoupload = this.FileLength - this.SizeUploaded;
            UriBuilder uribuilder = new UriBuilder(this.UpLoadFolder);
            bool status = bytestoupload <= 2500000;
            uribuilder.Query = "filename=" + this.FileName + "&" + "bytestoupload=" + this.SizeUploaded.ToString() + "&status=" + status.ToString();
            HttpWebRequest httpwebrequest = (HttpWebRequest)WebRequest.Create(uribuilder.Uri);
            httpwebrequest.Method = "POST";
            httpwebrequest.BeginGetRequestStream(new AsyncCallback(this.WritingToServer), httpwebrequest);
        }

        /// <summary>
        /// Method used to write the file content to the server
        /// </summary>
        /// <param name="result">The IAsyncResult</param>
        internal void WritingToServer(IAsyncResult result)
        {
            HttpWebRequest webrequest = (HttpWebRequest)result.AsyncState;
            Stream requestStream = webrequest.EndGetRequestStream(result);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            int tempTotal = 0;
            this.fileStream = this.File.OpenRead();
            this. fileStream.Position = this.SizeUploaded;
            while ((bytesRead = this.fileStream.Read(buffer, 0, buffer.Length)) != 0 && tempTotal + bytesRead < 4194304)
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();
                this.SizeUploaded = this.SizeUploaded + bytesRead;
                tempTotal += bytesRead;
                this.PercentageUploaded = (int)(((double)this.SizeUploaded / (double)this.FileLength) * 100);
            }

            requestStream.Close();
            webrequest.BeginGetResponse(new AsyncCallback(this.ReadCallback), webrequest);
        }
        #endregion Internal Methods

        #region Private Methods
        /// <summary>
        /// Provides handler when the file from web being downloaded
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Event Argument</param>
        private void Webclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            long downloadstringlength = 0;
            if (!string.IsNullOrEmpty(e.Result))
            {
                downloadstringlength = long.Parse(e.Result);
            }

            if (downloadstringlength > 0)
            {
                if (downloadstringlength == this.FileLength)
                {
                    if (this.FileStatus != "Canceled")
                    {
                        this.Container.RaiseFileExistsEvent(true, this);
                        if (this.Container.CanOverwrite)
                        {
                            downloadstringlength = 0;
                            this.FileStatus = "Overwriting";
                        }
                        else
                        {
                            this.SizeUploaded = 0;
                            this.FileStatus = "Already Exists";
                            return;
                        }
                    }
                    else
                    {
                        this.FileStatus = "Uploading";
                    }
                }
                else
                {
                    if (this.FileStatus != "Canceled")
                    {
                        this.Container.RaiseFileExistsEvent(true, this);
                        if (this.Container.CanOverwrite)
                        {
                            downloadstringlength = 0;
                            this.FileStatus = "Overwriting";
                        }
                        else
                        {
                            this.SizeUploaded = 0;
                            this.FileStatus = "Already Exists";
                            return;
                        }
                    }
                    else
                    {
                        this.FileStatus = "Uploading";
                    }
                }
            }

            this.UploadToServer();
        }
        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// Provides handling when the property is changed
        /// </summary>
        /// <param name="str">Event Argument</param>
        public void NotifyPropertyChanged(string str)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }
        #endregion Public Methods
    }
}
