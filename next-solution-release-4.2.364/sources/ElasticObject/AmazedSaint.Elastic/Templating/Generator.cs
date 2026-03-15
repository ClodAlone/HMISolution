using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TextTemplating;
using EnvDTE;
using System.CodeDom.Compiler;

namespace AmazedSaint.Elastic.Templating
{
    public class Generator
    {

        #region Varibales

        CompilerErrorCollection _errors = new CompilerErrorCollection();
        string _basePath = string.Empty;

        #endregion

        #region Properties
        public CompilerErrorCollection LastErrors
        {
            get
            {
                return _errors;
            }
        }
        #endregion

        #region Methods

        public Generator(string basePath)
        {
            _basePath = basePath;
        }

        public Generator()
        {
            
        }

        /// <summary>
        /// Specify the pre processing options
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="modelType"></param>
        /// <param name="ttPath"></param>
        /// <param name="preProcess"></param>
        /// <returns></returns>
        public string RenderModel(string modelPath, string modelType, string ttPath)
        {
            return RenderModel(modelPath, modelType, ttPath,true);
        }

        /// <summary>
        /// Render the model and returns the results
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="modelType"></param>
        /// <param name="ttPath"></param>
        public string RenderModel(string modelPath,string modelType, string ttPath,bool preProcess)
        {

            if (!string.IsNullOrEmpty(_basePath))
            {
                modelPath = modelPath.Replace("~", _basePath);
                ttPath = ttPath.Replace("~", _basePath);
                
            }

            if (!File.Exists(modelPath) || !File.Exists(ttPath))
            {
                throw new Exception("Model file ('" + modelPath + "') or Template file ('" + ttPath + "') doesn't exist.");
            }

            string templateData = Properties.Resources.Header + 
                File.ReadAllText(ttPath) +
                Properties.Resources.Footer.Replace("~~", Path.GetFullPath(modelPath));

            File.WriteAllText(ttPath + ".tmp", templateData);

            DynamicTemplateHost thost = new DynamicTemplateHost();
            string data = string.Empty;
            _errors.Clear();
            var results = thost.ProcessTemplate(ttPath + ".tmp", out data);

            foreach (var res in results)
                _errors.Add(res as CompilerError);

            try
            {
                File.Delete(ttPath + ".tmp");
            }
            catch { }

            return data;

        }

        #endregion

    }
}
