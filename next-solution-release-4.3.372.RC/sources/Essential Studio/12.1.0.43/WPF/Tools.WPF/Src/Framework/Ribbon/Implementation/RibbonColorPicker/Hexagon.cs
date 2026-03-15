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
using System.Windows.Media.Animation;

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
    [TemplatePart(Name = "Part_Panel", Type = typeof(Canvas))]
    public class Hexagon : Control
    {
        /// <summary>
        /// Represents the first flag
        /// </summary>
        private bool first=true;

        /// <summary>
        /// Represents the canvas panel
        /// </summary>
        Canvas panel;

        /// <summary>
        /// Represents the size which is set to 300
        /// </summary>
        double p_size=300;

        /// <summary>
        /// Represents the lastselected polygon
        /// </summary>
        private Polygon lastSelected = new Polygon();

        /// <summary>
        /// Represents the white polygon
        /// </summary>
        private Polygon white = new Polygon();

        /// <summary>
        /// Represents the black polygon
        /// </summary>
        private Polygon black = new Polygon();

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
        /// Represents the selected color, it is a dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register("SelectedColor",
            typeof(Color),
            typeof(Hexagon)
            );

        /// <summary>
        /// Selecteds the color_ property changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void SelectedColor_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hexagon"/> class.
        /// </summary>
        public Hexagon()
        {
          
            this.Loaded += new RoutedEventHandler(Hexagon_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the Hexagon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Hexagon_Loaded(object sender, RoutedEventArgs e)
        {
            this.ApplyTemplate();
            this.UpdateLayout();

            this.ApplyTemplate();
            this.UpdateLayout();
            ColorPickerDialog cpd = (ColorPickerDialog)Window.GetWindow(this.Parent);
            cpd.ApplyTemplate();
            cpd.UpdateLayout();
            bool flag = false;
            if (first)
            {
                cpd.tabcontrol1 = (TabControl)cpd.cpdp.Template.FindName("tabcontrol1", cpd.cpdp);
                cpd.tabcontrol1.ApplyTemplate();
                cpd.tabcontrol1.UpdateLayout();
                cpd.tabcontrol1.SelectionChanged += new SelectionChangedEventHandler(tabcontrol1_SelectionChanged);
                first = false;
                for (int i = 0; i < panel.Children.Count - 1; i++)
                {
                    cpd.lblNew = (Label)cpd.cpdp.Template.FindName("lblNew", cpd.cpdp);
                    if (cpd.selectedColor.Equals(colorCollection[i]))
                    {
                        Polygon hex = (Polygon)panel.Children[i + 1];
                        hex.Focus();
                        flag = true;
                    }
                }
                if (!flag)
                {
                    cpd.tabcontrol1.SelectedIndex = 1;
                }
            }
            else
            {
                cpd.tabcontrol1 = (TabControl)cpd.cpdp.Template.FindName("tabcontrol1", cpd.cpdp);
                cpd.tabcontrol1.ApplyTemplate();
                cpd.tabcontrol1.UpdateLayout();
                cpd.tabcontrol1.SelectionChanged += new SelectionChangedEventHandler(tabcontrol1_SelectionChanged);
                cpd.lblNew = (Label)cpd.cpdp.Template.FindName("lblNew", cpd.cpdp);
                if (!(this.lastSelected.Fill as SolidColorBrush).Color.Equals((cpd.lblNew.Background as SolidColorBrush).Color))
                {
                    unselectHexagon(lastSelected);
                    Canvas.SetZIndex(Selected, -1);
                }
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the tabcontrol1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void tabcontrol1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ApplyTemplate();
            this.UpdateLayout();
            ColorPickerDialog cpd = (ColorPickerDialog)Window.GetWindow(this.Parent);
            cpd.ApplyTemplate();
            cpd.UpdateLayout();
            bool flag = false;

            for (int i = 0; i < panel.Children.Count - 1; i++)
            {
                cpd.lblNew = (Label)cpd.cpdp.Template.FindName("lblNew", cpd.cpdp);
                cpd.lblNew.ApplyTemplate(); cpd.UpdateLayout();
                if ((cpd.lblNew.Background as SolidColorBrush).Color.Equals(colorCollection[i]))
                {                    
                    Polygon hex = (Polygon)panel.Children[i + 1];
                    if(!hex.Equals(lastSelected))
                        hex.Focus();
                    flag = true;
                }
            }
            if (!flag)
            { }
        }

        /// <summary>
        /// Initializes the <see cref="Hexagon"/> class.
        /// </summary>
        static Hexagon()
        {
            
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Hexagon), new FrameworkPropertyMetadata(typeof(Hexagon)));
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            panel = base.GetTemplateChild("Part_Panel") as Canvas;
            Window.GetWindow(this.Parent).SizeChanged += new SizeChangedEventHandler(Hexagon_SizeChanged);
            refresh();
            
        }

        /// <summary>
        /// Handles the SizeChanged event of the Hexagon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        void Hexagon_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            refresh();
            
        }


        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        private void refresh()
        {
            Window x= Window.GetWindow(this.Parent);
            panel.Width = x.Width - 150;
            panel.Height = x.Height;
            double temp;
            if (panel.ActualHeight > panel.ActualWidth)
                temp = panel.ActualWidth;
            else
                temp = panel.ActualHeight;
            p_size = temp;
            fillPanel();

            
            this.ApplyTemplate();
            this.UpdateLayout();
                ColorPickerDialog cpd = (ColorPickerDialog)Window.GetWindow(this.Parent);
                cpd.ApplyTemplate();
                cpd.UpdateLayout();
                bool flag = false;
            for (int i = 0; i < panel.Children.Count-1; i++)
            {
                cpd.lblNew = (Label)cpd.cpdp.Template.FindName("lblNew", cpd.cpdp);
                cpd.lblNew.ApplyTemplate(); cpd.UpdateLayout();
                if ((cpd.lblNew.Background as SolidColorBrush).Color.Equals(colorCollection[i]))
                {
                    Polygon hex = (Polygon)panel.Children[i+1];
                    hex.Focus();
                    flag = true;
                }
            }
        }

        /// <summary>
        /// Fills the panel.
        /// </summary>
        private void fillPanel()
        {
            panel.Children.RemoveRange(0, panel.Children.Count);
            panel.Children.Remove(Selected);
            panel.Children.Add(Selected);
            Selected.Points = null;
            if(colorCollection.Count<=1)
                setColors();
            int colIndex = 0;

            double p_b = p_size;
            double p_s = p_b / 2;
            double p_h = p_s / 2;
            double p_r = Math.Sqrt(3) / 2 * p_s;
            double p_a = p_r*2;

            double c_b = (p_a / 13) + (1 / 4) * (p_a / 13);
            double c_s = c_b / 2;
            double c_h = c_s / 2;
            double c_r = Math.Sqrt(3) / 2 * c_s;
            double c_a = c_r * 2;

            double x = p_h;
            double y = 0.0;

            
            int lim = 7;
            
            for (int i = 0; i < 13; i++)
            {
                y=i*(c_s+c_h);
                if(i<7)
                    x = x - c_r;
                else
                    x = x + c_r;
                for (int j = 0; j < lim; j++)
                {
                    Polygon hex = new Polygon();
                    hex.Fill = new SolidColorBrush(colorCollection[colIndex++]);
                    hex.Points = getPoints(c_s, 0, c_h);
                    Canvas.SetTop(hex,y);
                    Canvas.SetLeft(hex, x+(c_a*j));
                    panel.Children.Add(hex);

                    hex.MouseDown += new MouseButtonEventHandler(hex_MouseDown);
                    hex.MouseEnter += new MouseEventHandler(hex_MouseEnter);
                    hex.MouseLeave += new MouseEventHandler(hex_MouseLeave);
                    hex.Focusable = true;
                    hex.GotFocus += new RoutedEventHandler(hex_GotFocus);
                    hex.LostFocus += new RoutedEventHandler(hex_LostFocus);

                    hex.KeyDown += new KeyEventHandler(hex_KeyDown);
                }
                if (i < 6)
                    lim++;
                else
                    lim--;
            }
            

            x = 0;
            y = y + 10;

            
            Polygon tem = new Polygon();
            tem.Fill = new SolidColorBrush(colorCollection[colIndex++]);
            tem.Points = getPoints(c_s*2, 0, c_h);
            Canvas.SetTop(tem, 14 * (c_s + c_h));
            Canvas.SetLeft(tem, x + (c_a));
            panel.Children.Add(tem);
            black = tem;
            

            tem.MouseDown += new MouseButtonEventHandler(hex_MouseDown);
            tem.MouseEnter += new MouseEventHandler(hex_MouseEnter);
            tem.MouseLeave += new MouseEventHandler(hex_MouseLeave);
            tem.Focusable = true;
            tem.GotFocus += new RoutedEventHandler(hex_GotFocus);
            tem.LostFocus += new RoutedEventHandler(hex_LostFocus);
            tem.KeyDown += new KeyEventHandler(hex_KeyDown);
           
            tem = new Polygon();
            tem.Fill = new SolidColorBrush(colorCollection[colIndex++]);
            tem.Points = getPoints(c_s*2, 0, c_h);
            Canvas.SetTop(tem, 14 * (c_s + c_h));
            
            Canvas.SetLeft(tem, x + (p_b * (0.65)) + (c_a));
            panel.Children.Add(tem);
            white=tem;
           

            tem.MouseDown += new MouseButtonEventHandler(hex_MouseDown);
            tem.MouseEnter += new MouseEventHandler(hex_MouseEnter);
            tem.MouseLeave += new MouseEventHandler(hex_MouseLeave);
            tem.Focusable = true;
            tem.GotFocus += new RoutedEventHandler(hex_GotFocus);
            tem.LostFocus += new RoutedEventHandler(hex_LostFocus);
            tem.KeyDown += new KeyEventHandler(hex_KeyDown);
            
            lim = 8;
            
            x += p_b * (0.16);
            for (int i = 0; i < 2; i++)
            {
                y = (i + 14) * (c_s + c_h);
                x = x + c_r;
                
                for (int j = 0; j < lim; j++)
                {
                    Polygon hex = new Polygon();
                    hex.Fill = new SolidColorBrush(colorCollection[colIndex++]);
                    hex.Points = getPoints(c_s, 0, c_h);
                    Canvas.SetTop(hex, y);
                    Canvas.SetLeft(hex, x + (c_a * j));
                    panel.Children.Add(hex);
                    

                    hex.MouseDown += new MouseButtonEventHandler(hex_MouseDown);
                    hex.MouseEnter += new MouseEventHandler(hex_MouseEnter);
                    hex.MouseLeave += new MouseEventHandler(hex_MouseLeave);
                    hex.Focusable = true;
                    hex.GotFocus += new RoutedEventHandler(hex_GotFocus);
                    hex.LostFocus += new RoutedEventHandler(hex_LostFocus);
                    hex.KeyDown += new KeyEventHandler(hex_KeyDown);
                    tempy = hex.Points[0].Y;
                    temps = hex.Points[5].Y - tempy;
                }
                lim--;
            }

            tempx = 0;
            
            
        }

        /// <summary>
        /// Handles the KeyDown event of the hex control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void hex_KeyDown(object sender, KeyEventArgs e)
        {
            bool flag = false;
            Polygon hex = (Polygon)sender;
            int i=panel.Children.IndexOf(hex);
            if(((Key.Right)== e.Key))
            {
                if (i < 128)
                    i++;
                else if (i == 128)
                    i = 130;
                else if (i == 129)
                    i = 129;
                else if (i < 137)
                    i = i + 8;
                else if (i == 137)
                    i = 129;
                else if (i < 145)
                    i = i - 7;
                else
                    i++;

                if(i!=panel.Children.Count)
                    hex=(Polygon)panel.Children[i];

                flag = hex.Focus();
                e.Handled = true;
            }
            else if (((Key.Left) == e.Key))
            {
                if (i < 128)
                    i--;
                else if (i == 128)
                    i = 127;
                else if (i == 129)
                    i = 137;
                else if (i == 130)
                    i = 128;
                else if (i < 138)
                    i = i + 7;
                else if (i < 145)
                    i = i - 8;

                if (i >= 0)
                    hex = (Polygon)panel.Children[i];

                flag = hex.Focus();
                e.Handled = true;
            }
            else if (((Key.Up) == e.Key))
            {
                if (i >= 0 && i < 7)
                    i = i - 8;
                else if (i < 15)
                    i = i - 8;
                else if (i < 24)
                    i = i - 9;
                else if (i < 34)
                    i = i - 10;
                else if (i < 45)
                    i = i - 11;
                else if (i < 57)
                    i = i - 12;
                else if (i < 70)
                    i = i - 13;
                //
                else if (i < 82)
                    i = i - 13;
                else if (i < 93)
                    i = i - 12;
                else if (i < 103)
                    i = i - 11;
                else if (i < 112)
                    i = i - 10;
                else if (i < 120)
                    i = i - 9;

                else if (i < 128)
                    i = i - 8;
                else if (i == 128)
                    i = 127;
                else if (i == 129)
                    i = 137;
                else if (i == 130)
                    i = 128;
                else if (i < 138)
                    i = i + 7;
                else if (i < 145)
                    i = i - 8;
                
                if(i>=0)
                    hex = (Polygon)panel.Children[i];
                flag = hex.Focus();
                e.Handled = true;
            }
            else if (((Key.Down) == e.Key))
            {
                if (i >= 0 && i < 7)
                    i = i + 8;
                else if (i < 15)
                    i = i + 9;
                else if (i < 24)
                    i = i + 10;
                else if (i < 34)
                    i = i + 11;
                else if (i < 45)
                    i = i + 12;
                else if (i < 57)
                    i = i + 13;
                else if (i < 70)
                    i = i + 13;
                //
                else if (i < 82)
                    i = i + 12;
                else if (i < 93)
                    i = i + 11;
                else if (i < 103)
                    i = i + 10;
                else if (i < 112)
                    i = i + 9;
                else if (i < 120)
                    i = i + 8;

                else if (i < 128)
                    i++;
                else if (i == 128)
                    i = 130;
                else if (i == 129)
                    i = 129;
                else if (i < 137)
                    i = i + 8;
                else if (i == 137)
                    i = 129;
                else if (i < 145)
                    i = i - 7;
                
                hex = (Polygon)panel.Children[i];
                flag = hex.Focus(); 
                e.Handled = true;
            }
            
        }

        /// <summary>
        /// Handles the LostFocus event of the hex control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void hex_LostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            
        }

        /// <summary>
        /// Handles the GotFocus event of the hex control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void hex_GotFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            selectHexagon((Polygon)sender);
        }

        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private PointCollection getPoints(double s, double x, double y)
        {
            PointCollection points = new PointCollection();
            
            double sin = 1d / 2;
            double cos = Math.Sqrt(3) / 2;
            double h = sin * s;
            double r = cos * s;
            double b = s + 2 * h;
            double a = 2 * r;

            

            points.Add(new Point(x, y));
            points.Add(new Point(x + r, y - h));
            points.Add(new Point(x + a, y));
            points.Add(new Point(x + a, y + s));
            points.Add(new Point(x + r, y + s + h));
            points.Add(new Point(x, y + s));
            return points;
        }

        /// <summary>
        /// Handles the MouseLeave event of the hex control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void hex_MouseLeave(object sender, MouseEventArgs e)
        {
           
        }

        /// <summary>
        /// Handles the MouseEnter event of the hex control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void hex_MouseEnter(object sender, MouseEventArgs e)
        {
            if(!(sender as Polygon).Equals(lastSelected) && e.LeftButton == MouseButtonState.Pressed)
                selectHexagon((Polygon)sender);
        }

        /// <summary>
        /// Represents the temp z index
        /// </summary>
        int tempZIndex;

        /// <summary>
        /// Represents the temp variable
        /// </summary>
        double temps;

        /// <summary>
        /// Represents the temp x
        /// </summary>
        double tempx;

        /// <summary>
        /// Represents the temp y
        /// </summary>
        double tempy;

        /// <summary>
        /// Represents the selected polygon
        /// </summary>
        Polygon Selected = new Polygon();

        /// <summary>
        /// Selects the hexagon.
        /// </summary>
        /// <param name="hex">The hex.</param>
        void selectHexagon(Polygon hex)
        {
            
            if (hex == lastSelected)
                return;
            double s_s, s_x, s_y;
            s_x = 0;
            s_y = hex.Points[0].Y;
            s_s = hex.Points[5].Y - s_y;
            Selected.Points = getPoints(s_s+1.4, s_x-1, s_y-1);
            Selected.Fill = new SolidColorBrush(Colors.White);
            Selected.Stroke = new SolidColorBrush(Colors.Black);
            Selected.StrokeThickness = 1;
            Canvas.SetTop(Selected, Canvas.GetTop(hex));
            Canvas.SetLeft(Selected, Canvas.GetLeft(hex));
            Canvas.SetZIndex(Selected, 300);
            
            tempZIndex = Canvas.GetZIndex(hex);
            Canvas.SetZIndex(hex, 311);
            
            hex.Stroke = new SolidColorBrush(Colors.Black);
            hex.Style = null;
           
            tempx = 0;
            
            if (hex != lastSelected)
                hex.Points = getPoints(s_s * 0.8, Math.Sqrt(3) * s_s * 0.09, s_y + s_s * 0.09);
            hex.StrokeThickness = 1;
            ColorPickerDialog cpd=(ColorPickerDialog) Window.GetWindow(this.Parent);
            cpd.lblNew=(Label) cpd.cpdp.Template.FindName("lblNew", cpd.cpdp);
            cpd.lblNew.Background = hex.Fill;
            if(hex!=lastSelected)
                unselectHexagon(lastSelected);
            lastSelected = hex;
            
        }

        /// <summary>
        /// Unselects the hexagon.
        /// </summary>
        /// <param name="hex">The hex.</param>
        void unselectHexagon(Polygon hex)
        {
            
            Canvas.SetZIndex(hex, tempZIndex);
            PointCollection points = hex.Points;
            

            hex.Stroke = new SolidColorBrush(Colors.White);
            hex.StrokeThickness = 0;
            if(hex == white || hex==black)
            {
                hex.Points = getPoints(temps*2, tempx, tempy);
            }
            else
            hex.Points = getPoints(temps, tempx, tempy);
            
        }

        /// <summary>
        /// Handles the MouseDown event of the hex control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void hex_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Polygon)sender).Focus();
            
        }

        /// <summary>
        /// Represents the color collection list
        /// </summary>
        List<Color> colorCollection = new List<Color>();

        /// <summary>
        /// Sets the colors.
        /// </summary>
        void setColors()
        {
            //
            colorCollection.Add(Color.FromArgb(255, 0, 51, 102));
            colorCollection.Add(Color.FromArgb(255, 51, 102, 153));
            colorCollection.Add(Color.FromArgb(255, 51, 102, 204));
            colorCollection.Add(Color.FromArgb(255, 0, 51, 153));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 153));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 204));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 102));
            //
            colorCollection.Add(Color.FromArgb(255, 0, 102, 102));
            colorCollection.Add(Color.FromArgb(255, 0, 102, 153));
            colorCollection.Add(Color.FromArgb(255, 0, 153, 204));
            colorCollection.Add(Color.FromArgb(255, 0, 102, 204));
            colorCollection.Add(Color.FromArgb(255, 0, 51, 204));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 255));
            colorCollection.Add(Color.FromArgb(255, 51, 51, 255));
            colorCollection.Add(Color.FromArgb(255, 51, 51, 153));
            //
            colorCollection.Add(Color.FromArgb(255, 0, 128, 128));
            colorCollection.Add(Color.FromArgb(255, 0, 153, 153));
            colorCollection.Add(Color.FromArgb(255, 51, 204, 204));
            colorCollection.Add(Color.FromArgb(255, 0, 204, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 153, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 102, 255));
            colorCollection.Add(Color.FromArgb(255, 51, 102, 255));
            colorCollection.Add(Color.FromArgb(255, 51, 51, 204));
            colorCollection.Add(Color.FromArgb(255, 102, 102, 153));
            //
            colorCollection.Add(Color.FromArgb(255, 51, 153, 102));
            colorCollection.Add(Color.FromArgb(255, 0, 204, 153));
            colorCollection.Add(Color.FromArgb(255, 0, 255, 204));
            colorCollection.Add(Color.FromArgb(255, 0, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 51, 204, 255));
            colorCollection.Add(Color.FromArgb(255, 51, 153, 255));
            colorCollection.Add(Color.FromArgb(255, 102, 153, 255));
            colorCollection.Add(Color.FromArgb(255, 102, 102, 255));
            colorCollection.Add(Color.FromArgb(255, 102, 0, 255));
            colorCollection.Add(Color.FromArgb(255, 102, 0, 204));
            //
            colorCollection.Add(Color.FromArgb(255, 51, 153, 51));
            colorCollection.Add(Color.FromArgb(255, 0, 204, 102));
            colorCollection.Add(Color.FromArgb(255, 0, 255, 153));
            colorCollection.Add(Color.FromArgb(255, 102, 255, 204));
            colorCollection.Add(Color.FromArgb(255, 102, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 102, 204, 255));
            colorCollection.Add(Color.FromArgb(255, 153, 204, 255));
            colorCollection.Add(Color.FromArgb(255, 153, 153, 255));
            colorCollection.Add(Color.FromArgb(255, 153, 102, 255));
            colorCollection.Add(Color.FromArgb(255, 153, 51, 255));
            colorCollection.Add(Color.FromArgb(255, 153, 0, 255));
            //
            colorCollection.Add(Color.FromArgb(255, 0, 102, 0));
            colorCollection.Add(Color.FromArgb(255, 0, 204, 0));
            colorCollection.Add(Color.FromArgb(255, 0, 255, 0));
            colorCollection.Add(Color.FromArgb(255, 102, 255, 153));
            colorCollection.Add(Color.FromArgb(255, 153, 255, 204));
            colorCollection.Add(Color.FromArgb(255, 204, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 204, 236, 255));
            colorCollection.Add(Color.FromArgb(255, 204, 204, 255));
            colorCollection.Add(Color.FromArgb(255, 204, 153, 255));
            colorCollection.Add(Color.FromArgb(255, 204, 102, 255));
            colorCollection.Add(Color.FromArgb(255, 204, 0, 255));
            colorCollection.Add(Color.FromArgb(255, 153, 0, 204));
            //
            colorCollection.Add(Color.FromArgb(255, 0, 51, 0));
            colorCollection.Add(Color.FromArgb(255, 0, 128, 0));
            colorCollection.Add(Color.FromArgb(255, 51, 204, 51));
            colorCollection.Add(Color.FromArgb(255, 102, 255, 102));
            colorCollection.Add(Color.FromArgb(255, 153, 255, 153));
            colorCollection.Add(Color.FromArgb(255, 204, 255, 204));
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 255, 204, 255));
            colorCollection.Add(Color.FromArgb(255, 255, 153, 255));
            colorCollection.Add(Color.FromArgb(255, 255, 102, 255));
            colorCollection.Add(Color.FromArgb(255, 255, 0, 255));
            colorCollection.Add(Color.FromArgb(255, 204, 0, 204));
            colorCollection.Add(Color.FromArgb(255, 102, 0, 102));
            //
            colorCollection.Add(Color.FromArgb(255, 51, 102, 0));
            colorCollection.Add(Color.FromArgb(255, 0, 153, 0));
            colorCollection.Add(Color.FromArgb(255, 102, 255, 51));
            colorCollection.Add(Color.FromArgb(255, 153, 255, 102));
            colorCollection.Add(Color.FromArgb(255, 204, 255, 153));
            colorCollection.Add(Color.FromArgb(255, 255, 255, 204));
            colorCollection.Add(Color.FromArgb(255, 255, 204, 204));
            colorCollection.Add(Color.FromArgb(255, 255, 153, 204));
            colorCollection.Add(Color.FromArgb(255, 255, 102, 204));
            colorCollection.Add(Color.FromArgb(255, 255, 51, 204));
            colorCollection.Add(Color.FromArgb(255, 204, 0, 153));
            colorCollection.Add(Color.FromArgb(255, 128, 0, 128));
            //
            colorCollection.Add(Color.FromArgb(255, 51, 51, 0));
            colorCollection.Add(Color.FromArgb(255, 102, 153, 0));
            colorCollection.Add(Color.FromArgb(255, 153, 255, 51));
            colorCollection.Add(Color.FromArgb(255, 204, 255, 102));
            colorCollection.Add(Color.FromArgb(255, 255, 255, 153));
            colorCollection.Add(Color.FromArgb(255, 255, 204, 153));
            colorCollection.Add(Color.FromArgb(255, 255, 153, 153));
            colorCollection.Add(Color.FromArgb(255, 255, 102, 153));
            colorCollection.Add(Color.FromArgb(255, 255, 51, 153));
            colorCollection.Add(Color.FromArgb(255, 204, 51, 153));
            colorCollection.Add(Color.FromArgb(255, 153, 0, 153));
            //
            colorCollection.Add(Color.FromArgb(255, 102, 102, 51));
            colorCollection.Add(Color.FromArgb(255, 153, 204, 0));
            colorCollection.Add(Color.FromArgb(255, 204, 255, 51));
            colorCollection.Add(Color.FromArgb(255, 255, 255, 102));
            colorCollection.Add(Color.FromArgb(255, 255, 204, 102));
            colorCollection.Add(Color.FromArgb(255, 255, 153, 102));
            colorCollection.Add(Color.FromArgb(255, 255, 124, 128));
            colorCollection.Add(Color.FromArgb(255, 255, 0, 102));
            colorCollection.Add(Color.FromArgb(255, 214, 0, 147));
            colorCollection.Add(Color.FromArgb(255, 153, 51, 102));
            //
            colorCollection.Add(Color.FromArgb(255, 128, 128, 0));
            colorCollection.Add(Color.FromArgb(255, 204, 204, 0));
            colorCollection.Add(Color.FromArgb(255, 255, 255, 0));
            colorCollection.Add(Color.FromArgb(255, 255, 204, 0));
            colorCollection.Add(Color.FromArgb(255, 255, 153, 51));
            colorCollection.Add(Color.FromArgb(255, 255, 102, 0));
            colorCollection.Add(Color.FromArgb(255, 255, 80, 80));
            colorCollection.Add(Color.FromArgb(255, 204, 0, 102));
            colorCollection.Add(Color.FromArgb(255, 102, 0, 51));
            //
            colorCollection.Add(Color.FromArgb(255, 153, 102, 51));
            colorCollection.Add(Color.FromArgb(255, 204, 153, 0));
            colorCollection.Add(Color.FromArgb(255, 255, 153, 0));
            colorCollection.Add(Color.FromArgb(255, 204, 102, 0));
            colorCollection.Add(Color.FromArgb(255, 255, 51, 0));
            colorCollection.Add(Color.FromArgb(255, 255, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 204, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 153, 0, 51));
            //
            colorCollection.Add(Color.FromArgb(255, 102, 51, 0));
            colorCollection.Add(Color.FromArgb(255, 153, 102, 0));
            colorCollection.Add(Color.FromArgb(255, 204, 51, 0));
            colorCollection.Add(Color.FromArgb(255, 153, 51, 0));
            colorCollection.Add(Color.FromArgb(255, 153, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 128, 0, 0));
            colorCollection.Add(Color.FromArgb(255, 165, 0, 33));
            colorCollection.Add(Color.FromArgb(255, 255, 255, 255));
            colorCollection.Add(Color.FromArgb(255, 0, 0, 0));
            //
            colorCollection.Add(Color.FromArgb(255, 248, 248, 248));
            colorCollection.Add(Color.FromArgb(255, 221, 221, 221));
            colorCollection.Add(Color.FromArgb(255, 178, 178, 178));
            colorCollection.Add(Color.FromArgb(255, 128, 128, 128));
            colorCollection.Add(Color.FromArgb(255, 95, 95, 95));
            colorCollection.Add(Color.FromArgb(255, 51, 51, 51));
            colorCollection.Add(Color.FromArgb(255, 28, 28, 28));
            colorCollection.Add(Color.FromArgb(255, 8, 8, 8));
            //
            colorCollection.Add(Color.FromArgb(255, 234, 234, 234));
            colorCollection.Add(Color.FromArgb(255, 192, 192, 192));
            colorCollection.Add(Color.FromArgb(255, 150, 150, 150));
            colorCollection.Add(Color.FromArgb(255, 119, 119, 119));
            colorCollection.Add(Color.FromArgb(255, 77, 77, 77));
            colorCollection.Add(Color.FromArgb(255, 41, 41, 41));
            colorCollection.Add(Color.FromArgb(255, 17, 17, 17));
            //


        }
       
    }

}
