#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.IO;
using System.Xml.Serialization;

namespace Syncfusion.ProjIO
{
    internal class ProjectWriter
    {

        #region Initializer
        public ProjectWriter()
        { }
        #endregion

        #region Methods
        public void Write(Project p, FileStream fs)
        {
            FileStream FileStr = fs;
            XmlSerializer serializer;
            TextWriter writer;

            serializer = new XmlSerializer(typeof(Project));
            writer = new StreamWriter(FileStr);
            serializer.Serialize(writer, p);

            writer.Close();
        }
        #endregion
    }
}
