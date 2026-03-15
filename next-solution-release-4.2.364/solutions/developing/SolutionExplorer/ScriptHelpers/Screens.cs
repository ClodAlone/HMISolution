using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DocumentManager.ComponentService;
using System.Collections;
using System.ComponentModel;

namespace UFProjectManager.ScriptHelpers
{
    public class Screens
    {
        readonly IDocumentManager documentManager;
        readonly IDocumentManager paramenterDocumentManager;
        readonly UFProjectDocument Document;

        public Screens(UFProjectDocument d, IDocumentManager m, IDocumentManager p)
        {
            Document = d;
            paramenterDocumentManager = p;
            documentManager = m;
        }

        void SetParameterFile(Dictionary<String, Object> context, String parameterFile)
        {
            if (!String.IsNullOrEmpty(parameterFile))
            {
                if (paramenterDocumentManager == null || parameterFile.Contains('.'))
                    context.Add("ParameterFile", parameterFile);
                else
                    context.Add("ParameterFile", String.Format("{0}\\{1}{2}", paramenterDocumentManager.TypeLabel, parameterFile, paramenterDocumentManager.FileType));
            }
        }

        public void OpenScreenNormal(String name, String parameterFile = null, int nRequestedMonitor = -1)
        {
            var context = new Dictionary<String, Object>();
            SetParameterFile(context, parameterFile);
            if (nRequestedMonitor != -1)
                context.Add("Monitor", nRequestedMonitor);

            var file = String.Format("{0}\\{1}{2}", documentManager.TypeLabel, name, documentManager.FileType);
            documentManager.Execute(new Uri(file, UriKind.RelativeOrAbsolute), Document, ExecutionMode.Normal, context);
        }

        public void OpenScreenModal(String name, String parameterFile = null, int nRequestedMonitor = -1, 
            bool popup = false, double x = Double.NaN, double y = Double.NaN, bool isRelative = false, int autoCloseSec = -1,
            double width = Double.NaN, double height = Double.NaN)
        {
            var context = new Dictionary<String, Object>();
            SetParameterFile(context, parameterFile);
            if (nRequestedMonitor != -1)
                context.Add("Monitor", nRequestedMonitor);
            if (popup)
                context.Add("SynchroPopup", popup);
            if (!Double.IsNaN(x))
                context.Add("X", x);
            if (!Double.IsNaN(y))
                context.Add("Y", y);
            if (!Double.IsNaN(width))
                context.Add("Width", width);
            if (!Double.IsNaN(height))
                context.Add("Height", height);
            if (autoCloseSec > 0)
                context.Add("AutoCloseSecs", autoCloseSec);
            context.Add("IsRelative", isRelative);

            var file = String.Format("{0}\\{1}{2}", documentManager.TypeLabel, name, documentManager.FileType);
            documentManager.Execute(new Uri(file, UriKind.RelativeOrAbsolute), Document, ExecutionMode.Synchro, context);
        }

        public void OpenScreenFrame(String name, String parameterFile = null, int nRequestedMonitor = -1, 
            double x = Double.NaN, double y = Double.NaN, bool isRelative = false, int autoCloseSec = -1,
            double width = Double.NaN, double height = Double.NaN)
        {
            var context = new Dictionary<String, Object>();
            SetParameterFile(context, parameterFile);
            if (nRequestedMonitor != -1)
                context.Add("Monitor", nRequestedMonitor);
            context.Add("SynchroFrame", true);
            if (!Double.IsNaN(x))
                context.Add("X", x);
            if (!Double.IsNaN(y))
                context.Add("Y", y);
            if (!Double.IsNaN(width))
                context.Add("Width", width);
            if (!Double.IsNaN(height))
                context.Add("Height", height);
            if (autoCloseSec > 0)
                context.Add("AutoCloseSecs", autoCloseSec);
            context.Add("IsRelative", isRelative);

            var file = String.Format("{0}\\{1}{2}", documentManager.TypeLabel, name, documentManager.FileType);
            documentManager.Execute(new Uri(file, UriKind.RelativeOrAbsolute), Document, ExecutionMode.Synchro, context);
        }

        public void CloseScreen(String name)
        {
            var file = String.Format("{0}\\{1}{2}", documentManager.TypeLabel, name, documentManager.FileType);
            documentManager.Execute(new Uri(file, UriKind.RelativeOrAbsolute), Document, ExecutionMode.Stop, Document);
        }

        #region Override Methods
        /// <summary>
        /// Redeclaration that hides the <see cref="object.GetHashCode()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Redeclaration that hides the <see cref="object.ToString()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Redeclaration that hides the <see cref="object.Equals(object)"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        #endregion
    }
}
