#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
	public delegate void SettingsChangedEventHandler();

	public class SystemInfo
	{
		static SystemInfo()
		{
			SettingsChanged = null;

			SystemEvents.DisplaySettingsChanged += new EventHandler( DisplaySettingsChangedHandler );
			SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler( UserPreferenceChangedHandler );

			Update();
		}

		private static void DisplaySettingsChangedHandler(Object sender,EventArgs e)
		{
			Update();
			OnSettingsChanged();
		}
		
		private static void UserPreferenceChangedHandler( Object sender, UserPreferenceChangedEventArgs e )
		{
			Update();
			OnSettingsChanged();
		}

		static protected void OnSettingsChanged()
		{
			if ( SettingsChanged != null )
			{
				SettingsChanged();
			}
		}

		static private void Update()
		{
			//m_sBitsPerPixel = ( short )Screen.PrimaryScreen.BitsPerPixel;
			m_bIsVisualStyleEnabled = !SystemInformation.HighContrast /*&& m_sBitsPerPixel > 8*/;
		}

		static public bool IsVisualStyleEnabled
		{
			get
			{
				return m_bIsVisualStyleEnabled;
			}
		}

		static private bool m_bIsVisualStyleEnabled;
		//static private short m_sBitsPerPixel;

		static public event SettingsChangedEventHandler SettingsChanged;
	}
}
#endif