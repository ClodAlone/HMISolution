using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;

namespace UFRecipeLayout.LayoutItemControls
{
    internal class RecipeIndexViewModel
    {
        #region Declarations
        readonly ObservableCollection<RecipeIndex> recipes = new ObservableCollection<RecipeIndex>();
        readonly DataTable dataTable;
        readonly String primaryKeyName;
        readonly String columnName;
        #endregion

        #region Constructors
        public RecipeIndexViewModel(DataTable dataTable, String primaryKeyName, String columnName)
        {
            this.dataTable = dataTable;
            this.primaryKeyName = primaryKeyName;
            this.columnName = columnName;

            LoadRecipes();
        }
        #endregion

        #region Properties
        public ObservableCollection<RecipeIndex> Recipes
        {
            get
            {
                return recipes;
            }
        }
        #endregion

        #region Methods
        public void LoadRecipes()
        {
            recipes.Clear();
            using (var viewRecipes = new DataView(dataTable))
            {
                viewRecipes.Sort = String.Format("[{0}] ASC", columnName);
                for (int ii = 0; ii < viewRecipes.Count; ii++)
                {
                    Guid uniqueId;
                    var key = viewRecipes[ii][primaryKeyName];
                    try
                    {
                        uniqueId = (Guid)key;
                    }
                    catch (InvalidCastException ex)
                    {
                        if (!Guid.TryParse(key as String, out uniqueId))
                            continue;
                    }
                    
                    var recipeId = new RecipeIndex()
                    {
                        UniqueId = uniqueId,
                        Name = viewRecipes[ii][columnName] as String
                    };

                    recipes.Add(recipeId);
                }
            }
        }
        #endregion
    }

    public class RecipeIndex : IFormattable, INotifyPropertyChanged
    {
        #region Properties
        public Guid uniqueId;
        public Guid UniqueId
        {
            get
            {
                return uniqueId;
            }
            set
            {
                if (uniqueId == value)
                    return;
                uniqueId = value;
                OnPropertyChanged("UniqueId");
            }
        }

        public String name;
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value)
                    return;
                name = value;
                OnPropertyChanged("Name");
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return name;
        }
        #endregion
    }
}
