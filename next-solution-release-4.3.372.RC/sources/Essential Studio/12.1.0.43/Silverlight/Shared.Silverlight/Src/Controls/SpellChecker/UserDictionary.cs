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
using System.IO;
using System.Collections;
using System.Security;
using System.IO.IsolatedStorage;

namespace Syncfusion.Windows.Shared
{
   
    /// <summary>
    /// 
    /// </summary>
    public class UserDictionary
    {
        // Fields
        internal string m_sDictFile;
        internal Stream UserDictionaryStream = null;
        private bool m_bValid;
        private int m_nWordLimit;
        private List<object> m_arrWordList;

        // Methods
        /// <summary>
        /// 
        /// </summary>
        public UserDictionary()
        {
            m_bValid = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDictionaryFile"></param>
        public UserDictionary(string userDictionaryFile) :
            this(userDictionaryFile, 0x1388)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDictionaryFile"></param>
        /// <param name="wordLimit"></param>
        public UserDictionary(string userDictionaryFile, int wordLimit)
        {
            m_bValid = false;
            if ((wordLimit > 0x249f0) || (wordLimit <= 0))
            {
                wordLimit = 0x249f0;
            }
            try
            {
#if WPF
                if ((File.GetAttributes(userDictionaryFile) & FileAttributes.Directory) != 0)
                {
                    m_bValid = false;
                }
#endif

#if SILVERLIGHT
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

                if (!file.FileExists(userDictionaryFile))
                {
                    m_bValid = false;
                }
#endif
                else
                {
                    m_bValid = true;
                }
            }
            catch (IsolatedStorageException )
            {

            }
            catch (SecurityException exception)
            {
                Console.WriteLine("SecurityException occured working with user dictionary file: " + exception);
                m_bValid = false;
            }
            catch (FileNotFoundException exception2)
            {
                Console.WriteLine("FileNotFoundException occured working with user dictionary file, creating it. " + exception2);
                File.CreateText(userDictionaryFile).Close();
                m_bValid = true;
            }
            m_nWordLimit = wordLimit;
            m_arrWordList = new List<object>();
            m_sDictFile = userDictionaryFile;
            if (m_bValid)
            {
                ReadDict();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="wordLimit"></param>
        public UserDictionary(Stream stream, int wordLimit)
        {
            UserDictionaryStream = stream;
            m_nWordLimit = wordLimit;
            m_arrWordList = new List<object>();
            m_sDictFile = string.Empty;
            ReadDict();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public virtual bool AddWord(string word)
        {
            lock (this)
            {
                if (IsValid() && (word.Length > 0))
                {
                    if (m_arrWordList.Count < m_nWordLimit)
                    {
                        m_arrWordList.Add(word);
                        return WriteDict();
                    }
                    return false;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            return m_bValid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual int ReadAll(List<object> list)
        {
            lock (list)
            {
                list.Clear();
                for (int i = 0; i < m_arrWordList.Count; i++)
                {
                    list.Add(m_arrWordList[i]);
                }
                return m_arrWordList.Count;
            }
        }

        private bool ReadDict()
        {
            bool isStream = true;

            if (!string.IsNullOrEmpty(m_sDictFile))
            {
                isStream = false;

#if SILVERLIGHT
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (file.FileExists(m_sDictFile))
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(m_sDictFile, FileMode.Open, file))
                        {
                            return ReadDict(stream);
                        }
                    }
                    else
                    {
                        isStream = true;
                    }
                }
#endif

#if WPF
                string str = "";
                int num = 0;
                try
                {
                    StreamReader reader = new StreamReader(m_sDictFile, new UTF8Encoding());
                    while (str != null)
                    {
                        str = reader.ReadLine();
                        if (str != null)
                        {
                            if (str.StartsWith("Keyoti RapidSpell Dict File"))
                            {
                                throw new ArgumentException("The user dictionary specified is a binary DictFile created by 'Dict Manager' - user dictionaries should be PLAIN TEXT files.  Please set the user dictionary path to a text file (or a path where we can create the user dictionary).");
                            }
                            m_arrWordList.Add(str);
                            num++;
                        }
                    }
                    reader.Close();
                    m_bValid = true;
                }
                catch (FileNotFoundException exception)
                {
                    Console.WriteLine("Can't find wordlist: " + exception);
                    m_bValid = false;
                }
                catch (IOException exception2)
                {
                    Console.WriteLine("Can't open wordlist: " + exception2);
                    m_bValid = false;
                }
                return m_bValid;
#endif
            }

            if (isStream && UserDictionaryStream != null)
            {
                return ReadDict(UserDictionaryStream);
            }

            return false;

        }

        private bool ReadDict(Stream stream)
        {
            if(stream == null)
                return false;
            lock (this)
            {
                string str = "";
                int num = 0;
                try
                {                  
                    StreamReader reader = new StreamReader(stream, new UTF8Encoding());
                    while (str != null)
                    {
                        str = reader.ReadLine();
                        if (str != null && !m_arrWordList.Contains(str))
                        {
                            if (str.StartsWith("Keyoti RapidSpell Dict File"))
                            {
                                throw new ArgumentException("The user dictionary specified is a binary DictFile created by 'Dict Manager' - user dictionaries should be PLAIN TEXT files.  Please set the user dictionary path to a text file (or a path where we can create the user dictionary).");
                            }
                            m_arrWordList.Add(str);
                            num++;
                        }
                    }
                    reader.Close();
                    m_bValid = true;
                }
                catch (Exception )
                {
                    m_bValid = false;
                }
                return m_bValid;
            }
        }

        private bool WriteDict()
        {
            bool flag;
            lock (this)
            {
                try
                {
                    if (UserDictionaryStream != null)
                    {
                        if (!UserDictionaryStream.CanWrite)
                            return false;
                        StreamWriter writer = new StreamWriter(UserDictionaryStream, new UTF8Encoding());
                        for (int i = 0; i < m_arrWordList.Count; i++)
                        {
                            writer.WriteLine(m_arrWordList[i]);
                        }
                        writer.Flush();
                        writer.Close();
                        flag = true;
                    }
                    else
                    {

#if WPF
                        StreamWriter writer = new StreamWriter(m_sDictFile, false, new UTF8Encoding());
                    for (int i = 0; i < m_arrWordList.Count; i++)
                    {
                        writer.WriteLine(m_arrWordList[i]);
                    }
                    writer.Flush();
                    writer.Close();
                    flag = true;
#endif

#if SILVERLIGHT
                        using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            if (!file.FileExists(m_sDictFile))
                            {
                                file.CreateFile(m_sDictFile);
                            }
                            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(m_sDictFile, FileMode.Open, file))
                            {
                                StreamWriter writer = new StreamWriter(stream, new UTF8Encoding());
                                for (int i = 0; i < m_arrWordList.Count; i++)
                                {
                                    writer.WriteLine(m_arrWordList[i]);
                                }
                                writer.Flush();
                                writer.Close();
                                flag = true;

                            }
                        }
#endif
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("UserDictionary Exception " + exception);
                    m_bValid = false;
                    flag = false;
                }
            }
            return flag;
        }
    }
}
