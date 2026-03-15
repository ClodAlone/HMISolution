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
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Syncfusion.Design.Controls
{
    public class SmartTagPresenter : ContentControl
    {
        #region Private Members        
        /// <summary>
        /// 
        /// </summary>
        private Button m_closeButton;
        /// <summary>
        /// 
        /// </summary>
        private Popup m_popup;
        /// <summary>
        /// 
        /// </summary>
        private bool m_skipDropDownOpen = false;       
        #endregion

        #region Initialization
        /// <summary>
        /// 
        /// </summary>
        static SmartTagPresenter( )
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata( typeof( SmartTagPresenter ), new FrameworkPropertyMetadata( typeof( SmartTagPresenter ) ) );
        }
        /// <summary>
        /// 
        /// </summary>
        public SmartTagPresenter( )
        {
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler( this, OnPreviewMouseDownOutsideCapturedElement );            
        }
        #endregion

        #region DP getters & setters
        /// <summary>
        /// Gets or sets the value of the Title dependency property.
        /// </summary>
        public string Title
        {
            get
            {
                return (string)GetValue( TitleProperty );
            }
            set
            {
                SetValue( TitleProperty, value );
            }
        }
        /// <summary>
        /// Gets or sets the value of the IsDropDownOpen dependency property.
        /// </summary>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)GetValue( IsDropDownOpenProperty );
            }
            set
            {
                SetValue( IsDropDownOpenProperty, value );
            }
        }
        #endregion

        #region Override methods
        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate( )
        {
            base.OnApplyTemplate( );

            m_closeButton = ( Button )GetTemplateChild( "PART_CloseButton" );

            if( m_closeButton != null )
            {
                m_closeButton.Click += new RoutedEventHandler( CloseButton_Click );
            }

            m_popup = (Popup)GetTemplateChild( "PART_Popup" );

            if( m_popup != null )
            {
                m_popup.MouseDown += new System.Windows.Input.MouseButtonEventHandler( Popup_MouseDown );
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown( MouseButtonEventArgs e )
        {
            if( m_skipDropDownOpen )
            {
                e.Handled = true;
                m_skipDropDownOpen = false;
                return;
            }
            
            base.OnPreviewMouseDown( e );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter( MouseEventArgs e )
        {
            m_skipDropDownOpen = false;
            base.OnMouseEnter( e );
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnTitleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTitleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            SmartTagPresenter instance = (SmartTagPresenter)d;
            instance.OnTitleChanged( e );
        }
        /// <summary>
        /// Updates property value cache and raises TitleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTitleChanged( DependencyPropertyChangedEventArgs e )
        {
            if( TitleChanged != null )
            {
                TitleChanged( this, e );
            }
        }
        /// <summary>
        /// Calls OnIsDropDownOpenChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsDropDownOpenChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            SmartTagPresenter instance = (SmartTagPresenter)d;
            instance.OnIsDropDownOpenChanged( e );
        }
        /// <summary>
        /// Updates property value cache and raises IsDropDownOpenChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsDropDownOpenChanged( DependencyPropertyChangedEventArgs e )
        {
            if( IsDropDownOpen == true )
            {
                Mouse.Capture( this, CaptureMode.SubTree );
                //Keyboard.Focus( this );
            }
            else
            {
                Mouse.Capture( null );
            }

            if( IsDropDownOpenChanged != null )
            {
                IsDropDownOpenChanged( this, e );
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click( object sender, RoutedEventArgs e )
        {
            IsDropDownOpen = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Popup_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            e.Handled = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseDownOutsideCapturedElement( object sender, MouseButtonEventArgs e )
        {
            IsDropDownOpen = false;
            m_skipDropDownOpen = true;
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Title property is changed.
        /// </summary>
        public event PropertyChangedCallback TitleChanged;
        /// <summary>
        /// Event that is raised when IsDropDownOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDropDownOpenChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register( "Title", typeof( string ), typeof( SmartTagPresenter ), new FrameworkPropertyMetadata( string.Empty, new PropertyChangedCallback( OnTitleChanged ) ) );
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register( "IsDropDownOpen", typeof( bool ), typeof( SmartTagPresenter ), new FrameworkPropertyMetadata( false, new PropertyChangedCallback( OnIsDropDownOpenChanged ) ) );
        #endregion
    }
}
