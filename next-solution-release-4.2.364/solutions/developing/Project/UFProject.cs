using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using UFInterfaces.ResourceBase;
using UFInterfaces.Constants;
using System.Xml;

namespace UFProject
{
    [DataContract(Name = "ProjectData", Namespace = Namespaces.UriProgea)]
    public class UFProject : IResourceBase
    {
        #region Declarations

        Dictionary<String, UFProject> mapChildProjects;
        IResourceBase parent;
        ProjectStatus projectStatus;
        OPCUAClientSettings opcuaClientSettings;
        #endregion

        #region Persistance

        [DataMember]
        List<String> listChildProjectPaths;

        #endregion

        #region Constructor

        public UFProject()
        {
            Initialize();
        }

        public UFProject(IResourceBase p)
        {
            parent = p;
            Initialize();
        }

        void Initialize()
        {
            projectStatus = new ProjectStatus(this);
        }
        #endregion
        
        #region Methods
        static UFProject FromFile(String path)
        {
            if (String.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                DataContractSerializer formatter = new DataContractSerializer(typeof(UFProject));
                UFProject ret = formatter.ReadObject(fileStream) as UFProject;
                ret.ProjectPath = path;

                // ret.opcuaClientSettings = OPCUAClientSettings.Open(path);

                return ret;
            }
        }

        #endregion

        public bool SaveToFile()
        {
            Stream ostrm = File.Open(ProjectPath, FileMode.Create, FileAccess.ReadWrite);

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings);

            bool bRet = false;
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(UFProject));
                serializer.WriteObject(writer, this);
                bRet = true;
            }
            finally
            {
                writer.Close();
            }

            return bRet;
        }

        #region Properties

        String projectPath;
        public String ProjectPath
        {
            get
            {
                return projectPath;
            }
            set
            {
                if (projectPath == value)
                    return;
                projectPath = value;
            }
        }

        public String ProjectFolder
        {
            get
            {
                String ret = String.Format("{0}\\{1}", Path.GetDirectoryName(ProjectPath), Path.GetFileNameWithoutExtension(ProjectPath));
                return ret;
            }
        }

        public ProjectStatus ProjectStatus
        {
            get
            {
                return projectStatus;
            }
        }
        #endregion

        #region IResourceBase Members

        public String AbsolutePath
        {
            get
            {
                return String.Empty;
            }
        }

        public IResourceBase Parent
        {
            get { return parent; }
        }

        public bool IsOpen 
        {
            get
            {
                return false;
            }
        }

        public Uri ChildResourceRelativeUri
        {
            get
            {
                String path = String.Format("{0}{1}", Path.GetFullPath(ProjectPath), Path.GetFileName(ProjectPath));
                return new Uri(path);
            }
        }

        #endregion
    }
}
