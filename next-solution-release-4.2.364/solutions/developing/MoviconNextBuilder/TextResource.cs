using DocumentManager.ComponentService;
using StringManager.ComponentService;
using StringManager.Document;
using StringModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace MoviconNextBuilder
{
    public class TextResource : IDisposable
    {
        #region Ctor
        public TextResource(IDocument parent, StringEditorManagerComponent editormanager)
        {
            stringManagerComponent = editormanager;
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            document = StringEditorDocument.FromFile(uri.GetPathString(), editormanager, parent);
        }
        #endregion Ctor

        #region data
        StringEditorDocument document;

        #endregion data

        #region Properties
        StringEditorManagerComponent stringManagerComponent;
        public StringEditorManagerComponent StringManagerComponent
        {
            get { return stringManagerComponent; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds a language column to the Text resource
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public CultureInfo AddLanguage(string language)
        {
            if(document != null)
                return document.AddLocale(language);
            return null;
        }

        /// <summary>
        /// Remove the whole language column from the Text resource
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool DeleteLanguage(string language)
        {
            if(document != null)
            {
                document.RemoveLocale(language);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a list of strings associated to the stringid, one for every defined language.
        /// </summary>
        /// <param name="stringid"></param>
        /// <returns>returns null if stringid is already present</returns>
        public List<UFStringLocaleText> AddString(string stringid)
        {
            if(document != null && !document.LocaleTextIDExists(stringid))
            {
                return document.AddLocaleText(stringid);
            }
            return null;
        }

        /// <summary>
        /// Gets a list of strings associated to the stringid, one for every defined language.
        /// </summary>
        /// <param name="stringid"></param>
        /// <returns>list is empty if stringid is not present</returns>
        public List<UFStringLocaleText> GetString(string stringid)
        {
            if(document != null)
                return document.GetLocaleTextFromId(stringid);
            return null;
        }

        /// <summary>
        /// Sets a new value for a strinid, in a specified language.
        /// </summary>
        /// <param name="stringid"></param>
        /// <param name="language"></param>
        /// <param name="newvalue"></param>
        /// <returns>returns true if the stringid and the language exists</returns>
        public bool SetString(string stringid, string language, string newvalue)
        {
            var list = GetString(stringid);
            if(list != null  && list.Count > 0)
            {
                var found = (from t in list
                             where t.UFStringLocale.Name == language select t).ToList();
                if(found.Count > 0)
                {
                    found[0].Locale = newvalue;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Deletes dìthe anmed string
        /// </summary>
        /// <param name="stringid"></param>
        /// <returns></returns>
        public bool DeleteString(string stringid)
        {
            if (document == null)
                return false;
            return document.RemoveLocaleText(stringid);
        }

        public void Save()
        {
            if (document != null && document.NeedsSave)
                document.SaveToFile();
        }

        public void Dispose()
        {
            if(document != null)
            {
                document.Dispose();
                document = null;
            }
        }
        #endregion Methods
    }
}
