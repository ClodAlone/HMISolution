#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Syncfusion.Windows.Tools.Controls
{
	/// <summary>
	///
	/// </summary>
	public class SplitMenuButton : MenuButtonBase, ICommandSource
	{
		#region Private members
		Border m_ClickableArea; 
		#endregion 

		#region Dependency properties
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Key through which IsPressed property can be changed. 
		/// </summary>
		protected static readonly DependencyPropertyKey IsPressedPropertyKey = DependencyProperty.RegisterReadOnly( "IsPressed", typeof( bool ), typeof( SplitMenuButton ), new FrameworkPropertyMetadata( false ) );
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Identifies when button is pressed. 
		/// </summary>
		public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Using a DependencyProperty as the backing store for
		/// CanCommandExecute. This enables animation, styling, binding,
		/// etc...
		/// </summary>
		public static readonly DependencyProperty CanCommandExecuteProperty =
			DependencyProperty.Register("CanCommandExecute", typeof(bool), typeof(SplitMenuButton), new UIPropertyMetadata(false));

		#endregion

		#region Properties
		/// <property name="flag" value="Finished" />
		///
		/// <summary>
		/// Gets or sets the value of the IsPressed dependency property.
		/// </summary>
		protected bool IsPressed
		{
			get
			{
				return (bool)GetValue( IsPressedProperty );
			}
		}
		public bool CanCommandExecute
		{
			get
			{
				return (bool)GetValue(CanCommandExecuteProperty);
			}
			set
			{
				SetValue(CanCommandExecuteProperty, value);
			}
		}
		#endregion

		#region Initialization
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Default static constructor.
		/// </summary>
		static SplitMenuButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata( typeof( SplitMenuButton ), new FrameworkPropertyMetadata( typeof( SplitMenuButton ) ) );
		}
		#endregion

		#region Implementation
		private void SetIsPressed( bool pressed )
		{
			if( pressed )
			{
				base.SetValue( IsPressedPropertyKey, true );
			}
			else
			{
				base.ClearValue( IsPressedPropertyKey );
			}
		}
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Invoked when Click event is raised. 
		/// </summary>
		protected virtual void OnClick()
		{			
			if( Command != null && CanCommandExecute )
				Command.Execute( CommandParameter );
		}
		#endregion		
		#region Overrides
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			m_ClickableArea = (Border)GetTemplateChild("PART_OutterBorder");
			if (m_ClickableArea == null)
				throw new NullReferenceException("Template key part could not be found");
		}
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Invoked when an unhandled MouseLeftButtonDown routed event is
		/// raised on this element. Implement this method to add class
		/// handling for this event.
		/// </summary>
		/// <param name="e">The MouseButtonEventArgs that contains the
		/// event data. The event data reports that the
		/// left mouse button was pressed.</param>
		protected override void OnMouseLeftButtonDown( MouseButtonEventArgs e )
		{
			e.Handled = true;
			base.Focus();
			if (m_ClickableArea.IsMouseOver)
			{
				if (e.ButtonState == MouseButtonState.Pressed)
				{
					m_ClickableArea.CaptureMouse();
					if (m_ClickableArea.IsMouseCaptured)
					{
						if (e.ButtonState == MouseButtonState.Pressed)
						{
							if (!this.IsPressed)
							{
								this.SetIsPressed(true);
							}
						}
						else
						{
							m_ClickableArea.ReleaseMouseCapture();
						}
					}
				}
			}
			base.OnMouseLeftButtonDown( e );
		}
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Raises the MouseMove event.
		/// </summary>
		/// <param name="e">A MouseEventArgs that contains the event
		/// data.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (m_ClickableArea.IsMouseCaptured && (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed))
			{
				Point position = Mouse.PrimaryDevice.GetPosition(this);
				if (((position.X >= 0) && (position.X <= m_ClickableArea.ActualWidth)) && ((position.Y >= 0) && (position.Y <= m_ClickableArea.ActualHeight)))
				{
					if (!this.IsPressed)
					{
						this.SetIsPressed(true);
					}
				}
				else if (this.IsPressed)
				{
					this.SetIsPressed(false);
				}

				e.Handled = true;
			}

		}		
		//protected override void OnMouseLeave( MouseEventArgs e )
		//{
		//    base.OnMouseLeave(e);
		//    SetIsPressed(false);
		//    e.Handled = true;
		//}

        /// <property name="flag" value="Finished" />
        /// 
        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is
        /// raised on this element. Implement this method to add class
        /// handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the
        /// event data. The event data reports that the
        /// left mouse button was pressed.</param>
		protected override void OnMouseLeftButtonUp( MouseButtonEventArgs e )
		{
			e.Handled = true;			
			
			if (m_ClickableArea.IsMouseCaptured)
				m_ClickableArea.ReleaseMouseCapture();			

			if (IsPressed)
			{
				OnClick();
			}

			base.OnMouseLeftButtonUp( e );

		}
		/// <property name="flag" value="Finished" />
		///
		/// <summary>
		/// Creates AutomationPeer for ribbon button.
		/// </summary>
		/// <returns>
		/// An appropriate SplitMenuButtonAutomationPeer for this control as
		/// part of the WPF infrastructure.
		/// </returns>
		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new SplitMenuButtonAutomationPeer(this);

		}		
		#endregion
			
		#region ICommand Interface Memembers
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Make Command a dependency property so it can be DataBound.
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register( "Command", typeof( ICommand ), typeof( SplitMenuButton ), new PropertyMetadata( (ICommand)null, new PropertyChangedCallback( CommandChanged ) ) );

		public ICommand Command
		{
			get
			{
				return (ICommand)GetValue( CommandProperty );
			}
			set
			{
				SetValue( CommandProperty, value );
			}
		}

		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Make CommandTarget a dependency property so it can be
		/// DataBound.
		/// </summary>
		public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register( "CommandTarget", typeof( IInputElement ), typeof( SplitMenuButton ), new PropertyMetadata( (IInputElement)null ) );

		public IInputElement CommandTarget
		{
			get
			{
				return (IInputElement)GetValue( CommandTargetProperty );
			}
			set
			{
				SetValue( CommandTargetProperty, value );
			}
		}

		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Make CommandParameter a dependency property so it can be
		/// DataBound.
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register( "CommandParameter", typeof( object ), typeof( SplitMenuButton ), new PropertyMetadata( (object)null ) );

		public object CommandParameter
		{
			get
			{
				return (object)GetValue( CommandParameterProperty );
			}
			set
			{
				SetValue( CommandParameterProperty, value );
			}
		}

		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Command dependency property change callback.
		/// </summary>
		private static void CommandChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			SplitMenuButton instance = (SplitMenuButton)d;
			instance.HookUpCommand( (ICommand)e.OldValue, (ICommand)e.NewValue );
		}
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Adds a new command to the Command Property.
		/// </summary>
		private void HookUpCommand( ICommand oldCommand, ICommand newCommand )
		{
			//if oldCommand is not null, then we need to remove the handlers
			if( oldCommand != null )
			{
				RemoveCommand( oldCommand, newCommand );
			}
			AddCommand( oldCommand, newCommand );
		}

		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Removes an old command from the Command Property.
		/// </summary>
		private void RemoveCommand( ICommand oldCommand, ICommand newCommand )
		{
			EventHandler handler = CanExecuteChanged;
			oldCommand.CanExecuteChanged -= handler;
		}

		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Adds the command.
		/// </summary>
		private void AddCommand( ICommand oldCommand, ICommand newCommand )
		{
			EventHandler handler = new EventHandler( CanExecuteChanged );
			canExecuteChangedHandler = handler;
			if( newCommand != null )
			{
				newCommand.CanExecuteChanged += canExecuteChangedHandler;
			}
		}
		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Invoked when command CanExecute is changed.
		/// </summary>
		/// <param name="sender">Element which raised the event. </param>
		/// <param name="e">The instance containing the event data.
		/// </param>
		private void CanExecuteChanged( object sender, EventArgs e )
		{
			if( this.Command != null )
			{
				RoutedCommand command = this.Command as RoutedCommand;

				// if RoutedCommand
				//if( command != null )
				//{
				//    if( command.CanExecute( CommandParameter, CommandTarget ) )
				//    {
				//        this.IsEnabled = true;
				//    }
				//    else
				//    {
				//        this.IsEnabled = false;
				//    }
				//}
				//// if not RoutedCommand
				//else
				//{
				//    if( Command.CanExecute( CommandParameter ) )
				//    {
				//        this.IsEnabled = true;
				//    }
				//    else
				//    {
				//        this.IsEnabled = false;
				//    }
				//}
			}
		}

		/// <property name="flag" value="Finished" />
		/// 
		/// <summary>
		/// Keep a copy of the handler so that the garbage collector
		/// would not get it.
		/// </summary>
		private static EventHandler canExecuteChangedHandler;
		#endregion
	
	}

	#region UI Automation support
	/// <property name="flag" value="Finished" />
	///
	/// <summary>
	/// Class that provides UI Automation support.
	/// </summary>
	public class SplitMenuButtonAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
	{
		/// <property name="flag" value="Finished" />
		///
		/// <summary>
		/// Public constructor. Initializes AutomationPeer for ribbon
		/// button.
		/// </summary>
		public SplitMenuButtonAutomationPeer(SplitMenuButton control)
			: base(control)
		{
		}
		/// <property name="flag" value="Finished" />
		///
		/// <summary>
		/// Returns SplitMenuButton class name.
		/// </summary>
		protected override string GetClassNameCore()
		{
			return "SplitMenuButton";
		}
		/// <property name="flag" value="Finished" />
		///
		/// <summary>
		/// Gets the localized version of the control type for the
		/// SplitMenuButton.
		/// </summary>
		protected override string GetLocalizedControlTypeCore()
		{
			return "button";
		}
		/// <property name="flag" value="Finished" />
		///
		/// <summary>
		/// Gets the control type for the ribbon button that is
		/// associated with this SplitMenuButtonAutomationPeer.
		/// </summary>
		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Button;
		}

		/// <property name="flag" value="Finished" />
		///
		/// <summary>
		/// Gets the control pattern for the SplitMenuButton that is
		/// associated with this SplitMenuButtonAutomationPeer.
		/// </summary>
		public override object GetPattern(PatternInterface patternInterface)
		{
			if (patternInterface == PatternInterface.Invoke)
			{
				return this;
			}

			return base.GetPattern(patternInterface);
		}
		/// <property name="flag" value="Finished" />
		///
		/// <summary>
		/// Gets ribbon button object.
		/// </summary>
		private SplitMenuButton MyOwner
		{
			get
			{
				return (SplitMenuButton)base.Owner;
			}
		}

		#region IInvokeProvider Members
		/// <property name="flag" value="Finished" />
		///
		/// <summary>
		/// Invokes SplitMenuButton Click event.
		/// </summary>
		public void Invoke()
		{
			RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
			MyOwner.RaiseEvent(newEventArgs);
			if (MyOwner.Command != null)
				MyOwner.Command.Execute(MyOwner.CommandParameter);
		}
		#endregion

	}
	#endregion
}
