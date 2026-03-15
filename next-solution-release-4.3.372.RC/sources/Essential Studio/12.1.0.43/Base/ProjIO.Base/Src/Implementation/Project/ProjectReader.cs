#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Reads Project files
    /// </summary>
    public class ProjectReader
    {
        #region Methods
        /// <summary>
        /// Opens the project file from the stream
        /// </summary>
        /// <param name="fs">Stream which contains the project file contents</param>
        /// <returns>Instance of Project</returns>
        public static Project Open(Stream fs)
        {
            Project P;
            XmlSerializer serializer = new XmlSerializer(typeof(Project));
            P = (Project)serializer.Deserialize(fs);
            if (P.RootTask.Children.Count > 0)
                P = ProjectReader.GetTasks(P);
            return P;
        }

        /// <summary>
        /// Open the project file from the specified file name
        /// </summary>
        /// <param name="fileName">Name of the project file</param>
        /// <returns>Instance of Project</returns>
        public static Project Open(String fileName)
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open))
            {
                return Open(stream);
            }
        }

        /// <summary>
        /// Get tasks based on task hierarchy
        /// </summary>
        /// <param name="P">Project instance</param>
        /// <returns>Project instance</returns>
        internal static Project GetTasks(Project P)
        {
            string wbs = string.Empty;
            List<Task> taskList = new List<Task>();
            Task Parent = P.RootTask.Children[0];
            List<Task> tasks = P.RootTask.Children;
            int i = 1;
            int outlevel = 1;
            taskList.Add(P.RootTask.Children[0]);
            
            //To get task hierarchy
            while (i < P.RootTask.Children.Count)
            {
                if (P.RootTask.Children[i].OutlineLevel == outlevel)
                {
                    P.RootTask.Children[i].Parent = Parent;
                    Parent.Children.Add(P.RootTask.Children[i]);
                }
                else if (P.RootTask.Children[i].OutlineLevel > outlevel)
                {
                    if (P.RootTask.Children[i].OutlineLevel == P.RootTask.Children[i - 1].OutlineLevel)
                        P.RootTask.Children[i].Parent = P.RootTask.Children[i - 1].Parent;
                    else if (P.RootTask.Children[i].OutlineLevel < P.RootTask.Children[i - 1].OutlineLevel)
                    {
                        int q = 1;
                        while (P.RootTask.Children[i-q].OutlineLevel > 0)
                        {
                            if (P.RootTask.Children[i].OutlineLevel == P.RootTask.Children[i - q].OutlineLevel)
                            {
                                P.RootTask.Children[i].Parent = P.RootTask.Children[i - q].Parent;
                                break;                                
                            }
                            q++;
                        }
                    }
                        
                    else
                        P.RootTask.Children[i].Parent = P.RootTask.Children[i - 1];
                    P.RootTask.Children[i].Parent.Children.Add(P.RootTask.Children[i]);
                    taskList.Add(P.RootTask.Children[i]);
                }
                i++;
            }
           
            // To get predecessor, successor of PredecessorLink
            for (int m = 0; m < P.RootTask.Children.Count; m++)
            {
                for (int n = 0; n < P.RootTask.Children[m].PredecessorLink.Count; n++)
                {
                    int puid = P.RootTask.Children[m].PredecessorLink[n].PredecessorUID;
                    P.RootTask.Children[m].PredecessorLink[n].Successor = P.RootTask.Children[m];
                    for (int r = 0; r < P.RootTask.Children.Count; r++)
                    {
                        if (P.RootTask.Children[r].UID == puid)
                        {
                            P.RootTask.Children[m].PredecessorLink[n].Predecessor = P.RootTask.Children[r];
                            break;
                        }
                    }
                }
            }

            // To get task of assignment
            for (int w = 0; w < P.Assignments.Count; w++)
            {
                for (int q = 1; q < tasks.Count; q++)
                {
                    if (P.Assignments[w].TaskUID == tasks[q].UID)
                    {
                        P.Assignments[w].Task = tasks[q];
                        break;
                    }
                }
            }

            // To get resource of assignment
            for (int w = 0; w < P.Assignments.Count; w++)
            {
                for (int q = 1; q < P.Resources.Count; q++)
                {
                    if (P.Assignments[w].ResourceUID == P.Resources[q].UID)
                    {
                        P.Assignments[w].Resource = P.Resources[q];
                        break;
                    }
                }
            }

            //To remove chld tasks from main child list
            for (int j = 0; j < taskList.Count; j++)
            {
                int k = 0;
                while (k < P.RootTask.Children.Count)
                {
                    if (P.RootTask.Children[k].Equals(taskList[j]))
                    {
                        P.RootTask.Children.RemoveAt(k);
                        break;
                    }
                    else
                        k++;

                }
            }
            
            return P;
        }
        #endregion
    }
}
