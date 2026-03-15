using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities.WPF;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for ObjectBrowseTreeItem.xaml
    /// </summary>
    /// 

    public partial class ObjectBrowseTreeItem : UserControl
    {
        //List<Model3D> listModelVisibility;
        //Dictionary<Model3D, Dictionary<Brush, double>> mapModelBrushOpacity;

        public String Title { get; set; }
        public int ZLayer { get; set; }
        public int VisualCount { get; set; }
        public int LogicCount { get; set; }
        public bool IsEditable { get; private set; }
        public bool ContainsCode { get; set; }
        public bool IsDynamic { get; set; }

        public ObjectBrowseTreeItem()
        {
            InitializeComponent();
        }

        public ObjectBrowseTreeItem(String title, bool bEditable/*, 
            List<Model3D> listModel = null, Dictionary<Model3D, Dictionary<Brush, double>> mapModel = null*/)
        {
            InitializeComponent();
            lbTitle.Content = title;
            IsEditable = bEditable;
            //listModelVisibility = listModel;
            //mapModelBrushOpacity = mapModel;
        }

        private void cbVisible_Checked(object sender, RoutedEventArgs e)
        {
            if (bLoading)
                return;

            var uie = DataContext as UIElement;
            if (uie != null)
            {
                bool? IsChecked = cbVisible.IsChecked;
                uie.Visibility = IsChecked != null && IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            }
            /*
            else
            {
                var model = DataContext as Model3D;
                if (model != null)
                {
                    if (!listModelVisibility.Contains(model))
                    {
                        SetOpacity(model, 0);
                        listModelVisibility.Add(model);
                    }
                    else
                    {
                        SetOpacity(model, 1);
                        listModelVisibility.Remove(model);
                    }
                }
            }
            */
        }

        bool bLoading;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            bLoading = true;
            try
            {
                var uie = DataContext as UIElement;
                if (uie != null)
                    cbVisible.IsChecked = uie.Visibility == Visibility.Visible;
                else
                {
                    cbVisible.Visibility = System.Windows.Visibility.Collapsed;
                    /*
                    var model = DataContext as Model3D;
                    if (model != null)
                        cbVisible.IsChecked = !listModelVisibility.Contains(model);
                    */
                }
            }
            finally
            {
                bLoading = false;
            }
        }

        /*
        List<GeometryModel3D> geometries;
        void SetOpacity(Model3D model, double opacity)
        {
            if (!mapModelBrushOpacity.ContainsKey(model))
                mapModelBrushOpacity.Add(model, new Dictionary<Brush, double>());
            Dictionary<Brush, double> mapBrushOpacity = mapModelBrushOpacity[model];
            if (geometries == null)
            {
                geometries = new List<GeometryModel3D>();
                if (model is GeometryModel3D)
                    geometries.Add(model as GeometryModel3D);
                else if (model is Model3DGroup)
                    geometries = DependencyObjectExtensions.GetAllGeometries(model as Model3DGroup);
            }

            geometries.ForEach(geometry =>
            {
                SetOpacity(geometry.Material, opacity, mapBrushOpacity);
                SetOpacity(geometry.BackMaterial, opacity, mapBrushOpacity);
            });
        }

        public void SetOpacity(Material material, double opacity, Dictionary<Brush, double> mapBrushOpacity)
        {
            var group = material as MaterialGroup;
            if (group != null)
            {
                foreach (var mat in group.Children)
                {
                    SetOpacity(mat, opacity, mapBrushOpacity);
                }
                return;
            }

            Brush brush = null;
            var diffuse = material as DiffuseMaterial;
            var emissive = material as EmissiveMaterial;
            var specular = material as SpecularMaterial;
            if (diffuse != null)
                brush = diffuse.Brush;
            if (emissive != null)
                brush = emissive.Brush;
            if (specular != null)
                brush = specular.Brush;

            if (brush == null)
                return;

            if (!mapBrushOpacity.ContainsKey(brush))
                mapBrushOpacity.Add(brush, brush.Opacity);
            brush.Opacity = opacity;
        }
        */
    }
}
