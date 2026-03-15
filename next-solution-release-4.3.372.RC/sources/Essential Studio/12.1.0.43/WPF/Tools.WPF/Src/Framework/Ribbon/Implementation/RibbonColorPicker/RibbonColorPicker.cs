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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Syncfusion.Windows.Tools.Controls;
using System.Reflection;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Shared;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    /// xmlns:MyNamespace="clr-namespace:CustomColorChooser"
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    /// xmlns:MyNamespace="clr-namespace:CustomColorChooser;assembly=CustomColorChooser"
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    /// Right click on the target project in the Solution Explorer and
    /// "Add Reference"-&gt;"Projects"-&gt;[Browse to and select this project]
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    /// </summary>

 
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonColorPicker : Control
    {
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        //.................................................................
        /// <summary>
        /// Initializes the <see cref="RibbonColorPicker"/> class.
        /// </summary>
        static RibbonColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonColorPicker), new FrameworkPropertyMetadata(typeof(RibbonColorPicker)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonColorPicker"/> class.
        /// </summary>
        public RibbonColorPicker()
        {
            DS = new DataSource();
            //AvailableThemes=DS.themes
        }

        /// <summary>
        /// Represents the a panel
        /// </summary>
        StackPanel panel;

        /// <summary>
        /// Represents the a  grid
        /// </summary>
        Grid Rct;

        /// <summary>
        /// Represents the a datasource
        /// </summary>
        DataSource DS;

        /// <summary>
        /// Represents the width and height
        /// </summary>
        int W=15, H=15;

        /// <summary>
        /// Represents the a popup
        /// </summary>
        Popup ColPick;

        /// <summary>
        /// Represents the color pick
        /// </summary>
        xColorEdit PickCol;

        /// <summary>
        /// Represents the parent ui element
        /// </summary>
        UIElement parentWin;

        /// <summary>
        /// Gets or sets the color of the preview.
        /// </summary>
        /// <value>The color of the preview.</value>
        public Color PreviewColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the available themes.
        /// </summary>
        /// <value>The available themes.</value>
        public ObservableCollection<ColorTheme> AvailableThemes
        {
            get
            {
                return DS.themes;
            }
            set
            {
                DS.themes = value;
            }
        }



        /// <summary>
        /// Represents the previous border
        /// </summary>
        private Border previousBorder=new Border();

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();
            
            parentWin = Window.GetWindow(this.Parent);
            panel = (StackPanel) this.Template.FindName("Tpanel", this); //base.GetTemplateChild("Part_xxx") as StackPanel;
            
            Grid tempGrid = (Grid)this.Template.FindName("ThemeGrid", this);
            ColorTheme t = getTheme(SelectedTheme);
            getColorGrid(ref tempGrid,t.ThemeColors, true);
            
            tempGrid = (Grid)this.Template.FindName("StdGrid", this);
            getColorGrid(ref tempGrid,DS.Std.ThemeColors, false);
            
            ((Grid)this.Template.FindName("RctGrid", this)).Visibility = Visibility.Collapsed;
            ((Label)this.Template.FindName("RctHeader", this)).Visibility = Visibility.Collapsed;
            Button b = (Button)this.Template.FindName("CPDlg", this);
            b.Click += new RoutedEventHandler(ChooseColor_Click);
        }

        /// <summary>
        /// Gets the color grid.
        /// </summary>
        /// <param name="tempGrid">The temp grid.</param>
        /// <param name="list">The list.</param>
        /// <param name="flag">if set to <c>true</c> [flag].</param>
        private void getColorGrid(ref Grid tempGrid, List<Color> list, bool flag)
        {
            Grid g = tempGrid;
            g.Children.Clear();
            Border b;
            if (g.RowDefinitions.Count > 0) 
                g.RowDefinitions.RemoveRange(0, g.RowDefinitions.Count);
            if (g.ColumnDefinitions.Count > 0)
                g.ColumnDefinitions.RemoveRange(0, g.ColumnDefinitions.Count);
            g.RowDefinitions.Add(new RowDefinition());
            if (list != null)
            {

                for (int i = 0; i < list.Count; i++)
                {
                    g.RowDefinitions.Add(new RowDefinition()); g.HorizontalAlignment = HorizontalAlignment.Left;
                    g.ColumnDefinitions.Add(new ColumnDefinition());
                    if (flag && (i == 0 || i == 1)&& (list[0]==Colors.White || list[1]==Colors.Black))
                    {
                        b = new Border();
                        b.Height = H; b.Width = W;
                        b.Background = new SolidColorBrush(list[i]);
                        b.BorderBrush = new SolidColorBrush(Colors.White);
                        b.BorderThickness = new Thickness(1, 0, 1, 0);
                        b.MouseEnter += new MouseEventHandler(b_MouseEnter);
                        b.MouseLeave += new MouseEventHandler(b_MouseLeave);
                        b.MouseDown += new MouseButtonEventHandler(b_MouseDown);
                        b.Focusable = true;
                        //b.Content = this.ThemeColorList[i].R.ToString() + "," + this.ThemeColorList[i].G.ToString() + "," + this.ThemeColorList[i].B.ToString() + ",";
                        g.Children.Add(b);
                        Grid.SetRow(b, 0);
                        Grid.SetColumn(b, i);
                        if (!flag)
                            continue;
                        b = new Border();
                        b.Height = H * 0.5; b.Width = W * 0.5;
                        b.Background = new SolidColorBrush(Colors.Transparent);
                        
                        Color[] col = getInitialRows(i);
                        for (int j = 0; j < 5 && flag; j++)
                        {
                            b = new Border();
                            b.Height = H; b.Width = W;
                            b.Background = new SolidColorBrush(col[j]);
                            b.BorderBrush = new SolidColorBrush(Colors.Transparent);
                            b.BorderThickness = new Thickness(1, 0, 1, 0);
                            b.MouseEnter += new MouseEventHandler(b_MouseEnter);
                            b.MouseLeave += new MouseEventHandler(b_MouseLeave);
                            b.MouseDown += new MouseButtonEventHandler(b_MouseDown);
                            b.Focusable = true;
                            //b.Content = col[j].R.ToString() +","+ col[j].G.ToString() + ","+col[j].B.ToString();
                            g.Children.Add(b);
                            Grid.SetRow(b, j + 2);
                            Grid.SetColumn(b, i);
                        }
                    }
                    else
                    {
                        b = new Border();
                        b.Height = H; b.Width = W;
                        b.Background = new SolidColorBrush(list[i]);
                        b.BorderBrush = new SolidColorBrush(Colors.White);
                        b.BorderThickness = new Thickness(1, 0, 1, 0);
                        b.MouseEnter += new MouseEventHandler(b_MouseEnter);
                        b.MouseLeave += new MouseEventHandler(b_MouseLeave);
                        b.MouseDown += new MouseButtonEventHandler(b_MouseDown);
                        b.Focusable = true;
                        if ((b.Background as SolidColorBrush).Color.Equals(this.SelectedColor))
                        {
                            previousBorder = b;
                            b.BorderBrush = new SolidColorBrush(Colors.Orange);
                            b.BorderThickness = new Thickness(2);
                        }
                        //b.Content = this.ThemeColorList[i].R.ToString() + "," + this.ThemeColorList[i].G.ToString() + "," + this.ThemeColorList[i].B.ToString() + ",";
                        g.Children.Add(b);
                        Grid.SetRow(b, 0);
                        Grid.SetColumn(b, i);
                        if (!flag)
                            continue;
                        b = new Border();
                        b.Height = H * 0.5; b.Width = W * 0.5;
                        b.Background = new SolidColorBrush(Colors.Transparent);
                        
                        g.Children.Add(b);
                        Grid.SetRow(b, 1);
                        Grid.SetColumn(b, i);
                        Color[] col = getRelatedColors(list[i]);
                        for (int j = 0; j < 5 && flag; j++)
                        {
                            b = new Border();
                            b.Height = H; b.Width = W;
                            b.Background = new SolidColorBrush(col[j]);
                            b.BorderBrush = new SolidColorBrush(Colors.Transparent);
                            b.BorderThickness = new Thickness(1, 0, 1, 0);
                            b.MouseEnter += new MouseEventHandler(b_MouseEnter);
                            b.MouseLeave += new MouseEventHandler(b_MouseLeave);
                            b.MouseDown += new MouseButtonEventHandler(b_MouseDown);
                            b.Focusable = true;
                            //b.Content = col[j].R.ToString() +","+ col[j].G.ToString() + ","+col[j].B.ToString();
                            g.Children.Add(b);
                            Grid.SetRow(b, j + 2);
                            Grid.SetColumn(b, i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the initial rows.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        private Color[] getInitialRows(int i)
        {
            List<Color> l = new List<Color>();
            if (i == 0)
            {
                l.Add(Color.FromArgb(255,242, 242, 242));
                l.Add( Color.FromArgb(255, 216, 216, 216));
                l.Add( Color.FromArgb(255, 191, 191, 191));
                l.Add( Color.FromArgb(255, 165, 165, 165));
                l.Add( Color.FromArgb(255, 127, 127, 127));
            }
            else
            {
                l.Add( Color.FromArgb(255, 127, 127, 127));
                l.Add( Color.FromArgb(255, 89, 89, 89));
                l.Add( Color.FromArgb(255, 63, 63, 63));
                l.Add( Color.FromArgb(255, 38, 38, 38));
                l.Add( Color.FromArgb(255, 12, 12, 12));
            }
            return l.ToArray();
        }

        /// <summary>
        /// Handles the Click event of the ChooseColor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void ChooseColor_Click(object sender, RoutedEventArgs e)
        {
            
            ColorPickerDialog cpd = new ColorPickerDialog();
            cpd.selectedColor = this.SelectedColor;
            
            cpd.Owner = Window.GetWindow(this.Parent);

            cpd.ShowInTaskbar = false;
            bool? dialogResult = cpd.ShowDialog();
            if (dialogResult != null && (bool)cpd.ok == true)
            {
                this.SelectedColor = cpd.selectedColor;

                
                addRecent(new SolidColorBrush(cpd.selectedColor));
                Border b=new Border();
                foreach (UIElement ui in Rct.Children)
                {
                    b = ui as Border;
                    if(cpd.selectedColor.Equals((b.Background as SolidColorBrush).Color))
                    {
                        break;
                    }
                }
                foreach (UIElement element in ((Grid)this.Template.FindName("ThemeGrid", this)).Children)
                {
                    if (element is Border)
                    {
                        (element as Border).BorderBrush = new SolidColorBrush(Colors.White);
                        (element as Border).BorderThickness = new Thickness(1, 0, 1, 0);
                    }
                }
                foreach (UIElement element in ((Grid)this.Template.FindName("StdGrid", this)).Children)
                {
                    if (element is Border)
                    {
                        (element as Border).BorderBrush = new SolidColorBrush(Colors.White);
                        (element as Border).BorderThickness = new Thickness(1, 0, 1, 0);
                    }
                }
                previousBorder.BorderBrush = new SolidColorBrush(Colors.White);
                previousBorder.BorderThickness = new Thickness(1, 0, 1, 0);
                previousBorder = b;
                b.BorderBrush = new SolidColorBrush(Colors.Orange);
                b.BorderThickness = new Thickness(2);
            }
            cpd = null;
           
        }

        /// <summary>
        /// Adds the recent.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns></returns>
        int addRecent(SolidColorBrush brush)
        {
            int index=0;
            foreach (Color tmp in DS.Recent.ThemeColors.ToArray())
            {              
                if (tmp.Equals(brush.Color))
                    return index;
                index++;
            }
            if (DS.Recent.ThemeColors == null || DS.Recent.ThemeColors.Count <= 9)
            {
                DS.Recent.ThemeColors.Add(brush.Color);
                index = DS.Recent.ThemeColors.Count-1;
            }
            else
            {
                DS.Recent.ThemeColors.Add(brush.Color);
                DS.Recent.ThemeColors = DS.Recent.ThemeColors.GetRange(1, 10);
                index = 9;
            }
            if (Rct != null)
            {
                int i = panel.Children.IndexOf(Rct);
                //panel.Children.RemoveAt(i);
                Rct = (Grid)this.Template.FindName("RctGrid", this);
                getColorGrid(ref Rct,DS.Recent.ThemeColors, false);
                //panel.Children.Insert(i, Rct);
            }
            else
            {
                Label RctHeader = new Label();
                RctHeader.Content = "Recent Colors";
                RctHeader.Margin = new Thickness(0, 5, 0, 5);
                RctHeader.Padding = new Thickness(5, 0, 5, 0);
                RctHeader.FontWeight = FontWeights.Bold;
                Color blue = new Color();
                blue.R = byte.Parse("220"); blue.G = byte.Parse("230"); blue.B = byte.Parse("240");
                LinearGradientBrush lgb = new LinearGradientBrush(Color.FromRgb(blue.R, blue.G, blue.B), Colors.Black, new Point(0, 0.95), new Point(0, 1));
                RctHeader.Background = lgb;
                RctHeader.Foreground = new SolidColorBrush(Colors.MediumBlue);
                //panel.Children.Insert(4,RctHeader);
                Rct=(Grid)this.Template.FindName("RctGrid", this);
                getColorGrid(ref Rct,DS.Recent.ThemeColors, false);
                ((Grid)this.Template.FindName("RctGrid", this)).Visibility = Visibility.Visible;
                ((Label)this.Template.FindName("RctHeader", this)).Visibility = Visibility.Visible;
                
            }
            return index;
        }

        /// <summary>
        /// Handles the MouseDown event of the b control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void b_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border b=e.Source as Border;

            SolidColorBrush brush=b.Background as SolidColorBrush;
            //Pen p=new Pen(brush,1);
            this.SelectedColor = brush.Color;
            previousBorder.BorderBrush = new SolidColorBrush(Colors.White);
            previousBorder.BorderThickness = new Thickness(1,0,1,0);
            previousBorder = b;
            b.BorderBrush = new SolidColorBrush(Colors.Orange);
            b.BorderThickness = new Thickness(2);
            //this.SelectedBrush = brush;            
        }

        /// <summary>
        /// Handles the MouseLeave event of the b control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void b_MouseLeave(object sender, MouseEventArgs e)
        {
            Border bor = e.Source as Border;
            Color col = Colors.White;
            //if((bor.Background as SolidColorBrush).Color!=SelectedColor)
            if(!bor.Equals(previousBorder))
            {
            bor.BorderBrush = new SolidColorBrush(col);
            bor.BorderThickness = new Thickness(1,0,1,0);
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the b control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void b_MouseEnter(object sender, MouseEventArgs e)
        {
            Border bor = e.Source as Border;
            
            bor.BorderBrush = new SolidColorBrush(Colors.Orange);
            bor.BorderThickness = new Thickness(2);

            PreviewColor = (bor.Background as SolidColorBrush).Color;
            PreviewColor = Color.FromRgb(PreviewColor.R, PreviewColor.G, PreviewColor.B);
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (this.PropertyChanged != null)
            {
                handler(this, new PropertyChangedEventArgs("PreviewColor"));
            }
        }



        /// <summary>
        /// Gets the related colors.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        private Color[] getRelatedColors(Color original)
        {
            int x, y, z;
            List<Color> col = new List<Color>();
            x = (int)original.R;//.ScR;
            y = (int)original.G;//.ScG;
            z = (int)original.B;// ScB;
            int avg = (x + y + z) / 3;
            if (avg < 38)
            {
                setLighter(x, y, z, ref col, 5);
            }
            else if (avg < 75)
            {
                setLighter(x, y, z, ref col, 4);
                setDarker(x, y, z, ref col, 1);

            }
            else if (avg < 113)
            {
                setLighter(x, y, z, ref col, 3);
                setDarker(x, y, z, ref col, 2);

            }
            else if (avg < 150)
            {
                setLighter(x, y, z, ref col, 2);
                setDarker(x, y, z, ref col, 3);

            }
            else if (avg < 188)
            {
                setLighter(x, y, z, ref col, 1);
                setDarker(x, y, z, ref col, 4);

            }
            else
            {
                //setLighter(x, y, z, ref col, 0);
                setDarker(x, y, z, ref col, 5);

            }
            
            return col.ToArray();
        }

        /// <summary>
        /// Sets the lighter.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="col">The col.</param>
        /// <param name="lim">The lim.</param>
        private void setLighter(int x, int y, int z, ref List<Color> col, int lim)
        {
            lim++;
            Color temp = new Color();
            for (int i = lim - 2; i >= 0; i--)
            {
                temp.R = byte.Parse((x + (((255 - x) / lim) * (i + 1))).ToString());
                temp.G = byte.Parse((y + (((255 - y) / lim) * (i + 1))).ToString());
                temp.B = byte.Parse((z + (((255 - z) / lim) * (i + 1))).ToString());
                col.Add(Color.FromRgb(temp.R, temp.G, temp.B));
            }
        }

        /// <summary>
        /// Sets the darker.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="col">The col.</param>
        /// <param name="lim">The lim.</param>
        private void setDarker(int x, int y, int z, ref List<Color> col, int lim)
        {
            lim++;
            Color temp = new Color();
            for (int i = 0; i < lim - 1; i++)
            {
                temp.R = byte.Parse((x - (((x) / lim) * (i + 1))).ToString());
                temp.G = byte.Parse((y - (((y) / lim) * (i + 1))).ToString());
                temp.B = byte.Parse((z - (((z) / lim) * (i + 1))).ToString());
                col.Add(Color.FromRgb(temp.R, temp.G, temp.B));
            }
        }

        /// <summary>
        /// Gets or sets the color of the selected.
        /// </summary>
        /// <value>The color of the selected.</value>
        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
            }
        }

        /// <summary>
        /// Represents the selected color, this is a dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register("SelectedColor",
            typeof(Color),
            typeof(RibbonColorPicker),
            new FrameworkPropertyMetadata(Colors.White,new PropertyChangedCallback(OnSelectedColorChanged))
            
            );

        /// <summary>
        /// Called when [selected color changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = (d as RibbonColorPicker).PropertyChanged;
            if (handler != null)
            {
                handler(d, new PropertyChangedEventArgs("Color"));
            }
        }

        /// <summary>
        /// Gets or sets the selected theme.
        /// </summary>
        /// <value>The selected theme.</value>
        public string SelectedTheme
        {
            get
            {
                return (string)GetValue(SelectedThemeProperty);
            }
            set
            {
                SetValue(SelectedThemeProperty, value);
            }
        }

        /// <summary>
        /// Represents the selected theme, this is a dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedThemeProperty =
        DependencyProperty.Register("SelectedTheme",
            typeof(string),
            typeof(RibbonColorPicker),
            new FrameworkPropertyMetadata("Office",new PropertyChangedCallback(OnSelectedThemeChanged))
            
            );

        /// <summary>
        /// Called when [selected theme changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                RibbonColorPicker instance = (RibbonColorPicker)d;
                instance.ApplyTemplate();
                instance.UpdateLayout();
                bool flag = true;
                Grid tempGrid = (Grid)instance.Template.FindName("ThemeGrid", instance);
                foreach (ColorTheme ts in instance.AvailableThemes)
                {
                    if (ts.Name == e.NewValue.ToString())
                    {
                        instance.getColorGrid(ref tempGrid, ts.ThemeColors, true);
                    }
                }

                PropertyChangedEventHandler handler = (d as RibbonColorPicker).PropertyChanged;
                if (handler != null)
                {
                    handler(d, new PropertyChangedEventArgs("Theme"));
                }
            }
            catch (Exception ex) { }

        }

        /// <summary>
        /// Gets the theme.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private ColorTheme getTheme(string name)
        {
            foreach (ColorTheme ts in AvailableThemes)
            {
                if (ts.Name == name)
                {
                    return ts;
                }
            }
            return AvailableThemes[0];
        }
    }

    /// <summary>
    /// Class reperesent the Data source of list of themes
    /// </summary>
    internal class DataSource : List<ColorTheme>
    {
        /// <summary>
        /// Represents the color theme
        /// </summary>
        public ColorTheme theme;

        /// <summary>
        /// Represents the standard theme
        /// </summary>
        public ColorTheme Std;

        /// <summary>
        /// Represents the recent theme
        /// </summary>
        public ColorTheme Recent;

        /// <summary>
        /// Represents the collection of themes
        /// </summary>
        public ObservableCollection<ColorTheme> themes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class.
        /// </summary>
        public DataSource()
        {
            theme = new ColorTheme("Random");
            themes = new ObservableCollection<ColorTheme>(); //new Theme("Random");
            Std = new ColorTheme("Standard");
            Recent = new ColorTheme("Recent");
            theme.ThemeColors = new List<Color>();
            Std.ThemeColors = new List<Color>();
            Recent.ThemeColors = new List<Color>();
           
            List<Color> colorCollection = new List<Color>();
            colorCollection.Add(Colors.DarkRed);
            colorCollection.Add(Colors.Red);
            colorCollection.Add(Colors.Orange);
            colorCollection.Add(Colors.Yellow);
            colorCollection.Add(Colors.LightGreen);
            colorCollection.Add(Colors.Green);
            colorCollection.Add(Colors.LightBlue);
            colorCollection.Add(Colors.Blue);
            colorCollection.Add(Colors.DarkBlue);
            colorCollection.Add(Colors.Purple);
            Std.ThemeColors = colorCollection;

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 238, 236, 225));
            colorCollection.Add(Color.FromArgb(255, 31, 73, 125));
            colorCollection.Add(Color.FromArgb(255, 79, 129, 189));
            colorCollection.Add(Color.FromArgb(255, 192, 80, 77));
            colorCollection.Add(Color.FromArgb(255, 155, 187, 89));
            colorCollection.Add(Color.FromArgb(255, 128, 100, 162));
            colorCollection.Add(Color.FromArgb(255, 75, 172, 198));
            colorCollection.Add(Color.FromArgb(255, 247, 150, 70));
            theme = new ColorTheme("Office");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 248, 248, 248));
            colorCollection.Add(Color.FromArgb(255, 10, 10, 10));
            colorCollection.Add(Color.FromArgb(255, 221, 221, 221));
            colorCollection.Add(Color.FromArgb(255, 178, 178, 178));
            colorCollection.Add(Color.FromArgb(255, 150, 150, 150));
            colorCollection.Add(Color.FromArgb(255, 128, 128, 128));
            colorCollection.Add(Color.FromArgb(255, 95, 95, 95));
            colorCollection.Add(Color.FromArgb(255, 77, 77, 77));
            theme = new ColorTheme("Grayscale");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 201, 194, 209));
            colorCollection.Add(Color.FromArgb(255, 105, 103, 109));
            colorCollection.Add(Color.FromArgb(255, 206, 185, 102));
            colorCollection.Add(Color.FromArgb(255, 156, 176, 132));
            colorCollection.Add(Color.FromArgb(255, 107, 177, 201));
            colorCollection.Add(Color.FromArgb(255, 101, 133, 207));
            colorCollection.Add(Color.FromArgb(255, 126, 107, 201));
            colorCollection.Add(Color.FromArgb(255, 163, 121, 187));
            theme = new ColorTheme("Apex");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 227, 222, 209));
            colorCollection.Add(Color.FromArgb(255, 50, 50, 50));
            colorCollection.Add(Color.FromArgb(255, 240, 127, 9));
            colorCollection.Add(Color.FromArgb(255, 159, 41, 54));
            colorCollection.Add(Color.FromArgb(255, 27, 88, 124));
            colorCollection.Add(Color.FromArgb(255, 78, 133, 66));
            colorCollection.Add(Color.FromArgb(255, 96, 72, 120));
            colorCollection.Add(Color.FromArgb(255, 193, 152, 89));
            theme = new ColorTheme("Aspect");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 197, 209, 215));
            colorCollection.Add(Color.FromArgb(255, 100, 107, 134));
            colorCollection.Add(Color.FromArgb(255, 209, 99, 73));
            colorCollection.Add(Color.FromArgb(255, 204, 180, 0));
            colorCollection.Add(Color.FromArgb(255, 140, 173, 174));
            colorCollection.Add(Color.FromArgb(255, 140, 123, 112));
            colorCollection.Add(Color.FromArgb(255, 143, 176, 140));
            colorCollection.Add(Color.FromArgb(255, 209, 144, 73));
            theme = new ColorTheme("Civic");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 233, 229, 220));
            colorCollection.Add(Color.FromArgb(255, 105, 100, 100));
            colorCollection.Add(Color.FromArgb(255, 211, 72, 23));
            colorCollection.Add(Color.FromArgb(255, 155, 45, 31));
            colorCollection.Add(Color.FromArgb(255, 162, 142, 106));
            colorCollection.Add(Color.FromArgb(255, 149, 98, 81));
            colorCollection.Add(Color.FromArgb(255, 145, 132, 133));
            colorCollection.Add(Color.FromArgb(255, 133, 93, 93));
            theme = new ColorTheme("Equity");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 219, 245, 249));
            colorCollection.Add(Color.FromArgb(255, 4, 97, 123));
            colorCollection.Add(Color.FromArgb(255, 15, 111, 198));
            colorCollection.Add(Color.FromArgb(255, 0, 157, 217));
            colorCollection.Add(Color.FromArgb(255, 11, 208, 217));
            colorCollection.Add(Color.FromArgb(255, 16, 207, 155));
            colorCollection.Add(Color.FromArgb(255, 124, 202, 98));
            colorCollection.Add(Color.FromArgb(255, 165, 194, 73));
            theme = new ColorTheme("Flow");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 234, 235, 222));
            colorCollection.Add(Color.FromArgb(255, 103, 106, 85));
            colorCollection.Add(Color.FromArgb(255, 114, 163, 118));
            colorCollection.Add(Color.FromArgb(255, 176, 204, 176));
            colorCollection.Add(Color.FromArgb(255, 168, 205, 215));
            colorCollection.Add(Color.FromArgb(255, 192, 190, 175));
            colorCollection.Add(Color.FromArgb(255, 206, 197, 151));
            colorCollection.Add(Color.FromArgb(255, 232, 183, 183));
            theme = new ColorTheme("Foundary");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 235, 221, 195));
            colorCollection.Add(Color.FromArgb(255, 119, 95, 85));
            colorCollection.Add(Color.FromArgb(255, 148, 182, 210));
            colorCollection.Add(Color.FromArgb(255, 221, 128, 71));
            colorCollection.Add(Color.FromArgb(255, 165, 171, 129));
            colorCollection.Add(Color.FromArgb(255, 216, 178, 92));
            colorCollection.Add(Color.FromArgb(255, 123, 167, 157));
            colorCollection.Add(Color.FromArgb(255, 150, 140, 140));
            theme = new ColorTheme("Median");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);

            colorCollection = new List<Color>();
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 214, 236, 255));
            colorCollection.Add(Color.FromArgb(255, 78, 91, 111));
            colorCollection.Add(Color.FromArgb(255, 127, 209, 59));
            colorCollection.Add(Color.FromArgb(255, 234, 21, 122));
            colorCollection.Add(Color.FromArgb(255, 254, 184, 10));
            colorCollection.Add(Color.FromArgb(255, 0, 173, 220));
            colorCollection.Add(Color.FromArgb(255, 115, 138, 200));
            colorCollection.Add(Color.FromArgb(255, 26, 179, 159));
            theme = new ColorTheme("Metro");
            theme.ThemeColors = new List<Color>();
            theme.ThemeColors.AddRange(colorCollection.ToArray());
            themes.Add(theme);
        }

    }

    /// <summary>
    /// Class of the color themes
    /// </summary>
    public class ColorTheme
    {
        /// <summary>
        /// Represent the name
        /// </summary>
        string _name;

        /// <summary>
        /// Represent the color list
        /// </summary>
        List<Color> _colors=new List<Color>();

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get { return _name; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorTheme"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ColorTheme(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Gets or sets the theme colors.
        /// </summary>
        /// <value>The theme colors.</value>
        public List<Color> ThemeColors
        {
            get
            {
                return _colors;
            }
            set
            {
                _colors = value;
            }
        }
    }
}
