using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFRecipeSettings.Helpers;
using UFRecipeSettings.UFRecipeModel;

namespace UFRecipeLayout.Helpers
{
    public sealed class DataBindingHelper
    {
        #region Path Helpers

        public static String GetDataValuePath(UFRecipeEntity recipe, String id)
        {
            var datavalue = FindDataValue(recipe, id);
            if (datavalue != null)
                return GetDataValuePath(datavalue);

            return String.Empty;
        }


        public static String GetDataValuePath(UFDataValueEntity value)
        {
            if (value.UFRecipeAss != null)
                return String.Format("{0}/{1}", DataSetHelper.TableName(value.UFRecipeAss), DataSetHelper.ColumnName(value));
            else if (value.UFGroupAss != null)
                return String.Format("{0}/{1}", DataSetHelper.TableName(value.UFGroupAss), DataSetHelper.ColumnName(value));
            else
                throw new InvalidOperationException(String.Format("Invalid state for DataValue '{0}' !", value.Name));
        }

        #endregion

        #region UFRecipeModel Helpers

        public static UFDataValueEntity FindDataValue(UFRecipeEntity recipe, String id)
        {
            var values = (from c in recipe.DataValues.AsParallel()
                          where Utilities.WPF.DependencyObjectExtensions.AdaptName(c.NodeId.ToString()) == id
                          select c).ToList();

            if (values.Count > 0)
                return values[0];

            foreach (var group in recipe.Groups.AsParallel())
            {
                var value = FindDataValue(group, id);
                if (value != null)
                    return value;
            }

            return null;
        }

        public static UFDataValueEntity FindDataValue(UFGroupEntity ufgroup, String id)
        {
            var values = (from c in ufgroup.DataValues.AsParallel() 
                          where Utilities.WPF.DependencyObjectExtensions.AdaptName(c.NodeId.ToString()) == id
                          select c).ToList();

            if (values.Count > 0)
                return values[0];

            return null;
        }

        #endregion

    }
}
