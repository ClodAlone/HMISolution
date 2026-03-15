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
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;

#if !SILVERLIGHT
namespace Syncfusion.Windows.Controls.PivotGrid
#else
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Class that holds the members of PivotGridCellStyle
    /// </summary>
    public class PivotGridCellStyle : DependencyObject, INotifyPropertyChanged, IXmlSerializable
    {
        #region [ Variable Declarations ]
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.Background"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.Background"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty BackgroundProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("Background", typeof(Brush), typeof(PivotGridCellStyle), new UIPropertyMetadata(null,PivotGridCellStyle.OnBackgroundChanged));
#else
            DependencyProperty.Register("Background", typeof(Brush), typeof(PivotGridCellStyle), new PropertyMetadata(null,PivotGridCellStyle.OnBackgroundChanged));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.FontFamily"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.FontFamily"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FontFamilyProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(PivotGridCellStyle), new UIPropertyMetadata(new FontFamily("Segoe UI"),PivotGridCellStyle.OnFontFamilyChanged));
#else
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(PivotGridCellStyle), new PropertyMetadata(new FontFamily("Segoe UI"),PivotGridCellStyle.OnFontFamilyChanged));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.FontSize"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.FontSize"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FontSizeProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("FontSize", typeof(int), typeof(PivotGridCellStyle), new UIPropertyMetadata(12,PivotGridCellStyle.OnFontSizeChanged));
#else
            DependencyProperty.Register("FontSize", typeof(int), typeof(PivotGridCellStyle), new PropertyMetadata(12,PivotGridCellStyle.OnFontSizeChanged));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.FontWeight"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.FontWeight"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FontWeightProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(PivotGridCellStyle), new UIPropertyMetadata(new FontWeight(),PivotGridCellStyle.OnFontWeightChanged));
#else
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(PivotGridCellStyle), new PropertyMetadata(new FontWeight(),PivotGridCellStyle.OnFontWeightChanged));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.Foreground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.Foreground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ForegroundProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(PivotGridCellStyle), new UIPropertyMetadata(Brushes.Black,PivotGridCellStyle.OnForegroundChanged));
#else
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(PivotGridCellStyle), new PropertyMetadata(new SolidColorBrush(Colors.Black),PivotGridCellStyle.OnForegroundChanged));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.Style"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.Style"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty StyleProperty =
#if !SILVERLIGHT
           DependencyProperty.Register("Style", typeof(Style), typeof(PivotGridCellStyle), new UIPropertyMetadata(null, PivotGridCellStyle.OnStyleChanged));
#else
           DependencyProperty.Register("Style", typeof(Style), typeof(PivotGridCellStyle), new PropertyMetadata(null, PivotGridCellStyle.OnStyleChanged));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.IsHyperlinkCell"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.IsHyperlinkCell"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty HyperlinkCellProperty =
#if !SILVERLIGHT
           DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(PivotGridCellStyle), new UIPropertyMetadata(false, PivotGridCellStyle.OnHyperlinkChanged));
#else
           DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(PivotGridCellStyle), new PropertyMetadata(false, PivotGridCellStyle.OnHyperlinkChanged));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.EnableContextMenu"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.EnableContextMenu"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty EnableContextMenuProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("EnableContextMenu", typeof(bool), typeof(PivotGridCellStyle), new UIPropertyMetadata(false, PivotGridCellStyle.OnEnableContextMenuChanged));
#else
 DependencyProperty.Register("EnableContextMenu", typeof(bool), typeof(PivotGridCellStyle), new PropertyMetadata(false, PivotGridCellStyle.OnEnableContextMenuChanged));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.ToolTipEnabled"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.ToolTipEnabled"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ToolTipEnabledProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ToolTipEnabled", typeof(bool), typeof(PivotGridCellStyle), new UIPropertyMetadata(false, PivotGridCellStyle.OnToolTipEnabledPropertyChanged));
#else
 DependencyProperty.Register("ToolTipEnabled", typeof(bool), typeof(PivotGridCellStyle), new PropertyMetadata(false, PivotGridCellStyle.OnToolTipEnabledPropertyChanged));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.CustomToolTipTemplateKey"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridCellStyle.CustomToolTipTemplateKey"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty CustomToolTipTemplateKeyProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("CustomToolTipTemplateKey", typeof(string), typeof(PivotGridCellStyle), new UIPropertyMetadata(null, PivotGridCellStyle.CustomToolTipTemplateKeyPropertyChanged));
#else
 DependencyProperty.Register("CustomToolTipTemplateKey", typeof(string), typeof(PivotGridCellStyle), new PropertyMetadata(null, PivotGridCellStyle.CustomToolTipTemplateKeyPropertyChanged));
#endif
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }



        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>The font weight.</value>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>The foreground.</value>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the internal source style.
        /// </summary>
        /// <value>The internal source style.</value>
        internal PivotGridCellStyle InternalSourceStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        [Browsable(false)]
        public Style Style
        {
            get { return (Style)GetValue(StyleProperty); }
            set { SetValue(StyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the grid cells are hyperlink
        /// </summary>
        public bool IsHyperlinkCell
        {
            get { return (bool)GetValue(HyperlinkCellProperty); }
            set { SetValue(HyperlinkCellProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the grid cells can have ContextMenu (Applies only for Expander Cells)
        /// </summary>
        public bool EnableContextMenu
        {
            get { return (bool)GetValue(EnableContextMenuProperty); }
            set { SetValue(EnableContextMenuProperty, value); }
        }


        /// <summary>
        /// Gets or sets whether the grid cells style can have ToolTip
        /// </summary>
        public bool ToolTipEnabled
        {
            get { return (bool)GetValue(ToolTipEnabledProperty); }
            set { SetValue(ToolTipEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets Custom ToolTip template Key
        /// </summary>
        public string CustomToolTipTemplateKey
        {
            get { return (string)GetValue(CustomToolTipTemplateKeyProperty); }
            set { SetValue(CustomToolTipTemplateKeyProperty, value); }
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="info">The info.</param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        #region [ Events ]
        /// <summary>
        /// An event that notifies user whenever property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region [ Dependency Properties Implementation ]

        /// <summary>
        /// Called when [background changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnBackgroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("Background");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.Background = gridCellStyle.Background;
            }
        }

        /// <summary>
        /// Called when [font family changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnFontFamilyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("FontFamily");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.FontFamily = gridCellStyle.FontFamily;
            }
        }

        /// <summary>
        /// Called when [font size changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnFontSizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("FontSize");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.FontSize = gridCellStyle.FontSize;
            }
        }

        /// <summary>
        /// Called when [font weight changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnFontWeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("FontWeight");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.FontWeight = gridCellStyle.FontWeight;
            }
        }

        /// <summary>
        /// Called when [foreground changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnForegroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("Foreground");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.Foreground = gridCellStyle.Foreground;
            }
        }

        /// <summary>
        /// Called when [style changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("Style");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.Style = gridCellStyle.Style;
            }
        }

        /// <summary>
        /// Called when [hyperlink changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnHyperlinkChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("IsHyperlinkCell");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.IsHyperlinkCell = gridCellStyle.IsHyperlinkCell;

            }
        }
        /// <summary>
        /// Called when [EnableContextMenu changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
      
        public static void OnEnableContextMenuChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("EnableContextMenu");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.EnableContextMenu = gridCellStyle.EnableContextMenu;

            }
        }

        static void OnToolTipEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("ToolTipEnabled");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.ToolTipEnabled  = gridCellStyle.ToolTipEnabled;
            }
        }

        static void CustomToolTipTemplateKeyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                gridCellStyle.NotifyPropertyChanged("CustomToolTipTemplateKey");
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.CustomToolTipTemplateKey = gridCellStyle.CustomToolTipTemplateKey;
            }
        }
        
        #endregion

        #region [ IXmlSerializable Members ]
        /// <summary>
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable"/> interface, you should return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml"/> method.
        /// </returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.Read())
            {
#if !SILVERLIGHT
                reader.ReadStartElement("Background");
                this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(reader.ReadString()));
                reader.ReadEndElement();
                reader.ReadStartElement("FontFamily");
                this.FontFamily = new FontFamily(reader.ReadString());
                reader.ReadEndElement();
                reader.ReadStartElement("FontSize");
                this.FontSize = int.Parse(reader.ReadString());
                reader.ReadEndElement();
                reader.ReadStartElement("FontWeight");
                this.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(reader.ReadString());
                reader.ReadEndElement();
                reader.ReadStartElement("Foreground");
                this.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(reader.ReadString()));
                reader.ReadEndElement();
#else
                reader.ReadStartElement("Background");
                this.Background = Common.GetColorFromHexaDecimal(reader.ReadContentAsString());
                reader.ReadEndElement();
                reader.ReadStartElement("FontFamily");
                this.FontFamily = new FontFamily(reader.ReadContentAsString());
                reader.ReadEndElement();
                reader.ReadStartElement("FontSize");
                this.FontSize = reader.ReadContentAsInt();
                reader.ReadEndElement();
                reader.ReadStartElement("FontWeight");
                this.FontWeight = Common.GetFontWeightFromString(reader.ReadContentAsString());
                reader.ReadEndElement();
                reader.ReadStartElement("Foreground");
                this.Foreground = Common.GetColorFromHexaDecimal(reader.ReadContentAsString());
                reader.ReadEndElement();
#endif
                reader.ReadStartElement("IsHyperlinkCell");
                this.IsHyperlinkCell = reader.ReadContentAsBoolean();
                reader.ReadEndElement();
                reader.ReadStartElement("EnableContextMenu");
                this.EnableContextMenu = reader.ReadContentAsBoolean();
                reader.ReadEndElement();
                reader.ReadStartElement("ToolTipEnabled");
                this.ToolTipEnabled = reader.ReadContentAsBoolean();
                reader.ReadEndElement();
                reader.Read();
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (this.Background != null && this.Background is SolidColorBrush)
            {
                writer.WriteStartElement("Background");
                writer.WriteValue((this.Background as SolidColorBrush).Color.ToString());
                writer.WriteEndElement();
            }
            writer.WriteStartElement("FontFamily");
            writer.WriteValue(this.FontFamily.Source);
            writer.WriteEndElement();
            writer.WriteStartElement("FontSize");
            writer.WriteValue(this.FontSize);
            writer.WriteEndElement();
            writer.WriteStartElement("FontWeight");
            writer.WriteValue(this.FontWeight.ToString());
            writer.WriteEndElement();
            if (this.Foreground != null && this.Foreground is SolidColorBrush)
            {
                writer.WriteStartElement("Foreground");
                writer.WriteValue((this.Foreground as SolidColorBrush).Color.ToString());
                writer.WriteEndElement();
            }
            writer.WriteStartElement("IsHyperlinkCell");
            writer.WriteValue(this.IsHyperlinkCell);
            writer.WriteEndElement();
            writer.WriteStartElement("EnableContextMenu");
            writer.WriteValue(this.EnableContextMenu);
            writer.WriteEndElement();
            writer.WriteStartElement("ToolTipEnabled");
            writer.WriteValue(this.ToolTipEnabled);
            writer.WriteEndElement();
        } 
        #endregion
    }
}
