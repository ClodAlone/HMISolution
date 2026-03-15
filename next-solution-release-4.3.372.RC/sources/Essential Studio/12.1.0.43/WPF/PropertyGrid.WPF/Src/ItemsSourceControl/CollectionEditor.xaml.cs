#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls;

namespace Syncfusion.Windows.PropertyGrid
{
    /// <summary>
    /// Interaction logic for CollectionEditor UI.
    /// </summary>
    public partial class CollectionEditor : Window
    {
        private ItemsSourceControl ItemsSourceCtrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEditor"/> class.
        /// </summary>
        public CollectionEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEditor"/> class.
        /// </summary>
        /// <param name="itemsSourceCtrl">The items source CTRL.</param>
        public CollectionEditor(ItemsSourceControl itemsSourceCtrl)
        {
            InitializeComponent();
            this.ItemsSourceCtrl = itemsSourceCtrl;
            this.Part_ListBox.ItemsSource = itemsSourceCtrl.ItemsSource;
            this.DataContext = this;
            this.Loaded += new RoutedEventHandler(CollectionEditor_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the CollectionEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void CollectionEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Part_ListBox.Items.Count > 0)
                this.SelectedObject = Part_ListBox.Items[0];

            Type type =typeof(TextBlock );

            if (SelectedObject !=null )
                type = this.SelectedObject.GetType();

            string itemType = type.Name.ToString();

            this.ItemType.Clear();

            this.ItemType.Add(itemType);

            this.Part_Combo.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Click event of the AddBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Type selectedItemType = typeof(TextBlock);

            if (SelectedObject != null)
                selectedItemType = this.SelectedObject.GetType();

            object newObj = GetNewInstance(selectedItemType);

            List<object> source = new List<object>();

            foreach (var item in Part_ListBox.Items)
                source.Add(item);

            source.Add(newObj);

            this.Part_ListBox.ItemsSource = null;

            this.Part_ListBox.ItemsSource = source;

            this.Part_ListBox.SelectedIndex = this.Part_ListBox.Items.Count - 1;
        }
     
        /// <summary>
        /// Gets the new instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private object GetNewInstance(Type type)
        {
            switch (type.Name.ToString())
            {
                case "RibbonButton":
                    return new RibbonButton();

                case "RibbonComboBoxItem":
                    return new RibbonComboBoxItem();

                case "CheckListBoxItem":
                    return new CheckListBoxItem();

                case "MenuItemAdv":
                    return new MenuItemAdv();

                case "ComboBoxItem":
                    return new ComboBoxItem();

                case "CheckBox":
                    return new CheckBox();

                case "TabItemExt":
                    return new TabItemExt();

                case "TileViewItem":
                    return new TileViewItem();

                case "TreeViewItemAdv":
                    return new TreeViewItemAdv();

                default:
                    return new TextBlock { Text = "New Text Block" };
            }
        }

        /// <summary>
        /// Handles the Click event of the OkBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            List<object> source = new List<object>();

            foreach (var item in Part_ListBox.Items)
                source.Add(item);

            this.Part_ListBox.ItemsSource = null;

            this.Part_ListBox.ItemsSource = source;

            this.ItemsSourceCtrl.ItemsSource = this.Part_ListBox.ItemsSource;

            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the CancelBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Handles the Click event of the RemoveBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedObject != null)
            {
                List<object> source = new List<object>();

                foreach (var item in Part_ListBox.Items)
                    source.Add(item);

                object selectedItem = this.SelectedObject;

                var selectedIndex = this.Part_ListBox.SelectedIndex;

                this.Part_ListBox.ItemsSource = null;

                source.Remove(selectedItem);

                this.Part_ListBox.ItemsSource = source;

                this.Part_ListBox.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public object SelectedObject
        {
            get { return (object)GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedObject.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(CollectionEditor), new UIPropertyMetadata(null));


        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        /// <value>The type of the item.</value>
        public List<string> ItemType
        {
            get { return (List<string>)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(List<string>), typeof(CollectionEditor), new PropertyMetadata(new List<string>()));
    }
}
