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
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Controls.Map
{

    /// <summary>
    ///  MapShape is a graphical shape that defines the area of the map content
    /// </summary>
    public class MapShapes : ContentControl
    {
        #region Private Fields
        private static ResourceDictionary resource = new ResourceDictionary();
        private ShapeFileLayer shapeFileLayer;
#if WPF
        private BindingExpression fillExp;
        private BindingExpression strokeExp;
        private BindingExpression strokeThicknessExp;
        private Brush tempFill;
        private Brush tempStroke;
        private double tempStrokeThickness;
#endif        
        #endregion

        #region  Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.MapShapes">MapShapes</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public MapShapes()
        {
            this.DefaultStyleKey = typeof(MapShapes);           
#if SILVERLIGHT
            if (MapShapes.resource.Source.ToString() != "/Syncfusion.Maps.Silverlight;component/Themes/Generic.xaml")
            {
                MapShapes.resource.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            }
            if (resource != null)
            {
                this.Style = resource["PART_ShapeStyle"] as Style;
            }
#endif
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.shapeFileLayer = MapControl.FindParent<ShapeFileLayer>(this);
#if WPF
            this.MouseEnter += new System.Windows.Input.MouseEventHandler(MapShapes_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(MapShapes_MouseLeave);
#endif
                             

         }

#if WPF

        void MapShapes_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.shapeFileLayer.EnableHoverEffects)
            {
                if (this.fillExp != null)
                {
                    this.Shape.SetBinding(Path.FillProperty, this.fillExp.ParentBinding);
                    this.fillExp = null;
                }
                else
                {
                    this.Shape.Fill = this.tempFill;
                }
                if (this.strokeExp != null)
                {
                    this.Shape.SetBinding(Path.StrokeProperty, this.strokeExp.ParentBinding);
                    this.strokeExp = null;
                }
                else
                {
                    this.Shape.Stroke = this.tempStroke;
                }
                if (this.strokeThicknessExp != null)
                {
                    this.Shape.SetBinding(Path.StrokeThicknessProperty, this.strokeThicknessExp.ParentBinding);
                    this.strokeThicknessExp = null;
                }
                else
                {
                    this.Shape.StrokeThickness = this.tempStrokeThickness;
                }
            }
            this.shapeFileLayer.MouseOverItem = null;
        }

        void MapShapes_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            shapeFileLayer.RaiseHoverChanged(this);
            if (this.shapeFileLayer.EnableHoverEffects)
            {
                if (this.Shape.GetBindingExpression(Path.FillProperty) != null)
                {
                    this.fillExp = this.Shape.GetBindingExpression(Path.FillProperty);
                }
                else
                {
                    this.tempFill = this.Shape.Fill;
                }
                if (this.Shape.GetBindingExpression(Path.StrokeProperty) != null)
                {
                    this.strokeExp = this.Shape.GetBindingExpression(Path.StrokeProperty);
                }
                else
                {
                    this.tempStroke = this.Shape.Stroke;
                }
                if (this.Shape.GetBindingExpression(Path.StrokeThicknessProperty) != null)
                {
                    this.strokeThicknessExp = this.Shape.GetBindingExpression(Path.StrokeThicknessProperty);
                }
                else
                {
                    this.tempStrokeThickness = this.Shape.StrokeThickness;
                }
                if (this.shapeFileLayer.EnableHoverEffects)
                {
                    this.Shape.SetBinding(Path.FillProperty, new Binding { Source = this.shapeFileLayer, Path = new PropertyPath("ShapeHoverFill") });
                    this.Shape.SetBinding(Path.StrokeProperty, new Binding { Source = this.shapeFileLayer, Path = new PropertyPath("ShapeHoverStroke") });
                    this.Shape.SetBinding(Path.StrokeThicknessProperty, new Binding { Source = this.shapeFileLayer, Path = new PropertyPath("ShapeHoverStrokeThickness") });
         
                }
                this.shapeFileLayer.MouseOverItem = this;               
            }
        }
#endif

        #endregion

        #region Properties

        #region Shape(Dependency Property)

        /// <summary>
        /// Gets or sets Path of the Map Shape.
        /// </summary>
        /// <value>
        /// Path
        /// </value>
        public Path Shape
        {
            get { return (Path)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Shape.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register("Shape", typeof(Path), typeof(MapShapes), new PropertyMetadata(null));

        #endregion

        #region ShapeInfo(Dependency Property)

        /// <summary>
        /// Gets or sets ShapeInformation of map Shapes.
        /// </summary>
        /// <value>
        /// ShapeInfo
        /// </value>
        public List<object> ShapeInfo
        {
            get { return (List<object>)GetValue(ShapeInfoProperty); }
            set { SetValue(ShapeInfoProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShapeInfo.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShapeInfoProperty =
            DependencyProperty.Register("ShapeInfo", typeof(List<object>), typeof(MapShapes), new PropertyMetadata(new List<object>()));

        #endregion

        #region ShapeDetails




        /// <summary>
        /// Gets or sets Details Of the Map Shapes.
        /// </summary>
        /// <value>
        /// Collection of ShapeDetails
        /// </value>
        public ObservableCollection<ShapeDetails> ShapeDetails
        {
            get { return (ObservableCollection<ShapeDetails>)GetValue(ShapeDetailsProperty); }
            set { SetValue(ShapeDetailsProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShapeDetails.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShapeDetailsProperty =
            DependencyProperty.Register("ShapeDetails", typeof(ObservableCollection<ShapeDetails>), typeof(MapShapes), new PropertyMetadata(new ObservableCollection<ShapeDetails>()));



        #endregion

        #endregion

    }
    /// <summary>
    ///  ShapeDetails Consist of Shape Properties like Key and Value
    /// </summary>
    public class ShapeDetails
    {
        string _key=string.Empty;
        object _value=null;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ShapeDetails">ShapeDetails</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ShapeDetails()
        {

        }

        /// <summary>
        /// Gets or sets Key Value of map Shape.
        /// </summary>
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        /// <summary>
        /// Gets or sets Value of map Shape.
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }

}
