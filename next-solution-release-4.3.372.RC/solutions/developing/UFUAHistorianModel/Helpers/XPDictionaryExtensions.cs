using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;

namespace UFUAHistorianModel.Helpers
{
    public static class XPDictionaryExtensions
    {
        public static void ChangeTableName<T>(this XPDictionary dictionary, String newName) where T : XPObject
        {
            var oldName = typeof(T).Name;
//#if !NET_STANDARD
            //var tableNameCustomizer = new DevExpress.ExpressApp.Xpo.Utils.TableNameCustomizer();
            //tableNameCustomizer.CustomizeTableName += (s, e) =>
            //{
            //    if (e.TableName == oldName)
            //    {
            //        e.Handled = true;
            //        e.TableName = newName;
            //    }
            //};
            //tableNameCustomizer.Customize(dictionary);
//#else
            foreach (XPClassInfo ci in dictionary.Classes)
            {
                if (ci.IsPersistent && ci.TableMapType == MapInheritanceType.OwnTable && ci.TableName == oldName)
                    ci.AddAttribute(new PersistentAttribute(newName));
            }
//#endif
        }
    }
}
