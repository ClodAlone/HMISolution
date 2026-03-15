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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Design.Controls
{
    public class ImagePicker : Control//, IComponentConnector
    {
        static ImagePicker( )
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata( typeof( ImagePicker ), new FrameworkPropertyMetadata( typeof( ImagePicker ) ) );
        }

        public ImagePicker( )
        {
            //InitializeComponent( );

            //EnvDTE.DTE dte = ( EnvDTE.DTE )System.Runtime.InteropServices.Marshal.GetActiveObject( "VisualStudio.DTE" );
            //string folder = System.IO.Path.GetDirectoryName( dte.ActiveDocument.FullName );
        }

        #region IComponentConnector Members

        public void Connect( int connectionId, object target )
        {
            throw new NotImplementedException( );
        }

        public void InitializeComponent( )
        {
            Uri resourceLocator = new Uri( "/Syncfusion.Tools.WPF.VisualStudio.Design;component/Controls/ImagePicker/Themes/ImagePicker.xaml", UriKind.Relative );
            Application.LoadComponent( this, resourceLocator );
        }

        #endregion

        /// <summary>
        /// Gets or sets the value of the ImageSource dependency property.
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return ( ImageSource )GetValue( ImageSourceProperty );
            }
            set
            {
                SetValue( ImageSourceProperty, value );
            }
        }

        /// <summary>
        /// Event that is raised when ImageSource property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageSourceChanged;

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register( "ImageSource", typeof( ImageSource ), typeof( ImagePicker ), new FrameworkPropertyMetadata( null, new PropertyChangedCallback( OnImageSourceChanged ) ) );

        /// <summary>
        /// Calls OnImageSourceChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnImageSourceChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ImagePicker instance = ( ImagePicker )d;
            instance.OnImageSourceChanged( e );
        }
        /// <summary>
        /// Updates property value cache and raises ImageSourceChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnImageSourceChanged( DependencyPropertyChangedEventArgs e )
        {
            if( ImageSourceChanged != null )
            {
                ImageSourceChanged( this, e );
            }
        }


    }
}
