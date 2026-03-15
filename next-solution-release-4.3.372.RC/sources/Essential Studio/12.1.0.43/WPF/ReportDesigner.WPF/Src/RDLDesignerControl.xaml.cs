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
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.Windows.Edit;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Syncfusion.Windows.Reports.Designer
{
    /// <summary>
    /// Interaction logic for DesignerControl.xaml
    /// </summary>
    internal partial class RDLDesignerControl : UserControl
    {
        public bool IsRdlVisible
        {
            get { return (bool)GetValue(IsRdlVisibleProperty); }
            set { SetValue(IsRdlVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsRdlVisibleProperty =
            DependencyProperty.Register("IsRdlVisible", typeof(bool), typeof(RDLDesignerControl), new UIPropertyMetadata(true, onIsRdlVisiblePropertyChanged));

        public static void onIsRdlVisiblePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RDLDesignerControl designerControl = dependencyObject as RDLDesignerControl;

            if (e.NewValue != e.OldValue)
            {
                bool showRDLtab = (bool)e.NewValue;
                
                if (showRDLtab)
                {
                    designerControl.rdlControl.Visibility = Visibility.Visible;
                    designerControl.designControl.Visibility = Visibility.Collapsed;
                    
                    designerControl.designControl.Content = null;
                    designerControl.designTab.Content = designerControl.DesignControl;
                }
                else
                {
                    designerControl.rdlControl.Visibility = Visibility.Collapsed;
                    designerControl.designControl.Visibility = Visibility.Visible;

                    designerControl.designTab.Content = null;
                    designerControl.designControl.Content = designerControl.DesignControl;
                    
                }
            }
        }

        public DesignPanel DesignControl
        {
            get { return (DesignPanel)GetValue(DesignControlProperty); }
            set { SetValue(DesignControlProperty, value); }
        }

        public static readonly DependencyProperty DesignControlProperty =
            DependencyProperty.Register("DesignControl", typeof(DesignPanel), typeof(RDLDesignerControl), new UIPropertyMetadata(null, DesignControlPropertyChanged));

        public static void DesignControlPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RDLDesignerControl designerControl = dependencyObject as RDLDesignerControl;

            if (e.NewValue != e.OldValue)
            {
                bool showRDLtab = designerControl.IsRdlVisible;

                if (showRDLtab)
                {
                    designerControl.designControl.Content = null;
                    designerControl.designTab.Content = designerControl.DesignControl;
                }
                else
                {
                    designerControl.designTab.Content = null;
                    designerControl.designControl.Content = designerControl.DesignControl;
                }
            }
        }

        public RDLDesignerControl()
        {
            InitializeComponent();
            this.rdlControl.SelectedItemChangedEvent += new Tools.Controls.SelectedItemChangedEventHandler(rdlControl_SelectedItemChangedEvent);
        }

        void rdlControl_SelectedItemChangedEvent(object sender, Tools.Controls.SelectedItemChangedEventArgs e)
        {
            if (e.NewSelectedItem == this.rdlViewTab)
            {
                this.UpdateEditControl();
            }
        }

        void DesignerControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void UpdateEditControl()
        {
            Syncfusion.RDL.DOM.ReportDefinition Report = this.DesignControl.GetReportDefinition();
            string xmlString = RDLString(Report);

            if (!string.Equals(rdlTextFile.Text, xmlString))
            {
                rdlTextFile.Text = xmlString;
            }
        }

        private string RDLString(Syncfusion.RDL.DOM.ReportDefinition Report)
        {
            string xmlString = string.Empty;

            if (Report != null)
            {
                System.Xml.Serialization.XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
                xs.Add("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

                xmlString = this.ToXml(Report, Report.GetType(), xs, "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
                xmlString = xmlString.Replace(">", ">" + Environment.NewLine);
            }

            return xmlString;
        }

        private string ToXml(object Obj, System.Type ObjType, XmlSerializerNamespaces xs, string NameSpace)
        {
            XmlSerializer ser = new XmlSerializer(ObjType, NameSpace);
            MemoryStream memStream = new MemoryStream();
            XmlTextWriter xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
            xmlWriter.Namespaces = true;
            ser.Serialize(xmlWriter, Obj, xs);
            xmlWriter.Close();
            memStream.Close();
            string xml;
            xml = Encoding.UTF8.GetString(memStream.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            return xml;
        }
    }
}
