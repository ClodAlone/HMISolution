using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using ScriptVariableValues;

namespace UFProjectManager.ScriptHelpers
{
    public class Scripts
    {
        readonly IDocumentManager documentManager;
        readonly UFProjectDocument Document;

        public Scripts(UFProjectDocument d, IDocumentManager m)
        {
            Document = d;
            documentManager = m;
        }

        public void RunScriptNormal(String name, String parameter = null)
        {
            var file = String.Format("{0}\\{1}{2}", documentManager.TypeLabel, name, documentManager.FileType);
            var startupContext = new StartupContext();
            startupContext.Context = Document;
            startupContext.Parameter = parameter;
            documentManager.Execute(new Uri(file, UriKind.RelativeOrAbsolute), Document, ExecutionMode.Normal, startupContext);
        }

        public void RunScriptAndWait(String name, String parameter = null)
        {
            var file = String.Format("{0}\\{1}{2}", documentManager.TypeLabel, name, documentManager.FileType);
            var startupContext = new StartupContext();
            startupContext.Context = Document;
            startupContext.Parameter = parameter;
            documentManager.Execute(new Uri(file, UriKind.RelativeOrAbsolute), Document, ExecutionMode.Synchro, startupContext);
        }

        public void StopScript(String name)
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
