#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
/// <summary>
/// Description for BaseControlEditControlConfigurator.
/// </summary>
[ToolboxItem( false )]
public class BaseControlEditControlConfigurator
: System.Windows.Forms.UserControl
{
#region Class Private Members
/// <summary>
/// EditControl to be used for getting supported languages list.
/// </summary>
private EditControl m_editControl;

/// <summary>
/// Currently used configuration.
/// </summary>
private Config m_config;
#endregion

#region Class Properties
/// <summary>
/// Gets or sets edit control used as a source for languages list.
/// </summary>
[Browsable( true )]
[TypeConverter( typeof( ComponentConverter ) )]
[Category( "Data" )]
public EditControl EditControl
{
get
{
return m_editControl;
}
set
{
if ( m_editControl != value )
{
DetachEvents();
m_editControl = value;
AtachEvents();

OnEditControlChanged();
}
}
}

/// <summary>
/// Gets or sets currently used configuration. If EditControl is not null, it's configuration is used.
/// </summary>
[Browsable( false )]
public Config Configuration
{
get
{
if ( EditControl != null )
return EditControl.Configurator;

return m_config;
}
set
{
if ( m_editControl != null ) return;

if ( m_config != value )
{
DetachEvents();
m_config = value;
AtachEvents();

OnEditControlChanged();
}
}
}
#endregion

#region Class Events
/// <summary>
/// Event that is raised when EditControl property has been changed.
/// </summary>
public event EventHandler EditControlChanged;
#endregion

#region Class Initialization/Finalization
/// <summary> 
/// Initializes a new instance of the BaseControlEditControlConfigurator class.
/// </summary>
public BaseControlEditControlConfigurator()
{
}
#endregion

#region Class Helper Methods
/// <summary>
/// Detaches configurationchanged event handler.
/// </summary>
private void DetachEvents()
{
if ( null != m_editControl )
m_editControl.ConfigurationChanged -= new EventHandler( EditControl_ConfigurationChanged );
else if ( m_config != null )
m_config.ConfigurationChanged -= new EventHandler( EditControl_ConfigurationChanged );
}

/// <summary>
/// Ataches configurationchanged event handler.
/// </summary>
private void AtachEvents()
{
if ( null != m_editControl )
m_editControl.ConfigurationChanged += new EventHandler( EditControl_ConfigurationChanged );
else if ( null != m_config )
m_config.ConfigurationChanged += new EventHandler( EditControl_ConfigurationChanged );
}

/// <summary>
/// Calls OnConfigurationChanged() method.
/// </summary>
/// <param name="sender">The sender</param>
/// <param name="e">The event argument</param>
private void EditControl_ConfigurationChanged( object sender, EventArgs e )
{
OnConfigurationChanged();
}
#endregion

#region Class Virtual Methods
/// <summary>
/// Raises EditControlChanged event.
/// </summary>
protected virtual void OnEditControlChanged()
{
if ( EditControlChanged != null )
{
EditControlChanged( this, EventArgs.Empty );
}
}

/// <summary>
/// Called when configuration is changed.
/// </summary>
protected virtual void OnConfigurationChanged()
{
}
#endregion
}
}
