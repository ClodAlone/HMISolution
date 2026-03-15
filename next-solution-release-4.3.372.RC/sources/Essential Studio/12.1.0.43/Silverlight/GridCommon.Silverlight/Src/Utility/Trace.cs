#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace Syncfusion.Windows.Diagnostics
{
    public class Trace
    {
        private static IsolatedStorageFile _storageFile = null;
        private static IsolatedStorageFileStream _storageFileStream = null;
        private static StreamWriter _streamWriter = null;
        private static Trace dummy;

        private static StreamWriter StreamWriter
        {
            get
            {
                if (_streamWriter == null && dummy == null)
                {
                    _storageFile = IsolatedStorageFile.GetUserStoreForApplication();
                    try
                    {
                        FileMode fm = FileMode.CreateNew;
                        if (_storageFile.FileExists("Trace.log"))
                            fm = FileMode.Truncate;
                        _storageFileStream = _storageFile.OpenFile("Trace.log", fm, FileAccess.ReadWrite, FileShare.ReadWrite);
                        _streamWriter = new System.IO.StreamWriter(_storageFileStream);
                        _streamWriter.AutoFlush = true;
                    }
                    catch
                    {
                    }
                    dummy = new Trace();
                }
                return _streamWriter;
            }
        }

        static Trace()
        {
        }

        ~Trace()
        {
            if (_streamWriter != null)
                _storageFileStream.Close();
        }

        [System.Diagnostics.Conditional("TRACE")]
        public static void Write(String message)
        {
            WriteLine(message);
        }

        [System.Diagnostics.Conditional("TRACE")]
        public static void WriteLine(String message)
        {
            StreamWriter sw = StreamWriter;
            if (sw != null)
                sw.WriteLine(message);
        }

    }

}
