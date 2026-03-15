using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringManager.ComponentService;

namespace UFProjectManager.ScriptHelpers
{
    public class Strings
    {
        readonly IStringEditorManager documentManager;
        readonly UFProjectDocument Document;

        public Strings(UFProjectDocument d, IStringEditorManager m)
        {
            Document = d;
            documentManager = m;
        }

        public String GetStringFromID(String id)
        {
            var active = documentManager.GetActiveCulture(Document);
            if (!String.IsNullOrEmpty(active))
            {
                var map = documentManager.GetListStringForCulture(Document, active);
                if (map != null && map.ContainsKey(id))
                    return map[id];
            }
            return String.Empty;
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
