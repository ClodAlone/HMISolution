#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;

namespace Syncfusion.Windows.Shared
{
    
    internal class DictionaryFile
    {
        // Fields
        private Stream m_streamDictFile = null;
        private Stream m_streamReverseDictFile = null;
        private string m_sDictFilePath = "";
        private string m_sKey = "";


        private static Dictionary<string, List<object>> m_htdictPathVsEntriesList = new Dictionary<string, List<object>>();

        public static int WordList = 1;
        public static int ReverseList = 2;

        private void SetKey(string sKey)
        {
            m_sKey = sKey;
        }

        // Methods
        public DictionaryFile(Stream dictFileStream, string sKey)
        {
            m_streamDictFile = dictFileStream;
            SetKey(sKey);
        }

        public DictionaryFile(Stream dictFileStream, Stream dictReverseFileStream, string sKey) :
            this(dictFileStream, sKey)
        {
            m_streamReverseDictFile = dictReverseFileStream;
        }

        public DictionaryFile(string dictFilePath)
        {
#if WPF
            m_sDictFilePath = dictFilePath;
#endif
#if SILVERLIGHT
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists(dictFilePath))
                {
                    m_sDictFilePath = dictFilePath;
                }
                else
                {
                    throw new ArgumentException("File does not exists in IsolatedStorage");
                }
            }   
#endif
            SetKey(m_sDictFilePath);
        }

        /// <exclude/>
        public static bool GetIsDictionaryLoadedToMemory(string sPath)
        {
            bool bRes = GetIsDictionaryLoadedToMemory((int)DictionaryFile.WordList, sPath) &&
                GetIsDictionaryLoadedToMemory((int)DictionaryFile.ReverseList, sPath);
            return bRes;
        }

        private static bool GetIsDictionaryLoadedToMemory(int type, string sPath)
        {
            string sKey = type + "_" + sPath;
            bool bRes = null != m_htdictPathVsEntriesList[sKey];
            return bRes;
        }

#if WPF

        public List<object> ReadWordListStream(List<object> list, int type, string encoding)
        {
            int num = 0;
            string sDictKey = type + "_" + m_sKey;
            bool bUseStream = true;
            if (!m_htdictPathVsEntriesList.ContainsKey(sDictKey) || null == m_htdictPathVsEntriesList[sDictKey])
            {
                string str;
                StreamReader reader = null;
                if (bUseStream)
                {
                    if ((type == DictionaryFile.ReverseList) && (null != m_streamReverseDictFile))
                    {
                        reader = new StreamReader(m_streamReverseDictFile);
                    }
                    else if (m_streamDictFile != null)
                    {
                        reader = new StreamReader(m_streamDictFile);
                    }
                    else if (m_streamDictFile == null && !string.IsNullOrEmpty(m_sDictFilePath))
                    {
                        reader = File.OpenText(m_sDictFilePath);
                    }
                }
                else
                {
                    reader = File.OpenText(m_sDictFilePath);
                }

                if (bUseStream)
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                }
                while ((str = reader.ReadLine()) != null)
                {
                    list.Add(str);
                    num++;
                }
                if (!bUseStream)
                {
                    reader.Close();
                }
                if ((type == DictionaryFile.ReverseList) &&
                    (!bUseStream || (null == m_streamReverseDictFile)))
                {
                    //Should be optimized
                    //list.Sort(new ReverseSorter());
                }
                m_htdictPathVsEntriesList[sDictKey] = list;
            }
            if (null != m_htdictPathVsEntriesList[sDictKey])
            {
                list = (List<object>)m_htdictPathVsEntriesList[sDictKey];
            }
            return list;
        }

#endif

#if SILVERLIGHT
        public List<object> ReadWordListStream(List<object> list, int type, string encoding)
        {
            int num = 0;
            string sDictKey = type + "_" + m_sKey;
            bool bUseStream = true;// Shared.Utilities.IsStringEmpty(m_sDictFilePath);
            if (!m_htdictPathVsEntriesList.ContainsKey(sDictKey) || null == m_htdictPathVsEntriesList[sDictKey])
            {
                string str;
                StreamReader reader = null;
                if (bUseStream)
                {
                    if ((type == DictionaryFile.ReverseList) && (null != m_streamReverseDictFile))
                    {
                        reader = new StreamReader(m_streamReverseDictFile);
                    }
                    else
                    {
                        reader = new StreamReader(m_streamDictFile);
                    }
                }
                else
                {
                    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(m_sDictFilePath, FileMode.Open, file))
                        {
                            reader = new StreamReader(stream);
                        }
                    }
                }

                if (bUseStream)
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                }
                while ((str = reader.ReadLine()) != null)
                {
                    list.Add(str);
                    num++;
                }
                if (!bUseStream)
                {
                    reader.Close();
                }
                if ((type == DictionaryFile.ReverseList) &&
                    (!bUseStream || (null == m_streamReverseDictFile)))
                {
                    //Should be optimized
                    //list.Sort(new ReverseSorter());
                }
                m_htdictPathVsEntriesList[sDictKey] = list;
            }
            if (null != m_htdictPathVsEntriesList[sDictKey])
            {
                list = (List<object>)m_htdictPathVsEntriesList[sDictKey];
            }
            return list;
        }
#endif
    }
}
