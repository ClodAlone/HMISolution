using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UFRecipeSettings.UFRecipeModel;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using System.ComponentModel;

namespace UFRecipeEditor.CsvHelper
{
    internal class UFBaseEntityMap<T> : ClassMap<T>
    {
        #region Public Events
        public event EventHandler<System.IO.ErrorEventArgs> Warning;
        #endregion

        #region Protected Methods
        protected void Validate(T entity)
        {
            if (entity is IDataErrorInfo)
            {
                var dataErrorInfo = entity as IDataErrorInfo;
                foreach (var map in MemberMaps)
                {
                    var error = dataErrorInfo[map.Data.Member.Name];
                    if (!String.IsNullOrEmpty(error))
                        throw new InvalidOperationException(error);
                        //throw new InvalidOperationException(
                        //    String.Format("{0}: {1}", map.Data.IsNameSet ? map.Data.Names[0] : map.Data.Member.Name, error));
                }
            }
        }

        protected void OnWarning(string message)
        {
            var e = Warning;
            if (e != null)
                e(this, new System.IO.ErrorEventArgs(new Exception(message)));
        }

        protected string GetTagEntityReference(IDocument parent, OPCUAEntityReference tagEntityReference, bool useCache = false)
        {
            var ufuaEditor = parent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufuaEditor == null)
                return null;

            var split = tagEntityReference.HumanReadableNoProject.Split(':');
            var instance = split[0];
            var tagName = split[0];
            if (split.Length > 1)
                tagName = split[1];
            else
                instance = null;

            var tag = ufuaEditor.GetTagEntityReference(parent, tagName, instance, false, true);
            if (tag == null)
                OnWarning(String.Format(Properties.Resources.CsvImportTagNotFound, tagEntityReference.HumanReadableNoProject));

            return tag;
        }
        #endregion
    }
}
